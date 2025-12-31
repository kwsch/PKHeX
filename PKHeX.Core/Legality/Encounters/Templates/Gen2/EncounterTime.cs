using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 Time of Encounter enum
/// </summary>
[Flags]
public enum EncounterTime : byte
{
    Any = 0,
    Morning = 1 << 1,
    Day = 1 << 2,
    Night = 1 << 3,
}

public interface IEncounterTime
{
    EncounterTime EncounterTime { get; }
    public int GetRandomTime();
}

public static class EncounterTimeExtension
{
    extension(EncounterTime t1)
    {
        internal bool Contains(int t2) => t1 == EncounterTime.Any || (t1 & (EncounterTime)(1 << t2)) != 0;

        internal int RandomValidTime()
        {
            var rnd = Util.Rand;
            int val = rnd.Next(1, 4);
            if (t1 == EncounterTime.Any)
                return val;
            while (!t1.Contains(val))
                val = rnd.Next(1, 4);
            return val;
        }
    }
}
