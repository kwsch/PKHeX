using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.SCBlockUtil;

namespace PKHeX.WinForms;

public partial class SAV_BlockDump8 : Form
{
    private readonly ISCBlockArray SAV;
    private readonly SCBlockMetadata Metadata;
    private readonly ComboItem[] SortedBlockKeys;

    private SCBlock CurrentBlock = null!;
    private string Filter = string.Empty;

    public SAV_BlockDump8(ISCBlockArray sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        PG_BlockView.Size = RTB_Hex.Size;

        // Get an external source of names if available.
        var extra = GetExtraKeyNames(sav);
        Metadata = new SCBlockMetadata(SAV.Accessor, extra, Main.Settings.Advanced.GetExclusionList8());

        CB_Key.InitializeBinding();
        SortedBlockKeys = Metadata.GetSortedBlockKeyList().ToArray();
        CB_Key.DataSource = SortedBlockKeys;

        CB_TypeToggle.InitializeBinding();
        CB_TypeToggle.DataSource = new[]
        {
            new ComboItem(nameof(SCTypeCode.Bool1), (int)SCTypeCode.Bool1),
            new ComboItem(nameof(SCTypeCode.Bool2), (int)SCTypeCode.Bool2),
        };
        CB_TypeToggle.SelectedIndexChanged += CB_TypeToggle_SelectedIndexChanged;
        CB_Key.KeyDown += WinFormsUtil.RemoveDropCB;

        CB_Key.SelectedIndex = 0;
    }

    private static IEnumerable<string> GetExtraKeyNames(ISCBlockArray obj)
    {
        var extra = Main.Settings.Advanced.PathBlockKeyList;
        if (extra.Length != 0 && !Directory.Exists(extra))
            return [];

        var file = Path.Combine(extra, obj.GetType().Name);
        file = $"{file}.txt";
        if (!File.Exists(file))
            return [];

        return File.ReadLines(file);
    }

    private void CB_Key_SelectedIndexChanged(object sender, EventArgs e)
    {
        var key = (uint)WinFormsUtil.GetIndex(CB_Key);
        CurrentBlock = SAV.Accessor.GetBlock(key);
        UpdateBlockSummaryControls();
        if (CurrentBlock.Type.IsBoolean())
        {
            CB_TypeToggle.SelectedValue = (int)CurrentBlock.Type;
            CB_TypeToggle.Visible = true;
        }
        else
        {
            CB_TypeToggle.Visible = false;
        }
    }

    private void UpdateBlockSummaryControls()
    {
        var block = CurrentBlock;
        L_Detail_R.Text = GetBlockSummary(block);
        RTB_Hex.Text = string.Join(' ', block.Data.Select(z => $"{z:X2}"));

        var blockName = Metadata.GetBlockName(block, out var obj);
        if (blockName != null)
        {
            L_BlockName.Visible = true;
            L_BlockName.Text = blockName;
        }
        else
        {
            L_BlockName.Visible = false;
        }

        if (ModifierKeys != Keys.Control)
        {
            // Show a PropertyGrid to edit
            if (obj != null)
            {
                var props = ReflectUtil.GetPropertiesCanWritePublicDeclared(obj.GetType());
                if (props.Count() > 1 || ModifierKeys == Keys.Shift)
                {
                    PG_BlockView.Visible = true;
                    PG_BlockView.SelectedObject = obj;
                    return;
                }
            }

            var o = SCBlockMetadata.GetEditableBlockObject(block);
            if (o != null)
            {
                PG_BlockView.Visible = true;
                PG_BlockView.SelectedObject = o;
                return;
            }
        }
        PG_BlockView.Visible = false;
    }

    private void CB_TypeToggle_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var block = CurrentBlock;
        var cType = block.Type;
        var cValue = (SCTypeCode)WinFormsUtil.GetIndex(CB_TypeToggle);
        if (cType == cValue)
            return;
        block.ChangeBooleanType(cValue);
        UpdateBlockSummaryControls();
    }

    private void B_ExportAll_Click(object sender, EventArgs e)
    {
        using var fbd = new FolderBrowserDialog();
        if (fbd.ShowDialog() != DialogResult.OK)
            return;
        var path = fbd.SelectedPath;
        var blocks = SAV.AllBlocks;
        ExportAllBlocks(blocks, path);
    }

    private static void ExportAllBlocks(IEnumerable<SCBlock> blocks, string path)
    {
        foreach (var b in blocks.Where(z => z.Data.Length != 0))
        {
            var fn = $"{GetBlockFileNameWithoutExtension(b)}.bin";
            var fileName = Path.Combine(path, fn);
            File.WriteAllBytes(fileName, b.Data);
        }
    }

    private void B_ImportFolder_Click(object sender, EventArgs e)
    {
        using var fbd = new FolderBrowserDialog();
        if (fbd.ShowDialog() != DialogResult.OK)
            return;

        var failed = ImportBlocksFromFolder(fbd.SelectedPath, SAV);
        if (failed.Count != 0)
        {
            var msg = string.Join(Environment.NewLine, failed);
            WinFormsUtil.Error("Failed to import:", msg);
        }
    }

    private void B_ImportCurrent_Click(object sender, EventArgs e) => ImportSelectBlock(CurrentBlock);
    private void B_ExportCurrent_Click(object sender, EventArgs e) => ExportSelectBlock(CurrentBlock);

    private void B_ExportAllSingle_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.FileName = "raw.bin";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        var path = sfd.FileName;
        var blocks = SAV.Accessor.BlockInfo;
        var option = GetExportOption();

        ExportAllBlocksAsSingleFile(blocks, path, option);
    }

    private SCBlockExportOption GetExportOption()
    {
        var option = SCBlockExportOption.None;
        if (CHK_DataOnly.Checked)
            option |= SCBlockExportOption.DataOnly;
        if (CHK_Key.Checked)
            option |= SCBlockExportOption.Key;
        if (CHK_Type.Checked)
            option |= SCBlockExportOption.TypeInfo;
        if (CHK_FakeHeader.Checked)
            option |= SCBlockExportOption.FakeHeader;
        return option;
    }

    private void B_LoadOld_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.FileName = "main";
        if (ofd.ShowDialog() != DialogResult.OK)
            return;
        TB_OldSAV.Text = ofd.FileName;
        if (!string.IsNullOrWhiteSpace(TB_NewSAV.Text))
            CompareSaves();
    }

    private void B_LoadNew_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.FileName = "main";
        if (ofd.ShowDialog() != DialogResult.OK)
            return;
        TB_NewSAV.Text = ofd.FileName;
        if (!string.IsNullOrWhiteSpace(TB_OldSAV.Text))
            CompareSaves();
    }

    private void CompareSaves()
    {
        var p1 = TB_OldSAV.Text;
        var p2 = TB_NewSAV.Text;

        var f1 = new FileInfo(p1);
        if (!SaveUtil.IsSizeValid((int)f1.Length))
            return;
        var f2 = new FileInfo(p1);
        if (!SaveUtil.IsSizeValid((int)f2.Length))
            return;

        var s1 = SaveUtil.GetVariantSAV(p1);
        if (s1 is not ISCBlockArray w1)
            return;
        var s2 = SaveUtil.GetVariantSAV(p2);
        if (s2 is not ISCBlockArray w2)
            return;

        // Get an external source of names if available.
        var extra = GetExtraKeyNames(w1);
        var compare = new SCBlockCompare(w1.Accessor, w2.Accessor, extra);
        richTextBox1.Lines = [.. compare.Summary()];
    }

    private static void ExportSelectBlock(SCBlock block)
    {
        var name = GetBlockFileNameWithoutExtension(block);
        using var sfd = new SaveFileDialog();
        sfd.FileName = $"{name}.bin";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;
        File.WriteAllBytes(sfd.FileName, block.Data);
    }

    private static void ImportSelectBlock(SCBlock blockTarget)
    {
        var key = blockTarget.Key;
        var data = blockTarget.Data;
        using var ofd = new OpenFileDialog();
        ofd.FileName = $"{key:X8}.bin";
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var path = ofd.FileName;
        var file = new FileInfo(path);
        if (file.Length != data.Length)
        {
            WinFormsUtil.Error(string.Format(MessageStrings.MsgFileSize, $"0x{file.Length:X8}"));
            return;
        }

        var bytes = File.ReadAllBytes(path);
        blockTarget.ChangeData(bytes);
    }

    private void PG_BlockView_PropertyValueChanged(object s, PropertyValueChangedEventArgs? e)
    {
        if (e?.ChangedItem is not null)
            Debug.WriteLine($"ChangedItem = {e.ChangedItem.Label}, OldValue = {e.OldValue}, NewValue = {e.ChangedItem.Value}");
        if (CurrentBlock.Type != SCTypeCode.Object && CurrentBlock.Type != SCTypeCode.Array)
            L_Detail_R.Text = GetBlockSummary(CurrentBlock);
    }

    private void CB_Key_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
            return;

        var text = CB_Key.Text.Trim();
        if (text.Length == 8)
        {
            var hex = (int)Util.GetHexValue(text);
            if (hex != 0)
            {
                if (!string.IsNullOrEmpty(Filter))
                {
                    // Clear the filter
                    CB_Key.DataSource = SortedBlockKeys;
                    Filter = string.Empty;
                }
                // Input is hexadecimal number, select the item -- if it exists.
                bool exists = SortedBlockKeys.Any(z => z.Value == hex);
                if (exists)
                {
                    CB_Key.SelectedValue = hex;
                    return;
                }
            }
        }

        if (CB_Key.SelectedItem != null && text.Equals(CB_Key.SelectedText))
            return; // User press enter on selected item

        if (Filter.Equals(text, StringComparison.InvariantCultureIgnoreCase))
            return; // Filter hasn't changed

        Filter = text;
        if (string.IsNullOrEmpty(text))
        {
            // User has cleared the filter. Restore original metadata
            CB_Key.DataSource = SortedBlockKeys;
            CB_Key.SelectedIndex = 0;
            return;
        }

        // Filter combo items that contains input text
        var filtered = Array.FindAll(SortedBlockKeys, x => x.Text.Contains(text, StringComparison.InvariantCultureIgnoreCase));
        if (filtered.Length == 0)
            return; // no results

        CB_Key.DataSource = filtered;
        CB_Key.SelectedIndex = 0;
    }
}
