using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_SuperTrain : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6 SAV;
        private readonly SuperTrainBlock STB;

        public SAV_SuperTrain(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV6)(Origin = sav).Clone();
            trba = GameInfo.Strings.trainingbags;
            trba[0] = "---";
            STB = ((ISaveBlock6Main) SAV).SuperTrain;
            var ofs = STB.Offset;
            offsetTime = ofs + 0x08;
            offsetSpec = ofs + 0x188;
            offsetVal = ofs + 0x18A;
            string[] stages = GameInfo.Strings.trainingstage;
            listBox1.Items.Clear();
            for (int i = 0; i < 32; i++)
                listBox1.Items.Add($"{i+1:00} - {stages[i]}");

            Setup();
        }

        private readonly string[] trba;
        private readonly int offsetVal;
        private readonly int offsetTime;
        private readonly int offsetSpec;

        private void Setup()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            {
                CB_Species.InitializeBinding();
                CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList(), null);

                CB_S2.InitializeBinding();
                CB_S2.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList(), null);
            }
            listBox1.SelectedIndex = 0;
            FillTrainingBags();

            CB_S2.SelectedValue = (int)BitConverter.ToUInt16(SAV.Data, offsetSpec + (4 * 30));
            TB_Time1.Text = BitConverter.ToSingle(SAV.Data, offsetTime + (4 * 30)).ToString(CultureInfo.InvariantCulture);
            TB_Time2.Text = BitConverter.ToSingle(SAV.Data, offsetTime + (4 * 31)).ToString(CultureInfo.InvariantCulture);
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
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            {
                foreach (string t in trba)
                {
                    if (t.Length > 0)
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch { Console.WriteLine("Failed to modify item."); }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private bool loading = true;

        private void ChangeListRecordSelection(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0)
                return;
            loading = true;
            TB_Time.Text = BitConverter.ToSingle(SAV.Data, offsetTime + (4 * index)).ToString(CultureInfo.InvariantCulture);
            TB_Unk.Text = BitConverter.ToUInt16(SAV.Data, offsetVal + (4 * index)).ToString();
            CB_Species.SelectedValue = (int)BitConverter.ToUInt16(SAV.Data, offsetSpec + (4 * index));
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

            if (float.TryParse(TB_Time1.Text, out var t1))
                SAV.SetData(BitConverter.GetBytes(t1), offsetTime + (4 * 30));
            if (float.TryParse(TB_Time2.Text, out var t2))
                SAV.SetData(BitConverter.GetBytes(t2), offsetTime + (4 * 31));
            SAV.SetData(BitConverter.GetBytes((ushort)WinFormsUtil.GetIndex(CB_S2)), offsetSpec + (4 * 30));

            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChangeRecordSpecies(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0 || loading)
                return;
            SAV.SetData(BitConverter.GetBytes(WinFormsUtil.GetIndex(CB_Species)), offsetSpec + (4 * index));
        }

        private void ChangeRecordVal(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0 || loading)
                return;
            if (ushort.TryParse(TB_Unk.Text, out var val))
                SAV.SetData(BitConverter.GetBytes(val), offsetVal + (4 * index));
        }

        private void ChangeRecordTime(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0 || loading)
                return;
            if (float.TryParse(TB_Time.Text, out var val))
                SAV.SetData(BitConverter.GetBytes(val), offsetTime + (4 * index));
        }
    }
}

