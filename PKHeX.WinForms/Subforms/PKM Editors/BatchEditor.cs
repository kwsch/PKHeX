using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class BatchEditor : Form
{
    private readonly SaveFile SAV;
    private readonly PKM Entity;
    private int currentFormat = -1;

    public BatchEditor(PKM pk, SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        Entity = pk;
        SAV = sav;
        DragDrop += TabMain_DragDrop;
        DragEnter += TabMain_DragEnter;

        CB_Format.Items.Clear();
        CB_Format.Items.Add(MsgAny);
        foreach (Type t in BatchEditing.Types)
            CB_Format.Items.Add(t.Name.ToLowerInvariant());
        CB_Format.Items.Add(MsgAll);

        CB_Format.SelectedIndex = CB_Require.SelectedIndex = 0;
        toolTip1.SetToolTip(CB_Property, MsgBEToolTipPropName);
        toolTip2.SetToolTip(L_PropType, MsgBEToolTipPropType);
        toolTip3.SetToolTip(L_PropValue, MsgBEToolTipPropValue);
    }

    private void B_Open_Click(object sender, EventArgs e)
    {
        if (!B_Go.Enabled) return;
        using var fbd = new FolderBrowserDialog();
        if (fbd.ShowDialog() != DialogResult.OK)
            return;

        TB_Folder.Text = fbd.SelectedPath;
        TB_Folder.Visible = true;
    }

    private void B_SAV_Click(object sender, EventArgs e)
    {
        TB_Folder.Text = string.Empty;
        TB_Folder.Visible = false;
    }

    private void B_Go_Click(object sender, EventArgs e)
    {
        RunBackgroundWorker();
    }

    private void B_Add_Click(object sender, EventArgs e)
    {
        if (CB_Property.SelectedIndex < 0)
        { WinFormsUtil.Alert(MsgBEPropertyInvalid); return; }

        var prefix = StringInstruction.Prefixes;
        string s = prefix[CB_Require.SelectedIndex] + CB_Property.Items[CB_Property.SelectedIndex].ToString() + StringInstruction.SplitInstruction;
        if (RTB_Instructions.Lines.Length != 0 && RTB_Instructions.Lines[^1].Length > 0)
            s = Environment.NewLine + s;

        RTB_Instructions.AppendText(s);
    }

    private void CB_Format_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (currentFormat == CB_Format.SelectedIndex)
            return;

        int format = CB_Format.SelectedIndex;
        CB_Property.Items.Clear();
        CB_Property.Items.AddRange(BatchEditing.Properties[format]);
        CB_Property.SelectedIndex = 0;
        currentFormat = format;
    }

    private void CB_Property_SelectedIndexChanged(object sender, EventArgs e)
    {
        L_PropType.Text = BatchEditing.GetPropertyType(CB_Property.Text, CB_Format.SelectedIndex);
        if (BatchEditing.TryGetHasProperty(Entity, CB_Property.Text, out var pi))
        {
            L_PropValue.Text = pi.GetValue(Entity)?.ToString();
            L_PropType.ForeColor = L_PropValue.ForeColor; // reset color
        }
        else // no property, flag
        {
            L_PropValue.Text = string.Empty;
            L_PropType.ForeColor = Color.Red;
        }
    }

    private static void TabMain_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e?.Data is null)
            return;
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
    }

    private void TabMain_DragDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] {Length: not 0} files)
            return;
        if (!Directory.Exists(files[0]))
            return;

        TB_Folder.Text = files[0];
        TB_Folder.Visible = true;
        RB_Boxes.Checked = RB_Party.Checked = false;
        RB_Path.Checked = true;
    }

    private void RunBackgroundWorker()
    {
        if (RTB_Instructions.Lines.Any(line => line.Length == 0))
        { WinFormsUtil.Error(MsgBEInstructionInvalid); return; }

        var sets = StringInstructionSet.GetBatchSets(RTB_Instructions.Lines).ToArray();
        if (sets.Any(s => s.Filters.Any(z => string.IsNullOrWhiteSpace(z.PropertyValue))))
        { WinFormsUtil.Error(MsgBEFilterEmpty); return; }

        if (sets.Any(z => z.Instructions.Count == 0))
        { WinFormsUtil.Error(MsgBEInstructionNone); return; }

        var emptyVal = sets.SelectMany(s => s.Instructions.Where(z => string.IsNullOrWhiteSpace(z.PropertyValue))).ToArray();
        if (emptyVal.Length > 0)
        {
            string props = string.Join(", ", emptyVal.Select(z => z.PropertyName));
            string invalid = MsgBEPropertyEmpty + Environment.NewLine + props;
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, invalid, MsgContinue))
                return;
        }

        string? destPath = null;
        if (RB_Path.Checked)
        {
            WinFormsUtil.Alert(MsgExportFolder, MsgExportFolderAdvice);
            using var fbd = new FolderBrowserDialog();
            var dr = fbd.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            destPath = fbd.SelectedPath;
        }

        FLP_RB.Enabled = RTB_Instructions.Enabled = B_Go.Enabled = false;

        foreach (var set in sets)
        {
            BatchEditing.ScreenStrings(set.Filters);
            BatchEditing.ScreenStrings(set.Instructions);
        }
        RunBatchEdit(sets, TB_Folder.Text, destPath);
    }

    private void RunBatchEdit(StringInstructionSet[] sets, string source, string? destination)
    {
        editor = new Core.BatchEditor();
        bool finished = false, displayed = false; // hack cuz DoWork event isn't cleared after completion
        b.DoWork += (sender, e) =>
        {
            if (finished)
                return;
            if (RB_Boxes.Checked)
                RunBatchEditSaveFile(sets, boxes: true);
            else if (RB_Party.Checked)
                RunBatchEditSaveFile(sets, party: true);
            else if (destination != null)
                RunBatchEditFolder(sets, source, destination);
            finished = true;
        };
        b.ProgressChanged += (sender, e) => SetProgressBar(e.ProgressPercentage);
        b.RunWorkerCompleted += (sender, e) =>
        {
            string result = editor.GetEditorResults(sets);
            if (!displayed) WinFormsUtil.Alert(result);
            displayed = true;
            FLP_RB.Enabled = RTB_Instructions.Enabled = B_Go.Enabled = true;
            SetupProgressBar(0);
        };
        b.RunWorkerAsync();
    }

    private void RunBatchEditFolder(IReadOnlyCollection<StringInstructionSet> sets, string source, string destination)
    {
        var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        SetupProgressBar(files.Length * sets.Count);
        foreach (var set in sets)
            ProcessFolder(files, destination, set.Filters, set.Instructions);
    }

    private void RunBatchEditSaveFile(IReadOnlyCollection<StringInstructionSet> sets, bool boxes = false, bool party = false)
    {
        var data = new List<SlotCache>();
        if (party)
        {
            SlotInfoLoader.AddPartyData(SAV, data);
            process(data);
            SAV.PartyData = data.ConvertAll(z => z.Entity);
        }
        if (boxes)
        {
            SlotInfoLoader.AddBoxData(SAV, data);
            process(data);
            SAV.BoxData = data.ConvertAll(z => z.Entity);
        }
        void process(IList<SlotCache> d)
        {
            SetupProgressBar(d.Count * sets.Count);
            foreach (var set in sets)
                ProcessSAV(d, set.Filters, set.Instructions);
        }
    }

    // Progress Bar
    private void SetupProgressBar(int count)
    {
        MethodInvoker mi = () => { PB_Show.Minimum = 0; PB_Show.Step = 1; PB_Show.Value = 0; PB_Show.Maximum = count; };
        if (PB_Show.InvokeRequired)
            PB_Show.Invoke(mi);
        else
            mi.Invoke();
    }

    private void SetProgressBar(int position)
    {
        if (PB_Show.InvokeRequired)
            PB_Show.Invoke((MethodInvoker)(() => PB_Show.Value = position));
        else
            PB_Show.Value = position;
    }

    // Mass Editing
    private Core.BatchEditor editor = new();

    private void ProcessSAV(IList<SlotCache> data, IReadOnlyList<StringInstruction> Filters, IReadOnlyList<StringInstruction> Instructions)
    {
        if (data.Count == 0)
            return;

        var filterMeta = Filters.Where(f => BatchFilters.FilterMeta.Any(z => z.IsMatch(f.PropertyName))).ToArray();
        if (filterMeta.Length != 0)
            Filters = Filters.Except(filterMeta).ToArray();

        var max = data[0].Entity.MaxSpeciesID;

        for (int i = 0; i < data.Count; i++)
        {
            var entry = data[i];
            var pk = data[i].Entity;

            var spec = pk.Species;
            if (spec <= 0 || spec > max)
                continue;

            if (entry.Source is SlotInfoBox info && SAV.GetSlotFlags(info.Box, info.Slot).IsOverwriteProtected())
                editor.AddSkipped();
            else if (!BatchEditing.IsFilterMatchMeta(filterMeta, entry))
                editor.AddSkipped();
            else
                editor.Process(pk, Filters, Instructions);

            b.ReportProgress(i);
        }
    }

    private void ProcessFolder(IReadOnlyList<string> files, string destDir, IReadOnlyList<StringInstruction> pkFilters, IReadOnlyList<StringInstruction> instructions)
    {
        var filterMeta = pkFilters.Where(f => BatchFilters.FilterMeta.Any(z => z.IsMatch(f.PropertyName))).ToArray();
        if (filterMeta.Length != 0)
            pkFilters = pkFilters.Except(filterMeta).ToArray();

        for (int i = 0; i < files.Count; i++)
        {
            TryProcess(files[i], destDir, filterMeta, pkFilters, instructions);
            b.ReportProgress(i);
        }
    }

    private void TryProcess(string source, string destDir, IReadOnlyList<StringInstruction> metaFilters, IReadOnlyList<StringInstruction> pkFilters, IReadOnlyList<StringInstruction> instructions)
    {
        var fi = new FileInfo(source);
        if (!EntityDetection.IsSizePlausible(fi.Length))
            return;

        byte[] data = File.ReadAllBytes(source);
        _ = FileUtil.TryGetPKM(data, out var pk, fi.Extension, SAV);
        if (pk == null)
            return;

        var info = new SlotInfoFile(source);
        var entry = new SlotCache(info, pk);
        if (!BatchEditing.IsFilterMatchMeta(metaFilters, entry))
        {
            editor.AddSkipped();
            return;
        }

        if (editor.Process(pk, pkFilters, instructions))
            File.WriteAllBytes(Path.Combine(destDir, Path.GetFileName(source)), pk.DecryptedPartyData);
    }
}
