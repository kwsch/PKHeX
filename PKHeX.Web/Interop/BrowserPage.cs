using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PKHeX.Web.Interop;

/// <summary>
/// Page helpers the app needs from the browser: waiting for a paint before synchronous work, showing a modal dialog, moving focus to a section by id (on a narrow screen only, when a pane change asks), and the
/// diagnostic report's user agent, copy and select.
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

    /// <summary>
    /// Scrolls to and focuses the element with id <paramref name="id"/>, but only while the page shows one pane at a time
    /// (<see cref="Components.WorkspaceLayout.NarrowQuery"/>); false when it does not, or there is no such element.
    /// </summary>
    public async Task<bool> FocusIfNarrowAsync(string id)
    {
        var module = await browser.GetAsync();
        return await module.InvokeAsync<bool>("focusIfNarrow", id);
    }

    /// <summary>
    /// Focuses the element with id <paramref name="id"/> only when focus has been lost to the page body (the element that had it was removed);
    /// false when focus was elsewhere, or there is no such element.
    /// </summary>
    public async Task<bool> FocusIfLostAsync(string id)
    {
        var module = await browser.GetAsync();
        return await module.InvokeAsync<bool>("focusIfLost", id);
    }

    /// <summary>
    /// Shows the <c>dialog</c> element <paramref name="dialog"/> as a modal, so the page behind it is inert and focus stays inside it; does
    /// nothing when it is already open.
    /// </summary>
    public async Task ShowModalAsync(ElementReference dialog)
    {
        var module = await browser.GetAsync();
        await module.InvokeVoidAsync("showModal", dialog);
    }

    /// <summary>The browser's user agent string. Read only when the user prepares a diagnostic report.</summary>
    public async Task<string> GetUserAgentAsync()
    {
        var module = await browser.GetAsync();
        return await module.InvokeAsync<string>("userAgent");
    }

    /// <summary>Copies <paramref name="text"/> to the clipboard; false when the browser refused.</summary>
    public async Task<bool> CopyTextAsync(string text)
    {
        var module = await browser.GetAsync();
        return await module.InvokeAsync<bool>("copyText", text);
    }

    /// <summary>Selects the text of the element with id <paramref name="id"/>, so it can be copied by hand; false when there is no such element.</summary>
    public async Task<bool> SelectTextAsync(string id)
    {
        var module = await browser.GetAsync();
        return await module.InvokeAsync<bool>("selectText", id);
    }

    /// <summary>Releases the imported module.</summary>
    public ValueTask DisposeAsync() => browser.DisposeAsync();
}
