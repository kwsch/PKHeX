using System;
using System.Collections.Generic;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BattleVideo3(Memory<byte> Raw) : IBattleVideo
{
    public BattleVideo3() : this(new byte[SIZE]) { }

    public Span<byte> Data => Raw.Span;
    internal const int SIZE = SAV3.SIZE_SECTOR_USED - 4; // Skip sentinel
    public byte Generation => 3;

    public IEnumerable<PKM> Contents => PlayerTeams.SelectMany(z => z);

    public static bool IsValid(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE)
            return false;
        var chkSpan = data[^4..];
        var chk = ReadUInt32LittleEndian(chkSpan);
        if (chk > 0xF7080)
            return false; // max if all are FF

        var chkRegion = data[..^4];
        var expect = GetByteSum(chkRegion);
        return chk == expect;
    }

    public IReadOnlyList<PK3[]> PlayerTeams
    {
        get => [GetTeam(0), GetTeam(1)];
        set
        {
            SetTeam(value[0], 0);
            SetTeam(value[1], 1);
        }
    }

    public PK3[] GetTeam(int teamIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)teamIndex, 2);

        var ofs = 6 * PokeCrypto.SIZE_3PARTY * teamIndex;
        var team = new PK3[6];
        for (int p = 0; p < 6; p++)
        {
            int offset = ofs + (PokeCrypto.SIZE_3PARTY * p);
            var span = Data.Slice(offset, PokeCrypto.SIZE_3PARTY);
            team[p] = new PK3(span.ToArray());
        }

        return team;
    }

    public void SetTeam(IReadOnlyList<PK3> team, int teamIndex)
    {
        var ofs = 6 * PokeCrypto.SIZE_3PARTY * teamIndex;
        for (int p = 0; p < 6; p++)
        {
            int offset = ofs + (PokeCrypto.SIZE_3PARTY * p);
            team[p].EncryptedPartyData.CopyTo(Data[offset..]);
        }
    }

    // 0x4B0 - string3[4][8] Trainer Names
    // 0x4D0 - u8[4] Trainer Genders
    // 0x4D4 - u32[4] Trainer IDs
    // 0x4E4 - u8[4] Trainer Languages

    public uint Seed
    {
        get => ReadUInt32LittleEndian(Data[0x4E8..]);
        set => WriteUInt32LittleEndian(Data[0x4E8..], value);
    }

    public uint Mode
    {
        get => ReadUInt32LittleEndian(Data[0x4EC..]);
        set => WriteUInt32LittleEndian(Data[0x4EC..], value);
    }

    // ...

    public uint Checksum
    {
        get => ReadUInt32LittleEndian(Data[^4..]);
        set => WriteUInt32LittleEndian(Data[^4..], value);
    }

    public bool IsChecksumValid() => Checksum == GetByteSum(Data[..^4]);

    public static uint GetByteSum(ReadOnlySpan<byte> data)
    {
        uint result = 0;
        foreach (byte b in data)
            result += b;
        return result;
    }
}
