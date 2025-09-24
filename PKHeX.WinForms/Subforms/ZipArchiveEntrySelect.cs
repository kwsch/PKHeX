using PKHeX.Core;

using System;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class ZipArchiveEntrySelect : Form
{
    public ZipArchiveEntry? SelectedEntry { get; private set; } = null;

    public ZipArchiveEntrySelect(ZipArchive zip, string pathDisplay)
    {
        InitializeComponent();
        labelInstruction.Text = MessageStrings.MsgZipArchiveSelectEntryPrompt + pathDisplay;
        listViewArchiveEntries.Items.AddRange(CreateListViewItems(zip));

        if (listViewArchiveEntries.Items.Count > 0)
        {
            listViewArchiveEntries.Items[0].Selected = true;
        }
    }

    private void ClickButtonCancel(object sender, EventArgs e)
    {
        SelectedEntry = null;
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void ClickButtonSelect(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void ItemSelectionChangedListViewArchiveEntries(object sender, ListViewItemSelectionChangedEventArgs e)
    {
        SelectedEntry = e.Item?.Tag as ZipArchiveEntry ?? throw new InvalidCastException();
        buttonSelect.Enabled = SelectedEntry != null;
        textBoxSelectedEntry.Text = SelectedEntry?.FullName ?? string.Empty;
    }

    private void MouseDoubleClickListViewArchiveEntries(object sender, MouseEventArgs e)
    {
        ListViewHitTestInfo hit = listViewArchiveEntries.HitTest(e.Location);

        if (hit.Item == null)
            return;

        SelectedEntry = hit.Item.Tag as ZipArchiveEntry ?? throw new InvalidCastException();
        DialogResult = DialogResult.OK;
        Close();
    }

    static private ListViewItem[] CreateListViewItems(ZipArchive zip)
    {
        return zip.Entries
            .Where(e => e.Length > 0 && !string.IsNullOrWhiteSpace(e.Name))
            .OrderBy(SaveFilenameOpionionatedSort)
            .ThenByDescending(e => e.Length)
            .ThenBy(e => e.FullName)
            .Select(e =>
            {
                ListViewItem lvi = new(e.FullName);
                lvi.Tag = e;
                lvi.SubItems.Add(BytesToHumanReadable(e.Length));
                return lvi;
            })
            .ToArray();

        // a sort function that prefers common names of save files
        static long SaveFilenameOpionionatedSort(ZipArchiveEntry e)
        {
            string name = e.Name.ToLowerInvariant();

            if (name.EndsWith(".sav", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            return name switch
            {
                "main" => 0,
                "savedata.bin" => 0,
                "sav.dat" => 0,
                "backup" => 1,
                "backup.bin" => 1,
                _ => 999,
            };
        }
    }

    // via https://stackoverflow.com/a/62698159
    private static string BytesToHumanReadable(long bytes)
    {
        var unit = 1024;
        if (bytes < unit) { return $"{bytes} B"; }

        var exp = (int)(Math.Log(bytes) / Math.Log(unit));
        return $"{bytes / Math.Pow(unit, exp):F2} {("KMGTPE")[exp - 1]}B";
    }
}
