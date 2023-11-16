using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Apricorn : Form
{
    public SAV_Apricorn(SAV4HGSS sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4HGSS)(Origin = sav).Clone();

        Setup();
    }

    private readonly SAV4HGSS Origin;
    private readonly SAV4HGSS SAV;
    private const int Count = 7;
    private static readonly string[] itemlist = ["Red", "Yellow", "Blue", "Green", "Pink", "White", "Black"];

    private void Setup()
    {
        dgv.Rows.Clear();
        dgv.Columns.Clear();

        DataGridViewColumn dgvApricorn = new DataGridViewTextBoxColumn();
        {
            dgvApricorn.HeaderText = "Slot";
            dgvApricorn.DisplayIndex = 0;
            dgvApricorn.Width = 135;
            dgvApricorn.ReadOnly = true;
            dgvApricorn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        DataGridViewTextBoxColumn dgvCount = new()
        {
            DisplayIndex = 1,
            Width = 45,
        };
        dgv.Columns.Add(dgvApricorn);
        dgv.Columns.Add(dgvCount);

        dgv.Rows.Add(Count);
        for (int i = 0; i < Count; i++)
            dgv.Rows[i].Cells[0].Value = itemlist[i];
        LoadCount();
    }

    private void LoadCount()
    {
        for (int i = 0; i < Count; i++)
            dgv.Rows[i].Cells[1].Value = SAV.GetApricornCount(i).ToString();
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < Count; i++)
            SAV.SetApricornCount(i, 99);
        LoadCount();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < Count; i++)
            SAV.SetApricornCount(i, 0);
        LoadCount();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < Count; i++)
        {
            var cells = dgv.Rows[i].Cells;
            var count = int.TryParse(cells[1].Value?.ToString() ?? "0", out var val) ? val : 0;
            SAV.SetApricornCount(i, Math.Min(byte.MaxValue, count));
        }
        Origin.CopyChangesFrom(SAV);
        Close();
    }
}
