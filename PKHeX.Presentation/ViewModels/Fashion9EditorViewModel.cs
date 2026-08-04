using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Fashion9EditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    public bool IsSupported { get; }

    public ObservableCollection<FashionBlockViewModelBase> Tabs { get; } = [];

    public Fashion9EditorViewModel(SaveFile sav)
    {
        _sav = sav;
        IsSupported = sav is SAV9SV || sav is SAV9ZA;

        if (sav is SAV9SV sav9sv)
            LoadSV(sav9sv);
        else if (sav is SAV9ZA sav9za)
            LoadZA(sav9za);
    }

    private void LoadSV(SAV9SV sav)
    {
        AddBlockSV(sav, SaveBlockAccessor9SV.KFashionUnlockedEyewear, "Eyewear");
        AddBlockSV(sav, SaveBlockAccessor9SV.KFashionUnlockedGloves, "Gloves");
        AddBlockSV(sav, SaveBlockAccessor9SV.KFashionUnlockedBag, "Bag");
        AddBlockSV(sav, SaveBlockAccessor9SV.KFashionUnlockedFootwear, "Footwear");
        AddBlockSV(sav, SaveBlockAccessor9SV.KFashionUnlockedHeadwear, "Headwear");
        AddBlockSV(sav, SaveBlockAccessor9SV.KFashionUnlockedLegwear, "Legwear");
        AddBlockSV(sav, SaveBlockAccessor9SV.KFashionUnlockedClothing, "Clothing");
        AddBlockSV(sav, SaveBlockAccessor9SV.KFashionUnlockedPhoneCase, "Phone Case");
    }

    private void AddBlockSV(SAV9SV sav, uint blockId, string name)
    {
        var block = sav.Blocks.GetBlock(blockId);
        Tabs.Add(new FashionBlock9ViewModel(block, name));
    }

    private void LoadZA(SAV9ZA sav)
    {
        // Fashion
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionTops, "Tops");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionBottoms, "Bottoms");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionAllInOne, "All-In-One");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionHeadwear, "Headwear");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionEyewear, "Eyewear");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionGloves, "Gloves");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionLegwear, "Legwear");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionFootwear, "Footwear");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionSatchels, "Satchels");
        AddBlockZA(sav, SaveBlockAccessor9ZA.KFashionEarrings, "Earrings");

        // Hair/Makeup
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake00StyleHair, "Hair Style", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake01StyleBangs, "Bangs", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake02ColorHair, "Hair Color", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake03ColorHair, "Hair Color 2", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake04ColorHair, "Hair Color 3", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake05StyleEyebrow, "Eyebrow Style", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake06ColorEyebrow, "Eyebrow Color", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake07StyleEyes, "Eyes Style", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake08ColorEyes, "Eye Color", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake09StyleEyelash, "Eyelash Style", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake10ColorEyelash, "Eyelash Color", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake11Lips, "Lips", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake12BeautyMark, "Beauty Mark", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake13Freckles, "Freckles", true);
        AddBlockZA(sav, SaveBlockAccessor9ZA.KHairMake14DarkCircles, "Dark Circles", true);
    }

    private void AddBlockZA(SAV9ZA sav, uint blockId, string name, bool isHair = false)
    {
        var block = sav.Blocks.GetBlock(blockId);
        if (isHair)
            Tabs.Add(new HairMakeBlockViewModel(block, name));
        else
            Tabs.Add(new FashionBlock9aViewModel(block, name));
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var tab in Tabs)
            tab.Save();
        
        _sav.State.Edited = true;
    }

    [RelayCommand]
    private void Unlocks(string mode)
    {
        bool unlock = mode == "All"; // Simplify for now, logic determines if we unlock or clear
        // WinForms has SetAllOwned(state)
        // I'll implement SetAllOwned(true) for now as "Unlock All"
        
        // This could be "Unlock All" command
        foreach (var tab in Tabs)
            tab.SetAllOwned(true);
    }
}

public abstract class FashionBlockViewModelBase : ViewModelBase
{
    protected readonly SCBlock Block;
    public string Name { get; }

    protected FashionBlockViewModelBase(SCBlock block, string name)
    {
        Block = block;
        Name = name;
    }

    public abstract void Save();
    public abstract void SetAllOwned(bool state);
}

public partial class FashionBlock9ViewModel : FashionBlockViewModelBase
{
    public ObservableCollection<FashionItem9ViewModel> Items { get; } = [];

    public FashionBlock9ViewModel(SCBlock block, string name) : base(block, name)
    {
        Load();
    }

    private void Load()
    {
        var items = FashionItem9.GetArray(Block.Data);
        Items.Clear();
        for (int i = 0; i < items.Length; i++)
            Items.Add(new FashionItem9ViewModel(items[i]));
    }

    public override void Save()
    {
        var items = new FashionItem9[Items.Count];
        for (int i = 0; i < Items.Count; i++)
            items[i] = Items[i].Item; // Item is reference type, modified in place via properties
        FashionItem9.SetArray(items, Block.Data);
    }

    public override void SetAllOwned(bool state)
    {
        // SV doesn't have "IsOwned" flag exposed in the wrapper, only IsNew.
        // But logic might imply setting IDs?
        // Wait, SV items are "Unlocked" by presence or flags?
        // FashionItem9 has Value and Flags (IsNew).
        // It seems SV fashion unlock structure is just a list of unlocked item IDs? 
        // Or flags? 
        // FashionItem9 has "Value" which is ItemID.
        // WinForms FashionItem9Editor SetAllOwned does nothing.
    }
}

public partial class FashionItem9ViewModel : ViewModelBase
{
    public FashionItem9 Item { get; }

    public FashionItem9ViewModel(FashionItem9 item)
    {
        Item = item;
    }

    public uint Value
    {
        get => Item.Value;
        set
        {
            if (Item.Value == value) return;
            Item.Value = value;
            OnPropertyChanged();
        }
    }

    public bool IsNew
    {
        get => Item.IsNew;
        set
        {
            if (Item.IsNew == value) return;
            Item.IsNew = value;
            OnPropertyChanged();
        }
    }
}

public partial class FashionBlock9aViewModel : FashionBlockViewModelBase
{
    public ObservableCollection<FashionItem9aViewModel> Items { get; } = [];

    public FashionBlock9aViewModel(SCBlock block, string name) : base(block, name)
    {
        Load();
    }

    private void Load()
    {
        var items = FashionItem9a.GetArray(Block.Data);
        Items.Clear();
        for (int i = 0; i < items.Length; i++)
            Items.Add(new FashionItem9aViewModel(items[i]));
    }

    public override void Save()
    {
        var items = new FashionItem9a[Items.Count];
        for (int i = 0; i < Items.Count; i++)
            items[i] = Items[i].Item;
        FashionItem9a.SetArray(items, Block.Data);
    }

    public override void SetAllOwned(bool state)
    {
        foreach (var item in Items)
            item.IsOwned = state;
    }
}

public partial class FashionItem9aViewModel : ViewModelBase
{
    public FashionItem9a Item { get; }

    public FashionItem9aViewModel(FashionItem9a item)
    {
        Item = item;
    }

    public uint Value
    {
        get => Item.Value;
        set
        {
            if (Item.Value == value) return;
            Item.Value = value;
            OnPropertyChanged();
        }
    }

    public bool IsNew
    {
        get => Item.IsNew;
        set
        {
            if (Item.IsNew == value) return;
            Item.IsNew = value;
            OnPropertyChanged();
        }
    }

    public bool IsNewShop
    {
        get => Item.IsNewShop;
        set
        {
            if (Item.IsNewShop == value) return;
            Item.IsNewShop = value;
            OnPropertyChanged();
        }
    }

    public bool IsNewGroup
    {
        get => Item.IsNewGroup;
        set
        {
            if (Item.IsNewGroup == value) return;
            Item.IsNewGroup = value;
            OnPropertyChanged();
        }
    }

    public bool IsEquipped
    {
        get => Item.IsEquipped;
        set
        {
            if (Item.IsEquipped == value) return;
            Item.IsEquipped = value;
            OnPropertyChanged();
        }
    }

    public bool IsOwned
    {
        get => Item.IsOwned;
        set
        {
            if (Item.IsOwned == value) return;
            Item.IsOwned = value;
            OnPropertyChanged();
        }
    }
}

public partial class HairMakeBlockViewModel : FashionBlockViewModelBase
{
    public ObservableCollection<HairMakeItem9aViewModel> Items { get; } = [];

    public HairMakeBlockViewModel(SCBlock block, string name) : base(block, name)
    {
        Load();
    }

    private void Load()
    {
        var items = HairMakeItem9a.GetArray(Block.Data);
        Items.Clear();
        for (int i = 0; i < items.Length; i++)
            Items.Add(new HairMakeItem9aViewModel(items[i]));
    }

    public override void Save()
    {
        var items = new HairMakeItem9a[Items.Count];
        for (int i = 0; i < Items.Count; i++)
            items[i] = Items[i].Item;
        HairMakeItem9a.SetArray(items, Block.Data);
    }
    
    public override void SetAllOwned(bool state) { }
}

public partial class HairMakeItem9aViewModel : ViewModelBase
{
    public HairMakeItem9a Item { get; }

    public HairMakeItem9aViewModel(HairMakeItem9a item)
    {
        Item = item;
    }

    public uint Value
    {
        get => Item.Value;
        set
        {
            if (Item.Value == value) return;
            Item.Value = value;
            OnPropertyChanged();
        }
    }

    public bool IsNew
    {
        get => Item.IsNew;
        set
        {
            if (Item.IsNew == value) return;
            Item.IsNew = value;
            OnPropertyChanged();
        }
    }
}
