using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Chatter Recording
/// </summary>
public sealed class Chatter5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw), IChatter
{
    public bool Initialized
    {
        get => ReadUInt32LittleEndian(Data) == 1u;
        set => WriteUInt32LittleEndian(Data, value ? 1u : 0u);
    }

    public Span<byte> Recording => Data.Slice(sizeof(uint), IChatter.SIZE_PCM);

    public int ConfusionChance => !Initialized ? 0 : (Recording[99] ^ Recording[499] ^ Recording[699]) switch
    {
        < 100 or >= 150 => 10,
        _ => 0,
    };
}
