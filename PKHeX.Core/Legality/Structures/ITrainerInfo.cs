namespace PKHeX.Core
{
    /// <summary>
    /// Minimal Trainer Information necessary for generating a <see cref="PKM"/>.
    /// </summary>
    public interface ITrainerInfo
    {
        string OT { get; }
        ushort TID { get; }
        ushort SID { get; }
        int Gender { get; }
        int Game { get; }
        int Language { get; }

        int Country { get; }
        int SubRegion { get; }
        int ConsoleRegion { get; }

        int Generation { get; }
    }
}