using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for an underground score entry.
/// </summary>
public partial class UGScoreModel : ObservableObject
{
    public string Label { get; }

    [ObservableProperty]
    private uint _value;

    public uint Maximum { get; }

    public UGScoreModel(string label, uint value, uint max)
    {
        Label = label;
        _value = Math.Clamp(value, 0, max);
        Maximum = max;
    }
}

/// <summary>
/// ViewModel for the Underground editor (Gen 4 Sinnoh).
/// Edits underground scores and inventory.
/// </summary>
public partial class Underground4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4Sinnoh SAV4S;

    public ObservableCollection<UGScoreModel> Scores { get; } = [];

    public Underground4ViewModel(SAV4Sinnoh sav) : base(sav)
    {
        SAV4S = sav;

        var max = SAV4Sinnoh.UG_MAX;
        Scores.Add(new UGScoreModel("Players Met", sav.UG_PeopleMet, max));
        Scores.Add(new UGScoreModel("Gifts Given", sav.UG_GiftsGiven, max));
        Scores.Add(new UGScoreModel("Gifts Received", sav.UG_GiftsReceived, max));
        Scores.Add(new UGScoreModel("Spheres Found", sav.UG_Spheres, max));
        Scores.Add(new UGScoreModel("Fossils Found", sav.UG_Fossils, max));
        Scores.Add(new UGScoreModel("Trap Players", sav.UG_TrapPlayers, max));
        Scores.Add(new UGScoreModel("Trap Self", sav.UG_TrapSelf, max));
        Scores.Add(new UGScoreModel("Base Moved", sav.UG_MyBaseMoved, max));
        Scores.Add(new UGScoreModel("Flags Obtained", sav.UG_FlagsTaken, max));
        Scores.Add(new UGScoreModel("My Flag Taken", sav.UG_FlagsFromMe, max));
        Scores.Add(new UGScoreModel("My Flag Recovered", sav.UG_FlagsRecovered, max));
        Scores.Add(new UGScoreModel("Flags Captured", sav.UG_FlagsCaptured, max));
        Scores.Add(new UGScoreModel("Helped Others", sav.UG_HelpedOthers, max));
    }

    [RelayCommand]
    private void Save()
    {
        SAV4S.UG_PeopleMet = Scores[0].Value;
        SAV4S.UG_GiftsGiven = Scores[1].Value;
        SAV4S.UG_GiftsReceived = Scores[2].Value;
        SAV4S.UG_Spheres = Scores[3].Value;
        SAV4S.UG_Fossils = Scores[4].Value;
        SAV4S.UG_TrapPlayers = Scores[5].Value;
        SAV4S.UG_TrapSelf = Scores[6].Value;
        SAV4S.UG_MyBaseMoved = Scores[7].Value;
        SAV4S.UG_FlagsTaken = Scores[8].Value;
        SAV4S.UG_FlagsFromMe = Scores[9].Value;
        SAV4S.UG_FlagsRecovered = Scores[10].Value;
        SAV4S.UG_FlagsCaptured = Scores[11].Value;
        SAV4S.UG_HelpedOthers = Scores[12].Value;

        SAV.State.Edited = true;
        Modified = true;
    }
}
