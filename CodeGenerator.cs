using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace PKHeX
{
    public partial class CodeGenerator : Form
    {
        Form1 m_parent;
        byte[] codedata = new Byte[232];
        byte[] newdata = new Byte[232];
        SaveGames.SaveStruct SaveGame = new SaveGames.SaveStruct(null);
        
        public CodeGenerator(Form1 frm1)
        {
            InitializeComponent();
            this.CenterToParent();
            RTB_Code.Clear();
            TB_Write.Clear();
            m_parent = frm1;
            SaveGame = m_parent.SaveGame;
            CB_Box.Items.Clear();
            for (int i = 1; i <= 31; i++)
                CB_Box.Items.Add(i.ToString());

            CB_Source.SelectedIndex = 0;
            CB_Slot.SelectedIndex = 0;
            CB_Box.SelectedIndex = 0;
        }
        private bool loaddata()
        {
            if (CB_Source.SelectedIndex == 0)
            {
                if (m_parent.verifiedpkx())
                {
                    byte[] pkx = m_parent.preparepkx(m_parent.buff);
                    newdata = new Byte[232];
                    Array.Copy(m_parent.encryptArray(pkx), newdata, 232);
                }
                else return false;

            }
            else if (CB_Source.SelectedIndex == 1)
            {
                newdata = new Byte[0xE8];
                Array.Copy(m_parent.savefile,
                    SaveGame.Box                   // Box Offset
                        + CB_Box.SelectedIndex * (232 * 30) // Box Shift
                        + CB_Slot.SelectedIndex * 232,      // Slot Shift
                    newdata, 0, 0xE8);
            }
            else if (CB_Source.SelectedIndex == 2)
            {
                // Wondercard
                newdata = new Byte[0x108];
                // Wondercard #
                int wcn = CB_Slot.SelectedIndex;
                // copy from save, the chosen wondercard offset, to new data
                Array.Copy(m_parent.savefile, SaveGame.Wondercard + wcn * 0x108, newdata, 0, 0x108);
            }
            return true;
        }
        private void changeDataSource(object sender, EventArgs e)
        {
            int sourceindex = CB_Source.SelectedIndex;
            B_Add.Enabled = true;

            if (sourceindex == 0)
            {
                // Hide Box/Etc
                CB_Box.Visible = false;
                L_Box.Visible = false;
                CB_Slot.Visible = false;
                L_Slot.Visible = false;

                B_Add.Enabled = m_parent.verifiedpkx();
            }
            else if (sourceindex == 1)
            {
                CB_Box.Visible = true;
                L_Box.Visible = true;
                CB_Slot.Visible = true;
                L_Slot.Visible = true;

                L_Slot.Text = "Slot:";

                CB_Slot.Items.Clear();
                for (int i = 1; i <= 30; i++)
                    CB_Slot.Items.Add(i.ToString());

                CB_Slot.SelectedIndex = 0;
            }
            else if (sourceindex == 2)
            {
                CB_Box.Visible = false;
                L_Box.Visible = false;
                CB_Slot.Visible = true;
                L_Slot.Visible = true;

                L_Slot.Text = "Card:";

                // Set up cards
                CB_Slot.Items.Clear();
                for (int i = 1; i <= 24; i++)
                    CB_Slot.Items.Add(i.ToString());

                CB_Slot.SelectedIndex = 0;
            }
        }

        private void changeSourceIndex(object sender, EventArgs e)
        {
        }

        public static string RemoveTroublesomeCharacters(TextBox tb)
        {
            string inString = tb.Text;
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                // filter for hex
                if ((ch < 0x0047 && ch > 0x002F) || (ch < 0x0067 && ch > 0x0060))
                    newString.Append(ch);
                else
                    System.Media.SystemSounds.Beep.Play();
            }
            if (newString.Length == 0)
                newString.Append("0");
            uint value = UInt32.Parse(newString.ToString(), NumberStyles.HexNumber);
            tb.Text = value.ToString("X8");
            return newString.ToString();

        }
        private uint getHEXval(TextBox tb)
        {
            if (tb.Text == null)
                return 0;
            string str = RemoveTroublesomeCharacters(tb);
            return UInt32.Parse(str, NumberStyles.HexNumber);
        }
        private void B_Add_Click(object sender, EventArgs e)
        {
            // Add the new code to the textbox.
            if (!loaddata()) return;
            uint writeoffset = getHEXval(TB_Write);
            for (int i = 0; i < newdata.Length / (4); i++)
            {
                // Add Operator

                RTB_Code.AppendText("00000001 ");   // 01 00 00 00
                RTB_Code.AppendText((writeoffset + i * 4).ToString("X8") + " ");
                RTB_Code.AppendText(BitConverter.ToUInt32(newdata,i*4).ToString("X8") + "\n");
            }
        }

        private void B_Clear_Click(object sender, EventArgs e)
        {
            RTB_Code.Clear();
        }

        private void B_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Code File|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;
                byte[] ncf = File.ReadAllBytes(path);
                uint length = BitConverter.ToUInt32(ncf, 0);

                if (ncf.Length != length + 4)
                {
                    MessageBox.Show("Not a valid code file.", "Error");
                    return;
                }
                if (RTB_Code.Text.Length > 0)
                {
                    DialogResult ld = MessageBox.Show("Replace current code?","Alert",MessageBoxButtons.YesNo);
                    if (ld == DialogResult.Yes)
                        RTB_Code.Clear();
                    else if (ld != DialogResult.No)
                        return;
                }
                for (int i = 4; i <= ncf.Length-12; i+=12)
                {
                    RTB_Code.AppendText(BitConverter.ToUInt32(ncf, i + 0 * 4).ToString("X8") + " ");
                    RTB_Code.AppendText(BitConverter.ToUInt32(ncf, i + 1 * 4).ToString("X8") + " ");
                    RTB_Code.AppendText(BitConverter.ToUInt32(ncf, i + 2 * 4).ToString("X8") + "\n");
                }
            }
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            // Gotta read in the textbox.
            if (RTB_Code.Text.Length < 1) return;

            byte[] ncf = new Byte[4 + (RTB_Code.Lines.Count()-1) * (3 * 4)];
            Array.Copy(BitConverter.GetBytes(ncf.Length - 4), ncf, 4);

            for (int i = 0; i < RTB_Code.Lines.Count()-1; i++)
            {
                string line = RTB_Code.Lines[i];
                string[] rip = Regex.Split(line, " ");

                // Write the 3 u32's to an array.
                Array.Copy(BitConverter.GetBytes(UInt32.Parse(rip[0], NumberStyles.HexNumber)),0,ncf,4+i*12+0,4);
                Array.Copy(BitConverter.GetBytes(UInt32.Parse(rip[1], NumberStyles.HexNumber)),0,ncf,4+i*12+4,4);
                Array.Copy(BitConverter.GetBytes(UInt32.Parse(rip[2], NumberStyles.HexNumber)),0,ncf,4+i*12+8,4);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "code.bin";
            sfd.Filter = "Code File|*.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string path = sfd.FileName;
                if (File.Exists(path))
                {
                    // File already exists, save a .bak
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(path + ".bak", backupfile);
                }
                File.WriteAllBytes(path, ncf);
            }
        }
    }
}
