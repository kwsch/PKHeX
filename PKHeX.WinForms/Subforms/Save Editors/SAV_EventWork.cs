using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public sealed partial class SAV_EventWork : Form
    {
        private readonly SaveFile Origin;
        private readonly IEventWork<int> SAV;
        private readonly SplitEventEditor<int> Editor;

        public SAV_EventWork(SaveFile sav)
        {
            InitializeComponent();

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            if (sav is SAV7b s7b)
                SAV = s7b.Blocks.EventWork;
            //else if (sav is SAV8SWSH s8ss)
            //    SAV = s8ss.Blocks.EventWork;
            Origin = sav;

            DragEnter += Main_DragEnter;
            DragDrop += Main_DragDrop;

            CB_Stats.Items.Clear();
            for (int i = 0; i < SAV.MaxWork; i++)
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
            NUD_Flag.Maximum = SAV.MaxFlag - 1;
            NUD_Flag.Text = "0";
            c_CustomFlag.Checked = SAV.GetFlag(0);

            Text = $"{Text} ({sav.Version})";
        }

        private void LoadFlags(IEnumerable<EventVarGroup> editorFlag)
        {
            foreach (var g in editorFlag)
            {
                var tlp = new TableLayoutPanel {Dock = DockStyle.Fill, Name = $"TLP_F{g.Type}", AutoScroll = true};
                tlp.SuspendLayout();
                int i = 0;
                foreach (var f in g.Vars.OfType<EventFlag>())
                {
                    var lbl = new Label
                    {
                        Text = f.Name,
                        Name = flagLabelTag + f.RawIndex.ToString("0000"),
                        Margin = Padding.Empty,
                        AutoSize = true
                    };
                    var chk = new CheckBox
                    {
                        Name = flagTag + f.RawIndex.ToString("0000"),
                        CheckAlign = ContentAlignment.MiddleLeft,
                        Margin = Padding.Empty,
                        Checked = f.Flag,
                        AutoSize = true
                    };
                    lbl.Click += (sender, e) => chk.Checked ^= true;
                    chk.CheckedChanged += (s, e) => f.Flag = chk.Checked;
                    tlp.Controls.Add(chk, 0, i);
                    tlp.Controls.Add(lbl, 1, i);
                    i++;
                }
                var tab = new TabPage
                {
                    Name = $"Tab_F{g.Type}",
                    Text = g.Type.ToString()
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
                var tlp = new TableLayoutPanel {Dock = DockStyle.Fill, Name = $"TLP_W{g.Type}", AutoScroll = true};
                tlp.SuspendLayout();
                int i = 0;
                foreach (var f in g.Vars.OfType<EventWork<int>>())
                {
                    var lbl = new Label
                    {
                        Text = f.Name,
                        Name = constLabelTag + f.RawIndex.ToString("0000"),
                        Margin = Padding.Empty,
                        AutoSize = true
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
                        DropDownWidth = Width + 100
                    };
                    cb.InitializeBinding();
                    cb.DataSource = new BindingSource(f.Options.Select(z => new ComboItem(z.Text, z.Value)).ToList(), null);
                    cb.SelectedValue = f.Value;
                    if (cb.SelectedIndex < 0)
                        cb.SelectedIndex = 0;

                    cb.SelectedValueChanged += (s, e) =>
                    {
                        if (editing)
                            return;
                        var val = (int) cb.SelectedValue;
                        editing = true;
                        var match = f.Options.FirstOrDefault(z => z.Value == val);
                        nud.Enabled = match?.Custom == true;
                        if (!nud.Enabled)
                        {
                            nud.Value = (ushort) val;
                            f.Value = val;
                        }
                        editing = false;
                    };
                    nud.ValueChanged += (s, e) =>
                    {
                        if (editing)
                            return;

                        var val = Util.ToInt32(nud.Text);
                        f.Value = val;
                        editing = true;
                        if (f.RawIndex == CB_Stats.SelectedIndex)
                            NUD_Stat.Text = val.ToString();
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
                    Text = g.Type.ToString()
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
            HandleSpecialFlags();
            Editor.Save();
            Origin.Edited = true;
            Close();
        }

        private void HandleSpecialFlags()
        {
        }

        private void ChangeCustomFlag(object sender, EventArgs e)
        {
            c_CustomFlag.Checked = SAV.GetFlag((int) NUD_Flag.Value);
        }

        private void ChangeConstantIndex(object sender, EventArgs e)
        {
            NUD_Stat.Value = SAV.GetWork(CB_Stats.SelectedIndex);
        }

        private void ChangeSAV()
        {
            if (TB_NewSAV.Text.Length > 0 && TB_OldSAV.Text.Length > 0)
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

        private static string[] GetStringList(GameVersion game, string type)
        {
            var gamePrefix = GetGameFilePrefix(game);
            return GameLanguage.GetStrings(gamePrefix, GameInfo.CurrentLanguage, type);
        }

        private static string GetGameFilePrefix(GameVersion game)
        {
            switch (game)
            {
                case GameVersion.SW: case GameVersion.SH: case GameVersion.SWSH: return "swsh";
                case GameVersion.GP: case GameVersion.GE: case GameVersion.GG: return "gg";
                case GameVersion.X: case GameVersion.Y: return "xy";
                case GameVersion.OR: case GameVersion.AS: return "oras";
                case GameVersion.SN: case GameVersion.MN: return "sm";
                case GameVersion.US: case GameVersion.UM: return "usum";
                case GameVersion.DP: return "dp";
                case GameVersion.Pt: return "pt";
                case GameVersion.HGSS: return "hgss";
                case GameVersion.BW: return "bw";
                case GameVersion.B2W2: return "b2w2";
                case GameVersion.E: return "e";
                case GameVersion.C: return "c";
                case GameVersion.R: case GameVersion.S: case GameVersion.RS: return "rs";
                case GameVersion.FR: case GameVersion.LG: case GameVersion.FRLG: return "frlg";
                default: throw new ArgumentException(nameof(game));
            }
        }

        private void DiffSaves()
        {
            var diff7b = new EventWorkDiff7b(TB_OldSAV.Text, TB_NewSAV.Text);
            if (diff7b.Message != null)
            {
                WinFormsUtil.Alert(diff7b.Message);
                return;
            }

            RTB_Diff.Lines = diff7b.Summarize().ToArray();
        }

        private static void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, Name, "Yes: Old Save" + Environment.NewLine + "No: New Save");
            var button = dr == DialogResult.Yes ? B_LoadOld : B_LoadNew;
            LoadSAV(button, files[0]);
        }

        private void B_ApplyFlag_Click(object sender, EventArgs e)
        {
            var index = (int)NUD_Flag.Value;
            SAV.SetFlag(index, c_CustomFlag.Checked);
            Origin.Edited = true;
        }

        private void B_ApplyWork_Click(object sender, EventArgs e)
        {
            var index = CB_Stats.SelectedIndex;
            SAV.SetWork(index, (int)NUD_Stat.Value);
            Origin.Edited = true;
        }
    }
}