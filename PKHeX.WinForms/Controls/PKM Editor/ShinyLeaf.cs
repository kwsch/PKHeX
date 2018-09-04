using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    public partial class ShinyLeaf : UserControl
    {
        public ShinyLeaf()
        {
            InitializeComponent();
            Flags = new[] {CHK_1, CHK_2, CHK_3, CHK_4, CHK_5, CHK_C};
            greyLeaf = ImageUtil.ChangeOpacity(ImageUtil.ToGrayscale(CHK_1.Image), 0.4);
            greyCrown = ImageUtil.ChangeOpacity(ImageUtil.ToGrayscale(CHK_C.Image), 0.4);
            foreach (var chk in Flags)
                UpdateFlagState(chk, null);
        }

        private readonly CheckBox[] Flags;
        private readonly Bitmap greyLeaf, greyCrown;
        public void CheckAll(bool all = true) => Value = all ? 0b00111111 : 0;

        public int Value
        {
            get
            {
                int value = 0;
                for (int i = 0; i < Flags.Length; i++)
                {
                    if (Flags[i].Checked)
                        value |= 1 << i;
                }

                return value;
            }
            set
            {
                for (int i = 0; i < Flags.Length; i++)
                    Flags[i].Checked = (value >> i & 1) == 1;
            }
        }

        private void UpdateFlagState(object sender, EventArgs e)
        {
            if (!(sender is CheckBox c))
                return;

            if (CHK_C == c)
            {
                c.Image = c.Checked ? Resources.crown : greyCrown;
            }
            else
            {
                if (!c.Checked)
                    CHK_C.Checked = CHK_C.Enabled = false;
                else if (Flags.Take(5).All(z => z.Checked))
                    CHK_C.Enabled = true;
                c.Image = c.Checked ? Resources.leaf : greyLeaf;
            }
        }
    }
}
