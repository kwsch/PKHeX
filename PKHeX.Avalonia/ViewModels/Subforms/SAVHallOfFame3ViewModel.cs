using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 3 Hall of Fame viewer/editor.
/// Shows 50 entries with 6 team members each.
/// </summary>
public partial class SAVHallOfFame3ViewModel : SaveEditorViewModelBase
{
    private readonly SAV3 _sav;
    private readonly HallFame3Entry[] _fame;
    private int _prevEntry;
    private int _prevMember;
    private bool _loading;

    public List<int> EntryIndices { get; }
    public List<ComboItem> SpeciesList { get; }

    [ObservableProperty]
    private int _selectedEntryIndex;

    [ObservableProperty]
    private int _memberIndex;

    [ObservableProperty]
    private int _selectedSpecies;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private int _level;

    [ObservableProperty]
    private string _tid = "00000";

    [ObservableProperty]
    private string _sid = "00000";

    [ObservableProperty]
    private string _pid = "00000000";

    [ObservableProperty]
    private bool _isShiny;

    public SAVHallOfFame3ViewModel(SAV3 sav) : base(sav)
    {
        _sav = sav;
        _fame = HallFame3Entry.GetEntries(sav);

        EntryIndices = Enumerable.Range(0, 50).ToList();
        SpeciesList = GameInfo.FilteredSources.Species.ToList();

        _selectedEntryIndex = 0;
        _memberIndex = 0;
        _prevEntry = 0;
        _prevMember = 0;

        LoadEntry(_fame[0].Team[0]);
    }

    partial void OnSelectedEntryIndexChanged(int value)
    {
        if (_loading || value < 0)
            return;
        SaveEntry(_fame[_prevEntry].Team[_prevMember]);
        MemberIndex = 0;
        var pkm = _fame[value].Team[0];
        LoadEntry(pkm);
        _prevMember = 0;
        _prevEntry = value;
    }

    partial void OnMemberIndexChanged(int value)
    {
        if (_loading || value < 0 || SelectedEntryIndex < 0)
            return;
        SaveEntry(_fame[_prevEntry].Team[_prevMember]);
        var pkm = _fame[SelectedEntryIndex].Team[value];
        LoadEntry(pkm);
        _prevMember = value;
        _prevEntry = SelectedEntryIndex;
    }

    private void LoadEntry(HallFame3PKM pk)
    {
        _loading = true;
        Tid = pk.TID16.ToString("00000");
        Sid = pk.SID16.ToString("00000");
        Pid = pk.PID.ToString("X8");
        Nickname = pk.Nickname;
        Level = pk.Level;
        SelectedSpecies = pk.Species;
        IsShiny = pk.IsShiny;
        _loading = false;
    }

    private void SaveEntry(HallFame3PKM pk)
    {
        pk.TID16 = ushort.TryParse(Tid, out var tid) ? tid : (ushort)0;
        pk.SID16 = ushort.TryParse(Sid, out var sid) ? sid : (ushort)0;
        pk.PID = uint.TryParse(Pid, System.Globalization.NumberStyles.HexNumber, null, out var pid) ? pid : 0;
        if (pk.Nickname != Nickname)
            pk.Nickname = Nickname;
        pk.Level = Level;
        pk.Species = (ushort)SelectedSpecies;
    }

    [RelayCommand]
    private void ClearFields()
    {
        _loading = true;
        Tid = "0";
        Sid = "0";
        Pid = "0";
        Nickname = string.Empty;
        Level = 0;
        SelectedSpecies = 0;
        IsShiny = false;
        _loading = false;
    }

    [RelayCommand]
    private void Save()
    {
        var pkm = _fame[SelectedEntryIndex].Team[MemberIndex];
        SaveEntry(pkm);
        HallFame3Entry.SetEntries(_sav, _fame);
        Modified = true;
    }
}
