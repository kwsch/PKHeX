using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator8 : IEncounterGenerator
{
    public static readonly EncounterGenerator8 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G8;
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
            foreach (var enc in GetPossible(chain, Encounters8.StaticSWSH))
                yield return enc;
            var table = game == GameVersion.SW ? Encounters8.StaticSW : Encounters8.StaticSH;
            foreach (var enc in GetPossible(chain, table))
                yield return enc;
            foreach (var enc in GetPossible(chain, Encounters8Nest.Nest_SW))
                yield return enc;
            foreach (var enc in GetPossible(chain, Encounters8Nest.Nest_SH))
                yield return enc;
            foreach (var enc in GetPossible(chain, Encounters8Nest.Dist_SW))
                yield return enc;
            foreach (var enc in GetPossible(chain, Encounters8Nest.Dist_SH))
                yield return enc;
            foreach (var enc in GetPossible(chain, Encounters8Nest.DynAdv_SWSH))
                yield return enc;
            foreach (var enc in GetPossible(chain, Encounters8Nest.Crystal_SWSH))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = game == GameVersion.SW ? Encounters8.SlotsSW : Encounters8.SlotsSH;
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = Encounters8.TradeSWSH;
            foreach (var enc in GetPossible(chain, table))
                yield return enc;
            var specific = game == GameVersion.SW ? Encounters8.TradeSW : Encounters8.TradeSH;
            foreach (var enc in GetPossible(chain, specific))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<WC8> table)
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

    private static IEnumerable<T> GetPossible<T>(EvoCriteria[] chain, T[] table) where T : IEncounterTemplate
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

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea8[] areas)
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

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (chain.Length == 0)
            yield break;
        if (pk.FatefulEncounter)
        {
            bool yielded = false;
            foreach (var z in EncounterEvent.MGDB_G8)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != z.Species)
                        continue;
                    if (!z.IsMatchExact(pk, evo))
                        break;

                    yield return z;
                    yielded = true;
                    break;
                }
            }
            if (yielded)
                yield break;
        }

        var game = (GameVersion)pk.Version;
        if (Locations.IsEggLocationBred6(pk.Egg_Location))
        {
            bool yielded = false;
            var eggs = GetEggs(chain, game);
            foreach (var egg in eggs)
            { yield return egg; yielded = true; }
            if (yielded) yield break;
        }

        // Trades
        if (pk.Met_Location == Locations.LinkTrade6NPC)
        {
            foreach (var enc in GetEncountersTrade(pk, chain, game))
                yield return enc;
            yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = MaxNotMatch;

        // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
        foreach (var enc in Encounters8.StaticSWSH)
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
        var encStatic = game == GameVersion.SW ? Encounters8.StaticSW : Encounters8.StaticSH;
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
        foreach (var enc in Encounters8Nest.Nest_SW)
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
        foreach (var enc in Encounters8Nest.Nest_SH)
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
        foreach (var enc in Encounters8Nest.Dist_SW)
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
        foreach (var enc in Encounters8Nest.Dist_SH)
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
        foreach (var enc in Encounters8Nest.DynAdv_SWSH)
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
        foreach (var enc in Encounters8Nest.Crystal_SWSH)
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

        // Wild Encounters
        if (CanBeWildEncounter(pk))
        {
            var location = pk.Met_Location;
            var areas = game == GameVersion.SW ? Encounters8.SlotsSW : Encounters8.SlotsSH;
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
                    break;
                }
            }
        }

        if (cache != null)
            yield return cache;
    }

    private static IEnumerable<IEncounterable> GetEncountersTrade(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        EncounterMatchRating rating = MaxNotMatch;
        IEncounterable? cache = null;
        foreach (var z in Encounters8.TradeSWSH)
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
        var specific = game == GameVersion.SW ? Encounters8.TradeSW : Encounters8.TradeSH;
        foreach (var z in specific)
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

        if (cache != null)
            yield return cache;
    }

    private const int Generation = 8;
    private const EntityContext Context = EntityContext.Gen8;
    private const byte EggLevel = 1;

    private static IEnumerable<EncounterEgg> GetEggs(EvoCriteria[] chain, GameVersion version)
    {
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            yield break;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            yield break; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.SWSH.IsPresentInGame(species, form))
            yield break;

        yield return CreateEggEncounter(species, form, version);

        // Check for split-breed
        if (species == devolved.Species)
        {
            if (chain.Length < 2)
                yield break; // no split-breed
            devolved = chain[^2];
        }
        if (!Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            yield break;

        yield return CreateEggEncounter(devolved.Species, devolved.Form, version);
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        var pt = PersonalTable.SWSH;
        var pi = pt.GetFormEntry(lowest.Species, lowest.Form);
        if (pi.HatchSpecies != lowest.Species)
            return default; // Something in the evolution chain prevented reaching the baby species-form.
        return (lowest.Species, lowest.Form);
    }
}
