namespace PKHeX.Core;

public interface IEventVar<T> : IEventFlag, IEventWork<T>
{
    T GetWork(EventVarType type, int index);
    void SetWork(EventVarType type, int index, T value);

    bool GetFlag(EventVarType type, int index);
    void SetFlag(EventVarType type, int index, bool value = true);

    EventVarType GetFlagType(int index, out int subIndex);
    EventVarType GetWorkType(int index, out int subIndex);
    int GetFlagRawIndex(EventVarType type, int index);
    int GetWorkRawIndex(EventVarType type, int index);
}

public interface IEventFlag
{
    bool GetFlag(int index);
    void SetFlag(int index, bool value = true);
    int CountFlag { get; }
}

public interface ISystemFlag
{
    bool GetSystemFlag(int index);
    void SetSystemFlag(int index, bool value = true);
    int CountSystem { get; }
}

public interface IEventWork<T>
{
    T GetWork(int index);
    void SetWork(int index, T value);
    int CountWork { get; }
}
