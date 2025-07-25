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

    // Standard methods for creating CheckResults
    protected CheckResult GetInvalid(LegalityCheckResultCode msg) => Get(msg, Severity.Invalid);
    protected CheckResult GetValid(LegalityCheckResultCode msg) => Get(msg, Severity.Valid);
    protected CheckResult Get(LegalityCheckResultCode msg, Severity s) => new(s, Identifier, msg);

    // Methods for creating CheckResults with an argument
    protected CheckResult GetInvalid(LegalityCheckResultCode msg, ushort argument) => Get(msg, Severity.Invalid, argument);
    protected CheckResult GetValid(LegalityCheckResultCode msg, ushort argument) => Get(msg, Severity.Valid, argument);
    protected CheckResult Get(LegalityCheckResultCode msg, Severity s, ushort argument) => new(s, Identifier, msg, argument);

    // Static methods for creating CheckResults with a specific identifier and argument
    protected static CheckResult GetInvalid(LegalityCheckResultCode msg, CheckIdentifier c, ushort argument = 0) => Get(msg, Severity.Invalid, c, argument);
    protected static CheckResult GetInvalid(LegalityCheckResultCode msg, CheckIdentifier c, byte arg0, byte arg1 = 0) => Get(msg, Severity.Invalid, c, (ushort)(arg0 | (arg1 << 8)));
    protected static CheckResult GetValid(LegalityCheckResultCode msg, CheckIdentifier c, ushort argument = 0) => Get(msg, Severity.Valid, c, argument);
    protected static CheckResult Get(LegalityCheckResultCode msg, Severity s, CheckIdentifier c, ushort argument = 0) => new(s, c, msg, argument);
}
