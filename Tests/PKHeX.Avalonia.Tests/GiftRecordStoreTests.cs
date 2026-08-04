using System.Buffers.Binary;
using Moq;
using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Avalonia.Services;
using PKHeX.Core;
using PKHeX.Infrastructure.GiftRecords;
using PKHeX.Presentation.Localization;
using PKHeX.Presentation.ViewModels;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Tests for the Switch gift record stores (SWSH/BDSP/PLA/SV) and the records mode of
/// <see cref="MysteryGiftEditorViewModel"/>. Blank saves are used, so every mutation is
/// validated by reading back through the same parser.
/// </summary>
public class GiftRecordStoreTests(ITestOutputHelper output)
{
    private static readonly GiftRecordProvider Provider = new();

    // -----------------------------------------------------------------------
    // 1. Provider resolves a store for every Switch title, and nothing else
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.SW, 50, "SWSH")]
    [InlineData(GameVersion.BD, 60, "BDSP")]  // 50 received + 10 one-day serial entries
    [InlineData(GameVersion.PLA, 50, "PLA")]
    [InlineData(GameVersion.SL, 32, "SV")]
    public void Provider_ResolvesSwitchSaves(GameVersion version, int expectedCount, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var store = Provider.GetStore(sav);

        Assert.NotNull(store);
        Assert.Equal(expectedCount, store.Count);
        var entries = store.ReadAll();
        Assert.Equal(expectedCount, entries.Count);
        Assert.All(entries, e => Assert.True(e.IsEmpty, $"{label}: slot {e.Index} should be empty on a blank save"));
        output.WriteLine($"{label}: store resolved, {expectedCount} empty slots ✓");
    }

    [Theory]
    [InlineData(GameVersion.E, "Gen3-Emerald")]
    [InlineData(GameVersion.X, "Gen6-X")]
    [InlineData(GameVersion.GP, "Gen7b-LGPE")]
    public void Provider_ReturnsNull_ForNonSwitchSaves(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        Assert.Null(Provider.GetStore(sav));
        output.WriteLine($"{label}: no record store ✓");
    }

    // -----------------------------------------------------------------------
    // 2. SWSH: WC8 import round-trips through the WR8 parser
    // -----------------------------------------------------------------------

    [Fact]
    public void Swsh_ImportWc8_RoundTrips()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var store = Provider.GetStore(sav)!;

        var wc = new WC8
        {
            CardID = 1234,
            CardTitleIndex = 3,
            CardType = WC8.GiftType.Pokemon,
            RestrictVersion = 3,
            Species = (ushort)Species.Pikachu,
            Form = 0,
            IsEgg = false,
            Move1 = 84, // Thunder Shock
        };
        wc.SetOT((int)LanguageID.English, "Cheryl");

        var received = new DateTime(2024, 5, 17, 12, 30, 45);
        var ok = store.TryImport(0, wc.Write().ToArray(), ".wc8", received, out var error);

        Assert.True(ok, $"import failed: {error}");
        var entry = store.ReadAll()[0];
        Assert.False(entry.IsEmpty);
        Assert.Equal(1234, entry.CardId);
        Assert.Equal(GiftRecordKind.Pokemon, entry.Kind);
        Assert.Equal((ushort)Species.Pikachu, entry.Species);
        Assert.Equal(received, entry.ReceivedAt);
        Assert.NotNull(entry.Card);
        output.WriteLine($"SWSH: imported WR8 record ({entry.Species}, '{entry.OriginalTrainerName}', {entry.ReceivedAt}) ✓");

        store.ClearEntry(0);
        Assert.True(store.ReadAll()[0].IsEmpty);
        output.WriteLine("SWSH: ClearEntry empties the slot ✓");
    }

    [Fact]
    public void Swsh_ImportWritesDocumentedLayout_WithoutTouchingOtherSlots()
    {
        var sav = (SAV8SWSH)BlankSaveFile.Get(GameVersion.SW);
        var store = Provider.GetStore(sav)!;
        var block = sav.Blocks.GetBlock(0x112D5141);
        var before = block.Data.ToArray();
        var wc = new WC8
        {
            CardID = 1234,
            CardTitleIndex = 3,
            CardType = WC8.GiftType.Pokemon,
            RestrictVersion = 3,
            HeldItem = 25,
            Species = (ushort)Species.Pikachu,
            Form = 1,
            IsEgg = true,
            Move1 = 10,
            Move2 = 20,
            Move3 = 30,
            Move4 = 40,
        };
        wc.SetOT((int)LanguageID.English, "Cheryl");
        var received = new DateTime(2024, 5, 17, 12, 30, 45);

        Assert.True(store.TryImport(3, wc.Write().ToArray(), ".wc8", received, out var error), error.ToString());

        const int start = 3 * 0x68;
        var slot = block.Data.Slice(start, 0x68);
        ulong expectedTimestamp = (2024UL << 26) | (5UL << 22) | (17UL << 17)
                                  | (12UL << 12) | (30UL << 6) | 45UL;
        Assert.Equal(expectedTimestamp, BinaryPrimitives.ReadUInt64LittleEndian(slot));
        Assert.Equal((ushort)1234, BinaryPrimitives.ReadUInt16LittleEndian(slot[0x08..]));
        Assert.Equal(3, slot[0x0A]);
        Assert.Equal(1, slot[0x0C]);
        Assert.Equal(1, slot[0x0E]);
        Assert.Equal((ushort)25, BinaryPrimitives.ReadUInt16LittleEndian(slot[0x10..]));
        Assert.Equal(1, slot[0x12]);
        Assert.Equal((ushort)Species.Pikachu, BinaryPrimitives.ReadUInt16LittleEndian(slot[0x30..]));
        Assert.Equal((ushort)1, BinaryPrimitives.ReadUInt16LittleEndian(slot[0x32..]));
        Assert.Equal((ushort)10, BinaryPrimitives.ReadUInt16LittleEndian(slot[0x38..]));
        Assert.Equal((ushort)20, BinaryPrimitives.ReadUInt16LittleEndian(slot[0x3C..]));
        Assert.Equal((ushort)30, BinaryPrimitives.ReadUInt16LittleEndian(slot[0x40..]));
        Assert.Equal((ushort)40, BinaryPrimitives.ReadUInt16LittleEndian(slot[0x44..]));
        Assert.Equal((byte)LanguageID.English, slot[0x62]);
        Assert.Equal(before[..start], block.Data[..start].ToArray());
        Assert.Equal(before[(start + 0x68)..], block.Data[(start + 0x68)..].ToArray());
    }

    [Fact]
    public void Swsh_RecipientOwnedPokemon_UsesSaveTrainerMetadata()
    {
        var sav = (SAV8SWSH)BlankSaveFile.Get(GameVersion.SW);
        sav.OT = "Recipient";
        sav.Gender = 1;
        sav.Language = (int)LanguageID.English;
        var store = Provider.GetStore(sav)!;
        var wc = new WC8
        {
            CardType = WC8.GiftType.Pokemon,
            RestrictVersion = 3,
            Species = (ushort)Species.Pikachu,
            Gender = 1,
            OTGender = 3,
        };
        wc.Data[0x1C] = 4;
        wc.SetRibbonAtIndex(0, 42);

        Assert.True(store.TryImport(0, wc.Write().ToArray(), ".wc8", new DateTime(2024, 1, 1), out var error), error.ToString());

        var slot = sav.Blocks.GetBlock(0x112D5141).Data[..0x68];
        Assert.Equal("Recipient", StringConverter8.GetString(slot.Slice(0x48, 0x18)));
        Assert.Equal(1, slot[0x34]);
        Assert.Equal(1, slot[0x64]);
        Assert.Equal(42, slot[0x63]);
        Assert.Equal(5, slot[0x0F]);
        Assert.Equal((byte)LanguageID.English, slot[0x62]);
    }

    [Fact]
    public void Swsh_WrongVersion_DoesNotMutateSlot()
    {
        var sav = (SAV8SWSH)BlankSaveFile.Get(GameVersion.SW);
        var store = Provider.GetStore(sav)!;
        var slot = sav.Blocks.GetBlock(0x112D5141).Data[..0x68];
        slot.Fill(0xA5);
        var before = slot.ToArray();
        var wc = new WC8 { CardType = WC8.GiftType.Pokemon, RestrictVersion = 2 };

        Assert.False(store.TryImport(0, wc.Write().ToArray(), ".wc8", new DateTime(2024, 1, 1), out var error));
        Assert.Equal(GiftRecordImportError.WrongGame, error);
        Assert.Equal(before, slot.ToArray());
    }

    [Fact]
    public void Swsh_InvalidTimestamp_DoesNotMutateSlot()
    {
        var sav = (SAV8SWSH)BlankSaveFile.Get(GameVersion.SW);
        var store = Provider.GetStore(sav)!;
        var slot = sav.Blocks.GetBlock(0x112D5141).Data[..0x68];
        slot.Fill(0xA5);
        var before = slot.ToArray();
        var wc = new WC8 { CardType = WC8.GiftType.Pokemon, RestrictVersion = 3 };

        Assert.False(store.TryImport(0, wc.Write().ToArray(), ".wc8", new DateTime(4096, 1, 1), out var error));
        Assert.Equal(GiftRecordImportError.InvalidTimestamp, error);
        Assert.Equal(before, slot.ToArray());
    }

    [Fact]
    public void Swsh_ImportWrongGame_Rejected()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var store = Provider.GetStore(sav)!;

        var wc9 = new WC9 { CardID = 1, CardType = WC9.GiftType.Pokemon };
        var ok = store.TryImport(0, wc9.Write().ToArray(), ".wc9", DateTime.Now, out var error);

        Assert.False(ok);
        Assert.Equal(GiftRecordImportError.WrongGame, error);
        Assert.True(store.ReadAll()[0].IsEmpty, "rejected import must not modify the slot");
        output.WriteLine("SWSH: WC9 import rejected as WrongGame ✓");
    }

    [Fact]
    public void Swsh_ReadsBpAndMoneyAmount_FromDocumentedOffset()
    {
        var sav = (SAV8SWSH)BlankSaveFile.Get(GameVersion.SW);
        var store = Provider.GetStore(sav)!;
        var block = sav.Blocks.GetBlock(0x112D5141);

        Span<byte> bp = block.Data[..0x68];
        bp.Clear();
        bp[0x0C] = 3;
        BinaryPrimitives.WriteUInt16LittleEndian(bp[0x46..], 125);

        Span<byte> money = block.Data.Slice(0x68, 0x68);
        money.Clear();
        money[0x0C] = 5;
        BinaryPrimitives.WriteUInt16LittleEndian(money[0x46..], 500);

        var entries = store.ReadAll();
        Assert.Equal(GiftRecordKind.BattlePoints, entries[0].Kind);
        Assert.Equal(125u, entries[0].Amount);
        Assert.Equal(GiftRecordKind.Money, entries[1].Kind);
        Assert.Equal(500u, entries[1].Amount);
    }

    [Fact]
    public void Swsh_UnsupportedClothingImport_DoesNotMutateSlot()
    {
        var sav = (SAV8SWSH)BlankSaveFile.Get(GameVersion.SW);
        var store = Provider.GetStore(sav)!;
        var block = sav.Blocks.GetBlock(0x112D5141);
        block.Data[..0x68].Fill(0xA5);
        var before = block.Data[..0x68].ToArray();
        var wc = new WC8 { CardID = 55, CardType = WC8.GiftType.Clothing };

        Assert.False(store.TryImport(0, wc.Write().ToArray(), ".wc8", new DateTime(2024, 1, 1), out var error));
        Assert.Equal(GiftRecordImportError.UnsupportedGiftType, error);
        Assert.Equal(before, block.Data[..0x68].ToArray());
    }

    // -----------------------------------------------------------------------
    // 3. SV / PLA: trimmed-wondercard entries round-trip and export
    // -----------------------------------------------------------------------

    [Fact]
    public void Sv_ImportWc9_RoundTrips_AndExports()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var store = Provider.GetStore(sav)!;

        var wc = new WC9
        {
            CardID = 1503,
            CardTitleIndex = 6,
            CardType = WC9.GiftType.Pokemon,
            Species = (ushort)Species.FlutterMane,
        };
        var ok = store.TryImport(2, wc.Write().ToArray(), ".wc9", new DateTime(2025, 1, 2, 3, 4, 5), out var error);

        Assert.True(ok, $"import failed: {error}");
        var entry = store.ReadAll()[2];
        Assert.False(entry.IsEmpty);
        Assert.Equal(1503, entry.CardId);
        Assert.Equal(GiftRecordKind.Pokemon, entry.Kind);
        Assert.Equal((ushort)Species.FlutterMane, entry.Species);

        var exported = store.ExportCard(2);
        var wc9 = Assert.IsType<WC9>(exported);
        Assert.Equal(1503, wc9.CardID);
        Assert.Equal((ushort)Species.FlutterMane, wc9.Species);
        Assert.Equal(6, wc9.CardTitleIndex);
        output.WriteLine($"SV: trimmed entry round-trip + export OK (CardID={wc9.CardID}) ✓");
    }

    [Fact]
    public void Sv_ImportWritesDocumentedTrimmedLayout_AndPreservesBlockTail()
    {
        var sav = (SAV9SV)BlankSaveFile.Get(GameVersion.SL);
        var store = Provider.GetStore(sav)!;
        var block = sav.Blocks.GetBlock(0x99E1625E);
        var before = block.Data.ToArray();
        var wc = new WC9
        {
            CardID = 1503,
            CardTitleIndex = 6,
            CardType = WC9.GiftType.Pokemon,
            Species = (ushort)Species.FlutterMane,
        };
        var received = new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Local);

        Assert.True(store.TryImport(2, wc.Write().ToArray(), ".wc9", received, out var error), error.ToString());

        const int start = 2 * 0x278;
        var slot = block.Data.Slice(start, 0x278);
        Assert.Equal((uint)new DateTimeOffset(received).ToUnixTimeSeconds(),
            BinaryPrimitives.ReadUInt32LittleEndian(slot));
        Assert.Equal(wc.Data[0x08..0x0C].ToArray(), slot[0x08..0x0C].ToArray());
        Assert.Equal(wc.CardTitleIndex, slot[0x0C]);
        Assert.Equal((byte)wc.CardType, slot[0x0E]);
        Assert.Equal(wc.Data[0x18..0x280].ToArray(), slot[0x10..].ToArray());
        Assert.Equal(new byte[4], slot[0x04..0x08].ToArray());
        Assert.Equal(0, slot[0x0D]);
        Assert.Equal(0, slot[0x0F]);
        Assert.Equal(before[..start], block.Data[..start].ToArray());
        Assert.Equal(before[(start + 0x278)..], block.Data[(start + 0x278)..].ToArray());
    }

    [Fact]
    public void Sv_PreUnixTimestamp_DoesNotMutateSlot()
    {
        var sav = (SAV9SV)BlankSaveFile.Get(GameVersion.SL);
        var store = Provider.GetStore(sav)!;
        var slot = sav.Blocks.GetBlock(0x99E1625E).Data[..0x278];
        slot.Fill(0xA5);
        var before = slot.ToArray();
        var wc = new WC9 { CardType = WC9.GiftType.Pokemon };

        Assert.False(store.TryImport(0, wc.Write().ToArray(), ".wc9", new DateTime(1969, 1, 1), out var error));
        Assert.Equal(GiftRecordImportError.InvalidTimestamp, error);
        Assert.Equal(before, slot.ToArray());
    }

    [Fact]
    public void Sv_UnsupportedItemImport_DoesNotMutateSlot()
    {
        var sav = (SAV9SV)BlankSaveFile.Get(GameVersion.SL);
        var store = Provider.GetStore(sav)!;
        var block = sav.Blocks.GetBlock(0x99E1625E);
        block.Data[..0x278].Fill(0xA5);
        var before = block.Data[..0x278].ToArray();
        var wc = new WC9 { CardID = 55, CardType = WC9.GiftType.Item };

        Assert.False(store.TryImport(0, wc.Write().ToArray(), ".wc9", new DateTime(2024, 1, 1), out var error));
        Assert.Equal(GiftRecordImportError.UnsupportedGiftType, error);
        Assert.Equal(before, block.Data[..0x278].ToArray());
    }

    [Fact]
    public void Sv_WrongVersion_DoesNotMutateSlot()
    {
        var sav = (SAV9SV)BlankSaveFile.Get(GameVersion.SL);
        var store = Provider.GetStore(sav)!;
        var slot = sav.Blocks.GetBlock(0x99E1625E).Data[..0x278];
        slot.Fill(0xA5);
        var before = slot.ToArray();
        var wc = new WC9 { CardType = WC9.GiftType.Pokemon, RestrictVersion = 2 };

        Assert.False(store.TryImport(0, wc.Write().ToArray(), ".wc9", new DateTime(2024, 1, 1), out var error));
        Assert.Equal(GiftRecordImportError.WrongGame, error);
        Assert.Equal(before, slot.ToArray());
    }

    [Fact]
    public void Pla_ImportWa8_RoundTrips()
    {
        var sav = BlankSaveFile.Get(GameVersion.PLA);
        var store = Provider.GetStore(sav)!;

        var wa = new WA8
        {
            CardID = 501,
            CardTitleIndex = 2,
            CardType = WA8.GiftType.Pokemon,
            Species = (ushort)Species.Rowlet,
        };
        var ok = store.TryImport(0, wa.Write().ToArray(), ".wa8", DateTime.Now, out var error);

        Assert.True(ok, $"import failed: {error}");
        var entry = store.ReadAll()[0];
        Assert.Equal(501, entry.CardId);
        Assert.Equal((ushort)Species.Rowlet, entry.Species);
        Assert.Equal(GiftRecordKind.Pokemon, entry.Kind);

        // WC9 into a PLA save is the wrong card format
        var wc9 = new WC9 { CardID = 1 };
        Assert.False(store.TryImport(1, wc9.Write().ToArray(), ".wc9", DateTime.Now, out var err2));
        Assert.Equal(GiftRecordImportError.WrongGame, err2);
        output.WriteLine("PLA: WA8 accepted, WC9 rejected ✓");
    }

    // -----------------------------------------------------------------------
    // 4. BDSP: typed block — records, flags, serial lock
    // -----------------------------------------------------------------------

    [Fact]
    public void Bdsp_ImportWb8_Flags_And_SerialLock()
    {
        var sav = (SAV8BS)BlankSaveFile.Get(GameVersion.BD);
        var store = Provider.GetStore(sav)!;

        Assert.True(store.SupportsReceivedFlags);
        Assert.True(store.SupportsSerialLock);

        var wb = new WB8
        {
            CardID = 9001,
            CardType = WB8.GiftType.Pokemon,
            Species = (ushort)Species.Turtwig,
        };
        var ok = store.TryImport(0, wb.Write().ToArray(), ".wb8", new DateTime(2023, 11, 21, 8, 0, 0), out var error);

        Assert.True(ok, $"import failed: {error}");
        var entry = store.ReadAll()[0];
        Assert.Equal(9001, entry.CardId);
        Assert.Equal((ushort)Species.Turtwig, entry.Species);
        Assert.Equal(GiftRecordKind.Pokemon, entry.Kind);

        // Received flags
        Assert.Empty(store.GetReceivedFlagIndexes());
        store.SetReceivedFlag(42, true);
        Assert.Contains(42, store.GetReceivedFlagIndexes());
        store.SetReceivedFlag(42, false);
        Assert.DoesNotContain(42, store.GetReceivedFlagIndexes());

        foreach (var flag in new[] { 0, 255, 256, 2047 })
            store.SetReceivedFlag(flag, true);
        Assert.Equal(new[] { 0, 255, 256, 2047 }, store.GetReceivedFlagIndexes());
        const int flagBase = 0xE9BE8 + MysteryBlock8b.OFS_RECVFLAG;
        Assert.NotEqual(0, sav.Data[flagBase + (2047 >> 3)] & (1 << (2047 & 7)));
        store.SetReceivedFlag(256, false);
        Assert.Equal(new[] { 0, 255, 2047 }, store.GetReceivedFlagIndexes());

        // Serial lock
        Assert.Null(store.SerialLockTimestamp);
        sav.MysteryRecords.TimestampSerialLock = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.NotNull(store.SerialLockTimestamp);
        store.ResetSerialLock();
        Assert.Null(store.SerialLockTimestamp);
        output.WriteLine("BDSP: record import, flags, serial lock all OK ✓");
    }

    [Fact]
    public void Bdsp_UnsupportedMoneyImport_DoesNotMutateSlot()
    {
        var sav = (SAV8BS)BlankSaveFile.Get(GameVersion.BD);
        var store = Provider.GetStore(sav)!;
        var original = sav.MysteryRecords.GetReceived(0);
        original.DeliveryID = 99;
        var wb = new WB8 { CardID = 55, CardType = WB8.GiftType.Money };

        Assert.False(store.TryImport(0, wb.Write().ToArray(), ".wb8", new DateTime(2024, 1, 1), out var error));
        Assert.Equal(GiftRecordImportError.UnsupportedGiftType, error);
        Assert.Equal(99, sav.MysteryRecords.GetReceived(0).DeliveryID);
    }

    [Fact]
    public void Bdsp_RecipientOwnedPokemon_UsesSaveTrainerMetadata()
    {
        var sav = (SAV8BS)BlankSaveFile.Get(GameVersion.BD);
        sav.OT = "Recipient";
        sav.Gender = 1;
        sav.Language = (int)LanguageID.English;
        var store = Provider.GetStore(sav)!;
        var wb = new WB8
        {
            CardType = WB8.GiftType.Pokemon,
            Species = (ushort)Species.Turtwig,
            OTGender = 3,
        };

        Assert.True(store.TryImport(0, wb.Write().ToArray(), ".wb8", new DateTime(2024, 1, 1), out var error), error.ToString());

        var record = sav.MysteryRecords.GetReceived(0);
        Assert.Equal("Recipient", record.OT);
        Assert.Equal(1, record.OriginalTrainerGender);
        Assert.Equal((byte)LanguageID.English, record.Language);
    }

    [Fact]
    public void Bdsp_PreFileTimeTimestamp_DoesNotMutateSlot()
    {
        var sav = (SAV8BS)BlankSaveFile.Get(GameVersion.BD);
        var store = Provider.GetStore(sav)!;
        var original = sav.MysteryRecords.GetReceived(0);
        original.DeliveryID = 99;
        var wb = new WB8 { CardType = WB8.GiftType.Pokemon };

        Assert.False(store.TryImport(0, wb.Write().ToArray(), ".wb8", new DateTime(1500, 1, 1), out var error));
        Assert.Equal(GiftRecordImportError.InvalidTimestamp, error);
        Assert.Equal(99, sav.MysteryRecords.GetReceived(0).DeliveryID);
    }

    [Fact]
    public void Bdsp_UndergroundItemRecord_HasDistinctKind()
    {
        var sav = (SAV8BS)BlankSaveFile.Get(GameVersion.BD);
        var record = new RecvData8b(new byte[RecvData8b.SIZE])
        {
            DeliveryID = 123,
            DataType = (byte)WB8.GiftType.UnderGroundItem,
            Item1 = 1,
            Item1Count = 2,
        };
        sav.MysteryRecords.SetReceived(0, record);

        var entry = Provider.GetStore(sav)!.ReadAll()[0];

        Assert.Equal(GiftRecordKind.UndergroundItem, entry.Kind);
    }

    [Fact]
    public void Bdsp_ResetInactiveSerialLock_DoesNotDirtySave()
    {
        var sav = (SAV8BS)BlankSaveFile.Get(GameVersion.BD);
        var store = Provider.GetStore(sav)!;
        sav.State.Edited = false;

        store.ResetSerialLock();

        Assert.False(sav.State.Edited);
    }

    // -----------------------------------------------------------------------
    // 5. ViewModel records mode
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.SW, "SWSH")]
    [InlineData(GameVersion.BD, "BDSP")]
    [InlineData(GameVersion.PLA, "PLA")]
    [InlineData(GameVersion.SL, "SV")]
    public void ViewModel_RecordsMode_ActiveForSwitchSaves(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new MysteryGiftEditorViewModel(sav, new Mock<IDialogService>().Object, Provider);

        Assert.False(vm.IsSupported, $"{label}: no wondercard album expected");
        Assert.True(vm.IsRecordsSupported, $"{label}: records mode expected");
        Assert.True(vm.HasAnySupport);
        Assert.Equal(vm.RecordCount, vm.Records.Count);
        Assert.NotNull(vm.SelectedRecord);
        output.WriteLine($"{label}: records mode active with {vm.RecordCount} slots ✓");
    }

    [Fact]
    public void ViewModel_NoSupportAtAll_ForGen3()
    {
        var sav = BlankSaveFile.Get(GameVersion.E);
        var vm = new MysteryGiftEditorViewModel(sav, new Mock<IDialogService>().Object, Provider);

        Assert.False(vm.IsSupported);
        Assert.False(vm.IsRecordsSupported);
        Assert.False(vm.HasAnySupport);
        output.WriteLine("Gen3: neither album nor records ✓");
    }

    [Fact]
    public void ViewModel_AlbumModeWins_ForGen6()
    {
        var sav = BlankSaveFile.Get(GameVersion.X);
        var vm = new MysteryGiftEditorViewModel(sav, new Mock<IDialogService>().Object, Provider);

        Assert.True(vm.IsSupported);
        Assert.False(vm.IsRecordsSupported);
        output.WriteLine("Gen6: album mode, records inactive ✓");
    }

    [Fact]
    public void ViewModel_DeleteRecord_ClearsSlot()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var store = Provider.GetStore(sav)!;
        var wc = new WC9 { CardID = 77, CardType = WC9.GiftType.Pokemon, Species = (ushort)Species.Sprigatito };
        Assert.True(store.TryImport(0, wc.Write().ToArray(), ".wc9", DateTime.Now, out _));

        var vm = new MysteryGiftEditorViewModel(sav, new Mock<IDialogService>().Object, Provider);
        Assert.False(vm.Records[0].IsEmpty);

        vm.SelectedRecord = vm.Records[0];
        vm.DeleteRecordCommand.Execute(null);

        Assert.True(vm.Records[0].IsEmpty);
        output.WriteLine("SV VM: DeleteRecord clears the slot ✓");
    }

    [Fact]
    public void ViewModel_BdspOneDaySlot_CannotImport()
    {
        var sav = BlankSaveFile.Get(GameVersion.BD);
        var vm = new MysteryGiftEditorViewModel(sav, new Mock<IDialogService>().Object, Provider);

        vm.SelectedRecord = vm.Records[50];

        Assert.False(vm.CanImportRecord);
    }

    [Fact]
    public void ViewModel_RefreshLocalization_RebuildsTextAndPreservesSelection()
    {
        var loc = LocalizedStrings.Instance;
        try
        {
            loc.SetLanguage("en");
            var sav = BlankSaveFile.Get(GameVersion.SW);
            var vm = new MysteryGiftEditorViewModel(sav, new Mock<IDialogService>().Object, Provider);
            vm.SelectedRecord = vm.Records[4];
            var englishTitle = vm.SelectedRecord.Title;

            loc.SetLanguage("de");
            vm.RefreshLocalization();

            Assert.Equal(4, vm.SelectedRecord.Index);
            Assert.NotEqual(englishTitle, vm.SelectedRecord.Title);
            Assert.DoesNotContain("slots", vm.RecordCountText, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            loc.SetLanguage("en");
        }
    }
}
