using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.Ball;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.Ball"/> value.
/// </summary>
public sealed class BallVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Ball;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity.Format <= 2)
            return; // no ball info saved
        var result = VerifyBall(data);
        data.AddLine(result);
    }

    private static int IsReplacedBall(IVersion enc, PKM pk) => pk switch
    {
        // Trading from PLA origin -> SW/SH will replace the Legends: Arceus ball with a regular Poké Ball
        PK8 when enc.Version == GameVersion.PLA => (int)Poke,

        // No replacement done.
        _ => (int)None,
    };

    private CheckResult VerifyBall(LegalityAnalysis data)
    {
        var Info = data.Info;
        var enc = Info.EncounterMatch;

        var ball = IsReplacedBall(enc, data.Entity);
        if (ball != 0)
            return VerifyBallEquals(data, ball);

        // Capture / Inherit cases -- can be one of many balls
        var pk = data.Entity;
        if (pk.Species == (int)Species.Shedinja && enc.Species != (int)Species.Shedinja) // Shedinja. For Gen3, copy the ball from Nincada
        {
            // Only a Gen3 origin Shedinja can copy the wild ball.
            // Evolution chains will indicate if it could have existed as Shedinja in Gen3.
            // The special move verifier has a similar check!
            if (pk is { HGSS: true, Ball: (int)Sport }) // Can evolve in D/P to retain the HG/SS ball (separate byte) -- not able to be captured in any other ball
                return VerifyBallEquals(data, (int)Sport);
            if (Info.Generation != 3 || Info.EvoChainsAllGens.Gen3.Length != 2) // not evolved in Gen3 Nincada->Shedinja
                return VerifyBallEquals(data, (int)Poke); // Poké Ball Only
        }

        // Fixed ball cases -- can be only one ball ever
        switch (enc)
        {
            case IFixedBall { FixedBall: not None } s:
                return VerifyBallEquals(data, (byte)s.FixedBall);
            case EncounterSlot8GO: // Already a strict match
                return GetResult(true);
        }

        // Capturing with Heavy Ball is impossible in Sun/Moon for specific species.
        if (pk is { Ball: (int)Heavy, SM: true } && enc is not EncounterEgg && BallUseLegality.IsAlolanCaptureNoHeavyBall(enc.Species))
            return GetInvalid(LBallHeavy); // Heavy Ball, can inherit if from egg (US/UM fixed catch rate calc)

        return enc switch
        {
            EncounterStatic5Entree => VerifyBallEquals((Ball)pk.Ball, BallUseLegality.DreamWorldBalls),
            EncounterEgg => VerifyBallEgg(data),
            EncounterInvalid => VerifyBallEquals(data, pk.Ball), // ignore ball, pass whatever
            _ => VerifyBallEquals((Ball)pk.Ball, BallUseLegality.GetWildBalls(data.Info.Generation, enc.Version)),
        };
    }

    private CheckResult VerifyBallEgg(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var info = data.Info;
        if (info.Generation < 6) // No inheriting Balls
            return VerifyBallEquals(data, (int)Poke); // Must be Pokéball -- no ball inheritance.

        return pk.Ball switch
        {
            (int)Master => GetInvalid(LBallEggMaster), // Master Ball
            (int)Cherish => GetInvalid(LBallEggCherish), // Cherish Ball
            _ => VerifyBallInherited(data, info.EncounterMatch.Context),
        };
    }

    private CheckResult VerifyBallInherited(LegalityAnalysis data, EntityContext context) => context switch
    {
        EntityContext.Gen6 => VerifyBallEggGen6(data), // Gen6 Inheritance Rules
        EntityContext.Gen7 => VerifyBallEggGen7(data), // Gen7 Inheritance Rules
        EntityContext.Gen8 => VerifyBallEggGen8(data),
        EntityContext.Gen8b => VerifyBallEggGen8BDSP(data),
        EntityContext.Gen9 => VerifyBallEggGen9(data),
        _ => GetInvalid(LBallNone),
    };

    private CheckResult VerifyBallEggGen6(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var ball = (Ball)pk.Ball;
        var instance = BallContext6.Instance;
        if (ball > Dream)
            return GetInvalid(LBallUnavailable);

        var enc = data.EncounterMatch;
        var result = instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return result switch
        {
            BallInheritanceResult.Valid => GetValid(LBallSpeciesPass),
            BallInheritanceResult.BadAbility => GetInvalid(LBallAbility),
            _ => GetInvalid(LBallSpecies),
        };
    }

    private CheckResult VerifyBallEggGen7(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var ball = (Ball)pk.Ball;
        var instance = BallContext7.Instance;
        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        var enc = data.EncounterMatch;
        var result = instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return result switch
        {
            BallInheritanceResult.Valid => GetValid(LBallSpeciesPass),
            BallInheritanceResult.BadAbility => GetInvalid(LBallAbility),
            _ => GetInvalid(LBallSpecies),
        };
    }

    private CheckResult VerifyBallEggGen8BDSP(LegalityAnalysis data)
    {
        var species = data.EncounterMatch.Species;
        var pk = data.Entity;
        var ball = (Ball)pk.Ball;
        if (species is (int)Species.Spinda) // Can't transfer via HOME.
            return VerifyBallEquals(ball, BallUseLegality.WildPokeBalls4_HGSS);

        var instance = BallContextHOME.Instance;
        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        var enc = data.EncounterMatch;
        var result = instance.CanBreedWithBall(species, enc.Form, ball, pk);
        return result switch
        {
            BallInheritanceResult.Valid => GetValid(LBallSpeciesPass),
            BallInheritanceResult.BadAbility => GetInvalid(LBallAbility),
            _ => GetInvalid(LBallSpecies),
        };
    }

    private CheckResult VerifyBallEggGen8(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var ball = (Ball)pk.Ball;
        var instance = BallContextHOME.Instance;
        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        var enc = data.EncounterMatch;
        var result = instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return result switch
        {
            BallInheritanceResult.Valid => GetValid(LBallSpeciesPass),
            BallInheritanceResult.BadAbility => GetInvalid(LBallAbility),
            _ => GetInvalid(LBallSpecies),
        };
    }

    private CheckResult VerifyBallEggGen9(LegalityAnalysis data)
    {
        var enc = data.EncounterMatch;
        var species = enc.Species;
        var pk = data.Entity;
        var ball = (Ball)pk.Ball;

        // Paldea Starters: Only via GO (Adventures Abound)
        if (species is >= (int)Species.Sprigatito and <= (int)Species.Quaquaval)
            return VerifyBallEquals(ball, BallUseLegality.WildPokeballs8g_WithoutRaid);

        // PLA Voltorb: Only via PLA (transfer only, not wild) and GO
        if (enc is { Species: (ushort)Species.Voltorb, Form: 1 })
            return VerifyBallEquals(ball, BallUseLegality.WildPokeballs8g_WithRaid);

        // S/V Tauros forms > 1: Only local Wild Balls for Blaze/Aqua breeds -- can't inherit balls from Kantonian/Combat.
        if (enc is { Species: (ushort)Species.Tauros, Form: > 1 })
            return VerifyBallEquals(ball, BallUseLegality.WildPokeballs9);

        var instance = BallContextHOME.Instance;
        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        var result = instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return result switch
        {
            BallInheritanceResult.Valid => GetValid(LBallSpeciesPass),
            BallInheritanceResult.BadAbility => GetInvalid(LBallAbility),
            _ => GetInvalid(LBallSpecies),
        };
    }

    private CheckResult VerifyBallEquals(LegalityAnalysis data, int ball) => GetResult(ball == data.Entity.Ball);
    private CheckResult VerifyBallEquals(Ball ball, ulong permit) => GetResult(BallUseLegality.IsBallPermitted(permit, (int)ball));

    private CheckResult GetResult(bool valid) => valid ? GetValid(LBallEnc) : GetInvalid(LBallEncMismatch);
}
