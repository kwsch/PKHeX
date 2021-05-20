using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Raid8 : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV8SWSH SAV;
        private readonly RaidSpawnList8 Raids;

        public SAV_Raid8(SaveFile sav, RaidSpawnList8 raid)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV8SWSH)(Origin = sav).Clone();
            Raids = raid;
            CB_Den.Items.AddRange(Enumerable.Range(1, raid.CountUsed).Select(z => (object)$"Den {z:000}").ToArray());
            CB_Den.SelectedIndex = 0;
        }

        private void LoadDen(int index) => PG_Den.SelectedObject = Raids.GetRaid(index);

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            // We've been editing the original save file blocks. Restore the clone's data.
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CB_Den_SelectedIndexChanged(object sender, EventArgs e) => LoadDen(CB_Den.SelectedIndex);

        private void B_ActivateAll_Click(object sender, EventArgs e)
        {
            bool alt = (ModifierKeys & Keys.Alt) != 0;
            if (alt)
            {
                if ((ModifierKeys & Keys.Control) == 0)
                {
                    Raids.DectivateAllRaids();
                    LoadDen(CB_Den.SelectedIndex);
                }
                var txt = Raids.DumpAll();
                var concat = string.Join(Environment.NewLine, txt);
                if (WinFormsUtil.SetClipboardText(concat))
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            bool rare = (ModifierKeys & Keys.Control) != 0;
            bool isEvent = (ModifierKeys & Keys.Shift) != 0;
            Raids.ActivateAllRaids(rare, isEvent);
            LoadDen(CB_Den.SelectedIndex);
        }
    }
}
