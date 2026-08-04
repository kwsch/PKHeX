using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using PKHeX.Avalonia.Tests.Fixtures;
using PKHeX.Avalonia.Views;
using PKHeX.Core;
using PKHeX.Infrastructure.GiftRecords;
using PKHeX.Presentation.Localization;
using PKHeX.Presentation.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests.Harness;

/// <summary>
/// Full-app coverage for the Switch received-gift record UI. These tests load saves through the
/// production composition root, select the real Gifts tab, and activate the rendered controls.
/// </summary>
public sealed class HeadlessGiftRecordTests(ITestOutputHelper output)
{
    [AvaloniaTheory]
    [InlineData(GameVersion.SW, 50)]
    [InlineData(GameVersion.BD, 60)]
    [InlineData(GameVersion.PLA, 50)]
    [InlineData(GameVersion.SL, 32)]
    public void SwitchSave_LoadsAndRealizesGiftRecordSlots(GameVersion version, int expectedSlots)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSaveInstance(BlankSaveFile.Get(version));

        app.SelectTab(LocalizedStrings.Instance["Tab_Gifts"]);

        var editor = app.Find<MysteryGiftEditor>();
        Assert.NotNull(editor);
        Assert.True(editor.IsEffectivelyVisible);
        Assert.True(editor.Bounds.Width > 0);
        Assert.True(editor.Bounds.Height > 0);

        var vm = Assert.IsType<MysteryGiftEditorViewModel>(editor.DataContext);
        Assert.True(vm.IsRecordsSupported);
        Assert.False(vm.IsSupported);
        Assert.Equal(expectedSlots, vm.RecordCount);
        Assert.Equal(expectedSlots, vm.Records.Count);

        var realizedSlots = editor.GetVisualDescendants()
            .OfType<Button>()
            .Count(button => ReferenceEquals(button.Command, vm.SelectRecordCommand));
        Assert.Equal(expectedSlots, realizedSlots);
    }

    [AvaloniaFact]
    public void SvDeleteButton_ClearsSelectedRecordThroughRenderedCommand()
    {
        var sav = (SAV9SV)BlankSaveFile.Get(GameVersion.SL);
        var store = new GiftRecordProvider().GetStore(sav)!;
        var gift = new WC9
        {
            CardID = 1503,
            CardType = WC9.GiftType.Pokemon,
            RestrictVersion = 3,
            Species = (ushort)Species.Sprigatito,
        };
        Assert.True(store.TryImport(0, gift.Write().ToArray(), ".wc9", new DateTime(2025, 1, 2), out _));

        using var app = new HeadlessAppFixture();
        app.LoadSaveInstance(sav);
        app.SelectTab(LocalizedStrings.Instance["Tab_Gifts"]);

        var editor = app.Find<MysteryGiftEditor>()!;
        var vm = Assert.IsType<MysteryGiftEditorViewModel>(editor.DataContext);
        Assert.False(vm.Records[0].IsEmpty);

        var delete = editor.GetVisualDescendants()
            .OfType<Button>()
            .Single(button => button.IsEffectivelyVisible
                              && Equals(button.Content, LocalizedStrings.Instance["Common_Delete"]));
        app.Click(delete);

        Assert.True(vm.Records[0].IsEmpty);
        Assert.True(store.ReadAll()[0].IsEmpty);
    }

    [AvaloniaFact]
    public void BdspManagementCards_ShowFlagsAndResetActiveSerialLock()
    {
        var sav = (SAV8BS)BlankSaveFile.Get(GameVersion.BD);
        var store = new GiftRecordProvider().GetStore(sav)!;
        store.SetReceivedFlag(2047, true);
        sav.MysteryRecords.TimestampSerialLock = new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc);

        using var app = new HeadlessAppFixture();
        app.LoadSaveInstance(sav);
        app.SelectTab(LocalizedStrings.Instance["Tab_Gifts"]);

        var editor = app.Find<MysteryGiftEditor>()!;
        var vm = Assert.IsType<MysteryGiftEditorViewModel>(editor.DataContext);
        Assert.True(vm.CanManageRecordFlags);
        Assert.Contains("2047", vm.RecordReceivedFlags);
        Assert.NotNull(store.SerialLockTimestamp);

        vm.SelectedRecordFlag = "2047";
        app.Pump();
        var deleteFlag = editor.GetVisualDescendants()
            .OfType<Button>()
            .Single(button => button.IsEffectivelyVisible
                              && Equals(button.Content, LocalizedStrings.Instance["MysteryGiftEditor_DeleteFlag"]));
        app.Click(deleteFlag);
        Assert.DoesNotContain("2047", vm.RecordReceivedFlags);
        Assert.DoesNotContain(2047, store.GetReceivedFlagIndexes());

        var reset = editor.GetVisualDescendants()
            .OfType<Button>()
            .Single(button => Equals(button.Content, LocalizedStrings.Instance["MysteryGiftEditor_ResetSerialLock"]));
        app.Click(reset);

        Assert.Null(store.SerialLockTimestamp);
        Assert.Equal(LocalizedStrings.Instance["MysteryGiftEditor_SerialLockInactive"], vm.SerialLockText);
    }

    [AvaloniaFact]
    public void CaptureSvGiftRecords_WhenEnabled_WritesPng()
    {
        if (Environment.GetEnvironmentVariable("PKHEX_HEADLESS_CAPTURE") != "1")
        {
            output.WriteLine("Skipped: set PKHEX_HEADLESS_CAPTURE=1 with the Skia headless app builder.");
            return;
        }

        var saveDirectory = SaveFileFixture.FindSaveFilesPath();
        Assert.NotNull(saveDirectory);
        var savePath = Path.Combine(saveDirectory, "gen9_scarlet.main");
        var sav = Assert.IsType<SAV9SV>(FileUtil.GetSupportedFile(savePath));
        var store = new GiftRecordProvider().GetStore(sav)!;
        var gift = new WC9
        {
            CardID = 1503,
            CardType = WC9.GiftType.Pokemon,
            RestrictVersion = 3,
            Species = (ushort)Species.Sprigatito,
        };
        gift.SetOT((int)LanguageID.English, "Paldea");
        Assert.True(store.TryImport(0, gift.Write().ToArray(), ".wc9", new DateTime(2025, 1, 2), out _));

        using var app = new HeadlessAppFixture();
        app.LoadSaveInstance(sav);
        int occupiedSlot = Enumerable.Range(0, sav.BoxSlotCount)
            .First(slot => sav.GetBoxSlotAtIndex(0, slot).Species != 0);
        app.ClickSlot(0, occupiedSlot);
        var boxView = app.Find<BoxViewer>()!;
        app.Focus(boxView);
        app.PressKey(PhysicalKey.Enter);
        app.SelectTab(LocalizedStrings.Instance["Tab_Gifts"]);

        var directory = Environment.GetEnvironmentVariable("PKHEX_HEADLESS_CAPTURE_DIR")
                        ?? Path.Combine(Path.GetTempPath(), "pkhex-headless-frames");
        var path = Path.Combine(directory, "gift-records-sv.png");
        var saved = app.CaptureFrame(path);

        Assert.Equal(path, saved);
        Assert.True(File.Exists(path));
        Assert.True(new FileInfo(path).Length > 0);
        output.WriteLine($"Saved SV gift-record screenshot to {path}");
    }
}
