using static PKHeX.Core.TrainerIDFormat;

namespace PKHeX.Core;

/// <summary>
/// Object stores a numerical trainer ID.
/// </summary>
public interface ITrainerID32 : ITrainerID16
{
    /// <summary>
    /// 32-bit Trainer ID (0-4294967295)
    /// </summary>
    uint ID32 { get; set; }

    /// <summary>
    /// 16-bit Secret ID (0-65535)
    /// </summary>
    ushort SID16 { get; set; }
}

public interface ITrainerID16 : ITrainerID
{
    /// <summary>
    /// 16-bit Trainer ID (0-65535)
    /// </summary>
    ushort TID16 { get; set; }
}

public static class ITrainerID32Extensions
{
    /// <summary>
    /// Checks if the <see cref="pid"/> is shiny when owned by the <see cref="ITrainerID32"/>.
    /// </summary>
    /// <param name="tr">Possessing trainer</param>
    /// <param name="pid"><see cref="PKM.PID"/></param>
    /// <param name="generation">Generation of origin.</param>
    /// <returns>True if shiny, false if not.</returns>
    public static bool IsShiny(this ITrainerID32 tr, uint pid, byte generation = 7)
    {
        var xor = GetShinyXor(tr, pid);
        var threshold = (generation >= 7 ? ShinyXorThreshold7 : ShinyXorThreshold36);
        return xor < threshold;
    }

    private const int ShinyXorThreshold36 = 8; // 1:8192
    private const int ShinyXorThreshold7 = 16; // 1:4096

    /// <summary>
    /// Calculates the <see cref="pid"/> and <see cref="ITrainerID32.ID32"/> xor.
    /// </summary>
    public static uint GetShinyXor(this ITrainerID32 tr, uint pid) => (pid >> 16) ^ (pid & 0xFFFF) ^ tr.SID16 ^ tr.TID16;

    public static uint GetTrainerTID7(this ITrainerID32 tr) => tr.ID32 % 1000000;
    public static uint GetTrainerSID7(this ITrainerID32 tr) => tr.ID32 / 1000000;
    public static uint SetTrainerTID7(this ITrainerID32 tr, uint value) => tr.ID32 = ((tr.ID32 / 1000000) * 1000000) + value;
    public static uint SetTrainerSID7(this ITrainerID32 tr, uint value) => tr.ID32 = (value * 1000000) + (tr.ID32 % 1000000);
    public static uint SetTrainerID16(this ITrainerID32 tr, ushort tid16, ushort sid16) => tr.ID32 = ((uint)sid16 << 16) | tid16;
    public static uint SetTrainerID7(this ITrainerID32 tr, uint sid7, uint tid7) => tr.ID32 = (sid7 * 1000000) + tid7;

    public static uint GetDisplayTID(this ITrainerID32 tr) => tr.TrainerIDDisplayFormat switch
    {
        SixDigit => GetTrainerTID7(tr),
        _ => tr.TID16,
    };

    public static uint GetDisplaySID(this ITrainerID32 tr) => tr.TrainerIDDisplayFormat switch
    {
        SixDigit => GetTrainerSID7(tr),
        _ => tr.SID16,
    };

    public static void SetDisplayTID(this ITrainerID32 tr, uint value)
    {
        switch (tr.TrainerIDDisplayFormat)
        {
            case SixDigit: tr.SetTrainerTID7(value); break;
            default: tr.TID16 = (ushort)value; break;
        }
    }

    public static void SetDisplaySID(this ITrainerID32 tr, uint value)
    {
        switch (tr.TrainerIDDisplayFormat)
        {
            case SixDigit: tr.SetTrainerSID7(value); break;
            default: tr.SID16 = (ushort)value; break;
        }
    }

    public static void SetDisplayID(this ITrainerID32 tr, uint tid, uint sid)
    {
        switch (tr.TrainerIDDisplayFormat)
        {
            case SixDigit: tr.SetTrainerID7(sid, tid); break;
            default: tr.SetTrainerID16((ushort)tid, (ushort)sid); break;
        }
    }
}
