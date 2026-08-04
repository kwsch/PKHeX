using System.Text;
using PKHeX.Core;

namespace PKHeX.Application.UseCases;

/// <summary>
/// Serializes every non-empty slot of a box (or all boxes) as blank-line-separated Showdown sets,
/// in box/slot order. Returns an empty string when no Pokémon are present.
/// </summary>
public sealed class ExportShowdownBoxUseCase
{
    public string Execute(SaveFile sav, int box)
    {
        var sb = new StringBuilder();
        AppendBox(sb, sav, box);
        return sb.ToString();
    }

    public string ExecuteAll(SaveFile sav)
    {
        var sb = new StringBuilder();
        for (int box = 0; box < sav.BoxCount; box++)
            AppendBox(sb, sav, box);
        return sb.ToString();
    }

    private static void AppendBox(StringBuilder sb, SaveFile sav, int box)
    {
        for (int slot = 0; slot < sav.BoxSlotCount; slot++)
        {
            var pk = sav.GetBoxSlotAtIndex(box, slot);
            if (pk.Species == 0)
                continue;
            if (sb.Length > 0)
                sb.Append('\n').Append('\n');
            sb.Append(new ShowdownSet(pk).Text);
        }
    }
}
