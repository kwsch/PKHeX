namespace PKHeX.Core
{
    /// <summary>
    /// Minimal Trainer Information necessary for generating a <see cref="PKM"/>.
    /// </summary>
    public interface ITrainerInfo : ITrainerID
    {
        string OT { get; }
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
            pk.SID = pk.Format < 3 || pk.VC ? 0 : info.SID;
            pk.OT_Gender = info.Gender;
            pk.Language = info.Language;
            pk.Version = info.Game;

            pk.Country = info.Country;
            pk.Region = info.SubRegion;
            pk.ConsoleRegion = info.ConsoleRegion;
        }

        public static void ApplyHandlingTrainerInfo(this ITrainerInfo SAV, PKM pk)
        {
            if (pk.Format == SAV.Generation)
                return;

            pk.HT_Name = SAV.OT;
            pk.HT_Gender = SAV.Gender;
            pk.HT_Friendship = pk.OT_Friendship;
            pk.CurrentHandler = 1;

            if (pk.Format == 6)
            {
                pk.Geo1_Country = SAV.Country;
                pk.Geo1_Region = SAV.SubRegion;
            }
        }
    }
}