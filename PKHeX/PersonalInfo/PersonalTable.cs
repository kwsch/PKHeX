using System;

namespace PKHeX.Core
{
    public class PersonalTable
    {
        public static readonly PersonalTable SM = new PersonalTable(Properties.Resources.personal_sm, GameVersion.SM);
        public static readonly PersonalTable AO = new PersonalTable(Properties.Resources.personal_ao, GameVersion.ORAS);
        public static readonly PersonalTable XY = new PersonalTable(Properties.Resources.personal_xy, GameVersion.XY);
        public static readonly PersonalTable B2W2 = new PersonalTable(Properties.Resources.personal_b2w2, GameVersion.B2W2);
        public static readonly PersonalTable BW = new PersonalTable(Properties.Resources.personal_bw, GameVersion.BW);
        public static readonly PersonalTable HGSS = new PersonalTable(Properties.Resources.personal_hgss, GameVersion.HGSS);
        public static readonly PersonalTable Pt = new PersonalTable(Properties.Resources.personal_pt, GameVersion.Pt);
        public static readonly PersonalTable DP = new PersonalTable(Properties.Resources.personal_dp, GameVersion.DP);
        public static readonly PersonalTable LG = new PersonalTable(Properties.Resources.personal_lg, GameVersion.LG);
        public static readonly PersonalTable FR = new PersonalTable(Properties.Resources.personal_fr, GameVersion.FR);
        public static readonly PersonalTable E = new PersonalTable(Properties.Resources.personal_e, GameVersion.E);
        public static readonly PersonalTable RS = new PersonalTable(Properties.Resources.personal_rs, GameVersion.RS);
        public static readonly PersonalTable C = new PersonalTable(Properties.Resources.personal_c, GameVersion.C);
        public static readonly PersonalTable GS = new PersonalTable(Properties.Resources.personal_c, GameVersion.GS);
        public static readonly PersonalTable RB = new PersonalTable(Properties.Resources.personal_rb, GameVersion.RBY);
        public static readonly PersonalTable Y = new PersonalTable(Properties.Resources.personal_y, GameVersion.RBY);

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
                case GameVersion.SM: size = PersonalInfoSM.SIZE; break;
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
                case GameVersion.SM:
                    for (int i = 0; i < d.Length; i++)
                        d[i] = new PersonalInfoSM(entries[i]);
                    break;
            }
            Table = d;
        }

        private readonly PersonalInfo[] Table;
        public PersonalInfo this[int index]
        {
            get
            {
                if (index < Table.Length)
                    return Table[index];
                return Table[0];
            }
            set
            {
                if (index < Table.Length)
                    return;
                Table[index] = value; 
            }
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

        public int TableLength => Table.Length;
        public string[][] getFormList(string[] species, int MaxSpecies)
        {
            string[][] FormList = new string[MaxSpecies+1][];
            for (int i = 0; i <= MaxSpecies; i++) //Hardcode 721 species + null
            {
                int FormCount = this[i].FormeCount;
                FormList[i] = new string[FormCount];
                if (FormCount <= 0) continue;
                FormList[i][0] = species[i];
                for (int j = 1; j < FormCount; j++)
                {
                    FormList[i][j] = $"{species[i]} " + j;
                }
            }

            return FormList;
        }
        public string[] getPersonalEntryList(string[][] AltForms, string[] species, int MaxSpecies, out int[] baseForm, out int[] formVal)
        {
            string[] result = new string[Table.Length];
            baseForm = new int[result.Length];
            formVal = new int[result.Length];
            for (int i = 0; i <= MaxSpecies; i++)
            {
                result[i] = species[i];
                if (AltForms[i].Length == 0) continue;
                int altformpointer = this[i].FormStatsIndex;
                if (altformpointer <= 0) continue;
                for (int j = 1; j < AltForms[i].Length; j++)
                {
                    int ptr = altformpointer + j - 1;
                    baseForm[ptr] = i;
                    formVal[ptr] = j;
                    result[ptr] = AltForms[i][j];
                }
            }
            return result;
        }
    }
}
