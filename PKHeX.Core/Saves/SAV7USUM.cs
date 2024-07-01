using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SAV7USUM : SAV7, ISaveBlock7USUM
{
    public SAV7USUM(byte[] data) : base(data, SaveBlockAccessor7USUM.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor7USUM(this);
        Initialize();
        ClearMemeCrypto();
    }

    public SAV7USUM() : base(SaveUtil.SIZE_G7USUM, boUU)
    {
        Blocks = new SaveBlockAccessor7USUM(this);
        Initialize();
    }

    public override bool HasPokeDex => true;

    private void Initialize()
    {
        Party = Blocks.BlockInfo[04].Offset;
        TeamSlots = Blocks.BoxLayout.TeamSlots;
        Box = Blocks.BlockInfo[14].Offset;

        ReloadBattleTeams();
    }

    public override PersonalTable7 Personal => PersonalTable.USUM;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_USUM;
    protected override SAV7USUM CloneInternal() => new((byte[])Data.Clone());
    public override ushort MaxMoveID => Legal.MaxMoveID_7_USUM;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_7_USUM;
    public override int MaxItemID => Legal.MaxItemID_7_USUM;
    public override int MaxAbilityID => Legal.MaxAbilityID_7_USUM;

    private const int boUU = SaveUtil.SIZE_G7USUM - 0x200;

    #region Blocks
    public SaveBlockAccessor7USUM Blocks { get; }
    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
    public override MyItem7USUM Items => Blocks.Items;
    public override MysteryBlock7 MysteryGift => Blocks.MysteryGift;
    public override PokeFinder7 PokeFinder => Blocks.PokeFinder;
    public override JoinFesta7 Festa => Blocks.Festa;
    public override Daycare7 Daycare => Blocks.Daycare;
    public override RecordBlock7USUM Records => Blocks.Records;
    public override PlayTime6 Played => Blocks.Played;
    public override MyStatus7 MyStatus => Blocks.MyStatus;
    public override FieldMoveModelSave7 Overworld => Blocks.Overworld;
    public override Situation7 Situation => Blocks.Situation;
    public override ConfigSave7 Config => Blocks.Config;
    public override GameTime7 GameTime => Blocks.GameTime;
    public override Misc7 Misc => Blocks.Misc;
    public override Zukan7 Zukan => Blocks.Zukan;
    public override BoxLayout7 BoxLayout => Blocks.BoxLayout;
    public override BattleTree7 BattleTree => Blocks.BattleTree;
    public override ResortSave7 ResortSave => Blocks.ResortSave;
    public override FieldMenu7 FieldMenu => Blocks.FieldMenu;
    public override FashionBlock7 Fashion => Blocks.Fashion;
    public override EventWork7USUM EventWork => Blocks.EventWork;
    public override UnionPokemon7 Fused => Blocks.Fused;
    public override GTS7 GTS => Blocks.GTS;
    public BattleAgency7 BattleAgency => Blocks.BattleAgency;
    #endregion

    private const ulong MagearnaConst = 0xCBE05F18356504AC;
    private const ulong CapPikachuConst = 0xF44E94EA7D19A8D6;

    public override void UpdateQrConstants()
    {
        var qr = Blocks.BlockInfo[35];
        var flag = EventWork.GetEventFlag(EventWork7USUM.MagearnaEventFlag); // 4060
        ulong value = flag ? MagearnaConst : 0ul;
        WriteUInt64LittleEndian(Data.AsSpan(qr.Offset + 0x168), value);

        flag = EventWork.GetEventFlag(EventWork7USUM.CapPikachuEventFlag); // 4060
        value = flag ? CapPikachuConst : 0ul;
        WriteUInt64LittleEndian(Data.AsSpan(qr.Offset + 0x16C), value);
    }
}
