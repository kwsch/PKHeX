namespace PKHeX.Core;

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
        if (tr is PKM pk)
        {
            if (pk.Version is 0)
                return pk.Format;

            var format = pk.Generation;
            if (format < 3 && pk.Format >= 7) // VC or bad gen
                return 4; // use TID/SID 16bit style
            if (format <= 0)
                return 4; // use TID/SID 16bit style
            return format;
        }
        if (tr is SaveFile s)
            return s.Generation;
        return -1;
    }

    public static bool IsShiny(this ITrainerID tr, uint pid, int gen = 7)
    {
        var xor = tr.SID ^ tr.TID ^ (pid >> 16) ^ (pid & 0xFFFF);
        return xor < (gen >= 7 ? 16 : 8);
    }
}
