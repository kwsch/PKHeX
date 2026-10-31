using System;

namespace PKHeX.Core;

/// <summary>
/// Terrain information for the HGSS Safari Zone areas.
/// </summary>
/// <remarks>
/// Objects are placed on the X/Z plane; Y is the height off the ground. An object stores its components in X, Z, Y order.
/// Terrain is stored per map as a <see cref="GridSize"/> x <see cref="GridSize"/> grid whose origin is the map's <see cref="GetAnchor"/>.
/// </remarks>
public static partial class SafariZoneMap4
{
    /// <summary> Width and height of the stored terrain grid. </summary>
    public const int GridSize = 28;

    /// <summary> Width and height of the full world grid (valid object coordinates are 0 to <see cref="WorldSize"/> - 1). </summary>
    public const int WorldSize = 32;

    private const int TileCount = GridSize * GridSize;

    /// <summary> Bytes of one map's placeability bitfield. </summary>
    public const int BitfieldSize = (TileCount + 7) / 8;

    /// <summary> Bytes of one map's height nibble map. </summary>
    public const int HeightmapSize = (TileCount + 1) / 2;

    /// <summary> Heights are stored divided by this value. </summary>
    private const int HeightUnit = 8;

    /// <summary> Gets the world coordinates of the first terrain tile (grid origin) for the map. </summary>
    public static (int X, int Z) GetAnchor(SafariZoneType4 map) => map switch
    {
        SafariZoneType4.Peak or SafariZoneType4.RockyBeach or SafariZoneType4.Mountain => (4, 4),
        _ => (2, 2),
    };

    private static bool TryGetIndex(SafariZoneType4 map, int x, int z, out int index)
    {
        index = -1;
        if ((uint)map > (uint)SafariZoneType4.Desert)
            return false;
        var (ax, az) = GetAnchor(map);
        int gx = x - ax;
        int gz = z - az;
        if ((uint)gx >= GridSize || (uint)gz >= GridSize)
            return false;
        index = (gz * GridSize) + gx;
        return true;
    }

    private static bool GetBit(ReadOnlySpan<byte> data, SafariZoneType4 map, int index)
    {
        var offset = ((int)map * BitfieldSize) + (index >> 3);
        return (data[offset] & (1 << (index & 7))) != 0;
    }

    /// <summary> Indicates if a land object can be placed on the tile. </summary>
    public static bool IsLand(SafariZoneType4 map, int x, int z) => TryGetIndex(map, x, z, out var i) && GetBit(LandData, map, i);

    /// <summary> Indicates if a fountain (water) object can be placed on the tile. </summary>
    public static bool IsWater(SafariZoneType4 map, int x, int z) => TryGetIndex(map, x, z, out var i) && GetBit(WaterData, map, i);

    /// <summary> Indicates if the tile exists in the map's terrain at all. </summary>
    public static bool IsPlaceable(SafariZoneType4 map, int x, int z) => TryGetIndex(map, x, z, out var i) && (GetBit(LandData, map, i) || GetBit(WaterData, map, i));

    /// <summary> Gets the terrain height (Y) of a tile, or -1 if the tile is not part of the map's terrain. </summary>
    public static int GetHeight(SafariZoneType4 map, int x, int z)
    {
        if (!TryGetIndex(map, x, z, out var i) || !(GetBit(LandData, map, i) || GetBit(WaterData, map, i)))
            return -1;
        var offset = ((int)map * HeightmapSize) + (i >> 1);
        var nibble = (HeightData[offset] >> ((i & 1) * 4)) & 0xF;
        return nibble * HeightUnit;
    }

    /// <summary> Gets the footprint of an object type, in tiles. Footprints extend toward +X and +Z from the object's coordinate. </summary>
    public static (int Width, int Depth) GetSize(SafariZoneObjectType4 type) => type switch
    {
        SafariZoneObjectType4.Tree or SafariZoneObjectType4.Stump or SafariZoneObjectType4.Branches => (2, 2),
        SafariZoneObjectType4.BigRock or SafariZoneObjectType4.MossyRock or SafariZoneObjectType4.Puddle => (2, 2),
        SafariZoneObjectType4.Fountain or SafariZoneObjectType4.WaterHole => (2, 2),
        SafariZoneObjectType4.Bench or SafariZoneObjectType4.LongFenceHorizontal => (2, 1),
        SafariZoneObjectType4.LongFenceVertical => (1, 2),
        _ => (1, 1),
    };

    /// <summary> Indicates if the object can only be placed on water tiles. </summary>
    public static bool IsWaterObject(SafariZoneObjectType4 type) => type == SafariZoneObjectType4.Fountain;

    /// <summary> Indicates if every tile under the object's footprint is valid terrain for the object. </summary>
    public static bool CanPlace(SafariZoneType4 map, in SafariZoneObject4 obj)
    {
        var (w, d) = GetSize(obj.Type);
        bool water = IsWaterObject(obj.Type);
        for (int dz = 0; dz < d; dz++)
        {
            for (int dx = 0; dx < w; dx++)
            {
                int x = obj.X + dx;
                int z = obj.Z + dz;
                if (!(water ? IsWater(map, x, z) : IsLand(map, x, z)))
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Evaluates every non-empty object, flagging objects that overlap another object or rest on unplaceable terrain.
    /// </summary>
    /// <param name="map">Map the objects are placed on.</param>
    /// <param name="objects">Objects to evaluate.</param>
    /// <param name="result">Receives one flag set per object; must be at least as long as <paramref name="objects"/>.</param>
    public static void Evaluate(SafariZoneType4 map, ReadOnlySpan<SafariZoneObject4> objects, Span<SafariZonePlacement4> result)
    {
        result = result[..objects.Length];
        result.Clear();
        for (int i = 0; i < objects.Length; i++)
        {
            ref readonly var a = ref objects[i];
            if (a.IsEmpty)
                continue;
            if (!CanPlace(map, a))
                result[i] |= SafariZonePlacement4.Unplaceable;

            var (aw, ad) = GetSize(a.Type);
            for (int j = i + 1; j < objects.Length; j++)
            {
                ref readonly var b = ref objects[j];
                if (b.IsEmpty)
                    continue;
                var (bw, bd) = GetSize(b.Type);
                if (a.X >= b.X + bw || b.X >= a.X + aw || a.Z >= b.Z + bd || b.Z >= a.Z + ad)
                    continue;
                result[i] |= SafariZonePlacement4.Overlap;
                result[j] |= SafariZonePlacement4.Overlap;
            }
        }
    }
}

/// <summary>
/// Placement problems for a Safari Zone object.
/// </summary>
[Flags]
public enum SafariZonePlacement4
{
    None = 0,

    /// <summary> Shares a tile with another object. </summary>
    Overlap = 1,

    /// <summary> Rests on at least one tile that cannot hold the object. </summary>
    Unplaceable = 2,
}
