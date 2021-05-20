using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
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
        [InlineData(GD, Bulbasaur, 0, Tackle, Growl)]
        [InlineData(SV, Igglybuff, 0, FeintAttack, Pound, Curse, ZapCannon)]
        [InlineData( C, Igglybuff, 0, FeintAttack, Pound, Flamethrower, Sing)]
        [InlineData( B, Heracross, 0, Megahorn, NightSlash, CloseCombat, StoneEdge)]
        [InlineData( B, Heracross, 0, Bide, Megahorn, Counter, Reversal)]
        [InlineData( B, Heracross, 0, HornAttack, Endure, Megahorn, TakeDown)]
        [InlineData( B, Heracross, 0, Endure, Megahorn, FocusPunch, Feint)]
        [InlineData( B, Heracross, 0, Megahorn, Reversal, Bulldoze, Fling)]
        [InlineData( X, Growlithe, 0, Bite, Roar, FlareBlitz, MorningSun)]
        [InlineData(OR, Growlithe, 0, MorningSun, IronTail, Crunch, HeatWave)]
        [InlineData(OR, Dratini, 0, Wrap, Leer, DragonDance, ExtremeSpeed)]
        [InlineData(OR, Rotom, 0, Astonish, ThunderWave, ThunderShock, ConfuseRay)]
        public void VerifyBreed(GameVersion game, Species species, int form, params Move[] movelist)
        {
            var gen = game.GetGeneration();
            var moves = GetMoves(movelist);
            var test = MoveBreed.Process(gen, (int) species, form, game, moves, out var valid);
            valid.Should().BeTrue();

            var x = ((byte[])test);

            if (gen != 2)
                x.SequenceEqual(x.OrderBy(z => z)).Should().BeTrue();
            else
                x.SequenceEqual(x.OrderBy(z => z != (byte)EggSource2.Base)).Should().BeTrue();
        }

        [Theory]
        [InlineData(C, Igglybuff, 0, Charm, DefenseCurl, Sing, Flamethrower)] // invalid push-out order
        [InlineData(SH, Honedge, 0, FuryCutter, WideGuard, DestinyBond)] // insufficient move count
        [InlineData(OR, Rotom, 0, Discharge, Charge, Trick, ConfuseRay)] // invalid push-out order
        [InlineData(OR, Rotom, 0, ThunderWave, ThunderShock, ConfuseRay, Discharge)] // no inheriting levelup
        public void CheckBad(GameVersion game, Species species, int form, params Move[] movelist)
        {
            var gen = game.GetGeneration();
            var moves = GetMoves(movelist);
            var test = MoveBreed.Process(gen, (int)species, form, game, moves);
            test.Should().BeFalse();
        }

        [Theory]
        [InlineData(GD, Bulbasaur, 0, Growl, Tackle)] // swap order, two base moves
        [InlineData(UM, Charmander, 0, Ember, BellyDrum, Scratch, Growl)] // swap order, inherit + egg moves
        public void CheckFix(GameVersion game, Species species, int form, params Move[] movelist)
        {
            var gen = game.GetGeneration();
            var moves = GetMoves(movelist);

            var test = MoveBreed.Process(gen, (int)species, form, game, moves, out var valid);
            valid.Should().BeFalse();
            var reorder = MoveBreed.GetExpectedMoves(gen, (int)species, form, game, moves, test);

            // fixed order should be different now.
            reorder.SequenceEqual(moves).Should().BeFalse();
            // nonzero move count should be same
            reorder.Count(z => z != 0).Should().IsSameOrEqualTo(moves.Count(z => z != 0));
        }
    }
}
