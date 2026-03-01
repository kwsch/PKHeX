using static PKHeX.Core.TrainerIDFormat;

namespace PKHeX.Core;

/// <summary>
/// Object stores a numerical trainer ID.
/// </summary>
public interface ITrainerID32 : ITrainerID16, ITrainerID32ReadOnly
{
    /// <summary>
    /// 32-bit Trainer ID (0-4294967295)
    /// </summary>
    new uint ID32 { get; set; }

    /// <summary>
    /// 16-bit Secret ID (0-65535)
    /// </summary>
    new ushort SID16 { get; set; }
}

public interface ITrainerID16 : ITrainerID, ITrainerID16ReadOnly
{
    /// <summary>
    /// 16-bit Trainer ID (0-65535)
    /// </summary>
    new ushort TID16 { get; set; }
}

public interface ITrainerID16ReadOnly
{
    /// <summary>
    /// 16-bit Trainer ID (0-65535)
    /// </summary>
    ushort TID16 { get; }
}

public interface ITrainerID32ReadOnly : ITrainerID16ReadOnly
{
    /// <summary>
    /// 32-bit Trainer ID (0-4294967295)
    /// </summary>
    uint ID32 { get; }

    /// <summary>
    /// 16-bit Secret ID (0-65535)
    /// </summary>
    ushort SID16 { get; }
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
        var xor = tr.GetShinyXor(pid);
        var threshold = (generation >= 7 ? ShinyXorThreshold7 : ShinyXorThreshold36);
        return xor < threshold;
    }

    private const int ShinyXorThreshold36 = 8; // 1:8192
    private const int ShinyXorThreshold7 = 16; // 1:4096

    extension(ITrainerID32 tr)
    {
        /// <summary>
        /// Calculates the <see cref="pid"/> and <see cref="ITrainerID32.ID32"/> xor.
        /// </summary>
        public uint GetShinyXor(uint pid) => (pid >> 16) ^ (pid & 0xFFFF) ^ tr.SID16 ^ tr.TID16;

        public uint GetTrainerTID7() => tr.ID32 % 1000000;
        public uint GetTrainerSID7() => tr.ID32 / 1000000;
        public uint SetTrainerTID7(uint value) => tr.ID32 = ((tr.ID32 / 1000000) * 1000000) + value;
        public uint SetTrainerSID7(uint value) => tr.ID32 = (value * 1000000) + (tr.ID32 % 1000000);
        public uint SetTrainerID16(ushort tid16, ushort sid16) => tr.ID32 = ((uint)sid16 << 16) | tid16;
        public uint SetTrainerID7(uint sid7, uint tid7) => tr.ID32 = (sid7 * 1000000) + tid7;

        public uint GetDisplayTID() => tr.TrainerIDDisplayFormat switch
        {
            SixDigit => tr.GetTrainerTID7(),
            _ => tr.TID16,
        };

        public uint GetDisplaySID() => tr.TrainerIDDisplayFormat switch
        {
            SixDigit => tr.GetTrainerSID7(),
            _ => tr.SID16,
        };

        public void SetDisplayTID(uint value)
        {
            switch (tr.TrainerIDDisplayFormat)
            {
                case SixDigit: tr.SetTrainerTID7(value); break;
                default: tr.TID16 = (ushort)value; break;
            }
        }

        public void SetDisplaySID(uint value)
        {
            switch (tr.TrainerIDDisplayFormat)
            {
                case SixDigit: tr.SetTrainerSID7(value); break;
                default: tr.SID16 = (ushort)value; break;
            }
        }

        public void SetDisplayID(uint tid, uint sid)
        {
            switch (tr.TrainerIDDisplayFormat)
            {
                case SixDigit: tr.SetTrainerID7(sid, tid); break;
                default: tr.SetTrainerID16((ushort)tid, (ushort)sid); break;
            }
        }
    }
}
