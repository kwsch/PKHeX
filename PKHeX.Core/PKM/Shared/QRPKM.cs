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
            if ((uint) pkm.Species < s.Species.Count)
            {
                var name = s.Species[pkm.Species];
                if (pkm.Nickname != name)
                    filename += $" ({name})";
            }
            yield return filename;

            if (pkm.Format >= 3 && (uint)pkm.Ability < s.Ability.Count)
                yield return $"[{s.Ability[pkm.Ability]}]";

            var level = pkm.Stat_Level;
            if (level == 0)
                level = pkm.CurrentLevel;
            yield return $"lv{level}";

            if (pkm.HeldItem > 0)
            {
                var items = s.GetItemStrings(pkm.Format);
                if ((uint)pkm.HeldItem < items.Count)
                    yield return $" @ {items[pkm.HeldItem]}";
            }

            if (pkm.Format >= 3 && (uint)pkm.Nature < s.Natures.Count)
                yield return s.natures[pkm.Nature];
        }
    }
}
