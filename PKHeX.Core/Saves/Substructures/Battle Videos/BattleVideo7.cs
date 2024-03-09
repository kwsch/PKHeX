using System;
using System.Collections.Generic;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BattleVideo7(byte[] data) : IBattleVideo
{
    public const int SIZE = 0x2BC0;
    private const string NPC = "NPC";
    private const int PlayerCount = 4;

    public byte Generation => 7;
    private readonly byte[] Data = (byte[])data.Clone();

    public IEnumerable<PKM> Contents => PlayerTeams.SelectMany(t => t);
    public static bool IsValid(ReadOnlySpan<byte> data) => data.Length == SIZE;

    private static ReadOnlySpan<ushort> TeamOffsets => [0xE41, 0x145E, 0x1A7B, 0x2098];

    public IReadOnlyList<PK7[]> PlayerTeams
    {
        get
        {
            var Teams = new PK7[PlayerCount][];
            for (int t = 0; t < PlayerCount; t++)
                Teams[t] = GetTeam(t);
            return Teams;
        }
        set
        {
            for (int t = 0; t < PlayerCount; t++)
                SetTeam(value[t], t);
        }
    }

    public PK7[] GetTeam(int teamIndex)
    {
        var team = new PK7[6];
        var ofs = TeamOffsets[teamIndex];
        for (int p = 0; p < 6; p++)
        {
            int offset = ofs + (PokeCrypto.SIZE_6PARTY * p);
            var span = Data.AsSpan(offset, PokeCrypto.SIZE_6STORED);
            team[p] = new PK7(span.ToArray());
        }

        return team;
    }

    public void SetTeam(IReadOnlyList<PK7> team, int teamIndex)
    {
        var ofs = TeamOffsets[teamIndex];
        for (int p = 0; p < 6; p++)
        {
            int offset = ofs + (PokeCrypto.SIZE_6PARTY * p);
            team[p].EncryptedPartyData.CopyTo(Data, offset);
        }
    }

    public string[] GetPlayerNames()
    {
        string[] trainers = new string[PlayerCount];
        for (int i = 0; i < PlayerCount; i++)
        {
            var span = Data.AsSpan(0x12C + +(0x1A * i), 0x1A);
            var str = StringConverter7.GetString(span);
            trainers[i] = string.IsNullOrWhiteSpace(trainers[i]) ? NPC : str;
        }
        return trainers;
    }

    public void SetPlayerNames(IReadOnlyList<string> value)
    {
        if (value.Count != PlayerCount)
            return;

        for (int i = 0; i < PlayerCount; i++)
        {
            string tr = value[i] == NPC ? string.Empty : value[i];
            var span = Data.AsSpan(0x12C + +(0x1A * i), 0x1A);
            StringConverter7.SetString(span, tr, 12, 0, StringConverterOption.ClearZero);
        }
    }

    private int MatchYear { get => ReadUInt16LittleEndian(Data.AsSpan(0x2BB0)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2BB0), (ushort)value); }
    private int MatchDay { get => Data[0x2BB3]; set => Data[0x2BB3] = (byte)value; }
    private int MatchMonth { get => Data[0x2BB2]; set => Data[0x2BB2] = (byte)value; }
    private int MatchHour { get => Data[0x2BB4]; set => Data[0x2BB4] = (byte)value; }
    private int MatchMinute { get => Data[0x2BB5]; set => Data[0x2BB5] = (byte)value; }
    private int MatchSecond { get => Data[0x2BB6]; set => Data[0x2BB6] = (byte)value; }

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
                MatchYear = MatchDay = MatchMonth = MatchHour = MatchMinute = MatchSecond = 0;
            }
        }
    }

    public int MusicID { get => Data[0x21C]; set => Data[0x21C] = (byte)value; }
    public bool SilentBGM { get => MusicID == 0xFF; set => MusicID = (byte)(value ? 0xFF : MusicID); }
}
