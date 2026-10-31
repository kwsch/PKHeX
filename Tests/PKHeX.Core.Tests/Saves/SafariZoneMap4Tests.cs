using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Saves;

public class SafariZoneMap4Tests
{
    [Fact]
    public void TerrainMatchesSpreadsheet()
    {
        const SafariZoneType4 plains = SafariZoneType4.Plains;
        SafariZoneMap4.IsLand(plains, 12, 2).Should().BeTrue();
        SafariZoneMap4.IsLand(plains, 2, 12).Should().BeTrue();
        SafariZoneMap4.IsLand(plains, 2, 2).Should().BeFalse();
        SafariZoneMap4.IsLand(plains, 30, 29).Should().BeFalse();
        SafariZoneMap4.IsLand(plains, 1, 2).Should().BeFalse();
        SafariZoneMap4.GetHeight(plains, 12, 2).Should().Be(16);

        // Peak is anchored at 4,4 and has a raised area.
        SafariZoneMap4.GetHeight(SafariZoneType4.Peak, 8, 4).Should().Be(48);
        SafariZoneMap4.IsLand(SafariZoneType4.Peak, 3, 4).Should().BeFalse();

        // Water tiles only exist on lake maps and sit lower.
        int water = 0;
        for (int z = 0; z < SafariZoneMap4.WorldSize; z++)
        {
            for (int x = 0; x < SafariZoneMap4.WorldSize; x++)
            {
                if (SafariZoneMap4.IsWater(SafariZoneType4.Meadow, x, z))
                {
                    water++;
                    SafariZoneMap4.GetHeight(SafariZoneType4.Meadow, x, z).Should().Be(8);
                }
                SafariZoneMap4.IsWater(plains, x, z).Should().BeFalse();
            }
        }
        water.Should().Be(30);
    }

    [Fact]
    public void LandTileCountsMatchSpreadsheet()
    {
        Count(SafariZoneType4.Plains).Should().Be(468);
        Count(SafariZoneType4.Mountain).Should().Be(338);
        Count(SafariZoneType4.Wetland).Should().Be(451);
        return;

        static int Count(SafariZoneType4 map)
        {
            int count = 0;
            for (int z = 0; z < SafariZoneMap4.WorldSize; z++)
            {
                for (int x = 0; x < SafariZoneMap4.WorldSize; x++)
                {
                    if (SafariZoneMap4.IsPlaceable(map, x, z))
                        count++;
                }
            }
            return count;
        }
    }

    [Fact]
    public void EvaluateFlagsOverlapAndTerrain()
    {
        var objects = new SafariZoneObject4[4];
        objects[0] = new SafariZoneObject4 { Type = SafariZoneObjectType4.Tree, X = 2, Y = 16, Z = 12 };
        objects[1] = new SafariZoneObject4 { Type = SafariZoneObjectType4.Shrubbery, X = 3, Y = 16, Z = 13 }; // inside the tree
        objects[2] = new SafariZoneObject4 { Type = SafariZoneObjectType4.Shrubbery, X = 2, Y = 16, Z = 14 }; // adjacent, not overlapping
        objects[3] = new SafariZoneObject4 { Type = SafariZoneObjectType4.Tree, X = 29, Y = 16, Z = 20 }; // 2x2 hangs off the map

        var result = new SafariZonePlacement4[objects.Length];
        SafariZoneMap4.Evaluate(SafariZoneType4.Plains, objects, result);

        result[0].Should().Be(SafariZonePlacement4.Overlap);
        result[1].Should().Be(SafariZonePlacement4.Overlap);
        result[2].Should().Be(SafariZonePlacement4.None);
        result[3].Should().Be(SafariZonePlacement4.Unplaceable);
    }

    [Fact]
    public void FountainRequiresWater()
    {
        var fountain = new SafariZoneObject4 { Type = SafariZoneObjectType4.Fountain, X = 12, Y = 16, Z = 12 };
        SafariZoneMap4.CanPlace(SafariZoneType4.Plains, fountain).Should().BeFalse();
    }

    [Fact]
    public void ObjectNamesLoad()
    {
        var names = GameInfo.GetStrings("en").safariblocks;
        names.Length.Should().Be(24);
        names[(int)SafariZoneObjectType4.Tree].Should().Be("Tree");
    }
}
