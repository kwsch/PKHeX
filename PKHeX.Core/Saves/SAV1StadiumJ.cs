using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pocket Monsters Stadium
/// </summary>
public sealed class SAV1StadiumJ : SAV_STADIUM
{
    // Required since PK1 logic comparing a save file assumes the save file can be U/J
    public override int SaveRevision => 0;
    public override string SaveRevisionString => "0"; // so we're different from Japanese SAV1Stadium naming...

    public override PersonalTable1 Personal => PersonalTable.Y;
    public override int MaxEV => EffortValues.Max12;
    public override ReadOnlySpan<ushort> HeldItems => [];
    public override GameVersion Version { get => GameVersion.StadiumJ; set { } }

    protected override SAV1StadiumJ CloneInternal() => new((byte[])Data.Clone());

    public override byte Generation => 1;
    public override EntityContext Context => EntityContext.Gen1;
    private const int StringLength = 6; // Japanese Only
    public override int MaxStringLengthTrainer => StringLength;
    public override int MaxStringLengthNickname => StringLength;
    public override int BoxCount => 4; // 8 boxes stored sequentially; latter 4 are backups
    public override int BoxSlotCount => 30;

    public override ushort MaxMoveID => Legal.MaxMoveID_1;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_1;
    public override int MaxAbilityID => Legal.MaxAbilityID_1;
    public override int MaxItemID => Legal.MaxItemID_1;
    private const int SIZE_PK1J = PokeCrypto.SIZE_1STORED + (2 * StringLength); // 0x2D
    protected override int SIZE_STORED => SIZE_PK1J;
    protected override int SIZE_PARTY => SIZE_PK1J;

    public override Type PKMType => typeof(PK1);
    public override PK1 BlankPKM => new(true);

    private const int ListHeaderSize = 0x14;
    private const int ListFooterSize = 6; // POKE + 2byte checksum
    private const uint MAGIC_FOOTER = 0x454B4F50; // POKE

    protected override int TeamCount => 16; // 32 teams stored sequentially; latter 16 are backups
    private const int TeamSizeJ = 0x14 + (SIZE_PK1J * 6) + ListFooterSize; // 0x128
    private const int BoxSizeJ = 0x560;
    private const int BoxStart = 0x2500;

    public SAV1StadiumJ(byte[] data) : base(data, true, GetIsSwap(data))
    {
        Box = BoxStart;
    }

    public SAV1StadiumJ() : base(true, SaveUtil.SIZE_G1STAD)
    {
        Box = BoxStart;
        ClearBoxes();
    }

    protected override bool GetIsBoxChecksumValid(int box)
    {
        var boxOfs = GetBoxOffset(box) - ListHeaderSize;
        const int size = BoxSizeJ - 2;
        var chk = Checksums.CheckSum16(new ReadOnlySpan<byte>(Data, boxOfs, size));
        var actual = ReadUInt16BigEndian(Data.AsSpan(boxOfs + size));
        return chk == actual;
    }

    protected override void SetBoxChecksum(int box)
    {
        var boxOfs = GetBoxOffset(box) - ListHeaderSize;
        const int size = BoxSizeJ - 2;
        var chk = Checksums.CheckSum16(new ReadOnlySpan<byte>(Data, boxOfs, size));
        WriteUInt16BigEndian(Data.AsSpan(boxOfs + size), chk);
    }

    protected override void SetBoxMetadata(int box)
    {
        // Not implemented
    }

    protected override PK1 GetPKM(byte[] data)
    {
        const int len = StringLength;
        var nick = data.AsSpan(0x21, len);
        var ot = data.AsSpan(0x21 + len, len);
        data = data[..0x21];
        var pk1 = new PK1(data, true);
        nick.CopyTo(pk1.NicknameTrash);
        ot.CopyTo(pk1.OriginalTrainerTrash);
        return pk1;
    }

    public override byte[] GetDataForFormatStored(PKM pk)
    {
        byte[] result = new byte[SIZE_STORED];
        var gb = (PK1)pk;

        var data = pk.Data;
        const int len = StringLength;
        data.CopyTo(result, 0);
        gb.NicknameTrash.CopyTo(result.AsSpan(PokeCrypto.SIZE_1STORED));
        gb.OriginalTrainerTrash.CopyTo(result.AsSpan(PokeCrypto.SIZE_1STORED + len));
        return result;
    }

    public override byte[] GetDataForFormatParty(PKM pk) => GetDataForFormatStored(pk);
    public override byte[] GetDataForParty(PKM pk) => GetDataForFormatStored(pk);
    public override byte[] GetDataForBox(PKM pk) => GetDataForFormatStored(pk);

    public override int GetBoxOffset(int box) => Box + ListHeaderSize + (box * BoxSizeJ);
    public static int GetTeamOffset(int team) => 0 + (team * 2 * TeamSizeJ); // backups are after each team

    public string GetTeamName(int team)
    {
        var name = $"Team {team + 1}";

        var ofs = GetTeamOffset(team);
        var str = GetString(Data.AsSpan(ofs + 2, 5));
        if (string.IsNullOrWhiteSpace(str))
            return name;
        var id = ReadUInt16BigEndian(Data.AsSpan(ofs + 8));
        return $"{name} [{id:D5}:{str}]";
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
            members[i] = (PK1)GetStoredSlot(Data.AsSpan(rel));
        }
        return new SlotGroup(name, members);
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
        if (data.Length != SaveUtil.SIZE_G1STADJ)
            return false;
        return GetType(data) != StadiumSaveType.None;
    }

    private static StadiumSaveType GetType(ReadOnlySpan<byte> data)
    {
        var team = StadiumUtil.IsMagicPresentEither(data, TeamSizeJ, MAGIC_FOOTER, 10);
        if (team != StadiumSaveType.None)
            return team;
        var box = StadiumUtil.IsMagicPresentEither(data[BoxStart..], BoxSizeJ, MAGIC_FOOTER, 1);
        if (box != StadiumSaveType.None)
            return box;
        return StadiumSaveType.None;
    }

    private static bool GetIsSwap(ReadOnlySpan<byte> data) => GetType(data) == StadiumSaveType.Swapped;
}
