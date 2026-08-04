using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for RecordsEditorViewModel.
/// Records exist for Gen5-8; other generations have HasRecords=false.
/// Editing a RecordItemViewModel immediately writes through to the save
/// via OnValueChanged partial method.
/// </summary>
public class RecordsEditorTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------------
    // 1. HasRecords is true for Gen5-8 saves
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    [InlineData(GameVersion.SW, "Gen8-Sword")]
    public void Records_Gen6To8_HasRecords_True(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new RecordsEditorViewModel(sav);

        Assert.True(vm.HasRecords, $"{label}: expected HasRecords=true");
        Assert.NotEmpty(vm.Records);
        output.WriteLine($"{label}: HasRecords=true, {vm.Records.Count} records loaded ✓");
    }

    // -----------------------------------------------------------------------
    // 2. HasRecords is false for Gen1-4 and Gen9 saves
    // -----------------------------------------------------------------------

    // Gen5 has a RecordList but SAV5 does not implement ITrainerStatRecord,
    // so HasRecords is false for Gen5 saves.
    [Theory]
    [InlineData(GameVersion.RD, "Gen1-Red")]
    [InlineData(GameVersion.GD, "Gen2-Gold")]
    [InlineData(GameVersion.E,  "Gen3-Emerald")]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.W2, "Gen5-White2")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void Records_OtherGens_HasRecords_False(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new RecordsEditorViewModel(sav);

        Assert.False(vm.HasRecords, $"{label}: expected HasRecords=false");
        Assert.Empty(vm.Records);
        output.WriteLine($"{label}: HasRecords=false ✓");
    }

    // -----------------------------------------------------------------------
    // 3. Editing a record value immediately writes through to the save
    // -----------------------------------------------------------------------

    [Fact]
    public void Records_Gen6_EditValue_ImmediatelyUpdatesSave()
    {
        var sav = new SAV6XY();
        var vm = new RecordsEditorViewModel(sav);

        Assert.True(vm.HasRecords);
        Assert.NotEmpty(vm.Records);

        var record = vm.Records[0];
        var recordId = record.Id;
        var original = record.Value;

        var storage = (ITrainerStatRecord)sav;
        var newValue = original + 100;
        record.Value = newValue;

        // Live-write: OnValueChanged calls _storage.SetRecord immediately
        Assert.Equal(newValue, storage.GetRecord(recordId));
        output.WriteLine($"Gen6 record[{recordId}]: {original} → {newValue} immediately in save ✓");
    }

    // -----------------------------------------------------------------------
    // 4. Search filter narrows filtered records
    // -----------------------------------------------------------------------

    [Fact]
    public void Records_Gen7_SearchFilter_NarrowsResults()
    {
        var sav = BlankSaveFile.Get(GameVersion.SN);
        var vm = new RecordsEditorViewModel(sav);

        Assert.True(vm.HasRecords);
        var totalCount = vm.FilteredRecords.Count;
        Assert.True(totalCount > 0);

        // Filter to something that won't match anything
        vm.SearchText = "ZZZZNONEXISTENT9999";
        Assert.Empty(vm.FilteredRecords);

        // Clear filter → all records back
        vm.SearchText = string.Empty;
        Assert.Equal(totalCount, vm.FilteredRecords.Count);

        output.WriteLine($"Gen7: SearchFilter works, {totalCount} records total ✓");
    }

    // -----------------------------------------------------------------------
    // 5. RefreshRecordsCommand reloads without throwing
    // -----------------------------------------------------------------------

    [Fact]
    public void Records_Gen8_Refresh_DoesNotThrow()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var vm = new RecordsEditorViewModel(sav);

        var countBefore = vm.Records.Count;
        var ex = Record.Exception(() => vm.RefreshRecordsCommand.Execute(null));

        Assert.Null(ex);
        Assert.Equal(countBefore, vm.Records.Count);
        output.WriteLine($"Gen8: Refresh OK, {countBefore} records ✓");
    }

    // -----------------------------------------------------------------------
    // 6. FilteredRecords equals Records when search is empty on load
    // -----------------------------------------------------------------------

    [Fact]
    public void Records_Gen6_FilteredRecords_EqualsRecordsOnLoad()
    {
        var sav = BlankSaveFile.Get(GameVersion.X);
        var vm = new RecordsEditorViewModel(sav);

        Assert.True(vm.HasRecords);
        Assert.Equal(vm.Records.Count, vm.FilteredRecords.Count);
        output.WriteLine($"Gen6: FilteredRecords={vm.FilteredRecords.Count} == Records={vm.Records.Count} ✓");
    }
}
