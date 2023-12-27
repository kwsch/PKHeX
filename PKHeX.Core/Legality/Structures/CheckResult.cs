namespace PKHeX.Core;

/// <summary>
/// Result of a Legality Check
/// </summary>
[System.Diagnostics.DebuggerDisplay($"{{{nameof(Identifier)}}}: {{{nameof(Comment)}}}")]
// ReSharper disable once NotAccessedPositionalProperty.Global
public readonly record struct CheckResult(Severity Judgement, CheckIdentifier Identifier, string Comment)
{
    public bool Valid => Judgement != Severity.Invalid;
    public string Rating => Judgement.Description();

    internal CheckResult(CheckIdentifier i) : this(Severity.Valid, i, LegalityCheckStrings.L_AValid) { }

    public string Format(string format) => string.Format(format, Rating, Comment);
}
