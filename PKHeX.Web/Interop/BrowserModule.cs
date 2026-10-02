using Microsoft.JSInterop;

namespace PKHeX.Web.Interop;

/// <summary>
/// The app's <c>wwwroot/browser.js</c> module, imported on first use. Every service that needs it holds its own instance; the browser keeps one
/// copy of the module per page, so later imports reuse it and make no request.
/// </summary>
internal sealed class BrowserModule(IJSRuntime js) : IAsyncDisposable
{
    private Task<IJSObjectReference>? module;

    /// <summary>Imports <c>browser.js</c> once. A failed import is not kept, so the next call tries again.</summary>
    public async Task<IJSObjectReference> GetAsync()
    {
        var task = module ??= js.InvokeAsync<IJSObjectReference>("import", "./browser.js").AsTask();
        try
        {
            return await task;
        }
        catch
        {
            if (ReferenceEquals(module, task))
            {
                module = null;
            }
            throw;
        }
    }

    /// <summary>Releases the module reference, if it was imported.</summary>
    public async ValueTask DisposeAsync()
    {
        if (module is { IsCompletedSuccessfully: true })
        {
            try
            {
                await module.Result.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // The page is going away; there is nothing left to release.
            }
        }
    }
}
