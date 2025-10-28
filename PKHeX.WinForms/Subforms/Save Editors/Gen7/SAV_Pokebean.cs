using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Pokebean : Form
{
    private readonly SaveFile Origin;
    private readonly SAV7 SAV;

    public SAV_Pokebean(SAV7 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV7)(Origin = sav).Clone();

        InitializeGrid();
        LoadValues();
    }

    private void LoadValues()
    {
        dgv.Rows.Clear();

        var names = ResortSave7.GetBeanIndexNames();
        var beans = SAV.ResortSave.GetBeans();
        dgv.Rows.Add(beans.Length);
        for (int i = 0; i < beans.Length; i++)
        {
            dgv.Rows[i].Cells[0].Value = names[i];
            dgv.Rows[i].Cells[1].Value = beans[i].ToString();
        }
    }

    private void InitializeGrid()
    {
        var dgvBean = new DataGridViewTextBoxColumn
        {
            HeaderText = "Slot",
            DisplayIndex = 0,
            Width = 135,
            ReadOnly = true,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
        };
        var dgvCount = new DataGridViewTextBoxColumn
        {
            DisplayIndex = 1,
            Width = 45,
        };

        dgv.Columns.Add(dgvBean);
        dgv.Columns.Add(dgvCount);
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        SAV.ResortSave.FillBeans();
        LoadValues();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        SAV.ResortSave.ClearBeans();
        LoadValues();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        var beans = SAV.ResortSave.GetBeans();
        for (int i = 0; i < beans.Length; i++)
        {
            var cells = dgv.Rows[i].Cells;
            var count = int.TryParse(cells[1].Value?.ToString() ?? "0", out var val) ? val : 0;
            beans[i] = (byte)Math.Min(byte.MaxValue, count);
        }
        Origin.CopyChangesFrom(SAV);
        Close();
    }
}
