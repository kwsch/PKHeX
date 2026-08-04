using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class SuperTrainingEditorViewModel : ViewModelBase
{
    private readonly SAV6 _sav;
    private readonly SuperTrainBlock _block;

    public SuperTrainingEditorViewModel(SaveFile sav)
    {
        _sav = (SAV6)sav;
        _block = ((ISaveBlock6Main)_sav).SuperTrain;
        IsSupported = true;

        LoadBags();
        LoadRecords();
        LoadStages();
    }

    public bool IsSupported { get; }

    // Bags
    public ObservableCollection<TrainingBagViewModel> Bags { get; } = [];
    public string[] BagNames { get; } = GameInfo.Strings.trainingbags;

    // Stages
    public ObservableCollection<SuperTrainStageViewModel> Stages { get; } = [];

    // Records
    public ObservableCollection<SuperTrainRecordViewModel> Records { get; } = [];
    
    [ObservableProperty]
    private SuperTrainRecordViewModel? _selectedRecord;

    public void LoadBags()
    {
        Bags.Clear();
        for (int i = 0; i < 12; i++)
        {
            Bags.Add(new TrainingBagViewModel(i, _block.GetBag(i), _block));
        }
    }

    public void LoadStages()
    {
        Stages.Clear();
        string[] stages = GameInfo.Strings.trainingstage;
        for (int i = 0; i < 48; i++)
        {
            if (i >= stages.Length) break;
            Stages.Add(new SuperTrainStageViewModel(i, stages[i], _block.GetIsRegimenUnlocked(i), _block));
        }
    }

    public void LoadRecords()
    {
        Records.Clear();
        string[] stages = GameInfo.Strings.trainingstage;
        for (int i = 0; i < 48; i++)
        {
            if (i >= stages.Length) break;
            Records.Add(new SuperTrainRecordViewModel(i, stages[i], _block));
        }
        if (Records.Count > 0)
            SelectedRecord = Records[0];
    }

    [RelayCommand]
    private void UnlockAll()
    {
        _block.UnlockAllStages(true);
        LoadStages();
    }

    [RelayCommand]
    private void Save()
    {
        // Many properties are updated via partial methods in sub-ViewModels
        _sav.State.Edited = true;
    }
}

public partial class TrainingBagViewModel : ViewModelBase
{
    private readonly int _index;
    private readonly SuperTrainBlock _block;

    public TrainingBagViewModel(int index, byte bag, SuperTrainBlock block)
    {
        _index = index;
        _bagId = bag;
        _block = block;
    }

    public string SlotName => (_index + 1).ToString();

    [ObservableProperty]
    private byte _bagId;

    partial void OnBagIdChanged(byte value) => _block.SetBag(_index, value);
}

public partial class SuperTrainStageViewModel : ViewModelBase
{
    private readonly int _index;
    private readonly SuperTrainBlock _block;

    public SuperTrainStageViewModel(int index, string name, bool isUnlocked, SuperTrainBlock block)
    {
        _index = index;
        Name = name;
        _isUnlocked = isUnlocked;
        _block = block;
    }

    public string Name { get; }

    [ObservableProperty]
    private bool _isUnlocked;

    partial void OnIsUnlockedChanged(bool value) => _block.SetIsRegimenUnlocked(_index, value);
}

public partial class SuperTrainRecordViewModel : ViewModelBase
{
    private readonly int _index;
    private readonly SuperTrainBlock _block;

    public SuperTrainRecordViewModel(int index, string name, SuperTrainBlock block)
    {
        _index = index;
        Name = name;
        _block = block;

        _time1 = block.GetTime1(index);
        _time2 = block.GetTime2(index);
        
        var h1 = block.GetHolder1(index);
        _species1 = h1.Species;
        _form1 = h1.Form;
        _gender1 = h1.Gender;

        var h2 = block.GetHolder2(index);
        _species2 = h2.Species;
        _form2 = h2.Form;
        _gender2 = h2.Gender;
    }

    public string Name { get; }

    // Record 1
    [ObservableProperty] private float _time1;
    [ObservableProperty] private ushort _species1;
    [ObservableProperty] private byte _form1;
    [ObservableProperty] private byte _gender1;

    // Record 2
    [ObservableProperty] private float _time2;
    [ObservableProperty] private ushort _species2;
    [ObservableProperty] private byte _form2;
    [ObservableProperty] private byte _gender2;

    partial void OnTime1Changed(float value) => _block.SetTime1(_index, value);
    partial void OnTime2Changed(float value) => _block.SetTime2(_index, value);

    partial void OnSpecies1Changed(ushort value) { var h = _block.GetHolder1(_index); h.Species = value; }
    partial void OnForm1Changed(byte value) { var h = _block.GetHolder1(_index); h.Form = value; }
    partial void OnGender1Changed(byte value) { var h = _block.GetHolder1(_index); h.Gender = value; }

    partial void OnSpecies2Changed(ushort value) { var h = _block.GetHolder2(_index); h.Species = value; }
    partial void OnForm2Changed(byte value) { var h = _block.GetHolder2(_index); h.Form = value; }
    partial void OnGender2Changed(byte value) { var h = _block.GetHolder2(_index); h.Gender = value; }
}
