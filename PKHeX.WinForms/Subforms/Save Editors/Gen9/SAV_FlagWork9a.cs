using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public sealed partial class SAV_FlagWork9a : Form
{
    private readonly Dictionary<ulong, string> Lookup = new() { {FnvHash.HashEmpty, ""} };
    private readonly EventWorkLookup Names;

    private readonly IEventWorkGrid[] Grids;

    public SAV_FlagWork9a(SAV9ZA sav)
    {
        InitializeComponent();

        var path = Path.Combine(Main.Settings.Advanced.PathBlockKeyList, $"{sav.GetType().Name}_flagwork.txt");
        if (File.Exists(path))
            SCBlockMetadata.AddExtraKeyNames64(Lookup, File.ReadLines(path));

        Names = new EventWorkLookup(Lookup);

        // Create grids for each block
        Grids =
        [
            EventWorkGrid64<bool>.CreateFlags(GetTab(nameof(sav.Blocks.Flags)), sav.Blocks.Flags, Names),
            EventWorkGrid64<bool>.CreateFlags(GetTab(nameof(sav.Blocks.Event)), sav.Blocks.Event, Names),
            EventWorkGrid64<ulong>.CreateValues(GetTab(nameof(sav.Blocks.Work)), sav.Blocks.Work, Names),
            EventWorkGrid64<ulong>.CreateValues(GetTab(nameof(sav.Blocks.Quest)), sav.Blocks.Quest, Names),
            EventWorkGrid64<ulong>.CreateValues(GetTab(nameof(sav.Blocks.WorkMable)), sav.Blocks.WorkMable, Names),
            EventWorkGrid64<ulong>.CreateValues(GetTab(nameof(sav.Blocks.CountMable)), sav.Blocks.CountMable, Names),

            EventWorkGrid64<ulong>.CreateValues(GetTab(nameof(sav.Blocks.CountTitle)), sav.Blocks.CountTitle, Names),
            EventWorkGridTuple.CreateValues(GetTab(nameof(sav.Blocks.Report)), sav.Blocks.Report, Names),

            EventWorkGrid64<ulong>.CreateValues(GetTab(nameof(sav.Blocks.WorkSpawn)), sav.Blocks.WorkSpawn, Names),
            EventWorkGrid64<ulong>.CreateValues(GetTab(nameof(sav.Blocks.InfiniteRank)), sav.Blocks.InfiniteRank, Names),
            EventWorkGridTuple.CreateValues(GetTab(nameof(sav.Blocks.Spawner2)), sav.Blocks.Spawner2, Names),
            EventWorkGrid128.CreateValues(GetTab(nameof(sav.Blocks.Spawner4)), sav.Blocks.Spawner4, Names),

            EventWorkGridTuple.CreateValues(GetTab(nameof(sav.Blocks.Obstruction)), sav.Blocks.Obstruction, Names),
            EventWorkGrid64<bool>.CreateFlags(GetTab(nameof(sav.Blocks.FieldItems)), sav.Blocks.FieldItems, Names),
            EventWorkGrid192.CreateValues(GetTab(nameof(sav.Blocks.FieldObjectInteractable)), sav.Blocks.FieldObjectInteractable, Names),
        ];

        // Translate headings
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        // Load data
        foreach (var grid in Grids)
            grid.Load();

        AllowDrop = true;
        DragEnter += Main_DragEnter;
        DragDrop += Main_DragDrop;
    }

    private TabPage GetTab(string name)
    {
        var result = new TabPage(name) { Name = $"Tab_{name}" };
        var current = TC_Features.TabPages;
        TC_Features.TabPages.Insert(current.Count - 1, result);
        return result;
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        foreach (var grid in Grids)
            grid.Save();
        Close();
    }

    private void ChangeSAV()
    {
        if (TB_NewSAV.Text.Length != 0 && TB_OldSAV.Text.Length != 0)
            DiffSaves(TB_NewSAV.Text, TB_OldSAV.Text);
    }

    private void OpenSAV(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Title = MessageStrings.MsgFileLoadSaveSelectGame;
        if (ofd.ShowDialog() == DialogResult.OK)
            LoadSAV(sender, ofd.FileName);
    }

    private void LoadSAV(object sender, string path)
    {
        if (sender == B_LoadOld)
            TB_OldSAV.Text = path;
        else
            TB_NewSAV.Text = path;
        ChangeSAV();
    }

    private void DiffSaves(string fileUpdated, string filePrevious)
    {
        if (!SaveUtil.TryGetSaveFile(fileUpdated, out var s1) || s1 is not SAV9ZA updated)
        {
            WinFormsUtil.Error(EventWorkDiffCompatibility.FileMissing1.GetMessage());
            return;
        }
        if (!SaveUtil.TryGetSaveFile(filePrevious, out var s2) || s2 is not SAV9ZA previous)
        {
            WinFormsUtil.Error(EventWorkDiffCompatibility.FileMissing2.GetMessage());
            return;
        }
        List<string> result = [];

        AppendDiff<EventWorkFlagStorage, bool>(result, updated.Blocks.Flags, previous.Blocks.Flags);
        AppendDiff<EventWorkFlagStorage, bool>(result, updated.Blocks.Event, previous.Blocks.Event);

        AppendDiff<EventWorkValueStorage, ulong>(result, updated.Blocks.Work, previous.Blocks.Work);
        AppendDiff<EventWorkValueStorage, ulong>(result, updated.Blocks.Quest, previous.Blocks.Quest);
        AppendDiff<EventWorkValueStorage, ulong>(result, updated.Blocks.WorkMable, previous.Blocks.WorkMable);
        AppendDiff<EventWorkValueStorage, ulong>(result, updated.Blocks.CountMable, previous.Blocks.CountMable);
        AppendDiff<EventWorkValueStorage, ulong>(result, updated.Blocks.CountTitle, previous.Blocks.CountTitle);
        AppendDiff<EventWorkValueStorage, ulong>(result, updated.Blocks.WorkSpawn, previous.Blocks.WorkSpawn);
        AppendDiff<EventWorkFlagStorage, bool>(result, updated.Blocks.FieldItems, previous.Blocks.FieldItems);

        if (result.Count == 0)
            result.Add("No differences found.");

        RTB_Diff.Lines = result.ToArray();
    }

    private void AppendDiff<T1, T2>(List<string> result, T1 update, T1 previous, [CallerArgumentExpression(nameof(update))] string title = null!)
        where T1 : IEventValueStorage<T2> where T2 : unmanaged, IEquatable<T2>
    {
        var diff = DiffBlocks<T1, T2>(update, previous);
        if (diff.Count == 0)
            return;
        result.Add("=====");
        result.Add($"{title}:");
        result.Add("=====");
        result.AddRange(diff);
        result.Add(string.Empty);
    }

    private List<string> DiffBlocks<T1, T2>(T1 update, T1 previous) where T1 : IEventValueStorage<T2> where T2 : unmanaged, IEquatable<T2>
    {
        List<string> result = [];
        HashSet<ulong> hashes = [];
        var count = update.Count;
        for (int i = 0; i < count; i++)
        {
            var hash = update.GetKey(i);
            if (hash == FnvHash.HashEmpty)
                break;
            var u = update.GetValue(i);

            var name = Names.GetName(hash);
            if (!previous.TryGetValue(hash, out var p))
                result.Add($"{name} added with value {u}");
            else if (!p.Equals(u))
                result.Add($"{name} changed from {p} to {u}");

            hashes.Add(hash);
        }

        // Check that none were removed.
        count = previous.Count;
        for (var i = 0; i < count; i++)
        {
            var hash = update.GetKey(i);
            if (hash == FnvHash.HashEmpty)
                break;
            if (hashes.Contains(hash))
                continue;
            var p = previous.GetValue(i);
            var name = Names.GetName(hash);
            result.Add($"{name} @ {i:X} removed, was {p}");
        }
        return result;
    }

    private static void Main_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e?.Data is null)
            return;
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
    }

    private void Main_DragDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;

        foreach (var file in files)
        {
            var result = this.SelectNewOld(file, B_LoadNew.Text, B_LoadOld.Text);
            if (result == DualDiffSelection.New)
                TB_NewSAV.Text = file;
            else if (result == DualDiffSelection.Old)
                TB_OldSAV.Text = file;
            else if (ModifierKeys == Keys.Escape)
                return;
        }
        ChangeSAV();
    }
}
