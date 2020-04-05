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
        public static IEnumerable<PKM> GeneratePKMs(PKM pk, ITrainerInfo info, int[]? moves = null, params GameVersion[] versions)
        {
            pk.TID = info.TID;
            var m = moves ?? pk.Moves;
            var vers = versions.Length >= 1 ? versions : GameUtil.GetVersionsWithinRange(pk, pk.Format);
            foreach (var ver in vers)
            {
                var encs = GenerateVersionEncounters(pk, m, ver);
                foreach (var enc in encs)
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
            var et = EvolutionTree.GetEvolutionTree(pk, pk.Species <= Legal.MaxSpeciesID_7_USUM ? 7 : PKX.Generation); // temp workaround as G8 doesn't have all evolutions
            var dl = et.GetValidPreEvolutions(pk, maxLevel: 100, skipChecks: true);
            int[] needs = GetNeededMoves(pk, moves, dl);

            return PriorityList.SelectMany(type => GetPossibleOfType(pk, needs, version, type));
        }

        private static int[] GetNeededMoves(PKM pk, IEnumerable<int> moves, IReadOnlyList<EvoCriteria> dl)
        {
            if (pk.Species == (int)Species.Smeargle)
                return moves.Intersect(Legal.InvalidSketch).ToArray(); // Can learn anything

            var gens = VerifyCurrentMoves.GetGenMovesCheckOrder(pk);
            var canlearn = gens.SelectMany(z => GetMovesForGeneration(pk, dl, z));
            return moves.Except(canlearn).ToArray();
        }

        private static IEnumerable<int> GetMovesForGeneration(PKM pk, IReadOnlyList<EvoCriteria> dl, int generation)
        {
            IEnumerable<int> moves = Legal.GetValidMoves(pk, dl, generation);
            if (pk.Format >= 8)
            {
                // Shared Egg Moves via daycare
                // Any egg move can be obtained
                var evo = dl[dl.Count - 1];
                var shared = MoveEgg.GetEggMoves(8, evo.Species, evo.Form, GameVersion.SW);
                return moves.Concat(shared);
            }
            if (dl[0].Species == (int)Species.Shedinja)
            {
                // Leveling up Nincada in Gen3/4 levels up, evolves to Ninjask, applies moves for Ninjask, then spawns Shedinja with the current moveset.
                // Future games spawn the Shedinja before doing Ninjask moves, so this is a special case.
                // Can't get more than the evolved-at level move; >=2 special moves will get caught by the legality checker later.
                if (generation == 3)
                    return moves.Concat(Legal.LevelUpE[(int)Species.Ninjask].GetMoves(100, 20));
                if (generation == 4)
                    return moves.Concat(Legal.LevelUpPt[(int)Species.Ninjask].GetMoves(100, 20));
            }
            return moves;
        }

        private static IEnumerable<IEncounterable> GetPossibleOfType(PKM pk, IReadOnlyCollection<int> needs, GameVersion version, EncounterOrder type)
        {
            return type switch
            {
                EncounterOrder.Egg => GetEggs(pk, needs, version),
                EncounterOrder.Mystery => GetGifts(pk, needs),
                EncounterOrder.Static => GetStatic(pk, needs),
                EncounterOrder.Trade => GetTrades(pk, needs),
                EncounterOrder.Slot => GetSlots(pk, needs),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <param name="version">Specific version to iterate for. Necessary for retrieving possible Egg Moves.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<EncounterEgg> GetEggs(PKM pk, IReadOnlyCollection<int> needs, GameVersion version)
        {
            if (GameVersion.CXD.Contains(version) || GameVersion.GG.Contains(version))
                yield break; // no eggs from these games

            var eggs = EncounterEggGenerator.GenerateEggs(pk, all: true);
            foreach (var egg in eggs)
            {
                if (needs.Count == 0)
                {
                    yield return egg;
                    continue;
                }

                IEnumerable<int> em = MoveEgg.GetEggMoves(pk, egg.Species, egg.Form, version);
                if (Legal.LightBall.Contains(egg.Species) && needs.Contains(344))
                    em = em.Concat(new[] {344}); // Volt Tackle
                if (!needs.Except(em).Any())
                    yield return egg;
            }
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<MysteryGift> GetGifts(PKM pk, IReadOnlyCollection<int> needs)
        {
            var gifts = MysteryGiftGenerator.GetPossible(pk);
            foreach (var gift in gifts)
            {
                if (gift is WC3 wc3 && wc3.NotDistributed)
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
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<EncounterStatic> GetStatic(PKM pk, IReadOnlyCollection<int> needs)
        {
            var encs = EncounterStaticGenerator.GetPossible(pk);
            foreach (var enc in encs)
            {
                if (enc.IsUnobtainable(pk))
                    continue;
                if (needs.Count == 0)
                {
                    yield return enc;
                    continue;
                }

                // Some rare encounters have special moves hidden in the Relearn section (Gen7 Wormhole Ho-Oh). Include relearn moves
                var em = enc.Moves.Concat(enc.Relearn);
                if (!needs.Except(em).Any())
                    yield return enc;
            }
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<EncounterTrade> GetTrades(PKM pk, IReadOnlyCollection<int> needs)
        {
            var trades = EncounterTradeGenerator.GetPossible(pk);
            foreach (var trade in trades)
            {
                if (needs.Count == 0)
                {
                    yield return trade;
                    continue;
                }
                var em = trade.Moves;
                if (!needs.Except(em).Any())
                    yield return trade;
            }
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<EncounterSlot> GetSlots(PKM pk, IReadOnlyCollection<int> needs)
        {
            var slots = EncounterSlotGenerator.GetPossible(pk);
            foreach (var slot in slots)
            {
                if (slot.IsUnobtainable(pk))
                    continue;

                if (needs.Count == 0)
                {
                    yield return slot;
                    continue;
                }

                if (slot is IMoveset m && needs.Except(m.Moves).Any())
                    yield return slot;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUnobtainable(this EncounterSlot slot, ITrainerID pk)
        {
            switch (slot.Generation)
            {
                case 2:
                    if ((slot.Type & SlotType.Safari) != 0) // Safari Zone is unavailable in Gen 2.
                        return true;

                    if ((slot.Type & SlotType.Headbutt) != 0) // Unreachable Headbutt Trees.
                        return Encounters2.GetGSCHeadbuttAvailability(slot, pk.TID) != TreeEncounterAvailable.ValidTree;
                    break;
                case 4:
                    if (slot.Location == 193 && slot.Type == SlotType.Surf) // Johto Route 45 surfing encounter. Unreachable Water tiles.
                        return true;
                    break;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUnobtainable(this EncounterStatic enc, PKM pk)
        {
            switch (enc.Generation)
            {
                case 4 when enc is EncounterStaticTyped t && enc.Location == 193:
                    if (t.TypeEncounter == EncounterType.Surfing_Fishing) // Johto Route 45 surfing encounter. Unreachable Water tiles.
                        return true; // only hits for Roamer Raikou
                    break;
                case 4:
                    switch (pk.Species)
                    {
                        case (int)Species.Darkrai when enc.Location == 079 && !pk.Pt: // DP Darkrai
                            return true;
                        case (int)Species.Shaymin when enc.Location == 063 && !pk.Pt: // DP Shaymin
                            return true;
                        case (int)Species.Arceus when enc.Location == 086: // Azure Flute Arceus
                            return true;
                    }
                    break;
            }

            return false;
        }
    }
}
