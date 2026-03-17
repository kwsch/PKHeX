using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single gear item entry.
/// </summary>
public partial class GearEntryModel : ObservableObject
{
    public int Index { get; }
    public string CharacterStyle { get; }
    public string Category { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _unlocked;

    public GearEntryModel(int index, string characterStyle, string category, string name, bool unlocked)
    {
        Index = index;
        CharacterStyle = characterStyle;
        Category = category;
        Name = name;
        _unlocked = unlocked;
    }
}

/// <summary>
/// ViewModel for the Gear editor (Gen 4 Battle Revolution).
/// Edits gear unlock status and shiny outfit flags.
/// </summary>
public partial class Gear4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4BR _origin;
    private readonly SAV4BR SAV4BR;

    public ObservableCollection<GearEntryModel> GearItems { get; } = [];

    [ObservableProperty] private bool _shinyGroudon;
    [ObservableProperty] private bool _shinyLucario;
    [ObservableProperty] private bool _shinyElectivire;
    [ObservableProperty] private bool _shinyKyogre;
    [ObservableProperty] private bool _shinyRoserade;
    [ObservableProperty] private bool _shinyPachirisu;

    public Gear4ViewModel(SAV4BR sav) : base(sav)
    {
        _origin = sav;
        SAV4BR = (SAV4BR)sav.Clone();

        _shinyGroudon = sav.GearShinyGroudonOutfit;
        _shinyLucario = sav.GearShinyLucarioOutfit;
        _shinyElectivire = sav.GearShinyElectivireOutfit;
        _shinyKyogre = sav.GearShinyKyogreOutfit;
        _shinyRoserade = sav.GearShinyRoseradeOutfit;
        _shinyPachirisu = sav.GearShinyPachirisuOutfit;

        LoadGear();
    }

    private void LoadGear()
    {
        GearItems.Clear();
        var gearNames = GameLanguage.GetStrings("gear", "en");
        var modelNames = new[] { "(None)", "Young Boy", "Cool Boy", "Young Girl", "Cool Girl", "Muscle Man", "Little Girl" };
        var categoryNames = new[] { "Head", "Hair", "Face", "Glasses", "Top", "Hands", "Bottom", "Shoes", "Badges", "Bags" };

        for (ModelBR model = ModelBR.YoungBoy; model <= ModelBR.LittleGirl; model++)
        {
            for (GearCategory category = 0; (int)category < GearUnlock.CategoryCount; category++)
            {
                var (offset, count) = GearUnlock.GetOffsetCount(model, category);
                for (int i = 0; i < count; i++)
                {
                    var gearIndex = offset + i;
                    bool shared = category is GearCategory.Badges && i != 0;
                    var modelName = shared ? "All" : ((int)model < modelNames.Length ? modelNames[(int)model] : model.ToString());
                    var catName = (int)category < categoryNames.Length ? categoryNames[(int)category] : category.ToString();
                    var name = gearIndex < gearNames.Length ? gearNames[gearIndex] : $"Gear {gearIndex}";
                    GearItems.Add(new GearEntryModel(gearIndex, modelName, catName, name, SAV4BR.GearUnlock.Get(gearIndex)));
                }
            }
        }
    }

    [RelayCommand]
    private void UnlockAll()
    {
        SAV4BR.GearUnlock.UnlockAll();
        RefreshGear();
    }

    [RelayCommand]
    private void ClearAll()
    {
        SAV4BR.GearUnlock.Clear();
        RefreshGear();
    }

    private void RefreshGear()
    {
        foreach (var item in GearItems)
            item.Unlocked = SAV4BR.GearUnlock.Get(item.Index);
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var item in GearItems)
            SAV4BR.GearUnlock.Set(item.Index, item.Unlocked);

        SAV4BR.GearShinyGroudonOutfit = ShinyGroudon;
        SAV4BR.GearShinyLucarioOutfit = ShinyLucario;
        SAV4BR.GearShinyElectivireOutfit = ShinyElectivire;
        SAV4BR.GearShinyKyogreOutfit = ShinyKyogre;
        SAV4BR.GearShinyRoseradeOutfit = ShinyRoserade;
        SAV4BR.GearShinyPachirisuOutfit = ShinyPachirisu;

        _origin.CopyChangesFrom(SAV4BR);
        Modified = true;
    }
}
