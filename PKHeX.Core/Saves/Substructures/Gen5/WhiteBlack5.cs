using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class WhiteBlack5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public uint Unknown { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
}

public sealed class WhiteBlack5BW(SAV5BW sav, Memory<byte> raw) : WhiteBlack5(sav, raw)
{
    public const int ForestCitySize = 0x1E8;
    public Memory<byte> ForestCity => Raw[..ForestCitySize];
}

public sealed class WhiteBlack5B2W2(SAV5B2W2 sav, Memory<byte> raw) : WhiteBlack5(sav, raw)
{
    public Memory<byte> Fused => Raw.Slice(4, PokeCrypto.SIZE_5PARTY);
}
