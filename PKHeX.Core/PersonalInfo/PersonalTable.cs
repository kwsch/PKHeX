using System;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> table (array).
/// </summary>
/// <remarks>
/// Serves as the main object that is accessed for stat data in a particular generation/game format.
/// </remarks>
public static class PersonalTable
{
    /// <summary>
    /// Personal Table used in <see cref="GameVersion.PLA"/>.
    /// </summary>
    public static readonly PersonalTable8LA LA = new(GetTable("la"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.BDSP"/>.
    /// </summary>
    public static readonly PersonalTable8BDSP BDSP = new(GetTable("bdsp"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.SWSH"/>.
    /// </summary>
    public static readonly PersonalTable8SWSH SWSH = new(GetTable("swsh"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.GG"/>.
    /// </summary>
    public static readonly PersonalTable7GG GG = new(GetTable("gg"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.USUM"/>.
    /// </summary>
    public static readonly PersonalTable7 USUM = new(GetTable("uu"), Legal.MaxSpeciesID_7_USUM);

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.SM"/>.
    /// </summary>
    public static readonly PersonalTable7 SM = new(GetTable("sm"), Legal.MaxSpeciesID_7);

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.ORAS"/>.
    /// </summary>
    public static readonly PersonalTable6AO AO = new(GetTable("ao"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.XY"/>.
    /// </summary>
    public static readonly PersonalTable6XY XY = new(GetTable("xy"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.B2W2"/>.
    /// </summary>
    public static readonly PersonalTable5B2W2 B2W2 = new(GetTable("b2w2"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.BW"/>.
    /// </summary>
    public static readonly PersonalTable5BW BW = new(GetTable("bw"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.HGSS"/>.
    /// </summary>
    public static readonly PersonalTable4 HGSS = new(GetTable("hgss"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.Pt"/>.
    /// </summary>
    public static readonly PersonalTable4 Pt = new(GetTable("pt"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.DP"/>.
    /// </summary>
    public static readonly PersonalTable4 DP = new(GetTable("dp"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.LG"/>.
    /// </summary>
    public static readonly PersonalTable3 LG = new(GetTable("lg"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.FR"/>.
    /// </summary>
    public static readonly PersonalTable3 FR = new(GetTable("fr"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.E"/>.
    /// </summary>
    public static readonly PersonalTable3 E = new(GetTable("e"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.RS"/>.
    /// </summary>
    public static readonly PersonalTable3 RS = new(GetTable("rs"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.C"/>.
    /// </summary>
    public static readonly PersonalTable2 C = new(GetTable("c"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.GS"/>.
    /// </summary>
    public static readonly PersonalTable2 GS = new(GetTable("gs"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.RB"/>.
    /// </summary>
    public static readonly PersonalTable1 RB = new(GetTable("rb"));

    /// <summary>
    /// Personal Table used in <see cref="GameVersion.YW"/>.
    /// </summary>
    public static readonly PersonalTable1 Y = new(GetTable("y"));

    private static ReadOnlySpan<byte> GetTable(string game) => Util.GetBinaryResource($"personal_{game}");

    static PersonalTable() // Finish Setup
    {
        FixPersonalTableG1();
        PopulateGen3Tutors();
        PopulateGen4Tutors();
        CopyDexitGenders();
    }

    private static void FixPersonalTableG1()
    {
        var gs = GS.Table;
        RB.LoadValuesFrom(gs);
        Y.LoadValuesFrom(gs);
    }

    private static void PopulateGen3Tutors()
    {
        // Update Gen3 data with Emerald's data, FR/LG is a subset of Emerald's compatibility.
        var machine = BinLinkerAccessor.Get(Util.GetBinaryResource("hmtm_g3.pkl"), "g3");
        var tutors = BinLinkerAccessor.Get(Util.GetBinaryResource("tutors_g3.pkl"), "g3");
        var table = E.Table;
        for (int i = Legal.MaxSpeciesID_3; i >= 1; i--)
        {
            var entry = table[i];
            entry.AddTMHM(machine[i]);
            entry.AddTypeTutors(tutors[i]);

            // Copy to other tables
            RS.Table[i].TMHM = FR.Table[i].TMHM = LG.Table[i].TMHM = entry.TMHM;
            RS.Table[i].TypeTutors = FR.Table[i].TypeTutors = LG.Table[i].TypeTutors = entry.TypeTutors;
        }
    }

    private static void PopulateGen4Tutors()
    {
        var tutors = BinLinkerAccessor.Get(Util.GetBinaryResource("tutors_g4.pkl"), "g4");
        var table = HGSS.Table;
        var count = tutors.Length;
        for (int i = 1; i < count; i++)
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

        var la = LA;
        for (int i = 1; i <= Legal.MaxSpeciesID_8_R2; i++)
        {
            var e = la.Table[i];
            var fc = e.FormCount;
            for (int f = 0; f < fc; f++)
            {
                var l = la.GetFormEntry(i, f);
                if (l.HP != 0)
                    continue;
                var s = SWSH.GetFormEntry(i, f);
                l.Ability1 = s.Ability1;
                l.Ability2 = s.Ability2;
                l.AbilityH = s.AbilityH;
                l.Gender = s.Gender;
                l.EXPGrowth = s.EXPGrowth;
            }
        }
    }
}
