using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.PokedexType8a;
using static PKHeX.Core.PokedexResearchTaskType8a;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.PLA"/>.
/// </summary>>
public sealed class PokedexSave8a(SAV8LA SaveFile, SCBlock block)
{
    private readonly PokedexSaveData SaveData = new(block.Data);

    public const int MAX_SPECIES = 981;
    public const int MAX_FORM = 120;

    private static PersonalTable8LA Personal => PersonalTable.LA;
    private const int MaxSpeciesID = Legal.MaxSpeciesID_8a;

    private const int DexInvalid = 0;

    public static ushort GetDexIndex(PokedexType8a which, ushort species)
    {
        // Check species is valid
        if (species > MaxSpeciesID)
            return DexInvalid;

        // Check each form
        var table = Personal;
        var formCount = table[species].FormCount;
        for (byte form = 0; form < formCount; form++)
        {
            var entry = table.GetFormEntry(species, form);
            if (entry.DexIndexHisui == 0)
                continue;

            // If we're getting for Hisui dex, return the index
            if (which == Hisui)
                return entry.DexIndexHisui;

            // Otherwise, check the local dex index
            var localIndex = GetLocalIndex(which, entry);

            // Return the index if non-zero
            if (localIndex != 0)
                return localIndex;
        }

        // No valid index
        return DexInvalid;
    }

    private static ushort GetLocalIndex(PokedexType8a which, PersonalInfo8LA entry) => which switch
    {
        Local1 => entry.DexIndexLocal1,
        Local2 => entry.DexIndexLocal2,
        Local3 => entry.DexIndexLocal3,
        Local4 => entry.DexIndexLocal4,
        Local5 => entry.DexIndexLocal5,
        _ => throw new ArgumentOutOfRangeException(nameof(which)),
    };

    public bool IsPokedexCompleted(PokedexType8a which) => SaveData.IsPokedexCompleted(which);

    public bool IsPokedexPerfect(PokedexType8a which) => SaveData.IsPokedexPerfect(which);

    public static int GetDexTotalCount(PokedexType8a which)
    {
        var count = 0;
        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            if (GetDexIndex(which, species) != DexInvalid)
                count++;
        }
        return count;
    }

    public int GetDexTotalEverBeenUpdated()
    {
        var count = 0;
        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            if (SaveData.GetResearchEntry(species).HasEverBeenUpdated)
                count++;
        }
        return count;
    }

    public int GetDexGetCount(PokedexType8a which, out bool all)
    {
        all = true;
        var count = 0;
        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            if (GetDexIndex(which, species) == DexInvalid)
                continue;

            if (SaveData.GetPokeGetCount(species) > 0)
                count++;
            else
                all = false;
        }
        return count;
    }

    public int GetDexGetCount(PokedexType8a which) => GetDexGetCount(which, out _);

    public int GetPokeGetCount(ushort species) => species < MAX_SPECIES ? SaveData.GetPokeGetCount(species) : 0;

    public bool GetPokeHasAnyReport(ushort species) => species < MAX_SPECIES && SaveData.HasAnyReport(species);

    public int GetCompletePokeAnyDexNum()
    {
        var complete = 0;
        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            if (IsComplete(species))
                complete++;
        }
        return complete;
    }

    public int GetPokeResearchRate(ushort species)
    {
        if (species >= MAX_SPECIES)
            return 0;

        var rawRate = SaveData.GetPokeResearchRate(species);
        if (rawRate >= 100 && !IsAllRequiredTasksComplete(species))
            rawRate = 99;

        return rawRate;
    }

    public bool IsComplete(ushort species) => GetPokeResearchRate(species) >= 100;

    public bool IsPerfect(ushort species) => SaveData.IsPerfect(species);

    public int GetUpdateIndex(ushort species) => SaveData.GetResearchEntry(species).UpdateCounter;
    public int GetLastReportedIndex(ushort species) => SaveData.GetResearchEntry(species).LastUpdatedReportCounter;

    public int GetCompletePokeNum()
    {
        var complete = 0;
        for (ushort species = 0; species <= MaxSpeciesID; species++)
        {
            if (GetDexIndex(Hisui, species) != 0 && IsComplete(species))
                complete++;
        }
        return complete;
    }

    public int GetReportPokeNum()
    {
        var count = 0;

        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            // Only allow reports of Pokémon in Hisui dex
            if (GetDexIndex(Hisui, species) == 0)
                continue;

            // Only allow reports of Pokémon which have been caught
            if (SaveData.GetPokeGetCount(species) == 0)
                continue;

            // Check if the Pokémon is unreported or has unreported tasks.
            if (!SaveData.HasAnyReport(species) || GetUnreportedTaskCount(species) > 0)
                count++;
        }

        return count;
    }

    public int GetTotalReportNum()
    {
        var count = 0;

        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            // Only allow reports of Pokémon which have been caught
            if (SaveData.GetPokeGetCount(species) == 0)
                continue;

            count += GetUnreportedTaskCount(species);
        }

        return count;
    }

    public int GetUnreportedTaskCount(ushort species)
    {
        if (species >= MAX_SPECIES)
            return 0;

        if (!TryGetResearchTasks(species, out var tasks))
            return 0;

        var unreported = 0;
        for (var i = 0; i < tasks.Length; i++)
        {
            unreported += GetResearchTaskLevel(species, i, out _, out _, out _);
        }

        return unreported;
    }

    public void UpdateAllReportPoke() => UpdateAllReportPoke(out _);

    public void UpdateAllReportPoke(out PokedexUpdateInfo8a outInfo) => UpdateSpecificReportPoke(out outInfo, GetAll());

    public void UpdateSpecificReportPoke(ushort species) => UpdateSpecificReportPoke(species, out _);
    public void UpdateSpecificReportPoke(ushort species, out PokedexUpdateInfo8a outInfo) => UpdateSpecificReportPoke(out outInfo, GetAll(species));

    private static IEnumerable<ushort> GetAll(ushort species)
    {
        yield return species;
    }

    private static IEnumerable<ushort> GetAll()
    {
        for (ushort i = 0; i < MaxSpeciesID; i++)
            yield return i;
    }

    private void UpdateSpecificReportPoke(out PokedexUpdateInfo8a outInfo, IEnumerable<ushort> speciesToUpdate)
    {
        // Get the save file's rank.
        var urankBlock = SaveFile.Accessor.GetBlock(SaveBlockAccessor8LA.KExpeditionTeamRank);
        var uRankBeforeUpdate = (int)(uint)urankBlock.GetValue();

        // Get the points for the current/next rank.
        var pointsForCurRank = GetResearchPoint(uRankBeforeUpdate);
        var pointsForNextRank = GetResearchPoint(uRankBeforeUpdate + 1);

        // Get the total research points before update.
        var allPokeResearchPoint = GetAllPokeResearchPoint();
        var totalResearchPointBeforeUpdate = GetTotalResearchPoint();
        if (allPokeResearchPoint > totalResearchPointBeforeUpdate)
            totalResearchPointBeforeUpdate = allPokeResearchPoint;

        // Determine how many points count towards our next rank.
        bool canAchieveNextRank = pointsForNextRank >= totalResearchPointBeforeUpdate;
        var pointsNeededForNextRankBeforeUpdate = canAchieveNextRank ? pointsForNextRank - totalResearchPointBeforeUpdate : 0;

        // Declare variables we'll be processing for update
        var updatedReportCounter = false;
        var newlyCompleteResearchCount = 0;
        var tasksReported = 0;
        var pointsGainedFromCompletingPokeTasks = 0;
        var numPokesWithNewlyCompletedTasks = 0;

        // Iterate, processing all species.
        foreach (ushort species in speciesToUpdate)
        {
            // Only process species with dex ids
            if (GetDexIndex(Hisui, species) == 0)
                continue;

            // Get the species research entry
            var researchEntry = SaveData.GetResearchEntry(species);

            // Set that the species now has at least one report
            var hasNewReportInfo = !researchEntry.HasAnyReport;
            researchEntry.HasAnyReport = true;

            // Get research tasks for the species
            var hasTasks = TryGetResearchTasks(species, out var tasks);

            // Ensure all tasks have at least progress 1 (no tasks complete).
            if (hasTasks)
            {
                for (var taskId = 0; taskId < tasks!.Length; taskId++)
                {
                    if (researchEntry.GetReportedResearchProgress(taskId) == 0)
                        researchEntry.SetReportedResearchProgress(taskId, 1);
                }
            }

            // Get the research rate before report.
            var curSpeciesResearchRateBeforeReport = researchEntry.ResearchRate;
            if (researchEntry.ResearchRate >= 100 && !IsAllRequiredTasksComplete(species))
                curSpeciesResearchRateBeforeReport = 99;

            // Determine points gained for the current species in this report
            var totalPointsGainedForCurPoke = 0;
            var totalProgressForCurPoke = 0;
            if (hasTasks)
            {
                for (var taskId = 0; taskId < tasks!.Length; taskId++)
                {
                    // Get the current task
                    var task = tasks[taskId];

                    // Get the number of unreported tasks for the current task
                    var unreportedTasks = GetResearchTaskLevel(species, taskId, out _, out _, out _);

                    // NOTE: Here, the game sets this->UnreportedProgressInLastReport[species] to include unreportedTasks in 3-bit entries as usual.

                    // Set the updated progress.
                    for (var i = 0; i < unreportedTasks; i++)
                    {
                        var oldProgress = researchEntry.GetReportedResearchProgress(taskId);
                        var newProgress = oldProgress + 1;
                        if (oldProgress <= 6 && newProgress <= 6)
                            researchEntry.SetReportedResearchProgress(taskId, newProgress);
                    }

                    // Determine earned points/progress
                    totalPointsGainedForCurPoke += unreportedTasks * (task.PointsSingle + task.PointsBonus);
                    totalProgressForCurPoke += unreportedTasks;
                    hasNewReportInfo |= unreportedTasks > 0;
                }
            }

            // Update Research Rate
            var curSpeciesResearchRateAfterReport = (ushort)Math.Min(researchEntry.ResearchRate + totalPointsGainedForCurPoke, PokedexConstants8a.MaxPokedexResearchPoints);
            researchEntry.ResearchRate = curSpeciesResearchRateAfterReport;

            // If we complete a poke this report, add to newly complete research
            if (curSpeciesResearchRateBeforeReport < 100 && curSpeciesResearchRateAfterReport >= 100 && IsAllRequiredTasksComplete(species))
                newlyCompleteResearchCount++;

            // If we earned any points, update our global progress
            if (totalPointsGainedForCurPoke > 0)
            {
                tasksReported += totalProgressForCurPoke;
                pointsGainedFromCompletingPokeTasks += totalPointsGainedForCurPoke;
            }

            // If we have anything updated to report, update our progress
            if (hasNewReportInfo)
            {
                // Update global progress
                numPokesWithNewlyCompletedTasks++;

                // Update the global report counter if needed.
                if (!updatedReportCounter)
                {
                    SaveData.IncrementReportCounter();
                    updatedReportCounter = true;
                }

                // Set the last-updated report for the current entry to the current report counter
                researchEntry.LastUpdatedReportCounter = SaveData.GetReportCounter();
            }

            // Check if the entry is now perfect
            var perfect = hasTasks;
            if (hasTasks)
            {
                for (var taskId = 0; taskId < tasks!.Length; taskId++)
                {
                    var progress = researchEntry.GetReportedResearchProgress(taskId);
                    if (tasks[taskId].TaskThresholds.Length + 1 <= progress)
                        continue;

                    perfect = false;
                    break;
                }
            }

            if (perfect)
                researchEntry.IsPerfect = true;
        }

        // Make complete flags reflect the newly reported research
        UpdateAllCompleteFlags();

        // For unknown reasons, the game calls GetAllPokeResearchPoint() here, discarding the value.
        GetAllPokeResearchPoint();

        // Determine points after update
        var totalResearchPointAfterUpdate = totalResearchPointBeforeUpdate + pointsGainedFromCompletingPokeTasks + (100 * newlyCompleteResearchCount);

        // Determine points needed for next rank after update
        var pointsNeededForNextRankAfterUpdate = pointsForNextRank >= totalResearchPointAfterUpdate ? pointsForNextRank - totalResearchPointAfterUpdate : 0;

        // Determine percentage to next rank before/after update
        var percentToNextRankBeforeUpdate = 100;
        var percentToNextRankAfterUpdate = 100;
        if (pointsForNextRank > 0)
        {
            var pointsBetweenCurrentAndNextRank = pointsForNextRank - pointsForCurRank;

            if (pointsForNextRank > totalResearchPointBeforeUpdate)
            {
                percentToNextRankBeforeUpdate = 0;
                if (pointsBetweenCurrentAndNextRank > 0 && totalResearchPointBeforeUpdate > pointsForCurRank)
                    percentToNextRankBeforeUpdate = (100 * (totalResearchPointBeforeUpdate - pointsForCurRank)) / pointsBetweenCurrentAndNextRank;
            }

            if (pointsForNextRank > totalResearchPointAfterUpdate)
            {
                percentToNextRankAfterUpdate = 0;
                if (pointsBetweenCurrentAndNextRank > 0 && totalResearchPointAfterUpdate > pointsForCurRank)
                    percentToNextRankAfterUpdate = (100 * (totalResearchPointAfterUpdate - pointsForCurRank)) / pointsBetweenCurrentAndNextRank;
            }
        }

        // Set output
        outInfo = new PokedexUpdateInfo8a
        {
            ProgressPokeNum = numPokesWithNewlyCompletedTasks,
            ProgressNum = tasksReported,
            PointsGainedFromProgressPoke = pointsGainedFromCompletingPokeTasks,
            NewCompleteResearchNum = newlyCompleteResearchCount,
            PointsGainedFromCompleteResearch = 100 * newlyCompleteResearchCount,
            TotalResearchPointBeforeUpdate = totalResearchPointBeforeUpdate,
            TotalResearchPointAfterUpdate = totalResearchPointAfterUpdate,
            RankBeforeUpdate = uRankBeforeUpdate,
            PointsNeededForNextRankBeforeUpdate = pointsNeededForNextRankBeforeUpdate,
            PointsNeededForNextRankAfterUpdate = pointsNeededForNextRankAfterUpdate,
            ProgressPercentToNextRankBeforeUpdate = percentToNextRankBeforeUpdate,
            ProgressPercentToNextRankAfterUpdate = percentToNextRankAfterUpdate,
            TotalResearchPointAfterUpdate_Duplicate = totalResearchPointAfterUpdate,
        };

        // Update total research point
        SaveData.SetTotalResearchPoint(Math.Min(totalResearchPointAfterUpdate, 99999));
    }

    public int GetAllPokeResearchPoint()
    {
        var allPokeResearchPoint = 0;

        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            // Only return Pokémon with all required tasks complete
            if (!IsAllRequiredTasksComplete(species))
                continue;

            // On hitting 100 points/rate, 100 bonus points are awarded for completion
            var rate = GetPokeResearchRate(species);
            if (rate >= 100)
                rate = 200;

            allPokeResearchPoint += rate;
        }

        return allPokeResearchPoint;
    }

    public int GetTotalResearchPoint() => SaveData.GetTotalResearchPoint();

    public int GetResearchTaskLevel(ushort species, int taskIndex, out int reportedLevel, out int curValue, out int unreportedLevel)
    {
        // Default to all zeroes
        reportedLevel = 0;
        curValue = 0;
        unreportedLevel = 0;

        // If no tasks, don't continue.
        if (!TryGetResearchTasks(species, out var tasks))
            return unreportedLevel - reportedLevel;

        // Check that task is in bounds
        if ((uint)taskIndex >= tasks.Length)
            throw new ArgumentOutOfRangeException(nameof(taskIndex));

        // Get the species research entry
        var speciesEntry = SaveData.GetResearchEntry(species);

        // Get the task parameter.
        var task = tasks[taskIndex];
        curValue = GetCurrentResearchLevel(species, task, speciesEntry);

        // Get reported level.
        reportedLevel = speciesEntry.GetReportedResearchProgress(taskIndex);
        if (reportedLevel == 0)
            reportedLevel = 1;

        // Get the unreported level
        unreportedLevel = 1;
        foreach (var levelThreshold in task.TaskThresholds)
        {
            if (curValue < levelThreshold)
                break;
            unreportedLevel++;
        }

        // NOTE: Game does not do this, but they have monotonic increase of value.
        if (unreportedLevel < reportedLevel)
            unreportedLevel = reportedLevel;

        return unreportedLevel - reportedLevel;
    }

    private int GetCurrentResearchLevel(ushort species, PokedexResearchTask8a task, PokedexSaveResearchEntry speciesEntry) => task.Task switch
    {
        ObtainForms  => GetObtainedFormCounts(species),
        PartOfArceus => GetPartOfArceusValue(task.Hash_08),
        SpeciesQuest => GetSpeciesQuestState(task.Hash_06) == 0xFF ? 1 : 0,
        _ => speciesEntry.GetCurrentResearchLevel(task.Task, task.Index),
    };

    // Find all forms obtained (including gender variants)
    public int GetObtainedFormCounts(ushort species, int overridePack = -1)
    {
        int count = 0;
        var formCount = Personal[species].FormCount;
        for (byte form = 0; form < formCount; form++)
        {
            var mash = (ushort)(species | (form << 11));
            var index = PokedexConstants8a.PokemonInfoIds.BinarySearch(mash);
            if (index < 0)
                continue;

            if (!SaveData.TryGetStatisticsEntry(species, form, out var statEntry))
                continue;

            var obtainFlags = statEntry.ObtainFlags;
            if (overridePack >= 0 && form == (overridePack & 0xFF))
                obtainFlags = (byte)(overridePack >> 16);

            var g0 = (obtainFlags & 0x55) != 0;
            var g1 = (obtainFlags & 0xAA) != 0;

            var genderPack = PokedexConstants8a.PokemonInfoGenders[index];
            if ((genderPack & (1 << 0)) != 0 && g0) count++;
            if ((genderPack & (1 << 1)) != 0 && g1) count++;
            if ((genderPack & (1 << 2)) != 0 && g0) count++;
            if ((genderPack & (1 << 3)) != 0 && (g0 || g1)) count++;
            if ((genderPack & (1 << 4)) != 0 && g0) count++;
            if ((genderPack & (1 << 5)) != 0 && g1) count++;
        }
        return count;
    }

    public static bool HasMultipleGenders(ushort species)
    {
        var formCount = Personal[species].FormCount;
        for (var form = 0; form < formCount; form++)
        {
            var mash = (ushort)(species | (form << 11));
            var index = PokedexConstants8a.PokemonInfoIds.BinarySearch(mash);
            if (index < 0)
                continue;

            var genderPack = PokedexConstants8a.PokemonInfoGenders[index];
            if ((genderPack & 0x0B) != 0)
                return true;
        }

        return false;
    }

    private void UpdateAllCompleteFlags()
    {
        var hisuiComplete = true;
        var hisuiPerfect = true;
        Span<bool> localComplete = [true, true, true, true, true];
        Span<bool> localPerfect = [true, true, true, true, true];

        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            var dexHisui = GetDexIndex(Hisui, species);
            if (dexHisui == DexInvalid)
                continue;

            var dexLocal1 = GetDexIndex(Local1, species);
            var dexLocal2 = GetDexIndex(Local2, species);
            var dexLocal3 = GetDexIndex(Local3, species);
            var dexLocal4 = GetDexIndex(Local4, species);
            var dexLocal5 = GetDexIndex(Local5, species);

            if (!IsComplete(species))
            {
                hisuiComplete = false;
                if (dexLocal1 != DexInvalid) localComplete[0] = false;
                if (dexLocal2 != DexInvalid) localComplete[1] = false;
                if (dexLocal3 != DexInvalid) localComplete[2] = false;
                if (dexLocal4 != DexInvalid) localComplete[3] = false;
                if (dexLocal5 != DexInvalid) localComplete[4] = false;
            }

            if (!IsPerfect(species))
            {
                hisuiPerfect = false;
                if (dexLocal1 != DexInvalid) localPerfect[0] = false;
                if (dexLocal2 != DexInvalid) localPerfect[1] = false;
                if (dexLocal3 != DexInvalid) localPerfect[2] = false;
                if (dexLocal4 != DexInvalid) localPerfect[3] = false;
                if (dexLocal5 != DexInvalid) localPerfect[4] = false;
            }
        }

        // Set hisui/local complete/perfect flags.
        if (hisuiComplete) SaveData.SetPokedexCompleted(Hisui);
        if (localComplete[0]) SaveData.SetPokedexCompleted(Local1);
        if (localComplete[1]) SaveData.SetPokedexCompleted(Local2);
        if (localComplete[2]) SaveData.SetPokedexCompleted(Local3);
        if (localComplete[3]) SaveData.SetPokedexCompleted(Local4);
        if (localComplete[4]) SaveData.SetPokedexCompleted(Local5);

        if (hisuiPerfect) SaveData.SetPokedexPerfect(Hisui);
        if (localPerfect[0]) SaveData.SetPokedexPerfect(Local1);
        if (localPerfect[1]) SaveData.SetPokedexPerfect(Local2);
        if (localPerfect[2]) SaveData.SetPokedexPerfect(Local3);
        if (localPerfect[3]) SaveData.SetPokedexPerfect(Local4);
        if (localPerfect[4]) SaveData.SetPokedexPerfect(Local5);
    }

    public static int GetResearchPoint(int rank)
    {
        var table = PokedexConstants8a.ResearchPointsForRank;
        if ((uint)rank >= table.Length)
            return 0;

        return table[rank];
    }

    public int GetCurrentReportCounter() => SaveData.GetReportCounter();

    public void SetPokeSeenInWild(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        SetPokeHasBeenUpdated(pk.Species);

        SetPokeSeenInWildImpl(pk);
    }

    public void SetPokeHasBeenUpdated(ushort species)
    {
        if (species >= MAX_SPECIES)
            return;

        var entry = SaveData.GetResearchEntry(species);
        entry.HasEverBeenUpdated = true;

        if (entry.UpdateCounter == 0)
            entry.UpdateCounter = SaveData.NextUpdateCounter();
    }

    private void SetPokeSeenInWildImpl(PKM pk)
    {
        if (pk.IsEgg)
            return;

        if (!SaveData.TryGetStatisticsEntry(pk, out var statEntry, out var shift))
            return;

        statEntry.SeenInWildFlags |= (byte)(1 << shift);

        if (GetPokeGetCount(pk.Species) == 0)
            SetSelectedGenderForm(pk);
    }

    private void IncreaseResearchTaskProgress(ushort species, PokedexResearchTaskType8a task, ushort delta, bool doIncrease = true, int idx = -1)
    {
        // Only perform research updates for valid species
        if (species >= MAX_SPECIES)
            return;

        // All research increases set the update flag whether they increment the value
        SetPokeHasBeenUpdated(species);

        // If we shouldn't, don't do the update
        if (!doIncrease)
            return;

        // Get the research entry
        var researchEntry = SaveData.GetResearchEntry(species);

        // Increase the research value
        researchEntry.IncreaseCurrentResearchLevel(task, idx, delta);
    }

    public bool GetResearchTaskProgressByForce(ushort species, PokedexResearchTaskType8a type, int idx, out int curValue)
    {
        curValue = 0;

        if (species >= MAX_SPECIES)
            return false;

        // Get the species research value
        curValue = SaveData.GetResearchEntry(species).GetCurrentResearchLevel(type, idx);
        return true;
    }

    public void SetResearchTaskProgressByForce(ushort species, PokedexResearchTaskType8a task, int value, int idx)
    {
        // Only perform research updates for valid species
        if (species >= MAX_SPECIES)
            return;

        // All research increases set the update flag whether they increment the value
        SetPokeHasBeenUpdated(species);

        // Get the research entry
        var researchEntry = SaveData.GetResearchEntry(species);

        // Set the research value
        researchEntry.SetCurrentResearchLevel(task, idx, value);
    }

    public void SetResearchTaskProgressByForce(ushort species, PokedexResearchTask8a task, int value)
        => SetResearchTaskProgressByForce(species, task.Task, value, task.Index);

    private void IncrementResearchTaskProgress(ushort species, PokedexResearchTaskType8a task, bool doIncrease = true, int idx = -1)
        => IncreaseResearchTaskProgress(species, task, 1, doIncrease, idx);

    public bool HasPokeEverBeenUpdated(ushort species)
    {
        if (species >= MAX_SPECIES)
            return false;

        return SaveData.GetResearchEntry(species).HasEverBeenUpdated;
    }

    public byte GetPokeSeenInWildFlags(ushort species, byte form) => SaveData.TryGetStatisticsEntry(species, form, out var statEntry) ? statEntry.SeenInWildFlags : (byte)0;
    public byte GetPokeObtainFlags(ushort species, byte form) => SaveData.TryGetStatisticsEntry(species, form, out var statEntry) ? statEntry.ObtainFlags : (byte)0;
    public byte GetPokeCaughtInWildFlags(ushort species, byte form) => SaveData.TryGetStatisticsEntry(species, form, out var statEntry) ? statEntry.CaughtInWildFlags : (byte)0;

    public void SetPokeSeenInWildFlags(ushort species, byte form, byte flags)
    {
        if (SaveData.TryGetStatisticsEntry(species, form, out var statEntry))
        {
            statEntry.SeenInWildFlags = flags;
        }
    }
    public void SetPokeObtainFlags(ushort species, byte form, byte flags)
    {
        if (SaveData.TryGetStatisticsEntry(species, form, out var statEntry))
        {
            statEntry.ObtainFlags = flags;
        }
    }
    public void SetPokeCaughtInWildFlags(ushort species, byte form, byte flags)
    {
        if (SaveData.TryGetStatisticsEntry(species, form, out var statEntry))
        {
            statEntry.CaughtInWildFlags = flags;
        }
    }

    public bool HasAnyPokeSeenInWildFlags(ushort species, byte form) => GetPokeSeenInWildFlags(species, form) != 0;

    public bool HasAnyPokeObtainFlags(ushort species, byte form) => GetPokeObtainFlags(species, form) != 0;

    public bool HasAnyPokeCaughtInWildFlags(ushort species, byte form) => GetPokeCaughtInWildFlags(species, form) != 0;

    public int GetSelectedForm(ushort species)
    {
        if (species >= MAX_SPECIES)
            return 0;

        return SaveData.GetResearchEntry(species).SelectedForm;
    }

    public bool GetSelectedAlpha(ushort species)
    {
        if (species >= MAX_SPECIES)
            return false;

        return SaveData.GetResearchEntry(species).SelectedAlpha;
    }

    public bool GetSelectedShiny(ushort species)
    {
        if (species >= MAX_SPECIES)
            return false;

        return SaveData.GetResearchEntry(species).SelectedShiny;
    }

    public bool GetSelectedGender1(ushort species)
    {
        if (species >= MAX_SPECIES)
            return false;

        return SaveData.GetResearchEntry(species).SelectedGender1;
    }

    public bool GetSolitudeComplete(ushort species)
    {
        if (species >= MAX_SPECIES)
            return false;

        return SaveData.GetResearchEntry(species).IsSolitudeComplete;
    }

    public void SetSolitudeComplete(ushort species, bool value)
    {
        if (species >= MAX_SPECIES)
            return;

        SaveData.GetResearchEntry(species).IsSolitudeComplete = value;
    }

    public void SetSolitudeAll(bool value = true)
    {
        for (ushort i = MaxSpeciesID; i >= 1; i--)
        {
            // Set only species captures with dex indexes.
            var index = GetDexIndex(Hisui, i);
            if (index == DexInvalid)
                continue;

            SaveData.GetResearchEntry(i).IsSolitudeComplete = value;
        }
    }

    public void OnPokeEvolve(PKM pk, ushort fromSpecies)
    {
        OnPokeEvolved(fromSpecies, pk.Species);
        OnPokeGet_NoSpecialCatch(pk);
    }

    public void OnPokeGet_Caller1(PKM pk)
    {
        // 1.0.1: sub_7101283760
        // Is this on receiving an egg?

        // var metLoc = pk.MetLocation;
        // if (!GetCurrentTime())
        //     return;
        // SetMetOrEggLocation(pk, metLoc, calendarTime);
        // if (Unknown(pk) == 3)
        //     return;

        OnPokeGet_NoSpecialCatch(pk);
    }

    public void OnPokeGet_Trade(PKM pk, PKM evoPk)
    {
        // 1.0.1: sub_71026A34B4

        // Get the un-evolved trade receive poke
        if (pk.Species == 0 || pk.IsEgg)
            return;
        OnPokeGet_NoSpecialCatch(pk);

        // Get the trade-evolved trade receive poke
        if (evoPk.Species == 0 || evoPk.IsEgg)
            return;
        OnPokeEvolved(pk.Species, evoPk.Species);
        OnPokeGet_NoSpecialCatch(pk);
    }

    public void OnPokeGet_TradeWithoutEvolution(PKM pk)
    {
        // This isn't a real caller, but more convenient than making a fake species-0 evo result?
        if (pk.Species == 0 || pk.IsEgg)
            return;
        OnPokeGet_NoSpecialCatch(pk);
    }

    public void OnPokeGet_NoSpecialCatch(PKM pk)
        => OnPokeGet(pk, sleeping: false, inAir: false, notSpotted: false, skipCaughtAtTime: true, timeOfDay: PokedexTimeOfDay8a.Invalid, caught: false);

    public void OnPokeGet(PKM pk, bool sleeping, bool inAir, bool notSpotted, bool skipCaughtAtTime, PokedexTimeOfDay8a timeOfDay, bool caught)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        var pa8 = (PA8)pk;

        // Normal species obtained
        OnPokeObtained(pk.Species);

        // Alpha potentially obtained
        OnPokeAlphaCaught(pk.Species, pa8.IsAlpha);

        // Large poke potentially obtained 
        OnPokeLargeCaught(pk.Species, pa8.HeightAbsolute);

        // Small poke potentially obtained 
        OnPokeSmallCaught(pk.Species, pa8.HeightAbsolute);

        // Heavy poke potentially obtained 
        OnPokeHeavyCaught(pk.Species, pa8.WeightAbsolute);

        // Light poke potentially obtained 
        OnPokeLightCaught(pk.Species, pa8.WeightAbsolute);

        // Handle if Pokémon was caught while sleeping
        if (sleeping)
            OnPokeCaughtSleeping(pk.Species);

        // Handle if Pokémon was caught while in the air
        if (inAir)
            OnPokeCaughtInAir(pk.Species);

        // Handle if Pokémon was caught while not spotted
        if (notSpotted)
            OnPokeCaughtNotSpotted(pk.Species);

        // Handle time of day, if not invalid
        if (!skipCaughtAtTime && timeOfDay != PokedexTimeOfDay8a.Invalid)
            OnPokeCaughtAtTime(pk.Species, timeOfDay);

        // Process remaining logic for obtaining poke
        SetPokeObtained(pk, caught);
    }

    private void OnPokeObtained(ushort species) => IncrementResearchTaskProgress(species, Catch);

    private void OnPokeAlphaCaught(ushort species, bool alpha) => IncrementResearchTaskProgress(species, CatchAlpha, alpha);

    private void OnPokeLargeCaught(ushort species, float height) => IncrementResearchTaskProgress(species, CatchLarge, IsPokeLarge(species, height));

    private void OnPokeSmallCaught(ushort species, float height) => IncrementResearchTaskProgress(species, CatchSmall, IsPokeSmall(species, height));
    private void OnPokeHeavyCaught(ushort species, float weight) => IncrementResearchTaskProgress(species, CatchHeavy, IsPokeHeavy(species, weight));
    private void OnPokeLightCaught(ushort species, float weight) => IncrementResearchTaskProgress(species, CatchLight, IsPokeLight(species, weight));

    public static bool IsPokeLarge(ushort species, float height) => TryGetTriggeredTask(species, CatchLarge, out var task) && height >= task.Threshold;
    public static bool IsPokeSmall(ushort species, float height) => TryGetTriggeredTask(species, CatchSmall, out var task) && height <= task.Threshold;
    public static bool IsPokeHeavy(ushort species, float weight) => TryGetTriggeredTask(species, CatchHeavy, out var task) && weight >= task.Threshold / 10.0;
    public static bool IsPokeLight(ushort species, float weight) => TryGetTriggeredTask(species, CatchLight, out var task) && weight <= task.Threshold / 10.0;

    private void OnPokeCaughtSleeping(ushort species) => IncrementResearchTaskProgress(species, CatchSleeping);

    private void OnPokeCaughtInAir(ushort species) => IncrementResearchTaskProgress(species, CatchInAir);

    private void OnPokeCaughtNotSpotted(ushort species) => IncrementResearchTaskProgress(species, CatchNotSpotted);
    private void OnPokeCaughtAtTime(ushort species, PokedexTimeOfDay8a timeOfDay) => IncrementResearchTaskProgress(species, CatchAtTime, CanUpdateCatchAtTime(species, timeOfDay));

    public static bool CanUpdateCatchAtTime(ushort species, PokedexTimeOfDay8a timeOfDay) => TryGetTriggeredTask(species, timeOfDay, out var task) && task.TimeOfDay != PokedexTimeOfDay8a.Invalid;

    public int GetTotalGetAnyDexCount()
    {
        var count = 0;
        for (ushort species = 1; species <= MaxSpeciesID; species++)
        {
            if (GetPokeGetCount(species) > 0)
                count++;
        }
        return count;
    }

    public bool IsNewUnreportedPoke(ushort species)
    {
        if (species >= MAX_SPECIES)
            return false;

        var researchEntry = SaveData.GetResearchEntry(species);
        return researchEntry.NumObtained != 0 && !researchEntry.HasAnyReport;
    }

    public void OnPokeDefeated(PKM pk, MoveType moveType)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeDefeated(pk.Species);
        OnPokeDefeatedWithMoveType(pk.Species, moveType);
    }

    public void OnPokeDefeated(PKM pk) => OnPokeDefeated(pk, MoveType.Any);

    private void OnPokeDefeated(ushort species) => IncrementResearchTaskProgress(species, Defeat);

    private void OnPokeDefeatedWithMoveType(ushort species, MoveType moveType)
        => IncrementResearchTaskProgress(species, DefeatWithMoveType, TryGetTriggeredTask(species, moveType, out var task), task.Index);

    public void OnPokeUseMove(PKM pk, ushort move)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeUseMove(pk.Species, move);
    }

    private void OnPokeUseMove(ushort species, ushort move)
        => IncrementResearchTaskProgress(species, UseMove, TryGetTriggeredTask(species, move, out var task), task.Index);

    public void OnPokeEvolved(ushort fromSpecies, ushort toSpecies)
    {
        OnPokeEvolved(fromSpecies);
        OnPokeEvolved(toSpecies);
    }

    private void OnPokeEvolved(ushort species) => IncrementResearchTaskProgress(species, Evolve);

    public void OnPokeGivenFood(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeGivenFood(pk.Species);
    }

    private void OnPokeGivenFood(ushort species) => IncrementResearchTaskProgress(species, GiveFood);

    public void OnPokeStunnedWithItem(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeStunnedWithItem(pk.Species);
    }

    private void OnPokeStunnedWithItem(ushort species) => IncrementResearchTaskProgress(species, StunWithItems);

    public void OnPokeScaredWithScatterBang(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeScaredWithScatterBang(pk.Species);
    }

    private void OnPokeScaredWithScatterBang(ushort species) => IncrementResearchTaskProgress(species, ScareWithScatterBang);

    public void OnPokeLuredWithPokeshiDoll(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeLuredWithPokeshiDoll(pk.Species);
    }

    private void OnPokeLuredWithPokeshiDoll(ushort species) => IncrementResearchTaskProgress(species, LureWithPokeshiDoll);

    public void OnPokeUseStrongStyleMove(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeUseStrongStyleMove(pk.Species);
    }

    private void OnPokeUseStrongStyleMove(ushort species) => IncrementResearchTaskProgress(species, UseStrongStyleMove);

    public void OnPokeUseAgileStyleMove(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeUseAgileStyleMove(pk.Species);
    }

    private void OnPokeUseAgileStyleMove(ushort species) => IncrementResearchTaskProgress(species, UseAgileStyleMove);

    public void OnPokeLeapFromTree(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeLeapFromTree(pk.Species);
    }

    private void OnPokeLeapFromTree(ushort species) => IncrementResearchTaskProgress(species, LeapFromTrees);
    public void OnPokeLeapFromLeaves(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeLeapFromLeaves(pk.Species);
    }

    private void OnPokeLeapFromLeaves(ushort species) => IncrementResearchTaskProgress(species, LeapFromLeaves);
    public void OnPokeLeapFromSnow(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeLeapFromSnow(pk.Species);
    }

    private void OnPokeLeapFromSnow(ushort species) => IncrementResearchTaskProgress(species, LeapFromSnow);
    public void OnPokeLeapFromOre(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeLeapFromOre(pk.Species);
    }

    private void OnPokeLeapFromOre(ushort species) => IncrementResearchTaskProgress(species, LeapFromOre);
    public void OnPokeLeapFromTussock(PKM pk)
    {
        if (pk.IsEgg || IsNoblePokemon(pk.Species, pk.Form))
            return;

        OnPokeLeapFromTussock(pk.Species);
    }

    private void OnPokeLeapFromTussock(ushort species) => IncrementResearchTaskProgress(species, LeapFromTussocks);

    public bool IsAllRequiredTasksComplete(ushort species)
    {
        if (!TryGetResearchTasks(species, out var tasks))
            return true;

        for (var i = 0; i < tasks.Length; i++)
        {
            if (!tasks[i].RequiredForCompletion)
                continue;
            GetResearchTaskLevel(species, i, out var reportedLevel, out _, out _);
            if (reportedLevel < 2)
                return false;
        }

        return true;
    }

    public void SetPokeSeenInWildAndObtained(PKM pk, bool caught)
    {
        SetPokeSeenInWildImpl(pk);

        if (!pk.IsEgg)
            SetPokeObtained(pk, caught);
    }

    private int GetPartOfArceusValue(ulong hash)
    {
        if (hash == 0xCBF29CE484222645)
            return 0;

        return (int)(uint)SaveFile.Accessor.GetBlockValue(GetSaveBlockKey(hash));
    }

    private static uint GetSaveBlockKey(ulong hash) => (uint)hash; // truncate to 32-bit

    private int GetSpeciesQuestState(ulong hash)
    {
        if (hash is 0xC0EA47549AB5F3D9 or 0xCBF29CE484222645)
            return 0;

        // These are single-byte blocks, but type is "object"...
        var key = GetSaveBlockKey(hash);
        return SaveFile.Accessor.GetBlock(key).Data[0];
    }

    public static bool IsAnyTaskTriggered(ushort species, PokedexResearchTaskType8a which, MoveType moveType, int move, PokedexTimeOfDay8a timeOfDay)
        => TryGetTriggeredTask(species, which, moveType, move, timeOfDay, out _);

    public static int GetTriggeredTaskFinalThreshold(ushort species, PokedexResearchTaskType8a which, MoveType moveType, int move, PokedexTimeOfDay8a timeOfDay)
    {
        if (!TryGetTriggeredTask(species, which, moveType, move, timeOfDay, out var task))
            return 0;

        return task.TaskThresholds[^1];
    }

    private static bool TryGetTriggeredTask(ushort species, PokedexResearchTaskType8a which, out PokedexResearchTask8a outTask)
        => TryGetTriggeredTask(species, which, MoveType.Any, -1, PokedexTimeOfDay8a.Invalid, out outTask);
    private static bool TryGetTriggeredTask(ushort species, MoveType moveType, out PokedexResearchTask8a outTask)
        => TryGetTriggeredTask(species, DefeatWithMoveType, moveType, -1, PokedexTimeOfDay8a.Invalid, out outTask);
    private static bool TryGetTriggeredTask(ushort species, int move, out PokedexResearchTask8a outTask)
        => TryGetTriggeredTask(species, UseMove, MoveType.Any, move, PokedexTimeOfDay8a.Invalid, out outTask);
    private static bool TryGetTriggeredTask(ushort species, PokedexTimeOfDay8a timeOfDay, out PokedexResearchTask8a outTask)
        => TryGetTriggeredTask(species, CatchAtTime, MoveType.Any, -1, timeOfDay, out outTask);

    private static bool TryGetTriggeredTask(ushort species, PokedexResearchTaskType8a which, MoveType moveType, int move, PokedexTimeOfDay8a timeOfDay, out PokedexResearchTask8a outTask)
    {
        outTask = new PokedexResearchTask8a();

        if (!TryGetResearchTasks(species, out var tasks))
            return false;

        foreach (var task in tasks)
        {
            if (task.Task != which)
                continue;

            var isTriggeredTask = task.Task switch
            {
                UseMove => task.Move == move,
                DefeatWithMoveType => task.Type == moveType,
                CatchAtTime => task.TimeOfDay == timeOfDay,
                _ => true,
            };

            if (!isTriggeredTask)
                continue;

            outTask = task;
            return true;
        }

        return false;
    }

    public void SetSelectedGenderForm(PKM pk) => SetSelectedGenderForm(pk.Species, pk.Form, pk.Gender == 1, pk.IsShiny, ((PA8)pk).IsAlpha);

    public void SetSelectedGenderForm(ushort species, byte form, bool gender1, bool shiny, bool alpha)
    {
        if (species >= MAX_SPECIES || form >= MAX_FORM)
            return;

        var speciesEntry = SaveData.GetResearchEntry(species);

        var mash = (ushort)(species | (form << 11));
        var pokeInfoIndex = PokedexConstants8a.PokemonInfoIds.BinarySearch(mash);
        if (pokeInfoIndex >= 0 && (PokedexConstants8a.PokemonInfoGenders[pokeInfoIndex] & (1 << 3)) != 0)
        {
            var g0 = false;
            var g1 = false;

            if (SaveData.TryGetStatisticsEntry(species, form, out var statEntry))
            {
                var flags = speciesEntry.NumObtained > 0 ? statEntry.ObtainFlags : statEntry.SeenInWildFlags;

                var ofs = (shiny ? 4 : 0) + (alpha ? 2 : 0);

                g0 = (flags & (1 << (ofs + 0))) != 0;
                g1 = (flags & (1 << (ofs + 1))) != 0;
            }

            if (gender1)
                gender1 = !g0 || g1;
            else
                gender1 = !g0 && g1;
        }

        speciesEntry.SelectedGender1 = gender1;
        speciesEntry.SelectedShiny = shiny;
        speciesEntry.SelectedAlpha = alpha;
        speciesEntry.SelectedForm = form;
    }

    private void SetPokeObtained(PKM pk, bool caught)
    {
        if (pk.IsEgg)
            return;

        // Get statistics entry
        if (!SaveData.TryGetStatisticsEntry(pk, out var statEntry, out var shift))
            return;

        var pa8 = (PA8)pk;

        // Update statistics if not alpha
        if (!pa8.IsAlpha)
        {
            if ((statEntry.ObtainFlags & 0x33) != 0)
            {
                if (statEntry.HasMaximumStatistics)
                {
                    // We have previous max stats, so update max stats
                    statEntry.MaxHeight = Math.Max(pa8.HeightAbsolute, statEntry.MaxHeight);
                    statEntry.MaxWeight = Math.Max(pa8.WeightAbsolute, statEntry.MaxWeight);
                }
                else
                {
                    // We have no previous max stats, so set initial max stats
                    statEntry.MaxHeight = Math.Max(pa8.HeightAbsolute, statEntry.MinHeight);
                    statEntry.MaxWeight = Math.Max(pa8.WeightAbsolute, statEntry.MinWeight);
                    statEntry.HasMaximumStatistics = true;
                }

                // Update min stats
                statEntry.MinHeight = Math.Min(pa8.HeightAbsolute, statEntry.MinHeight);
                statEntry.MinWeight = Math.Min(pa8.WeightAbsolute, statEntry.MinWeight);
            }
            else
            {
                // If nothing previously obtained, just set min height/weight
                statEntry.MinHeight = pa8.HeightAbsolute;
                statEntry.MinWeight = pa8.WeightAbsolute;
            }
        }

        // Update obtain flags
        statEntry.ObtainFlags |= (byte)(1u << shift);
        if (caught)
            statEntry.CaughtInWildFlags |= (byte)(1u << shift);

        // If the poke is new to our dex/unreported, update the selected icon
        if (IsNewUnreportedPoke(pk.Species))
            SetSelectedGenderForm(pk);
    }

    public void SetGlobalField04(byte v) => SaveData.SetGlobalField04(v);
    public byte GetGlobalField04() => SaveData.GetGlobalField04();

    public void SetLocalField03(PokedexType8a which, byte v)
    {
        if (IsLocalDex(which))
            SaveData.SetLocalField03(GetLocalDexIndex(which), v);
    }

    public byte GetLocalField03(PokedexType8a which)
    {
        if (IsLocalDex(which))
            return SaveData.GetLocalField03(GetLocalDexIndex(which));
        return 0;
    }

    public void GetLocalParameters(PokedexType8a which, out byte outField02, out ushort outField00, out uint outField04, out uint outField08, out ushort outField0C)
    {
        // Unclear what these are (they don't ever seem to be set unless code has been missed)?
        // This is called by a func with "pane_N_detail_00", "list_panel" strings in code.
        if (IsLocalDex(which))
        {
            SaveData.GetLocalParameters(GetLocalDexIndex(which), out outField02, out outField00, out outField04, out outField08, out outField0C);
        }
        else
        {
            outField02 = 0;
            outField00 = 0;
            outField04 = 0;
            outField08 = 0;
            outField0C = 0;
        }
    }

    public void SetGlobalFormField(byte form)
    {
        if (form < MAX_FORM)
            SaveData.SetGlobalFormField(form);
    }

    public int GetGlobalFormField() => SaveData.GetGlobalFormField();

    public bool HasFormStorage(ushort species, byte form) => SaveData.TryGetStatisticsEntry(species, form, out _);

    public bool IsBlacklisted(ushort species, byte form) => IsNoblePokemon(species, form);

    public bool GetSizeStatistics(ushort species, byte form, out bool hasMax, out float minHeight, out float maxHeight, out float minWeight, out float maxWeight)
    {
        hasMax = false;
        minHeight = 0;
        maxHeight = 0;
        minWeight = 0;
        maxWeight = 0;

        if (!SaveData.TryGetStatisticsEntry(species, form, out var entry))
            return false;

        hasMax = entry.HasMaximumStatistics;
        minHeight = entry.MinHeight;
        maxHeight = entry.HasMaximumStatistics ? entry.MaxHeight : entry.MinHeight;
        minWeight = entry.MinWeight;
        maxWeight = entry.HasMaximumStatistics ? entry.MaxWeight : entry.MinWeight;

        return true;
    }

    public void SetSizeStatistics(ushort species, byte form, bool hasMax, float minHeight, float maxHeight, float minWeight, float maxWeight)
    {
        if (minHeight < 0)
            minHeight = 0;
        if (minWeight < 0)
            minWeight = 0;

        if (maxHeight < minHeight || !hasMax)
            maxHeight = minHeight;
        if (maxWeight < minWeight || !hasMax)
            maxWeight = minWeight;

        if (!SaveData.TryGetStatisticsEntry(species, form, out var entry))
            return;

        entry.HasMaximumStatistics = hasMax;
        entry.MinHeight = minHeight;
        entry.MaxHeight = maxHeight;
        entry.MinWeight = minWeight;
        entry.MaxWeight = maxWeight;

        if (minWeight != 0 || maxWeight != 0 || minHeight != 0 || maxHeight != 0)
            SetPokeHasBeenUpdated(species);
    }

    public static bool IsNoblePokemon(ushort species, byte form) => form switch
    {
        1 => species == 900,
        2 => species is 59 or 101 or 549 or 713,
        _ => false,
    };

    private static int GetLocalDexIndex(PokedexType8a which) => which switch
    {
        Local1 => 0,
        Local2 => 1,
        Local3 => 2,
        Local4 => 3,
        Local5 => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(which)),
    };

    private static bool IsLocalDex(PokedexType8a which) => which switch
    {
        Local1 => true,
        Local2 => true,
        Local3 => true,
        Local4 => true,
        Local5 => true,
        _ => false,
    };

    private static bool TryGetResearchTasks(ushort species, [NotNullWhen(true)] out PokedexResearchTask8a[]? tasks)
    {
        var dexIndex = GetDexIndex(Hisui, species);
        if (dexIndex == DexInvalid)
        {
            tasks = null;
            return false;
        }

        tasks = PokedexConstants8a.ResearchTasks[dexIndex - 1];
        return true;
    }
}
