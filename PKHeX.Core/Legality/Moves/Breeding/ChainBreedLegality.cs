using System;
using System.Text;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Verifies if a Generation 2-5 egg move set can be produced by a single compatible father chain.
/// </summary>
public static class ChainBreedLegality
{
    private const byte FlagBase = 1 << 0;
    private const byte FlagLevelUp = 1 << 1;
    private const byte FlagGeneral = 1 << 2;
    private const int MaxMoveCount = 4;
    // The longest known in-generation chain is five fathers. Leave room for
    // an evolution/baby-species transition without allowing unbounded search.
    private const int MaxChainDepth = 8;

    public static bool IsValid(ushort species, GameVersion version, params ReadOnlySpan<ushort> moves)
        => TryValidate(species, version, moves, out _);

    public static bool TryValidate(ushort species, GameVersion version, ReadOnlySpan<ushort> moves, out ChainBreedSummary summary)
    {
        summary = default;
        int count = moves.IndexOf((ushort)0);
        if (count == 0)
            return false;
        if (count == -1)
            count = moves.Length;
        if (count > MaxMoveCount)
            return false;

        var generation = version.Generation;
        if (generation is < 2 or > 5)
            return IsValidRelaxed(species, version, moves, out summary);

        var learn = GameData.GetLearnSource(version);
        var learnset = learn.GetLearnset(species, 0);
        var baseMoves = learnset.GetBaseEggMoves(GetEggLevel(generation));

        Span<byte> flags = stackalloc byte[count];
        if (!MarkChildMoveFlags(species, version, moves[..count], learnset, flags))
            return false;

        // For male-only split breed species (Volbeat/Nidoran-M) in Gen 2-5, we need special validation:
        // All EGG moves must come from a single father.
        // Level-up moves don't need this restriction (can breed with Ditto).
        var isMaleSplit = IsMaleOnlySplitBreed(species);
        if (isMaleSplit)
        {
            // Filter to only egg moves (not level-up moves)
            Span<ushort> eggMovesOnly = stackalloc ushort[MaxMoveCount];
            int eggMoveCount = 0;
            for (int i = 0; i < count; i++)
            {
                // Skip if this move can be obtained via level-up
                if ((flags[i] & FlagLevelUp) != 0)
                    continue;
                eggMovesOnly[eggMoveCount++] = moves[i];
            }

            // If there are egg moves, check if a single father can pass them all
            if (eggMoveCount > 0)
            {
                RemapSpeciesToMother(ref species);
                return CanSingleFatherPassAllMoves(species, version, eggMovesOnly[..eggMoveCount], out summary);
            }
        }

        Span<ChainQueryState> visited = stackalloc ChainQueryState[MaxChainDepth];
        return TryValidateCore(species, version, moves[..count], baseMoves, flags, visited, 0, out summary);
    }

    private static void RemapSpeciesToMother(ref ushort species)
    {
        if (species is (ushort)Species.NidoranM)
            species = (ushort)Species.NidoranF;
        if (species is (ushort)Species.Volbeat)
            species = (ushort)Species.Illumise;
    }

    private static bool IsMaleOnlySplitBreed(ushort species)
    {
        return species is (ushort)Species.NidoranM or (ushort)Species.Volbeat;
    }

    private static bool IsValidRelaxed(ushort species, GameVersion version, ReadOnlySpan<ushort> moves, out ChainBreedSummary summary)
    {
        // Gen 6+ games have relaxed breeding rules where most chains are valid as Mothers can now pass, allowing for fusing chains.
        // However, only-Female offspring still have some restrictions:
        // A Smoochum(no egg group; bred from Jynx, in the Human-like group) can't inherit Powder Snow (learned at level 4)
        // Jynx is Female only, and there are no other species of the Human-like group that can know Powder Snow.
        // So, it must breed with a male from the same egg group; that male must know the moves needed.

        // However, Confusion is valid because Alakazam (Human-like) can learn it and breed with Jynx.

        // Additionally, for male-only split breed species (Volbeat/Nidoran-M), in Gen 6-7:
        // - Mother (Illumise/Nidoran-F) can pass moves she learns as egg moves
        // - Father must pass moves the mother cannot learn
        // - In Gen 8+, egg move sharing means this restriction doesn't apply
        summary = default;

        var generation = version.Generation;
        var table = GameData.GetPersonal(version);
        if (!table.IsPresentInGame(species, 0))
            return true;

        var pi = table[species, 0];

        // Genderless species must breed with Ditto, but since they always must breed with Ditto,
        // they are already handled by the relaxed rules (no level-up moves can be inherited).
        if (pi.Genderless)
            return true;

        var isMaleSplit = IsMaleOnlySplitBreed(species);
        RemapSpeciesToMother(ref species);

        // For male-only split breed species in Gen 6-7, check if father can pass moves mother can't learn
        if (isMaleSplit && generation < 8)
        {
            return CanMotherAndFatherPassAllMoves(species, version, moves, out summary);
        }

        // Check if this is a baby Pokemon bred from a female-only species (Gen 8+ restriction)
        if (generation < 8)
            return true; // Gen 6-7: fully relaxed for non-male-split species

        var context = version.Context;
        var tree = EvolutionTree.GetEvolutionTree(context);
        var evolutions = tree.Forward.GetEvolutions(species, 0);

        foreach (var (evoSpecies, _) in evolutions)
        {
            if (!table.IsPresentInGame(evoSpecies, 0))
                continue;

            var evoPi = table[evoSpecies, 0];

            // If the evolved form is female-only, it must breed with a compatible father
            if (evoPi.OnlyFemale)
            {
                // Check if any of the moves are level-up moves that cannot be inherited
                var learn = GameData.GetLearnSource(version);

                // Check each move to see if it's a level-up move that has no compatible father
                foreach (var move in moves)
                {
                    if (move == 0)
                        break;

                    // Check if this is a level-up move for the baby species or evolved species
                    var babyLearnset = learn.GetLearnset(species, 0);
                    var evoLearnset = learn.GetLearnset(evoSpecies, 0);

                    bool isLevelUpMove = babyLearnset.TryGetLevelLearnMove(move, out _) || 
                                        evoLearnset.TryGetLevelLearnMove(move, out _);

                    if (!isLevelUpMove)
                        continue; // Not a level-up move, so it can be inherited normally

                    // This is a level-up move. Check if any compatible father can learn it.
                    if (!CanAnyCompatibleFatherLearnMove(evoPi, move, table, learn))
                        return false; // No compatible father can pass this level-up move
                }
            }
        }

        return true;
    }

    private static bool CanAnyCompatibleFatherLearnMove(IPersonalInfo motherInfo, ushort move, IPersonalTable table, ILearnSource learn)
    {
        // Check if any species in the mother's egg groups can learn this move
        var maxSpecies = table.MaxSpeciesID;
        for (ushort fatherSpecies = 1; fatherSpecies <= maxSpecies; fatherSpecies++)
        {
            if (!table.IsPresentInGame(fatherSpecies, 0))
                continue;

            var fatherInfo = table[fatherSpecies, 0];

            // Father must be in the same egg group and not be Ditto (or genderless/female-only)
            if (!IsCompatibleFatherForMove(motherInfo, fatherSpecies, fatherInfo))
                continue;

            // Check if this father can learn the move via level-up
            var fatherLearnset = learn.GetLearnset(fatherSpecies, 0);
            if (fatherLearnset.TryGetLevelLearnMove(move, out _))
                return true; // Found a compatible father that can learn this move
        }

        return false; // No compatible father found
    }

    private static bool IsCompatibleFatherForMove(IPersonalInfo mother, ushort fatherSpecies, IPersonalInfo father)
    {
        // Ditto can't pass down level-up moves
        if (fatherSpecies == (ushort)Species.Ditto)
            return false;

        // Father can't be genderless or female-only
        if (father.Genderless || father.OnlyFemale)
            return false;

        // Father must share an egg group with the mother
        if (!SharesEggGroup(mother.EggGroup1, mother.EggGroup2, father.EggGroup1))
            return false;
        if (!SharesEggGroup(mother.EggGroup1, mother.EggGroup2, father.EggGroup2))
            return false;

        return true;
    }

    private static bool CanSingleFatherPassAllMoves(ushort motherSpecies, GameVersion version, ReadOnlySpan<ushort> moves, out ChainBreedSummary summary)
    {
        // For male-only split breed species in Gen 2-5, all egg moves must come from a single father
        summary = default;
        var table = GameData.GetPersonal(version);
        if (!table.IsPresentInGame(motherSpecies, 0))
            return false;

        var motherInfo = table[motherSpecies, 0];
        var learn = GameData.GetLearnSource(version);
        var maxSpecies = table.MaxSpeciesID;

        // Try each potential father species
        for (ushort fatherSpecies = 1; fatherSpecies <= maxSpecies; fatherSpecies++)
        {
            if (!table.IsPresentInGame(fatherSpecies, 0))
                continue;

            var fatherInfo = table[fatherSpecies, 0];

            // Father must be in the same egg group and not be Ditto (or genderless/female-only)
            if (!IsCompatibleFatherForBreeding(motherInfo, fatherSpecies, fatherInfo))
                continue;

            // Check if this father can learn ALL the moves
            bool canLearnAll = true;

            foreach (var move in moves)
            {
                if (move == 0)
                    break;

                // Father must be able to learn this move as an egg move or level-up move
                if (!CanFatherLearnMoveForEgg(fatherSpecies, move, learn))
                {
                    canLearnAll = false;
                    break;
                }
            }

            if (canLearnAll)
            {
                summary = new ChainBreedSummary(motherSpecies, fatherSpecies, 1);
                return true;
            }
        }

        return false; // No single father can pass all moves
    }

    private static bool CanMotherAndFatherPassAllMoves(ushort motherSpecies, GameVersion version, ReadOnlySpan<ushort> moves, out ChainBreedSummary summary)
    {
        // For male-only split breed species in Gen 6-7:
        // - Mother can pass moves she learns as egg moves
        // - Father must pass moves the mother cannot learn
        summary = default;
        var table = GameData.GetPersonal(version);
        if (!table.IsPresentInGame(motherSpecies, 0))
            return false;

        var motherInfo = table[motherSpecies, 0];
        var learn = GameData.GetLearnSource(version);

        // Determine which moves the mother can learn as egg moves
        Span<bool> motherCanLearn = stackalloc bool[MaxMoveCount];
        Span<ushort> fatherMustPass = stackalloc ushort[MaxMoveCount];
        int fatherMoveCount = 0;

        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            if (move == 0)
                break;

            // Check if mother can learn this move as an egg move
            if (CanMotherLearnMoveAsEgg(motherSpecies, move, learn))
            {
                motherCanLearn[i] = true;
            }
            else
            {
                // Father must pass this move
                fatherMustPass[fatherMoveCount++] = move;
            }
        }

        // If mother can learn all moves, it's valid
        if (fatherMoveCount == 0)
        {
            summary = new ChainBreedSummary(motherSpecies, 0, 0);
            return true;
        }

        // Check if a single father can pass all the moves the mother cannot learn
        var maxSpecies = table.MaxSpeciesID;
        for (ushort fatherSpecies = 1; fatherSpecies <= maxSpecies; fatherSpecies++)
        {
            if (!table.IsPresentInGame(fatherSpecies, 0))
                continue;

            var fatherInfo = table[fatherSpecies, 0];

            // Father must be in the same egg group
            if (!IsCompatibleFatherForBreeding(motherInfo, fatherSpecies, fatherInfo))
                continue;

            // Check if this father can learn all the moves mother cannot learn
            bool canLearnAll = true;
            for (int i = 0; i < fatherMoveCount; i++)
            {
                var move = fatherMustPass[i];
                if (CanFatherLearnMoveForEgg(fatherSpecies, move, learn))
                    continue;
                canLearnAll = false;
                break;
            }

            if (canLearnAll)
            {
                summary = new ChainBreedSummary(motherSpecies, fatherSpecies, 1);
                return true;
            }
        }

        return false; // No father can pass all the moves mother cannot learn
    }

    private static bool CanFatherLearnMoveForEgg(ushort fatherSpecies, ushort move, ILearnSource learn)
    {
        // For male-only split breed validation, check if the father can pass this move to the child
        // The father can pass a move if he can HAVE it in his moveset via level-up only
        // (not via egg moves, since that would require another breeding chain)
        var learnset = learn.GetLearnset(fatherSpecies, 0);

        // Only check level-up
        return learnset.TryGetLevelLearnMove(move, out _);
    }

    private static bool CanMotherLearnMoveAsEgg(ushort motherSpecies, ushort move, ILearnSource learn)
    {
        // Check if the mother can learn this move as an egg move
        var eggMoves = learn.GetEggMoves(motherSpecies, 0);
        return eggMoves.Contains(move);
    }

    private static bool IsCompatibleFatherForBreeding(IPersonalInfo mother, ushort fatherSpecies, IPersonalInfo father)
    {
        // Ditto can't pass down egg moves
        if (fatherSpecies == (ushort)Species.Ditto)
            return false;

        // Father can't be genderless or female-only
        if (father.Genderless || father.OnlyFemale)
            return false;

        // Father must share at least one egg group with the mother
        if (SharesEggGroup(mother.EggGroup1, mother.EggGroup2, father.EggGroup1))
            return true;
        if (SharesEggGroup(mother.EggGroup1, mother.EggGroup2, father.EggGroup2))
            return true;

        return false; // No shared egg groups
    }

    private static bool TryValidateCore(ushort eggSpecies, GameVersion version, ReadOnlySpan<ushort> moves, ReadOnlySpan<ushort> baseMoves, ReadOnlySpan<byte> flags, Span<ChainQueryState> visited, int depth, out ChainBreedSummary summary)
    {
        summary = default;
        if ((uint)depth >= (uint)visited.Length)
            return false;

        var state = new ChainQueryState(eggSpecies, moves);
        // Check against visited[0] through visited[depth-1] for exact duplicates
        for (int i = 0; i < depth; i++)
        {
            if (visited[i].Equals(state))
                return false;
        }
        // Also check if we're about to overwrite the same state at visited[depth]
        // (happens when recursing at the same depth level via CanFatherKnowAllMoves)
        if ((uint)depth < (uint)visited.Length && visited[depth].Equals(state))
            return false;

        visited[depth] = state;

        int maxBase = Math.Min(moves.Length, baseMoves.Length);
        Span<ushort> inheritedMoves = stackalloc ushort[MaxMoveCount];
        return TryValidateBaseCounts(eggSpecies, version, moves, baseMoves, flags, inheritedMoves, maxBase, visited, depth, out summary);
    }

    private static bool TryValidateBaseCounts(ushort eggSpecies, GameVersion version, ReadOnlySpan<ushort> moves, ReadOnlySpan<ushort> baseMoves, ReadOnlySpan<byte> flags, Span<ushort> inheritedMoves, int maxBase, Span<ChainQueryState> visited, int depth, out ChainBreedSummary summary)
    {
        summary = default;
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
            for (int i = 0; i < inheritedCount; i++)
                inheritedMoves[i] = suffix[i];

            if (TryResolveInheritedSources(eggSpecies, version, inheritedMoves[..inheritedCount], suffixFlags, 0, visited, depth + 1, out summary))
                return true;
        }

        return false;
    }

    private static bool TryResolveInheritedSources(ushort eggSpecies, GameVersion version, ReadOnlySpan<ushort> moves, ReadOnlySpan<byte> flags, int index, Span<ChainQueryState> visited, int depth, out ChainBreedSummary summary)
    {
        if (index == moves.Length)
            return TryResolveFather(eggSpecies, version, moves, visited, depth, out summary);

        var flag = flags[index];
        if ((flag & FlagGeneral) != 0)
        {
            if (TryResolveInheritedSources(eggSpecies, version, moves, flags, index + 1, visited, depth, out summary))
                return true;
        }

        if ((flag & FlagLevelUp) != 0)
        {
            if (TryResolveInheritedSources(eggSpecies, version, moves, flags, index + 1, visited, depth, out summary))
                return true;
        }

        summary = default;
        return false;
    }

    private static bool TryResolveFather(ushort eggSpecies, GameVersion version, ReadOnlySpan<ushort> moves, Span<ChainQueryState> visited, int depth, out ChainBreedSummary summary)
    {
        summary = default;
        var table = GameData.GetPersonal(version);
        if (!table.IsPresentInGame(eggSpecies, 0))
            return false;

        var mother = table[eggSpecies, 0];

        // If the egg species can't breed (baby Pokemon like Tyrogue), check if its evolutions can act as fathers
        if (mother.Genderless || mother.OnlyMale || mother.EggGroup1 == (int)EggGroup.Undiscovered)
        {
            // Try to find evolved forms that can breed and produce this egg species
            return TryResolveFatherViaEvolution(eggSpecies, version, moves, visited, depth, out summary);
        }

        ushort maxSpecies = table.MaxSpeciesID;
        for (ushort fatherSpecies = 1; fatherSpecies <= maxSpecies; fatherSpecies++)
        {
            if (!table.IsPresentInGame(fatherSpecies, 0))
                continue;

            var father = table[fatherSpecies, 0];
            if (!IsCompatibleFather(mother, fatherSpecies, father))
                continue;

            if (!CanFatherKnowAllMoves(fatherSpecies, version, moves, visited, depth, out var chainDepth))
                continue;

            summary = new ChainBreedSummary(eggSpecies, fatherSpecies, chainDepth);
            return true;
        }

        return false;
    }

    private static bool TryResolveFatherViaEvolution(ushort eggSpecies, GameVersion version, ReadOnlySpan<ushort> moves, Span<ChainQueryState> visited, int depth, out ChainBreedSummary summary)
    {
        summary = default;
        var tree = EvolutionTree.GetEvolutionTree(version.Context);
        var evos = tree.Forward.GetEvolutions(eggSpecies, 0);

        foreach (var (evoSpecies, _) in evos)
        {
            if (!CanFatherKnowAllMoves(evoSpecies, version, moves, visited, depth, out var chainDepth))
                continue;

            summary = new ChainBreedSummary(eggSpecies, evoSpecies, chainDepth);
            return true;
        }

        return false;
    }

    private static bool CanFatherKnowAllMoves(ushort fatherSpecies, GameVersion version, ReadOnlySpan<ushort> moves, Span<ChainQueryState> visited, int depth, out byte chainDepth)
    {
        chainDepth = 1;
        Span<ushort> pending = stackalloc ushort[MaxMoveCount];
        int pendingCount = 0;
        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            if (!CanLearnDirectlyInLine(fatherSpecies, version, move))
                pending[pendingCount++] = move;
        }

        if (pendingCount == 0)
            return true;

        Span<ushort> eggSpecies = stackalloc ushort[2];
        int eggSpeciesCount = GetEggSpeciesCandidates(fatherSpecies, version, eggSpecies);
        Span<byte> flags = stackalloc byte[MaxMoveCount];
        for (int i = 0; i < eggSpeciesCount; i++)
        {
            var candidate = eggSpecies[i];
            if (candidate == 0)
                continue;

            var learn = GameData.GetLearnSource(version);
            var learnset = learn.GetLearnset(candidate, 0);
            flags.Clear();
            if (!MarkChildMoveFlags(candidate, version, pending[..pendingCount], learnset, flags))
                continue;

            var baseMoves = learnset.GetBaseEggMoves(GetEggLevel(version.Generation));
            if (!TryValidateCore(candidate, version, pending[..pendingCount], baseMoves, flags, visited, depth, out var nested))
                continue;

            chainDepth = (byte)(nested.ChainDepth + 1);
            return true;
        }

        return false;
    }

    private static bool CanLearnDirectlyInLine(ushort species, GameVersion version, ushort move)
    {
        if (species == (ushort)Species.Smeargle)
            return MoveInfo.IsSketchValid(move, version.Context);

        var tree = EvolutionTree.GetEvolutionTree(version.Context);

        // Check backwards through pre-evolutions
        ushort current = species;
        while (true)
        {
            if (CanLearnDirectly(current, version, move))
                return true;

            ref readonly var node = ref tree.Reverse.GetReverse(current, 0);
            var previous = node.First.Species;
            if (previous == 0)
                break;
            current = previous;
        }

        // Check forward through evolutions (e.g., Tyrogue -> Hitmonlee/Hitmonchan/Hitmontop)
        var evos = tree.Forward.GetEvolutions(species, 0);
        foreach (var (evoSpecies, _) in evos)
        {
            if (CanLearnDirectly(evoSpecies, version, move))
                return true;
        }

        // Check cross-generation and special encounter sources
        if (CanLearnFromHistoricalSource(species, version, move))
            return true;

        return false;
    }

    private static bool CanLearnFromHistoricalSource(ushort species, GameVersion version, ushort move)
    {
        var generation = version.Generation;

        // Gen 3: Can use XD/Colo special encounters
        if (generation == 3)
        {
            if (CanLearnFromGen3Special(species, move))
                return true;
        }

        // Gen 4: Can transfer from Gen 3 (including XD/Colo), but NOT native Gen4-only moves through XD
        if (generation == 4)
        {
            // Only allow Gen 3 XD/Colo moves (not Gen 4 moves)
            if (move <= Legal.MaxMoveID_3 && CanLearnFromGen3Special(species, move))
                return true;
        }

        // Gen 5: Can transfer from Gen 3/4
        if (generation == 5)
        {
            // Gen 4 TMs (e.g., Shellder + Avalanche via Gen4 TM72)
            // Check if the move is a Gen4-exclusive move learnable via TM in Gen4
            if (move is (> Legal.MaxMoveID_3 and <= Legal.MaxMoveID_4))
            {
                // Check all Gen4 versions for TM availability
                if (CanLearnDirectly(species, Pt, move) ||
                    CanLearnDirectly(species, HGSS, move))
                    return true;
            }

            // Gen 3 special encounters (XD/Colo)
            if (move <= Legal.MaxMoveID_3 && CanLearnFromGen3Special(species, move))
                return true;
        }

        return false;
    }

    private static bool CanLearnFromGen3Special(ushort species, ushort move)
    {
        // Check XD Shadow Pokemon encounters
        foreach (var enc in Encounters3XD.Shadow)
        {
            if (enc.Species == species)
            {
                var moves = enc.Moves.AsSpan();
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i] == 0)
                        break;
                    if (moves[i] == move)
                        return true;
                }
            }
        }

        return false;
    }

    private static bool CanLearnDirectly(ushort species, GameVersion version, ushort move) => version switch
    {
        GD or SI or GS => CanLearnDirectly2(LearnSource2GS.Instance, species, move, false),
        C or GSC => CanLearnDirectly2(LearnSource2C.Instance, species, move, true),

        R or S or RS => CanLearnDirectly3(LearnSource3RS.Instance, species, move),
        E or RSE => CanLearnDirectly3(LearnSource3E.Instance, species, move),
        FR or FRLG => CanLearnDirectly3(LearnSource3FR.Instance, species, move),
        LG => CanLearnDirectly3(LearnSource3LG.Instance, species, move),

        D or P or DP => CanLearnDirectly4(LearnSource4DP.Instance, species, move, false),
        Pt or DPPt => CanLearnDirectly4(LearnSource4Pt.Instance, species, move, false),
        HG or SS or HGSS => CanLearnDirectly4(LearnSource4HGSS.Instance, species, move, true),

        B or W or BW => CanLearnDirectly5(LearnSource5BW.Instance, species, move),
        B2 or W2 or B2W2 => CanLearnDirectly5(LearnSource5B2W2.Instance, species, move),
        _ => false,
    };

    private static bool CanLearnDirectly2(ILearnSource<PersonalInfo2> source, ushort species, ushort move, bool crystal)
    {
        if (!source.TryGetPersonal(species, 0, out var pi))
            return false;
        if (source.GetLearnset(species, 0).GetIsLearn(move))
            return true;

        var tmIndex = PersonalInfo2.MachineMoves.IndexOf((byte)move);
        if (move <= Legal.MaxMoveID_2 && tmIndex >= 0 && pi.GetIsLearnTM(tmIndex))
            return true;

        var tutorIndex = PersonalInfo2.TutorMoves.IndexOf((byte)move);
        return crystal && tutorIndex >= 0 && pi.GetIsLearnTutorType(tutorIndex);
    }

    private static bool CanLearnDirectly3(ILearnSource<PersonalInfo3> source, ushort species, ushort move)
    {
        if (!source.TryGetPersonal(species, 0, out var pi))
            return false;
        if (source.GetLearnset(species, 0).GetIsLearn(move))
            return true;

        var tmIndex = PersonalInfo3.MachineMovesTechnical.IndexOf(move);
        if (tmIndex >= 0 && pi.TMHM[tmIndex])
            return true;

        var hmIndex = PersonalInfo3.MachineMovesHidden.IndexOf(move);
        return hmIndex >= 0 && pi.TMHM[50 + hmIndex];
    }

    private static bool CanLearnDirectly4(ILearnSource<PersonalInfo4> source, ushort species, ushort move, bool hgss)
    {
        if (!source.TryGetPersonal(species, 0, out var pi))
            return false;
        if (source.GetLearnset(species, 0).GetIsLearn(move))
            return true;

        var tmIndex = PersonalInfo4.MachineMovesTechnical.IndexOf(move);
        if (tmIndex >= 0 && pi.GetIsLearnTM(tmIndex))
            return true;

        var hms = hgss ? PersonalInfo4.MachineMovesHiddenHGSS : PersonalInfo4.MachineMovesHiddenDPPt;
        var hmIndex = hms.IndexOf(move);
        return hmIndex >= 0 && pi.GetIsLearnHM(hmIndex);
    }

    private static bool CanLearnDirectly5<T>(ILearnSource<T> source, ushort species, ushort move) where T : PersonalInfo
    {
        if (!source.TryGetPersonal(species, 0, out var pi))
            return false;
        if (source.GetLearnset(species, 0).GetIsLearn(move))
            return true;
        if (pi is not IPersonalInfoTM tm)
            return false;

        var tmIndex = PersonalInfo5BW.MachineMoves.IndexOf(move);
        return tmIndex >= 0 && tm.GetIsLearnTM(tmIndex);
    }

    private static bool MarkChildMoveFlags(ushort species, GameVersion version, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags) => version switch
    {
        GD or SI or GS => MarkChildMoveFlags2(species, LearnSource2GS.Instance, PersonalTable.GS[species], version, moves, learnset, flags),
        C or GSC => MarkChildMoveFlags2(species, LearnSource2C.Instance, PersonalTable.C[species], version, moves, learnset, flags),

        R or S or RS => MarkChildMoveFlags3(species, LearnSource3RS.Instance, PersonalTable.RS[species], moves, learnset, flags),
        E or RSE or COLO or XD or CXD or EFL => MarkChildMoveFlags3(species, LearnSource3E.Instance, PersonalTable.E[species], moves, learnset, flags),
        FR or FRLG => MarkChildMoveFlags3(species, LearnSource3FR.Instance, PersonalTable.FR[species], moves, learnset, flags),
        LG => MarkChildMoveFlags3(species, LearnSource3LG.Instance, PersonalTable.LG[species], moves, learnset, flags),

        D or P or DP => MarkChildMoveFlags4(species, LearnSource4DP.Instance, PersonalTable.DP[species], version, moves, learnset, flags),
        Pt or DPPt => MarkChildMoveFlags4(species, LearnSource4Pt.Instance, PersonalTable.Pt[species], version, moves, learnset, flags),
        HG or SS or HGSS => MarkChildMoveFlags4(species, LearnSource4HGSS.Instance, PersonalTable.HGSS[species], version, moves, learnset, flags),

        B or W or BW => MarkChildMoveFlags5(species, LearnSource5BW.Instance, PersonalTable.BW[species], moves, learnset, flags),
        B2 or W2 or B2W2 => MarkChildMoveFlags5(species, LearnSource5B2W2.Instance, PersonalTable.B2W2[species], moves, learnset, flags),
        _ => false,
    };

    private static bool MarkChildMoveFlags2(ushort species, ILearnSource source, PersonalInfo2 info, GameVersion version, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags)
    {
        bool inheritLevelUp = Breeding.GetCanInheritMoves(species);
        var baseMoves = learnset.GetBaseEggMoves(GetEggLevel(2));
        var eggMoves = source.GetEggMoves(species, 0);
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

    private static bool MarkChildMoveFlags3(ushort species, ILearnSource source, PersonalInfo3 info, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags)
    {
        bool inheritLevelUp = Breeding.GetCanInheritMoves(species);
        var baseMoves = learnset.GetBaseEggMoves(GetEggLevel(3));
        var eggMoves = source.GetEggMoves(species, 0);
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

    private static bool MarkChildMoveFlags4(ushort species, ILearnSource source, PersonalInfo4 info, GameVersion version, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags)
    {
        bool inheritLevelUp = Breeding.GetCanInheritMoves(species);
        var baseMoves = learnset.GetBaseEggMoves(GetEggLevel(4));
        var eggMoves = source.GetEggMoves(species, 0);
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

    private static bool MarkChildMoveFlags5(ushort species, ILearnSource source, IPersonalInfoTM info, ReadOnlySpan<ushort> moves, Learnset learnset, Span<byte> flags)
    {
        bool inheritLevelUp = Breeding.GetCanInheritMoves(species);
        var baseMoves = learnset.GetBaseEggMoves(GetEggLevel(5));
        var eggMoves = source.GetEggMoves(species, 0);
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

    private static bool IsCompatibleFather(PersonalInfo mother, ushort fatherSpecies, PersonalInfo father)
    {
        if (fatherSpecies == (ushort)Species.Ditto)
            return false;
        if (father.Genderless || father.OnlyFemale)
            return false;
        return SharesEggGroup(mother, father);
    }

    private static bool SharesEggGroup(PersonalInfo left, PersonalInfo right)
    {
        return SharesEggGroup(left.EggGroup1, left.EggGroup2, right.EggGroup1)
            || SharesEggGroup(left.EggGroup1, left.EggGroup2, right.EggGroup2);
    }

    private static bool SharesEggGroup(int group1, int group2, int other)
    {
        if (!IsBreedGroup(other))
            return false;
        return group1 == other || group2 == other;
    }

    private static bool IsBreedGroup(int group) => group is not ((int)EggGroup.None or (int)EggGroup.Ditto or (int)EggGroup.Undiscovered);

    private static int GetEggSpeciesCandidates(ushort species, GameVersion version, Span<ushort> result)
    {
        var tree = EvolutionTree.GetEvolutionTree(version.Context);
        ushort current = species;
        ushort baseSpecies;
        ushort splitSpecies = 0;

        while (true)
        {
            baseSpecies = current;
            if (splitSpecies == 0 && Breeding.IsSplitBreedNotBabySpecies(current, version.Generation))
                splitSpecies = current;

            ref readonly var node = ref tree.Reverse.GetReverse(current, 0);
            var previous = node.First.Species;
            if (previous == 0)
                break;
            current = previous;
        }

        result[0] = baseSpecies;
        int count = 1;
        if (splitSpecies != 0 && splitSpecies != baseSpecies)
            result[count++] = splitSpecies;
        return count;
    }

    private static byte GetEggLevel(byte generation) => EggStateLegality.GetEggLevel(generation);

    private readonly record struct ChainQueryState
    {
        private readonly Species Species;
        private readonly byte Count;
        private readonly ushort Move1;
        private readonly ushort Move2;
        private readonly ushort Move3;
        private readonly ushort Move4;

        public ChainQueryState(ushort species, ReadOnlySpan<ushort> moves)
        {
            Species = (Species)species;
            Count = (byte)moves.Length;
            Move1 = moves.Length > 0 ? moves[0] : (ushort)0;
            Move2 = moves.Length > 1 ? moves[1] : (ushort)0;
            Move3 = moves.Length > 2 ? moves[2] : (ushort)0;
            Move4 = moves.Length > 3 ? moves[3] : (ushort)0;
        }

        public string Format(ReadOnlySpan<string> species, ReadOnlySpan<string> moves)
        {
            var sb = new StringBuilder();
            sb.Append(species[(int)Species]);
            sb.Append(" [");
            if (Count > 0) sb.Append(moves[Move1]);
            if (Count > 1) { sb.Append(", "); sb.Append(moves[Move2]); }
            if (Count > 2) { sb.Append(", "); sb.Append(moves[Move3]); }
            if (Count > 3) { sb.Append(", "); sb.Append(moves[Move4]); }
            sb.Append(']');
            return sb.ToString();
        }
    }
}

public readonly record struct ChainBreedSummary(ushort EggSpecies, ushort FatherSpecies, byte ChainDepth);
