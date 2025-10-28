using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_RaidSevenStar9 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV9SV SAV;
    private readonly RaidSevenStar9 Raids;

    public SAV_RaidSevenStar9(SAV9SV sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();
        Raids = SAV.RaidSevenStar;
        CB_Raid.Items.AddRange(Enumerable.Range(1, Raids.CountAll).Select(z => (object)$"Raid {z:0000}").ToArray());
        CB_Raid.SelectedIndex = 0;
    }

    private void LoadRaid(int index) => PG_Raid.SelectedObject = Raids.GetRaid(index);

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        ValidateChildren();
        Validate();
        Close();
    }

    private void CB_Raid_SelectedIndexChanged(object sender, EventArgs e) => LoadRaid(CB_Raid.SelectedIndex);
}
