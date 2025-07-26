using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.Ball;
using static PKHeX.Core.BallVerificationResult;

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
        data.AddLine(Localize(result));
    }

    private static Ball IsReplacedBall(IVersion enc, PKM pk) => pk switch
    {
        // Trading from PLA origin -> SW/SH will replace the Legends: Arceus ball with a regular Poké Ball
        // Enamorus is a special case where the ball is not replaced with a Poké Ball (it's a Cherish Ball)
        PK8 when enc.Version == GameVersion.PLA && enc is not IFixedBall { FixedBall: (> 0 and < Strange) } => Poke,

        // No replacement done.
        _ => NoBallReplace,
    };

    private const Ball NoBallReplace = None;

    public static BallVerificationResult VerifyBall(LegalityAnalysis data)
    {
        var info = data.Info;
        var enc = info.EncounterOriginal;
        var pk = data.Entity;

        Ball current = (Ball)pk.Ball;
        if (enc.Species == (int)Species.Nincada && pk.Species == (int)Species.Shedinja)
            return VerifyEvolvedShedinja(enc, current, pk, info);

        return VerifyBall(enc, current, pk);
    }

    private static BallVerificationResult VerifyEvolvedShedinja(IEncounterable enc, Ball current, PKM pk, LegalInfo info)
    {
        // Nincada evolving into Shedinja normally reverts to Poké Ball.

        // Gen3 Evolution: Copy current ball.
        if (enc is EncounterSlot3 && info.EvoChainsAllGens.Gen3.Length == 2)
            return VerifyBall(enc, current, pk);

        // Gen4 D/P/Pt: Retain the HG/SS ball (stored in a separate byte) -- can only be caught in BCC with Sport Ball.
        if (enc is EncounterSlot4 { Type: SlotType4.BugContest } && info.EvoChainsAllGens.Gen4.Length == 2)
            return GetResult(current is Sport or Poke);

        return VerifyBallEquals(current, Poke); // Poké Ball Only
    }

    /// <summary>
    /// Verifies the currently set ball for the <see cref="PKM"/>.
    /// </summary>
    /// <param name="enc">Encounter template</param>
    /// <param name="current">Current ball</param>
    /// <param name="pk">Misc details like Version and Ability</param>
    /// <remarks>Call this directly instead of the <see cref="LegalityAnalysis"/> overload if you've already ruled out the above cases needing Evolution chains.</remarks>
    public static BallVerificationResult VerifyBall(IEncounterTemplate enc, Ball current, PKM pk)
    {
        // Capture / Inherit cases -- can be one of many balls
        var ball = IsReplacedBall(enc, pk);
        if (ball != NoBallReplace)
            return VerifyBallEquals(current, ball);

        // Capturing with Heavy Ball is impossible in Sun/Moon for specific species.
        if (current is Heavy && enc is not EncounterEgg7 && pk is { SM: true } && BallUseLegality.IsAlolanCaptureNoHeavyBall(enc.Species))
            return BadCaptureHeavy; // Heavy Ball, can inherit if from egg (US/UM fixed catch rate calc)

        return enc switch
        {
            EncounterInvalid => GetResult(true), // ignore ball, pass whatever
            EncounterSlot8GO g => GetResult(g.IsBallValid(current, pk.Species, pk)),
            IFixedBall { FixedBall: not None } s => VerifyBallEquals(current, s.FixedBall),
            EncounterSlot8 when pk is IRibbonSetMark8 { RibbonMarkCurry: true } or IRibbonSetAffixed { AffixedRibbon: (sbyte)RibbonIndex.MarkCurry }
                => GetResult(current is Poke or Great or Ultra),

            IEncounterEgg egg => VerifyBallEgg(egg, current, pk), // Inheritance rules can vary.
            EncounterStatic5Entree => VerifyBallEquals(current, BallUseLegality.DreamWorldBalls),
            _ => VerifyBallEquals(current, BallUseLegality.GetWildBalls(enc.Generation, enc.Version)),
        };
    }

    private static BallVerificationResult VerifyBallEgg(IEncounterEgg enc, Ball ball, PKM pk)
    {
        if (enc.Generation < 6) // No inheriting Balls
            return VerifyBallEquals(ball, Poke); // Must be Poké Ball -- no ball inheritance.

        return ball switch
        {
            Master => BadInheritMaster,
            Cherish => BadInheritCherish,
            _ => VerifyBallInherited(enc, ball, pk),
        };
    }

    private static BallVerificationResult VerifyBallInherited(IEncounterEgg egg, Ball ball, PKM pk) => egg switch
    {
        EncounterEgg6 e6 => VerifyBallEggGen6(e6, ball, pk), // Gen6 Inheritance Rules
        EncounterEgg7 e7 => VerifyBallEggGen7(e7, ball, pk), // Gen7 Inheritance Rules
        EncounterEgg8 e8 => VerifyBallEggGen8(e8, ball),
        EncounterEgg8b b => VerifyBallEggGen8BDSP(b, ball),
        EncounterEgg9 e9 => VerifyBallEggGen9(e9, ball),
        _ => BadEncounter,
    };

    private static BallVerificationResult VerifyBallEggGen6(EncounterEgg6 enc, Ball ball, PKM pk)
    {
        if (ball > Dream)
            return BadOutOfRange;

        var result = BallContext6.Instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEggGen7(EncounterEgg7 enc, Ball ball, PKM pk)
    {
        if (ball > Beast)
            return BadOutOfRange;

        var result = BallContext7.Instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEggGen8BDSP(EncounterEgg8b enc, Ball ball)
    {
        if (ball > Beast)
            return BadOutOfRange;

        var species = enc.Species;
        if (species is (int)Species.Spinda) // Can't transfer via HOME.
            return VerifyBallEquals(ball, BallUseLegality.WildPokeBalls4_HGSS);

        var result = BallContextHOME.Instance.CanBreedWithBall(species, enc.Form, ball);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEggGen8(EncounterEgg8 enc, Ball ball)
    {
        if (ball > Beast)
            return BadOutOfRange;

        var result = BallContextHOME.Instance.CanBreedWithBall(enc.Species, enc.Form, ball);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEggGen9(EncounterEgg9 enc, Ball ball)
    {
        if (ball > Beast)
            return BadOutOfRange;

        var species = enc.Species;
        var result = BallContextHOME.Instance.CanBreedWithBall(species, enc.Form, ball);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEquals(Ball ball, Ball permit) => GetResult(ball == permit);
    private static BallVerificationResult VerifyBallEquals(Ball ball, ulong permit) => GetResult(BallUseLegality.IsBallPermitted(permit, (byte)ball));

    private static BallVerificationResult GetResult(bool valid) => valid ? ValidEncounter : BadEncounter;

    private static BallVerificationResult GetResult(BallInheritanceResult result) => result switch
    {
        BallInheritanceResult.Valid => ValidInheritedSpecies,
        BallInheritanceResult.BadAbility => BadInheritAbility,
        _ => BadInheritSpecies,
    };

    private CheckResult Localize(BallVerificationResult value)
    {
        bool valid = value.IsValid();
        var msg = value.GetMessage();
        return Get(valid ? Severity.Valid : Severity.Invalid, msg);
    }
}

public enum BallVerificationResult
{
    ValidEncounter,
    ValidInheritedSpecies,

    BadEncounter,
    BadCaptureHeavy,

    BadInheritAbility,
    BadInheritSpecies,
    BadInheritCherish,
    BadInheritMaster,

    BadOutOfRange,
}

public static class BallVerificationResultExtensions
{
    public static bool IsValid(this BallVerificationResult value) => value switch
    {
        ValidEncounter => true,
        ValidInheritedSpecies => true,
        _ => false,
    };

    public static LegalityCheckResultCode GetMessage(this BallVerificationResult value) => value switch
    {
        ValidEncounter => BallEnc,
        ValidInheritedSpecies => BallSpeciesPass,
        BadEncounter => BallEncMismatch,
        BadCaptureHeavy => BallHeavy,
        BadInheritAbility => BallAbility,
        BadInheritSpecies => BallSpecies,
        BadInheritCherish => BallEggCherish,
        BadInheritMaster => BallEggMaster,
        BadOutOfRange => BallUnavailable,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };
}
