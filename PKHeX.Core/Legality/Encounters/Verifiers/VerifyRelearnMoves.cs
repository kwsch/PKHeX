using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.ParseSettings;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic to verify the current <see cref="PKM.RelearnMoves"/>.
    /// </summary>
    public static class VerifyRelearnMoves
    {
        public static CheckResult[] VerifyRelearn(PKM pkm, IEncounterable enc)
        {
            if (enc.Generation < 6 || (pkm is IBattleVersion {BattleVersion: not 0}))
                return VerifyRelearnNone(pkm);

            return enc switch
            {
                IRelearn s when s.Relearn.Count > 0 => VerifyRelearnSpecifiedMoveset(pkm, s.Relearn),
                EncounterEgg e => VerifyRelearnEggBase(pkm, e),
                EncounterSlot6AO z when pkm.RelearnMove1 != 0 && z.CanDexNav => VerifyRelearnDexNav(pkm),
                _ => VerifyRelearnNone(pkm)
            };
        }

        public static IReadOnlyList<int> GetSuggestedRelearn(PKM pkm, IEncounterable enc, CheckResult[] relearn)
        {
            if (enc.Generation < 6 || (pkm is IBattleVersion {BattleVersion: not 0}))
                return Array.Empty<int>();

            return enc switch
            {
                IRelearn s when s.Relearn.Count > 0 => s.Relearn,
                EncounterEgg e => MoveList.GetBaseEggMoves(pkm, e.Species, e.Form, e.Version, e.Level),
                EncounterSlot6AO z when pkm.RelearnMove1 != 0 && z.CanDexNav => relearn.All(r => r.Valid) ? pkm.RelearnMoves : Array.Empty<int>(),
                _ => Array.Empty<int>(),
            };
        }

        private static CheckResult[] VerifyRelearnSpecifiedMoveset(PKM pkm, IReadOnlyList<int> required)
        {
            CheckResult[] res = new CheckResult[4];
            int[] relearn = pkm.RelearnMoves;

            for (int i = 0; i < 4; i++)
            {
                res[i] = relearn[i] != required[i]
                    ? new CheckResult(Severity.Invalid, string.Format(LMoveFExpect_0, MoveStrings[required[i]]), CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);
            }

            return res;
        }

        private static CheckResult[] VerifyRelearnDexNav(PKM pkm)
        {
            var result = new CheckResult[4];
            int[] relearn = pkm.RelearnMoves;

            // DexNav Pokémon can have 1 random egg move as a relearn move.
            var baseSpec = EvoBase.GetBaseSpecies(pkm);
            result[0] = !MoveEgg.GetEggMoves(6, baseSpec.Species, baseSpec.Form, GameVersion.OR).Contains(relearn[0])
                ? new CheckResult(Severity.Invalid, LMoveRelearnDexNav, CheckIdentifier.RelearnMove)
                : new CheckResult(CheckIdentifier.RelearnMove);

            // All other relearn moves must be empty.
            for (int i = 1; i < 4; i++)
            {
                result[i] = relearn[i] != 0
                    ? new CheckResult(Severity.Invalid, LMoveRelearnNone, CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);
            }

            return result;
        }

        private static CheckResult[] VerifyRelearnNone(PKM pkm)
        {
            var result = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;

            // No relearn moves should be present.
            for (int i = 0; i < 4; i++)
            {
                result[i] = RelearnMoves[i] != 0
                    ? new CheckResult(Severity.Invalid, LMoveRelearnNone, CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);
            }

            return result;
        }

        private static CheckResult[] VerifyRelearnEggBase(PKM pkm, EncounterEgg e)
        {
            int[] RelearnMoves = pkm.RelearnMoves;
            var result = new CheckResult[4];
            // Level up moves cannot be inherited if Ditto is the parent
            // that means genderless species and male only species except Nidoran and Volbeat (they breed with female nidoran and illumise) could not have level up moves as an egg
            bool inheritLvlMoves = Breeding.GetCanInheritMoves(e.Species);

            // Obtain level1 moves
            var baseMoves = MoveList.GetBaseEggMoves(pkm, e.Species, e.Form, e.Version, 1);
            int baseCt = Math.Min(4, baseMoves.Length);

            // Obtain Inherited moves
            var inheritMoves = MoveList.GetValidRelearn(pkm, e.Species, e.Form, inheritLvlMoves, e.Version).ToList();
            int reqBase = GetRequiredBaseMoves(RelearnMoves, baseMoves, baseCt, inheritMoves);

            // Check if the required amount of Base Egg Moves are present.
            FlagBaseEggMoves(result, reqBase, baseMoves, RelearnMoves);

            // Non-Base moves that can magically appear in the regular movepool
            if (Legal.LightBall.Contains(pkm.Species))
                inheritMoves.Add((int)Move.VoltTackle);

            // If any splitbreed moves are invalid, flag accordingly
            IReadOnlyList<int> splitMoves = e is EncounterEggSplit s
                ? MoveList.GetValidRelearn(pkm, s.OtherSpecies, s.Form, inheritLvlMoves, e.Version).ToList()
                : Array.Empty<int>();

            // Inherited moves appear after the required base moves.
            // If the pkm is capable of split-species breeding and any inherited move is from the other split scenario, flag accordingly.
            bool splitInvalid = FlagInvalidInheritedMoves(result, reqBase, RelearnMoves, inheritMoves, splitMoves);
            if (splitInvalid && e is EncounterEggSplit x)
                FlagSplitbreedMoves(result, reqBase, x);

            var dupe = IsAnyRelearnMoveDuplicate(pkm);
            if (dupe > 0)
                result[dupe] = new CheckResult(Severity.Invalid, LMoveSourceDuplicate, CheckIdentifier.RelearnMove);
            return result;
        }

        private static void FlagBaseEggMoves(CheckResult[] result, int required, IReadOnlyList<int> baseMoves, IReadOnlyList<int> RelearnMoves)
        {
            for (int i = 0; i < required; i++)
            {
                if (!baseMoves.Contains(RelearnMoves[i]))
                {
                    FlagRelearnMovesMissing(result, required, baseMoves, i);
                    return;
                }
                result[i] = new CheckResult(Severity.Valid, LMoveRelearnEgg, CheckIdentifier.RelearnMove);
            }
        }

        private static void FlagRelearnMovesMissing(CheckResult[] result, int required, IReadOnlyList<int> baseMoves, int start)
        {
            for (int z = start; z < required; z++)
                result[z] = new CheckResult(Severity.Invalid, LMoveRelearnEggMissing, CheckIdentifier.RelearnMove);

            // provide the list of suggested base moves for the last required slot
            string em = string.Join(", ", GetMoveNames(baseMoves));
            result[required - 1].Comment += string.Format(Environment.NewLine + LMoveRelearnFExpect_0, em);
        }

        private static bool FlagInvalidInheritedMoves(CheckResult[] result, int required, IReadOnlyList<int> RelearnMoves, IReadOnlyList<int> inheritMoves, IReadOnlyList<int> splitMoves)
        {
            bool splitInvalid = false;
            bool isSplit = splitMoves.Count > 0;
            for (int i = required; i < 4; i++)
            {
                if (RelearnMoves[i] == 0) // empty
                    result[i] = new CheckResult(Severity.Valid, LMoveSourceEmpty, CheckIdentifier.RelearnMove);
                else if (inheritMoves.Contains(RelearnMoves[i])) // inherited
                    result[i] = new CheckResult(Severity.Valid, LMoveSourceRelearn, CheckIdentifier.RelearnMove);
                else if (isSplit && splitMoves.Contains(RelearnMoves[i])) // inherited
                    splitInvalid = true;
                else // not inheritable, flag
                    result[i] = new CheckResult(Severity.Invalid, LMoveRelearnInvalid, CheckIdentifier.RelearnMove);
            }

            return splitInvalid;
        }

        private static void FlagSplitbreedMoves(CheckResult[] res, int required, EncounterEggSplit x)
        {
            var other = x.OtherSpecies;
            for (int i = required; i < 4; i++)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (res[i] != null)
                    continue;

                string message = string.Format(LMoveEggFIncompatible0_1, SpeciesStrings[other], SpeciesStrings[x.Species]);
                res[i] = new CheckResult(Severity.Invalid, message, CheckIdentifier.RelearnMove);
            }
        }

        private static int GetRequiredBaseMoves(int[] RelearnMoves, IReadOnlyList<int> baseMoves, int baseCt, IReadOnlyList<int> inheritMoves)
        {
            var inherited = RelearnMoves.Where(m => m != 0 && (!baseMoves.Contains(m) || inheritMoves.Contains(m))).ToList();
            int inheritCt = inherited.Count;

            // Get required amount of base moves
            int unique = baseMoves.Union(inherited).Count();
            int reqBase = inheritCt == 4 || baseCt + inheritCt > 4 ? 4 - inheritCt : baseCt;
            if (RelearnMoves.Count(m => m != 0) < Math.Min(4, baseMoves.Count))
                reqBase = Math.Min(4, unique);
            return reqBase;
        }

        private static int IsAnyRelearnMoveDuplicate(PKM pk)
        {
            int m1 = pk.RelearnMove1;
            int m2 = pk.RelearnMove2;

            if (m1 != 0 && m1 == m2)
                return 1;
            int m3 = pk.RelearnMove3;
            if (m3 != 0 && (m1 == m3 || m2 == m3))
                return 2;
            int m4 = pk.RelearnMove4;
            if (m4 != 0 && (m1 == m4 || m2 == m4 || m3 == m4))
                return 3;
            return -1;
        }
    }
}
