using System.Linq;
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
            var enc = data.EncounterOriginal;
            if (enc is MysteryGift gift)
            {
                if (gift.Level != pkm.Met_Level && pkm.HasOriginalMetLocation)
                {
                    switch (gift)
                    {
                        case WC3 wc3 when wc3.Met_Level == pkm.Met_Level || wc3.IsEgg:
                            break;
                        case WC7 wc7 when wc7.MetLevel == pkm.Met_Level:
                            break;
                        case PGT {IsManaphyEgg: true} when pkm.Met_Level == 0:
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
                int elvl = enc.LevelMin;
                if (elvl != pkm.CurrentLevel)
                {
                    data.AddLine(GetInvalid(string.Format(LEggFMetLevel_0, elvl)));
                    return;
                }

                var reqEXP = enc is EncounterStatic2Odd
                    ? 125 // Gen2 Dizzy Punch gifts always have 125 EXP, even if it's more than the Lv5 exp required.
                    : Experience.GetEXP(elvl, pkm.PersonalInfo.EXPGrowth);
                if (reqEXP != pkm.EXP)
                    data.AddLine(GetInvalid(LEggEXP));
                return;
            }

            int lvl = pkm.CurrentLevel;
            if (lvl >= 100)
            {
                var expect = Experience.GetEXP(100, pkm.PersonalInfo.EXPGrowth);
                if (pkm.EXP != expect)
                    data.AddLine(GetInvalid(LLevelEXPTooHigh));
            }

            if (lvl < pkm.Met_Level)
                data.AddLine(GetInvalid(LLevelMetBelow));
            else if (!enc.IsWithinEncounterRange(pkm) && lvl != 100 && pkm.EXP == Experience.GetEXP(lvl, pkm.PersonalInfo.EXPGrowth))
                data.AddLine(Get(LLevelEXPThreshold, Severity.Fishy));
            else
                data.AddLine(GetValid(LLevelMetSane));
        }

        public void VerifyG1(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var enc = data.EncounterMatch;
            if (pkm.IsEgg)
            {
                const int elvl = 5;
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

            if (IsTradeEvolutionRequired(data, enc))
            {
                // Pokemon has been traded illegally between games without evolving.
                // Trade evolution species IDs for Gen1 are sequential dex numbers.
                var species = enc.Species;
                var evolved = ParseSettings.SpeciesStrings[species + 1];
                var unevolved = ParseSettings.SpeciesStrings[species];
                data.AddLine(GetInvalid(string.Format(LEvoTradeReqOutsider, unevolved, evolved)));
            }
        }

        /// <summary>
        /// Checks if a Gen1 trade evolution must have occurred.
        /// </summary>
        private static bool IsTradeEvolutionRequired(LegalityAnalysis data, IEncounterTemplate enc)
        {
            // There is no way to prevent a Gen1 trade evolution, as held items (Everstone) did not exist.
            // Machoke, Graveler, Haunter and Kadabra captured in the second phase evolution, excluding in-game trades, are already checked
            var pkm = data.pkm;
            var species = pkm.Species;

            // This check is only applicable if it's a trade evolution that has not been evolved.
            if (!GBRestrictions.Trade_Evolution1.Contains(enc.Species) || enc.Species != species)
                return false;

            // Context check is only applicable to gen1/2; transferring to Gen2 is a trade.
            // Stadium 2 can transfer across game/generation boundaries without initiating a trade.
            // Ignore this check if the environment's loaded trainer is not from Gen1/2 or is from GB Era.
            if (ParseSettings.ActiveTrainer.Generation >= 3 || ParseSettings.AllowGBCartEra)
                return false;

            // Gen2 stuff can be traded between Gen2 games holding an Everstone, assuming it hasn't been transferred to Gen1 for special moves.
            if (enc.Generation == 2)
                return data.Info.Moves.All(z => z.Generation == 2);
            // Gen1 stuff can only be un-evolved if it was never traded from the OT.
            if (data.Info.Moves.Any(z => z.Generation != 1))
                return true; // traded to Gen2 for special moves
            if (pkm.Format != 1)
                return true; // traded to Gen2 (current state)
            return !ParseSettings.IsFromActiveTrainer(pkm); // not with OT
        }
    }
}
