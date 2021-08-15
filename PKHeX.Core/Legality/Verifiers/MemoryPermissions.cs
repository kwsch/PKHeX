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
        public static bool IsMoveKnowMemory(int memory) => memory is 16 or 48 or 80 or 81;

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

        public static bool CanBuyItem(int generation, int item, GameVersion version = GameVersion.Any)
        {
            return generation switch
            {
                6 => CanBuyItem6(item, version),
                8 => CanBuyItem8(item, version),
                _ => true, // todo
            };
        }

        private static bool CanBuyItem6(int item, GameVersion version)
        {
            if (version is GameVersion.Any)
                return PurchaseableItemAO.Contains((ushort) item) || PurchaseableItemXY.Contains((ushort) item);
            if (version is GameVersion.X or GameVersion.Y)
                return PurchaseableItemXY.Contains((ushort)item);
            if (version is GameVersion.AS or GameVersion.OR)
                return PurchaseableItemAO.Contains((ushort)item);
            return false;
        }

        private static bool CanBuyItem8(int item, GameVersion version)
        {
            if (item is 1085) // Bob's Food Tin
                return version is GameVersion.SW or GameVersion.Any;
            if (item is 1086) // Bach's Food Tin
                return version is GameVersion.SH or GameVersion.Any;
            return PurchaseableItemSWSH.Contains((ushort) item);
        }

        public static bool GetCanLearnMachineMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            var evos = EvolutionChain.GetValidPreEvolutions(pkm);
            return GetCanLearnMachineMove(pkm, evos, move, generation, version);
        }

        public static bool GetCanLearnMachineMove(PKM pkm, IReadOnlyList<EvoCriteria> evos, int move, int generation, GameVersion version = GameVersion.Any)
        {
            if (IsOtherFormMove(pkm, evos, move, generation, version, types: MoveSourceType.AllMachines))
                return true;
            return MoveList.GetValidMoves(pkm, version, evos, generation, types: MoveSourceType.AllMachines).Contains(move);
        }

        private static bool IsOtherFormMove(PKM pkm, IReadOnlyList<EvoCriteria> evos, int move, int generation, GameVersion version, MoveSourceType types)
        {
            if (!Legal.FormChangeMoves.TryGetValue(pkm.Species, out var criteria))
                return false;
            if (!criteria(generation, pkm.Form))
                return false;
            var list = new List<int>(8);
            MoveList.GetValidMovesAllForms(pkm, evos, version, generation, types, false, pkm.Species, list);
            return list.Contains(move);
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

            if (GetCanKnowMove(pkm, move, gen, info.EvoChainsAllGens))
                return true;

            var enc = info.EncounterMatch;
            if (enc is IMoveset ms && ms.Moves.Contains(move))
                return true;
            if (enc is IRelearn r && r.Relearn.Contains(move))
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
            if (IsOtherFormMove(pkm, evos, move, generation, version, types: MoveSourceType.Reminder))
                return true;
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
                var chain = evos[i];
                if (chain.Count == 0)
                    continue;

                var moves = MoveList.GetValidMoves(pkm, version, chain, i, types: MoveSourceType.All);
                if (moves.Contains(move))
                    return true;

                if (IsOtherFormMove(pkm, chain, move, i, GameVersion.Any, types: MoveSourceType.All))
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
            if (area.Any(loc => loc.HasSpecies(species)))
                return true;
            if (statics.Any(enc => enc.Species == species && !enc.Gift))
                return true;
            return false;
        }

        public static bool GetCanDynamaxTrainer(int species, int gen, GameVersion version)
        {
            if (gen != 8)
                return false;

            return version switch
            {
                GameVersion.SW => DynamaxTrainer_SWSH.Contains(species) || IsDynamaxSW(species),
                GameVersion.SH => DynamaxTrainer_SWSH.Contains(species) || IsDynamaxSH(species),
                _              => DynamaxTrainer_SWSH.Contains(species) || IsDynamaxSW(species) || IsDynamaxSH(species)
            };
        }

        // exclusive to version
        private static bool IsDynamaxSW(int species) => species is (int) Machamp or (int) Coalossal or (int) Flapple;
        private static bool IsDynamaxSH(int species) => species is (int) Gengar or (int) Lapras or (int) Appletun;

        // common to SW & SH
        private static readonly HashSet<int> DynamaxTrainer_SWSH = new()
        {
            (int)Venusaur,
            (int)Blastoise,
            (int)Charizard,
            (int)Slowbro,
            (int)Snorlax,
            (int)Slowking,
            (int)Garbodor,
            (int)Rillaboom,
            (int)Inteleon,
            (int)Cinderace,
            (int)Corviknight,
            (int)Eldegoss,
            (int)Drednaw,
            (int)Centiskorch,
            (int)Hatterene,
            (int)Grimmsnarl,
            (int)Alcremie,
            (int)Copperajah,
            (int)Duraludon,
            (int)Urshifu,
        };

        private static readonly HashSet<ushort> PurchaseableItemXY = new()
        {
            002, 003, 004, 006, 007, 008, 009, 010, 011, 012,
            013, 014, 015, 017, 018, 019, 020, 021, 022, 023,
            024, 025, 026, 027, 028, 034, 035, 036, 037, 045,
            046, 047, 048, 049, 052, 055, 056, 057, 058, 059,
            060, 061, 062, 076, 077, 078, 079, 082, 084, 085,
            254, 255, 314, 315, 316, 317, 318, 319, 320, 334,
            338, 341, 342, 343, 345, 347, 352, 355, 360, 364,
            365, 377, 379, 395, 402, 403, 405, 411, 618,
        };

        private static readonly HashSet<ushort> PurchaseableItemAO = new()
        {
            002, 003, 004, 006, 007, 008, 009, 010, 011, 013,
            014, 015, 017, 018, 019, 020, 021, 022, 023, 024,
            025, 026, 027, 028, 034, 035, 036, 037, 045, 046,
            047, 048, 049, 052, 055, 056, 057, 058, 059, 060,
            061, 062, 063, 064, 076, 077, 078, 079, 254, 255,
            314, 315, 316, 317, 318, 319, 320, 328, 336, 341,
            342, 343, 344, 347, 352, 360, 365, 367, 369, 374,
            379, 384, 395, 398, 400, 403, 405, 409, 577, 692,
            694,
        };

        internal static readonly HashSet<ushort> PurchaseableItemSWSH = new(Legal.TR_SWSH)
        {
            0002, 0003, 0004, 0006, 0007, 0008, 0009, 0010, 0011, 0013,
            0014, 0015, 0017, 0018, 0019, 0020, 0021, 0022, 0023, 0024,
            0025, 0026, 0027, 0028, 0033, 0034, 0035, 0036, 0037, 0042,
            0045, 0046, 0047, 0048, 0049, 0050, 0051, 0052, 0055, 0056,
            0057, 0058, 0059, 0060, 0061, 0062, 0063, 0076, 0077, 0079,
            0089, 0149, 0150, 0151, 0155, 0214, 0215, 0219, 0220, 0254,
            0255, 0269, 0270, 0271, 0275, 0280, 0287, 0289, 0290, 0291,
            0292, 0293, 0294, 0297, 0314, 0315, 0316, 0317, 0318, 0319,
            0320, 0321, 0325, 0326, 0541, 0542, 0545, 0546, 0547, 0639,
            0640, 0645, 0646, 0647, 0648, 0649, 0795, 0846, 0879, 1084,
            1087, 1088, 1089, 1090, 1091, 1094, 1095, 1097, 1098, 1099,
            1118, 1119, 1121, 1122, 1231, 1232, 1233, 1234, 1235, 1236,
            1237, 1238, 1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246,
            1247, 1248, 1249, 1250, 1251, 1252, 1256, 1257, 1258, 1259,
            1260, 1261, 1262, 1263,
        };
    }
}
