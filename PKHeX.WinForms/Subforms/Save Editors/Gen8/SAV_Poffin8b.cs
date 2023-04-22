using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Poffin8b : Form
{
    private readonly SAV8BS SAV;
    private readonly IReadOnlyList<Poffin8b> AllItems;

    private readonly string[] ItemNames = Util.GetStringList("poffin8b", Main.CurrentLanguage);

    public SAV_Poffin8b(SAV8BS sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        ItemNames[0] = GameInfo.Strings.Item[0];
        AllItems = SAV.Poffins.GetPoffins();
        Setup(PoffinSaveData8b.COUNT_POFFIN);
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

        var cType = new DataGridViewComboBoxColumn
        {
            HeaderText = "Type",
            DisplayIndex = 1,
            Width = 120,
            SortMode = DataGridViewColumnSortMode.Automatic,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
            FlatStyle = FlatStyle.Flat,
        };

        var cLevel = new DataGridViewTextBoxColumn
        {
            HeaderText = "Level",
            DisplayIndex = 2,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cTaste = new DataGridViewTextBoxColumn
        {
            HeaderText = "Smooth",
            DisplayIndex = 3,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cNew = new DataGridViewCheckBoxColumn
        {
            HeaderText = "New",
            DisplayIndex = 4,
            Width = 35,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cF1 = new DataGridViewTextBoxColumn
        {
            HeaderText = "Spicy",
            DisplayIndex = 5,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cF2 = new DataGridViewTextBoxColumn
        {
            HeaderText = "Dry",
            DisplayIndex = 6,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cF3 = new DataGridViewTextBoxColumn
        {
            HeaderText = "Sweet",
            DisplayIndex = 7,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cF4 = new DataGridViewTextBoxColumn
        {
            HeaderText = "Bitter",
            DisplayIndex = 8,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        var cF5 = new DataGridViewTextBoxColumn
        {
            HeaderText = "Sour",
            DisplayIndex = 9,
            Width = 50,
            SortMode = DataGridViewColumnSortMode.Automatic,
        };

        cIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cLevel.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cTaste.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        cF1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cF2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cF3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cF4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        cF5.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        dgv.Columns.Add(cIndex);
        dgv.Columns.Add(cType);
        dgv.Columns.Add(cLevel);
        dgv.Columns.Add(cTaste);
        dgv.Columns.Add(cNew);
        dgv.Columns.Add(cF1);
        dgv.Columns.Add(cF2);
        dgv.Columns.Add(cF3);
        dgv.Columns.Add(cF4);
        dgv.Columns.Add(cF5);

        cType.Items.AddRange(ItemNames);

        dgv.Rows.Add(rowCount);
    }

    private void LoadItems(IReadOnlyList<Poffin8b> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var cells = dgv.Rows[i].Cells;
            cells[0].Value = i.ToString("000");
            cells[1].Value = GetPoffinName(item.MstID);
            cells[2].Value = item.Level.ToString();
            cells[3].Value = item.Taste.ToString();
            cells[4].Value = item.IsNew;
            cells[5].Value = item.FlavorSpicy.ToString();
            cells[6].Value = item.FlavorDry.ToString();
            cells[7].Value = item.FlavorSweet.ToString();
            cells[8].Value = item.FlavorBitter.ToString();
            cells[9].Value = item.FlavorSour.ToString();
        }
    }

    private string GetPoffinName(byte itemMstId)
    {
        var index = (int)(byte)(itemMstId + 1);
        var strings = ItemNames;
        if ((uint)index >= strings.Length)
            index = 0;
        return ItemNames[index];
    }

    private byte SetPoffinName(string value)
    {
        var index = Array.IndexOf(ItemNames, value);
        if (index != -1)
            return (byte)(index - 1);
        return unchecked((byte)-1);
    }

    private void SaveItems(IReadOnlyList<Poffin8b> items)
    {
        for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
        {
            var cells = dgv.Rows[rowIndex].Cells;
            var index = int.Parse(cells[0].Value?.ToString() ?? "0");
            var item = items[index];

            item.MstID = SetPoffinName(cells[1].Value?.ToString() ?? "");
            item.Level = Parse(cells[2]);
            item.Taste = Parse(cells[3]);
            item.IsNew = (bool)cells[4].Value;
            item.FlavorSpicy = Parse(cells[5]);
            item.FlavorDry = Parse(cells[6]);
            item.FlavorSweet = Parse(cells[7]);
            item.FlavorBitter = Parse(cells[8]);
            item.FlavorSour = Parse(cells[9]);

            static byte Parse(DataGridViewCell c) => (byte)(byte.TryParse(c.Value.ToString(), out var p) ? p : 0);
        }
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        foreach (var poffin in AllItems)
        {
            poffin.MstID = 0x1C;
            poffin.Level = 60;
            poffin.Taste = 0xFF;
            poffin.FlavorSpicy = poffin.FlavorBitter = poffin.FlavorDry = poffin.FlavorSour = poffin.FlavorSweet = 0xFF;
        }
        LoadItems(AllItems);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        foreach (var poffin in AllItems)
            poffin.ToNull();
        LoadItems(AllItems);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveItems(AllItems);
        SAV.Poffins.SetPoffins(AllItems);
        Close();
    }
}
