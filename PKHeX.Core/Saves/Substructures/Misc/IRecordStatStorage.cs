namespace PKHeX.Core;

/// <summary>
/// Provides a minimal API for mutating stat records.
/// </summary>
public interface IRecordStatStorage
{
    int GetRecord(int recordID);
    void SetRecord(int recordID, int value);
    void AddRecord(int recordID, int count = 1);
}
