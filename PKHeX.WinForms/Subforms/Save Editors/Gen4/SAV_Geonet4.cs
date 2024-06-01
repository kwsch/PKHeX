using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Geonet4 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV4 SAV;
    private readonly Geonet4 Geonet;

    private readonly List<ComboItem> countryList;
    private readonly List<ComboItem> subregionListDefault;
    private readonly List<ComboItem> pointList;

    public SAV_Geonet4(SAV4 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4)(Origin = sav).Clone();

        Geonet = new Geonet4(SAV);

        countryList = Util.GetCountryRegionList("gen4_countries", Main.CurrentLanguage);
        subregionListDefault = Util.GetCountryRegionList("gen4_sr_default", Main.CurrentLanguage);
        pointList = Util.GetGeonetPointList();
        InitializeDGVGeonet();

        CHK_GlobalFlag.Checked = Geonet.GlobalFlag;
    }

    private void InitializeDGVGeonet()
    {
        DGV_Geonet.Rows.Clear();

        Item_Point.InitializeBinding();
        Item_Point.DataSource = pointList;

        for (int i = 1; i <= Geonet4.CountryCount; i++)
        {
            var country = countryList[i].Value;
            var countryName = countryList[i].Text;
            var subregionCount = Geonet4.GetSubregionCount((byte)country);
            var subregionList = (subregionCount == 0) ? subregionListDefault : Util.GetCountryRegionList($"gen4_sr_{country:000}", Main.CurrentLanguage);
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
        var point = Geonet.GetCountrySubregion((byte)country, (byte)subregion);
        var row = DGV_Geonet.Rows[DGV_Geonet.Rows.Add()];
        row.Cells[0].Value = country;
        row.Cells[1].Value = countryName;
        row.Cells[2].Value = subregion;
        row.Cells[3].Value = subregionName;
        row.Cells[4].Value = (int)point;
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Geonet.ClearAll();
        for (int i = 0; i < DGV_Geonet.Rows.Count; i++)
        {
            var row = DGV_Geonet.Rows[i];
            var country = (int)row.Cells[0].Value;
            var subregion = (int)row.Cells[2].Value;
            var point = (GeonetPoint)row.Cells[4].Value;
            if (country > 0)
                Geonet.SetCountrySubregion((byte)country, (byte)subregion, point);
        }
        Geonet.SetSAVCountry();
        Geonet.Save();

        Geonet.GlobalFlag = CHK_GlobalFlag.Checked;
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_SetAllLocations_Click(object sender, EventArgs e)
    {
        Geonet.SetAll();
        InitializeDGVGeonet();
        CHK_GlobalFlag.Checked = Geonet.GlobalFlag;
    }

    private void B_SetAllLegalLocations_Click(object sender, EventArgs e)
    {
        Geonet.SetAllLegal();
        InitializeDGVGeonet();
        CHK_GlobalFlag.Checked = Geonet.GlobalFlag;
    }

    private void B_ClearLocations_Click(object sender, EventArgs e)
    {
        Geonet.ClearAll();
        InitializeDGVGeonet();
        CHK_GlobalFlag.Checked = Geonet.GlobalFlag;
    }
}
