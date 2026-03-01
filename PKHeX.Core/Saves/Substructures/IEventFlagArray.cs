using System;

namespace PKHeX.Core;

public interface IEventFlagArray
{
    int EventFlagCount { get; }
    bool GetEventFlag(int flagNumber);
    void SetEventFlag(int flagNumber, bool value);
}

public interface IEventFlag37 : IEventFlagArray, IEventWorkArray<ushort>;

public interface IEventFlagProvider37
{
    IEventFlag37 EventWork { get; }
}

public static class EventFlagArrayExtensions
{
    extension(IEventFlagArray source)
    {
        /// <summary> All Event Flag values for the savegame </summary>
        public bool[] GetEventFlags()
        {
            var result = new bool[source.EventFlagCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = source.GetEventFlag(i);
            return result;
        }

        /// <summary> All Event Flag values for the savegame </summary>
        public void SetEventFlags(ReadOnlySpan<bool> value)
        {
            if (value.Length != source.EventFlagCount)
                return;
            for (int i = 0; i < value.Length; i++)
                source.SetEventFlag(i, value[i]);
        }
    }
}

public interface IEventWorkArray<T> where T : unmanaged
{
    public int EventWorkCount { get; }
    public T GetWork(int index);
    public void SetWork(int index, T value = default);
}

public static class EventWorkArrayExtensions
{
    extension<T>(IEventWorkArray<T> source) where T : unmanaged
    {
        /// <summary> All Event Constant values for the savegame </summary>
        public T[] GetAllEventWork()
        {
            var result = new T[source.EventWorkCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = source.GetWork(i);
            return result;
        }

        /// <summary> All Event Constant values for the savegame </summary>
        public void SetAllEventWork(ReadOnlySpan<T> value)
        {
            if (value.Length != source.EventWorkCount)
                return;
            for (int i = 0; i < value.Length; i++)
                source.SetWork(i, value[i]);
        }
    }
}
