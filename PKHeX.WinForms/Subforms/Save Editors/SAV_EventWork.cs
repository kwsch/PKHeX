using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.GameVersion;

namespace PKHeX.WinForms;

public sealed partial class SAV_EventWork : Form
{
    private readonly SAV7b Origin;
    private readonly EventWork7b SAV;
    private readonly SplitEventEditor<int> Editor;

    public SAV_EventWork(SAV7b sav)
    {
        InitializeComponent();

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        SAV = sav.Blocks.EventWork;
        Origin = sav;

        DragEnter += Main_DragEnter;
        DragDrop += Main_DragDrop;

        CB_Stats.Items.Clear();
        for (int i = 0; i < SAV.CountWork; i++)
            CB_Stats.Items.Add(i.ToString());

        var work = GetStringList(sav.Version, "const");
        var flag = GetStringList(sav.Version, "flags");
        Editor = new SplitEventEditor<int>(SAV, work, flag);

        SuspendLayout();
        editing = true;
        LoadFlags(Editor.Flag);
        LoadWork(Editor.Work);
        editing = false;
        ResumeLayout();

        if (CB_Stats.Items.Count > 0)
        {
            CB_Stats.SelectedIndex = 0;
        }
        else
        {
            L_Stats.Visible = CB_Stats.Visible = NUD_Stat.Visible = false;
            TC_Features.TabPages.Remove(GB_Constants);
        }
        NUD_Flag.Maximum = SAV.CountFlag - 1;
        NUD_Flag.Text = "0";
        c_CustomFlag.Checked = SAV.GetFlag(0);

        Text = $"{Text} ({sav.Version})";
    }

    private void LoadFlags(IEnumerable<EventVarGroup> editorFlag)
    {
        foreach (var g in editorFlag)
        {
            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, Name = $"TLP_F{g.Type}", AutoScroll = true };
            tlp.SuspendLayout();
            int i = 0;
            foreach (var f in g.Vars.OfType<EventFlag>())
            {
                var lbl = new Label
                {
                    Text = f.Name,
                    Name = flagLabelTag + f.RawIndex.ToString("0000"),
                    Margin = Padding.Empty,
                    AutoSize = true,
                };
                var chk = new CheckBox
                {
                    Name = flagTag + f.RawIndex.ToString("0000"),
                    CheckAlign = ContentAlignment.MiddleLeft,
                    Margin = Padding.Empty,
                    Checked = f.Flag,
                    AutoSize = true,
                };
                lbl.Click += (_, _) => chk.Checked ^= true;
                chk.CheckedChanged += (_, _) => f.Flag = chk.Checked;
                tlp.Controls.Add(chk, 0, i);
                tlp.Controls.Add(lbl, 1, i);
                i++;
            }
            var tab = new TabPage
            {
                Name = $"Tab_F{g.Type}",
                Text = g.Type.ToString(),
            };
            tab.Controls.Add(tlp);
            TC_Flag.Controls.Add(tab);
            tlp.ResumeLayout();
        }
    }

    private void LoadWork(IEnumerable<EventVarGroup> editorWork)
    {
        foreach (var g in editorWork)
        {
            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, Name = $"TLP_W{g.Type}", AutoScroll = true };
            tlp.SuspendLayout();
            int i = 0;
            foreach (var f in g.Vars.OfType<EventWork<int>>())
            {
                var lbl = new Label
                {
                    Text = f.Name,
                    Name = constLabelTag + f.RawIndex.ToString("0000"),
                    Margin = Padding.Empty,
                    AutoSize = true,
                };
                var nud = new NumericUpDown
                {
                    Maximum = int.MaxValue,
                    Minimum = int.MinValue,
                    Value = f.Value,
                    Name = constTag + f.RawIndex.ToString("0000"),
                    Margin = Padding.Empty,
                    Width = 50,
                };
                var cb = new ComboBox
                {
                    Margin = Padding.Empty,
                    Width = 150,
                    Name = constCBTag + f.RawIndex.ToString("0000"),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BindingContext = BindingContext,
                    DropDownWidth = Width + 100,
                };
                cb.InitializeBinding();
                cb.DataSource = new BindingSource(f.Options.Select(z => new ComboItem(z.Text, z.Value)).ToList(), null);
                cb.SelectedValue = f.Value;
                if (cb.SelectedIndex < 0)
                    cb.SelectedIndex = 0;

                cb.SelectedValueChanged += (_, _) =>
                {
                    if (editing)
                        return;
                    var value = WinFormsUtil.GetIndex(cb);
                    editing = true;
                    var match = f.Options.FirstOrDefault(z => z.Value == value);
                    nud.Enabled = match?.Custom == true;
                    if (!nud.Enabled)
                    {
                        nud.Value = (ushort)value;
                        f.Value = value;
                    }
                    editing = false;
                };
                nud.ValueChanged += (_, _) =>
                {
                    if (editing)
                        return;

                    var value = Util.ToInt32(nud.Text);
                    f.Value = value;
                    editing = true;
                    if (f.RawIndex == CB_Stats.SelectedIndex)
                        NUD_Stat.Text = value.ToString();
                    editing = false;
                };
                tlp.Controls.Add(lbl, 0, i);
                tlp.Controls.Add(cb, 1, i);
                tlp.Controls.Add(nud, 2, i);
                {
                    var match = f.Options.FirstOrDefault(z => z.Value == f.Value);
                    if (match != null)
                    {
                        cb.SelectedValue = match.Value;
                        nud.Enabled = false;
                    }
                }
                i++;
            }
            var tab = new TabPage
            {
                Name = $"Tab_W{g.Type}",
                Text = g.Type.ToString(),
            };
            tab.Controls.Add(tlp);
            TC_Work.Controls.Add(tab);
            tlp.ResumeLayout();
        }
    }

    private const string flagTag = "bool_";
    private const string constTag = "const_";
    private const string constCBTag = "cbconst_";
    private const string flagLabelTag = "flag_";
    private const string constLabelTag = "L_";
    private bool editing;

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Editor.Save();
        Origin.State.Edited = true;
        Close();
    }

    private void ChangeCustomFlag(object sender, EventArgs e)
    {
        c_CustomFlag.Checked = SAV.GetFlag((int)NUD_Flag.Value);
    }

    private void ChangeConstantIndex(object sender, EventArgs e)
    {
        NUD_Stat.Value = SAV.GetWork(CB_Stats.SelectedIndex);
    }

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

    private static string[] GetStringList(GameVersion game, [ConstantExpected] string type)
    {
        var gamePrefix = GetGameFilePrefix(game);
        return GameLanguage.GetStrings(gamePrefix, GameInfo.CurrentLanguage, type);
    }

    private static string GetGameFilePrefix(GameVersion game) => game switch
    {
        SL or VL or SV => "sv",
        BD or SP or BDSP => "bdsp",
        SW or SH or SWSH => "swsh",
        GP or GE or GG => "gg",
        X or Y => "xy",
        OR or AS => "oras",
        SN or MN => "sm",
        US or UM => "usum",
        DP => "dp",
        Pt => "pt",
        HGSS => "hgss",
        BW => "bw",
        B2W2 => "b2w2",
        E => "e",
        C => "c",
        R or S or RS => "rs",
        FR or LG or FRLG => "frlg",
        _ => throw new IndexOutOfRangeException(nameof(game)),
    };

    private void DiffSaves()
    {
        var diff7b = new EventWorkDiff7b(TB_OldSAV.Text, TB_NewSAV.Text);
        if (diff7b.Message != EventWorkDiffCompatibility.Valid)
        {
            WinFormsUtil.Alert(diff7b.Message.GetMessage());
            return;
        }

        RTB_Diff.Lines = [.. diff7b.Summarize()];
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
        SAV.SetFlag(index, c_CustomFlag.Checked);
        Origin.State.Edited = true;
    }

    private void B_ApplyWork_Click(object sender, EventArgs e)
    {
        var index = CB_Stats.SelectedIndex;
        SAV.SetWork(index, (int)NUD_Stat.Value);
        Origin.State.Edited = true;
    }
}
