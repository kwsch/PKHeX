using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
                uint[] flyValues = GetCrystalFlyFlags();
                uint valFly = SAV.FlightLocations;
                List<string> destNames = GetDestNames(GetCrystalDestValues());

                for(int i = 0;i < destNames.Count; i++)
                {
                    bool state = (valFly & flyValues[i]) != 0;
                    CLB_FlyDest.Items.Add(destNames[i], state);
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

    private List<string> GetDestNames(int[] destValues)
    {
        List<string> destNames = new List<string>();

        IReadOnlyList<ComboItem> metLocationList = GameInfo.GetLocationList(GameVersion.C, EntityContext.Gen2, false);

        for (int i = 0; i < destValues.Length; i++)
        {
            int destValue = destValues[i];
            string destName = metLocationList.First(ml => ml.Value == destValue).Text;
            destNames.Add(destName);
        }

        return destNames;
    }

    private int[] GetCrystalDestValues()
    {
        return new int[] {
            38,     //Lake of Rage,
            41,     //Blackthorn City,
            46,     //Silver Cave,
            6,      //Violet City,
            12,     //Azalea Town,
            33,     //Cianwood City,
            16,     //Goldenrod City,
            27,     //Olivine City,
            22,     //Ecruteak City,
            36,     //Mahogony Town,
            69,     //Lavender Town,
            72,     //Saffron City,
            71,     //Celadon City,
            81,     //Fuchsia City,
            85,     //Cinnabar Island,
            90,     //Indigo Plateau,
            1,      //New Bark Town,
            3,      //Cherrygrove City,
            47,     //Pallet Town,
            49,     //Viridian City,
            51,     //Pewter City,
            55,     //Cerulean City,
            66,     //Rock Tunnel,
            61,     //Vermillion City
        };
    }

    private uint[] GetCrystalFlyFlags()
    {
        return new uint[] {
            0x1,            //Lake of Rage
            0x2,            //Blackthorn City
            0x4,            //Silver Cave
            0x100,          //Violet City
            0x400,          //Azalea Town
            0x800,          //Cianwood City
            0x1000,         //Goldenrod City
            0x2000,         //Olivine City
            0x4000,         //Ecruteak City
            0x8000,         //Mahogony Town
            0x10000,        //Lavender Town
            0x20000,        //Saffron City
            0x40000,        //Celadon City
            0x80000,        //Fuchsia City
            0x100000,       //Cinnabar Island
            0x200000,       //Indigo Plateau
            0x400000,       //New Bark Town
            0x800000,       //Cherrygrove City
            0x4000000,      //Pallet Town
            0x8000000,      //Viridian City
            0x10000000,     //Pewter City
            0x20000000,     //Cerulean City
            0x40000000,     //Rock Tunnel
            0x80000000      //Vermillion City
        };
    }

    private void SaveMain()
    {
        if (SAV.Version == GameVersion.C)
        {
            List<string> flyNames = GetDestNames(GetCrystalDestValues());
            uint[] flyValues = GetCrystalFlyFlags();
            uint newFlyValue = 0;

            for (int i = 0; i < flyNames.Count; i++)
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
