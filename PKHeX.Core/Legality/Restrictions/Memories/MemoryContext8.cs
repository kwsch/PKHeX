using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public sealed partial class MemoryContext8 : MemoryContext
    {
        private const int MAX_MEMORY_ID_SWSH = 89;

        private static ICollection<int> GetPokeCenterLocations(GameVersion game)
        {
            return GameVersion.SWSH.Contains(game) ? LocationsWithPokeCenter_SWSH : Array.Empty<int>();
        }

        public static bool GetHasPokeCenterLocation(GameVersion game, int loc)
        {
            if (game == GameVersion.Any)
                return GetHasPokeCenterLocation(GameVersion.SW, loc);
            return GetPokeCenterLocations(game).Contains(loc);
        }

        public override IEnumerable<ushort> GetKeyItemParams() => (KeyItemMemoryArgsGen8.Values).SelectMany(z => z).Distinct();

        public override IEnumerable<ushort> GetMemoryItemParams() => Legal.HeldItem_AO.Concat(Legal.HeldItems_SWSH).Distinct()
            .Concat(GetKeyItemParams())
            .Concat(Legal.TMHM_AO.Take(100).Select(z => (ushort)z))
            .Where(z => z < Legal.MaxItemID_8_R2);

        public override bool CanUseItemGeneric(int item) => true; // todo

        public override bool IsUsedKeyItemUnspecific(int item) => false;
        public override bool IsUsedKeyItemSpecific(int item, int species) => KeyItemMemoryArgsGen8.TryGetValue(species, out var value) && value.Contains((ushort)item);
        public override bool CanHoldItem(int item) => Legal.HeldItems_SWSH.Contains((ushort)item);

        public override bool CanObtainMemoryOT(GameVersion pkmVersion, int memory) => pkmVersion switch
        {
            GameVersion.SW or GameVersion.SH => CanObtainMemorySWSH(memory),
            _ => false,
        };

        public override bool CanObtainMemoryHT(GameVersion pkmVersion, int memory) => CanObtainMemorySWSH(memory);

        public override bool CanObtainMemory(int memory) => CanObtainMemorySWSH(memory);
        public override bool HasPokeCenter(GameVersion version, int location) => GetHasPokeCenterLocation(version, location);

        public override bool IsInvalidGeneralLocationMemoryValue(int memory, int variable, IEncounterTemplate enc, PKM pk)
        {
            if (!Memories.MemoryGeneral.Contains(memory))
                return false;

            if (memory is 1 or 2 or 3) // Encounter only
                return IsInvalidGenLoc8(memory, pk.Met_Location, pk.Egg_Location, variable, pk, enc);
            return IsInvalidGenLoc8Other(memory, variable);
        }

        private static bool CanObtainMemorySWSH(int memory) => memory <= MAX_MEMORY_ID_SWSH && !Memory_NotSWSH.Contains(memory);

        public override bool CanWinRotoLoto(int item) => true;

        public override bool CanBuyItem(int item, GameVersion version)
        {
            if (item is 1085) // Bob's Food Tin
                return version is GameVersion.SW or GameVersion.Any;
            if (item is 1086) // Bach's Food Tin
                return version is GameVersion.SH or GameVersion.Any;
            return PurchaseableItemSWSH.Contains((ushort)item);
        }

        private static bool IsInvalidGenLoc8(int memory, int loc, int egg, int variable, PKM pk, IEncounterTemplate enc)
        {
            if (variable > 255)
                return true;

            switch (memory)
            {
                case 1 when enc.EggEncounter || IsCurryEncounter(pk, enc):
                case 2 when !enc.EggEncounter:
                case 3 when (enc is not (EncounterTrade or EncounterStatic {Gift: true} or WC8 {IsHOMEGift: false}) && !IsCurryEncounter(pk, enc)):
                    return true;
            }

            var arg = (byte)variable;
            if (loc > 255) // gift
                return memory != 3 || !PossibleGeneralLocations8.Contains(arg);
            if (memory == 2 && egg == 0)
                return true;
            if (loc is Encounters8Nest.SharedNest)
                return !PossibleGeneralLocations8.Contains(arg) || arg is 79; // dangerous place - all locations are Y-Comm locked
            if (SingleGenLocAreas.TryGetValue((byte)loc, out var val))
                return arg != val;
            if (MultiGenLocAreas.TryGetValue((byte)loc, out var arr))
                return !arr.Contains(arg);
            return false;
        }

        private static bool IsCurryEncounter(PKM pk, IEncounterTemplate enc)
        {
            if (enc is not EncounterSlot8)
                return false;
            return pk is IRibbonSetMark8 { RibbonMarkCurry: true } || pk.Species == (int)Species.Shedinja;
        }

        private static bool IsInvalidGenLoc8Other(int memory, int variable)
        {
            if (variable > byte.MaxValue)
                return true;
            var arg = (byte)variable;
            return memory switch
            {
                _ => !PossibleGeneralLocations8.Contains(arg),
            };
        }

        public override bool CanHaveIntensity(int memory, int intensity) => true; // todo
        public override bool CanHaveFeeling(int memory, int feeling) => true; // todo
        public override int GetMinimumIntensity(int memory) => 3; // todo
    }
}
