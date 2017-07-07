namespace PKHeX.Core
{
    public enum EncounterTime
    {
        Any = -1,
        MorningDay = -2,
        Morning = 1,
        Day = 2,
        Night = 3
    }

    public static class EncounterTimeExtension
    {
        public static bool Contains(this EncounterTime t1, int t2) => t1.Contains((EncounterTime)t2);
        private static bool Contains(this EncounterTime t1, EncounterTime t2)
        {
            if (t1 == t2 || t1 == EncounterTime.Any || t2 == EncounterTime.Any)
                return true;

            if (t1 == EncounterTime.MorningDay)
                return t2 == EncounterTime.Morning || t2 == EncounterTime.Day;

            if (t2 == EncounterTime.MorningDay)
                return t1 == EncounterTime.Morning || t1 == EncounterTime.Day;

            return false;
        }
    }
}