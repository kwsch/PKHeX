using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator3 : IEncounterGenerator
{
    public static readonly EncounterGenerator3 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible3(chain, groups, game);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 3);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (chain.Length == 0)
            yield break;

        info.PIDIV = MethodFinder.Analyze(pk);
        var game = (GameVersion)pk.Version;
        var iterator = new EncounterEnumerator3(pk, chain, game);
        IEncounterable? deferType = null;
        EncounterSlot3? deferSlot = null;
        var leadQueue = new LeadEncounterQueue<EncounterSlot3>();

        bool emerald = pk.E;
        byte gender = (byte)pk.Gender;
        if (pk.Species is (int)Species.Marill or (int)Species.Azumarill)
            gender = (byte)EntityGender.GetFromPIDAndRatio(pk.EncryptionConstant, 0x3F);

        foreach (var enc in iterator)
        {
            var e = enc.Encounter;
            if (!IsTypeCompatible(e, pk, info.PIDIV.Type))
            {
                deferType ??= e;
                continue;
            }

            if (e is not EncounterSlot3 slot)
            {
                yield return e;
                continue;
            }
            if (slot is EncounterSlot3Swarm)
            {
                yield return slot;
                continue;
            }

            var evo = LeadFinder.GetLevelConstraint(pk, chain, slot, 3);
            var lead = LeadFinder.GetLeadInfo3(slot, info.PIDIV, evo, emerald, gender, (byte)pk.Format);
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

        if (deferType != null)
        {
            // Error will be flagged later if this is chosen.
            info.PIDIVMatches = false;
            yield return deferType;
        }
        else if (deferSlot != null)
        {
            info.FrameMatches = false;
            yield return deferSlot;
        }
    }

    private static bool IsTypeCompatible(IEncounterTemplate enc, PKM pk, PIDType type)
    {
        if (enc is IRandomCorrelation r)
            return r.IsCompatible(type, pk);
        return type == PIDType.None;
    }

    private const int Generation = 3;
    private const EntityContext Context = EntityContext.Gen3;
    private const byte EggLevel = 5;

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves3.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies3(devolved.Species))
            return false; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            return false;
        // Sanity Check 3
        if (!PersonalTable.E.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    // Version is not updated when hatching an Egg in Gen3. Version is a clear indicator of the game it originated on.

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
        if (!Breeding.IsSplitBreedNotBabySpecies3(devolved.Species))
            return false;

        result = other with { Species = devolved.Species, Form = devolved.Form };
        return true;
    }
}
