using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class MysteryGiftEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IMysteryGiftStorage? _storage;
    private readonly IDialogService _dialogService;

    public MysteryGiftEditorViewModel(SaveFile sav, IDialogService dialogService, IGiftRecordProvider? giftRecordProvider = null)
    {
        _sav = sav;
        _dialogService = dialogService;
        _storage = GetStorage(sav);

        if (_storage is not null)
        {
            GiftCount = _storage.GiftCountMax;
            IsSupported = true;
            _flags = _storage as IMysteryGiftFlags;
            CanManageFlags = _flags != null;

            LoadGifts();
            LoadReceivedFlags();
        }
        else
        {
            GiftCount = 0;
            IsSupported = false;
            _recordStore = giftRecordProvider?.GetStore(sav);
            if (_recordStore is not null)
                InitializeRecords();
        }
    }

    /// <summary>True when the save has either a wondercard album or a gift record log.</summary>
    public bool HasAnySupport => IsSupported || IsRecordsSupported;

    private static IMysteryGiftStorage? GetStorage(SaveFile sav)
    {
        if (sav is IMysteryGiftStorageProvider provider)
            return provider.MysteryGiftStorage;
        return null;
    }

    public int GiftCount { get; }
    public bool IsSupported { get; }
    public string GiftCountText => LocalizedStrings.Instance.Format("MysteryGiftEditor_SlotCount", GiftCount);

    [ObservableProperty]
    private ObservableCollection<MysteryGiftSlotViewModel> _gifts = [];

    [ObservableProperty]
    private MysteryGiftSlotViewModel? _selectedGift;

    private void LoadGifts()
    {
        if (_storage is null) return;

        int selected = SelectedGift?.Index ?? 0;
        Gifts.Clear();
        for (int i = 0; i < GiftCount; i++)
        {
            var gift = _storage.GetMysteryGift(i);
            Gifts.Add(new MysteryGiftSlotViewModel(i, gift));
        }

        SelectedGift = Gifts.Count == 0 ? null : Gifts[Math.Min(selected, Gifts.Count - 1)];
    }

    [RelayCommand]
    private void SelectGift(MysteryGiftSlotViewModel? gift)
    {
        if (gift is not null)
            SelectedGift = gift;
    }

    [RelayCommand]
    private void Save()
    {
        if (_storage is null) return;

        foreach (var slot in Gifts)
        {
            if (slot.Gift is not null)
                _storage.SetMysteryGift(slot.Index, slot.Gift);
        }

        if (_flags != null)
        {
            _flags.ClearReceivedFlags();
            foreach (var s in ReceivedFlags)
            {
               if (int.TryParse(s, out int id))
                   _flags.SetMysteryGiftReceivedFlag(id, true);
            }
        }
    }

    [RelayCommand]
    private void Reset()
    {
        LoadGifts();
    }

    [RelayCommand]
    private async Task ImportGiftAsync()
    {
        if (_storage is null || SelectedGift is null) return;

        var path = await _dialogService.OpenFileAsync(
            LocalizedStrings.Instance["MysteryGiftEditor_ImportGiftTitle"],
            ["*.wc9", "*.wa9", "*.wc8", "*.wa8", "*.wb8", "*.wc7", "*.wc6", "*.pgf", "*.pgt", "*.pcd", "*"]);

        if (string.IsNullOrEmpty(path)) return;

        try
        {
            var data = await System.IO.File.ReadAllBytesAsync(path);
            var ext = System.IO.Path.GetExtension(path);
            var gift = MysteryGift.GetMysteryGift(data, ext);

            if (gift is null)
            {
                await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["MysteryGiftEditor_ImportErrorTitle"], LocalizedStrings.Instance["MysteryGiftEditor_CouldNotParseGift"]);
                return;
            }

            // Check compatibility
            if (gift.Generation != _sav.Generation)
            {
                await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["MysteryGiftEditor_ImportErrorTitle"],
                    LocalizedStrings.Instance.Format("MysteryGiftEditor_GenerationMismatch", gift.Generation, _sav.Generation));
                return;
            }

            SelectedGift.Gift = gift;
            SelectedGift.UpdateFromGift();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["MysteryGiftEditor_ImportErrorTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private async Task ExportGiftAsync()
    {
        if (SelectedGift?.Gift is null || SelectedGift.IsEmpty) return;

        var gift = SelectedGift.Gift;
        var defaultName = gift.FileName;

        var path = await _dialogService.SaveFileAsync(LocalizedStrings.Instance["MysteryGiftEditor_ExportGiftTitle"], defaultName);
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            var data = gift.Write().ToArray();
            await System.IO.File.WriteAllBytesAsync(path, data);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["MysteryGiftEditor_ExportErrorTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private void DeleteGift()
    {
        if (SelectedGift?.Gift is null) return;

        SelectedGift.Gift.Clear();
        SelectedGift.UpdateFromGift();
    }

    private readonly IMysteryGiftFlags? _flags;

    [ObservableProperty]
    private ObservableCollection<string> _receivedFlags = [];

    [ObservableProperty]
    private string? _selectedReceivedFlag;

    [ObservableProperty]
    private bool _canManageFlags;

    private void LoadReceivedFlags()
    {
        ReceivedFlags.Clear();
        if (_flags is not { } f) return;

        int count = f.MysteryGiftReceivedFlagMax;
        for (int i = 1; i < count; i++)
        {
            if (f.GetMysteryGiftReceivedFlag(i))
                ReceivedFlags.Add(i.ToString("0000"));
        }
    }

    [RelayCommand]
    private void DeleteFlag()
    {
        if (SelectedReceivedFlag == null) return;
        ReceivedFlags.Remove(SelectedReceivedFlag);
    }

    // --- Gift record log (Switch titles: SWSH/BDSP/PLA/SV) ---
    // These saves keep a received-gift history instead of a wondercard album. Operations write
    // straight into the in-memory save; nothing reaches disk until the user saves the file.

    private readonly IGiftRecordStore? _recordStore;

    public bool IsRecordsSupported => _recordStore is not null;
    public int RecordCount => _recordStore?.Count ?? 0;
    public string RecordCountText => LocalizedStrings.Instance.Format("MysteryGiftEditor_SlotCount", RecordCount);
    public bool CanImportRecord => _recordStore?.SupportsImport == true
                                   && SelectedRecord is not null
                                   && _recordStore.CanImport(SelectedRecord.Index);
    public bool CanExportRecord => _recordStore?.SupportsExport == true;
    public bool CanManageRecordFlags => _recordStore?.SupportsReceivedFlags == true;
    public bool HasSerialLock => _recordStore?.SupportsSerialLock == true;

    [ObservableProperty]
    private ObservableCollection<GiftRecordEntryViewModel> _records = [];

    [ObservableProperty]
    private GiftRecordEntryViewModel? _selectedRecord;

    partial void OnSelectedRecordChanged(GiftRecordEntryViewModel? value) =>
        OnPropertyChanged(nameof(CanImportRecord));

    [ObservableProperty]
    private ObservableCollection<string> _recordReceivedFlags = [];

    [ObservableProperty]
    private string? _selectedRecordFlag;

    [ObservableProperty]
    private string _serialLockText = string.Empty;

    private void InitializeRecords()
    {
        LoadRecords();
        LoadRecordFlags();
        UpdateSerialLockText();
    }

    /// <summary>Rebuilds localized display text after the application language changes.</summary>
    public void RefreshLocalization()
    {
        if (_storage is not null)
        {
            foreach (var gift in Gifts)
                gift.UpdateFromGift();
        }
        if (_recordStore is not null)
        {
            LoadRecords();
            UpdateSerialLockText();
        }
        OnPropertyChanged(nameof(GiftCountText));
        OnPropertyChanged(nameof(RecordCountText));
    }

    private void LoadRecords()
    {
        if (_recordStore is null) return;

        int selected = SelectedRecord?.Index ?? 0;
        Records.Clear();
        foreach (var entry in _recordStore.ReadAll())
            Records.Add(new GiftRecordEntryViewModel(entry));
        SelectedRecord = Records.Count > selected ? Records[selected] : Records.FirstOrDefault();
    }

    private void LoadRecordFlags()
    {
        if (_recordStore is null || !_recordStore.SupportsReceivedFlags) return;

        RecordReceivedFlags.Clear();
        foreach (var flag in _recordStore.GetReceivedFlagIndexes())
            RecordReceivedFlags.Add(flag.ToString("0000"));
    }

    private void UpdateSerialLockText()
    {
        if (_recordStore is null || !_recordStore.SupportsSerialLock) return;

        var timestamp = _recordStore.SerialLockTimestamp;
        SerialLockText = timestamp is null
            ? LocalizedStrings.Instance["MysteryGiftEditor_SerialLockInactive"]
            : LocalizedStrings.Instance.Format("MysteryGiftEditor_SerialLockActive", timestamp.Value.ToString("g"));
    }

    [RelayCommand]
    private void SelectRecord(GiftRecordEntryViewModel? record)
    {
        if (record is not null)
            SelectedRecord = record;
    }

    [RelayCommand]
    private void DeleteRecord()
    {
        if (_recordStore is null || SelectedRecord is null || SelectedRecord.IsEmpty) return;

        _recordStore.ClearEntry(SelectedRecord.Index);
        LoadRecords();
    }

    [RelayCommand]
    private async Task ImportRecordAsync()
    {
        if (_recordStore is null || SelectedRecord is null || !CanImportRecord) return;

        var path = await _dialogService.OpenFileAsync(
            LocalizedStrings.Instance["MysteryGiftEditor_ImportGiftTitle"],
            [.. _recordStore.ImportExtensions, "*"]);
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            var data = await System.IO.File.ReadAllBytesAsync(path);
            var ext = System.IO.Path.GetExtension(path);
            if (!_recordStore.TryImport(SelectedRecord.Index, data, ext, DateTime.Now, out var error))
            {
                var message = error switch
                {
                    GiftRecordImportError.WrongGame => LocalizedStrings.Instance["MysteryGiftEditor_RecordWrongGame"],
                    GiftRecordImportError.UnsupportedGiftType => LocalizedStrings.Instance["MysteryGiftEditor_RecordUnsupportedType"],
                    GiftRecordImportError.InvalidTimestamp => LocalizedStrings.Instance["MysteryGiftEditor_RecordInvalidTimestamp"],
                    _ => LocalizedStrings.Instance["MysteryGiftEditor_CouldNotParseGift"],
                };
                await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["MysteryGiftEditor_ImportErrorTitle"], message);
                return;
            }
            LoadRecords();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["MysteryGiftEditor_ImportErrorTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private async Task ExportRecordAsync()
    {
        if (_recordStore is null || SelectedRecord is null || SelectedRecord.IsEmpty || !_recordStore.SupportsExport) return;

        var card = _recordStore.ExportCard(SelectedRecord.Index);
        if (card is null) return;

        var path = await _dialogService.SaveFileAsync(LocalizedStrings.Instance["MysteryGiftEditor_ExportGiftTitle"], card.FileName);
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            await System.IO.File.WriteAllBytesAsync(path, card.Write().ToArray());
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["MysteryGiftEditor_ExportErrorTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private void DeleteRecordFlag()
    {
        if (_recordStore is null || SelectedRecordFlag is null) return;

        if (int.TryParse(SelectedRecordFlag, out int flag))
            _recordStore.SetReceivedFlag(flag, false);
        LoadRecordFlags();
    }

    [RelayCommand]
    private void ResetSerialLock()
    {
        if (_recordStore is null || !_recordStore.SupportsSerialLock) return;

        _recordStore.ResetSerialLock();
        UpdateSerialLockText();
    }
}

/// <summary>Display model for one received-gift record slot.</summary>
public partial class GiftRecordEntryViewModel : ViewModelBase
{
    public GiftRecordEntryViewModel(GiftRecordEntry entry)
    {
        Index = entry.Index;
        IsEmpty = entry.IsEmpty;

        var loc = LocalizedStrings.Instance;
        if (entry.IsEmpty)
        {
            Title = loc.Format("MysteryGiftEditor_RecordSlotEmptyTitle", entry.Index + 1);
            return;
        }

        Title = loc.Format("MysteryGiftEditor_RecordSlotTitle", entry.Index + 1, GetCardTitle(entry, loc));
        Detail = GetDetail(entry, loc);
        Summary = BuildSummary(entry, loc);
    }

    public int Index { get; }
    public bool IsEmpty { get; }
    public string Title { get; } = string.Empty;
    public string Detail { get; } = string.Empty;
    public string Summary { get; } = string.Empty;

    private static string GetCardTitle(GiftRecordEntry entry, LocalizedStrings loc)
    {
        if (entry.Card is not null)
        {
            var title = entry.Card.CardTitle;
            if (!string.IsNullOrWhiteSpace(title))
                return title;
        }
        return GetKindLabel(entry.Kind, loc);
    }

    private static string GetKindLabel(GiftRecordKind kind, LocalizedStrings loc) => kind switch
    {
        GiftRecordKind.Pokemon => loc["MysteryGiftEditor_KindPokemon"],
        GiftRecordKind.Item => loc["MysteryGiftEditor_KindItem"],
        GiftRecordKind.UndergroundItem => loc["MysteryGiftEditor_KindUndergroundItem"],
        GiftRecordKind.BattlePoints => loc["MysteryGiftEditor_KindBP"],
        GiftRecordKind.Clothing => loc["MysteryGiftEditor_KindClothing"],
        GiftRecordKind.Money => loc["MysteryGiftEditor_KindMoney"],
        GiftRecordKind.OneDaySerial => loc["MysteryGiftEditor_KindOneDay"],
        _ => loc["MysteryGiftEditor_KindUnknown"],
    };

    private static string GetDetail(GiftRecordEntry entry, LocalizedStrings loc)
    {
        switch (entry.Kind)
        {
            case GiftRecordKind.Pokemon:
                var speciesNames = GameInfo.Strings.Species;
                var species = entry.Species < speciesNames.Count
                    ? speciesNames[entry.Species]
                    : loc.Format("MysteryGiftEditor_UnknownSpecies", entry.Species);
                if (entry.IsEgg)
                    species = loc.Format("MysteryGiftEditor_RecordEgg", species);
                return entry.OriginalTrainerName.Length == 0 ? species : $"{species} | {entry.OriginalTrainerName}";
            case GiftRecordKind.Item:
                var itemNames = GameInfo.Strings.Item;
                return string.Join(", ", entry.Items.Select(item =>
                {
                    var name = item.Id < itemNames.Count
                        ? itemNames[item.Id]
                        : loc.Format("MysteryGiftEditor_UnknownItem", item.Id);
                    return item.Count > 1 ? $"{name} ×{item.Count}" : name;
                }));
            case GiftRecordKind.Money:
                return entry.Amount > 0 ? loc.Format("MysteryGiftEditor_RecordMoney", entry.Amount) : GetKindLabel(entry.Kind, loc);
            case GiftRecordKind.BattlePoints:
                return entry.Amount > 0 ? $"{entry.Amount} {GetKindLabel(entry.Kind, loc)}" : GetKindLabel(entry.Kind, loc);
            default:
                return GetKindLabel(entry.Kind, loc);
        }
    }

    private static string BuildSummary(GiftRecordEntry entry, LocalizedStrings loc)
    {
        var parts = new List<string>(2);
        if (entry.ReceivedAt is { } received)
            parts.Add(loc.Format("MysteryGiftEditor_RecordReceivedAt", received.ToString("g")));
        if (entry.CardId != 0)
            parts.Add(loc.Format("MysteryGiftEditor_RecordCardId", entry.CardId));
        return string.Join(" | ", parts);
    }
}


public partial class MysteryGiftSlotViewModel : ViewModelBase
{
    public MysteryGiftSlotViewModel(int index, DataMysteryGift? gift)
    {
        Index = index;
        Gift = gift;
        UpdateFromGift();
    }

    public int Index { get; }
    public DataMysteryGift? Gift { get; set; }

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _species = string.Empty;

    [ObservableProperty]
    private string _summary = string.Empty;

    public void UpdateFromGift()
    {
        if (Gift is null || Gift.IsEmpty)
        {
            IsEmpty = true;
            Title = LocalizedStrings.Instance.Format("MysteryGiftEditor_SlotEmptyTitle", Index + 1);
            Species = string.Empty;
            Summary = string.Empty;
        }
        else
        {
            IsEmpty = false;
            Title = LocalizedStrings.Instance.Format("MysteryGiftEditor_SlotTitle", Index + 1, Gift.CardTitle);

            if (Gift.IsEntity)
            {
                var speciesId = Gift.Species;
                var speciesNames = GameInfo.Strings.Species;
                Species = speciesId < speciesNames.Count ? speciesNames[speciesId] : $"Species #{speciesId}";
                Summary = $"Lv. {Gift.Level} | {Gift.OriginalTrainerName}";
            }
            else
            {
                Species = Gift.Type.ToString();
                Summary = string.Empty;
            }
        }
    }
}
