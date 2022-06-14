namespace PKHeX.Core;

/// <summary>
/// Indicates the result of an entity conversion attempt.
/// </summary>
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
