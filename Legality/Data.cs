using System;
using System.IO;
using System.Linq;

namespace PKHeX
{
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
            for (int i = 0; i < Levels.Length; i++)
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
            if (data.Length < 2 || data.Length % 2 != 0)
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
        public readonly DexLevel[] Evos = new DexLevel[0];

        public Evolutions(byte[] data)
        {
            int Count = data.Length / 4;
            if (data.Length < 4 || data.Length % 4 != 0) return;
            Evos = new DexLevel[Count];
            for (int i = 0; i < data.Length; i += 4)
            {
                Evos[i/4] = new DexLevel
                {
                    Species = BitConverter.ToUInt16(data, i),
                    Level = BitConverter.ToUInt16(data, i + 2)
                };
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
    public class DexLevel
    {
        public int Species;
        public int Level;
    }

    public class EncounterArea
    {
        public int Location;
        public EncounterSlot[] Slots;
        public EncounterArea() { }
        public EncounterArea(byte[] data)
        {
            Location = BitConverter.ToUInt16(data, 0);
            Slots = new EncounterSlot[(data.Length-2)/4];
            for (int i = 0; i < Slots.Length; i++)
            {
                ushort SpecForm = BitConverter.ToUInt16(data, 2 + i*4);
                Slots[i] = new EncounterSlot
                {
                    Species = SpecForm & 0x7FF,
                    Form = SpecForm >> 11,
                    LevelMin = data[4 + i*4],
                    LevelMax = data[5 + i*4],
                };
            }
        }
        public static EncounterArea[] getArray(byte[][] entries)
        {
            EncounterArea[] data = new EncounterArea[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EncounterArea(entries[i]);
            return data;
        }
    }

    public class EncounterSlot
    {
        public int Species;
        public int Form;
        public int LevelMin;
        public int LevelMax;
    }
    public class EncounterStatic
    {
        public int Species;
        public int Level;

        public int Location = 0;
        public int Ability = 0;
        public int Form = 0;
        public bool? Shiny = null; // false = never, true = always, null = possible
        public int[] Relearn = new int[4];
        public int Gender = -1;
        public int EggLocation = 0;
        public Nature Nature = Nature.Random;
        public bool Gift = false;
        public GameVersion Version = GameVersion.Any;
        public int[] IVs = {-1, -1, -1, -1, -1, -1};
        public int[] Contest = {0, 0, 0, 0, 0, 0};
    }
    public class EncounterTrade
    {
        public int Species;
        public int Level;

        public int Location = 30001;
        public int Ability = 0;
        public Nature Nature = Nature.Random;
        public int TID;
        public int SID = 0;
        public int[] IVs = { -1, -1, -1, -1, -1, -1 };
        public int[] Moves;
        public int Form = 0;
        public bool? Shiny = false;
        public int Gender = -1;
    }
    public enum Nature
    {
        Random = -1,
        Hardy, Lonely, Brave, Adamant, Naughty, Bold,
        Docile, Relaxed, Impish, Lax, Timid, Hasty,
        Serious, Jolly, Naive, Modest, Mild, Quiet,
        Bashful, Rash, Calm, Gentle, Sassy, Careful,
        Quirky,
    }
    public class PersonalInfo
    {
        internal static int SizeAO = 0x50;
        internal static int SizeXY = 0x40;
        public byte HP, ATK, DEF, SPE, SPA, SPD;
        public int BST;
        public int EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD;
        public byte[] Types = new byte[2];
        public byte CatchRate, EvoStage;
        public ushort[] Items = new ushort[3];
        public byte Gender, HatchCycles, BaseFriendship, EXPGrowth;
        public byte[] EggGroups = new byte[2];
        public byte[] Abilities = new byte[3];
        public ushort FormStats, FormeSprite, BaseEXP;
        public byte FormeCount, Color;
        public float Height, Weight;
        public bool[] TMHM;
        public bool[] Tutors;
        public bool[][] ORASTutors = new bool[4][];
        public byte EscapeRate;

        public PersonalInfo(byte[] data)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
                HP = br.ReadByte(); ATK = br.ReadByte(); DEF = br.ReadByte();
                SPE = br.ReadByte(); SPA = br.ReadByte(); SPD = br.ReadByte();
                BST = HP + ATK + DEF + SPE + SPA + SPD;

                Types = new[] { br.ReadByte(), br.ReadByte() };
                CatchRate = br.ReadByte();
                EvoStage = br.ReadByte();

                ushort EVs = br.ReadUInt16();
                EV_HP = EVs >> 0 & 0x3;
                EV_ATK = EVs >> 2 & 0x3;
                EV_DEF = EVs >> 4 & 0x3;
                EV_SPE = EVs >> 6 & 0x3;
                EV_SPA = EVs >> 8 & 0x3;
                EV_SPD = EVs >> 10 & 0x3;

                Items = new[] { br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16() };
                Gender = br.ReadByte();
                HatchCycles = br.ReadByte();
                BaseFriendship = br.ReadByte();

                EXPGrowth = br.ReadByte();
                EggGroups = new[] { br.ReadByte(), br.ReadByte() };
                Abilities = new[] { br.ReadByte(), br.ReadByte(), br.ReadByte() };
                EscapeRate = br.ReadByte();
                FormStats = br.ReadUInt16();

                FormeSprite = br.ReadUInt16();
                FormeCount = br.ReadByte();
                Color = br.ReadByte();
                BaseEXP = br.ReadUInt16();

                Height = br.ReadUInt16();
                Weight = br.ReadUInt16();

                byte[] TMHMData = br.ReadBytes(0x10);
                TMHM = new bool[8 * TMHMData.Length];
                for (int j = 0; j < TMHM.Length; j++)
                    TMHM[j] = (TMHMData[j / 8] >> (j % 8) & 0x1) == 1; //Bitflags for TMHM

                byte[] TutorData = br.ReadBytes(8);
                Tutors = new bool[8 * TutorData.Length];
                for (int j = 0; j < Tutors.Length; j++)
                    Tutors[j] = (TutorData[j / 8] >> (j % 8) & 0x1) == 1; //Bitflags for Tutors

                if (br.BaseStream.Length - br.BaseStream.Position == 0x10) // ORAS
                {
                    byte[][] ORASTutorData =
                    {
                            br.ReadBytes(4), // 15
                            br.ReadBytes(4), // 17
                            br.ReadBytes(4), // 16
                            br.ReadBytes(4), // 15
                        };
                    for (int i = 0; i < 4; i++)
                    {
                        ORASTutors[i] = new bool[8 * ORASTutorData[i].Length];
                        for (int b = 0; b < 8 * ORASTutorData[i].Length; b++)
                            ORASTutors[i][b] = (ORASTutorData[i][b / 8] >> b % 8 & 0x1) == 1;
                    }
                }
            }
        }

        // Data Manipulation
        public int FormeIndex(int species, int forme)
        {
            return forme == 0 || FormStats == 0 ? species : FormStats + forme - 1;
        }
        public int RandomGender
        {
            get
            {
                switch (Gender)
                {
                    case 255: // Genderless
                        return 2;
                    case 254: // Female
                        return 1;
                    case 0: // Male
                        return 0;
                    default:
                        return (int)(Util.rnd32() % 2);
                }
            }
        }
        public bool HasFormes => FormeCount > 1;

        internal static PersonalInfo[] getPersonalArray(byte[] data, int size)
        {
            PersonalInfo[] d = new PersonalInfo[data.Length / size];
            for (int i = 0; i < d.Length; i++)
                d[i] = new PersonalInfo(data.Skip(i * size).Take(size).ToArray());
            return d;
        }
    }
}
