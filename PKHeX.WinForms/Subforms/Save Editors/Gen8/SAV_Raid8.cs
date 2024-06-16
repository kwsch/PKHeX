using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Raid8 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV8SWSH SAV;
    private readonly RaidSpawnList8 Raids;

    public SAV_Raid8(SAV8SWSH sav, MaxRaidOrigin raidOrigin)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8SWSH)(Origin = sav).Clone();
        Raids = raidOrigin switch
        {
            MaxRaidOrigin.Galar => SAV.RaidGalar,
            MaxRaidOrigin.IsleOfArmor => SAV.RaidArmor,
            MaxRaidOrigin.CrownTundra => SAV.RaidCrown,
            _ => throw new ArgumentOutOfRangeException($"Raid Origin {raidOrigin} is not valid for Sword and Shield"),
        };
        CB_Den.Items.AddRange(Enumerable.Range(1, Raids.CountUsed).Select(z => (object)$"Den {z:000}").ToArray());
        CB_Den.SelectedIndex = 0;
    }

    private void LoadDen(int index) => PG_Den.SelectedObject = Raids.GetRaid(index);

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void CB_Den_SelectedIndexChanged(object sender, EventArgs e) => LoadDen(CB_Den.SelectedIndex);
}
