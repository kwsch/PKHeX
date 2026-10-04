// Browser file interop: file-only drop zones and Blob downloads. Files are only ever read locally; nothing is fetched or uploaded.
// The page-wide navigation guard is drop-guard.js, loaded by index.html before the app.
// Also preloads the sprite atlas at startup, in builds that include it, and has page helpers (next paint, focus a section, show a modal) and the
// diagnostic report's helpers (user agent, copy, select).

// The preloaded atlas, kept referenced for the page's lifetime. Sprites are <img> elements with this same URL, which browsers serve from
// the document's list of already loaded images, so drawing a sprite never makes a request.
let spriteAtlas = null;

/**
 * Classifies a drop. Returns 'ok' for exactly one file, otherwise 'multiple', 'directory' or 'not-a-file'.
 * String items next to a single file are allowed: file managers add a file:// uri-list, and nothing is fetched from it.
 */
export function classifyDrop(dataTransfer) {
    const files = Array.from(dataTransfer?.items ?? []).filter(item => item.kind === 'file');
    if (files.length === 0) {
        return 'not-a-file';
    }
    if (files.length > 1 || (dataTransfer.files?.length ?? 0) > 1) {
        return 'multiple';
    }
    // Entries are only available during the drop event. Files created in script have no entry, and are treated as files.
    const entry = typeof files[0].webkitGetAsEntry === 'function' ? files[0].webkitGetAsEntry() : null;
    if (entry?.isDirectory === true) {
        return 'directory';
    }
    return 'ok';
}

/**
 * Makes zone accept one dropped file and hand it to input (an <input type="file">) through a change event,
 * so a drop takes the same read path as the picker. Refusals are reported to callbacks.OnRejected(code).
 */
export function registerDropZone(zone, input, callbacks) {
    // dragenter/dragleave also fire when moving between the zone's own children, and WebKit gives no relatedTarget,
    // so count them: the drag has left the zone when every enter has been matched by a leave.
    let depth = 0;
    const clear = () => {
        depth = 0;
        zone.removeAttribute('data-drag-over');
    };
    const over = event => {
        event.preventDefault();
        event.stopPropagation();
        // Always accept the drag: a dropEffect of 'none' cancels the drop, so refused drops would never be explained.
        event.dataTransfer.dropEffect = 'copy';
        zone.setAttribute('data-drag-over', '');
    };
    const enter = event => {
        depth++;
        over(event);
    };
    const leave = () => {
        depth = Math.max(0, depth - 1);
        if (depth === 0) {
            clear();
        }
    };
    const drop = event => {
        event.preventDefault();
        event.stopPropagation();
        clear();
        const result = input.disabled ? 'busy' : classifyDrop(event.dataTransfer);
        if (result !== 'ok') {
            // Nothing awaits this; a failure (e.g. the component was just disposed) must not surface as an unhandled rejection.
            callbacks.invokeMethodAsync('OnRejected', result).catch(() => { });
            return;
        }
        input.files = event.dataTransfer.files;
        input.dispatchEvent(new Event('change', { bubbles: true }));
    };
    zone.addEventListener('dragenter', enter);
    zone.addEventListener('dragover', over);
    zone.addEventListener('dragleave', leave);
    zone.addEventListener('drop', drop);
    return {
        dispose() {
            zone.removeEventListener('dragenter', enter);
            zone.removeEventListener('dragover', over);
            zone.removeEventListener('dragleave', leave);
            zone.removeEventListener('drop', drop);
            clear();
        },
    };
}

/** Downloads the streamed bytes as a binary file named fileName. The browser may still adjust the name when saving. */
export async function download(reference, fileName) {
    const data = await reference.arrayBuffer();
    const url = URL.createObjectURL(new Blob([data], { type: 'application/octet-stream' }));
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName;
    anchor.rel = 'noopener';
    anchor.click();
    // Permit browsers to consume the URL before releasing the generated snapshot.
    setTimeout(() => URL.revokeObjectURL(url), 30000);
}

/**
 * Loads the sprite stylesheet and atlas image, resolving once both are ready to draw and rejecting if either fails.
 * Called once, before the app renders, so both requests happen before any file can be chosen and are the same for every save.
 */
export async function preloadSprites(stylesheetHref, atlasHref) {
    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = stylesheetHref;
    await new Promise((resolve, reject) => {
        link.addEventListener('load', resolve, { once: true });
        link.addEventListener('error', () => reject(new Error('The sprite stylesheet could not be loaded.')), { once: true });
        document.head.appendChild(link);
    });
    const image = new Image();
    image.src = atlasHref;
    await image.decode();
    spriteAtlas = image;
}

/**
 * Resolves once the browser has had a chance to paint, so a status set just before (such as "Pending") is on screen before the app starts
 * synchronous work. A hidden tab does not run animation frames, so a timeout resolves it as well.
 */
export function nextPaint() {
    return new Promise(resolve => {
        let done = false;
        const finish = () => {
            if (!done) {
                done = true;
                resolve();
            }
        };
        requestAnimationFrame(() => setTimeout(finish, 0));
        setTimeout(finish, 100);
    });
}

/** Scrolls to the element with the given id and focuses it. Returns false when there is no such element. */
export function focusElement(id) {
    const element = document.getElementById(id);
    if (!element) {
        return false;
    }
    element.scrollIntoView({ block: 'start' });
    element.focus({ preventScroll: true });
    return true;
}

// The width under which the page shows one pane at a time: the same text as app.css and WorkspaceLayout.NarrowQuery.
const narrowQuery = '(max-width: 39.99rem)';

// The width from which both panes are shown side by side: the same text as app.css.
const wideQuery = '(min-width: 75rem)';

// Headings that can take focus from script, in the order tried: the editor, the party and boxes, the open heading.
const fallbackFocus = ['draft-title', 'storage-title', 'open-title'];

/**
 * When a change of width hides the element that has focus (a pane the new layout does not show), moves focus to the first heading still
 * shown, rather than leaving it on a hidden element, where the next Tab would start from the page's top. Focus that stays visible is left alone.
 */
function keepFocusShown() {
    // Runs after the new layout applies.
    requestAnimationFrame(() => {
        const active = document.activeElement;
        if (!active || active === document.body || active.getClientRects().length > 0) {
            return;
        }
        const shown = fallbackFocus.map(id => document.getElementById(id)).find(e => e && e.getClientRects().length > 0);
        shown?.focus();
    });
}

for (const query of [narrowQuery, wideQuery]) {
    window.matchMedia(query).addEventListener('change', keepFocusShown);
}

/**
 * Focuses the element with the given id only when focus has been lost to the page body, as it is when the element that had it was removed
 * (an error summary whose reasons were all resolved). Focus anywhere else is left alone. Returns true when it moved focus.
 */
export function focusIfLost(id) {
    const active = document.activeElement;
    if (active && active !== document.body) {
        return false;
    }
    const element = document.getElementById(id);
    if (!element) {
        return false;
    }
    element.focus();
    return true;
}

/** Like focusElement, but only while the page shows one pane at a time; returns false otherwise. */
export function focusIfNarrow(id) {
    return window.matchMedia(narrowQuery).matches && focusElement(id);
}

/**
 * Shows a <dialog> as a modal: the page behind it becomes inert and focus stays inside it. Does nothing when it is already open.
 * Escape's cancel event is not allowed to close it: the app decides (it cancels the exit, which removes the dialog), so the dialog never
 * closes while the app still shows it. A close the browser forces anyway raises close, which the app handles too.
 */
export function showModal(dialog) {
    if (!dialog.pkhexCancelGuard) {
        dialog.pkhexCancelGuard = true;
        dialog.addEventListener('cancel', event => event.preventDefault());
    }
    if (!dialog.open) {
        dialog.showModal();
    }
}

/** The browser's user agent string, read only when the user prepares a diagnostic report. */
export function userAgent() {
    return navigator.userAgent;
}

/**
 * Copies text to the clipboard. Returns false when the browser refuses (no clipboard API outside a secure context, or permission denied),
 * so the caller can offer the text to copy by hand.
 */
export async function copyText(text) {
    try {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            return true;
        }
    } catch {
        // Refused; the caller selects the text instead.
    }
    return false;
}

/** Selects the text content of the element with the given id, so it can be copied by hand. Returns false when there is no such element. */
export function selectText(id) {
    const element = document.getElementById(id);
    if (!element) {
        return false;
    }
    const range = document.createRange();
    range.selectNodeContents(element);
    const selection = window.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);
    return true;
}
