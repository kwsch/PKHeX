using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object for <see cref="GameVersion.B2W2"/>.
/// </summary>
/// <inheritdoc cref="SAV5" />
public sealed class SAV5B2W2 : SAV5, ISaveBlock5B2W2
{
    public SAV5B2W2() : base(SaveUtil.SIZE_G5RAW)
    {
        Blocks = new SaveBlockAccessor5B2W2(this);
        Initialize();
    }

    public SAV5B2W2(byte[] data) : base(data)
    {
        Blocks = new SaveBlockAccessor5B2W2(this);
        Initialize();
    }

    public override PersonalTable5B2W2 Personal => PersonalTable.B2W2;
    public SaveBlockAccessor5B2W2 Blocks { get; }
    protected override SAV5B2W2 CloneInternal() => new((byte[]) Data.Clone());
    public override int EventWorkCount => 0x1AF; // this doesn't seem right?
    public override int EventFlagCount => 0xBF8;
    protected override int EventWorkOffset => 0x1FF00;
    protected override int EventFlagOffset => EventWorkOffset + 0x35E;
    public override int MaxItemID => Legal.MaxItemID_5_B2W2;

    private void Initialize()
    {
        BattleBoxOffset = 0x20900;
        CGearInfoOffset = 0x1C000;
        CGearDataOffset = 0x52800;
        EntreeForestOffset = 0x22A00;
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
    public FestaBlock5 Festa => Blocks.Festa;
    public PWTBlock5 PWT => Blocks.PWT;
    public MedalList5 Medals => Blocks.Medals;
    public int Fused => 0x1FA00 + sizeof(uint);
    public override int GTS => 0x20400;

    public string Rival
    {
        get => GetString(Rival_Trash);
        set => SetString(Rival_Trash, value, MaxStringLengthOT, StringConverterOption.ClearZero);
    }

    public Span<byte> Rival_Trash
    {
        get => Data.AsSpan(0x23BA4, MaxStringLengthOT * 2);
        set { if (value.Length == MaxStringLengthOT * 2) value.CopyTo(Data.AsSpan(0x23BA4)); }
    }

    public override string GetDaycareRNGSeed(int loc) => $"{Daycare.GetSeed()!:X16}";
}
