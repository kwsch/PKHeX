using System.Linq;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.Species;
using static PKHeX.Core.Move;

namespace PKHeX.Core.Tests.Legality;

public static class TempTests
{
    [Theory]
    [InlineData(Fuecoco, Ember)]
    [InlineData(Sprigatito, Leafage)]
    [InlineData(Quaxly, WaterGun)]
    [InlineData(Tadbulb, ThunderShock)]
    [InlineData(Annihilape, RageFist)]
    [InlineData(Nacli, RockThrow)]
    [InlineData(Frigibax, IcyWind)]
    public static void CanLearnEggMoveBDSP(Species species, Move move)
    {
        MoveEgg.GetEggMoves(9, (ushort)species, 0, GameVersion.SV).Contains((ushort)move).Should().BeFalse();

        var pk9 = new PK9 { Species = (ushort)species };
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk9, new[] { (ushort)move }, GameVersion.SV);

        encs.Any().Should().BeFalse("Unavailable until HOME update supports S/V.");
    }
}
