namespace PKHeX.Core
{
    /// <summary>
    /// Object has Trainer ownership
    /// </summary>
    public interface ITrainerID
    {
        int TID { get; set; }
        int SID { get; set; }
    }

    public static partial class Extensions
    {
        public static int GetTrainerIDFormat(this ITrainerID tr)
        {
            if (tr is PKM p)
            {
                var format = p.GenNumber;
                if ((format < 3 && p.Format >= 7) || format <= 0) // VC or bad gen
                    return 4; // use TID/SID 16bit style
                return format;
            }
            if (tr is SaveFile s)
                return s.Generation;
            return -1;
        }
    }
}
