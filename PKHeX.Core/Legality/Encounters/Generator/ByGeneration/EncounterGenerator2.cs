using System;
using System.Collections.Generic;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator2 : IEncounterGenerator
{
    public static readonly EncounterGenerator2 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        bool korean = pk.Korean;
        if (groups.HasFlag(Mystery))
        {
            if (!korean && ParseSettings.AllowGBEraEvents)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters2GBEra.StaticEventsGB))
                    yield return enc;
            }
        }
        if (groups.HasFlag(Trade))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters2.TradeGift_GSC))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            if (TryGetEgg(chain, game, out var egg))
            {
                yield return egg;
                if (TryGetEggCrystal(pk, egg, out var crystal))
                    yield return crystal;
            }
        }
        if (groups.HasFlag(Static))
        {
            foreach (var enc in GetPossibleAll(chain, Encounters2.StaticGSC))
                yield return enc;
            if (game is GameVersion.GD)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticGD))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticGS))
                    yield return enc;
            }
            else if (game is GameVersion.SV)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticSV))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticGS))
                    yield return enc;
            }
            else if (game is GameVersion.C && !pk.Korean)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticC))
                    yield return enc;
            }
            else if (game is GameVersion.GS || pk.Korean)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticGD))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticSV))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticGS))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticGD))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticSV))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticGS))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters2.StaticC))
                    yield return enc;
            }
            if (ParseSettings.AllowGBVirtualConsole3DS && game is GameVersion.C or GameVersion.GSC && !korean)
            {
                var celebi = Encounters2.CelebiVC;
                if (chain[0].Species == celebi.Species)
                    yield return celebi;
            }
        }
        if (groups.HasFlag(Slot))
        {
            if (!korean)
            {
                foreach (var enc in GetPossibleSlots(chain, Encounters2.SlotsC, pk))
                    yield return enc;
            }
            foreach (var enc in GetPossibleSlots(chain, Encounters2.SlotsGD, pk))
                yield return enc;
            foreach (var enc in GetPossibleSlots(chain, Encounters2.SlotsSV, pk))
                yield return enc;
        }
    }

    private static IEnumerable<EncounterSlot2> GetPossibleSlots(EvoCriteria[] chain, EncounterArea2[] areas, ITrainerID16 pk)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;

                    if (slot.IsHeadbutt && !slot.IsTreeAvailable(pk.TID16))
                        break;
                    yield return slot;
                    break;
                }
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        throw new ArgumentException("Generator does not support direct calls to this method.");
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, GameVersion game)
    {
        var chain = EncounterOrigin.GetOriginChain12(pk, game);
        if (chain.Length == 0)
            return Array.Empty<IEncounterable>();
        return GetEncounters(pk, chain);
    }

    private static IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain)
    {
        var iterator = new EncounterEnumerator2(pk, chain);
        foreach (var enc in iterator)
            yield return enc.Encounter;
    }

    private const int Generation = 2;
    private const EntityContext Context = EntityContext.Gen2;
    private const byte EggLevel = 5;

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation))
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves2.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species)
            return false; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        if (form != 0)
            return false; // Forms don't exist in Gen2, besides Unown (which can't breed). Nothing can form-change.
        // Sanity Check 3
        if (!PersonalTable.C.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    // Depending on the game it was hatched (GS vs C), met data will be present.
    // Since met data can't be used to infer which game it was created on, we yield both if possible.
    public static bool TryGetEggCrystal(PKM pk, EncounterEgg egg, [NotNullWhen(true)] out EncounterEgg? crystal)
    {
        if (!ParseSettings.AllowGen2Crystal(pk))
        {
            crystal = null;
            return false;
        }
        crystal = egg with { Version = GameVersion.C };
        return true;
    }
}
