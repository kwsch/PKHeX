using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Bindable summary object that can fetch strings that summarize a <see cref="PKM"/>.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class EntitySummary : IFatefulEncounterReadOnly // do NOT seal, allow inheritance
{
    private static readonly IReadOnlyList<string> GenderSymbols = GameInfo.GenderSymbolASCII;

    private readonly GameStrings Strings;
    private readonly ushort[] Stats;
    public readonly PKM Entity; // protected for children generating extra properties

    public virtual string Position => "???";
    public string Nickname => Entity.Nickname;
    public string Species => Get(Strings.specieslist, Entity.Species);
    public string Nature => Get(Strings.natures, (byte)Entity.StatNature);
    public string Gender => Get(GenderSymbols, Entity.Gender);
    public string ESV => Entity.PSV.ToString("0000");
    public string HP_Type => Get(Strings.types, Entity.HPType + 1);
    public string Ability => Get(Strings.abilitylist, Entity.Ability);
    public string Move1 => Get(Strings.movelist, Entity.Move1);
    public string Move2 => Get(Strings.movelist, Entity.Move2);
    public string Move3 => Get(Strings.movelist, Entity.Move3);
    public string Move4 => Get(Strings.movelist, Entity.Move4);
    public string HeldItem => GetSpan(Strings.GetItemStrings(Entity.Context), Entity.HeldItem);
    public string HP => Stats[0].ToString();
    public string ATK => Stats[1].ToString();
    public string DEF => Stats[2].ToString();
    public string SPA => Stats[4].ToString();
    public string SPD => Stats[5].ToString();
    public string SPE => Stats[3].ToString();
    public string MetLoc => Entity.GetLocationString(eggmet: false);
    public string EggLoc => Entity.GetLocationString(eggmet: true);
    public string Ball => Get(Strings.balllist, Entity.Ball);
    public string OT => Entity.OriginalTrainerName;
    public string Version => Get(Strings.gamelist, (int)Entity.Version);
    public string OTLang => ((LanguageID)Entity.Language).ToString();
    public string Legal { get { var la = new LegalityAnalysis(Entity); return la.Parsed ? la.Valid.ToString() : "-"; } }

    #region Extraneous
    public string EC => Entity.EncryptionConstant.ToString("X8");
    public string PID => Entity.PID.ToString("X8");
    public int IV_HP => Entity.IV_HP;
    public int IV_ATK => Entity.IV_ATK;
    public int IV_DEF => Entity.IV_DEF;
    public int IV_SPA => Entity.IV_SPA;
    public int IV_SPD => Entity.IV_SPD;
    public int IV_SPE => Entity.IV_SPE;
    public uint EXP => Entity.EXP;
    public int Level => Entity.CurrentLevel;
    public int EV_HP => Entity.EV_HP;
    public int EV_ATK => Entity.EV_ATK;
    public int EV_DEF => Entity.EV_DEF;
    public int EV_SPA => Entity.EV_SPA;
    public int EV_SPD => Entity.EV_SPD;
    public int EV_SPE => Entity.EV_SPE;
    public int Cool => Entity is IContestStatsReadOnly s ? s.ContestCool : 0;
    public int Beauty => Entity is IContestStatsReadOnly s ? s.ContestBeauty : 0;
    public int Cute => Entity is IContestStatsReadOnly s ? s.ContestCute : 0;
    public int Smart => Entity is IContestStatsReadOnly s ? s.ContestSmart : 0;
    public int Tough => Entity is IContestStatsReadOnly s ? s.ContestTough : 0;
    public int Sheen => Entity is IContestStatsReadOnly s ? s.ContestSheen : 0;

    public string NotOT => Entity.Format > 5 ? Entity.HandlingTrainerName : "N/A";

    public int AbilityNum => Entity.Format > 5 ? Entity.AbilityNumber : -1;
    public byte GenderFlag => Entity.Gender;
    public byte Form => Entity.Form;
    public int PokerusStrain => Entity.PokerusStrain;
    public int PokerusDays => Entity.PokerusDays;
    public int MetLevel => Entity.MetLevel;
    public byte OriginalTrainerGender => Entity.OriginalTrainerGender;

    public bool FatefulEncounter => Entity.FatefulEncounter;
    public bool IsEgg => Entity.IsEgg;
    public bool IsNicknamed => Entity.IsNicknamed;
    public bool IsShiny => Entity.IsShiny;

    public ushort TID16 => Entity.TID16;
    public ushort SID16 => Entity.SID16;
    public uint TSV => Entity.TSV;
    public int Move1_PP => Entity.Move1_PP;
    public int Move2_PP => Entity.Move2_PP;
    public int Move3_PP => Entity.Move3_PP;
    public int Move4_PP => Entity.Move4_PP;
    public int Move1_PPUp => Entity.Move1_PPUps;
    public int Move2_PPUp => Entity.Move2_PPUps;
    public int Move3_PPUp => Entity.Move3_PPUps;
    public int Move4_PPUp => Entity.Move4_PPUps;
    public string Relearn1 => Get(Strings.movelist, Entity.RelearnMove1);
    public string Relearn2 => Get(Strings.movelist, Entity.RelearnMove2);
    public string Relearn3 => Get(Strings.movelist, Entity.RelearnMove3);
    public string Relearn4 => Get(Strings.movelist, Entity.RelearnMove4);
    public ushort Checksum => Entity is ISanityChecksum s ? s.Checksum : Checksums.CRC16_CCITT(Entity.Data.AsSpan(Entity.SIZE_STORED));
    public int Friendship => Entity.OriginalTrainerFriendship;
    public int EggYear => Entity.EggMetDate.GetValueOrDefault().Year;
    public int EggMonth => Entity.EggMetDate.GetValueOrDefault().Month;
    public int EggDay => Entity.EggMetDate.GetValueOrDefault().Day;
    public int MetYear => Entity.MetDate.GetValueOrDefault().Year;
    public int MetMonth => Entity.MetDate.GetValueOrDefault().Month;
    public int MetDay => Entity.MetDate.GetValueOrDefault().Day;

    #endregion

    protected EntitySummary(PKM pk, GameStrings strings)
    {
        Entity = pk;
        Strings = strings;
        Stats = Entity.GetStats(Entity.PersonalInfo);
    }

    /// <summary>
    /// Safely fetches the string from the array.
    /// </summary>
    /// <param name="arr">Array of strings</param>
    /// <param name="val">Index to fetch</param>
    /// <returns>Null if array is null</returns>
    private static string Get(IReadOnlyList<string> arr, int val) => (uint)val < arr.Count ? arr[val] : string.Empty;
    private static string GetSpan(ReadOnlySpan<string> arr, int val) => (uint)val < arr.Length ? arr[val] : string.Empty;
}
