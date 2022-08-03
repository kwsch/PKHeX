using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public sealed partial class MemoryContext6 : MemoryContext
{
    private const int MAX_MEMORY_ID_XY = 64;
    private const int MAX_MEMORY_ID_AO = 69;

    private static ICollection<int> GetPokeCenterLocations(GameVersion game)
    {
        return GameVersion.XY.Contains(game) ? LocationsWithPokeCenter_XY : LocationsWithPokeCenter_AO;
    }

    public static bool GetHasPokeCenterLocation(GameVersion game, int loc)
    {
        if (game == GameVersion.Any)
            return GetHasPokeCenterLocation(GameVersion.X, loc) || GetHasPokeCenterLocation(GameVersion.AS, loc);
        return GetPokeCenterLocations(game).Contains(loc);
    }

    public static int GetMemoryRarity(int memory) => (uint)memory >= MemoryRandChance.Length ? -1 : MemoryRandChance[memory];

    public override IEnumerable<ushort> GetKeyItemParams() => KeyItemUsableObserve6.Concat(KeyItemMemoryArgsGen6.Values.SelectMany(z => z)).Distinct();

    public override bool CanUseItemGeneric(int item)
    {
        // Key Item usage while in party on another species.
        if (KeyItemUsableObserve6.Contains((ushort)item))
            return true;
        if (KeyItemMemoryArgsGen6.Values.Any(z => z.Contains((ushort)item)))
            return true;

        return true; // todo
    }

    public override IEnumerable<ushort> GetMemoryItemParams() => Legal.HeldItem_AO.Distinct()
        .Concat(GetKeyItemParams())
        .Concat(Legal.Pouch_TMHM_AO.Take(100))
        .Where(z => z <= Legal.MaxItemID_6_AO);

    public override bool IsUsedKeyItemUnspecific(int item) => KeyItemUsableObserve6.Contains((ushort)item);
    public override bool IsUsedKeyItemSpecific(int item, int species) => KeyItemMemoryArgsGen6.TryGetValue(species, out var value) && value.Contains((ushort)item);

    public override bool CanPlantBerry(int item) => Legal.Pouch_Berry_XY.Contains((ushort)item);
    public override bool CanHoldItem(int item) => Legal.HeldItem_AO.Contains((ushort)item);

    public override bool CanObtainMemoryOT(GameVersion pkmVersion, byte memory) => pkmVersion switch
    {
        GameVersion.X or GameVersion.Y => CanObtainMemoryXY(memory),
        GameVersion.OR or GameVersion.AS => CanObtainMemoryAO(memory),
        _ => false,
    };

    public override bool CanObtainMemory(byte memory) => memory <= MAX_MEMORY_ID_AO;
    public override bool HasPokeCenter(GameVersion version, int location) => GetHasPokeCenterLocation(version, location);

    public override bool IsInvalidGeneralLocationMemoryValue(byte memory, ushort variable, IEncounterTemplate enc, PKM pk)
    {
        return false; // todo
    }

    public override bool IsInvalidMiscMemory(byte memory, ushort variable)
    {
        return false; // todo
    }

    private static bool CanObtainMemoryAO(int memory) => memory <= MAX_MEMORY_ID_AO && !Memory_NotAO.Contains(memory);
    private static bool CanObtainMemoryXY(int memory) => memory <= MAX_MEMORY_ID_XY && !Memory_NotXY.Contains(memory);
    public override bool CanObtainMemoryHT(GameVersion pkmVersion, byte memory) => CanObtainMemory(memory);

    public override bool CanWinLotoID(int item) => LotoPrizeXYAO.Contains((ushort)item);

    public override bool CanBuyItem(int item, GameVersion version)
    {
        if (version is GameVersion.Any)
            return PurchaseableItemAO.Contains((ushort)item) || PurchaseableItemXY.Contains((ushort)item);
        if (version is GameVersion.X or GameVersion.Y)
            return PurchaseableItemXY.Contains((ushort)item);
        if (version is GameVersion.AS or GameVersion.OR)
            return PurchaseableItemAO.Contains((ushort)item);
        return false;
    }

    public static bool CanHaveFeeling6(int memory, int feeling, int argument)
    {
        if (memory >= MemoryFeelings.Length)
            return false;
        if (memory == 4 && argument == 0) // Bank "came through Link Trade" @ "somewhere" is rand(0,10); doesn't use the bit-table to pick a feeling.
            return (uint)feeling < 10;
        return (MemoryFeelings[memory] & (1 << feeling)) != 0;
    }

    public static bool CanHaveIntensity6(int memory, int intensity)
    {
        if (memory >= MemoryFeelings.Length)
            return false;
        return MemoryMinIntensity[memory] <= intensity;
    }

    public static byte GetRandomFeeling6(int memory, int max = 24)
    {
        var bits = MemoryFeelings[memory];
        var rnd = Util.Rand;
        while (true)
        {
            int feel = rnd.Next(max);
            if ((bits & (1 << feel)) != 0)
                return (byte)feel;
        }
    }

    public static int GetMinimumIntensity6(int memory)
    {
        if (memory >= MemoryMinIntensity.Length)
            return -1;
        return MemoryMinIntensity[memory];
    }

    public override bool CanHaveIntensity(byte memory, byte intensity) => CanHaveIntensity6(memory, intensity);
    public override bool CanHaveFeeling(byte memory, byte feeling, ushort argument) => CanHaveFeeling6(memory, feeling, argument);
    public override int GetMinimumIntensity(byte memory) => GetMinimumIntensity6(memory);
}
