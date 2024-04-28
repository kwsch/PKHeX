using System;
using System.Globalization;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_SuperTrain : Form
{
    private readonly SaveFile Origin;
    private readonly SAV6 SAV;
    private readonly SuperTrainBlock STB;

    private readonly string[] trba;

    public SAV_SuperTrain(SAV6 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV6)(Origin = sav).Clone();
        trba = GameInfo.Strings.trainingbags;
        trba[0] = "---";
        STB = ((ISaveBlock6Main)SAV).SuperTrain;
        string[] stages = GameInfo.Strings.trainingstage;
        listBox1.Items.Clear();
        for (int i = 0; i < 32; i++)
            listBox1.Items.Add($"{i + 1:00} - {stages[i]}");

        Setup();
    }

    private void Setup()
    {
        dataGridView1.Rows.Clear();
        dataGridView1.Columns.Clear();
        {
            CB_Species1.InitializeBinding();
            CB_Species1.DataSource = new BindingSource(GameInfo.FilteredSources.Species, null);

            CB_Species2.InitializeBinding();
            CB_Species2.DataSource = new BindingSource(GameInfo.FilteredSources.Species, null);
        }
        listBox1.SelectedIndex = 0;
        FillTrainingBags();
    }

    private void FillTrainingBags()
    {
        DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
        {
            dgvIndex.HeaderText = "Slot";
            dgvIndex.DisplayIndex = 0;
            dgvIndex.Width = 25;
            dgvIndex.ReadOnly = true;
            dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        DataGridViewComboBoxColumn dgvBag = new()
        {
            DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
        };
        {
            foreach (string t in trba)
            {
                if (t.Length != 0)
                    dgvBag.Items.Add(t);
            }

            dgvBag.DisplayIndex = 1;
            dgvBag.Width = 135;
            dgvBag.FlatStyle = FlatStyle.Flat;
        }
        dataGridView1.Columns.Add(dgvIndex);
        dataGridView1.Columns.Add(dgvBag);

        dataGridView1.Rows.Add(12);
        for (int i = 0; i < 12; i++)
        {
            dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
            dataGridView1.Rows[i].Cells[1].Value = trba[STB.GetBag(i)];
        }
    }

    private void DropClick(object sender, DataGridViewCellEventArgs e)
    {
        try
        {
            if (e.ColumnIndex != 1)
                return;
            ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
            comboBox.DroppedDown = true;
        }
        catch { System.Diagnostics.Debug.WriteLine("Failed to modify item."); }
    }

    private bool loading = true;

    private void ChangeListRecordSelection(object sender, EventArgs e)
    {
        int index = listBox1.SelectedIndex;
        if (index < 0)
            return;

        loading = true;
        var holder1 = STB.GetHolder1(index);
        var holder2 = STB.GetHolder2(index);
        CB_Species1.SelectedValue = (int)holder1.Species;
        MTB_Gender1.Text = holder1.Gender.ToString();
        MTB_Form1.Text = holder1.Form.ToString();
        CB_Species2.SelectedValue = (int)holder2.Species;
        MTB_Gender2.Text = holder2.Gender.ToString();
        MTB_Form2.Text = holder2.Form.ToString();
        TB_Time1.Text = STB.GetTime1(index).ToString(CultureInfo.InvariantCulture);
        TB_Time2.Text = STB.GetTime2(index).ToString(CultureInfo.InvariantCulture);
        loading = false;
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        // Copy Bags
        int emptyslots = 0;
        for (int i = 0; i < 12; i++)
        {
            var bag = dataGridView1.Rows[i].Cells[1].Value.ToString();
            if (Array.IndexOf(trba, bag) == 0)
            {
                emptyslots++;
                continue;
            }
            STB.SetBag(i - emptyslots, (byte)Array.IndexOf(trba, bag));
        }

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void ChangeRecordSpecies1(object sender, EventArgs e)
    {
        int index = listBox1.SelectedIndex;
        if (index < 0 || loading)
            return;
        var holder = STB.GetHolder1(index);
        holder.Species = (ushort)WinFormsUtil.GetIndex(CB_Species1);
    }

    private void ChangeRecordMisc1(object sender, EventArgs e)
    {
        int index = listBox1.SelectedIndex;
        if (index < 0 || loading)
            return;
        var holder = STB.GetHolder1(index);
        if (byte.TryParse(MTB_Form1.Text, out var form))
            holder.Form = form;
        if (byte.TryParse(MTB_Gender1.Text, out var gender))
            holder.Gender = gender;
    }

    private void ChangeRecordSpecies2(object sender, EventArgs e)
    {
        int index = listBox1.SelectedIndex;
        if (index < 0 || loading)
            return;
        var holder = STB.GetHolder2(index);
        holder.Species = (ushort)WinFormsUtil.GetIndex(CB_Species2);
    }

    private void ChangeRecordMisc2(object sender, EventArgs e)
    {
        int index = listBox1.SelectedIndex;
        if (index < 0 || loading)
            return;
        var holder = STB.GetHolder2(index);
        if (byte.TryParse(MTB_Form2.Text, out var form))
            holder.Form = form;
        if (byte.TryParse(MTB_Gender2.Text, out var gender))
            holder.Gender = gender;
    }

    private void ChangeRecordTime1(object sender, EventArgs e)
    {
        int index = listBox1.SelectedIndex;
        if (index < 0 || loading)
            return;
        if (float.TryParse(TB_Time1.Text, out var value))
            STB.SetTime1(index, value);
    }

    private void ChangeRecordTime2(object sender, EventArgs e)
    {
        int index = listBox1.SelectedIndex;
        if (index < 0 || loading)
            return;
        if (float.TryParse(TB_Time2.Text, out var value))
            STB.SetTime2(index, value);
    }
}
