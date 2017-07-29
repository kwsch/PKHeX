using System;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    public class PersonalTable
    {
        public static readonly PersonalTable SM = GetTable("sm", GameVersion.SM);
        public static readonly PersonalTable AO = GetTable("ao", GameVersion.ORAS);
        public static readonly PersonalTable XY = GetTable("xy", GameVersion.XY);
        public static readonly PersonalTable B2W2 = GetTable("b2w2", GameVersion.B2W2);
        public static readonly PersonalTable BW = GetTable("bw", GameVersion.BW);
        public static readonly PersonalTable HGSS = GetTable("hgss", GameVersion.HGSS);
        public static readonly PersonalTable Pt = GetTable("pt", GameVersion.Pt);
        public static readonly PersonalTable DP = GetTable("dp", GameVersion.DP);
        public static readonly PersonalTable LG = GetTable("lg", GameVersion.LG);
        public static readonly PersonalTable FR = GetTable("fr", GameVersion.FR);
        public static readonly PersonalTable E = GetTable("e", GameVersion.E);
        public static readonly PersonalTable RS = GetTable("rs", GameVersion.RS);
        public static readonly PersonalTable C = GetTable("c", GameVersion.C);
        public static readonly PersonalTable GS = GetTable("c", GameVersion.GS);
        public static readonly PersonalTable RB = GetTable("rb", GameVersion.RBY);
        public static readonly PersonalTable Y = GetTable("y", GameVersion.RBY);
        private static PersonalTable GetTable(string game, GameVersion format)
        {
            return new PersonalTable(Util.GetBinaryResource($"personal_{game}"), format);
        }

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
        private static Func<byte[], PersonalInfo> GetConstructor(GameVersion format)
        {
            switch (format)
            {
                case GameVersion.RBY:
                    return z => new PersonalInfoG1(z);
                case GameVersion.GS: case GameVersion.C:
                    return z => new PersonalInfoG2(z);
                case GameVersion.RS: case GameVersion.E: case GameVersion.FR: case GameVersion.LG: 
                    return z => new PersonalInfoG3(z);
                case GameVersion.DP: case GameVersion.Pt: case GameVersion.HGSS:
                    return z => new PersonalInfoG4(z);
                case GameVersion.BW:
                    return z => new PersonalInfoBW(z);
                case GameVersion.B2W2:
                    return z => new PersonalInfoB2W2(z);
                case GameVersion.XY:
                    return z => new PersonalInfoXY(z);
                case GameVersion.ORAS:
                    return z => new PersonalInfoORAS(z);
                case GameVersion.SM:
                    return z => new PersonalInfoSM(z);
            }
            return null;
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
        static PersonalTable() // Finish Setup
        {
            FixPersonalTableY();
            PopulateGen3Tutors();
            PopulateGen4Tutors();
        }
        private static void FixPersonalTableY()
        {
            // Update Yellow's catch rates; currently matches Red/Blue's values.
            Y[25].CatchRate = 163; // Pikachu
            Y[64].CatchRate = 96; // Kadabra
        }
        private static void PopulateGen3Tutors()
        {
            // Update Gen3 data with Emerald's data, FR/LG is a subset of Emerald's compatibility.
            var TMHM = Data.UnpackMini(Util.GetBinaryResource("hmtm_g3.pkl"), "g3");
            var tutors = Data.UnpackMini(Util.GetBinaryResource("tutors_g3.pkl"), "g3");
            for (int i = 0; i <= Legal.MaxSpeciesID_3; i++)
            {
                E[i].AddTMHM(TMHM[i]);
                E[i].AddTypeTutors(tutors[i]);
            }
        }
        private static void PopulateGen4Tutors()
        {
            var tutors = Data.UnpackMini(Util.GetBinaryResource("tutors_g4.pkl"), "g4");
            for (int i = 0; i <= Legal.MaxSpeciesID_4; i++)
                HGSS[i].AddTypeTutors(tutors[i]);
        }

        private PersonalTable(byte[] data, GameVersion format)
        {
            Func<byte[], PersonalInfo> get = GetConstructor(format);
            int size = GetEntrySize(format);
            byte[][] entries = SplitBytes(data, size);
            Table = new PersonalInfo[data.Length / size];
            for (int i = 0; i < Table.Length; i++)
                Table[i] = get(entries[i]);
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
            { species = 0; Debug.WriteLine("Requested out of bounds SpeciesID"); }
            return this[GetFormeIndex(species, forme)].Abilities;
        }
        public int GetFormeIndex(int species, int forme)
        {
            if (species >= Table.Length)
            { species = 0; Debug.WriteLine("Requested out of bounds SpeciesID"); }
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
            for (int i = 0; i < FormList.Length; i++)
            {
                int FormCount = this[i].FormeCount;
                FormList[i] = new string[FormCount];
                if (FormCount <= 0) continue;
                FormList[i][0] = species[i];
                for (int j = 1; j < FormCount; j++)
                    FormList[i][j] = $"{species[i]} " + j;
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
