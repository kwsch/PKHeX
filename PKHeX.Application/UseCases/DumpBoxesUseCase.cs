using PKHeX.Core;

namespace PKHeX.Application.UseCases;

public readonly record struct DumpBoxesResult(bool Success, int Count, string Message);

/// <summary>
/// Dumps all box Pokémon to a folder as individual files. Wraps Core's <see cref="BoxUtil.DumpBoxes"/>.
/// </summary>
public sealed class DumpBoxesUseCase
{
    public DumpBoxesResult Execute(SaveFile sav, string path)
    {
        var count = sav.DumpBoxes(path);
        return count < 0
            ? new DumpBoxesResult(false, count, "This save file has no boxes to dump.")
            : new DumpBoxesResult(true, count, $"Dumped {count} Pokémon to {path}");
    }
}
