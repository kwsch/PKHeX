using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Logic for encoding and decoding QR codes used to visually transfer data between other applications.
/// </summary>
public static class QRPKM
{
    /// <summary>
    /// Summarizes the details of a <see cref="PKM"/> into multiple lines for display in an image.
    /// </summary>
    /// <param name="pk">Pokémon to generate details for.</param>
    /// <returns>Lines representing a Header, Moves, and IVs/EVs</returns>
    public static string[] GetQRLines(this PKM pk)
    {
        var s = GameInfo.Strings;

        var header = GetHeader(pk, s);
        var sb = new StringBuilder(48);
        for (int i = 0; i < 4; i++)
        {
            var move = pk.GetMove(i);
            if (move == 0)
                continue;
            if (sb.Length != 0)
                sb.Append(Moveset.DefaultSeparator);
            var moveName = move < s.movelist.Length ? s.movelist[move] : "ERROR";
            sb.Append(moveName);
        }

        string IVs = $"IVs: {pk.IV_HP:00}/{pk.IV_ATK:00}/{pk.IV_DEF:00}/{pk.IV_SPA:00}/{pk.IV_SPD:00}/{pk.IV_SPE:00}";
        string EVs = $"EVs: {pk.EV_HP:00}/{pk.EV_ATK:00}/{pk.EV_DEF:00}/{pk.EV_SPA:00}/{pk.EV_SPD:00}/{pk.EV_SPE:00}";

        return
        [
            string.Join(' ', header),
            sb.ToString(),
            IVs + "   " + EVs,
        ];
    }

    private static IEnumerable<string> GetHeader(PKM pk, GameStrings s)
    {
        string filename = pk.Nickname;
        if ((uint) pk.Species < s.Species.Count)
        {
            var name = s.Species[pk.Species];
            if (pk.Nickname != name)
                filename += $" ({name})";
        }
        yield return filename;

        if (pk.Format >= 3 && (uint)pk.Ability < s.Ability.Count)
            yield return $"[{s.Ability[pk.Ability]}]";

        var level = pk.Stat_Level;
        if (level == 0)
            level = pk.CurrentLevel;
        yield return $"lv{level}";

        if (pk.HeldItem > 0)
        {
            var items = s.GetItemStrings(pk.Context);
            if ((uint)pk.HeldItem < items.Length)
                yield return $" @ {items[pk.HeldItem]}";
        }

        if (pk.Format >= 3 && (uint)pk.Nature < s.Natures.Count)
            yield return s.natures[(byte)pk.Nature];
    }
}
