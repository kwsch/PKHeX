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
        /// Gets possible <see cref="PKM"/> objects that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="info">Trainer information of the receiver.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
        /// <returns>A consumable <see cref="PKM"/> list of possible results.</returns>
        public static IEnumerable<PKM> GeneratePKMs(PKM pk, ITrainerInfo info, int[] moves = null, params GameVersion[] versions)
        {
            pk.TID = info.TID;
            var m = moves ?? pk.Moves;
            var vers = versions?.Length >= 1 ? versions : GameUtil.GetVersionsWithinRange(pk, pk.Format);
            foreach (var ver in vers)
            {
                var encs = GenerateVersionEncounters(pk, m, ver);
                foreach (var enc in encs)
                {
                    var result = enc.ConvertToPKM(info);
#if DEBUG
                    var la = new LegalityAnalysis(result);
                    if (!la.Valid)
                        throw new Exception();
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
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="generation">Specific generation to iterate versions for.</param>
        public static IEnumerable<PKM> GeneratePKMs(PKM pk, ITrainerInfo info, int generation, int[] moves = null)
        {
            var vers = GameUtil.GetVersionsInGeneration(generation);
            return GeneratePKMs(pk, info, moves, vers);
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="generation">Specific generation to iterate versions for.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateEncounter(PKM pk, int generation, int[] moves = null)
        {
            var vers = GameUtil.GetVersionsInGeneration(generation);
            return GenerateEncounters(pk, moves, vers);
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, int[] moves = null, params GameVersion[] versions)
        {
            var m = moves ?? pk.Moves;
            var vers = versions?.Length >= 1 ? versions : GameUtil.GetVersionsWithinRange(pk, pk.Format);
            return vers.SelectMany(ver => GenerateVersionEncounters(pk, m, ver));
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned, restricted to the maximum for the current format.
        /// </summary>
        /// <param name="pk">Complete Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn. If left blank, the current moves will be used.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, int[] moves = null)
        {
            return GenerateEncounters(pk, moves ?? pk.Moves, null);
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
            var et = EvolutionTree.GetEvolutionTree(PKX.Generation);
            var dl = et.GetValidPreEvolutions(pk, maxLevel: 100, skipChecks: true);
            int[] needs = GetNeededMoves(pk, moves, dl);

            foreach (var enc in GetPossible(pk, needs, version))
                yield return enc;
        }

        private static int[] GetNeededMoves(PKM pk, IEnumerable<int> moves, IReadOnlyList<EvoCriteria> dl)
        {
            if (pk.Species == 235) // Smeargle
                return moves.Intersect(Legal.InvalidSketch).ToArray(); // Can learn anything

            var gens = VerifyCurrentMoves.GetGenMovesCheckOrder(pk);
            var canlearn = gens.SelectMany(z => Legal.GetValidMoves(pk, dl, z));
            return moves.Except(canlearn).ToArray();
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="needs">Moves which cannot be taught by the player.</param>
        /// <param name="version">Specific version to iterate for. Necessary for retrieving possible Egg Moves.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        private static IEnumerable<IEncounterable> GetPossible(PKM pk, IReadOnlyCollection<int> needs, GameVersion version)
        {
            // generate possible eggs
            var eggs = GetEggs(pk, needs, version);
            if (!GameVersion.CXD.Contains(version))
            {
                foreach (var egg in eggs)
                    yield return egg;
            }

            // mystery gifts next
            var gifts = GetGifts(pk, needs);
            foreach (var gift in gifts)
                yield return gift;

            // link stuff
            var links = GetLink(pk, needs);
            foreach (var link in links)
                yield return link;

            // static encounters last
            var statics = GetStatic(pk, needs);
            foreach (var enc in statics)
                yield return enc;

            // trades for kicks
            var trades = GetTrades(pk, needs);
            foreach (var trade in trades)
                yield return trade;

            // why not slots
            var slots = GetSlots(pk, needs);
            foreach (var slot in slots)
                yield return slot;
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
            var eggs = EncounterEggGenerator.GenerateEggs(pk, all: true);
            foreach (var egg in eggs)
            {
                if (needs.Count == 0)
                {
                    yield return egg;
                    continue;
                }

                IEnumerable<int> em = MoveEgg.GetEggMoves(pk, egg.Species, pk.AltForm, version);
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
        private static IEnumerable<EncounterLink> GetLink(PKM pk, IReadOnlyCollection<int> needs)
        {
            var gifts = EncounterLinkGenerator.GetPossible(pk);
            foreach (var gift in gifts)
            {
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

                var em = enc.Moves;
                if (em != null && !needs.Except(em).Any())
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
                if (em != null && !needs.Except(em).Any())
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
                        case 491 when enc.Location == 079 && !pk.Pt: // DP Darkrai
                            return true;
                        case 492 when enc.Location == 063 && !pk.Pt: // DP Shaymin
                            return true;
                        case 493 when enc.Location == 086: // Azure Flute Arceus
                            return true;
                    }
                    break;
            }

            return false;
        }
    }
}
