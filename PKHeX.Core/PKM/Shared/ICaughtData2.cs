namespace PKHeX.Core
{
    /// <summary>
    /// Generation 2 met data interface for details introduced by <see cref="GameVersion.C"/>
    /// </summary>
    public interface ICaughtData2
    {
        ushort CaughtData { get; set; }
        int Met_TimeOfDay { get; set; }
        int Met_Level { get; set; }
        int OT_Gender { get; set; }
        int Met_Location { get; set; }
    }
}
