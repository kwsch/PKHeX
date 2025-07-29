using System;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class HallFame3Entry(Memory<byte> Raw, bool Japanese)
{
    private const int Count = 6;
    public const int SIZE = Count * HallFame3PKM.SIZE;

    public Span<byte> Data => Raw.Span;

    private static int GetMemberOffset(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Count, nameof(index));
        return index * HallFame3PKM.SIZE;
    }

    private Memory<byte> GetMemberSlice(int index)
        => Raw.Slice(GetMemberOffset(index), HallFame3PKM.SIZE);
    private HallFame3PKM GetMember(int index) => new(GetMemberSlice(index), Japanese);

    public HallFame3PKM[] Team
    {
        get
        {
            var team = new HallFame3PKM[6];
            for (int i = 0; i < Count; i++)
                team[i] = GetMember(i);
            return team;
        }
    }

    private const int MaxEntries = 50;
    private const int MaxLength = MaxEntries * SIZE;

    public static HallFame3Entry[] GetEntries(SAV3 sav)
    {
        byte[] data = sav.GetHallOfFameData();
        Debug.Assert(data.Length >= MaxLength);
        bool jp = sav.Japanese;

        var result = new HallFame3Entry[MaxEntries];
        for (int i = 0; i < result.Length; i++)
            result[i] = new HallFame3Entry(data.AsMemory(i * SIZE), jp);
        return result;
    }

    public static void SetEntries(SAV3 sav, HallFame3Entry[] entries)
    {
        byte[] data = sav.GetHallOfFameData();
        Debug.Assert(data.Length >= MaxLength);

        for (int i = 0; i < entries.Length; i++)
            entries[i].Data.CopyTo(data.AsSpan(i * SIZE));
        sav.SetHallOfFameData(data);
    }
}

public sealed class HallFame3PKM(Memory<byte> Raw, bool Japanese) : ISpeciesForm
{
    public const int SIZE = 0x14;

    public Span<byte> Data => Raw.Span;
    public byte Form => 0; // no forms; derive Unown's from PID else use the Version for Deoxys.

    public uint ID32         { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public ushort TID16      { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort SID16      { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public uint PID          { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }
    private ushort SpecLevel { get => ReadUInt16LittleEndian(Data[8..]); set => WriteUInt16LittleEndian(Data[8..], value); }
    public Span<byte> NicknameTrash => Data.Slice(10, 10);

    public string Nickname
    {
        get => StringConverter3.GetString(NicknameTrash, Japanese);
        set => StringConverter3.SetString(NicknameTrash, value, 10, Japanese, StringConverterOption.ClearZero);
    }

    public int Level
    {
        get => SpecLevel >> 9;
        set => SpecLevel = (ushort)((SpecLevel & 0x1FF) | (Math.Min(100, value) << 9));
    }

    public ushort Species
    {
        get => SpeciesConverter.GetNational3((ushort)(SpecLevel & 0x1FF));
        set => SpecLevel = (ushort)((SpecLevel & 0xFE00) | SpeciesConverter.GetInternal3(value));
    }

    public byte DisplayForm(GameVersion version) => Species switch
    {
        (ushort)Core.Species.Unown => EntityPID.GetUnownForm3(PID),
        (ushort)Core.Species.Deoxys => version switch
        {
            GameVersion.FR => 1, // Attack
            GameVersion.LG => 2, // Defense
            GameVersion.E => 3, // Speed
            _ => 0,
        },
        _ => Form,
    };

    public bool IsShiny => ShinyUtil.GetIsShiny3(ID32, PID);
}
