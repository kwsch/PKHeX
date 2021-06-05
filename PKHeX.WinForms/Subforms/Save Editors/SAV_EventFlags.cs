using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public sealed partial class SAV_EventFlags : Form
    {
        private readonly EventWorkspace Editor;
        private readonly Dictionary<int, NumericUpDown> WorkDict = new();
        private readonly Dictionary<int, CheckBox> FlagDict = new();

        private bool editing;

        public SAV_EventFlags(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            var editor = Editor = new EventWorkspace(sav);
            DragEnter += Main_DragEnter;
            DragDrop += Main_DragDrop;

            editing = true;
            CB_Stats.Items.Clear();
            for (int i = 0; i < editor.Values.Length; i++)
                CB_Stats.Items.Add(i.ToString());

            TLP_Flags.SuspendLayout();
            TLP_Const.SuspendLayout();
            TLP_Flags.Scroll += WinFormsUtil.PanelScroll;
            TLP_Const.Scroll += WinFormsUtil.PanelScroll;
            TLP_Flags.Controls.Clear();
            TLP_Const.Controls.Clear();
            AddFlagList(editor.Labels, editor.Flags);
            AddConstList(editor.Labels, editor.Values);

            TLP_Flags.ResumeLayout();
            TLP_Const.ResumeLayout();

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
            var labels = list.Flag;
            if (labels.Count == 0)
            {
                TLP_Flags.Controls.Add(new Label { Text = MsgResearchRequired, Name = "TLP_Flags_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
                return;
            }

            for (int i = 0; i < labels.Count; i++)
            {
                var (name, index) = labels[i];
                var lbl = new Label { Text = name, Margin = Padding.Empty, AutoSize = true };
                var chk = new CheckBox
                {
                    CheckAlign = ContentAlignment.MiddleLeft,
                    Margin = Padding.Empty,
                    Checked = values[index],
                    AutoSize = true
                };
                lbl.Click += (sender, e) => chk.Checked ^= true;
                chk.CheckedChanged += (o, args) =>
                {
                    values[index] = chk.Checked;
                    if (NUD_Flag.Value == index)
                        c_CustomFlag.Checked = chk.Checked;
                };
                TLP_Flags.Controls.Add(chk, 0, i);
                TLP_Flags.Controls.Add(lbl, 1, i);

                FlagDict.Add(index, chk);
            }
        }

        private void AddConstList(EventLabelCollection list, ushort[] values)
        {
            var labels = list.Work;
            if (labels.Count == 0)
            {
                TLP_Const.Controls.Add(new Label { Text = MsgResearchRequired, Name = "TLP_Const_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
                return;
            }

            for (var i = 0; i < labels.Count; i++)
            {
                var entry = labels[i];
                var lbl = new Label { Text = entry.Name, Margin = Padding.Empty, AutoSize = true };
                var mtb = new NumericUpDown
                {
                    Maximum = ushort.MaxValue,
                    Minimum = ushort.MinValue,
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
                    DropDownWidth = Width + 100
                };
                cb.InitializeBinding();
                cb.DataSource = map;

                lbl.Click += (sender, e) => mtb.Value = 0;
                bool updating = false;
                mtb.ValueChanged += (o, args) =>
                {
                    if (updating)
                        return;

                    updating = true;
                    var value = (ushort) mtb.Value;
                    var (_, valueID) = map.Find(z => z.Value == value) ?? map[0];
                    if (WinFormsUtil.GetIndex(cb) != valueID)
                        cb.SelectedValue = valueID;

                    Editor.Values[entry.Index] = value;
                    if (CB_Stats.SelectedIndex == entry.Index)
                        MT_Stat.Text = ((int)mtb.Value).ToString();
                    updating = false;
                };
                cb.SelectedValueChanged += (o, args) =>
                {
                    if (editing || updating)
                        return;
                    var value = WinFormsUtil.GetIndex(cb);
                    mtb.Value = value is NamedEventConst.CustomMagicValue ? 0 : value;
                };

                mtb.Value = values[entry.Index];

                TLP_Const.Controls.Add(lbl, 0, i);
                TLP_Const.Controls.Add(cb, 1, i);
                TLP_Const.Controls.Add(mtb, 2, i);

                WorkDict.Add(entry.Index, mtb);
            }
        }

        private void ChangeCustomBool(object sender, EventArgs e)
        {
            if (editing)
                return;
            editing = true;
            var index = (int) NUD_Flag.Value;
            Editor.Flags[index] = c_CustomFlag.Checked;
            if (FlagDict.TryGetValue(index, out var chk))
                chk.Checked = c_CustomFlag.Checked;
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
            var parse = ushort.TryParse(MT_Stat.Text, out var value) ? value : (ushort)0;
            Editor.Values[index] = parse;
            if (WorkDict.TryGetValue(index, out var mtb))
                mtb.Value = parse;
            editing = false;
        }

        private void ChangeSAV(object sender, EventArgs e)
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
            var dest = sender == B_LoadOld ? TB_OldSAV : TB_NewSAV;
            dest.Text = path;
        }

        private void DiffSaves()
        {
            var diff = new EventBlockDiff(TB_OldSAV.Text, TB_NewSAV.Text);
            if (!string.IsNullOrWhiteSpace(diff.Message))
            {
                WinFormsUtil.Alert(diff.Message);
                return;
            }

            TB_IsSet.Text = string.Join(", ", diff.SetFlags.Select(z => $"{z:0000}"));
            TB_UnSet.Text = string.Join(", ", diff.ClearedFlags.Select(z => $"{z:0000}"));

            if (diff.WorkDiff.Count == 0)
            {
                WinFormsUtil.Alert("No Event Constant diff found.");
                return;
            }

            if (DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Copy Event Constant diff to clipboard?"))
                WinFormsUtil.SetClipboardText(string.Join(Environment.NewLine, diff.WorkDiff));
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
    }
}
