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

    public static partial class Extensions
    {
        public static void ApplyToPKM(this ITrainerInfo info, PKM pk)
        {
            pk.OT_Name = info.OT;
            pk.TID = info.TID;
            pk.SID = info.SID;
            pk.OT_Gender = info.Gender;
            pk.Language = info.Language;
            pk.Version = info.Game;

            pk.Country = info.Country;
            pk.Region = info.SubRegion;
            pk.ConsoleRegion = info.ConsoleRegion;
        }
    }
}