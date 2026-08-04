namespace PKHeX.Core.AutoMod;

/// <summary>
/// Result codes for legalization and import operations.
/// </summary>
public enum AutoModErrorCode
{
    None,
    NoSingleImport,

    /// <summary>
    /// Don't use this!
    /// </summary>
    CODE_SILENT,

    NotEnoughSpace,
    InvalidLines,
    VersionMismatch,
}

public static class AutoModErrorCodeExtensions
{
    public static string GetMessage(this AutoModErrorCode code) => code switch
    {
        <= AutoModErrorCode.CODE_SILENT => string.Empty,
        AutoModErrorCode.NotEnoughSpace => "Not enough space in the box.",
        AutoModErrorCode.InvalidLines => "Invalid lines detected.",
        _ => string.Empty,
    };
}
