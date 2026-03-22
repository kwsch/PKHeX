using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for <see cref="GameVersion.E"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3E : SAV3, IDaycareRandomState<uint>
{
    // Configuration
    protected override SAV3E CloneInternal() => new(GetFinalData()) { Language = Language };
    public override SaveBlock3SmallE SmallBlock { get; }
    public override SaveBlock3LargeE LargeBlock { get; }
    public override GameVersion Version { get => GameVersion.E; set { } }
    public override PersonalTable3 Personal => PersonalTable.E;

    public SAV3E(Memory<byte> data) : base(data)
    {
        SmallBlock = new SaveBlock3SmallE(SmallBuffer[..0xF2C]);
        LargeBlock = new SaveBlock3LargeE(LargeBuffer[..0x3D88]);
    }
    public SAV3E(bool japanese = false) : base(japanese)
    {
        SmallBlock = new SaveBlock3SmallE(SmallBuffer[..0xF2C]);
        LargeBlock = new SaveBlock3LargeE(LargeBuffer[..0x3D88]);
    }

    public override PlayerBag3E Inventory => new(this);

    public override int MaxItemID => Legal.MaxItemID_3_E;

    #region Small
    public override bool NationalDex
    {
        get => SmallBlock.PokedexNationalMagicRSE == PokedexNationalUnlockRSE;
        set
        {
            SmallBlock.PokedexMode = value ? (byte)1 : (byte)0; // mode
            SmallBlock.PokedexNationalMagicRSE = value ? PokedexNationalUnlockRSE : (byte)0; // magic
            SetEventFlag(0x896, value);
            SetWork(0x46, PokedexNationalUnlockWorkRSE);
        }
    }

    #endregion

    #region Large

    public override uint Money
    {
        get => LargeBlock.Money ^ SmallBlock.SecurityKey;
        set => LargeBlock.Money = value ^ SmallBlock.SecurityKey;
    }

    public override uint Coin
    {
        get => (ushort)(LargeBlock.Coin ^ SmallBlock.SecurityKey);
        set => LargeBlock.Coin = (ushort)(value ^ SmallBlock.SecurityKey);
    }

    protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(slot + 1) - 4; // @ end of each pk slot
    uint IDaycareRandomState<uint>.Seed // after the 2 slots, before the step counter
    {
        get => LargeBlock.DaycareSeed;
        set => LargeBlock.DaycareSeed = value;
    }
    #endregion

    private const uint EXTRADATA_SENTINEL = 0x0000B39D;
    public bool HasBattleVideo => IsFullSaveFile && ReadUInt32LittleEndian(GetFinalExternalData().Span) == EXTRADATA_SENTINEL;

    public void SetExtraDataSentinelBattleVideo() => WriteUInt32LittleEndian(GetFinalExternalData().Span, EXTRADATA_SENTINEL);

    public Memory<byte> BattleVideoData => GetFinalExternalData().Slice(4, BattleVideo3.SIZE);
    public BattleVideo3 BattleVideo
    {
        // decouple from the save file object on get, as the consumer might not be aware that mutations will touch the save.
        get => HasBattleVideo ? new BattleVideo3(BattleVideoData.ToArray()) : new BattleVideo3();
        set => SetData(BattleVideoData.Span, value.Data);
    }
}
