using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_SimplePokedex : Form
    {
        private readonly SaveFile Origin;
        private readonly SaveFile SAV;

        public SAV_SimplePokedex(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (Origin = sav).Clone();
            seen = new bool[SAV.MaxSpeciesID];
            caught = new bool[SAV.MaxSpeciesID];

            string[] spec = Util.GetSpeciesList(Main.CurrentLanguage);
            for (int i = 0; i < seen.Length; i++)
            {
                int species = i + 1;
                seen[i] = SAV.GetSeen(species);
                caught[i] = SAV.GetCaught(species);
                CLB_Seen.Items.Add(spec[species]);
                CLB_Caught.Items.Add(spec[species]);
                CLB_Seen.SetItemChecked(i, seen[i]);
                CLB_Caught.SetItemChecked(i, caught[i]);
            }
            initialized = true;
        }

        private readonly bool[] seen;
        private readonly bool[] caught;
        private readonly bool initialized;

        private void B_Save_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < seen.Length; i++)
            {
                int species = i + 1;
                SAV.SetSeen(species, seen[i]);
                SAV.SetCaught(species, caught[i]);
            }
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_SeenAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SAV.MaxSpeciesID; i++)
                CLB_Seen.SetItemChecked(i, true);
        }

        private void B_SeenNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SAV.MaxSpeciesID; i++)
                CLB_Seen.SetItemChecked(i, false);
        }

        private void B_CaughtAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SAV.MaxSpeciesID; i++)
                CLB_Caught.SetItemChecked(i, true);
        }

        private void B_CaughtNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SAV.MaxSpeciesID; i++)
                CLB_Caught.SetItemChecked(i, false);
        }

        private void CLB_Seen_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!initialized) return;
            seen[e.Index] = e.NewValue == CheckState.Checked;
        }

        private void CLB_Caught_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!initialized) return;
            caught[e.Index] = e.NewValue == CheckState.Checked;
        }
    }
}
