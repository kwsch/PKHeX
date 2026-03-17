using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a fly destination entry.
/// </summary>
public partial class FlyDestModel : ObservableObject
{
    public int FlagIndex { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _unlocked;

    public FlyDestModel(int flagIndex, string name, bool unlocked)
    {
        FlagIndex = flagIndex;
        Name = name;
        _unlocked = unlocked;
    }
}

/// <summary>
/// Model for a Poketch app entry.
/// </summary>
public partial class PoketchAppModel : ObservableObject
{
    public int Index { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _unlocked;

    public PoketchAppModel(int index, string label, bool unlocked)
    {
        Index = index;
        Label = label;
        _unlocked = unlocked;
    }
}

/// <summary>
/// Model for a Walker course entry.
/// </summary>
public partial class WalkerCourseModel : ObservableObject
{
    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _unlocked;

    public WalkerCourseModel(int index, string name, bool unlocked)
    {
        Index = index;
        Name = name;
        _unlocked = unlocked;
    }
}

/// <summary>
/// ViewModel for the Misc Gen 4 editor.
/// Consolidates coins, BP, fly destinations, Poketch, walker, records, etc.
/// </summary>
public partial class Misc4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4 _origin;
    private readonly SAV4 SAV4;

    // General
    [ObservableProperty] private uint _coins;
    [ObservableProperty] private int _bp;
    [ObservableProperty] private uint _maxCoins;

    // Fly Destinations
    public ObservableCollection<FlyDestModel> FlyDestinations { get; } = [];

    // Poketch (Sinnoh only)
    public bool ShowPoketch { get; }
    public ObservableCollection<PoketchAppModel> PoketchApps { get; } = [];

    [ObservableProperty]
    private int _currentPoketchApp;

    public string[] PoketchAppNames { get; }

    // Underground Flags (Sinnoh only)
    public bool ShowUGFlags { get; }

    [ObservableProperty]
    private uint _ugFlagsCaptured;

    // Walker (HGSS only)
    public bool ShowWalker { get; }
    public ObservableCollection<WalkerCourseModel> WalkerCourses { get; } = [];

    [ObservableProperty] private uint _walkerWatts;
    [ObservableProperty] private uint _walkerSteps;

    // Pokeathlon (HGSS only)
    public bool ShowPokeathlon { get; }

    [ObservableProperty]
    private uint _pokeathlonPoints;

    // Map (HGSS only)
    public bool ShowMap { get; }
    public string[] MapOptions { get; } = ["Map Johto", "Map Johto+", "Map Johto & Kanto"];

    [ObservableProperty]
    private int _mapUpgradeIndex;

    // Records
    [ObservableProperty] private int _record16Index;
    [ObservableProperty] private int _record16Value;
    [ObservableProperty] private int _record32Index;
    [ObservableProperty] private uint _record32Value;

    private readonly Record4 Record;

    private const int FlyFlagStart = 2480;
    private static ReadOnlySpan<byte> FlyWorkFlagSinnoh => [000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 067, 068];
    private static ReadOnlySpan<byte> LocationIDsSinnoh => [001, 002, 003, 004, 005, 082, 083, 006, 007, 008, 009, 010, 011, 012, 013, 014, 054, 081, 055, 015];
    private static ReadOnlySpan<byte> FlyWorkFlagHGSS   => [000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019, 020, 021, 022, 027, 030, 033, 035];
    private static ReadOnlySpan<byte> LocationIDsHGSS   => [138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 229, 227, 221, 225];

    public Misc4ViewModel(SAV4 sav) : base(sav)
    {
        _origin = sav;
        SAV4 = (SAV4)sav.Clone();
        Record = SAV4.Records;

        _maxCoins = (uint)SAV4.MaxCoins;
        _coins = Math.Clamp(SAV4.Coin, 0, _maxCoins);
        _bp = Math.Clamp(SAV4.BP, 0, 9999);

        PoketchAppNames = GameInfo.Strings.poketchapps;

        // Fly destinations
        var locations = SAV4 is SAV4Sinnoh ? LocationIDsSinnoh : LocationIDsHGSS;
        var flags = SAV4 is SAV4Sinnoh ? FlyWorkFlagSinnoh : FlyWorkFlagHGSS;
        for (int i = 0; i < locations.Length; i++)
        {
            var flagIndex = FlyFlagStart + flags[i];
            var state = SAV4.GetEventFlag(flagIndex);
            var locationID = locations[i];
            var name = GameInfo.Strings.Gen4.Met0[locationID];
            FlyDestinations.Add(new FlyDestModel(flagIndex, name, state));
        }

        // Sinnoh-specific
        ShowPoketch = SAV4 is SAV4Sinnoh;
        ShowUGFlags = SAV4 is SAV4Sinnoh;
        ShowWalker = SAV4 is SAV4HGSS;
        ShowPokeathlon = SAV4 is SAV4HGSS;
        ShowMap = SAV4 is SAV4HGSS;

        if (SAV4 is SAV4Sinnoh sinnoh)
        {
            _ugFlagsCaptured = Math.Clamp(sinnoh.UG_FlagsCaptured, 0, SAV4Sinnoh.UG_MAX);

            for (PoketchApp app = 0; app <= PoketchApp.Alarm_Clock; app++)
            {
                var name = (int)app < PoketchAppNames.Length ? PoketchAppNames[(int)app] : app.ToString();
                var title = $"{(int)app:00} - {name}";
                PoketchApps.Add(new PoketchAppModel((int)app, title, sinnoh.GetPoketchAppUnlocked(app)));
            }
            _currentPoketchApp = sinnoh.CurrentPoketchApp;
        }
        else if (SAV4 is SAV4HGSS hgss)
        {
            // Walker
            ReadOnlySpan<string> walkerCourseNames = GameInfo.Sources.Strings.walkercourses;
            Span<bool> courses = stackalloc bool[SAV4HGSS.PokewalkerCourseFlagCount];
            hgss.GetPokewalkerCoursesUnlocked(courses);
            for (int i = 0; i < walkerCourseNames.Length; i++)
                WalkerCourses.Add(new WalkerCourseModel(i, walkerCourseNames[i], i < courses.Length && courses[i]));

            _walkerWatts = hgss.PokewalkerWatts;
            _walkerSteps = hgss.PokewalkerSteps;
            _pokeathlonPoints = hgss.PokeathlonPoints;

            var mapState = hgss.MapUnlockState;
            if (mapState >= MapUnlockState4.Invalid)
                mapState = MapUnlockState4.JohtoKanto;
            _mapUpgradeIndex = (int)mapState;
        }

        // Records
        _record16Value = Record.GetRecord16(0);
        _record32Value = Record.GetRecord32(0);
    }

    partial void OnRecord16IndexChanged(int value)
    {
        if (value >= 0 && value < Record4.Record16)
            Record16Value = Record.GetRecord16(value);
    }

    partial void OnRecord32IndexChanged(int value)
    {
        if (value >= 0 && value < Record.Record32)
            Record32Value = Record.GetRecord32(value);
    }

    [RelayCommand]
    private void UnlockAllFlyDest()
    {
        foreach (var dest in FlyDestinations)
            dest.Unlocked = true;
    }

    [RelayCommand]
    private void UnlockAllPoketchApps()
    {
        foreach (var app in PoketchApps)
            app.Unlocked = true;
    }

    [RelayCommand]
    private void UnlockAllWalkerCourses()
    {
        if (SAV4 is SAV4HGSS hgss)
        {
            hgss.PokewalkerCoursesUnlockAll();
            Span<bool> courses = stackalloc bool[SAV4HGSS.PokewalkerCourseFlagCount];
            hgss.GetPokewalkerCoursesUnlocked(courses);
            for (int i = 0; i < WalkerCourses.Count && i < courses.Length; i++)
                WalkerCourses[i].Unlocked = courses[i];
        }
    }

    [RelayCommand]
    private void Save()
    {
        SAV4.Coin = Coins;
        SAV4.BP = (ushort)Math.Clamp(Bp, 0, ushort.MaxValue);

        // Fly destinations
        foreach (var dest in FlyDestinations)
            SAV4.SetEventFlag(dest.FlagIndex, dest.Unlocked);

        if (SAV4 is SAV4Sinnoh sinnoh)
        {
            sinnoh.UG_FlagsCaptured = UgFlagsCaptured;

            // Poketch
            int unlockedCount = 0;
            sinnoh.CurrentPoketchApp = (sbyte)Math.Clamp(CurrentPoketchApp, sbyte.MinValue, sbyte.MaxValue);
            foreach (var app in PoketchApps)
            {
                sinnoh.SetPoketchAppUnlocked((PoketchApp)app.Index, app.Unlocked);
                if (app.Unlocked) unlockedCount++;
            }
            sinnoh.PoketchUnlockedCount = (byte)unlockedCount;
        }
        else if (SAV4 is SAV4HGSS hgss)
        {
            // Walker
            Span<bool> courses = stackalloc bool[SAV4HGSS.PokewalkerCourseFlagCount];
            for (int i = 0; i < WalkerCourses.Count && i < courses.Length; i++)
                courses[i] = WalkerCourses[i].Unlocked;
            hgss.SetPokewalkerCoursesUnlocked(courses);
            hgss.PokewalkerWatts = WalkerWatts;
            hgss.PokewalkerSteps = WalkerSteps;
            hgss.PokeathlonPoints = PokeathlonPoints;
            hgss.MapUnlockState = (MapUnlockState4)MapUpgradeIndex;
        }

        // Records
        Record.SetRecord16(Record16Index, (ushort)Math.Clamp(Record16Value, 0, ushort.MaxValue));
        Record.SetRecord32(Record32Index, Record32Value);
        Record.EndAccess();

        _origin.CopyChangesFrom(SAV4);
        Modified = true;
    }
}
