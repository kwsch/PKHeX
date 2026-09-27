// Loaded by index.html as a classic script ahead of the Blazor boot script, so a drop anywhere on the page can never open a file
// or navigate to a link, even while the WebAssembly runtime is still loading. Drop zones (browser.js) stop propagation and handle
// their own drops.
(() => {
    // Controls whose default drop action is inserting text. Every other target, including checkboxes and buttons, is guarded.
    const textInputTypes = new Set(['text', 'search', 'email', 'url', 'tel', 'password', 'number']);

    function acceptsTextDrop(target) {
        if (!(target instanceof Element)) {
            return false;
        }
        if (target.isContentEditable) {
            return true;
        }
        if (target instanceof HTMLTextAreaElement) {
            return !target.disabled && !target.readOnly;
        }
        if (target instanceof HTMLInputElement) {
            return textInputTypes.has(target.type) && !target.disabled && !target.readOnly;
        }
        return false;
    }

    function guard(event) {
        const files = event.dataTransfer !== null && Array.from(event.dataTransfer.types).includes('Files');
        if (files || !acceptsTextDrop(event.target)) {
            event.preventDefault();
            if (event.dataTransfer) {
                event.dataTransfer.dropEffect = 'none';
            }
        }
    }

    window.addEventListener('dragover', guard);
    window.addEventListener('drop', guard);
})();
