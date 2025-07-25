namespace PKHeX.Core;

/// <summary>
/// Result of a Legality Check
/// </summary>
[System.Diagnostics.DebuggerDisplay($"{{{nameof(Identifier)}}}: {{{nameof(ResultCode)}}}")]
// ReSharper disable once NotAccessedPositionalProperty.Global
public readonly record struct CheckResult(Severity Judgement, CheckIdentifier Identifier, LegalityCheckResultCode ResultCode, ushort Argument = 0)
{
    public bool Valid => Judgement != Severity.Invalid;

    internal CheckResult(CheckIdentifier i) : this(Severity.Valid, i, LegalityCheckResultCode.Valid) { }

    /// <summary>
    /// Compatibility constructor for transition purposes. Will use a default error code.
    /// </summary>
    public CheckResult(Severity judgement, CheckIdentifier identifier, string _)
        : this(judgement, identifier, LegalityCheckResultCode.Error)
    {
        // This constructor is temporary for transition purposes
    }
}
