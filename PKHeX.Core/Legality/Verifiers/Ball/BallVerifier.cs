using System;
using static PKHeX.Core.LegalityCheckStrings;
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
        PK8 when enc.Version == GameVersion.PLA => Poke,

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
        if (current is Heavy && enc is not EncounterEgg && pk is { SM: true } && BallUseLegality.IsAlolanCaptureNoHeavyBall(enc.Species))
            return BadCaptureHeavy; // Heavy Ball, can inherit if from egg (US/UM fixed catch rate calc)

        return enc switch
        {
            EncounterInvalid => GetResult(true), // ignore ball, pass whatever
            EncounterSlot8GO g => GetResult(g.IsBallValid(current, pk.Species, pk)),
            IFixedBall { FixedBall: not None } s => VerifyBallEquals(current, s.FixedBall),
            EncounterSlot8 when pk is IRibbonSetMark8 { RibbonMarkCurry: true } or IRibbonSetAffixed { AffixedRibbon: (sbyte)RibbonIndex.MarkCurry }
                => GetResult(current is Poke or Great or Ultra),

            EncounterEgg => VerifyBallEgg(enc, current, pk), // Inheritance rules can vary.
            EncounterStatic5Entree => VerifyBallEquals(current, BallUseLegality.DreamWorldBalls),
            _ => VerifyBallEquals(current, BallUseLegality.GetWildBalls(enc.Generation, enc.Version)),
        };
    }

    private static BallVerificationResult VerifyBallEgg(IEncounterTemplate enc, Ball ball, PKM pk)
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

    private static BallVerificationResult VerifyBallInherited(IEncounterTemplate enc, Ball ball, PKM pk) => enc.Context switch
    {
        EntityContext.Gen6 => VerifyBallEggGen6(enc, ball, pk), // Gen6 Inheritance Rules
        EntityContext.Gen7 => VerifyBallEggGen7(enc, ball, pk), // Gen7 Inheritance Rules
        EntityContext.Gen8 => VerifyBallEggGen8(enc, ball),
        EntityContext.Gen8b => VerifyBallEggGen8BDSP(enc, ball),
        EntityContext.Gen9 => VerifyBallEggGen9(enc, ball),
        _ => BadEncounter,
    };

    private static BallVerificationResult VerifyBallEggGen6(IEncounterTemplate enc, Ball ball, PKM pk)
    {
        if (ball > Dream)
            return BadOutOfRange;

        var result = BallContext6.Instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEggGen7(IEncounterTemplate enc, Ball ball, PKM pk)
    {
        if (ball > Beast)
            return BadOutOfRange;

        var result = BallContext7.Instance.CanBreedWithBall(enc.Species, enc.Form, ball, pk);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEggGen8BDSP(IEncounterTemplate enc, Ball ball)
    {
        if (ball > Beast)
            return BadOutOfRange;

        var species = enc.Species;
        if (species is (int)Species.Spinda) // Can't transfer via HOME.
            return VerifyBallEquals(ball, BallUseLegality.WildPokeBalls4_HGSS);

        var result = BallContextHOME.Instance.CanBreedWithBall(species, enc.Form, ball);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEggGen8(IEncounterTemplate enc, Ball ball)
    {
        if (ball > Beast)
            return BadOutOfRange;

        var result = BallContextHOME.Instance.CanBreedWithBall(enc.Species, enc.Form, ball);
        return GetResult(result);
    }

    private static BallVerificationResult VerifyBallEggGen9(IEncounterTemplate enc, Ball ball)
    {
        if (ball > Beast)
            return BadOutOfRange;

        // Paldea Starters: Only via GO (Adventures Abound)
        var species = enc.Species;
        if (species is >= (int)Species.Sprigatito and <= (int)Species.Quaquaval)
            return VerifyBallEquals(ball, BallUseLegality.WildPokeballs8g_WithoutRaid);

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
        string msg = value.GetMessage();
        return Get(msg, valid ? Severity.Valid : Severity.Invalid);
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

    public static string GetMessage(this BallVerificationResult value) => value switch
    {
        ValidEncounter => LBallEnc,
        ValidInheritedSpecies => LBallSpeciesPass,
        BadEncounter => LBallEncMismatch,
        BadCaptureHeavy => LBallHeavy,
        BadInheritAbility => LBallAbility,
        BadInheritSpecies => LBallSpecies,
        BadInheritCherish => LBallEggCherish,
        BadInheritMaster => LBallEggMaster,
        BadOutOfRange => LBallUnavailable,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };
}
