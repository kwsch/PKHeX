using System;
using System.Collections.Generic;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;
// ReSharper disable UnusedType.Local

namespace PKHeX.Core;

public sealed class BattleVideo6(byte[] Data) : IBattleVideo
{
    private readonly byte[] Data = (byte[])Data.Clone();

    public const int SIZE = 0x2E60;
    private const string NPC = "NPC";
    private const int PlayerCount = 4;

    public IEnumerable<PKM> Contents => PlayerTeams.SelectMany(t => t);
    public byte Generation => 6;

    public static bool IsValid(ReadOnlySpan<byte> data)
    {
        if (data.Length != SIZE)
            return false;
        return ReadUInt64LittleEndian(data[0xE18..]) != 0 && ReadUInt16LittleEndian(data[0xE12..]) == 0;
    }

    public int Mode { get => Data[0x00]; set => Data[0x00] = (byte)value; }
    public int Style { get => Data[0x01]; set => Data[0x01] = (byte)value; }

    public string Debug1
    {
        get => StringConverter6.GetString(Data.AsSpan(0x6, 0x1A));
        set => StringConverter6.SetString(Data.AsSpan(0x6, 0x1A), value, 12, 0, StringConverterOption.ClearZero);
    }

    public string Debug2
    {
        get => StringConverter6.GetString(Data.AsSpan(0x50, 0x1A));
        set => StringConverter6.SetString(Data.AsSpan(0x50, 0x1A), value, 12, 0, StringConverterOption.ClearZero);
    }

    public ulong RNGConst1 { get => ReadUInt64LittleEndian(Data.AsSpan(0x1A0)); set => WriteUInt64LittleEndian(Data.AsSpan(0x1A0), value); }
    public ulong RNGConst2 { get => ReadUInt64LittleEndian(Data.AsSpan(0x1A4)); set => WriteUInt64LittleEndian(Data.AsSpan(0x1A4), value); }
    public ulong RNGSeed1  { get => ReadUInt64LittleEndian(Data.AsSpan(0x1A8)); set => WriteUInt64LittleEndian(Data.AsSpan(0x1A8), value); }
    public ulong RNGSeed2  { get => ReadUInt64LittleEndian(Data.AsSpan(0x1B0)); set => WriteUInt64LittleEndian(Data.AsSpan(0x1B0), value); }

    public int Background { get => ReadInt32LittleEndian(Data.AsSpan(0x1BC)); set => WriteInt32LittleEndian(Data.AsSpan(0x1BC), value); }
    public int Unk1CE { get => ReadUInt16LittleEndian(Data.AsSpan(0x1CE)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1CE), (ushort)value); }
    public int IntroID { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E4)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E4), (ushort)value); }
    public int MusicID { get => ReadUInt16LittleEndian(Data.AsSpan(0x1F0)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1F0), (ushort)value); }

    public string[] GetPlayerNames()
    {
        string[] trainers = new string[PlayerCount];
        for (int i = 0; i < PlayerCount; i++)
        {
            var span = Data.AsSpan(0xEC + (0x1A * i), 0x1A);
            var str = StringConverter6.GetString(span);
            trainers[i] = string.IsNullOrWhiteSpace(str) ? NPC : str;
        }
        return trainers;
    }

    public void SetPlayerNames(IReadOnlyList<string> value)
    {
        if (value.Count != PlayerCount)
            return;

        for (int i = 0; i < PlayerCount; i++)
        {
            var span = Data.AsSpan(0xEC + (0x1A * i), 0x1A);
            string tr = value[i] == NPC ? string.Empty : value[i];
            StringConverter6.SetString(span, tr, 12, 0, StringConverterOption.ClearZero);
        }
    }

    public IReadOnlyList<PK6[]> PlayerTeams
    {
        get
        {
            var Teams = new PK6[PlayerCount][];
            for (int t = 0; t < PlayerCount; t++)
                Teams[t] = GetTeam(t);
            return Teams;
        }
        set
        {
            var Teams = value;
            for (int t = 0; t < PlayerCount; t++)
                SetTeam(Teams[t], t);
        }
    }

    public PK6[] GetTeam(int t)
    {
        var team = new PK6[6];
        const int start = 0xE18;
        for (int p = 0; p < 6; p++)
        {
            int offset = start + (PokeCrypto.SIZE_6PARTY * ((t * 6) + p));
            offset += 8 * (((t * 6) + p) / 6); // 8 bytes padding between teams
            var span = Data.AsSpan(offset, PokeCrypto.SIZE_6PARTY);
            team[p] = new PK6(span.ToArray());
        }

        return team;
    }

    public void SetTeam(IReadOnlyList<PK6> team, int t)
    {
        const int start = 0xE18;
        for (int p = 0; p < 6; p++)
        {
            int offset = start + (PokeCrypto.SIZE_6PARTY * ((t * 6) + p));
            offset += 8 * (((t * 6) + p) / 6); // 8 bytes padding between teams
            team[p].EncryptedPartyData.CopyTo(Data, offset);
        }
    }

    public int MatchYear { get => ReadUInt16LittleEndian(Data.AsSpan(0x2E50)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2E50), (ushort)value); }
    public int MatchDay { get => Data[0x2E52]; set => Data[0x2E52] = (byte)value; }
    public int MatchMonth { get => Data[0x2E53]; set => Data[0x2E53] = (byte)value; }
    public int MatchHour { get => Data[0x2E54]; set => Data[0x2E54] = (byte)value; }
    public int MatchMinute { get => Data[0x2E55]; set => Data[0x2E55] = (byte)value; }
    public int MatchSecond { get => Data[0x2E56]; set => Data[0x2E56] = (byte)value; }
    public int MatchFlags { get => Data[0x2E57]; set => Data[0x2E57] = (byte)value; }

    public int UploadYear { get => ReadUInt16LittleEndian(Data.AsSpan(0x2E58)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2E58), (ushort)value); }
    public int UploadDay { get => Data[0x2E5A]; set => Data[0x2E5A] = (byte)value; }
    public int UploadMonth { get => Data[0x2E5B]; set => Data[0x2E5B] = (byte)value; }
    public int UploadHour { get => Data[0x2E5C]; set => Data[0x2E5C] = (byte)value; }
    public int UploadMinute { get => Data[0x2E5D]; set => Data[0x2E5D] = (byte)value; }
    public int UploadSecond { get => Data[0x2E5E]; set => Data[0x2E5E] = (byte)value; }
    public int UploadFlags { get => Data[0x2E5F]; set => Data[0x2E5F] = (byte)value; }

    public DateTime? MatchStamp
    {
        get
        {
            if (!DateUtil.IsDateValid(MatchYear, MatchMonth, MatchDay))
                return null;
            return new DateTime(MatchYear, MatchMonth, MatchDay, MatchHour, MatchMinute, MatchSecond);
        }
        set
        {
            if (value is { } dt)
            {
                MatchYear = dt.Year;
                MatchDay = dt.Day;
                MatchMonth = dt.Month;
                MatchHour = dt.Hour;
                MatchMinute = dt.Minute;
                MatchSecond = dt.Second;
            }
            else
            {
                MatchYear = MatchDay = MatchMonth = MatchHour = MatchMinute = MatchSecond = MatchFlags = 0;
            }
        }
    }

    public DateTime? UploadStamp
    {
        get
        {
            if (!DateUtil.IsDateValid(UploadYear, UploadMonth, UploadDay))
                return null;
            return new DateTime(UploadYear, UploadMonth, UploadDay, UploadHour, UploadMinute, UploadSecond);
        }
        set
        {
            if (value is { } dt)
            {
                UploadYear = dt.Year;
                UploadDay = dt.Day;
                UploadMonth = dt.Month;
                UploadHour = dt.Hour;
                UploadMinute = dt.Minute;
                UploadSecond = dt.Second;
            }
            else
            {
                UploadYear = UploadDay = UploadMonth = UploadHour = UploadMinute = UploadSecond = UploadFlags = 0;
            }
        }
    }

    private enum TurnAction
    {
        None = 0,
        Fight = 1,
        Unk2 = 2,
        Switch = 3,
        Run = 4,
        Unk5 = 5,
        Rotate = 6,
        Unk7 = 7,
        MegaEvolve = 8,
    }

    private enum TurnTarget
    {
        U0 = 0,
        U1 = 1,
        U2 = 2,
        U3 = 3,
        U4 = 4,
        U5 = 5,
        U6 = 6,
        U7 = 7,
        U8 = 8,
        U9 = 9,
        OppositeEnemy,
        U11 = 11,
        U12 = 12,
        U13 = 13,
        AllExceptUser = 14,
        Everyone = 15,
    }

    private enum TurnRotate
    {
        None,
        Right,
        Left,
        Unk3,
    }

    public enum BVType
    {
        Link = 0,
        Maison = 1,
        SuperMaison = 2,
        BattleSpotFree = 3,
        BattleSpotRating = 4,
        BattleSpotSpecial = 5,
        UNUSED = 6,
        JP1 = 7,
        JP2 = 8,
        BROKEN = 9,
    }

    public enum BVStyle
    {
        Single = 0,
        Double = 1,
        Triple = 2,
        Rotation = 3,
        Multi = 4,
    }
}
