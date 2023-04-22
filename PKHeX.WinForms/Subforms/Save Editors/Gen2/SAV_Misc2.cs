using PKHeX.Core;
using System;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class SAV_Misc2 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV2 SAV;

    public SAV_Misc2(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV2)(Origin = sav).Clone();

        switch (SAV.Version)
        {
            case GameVersion.C:
                uint valFly = SAV.FlightLocations;
                string[] flyNames = GetDestNames();
                uint[] flyValues = SAV2.GetCrystalFlyFlags();

                for (int i = 0; i < flyNames.Length; i++)
                {
                    bool state = (valFly & flyValues[i]) != 0;
                    CLB_FlyDest.Items.Add(flyNames[i], state);
                }
                break;
            default:
                GB_FlyDest.Visible = false;
                break;
        }
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveMain();

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private string[] GetDestNames()
    {
        return new string[] { "Lake of Rage", "Blackthorn City", "Silver Cave", "Violet City", "Azalea Town",
                    "Cianwood City", "Goldenrod City", "Olivine City", "Ecruteak City", "Mahogony Town", "Lavender Town", "Saffron City", "Celadon City", "Fuchsia City", "Cinnabar Island", "Indigo Plateau",
                    "New Bark Town", "Cherrygrove City", "Pallet Town", "Viridian City", "Pewter City", "Cerulean City", "Rock Tunnel", "Vermillion City"};
    }

    private void SaveMain()
    {
        if (SAV.Version == GameVersion.C)
        {
            string[] flyNames = GetDestNames();
            uint[] flyValues = SAV2.GetCrystalFlyFlags();
            uint newFlyValue = 0;

            for (int i = 0; i < flyNames.Length; i++)
            {
                if (CLB_FlyDest.CheckedItems.Contains(flyNames[i]))
                {
                    newFlyValue += flyValues[i];
                }
            }

            SAV.FlightLocations = newFlyValue;
        }
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_AllFlyDest_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            CLB_FlyDest.SetItemChecked(i, true);
    }
}
