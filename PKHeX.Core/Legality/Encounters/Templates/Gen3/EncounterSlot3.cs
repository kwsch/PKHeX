using System;
using static PKHeX.Core.PIDType;
using static PKHeX.Core.SlotType3;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen3"/>.
/// </summary>
public record EncounterSlot3(EncounterArea3 Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax, byte SlotNumber, byte MagnetPullIndex, byte MagnetPullCount, byte StaticIndex, byte StaticCount)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK3>, IEncounterSlot3, IRandomCorrelation
{
    public byte Generation => 3;
    ushort ILocation.Location => Location;
    public EntityContext Context => EntityContext.Gen3;
    public bool EggEncounter => false;
    public Ball FixedBall => GetRequiredBall();
    public byte AreaRate => Parent.Rate;

    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public byte Location => Parent.Location;
    public SlotType3 Type => Parent.Type;
    public bool IsSafari => Locations.IsSafariZoneLocation3(Location);
    public bool IsSafariHoenn => Locations.IsSafariZoneLocation3RSE(Location);

    private Ball GetRequiredBall(Ball fallback = Ball.None) => IsSafari ? Ball.Safari : fallback;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = Version != GameVersion.RSE ? Version : GameVersion.RSE.Contains(tr.Version) ? tr.Version : GameVersion.E;
        var pi = PersonalTable.E[Species];
        var pk = new PK3
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = version,
            Ball = (byte)GetRequiredBall(Ball.Poke),

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria, pi);
        SetEncounterMoves(pk);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PK3 pk, EncounterCriteria criteria, PersonalInfo3 pi)
    {
        var gender = criteria.GetGender(pi);
        var nature = criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);
        var lvl = new SingleLevelRange(LevelMin);
        int ctr = 0;

        if (Species == (int)Core.Species.Unown)
        {
            if (criteria.IsSpecifiedIVs() && SetUnownFromIVs(pk, criteria))
                return;

            // Generate a random Unown with the correct form and desired nature.
            SetUnownRandom(pk, criteria);
            return;
        }

        do
        {
            var seed = PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, Method_1);
            var result = MethodH.GetSeed(this, seed, lvl, pk.E, pk.Gender, 3);
            if (result.IsValid())
                return;
        } while (ctr++ < 10_000);
    }

    private void SetUnownRandom(PK3 pk, EncounterCriteria criteria)
    {
        //bool checkForm = forms.Contains(criteria.Form); // not a property :(
        var (min, max) = SlotMethodH.GetRangeGrass(SlotNumber);
        var seed = Util.Rand32() & 0x7FFF_FFFF;
        // Can't game the seed with % 100 increments as Unown's form calculation is based on the PID.

        while (true)
        {
            var esv = LCRNG.Next16(ref seed) % 100;
            if (esv < min || esv > max)
                continue;
            // Skip the level roll, always results in the same level value.
            var rand = LCRNG.Next2(seed);
            if (criteria.Nature != Nature.Random && (rand >> 16) % 25 != (byte)criteria.Nature)
                continue;

            while (true)
            {
                var a = LCRNG.Next16(ref rand);
                var b = LCRNG.Next16(ref rand);
                var pid = a << 16 | b;
                var form = EntityPID.GetUnownForm3(pid);
                if (form != Form)
                    continue;

                pk.PID = pid;
                var iv1 = LCRNG.Next16(ref rand);
                var iv2 = LCRNG.Next16(ref rand);
                pk.IV32 = ((iv2 & 0x7FFF) << 15) | (iv1 & 0x7FFF);
                return;
            }
        }
    }

    private bool SetUnownFromIVs(PK3 pk, EncounterCriteria criteria)
    {
        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];
        var count = LCRNGReversal.GetSeedsIVs(seeds, (byte)criteria.IV_HP, (byte)criteria.IV_ATK, (byte)criteria.IV_DEF, (byte)criteria.IV_SPA, (byte)criteria.IV_SPD, (byte)criteria.IV_SPE);
        seeds = seeds[..count];
        foreach (ref var seed in seeds)
        {
            seed = LCRNG.Prev2(seed);
            var s = seed;

            var a = LCRNG.Next16(ref s);
            var b = LCRNG.Next16(ref s);
            var pid = a << 16 | b;
            if (criteria.Nature != Nature.Random && (Nature)(pid % 25) != criteria.Nature)
                continue;
            var form = EntityPID.GetUnownForm3(pid);
            if (form != Form)
                continue;
            var lead = MethodH.GetSeed(this, seed, this, false, 2, 3);
            if (!lead.IsValid()) // Verifies the slot and nature loop; if it passes, apply the details.
                continue;

            pk.PID = pid;
            var iv1 = LCRNG.Next16(ref s);
            var iv2 = LCRNG.Next16(ref s);
            pk.IV32 = ((iv2 & 0x7FFF) << 15) | (iv1 & 0x7FFF);
            return true;
        }

        return false;
    }

    protected virtual void SetEncounterMoves(PKM pk) => EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Form != evo.Form && Species is not (int)Core.Species.Burmy)
            return false;

        if (pk.Format == 3)
        {
            // Must match level exactly.
            if (!this.IsLevelWithinRange(pk.MetLevel))
                return false;
        }
        else
        {
            if (evo.LevelMax < LevelMin)
                return false;
        }

        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsDeferredSafari3(pk.Ball == (int)Ball.Safari))
            return EncounterMatchRating.PartialMatch;
        if (IsDeferredWurmple(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }
    private bool IsDeferredWurmple(PKM pk) => Species == (int)Core.Species.Wurmple && pk.Species != (int)Core.Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);

    private bool IsDeferredSafari3(bool IsSafariBall) => IsSafariBall != Locations.IsSafariZoneLocation3(Location);
    #endregion

    public bool IsCompatible(PIDType val, PKM pk)
    {
        if (Species != (int)Core.Species.Unown)
            return val is (Method_1 or Method_2 or Method_3 or Method_4);
        return val is (Method_1_Unown or Method_2_Unown or Method_3_Unown or Method_4_Unown);
    }

    public PIDType GetSuggestedCorrelation() => Species == (int)Core.Species.Unown ? Method_1_Unown : Method_1;

    public byte PressureLevel => Type != Grass ? LevelMax : Parent.GetPressureMax(Species, LevelMax);
}
