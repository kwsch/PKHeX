using PKHeX.Core;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public sealed partial class SafariArea4Editor : UserControl
{
    private readonly SafariZoneArea4 Area;
    private bool IsLoading;

    private readonly string[] ObjectNames = WinFormsTranslator.GetEnumTranslation<SafariZoneObjectType4>(Main.CurrentLanguage);

    public SafariArea4Editor(SafariZoneArea4 area)
    {
        Area = area;
        InitializeComponent();
        DGV_Objects.RowCount = 30;

        var zoneNames = WinFormsTranslator.GetEnumTranslation<SafariZoneType4>(Main.CurrentLanguage);
        CB_Map.Items.AddRange(zoneNames);
        DGV_Type.DataSource = new BindingSource(ObjectNames, string.Empty);
    }

    public void LoadData()
    {
        IsLoading = true;
        CB_Map.SelectedIndex = Math.Min(CB_Map.Items.Count - 1, (int)Area.Map);
        LoadObjects();
        IsLoading = false;
    }

    public void SaveData()
    {
        if (IsLoading)
            return;

        Area.Map = (SafariZoneType4)CB_Map.SelectedIndex;
        Area.Count = (byte)NUD_Count.Value;
        SaveObjects();
    }

    private void LoadObjects()
    {
        NUD_Count.Value = Math.Min(NUD_Count.Maximum, Area.Count);
        for (int i = 0; i < Area.Objects.Length; i++)
        {
            var obj = Area.Objects[i];
            var row = DGV_Objects.Rows[i].Cells;
            row[0].Value = ObjectNames[Math.Min((int)obj.Type, (ObjectNames.Length - 1))];
            row[1].Value = obj.X.ToString();
            row[2].Value = obj.Y.ToString();
            row[3].Value = obj.Z.ToString();
        }
    }

    private void SaveObjects()
    {
        for (int i = 0; i < Area.Objects.Length; i++)
        {
            var row = DGV_Objects.Rows[i].Cells;
            Area.Objects[i] = new SafariZoneObject4
            {
                Type = ParseType(row[0].Value),
                X = ParseByte(row[1].Value),
                Y = ParseByte(row[2].Value),
                Z = ParseByte(row[3].Value),
            };
        }
    }

    private const string Filter = "Safari Area Binary (*.bin)|*.bin|All Files|*.*";

    private void B_Import_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog();
        dlg.Filter = Filter;
        dlg.FileName = CB_Map.Text;
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var fi = new FileInfo(dlg.FileName);
            if (fi.Length != SafariZoneArea4.SIZE)
            {
                WinFormsUtil.Error("Selected file is not the correct size for a Safari Zone area.");
                return;
            }
            var data = File.ReadAllBytes(dlg.FileName);
            Area.Read(data);
            LoadData();
            WinFormsUtil.Asterisk();
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error("Unable to import Safari Zone area.", ex);
        }
    }

    private void B_Export_Click(object? sender, EventArgs e)
    {
        SaveData();
        using var dlg = new SaveFileDialog();
        dlg.Filter = Filter;
        dlg.FileName = CB_Map.Text;
        dlg.DefaultExt = "bin";
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var data = new byte[SafariZoneArea4.SIZE];
            Area.Write(data, CHK_Auto.Checked);
            File.WriteAllBytes(dlg.FileName, data);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error("Unable to export Safari Zone area.", ex);
        }
    }

    private SafariZoneObjectType4 ParseType(object? value)
    {
        if (value is string s && ObjectNames.IndexOf(s) is var index and >= 0)
            return (SafariZoneObjectType4)index;
        return default;
    }

    private static byte ParseByte(object? value)
    {
        if (value is string s && byte.TryParse(s, out var result))
            return result;
        if (value is byte b)
            return b;
        if (value is int i)
            return (byte)Math.Clamp(i, 0, byte.MaxValue);
        return 0;
    }

    private void B_ClearAll_Click(object sender, EventArgs e)
    {
        // Wipe all objects
        Area.ClearObjects();
        LoadObjects();
        WinFormsUtil.Asterisk();
    }

    private void NUD_Count_ValueChanged(object sender, EventArgs e)
    {
        // Color the rows based on the count.
        var count = (int)NUD_Count.Value;
        for (int i = 0; i < DGV_Objects.Rows.Count; i++)
        {
            var row = DGV_Objects.Rows[i];
            row.DefaultCellStyle.BackColor = i < count ? SystemColors.Control : SystemColors.Window;
        }   
    }
}
