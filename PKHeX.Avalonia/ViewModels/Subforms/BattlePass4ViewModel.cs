using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a Battle Pass list entry.
/// </summary>
public partial class BattlePassEntryModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty]
    private string _label;

    public BattlePassEntryModel(int index, string label)
    {
        Index = index;
        _label = label;
    }
}

/// <summary>
/// ViewModel for the Battle Pass editor (Gen 4 Battle Revolution).
/// Edits battle pass trainer data, appearance, and party slots.
/// </summary>
public partial class BattlePass4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4BR SAV4BR;
    private BattlePass _currentPass;
    private int _currentPassIndex;

    public ObservableCollection<BattlePassEntryModel> PassList { get; } = [];

    [ObservableProperty]
    private int _selectedPassIndex;

    // Trainer Info
    [ObservableProperty] private string _passName = string.Empty;
    [ObservableProperty] private string _tid = string.Empty;
    [ObservableProperty] private string _sid = string.Empty;

    // Flags
    [ObservableProperty] private bool _available;
    [ObservableProperty] private bool _issued;
    [ObservableProperty] private bool _rental;
    [ObservableProperty] private bool _friend;

    // Catchphrases
    [ObservableProperty] private string _greeting = string.Empty;
    [ObservableProperty] private string _sentOut = string.Empty;
    [ObservableProperty] private string _shift1 = string.Empty;
    [ObservableProperty] private string _shift2 = string.Empty;
    [ObservableProperty] private string _win = string.Empty;
    [ObservableProperty] private string _lose = string.Empty;

    // Creator Info
    [ObservableProperty] private string _creatorName = string.Empty;
    [ObservableProperty] private string _birthMonth = string.Empty;
    [ObservableProperty] private string _birthDay = string.Empty;
    [ObservableProperty] private string _regionCode = string.Empty;
    [ObservableProperty] private string _playerIdHex = string.Empty;

    // Records
    [ObservableProperty] private int _battles;
    [ObservableProperty] private int _recordColosseumBattles;
    [ObservableProperty] private int _recordFreeBattles;
    [ObservableProperty] private int _recordWiFiBattles;

    public BattlePass4ViewModel(SAV4BR sav, int startIndex = 0) : base(sav)
    {
        SAV4BR = sav;

        LoadPassList();

        _currentPassIndex = startIndex;
        _currentPass = sav.BattlePasses[startIndex];
        _selectedPassIndex = startIndex;
        LoadCurrent(_currentPass);
    }

    private void LoadPassList()
    {
        PassList.Clear();
        for (int i = 0; i < BattlePassAccessor.PASS_COUNT; i++)
        {
            var pass = SAV4BR.BattlePasses[i];
            var name = pass.Name;
            if (pass is { Rental: false, Issued: false } || string.IsNullOrWhiteSpace(name))
                name = "(None)";
            var type = SAV4BR.BattlePasses.GetPassType(i);
            PassList.Add(new BattlePassEntryModel(i, $"{i + 1:00} {type}/{name}"));
        }
    }

    partial void OnSelectedPassIndexChanged(int value)
    {
        if (value < 0 || value >= BattlePassAccessor.PASS_COUNT)
            return;
        SaveCurrent();
        _currentPassIndex = value;
        _currentPass = SAV4BR.BattlePasses[value];
        LoadCurrent(_currentPass);
    }

    private void LoadCurrent(BattlePass pdata)
    {
        PassName = pdata.Name;
        Tid = pdata.TID.ToString("00000");
        Sid = pdata.SID.ToString("00000");

        Available = pdata.Available;
        Issued = pdata.Issued;
        Rental = pdata.Rental;
        Friend = pdata.Friend;

        Greeting = pdata.Greeting;
        SentOut = pdata.SentOut.Replace(StringConverter4GC.LineBreak, '\n');
        Shift1 = pdata.Shift1;
        Shift2 = pdata.Shift2;
        Win = pdata.Win.Replace(StringConverter4GC.LineBreak, '\n');
        Lose = pdata.Lose.Replace(StringConverter4GC.LineBreak, '\n');

        CreatorName = pdata.CreatorName;
        BirthMonth = pdata.BirthMonth;
        BirthDay = pdata.BirthDay;
        RegionCode = pdata.RegionCode;
        PlayerIdHex = pdata.PlayerID.ToString("X16");

        Battles = pdata.Battles;
        RecordColosseumBattles = pdata.RecordColosseumBattles;
        RecordFreeBattles = pdata.RecordFreeBattles;
        RecordWiFiBattles = pdata.RecordWiFiBattles;
    }

    private void SaveCurrent()
    {
        var pdata = _currentPass;
        pdata.Name = PassName;
        pdata.TID = (ushort)(uint.TryParse(Tid, out var tid) ? tid : 0);
        pdata.SID = (ushort)(uint.TryParse(Sid, out var sid) ? sid : 0);

        pdata.Available = Available;
        pdata.Issued = Issued;
        pdata.Rental = Rental;
        pdata.Friend = Friend;

        pdata.Greeting = Greeting;
        pdata.SentOut = SentOut.Replace("\n", StringConverter4GC.LineBreak.ToString());
        pdata.Shift1 = Shift1;
        pdata.Shift2 = Shift2;
        pdata.Win = Win.Replace("\n", StringConverter4GC.LineBreak.ToString());
        pdata.Lose = Lose.Replace("\n", StringConverter4GC.LineBreak.ToString());

        pdata.CreatorName = CreatorName;
        pdata.BirthMonth = BirthMonth;
        pdata.BirthDay = BirthDay;
        pdata.RegionCode = RegionCode;
        pdata.PlayerID = Convert.ToUInt64(PlayerIdHex, 16);

        pdata.Battles = Battles;
        pdata.RecordColosseumBattles = RecordColosseumBattles;
        pdata.RecordFreeBattles = RecordFreeBattles;
        pdata.RecordWiFiBattles = RecordWiFiBattles;

        // Refresh list label
        var name = pdata.Name;
        if (pdata is { Rental: false, Issued: false } || string.IsNullOrWhiteSpace(name))
            name = "(None)";
        var type = SAV4BR.BattlePasses.GetPassType(_currentPassIndex);
        if (_currentPassIndex < PassList.Count)
            PassList[_currentPassIndex].Label = $"{_currentPassIndex + 1:00} {type}/{name}";
    }

    [RelayCommand]
    private void UnlockCustom()
    {
        SaveCurrent();
        SAV4BR.BattlePasses.UnlockAllCustomPasses();
        LoadCurrent(_currentPass);
    }

    [RelayCommand]
    private void UnlockRental()
    {
        SaveCurrent();
        SAV4BR.BattlePasses.UnlockAllRentalPasses();
        LoadCurrent(_currentPass);
    }

    [RelayCommand]
    private void DeletePass()
    {
        SaveCurrent();
        SAV4BR.BattlePasses.Delete(_currentPassIndex);
        LoadPassList();
        if (_currentPassIndex < BattlePassAccessor.PASS_COUNT)
        {
            _currentPass = SAV4BR.BattlePasses[_currentPassIndex];
            LoadCurrent(_currentPass);
        }
    }

    [RelayCommand]
    private void MoveUp()
    {
        if (_currentPassIndex <= 0)
            return;
        SaveCurrent();
        SAV4BR.BattlePasses.Swap(_currentPassIndex, _currentPassIndex - 1);
        _currentPassIndex--;
        _currentPass = SAV4BR.BattlePasses[_currentPassIndex];
        LoadPassList();
        SelectedPassIndex = _currentPassIndex;
        LoadCurrent(_currentPass);
    }

    [RelayCommand]
    private void MoveDown()
    {
        if (_currentPassIndex >= BattlePassAccessor.PASS_COUNT - 1)
            return;
        SaveCurrent();
        SAV4BR.BattlePasses.Swap(_currentPassIndex, _currentPassIndex + 1);
        _currentPassIndex++;
        _currentPass = SAV4BR.BattlePasses[_currentPassIndex];
        LoadPassList();
        SelectedPassIndex = _currentPassIndex;
        LoadCurrent(_currentPass);
    }

    [RelayCommand]
    private void Save()
    {
        SaveCurrent();
        SAV.State.Edited = true;
        Modified = true;
    }
}
