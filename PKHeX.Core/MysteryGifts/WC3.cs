using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Mystery Gift Template File
/// </summary>
/// <remarks>
/// This is fabricated data built to emulate the future generation Mystery Gift objects.
/// Data here is not stored in any save file and cannot be naturally exported.
/// </remarks>
public sealed class WC3 : MysteryGift, IRibbonSetEvent3, ILangNicknamedTemplate
{
    public override MysteryGift Clone() => (WC3)MemberwiseClone();

    /// <summary>
    /// Matched <see cref="PIDIV"/> Type
    /// </summary>
    public PIDType Method;

    public override string OT_Name { get; set; } = string.Empty;
    public int OT_Gender { get; init; } = 3;
    public override int TID { get; set; }
    public override int SID { get; set; }
    public override int Location { get; set; } = 255;
    public override int EggLocation { get => 0; set {} }
    public override GameVersion Version { get; set; }
    public int Language { get; init; } = -1;
    public override int Species { get; set; }
    public override bool IsEgg { get; set; }
    public override IReadOnlyList<int> Moves { get; set; } = Array.Empty<int>();
    public bool NotDistributed { get; init; }
    public override Shiny Shiny { get; init; }
    public bool Fateful { get; init; } // Obedience Flag

    // Mystery Gift Properties
    public override int Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;
    public override byte Level { get; set; }
    public override int Ball { get; set; } = 4;
    public override bool IsShiny => Shiny == Shiny.Always;
    public override bool HasFixedIVs => false;
    public bool RibbonEarth { get; set; }
    public bool RibbonNational { get; set; }
    public bool RibbonCountry { get; set; }
    public bool RibbonChampionBattle { get; set; }
    public bool RibbonChampionRegional { get; set; }
    public bool RibbonChampionNational { get; set; }

    public string? Nickname { get; set; }

    // Description
    public override string CardTitle { get; set; } = "Generation 3 Event";
    public override string CardHeader => CardTitle;

    // Unused
    public override bool GiftUsed { get; set; }
    public override int CardID { get; set; }
    public override bool IsItem { get; set; }
    public override int ItemID { get; set; }
    public override bool IsEntity { get; set; } = true;
    public override bool Empty => false;
    public override int Gender { get; set; }
    public override int Form { get; set; }

    // Synthetic
    private readonly int? _metLevel;

    public int Met_Level
    {
        get => _metLevel ?? (IsEgg ? 0 : Level);
        init => _metLevel = value;
    }

    public override AbilityPermission Ability => AbilityPermission.Any12;

    public override PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        PK3 pk = new()
        {
            Species = Species,
            Met_Level = Met_Level,
            Met_Location = Location,
            Ball = 4,

            // Ribbons
            RibbonCountry = RibbonCountry,
            RibbonNational = RibbonNational,
            RibbonEarth = RibbonEarth,
            RibbonChampionBattle = RibbonChampionBattle,
            RibbonChampionRegional = RibbonChampionRegional,
            RibbonChampionNational = RibbonChampionNational,

            FatefulEncounter = Fateful,
            Version = GetVersion(tr),
        };
        pk.EXP = Experience.GetEXP(Level, pk.PersonalInfo.EXPGrowth);
        SetMoves(pk);

        bool hatchedEgg = IsEgg && tr.Generation != 3;
        if (hatchedEgg)
        {
            SetForceHatchDetails(pk, tr);
        }
        else
        {
            pk.OT_Gender = OT_Gender != 3 ? OT_Gender & 1 : tr.Gender;
            pk.TID = TID;
            pk.SID = SID;

            pk.Language = (int)GetSafeLanguage((LanguageID)tr.Language);
            pk.OT_Name = !string.IsNullOrWhiteSpace(OT_Name) ? OT_Name : tr.OT;
            if (IsEgg)
                pk.IsEgg = true; // lang should be set to japanese already
        }
        pk.Nickname = Nickname ?? SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, 3); // will be set to Egg nickname if appropriate by PK3 setter

        var pi = pk.PersonalInfo;
        pk.OT_Friendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        // Generate PIDIV
        SetPINGA(pk, criteria);
        pk.HeldItem = 0; // clear, only random for Jirachi (?), no loss

        if (Version == GameVersion.XD)
            pk.FatefulEncounter = true; // pk3 is already converted from xk3

        pk.RefreshChecksum();
        return pk;
    }

    private void SetForceHatchDetails(PK3 pk, ITrainerInfo sav)
    {
        pk.Language = (int)GetSafeLanguageNotEgg((LanguageID)sav.Language);
        pk.OT_Name = sav.OT;
        // ugly workaround for character table interactions
        if (string.IsNullOrWhiteSpace(pk.OT_Name))
        {
            pk.Language = (int)LanguageID.English;
            pk.OT_Name = "PKHeX";
        }

        pk.OT_Gender = sav.Gender;
        pk.TID = sav.TID;
        pk.SID = sav.SID;
        pk.Met_Location = pk.FRLG ? 146 /* Four Island */ : 32; // Route 117
        pk.FatefulEncounter &= pk.FRLG; // clear flag for RSE
        pk.Met_Level = 0; // hatched
    }

    private int GetVersion(ITrainerInfo sav)
    {
        if (Version != 0)
            return (int) GetRandomVersion(Version);
        bool gen3 = sav.Game <= 15 && GameVersion.Gen3.Contains((GameVersion)sav.Game);
        return gen3 ? sav.Game : (int)GameVersion.R;
    }

    private void SetMoves(PK3 pk)
    {
        if (Moves.Count == 0) // not completely defined
            Moves = MoveList.GetBaseEggMoves(pk, Species, Form, (GameVersion)pk.Version, Level);
        if (Moves.Count != 4)
        {
            int[] moves = Moves.ToArray();
            Array.Resize(ref moves, 4);
            Moves = moves;
        }

        var m = (int[])Moves;
        pk.SetMoves(m);
        pk.SetMaximumPPCurrent(m);
    }

    private void SetPINGA(PK3 pk, EncounterCriteria _)
    {
        var seed = Util.Rand32();
        seed = TID == 06930 ? MystryMew.GetSeed(seed, Method) : GetSaneSeed(seed);
        PIDGenerator.SetValuesFromSeed(pk, Method, seed);
    }

    private uint GetSaneSeed(uint seed) => Method switch
    {
        PIDType.BACD_R => seed & 0x0000FFFF, // u16
        PIDType.BACD_R_S => seed & 0x000000FF, // u8
        _ => seed,
    };

    private LanguageID GetSafeLanguage(LanguageID hatchLang)
    {
        if (IsEgg)
            return LanguageID.Japanese;
        return GetSafeLanguageNotEgg(hatchLang);
    }

    private LanguageID GetSafeLanguageNotEgg(LanguageID hatchLang)
    {
        if (Language != -1)
            return (LanguageID) Language;
        if (hatchLang < LanguageID.Korean && hatchLang != LanguageID.Hacked)
            return hatchLang;
        return LanguageID.English; // fallback
    }

    private static GameVersion GetRandomVersion(GameVersion version)
    {
        if (version is <= GameVersion.CXD and > 0) // single game
            return version;

        return version switch
        {
            GameVersion.FRLG => GameVersion.FR + Util.Rand.Next(2), // or LG
            GameVersion.RS or GameVersion.RSE => GameVersion.S + Util.Rand.Next(2), // or R
            GameVersion.COLO or GameVersion.XD => GameVersion.CXD,
            _ => throw new Exception($"Unknown GameVersion: {version}"),
        };
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        // Gen3 Version MUST match.
        if (Version != 0 && !Version.Contains((GameVersion)pk.Version))
            return false;

        bool hatchedEgg = IsEgg && !pk.IsEgg;
        if (!hatchedEgg)
        {
            if (SID != -1 && SID != pk.SID) return false;
            if (TID != -1 && TID != pk.TID) return false;
            if (OT_Gender < 3 && OT_Gender != pk.OT_Gender) return false;
            var wcOT = OT_Name;
            if (!string.IsNullOrEmpty(wcOT))
            {
                if (wcOT.Length > 7) // Colosseum MATTLE Ho-Oh
                {
                    if (!GetIsValidOTMattleHoOh(wcOT, pk.OT_Name, pk is CK3))
                        return false;
                }
                else if (wcOT != pk.OT_Name)
                {
                    return false;
                }
            }
        }

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, pk.Format))
            return false;

        if (Language != -1 && Language != pk.Language) return false;
        if (Ball != pk.Ball) return false;
        if (Fateful != pk.FatefulEncounter)
        {
            // XD Gifts only at level 20 get flagged after transfer
            if (Version == GameVersion.XD != (pk is XK3))
                return false;
        }

        if (pk.IsNative)
        {
            if (hatchedEgg)
                return true; // defer egg specific checks to later.
            if (Met_Level != pk.Met_Level)
                return false;
            if (Location != pk.Met_Location)
                return false;
        }
        else
        {
            if (pk.IsEgg)
                return false;
            if (Level > pk.Met_Level)
                return false;
        }
        return true;
    }

    private static bool GetIsValidOTMattleHoOh(string wc, string ot, bool ck3)
    {
        if (ck3) // match original if still ck3, otherwise must be truncated 7char
            return wc == ot;
        return ot.Length == 7 && wc.StartsWith(ot, StringComparison.Ordinal);
    }

    protected override bool IsMatchDeferred(PKM pk) => Species != pk.Species;
    protected override bool IsMatchPartial(PKM pk) => false;

    public string GetNickname(int language) => Nickname ?? string.Empty;
    public bool GetIsNicknamed(int language) => Nickname != null;
    public bool CanBeAnyLanguage() => false;
    public bool CanHaveLanguage(int language) => Language == language;
    public bool CanHandleOT(int language) => IsEgg;
}
