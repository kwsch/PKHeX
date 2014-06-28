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
    public partial class SAV_EventFlags : Form
    {
        public SAV_EventFlags(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            populateFlags();
            nud.Text = "0";
        }
        Form1 m_parent;
        public byte[] sav = new Byte[0x100000];
        public int savindex;

        private void changeFlag(object sender, EventArgs e)
        {
            int offset = savindex * 0x7F000 + 0x1A0FC;
            int flag = (int)nud.Value;
            if (flag > 3072)
            {
                CHK_CustomFlag.Checked = false;
                nud.BackColor = Color.Red;
            }
            else
            {
                nud.BackColor = Color.White;
                CHK_CustomFlag.Checked = getFlagBool(flag, offset);
            }
        }
        private int getFlagNum(CheckBox chk)
        {
            string source = chk.Name;
            return Convert.ToInt32(source.Substring(Math.Max(0, source.Length - 4)));
        }
        private bool getFlagBool(int flagnum, int offset)
        {
            bool flagvalue = Convert.ToBoolean(((sav[offset + flagnum / 8]) >> (flagnum % 8)) & 1);
            return flagvalue;
        }
        private void populateFlags()
        {

            int offset = savindex * 0x7F000 + 0x1A0FC;
            CheckBox[] ck = {
                                flag_0001, flag_0002, flag_0003, flag_0004, flag_0005
                            };

            for (int i = 0; i < ck.Length; i++)
            {
                // Get Flag Number and get the bool: set or not
                ck[i].Checked = getFlagBool(getFlagNum(ck[i]),offset);
            }

        }

        private void B_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
