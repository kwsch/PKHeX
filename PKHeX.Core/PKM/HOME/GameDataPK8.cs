using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PK8"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPK8 : HomeOptional1, IGameDataSide, IGigantamax, IDynamaxLevel, ISociability
{
    private const int SIZE = HomeCrypto.SIZE_1GAME_PK8;
    private const HomeGameDataFormat Format = HomeGameDataFormat.PK8;

    public GameDataPK8() : base(Format, SIZE) { }
    public GameDataPK8(byte[] data, int offset = 0) : base(Format, SIZE, data, offset) { }
    public GameDataPK8 Clone() => new(ToArray(SIZE));
    public int CopyTo(Span<byte> result) => CopyTo(result, SIZE);

    #region Structure

    public bool CanGigantamax { get => Data[Offset + 0x00] != 0; set => Data[Offset + 0x00] = (byte)(value ? 1 : 0); }
    public uint Sociability { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x01)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x01), value); }

    public ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x05)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x05), value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x07)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x07), value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x09)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x09), value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0B)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0B), value); }

    public int Move1_PP { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }
    public int Move2_PP { get => Data[Offset + 0x0E]; set => Data[Offset + 0x0E] = (byte)value; }
    public int Move3_PP { get => Data[Offset + 0x0F]; set => Data[Offset + 0x0F] = (byte)value; }
    public int Move4_PP { get => Data[Offset + 0x10]; set => Data[Offset + 0x10] = (byte)value; }
    public int Move1_PPUps { get => Data[Offset + 0x11]; set => Data[Offset + 0x11] = (byte)value; }
    public int Move2_PPUps { get => Data[Offset + 0x12]; set => Data[Offset + 0x12] = (byte)value; }
    public int Move3_PPUps { get => Data[Offset + 0x13]; set => Data[Offset + 0x13] = (byte)value; }
    public int Move4_PPUps { get => Data[Offset + 0x14]; set => Data[Offset + 0x14] = (byte)value; }

    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x15)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x15), value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x17)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x17), value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x19)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x19), value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1B)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1B), value); }
    public byte DynamaxLevel { get => Data[Offset + 0x1D]; set => Data[Offset + 0x1D] = value; }

    public bool GetPokeJobFlag(int index)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x1E + ofs, index & 7);
    }

    public void SetPokeJobFlag(int index, bool value)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x1E + ofs, index & 7, value);
    }

    public bool GetPokeJobFlagAny() => Array.FindIndex(Data, Offset + 0x1E, 14, static z => z != 0) >= 0;
    public byte Fullness { get => Data[Offset + 0x2C]; set => Data[Offset + 0x2C] = value; }

    public bool GetMoveRecordFlag(int index)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x2D + ofs, index & 7);
    }

    public void SetMoveRecordFlag(int index, bool value)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x2D + ofs, index & 7, value);
    }

    public bool GetMoveRecordFlagAny() => Array.FindIndex(Data, Offset + 0x2D, 14, static z => z != 0) >= 0;

    public int Palma { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x3B)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x3B), value); }
    public int Ball { get => Data[Offset + 0x3F]; set => Data[Offset + 0x3F] = (byte)value; }
    public int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x40)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x40), (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x42)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x42), (ushort)value); }

    #endregion

    #region Conversion

    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.SWSH.GetFormEntry(species, form);

    public void CopyTo(PK8 pk)
    {
        ((IGameDataSide)this).CopyTo(pk);
        pk.CanGigantamax = CanGigantamax;
        pk.Sociability = Sociability;
        pk.DynamaxLevel = DynamaxLevel;
        pk.Fullness = Fullness;
        pk.Palma = Palma;
        Data.AsSpan(Offset + 0xE, 14).CopyTo(pk.Data.AsSpan(0xCE)); // PokeJob
        Data.AsSpan(Offset + 0x2D, 14).CopyTo(pk.Data.AsSpan(0x127)); // Move Record
    }

    public PKM ConvertToPKM(PKH pkh) => ConvertToPK8(pkh);

    public PK8 ConvertToPK8(PKH pkh)
    {
        var pk = new PK8();
        pkh.CopyTo(pk);
        CopyTo(pk);
        return pk;
    }

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPK8? TryCreate(PKH pkh)
    {
        if (pkh.DataPB7 is { } x)
            return GameDataPB7.Create<GameDataPK8>(x);

        if (pkh.DataPB8 is { } b)
        {
            if (pkh.Version is (int)GameVersion.SW or (int)GameVersion.SH && b.Met_Location is not (Locations.HOME_SWLA or Locations.HOME_SWBD or Locations.HOME_SHSP))
                return new GameDataPK8 { Ball = b.Ball, Met_Location = b.Met_Location, Egg_Location = b.Egg_Location is Locations.Default8bNone ? 0 : b.Egg_Location };

            var ball = b.Ball > (int)Core.Ball.Beast ? 4 : b.Ball;
            var ver = pkh.Version;
            var loc = Locations.GetMetSWSH(b.Met_Location, ver);
            return new GameDataPK8 { Ball = ball, Met_Location = loc, Egg_Location = loc != b.Met_Location ? Locations.HOME_SWSHBDSPEgg : b.Egg_Location };
        }
        if (pkh.DataPA8 is { } a)
        {
            if (pkh.Version is (int)GameVersion.SW or (int)GameVersion.SH && a.Met_Location is not (Locations.HOME_SWLA or Locations.HOME_SWBD or Locations.HOME_SHSP))
                return new GameDataPK8 { Ball = a.Ball > (int)Core.Ball.Beast ? 4 : a.Ball, Met_Location = a.Met_Location, Egg_Location = a.Egg_Location is Locations.Default8bNone ? 0 : a.Egg_Location };

            var ball = a.Ball > (int)Core.Ball.Beast ? 4 : a.Ball;
            var ver = pkh.Version;
            var loc = Locations.GetMetSWSH(a.Met_Location, ver);
            return new GameDataPK8 { Ball = ball, Met_Location = loc, Egg_Location = loc != a.Met_Location ? Locations.HOME_SWSHBDSPEgg : a.Egg_Location };
        }

        return null;
    }
}
