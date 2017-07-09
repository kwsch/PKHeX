using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_EventFlags : Form
    {
        private readonly SaveFile Origin;
        private readonly SaveFile SAV;
        public SAV_EventFlags(SaveFile sav)
        {
            SAV = (Origin = sav).Clone();
            InitializeComponent();

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

            Text = $"Event Flag Editor ({gamePrefix.ToUpper()})";


            CB_Stats.SelectedIndex = 0;
            NUD_Flag.Maximum = flags.Length - 1;
            NUD_Flag.Text = "0";
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

        private const ulong MagearnaConst = 0xCBE05F18356504AC;

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

            HandleSpecialFlags();

            // Copy back Constants
            ChangeConstantIndex(null, null); // Trigger Saving
            SAV.EventConsts = Constants;
            SAV.Data.CopyTo(Origin.Data, 0);
            Origin.Edited = true;
            Close();
        }

        private void HandleSpecialFlags()
        {
            if (SAV.SM) // Ensure magearna event flag has magic constant
            {
                BitConverter.GetBytes(flags[3100] ? MagearnaConst : 0).CopyTo(SAV.Data, ((SAV7)SAV).QRSaveData + 0x168);
            }

        }

        private string[] GetStringList(string type)
        {
            switch (SAV.Version)
            {
                case GameVersion.X:
                case GameVersion.Y:
                    gamePrefix = "xy";
                    break;
                case GameVersion.OR:
                case GameVersion.AS:
                    gamePrefix = "oras";
                    break;
                case GameVersion.SN:
                case GameVersion.MN:
                    gamePrefix = "sm";
                    break;
                case GameVersion.DP:
                    gamePrefix = "dp";
                    break;
                case GameVersion.Pt:
                    gamePrefix = "pt";
                    break;
                case GameVersion.HGSS:
                    gamePrefix = "hgss";
                    break;
                case GameVersion.BW:
                    gamePrefix = "bw";
                    break;
                case GameVersion.B2W2:
                    gamePrefix = "b2w2";
                    break;
                default:
                    return null;
            }
            return Util.GetStringList($"{type}_{gamePrefix}");
        }
        private void AddFlagList(string[] list)
        {
            if (list == null || list.Length == 0)
            {
                TLP_Flags.Controls.Add(new Label { Text = "Needs more research.", Name = "TLP_Flags_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
                return;
            }

            // Get list
            List<int> num = new List<int>();
            List<string> desc = new List<string>();

            foreach (string[] split in list.Select(s => s.Split('\t')).Where(split => split.Length == 2))
            {
                try
                {
                    int n = Convert.ToInt32(split[0]);
                    if (num.Contains(n))
                        continue;
                    num.Add(n);
                    desc.Add(split[1]);
                } catch { }
            }
            if (num.Count == 0)
            {
                TLP_Flags.Controls.Add(new Label { Text = "Needs more research.", Name = "TLP_Flags_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
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
        private void AddConstList(string[] list)
        {
            if (list == null || list.Length == 0)
            {
                TLP_Const.Controls.Add(new Label { Text = "Needs more research.", Name = "TLP_Const_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
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
                    int n = Convert.ToInt32(split[0]);
                    if (num.Contains(n))
                        continue;
                    num.Add(n);
                    desc.Add(split[1]);
                    enums.Add(split.Length == 3 ? split[2] : "");
                } catch { }
            }
            if (num.Count == 0)
            {
                TLP_Const.Controls.Add(new Label { Text = "Needs more research.", Name = "TLP_Const_Research", ForeColor = Color.Red, AutoSize = true }, 0, 0);
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
                    ValueMember = "Value",
                    DisplayMember = "Text",
                    Margin = Padding.Empty,
                    Width = 150,
                    Name = constCBTag + num[i].ToString("0000"),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    BindingContext = BindingContext,
                    DataSource = map,
                    SelectedIndex = 0,
                    DropDownWidth = Width + 100
                };
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

        private static int GetControlNum(Control c)
        {
            try
            {
                string source = c.Name.Split('_')[1];
                return Convert.ToInt32(source);
            }
            catch { return 0; }
        }
        private void ChangeCustomBool(object sender, EventArgs e)
        {
            if (editing)
                return;
            editing = true;
            flags[(int)NUD_Flag.Value] = c_CustomFlag.Checked;
            CheckBox c = TLP_Flags.Controls[flagTag + NUD_Flag.Value.ToString("0000")] as CheckBox;
            if (c != null)
            {
                c.Checked = c_CustomFlag.Checked;
            }
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
            MaskedTextBox m = TLP_Flags.Controls[constTag + CB_Stats.SelectedIndex.ToString("0000")] as MaskedTextBox;
            if (m != null)
                m.Text = MT_Stat.Text;

            editing = false;
        }
        private void ChangeConstantIndex(object sender, EventArgs e)
        {
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
            if (sender is ComboBox)
            {
                var nud = (NumericUpDown)TLP_Const.GetControlFromPosition(2, TLP_Const.GetRow((Control)sender));
                var sel_val = (int)((ComboBox)sender).SelectedValue;
                editing = true;
                nud.Enabled = sel_val == -1;
                if (sel_val != -1)
                    nud.Value = (ushort)sel_val;
                Constants[constnum] = (ushort)(Util.ToUInt32(nud.Text) & 0xFFFF);
                editing = false;
            }
            else if (sender is NumericUpDown)
            {
                editing = true;
                Constants[constnum] = (ushort)(Util.ToUInt32(((NumericUpDown)sender).Text) & 0xFFFF);
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
            OpenFileDialog ofd = new OpenFileDialog();
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
            if (!File.Exists(TB_OldSAV.Text)) { WinFormsUtil.Alert("Save 1 path invalid."); return; }
            if (!File.Exists(TB_NewSAV.Text)) { WinFormsUtil.Alert("Save 2 path invalid."); return; }
            if (new FileInfo(TB_OldSAV.Text).Length > 0x100000) { WinFormsUtil.Alert("Save 1 file invalid."); return; }
            if (new FileInfo(TB_NewSAV.Text).Length > 0x100000) { WinFormsUtil.Alert("Save 2 file invalid."); return; }

            SaveFile s1 = SaveUtil.GetVariantSAV(File.ReadAllBytes(TB_OldSAV.Text));
            SaveFile s2 = SaveUtil.GetVariantSAV(File.ReadAllBytes(TB_NewSAV.Text));

            if (s1.GetType() != s2.GetType()) { WinFormsUtil.Alert("Save types are different.", $"S1: {s1.GetType().Name}", $"S2: {s2.GetType().Name}"); return; }
            if (s1.Version != s2.Version) { WinFormsUtil.Alert("Save versions are different.", $"S1: {s1.Version}", $"S2: {s2.Version}"); return; }

            string tbIsSet = "";
            string tbUnSet = "";
            try
            {
                bool[] oldBits = s1.EventFlags;
                bool[] newBits = s2.EventFlags;
                if (oldBits.Length != newBits.Length)
                { WinFormsUtil.Alert("Event flag lengths for games are different.", $"S1: {(GameVersion)s1.Game}", $"S2: {(GameVersion)s2.Game}"); return; }

                for (int i = 0; i < oldBits.Length; i++)
                {
                    if (oldBits[i] == newBits[i]) continue;
                    if (newBits[i])
                        tbIsSet += i.ToString("0000") + ",";
                    else
                        tbUnSet += i.ToString("0000") + ",";
                }
            }
            catch (Exception e)
            {
                WinFormsUtil.Error("An unexpected error has occurred.", e);
                Debug.WriteLine(e);
            }
            TB_IsSet.Text = tbIsSet;
            TB_UnSet.Text = tbUnSet;

            string r = "";
            try
            {
                ushort[] oldConst = s1.EventConsts;
                ushort[] newConst = s2.EventConsts;
                if (oldConst.Length != newConst.Length)
                { WinFormsUtil.Alert("Event flag lengths for games are different.", $"S1: {(GameVersion)s1.Game}", $"S2: {(GameVersion)s2.Game}"); return; }

                for (int i = 0; i < newConst.Length; i++)
                    if (oldConst[i] != newConst[i])
                        r += $"{i}: {oldConst[i]}->{newConst[i]}{Environment.NewLine}";
            }
            catch (Exception e)
            {
                WinFormsUtil.Error("An unexpected error has occurred.", e);
                Debug.WriteLine(e);
            }

            if (string.IsNullOrEmpty(r))
            {
                WinFormsUtil.Alert("No Event Constant diff found.");
                return;
            }

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Copy Event Constant diff to clipboard?"))
                return;
            Clipboard.SetText(r);
        }

        private static void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadSAV(WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "FlagDiff Researcher:", "Yes: Old Save" + Environment.NewLine + "No: New Save") == DialogResult.Yes ? B_LoadOld : B_LoadNew, files[0]);
        }
    }
}