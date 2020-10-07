using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// This class is essentially a sub-class of <see cref="EncounterGenerator"/> specialized for Gen1 & Gen2 encounters.
    /// </summary>
    internal static class EncounterGenerator12
    {
        internal static IEnumerable<IEncounterable> GetEncounters12(PKM pkm, LegalInfo info)
        {
            foreach (var z in GenerateFilteredEncounters12(pkm))
            {
                info.Generation = z.Generation;
                info.Game = z.Version;
                yield return z;
            }
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters12(PKM pkm, GameVersion game)
        {
            // Since encounter matching is super weak due to limited stored data in the structure
            // Calculate all 3 at the same time and pick the best result (by species).
            // Favor special event move gifts as Static Encounters when applicable
            var chain = EncounterOrigin.GetOriginChain12(pkm, game);

            var deferred = new List<IEncounterable>();
            foreach (var t in EncounterTradeGenerator.GetValidEncounterTrades(pkm, chain, game))
            {
                // some OTs are longer than the keyboard entry; don't defer these
                if (pkm.Format >= 7 && pkm.OT_Name.Length <= (pkm.Japanese || pkm.Korean ? 5 : 7))
                {
                    deferred.Add(t);
                    continue;
                }
                if (t is EncounterTrade1 t1 && !t1.IsEncounterTrade1Valid(pkm))
                {
                    deferred.Add(t);
                    continue;
                }
                yield return t;
            }
            foreach (var s in EncounterStaticGenerator.GetValidStaticEncounter(pkm, chain, game))
            {
                yield return s;
            }
            foreach (var e in EncounterSlotGenerator.GetValidWildEncounters12(pkm, chain, game))
            {
                yield return e;
            }
            if (game != GameVersion.RBY)
            {
                foreach (var e in EncounterEggGenerator2.GenerateEggs(pkm, chain))
                    yield return e;
            }

            foreach (var s in GenerateGBEvents(pkm, chain, game))
            {
                yield return s;
            }

            foreach (var d in deferred)
                yield return d;
        }

        private static IEnumerable<EncounterStatic> GenerateGBEvents(PKM pkm, IReadOnlyList<EvoCriteria> chain, GameVersion game)
        {
            if (pkm.Korean) // only GS; no events
                yield break;

            foreach (var e in EncounterStaticGenerator.GetValidGBGifts(pkm, chain, game))
            {
                foreach (var evo in chain)
                {
                    if (e.IsMatch(pkm, evo))
                        yield return e;
                }
            }
        }

        private static IEnumerable<IEncounterable> GenerateFilteredEncounters12(PKM pkm)
        {
            // If the current data indicates that it must have originated from Crystal, only yield encounter data from Crystal.
            bool crystal = (pkm is ICaughtData2 pk2 && pk2.CaughtData != 0) || (pkm.Format >= 7 && pkm.OT_Gender == 1);
            if (crystal)
                return GenerateRawEncounters12(pkm, GameVersion.C);

            var visited = GBRestrictions.GetTradebackStatusInitial(pkm);
            switch (visited)
            {
                case TradebackType.Gen1_NotTradeback:
                    return GenerateRawEncounters12(pkm, GameVersion.RBY);
                case TradebackType.Gen2_NotTradeback:
                    return GenerateRawEncounters12(pkm, GameVersion.GSC);
                default:
                    if (pkm.Korean)
                        return GenerateFilteredEncounters12BothKorean(pkm);
                    return GenerateFilteredEncounters12Both(pkm);
            }
        }

        private static IEnumerable<IEncounterable> GenerateFilteredEncounters12BothKorean(PKM pkm)
        {
            // Korean origin PK1/PK2 can only originate from GS, but since we're nice we'll defer & yield matches from other games.
            // Yield GS first, then Crystal, then RBY. Anything other than GS will be flagged by later checks.

            var deferred = new List<IEncounterable>();
            var get2 = GenerateRawEncounters12(pkm, GameVersion.GSC);
            foreach (var enc in get2)
            {
                if (enc.Version == GameVersion.C)
                    deferred.Add(enc);
                else
                    yield return enc;
            }

            foreach (var enc in deferred)
                yield return enc;

            var get1 = GenerateRawEncounters12(pkm, GameVersion.RBY);
            foreach (var enc in get1)
                yield return enc;
        }

        private static IEnumerable<IEncounterable> GenerateFilteredEncounters12Both(PKM pkm)
        {
            // Iterate over both games, consuming from one list at a time until the other list has higher priority encounters
            // Buffer the encounters so that we can consume each iterator separately
            var get1 = GenerateRawEncounters12(pkm, GameVersion.RBY);
            var get2 = GenerateRawEncounters12(pkm, GameVersion.GSC);
            using var g1i = new PeekEnumerator<IEncounterable>(get1);
            using var g2i = new PeekEnumerator<IEncounterable>(get2);
            while (g2i.PeekIsNext() || g1i.PeekIsNext())
            {
                var iter = PickPreferredIterator(pkm, g1i, g2i);
                yield return iter.Current;
                iter.MoveNext();
            }
        }

        private static PeekEnumerator<IEncounterable> PickPreferredIterator(PKM pkm, PeekEnumerator<IEncounterable> g1i, PeekEnumerator<IEncounterable> g2i)
        {
            if (!g1i.PeekIsNext())
                return g2i;
            if (!g2i.PeekIsNext())
                return g1i;
            var p1 = GetGBEncounterPriority(pkm, g1i.Current);
            var p2 = GetGBEncounterPriority(pkm, g2i.Current);
            return p1 > p2 ? g1i : g2i;
        }

        private static GBEncounterPriority GetGBEncounterPriority(PKM pkm, IEncounterable Encounter)
        {
            switch (Encounter)
            {
                case EncounterTrade1 t1:
                    return t1.IsEncounterTrade1Valid(pkm) ? GBEncounterPriority.TradeEncounterG1 : GBEncounterPriority.Least;
                case EncounterTrade2 _:
                    return GBEncounterPriority.TradeEncounterG2;
                case EncounterStatic s:
                    if (s.Moves.Count != 0 && s.Moves[0] != 0 && pkm.HasMove(s.Moves[0]))
                        return GBEncounterPriority.SpecialEncounter;
                    return GBEncounterPriority.StaticEncounter;
                case EncounterSlot _:
                    return GBEncounterPriority.WildEncounter;

                default:
                    return GBEncounterPriority.EggEncounter;
            }
        }

        /// <summary>
        /// Generation 1/2 Encounter Data type, which serves as a 'best match' priority rating when returning from a list.
        /// </summary>
        private enum GBEncounterPriority
        {
            Least,
            EggEncounter,
            WildEncounter,
            StaticEncounter,
            SpecialEncounter,
            TradeEncounterG1,
            TradeEncounterG2,
        }
    }
}
