using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator4 : IEncounterGenerator
{
    public static readonly EncounterGenerator4 Instance = new();

    // Utility
    internal static readonly PGT RangerManaphy = new() { Data = { [0] = 7, [8] = 1 } };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 4);
        if (chain.Length == 0)
            return [];
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible4(chain, groups, game, pk);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        info.PIDIV = MethodFinder.Analyze(pk);
        var game = (GameVersion)pk.Version;
        var iterator = new EncounterEnumerator4(pk, chain, game);
        EncounterSlot4? deferSlot = null;
        IEncounterable? deferTile = null;
        IEncounterable? deferType = null;
        var leadQueue = new LeadEncounterQueue<EncounterSlot4>();

        foreach (var enc in iterator)
        {
            var e = enc.Encounter;
            if (!IsTileCompatible(e, pk))
            {
                deferTile ??= e;
                continue;
            }

            if (e is not EncounterSlot4 slot)
            {
                yield return e;
                continue;
            }
            if (!IsTypeCompatible(e, pk, info.PIDIV.Type))
            {
                deferSlot ??= slot;
                continue;
            }

            var evo = LeadFinder.GetLevelConstraint(pk, chain, slot, 4);
            var lead = LeadFinder.GetLeadInfo4(pk, slot, info.PIDIV, evo);
            if (!lead.IsValid())
            {
                deferSlot ??= slot;
                continue;
            }
            leadQueue.Insert(lead, slot);
        }

        foreach (var cache in leadQueue.List)
        {
            info.PIDIV = info.PIDIV.AsEncounteredVia(cache.Lead);
            yield return cache.Encounter;
        }
        if (leadQueue.List.Count != 0)
            yield break;

        if (deferTile != null)
        {
            // Error will be flagged later if this is chosen.
            yield return deferTile;
        }
        else if (deferType != null)
        {
            info.PIDIVMatches = false;
            yield return deferType;
        }
        else if (deferSlot != null)
        {
            info.FrameMatches = false;
            yield return deferSlot;
        }
    }

    private static bool IsTileCompatible(IEncounterTemplate enc, PKM pk)
    {
        if (pk is not IGroundTile e)
            return true; // No longer has the data to check
        if (enc is not IGroundTypeTile t)
            return e.GroundTile == 0;
        return t.GroundTile.Contains(e.GroundTile);
    }

    private static bool IsTypeCompatible(IEncounterTemplate enc, PKM pk, PIDType type)
    {
        if (enc is IRandomCorrelation r)
            return r.IsCompatible(type, pk);
        return type == PIDType.None;
    }

    private const int Generation = 4;
    private const EntityContext Context = EntityContext.Gen4;
    private const byte EggLevel = 1;

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves4.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            return false; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            return false;
        // Sanity Check 3
        if (!PersonalTable.HGSS.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    // Version is not updated when hatching an Egg in Gen4. Version is a clear indicator of the game it originated on.

    public static bool TryGetSplit(EncounterEgg other, ReadOnlySpan<EvoCriteria> chain, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        // Check for split-breed
        var devolved = chain[^1];
        if (other.Species == devolved.Species)
        {
            if (chain.Length < 2)
                return false; // no split-breed
            devolved = chain[^2];
        }
        if (!Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            return false;

        result = other with { Species = devolved.Species, Form = devolved.Form };
        return true;
    }
}
