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
            if (generation == 6)
            {
                // Key Item usage while in party on another species.
                if (Memories.KeyItemUsableObserve6.Contains((ushort) item))
                    return true;
                if (Memories.KeyItemMemoryArgsGen6.Values.Any(z => z.Contains((ushort) item)))
                    return true;
            }
            return true; // todo
        }

        public static bool CanUseItem(int generation, int item, int species)
        {
            if (IsUsedKeyItemUnspecific(generation, item))
                return true;
            if (IsUsedKeyItemSpecific(generation, item, species))
                return true;
            return true; // todo
        }

        private static bool IsUsedKeyItemUnspecific(int generation, int item) => generation switch
        {
            6 => Memories.KeyItemUsableObserve6.Contains((ushort)item),
            _ => false
        };

        private static bool IsUsedKeyItemSpecific(int generation, int item, int species) => generation switch
        {
            6 => Memories.KeyItemMemoryArgsGen6.TryGetValue(species, out var value) && value.Contains((ushort) item),
            8 => Memories.KeyItemMemoryArgsGen8.TryGetValue(species, out var value) && value.Contains((ushort) item),
            _ => false
        };

        public static bool CanBuyItem(int generation, int item)
        {
            return true; // todo
        }

        public static bool GetCanLearnMachineMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            var evos = EvolutionChain.GetValidPreEvolutions(pkm);
            return MoveList.GetValidMoves(pkm, version, evos, generation, types: MoveSourceType.AllMachines).Contains(move);
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

            if (GetCanKnowMove(pkm, memory.Variable, gen, info.EvoChainsAllGens))
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

        public static bool GetCanRelearnMove(PKM pkm, int move, int generation, IReadOnlyList<EvoCriteria> evos, GameVersion version = GameVersion.Any)
        {
            return MoveList.GetValidMoves(pkm, version, evos, generation, types: MoveSourceType.Reminder).Contains(move);
        }

        private static bool GetCanKnowMove(PKM pkm, int move, int generation, IReadOnlyList<IReadOnlyList<EvoCriteria>> evos, GameVersion version = GameVersion.Any)
        {
            if (pkm.Species == (int)Smeargle)
                return Legal.IsValidSketch(move, generation);

            if (generation >= 8 && MoveEgg.GetIsSharedEggMove(pkm, generation, move))
                return true;

            if (evos.Count <= generation)
                return false;
            for (int i = 1; i <= generation; i++)
            {
                var moves = MoveList.GetValidMoves(pkm, version, evos[i], i, types: MoveSourceType.All);
                if (moves.Contains(move))
                    return true;
            }
            return false;
        }

        public static bool GetCanBeCaptured(int species, int gen, GameVersion version) => gen switch
        {
            6 => version switch
            {
                GameVersion.Any => GetCanBeCaptured(species, SlotsX, StaticX) || GetCanBeCaptured(species, SlotsY, StaticY)
                                || GetCanBeCaptured(species, SlotsA, StaticA) || GetCanBeCaptured(species, SlotsO, StaticO),

                GameVersion.X => GetCanBeCaptured(species, SlotsX, StaticX),
                GameVersion.Y => GetCanBeCaptured(species, SlotsY, StaticY),

                GameVersion.AS => GetCanBeCaptured(species, SlotsA, StaticA),
                GameVersion.OR => GetCanBeCaptured(species, SlotsO, StaticO),
                _ => false
            },
            8 => version switch
            {
                GameVersion.Any => GetCanBeCaptured(species, SlotsSW.Concat(SlotsSH), StaticSW.Concat(StaticSH)),
                GameVersion.SW => GetCanBeCaptured(species, SlotsSW, StaticSW),
                GameVersion.SH => GetCanBeCaptured(species, SlotsSH, StaticSH),
                _ => false
            },
            _ => false
        };

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
