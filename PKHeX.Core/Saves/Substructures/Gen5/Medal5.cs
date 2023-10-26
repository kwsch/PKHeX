using System;

namespace PKHeX.Core;

public sealed class Medal5
{

    private const int SIZE = 4;

    private readonly byte[] Data;
    private Span<byte> Span => Data.AsSpan(Offset);
    private readonly int Offset;
    public readonly Epoch2000Value Date;

    public Medal5(byte[] data, int baseOffset, int index)
    {
        Data = data;
        Offset = baseOffset + (SIZE * index);
        Date = new Epoch2000Value(Data.AsMemory(Offset, 2));
    }

    public bool Unread
    {
        get => FlagUtil.GetFlag(Span, 2, 3);
        set => FlagUtil.SetFlag(Span, 2, 3, value);
    }

    public Medal5State State
    {
        get => (Medal5State)(Span[2] & 0b0111);
        set => Span[2] = (byte)((Span[2] & 0b1000) | ((int)value & 0b0111));
    }

    public bool HasDateBytesSet => BitConverter.ToUInt16(Span) != 0;

    public bool CanHaveDate => State == Medal5State.HintMedalObtained || State == Medal5State.MedalObtained || State == Medal5State.CanObtainMedal && HasDateBytesSet;
}

public enum Medal5State
{
    Unobtained = 0,
    CanObtainHintMedal = 1,
    HintMedalObtained = 2,
    CanObtainMedal = 3,
    MedalObtained = 4,
}
