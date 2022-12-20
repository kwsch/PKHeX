using static PKHeX.Core.TrainerIDFormat;

namespace PKHeX.Core;

public interface ITrainerID32 : ITrainerID
{
    uint ID32 { get; set; }
    uint TID16 { get => ID32 & 0xFFFF; set => ID32 = (ID32 & 0xFFFF0000) | (value & 0xFFFF); }
    uint SID16 { get => ID32 >> 16;    set => ID32 = (value << 16) | TID16; }

    uint TrainerID7 { get => ID32 % 1000000; set => SetID7(TrainerSID7, value); }
    uint TrainerSID7 { get => ID32 / 1000000; set => SetID7(value, TrainerID7); }

    public uint GetDisplayTID() => TrainerIDDisplayFormat switch
    {
        SixDigit => TrainerID7,
        _ => TID16,
    };

    public uint GetDisplaySID() => TrainerIDDisplayFormat switch
    {
        SixDigit => TrainerSID7,
        _ => SID16,
    };

    public void SetDisplayTID(uint value)
    {
        switch (TrainerIDDisplayFormat)
        {
            case SixDigit: TrainerID7 = value; break;
            default: TID16 = value; break;
        }
    }

    public void SetDisplaySID(uint value)
    {
        switch (TrainerIDDisplayFormat)
        {
            case SixDigit: TrainerSID7 = value; break;
            default: SID16 = value; break;
        }
    }

    public void SetDisplayID(uint tid, uint sid)
    {
        switch (TrainerIDDisplayFormat)
        {
            case SixDigit: SetID7(sid, tid); break;
            default: ID32 = (sid << 16) | tid; break;
        }
    }

    public void SetID7(uint sid, uint tid) => ID32 = (sid * 1000000) + tid;
}

public enum TrainerIDFormat
{
    None,
    SixteenBitSingle,
    SixteenBit,
    SixDigit,
}

/// <summary>
/// Object has Trainer ownership
/// </summary>
public interface ITrainerID
{
    TrainerIDFormat TrainerIDDisplayFormat { get; }
}

public static partial class Extensions
{
    public static TrainerIDFormat GetTrainerIDFormat(this ITrainerID tr) => tr switch
    {
        PKM { Version: 0 } pk => pk.Format     >= 7 ? SixDigit : SixteenBit,
        PKM pk                => pk.Generation >= 7 ? SixDigit : SixteenBit,
        ITrainerInfo sv       => sv.Generation >= 7 ? SixDigit : SixteenBit,
        _ => SixteenBit,
    };

    public static string GetTrainerIDFormatString(this TrainerIDFormat format) => format switch
    {
        SixDigit => "000000",
        _ => "00000",
    };

    public static bool IsShiny(this ITrainerID32 tr, uint pid, int gen = 7)
    {
        var xor = tr.SID16 ^ tr.TID16 ^ (pid >> 16) ^ (pid & 0xFFFF);
        return xor < (gen >= 7 ? 16 : 8);
    }
}
