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
    public SAV5BW(Memory<byte> data) : base(data) => Blocks = new SaveBlockAccessor5BW(this);

    public override PersonalTable5BW Personal => PersonalTable.BW;
    public SaveBlockAccessor5BW Blocks { get; }
    protected override SAV5BW CloneInternal() => new(Data.ToArray());
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
    public override BattleSubwayPlay5 BattleSubwayPlay => Blocks.BattleSubwayPlay;
    public override BattleSubway5 BattleSubway => Blocks.BattleSubway;
    public override Entralink5BW Entralink => Blocks.Entralink;
    public override Musical5 Musical => Blocks.Musical;
    public override Encount5BW Encount => Blocks.Encount;
    public override UnityTower5 UnityTower => Blocks.UnityTower;
    public override SkinInfo5 SkinInfo => Blocks.SkinInfo;
    public override EventWork5BW EventWork => Blocks.EventWork;
    public override BattleBox5 BattleBox => Blocks.BattleBox;
    public override EntreeForest EntreeForest => Blocks.EntreeForest;
    public override GlobalLink5 GlobalLink => Blocks.GlobalLink;
    public override GTS5 GTS => Blocks.GTS;
    public override WhiteBlack5BW Forest => Blocks.Forest;
    public override AdventureInfo5 AdventureInfo => Blocks.AdventureInfo;
    public override Record5 Records => Blocks.Records;

    protected override int ExtBattleVideoNativeOffset => 0x4A000;
    protected override int ExtBattleVideoDownload1Offset => 0x4C000; // 0x2000
    protected override int ExtBattleVideoDownload2Offset => 0x4E000; // 0x2000
    protected override int ExtBattleVideoDownload3Offset => 0x50000; // 0x2000
    protected override int ExtCGearOffset => 0x52000; // 0x3000
    protected override int ExtBattleTestOffset => 0x55000; // 0x1000
    protected override int ExtMusicalDownloadOffset => 0x56000; // 0x20000
    protected override int ExtPokeDexSkinOffset => 0x76000; // 0x6800
    protected override int ExtHallOfFame1Offset => 0x7C800; // 0x1800
    protected override int ExtHallOfFame2Offset => 0x7E000; // 0x1800

    protected override int ExtLink1Offset => 0x7F800; // 0x400
    protected override int ExtLink2Offset => 0x7FC00; // 0x400

    public override int MusicalDownloadSize => MusicalShow5.SIZE_BW;
}
