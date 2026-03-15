using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static PKHeX.Core.PokedexResearchTaskType8a;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Legends Arceus advanced research task editor.
/// Allows editing all possible research task values for a specific species.
/// </summary>
public partial class PokedexResearchEditorLAViewModel : SaveEditorViewModelBase
{
    private readonly SAV8LA _origin;
    private readonly SAV8LA _sav;
    private readonly PokedexSave8a _dex;
    private readonly ushort _species;
    private readonly bool _wasEmpty;

    public string SpeciesName { get; }

    private static ReadOnlySpan<PokedexResearchTaskType8a> TaskTypes =>
    [
        Catch, CatchAlpha, CatchLarge, CatchSmall, CatchHeavy,
        CatchLight, CatchAtTime, CatchSleeping, CatchInAir, CatchNotSpotted,
        UseMove, UseMove, UseMove, UseMove,
        DefeatWithMoveType, DefeatWithMoveType, DefeatWithMoveType,
        Defeat, UseStrongStyleMove, UseAgileStyleMove,
        Evolve, GiveFood, StunWithItems, ScareWithScatterBang, LureWithPokeshiDoll,
        LeapFromTrees, LeapFromLeaves, LeapFromSnow, LeapFromOre, LeapFromTussocks,
    ];

    private static ReadOnlySpan<sbyte> TaskIndexes =>
    [
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
         0,  1,  2,  3,  0,  1,  2, -1, -1, -1,
        -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1,
    ];

    // Task values (30 tasks)
    [ObservableProperty] private int _task0;
    [ObservableProperty] private int _task1;
    [ObservableProperty] private int _task2;
    [ObservableProperty] private int _task3;
    [ObservableProperty] private int _task4;
    [ObservableProperty] private int _task5;
    [ObservableProperty] private int _task6;
    [ObservableProperty] private int _task7;
    [ObservableProperty] private int _task8;
    [ObservableProperty] private int _task9;
    [ObservableProperty] private int _task10;
    [ObservableProperty] private int _task11;
    [ObservableProperty] private int _task12;
    [ObservableProperty] private int _task13;
    [ObservableProperty] private int _task14;
    [ObservableProperty] private int _task15;
    [ObservableProperty] private int _task16;
    [ObservableProperty] private int _task17;
    [ObservableProperty] private int _task18;
    [ObservableProperty] private int _task19;
    [ObservableProperty] private int _task20;
    [ObservableProperty] private int _task21;
    [ObservableProperty] private int _task22;
    [ObservableProperty] private int _task23;
    [ObservableProperty] private int _task24;
    [ObservableProperty] private int _task25;
    [ObservableProperty] private int _task26;
    [ObservableProperty] private int _task27;
    [ObservableProperty] private int _task28;
    [ObservableProperty] private int _task29;

    // Labels for the 30 tasks
    public string[] TaskLabels { get; } = new string[30];

    public PokedexResearchEditorLAViewModel(SAV8LA sav, ushort species) : base(sav)
    {
        _sav = (SAV8LA)(_origin = sav).Clone();
        _dex = _sav.Blocks.PokedexSave;
        _species = species;

        SpeciesName = GameInfo.Strings.Species[species];

        var tasks = new string[30];
        for (int t = 0; t < tasks.Length; t++)
            tasks[t] = $"Task {t}";
        var timeTasks = new string[30];
        for (int t = 0; t < timeTasks.Length; t++)
            timeTasks[t] = $"Time Task {t}";

        for (int i = 0; i < 30; i++)
        {
            TaskLabels[i] = PokedexResearchTask8aExtensions.GetGenericTaskLabelString(
                TaskTypes[i], TaskIndexes[i], -1, tasks, timeTasks);

            _dex.GetResearchTaskProgressByForce(_species, TaskTypes[i], TaskIndexes[i], out var curValue);
            SetTaskValue(i, curValue);
        }

        _wasEmpty = IsEmpty();
    }

    private int GetTaskValue(int index) => index switch
    {
        0 => Task0, 1 => Task1, 2 => Task2, 3 => Task3, 4 => Task4,
        5 => Task5, 6 => Task6, 7 => Task7, 8 => Task8, 9 => Task9,
        10 => Task10, 11 => Task11, 12 => Task12, 13 => Task13, 14 => Task14,
        15 => Task15, 16 => Task16, 17 => Task17, 18 => Task18, 19 => Task19,
        20 => Task20, 21 => Task21, 22 => Task22, 23 => Task23, 24 => Task24,
        25 => Task25, 26 => Task26, 27 => Task27, 28 => Task28, 29 => Task29,
        _ => 0,
    };

    private void SetTaskValue(int index, int value)
    {
        switch (index)
        {
            case 0: Task0 = value; break;
            case 1: Task1 = value; break;
            case 2: Task2 = value; break;
            case 3: Task3 = value; break;
            case 4: Task4 = value; break;
            case 5: Task5 = value; break;
            case 6: Task6 = value; break;
            case 7: Task7 = value; break;
            case 8: Task8 = value; break;
            case 9: Task9 = value; break;
            case 10: Task10 = value; break;
            case 11: Task11 = value; break;
            case 12: Task12 = value; break;
            case 13: Task13 = value; break;
            case 14: Task14 = value; break;
            case 15: Task15 = value; break;
            case 16: Task16 = value; break;
            case 17: Task17 = value; break;
            case 18: Task18 = value; break;
            case 19: Task19 = value; break;
            case 20: Task20 = value; break;
            case 21: Task21 = value; break;
            case 22: Task22 = value; break;
            case 23: Task23 = value; break;
            case 24: Task24 = value; break;
            case 25: Task25 = value; break;
            case 26: Task26 = value; break;
            case 27: Task27 = value; break;
            case 28: Task28 = value; break;
            case 29: Task29 = value; break;
        }
    }

    private bool IsEmpty()
    {
        for (int i = 0; i < 30; i++)
        {
            if (GetTaskValue(i) != 0)
                return false;
        }
        return true;
    }

    [RelayCommand]
    private void Save()
    {
        if (!_wasEmpty || !IsEmpty())
        {
            for (int i = 0; i < 30; i++)
                _dex.SetResearchTaskProgressByForce(_species, TaskTypes[i], GetTaskValue(i), TaskIndexes[i]);
        }
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}
