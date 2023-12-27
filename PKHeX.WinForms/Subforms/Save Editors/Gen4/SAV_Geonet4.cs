using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Geonet4 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV4 SAV;
    private readonly Geonet4 Geonet;

    public SAV_Geonet4(SAV4 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4)(Origin = sav).Clone();

        Geonet = new Geonet4(SAV);
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Geonet.Save();

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void SAV_Geonet4_Load(object sender, EventArgs e)
    {
        CHK_GlobalFlag.Checked = Geonet.GlobalFlag;
    }

    private void B_SetAllLocations_Click(object sender, EventArgs e)
    {
        Geonet.SetAll();
        CHK_GlobalFlag.Checked = Geonet.GlobalFlag;
    }

    private void B_SetAllLegalLocations_Click(object sender, EventArgs e)
    {
        Geonet.SetAllLegal();
        CHK_GlobalFlag.Checked = Geonet.GlobalFlag;
    }

    private void B_ClearLocations_Click(object sender, EventArgs e)
    {
        Geonet.ClearAll();
        CHK_GlobalFlag.Checked = Geonet.GlobalFlag;
    }
}
