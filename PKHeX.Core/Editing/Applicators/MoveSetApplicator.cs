using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Logic for getting valid movesets.
/// </summary>
public static class MoveSetApplicator
{
    /// <summary>
    /// Gets a moveset for the provided <see cref="PKM"/> data.
    /// </summary>
    /// <param name="pk">PKM to generate for</param>
    /// <param name="random">Full movepool &amp; shuffling</param>
    /// <returns>4 moves</returns>
    public static ushort[] GetMoveSet(this PKM pk, bool random = false)
    {
        var la = new LegalityAnalysis(pk);
        var moves = la.GetMoveSet(random);

        if (random)
            return moves;

        var clone = pk.Clone();
        clone.SetMoves(moves);
        clone.SetMaximumPPCurrent(moves);
        var newLa = new LegalityAnalysis(clone);

        // ReSharper disable once TailRecursiveCall
        return newLa.Valid ? moves : GetMoveSet(pk, true);
    }

    /// <summary>
    /// Gets a moveset for the provided <see cref="PKM"/> data.
    /// </summary>
    /// <param name="la">Precomputed optional</param>
    /// <param name="random">Full movepool &amp; shuffling</param>
    /// <returns>4 moves</returns>
    public static ushort[] GetMoveSet(this LegalityAnalysis la, bool random = false)
    {
        var m = la.GetSuggestedCurrentMoves(random ? MoveSourceType.All : MoveSourceType.Encounter);
        if (random && !la.Entity.IsEgg)
            Util.Shuffle(m.AsSpan());

        const int count = 4;
        if (m.Length > count)
            return m.AsSpan(m.Length - count).ToArray();
        Array.Resize(ref m, count);
        return m;
    }

    /// <summary>
    /// Fetches <see cref="PKM.RelearnMoves"/> based on the provided <see cref="LegalityAnalysis"/>.
    /// </summary>
    /// <param name="legal"><see cref="LegalityAnalysis"/> which contains parsed information pertaining to legality.</param>
    /// <param name="enc">Encounter the relearn moves should be suggested for. If not provided, will use the original encounter from the analysis. </param>
    /// <returns><see cref="PKM.RelearnMoves"/> best suited for the current <see cref="PKM"/> data.</returns>
    public static IReadOnlyList<ushort> GetSuggestedRelearnMoves(this LegalityAnalysis legal, IEncounterTemplate? enc = null)
    {
        enc ??= legal.EncounterOriginal;
        var m = legal.GetSuggestedRelearnMovesFromEncounter(enc);
        if (m.Any(z => z != 0))
            return m;

        if (enc is MysteryGift or EncounterEgg)
            return m;

        if (enc is EncounterSlot6AO {CanDexNav: true} dn)
        {
            var moves = legal.Info.Moves;
            for (int i = 0; i < moves.Length; i++)
            {
                if (!moves[i].ShouldBeInRelearnMoves())
                    continue;

                var move = legal.Entity.GetMove(i);
                if (dn.CanBeDexNavMove(move))
                    return new ushort[] { move, 0, 0, 0 };
            }
        }

        if (enc is EncounterSlot8b { IsUnderground: true } ug)
        {
            var moves = legal.Info.Moves;
            for (int i = 0; i < moves.Length; i++)
            {
                if (!moves[i].ShouldBeInRelearnMoves())
                    continue;

                var move = legal.Entity.GetMove(i);
                if (ug.CanBeUndergroundMove(move))
                    return new ushort[] { move, 0, 0, 0 };
            }

            if (ug.GetBaseEggMove(out var any))
                return new ushort[] { any, 0, 0, 0 };
        }

        var encounter = EncounterSuggestion.GetSuggestedMetInfo(legal.Entity);
        if (encounter is IRelearn {Relearn: {HasMoves:true} r})
            return r.ToArray();

        return m;
    }
}
