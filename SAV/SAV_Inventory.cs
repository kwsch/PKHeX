using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Inventory : Form
    {
        public SAV_Inventory(Form1 frm1)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            shiftval = savindex * 0x7F000;
            if (m_parent.savegame_oras)
            {
                bagoffsets = new int[] 
                {
                    0x05800,
                    0x05E40,
                    0x05FC0,
                    0x06170,
                    0x06270, 
                };
            }

            getListItems();
            getListKeyItems();
            getListTMHM();
            getListMedicine();
            getListBerries();

            B_DisplayItems.ForeColor = Color.Red;

            popItems();

            B_DisplayItems.Text = Form1.itempouch[0];
            B_DisplayMedicine.Text = Form1.itempouch[1];
            B_DisplayTMHM.Text = Form1.itempouch[2];
            B_DisplayBerries.Text = Form1.itempouch[3];
            B_DisplayKeyItems.Text = Form1.itempouch[4];
        }
        Form1 m_parent;
        public byte[] sav = new byte[0x100000];
        public int savindex; int shiftval;
        public bool editing = false;

        public string[] item_val;
        public string[] keyitem_val;
        public string[] tmhm_val;
        public string[] medicine_val;
        public string[] berries_val;

        public int[] bagoffsets = new int[] {
                                    0x05800,
                                    0x05E40,
                                    0x05FC0,
                                    0x06168,
                                    0x06268,
                                };

        // Initialize String Tables
        private void getListItems()
        {
            ushort[] itemlist = (m_parent.savegame_oras) ? Legal.Pouch_Items_ORAS : Legal.Pouch_Items_XY;
            item_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                item_val[i] = Form1.itemlist[itemlist[i]];
            Array.Sort(item_val);
        }
        private void getListKeyItems()
        {
            ushort[] itemlist = (m_parent.savegame_oras) ? Legal.Pouch_Key_ORAS : Legal.Pouch_Key_XY;
            keyitem_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                keyitem_val[i] = Form1.itemlist[itemlist[i]];
            Array.Sort(keyitem_val);
        }
        private void getListTMHM()
        {
            ushort[] itemlist = (m_parent.savegame_oras) ? Legal.Pouch_TMHM_ORAS : Legal.Pouch_TMHM_XY;
            tmhm_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                tmhm_val[i] = Form1.itemlist[itemlist[i]];
            // Array.Sort(tmhm_val); Already sorted, keep HMs last.
        }
        private void getListMedicine()
        {
            ushort[] itemlist =  (m_parent.savegame_oras) ? Legal.Pouch_Medicine_ORAS : Legal.Pouch_Medicine_XY;
            medicine_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                medicine_val[i] = Form1.itemlist[itemlist[i]];
            Array.Sort(medicine_val); 
        }
        private void getListBerries()
        {
            ushort[] itemlist = Legal.Pouch_Berry_XY;
            berries_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                berries_val[i] = Form1.itemlist[itemlist[i]];
            Array.Sort(berries_val); 
        }

        // Populate DataGrid
        private void popItems()
        {
            int offset = bagoffsets[0] + shiftval;
            populateList(item_val, offset, item_val.Length - 1); // max 400
        }
        private void popKeyItems()
        {
            int offset = bagoffsets[1] + shiftval;
            populateList(keyitem_val, offset, keyitem_val.Length - 1); // max 96
        }
        private void popTMHM()
        {
            int offset = bagoffsets[2] + shiftval;
            populateList(tmhm_val, offset, tmhm_val.Length - 1);
        }
        private void popMedicine()
        {
            int offset = bagoffsets[3] + shiftval;
            populateList(medicine_val, offset, medicine_val.Length - 1); // 64 total slots
        }
        private void popBerries()
        {
            int offset = bagoffsets[4] + shiftval;
            populateList(berries_val, offset, berries_val.Length - 1); // 102 slots
        }

        private void populateList(string[] itemarr, int offset, int itemcount)
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
            DataGridViewComboBoxColumn dgvItemVal = new DataGridViewComboBoxColumn();
            dgvItemVal.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            {
                for (int i = 0; i < itemarr.Length; i++)
                    dgvItemVal.Items.Add(itemarr[i]); // add only the Item Names

                dgvItemVal.DisplayIndex = 0;
                dgvItemVal.Width = 135;
                dgvItemVal.FlatStyle = FlatStyle.Flat;
            }
            dataGridView1.Columns.Add(dgvItemVal);
            dataGridView1.Columns.Add(dgvIndex);

            dataGridView1.Rows.Add(itemcount);
            dataGridView1.CancelEdit();

            string itemname = "";
            for (int i = 0; i < itemcount; i++)
            {
                int itemvalue = BitConverter.ToUInt16(sav, offset + i*4);
                try { itemname = Form1.itemlist[itemvalue]; }
                catch
                {
                    Util.Error("Unknown item detected.", "Item ID: " + itemvalue, "Item is after: " + itemname);
                    continue;
                }
                int itemarrayval = Array.IndexOf(itemarr,itemname);
                if (itemarrayval == -1)
                {
                    dataGridView1.Rows[i].Cells[0].Value = itemarr[0];
                    dataGridView1.Rows[i].Cells[1].Value = 0;
                    Util.Alert(itemname + " removed from item pouch.", "If you exit the Item Editor by saving changes, the item will no longer be in the pouch.");
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
            if (e.ColumnIndex == 0)
            {
                ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
                comboBox.DroppedDown = true;
            }
        }
        private void saveBag(object sender)
        {
            string[] itemstrarr = {};
            int offset = 0;
            if (B_DisplayItems.ForeColor == Color.Red)
            {
                itemstrarr = item_val;
                offset = bagoffsets[0] + shiftval;
            }
            else if (B_DisplayKeyItems.ForeColor == Color.Red)
            {
                itemstrarr = keyitem_val;
                offset = bagoffsets[1] + shiftval;
            }
            else if (B_DisplayTMHM.ForeColor == Color.Red)
            {
                itemstrarr = tmhm_val;
                offset = bagoffsets[2] + shiftval;
            }
            else if (B_DisplayMedicine.ForeColor == Color.Red)
            {
                itemstrarr = medicine_val;
                offset = bagoffsets[3] + shiftval;
            }
            else if (B_DisplayBerries.ForeColor == Color.Red)
            {
                itemstrarr = berries_val;
                offset = bagoffsets[4] + shiftval;
            }

            // Fetch Data
            int itemcount = dataGridView1.Rows.Count;
            int emptyslots = 0;
            for (int i = 0; i < itemcount; i++)
            {
                string item = dataGridView1.Rows[i].Cells[0].Value.ToString();
                int itemindex = Array.IndexOf(Form1.itemlist, item);
                int itemcnt = 0;
                try 
                { itemcnt = Convert.ToUInt16(dataGridView1.Rows[i].Cells[1].Value.ToString()); }
                catch { itemcnt = 0; }

                if (itemindex == 0) // Compression of Empty Slots
                {
                    emptyslots++;
                    continue;
                }
                else if (itemcnt == 0)
                    itemcnt++; // No 0 count of items
                else if (itemcnt > 995)
                    itemcnt = 995;

                // Write Data into Save File
                Array.Copy(BitConverter.GetBytes(itemindex), 0, sav, offset + 4 * (i - emptyslots), 2); // item #
                Array.Copy(BitConverter.GetBytes(itemcnt), 0, sav, offset + 4 * (i - emptyslots) + 2, 2); // count
            }

            // Delete Empty Trash
            for (int i = itemcount - emptyslots; i < itemcount; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, sav, offset + 4 * i + 0, 2); // item #
                Array.Copy(BitConverter.GetBytes(0), 0, sav, offset + 4 * i + 2, 2); // count
            }

            // Load New Button Color, after finished we'll load the new data.
            Button btn = sender as Button;
            B_DisplayItems.ForeColor =
            B_DisplayKeyItems.ForeColor =
            B_DisplayTMHM.ForeColor =
            B_DisplayMedicine.ForeColor =
            B_DisplayBerries.ForeColor = Form1.defaultControlText;

            btn.ForeColor = Color.Red;
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
            popItems();
            if (ModifierKeys == Keys.Alt)
                giveAll(item_val,995);
        }
        private void B_DisplayKeyItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popKeyItems();
            if (ModifierKeys == Keys.Alt && Util.Prompt(MessageBoxButtons.YesNo, String.Format("Warning: Adding all {0} is dangerous.", B_DisplayKeyItems.Text), "Continue?") == DialogResult.Yes)
                giveAll(keyitem_val, 1);
        }
        private void B_DisplayTMHM_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popTMHM();
            if (ModifierKeys == Keys.Alt && Util.Prompt(MessageBoxButtons.YesNo, String.Format("Warning: Adding all {0} is dangerous.", B_DisplayTMHM.Text), "Continue?") == DialogResult.Yes)
                giveAll(tmhm_val, 1);
        }
        private void B_DisplayMedicine_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popMedicine();
            if (ModifierKeys == Keys.Alt)
                giveAll(medicine_val,995);
        }
        private void B_DisplayBerries_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popBerries();
            if (ModifierKeys == Keys.Alt)
                giveAll(berries_val,995);
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            saveBag(sender);
            Array.Copy(sav, m_parent.savefile, 0x100000);
            m_parent.savedited = true;
            Close();
        }
    }
}
