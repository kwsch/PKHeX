using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using PKHeX.Web.Services;

namespace PKHeX.Web.Interop;

/// <summary>
/// Browser file operations: bounded reads of a chosen file, file-only drop zones and Blob downloads.
/// </summary>
/// <remarks>
/// Everything stays in the browser: files are read through Blazor's <see cref="IBrowserFile"/> and downloads are Blob URLs, so no file
/// content or name is sent anywhere. The JS side is <c>wwwroot/browser.js</c>, imported on first use.
/// </remarks>
public sealed class BrowserFileService(IJSRuntime js) : IAsyncDisposable
{
    /// <summary>First buffer size when a stream sends more than its declared length (or none was declared); the buffer doubles from there.</summary>
    private const int ChunkSize = 81920;

    private readonly BrowserModule browser = new(js);

    /// <summary>
    /// Reads <paramref name="file"/> into memory, enforcing <see cref="SaveLoader.MaxInputBytes"/> before and while streaming.
    /// </summary>
    /// <param name="file">File from an <see cref="InputFile"/> change event.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>
    /// The sanitised name and, when <see cref="FileReadStatus.Ok"/>, the bytes. Any read failure, including running out of memory
    /// for the copy, is returned as <see cref="FileReadStatus.ReadFailed"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public async Task<FileReadResult> ReadAsync(IBrowserFile file, CancellationToken cancellationToken = default)
    {
        var name = FileNaming.Sanitize(file.Name);
        if (file.Size == 0)
        {
            return FileReadResult.Failed(FileReadStatus.Empty, name);
        }
        if (file.Size > SaveLoader.MaxInputBytes)
        {
            return FileReadResult.Failed(FileReadStatus.TooLarge, name);
        }

        try
        {
            // The browser's declared size is not trusted on its own: ReadBoundedAsync counts the bytes it actually receives.
            await using var stream = file.OpenReadStream(SaveLoader.MaxInputBytes, cancellationToken);
            return await ReadBoundedAsync(stream, name, SaveLoader.MaxInputBytes, cancellationToken, file.Size);
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            // Browser file streams fail in several ways (IO, JS, length checks when the file changed); none may escape to the page.
            return FileReadResult.Failed(FileReadStatus.ReadFailed, name);
        }
    }

    /// <summary>
    /// Copies <paramref name="stream"/> into memory, stopping as soon as more than <paramref name="maxBytes"/> bytes have arrived.
    /// </summary>
    /// <remarks>
    /// The bytes are read straight into one array sized from <paramref name="declaredLength"/>, which is handed over as it is, so a file the
    /// browser sized correctly is held once. WebAssembly memory never shrinks, so every extra copy of a large file would raise the page's memory
    /// for the rest of the session. The declared length is only a starting size: the limit is enforced on the bytes actually received, and
    /// the array grows (or is trimmed) when the stream sends more (or fewer) bytes than declared.
    /// </remarks>
    /// <param name="stream">Source stream; it is not disposed.</param>
    /// <param name="fileName">Sanitised name to carry into the result.</param>
    /// <param name="maxBytes">Largest accepted length.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <param name="declaredLength">The length the browser reported for the file, or 0 when unknown.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    internal static async Task<FileReadResult> ReadBoundedAsync(Stream stream, string fileName, int maxBytes, CancellationToken cancellationToken = default, long declaredLength = 0)
    {
        try
        {
            var buffer = new byte[(int)Math.Clamp(declaredLength, 0, maxBytes)];
            var length = 0;
            while (true)
            {
                if (length == buffer.Length)
                {
                    if (length == maxBytes)
                    {
                        // The buffer holds the limit: one more byte means the file is too large.
                        var probe = new byte[1];
                        if (await stream.ReadAsync(probe, cancellationToken) > 0)
                        {
                            return FileReadResult.Failed(FileReadStatus.TooLarge, fileName);
                        }
                        break;
                    }
                    Array.Resize(ref buffer, (int)Math.Min(Math.Max(2L * buffer.Length, ChunkSize), maxBytes));
                }
                var read = await stream.ReadAsync(buffer.AsMemory(length), cancellationToken);
                if (read == 0)
                {
                    break;
                }
                length += read;
            }
            if (length == 0)
            {
                return FileReadResult.Failed(FileReadStatus.Empty, fileName);
            }
            if (length != buffer.Length)
            {
                Array.Resize(ref buffer, length);
            }
            return new FileReadResult(FileReadStatus.Ok, fileName, buffer);
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return FileReadResult.Failed(FileReadStatus.ReadFailed, fileName);
        }
    }

    /// <summary>
    /// Makes <paramref name="zone"/> accept a single dropped file and pass it to <paramref name="input"/>, so drops use the same read path as the picker.
    /// </summary>
    /// <remarks>
    /// Multiple files, folders and text or link drops are refused and reported through <paramref name="onRejected"/>, as is a drop while
    /// <paramref name="input"/> is disabled. Drops outside every zone are blocked by <c>drop-guard.js</c>, which <c>index.html</c> loads before the app.
    /// </remarks>
    /// <param name="zone">Element that receives drops.</param>
    /// <param name="input">The <c>&lt;input type="file"&gt;</c> rendered by an <see cref="InputFile"/>.</param>
    /// <param name="onRejected">Called for each refused drop.</param>
    /// <returns>Unregisters the zone when disposed.</returns>
    public async Task<IAsyncDisposable> RegisterDropZoneAsync(ElementReference zone, ElementReference input, Func<DropRejection, Task> onRejected)
    {
        var callbacks = DotNetObjectReference.Create(new DropCallbacks(onRejected));
        try
        {
            var module = await browser.GetAsync();
            var registration = await module.InvokeAsync<IJSObjectReference>("registerDropZone", zone, input, callbacks);
            return new DropZoneRegistration(registration, callbacks);
        }
        catch
        {
            callbacks.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Starts a browser download of <paramref name="data"/> as a binary file.
    /// </summary>
    /// <remarks>
    /// Starting the download does not mean the user kept the file; the UI must say so. The name is only a suggestion: browsers may
    /// still adjust it when saving (for example collapsing spaces or renaming extensions they consider unsafe).
    /// </remarks>
    /// <param name="data">File contents.</param>
    /// <param name="fileName">Suggested name; it is passed through <see cref="FileNaming.Sanitize"/>.</param>
    public async Task DownloadAsync(byte[] data, string fileName)
    {
        var module = await browser.GetAsync();
        using var stream = new MemoryStream(data, writable: false);
        using var reference = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("download", reference, FileNaming.Sanitize(fileName));
    }

    /// <summary>Releases the imported module.</summary>
    public ValueTask DisposeAsync() => browser.DisposeAsync();

    /// <summary>Receives drop rejections from <c>browser.js</c>.</summary>
    private sealed class DropCallbacks(Func<DropRejection, Task> onRejected)
    {
        [JSInvokable]
        public Task OnRejected(string code) => onRejected(code switch
        {
            "multiple" => DropRejection.MultipleFiles,
            "directory" => DropRejection.Directory,
            "busy" => DropRejection.Busy,
            _ => DropRejection.NotAFile,
        });
    }

    /// <summary>Removes the zone's listeners and releases the callback object.</summary>
    private sealed class DropZoneRegistration(IJSObjectReference registration, IDisposable callbacks) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            try
            {
                await registration.InvokeVoidAsync("dispose");
                await registration.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // The page is going away; its listeners go with it.
            }
            finally
            {
                callbacks.Dispose();
            }
        }
    }
}
