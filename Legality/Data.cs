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
        public SlotType Type = SlotType.Any;
        public bool AllowDexNav;
        public bool Pressure;
        public bool DexNav;
        public bool WhiteFlute;
        public bool BlackFlute;
        public bool Normal => !(WhiteFlute || BlackFlute || DexNav);
        public EncounterSlot() { }

        public EncounterSlot(EncounterSlot template)
        {
            Species = template.Species;
            AllowDexNav = template.AllowDexNav;
            LevelMax = template.LevelMax;
            LevelMin = template.LevelMin;
            Type = template.Type;
            Pressure = template.Pressure;
        }
    }

    public enum SlotType
    {
        Any,
        Grass,
        Rough_Terrain,
        Yellow_Flowers,
        Purple_Flowers,
        Red_Flowers,
        Surf,
        Old_Rod,
        Good_Rod,
        Super_Rod,
        Rock_Smash,
        Horde,
        FriendSafari,
        Special,
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
        public int Ball = 4; // Gift Only
        public GameVersion Version = GameVersion.Any;
        public int[] IVs = {-1, -1, -1, -1, -1, -1};
        public bool IV3;
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
        public bool Shiny = false;
        public int Gender = -1;
    }
    public class EncounterLink
    {
        public int Species;
        public int Level;
        public int Location = 30011;
        public int Ability = 4;
        public int Ball = 4; // Pokéball
        public Nature Nature = Nature.Random;
        public int[] IVs = { -1, -1, -1, -1, -1, -1 };
        public int FlawlessIVs = 0;
        public bool Classic = true;
        public bool Fateful = false;
        public int[] RelearnMoves = new int[4];
        public bool XY = true;
        public bool ORAS = true;
        public bool? Shiny = false;
        public bool OT = false;
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

    internal static class Data
    {
        internal static byte[][] unpackMini(byte[] fileData, string identifier)
        {
            using (var s = new MemoryStream(fileData))
            using (var br = new BinaryReader(s))
            {
                if (identifier != new string(br.ReadChars(2)))
                    return null;

                ushort count = br.ReadUInt16();
                byte[][] returnData = new byte[count][];

                uint[] offsets = new uint[count + 1];
                for (int i = 0; i < count; i++)
                    offsets[i] = br.ReadUInt32();

                uint length = br.ReadUInt32();
                offsets[offsets.Length - 1] = length;

                for (int i = 0; i < count; i++)
                {
                    br.BaseStream.Seek(offsets[i], SeekOrigin.Begin);
                    using (MemoryStream dataout = new MemoryStream())
                    {
                        byte[] data = new byte[0];
                        s.CopyTo(dataout, (int)offsets[i]);
                        int len = (int)offsets[i + 1] - (int)offsets[i];

                        if (len != 0)
                        {
                            data = dataout.ToArray();
                            Array.Resize(ref data, len);
                        }
                        returnData[i] = data;
                    }
                }
                return returnData;
            }
        }
    }
}
