using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality
{
    public static class LegalityBDSP
    {
        private static readonly int[] LocationsNoHatch =
        {
            020, 022, 023,                // Jubilife City
            035,                          // Canalave City
            094, 103, 107,                // Hearthome City
            154, 155, 158,                // Sunyshore City
            181, 182, 183,                // Pokémon League
            218,                          // Hall of Origin (Diamond)
            285,                          // Flower Paradise
            323, 329,                     // Lake Verity (start), Lake Acuity
            332, 333,                     // Newmoon Island
            337, 338,                     // Battle Park
            339, 340, 341, 342, 343, 344, // Battle Tower
            345, 353, 421,                // Mystery Zone
            474,                          // Resort Area
            483, 484,                     // Mystery Zone
            490,                          // Seabreak Path
            491, 492, 493,                // Mystery Zone
            495,                          // Ramanas Park
            618,                          // Hall of Origin (Pearl)
            620, 621, 622, 623,           // Grand Underground (Secret Base)
            625,                          // Sea (sailing animation)
            627, 628, 629, 630, 631, 632, // Grand Underground (Secret Base)
            633, 634, 635, 636, 637, 638, // Grand Underground (Secret Base)
            639, 640, 641, 642, 643, 644, // Grand Underground (Secret Base)
            645, 646, 647,                // Grand Underground (Secret Base)
        };

        [Fact]
        public static void NoHatchLocations()
        {
            const int maxLegal = 658;
            for (int i = 0; i < maxLegal; i++)
            {
                bool banned = LocationsNoHatch.Contains(i);
                var isPermitted = Legal.ValidMet_BDSP.Contains(i);
                isPermitted.Should().Be(!banned);
            }
        }
    }
}
