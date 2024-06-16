using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.ORASDEMO"/>.
/// </summary>
/// <inheritdoc cref="SAV6" />
public sealed class SAV6AODemo : SAV6, ISaveBlock6Core
{
    public SAV6AODemo(byte[] data) : base(data, SaveBlockAccessor6AODemo.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor6AODemo(this);
        Initialize();
    }

    public SAV6AODemo() : base(SaveUtil.SIZE_G6ORASDEMO, SaveBlockAccessor6AODemo.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor6AODemo(this);
        Initialize();
    }

    public override PersonalTable6AO Personal => PersonalTable.AO;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_AO;
    protected override SAV6AODemo CloneInternal() => new((byte[])Data.Clone());
    public override ushort MaxMoveID => Legal.MaxMoveID_6_AO;
    public override int MaxItemID => Legal.MaxItemID_6_AO;
    public override int MaxAbilityID => Legal.MaxAbilityID_6_AO;
    public SaveBlockAccessor6AODemo Blocks { get; }

    private void Initialize()
    {
        Party            = 0x03E00;
    }

    public override bool IsVersionValid() => Version is GameVersion.AS or GameVersion.OR;

    public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }
    public override int Vivillon { get => Blocks.Misc.Vivillon; set => Blocks.Misc.Vivillon = value; } // unused
    public override int Badges { get => Blocks.Misc.Badges; set => Blocks.Misc.Badges = value; } // unused
    public override int BP { get => Blocks.Misc.BP; set => Blocks.Misc.BP = value; } // unused
    public override MyItem6AO Items => Blocks.Items;
    public override ItemInfo6 ItemInfo => Blocks.ItemInfo;
    public override GameTime6 GameTime => Blocks.GameTime;
    public override Situation6 Situation => Blocks.Situation;
    public override PlayTime6 Played => Blocks.Played;
    public override MyStatus6 Status => Blocks.Status;
    public override RecordBlock6 Records => Blocks.Records;
    public override EventWork6 EventWork => Blocks.EventWork;
    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;

    MyItem ISaveBlock6Core.Items => Items;
    RecordBlock6 ISaveBlock6Core.Records => Records;
}
