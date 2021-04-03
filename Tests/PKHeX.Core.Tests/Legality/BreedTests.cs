using FluentAssertions;
using PKHeX.Core;
using Xunit;
using static PKHeX.Core.Move;
using static PKHeX.Core.Species;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Tests.Legality
{
    public class BreedTests
    {
        private static int[] GetMoves(Move[] moves)
        {
            var result = new int[4];
            for (int i = 0; i < moves.Length; i++)
                result[i] = (int) moves[i];
            return result;
        }

        [Theory]
        [InlineData(GD, 5, Bulbasaur, 0, Tackle, Growl)]
        [InlineData(SV, 5, Igglybuff, 0, FeintAttack, Pound, Curse, ZapCannon)]
        [InlineData(C, 5, Igglybuff, 0, FeintAttack, Pound, Flamethrower, Sing)]
        public void VerifyBreed2(GameVersion game, int lvl, Species species, int form, params Move[] movelist)
        {
            var gen = game.GetGeneration();
            var moves = GetMoves(movelist);
            var test = MoveBreed.Process25(gen, (int) species, form, game, moves, lvl);
            test.Should().BeTrue();
        }

        [Theory]
        [InlineData(X, 1, Growlithe, 0, Bite, Roar, FlareBlitz, MorningSun)]
        [InlineData(OR, 1, Growlithe, 0, MorningSun, IronTail, Crunch, HeatWave)]
        [InlineData(OR, 1, Dratini, 0, Wrap, Leer, DragonDance, ExtremeSpeed)]
        [InlineData(OR, 1, Rotom, 0, Astonish, ThunderWave, ThunderShock, ConfuseRay)]
        public void VerifyBreed6(GameVersion game, int lvl, Species species, int form, params Move[] movelist)
        {
            var gen = game.GetGeneration();
            var moves = GetMoves(movelist);
            var test = MoveBreed.Process6(gen, (int)species, form, game, moves, lvl);
            test.Should().BeTrue();
        }

        [Theory]
        [InlineData(C, 5, Igglybuff, 0, Charm, DefenseCurl, Sing, Flamethrower)] // invalid push-out order
        public void CheckBad2(GameVersion game, int lvl, Species species, int form, params Move[] movelist)
        {
            var gen = game.GetGeneration();
            var moves = GetMoves(movelist);
            var test = MoveBreed.Process25(gen, (int)species, form, game, moves, lvl);
            test.Should().BeFalse();
        }

        [Theory]
        [InlineData(SH, 1, Honedge, 0, FuryCutter, WideGuard, DestinyBond)] // insufficient move count
        [InlineData(OR, 1, Rotom, 0, Discharge, Charge, Trick, ConfuseRay)] // invalid push-out order
        [InlineData(OR, 1, Rotom, 0, ThunderWave, ThunderShock, ConfuseRay, Discharge)] // no inheriting levelup
        public void CheckBad6(GameVersion game, int lvl, Species species, int form, params Move[] movelist)
        {
            var gen = game.GetGeneration();
            var moves = GetMoves(movelist);
            var test = MoveBreed.Process6(gen, (int)species, form, game, moves, lvl);
            test.Should().BeFalse();
        }
    }
}
