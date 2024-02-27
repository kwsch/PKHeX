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
    protected readonly PKM pk; // protected for children generating extra properties

    public virtual string Position => "???";
    public string Nickname => pk.Nickname;
    public string Species => Get(Strings.specieslist, pk.Species);
    public string Nature => Get(Strings.natures, (byte)pk.StatNature);
    public string Gender => Get(GenderSymbols, pk.Gender);
    public string ESV => pk.PSV.ToString("0000");
    public string HP_Type => Get(Strings.types, pk.HPType + 1);
    public string Ability => Get(Strings.abilitylist, pk.Ability);
    public string Move1 => Get(Strings.movelist, pk.Move1);
    public string Move2 => Get(Strings.movelist, pk.Move2);
    public string Move3 => Get(Strings.movelist, pk.Move3);
    public string Move4 => Get(Strings.movelist, pk.Move4);
    public string HeldItem => GetSpan(Strings.GetItemStrings(pk.Context), pk.HeldItem);
    public string HP => Stats[0].ToString();
    public string ATK => Stats[1].ToString();
    public string DEF => Stats[2].ToString();
    public string SPA => Stats[4].ToString();
    public string SPD => Stats[5].ToString();
    public string SPE => Stats[3].ToString();
    public string MetLoc => pk.GetLocationString(eggmet: false);
    public string EggLoc => pk.GetLocationString(eggmet: true);
    public string Ball => Get(Strings.balllist, pk.Ball);
    public string OT => pk.OriginalTrainerName;
    public string Version => Get(Strings.gamelist, (int)pk.Version);
    public string OTLang => ((LanguageID)pk.Language).ToString();
    public string Legal { get { var la = new LegalityAnalysis(pk); return la.Parsed ? la.Valid.ToString() : "-"; } }

    #region Extraneous
    public string EC => pk.EncryptionConstant.ToString("X8");
    public string PID => pk.PID.ToString("X8");
    public int IV_HP => pk.IV_HP;
    public int IV_ATK => pk.IV_ATK;
    public int IV_DEF => pk.IV_DEF;
    public int IV_SPA => pk.IV_SPA;
    public int IV_SPD => pk.IV_SPD;
    public int IV_SPE => pk.IV_SPE;
    public uint EXP => pk.EXP;
    public int Level => pk.CurrentLevel;
    public int EV_HP => pk.EV_HP;
    public int EV_ATK => pk.EV_ATK;
    public int EV_DEF => pk.EV_DEF;
    public int EV_SPA => pk.EV_SPA;
    public int EV_SPD => pk.EV_SPD;
    public int EV_SPE => pk.EV_SPE;
    public int Cool => pk is IContestStatsReadOnly s ? s.ContestCool : 0;
    public int Beauty => pk is IContestStatsReadOnly s ? s.ContestBeauty : 0;
    public int Cute => pk is IContestStatsReadOnly s ? s.ContestCute : 0;
    public int Smart => pk is IContestStatsReadOnly s ? s.ContestSmart : 0;
    public int Tough => pk is IContestStatsReadOnly s ? s.ContestTough : 0;
    public int Sheen => pk is IContestStatsReadOnly s ? s.ContestSheen : 0;

    public string NotOT => pk.Format > 5 ? pk.HandlingTrainerName : "N/A";

    public int AbilityNum => pk.Format > 5 ? pk.AbilityNumber : -1;
    public byte GenderFlag => pk.Gender;
    public byte Form => pk.Form;
    public int PokerusStrain => pk.PokerusStrain;
    public int PokerusDays => pk.PokerusDays;
    public int MetLevel => pk.MetLevel;
    public byte OriginalTrainerGender => pk.OriginalTrainerGender;

    public bool FatefulEncounter => pk.FatefulEncounter;
    public bool IsEgg => pk.IsEgg;
    public bool IsNicknamed => pk.IsNicknamed;
    public bool IsShiny => pk.IsShiny;

    public ushort TID16 => pk.TID16;
    public ushort SID16 => pk.SID16;
    public uint TSV => pk.TSV;
    public int Move1_PP => pk.Move1_PP;
    public int Move2_PP => pk.Move2_PP;
    public int Move3_PP => pk.Move3_PP;
    public int Move4_PP => pk.Move4_PP;
    public int Move1_PPUp => pk.Move1_PPUps;
    public int Move2_PPUp => pk.Move2_PPUps;
    public int Move3_PPUp => pk.Move3_PPUps;
    public int Move4_PPUp => pk.Move4_PPUps;
    public string Relearn1 => Get(Strings.movelist, pk.RelearnMove1);
    public string Relearn2 => Get(Strings.movelist, pk.RelearnMove2);
    public string Relearn3 => Get(Strings.movelist, pk.RelearnMove3);
    public string Relearn4 => Get(Strings.movelist, pk.RelearnMove4);
    public ushort Checksum => pk is ISanityChecksum s ? s.Checksum : Checksums.CRC16_CCITT(pk.Data.AsSpan(pk.SIZE_STORED));
    public int Friendship => pk.OriginalTrainerFriendship;
    public int EggYear => pk.EggMetDate.GetValueOrDefault().Year;
    public int EggMonth => pk.EggMetDate.GetValueOrDefault().Month;
    public int EggDay => pk.EggMetDate.GetValueOrDefault().Day;
    public int MetYear => pk.MetDate.GetValueOrDefault().Year;
    public int MetMonth => pk.MetDate.GetValueOrDefault().Month;
    public int MetDay => pk.MetDate.GetValueOrDefault().Day;

    #endregion

    protected EntitySummary(PKM p, GameStrings strings)
    {
        pk = p;
        Strings = strings;
        Stats = pk.GetStats(pk.PersonalInfo);
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
