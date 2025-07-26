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
    protected CheckResult GetValid(LegalityCheckResultCode msg) => Get(Severity.Valid, msg);
    protected CheckResult GetInvalid(LegalityCheckResultCode msg) => Get(Severity.Invalid, msg);
    protected CheckResult Get(Severity s, LegalityCheckResultCode msg) => CheckResult.Get(s, Identifier, msg);

    protected CheckResult GetValid(LegalityCheckResultCode msg, uint argument) => Get(Severity.Valid, msg, argument);
    protected CheckResult GetInvalid(LegalityCheckResultCode msg, uint argument) => Get(Severity.Invalid, msg, argument);
    protected CheckResult GetInvalid(LegalityCheckResultCode msg, ushort arg0, ushort arg1) => GetInvalid(Identifier, msg, arg0, arg1);
    protected CheckResult Get(Severity s, LegalityCheckResultCode msg, uint argument) => CheckResult.Get(s, Identifier, msg, argument);

    protected static CheckResult GetValid(CheckIdentifier c, LegalityCheckResultCode msg, uint argument = 0) => Get(c, Severity.Valid, msg, argument);
    protected static CheckResult Get(CheckIdentifier c, Severity s, LegalityCheckResultCode msg, uint argument = 0) => CheckResult.Get(s, c, msg, argument);

    protected static CheckResult GetInvalid(CheckIdentifier c, LegalityCheckResultCode msg, uint value = 0) => CheckResult.Get(Severity.Invalid, c, msg, value);
    protected static CheckResult GetInvalid(CheckIdentifier c, LegalityCheckResultCode msg, ushort arg0, ushort arg1 = 0) => GetInvalid(c, msg, arg0 | (uint)arg1 << 16);
}
