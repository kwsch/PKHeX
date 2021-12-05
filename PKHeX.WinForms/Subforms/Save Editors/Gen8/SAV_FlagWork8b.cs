using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.GameVersion;

namespace PKHeX.WinForms
{
    public sealed partial class SAV_FlagWork8b : Form
    {
        private readonly SAV8BS Origin;
        private readonly SAV8BS SAV;

        private bool editing;
        private readonly Dictionary<int, NumericUpDown> WorkDict = new();
        private readonly Dictionary<int, CheckBox> FlagDict = new();
        private readonly Dictionary<int, CheckBox> SystemDict = new();

        public SAV_FlagWork8b(SAV8BS sav)
        {
            InitializeComponent();

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            SAV = (SAV8BS)sav.Clone();
            Origin = sav;

            DragEnter += Main_DragEnter;
            DragDrop += Main_DragDrop;

            CB_CustomWork.Items.Clear();
            for (int i = 0; i < SAV.Work.CountWork; i++)
                CB_CustomWork.Items.Add(i.ToString());

            SuspendLayout();
            editing = true;
            var obj = SAV.Work;
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

            Text = $"{Text} ({sav.Version})";
        }

        private void LoadFlags(EventLabelCollectionSystem editor)
        {
            TLP_Flags.SuspendLayout();
            TLP_Flags.Scroll += WinFormsUtil.PanelScroll;
            TLP_Flags.Controls.Clear();
            var labels = editor.Flag;

            var hide = Main.Settings.Advanced.HideEventTypeBelow;
            labels = labels.OrderByDescending(z => z.Type).ToList();
            for (int i = 0; i < labels.Count; i++)
            {
                var (name, index, type) = labels[i];
                if (type < hide)
                    break;

                var lbl = new Label { Text = name, Margin = Padding.Empty, AutoSize = true };
                var chk = new CheckBox
                {
                    CheckAlign = ContentAlignment.MiddleLeft,
                    Margin = Padding.Empty,
                    Checked = SAV.Work.GetFlag(index),
                    AutoSize = true,
                };
                lbl.Click += (_, __) => chk.Checked ^= true;
                chk.CheckedChanged += (_, __) =>
                {
                    SAV.Work.SetFlag(index, chk.Checked);
                    if (NUD_Flag.Value == index)
                        CHK_CustomFlag.Checked = chk.Checked;
                };
                TLP_Flags.Controls.Add(chk, 0, i);
                TLP_Flags.Controls.Add(lbl, 1, i);

                FlagDict.Add(index, chk);
            }

            TLP_Flags.ResumeLayout();
        }

        private void LoadSystem(EventLabelCollectionSystem editor)
        {
            TLP_System.SuspendLayout();
            TLP_System.Scroll += WinFormsUtil.PanelScroll;
            TLP_System.Controls.Clear();
            var labels = editor.System;

            var hide = Main.Settings.Advanced.HideEventTypeBelow;
            labels = labels.OrderByDescending(z => z.Type).ToList();
            for (int i = 0; i < labels.Count; i++)
            {
                var (name, index, type) = labels[i];
                if (type < hide)
                    break;

                var lbl = new Label { Text = name, Margin = Padding.Empty, AutoSize = true };
                var chk = new CheckBox
                {
                    CheckAlign = ContentAlignment.MiddleLeft,
                    Margin = Padding.Empty,
                    Checked = SAV.Work.GetSystemFlag(index),
                    AutoSize = true,
                };
                lbl.Click += (_, __) => chk.Checked ^= true;
                chk.CheckedChanged += (_, __) =>
                {
                    SAV.Work.SetSystemFlag(index, chk.Checked);
                    if (NUD_System.Value == index)
                        CHK_CustomSystem.Checked = chk.Checked;
                };
                TLP_System.Controls.Add(chk, 0, i);
                TLP_System.Controls.Add(lbl, 1, i);

                SystemDict.Add(index, chk);
            }

            TLP_System.ResumeLayout();
        }

        private void LoadWork(EventLabelCollectionSystem editor)
        {
            TLP_Work.SuspendLayout();
            TLP_Work.Scroll += WinFormsUtil.PanelScroll;
            TLP_Work.Controls.Clear();
            var labels = editor.Work;
            var hide = Main.Settings.Advanced.HideEventTypeBelow;
            labels = labels.OrderByDescending(z => z.Type).ToList();
            for (var i = 0; i < labels.Count; i++)
            {
                var entry = labels[i];
                if (entry.Type < hide)
                    break;
                var lbl = new Label { Text = entry.Name, Margin = Padding.Empty, AutoSize = true };
                var mtb = new NumericUpDown
                {
                    Maximum = int.MaxValue,
                    Minimum = int.MinValue,
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
                cb.InitializeBinding();
                cb.DataSource = map;

                lbl.Click += (_, __) => mtb.Value = 0;
                bool updating = false;
                mtb.ValueChanged += ChangeConstValue;
                void ChangeConstValue(object? sender, EventArgs e)
                {
                    if (updating)
                        return;

                    updating = true;
                    var value = (ushort)mtb.Value;
                    var (_, valueID) = map.Find(z => z.Value == value) ?? map[0];
                    if (WinFormsUtil.GetIndex(cb) != valueID)
                        cb.SelectedValue = valueID;

                    SAV.Work.SetWork(entry.Index, value);
                    if (CB_CustomWork.SelectedIndex == entry.Index)
                        mtb.Text = ((int)mtb.Value).ToString();
                    updating = false;
                }
                cb.SelectedValueChanged += (o, args) =>
                {
                    if (editing || updating)
                        return;
                    var value = WinFormsUtil.GetIndex(cb);
                    mtb.Value = value == NamedEventConst.CustomMagicValue ? 0 : value;
                };

                mtb.Value = SAV.Work.GetWork(entry.Index);
                if (mtb.Value == 0)
                    ChangeConstValue(this, EventArgs.Empty);

                TLP_Work.Controls.Add(lbl, 0, i);
                TLP_Work.Controls.Add(cb, 1, i);
                TLP_Work.Controls.Add(mtb, 2, i);

                WorkDict.Add(entry.Index, mtb);
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

        private void ChangeCustomFlag(object sender, EventArgs e) => CHK_CustomFlag.Checked = SAV.Work.GetFlag((int)NUD_Flag.Value);
        private void ChangeCustomSystem(object sender, EventArgs e) => CHK_CustomSystem.Checked = SAV.Work.GetSystemFlag((int)NUD_System.Value);
        private void ChangeConstantIndex(object sender, EventArgs e) => NUD_Work.Value = SAV.Work.GetWork(CB_CustomWork.SelectedIndex);

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

        private static string GetGameFilePrefix(GameVersion game) => game switch
        {
            BD or SP or BDSP => "bdsp",
            _ => throw new IndexOutOfRangeException(nameof(game)),
        };

        private void DiffSaves()
        {
            var diff = new EventWorkDiff8b(TB_OldSAV.Text, TB_NewSAV.Text);
            if (diff.Message.Length != 0)
            {
                WinFormsUtil.Alert(diff.Message);
                return;
            }

            RTB_Diff.Lines = diff.Summarize().ToArray();
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
            SAV.Work.SetFlag(index, CHK_CustomFlag.Checked);
            Origin.State.Edited = true;

            editing = true;
            if (FlagDict.TryGetValue(index, out var chk))
                chk.Checked = CHK_CustomFlag.Checked;
            editing = false;
        }

        private void B_ApplySystemFlag_Click(object sender, EventArgs e)
        {
            var index = (int)NUD_System.Value;
            SAV.Work.SetSystemFlag(index, CHK_CustomSystem.Checked);
            Origin.State.Edited = true;

            editing = true;
            if (SystemDict.TryGetValue(index, out var chk))
                chk.Checked = CHK_CustomSystem.Checked;
            editing = false;
        }

        private void B_ApplyWork_Click(object sender, EventArgs e)
        {
            var index = CB_CustomWork.SelectedIndex;
            SAV.Work.SetWork(index, (int)NUD_Work.Value);
            Origin.State.Edited = true;

            editing = true;
            if (WorkDict.TryGetValue(index, out var nud))
                nud.Value = NUD_Work.Value;
            editing = false;
        }
    }
}
