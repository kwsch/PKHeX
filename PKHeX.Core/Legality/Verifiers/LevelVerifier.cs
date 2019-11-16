using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.CurrentLevel"/>.
    /// </summary>
    public sealed class LevelVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Level;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterOriginal;
            if (EncounterMatch is MysteryGift gift)
            {
                if (gift.Level != pkm.Met_Level && pkm.HasOriginalMetLocation)
                {
                    switch (gift)
                    {
                        case WC3 wc3 when wc3.Met_Level == pkm.Met_Level || wc3.IsEgg:
                            break;
                        case WC7 wc7 when wc7.MetLevel == pkm.Met_Level:
                            break;
                        default:
                            data.AddLine(GetInvalid(LLevelMetGift));
                            return;
                    }
                }
                if (gift.Level > pkm.CurrentLevel)
                {
                    data.AddLine(GetInvalid(LLevelMetGiftFail));
                    return;
                }
            }

            if (pkm.IsEgg)
            {
                int elvl = Legal.GetEggHatchLevel(pkm);
                if (elvl != pkm.CurrentLevel)
                {
                    data.AddLine(GetInvalid(string.Format(LEggFMetLevel_0, elvl)));
                    return;
                }

                var reqEXP = EncounterMatch is EncounterStatic s && s.Version == GameVersion.C
                    ? 125 // Gen2 Dizzy Punch gifts always have 125 EXP, even if it's more than the Lv5 exp required.
                    : Experience.GetEXP(elvl, pkm.PersonalInfo.EXPGrowth);
                if (reqEXP != pkm.EXP)
                    data.AddLine(GetInvalid(LEggEXP));
                return;
            }

            int lvl = pkm.CurrentLevel;
            if (lvl < pkm.Met_Level)
                data.AddLine(GetInvalid(LLevelMetBelow));
            else if (!EncounterMatch.IsWithinRange(pkm) && lvl != 100 && pkm.EXP == Experience.GetEXP(lvl, pkm.PersonalInfo.EXPGrowth))
                data.AddLine(Get(LLevelEXPThreshold, Severity.Fishy));
            else
                data.AddLine(GetValid(LLevelMetSane));
        }

        public void VerifyG1(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterOriginal;
            if (pkm.IsEgg)
            {
                int elvl = Legal.GetEggHatchLevel(pkm);
                if (elvl != pkm.CurrentLevel)
                    data.AddLine(GetInvalid(string.Format(LEggFMetLevel_0, elvl)));
                return;
            }
            if (pkm.Met_Location != 0) // crystal
            {
                int lvl = pkm.CurrentLevel;
                if (lvl < pkm.Met_Level)
                    data.AddLine(GetInvalid(LLevelMetBelow));
            }

            // There is no way to prevent a gen1 trade evolution as held items (everstone) did not exist.
            // Machoke, Graveler, Haunter and Kadabra captured in the second phase evolution, excluding in-game trades, are already checked
            if (pkm.Format <= 2 && !(EncounterMatch is EncounterTrade) && EncounterMatch.Species == pkm.Species && GBRestrictions.Trade_Evolution1.Contains(EncounterMatch.Species))
                VerifyG1TradeEvo(data);
        }

        private void VerifyG1TradeEvo(LegalityAnalysis data)
        {
            if (ParseSettings.ActiveTrainer.Generation >= 3)
                return; // context check is only applicable to gen1/2; transferring to gen2 is a trade.
            var pkm = data.pkm;
            var mustevolve = pkm.TradebackStatus == TradebackType.WasTradeback || (pkm.Format == 1 && !ParseSettings.IsFromActiveTrainer(pkm)) || GBRestrictions.IsTradedKadabraG1(pkm);
            if (!mustevolve)
                return;
            // Pokemon have been traded but it is not evolved, trade evos are sequential dex numbers
            var evolved = LegalityAnalysis.SpeciesStrings[pkm.Species + 1];
            var unevolved = LegalityAnalysis.SpeciesStrings[pkm.Species];
            data.AddLine(GetInvalid(string.Format(LEvoTradeReqOutsider, unevolved, evolved)));
        }
    }
}
