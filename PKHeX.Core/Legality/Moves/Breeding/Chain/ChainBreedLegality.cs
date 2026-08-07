using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Verifies if an egg move set can be produced by a single compatible father chain.
/// </summary>
public static class ChainBreedLegality
{
    // The longest known in-generation chain is five fathers(?). Leave room for transfer steps.
    internal const int MaxChainDepth = 10;

    // Max moveset across all formats has always been 4; this is just a hard-code.
    private const int MaxMoveCount = 4;

    // Internal flags for marking the source of a child move. These are used to determine which moves can be inherited from a given father.
    private const byte FlagBase = 1 << 0;
    private const byte FlagLevelUp = 1 << 1;
    private const byte FlagGeneral = 1 << 2;

    /// <summary>
    /// Checks if the given species, form, version, and moves can be produced by a valid breeding chain.
    /// This method uses a stack-allocated buffer for trace storage and does not return the trace details.
    /// </summary>
    /// <param name="species">The species of the Pokémon.</param>
    /// <param name="form">The form of the Pokémon.</param>
    /// <param name="version">The game version where the breeding occurs.</param>
    /// <param name="moves">The moves of the Pokémon.</param>
    /// <returns>True if the moves can be produced by a valid breeding chain; otherwise, false.</returns>
    public static bool IsValid(ushort species, byte form, GameVersion version, params ReadOnlySpan<ushort> moves)
    {
        Span<ChainBreedStep> buffer = stackalloc ChainBreedStep[MaxChainDepth];
        var trace = new ChainBreedTrace(buffer);
        return IsValid(species, form, version, moves, ref trace);
    }

    /// <summary>
    /// Checks if the given species, form, version, and moves can be produced by a valid breeding chain.
    /// This method uses a stack-allocated buffer for trace storage and returns a summary of the breeding chain if valid.
    /// </summary>
    /// <param name="species">The species of the Pokémon.</param>
    /// <param name="form">The form of the Pokémon.</param>
    /// <param name="version">The game version where the breeding occurs.</param>
    /// <param name="moves">The moves of the Pokémon.</param>
    /// <param name="summary">The summary of the breeding chain if the moves are valid.</param>
    /// <returns>True if the moves can be produced by a valid breeding chain; otherwise, false.</returns>
    public static bool IsValid(ushort species, byte form, GameVersion version, ReadOnlySpan<ushort> moves, out ChainBreedSummary summary)
    {
        Span<ChainBreedStep> buffer = stackalloc ChainBreedStep[MaxChainDepth];
        var trace = new ChainBreedTrace(buffer);
        bool result = IsValid(species, form, version, moves, ref trace);
        summary = result ? trace.GetSummary() : default;
        return result;
    }

    /// <summary>
    /// Checks if the given species, form, version, and moves can be produced by a valid breeding chain.
    /// This method uses a stack-allocated buffer for trace storage which can be used to generate a summary of the breeding chain if valid.
    /// </summary>
    /// <param name="species">The species of the Pokémon.</param>
    /// <param name="form">The form of the Pokémon.</param>
    /// <param name="version">The game version where the breeding occurs.</param>
    /// <param name="moves">The moves of the Pokémon.</param>
    /// <param name="trace">The trace of the breeding chain.</param>
    /// <returns>True if the moves can be produced by a valid breeding chain; otherwise, false.</returns>
    public static bool IsValid(ushort species, byte form, GameVersion version, ReadOnlySpan<ushort> moves, ref ChainBreedTrace trace)
    {
        trace.Reset();
        // Sanity check our result storage.
        if (trace.Capacity < MaxChainDepth)
            return false;

        // Sanity check inputs
        int count = moves.IndexOf((ushort)0);
        if (count == 0)
            return false;
        if (count == -1)
            count = moves.Length;
        if (count > MaxMoveCount)
            return false;

        // Gen6+: mothers can pass egg moves
        var generation = version.Generation;
        if (generation >= 6)
        {
            bool result = IsValidRelaxed(species, form, version, moves, out var summary);
            if (result)
                trace.SetRelaxed(species, form, version, moves[..count], summary);
            return result;
        }

        // Retrace origins of requested moves
        var learn = GameData.GetLearnSource(version);
        var learnset = learn.GetLearnset(species, form);
        var baseMoves = learnset.GetBaseEggMoves(EggStateLegality.GetEggLevel(generation));

        Span<byte> flags = stackalloc byte[count];
        if (!MarkChildMoveFlags(species, form, version, moves[..count], learnset, flags))
            return false;

        // Run the validation.
        return TryValidateCore(species, form, version, moves[..count], baseMoves, flags, ref trace, 0);
    }
    private static bool TryValidateCore(ushort eggSpecies, byte eggForm, GameVersion version,
        scoped ReadOnlySpan<ushort> moves, scoped ReadOnlySpan<ushort> baseMoves, scoped ReadOnlySpan<byte> flags,
        ref ChainBreedTrace trace, int depth)
    {
        if ((uint)depth >= (uint)trace.Capacity)
            return false;

        var state = ChainBreedStep.CreateEgg(eggSpecies, eggForm, version, moves);
        // Check against visited for exact duplicates (avoid cycles)
        for (int i = 0; i < depth; i++)
        {
            if (trace.QueryEquals(i, state))
                return false;
        }
        trace.SetPending(depth, state);

        int maxBase = Math.Min(moves.Length, baseMoves.Length);
        Span<ushort> inheritedMoves = stackalloc ushort[MaxMoveCount];
        bool result = TryValidateBaseCounts(eggSpecies, eggForm, version, moves, baseMoves, flags, inheritedMoves, maxBase, ref trace, depth);
        if (result)
            trace.Commit(depth + 1);
        return result;
    }

    private static bool TryValidateBaseCounts(ushort eggSpecies, byte eggForm, GameVersion version,
        scoped ReadOnlySpan<ushort> moves,
        scoped ReadOnlySpan<ushort> baseMoves,
        scoped ReadOnlySpan<byte> flags,
        scoped Span<ushort> inheritedMoves,
        int maxBase, ref ChainBreedTrace trace, int depth)
    {
        for (int baseCount = 0; baseCount <= maxBase; baseCount++)
        {
            if (!IsValidBaseCount(baseCount, moves, baseMoves, flags))
                continue;

            int inheritedCount = moves.Length - baseCount;
            if (inheritedCount == 0)
                return true;
            // Note: Even if eggSpecies is in Undiscovered (like baby Pokemon), it can still have
            // inherited moves if its evolved forms can breed (e.g., Tyrogue from Hitmonlee/Hitmonchan/Hitmontop).
            // Let TryResolveInheritedSources handle the validation through TryResolveFatherViaEvolution.

            var suffix = moves.Slice(baseCount, inheritedCount);
            var suffixFlags = flags.Slice(baseCount, inheritedCount);
            suffix.CopyTo(inheritedMoves);

            if (TryResolveInheritedSources(eggSpecies, eggForm, version, inheritedMoves[..inheritedCount], suffixFlags, 0, ref trace, depth + 1))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Species that have male-female splits (Nidoran-M/Nidoran-F, Volbeat/Illumise) must be remapped to the female species (mother).
    /// </summary>
    private static void RemapSpeciesToMother(ref ushort species)
    {
        if (species is (ushort)Species.NidoranM)
            species = (ushort)Species.NidoranF;
        if (species is (ushort)Species.Volbeat)
            species = (ushort)Species.Illumise;
    }

    /// <summary>
    /// Checks if the species is a male-only split.
    /// </summary>
    /// <remarks>
    /// <see cref="RemapSpeciesToMother"/>
    /// </remarks>
    private static bool IsMaleOnlySplitBreed(ushort species) => species is (ushort)Species.NidoranM or (ushort)Species.Volbeat;


    private static bool TryResolveInheritedSources(ushort eggSpecies, byte eggForm, GameVersion version,
        scoped ReadOnlySpan<ushort> moves,
        scoped ReadOnlySpan<byte> flags,
        int index, ref ChainBreedTrace trace, int depth)
    {
        if (index == moves.Length)
            return TryResolveFather(eggSpecies, eggForm, version, moves, ref trace, depth);

        var flag = flags[index];
        if ((flag & FlagGeneral) != 0)
        {
            if (TryResolveInheritedSources(eggSpecies, eggForm, version, moves, flags, index + 1, ref trace, depth))
                return true;
        }

        if ((flag & FlagLevelUp) != 0)
        {
            if (TryResolveInheritedSources(eggSpecies, eggForm, version, moves, flags, index + 1, ref trace, depth))
                return true;
        }

        return false;
    }

    private static (PersonalInfo mi, (ushort Species, byte Form) mother) ResolveMother(ushort species, byte form, GameVersion version, IPersonalTable table)
    {
        PersonalInfo mi;
        (ushort Species, byte Form) mother = (species, form);
        if (IsMaleOnlySplitBreed(species))
        {
            RemapSpeciesToMother(ref mother.Species);
            mi = table[mother.Species, mother.Form];
        }
        else
        {
            mi = table[mother.Species, mother.Form];
            if (mi.EggGroup1 == (int)EggGroup.Undiscovered && TryGetEvolvedMother(species, form, version, out var newMother))
            {
                mi = table[newMother.Species, newMother.Form];
                mother = newMother;
            }
        }

        return (mi, mother);
    }

    private static bool TryResolveFather(ushort eggSpecies, byte eggForm, GameVersion version,
        scoped ReadOnlySpan<ushort> moves,
        ref ChainBreedTrace trace, int depth)
    {
        var table = GameData.GetPersonal(version);
        if (!table.IsPresentInGame(eggSpecies, eggForm))
            return false;

        var (mi, mother) = ResolveMother(eggSpecies, eggForm, version, table);

        // If the egg species can't breed (baby Pokemon like Tyrogue), check if its evolutions can act as fathers
        if (mi.Genderless || mi.OnlyMale || mi.EggGroup1 == (int)EggGroup.Undiscovered)
        {
            // Try to find evolved forms that can breed and produce this egg species
            return TryResolveFatherViaEvolution(eggSpecies, eggForm, version, moves, ref trace, depth);
        }

        ushort maxSpecies = table.MaxSpeciesID;
        for (ushort fatherSpecies = 1; fatherSpecies <= maxSpecies; fatherSpecies++)
        {
            var fatherBase = table[fatherSpecies];
            var formCount = fatherBase.FormCount;
            for (byte fatherForm = 0; fatherForm < formCount; fatherForm++)
            {
                if (!table.IsPresentInGame(fatherSpecies, fatherForm))
                    continue;

                var father = table[fatherSpecies, fatherForm];
                if (!IsCompatibleFather(mi, father))
                    continue;

                if (!CanFatherKnowAllMoves(fatherSpecies, fatherForm, version, moves, ref trace, depth))
                    continue;

                trace.Resolve(depth - 1, mother.Species, mother.Form, fatherSpecies, fatherForm);
                return true;
            }
        }

        return false;
    }

    private static bool TryGetEvolvedMother(ushort eggSpecies, byte eggForm, GameVersion version, out (ushort Species, byte Form) newMother)
    {
        var tree = EvolutionTree.GetEvolutionTree(version.Context);
        var evos = tree.Forward.GetEvolutions(eggSpecies, eggForm);
        var pt = GameData.GetPersonal(version);

        foreach (var (evoSpecies, evoForm) in evos)
        {
            var pi = pt[evoSpecies, evoForm];
            if (pi.EggGroup1 == (int)EggGroup.Undiscovered)
                continue;

            newMother = (evoSpecies, evoForm);
            return true;

        }
        newMother = default;
        return false;
    }

    private static bool TryResolveFatherViaEvolution(ushort eggSpecies, byte eggForm, GameVersion version,
        scoped ReadOnlySpan<ushort> moves,
        ref ChainBreedTrace trace, int depth)
    {
        var tree = EvolutionTree.GetEvolutionTree(version.Context);
        var evos = tree.Forward.GetEvolutions(eggSpecies, eggForm);

        foreach (var (evoSpecies, evoForm) in evos)
        {
            if (!CanFatherKnowAllMoves(evoSpecies, evoForm, version, moves, ref trace, depth))
                continue;

            trace.Resolve(depth - 1, eggSpecies, eggForm, evoSpecies, evoForm);
            return true;
        }

        return false;
    }

    private static bool CanFatherKnowAllMoves(ushort fatherSpecies, byte fatherForm, GameVersion version,
        scoped ReadOnlySpan<ushort> moves,
        ref ChainBreedTrace trace, int depth)
    {
        Span<ushort> pending = stackalloc ushort[MaxMoveCount];
        int pendingCount = 0;
        foreach (var move in moves)
        {
            if (!CanLearnDirectlyInLine(fatherSpecies, fatherForm, version, move))
                pending[pendingCount++] = move;
        }

        if (pendingCount == 0)
            return true;

        Span<(ushort Species, byte Form)> eggSpecies = stackalloc (ushort, byte)[2];
        int eggSpeciesCount = GetEggSpeciesCandidates(fatherSpecies, fatherForm, version, eggSpecies);
        Span<byte> flags = stackalloc byte[MaxMoveCount];
        for (int i = 0; i < eggSpeciesCount; i++)
        {
            var candidate = eggSpecies[i];
            if (candidate.Species == 0)
                continue;

            var learn = GameData.GetLearnSource(version);
            var learnset = learn.GetLearnset(candidate.Species, candidate.Form);
            flags.Clear();
            if (!MarkChildMoveFlags(candidate.Species, candidate.Form, version, pending[..pendingCount], learnset, flags))
                continue;

            var baseMoves = learnset.GetBaseEggMoves(EggStateLegality.GetEggLevel(version.Generation));
            if (!TryValidateCore(candidate.Species, candidate.Form, version, pending[..pendingCount], baseMoves, flags, ref trace, depth))
                continue;
            return true;
        }

        return CanFatherKnowAllMovesPast(fatherSpecies, fatherForm, version, moves, ref trace, depth);
    }

    private static bool CanFatherKnowAllMovesPast(ushort fatherSpecies, byte fatherForm, GameVersion version,
        scoped ReadOnlySpan<ushort> moves,
        ref ChainBreedTrace trace, int depth)
    {
        // A father used in this generation could have been transferred from the immediately preceding generation with its complete moveset intact.
        // Re-run the possession proof in each relevant predecessor ruleset.
        // Recursing one generation at a time also permits the Gen3 -> Gen4 -> Gen5 path.
        Span<GameVersion> previous = stackalloc GameVersion[MaxTransferPredecessorVersions];
        int previousCount = GetTransferPredecessorVersions(version, previous);
        for (int i = 0; i < previousCount; i++)
        {
            trace.SetTransfer(depth, fatherSpecies, fatherForm, version, previous[i], moves);
            if (!CanFatherKnowAllMoves(fatherSpecies, fatherForm, previous[i], moves, ref trace, depth + 1))
                continue;

            // Transfer changes neither the number of breeding links nor the father species reported by the caller.
            trace.Commit(depth + 1);
            return true;
        }

        return false;
    }

    /// <summary>
    /// For storing the possible predecessor versions for transfer checks.
    /// This is a constant to avoid heap allocation of a list.
    /// </summary>
    /// <remarks>
    /// The following method should not exceed this value.
    /// </remarks>
    private const int MaxTransferPredecessorVersions = 2;

    private static int GetTransferPredecessorVersions(GameVersion version, Span<GameVersion> result)
    {
        // There is no main-series Gen2 -> Gen3 transfer path (only to Gen7).
        // Aggregate versions are intentional; each represents a distinct learn/personal ruleset already supported by the switch methods below.
        // Recursive entry to this method will result in lesser generations being checked in turn.
        switch (version.Generation)
        {
            case 4:
                result[0] = RSE;
                result[1] = FRLG;
                return 2;
            case 5:
                result[0] = HGSS;
                result[1] = DPPt;
                return 2;
            default:
                return 0;
        }
    }

    private static bool CanLearnDirectlyInLine(ushort species, byte form, GameVersion version, ushort move)
    {
        // Current ruleset only:
        // level-up, tm/hm, tutor, evolution-line sources, same-generation encounters.
        if (species == (ushort)Species.Smeargle)
            return MoveInfo.IsSketchValid(move, version.Context);

        var tree = EvolutionTree.GetEvolutionTree(version.Context);

        // Check backwards through pre-evolutions
        (ushort Species, byte Form) current = (species, form);
        while (true)
        {
            if (CanLearnDirectly(current.Species, current.Form, version, move))
                return true;

            ref readonly var node = ref tree.Reverse.GetReverse(current.Species, current.Form);
            var previous = node.First;
            if (previous.Species == 0)
                break;
            current = (previous.Species, previous.Form);

        }

        // Check forward through evolutions (e.g., Tyrogue -> Hitmonlee/Hitmonchan/Hitmontop)
        var evos = tree.Forward.GetEvolutions(species, form);
        foreach (var (evoSpecies, evoForm) in evos)
        {
            if (CanLearnDirectly(evoSpecies, evoForm, version, move))
                return true;
        }

        // Check cross-generation and special encounter sources
        if (CanLearnFromHistoricalSource(species, form, version, move))
            return true;

        return false;
    }

    private static bool CanLearnFromHistoricalSource(ushort species, byte form, GameVersion version, ushort move)
    {
        var generation = version.Generation;

        // Gen 3: Can use XD/Colo special encounters
        if (generation >= 3 && move <= Legal.MaxMoveID_3)
        {
            if (CanLearnDirectly(species, form, RS, move))
                return true;
            // Check XD Shadow Pokemon encounters as origin.
            if (CanLearnFromEncounterSpecial(Encounters3XD.Shadow, species, move))
                return true;
        }

        // Gen 5: Can transfer from Gen 3/4
        if (generation >= 5 && move <= Legal.MaxMoveID_4)
        {
            // Gen 4 TMs (e.g., Shellder + Avalanche via Gen4 TM72)
            // Check if the move is a Gen4-exclusive move learnable via TM in Gen4
            // Check all Gen4 versions for TM availability
            if (CanLearnDirectly(species, form, Pt, move) || CanLearnDirectly(species, form, HGSS, move))
                return true;
        }

        return false;
    }

    private static bool CanLearnFromEncounterSpecial<T>(ReadOnlySpan<T> source, ushort species, ushort move) where T : IMoveset, ISpeciesForm
    {
        foreach (var enc in source)
        {
            if (enc.Species != species)
                continue;
            if (enc.Moves.Contains(move))
                return true;
        }
        return false;
    }

    private static bool CanLearnDirectly(ushort species, byte form, GameVersion version, ushort move) => version switch
    {
        GD or SI or GS => CanLearnDirectly2(LearnSource2GS.Instance, species, form, move, false),
        C or GSC => CanLearnDirectly2(LearnSource2C.Instance, species, form, move, true),

        R or S or RS => CanLearnDirectly3(LearnSource3RS.Instance, species, form, move),
        E or RSE => CanLearnDirectly3(LearnSource3E.Instance, species, form, move),
        FR or FRLG => CanLearnDirectly3(LearnSource3FR.Instance, species, form, move),
        LG => CanLearnDirectly3(LearnSource3LG.Instance, species, form, move),

        D or P or DP => CanLearnDirectly4(LearnSource4DP.Instance, species, form, move, false),
        Pt or DPPt => CanLearnDirectly4(LearnSource4Pt.Instance, species, form, move, false),
        HG or SS or HGSS => CanLearnDirectly4(LearnSource4HGSS.Instance, species, form, move, true),

        B or W or BW => CanLearnDirectly5(LearnSource5BW.Instance, species, form, move),
        B2 or W2 or B2W2 => CanLearnDirectly5(LearnSource5B2W2.Instance, species, form, move),
        _ => false,
    };

    private static bool CanLearnDirectly2(ILearnSource<PersonalInfo2> source, ushort species, byte form, ushort move, bool crystal)
    {
        if (!source.TryGetPersonal(species, form, out var pi))
            return false;
        if (source.GetLearnset(species, form).GetIsLearn(move))
            return true;

        var tmIndex = PersonalInfo2.MachineMoves.IndexOf((byte)move);
        if (move <= Legal.MaxMoveID_2 && tmIndex >= 0 && pi.GetIsLearnTM(tmIndex))
            return true;

        var tutorIndex = PersonalInfo2.TutorMoves.IndexOf((byte)move);
        return crystal && tutorIndex >= 0 && pi.GetIsLearnTutorType(tutorIndex);
    }

    private static bool CanLearnDirectly3(ILearnSource<PersonalInfo3> source, ushort species, byte form, ushort move)
    {
        if (!source.TryGetPersonal(species, form, out var pi))
            return false;
        if (source.GetLearnset(species, form).GetIsLearn(move))
            return true;

        var tmIndex = PersonalInfo3.MachineMovesTechnical.IndexOf(move);
        if (tmIndex >= 0 && pi.TMHM[tmIndex])
            return true;

        var hmIndex = PersonalInfo3.MachineMovesHidden.IndexOf(move);
        if (hmIndex >= 0 && pi.TMHM[50 + hmIndex])
            return true;

        if (LearnSource3RS.GetIsTutor(species, move))
            return true; // XD
        if (LearnSource3E.GetIsTutorFRLG(species, move))
            return true; // FR/LG and Emerald
        if (LearnSource3E.GetIsSpecialTutor(species, move))
            return true; // Emerald

        return false;
    }

    private static bool CanLearnDirectly4(ILearnSource<PersonalInfo4> source, ushort species, byte form, ushort move, bool hgss)
    {
        if (!source.TryGetPersonal(species, form, out var pi))
            return false;
        if (source.GetLearnset(species, form).GetIsLearn(move))
            return true;

        var tmIndex = PersonalInfo4.MachineMovesTechnical.IndexOf(move);
        if (tmIndex >= 0 && pi.GetIsLearnTM(tmIndex))
            return true;

        var hms = hgss ? PersonalInfo4.MachineMovesHiddenHGSS : PersonalInfo4.MachineMovesHiddenDPPt;
        var hmIndex = hms.IndexOf(move);
        return hmIndex >= 0 && pi.GetIsLearnHM(hmIndex);
    }

    private static bool CanLearnDirectly5<T>(ILearnSource<T> source, ushort species, byte form, ushort move) where T : PersonalInfo
    {
        if (!source.TryGetPersonal(species, form, out var pi))
            return false;
        if (source.GetLearnset(species, form).GetIsLearn(move))
            return true;
        if (pi is not IPersonalInfoTM tm)
            return false;

        var tmIndex = PersonalInfo5BW.MachineMoves.IndexOf(move);
        return tmIndex >= 0 && tm.GetIsLearnTM(tmIndex);
    }

    // Can potentially move these to LearnSource2/3/4/5, but they are only used here and are not part of the public API.
    private static bool MarkChildMoveFlags(ushort species, byte form, GameVersion version, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags) => version switch
    {
        GD or SI or GS => MarkChildMoveFlags2(species, form, LearnSource2GS.Instance, PersonalTable.GS[species, form], version, moves, learnset, flags),
        C or GSC => MarkChildMoveFlags2(species, form, LearnSource2C.Instance, PersonalTable.C[species, form], version, moves, learnset, flags),

        // RSE are all the same learnsets for anything that can breed (only Deoxys differs between RS/E).
        R or S or E or RS or RSE => MarkChildMoveFlags3(species, form, LearnSource3E.Instance, PersonalTable.E[species, form], moves, learnset, flags),
        // FR/LG are all the same learnsets for anything that can breed (only Deoxys; Dugtrio has swapped move ordering: Sand-Attack is listed above Scratch only in LeafGreen).
        FR or LG or FRLG => MarkChildMoveFlags3(species, form, LearnSource3FR.Instance, PersonalTable.FR[species, form], moves, learnset, flags),

        D or P or DP => MarkChildMoveFlags4(species, form, LearnSource4DP.Instance, PersonalTable.DP[species, form], version, moves, learnset, flags),
        Pt or DPPt => MarkChildMoveFlags4(species, form, LearnSource4Pt.Instance, PersonalTable.Pt[species, form], version, moves, learnset, flags),
        HG or SS or HGSS => MarkChildMoveFlags4(species, form, LearnSource4HGSS.Instance, PersonalTable.HGSS[species, form], version, moves, learnset, flags),

        B or W or BW => MarkChildMoveFlags5(species, form, LearnSource5BW.Instance, PersonalTable.BW[species, form], moves, learnset, flags),
        B2 or W2 or B2W2 => MarkChildMoveFlags5(species, form, LearnSource5B2W2.Instance, PersonalTable.B2W2[species, form], moves, learnset, flags),
        _ => false,
    };

    private static bool MarkChildMoveFlags2(ushort species, byte form, ILearnSource source, PersonalInfo2 info, GameVersion version, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags)
    {
        bool inheritLevelUp = Breeding.GetCanInheritMoves(species);
        var baseMoves = learnset.GetBaseEggMoves(EggStateLegality.EggLevel23);
        var eggMoves = source.GetEggMoves(species, form);
        var tmMoves = PersonalInfo2.MachineMoves;
        var tutorMoves = PersonalInfo2.TutorMoves;

        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            byte value = 0;
            if (baseMoves.Contains(move))
                value |= FlagBase;
            if (move <= Legal.MaxMoveID_2)
            {
                if (eggMoves.Contains(move))
                    value |= FlagGeneral;
                if (info.GetIsLearnTM(tmMoves.IndexOf((byte)move)))
                    value |= FlagGeneral;
                if (inheritLevelUp && learnset.GetIsLearn(move))
                    value |= FlagLevelUp;
                if (version is C or GSC && info.GetIsLearnTutorType(tutorMoves.IndexOf((byte)move)))
                    value |= FlagGeneral;
            }
            if (value == 0)
                return false;
            flags[i] = value;
        }
        return true;
    }

    private static bool MarkChildMoveFlags3(ushort species, byte form, ILearnSource source, PersonalInfo3 info, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags)
    {
        bool inheritLevelUp = Breeding.GetCanInheritMoves(species);
        var baseMoves = learnset.GetBaseEggMoves(EggStateLegality.EggLevel23);
        var eggMoves = source.GetEggMoves(species, form);
        var tms = PersonalInfo3.MachineMovesTechnical;
        var hms = PersonalInfo3.MachineMovesHidden;
        var tmhm = info.TMHM;

        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            byte value = 0;
            if (baseMoves.Contains(move))
                value |= FlagBase;
            if (eggMoves.Contains(move))
                value |= FlagGeneral;
            if (inheritLevelUp && learnset.GetIsLearn(move))
                value |= FlagLevelUp;

            int tmIndex = tms.IndexOf(move);
            if (tmIndex != -1 && tmhm[tmIndex])
                value |= FlagGeneral;

            int hmIndex = hms.IndexOf(move);
            if (hmIndex != -1 && tmhm[50 + hmIndex])
                value |= FlagGeneral;

            if (value == 0)
                return false;
            flags[i] = value;
        }
        return true;
    }

    private static bool MarkChildMoveFlags4(ushort species, byte form, ILearnSource source, PersonalInfo4 info, GameVersion version, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags)
    {
        bool inheritLevelUp = Breeding.GetCanInheritMoves(species);
        var baseMoves = learnset.GetBaseEggMoves(EggStateLegality.EggLevel);
        var eggMoves = source.GetEggMoves(species, form);
        var tms = PersonalInfo4.MachineMovesTechnical;
        var hms = version is HG or SS or HGSS ? PersonalInfo4.MachineMovesHiddenHGSS : PersonalInfo4.MachineMovesHiddenDPPt;

        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            byte value = 0;
            if (baseMoves.Contains(move))
                value |= FlagBase;
            if (eggMoves.Contains(move))
                value |= FlagGeneral;
            if (inheritLevelUp && learnset.GetIsLearn(move))
                value |= FlagLevelUp;

            int tmIndex = tms.IndexOf(move);
            if (tmIndex != -1 && info.GetIsLearnTM(tmIndex))
                value |= FlagGeneral;

            int hmIndex = hms.IndexOf(move);
            if (hmIndex != -1 && info.GetIsLearnHM(hmIndex))
                value |= FlagGeneral;

            if (value == 0)
                return false;
            flags[i] = value;
        }
        return true;
    }

    private static bool MarkChildMoveFlags5(ushort species, byte form, ILearnSource source, IPersonalInfoTM info, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags)
    {
        bool inheritLevelUp = Breeding.GetCanInheritMoves(species);
        var baseMoves = learnset.GetBaseEggMoves(EggStateLegality.EggLevel);
        var eggMoves = source.GetEggMoves(species, form);
        var tms = PersonalInfo5BW.MachineMoves;

        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            byte value = 0;
            if (baseMoves.Contains(move))
                value |= FlagBase;
            if (eggMoves.Contains(move))
                value |= FlagGeneral;
            if (inheritLevelUp && learnset.GetIsLearn(move))
                value |= FlagLevelUp;

            int tmIndex = tms.IndexOf(move);
            if (tmIndex != -1 && info.GetIsLearnTM(tmIndex))
                value |= FlagGeneral;

            if (value == 0)
                return false;
            flags[i] = value;
        }
        return true;
    }

    private static bool IsValidBaseCount(int baseCount, ReadOnlySpan<ushort> moves, ReadOnlySpan<ushort> baseMoves, ReadOnlySpan<byte> flags)
    {
        if (baseMoves.Length < baseCount)
            return false;

        for (int i = 0; i < baseCount; i++)
        {
            if ((flags[i] & FlagBase) == 0)
                return false;

            var expected = baseMoves[baseMoves.Length - baseCount + i];
            if (moves[i] != expected)
                return false;
        }

        for (int i = baseCount; i < moves.Length; i++)
        {
            if ((flags[i] & (FlagLevelUp | FlagGeneral)) == 0)
                return false;

            if ((flags[i] & FlagBase) == 0)
                continue;

            int baseIndex = baseMoves.IndexOf(moves[i]);
            if (baseIndex == -1)
                continue;

            int min = moves.Length - baseMoves.Length + baseIndex;
            if (i < min + baseCount)
                return false;
        }

        return true;
    }

    private static bool IsCompatibleFather(PersonalInfo mother, PersonalInfo father)
    {
        if (!((EggGroup)father.EggGroup1).CanBreed)
            return false;
        if (father.Genderless || father.OnlyFemale)
            return false;
        return mother.SharesCommonEggGroup(father);
    }

    private static int GetEggSpeciesCandidates(ushort species, byte form, GameVersion version, Span<(ushort, byte)> result)
    {
        var tree = EvolutionTree.GetEvolutionTree(version.Context);
        (ushort Species, byte Form) current = (species, form);
        (ushort Species, byte Form) baseSpecies;
        (ushort Species, byte Form) split = default;

        while (true)
        {
            baseSpecies = current;
            if (split.Species == 0 && Breeding.IsSplitBreedNotBabySpecies(current.Species, version.Generation))
                split = current;

            ref readonly var node = ref tree.Reverse.GetReverse(current.Species, current.Form);
            var prev = node.First;
            if (prev.Species == 0)
                break;
            current = (prev.Species, prev.Form);
        }

        result[0] = baseSpecies;
        int count = 1;
        if (split.Species != 0 && split != baseSpecies)
            result[count++] = split;
        return count;
    }

    /// <summary>
    /// Gen 6+ games allow mothers to pass egg moves, allowing for fusing chains.
    /// </summary>
    private static bool IsValidRelaxed(ushort species, byte form, GameVersion version, ReadOnlySpan<ushort> moves, out ChainBreedSummary summary)
    {
        // Some restrictions still remain:
        // - Only-Female mothers can only pass level-up moves if any same-group father can learn all moves.
        // ~~ Example: [BD/SP] Smoochum w/ Powder Snow (level 4).
        // - Dual-Species (Volbeat/Illumise and Nidoran-M/Nidoran-F) must still have a compatible father for any level-up moves AND egg moves (if Mother cannot pass).
        // ~~ Example: Volbeat w/ Dizzy Punch & Lunge (Illumise cannot learn either, and no father can share both).
        // ~~ In Gen8+, Egg Moves can be shared from other fathers onto the male then used for breeding, thus leaving only level up moves required for both parents.

        summary = default;

        var table = GameData.GetPersonal(version);
        var pi = table[species, form];
        // Genderless offspring must arise from Ditto. Inheriting Level Up moves should not occur, and no egg moves are assigned.
        if (pi.Genderless)
            return true;

        var (mi, mother) = ResolveMother(species, form, version, table);
        // If the evolved form is female-only, it must breed with a compatible father
        if (!mi.OnlyFemale)
            return true;

        bool isMaleSplit = IsMaleOnlySplitBreed(species);
        bool ignoreEggMoves = version.Generation >= 8; // In Gen8+, egg move sharing
        bool checkBaby = species is not ((ushort)Species.Volbeat or (ushort)Species.NidoranM);

        // For male-only split breed species in Gen 6-7, we need to check if a single father can pass all moves
        // that the mother cannot learn as egg moves
        var learn = GameData.GetLearnSource(version);
        if (isMaleSplit && !ignoreEggMoves)
        {
            // Collect moves that mother cannot learn as egg moves
            Span<ushort> fatherMustPass = stackalloc ushort[MaxMoveCount];
            int fatherMoveCount = 0;

            foreach (var move in moves)
            {
                if (move == 0)
                    break;

                // Check if mother can learn this as an egg move
                var motherEggMoves = learn.GetEggMoves(mother.Species, mother.Form);
                if (!motherEggMoves.Contains(move))
                {
                    // Mother cannot learn this egg move, father must pass it
                    fatherMustPass[fatherMoveCount++] = move;
                }
            }

            // If father must pass any moves, check if a single father can pass them all
            if (fatherMoveCount > 0)
            {
                if (!CanSingleFatherPassAllMovesRelaxed(mi, fatherMustPass[..fatherMoveCount], table, version))
                    return false;
            }

            return true; // All moves can be passed
        }

        // Check if any of the moves are level-up moves that cannot be inherited

        // Check each move to see if it's a level-up move that has no compatible father
        foreach (var move in moves)
        {
            if (move == 0)
                break;

            // Check if this is a level-up move for the baby species or evolved species
            var babyLearnset = learn.GetLearnset(species, form);
            var evoLearnset = learn.GetLearnset(mother.Species, mother.Form);
            bool isLevelUpMove = (checkBaby && babyLearnset.TryGetLevelLearnMove(move, out _)) ||
                                 evoLearnset.TryGetLevelLearnMove(move, out _);

            if (!isLevelUpMove)
                continue; // Not a level-up move, so it can be inherited normally

            // This is a level-up move. Check if any compatible father can learn it.
            if (!CanAnyCompatibleFatherLearnMove(mi, move, table, learn))
                return false; // No compatible father can pass this level-up move
        }
        return true;
    }

    private static bool CanSingleFatherPassAllMovesRelaxed(PersonalInfo motherInfo, ReadOnlySpan<ushort> moves, IPersonalTable table, GameVersion version)
    {
        // Check if there exists a single father that can pass all the moves
        var maxSpecies = table.MaxSpeciesID;
        for (ushort fatherSpecies = 1; fatherSpecies <= maxSpecies; fatherSpecies++)
        {
            var baseFather = table[fatherSpecies];
            var formCount = baseFather.FormCount;
            for (byte fatherForm = 0; fatherForm < formCount; fatherForm++)
            {
                if (!table.IsPresentInGame(fatherSpecies, fatherForm))
                    continue;

                var fatherInfo = table[fatherSpecies, fatherForm];
                // Father must be in the same egg group
                if (!IsCompatibleFatherForBreeding(motherInfo, fatherInfo))
                    continue;

                // Check if this father can have all the moves
                if (CanFatherLearnAll(moves, version, fatherSpecies, fatherForm))
                    return true;
            }
        }

        return false; // No father can pass all the moves
    }

    private static bool CanFatherLearnAll(ReadOnlySpan<ushort> moves, GameVersion version, ushort fatherSpecies, byte fatherForm)
    {
        foreach (var move in moves)
        {
            if (move == 0)
                break;
            if (!CanLearnDirectlyInLine(fatherSpecies, fatherForm, version, move))
                return false;
        }
        return true;
    }

    private static bool CanAnyCompatibleFatherLearnMove(PersonalInfo motherInfo, ushort move, IPersonalTable table, ILearnSource learn)
    {
        // Check if any species in the mother's egg groups can learn this move
        var maxSpecies = table.MaxSpeciesID;
        for (ushort fatherSpecies = 1; fatherSpecies <= maxSpecies; fatherSpecies++)
        {
            var baseFather = table[fatherSpecies];
            var formCount = baseFather.FormCount;
            for (byte fatherForm = 0; fatherForm < formCount; fatherForm++)
            {
                if (!table.IsPresentInGame(fatherSpecies, fatherForm))
                    continue;

                var fatherInfo = table[fatherSpecies, fatherForm];

                // Father must be in the same egg group and not be Ditto (or genderless/female-only)
                if (!IsCompatibleFatherForMove(motherInfo, fatherInfo))
                    continue;

                // Check if this father can learn the move via level-up
                var fatherLearnset = learn.GetLearnset(fatherSpecies, fatherForm);
                if (fatherLearnset.TryGetLevelLearnMove(move, out _))
                    return true; // Found a compatible father that can learn this move
            }
        }

        return false; // No compatible father found
    }

    private static bool IsCompatibleFatherForMove(PersonalInfo mother, PersonalInfo father)
    {
        // Father can't be genderless or female-only
        if (father.Genderless || father.OnlyFemale)
            return false;

        // Father must share an egg group with the mother
        if (!mother.SharesCommonEggGroup(father))
            return false;

        return true;
    }

    private static bool IsCompatibleFatherForBreeding(PersonalInfo mother, PersonalInfo father)
    {
        // Father can't be genderless or female-only
        if (father.Genderless || father.OnlyFemale)
            return false;

        // Father must share at least one egg group with the mother
        if (mother.SharesCommonEggGroup(father))
            return true;

        return false; // No shared egg groups
    }
}
