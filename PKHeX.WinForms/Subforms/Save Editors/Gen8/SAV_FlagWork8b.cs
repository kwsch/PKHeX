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

    private void LoadFlags(EventLabelCollectionSystem editor)
    {
        TLP_Flags.SuspendLayout();
        TLP_Flags.Scroll += WinFormsUtil.PanelScroll;
        TLP_Flags.Controls.Clear();
        IEnumerable<NamedEventValue> labels = editor.Flag;

        var hide = Main.Settings.Advanced.HideEventTypeBelow;
        labels = labels.OrderByDescending(z => z.Type);
        int i = 0;
        foreach (var (name, index, type) in labels)
        {
            if (type < hide)
                break;

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
            TLP_Flags.Controls.Add(chk, 0, i);
            TLP_Flags.Controls.Add(lbl, 1, i);

            FlagDict.Add(index, chk);
            i++;
        }

        TLP_Flags.ResumeLayout();
    }

    private void LoadSystem(EventLabelCollectionSystem editor)
    {
        TLP_System.SuspendLayout();
        TLP_System.Scroll += WinFormsUtil.PanelScroll;
        TLP_System.Controls.Clear();
        IEnumerable<NamedEventValue> labels = editor.System;

        var hide = Main.Settings.Advanced.HideEventTypeBelow;
        labels = labels.OrderByDescending(z => z.Type);
        int i = 0;
        foreach (var (name, index, type) in labels)
        {
            if (type < hide)
                break;

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
            TLP_System.Controls.Add(chk, 0, i);
            TLP_System.Controls.Add(lbl, 1, i);

            SystemDict.Add(index, chk);
            i++;
        }

        TLP_System.ResumeLayout();
    }

    private void LoadWork(EventLabelCollectionSystem editor)
    {
        TLP_Work.SuspendLayout();
        TLP_Work.Scroll += WinFormsUtil.PanelScroll;
        TLP_Work.Controls.Clear();
        IEnumerable<NamedEventWork> labels = editor.Work;
        var hide = Main.Settings.Advanced.HideEventTypeBelow;
        labels = labels.OrderByDescending(z => z.Type);
        int i = 0;
        foreach (var entry in labels)
        {
            if (entry.Type < hide)
                break;
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
                    mtb.Text = ((int)mtb.Value).ToString();
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

            TLP_Work.Controls.Add(lbl, 0, i);
            TLP_Work.Controls.Add(cb, 1, i);
            TLP_Work.Controls.Add(mtb, 2, i);

            WorkDict.Add(entry.Index, mtb);
            i++;
        }

        TLP_Work.ResumeLayout();
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

    private static string GetGameFilePrefix(GameVersion game) => game switch
    {
        BD or SP or BDSP => "bdsp",
        _ => throw new IndexOutOfRangeException(nameof(game)),
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
        var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, Name, "Yes: Old Save" + Environment.NewLine + "No: New Save");
        var button = dr == DialogResult.Yes ? B_LoadOld : B_LoadNew;
        LoadSAV(button, files[0]);
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
