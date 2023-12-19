using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Pokepuff : Form
{
    private readonly ISaveBlock6Main SAV;

    public SAV_Pokepuff(ISaveBlock6Main sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        var puffs = SAV.Puff.GetPuffs();
        Setup(puffs.Length);
        LoadPuffs(puffs);

        toolTip1.SetToolTip(B_Sort, "Hold CTRL to reverse sort.");
        toolTip2.SetToolTip(B_All, "Hold CTRL to best instead of varied.");
    }

    private readonly string[] pfa = GameInfo.Strings.puffs;

    private void Setup(int rowCount)
    {
        dgv.Rows.Clear();
        dgv.Columns.Clear();

        DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
        {
            dgvIndex.HeaderText = "Slot";
            dgvIndex.DisplayIndex = 0;
            dgvIndex.Width = 45;
            dgvIndex.ReadOnly = true;
            dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        DataGridViewComboBoxColumn dgvPuff = new()
        {
            DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
        };
        {
            foreach (string t in pfa)
                dgvPuff.Items.Add(t);

            dgvPuff.DisplayIndex = 1;
            dgvPuff.Width = 135;
            dgvPuff.FlatStyle = FlatStyle.Flat;
        }
        dgv.Columns.Add(dgvIndex);
        dgv.Columns.Add(dgvPuff);
        dgv.Rows.Add(rowCount);
    }

    private void LoadPuffs(ReadOnlySpan<byte> Puffs)
    {
        for (int i = 0; i < Puffs.Length; i++)
        {
            dgv.Rows[i].Cells[0].Value = (i + 1).ToString();
            int puffval = Puffs[i];
            if (puffval >= pfa.Length)
            {
                WinFormsUtil.Error($"Invalid Puff Index: {i}", $"Expected < ${pfa.Length}");
                puffval = 0;
            }
            dgv.Rows[i].Cells[1].Value = pfa[puffval];
        }
    }

    private void DropClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex != 1)
            return;
        ((ComboBox)((DataGridView)sender).EditingControl).DroppedDown = true;
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        SAV.Puff.MaxCheat(ModifierKeys == Keys.Control);
        LoadPuffs(SAV.Puff.GetPuffs());
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        SAV.Puff.Reset();
        LoadPuffs(SAV.Puff.GetPuffs());
    }

    private void B_Sort_Click(object sender, EventArgs e)
    {
        SAV.Puff.Sort(ModifierKeys == Keys.Control);
        LoadPuffs(SAV.Puff.GetPuffs());
    }

    private byte[] GetPuffs()
    {
        var count = dgv.Rows.Count;
        var puffs = new byte[count];
        for (int i = 0; i < dgv.Rows.Count; i++)
        {
            var puff = dgv.Rows[i].Cells[1].Value?.ToString();
            int index = (byte)Array.IndexOf(pfa, puff);
            puffs[i] = (byte)index;
        }
        return puffs;
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        var puffs = GetPuffs();
        SAV.Puff.SetPuffs(puffs);
        SAV.Puff.PuffCount = puffs.Length;
        Close();
    }
}
