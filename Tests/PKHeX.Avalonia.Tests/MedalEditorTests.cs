using System.Linq;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class MedalEditorTests
{
    private static SAV5B2W2 NewSave() => new();

    [Fact]
    public void Medal_NonB2W2_IsNotSupported()
    {
        var vm = new MedalEditorViewModel(new SAV5BW());
        Assert.False(vm.IsSupported);
        Assert.Empty(vm.MedalRows);
        Assert.Empty(vm.HabitatRows);
    }

    [Fact]
    public void Medal_B2W2_LoadsAllRowsAndHabitat()
    {
        var vm = new MedalEditorViewModel(NewSave());

        Assert.True(vm.IsSupported);
        Assert.Equal(255, vm.MedalRows.Count);
        Assert.Equal(HabitatList5.HabitatCount, vm.HabitatRows.Count);
        Assert.Equal(5, vm.Categories.Count);
        Assert.Equal(255, vm.Categories.Sum(c => c.Medals.Count));
    }

    [Fact]
    public void Medal_EditState_WritesThroughToSave()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);

        var row = vm.MedalRows[10];
        row.StateIndex = (int)MedalState5.Obtained;

        Assert.Equal(MedalState5.Obtained, sav.Medals[10].State);
        Assert.True(row.IsObtained);
        Assert.True(row.CanHaveDate);
        // Stamping a date is automatic when transitioning into a date-capable obtained state.
        Assert.NotEqual(string.Empty, row.Date);
        Assert.Equal(1, vm.TotalObtained);
    }

    [Fact]
    public void Medal_UnreadFlag_WritesThroughToSave()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);

        vm.MedalRows[3].IsUnread = true;
        Assert.True(sav.Medals[3].IsUnread);

        vm.MedalRows[3].IsUnread = false;
        Assert.False(sav.Medals[3].IsUnread);
    }

    [Fact]
    public void Medal_Date_DisabledWhenStateCannotHaveDate()
    {
        var vm = new MedalEditorViewModel(NewSave());
        var row = vm.MedalRows[0];
        Assert.Equal((int)MedalState5.Unobtained, row.StateIndex);
        Assert.False(row.CanHaveDate);
    }

    [Fact]
    public void Medal_GiveAll_ObtainsEverythingAndSetsRank()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);

        vm.GiveAllCommand.Execute(null);

        Assert.Equal(255, vm.TotalObtained);
        Assert.Equal(255, sav.Medals.GetCountObtained());
        Assert.Equal((int)MedalRank5.Legend, vm.RankIndex);
        Assert.True(vm.MedalRows.All(r => r.IsObtained));
    }

    [Fact]
    public void Medal_ClearAll_ResetsEverything()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);
        vm.GiveAllCommand.Execute(null);

        vm.ClearAllCommand.Execute(null);

        Assert.Equal(0, vm.TotalObtained);
        Assert.Equal(0, sav.Medals.GetCountObtained());
        Assert.True(vm.MedalRows.All(r => !r.IsObtained));
    }

    [Fact]
    public void Medal_Settings_WriteThroughToSave()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);

        vm.PinnedMedal = 42;
        vm.RankIndex = (int)MedalRank5.Master;
        vm.IsTutorialComplete = true;

        Assert.Equal(42, sav.Medals.PinnedMedal);
        Assert.Equal(MedalRank5.Master, sav.Medals.Rank);
        Assert.True(sav.Medals.IsTutorialComplete);
    }

    [Fact]
    public void Medal_RecalculateRank_MatchesObtainedCount()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);

        // Obtain 100 medals -> Elite rank (>=100).
        for (int i = 0; i < 100; i++)
            vm.MedalRows[i].StateIndex = (int)MedalState5.Obtained;

        vm.RankIndex = (int)MedalRank5.None; // wrong on purpose
        vm.RecalculateRankCommand.Execute(null);

        Assert.Equal((int)MedalRank5.Elite, vm.RankIndex);
        Assert.Equal(MedalRank5.Elite, sav.Medals.Rank);
    }

    [Fact]
    public void Habitat_EditStatus_WritesThroughToSave()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);

        var row = vm.HabitatRows[5];
        row.GrassIndex = (int)HabitatCompletion5.Complete;
        row.SurfIndex = (int)HabitatCompletion5.Caught;
        row.FishIndex = (int)HabitatCompletion5.Seen;
        row.IsComplete = true;

        var habitat = sav.Medals.HabitatList.GetHabitat(5);
        Assert.Equal(HabitatCompletion5.Complete, habitat.GetStatus(HabitatEncounterType5.Grass));
        Assert.Equal(HabitatCompletion5.Caught, habitat.GetStatus(HabitatEncounterType5.Surf));
        Assert.Equal(HabitatCompletion5.Seen, habitat.GetStatus(HabitatEncounterType5.Fish));
        Assert.True(habitat.IsComplete);
    }

    [Fact]
    public void Habitat_CompleteAll_ThenClearAll()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);

        vm.HabitatCompleteAllCommand.Execute(null);
        Assert.True(vm.HabitatRows.All(r => r.IsComplete));
        for (int i = 0; i < HabitatList5.HabitatCount; i++)
            Assert.True(sav.Medals.HabitatList.GetHabitat(i).IsComplete);

        vm.HabitatClearAllCommand.Execute(null);
        Assert.True(vm.HabitatRows.All(r => !r.IsComplete));
        for (int i = 0; i < HabitatList5.HabitatCount; i++)
            Assert.False(sav.Medals.HabitatList.GetHabitat(i).IsComplete);
    }

    [Fact]
    public void Habitat_Settings_WriteThroughToSave()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);

        vm.HabitatTutorialViewed = true;
        vm.HabitatTutorialCompleteCapture = true;
        vm.LastEncounterTypeIndex = (int)HabitatEncounterType5.Surf;

        var habitat = sav.Medals.HabitatList;
        Assert.True(habitat.IsTutorialViewed);
        Assert.True(habitat.IsTutorialCompleteCapture);
        Assert.Equal(HabitatEncounterType5.Surf, habitat.LastEncounterType);
    }

    [Fact]
    public void Medal_ImportExportBlob_RoundTrips()
    {
        var sav = NewSave();
        var vm = new MedalEditorViewModel(sav);
        vm.GiveAllCommand.Execute(null);

        // Snapshot the exported blob, mutate, then restore via the same span surface used by import/export.
        var exported = sav.Medals.AllMedals.ToArray();
        Assert.Equal(MedalList5.LengthAllMedals, exported.Length);

        vm.ClearAllCommand.Execute(null);
        Assert.Equal(0, sav.Medals.GetCountObtained());

        exported.AsSpan().CopyTo(sav.Medals.AllMedals);
        Assert.Equal(255, sav.Medals.GetCountObtained());
    }
}
