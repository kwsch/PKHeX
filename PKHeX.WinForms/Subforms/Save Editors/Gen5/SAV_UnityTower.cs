using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_UnityTower : Form
{
    private readonly SaveFile Origin;
    private readonly SAV5 SAV;
    private readonly UnityTower5 UnityTower;

    private readonly List<ComboItem> countryList;
    private readonly List<ComboItem> subregionListDefault;
    private readonly List<ComboItem> pointList;

    public SAV_UnityTower(SAV5 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV5)(Origin = sav).Clone();

        UnityTower = SAV.UnityTower;

        countryList = Util.GetCountryRegionList("gen5_countries", Main.CurrentLanguage);
        subregionListDefault = Util.GetCountryRegionList("gen5_sr_default", Main.CurrentLanguage);
        pointList = Util.GetGeonetPointList();
        InitializeDGVGeonet();
        InitializeDGVUnityTower();

        CHK_GlobalFlag.Checked = UnityTower.GlobalFlag;
        CHK_UnityTowerFlag.Checked = UnityTower.UnityTowerFlag;
    }

    private void InitializeDGVGeonet()
    {
        DGV_Geonet.Rows.Clear();

        Item_Point.InitializeBinding();
        Item_Point.DataSource = pointList;

        for (int i = 1; i <= UnityTower5.CountryCount; i++)
        {
            var country = countryList[i].Value;
            var countryName = countryList[i].Text;
            var subregionCount = UnityTower5.GetSubregionCount((byte)country);
            var subregionList = (subregionCount == 0) ? subregionListDefault : Util.GetCountryRegionList($"gen5_sr_{country:000}", Main.CurrentLanguage);
            if (subregionCount == 0)
            {
                var subregion = subregionList[0].Value;
                var subregionName = subregionList[0].Text;
                AddCountrySubregionRowDGV(country, subregion, countryName, subregionName);
            }
            for (int j = 1; j <= subregionCount; j++)
            {
                var subregion = subregionList[j].Value;
                var subregionName = subregionList[j].Text;
                AddCountrySubregionRowDGV(country, subregion, countryName, subregionName);
            }
        }
    }

    private void AddCountrySubregionRowDGV(int country, int subregion, string countryName, string subregionName)
    {
        var point = UnityTower.GetCountrySubregion((byte)country, (byte)subregion);
        var row = DGV_Geonet.Rows[DGV_Geonet.Rows.Add()];
        row.Cells[0].Value = country;
        row.Cells[1].Value = countryName;
        row.Cells[2].Value = subregion;
        row.Cells[3].Value = subregionName;
        row.Cells[4].Value = (int)point;
    }

    private void InitializeDGVUnityTower()
    {
        DGV_UnityTower.Rows.Clear();

        DGV_UnityTower.Rows.Add(UnityTower5.CountryCount);
        for (int i = 0; i < UnityTower5.CountryCount; i++)
        {
            var row = DGV_UnityTower.Rows[i];
            var country = countryList[i + 1].Value;
            var countryName = countryList[i + 1].Text;
            ((DataGridViewCheckBoxCell)row.Cells[0]).Value = UnityTower.GetUnityTowerFloor((byte)country);
            row.Cells[1].Value = country;
            row.Cells[2].Value = countryName;
        }
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        UnityTower.ClearAll();
        for (int i = 0; i < DGV_Geonet.Rows.Count; i++)
        {
            var row = DGV_Geonet.Rows[i];
            var country = (int)row.Cells[0].Value;
            var subregion = (int)row.Cells[2].Value;
            var point = (GeonetPoint)row.Cells[4].Value;
            if (country > 0)
                UnityTower.SetCountrySubregion((byte)country, (byte)subregion, point);
        }
        for (int i = 0; i < DGV_UnityTower.Rows.Count; i++)
        {
            var row = DGV_UnityTower.Rows[i];
            var unlocked = (bool)row.Cells[0].Value;
            var country = (int)row.Cells[1].Value;
            UnityTower.SetUnityTowerFloor((byte)country, unlocked);
        }
        UnityTower.SetSAVCountry();

        UnityTower.GlobalFlag = CHK_GlobalFlag.Checked;
        UnityTower.UnityTowerFlag = CHK_UnityTowerFlag.Checked;
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_SetAllLocations_Click(object sender, EventArgs e)
    {
        UnityTower.SetAll();
        InitializeDGVGeonet();
        InitializeDGVUnityTower();
        CHK_GlobalFlag.Checked = UnityTower.GlobalFlag;
        CHK_UnityTowerFlag.Checked = UnityTower.UnityTowerFlag;
    }

    private void B_SetAllLegalLocations_Click(object sender, EventArgs e)
    {
        UnityTower.SetAllLegal();
        InitializeDGVGeonet();
        InitializeDGVUnityTower();
        CHK_GlobalFlag.Checked = UnityTower.GlobalFlag;
        CHK_UnityTowerFlag.Checked = UnityTower.UnityTowerFlag;
    }

    private void B_ClearLocations_Click(object sender, EventArgs e)
    {
        UnityTower.ClearAll();
        InitializeDGVGeonet();
        InitializeDGVUnityTower();
        CHK_GlobalFlag.Checked = UnityTower.GlobalFlag;
        CHK_UnityTowerFlag.Checked = UnityTower.UnityTowerFlag;
    }
}
