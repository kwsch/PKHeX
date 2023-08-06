using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;
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
            return Array.Empty<IEncounterable>();
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            if (chain[^1].Species == (int)Species.Manaphy)
                yield return RangerManaphy;

            var table = EncounterEvent.MGDB_G4;
            foreach (var enc in GetPossibleGifts(chain, table, game))
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
            if (game is GameVersion.HG or GameVersion.SS)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters4HGSS.Encounter_HGSS))
                    yield return enc;
                var specific = game == GameVersion.HG ? Encounters4HGSS.StaticHG : Encounters4HGSS.StaticSS;
                foreach (var enc in GetPossibleAll(chain, specific))
                    yield return enc;
                foreach (var enc in GetPossibleAll(chain, Encounters4HGSS.Encounter_PokeWalker))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossibleAll(chain, Encounters4DPPt.StaticDPPt))
                    yield return enc;
                if (game is GameVersion.Pt)
                {
                    foreach (var enc in GetPossibleAll(chain, Encounters4DPPt.StaticPt))
                        yield return enc;
                }
                else
                {
                    foreach (var enc in GetPossibleAll(chain, Encounters4DPPt.StaticDP))
                        yield return enc;
                    var specific = game == GameVersion.D ? Encounters4DPPt.StaticD : Encounters4DPPt.StaticP;
                    foreach (var enc in GetPossibleAll(chain, specific))
                        yield return enc;
                }
            }
        }
        if (groups.HasFlag(Slot))
        {
            if (game is GameVersion.HG)
            {
                foreach (var enc in GetPossibleSlots<EncounterArea4, EncounterSlot4>(chain, Encounters4HGSS.SlotsHG))
                    yield return enc;
            }
            else if (game is GameVersion.SS)
            {
                foreach (var enc in GetPossibleSlots<EncounterArea4, EncounterSlot4>(chain, Encounters4HGSS.SlotsSS))
                    yield return enc;
            }
            else if (game is GameVersion.D)
            {
                foreach (var enc in GetPossibleSlots<EncounterArea4, EncounterSlot4>(chain, Encounters4DPPt.SlotsD))
                    yield return enc;
            }
            else if (game is GameVersion.P)
            {
                foreach (var enc in GetPossibleSlots<EncounterArea4, EncounterSlot4>(chain, Encounters4DPPt.SlotsP))
                    yield return enc;
            }
            else if (game is GameVersion.Pt)
            {
                foreach (var enc in GetPossibleSlots<EncounterArea4, EncounterSlot4>(chain, Encounters4DPPt.SlotsPt))
                    yield return enc;
            }
        }
        if (groups.HasFlag(Trade))
        {
            if (game is GameVersion.HG or GameVersion.SS)
            {
                foreach (var enc in GetPossibleAll(chain, Encounters4HGSS.TradeGift_HGSS))
                    yield return enc;
            }
            else
            {
                foreach (var enc in GetPossibleAll(chain, Encounters4DPPt.TradeGift_DPPtIngame))
                    yield return enc;
                if (game is GameVersion.D)
                {
                    foreach (var enc in GetPossibleAll(chain, Encounters4DPPt.RanchGifts))
                        yield return enc;
                }
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<PCD> table, GameVersion game)
    {
        foreach (var enc in table)
        {
            if (!enc.CanBeReceivedByVersion((int)game))
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
        info.PIDIV = MethodFinder.Analyze(pk);
        var deferredPIDIV = new List<IEncounterable>();
        var deferredEType = new List<IEncounterable>();

        foreach (var z in GetEncountersInner(pk, chain, info))
        {
            if (!info.PIDIV.Type.IsCompatible4(z, pk))
                deferredPIDIV.Add(z);
            else if (pk is IGroundTile e && !(z is IGroundTypeTile t ? t.GroundTile.Contains(e.GroundTile) : e.GroundTile == 0))
                deferredEType.Add(z);
            else
                yield return z;
        }

        foreach (var z in deferredEType)
            yield return z;

        if (deferredPIDIV.Count == 0)
            yield break;

        info.PIDIVMatches = false;
        foreach (var z in deferredPIDIV)
            yield return z;
    }

    private static IEnumerable<IEncounterable> GetEncountersInner(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var game = (GameVersion)pk.Version;
        var iterator = new EncounterEnumerator4(pk, chain, game);
        EncounterSlot4? deferSlot = null;
        List<Frame>? frames = null;
        foreach (var enc in iterator)
        {
            var e = enc.Encounter;
            if (e is not EncounterSlot4 s4)
            {
                yield return e;
                continue;
            }

            var wildFrames = frames ?? AnalyzeFrames(pk, info);
            var frame = wildFrames.Find(s => s.IsSlotCompatibile(s4, pk));
            if (frame != null)
                yield return s4;
            deferSlot ??= s4;
        }
        if (deferSlot != null)
            yield return deferSlot;
    }

    private static List<Frame> AnalyzeFrames(PKM pk, LegalInfo info)
    {
        return FrameFinder.GetFrames(info.PIDIV, pk).ToList();
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
