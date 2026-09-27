let dirty = false;
function warn(event) {
    if (!dirty) return;
    event.preventDefault();
    event.returnValue = '';
}
window.addEventListener('beforeunload', warn);
window.addEventListener('pageshow', event => {
    if (event.persisted) window.location.reload();
});
export function setDirty(value) { dirty = value; }
export async function download(reference) {
    const data = await reference.arrayBuffer();
    const url = URL.createObjectURL(new Blob([data], { type: 'application/octet-stream' }));
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = 'main';
    anchor.click();
    // Permit browsers to consume the URL before releasing the generated snapshot.
    setTimeout(() => URL.revokeObjectURL(url), 30000);
}
