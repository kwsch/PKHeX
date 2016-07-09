using System;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_BerryFieldXY : Form
    {
        public SAV_BerryFieldXY()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            listBox1.SelectedIndex = 0;
        }
        private readonly SAV6 SAV = new SAV6(Main.SAV.Data);

        private void changefield(object sender, EventArgs e)
        {
            // Change Berry Field

            // Gather Data
            int berry = BitConverter.ToUInt16(SAV.Data, SAV.BerryField + 0xC + listBox1.SelectedIndex*0x18 + 1 * 0);
            int u1 = BitConverter.ToUInt16(SAV.Data, SAV.BerryField + 0xC + listBox1.SelectedIndex * 0x18 + 1 * 2);
            int u2 = BitConverter.ToUInt16(SAV.Data, SAV.BerryField + 0xC + listBox1.SelectedIndex * 0x18 + 2 * 2);
            int u3 = BitConverter.ToUInt16(SAV.Data, SAV.BerryField + 0xC + listBox1.SelectedIndex * 0x18 + 3 * 2);
            int u4 = BitConverter.ToUInt16(SAV.Data, SAV.BerryField + 0xC + listBox1.SelectedIndex * 0x18 + 4 * 2);
            int u5 = BitConverter.ToUInt16(SAV.Data, SAV.BerryField + 0xC + listBox1.SelectedIndex * 0x18 + 5 * 2);
            int u6 = BitConverter.ToUInt16(SAV.Data, SAV.BerryField + 0xC + listBox1.SelectedIndex * 0x18 + 6 * 2);
            int u7 = BitConverter.ToUInt16(SAV.Data, SAV.BerryField + 0xC + listBox1.SelectedIndex * 0x18 + 7 * 2);


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
