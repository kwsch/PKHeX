using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Bindable summary object that can fetch strings that summarize a <see cref="PKM"/>.
    /// </summary>
    public class PKMSummary // do NOT seal, allow inheritance
    {
        private static readonly IReadOnlyList<string> GenderSymbols = GameInfo.GenderSymbolASCII;

        private readonly GameStrings Strings;
        private readonly ushort[] Stats;
        protected readonly PKM pkm; // protected for children generating extra properties

        public string? Position => pkm.Identifier;
        public string Nickname => pkm.Nickname;
        public string Species => Get(Strings.specieslist, pkm.Species);
        public string Nature => Get(Strings.natures, pkm.StatNature);
        public string Gender => Get(GenderSymbols, pkm.Gender);
        public string ESV => pkm.PSV.ToString("0000");
        public string HP_Type => Get(Strings.types, pkm.HPType + 1);
        public string Ability => Get(Strings.abilitylist, pkm.Ability);
        public string Move1 => Get(Strings.movelist, pkm.Move1);
        public string Move2 => Get(Strings.movelist, pkm.Move2);
        public string Move3 => Get(Strings.movelist, pkm.Move3);
        public string Move4 => Get(Strings.movelist, pkm.Move4);
        public string HeldItem => Get(Strings.GetItemStrings(pkm.Format), pkm.HeldItem);
        public string HP => Stats[0].ToString();
        public string ATK => Stats[1].ToString();
        public string DEF => Stats[2].ToString();
        public string SPA => Stats[4].ToString();
        public string SPD => Stats[5].ToString();
        public string SPE => Stats[3].ToString();
        public string MetLoc => pkm.GetLocationString(eggmet: false);
        public string EggLoc => pkm.GetLocationString(eggmet: true);
        public string Ball => Get(Strings.balllist, pkm.Ball);
        public string OT => pkm.OT_Name;
        public string Version => Get(Strings.gamelist, pkm.Version);
        public string OTLang => Get(GameDataSource.Languages, pkm.Language);
        public string Legal { get { var la = new LegalityAnalysis(pkm); return la.Parsed ? la.Valid.ToString() : "-"; } }
        public string CountryID => pkm.Format > 5 ? pkm.Country.ToString() : "N/A";
        public string RegionID => pkm.Format > 5 ? pkm.Region.ToString() : "N/A";
        public string DSRegionID => pkm.Format > 5 ? pkm.ConsoleRegion.ToString() : "N/A";

        #region Extraneous
        public string EC => pkm.EncryptionConstant.ToString("X8");
        public string PID => pkm.PID.ToString("X8");
        public int HP_IV => pkm.IV_HP;
        public int ATK_IV => pkm.IV_ATK;
        public int DEF_IV => pkm.IV_DEF;
        public int SPA_IV => pkm.IV_SPA;
        public int SPD_IV => pkm.IV_SPD;
        public int SPE_IV => pkm.IV_SPE;
        public uint EXP => pkm.EXP;
        public int Level => pkm.CurrentLevel;
        public int HP_EV => pkm.EV_HP;
        public int ATK_EV => pkm.EV_ATK;
        public int DEF_EV => pkm.EV_DEF;
        public int SPA_EV => pkm.EV_SPA;
        public int SPD_EV => pkm.EV_SPD;
        public int SPE_EV => pkm.EV_SPE;
        public int Cool => pkm is IContestStats s ? s.CNT_Cool : 0;
        public int Beauty => pkm is IContestStats s ? s.CNT_Beauty : 0;
        public int Cute => pkm is IContestStats s ? s.CNT_Cute : 0;
        public int Smart => pkm is IContestStats s ? s.CNT_Smart : 0;
        public int Tough => pkm is IContestStats s ? s.CNT_Tough : 0;
        public int Sheen => pkm is IContestStats s ? s.CNT_Sheen : 0;
        public int Markings => pkm.MarkValue;

        public string NotOT => pkm.Format > 5 ? pkm.HT_Name : "N/A";

        public int AbilityNum => pkm.Format > 5 ? pkm.AbilityNumber : -1;
        public int GenderFlag => pkm.Gender;
        public int AltForms => pkm.AltForm;
        public int PKRS_Strain => pkm.PKRS_Strain;
        public int PKRS_Days => pkm.PKRS_Days;
        public int MetLevel => pkm.Met_Level;
        public int OT_Gender => pkm.OT_Gender;

        public bool FatefulFlag => pkm.FatefulEncounter;
        public bool IsEgg => pkm.IsEgg;
        public bool IsNicknamed => pkm.IsNicknamed;
        public bool IsShiny => pkm.IsShiny;

        public int TID => pkm.DisplayTID;
        public int SID => pkm.DisplaySID;
        public int TSV => pkm.TSV;
        public int Move1_PP => pkm.Move1_PP;
        public int Move2_PP => pkm.Move2_PP;
        public int Move3_PP => pkm.Move3_PP;
        public int Move4_PP => pkm.Move4_PP;
        public int Move1_PPUp => pkm.Move1_PPUps;
        public int Move2_PPUp => pkm.Move2_PPUps;
        public int Move3_PPUp => pkm.Move3_PPUps;
        public int Move4_PPUp => pkm.Move4_PPUps;
        public string Relearn1 => Get(Strings.movelist, pkm.RelearnMove1);
        public string Relearn2 => Get(Strings.movelist, pkm.RelearnMove2);
        public string Relearn3 => Get(Strings.movelist, pkm.RelearnMove3);
        public string Relearn4 => Get(Strings.movelist, pkm.RelearnMove4);
        public ushort Checksum => pkm.Checksum;
        public int Friendship => pkm.OT_Friendship;
        public int OT_Affection => pkm.OT_Affection;
        public int Egg_Year => pkm.EggMetDate.GetValueOrDefault().Year;
        public int Egg_Month => pkm.EggMetDate.GetValueOrDefault().Month;
        public int Egg_Day => pkm.EggMetDate.GetValueOrDefault().Day;
        public int Met_Year => pkm.MetDate.GetValueOrDefault().Year;
        public int Met_Month => pkm.MetDate.GetValueOrDefault().Month;
        public int Met_Day => pkm.MetDate.GetValueOrDefault().Day;
        public int Encounter => pkm.EncounterType;

        #endregion

        protected PKMSummary(PKM p, GameStrings strings)
        {
            pkm = p;
            Strings = strings;
            Stats = pkm.GetStats(pkm.PersonalInfo);
        }

        /// <summary>
        /// Safely fetches the string from the array.
        /// </summary>
        /// <param name="arr">Array of strings</param>
        /// <param name="val">Index to fetch</param>
        /// <returns>Null if array is null</returns>
        private static string Get(IReadOnlyList<string> arr, int val) => (uint)val < arr.Count ? arr[val] : string.Empty;
    }
}