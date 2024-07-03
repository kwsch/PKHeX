using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator7 : IEncounterGenerator
{
    public static readonly EncounterGenerator7 Instance = new();
    public bool CanGenerateEggs => true;

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible7(chain, groups, game);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var iterator = new EncounterEnumerator7(pk, chain, pk.Version);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    internal static EncounterTransfer7 GetVCStaticTransferEncounter(PKM pk, ushort encSpecies, ReadOnlySpan<EvoCriteria> chain)
    {
        // Obtain the lowest evolution species with matching OT friendship. Not all species chains have the same base friendship.
        var met = pk.MetLevel;
        if (pk.VC1)
        {
            // Only yield a VC1 template if it could originate in VC1.
            // Catch anything that can only exist in VC2 (Entei) even if it was "transferred" from VC1.
            var species = GetVCSpecies(chain, pk, Legal.MaxSpeciesID_1);
            var vc1Species = species > Legal.MaxSpeciesID_1 ? encSpecies : species;
            if (vc1Species <= Legal.MaxSpeciesID_1)
                return EncounterTransfer7.GetVC1(vc1Species, met);
        }
        // fall through else
        {
            var species = GetVCSpecies(chain, pk, Legal.MaxSpeciesID_2);
            return EncounterTransfer7.GetVC2(species > Legal.MaxSpeciesID_2 ? encSpecies : species, met);
        }
    }

    /// <summary>
    /// Get the most devolved species that matches the <see cref="pk"/> <see cref="PKM.OriginalTrainerFriendship"/>.
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
            if (PersonalTable.SM.GetFormEntry(evo.Species, evo.Form).BaseFriendship != pk.OriginalTrainerFriendship)
                continue;
            return evo.Species;
        }
        return pk.Species;
    }

    private const byte Generation = 7;
    private const EntityContext Context = EntityContext.Gen7;
    private const byte EggLevel = EggStateLegality.EggMetLevel;

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
        if (!PersonalTable.USUM.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    public static EncounterEgg MutateEggTrade(EncounterEgg egg) => egg with { Version = GetOtherGamePair(egg.Version) };

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

    private static GameVersion GetOtherGamePair(GameVersion version)
    {
        // 30 -> 32 (SN -> US)
        // 31 -> 33 (MN -> UM)
        // 32 -> 30 (US -> SN)
        // 33 -> 31 (UM -> MN)
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
#pragma warning disable RCS1130 // Bitwise operation on enum without Flags attribute.
        return version ^ (GameVersion)0b111110;
#pragma warning restore RCS1130 // Bitwise operation on enum without Flags attribute.
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
