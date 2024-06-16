using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.MemoryPermissions;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="IMemoryOT.OriginalTrainerMemory"/>, <see cref="IMemoryHT.HandlingTrainerMemory"/>, and associated values.
/// </summary>
public sealed class MemoryVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Memory;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var sources = MemoryRules.GetPossibleSources(data.Info.EvoChainsAllGens);
        if (sources == MemorySource.None)
        {
            VerifyOTMemoryIs(data, 0, 0, 0, 0);
            VerifyHTMemoryNone(data, (ITrainerMemories)pk);
            return;
        }
        VerifyOTMemory(data);
        VerifyHTMemory(data, sources);
    }

    private void VerifyHTMemory(LegalityAnalysis data, MemorySource sources)
    {
        var pk = data.Entity;
        sources = MemoryRules.ReviseSourcesHandler(pk, sources, data.EncounterMatch);
        if (sources == MemorySource.None)
        {
            VerifyHTMemoryNone(data, (ITrainerMemories)pk);
            VerifyHTLanguage(data, MemorySource.None);
            return;
        }
        VerifyHTMemoryContextVisited(data, sources);
    }

    private void VerifyHTMemoryContextVisited(LegalityAnalysis data, MemorySource sources)
    {
        // Memories aren't reset when imported into formats, so it could be from any context visited.
        var results = data.Info.Parse;
        var start = results.Count;
        if (sources.HasFlag(MemorySource.Gen6))
        {
            results.RemoveRange(start, results.Count - start);
            VerifyHTMemory(data, Gen6);
            VerifyHTLanguage(data, MemorySource.Gen6);
            if (ValidSet(results, start))
                return;
        }
        if (sources.HasFlag(MemorySource.Gen8))
        {
            results.RemoveRange(start, results.Count - start);
            VerifyHTMemory(data, Gen8);
            VerifyHTLanguage(data, MemorySource.Gen8);
            if (ValidSet(results, start))
                return;
        }
        if (sources.HasFlag(MemorySource.Bank))
        {
            results.RemoveRange(start, results.Count - start);
            VerifyHTMemoryTransferTo7(data, data.Entity, data.Info);
            VerifyHTLanguage(data, MemorySource.Bank);
            if (ValidSet(results, start))
                return;
        }
        if (sources.HasFlag(MemorySource.Deleted))
        {
            results.RemoveRange(start, results.Count - start);
            VerifyHTMemoryNone(data, (ITrainerMemories)data.Entity);
            VerifyHTLanguage(data, MemorySource.Deleted);
        }
    }

    private void VerifyHTLanguage(LegalityAnalysis data, MemorySource source)
    {
        var pk = data.Entity;
        if (pk is not IHandlerLanguage h)
            return;
        if (!GetIsHTLanguageValid(data.EncounterMatch, pk, h.HandlingTrainerLanguage, source))
            data.AddLine(GetInvalid(LMemoryHTLanguage));
    }

    private static bool GetIsHTLanguageValid(IEncounterTemplate enc, PKM pk, byte language, MemorySource source)
    {
        // Bounds check.
        if (language > (int)LanguageID.ChineseT)
            return false;

        // Gen6 and Bank don't have the HT language flag.
        if (source is MemorySource.Gen6 or MemorySource.Bank)
            return language == 0;

        // Some encounters erroneously set the HT flag.
        if (enc is EncounterStatic9 { GiftWithLanguage: true })
        {
            // Must be the SAV language or another-with-HandlingTrainerName.
            if (language == 0)
                return false;
            if (pk.IsUntraded)
                return language == pk.Language;
            return true;
        }

        if (pk.IsUntraded)
            return language == 0;

        // Can be anything within bounds.
        return true;
    }

    private static bool ValidSet(List<CheckResult> results, int start)
    {
        var count = results.Count - start;
        if (count == 0)
            return true; // None added.

        var span = CollectionsMarshal.AsSpan(results)[start..];
        foreach (var result in span)
        {
            if (result.Valid)
                continue;
            return false;
        }
        return true;
    }

    private CheckResult VerifyCommonMemory(PKM pk, int handler, LegalInfo info, MemoryContext mem)
    {
        var memory = MemoryVariableSet.Read((ITrainerMemories)pk, handler);

        // Actionable HM moves
        int hmIndex = MemoryContext6.MoveSpecificMemoryHM.IndexOf(memory.MemoryID);
        if (hmIndex != -1)
            return VerifyMemoryHM6(info, mem, memory, hmIndex);

        if (mem.IsInvalidGeneralLocationMemoryValue(memory.MemoryID, memory.Variable, info.EncounterMatch, pk))
            return GetInvalid(string.Format(LMemoryArgBadLocation, memory.Handler));

        if (mem.IsInvalidMiscMemory(memory.MemoryID, memory.Variable))
            return GetInvalid(string.Format(LMemoryArgBadID, memory.Handler));

        switch (memory.MemoryID)
        {
            case 19 when pk.Species is (int)Species.Urshifu   && memory.Variable is not 34: // tall building is the only location for evolving Urshifu
            case 19 when pk.Species is (int)Species.Runerigus && memory.Variable is not 72: // vast field is the only location for evolving Runerigus
                return GetInvalid(string.Format(LMemoryArgBadLocation, memory.Handler));

            // {0} saw {2} carrying {1} on its back. {4} that {3}.
            case 21 when mem.Context != Gen6 || !PersonalTable.AO.GetFormEntry(memory.Variable, 0).GetIsLearnHM(2): // Fly
                return BadSpeciesMove(memory.Handler);

            // {0} used {2} at {1}’s instruction, but it had no effect. {4} that {3}.
            // The Move Deleter that {0} met through {1} made it forget {2}. {4} that {3}.
            case 16 or 48 when !CanKnowMove(pk, memory, mem.Context, info, memory.MemoryID == 16):
                return BadSpeciesMove(memory.Handler);

            case 49 when memory.Variable == 0 || !GetCanRelearnMove(pk, memory.Variable, mem.Context, info.EvoChainsAllGens, info.EncounterOriginal):
                return BadSpeciesMove(memory.Handler);

            // Dynamaxing
            // {0} battled at {1}’s side against {2} that Dynamaxed. {4} that {3}.
            case 71 when !GetCanDynamaxTrainer(memory.Variable, 8, handler == 0 ? pk.Version : GameVersion.Any):
            // {0} battled {2} and Dynamaxed upon {1}’s instruction. {4} that {3}.
            case 72 when !PersonalTable.SWSH.IsSpeciesInGame(memory.Variable):
                return GetInvalid(string.Format(LMemoryArgBadSpecies, memory.Handler));

            // Move
            // {0} studied about how to use {2} in a Box, thinking about {1}. {4} that {3}.
            // {0} practiced its cool pose for the move {2} in a Box, wishing to be praised by {1}. {4} that {3}.
            case 80 or 81 when !CanKnowMove(pk, memory, mem.Context, info):
                return BadSpeciesMove(memory.Handler);

            // Species
            // With {1}, {0} went fishing, and they caught {2}. {4} that {3}.
            case 7 when !GetCanFishSpecies(memory.Variable, mem.Context, handler == 0 ? pk.Version : GameVersion.Any):
                return GetInvalid(string.Format(LMemoryArgBadSpecies, memory.Handler));

            // {0} saw {1} paying attention to {2}. {4} that {3}.
            // {0} fought hard until it had to use Struggle when it battled at {1}’s side against {2}. {4} that {3}.
            // {0} was taken to a Pokémon Nursery by {1} and left with {2}. {4} that {3}.
            case 9 or 60 or 75 when mem.Context == Gen8 && !PersonalTable.SWSH.IsSpeciesInGame(memory.Variable):
                return GetInvalid(string.Format(LMemoryArgBadSpecies, memory.Handler));

            // {0} had a great chat about {1} with the {2} that it was in a Box with. {4} that {3}.
            // {0} became good friends with the {2} in a Box, practiced moves with it, and talked about the day that {0} would be praised by {1}. {4} that {3}.
            // {0} got in a fight with the {2} that it was in a Box with about {1}. {4} that {3}.
            case 82 or 83 or 87 when !PersonalTable.SWSH.IsSpeciesInGame(memory.Variable):
                return GetInvalid(string.Format(LMemoryArgBadSpecies, memory.Handler));

            // {0} had a very hard training session with {1}. {4} that {3}.
            case 53 when mem.Context == Gen8 && pk is IHyperTrain t && !t.IsHyperTrained():
                return GetInvalid(string.Format(LMemoryArgBadID, memory.Handler));

            // Item
            // {0} went to a Pokémon Center with {1} to buy {2}. {4} that {3}.
            case 5 when !CanBuyItem(mem.Context, memory.Variable, handler == 0 ? pk.Version : GameVersion.Any):
            // {1} used {2} when {0} was in trouble. {4} that {3}.
            case 15 when !CanUseItem(mem.Context, memory.Variable, pk.Species):
            // {0} saw {1} using {2}. {4} that {3}.
            case 26 when !CanUseItemGeneric(mem.Context, memory.Variable):
            // {0} planted {2} with {1} and imagined a big harvest. {4} that {3}.
            case 34 when !CanPlantBerry(mem.Context, memory.Variable):
            // {1} had {0} hold items like {2} to help it along. {4} that {3}.
            case 40 when !CanHoldItem(mem.Context, memory.Variable):
            // {0} was excited when {1} won prizes like {2} through Loto-ID. {4} that {3}.
            case 51 when !CanWinLotoID(mem.Context, memory.Variable):
            // {0} was worried if {1} was looking for the {2} that it was holding in a Box. {4} that {3}.
            // When {0} was in a Box, it thought about the reason why {1} had it hold the {2}. {4} that {3}.
            case 84 or 88 when !Legal.HeldItems_SWSH.Contains(memory.Variable) || pk.IsEgg:
                return GetInvalid(string.Format(LMemoryArgBadItem, memory.Handler));
        }

        return VerifyCommonMemoryEtc(memory, mem);
    }

    private CheckResult VerifyMemoryHM6(LegalInfo info, MemoryContext mem, MemoryVariableSet memory, int hmIndex)
    {
        if (mem.Context != Gen6) // Gen8 has no HMs, so this memory can never exist.
            return BadSpeciesMove(memory.Handler);

        if (info.EncounterMatch.Species == (int)Species.Smeargle)
            return VerifyCommonMemoryEtc(memory, mem);

        // All AO hidden machine permissions are super-sets of Gen 3-5 games.
        // Don't need to check the move history -- a learned HM in a prior game can still be learned in Gen6.
        var pt = PersonalTable.AO;
        foreach (ref readonly var evo in info.EvoChainsAllGens.Gen6.AsSpan())
        {
            var entry = pt[evo.Species];
            var canLearn = entry.GetIsLearnHM(hmIndex);
            if (canLearn)
                return VerifyCommonMemoryEtc(memory, mem);
            break;
        }

        return BadSpeciesMove(memory.Handler);
    }

    private CheckResult BadSpeciesMove(string handler) => GetInvalid(string.Format(LMemoryArgBadMove, handler));

    private CheckResult VerifyCommonMemoryEtc(MemoryVariableSet memory, MemoryContext context)
    {
        if (!context.CanHaveIntensity(memory.MemoryID, memory.Intensity))
        {
            var min = context.GetMinimumIntensity(memory.MemoryID);
            return GetInvalid(string.Format(LMemoryIndexIntensityMin, memory.Handler, min));
        }

        if (!context.CanHaveFeeling(memory.MemoryID, memory.Feeling, memory.Variable))
            return GetInvalid(string.Format(LMemoryFeelInvalid, memory.Handler));

        return GetValid(string.Format(LMemoryF_0_Valid, memory.Handler));
    }

    /// <summary>
    /// Used for enforcing a fixed memory detail.
    /// </summary>
    /// <param name="data">Output storage</param>
    /// <param name="m">Memory ID</param>
    /// <param name="i">Intensity</param>
    /// <param name="t">Text Variable</param>
    /// <param name="f">Feeling</param>
    private void VerifyOTMemoryIs(LegalityAnalysis data, byte m, byte i, ushort t, byte f)
    {
        var pk = (ITrainerMemories)data.Entity;
        if (pk.OriginalTrainerMemory != m)
            data.AddLine(GetInvalid(string.Format(LMemoryIndexID, L_XOT, m)));
        if (pk.OriginalTrainerMemoryIntensity != i)
            data.AddLine(GetInvalid(string.Format(LMemoryIndexIntensity, L_XOT, i)));
        if (pk.OriginalTrainerMemoryVariable != t)
            data.AddLine(GetInvalid(string.Format(LMemoryIndexVar, L_XOT, t)));
        if (pk.OriginalTrainerMemoryFeeling != f)
            data.AddLine(GetInvalid(string.Format(LMemoryIndexFeel, L_XOT, f)));
    }

    private void VerifyHTMemoryNone(LegalityAnalysis data, ITrainerMemories pk)
    {
        if (pk.HandlingTrainerMemory != 0 || pk.HandlingTrainerMemoryVariable != 0 || pk.HandlingTrainerMemoryIntensity != 0 || pk.HandlingTrainerMemoryFeeling != 0)
            data.AddLine(GetInvalid(string.Format(LMemoryCleared, L_XHT)));
    }

    private void VerifyOTMemory(LegalityAnalysis data)
    {
        var enc = data.Info.EncounterMatch;
        var context = enc.Context;
        var pk = data.Entity;
        var mem = (ITrainerMemories)pk;

        // If the encounter has a memory from the OT that could never have it replaced, ensure it was not modified.
        switch (data.EncounterMatch)
        {
            case WC6 {IsEgg: false} g when g.OTGender != 3:
                if (g.OriginalTrainerMemory is not 0 && ParseSettings.Settings.Handler.Restrictions.AllowHandleOTGen6)
                    break;
                VerifyOTMemoryIs(data, g.OriginalTrainerMemory, g.OriginalTrainerMemoryIntensity, g.OriginalTrainerMemoryVariable, g.OriginalTrainerMemoryFeeling);
                return;
            case WC7 {IsEgg: false} g when g.OTGender != 3:
                if (g.OriginalTrainerMemory is not 0 && ParseSettings.Settings.Handler.Restrictions.AllowHandleOTGen7)
                    break;
                VerifyOTMemoryIs(data, g.OriginalTrainerMemory, g.OriginalTrainerMemoryIntensity, g.OriginalTrainerMemoryVariable, g.OriginalTrainerMemoryFeeling);
                return;
            case WC8 {IsEgg: false} g when g.OTGender != 3:
                if (g.OriginalTrainerMemory is not 0 && ParseSettings.Settings.Handler.Restrictions.AllowHandleOTGen8)
                    break;
                VerifyOTMemoryIs(data, g.OriginalTrainerMemory, g.OriginalTrainerMemoryIntensity, g.OriginalTrainerMemoryVariable, g.OriginalTrainerMemoryFeeling);
                return;

            case IMemoryOTReadOnly t and not MysteryGift: // Ignore Mystery Gift cases (covered above)
                VerifyOTMemoryIs(data, t.OriginalTrainerMemory, t.OriginalTrainerMemoryIntensity, t.OriginalTrainerMemoryVariable, t.OriginalTrainerMemoryFeeling);
                return;
        }

        var memory = mem.OriginalTrainerMemory;

        if (pk.IsEgg)
        {
            // Traded unhatched eggs in Gen8 have OT link trade memory applied erroneously.
            // They can also have the box-inspect memory!
            if (context != Gen8 || !((pk.MetLocation == Locations.LinkTrade6 && memory == 4) || memory == 85))
            {
                VerifyOTMemoryIs(data, 0, 0, 0, 0); // empty
                return;
            }
        }
        else if (!CanHaveMemoryForOT(pk, context, memory))
        {
            VerifyOTMemoryIs(data, 0, 0, 0, 0); // empty
            return;
        }

        // Bounds checking
        var mc = Memories.GetContext(context);
        if (!mc.CanObtainMemoryOT(pk.Version, memory))
            data.AddLine(GetInvalid(string.Format(LMemoryArgBadID, L_XOT)));

        // Verify memory if specific to OT
        switch (memory)
        {
            // No Memory
            case 0: // SW/SH trades don't set HT memories immediately, which is hilarious.
                data.AddLine(Get(LMemoryMissingOT, mc.Context == Gen8 ? Severity.Fishy : Severity.Invalid));
                VerifyOTMemoryIs(data, 0, 0, 0, 0);
                return;

            // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
            case 2 when !enc.IsEgg:
                data.AddLine(GetInvalid(string.Format(LMemoryArgBadHatch, L_XOT)));
                break;

            // {0} became {1}’s friend when it arrived via Link Trade at... {2}. {4} that {3}.
            case 4 when mc.Context == Gen6: // Gen8 applies this memory erroneously
                data.AddLine(GetInvalid(string.Format(LMemoryArgBadOTEgg, L_XOT)));
                return;

            // {0} went to the Pokémon Center in {2} with {1} and had its tired body healed there. {4} that {3}.
            case 6 when !mc.HasPokeCenter(pk.Version, mem.OriginalTrainerMemoryVariable):
                data.AddLine(GetInvalid(string.Format(LMemoryArgBadLocation, L_XOT)));
                return;

            // {0} was with {1} when {1} caught {2}. {4} that {3}.
            case 14:
                var result = GetCanBeCaptured(mem.OriginalTrainerMemoryVariable, mc.Context, pk.Version) // Any Game in the Handling Trainer's generation
                    ? GetValid(string.Format(LMemoryArgSpecies, L_XOT))
                    : GetInvalid(string.Format(LMemoryArgBadSpecies, L_XOT));
                data.AddLine(result);
                return;
        }

        data.AddLine(VerifyCommonMemory(pk, 0, data.Info, mc));
    }

    private static bool CanHaveMemoryForOT(PKM pk, EntityContext origin, int memory)
    {
        // Eggs cannot have memories
        if (pk.IsEgg)
        {
            if (origin == Gen8) // Gen8 sets for traded eggs.
                return pk.IsTradedEgg;
            return false;
        }

        return origin switch
        {
            Gen1 or Gen2 or Gen7 => memory == 4, // VC transfers can only have Bank memory.
            Gen6 => true,
            Gen8 => !(pk.GO_HOME || pk.MetLocation == Locations.HOME8), // HOME does not set memories.
            _ => false,
        };
    }

    private void VerifyHTMemory(LegalityAnalysis data, EntityContext memoryGen)
    {
        var pk = data.Entity;
        var mem = (ITrainerMemories)pk;

        var memory = mem.HandlingTrainerMemory;

        if (pk.IsUntraded)
        {
            if (memory == 4 && WasTradedSWSHEgg(pk))
            {
                // Untraded link trade eggs in Gen8 have HT link trade memory applied erroneously.
                // Verify the link trade memory later.
            }
            else
            {
                VerifyHTMemoryNone(data, mem);
                return;
            }
        }

        if (pk.Format == 7)
        {
            VerifyHTMemoryTransferTo7(data, pk, data.Info);
            return;
        }

        // Bounds checking
        var mc = Memories.GetContext(memoryGen);
        if (!mc.CanObtainMemoryHT(pk.Version, memory))
            data.AddLine(GetInvalid(string.Format(LMemoryArgBadID, L_XHT)));

        // Verify memory if specific to HT
        switch (memory)
        {
            // No Memory
            case 0: // SW/SH memory application has an off-by-one error: [0,99] + 1 <= chance --> don't apply
                var severity = mc.Context switch
                {
                    Gen8 when pk is not PK8 && !pk.SWSH => Severity.Valid,
                    Gen8 => ParseSettings.Settings.Game.Gen8.Gen8MemoryMissingHT,
                    _ => Severity.Invalid,
                };
                if (severity != Severity.Valid)
                    data.AddLine(Get(LMemoryMissingHT, severity));
                VerifyHTMemoryNone(data, mem);
                return;

            // {0} met {1} at... {2}. {1} threw a Poké Ball at it, and they started to travel together. {4} that {3}.
            case 1:
                data.AddLine(GetInvalid(string.Format(LMemoryArgBadCatch, L_XHT)));
                return;

            // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
            case 2:
                data.AddLine(GetInvalid(string.Format(LMemoryArgBadHatch, L_XHT)));
                return;

            // {0} went to the Pokémon Center in {2} with {1} and had its tired body healed there. {4} that {3}.
            case 6 when !mc.HasPokeCenter(GameVersion.Any, mem.HandlingTrainerMemoryVariable):
                data.AddLine(GetInvalid(string.Format(LMemoryArgBadLocation, L_XHT)));
                return;

            // {0} was with {1} when {1} caught {2}. {4} that {3}.
            case 14:
                var result = GetCanBeCaptured(mem.HandlingTrainerMemoryVariable, mc.Context, GameVersion.Any) // Any Game in the Handling Trainer's generation
                    ? GetValid(string.Format(LMemoryArgSpecies, L_XHT))
                    : GetInvalid(string.Format(LMemoryArgBadSpecies, L_XHT));
                data.AddLine(result);
                return;
        }

        var commonResult = VerifyCommonMemory(pk, 1, data.Info, mc);
        data.AddLine(commonResult);
    }

    private static bool WasTradedSWSHEgg(PKM pk) => pk.SWSH && (!pk.IsEgg ? pk.EggLocation : pk.MetLocation) is Locations.LinkTrade6;

    private void VerifyHTMemoryTransferTo7(LegalityAnalysis data, PKM pk, LegalInfo Info)
    {
        var mem = (ITrainerMemories)pk;
        // Bank Transfer adds in the Link Trade Memory.
        // Trading 7<->7 between games (not Bank) clears this data.
        if (mem.HandlingTrainerMemory == 0)
        {
            VerifyHTMemoryNone(data, mem);
            return;
        }

        // Transfer 6->7 & withdraw to same HT => keeps past gen memory
        // Don't require link trade memory for these past gen cases
        var gen = Info.Generation;
        if (gen is >= 3 and < 7 && pk.CurrentHandler == 1)
            return;

        if (mem.HandlingTrainerMemory != 4)
            data.AddLine(Severity.Invalid, LMemoryIndexLinkHT, CheckIdentifier.Memory);
        if (mem.HandlingTrainerMemoryVariable != 0)
            data.AddLine(Severity.Invalid, LMemoryIndexArgHT, CheckIdentifier.Memory);
        if (mem.HandlingTrainerMemoryIntensity != 1)
            data.AddLine(Severity.Invalid, LMemoryIndexIntensityHT1, CheckIdentifier.Memory);
        if (mem.HandlingTrainerMemoryFeeling > 10)
            data.AddLine(Severity.Invalid, LMemoryIndexFeelHT09, CheckIdentifier.Memory);
    }
}
