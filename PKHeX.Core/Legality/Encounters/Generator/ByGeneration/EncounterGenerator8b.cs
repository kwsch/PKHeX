using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator8b : IEncounterGenerator
{
    public static readonly EncounterGenerator8b Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G8B;
            foreach (var enc in GetPossibleGifts(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            var eggs = GetEggs(chain, game);
            foreach (var enc in eggs)
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = game == GameVersion.BD ? Encounters8b.StaticBD : Encounters8b.StaticSP;
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = game == GameVersion.BD ? Encounters8b.SlotsBD : Encounters8b.SlotsSP;
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            foreach (var enc in GetPossibleTrades(chain, game))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<WB8> table)
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

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic8b[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea8b[] areas)
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

    private static IEnumerable<IEncounterable> GetPossibleTrades(EvoCriteria[] chain, GameVersion game)
    {
        var table = Encounters8b.TradeGift_BDSP;
        foreach (var enc in table)
        {
            if (enc.Version != GameVersion.BDSP && enc.Version != game)
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
        if (pk is PK8)
            yield break;

        bool yielded = false;
        var game = (GameVersion)pk.Version;

        if (pk.FatefulEncounter)
        {
            foreach (var enc in EncounterEvent.MGDB_G8B)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;
                    yield return enc;
                    yielded = true;
                    break;
                }
            }
            if (yielded)
                yield break;
        }

        if (Locations.IsEggLocationBred8b(pk.Egg_Location))
        {
            var egg = GetEggs(chain, game);
            foreach (var enc in egg)
            {
                yield return enc;
                yielded = true;
            }
            if (yielded)
                yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = MaxNotMatch;

        // Trades
        if (pk is { IsEgg: false, Met_Location: Locations.LinkTrade6NPC })
        {
            foreach (var enc in GetEncountersTrade(pk, chain, game))
                yield return enc;
            yield break;
        }

        // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
        var encStatic = game == GameVersion.BD ? Encounters8b.StaticBD : Encounters8b.StaticSP;
        foreach (var enc in encStatic)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match == Match)
                {
                    yield return enc;
                }
                else if (match < rating)
                {
                    cache = enc;
                    rating = match;
                }
                break;
            }
        }

        if (CanBeWildEncounter(pk))
        {
            bool hasOriginalLocation = pk is not PK8;
            var location = pk.Met_Location;
            var encWild = game == GameVersion.BD ? Encounters8b.SlotsBD : Encounters8b.SlotsSP;
            foreach (var area in encWild)
            {
                if (hasOriginalLocation && !area.IsMatchLocation(location))
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

    private static IEnumerable<IEncounterable> GetEncountersTrade(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        bool yielded = false;
        EncounterMatchRating rating = MaxNotMatch;
        IEncounterable? cache = null;
        foreach (var enc in Encounters8b.TradeGift_BDSP)
        {
            if (enc.Version != GameVersion.BDSP && enc.Version != game)
                continue;
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match == Match)
                {
                    yield return enc;
                    yielded = true;
                }
                else if (match < rating)
                {
                    cache = enc;
                    rating = match;
                }

                break;
            }
        }

        if (yielded)
            yield break;
        if (cache != null)
            yield return cache;
    }

    public IEnumerable<IEncounterable> GetEncountersSWSH(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        bool yielded = false;
        if (pk.FatefulEncounter)
        {
            foreach (var enc in EncounterEvent.MGDB_G8B)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;
                    yield return enc;
                    yielded = true;
                    break;
                }
            }
            if (yielded)
                yield break;
        }

        bool wasEgg = pk.Egg_Location switch
        {
            Locations.HOME_SWSHBDSPEgg => true, // Regular hatch location (not link trade)
            Locations.HOME_SWBD => pk.Met_Location == Locations.HOME_SWBD, // Link Trade transferred over must match Met Location
            Locations.HOME_SHSP => pk.Met_Location == Locations.HOME_SHSP, // Link Trade transferred over must match Met Location
            _ => false,
        };
        if (wasEgg && pk.Met_Level == 1)
        {
            var egg = GetEggs(chain, game);
            foreach (var enc in egg)
            {
                yield return enc;
                yielded = true;
            }
            if (yielded)
                yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = MaxNotMatch;

        // Trades
        if (!pk.IsEgg)
        {
            foreach (var enc in Encounters8b.TradeGift_BDSP)
            {
                if (enc.Version != GameVersion.BDSP && enc.Version != game)
                    continue;
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;

                    var match = enc.GetMatchRating(pk);
                    if (match == Match)
                    {
                        yield return enc;
                    }
                    else if (match < rating)
                    {
                        cache = enc;
                        rating = match;
                    }
                    break;
                }
            }
        }

        // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
        var encStatic = game == GameVersion.BD ? Encounters8b.StaticBD : Encounters8b.StaticSP;
        foreach (var enc in encStatic)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match == Match)
                {
                    yield return enc;
                }
                else if (match < rating)
                {
                    cache = enc;
                    rating = match;
                }
                break;
            }
        }

        // Only yield if Safari and Marsh encounters match.
        bool safari = pk is PK8 { Ball: (int)Ball.Safari };
        var encWild = game == GameVersion.BD ? Encounters8b.SlotsBD : Encounters8b.SlotsSP;
        foreach (var area in encWild)
        {
            var slots = area.GetMatchingSlots(pk, chain);
            foreach (var slot in slots)
            {
                var match = slot.GetMatchRating(pk);
                var marsh = Locations.IsSafariZoneLocation8b(area.Location);
                if (safari != marsh)
                    match = DeferredErrors;
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

        if (cache != null)
            yield return cache;
    }

    private const int Generation = 8;
    private const EntityContext Context = EntityContext.Gen8b;
    private const byte EggLevel = 1;

    private static IEnumerable<EncounterEgg> GetEggs(EvoCriteria[] chain, GameVersion version)
    {
        var devolved = chain[^1];

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !IsValidBabySpecies(devolved.Species))
            yield break; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.BDSP.IsPresentInGame(species, form))
            yield break;

        yield return CreateEggEncounter(species, form, version);

        // Check for split-breed
        if (species == devolved.Species)
        {
            if (chain.Length < 2)
                yield break; // no split-breed
            devolved = chain[^2];
        }
        var splitSet = Breeding.GetSplitBreedGeneration(Generation);
        if (splitSet is null)
            yield break; // Shouldn't happen.
        if (!splitSet.Contains(devolved.Species))
            yield break;

        yield return CreateEggEncounter(devolved.Species, devolved.Form, version);
    }

    private static bool IsValidBabySpecies(ushort species)
    {
        var split = Breeding.GetSplitBreedGeneration(Generation);
        return split is not null && split.Contains(species);
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves8b.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }
}
