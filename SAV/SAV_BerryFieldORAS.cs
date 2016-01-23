using System;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_BerryFieldORAS : Form
    {
        public SAV_BerryFieldORAS()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            sav = Main.SAV.Data.Clone() as byte[];
            nup_spec = new[] { NUP_Red, NUP_Blue, NUP_Pink, NUP_Green, NUP_Yellow, NUP_Rainbow, NUP_RedPlus, NUP_BluePlus, NUP_PinkPlus, NUP_GreenPlus, NUP_YellowPlus, NUP_RainbowPlus };
            lbl_spec = new[] { L_Red, L_Blue, L_Pink, L_Green, L_Yellow, L_Rainbow, L_RedPlus, L_BluePlus, L_PinkPlus, L_GreenPlus, L_YellowPlus, L_RainbowPlus };

            for (int i = 0; i < lbl_spec.Length; i++)
            {
                lbl_spec[i].Text = string.Format("{0}:", Main.pokeblocks[94 + i]);
                nup_spec[i].Value = BitConverter.ToUInt32(sav, Main.SAV.Contest + (i) * 4);
            }
            editing = true;
        }
        public byte[] sav;
        public bool editing = false;

        private NumericUpDown[] nup_spec;
        private Label[] lbl_spec;

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            Main.SAV.Data = sav.Clone() as byte[];
            Close();
        }

        private void B_RandomizeBerries_Click(object sender, EventArgs e)
        {
            DialogResult dr = Util.Prompt(MessageBoxButtons.YesNo, "Repopulate all with random berries?");
            if (dr != DialogResult.Yes) return; // abort
                                                // Randomize the trees.

            byte[] ready = { 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x80, 0x40, 0x01, 0x00, 0x00, 0x00, };
            int[] berrylist =
            {
                    0,149,150,151,152,153,154,155,156,157,158,159,160,161,162,
                    163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,
                    178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,
                    193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,
                    208,209,210,211,212,686,687,688,
                };
            for (int i = 0; i < 90; i++)
            {
                Array.Copy(ready, 0, sav, Main.SAV.BerryField + 0x10 * i, 0x10); // prep the berry template tree (which we replace offset 0x6 for the Tree Item)
                int randberry = (int)(Util.rnd32() % berrylist.Length); // generate a random berry that will go into the tree
                int index = berrylist[randberry]; // get berry item ID from list
                Array.Copy(BitConverter.GetBytes(index), 0, sav, Main.SAV.BerryField + 0x10 * i + 6, 2); // put berry into tree.
            }
        }

        private void NUP_Pokeblock_ValueChanged(object sender, EventArgs e)
        {
            if (!editing)
                return;
            for (int i = 0; i < nup_spec.Length; i++)
            {
                BitConverter.GetBytes((uint)nup_spec[i].Value).CopyTo(sav, Main.SAV.Contest + i * 4);
            }
        }
    }
}
