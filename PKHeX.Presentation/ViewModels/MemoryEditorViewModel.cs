using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class MemoryEditorViewModel : ViewModelBase
{
    private readonly PKM _pkm;
    private readonly Action? _closeRequested;

    // Geolocation
    [ObservableProperty] private int _country0;
    [ObservableProperty] private int _country1;
    [ObservableProperty] private int _country2;
    [ObservableProperty] private int _country3;
    [ObservableProperty] private int _country4;
    
    [ObservableProperty] private int _region0;
    [ObservableProperty] private int _region1;
    [ObservableProperty] private int _region2;
    [ObservableProperty] private int _region3;
    [ObservableProperty] private int _region4;

    // Affection / Fullness (Gen 6+)
    [ObservableProperty] private byte _fullness;
    [ObservableProperty] private byte _enjoyment;
    [ObservableProperty] private byte _otFriendship;
    [ObservableProperty] private byte _htFriendship;
    [ObservableProperty] private byte _otAffection;
    [ObservableProperty] private byte _htAffection;

    // Memories
    [ObservableProperty] private int _otMemory;
    [ObservableProperty] private int _otMemoryVar;
    [ObservableProperty] private int _otMemoryFeel;
    [ObservableProperty] private int _otMemoryQual;
    
    [ObservableProperty] private int _htMemory;
    [ObservableProperty] private int _htMemoryVar;
    [ObservableProperty] private int _htMemoryFeel;
    [ObservableProperty] private int _htMemoryQual;

    public MemoryEditorViewModel(PKM pkm, Action? closeHelper = null)
    {
        _pkm = pkm;
        _closeRequested = closeHelper;
        Load();
    }

    private void Load()
    {
        if (_pkm is IGeoTrack g)
        {
            Country0 = g.Geo1_Country;
            Country1 = g.Geo2_Country;
            Country2 = g.Geo3_Country;
            Country3 = g.Geo4_Country;
            Country4 = g.Geo5_Country;
            
            Region0 = g.Geo1_Region;
            Region1 = g.Geo2_Region;
            Region2 = g.Geo3_Region;
            Region3 = g.Geo4_Region;
            Region4 = g.Geo5_Region;
        }

        if (_pkm is IFullnessEnjoyment f)
        {
            Fullness = f.Fullness;
            Enjoyment = f.Enjoyment;
        }

        OtFriendship = _pkm.OriginalTrainerFriendship;
        HtFriendship = _pkm.HandlingTrainerFriendship;

        if (_pkm is IAffection a)
        {
            OtAffection = a.OriginalTrainerAffection;
            HtAffection = a.HandlingTrainerAffection;
        }

        if (_pkm is ITrainerMemories m)
        {
            OtMemory = m.OriginalTrainerMemory;
            OtMemoryVar = m.OriginalTrainerMemoryVariable;
            OtMemoryFeel = m.OriginalTrainerMemoryFeeling;
            OtMemoryQual = m.OriginalTrainerMemoryIntensity;
            
            HtMemory = m.HandlingTrainerMemory;
            HtMemoryVar = m.HandlingTrainerMemoryVariable;
            HtMemoryFeel = m.HandlingTrainerMemoryFeeling;
            HtMemoryQual = m.HandlingTrainerMemoryIntensity;
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (_pkm is IGeoTrack g)
        {
            g.Geo1_Country = (byte)Country0;
            g.Geo2_Country = (byte)Country1;
            g.Geo3_Country = (byte)Country2;
            g.Geo4_Country = (byte)Country3;
            g.Geo5_Country = (byte)Country4;
            
            g.Geo1_Region = (byte)Region0;
            g.Geo2_Region = (byte)Region1;
            g.Geo3_Region = (byte)Region2;
            g.Geo4_Region = (byte)Region3;
            g.Geo5_Region = (byte)Region4;
        }

        if (_pkm is IFullnessEnjoyment f)
        {
            f.Fullness = Fullness;
            f.Enjoyment = Enjoyment;
        }

        _pkm.OriginalTrainerFriendship = OtFriendship;
        _pkm.HandlingTrainerFriendship = HtFriendship;

        if (_pkm is IAffection a)
        {
            a.OriginalTrainerAffection = OtAffection;
            a.HandlingTrainerAffection = HtAffection;
        }

        if (_pkm is ITrainerMemories m)
        {
            m.OriginalTrainerMemory = (byte)OtMemory;
            m.OriginalTrainerMemoryVariable = (ushort)OtMemoryVar;
            m.OriginalTrainerMemoryFeeling = (byte)OtMemoryFeel;
            m.OriginalTrainerMemoryIntensity = (byte)OtMemoryQual;
            
            m.HandlingTrainerMemory = (byte)HtMemory;
            m.HandlingTrainerMemoryVariable = (ushort)HtMemoryVar;
            m.HandlingTrainerMemoryFeeling = (byte)HtMemoryFeel;
            m.HandlingTrainerMemoryIntensity = (byte)HtMemoryQual;
        }

        _closeRequested?.Invoke();
    }
    [RelayCommand]
    private void Close() => _closeRequested?.Invoke();
}
