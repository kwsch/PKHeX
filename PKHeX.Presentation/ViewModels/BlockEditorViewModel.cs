using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class BlockEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IDialogService _dialogService;

    public BlockEditorViewModel(SaveFile sav, IDialogService dialogService)
    {
        _sav = sav;
        _dialogService = dialogService;
        LoadBlocks();
    }

    [ObservableProperty]
    private ObservableCollection<BlockViewModel> _blocks = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<BlockViewModel> _filteredBlocks = [];

    [ObservableProperty]
    private BlockViewModel? _selectedBlock;

    partial void OnSearchTextChanged(string value) => FilterBlocks();

    private void LoadBlocks()
    {
        Blocks.Clear();

        // Gen 8/9 SCBlocks
        if (_sav is ISCBlockArray sc)
        {
            foreach (var b in sc.AllBlocks)
            {
                Blocks.Add(new BlockViewModel(b));
            }
        }
        // Gen 6/7/8b BlockInfo
        else if (TryGetBlockInfo(_sav, out var blockInfos))
        {
            foreach (var b in blockInfos)
            {
                Blocks.Add(new BlockViewModel(b, _sav.Buffer));
            }
        }

        FilterBlocks();
    }

    private bool TryGetBlockInfo(SaveFile sav, out IReadOnlyList<BlockInfo> blocks)
    {
        blocks = null!;
        
        // Use reflection to find a property named "Blocks" that implements ISaveBlockAccessor<out T>
        var blocksProp = sav.GetType().GetProperty("Blocks");
        if (blocksProp == null)
            return false;

        var accessor = blocksProp.GetValue(sav);
        if (accessor == null)
            return false;

        var blockInfoProp = accessor.GetType().GetProperty("BlockInfo");
        if (blockInfoProp == null)
            return false;

        var value = blockInfoProp.GetValue(accessor);
        if (value is IEnumerable<BlockInfo> biList)
        {
            blocks = biList.ToList();
            return true;
        }

        return false;
    }

    private void FilterBlocks()
    {
        var search = SearchText.ToLowerInvariant();
        FilteredBlocks.Clear();
        foreach (var b in Blocks)
        {
            if (string.IsNullOrEmpty(search) || 
                b.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                b.Key.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                b.Offset.ToString("X").Contains(search, StringComparison.OrdinalIgnoreCase))
            {
                FilteredBlocks.Add(b);
            }
        }
    }

    [RelayCommand]
    private async Task ExportBlockAsync()
    {
        if (SelectedBlock == null) return;
        var data = SelectedBlock.GetData();
        if (data == null) return;

        var fileName = SelectedBlock.Key.Length > 0 ? SelectedBlock.Key : SelectedBlock.Name;
        var path = await _dialogService.SaveFileAsync("Export Block", fileName + ".bin");
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            await System.IO.File.WriteAllBytesAsync(path, data);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BlockEditor_ExportErrorTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private async Task ImportBlockAsync()
    {
        if (SelectedBlock == null) return;
        var path = await _dialogService.OpenFileAsync(LocalizedStrings.Instance["BlockEditor_ImportBlockTitle"], ["*.*"]);
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            var data = await System.IO.File.ReadAllBytesAsync(path);
            if (data.Length != SelectedBlock.Length)
            {
                await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BlockEditor_ImportErrorTitle"], LocalizedStrings.Instance.Format("BlockEditor_SizeMismatch", SelectedBlock.Length, data.Length));
                return;
            }

            SelectedBlock.SetData(data);
            _sav.State.Edited = true;
            await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["BlockEditor_ImportSuccessTitle"], LocalizedStrings.Instance.Format("BlockEditor_ImportSuccessMessage", SelectedBlock.Name));
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BlockEditor_ImportErrorTitle"], ex.Message);
        }
    }
}

public class BlockViewModel
{
    public string Name { get; }
    public string Key { get; }
    public int Offset { get; }
    public int Length { get; }
    public string SizeString => $"0x{Length:X}";
    public string OffsetString => Offset >= 0 ? $"0x{Offset:X5}" : "N/A";

    private readonly SCBlock? _sc;
    private readonly BlockInfo? _bi;
    private readonly Memory<byte> _savData;

    public BlockViewModel(SCBlock sc)
    {
        _sc = sc;
        Name = sc.Type.ToString();
        Key = $"{sc.Key:X8}";
        Offset = -1;
        Length = sc.Data.Length;
    }

    public BlockViewModel(BlockInfo bi, Memory<byte> savData)
    {
        _bi = bi;
        _savData = savData;
        Name = bi.ID.ToString();
        Key = string.Empty;
        Offset = bi.Offset;
        Length = bi.Length;
    }

    public byte[] GetData()
    {
        if (_sc != null) return _sc.Data.ToArray();
        if (_bi != null) return _savData.Span.Slice(_bi.Offset, _bi.Length).ToArray();
        return [];
    }

    public void SetData(byte[] data)
    {
        if (_sc != null) _sc.ChangeData(data);
        if (_bi != null) data.CopyTo(_savData.Span[_bi.Offset..]);
    }
}
