using System;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_SimplePokedex : Form
    {
        public SAV_SimplePokedex()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            seen = new bool[SAV.MaxSpeciesID];
            caught = new bool[SAV.MaxSpeciesID];

            string[] spec = Util.getSpeciesList(Main.curlanguage);
            for (int i = 0; i < seen.Length; i++)
            {
                PKM tempPkm = new PK6();
                tempPkm.Species = i + 1;
                seen[i] = SAV.getSeen(tempPkm);
                caught[i] = SAV.getCaught(tempPkm);
                CLB_Seen.Items.Add(spec[i + 1]);
                CLB_Caught.Items.Add(spec[i + 1]);
                CLB_Seen.SetItemChecked(i, seen[i]);
                CLB_Caught.SetItemChecked(i, caught[i]);
            }
            initialized = true;
        }
        private readonly SaveFile SAV = Main.SAV.Clone();

        private readonly bool[] seen;
        private readonly bool[] caught;
        private readonly bool initialized;

        private void B_Save_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < seen.Length; i++)
            {
                PKM tempPkm = new PK6();
                tempPkm.Species = i + 1;
                SAV.setSeen(tempPkm, seen[i]);
                SAV.setCaught(tempPkm, caught[i]);
            }
            SAV.Data.CopyTo(Main.SAV.Data, 0);
            Main.SAV.Edited = true;
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
