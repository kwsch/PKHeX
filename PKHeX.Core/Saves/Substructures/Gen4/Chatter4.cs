using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Chatter Recording
/// </summary>
public sealed class Chatter4: SaveBlock<SAV4>, IChatter
{
    public Chatter4(SAV4 SAV) : base(SAV) => Offset = SAV.ChatterOffset;

    public bool Initialized
    {
        get => ReadUInt32LittleEndian(SAV.General[Offset..]) == 1u;
        set => WriteUInt32LittleEndian(SAV.General[Offset..], value ? 1u : 0u);
    }

    public Span<byte> Recording => SAV.General.Slice(Offset + sizeof(uint), IChatter.SIZE_PCM);

    public int ConfusionChance => !Initialized ? 1 : (sbyte)Recording[15] switch
    {
        < -30 => 11,
        >= 30 => 31,
        _ => 1,
    };
}
