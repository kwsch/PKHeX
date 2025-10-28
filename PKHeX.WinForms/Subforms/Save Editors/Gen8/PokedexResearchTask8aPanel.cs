using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Drawing;
using PKHeX.WinForms.Properties;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PokedexResearchTask8aPanel : UserControl
{
    public ushort Species { get; private set; }
    public int ReportedCount { get; private set; }
    public PokedexResearchTask8a Task { get; private set; } = new();

    private string[] TaskDescriptions = [];
    private string[] SpeciesQuests = [];
    private string[] TimeTaskDescriptions = [];

    private readonly MaskedTextBox[] ThresholdBoxes;
    private readonly bool Loaded;

    public PokedexResearchTask8aPanel()
    {
        InitializeComponent();

        ThresholdBoxes = [MTB_Threshold1, MTB_Threshold2, MTB_Threshold3, MTB_Threshold4, MTB_Threshold5];
        Loaded = true;
    }

    public int CurrentValue
    {
        get => (int)NUP_CurrentValue.Value;
        set => NUP_CurrentValue.Value = value;
    }

    public int PointsPerLevel => Task.PointsSingle + Task.PointsBonus;

    public void SetStrings(string[] tasks, string[] speciesQuests, string[] timeTasks)
    {
        TaskDescriptions = tasks;
        SpeciesQuests = speciesQuests;
        TimeTaskDescriptions = timeTasks;
    }

    public void SetTask(ushort species, PokedexResearchTask8a task, int reportedLevel)
    {
        Species = species;
        Task = task;
        ReportedCount = reportedLevel - 1;

        SuspendLayout();

        PB_Bonus.Image = Task.PointsBonus != 0 ? Resources.research_bonus_points : null;
        Label_Task.Text = $"{TaskLabelString}:";
        NUP_CurrentValue.Enabled = CanSetCurrentValue;

        FLP_T1Right.Controls.Clear();

        ShadeBoxes();

        for (var t = 0; t < task.TaskThresholds.Length; t++)
            ThresholdBoxes[t].Text = $"{task.TaskThresholds[t]}";

        for (var t = 0; t < task.TaskThresholds.Length; t++)
            FLP_T1Right.Controls.Add(ThresholdBoxes[task.TaskThresholds.Length - 1 - t]);

        ResumeLayout();
    }

    public void ShadeBoxes()
    {
        if (!Loaded)
            return;

        var currentValue = CurrentValue;
        for (var i = 0; i < Task.TaskThresholds.Length; i++)
            ThresholdBoxes[i].BackColor = GetTaskColor(currentValue, i);
    }

    private Color GetTaskColor(int currentValue, int thresholdIndex)
    {
        bool belowReported = thresholdIndex < ReportedCount;
        if (currentValue >= Task.TaskThresholds[thresholdIndex])
        {
            if (belowReported)
                return ColorUtil.Blend(Color.Green, SystemColors.Window, 0.4);
            return ColorUtil.Blend(Color.YellowGreen, SystemColors.Window, 0.4);
        }

        if (belowReported)
            return ColorUtil.Blend(Color.Red, SystemColors.Window, 0.4);
        return SystemColors.Window;
    }

    private void NUP_CurrentValue_Changed(object sender, EventArgs e)
    {
        ShadeBoxes();
    }

    public bool CanSetCurrentValue => Task.Task.CanSetCurrentValue();

    private string TaskLabelString => Task.GetTaskLabelString(TaskDescriptions, TimeTaskDescriptions, SpeciesQuests);
}
