using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class PokeathlonEditorTests
{
    [Fact]
    public void Pokeathlon_HGSS_LoadsAndIsSupported()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.Equal(PokeathlonEditorViewModel.MaxSpeciesGen4, vm.Medals.Count);
        Assert.Equal((int)PokeathlonStat4.Count, vm.Courses.Count);
        Assert.Equal((int)PokeathlonEvent4.Count, vm.SelfEvents.Count);
        Assert.Equal((int)PokeathlonEvent4.Count, vm.Connections.Count);
        Assert.Equal((int)PokeathlonEvent4.Count, vm.EventStats.Count);
        Assert.Equal(PokeathlonEditorViewModel.DailyShopBits, vm.DailyShop.Count);
        Assert.Equal((int)DataCard4.Count, vm.DataCards.Count);
    }

    [Fact]
    public void Pokeathlon_LoadingDoesNotMarkEdited()
    {
        var sav = new SAV4HGSS();
        _ = new PokeathlonEditorViewModel(sav);

        Assert.False(sav.State.Edited);
    }

    [Fact]
    public void Pokeathlon_IsSupportedGating_OnlyHGSS()
    {
        Assert.True(PokeathlonEditorViewModel.IsSupportedSave(new SAV4HGSS()));

        // Other Gen 4 variants (no Pokéathlon block) and unrelated saves are not supported.
        Assert.False(PokeathlonEditorViewModel.IsSupportedSave(new SAV4DP()));
        Assert.False(PokeathlonEditorViewModel.IsSupportedSave(new SAV4Pt()));
        Assert.False(PokeathlonEditorViewModel.IsSupportedSave(new SAV5B2W2()));
    }

    [Fact]
    public void Pokeathlon_Points_ReflectsExistingValue()
    {
        var sav = new SAV4HGSS();
        sav.Pokeathlon.Points = 12345;

        var vm = new PokeathlonEditorViewModel(sav);

        Assert.Equal(12345, vm.Points);
    }

    [Fact]
    public void Pokeathlon_Points_WriteThroughAndClamp()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        vm.Points = 50_000;
        Assert.Equal(50_000u, sav.Pokeathlon.Points);
        Assert.True(sav.State.Edited);

        // Core clamps to MaxPoints.
        vm.Points = 999_999;
        Assert.Equal(Pokeathlon4.MaxPoints, sav.Pokeathlon.Points);
    }

    [Fact]
    public void Pokeathlon_Counter_WriteThroughToSave()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        vm.PlacedFirst = 777;
        vm.Fame = 4242;

        Assert.Equal(777u, sav.Pokeathlon.GlobalCounters.PlacedFirst);
        Assert.Equal(4242u, sav.Pokeathlon.GlobalCounters.Fame);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void Pokeathlon_EventStat_WriteThroughToSave()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        var hurdle = vm.EventStats[(int)PokeathlonEvent4.HurdleDash];
        hurdle.FirstPlaces = 9;
        hurdle.BestScore = 555;

        Assert.Equal(9u, sav.Pokeathlon.GlobalCounters[PokeathlonEvent4.HurdleDash]);
        Assert.Equal(555, sav.Pokeathlon.GetBestScore(PokeathlonEvent4.HurdleDash));
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void Pokeathlon_SelfEventAttempts_WriteThroughToSave()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        var selfEvent = vm.SelfEvents[(int)PokeathlonEvent4.BlockSmash];
        selfEvent.Attempts = 1234;

        Assert.Equal(1234u, sav.Pokeathlon.GetEventSelf(PokeathlonEvent4.BlockSmash).Attempts);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void Pokeathlon_CourseParticipant_SpeciesFormWriteThrough()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        var course = vm.Courses[(int)PokeathlonStat4.Speed];
        var participant = course.Participants[0];
        participant.Species = (int)Species.Pikachu; // 25
        participant.IsShiny = true;
        participant.Tid16 = 0x1234;

        var saved = sav.Pokeathlon.GetCourseRecord(PokeathlonStat4.Speed).GetParticipant(0);
        Assert.Equal((ushort)Species.Pikachu, saved.Species);
        Assert.True(saved.IsShiny);
        Assert.Equal((ushort)0x1234, saved.TID16);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void Pokeathlon_EventRecordEntry_SpeciesFormWriteThrough()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        var record = vm.SelfEvents[(int)PokeathlonEvent4.RelayRun].Records[0];
        record.Record = 4321;
        record.Entry0.Species = (int)Species.Bulbasaur; // 1

        var saved = sav.Pokeathlon.GetEventSelf(PokeathlonEvent4.RelayRun).GetRecord(0);
        Assert.Equal((ushort)4321, saved.Record);
        Assert.Equal((ushort)Species.Bulbasaur, saved.Entry0.Species);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void Pokeathlon_ConnectionTrainer_WriteThroughToSave()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        var connection = vm.Connections[(int)PokeathlonEvent4.SnowThrow];
        connection.Attempts = 88;
        var trainer = connection.Trainers[0];
        trainer.Sid16 = 4567;

        var savedConn = sav.Pokeathlon.GetEventConnection(PokeathlonEvent4.SnowThrow);
        Assert.Equal(88u, savedConn.Inner.Attempts);
        Assert.Equal((ushort)4567, savedConn.GetTrainer(0).SID16);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void Pokeathlon_Medal_WriteThroughToSave()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        var medal = vm.Medals[0]; // species 1 (Bulbasaur)
        Assert.Equal(1, medal.Species);
        medal.Speed = true;
        medal.Jump = true;

        // Speed = bit0 (0x01), Jump = bit4 (0x10) => 0x11
        Assert.Equal(0x11, sav.Pokeathlon.Medals.GetMedal(1));
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void Pokeathlon_GiveAllMedals_SetsEverySpecies()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        vm.GiveAllMedalsCommand.Execute(null);

        Assert.True(sav.State.Edited);
        Assert.Equal(PokeathlonMedalManager4.MaxMedalBits, sav.Pokeathlon.Medals.GetMedal(1));
        Assert.Equal(PokeathlonMedalManager4.MaxMedalBits, sav.Pokeathlon.Medals.GetMedal((ushort)PokeathlonEditorViewModel.MaxSpeciesGen4));

        // VM rows reflect the change.
        Assert.True(vm.Medals[0].Speed);
        Assert.True(vm.Medals[0].Jump);
    }

    [Fact]
    public void Pokeathlon_DataCardFlags_WriteThroughToSave()
    {
        var sav = new SAV4HGSS();
        var vm = new PokeathlonEditorViewModel(sav);

        vm.DataCards[0].IsChecked = true;
        vm.DataCards[3].IsChecked = true;

        var flags = sav.Pokeathlon.FlagsDataCard;
        Assert.True((flags & 1u) != 0);
        Assert.True((flags & (1u << 3)) != 0);
        Assert.True(sav.State.Edited);

        vm.GiveAllDataCardsCommand.Execute(null);
        Assert.Equal(Pokeathlon4.DataCardAllObtained, sav.Pokeathlon.FlagsDataCard);
        Assert.True(vm.DataCards[10].IsChecked);
    }
}
