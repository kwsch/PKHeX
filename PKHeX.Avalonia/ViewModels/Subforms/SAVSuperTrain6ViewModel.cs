using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a training bag slot.
/// </summary>
public partial class TrainingBagSlotModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty]
    private int _selectedBagIndex;

    public string DisplayName => $"Slot {Index + 1}";

    public TrainingBagSlotModel(int index, int bagIndex)
    {
        Index = index;
        _selectedBagIndex = bagIndex;
    }
}

/// <summary>
/// ViewModel for the Gen 6 Super Training editor.
/// Edits training bags and regimen records.
/// </summary>
public partial class SAVSuperTrain6ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV6 _sav;
    private readonly SuperTrainBlock _stb;
    private bool _loading;

    public List<string> StageNames { get; } = [];
    public List<string> BagNames { get; }
    public List<ComboItem> SpeciesList { get; }
    public ObservableCollection<TrainingBagSlotModel> BagSlots { get; } = [];

    [ObservableProperty]
    private int _selectedStageIndex = -1;

    // Record holder 1
    [ObservableProperty]
    private int _species1;

    [ObservableProperty]
    private string _gender1 = "0";

    [ObservableProperty]
    private string _form1 = "0";

    [ObservableProperty]
    private string _time1 = "0";

    // Record holder 2
    [ObservableProperty]
    private int _species2;

    [ObservableProperty]
    private string _gender2 = "0";

    [ObservableProperty]
    private string _form2 = "0";

    [ObservableProperty]
    private string _time2 = "0";

    public SAVSuperTrain6ViewModel(SAV6 sav) : base(sav)
    {
        _sav = (SAV6)(_origin = sav).Clone();
        _stb = ((ISaveBlock6Main)_sav).SuperTrain;

        var bagNames = GameInfo.Strings.trainingbags.ToList();
        if (bagNames.Count > 0)
            bagNames[0] = "---";
        BagNames = bagNames;

        SpeciesList = GameInfo.FilteredSources.Species.ToList();

        var stages = GameInfo.Strings.trainingstage;
        for (int i = 0; i < 32; i++)
            StageNames.Add($"{i + 1:00} - {stages[i]}");

        // Load bags
        for (int i = 0; i < 12; i++)
        {
            int bagId = _stb.GetBag(i);
            BagSlots.Add(new TrainingBagSlotModel(i, bagId));
        }

        SelectedStageIndex = 0;
    }

    partial void OnSelectedStageIndexChanged(int value)
    {
        if (value < 0)
            return;
        LoadStageRecord(value);
    }

    private void LoadStageRecord(int index)
    {
        _loading = true;
        var h1 = _stb.GetHolder1(index);
        var h2 = _stb.GetHolder2(index);
        Species1 = h1.Species;
        Gender1 = h1.Gender.ToString();
        Form1 = h1.Form.ToString();
        Species2 = h2.Species;
        Gender2 = h2.Gender.ToString();
        Form2 = h2.Form.ToString();
        Time1 = _stb.GetTime1(index).ToString(CultureInfo.InvariantCulture);
        Time2 = _stb.GetTime2(index).ToString(CultureInfo.InvariantCulture);
        _loading = false;
    }

    private void SaveStageRecord(int index)
    {
        if (index < 0)
            return;
        var h1 = _stb.GetHolder1(index);
        h1.Species = (ushort)Species1;
        if (byte.TryParse(Gender1, out var g1)) h1.Gender = g1;
        if (byte.TryParse(Form1, out var f1)) h1.Form = f1;
        var h2 = _stb.GetHolder2(index);
        h2.Species = (ushort)Species2;
        if (byte.TryParse(Gender2, out var g2)) h2.Gender = g2;
        if (byte.TryParse(Form2, out var f2)) h2.Form = f2;
        if (float.TryParse(Time1, out var t1)) _stb.SetTime1(index, t1);
        if (float.TryParse(Time2, out var t2)) _stb.SetTime2(index, t2);
    }

    [RelayCommand]
    private void Save()
    {
        SaveStageRecord(SelectedStageIndex);

        // Save bags
        int emptySlots = 0;
        for (int i = 0; i < 12; i++)
        {
            int bagIndex = BagSlots[i].SelectedBagIndex;
            if (bagIndex <= 0)
            {
                emptySlots++;
                continue;
            }
            _stb.SetBag(i - emptySlots, (byte)bagIndex);
        }

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}
