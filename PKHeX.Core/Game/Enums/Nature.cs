namespace PKHeX.Core
{
    /// <summary>
    /// Nature ID values for the corresponding English nature name.
    /// </summary>
    public enum Nature : byte
    {
        Hardy = 0,
        Lonely = 1,
        Brave = 2,
        Adamant = 3,
        Naughty = 4,
        Bold = 5,
        Docile = 6,
        Relaxed = 7,
        Impish = 8,
        Lax = 9,
        Timid = 10,
        Hasty = 11,
        Serious = 12,
        Jolly = 13,
        Naive = 14,
        Modest = 15,
        Mild = 16,
        Quiet = 17,
        Bashful = 18,
        Rash = 19,
        Calm = 20,
        Gentle = 21,
        Sassy = 22,
        Careful = 23,
        Quirky = 24,

        Random = 25,
    }

    public static class NatureUtil
    {
        public static Nature GetNature(int value) => value switch
        {
            < 0 or >= (int)Nature.Random => Nature.Random,
            _ => (Nature)value
        };

        public static bool IsFixed(this Nature value) => value is >= 0 and < Nature.Random;

        public static bool IsNeutral(this Nature value) => value.IsFixed() && (byte)value % 6 == 0;
    }
}
