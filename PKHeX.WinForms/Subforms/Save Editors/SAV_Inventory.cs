using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_Inventory : Form
{
    private readonly SaveFile Origin;
    private readonly SaveFile SAV;

    private static readonly ImageList IL_Pouch = InventoryTypeImageUtil.GetImageList();

    public SAV_Inventory(SaveFile sav)
    {
        InitializeComponent();
        tabControl1.ImageList = IL_Pouch;
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (Origin = sav).Clone();
        itemlist = [.. GameInfo.Strings.GetItemStrings(SAV.Context, SAV.Version)]; // copy

        for (int i = 0; i < itemlist.Length; i++)
        {
            if (string.IsNullOrEmpty(itemlist[i]))
                itemlist[i] = $"(Item #{i:000})";
        }

        Pouches = SAV.Inventory;
        var item0 = Pouches[0].Items[0];
        HasFreeSpace = item0 is IItemFreeSpace;
        HasFreeSpaceIndex = item0 is IItemFreeSpaceIndex;
        HasFavorite = item0 is IItemFavorite;
        HasNew = item0 is IItemNewFlag;

        CreateBagViews();
        LoadAllBags();
        ChangeViewedPouch(0);
    }

    private readonly IReadOnlyList<InventoryPouch> Pouches;
    private readonly bool HasFreeSpace;
    private readonly bool HasFreeSpaceIndex;
    private readonly bool HasFavorite;
    private readonly bool HasNew;

    // assume that all pouches have the same amount of columns
    private int ColumnItem;
    private int ColumnCount;
    private int ColumnFreeSpace;
    private int ColumnFreeSpaceIndex;
    private int ColumnFavorite;
    private int ColumnNEW;

    private readonly Dictionary<InventoryType, DataGridView> ControlGrids = [];
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
            var tab = new TabPage { ImageIndex = InventoryTypeImageUtil.GetImageIndex(pouch.Type) };
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
        if (HasFavorite)
            dgv.Columns.Add(GetCheckColumn(ColumnFavorite = dgv.Columns.Count,"Fav"));
        if (HasNew)
            dgv.Columns.Add(GetCheckColumn(ColumnNEW = dgv.Columns.Count, "New"));

        if (HasFreeSpace)
            dgv.Columns.Add(GetCheckColumn(ColumnFreeSpace = dgv.Columns.Count, "Free"));
        if (HasFreeSpaceIndex)
            dgv.Columns.Add(GetCountColumn(pouch, true, ColumnFreeSpaceIndex = dgv.Columns.Count, "Free"));

        // Populate with rows
        var itemarr = Main.HaX ? itemlist : GetStringsForPouch(pouch.GetAllItems());
        item.Items.AddRange(itemarr);

        var items = pouch.Items;
        if (items.Length != 0)
            dgv.Rows.Add(items.Length);
        dgv.CancelEdit();

        return dgv;
    }

    private static DataGridView GetBaseDataGrid(InventoryPouch pouch) => new()
    {
        Dock = DockStyle.Fill,
        Text = $"{pouch.Type}",
        Name = $"DGV_{pouch.Type}",

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

    private static DataGridViewComboBoxColumn GetItemColumn(int c, string name = "Item") => new()
    {
        HeaderText = name,
        DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
        DisplayIndex = c,
        Width = 135,
        FlatStyle = FlatStyle.Flat,
    };

    private static DataGridViewCheckBoxColumn GetCheckColumn(int c, string name) => new()
    {
        HeaderText = name,
        DisplayIndex = c,
        Width = 40,
        FlatStyle = FlatStyle.Flat,
    };

    private static DataGridViewTextBoxColumn GetCountColumn(InventoryPouch pouch, bool HaX, int c, string name = "Count")
    {
        var dgvIndex = new DataGridViewTextBoxColumn
        {
            HeaderText = name,
            DisplayIndex = c,
            Width = 45,
            DefaultCellStyle = {Alignment = DataGridViewContentAlignment.MiddleCenter},
        };
        if (!HaX)
            dgvIndex.MaxInputLength = (int)(Math.Log10(Math.Max(1, pouch.MaxCount)) + 1);
        return dgvIndex;
    }

    private void LoadAllBags()
    {
        foreach (var pouch in Pouches)
        {
            var dgv = GetGrid(pouch.Type);

            // Sanity Screen
            var invalid = Array.FindAll(pouch.Items, item => item.Index != 0 && !pouch.CanContain((ushort)item.Index));
            var outOfBounds = Array.FindAll(invalid, item => item.Index >= itemlist.Length);
            var incorrectPouch = Array.FindAll(invalid, item => item.Index < itemlist.Length);

            if (outOfBounds.Length != 0)
                WinFormsUtil.Error(MsgItemPouchUnknown, $"Item ID(s): {string.Join(", ", outOfBounds.Select(item => item.Index))}");
            if (!Main.HaX && incorrectPouch.Length != 0)
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
        var valid = pouch.GetAllItems();
        for (int i = 0; i < dgv.Rows.Count; i++)
        {
            var item = pouch.Items[i];
            if (item.Index != 0 && !valid.Contains((ushort)item.Index) && !Main.HaX)
                item = pouch.Items[i] = pouch.GetEmpty();

            var cells = dgv.Rows[i].Cells;
            cells[ColumnItem].Value = itemlist[item.Index];
            cells[ColumnCount].Value = item.Count;

            if (item is IItemFreeSpace f)
                cells[ColumnFreeSpace].Value = f.IsFreeSpace;
            if (item is IItemFreeSpaceIndex fi)
                cells[ColumnFreeSpaceIndex].Value = fi.FreeSpaceIndex;
            if (item is IItemFavorite v)
                cells[ColumnFavorite].Value = v.IsFavorite;
            if (item is IItemNewFlag n)
                cells[ColumnNEW].Value = n.IsNew;
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

            bool result = int.TryParse(cells[ColumnCount].Value?.ToString(), out int itemcnt);
            if (!result)
                continue;
            if (!pouch.IsValidItemAndCount(SAV, itemindex, HasNew, Main.HaX, ref itemcnt))
                continue; // ignore item

            // create clean item data when saving
            var item = pouch.GetEmpty(itemindex, itemcnt);
            if (item is IItemFreeSpace f)
                f.IsFreeSpace = (bool)cells[ColumnFreeSpace].Value;
            if (item is IItemFreeSpaceIndex fi)
                fi.FreeSpaceIndex = uint.TryParse(cells[ColumnFreeSpaceIndex].Value?.ToString(), out var fsi) ? fsi : 0;
            if (item is IItemFavorite v)
                v.IsFavorite = (bool)cells[ColumnFavorite].Value;
            if (item is IItemNewFlag n)
                n.IsNew = (bool)cells[ColumnNEW].Value;

            pouch.Items[ctr] = item;
            ctr++;
        }
        for (int i = ctr; i < pouch.Items.Length; i++)
            pouch.Items[i] = pouch.GetEmpty(); // Empty Slots at the end
    }

    private void ChangeViewedPouch(int index)
    {
        var pouch = Pouches[index];
        NUD_Count.Maximum = GetMax(SAV, pouch, Main.HaX);

        bool disable = pouch.Type is InventoryType.PCItems or InventoryType.FreeSpace && SAV is not SAV8LA;
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

    private static int GetMax(ITrainerInfo sav, InventoryPouch pouch, bool HaX)
    {
        if (HaX)
            return pouch.MaxCount;

        return sav.Generation switch
        {
            // Cap at absolute maximum
            <= 2 => byte.MaxValue,
            >= 7 => pouch.MaxCount,
            _ => ushort.MaxValue,
        };
    }

    // Initialize String Tables
    private readonly string[] itemlist;

    private string[] GetStringsForPouch(ReadOnlySpan<ushort> items, bool sort = true)
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

        var items = pouch.GetAllItems().ToArray();
        if (truncate)
        {
            if (shuffle)
                Util.Rand.Shuffle(items.AsSpan());
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
        var dgv = GetGrid(pouch);
        var p = Pouches[pouch];
        SetBag(dgv, p); // save current
        func(p); // update
        GetBag(dgv, p); // load current
    }
}

/// <summary>
/// File specific utility class for creating a <see cref="ImageList"/> for displaying an icon in each of the tabs.
/// </summary>
file static class InventoryTypeImageUtil
{
    /// <summary>
    /// Gets the index within the <see cref="ImageList"/> for the given <see cref="InventoryType"/>.
    /// </summary>
    /// <remarks><see cref="InventoryType.None"/> is skipped.</remarks>
    public static int GetImageIndex(InventoryType type) => (int)type - 1;

    /// <summary>
    /// Creates a <see cref="ImageList"/> for displaying an icon in each of the tabs.
    /// </summary>
    public static ImageList GetImageList()
    {
        var result = new ImageList
        {
            TransparentColor = Color.Transparent,
            ImageSize = Properties.Resources.bag_items.Size, // Match the size of the resources.
        };
        var images = result.Images;
        var types = Enum.GetValues<InventoryType>();
        foreach (var type in types)
        {
            if (type is InventoryType.None)
                continue;
            var img = GetImage(type);

            int index = GetImageIndex(type);
            var name = type.ToString();
            images.Add(name, img);
            images.SetKeyName(index, name);
        }
        return result;
    }

    private static Bitmap GetImage(InventoryType type) => type switch
    {
        InventoryType.Items => Properties.Resources.bag_items,
        InventoryType.KeyItems => Properties.Resources.bag_key,
        InventoryType.TMHMs => Properties.Resources.bag_tech,
        InventoryType.Medicine => Properties.Resources.bag_medicine,
        InventoryType.Berries => Properties.Resources.bag_berries,
        InventoryType.Balls => Properties.Resources.bag_balls,
        InventoryType.BattleItems => Properties.Resources.bag_battle,
        InventoryType.MailItems => Properties.Resources.bag_mail,
        InventoryType.PCItems => Properties.Resources.bag_pcitems,
        InventoryType.FreeSpace => Properties.Resources.bag_free,
        InventoryType.ZCrystals => Properties.Resources.bag_z,
        InventoryType.Candy => Properties.Resources.bag_candy,
        InventoryType.Treasure => Properties.Resources.bag_treasure,
        InventoryType.Ingredients => Properties.Resources.bag_ingredient,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
