using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Underground8b : Form
{
    private readonly SAV8BS SAV;
    private readonly IReadOnlyList<UndergroundItem8b> AllItems;

    private readonly string[] ItemNames = Util.GetStringList("ug_item", Main.CurrentLanguage);

    public SAV_Underground8b(SAV8BS sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        AllItems = SAV.Underground.ReadItems();
        Setup(AllItems.Count(z => z.Type != UgItemType.None));
        LoadItems(AllItems);
    }

    private void Setup(int rowCount)
    {
        dgv.Rows.Clear();
        dgv.Columns.Clear();

        var cIndex = new DataGridViewTextBoxColumn
        {
            HeaderText = "ID",
            DisplayIndex = 0,
            Width = 40,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cType = new DataGridViewTextBoxColumn
        {
            HeaderText = "Type",
            DisplayIndex = 1,
            Width = 60,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cName = new DataGridViewTextBoxColumn
        {
            HeaderText = "Name",
            DisplayIndex = 2,
            Width = 150,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cCount = new DataGridViewTextBoxColumn
        {
            HeaderText = "Count",
            DisplayIndex = 3,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        cIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        var cNew = new DataGridViewCheckBoxColumn
        {
            HeaderText = "New",
            DisplayIndex = 4,
            Width = 35,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cFav = new DataGridViewCheckBoxColumn
        {
            HeaderText = "Favorite",
            DisplayIndex = 5,
            Width = 55,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        dgv.Columns.Add(cIndex);
        dgv.Columns.Add(cType);
        dgv.Columns.Add(cName);
        dgv.Columns.Add(cCount);
        dgv.Columns.Add(cNew);
        dgv.Columns.Add(cFav);
        dgv.Rows.Add(rowCount);
    }

    private void LoadItems(IEnumerable<UndergroundItem8b> items)
    {
        int rowIndex = 0;
        foreach (var item in items)
        {
            var type = item.Type;
            if (type == UgItemType.None)
                continue;
            var cells = dgv.Rows[rowIndex++].Cells;
            cells[0].Value = item.Index.ToString("000");
            cells[1].Value = item.Type.ToString();
            cells[2].Value = ItemNames[item.Index];
            cells[3].Value = item.Count.ToString();
            cells[4].Value = !item.HideNewFlag;
            cells[5].Value = item.IsFavoriteFlag;
        }
    }

    private void SaveItems(IReadOnlyList<UndergroundItem8b> items)
    {
        for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
        {
            var cells = dgv.Rows[rowIndex].Cells;
            var index = int.Parse(cells[0].Value?.ToString() ?? "0");
            var item = items[index];

            item.Count = int.TryParse(cells[3].Value.ToString(), out var count) ? count : 0;
            item.HideNewFlag = !(bool)cells[4].Value;
            item.IsFavoriteFlag = (bool)cells[5].Value;
        }
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
        {
            var cells = dgv.Rows[rowIndex].Cells;
            var index = int.Parse(cells[0].Value?.ToString() ?? "0");
            var item = AllItems[index];
            cells[3].Value = item.MaxValue.ToString();
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
        {
            var cells = dgv.Rows[rowIndex].Cells;
            cells[3].Value = 0.ToString();
            cells[4].Value = true;
            cells[5].Value = false;
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveItems(AllItems);
        SAV.Underground.WriteItems(AllItems);
        Close();
    }
}
