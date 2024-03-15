namespace PKHeX.Core;

public abstract class EventUnlocker<T>(T sav)
    where T : SaveFile
{
    protected T SAV { get; } = sav;
}
