using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public sealed partial class SAV_Pokeathlon4 : Form
{
    private const int MaxSpeciesGen4 = 493;

    private readonly SAV4HGSS Origin;
    private readonly SAV4HGSS SAV;
    private readonly Pokeathlon4 Pokeathlon;
    private readonly CheckBox[] DailyShopEditors;
    private readonly NumericUpDown[] CourseScoreEditors;
    private readonly PokeathlonParticipant4Editor[] CourseParticipantEditors;
    private readonly List<CounterBinding> CounterEditors = [];
    private readonly List<(PokeathlonEvent4 Event, NumericUpDown Editor)> BestScoreEditors = [];
    private readonly Button B_BestSpacer = new();

    private bool IsLoading;
    private int CurrentCourseIndex = -1;
    private int CurrentSelfEventIndex = -1;
    private int CurrentConnectionIndex = -1;

    private static string Localize<T>(T value) where T : Enum => WinFormsTranslator.TranslateEnum(value, Main.CurrentLanguage);
    private static string GetCourseDisplayName(PokeathlonStat4 stat) => $"{(int)stat + 1} - {Localize(stat)}";
    private static string GetEventDisplayName(PokeathlonEvent4 value) => $"{(int)value + 1} - {Localize(value)}";

    public SAV_Pokeathlon4(SAV4HGSS sav)
    {
        InitializeComponent();

#if DEBUG // Translation Rip
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident
        sav ??= new();
#endif

        Origin = sav;
        SAV = (SAV4HGSS)sav.Clone();
        Pokeathlon = SAV.Pokeathlon;

        DailyShopEditors =
        [
            CHK_DailyShop0, CHK_DailyShop1, CHK_DailyShop2, CHK_DailyShop3, CHK_DailyShop4, CHK_DailyShop5,
            CHK_DailyShop6, CHK_DailyShop7, CHK_DailyShop8, CHK_DailyShop9, CHK_DailyShop10, CHK_DailyShop11,
        ];
        CourseScoreEditors = [NUD_CourseScore0, NUD_CourseScore1, NUD_CourseScore2, NUD_CourseScoreMax];
        CourseParticipantEditors = [UC_CourseParticipant0, UC_CourseParticipant1, UC_CourseParticipant2];

        InitializeGeneral();
        InitializeMedals();
        InitializeCounters();
        InitializeBestScores();
        InitializeIndexes();

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        LoadData();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveCurrentViews();
        SaveGeneral();
        SaveMedals();
        SaveCounters();
        SaveBestScores();
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void CB_CourseIndex_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsLoading)
            return;

        var index = WinFormsUtil.GetIndex(CB_CourseIndex);
        if (CurrentCourseIndex >= 0)
            SaveCourse(CurrentCourseIndex);
        LoadCourse(index);
    }

    private void CB_SelfEventIndex_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsLoading)
            return;

        var index = WinFormsUtil.GetIndex(CB_SelfEventIndex);
        if (CurrentSelfEventIndex >= 0)
            SaveSelfEvent(CurrentSelfEventIndex);
        LoadSelfEvent(index);
    }

    private void CB_ConnectionIndex_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsLoading)
            return;

        var index = WinFormsUtil.GetIndex(CB_ConnectionIndex);
        if (CurrentConnectionIndex >= 0)
            SaveConnection(index: CurrentConnectionIndex);
        LoadConnection(index);
    }

    private void InitializeGeneral()
    {
        var items = GameInfo.Strings.GetItemStrings(EntityContext.Gen4);
        CLB_DataCards.Items.Clear();
        for (int i = 0; i < (int)DataCard4.Count; i++)
        {
            var itemID = 505 + i; // Data Card 01...
            var name = items[itemID];
            CLB_DataCards.Items.Add($"[{i}] {name}");
        }
    }

    private void InitializeMedals()
    {
        DGV_Medals.Columns.Clear();
        DGV_Medals.Rows.Clear();
        DGV_Medals.AllowUserToAddRows = false;
        DGV_Medals.AllowUserToDeleteRows = false;
        DGV_Medals.AllowUserToResizeRows = false;
        DGV_Medals.MultiSelect = false;
        DGV_Medals.RowHeadersVisible = false;
        DGV_Medals.RowTemplate.Height = 40;

        DGV_Medals.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Species",
            HeaderText = "Species",
            Width = 80,
            ReadOnly = true,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
        });
        DGV_Medals.Columns.Add(new DataGridViewImageColumn
        {
            Name = "Sprite",
            HeaderText = "Sprite",
            Width = 48,
            ReadOnly = true,
            ImageLayout = DataGridViewImageCellLayout.Zoom,
        });

        foreach (var name in Enum.GetNames<PokeathlonStat4>().Take((int)PokeathlonStat4.Count))
        {
            DGV_Medals.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = name,
                HeaderText = name,
                Width = 80,
                SortMode = DataGridViewColumnSortMode.Automatic,
            });
        }

        DGV_Medals.Rows.Add(MaxSpeciesGen4);
        for (int species = 1; species <= MaxSpeciesGen4; species++)
        {
            var row = DGV_Medals.Rows[species - 1];
            row.Cells[0].Value = species;
            row.Cells[1].Value = SpriteUtil.GetSprite((ushort)species, 0, 0, 0, 0, false, Shiny.Never, EntityContext.Gen4);
            row.Tag = (ushort)species;
            row.Cells[0].ToolTipText = GameInfo.Strings.specieslist[species];
            row.Cells[1].ToolTipText = GameInfo.Strings.specieslist[species];
        }
    }

    private void InitializeCounters()
    {
        AddCounterRow("TimeSpent", PokeathlonGlobalCounters4.MaxPlay, () => Pokeathlon.GlobalCounters.TimeSpent, value => { var c = Pokeathlon.GlobalCounters; c.TimeSpent = value; });
        AddCounterRow("SessionsJoined", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.SessionsJoined, value => { var c = Pokeathlon.GlobalCounters; c.SessionsJoined = value; });
        AddCounterRow("PlacedFirst", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.PlacedFirst, value => { var c = Pokeathlon.GlobalCounters; c.PlacedFirst = value; });
        AddCounterRow("PlacedLast", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.PlacedLast, value => { var c = Pokeathlon.GlobalCounters; c.PlacedLast = value; });
        AddCounterRow("BonusesEarned", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.BonusesEarned, value => { var c = Pokeathlon.GlobalCounters; c.BonusesEarned = value; });
        AddCounterRow("Instructions", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.Instructions, value => { var c = Pokeathlon.GlobalCounters; c.Instructions = value; });
        AddCounterRow("Failed", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.Failed, value => { var c = Pokeathlon.GlobalCounters; c.Failed = value; });
        AddCounterRow("Jumped", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.Jumped, value => { var c = Pokeathlon.GlobalCounters; c.Jumped = value; });
        AddCounterRow("Acquired", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.Acquired, value => { var c = Pokeathlon.GlobalCounters; c.Acquired = value; });
        AddCounterRow("Tackled", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.Tackled, value => { var c = Pokeathlon.GlobalCounters; c.Tackled = value; });
        AddCounterRow("FellDown", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.FellDown, value => { var c = Pokeathlon.GlobalCounters; c.FellDown = value; });
        AddCounterRow("Dashed", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.Dashed, value => { var c = Pokeathlon.GlobalCounters; c.Dashed = value; });
        AddCounterRow("Switched", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.Switched, value => { var c = Pokeathlon.GlobalCounters; c.Switched = value; });
        AddCounterRow("SelfImpeded", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.SelfImpeded, value => { var c = Pokeathlon.GlobalCounters; c.SelfImpeded = value; });
        AddCounterRow("ConnectionJoined", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.ConnectionJoined, value => { var c = Pokeathlon.GlobalCounters; c.ConnectionJoined = value; });
        AddCounterRow("ConnectionFirst", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.ConnectionFirst, value => { var c = Pokeathlon.GlobalCounters; c.ConnectionFirst = value; });
        AddCounterRow("ConnectionLast", PokeathlonGlobalCounters4.MaxStat, () => Pokeathlon.GlobalCounters.ConnectionLast, value => { var c = Pokeathlon.GlobalCounters; c.ConnectionLast = value; });

        for (int i = 0; i < (int)PokeathlonEvent4.Count; i++)
        {
            var eventIndex = (PokeathlonEvent4)i;
            AddCounterRow($"{Localize(eventIndex)}First", PokeathlonGlobalCounters4.MaxStat,
                () => { var c = Pokeathlon.GlobalCounters; return c[eventIndex]; },
                value => { var c = Pokeathlon.GlobalCounters; c[eventIndex] = value; });
        }

        AddCounterRow("TotalEventFirst", PokeathlonGlobalCounters4.MaxStat * (decimal)PokeathlonEvent4.Count,
            () => { var c = Pokeathlon.GlobalCounters; return c.TotalEventFirst; }, null);
        AddCounterRow("TotalEventLast", PokeathlonGlobalCounters4.MaxStat,
            () => Pokeathlon.GlobalCounters.TotalEventLast,
            value => { var c = Pokeathlon.GlobalCounters; c.TotalEventLast = value; });
        AddCounterRow("Fame", PokeathlonGlobalCounters4.MaxFame,
            () => Pokeathlon.GlobalCounters.Fame,
            value => { var c = Pokeathlon.GlobalCounters; c.Fame = value; });
    }

    private void InitializeBestScores()
    {
        for (int i = 0; i < (int)PokeathlonEvent4.Count; i++)
        {
            var label = new Label
            {
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                Text = GetEventDisplayName((PokeathlonEvent4)i) + ':',
            };
            var editor = CreateNumericEditor(ushort.MaxValue);
            TLP_Best.RowStyles.Add(new RowStyle());
            TLP_Best.Controls.Add(label, 0, i);
            TLP_Best.Controls.Add(editor, 1, i);
            BestScoreEditors.Add(((PokeathlonEvent4)i, editor));
        }

        int spacerRow = BestScoreEditors.Count;
        TLP_Best.RowStyles.Add(new RowStyle());
        B_BestSpacer.Enabled = false;
        B_BestSpacer.TabStop = false;
        B_BestSpacer.Margin = Padding.Empty;
        B_BestSpacer.Size = new Size(120, 1);
        TLP_Best.Controls.Add(B_BestSpacer, 1, spacerRow);
    }

    private void InitializeIndexes()
    {
        InitializeCombo(CB_CourseIndex, [.. Enum.GetValues<PokeathlonStat4>().Take((int)PokeathlonStat4.Count).Select(z => new ComboItem(GetCourseDisplayName(z), (int)z))]);
        List<ComboItem> events = [.. Enum.GetValues<PokeathlonEvent4>().Take((int)PokeathlonEvent4.Count).Select(z => new ComboItem(GetEventDisplayName(z), (int)z))];
        InitializeCombo(CB_SelfEventIndex, events);
        InitializeCombo(CB_ConnectionIndex, events);
    }

    private void LoadData()
    {
        IsLoading = true;
        LoadGeneral();
        LoadMedals();
        LoadCounters();
        LoadBestScores();
        CB_CourseIndex.SelectedValue = 0;
        CB_SelfEventIndex.SelectedValue = 0;
        CB_ConnectionIndex.SelectedValue = 0;
        IsLoading = false;

        LoadCourse(0);
        LoadSelfEvent(0);
        LoadConnection(0);
    }

    private void LoadGeneral()
    {
        NUD_Points.SetValueClamped(Pokeathlon.Points);

        var dailyFlags = Pokeathlon.FlagsDailyShop;
        for (int i = 0; i < DailyShopEditors.Length; i++)
            DailyShopEditors[i].Checked = ((dailyFlags >> i) & 1) != 0;

        var dataCardFlags = Pokeathlon.FlagsDataCard;
        for (int i = 0; i < CLB_DataCards.Items.Count; i++)
            CLB_DataCards.SetItemChecked(i, ((dataCardFlags >> i) & 1) != 0);
    }

    private void SaveGeneral()
    {
        Pokeathlon.Points = (uint)NUD_Points.Value;

        ushort dailyFlags = 0;
        for (int i = 0; i < DailyShopEditors.Length; i++)
        {
            if (DailyShopEditors[i].Checked)
                dailyFlags |= (ushort)(1 << i);
        }
        Pokeathlon.FlagsDailyShop = dailyFlags;

        uint dataCardFlags = 0;
        for (int i = 0; i < CLB_DataCards.Items.Count; i++)
        {
            if (CLB_DataCards.GetItemChecked(i))
                dataCardFlags |= 1u << i;
        }
        Pokeathlon.FlagsDataCard = dataCardFlags;
    }

    private void LoadMedals()
    {
        var medals = Pokeathlon.Medals;
        for (int species = 1; species <= MaxSpeciesGen4; species++)
        {
            var bits = medals.GetMedal((ushort)species);
            var row = DGV_Medals.Rows[species - 1];
            for (int i = 0; i < (int)PokeathlonStat4.Count; i++)
                row.Cells[2 + i].Value = ((bits >> i) & 1) != 0;
        }
    }

    private void SaveMedals()
    {
        var medals = Pokeathlon.Medals;
        for (int species = 1; species <= MaxSpeciesGen4; species++)
        {
            byte bits = 0;
            var row = DGV_Medals.Rows[species - 1];
            for (int i = 0; i < (int)PokeathlonStat4.Count; i++)
            {
                if ((bool?)row.Cells[2 + i].Value == true)
                    bits |= (byte)(1 << i);
            }
            medals.SetMedal((ushort)species, bits);
        }
    }

    private void LoadCounters()
    {
        foreach (var binding in CounterEditors)
            binding.Editor.SetValueClamped(binding.Getter());
    }

    private void SaveCounters()
    {
        foreach (var binding in CounterEditors)
        {
            if (binding.Setter is not null)
                binding.Setter((uint)binding.Editor.Value);
        }
    }

    private void LoadBestScores()
    {
        foreach (var binding in BestScoreEditors)
            binding.Editor.SetValueClamped(Pokeathlon.GetBestScore(binding.Event));
    }

    private void SaveBestScores()
    {
        foreach (var binding in BestScoreEditors)
            Pokeathlon.SetBestScore(binding.Event, (ushort)binding.Editor.Value);
    }

    private void LoadCourse(int index)
    {
        IsLoading = true;
        var course = Pokeathlon.GetCourseRecord((PokeathlonStat4)index);
        CourseScoreEditors[0].SetValueClamped(course.Score0);
        CourseScoreEditors[1].SetValueClamped(course.Score1);
        CourseScoreEditors[2].SetValueClamped(course.Score2);
        CourseScoreEditors[3].SetValueClamped(course.ScoreMax);
        for (int i = 0; i < CourseParticipantEditors.Length; i++)
            CourseParticipantEditors[i].LoadObject(course.GetParticipant(i));
        CurrentCourseIndex = index;
        IsLoading = false;
    }

    private void SaveCourse(int index)
    {
        var course = Pokeathlon.GetCourseRecord((PokeathlonStat4)index);
        course.Score0 = (ushort)CourseScoreEditors[0].Value;
        course.Score1 = (ushort)CourseScoreEditors[1].Value;
        course.Score2 = (ushort)CourseScoreEditors[2].Value;
        course.ScoreMax = (ushort)CourseScoreEditors[3].Value;
        for (int i = 0; i < CourseParticipantEditors.Length; i++)
            CourseParticipantEditors[i].SaveObject(course.GetParticipant(i));
    }

    private void LoadSelfEvent(int index)
    {
        IsLoading = true;
        UC_SelfEventData.LoadObject(Pokeathlon.GetEventSelf((PokeathlonEvent4)index));
        CurrentSelfEventIndex = index;
        IsLoading = false;
    }

    private void SaveSelfEvent(int index)
    {
        UC_SelfEventData.SaveObject(Pokeathlon.GetEventSelf((PokeathlonEvent4)index));
    }

    private void LoadConnection(int index)
    {
        IsLoading = true;
        UC_Connection.LoadObject(Pokeathlon.GetEventConnection((PokeathlonEvent4)index));
        CurrentConnectionIndex = index;
        IsLoading = false;
    }

    private void SaveConnection(int index)
    {
        UC_Connection.SaveObject(Pokeathlon.GetEventConnection((PokeathlonEvent4)index));
    }

    private void SaveCurrentViews()
    {
        if (CurrentCourseIndex >= 0)
            SaveCourse(CurrentCourseIndex);
        if (CurrentSelfEventIndex >= 0)
            SaveSelfEvent(CurrentSelfEventIndex);
        if (CurrentConnectionIndex >= 0)
            SaveConnection(CurrentConnectionIndex);
    }

    private void AddCounterRow(string name, decimal maximum, Func<uint> getter, Action<uint>? setter)
    {
        int row = CounterEditors.Count;
        TLP_Counters.RowStyles.Add(new RowStyle());
        var label = new Label
        {
            Name = $"L_{name}",
            Anchor = AnchorStyles.Right,
            AutoSize = true,
            Text = name,
        };
        var editor = CreateNumericEditor(maximum, setter is not null);
        TLP_Counters.Controls.Add(label, 0, row);
        TLP_Counters.Controls.Add(editor, 1, row);
        CounterEditors.Add(new CounterBinding(name, editor, getter, setter));
    }

    private static NumericUpDown CreateNumericEditor(decimal maximum, bool enabled = true) => new()
    {
        Maximum = maximum,
        Size = new Size(120, 25),
        Enabled = enabled,
    };

    private static void InitializeCombo(ComboBox cb, IReadOnlyList<ComboItem> source)
    {
        cb.InitializeBinding();
        cb.DataSource = new BindingSource(source, string.Empty);
    }

    private void B_MedalsGiveAll_Click(object? sender, EventArgs e)
    {
        var medals = Pokeathlon.Medals;
        medals.SetAllMedals();
        LoadMedals();
        WinFormsUtil.Asterisk();
    }

    private void B_MedalsClearAll_Click(object? sender, EventArgs e)
    {
        var medals = Pokeathlon.Medals;
        medals.Clear();
        LoadMedals();
        WinFormsUtil.Asterisk();
    }

    private sealed record CounterBinding(string Name, NumericUpDown Editor, Func<uint> Getter, Action<uint>? Setter);
}
