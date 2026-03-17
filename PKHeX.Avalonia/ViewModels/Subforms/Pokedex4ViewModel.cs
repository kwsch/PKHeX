using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static PKHeX.Core.Zukan4;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Gen 4 Pokedex species entry.
/// </summary>
public partial class Pokedex4EntryModel : ObservableObject
{
    public ushort Species { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _seen;

    [ObservableProperty]
    private bool _caught;

    [ObservableProperty]
    private bool[] _languages;

    /// <summary>
    /// Gender ratio category from PersonalInfo.
    /// 0 = dual gender, 1 = male only, 2 = female only, 3 = genderless.
    /// </summary>
    public int GenderType { get; }

    /// <summary>Whether this species can be either male or female.</summary>
    public bool IsDualGender => GenderType == 0;

    /// <summary>
    /// For dual-gender species: true if the first-seen gender is Female, false if Male.
    /// </summary>
    [ObservableProperty]
    private bool _isFemaleFirst;

    /// <summary>
    /// For dual-gender species: whether both genders have been seen.
    /// When true, second gender is the opposite of the first.
    /// </summary>
    [ObservableProperty]
    private bool _seenBothGenders;

    /// <summary>
    /// Whether this species has alternate form data in the dex.
    /// </summary>
    public bool HasForms { get; }

    /// <summary>
    /// Form names that have been seen in the dex.
    /// </summary>
    public ObservableCollection<string> SeenForms { get; } = [];

    /// <summary>
    /// Form names that have not been seen in the dex.
    /// </summary>
    public ObservableCollection<string> NotSeenForms { get; } = [];

    /// <summary>
    /// Display text summarizing gender state.
    /// </summary>
    public string GenderDisplay => GenderType switch
    {
        1 => "Male Only",
        2 => "Female Only",
        3 => "Genderless",
        _ when !Seen => "-",
        _ when SeenBothGenders => IsFemaleFirst ? "F, M" : "M, F",
        _ => IsFemaleFirst ? "F" : "M",
    };

    public Pokedex4EntryModel(ushort species, string label, bool seen, bool caught, bool[] languages,
        int genderType, bool isFemaleFirst, bool seenBothGenders, bool hasForms)
    {
        Species = species;
        Label = label;
        _seen = seen;
        _caught = caught;
        _languages = languages;
        GenderType = genderType;
        _isFemaleFirst = isFemaleFirst;
        _seenBothGenders = seenBothGenders;
        HasForms = hasForms;
    }

    partial void OnIsFemaleFirstChanged(bool value) => OnPropertyChanged(nameof(GenderDisplay));
    partial void OnSeenBothGendersChanged(bool value) => OnPropertyChanged(nameof(GenderDisplay));
    partial void OnSeenChanged(bool value) => OnPropertyChanged(nameof(GenderDisplay));
}

/// <summary>
/// ViewModel for the Gen 4 Pokedex editor.
/// Edits seen/caught/language/gender/form status per species.
/// </summary>
public partial class Pokedex4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4 _origin;
    private readonly SAV4 SAV4;
    private const int LangCount = 6;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _dexUpgraded;

    public string[] DexModes { get; }

    public ObservableCollection<Pokedex4EntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<Pokedex4EntryModel> _filteredEntries = [];

    /// <summary>
    /// The currently selected entry for detail editing (gender/forms).
    /// </summary>
    [ObservableProperty]
    private Pokedex4EntryModel? _selectedEntry;

    public Pokedex4ViewModel(SAV4 sav) : base(sav)
    {
        _origin = sav;
        SAV4 = (SAV4)sav.Clone();
        var speciesNames = GameInfo.Strings.specieslist;
        var dex = SAV4.Dex;

        for (ushort i = 1; i <= sav.MaxSpeciesID; i++)
        {
            var name = i < speciesNames.Length ? speciesNames[i] : $"Species {i}";
            var label = $"{i:000} - {name}";
            var langs = new bool[LangCount];
            if (dex.HasLanguage(i))
            {
                for (int l = 0; l < LangCount; l++)
                    langs[l] = dex.GetLanguageBitIndex(i, l);
            }

            // Gender info from PersonalInfo
            var pi = sav.Personal[i];
            var gr = pi.Gender;
            int genderType;
            bool isFemaleFirst = false;
            bool seenBothGenders = false;
            if (gr == PersonalInfo.RatioMagicGenderless)
                genderType = 3;
            else if (gr == PersonalInfo.RatioMagicMale)
                genderType = 1;
            else if (gr == PersonalInfo.RatioMagicFemale)
                genderType = 2;
            else
            {
                genderType = 0;
                isFemaleFirst = dex.GetSeenGenderFirst(i) == 1;
                seenBothGenders = !dex.GetSeenSingleGender(i);
            }

            // Form info
            var forms = dex.GetForms(i);
            bool hasForms = forms.Length > 0;

            var entry = new Pokedex4EntryModel(i, label, dex.GetSeen(i), dex.GetCaught(i), langs,
                genderType, isFemaleFirst, seenBothGenders, hasForms);

            if (hasForms)
                LoadFormsForEntry(entry, dex, forms);

            AllEntries.Add(entry);
        }

        FilteredEntries = new ObservableCollection<Pokedex4EntryModel>(AllEntries);

        string[] dexMode = ["not given", "simple mode", "detect forms", "national dex", "other languages"];
        if (sav is SAV4HGSS)
            dexMode = dexMode.Where((_, idx) => idx != 2).ToArray();
        DexModes = dexMode;
        _dexUpgraded = Math.Clamp(sav.DexUpgraded, 0, DexModes.Length - 1);
    }

    private static void LoadFormsForEntry(Pokedex4EntryModel entry, Zukan4 dex, byte[] forms)
    {
        string[] formNames = GetFormNames4Dex(entry.Species);
        bool seen = dex.GetSeen(entry.Species);

        var seenFormNames = forms
            .Where(z => seen && z != FORM_NONE && z < formNames.Length)
            .Distinct()
            .Select(z => formNames[z])
            .ToArray();

        var notSeenFormNames = formNames.Except(seenFormNames).ToArray();

        foreach (var f in seenFormNames)
            entry.SeenForms.Add(f);
        foreach (var f in notSeenFormNames)
            entry.NotSeenForms.Add(f);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<Pokedex4EntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<Pokedex4EntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void CompleteAll()
    {
        foreach (var entry in AllEntries)
        {
            entry.Seen = true;
            entry.Caught = true;
            var langs = new bool[LangCount];
            for (int i = 0; i < LangCount; i++)
                langs[i] = true;
            entry.Languages = langs;

            // Complete gender
            if (entry.IsDualGender)
            {
                entry.IsFemaleFirst = false; // Male first
                entry.SeenBothGenders = true;
            }

            // Complete forms
            if (entry.HasForms)
            {
                var allForms = entry.SeenForms.Concat(entry.NotSeenForms).Distinct().ToList();
                entry.SeenForms.Clear();
                entry.NotSeenForms.Clear();
                foreach (var f in allForms)
                    entry.SeenForms.Add(f);
            }
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var entry in AllEntries)
        {
            entry.Seen = false;
            entry.Caught = false;
            entry.Languages = new bool[LangCount];

            // Clear gender
            if (entry.IsDualGender)
            {
                entry.IsFemaleFirst = false;
                entry.SeenBothGenders = false;
            }

            // Clear forms
            if (entry.HasForms)
            {
                var allForms = entry.SeenForms.Concat(entry.NotSeenForms).Distinct().ToList();
                entry.SeenForms.Clear();
                entry.NotSeenForms.Clear();
                foreach (var f in allForms)
                    entry.NotSeenForms.Add(f);
            }
        }
    }

    /// <summary>
    /// Move a form from NotSeen to Seen for the selected entry.
    /// </summary>
    [RelayCommand]
    private void AddForm(string formName)
    {
        if (SelectedEntry is not { HasForms: true } entry)
            return;
        if (!entry.NotSeenForms.Remove(formName))
            return;
        entry.SeenForms.Add(formName);
    }

    /// <summary>
    /// Move a form from Seen to NotSeen for the selected entry.
    /// </summary>
    [RelayCommand]
    private void RemoveForm(string formName)
    {
        if (SelectedEntry is not { HasForms: true } entry)
            return;
        if (!entry.SeenForms.Remove(formName))
            return;
        entry.NotSeenForms.Add(formName);
    }

    /// <summary>
    /// Move a seen form up in the order (for form ordering).
    /// </summary>
    [RelayCommand]
    private void MoveFormUp(string formName)
    {
        if (SelectedEntry is not { HasForms: true } entry)
            return;
        int idx = entry.SeenForms.IndexOf(formName);
        if (idx <= 0)
            return;
        entry.SeenForms.Move(idx, idx - 1);
    }

    /// <summary>
    /// Move a seen form down in the order (for form ordering).
    /// </summary>
    [RelayCommand]
    private void MoveFormDown(string formName)
    {
        if (SelectedEntry is not { HasForms: true } entry)
            return;
        int idx = entry.SeenForms.IndexOf(formName);
        if (idx < 0 || idx >= entry.SeenForms.Count - 1)
            return;
        entry.SeenForms.Move(idx, idx + 1);
    }

    [RelayCommand]
    private void Save()
    {
        var dex = SAV4.Dex;
        foreach (var entry in AllEntries)
        {
            dex.SetCaught(entry.Species, entry.Caught);
            dex.SetSeen(entry.Species, entry.Seen);

            // Save gender flags
            dex.SetSeenGenderNeither(entry.Species);
            if (entry.Seen)
            {
                switch (entry.GenderType)
                {
                    case 0: // dual gender
                    {
                        byte firstGender = entry.IsFemaleFirst ? (byte)1 : (byte)0;
                        dex.SetSeenGenderNewFlag(entry.Species, firstGender);
                        if (entry.SeenBothGenders)
                            dex.SetSeenGenderSecond(entry.Species, firstGender ^ 1);
                        break;
                    }
                    case 1: // male only
                        dex.SetSeenGenderNewFlag(entry.Species, 0);
                        break;
                    case 2: // female only
                        dex.SetSeenGenderNewFlag(entry.Species, 1);
                        break;
                    // genderless: leave at neither (0, 0)
                }
            }

            // Save language flags
            if (dex.HasLanguage(entry.Species))
            {
                for (int i = 0; i < LangCount; i++)
                    dex.SetLanguageBitIndex(entry.Species, i, entry.Languages[i]);
            }

            // Save form flags
            if (entry.HasForms)
            {
                var forms = dex.GetForms(entry.Species);
                if (forms.Length != 0)
                {
                    string[] formNames = GetFormNames4Dex(entry.Species);
                    var arr = new byte[entry.SeenForms.Count];
                    for (int i = 0; i < entry.SeenForms.Count; i++)
                    {
                        int idx = Array.IndexOf(formNames, entry.SeenForms[i]);
                        arr[i] = (byte)(idx >= 0 ? idx : 0);
                    }
                    dex.SetForms(entry.Species, arr);
                }
            }
        }
        if (DexUpgraded >= 0)
            SAV4.DexUpgraded = DexUpgraded;

        _origin.CopyChangesFrom(SAV4);
        Modified = true;
    }
}
