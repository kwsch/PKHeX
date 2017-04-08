using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Misc4 : Form
    {
        private readonly SAV4 SAV = (SAV4)Main.SAV.Clone();
        public SAV_Misc4()
        {
            InitializeComponent();

            Apps = new[] {
                CHK_App01,CHK_App02,CHK_App03,CHK_App04,CHK_App05,
                CHK_App06,CHK_App07,CHK_App08,CHK_App09,CHK_App10,
                CHK_App11,CHK_App12,CHK_App13,CHK_App14,CHK_App15,
                CHK_App16,CHK_App17,CHK_App18,CHK_App19,CHK_App20,
                CHK_App21,CHK_App22,CHK_App23,CHK_App24,CHK_App25,
            };

            int ret = SAV.PoketchApps;
            for(int i = 0; i < Apps.Length; i++)
            {
                Apps[i].Checked = (ret & 1 << i) != 0;
            }
        }

        private readonly CheckBox[] Apps;

        private void saveMisc()
        {
            int ret = 0;
            for(int i = 0; i < Apps.Length; i++)
            {
                if (Apps[i].Checked) ret |= 1 << i;
            }
            SAV.PoketchApps = ret;
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            saveMisc();
            SAV.Data.CopyTo(Main.SAV.Data, 0);
            Main.SAV.Edited = true;
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            foreach (CheckBox c in Apps) c.Checked = true;
        }
    }
}
