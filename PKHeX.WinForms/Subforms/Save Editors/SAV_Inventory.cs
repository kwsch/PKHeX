using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class SAV_Inventory : Form
    {
        private readonly SaveFile Origin;
        private readonly SaveFile SAV;

        public SAV_Inventory(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (Origin = sav).Clone();
            itemlist = GameInfo.Strings.GetItemStrings(SAV.Generation, SAV.Version).ToArray();

            for (int i = 0; i < itemlist.Length; i++)
            {
                if (string.IsNullOrEmpty(itemlist[i]))
                    itemlist[i] = $"(Item #{i:000})";
            }

            HasFreeSpace = SAV.Generation == 7 && !(SAV is SAV7b);
            HasNew = CHK_NEW.Visible = SAV.Generation == 7;
            Pouches = SAV.Inventory;
            InitBags();
            GetBags();
            SwitchBag(null, EventArgs.Empty); // bag 0
        }

        private readonly InventoryPouch[] Pouches;
        private const string DGVPrefix = "DGV_";
        private readonly bool HasFreeSpace;
        private readonly bool HasNew;

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SetBags();
            SAV.Inventory = Pouches;
            Origin.SetData(SAV.Data, 0);
            Close();
        }

        private void InitBags()
        {
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(IL_Pouch.Images[0].Width + 4, IL_Pouch.Images[0].Height + 4);
            for (int i = 0; i < Pouches.Length; i++)
            {
                // Add Tab
                tabControl1.TabPages.Add(new TabPage
                {
                    // Text = Pouches[i].Type.ToString(),
                    ImageIndex = (int)Pouches[i].Type
                });

                tabControl1.TabPages[i].Controls.Add(GetDGV(Pouches[i]));
            }
        }

        private DataGridView GetDGV(InventoryPouch pouch)
        {
            // Add DataGrid
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                Text = pouch.Type.ToString(),
                Name = DGVPrefix + pouch.Type,

                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                RowHeadersVisible = false,
                //ColumnHeadersVisible = false,
                MultiSelect = false,
                ShowEditingIcon = false,

                EditMode = DataGridViewEditMode.EditOnEnter,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
            };

            int c = 0;
            var dgvItemVal = new DataGridViewComboBoxColumn
            {
                HeaderText = "Item",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = c++,
                Width = 135,
                FlatStyle = FlatStyle.Flat
            };
            var dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "Count";
                dgvIndex.DisplayIndex = c++;
                dgvIndex.Width = 45;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (!Main.HaX)
                dgvIndex.MaxInputLength = (int)(Math.Log10(Math.Max(1, pouch.MaxCount)) + 1);
            }

            dgv.Columns.Add(dgvItemVal);
            dgv.Columns.Add(dgvIndex);

            if (HasFreeSpace)
            {
                var dgvFree = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "Free",
                    DisplayIndex = c++,
                    Width = 40,
                    FlatStyle = FlatStyle.Flat
                };
                dgv.Columns.Add(dgvFree);
            }
            if (HasNew)
            {
                var dgvNew = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "NEW",
                    DisplayIndex = c++,
                    Width = 40,
                    FlatStyle = FlatStyle.Flat
                };
                dgv.Columns.Add(dgvNew);
            }

            var itemcount = pouch.Items.Length;
            string[] itemarr = Main.HaX ? (string[])itemlist.Clone() : GetItems(pouch.LegalItems);

            var combo = (DataGridViewComboBoxColumn) dgv.Columns[0];
            foreach (string t in itemarr)
                combo.Items.Add(t); // add only the Item Names

            dgv.Rows.Add(itemcount > 0 ? itemcount : itemarr.Length);
            dgv.CancelEdit();

            return dgv;
        }

        private DataGridView GetGrid(InventoryType type) => (DataGridView)Controls.Find(DGVPrefix + type, true)[0];
        private DataGridView GetGrid(int pouch) => GetGrid(Pouches[pouch].Type);

        private void GetBags()
        {
            foreach (InventoryPouch pouch in Pouches)
            {
                var dgv = GetGrid(pouch.Type);

                // Sanity Screen
                var invalid = pouch.Items.Where(item => item.Index != 0 && !pouch.LegalItems.Contains((ushort)item.Index)).ToArray();
                var outOfBounds = invalid.Where(item => item.Index >= itemlist.Length).ToArray();
                var incorrectPouch = invalid.Where(item => item.Index < itemlist.Length).ToArray();

                if (outOfBounds.Length > 0)
                    WinFormsUtil.Error(MsgItemPouchUnknown, $"Item ID(s): {string.Join(", ", outOfBounds.Select(item => item.Index))}");
                if (!Main.HaX && incorrectPouch.Length > 0)
                    WinFormsUtil.Alert(string.Format(MsgItemPouchRemoved, pouch.Type), string.Join(", ", incorrectPouch.Select(item => itemlist[item.Index])), MsgItemPouchWarning);

                pouch.Sanitize(Main.HaX, itemlist.Length - 1);
                GetBag(dgv, pouch);
            }
        }

        private void SetBags()
        {
            foreach (var pouch in Pouches)
            {
                var dgv = GetGrid(pouch.Type);
                SetBag(dgv, pouch);
            }
        }

        private void GetBag(DataGridView dgv, InventoryPouch pouch)
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                int c = 0;
                dgv.Rows[i].Cells[c++].Value = itemlist[pouch.Items[i].Index];
                dgv.Rows[i].Cells[c++].Value = pouch.Items[i].Count;
                if (HasFreeSpace)
                    dgv.Rows[i].Cells[c++].Value = pouch.Items[i].FreeSpace;
                if (HasNew)
                    dgv.Rows[i].Cells[c].Value = pouch.Items[i].New;
            }
        }

        private void SetBag(DataGridView dgv, InventoryPouch pouch)
        {
            int ctr = 0;
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                int c = 0;
                string item = dgv.Rows[i].Cells[c++].Value.ToString();
                int itemindex = Array.IndexOf(itemlist, item);

                if (itemindex <= 0 && !HasNew) // Compression of Empty Slots
                    continue;

                int.TryParse(dgv.Rows[i].Cells[c++].Value?.ToString(), out int itemcnt);

                if (Main.HaX && SAV.Generation != 7) // Gen7 has true cap at 1023, keep 999 cap.
                {
                    // Cap at absolute maximum
                    if (SAV.Generation <= 2 && itemcnt > byte.MaxValue)
                        itemcnt = byte.MaxValue;
                    else if (SAV.Generation >= 3 && itemcnt > ushort.MaxValue)
                        itemcnt = ushort.MaxValue;
                }
                else if (itemcnt > pouch.MaxCount)
                {
                    if (itemindex == 797 && itemcnt >= 2) // Edge case when for some reason the item count for Z-Ring was 2 in an unedited save and set 1 after using PKHeX
                        itemcnt = 2;
                    else
                        itemcnt = pouch.MaxCount; // Cap at pouch maximum
                }
                else if (itemcnt <= 0 && !HasNew)
                {
                    continue; // ignore item
                }

                pouch.Items[ctr] = new InventoryItem { Index = itemindex, Count = itemcnt };
                if (HasFreeSpace)
                    pouch.Items[ctr].FreeSpace = (bool)dgv.Rows[i].Cells[c++].Value;
                if (HasNew)
                    pouch.Items[ctr].New = (bool)dgv.Rows[i].Cells[c].Value;
                ctr++;
            }
            for (int i = ctr; i < pouch.Items.Length; i++)
                pouch.Items[i] = new InventoryItem(); // Empty Slots at the end
        }

        private void SwitchBag(object sender, EventArgs e)
        {
            int index = tabControl1.SelectedIndex;
            var pouch = Pouches[index];
            NUD_Count.Maximum = GetMax(SAV, pouch, Main.HaX);

            bool disable = pouch.Type == InventoryType.PCItems || pouch.Type == InventoryType.FreeSpace;
            NUD_Count.Visible = L_Count.Visible = B_GiveAll.Visible = !disable;
            if (disable && !Main.HaX)
            {
                giveMenu.Items.Remove(giveAll);
                giveMenu.Items.Remove(giveModify);
            }
            else if (!giveMenu.Items.Contains(giveAll))
            {
                giveMenu.Items.Insert(0, giveAll);
                giveMenu.Items.Add(giveModify);
            }
            NUD_Count.Value = Math.Max(1, pouch.MaxCount - 4);
        }

        private static int GetMax(ITrainerInfo sav, InventoryPouch pouch, bool haX)
        {
            if (haX)
                return pouch.MaxCount;

            // Cap at absolute maximum
            if (sav.Generation <= 2)
                return byte.MaxValue;
            if (sav.Generation >= 7)
                return pouch.MaxCount;
            // if (SAV.Generation >= 3)
            return ushort.MaxValue;
        }

        // Initialize String Tables
        private readonly string[] itemlist;

        private string[] GetItems(ushort[] items, bool sort = true)
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
        private int CurrentPouch => tabControl1.SelectedIndex;
        private void B_GiveAll_Click(object sender, EventArgs e) => ShowContextMenuBelow(giveMenu, (Control)sender);
        private void B_Sort_Click(object sender, EventArgs e) => ShowContextMenuBelow(sortMenu, (Control)sender);

        private void SortByName(object sender, EventArgs e) => Sort(p => p.SortByName(itemlist, reverse: sender == mnuSortNameReverse));
        private void SortByCount(object sender, EventArgs e) => Sort(p => p.SortByCount(reverse: sender == mnuSortCountReverse));
        private void SortByIndex(object sender, EventArgs e) => Sort(p => p.SortByIndex(reverse: sender == mnuSortIndexReverse));

        private static void ShowContextMenuBelow(ToolStripDropDown c, Control n) => c.Show(n.PointToScreen(new Point(0, n.Height)));

        private void GiveAllItems(object sender, EventArgs e)
        {
            // Get Current Pouch
            int pouch = CurrentPouch;
            if (pouch < 0)
                return;
            var p = Pouches[pouch];
            ushort[] legalitems = (ushort[])p.LegalItems.Clone();
            bool resize = legalitems.Length > p.Items.Length;
            if (resize)
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgItemPouchSizeSmall,
                    string.Format(MsgItemPouchRandom, Environment.NewLine));
                if (dr == DialogResult.Cancel)
                    return;
                if (dr == DialogResult.No)
                    Util.Shuffle(legalitems);
                Array.Resize(ref legalitems, p.Items.Length);
            }

            var dgv = GetGrid(pouch);
            SetBag(dgv, p);

            int Count = (int)NUD_Count.Value;
            for (int i = 0; i < legalitems.Length; i++)
            {
                ushort item = legalitems[i];
                var itemname = itemlist[item];
                int c = Count;

                // Override for HMs
                if (ItemConverter.IsItemHM(item, SAV.Generation))
                    c = 1;

                int l = 0;
                dgv.Rows[i].Cells[l++].Value = itemname;
                dgv.Rows[i].Cells[l++].Value = c;

                var t = Array.Find(p.Items, m => m.Index == item);

                if (HasFreeSpace)
                    dgv.Rows[i].Cells[l++].Value = t?.FreeSpace ?? false;
                if (HasNew)
                    dgv.Rows[i].Cells[l].Value = t?.New ?? CHK_NEW.Checked;
            }
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void RemoveAllItems(object sender, EventArgs e)
        {
            // Get Current Pouch
            int pouch = CurrentPouch;
            if (pouch < 0)
                return;

            var dgv = GetGrid(pouch);

            for (int i = 0; i < dgv.RowCount; i++)
            {
                int c = 0;
                dgv.Rows[i].Cells[c++].Value = itemlist[0];
                dgv.Rows[i].Cells[c++].Value = 0;
                if (HasFreeSpace)
                    dgv.Rows[i].Cells[c++].Value = false;
                if (HasNew)
                    dgv.Rows[i].Cells[c].Value = false;
            }
            WinFormsUtil.Alert(MsgItemCleared);
        }

        private void ModifyAllItems(object sender, EventArgs e)
        {
            // Get Current Pouch
            int pouch = CurrentPouch;
            if (pouch < 0)
                return;

            var dgv = GetGrid(pouch);

            for (int i = 0; i < dgv.RowCount; i++)
            {
                string item = dgv.Rows[i].Cells[0].Value.ToString();
                int itemindex = Array.IndexOf(itemlist, item);
                if (itemindex > 0)
                    dgv.Rows[i].Cells[1].Value = ItemConverter.IsItemHM((ushort)itemindex, SAV.Generation) ? 1 : NUD_Count.Value;
            }
            WinFormsUtil.Alert(MsgItemPouchCountUpdated);
        }

        private void Sort(Action<InventoryPouch> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            int pouch = CurrentPouch;
            var dgv = GetGrid(pouch);
            var p = Pouches[pouch];
            SetBag(dgv, p); // save current
            func(p); // update
            GetBag(dgv, p); // load current
        }
    }
}
