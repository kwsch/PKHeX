namespace PKHeX.Core;

/// <summary>
/// Result of a Legality Check
/// </summary>
[System.Diagnostics.DebuggerDisplay($"{{{nameof(Identifier)}}}: {{{nameof(Comment)}}}")]
// ReSharper disable once NotAccessedPositionalProperty.Global
public sealed record CheckResult(Severity Judgement, string Comment, CheckIdentifier Identifier)
{
    public bool Valid => Judgement >= Severity.Fishy;
    public string Rating => Judgement.Description();

    internal CheckResult(CheckIdentifier i) : this(Severity.Valid, LegalityCheckStrings.L_AValid, i) { }

    public string Format(string format) => string.Format(format, Rating, Comment);
}
