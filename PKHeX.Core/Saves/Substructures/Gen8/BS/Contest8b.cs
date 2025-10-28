using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Contest photo and Rank Point storage.
/// </summary>
/// <remarks>size 0x720, struct_name CONTEST_DATA</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Contest8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int SIZE_CONTEST_PHOTO = 0x16C;
    public const int PHOTO_MAX = 5;

    private const int OFS_RANK = SIZE_CONTEST_PHOTO * PHOTO_MAX; // 0x71C;

    public uint ContestRankPoint { get => ReadUInt32LittleEndian(Data[OFS_RANK..]); set => WriteUInt32LittleEndian(Data[OFS_RANK..], value); }
}
