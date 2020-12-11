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
                var format = p.Generation;
                if ((format < 3 && p.Format >= 7) || format <= 0) // VC or bad gen
                    return 4; // use TID/SID 16bit style
                return format;
            }
            if (tr is SaveFile s)
                return s.Generation;
            return -1;
        }

        public static bool IsShiny(this ITrainerID tr, uint pid, int gen = 7)
        {
            var xor = tr.SID ^ tr.TID ^ (pid >> 16) ^ pid;
            return xor < (gen >= 7 ? 16 : 8);
        }
    }
}
