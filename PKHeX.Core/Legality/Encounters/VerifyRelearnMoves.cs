using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.LegalityAnalysis;

namespace PKHeX.Core
{
    public static class VerifyRelearnMoves
    {
        public static CheckResult[] verifyRelearn(PKM pkm, LegalInfo info)
        {
            if (pkm.GenNumber < 6 || pkm.VC1)
                return verifyRelearnNone(pkm, info);

            if (info.EncounterMatch is EncounterLink l)
                return verifyRelearnSpecifiedMoveset(pkm, info, l.RelearnMoves);
            if (info.EncounterMatch is MysteryGift g)
                return verifyRelearnSpecifiedMoveset(pkm, info, g.RelearnMoves);
            if (info.EncounterMatch is EncounterStatic s)
                return verifyRelearnSpecifiedMoveset(pkm, info, s.Relearn);

            if (info.EncounterMatch is EncounterEgg e)
                return verifyRelearnEggBase(pkm, info, e);

            if (pkm.RelearnMove1 != 0 && info.EncounterMatch is EncounterSlot z && z.DexNav && EncounterGenerator.getDexNavValid(pkm))
                return verifyRelearnDexNav(pkm, info);

            return verifyRelearnNone(pkm, info);
        }

        private static CheckResult[] verifyRelearnSpecifiedMoveset(PKM pkm, LegalInfo info, int[] moves)
        {
            CheckResult[] res = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;
            // Get gifts that match

            for (int i = 0; i < 4; i++)
                res[i] = moves[i] != RelearnMoves[i]
                    ? new CheckResult(Severity.Invalid, string.Format(V178, movelist[moves[i]]), CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);

            info.RelearnBase = moves;
            return res;
        }
        private static CheckResult[] verifyRelearnDexNav(PKM pkm, LegalInfo info)
        {
            CheckResult[] res = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;

            // DexNav Pokémon can have 1 random egg move as a relearn move.
            res[0] = !Legal.getValidRelearn(pkm, 0).Contains(RelearnMoves[0])
                ? new CheckResult(Severity.Invalid, V183, CheckIdentifier.RelearnMove)
                : new CheckResult(CheckIdentifier.RelearnMove);

            // All other relearn moves must be empty.
            for (int i = 1; i < 4; i++)
                res[i] = RelearnMoves[i] != 0
                    ? new CheckResult(Severity.Invalid, V184, CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);

            // Update the relearn base moves if the first relearn move is okay.
            info.RelearnBase = res[0].Valid
                ? RelearnMoves
                : new int[4];

            return res;
        }
        private static CheckResult[] verifyRelearnNone(PKM pkm, LegalInfo info)
        {
            CheckResult[] res = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;
            info.RelearnBase = new int[4];

            // No relearn moves should be present.
            for (int i = 0; i < 4; i++)
                res[i] = RelearnMoves[i] != 0
                    ? new CheckResult(Severity.Invalid, V184, CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);

            return res;
        }
        private static CheckResult[] verifyRelearnEggBase(PKM pkm, LegalInfo info, EncounterEgg e)
        {
            int[] RelearnMoves = pkm.RelearnMoves;
            info.RelearnBase = new int[4];
            CheckResult[] res = new CheckResult[4];

            // Obtain level1 moves
            List<int> baseMoves = new List<int>(Legal.getBaseEggMoves(pkm, e.Species, e.Game, 1));
            int baseCt = baseMoves.Count;
            if (baseCt > 4) baseCt = 4;

            // Obtain Inherited moves
            var inheritMoves = Legal.getValidRelearn(pkm, e.Species).ToList();
            var inherited = RelearnMoves.Where(m => m != 0 && (!baseMoves.Contains(m) || inheritMoves.Contains(m))).ToList();
            int inheritCt = inherited.Count;


            // Get required amount of base moves
            int unique = baseMoves.Concat(inherited).Distinct().Count();
            int reqBase = inheritCt == 4 || baseCt + inheritCt > 4 ? 4 - inheritCt : baseCt;
            if (RelearnMoves.Where(m => m != 0).Count() < Math.Min(4, baseMoves.Count))
                reqBase = Math.Min(4, unique);

            // Check if the required amount of Base Egg Moves are present.
            for (int i = 0; i < reqBase; i++)
            {
                if (baseMoves.Contains(RelearnMoves[i]))
                    res[i] = new CheckResult(Severity.Valid, V179, CheckIdentifier.RelearnMove);
                else
                {
                    // mark remaining base egg moves missing
                    for (int z = i; z < reqBase; z++)
                        res[z] = new CheckResult(Severity.Invalid, V180, CheckIdentifier.RelearnMove);

                    // provide the list of suggested base moves for the last required slot
                    string em = string.Join(", ", baseMoves.Select(m => m >= movelist.Length ? V190 : movelist[m]));
                    res[reqBase - 1].Comment += string.Format(Environment.NewLine + V181, em);
                    break;
                }
            }

            // Non-Base moves that can magically appear in the regular movepool
            if (Legal.LightBall.Contains(pkm.Species))
                inheritMoves.Add(344);

            // Begin verification of moves

            // If any splitbreed moves are invalid, flag accordingly
            var splitInvalid = false;
            var splitMoves = e.SplitBreed ? Legal.getValidRelearn(pkm, Legal.getBaseEggSpecies(pkm)).ToList() : new List<int>();

            // Inherited moves appear after the required base moves.
            for (int i = reqBase; i < 4; i++)
            {
                if (RelearnMoves[i] == 0) // empty
                    res[i] = new CheckResult(Severity.Valid, V167, CheckIdentifier.RelearnMove);
                else if (inheritMoves.Contains(RelearnMoves[i])) // inherited
                    res[i] = new CheckResult(Severity.Valid, V172, CheckIdentifier.RelearnMove);
                else if (e.SplitBreed && splitMoves.Contains(RelearnMoves[i])) // inherited
                    splitInvalid = true;
                else // not inheritable, flag
                    res[i] = new CheckResult(Severity.Invalid, V182, CheckIdentifier.RelearnMove);
            }

            if (splitInvalid)
            {
                var splitSpecies = Legal.getBaseEggSpecies(pkm);
                for (int i = reqBase; i < 4; i++)
                {
                    if (inheritMoves.Contains(RelearnMoves[i]) && !splitMoves.Contains(RelearnMoves[i]))
                        res[i] = new CheckResult(Severity.Invalid, string.Format(V379, specieslist[splitSpecies], specieslist[e.Species]), CheckIdentifier.RelearnMove);
                    if (!inheritMoves.Contains(RelearnMoves[i]) && splitMoves.Contains(RelearnMoves[i]))
                        res[i] = new CheckResult(Severity.Invalid, string.Format(V379, specieslist[e.Species], specieslist[splitSpecies]), CheckIdentifier.RelearnMove);
                }
            }

            info.RelearnBase = baseMoves.ToArray();
            return res;
        }
    }
}
