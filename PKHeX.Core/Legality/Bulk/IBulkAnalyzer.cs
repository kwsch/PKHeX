namespace PKHeX.Core.Bulk;

/// <summary>
/// Defines a contract for performing batch legality checks or analyses on a <see cref="BulkAnalysis"/> instance.
/// Implementations analyze collections of Pokémon data for specific legality or duplication issues.
/// </summary>
public interface IBulkAnalyzer
{
    /// <summary>
    /// Performs a specific batch analysis or legality check on the provided <see cref="BulkAnalysis"/> input.
    /// </summary>
    /// <param name="input">The <see cref="BulkAnalysis"/> object containing all data and analysis results to process.</param>
    void Analyze(BulkAnalysis input);
}
