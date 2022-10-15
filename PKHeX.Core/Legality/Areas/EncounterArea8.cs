using System;
using System.Collections.Generic;
using static PKHeX.Core.AreaWeather8;
using static PKHeX.Core.AreaSlotType8;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.SWSH"/> encounter area
/// </summary>
public sealed record EncounterArea8 : EncounterArea
{
    public readonly EncounterSlot8[] Slots;

    protected override IReadOnlyList<EncounterSlot> Raw => Slots;
    /// <summary>
    /// Slots from this area can cross over to another area, resulting in a different met location.
    /// </summary>
    /// <remarks>
    /// Should only be true if it is a Symbol (visible) encounter.
    /// </remarks>
    public readonly bool PermitCrossover;

    public override bool IsMatchLocation(int location)
    {
        if (Location == location)
            return true;

        if (!PermitCrossover)
            return false;

        // Get all other areas that the Location can bleed encounters to
        if (!ConnectingArea8.TryGetValue(Location, out var others))
            return false;

        // Check if any of the other areas are the met location
        return Array.IndexOf(others, (byte)location) != -1;
    }

    public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        var metLocation = pk.Met_Location;
        // wild area gets boosted up to level 60 post-game
        var met = pk.Met_Level;
        bool isBoosted = met == BoostLevel && IsBoostedArea60(Location);
        if (isBoosted)
            return GetBoostedMatches(chain, metLocation);
        return GetUnboostedMatches(chain, met, metLocation);
    }

    private IEnumerable<EncounterSlot8> GetUnboostedMatches(EvoCriteria[] chain, int metLevel, int metLocation)
    {
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                if (!slot.IsLevelWithinRange(metLevel))
                    break;

                if (slot.Form != evo.Form && slot.Species is not (int)Species.Rotom)
                    break;

                if (slot.Weather is Heavy_Fog && IsWildArea8(Location))
                    break;

                if (Location != metLocation && !CanCrossoverTo(Location, metLocation, slot.SlotType))
                    break;

                yield return slot;
                break;
            }
        }
    }

    private IEnumerable<EncounterSlot8> GetBoostedMatches(EvoCriteria[] chain, int metLocation)
    {
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                // Ignore max met level comparison; we already know it is permissible to boost to level 60.
                if (slot.LevelMin > BoostLevel)
                    break; // Can't downlevel, only boost to 60.

                if (slot.Form != evo.Form && slot.Species is not (int)Species.Rotom)
                    break;

                if (Location != metLocation && !CanCrossoverTo(Location, metLocation, slot.SlotType))
                    break;

                yield return slot;
                break;
            }
        }
    }

    private static bool CanCrossoverTo(int fromLocation, int toLocation, AreaSlotType8 type)
    {
        if (!type.CanCrossover())
            return false;
        return true;
    }

    public const int BoostLevel = 60;

    public static bool IsWildArea(int location) => IsWildArea8(location) || IsWildArea8Armor(location) || IsWildArea8Crown(location);
    public static bool IsBoostedArea60(int location) => IsWildArea(location);
    public static bool IsBoostedArea60Fog(int location) => IsWildArea8(location); // IoA doesn't have fog restriction by badges, and all Crown stuff is above 60.

    public static bool IsWildArea8(int location)      => location is >= 122 and <= 154; // Rolling Fields -> Lake of Outrage
    public static bool IsWildArea8Armor(int location) => location is >= 164 and <= 194; // Fields of Honor -> Honeycalm Island
    public static bool IsWildArea8Crown(int location) => location is >= 204 and <= 234 and not 206; // Slippery Slope -> Dyna Tree Hill, skip Freezington

    // Location, and areas that it can feed encounters to.
    public static readonly IReadOnlyDictionary<int, byte[]> ConnectingArea8 = new Dictionary<int, byte[]>
    {
        // Route 3
        // City of Motostoke
        {28, new byte[] {20}},

        // Rolling Fields
        // Dappled Grove, East Lake Axewell, West Lake Axewell
        // Also connects to South Lake Miloch but too much of a stretch
        {122, new byte[] {124, 128, 130}},

        // Dappled Grove
        // Rolling Fields, Watchtower Ruins
        {124, new byte[] {122, 126}},

        // Watchtower Ruins
        // Dappled Grove, West Lake Axewell
        {126, new byte[] {124, 130}},

        // East Lake Axewell
        // Rolling Fields, West Lake Axewell, Axew's Eye, North Lake Miloch
        {128, new byte[] {122, 130, 132, 138}},

        // West Lake Axewell
        // Rolling Fields, Watchtower Ruins, East Lake Axewell, Axew's Eye
        {130, new byte[] {122, 126, 128, 132}},

        // Axew's Eye
        // East Lake Axewell, West Lake Axewell
        {132, new byte[] {128, 130}},

        // South Lake Miloch
        // Giant's Seat, North Lake Miloch
        {134, new byte[] {136, 138}},

        // Giant's Seat
        // South Lake Miloch, North Lake Miloch
        {136, new byte[] {134, 138}},

        // North Lake Miloch
        // East Lake Axewell, South Lake Miloch, Giant's Seat
        // Also connects to Motostoke Riverbank but too much of a stretch
        {138, new byte[] {134, 136}},

        // Motostoke Riverbank
        // Bridge Field
        {140, new byte[] {142}},

        // Bridge Field
        // Motostoke Riverbank, Stony Wilderness
        {142, new byte[] {140, 144}},

        // Stony Wilderness
        // Bridge Field, Dusty Bowl, Giant's Mirror, Giant's Cap
        {144, new byte[] {142, 146, 148, 152}},

        // Dusty Bowl
        // Stony Wilderness, Giant's Mirror, Hammerlocke Hills
        {146, new byte[] {144, 148, 150}},

        // Giant's Mirror
        // Stony Wilderness, Dusty Bowl, Hammerlocke Hills
        {148, new byte[] {144, 146, 148}},

        // Hammerlocke Hills
        // Dusty Bowl, Giant's Mirror, Giant's Cap
        {150, new byte[] {146, 148, 152}},

        // Giant's Cap
        // Stony Wilderness, Giant's Cap
        // Also connects to Lake of Outrage but too much of a stretch
        {152, new byte[] {144, 150}},

        // Lake of Outrage is just itself.

        // Challenge Beach
        // Soothing Wetlands, Courageous Cavern
        {170, new byte[] {166, 176}},

        // Challenge Road
        // Brawler's Cave
        {174, new byte[] {172}},

        // Courageous Cavern
        // Loop Lagoon
        {176, new byte[] {178}},

        // Warm-Up Tunnel
        // Training Lowlands, Potbottom Desert
        {182, new byte[] {180, 184}},

        // Workout Sea
        // Fields of Honor
        {186, new byte[] {164}},

        // Stepping-Stone Sea
        // Fields of Honor
        {188, new byte[] {170}},

        // Insular Sea
        // Honeycalm Sea
        {190, new byte[] {192}},

        // Honeycalm Sea
        // Honeycalm Island
        {192, new byte[] {194}},

        // Frostpoint Field
        // Freezington
        {208, new byte[] {206}},

        // Old Cemetery
        // Giant’s Bed
        {212, new byte[] {210}},

        // Roaring-Sea Caves
        // Giant’s Foot
        {224, new byte[] {222}},

        // Ballimere Lake
        // Lakeside Cave
        {230, new byte[] {232}},
    };

    /// <summary>
    /// Location IDs matched with possible weather types. Unlisted locations may only have Normal weather.
    /// </summary>
    internal static readonly Dictionary<int, AreaWeather8> WeatherbyArea = new()
    {
        { 68, Intense_Sun }, // Route 6
        { 88, Snowing }, // Route 8 (Steamdrift Way)
        { 90, Snowing }, // Route 9
        { 92, Snowing }, // Route 9 (Circhester Bay)
        { 94, Overcast }, // Route 9 (Outer Spikemuth)
        { 106, Snowstorm }, // Route 10
        { 122, All }, // Rolling Fields
        { 124, All }, // Dappled Grove
        { 126, All }, // Watchtower Ruins
        { 128, All }, // East Lake Axewell
        { 130, All }, // West Lake Axewell
        { 132, All }, // Axew's Eye
        { 134, All }, // South Lake Miloch
        { 136, All }, // Giant's Seat
        { 138, All }, // North Lake Miloch
        { 140, All }, // Motostoke Riverbank
        { 142, All }, // Bridge Field
        { 144, All }, // Stony Wilderness
        { 146, All }, // Dusty Bowl
        { 148, All }, // Giant's Mirror
        { 150, All }, // Hammerlocke Hills
        { 152, All }, // Giant's Cap
        { 154, All }, // Lake of Outrage
        { 164, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Fields of Honor
        { 166, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Soothing Wetlands
        { 168, All_IoA }, // Forest of Focus
        { 170, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Challenge Beach
        { 174, All_IoA }, // Challenge Road
        { 178, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Loop Lagoon
        { 180, All_IoA }, // Training Lowlands
        { 184, Normal | Overcast | Raining | Sandstorm | Intense_Sun | Heavy_Fog }, // Potbottom Desert
        { 186, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Workout Sea
        { 188, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Stepping-Stone Sea
        { 190, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Insular Sea
        { 192, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Honeycalm Sea
        { 194, Normal | Overcast | Stormy | Intense_Sun | Heavy_Fog }, // Honeycalm Island
        { 204, Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Slippery Slope
        { 208, Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Frostpoint Field
        { 210, All_CT }, // Giant's Bed
        { 212, All_CT }, // Old Cemetery
        { 214, Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Snowslide Slope
        { 216, Overcast }, // Tunnel to the Top
        { 218, Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Path to the Peak
        { 222, All_CT }, // Giant's Foot
        { 224, Overcast }, // Roaring-Sea Caves
        { 226, No_Sun_Sand }, // Frigid Sea
        { 228, All_CT }, // Three-Point Pass
        { 230, All_Ballimere }, // Ballimere Lake
        { 232, Overcast }, // Lakeside Cave
    };

    /// <summary>
    /// Weather types that may bleed into each location from adjacent locations for standard symbol encounter slots.
    /// </summary>
    internal static readonly Dictionary<int, AreaWeather8> WeatherBleedSymbol = new()
    {
        { 166, All_IoA }, // Soothing Wetlands from Forest of Focus
        { 170, All_IoA }, // Challenge Beach from Forest of Focus
        { 182, All_IoA }, // Warm-Up Tunnel from Training Lowlands
        { 208, All_CT }, // Frostpoint Field from Giant's Bed
        { 216, Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Tunnel to the Top from Path to the Peak
        { 224, All_CT }, // Roaring-Sea Caves from Three-Point Pass
        { 232, All_Ballimere }, // Lakeside Cave from Ballimere Lake
        { 230, All_CT }, // Ballimere Lake from Giant's Bed
    };

    /// <summary>
    /// Weather types that may bleed into each location from adjacent locations for surfing symbol encounter slots.
    /// </summary>
    private static readonly Dictionary<int, AreaWeather8> WeatherBleedSymbolSurfing = new()
    {
        { 192, All_IoA }, // Honeycalm Sea from Training Lowlands
        { 224, All_CT }, // Roaring-Sea Caves from Giant's Foot
    };

    /// <summary>
    /// Weather types that may bleed into each location from adjacent locations for Sharpedo symbol encounter slots.
    /// </summary>
    private static readonly Dictionary<int, AreaWeather8> WeatherBleedSymbolSharpedo = new()
    {
        { 192, All_IoA }, // Honeycalm Sea from Training Lowlands
    };

    /// <summary>
    /// Weather types that may bleed into each location from adjacent locations, for standard hidden grass encounter slots.
    /// </summary>
    private static readonly Dictionary<int, AreaWeather8> WeatherBleedHiddenGrass = new()
    {
        { 166, All_IoA }, // Soothing Wetlands from Forest of Focus
        { 170, All_IoA }, // Challenge Beach from Forest of Focus
        { 208, All_CT }, // Frostpoint Field from Giant's Bed
        { 230, All_CT }, // Ballimere Lake from Giant's Bed
    };

    public static bool IsCrossoverBleedPossible(AreaSlotType8 type, int fromLocation, int toLocation) => true;

    public static bool IsWeatherBleedPossible(AreaSlotType8 type, AreaWeather8 permit, int location) => type switch
    {
        SymbolMain or SymbolMain2 or SymbolMain3 => WeatherBleedSymbol        .TryGetValue(location, out var weather) && weather.HasFlag(permit),
        HiddenMain or HiddenMain2                => WeatherBleedHiddenGrass   .TryGetValue(location, out var weather) && weather.HasFlag(permit),
        Surfing                                  => WeatherBleedSymbolSurfing .TryGetValue(location, out var weather) && weather.HasFlag(permit),
        Sharpedo                                 => WeatherBleedSymbolSharpedo.TryGetValue(location, out var weather) && weather.HasFlag(permit),
        _ => false,
    };

    public static EncounterArea8[] GetAreas(BinLinkerAccessor input, GameVersion game, bool symbol = false)
    {
        var result = new EncounterArea8[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea8(input[i], symbol, game);
        return result;
    }

    private EncounterArea8(ReadOnlySpan<byte> areaData, bool symbol, GameVersion game) : base(game)
    {
        PermitCrossover = symbol;
        Location = areaData[0];
        Slots = ReadSlots(areaData, areaData[1]);
    }

    private EncounterSlot8[] ReadSlots(ReadOnlySpan<byte> areaData, byte slotCount)
    {
        var slots = new EncounterSlot8[slotCount];

        int ctr = 0;
        int ofs = 2;
        do
        {
            // Read area metadata
            var meta = areaData.Slice(ofs, 6);
            var flags = (AreaWeather8) ReadUInt16LittleEndian(meta);
            var min = meta[2];
            var max = meta[3];
            var count = meta[4];
            var slotType = (AreaSlotType8)meta[5];
            ofs += 6;

            // Read slots
            const int bpe = 2;
            for (int i = 0; i < count; i++, ctr++, ofs += bpe)
            {
                var entry = areaData.Slice(ofs, bpe);
                var species = ReadUInt16LittleEndian(entry);
                byte form = (byte)(species >> 11);
                species &= 0x3FF;
                slots[ctr] = new EncounterSlot8(this, species, form, min, max, flags, slotType);
            }
        } while (ctr != slots.Length);

        return slots;
    }
}

/// <summary>
/// Encounter Conditions for <see cref="GameVersion.SWSH"/>
/// </summary>
/// <remarks>Values above <see cref="AreaWeather8.All"/> are for Shaking/Fishing hidden encounters only.</remarks>
[Flags]
public enum AreaWeather8 : ushort
{
    None,
    Normal = 1,
    Overcast = 1 << 1,
    Raining = 1 << 2,
    Thunderstorm = 1 << 3,
    Intense_Sun = 1 << 4,
    Snowing = 1 << 5,
    Snowstorm = 1 << 6,
    Sandstorm = 1 << 7,
    Heavy_Fog = 1 << 8,

    All = Normal | Overcast | Raining | Thunderstorm | Intense_Sun | Snowing | Snowstorm | Sandstorm | Heavy_Fog,
    Stormy = Raining | Thunderstorm,
    Icy = Snowing | Snowstorm,
    All_IoA = Normal | Overcast | Stormy | Intense_Sun | Sandstorm | Heavy_Fog,         // IoA can have everything but snow
    All_CT = Normal | Overcast | Stormy | Intense_Sun | Icy | Heavy_Fog,                // CT can have everything but sand
    No_Sun_Sand = Normal | Overcast | Stormy | Icy | Heavy_Fog,                         // Everything but sand and sun
    All_Ballimere = Normal | Overcast | Stormy | Intense_Sun | Snowing | Heavy_Fog,     // All Ballimere Lake weather

    Shaking_Trees = 1 << 9,
    Fishing = 1 << 10,

    NotWeather = Shaking_Trees | Fishing,
}

public static class AreaWeather8Extensions
{
    public static bool IsMarkCompatible(this AreaWeather8 weather, IRibbonSetMark8 m)
    {
        if (m.RibbonMarkCloudy) return (weather & Overcast) != 0;
        if (m.RibbonMarkRainy) return (weather & Raining) != 0;
        if (m.RibbonMarkStormy) return (weather & Thunderstorm) != 0;
        if (m.RibbonMarkSnowy) return (weather & Snowing) != 0;
        if (m.RibbonMarkBlizzard) return (weather & Snowstorm) != 0;
        if (m.RibbonMarkDry) return (weather & Intense_Sun) != 0;
        if (m.RibbonMarkSandstorm) return (weather & Sandstorm) != 0;
        if (m.RibbonMarkMisty) return (weather & Heavy_Fog) != 0;
        return true; // no mark / etc is fine; check later.
    }
}

/// <summary>
/// Encounter Slot Types for <see cref="GameVersion.SWSH"/>
/// </summary>
public enum AreaSlotType8 : byte
{
    SymbolMain,
    SymbolMain2,
    SymbolMain3,

    HiddenMain, // Both HiddenMain tables include the tree/fishing slots for the area.
    HiddenMain2,

    Surfing,
    Surfing2,
    Sky,
    Sky2,
    Ground,
    Ground2,
    Sharpedo,

    OnlyFishing, // More restricted hidden table that ignores the weather slots like grass Tentacool.
    Inaccessible, // Shouldn't show up since these tables are not dumped.
}

public static class AreaSlotType8Extensions
{
    public static bool CanCrossover(this AreaSlotType8 type) => type is not (HiddenMain or HiddenMain2 or OnlyFishing);
    public static bool CanEncounterViaFishing(this AreaSlotType8 type, AreaWeather8 weather) => type is OnlyFishing || weather.HasFlag(Fishing);
    public static bool CanEncounterViaCurry(this AreaSlotType8 type) => type is HiddenMain or HiddenMain2;
}
