using FluentAssertions;
using Microsoft.AspNetCore.Components.Forms;
using PKHeX.Web.Interop;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The bounded read behind <see cref="BrowserFileService.ReadAsync"/>, and the file name a session keeps.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class BrowserFileServiceTests
{
    private const int Limit = 200_000; // Larger than one copy chunk, so the limit is checked across chunks.

    [Fact]
    public async Task AcceptsExactlyTheLimit()
    {
        var data = Enumerable.Range(0, Limit).Select(i => (byte)i).ToArray();
        var result = await BrowserFileService.ReadBoundedAsync(new MemoryStream(data), "main", Limit);
        result.Status.Should().Be(FileReadStatus.Ok);
        result.FileName.Should().Be("main");
        result.Bytes.Should().Equal(data);
    }

    [Fact]
    public async Task StopsWhenStreamedBytesExceedTheLimit()
    {
        // The stream is not asked for its length: only the bytes received count, whatever size the browser declared.
        var stream = new UnseekableStream(new byte[Limit * 4]);
        var result = await BrowserFileService.ReadBoundedAsync(stream, "main", Limit);
        result.Status.Should().Be(FileReadStatus.TooLarge);
        result.Bytes.Should().BeEmpty();
        stream.Position.Should().BeLessThan(Limit * 2, "reading stops at the first chunk past the limit");
    }

    [Fact]
    public async Task RejectsOnDeclaredSizeWithoutOpening()
    {
        var service = new BrowserFileService(null!);
        var empty = new FakeFile("a\u0007.sav", 0);
        var result = await service.ReadAsync(empty);
        result.Status.Should().Be(FileReadStatus.Empty);
        result.FileName.Should().Be("a.sav");
        empty.Opened.Should().BeFalse();

        var large = new FakeFile("main", SaveLoader.MaxInputBytes + 1);
        (await service.ReadAsync(large)).Status.Should().Be(FileReadStatus.TooLarge);
        large.Opened.Should().BeFalse();
    }

    [Fact]
    public async Task ReadsDeclaredFileAndContainsAnyFailure()
    {
        var service = new BrowserFileService(null!);
        var ok = await service.ReadAsync(new FakeFile("main", 3, () => new MemoryStream([1, 2, 3])));
        ok.Status.Should().Be(FileReadStatus.Ok);
        ok.Bytes.Should().Equal(1, 2, 3);

        // Browser file streams also throw non-IO exceptions, for example when the file changed after it was chosen.
        var failed = await service.ReadAsync(new FakeFile("main", 3, () => throw new InvalidOperationException()));
        failed.Status.Should().Be(FileReadStatus.ReadFailed);
    }

    [Fact]
    public async Task CancellationIsNotReportedAsReadFailure()
    {
        using var cancel = new CancellationTokenSource();
        await cancel.CancelAsync();
        var act = () => BrowserFileService.ReadBoundedAsync(new MemoryStream([1]), "main", Limit, cancel.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();

        var service = new BrowserFileService(null!);
        var file = new FakeFile("main", 1, () => throw new OperationCanceledException(cancel.Token));
        var read = () => service.ReadAsync(file, cancel.Token);
        await read.Should().ThrowAsync<OperationCanceledException>();
    }

    [Theory]
    [InlineData(Limit)] // declared correctly
    [InlineData(1000)] // the stream sends more than declared
    [InlineData(Limit * 3)] // declared over the limit; the bytes received are what count
    [InlineData(0)] // nothing declared
    public async Task ReadsWhatArrivesWhateverWasDeclared(long declared)
    {
        var data = Enumerable.Range(0, Limit).Select(i => (byte)(i * 7)).ToArray();
        var result = await BrowserFileService.ReadBoundedAsync(new UnseekableStream(data), "main", Limit, declaredLength: declared);
        result.Status.Should().Be(FileReadStatus.Ok);
        result.Bytes.Should().Equal(data);
    }

    [Fact]
    public async Task TrimsAFileShorterThanDeclared()
    {
        var result = await BrowserFileService.ReadBoundedAsync(new UnseekableStream([1, 2, 3]), "main", Limit, declaredLength: 10);
        result.Status.Should().Be(FileReadStatus.Ok);
        result.Bytes.Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task OneByteOverADeclaredLimitIsTooLarge()
    {
        var result = await BrowserFileService.ReadBoundedAsync(new UnseekableStream(new byte[Limit + 1]), "main", Limit, declaredLength: Limit);
        result.Status.Should().Be(FileReadStatus.TooLarge);
    }

    [Fact]
    public async Task AFileSizedAsDeclaredIsHeldOnce()
    {
        // WebAssembly memory never shrinks, so each extra copy of a large file raises the page's memory for good.
        const int size = 4 * 1024 * 1024;
        var stream = new MemoryStream(new byte[size]);
        var before = GC.GetAllocatedBytesForCurrentThread();
        // A memory stream completes synchronously, so the awaited read finishes on this thread before the count is taken.
        var result = await BrowserFileService.ReadBoundedAsync(stream, "main", size, declaredLength: size);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        result.Status.Should().Be(FileReadStatus.Ok);
        allocated.Should().BeLessThan(size + (size / 8), "the bytes are read into one array and handed over without another copy");
    }

    [Fact]
    public async Task ReportsEmptyStream()
    {
        var result = await BrowserFileService.ReadBoundedAsync(new MemoryStream(), "main", Limit);
        result.Status.Should().Be(FileReadStatus.Empty);
    }

    [Fact]
    public async Task ReportsReadFailure()
    {
        var result = await BrowserFileService.ReadBoundedAsync(new FailingStream(), "main", Limit);
        result.Status.Should().Be(FileReadStatus.ReadFailed);
        result.Bytes.Should().BeEmpty();
    }

    [Fact]
    public void SessionKeepsSanitisedFileName()
    {
        var bytes = SaveFixtures.Synthetic(false);
        SaveFixtures.Open(bytes).FileName.Should().Be(FileNaming.DefaultSaveName);
        SaveFixtures.Open(bytes, "x\u0007.sav").FileName.Should().Be("x.sav");
        SaveFixtures.Open(bytes, "   ").FileName.Should().Be(FileNaming.DefaultSaveName);
    }

    /// <summary>An <see cref="IBrowserFile"/> with a declared size and a stream factory.</summary>
    private sealed class FakeFile(string name, long size, Func<Stream>? open = null) : IBrowserFile
    {
        public bool Opened { get; private set; }
        public string Name => name;
        public DateTimeOffset LastModified => DateTimeOffset.UnixEpoch;
        public long Size => size;
        public string ContentType => "application/octet-stream";

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            Opened = true;
            return (open ?? throw new InvalidOperationException("The file should not be opened."))();
        }
    }

    /// <summary>A stream with no length, like a browser file stream that cannot be trusted to report one.</summary>
    private sealed class UnseekableStream(byte[] data) : MemoryStream(data)
    {
        public override bool CanSeek => false;
        public override long Length => throw new NotSupportedException();
    }

    private sealed class FailingStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => 0; set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("The file changed after it was chosen.");
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new IOException("The file changed after it was chosen.");
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
