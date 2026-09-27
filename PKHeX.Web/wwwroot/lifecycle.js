// Page lifecycle hooks for the temporary in-memory session.
let dirty = false;
function warn(event) {
    if (!dirty) {
        return;
    }
    event.preventDefault();
    event.returnValue = '';
}
window.addEventListener('beforeunload', warn);
window.addEventListener('pageshow', event => {
    if (event.persisted) {
        window.location.reload();
    }
});
export function setDirty(value) { dirty = value; }
