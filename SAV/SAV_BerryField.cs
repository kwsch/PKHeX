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
    public partial class SAV_BerryField : Form
    {
        public SAV_BerryField(Form1 frm1, int bfo)
        {
            berryfoffset = bfo;
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            shiftval = savindex * 0x7F000;
            listBox1.SelectedIndex = 0;
        }
        int berryfoffset;
        Form1 m_parent;
        public byte[] sav = new byte[0x100000];
        public int savindex; int shiftval;
        public bool editing = false;

        private void changefield(object sender, EventArgs e)
        {
            // Change Berry Field
            int offset = berryfoffset + shiftval;

            // Gather Data
            int berry = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex*0x18 + 1 * 0);
            int u1 = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex * 0x18 + 1 * 2);
            int u2 = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex * 0x18 + 2 * 2);
            int u3 = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex * 0x18 + 3 * 2);
            int u4 = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex * 0x18 + 4 * 2);
            int u5 = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex * 0x18 + 5 * 2);
            int u6 = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex * 0x18 + 6 * 2);
            int u7 = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex * 0x18 + 7 * 2);


            // Display Data
            TB_Berry.Text = berry.ToString();
            TB_u1.Text = u1.ToString();
            TB_u2.Text = u2.ToString();
            TB_u3.Text = u3.ToString();
            TB_u4.Text = u4.ToString();
            TB_u5.Text = u5.ToString();
            TB_u6.Text = u6.ToString();
            TB_u7.Text = u7.ToString();

        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
