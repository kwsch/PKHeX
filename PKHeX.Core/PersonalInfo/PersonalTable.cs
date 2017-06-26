using System;
using System.Linq;

namespace PKHeX.Core
{
    public class PersonalTable
    {
        public static readonly PersonalTable SM = new PersonalTable(Util.GetBinaryResource("personal_sm"), GameVersion.SM);
        public static readonly PersonalTable AO = new PersonalTable(Util.GetBinaryResource("personal_ao"), GameVersion.ORAS);
        public static readonly PersonalTable XY = new PersonalTable(Util.GetBinaryResource("personal_xy"), GameVersion.XY);
        public static readonly PersonalTable B2W2 = new PersonalTable(Util.GetBinaryResource("personal_b2w2"), GameVersion.B2W2);
        public static readonly PersonalTable BW = new PersonalTable(Util.GetBinaryResource("personal_bw"), GameVersion.BW);
        public static readonly PersonalTable HGSS = new PersonalTable(Util.GetBinaryResource("personal_hgss"), GameVersion.HGSS);
        public static readonly PersonalTable Pt = new PersonalTable(Util.GetBinaryResource("personal_pt"), GameVersion.Pt);
        public static readonly PersonalTable DP = new PersonalTable(Util.GetBinaryResource("personal_dp"), GameVersion.DP);
        public static readonly PersonalTable LG = new PersonalTable(Util.GetBinaryResource("personal_lg"), GameVersion.LG);
        public static readonly PersonalTable FR = new PersonalTable(Util.GetBinaryResource("personal_fr"), GameVersion.FR);
        public static readonly PersonalTable E = new PersonalTable(Util.GetBinaryResource("personal_e"), GameVersion.E);
        public static readonly PersonalTable RS = new PersonalTable(Util.GetBinaryResource("personal_rs"), GameVersion.RS);
        public static readonly PersonalTable C = new PersonalTable(Util.GetBinaryResource("personal_c"), GameVersion.C);
        public static readonly PersonalTable GS = new PersonalTable(Util.GetBinaryResource("personal_c"), GameVersion.GS);
        public static readonly PersonalTable RB = new PersonalTable(Util.GetBinaryResource("personal_rb"), GameVersion.RBY);
        public static readonly PersonalTable Y = new PersonalTable(Util.GetBinaryResource("personal_y"), GameVersion.RBY);

        private static byte[][] SplitBytes(byte[] data, int size)
        {
            byte[][] r = new byte[data.Length / size][];
            for (int i = 0; i < data.Length; i += size)
            {
                r[i / size] = new byte[size];
                Array.Copy(data, i, r[i / size], 0, size);
            }
            return r;
        }
        private static PersonalInfo[] GetArray(byte[] data, GameVersion format, int size)
        {
            byte[][] entries = SplitBytes(data, size);
            PersonalInfo[] d = new PersonalInfo[data.Length / size];

            switch (format)
            {
                case GameVersion.RBY:
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoG1(entries[i]);
                    return d;
                case GameVersion.GS: case GameVersion.C:
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoG2(entries[i]);
                    return d;
                case GameVersion.RS: case GameVersion.E: case GameVersion.FR: case GameVersion.LG: 
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoG3(entries[i]);
                    return d;
                case GameVersion.DP: case GameVersion.Pt: case GameVersion.HGSS:
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoG4(entries[i]);
                    return d;
                case GameVersion.BW:
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoBW(entries[i]);
                    return d;
                case GameVersion.B2W2:
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoB2W2(entries[i]);
                    return d;
                case GameVersion.XY:
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoXY(entries[i]);
                    return d;
                case GameVersion.ORAS:
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoORAS(entries[i]);
                    return d;
                case GameVersion.SM:
                    for (int i = 0; i < d.Length; i++) d[i] = new PersonalInfoSM(entries[i]);
                    return d;
                default:
                    return null;
            }
        }
        private static int GetEntrySize(GameVersion format)
        {
            switch (format)
            {
                case GameVersion.RBY: return PersonalInfoG1.SIZE;
                case GameVersion.GS:
                case GameVersion.C: return PersonalInfoG2.SIZE;
                case GameVersion.RS:
                case GameVersion.E:
                case GameVersion.FR:
                case GameVersion.LG: return PersonalInfoG3.SIZE;
                case GameVersion.DP:
                case GameVersion.Pt:
                case GameVersion.HGSS: return PersonalInfoG4.SIZE;
                case GameVersion.BW: return PersonalInfoBW.SIZE;
                case GameVersion.B2W2: return PersonalInfoB2W2.SIZE;
                case GameVersion.XY: return PersonalInfoXY.SIZE;
                case GameVersion.ORAS: return PersonalInfoORAS.SIZE;
                case GameVersion.SM: return PersonalInfoSM.SIZE;

                default: return -1;
            }
        }

        private PersonalTable(byte[] data, GameVersion format)
        {
            int size = GetEntrySize(format);
            Table = GetArray(data, format, size);
        }
        private readonly PersonalInfo[] Table;
        public PersonalInfo this[int index]
        {
            get
            {
                if (0 <= index && index < Table.Length)
                    return Table[index];
                return Table[0];
            }
            set
            {
                if (index < 0 || index >= Table.Length)
                    return;
                Table[index] = value; 
            }
        }

        public int[] GetAbilities(int species, int forme)
        {
            if (species >= Table.Length)
            { species = 0; Console.WriteLine("Requested out of bounds SpeciesID"); }
            return this[GetFormeIndex(species, forme)].Abilities;
        }
        public int GetFormeIndex(int species, int forme)
        {
            if (species >= Table.Length)
            { species = 0; Console.WriteLine("Requested out of bounds SpeciesID"); }
            return this[species].FormeIndex(species, forme);
        }
        public PersonalInfo GetFormeEntry(int species, int forme)
        {
            return this[GetFormeIndex(species, forme)];
        }

        public int TableLength => Table.Length;
        public string[][] GetFormList(string[] species, int MaxSpecies)
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
        public string[] GetPersonalEntryList(string[][] AltForms, string[] species, int MaxSpecies, out int[] baseForm, out int[] formVal)
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
        public bool IsValidTypeCombination(int Type1, int Type2)
        {
            return Table.Any(p => p.Types[0] == Type1 && p.Types[1] == Type2);
        }
    }
}
