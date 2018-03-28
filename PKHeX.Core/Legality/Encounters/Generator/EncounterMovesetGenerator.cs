using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generates weakly matched <see cref="IEncounterable"/> objects for an input <see cref="PKM"/> (and/or criteria).
    /// </summary>
    public static class EncounterMovesetGenerator
    {
        /// <summary>
        /// List of possible <see cref="GameVersion"/> values a <see cref="PKM.Version"/> can have.
        /// </summary>
        private static readonly GameVersion[] Versions = ((GameVersion[]) Enum.GetValues(typeof(GameVersion))).Where(z => z < GameVersion.RB && z > 0).Reverse().ToArray();

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned.
        /// </summary>
        /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
        /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, int[] moves = null, params GameVersion[] versions)
        {
            return (versions ?? Versions).SelectMany(ver => GenerateVersionEncounters(pk, moves ?? pk.Moves, ver));
        }

        /// <summary>
        /// Gets possible encounters that allow all moves requested to be learned, restricted to the maximum for the current format.
        /// </summary>
        /// <param name="pk">Complete Pokémon data which contains the requested species, gender, and form.</param>
        /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn. If left blank, the current moves will be used.</param>
        /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
        public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, int[] moves = null)
        {
            var vers = Versions.Where(z => z <= (GameVersion)pk.MaxGameID).ToArray();
            return GenerateEncounters(pk, moves ?? pk.Moves, vers);
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
            var dl = et.GetValidPreEvolutions(pk, maxLevel: 100, skipChecks: true).ToArray();

            var gens = VerifyCurrentMoves.GetGenMovesCheckOrder(pk);
            var canlearn = gens.SelectMany(z => Legal.GetValidMoves(pk, dl, z));
            var needs = moves.Except(canlearn).ToArray();

            foreach (var enc in GetPossible(pk, needs, version))
                yield return enc;
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
            foreach (var egg in eggs)
                yield return egg;

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

                var em = Legal.GetEggMoves(pk, egg.Species, pk.AltForm, version);
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
                if (needs.Count == 0)
                {
                    yield return slot;
                    continue;
                }

                if (slot is IMoveset m && needs.Except(m.Moves).Any())
                    yield return slot;
            }
        }
    }
}
