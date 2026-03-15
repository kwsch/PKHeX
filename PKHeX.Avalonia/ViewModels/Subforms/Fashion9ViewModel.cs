using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single fashion item row in the grid.
/// </summary>
public partial class FashionItemModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty] private uint _value;
    [ObservableProperty] private bool _isNew;
    [ObservableProperty] private bool _isNewShop;
    [ObservableProperty] private bool _isNewGroup;
    [ObservableProperty] private bool _isEquipped;
    [ObservableProperty] private bool _isOwned;

    /// <summary>Whether this is a ZA fashion item (has extra columns).</summary>
    public bool IsExtended { get; }

    public FashionItemModel(int index, bool isExtended)
    {
        Index = index;
        IsExtended = isExtended;
    }
}

/// <summary>
/// Model for a single fashion block tab.
/// </summary>
public partial class FashionBlockModel : ObservableObject
{
    public string Name { get; }
    public string DisplayName { get; }
    public SCBlock Block { get; }
    public bool IsExtended { get; }
    public bool IsHairMake { get; }
    public ObservableCollection<FashionItemModel> Items { get; } = [];

    public FashionBlockModel(string name, SCBlock block, bool isExtended, bool isHairMake)
    {
        Name = name;
        Block = block;
        IsExtended = isExtended;
        IsHairMake = isHairMake;

        if (name.StartsWith("KHairMake"))
            DisplayName = name.Replace("KHairMake", "");
        else if (name.StartsWith("KFashionUnlocked"))
            DisplayName = name.Replace("KFashionUnlocked", "");
        else
            DisplayName = name.Replace("KFashion", "");
    }
}

/// <summary>
/// ViewModel for the Gen 9 Fashion/Clothing editor (SV and ZA).
/// </summary>
public partial class Fashion9ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SaveFile _sav;

    public ObservableCollection<FashionBlockModel> Tabs { get; } = [];

    [ObservableProperty] private FashionBlockModel? _selectedTab;
    [ObservableProperty] private bool _showSetAllOwned;

    public Fashion9ViewModel(SAV9SV sav) : base(sav)
    {
        _sav = (_origin = sav).Clone();
        ShowSetAllOwned = false;
        InitBlocksSV(((SAV9SV)_sav).Blocks);
        LoadAll();
    }

    public Fashion9ViewModel(SAV9ZA sav) : base(sav)
    {
        _sav = (_origin = sav).Clone();
        ShowSetAllOwned = true;
        InitBlocksZA(((SAV9ZA)_sav).Blocks);
        LoadAll();
    }

    private void InitBlocksSV(SaveBlockAccessor9SV accessor)
    {
        AddBlock(accessor, SaveBlockAccessor9SV.KFashionUnlockedEyewear, nameof(SaveBlockAccessor9SV.KFashionUnlockedEyewear), false, false);
        AddBlock(accessor, SaveBlockAccessor9SV.KFashionUnlockedGloves, nameof(SaveBlockAccessor9SV.KFashionUnlockedGloves), false, false);
        AddBlock(accessor, SaveBlockAccessor9SV.KFashionUnlockedBag, nameof(SaveBlockAccessor9SV.KFashionUnlockedBag), false, false);
        AddBlock(accessor, SaveBlockAccessor9SV.KFashionUnlockedFootwear, nameof(SaveBlockAccessor9SV.KFashionUnlockedFootwear), false, false);
        AddBlock(accessor, SaveBlockAccessor9SV.KFashionUnlockedHeadwear, nameof(SaveBlockAccessor9SV.KFashionUnlockedHeadwear), false, false);
        AddBlock(accessor, SaveBlockAccessor9SV.KFashionUnlockedLegwear, nameof(SaveBlockAccessor9SV.KFashionUnlockedLegwear), false, false);
        AddBlock(accessor, SaveBlockAccessor9SV.KFashionUnlockedClothing, nameof(SaveBlockAccessor9SV.KFashionUnlockedClothing), false, false);
        AddBlock(accessor, SaveBlockAccessor9SV.KFashionUnlockedPhoneCase, nameof(SaveBlockAccessor9SV.KFashionUnlockedPhoneCase), false, false);
    }

    private void InitBlocksZA(SaveBlockAccessor9ZA accessor)
    {
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionTops, nameof(SaveBlockAccessor9ZA.KFashionTops), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionBottoms, nameof(SaveBlockAccessor9ZA.KFashionBottoms), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionAllInOne, nameof(SaveBlockAccessor9ZA.KFashionAllInOne), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionHeadwear, nameof(SaveBlockAccessor9ZA.KFashionHeadwear), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionEyewear, nameof(SaveBlockAccessor9ZA.KFashionEyewear), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionGloves, nameof(SaveBlockAccessor9ZA.KFashionGloves), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionLegwear, nameof(SaveBlockAccessor9ZA.KFashionLegwear), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionFootwear, nameof(SaveBlockAccessor9ZA.KFashionFootwear), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionSatchels, nameof(SaveBlockAccessor9ZA.KFashionSatchels), true, false);
        AddBlock(accessor, SaveBlockAccessor9ZA.KFashionEarrings, nameof(SaveBlockAccessor9ZA.KFashionEarrings), true, false);

        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake00StyleHair, nameof(SaveBlockAccessor9ZA.KHairMake00StyleHair), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake01StyleBangs, nameof(SaveBlockAccessor9ZA.KHairMake01StyleBangs), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake02ColorHair, nameof(SaveBlockAccessor9ZA.KHairMake02ColorHair), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake03ColorHair, nameof(SaveBlockAccessor9ZA.KHairMake03ColorHair), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake04ColorHair, nameof(SaveBlockAccessor9ZA.KHairMake04ColorHair), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake05StyleEyebrow, nameof(SaveBlockAccessor9ZA.KHairMake05StyleEyebrow), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake06ColorEyebrow, nameof(SaveBlockAccessor9ZA.KHairMake06ColorEyebrow), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake07StyleEyes, nameof(SaveBlockAccessor9ZA.KHairMake07StyleEyes), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake08ColorEyes, nameof(SaveBlockAccessor9ZA.KHairMake08ColorEyes), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake09StyleEyelash, nameof(SaveBlockAccessor9ZA.KHairMake09StyleEyelash), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake10ColorEyelash, nameof(SaveBlockAccessor9ZA.KHairMake10ColorEyelash), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake11Lips, nameof(SaveBlockAccessor9ZA.KHairMake11Lips), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake12BeautyMark, nameof(SaveBlockAccessor9ZA.KHairMake12BeautyMark), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake13Freckles, nameof(SaveBlockAccessor9ZA.KHairMake13Freckles), false, true);
        AddBlock(accessor, SaveBlockAccessor9ZA.KHairMake14DarkCircles, nameof(SaveBlockAccessor9ZA.KHairMake14DarkCircles), false, true);
    }

    private void AddBlock(SCBlockAccessor accessor, uint key, string name, bool isExtended, bool isHairMake)
    {
        var block = accessor.GetBlock(key);
        var tab = new FashionBlockModel(name, block, isExtended, isHairMake);
        Tabs.Add(tab);
    }

    private void LoadAll()
    {
        foreach (var tab in Tabs)
            LoadTab(tab);

        if (Tabs.Count > 0)
            SelectedTab = Tabs[0];
    }

    private static void LoadTab(FashionBlockModel tab)
    {
        tab.Items.Clear();
        if (tab.IsExtended)
        {
            var array = FashionItem9a.GetArray(tab.Block.Data);
            for (int i = 0; i < array.Length; i++)
            {
                var item = array[i];
                var model = new FashionItemModel(i, true)
                {
                    Value = item.Value,
                    IsNew = item.IsNew,
                    IsNewShop = item.IsNewShop,
                    IsNewGroup = item.IsNewGroup,
                    IsEquipped = item.IsEquipped,
                    IsOwned = item.IsOwned,
                };
                tab.Items.Add(model);
            }
        }
        else if (tab.IsHairMake)
        {
            var array = HairMakeItem9a.GetArray(tab.Block.Data);
            for (int i = 0; i < array.Length; i++)
            {
                var item = array[i];
                var model = new FashionItemModel(i, false)
                {
                    Value = item.Value,
                    IsNew = item.IsNew,
                };
                tab.Items.Add(model);
            }
        }
        else
        {
            var array = FashionItem9.GetArray(tab.Block.Data);
            for (int i = 0; i < array.Length; i++)
            {
                var item = array[i];
                var model = new FashionItemModel(i, false)
                {
                    Value = item.Value,
                    IsNew = item.IsNew,
                };
                tab.Items.Add(model);
            }
        }
    }

    private static void SaveTab(FashionBlockModel tab)
    {
        if (tab.IsExtended)
        {
            var array = FashionItem9a.GetArray(tab.Block.Data);
            for (int i = 0; i < array.Length && i < tab.Items.Count; i++)
            {
                var model = tab.Items[i];
                array[i].Value = model.Value;
                array[i].IsNew = model.IsNew;
                array[i].IsNewShop = model.IsNewShop;
                array[i].IsNewGroup = model.IsNewGroup;
                array[i].IsEquipped = model.IsEquipped;
                array[i].IsOwned = model.IsOwned;
            }
            FashionItem9a.SetArray(array, tab.Block.Data);
        }
        else if (tab.IsHairMake)
        {
            var array = HairMakeItem9a.GetArray(tab.Block.Data);
            for (int i = 0; i < array.Length && i < tab.Items.Count; i++)
            {
                var model = tab.Items[i];
                array[i].Value = model.Value;
                array[i].IsNew = model.IsNew;
            }
            HairMakeItem9a.SetArray(array, tab.Block.Data);
        }
        else
        {
            var array = FashionItem9.GetArray(tab.Block.Data);
            for (int i = 0; i < array.Length && i < tab.Items.Count; i++)
            {
                var model = tab.Items[i];
                array[i].Value = model.Value;
                array[i].IsNew = model.IsNew;
            }
            FashionItem9.SetArray(array, tab.Block.Data);
        }
    }

    [RelayCommand]
    private void SetAllOwned()
    {
        if (SelectedTab is not { IsExtended: true } tab)
            return;
        FashionItem9a.ModifyAll(tab.Block.Data, z => z.IsOwned = true);
        LoadTab(tab);
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var tab in Tabs)
            SaveTab(tab);
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}
