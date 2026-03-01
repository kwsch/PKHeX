using PKHeX.Core;
using System;
using System.IO;
using System.Windows.Forms;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms;

public sealed partial class SAV_Fashion9 : Form
{
    private readonly IFashionBlockEditor[] _grids;
    private readonly SaveFile SAV;
    private readonly SaveFile Origin;

    private SAV_Fashion9(SaveFile sav)
    {
        InitializeComponent();

        SAV = (Origin = sav).Clone();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        // Allow drag/drop on form and main tab control
        AllowDrop = true;
        DragEnter += Main_DragEnter;
        DragDrop += Main_DragDrop;
        TC_Features.AllowDrop = true;
        TC_Features.DragEnter += Main_DragEnter;
        TC_Features.DragDrop += Main_DragDrop;
        TC_Features.Multiline = true;

        // Create grids for each block
        if (SAV is SAV9SV sav9sv)
        {
            _grids = GetBlocks9SV(sav9sv.Blocks);
            B_SetAllOwned.Visible = false;
        }
        else if (SAV is SAV9ZA sav9ZA)
        {
            _grids = GetBlocks9ZA(sav9ZA.Blocks);
        }
        else
        {
            throw new ArgumentException("Invalid SaveFile Type");
        }

        // Translate headings
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        // Load data
        foreach (var grid in _grids)
            grid.Load();
    }

    private IFashionBlockEditor[] GetBlocks9SV(SaveBlockAccessor9SV accessor) =>
    [
        Create(accessor, SaveBlockAccessor9SV.KFashionUnlockedEyewear, nameof(SaveBlockAccessor9SV.KFashionUnlockedEyewear)),
        Create(accessor, SaveBlockAccessor9SV.KFashionUnlockedGloves, nameof(SaveBlockAccessor9SV.KFashionUnlockedGloves)),
        Create(accessor, SaveBlockAccessor9SV.KFashionUnlockedBag, nameof(SaveBlockAccessor9SV.KFashionUnlockedBag)),
        Create(accessor, SaveBlockAccessor9SV.KFashionUnlockedFootwear, nameof(SaveBlockAccessor9SV.KFashionUnlockedFootwear)),
        Create(accessor, SaveBlockAccessor9SV.KFashionUnlockedHeadwear, nameof(SaveBlockAccessor9SV.KFashionUnlockedHeadwear)),
        Create(accessor, SaveBlockAccessor9SV.KFashionUnlockedLegwear, nameof(SaveBlockAccessor9SV.KFashionUnlockedLegwear)),
        Create(accessor, SaveBlockAccessor9SV.KFashionUnlockedClothing, nameof(SaveBlockAccessor9SV.KFashionUnlockedClothing)),
        Create(accessor, SaveBlockAccessor9SV.KFashionUnlockedPhoneCase, nameof(SaveBlockAccessor9SV.KFashionUnlockedPhoneCase)),
    ];

    private IFashionBlockEditor[] GetBlocks9ZA(SaveBlockAccessor9ZA accessor) =>
    [
        Create(accessor, SaveBlockAccessor9ZA.KFashionTops, nameof(SaveBlockAccessor9ZA.KFashionTops)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionBottoms, nameof(SaveBlockAccessor9ZA.KFashionBottoms)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionAllInOne, nameof(SaveBlockAccessor9ZA.KFashionAllInOne)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionHeadwear, nameof(SaveBlockAccessor9ZA.KFashionHeadwear)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionEyewear, nameof(SaveBlockAccessor9ZA.KFashionEyewear)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionGloves, nameof(SaveBlockAccessor9ZA.KFashionGloves)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionLegwear, nameof(SaveBlockAccessor9ZA.KFashionLegwear)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionFootwear, nameof(SaveBlockAccessor9ZA.KFashionFootwear)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionSatchels, nameof(SaveBlockAccessor9ZA.KFashionSatchels)),
        Create(accessor, SaveBlockAccessor9ZA.KFashionEarrings, nameof(SaveBlockAccessor9ZA.KFashionEarrings)),

        Create(accessor, SaveBlockAccessor9ZA.KHairMake00StyleHair, nameof(SaveBlockAccessor9ZA.KHairMake00StyleHair), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake01StyleBangs, nameof(SaveBlockAccessor9ZA.KHairMake01StyleBangs), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake02ColorHair, nameof(SaveBlockAccessor9ZA.KHairMake02ColorHair), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake03ColorHair, nameof(SaveBlockAccessor9ZA.KHairMake03ColorHair), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake04ColorHair, nameof(SaveBlockAccessor9ZA.KHairMake04ColorHair), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake05StyleEyebrow, nameof(SaveBlockAccessor9ZA.KHairMake05StyleEyebrow), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake06ColorEyebrow, nameof(SaveBlockAccessor9ZA.KHairMake06ColorEyebrow), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake07StyleEyes, nameof(SaveBlockAccessor9ZA.KHairMake07StyleEyes), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake08ColorEyes, nameof(SaveBlockAccessor9ZA.KHairMake08ColorEyes), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake09StyleEyelash, nameof(SaveBlockAccessor9ZA.KHairMake09StyleEyelash), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake10ColorEyelash, nameof(SaveBlockAccessor9ZA.KHairMake10ColorEyelash), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake11Lips, nameof(SaveBlockAccessor9ZA.KHairMake11Lips), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake12BeautyMark, nameof(SaveBlockAccessor9ZA.KHairMake12BeautyMark), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake13Freckles, nameof(SaveBlockAccessor9ZA.KHairMake13Freckles), hair: true),
        Create(accessor, SaveBlockAccessor9ZA.KHairMake14DarkCircles, nameof(SaveBlockAccessor9ZA.KHairMake14DarkCircles), hair: true),
    ];

    public SAV_Fashion9(SAV9ZA sav): this((SaveFile)sav) { }

    public SAV_Fashion9(SAV9SV sav) : this((SaveFile)sav) { }

    private TabPage GetTab(string name)
    {
        var result = new TabPage(name) { Name = GetTabName(name), Text = GetTabText(name), AllowDrop = true };
        result.DragEnter += Main_DragEnter;
        result.DragDrop += Main_DragDrop;
        TC_Features.TabPages.Add(result);
        return result;
    }

    private static string GetTabText(string name)
    {
        if (name.StartsWith("KHairMake"))
            return name.Replace("KHairMake", "");
        else if (name.StartsWith("KFashionUnlocked"))
            return name.Replace("KFashionUnlocked", "");
        return name.Replace("KFashion", "");
    }

    private static string GetTabName(string name) => $"Tab_{name}";

    private IFashionBlockEditor GetEditor(TabPage selectedTab)
    {
        var name = selectedTab.Name;
        foreach (var grid in _grids)
        {
            if (GetTabName(grid.Name) == name)
                return grid;
        }
        throw new InvalidOperationException("Could not find editor for selected tab.");
    }

    private IFashionBlockEditor? TryGetEditorFromSender(object? sender)
    {
        switch (sender)
        {
            case null:
                return null;
            case DataGridView dgv:
                foreach (var ed in _grids)
                {
                    if (ReferenceEquals(ed.Grid, dgv))
                        return ed;
                }

                // if parent is a TabPage, try map via parent
                if (dgv.Parent is TabPage tp1)
                    return GetEditor(tp1);
                return null;
            case TabPage tp:
                return GetEditor(tp);
            case TabControl:
                if (TC_Features.SelectedTab is { } selected)
                    return GetEditor(selected);
                return null;
            case Control c:
                // Walk up to find a TabPage
                while (c is not TabPage and { Parent: { } parent })
                    c = parent;
                if (c is TabPage tp2)
                    return GetEditor(tp2);
                return null;
            default:
                return null;
        }
    }

    public IFashionBlockEditor Create(SCBlockAccessor accesor, uint blockLoc, string name, bool hair = false)
    {
        var tab = GetTab(name);
        // Tab already inserted at the desired position in GetTab; don't add again.
        SCBlock block = accesor.GetBlock(blockLoc);

        IFashionBlockEditor editor;
        if (accesor is SaveBlockAccessor9SV)
        {
            editor = new FashionItem9Editor(block, name, tab);
        }
        else if (accesor is SaveBlockAccessor9ZA)
        {
            editor = hair
                ? new HairMake9aEditor(block, name, tab)
                : new FashionItem9aEditor(block, name, tab);
        }
        else
        {
            throw new ArgumentException("Invalid Accessor Type");
        }

        // Enable drag/drop on the grid and route to form handlers
        editor.Grid.AllowDrop = true;
        editor.Grid.DragEnter += Main_DragEnter;
        editor.Grid.DragDrop += Main_DragDrop;

        return editor;
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        foreach (var grid in _grids)
            grid.Save();
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void Main_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e?.Data is null)
            return;
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;
        // Only allow drop if we can resolve a target editor
        if (TryGetEditorFromSender(sender) is not null)
            e.Effect = DragDropEffects.Copy;
    }

    private void Main_DragDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;

        var editor = TryGetEditorFromSender(sender) ?? (TC_Features.SelectedTab is { } tab ? GetEditor(tab) : null);
        if (editor is null)
            return;

        var size = new FileInfo(files[0]).Length;
        if (size > 0x1_0000)
        {
            WinFormsUtil.Alert("File too large to be a valid block file.");
            return;
        }
        var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, Name, "Load block to current tab?");
        if (dr != DialogResult.Yes)
            return;

        LoadBlock(editor, files[0]);
    }

    private static void LoadBlock(IFashionBlockEditor editor, string file)
    {
        var data = File.ReadAllBytes(file);
        SCBlock block = editor.Block;
        if (data.Length != block.Raw.Length)
        {
            WinFormsUtil.Alert($"File size does not match block size of {block.Raw.Length} bytes.");
            return;
        }
        block.ChangeData(data);
        editor.Load();
    }

    private void B_SetAllOwned_Click(object sender, EventArgs e)
    {
        bool state = !ModifierKeys.HasFlag(Keys.Alt);
        bool allTabs = ModifierKeys.HasFlag(Keys.Shift);
        if (allTabs)
        {
            foreach (var editor in _grids)
                editor.SetAllOwned(state);
        }
        else
        {
            var current = GetEditor(TC_Features.SelectedTab!);
            current.SetAllOwned(state);
        }
        System.Media.SystemSounds.Asterisk.Play();
    }
}

public interface IFashionBlockEditor
{
    string Name { get; }
    SCBlock Block { get; }
    DataGridView Grid { get; }
    void Load();
    void Save();
    void SetAllOwned(bool state);
}

public sealed class FashionItem9Editor : IFashionBlockEditor
{
    public SCBlock Block { get; }
    public string Name { get; }
    public DataGridView Grid { get; }

    private const string ColValue = nameof(FashionItem9.Value);
    private const string ColIsNew = nameof(FashionItem9.IsNew);

    public FashionItem9Editor(SCBlock block, string name, TabPage tab)
    {
        Block = block;
        Name = name;
        Grid = CreateGrid(tab);
    }

    private DoubleBufferedDataGridView CreateGrid(TabPage tab)
    {
        var count = Block.Raw.Length / FashionItem9.SIZE;
        var dgv = new DoubleBufferedDataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            EditMode = DataGridViewEditMode.EditOnEnter,
        };

        // Columns
        var cValue = new DataGridViewTextBoxColumn
        {
            Name = ColValue,
            HeaderText = nameof(FashionItem9.Value),
            ValueType = typeof(uint),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
        };
        dgv.Columns.Add(cValue);
        dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = ColIsNew, HeaderText = nameof(FashionItem9.IsNew) });

        dgv.RowCount = count;

        // Validation for Value column (uint)
        dgv.CellValidating += (s, e) =>
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            if (dgv.Columns[e.ColumnIndex].Name != ColValue)
                return;

            var txt = e.FormattedValue?.ToString();
            if (string.IsNullOrWhiteSpace(txt))
            {
                WinFormsUtil.Alert("Value must be a number.");
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = FashionItem9.None;
                e.Cancel = true;
                return;
            }
            if (!uint.TryParse(txt, out _))
            {
                WinFormsUtil.Alert("Value must be a number.");
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = FashionItem9.None;
                e.Cancel = true;
            }
        };
        dgv.CellEndEdit += (_, e) => dgv.Rows[e.RowIndex].ErrorText = string.Empty;

        tab.Controls.Add(dgv);
        return dgv;
    }

    public void Load()
    {
        var array = FashionItem9.GetArray(Block.Data);
        // Load hair/make items into grid from block
        for (int i = 0; i < array.Length; i++)
            LoadItem(i, array[i]);
    }

    public void Save()
    {
        var array = FashionItem9.GetArray(Block.Data);
        // Write hair/make items from grid back to block
        for (int i = 0; i < array.Length; i++)
            SaveItem(i, array[i]);
        FashionItem9.SetArray(array, Block.Data);
    }

    public void SetAllOwned(bool state) { }

    private void LoadItem(int index, FashionItem9 item)
    {
        var row = Grid.Rows[index];
        row.Cells[ColValue].Value = item.Value;
        row.Cells[ColIsNew].Value = item.IsNew;
    }

    private static bool GetBool(object? o) => o is true;

    private void SaveItem(int index, FashionItem9 item)
    {
        var row = Grid.Rows[index];

        // Value
        var txt = row.Cells[ColValue].Value?.ToString();
        if (!uint.TryParse(txt, out var value))
        {
            WinFormsUtil.Alert($"Invalid Value at row {index}. Resetting to None.");
            value = FashionItem9.None;
            row.Cells[ColValue].Value = value;
        }
        item.Value = value;

        // Flags via checkbox
        item.IsNew = GetBool(row.Cells[ColIsNew].Value);
    }
}

public sealed class FashionItem9aEditor : IFashionBlockEditor
{
    public SCBlock Block { get; }
    public string Name { get; }
    public DataGridView Grid { get; }

    private const string ColValue = nameof(FashionItem9a.Value);
    private const string ColIsNew = nameof(FashionItem9a.IsNew);
    private const string ColIsNewShop = nameof(FashionItem9a.IsNewShop);
    private const string ColIsNewGroup = nameof(FashionItem9a.IsNewGroup);
    private const string ColIsEquipped = nameof(FashionItem9a.IsEquipped);
    private const string ColIsOwned = nameof(FashionItem9a.IsOwned);

    public FashionItem9aEditor(SCBlock block, string name, TabPage tab)
    {
        Block = block;
        Name = name;
        Grid = CreateGrid(tab);
    }

    private DoubleBufferedDataGridView CreateGrid(TabPage tab)
    {
        var count = Block.Raw.Length / FashionItem9a.SIZE;
        var dgv = new DoubleBufferedDataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            EditMode = DataGridViewEditMode.EditOnEnter,
        };

        // Columns
        var cValue = new DataGridViewTextBoxColumn
        {
            Name = ColValue,
            HeaderText = nameof(FashionItem9a.Value),
            ValueType = typeof(uint),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
        };
        dgv.Columns.Add(cValue);
        dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = ColIsNew, HeaderText = nameof(FashionItem9a.IsNew) });
        dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = ColIsNewShop, HeaderText = nameof(FashionItem9a.IsNewShop) });
        dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = ColIsNewGroup, HeaderText = nameof(FashionItem9a.IsNewGroup) });
        dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = ColIsEquipped, HeaderText = nameof(FashionItem9a.IsEquipped) });
        dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = ColIsOwned, HeaderText = nameof(FashionItem9a.IsOwned) });

        dgv.RowCount = count;

        // Validation for Value column (uint)
        dgv.CellValidating += (s, e) =>
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            if (dgv.Columns[e.ColumnIndex].Name != ColValue)
                return;

            var txt = e.FormattedValue?.ToString();
            if (string.IsNullOrWhiteSpace(txt))
            {
                WinFormsUtil.Alert("Value must be a number.");
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = FashionItem9a.None;
                e.Cancel = true;
                return;
            }
            if (!uint.TryParse(txt, out _))
            {
                WinFormsUtil.Alert("Value must be a number.");
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = FashionItem9a.None;
                e.Cancel = true;
            }
        };
        dgv.CellEndEdit += (_, e) => dgv.Rows[e.RowIndex].ErrorText = string.Empty;

        tab.Controls.Add(dgv);
        return dgv;
    }

    public void Load()
    {
        var array = FashionItem9a.GetArray(Block.Data);
        // Load items into grid from block
        for (int i = 0; i < array.Length; i++)
            LoadItem(i, array[i]);
    }

    public void Save()
    {
        var array = FashionItem9a.GetArray(Block.Data);
        // Write items from grid back to block
        for (int i = 0; i < array.Length; i++)
            SaveItem(i, array[i]);
        FashionItem9a.SetArray(array, Block.Data);
    }

    public void SetAllOwned(bool state)
    {
        Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        Save();
        FashionItem9a.ModifyAll(Block.Data, z => z.IsOwned = state);
        Load();
        Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void LoadItem(int index, FashionItem9a item)
    {
        var row = Grid.Rows[index];
        row.Cells[ColValue].Value = item.Value;
        row.Cells[ColIsNew].Value = item.IsNew;
        row.Cells[ColIsNewShop].Value = item.IsNewShop;
        row.Cells[ColIsNewGroup].Value = item.IsNewGroup;
        row.Cells[ColIsEquipped].Value = item.IsEquipped;
        row.Cells[ColIsOwned].Value = item.IsOwned;
    }

    private static bool GetBool(object? o) => o is true;

    private void SaveItem(int index, FashionItem9a item)
    {
        var row = Grid.Rows[index];

        // Value
        var txt = row.Cells[ColValue].Value?.ToString();
        if (!uint.TryParse(txt, out var value))
        {
            WinFormsUtil.Alert($"Invalid Value at row {index}. Resetting to None.");
            value = FashionItem9a.None;
            row.Cells[ColValue].Value = value;
        }
        item.Value = value;

        // Flags via checkboxes
        item.IsNew = GetBool(row.Cells[ColIsNew].Value);
        item.IsNewShop = GetBool(row.Cells[ColIsNewShop].Value);
        item.IsNewGroup = GetBool(row.Cells[ColIsNewGroup].Value);
        item.IsEquipped = GetBool(row.Cells[ColIsEquipped].Value);
        item.IsOwned = GetBool(row.Cells[ColIsOwned].Value);
    }
}

public sealed record HairMake9aEditor : IFashionBlockEditor
{
    public SCBlock Block { get; }
    public string Name { get; }
    public DataGridView Grid { get; }

    private const string ColValue = nameof(HairMakeItem9a.Value);
    private const string ColIsNew = nameof(HairMakeItem9a.IsNew);

    public HairMake9aEditor(SCBlock block, string name, TabPage tab)
    {
        Block = block;
        Name = name;
        Grid = CreateGrid(tab);
    }

    private DoubleBufferedDataGridView CreateGrid(TabPage tab)
    {
        var count = Block.Raw.Length / HairMakeItem9a.SIZE;
        var dgv = new DoubleBufferedDataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            EditMode = DataGridViewEditMode.EditOnEnter,
        };

        // Columns
        var cValue = new DataGridViewTextBoxColumn
        {
            Name = ColValue,
            HeaderText = nameof(HairMakeItem9a.Value),
            ValueType = typeof(uint),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
        };
        dgv.Columns.Add(cValue);
        dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = ColIsNew, HeaderText = nameof(HairMakeItem9a.IsNew) });

        dgv.RowCount = count;

        // Validation for Value column (uint)
        dgv.CellValidating += (s, e) =>
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            if (dgv.Columns[e.ColumnIndex].Name != ColValue)
                return;

            var txt = e.FormattedValue?.ToString();
            if (string.IsNullOrWhiteSpace(txt))
            {
                WinFormsUtil.Alert("Value must be a number.");
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = HairMakeItem9a.None;
                e.Cancel = true;
                return;
            }
            if (!uint.TryParse(txt, out _))
            {
                WinFormsUtil.Alert("Value must be a number.");
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = HairMakeItem9a.None;
                e.Cancel = true;
            }
        };
        dgv.CellEndEdit += (_, e) => dgv.Rows[e.RowIndex].ErrorText = string.Empty;

        tab.Controls.Add(dgv);
        return dgv;
    }

    public void Load()
    {
        var array = HairMakeItem9a.GetArray(Block.Data);
        // Load hair/make items into grid from block
        for (int i = 0; i < array.Length; i++)
            LoadHairMake(i, array[i]);
    }

    public void Save()
    {
        var array = HairMakeItem9a.GetArray(Block.Data);
        // Write hair/make items from grid back to block
        for (int i = 0; i < array.Length; i++)
            SaveHairMake(i, array[i]);
        HairMakeItem9a.SetArray(array, Block.Data);
    }

    public void SetAllOwned(bool state) { }

    private void LoadHairMake(int index, HairMakeItem9a item)
    {
        var row = Grid.Rows[index];
        row.Cells[ColValue].Value = item.Value;
        row.Cells[ColIsNew].Value = item.IsNew;
    }

    private static bool GetBool(object? o) => o is true;

    private void SaveHairMake(int index, HairMakeItem9a item)
    {
        var row = Grid.Rows[index];

        // Value
        var txt = row.Cells[ColValue].Value?.ToString();
        if (!uint.TryParse(txt, out var value))
        {
            WinFormsUtil.Alert($"Invalid Value at row {index}. Resetting to None.");
            value = HairMakeItem9a.None;
            row.Cells[ColValue].Value = value;
        }
        item.Value = value;

        // Flags via checkbox
        item.IsNew = GetBool(row.Cells[ColIsNew].Value);
    }
}
