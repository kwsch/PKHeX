using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Apricorn entry.
/// </summary>
public partial class ApricornEntryModel : ObservableObject
{
    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private int _count;

    public ApricornEntryModel(int index, string name, int count)
    {
        Index = index;
        Name = name;
        _count = count;
    }
}

/// <summary>
/// ViewModel for the Apricorn editor (Gen 4 HGSS).
/// Edits apricorn counts.
/// </summary>
public partial class Apricorn4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4HGSS SAV4H;
    private const int Count = 7;
    private const int ItemNameBase = 485;

    private static ReadOnlySpan<byte> ItemNameOffset => [0, 2, 1, 3, 4, 5, 6];

    public ObservableCollection<ApricornEntryModel> Apricorns { get; } = [];

    public Apricorn4ViewModel(SAV4HGSS sav) : base(sav)
    {
        SAV4H = sav;
        var itemNames = GameInfo.Strings.itemlist;

        for (int i = 0; i < Count; i++)
        {
            var itemId = ItemNameBase + ItemNameOffset[i];
            var name = itemId < itemNames.Length ? itemNames[itemId] : $"Apricorn {i}";
            Apricorns.Add(new ApricornEntryModel(i, name, sav.GetApricornCount(i)));
        }
    }

    [RelayCommand]
    private void GiveAll()
    {
        foreach (var a in Apricorns)
            a.Count = 99;
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var a in Apricorns)
            a.Count = 0;
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var a in Apricorns)
            SAV4H.SetApricornCount(a.Index, Math.Min(byte.MaxValue, a.Count));

        SAV.State.Edited = true;
        Modified = true;
    }
}
