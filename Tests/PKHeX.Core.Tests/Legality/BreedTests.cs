using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Legality
{
    public class BreedTests
    {
        [Theory]
        [InlineData(2, GameVersion.GD, 5, Species.Bulbasaur, 0, (int)Move.Tackle, (int)Move.Growl)]
        public void VerifyBreed2(int gen, GameVersion game, int lvl, Species species, int form, params int[] moves)
        {
            var test = MoveBreed.Process25(gen, (int) species, form, game, moves, lvl);
            test.Should().BeTrue();
        }

        [Theory]
        [InlineData(8, GameVersion.SH, 1, Species.Honedge, 0, (int)Move.FuryCutter, (int)Move.WideGuard, (int)Move.DestinyBond)]
        public void CheckBad6(int gen, GameVersion game, int lvl, Species species, int form, params int[] moves)
        {
            var test = MoveBreed.Process6(gen, (int)species, form, game, moves, lvl);
            test.Should().BeFalse();
        }
    }
}
