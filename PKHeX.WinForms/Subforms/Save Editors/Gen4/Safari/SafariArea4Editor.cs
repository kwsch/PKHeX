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

    private readonly string[] ObjectNames = GameInfo.Strings.safariblocks;

    public SafariArea4Editor(SafariZoneArea4 area)
    {
        Area = area;
        InitializeComponent();
        DGV_Objects.RowCount = 30;
        NUD_Count.Enabled = !CHK_Auto.Checked;

        var zoneNames = WinFormsTranslator.GetEnumTranslation<SafariZoneType4>(Main.CurrentLanguage);
        CB_Map.Items.AddRange(zoneNames);
        DGV_Type.DataSource = new BindingSource(ObjectNames, string.Empty);
    }

    public bool IsAutoCount => CHK_Auto.Checked;

    public void LoadData()
    {
        IsLoading = true;
        CB_Map.SelectedIndex = Math.Min(CB_Map.Items.Count - 1, (int)Area.Map);
        LoadObjects();
        IsLoading = false;
        RefreshView();
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
            row[2].Value = obj.Z.ToString();
            row[3].Value = obj.Y.ToString();
        }
    }

    private void SaveObjects()
    {
        for (int i = 0; i < Area.Objects.Length; i++)
            Area.Objects[i] = ReadRow(i);
    }

    private SafariZoneObject4 ReadRow(int index)
    {
        var row = DGV_Objects.Rows[index].Cells;
        return new SafariZoneObject4
        {
            Type = ParseType(row[0].Value),
            X = ParseByte(row[1].Value),
            Z = ParseByte(row[2].Value),
            Y = ParseByte(row[3].Value),
        };
    }

    /// <summary> Pushes the whole grid to the visualizer; used after bulk changes. </summary>
    private void RefreshView()
    {
        Span<SafariZoneObject4> objects = stackalloc SafariZoneObject4[DGV_Objects.RowCount];
        for (int i = 0; i < objects.Length; i++)
            objects[i] = ReadRow(i);
        View_Map.SetMap((SafariZoneType4)Math.Max(0, CB_Map.SelectedIndex));
        View_Map.SetObjects(objects);
        UpdateAutoCount(objects);
        ColorRows();
    }

    private void UpdateAutoCount(ReadOnlySpan<SafariZoneObject4> objects)
    {
        if (!CHK_Auto.Checked)
            return;
        int count = 0;
        foreach (var obj in objects)
        {
            if (!obj.IsEmpty)
                count++;
        }
        NUD_Count.Value = Math.Min(NUD_Count.Maximum, count);
    }

    private void CHK_Auto_CheckedChanged(object? sender, EventArgs e)
    {
        NUD_Count.Enabled = !CHK_Auto.Checked;
        if (!IsLoading)
            RefreshView();
    }

    private void CB_Map_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!IsLoading)
            View_Map.SetMap((SafariZoneType4)Math.Max(0, CB_Map.SelectedIndex));
    }

    private void DGV_Objects_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        // Combo box edits only commit on leaving the cell otherwise.
        if (DGV_Objects.IsCurrentCellDirty && DGV_Objects.CurrentCell is DataGridViewComboBoxCell)
            DGV_Objects.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void DGV_Objects_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        // Bulk loads set every cell; they refresh the view once afterward.
        if (IsLoading || e.RowIndex < 0)
            return;
        View_Map.SetObject(e.RowIndex, ReadRow(e.RowIndex));
        if (CHK_Auto.Checked)
        {
            RefreshAutoCount();
            ColorRows();
        }
    }

    private void RefreshAutoCount()
    {
        Span<SafariZoneObject4> objects = stackalloc SafariZoneObject4[DGV_Objects.RowCount];
        for (int i = 0; i < objects.Length; i++)
            objects[i] = ReadRow(i);
        UpdateAutoCount(objects);
    }

    private void View_Map_EmptyTileClicked(int x, int z, int y)
    {
        // Claim the first blank row for the clicked tile; the user fills in the rest.
        for (int i = 0; i < DGV_Objects.RowCount; i++)
        {
            if (!ReadRow(i).IsEmpty)
                continue;

            var row = DGV_Objects.Rows[i].Cells;
            IsLoading = true;
            row[1].Value = x.ToString();
            row[2].Value = z.ToString();
            row[3].Value = y.ToString();
            IsLoading = false;
            RefreshView();
            SelectRow(i);
            return;
        }
        System.Media.SystemSounds.Beep.Play();
    }

    private void View_Map_ObjectClicked(int index)
    {
        if ((uint)index >= DGV_Objects.RowCount)
            return;
        SelectRow(index);
    }

    private void View_Map_ObjectDeleteRequested(int index)
    {
        if ((uint)index >= DGV_Objects.RowCount)
            return;
        var cells = DGV_Objects.Rows[index].Cells;
        IsLoading = true;
        cells[0].Value = ObjectNames[0];
        cells[1].Value = "0";
        cells[2].Value = "0";
        cells[3].Value = "0";
        IsLoading = false;
        RefreshView();
        RefreshAutoCount();
        SelectRow(index);
    }

    private void SelectRow(int index)
    {
        DGV_Objects.ClearSelection();
        DGV_Objects.CurrentCell = DGV_Objects.Rows[index].Cells[0];
        DGV_Objects.Rows[index].Selected = true;
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
        IsLoading = true;
        LoadObjects();
        IsLoading = false;
        RefreshView();
        WinFormsUtil.Asterisk();
    }

    private void NUD_Count_ValueChanged(object sender, EventArgs e)
    {
        ColorRows();
    }

    /// <summary> Shades the rows in use: non-default rows when auto counting, otherwise the first <see cref="NUD_Count"/> rows. </summary>
    private void ColorRows()
    {
        var auto = CHK_Auto.Checked;
        var count = (int)NUD_Count.Value;
        for (int i = 0; i < DGV_Objects.Rows.Count; i++)
        {
            var used = auto ? !ReadRow(i).IsEmpty : i < count;
            DGV_Objects.Rows[i].DefaultCellStyle.BackColor = used ? SystemColors.Control : SystemColors.Window;
        }
    }
}
