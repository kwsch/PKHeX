namespace PKHeX.Core;

/// <summary>
/// Generation 2 met data interface for details introduced by <see cref="GameVersion.C"/>
/// </summary>
public interface ICaughtData2
{
    ushort CaughtData { get; set; }
    int MetTimeOfDay { get; set; }
    byte MetLevel { get; set; }
    byte OriginalTrainerGender { get; set; }
    ushort MetLocation { get; set; }
}
