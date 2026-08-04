using PKHeX.Application.UseCases;
using PKHeX.Core;

namespace PKHeX.Avalonia.Tests;

public class ShowdownTeamTests
{
    private const string TwoSets = """
        Mudkip
        Level: 5
        - Tackle

        Torchic
        Level: 5
        - Scratch
        """;

    [Fact]
    public void ImportTeam_PlacesSetsInFirstEmptySlots()
    {
        var sav = new SAV3E();
        var result = new ImportShowdownTeamUseCase().Execute(sav, TwoSets, box: 0);

        Assert.True(result.Success);
        Assert.Equal(2, result.Imported);
        Assert.Empty(result.SetErrors);
        Assert.Equal((ushort)Species.Mudkip, sav.GetBoxSlotAtIndex(0, 0).Species);
        Assert.Equal((ushort)Species.Torchic, sav.GetBoxSlotAtIndex(0, 1).Species);
    }

    [Fact]
    public void ImportTeam_SkipsOccupiedSlots()
    {
        var sav = new SAV3E();
        sav.SetBoxSlotAtIndex(new PK3 { Species = (ushort)Species.Treecko }, 0, 0);

        var result = new ImportShowdownTeamUseCase().Execute(sav, TwoSets, box: 0);

        Assert.True(result.Success);
        Assert.Equal((ushort)Species.Treecko, sav.GetBoxSlotAtIndex(0, 0).Species);
        Assert.Equal((ushort)Species.Mudkip, sav.GetBoxSlotAtIndex(0, 1).Species);
        Assert.Equal((ushort)Species.Torchic, sav.GetBoxSlotAtIndex(0, 2).Species);
    }

    [Fact]
    public void ImportTeam_ReportsMalformedSet_ImportsRest()
    {
        var text = TwoSets + "\n\nNotARealSpeciesXyz\n- Fake Move";
        var sav = new SAV3E();

        var result = new ImportShowdownTeamUseCase().Execute(sav, text, box: 0);

        Assert.True(result.Success);
        Assert.Equal(2, result.Imported);
        var error = Assert.Single(result.SetErrors);
        Assert.StartsWith("Set 3:", error);
    }

    [Fact]
    public void ImportTeam_AllOrNothing_WhenBoxLacksSpace()
    {
        var sav = new SAV3E();
        for (int slot = 0; slot < sav.BoxSlotCount - 1; slot++)
            sav.SetBoxSlotAtIndex(new PK3 { Species = (ushort)Species.Zigzagoon }, 0, slot);

        var result = new ImportShowdownTeamUseCase().Execute(sav, TwoSets, box: 0);

        Assert.False(result.Success);
        Assert.Equal(0, result.Imported);
        Assert.Contains("Not enough empty slots", result.FatalError);
        Assert.Equal(0, sav.GetBoxSlotAtIndex(0, sav.BoxSlotCount - 1).Species); // last slot untouched
    }

    [Fact]
    public void ImportTeam_EmptyClipboard_Fails()
    {
        var result = new ImportShowdownTeamUseCase().Execute(new SAV3E(), "  ", box: 0);
        Assert.False(result.Success);
    }

    [Fact]
    public void ExportBox_RoundTripsThroughImport()
    {
        var sav = new SAV3E();
        Assert.True(new ImportShowdownTeamUseCase().Execute(sav, TwoSets, box: 0).Success);

        var text = new ExportShowdownBoxUseCase().Execute(sav, box: 0);
        Assert.Contains("Mudkip", text);
        Assert.Contains("Torchic", text);

        // Re-import the exported text into another box: same species, same count.
        var result = new ImportShowdownTeamUseCase().Execute(sav, text, box: 1);
        Assert.True(result.Success);
        Assert.Equal(2, result.Imported);
        Assert.Equal((ushort)Species.Mudkip, sav.GetBoxSlotAtIndex(1, 0).Species);
    }

    [Fact]
    public void ExportAllBoxes_IncludesEveryBox()
    {
        var sav = new SAV3E();
        sav.SetBoxSlotAtIndex(new PK3 { Species = (ushort)Species.Mudkip, Move1 = (ushort)Move.Tackle }, 0, 0);
        sav.SetBoxSlotAtIndex(new PK3 { Species = (ushort)Species.Torchic, Move1 = (ushort)Move.Scratch }, 2, 5);

        var text = new ExportShowdownBoxUseCase().ExecuteAll(sav);

        Assert.Contains("Mudkip", text);
        Assert.Contains("Torchic", text);
    }

    [Fact]
    public void ExportBox_Empty_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, new ExportShowdownBoxUseCase().Execute(new SAV3E(), 0));
    }
}
