using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;
using System.Diagnostics.CodeAnalysis;

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
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            if (TryGetEgg(chain, game, out var egg))
            {
                yield return egg;
                if (TryGetSplit(egg, chain, out var split))
                    yield return split;
            }
        }
        if (groups.HasFlag(Static))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters8.StaticSWSH))
                yield return enc;
            var table = game == GameVersion.SW ? Encounters8.StaticSW : Encounters8.StaticSH;
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters8Nest.Nest_SW))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters8Nest.Nest_SH))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters8Nest.Dist_SW))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters8Nest.Dist_SH))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters8Nest.DynAdv_SWSH))
                yield return enc;
            foreach (var enc in GetPossibleAll(chain, Encounters8Nest.Crystal_SWSH))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            if (game is GameVersion.SW)
            {
                foreach (var enc in GetPossibleSlots<EncounterArea8, EncounterSlot8>(chain, Encounters8.SlotsSW_Symbol))
                    yield return enc;
                foreach (var enc in GetPossibleSlots<EncounterArea8, EncounterSlot8>(chain, Encounters8.SlotsSW_Hidden))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossibleSlots<EncounterArea8, EncounterSlot8>(chain, Encounters8.SlotsSH_Symbol))
                    yield return enc;
                foreach (var enc in GetPossibleSlots<EncounterArea8, EncounterSlot8>(chain, Encounters8.SlotsSH_Hidden))
                    yield return enc;
            }
        }
        if (groups.HasFlag(Trade))
        {
            var table = Encounters8.TradeSWSH;
            foreach (var enc in GetPossibleAll(chain, table))
                yield return enc;
            var specific = game == GameVersion.SW ? Encounters8.TradeSW : Encounters8.TradeSH;
            foreach (var enc in GetPossibleAll(chain, specific))
                yield return enc;
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator8(pk, chain, (GameVersion)pk.Version);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private const int Generation = 8;
    private const EntityContext Context = EntityContext.Gen8;
    private const byte EggLevel = 1;

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
        if (!PersonalTable.SWSH.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

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
