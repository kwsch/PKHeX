using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Drawing;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    public partial class ShinyLeaf : UserControl
    {
        public ShinyLeaf()
        {
            InitializeComponent();
            Flags = new[] { CHK_1, CHK_2, CHK_3, CHK_4, CHK_5, CHK_C };
        }

        private readonly CheckBox[] Flags;

        public void CheckAll(bool all = true) => SetValue(all ? 0b00111111 : 0);

        public int GetValue()
        {
            int value = 0;
            for (int i = 0; i < Flags.Length; i++)
            {
                if (Flags[i].Checked)
                    value |= 1 << i;
            }
            return value;
        }

        public void SetValue(int value)
        {
            for (int i = 0; i < Flags.Length; i++)
                Flags[i].Checked = (value >> i & 1) == 1;
        }

        private void UpdateFlagState(object sender, EventArgs e)
        {
            if (sender is not CheckBox c)
                return;

            Image resource;
            if (CHK_C == c)
            {
                resource = Resources.crown;
            }
            else
            {
                resource = Resources.leaf;
                if (!c.Checked)
                    CHK_C.Checked = CHK_C.Enabled = false;
                else if (Flags.Take(5).All(z => z.Checked))
                    CHK_C.Enabled = true;
            }
            if (!c.Checked)
                resource = ImageUtil.ChangeOpacity(resource, 0.4);
            c.Image = resource;
        }
    }
}
