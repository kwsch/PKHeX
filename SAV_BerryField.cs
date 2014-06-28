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
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            shiftval = savindex * 0x7F000;
            listBox1.SelectedIndex = 0;
        }
        int berryfoffset;
        Form1 m_parent;
        public byte[] sav = new Byte[0x100000];
        public int savindex; int shiftval;
        public bool editing = false;

        private void changefield(object sender, EventArgs e)
        {
            // Change Berry Field
            int offset = berryfoffset + shiftval;
            int index = BitConverter.ToUInt16(sav, offset + 0xC + listBox1.SelectedIndex*0x18);
            textBox1.Text = index.ToString();

        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
