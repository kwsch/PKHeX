using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_SealStickers8b : Form
{
    private readonly SAV8BS SAV;
    private readonly IReadOnlyList<SealSticker8b> AllItems;

    private readonly string[] ItemNames = Util.GetStringList("stickers", Main.CurrentLanguage);

    public SAV_SealStickers8b(SAV8BS sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        AllItems = SAV.SealList.ReadItems();
        Setup(AllItems.Count(z => z.Index < ItemNames.Length && ItemNames[z.Index].Length != 0));
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

        var cName = new DataGridViewTextBoxColumn
        {
            HeaderText = "Name",
            DisplayIndex = 1,
            Width = 150,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cCount = new DataGridViewTextBoxColumn
        {
            HeaderText = "Count",
            DisplayIndex = 2,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cTotal = new DataGridViewTextBoxColumn
        {
            HeaderText = "Total",
            DisplayIndex = 3,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        cIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        var cGet = new DataGridViewCheckBoxColumn
        {
            HeaderText = "Obtained",
            DisplayIndex = 4,
            Width = 65,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        dgv.Columns.Add(cIndex);
        dgv.Columns.Add(cName);
        dgv.Columns.Add(cCount);
        dgv.Columns.Add(cTotal);
        dgv.Columns.Add(cGet);
        dgv.Rows.Add(rowCount);
    }

    private void LoadItems(IReadOnlyList<SealSticker8b> items)
    {
        int rowIndex = 0;
        foreach (var item in items)
        {
            var index = item.Index;
            if ((uint)index >= ItemNames.Length || ItemNames[index].Length == 0)
                continue;
            var cells = dgv.Rows[rowIndex++].Cells;
            cells[0].Value = item.Index.ToString("000");
            cells[1].Value = ItemNames[item.Index];
            cells[2].Value = item.Count.ToString();
            cells[3].Value = item.TotalCount.ToString();
            cells[4].Value = item.IsGet;
        }
    }

    private void SaveItems(IReadOnlyList<SealSticker8b> items)
    {
        for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
        {
            var cells = dgv.Rows[rowIndex].Cells;
            var index = int.Parse(cells[0].Value?.ToString() ?? "0");
            var item = items[index];

            item.Count = int.TryParse(cells[2].Value.ToString(), out var count) ? count : 0;
            item.TotalCount = int.TryParse(cells[3].Value.ToString(), out var total) ? total : 0;
            item.IsGet = (bool)cells[4].Value;
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
            var count = int.TryParse(cells[2].Value?.ToString() ?? "0", out var c) ? c : 0;
            var total = int.TryParse(cells[3].Value?.ToString() ?? "0", out var t) ? t : 0;

            const int max = SealSticker8b.MaxValue;
            var increment = max - total;

            count += increment;
            total = max;
            cells[2].Value = count.ToString();
            cells[3].Value = total.ToString();
            cells[4].Value = true;
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
        {
            var cells = dgv.Rows[rowIndex].Cells;
            cells[2].Value = cells[3].Value = 0.ToString();
            cells[4].Value = false;
        }
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveItems(AllItems);
        SAV.SealList.WriteItems(AllItems);
        Close();
    }
}
