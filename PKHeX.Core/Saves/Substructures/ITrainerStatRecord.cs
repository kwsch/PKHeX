namespace PKHeX.Core;

/// <summary>
/// Provides an API for fluent record editors.
/// </summary>
public interface ITrainerStatRecord
{
    int GetRecord(int recordID);
    int GetRecordOffset(int recordID);
    int GetRecordMax(int recordID);
    void SetRecord(int recordID, int value);
    int RecordCount { get; }
}
