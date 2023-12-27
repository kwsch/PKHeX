using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object for <see cref="GameVersion.BW"/>.
/// </summary>
/// <inheritdoc cref="SAV5" />
public sealed class SAV5BW : SAV5
{
    public SAV5BW() : base(SaveUtil.SIZE_G5RAW)
    {
        Blocks = new SaveBlockAccessor5BW(this);
        Initialize();
    }

    public SAV5BW(byte[] data) : base(data)
    {
        Blocks = new SaveBlockAccessor5BW(this);
        Initialize();
    }

    public override PersonalTable5BW Personal => PersonalTable.BW;
    public SaveBlockAccessor5BW Blocks { get; }
    protected override SAV5BW CloneInternal() => new((byte[])Data.Clone());
    public override int EventWorkCount => 0x13E;
    public override int EventFlagCount => 0xB60;
    protected override int EventWorkOffset => 0x20100;
    protected override int EventFlagOffset => EventWorkOffset + 0x27C;
    public override int MaxItemID => Legal.MaxItemID_5_BW;

    private void Initialize()
    {
        BattleBoxOffset = 0x20A00;
        CGearInfoOffset = 0x1C000;
        CGearDataOffset = 0x52000;
        EntreeForestOffset = 0x22C00;
        PokeDex = Blocks.Zukan.PokeDex;
        WondercardData = Blocks.Mystery.Offset;
        DaycareOffset = Blocks.Daycare.Offset;
    }

    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
    public override MyItem Items => Blocks.Items;
    public override Zukan5 Zukan => Blocks.Zukan;
    public override Misc5 Misc => Blocks.Misc;
    public override MysteryBlock5 Mystery => Blocks.Mystery;
    public override Chatter5 Chatter => Blocks.Chatter;
    public override Daycare5 Daycare => Blocks.Daycare;
    public override BoxLayout5 BoxLayout => Blocks.BoxLayout;
    public override PlayerData5 PlayerData => Blocks.PlayerData;
    public override BattleSubway5 BattleSubway => Blocks.BattleSubway;
    public override Entralink5 Entralink => Blocks.Entralink;
    public override Musical5 Musical => Blocks.Musical;
    public override Encount5 Encount => Blocks.Encount;
    public override UnityTower5 UnityTower => Blocks.UnityTower;
    public override int GTS => 0x20500;
}
