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

    private static byte IsReplacedBall(IVersion enc, PKM pk) => pk switch
    {
        // Trading from PLA origin -> SW/SH will replace the Legends: Arceus ball with a regular Poké Ball
        PK8 when enc.Version == GameVersion.PLA => (int)Poke,

        // No replacement done.
        _ => NoBallReplace,
    };

    private const int NoBallReplace = (int)None;

    private CheckResult VerifyBall(LegalityAnalysis data)
    {
        var info = data.Info;
        var enc = info.EncounterOriginal;
        var pk = data.Entity;

        var ball = IsReplacedBall(enc, pk);
        if (ball != NoBallReplace)
            return VerifyBallEquals(pk, ball);

        // Capture / Inherit cases -- can be one of many balls
        if (pk.Species == (int)Species.Shedinja && enc.Species != (int)Species.Shedinja) // Shedinja. For Gen3, copy the ball from Nincada
        {
            // Only a Gen3 origin Shedinja can copy the wild ball.
            // Evolution chains will indicate if it could have existed as Shedinja in Gen3.
            // The special move verifier has a similar check!
            if (pk is { HGSS: true, Ball: (int)Sport }) // Can evolve in D/P to retain the HG/SS ball (separate byte) -- not able to be captured in any other ball
                return GetResult(true);
            if (enc.Generation != 3 || info.EvoChainsAllGens.Gen3.Length != 2) // not evolved in Gen3 Nincada->Shedinja
                return VerifyBallEquals(pk, (int)Poke); // Poké Ball Only
        }

        // Capturing with Heavy Ball is impossible in Sun/Moon for specific species.
        if (pk is { Ball: (int)Heavy, SM: true } && enc is not EncounterEgg && BallUseLegality.IsAlolanCaptureNoHeavyBall(enc.Species))
            return GetInvalid(LBallHeavy); // Heavy Ball, can inherit if from egg (US/UM fixed catch rate calc)

        return enc switch
        {
            EncounterSlot8GO => GetResult(true), // Already a strict match
            EncounterInvalid => GetResult(true), // ignore ball, pass whatever
            IFixedBall { FixedBall: not None } s => VerifyBallEquals(pk, (byte)s.FixedBall),

            EncounterEgg => VerifyBallEgg(pk, enc), // Inheritance rules can vary.
            EncounterStatic5Entree => VerifyBallEquals((Ball)pk.Ball, BallUseLegality.DreamWorldBalls),
            _ => VerifyBallEquals((Ball)pk.Ball, BallUseLegality.GetWildBalls(enc.Generation, enc.Version)),
        };
    }

    private CheckResult VerifyBallEgg(PKM pk, IEncounterTemplate enc)
    {
        if (enc.Generation < 6) // No inheriting Balls
            return VerifyBallEquals(pk, (int)Poke); // Must be Poké Ball -- no ball inheritance.

        return pk.Ball switch
        {
            (int)Master => GetInvalid(LBallEggMaster), // Master Ball
            (int)Cherish => GetInvalid(LBallEggCherish), // Cherish Ball
            _ => VerifyBallInherited(pk, enc),
        };
    }

    private CheckResult VerifyBallInherited(PKM pk, IEncounterTemplate enc) => enc.Context switch
    {
        EntityContext.Gen6 => VerifyBallEggGen6(pk, enc), // Gen6 Inheritance Rules
        EntityContext.Gen7 => VerifyBallEggGen7(pk, enc), // Gen7 Inheritance Rules
        EntityContext.Gen8 => VerifyBallEggGen8(pk, enc),
        EntityContext.Gen8b => VerifyBallEggGen8BDSP(pk, enc),
        EntityContext.Gen9 => VerifyBallEggGen9(pk, enc),
        _ => GetInvalid(LBallNone),
    };

    private CheckResult VerifyBallEggGen6(PKM pk, IEncounterTemplate enc)
    {
        var ball = (Ball)pk.Ball;
        if (ball > Dream)
            return GetInvalid(LBallUnavailable);

        var result = BallContext6.Instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return GetResult(result);
    }

    private CheckResult VerifyBallEggGen7(PKM pk, IEncounterTemplate enc)
    {
        var ball = (Ball)pk.Ball;
        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        var result = BallContext7.Instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return GetResult(result);
    }

    private CheckResult VerifyBallEggGen8BDSP(PKM pk, IEncounterTemplate enc)
    {
        var ball = (Ball)pk.Ball;
        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        var species = enc.Species;
        if (species is (int)Species.Spinda) // Can't transfer via HOME.
            return VerifyBallEquals(ball, BallUseLegality.WildPokeBalls4_HGSS);

        var result = BallContextHOME.Instance.CanBreedWithBall(species, enc.Form, ball);
        return GetResult(result);
    }

    private CheckResult VerifyBallEggGen8(PKM pk, IEncounterTemplate enc)
    {
        var ball = (Ball)pk.Ball;
        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        var result = BallContextHOME.Instance.CanBreedWithBall(enc.Species, enc.Form, ball);
        return GetResult(result);
    }

    private CheckResult VerifyBallEggGen9(PKM pk, IEncounterTemplate enc)
    {
        var ball = (Ball)pk.Ball;
        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        // Paldea Starters: Only via GO (Adventures Abound)
        var species = enc.Species;
        if (species is >= (int)Species.Sprigatito and <= (int)Species.Quaquaval)
            return VerifyBallEquals(ball, BallUseLegality.WildPokeballs8g_WithoutRaid);

        var result = BallContextHOME.Instance.CanBreedWithBall(species, enc.Form, ball);
        return GetResult(result);
    }

    private CheckResult VerifyBallEquals(PKM pk, byte ball) => GetResult(ball == pk.Ball);
    private CheckResult VerifyBallEquals(Ball ball, ulong permit) => GetResult(BallUseLegality.IsBallPermitted(permit, (byte)ball));

    private CheckResult GetResult(bool valid) => valid ? GetValid(LBallEnc) : GetInvalid(LBallEncMismatch);

    private CheckResult GetResult(BallInheritanceResult result) => result switch
    {
        BallInheritanceResult.Valid => GetValid(LBallSpeciesPass),
        BallInheritanceResult.BadAbility => GetInvalid(LBallAbility),
        _ => GetInvalid(LBallSpecies),
    };
}
