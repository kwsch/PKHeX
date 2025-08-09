using System;
using static PKHeX.Core.Ball;

namespace PKHeX.Core;

/// <summary>
/// Contains logic to apply a new <see cref="Ball"/> value to a <see cref="PKM"/>.
/// </summary>
public static class BallApplicator
{
    private const Ball BallMin = Master; // first defined Enum value
    private const Ball BallMax = LAOrigin; // all indexes up to and including LAOrigin are defined Enum values.

    /// <summary>
    /// Maximum number of <see cref="Ball"/> values that can be returned in a span.
    /// </summary>
    public const byte MaxBallSpanAlloc = (byte)BallMax + 1;

    private static IEncounterTemplate Get(LegalityAnalysis la) => la.EncounterOriginal;

    /// <remarks>
    /// Requires checking the <see cref="LegalityAnalysis"/>.
    /// </remarks>
    /// <inheritdoc cref="GetLegalBalls(Span{Ball}, PKM, IEncounterTemplate)"/>
    public static int GetLegalBalls(Span<Ball> result, PKM pk) => GetLegalBalls(result, pk, new LegalityAnalysis(pk));

    /// <inheritdoc cref="GetLegalBalls(Span{Ball}, PKM, IEncounterTemplate)"/>
    public static int GetLegalBalls(Span<Ball> result, PKM pk, LegalityAnalysis la) => GetLegalBalls(result, pk, Get(la));

    /// <summary>
    /// Gets all balls that are legal for the input <see cref="PKM"/>.
    /// </summary>
    /// <param name="result">Result storage.</param>
    /// <param name="pk">Pokémon to retrieve a list of valid balls for.</param>
    /// <param name="enc">Encounter matched to.</param>
    /// <returns>Count of <see cref="Ball"/> values that the <see cref="PKM"/> is legal with.</returns>
    public static int GetLegalBalls(Span<Ball> result, PKM pk, IEncounterTemplate enc)
    {
        if (enc is EncounterInvalid)
            return 0;
        if (enc.Species is (ushort)Species.Nincada && pk.Species is (ushort)Species.Shedinja)
            return GetLegalBallsEvolvedShedinja(result, pk, enc);
        return LoadLegalBalls(result, pk, enc);
    }

    private static ReadOnlySpan<Ball> ShedinjaEvolve4 => [Sport, Poke];

    private static int GetLegalBallsEvolvedShedinja(Span<Ball> result, PKM pk, IEncounterTemplate enc)
    {
        switch (enc)
        {
            case EncounterSlot4 s4 when IsNincadaEvolveInOrigin(pk, s4):
                ShedinjaEvolve4.CopyTo(result);
                return ShedinjaEvolve4.Length;
            case EncounterSlot3 s3 when IsNincadaEvolveInOrigin(pk, s3):
                return LoadLegalBalls(result, pk, enc);
        }
        result[0] = Poke;
        return 1;
    }

    private static bool IsNincadaEvolveInOrigin(PKM pk, IEncounterTemplate enc)
    {
        // Rough check to see if Nincada evolved in the origin context (Gen3/4).
        // Does not do PID/IV checks to know the original met level.
        var current = pk.CurrentLevel;
        var met = pk.MetLevel;
        if (pk.Format == enc.Generation)
            return current > met;
        return enc.LevelMin != met && current > enc.LevelMin;
    }

    private static int LoadLegalBalls(Span<Ball> result, PKM pk, IEncounterTemplate enc)
    {
        int ctr = 0;
        for (var b = BallMin; b <= BallMax; b++)
        {
            if (BallVerifier.VerifyBall(enc, b, pk).IsValid())
                result[ctr++] = b;
        }
        return ctr;
    }

    /// <remarks>
    /// Requires checking the <see cref="LegalityAnalysis"/>.
    /// </remarks>
    /// <inheritdoc cref="ApplyBallLegalRandom(PKM, IEncounterTemplate)"/>
    public static byte ApplyBallLegalRandom(PKM pk) => ApplyBallLegalRandom(pk, new LegalityAnalysis(pk));

    /// <inheritdoc cref="ApplyBallLegalRandom(PKM, IEncounterTemplate)"/>
    public static byte ApplyBallLegalRandom(PKM pk, LegalityAnalysis la) => ApplyBallLegalRandom(pk, Get(la));

    /// <summary>
    /// Applies a random legal ball value if any exist.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="enc">Encounter matched to.</param>
    public static byte ApplyBallLegalRandom(PKM pk, IEncounterTemplate enc)
    {
        Span<Ball> balls = stackalloc Ball[MaxBallSpanAlloc];
        var count = GetLegalBalls(balls, pk, enc);
        balls = balls[..count];
        Util.Rand.Shuffle(balls);
        return ApplyFirstLegalBall(pk, balls, []);
    }

    /// <remarks>
    /// Requires checking the <see cref="LegalityAnalysis"/>.
    /// </remarks>
    /// <inheritdoc cref="ApplyBallLegalByColor(PKM, IEncounterTemplate, PersonalColor)"/>
    public static byte ApplyBallLegalByColor(PKM pk) => ApplyBallLegalByColor(pk, PersonalColorUtil.GetColor(pk));
    /// <inheritdoc cref="ApplyBallLegalByColor(PKM, IEncounterTemplate, PersonalColor)"/>
    public static byte ApplyBallLegalByColor(PKM pk, PersonalColor color) => ApplyBallLegalByColor(pk, new LegalityAnalysis(pk), color);
    /// <inheritdoc cref="ApplyBallLegalByColor(PKM, IEncounterTemplate, PersonalColor)"/>
    public static byte ApplyBallLegalByColor(PKM pk, LegalityAnalysis la, PersonalColor color) => ApplyBallLegalByColor(pk, Get(la), color);

    /// <summary>
    /// Applies a legal ball value if any exist, ordered by color.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="enc">Encounter matched to.</param>
    /// <param name="color">Color preference to order by.</param>
    public static byte ApplyBallLegalByColor(PKM pk, IEncounterTemplate enc, PersonalColor color)
    {
        Span<Ball> balls = stackalloc Ball[MaxBallSpanAlloc];
        var count = GetLegalBalls(balls, pk, enc);
        balls = balls[..count];
        var prefer = PersonalColorUtil.GetPreferredByColor(enc, color);
        return ApplyFirstLegalBall(pk, balls, prefer);
    }

    private static byte ApplyFirstLegalBall(PKM pk, Span<Ball> legal, ReadOnlySpan<Ball> prefer)
    {
        foreach (var ball in prefer)
        {
            if (Contains(legal, ball))
                return pk.Ball = (byte)ball;
        }
        foreach (var ball in legal)
        {
            if (!Contains(prefer, ball))
                return pk.Ball = (byte)ball;
        }
        return pk.Ball; // fail

        static bool Contains(ReadOnlySpan<Ball> balls, Ball ball)
        {
            foreach (var b in balls)
            {
                if (b == ball)
                    return true;
            }
            return false;
        }
    }
}
