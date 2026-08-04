using CommunityToolkit.Mvvm.ComponentModel;

namespace PKHeX.Presentation.Models;

/// <summary>
/// Data for a single box slot, displayed in the BoxViewer grid.
/// </summary>
public partial class SlotData : ObservableObject
{
    public SlotLocation Location => SlotLocation.FromBox(Box, Slot);
    
    [ObservableProperty] private int _slot;
    [ObservableProperty] private int _box;
    [ObservableProperty] private ushort _species;
    [ObservableProperty] private byte[]? _sprite;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private bool _isShiny;
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private string _speciesName = string.Empty;
    [ObservableProperty] private byte _level;
    [ObservableProperty] private byte _gender; // 0=Male, 1=Female, 2=Genderless
    [ObservableProperty] private ushort _heldItem;
    [ObservableProperty] private string _heldItemName = string.Empty;
    [ObservableProperty] private bool _isEgg;
    [ObservableProperty] private byte _form;
    [ObservableProperty] private ushort _ability;
    [ObservableProperty] private string _abilityName = string.Empty;
    [ObservableProperty] private byte _nature;
    [ObservableProperty] private string _natureName = string.Empty;
    [ObservableProperty] private string _showdownSummary = string.Empty;
    [ObservableProperty] private bool _isLegal;
    
    /// <summary>
    /// Short summary for tooltip.
    /// </summary>
    public string ToolTipSummary => IsEmpty
        ? "Empty"
        : ShowdownSummary;

    /// <summary>
    /// Concise, screen-reader-friendly announcement for this slot, e.g.
    /// "Slot 12: Pikachu, Lv. 25, shiny" or "Slot 3: Empty".
    /// </summary>
    public string AccessibleName
    {
        get
        {
            var header = $"Slot {Slot + 1}";
            if (IsEmpty)
                return $"{header}: Empty";

            if (IsEgg)
                return $"{header}: {SpeciesName} Egg";

            var parts = new System.Collections.Generic.List<string>
            {
                SpeciesName,
                $"Lv. {Level}",
            };
            if (IsShiny)
                parts.Add("shiny");
            if (!IsLegal)
                parts.Add("illegal");

            return $"{header}: {string.Join(", ", parts)}";
        }
    }

    public string GenderSymbol => Gender switch
    {
        0 => "♂",
        1 => "♀",
        _ => ""
    };
}
