using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public sealed partial class MemoryContext8 : MemoryContext
{
    private const int MAX_MEMORY_ID_SWSH = 89;

    public override IEnumerable<ushort> GetKeyItemParams() => (KeyItemMemoryArgsGen8.Values).SelectMany(z => z).Distinct();

    public override IEnumerable<ushort> GetMemoryItemParams() => Legal.HeldItem_AO.Concat(Legal.HeldItems_SWSH).Distinct()
        .Concat(GetKeyItemParams())
        .Concat(Legal.Pouch_TMHM_AO.Take(100))
        .Where(z => z <= Legal.MaxItemID_8_R2);

    public override bool CanUseItemGeneric(int item) => true; // todo

    public override bool IsUsedKeyItemUnspecific(int item) => false;
    public override bool IsUsedKeyItemSpecific(int item, int species) => KeyItemMemoryArgsGen8.TryGetValue(species, out var value) && value.Contains((ushort)item);
    public override bool CanHoldItem(int item) => Legal.HeldItems_SWSH.Contains((ushort)item);

    public override bool CanObtainMemoryOT(GameVersion pkmVersion, byte memory) => pkmVersion switch
    {
        GameVersion.SW or GameVersion.SH => CanObtainMemorySWSH(memory),
        _ => memory is 0,
    };

    public override bool CanObtainMemoryHT(GameVersion pkmVersion, byte memory) => CanObtainMemorySWSH(memory);

    public override bool CanObtainMemory(byte memory) => CanObtainMemorySWSH(memory);
    public override bool HasPokeCenter(GameVersion version, int location) => location == 9; // in a Pokémon Center

    public override bool IsInvalidGeneralLocationMemoryValue(byte memory, ushort variable, IEncounterTemplate enc, PKM pk)
    {
        if (!Memories.MemoryGeneral.Contains(memory))
            return false;

        if (memory is 1 or 2 or 3) // Encounter only
            return IsInvalidGenLoc8(memory, pk.Met_Location, pk.Egg_Location, variable, pk, enc);
        return IsInvalidGenLoc8Other(memory, variable);
    }

    public override bool IsInvalidMiscMemory(byte memory, ushort variable)
    {
        return memory switch
        {
            // {0} encountered {2} when it was with {1}. {4} that {3}.
            29 when variable is not (888 or 889 or 890 or 898) => true, // Zacian, Zamazenta, Eternatus, Calyrex
            _ => false,
        };
    }

    private static bool CanObtainMemorySWSH(int memory) => memory <= MAX_MEMORY_ID_SWSH && !Memory_NotSWSH.Contains(memory);

    public override bool CanWinLotoID(int item) => LotoPrizeSWSH.Contains((ushort)item);

    public override bool CanBuyItem(int item, GameVersion version) => item switch
    {
        1085 => version is GameVersion.SW or GameVersion.Any, // Bob's Food Tin
        1086 => version is GameVersion.SH or GameVersion.Any, // Bach's Food Tin
        _ => PurchaseableItemSWSH.Contains((ushort)item),
    };

    private static bool IsInvalidGenLoc8(byte memory, int loc, int egg, ushort variable, PKM pk, IEncounterTemplate enc)
    {
        if (variable > 255)
            return true;

        switch (memory)
        {
            case 1 when !IsWildEncounter(pk, enc):
            case 2 when !enc.EggEncounter:
            case 3 when !IsWildEncounterMeet(pk, enc):
                return true;
        }

        var arg = (byte)variable;
        if (loc > 255) // gift
            return memory != 3 || !PossibleGeneralLocations8.Contains(arg);
        if (memory == 2 && egg == 0)
            return true;
        if (loc is Encounters8Nest.SharedNest)
            return !PossibleGeneralLocations8.Contains(arg) || arg is 79; // dangerous place - all locations are Y-Comm locked
        if (SingleGenLocAreas.TryGetValue((byte)loc, out var value))
            return arg != value;
        if (MultiGenLocAreas.TryGetValue((byte)loc, out var arr))
            return !arr.Contains(arg);
        return false;
    }

    private static bool IsWildEncounterMeet(PKM pk, IEncounterTemplate enc)
    {
        if (enc is EncounterTrade or EncounterStatic { Gift: true } or WC8 { IsHOMEGift: false })
            return true;
        if (IsCurryEncounter(pk, enc))
            return true;
        return false;
    }

    private static bool IsWildEncounter(PKM pk, IEncounterTemplate enc)
    {
        if (enc is not (EncounterSlot or EncounterStatic { Gift: false }))
            return false;
        if (pk is IRibbonSetMark8 { RibbonMarkCurry: true })
            return false;
        if (pk.Species == (int)Species.Shedinja && pk is IRibbonSetAffixed { AffixedRibbon: (int)RibbonIndex.MarkCurry })
            return false;
        return true;
    }

    private static bool IsCurryEncounter(PKM pk, IEncounterTemplate enc)
    {
        if (enc is not EncounterSlot8)
            return false;
        return pk is IRibbonSetMark8 { RibbonMarkCurry: true } || pk.Species == (int)Species.Shedinja;
    }

    private static bool IsInvalidGenLoc8Other(byte memory, ushort variable)
    {
        if (variable > byte.MaxValue)
            return true;
        var arg = (byte)variable;
        return memory switch
        {
            // {0} became {1}’s friend when it came through Link Trade {2}. {4} that {3}.
            4 when !PossibleGeneralLocations8.Contains(arg) || arg is 79 => true, // dangerous place - all locations are Y-Comm locked

            // {0} rode a bike with {1} {2}. {4} that {3}.
            32 when arg is not (1 or 8 or 12 or 22 or 33 or 35 or 37 or 40 or 41 or 44 or 47 or 48 or 49 or 50 or 51 or 53 or 65 or 71 or 72 or 75 or 76 or 77 or 78) => true,

            // {0} saw {1} secretly picking up something {2}. {4} that {3}.
            39 when arg is not (8 or 12 or 22 or 33 or 35 or 37 or 40 or 41 or 44 or 47 or 48 or 49 or 50 or 51 or 53 or 65 or 71 or 72 or 75 or 76 or 77) => true,

            // {0} checked the sign with {1} {2}. {4} that {3}.
            42 when arg is not (1 or 12 or 22 or 33 or 35 or 37 or 44 or 47 or 53 or 71 or 72 or 76 or 77) => true,

            // {0} sat with {1} on a bench {2}. {4} that {3}.
            70 when arg is not (12 or 22 or 28 or 33 or 35 or 37 or 38 or 44 or 53 or 77) => true,

            _ => !PossibleGeneralLocations8.Contains(arg),
        };
    }

    public static bool CanHaveFeeling8(byte memory, byte feeling, ushort argument)
    {
        if (memory >= MemoryFeelings.Length)
            return false;
        if (feeling <= 0)
            return false; // Different from Gen6; this +1 is to match them treating 0 as empty
        return (MemoryFeelings[memory] & (1 << --feeling)) != 0;
    }

    public static bool CanHaveIntensity8(byte memory, byte intensity)
    {
        if (memory >= MemoryFeelings.Length)
            return false;
        return MemoryMinIntensity[memory] <= intensity;
    }

    public static byte GetRandomFeeling8(int memory, int max = 24)
    {
        var bits = MemoryFeelings[memory];
        var rnd = Util.Rand;
        while (true)
        {
            int feel = rnd.Next(max);
            if ((bits & (1 << feel)) != 0)
                return (byte)(feel + 1); // Different from Gen6; this +1 is to match them treating 0 as empty
        }
    }

    public static int GetMinimumIntensity8(int memory)
    {
        if (memory >= MemoryMinIntensity.Length)
            return -1;
        return MemoryMinIntensity[memory];
    }

    public override bool CanHaveIntensity(byte memory, byte intensity) => CanHaveIntensity8(memory, intensity);
    public override bool CanHaveFeeling(byte memory, byte feeling, ushort argument) => CanHaveFeeling8(memory, feeling, argument);
    public override int GetMinimumIntensity(byte memory) => GetMinimumIntensity8(memory);
}
