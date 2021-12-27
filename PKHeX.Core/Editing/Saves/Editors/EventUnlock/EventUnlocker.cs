namespace PKHeX.Core;

public abstract class EventUnlocker<T> where T : SaveFile
{
    protected T SAV { get; }
    protected EventUnlocker(T sav) => SAV = sav;
}
