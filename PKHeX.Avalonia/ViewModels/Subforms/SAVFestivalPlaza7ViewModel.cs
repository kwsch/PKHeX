using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a facility slot in the Festival Plaza.
/// </summary>
public partial class FacilitySlotModel : ObservableObject
{
    public int Index { get; }
    public string DisplayName => $"Facility {Index + 1}";

    [ObservableProperty] private int _type;
    [ObservableProperty] private int _color;
    [ObservableProperty] private int _npc;
    [ObservableProperty] private bool _isIntroduced;
    [ObservableProperty] private string _otName = string.Empty;

    public FacilitySlotModel(int index)
    {
        Index = index;
    }
}

/// <summary>
/// Model for a phrase entry.
/// </summary>
public partial class PhraseModel : ObservableObject
{
    public string Text { get; }

    [ObservableProperty]
    private bool _isUnlocked;

    public PhraseModel(string text, bool isUnlocked)
    {
        Text = text;
        _isUnlocked = isUnlocked;
    }
}

/// <summary>
/// ViewModel for the Gen 7 Festival Plaza editor.
/// Edits Festival Coins, rank, facilities, phrases, and rewards.
/// </summary>
public partial class SAVFestivalPlaza7ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV7 _sav;
    private readonly FestaFacility[] _facilities;

    // FC / Rank
    [ObservableProperty] private int _fcCurrent;
    [ObservableProperty] private int _fcUsed;
    [ObservableProperty] private string _fcCollected = "0";
    [ObservableProperty] private int _rank;
    [ObservableProperty] private string _rankFcRange = string.Empty;

    // Plaza Name
    [ObservableProperty] private string _plazaName = string.Empty;

    // Facilities
    public ObservableCollection<FacilitySlotModel> FacilitySlots { get; } = [];

    [ObservableProperty] private int _selectedFacilityIndex = -1;
    [ObservableProperty] private int _facilityType;
    [ObservableProperty] private int _facilityColor;
    [ObservableProperty] private int _facilityNpc;
    [ObservableProperty] private bool _facilityIntroduced;
    [ObservableProperty] private string _facilityOtName = string.Empty;

    // Phrases
    public ObservableCollection<PhraseModel> Phrases { get; } = [];

    // Messages
    [ObservableProperty] private int _messageMeet;
    [ObservableProperty] private int _messagePart;
    [ObservableProperty] private int _messageMoved;
    [ObservableProperty] private int _messageDisappointed;

    // Festa Date
    [ObservableProperty] private DateTimeOffset _festaDate = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public bool IsUsum { get; }

    public SAVFestivalPlaza7ViewModel(SAV7 sav) : base(sav)
    {
        _sav = (SAV7)(_origin = sav).Clone();
        IsUsum = _sav is SAV7USUM;
        _facilities = new FestaFacility[JoinFesta7.FestaFacilityCount];

        LoadData();
    }

    private void LoadData()
    {
        var festa = _sav.Festa;
        FcCurrent = festa.FestaCoins;
        FcUsed = _sav.GetRecord(038);
        FcCollected = (FcCurrent + FcUsed).ToString();
        Rank = festa.FestaRank;
        RankFcRange = GetRankText(Rank);

        PlazaName = festa.FestivalPlazaName;

        // Facilities
        for (int i = 0; i < (int)JoinFesta7.FestaFacilityCount; i++)
        {
            _facilities[i] = festa.GetFestaFacility(i);
            var slot = new FacilitySlotModel(i)
            {
                Type = _facilities[i].Type,
                Color = _facilities[i].Color,
                Npc = _facilities[i].NPC,
                IsIntroduced = _facilities[i].IsIntroduced,
                OtName = _facilities[i].OriginalTrainerName,
            };
            FacilitySlots.Add(slot);
        }

        // Messages
        MessageMeet = festa.GetFestaMessage(0);
        MessagePart = festa.GetFestaMessage(1);
        MessageMoved = festa.GetFestaMessage(2);
        MessageDisappointed = festa.GetFestaMessage(3);

        // Phrases
        Phrases.Add(new PhraseModel("Festival Plaza Rank 100!", festa.GetFestaPhraseUnlocked(106)));
        // Tent phrases (0..105)
        string[] tentNames =
        [
            "Pokemon House", "Kanto Tent", "Johto Tent", "Hoenn Tent",
            "Sinnoh Tent", "Unova Tent", "Kalos Tent",
        ];
        for (int tent = 0; tent < 7; tent++)
        {
            for (int phrase = 0; phrase < (tent == 6 ? 15 : 16); phrase++)
            {
                int idx = tent * 16 + phrase;
                if (idx >= 106) break;
                Phrases.Add(new PhraseModel($"{tentNames[tent]} #{phrase + 1}", festa.GetFestaPhraseUnlocked(idx)));
            }
        }

        // Festa Date
        DateTime dt = festa.FestaDate ?? new DateTime(2000, 1, 1);
        FestaDate = new DateTimeOffset(dt, TimeSpan.Zero);
    }

    partial void OnFcCurrentChanged(int value)
    {
        FcCollected = (value + FcUsed).ToString();
    }

    partial void OnFcUsedChanged(int value)
    {
        FcCollected = (FcCurrent + value).ToString();
    }

    partial void OnRankChanged(int value)
    {
        RankFcRange = GetRankText(value);
    }

    partial void OnSelectedFacilityIndexChanged(int value)
    {
        if (value < 0 || value >= FacilitySlots.Count)
            return;
        var slot = FacilitySlots[value];
        FacilityType = slot.Type;
        FacilityColor = slot.Color;
        FacilityNpc = slot.Npc;
        FacilityIntroduced = slot.IsIntroduced;
        FacilityOtName = slot.OtName;
    }

    [RelayCommand]
    private void UpdateFacility()
    {
        if (SelectedFacilityIndex < 0 || SelectedFacilityIndex >= FacilitySlots.Count)
            return;
        var slot = FacilitySlots[SelectedFacilityIndex];
        slot.Type = FacilityType;
        slot.Color = FacilityColor;
        slot.Npc = FacilityNpc;
        slot.IsIntroduced = FacilityIntroduced;
        slot.OtName = FacilityOtName;
    }

    [RelayCommand]
    private void UnlockAllPhrases()
    {
        foreach (var p in Phrases)
            p.IsUnlocked = true;
    }

    [RelayCommand]
    private void Save()
    {
        var festa = _sav.Festa;

        // FC
        _sav.SetRecord(038, FcUsed);
        festa.FestaCoins = FcCurrent;
        festa.FestaRank = (ushort)Math.Clamp(Rank, 0, ushort.MaxValue);
        festa.FestivalPlazaName = PlazaName;

        // Festa Date
        festa.FestaDate = FestaDate.DateTime;

        // Phrases
        if (Phrases.Count > 0)
        {
            festa.SetFestaPhraseUnlocked(106, Phrases[0].IsUnlocked);
            for (int i = 1; i < Phrases.Count; i++)
                festa.SetFestaPhraseUnlocked(i - 1, Phrases[i].IsUnlocked);
        }

        // Messages
        festa.SetFestaMessage(0, (ushort)Math.Clamp(MessageMeet, 0, ushort.MaxValue));
        festa.SetFestaMessage(1, (ushort)Math.Clamp(MessagePart, 0, ushort.MaxValue));
        festa.SetFestaMessage(2, (ushort)Math.Clamp(MessageMoved, 0, ushort.MaxValue));
        festa.SetFestaMessage(3, (ushort)Math.Clamp(MessageDisappointed, 0, ushort.MaxValue));

        // Facilities
        for (int i = 0; i < FacilitySlots.Count && i < _facilities.Length; i++)
        {
            var slot = FacilitySlots[i];
            _facilities[i].Type = slot.Type;
            _facilities[i].Color = slot.Color;
            _facilities[i].NPC = slot.Npc;
            _facilities[i].IsIntroduced = slot.IsIntroduced;
            _facilities[i].OriginalTrainerName = slot.OtName;
        }

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }

    private static string GetRankText(int rank)
    {
        if (rank < 1) return string.Empty;
        if (rank == 1) return "0 - 5";
        if (rank == 2) return "6 - 15";
        if (rank == 3) return "16 - 30";
        if (rank <= 10)
        {
            int i = ((rank - 1) * (rank - 2) * 5) + 1;
            return $"{i} - {i + ((rank - 1) * 10) - 1}";
        }
        if (rank <= 20)
        {
            int i = (rank * 100) - 649;
            return $"{i} - {i + 99}";
        }
        if (rank <= 70)
        {
            int j = (rank - 1) / 10;
            int i = (rank * ((j * 30) + 60)) - ((j * j * 150) + (j * 180) + 109);
            return $"{i} - {i + (j * 30) + 59}";
        }
        if (rank <= 100)
        {
            int i = (rank * 270) - 8719;
            return $"{i} - {i + 269}";
        }
        if (rank <= 998)
        {
            int i = (rank * 300) - 11749;
            return $"{i} - {i + 299}";
        }
        if (rank == 999)
            return "287951 - ";
        return string.Empty;
    }
}
