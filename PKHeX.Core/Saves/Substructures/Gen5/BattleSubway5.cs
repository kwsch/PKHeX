using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BattleSubway5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public int BP { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, (ushort)value); }
    public int Flags { get => Data[0x04]; set => Data[0x04] = (byte)value; }
    public bool Flag0       { get => ((Flags >> 0) & 1) != 0; set => Flags = (Flags & ~(1 << 0)) | ((value ? 1 : 0) << 0); }
    public bool Flag1       { get => ((Flags >> 1) & 1) != 0; set => Flags = (Flags & ~(1 << 1)) | ((value ? 1 : 0) << 1); }
    public bool Flag2       { get => ((Flags >> 2) & 1) != 0; set => Flags = (Flags & ~(1 << 2)) | ((value ? 1 : 0) << 2); }
    public bool Flag3       { get => ((Flags >> 3) & 1) != 0; set => Flags = (Flags & ~(1 << 3)) | ((value ? 1 : 0) << 3); }
    public bool SuperSingle { get => ((Flags >> 4) & 1) != 0; set => Flags = (Flags & ~(1 << 4)) | ((value ? 1 : 0) << 4); }
    public bool SuperDouble { get => ((Flags >> 5) & 1) != 0; set => Flags = (Flags & ~(1 << 5)) | ((value ? 1 : 0) << 5); }
    public bool SuperMulti  { get => ((Flags >> 6) & 1) != 0; set => Flags = (Flags & ~(1 << 6)) | ((value ? 1 : 0) << 6); }
    public bool Flag7       { get => ((Flags >> 7) & 1) != 0; set => Flags = (Flags & ~(1 << 7)) | ((value ? 1 : 0) << 7); }

    public int SinglePast              { get => ReadUInt16LittleEndian(Data[0x08..]); set => WriteUInt16LittleEndian(Data[0x08..], (ushort)value); }
    public int DoublePast              { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], (ushort)value); }
    public int MultiNPCPast            { get => ReadUInt16LittleEndian(Data[0x0C..]); set => WriteUInt16LittleEndian(Data[0x0C..], (ushort)value); }
    public int MultiFriendsPast        { get => ReadUInt16LittleEndian(Data[0x0E..]); set => WriteUInt16LittleEndian(Data[0x0E..], (ushort)value); }
    public int SuperSinglePast         { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], (ushort)value); }
    public int SuperDoublePast         { get => ReadUInt16LittleEndian(Data[0x14..]); set => WriteUInt16LittleEndian(Data[0x14..], (ushort)value); }
    public int SuperMultiNPCPast       { get => ReadUInt16LittleEndian(Data[0x16..]); set => WriteUInt16LittleEndian(Data[0x16..], (ushort)value); }
    public int SuperMultiFriendsPast   { get => ReadUInt16LittleEndian(Data[0x18..]); set => WriteUInt16LittleEndian(Data[0x18..], (ushort)value); }
    public int SingleRecord            { get => ReadUInt16LittleEndian(Data[0x1A..]); set => WriteUInt16LittleEndian(Data[0x1A..], (ushort)value); }
    public int DoubleRecord            { get => ReadUInt16LittleEndian(Data[0x1C..]); set => WriteUInt16LittleEndian(Data[0x1C..], (ushort)value); }
    public int MultiNPCRecord          { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], (ushort)value); }
    public int MultiFriendsRecord      { get => ReadUInt16LittleEndian(Data[0x20..]); set => WriteUInt16LittleEndian(Data[0x20..], (ushort)value); }
    public int SuperSingleRecord       { get => ReadUInt16LittleEndian(Data[0x24..]); set => WriteUInt16LittleEndian(Data[0x24..], (ushort)value); }
    public int SuperDoubleRecord       { get => ReadUInt16LittleEndian(Data[0x26..]); set => WriteUInt16LittleEndian(Data[0x26..], (ushort)value); }
    public int SuperMultiNPCRecord     { get => ReadUInt16LittleEndian(Data[0x28..]); set => WriteUInt16LittleEndian(Data[0x28..], (ushort)value); }
    public int SuperMultiFriendsRecord { get => ReadUInt16LittleEndian(Data[0x2A..]); set => WriteUInt16LittleEndian(Data[0x2A..], (ushort)value); }
}
