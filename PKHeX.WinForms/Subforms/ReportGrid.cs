using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class ReportGrid : Form
{
    public ReportGrid()
    {
        InitializeComponent();
        CenterToParent();
        GetContextMenu();
    }

    private void GetContextMenu()
    {
        var mnuHide = new ToolStripMenuItem { Name = "mnuHide", Text = MsgReportColumnHide };
        mnuHide.Click += (_, _) =>
        {
            int c = dgData.SelectedCells.Count;
            if (c == 0)
            { WinFormsUtil.Alert(MsgReportColumnHideFail); return; }

            for (int i = 0; i < c; i++)
                dgData.Columns[dgData.SelectedCells[i].ColumnIndex].Visible = false;
        };
        var mnuRestore = new ToolStripMenuItem { Name = "mnuRestore", Text = MsgReportColumnRestore };
        mnuRestore.Click += (_, _) =>
        {
            int c = dgData.ColumnCount;
            for (int i = 0; i < c; i++)
                dgData.Columns[i].Visible = true;

            WinFormsUtil.Alert(MsgReportColumnRestoreSuccess);
        };

        ContextMenuStrip mnu = new();
        mnu.Items.Add(mnuHide);
        mnu.Items.Add(mnuRestore);

        dgData.ContextMenuStrip = mnu;
    }

    private sealed class PokemonList<T> : SortableBindingList<T> where T : class;

    public void PopulateData(IReadOnlyList<SlotCache> data) => PopulateData(data, [], []);

    public void PopulateData(IReadOnlyList<SlotCache> data, ReadOnlySpan<string> extra, ReadOnlySpan<string> hide)
    {
        SuspendLayout();
        var PL = new PokemonList<EntitySummaryImage>();
        var strings = GameInfo.Strings;
        foreach (var entry in data)
        {
            var pk = entry.Entity;
            if (pk.Species - 1u >= pk.MaxSpeciesID)
            {
                continue;
            }
            pk.Stat_Level = pk.CurrentLevel; // recalc Level
            PL.Add(new EntitySummaryImage(pk, strings, entry.Identify()));
        }

        dgData.DataSource = PL;
        dgData.AutoGenerateColumns = true;

        if (hide.Length != 0)
            HideSpecifiedColumns(hide);
        if (extra.Length != 0)
            AddExtraColumns(PL, extra);

        for (int i = 0; i < dgData.Columns.Count; i++)
        {
            var col = dgData.Columns[i];
            if (col is DataGridViewImageColumn)
                continue; // Don't add sorting for Sprites
            col.SortMode = DataGridViewColumnSortMode.Automatic;
        }

        // Trigger Resizing
        dgData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        for (int i = 0; i < dgData.Columns.Count; i++)
        {
            int w = dgData.Columns[i].Width;
            dgData.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgData.Columns[i].Width = w;
        }
        dgData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        Data_Sorted(this, EventArgs.Empty); // trigger row resizing

        ResumeLayout();
    }

    private void HideSpecifiedColumns(ReadOnlySpan<string> hide)
    {
        foreach (var prop in hide)
        {
            if (prop.Length == 0)
                continue;
            var col = dgData.Columns[prop];
            if (col != null)
                col.Visible = false;
        }
    }

    private void AddExtraColumns(PokemonList<EntitySummaryImage> data, ReadOnlySpan<string> extra)
    {
        var rent = ArrayPool<string>.Shared.Rent(data.Count);
        var span = rent.AsSpan(0, data.Count);
        foreach (var prop in extra)
        {
            if (prop.Length == 0)
                continue;
            span.Clear();
            bool any = false;
            for (int i = 0; i < data.Count; i++)
            {
                var pk = data[i].Entity;
                if (!TryGetCustomCell(pk, prop, out var str))
                    continue;
                span[i] = str;
                any = true;
            }

            if (!any)
                continue;

            var col = new DataGridViewTextBoxColumn { Name = prop, HeaderText = prop };
            var c = dgData.Columns.Add(col);
            for (int i = 0; i < data.Count; i++)
                dgData.Rows[i].Cells[c].Value = span[i];
        }
        ArrayPool<string>.Shared.Return(rent, true);
    }

    public IPropertyProvider PropertyProvider { get; init; } = DefaultPropertyProvider.Instance;

    private bool TryGetCustomCell(PKM pk, string prop, [NotNullWhen(true)] out string? result)
    {
        if (PropertyProvider.TryGetProperty(pk, prop, out result))
            return true;
        return false;
    }

    private void Data_Sorted(object sender, EventArgs e)
    {
        int height = SpriteUtil.Spriter.Height + 1; // max height of a row, +1px
        for (int i = 0; i < dgData.Rows.Count; i++)
            dgData.Rows[i].Height = height;
    }

    private void PromptSaveCSV(object sender, FormClosingEventArgs e)
    {
        if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgReportExportCSV) != DialogResult.Yes)
            return;
        using var savecsv = new SaveFileDialog();
        savecsv.Filter = "Spreadsheet|*.csv";
        savecsv.FileName = "Box Data Dump.csv";
        if (savecsv.ShowDialog() == DialogResult.OK)
        {
            Hide();
            var path = savecsv.FileName;
            var t = Task.Run(() => Export_CSV(path));
            t.Wait(); // don't start disposing until the saving is complete
        }
    }

    private async Task Export_CSV(string path)
    {
        await using var fs = new FileStream(path, FileMode.Create);
        await using var s = new StreamWriter(fs, new UTF8Encoding(false));

        var headers = dgData.Columns.Cast<DataGridViewColumn>();
        await s.WriteLineAsync(string.Join(",", headers.Skip(1).Select(column => $"\"{column.HeaderText}\""))).ConfigureAwait(false);

        foreach (var cells in dgData.Rows.Cast<DataGridViewRow>().Select(row => row.Cells.Cast<DataGridViewCell>()))
            await s.WriteLineAsync(string.Join(",", cells.Skip(1).Select(cell => $"\"{cell.Value}\""))).ConfigureAwait(false);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        bool cp = keyData == (Keys.Control | Keys.C) && ActiveControl is DataGridView;
        if (!cp)
            return base.ProcessCmdKey(ref msg, keyData);

        var content = dgData.GetClipboardContent();
        if (content == null)
            return base.ProcessCmdKey(ref msg, keyData);

        string data = content.GetText();
        var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgReportExportTable);
        if (dr != DialogResult.Yes)
        {
            WinFormsUtil.SetClipboardText(data);
            return true;
        }

        // Reformat datagrid clipboard content
        string[] lines = data.Split(Environment.NewLine);
        string[] newlines = ConvertTabbedToRedditTable(lines);
        WinFormsUtil.SetClipboardText(string.Join(Environment.NewLine, newlines));
        return true;
    }

    private static string[] ConvertTabbedToRedditTable(ReadOnlySpan<string> lines)
    {
        string[] newlines = new string[lines.Length + 1];
        int tabcount = lines[0].AsSpan().Count('\t');

        newlines[0] = lines[0].Replace('\t', '|');
        newlines[1] = string.Join(":--:", Enumerable.Repeat('|', tabcount + 2)); // 2 pipes for each end
        for (int i = 1; i < lines.Length; i++)
            newlines[i + 1] = lines[i].Replace('\t', '|');
        return newlines;
    }
}
