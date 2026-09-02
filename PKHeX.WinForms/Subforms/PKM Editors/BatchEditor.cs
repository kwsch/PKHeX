using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class BatchEditor : Form
{
    private readonly SaveFile _sav;
    private readonly SlotChangelog _changelog;

    // Cached source data. The cache is intentionally mutable; batch edits are accumulated here until the user chooses Save.
    private IReadOnlyList<SlotCache>? _boxData;
    private IReadOnlyList<SlotCache>? _party;
    private IReadOnlyList<SlotCache>? _folder;
    private readonly Dictionary<ISlotInfo, string> _folderPaths = new();
    private readonly HashSet<ISlotInfo> _modifiedSlots = [];
    private readonly string _matchingCountFormat;

    private EntityBatchProcessor _editor = new();
    private readonly EntityInstructionBuilder _builder;

    /// <summary>
    /// Remember the last used commands so that they can be restored when the form is reopened.
    /// </summary>
    private static string _lastUsedCommands = string.Empty;

    public BatchEditor(PKM pk, SaveFile sav, SlotChangelog changelog)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        _matchingCountFormat = L_Count.Text; // cache the translated string
        _sav = sav;
        _changelog = changelog;

        // Builder needs to be late-bound to the input PKM from the main form.
        _builder = new EntityInstructionBuilder(() => pk) { Dock = DockStyle.Fill, Margin = new Padding(4) };
        TLP_Bottom.Controls.Add(_builder, 0, 1);
        TLP_Bottom.SetColumnSpan(_builder, TLP_Bottom.ColumnCount - 1); // Add button occupies last column.

        // Boxes are the default source and are immediately available for filter analysis.
        _boxData = CreateBoxData();
        UpdateFilterCountDebounced();
        UpdateButtons();

        RTB_Instructions.Text = _lastUsedCommands;
    }

    public IReadOnlyList<ISlotInfo> GetModifiedSlots() => [.. _modifiedSlots];

    private IReadOnlyList<SlotCache> CreateBoxData()
    {
        var data = new List<SlotCache>(_sav.SlotCount);
        SlotInfoLoader.AddBoxData(_sav, data);
        return data;
    }

    private IReadOnlyList<SlotCache> CreatePartyData()
    {
        var data = new List<SlotCache>(_sav.PartyCount);
        SlotInfoLoader.AddPartyData(_sav, data);
        return data;
    }

    private IReadOnlyList<SlotCache> CreateFolderData()
    {
        if (!Directory.Exists(TB_Folder.Text))
            return [];

        var result = new List<SlotCache>();
        IEnumerable<string> files;
        try
        {
            files = Directory.GetFiles(TB_Folder.Text, "*", SearchOption.AllDirectories);
        }
        catch (IOException)
        {
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            return result;
        }

        foreach (var source in files)
        {
            var fi = new FileInfo(source);
            if (!EntityDetection.IsSizePlausible(fi.Length))
                continue;

            try
            {
                var data = File.ReadAllBytes(source);
                if (FileUtil.TryGetPKM(data, out var pk, fi.Extension, _sav))
                {
                    var info = new SlotInfoFileSingle(source);
                    result.Add(new SlotCache(info, pk));
                    _folderPaths[info] = source;
                }
            }
            catch (IOException)
            {
                // A file that cannot be read is simply not a processable source entity.
            }
            catch (UnauthorizedAccessException)
            {
                // A file that cannot be read is simply not a processable source entity.
            }
        }

        return result;
    }

    private IReadOnlyList<SlotCache> GetCurrentData()
    {
        if (RB_Party.Checked)
            return _party ??= CreatePartyData();

        if (RB_Path.Checked)
            return _folder ??= CreateFolderData();

        return _boxData!;
    }

    private void B_Open_Click(object sender, EventArgs e)
    {
        using var fbd = new FolderBrowserDialog();
        if (fbd.ShowDialog() != DialogResult.OK)
            return;

        TB_Folder.Text = fbd.SelectedPath;
        TB_Folder.Visible = true;
        RB_Path.Checked = true;
        _folder = null;
        _folderPaths.Clear();
        UpdateFilterCountDebounced();
        UpdateButtons();
    }

    private void B_SAV_Click(object sender, EventArgs e)
    {
        TB_Folder.Text = string.Empty;
        TB_Folder.Visible = false;
        _folder = null;
        _folderPaths.Clear();
        UpdateFilterCountDebounced();
        UpdateButtons();
    }

    private void B_Reset_Click(object sender, EventArgs e)
    {
        // Reset only discards the in-memory save-file work. Folder operations have already
        // been written to disk and intentionally cannot be reverted by this form.
        _modifiedSlots.Clear();
        _boxData = null;
        _party = null;
        _folder = null;
        _folderPaths.Clear();
        _editor = new EntityBatchProcessor();

        RB_Boxes.Checked = true;
        TB_Folder.Text = string.Empty;
        TB_Folder.Visible = false;

        _boxData = CreateBoxData();
        UpdateFilterCountDebounced();
        UpdateButtons();
    }

    private void B_Run_Click(object sender, EventArgs e)
    {
        ReadOnlySpan<char> text = RTB_Instructions.Text;
        if (!TryGetInstructionSets(text, out var sets, promptForEmptyValues: true, showErrors: true))
            return;

        foreach (var set in sets)
        {
            EntityBatchEditor.ScreenStrings(set.Filters);
            EntityBatchEditor.ScreenStrings(set.Instructions);
        }

        if (RB_Path.Checked)
        {
            RunBatchEditFolder(sets);
            return;
        }

        RunBatchEditSaveFile(sets);
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        if (_modifiedSlots.Count == 0)
        {
            DialogResult = DialogResult.OK;
            return;
        }

        // Flush all modified savedata slots back to the save.
        var slots = GetChangelogSlots();
        using var change = _changelog.Begin(slots);
        var settings = default(EntityImportSettings) with { UpdateRecord = EntityImportOption.Disable };
        foreach (var slot in slots)
        {
            if (TryGetCachedSlot(slot, out var cache))
                slot.WriteTo(_sav, cache.Entity, settings);
        }

        change.Commit();
        DialogResult = DialogResult.OK;
    }

    private IReadOnlyList<ISlotInfo> GetChangelogSlots() => [.. _modifiedSlots];

    private bool TryGetCachedSlot(ISlotInfo source, [NotNullWhen(true)] out SlotCache? cache)
    {
        cache = _boxData?.FirstOrDefault(z => ReferenceEquals(z.Source, source))
               ?? _party?.FirstOrDefault(z => ReferenceEquals(z.Source, source));
        return cache is not null;
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void B_Add_Click(object sender, EventArgs e)
    {
        var s = _builder.Create();
        if (s.Length == 0)
        { WinFormsUtil.Alert(MsgBEPropertyInvalid); return; }

        // If we already have text, add a new line (except if the last line is blank).
        var tb = RTB_Instructions;
        var batchText = tb.Text;
        if (batchText.Length != 0 && !batchText.EndsWith('\n'))
            tb.AppendText(Environment.NewLine);
        RTB_Instructions.AppendText(s);
    }

    private void TabMain_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data is null)
            return;
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
    }

    private void TabMain_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;
        if (!Directory.Exists(files[0]))
            return;

        TB_Folder.Text = files[0];
        TB_Folder.Visible = true;
        RB_Boxes.Checked = RB_Party.Checked = false;
        RB_Path.Checked = true;
        _folder = null;
        _folderPaths.Clear();
        UpdateFilterCountDebounced();
        UpdateButtons();
    }

    private void RTB_Instructions_TextChanged(object? sender, EventArgs e) => UpdateFilterCountDebounced();
    private CancellationTokenSource _filterCountCancellation = new();
    private int _filterCountGeneration;

    private async void UpdateFilterCountDebounced()
    {
        try
        {
            var text = RTB_Instructions.Text;
            await (_filterCountCancellation.CancelAsync());
            _filterCountCancellation.Dispose();

            var cancellation = new CancellationTokenSource();
            _filterCountCancellation = cancellation;

            var generation = ++_filterCountGeneration;
            await Task.Delay(250, cancellation.Token);
            if (cancellation.IsCancellationRequested)
                return;

            var result = await Task.Run(() => TryGetFilterMessage(text, cancellation.Token, out var message)
                    ? message
                    : null, cancellation.Token); // return to GUI thread

            if (cancellation.IsCancellationRequested)
                return;
            if (generation != _filterCountGeneration)
                return;
            L_Count.Text = result;
            UpdateButtons();
        }
        catch
        {
            // Don't care.
        }
    }

    private bool TryGetFilterMessage(ReadOnlySpan<char> text, CancellationToken token, [NotNullWhen(true)] out string? result)
    {
        result = null;
        var data = GetCurrentData();
        int total = data.Count(z => z.Entity.Species != 0);
        if (total == 0)
        {
            result = string.Format(_matchingCountFormat, 0, 0);
            return true;
        }

        if (!TryGetInstructionSets(text, out var sets, promptForEmptyValues: false, allowOnlyFilters: true))
        {
            result = string.Format(_matchingCountFormat, "-", total);
            return true;
        }

        foreach (var set in sets)
            EntityBatchEditor.ScreenStrings(set.Filters);

        if (token.IsCancellationRequested)
            return false;

        int matched = 0;
        var max = _sav.MaxSpeciesID;
        foreach (var entry in data)
        {
            var pk = entry.Entity;
            if (pk.Species == 0 || pk.Species > max)
                continue;
            if (entry.Source is SlotInfoBox info && _sav.GetBoxSlotFlags(info.Box, info.Slot).IsOverwriteProtected())
                continue;

            if (token.IsCancellationRequested)
                return false;

            if (sets.Any(set => IsFilterMatch(entry, set)))
                matched++;
        }

        result = string.Format(_matchingCountFormat, matched, total);
        return true;
    }

    private static bool IsFilterMatch(SlotCache entry, StringInstructionSet set)
    {
        var filterMeta = set.Filters.Where(IsMetaFilter).ToArray();
        var filters = set.Filters.Where(z => !IsMetaFilter(z)).ToArray();

        if (!EntityBatchEditor.IsFilterMatchMeta(filterMeta, entry))
            return false;

        return filters.Length == 0 || BatchEditingUtil.IsFilterMatch(filters, entry.Entity);
    }

    private static bool IsMetaFilter(StringInstruction filter) => BatchFilters.FilterMeta.Any(z => z.IsMatch(filter.PropertyName));

    private static bool TryGetInstructionSets(ReadOnlySpan<char> text, out StringInstructionSet[] sets, bool promptForEmptyValues, bool showErrors = false, bool allowOnlyFilters = false)
    {
        sets = [];
        if (text.IsEmpty)
            return false;
        if (StringInstructionSet.HasEmptyLine(text))
        {
            if (showErrors)
                WinFormsUtil.Error(MsgBEInstructionInvalid);
            return false;
        }

        try
        {
            sets = StringInstructionSet.GetBatchSets(text);
        }
        catch
        {
            if (showErrors)
                WinFormsUtil.Error(MsgBEInstructionInvalid);
            return false;
        }

        if (Array.Exists(sets, s => s.Filters.Any(z => string.IsNullOrWhiteSpace(z.PropertyValue))))
        {
            if (showErrors)
                WinFormsUtil.Error(MsgBEFilterEmpty);
            return false;
        }
        if (Array.Exists(sets, z => z.Instructions.Count == 0))
        {
            if (showErrors)
                WinFormsUtil.Error(MsgBEInstructionNone);
            return (allowOnlyFilters && sets.Any(z => z.Filters.Count != 0));
        }

        if (!promptForEmptyValues)
            return true;

        var emptyVal = sets.SelectMany(s => s.Instructions.Where(z => string.IsNullOrWhiteSpace(z.PropertyValue))).ToArray();
        if (emptyVal.Length == 0)
            return true;

        string props = string.Join(", ", emptyVal.Select(z => z.PropertyName));
        string invalid = MsgBEPropertyEmpty + Environment.NewLine + props;
        return DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, invalid, MsgContinue);
    }

    private void RunBatchEditSaveFile(IReadOnlyCollection<StringInstructionSet> sets)
    {
        var data = GetCurrentData();
        if (data.Count == 0)
            return;

        _editor = new EntityBatchProcessor();
        foreach (var set in sets)
            ProcessSAV(data, set.Filters, set.Instructions);

        UpdateFilterCountDebounced();
        UpdateButtons();

        string result = _editor.GetEditorResults(sets);
        WinFormsUtil.Alert(result);
    }

    private void ProcessSAV(IReadOnlyList<SlotCache> data, IReadOnlyList<StringInstruction> filters, IReadOnlyList<StringInstruction> instructions)
    {
        var filterMeta = filters.Where(IsMetaFilter).ToArray();
        if (filterMeta.Length != 0)
            filters = [.. filters.Where(z => !IsMetaFilter(z))];

        var max = _sav.MaxSpeciesID;
        foreach (var entry in data)
        {
            var pk = entry.Entity;
            var spec = pk.Species;
            if (spec == 0 || spec > max)
                continue;

            if (entry.Source is SlotInfoBox info && _sav.GetBoxSlotFlags(info.Box, info.Slot).IsOverwriteProtected())
                continue;
            if (!EntityBatchEditor.IsFilterMatchMeta(filterMeta, entry))
                continue;

            if (_editor.Process(pk, filters, instructions))
                _modifiedSlots.Add(entry.Source);
        }
    }

    private void RunBatchEditFolder(IReadOnlyCollection<StringInstructionSet> sets)
    {
        if (string.IsNullOrWhiteSpace(TB_Folder.Text))
            return;

        WinFormsUtil.Alert(MsgExportFolder, MsgExportFolderAdvice);
        using var fbd = new FolderBrowserDialog();
        if (fbd.ShowDialog() != DialogResult.OK)
            return;

        var data = GetCurrentData();
        if (data.Count == 0)
            return;

        var destination = fbd.SelectedPath;
        _editor = new EntityBatchProcessor();
        foreach (var set in sets)
            ProcessFolder(data, destination, set.Filters, set.Instructions);

        string result = _editor.GetEditorResults(sets);
        WinFormsUtil.Alert(result);
        UpdateFilterCountDebounced();
    }

    private void ProcessFolder(IReadOnlyList<SlotCache> data, string destDir, IReadOnlyList<StringInstruction> pkFilters, IReadOnlyList<StringInstruction> instructions)
    {
        var filterMeta = pkFilters.Where(IsMetaFilter).ToArray();
        if (filterMeta.Length != 0)
            pkFilters = [.. pkFilters.Where(z => !IsMetaFilter(z))];

        Span<byte> maxEntity = stackalloc byte[0x800]; // lol too big, futureproof for now
        foreach (var entry in data)
        {
            if (!EntityBatchEditor.IsFilterMatchMeta(filterMeta, entry))
                continue;

            if (!_editor.Process(entry.Entity, pkFilters, instructions))
                continue;

            if (!_folderPaths.TryGetValue(entry.Source, out var source))
                continue;

            // We might have mixed size files, so we can't have a shared stackalloc
            var result = maxEntity[..entry.Entity.SIZE_PARTY];
            entry.Entity.ForcePartyData();
            entry.Entity.WriteDecryptedDataParty(result);
            File.WriteAllBytes(Path.Combine(destDir, Path.GetFileName(source)), result);
        }
    }

    private void UpdateButtons()
    {
        bool isOperatingOnFolder = RB_Path.Checked;
        B_Run.Enabled = RTB_Instructions.Text.Length != 0 && (isOperatingOnFolder || GetCurrentData().Count(z => z.Entity.Species != 0) != 0);
        B_Save.Enabled = isOperatingOnFolder || _modifiedSlots.Count != 0;
        B_Reset.Enabled = _modifiedSlots.Count != 0;
    }

    private void BatchEditor_FormClosing(object? sender, FormClosingEventArgs e) => _lastUsedCommands = RTB_Instructions.Text;
}
