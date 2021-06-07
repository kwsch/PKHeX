//#define VERIFY_GEN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    /// <summary>
    /// Generates weakly matched <see cref="IEncounterable"/> objects for an input <see cref="PKM"/> (and/or criteria).
    /// </summary>
    public static class EncounterMovesetGenerator
    {
        /// <summary>
        /// Order in which <see cref="IEncounterable"/> objects are yielded from the <see cref="GenerateVersionEncounters"/> generator.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public static IReadOnlyCollection<EncounterOrder> PriorityList { get; set; } = PriorityList = (EncounterOrder[])Enum.GetValues(typeof(EncounterOrder));

        /// <summary>
        /// Resets the <see cref="PriorityList"/> to the default values.
        /// </summary>
        public static void ResetFilters() => PriorityList = (EncounterOrder[])Enum.GetValues(typeof(EncounterOrder));

        /// <summary>
        /// Gets possible <see cref="PKM"/> objects that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="info">Trainer information of the receiver.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
        /// <returns>A consumable <see cref="PKM"/> list of possible results.</returns>
        /// <remarks>When updating, update the sister <see cref="GenerateEncounters(PKM,ITrainerInfo,int[],GameVersion[])"/> method.</remarks>
        public static IEnumerable<PKM> GeneratePKMs(PKM pk, ITrainerInfo info, int[]? moves = null, params GameVersion[] versions)
        {
            pk.TID = info.TID;
            var m = moves ?? pk.Moves;
            var vers = versions.Length >= 1 ? versions : GameUtil.GetVersionsWithinRange(pk, pk.Format);
            foreach (var ver in vers)
            {
                var encounters = GenerateVersionEncounters(pk, m, ver);
                foreach (var enc in encounters)
                {
                    var result = enc.ConvertToPKM(info);
#if VERIFY_GEN
                    var la = new LegalityAnalysis(result);
                    if (!la.Valid)
                        throw new Exception("Legality analysis of generated Pokémon is invalid");
#endif
                    yield return result;
                }
            }
        }

        /// <summary>
        /// Gets possible <see cref="IEncounterable"/> objects that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="info">Trainer information of the receiver.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible results.</returns>
        /// <remarks>When updating, update the sister <see cref="GeneratePKMs(PKM,ITrainerInfo,int[],GameVersion[])"/> method.</remarks>
        public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, ITrainerInfo info, int[]? moves = null, params GameVersion[] versions)
        {
            pk.TID = info.TID;
            var m = moves ?? pk.Moves;
            var vers = versions.Length >= 1 ? versions : GameUtil.GetVersionsWithinRange(pk, pk.Format);
            foreach (var ver in vers)
            {
                var encounters = GenerateVersionEncounters(pk, m, ver);
                foreach (var enc in encounters)
                    yield return enc;
            }
        }

        /// <summary>
        /// Gets possible <see cref="PKM"/> objects that allow all moves requested to be learned within a specific generation.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="info">Trainer information of the receiver.</param>
        /// <param name="generation">Specific generation to iterate versions for.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        public static IEnumerable<PKM> GeneratePKMs(PKM pk, ITrainerInfo info, int generation, int[]? moves = null)
        {
            var vers = GameUtil.GetVersionsInGeneration(generation, pk.Version);
            return GeneratePKMs(pk, info, moves, vers);
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="generation">Specific generation to iterate versions for.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateEncounter(PKM pk, int generation, int[]? moves = null)
        {
            var vers = GameUtil.GetVersionsInGeneration(generation, pk.Version);
            return GenerateEncounters(pk, moves, vers);
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, int[]? moves = null, params GameVersion[] versions)
        {
            moves ??= pk.Moves;
            if (versions.Length > 0)
                return GenerateEncounters(pk, moves, (IReadOnlyList<GameVersion>)versions);

            var vers = GameUtil.GetVersionsWithinRange(pk, pk.Format);
            return vers.SelectMany(ver => GenerateVersionEncounters(pk, moves, ver));
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="vers">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, int[]? moves, IReadOnlyList<GameVersion> vers)
        {
            moves ??= pk.Moves;
            return vers.SelectMany(ver => GenerateVersionEncounters(pk, moves, ver));
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="version">Specific version to iterate for.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateVersionEncounters(PKM pk, IEnumerable<int> moves, GameVersion version)
        {
            pk.Version = (int)version;
            var et = EvolutionTree.GetEvolutionTree(pk.Format);
            var chain = et.GetValidPreEvolutions(pk, maxLevel: 100, skipChecks: true);
            int[] needs = GetNeededMoves(pk, moves, chain);

            return PriorityList.SelectMany(type => GetPossibleOfType(pk, needs, version, type, chain));
        }

        private static int[] GetNeededMoves(PKM pk, IEnumerable<int> moves, IReadOnlyList<EvoCriteria> chain)
        {
            if (pk.Species == (int)Species.Smeargle)
                return moves.Where(z => !Legal.IsValidSketch(z, pk.Format)).ToArray(); // Can learn anything

            // Roughly determine the generation the PKM is originating from
            var ver = pk.Version;
            int origin = pk.Generation;
            if (origin < 0)
                origin = ((GameVersion)ver).GetGeneration();

            // Temporarily replace the Version for VC1 transfers, so that they can have VC2 moves if needed.
            bool vcBump = origin == 1 && pk.Format >= 7;
            if (vcBump)
                pk.Version = (int)GameVersion.C;

            var gens = GenerationTraversal.GetVisitedGenerationOrder(pk, origin);
            var canlearn = gens.SelectMany(z => GetMovesForGeneration(pk, chain, z));
            var result = moves.Except(canlearn).Where(z => z != 0).ToArray();

            if (vcBump)
                pk.Version = ver;

            return result;
        }

        private static IEnumerable<int> GetMovesForGeneration(PKM pk, IReadOnlyList<EvoCriteria> chain, int generation)
        {
            IEnumerable<int> moves = MoveList.GetValidMoves(pk, chain, generation);
            if (pk.Format >= 8)
            {
                // Shared Egg Moves via daycare
                // Any egg move can be obtained
                moves = moves.Concat(MoveEgg.GetSharedEggMoves(pk, generation));

                // TR moves -- default logic checks the TR flags, so we need to add all possible ones here.
                moves = moves.Concat(MoveTechnicalMachine.GetAllPossibleRecords(pk.Species, pk.Form));
            }
            if (pk.Species == (int)Species.Shedinja)
            {
                // Leveling up Nincada in Gen3/4 levels up, evolves to Ninjask, applies moves for Ninjask, then spawns Shedinja with the current moveset.
                // Future games spawn the Shedinja before doing Ninjask moves, so this is a special case.
                // Can't get more than the evolved-at level move; >=2 special moves will get caught by the legality checker later.
                return generation switch
                {
                    3 => moves.Concat(Legal.LevelUpE [(int)Species.Ninjask].GetMoves(100, 20)),
                    4 => moves.Concat(Legal.LevelUpPt[(int)Species.Ninjask].GetMoves(100, 20)),
                    _ => moves
                };
            }
            return moves;
        }

        private static IEnumerable<IEncounterable> GetPossibleOfType(PKM pk, IReadOnlyList<int> needs, GameVersion version, EncounterOrder type, IReadOnlyList<EvoCriteria> chain)
        {
            return type switch
            {
                EncounterOrder.Egg => GetEggs(pk, needs, chain, version),
                EncounterOrder.Mystery => GetGifts(pk, needs, chain),
                EncounterOrder.Static => GetStatic(pk, needs, chain, version),
                EncounterOrder.Trade => GetTrades(pk, needs, chain, version),
                EncounterOrder.Slot => GetSlots(pk, needs, chain, version),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <param name="chain">Origin possible evolution chain</param>
        /// <param name="version">Specific version to iterate for. Necessary for retrieving possible Egg Moves.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<EncounterEgg> GetEggs(PKM pk, IReadOnlyCollection<int> needs, IReadOnlyList<EvoCriteria> chain, GameVersion version)
        {
            if (GameVersion.CXD.Contains(version) || GameVersion.Gen7b.Contains(version))
                yield break; // no eggs from these games
            int gen = version.GetGeneration();
            if (gen < 2)
                yield break;
            var eggs = gen == 2
                ? EncounterEggGenerator2.GenerateEggs(pk, chain, all: true)
                : EncounterEggGenerator.GenerateEggs(pk, chain, gen, all: true);
            foreach (var egg in eggs)
            {
                if (needs.Count == 0)
                {
                    yield return egg;
                    continue;
                }

                IEnumerable<int> em = MoveEgg.GetEggMoves(pk.PersonalInfo, egg.Species, egg.Form, egg.Version, egg.Generation);
                if (egg.Generation <= 2)
                    em = em.Concat(MoveLevelUp.GetEncounterMoves(egg.Species, 0, egg.Level, egg.Version));
                else if (Legal.LightBall.Contains(egg.Species) && needs.Contains((int)Move.VoltTackle))
                    em = em.Concat(new[] { (int)Move.VoltTackle });

                if (!needs.Except(em).Any())
                    yield return egg;
            }
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <param name="chain">Origin possible evolution chain</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<MysteryGift> GetGifts(PKM pk, IReadOnlyCollection<int> needs, IReadOnlyList<EvoCriteria> chain)
        {
            var gifts = MysteryGiftGenerator.GetPossible(pk, chain);
            foreach (var gift in gifts)
            {
                if (gift is WC3 {NotDistributed: true})
                    continue;
                if (!IsSane(chain, gift))
                    continue;
                if (needs.Count == 0)
                {
                    yield return gift;
                    continue;
                }
                var em = gift.Moves;
                if (!needs.Except(em).Any())
                    yield return gift;
            }
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <param name="chain">Origin possible evolution chain</param>
        /// <param name="version">Specific version to iterate for.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<EncounterStatic> GetStatic(PKM pk, IReadOnlyCollection<int> needs, IReadOnlyList<EvoCriteria> chain, GameVersion version)
        {
            var encounters = EncounterStaticGenerator.GetPossible(pk, chain, version);
            foreach (var enc in encounters)
            {
                if (!IsSane(chain, enc))
                    continue;
                if (enc.IsUnobtainable())
                    continue;
                if (needs.Count == 0)
                {
                    yield return enc;
                    continue;
                }

                // Some rare encounters have special moves hidden in the Relearn section (Gen7 Wormhole Ho-Oh). Include relearn moves
                IEnumerable<int> em = enc.Moves;
                if (enc is IRelearn r)
                    em = em.Concat(r.Relearn);
                if (enc.Generation <= 2)
                    em = em.Concat(MoveLevelUp.GetEncounterMoves(enc.Species, 0, enc.Level, enc.Version));

                if (!needs.Except(em).Any())
                    yield return enc;
            }

            int gen = version.GetGeneration();
            if ((uint)gen >= 3)
                yield break;

            var gifts = EncounterStaticGenerator.GetPossibleGBGifts(chain, version);
            foreach (var enc in gifts)
            {
                if (needs.Count == 0)
                {
                    yield return enc;
                    continue;
                }

                var em = enc.Moves;
                if (!needs.Except(em).Any())
                    yield return enc;
            }
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <param name="chain">Origin possible evolution chain</param>
        /// <param name="version">Specific version to iterate for.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<EncounterTrade> GetTrades(PKM pk, IReadOnlyCollection<int> needs, IReadOnlyList<EvoCriteria> chain, GameVersion version)
        {
            var trades = EncounterTradeGenerator.GetPossible(pk, chain, version);
            foreach (var trade in trades)
            {
                if (!IsSane(chain, trade))
                    continue;
                if (needs.Count == 0)
                {
                    yield return trade;
                    continue;
                }
                IEnumerable<int> em = trade.Moves;
                if (trade.Generation <= 2)
                    em = em.Concat(MoveLevelUp.GetEncounterMoves(trade.Species, 0, trade.Level, trade.Version));
                if (!needs.Except(em).Any())
                    yield return trade;
            }
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <param name="chain">Origin possible evolution chain</param>
        /// <param name="version">Origin version</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<EncounterSlot> GetSlots(PKM pk, IReadOnlyList<int> needs, IReadOnlyList<EvoCriteria> chain, GameVersion version)
        {
            var slots = EncounterSlotGenerator.GetPossible(pk, chain, version);
            foreach (var slot in slots)
            {
                if (!IsSane(chain, slot))
                    continue;
                if (slot.IsUnobtainable())
                    continue;

                if (needs.Count == 0)
                {
                    yield return slot;
                    continue;
                }

                if (slot is IMoveset m && !needs.Except(m.Moves).Any())
                    yield return slot;
                else if (needs.Count == 1 && slot is EncounterSlot6AO {CanDexNav: true} dn && dn.CanBeDexNavMove(needs[0]))
                    yield return slot;
                else if (slot.Generation <= 2 && !needs.Except(MoveLevelUp.GetEncounterMoves(slot.Species, 0, slot.LevelMin, slot.Version)).Any())
                    yield return slot;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSane(IReadOnlyList<EvoCriteria> chain, IEncounterTemplate enc)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (evo.Form == enc.Form)
                    return true;
                if (FormInfo.IsFormChangeable(enc.Species, enc.Form, evo.Form, enc.Generation))
                    return true;
                break;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUnobtainable(this EncounterSlot slot)
        {
            switch (slot.Generation)
            {
                case 4:
                    if (slot.Location == 193 && slot.Area.Type == SlotType.Surf) // Johto Route 45 surfing encounter. Unreachable Water tiles.
                        return true;
                    break;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUnobtainable(this EncounterStatic enc)
        {
            if (enc is not EncounterStatic4 s)
                return false;

            return s.Species switch
            {
                (int)Species.Darkrai when s.Version != GameVersion.Pt => true, // DP Darkrai
                (int)Species.Shaymin when s.Version != GameVersion.Pt => true, // DP Shaymin
                (int)Species.Arceus => true, // Azure Flute Arceus
                _ => false
            };
        }
    }
}
