using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public static partial class Legal
    {
        // PKHeX master personal.dat
        internal static readonly PersonalInfo[] PersonalAO = PersonalInfo.getPersonalArray(Properties.Resources.personal_ao, PersonalInfo.SizeAO);

        private static readonly PersonalInfo[] PersonalXY = PersonalInfo.getPersonalArray(Properties.Resources.personal_xy, PersonalInfo.SizeXY);
        private static readonly EggMoves[] EggMoveXY = EggMoves.getArray(Util.unpackMini(Properties.Resources.eggmove_xy, "xy"));
        private static readonly Learnset[] LevelUpXY = Learnset.getArray(Util.unpackMini(Properties.Resources.lvlmove_xy, "xy"));
        private static readonly EggMoves[] EggMoveAO = EggMoves.getArray(Util.unpackMini(Properties.Resources.eggmove_ao, "ao"));
        private static readonly Learnset[] LevelUpAO = Learnset.getArray(Util.unpackMini(Properties.Resources.lvlmove_ao, "ao"));
        private static readonly Evolutions[] Evolves = Evolutions.getArray(Util.unpackMini(Properties.Resources.evos_ao, "ao"));
        private static readonly DexNavLocations[] DexNavAO = DexNavLocations.getArray(Util.unpackMini(Properties.Resources.dexnav_ao, "ao"));

        internal static bool getDexNavValid(int species, int location, int level)
        {
            DexNavLocations[] locs = DexNavAO.Where(l => l.Location == location).ToArray();
            return locs.Any(loc => loc.Slots.Any(slot => slot.Species == species && slot.LevelMin <= level));
        }

        internal static int[] getValidMoves(int species, int level)
        {
            int[] r = new int[1];

            // r = r.Concat(getEggMoves(species)).ToArray();
            r = r.Concat(getLVLMoves(species, level)).ToArray();
            r = r.Concat(getTutorMoves(species)).ToArray();
            r = r.Concat(getMachineMoves(species)).ToArray();
            Evolutions e = Evolves[species];
            int dec = 0;
            for (int i = 0; i < e.Species.Length; i++)
            {
                if (e.Level[i] == 1) // In order to level up evolve, the list of available moves is for one level previous.
                    dec++;
                r = r.Concat(getLVLMoves(e.Species[i], level - dec)).ToArray();
                r = r.Concat(getTutorMoves(e.Species[i])).ToArray();
                r = r.Concat(getMachineMoves(e.Species[i])).ToArray();
            }
            return r.Distinct().ToArray();
        }
        internal static int[] getValidRelearn(int species)
        {
            int[] moves = new int[0];
            foreach (int spec in Evolves[species].Species)
            {
                moves = moves.Concat(getLVLMoves(spec, 1)).ToArray();
                moves = moves.Concat(getEggMoves(spec)).ToArray();
                moves = moves.Concat(getLVLMoves(spec, 100)).ToArray();
            }
            moves = moves.Concat(getLVLMoves(species, 1)).ToArray();
            moves = moves.Concat(getEggMoves(species)).ToArray();
            moves = moves.Concat(getLVLMoves(species, 100)).ToArray();
            return moves.Concat(new int[1]).Distinct().ToArray();
            // Not implemented: DexNav exclusive moves, Wonder Cards
        }

        private static int[] getEggMoves(int species)
        {
            return EggMoveAO[species].Moves.Concat(EggMoveXY[species].Moves).ToArray();
        }
        private static int[] getLVLMoves(int species, int lvl)
        {
            return LevelUpXY[species].getMoves(lvl).Concat(LevelUpAO[species].getMoves(lvl)).ToArray();
        }
        private static int[] getTutorMoves(int species)
        {
            PersonalInfo pkAO = PersonalAO[species];

            // Type Tutor
            List<int> moves = TypeTutor.Where((t, i) => pkAO.Tutors[i]).ToList();

            // Varied Tutors
            for (int i = 0; i < Tutors_AO.Length; i++)
                for (int b = 0; b < Tutors_AO[i].Length; b++)
                    if (pkAO.ORASTutors[i][b])
                        moves.Add(Tutors_AO[i][b]);

            return moves.ToArray();
        }
        private static int[] getMachineMoves(int species)
        {
            PersonalInfo pkXY = PersonalXY[species];
            PersonalInfo pkAO = PersonalAO[species];
            List<int> moves = new List<int>();
            moves.AddRange(TMHM_XY.Where((t, i) => pkXY.TMHM[i]));
            moves.AddRange(TMHM_AO.Where((t, i) => pkAO.TMHM[i]));
            return moves.ToArray();
        }
    }
}
