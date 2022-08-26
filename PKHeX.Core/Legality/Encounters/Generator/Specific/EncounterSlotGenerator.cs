using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;
using static PKHeX.Core.Encounters1;
using static PKHeX.Core.Encounters2;
using static PKHeX.Core.Encounters3;
using static PKHeX.Core.Encounters3GC;
using static PKHeX.Core.Encounters4;
using static PKHeX.Core.Encounters5;
using static PKHeX.Core.Encounters6;
using static PKHeX.Core.Encounters7;
using static PKHeX.Core.Encounters7b;
using static PKHeX.Core.Encounters8;
using static PKHeX.Core.Encounters8a;
using static PKHeX.Core.Encounters8b;
using static PKHeX.Core.EncountersGO;

using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class EncounterSlotGenerator
{
    public static IEnumerable<EncounterSlot> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion gameSource)
    {
        var possibleAreas = GetAreasByGame(pk, gameSource);
        foreach (var area in possibleAreas)
        {
            foreach (var result in area.GetSpecies(chain))
                yield return result;
        }
    }

    private static IEnumerable<EncounterArea> GetAreasByGame(PKM pk, GameVersion gameSource) => gameSource switch
    {
        RD => SlotsRD,
        GN => SlotsGN,
        BU => SlotsBU,
        YW => SlotsYW,

        GD => SlotsGD,
        SI => SlotsSV,
        C => SlotsC,

        _ => GetEncounterTable(pk, gameSource),
    };

    private static IEnumerable<EncounterSlot> GetRawEncounterSlots(PKM pk, EvoCriteria[] chain, GameVersion gameSource)
    {
        if (pk.IsEgg)
            yield break;
        if (IsMetAsEgg(pk))
            yield break;

        var possibleAreas = GetEncounterAreas(pk, gameSource);
        foreach (var area in possibleAreas)
        {
            var slots = area.GetMatchingSlots(pk, chain);
            foreach (var s in slots)
                yield return s;
        }
    }

    public static IEnumerable<EncounterSlot> GetValidWildEncounters12(PKM pk, EvoCriteria[] chain, GameVersion gameSource)
    {
        return GetRawEncounterSlots(pk, chain, gameSource);
    }

    public static IEnumerable<EncounterSlot> GetValidWildEncounters(PKM pk, EvoCriteria[] chain, GameVersion gameSource)
    {
        return GetRawEncounterSlots(pk, chain, gameSource);
    }

    public static IEnumerable<EncounterSlot> GetValidWildEncounters(PKM pk, EvoCriteria[] chain)
    {
        var gameSource = (GameVersion)pk.Version;
        return GetRawEncounterSlots(pk, chain, gameSource);
    }

    private static IEnumerable<EncounterArea> GetEncounterAreas(PKM pk, GameVersion gameSource)
    {
        var slots = GetEncounterTable(pk, gameSource);
        bool noMet = !pk.HasOriginalMetLocation || (pk.Format == 2 && gameSource != C);
        if (noMet)
            return slots;
        return GetIsMatchLocation(pk, slots);
    }

    private static IEnumerable<EncounterArea> GetIsMatchLocation(PKM pk, IEnumerable<EncounterArea> areas)
    {
        var metLocation = pk.Met_Location;
        foreach (var area in areas)
        {
            if (area.IsMatchLocation(metLocation))
                yield return area;
        }
    }

    internal static EncounterSlot? GetCaptureLocation(PKM pk, EvoCriteria[] chain)
    {
        var possible = GetPossible(pk, chain, (GameVersion)pk.Version);
        return EncounterUtil.GetMinByLevel(chain, possible);
    }

    private static IEnumerable<EncounterArea> GetEncounterTable(PKM pk, GameVersion game) => game switch
    {
        RBY or RD or BU or GN or YW => pk.Japanese ? SlotsRGBY : SlotsRBY,

        GSC or GD or SI or C => GetEncounterTableGSC(pk),

        R => SlotsR,
        S => SlotsS,
        E => SlotsE,
        FR => SlotsFR,
        LG => SlotsLG,
        CXD => SlotsXD,

        D => SlotsD,
        P => SlotsP,
        Pt => SlotsPt,
        HG => SlotsHG,
        SS => SlotsSS,

        B => SlotsB,
        W => SlotsW,
        B2 => SlotsB2,
        W2 => SlotsW2,

        X => SlotsX,
        Y => SlotsY,
        AS => SlotsA,
        OR => SlotsO,

        SN => SlotsSN,
        MN => SlotsMN,
        US => SlotsUS,
        UM => SlotsUM,
        GP => SlotsGP,
        GE => SlotsGE,

        GO => GetEncounterTableGO(pk),
        SW => SlotsSW,
        SH => SlotsSH,
        BD => SlotsBD,
        SP => SlotsSP,
        PLA => SlotsLA,
        _ => Array.Empty<EncounterArea>(),
    };

    private static EncounterArea[] GetEncounterTableGSC(PKM pk)
    {
        if (!ParseSettings.AllowGen2Crystal(pk))
            return SlotsGS;

        // Gen 2 met location is lost outside gen 2 games
        if (pk.Format != 2)
            return SlotsGSC;

        // Format 2 with met location, encounter should be from Crystal
        if (pk.HasOriginalMetLocation)
            return SlotsC;

        // Format 2 without met location but pokemon could not be tradeback to gen 1,
        // encounter should be from gold or silver
        if (pk.Species > MaxSpeciesID_1 && !EvolutionLegality.FutureEvolutionsGen1.Contains(pk.Species))
            return SlotsGS;

        // Encounter could be any gen 2 game, it can have empty met location for have a g/s origin
        // or it can be a Crystal pokemon that lost met location after being tradeback to gen 1 games
        return SlotsGSC;
    }

    private static IEnumerable<EncounterArea> GetEncounterTableGO(PKM pk)
    {
        if (pk.Format < 8)
            return SlotsGO_GG;

        // If we know the met location, return the specific area list.
        // If we're just getting all encounters (lack of met location is kinda bad...), just return everything.
        var met = pk.Met_Location;
        return met switch
        {
            Locations.GO8 => SlotsGO,
            Locations.GO7 => SlotsGO_GG,
            _ => SlotsGO_GG.Concat<EncounterArea>(SlotsGO),
        };
    }
}
