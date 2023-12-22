using static PKHeX.Core.AreaWeather9;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.SV"/>.
/// </summary>
public sealed record EncounterSlot9(EncounterArea9 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte Gender, byte Time)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK9>, IEncounterFormRandom, IFixedGender
{
    public int Generation => 9;
    public EntityContext Context => EntityContext.Gen9;
    public bool EggEncounter => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public int EggLocation => 0;
    public bool IsRandomUnspecificForm => Form >= EncounterUtil1.FormDynamic;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.CrossFrom == 0 ? Parent.Location : Parent.CrossFrom;

    private static int GetTime(RibbonIndex mark) => mark switch
    {
        RibbonIndex.MarkLunchtime => 0,
        RibbonIndex.MarkSleepyTime => 1,
        RibbonIndex.MarkDusk => 2,
        RibbonIndex.MarkDawn => 3,
        _ => 4,
    };

    public bool CanSpawnAtTime(RibbonIndex mark) => (Time & (1 << GetTime(mark))) == 0;

    public bool CanSpawnInWeather(RibbonIndex mark)
    {
        var loc = (byte)Location;
        return CanSpawnInWeather(mark, loc);
    }

    public static bool CanSpawnInWeather(RibbonIndex mark, byte loc)
    {
        var weather = GetWeather(loc);
        return weather.IsMarkCompatible(mark);
    }

    /// <summary>
    /// Location IDs matched with possible weather types. Unlisted locations may only have Normal weather.
    /// </summary>
    public static AreaWeather9 GetWeather(byte location) => location switch
    {
        006 => Standard,                       // South Province (Area One)
        010 => Standard,                       // Pokémon League
        012 => Standard,                       // South Province (Area Two)
        014 => Standard,                       // South Province (Area Four)
        016 => Standard,                       // South Province (Area Six)
        018 => Standard,                       // South Province (Area Five)
        020 => Standard,                       // South Province (Area Three)
        022 => Standard,                       // West Province (Area One)
        024 => Sand,                           // Asado Desert
        026 => Standard,                       // West Province (Area Two)
        028 => Standard,                       // West Province (Area Three)
        030 => Standard,                       // Tagtree Thicket
        032 => Standard,                       // East Province (Area Three)
        034 => Standard,                       // East Province (Area One)
        036 => Standard,                       // East Province (Area Two)
        038 => Snow,                           // Glaseado Mountain (1)
        040 => Standard,                       // Casseroya Lake
        044 => Standard,                       // North Province (Area Three)
        046 => Standard,                       // North Province (Area One)
        048 => Standard,                       // North Province (Area Two)
        050 => Standard,                       // Great Crater of Paldea
        056 => Standard,                       // South Paldean Sea
        058 => Standard,                       // West Paldean Sea
        060 => Standard,                       // East Paldean Sea
        062 => Standard,                       // North Paldean Sea
        064 => Inside,                         // Inlet Grotto
        067 => Inside,                         // Alfornada Cavern
        069 => Standard | Inside | Snow | Snow,// Dalizapa Passage (Near Medali, Tunnels, Near Pokémon Center, Near Zapico)
        070 => Standard,                       // Poco Path
        080 => Standard,                       // Cabo Poco
        109 => Standard,                       // Socarrat Trail
        124 => Inside,                         // Area Zero (5)

        132 => Standard, // Kitakami Road
        134 => Standard, // Mossui Town
        136 => Standard, // Apple Hills
        138 => Standard, // Loyalty Plaza
        140 => Standard, // Reveler’s Road
        142 => Standard, // Kitakami Hall
        144 => Standard, // Oni Mountain
        146 => Standard, // Dreaded Den
        148 => Standard, // Oni’s Maw
        150 => Standard, // Oni Mountain
        152 => Standard, // Crystal Pool
        154 => Standard, // Crystal Pool
        156 => Standard, // Wistful Fields
        158 => Standard, // Mossfell Confluence
        160 => Standard, // Fellhorn Gorge
        162 => Standard, // Paradise Barrens
        164 => Standard, // Kitakami Wilds
        166 => Standard, // Timeless Woods
        168 => Standard, // Infernal Pass
        170 => Standard, // Chilling Waterhead

        174 => Standard, // Savanna Biome
        176 => Standard, // Coastal Biome
        178 => Standard, // Canyon Biome
        180 => Snow,     // Polar Biome
        182 => Standard, // Central Plaza
        184 => Standard, // Savanna Plaza
        186 => Standard, // Coastal Plaza
        188 => Standard, // Canyon Plaza
        190 => Standard, // Polar Plaza
        192 => Inside,   // Chargestone Cavern
        194 => Inside,   // Torchlit Labyrinth

        _ => None,
    };

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK9 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PK9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var form = GetWildForm(Form);
        var version = Version != GameVersion.SV ? Version : GameVersion.SV.Contains(tr.Game) ? (GameVersion)tr.Game : GameVersion.SL;
        var pi = PersonalTable.SV[Species, form];
        var pk = new PK9
        {
            Species = Species,
            Form = form,
            CurrentLevel = LevelMin,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)version,
            Ball = (byte)Ball.Poke,
            MetDate = EncounterDate.GetDateSwitch(),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Obedience_Level = LevelMin,
            OT_Friendship = pi.BaseFriendship,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };
        SetPINGA(pk, criteria, pi);
        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.ResetPartyStats();
        return pk;
    }

    private byte GetWildForm(byte form)
    {
        if (form < EncounterUtil1.FormDynamic)
            return form;
        if (form == EncounterUtil1.FormVivillon)
            return Vivillon3DS.FancyFormID; // Fancy Vivillon
        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.SV[Species].FormCount);
    }

    private void SetPINGA(PK9 pk, EncounterCriteria criteria, PersonalInfo9SV pi)
    {
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        criteria.SetRandomIVs(pk);

        pk.Nature = pk.StatNature = (int)criteria.GetNature();
        pk.Gender = criteria.GetGender(Gender, pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));

        var rand = new Xoroshiro128Plus(Util.Rand.Rand64());
        var type = Tera9RNG.GetTeraTypeFromPersonal(Species, Form, rand.Next());
        pk.TeraTypeOriginal = (MoveType)type;
        if (criteria.TeraType != -1 && type != criteria.TeraType)
            pk.SetTeraType(type); // sets the override type
        if (Species == (int)Core.Species.Toxtricity)
            pk.Nature = ToxtricityUtil.GetRandomNature(ref rand, Form);

        pk.HeightScalar = PokeSizeUtil.GetRandomScalar();
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar();
        pk.Scale = PokeSizeUtil.GetRandomScalar();
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Form != evo.Form && !IsRandomUnspecificForm && !IsFormOkayWild(Species, evo.Form))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (!this.IsLevelWithinRange(pk.Met_Level))
            return false;

        if (pk is ITeraType t)
        {
            var orig = (byte)t.TeraTypeOriginal;
            var pi = PersonalTable.SV[Species, Form];
            if (pi.Type1 != orig && pi.Type2 != orig)
                return false;
        }

        return true;
    }

    private static bool IsFormOkayWild(ushort species, byte form) => species switch
    {
        (int)Core.Species.Rotom => form <= 5,
        (int)Core.Species.Deerling or (int)Core.Species.Sawsbuck => form < 4,
        (int)Core.Species.Oricorio => form < 4,
        _ => false,
    };

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }
    #endregion
}
