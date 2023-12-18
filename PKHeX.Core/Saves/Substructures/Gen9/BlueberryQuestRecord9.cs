using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BlueberryQuestRecord9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    private Span<byte> Span => Data.AsSpan();

    public uint QuestsDoneSolo  { get => ReadUInt32LittleEndian(Span[0x188..]); set => WriteUInt32LittleEndian(Span[0x188..], value); }
    public uint QuestsDoneGroup { get => ReadUInt32LittleEndian(Span[0x18C..]); set => WriteUInt32LittleEndian(Span[0x18C..], value); }
}
