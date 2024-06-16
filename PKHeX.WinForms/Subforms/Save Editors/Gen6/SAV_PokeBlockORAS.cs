using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_PokeBlockORAS : Form
{
    private readonly SaveFile Origin;
    private readonly SAV6AO SAV;

    public SAV_PokeBlockORAS(SAV6AO sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV6AO)(Origin = sav).Clone();
        nup_spec = [NUP_Red, NUP_Blue, NUP_Pink, NUP_Green, NUP_Yellow, NUP_Rainbow, NUP_RedPlus, NUP_BluePlus, NUP_PinkPlus, NUP_GreenPlus, NUP_YellowPlus, NUP_RainbowPlus];
        Label[] lbl_spec = [L_Red, L_Blue, L_Pink, L_Green, L_Yellow, L_Rainbow, L_RedPlus, L_BluePlus, L_PinkPlus, L_GreenPlus, L_YellowPlus, L_RainbowPlus];

        var contest = SAV.Contest;
        for (int i = 0; i < lbl_spec.Length; i++)
        {
            lbl_spec[i].Text = $"{GameInfo.Strings.pokeblocks[94 + i]}:";
            nup_spec[i].Value = contest.GetBlockCount(i);
        }
    }

    private readonly NumericUpDown[] nup_spec;

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        var contest = SAV.Contest;
        for (int i = 0; i < nup_spec.Length; i++)
            contest.SetBlockCount(i, (uint)nup_spec[i].Value);
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_RandomizeBerries_Click(object sender, EventArgs e)
    {
        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Repopulate all berry plots with random berries?"))
            return;

        // Randomize the trees.
        SAV.BerryField.ResetAndRandomize(Util.Rand, ItemStorage6XY.Pouch_Berry_XY);
    }

    private void B_GiveAllBlocks_Click(object sender, EventArgs e)
    {
        foreach (NumericUpDown n in nup_spec)
            n.Value = ModifierKeys == Keys.Control ? 0 : 999;
    }
}
