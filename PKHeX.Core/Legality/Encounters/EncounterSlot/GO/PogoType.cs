namespace PKHeX.Core
{
    public enum PogoType : byte
    {
        None, // Don't use this.

        Wild,
        Egg,

        /// <summary> Raid Boss, requires Lv. 15 and IV=1 </summary>
        Raid15 = 10,
        /// <summary> Raid Boss, requires Lv. 20 and IV=10 </summary>
        Raid20,

        /// <summary> Field Research, requires Lv. 15 and IV=1 </summary>
        Field15 = 20,
        /// <summary> Field Research, requires Lv. 15 and IV=10 (Mythicals) </summary>
        FieldM,
        /// <summary> Field Research, requires Lv. 15 and IV=10 (Mythicals, Poké Ball only) </summary>
        FieldP,
        /// <summary> Field Research, requires Lv. 20 and IV=10 (GBL Mythicals) </summary>
        Field20,

        /// <summary> Purified, requires Lv. 8 and IV=1 (Premier Ball) </summary>
        Shadow = 30,
        /// <summary> Purified, requires Lv. 8 and IV=1 (No Premier Ball) </summary>
        ShadowPGU,
    }

    public static class PogoTypeExtensions
    {
        public static int GetMinLevel(this PogoType t) => t switch
        {
            PogoType.Raid15 => 15,
            PogoType.Raid20 => 20,
            PogoType.Field15 => 15,
            PogoType.FieldM => 15,
            PogoType.Field20 => 20,
            PogoType.Shadow => 8,
            PogoType.ShadowPGU => 8,
            _ => 1,
        };

        public static int GetMinIV(this PogoType t) => t switch
        {
            PogoType.Wild => 0,
            PogoType.Raid20 => 10,
            PogoType.FieldM => 10,
            PogoType.FieldP => 10,
            PogoType.Field20 => 10,
            _ => 1,
        };

        public static bool IsBallValid(this PogoType t, Ball b)
        {
            var req = t.GetValidBall();
            if (req == Ball.None)
                return (uint)(b - 2) <= 2; // Poke, Great, Ultra
            return b == req;
        }

        public static Ball GetValidBall(this PogoType t) => t switch
        {
            PogoType.Egg => Ball.Poke,
            PogoType.FieldP => Ball.Poke,
            PogoType.Raid15 => Ball.Premier,
            PogoType.Raid20 => Ball.Premier,
            _ => Ball.None, // Poke, Great, Ultra
        };
    }
}
