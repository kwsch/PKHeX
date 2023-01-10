using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator9 : IEncounterGenerator
{
    public static readonly EncounterGenerator9 Instance = new();

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G9;
            foreach (var enc in GetPossibleEvents(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            var eggs = GetEggs(pk, chain, game);
            foreach (var enc in eggs)
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = game == GameVersion.SL ? Encounters9.StaticSL : Encounters9.StaticVL;
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = Encounters9.Slots;
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = Encounters9.TradeGift_SV;
            foreach (var enc in GetPossibleTrade(chain, table, game))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleEvents(EvoCriteria[] chain, IReadOnlyList<WC9> table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic[] table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea9[] areas)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;
                    yield return slot;
                    break;
                }
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleTrade(EvoCriteria[] chain, EncounterTrade9[] table, GameVersion game)
    {
        foreach (var enc in table)
        {
            if (enc.Version != GameVersion.SV && enc.Version != game)
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (pk.FatefulEncounter)
        {
            bool yielded = false;
            foreach (var mg in EncounterEvent.MGDB_G9)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != mg.Species)
                        continue;

                    if (mg.IsMatchExact(pk, evo))
                    {
                        yield return mg;
                        yielded = true;
                    }
                    break;
                }
            }
            if (yielded)
                yield break;
        }

        var game = (GameVersion)pk.Version;

        // While an unhatched picnic egg, the Version remains 0.
        if (Locations.IsEggLocationBred9(pk.Egg_Location) && !(pk.IsEgg && game != 0))
        {
            bool yielded = false;
            var eggs = GetEggs(pk, chain, game);
            foreach (var egg in eggs)
            {
                yield return egg;
                yielded = true;
            }
            if (yielded)
                yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = MaxNotMatch;

        // Trades
        if (pk.Met_Location == Locations.LinkTrade6NPC)
        {
            foreach (var z in Encounters9.TradeGift_SV)
            {
                foreach (var evo in chain)
                {
                    if (z.Version != GameVersion.SV && z.Version != game)
                        continue;
                    if (evo.Species != z.Species)
                        continue;
                    if (!z.IsMatchExact(pk, evo))
                        break;

                    var match = z.GetMatchRating(pk);
                    if (match == Match)
                    {
                        yield return z;
                    }
                    else if (match < rating)
                    {
                        cache = z;
                        rating = match;
                    }
                    break;
                }
            }
            if (cache != null)
                yield return cache;
            yield break;
        }

        if (pk is not IRibbonIndex r || !r.HasEncounterMark())
        {
            var encStatic = game == GameVersion.SL ? Encounters9.StaticSL : Encounters9.StaticVL;
            foreach (var z in encStatic)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != z.Species)
                        continue;
                    if (!z.IsMatchExact(pk, evo))
                        break;

                    var match = z.GetMatchRating(pk);
                    if (match == Match)
                    {
                        yield return z;
                    }
                    else if (match < rating)
                    {
                        cache = z;
                        rating = match;
                    }
                    break;
                }
            }
        }

        // Wild encounters are more permissive than static encounters.
        // Can have encounter marks, can have varied scales/shiny states.
        if (CanBeWildEncounter(pk))
        {
            var location = pk.Met_Location;
            var areas = Encounters9.Slots;
            foreach (var area in areas)
            {
                if (!area.IsMatchLocation(location))
                    continue;

                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var slot in slots)
                {
                    var match = slot.GetMatchRating(pk);
                    if (match == Match)
                    {
                        yield return slot;
                    }
                    else if (match < rating)
                    {
                        cache = slot;
                        rating = match;
                    }
                }
            }
        }

        if (cache != null)
            yield return cache;
    }

    private const int Generation = 9;
    private const EntityContext Context = EntityContext.Gen9;
    private const byte EggLevel = 1;

    private static IEnumerable<EncounterEgg> GetEggs(PKM pk, EvoCriteria[] chain, GameVersion version)
    {
        var devolved = chain[^1];
        if (version == 0 && pk.IsEgg)
            version = GameVersion.SL;

        // Ensure most devolved species is the same as the egg species.
        // No split-breed to consider.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species)
            yield break; // no split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.SV.IsPresentInGame(species, form))
            yield break;

        yield return CreateEggEncounter(species, form, version);
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves9.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }
}
