using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class QRPKM
    {
        /// <summary>
        /// Summarizes the details of a <see cref="PKM"/> into multiple lines for display in an image.
        /// </summary>
        /// <param name="pkm">Pokémon to generate details for.</param>
        /// <returns>Lines representing a Header, Moves, and IVs/EVs</returns>
        public static string[] GetQRLines(this PKM pkm)
        {
            var s = GameInfo.Strings;

            var header = GetHeader(pkm, s);
            string moves = string.Join(" / ", pkm.Moves.Select(move => move < s.movelist.Length ? s.movelist[move] : "ERROR"));
            string IVs = $"IVs: {pkm.IV_HP:00}/{pkm.IV_ATK:00}/{pkm.IV_DEF:00}/{pkm.IV_SPA:00}/{pkm.IV_SPD:00}/{pkm.IV_SPE:00}";
            string EVs = $"EVs: {pkm.EV_HP:00}/{pkm.EV_ATK:00}/{pkm.EV_DEF:00}/{pkm.EV_SPA:00}/{pkm.EV_SPD:00}/{pkm.EV_SPE:00}";

            return new[]
            {
                string.Join(" ", header),
                moves,
                IVs + "   " + EVs,
            };
        }

        private static IEnumerable<string> GetHeader(PKM pkm, GameStrings s)
        {
            string filename = pkm.Nickname;
            if (pkm.Nickname != s.specieslist[pkm.Species] && s.specieslist[pkm.Species] != null)
                filename += $" ({s.specieslist[pkm.Species]})";
            yield return filename;

            if (pkm.Format >= 3)
                yield return $"[{s.abilitylist[pkm.Ability]}]";

            yield return $"lv{pkm.Stat_Level}";

            if (pkm.HeldItem > 0)
            {
                var str = s.GetItemStrings(pkm.Format);
                if (pkm.HeldItem < str.Count)
                    yield return $" @ {str[pkm.HeldItem]}";
            }

            if (pkm.Format >= 3)
                yield return s.natures[pkm.Nature];
        }
    }
}
