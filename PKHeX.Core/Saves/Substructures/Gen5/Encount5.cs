using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class Encount5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw)
{
    public ushort LastLocation { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort CaptureUnknown { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }

    public Roamer5 Roamer1 => new(Raw.Slice(4, Roamer5.SIZE));
    public Roamer5 Roamer2 => new(Raw.Slice(4 + Roamer5.SIZE, Roamer5.SIZE));

    public abstract byte SwarmSeed { get; set; }
    public abstract uint SwarmMaxCountModulo { get; }

    public uint SwarmIndex
    {
        get => SwarmSeed % SwarmMaxCountModulo;
        set => SwarmSeed = (byte)(value % SwarmMaxCountModulo);
    }
}

public sealed class Encount5BW(SAV5BW SAV, Memory<byte> raw) : Encount5(SAV, raw)
{
    public override byte SwarmSeed { get => Data[0x30]; set => Data[0x30] = value; }
    public override uint SwarmMaxCountModulo => 17;
}

public sealed class Encount5B2W2(SAV5B2W2 SAV, Memory<byte> raw) : Encount5(SAV, raw)
{
    public override byte SwarmSeed { get => Data[0x2C]; set => Data[0x2C] = value; }
    public override uint SwarmMaxCountModulo => 19;
}
