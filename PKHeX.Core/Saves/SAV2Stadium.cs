using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokémon Stadium 2 (Pokémon Stadium GS in Japan)
/// </summary>
public sealed class SAV2Stadium : SAV_STADIUM, IBoxDetailName
{
    public override int SaveRevision => Japanese ? 0 : 1;
    public override string SaveRevisionString => Japanese ? "J" : "U";

    public override PersonalTable2 Personal => PersonalTable.C;
    public override int MaxEV => EffortValues.Max12;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_GSC;
    public override GameVersion Version { get => GameVersion.Stadium2; set { } }

    protected override SAV2Stadium CloneInternal() => new((byte[])Data.Clone(), Japanese);

    public override byte Generation => 2;
    public override EntityContext Context => EntityContext.Gen2;
    private const int StringLength = 12;
    public override int MaxStringLengthTrainer => StringLength;
    public override int MaxStringLengthNickname => StringLength;
    public override int BoxCount => Japanese ? 9 : 14;
    public override int BoxSlotCount => Japanese ? 30 : 20;

    public override ushort MaxMoveID => Legal.MaxMoveID_2;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_2;
    public override int MaxAbilityID => Legal.MaxAbilityID_2;
    public override int MaxItemID => Legal.MaxItemID_2;

    public override Type PKMType => typeof(SK2);
    public override SK2 BlankPKM => new(Japanese);
    protected override SK2 GetPKM(byte[] data) => new(data, Japanese);

    private const int SIZE_SK2 = PokeCrypto.SIZE_2STADIUM; // 60
    protected override int SIZE_STORED => SIZE_SK2;
    protected override int SIZE_PARTY => SIZE_SK2;

    private const int ListHeaderSizeTeam = 0x10;
    private const int ListHeaderSizeBox = 0x20;
    private const int ListFooterSize = 6; // POKE + 2byte checksum

    protected override int TeamCount => 60;
    private const int TeamCountType = 10;
    private const int TeamSize = ListHeaderSizeTeam + (SIZE_SK2 * 6) + 2 + ListFooterSize; // 0x180

    private int BoxSize => Japanese ? BoxSizeJ : BoxSizeU;
    private const int BoxSizeJ = ListHeaderSizeBox + (SIZE_SK2 * 30) + 2 + ListFooterSize; // 0x730
    private const int BoxSizeU = ListHeaderSizeBox + (SIZE_SK2 * 20) + 2 + ListFooterSize; // 0x4D8

    // Box 1 is stored separately from the remainder of the boxes.
    private const int BoxStart = 0x5E00; // Box 1
    private const int BoxContinue = 0x8000; // Box 2+

    private const uint MAGIC_FOOTER = 0x30763350; // P3v0

    public SAV2Stadium(byte[] data) : this(data, IsStadiumJ(data)) { }

    public SAV2Stadium(byte[] data, bool japanese) : base(data, japanese, GetIsSwap(data, japanese))
    {
        Box = BoxStart;
    }

    public SAV2Stadium(bool japanese = false) : base(japanese, SaveUtil.SIZE_G2STAD)
    {
        Box = BoxStart;
        ClearBoxes();
    }

    protected override bool GetIsBoxChecksumValid(int box)
    {
        var boxOfs = GetBoxOffset(box) - ListHeaderSizeBox;
        var size = BoxSize - 2;
        var chk = Checksums.CheckSum16(new ReadOnlySpan<byte>(Data, boxOfs, size));
        var actual = ReadUInt16BigEndian(Data.AsSpan(boxOfs + size));
        return chk == actual;
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
        }

        var boxOfs = bdata - ListHeaderSizeBox;
        var slice = Data.AsSpan(boxOfs, ListHeaderSizeBox);
        if (slice[0] == 0)
        {
            slice[0] = 1;
            slice[1] = (byte)count;
            slice[4] = StringConverter2.TerminatorCode;
            for (int i = 0; i < 4; i++)
                slice[0x10 + i] = (byte)(0xF6 + i); // 1234
        }
        else
        {
            slice[1] = (byte)count;
        }
    }

    protected override void SetBoxChecksum(int box)
    {
        var boxOfs = GetBoxOffset(box) - ListHeaderSizeBox;
        var size = BoxSize - 2;
        var chk = Checksums.CheckSum16(new ReadOnlySpan<byte>(Data, boxOfs, size));
        WriteUInt16BigEndian(Data.AsSpan(boxOfs + size), chk);
    }

    public static int GetTeamOffset(Stadium2TeamType type, int team)
    {
        if ((uint)team >= TeamCountType)
            throw new ArgumentOutOfRangeException(nameof(team));

        var index = (TeamCountType * (int)type) + team;
        return GetTeamOffset(index);
    }

    public static int GetTeamOffset(int team)
    {
        if (team < 40)
            return 0 + (team * TeamSize);
        // Teams 41-60 are in a separate chunk
        return 0x4000 + ((team - 40) * TeamSize);
    }

    public string GetTeamName(int team)
    {
        var name = $"{((Stadium2TeamType) (team / TeamCountType)).ToString().Replace('_', ' ')} {(team % 10) + 1}";

        var ofs = GetTeamOffset(team);
        var str = GetString(Data.AsSpan(ofs + 4, 7));
        if (string.IsNullOrWhiteSpace(str))
            return name;
        var id = ReadUInt16BigEndian(Data.AsSpan(ofs + 2));
        return $"{name} [{id:D5}:{str}]";
    }

    public string GetBoxName(int box)
    {
        var ofs = GetBoxOffset(box) - 0x10;
        var boxNameSpan = Data.AsSpan(ofs, 0x10);
        var str = GetString(boxNameSpan);
        if (string.IsNullOrWhiteSpace(str))
            return BoxDetailNameExtensions.GetDefaultBoxName(box);
        return str;
    }

    public void SetBoxName(int box, ReadOnlySpan<char> name)
    {
        if (name.Length > StringLength)
            throw new ArgumentOutOfRangeException(nameof(name), "Box name is too long.");
        var ofs = GetBoxOffset(box) - 0x10;
        var boxNameSpan = Data.AsSpan(ofs, 0x10);
        SetString(boxNameSpan, name, StringLength, StringConverterOption.None);
    }

    public override SlotGroup GetTeam(int team)
    {
        if ((uint)team >= TeamCount)
            throw new ArgumentOutOfRangeException(nameof(team));

        var name = GetTeamName(team);
        var members = new SK2[6];
        var ofs = GetTeamOffset(team);
        for (int i = 0; i < 6; i++)
        {
            var rel = ofs + ListHeaderSizeTeam + (i * SIZE_STORED);
            members[i] = (SK2)GetStoredSlot(Data.AsSpan(rel));
        }
        return new SlotGroup(name, members);
    }

    public override int GetBoxOffset(int box)
    {
        if (box == 0)
            return BoxStart + ListHeaderSizeBox;
        return BoxContinue + ListHeaderSizeBox + ((box - 1) * BoxSize);
    }

    public static bool IsStadium(ReadOnlySpan<byte> data)
    {
        if (data.Length is not (SaveUtil.SIZE_G2STAD or SaveUtil.SIZE_G2STADF))
            return false;
        if (IsStadiumJ(data) || IsStadiumU(data))
            return true;
        return StadiumUtil.IsMagicPresentEither(data, TeamSize, MAGIC_FOOTER, 1) != StadiumSaveType.None;
    }

    // Check Box 1's footer magic.
    private static bool IsStadiumJ(ReadOnlySpan<byte> data) => StadiumUtil.IsMagicPresentAbsolute(data, BoxStart + BoxSizeJ - ListFooterSize, MAGIC_FOOTER) != StadiumSaveType.None;
    private static bool IsStadiumU(ReadOnlySpan<byte> data) => StadiumUtil.IsMagicPresentAbsolute(data, BoxStart + BoxSizeU - ListFooterSize, MAGIC_FOOTER) != StadiumSaveType.None;

    private static bool GetIsSwap(ReadOnlySpan<byte> data, bool japanese)
    {
        var teamSwap = StadiumUtil.IsMagicPresentSwap(data, TeamSize, MAGIC_FOOTER, 1);
        if (teamSwap)
            return true;

        var boxSpan = data[BoxStart..];
        if (japanese)
            return StadiumUtil.IsMagicPresentSwap(boxSpan, BoxSizeJ, MAGIC_FOOTER, 1);
        return StadiumUtil.IsMagicPresentSwap(boxSpan, BoxSizeU, MAGIC_FOOTER, 1);
    }
}

public enum Stadium2TeamType
{
    Anything_Goes = 0,
    Little_Cup = 1,
    Poke_Cup = 2,
    Prime_Cup = 3,
    GymLeader_Castle = 4,
    Vs_Rival = 5,
}
