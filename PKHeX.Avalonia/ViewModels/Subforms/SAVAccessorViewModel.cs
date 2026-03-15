using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the SAV_Accessor subform.
/// Provides block-level access to save file data for inspection.
/// </summary>
public partial class SAVAccessorViewModel : ObservableObject
{
    private readonly SaveBlockMetadata<BlockInfo> _metadata;

    [ObservableProperty]
    private string? _selectedBlockName;

    [ObservableProperty]
    private string _blockSummary = string.Empty;

    /// <summary>Sorted list of accessible block names.</summary>
    public ObservableCollection<string> BlockNames { get; } = [];

    public SAVAccessorViewModel(SaveFile sav, ISaveBlockAccessor<BlockInfo> accessor)
    {
        _metadata = new SaveBlockMetadata<BlockInfo>(accessor);
        foreach (var name in _metadata.GetSortedBlockList())
            BlockNames.Add(name);

        if (BlockNames.Count > 0)
            SelectedBlockName = BlockNames[0];
    }

    partial void OnSelectedBlockNameChanged(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            BlockSummary = string.Empty;
            return;
        }

        var block = _metadata.GetBlock(value);
        BlockSummary = $"Block: {value}\nData Length: {block.Data.Length} bytes";
    }
}
