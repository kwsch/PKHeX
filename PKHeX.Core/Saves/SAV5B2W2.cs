using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object for <see cref="GameVersion.B2W2"/>.
/// </summary>
/// <inheritdoc cref="SAV5" />
public sealed class SAV5B2W2 : SAV5, ISaveBlock5B2W2
{
    public SAV5B2W2() : base(SaveUtil.SIZE_G5RAW) => Blocks = new SaveBlockAccessor5B2W2(this);
    public SAV5B2W2(Memory<byte> data) : base(data) => Blocks = new SaveBlockAccessor5B2W2(this);

    public override PersonalTable5B2W2 Personal => PersonalTable.B2W2;
    public SaveBlockAccessor5B2W2 Blocks { get; }
    protected override SAV5B2W2 CloneInternal() => new(Data.ToArray());
    public override int MaxItemID => Legal.MaxItemID_5_B2W2;

    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
    public override MyItem Items => Blocks.Items;
    public override Zukan5 Zukan => Blocks.Zukan;
    public override Misc5 Misc => Blocks.Misc;
    public override MysteryBlock5 Mystery => Blocks.Mystery;
    public override Chatter5 Chatter => Blocks.Chatter;
    public override Daycare5 Daycare => Blocks.Daycare;
    public override BoxLayout5 BoxLayout => Blocks.BoxLayout;
    public override PlayerData5B2W2 PlayerData => Blocks.PlayerData;
    public override PlayerPosition5 PlayerPosition => Blocks.PlayerPosition;
    public override BattleSubwayPlay5 BattleSubwayPlay => Blocks.BattleSubwayPlay;
    public override BattleSubway5 BattleSubway => Blocks.BattleSubway;
    public override Entralink5 Entralink => Blocks.Entralink;
    public override Musical5 Musical => Blocks.Musical;
    public override Encount5 Encount => Blocks.Encount;
    public override UnityTower5 UnityTower => Blocks.UnityTower;
    public override SkinInfo5 SkinInfo => Blocks.SkinInfo;
    public override EventWork5B2W2 EventWork => Blocks.EventWork;
    public override BattleBox5 BattleBox => Blocks.BattleBox;
    public override EntreeForest EntreeForest => Blocks.EntreeForest;
    public override GlobalLink5 GlobalLink => Blocks.GlobalLink;
    public override GTS5 GTS => Blocks.GTS;
    public override WhiteBlack5B2W2 Forest => Blocks.Forest;
    public override AdventureInfo5 AdventureInfo => Blocks.AdventureInfo;
    public override Record5 Records => Blocks.Records;

    public FestaBlock5 Festa => Blocks.Festa;
    public PWTBlock5 PWT => Blocks.PWT;
    public MedalList5 Medals => Blocks.Medals;
    public KeySystem5 Keys => Blocks.Keys;

    public string Rival
    {
        get => GetString(RivalTrash);
        set => SetString(RivalTrash, value, MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public Span<byte> RivalTrash
    {
        get => Data.Slice(0x23BA4, MaxStringLengthTrainer * 2);
        set { if (value.Length == MaxStringLengthTrainer * 2) value.CopyTo(Data.Slice(0x23BA4)); }
    }

    protected override int ExtBattleVideoNativeOffset    => 0x4C000; // 0x1A00
    protected override int ExtBattleVideoDownload1Offset => 0x4DA00; // 0x1A00
    protected override int ExtBattleVideoDownload2Offset => 0x4F400; // 0x1A00
    protected override int ExtBattleVideoDownload3Offset => 0x50E00; // 0x1A00
    protected override int ExtCGearOffset => 0x52800; // 0x2800
    protected override int ExtBattleTestOffset => 0x55000; // 0x800
    protected override int ExtMusicalDownloadOffset => 0x55800; // 0x18000
    protected override int ExtPokeDexSkinOffset => 0x6D800; // 0x6800
    protected override int ExtHallOfFame1Offset => 0x74000; // 0x1800
    protected override int ExtHallOfFame2Offset => 0x75800; // 0x1800

    public const int PokestarOffset = 0x77000; // 0x600 * 8
    public const int PWTOffset = 0x7A000; // 0x1400 * 3

    public const int KeyDataOffset = 0x7DC00; // 0x400, size 0xC8

    protected override int ExtLink1Offset => 0x7E000; // 0x400
    protected override int ExtLink2Offset => 0x7E400; // 0x400

    public override int MusicalDownloadSize => MusicalShow5.SIZE_B2W2;
    public const int ExtUnk7E800 = 0x7E800; // Key System?
    public const int ExtUnkCRGF = 0x7F000; // [0..F] => checksum @ 0x10

    // B2/W2 have Pokéstar Studios and PWT DLC data too.
    public const int PokestarCount = 8;
    public const int PokestarLength   = PokestarMovie5.SIZE;
    public const int PokestarInterval = 0x600;
    private static int GetPokestarOffset(int index) => PokestarOffset + (index * PokestarInterval);

    public Memory<byte> GetPokestarMovie([Range(0, PokestarCount)] int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, PokestarCount, nameof(index));
        return Buffer.Slice(GetPokestarOffset(index), PokestarLength);
    }

    public void SetPokestarMovie([Range(0, PokestarCount)] int index, ReadOnlySpan<byte> data, ushort count = 1)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, PokestarCount, nameof(index));
        var offset = GetPokestarOffset(index);
        WriteExtSection(data, offset, PokestarLength, count);
        PlayerData.UpdateExtData(ExtDataSectionNote5.Movie1 + index, count);
    }

    public const int PWTCount = 3;
    public const int PWTLength   = 0x1214;
    public const int PWTInterval = 0x1400;
    private static int GetPWTOffset(int index) => PWTOffset + (index * PWTInterval);

    public Memory<byte> GetPWT([Range(0, PWTCount)] int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, PWTCount, nameof(index));
        return Buffer.Slice(PWTOffset + (index * PWTInterval), PWTLength);
    }

    public void SetPWT([Range(0, PWTCount)] int index, ReadOnlySpan<byte> data, ushort count = 1)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, PWTCount, nameof(index));
        var offset = GetPWTOffset(index);
        WriteExtSection(data, offset, PWTLength, count);
        PlayerData.UpdateExtData(ExtDataSectionNote5.PWT1 + index, count);
    }

    public Memory<byte> GetKeyData() => Buffer.Slice(KeyDataOffset, 0xC8);

    public void SetKeyData(ReadOnlySpan<byte> data, ushort count = 1)
    {
        PlayerData.UpdateExtData(ExtDataSectionNote5.KeyData, count);
        WriteExtSection(data, KeyDataOffset, PWTLength, count);
    }
}
