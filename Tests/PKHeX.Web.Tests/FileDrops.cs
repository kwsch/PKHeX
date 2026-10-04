using Microsoft.Playwright;

namespace PKHeX.Web.Tests;

/// <summary>
/// Drops files on the published page with a script-built <c>DataTransfer</c>, since Playwright cannot drag files from the operating system.
/// </summary>
internal static class FileDrops
{
    /// <summary>
    /// Dispatches a drop on the first element matching <c>selector</c> and returns whether the page prevented the browser's default action.
    /// Each file is <c>{ name, data }</c> with base64 data; <c>text</c>, when set, is added as a <c>text/uri-list</c> item.
    /// </summary>
    private const string DropScript = """
        ({ selector, files, text }) => {
            const dataTransfer = new DataTransfer();
            for (const file of files) {
                const binary = atob(file.data);
                const bytes = new Uint8Array(binary.length);
                for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
                dataTransfer.items.add(new File([bytes], file.name, { type: 'application/octet-stream' }));
            }
            if (text) dataTransfer.setData('text/uri-list', text);
            const event = new DragEvent('drop', { dataTransfer, bubbles: true, cancelable: true });
            document.querySelector(selector).dispatchEvent(event);
            return event.defaultPrevented;
        }
        """;

    /// <summary>
    /// Drops <paramref name="files"/> (and <paramref name="text"/> as a link, when set) on the first element matching <paramref name="selector"/>,
    /// returning whether the page prevented the browser's default action.
    /// </summary>
    public static Task<bool> Drop(IPage page, string selector, (string Name, byte[] Data)[] files, string? text = null) =>
        page.EvaluateAsync<bool>(DropScript, new
        {
            selector,
            files = files.Select(f => new { name = f.Name, data = Convert.ToBase64String(f.Data) }).ToArray(),
            text,
        });
}
