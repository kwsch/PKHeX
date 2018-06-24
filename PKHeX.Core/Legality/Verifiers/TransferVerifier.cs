using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class TransferVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Encounter;
        public override void Verify(LegalityAnalysis data)
        {
            throw new NotImplementedException();
        }

        public void VerifyTransferLegalityG3(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format == 4 && pkm.Met_Location != Legal.Transfer3) // Pal Park
                data.AddLine(GetInvalid(V60));
            if (pkm.Format != 4 && pkm.Met_Location != Legal.Transfer4)
                data.AddLine(GetInvalid(V61));
        }

        public void VerifyTransferLegalityG4(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            // Transfer Legality
            int loc = pkm.Met_Location;
            if (loc != 30001) // PokéTransfer
            {
                // Crown
                switch (pkm.Species)
                {
                    case 251: // Celebi
                        if (loc != Legal.Transfer4_CelebiUnused && loc != Legal.Transfer4_CelebiUsed)
                            data.AddLine(GetInvalid(V351));
                        break;
                    case 243: // Raikou
                    case 244: // Entei
                    case 245: // Suicune
                        if (loc != Legal.Transfer4_CrownUnused && loc != Legal.Transfer4_CrownUsed)
                            data.AddLine(GetInvalid(V351));
                        break;
                    default:
                        data.AddLine(GetInvalid(V61));
                        break;
                }
            }
        }

        public static IEnumerable<CheckResult> VerifyVCEncounter(PKM pkm, IEncounterable encounter, ILocation transfer, IList<CheckMoveResult> Moves)
        {
            // Check existing EncounterMatch
            if (encounter is EncounterInvalid || transfer == null)
                yield break; // Avoid duplicate invaild message

            if (encounter is EncounterStatic v && (GameVersion.GBCartEraOnly.Contains(v.Version) || v.Version == GameVersion.VCEvents))
            {
                bool exceptions = false;
                exceptions |= v.Version == GameVersion.VCEvents && encounter.Species == 151 && pkm.TID == 22796;
                if (!exceptions)
                    yield return new CheckResult(Severity.Invalid, V79, CheckIdentifier.Encounter);
            }

            if (pkm.Met_Location != transfer.Location)
                yield return new CheckResult(Severity.Invalid, V81, CheckIdentifier.Encounter);
            if (pkm.Egg_Location != transfer.EggLocation)
                yield return new CheckResult(Severity.Invalid, V59, CheckIdentifier.Encounter);

            // Flag Moves that cannot be transferred
            if (encounter is EncounterStatic s && s.Version == GameVersion.C && s.EggLocation == 256) // Dizzy Punch Gifts
            {
                // can't have Dizzy Punch at all
                int index = Array.IndexOf(pkm.Moves, 146); // Dizzy Punch
                if (index >= 0)
                {
                    var chk = Moves[index];
                    if (chk.Generation == 2) // not obtained from a future gen
                        Moves[index] = new CheckMoveResult(chk.Source, chk.Generation, Severity.Invalid, V82, CheckIdentifier.Move);
                }
            }

            bool checkShiny = pkm.VC2 || pkm.TradebackStatus == TradebackType.WasTradeback && pkm.VC1;
            if (!checkShiny)
                yield break;
            if (pkm.Gender == 1) // female
            {
                if (pkm.PersonalInfo.Gender == 31 && pkm.IsShiny) // impossible gender-shiny
                    yield return new CheckResult(Severity.Invalid, V209, CheckIdentifier.PID);
            }
            else if (pkm.Species == 201) // unown
            {
                if (pkm.AltForm != 8 && pkm.AltForm != 21 && pkm.IsShiny) // impossibly form-shiny (not I or V)
                    yield return new CheckResult(Severity.Invalid, V209, CheckIdentifier.PID);
            }
        }
    }
}
