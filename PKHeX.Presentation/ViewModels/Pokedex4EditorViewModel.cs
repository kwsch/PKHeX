using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Pokedex4EditorViewModel : ViewModelBase
{
    private readonly SAV4? _sav;
    private readonly Zukan4? _zukan;
    
    public Pokedex4EditorViewModel(SaveFile sav)
    {
        _sav = sav as SAV4;
        _zukan = _sav?.Dex as Zukan4;
        IsSupported = _sav is not null && _zukan is not null;

        if (IsSupported)
        {
            var speciesList = GameInfo.Strings.Species;
            Species = new ObservableCollection<ComboItem>(
                Enumerable.Range(1, _sav!.MaxSpeciesID)
                .Select(i => new ComboItem(speciesList[i], i)));
                
            LoadDexModeOptions();
            
            _selectedSpecies = Species.FirstOrDefault(s => s.Value == 1) ?? Species.FirstOrDefault();
            if (_selectedSpecies != null)
                LoadEntry(_selectedSpecies.Value);
        }
    }

    public bool IsSupported { get; }
    public ObservableCollection<ComboItem> Species { get; } = [];
    
    [ObservableProperty]
    private ComboItem? _selectedSpecies;
    
    [ObservableProperty]
    private string _searchText = "";

    partial void OnSearchTextChanged(string value)
    {
        // Filter logic could be added here or via a ICollectionView
        // For now, we rely on the view binding to a filtered source if desired, 
        // or just simple selection for this editor.
        // Implementing simple search selection:
        if (string.IsNullOrWhiteSpace(value)) return;
        
        var match = Species.FirstOrDefault(s => s.Text.Contains(value, StringComparison.OrdinalIgnoreCase));
        if (match != null)
            SelectedSpecies = match;
    }

    partial void OnSelectedSpeciesChanged(ComboItem? value)
    {
        if (value is not null)
            LoadEntry(value.Value);
    }
    
    // Dex Mode
    public ObservableCollection<string> DexModes { get; } = [];
    
    [ObservableProperty]
    private int _dexMode;
    
    partial void OnDexModeChanged(int value)
    {
        if (_sav is not null && value >= 0)
            _sav.DexUpgraded = value;
    }
    
    private void LoadDexModeOptions()
    {
        DexModes.Clear();
        string[] modes = ["Not Given", "Simple Mode", "Detect Forms", "National Dex", "Other Languages"];
        // 2 (Detect Forms) is skipped in HGSS
        if (_sav is SAV4HGSS)
        {
            DexModes.Add(modes[0]);
            DexModes.Add(modes[1]);
            DexModes.Add(modes[3]);
            DexModes.Add(modes[4]);
        }
        else
        {
            foreach (var m in modes) DexModes.Add(m);
        }
        
        if (_sav != null && _sav.DexUpgraded < DexModes.Count)
            DexMode = _sav.DexUpgraded;
    }

    // Entry Data
    [ObservableProperty] private bool _caught;
    [ObservableProperty] private bool _seen;
    
    // Genders
    public ObservableCollection<string> SeenGenders { get; } = [];
    public ObservableCollection<string> UnseenGenders { get; } = [];
    
    [ObservableProperty] private string? _selectedSeenGender;
    [ObservableProperty] private string? _selectedUnseenGender;

    // Forms
    public ObservableCollection<string> SeenForms { get; } = [];
    public ObservableCollection<string> UnseenForms { get; } = [];
    
    [ObservableProperty] private string? _selectedSeenForm;
    [ObservableProperty] private string? _selectedUnseenForm;
    
    // Languages
    public bool HasLanguages => _zukan != null && SelectedSpecies != null && _zukan.HasLanguage((ushort)SelectedSpecies.Value);
    
    [ObservableProperty] private bool _langJPN;
    [ObservableProperty] private bool _langENG;
    [ObservableProperty] private bool _langFRA;
    [ObservableProperty] private bool _langGER;
    [ObservableProperty] private bool _langITA;
    [ObservableProperty] private bool _langSPA;
    // No KOR in Gen 4

    private void LoadEntry(int species)
    {
        if (_zukan is null || _sav is null) return;
        
        var sp = (ushort)species;
        Caught = _zukan.GetCaught(sp);
        Seen = _zukan.GetSeen(sp);
        
        LoadGenders(sp);
        LoadForms(sp);
        LoadLanguages(sp);
        
        OnPropertyChanged(nameof(HasLanguages));
    }
    
    private void LoadGenders(ushort species)
    {
        SeenGenders.Clear();
        UnseenGenders.Clear();
        
        // Logic from SAV_Pokedex4.LoadGenders
        if (_sav is null || _zukan is null) return;
        
        bool seen = _zukan.GetSeen(species);
        var firstList = seen ? SeenGenders : UnseenGenders;
        var secondList = _zukan.GetSeenSingleGender(species) ? UnseenGenders : firstList;
        
        var pi = _sav.Personal[species];
        var gr = pi.Gender;
        
        switch (gr)
        {
            case PersonalInfo.RatioMagicGenderless: // Genderless
                firstList.Add(Zukan4.GENDERLESS);
                break;
            case PersonalInfo.RatioMagicMale: // Male only
                firstList.Add(Zukan4.MALE);
                break;
            case PersonalInfo.RatioMagicFemale: // Female only
                firstList.Add(Zukan4.FEMALE);
                break;
            default: // Dual gender
                var firstFem = _zukan.GetSeenGenderFirst(species) == 1;
                firstList.Add(firstFem ? Zukan4.FEMALE : Zukan4.MALE);
                secondList.Add(firstFem ? Zukan4.MALE : Zukan4.FEMALE);
                break;
        }
    }
    
    private void LoadForms(ushort species)
    {
        SeenForms.Clear();
        UnseenForms.Clear();
        
        if (_zukan is null) return;
        
        var forms = _zukan.GetForms(species);
        if (forms.Length == 0) return;
        
        string[] formNames = Zukan4.GetFormNames4Dex(species);
        var seenChecked = _zukan.GetSeen(species);
        
        // Zukan4 GetForms returns byte[] of form indices (or maxvalue for none)
        // SAV_Pokedex4 uses forms.Length to determine count
        
        // Wait, Zukan4.GetForms(species) returns byte[], but SAV_Pokedex4 uses Zukan4.GetForms(species).
        // Let's re-read Zukan4.cs GetForms logic.
        // It returns a byte array of form INDICES.
        // E.g. Shellos: byte[2]. 0=West, 1=East.
        // If Data says 1 form seen, it might return [0, 255] or [0]. 
        // Actually, specific implementaiton in Zukan4 returns filled array with mapped bits.
        // e.g. for Shellos, it calls GetDexFormValues(val, 1, 2) -> returns byte[2].
        // If not seen, GetDexFormValues might return 0s? 
        // No, GetDexFormValues extracts bits.
        
        // Re-reading SAV_Pokedex4.LoadForms:
        /*
        var forms = SAV.Dex.GetForms(species); // byte array of indices
        var seen = forms.Where(z => seen_checked && z != FORM_NONE && z < forms.Length).Distinct().Select((_, i) => formNames[forms[i]]).ToArray();
        */
        // This implies `forms` contains the INDICES of forms that ARE SEEN (or available?).
        // Actually, GetForms returns the RAW form values stored in the save?
        // Zukan4.GetForms returns `GetDexFormValues`.
        // GetDexFormValues(Value, BitsPerForm, ReadCt) returns byte[ReadCt].
        // Each byte is a form index.
        // So for Shellos (ReadCt=2), it returns 2 bytes. 
        // If form 0 is seen, form 0 index is in array?
        // Actually, Gen 4 Dex form storage is weird.
        // It seems to store "Order in which forms were seen".
        // e.g. if I see West (0) then East (1), save stores [0, 1].
        // If I see East (1) then West (0), save stores [1, 0].
        // So `GetForms` returns this ordered list.
        // Unseen slots are `FORM_NONE` (255).
        
        var seenValues = forms.Where(z => z != Zukan4.FORM_NONE).ToArray();
        foreach (var val in seenValues)
        {
             if (val < formNames.Length)
                SeenForms.Add(formNames[val]);
        }
        
        // Available forms that are NOT in seenValues
        for (int i = 0; i < formNames.Length; i++)
        {
            if (!seenValues.Contains((byte)i))
                UnseenForms.Add(formNames[i]);
        }
    }
    
    private void LoadLanguages(ushort species)
    {
        if (_zukan is null || _sav is null) return;
        
        if (_zukan.HasLanguage(species))
        {
            LangJPN = _zukan.GetLanguageBitIndex(species, 0); // JPN
            LangENG = _zukan.GetLanguageBitIndex(species, 1); // ENG
            LangFRA = _zukan.GetLanguageBitIndex(species, 2); // FRA
            LangGER = _zukan.GetLanguageBitIndex(species, 3); // GER
            LangITA = _zukan.GetLanguageBitIndex(species, 4); // ITA
            LangSPA = _zukan.GetLanguageBitIndex(species, 5); // SPA
        }
        else
        {
            LangJPN = LangENG = LangFRA = LangGER = LangITA = LangSPA = false;
        }
    }
    
    [RelayCommand]
    private void Save()
    {
        if (_zukan is null || _sav is null || SelectedSpecies is null) return;
        
        var sp = (ushort)SelectedSpecies.Value;
        
        _zukan.SetCaught(sp, Caught);
        _zukan.SetSeen(sp, Seen);
        _zukan.SetSeenGenderNeither(sp);
        
        if (SeenGenders.Count > 0)
        {
            var femaleFirst = SeenGenders[0] == Zukan4.FEMALE;
            var firstGender = femaleFirst ? (byte)1 : (byte)0;
            _zukan.SetSeenGenderNewFlag(sp, firstGender);
            
            if (SeenGenders.Count > 1) // Both seen
                _zukan.SetSeenGenderSecond(sp, (byte)(firstGender ^ 1));
        }
        
        if (_zukan.HasLanguage(sp))
        {
            _zukan.SetLanguageBitIndex(sp, 0, LangJPN);
            _zukan.SetLanguageBitIndex(sp, 1, LangENG);
            _zukan.SetLanguageBitIndex(sp, 2, LangFRA);
            _zukan.SetLanguageBitIndex(sp, 3, LangGER);
            _zukan.SetLanguageBitIndex(sp, 4, LangITA);
            _zukan.SetLanguageBitIndex(sp, 5, LangSPA);
        }
        
        // Forms
        if (SeenForms.Count > 0)
        {
            var formNames = Zukan4.GetFormNames4Dex(sp);
            var formIndices = new byte[SeenForms.Count];
            for (int i = 0; i < SeenForms.Count; i++)
            {
                var idx = Array.IndexOf(formNames, SeenForms[i]);
                formIndices[i] = idx >= 0 ? (byte)idx : Zukan4.FORM_NONE;
            }
            _zukan.SetForms(sp, formIndices);
        }
        else
        {
            _zukan.SetForms(sp, []);
        }
        
        _sav.State.Edited = true;
    }    
    // Commands for lists
    [RelayCommand]
    private void RemoveGender()
    {
        if (!string.IsNullOrEmpty(SelectedSeenGender))
        {
            var item = SelectedSeenGender;
            SeenGenders.Remove(item);
            UnseenGenders.Add(item);
            SelectedSeenGender = null;
        }
    }

    [RelayCommand]
    private void AddGender()
    {
        if (!string.IsNullOrEmpty(SelectedUnseenGender))
        {
            var item = SelectedUnseenGender;
            UnseenGenders.Remove(item);
            SeenGenders.Add(item);
            SelectedUnseenGender = null;
        }
    }
    
    [RelayCommand]
    private void MoveGenderUp()
    {
        if (string.IsNullOrEmpty(SelectedSeenGender)) return;
        var index = SeenGenders.IndexOf(SelectedSeenGender);
        if (index > 0)
        {
            SeenGenders.Move(index, index - 1);
        }
    }
    
    [RelayCommand]
    private void MoveGenderDown()
    {
        if (string.IsNullOrEmpty(SelectedSeenGender)) return;
        var index = SeenGenders.IndexOf(SelectedSeenGender);
        if (index < SeenGenders.Count - 1)
        {
            SeenGenders.Move(index, index + 1);
        }
    }
    
    [RelayCommand]
    private void RemoveForm()
    {
        if (!string.IsNullOrEmpty(SelectedSeenForm))
        {
            var item = SelectedSeenForm;
            SeenForms.Remove(item);
            UnseenForms.Add(item);
            SelectedSeenForm = null;
        }
    }

    [RelayCommand]
    private void AddForm()
    {
        if (!string.IsNullOrEmpty(SelectedUnseenForm))
        {
            var item = SelectedUnseenForm;
            UnseenForms.Remove(item);
            SeenForms.Add(item);
            SelectedUnseenForm = null;
        }
    }
    
    [RelayCommand]
    private void MoveFormUp()
    {
        if (string.IsNullOrEmpty(SelectedSeenForm)) return;
        var index = SeenForms.IndexOf(SelectedSeenForm);
        if (index > 0)
        {
            SeenForms.Move(index, index - 1);
        }
    }
    
    [RelayCommand]
    private void MoveFormDown()
    {
        if (string.IsNullOrEmpty(SelectedSeenForm)) return;
        var index = SeenForms.IndexOf(SelectedSeenForm);
        if (index < SeenForms.Count - 1)
        {
            SeenForms.Move(index, index + 1);
        }
    }
}
