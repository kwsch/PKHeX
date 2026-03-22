using System;

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

public interface IRecordStatStorage<in TType, TValue> where TType : Enum where TValue : struct
{
    TValue GetRecord(TType recordID);
    void SetRecord(TType recordID, TValue value);
    void AddRecord(TType recordID, TValue count = default);
}
