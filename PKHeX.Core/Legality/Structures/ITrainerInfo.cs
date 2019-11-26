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

            if (pk.Format >= 8 || pk is PB7)
                return;
            pk.Country = info.Country;
            pk.Region = info.SubRegion;
            pk.ConsoleRegion = info.ConsoleRegion;
        }

        public static void ApplyHandlingTrainerInfo(this ITrainerInfo SAV, PKM pk, bool force = false)
        {
            if (pk.Format == SAV.Generation && !force)
                return;

            pk.HT_Name = SAV.OT;
            pk.HT_Gender = SAV.Gender;
            pk.HT_Friendship = pk.OT_Friendship;
            pk.CurrentHandler = 1;

            if (pk.Format == 6)
            {
                var g = (IGeoTrack) pk;
                g.Geo1_Country = SAV.Country;
                g.Geo1_Region = SAV.SubRegion;
                ((PK6)pk).TradeMemory(true);
            }
        }

        public static bool IsFromTrainer(this ITrainerInfo tr, PKM pk)
        {
            if (tr.Game == (int)GameVersion.Any)
                return true;

            if (tr.TID != pk.TID)
                return false;
            if (tr.OT != pk.OT_Name)
                return false;
            if (pk.Format <= 2)
                return false;

            if (tr.SID != pk.SID)
                return false;
            if (pk.Format == 3)
                return false;

            if (tr.Gender != pk.OT_Gender)
                return false;
            if (tr.Game != pk.Version)
                return false;

            return true;
        }
    }
}