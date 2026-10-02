using Microsoft.JSInterop;

namespace PKHeX.Web.Interop;

/// <summary>
/// Page helpers the editor needs from the browser: waiting for a paint before synchronous work, and moving focus to a section by id.
/// </summary>
/// <remarks>
/// The JS side is in <c>wwwroot/browser.js</c> (see <see cref="BrowserModule"/>). That module is already imported when the drop zone
/// registers at startup, so importing it here reuses the loaded module and makes no request.
/// </remarks>
public sealed class BrowserPage(IJSRuntime js) : IAsyncDisposable
{
    private readonly BrowserModule browser = new(js);

    /// <summary>Completes once the browser has painted (or after a short timeout in a hidden tab).</summary>
    public async Task NextPaintAsync()
    {
        var module = await browser.GetAsync();
        await module.InvokeVoidAsync("nextPaint");
    }

    /// <summary>Scrolls to the element with id <paramref name="id"/> and focuses it; false when the page has no such element.</summary>
    public async Task<bool> FocusAsync(string id)
    {
        var module = await browser.GetAsync();
        return await module.InvokeAsync<bool>("focusElement", id);
    }

    /// <summary>Releases the imported module.</summary>
    public ValueTask DisposeAsync() => browser.DisposeAsync();
}
