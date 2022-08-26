using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SecretBase3Team
{
    private const int O_PID = 0;
    private const int O_Moves = 0x18;
    private const int O_Species = 0x24;
    private const int O_Item = 0x30;
    private const int O_Level = 0x3C;
    private const int O_EV = 0x42;

    private static int GetOffsetPID(int index) => O_PID + (index * 4);
    private static int GetOffsetMove(int index, int moveIndex) => O_Moves + (index * 8) + (moveIndex * 2);
    private static int GetOffsetSpecies(int index) => O_Species + (index * 2);
    private static int GetOffsetItem(int index) => O_Item + (index * 2);

    public readonly SecretBase3PKM[] Team;
    private readonly byte[] Data;

    public SecretBase3Team(byte[] data)
    {
        Team = new SecretBase3PKM[6];
        for (int i = 0; i < Team.Length; i++)
            Team[i] = GetPKM(i);
        Data = data;
    }

    public byte[] Write()
    {
        for (int i = 0; i < Team.Length; i++)
            SetPKM(i);
        return Data;
    }

    private SecretBase3PKM GetPKM(int index) => new()
    {
        PID      = ReadUInt32LittleEndian(Data.AsSpan(GetOffsetPID(index))),
        Species  = ReadUInt16LittleEndian(Data.AsSpan(GetOffsetSpecies(index))),
        HeldItem = ReadUInt16LittleEndian(Data.AsSpan(GetOffsetItem(index))),
        Move1    = ReadUInt16LittleEndian(Data.AsSpan(GetOffsetMove(index, 0))),
        Move2    = ReadUInt16LittleEndian(Data.AsSpan(GetOffsetMove(index, 1))),
        Move3    = ReadUInt16LittleEndian(Data.AsSpan(GetOffsetMove(index, 2))),
        Move4    = ReadUInt16LittleEndian(Data.AsSpan(GetOffsetMove(index, 3))),
        Level    = Data[O_Level + index],
        EVAll    = Data[O_EV + index],
    };

    private void SetPKM(int index)
    {
        var pk = Team[index];
        WriteUInt32LittleEndian(Data.AsSpan(GetOffsetPID(index)), pk.PID);
        WriteUInt16LittleEndian(Data.AsSpan(GetOffsetSpecies(index)), pk.Species);
        WriteUInt16LittleEndian(Data.AsSpan(GetOffsetItem(index)), (ushort)pk.HeldItem);
        WriteUInt16LittleEndian(Data.AsSpan(GetOffsetMove(index, 0)), (ushort)pk.Move1);
        WriteUInt16LittleEndian(Data.AsSpan(GetOffsetMove(index, 1)), (ushort)pk.Move2);
        WriteUInt16LittleEndian(Data.AsSpan(GetOffsetMove(index, 2)), (ushort)pk.Move3);
        WriteUInt16LittleEndian(Data.AsSpan(GetOffsetMove(index, 3)), (ushort)pk.Move4);
        Data[O_Level + index] = (byte)pk.Level;
        Data[O_EV + index] = (byte)pk.EVAll;
    }
}
