// Page lifecycle hooks for the temporary in-memory session.
let dirty = false;

// Best-effort warning before the page is left while leaving would lose work. Mobile browsers may end a tab without any event.
function warn(event) {
    if (!dirty) {
        return;
    }
    event.preventDefault();
    event.returnValue = '';
}
window.addEventListener('beforeunload', warn);

// A page kept in the back-forward cache is hidden as it goes in, so a restored copy never shows or accepts input for the old session,
// even for the moment before the reload below replaces it.
window.addEventListener('pagehide', event => {
    if (event.persisted) {
        document.documentElement.setAttribute('data-session-ended', '');
    }
});

// A restored page still holds the old session in memory. It is cleared by loading the page afresh, which discards the runtime and every
// copy of the save. The user already chose to leave, so the reload must not ask again.
window.addEventListener('pageshow', event => {
    if (event.persisted) {
        dirty = false;
        window.location.reload();
    }
});

export function setDirty(value) { dirty = value; }
