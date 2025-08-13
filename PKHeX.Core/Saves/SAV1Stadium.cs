using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokémon Stadium (Pokémon Stadium 2 in Japan)
/// </summary>
public sealed class SAV1Stadium : SAV_STADIUM, IStorageCleanup
{
    public override int SaveRevision => Japanese ? 0 : 1;
    public override string SaveRevisionString => Japanese ? "J" : "U";

    public override PersonalTable1 Personal => PersonalTable.Y;
    public override int MaxEV => EffortValues.Max12;
    public override ReadOnlySpan<ushort> HeldItems => [];
    public override GameVersion Version { get => GameVersion.Stadium; set { } }

    protected override SAV1Stadium CloneInternal() => new(Data.ToArray(), Japanese);

    public override byte Generation => 1;
    public override EntityContext Context => EntityContext.Gen1;
    private int StringLength => Japanese ? StringLengthJ : StringLengthU;
    private const int StringLengthJ = 6;
    private const int StringLengthU = 11;
    public override int MaxStringLengthTrainer => StringLength;
    public override int MaxStringLengthNickname => StringLength;
    public override int BoxCount => Japanese ? 8 : 12;
    public override int BoxSlotCount => Japanese ? 30 : 20;

    public override ushort MaxMoveID => Legal.MaxMoveID_1;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_1;
    public override int MaxAbilityID => Legal.MaxAbilityID_1;
    public override int MaxItemID => Legal.MaxItemID_1;

    public override Type PKMType => typeof(PK1);
    public override PK1 BlankPKM => new(Japanese);
    private const int SIZE_PK1J = PokeCrypto.SIZE_1STORED + (2 * StringLengthJ); // 0x2D
    private const int SIZE_PK1U = PokeCrypto.SIZE_1STORED + (2 * StringLengthU); // 0x37
    protected override int SIZE_STORED => Japanese ? SIZE_PK1J : SIZE_PK1U;
    protected override int SIZE_PARTY => Japanese ? SIZE_PK1J : SIZE_PK1U;

    private int ListHeaderSize => Japanese ? 0x0C : 0x10;
    private const int ListFooterSize = 6; // POKE + 2byte checksum

    private const int TeamCountU = 10;
    private const int TeamCountJ = 12;
    private const int TeamCountTypeU = 9; // team-types 1 & 2 are unused
    private const int TeamCountTypeJ = 9;
    protected override int TeamCount => Japanese ? TeamCountJ * TeamCountTypeJ : TeamCountU * TeamCountTypeU;
    private const int TeamSizeJ = 0x0C + (SIZE_PK1J * 6) + ListFooterSize; // 0x120
    private const int TeamSizeU = 0x10 + (SIZE_PK1U * 6) + ListFooterSize; // 0x160

    private const uint MAGIC_FOOTER = 0x454B4F50; // POKE

    private int BoxSize => Japanese ? BoxSizeJ : BoxSizeU;
    //private int ListHeaderSizeBox => Japanese ? 0x0C : 0x10;
    private const int BoxSizeJ = 0x0C + (SIZE_PK1J * 30) + ListFooterSize; // 0x558
    private const int BoxSizeU = 0x10 + (SIZE_PK1U * 20) + 6 + ListFooterSize; // 0x468 (6 bytes alignment)
    private const int BoxStart = 0xC000;
    public override int GetBoxOffset(int box) => Box + ListHeaderSize + (box * BoxSize);

    public SAV1Stadium(Memory<byte> data) : this(data, IsStadiumJ(data.Span)) { }

    public SAV1Stadium(Memory<byte> data, bool japanese) : base(data, japanese, GetIsSwap(data.Span, japanese))
    {
        Box = BoxStart;
        ConditionBoxes();
    }

    public SAV1Stadium(bool japanese = false) : base(japanese, SaveUtil.SIZE_G1STAD)
    {
        Box = BoxStart;
        ClearBoxes();
    }

    public static bool IsHeaderValid(ReadOnlySpan<byte> header, ReadOnlySpan<byte> footer, bool japanese)
    {
        var h = ReadUInt32BigEndian(header);
        var expect = GetExpectedHeader4(japanese);
        if (h != expect)
            return false;

        var f = ReadUInt32LittleEndian(footer);
        if (f != MAGIC_FOOTER)
            return false;

        return true;
    }

    private static uint GetExpectedHeader4(bool japanese) => japanese ? 0x00_00_50_00u : 0x00_50_00_00u;

    public bool IsUsingBackupBoxSlots { get; private set; }

    /// <summary>
    /// Detect the current box buffer, initialize boxes if not already, and hide (delete) slots that aren't present (ignore ghost slots).
    /// </summary>
    private void ConditionBoxes()
    {
        var blank = BlankPKM;
        for (int i = 0; i < BoxCount; i++)
        {
            // If the box is uninitialized, reset it to the right state.
            var ofs = GetBoxOffset(i);
            var raw = Data.Slice(ofs - ListHeaderSize, BoxSize);
            var header = raw[..ListHeaderSize];
            var footer = raw[^ListFooterSize..];

            if (!IsHeaderValid(header, footer, Japanese))
            {
                if (i == 0 && !IsUsingBackupBoxSlots && IsBackupSegmentValid())
                {
                    IsUsingBackupBoxSlots = true;
                    Box += 0x1_0000;
                    i--; // reset to repeat iteration on backup-Box-0.
                    continue;
                }
                ResetBox(raw, header, footer);
                continue;
            }

            // Wipe empty slots after the count; don't display ghost slots.
            var count = GetBoxSlotCount(ofs);
            if (count >= BoxSlotCount)
                continue; // already full

            // Fill empty slots with blank PKM so that arbitrary reads are correct
            // If you want to see the ghost slots, add your own code to `continue` instead of doing the loop.
            for (int s = count; s < BoxSlotCount; s++)
            {
                var rel = ofs + (s * SIZE_STORED);
                var slice = Data.Slice(rel, SIZE_STORED);
                var species = slice[0];
                if (species == 0) // don't bother converting from internal->national
                    continue; // don't bother wiping already-empty slots.
                WriteBoxSlot(blank, slice);
            }
        }
    }

    private bool IsBackupSegmentValid()
    {
        var ofs = GetBoxOffset(0);
        ofs += 0x1_0000;
        var raw = Data.Slice(ofs - ListHeaderSize, BoxSize);
        var header = raw[..ListHeaderSize];
        var footer = raw[^ListFooterSize..];
        return IsHeaderValid(header, footer, Japanese);
    }

    private void ResetBox(Span<byte> raw, Span<byte> header, Span<byte> footer)
    {
        raw.Clear();
        header.Clear();
        WriteUInt32BigEndian(header, GetExpectedHeader4(Japanese));
        WriteUInt32LittleEndian(footer, MAGIC_FOOTER);
    }

    protected override bool GetIsBoxChecksumValid(int box)
    {
        var boxOfs = GetBoxOffset(box) - ListHeaderSize;
        var size = BoxSize - 2;
        var chk = Checksums.CheckSum16(Data.Slice(boxOfs, size));
        var actual = ReadUInt16BigEndian(Data[(boxOfs + size)..]);
        return chk == actual;
    }

    protected override void SetBoxChecksum(int box)
    {
        var boxOfs = GetBoxOffset(box) - ListHeaderSize;
        var size = BoxSize - 2;
        var chk = Checksums.CheckSum16(Data.Slice(boxOfs, size));
        WriteUInt16BigEndian(Data[(boxOfs + size)..], chk);
    }

    protected override void SetBoxMetadata(int box)
    {
        var bdata = GetBoxOffset(box);

        // Set box count
        int count = 0;
        for (int s = 0; s < BoxSlotCount; s++)
        {
            var rel = bdata + (SIZE_STORED * s);
            if (Data[rel] != 0) // Species present
                count++;
            else
                break; // stop at first empty slot
        }

        // Last byte of header
        SetBoxSlotCount(bdata, count);
    }

    // offset immediately after the header
    private void SetBoxSlotCount(int boxDataStart, int count) => Data[boxDataStart - 1] = (byte)count;
    private byte GetBoxSlotCount(int boxDataStart) => Data[boxDataStart - 1];

    public bool FixStoragePreWrite()
    {
        // Compress the storage.
        bool anyShifted = false;
        // For each box, move present slots to the front.
        for (int i = 0; i < BoxCount; i++)
        {
            int present = 0;
            var ofs = GetBoxOffset(i);
            for (int s = 0; s < BoxSlotCount; s++)
            {
                var rel = ofs + (s * SIZE_STORED);
                var species = Data[rel];
                if (species == 0)
                    continue;
                if (present != s)
                {
                    anyShifted = true;
                    var upSlot = Data[(ofs + (present * SIZE_STORED))..];
                    var src = Data.Slice(rel, SIZE_STORED);
                    src.CopyTo(upSlot);
                    // wipe the old slot
                    src.Clear();
                }
                present++;
            }
        }
        return anyShifted;
    }

    protected override PK1 GetPKM(byte[] data)
    {
        int len = StringLength;
        var nick = data.AsSpan(PokeCrypto.SIZE_1STORED, len);
        var ot = data.AsSpan(PokeCrypto.SIZE_1STORED + len, len);
        var pk1 = new PK1(data[..PokeCrypto.SIZE_1STORED], Japanese);
        nick.CopyTo(pk1.NicknameTrash);
        ot.CopyTo(pk1.OriginalTrainerTrash);
        return pk1;
    }

    public override byte[] GetDataForFormatStored(PKM pk)
    {
        byte[] result = new byte[SIZE_STORED];
        var gb = (PK1)pk;

        var data = pk.Data;
        int len = StringLength;
        data.CopyTo(result);
        gb.NicknameTrash.CopyTo(result.AsSpan(PokeCrypto.SIZE_1STORED));
        gb.OriginalTrainerTrash.CopyTo(result.AsSpan(PokeCrypto.SIZE_1STORED + len));
        return result;
    }

    public override byte[] GetDataForFormatParty(PKM pk) => GetDataForFormatStored(pk);
    public override byte[] GetDataForParty(PKM pk) => GetDataForFormatStored(pk);
    public override byte[] GetDataForBox(PKM pk) => GetDataForFormatStored(pk);

    public int GetTeamOffset(int team) => Japanese ? GetTeamOffsetJ(team) : GetTeamOffsetU(team);

    private int GetTeamOffsetJ(int team)
    {
        if ((uint) team >= TeamCount)
            throw new ArgumentOutOfRangeException(nameof(team));
        return GetTeamTypeOffsetJ(team / TeamCountJ) + (TeamSizeJ * (team % TeamCountJ));
    }

    private int GetTeamOffsetU(int team)
    {
        if ((uint)team >= TeamCount)
            throw new ArgumentOutOfRangeException(nameof(team));
        return GetTeamTypeOffsetU(team / TeamCountU) + (TeamSizeU * (team % TeamCountU));
    }

    private static int GetTeamTypeOffsetJ(int team) => team switch
    {
        0 => 0x0000, // Anything Goes
        1 => 0x0D80, // Nintendo Cup '97
        2 => 0x1B00, // Nintendo Cup '98
        3 => 0x2880, // Nintendo Cup '99

        4 => 0x4000, // Petit Cup
        5 => 0x4D80, // Pika Cup
        6 => 0x5B00, // Prime Cup
        7 => 0x6880, // Gym Leader Castle

        8 => 0x8000, // Vs. Mewtwo
        _ => throw new ArgumentOutOfRangeException(nameof(team)),
    };

    private static int GetTeamTypeOffsetU(int team) => team switch
    {
        0 => 0x0000,
        1 => 0x0DC0, // Unused
        2 => 0x1B80, // Unused
        3 => 0x2940, // Poke Cup

        4 => 0x4000, // Petit Cup
        5 => 0x4DC0, // Pika Cup
        6 => 0x5B80, // Prime Cup
        7 => 0x6940, // Gym Leader Castle

        8 => 0x8000, // Vs. Mewtwo
        _ => throw new ArgumentOutOfRangeException(nameof(team)),
    };

    public int GetTeamOffset(Stadium2TeamType type, int team)
    {
        if (Japanese)
            return GetTeamTypeOffsetJ((int)type) + (TeamSizeJ * team);
        return GetTeamTypeOffsetU((int)type) + (TeamSizeU * team);
    }

    public string GetTeamName(int team)
    {
        if ((uint)team >= TeamCount)
            throw new ArgumentOutOfRangeException(nameof(team));

        var teamsPerType = Japanese ? TeamCountJ : TeamCountU;
        var type = team / teamsPerType;
        var index = team % teamsPerType;
        var name = $"{GetTeamTypeName(type).Replace('_', ' ')} {index + 1}";

        var ofs = GetTeamOffset(team);
        var otOfs = ofs + (Japanese ? 2 : 1);
        var str = GetString(Data.Slice(otOfs, Japanese ? 5 : 7));
        if (string.IsNullOrWhiteSpace(str))
            return name;
        var idOfs = ofs + (Japanese ? 0x8 : 0xC);
        var id = ReadUInt16BigEndian(Data[idOfs..]);
        return $"{name} [{id:D5}:{str}]";
    }

    private string GetTeamTypeName(int type)
    {
        if (Japanese)
            return ((Stadium1TeamType) type).ToString();
        return type switch
        {
            1 => "Unused1",
            2 => "Unused2",
            3 => "Poke_Cup",
            _ => ((Stadium1TeamType)type).ToString(),
        };
    }

    public override SlotGroup[] GetRegisteredTeams()
    {
        var result = base.GetRegisteredTeams();
        if (Japanese)
            return result;

        // Trim out the teams that aren't accessible
        var noUnused = new SlotGroup[result.Length - (2 * TeamCountU)];
        Array.Copy(result, 0, noUnused, 0, TeamCountU);
        Array.Copy(result, 3 * TeamCountU, noUnused, TeamCountU, noUnused.Length - TeamCountU);
        return noUnused;
    }

    public override SlotGroup GetTeam(int team)
    {
        if ((uint)team >= TeamCount)
            throw new ArgumentOutOfRangeException(nameof(team));

        var name = GetTeamName(team);
        var members = new PK1[6];
        var ofs = GetTeamOffset(team);
        for (int i = 0; i < 6; i++)
        {
            var rel = ofs + ListHeaderSize + (i * SIZE_STORED);
            members[i] = (PK1)GetStoredSlot(Data[rel..]);
        }
        return new SlotGroup(name, members, StorageSlotType.Box);
    }

    public override void WriteSlotFormatStored(PKM pk, Span<byte> data)
    {
        // pk that have never been boxed have yet to save the 'current level' for box indication
        // set this value at this time
        ((PK1)pk).Stat_LevelBox = pk.CurrentLevel;
        base.WriteSlotFormatStored(pk, data);
    }

    public override void WriteBoxSlot(PKM pk, Span<byte> data)
    {
        // pk that have never been boxed have yet to save the 'current level' for box indication
        // set this value at this time
        ((PK1)pk).Stat_LevelBox = pk.CurrentLevel;
        base.WriteBoxSlot(pk, data);
    }

    public static bool IsStadium(ReadOnlySpan<byte> data)
    {
        if (data.Length is not (SaveUtil.SIZE_G1STAD or SaveUtil.SIZE_G1STADF))
            return false;
        if (IsStadiumJ(data))
            return true;
        if (IsStadiumU(data))
            return true;
        return false;
    }

    private static bool IsStadiumJ(ReadOnlySpan<byte> data) => IsStadium(data, TeamSizeJ, BoxSizeJ) != StadiumSaveType.None;
    private static bool IsStadiumU(ReadOnlySpan<byte> data) => IsStadium(data, TeamSizeU, BoxSizeU) != StadiumSaveType.None;

    private static bool GetIsSwap(ReadOnlySpan<byte> data, bool japanese)
    {
        var result = japanese ? IsStadium(data, TeamSizeJ, BoxSizeJ) : IsStadium(data, TeamSizeU, BoxSizeU);
        return result == StadiumSaveType.Swapped;
    }

    private static StadiumSaveType IsStadium(ReadOnlySpan<byte> data, [ConstantExpected] int teamSize, [ConstantExpected] int boxSize)
    {
        var isTeam = StadiumUtil.IsMagicPresentEither(data, teamSize, MAGIC_FOOTER, 10);
        if (isTeam != StadiumSaveType.None)
            return isTeam;
        var isBox = StadiumUtil.IsMagicPresentEither(data[BoxStart..], boxSize, MAGIC_FOOTER, 5);
        if (isBox != StadiumSaveType.None)
            return isBox;
        return StadiumSaveType.None;
    }
}

public enum Stadium1TeamType
{
    Anything_Goes = 0,
    Nintendo_Cup97 = 1, // unused in non-JP
    Nintendo_Cup98 = 2, // unused in non-JP
    Nintendo_Cup99 = 3, // Poke Cup in non-JP
    Petit_Cup = 4,
    Pika_Cup = 5,
    Prime_Cup = 6,
    Gym_Leader_Castle = 7,
    Vs_Mewtwo = 8,
}
