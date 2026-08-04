using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class JoinAvenueEditorTests
{
    [Fact]
    public void JoinAvenue_B2W2_LoadsAndIsSupported()
    {
        var sav = new SAV5B2W2();
        var vm = new JoinAvenueEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.Equal(JoinAvenue5.VisitorCount, vm.Visitors.Count);
        Assert.Equal(JoinAvenue5.OccupantCount, vm.Occupants.Count);
        Assert.Equal(JoinAvenue5.FanCount, vm.Fans.Count);
        Assert.Equal(JoinAvenue5.AssistantCount, vm.Assistants.Count);
        Assert.Equal(JoinAvenueSettings5.CountVisitingPlayersRemembered, vm.VisitingPlayers.Count);
        Assert.NotNull(vm.Self);
        Assert.NotEmpty(vm.CeilingColorList);
    }

    [Fact]
    public void JoinAvenue_IsSupportedSave_GatesCorrectly()
    {
        Assert.True(JoinAvenueEditorViewModel.IsSupportedSave(new SAV5B2W2()));
        Assert.False(JoinAvenueEditorViewModel.IsSupportedSave(new SAV5BW()));
        Assert.False(JoinAvenueEditorViewModel.IsSupportedSave(new SAV4HGSS()));
    }

    [Fact]
    public void JoinAvenue_ReflectsExistingSettings()
    {
        var sav = new SAV5B2W2();
        var settings = sav.JoinAvenue.Settings;
        settings.Rank = 1234;
        settings.Experience = 555_000;
        settings.CeilingColor = JoinAvenueCeilingColor5.Green;

        var vm = new JoinAvenueEditorViewModel(sav);

        Assert.Equal(1234, vm.Rank);
        Assert.Equal(555_000, vm.Experience);
        Assert.Equal((int)JoinAvenueCeilingColor5.Green, vm.CeilingColorIndex);
    }

    [Fact]
    public void JoinAvenue_Settings_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new JoinAvenueEditorViewModel(sav);

        vm.Rank = 4321;
        vm.Experience = 9000;
        vm.CeilingColorIndex = (int)JoinAvenueCeilingColor5.Blue;
        vm.IsPromotionActive = true;
        vm.ScriptFlag = true;

        Assert.Equal(4321, sav.JoinAvenue.Settings.Rank);
        Assert.Equal(9000u, sav.JoinAvenue.Settings.Experience);
        Assert.Equal(JoinAvenueCeilingColor5.Blue, sav.JoinAvenue.Settings.CeilingColor);
        Assert.True(sav.JoinAvenue.Settings.IsPromotionActive);
        Assert.True(sav.JoinAvenue.ScriptFlag);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void JoinAvenue_Rank_ClampsToMax()
    {
        var sav = new SAV5B2W2();
        var vm = new JoinAvenueEditorViewModel(sav);

        vm.Rank = 999_999;
        Assert.Equal(JoinAvenueSettings5.MaxAvenueRank, sav.JoinAvenue.Settings.Rank);
    }

    [Fact]
    public void JoinAvenue_VisitingPlayer_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new JoinAvenueEditorViewModel(sav);

        vm.VisitingPlayers[3].Value = 0x12345678;
        Assert.Equal(0x12345678u, sav.JoinAvenue.Settings.GetVisitingPlayerTrainerID(3));
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void JoinAvenue_Visitor_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new JoinAvenueEditorViewModel(sav);

        var visitor = vm.Visitors[2];
        visitor.Name = "Tester";
        visitor.JoinAvenueLevel = 7;
        visitor.ShopExperience = 250;
        visitor.MedalCount = 13;
        visitor.Favorite!.Species = (int)Species.Pikachu;

        var live = sav.JoinAvenue.GetVisitor(2);
        Assert.Equal("Tester", live.Name);
        Assert.Equal(7, live.JoinAvenueLevel);
        Assert.Equal(250, live.ShopExperience);
        Assert.Equal(13, live.MedalCount);
        Assert.Equal((ushort)Species.Pikachu, live.FavoriteSpecies);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void JoinAvenue_Fan_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new JoinAvenueEditorViewModel(sav);

        var fan = vm.Fans[1];
        fan.Name = "FanGuy";
        fan.BubbleTarget = 5;
        fan.FanSpecies!.Species = (int)Species.Snivy;

        var live = sav.JoinAvenue.GetFan(1);
        Assert.Equal("FanGuy", live.Name);
        Assert.Equal(5, live.BubbleTarget);
        Assert.Equal((ushort)Species.Snivy, live.Species);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void JoinAvenue_Assistant_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new JoinAvenueEditorViewModel(sav);

        var assistant = vm.Assistants[0];
        assistant.Name = "Helper";
        assistant.Position0 = 9;

        var live = sav.JoinAvenue.GetAssistant(0);
        Assert.Equal("Helper", live.Name);
        Assert.Equal(9, live.Position0);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void JoinAvenue_Self_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new JoinAvenueEditorViewModel(sav);

        vm.Self.Name = "Avenue";
        vm.Self.JoinAvenueRank = 4;

        Assert.Equal("Avenue", sav.JoinAvenue.Self.Name);
        Assert.Equal(4, sav.JoinAvenue.Self.JoinAvenueRank);
        Assert.True(sav.State.Edited);
    }
}
