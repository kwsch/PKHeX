using System;
using static PKHeX.Core.SlotType8b;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.BDSP"/>.
/// </summary>
public sealed record EncounterSlot8b(EncounterArea8b Parent, ushort Species, byte Form, byte LevelMin, byte LevelMax)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PB8>
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8b;
    public bool IsEgg => false;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public bool IsUnderground => Locations8b.IsUnderground(Parent.Location);
    public bool IsMarsh => Locations8b.IsMarsh(Parent.Location);
    public Ball FixedBall => GetRequiredBall();
    private Ball GetRequiredBall(Ball fallback = Ball.None) => IsMarsh ? Ball.Safari : fallback;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType8b Type => Parent.Type;

    public bool CanUseRadar => Type is Grass && !IsUnderground && !IsMarsh && CanUseRadarOverworld(Location);

    private static bool CanUseRadarOverworld(ushort location) => location switch
    {
        195 or 196 => false, // Oreburgh Mine
        203 or 204 or 205 or 208 or 209 or 210 or 211 or 212 or 213 or 214 or 215 => false, // Mount Coronet, 206/207 exterior
        >= 225 and <= 243 => false, // Solaceon Ruins
        244 or 245 or 246 or 247 or 248 or 249 => false, // Victory Road
        252 => false, // Ravaged Path
        255 or 256 => false, // Oreburgh Gate
        260 or 261 or 262 => false, // Stark Mountain, 259 exterior
        >= 264 and <= 284 => false, // Turnback Cave
        286 or 287 or 288 or 289 or 290 or 291 => false, // Snowpoint Temple
        292 or 293 => false, // Wayward Cave
        294 or 295 => false, // Ruin Maniac Cave
        296 => false, // Maniac Tunnel
        299 or 300 or 301 or 302 or 303 or 304 or 305 => false, // Iron Island, 298 exterior
        306 or 307 or 308 or 309 or 310 or 311 or 312 or 313 or 314 => false, // Old Château
        368 or 369 or 370 or 371 or 372 => false, // Route 209 (Lost Tower)
        _ => true,
    };

    private HiddenAbilityPermission IsHiddenAbilitySlot() => CanUseRadar
        ? HiddenAbilityPermission.Possible
        : HiddenAbilityPermission.Never;

    public AbilityPermission Ability => IsHiddenAbilitySlot() switch
    {
        HiddenAbilityPermission.Never => AbilityPermission.Any12,
        HiddenAbilityPermission.Always => AbilityPermission.OnlyHidden,
        _ => AbilityPermission.Any12H,
    };

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PB8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PB8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pi = PersonalTable.BDSP[Species, Form];
        var pk = new PB8
        {
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            Version = Version,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)GetRequiredBall(Ball.Poke),

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            OriginalTrainerFriendship = pi.BaseFriendship,
        };
        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        if (IsUnderground && GetBaseEggMove(out var move1, pi))
            pk.RelearnMove1 = move1;
        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PB8 pk, EncounterCriteria criteria, PersonalInfo8BDSP pi)
    {
        var rnd = Util.Rand;
        pk.PID = rnd.Rand32();
        pk.EncryptionConstant = rnd.Rand32();
        criteria.SetRandomIVs(pk);
        pk.Nature = pk.StatNature = criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));
    }

    public bool GetBaseEggMove(out ushort move) => GetBaseEggMove(out move, PersonalTable.BDSP[Species, Form]);

    private static bool GetBaseEggMove(out ushort move, PersonalInfo8BDSP pi)
    {
        var species = pi.HatchSpecies;
        var baseEgg = LearnSource8BDSP.Instance.GetEggMoves(species, 0);
        if (baseEgg.Length == 0)
        {
            move = 0;
            return false;
        }

        // Official method creates a new List<ushort>() with all the egg moves, removes all ignored, then picks a random index.
        // However, the "excluded egg moves" list was unreferenced in v1.0, so all egg moves are allowed.
        // We can't know which patch the encounter originated from, because they never added any new content.
        var rnd = Util.Rand;
        var index = rnd.Next(baseEgg.Length);
        move = baseEgg[index];
        return true;
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;

        if (Form != evo.Form && Species is not (int)Core.Species.Burmy)
            return false;

        // A/B/C tables, only Munchlax is a 'C' encounter, and A/B are accessible from any tree.
        // C table encounters are only available from 4 trees, which are determined by TID16/SID16 of the save file.
        if (IsInvalidMunchlaxTree(pk))
            return false;

        return true;
    }

    public bool IsInvalidMunchlaxTree(PKM pk)
    {
        if (Type is not HoneyTree)
            return false;
        return Species == (int)Core.Species.Munchlax && !Parent.IsMunchlaxTree(pk);
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;
        if (IsDeferredWurmple(pk))
            return EncounterMatchRating.PartialMatch;
        if (IsDeferredHiddenAbility(isHidden))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }

    private bool IsDeferredWurmple(PKM pk) => Species == (int)Core.Species.Wurmple && pk.Species != (int)Core.Species.Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pk);

    private bool IsDeferredHiddenAbility(bool IsHidden) => IsHiddenAbilitySlot() switch
    {
        HiddenAbilityPermission.Never => IsHidden,
        HiddenAbilityPermission.Always => !IsHidden,
        _ => false,
    };

    public bool CanBeUndergroundMove(ushort move)
    {
        var et = PersonalTable.BDSP;
        var sf = et.GetFormEntry(Species, Form);
        var species = sf.HatchSpecies;
        var baseEgg = LearnSource8BDSP.Instance.GetEggMoves(species, 0);
        if (baseEgg.Length == 0)
            return move == 0;
        return baseEgg.Contains(move);
    }
    #endregion
}
