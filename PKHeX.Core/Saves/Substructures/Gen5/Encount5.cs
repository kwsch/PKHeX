using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class Encount5 : SaveBlock<SAV5>
{
    protected Encount5(SAV5 SAV, int offset) : base(SAV) => Offset = offset;
    public ushort LastLocation { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort CaptureUnknown { get => ReadUInt16LittleEndian(Data.AsSpan()[0x02..]); set => WriteUInt16LittleEndian(Data.AsSpan()[0x02..], value); }

    public Roamer5 Roamer1 => new(Data, Offset + 4);
    public Roamer5 Roamer2 => new(Data, Offset + 4 + Roamer5.SIZE);

    public abstract byte SwarmSeed { get; set; }
    public abstract uint SwarmMaxCountModulo { get; }

    public uint SwarmIndex
    {
        get => SwarmSeed % SwarmMaxCountModulo;
        set => SwarmSeed = (byte)(value % SwarmMaxCountModulo);
    }
}

public sealed class Encount5BW(SAV5BW SAV, int offset) : Encount5(SAV, offset)
{
    public override byte SwarmSeed { get => Data[Offset + 0x30]; set => Data[Offset + 0x30] = value; }
    public override uint SwarmMaxCountModulo => 17;
}

public sealed class Encount5B2W2(SAV5B2W2 SAV, int offset) : Encount5(SAV, offset)
{
    public override byte SwarmSeed { get => Data[Offset + 0x2C]; set => Data[Offset + 0x2C] = value; }
    public override uint SwarmMaxCountModulo => 19;
}
