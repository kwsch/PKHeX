using PKHeX.Core;

namespace PKHeX.Application.UseCases;

public readonly record struct LoadBoxesResult(bool Success, int Count, string Message);

/// <summary>
/// Loads box Pokémon from a folder, handling format conversion. Wraps Core's
/// <see cref="BoxUtil.LoadBoxes"/>.
/// </summary>
public sealed class LoadBoxesUseCase
{
    public LoadBoxesResult Execute(SaveFile sav, string path)
    {
        var count = sav.LoadBoxes(path, out var result, all: true);
        return new LoadBoxesResult(count >= 0, count, result);
    }
}
