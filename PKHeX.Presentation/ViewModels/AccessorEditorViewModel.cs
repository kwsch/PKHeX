using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using System.Linq;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Accessor Editor for viewing/editing raw SCBlocks (Gen 8+).
/// Corresponds to SAV_Accessor / SAV_BlockDump8 in WinForms.
/// </summary>
public partial class AccessorEditorViewModel : ViewModelBase
{
    private readonly ISaveBlockAccessor<BlockInfo>? _accessor;
    private readonly SaveBlockMetadata<BlockInfo>? _metadata;

    public AccessorEditorViewModel(SaveFile sav)
    {
        if (sav is ISaveBlockAccessor<BlockInfo> acc)
        {
            _accessor = acc;
            IsSupported = true;
            _metadata = new SaveBlockMetadata<BlockInfo>(acc);
            
            foreach (var b in _metadata.GetSortedBlockList())
            {
                BlockKeys.Add(b);
            }
            if (BlockKeys.Count > 0)
                SelectedKey = BlockKeys[0];
        }
        else
        {
            IsSupported = false;
        }
    }

    public bool IsSupported { get; }

    public ObservableCollection<string> BlockKeys { get; } = [];

    [ObservableProperty] private string _selectedKey = string.Empty;
    [ObservableProperty] private string _blockDetails = string.Empty;

    partial void OnSelectedKeyChanged(string value)
    {
        if (_metadata != null && !string.IsNullOrEmpty(value))
        {
            var block = _metadata.GetBlock(value);
            BlockDetails = block != null 
                ? $"Block: {value}\nType: {block.GetType().Name}\n{block}" 
                : "Block not found";
        }
    }
}
