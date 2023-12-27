namespace PKHeX.Core;

/// <summary>
/// Verification that provides new <see cref="CheckResult"/> values for a <see cref="LegalityAnalysis"/>.
/// </summary>
public abstract class Verifier
{
    /// <summary>
    /// <see cref="CheckResult"/> category.
    /// </summary>
    protected abstract CheckIdentifier Identifier { get; }

    /// <summary>
    /// Processes the <see cref="data"/> and adds any relevant <see cref="CheckResult"/> values to the <see cref="LegalityAnalysis.Parse"/>.
    /// </summary>
    /// <param name="data">Analysis data to process</param>
    public abstract void Verify(LegalityAnalysis data);

    protected CheckResult GetInvalid(string msg) => Get(msg, Severity.Invalid);
    protected CheckResult GetValid(string msg) => Get(msg, Severity.Valid);
    protected CheckResult Get(string msg, Severity s) => new(s, Identifier, msg);

    protected static CheckResult GetInvalid(string msg, CheckIdentifier c) => Get(msg, Severity.Invalid, c);
    protected static CheckResult GetValid(string msg, CheckIdentifier c) => Get(msg, Severity.Valid, c);
    protected static CheckResult Get(string msg, Severity s, CheckIdentifier c) => new(s, c, msg);
}
