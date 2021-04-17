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
            if (ShouldNotHaveRelearnMoves(enc, pkm))
                return VerifyRelearnNone(pkm, result);

            return enc switch
            {
                IRelearn s when s.Relearn.Count != 0 => VerifyRelearnSpecifiedMoveset(pkm, s.Relearn, result),
                EncounterEgg e => VerifyEggMoveset(e, result, pkm.RelearnMoves),
                EncounterSlot6AO z when pkm.RelearnMove1 != 0 && z.CanDexNav => VerifyRelearnDexNav(pkm, result),
                _ => VerifyRelearnNone(pkm, result)
            };
        }

        public static bool ShouldNotHaveRelearnMoves(IGeneration enc, PKM pkm) => enc.Generation < 6 || pkm is IBattleVersion {BattleVersion: not 0};

        private static CheckResult[] VerifyRelearnSpecifiedMoveset(PKM pkm, IReadOnlyList<int> required, CheckResult[] result)
        {
            result[3] = CheckResult(pkm.RelearnMove4, required[3]);
            result[2] = CheckResult(pkm.RelearnMove3, required[2]);
            result[1] = CheckResult(pkm.RelearnMove2, required[1]);
            result[0] = CheckResult(pkm.RelearnMove1, required[0]);
            return result;

            static CheckResult CheckResult(int move, int require)
            {
                if (move == require)
                    return DummyValid;
                return new CheckResult(Severity.Invalid, string.Format(LMoveFExpect_0, MoveStrings[require]), CheckIdentifier.RelearnMove);
            }
        }

        private static CheckResult[] VerifyRelearnDexNav(PKM pkm, CheckResult[] result)
        {
            // DexNav Pokémon can have 1 random egg move as a relearn move.
            var baseSpec = EvoBase.GetBaseSpecies(pkm);
            var firstRelearn = pkm.RelearnMove1;
            var eggMoves = MoveEgg.GetEggMoves(6, baseSpec.Species, baseSpec.Form, GameVersion.OR);
            result[0] = Array.IndexOf(eggMoves, firstRelearn) == -1 // not found
                ? new CheckResult(Severity.Invalid, LMoveRelearnDexNav, CheckIdentifier.RelearnMove)
                : DummyValid;

            // All other relearn moves must be empty.
            result[3] = pkm.RelearnMove4 == 0 ? DummyValid : DummyNone;
            result[2] = pkm.RelearnMove3 == 0 ? DummyValid : DummyNone;
            result[1] = pkm.RelearnMove2 == 0 ? DummyValid : DummyNone;

            return result;
        }

        private static CheckResult[] VerifyRelearnNone(PKM pkm, CheckResult[] result)
        {
            // No relearn moves should be present.
            result[3] = pkm.RelearnMove4 == 0 ? DummyValid : DummyNone;
            result[2] = pkm.RelearnMove3 == 0 ? DummyValid : DummyNone;
            result[1] = pkm.RelearnMove2 == 0 ? DummyValid : DummyNone;
            result[0] = pkm.RelearnMove1 == 0 ? DummyValid : DummyNone;
            return result;
        }

        internal static CheckResult[] VerifyEggMoveset(EncounterEgg e, CheckResult[] result, int[] moves, CheckIdentifier type = CheckIdentifier.RelearnMove)
        {
            int gen = e.Generation;
            var origins = MoveBreed.Process(gen, e.Species, e.Form, e.Version, moves, out var valid);
            if (valid)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    var msg = EggSourceUtil.GetSource(origins, gen, i);
                    result[i] = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Valid, msg, type);
                }
            }
            else
            {
                var expected = MoveBreed.GetExpectedMoves(moves, e);
                origins = MoveBreed.Process(gen, e.Species, e.Form, e.Version, expected, out _);
                for (int i = 0; i < moves.Length; i++)
                {
                    var msg = EggSourceUtil.GetSource(origins, gen, i);
                    var expect = expected[i];
                    CheckMoveResult line;
                    if (moves[i] == expect)
                    {
                        line = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Valid, msg, type);
                    }
                    else
                    {
                        msg = string.Format(LMoveRelearnFExpect_0, MoveStrings[expect], msg);
                        line = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Invalid, msg, type);
                    }
                    result[i] = line;
                }
            }

            var dupe = IsAnyMoveDuplicate(moves);
            if (dupe != NO_DUPE)
                result[dupe] = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Invalid, LMoveSourceDuplicate, type);
            return result;
        }

        private const int NO_DUPE = -1;

        private static int IsAnyMoveDuplicate(int[] move)
        {
            int m1 = move[0];
            int m2 = move[1];

            if (m1 != 0 && m1 == m2)
                return 1;
            int m3 = move[2];
            if (m3 != 0 && (m1 == m3 || m2 == m3))
                return 2;
            int m4 = move[3];
            if (m4 != 0 && (m1 == m4 || m2 == m4 || m3 == m4))
                return 3;
            return NO_DUPE;
        }
    }
}
