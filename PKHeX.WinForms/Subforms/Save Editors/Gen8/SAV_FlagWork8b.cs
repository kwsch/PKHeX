using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.GameVersion;

namespace PKHeX.WinForms;

public sealed partial class SAV_FlagWork8b : Form
{
    private readonly SAV8BS Origin;
    private readonly SAV8BS SAV;

    private bool editing;
    private readonly Dictionary<int, NumericUpDown> WorkDict = [];
    private readonly Dictionary<int, CheckBox> FlagDict = [];
    private readonly Dictionary<int, CheckBox> SystemDict = [];

    public SAV_FlagWork8b(SAV8BS sav)
    {
        InitializeComponent();

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        SAV = (SAV8BS)sav.Clone();
        Origin = sav;
        TC_Features.SelectedTab = GB_Research;

        AllowDrop = true;
        DragEnter += Main_DragEnter;
        DragDrop += Main_DragDrop;

        NUD_WorkIndex.Minimum = 0;
        NUD_WorkIndex.Maximum = SAV.FlagWork.CountWork - 1;

        SuspendLayout();
        editing = true;
        var obj = SAV.FlagWork;
        var game = GetGameFilePrefix(SAV.Version);
        var editor = new EventLabelCollectionSystem(game, obj.CountFlag - 1, obj.CountSystem - 1, obj.CountWork - 1);
        LoadFlags(editor);
        LoadSystem(editor);
        LoadWork(editor);

        if (Application.IsDarkModeEnabled)
        {
            WinFormsTranslator.ReformatDark(TC_Flags);
            WinFormsTranslator.ReformatDark(TC_System);
            WinFormsTranslator.ReformatDark(TC_Work);
            WinFormsTranslator.ReformatDark(TC_Features);
        }

        editing = false;
        ResumeLayout();

        NUD_Flag.Maximum = obj.CountFlag - 1;
        NUD_Flag.Text = "0";
        CHK_CustomFlag.Checked = obj.GetFlag(0);
        NUD_System.Maximum = obj.CountSystem - 1;
        NUD_System.Text = "0";
        CHK_CustomSystem.Checked = obj.GetSystemFlag(0);

        ChangeConstantIndex(this, EventArgs.Empty);

        Text = $"{Text} ({sav.Version})";
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        TC_Features.SelectedIndex = 0;
    }

    private void LoadFlags(EventLabelCollectionSystem editor)
    {
        FlagDict.Clear();
        TC_Flags.TabPages.Clear();
        var labels = editor.Flag;
        foreach (var group in labels.GroupBy(z => z.Type).OrderBy(z => (int)z.Key))
        {
            var tab = new TabPage
            {
                Name = $"Tab_F{group.Key}",
                Text = group.Key.ToString(),
            };

            var panel = CreateBoolPanel($"TLP_F{group.Key}");
            var rows = new List<(string Search, CheckBox Check, Label Label)>();
            TC_Flags.TabPages.Add(tab);

            foreach (var (name, index, _) in group)
            {
                var lbl = new Label { Text = name, Margin = Padding.Empty, AutoSize = true };
                var chk = new CheckBox
                {
                    CheckAlign = ContentAlignment.MiddleLeft,
                    Margin = Padding.Empty,
                    Checked = SAV.FlagWork.GetFlag(index),
                    AutoSize = true,
                };
                lbl.Click += (_, _) => chk.Checked ^= true;
                chk.CheckedChanged += (_, _) =>
                {
                    SAV.FlagWork.SetFlag(index, chk.Checked);
                    if (NUD_Flag.Value == index)
                        CHK_CustomFlag.Checked = chk.Checked;
                };
                rows.Add((name, chk, lbl));
                FlagDict.Add(index, chk);
            }

            ApplyBoolFilter(panel, rows, string.Empty);
            var search = CreateSearchBox(text => ApplyBoolFilter(panel, rows, text));
            var host = CreateSearchHost(search, panel);
            tab.Controls.Add(host);
        }
    }

    private void LoadSystem(EventLabelCollectionSystem editor)
    {
        SystemDict.Clear();
        TC_System.TabPages.Clear();
        var labels = editor.System;
        foreach (var group in labels.GroupBy(z => z.Type).OrderBy(z => (int)z.Key))
        {
            var tab = new TabPage
            {
                Name = $"Tab_S{group.Key}",
                Text = group.Key.ToString(),
            };

            var panel = CreateBoolPanel($"TLP_S{group.Key}");
            var rows = new List<(string Search, CheckBox Check, Label Label)>();
            TC_System.TabPages.Add(tab);

            foreach (var (name, index, _) in group)
            {
                var lbl = new Label { Text = name, Margin = Padding.Empty, AutoSize = true };
                var chk = new CheckBox
                {
                    CheckAlign = ContentAlignment.MiddleLeft,
                    Margin = Padding.Empty,
                    Checked = SAV.FlagWork.GetSystemFlag(index),
                    AutoSize = true,
                };
                lbl.Click += (_, _) => chk.Checked ^= true;
                chk.CheckedChanged += (_, _) =>
                {
                    SAV.FlagWork.SetSystemFlag(index, chk.Checked);
                    if (NUD_System.Value == index)
                        CHK_CustomSystem.Checked = chk.Checked;
                };
                rows.Add((name, chk, lbl));
                SystemDict.Add(index, chk);
            }

            ApplyBoolFilter(panel, rows, string.Empty);
            var search = CreateSearchBox(text => ApplyBoolFilter(panel, rows, text));
            var host = CreateSearchHost(search, panel);
            tab.Controls.Add(host);
        }
    }

    private void LoadWork(EventLabelCollectionSystem editor)
    {
        WorkDict.Clear();
        TC_Work.TabPages.Clear();
        var labels = editor.Work;
        foreach (var group in labels.GroupBy(z => z.Type).OrderBy(z => (int)z.Key))
        {
            var tab = new TabPage
            {
                Name = $"Tab_W{group.Key}",
                Text = group.Key.ToString(),
            };

            var panel = CreateWorkPanel($"TLP_W{group.Key}");
            var rows = new List<(string Search, Label Label, ComboBox Combo, NumericUpDown Numeric)>();
            TC_Work.TabPages.Add(tab);

            foreach (var entry in group)
            {
                var lbl = new Label { Text = entry.Name, Margin = Padding.Empty, AutoSize = true };
                var mtb = new NumericUpDown
                {
                    Maximum = int.MaxValue,
                    Minimum = int.MinValue,
                    Margin = Padding.Empty,
                    Width = 85,
                };

                var map = entry.PredefinedValues.Select(z => new ComboItem(z.Name, z.Value)).ToList();
                var cb = new ComboBox
                {
                    Margin = Padding.Empty,
                    Width = 165,
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
                    var value = (int)mtb.Value;
                    var (_, valueID) = map.Find(z => z.Value == value) ?? map[0];
                    if (WinFormsUtil.GetIndex(cb) != valueID)
                        cb.SelectedValue = valueID;

                    SAV.FlagWork.SetWork(entry.Index, value);
                    if (NUD_WorkIndex.Value == entry.Index)
                        NUD_Work.Value = value;
                    updating = false;
                }
                cb.SelectedValueChanged += (_, _) =>
                {
                    if (editing || updating)
                        return;
                    var value = WinFormsUtil.GetIndex(cb);
                    mtb.Value = value == NamedEventConst.CustomMagicValue ? 0 : value;
                };

                mtb.Value = SAV.FlagWork.GetWork(entry.Index);
                if (mtb.Value == 0)
                    ChangeConstValue(this, EventArgs.Empty);

                rows.Add((entry.Name, lbl, cb, mtb));
                WorkDict.Add(entry.Index, mtb);
            }

            ApplyWorkFilter(panel, rows, string.Empty);
            var search = CreateSearchBox(text => ApplyWorkFilter(panel, rows, text));
            var host = CreateSearchHost(search, panel);
            tab.Controls.Add(host);
        }
    }

    private static void ApplyBoolFilter(TableLayoutPanel panel, IReadOnlyList<(string Search, CheckBox Check, Label Label)> rows, string text)
    {
        var query = text.Trim();
        var hasQuery = query.Length != 0;

        panel.SuspendLayout();
        panel.Controls.Clear();
        var rowIndex = 0;
        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (hasQuery && !row.Search.Contains(query, StringComparison.CurrentCultureIgnoreCase))
                continue;

            panel.Controls.Add(row.Check, 0, rowIndex);
            panel.Controls.Add(row.Label, 1, rowIndex);
            rowIndex++;
        }
        panel.ResumeLayout();
    }

    private static void ApplyWorkFilter(TableLayoutPanel panel, IReadOnlyList<(string Search, Label Label, ComboBox Combo, NumericUpDown Numeric)> rows, string text)
    {
        var query = text.Trim();
        var hasQuery = query.Length != 0;

        panel.SuspendLayout();
        panel.Controls.Clear();
        var rowIndex = 0;
        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (hasQuery && !row.Search.Contains(query, StringComparison.CurrentCultureIgnoreCase))
                continue;

            panel.Controls.Add(row.Label, 0, rowIndex);
            panel.Controls.Add(row.Combo, 1, rowIndex);
            panel.Controls.Add(row.Numeric, 2, rowIndex);
            rowIndex++;
        }
        panel.ResumeLayout();
    }

    private TextBox CreateSearchBox(Action<string> applyFilter)
    {
        var box = new TextBox
        {
            PlaceholderText = "Search...",
            Dock = DockStyle.Top,
            Margin = Padding.Empty,
        };
        if (Application.IsDarkModeEnabled)
            WinFormsTranslator.ReformatDark(box);

        var timer = new Timer { Interval = 150 };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            applyFilter(box.Text);
        };
        box.TextChanged += (_, _) =>
        {
            timer.Stop();
            timer.Start();
        };
        box.Disposed += (_, _) => timer.Dispose();
        return box;
    }

    private static Panel CreateSearchHost(TextBox search, Control content)
    {
        var host = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
        };
        content.Dock = DockStyle.Fill;
        host.Controls.Add(content);
        host.Controls.Add(search);
        return host;
    }

    private static TableLayoutPanel CreateBoolPanel(string name)
    {
        var panel = new TableLayoutPanel
        {
            AutoScroll = true,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(4, 3, 4, 3),
            Name = name,
            RowCount = 2,
        };
        panel.ColumnStyles.Add(new ColumnStyle());
        panel.ColumnStyles.Add(new ColumnStyle());
        panel.RowStyles.Add(new RowStyle());
        panel.RowStyles.Add(new RowStyle());
        panel.Scroll += WinFormsUtil.PanelScroll;
        return panel;
    }

    private static TableLayoutPanel CreateWorkPanel(string name)
    {
        var panel = new TableLayoutPanel
        {
            AutoScroll = true,
            ColumnCount = 3,
            Dock = DockStyle.Fill,
            Margin = new Padding(4, 3, 4, 3),
            Name = name,
            RowCount = 1,
        };
        panel.ColumnStyles.Add(new ColumnStyle());
        panel.ColumnStyles.Add(new ColumnStyle());
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 579F));
        panel.RowStyles.Add(new RowStyle());
        panel.RowStyles.Add(new RowStyle());
        panel.Scroll += WinFormsUtil.PanelScroll;
        return panel;
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void ChangeCustomFlag(object sender, EventArgs e) => CHK_CustomFlag.Checked = SAV.FlagWork.GetFlag((int)NUD_Flag.Value);
    private void ChangeCustomSystem(object sender, EventArgs e) => CHK_CustomSystem.Checked = SAV.FlagWork.GetSystemFlag((int)NUD_System.Value);
    private void ChangeConstantIndex(object sender, EventArgs e) => NUD_Work.Value = SAV.FlagWork.GetWork((int)NUD_WorkIndex.Value);

    private void ChangeSAV()
    {
        if (TB_NewSAV.Text.Length != 0 && TB_OldSAV.Text.Length != 0)
            DiffSaves();
    }

    private void OpenSAV(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Title = MessageStrings.MsgFileLoadSaveSelectGame;
        if (ofd.ShowDialog() == DialogResult.OK)
            LoadSAV(sender, ofd.FileName);
    }

    private void LoadSAV(object sender, string path)
    {
        if (sender == B_LoadOld)
            TB_OldSAV.Text = path;
        else
            TB_NewSAV.Text = path;
        ChangeSAV();
    }

    private static string GetGameFilePrefix(GameVersion version) => version switch
    {
        BD or SP or BDSP => "bdsp",
        _ => throw new IndexOutOfRangeException(nameof(version)),
    };

    private void DiffSaves()
    {
        var diff = new EventWorkDiff8b(TB_OldSAV.Text, TB_NewSAV.Text);
        if (diff.Message != EventWorkDiffCompatibility.Valid)
        {
            WinFormsUtil.Alert(diff.Message.GetMessage());
            return;
        }

        RTB_Diff.Lines = [.. diff.Summarize()];
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

    private void B_ApplyFlag_Click(object sender, EventArgs e)
    {
        var index = (int)NUD_Flag.Value;
        SAV.FlagWork.SetFlag(index, CHK_CustomFlag.Checked);

        editing = true;
        if (FlagDict.TryGetValue(index, out var chk))
            chk.Checked = CHK_CustomFlag.Checked;
        editing = false;
    }

    private void B_ApplySystemFlag_Click(object sender, EventArgs e)
    {
        var index = (int)NUD_System.Value;
        SAV.FlagWork.SetSystemFlag(index, CHK_CustomSystem.Checked);

        editing = true;
        if (SystemDict.TryGetValue(index, out var chk))
            chk.Checked = CHK_CustomSystem.Checked;
        editing = false;
    }

    private void B_ApplyWork_Click(object sender, EventArgs e)
    {
        var index = (int)NUD_WorkIndex.Value;
        SAV.FlagWork.SetWork(index, (int)NUD_Work.Value);

        editing = true;
        if (WorkDict.TryGetValue(index, out var nud))
            nud.Value = NUD_Work.Value;
        editing = false;
    }
}
