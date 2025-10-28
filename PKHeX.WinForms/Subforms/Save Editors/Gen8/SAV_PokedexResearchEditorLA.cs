using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.PokedexResearchTaskType8a;

namespace PKHeX.WinForms;

public partial class SAV_PokedexResearchEditorLA : Form
{
    private readonly SAV8LA Origin;
    private readonly SAV8LA SAV;
    private readonly PokedexSave8a Dex;

    private readonly ushort Species;
    private readonly bool WasEmpty;

    private readonly NumericUpDown[] TaskNUPs;

    private static ReadOnlySpan<PokedexResearchTaskType8a> TaskTypes =>
    [
        Catch,
        CatchAlpha,
        CatchLarge,
        CatchSmall,
        CatchHeavy,
        CatchLight,
        CatchAtTime,
        CatchSleeping,
        CatchInAir,
        CatchNotSpotted,

        UseMove,
        UseMove,
        UseMove,
        UseMove,
        DefeatWithMoveType,
        DefeatWithMoveType,
        DefeatWithMoveType,
        Defeat,
        UseStrongStyleMove,
        UseAgileStyleMove,

        Evolve,
        GiveFood,
        StunWithItems,
        ScareWithScatterBang,
        LureWithPokeshiDoll,

        LeapFromTrees,
        LeapFromLeaves,
        LeapFromSnow,
        LeapFromOre,
        LeapFromTussocks,
    ];

    private static ReadOnlySpan<sbyte> TaskIndexes =>
    [
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 1, 2, 3, 0, 1, 2, -1, -1, -1,
        -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1,
    ];

    private readonly int[] TaskParameters;

    public SAV_PokedexResearchEditorLA(SAV8LA sav, ushort species, int dexIdx, IReadOnlyList<string> tasks, IReadOnlyList<string> timeTasks)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8LA)(Origin = sav).Clone();
        Dex = SAV.Blocks.PokedexSave;

        Species = species;

        L_Species.Text = GameInfo.Strings.Species[species];

        #region Declare Arrays
        Label[] taskLabels =
        [
            L_Catch,
            L_CatchAlpha,
            L_CatchLarge,
            L_CatchSmall,
            L_CatchHeavy,
            L_CatchLight,
            L_CatchAtTime,
            L_CatchSleeping,
            L_CatchInAir,
            L_CatchNotSpotted,

            L_UseMove0,
            L_UseMove1,
            L_UseMove2,
            L_UseMove3,
            L_DefeatWithMove0,
            L_DefeatWithMove1,
            L_DefeatWithMove2,
            L_Defeat,
            L_StrongStyle,
            L_AgileStyle,

            L_Evolve,
            L_GiveFood,
            L_Stun,
            L_Scare,
            L_Lure,

            L_LeapTrees,
            L_LeapLeaves,
            L_LeapSnow,
            L_LeapOre,
            L_LeapTussocks,
        ];

        TaskNUPs =
        [
            NUP_Catch,
            NUP_CatchAlpha,
            NUP_CatchLarge,
            NUP_CatchSmall,
            NUP_CatchHeavy,
            NUP_CatchLight,
            NUP_CatchAtTime,
            NUP_CatchSleeping,
            NUP_CatchInAir,
            NUP_CatchNotSpotted,

            NUP_UseMove0,
            NUP_UseMove1,
            NUP_UseMove2,
            NUP_UseMove3,
            NUP_DefeatWithMove0,
            NUP_DefeatWithMove1,
            NUP_DefeatWithMove2,
            NUP_Defeat,
            NUP_StrongStyle,
            NUP_AgileStyle,

            NUP_Evolve,
            NUP_GiveFood,
            NUP_Stun,
            NUP_Scare,
            NUP_Lure,

            NUP_LeapTrees,
            NUP_LeapLeaves,
            NUP_LeapSnow,
            NUP_LeapOre,
            NUP_LeapTussocks,
        ];

        TaskParameters = new int[TaskIndexes.Length];
        InitializeTaskParameters(dexIdx);
        #endregion

        // Initialize labels/values
        for (int i = 0; i < taskLabels.Length; i++)
        {
            taskLabels[i].Text = PokedexResearchTask8aExtensions.GetGenericTaskLabelString(TaskTypes[i], TaskIndexes[i], TaskParameters[i], tasks, timeTasks);

            Dex.GetResearchTaskProgressByForce(Species, TaskTypes[i], TaskIndexes[i], out var curValue);
            TaskNUPs[i].Value = curValue;
        }

        // Detect empty
        WasEmpty = IsEmpty();
    }

    private void InitializeTaskParameters(int idx)
    {
        if (idx < 0)
        {
            for (int i = 0; i < TaskParameters.Length; i++)
                TaskParameters[i] = TaskTypes[i] == CatchAtTime ? 0 : -1;
            return;
        }

        var tasks = PokedexConstants8a.ResearchTasks[idx];
        for (int i = 0; i < TaskParameters.Length; i++)
        {
            TaskParameters[i] = -1;

            switch (TaskTypes[i])
            {
                case UseMove:
                    foreach (var task in tasks)
                    {
                        if (task.Task != UseMove || task.Index != TaskIndexes[i])
                            continue;
                        TaskParameters[i] = task.Move;
                        break;
                    }
                    break;
                case DefeatWithMoveType:
                    foreach (var task in tasks)
                    {
                        if (task.Task != DefeatWithMoveType || task.Index != TaskIndexes[i])
                            continue;
                        TaskParameters[i] = (int)task.Type;
                        break;
                    }
                    break;
                case CatchAtTime:
                    TaskParameters[i] = 0;
                    foreach (var task in tasks)
                    {
                        if (task.Task != CatchAtTime)
                            continue;
                        TaskParameters[i] = (int)task.TimeOfDay;
                        break;
                    }
                    break;
            }
        }
    }

    private bool IsEmpty()
    {
        foreach (var nup in TaskNUPs)
        {
            if (nup.Value != 0)
                return false;
        }
        return true;
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        // If we should, set values.
        if (!WasEmpty || !IsEmpty())
        {
            for (int i = 0; i < TaskNUPs.Length; i++)
                Dex.SetResearchTaskProgressByForce(Species, TaskTypes[i], (int)TaskNUPs[i].Value, TaskIndexes[i]);
        }

        Origin.CopyChangesFrom(SAV);
        Close();
    }
}
