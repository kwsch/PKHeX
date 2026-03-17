using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Memory/Amie editor subform.
/// Edits OT/HT memories, friendship, affection, fullness, and enjoyment.
/// </summary>
public partial class MemoryAmieViewModel : ObservableObject
{
    private readonly PKM _entity;
    private readonly MemoryStrings _memStrings;

    [ObservableProperty]
    private bool _modified;

    // Memory lists for ComboBoxes
    public List<ComboItem> MemoryList { get; }
    public ObservableCollection<ComboItem> OTVarList { get; } = [];
    public ObservableCollection<ComboItem> HTVarList { get; } = [];

    // Intensity (quality) and feeling lists
    public ObservableCollection<string> IntensityList { get; } = [];
    public ObservableCollection<string> OTFeelingList { get; } = [];
    public ObservableCollection<string> HTFeelingList { get; } = [];

    // OT Memory fields
    [ObservableProperty]
    private int _otMemoryIndex;

    [ObservableProperty]
    private int _otIntensityIndex;

    [ObservableProperty]
    private int _otFeelingIndex;

    [ObservableProperty]
    private int _otVarIndex;

    // HT Memory fields
    [ObservableProperty]
    private int _htMemoryIndex;

    [ObservableProperty]
    private int _htIntensityIndex;

    [ObservableProperty]
    private int _htFeelingIndex;

    [ObservableProperty]
    private int _htVarIndex;

    // Friendship
    [ObservableProperty]
    private byte _otFriendship;

    [ObservableProperty]
    private byte _htFriendship;

    // Affection (Gen6/7 only)
    [ObservableProperty]
    private byte _otAffection;

    [ObservableProperty]
    private byte _htAffection;

    [ObservableProperty]
    private bool _hasAffection;

    // Fullness/Enjoyment
    [ObservableProperty]
    private byte _fullness;

    [ObservableProperty]
    private byte _enjoyment;

    [ObservableProperty]
    private bool _hasFullnessEnjoyment;

    // Sociability (Gen8+)
    [ObservableProperty]
    private uint _sociability;

    [ObservableProperty]
    private bool _hasSociability;

    // Visibility flags
    [ObservableProperty]
    private bool _hasMemories;

    [ObservableProperty]
    private bool _isOTEditable;

    [ObservableProperty]
    private string _otLabel = "OT Memories";

    [ObservableProperty]
    private string _htLabel = "HT Memories";

    // Memory display text
    [ObservableProperty]
    private string _otMemoryText = string.Empty;

    [ObservableProperty]
    private string _htMemoryText = string.Empty;

    public MemoryAmieViewModel(PKM pk)
    {
        _entity = pk;
        _memStrings = new MemoryStrings(GameInfo.Strings);
        MemoryList = _memStrings.Memory;

        LoadIntensityList();
        LoadFeelingLists();
        LoadFields();
    }

    private void LoadIntensityList()
    {
        var qualities = _memStrings.GetMemoryQualities();
        foreach (var q in qualities)
            IntensityList.Add(q);
    }

    private void LoadFeelingLists()
    {
        var otFeelings = _memStrings.GetMemoryFeelings(_entity.Generation == 0 ? _entity.Format : _entity.Generation);
        foreach (var f in otFeelings)
            OTFeelingList.Add(f);

        var htFeelings = _memStrings.GetMemoryFeelings(_entity.Format);
        foreach (var f in htFeelings)
            HTFeelingList.Add(f);
    }

    private void LoadFields()
    {
        // Friendship
        OtFriendship = (byte)_entity.OriginalTrainerFriendship;
        HtFriendship = (byte)_entity.HandlingTrainerFriendship;

        // Affection
        if (_entity is IAffection a)
        {
            HasAffection = _entity.Format <= 7;
            OtAffection = a.OriginalTrainerAffection;
            HtAffection = a.HandlingTrainerAffection;
        }

        // Fullness/Enjoyment
        if (_entity is IFullnessEnjoyment f)
        {
            HasFullnessEnjoyment = true;
            Fullness = f.Fullness;
            Enjoyment = f.Enjoyment;
        }

        // Sociability
        if (_entity is ISociability s)
        {
            HasSociability = true;
            Sociability = s.Sociability;
        }

        // Memories
        if (_entity is ITrainerMemories m)
        {
            HasMemories = true;

            // Determine editability
            IsOTEditable = !_entity.IsEgg && _entity.Generation >= 6;

            // Set labels
            OtLabel = $"Memories with {_entity.OriginalTrainerName} (OT)";
            var htName = string.IsNullOrWhiteSpace(_entity.HandlingTrainerName)
                ? "----"
                : _entity.HandlingTrainerName;
            HtLabel = $"Memories with {htName} (HT)";

            // Load OT Memory
            OtMemoryIndex = FindComboIndex(MemoryList, m.OriginalTrainerMemory);
            OtIntensityIndex = m.OriginalTrainerMemoryIntensity;
            OtFeelingIndex = m.OriginalTrainerMemoryFeeling;

            // Load OT Var
            UpdateOTVarList(m.OriginalTrainerMemory);
            OtVarIndex = FindComboIndex(OTVarList, m.OriginalTrainerMemoryVariable);

            // Load HT Memory
            HtMemoryIndex = FindComboIndex(MemoryList, m.HandlingTrainerMemory);
            HtIntensityIndex = m.HandlingTrainerMemoryIntensity;
            HtFeelingIndex = m.HandlingTrainerMemoryFeeling;

            // Load HT Var
            UpdateHTVarList(m.HandlingTrainerMemory);
            HtVarIndex = FindComboIndex(HTVarList, m.HandlingTrainerMemoryVariable);
        }
    }

    partial void OnOtMemoryIndexChanged(int value)
    {
        if (value < 0 || value >= MemoryList.Count)
            return;
        var memory = (byte)MemoryList[value].Value;
        UpdateOTVarList(memory);
    }

    partial void OnHtMemoryIndexChanged(int value)
    {
        if (value < 0 || value >= MemoryList.Count)
            return;
        var memory = (byte)MemoryList[value].Value;
        UpdateHTVarList(memory);
    }

    private void UpdateOTVarList(byte memory)
    {
        int memoryGen = _entity.Generation == 0 ? _entity.Format : _entity.Generation;
        var argType = Memories.GetMemoryArgType(memory, memoryGen);
        var args = _memStrings.GetArgumentStrings(argType, memoryGen);
        OTVarList.Clear();
        foreach (var item in args)
            OTVarList.Add(item);
        if (OtVarIndex >= OTVarList.Count)
            OtVarIndex = OTVarList.Count > 0 ? 0 : -1;
    }

    private void UpdateHTVarList(byte memory)
    {
        int memoryGen = _entity.Format;
        var argType = Memories.GetMemoryArgType(memory, memoryGen);
        var args = _memStrings.GetArgumentStrings(argType, memoryGen);
        HTVarList.Clear();
        foreach (var item in args)
            HTVarList.Add(item);
        if (HtVarIndex >= HTVarList.Count)
            HtVarIndex = HTVarList.Count > 0 ? 0 : -1;
    }

    private static int FindComboIndex(IList<ComboItem> list, int value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Value == value)
                return i;
        }
        return 0;
    }

    /// <summary>
    /// Saves all fields back to the entity.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        _entity.OriginalTrainerFriendship = OtFriendship;
        _entity.HandlingTrainerFriendship = HtFriendship;

        if (_entity is IAffection a)
        {
            a.OriginalTrainerAffection = OtAffection;
            a.HandlingTrainerAffection = HtAffection;
        }

        if (_entity is IFullnessEnjoyment f)
        {
            f.Fullness = Fullness;
            f.Enjoyment = Enjoyment;
        }

        if (_entity is ISociability s)
            s.Sociability = Sociability;

        if (_entity is ITrainerMemories m)
        {
            // Save OT memories
            m.OriginalTrainerMemory = GetSelectedMemoryValue(MemoryList, OtMemoryIndex);
            m.OriginalTrainerMemoryIntensity = (byte)OtIntensityIndex;
            m.OriginalTrainerMemoryFeeling = (byte)OtFeelingIndex;
            m.OriginalTrainerMemoryVariable = GetSelectedVarValue(OTVarList, OtVarIndex);

            // Save HT memories
            m.HandlingTrainerMemory = GetSelectedMemoryValue(MemoryList, HtMemoryIndex);
            m.HandlingTrainerMemoryIntensity = (byte)HtIntensityIndex;
            m.HandlingTrainerMemoryFeeling = (byte)HtFeelingIndex;
            m.HandlingTrainerMemoryVariable = GetSelectedVarValue(HTVarList, HtVarIndex);
        }

        Modified = true;
    }

    private static byte GetSelectedMemoryValue(IList<ComboItem> list, int index)
    {
        if (index < 0 || index >= list.Count)
            return 0;
        return (byte)list[index].Value;
    }

    private static ushort GetSelectedVarValue(IList<ComboItem> list, int index)
    {
        if (index < 0 || index >= list.Count)
            return 0;
        return (ushort)list[index].Value;
    }
}
