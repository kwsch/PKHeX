using System;
using System.Collections.Generic;
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
            CreateBagViews();
            LoadAllBags();
            ChangeViewedPouch(0);
        }

        private readonly InventoryPouch[] Pouches;
        private readonly bool HasFreeSpace;
        private readonly bool HasNew;

        // assume that all pouches have the same amount of columns
        private int ColumnItem;
        private int ColumnCount;
        private int ColumnFreeSpace;
        private int ColumnNEW;

        private readonly Dictionary<InventoryType, DataGridView> ControlGrids = new Dictionary<InventoryType, DataGridView>();
        private DataGridView GetGrid(InventoryType type) => ControlGrids[type];
        private DataGridView GetGrid(int pouch) => ControlGrids[Pouches[pouch].Type];

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            SetBags();
            SAV.Inventory = Pouches;
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void CreateBagViews()
        {
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(IL_Pouch.Images[0].Width + 4, IL_Pouch.Images[0].Height + 4);
            foreach (var pouch in Pouches)
            {
                var tab = new TabPage {ImageIndex = (int)pouch.Type};
                var dgv = GetDGV(pouch);
                ControlGrids.Add(pouch.Type, dgv);
                tab.Controls.Add(dgv);
                tabControl1.TabPages.Add(tab);
                tabControl1.ShowToolTips = true;
                tab.ToolTipText = pouch.Type.ToString();
            }
        }

        private DataGridView GetDGV(InventoryPouch pouch)
        {
            // Add DataGrid
            var dgv = GetBaseDataGrid(pouch);

            // Get Columns
            var item = GetItemColumn(ColumnItem = dgv.Columns.Count);
            dgv.Columns.Add(item);
            dgv.Columns.Add(GetCountColumn(pouch, Main.HaX, ColumnCount = dgv.Columns.Count));
            if (HasFreeSpace)
                dgv.Columns.Add(GetFreeSpaceColumn(ColumnFreeSpace = dgv.Columns.Count));
            if (HasNew)
                dgv.Columns.Add(GetNewColumn(ColumnNEW = dgv.Columns.Count));

            // Populate with rows
            var itemarr = Main.HaX ? itemlist : GetStringsForPouch(pouch.LegalItems);
            item.Items.AddRange(itemarr);

            dgv.Rows.Add(pouch.Items.Length);
            dgv.CancelEdit();

            return dgv;
        }

        private static DataGridView GetBaseDataGrid(InventoryPouch pouch)
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                Text = pouch.Type.ToString(),
                Name = "DGV_" + pouch.Type,

                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                RowHeadersVisible = false,
                MultiSelect = false,
                ShowEditingIcon = false,

                EditMode = DataGridViewEditMode.EditOnEnter,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
            };
        }

        private static DataGridViewComboBoxColumn GetItemColumn(int c)
        {
            return new DataGridViewComboBoxColumn
            {
                HeaderText = "Item",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = c,
                Width = 135,
                FlatStyle = FlatStyle.Flat
            };
        }

        private static DataGridViewColumn GetCountColumn(InventoryPouch pouch, bool HaX, int c)
        {
            var dgvIndex = new DataGridViewTextBoxColumn
            {
                HeaderText = "Count",
                DisplayIndex = c,
                Width = 45,
                DefaultCellStyle = {Alignment = DataGridViewContentAlignment.MiddleCenter},
            };
            if (!HaX)
                dgvIndex.MaxInputLength = (int)(Math.Log10(Math.Max(1, pouch.MaxCount)) + 1);
            return dgvIndex;
        }

        private static DataGridViewColumn GetFreeSpaceColumn(int c)
        {
            return new DataGridViewCheckBoxColumn
            {
                HeaderText = "Free",
                DisplayIndex = c,
                Width = 40,
                FlatStyle = FlatStyle.Flat
            };
        }

        private static DataGridViewColumn GetNewColumn(int c)
        {
            return new DataGridViewCheckBoxColumn
            {
                HeaderText = "NEW",
                DisplayIndex = c,
                Width = 40,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void LoadAllBags()
        {
            foreach (var pouch in Pouches)
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

                pouch.Sanitize(itemlist.Length - 1, Main.HaX);
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
                var cells = dgv.Rows[i].Cells;
                cells[ColumnItem].Value = itemlist[pouch.Items[i].Index];
                cells[ColumnCount].Value = pouch.Items[i].Count;
                if (HasFreeSpace)
                    cells[ColumnFreeSpace].Value = pouch.Items[i].FreeSpace;
                if (HasNew)
                    cells[ColumnNEW].Value = pouch.Items[i].New;
            }
        }

        private void SetBag(DataGridView dgv, InventoryPouch pouch)
        {
            int ctr = 0;
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                var cells = dgv.Rows[i].Cells;
                var str = cells[ColumnItem].Value.ToString();
                var itemindex = Array.IndexOf(itemlist, str);

                if (itemindex <= 0 && !HasNew) // Compression of Empty Slots
                    continue;

                int.TryParse(cells[ColumnCount].Value?.ToString(), out int itemcnt);
                if (!pouch.IsValidItemAndCount(SAV, itemindex, HasNew, Main.HaX, ref itemcnt))
                    continue; // ignore item

                // create clean item data when saving
                var obj = new InventoryItem {Index = itemindex, Count = itemcnt};
                if (HasFreeSpace)
                    obj.FreeSpace = (bool)cells[ColumnFreeSpace].Value;
                if (HasNew)
                    obj.New = (bool)cells[ColumnNEW].Value;
                pouch.Items[ctr] = obj;
                ctr++;
            }
            for (int i = ctr; i < pouch.Items.Length; i++)
                pouch.Items[i] = new InventoryItem(); // Empty Slots at the end
        }

        private void ChangeViewedPouch(int index)
        {
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

        private string[] GetStringsForPouch(ushort[] items, bool sort = true)
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
        private void SwitchBag(object sender, EventArgs e) => ChangeViewedPouch(CurrentPouch);
        private void B_GiveAll_Click(object sender, EventArgs e) => ShowContextMenuBelow(giveMenu, (Control)sender);
        private void B_Sort_Click(object sender, EventArgs e) => ShowContextMenuBelow(sortMenu, (Control)sender);

        private void SortByName(object sender, EventArgs e) => ModifyPouch(CurrentPouch, p => p.SortByName(itemlist, reverse: sender == mnuSortNameReverse));
        private void SortByCount(object sender, EventArgs e) => ModifyPouch(CurrentPouch, p => p.SortByCount(reverse: sender == mnuSortCountReverse));
        private void SortByIndex(object sender, EventArgs e) => ModifyPouch(CurrentPouch, p => p.SortByIndex(reverse: sender == mnuSortIndexReverse));

        private static void ShowContextMenuBelow(ToolStripDropDown c, Control n) => c.Show(n.PointToScreen(new Point(0, n.Height)));

        private void GiveAllItems(object sender, EventArgs e)
        {
            var pouch = Pouches[CurrentPouch];
            if (!GetModifySettings(pouch, out var truncate, out var shuffle))
                return;

            var items = pouch.LegalItems;
            if (truncate)
            {
                items = (ushort[])items.Clone();
                if (shuffle)
                    Util.Shuffle(items);
                Array.Resize(ref items, pouch.Items.Length);
            }

            ModifyPouch(CurrentPouch, p => p.GiveAllItems(SAV, items, (int)NUD_Count.Value));
            System.Media.SystemSounds.Asterisk.Play();
        }

        private static bool GetModifySettings(InventoryPouch pouch, out bool truncate, out bool shuffle)
        {
            truncate = false;
            shuffle = false;
            if (!pouch.IsCramped)
                return true;

            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgItemPouchSizeSmall,
                string.Format(MsgItemPouchRandom, Environment.NewLine));
            if (dr == DialogResult.Cancel)
                return false;
            truncate = true;
            if (dr == DialogResult.No)
                shuffle = true;

            return true;
        }

        private void RemoveAllItems(object sender, EventArgs e)
        {
            ModifyPouch(CurrentPouch, p => p.RemoveAll());
            WinFormsUtil.Alert(MsgItemCleared);
        }

        private void ModifyAllItems(object sender, EventArgs e)
        {
            ModifyPouch(CurrentPouch, p => p.ModifyAllCount(SAV, (int)NUD_Count.Value));
            WinFormsUtil.Alert(MsgItemPouchCountUpdated);
        }

        private void ModifyPouch(int pouch, Action<InventoryPouch> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var dgv = GetGrid(pouch);
            var p = Pouches[pouch];
            SetBag(dgv, p); // save current
            func(p); // update
            GetBag(dgv, p); // load current
        }
    }
}
