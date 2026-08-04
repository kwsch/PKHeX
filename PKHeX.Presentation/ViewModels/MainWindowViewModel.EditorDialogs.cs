using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class MainWindowViewModel
{
    /// <summary>Short alias for localized dialog-title lookup (issue #132 UI localization).</summary>
    private static string T(string key) => LocalizedStrings.Instance[key];

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo() => _undoRedo.Undo();

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo() => _undoRedo.Redo();

    [RelayCommand]
    private void ChangeLanguage(string languageCode) => _languageService.SetLanguage(languageCode);

    [RelayCommand]
    private async Task OpenAboutAsync()
    {
        await _windowService.ShowDialogAsync(new AboutViewModel(_updateCoordinator), T("Dialog_About"));
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        var vm = new SettingsViewModel(_settings, _settingsStore, _themeService, _languageService, _updateCoordinator);
        await _windowService.ShowDialogAsync(vm, T("Dialog_Settings"));

        // The sprite preference may have changed; re-apply the style and refresh open views.
        if (CurrentSave is not null)
        {
            _spriteRenderer.Initialize(CurrentSave);
            BoxViewer?.RefreshCurrentBox();
            PartyViewer?.RefreshParty();
            CurrentPokemonEditor?.RefreshSprite();
        }
    }

    [RelayCommand]
    private async Task OpenFolderListAsync()
    {
        var vm = new FolderListViewModel(_saveFileService, _settings, _dialogService);
        await _windowService.ShowDialogAsync(vm, T("Dialog_SaveFolderList"));
    }

    // No HasSave gate: the troubleshooter exists to open a save the normal loader cannot recognize,
    // so it must be reachable with no save loaded.
    [RelayCommand]
    private async Task OpenSaveHandlerTroubleshooterAsync()
    {
        var vm = new SaveHandlerTroubleshooterViewModel(_dialogService, _saveFileService);
        await _windowService.ShowDialogAsync(vm, T("Dialog_SaveHandlerTroubleshooter"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenBatchEditorAsync()
    {
        if (CurrentSave is null) return;
        var vm = new BatchEditorViewModel(CurrentSave, _dialogService);
        vm.BatchEditCompleted += OnBatchEditCompleted;
        await _windowService.ShowDialogAsync(vm, T("Dialog_BatchEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenBlockEditorAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new BlockEditorViewModel(CurrentSave, _dialogService),
            T("Dialog_BlockEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenBoxManipAsync()
    {
        if (CurrentSave is null) return;
        var vm = new BoxManipViewModel(CurrentSave, _dialogService, () => BoxViewer?.RefreshCurrentBox());
        await _windowService.ShowDialogAsync(vm, T("Dialog_BoxManipulation"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenEncounterDatabaseAsync()
    {
        if (CurrentSave is null) return;
        var vm = new EncounterDatabaseViewModel(CurrentSave, _spriteRenderer, _dialogService,
            pk => CurrentPokemonEditor?.LoadPKM(pk));
        await _windowService.ShowDialogAsync(vm, T("Dialog_EncounterDatabase"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenGroupViewerAsync()
    {
        if (CurrentSave is null) return;

        System.Collections.Generic.IReadOnlyList<SlotGroup>? groups = null;
        if (CurrentSave is SAV_STADIUM s0)
            groups = s0.GetRegisteredTeams();

        if (groups is null || groups.Count == 0)
        {
            await _dialogService.ShowErrorAsync(T("Common_Info"), T("Msg_NoGroupsAvailable"));
            return;
        }

        var vm = new GroupViewerViewModel(CurrentSave, groups, _spriteRenderer, _slotService);
        await _windowService.ShowDialogAsync(vm, T("Dialog_GroupViewer"));
    }

    // — Save-specific editors —

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenDaycareAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new DaycareEditorViewModel(CurrentSave, _spriteRenderer),
            T("Dialog_Daycare"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRecordsAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new RecordsEditorViewModel(CurrentSave),
            T("Dialog_GameRecords"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenHallOfFameAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new HallOfFameEditorViewModel(CurrentSave, _spriteRenderer),
            T("Dialog_HallOfFame"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenSecretBaseAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new SecretBaseEditorViewModel(CurrentSave, _spriteRenderer),
            T("Dialog_SecretBaseEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPokebeanAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new PokebeanEditorViewModel(CurrentSave),
            T("Dialog_PokeBeanEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenFestivalPlazaAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new FestivalPlazaEditorViewModel(CurrentSave),
            T("Dialog_FestivalPlazaEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRaidEditorAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new RaidEditorViewModel(CurrentSave),
            T("Dialog_RaidEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenSuperTrainingAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new SuperTrainingEditorViewModel(CurrentSave),
            T("Dialog_SuperTrainingEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenApricornAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new ApricornEditorViewModel(CurrentSave),
            T("Dialog_ApricornEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPokeGear4Async()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new PokeGear4EditorViewModel(CurrentSave),
            T("Dialog_PokeGearEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenGeonet4Async()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new Geonet4EditorViewModel(CurrentSave),
            T("Dialog_GeonetGlobeEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenBoxLayoutAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new BoxLayoutEditorViewModel(CurrentSave),
            T("Dialog_BoxLayoutEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenHoneyTreeAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new HoneyTreeEditorViewModel(CurrentSave),
            T("Dialog_HoneyTreeEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenUndergroundAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new UndergroundEditorViewModel(CurrentSave),
            T("Dialog_UndergroundEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRoamerAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new RoamerEditorViewModel(CurrentSave),
            T("Dialog_RoamerEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenOPowerAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new OPowerEditorViewModel(CurrentSave),
            T("Dialog_OPowerEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenZygardeCellAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new ZygardeCellEditorViewModel(CurrentSave),
            T("Dialog_ZygardeCellEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRaid9Async()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new Raid9EditorViewModel(CurrentSave),
            T("Dialog_TeraRaidEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRaidSevenStar9Async()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new RaidSevenStar9EditorViewModel(CurrentSave),
            T("Dialog_SevenStarTeraRaidEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenFashion9Async()
    {
        if (CurrentSave is not (SAV9SV or SAV9ZA)) return;
        await _windowService.ShowDialogAsync(
            new Fashion9EditorViewModel(CurrentSave!),
            T("Dialog_FashionEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenDonutAsync()
    {
        if (CurrentSave is not SAV9ZA sav) return;
        await _windowService.ShowDialogAsync(
            new DonutEditorViewModel(sav),
            T("Dialog_DonutEditorPLZA"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenDLC5Async()
    {
        if (CurrentSave is not SAV5 sav) return;
        await _windowService.ShowDialogAsync(
            new DLC5EditorViewModel(sav, _dialogService),
            T("Dialog_DLCEditorGen5"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenEntralinkAsync()
    {
        if (CurrentSave is not SAV5 sav) return;
        await _windowService.ShowDialogAsync(
            new EntralinkEditorViewModel(sav),
            T("Dialog_EntralinkEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenGlobalLink5Async()
    {
        if (CurrentSave is not SAV5 sav) return;
        await _windowService.ShowDialogAsync(
            new GlobalLink5EditorViewModel(sav, _spriteRenderer),
            T("Dialog_GlobalLinkEditorGen5"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPokedexAsync()
    {
        if (CurrentSave is null) return;

        (object view, string title) = CurrentSave switch
        {
            SAV9SV s    => ((object)new PokedexGen9EditorViewModel(s),   T("Dialog_PokedexEditorGen9SV")),
            SAV8SWSH s  => (new Pokedex8EditorViewModel(s),                T("Dialog_PokedexEditorGen8SwSh")),
            SAV8BS s    => (new Pokedex8bEditorViewModel(s),               T("Dialog_PokedexEditorGen8BDSP")),
            SAV8LA s    => (new PokedexLAEditorViewModel(s),               T("Dialog_PokedexEditorPLA")),
            SAV7b s     => (new Pokedex7bEditorViewModel(s),               T("Dialog_PokedexEditorLetsGo")),
            SAV7 s      => (new Pokedex7EditorViewModel(s),                T("Dialog_PokedexEditorGen7")),
            SAV6 s      => (new Pokedex6EditorViewModel(s),                T("Dialog_PokedexEditorGen6")),
            SAV5 s      => (new Pokedex5EditorViewModel(s),                T("Dialog_PokedexEditorGen5")),
            SAV4 s      => (new Pokedex4EditorViewModel(s),                T("Dialog_PokedexEditorGen4")),
            _           => (new PokedexSimpleEditorViewModel(CurrentSave), T("Dialog_PokedexEditorSimple")),
        };

        await _windowService.ShowDialogAsync(view, title);
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenBattlePassAsync()
    {
        if (CurrentSave is not SAV4BR sav) return;
        await _windowService.ShowDialogAsync(
            new BattlePassEditorViewModel(sav),
            T("Dialog_BattlePassEditorPBR"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenUnderground8bAsync()
    {
        if (CurrentSave is not SAV8BS sav) return;
        await _windowService.ShowDialogAsync(
            new Underground8bEditorViewModel(sav),
            T("Dialog_UndergroundEditorBDSP"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenSealStickers8bAsync()
    {
        if (CurrentSave is not SAV8BS sav) return;
        await _windowService.ShowDialogAsync(
            new SealStickers8bEditorViewModel(sav),
            T("Dialog_SealStickersEditorBDSP"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPoffin8bAsync()
    {
        if (CurrentSave is not SAV8BS sav) return;
        await _windowService.ShowDialogAsync(
            new Poffin8bEditorViewModel(sav),
            T("Dialog_PoffinEditorBDSP"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenLink6Async()
    {
        if (CurrentSave is not SAV6 sav) return;
        await _windowService.ShowDialogAsync(
            new Link6EditorViewModel(sav, _dialogService),
            T("Dialog_PokemonLinkEditorGen6"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenSimpleTrainerAsync()
    {
        if (CurrentSave is not (SAV1 or SAV2 or SAV3 or SAV4 or SAV5)) return;
        await _windowService.ShowDialogAsync(
            new SimpleTrainerEditorViewModel(CurrentSave!),
            T("Dialog_SimpleTrainerEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenGearBRAsync()
    {
        if (CurrentSave is not SAV4BR sav) return;
        await _windowService.ShowDialogAsync(
            new GearBREditorViewModel(sav),
            T("Dialog_GearEditorBattleRevolution"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenSecretBase3Async()
    {
        if (CurrentSave is not SAV3 sav3hoenn || sav3hoenn is not (SAV3RS or SAV3E)) return;
        await _windowService.ShowDialogAsync(
            new SecretBase3EditorViewModel(sav3hoenn),
            T("Dialog_SecretBaseEditorRSE"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenSecretBase6Async()
    {
        if (CurrentSave is not SAV6AO sav) return;
        await _windowService.ShowDialogAsync(
            new SecretBase6EditorViewModel(sav),
            T("Dialog_SecretBaseEditorORAS"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPokepuffAsync()
    {
        if (CurrentSave is not ISaveBlock6Main sav) return;
        await _windowService.ShowDialogAsync(
            new PokepuffEditorViewModel(sav),
            T("Dialog_PokePuffEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPokeBlockAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new PokeBlockEditorViewModel(CurrentSave),
            T("Dialog_PokeblockEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenBerryFieldAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new BerryFieldEditorViewModel(CurrentSave),
            T("Dialog_BerryFieldEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRoamer6Async()
    {
        if (CurrentSave is not SAV6XY sav) return;
        await _windowService.ShowDialogAsync(
            new Roamer6EditorViewModel(sav),
            T("Dialog_RoamerEditorXY"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenChatterAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new ChatterEditorViewModel(CurrentSave),
            T("Dialog_ChatterEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRTCAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new RTCEditorViewModel(CurrentSave),
            T("Dialog_RTCEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMedalAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new MedalEditorViewModel(CurrentSave, _dialogService),
            T("Dialog_MedalRallyEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenUnityTower5Async()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new UnityTower5EditorViewModel(CurrentSave),
            T("Dialog_UnityTowerEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPoffinCaseAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new PoffinCaseEditorViewModel(CurrentSave),
            T("Dialog_PoffinCaseEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPoketchAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new PoketchEditorViewModel(CurrentSave),
            T("Dialog_PoketchEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPokeBlock3CaseAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new PokeBlock3CaseEditorViewModel(CurrentSave),
            T("Dialog_PokeblockCaseEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenHallOfFame3Async()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new HallOfFame3EditorViewModel(CurrentSave),
            T("Dialog_HallOfFameGen3"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenFashionAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new FashionEditorViewModel(CurrentSave),
            T("Dialog_FashionEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenTrainerCard8Async()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new TrainerCard8EditorViewModel(CurrentSave),
            T("Dialog_TrainerCardEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMailBoxAsync()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new MailBoxEditorViewModel(CurrentSave),
            T("Dialog_MailBoxEditor"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenEventFlags2Async()
    {
        if (CurrentSave is not SAV2 sav) return;
        await _windowService.ShowDialogAsync(
            new EventFlags2EditorViewModel(sav),
            T("Dialog_EventFlagsGen2"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc2Async()
    {
        if (CurrentSave is not SAV2 sav) return;
        await _windowService.ShowDialogAsync(
            new Misc2EditorViewModel(sav),
            T("Dialog_MiscEditorGen2"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc3Async()
    {
        if (CurrentSave is not SAV3 sav) return;
        await _windowService.ShowDialogAsync(
            new Misc3EditorViewModel(sav),
            T("Dialog_MiscEditorGen3"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc4Async()
    {
        if (CurrentSave is not SAV4 sav) return;
        await _windowService.ShowDialogAsync(
            new Misc4EditorViewModel(sav),
            T("Dialog_MiscEditorGen4"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPokeathlonAsync()
    {
        if (CurrentSave is not SAV4HGSS sav) return;
        await _windowService.ShowDialogAsync(
            new PokeathlonEditorViewModel(sav, _spriteRenderer),
            T("Dialog_PokeathlonEditorHGSS"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc5Async()
    {
        if (CurrentSave is not SAV5 sav) return;
        await _windowService.ShowDialogAsync(
            new Misc5EditorViewModel(sav),
            T("Dialog_MiscEditorGen5"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenJoinAvenueAsync()
    {
        if (CurrentSave is not SAV5B2W2 sav) return;
        await _windowService.ShowDialogAsync(
            new JoinAvenueEditorViewModel(sav, _spriteRenderer, _dialogService),
            T("Dialog_JoinAvenueEditorB2W2"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc7Async()
    {
        if (CurrentSave is not SAV7 sav) return;
        await _windowService.ShowDialogAsync(
            new Misc7EditorViewModel(sav),
            T("Dialog_MiscEditorGen7"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc8Async()
    {
        if (CurrentSave is not SAV8SWSH sav) return;
        await _windowService.ShowDialogAsync(
            new Misc8EditorViewModel(sav),
            T("Dialog_MiscEditorSWSH"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc9Async()
    {
        if (CurrentSave is not SAV9SV sav) return;
        await _windowService.ShowDialogAsync(
            new Misc9EditorViewModel(sav),
            T("Dialog_MiscEditorSV"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc7bAsync()
    {
        if (CurrentSave is not SAV7b sav) return;
        await _windowService.ShowDialogAsync(
            new Misc7bEditorViewModel(sav),
            T("Dialog_MiscEditorLetsGo"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc8aAsync()
    {
        if (CurrentSave is not SAV8LA sav) return;
        await _windowService.ShowDialogAsync(
            new Misc8aEditorViewModel(sav),
            T("Dialog_MiscEditorLegendsArceus"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMisc8bAsync()
    {
        if (CurrentSave is not SAV8BS sav) return;
        await _windowService.ShowDialogAsync(
            new Misc8bEditorViewModel(sav),
            T("Dialog_MiscEditorBDSP"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenEventReset1Async()
    {
        if (CurrentSave is not SAV1 sav) return;
        await _windowService.ShowDialogAsync(
            new EventReset1EditorViewModel(sav),
            T("Dialog_EventResetGen1"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenHallOfFame1Async()
    {
        if (CurrentSave is not SAV1 sav) return;
        await _windowService.ShowDialogAsync(
            new HallOfFame1EditorViewModel(sav),
            T("Dialog_HallOfFameGen1"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRoamer3Async()
    {
        if (CurrentSave is not SAV3 sav) return;
        await _windowService.ShowDialogAsync(
            new Roamer3EditorViewModel(sav),
            T("Dialog_RoamerEditorGen3"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenRTC3Async()
    {
        if (CurrentSave is not (SAV3RS or SAV3E)) return;
        await _windowService.ShowDialogAsync(
            new RTC3EditorViewModel(CurrentSave!),
            T("Dialog_RTCEditorGen3"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenCapture7GGAsync()
    {
        if (CurrentSave is not SAV7b sav) return;
        await _windowService.ShowDialogAsync(
            new Capture7GGEditorViewModel(sav),
            T("Dialog_CaptureRecordsLetsGo"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenHallOfFame7Async()
    {
        if (CurrentSave is null) return;
        await _windowService.ShowDialogAsync(
            new HallOfFame7EditorViewModel(CurrentSave),
            T("Dialog_HallOfFameSMUSUM"));
    }
}
