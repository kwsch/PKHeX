using System;
using System.ComponentModel;

namespace PKHeX.Core;

public sealed class Poffin4(byte[] Data)
{
    public const int SIZE = 8;
    public readonly byte[] Data = Data;

    private const string Stats = nameof(Stats);

    public PoffinFlavor4 Type{ get => (PoffinFlavor4)Data[0]; set => Data[0] = (byte)value; }

    [Category(Stats), Description("Cool Stat Boost")]
    public byte BoostSpicy  { get => Data[1]; set => Data[1] = value; }

    [Category(Stats), Description("Beauty Stat Boost")]
    public byte BoostDry    { get => Data[2]; set => Data[2] = value; }

    [Category(Stats), Description("Cute Stat Boost")]
    public byte BoostSweet  { get => Data[3]; set => Data[3] = value; }

    [Category(Stats), Description("Smart/Clever Stat Boost")]
    public byte BoostBitter { get => Data[4]; set => Data[4] = value; }

    [Category(Stats), Description("Tough Stat Boost")]
    public byte BoostSour   { get => Data[5]; set => Data[5] = value; }

    [Category(Stats), Description("Sheen Stat Boost")]
    public byte Smoothness  { get => Data[6]; set => Data[6] = value; }
    // public byte Unused   { get => Data[7]; set => Data[7] = value; }

    public byte Level => Math.Max(Math.Max(Math.Max(Math.Max(BoostSpicy, BoostDry), BoostSweet), BoostBitter), BoostSour);

    public bool IsManyStat => Type >= PoffinFlavor4.Rich;
    public PoffinFlavor4 StatPrimary => IsManyStat ? PoffinFlavor4.None : (PoffinFlavor4)(((byte) Type / 5) * 6);
    public PoffinFlavor4 StatSecondary => IsManyStat ? PoffinFlavor4.None : (PoffinFlavor4)(((byte)Type % 5) * 6);

    public void SetAll(byte value = 255, PoffinFlavor4 type = PoffinFlavor4.Rich)
    {
        Type = type;
        BoostSpicy = BoostDry = BoostSweet = BoostBitter = BoostSour = Smoothness = value;
    }

    public void SetStat(int stat, byte value)
    {
        if ((uint) stat > 5)
            throw new ArgumentOutOfRangeException(nameof(stat));
        Data[1 + stat] = value;
    }

    public byte GetStat(int stat)
    {
        if ((uint)stat > 5)
            throw new ArgumentOutOfRangeException(nameof(stat));
        return Data[1 + stat];
    }

    public void Delete() => SetAll(0, PoffinFlavor4.None);
}
