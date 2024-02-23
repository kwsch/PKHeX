using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public sealed partial class MemoryContext6 : MemoryContext
{
    private const int MAX_MEMORY_ID_XY = 64;
    private const int MAX_MEMORY_ID_AO = 69;
    public static readonly MemoryContext6 Instance = new();
    private MemoryContext6() { }

    public override EntityContext Context => EntityContext.Gen6;

    public static bool GetCanBeCaptured(ushort species, GameVersion version) => version switch
    {
        GameVersion.Any => GetCanBeCaptured(species, CaptureFlagsX) || GetCanBeCaptured(species, CaptureFlagsY)
                        || GetCanBeCaptured(species, CaptureFlagsAS) || GetCanBeCaptured(species, CaptureFlagsOR),
        GameVersion.X  => GetCanBeCaptured(species, CaptureFlagsX),
        GameVersion.Y  => GetCanBeCaptured(species, CaptureFlagsY),
        GameVersion.AS => GetCanBeCaptured(species, CaptureFlagsAS),
        GameVersion.OR => GetCanBeCaptured(species, CaptureFlagsOR),
        _ => false,
    };

    private static bool GetCanBeCaptured(ushort species, ReadOnlySpan<byte> flags)
    {
        int offset = species >> 3;
        if (offset >= flags.Length)
            return false;
        int bitIndex = species & 7;
        return (flags[offset] & (1 << bitIndex)) != 0;
    }

    private static ReadOnlySpan<byte> GetPokeCenterLocations(GameVersion game)
    {
        return GameVersion.XY.Contains(game) ? LocationsWithPokeCenter_XY : LocationsWithPokeCenter_AO;
    }

    public static bool GetHasPokeCenterLocation(GameVersion game, ushort loc)
    {
        if (game == GameVersion.Any)
            return GetHasPokeCenterLocation(GameVersion.X, loc) || GetHasPokeCenterLocation(GameVersion.AS, loc);
        if (loc > byte.MaxValue)
            return false;
        return GetPokeCenterLocations(game).Contains((byte)loc);
    }

    public static int GetMemoryRarity(byte memory) => memory >= MemoryRandChance.Length ? -1 : MemoryRandChance[memory];

    public override bool CanUseItemGeneric(int item)
    {
        // Key Item usage while in party on another species.
        if (KeyItemUsableObserveEonFlute == item)
            return true;
        if (KeyItemMemoryArgsAnySpecies.Contains((ushort)item))
            return true;

        return true; // todo
    }

    public override IEnumerable<ushort> GetMemoryItemParams()
    {
        var hashSet = new HashSet<ushort>(Legal.HeldItems_AO) { KeyItemUsableObserveEonFlute };
        foreach (var item in KeyItemMemoryArgsAnySpecies)
            hashSet.Add(item);
        foreach (var tm in ItemStorage6AO.Pouch_TMHM_AO[..100])
            hashSet.Add(tm);
        return hashSet;
    }

    public override bool IsUsedKeyItemUnspecific(int item) => KeyItemUsableObserveEonFlute == item;
    public override bool IsUsedKeyItemSpecific(int item, ushort species) => IsKeyItemMemoryArgValid(species, (ushort)item);

    public override bool CanPlantBerry(int item) => ItemStorage6XY.Pouch_Berry_XY.Contains((ushort)item);
    public override bool CanHoldItem(int item) => Legal.HeldItems_AO.Contains((ushort)item);

    public override bool CanObtainMemoryOT(GameVersion pkmVersion, byte memory) => pkmVersion switch
    {
        GameVersion.X or GameVersion.Y => CanObtainMemoryXY(memory),
        GameVersion.OR or GameVersion.AS => CanObtainMemoryAO(memory),
        _ => false,
    };

    public override bool CanObtainMemory(byte memory) => memory <= MAX_MEMORY_ID_AO;
    public override bool HasPokeCenter(GameVersion version, ushort location) => GetHasPokeCenterLocation(version, location);

    public override bool IsInvalidGeneralLocationMemoryValue(byte memory, ushort variable, IEncounterTemplate enc, PKM pk)
    {
        return false; // todo
    }

    public override bool IsInvalidMiscMemory(byte memory, ushort variable)
    {
        return false; // todo
    }

    private static bool CanObtainMemoryAO(byte memory) => memory <= MAX_MEMORY_ID_AO && !Memory_NotAO.Contains(memory);
    private static bool CanObtainMemoryXY(byte memory) => memory <= MAX_MEMORY_ID_XY && !Memory_NotXY.Contains(memory);
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

    public const byte MaxIntensity = 7;

    public static bool CanHaveIntensity6(int memory, int intensity)
    {
        if ((uint)intensity > MaxIntensity)
            return false;
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

    public static byte GetMinimumIntensity6(int memory)
    {
        if (memory >= MemoryMinIntensity.Length)
            return 0;
        return MemoryMinIntensity[memory];
    }

    public override bool CanHaveIntensity(byte memory, byte intensity) => CanHaveIntensity6(memory, intensity);
    public override bool CanHaveFeeling(byte memory, byte feeling, ushort argument) => CanHaveFeeling6(memory, feeling, argument);
    public override int GetMinimumIntensity(byte memory) => GetMinimumIntensity6(memory);
}
