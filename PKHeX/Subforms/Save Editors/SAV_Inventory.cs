using System;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Inventory : Form
    {
        public SAV_Inventory()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            switch (SAV.Generation)
            {
                case 1:
                    itemlist = Main.g1items;
                    B_GiveAll.Visible = false; // Can't give all, not enough room
                    break;
                case 2:
                    itemlist = Main.g2items;
                    B_GiveAll.Visible = false;
                    break;
                case 3:
                    itemlist = Main.g3items;
                    break;
                default:
                    itemlist = Main.itemlist;
                    break;
            }
            Pouches = SAV.Inventory;
            getBags();
        }

        private readonly SaveFile SAV = Main.SAV.Clone();
        private readonly InventoryPouch[] Pouches;
        private const string TabPrefix = "TAB_";
        private const string DGVPrefix = "DGV_";

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            packBags();
            SAV.Inventory = Pouches;
            Array.Copy(SAV.Data, Main.SAV.Data, SAV.Data.Length);
            Main.SAV.Edited = true;
            Close();
        }

        private void getBags()
        {
            for (int i = 0; i < Pouches.Length; i++)
            {
                // Add Tab
                tabControl1.TabPages.Add(new TabPage
                {
                    // Text = Pouches[i].Type.ToString(),
                    ImageIndex = (int)Pouches[i].Type
                });

                // Add DataGrid
                DataGridView dgv = new DataGridView
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, // All
                    Height = tabControl1.TabPages[i].Height,
                    Width = tabControl1.TabPages[i].Width,
                    Text = Pouches[i].Type.ToString(),
                    Name = DGVPrefix + Pouches[i].Type,

                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToResizeRows = false,
                    AllowUserToResizeColumns = false,
                    RowHeadersVisible = false,
                    ColumnHeadersVisible = false,
                    MultiSelect = false,
                    ShowEditingIcon = false,

                    EditMode = DataGridViewEditMode.EditOnEnter,
                    ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    SelectionMode = DataGridViewSelectionMode.CellSelect,
                    CellBorderStyle = DataGridViewCellBorderStyle.None,
                };

                tabControl1.TabPages[i].Controls.Add(dgv);

                spillBag(dgv, i);
            }
        }
        private void spillBag(DataGridView dgv, int bag)
        {
            var pouch = Pouches[bag];
            var itemcount = Pouches[bag].Items.Length;
            string[] itemarr = getItems(pouch.LegalItems);
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            DataGridViewComboBoxColumn dgvItemVal = new DataGridViewComboBoxColumn
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0,
                Width = 135,
                FlatStyle = FlatStyle.Flat
            };
            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "CNT";
                dgvIndex.DisplayIndex = 1;
                dgvIndex.Width = 45;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (string t in itemarr)
                dgvItemVal.Items.Add(t); // add only the Item Names

            dgv.Columns.Add(dgvItemVal);
            dgv.Columns.Add(dgvIndex);

            dgv.Rows.Add(itemcount > 0 ? itemcount : itemarr.Length);
            dgv.CancelEdit();

            string itemname = "";
            string err = "";
            for (int i = 0; i < pouch.Items.Length; i++)
            {
                int itemvalue = pouch.Items[i].Index;
                try { itemname = itemlist[itemvalue]; }
                catch
                {
                    Util.Error("Unknown item detected.", "Item ID: " + itemvalue, "Item is after: " + itemname);
                    continue;
                }
                int itemarrayval = Array.IndexOf(itemarr, itemname);
                if (itemarrayval == -1)
                {
                    dgv.Rows[i].Cells[0].Value = itemarr[0];
                    dgv.Rows[i].Cells[1].Value = 0;

                    err += itemname + ", ";
                }
                else
                {
                    dgv.Rows[i].Cells[0].Value = itemname;
                    dgv.Rows[i].Cells[1].Value = pouch.Items[i].Count;
                }
            }
            if (err.Length > 0)
                Util.Alert($"The following items have been removed from {Pouches[bag].Type} pouch.", err,
                    "If you save changes, the item(s) will no longer be in the pouch.");
        }
        private void packBags()
        {
            foreach (InventoryPouch t in Pouches)
            {
                // Get DataGridView
                DataGridView dgv = Controls.Find(DGVPrefix + t.Type, true).FirstOrDefault() as DataGridView;

                int ctr = 0;
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    string item = dgv.Rows[i].Cells[0].Value.ToString();
                    int itemindex = Array.IndexOf(itemlist, item);
                    int itemcnt;
                    try
                    { itemcnt = Convert.ToUInt16(dgv.Rows[i].Cells[1].Value.ToString()); }
                    catch { itemcnt = 0; }

                    if (itemcnt == 0)
                        itemcnt++; // No 0 count of items
                    else if (itemcnt > 995)
                        itemcnt = 995; // cap out
                    else if (itemcnt > 99 && SAV.Generation < 3)
                        itemcnt = 99;

                    if (itemindex == 0) // Compression of Empty Slots
                        continue;

                    t.Items[ctr++] = new InventoryItem {Index = itemindex, Count = itemcnt};
                }
                for (int i = ctr; i < t.Items.Length; i++)
                    t.Items[i] = new InventoryItem(); // Empty Slots at the end
            }
        }

        // Initialize String Tables
        private readonly string[] itemlist;
        private string[] getItems(ushort[] items, bool sort = true)
        {
            string[] res = new string[items.Length + 1];
            for (int i = 0; i < res.Length - 1; i++)
                res[i] = itemlist[items[i]];
            res[items.Length] = itemlist[0];
            if (sort)
                Array.Sort(res);
            return res;
        }

        // User Cheats
        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            // Get Current Pouch
            int pouch = tabControl1.SelectedIndex;
            if (pouch < 0)
                return;

            ushort[] legalitems = Pouches[pouch].LegalItems;

            DataGridView dgv = Controls.Find(DGVPrefix + Pouches[pouch].Type, true).FirstOrDefault() as DataGridView;

            if (ModifierKeys == Keys.Alt)
            {
                for (int i = 0; i < legalitems.Length; i++)
                {
                    dgv.Rows[i].Cells[0].Value = itemlist[0];
                    dgv.Rows[i].Cells[1].Value = 0;
                }
                Util.Alert("Items cleared.");
                return;
            }
            int Count = ModifierKeys == Keys.Control ? 1 : Pouches[pouch].MaxCount;
            for (int i = 0; i < legalitems.Length; i++)
            {
                int item = legalitems[i];
                string itemname;
                int c = Count;

                // Override for HMs
                switch (SAV.Generation)
                {
                    case 3: {
                        itemname = itemlist[item];
                        if (Legal.Pouch_HM_RS.Contains(legalitems[i])) c = 1;
                        break;
                    }
                    default: {
                        itemname = itemlist[item];
                        if (new[] { 420, 421, 422, 423, 423, 424, 425, 426, 427, 737 }.Contains(legalitems[i])) c = 1;
                        break;
                    }
                }

                dgv.Rows[i].Cells[0].Value = itemname;
                dgv.Rows[i].Cells[1].Value = c;
            }
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
