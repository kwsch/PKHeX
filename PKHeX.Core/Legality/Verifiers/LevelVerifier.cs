using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.CurrentLevel"/>.
/// </summary>
public sealed class LevelVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Level;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterOriginal;
        if (enc is MysteryGift gift)
        {
            if (!IsMetLevelMatchEncounter(gift, pk))
            {
                data.AddLine(GetInvalid(LevelMetGift));
                return;
            }
            if (gift.Level > pk.CurrentLevel)
            {
                data.AddLine(GetInvalid(LevelMetGiftFail, gift.Level));
                return;
            }
        }

        if (pk.IsEgg)
        {
            if (pk.CurrentLevel != enc.LevelMin)
            {
                data.AddLine(GetInvalid(EggFMetLevel_0, enc.LevelMin));
                return;
            }

            var reqEXP = enc is EncounterStatic2 { DizzyPunchEgg: true }
                ? 125 // Gen2 Dizzy Punch gifts always have 125 EXP, even if it's more than the Lv5 exp required.
                : Experience.GetEXP(enc.LevelMin, data.PersonalInfo.EXPGrowth);
            if (reqEXP != pk.EXP)
                data.AddLine(GetInvalid(EggEXP, reqEXP));
            return;
        }

        var lvl = pk.CurrentLevel;
        if (lvl >= 100)
        {
            var expect = Experience.GetEXP(100, data.PersonalInfo.EXPGrowth);
            if (pk.EXP != expect)
                data.AddLine(GetInvalid(Identifier, LevelEXPTooHigh, expect));
        }

        if (lvl < pk.MetLevel)
            data.AddLine(GetInvalid(LevelMetBelow, lvl));
        else if (!enc.IsWithinEncounterRange(pk) && lvl != 100 && pk.EXP == Experience.GetEXP(lvl, data.PersonalInfo.EXPGrowth))
            data.AddLine(Get(Severity.Fishy, LevelEXPThreshold));
        else
            data.AddLine(GetValid(LevelMetSane));
    }

    private static bool IsMetLevelMatchEncounter(MysteryGift gift, PKM pk)
    {
        if (gift.Level == pk.MetLevel)
            return true;
        if (!pk.HasOriginalMetLocation)
            return true;

        return gift switch
        {
            WC7 wc7 when wc7.MetLevel == pk.MetLevel => true,
            PGT { IsManaphyEgg: true } when pk.MetLevel == 0 => true,
            _ => false,
        };
    }

    public void VerifyG1(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;
        if (pk.IsEgg)
        {
            if (pk.CurrentLevel != EncounterEgg2.Level)
                data.AddLine(GetInvalid(EggFMetLevel_0, EncounterEgg2.Level));
            return;
        }
        if (pk.MetLocation != 0) // crystal
        {
            var lvl = pk.CurrentLevel;
            if (lvl < pk.MetLevel)
                data.AddLine(GetInvalid(LevelMetBelow, lvl));
        }

        if (IsTradeEvolutionRequired(data, enc))
        {
            // Pokémon has been traded illegally between games without evolving.
            // Trade evolution species IDs for Gen1 are sequential dex numbers.
            data.AddLine(GetInvalid(EvoTradeReqOutsider_0, enc.Species + 1u));
        }
    }

    /// <summary>
    /// Checks if a Gen1 trade evolution must have occurred.
    /// </summary>
    private static bool IsTradeEvolutionRequired(LegalityAnalysis data, IEncounterTemplate enc)
    {
        // There is no way to prevent a Gen1 trade evolution, as held items (Everstone) did not exist.
        // Machoke, Graveler, Haunter and Kadabra captured in the second phase evolution, excluding in-game trades, are already checked
        var pk = data.Entity;
        var species = pk.Species;

        // This check is only applicable if it's a trade evolution that has not been evolved.
        if (enc.Species != species)
            return false;
        if (!GBRestrictions.IsTradeEvolution1(enc.Species))
            return false;

        // Context check is only applicable to Gen1/2; transferring to Gen2 is a trade.
        // Stadium 2 can transfer across game/generation boundaries without initiating a trade.
        // Ignore this check if the environment's loaded trainer is not from Gen1/2 or is from GB Era.
        if (ParseSettings.AllowGBStadium2 || ParseSettings.ActiveTrainer is { Generation: not (1 or 2) })
            return false;

        var moves = data.Info.Moves;
        // Gen2 stuff can be traded between Gen2 games holding an Everstone, assuming it hasn't been transferred to Gen1 for special moves.
        if (enc.Generation == 2)
            return MoveInfo.IsAnyFromGeneration(1, moves);
        // Gen1 stuff can only be un-evolved if it was never traded from the OT.
        if (MoveInfo.IsAnyFromGeneration(2, moves))
            return true; // traded to Gen2 for special moves
        if (pk.Format != 1)
            return true; // traded to Gen2 (current state)

        if (ParseSettings.ActiveTrainer is { } tr)
            return !tr.IsFromTrainer(pk); // not with OT
        return false;
    }
}
