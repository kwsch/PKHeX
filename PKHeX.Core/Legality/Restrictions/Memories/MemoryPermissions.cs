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
        public static bool IsMemoryOfKnownMove(int memory) => memory is 48 or 80 or 81;

        public static bool CanWinLotoID(int generation, int item)
        {
            var context = Memories.GetContext(generation);
            return context.CanWinLotoID(item);
        }

        public static bool CanHoldItem(int generation, int item)
        {
            var context = Memories.GetContext(generation);
            return context.CanHoldItem(item);
        }

        public static bool CanPlantBerry(int generation, int item)
        {
            var context = Memories.GetContext(generation);
            return context.CanPlantBerry(item);
        }

        public static bool CanUseItemGeneric(int generation, int item)
        {
            var context = Memories.GetContext(generation);
            return context.CanUseItemGeneric(item);
        }

        public static bool CanUseItem(int generation, int item, int species)
        {
            if (IsUsedKeyItemUnspecific(generation, item))
                return true;
            if (IsUsedKeyItemSpecific(generation, item, species))
                return true;
            return true; // todo
        }

        private static bool IsUsedKeyItemUnspecific(int generation, int item)
        {
            var context = Memories.GetContext(generation);
            return context.IsUsedKeyItemUnspecific(item);
        }

        private static bool IsUsedKeyItemSpecific(int generation, int item, int species)
        {
            var context = Memories.GetContext(generation);
            return context.IsUsedKeyItemSpecific(item, species);
        }

        public static bool CanBuyItem(int generation, int item, GameVersion version = GameVersion.Any)
        {
            var context = Memories.GetContext(generation);
            return context.CanBuyItem(item, version);
        }

        public static bool GetCanLearnMachineMove(PKM pkm, EvoCriteria[] evos, int move, int generation, GameVersion version = GameVersion.Any)
        {
            if (IsOtherFormMove(pkm, evos, move, generation, version, types: MoveSourceType.AllMachines))
                return true;
            return MoveList.GetValidMoves(pkm, version, evos, generation, types: MoveSourceType.AllMachines).Contains(move);
        }

        private static bool IsOtherFormMove(PKM pkm, EvoCriteria[] evos, int move, int generation, GameVersion version, MoveSourceType types)
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

            // Relearns can be wiped via Battle Version. Check for eggs too.
            if (IsSpecialEncounterMoveEggDeleted(pkm, enc))
            {
                var em = MoveEgg.GetEggMoves(enc.Generation, enc.Species, enc.Form, enc.Version);
                if (em.Contains(move))
                    return true;
            }

            if (battleOnly)
            {
                // Some moves can only be known in battle; outside of battle they aren't obtainable as a memory parameter.
                switch (move)
                {
                    case (int)BehemothBlade when pkm.Species == (int)Zacian:
                    case (int)BehemothBash when pkm.Species == (int)Zamazenta:
                        return true;
                }
                if (gen == 8 && Legal.IsDynamaxMove(move))
                    return true;
                if (pkm.Species == (int)Ditto)
                {
                    if (move == (int)Struggle)
                        return false;
                    return gen switch
                    {
                        8 => move <= Legal.MaxMoveID_8_R2 && !Legal.GetDummiedMovesHashSet(pkm).Contains(move),
                        _ => move <= Legal.MaxMoveID_6_AO,
                    };
                }
            }

            return false;
        }

        private static bool IsSpecialEncounterMoveEggDeleted(PKM pkm, IEncounterable enc)
        {
            if (pkm is IBattleVersion { BattleVersion: not 0 }) // can hide Relearn moves (Gen6+ Eggs, or DexNav)
                return enc is EncounterEgg { Generation: >= 6 } or EncounterSlot6AO { CanDexNav: true } or EncounterSlot8b { IsUnderground: true };
            return enc is EncounterEgg { Generation: < 6 }; // egg moves that are no longer in the movepool
        }

        public static bool GetCanRelearnMove(PKM pkm, int move, int generation, EvoCriteria[] evos, GameVersion version = GameVersion.Any)
        {
            if (IsOtherFormMove(pkm, evos, move, generation, version, types: MoveSourceType.Reminder))
                return true;
            return MoveList.GetValidMoves(pkm, version, evos, generation, types: MoveSourceType.Reminder).Contains(move);
        }

        private static bool GetCanKnowMove(PKM pkm, int move, int generation, EvolutionHistory evos, GameVersion version = GameVersion.Any)
        {
            if (pkm.Species == (int)Smeargle)
                return Legal.IsValidSketch(move, generation);

            if (generation >= 8 && MoveEgg.GetIsSharedEggMove(pkm, generation, move))
                return true;

            if (evos.Length <= generation)
                return false;
            for (int i = 1; i <= generation; i++)
            {
                var chain = evos[i];
                if (chain.Length == 0)
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
                _ => false,
            },
            8 => version switch
            {
                GameVersion.Any => GetCanBeCaptured(species, SlotsSW.Concat(SlotsSH), StaticSW.Concat(StaticSH)),
                GameVersion.SW => GetCanBeCaptured(species, SlotsSW, StaticSW),
                GameVersion.SH => GetCanBeCaptured(species, SlotsSH, StaticSH),
                _ => false,
            },
            _ => false,
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
                _              => DynamaxTrainer_SWSH.Contains(species) || IsDynamaxSW(species) || IsDynamaxSH(species),
            };
        }

        // exclusive to version
        private static bool IsDynamaxSW(int species) => species is (int)Machamp or (int)Gigalith or (int)Conkeldurr or (int)Coalossal or (int)Flapple;
        private static bool IsDynamaxSH(int species) => species is (int)Gengar or (int)Lapras or (int)Dusknoir or (int)Froslass or (int)Appletun;

        // common to SW & SH
        private static readonly HashSet<int> DynamaxTrainer_SWSH = new()
        {
            (int)Venusaur,
            (int)Blastoise,
            (int)Charizard,
            (int)Slowbro,
            (int)Gyarados,
            (int)Snorlax,
            (int)Slowking,
            (int)Torkoal,
            (int)Vespiquen,
            (int)Regigigas,
            (int)Garbodor,
            (int)Haxorus,
            (int)Tsareena,
            (int)Rillaboom,
            (int)Inteleon,
            (int)Cinderace,
            (int)Greedent,
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

        public static bool GetCanFishSpecies(int species, int gen, GameVersion version) => gen switch
        {
            6 => version switch
            {
                GameVersion.Any => FishingSpecies_XY.Contains(species) || FishingSpecies_AO.Contains(species)
                                || IsFishingSpeciesX(species) || IsFishingSpeciesY(species),

                GameVersion.X => FishingSpecies_XY.Contains(species) || IsFishingSpeciesX(species),
                GameVersion.Y => FishingSpecies_XY.Contains(species) || IsFishingSpeciesY(species),

                GameVersion.OR or GameVersion.AS => FishingSpecies_AO.Contains(species),
                _ => false,
            },
            8 => version switch
            {
                GameVersion.Any or GameVersion.SW or GameVersion.SH => FishingSpecies_SWSH.Contains(species),
                _ => false,
            },
            _ => false,
        };

        private static readonly HashSet<int> FishingSpecies_SWSH = new()
        {
            (int)Shellder, (int)Cloyster,
            (int)Krabby,
            (int)Goldeen,
            (int)Magikarp, (int)Gyarados,
            (int)Lapras,
            (int)Dratini,
            (int)Chinchou, (int)Lanturn,
            (int)Qwilfish,
            (int)Remoraid, (int)Octillery,
            (int)Carvanha, (int)Sharpedo,
            (int)Wailmer, (int)Wailord,
            (int)Barboach, (int)Whiscash,
            (int)Corphish,
            (int)Lileep,
            (int)Feebas,
            (int)Mantyke, (int)Mantine,
            (int)Basculin,
            (int)Wishiwashi,
            (int)Mareanie,
            (int)Pyukumuku,
            (int)Dhelmise,
            (int)Chewtle, (int)Drednaw,
            (int)Arrokuda, (int)Barraskewda,
        };

        private static readonly HashSet<int> FishingSpecies_AO = new()
        {
            (int)Tentacool,
            (int)Horsea, (int)Seadra,
            (int)Goldeen, (int)Seaking,
            (int)Staryu,
            (int)Magikarp, (int)Gyarados,
            (int)Corsola,
            (int)Remoraid, (int)Octillery,
            (int)Carvanha, (int)Sharpedo,
            (int)Wailmer,
            (int)Barboach, (int)Whiscash,
            (int)Corphish, (int)Crawdaunt,
            (int)Feebas,
            (int)Luvdisc,
        };

        // exclusive to version
        private static bool IsFishingSpeciesX(int species) => species is (int)Staryu or (int)Starmie or (int)Huntail or (int)Clauncher or (int)Clawitzer;
        private static bool IsFishingSpeciesY(int species) => species is (int)Shellder or (int)Cloyster or (int)Gorebyss or (int)Skrelp or (int)Dragalge;

        // common to X & Y
        private static readonly HashSet<int> FishingSpecies_XY = new()
        {
            (int)Poliwag, (int)Poliwhirl, (int)Poliwrath, (int)Politoed,
            (int)Horsea, (int)Seadra,
            (int)Goldeen, (int)Seaking,
            (int)Magikarp, (int)Gyarados,
            (int)Dratini, (int)Dragonair,
            (int)Chinchou, (int)Lanturn,
            (int)Qwilfish,
            (int)Corsola,
            (int)Remoraid, (int)Octillery,
            (int)Carvanha, (int)Sharpedo,
            (int)Barboach, (int)Whiscash,
            (int)Corphish, (int)Crawdaunt,
            (int)Clamperl,
            (int)Relicanth,
            (int)Luvdisc,
            (int)Basculin,
            (int)Alomomola,
        };
    }
}
