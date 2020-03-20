using System;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> table (array).
    /// </summary>
    /// <remarks>
    /// Serves as the main object that is accessed for stat data in a particular generation/game format.
    /// </remarks>
    public class PersonalTable
    {
        /// <summary>
        /// Personal Table used in <see cref="GameVersion.SWSH"/>.
        /// </summary>
        public static readonly PersonalTable SWSH = GetTable("swsh", GameVersion.SWSH); // todo

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.GG"/>.
        /// </summary>
        public static readonly PersonalTable GG = GetTable("gg", GameVersion.GG);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.USUM"/>.
        /// </summary>
        public static readonly PersonalTable USUM = GetTable("uu", GameVersion.USUM);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.SM"/>.
        /// </summary>
        public static readonly PersonalTable SM = GetTable("sm", GameVersion.SM);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.ORAS"/>.
        /// </summary>
        public static readonly PersonalTable AO = GetTable("ao", GameVersion.ORAS);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.XY"/>.
        /// </summary>
        public static readonly PersonalTable XY = GetTable("xy", GameVersion.XY);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.B2W2"/>.
        /// </summary>
        public static readonly PersonalTable B2W2 = GetTable("b2w2", GameVersion.B2W2);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.BW"/>.
        /// </summary>
        public static readonly PersonalTable BW = GetTable("bw", GameVersion.BW);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.HGSS"/>.
        /// </summary>
        public static readonly PersonalTable HGSS = GetTable("hgss", GameVersion.HGSS);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.Pt"/>.
        /// </summary>
        public static readonly PersonalTable Pt = GetTable("pt", GameVersion.Pt);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.DP"/>.
        /// </summary>
        public static readonly PersonalTable DP = GetTable("dp", GameVersion.DP);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.LG"/>.
        /// </summary>
        public static readonly PersonalTable LG = GetTable("lg", GameVersion.LG);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.FR"/>.
        /// </summary>
        public static readonly PersonalTable FR = GetTable("fr", GameVersion.FR);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.E"/>.
        /// </summary>
        public static readonly PersonalTable E = GetTable("e", GameVersion.E);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.RS"/>.
        /// </summary>
        public static readonly PersonalTable RS = GetTable("rs", GameVersion.RS);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.C"/>.
        /// </summary>
        public static readonly PersonalTable C = GetTable("c", GameVersion.C);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.GS"/>.
        /// </summary>
        public static readonly PersonalTable GS = GetTable("c", GameVersion.GS);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.RB"/>.
        /// </summary>
        public static readonly PersonalTable RB = GetTable("rb", GameVersion.RB);

        /// <summary>
        /// Personal Table used in <see cref="GameVersion.YW"/>.
        /// </summary>
        public static readonly PersonalTable Y = GetTable("y", GameVersion.YW);

        private static PersonalTable GetTable(string game, GameVersion format)
        {
            return new PersonalTable(Util.GetBinaryResource($"personal_{game}"), format);
        }

        private static Func<byte[], PersonalInfo> GetConstructor(GameVersion format)
        {
            switch (format)
            {
                case GameVersion.RB: case GameVersion.YW: case GameVersion.RBY:
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
                case GameVersion.SM: case GameVersion.USUM:
                    return z => new PersonalInfoSM(z);
                case GameVersion.GG:
                    return z => new PersonalInfoGG(z);
                default:
                    return z => new PersonalInfoSWSH(z);
            }
        }

        private static int GetEntrySize(GameVersion format)
        {
            switch (format)
            {
                case GameVersion.RB:
                case GameVersion.YW:
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
                case GameVersion.SM:
                case GameVersion.USUM:
                case GameVersion.GG: return PersonalInfoSM.SIZE;
                case GameVersion.SWSH: return PersonalInfoSWSH.SIZE;

                default: return -1;
            }
        }

        static PersonalTable() // Finish Setup
        {
            FixPersonalTableG1();
            PopulateGen3Tutors();
            PopulateGen4Tutors();
            CopyDexitGenders();
        }

        private static void FixPersonalTableG1()
        {
            // Update Yellow's catch rates; currently matches Red/Blue's values.
            Y[25].CatchRate = 163; // Pikachu
            Y[64].CatchRate = 96; // Kadabra

            // Load Gen2 Gender Ratios into Gen1
            for (int i = 0; i <= Legal.MaxSpeciesID_1; i++)
                RB[i].Gender = Y[i].Gender = GS[i].Gender;
        }

        private static void PopulateGen3Tutors()
        {
            // Update Gen3 data with Emerald's data, FR/LG is a subset of Emerald's compatibility.
            var TMHM = BinLinker.Unpack(Util.GetBinaryResource("hmtm_g3.pkl"), "g3");
            var tutors = BinLinker.Unpack(Util.GetBinaryResource("tutors_g3.pkl"), "g3");
            for (int i = 0; i <= Legal.MaxSpeciesID_3; i++)
            {
                E[i].AddTMHM(TMHM[i]);
                E[i].AddTypeTutors(tutors[i]);
            }
        }

        private static void PopulateGen4Tutors()
        {
            var tutors = BinLinker.Unpack(Util.GetBinaryResource("tutors_g4.pkl"), "g4");
            for (int i = 0; i < tutors.Length; i++)
                HGSS[i].AddTypeTutors(tutors[i]);
        }

        /// <summary>
        /// Sword/Shield do not contain personal data (stubbed) for all species that are not allowed to visit the game.
        /// Copy all the genders from <see cref="USUM"/>'s table for all past species, since we need it for <see cref="PKX.Personal"/> gender lookups for all generations.
        /// </summary>
        private static void CopyDexitGenders()
        {
            for (int i = 1; i <= 807; i++)
            {
                var ss = SWSH[i];
                if (ss.HP == 0)
                    ss.Gender = USUM[i].Gender;
            }
        }

        public PersonalTable(byte[] data, GameVersion format)
        {
            var get = GetConstructor(format);
            int size = GetEntrySize(format);
            byte[][] entries = data.Split(size);
            Table = new PersonalInfo[entries.Length];
            for (int i = 0; i < Table.Length; i++)
                Table[i] = get(entries[i]);

            MaxSpeciesID = format.GetMaxSpeciesID();
            Game = format;
        }

        private readonly PersonalInfo[] Table;

        /// <summary>
        /// Gets an index from the inner <see cref="Table"/> array.
        /// </summary>
        /// <remarks>Has built in length checks; returns empty (0) entry if out of range.</remarks>
        /// <param name="index">Index to retrieve</param>
        /// <returns>Requested index entry</returns>
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

        /// <summary>
        /// Gets the abilities possible for a given <see cref="PKM.Species"/> and <see cref="PKM.AltForm"/>.
        /// </summary>
        /// <param name="species"><see cref="PKM.Species"/></param>
        /// <param name="forme"><see cref="PKM.AltForm"/></param>
        /// <returns>Array of possible abilities</returns>
        public int[] GetAbilities(int species, int forme)
        {
            return GetFormeEntry(species, forme).Abilities;
        }

        /// <summary>
        /// Gets the <see cref="PersonalInfo"/> entry index for a given <see cref="PKM.Species"/> and <see cref="PKM.AltForm"/>.
        /// </summary>
        /// <param name="species"><see cref="PKM.Species"/></param>
        /// <param name="forme"><see cref="PKM.AltForm"/></param>
        /// <returns>Entry index for the input criteria</returns>
        public int GetFormeIndex(int species, int forme)
        {
            if (species > MaxSpeciesID)
            { Debug.WriteLine($"Requested out of bounds {nameof(species)}: {species} (max={MaxSpeciesID})"); species = 0; }
            return this[species].FormeIndex(species, forme);
        }

        /// <summary>
        /// Gets the <see cref="PersonalInfo"/> entry for a given <see cref="PKM.Species"/> and <see cref="PKM.AltForm"/>.
        /// </summary>
        /// <param name="species"><see cref="PKM.Species"/></param>
        /// <param name="forme"><see cref="PKM.AltForm"/></param>
        /// <returns>Entry for the input criteria</returns>
        public PersonalInfo GetFormeEntry(int species, int forme)
        {
            return this[GetFormeIndex(species, forme)];
        }

        /// <summary>
        /// Count of entries in the table, which includes default species entries and their separate <see cref="PKM.AltForm"/> entreis.
        /// </summary>
        public int TableLength => Table.Length;

        /// <summary>
        /// Maximum Species ID for the Table.
        /// </summary>
        public readonly int MaxSpeciesID;

        /// <summary>
        /// Game(s) the <see cref="Table"/> originated from.
        /// </summary>
        public readonly GameVersion Game;

        /// <summary>
        /// Gets form names for every species.
        /// </summary>
        /// <param name="species">Raw string resource (Species) for the corresponding table.</param>
        /// <param name="MaxSpecies">Max Species ID (<see cref="PKM.Species"/>)</param>
        /// <returns>Array of species containing an array of form names for that species.</returns>
        public string[][] GetFormList(string[] species, int MaxSpecies)
        {
            string[][] FormList = new string[MaxSpecies+1][];
            for (int i = 0; i < FormList.Length; i++)
            {
                int FormCount = this[i].FormeCount;
                FormList[i] = new string[FormCount];
                if (FormCount <= 0)
                    continue;

                FormList[i][0] = species[i];
                for (int j = 1; j < FormCount; j++)
                    FormList[i][j] = $"{species[i]} {j}";
            }

            return FormList;
        }

        /// <summary>
        /// Gets an arranged list of Form names and indexes for use with the individual <see cref="PersonalInfo"/> <see cref="PKM.AltForm"/> values.
        /// </summary>
        /// <param name="AltForms">Raw string resource (Forms) for the corresponding table.</param>
        /// <param name="species">Raw string resource (Species) for the corresponding table.</param>
        /// <param name="MaxSpecies">Max Species ID (<see cref="PKM.Species"/>)</param>
        /// <param name="baseForm">Pointers for base form IDs</param>
        /// <param name="formVal">Pointers for table indexes for each form</param>
        /// <returns>Sanitized list of species names, and outputs indexes for various lookup purposes.</returns>
        public string[] GetPersonalEntryList(string[][] AltForms, string[] species, int MaxSpecies, out int[] baseForm, out int[] formVal)
        {
            string[] result = new string[Table.Length];
            baseForm = new int[result.Length];
            formVal = new int[result.Length];
            for (int i = 0; i <= MaxSpecies; i++)
            {
                result[i] = species[i];
                if (AltForms[i].Length == 0)
                    continue;
                int altformpointer = this[i].FormStatsIndex;
                if (altformpointer <= 0)
                    continue;
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

        /// <summary>
        /// Checks to see if either of the input type combinations exist in the table.
        /// </summary>
        /// <remarks>Only useful for checking Generation 1 <see cref="PK1.Type_A"/> and <see cref="PK1.Type_B"/> properties.</remarks>
        /// <param name="Type1">First type</param>
        /// <param name="Type2">Second type</param>
        /// <returns>Indication that the combination exists in the table.</returns>
        public bool IsValidTypeCombination(int Type1, int Type2)
        {
            return Table.Any(p => p.IsValidTypeCombination(Type1, Type2));
        }
    }
}
