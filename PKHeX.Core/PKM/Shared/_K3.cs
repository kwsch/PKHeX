namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Base <see cref="PKM"/> Class
    /// </summary>
    public abstract class _K3 : PKM, IRibbonSetEvent3, IRibbonSetCommon3, IRibbonSetUnique3, IRibbonSetOnly3
    {
        // Maximums
        public override int MaxMoveID => Legal.MaxMoveID_3;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public override int MaxAbilityID => Legal.MaxAbilityID_3;
        public override int MaxItemID => Legal.MaxItemID_3;
        public override int MaxBallID => Legal.MaxBallID_3;
        public override int MaxGameID => Legal.MaxGameID_3;
        public override int MaxIV => 31;
        public override int MaxEV => 255;
        public override int OTLength => 7;
        public override int NickLength => 10;

        // Generated Attributes
        public override int PSV => (int)((PID >> 16 ^ PID & 0xFFFF) >> 3);
        public override int TSV => (TID ^ SID) >> 3;
        public override bool Japanese => Language == (int)LanguageID.Japanese;
        public override bool WasEvent => Met_Location == 255; // Fateful
        public override bool WasIngameTrade => Met_Location == 254; // Trade
        public override bool WasGiftEgg => IsEgg && Met_Location == 253; // Gift Egg, indistinguible from normal eggs after hatch
        public override bool WasEventEgg => IsEgg && Met_Location == 255; // Event Egg, indistinguible from normal eggs after hatch

        public override int Ability { get { int[] abils = PersonalInfo.Abilities; return abils[abils[1] == 0 ? 0 : AbilityNumber >> 1]; } set { } }
        public override uint EncryptionConstant { get => PID; set { } }
        public override int Nature { get => (int)(PID % 25); set { } }
        public override int AltForm { get => Species == 201 ? PKX.GetUnownForm(PID) : 0; set { } }
        public override bool IsNicknamed { get => PKX.IsNicknamedAnyLanguage(Species, Nickname, Format); set { } }
        public override int Gender { get => PKX.GetGenderFromPID(Species, PID); set { } }
        public override int Characteristic => -1;
        public override int CurrentFriendship { get => OT_Friendship; set => OT_Friendship = value; }
        public override int CurrentHandler { get => 0; set { } }
        public override int Egg_Location { get => 0; set { } }

        public abstract bool RibbonEarth { get; set; }
        public abstract bool RibbonNational { get; set; }
        public abstract bool RibbonCountry { get; set; }
        public abstract bool RibbonChampionBattle { get; set; }
        public abstract bool RibbonChampionRegional { get; set; }
        public abstract bool RibbonChampionNational { get; set; }
        public abstract bool RibbonChampionG3Hoenn { get; set; }
        public abstract bool RibbonArtist { get; set; }
        public abstract bool RibbonEffort { get; set; }
        public abstract bool RibbonWinning { get; set; }
        public abstract bool RibbonVictory { get; set; }
        public abstract int RibbonCountG3Cool { get; set; }
        public abstract int RibbonCountG3Beauty { get; set; }
        public abstract int RibbonCountG3Cute { get; set; }
        public abstract int RibbonCountG3Smart { get; set; }
        public abstract int RibbonCountG3Tough { get; set; }
        public abstract bool RibbonWorld { get; set; }
        public abstract bool Unused1 { get; set; }
        public abstract bool Unused2 { get; set; }
        public abstract bool Unused3 { get; set; }
        public abstract bool Unused4 { get; set; }

        /// <summary>
        /// Interconversion for Generation 3 <see cref="PKM"/> formats.
        /// </summary>
        /// <typeparam name="T">Generation 3 format to convert to</typeparam>
        /// <returns>New object with transferred properties.</returns>
        protected T ConvertTo<T>() where T : _K3, new()
        {
            return new T // Convert away!
            {
                Species = Species,
                Language = Language,
                PID = PID,
                TID = TID,
                SID = SID,
                EXP = EXP,
                HeldItem = HeldItem,
                AbilityNumber = AbilityNumber,
                IsEgg = IsEgg,
                FatefulEncounter = FatefulEncounter,

                Met_Location = Met_Location,
                Met_Level = Met_Level,
                Version = Version,
                Ball = Ball,

                Nickname = Nickname,
                OT_Name = OT_Name,
                OT_Gender = OT_Gender,
                OT_Friendship = OT_Friendship,

                Move1_PPUps = Move1_PPUps,
                Move2_PPUps = Move2_PPUps,
                Move3_PPUps = Move3_PPUps,
                Move4_PPUps = Move4_PPUps,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Move1_PP = Move1_PP,
                Move2_PP = Move2_PP,
                Move3_PP = Move3_PP,
                Move4_PP = Move4_PP,

                IV_HP = IV_HP,
                IV_ATK = IV_ATK,
                IV_DEF = IV_DEF,
                IV_SPE = IV_SPE,
                IV_SPA = IV_SPA,
                IV_SPD = IV_SPD,
                EV_HP = EV_HP,
                EV_ATK = EV_ATK,
                EV_DEF = EV_DEF,
                EV_SPE = EV_SPE,
                EV_SPA = EV_SPA,
                EV_SPD = EV_SPD,
                CNT_Cool = CNT_Cool,
                CNT_Beauty = CNT_Beauty,
                CNT_Cute = CNT_Cute,
                CNT_Smart = CNT_Smart,
                CNT_Tough = CNT_Tough,
                CNT_Sheen = CNT_Sheen,

                PKRS_Days = PKRS_Days,
                PKRS_Strain = PKRS_Strain,

                // Transfer Ribbons
                RibbonCountG3Cool = RibbonCountG3Cool,
                RibbonCountG3Beauty = RibbonCountG3Beauty,
                RibbonCountG3Cute = RibbonCountG3Cute,
                RibbonCountG3Smart = RibbonCountG3Smart,
                RibbonCountG3Tough = RibbonCountG3Tough,
                RibbonChampionG3Hoenn = RibbonChampionG3Hoenn,
                RibbonWinning = RibbonWinning,
                RibbonVictory = RibbonVictory,
                RibbonArtist = RibbonArtist,
                RibbonEffort = RibbonEffort,
                RibbonChampionBattle = RibbonChampionBattle,
                RibbonChampionRegional = RibbonChampionRegional,
                RibbonChampionNational = RibbonChampionNational,
                RibbonCountry = RibbonCountry,
                RibbonNational = RibbonNational,
                RibbonEarth = RibbonEarth,
                RibbonWorld = RibbonWorld,
                Unused1 = Unused1,
                Unused2 = Unused2,
                Unused3 = Unused3,
                Unused4 = Unused4,
            };
        }
    }
}
