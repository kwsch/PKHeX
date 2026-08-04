namespace PKHeX.Core.AutoMod;

/// <summary>
/// Suggestion edits that rely on a <see cref="LegalityAnalysis"/> being done.
/// </summary>
public static class LegalEdits
{
    public static bool ReplaceBallPrefixLA { get; set; }

    private static Ball GetBallLA(Ball ball) => ball switch
    {
        Ball.Poke => Ball.LAPoke,
        Ball.Great => Ball.LAGreat,
        Ball.Ultra => Ball.LAUltra,
        Ball.Heavy => Ball.LAHeavy,
        _ => ball,
    };

    /// <summary>
    /// Set a valid Pokeball based on a legality check's suggestions.
    /// </summary>
    /// <param name="pk">Pokémon to modify</param>
    /// <param name="enc"></param>
    /// <param name="matching">Set matching ball</param>
    /// <param name="force"></param>
    /// <param name="ball"></param>
    public static void SetSuggestedBall(this PKM pk, IEncounterTemplate enc, bool matching = true, bool force = false, Ball ball = Ball.None)
    {
        if (enc is MysteryGift)
            return;

        if (ball != Ball.None)
        {
            if (pk.LA && ReplaceBallPrefixLA && pk is not PK8)
                ball = GetBallLA(ball);

            var orig = pk.Ball;
            pk.Ball = (byte)ball;
            if (force || BallVerifier.VerifyBall(enc, ball, pk).IsValid)
                return;
            pk.Ball = orig;
        }
        if (matching)
        {
            bool isShiny = pk.IsShiny;
            var color = !isShiny ? PersonalColorUtil.GetColor(pk) : Aesthetics.GetShinyColor(pk.Species);
            BallApplicator.ApplyBallLegalByColor(pk, color); // TODO: use overload with enc on next NuGet release
        }
    }

    /// <summary>
    /// Sets all ribbon flags according to a legality report.
    /// </summary>
    /// <param name="pk">Pokémon to modify</param>
    /// <param name="set"></param>
    /// <param name="enc">Encounter matched to</param>
    /// <param name="allValid">Set all valid ribbons only</param>
    public static void SetSuggestedRibbons(this PKM pk, IBattleTemplate set, IEncounterTemplate enc, bool allValid)
    {
        if (!allValid)
            return;

        RibbonApplicator.SetAllValidRibbons(pk);
        if (pk is PK8 { Species: not (int)Species.Shedinja } pk8 && pk8.TryGetRandomValidMark(set, enc, out var mark))
            pk8.SetRibbonIndex(mark);
        if (pk is PK9 { Species: not (int)Species.Shedinja } pk9 && pk9.TryGetRandomValidMark(set, enc, out var mark9))
            pk9.SetRibbonIndex(mark9);
    }
}
