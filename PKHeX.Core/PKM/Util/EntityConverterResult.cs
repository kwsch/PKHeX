namespace PKHeX.Core;

public enum EntityConverterResult
{
    None,
    Success,
    SuccessIncompatibleManual,
    SuccessIncompatibleReflection,
    IncompatibleForm,
    NoTransferRoute,
    IncompatibleSpecies,
    IncompatibleLanguageGB,
}
