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
    /// <summary> All Event Flag values for the savegame </summary>
    public static bool[] GetEventFlags(this IEventFlagArray source)
    {
        var result = new bool[source.EventFlagCount];
        for (int i = 0; i < result.Length; i++)
            result[i] = source.GetEventFlag(i);
        return result;
    }

    /// <summary> All Event Flag values for the savegame </summary>
    public static void SetEventFlags(this IEventFlagArray source, ReadOnlySpan<bool> value)
    {
        if (value.Length != source.EventFlagCount)
            return;
        for (int i = 0; i < value.Length; i++)
            source.SetEventFlag(i, value[i]);
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
    /// <summary> All Event Constant values for the savegame </summary>
    public static T[] GetAllEventWork<T>(this IEventWorkArray<T> source) where T : unmanaged
    {
        var result = new T[source.EventWorkCount];
        for (int i = 0; i < result.Length; i++)
            result[i] = source.GetWork(i);
        return result;
    }

    /// <summary> All Event Constant values for the savegame </summary>
    public static void SetAllEventWork<T>(this IEventWorkArray<T> source, ReadOnlySpan<T> value) where T : unmanaged
    {
        if (value.Length != source.EventWorkCount)
            return;
        for (int i = 0; i < value.Length; i++)
            source.SetWork(i, value[i]);
    }
}
