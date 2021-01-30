using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data with a fixed Catch Rate
    /// </summary>
    /// <remarks>
    /// Generation 1 specific value used in detecting unmodified/un-traded Generation 1 Trade Encounter data.
    /// Species & Minimum level (legal) possible to acquire at.
    /// </remarks>
    public sealed record EncounterTrade1 : EncounterTradeGB
    {
        public override int Generation => 1;
        public override int LevelMin => CanObtainMinGSC() ? LevelMinGSC : LevelMinRBY;

        private readonly int LevelMinRBY;
        private readonly int LevelMinGSC;

        public EncounterTrade1(int species, GameVersion game, int rby, int gsc) : base(species, gsc, game)
        {
            TrainerNames = StringConverter12.G1TradeOTName;

            LevelMinRBY = rby;
            LevelMinGSC = gsc;
        }

        public EncounterTrade1(int species, GameVersion game, int rby) : this(species, game, rby, rby) { }

        public byte GetInitialCatchRate()
        {
            var pt = Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
            return (byte)pt[Species].CatchRate;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            var pk1 = (PK1)pk;
            pk1.Catch_Rate = GetInitialCatchRate();
        }

        internal bool IsNicknameValid(PKM pkm)
        {
            var nick = pkm.Nickname;
            if (pkm.Format <= 2)
                return Nicknames.Contains(nick);

            // Converted string 1/2->7 to language specific value
            // Nicknames can be from any of the languages it can trade between.
            int lang = pkm.Language;
            if (lang == 1)
            {
                // Special consideration for Hiragana strings that are transferred
                if (Version == GameVersion.YW && Species == (int)Core.Species.Dugtrio)
                    return nick == "ぐりお";
                return nick == Nicknames[1];
            }

            return GetNicknameIndex(nick) >= 2;
        }

        internal bool IsTrainerNameValid(PKM pkm)
        {
            string ot = pkm.OT_Name;
            if (pkm.Format <= 2)
                return ot == StringConverter12.G1TradeOTStr;

            // Converted string 1/2->7 to language specific value
            int lang = pkm.Language;
            var tr = GetOT(lang);
            return ot == tr;
        }

        private int GetNicknameIndex(string nickname)
        {
            var nn = Nicknames;
            for (int i = 0; i < nn.Count; i++)
            {
                if (nn[i] == nickname)
                    return i;
            }
            return -1;
        }

        private bool CanObtainMinGSC()
        {
            if (!ParseSettings.AllowGen1Tradeback)
                return false;
            if (Version == GameVersion.BU && EvolveOnTrade)
                return ParseSettings.AllowGBCartEra;
            return true;
        }

        private bool IsMatchLevel(PKM pkm, int lvl)
        {
            if (pkm is not PK1)
                return lvl >= LevelMinGSC;
            return lvl >= LevelMin;
        }

        public override bool IsMatchExact(PKM pkm, DexLevel dl)
        {
            if (!IsMatchLevel(pkm, pkm.CurrentLevel)) // minimum required level
                return false;

            if (Version == GameVersion.BU)
            {
                // Encounters with this version have to originate from the Japanese Blue game.
                if (!pkm.Japanese)
                    return false;
                // Stadium 2 can transfer from GSC->RBY without a "Trade", thus allowing un-evolved outsiders
                if (EvolveOnTrade && !ParseSettings.AllowGBCartEra && pkm.CurrentLevel < LevelMinRBY)
                    return false;
            }

            return true;
        }

        protected override bool IsMatchPartial(PKM pkm)
        {
            if (!IsTrainerNameValid(pkm))
                return true;
            if (!IsNicknameValid(pkm))
                return true;

            if (ParseSettings.AllowGen1Tradeback)
                return false;
            if (pkm is not PK1 pk1)
                return false;

            var req = GetInitialCatchRate();
            return req != pk1.Catch_Rate;
        }
    }
}
