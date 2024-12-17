using System;
using System.Text;

namespace PKHeX.Core;

public ref struct HallOfFameEntity1(Span<byte> data, bool Japanese)
{
    public const int SIZE = 0x10;
    public Span<byte> Data { get; } = data;

    public byte SpeciesInternal { readonly get => Data[0]; set => Data[0] = value; } // raw access
    public byte Level { readonly get => Data[1]; set => Data[1] = value; }
    public readonly Span<byte> NicknameTrash => Data.Slice(2, StringLength(Japanese) + 1);

    private static int StringLength(bool jp) => jp ? 5 : 10;

    public ushort Species
    {
        readonly get => SpeciesConverter.GetNational1(SpeciesInternal);
        set => SpeciesInternal = SpeciesConverter.GetInternal1(value);
    }

    public string Nickname
    {
        readonly get => GetString(NicknameTrash);
        set => SetString(NicknameTrash, value, Japanese ? 5 : 10, StringConverterOption.Clear50);
    }

    public readonly bool IsEmpty => SpeciesInternal == PokeList1.SlotEmpty;

    public void Clear()
    {
        Data.Clear();
        NicknameTrash.Fill(StringConverter1.TerminatorCode);
        SpeciesInternal = PokeList1.SlotEmpty;
    }

    public void Register(PK1 pk)
    {
        SpeciesInternal = pk.SpeciesInternal;
        Level = pk.CurrentLevel;
        pk.NicknameTrash.CopyTo(NicknameTrash);
    }

    private readonly string GetString(ReadOnlySpan<byte> data)
        => StringConverter1.GetString(data, Japanese);
    private readonly int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter1.SetString(destBuffer, value, maxLength, Japanese, option);
}

public sealed class HallOfFameReader1(Memory<byte> Raw, bool Japanese)
{
    private Span<byte> Data => Raw.Span;

    public const int TeamCount = 50;
    public const int SlotsPerTeam = 6;
    private const int SizePerTeam = SlotsPerTeam * HallOfFameEntity1.SIZE;
    public const int SIZE = SizePerTeam * TeamCount;

    private static int GetOffset(int team, int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)team, TeamCount, nameof(team));
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, SlotsPerTeam, nameof(slot));
        return (team * SizePerTeam) + (slot * HallOfFameEntity1.SIZE);
    }

    private Span<byte> GetSlot(int team, int slot) => Data.Slice(GetOffset(team, slot), HallOfFameEntity1.SIZE);
    public HallOfFameEntity1 GetEntity(int team, int slot) => new(GetSlot(team, slot), Japanese);

    public int GetTeamMemberCount(int team)
    {
        for (int i = 0; i < SlotsPerTeam; i++)
        {
            var entity = GetEntity(team, i);
            if (entity.IsEmpty)
                return i;
        }
        return SlotsPerTeam;
    }

    public void Clear()
    {
        for (int i = 0; i < TeamCount; i++)
        {
            for (int j = 0; j < SlotsPerTeam; j++)
                GetEntity(i, j).Clear();
        }
    }

    public void ShiftTeams()
    {
        // Slide all teams up by one slot
        Data[SizePerTeam..SIZE].CopyTo(Data);
        // Logic to register the team will initialize all slots
    }

    public void Delete(int team)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)team, TeamCount, nameof(team));
        int offset = team * SizePerTeam;
        int length = SIZE - offset - SizePerTeam;
        Data.Slice(offset + SizePerTeam, length).CopyTo(Data[offset..]);
        Reset(TeamCount - 1);
    }

    private void Reset(int team)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)team, TeamCount, nameof(team));
        for (int i = 0; i < SlotsPerTeam; i++)
            GetEntity(team, i).Clear();
    }

    public byte RegisterParty(SAV1 sav, byte currentCount)
    {
        var count = currentCount;
        // If the entire list is full, shift all teams up by one slot
        if (count >= TeamCount)
        {
            // Shift teams to make room
            ShiftTeams();
            count = TeamCount - 1;
        }
        RegisterParty(count, sav);
        return (byte)(count + 1);
    }

    private void RegisterParty(int team, SAV1 party)
    {
        for (int i = 0; i < SlotsPerTeam; i++)
        {
            var pk = (PK1)party.GetPartySlotAtIndex(i);
            var entity = GetEntity(team, i);

            bool empty = PokeList1.GetHeaderIdentifierMark(pk) == PokeList1.SlotEmpty;
            if (empty)
                entity.Clear();
            else
                entity.Register(pk);
        }
    }

    public string GetTeamSummary(int team, ReadOnlySpan<string> speciesNames)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < SlotsPerTeam; i++)
        {
            var entity = GetEntity(team, i);
            var speciesName = entity switch
            {
                { IsEmpty: true } => "✕",
                { Species: 0 or > 151 } => $"{entity.SpeciesInternal}",
                _ => speciesNames[entity.Species],
            };
            sb.AppendLine($"{i + 1}: {entity.Nickname} ({speciesName})");
        }
        return sb.ToString();
    }
}
