namespace PKHeX.Core
{
    /// <summary>
    /// Properties common to RS &amp; Emerald save files.
    /// </summary>
    public interface IGen3Hoenn
    {
        RTC3 ClockInitial { get; set; }
        RTC3 ClockElapsed { get; set; }
        PokeBlock3Case PokeBlocks { get; set; }
        DecorationInventory3 Decorations { get; set; }
        bool HasReceivedWishmkrJirachi { get; set; }
    }
}
