using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class LevelVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Level;
        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
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
                            data.AddLine(GetInvalid(V83));
                            return;
                    }
                }
                if (gift.Level > pkm.CurrentLevel)
                {
                    data.AddLine(GetInvalid(V84));
                    return;
                }
            }

            if (pkm.IsEgg)
            {
                int elvl = Legal.GetEggHatchLevel(pkm);
                if (elvl != pkm.CurrentLevel)
                    data.AddLine(GetInvalid(string.Format(V52, elvl)));
                return;
            }

            int lvl = pkm.CurrentLevel;
            if (lvl < pkm.Met_Level)
                data.AddLine(GetInvalid(V85));
            else if (!EncounterMatch.IsWithinRange(pkm) && lvl != 100 && pkm.EXP == PKX.GetEXP(lvl, pkm.Species))
                data.AddLine(Get(V87, Severity.Fishy));
            else
                data.AddLine(GetValid(V88));
        }
        public void VerifyG1(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            if (pkm.IsEgg)
            {
                int elvl = Legal.GetEggHatchLevel(pkm);
                if (elvl != pkm.CurrentLevel)
                    data.AddLine(GetInvalid(string.Format(V52, elvl)));
                return;
            }
            if (pkm.Met_Location != 0) // crystal
            {
                int lvl = pkm.CurrentLevel;
                if (lvl < pkm.Met_Level)
                    data.AddLine(GetInvalid(V85));
            }

            // There is no way to prevent a gen1 trade evolution as held items (everstone) did not exist.
            // Machoke, Graveler, Haunter and Kadabra captured in the second phase evolution, excluding in-game trades, are already checked
            if (pkm.Format <= 2 && !(EncounterMatch is EncounterTrade) && EncounterMatch.Species == pkm.Species && Legal.Trade_Evolution1.Contains(EncounterMatch.Species))
                VerifyG1TradeEvo(data);
        }
        private void VerifyG1TradeEvo(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var mustevolve = pkm.TradebackStatus == TradebackType.WasTradeback || (pkm.Format == 1 && Legal.IsOutsider(pkm)) || Legal.IsTradedKadabraG1(pkm);
            if (!mustevolve)
                return;
            // Pokemon have been traded but it is not evolved, trade evos are sequential dex numbers
            var unevolved = LegalityAnalysis.SpeciesStrings[pkm.Species];
            var evolved = LegalityAnalysis.SpeciesStrings[pkm.Species + 1];
            data.AddLine(GetInvalid(string.Format(V405, unevolved, evolved)));
        }
    }
}
