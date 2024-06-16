using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object for <see cref="GameVersion.BW"/>.
/// </summary>
/// <inheritdoc cref="SAV5" />
public sealed class SAV5BW : SAV5
{
    public SAV5BW() : base(SaveUtil.SIZE_G5RAW) => Blocks = new SaveBlockAccessor5BW(this);
    public SAV5BW(byte[] data) : base(data) => Blocks = new SaveBlockAccessor5BW(this);

    public override PersonalTable5BW Personal => PersonalTable.BW;
    public SaveBlockAccessor5BW Blocks { get; }
    protected override SAV5BW CloneInternal() => new((byte[])Data.Clone());
    public override int MaxItemID => Legal.MaxItemID_5_BW;

    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
    public override MyItem5BW Items => Blocks.Items;
    public override Zukan5 Zukan => Blocks.Zukan;
    public override Misc5BW Misc => Blocks.Misc;
    public override MysteryBlock5 Mystery => Blocks.Mystery;
    public override Chatter5 Chatter => Blocks.Chatter;
    public override Daycare5 Daycare => Blocks.Daycare;
    public override BoxLayout5 BoxLayout => Blocks.BoxLayout;
    public override PlayerData5 PlayerData => Blocks.PlayerData;
    public override PlayerPosition5 PlayerPosition => Blocks.PlayerPosition;
    public override BattleSubway5 BattleSubway => Blocks.BattleSubway;
    public override Entralink5BW Entralink => Blocks.Entralink;
    public override Musical5 Musical => Blocks.Musical;
    public override Encount5BW Encount => Blocks.Encount;
    public override UnityTower5 UnityTower => Blocks.UnityTower;
    public override EventWork5BW EventWork => Blocks.EventWork;
    public override BattleBox5 BattleBox => Blocks.BattleBox;
    public override EntreeForest EntreeForest => Blocks.EntreeForest;
    public override GlobalLink5 GlobalLink => Blocks.GlobalLink;
    public override GTS5 GTS => Blocks.GTS;
    public override WhiteBlack5BW Forest => Blocks.Forest;
    public override AdventureInfo5 AdventureInfo => Blocks.AdventureInfo;
    public override Record5 Records => Blocks.Records;

    public override Memory<byte> BattleVideoNative    => Data.AsMemory(0x4A000, BattleVideo5.SIZE_USED);
    public override Memory<byte> BattleVideoDownload1 => Data.AsMemory(0x4C000, BattleVideo5.SIZE_USED);
    public override Memory<byte> BattleVideoDownload2 => Data.AsMemory(0x4E000, BattleVideo5.SIZE_USED);
    public override Memory<byte> BattleVideoDownload3 => Data.AsMemory(0x50000, BattleVideo5.SIZE_USED);
    protected override int CGearDataOffset => 0x52000; // ^ + 0x2000 spacing
}
