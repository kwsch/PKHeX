using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class UndergroundEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly SAV4Sinnoh? _sinnoh;

    public UndergroundEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        _sinnoh = sav as SAV4Sinnoh;
        IsSupported = _sinnoh is not null;

        if (IsSupported)
            LoadData();
    }

    public bool IsSupported { get; }

    // Statistics
    [ObservableProperty] private uint _playersMet;
    partial void OnPlayersMetChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_PeopleMet = value; }

    [ObservableProperty] private uint _giftsGiven;
    partial void OnGiftsGivenChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_GiftsGiven = value; }

    [ObservableProperty] private uint _giftsReceived;
    partial void OnGiftsReceivedChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_GiftsReceived = value; }

    [ObservableProperty] private uint _spheresBuried;
    partial void OnSpheresBuriedChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_Spheres = value; }

    [ObservableProperty] private uint _fossilsDugUp;
    partial void OnFossilsDugUpChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_Fossils = value; }

    [ObservableProperty] private uint _trapsTriggeredOthers;
    partial void OnTrapsTriggeredOthersChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_TrapPlayers = value; }

    [ObservableProperty] private uint _trapsTriggeredSelf;
    partial void OnTrapsTriggeredSelfChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_TrapSelf = value; }

    [ObservableProperty] private uint _baseMoved;
    partial void OnBaseMovedChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_MyBaseMoved = value; }

    [ObservableProperty] private uint _flagsObtained;
    partial void OnFlagsObtainedChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_FlagsTaken = value; }

    [ObservableProperty] private uint _myFlagTaken;
    partial void OnMyFlagTakenChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_FlagsFromMe = value; }

    [ObservableProperty] private uint _myFlagRecovered;
    partial void OnMyFlagRecoveredChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_FlagsRecovered = value; }

    [ObservableProperty] private uint _flagsCaptured;
    partial void OnFlagsCapturedChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_FlagsCaptured = value; }

    [ObservableProperty] private uint _helpedOthers;
    partial void OnHelpedOthersChanged(uint value) { if (_sinnoh is not null) _sinnoh.UG_HelpedOthers = value; }

    private void LoadData()
    {
        if (_sinnoh is null) return;

        PlayersMet = Math.Min(_sinnoh.UG_PeopleMet, SAV4Sinnoh.UG_MAX);
        GiftsGiven = Math.Min(_sinnoh.UG_GiftsGiven, SAV4Sinnoh.UG_MAX);
        GiftsReceived = Math.Min(_sinnoh.UG_GiftsReceived, SAV4Sinnoh.UG_MAX);
        SpheresBuried = Math.Min(_sinnoh.UG_Spheres, SAV4Sinnoh.UG_MAX);
        FossilsDugUp = Math.Min(_sinnoh.UG_Fossils, SAV4Sinnoh.UG_MAX);
        TrapsTriggeredOthers = Math.Min(_sinnoh.UG_TrapPlayers, SAV4Sinnoh.UG_MAX);
        TrapsTriggeredSelf = Math.Min(_sinnoh.UG_TrapSelf, SAV4Sinnoh.UG_MAX);
        BaseMoved = Math.Min(_sinnoh.UG_MyBaseMoved, SAV4Sinnoh.UG_MAX);
        FlagsObtained = Math.Min(_sinnoh.UG_FlagsTaken, SAV4Sinnoh.UG_MAX);
        MyFlagTaken = Math.Min(_sinnoh.UG_FlagsFromMe, SAV4Sinnoh.UG_MAX);
        MyFlagRecovered = Math.Min(_sinnoh.UG_FlagsRecovered, SAV4Sinnoh.UG_MAX);
        FlagsCaptured = Math.Min(_sinnoh.UG_FlagsCaptured, SAV4Sinnoh.UG_MAX);
        HelpedOthers = Math.Min(_sinnoh.UG_HelpedOthers, SAV4Sinnoh.UG_MAX);
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}
