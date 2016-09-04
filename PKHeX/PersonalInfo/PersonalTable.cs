using System;

namespace PKHeX
{
    public class PersonalTable
    {
        internal static readonly PersonalTable AO = new PersonalTable(Properties.Resources.personal_ao, GameVersion.ORAS);
        internal static readonly PersonalTable XY = new PersonalTable(Properties.Resources.personal_xy, GameVersion.XY);
        internal static readonly PersonalTable B2W2 = new PersonalTable(Properties.Resources.personal_b2w2, GameVersion.B2W2);
        internal static readonly PersonalTable BW = new PersonalTable(Properties.Resources.personal_bw, GameVersion.BW);
        internal static readonly PersonalTable HGSS = new PersonalTable(Properties.Resources.personal_hgss, GameVersion.HGSS);
        internal static readonly PersonalTable Pt = new PersonalTable(Properties.Resources.personal_pt, GameVersion.Pt);
        internal static readonly PersonalTable DP = new PersonalTable(Properties.Resources.personal_dp, GameVersion.DP);
        internal static readonly PersonalTable LG = new PersonalTable(Properties.Resources.personal_lg, GameVersion.LG);
        internal static readonly PersonalTable FR = new PersonalTable(Properties.Resources.personal_fr, GameVersion.FR);
        internal static readonly PersonalTable E = new PersonalTable(Properties.Resources.personal_e, GameVersion.E);
        internal static readonly PersonalTable RS = new PersonalTable(Properties.Resources.personal_rs, GameVersion.RS);
        internal static readonly PersonalTable C = new PersonalTable(Properties.Resources.personal_c, GameVersion.C);
        internal static readonly PersonalTable GS = new PersonalTable(Properties.Resources.personal_c, GameVersion.GS);
        internal static readonly PersonalTable RBY = new PersonalTable(Properties.Resources.personal_rby, GameVersion.RBY);

        private static byte[][] splitBytes(byte[] data, int size)
        {
            byte[][] r = new byte[data.Length / size][];
            for (int i = 0; i < data.Length; i += size)
            {
                r[i / size] = new byte[size];
                Array.Copy(data, i, r[i / size], 0, size);
            }
            return r;
        }
        private PersonalTable(byte[] data, GameVersion format)
        {
            int size = 0;
            switch (format)
            {
                case GameVersion.RBY: size = PersonalInfoG1.SIZE; break;
                case GameVersion.GS:
                case GameVersion.C: size = PersonalInfoG2.SIZE; break;
                case GameVersion.RS:
                case GameVersion.E:
                case GameVersion.FR:
                case GameVersion.LG: size = PersonalInfoG3.SIZE; break;
                case GameVersion.DP:
                case GameVersion.Pt:
                case GameVersion.HGSS: size = PersonalInfoG4.SIZE; break;
                case GameVersion.BW: size = PersonalInfoBW.SIZE; break;
                case GameVersion.B2W2: size = PersonalInfoB2W2.SIZE; break;
                case GameVersion.XY: size = PersonalInfoXY.SIZE; break;
                case GameVersion.ORAS: size = PersonalInfoORAS.SIZE; break;
            }

            if (size == 0)
            { Table = null; return; }

            byte[][] entries = splitBytes(data, size);
            PersonalInfo[] d = new PersonalInfo[data.Length / size];

            switch (format)
            {
                case GameVersion.RBY:
                    for (int i = 0; i < d.Length; i++)
                        d[i] = new PersonalInfoG1(entries[i]);
                    break;
                case GameVersion.GS:
                case GameVersion.C:
                    for (int i = 0; i < d.Length; i++)
                        d[i] = new PersonalInfoG2(entries[i]);
                    break;
                case GameVersion.RS:
                case GameVersion.E:
                case GameVersion.FR:
                case GameVersion.LG:
                    Array.Resize(ref d, 387);
                    for (int i = 0; i < d.Length; i++) // entries are not in order of natdexID
                        d[i] = new PersonalInfoG3(entries[PKX.getG3Species(i)]);
                    break;
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
            Table = d;
        }

        private readonly PersonalInfo[] Table;
        public PersonalInfo this[int index]
        {
            get { return Table[index]; }
            set { Table[index] = value; }
        }

        public int[] getAbilities(int species, int forme)
        {
            if (species >= Table.Length)
            { species = 0; Console.WriteLine("Requested out of bounds SpeciesID"); }
            return this[getFormeIndex(species, forme)].Abilities;
        }
        public int getFormeIndex(int species, int forme)
        {
            if (species >= Table.Length)
            { species = 0; Console.WriteLine("Requested out of bounds SpeciesID"); }
            return this[species].FormeIndex(species, forme);
        }
        public PersonalInfo getFormeEntry(int species, int forme)
        {
            return this[getFormeIndex(species, forme)];
        }
    }
}
