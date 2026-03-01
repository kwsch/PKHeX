using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscG1Verifier : Verifier
{
    private static readonly EggVerifier Eggs = new();

    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data) => VerifyG1(data, data.Entity);

    internal void VerifyG1(LegalityAnalysis data, PKM pk)
    {
        if (pk.IsEgg)
            Eggs.VerifyCommon(data, pk);

        if (pk is not PK1 pk1)
        {
            if (pk is ICaughtData2 { CaughtData: not 0 } t)
            {
                var time = t.MetTimeOfDay;
                bool valid = data.EncounterOriginal switch
                {
                    EncounterGift2 g2 when (!g2.IsEgg || pk.IsEgg) => time == 0,
                    EncounterTrade2 => time == 0,
                    _ => time is 1 or 2 or 3,
                };
                if (!valid)
                    data.AddLine(GetInvalid(Encounter, MetDetailTimeOfDay));
            }
            return;
        }

        VerifyMiscG1Types(data, pk1);
        VerifyMiscG1CatchRate(data, pk1);
    }

    private void VerifyMiscG1Types(LegalityAnalysis data, PK1 pk1)
    {
        var species = pk1.Species;
        if (species == (int)Species.Porygon)
        {
            // Can have any type combination of any species by using Conversion.
            if (!PersonalTable1.TypeIDExists(pk1.Type1))
            {
                data.AddLine(GetInvalid(G1TypePorygonFail1));
            }
            if (!PersonalTable1.TypeIDExists(pk1.Type2))
            {
                data.AddLine(GetInvalid(G1TypePorygonFail2));
            }
            else // Both types exist, ensure a Gen1 species has this combination
            {
                var matchSpecies = PersonalTable.RB.IsValidTypeCombination(pk1);
                var result = matchSpecies != -1 ? GetValid(G1TypeMatchPorygon) : GetInvalid(G1TypePorygonFail);
                data.AddLine(result);
            }
        }
        else // Types must match species types
        {
            var pi = PersonalTable.RB[species];
            var (match1, match2) = pi.IsMatchType(pk1);
            if (!match2 && ParseSettings.AllowGBStadium2)
                match2 = (species is (int)Species.Magnemite or (int)Species.Magneton) && pk1.Type2 == 9; // Steel Magnemite via Stadium2

            var first = match1 ? GetValid(G1TypeMatch1) : GetInvalid(G1Type1Fail);
            var second = match2 ? GetValid(G1TypeMatch2) : GetInvalid(G1Type2Fail);
            data.AddLine(first);
            data.AddLine(second);
        }
    }

    private void VerifyMiscG1CatchRate(LegalityAnalysis data, PK1 pk1)
    {
        var tradeback = GBRestrictions.IsTimeCapsuleTransferred(pk1, data.Info.Moves, data.EncounterMatch);
        var result = tradeback is TimeCapsuleEvaluation.NotTransferred or TimeCapsuleEvaluation.BadCatchRate
            ? GetWasNotTradeback(data, pk1, tradeback)
            : GetWasTradeback(data, pk1, tradeback);
        data.AddLine(result);
    }

    private CheckResult GetWasTradeback(LegalityAnalysis data, PK1 pk1, TimeCapsuleEvaluation eval)
    {
        var rate = pk1.CatchRate;
        if (ItemConverter.IsCatchRateHeldItem(rate))
            return GetValid(G1CatchRateMatchTradeback);
        return GetWasNotTradeback(data, pk1, eval);
    }

    private CheckResult GetWasNotTradeback(LegalityAnalysis data, PK1 pk1, TimeCapsuleEvaluation eval)
    {
        var rate = pk1.CatchRate;
        if (MoveInfo.IsAnyFromGeneration(EntityContext.Gen2, data.Info.Moves))
            return GetInvalid(G1CatchRateItem);
        var e = data.EncounterMatch;
        if (e is EncounterGift1 { Version: GameVersion.Stadium } or EncounterTrade1)
            return GetValid(G1CatchRateMatchPrevious); // Encounters detected by the catch rate, cant be invalid if match this encounters

        ushort species = pk1.Species;
        if (GBRestrictions.IsSpeciesNotAvailableCatchRate((byte)species) && rate == PersonalTable.RB[species].CatchRate)
        {
            if (species != (int)Species.Dragonite || rate != 45 || !(e.Version == GameVersion.BU || e.Version.Contains(GameVersion.YW)))
                return GetInvalid(G1CatchRateEvo);
        }
        if (!GBRestrictions.RateMatchesEncounter(e.Species, e.Version, rate))
            return GetInvalid(eval == TimeCapsuleEvaluation.Transferred12 ? G1CatchRateChain : G1CatchRateNone);
        return GetValid(G1CatchRateMatchPrevious);
    }
}
