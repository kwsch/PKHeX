using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BlueberryQuestRecord9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public uint QuestsDoneSolo  { get => ReadUInt32LittleEndian(Data[0x188..]); set => WriteUInt32LittleEndian(Data[0x188..], value); }
    public uint QuestsDoneGroup { get => ReadUInt32LittleEndian(Data[0x18C..]); set => WriteUInt32LittleEndian(Data[0x18C..], value); }
}
