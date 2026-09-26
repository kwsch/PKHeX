using PKHeX.Core;
using System;
using System.Windows.Forms;
using static PKHeX.Core.RibbonIndex;

namespace PKHeX.WinForms;

public partial class SAV_GiftRibbons : Form
{
    private readonly SaveFile Origin;
    private readonly SaveFile SAV;
    private readonly IGiftRibbons Block;
    private static RibbonStrings RibbonStrings => GameInfo.Strings.Ribbons;
    private int Count => Block.GiftRibbons.Length;

    public SAV_GiftRibbons(IGiftRibbons sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        Origin = (SaveFile)sav;
        SAV = Origin.Clone();
        Block = (IGiftRibbons)SAV;

        Setup();
        LoadValues();
    }

    private void Setup()
    {
        dgv.Rows.Clear();
        dgv.Rows.Add(Count);
        for (int i = 0; i < Count; i++)
            dgv.Rows[i].Cells[0].Value = RibbonStrings.GetName(IGiftRibbons.Index[i].PropertyName);
    }

    private void LoadValues()
    {
        for (int i = 0; i < Count; i++)
            dgv.Rows[i].Cells[1].Value = Block.GiftRibbons[i].ToString();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Legal_Click(object sender, EventArgs e)
    {
        Block.GiftRibbonsClear();
        if (SAV.Generation == 3)
        {
            Block.GiftRibbonSet(Country, 32);  // "POKéMON LEAGUE CHAMPION RIBBON"
            Block.GiftRibbonSet(National, 44); // "RIBBON awarded for clearing all difficulties."
            Block.GiftRibbonSet(Earth, 45);    // "100-straight Win Commemorative RIBBON"
            Block.GiftRibbonSet(World, 32);    // "POKéMON LEAGUE CHAMPION RIBBON"
        }
        else if (SAV.Generation == 4)
        {
            Block.GiftRibbonSet(Classic, 64); // "A Ribbon that proclaims love for Pokémon."
            Block.GiftRibbonSet(Premier, 59); // "Special Holiday Ribbon"
        }
        LoadValues();
    }

    private void B_Reset_Click(object sender, EventArgs e)
    {
        Block.GiftRibbonsClear();
        LoadValues();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < Count; i++)
        {
            var cells = dgv.Rows[i].Cells;
            var desc = byte.TryParse(cells[1].Value?.ToString(), out var val) ? val : (byte)0;
            Block.GiftRibbons[i] = desc;
        }
        Origin.CopyChangesFrom(SAV);
        Close();
    }
}
