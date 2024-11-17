using System;
using static PKHeX.Core.Ball;

namespace PKHeX.Core;

/// <summary>
/// Contains logic to apply a new <see cref="Ball"/> value to a <see cref="PKM"/>.
/// </summary>
public static class BallApplicator
{
    private static readonly Ball[] BallList = Enum.GetValues<Ball>();
    public const byte MaxBallSpanAlloc = (byte)LAOrigin + 1;

    /// <remarks>
    /// Requires checking the <see cref="LegalityAnalysis"/>.
    /// </remarks>
    /// <inheritdoc cref="GetLegalBalls(Span{Ball}, PKM, IEncounterTemplate)"/>
    public static int GetLegalBalls(Span<Ball> result, PKM pk) => GetLegalBalls(result, pk, new LegalityAnalysis(pk));

    /// <inheritdoc cref="GetLegalBalls(Span{Ball}, PKM, IEncounterTemplate)"/>
    public static int GetLegalBalls(Span<Ball> result, PKM pk, LegalityAnalysis la) => GetLegalBalls(result, pk, la.EncounterOriginal);

    /// <summary>
    /// Gets all balls that are legal for the input <see cref="PKM"/>.
    /// </summary>
    /// <param name="result">Result storage.</param>
    /// <param name="pk">Pokémon to retrieve a list of valid balls for.</param>
    /// <param name="enc">Encounter matched to.</param>
    /// <returns>Count of <see cref="Ball"/> values that the <see cref="PKM"/> is legal with.</returns>
    public static int GetLegalBalls(Span<Ball> result, PKM pk, IEncounterTemplate enc)
    {
        if (enc.Species is (ushort)Species.Nincada && pk.Species is (ushort)Species.Shedinja)
            return GetLegalBallsEvolvedShedinja(result, pk, enc);
        return LoadLegalBalls(result, pk, enc);
    }

    private static ReadOnlySpan<Ball> ShedinjaEvolve4 => [Sport, Poke];

    private static int GetLegalBallsEvolvedShedinja(Span<Ball> result, PKM pk, IEncounterTemplate enc)
    {
        switch (enc)
        {
            case EncounterSlot4 when IsNincadaEvolveInOrigin(pk, enc):
                ShedinjaEvolve4.CopyTo(result);
                return ShedinjaEvolve4.Length;
            case EncounterSlot3 when IsNincadaEvolveInOrigin(pk, enc):
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
        foreach (var b in BallList)
        {
            if (BallVerifier.VerifyBall(enc, b, pk).IsValid())
                result[ctr++] = b;
        }
        return ctr;
    }

    /// <summary>
    /// Applies a random legal ball value if any exist.
    /// </summary>
    /// <remarks>
    /// Requires checking the <see cref="LegalityAnalysis"/>.
    /// </remarks>
    /// <param name="pk">Pokémon to modify.</param>
    public static byte ApplyBallLegalRandom(PKM pk)
    {
        Span<Ball> balls = stackalloc Ball[MaxBallSpanAlloc];
        var count = GetLegalBalls(balls, pk);
        balls = balls[..count];
        Util.Rand.Shuffle(balls);
        return ApplyFirstLegalBall(pk, balls, []);
    }

    public static byte ApplyBallLegalByColor(PKM pk) => ApplyBallLegalByColor(pk, PersonalColorUtil.GetColor(pk));
    public static byte ApplyBallLegalByColor(PKM pk, PersonalColor color) => ApplyBallLegalByColor(pk, new LegalityAnalysis(pk), color);
    public static byte ApplyBallLegalByColor(PKM pk, LegalityAnalysis la, PersonalColor color) => ApplyBallLegalByColor(pk, la.EncounterOriginal, color);

    /// <summary>
    /// Applies a legal ball value if any exist, ordered by color.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    /// <param name="enc">Encounter matched to.</param>
    /// <param name="color">Color preference to order by.</param>
    private static byte ApplyBallLegalByColor(PKM pk, IEncounterTemplate enc, PersonalColor color)
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
