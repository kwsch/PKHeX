namespace PKHeX.Core
{
    /// <summary>
    /// Ball IDs for the corresponding English ball name.
    /// </summary>
    public enum Ball : byte
    {
        None = 0,

        Master = 1,
        Ultra = 2,
        Great = 3,
        Poke = 4,

        Safari = 5,

        Net = 6,
        Dive = 7,
        Nest = 8,
        Repeat = 9,
        Timer = 10,
        Luxury = 11,
        Premier = 12,
        Dusk = 13,
        Heal = 14,
        Quick = 15,

        Cherish = 16,

        Fast = 17,
        Level = 18,
        Lure = 19,
        Heavy = 20,
        Love = 21,
        Friend = 22,
        Moon = 23,

        Sport = 24,
        Dream = 25,
        Beast = 26,
    }

    public static class BallExtensions
    {
        /// <summary>
        /// Checks if the <see cref="ball"/> is an Apricorn Ball (HG/SS)
        /// </summary>
        /// <param name="ball">Ball ID</param>
        /// <returns>True if Apricorn, false if not.</returns>
        public static bool IsApricornBall(this Ball ball) => Ball.Fast <= ball && ball <= Ball.Moon;

        public static Ball GetRequiredBallValueWild(int gen, int loc)
        {
            return gen switch
            {
                // For Gen3 Safari Zones, we've already deferred partial match encounters.
                3 when Locations.IsSafariZoneLocation3(loc) => Ball.Safari,

                // For Gen4 Safari Zones and BCC, we've already deferred partial match encounters.
                4 when Locations.IsSafariZoneLocation4(loc) => Ball.Safari,
                4 when Locations.BugCatchingContest4 == loc => Ball.Sport,

                // Poké Pelago
                7 when loc == 30016 => Ball.Poke,

                _ => Ball.None,
            };
        }
    }
}