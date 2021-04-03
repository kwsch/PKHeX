using System;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Legality
{
    public class BreedTests
    {
        [Theory]
        [InlineData(2, GameVersion.GD, 5, Species.Bulbasaur, 0, (int)Move.Tackle, (int)Move.Growl)]
        [InlineData(2, GameVersion.C, 5, Species.Igglybuff, 0, (int)Move.FeintAttack, (int)Move.Pound, (int)Move.Curse, (int)Move.ZapCannon)]
        [InlineData(2, GameVersion.C, 5, Species.Igglybuff, 0, (int)Move.FeintAttack, (int)Move.Pound, (int)Move.Flamethrower, (int)Move.Sing)]
        public void VerifyBreed2(int gen, GameVersion game, int lvl, Species species, int form, params int[] moves)
        {
            Array.Resize(ref moves, 4);
            var test = MoveBreed.Process25(gen, (int) species, form, game, moves, lvl);
            test.Should().BeTrue();
        }

        [Theory]
        [InlineData(6, GameVersion.X, 1, Species.Growlithe, 0, (int)Move.Bite, (int)Move.Roar, (int)Move.FlareBlitz, (int)Move.MorningSun)]
        public void VerifyBreed6(int gen, GameVersion game, int lvl, Species species, int form, params int[] moves)
        {
            Array.Resize(ref moves, 4);
            var test = MoveBreed.Process6(gen, (int)species, form, game, moves, lvl);
            test.Should().BeTrue();
        }

        [Theory]
        [InlineData(8, GameVersion.SH, 1, Species.Honedge, 0, (int)Move.FuryCutter, (int)Move.WideGuard, (int)Move.DestinyBond)]
        public void CheckBad6(int gen, GameVersion game, int lvl, Species species, int form, params int[] moves)
        {
            Array.Resize(ref moves, 4);
            var test = MoveBreed.Process6(gen, (int)species, form, game, moves, lvl);
            test.Should().BeFalse();
        }
    }
}
