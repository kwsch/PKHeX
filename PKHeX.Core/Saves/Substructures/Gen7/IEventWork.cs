namespace PKHeX.Core
{
    public interface IEventWork<T>
    {
        int MaxFlag { get; }
        int MaxWork { get; }

        T GetWork(int index);
        void SetWork(int index, T value);
        T GetWork(EventVarType type, int index);
        void SetWork(EventVarType type, int index, T value);

        bool GetFlag(int index);
        void SetFlag(int index, bool value = true);
        bool GetFlag(EventVarType type, int index);
        void SetFlag(EventVarType type, int index, bool value = true);

        EventVarType GetFlagType(int index, out int subIndex);
        EventVarType GetWorkType(int index, out int subIndex);
        int GetFlagRawIndex(EventVarType type, int index);
        int GetWorkRawIndex(EventVarType type, int index);
    }
}