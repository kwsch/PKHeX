using System;
using System.Windows.Forms;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

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

        var span = SAV.Data.AsSpan(SAV6AO.Contest);
        for (int i = 0; i < lbl_spec.Length; i++)
        {
            lbl_spec[i].Text = $"{GameInfo.Strings.pokeblocks[94 + i]}:";
            nup_spec[i].Value = ReadUInt32LittleEndian(span[(i * 4)..]);
        }
    }

    private readonly NumericUpDown[] nup_spec;

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        var span = SAV.Data.AsSpan(SAV6AO.Contest);
        for (int i = 0; i < nup_spec.Length; i++)
            WriteUInt32LittleEndian(span[(i * 4)..], (uint)nup_spec[i].Value);
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private static ReadOnlySpan<byte> DefaultBerryTree => [ 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x80, 0x40, 0x01, 0x00, 0x00, 0x00 ];

    private void B_RandomizeBerries_Click(object sender, EventArgs e)
    {
        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Repopulate all berry plots with random berries?"))
            return;

        // Randomize the trees.
        ReadOnlySpan<byte> tree = DefaultBerryTree;
        var plantable = ItemStorage6XY.Pouch_Berry_XY; // 0 index is None, skip with rand
        var rnd = Util.Rand;

        var plots = SAV.Data.AsSpan(SAV.BerryField);
        for (int i = 0; i < 90; i++) // amount of plots in the game
        {
            var plot = plots[(i * 0x10)..];
            tree.CopyTo(plot); // put tree into plot

            ushort berry = plantable[rnd.Next(1, plantable.Length)]; // get random berry item ID from list
            WriteUInt16LittleEndian(plot[6..], berry); // put berry into tree.
        }
    }

    private void B_GiveAllBlocks_Click(object sender, EventArgs e)
    {
        foreach (NumericUpDown n in nup_spec)
            n.Value = ModifierKeys == Keys.Control ? 0 : 999;
    }
}
