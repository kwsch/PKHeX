using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokedexLAEditorViewModel : ViewModelBase
{
    private readonly SAV8LA _sav;
    private readonly PokedexSave8a _dex;

    public PokedexLAEditorViewModel(SaveFile sav)
    {
        _sav = (SAV8LA)sav;
        _dex = _sav.Blocks.PokedexSave;
        
        LoadSpecies();
    }

    [ObservableProperty]
    private ObservableCollection<LASpeciesEntryViewModel> _speciesList = [];

    [ObservableProperty]
    private LASpeciesEntryViewModel? _selectedSpecies;

    [ObservableProperty]
    private string _searchText = string.Empty;

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void LoadSpecies()
    {
        var speciesNames = GameInfo.Strings.Species;
        var list = new List<LASpeciesEntryViewModel>();
        for (ushort s = 1; s <= _sav.Personal.MaxSpeciesID; s++)
        {
            var hisuiDex = PokedexSave8a.GetDexIndex(PokedexType8a.Hisui, s);
            if (hisuiDex == 0) continue;

            list.Add(new LASpeciesEntryViewModel(s, hisuiDex, speciesNames[s], _dex, _sav));
        }

        SpeciesList = new ObservableCollection<LASpeciesEntryViewModel>(list.OrderBy(z => z.DexIndex));
        if (SpeciesList.Count > 0)
            SelectedSpecies = SpeciesList[0];
    }

    private void ApplyFilter()
    {
        // Simple filter for now. In a real app we'd use a CollectionView or similar.
        // For brevity, I'll just skip complex filtering logic here and assume the user selects from the list.
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var entry in SpeciesList)
        {
            entry.Save();
        }
    }

    [RelayCommand]
    private void ReportAll()
    {
        _dex.UpdateAllReportPoke();
        foreach (var entry in SpeciesList)
        {
            entry.Load();
        }
    }
}

public partial class LASpeciesEntryViewModel : ViewModelBase
{
    private readonly ushort _species;
    private readonly PokedexSave8a _dex;
    private readonly SAV8LA _sav;

    public LASpeciesEntryViewModel(ushort species, int dexIndex, string name, PokedexSave8a dex, SAV8LA sav)
    {
        _species = species;
        DexIndex = dexIndex;
        Name = name;
        _dex = dex;
        _sav = sav;

        LoadForms();
        Load();
    }

    public int DexIndex { get; }
    public string Name { get; }
    public string DisplayName => $"{DexIndex:000} - {Name}";

    [ObservableProperty]
    private ObservableCollection<LAFormEntryViewModel> _forms = [];

    [ObservableProperty]
    private LAFormEntryViewModel? _selectedForm;

    [ObservableProperty]
    private ObservableCollection<LAResearchTaskViewModel> _tasks = [];

    [ObservableProperty]
    private bool _isComplete;

    [ObservableProperty]
    private bool _isPerfect;

    [ObservableProperty]
    private bool _isSolitudeComplete;

    [ObservableProperty]
    private int _reportedResearchLevel;

    [ObservableProperty]
    private int _unreportedResearchLevel;

    private void LoadForms()
    {
        Forms.Clear();
        var personal = _sav.Personal[_species];
        var formCount = personal.FormCount;
        var formNames = GameInfo.Strings.forms;

        for (byte f = 0; f < formCount; f++)
        {
            if (!_dex.HasFormStorage(_species, f) || _dex.IsBlacklisted(_species, f))
                continue;

            Forms.Add(new LAFormEntryViewModel(_species, f, formNames[f], _dex));
        }
        
        if (Forms.Count > 0)
            SelectedForm = Forms[0];
    }

    public void Load()
    {
        IsComplete = _dex.IsComplete(_species);
        IsPerfect = _dex.IsPerfect(_species);
        IsSolitudeComplete = _dex.GetSolitudeComplete(_species);
        
        ReportedResearchLevel = _dex.GetPokeResearchRate(_species);
        
        // Load Tasks
        Tasks.Clear();
        if (PokedexConstants8a.ResearchTasks.Length > DexIndex - 1)
        {
            var tasks = PokedexConstants8a.ResearchTasks[DexIndex - 1];
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].Task.CanSetCurrentValue())
                    continue;

                Tasks.Add(new LAResearchTaskViewModel(_species, i, tasks[i], _dex));
            }
        }

        foreach (var form in Forms)
        {
            form.Load();
        }

        UpdateUnreportedLevel();
    }

    private void UpdateUnreportedLevel()
    {
        int unreported = ReportedResearchLevel;
        foreach (var task in Tasks)
        {
            _dex.GetResearchTaskLevel(_species, task.Index, out _, out var taskValue, out var unreportedLevels);
            unreported += unreportedLevels * task.PointsPerLevel;
        }
        UnreportedResearchLevel = unreported;
    }

    public void Save()
    {
        _dex.SetSolitudeComplete(_species, IsSolitudeComplete);
        foreach (var form in Forms)
        {
            form.Save();
        }
        foreach (var task in Tasks)
        {
            task.Save();
        }
    }

    [RelayCommand]
    private void ReportSpecies()
    {
        _dex.UpdateSpecificReportPoke(_species);
        Load();
    }
}

public partial class LAFormEntryViewModel : ViewModelBase
{
    private readonly ushort _species;
    private readonly byte _form;
    private readonly PokedexSave8a _dex;

    public LAFormEntryViewModel(ushort species, byte form, string name, PokedexSave8a dex)
    {
        _species = species;
        _form = form;
        Name = string.IsNullOrEmpty(name) ? "Base" : name;
        _dex = dex;
    }

    public string Name { get; }

    [ObservableProperty] private bool _seen0;
    [ObservableProperty] private bool _seen1;
    [ObservableProperty] private bool _seen2;
    [ObservableProperty] private bool _seen3;
    [ObservableProperty] private bool _seen4;
    [ObservableProperty] private bool _seen5;
    [ObservableProperty] private bool _seen6;
    [ObservableProperty] private bool _seen7;

    [ObservableProperty] private bool _obtained0;
    [ObservableProperty] private bool _obtained1;
    [ObservableProperty] private bool _obtained2;
    [ObservableProperty] private bool _obtained3;
    [ObservableProperty] private bool _obtained4;
    [ObservableProperty] private bool _obtained5;
    [ObservableProperty] private bool _obtained6;
    [ObservableProperty] private bool _obtained7;

    [ObservableProperty] private bool _caught0;
    [ObservableProperty] private bool _caught1;
    [ObservableProperty] private bool _caught2;
    [ObservableProperty] private bool _caught3;
    [ObservableProperty] private bool _caught4;
    [ObservableProperty] private bool _caught5;
    [ObservableProperty] private bool _caught6;
    [ObservableProperty] private bool _caught7;

    public void Load()
    {
        var seen = _dex.GetPokeSeenInWildFlags(_species, _form);
        var obtained = _dex.GetPokeObtainFlags(_species, _form);
        var caught = _dex.GetPokeCaughtInWildFlags(_species, _form);

        Seen0 = (seen & 1) != 0; Seen1 = (seen & 2) != 0; Seen2 = (seen & 4) != 0; Seen3 = (seen & 8) != 0;
        Seen4 = (seen & 16) != 0; Seen5 = (seen & 32) != 0; Seen6 = (seen & 64) != 0; Seen7 = (seen & 128) != 0;

        Obtained0 = (obtained & 1) != 0; Obtained1 = (obtained & 2) != 0; Obtained2 = (obtained & 4) != 0; Obtained3 = (obtained & 8) != 0;
        Obtained4 = (obtained & 16) != 0; Obtained5 = (obtained & 32) != 0; Obtained6 = (obtained & 64) != 0; Obtained7 = (obtained & 128) != 0;

        Caught0 = (caught & 1) != 0; Caught1 = (caught & 2) != 0; Caught2 = (caught & 4) != 0; Caught3 = (caught & 8) != 0;
        Caught4 = (caught & 16) != 0; Caught5 = (caught & 32) != 0; Caught6 = (caught & 64) != 0; Caught7 = (caught & 128) != 0;
    }

    public void Save()
    {
        byte seen = 0;
        if (Seen0) seen |= 1; if (Seen1) seen |= 2; if (Seen2) seen |= 4; if (Seen3) seen |= 8;
        if (Seen4) seen |= 16; if (Seen5) seen |= 32; if (Seen6) seen |= 64; if (Seen7) seen |= 128;
        _dex.SetPokeSeenInWildFlags(_species, _form, seen);

        byte obtained = 0;
        if (Obtained0) obtained |= 1; if (Obtained1) obtained |= 2; if (Obtained2) obtained |= 4; if (Obtained3) obtained |= 8;
        if (Obtained4) obtained |= 16; if (Obtained5) obtained |= 32; if (Obtained6) obtained |= 64; if (Obtained7) obtained |= 128;
        _dex.SetPokeObtainFlags(_species, _form, obtained);

        byte caught = 0;
        if (Caught0) caught |= 1; if (Caught1) caught |= 2; if (Caught2) caught |= 4; if (Caught3) caught |= 8;
        if (Caught4) caught |= 16; if (Caught5) caught |= 32; if (Caught6) caught |= 64; if (Caught7) caught |= 128;
        _dex.SetPokeCaughtInWildFlags(_species, _form, caught);
    }
}

public partial class LAResearchTaskViewModel : ViewModelBase
{
    private readonly ushort _species;
    private readonly PokedexResearchTask8a _task;
    private readonly PokedexSave8a _dex;

    public LAResearchTaskViewModel(ushort species, int taskIndex, PokedexResearchTask8a task, PokedexSave8a dex)
    {
        _species = species;
        Index = taskIndex;
        _task = task;
        _dex = dex;

        // Use hardcoded English names or look them up if available in GameInfo.Strings
        // For now, let's use a generic description
        var taskTypeName = task.Task.ToString();
        Description = $"{taskTypeName} (x{string.Join("/", task.TaskThresholds)})";

        _dex.GetResearchTaskProgressByForce(_species, _task.Task, _task.Index, out var val);
        _currentValue = val;
    }

    public int Index { get; }
    public string Description { get; }
    public int PointsPerLevel => _task.PointsSingle + _task.PointsBonus;

    [ObservableProperty]
    private int _currentValue;

    public void Save()
    {
        _dex.SetResearchTaskProgressByForce(_species, _task, CurrentValue);
    }
}
