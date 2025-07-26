using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Result of a Legality Check
/// </summary>
[System.Diagnostics.DebuggerDisplay($"{{{nameof(Identifier)}}}: {{{nameof(Result)}}}")]
[StructLayout(LayoutKind.Explicit, Size = 8)]
public readonly record struct CheckResult
{
    /// <summary> Indicates whether the result is valid. </summary>
    public bool Valid => Judgement != Severity.Invalid;

    /// <summary> Indicates if the result isn't a generic "Valid" result, and might be worth displaying to the user. </summary>
    public bool IsNotGeneric() => Result != LegalityCheckResultCode.Valid;

    /// <summary> Indicates the severity of the result. </summary>
    [field: FieldOffset(0)] public required Severity Judgement { get; init; }

    /// <summary> Identifier for the check group that produced this result. </summary>
    [field: FieldOffset(1)] public required CheckIdentifier Identifier { get; init; }

    /// <summary> Result code for the check, indicating the analysis performed/flagged to arrive at the <see cref="Judgement"/>. </summary>
    [field: FieldOffset(2)] public required LegalityCheckResultCode Result { get; init; }

    #region Hint Parameters used for Human-readable messages
    /// <summary> Raw value used for hints, or storing a 32-bit number hint. </summary>
    [field: FieldOffset(4)] public uint Value { get; init; }
    /// <summary> First argument used for hints, or storing a 16-bit number hint. </summary>
    [field: FieldOffset(4)] public ushort Argument { get; init; }
    /// <summary> Second argument used for hints, or storing a 16-bit number hint. </summary>
    [field: FieldOffset(6)] public ushort Argument2 { get; init; }
    #endregion

    /// <summary>
    /// Simple method to create a valid result with the given identifier.
    /// </summary>
    public static CheckResult GetValid(CheckIdentifier ident) => Get(Severity.Valid, ident, LegalityCheckResultCode.Valid);

    /// <summary>
    /// Simple method to create a result with the given parameters.
    /// </summary>
    public static CheckResult Get(Severity judge, CheckIdentifier ident, LegalityCheckResultCode code, uint value = 0) => new()
    {
        Judgement = judge,
        Identifier = ident,
        Result = code,
        Value = value,
    };
}
