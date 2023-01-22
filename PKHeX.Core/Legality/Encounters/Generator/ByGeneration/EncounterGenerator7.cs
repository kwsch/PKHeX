using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator7 : IEncounterGenerator
{
    public static readonly EncounterGenerator7 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G7;
            foreach (var enc in GetPossibleGifts(chain, table))
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
            var table = GetStatic(game);
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game);
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = GetTrades(game);
            foreach (var enc in GetPossibleTrades(chain, table))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<WC7> table)
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

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic7[] table)
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

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea7[] areas)
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

    private static IEnumerable<IEncounterable> GetPossibleTrades(EvoCriteria[] chain, EncounterTrade7[] table)
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

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var game = (GameVersion)pk.Version;

        bool yielded = false;
        if (pk.FatefulEncounter)
        {
            foreach (var mg in EncounterEvent.MGDB_G7)
            {
                foreach (var evo in chain)
                {
                    if (mg.Species != evo.Species)
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

        if (Locations.IsEggLocationBred6(pk.Egg_Location))
        {
            var eggs = GetEggs(pk, chain, game);
            foreach (var egg in eggs)
                yield return egg;
            if (chain[^1].Species != (int)Species.Eevee) // Static encounter clash (gift egg)
                yield break;
        }

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        var table = GetStatic(game);
        foreach (var z in table)
        {
            foreach (var evo in chain)
            {
                if (z.Species != evo.Species)
                    continue;
                if (!z.IsMatchExact(pk, evo))
                    continue;

                var match = z.GetMatchRating(pk);
                switch (match)
                {
                    case Match: yield return z; yielded = true; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }
        }
        if (yielded)
            yield break;

        if (CanBeWildEncounter(pk))
        {
            var location = pk.Met_Location;
            var areas = GetAreas(game);
            foreach (var area in areas)
            {
                if (!area.IsMatchLocation(location))
                    continue;

                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var slot in slots)
                {
                    var match = slot.GetMatchRating(pk);
                    switch (match)
                    {
                        case Match: yield return slot; yielded = true; break;
                        case Deferred: deferred ??= slot; break;
                        case PartialMatch: partial ??= slot; break;
                    }
                }
            }
            if (yielded)
                yield break;
        }

        var trades = GetTrades(game);
        foreach (var trade in trades)
        {
            foreach (var evo in chain)
            {
                if (trade.Species != evo.Species)
                    continue;
                if (!trade.IsMatchExact(pk, evo))
                    continue;

                var match = trade.GetMatchRating(pk);
                switch (match)
                {
                    case Match: yield return trade; break;
                    case Deferred: deferred ??= trade; break;
                    case PartialMatch: partial ??= trade; break;
                }
            }
        }

        if (deferred != null)
            yield return deferred;
        else if (partial != null)
            yield return partial;
    }

    private static EncounterStatic7[] GetStatic(GameVersion game) => game switch
    {
        GameVersion.SN => Encounters7SM.StaticSN,
        GameVersion.MN => Encounters7SM.StaticMN,
        GameVersion.US => Encounters7USUM.StaticUS,
        GameVersion.UM => Encounters7USUM.StaticUM,
        _ => throw new ArgumentOutOfRangeException(nameof(game), game, null),
    };

    private static EncounterArea7[] GetAreas(GameVersion game) => game switch
    {
        GameVersion.SN => Encounters7SM.SlotsSN,
        GameVersion.MN => Encounters7SM.SlotsMN,
        GameVersion.US => Encounters7USUM.SlotsUS,
        GameVersion.UM => Encounters7USUM.SlotsUM,
        _ => throw new ArgumentOutOfRangeException(nameof(game), game, null),
    };

    private static EncounterTrade7[] GetTrades(GameVersion game) => game switch
    {
        GameVersion.SN => Encounters7SM.TradeGift_SM,
        GameVersion.MN => Encounters7SM.TradeGift_SM,
        GameVersion.US => Encounters7USUM.TradeGift_USUM,
        GameVersion.UM => Encounters7USUM.TradeGift_USUM,
        _ => throw new ArgumentOutOfRangeException(nameof(game), game, null),
    };

    internal static EncounterStatic7 GetVCStaticTransferEncounter(PKM pk, ushort encSpecies, ReadOnlySpan<EvoCriteria> chain)
    {
        // Obtain the lowest evolution species with matching OT friendship. Not all species chains have the same base friendship.
        var met = (byte)pk.Met_Level;
        if (pk.VC1)
        {
            // Only yield a VC1 template if it could originate in VC1.
            // Catch anything that can only exist in VC2 (Entei) even if it was "transferred" from VC1.
            var species = GetVCSpecies(chain, pk, Legal.MaxSpeciesID_1);
            var vc1Species = species > Legal.MaxSpeciesID_1 ? encSpecies : species;
            if (vc1Species <= Legal.MaxSpeciesID_1)
                return EncounterStatic7.GetVC1(vc1Species, met);
        }
        // fall through else
        {
            var species = GetVCSpecies(chain, pk, Legal.MaxSpeciesID_2);
            return EncounterStatic7.GetVC2(species > Legal.MaxSpeciesID_2 ? encSpecies : species, met);
        }
    }

    /// <summary>
    /// Get the most devolved species that matches the <see cref="pk"/> <see cref="PKM.OT_Friendship"/>.
    /// </summary>
    private static ushort GetVCSpecies(ReadOnlySpan<EvoCriteria> chain, PKM pk, ushort maxSpecies)
    {
        for (var i = chain.Length - 1; i >= 0; i--)
        {
            var evo = chain[i];
            if (evo.Species > maxSpecies)
                continue;
            if (evo.Form != 0)
                continue;
            if (PersonalTable.SM.GetFormEntry(evo.Species, evo.Form).BaseFriendship != pk.OT_Friendship)
                continue;
            return evo.Species;
        }
        return pk.Species;
    }

    private const int Generation = 7;
    private const EntityContext Context = EntityContext.Gen7;
    private const byte EggLevel = 1;

    private static IEnumerable<EncounterEgg> GetEggs(PKM pk, EvoCriteria[] chain, GameVersion version)
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
        if (!PersonalTable.USUM.IsPresentInGame(species, form))
            yield break;

        var egg = CreateEggEncounter(species, form, version);
        yield return egg;
        if (pk.IsEgg)
            yield break;
        bool otherVersion = pk is { Egg_Location: Locations.LinkTrade6 };
        if (otherVersion)
            yield return egg with { Version = GetOtherGamePair(version) };

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

        species = devolved.Species;
        form = devolved.Form;
        egg = CreateEggEncounter(species, form, version);
        yield return egg;
        if (otherVersion)
            yield return egg with { Version = GetOtherGamePair(version) };
    }

    private static bool IsValidBabySpecies(ushort species)
    {
        var split = Breeding.GetSplitBreedGeneration(Generation);
        return split is not null && split.Contains(species);
    }

    private static GameVersion GetOtherGamePair(GameVersion version)
    {
        // 30 -> 32 (SN -> US)
        // 31 -> 33 (MN -> UM)
        // 32 -> 30 (US -> SN)
        // 33 -> 31 (UM -> MN)
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        return version ^ (GameVersion)0b111110;
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves7.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }
}
