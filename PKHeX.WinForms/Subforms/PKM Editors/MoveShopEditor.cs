using System;
using System.ComponentModel;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class MoveShopEditor : Form
{
    private readonly IMoveShop8 Shop;
    private readonly IMoveShop8Mastery Master;
    private readonly PKM Entity;

    public MoveShopEditor(IMoveShop8 s, IMoveShop8Mastery m, PKM pk)
    {
        Shop = s;
        Master = m;
        Entity = pk;
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        Setup();
        PopulateRecords();
        LoadRecords();
    }

    private void Setup()
    {
        dgv.Rows.Clear();
        dgv.Columns.Clear();

        var cIndex = new DataGridViewTextBoxColumn
        {
            HeaderText = "Index",
            DisplayIndex = 0,
            Width = 40,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cMove = new DataGridViewTextBoxColumn
        {
            HeaderText = "Move",
            DisplayIndex = 1,
            Width = 100,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cPurchased = new DataGridViewCheckBoxColumn
        {
            HeaderText = "Purchased",
            DisplayIndex = 2,
            Width = 70,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cMastered = new DataGridViewCheckBoxColumn
        {
            HeaderText = "Mastered",
            DisplayIndex = 3,
            Width = 70,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        cIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cPurchased.SortMode = DataGridViewColumnSortMode.Programmatic;
        cMastered.SortMode = DataGridViewColumnSortMode.Programmatic;
        cPurchased.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cMastered.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        dgv.Columns.Add(cIndex);
        dgv.Columns.Add(cMove);
        dgv.Columns.Add(cPurchased);
        dgv.Columns.Add(cMastered);
    }

    // Inverted sort order for checkboxes, so that the first sort click has all True at top.
    private void ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        var column = dgv.Columns[e.ColumnIndex];
        if (column.SortMode != DataGridViewColumnSortMode.Programmatic)
            return;

        var header = column.HeaderCell;
        switch (header.SortGlyphDirection)
        {
            case SortOrder.None:
            case SortOrder.Ascending:
                dgv.Sort(column, ListSortDirection.Descending);
                header.SortGlyphDirection = SortOrder.Descending;
                break;
            case SortOrder.Descending:
                dgv.Sort(column, ListSortDirection.Ascending);
                header.SortGlyphDirection = SortOrder.Ascending;
                break;
        }
    }

    private const int Bias = 1;

    private void PopulateRecords()
    {
        var names = GameInfo.Strings.Move;
        var indexes = Shop.Permit.RecordPermitIndexes;
        dgv.Rows.Add(indexes.Length);
        for (int i = 0; i < indexes.Length; i++)
        {
            var row = dgv.Rows[i];
            row.Cells[0].Value = $"{i + Bias:00}";
            row.Cells[1].Value = names[indexes[i]];
        }
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save();
        Close();
    }

    private void LoadRecords()
    {
        for (int i = 0; i < dgv.Rows.Count; i++)
        {
            var row = dgv.Rows[i];
            var index = int.Parse((string)row.Cells[0].Value) - Bias;
            var purchased = row.Cells[2];
            var mastered = row.Cells[3];
            purchased.Value = Shop.GetPurchasedRecordFlag(index);
            mastered.Value = Master.GetMasteredRecordFlag(index);
        }
    }

    private void Save()
    {
        for (int i = 0; i < dgv.Rows.Count; i++)
        {
            var row = dgv.Rows[i];
            var index = int.Parse((string)row.Cells[0].Value) - Bias;
            var purchased = row.Cells[2];
            var mastered = row.Cells[3];
            Shop.SetPurchasedRecordFlag(index, (bool)purchased.Value);
            Master.SetMasteredRecordFlag(index, (bool)mastered.Value);
        }
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        Save();
        switch (ModifierKeys)
        {
            case Keys.Shift:
                Master.SetMoveShopFlagsAll(Entity);
                break;
            case Keys.Control:
                Shop.ClearMoveShopFlags();
                Master.SetMoveShopFlags(Entity);
                break;
            default:
                Master.SetMoveShopFlags(Entity);
                break;
        }
        Close();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        Save();
        Shop.ClearMoveShopFlags();
        LoadRecords();
        Close();
    }
}
