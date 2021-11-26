using System;
using System.Windows.Forms;
using PKHeX.Core;
namespace PKHeX.WinForms
{
    public partial class SAV_Misc8b : Form
    {
        private readonly SAV8BS Origin;
        private readonly SAV8BS SAV;

        public SAV_Misc8b(SAV8BS sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV8BS)(Origin = sav).Clone();

            ReadMain();
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveMain();
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void ReadMain()
        {
            B_Spiritomb.Enabled = SAV.UgSaveData.TalkedNPC < 32;
            B_Darkrai.Enabled = SAV.Work.GetFlag(301) || !(SAV.Work.GetWork(275) == 1 && SAV.Zukan.HasNationalDex && SAV.Items.GetItemQuantity(454) == 1); // HAIHUEVENT_ID_D18, Member Card
            B_Shaymin.Enabled = SAV.Work.GetFlag(545) || !(SAV.Work.GetWork(276) == 1 && SAV.Zukan.HasNationalDex && SAV.Items.GetItemQuantity(452) == 1 && SAV.Work.GetSystemFlag(5)); // HAIHUEVENT_ID_D30, Oak's Letter
        }

        private void SaveMain()
        {
        }

        private void B_Spiritomb_Click(object sender, EventArgs e)
        {
            var trainers = SAV.UgSaveData.GetTrainers();
            for (int i = 0; i < 32; i++)
                trainers[i] = (byte)(i + 1);
            System.Media.SystemSounds.Asterisk.Play();
            B_Spiritomb.Enabled = false;
        }

        private void B_Shaymin_Click(object sender, EventArgs e)
        {
            SAV.Zukan.HasNationalDex = true; // dex
            SAV.Work.SetSystemFlag(5, true); // clear
            SAV.Work.SetWork(276, 1); // haihu
            SAV.Items.SetItemQuantity(452, 1); // letter
            SAV.Work.SetFlag(545, false); // clear vanish

            System.Media.SystemSounds.Asterisk.Play();
            B_Shaymin.Enabled = false;
        }

        private void B_Darkrai_Click(object sender, EventArgs e)
        {
            SAV.Zukan.HasNationalDex = true; // dex
            SAV.Work.SetWork(275, 1); // haihu
            SAV.Items.SetItemQuantity(454, 1); // member
            SAV.Work.SetFlag(301, false); // clear vanish

            System.Media.SystemSounds.Asterisk.Play();
            B_Darkrai.Enabled = false;
        }
    }
}
