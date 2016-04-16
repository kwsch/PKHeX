using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_EventFlagsORAS : Form
    {
        public SAV_EventFlagsORAS()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);

            AllowDrop = true;
            DragEnter += tabMain_DragEnter;
            DragDrop += tabMain_DragDrop;
            Setup();

            nud.Text = "0"; // Prompts an update for flag 0.
            MT_Ash.Text = BitConverter.ToUInt16(Main.SAV.Data, Main.SAV.EventAsh).ToString();
        }
        private bool setup = true;
        private CheckBox[] chka;
        private readonly bool[] flags = new bool[3072];
        private readonly ushort[] Constants = new ushort[(Main.SAV.EventFlag - Main.SAV.EventConst) / 2];
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            // Gather Updated Flags
            foreach (CheckBox flag in chka)
                flags[getFlagNum(flag)] = flag.Checked;

            byte[] data = new byte[flags.Length / 8];

            for (int i = 0; i < flags.Length; i++)
                if (flags[i])
                    data[i / 8] |= (byte)(1 << i % 8);

            Array.Copy(data, 0, Main.SAV.Data, Main.SAV.EventFlag, 0x180);

            // Copy back Constants
            changeConstantIndex(null, null); // Trigger Saving
            for (int i = 0; i < Constants.Length; i++)
                Array.Copy(BitConverter.GetBytes(Constants[i]), 0, Main.SAV.Data, Main.SAV.EventConst + 2 * i, 2);

            // Copy back Volcanic Ash counter
            Array.Copy(BitConverter.GetBytes(Util.ToUInt32(MT_Ash.Text)), 0, Main.SAV.Data, Main.SAV.EventAsh, 2);

            Close();
        }
        private void Setup()
        {
            // Fill Bit arrays

            chka = new[] {
                
                flag_0173,flag_2811, // Raikou
                flag_0174,flag_2812, // Entei
                flag_0175,flag_2813, // Suicune
                flag_0209,flag_2814, // Lugia
                flag_0208,flag_2815, // Ho-Oh
                flag_0179,flag_2816, // Uxie
                flag_0180,flag_2817, // Mesprit
                flag_0181,flag_2818, // Azelf
                          flag_2819, // Dialga
                          flag_2820, // Palkia
                flag_0260,flag_2821, // Heatran
                flag_0252,flag_2822, // Regigigas
                          flag_2823, // Giratina
                flag_0172,flag_2824, // Cresselia
                flag_0176,flag_2825, // Cobalion
                flag_0177,flag_2826, // Terrakion
                flag_0178,flag_2827, // Virizion
                          flag_2828, // Tornadus
                          flag_2829, // Thundurus
                flag_0182,flag_2830, // Reshiram
                flag_0183,flag_2831, // Zekrom
                          flag_2832, // Landorus
                flag_0184,flag_2833, // Kyurem
                flag_0419,flag_2834, // Latios
                flag_0420,flag_2835, // Latias
                flag_0956,flag_2836, // Regirock
                flag_0957,flag_2837, // Regice
                flag_0958,flag_2838, // Registeel
                flag_0648,flag_2839, // Groudon
                flag_0647,flag_2840, // Kyogre
                                     // ??????
                flag_0945,flag_2842, // Deoxys

                // Cresselia, Regigigas
                // Terrakion, Virizion

                // Maison
                flag_0284,flag_0285,flag_0286,flag_0287,flag_0288, // Statuettes
                flag_0289,flag_0290,flag_0291,flag_0292,flag_0293, // Super Unlocks
                //flag_0675, // Chatelaine 50
                //flag_2546, // Pokedex
            };
            byte[] data = new byte[0x180];
            Array.Copy(Main.SAV.Data, Main.SAV.EventFlag, data, 0, 0x180);
            BitArray BitRegion = new BitArray(data);
            BitRegion.CopyTo(flags, 0);

            // Setup Event Constant Editor
            CB_Stats.Items.Clear();
            for (int i = 0; i < Constants.Length; i += 2)
            {
                CB_Stats.Items.Add($"0x{(i/2).ToString("X2")}");
                Constants[i / 2] = BitConverter.ToUInt16(Main.SAV.Data, Main.SAV.EventConst + i);
            }
            CB_Stats.SelectedIndex = 0;

            // Populate Flags
            setup = true;
            popFlags();
        }
        private void popFlags()
        {
            if (!setup) return;
            foreach (CheckBox flag in chka)
                flag.Checked = flags[getFlagNum(flag)];

            changeCustomFlag(null, null);
        }

        private int getFlagNum(CheckBox chk)
        {
            try
            {
                string source = chk.Name;
                return Convert.ToInt32(source.Substring(Math.Max(0, source.Length - 4)));
            }
            catch { return 0; }
        }
        private void changeCustomBool(object sender, EventArgs e)
        {
            flags[(int)nud.Value] = CHK_CustomFlag.Checked;
            popFlags();
        }
        private void changeCustomFlag(object sender, EventArgs e)
        {
            int flag = (int)nud.Value;
            if (flag >= 3072)
            {
                CHK_CustomFlag.Checked = false;
                CHK_CustomFlag.Enabled = false;
                nud.BackColor = Color.Red;
            }
            else
            {
                CHK_CustomFlag.Enabled = true;
                nud.BackColor = Main.defaultControlWhite;
                CHK_CustomFlag.Checked = flags[flag];
            }
        }
        private void changeCustomFlag(object sender, KeyEventArgs e)
        {
            changeCustomFlag(null, (EventArgs)e);
        }

        private void toggleFlag(object sender, EventArgs e)
        {
            flags[getFlagNum((CheckBox)sender)] = ((CheckBox)sender).Checked;
            changeCustomFlag(sender, e);
        }

        private void changeSAV(object sender, EventArgs e)
        {
            if (TB_NewSAV.Text.Length > 0 && TB_OldSAV.Text.Length > 0)
                diffSaves();
        }
        private void diffSaves()
        {
            BitArray oldBits = new BitArray(oldFlags);
            BitArray newBits = new BitArray(newFlags);

            string tbIsSet = "";
            string tbUnSet = "";
            for (int i = 0; i < oldBits.Length; i++)
            {
                if (oldBits[i] == newBits[i]) continue;
                if (newBits[i])
                    tbIsSet += i.ToString("0000") + ",";
                else
                    tbUnSet += i.ToString("0000") + ",";
            }
            TB_IsSet.Text = tbIsSet;
            TB_UnSet.Text = tbUnSet;

            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Copy Event Constant diff to clipboard?"))
                return;

            string r = "";
            for (int i = 0; i < newConst.Length; i += 2)
            {
                ushort oldval = BitConverter.ToUInt16(oldConst, i);
                ushort newval = BitConverter.ToUInt16(oldConst, i);
                if (oldval != newval)
                    r += $"0x{(i/2).ToString("X2")}: {oldval}->{newval}{Environment.NewLine}";
            }
            Clipboard.SetText(r);
        }
        private byte[] oldFlags = new byte[0x180];
        private byte[] newFlags = new byte[0x180];
        private byte[] oldConst = new byte[Main.SAV.EventFlag - Main.SAV.EventConst];
        private byte[] newConst = new byte[Main.SAV.EventFlag - Main.SAV.EventConst];
        private void openSAV(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                loadSAV(sender, ofd.FileName);
        }
        private void loadSAV(object sender, string path)
        {
            FileInfo fi = new FileInfo(path);
            if (fi.Length != SAV6.SIZE_ORAS)
            {
                Util.Error("Invalid SAV Size", string.Format("File Size: 0x{1} ({0} bytes)", fi.Length, fi.Length.ToString("X5")), "File Loaded: " + path);
                return;
            }

            byte[] data = File.ReadAllBytes(path);
            if (sender == B_LoadOld)
            {
                oldFlags = data.Skip(Main.SAV.EventFlag).Take(0x180).ToArray();
                oldConst = data.Skip(Main.SAV.EventConst).Take(Constants.Length * 2).ToArray();
                TB_OldSAV.Text = path;
            }
            else
            {
                newFlags = data.Skip(Main.SAV.EventFlag).Take(0x180).ToArray();
                newConst = data.Skip(Main.SAV.EventConst).Take(Constants.Length * 2).ToArray();
                TB_NewSAV.Text = path;
            }
        }
        private int entry = -1;
        private void changeConstantIndex(object sender, EventArgs e)
        {
            if (entry > -1) // Set Entry
                Constants[entry] = (ushort)Math.Min(Util.ToUInt32(MT_Stat.Text), 0xFFFF);

            entry = CB_Stats.SelectedIndex; // Get Entry
            MT_Stat.Text = Constants[entry].ToString();
        }
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            loadSAV(Util.Prompt(MessageBoxButtons.YesNo, "FlagDiff Researcher:", "Yes: Old Save" + Environment.NewLine + "No: New Save") == DialogResult.Yes ? B_LoadOld : B_LoadNew, files[0]);
        }
    }
}