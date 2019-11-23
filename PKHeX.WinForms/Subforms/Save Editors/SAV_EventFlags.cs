using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public sealed partial class SAV_EventFlags : Form
    {
        private readonly SaveFile Origin;
        private readonly SaveFile SAV;

        public SAV_EventFlags(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (Origin = sav).Clone();

            DragEnter += Main_DragEnter;
            DragDrop += Main_DragDrop;

            flags = SAV.EventFlags;
            Constants = SAV.EventConsts;

            CB_Stats.Items.Clear();
            for (int i = 0; i < Constants.Length; i++)
                CB_Stats.Items.Add(i.ToString());

            TLP_Flags.SuspendLayout();
            TLP_Const.SuspendLayout();
            TLP_Flags.Scroll += WinFormsUtil.PanelScroll;
            TLP_Const.Scroll += WinFormsUtil.PanelScroll;
            TLP_Flags.Controls.Clear();
            TLP_Const.Controls.Clear();
            AddFlagList(GetStringList("flags"));
            AddConstList(GetStringList("const"));

            TLP_Flags.ResumeLayout();
            TLP_Const.ResumeLayout();

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            Text = $"{Text} ({gamePrefix.ToUpper()})";

            if (CB_Stats.Items.Count > 0)
            {
                CB_Stats.SelectedIndex = 0;
            }
            else
            {
                L_Stats.Visible = CB_Stats.Visible = MT_Stat.Visible = false;
                tabControl1.TabPages.Remove(GB_Constants);
            }
            NUD_Flag.Maximum = flags.Length - 1;
            NUD_Flag.Text = "0";
            c_CustomFlag.Checked = flags[0];
        }

        private readonly bool[] flags;
        private readonly ushort[] Constants;
        private const string flagTag = "bool_";
        private const string constTag = "const_";
        private const string constCBTag = "cbconst_";
        private const string flagLabelTag = "flag_";
        private const string constLabelTag = "L_";
        private bool editing;
        private int constEntry = -1;
        private string gamePrefix = "unk";

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            // Gather Updated Flags
            foreach (CheckBox flag in TLP_Flags.Controls.OfType<CheckBox>())
                flags[GetControlNum(flag)] = flag.Checked;
            SAV.EventFlags = flags;

            // Copy back Constants
            ChangeConstantIndex(null, EventArgs.Empty); // Trigger Saving
            SAV.EventConsts = Constants;

            HandleSpecialFlags();
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void HandleSpecialFlags()
        {
            if (SAV is SAV7SM s7) // Ensure magearna event flag has magic constant
                s7.UpdateMagearnaConstant();
        }

        private string[] GetStringList(string type)
        {
            gamePrefix = GetResourceSuffix(SAV.Version);
            return GameLanguage.GetStrings(gamePrefix, GameInfo.CurrentLanguage, type);
        }

        private static string GetResourceSuffix(GameVersion ver)
        {
            switch (ver)
            {
                case GameVersion.X: case GameVersion.Y: case GameVersion.XY: return "xy";
                case GameVersion.OR: case GameVersion.AS: case GameVersion.ORAS: return "oras";
                case GameVersion.SN: case GameVersion.MN: case GameVersion.SM: return "sm";
                case GameVersion.US: case GameVersion.UM: case GameVersion.USUM: return "usum";
                case GameVersion.D: case GameVersion.P: case GameVersion.DP: return "dp";
                case GameVersion.Pt: case GameVersion.DPPt: return "pt";
                case GameVersion.HG: case GameVersion.SS: case GameVersion.HGSS: return "hgss";
                case GameVersion.B: case GameVersion.W: case GameVersion.BW: return "bw";
                case GameVersion.B2: case GameVersion.W2: case GameVersion.B2W2: return "b2w2";
                case GameVersion.R: case GameVersion.S: case GameVersion.RS: return "rs";
                case GameVersion.E: return "e";
                case GameVersion.FR: case GameVersion.LG: case GameVersion.FRLG: return "frlg";
                case GameVersion.C: return "c";
                case GameVersion.GD: case GameVersion.SV: case GameVersion.GS: return "gs";
                default:
                    throw new ArgumentException(nameof(GameVersion));
            }
        }

        private void AddFlagList(string[] list)
        {
            if (list == null || list.Length == 0)
            {
                TLP_Flags.Controls.Add(new Label { Text = MsgResearchRequired, Name = "TLP_Flags_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
                return;
            }

            // Get list
            List<int> num = new List<int>();
            List<string> desc = new List<string>();

            foreach (string[] split in list.Select(s => s.Split('\t')).Where(split => split.Length == 2))
            {
                try
                {
                    var flagIndex = split[0];
                    int n = TryParseHexDec(flagIndex);

                    if (num.Contains(n))
                        continue;
                    num.Add(n);
                    desc.Add(split[1]);
                }
                catch
                {
                    // Ignore bad user values
                    Debug.WriteLine(string.Concat(split));
                }
            }
            if (num.Count == 0)
            {
                TLP_Flags.Controls.Add(new Label { Text = MsgResearchRequired, Name = "TLP_Flags_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
                return;
            }

            for (int i = 0; i < num.Count; i++)
            {
                var lbl = new Label
                {
                    Text = desc[i],
                    Name = gamePrefix + flagLabelTag + num[i].ToString("0000"),
                    Margin = Padding.Empty,
                    AutoSize = true
                };
                var chk = new CheckBox
                {
                    Name = flagTag + num[i].ToString("0000"),
                    CheckAlign = ContentAlignment.MiddleLeft,
                    Margin = Padding.Empty,
                    Checked = flags[num[i]],
                    AutoSize = true
                };
                chk.CheckStateChanged += ToggleFlag;
                lbl.Click += (sender, e) => chk.Checked ^= true;
                TLP_Flags.Controls.Add(chk, 0, i);
                TLP_Flags.Controls.Add(lbl, 1, i);
            }
        }

        private static int TryParseHexDec(string flag)
        {
            if (!flag.StartsWith("0x"))
                return Convert.ToInt16(flag);
            flag = flag.Substring(2);
            return Convert.ToInt16(flag, 16);
        }

        private void AddConstList(string[] list)
        {
            if (list == null || list.Length == 0)
            {
                TLP_Const.Controls.Add(new Label { Text = MsgResearchRequired, Name = "TLP_Const_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
                return;
            }

            // Get list
            List<int> num = new List<int>();
            List<string> desc = new List<string>();
            List<string> enums = new List<string>();

            foreach (string[] split in list.Select(s => s.Split('\t')).Where(split => split.Length == 2 || split.Length == 3))
            {
                try
                {
                    var c = split[0];
                    int n = TryParseHexDecConst(c);

                    if (num.Contains(n))
                        continue;
                    num.Add(n);
                    desc.Add(split[1]);
                    enums.Add(split.Length == 3 ? split[2] : string.Empty);
                }
                catch
                {
                    // Ignore bad user values
                    Debug.WriteLine(string.Concat(split));
                }
            }
            if (num.Count == 0)
            {
                TLP_Const.Controls.Add(new Label { Text = MsgResearchRequired, Name = "TLP_Const_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
                return;
            }

            for (int i = 0; i < num.Count; i++)
            {
                var lbl = new Label
                {
                    Text = desc[i],
                    Name = gamePrefix + constLabelTag + num[i].ToString("0000"),
                    Margin = Padding.Empty,
                    AutoSize = true
                };
                var mtb = new NumericUpDown
                {
                    Maximum = ushort.MaxValue,
                    Minimum = ushort.MinValue,
                    Value = Constants[num[i]],
                    Name = constTag + num[i].ToString("0000"),
                    Margin = Padding.Empty,
                    Width = 50,
                };

                var map = new[] { new { Text = "Custom", Value = -1 } }.ToList();

                if (!string.IsNullOrWhiteSpace(enums[i]))
                {
                    foreach (var entry in enums[i].Split(','))
                    {
                        var spl = entry.Split(':');
                        map.Add(new { Text = spl[1], Value = Convert.ToInt32(spl[0])});
                    }
                }
                var cb = new ComboBox
                {
                    Margin = Padding.Empty,
                    Width = 150,
                    Name = constCBTag + num[i].ToString("0000"),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BindingContext = BindingContext,
                    DropDownWidth = Width + 100
                };
                cb.InitializeBinding();
                cb.DataSource = map;
                cb.SelectedIndex = 0;
                cb.SelectedValueChanged += ToggleConst;
                mtb.TextChanged += ToggleConst;
                TLP_Const.Controls.Add(lbl, 0, i);
                TLP_Const.Controls.Add(cb, 1, i);
                TLP_Const.Controls.Add(mtb, 2, i);
                if (map.Any(val => val.Value == Constants[num[i]]))
                {
                    cb.SelectedValue = (int)Constants[num[i]];
                }
            }
        }

        private static int TryParseHexDecConst(string c)
        {
            if (!c.StartsWith("0x40"))
                return Convert.ToInt16(c);
            c = c.Substring(4);
            return Convert.ToInt16(c, 16);
        }

        private static int GetControlNum(Control c)
        {
            string source = c.Name.Split('_')[1];
            return int.TryParse(source, out var val) ? val : 0;
        }

        private void ChangeCustomBool(object sender, EventArgs e)
        {
            if (editing)
                return;
            editing = true;
            flags[(int)NUD_Flag.Value] = c_CustomFlag.Checked;
            var name = flagTag + NUD_Flag.Value.ToString("0000");
            if (TLP_Flags.Controls[name] is CheckBox c)
                c.Checked = c_CustomFlag.Checked;
            editing = false;
        }

        private void ChangeCustomFlag(object sender, EventArgs e)
        {
            int flag = (int)NUD_Flag.Value;
            if (flag >= flags.Length)
            {
                c_CustomFlag.Checked = false;
                c_CustomFlag.Enabled = false;
                NUD_Flag.BackColor = Color.Red;
            }
            else
            {
                c_CustomFlag.Enabled = true;
                NUD_Flag.ResetBackColor();
                c_CustomFlag.Checked = flags[flag];
            }
        }

        private void ChangeCustomFlag(object sender, KeyEventArgs e)
        {
            ChangeCustomFlag(null, (EventArgs)e);
        }

        private void ToggleFlag(object sender, EventArgs e)
        {
            if (editing)
                return;
            editing = true;
            int flagnum = GetControlNum((CheckBox) sender);
            flags[flagnum] = ((CheckBox)sender).Checked;
            if (NUD_Flag.Value == flagnum)
                c_CustomFlag.Checked = flags[flagnum];
            editing = false;
        }

        private void ChangeCustomConst(object sender, EventArgs e)
        {
            if (editing)
                return;
            editing = true;

            Constants[CB_Stats.SelectedIndex] = (ushort)(Util.ToUInt32(((MaskedTextBox)sender).Text) & 0xFFFF);
            var name = constTag + CB_Stats.SelectedIndex.ToString("0000");
            if (TLP_Flags.Controls[name] is MaskedTextBox m)
                m.Text = MT_Stat.Text;

            editing = false;
        }

        private void ChangeConstantIndex(object sender, EventArgs e)
        {
            if (Constants.Length == 0)
                return;
            if (constEntry > -1) // Set Entry
                Constants[constEntry] = (ushort)Math.Min(Util.ToUInt32(MT_Stat.Text), 0xFFFF);

            constEntry = CB_Stats.SelectedIndex; // Get Entry
            MT_Stat.Text = Constants[constEntry].ToString();
        }

        private void ToggleConst(object sender, EventArgs e)
        {
            if (editing)
                return;

            int constnum = GetControlNum((Control)sender);
            if (sender is ComboBox cb)
            {
                var nud = (NumericUpDown)TLP_Const.GetControlFromPosition(2, TLP_Const.GetRow(cb));
                var sel_val = (int)cb.SelectedValue;
                editing = true;
                nud.Enabled = sel_val == -1;
                if (sel_val != -1)
                    nud.Value = (ushort)sel_val;
                Constants[constnum] = (ushort)Util.ToUInt32(nud.Text);
                editing = false;
            }
            else if (sender is NumericUpDown nud)
            {
                editing = true;
                Constants[constnum] = (ushort)Util.ToUInt32(nud.Text);
                if (constnum == CB_Stats.SelectedIndex)
                    MT_Stat.Text = Constants[constnum].ToString();
                editing = false;
            }
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
            if (sender == B_LoadOld)
                TB_OldSAV.Text = path;
            else
                TB_NewSAV.Text = path;
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