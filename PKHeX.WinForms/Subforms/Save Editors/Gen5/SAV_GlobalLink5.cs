using System;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public sealed partial class SAV_GlobalLink5 : Form
{
    private const int ItemSpriteColumnIndex = 0;
    private const int ItemNameColumnIndex = 1;
    private const int ItemQuantityColumnIndex = 2;

    private readonly SAV5 Origin;
    private readonly SAV5 SAV;
    private readonly GlobalLink5 Block;

    public SAV_GlobalLink5(SAV5 sav)
    {
        InitializeComponent();
        InitializeItemsTab();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        Origin = sav;
        SAV = (SAV5)sav.Clone();
        Block = SAV.GlobalLink;

        LoadData();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        if (!ValidateChildren())
            return;

        SaveData();
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void LoadData()
    {
        var date = Block.UploadDate;
        if (date.IsValid)
        {
            CAL_UploadDate.Value = date.ToDateOnly().ToDateTime(TimeOnly.MinValue);
            CAL_UploadDate.Visible = CHK_DateSet.Checked = true;
        }
        else
        {
            CAL_UploadDate.Value = DateTime.Today;
            CAL_UploadDate.Visible = CHK_DateSet.Checked = false;
        }

        NUD_UploadCount.Value = Block.UploadCount;
        NUD_UploadStatus.Value = Block.UploadStatus;
        CHK_IsSlotPresent.Checked = Block.IsSlotPresent;
        CHK_IsRegistered.Checked = Block.IsRegistered;
        CHK_IsFullAccess.Checked = Block.IsAccountFullAccess;
        NUD_Musical.Value = Block.Musical;
        NUD_CGearSkin.Value = Block.CGearSkin;
        NUD_DexSkin.Value = Block.DexSkin;

        NUD_FurnitureSelected.Value = Block.SelectedFurnitureIndex;
        CHK_FurnitureSynchronized.Checked = Block.IsFurnitureSynchronized;

        for (int i = 0; i < GlobalLink5.CountItems; i++)
        {
            var itemID = Block.GetItem(i);
            var quantity = Block.GetItemQuantity(i);
            var row = DGV_Items.Rows[i];
            row.Cells[ItemNameColumnIndex].Value = (int)itemID;
            row.Cells[ItemQuantityColumnIndex].Value = quantity;
        }

        for (int i = 0; i < GlobalLink5.CountFurniture; i++)
        {
            var furniture = Block.GetFurniture(i);
            var index = i + 1;
            GetFurnitureValue(index).Value = furniture.Value;
            GetFurnitureName(index).Text = furniture.Name;
        }
    }

    private void SaveData()
    {
        var value = CAL_UploadDate.Value;
        var date = Block.UploadDate;
        if (CHK_DateSet.Checked)
            date.FromDateOnly(new DateOnly(value.Year, value.Month, value.Day));
        else
            date.SetEmpty();

        Block.UploadCount = (int)NUD_UploadCount.Value;
        Block.UploadStatus = (byte)NUD_UploadStatus.Value;
        Block.IsSlotPresent = CHK_IsSlotPresent.Checked;
        Block.IsRegistered = CHK_IsRegistered.Checked;
        Block.IsAccountFullAccess = CHK_IsFullAccess.Checked;
        Block.Musical = (byte)NUD_Musical.Value;
        Block.CGearSkin = (byte)NUD_CGearSkin.Value;
        Block.DexSkin = (byte)NUD_DexSkin.Value;

        Block.SelectedFurnitureIndex = (byte)NUD_FurnitureSelected.Value;
        Block.IsFurnitureSynchronized = CHK_FurnitureSynchronized.Checked;

        for (int i = 0; i < GlobalLink5.CountItems; i++)
        {
            var row = DGV_Items.Rows[i];
            ushort.TryParse(row.Cells[ItemNameColumnIndex].Value?.ToString(), out var itemID);
            Block.SetItem(i, itemID);
            byte.TryParse(row.Cells[ItemQuantityColumnIndex].Value?.ToString(), out var quantity);
            Block.SetItemQuantity(i, quantity);
        }

        for (int i = 0; i < GlobalLink5.CountFurniture; i++)
        {
            var furniture = Block.GetFurniture(i);
            var index = i + 1;
            furniture.Value = (ushort)GetFurnitureValue(index).Value;
            furniture.Name = GetFurnitureName(index).Text;
        }
    }

    private void InitializeItemsTab()
    {
        DGV_Items.AllowUserToAddRows = false;
        DGV_Items.AllowUserToDeleteRows = false;
        DGV_Items.AllowUserToResizeRows = false;
        DGV_Items.AutoGenerateColumns = false;
        DGV_Items.EditMode = DataGridViewEditMode.EditOnEnter;
        DGV_Items.MultiSelect = false;
        DGV_Items.RowHeadersVisible = false;
        DGV_Items.SelectionMode = DataGridViewSelectionMode.CellSelect;
        DGV_Items.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

        var spriteColumn = new DataGridViewImageColumn
        {
            Name = "Sprite",
            HeaderText = string.Empty,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            ReadOnly = true,
            ImageLayout = DataGridViewImageCellLayout.Zoom,
        };

        var itemColumn = new DataGridViewComboBoxColumn
        {
            Name = "Item",
            HeaderText = "Item",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            FlatStyle = FlatStyle.Flat,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
        };

        var items = GameInfo.FilteredSources.Items;
        itemColumn.InitializeBinding();
        itemColumn.DataSource = new BindingSource(items, string.Empty);

        var quantityColumn = new DataGridViewTextBoxColumn
        {
            Name = "Count",
            HeaderText = "Count",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            ValueType = typeof(byte),
            MaxInputLength = 3,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
        };

        DGV_Items.Columns.Add(spriteColumn);
        DGV_Items.Columns.Add(itemColumn);
        DGV_Items.Columns.Add(quantityColumn);
        DGV_Items.RowCount = GlobalLink5.CountItems;

        DGV_Items.CurrentCellDirtyStateChanged += DGV_Items_CurrentCellDirtyStateChanged;
        DGV_Items.CellValueChanged += DGV_Items_CellValueChanged;
        DGV_Items.DataError += DGV_Items_DataError;
        DGV_Items.EditingControlShowing += DGV_Items_EditingControlShowing;
    }

    private NumericUpDown GetFurnitureValue(int index) => index switch
    {
        1 => NUD_Furniture1,
        2 => NUD_Furniture2,
        3 => NUD_Furniture3,
        4 => NUD_Furniture4,
        5 => NUD_Furniture5,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    private TextBox GetFurnitureName(int index) => index switch
    {
        1 => TB_Furniture1,
        2 => TB_Furniture2,
        3 => TB_Furniture3,
        4 => TB_Furniture4,
        5 => TB_Furniture5,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    private void DGV_Items_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (DGV_Items.IsCurrentCellDirty)
            DGV_Items.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void DGV_Items_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != ItemNameColumnIndex)
            return;

        var row = DGV_Items.Rows[e.RowIndex];
        var value = row.Cells[ItemNameColumnIndex].Value;
        if (!ushort.TryParse(value?.ToString(), out var itemID))
            return;

        row.Cells[ItemSpriteColumnIndex].Value = SpriteUtil.GetItemSprite(itemID) ?? new System.Drawing.Bitmap(1, 1);
    }

    private static void DGV_Items_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.Cancel = false;
        e.ThrowException = false;
    }

    private void DGV_Items_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
    {
        if (e.Control is ComboBox combo && DGV_Items.CurrentCell?.OwningColumn is DataGridViewComboBoxColumn)
        {
            DGV_Items.BeginInvoke((MethodInvoker)(() => combo.DroppedDown = true));
            return;
        }

        if (e.Control is TextBox tb && DGV_Items.CurrentCell?.ColumnIndex == ItemQuantityColumnIndex)
            tb.SelectAll();
    }

    private void CHK_DateSet_CheckedChanged(object sender, EventArgs e) => CAL_UploadDate.Visible = CHK_DateSet.Checked;
}
