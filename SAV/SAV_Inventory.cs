using System;
using System.Drawing;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Inventory : Form
    {
        public SAV_Inventory()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);

            item_val = getItems(Main.SAV.ORAS ? Legal.Pouch_Items_ORAS : Legal.Pouch_Items_XY);
            keyitem_val = getItems(Main.SAV.ORAS ? Legal.Pouch_Key_ORAS : Legal.Pouch_Key_XY);
            tmhm_val = getItems(Main.SAV.ORAS ? Legal.Pouch_TMHM_ORAS : Legal.Pouch_TMHM_XY, sort: false);
            medicine_val = getItems(Main.SAV.ORAS ? Legal.Pouch_Medicine_ORAS : Legal.Pouch_Medicine_XY);
            berries_val = getItems(Legal.Pouch_Berry_XY);

            B_DisplayItems.ForeColor = Color.Red;

            // Load Items
            populateList(item_val, Main.SAV.Items.HeldItem);

            B_DisplayItems.Text = Main.itempouch[0];
            B_DisplayMedicine.Text = Main.itempouch[1];
            B_DisplayTMHM.Text = Main.itempouch[2];
            B_DisplayBerries.Text = Main.itempouch[3];
            B_DisplayKeyItems.Text = Main.itempouch[4];
        }

        private readonly byte[] sav = (byte[])Main.SAV.Data.Clone();
        private readonly string[] item_val, keyitem_val, tmhm_val, medicine_val, berries_val;


        // Initialize String Tables
        private string[] getItems(ushort[] items, bool sort = true)
        {
            string[] res = new string[items.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = Main.itemlist[items[i]];
            if (sort)
                Array.Sort(res);
            return res;
        }

        // Populate DataGrid
        private void populateList(string[] itemarr, int offset, int itemcount = -1)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "CNT";
                dgvIndex.DisplayIndex = 1;
                dgvIndex.Width = 45;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvItemVal = new DataGridViewComboBoxColumn
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0,
                Width = 135,
                FlatStyle = FlatStyle.Flat
            };
            foreach (string t in itemarr)
                dgvItemVal.Items.Add(t); // add only the Item Names

            dataGridView1.Columns.Add(dgvItemVal);
            dataGridView1.Columns.Add(dgvIndex);

            dataGridView1.Rows.Add(itemcount > 0 ? itemcount : itemarr.Length);
            dataGridView1.CancelEdit();

            string itemname = "";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                int itemvalue = BitConverter.ToUInt16(sav, offset + i*4);
                try { itemname = Main.itemlist[itemvalue]; }
                catch
                {
                    Util.Error("Unknown item detected.", "Item ID: " + itemvalue, "Item is after: " + itemname);
                    continue;
                }
                int itemarrayval = Array.IndexOf(itemarr, itemname);
                if (itemarrayval == -1)
                {
                    dataGridView1.Rows[i].Cells[0].Value = itemarr[0];
                    dataGridView1.Rows[i].Cells[1].Value = 0;
                    Util.Alert(itemname + " removed from item pouch.", "If you save changes the item will no longer be in the pouch.");
                }
                else
                {
                    dataGridView1.Rows[i].Cells[0].Value = itemarr[itemarrayval];
                    dataGridView1.Rows[i].Cells[1].Value = BitConverter.ToUInt16(sav, offset + i * 4 + 2);
                }
            }
        }
        private void dropclick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0) return;

            ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
            comboBox.DroppedDown = true;
        }
        private void saveBag(object sender)
        {
            int offset = 0;
            if (B_DisplayItems.ForeColor == Color.Red)
                offset = Main.SAV.Items.HeldItem;
            else if (B_DisplayKeyItems.ForeColor == Color.Red)
                offset = Main.SAV.Items.KeyItem;
            else if (B_DisplayTMHM.ForeColor == Color.Red)
                offset = Main.SAV.Items.TMHM;
            else if (B_DisplayMedicine.ForeColor == Color.Red)
                offset = Main.SAV.Items.Medicine;
            else if (B_DisplayBerries.ForeColor == Color.Red)
                offset = Main.SAV.Items.Berry;

            // Fetch Data
            int itemcount = dataGridView1.Rows.Count;
            int emptyslots = 0;
            for (int i = 0; i < itemcount; i++)
            {
                string item = dataGridView1.Rows[i].Cells[0].Value.ToString();
                int itemindex = Array.IndexOf(Main.itemlist, item);
                int itemcnt;
                try 
                { itemcnt = Convert.ToUInt16(dataGridView1.Rows[i].Cells[1].Value.ToString()); }
                catch { itemcnt = 0; }

                if (itemindex == 0) // Compression of Empty Slots
                {
                    emptyslots++;
                    continue;
                }
                if (itemcnt == 0)
                    itemcnt++; // No 0 count of items
                else if (itemcnt > 995)
                    itemcnt = 995; // cap out

                // Write Data into Save File
                BitConverter.GetBytes((ushort)itemindex).CopyTo(sav, offset + 4 * (i - emptyslots)); // item #
                BitConverter.GetBytes((ushort)itemcnt).CopyTo(sav, offset + 4 * (i - emptyslots) + 2); // count
            }

            // Delete Empty Trash
            for (int i = itemcount - emptyslots; i < itemcount; i++)
            {
                BitConverter.GetBytes((ushort)0).CopyTo(sav, offset + 4 * i + 0); // item #
                BitConverter.GetBytes((ushort)0).CopyTo(sav, offset + 4 * i + 2); // count
            }

            // Load New Button Color, after finished we'll load the new data.
            B_DisplayItems.ForeColor =
            B_DisplayKeyItems.ForeColor =
            B_DisplayTMHM.ForeColor =
            B_DisplayMedicine.ForeColor =
            B_DisplayBerries.ForeColor = Main.defaultControlText;

            (sender as Button).ForeColor = Color.Red;
        }
        private void giveAll(string[] inarray, int count)
        {
            for (int i = 0; i < inarray.Length - 1; i++)
            {
                string itemname = inarray[i+1];
                int itemarrayval = Array.IndexOf(inarray, itemname);
                dataGridView1.Rows[i].Cells[0].Value = inarray[itemarrayval];
                dataGridView1.Rows[i].Cells[1].Value = count;
            }
        }
        private void B_DisplayItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            populateList(item_val, Main.SAV.Items.HeldItem);
            if (ModifierKeys == Keys.Alt)
                giveAll(item_val, 995);
        }
        private void B_DisplayKeyItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            populateList(keyitem_val, Main.SAV.Items.KeyItem);
            if (ModifierKeys == Keys.Alt && Util.Prompt(MessageBoxButtons.YesNo,
                $"Warning: Adding all {B_DisplayKeyItems.Text} is dangerous.", "Continue?") == DialogResult.Yes)
                giveAll(keyitem_val, 1);
        }
        private void B_DisplayTMHM_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            populateList(tmhm_val, Main.SAV.Items.TMHM);
            if (ModifierKeys == Keys.Alt && Util.Prompt(MessageBoxButtons.YesNo,
                $"Warning: Adding all {B_DisplayTMHM.Text} is dangerous.", "Continue?") == DialogResult.Yes)
                giveAll(tmhm_val, 1);
        }
        private void B_DisplayMedicine_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            populateList(medicine_val, Main.SAV.Items.Medicine);
            if (ModifierKeys == Keys.Alt)
                giveAll(medicine_val, 995);
        }
        private void B_DisplayBerries_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            populateList(berries_val, Main.SAV.Items.Berry);
            if (ModifierKeys == Keys.Alt)
                giveAll(berries_val, 995);
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            saveBag(sender);
            Array.Copy(sav, Main.SAV.Data, Main.SAV.Data.Length);
            Main.SAV.Edited = true;
            Close();
        }
    }
}
