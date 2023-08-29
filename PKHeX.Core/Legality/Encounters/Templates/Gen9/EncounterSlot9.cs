using System.Collections.Generic;
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
        if (AreaWeather.TryGetValue((byte)Location, out var areaWeather))
            return areaWeather.IsMarkCompatible(mark);
        return false;
    }

    /// <summary>
    /// Location IDs matched with possible weather types.
    /// </summary>
    internal static readonly Dictionary<byte, AreaWeather9> AreaWeather = new()
    {
        {   6, Standard },                       // South Province (Area One)
        {  10, Standard },                       // Pokémon League
        {  12, Standard },                       // South Province (Area Two)
        {  14, Standard },                       // South Province (Area Four)
        {  16, Standard },                       // South Province (Area Six)
        {  18, Standard },                       // South Province (Area Five)
        {  20, Standard },                       // South Province (Area Three)
        {  22, Standard },                       // West Province (Area One)
        {  24, Sand },                           // Asado Desert
        {  26, Standard },                       // West Province (Area Two)
        {  28, Standard },                       // West Province (Area Three)
        {  30, Standard },                       // Tagtree Thicket
        {  32, Standard },                       // East Province (Area Three)
        {  34, Standard },                       // East Province (Area One)
        {  36, Standard },                       // East Province (Area Two)
        {  38, Snow },                           // Glaseado Mountain (1)
        {  40, Standard },                       // Casseroya Lake
        {  44, Standard },                       // North Province (Area Three)
        {  46, Standard },                       // North Province (Area One)
        {  48, Standard },                       // North Province (Area Two)
        {  50, Standard },                       // Great Crater of Paldea
        {  56, Standard },                       // South Paldean Sea
        {  58, Standard },                       // West Paldean Sea
        {  60, Standard },                       // East Paldean Sea
        {  62, Standard },                       // North Paldean Sea
        {  64, Inside },                         // Inlet Grotto
        {  67, Inside },                         // Alfornada Cavern
        {  69, Standard | Inside | Snow | Snow },// Dalizapa Passage (Near Medali, Tunnels, Near Pokémon Center, Near Zapico)
        {  70, Standard },                       // Poco Path
        {  80, Standard },                       // Cabo Poco
        { 109, Standard },                       // Socarrat Trail
        { 124, Inside },                         // Area Zero (5)
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
            return 18; // Fancy Vivillon
        // flagged as totally random
        return (byte)Util.Rand.Next(PersonalTable.SV[Species].FormCount);
    }

    private void SetPINGA(PK9 pk, EncounterCriteria criteria, PersonalInfo9SV pi)
    {
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        criteria.SetRandomIVs(pk);

        pk.Nature = pk.StatNature = (int)criteria.GetNature(Nature.Random);
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
