using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Moq;
using PKHeX.Application.Abstractions;
using PKHeX.Avalonia.Services;
using PKHeX.Core;
using PKHeX.Presentation.Models;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Unit tests for <see cref="SlotDragTransfer"/> (crash-hardening drag-and-drop fix): the
/// synchronous <c>Create</c> overload used from <c>OnSlotPointerMoved</c> must never await
/// before <c>DragDrop.DoDragDropAsync</c> is invoked (macOS requires the native drag session to
/// start within the live pointer-moved frame), and it must degrade gracefully when a real OS
/// file reference can't be resolved synchronously.
/// </summary>
public class SlotDragTransferTests(ITestOutputHelper output)
{
    [Fact]
    public void Create_StringOnly_RoundTripsSlotLocation()
    {
        var source = SlotLocation.FromBox(2, 5);
        var data = new SlotDragData(source);

        var transfer = SlotDragTransfer.Create(data);
        var roundTripped = SlotDragTransfer.TryGet(transfer);

        Assert.NotNull(roundTripped);
        Assert.Equal(source.Box, roundTripped!.Source.Box);
        Assert.Equal(source.Slot, roundTripped.Source.Slot);
        Assert.Equal(source.IsParty, roundTripped.Source.IsParty);
        output.WriteLine("Create(data): slot location round-trips through the string payload ✓");
    }

    [Fact]
    public void Create_WithFile_IsFullySynchronous_NoTaskReturned()
    {
        // This is the core regression guard for the macOS drag-start defect: the overload used
        // by OnSlotPointerMoved must return DataTransfer directly (not Task<DataTransfer>), so
        // there is no way to accidentally reintroduce an await before DoDragDropAsync.
        var method = typeof(SlotDragTransfer).GetMethod("Create", [typeof(SlotDragData), typeof(PKM), typeof(IStorageProvider)]);

        Assert.NotNull(method);
        Assert.False(typeof(Task).IsAssignableFrom(method!.ReturnType));
        output.WriteLine($"Create(data, pk, storageProvider) returns {method.ReturnType.Name}, not a Task ✓");
    }

    [Fact]
    public void Create_NullPkm_DegradesToStringOnlyPayload()
    {
        var data = new SlotDragData(SlotLocation.FromParty(1));
        var storageProvider = Mock.Of<IStorageProvider>();

        var transfer = SlotDragTransfer.Create(data, null, storageProvider);

        Assert.NotNull(SlotDragTransfer.TryGet(transfer));
        output.WriteLine("Create: null PKM degrades to in-app-only (string) payload, no exception ✓");
    }

    [Fact]
    public void Create_NullStorageProvider_DegradesToStringOnlyPayload()
    {
        var sav = new SAV9SV();
        var pk = (PK9)sav.BlankPKM;
        pk.Species = 25;

        var data = new SlotDragData(SlotLocation.FromBox(0, 0));
        var transfer = SlotDragTransfer.Create(data, pk, null);

        Assert.NotNull(SlotDragTransfer.TryGet(transfer));
        output.WriteLine("Create: null IStorageProvider degrades to in-app-only (string) payload ✓");
    }

    [Fact]
    public void Create_StorageResolutionDoesNotCompleteSynchronously_DegradesGracefully()
    {
        var sav = new SAV9SV();
        var pk = (PK9)sav.BlankPKM;
        pk.Species = 6;
        pk.Nickname = "Charizard";

        // Simulates the real-world case where TryGetFileFromPathAsync can't complete inline
        // (e.g. it genuinely hits the filesystem asynchronously): Create must not block/await on
        // it, and must still return a usable in-app drag payload.
        var pending = new TaskCompletionSource<IStorageFile?>();
        var mockProvider = new Mock<IStorageProvider>();
        mockProvider.Setup(p => p.TryGetFileFromPathAsync(It.IsAny<System.Uri>()))
                    .Returns(pending.Task);

        var data = new SlotDragData(SlotLocation.FromBox(0, 0));
        var transfer = SlotDragTransfer.Create(data, pk, mockProvider.Object);

        Assert.NotNull(SlotDragTransfer.TryGet(transfer));
        output.WriteLine("Create: pending (not-yet-completed) storage resolution is skipped, in-app payload still present ✓");

        pending.TrySetResult(null); // avoid leaving the TCS dangling
    }

    [Fact]
    public void Create_EmptySlotPkm_SkipsFileExportButKeepsStringPayload()
    {
        var pk = new PK9(); // Species == 0 -> ExportEntityToFileUseCase returns null
        var storageProvider = Mock.Of<IStorageProvider>();

        var data = new SlotDragData(SlotLocation.FromBox(1, 3));
        var transfer = SlotDragTransfer.Create(data, pk, storageProvider);

        Assert.NotNull(SlotDragTransfer.TryGet(transfer));
        output.WriteLine("Create: blank PKM (no exportable entity) still yields a valid in-app payload ✓");
    }

    [Fact]
    public void TryGet_NullTransfer_ReturnsNull()
    {
        Assert.Null(SlotDragTransfer.TryGet(null));
        output.WriteLine("TryGet: null transfer returns null, no exception ✓");
    }
}
