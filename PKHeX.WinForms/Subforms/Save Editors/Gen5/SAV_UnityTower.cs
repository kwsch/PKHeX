using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_UnityTower : Form
{
    private readonly SaveFile Origin;
    private readonly SAV5 SAV;
    private readonly UnityTower5 UnityTower;

    public SAV_UnityTower(SAV5 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV5)(Origin = sav).Clone();

        UnityTower = SAV.UnityTower;
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        UnityTower.GlobalFlag = CHK_GlobalFlag.Checked;
        UnityTower.UnityTowerFlag = CHK_UnityTowerFlag.Checked;
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void SAV_UnityTower_Load(object sender, EventArgs e)
    {
        CHK_GlobalFlag.Checked = UnityTower.GlobalFlag;
        CHK_UnityTowerFlag.Checked = UnityTower.UnityTowerFlag;
    }

    private void B_SetAllLocations_Click(object sender, EventArgs e)
    {
        UnityTower.SetAll();
        CHK_GlobalFlag.Checked = UnityTower.GlobalFlag;
        CHK_UnityTowerFlag.Checked = UnityTower.UnityTowerFlag;
    }

    private void B_SetAllLegalLocations_Click(object sender, EventArgs e)
    {
        UnityTower.SetAllLegal();
        CHK_GlobalFlag.Checked = UnityTower.GlobalFlag;
        CHK_UnityTowerFlag.Checked = UnityTower.UnityTowerFlag;
    }

    private void B_ClearLocations_Click(object sender, EventArgs e)
    {
        UnityTower.ClearAll();
        CHK_GlobalFlag.Checked = UnityTower.GlobalFlag;
        CHK_UnityTowerFlag.Checked = UnityTower.UnityTowerFlag;
    }
}
