using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_PokeBlockORAS : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6AO SAV;

        public SAV_PokeBlockORAS(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV6AO)(Origin = sav).Clone();
            nup_spec = new[] { NUP_Red, NUP_Blue, NUP_Pink, NUP_Green, NUP_Yellow, NUP_Rainbow, NUP_RedPlus, NUP_BluePlus, NUP_PinkPlus, NUP_GreenPlus, NUP_YellowPlus, NUP_RainbowPlus };
            Label[] lbl_spec = { L_Red, L_Blue, L_Pink, L_Green, L_Yellow, L_Rainbow, L_RedPlus, L_BluePlus, L_PinkPlus, L_GreenPlus, L_YellowPlus, L_RainbowPlus };

            for (int i = 0; i < lbl_spec.Length; i++)
            {
                lbl_spec[i].Text = $"{GameInfo.Strings.pokeblocks[94 + i]}:";
                nup_spec[i].Value = BitConverter.ToUInt32(SAV.Data, SAV6AO.Contest + (i * 4));
            }
        }

        private readonly NumericUpDown[] nup_spec;

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < nup_spec.Length; i++)
                BitConverter.GetBytes((uint)nup_spec[i].Value).CopyTo(SAV.Data, SAV6AO.Contest + (i * 4));
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_RandomizeBerries_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Repopulate all berry plots with random berries?"))
                return;

            // Randomize the trees.
            byte[] tree = { 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x80, 0x40, 0x01, 0x00, 0x00, 0x00, };
            var plantable = Legal.Pouch_Berry_XY; // 0 index is None, skip with rand
            var rnd = Util.Rand;
            for (int i = 0; i < 90; i++) // amount of plots in the game
            {
                ushort berry = plantable[rnd.Next(1, plantable.Length)]; // get random berry item ID from list
                BitConverter.GetBytes(berry).CopyTo(tree, 6); // put berry into tree.
                tree.CopyTo(SAV.Data, SAV.BerryField + (0x10 * i)); // put tree into plot
            }
        }

        private void B_GiveAllBlocks_Click(object sender, EventArgs e)
        {
            foreach (NumericUpDown n in nup_spec)
                n.Value = ModifierKeys == Keys.Control ? 0 : 999;
        }
    }
}
