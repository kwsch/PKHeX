namespace PKHeX.Core.Bulk;

/// <summary>
/// Represents the result of a bulk check operation, including the outcome and an associated comment.
/// </summary>
/// <param name="Result">The outcome of the check operation, indicating success or failure.</param>
/// <param name="Comment">A descriptive comment providing additional context or details about the check result.</param>
/// <remarks>
/// Due to the complexity of bulk check results, and the infrequent nature of bulk checks, we permit the use of a comment field.
/// It'll store a generated string that describes the result of the check, which can be used for debugging or user feedback.
/// Is it something able to be localized? Anything is possible, but we only will generate identifier-like comments that aren't really localizable besides formatting chars.
/// </remarks>
public readonly record struct BulkCheckResult(CheckResult Result, string Comment, int Index1, int Index2 = BulkCheckResult.NoIndex)
{
    public const int NoIndex = -1;
}
