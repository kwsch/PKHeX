namespace PKHeX.Core;

/// <summary>
/// Result of a Legality Check
/// </summary>
public sealed record CheckResult(Severity Judgement, string Comment, CheckIdentifier Identifier)
{
    public bool Valid => Judgement >= Severity.Fishy;
    public string Rating => Judgement.Description();

    internal CheckResult(CheckIdentifier i) : this(Severity.Valid, LegalityCheckStrings.L_AValid, i) { }

    public override string ToString() => $"{Identifier}: {Comment}";
    public string Format(string format) => string.Format(format, Rating, Comment);
    public string Format(string format, int index) => string.Format(format, Rating, index, Comment);
}
