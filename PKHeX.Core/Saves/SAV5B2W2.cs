using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object for <see cref="GameVersion.B2W2"/>.
/// </summary>
/// <inheritdoc cref="SAV5" />
public sealed class SAV5B2W2 : SAV5, ISaveBlock5B2W2
{
    public SAV5B2W2() : base(SaveUtil.SIZE_G5RAW) => Blocks = new SaveBlockAccessor5B2W2(this);
    public SAV5B2W2(byte[] data) : base(data) => Blocks = new SaveBlockAccessor5B2W2(this);

    public override PersonalTable5B2W2 Personal => PersonalTable.B2W2;
    public SaveBlockAccessor5B2W2 Blocks { get; }
    protected override SAV5B2W2 CloneInternal() => new((byte[]) Data.Clone());
    public override int MaxItemID => Legal.MaxItemID_5_B2W2;

    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
    public override MyItem Items => Blocks.Items;
    public override Zukan5 Zukan => Blocks.Zukan;
    public override Misc5 Misc => Blocks.Misc;
    public override MysteryBlock5 Mystery => Blocks.Mystery;
    public override Chatter5 Chatter => Blocks.Chatter;
    public override Daycare5 Daycare => Blocks.Daycare;
    public override BoxLayout5 BoxLayout => Blocks.BoxLayout;
    public override PlayerData5 PlayerData => Blocks.PlayerData;
    public override PlayerPosition5 PlayerPosition => Blocks.PlayerPosition;
    public override BattleSubway5 BattleSubway => Blocks.BattleSubway;
    public override Entralink5 Entralink => Blocks.Entralink;
    public override Musical5 Musical => Blocks.Musical;
    public override Encount5 Encount => Blocks.Encount;
    public override UnityTower5 UnityTower => Blocks.UnityTower;
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
        get => Data.AsSpan(0x23BA4, MaxStringLengthTrainer * 2);
        set { if (value.Length == MaxStringLengthTrainer * 2) value.CopyTo(Data.AsSpan(0x23BA4)); }
    }

    public override Memory<byte> BattleVideoNative    => Data.AsMemory(0x4C000, BattleVideo5.SIZE_USED);
    public override Memory<byte> BattleVideoDownload1 => Data.AsMemory(0x4DA00, BattleVideo5.SIZE_USED);
    public override Memory<byte> BattleVideoDownload2 => Data.AsMemory(0x4F400, BattleVideo5.SIZE_USED);
    public override Memory<byte> BattleVideoDownload3 => Data.AsMemory(0x50E00, BattleVideo5.SIZE_USED);
    protected override int CGearDataOffset => 0x52800; // ^ + 0x1A00 spacing
}
