// Loaded by index.html as a classic script, so it is fetched with the app and never after a save is opened.
// Inside a slot grid ([role="grid"]) the app moves focus with the arrow keys, Home and End; this stops the same keys from also
// scrolling the page. Every other key, Tab included, keeps its default.
(function () {
    var navigationKeys = ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];

    document.addEventListener('keydown', function (event) {
        if (event.defaultPrevented || event.altKey || event.metaKey || navigationKeys.indexOf(event.key) < 0) {
            return;
        }
        if (event.target instanceof Element && event.target.closest('[role="grid"]') !== null) {
            event.preventDefault();
        }
    });
})();
