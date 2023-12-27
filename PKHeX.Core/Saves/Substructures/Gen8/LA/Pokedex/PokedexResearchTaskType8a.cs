using System;
using System.Collections.Generic;
using static PKHeX.Core.PokedexResearchTaskType8a;

namespace PKHeX.Core;

/// <summary>
/// Research Task types for <see cref="GameVersion.PLA"/> Pokédex entries.
/// </summary>
public enum PokedexResearchTaskType8a : byte
{
    Catch = 0,
    UseMove = 1,
    DefeatWithMoveType = 2,
    Defeat = 3,
    Evolve = 4,
    CatchAlpha = 5,
    CatchLarge = 6,
    CatchSmall = 7,
    CatchHeavy = 8,
    CatchLight = 9,
    CatchAtTime = 10,
    CatchSleeping = 11,
    CatchInAir = 12,
    CatchNotSpotted = 13,
    GiveFood = 14,
    StunWithItems = 15,
    ScareWithScatterBang = 16,
    LureWithPokeshiDoll = 17,
    UseStrongStyleMove = 18,
    UseAgileStyleMove = 19,
    LeapFromTrees = 20,
    LeapFromLeaves = 21,
    LeapFromSnow = 22,
    LeapFromOre = 23,
    LeapFromTussocks = 24,
    ObtainForms = 25,
    PartOfArceus = 26,
    SpeciesQuest = 27,
}

public static class PokedexResearchTask8aExtensions
{
    public static bool CanSetCurrentValue(this PokedexResearchTaskType8a task) => task switch
    {
        ObtainForms => false,
        PartOfArceus => false,
        SpeciesQuest => false,
        _ => true,
    };

    public static string GetTaskLabelString(this PokedexResearchTask8a task, IReadOnlyList<string> TaskDescriptions, IReadOnlyList<string> TimeTaskDescriptions, IReadOnlyList<string> SpeciesQuests) => task.Task switch
    {
        SpeciesQuest       => GetSpeciesQuestLabel(task.Hash_06, SpeciesQuests),
        UseMove            => GetGenericTaskLabelString(task.Task, task.Index, task.Move, TaskDescriptions, TimeTaskDescriptions),
        DefeatWithMoveType => GetGenericTaskLabelString(task.Task, task.Index, (int)task.Type, TaskDescriptions, TimeTaskDescriptions),
        CatchAtTime        => GetGenericTaskLabelString(task.Task, task.Index, (int)task.TimeOfDay, TaskDescriptions, TimeTaskDescriptions),
        _                  => GetGenericTaskLabelString(task.Task, task.Index, (int)task.TimeOfDay, TaskDescriptions, TimeTaskDescriptions),
    };

    public static string GetGenericTaskLabelString(PokedexResearchTaskType8a task, int idx, int param, IReadOnlyList<string> TaskDescriptions, IReadOnlyList<string> TimeTaskDescriptions) => task switch
    {
        UseMove => string.Format(TaskDescriptions[(int)task], param >= 0 ? GameInfo.Strings.Move[param] : $"(idx={idx})"),
        DefeatWithMoveType => string.Format(TaskDescriptions[(int)task], param >= 0 ? GameInfo.Strings.Types[param] : $"(idx={idx})"),
        CatchAtTime => TimeTaskDescriptions[param],
        _ => TaskDescriptions[(int)task],
    };

    private static string GetSpeciesQuestLabel(ulong hash, IReadOnlyList<string> labels) => hash switch
    {
        0xE68C0D2852AF9068 => labels[0],
        0xE68C0E2852AF921B => labels[1],
        0xE68F7B2852B28129 => labels[2],
        0xE68F7C2852B282DC => labels[3],
        0xE68F7E2852B28642 => labels[4],
        0xE68F812852B28B5B => labels[5],
        0xE68F802852B289A8 => labels[6],
        0xE6850D2852A971BA => labels[7],
        0xE6888B2852AC7DAB => labels[8],
        0xE6888F2852AC8477 => labels[9],
        0xE688842852AC71C6 => labels[10],
        0xE69D122852BE0C1A => labels[12],
        0xE69D092852BDFCCF => labels[13],
        0xE6927D2852B4BA66 => labels[14],
        0xE696022852B7D23C => labels[16],
        0xE67080285297D919 => labels[17],
        0xE6708C285297ED7D => labels[18],
        0xE6740B28529AFB21 => labels[19],
        0xE6740D28529AFE87 => labels[20],
        _ => throw new ArgumentOutOfRangeException(nameof(hash)),
    };
}
