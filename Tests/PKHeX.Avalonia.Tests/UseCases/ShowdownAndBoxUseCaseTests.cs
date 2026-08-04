using PKHeX.Application.UseCases;
using PKHeX.Core;

namespace PKHeX.Avalonia.Tests.UseCases;

public class ShowdownAndBoxUseCaseTests
{
    private static SaveFile NewSav() => new SAV8SWSH();

    [Fact]
    public void ImportShowdown_parses_valid_set()
    {
        var result = new ImportShowdownSetUseCase().Execute(NewSav(), "Pikachu\nLevel: 50");
        Assert.True(result.Success);
        Assert.NotNull(result.Pokemon);
        Assert.Equal((ushort)25, result.Pokemon!.Species);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ImportShowdown_rejects_empty(string? text)
    {
        var result = new ImportShowdownSetUseCase().Execute(NewSav(), text);
        Assert.False(result.Success);
        Assert.Null(result.Pokemon);
    }

    [Fact]
    public void ImportShowdown_rejects_garbage()
        => Assert.False(new ImportShowdownSetUseCase().Execute(NewSav(), "not a pokemon set").Success);

    [Fact]
    public void ExportShowdown_roundtrips_from_import()
    {
        var sav = NewSav();
        var imported = new ImportShowdownSetUseCase().Execute(sav, "Pikachu\nLevel: 50");
        var text = new ExportShowdownSetUseCase().Execute(imported.Pokemon!);
        Assert.NotNull(text);
        Assert.Contains("Pikachu", text!);
    }

    [Fact]
    public void ExportShowdown_returns_null_for_empty_slot()
        => Assert.Null(new ExportShowdownSetUseCase().Execute(NewSav().BlankPKM));

    [Fact]
    public void LoadBoxes_from_missing_path_reports_failure()
    {
        var result = new LoadBoxesUseCase().Execute(NewSav(), Path.Combine(Path.GetTempPath(), "pkhex-nope-" + Guid.NewGuid()));
        Assert.False(result.Success);
    }
}
