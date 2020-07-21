using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_BerryFieldXY : Form
    {
        private readonly SAV6XY SAV;

        public SAV_BerryFieldXY(SAV6XY sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = sav;
            listBox1.SelectedIndex = 0;
        }

        private void Changefield(object sender, EventArgs e)
        {
            // Change Berry Field

            // Gather Data
            int ofs = SAV.BerryField + 0xC + (listBox1.SelectedIndex * 0x18);
            int berry = BitConverter.ToUInt16(SAV.Data, ofs + (2 * 0));
            int u1 =    BitConverter.ToUInt16(SAV.Data, ofs + (2 * 1));
            int u2 =    BitConverter.ToUInt16(SAV.Data, ofs + (2 * 2));
            int u3 =    BitConverter.ToUInt16(SAV.Data, ofs + (2 * 3));
            int u4 =    BitConverter.ToUInt16(SAV.Data, ofs + (2 * 4));
            int u5 =    BitConverter.ToUInt16(SAV.Data, ofs + (2 * 5));
            int u6 =    BitConverter.ToUInt16(SAV.Data, ofs + (2 * 6));
            int u7 =    BitConverter.ToUInt16(SAV.Data, ofs + (2 * 7));

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
