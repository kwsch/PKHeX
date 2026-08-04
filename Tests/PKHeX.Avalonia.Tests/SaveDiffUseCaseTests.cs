using PKHeX.Application.UseCases;
using PKHeX.Core;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Covers the save-comparison behaviour required by issue #135: categorized diffs on synthetic saves,
/// and graceful rejection when comparing saves from different/incompatible games.
/// </summary>
public class SaveDiffUseCaseTests
{
    [Fact]
    public void Execute_DifferentGames_FailsGracefully_WithClearMessage()
    {
        var left = SaveFileFactory.CreateBlankSave(GameVersion.W2);
        var right = SaveFileFactory.CreateBlankSave(GameVersion.SL);

        var result = new SaveDiffUseCase().Execute(left, right);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Empty(result.Changes);
    }

    [Fact]
    public void Execute_DifferentSaveFileTypes_FailsGracefully()
    {
        var left = SaveFileFactory.CreateBlankSave(GameVersion.RD);
        var right = SaveFileFactory.CreateBlankSave(GameVersion.E);

        var result = new SaveDiffUseCase().Execute(left, right);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Execute_SameSave_NoDifferences()
    {
        var sav = SaveFileFactory.CreateBlankSave(GameVersion.W2);
        var before = ReloadFromBytes(sav);
        var after = ReloadFromBytes(sav);

        var result = new SaveDiffUseCase().Execute(before, after);

        Assert.True(result.Success);
        Assert.Empty(result.Changes);
    }

    [Fact]
    public void Execute_MovedPokemonAndChangedMoney_DetectsExactlyThoseTwoChanges()
    {
        var sav = SaveFileFactory.CreateBlankSave(GameVersion.W2);
        var before = ReloadFromBytes(sav);

        // Mutate: place a Pokémon into box slot 0, and change the money amount.
        var pk = SaveFileFactory.CreateTestPKM(sav);
        var boxData = sav.BoxData;
        boxData[0] = pk;
        sav.BoxData = boxData;
        sav.Money = 12345;

        var after = ReloadFromBytes(sav);

        var result = new SaveDiffUseCase().Execute(before, after);

        Assert.True(result.Success);
        Assert.Equal(2, result.Changes.Count);
        Assert.Contains(result.Changes, c => c.Category == SaveDiffCategory.Trainer && c.Description == "Money");
        Assert.Contains(result.Changes, c => c.Category == SaveDiffCategory.BoxParty);
    }

    [Fact]
    public void Execute_ChangedTrainerName_DetectsTrainerChange()
    {
        var sav = SaveFileFactory.CreateBlankSave(GameVersion.W2);
        var before = ReloadFromBytes(sav);

        sav.OT = "Someone";

        var after = ReloadFromBytes(sav);

        var result = new SaveDiffUseCase().Execute(before, after);

        Assert.True(result.Success);
        Assert.Contains(result.Changes, c => c.Category == SaveDiffCategory.Trainer && c.Description == "Trainer Name");
    }

    /// <summary>
    /// Round-trips through Write/GetSupportedFile so "before" and "after" are independent snapshots
    /// rather than aliases of the same mutable <see cref="SaveFile"/> instance.
    /// </summary>
    private static SaveFile ReloadFromBytes(SaveFile sav)
    {
        var bytes = sav.Write().ToArray();
        return (SaveFile)FileUtil.GetSupportedFile(bytes, "test.sav")!;
    }
}
