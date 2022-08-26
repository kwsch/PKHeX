using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Bindable summary object that can fetch strings that summarize a <see cref="PKM"/>.
/// </summary>
public class EntitySummary // do NOT seal, allow inheritance
{
    private static readonly IReadOnlyList<string> GenderSymbols = GameInfo.GenderSymbolASCII;

    private readonly GameStrings Strings;
    private readonly ushort[] Stats;
    protected readonly PKM pk; // protected for children generating extra properties

    public virtual string Position => "???";
    public string Nickname => pk.Nickname;
    public string Species => Get(Strings.specieslist, pk.Species);
    public string Nature => Get(Strings.natures, pk.StatNature);
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
    public string OT => pk.OT_Name;
    public string Version => Get(Strings.gamelist, pk.Version);
    public string OTLang => ((LanguageID)pk.Language).ToString();
    public string Legal { get { var la = new LegalityAnalysis(pk); return la.Parsed ? la.Valid.ToString() : "-"; } }

    #region Extraneous
    public string EC => pk.EncryptionConstant.ToString("X8");
    public string PID => pk.PID.ToString("X8");
    public int HP_IV => pk.IV_HP;
    public int ATK_IV => pk.IV_ATK;
    public int DEF_IV => pk.IV_DEF;
    public int SPA_IV => pk.IV_SPA;
    public int SPD_IV => pk.IV_SPD;
    public int SPE_IV => pk.IV_SPE;
    public uint EXP => pk.EXP;
    public int Level => pk.CurrentLevel;
    public int HP_EV => pk.EV_HP;
    public int ATK_EV => pk.EV_ATK;
    public int DEF_EV => pk.EV_DEF;
    public int SPA_EV => pk.EV_SPA;
    public int SPD_EV => pk.EV_SPD;
    public int SPE_EV => pk.EV_SPE;
    public int Cool => pk is IContestStats s ? s.CNT_Cool : 0;
    public int Beauty => pk is IContestStats s ? s.CNT_Beauty : 0;
    public int Cute => pk is IContestStats s ? s.CNT_Cute : 0;
    public int Smart => pk is IContestStats s ? s.CNT_Smart : 0;
    public int Tough => pk is IContestStats s ? s.CNT_Tough : 0;
    public int Sheen => pk is IContestStats s ? s.CNT_Sheen : 0;
    public int Markings => pk.MarkValue;

    public string NotOT => pk.Format > 5 ? pk.HT_Name : "N/A";

    public int AbilityNum => pk.Format > 5 ? pk.AbilityNumber : -1;
    public int GenderFlag => pk.Gender;
    public byte Form => pk.Form;
    public int PKRS_Strain => pk.PKRS_Strain;
    public int PKRS_Days => pk.PKRS_Days;
    public int MetLevel => pk.Met_Level;
    public int OT_Gender => pk.OT_Gender;

    public bool FatefulFlag => pk.FatefulEncounter;
    public bool IsEgg => pk.IsEgg;
    public bool IsNicknamed => pk.IsNicknamed;
    public bool IsShiny => pk.IsShiny;

    public int TID => pk.DisplayTID;
    public int SID => pk.DisplaySID;
    public int TSV => pk.TSV;
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
    public int Friendship => pk.OT_Friendship;
    public int Egg_Year => pk.EggMetDate.GetValueOrDefault().Year;
    public int Egg_Month => pk.EggMetDate.GetValueOrDefault().Month;
    public int Egg_Day => pk.EggMetDate.GetValueOrDefault().Day;
    public int Met_Year => pk.MetDate.GetValueOrDefault().Year;
    public int Met_Month => pk.MetDate.GetValueOrDefault().Month;
    public int Met_Day => pk.MetDate.GetValueOrDefault().Day;

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
