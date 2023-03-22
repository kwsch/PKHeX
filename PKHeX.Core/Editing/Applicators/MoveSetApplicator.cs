using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for getting valid movesets.
/// </summary>
public static class MoveSetApplicator
{
    public static void SetMoveset(this PKM pk, bool random = false)
    {
        Span<ushort> moves = stackalloc ushort[4];
        pk.GetMoveSet(moves, random);
        pk.SetMoves(moves);
    }

    public static void SetRelearnMoves(this PKM pk, LegalityAnalysis la)
    {
        Span<ushort> moves = stackalloc ushort[4];
        la.GetSuggestedRelearnMoves(moves);
        pk.SetRelearnMoves(moves);
    }

    /// <summary>
    /// Gets a moveset for the provided <see cref="PKM"/> data.
    /// </summary>
    /// <param name="pk">PKM to generate for</param>
    /// <param name="moves">Result storage</param>
    /// <param name="random">Full movepool &amp; shuffling</param>
    /// <returns>4 moves</returns>
    public static void GetMoveSet(this PKM pk, Span<ushort> moves, bool random = false)
    {
        var la = new LegalityAnalysis(pk);
        la.GetMoveSet(moves, random);

        if (random)
            return;

        var clone = pk.Clone();
        clone.SetMoves(moves);
        clone.SetMaximumPPCurrent(moves);
        var newLa = new LegalityAnalysis(clone);

        if (newLa.Valid)
            return;

        // ReSharper disable once TailRecursiveCall
        GetMoveSet(pk, moves, true);
    }

    /// <summary>
    /// Gets a moveset for the provided <see cref="PKM"/> data.
    /// </summary>
    /// <param name="la">Precomputed optional</param>
    /// <param name="moves">Result storage</param>
    /// <param name="random">Full movepool &amp; shuffling</param>
    /// <returns>4 moves</returns>
    public static void GetMoveSet(this LegalityAnalysis la, Span<ushort> moves, bool random = false)
    {
        la.GetSuggestedCurrentMoves(moves, random ? MoveSourceType.All : MoveSourceType.Encounter);
        if (random && !la.Entity.IsEgg)
            Util.Shuffle(moves);
    }

    /// <summary>
    /// Fetches <see cref="PKM.RelearnMoves"/> based on the provided <see cref="LegalityAnalysis"/>.
    /// </summary>
    /// <param name="legal"><see cref="LegalityAnalysis"/> which contains parsed information pertaining to legality.</param>
    /// <param name="moves">Result storage</param>
    /// <param name="enc">Encounter the relearn moves should be suggested for. If not provided, will use the original encounter from the analysis. </param>
    /// <returns><see cref="PKM.RelearnMoves"/> best suited for the current <see cref="PKM"/> data.</returns>
    public static void GetSuggestedRelearnMoves(this LegalityAnalysis legal, Span<ushort> moves, IEncounterTemplate? enc = null)
    {
        enc ??= legal.EncounterOriginal;
        legal.GetSuggestedRelearnMovesFromEncounter(moves, enc);
        if (moves[0] != 0)
            return;

        if (enc is MysteryGift or EncounterEgg)
            return;

        if (enc is EncounterSlot6AO {CanDexNav: true} dn)
        {
            var chk = legal.Info.Moves;
            for (int i = 0; i < chk.Length; i++)
            {
                if (!chk[i].ShouldBeInRelearnMoves())
                    continue;

                var move = legal.Entity.GetMove(i);
                if (!dn.CanBeDexNavMove(move))
                    continue;
                moves.Clear();
                moves[0] = move;
                return;
            }
        }

        if (enc is EncounterSlot8b { IsUnderground: true } ug)
        {
            var chk = legal.Info.Moves;
            for (int i = 0; i < chk.Length; i++)
            {
                if (!chk[i].ShouldBeInRelearnMoves())
                    continue;

                var move = legal.Entity.GetMove(i);
                if (!ug.CanBeUndergroundMove(move))
                    continue;
                moves.Clear();
                moves[0] = move;
                return;
            }

            if (ug.GetBaseEggMove(out var any))
            {
                moves.Clear();
                moves[0] = any;
                return;
            }
        }

        var encounter = EncounterSuggestion.GetSuggestedMetInfo(legal.Entity);
        if (encounter is IRelearn {Relearn: {HasMoves:true} r})
            r.CopyTo(moves);
    }
}
