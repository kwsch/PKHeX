using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public sealed partial class SAV_EventFlags2 : Form
{
    private readonly EventWorkspace<SAV2, byte> Editor;
    private readonly Dictionary<int, NumericUpDown> WorkDict = [];
    private readonly Dictionary<int, (DataGridView Grid, int RowIndex)> FlagDict = [];

    private bool editing;

    public SAV_EventFlags2(SAV2 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        var editor = Editor = new EventWorkspace<SAV2, byte>(sav, sav.Version);
        AllowDrop = true;
        DragEnter += Main_DragEnter;
        DragDrop += Main_DragDrop;
        tabControl1.SelectedTab = GB_Research;

        editing = true;
        CB_Stats.Items.Clear();
        for (int i = 0; i < editor.Values.Length; i++)
            CB_Stats.Items.Add(i.ToString());

        TC_Flags.SuspendLayout();
        TC_Const.SuspendLayout();
        AddFlagList(editor.Labels, editor.Flags);
        AddConstList(editor.Labels, editor.Values);

        if (Application.IsDarkModeEnabled)
        {
            WinFormsTranslator.ReformatDark(TC_Flags);
            WinFormsTranslator.ReformatDark(TC_Const);
        }

        TC_Flags.ResumeLayout();
        TC_Const.ResumeLayout();

        Text = $"{Text} ({sav.Version})";

        if (CB_Stats.Items.Count > 0)
        {
            CB_Stats.SelectedIndex = 0;
        }
        else
        {
            L_Stats.Visible = CB_Stats.Visible = MT_Stat.Visible = false;
            tabControl1.TabPages.Remove(GB_Constants);
        }
        NUD_Flag.Maximum = editor.Flags.Length - 1;
        NUD_Flag.Value = 0;
        c_CustomFlag.Checked = editor.Flags[0];
        editing = false;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        tabControl1.SelectedIndex = 0;
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Editor.Save();
        Close();
    }

    private void AddFlagList(EventLabelCollection list, bool[] values)
    {
        FlagDict.Clear();
        TC_Flags.TabPages.Clear();

        var labels = list.Flag;
        if (labels.Count == 0)
        {
            TC_Flags.Visible = false;
            var research = new Label { Text = MsgResearchRequired, Name = "TLP_Flags_Research", ForeColor = WinFormsUtil.ColorWarn, AutoSize = true, Location = new Point(20, 20) };
            GB_Flags.Controls.Add(research);
            return;
        }

        foreach (var group in labels.GroupBy(z => z.Type).OrderBy(z => (int)z.Key))
        {
            var tab = new TabPage
            {
                Name = $"Tab_F{group.Key}",
                Text = group.Key.ToString(),
            };

            var grid = CreateFlagGrid();
            tab.Controls.Add(grid);
            TC_Flags.TabPages.Add(tab);

            var cFlag = new DataGridViewCheckBoxColumn
            {
                DisplayIndex = 0,
                Width = 20,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            var cLabel = new DataGridViewTextBoxColumn
            {
                DisplayIndex = 1,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };

            cFlag.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cLabel.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            grid.Columns.Add(cFlag);
            grid.Columns.Add(cLabel);

            var grouped = group.ToList();
            grid.Rows.Add(grouped.Count);

            for (var i = 0; i < grouped.Count; i++)
                FlagDict[grouped[i].Index] = (grid, i);

            for (var i = 0; i < grouped.Count; i++)
            {
                var (name, index, _) = grouped[i];
                var cells = grid.Rows[i].Cells;
                cells[0].Value = values[index];
                cells[1].Value = name;
            }

            grid.CellValueChanged += (_, e) =>
            {
                if (e.ColumnIndex != 0 || e.RowIndex == -1)
                    return;

                var chk = (bool)grid.Rows[e.RowIndex].Cells[0].Value!;
                var index = grouped[e.RowIndex].Index;
                values[index] = chk;
                if (NUD_Flag.Value == index)
                    c_CustomFlag.Checked = chk;
            };
            grid.CellMouseUp += (_, e) =>
            {
                if (e.RowIndex == -1)
                    return;

                if (e.ColumnIndex == 0)
                {
                    grid.EndEdit();
                    return;
                }

                if (e.ColumnIndex != 1)
                    return;

                var chk = (bool)grid.Rows[e.RowIndex].Cells[0].Value!;
                grid.Rows[e.RowIndex].Cells[0].Value = !chk;
                var index = grouped[e.RowIndex].Index;
                values[index] = !chk;
                if (NUD_Flag.Value == index)
                    c_CustomFlag.Checked = !chk;
            };
        }
    }

    private static DoubleBufferedDataGridView CreateFlagGrid() => new()
    {
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeColumns = false,
        AllowUserToResizeRows = false,
        BackgroundColor = SystemColors.ControlLightLight,
        BorderStyle = BorderStyle.None,
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        ColumnHeadersVisible = false,
        Dock = DockStyle.Fill,
        EditMode = DataGridViewEditMode.EditOnEnter,
        Margin = Padding.Empty,
        MultiSelect = false,
        RowHeadersVisible = false,
        SelectionMode = DataGridViewSelectionMode.CellSelect,
        ShowEditingIcon = false,
    };

    private void AddConstList(EventLabelCollection list, ReadOnlySpan<byte> values)
    {
        WorkDict.Clear();
        TC_Const.TabPages.Clear();

        var labels = list.Work;
        if (labels.Count == 0)
        {
            TC_Const.Visible = false;
            GB_Constants.Controls.Add(new Label { Text = MsgResearchRequired, Name = "TLP_Const_Research", ForeColor = WinFormsUtil.ColorWarn, AutoSize = true, Location = new Point(20, 20) });
            return;
        }

        foreach (var group in labels.GroupBy(z => z.Type).OrderBy(z => (int)z.Key))
        {
            var tab = new TabPage
            {
                Name = $"Tab_W{group.Key}",
                Text = WinFormsTranslator.TranslateEnum(group.Key, Main.CurrentLanguage),
            };

            var panel = CreateConstPanel();
            tab.Controls.Add(panel);
            TC_Const.TabPages.Add(tab);

            var grouped = group.ToList();
            for (var i = 0; i < grouped.Count; i++)
            {
                var entry = grouped[i];
                var lbl = new Label { Text = entry.Name, Margin = Padding.Empty, AutoSize = true };
                var mtb = new NumericUpDown
                {
                    Maximum = byte.MaxValue,
                    Minimum = byte.MinValue,
                    Margin = Padding.Empty,
                    Width = 50,
                };

                var map = entry.PredefinedValues.Select(z => new ComboItem(z.Name, z.Value)).ToList();
                var cb = new ComboBox
                {
                    Margin = Padding.Empty,
                    Width = 150,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BindingContext = BindingContext,
                    DropDownWidth = Width + 100,
                };
                if (Application.IsDarkModeEnabled)
                    WinFormsTranslator.ReformatDark(cb);
                cb.InitializeBinding();
                cb.DataSource = map;

                lbl.Click += (_, _) => mtb.Value = 0;
                bool updating = false;
                mtb.ValueChanged += ChangeConstValue;
                void ChangeConstValue(object? sender, EventArgs e)
                {
                    if (updating)
                        return;

                    updating = true;
                    var value = (byte)mtb.Value;
                    var (_, valueID) = map.Find(z => z.Value == value) ?? map[0];
                    if (WinFormsUtil.GetIndex(cb) != valueID)
                        cb.SelectedValue = valueID;

                    Editor.Values[entry.Index] = value;
                    if (CB_Stats.SelectedIndex == entry.Index)
                        MT_Stat.Text = ((int)mtb.Value).ToString();
                    updating = false;
                }
                cb.SelectedValueChanged += (_, _) =>
                {
                    if (editing || updating)
                        return;
                    var value = WinFormsUtil.GetIndex(cb);
                    mtb.Value = value == NamedEventConst.CustomMagicValue ? 0 : value;
                };

                mtb.Value = values[entry.Index];
                if (mtb.Value == 0)
                    ChangeConstValue(this, EventArgs.Empty);

                panel.Controls.Add(lbl, 0, i);
                panel.Controls.Add(cb, 1, i);
                panel.Controls.Add(mtb, 2, i);

                WorkDict.Add(entry.Index, mtb);
            }
        }
    }

    private static TableLayoutPanel CreateConstPanel()
    {
        var panel = new TableLayoutPanel
        {
            AutoScroll = true,
            ColumnCount = 3,
            Dock = DockStyle.Fill,
            Margin = new Padding(4, 3, 4, 3),
            Name = "TLP_Const",
            RowCount = 1,
        };
        panel.ColumnStyles.Add(new ColumnStyle());
        panel.ColumnStyles.Add(new ColumnStyle());
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 485F));
        panel.RowStyles.Add(new RowStyle());
        panel.RowStyles.Add(new RowStyle());
        panel.Scroll += WinFormsUtil.PanelScroll;
        return panel;
    }

    private void ChangeCustomBool(object sender, EventArgs e)
    {
        if (editing)
            return;
        editing = true;
        var index = (int)NUD_Flag.Value;
        Editor.Flags[index] = c_CustomFlag.Checked;
        if (FlagDict.TryGetValue(index, out var result))
            result.Grid.Rows[result.RowIndex].Cells[0].Value = c_CustomFlag.Checked;
        editing = false;
    }

    private void ChangeCustomFlag(object sender, EventArgs e)
    {
        int flag = (int)NUD_Flag.Value;
        c_CustomFlag.Checked = Editor.Flags[flag];
    }

    private void ChangeCustomFlag(object sender, KeyEventArgs e) => ChangeCustomFlag(sender, (EventArgs)e);

    private void ChangeConstantIndex(object sender, EventArgs e)
    {
        var constants = Editor.Values;
        var index = CB_Stats.SelectedIndex;
        MT_Stat.Text = constants[index].ToString();
    }

    private void ChangeCustomConst(object sender, EventArgs e)
    {
        if (editing)
            return;
        editing = true;
        var index = CB_Stats.SelectedIndex;
        var parse = byte.TryParse(MT_Stat.Text, out var value) ? value : (byte)0;
        Editor.Values[index] = parse;
        if (WorkDict.TryGetValue(index, out var mtb))
            mtb.Value = parse;
        editing = false;
    }

    private void ChangeSAV(object sender, EventArgs e) => ChangeSAV();

    private void ChangeSAV()
    {
        if (TB_NewSAV.Text.Length != 0 && TB_OldSAV.Text.Length != 0)
            DiffSaves();
    }

    private void OpenSAV(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Title = MsgFileLoadSaveSelectGame;
        if (ofd.ShowDialog() == DialogResult.OK)
            LoadSAV(sender, ofd.FileName);
    }

    private void LoadSAV(object sender, string path)
    {
        var dest = sender == B_LoadOld ? TB_OldSAV : TB_NewSAV;
        dest.Text = path;
    }

    private void DiffSaves()
    {
        var diff = new EventBlockDiff<SAV2, byte>(TB_OldSAV.Text, TB_NewSAV.Text);
        if (diff.Message != EventWorkDiffCompatibility.Valid)
        {
            WinFormsUtil.Alert(diff.Message.GetMessage());
            return;
        }

        TB_IsSet.Text = string.Join(", ", diff.SetFlags.Select(z => $"{z:0000}"));
        TB_UnSet.Text = string.Join(", ", diff.ClearedFlags.Select(z => $"{z:0000}"));

        if (diff.WorkDiff.Count == 0)
        {
            WinFormsUtil.Alert("No Event Constant diff found.");
            return;
        }

        var promptCopy = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Copy Event Constant diff to clipboard?");
        if (promptCopy == DialogResult.Yes)
            WinFormsUtil.SetClipboardText(string.Join(Environment.NewLine, diff.WorkDiff));
    }

    private static void Main_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e?.Data is null)
            return;
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
    }

    private void Main_DragDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;

        foreach (var file in files)
        {
            var result = this.SelectNewOld(file, B_LoadNew.Text, B_LoadOld.Text);
            if (result == DualDiffSelection.New)
                TB_NewSAV.Text = file;
            else if (result == DualDiffSelection.Old)
                TB_OldSAV.Text = file;
            else if (ModifierKeys == Keys.Escape)
                return;
        }
        ChangeSAV();
    }
}
