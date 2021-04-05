using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.ParseSettings;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic to verify the current <see cref="PKM.RelearnMoves"/>.
    /// </summary>
    public static class VerifyRelearnMoves
    {
        internal static readonly CheckResult DummyValid = new(CheckIdentifier.RelearnMove);
        private static readonly CheckResult DummyNone = new(Severity.Invalid, LMoveRelearnNone, CheckIdentifier.RelearnMove);

        public static CheckResult[] VerifyRelearn(PKM pkm, IEncounterable enc, CheckResult[] result)
        {
            if (enc.Generation < 6 || (pkm is IBattleVersion {BattleVersion: not 0}))
                return VerifyRelearnNone(pkm, result);

            return enc switch
            {
                IRelearn s when s.Relearn.Count > 0 => VerifyRelearnSpecifiedMoveset(pkm, s.Relearn, result),
                EncounterEgg e => VerifyEggMoveset(pkm, e, result, pkm.RelearnMoves),
                EncounterSlot6AO z when pkm.RelearnMove1 != 0 && z.CanDexNav => VerifyRelearnDexNav(pkm, result),
                _ => VerifyRelearnNone(pkm, result)
            };
        }

        private static CheckResult[] VerifyRelearnSpecifiedMoveset(PKM pkm, IReadOnlyList<int> required, CheckResult[] result)
        {
            int[] relearn = pkm.RelearnMoves;
            for (int i = 0; i < 4; i++)
            {
                result[i] = relearn[i] == required[i] ? DummyValid
                    : new CheckResult(Severity.Invalid, string.Format(LMoveFExpect_0, MoveStrings[required[i]]), CheckIdentifier.RelearnMove);
            }

            return result;
        }

        private static CheckResult[] VerifyRelearnDexNav(PKM pkm, CheckResult[] result)
        {
            int[] relearn = pkm.RelearnMoves;

            // DexNav Pokémon can have 1 random egg move as a relearn move.
            var baseSpec = EvoBase.GetBaseSpecies(pkm);
            var firstRelearn = relearn[0];
            var eggMoves = MoveEgg.GetEggMoves(6, baseSpec.Species, baseSpec.Form, GameVersion.OR);
            result[0] = Array.IndexOf(eggMoves, firstRelearn) == -1 // not found
                ? new CheckResult(Severity.Invalid, LMoveRelearnDexNav, CheckIdentifier.RelearnMove)
                : DummyValid;

            // All other relearn moves must be empty.
            for (int i = 1; i < 4; i++)
                result[i] = relearn[i] == 0 ? DummyValid : DummyNone;

            return result;
        }

        private static CheckResult[] VerifyRelearnNone(PKM pkm, CheckResult[] result)
        {
            int[] RelearnMoves = pkm.RelearnMoves;

            // No relearn moves should be present.
            for (int i = 0; i < 4; i++)
                result[i] = RelearnMoves[i] == 0 ? DummyValid : DummyNone;

            return result;
        }

        internal static CheckResult[] VerifyEggMoveset(PKM pkm, EncounterEgg e, CheckResult[] result, int[] moves, CheckIdentifier type = CheckIdentifier.RelearnMove)
        {
            var origins = MoveBreed.Process(e.Generation, e.Species, e.Form, e.Version, moves, out var valid);
            if (valid)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    var msg = EggSourceExtensions.GetSource(origins, i);
                    result[i] = new CheckMoveResult(MoveSource.EggMove, e.Generation, Severity.Valid, msg, type);
                }
            }
            else
            {
                var fix = MoveBreed.GetExpectedMoves(moves, e);
                for (int i = 0; i < moves.Length; i++)
                {
                    var msg = EggSourceExtensions.GetSource(origins, i);
                    if (moves[i] == fix[i])
                    {
                        result[i] = new CheckMoveResult(MoveSource.EggMove, e.Generation, Severity.Valid, msg, type);
                        continue;
                    }

                    msg = string.Format(LMoveRelearnFExpect_0, GetMoveName(fix[i]) + $" ({msg})");
                    result[i] = new CheckMoveResult(MoveSource.EggMove, e.Generation, Severity.Invalid, msg, type);
                }
            }

            var dupe = IsAnyRelearnMoveDuplicate(pkm);
            if (dupe > 0)
                result[dupe] = new CheckMoveResult(MoveSource.EggMove, e.Generation, Severity.Invalid, LMoveSourceDuplicate, type);
            return result;
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
