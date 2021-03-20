namespace PKHeX.Core
{
    public interface ILegalityFormatter
    {
        string GetReport(LegalityAnalysis l);
        string GetReportVerbose(LegalityAnalysis l);
    }
}
