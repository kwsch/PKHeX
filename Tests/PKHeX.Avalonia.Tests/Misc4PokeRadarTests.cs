using PKHeX.Avalonia.Tests.Fixtures;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class Misc4PokeRadarTests
{
    private const ushort PokeRadarItem = 431;

    private static InventoryPouch4 KeyItemsPouch(SaveFile sav) =>
        (InventoryPouch4)sav.Inventory.Pouches.First(p => p.Type is InventoryType.KeyItems);

    [Fact]
    public void Misc4_PokeRadar_Pt_ToggleOn_AddsItemToKeyItems()
    {
        // Arrange
        var sav = new SAV4Pt();
        var vm = new Misc4EditorViewModel(sav);

        // Act
        vm.PokeRadar = true;
        vm.SaveCommand.Execute(null);

        // Assert
        var pouch = KeyItemsPouch(sav);
        Assert.True(pouch.HasItem(PokeRadarItem));
        Assert.Equal(1, pouch.Items.First(it => it.Index == PokeRadarItem).Count);
    }

    [Fact]
    public void Misc4_PokeRadar_Pt_ToggleOff_RemovesItemFromKeyItems()
    {
        // Arrange
        var sav = new SAV4Pt();
        var vm = new Misc4EditorViewModel(sav);
        vm.PokeRadar = true;
        vm.SaveCommand.Execute(null);

        // Act
        vm.PokeRadar = false;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.False(KeyItemsPouch(sav).HasItem(PokeRadarItem));
    }

    [Fact]
    public void Misc4_PokeRadar_Pt_LoadsExistingState()
    {
        // Arrange
        var sav = new SAV4Pt();
        var bag = sav.Inventory;
        var pouch = (InventoryPouch4)bag.Pouches.First(p => p.Type is InventoryType.KeyItems);
        pouch.Items[0].Index = PokeRadarItem;
        pouch.Items[0].Count = 1;
        bag.CopyTo(sav);

        // Act
        var vm = new Misc4EditorViewModel(sav);

        // Assert
        Assert.True(vm.PokeRadar);
    }

    [Fact]
    public void Misc4_PokeRadar_DP_ToggleOn_AddsItemToKeyItems()
    {
        // Arrange
        var sav = new SAV4DP();
        var vm = new Misc4EditorViewModel(sav);

        // Act
        vm.PokeRadar = true;
        vm.SaveCommand.Execute(null);

        // Assert
        var pouch = KeyItemsPouch(sav);
        Assert.True(pouch.HasItem(PokeRadarItem));
        Assert.Equal(1, pouch.Items.First(it => it.Index == PokeRadarItem).Count);
    }

    [Fact]
    public void Misc4_PokeRadar_DP_ToggleOff_RemovesItemFromKeyItems()
    {
        // Arrange
        var sav = new SAV4DP();
        var vm = new Misc4EditorViewModel(sav);
        vm.PokeRadar = true;
        vm.SaveCommand.Execute(null);

        // Act
        vm.PokeRadar = false;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.False(KeyItemsPouch(sav).HasItem(PokeRadarItem));
    }

    [Fact]
    public void Misc4_PokeRadar_NotVisible_ForHGSS()
    {
        // Arrange
        var sav = new SAV4HGSS();

        // Act
        var vm = new Misc4EditorViewModel(sav);

        // Assert
        Assert.False(vm.IsPokeRadarVisible);
        Assert.False(vm.PokeRadar);
    }

    // =========================================================================
    // Regression: OFF path must not leave a null hole in the null-terminated
    // Gen4 bag pouch when the radar sits mid-list (items follow it).
    // =========================================================================

    [Fact]
    public void Misc4_PokeRadar_Pt_ToggleOff_MidList_CompactsPouch_NoNullHole()
    {
        // Arrange: place the radar in the middle of a run of key items, with items after it.
        // Zeroing the radar's slot in place (without compacting) leaves those trailing items
        // stranded behind a null hole, making them invisible in-game.
        var sav = new SAV4Pt();
        var bag = sav.Inventory;
        var pouch = (InventoryPouch4)bag.Pouches.First(p => p.Type is InventoryType.KeyItems);

        ushort[] beforeRadar = [428, 433];
        ushort[] afterRadar = [434, 435, 438];
        int slot = 0;
        foreach (var idx in beforeRadar)
        {
            pouch.Items[slot].Index = idx;
            pouch.Items[slot].Count = 1;
            slot++;
        }
        pouch.Items[slot].Index = PokeRadarItem;
        pouch.Items[slot].Count = 1;
        slot++;
        foreach (var idx in afterRadar)
        {
            pouch.Items[slot].Index = idx;
            pouch.Items[slot].Count = 1;
            slot++;
        }
        bag.CopyTo(sav);

        var vm = new Misc4EditorViewModel(sav);
        Assert.True(vm.PokeRadar);

        // Act
        vm.PokeRadar = false;
        vm.SaveCommand.Execute(null);

        // Assert: no empty slot precedes a filled one (no null hole in the null-terminated pouch).
        var result = KeyItemsPouch(sav);
        bool seenEmpty = false;
        for (int i = 0; i < result.Items.Length; i++)
        {
            var isEmpty = result.Items[i].Count == 0;
            if (seenEmpty)
                Assert.True(isEmpty, $"Slot {i} is filled (Index={result.Items[i].Index}) after an earlier empty slot - null hole present");
            seenEmpty |= isEmpty;
        }

        // Assert: radar removed, all other items retained (order preserved).
        Assert.False(result.HasItem(PokeRadarItem));
        var expectedOrder = beforeRadar.Concat(afterRadar).ToArray();
        var actualOrder = result.Items.Where(it => it.Count > 0).Select(it => (ushort)it.Index).ToArray();
        Assert.Equal(expectedOrder, actualOrder);
    }

    // =========================================================================
    // Regression: byte-level round trip against a real save. Toggling the radar
    // must only touch the key-item pouch bytes and the general-block checksum
    // footer - nothing else in the file may change.
    // =========================================================================

    [Fact]
    public void Misc4_PokeRadar_Pt_RealSave_ToggleOn_WriteRoundTrip_OnlyTouchesPouchAndChecksum()
    {
        // Arrange
        var saveDir = SaveFileFixture.FindSaveFilesPath();
        Assert.NotNull(saveDir);
        var raw = File.ReadAllBytes(Path.Combine(saveDir!, "gen4_platinum.sav"));

        var sav = Assert.IsType<SAV4Pt>(SaveUtil.GetSaveFile(raw));

        var bag = sav.Inventory;
        var pouch = (InventoryPouch4)bag.Pouches.First(p => p.Type is InventoryType.KeyItems);

        // This fixture already carries the radar mid-list (slot 16 of 40). Establish a "radar
        // absent" baseline directly through the Core pouch API - rather than via the VM's OFF
        // path exercised by the compaction test above - so this byte-scoping check is independent
        // of that fix and purely exercises the (always-correct) ON path against real save bytes.
        pouch.Items.FirstOrDefault(it => it.Index == PokeRadarItem)?.Clear();
        bag.CopyTo(sav);
        var baseline = sav.Write().ToArray();

        var vm = new Misc4EditorViewModel(sav);
        Assert.False(vm.PokeRadar);

        // Act
        vm.PokeRadar = true;
        vm.SaveCommand.Execute(null);
        var after = sav.Write().ToArray();

        // Assert: only the key-item pouch region and the general-block checksum footer changed.
        Assert.Equal(baseline.Length, after.Length);

        // The active General block for this specific fixture (SAV4's double-buffered block
        // selection picks whichever of the two 0x40000 partitions holds the newer save
        // generation - a runtime fact not derivable from any public API) lives in the second
        // partition. Absolute pouch offset = partition base (0x40000) + PlayerBag4Pt's internal
        // BaseOffset (0x630) + KeyItems pouch offset (0x294) = 0x0408C4. Verified empirically
        // against this fixture file.
        const int pouchOffset = 0x0408C4;
        var pouchLength = pouch.Items.Length * 4; // InventoryPouch4: 4 bytes/item (u16 index, u16 count)
        const int generalBlockPartitionOffset = 0x40000;
        var checksumOffset = generalBlockPartitionOffset + SAV4Pt.GeneralSize - 2; // last 2 bytes of General = CRC16

        var unexpectedOffsets = new List<int>();
        for (int i = 0; i < baseline.Length; i++)
        {
            if (baseline[i] == after[i]) continue;
            var inPouch = i >= pouchOffset && i < pouchOffset + pouchLength;
            var inChecksum = i >= checksumOffset && i < checksumOffset + 2;
            if (!inPouch && !inChecksum)
                unexpectedOffsets.Add(i);
        }
        Assert.True(unexpectedOffsets.Count == 0,
            $"Unexpected byte changes outside the pouch/checksum regions at: {string.Join(", ", unexpectedOffsets.Take(20).Select(o => $"0x{o:X}"))}");

        var pouchChanged = false;
        for (int i = pouchOffset; i < pouchOffset + pouchLength; i++)
        {
            if (baseline[i] != after[i]) { pouchChanged = true; break; }
        }
        Assert.True(pouchChanged, "Expected the key-item pouch region to change when toggling the radar on");

        // Assert: the written bytes reload as a valid SAV4Pt with the radar present.
        var reloaded = Assert.IsType<SAV4Pt>(SaveUtil.GetSaveFile(after));
        Assert.True(reloaded.ChecksumsValid);
        var reloadedPouch = reloaded.Inventory.Pouches.First(p => p.Type is InventoryType.KeyItems);
        Assert.True(reloadedPouch.HasItem(PokeRadarItem));
    }
}
