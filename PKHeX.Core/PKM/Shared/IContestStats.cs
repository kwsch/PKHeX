namespace PKHeX.Core
{
    public interface IContestStats
    {
        int CNT_Cool { get; set; }
        int CNT_Beauty { get; set; }
        int CNT_Cute { get; set; }
        int CNT_Smart { get; set; }
        int CNT_Tough { get; set; }
        int CNT_Sheen { get; set; }
    }

    public static partial class Extensions
    {
        public static void SetContestStats(this IContestStats dest, int[] stats)
        {
            if (stats?.Length != 6)
                return;

            dest.CNT_Cool   = stats[0];
            dest.CNT_Beauty = stats[1];
            dest.CNT_Cute   = stats[2];
            dest.CNT_Smart  = stats[3];
            dest.CNT_Tough  = stats[4];
            dest.CNT_Sheen  = stats[5];
        }

        public static bool IsContestBelow(this PKM current, IContestStats initial) => !current.IsContestAboveOrEqual(initial);
        public static bool IsContestAboveOrEqual(this PKM current, IContestStats initial)
        {
            if (current.CNT_Cool   < initial.CNT_Cool)
                return false;
            if (current.CNT_Beauty < initial.CNT_Beauty)
                return false;
            if (current.CNT_Cute   < initial.CNT_Cute)
                return false;
            if (current.CNT_Smart  < initial.CNT_Smart)
                return false;
            if (current.CNT_Tough  < initial.CNT_Tough)
                return false;
            if (current.CNT_Sheen  < initial.CNT_Sheen)
                return false;
            return true;
        }
    }
}
