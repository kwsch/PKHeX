using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public sealed class EncounterGenerator9 : IEncounterGenerator
{
    public static readonly EncounterGenerator9 Instance = new();
    public bool CanGenerateEggs => true;

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 9);
        if (chain.Length == 0)
            return [];

        return pk.Version switch
        {
            SW when pk.MetLocation == LocationsHOME.SWSL => Instance.GetEncountersSWSH(pk, chain, SL),
            SH when pk.MetLocation == LocationsHOME.SHVL => Instance.GetEncountersSWSH(pk, chain, VL),
            _ => GetEncounters(pk, chain, info),
        };
    }

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible9(chain, groups, game);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncountersSWSH(PKM pk, EvoCriteria[] chain, GameVersion game)
    {
        var iterator = new EncounterEnumerator9SWSH(pk, chain, game);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator9(pk, chain, pk.Version);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private const byte Generation = 9;
    private const EntityContext Context = EntityContext.Gen9;
    private const byte EggLevel = 1;

    public static bool TryGetEgg(PKM pk, EvoCriteria[] chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        if (version == 0 && pk.IsEgg)
            version = SL;
        return TryGetEgg(chain, version, out result);
    }

    public static bool TryGetEgg(EvoCriteria[] chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        // No split-breed to consider.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species)
            return false; // no split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            return false;
        // Sanity Check 3
        if (!PersonalTable.SV.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (species == (int)Species.Scatterbug)
            form = Vivillon3DS.FancyFormID; // Fancy
        else if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        var pt = PersonalTable.SV;
        var pi = pt.GetFormEntry(lowest.Species, lowest.Form);
        if (pi.HatchSpecies != lowest.Species)
            return default; // Something in the evolution chain prevented reaching the baby species-form.
        return (lowest.Species, lowest.Form);
    }
}
