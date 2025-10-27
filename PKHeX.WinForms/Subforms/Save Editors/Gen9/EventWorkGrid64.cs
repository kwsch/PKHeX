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
/// Helper that creates and manages a DataGridView for editing an <see cref="EventWorkStorage64{T}"/> block.
/// </summary>
/// <typeparam name="T">Value type for the storage. Use bool for flags, ulong for values.</typeparam>
public sealed class EventWorkGrid64<T> : IEventWorkGrid where T : struct, IEquatable<T>
{
    private readonly EventWorkStorage64<T> Storage;
    private readonly Dictionary<ulong, string> Lookup;
    private readonly DoubleBufferedDataGridView Grid;
    private readonly SplitContainer Container;
    private readonly TextBox SearchBox;

    private const int ColumnIndex = 0;
    private const int ColumnValue = 1;
    private const int ColumnName = 2;

    private EventWorkGrid64(EventWorkStorage64<T> storage, Dictionary<ulong, string> lookup, SplitContainer container, DoubleBufferedDataGridView grid, TextBox search)
    {
        Storage = storage;
        Lookup = lookup;
        Container = container;
        Grid = grid;
        SearchBox = search;

        if (typeof(T) == typeof(ulong))
            Grid.CellValidated += ValidateU64;

        SearchBox.TextChanged += (_, _) => ApplyFilter(SearchBox.Text);

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
        if (ulong.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
            return;

        WinFormsUtil.Alert("Please enter a valid unsigned integer value.");
        // Revert to the original stored value.
        // Rather than use RowIndex, which may have changed due to sorting, use the Index cell value.
        var index = Convert.ToInt32(row.Cells[ColumnIndex].Value);
        var saved = Storage.GetValue(index);
        cell.Value = saved.ToString();
    }

    public static EventWorkGrid64<bool> CreateFlags(TabPage host, EventWorkFlagStorage storage, Dictionary<ulong, string> lookup)
    {
        var (container, grid, search) = CreateContainer(true);
        host.Controls.Clear();
        host.Controls.Add(container);
        return new EventWorkGrid64<bool>(storage, lookup, container, grid, search);
    }

    public static EventWorkGrid64<ulong> CreateValues(TabPage host, EventWorkValueStorage storage, Dictionary<ulong, string> lookup)
    {
        var (container, grid, search) = CreateContainer(false);
        host.Controls.Clear();
        host.Controls.Add(container);
        return new EventWorkGrid64<ulong>(storage, lookup, container, grid, search);
    }

    private static (SplitContainer Container, DoubleBufferedDataGridView Grid, TextBox Search) CreateContainer(bool isBoolean)
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

        // Default row font to Courier New (match designer)
        var font = new Font("Courier New", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        var grid = CreateGrid(isBoolean, font);

        container.Panel1.Controls.Add(search);
        container.Panel2.Controls.Add(grid);

        // Size the top panel to the search box height, leave rest for grid.
        var topHeight = Math.Max(search.PreferredHeight + 4, 24);
        container.Panel1MinSize = topHeight;
        container.SplitterDistance = topHeight;

        return (container, grid, search);
    }

    private static DoubleBufferedDataGridView CreateGrid(bool isBoolean, Font font)
    {
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
            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = SystemColors.ControlLight },
        };

        var colIndex = GetColumnIndex("Index", font);
        var colName = GetColumnName("Name", font);
        var colValue = GetColumnValue("Value", font, isBoolean);
        dgv.Columns.AddRange(colIndex, colValue, colName);
        return dgv;
    }

    private static DataGridViewTextBoxColumn GetColumnIndex(string title, Font font) => new()
    {
        Name = title,
        HeaderText = title,
        DefaultCellStyle = new DataGridViewCellStyle { Font = font },
        Width = 40,
        ReadOnly = true,
    };

    private static DataGridViewTextBoxColumn GetColumnName(string title, Font font) => new()
    {
        Name = title,
        HeaderText = title,
        DefaultCellStyle = new DataGridViewCellStyle { Font = font },
    };

    private static DataGridViewColumn GetColumnValue(string title, Font font, bool isBoolean)
    {
        if (isBoolean)
            return GetValueColumnBool(title, font);
        return GetValueColumnNumber(title, font);
    }

    private static DataGridViewTextBoxColumn GetValueColumnNumber(string title, Font font) => new()
    {
        Name = title,
        HeaderText = title,
        DefaultCellStyle = new DataGridViewCellStyle
        {
            Font = font,
            Alignment = DataGridViewContentAlignment.MiddleRight,
            NullValue = 0,
        },
    };

    private static DataGridViewCheckBoxColumn GetValueColumnBool(string title, Font font) => new()
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

    public void Load()
    {
        var rows = Grid.Rows;
        rows.Clear();

        var count = Storage.Count;
        rows.Add(count);
        for (int i = 0; i < count; i++)
        {
            var hash = Storage.GetKey(i);
            var name = GetNameDisplay(hash);
            var value = Storage.GetValue(i);

            var row = rows[i];
            var cells = row.Cells;
            cells[ColumnIndex].Value = i;
            cells[ColumnName].Value = name;
            cells[ColumnValue].Value = value;
        }
    }

    private string GetNameDisplay(ulong hash) => GetNameDisplay(hash, Lookup);

    public static string GetNameDisplay(ulong hash, IReadOnlyDictionary<ulong, string> lookup)
    {
        if (lookup.TryGetValue(hash, out var name))
            return name;
        if (hash == FnvHash.HashEmpty)
            return string.Empty;
        return hash.ToString("X16");
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

    public void Save()
    {
        var rows = Grid.Rows;
        var count = Math.Min(Storage.Count, rows.Count);

        for (int i = 0; i < count; i++)
        {
            var cells = rows[i].Cells;
            try
            {
                var name = cells[ColumnName].Value?.ToString() ?? string.Empty;
                var hash = GetHash(name.Trim());
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
                    // Should not happen for current use cases.
                    Storage.SetValue(i, (T)valObj!);
                }
                // Set hash last so that the entry only updates if value parsing succeeded.
                Storage.SetKey(i, hash);
            }
            catch
            {
                // Ignore invalid row
            }
        }
        Storage.Compress(); // Ensure the user didn't sneak in empty rows. There should be no gaps.
    }

    private ulong GetHash(string name)
    {
        // Generally, the Lookup will be empty. The user can enter an arbitrary flag name string or hash, and we should recognize both.
        // If a recognized name is entered, use the known hash.
        // If an unrecognized name is entered, check if it's a hex number and use that as the hash.
        // Otherwise, just consider it as a name and hash it.

        // O(n) reverse lookup is acceptable for a Saving operation.
        var match = Lookup.FirstOrDefault(z => z.Value == name);
        if (match.Value != null) // default if not found
            return match.Key;

        if (ulong.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hash))
            return hash;

        return FnvHash.HashFnv1a_64(name);
    }
}
