using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_HallOfFame : Form
    {
        public SAV_HallOfFame(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            shiftval = savindex * 0x7F000;
            if (m_parent.savegame_oras) data_offset = 0x1F200;
            listBox1.SelectedIndex = 0;
        }
        Form1 m_parent;
        public byte[] sav = new Byte[0x100000];
        public int savindex; int shiftval;
        public bool editing = false;
        private int data_offset = 0x1E800;

        private void B_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void displayEntry(object sender, EventArgs e)
        {
            RTB.Font = new Font("Courier New", 8);
            RTB.Text = "";
            RTB.LanguageOption = RichTextBoxLanguageOptions.DualFont;
            int index = listBox1.SelectedIndex;
            int offset = shiftval + data_offset + index * 0x1B4;

            uint vnd = BitConverter.ToUInt32(sav, offset + 0x1B0);
            uint vn = vnd & 0xFF;
            RTB.Text = "Entry #" + vn + "\r\n";
            uint date = (vnd >> 14) & 0x1FFFF;
            uint year = (date & 0xFF) + 2000;
            uint month = (date >> 8) & 0xF;
            uint day = (date >> 12);
            if (day == 0)
            {
                RTB.Text += "No records in this slot.";
                return;
            }
            RTB.Text += "Date: " + year.ToString() + "/" + month.ToString() + "/" + day.ToString() + "\r\n\r\n";

            for (int i = 0; i < 6; i++)
            {
                int species = BitConverter.ToUInt16(sav, offset + 0x00);
                int helditem = BitConverter.ToUInt16(sav, offset + 0x02);
                int move1 = BitConverter.ToUInt16(sav, offset + 0x04);
                int move2 = BitConverter.ToUInt16(sav, offset + 0x06);
                int move3 = BitConverter.ToUInt16(sav, offset + 0x08);
                int move4 = BitConverter.ToUInt16(sav, offset + 0x0A);

                int TID = BitConverter.ToUInt16(sav, offset + 0x10);
                int SID = BitConverter.ToUInt16(sav, offset + 0x12);

                uint slgf = BitConverter.ToUInt32(sav, offset + 0x14);
                uint form = slgf & 0x1F;
                uint gender = (slgf >> 5) & 3; // 0 M; 1 F; 2 G
                uint level = (slgf >> 7) & 0x7F;
                uint shiny = (slgf >> 14) & 0x1;
                uint unkn = slgf >> 15;

                string nickname = Util.TrimFromZero(Encoding.Unicode.GetString(sav, offset + 0x18, 22));
                string OTname = Util.TrimFromZero(Encoding.Unicode.GetString(sav, offset + 0x30, 22));

                if (species == 0) 
                {
                    continue; 
                }
                string genderstr="";
                switch (gender)
                {
                    case 0:
                        genderstr = "M";
                        break;
                    case 1:
                        genderstr = "F";
                        break;
                    case 2:
                        genderstr = "G";
                        break;
                }
                string shinystr = "";
                if (shiny == 0)
                {
                    shinystr = "No";
                }
                else shinystr = "Yes";

                RTB.Text += "Name: " + nickname;
                RTB.Text += " (" + Form1.specieslist[species] + " - " + genderstr + ")\r\n";
                RTB.Text += "Level: " + level.ToString() + "\r\n";
                RTB.Text += "Shiny: " + shinystr + "\r\n";
                RTB.Text += "Held Item: " + Form1.itemlist[helditem] + "\r\n";
                RTB.Text += "Move 1: " + Form1.movelist[move1] + "\r\n";
                RTB.Text += "Move 2: " + Form1.movelist[move2] + "\r\n";
                RTB.Text += "Move 3: " + Form1.movelist[move3] + "\r\n";
                RTB.Text += "Move 4: " + Form1.movelist[move4] + "\r\n";
                RTB.Text += "OT: " + OTname + " (" + TID.ToString() + "/" + SID.ToString() + ")\r\n";
                RTB.Text += "\r\n";

                offset += 0x48;
            }
            RTB.Text = RTB.Text;
            RTB.Font = new Font("Courier New", 8);
        }
    }
}
