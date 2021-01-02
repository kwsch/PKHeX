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
        public static readonly PersonalTable SWSH = GetTable("swsh", GameVersion.SWSH);

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
            return new(Util.GetBinaryResource($"personal_{game}"), format);
        }

        private static Func<byte[], PersonalInfo> GetConstructor(GameVersion format) => format switch
        {
            GameVersion.RB or GameVersion.YW => z => new PersonalInfoG1(z),
            GameVersion.GS or GameVersion.C => z => new PersonalInfoG2(z),
            GameVersion.RS or GameVersion.E or GameVersion.FR or GameVersion.LG => z => new PersonalInfoG3(z),
            GameVersion.DP or GameVersion.Pt or GameVersion.HGSS => z => new PersonalInfoG4(z),
            GameVersion.BW => z => new PersonalInfoBW(z),
            GameVersion.B2W2 => z => new PersonalInfoB2W2(z),
            GameVersion.XY => z => new PersonalInfoXY(z),
            GameVersion.ORAS => z => new PersonalInfoORAS(z),
            GameVersion.SM or GameVersion.USUM => z => new PersonalInfoSM(z),
            GameVersion.GG => z => new PersonalInfoGG(z),
            _ => z => new PersonalInfoSWSH(z),
        };

        private static int GetEntrySize(GameVersion format) => format switch
        {
            GameVersion.RB or GameVersion.YW => PersonalInfoG1.SIZE,
            GameVersion.GS or GameVersion.C => PersonalInfoG2.SIZE,
            GameVersion.RS or GameVersion.E or GameVersion.FR or GameVersion.LG => PersonalInfoG3.SIZE,
            GameVersion.DP or GameVersion.Pt or GameVersion.HGSS => PersonalInfoG4.SIZE,
            GameVersion.BW => PersonalInfoBW.SIZE,
            GameVersion.B2W2 => PersonalInfoB2W2.SIZE,
            GameVersion.XY => PersonalInfoXY.SIZE,
            GameVersion.ORAS => PersonalInfoORAS.SIZE,
            GameVersion.SM or GameVersion.USUM or GameVersion.GG => PersonalInfoSM.SIZE,
            GameVersion.SWSH => PersonalInfoSWSH.SIZE,
            _ => -1,
        };

        static PersonalTable() // Finish Setup
        {
            FixPersonalTableG1();
            PopulateGen3Tutors();
            PopulateGen4Tutors();
            CopyDexitGenders();
        }

        private static void FixPersonalTableG1()
        {
            // Load Gen2 Gender Ratios into Gen1
            PersonalInfo[] rb = RB.Table, y = Y.Table, gs = GS.Table;
            for (int i = 0; i <= Legal.MaxSpeciesID_1; i++)
                rb[i].Gender = y[i].Gender = gs[i].Gender;
        }

        private static void PopulateGen3Tutors()
        {
            // Update Gen3 data with Emerald's data, FR/LG is a subset of Emerald's compatibility.
            var machine = BinLinker.Unpack(Util.GetBinaryResource("hmtm_g3.pkl"), "g3");
            var tutors = BinLinker.Unpack(Util.GetBinaryResource("tutors_g3.pkl"), "g3");
            var table = E.Table;
            for (int i = 0; i <= Legal.MaxSpeciesID_3; i++)
            {
                table[i].AddTMHM(machine[i]);
                table[i].AddTypeTutors(tutors[i]);
            }
        }

        private static void PopulateGen4Tutors()
        {
            var tutors = BinLinker.Unpack(Util.GetBinaryResource("tutors_g4.pkl"), "g4");
            var table = HGSS.Table;
            for (int i = 0; i < tutors.Length; i++)
                table[i].AddTypeTutors(tutors[i]);
        }

        /// <summary>
        /// Sword/Shield do not contain personal data (stubbed) for all species that are not allowed to visit the game.
        /// Copy all the genders from <see cref="USUM"/>'s table for all past species, since we need it for <see cref="PKX.Personal"/> gender lookups for all generations.
        /// </summary>
        private static void CopyDexitGenders()
        {
            var swsh = SWSH.Table;
            var usum = USUM.Table;

            for (int i = 1; i <= Legal.MaxSpeciesID_7_USUM; i++)
            {
                var ss = swsh[i];
                if (ss.HP == 0)
                    ss.Gender = usum[i].Gender;
            }
        }

        public PersonalTable(byte[] data, GameVersion format)
        {
            var get = GetConstructor(format);
            int size = GetEntrySize(format);
            byte[][] entries = data.Split(size);
            var table = new PersonalInfo[entries.Length];
            for (int i = 0; i < table.Length; i++)
                table[i] = get(entries[i]);

            Table = table;
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
                var table = Table;
                if ((uint)index >= table.Length)
                    return table[0];
                return table[index];
            }
            set
            {
                var table = Table;
                if ((uint)index >= table.Length)
                    return;
                table[index] = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="PersonalInfo"/> entry index for a given <see cref="PKM.Species"/> and <see cref="PKM.Form"/>.
        /// </summary>
        /// <param name="species"><see cref="PKM.Species"/></param>
        /// <param name="form"><see cref="PKM.Form"/></param>
        /// <returns>Entry index for the input criteria</returns>
        public int GetFormIndex(int species, int form)
        {
            if ((uint)species <= MaxSpeciesID)
                return Table[species].FormIndex(species, form);
            Debug.WriteLine($"Requested out of bounds {nameof(species)}: {species} (max={MaxSpeciesID})");
            return 0;
        }

        /// <summary>
        /// Gets the <see cref="PersonalInfo"/> entry for a given <see cref="PKM.Species"/> and <see cref="PKM.Form"/>.
        /// </summary>
        /// <param name="species"><see cref="PKM.Species"/></param>
        /// <param name="form"><see cref="PKM.Form"/></param>
        /// <returns>Entry for the input criteria</returns>
        public PersonalInfo GetFormEntry(int species, int form)
        {
            return this[GetFormIndex(species, form)];
        }

        /// <summary>
        /// Count of entries in the table, which includes default species entries and their separate <see cref="PKM.Form"/> entreis.
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
                int FormCount = this[i].FormCount;
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
        /// Gets an arranged list of Form names and indexes for use with the individual <see cref="PersonalInfo"/> <see cref="PKM.Form"/> values.
        /// </summary>
        /// <param name="forms">Raw string resource (Forms) for the corresponding table.</param>
        /// <param name="species">Raw string resource (Species) for the corresponding table.</param>
        /// <param name="MaxSpecies">Max Species ID (<see cref="PKM.Species"/>)</param>
        /// <param name="baseForm">Pointers for base form IDs</param>
        /// <param name="formVal">Pointers for table indexes for each form</param>
        /// <returns>Sanitized list of species names, and outputs indexes for various lookup purposes.</returns>
        public string[] GetPersonalEntryList(string[][] forms, string[] species, int MaxSpecies, out int[] baseForm, out int[] formVal)
        {
            string[] result = new string[Table.Length];
            baseForm = new int[result.Length];
            formVal = new int[result.Length];
            for (int i = 0; i <= MaxSpecies; i++)
            {
                result[i] = species[i];
                if (forms[i].Length == 0)
                    continue;
                int basePtr = this[i].FormStatsIndex;
                if (basePtr <= 0)
                    continue;
                for (int j = 1; j < forms[i].Length; j++)
                {
                    int ptr = basePtr + j - 1;
                    baseForm[ptr] = i;
                    formVal[ptr] = j;
                    result[ptr] = forms[i][j];
                }
            }
            return result;
        }

        /// <summary>
        /// Checks to see if either of the input type combinations exist in the table.
        /// </summary>
        /// <remarks>Only useful for checking Generation 1 <see cref="PK1.Type_A"/> and <see cref="PK1.Type_B"/> properties.</remarks>
        /// <param name="type1">First type</param>
        /// <param name="type2">Second type</param>
        /// <returns>Indication that the combination exists in the table.</returns>
        public bool IsValidTypeCombination(int type1, int type2)
        {
            return Table.Any(p => p.IsValidTypeCombination(type1, type2));
        }
    }
}
