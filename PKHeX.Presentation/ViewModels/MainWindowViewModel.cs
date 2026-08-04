using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Application.Abstractions.LiveHex;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ISaveFileGateway _saveFileService;
    private readonly IDialogService _dialogService;
    private readonly IWindowService _windowService;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly ISlotService _slotService;
    private readonly IClipboardService _clipboardService;
    private readonly IQrCodeService _qrCodeService;
    private readonly ISaveBackupService _saveBackupService;
    private readonly AppSettings _settings;
    private readonly ISettingsStore _settingsStore;
    private readonly IThemeService _themeService;
    private readonly UndoRedoService _undoRedo;
    private readonly LanguageService _languageService;
    private readonly IAutoLegalityService _autoLegalityService;
    private readonly ILiveHexService _liveHexService;
    private readonly ILivingDexService _livingDexService;
    private readonly IGiftRecordProvider _giftRecordProvider;
    private readonly UpdateCheckCoordinator _updateCoordinator;

    // Captured on the UI thread at construction so update-check continuations (which may complete on
    // a thread-pool thread) marshal their status-bar mutation back onto the UI thread.
    private readonly SynchronizationContext? _uiContext = SynchronizationContext.Current;

    [ObservableProperty] private UpdateNotificationViewModel? _updateNotification;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSave))]
    [NotifyPropertyChangedFor(nameof(WindowTitle))]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    [NotifyCanExecuteChangedFor(nameof(SaveFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveFileAsCommand))]
    [NotifyCanExecuteChangedFor(nameof(CloseFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(ImportShowdownCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportShowdownCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenPKMDatabaseCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenBoxReportCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenAutoLegalityModCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenLiveHeXCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenLivingDexGeneratorCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenBackupManagerCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenSaveDiffCommand))]
    [NotifyCanExecuteChangedFor(nameof(UndoCommand))]
    [NotifyCanExecuteChangedFor(nameof(RedoCommand))]
    private SaveFile? _currentSave;

    [ObservableProperty] private BoxViewerViewModel? _boxViewer;
    [ObservableProperty] private PartyViewerViewModel? _partyViewer;
    [ObservableProperty] private TrainerEditorViewModel? _trainerEditor;
    [ObservableProperty] private InventoryEditorViewModel? _inventoryEditor;
    [ObservableProperty] private EventFlagsEditorViewModel? _eventFlagsEditor;
    [ObservableProperty] private MysteryGiftEditorViewModel? _mysteryGiftEditor;
    [ObservableProperty] private BatchEditorViewModel? _batchEditor;
    [ObservableProperty] private PokemonEditorViewModel? _currentPokemonEditor;

    public bool HasSave => CurrentSave is not null;
    public bool CanUndo => _undoRedo.CanUndo;
    public bool CanRedo => _undoRedo.CanRedo;

    public string WindowTitle => CurrentSave is not null
        ? $"PKHeX Avalonia - {CurrentSave.Version}"
        : "PKHeX Avalonia";

    /// <summary>Localized status-bar text: the loaded game version, or a "no save loaded" hint.</summary>
    public string StatusText => CurrentSave is not null
        ? LocalizedStrings.Instance.Format("Status_GameFormat", CurrentSave.Version)
        : LocalizedStrings.Instance["Status_NoSaveLoaded"];

    public LanguageService LanguageService => _languageService;

    public MainWindowViewModel(
        ISaveFileGateway saveFileService,
        IDialogService dialogService,
        IWindowService windowService,
        ISpriteRenderer spriteRenderer,
        ISlotService slotService,
        IClipboardService clipboardService,
        IQrCodeService qrCodeService,
        UpdateCheckCoordinator updateCoordinator,
        ISaveBackupService saveBackupService,
        AppSettings settings,
        ISettingsStore settingsStore,
        IThemeService themeService,
        UndoRedoService undoRedo,
        LanguageService languageService,
        IAutoLegalityService autoLegalityService,
        ILiveHexService liveHexService,
        ILivingDexService livingDexService,
        IGiftRecordProvider giftRecordProvider)
    {
        _saveFileService = saveFileService;
        _dialogService = dialogService;
        _windowService = windowService;
        _spriteRenderer = spriteRenderer;
        _slotService = slotService;
        _clipboardService = clipboardService;
        _qrCodeService = qrCodeService;
        _updateCoordinator = updateCoordinator;
        _saveBackupService = saveBackupService;
        _settings = settings;
        _settingsStore = settingsStore;
        _themeService = themeService;
        _undoRedo = undoRedo;
        _languageService = languageService;
        _autoLegalityService = autoLegalityService;
        _liveHexService = liveHexService;
        _livingDexService = livingDexService;
        _giftRecordProvider = giftRecordProvider;

        // Mirror the coordinator's status-bar notification (raised by either the startup check or a
        // manual "Check for Updates" from the Settings/About dialogs) into the bound property.
        // Marshal onto the UI thread: a startup check completes on a thread-pool continuation.
        _updateCoordinator.NotificationChanged += n => RunOnUiThread(() => UpdateNotification = n);
        UpdateNotification = _updateCoordinator.Notification;

        _saveFileService.SaveFileChanged += OnSaveFileChanged;
        _slotService.ViewRequested += OnViewRequested;
        _slotService.SetRequested += OnSetRequested;
        _slotService.DeleteRequested += OnDeleteRequested;
        _slotService.MoveRequested += OnMoveRequested;

        _undoRedo.StateChanged += (_, _) =>
        {
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
        };
        _undoRedo.UndoPerformed += OnUndoRedoPerformed;
        _undoRedo.RedoPerformed += OnUndoRedoPerformed;

        _languageService.LanguageChanged += OnLanguageChanged;

        // Bring the UI-chrome string table to the language the LanguageService was initialized with
        // (from persisted settings at startup) before the window renders.
        LocalizedStrings.Instance.SetLanguage(_languageService.CurrentLanguage);
    }

    private void OnLanguageChanged()
    {
        // Swap the shell's UI-chrome strings, then persist the choice so it survives a restart.
        LocalizedStrings.Instance.SetLanguage(_languageService.CurrentLanguage);
        if (!string.Equals(_settings.DisplayLanguage, _languageService.CurrentLanguage, StringComparison.Ordinal))
        {
            _settings.DisplayLanguage = _languageService.CurrentLanguage;
            _settingsStore.Save(_settings);
        }

        if (CurrentSave is not null)
            GameInfo.FilteredSources = new FilteredGameDataSource(CurrentSave, GameInfo.Sources);

        OnPropertyChanged(string.Empty);
        CurrentPokemonEditor?.RefreshLanguage();
        BoxViewer?.RefreshCurrentBox();
        TrainerEditor?.RefreshLanguage();
        InventoryEditor?.RefreshLanguage();
        MysteryGiftEditor?.RefreshLocalization();

        // Relay to on-demand dialog ViewModels (e.g. PKM database) that subscribe via the messenger.
        WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(_languageService.CurrentLanguage));
    }

    private void OnSaveFileChanged(SaveFile? sav)
    {
        // Dismiss any modeless tool windows (e.g. the box seek tool) bound to the previous save.
        _windowService.CloseAllTools();
        _boxReport = null;
        _legalityAudit = null;
        _autoLegalityMod = null;
        DisposeLiveHeX();
        _livingDexGenerator = null;
        _backupManager = null;
        _saveDiff = null;

        CurrentSave = sav;
        if (sav is not null)
        {
            try
            {
                _spriteRenderer.Initialize(sav);
                _undoRedo.Initialize(sav);
                GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);

                var pokemonEditor = new PokemonEditorViewModel(sav.BlankPKM, sav, _spriteRenderer, _dialogService, _windowService);
                pokemonEditor.SaveFileDropRequested += OnSaveFileDropRequested;
                CurrentPokemonEditor = pokemonEditor;

                var boxViewer = new BoxViewerViewModel(sav, _spriteRenderer, _slotService, _windowService, _dialogService);
                boxViewer.SlotActivated += OnBoxSlotActivated;
                boxViewer.ViewSlotRequested += OnBoxViewSlot;
                boxViewer.SetSlotRequested += OnBoxSetSlot;
                boxViewer.DeleteSlotRequested += OnBoxDeleteSlot;
                boxViewer.SaveFileDropRequested += OnSaveFileDropRequested;
                BoxViewer = boxViewer;

                var partyViewer = new PartyViewerViewModel(sav, _spriteRenderer, _slotService, _dialogService);
                partyViewer.SlotActivated += OnPartySlotActivated;
                partyViewer.ViewSlotRequested += OnPartyViewSlot;
                partyViewer.SetSlotRequested += OnPartySetSlot;
                partyViewer.SaveFileDropRequested += OnSaveFileDropRequested;
                PartyViewer = partyViewer;

                TrainerEditor = new TrainerEditorViewModel(sav);
                InventoryEditor = new InventoryEditorViewModel(sav, _spriteRenderer);
                EventFlagsEditor = new EventFlagsEditorViewModel(sav);
                MysteryGiftEditor = new MysteryGiftEditorViewModel(sav, _dialogService, _giftRecordProvider);
                BatchEditor = new BatchEditorViewModel(sav, _dialogService);
                BatchEditor.BatchEditCompleted += OnBatchEditCompleted;
            }
            catch (Exception ex)
            {
                _saveFileService.CloseSave();
                _ = _dialogService.ShowErrorAsync(
                    LocalizedStrings.Instance["Msg_FailedToOpenSave"],
                    LocalizedStrings.Instance.Format("Msg_FailedToOpenSave_Body", ex.GetType().Name, ex.Message));
            }
        }
        else
        {
            CurrentPokemonEditor = null;

            if (BoxViewer is not null)
            {
                BoxViewer.SlotActivated -= OnBoxSlotActivated;
                BoxViewer.ViewSlotRequested -= OnBoxViewSlot;
                BoxViewer.SetSlotRequested -= OnBoxSetSlot;
                BoxViewer.DeleteSlotRequested -= OnBoxDeleteSlot;
                BoxViewer.SaveFileDropRequested -= OnSaveFileDropRequested;
            }
            BoxViewer = null;

            if (PartyViewer is not null)
            {
                PartyViewer.SlotActivated -= OnPartySlotActivated;
                PartyViewer.ViewSlotRequested -= OnPartyViewSlot;
                PartyViewer.SetSlotRequested -= OnPartySetSlot;
                PartyViewer.SaveFileDropRequested -= OnSaveFileDropRequested;
            }
            PartyViewer = null;

            TrainerEditor = null;
            InventoryEditor = null;
            EventFlagsEditor = null;
            MysteryGiftEditor = null;

            if (BatchEditor is not null)
            {
                BatchEditor.BatchEditCompleted -= OnBatchEditCompleted;
                BatchEditor = null;
            }
        }
    }

    /// <summary>
    /// Fire-and-forget from the host at startup (never awaited there, so the main window is never
    /// blocked). Delegates to the shared <see cref="UpdateCheckCoordinator"/>, which honors the
    /// startup opt-out and stays silent on failure.
    /// </summary>
    public Task CheckForUpdatesAsync() => _updateCoordinator.RunStartupCheckAsync();

    private void RunOnUiThread(Action action)
    {
        if (_uiContext is null || SynchronizationContext.Current == _uiContext)
            action();
        else
            _uiContext.Post(_ => action(), null);
    }

    /// <summary>
    /// Single guarded entry point for loading a Pokémon into the current editor from any of the
    /// on-demand selection sources (PKM database, mystery gift database, Auto Legality Mod, box
    /// report, legality audit). A null/malformed entity or a conversion that throws mid-load
    /// surfaces as an error dialog instead of crashing the application.
    /// </summary>
    private void LoadEntity(PKM? pk)
    {
        if (pk is null || CurrentPokemonEditor is null)
            return;

        try
        {
            CurrentPokemonEditor.LoadPKM(pk);
        }
        catch (Exception ex)
        {
            _ = _dialogService.ShowErrorAsync(
                LocalizedStrings.Instance["Common_Error"],
                LocalizedStrings.Instance.Format("File_LoadEntityFailed", ex.Message));
        }
    }
}
