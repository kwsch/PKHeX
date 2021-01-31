using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Encounters6;
using static PKHeX.Core.Encounters8;
using static PKHeX.Core.Move;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Validation logic for specific memory conditions
    /// </summary>
    public static class MemoryPermissions
    {
        public static bool CanWinRotoLoto(int generation, int item)
        {
            return true; // todo
        }

        public static bool CanHoldItem(int generation, int item)
        {
            return true; // todo
        }

        public static bool CanPlantBerry(int generation, int item)
        {
            return true; // todo
        }

        public static bool CanUseItemGeneric(int generation, int item)
        {
            return true; // todo
        }

        public static bool CanUseItem(int generation, int item, int species)
        {
            return true; // todo
        }

        public static bool CanBuyItem(int generation, int item)
        {
            return true; // todo
        }

        public static bool CanKnowMove(PKM pkm, MemoryVariableSet memory, int gen, LegalInfo info, bool battleOnly = false)
        {
            var move = memory.Variable;
            if (move == 0)
                return false;

            if (pkm.HasMove(move))
                return true;

            if (pkm.IsEgg)
                return false;

            if (Legal.GetCanKnowMove(pkm, memory.Variable, gen, info.EvoChainsAllGens))
                return true;

            var enc = info.EncounterMatch;
            if (enc is IMoveset ms && ms.Moves.Contains(move))
                return true;

            if (battleOnly)
            {
                // Some moves can only be known in battle; outside of battle they aren't obtainable as a memory parameter.
                switch (move)
                {
                    case (int)BehemothBlade when pkm.Species == (int)Zacian:
                    case (int)BehemothBash when pkm.Species == (int)Zamazenta:
                        return true;
                }
            }

            return false;
        }

        public static bool GetCanBeCaptured(int species, int gen, GameVersion version)
        {
            switch (gen)
            {
                // Capture Memory only obtainable via Gen 6.
                case 6:
                    switch (version)
                    {
                        case GameVersion.Any:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsX, StaticX)
                                   || GetCanBeCaptured(species, SlotsY, StaticY)
                                   || GetCanBeCaptured(species, SlotsA, StaticA)
                                   || GetCanBeCaptured(species, SlotsO, StaticO);
                        case GameVersion.X:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsX, StaticX);
                        case GameVersion.Y:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsY, StaticY);

                        case GameVersion.AS:
                            return GetCanBeCaptured(species, SlotsA, StaticA);
                        case GameVersion.OR:
                            return GetCanBeCaptured(species, SlotsO, StaticO);
                    }
                    break;

                case 8:
                    {
                        switch (version)
                        {
                            case GameVersion.Any:
                                return GetCanBeCaptured(species, SlotsSW.Concat(SlotsSH), StaticSW.Concat(StaticSH));
                            case GameVersion.SW:
                                return GetCanBeCaptured(species, SlotsSW, StaticSW);
                            case GameVersion.SH:
                                return GetCanBeCaptured(species, SlotsSH, StaticSH);
                        }
                        break;
                    }
            }
            return false;
        }

        private static bool GetCanBeCaptured(int species, IEnumerable<EncounterArea> area, IEnumerable<EncounterStatic> statics)
        {
            if (area.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                return true;
            if (statics.Any(enc => enc.Species == species && !enc.Gift))
                return true;
            return false;
        }
    }
}
