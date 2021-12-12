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
            B_DialgaPalkia.Enabled = SAV.Work.GetFlag(308) && SAV.Work.GetWork(84) != 5; // FE_D05R0114_SPPOKE_GET, WK_SCENE_D05R0114 (1-3 story related, 4 = captured, 5 = can retry)
            B_Roamer.Enabled = SAV.Encounter.Roamer1Encount != 1 || SAV.Encounter.Roamer2Encount != 1; // 0 = inactive, 1 = roaming, 2 = KOed, 3 = captured
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

        private void B_DialgaPalkia_Click(object sender, EventArgs e)
        {
            SAV.Work.SetFlag(308, false); // captured
            SAV.Work.SetFlag(393, false); // clear vanish
            SAV.Work.SetFlag(1623, false); // can retry
            SAV.Work.SetWork(84, 5); // can retry

            System.Media.SystemSounds.Asterisk.Play();
            B_DialgaPalkia.Enabled = false;
        }

        private void B_Roamer_Click(object sender, EventArgs e)
        {
            // Mesprit
            SAV.Work.SetFlag(249, false); // clear met
            SAV.Work.SetFlag(420, false); // clear vanish
            SAV.Encounter.Roamer1Encount = 0; // not actively roaming

            // Cresselia
            SAV.Work.SetFlag(245, false); // clear met
            SAV.Work.SetFlag(532, false); // clear vanish
            SAV.Encounter.Roamer2Encount = 0; // not actively roaming

            System.Media.SystemSounds.Asterisk.Play();
            B_Roamer.Enabled = false;
        }
    }
}
