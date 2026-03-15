using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static PKHeX.Core.SCBlockUtil;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a block key entry in the block dump viewer.
/// </summary>
public class BlockKeyModel
{
    public string Text { get; }
    public int Value { get; }

    public BlockKeyModel(string text, int value)
    {
        Text = text;
        Value = value;
    }

    public override string ToString() => Text;
}

/// <summary>
/// ViewModel for the Gen 8+ SCBlock data dump viewer.
/// Allows browsing, viewing, importing, and exporting block data.
/// </summary>
public partial class BlockDump8ViewModel : SaveEditorViewModelBase
{
    private readonly ISCBlockArray _sav;
    private readonly SCBlockMetadata _metadata;
    private readonly ComboItem[] _sortedBlockKeys;

    [ObservableProperty] private string _blockDetail = string.Empty;
    [ObservableProperty] private string _hexDump = string.Empty;
    [ObservableProperty] private string _blockName = string.Empty;
    [ObservableProperty] private bool _hasBlockName;
    [ObservableProperty] private string _filterText = string.Empty;

    public ObservableCollection<BlockKeyModel> BlockKeys { get; } = [];

    [ObservableProperty] private BlockKeyModel? _selectedBlock;

    private SCBlock? _currentBlock;

    public BlockDump8ViewModel(ISCBlockArray sav) : base((SaveFile)sav)
    {
        _sav = sav;

        var extra = GetExtraKeyNames(sav);
        _metadata = new SCBlockMetadata(sav.Accessor, extra);

        _sortedBlockKeys = _metadata.GetSortedBlockKeyList().ToArray();
        foreach (var key in _sortedBlockKeys)
            BlockKeys.Add(new BlockKeyModel(key.Text, key.Value));

        if (BlockKeys.Count > 0)
            SelectedBlock = BlockKeys[0];
    }

    partial void OnSelectedBlockChanged(BlockKeyModel? value)
    {
        if (value == null)
            return;

        var key = (uint)value.Value;
        _currentBlock = _sav.Accessor.GetBlock(key);
        UpdateBlockDisplay();
    }

    private void UpdateBlockDisplay()
    {
        if (_currentBlock == null)
            return;

        BlockDetail = GetBlockSummary(_currentBlock);

        var sb = new StringBuilder();
        foreach (var b in _currentBlock.Data)
            sb.Append($"{b:X2} ");
        HexDump = sb.ToString();

        var blockName = _metadata.GetBlockName(_currentBlock, out _);
        if (blockName is not null)
        {
            HasBlockName = true;
            BlockName = blockName;
        }
        else
        {
            HasBlockName = false;
            BlockName = string.Empty;
        }
    }

    partial void OnFilterTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        BlockKeys.Clear();
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            foreach (var key in _sortedBlockKeys)
                BlockKeys.Add(new BlockKeyModel(key.Text, key.Value));
        }
        else
        {
            foreach (var key in _sortedBlockKeys)
            {
                if (key.Text.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                    BlockKeys.Add(new BlockKeyModel(key.Text, key.Value));
            }
        }
        if (BlockKeys.Count > 0)
            SelectedBlock = BlockKeys[0];
    }

    private static string[] GetExtraKeyNames(ISCBlockArray obj)
    {
        return [];
    }

    [RelayCommand]
    private void Save()
    {
        Modified = true;
    }
}
