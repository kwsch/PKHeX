using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PKHeX
{
    public partial class SAV_EventFlagsORAS : Form
    {
        public SAV_EventFlagsORAS(Form1 frm1)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            m_parent = frm1;
            savshift = 0x7F000 * m_parent.savindex;

            Setup();

            nud.Text = "0"; // Prompts an update for flag 0.
            MT_Ash.Text = BitConverter.ToUInt16(m_parent.savefile, savshift + 0x14A78 + 0x5400).ToString();
        }
        Form1 m_parent;
        public int savshift;
        bool setup = true;
        public CheckBox[] chka;
        public bool[] flags = new bool[3072];
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            // Gather Updated Flags
            for (int i = 0; i < chka.Length; i++)
                flags[getFlagNum(chka[i])] = chka[i].Checked;

            byte[] data = new byte[flags.Length / 8]; 
            
            for (int i = 0; i < flags.Length; i++)
                if (flags[i])
                    data[i/8] |= (byte)(1 << i%8);

            Array.Copy(data, 0, m_parent.savefile, 0x1A0FC + savshift, 0x180);
            
            // Copy back Volcanic Ash counter
            Array.Copy(BitConverter.GetBytes(Util.ToUInt32(MT_Ash)), 0, m_parent.savefile, 0x14A78 + 0x5400 + savshift, 2);

            this.Close();
        }
        private void Setup()
        {
            // Fill Bit arrays
            
            chka = new CheckBox[] {
                
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
            int offset = 0x1A0FC + savshift;
            byte[] data = new byte[0x180];
            Array.Copy(m_parent.savefile, offset, data, 0, 0x180);
            BitArray BitRegion = new BitArray(data);
            BitRegion.CopyTo(flags, 0);

            // Populate Flags
            setup = true;
            popFlags();
        }
        private void popFlags()
        {
            if (setup)
            {
                for (int i = 0; i < chka.Length; i++)
                    chka[i].Checked = flags[getFlagNum(chka[i])];

                changeCustomFlag(null, null);
            }
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
                nud.BackColor = Form1.defaultControlWhite;
                CHK_CustomFlag.Checked = flags[flag];
            }
        }
        private void changeCustomFlag(object sender, KeyEventArgs e)
        {
            changeCustomFlag(null, (EventArgs)e);
        }

        private void toggleFlag(object sender, EventArgs e)
        {
            flags[getFlagNum((CheckBox)(sender))] = ((CheckBox)(sender)).Checked;
            changeCustomFlag(sender, e);
        }

        private void changeSAV(object sender, EventArgs e)
        {
            if (TB_NewSAV.Text.Length > 0 && TB_OldSAV.Text.Length > 0)
                diffSaves();
        }
        private void diffSaves()
        {
            BitArray oldBits = new BitArray(olddata);
            BitArray newBits = new BitArray(newdata);

            for (int i = 0; i < oldBits.Length; i++)
                if (oldBits[i] != newBits[i])
                    if (newBits[i])
                        TB_IsSet.AppendText(i.ToString("0000") + ",");
                    else
                        TB_UnSet.AppendText(i.ToString("0000") + ",");
        }
        private byte[] olddata = new byte[0x180];
        private byte[] newdata = new byte[0x180];
        private void openSAV(object sender, EventArgs e)
        {
            byte[] eventflags = new byte[0x180];
            string path = "";
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
                FileInfo fi = new FileInfo(path);
                if (fi.Length == 0x100000)
                {
                    byte[] data = File.ReadAllBytes(path);
                    Array.Copy(data, 0x1A0FC, eventflags, 0, 0x180);
                }
                else if (fi.Length == 0x76000)
                {
                    byte[] data = File.ReadAllBytes(path);
                    Array.Copy(data, 0x1A0FC - 0x5400, eventflags, 0, 0x180);
                }
                else
                { Util.Error("Invalid SAV Size", String.Format("File Size: {0} (bytes)"), fi.Length.ToString(), "File Loaded: " + path); return; }
            }
            else 
                return;
            
            Button bs = (Button)sender;
            if (bs.Name == "B_LoadOld")
            {
                Array.Copy(eventflags, olddata, 0x180);
                TB_OldSAV.Text = path;
            }
            else 
            {
                Array.Copy(eventflags, newdata, 0x180);
                TB_NewSAV.Text = path;
            }
        }
    }
}
