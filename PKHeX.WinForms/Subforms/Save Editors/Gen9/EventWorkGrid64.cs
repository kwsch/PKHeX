using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms;

public interface IEventWorkGrid
{
    void Load();
    void Save();
}

/// <summary>
/// Immutable helper for name/hash lookup and reverse lookup.
/// </summary>
public sealed class EventWorkLookup(Dictionary<ulong, string> forward)
{
    private readonly Dictionary<string, ulong> _reverse = forward.ToDictionary(z => z.Value, z => z.Key);

    public string GetName(ulong hash)
    {
        if (forward.TryGetValue(hash, out var name))
            return name;
        return hash.ToString("X16");
    }

    public ulong GetHash(string nameOrHex)
    {
        if (_reverse.TryGetValue(nameOrHex, out var h))
            return h;
        if (ulong.TryParse(nameOrHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsed))
            return parsed;
        return FnvHash.HashFnv1a_64(nameOrHex);
    }
}

/// <summary>
/// Base class with shared UI helpers and wiring for EventWork grids.
/// </summary>
public abstract record EventWorkGridBase(SplitContainer Container, DataGridView Grid, TextBox Search)
    : IEventWorkGrid
{
    protected static (SplitContainer Container, TextBox Search) CreateContainerCommon()
    {
        var container = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            FixedPanel = FixedPanel.Panel1,
            IsSplitterFixed = true,
            Margin = new Padding(0),
        };

        var search = new TextBox
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            PlaceholderText = "Search...",
        };
        container.Panel1.Controls.Add(search);

        var topHeight = Math.Max(search.PreferredHeight + 4, 24);
        container.Panel1MinSize = topHeight;
        container.SplitterDistance = topHeight;
        return (container, search);
    }

    protected static DataGridViewTextBoxColumn MakeIndexColumn(Font font) => new()
    {
        Name = "Index",
        HeaderText = "Index",
        DefaultCellStyle = new DataGridViewCellStyle { Font = font },
        Width = 40,
        ReadOnly = true,
    };

    protected static DataGridViewTextBoxColumn MakeKeyColumn(Font font, string title = "Name") => new()
    {
        Name = title,
        HeaderText = title,
        DefaultCellStyle = new DataGridViewCellStyle
        {
            Font = font,
            Alignment = DataGridViewContentAlignment.MiddleLeft,
        },
    };

    protected static DataGridViewTextBoxColumn MakeValueNumberColumn(Font font, string title) => new()
    {
        Name = title,
        HeaderText = title,
        DefaultCellStyle = new DataGridViewCellStyle
        {
            Font = font,
            Alignment = DataGridViewContentAlignment.MiddleRight,
            NullValue = 0UL,
        },
    };

    protected static DataGridViewCheckBoxColumn MakeValueBoolColumn(Font font, string title) => new()
    {
        Name = title,
        HeaderText = title,
        DefaultCellStyle = new DataGridViewCellStyle
        {
            Font = font,
            Alignment = DataGridViewContentAlignment.MiddleCenter,
            NullValue = false,
        },
    };

    public abstract void Load();
    public abstract void Save();
}

/// <summary>
/// Helper that creates and manages a DataGridView for editing an <see cref="EventWorkStorage64{T}"/> block.
/// </summary>
/// <typeparam name="T">Value type for the storage. Use bool for flags, ulong for values.</typeparam>
public sealed record EventWorkGrid64<T> : EventWorkGridBase where T : struct, IEquatable<T>
{
    private readonly EventWorkStorage64<T> Storage;
    private readonly EventWorkLookup Names;

    private const int ColumnIndex = 0;
    private const int ColumnValue = 1;
    private const int ColumnName = 2;

    private EventWorkGrid64(EventWorkStorage64<T> storage, EventWorkLookup names, SplitContainer container, DataGridView grid, TextBox search)
        : base(container, grid, search)
    {
        Storage = storage;
        Names = names;

        if (typeof(T) == typeof(ulong))
            Grid.CellValidated += ValidateU64;

        Search.TextChanged += (_, _) => ApplyFilter(Search.Text);

        ToggleAutoSize(true);
    }

    private void ToggleAutoSize(bool state)
    {
        var columns = Grid.Columns;
        columns[ColumnIndex].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.ColumnHeader : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnValue].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnName].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None;
    }

    private void ValidateU64(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex != ColumnValue)
            return;
        var row = Grid.Rows[e.RowIndex];
        var cell = row.Cells[e.ColumnIndex];
        var text = cell.Value?.ToString();
        if (ulong.TryParse(text, CultureInfo.InvariantCulture, out _))
            return;

        WinFormsUtil.Alert("Please enter a valid unsigned integer value.");
        var index = Convert.ToInt32(row.Cells[ColumnIndex].Value);
        var saved = Storage.GetValue(index);
        cell.Value = saved.ToString();
    }

    public static EventWorkGrid64<bool> CreateFlags(TabPage host, EventWorkFlagStorage storage, EventWorkLookup names)
    {
        var (container, search) = CreateContainerCommon();
        host.Controls.Clear();
        host.Controls.Add(container);

        var font = new Font("Courier New", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        var dgv = new DoubleBufferedDataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AllowUserToResizeRows = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            Dock = DockStyle.Fill,
            RowHeadersVisible = false,
            Margin = new Padding(0),
            DefaultCellStyle = new DataGridViewCellStyle { Font = font },
        };
        dgv.Columns.AddRange(MakeIndexColumn(font), MakeValueBoolColumn(font, "Value"), MakeKeyColumn(font));
        container.Panel2.Controls.Add(dgv);
        return new EventWorkGrid64<bool>(storage, names, container, dgv, search);
    }

    public static EventWorkGrid64<ulong> CreateValues(TabPage host, EventWorkValueStorage storage, EventWorkLookup names)
    {
        var (container, search) = CreateContainerCommon();
        host.Controls.Clear();
        host.Controls.Add(container);

        var font = new Font("Courier New", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        var dgv = new DoubleBufferedDataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AllowUserToResizeRows = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            Dock = DockStyle.Fill,
            RowHeadersVisible = false,
            Margin = new Padding(0),
            DefaultCellStyle = new DataGridViewCellStyle { Font = font },
        };
        dgv.Columns.AddRange(MakeIndexColumn(font), MakeValueNumberColumn(font, "Value"), MakeKeyColumn(font));
        container.Panel2.Controls.Add(dgv);
        return new EventWorkGrid64<ulong>(storage, names, container, dgv, search);
    }

    public override void Load()
    {
        var rows = Grid.Rows;
        rows.Clear();

        var count = Storage.Count;
        rows.Add(count);
        for (int i = 0; i < count; i++)
        {
            var hash = Storage.GetKey(i);
            var name = Names.GetName(hash);
            var value = Storage.GetValue(i);

            var row = rows[i];
            var cells = row.Cells;
            cells[ColumnIndex].Value = i;
            cells[ColumnName].Value = name;
            cells[ColumnValue].Value = value;
        }
    }

    private void ApplyFilter(string text)
    {
        ToggleAutoSize(false);
        bool has = text.Length != 0;
        foreach (DataGridViewRow r in Grid.Rows)
        {
            if (!has)
            {
                r.Visible = true;
                continue;
            }

            var name = r.Cells[ColumnName].Value?.ToString() ?? string.Empty;
            r.Visible = name.Contains(text, StringComparison.OrdinalIgnoreCase);
        }
        ToggleAutoSize(true);
    }

    public override void Save()
    {
        var rows = Grid.Rows;
        var count = Math.Min(Storage.Count, rows.Count);

        for (int i = 0; i < count; i++)
        {
            var cells = rows[i].Cells;
            try
            {
                var name = cells[ColumnName].Value?.ToString() ?? string.Empty;
                var hash = Names.GetHash(name.Trim());
                var valObj = cells[ColumnValue].Value;

                if (typeof(T) == typeof(bool))
                {
                    var v = Convert.ToBoolean(valObj);
                    Storage.SetValue(i, (T)(object)v);
                }
                else if (typeof(T) == typeof(ulong))
                {
                    var v = Convert.ToUInt64(valObj);
                    Storage.SetValue(i, (T)(object)v);
                }
                else
                {
                    Storage.SetValue(i, (T)valObj!);
                }
                Storage.SetKey(i, hash);
            }
            catch
            {
                // Ignore invalid row
            }
        }
        Storage.Compress();
    }
}

/// <summary>
/// Grid for EventWorkStorage128 with one key columns and two values.
/// </summary>
public sealed record EventWorkGridTuple : EventWorkGridBase
{
    private readonly EventWorkValueStorageKey128 Storage;
    private readonly EventWorkLookup Names;

    private const int ColumnIndex = 0;
    private const int ColumnKey = 1;
    private const int ColumnValue1 = 2;
    private const int ColumnValue2 = 3;

    private EventWorkGridTuple(EventWorkValueStorageKey128 storage, EventWorkLookup names, SplitContainer container, DataGridView grid, TextBox search)
        : base(container, grid, search)
    {
        Storage = storage;
        Names = names;

        Grid.CellValueChanged += ValidateCell;
        Search.TextChanged += (_, _) => ApplyFilter(Search.Text);
        ToggleAutoSize(true);
    }

    public static EventWorkGridTuple CreateValues(TabPage host, EventWorkValueStorageKey128 storage, EventWorkLookup names)
    {
        var (container, search) = CreateContainerCommon();
        host.Controls.Clear();
        host.Controls.Add(container);

        var font = new Font("Courier New", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        var dgv = new DoubleBufferedDataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AllowUserToResizeRows = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            Dock = DockStyle.Fill,
            RowHeadersVisible = false,
            Margin = new Padding(0),
            DefaultCellStyle = new DataGridViewCellStyle { Font = font },
        };
        var c1 = MakeIndexColumn(font);
        var c2 = MakeKeyColumn(font, "Key A");
        var v1 = MakeValueNumberColumn(font, "Value 1");
        var v2 = MakeValueNumberColumn(font, "Value 2");

        dgv.Columns.AddRange(c1, c2, v1, v2);
        container.Panel2.Controls.Add(dgv);
        return new EventWorkGridTuple(storage, names, container, dgv, search);
    }

    private void ToggleAutoSize(bool state)
    {
        var columns = Grid.Columns;
        columns[ColumnIndex].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.ColumnHeader : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnKey].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnValue1].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnValue2].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
    }

    private void ValidateCell(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex is ColumnValue1 or ColumnValue2)
        {
            var row = Grid.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            var text = cell.Value?.ToString() ?? string.Empty;
            if (e.ColumnIndex is ColumnValue1 && long.TryParse(text, CultureInfo.InvariantCulture, out _))
                return;
            if (e.ColumnIndex is ColumnValue2 && ulong.TryParse(text, CultureInfo.InvariantCulture, out _))
                return;
            WinFormsUtil.Alert("Please enter a valid value.");
            var i = Convert.ToInt32(row.Cells[ColumnIndex].Value);
            var (a, b) = Storage.GetKey(i);
            if (e.ColumnIndex == ColumnKey)
                cell.Value = Names.GetName(a);
            else if (e.ColumnIndex == ColumnValue1)
                cell.Value = ((long)b).ToString();
            else
                cell.Value = Storage.GetValue(i);
        }
    }

    private void ApplyFilter(string text)
    {
        ToggleAutoSize(false);
        bool has = text.Length != 0;
        foreach (DataGridViewRow r in Grid.Rows)
        {
            if (!has)
            {
                r.Visible = true;
                continue;
            }
            var a = r.Cells[ColumnKey].Value?.ToString() ?? string.Empty;
            r.Visible = a.Contains(text, StringComparison.OrdinalIgnoreCase);
        }
        ToggleAutoSize(true);
    }

    public override void Load()
    {
        var rows = Grid.Rows;
        rows.Clear();

        var count = Storage.Count;
        rows.Add(count);
        for (int i = 0; i < count; i++)
        {
            var (a, b) = Storage.GetKey(i);
            var value = Storage.GetValue(i);

            var row = rows[i];
            var cells = row.Cells;
            cells[ColumnIndex].Value = i;
            cells[ColumnKey].Value = Names.GetName(a);
            cells[ColumnValue1].Value = ((long)b).ToString();
            cells[ColumnValue2].Value = value.ToString();
        }
    }

    public override void Save()
    {
        var rows = Grid.Rows;
        var count = Math.Min(Storage.Count, rows.Count);

        for (int i = 0; i < count; i++)
        {
            var cells = rows[i].Cells;
            try
            {
                var aText = cells[ColumnKey].Value?.ToString() ?? string.Empty;
                var vText1 = cells[ColumnValue1].Value?.ToString() ?? "0";
                var vText2 = cells[ColumnValue2].Value?.ToString() ?? "0";
                ulong a = Names.GetHash(aText);
                ulong b = long.TryParse(vText1, CultureInfo.InvariantCulture, out var bt) ? (ulong)bt : 0UL;
                ulong v = ulong.TryParse(vText2, CultureInfo.InvariantCulture, out var vt) ? vt : 0UL;

                Storage.SetValue(i, v);
                Storage.SetKey(i, a, b);
            }
            catch
            {
                // Ignore invalid row
            }
        }
        Storage.Compress();
    }
}

/// <summary>
/// Grid for EventWorkStorage192 with separate key columns A, B, and C, and one value.
/// </summary>
public sealed record EventWorkGrid128 : EventWorkGridBase
{
    private readonly EventWorkValueStorageKey128 Storage;
    private readonly EventWorkLookup Names;

    private const int ColumnIndex = 0;
    private const int ColumnKeyA = 1;
    private const int ColumnKeyB = 2;
    private const int ColumnValue = 3;

    private EventWorkGrid128(EventWorkValueStorageKey128 storage, EventWorkLookup names, SplitContainer container, DataGridView grid, TextBox search)
        : base(container, grid, search)
    {
        Storage = storage;
        Names = names;

        Grid.CellValidated += ValidateValue;
        Search.TextChanged += (_, _) => ApplyFilter(Search.Text);
        ToggleAutoSize(true);
    }

    public static EventWorkGrid128 CreateValues(TabPage host, EventWorkValueStorageKey128 storage, EventWorkLookup names)
    {
        var (container, search) = CreateContainerCommon();
        host.Controls.Clear();
        host.Controls.Add(container);

        var font = new Font("Courier New", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        var dgv = new DoubleBufferedDataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AllowUserToResizeRows = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            Dock = DockStyle.Fill,
            RowHeadersVisible = false,
            Margin = new Padding(0),
            DefaultCellStyle = new DataGridViewCellStyle { Font = font },
        };

        var c1 = MakeIndexColumn(font);
        var c2 = MakeKeyColumn(font, "Key A");
        var c3 = MakeKeyColumn(font, "Key B");
        var v1 = MakeValueNumberColumn(font, "Value");

        dgv.Columns.AddRange(c1, c2, c3, v1);
        container.Panel2.Controls.Add(dgv);
        return new EventWorkGrid128(storage, names, container, dgv, search);
    }

    private void ToggleAutoSize(bool state)
    {
        var columns = Grid.Columns;
        columns[ColumnIndex].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.ColumnHeader : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnKeyA].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnKeyB].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnValue].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
    }

    private void ValidateValue(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex is ColumnValue)
        {
            var row = Grid.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            var text = cell.Value?.ToString() ?? string.Empty;
            if (ulong.TryParse(text, CultureInfo.InvariantCulture, out _))
                return;
            WinFormsUtil.Alert("Please enter a valid value.");
            var i = Convert.ToInt32(row.Cells[ColumnIndex].Value);
            cell.Value = Storage.GetValue(i);
        }
    }

    private void ApplyFilter(string text)
    {
        ToggleAutoSize(false);
        bool has = text.Length != 0;
        foreach (DataGridViewRow r in Grid.Rows)
        {
            if (!has)
            {
                r.Visible = true;
                continue;
            }
            var a = r.Cells[ColumnKeyA].Value?.ToString() ?? string.Empty;
            var b = r.Cells[ColumnKeyB].Value?.ToString() ?? string.Empty;
            r.Visible = a.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                        b.Contains(text, StringComparison.OrdinalIgnoreCase);
        }
        ToggleAutoSize(true);
    }

    public override void Load()
    {
        var rows = Grid.Rows;
        rows.Clear();

        var count = Storage.Count;
        rows.Add(count);
        for (int i = 0; i < count; i++)
        {
            var (a, b) = Storage.GetKey(i);
            var value = Storage.GetValue(i);

            var row = rows[i];
            var cells = row.Cells;
            cells[ColumnIndex].Value = i;
            cells[ColumnKeyA].Value = Names.GetName(a);
            cells[ColumnKeyB].Value = Names.GetName(b);
            cells[ColumnValue].Value = value.ToString();
        }
    }

    public override void Save()
    {
        var rows = Grid.Rows;
        var count = Math.Min(Storage.Count, rows.Count);

        for (int i = 0; i < count; i++)
        {
            var cells = rows[i].Cells;
            try
            {
                var aText = cells[ColumnKeyA].Value?.ToString() ?? string.Empty;
                var bText = cells[ColumnKeyB].Value?.ToString() ?? string.Empty;
                var vText = cells[ColumnValue].Value?.ToString() ?? "0";
                ulong a = Names.GetHash(aText);
                ulong b = Names.GetHash(bText);
                ulong v = ulong.TryParse(vText, CultureInfo.InvariantCulture, out var vt) ? vt : 0UL;

                Storage.SetValue(i, v);
                Storage.SetKey(i, a, b);
            }
            catch
            {
                // Ignore invalid row
            }
        }
        Storage.Compress();
    }
}

/// <summary>
/// Grid for EventWorkStorage192 with separate key columns A, B, and C, and one value.
/// </summary>
public sealed record EventWorkGrid192 : EventWorkGridBase
{
    private readonly EventWorkValueStorageKey192 Storage;
    private readonly EventWorkLookup Names;

    private const int ColumnIndex = 0;
    private const int ColumnKeyA = 1;
    private const int ColumnKeyB = 2;
    private const int ColumnKeyC = 3;
    private const int ColumnValue = 4;

    private EventWorkGrid192(EventWorkValueStorageKey192 storage, EventWorkLookup names, SplitContainer container, DataGridView grid, TextBox search)
        : base(container, grid, search)
    {
        Storage = storage;
        Names = names;

        Grid.CellValidated += ValidateValue;
        Search.TextChanged += (_, _) => ApplyFilter(Search.Text);
        ToggleAutoSize(true);
    }

    public static EventWorkGrid192 CreateValues(TabPage host, EventWorkValueStorageKey192 storage, EventWorkLookup names)
    {
        var (container, search) = CreateContainerCommon();
        host.Controls.Clear();
        host.Controls.Add(container);

        var font = new Font("Courier New", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        var dgv = new DoubleBufferedDataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = true,
            AllowUserToResizeRows = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            Dock = DockStyle.Fill,
            RowHeadersVisible = false,
            Margin = new Padding(0),
            DefaultCellStyle = new DataGridViewCellStyle { Font = font },
        };

        var c1 = MakeIndexColumn(font);
        var c2 = MakeKeyColumn(font, "Key A");
        var c3 = MakeKeyColumn(font, "Key B");
        var c4 = MakeKeyColumn(font, "Key C");
        var v1 = MakeValueNumberColumn(font, "Value");

        dgv.Columns.AddRange(c1, c2, c3, c4, v1);
        container.Panel2.Controls.Add(dgv);
        return new EventWorkGrid192(storage, names, container, dgv, search);
    }

    private void ToggleAutoSize(bool state)
    {
        var columns = Grid.Columns;
        columns[ColumnIndex].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.ColumnHeader : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnKeyA].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnKeyB].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnKeyC].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
        columns[ColumnValue].AutoSizeMode = state ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.None;
    }

    private void ValidateValue(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex is ColumnValue)
        {
            var row = Grid.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];
            var text = cell.Value?.ToString() ?? string.Empty;
            if (ulong.TryParse(text, CultureInfo.InvariantCulture, out _))
                return;
            WinFormsUtil.Alert("Please enter a valid value.");
            var i = Convert.ToInt32(row.Cells[ColumnIndex].Value);
            cell.Value = Storage.GetValue(i);
        }
    }

    private void ApplyFilter(string text)
    {
        ToggleAutoSize(false);
        bool has = text.Length != 0;
        foreach (DataGridViewRow r in Grid.Rows)
        {
            if (!has)
            {
                r.Visible = true;
                continue;
            }
            var a = r.Cells[ColumnKeyA].Value?.ToString() ?? string.Empty;
            var b = r.Cells[ColumnKeyB].Value?.ToString() ?? string.Empty;
            var c = r.Cells[ColumnKeyC].Value?.ToString() ?? string.Empty;
            r.Visible = a.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                        b.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                        c.Contains(text, StringComparison.OrdinalIgnoreCase);
        }
        ToggleAutoSize(true);
    }

    public override void Load()
    {
        var rows = Grid.Rows;
        rows.Clear();

        var count = Storage.Count;
        rows.Add(count);
        for (int i = 0; i < count; i++)
        {
            var (a, b, c) = Storage.GetKey(i);
            var value = Storage.GetValue(i);

            var row = rows[i];
            var cells = row.Cells;
            cells[ColumnIndex].Value = i;
            cells[ColumnKeyA].Value = Names.GetName(a);
            cells[ColumnKeyB].Value = Names.GetName(b);
            cells[ColumnKeyC].Value = Names.GetName(c);
            cells[ColumnValue].Value = value.ToString();
        }
    }

    public override void Save()
    {
        var rows = Grid.Rows;
        var count = Math.Min(Storage.Count, rows.Count);

        for (int i = 0; i < count; i++)
        {
            var cells = rows[i].Cells;
            try
            {
                var aText = cells[ColumnKeyA].Value?.ToString() ?? string.Empty;
                var bText = cells[ColumnKeyB].Value?.ToString() ?? string.Empty;
                var cText = cells[ColumnKeyC].Value?.ToString() ?? string.Empty;
                var vText = cells[ColumnValue].Value?.ToString() ?? "0";
                ulong a = Names.GetHash(aText);
                ulong b = Names.GetHash(bText);
                ulong c = Names.GetHash(cText);
                ulong v = ulong.TryParse(vText, CultureInfo.InvariantCulture, out var vt) ? vt : 0UL;

                Storage.SetValue(i, v);
                Storage.SetKey(i, a, b, c);
            }
            catch
            {
                // Ignore invalid row
            }
        }
        Storage.Compress();
    }
}
