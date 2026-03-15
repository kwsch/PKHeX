using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Batch Editor subform.
/// Allows mass modification of PKM in boxes using instruction strings.
/// </summary>
public partial class BatchEditorViewModel : SaveEditorViewModelBase
{
    [ObservableProperty]
    private string _instructions = string.Empty;

    [ObservableProperty]
    private string _resultLog = string.Empty;

    [ObservableProperty]
    private bool _isExecuting;

    [ObservableProperty]
    private int _selectedScope;

    [ObservableProperty]
    private double _progress;

    /// <summary>
    /// Scope options: 0 = Current Box, 1 = All Boxes, 2 = Party
    /// </summary>
    public string[] ScopeOptions { get; } = ["Current Box", "All Boxes", "Party"];

    /// <summary>
    /// The current box index, set by the caller to scope edits.
    /// </summary>
    public int CurrentBox { get; set; }

    public BatchEditorViewModel(SaveFile sav) : base(sav)
    {
    }

    [RelayCommand]
    private async Task ExecuteAsync()
    {
        var text = Instructions;
        if (string.IsNullOrWhiteSpace(text))
        {
            ResultLog = "No instructions provided.";
            return;
        }

        if (StringInstructionSet.HasEmptyLine(text.AsSpan()))
        {
            ResultLog = "Error: Instructions contain an empty line. Remove blank lines between instructions.";
            return;
        }

        var sets = StringInstructionSet.GetBatchSets(text.AsSpan());
        if (sets.Length == 0)
        {
            ResultLog = "No valid instruction sets found.";
            return;
        }

        if (Array.Exists(sets, s => s.Instructions.Count == 0))
        {
            ResultLog = "Error: One or more instruction sets have no modifications.";
            return;
        }

        IsExecuting = true;
        ResultLog = "Executing...";
        Progress = 0;

        try
        {
            var result = await Task.Run(() => RunBatchEdit(sets));
            ResultLog = result;
            Modified = true;
        }
        catch (Exception ex)
        {
            ResultLog = $"Error: {ex.Message}";
        }
        finally
        {
            IsExecuting = false;
            Progress = 100;
        }
    }

    private string RunBatchEdit(StringInstructionSet[] sets)
    {
        foreach (var set in sets)
        {
            EntityBatchEditor.ScreenStrings(set.Filters);
            EntityBatchEditor.ScreenStrings(set.Instructions);
        }

        var editor = new EntityBatchProcessor();

        var data = new List<SlotCache>();
        switch (SelectedScope)
        {
            case 0: // Current Box
                SlotInfoLoader.AddBoxData(SAV, data);
                // Filter to current box only
                data = data.Where(s => s.Source is SlotInfoBox b && b.Box == CurrentBox).ToList();
                break;
            case 1: // All Boxes
                SlotInfoLoader.AddBoxData(SAV, data);
                break;
            case 2: // Party
                SlotInfoLoader.AddPartyData(SAV, data);
                break;
        }

        if (data.Count == 0)
            return "No data to process.";

        foreach (var set in sets)
        {
            foreach (var entry in data)
            {
                var pk = entry.Entity;
                if (pk.Species == 0)
                    continue;
                editor.Process(pk, set.Filters, set.Instructions);
            }
        }

        // Write back modified data
        foreach (var slot in data)
            slot.Source.WriteTo(SAV, slot.Entity, EntityImportSettings.None);

        return editor.GetEditorResults(sets);
    }

    [RelayCommand]
    private void ClearInstructions()
    {
        Instructions = string.Empty;
        ResultLog = string.Empty;
    }

    [RelayCommand]
    private void AddSampleInstructions()
    {
        Instructions = string.Join(Environment.NewLine,
            ".Level=100",
            ".CurrentFriendship=255"
        );
    }
}
