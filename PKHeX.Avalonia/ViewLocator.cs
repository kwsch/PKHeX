using System;
using System.Collections.Generic;
using Avalonia.Controls;
using PKHeX.Avalonia.Views;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia;

/// <summary>
/// Maps a dialog ViewModel type to its View. This is the single place that couples ViewModels to
/// Views; it lives in the host (Frameworks &amp; Drivers) so the Presentation layer never references Views.
/// Compile-checked: a renamed View breaks the build here.
/// </summary>
public static class ViewLocator
{
    private static readonly Dictionary<Type, Func<Control>> Map = new()
    {
        [typeof(AboutViewModel)] = () => new AboutView(),
        [typeof(ApricornEditorViewModel)] = () => new ApricornEditor(),
        [typeof(AutoLegalityModViewModel)] = () => new AutoLegalityModView(),
        [typeof(BackupManagerViewModel)] = () => new BackupManager(),
        [typeof(BatchEditorViewModel)] = () => new BatchEditor(),
        [typeof(BattlePassEditorViewModel)] = () => new BattlePassEditor(),
        [typeof(BerryFieldEditorViewModel)] = () => new BerryFieldEditorView(),
        [typeof(BlockEditorViewModel)] = () => new BlockEditor(),
        [typeof(BoxLayoutEditorViewModel)] = () => new BoxLayoutEditor(),
        [typeof(BoxManipViewModel)] = () => new BoxManipView(),
        [typeof(BoxReportViewModel)] = () => new BoxReportView(),
        [typeof(Capture7GGEditorViewModel)] = () => new Capture7GGEditor(),
        [typeof(ChatterEditorViewModel)] = () => new ChatterEditor(),
        [typeof(DLC5EditorViewModel)] = () => new DLC5Editor(),
        [typeof(DaycareEditorViewModel)] = () => new DaycareEditorView(),
        [typeof(DonutEditorViewModel)] = () => new DonutEditor(),
        [typeof(EncounterDatabaseViewModel)] = () => new EncounterDatabaseView(),
        [typeof(EntralinkEditorViewModel)] = () => new EntralinkEditor(),
        [typeof(EventFlags2EditorViewModel)] = () => new EventFlags2Editor(),
        [typeof(EventReset1EditorViewModel)] = () => new EventReset1Editor(),
        [typeof(Fashion9EditorViewModel)] = () => new Fashion9Editor(),
        [typeof(FashionEditorViewModel)] = () => new FashionEditorView(),
        [typeof(FestivalPlazaEditorViewModel)] = () => new FestivalPlazaEditor(),
        [typeof(FolderListViewModel)] = () => new FolderList(),
        [typeof(GearBREditorViewModel)] = () => new GearBREditor(),
        [typeof(Geonet4EditorViewModel)] = () => new Geonet4Editor(),
        [typeof(GlobalLink5EditorViewModel)] = () => new GlobalLink5Editor(),
        [typeof(GroupViewerViewModel)] = () => new GroupViewer(),
        [typeof(HallOfFame1EditorViewModel)] = () => new HallOfFame1Editor(),
        [typeof(HallOfFame3EditorViewModel)] = () => new HallOfFame3EditorView(),
        [typeof(HallOfFame7EditorViewModel)] = () => new HallOfFame7Editor(),
        [typeof(HallOfFameEditorViewModel)] = () => new HallOfFameEditor(),
        [typeof(HoneyTreeEditorViewModel)] = () => new HoneyTreeEditor(),
        [typeof(JoinAvenueEditorViewModel)] = () => new JoinAvenueEditor(),
        [typeof(LegalityAuditViewModel)] = () => new LegalityAuditView(),
        [typeof(LegalityViewModel)] = () => new LegalityView(),
        [typeof(LiveHeXViewModel)] = () => new LiveHeXView(),
        [typeof(Link6EditorViewModel)] = () => new Link6Editor(),
        [typeof(LivingDexGeneratorViewModel)] = () => new LivingDexGeneratorView(),
        [typeof(MailBoxEditorViewModel)] = () => new MailBoxEditor(),
        [typeof(MedalEditorViewModel)] = () => new MedalEditorView(),
        [typeof(MemoryEditorViewModel)] = () => new MemoryEditor(),
        [typeof(Misc2EditorViewModel)] = () => new Misc2Editor(),
        [typeof(Misc3EditorViewModel)] = () => new Misc3Editor(),
        [typeof(Misc4EditorViewModel)] = () => new Misc4Editor(),
        [typeof(Misc5EditorViewModel)] = () => new Misc5Editor(),
        [typeof(Misc7EditorViewModel)] = () => new Misc7Editor(),
        [typeof(Misc7bEditorViewModel)] = () => new Misc7bEditor(),
        [typeof(Misc8EditorViewModel)] = () => new Misc8Editor(),
        [typeof(Misc8aEditorViewModel)] = () => new Misc8aEditor(),
        [typeof(Misc8bEditorViewModel)] = () => new Misc8bEditor(),
        [typeof(Misc9EditorViewModel)] = () => new Misc9Editor(),
        [typeof(MysteryGiftDatabaseViewModel)] = () => new MysteryGiftDatabaseView(),
        [typeof(OPowerEditorViewModel)] = () => new OPowerEditor(),
        [typeof(PKMDatabaseViewModel)] = () => new PKMDatabaseView(),
        [typeof(EntitySeekViewModel)] = () => new EntitySeekView(),
        [typeof(Poffin8bEditorViewModel)] = () => new Poffin8bEditor(),
        [typeof(PoffinCaseEditorViewModel)] = () => new PoffinCaseEditorView(),
        [typeof(PokeBlock3CaseEditorViewModel)] = () => new PokeBlock3CaseEditorView(),
        [typeof(PokeBlockEditorViewModel)] = () => new PokeBlockEditor(),
        [typeof(PokeGear4EditorViewModel)] = () => new PokeGear4Editor(),
        [typeof(PokeathlonEditorViewModel)] = () => new PokeathlonEditor(),
        [typeof(PokebeanEditorViewModel)] = () => new PokebeanEditor(),
        [typeof(Pokedex4EditorViewModel)] = () => new Pokedex4Editor(),
        [typeof(Pokedex5EditorViewModel)] = () => new Pokedex5Editor(),
        [typeof(Pokedex6EditorViewModel)] = () => new Pokedex6Editor(),
        [typeof(Pokedex7EditorViewModel)] = () => new Pokedex7Editor(),
        [typeof(Pokedex7bEditorViewModel)] = () => new Pokedex7bEditor(),
        [typeof(Pokedex8EditorViewModel)] = () => new Pokedex8Editor(),
        [typeof(Pokedex8bEditorViewModel)] = () => new Pokedex8bEditor(),
        [typeof(PokedexGen9EditorViewModel)] = () => new PokedexGen9Editor(),
        [typeof(PokedexLAEditorViewModel)] = () => new PokedexLAEditor(),
        [typeof(PokedexSimpleEditorViewModel)] = () => new PokedexSimpleEditor(),
        [typeof(PokepuffEditorViewModel)] = () => new PokepuffEditor(),
        [typeof(PoketchEditorViewModel)] = () => new PoketchEditorView(),
        [typeof(QrCodeViewModel)] = () => new QrCodeView(),
        [typeof(RTC3EditorViewModel)] = () => new RTC3Editor(),
        [typeof(RTCEditorViewModel)] = () => new RTCEditor(),
        [typeof(Raid9EditorViewModel)] = () => new Raid9Editor(),
        [typeof(RaidEditorViewModel)] = () => new RaidEditor(),
        [typeof(RaidSevenStar9EditorViewModel)] = () => new RaidSevenStar9Editor(),
        [typeof(RecordsEditorViewModel)] = () => new RecordsEditorView(),
        [typeof(RibbonEditorViewModel)] = () => new RibbonEditor(),
        [typeof(Roamer3EditorViewModel)] = () => new Roamer3Editor(),
        [typeof(Roamer6EditorViewModel)] = () => new Roamer6Editor(),
        [typeof(RoamerEditorViewModel)] = () => new RoamerEditor(),
        [typeof(SaveDiffViewModel)] = () => new SaveDiffView(),
        [typeof(SaveHandlerTroubleshooterViewModel)] = () => new SaveHandlerTroubleshooter(),
        [typeof(SealStickers8bEditorViewModel)] = () => new SealStickers8bEditor(),
        [typeof(SecretBase3EditorViewModel)] = () => new SecretBase3Editor(),
        [typeof(SecretBase6EditorViewModel)] = () => new SecretBase6Editor(),
        [typeof(SecretBaseEditorViewModel)] = () => new SecretBaseEditor(),
        [typeof(SettingsViewModel)] = () => new SettingsView(),
        [typeof(SimpleTrainerEditorViewModel)] = () => new SimpleTrainerEditor(),
        [typeof(SuperTrainingEditorViewModel)] = () => new SuperTrainingEditor(),
        [typeof(TechRecordEditorViewModel)] = () => new TechRecordEditor(),
        [typeof(TrainerCard8EditorViewModel)] = () => new TrainerCard8EditorView(),
        [typeof(Underground8bEditorViewModel)] = () => new Underground8bEditor(),
        [typeof(UndergroundEditorViewModel)] = () => new UndergroundEditor(),
        [typeof(UnityTower5EditorViewModel)] = () => new UnityTower5Editor(),
        [typeof(UpdateChangelogViewModel)] = () => new UpdateChangelogView(),
        [typeof(UpdateDownloadViewModel)] = () => new UpdateDownloadView(),
        [typeof(ZygardeCellEditorViewModel)] = () => new ZygardeCellEditor(),
    };

    public static Control Build(object viewModel)
    {
        if (Map.TryGetValue(viewModel.GetType(), out var factory))
        {
            var view = factory();
            view.DataContext = viewModel;
            return view;
        }
        return new TextBlock { Text = "View not found: " + viewModel.GetType().Name };
    }
}
