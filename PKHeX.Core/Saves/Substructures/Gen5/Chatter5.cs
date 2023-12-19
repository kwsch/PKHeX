using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Chatter Recording
/// </summary>
public sealed class Chatter5 : SaveBlock<SAV5>, IChatter
{
    public Chatter5(SAV5 SAV, int offset) : base(SAV) => Offset = offset;

    public bool Initialized
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset)) == 1u;
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset), value ? 1u : 0u);
    }

    public Span<byte> Recording => Data.AsSpan(Offset + sizeof(uint), IChatter.SIZE_PCM);

    public int ConfusionChance => !Initialized ? 0 : (Recording[99] ^ Recording[499] ^ Recording[699]) switch
    {
        < 100 or >= 150 => 10,
        _ => 0,
    };
}
