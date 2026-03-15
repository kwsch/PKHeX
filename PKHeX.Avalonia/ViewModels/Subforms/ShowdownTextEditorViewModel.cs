using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Showdown Text Editor subform.
/// Multi-line text editor for Showdown import/export with parse preview.
/// </summary>
public partial class ShowdownTextEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private string _showdownText = string.Empty;

    [ObservableProperty]
    private string _previewText = string.Empty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private bool _modified;

    /// <summary>The parsed showdown sets from the text.</summary>
    public List<ShowdownSet> ParsedSets { get; } = [];

    /// <summary>
    /// Initializes empty for new text entry.
    /// </summary>
    public ShowdownTextEditorViewModel()
    {
    }

    /// <summary>
    /// Initializes with a PKM exported as Showdown text.
    /// </summary>
    public ShowdownTextEditorViewModel(PKM pk)
    {
        ShowdownText = ShowdownParsing.GetShowdownText(pk);
        ParsePreview();
    }

    /// <summary>
    /// Initializes with existing showdown text.
    /// </summary>
    public ShowdownTextEditorViewModel(string text)
    {
        ShowdownText = text;
        ParsePreview();
    }

    /// <summary>
    /// Parses the current text and updates the preview.
    /// </summary>
    [RelayCommand]
    private void ParsePreview()
    {
        ParsedSets.Clear();
        if (string.IsNullOrWhiteSpace(ShowdownText))
        {
            PreviewText = string.Empty;
            StatusText = "No text to parse.";
            return;
        }

        try
        {
            var sets = ShowdownParsing.GetShowdownSets(ShowdownText);
            var preview = new System.Text.StringBuilder();
            int count = 0;

            foreach (var set in sets)
            {
                if (set.Species == 0)
                    continue;
                ParsedSets.Add(set);
                count++;

                var species = GameInfo.Strings.specieslist;
                var name = set.Species < species.Length ? species[set.Species] : $"Species {set.Species}";
                preview.AppendLine($"--- Set {count}: {name} ---");
                preview.AppendLine($"  Level: {set.Level}");
                if (set.Ability >= 0)
                {
                    var abilities = GameInfo.Strings.abilitylist;
                    var abilName = set.Ability < abilities.Length ? abilities[set.Ability] : $"Ability {set.Ability}";
                    preview.AppendLine($"  Ability: {abilName}");
                }
                if (set.HeldItem > 0)
                {
                    var items = GameInfo.Strings.itemlist;
                    var itemName = set.HeldItem < items.Length ? items[set.HeldItem] : $"Item {set.HeldItem}";
                    preview.AppendLine($"  Item: {itemName}");
                }
                var moves = GameInfo.Strings.movelist;
                for (int i = 0; i < set.Moves.Length; i++)
                {
                    var move = set.Moves[i];
                    if (move == 0) continue;
                    var moveName = move < moves.Length ? moves[move] : $"Move {move}";
                    preview.AppendLine($"  Move {i + 1}: {moveName}");
                }
                if (set.InvalidLines.Count > 0)
                {
                    foreach (var err in set.InvalidLines)
                        preview.AppendLine($"  [Warning] {err}");
                }
                preview.AppendLine();
            }

            PreviewText = preview.ToString();
            StatusText = $"Parsed {count} set(s).";
        }
        catch (Exception ex)
        {
            PreviewText = string.Empty;
            StatusText = $"Parse error: {ex.Message}";
        }
    }

    /// <summary>
    /// Marks the text as accepted for import.
    /// </summary>
    [RelayCommand]
    private void Accept()
    {
        ParsePreview();
        Modified = true;
    }
}
