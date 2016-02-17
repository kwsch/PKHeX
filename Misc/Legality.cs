using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PKHeX
{
    public class LegalityChecker
    {
        private readonly EggMoves[] EggMoveXY = EggMoves.getArray(Util.unpackMini(Properties.Resources.eggmove_xy, "xy"));
        private readonly Learnset[] LevelUpXY = Learnset.getArray(Util.unpackMini(Properties.Resources.lvlmove_xy, "xy"));
        private readonly EggMoves[] EggMoveAO = EggMoves.getArray(Util.unpackMini(Properties.Resources.eggmove_ao, "ao"));
        private readonly Learnset[] LevelUpAO = Learnset.getArray(Util.unpackMini(Properties.Resources.lvlmove_ao, "ao"));
        private readonly Evolutions[] Evolves = Evolutions.getArray(Util.unpackMini(Properties.Resources.evos_ao, "ao"));
        private readonly PKX.PersonalInfo[] PersonalAO = PKX.getPersonalArray(Properties.Resources.personal_ao, PKX.PersonalInfo.SizeAO);
        private readonly PKX.PersonalInfo[] PersonalXY = PKX.getPersonalArray(Properties.Resources.personal_xy, PKX.PersonalInfo.SizeXY);

        public LegalityChecker()
        {
            Console.WriteLine("Initializing Legality Checker...");
        }
        public int[] getValidMoves(int species, int level)
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

        public int[] getValidRelearn(int species)
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
        private int[] getEggMoves(int species)
        {
            return EggMoveAO[species].Moves.Concat(EggMoveXY[species].Moves).ToArray();
        }
        private int[] getLVLMoves(int species, int lvl)
        {
            return LevelUpXY[species].getMoves(lvl).Concat(LevelUpAO[species].getMoves(lvl)).ToArray();
        }
        private int[] getTutorMoves(int species)
        {
            PKX.PersonalInfo pkAO = PersonalAO[species];

            // Type Tutor
            List<int> moves = Legal.AO_TypeTutor.Where((t, i) => pkAO.Tutors[i]).ToList();

            // Varied Tutors
            for (int i = 0; i < Legal.AO_Tutors.Length; i++)
                for (int b = 0; b < Legal.AO_Tutors[i].Length; b++)
                    if (pkAO.ORASTutors[i][b])
                        moves.Add(Legal.AO_Tutors[i][b]);

            return moves.ToArray();
        }
        private int[] getMachineMoves(int species)
        {
            PKX.PersonalInfo pkXY = PersonalXY[species];
            PKX.PersonalInfo pkAO = PersonalAO[species];
            List<int> moves = new List<int>();
            moves.AddRange(Legal.XY_TMHM.Where((t, i) => pkXY.TMHM[i]));
            moves.AddRange(Legal.AO_TMHM.Where((t, i) => pkAO.TMHM[i]));
            return moves.ToArray();
        }
    }

    public class Learnset
    {
        public readonly int Count;
        public readonly int[] Moves, Levels;

        public Learnset(byte[] data)
        {
            if (data.Length < 4 || data.Length % 4 != 0)
            { Count = 0; Levels = new int[0]; Moves = new int[0]; return; }
            Count = data.Length / 4 - 1;
            Moves = new int[Count];
            Levels = new int[Count];
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
                for (int i = 0; i < Count; i++)
                {
                    Moves[i] = br.ReadInt16();
                    Levels[i] = br.ReadInt16();
                }
        }

        public static Learnset[] getArray(byte[][] entries)
        {
            Learnset[] data = new Learnset[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new Learnset(entries[i]);
            return data;
        }

        public int[] getMoves(int level)
        {
            for (int i = 0; i > Levels.Length; i++)
                if (Levels[i] > level)
                    return Moves.Take(i).ToArray();
            return Moves;
        }
    }
    public class EggMoves
    {
        public readonly int Count;
        public readonly int[] Moves;

        public EggMoves(byte[] data)
        {
            if (data.Length < 2 || data.Length%2 != 0)
            { Count = 0; Moves = new int[0]; return; }
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
                Moves = new int[Count = br.ReadUInt16()];
                for (int i = 0; i < Count; i++)
                    Moves[i] = br.ReadUInt16();
            }
        }

        public static EggMoves[] getArray(byte[][] entries)
        {
            EggMoves[] data = new EggMoves[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EggMoves(entries[i]);
            return data;
        }
    }
    public class Evolutions
    {
        public readonly int[] Species, Level;

        public Evolutions(byte[] data)
        {
            int Count = data.Length / 4;
            Level = new int[Count];
            Species = new int[Count];
            if (data.Length < 4 || data.Length % 4 != 0) return;
            for (int i = 0; i < data.Length; i+=4)
            {
                Species[i/4] = BitConverter.ToUInt16(data, i);
                Level[i/4] = BitConverter.ToUInt16(data, i+2);
            }
        }

        public static Evolutions[] getArray(byte[][] entries)
        {
            Evolutions[] data = new Evolutions[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new Evolutions(entries[i]);
            return data;
        }
    }
}
