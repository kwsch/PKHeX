namespace PKHeX.Core;

public interface IProgramSettings
{
    IStartupSettings Startup { get; }
    BackupSettings Backup { get; }
    SaveLanguageSettings SaveLanguage { get; }
    SlotWriteSettings SlotWrite { get; }
    SetImportSettings Import { get; }
    LegalitySettings Legality { get; }
    EntityConverterSettings Converter { get; }
    LocalResourceSettings LocalResources { get; }
}
