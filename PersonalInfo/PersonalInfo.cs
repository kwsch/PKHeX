using System;

namespace PKHeX
{
    public abstract class PersonalInfo
    {
        protected const int SIZE_G4 = 0x2C;
        protected const int SIZE_BW = 0x3C;
        protected const int SIZE_B2W2 = 0x4C;
        protected const int SIZE_XY = 0x40;
        protected const int SIZE_AO = 0x50;

        protected byte[] Data;
        public abstract int HP { get; set; }
        public abstract int ATK { get; set; }
        public abstract int DEF { get; set; }
        public abstract int SPE { get; set; }
        public abstract int SPA { get; set; }
        public abstract int SPD { get; set; }
        public abstract int EV_HP { get; set; }
        public abstract int EV_ATK { get; set; }
        public abstract int EV_DEF { get; set; }
        public abstract int EV_SPE { get; set; }
        public abstract int EV_SPA { get; set; }
        public abstract int EV_SPD { get; set; }

        public abstract int[] Types { get; set; }
        public abstract int CatchRate { get; set; }
        public virtual int EvoStage { get; set; }
        public abstract int[] Items { get; set; }
        public abstract int Gender { get; set; }
        public abstract int HatchCycles { get; set; }
        public abstract int BaseFriendship { get; set; }
        public abstract int EXPGrowth { get; set; }
        public abstract int[] EggGroups { get; set; }
        public abstract int [] Abilities { get; set; }
        public abstract int EscapeRate { get; set; }
        public virtual int FormeCount { get; set; }
        protected virtual int FormStatsIndex { get; set; }
        public virtual int FormeSprite { get; set; }
        public abstract int BaseEXP { get; set; }
        public abstract int Color { get; set; }

        public virtual int Height { get; set; } = 0;
        public virtual int Weight { get; set; } = 0;

        public bool[] TMHM { get; set; }
        public bool[] TypeTutors { get; set; }
        public bool[][] SpecialTutors { get; set; } = new bool[0][];

        protected bool[] getBits(byte[] data)
        {
            bool[] r = new bool[8 * data.Length];
            for (int i = 0; i < r.Length; i++)
                r[i] = (data[i/8] >> (i&7) & 0x1) == 1;
            return r;
        }
        protected byte[] setBits(bool[] bits)
        {
            byte[] data = new byte[bits.Length/8];
            for (int i = 0; i < bits.Length; i++)
                data[i / 8] |= (byte)(bits[i] ? 1 << (i&0x7) : 0);
            return data;
        }
        public virtual byte[] Write() { return Data; }

        // Data Manipulation
        public int FormeIndex(int species, int forme)
        {
            return forme == 0 || FormStatsIndex == 0 ? species : FormStatsIndex + forme - 1;
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
        public int BST => HP + ATK + DEF + SPE + SPA + SPD;

        // Array Retrieval
        internal static PersonalInfo[] getArray(byte[] data, GameVersion format)
        {
            int size = 0;
            switch (format)
            {
                case GameVersion.DP:
                case GameVersion.Pt:
                case GameVersion.HGSS: size = SIZE_G4; break;
                case GameVersion.BW: size = SIZE_BW; break;
                case GameVersion.B2W2: size = SIZE_B2W2; break;
                case GameVersion.XY: size = SIZE_XY; break;
                case GameVersion.ORAS: size = SIZE_AO; break;
            }

            if (size == 0)
                return null;

            byte[][] entries = splitBytes(data, size);
            PersonalInfo[] d = new PersonalInfo[data.Length / size];

            switch (format)
            {
                case GameVersion.DP:
                case GameVersion.Pt:
                case GameVersion.HGSS:
                    for (int i = 0; i < d.Length; i++)
                        d[i] = new PersonalInfoG4(entries[i]);
                    break;
                case GameVersion.BW:
                    for (int i = 0; i < d.Length; i++)
                        d[i] = new PersonalInfoBW(entries[i]);
                    break;
                case GameVersion.B2W2:
                    for (int i = 0; i < d.Length; i++)
                        d[i] = new PersonalInfoB2W2(entries[i]);
                    break;
                case GameVersion.XY:
                    for (int i = 0; i < d.Length; i++)
                        d[i] = new PersonalInfoXY(entries[i]);
                    break;
                case GameVersion.ORAS:
                    for (int i = 0; i < d.Length; i++)
                        d[i] = new PersonalInfoORAS(entries[i]);
                    break;
            }
            return d;
        }
        private static byte[][] splitBytes(byte[] data, int size)
        {
            byte[][] r = new byte[data.Length/size][];
            for (int i = 0; i < data.Length; i += size)
            {
                r[i] = new byte[size];
                Array.Copy(data, i, r[i/size], 0, size);
            }
            return r;
        }
    }
}
