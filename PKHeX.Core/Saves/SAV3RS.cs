using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for <see cref="GameVersion.RS"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3RS : SAV3, IDaycareRandomState<ushort>
{
    // Configuration
    protected override SAV3RS CloneInternal() => new(GetFinalData()) { Language = Language };
    public override SaveBlock3SmallRS SmallBlock { get; }
    public override SaveBlock3LargeRS LargeBlock { get; }

    public override GameVersion Version
    {
        get;
        set => field = value is GameVersion.R or GameVersion.S ? value : GameVersion.RS;
    } = GameVersion.RS;

    public override PersonalTable3 Personal => PersonalTable.RS;
    public override int MaxItemID => Legal.MaxItemID_3_RS;

    public SAV3RS(Memory<byte> data) : base(data)
    {
        SmallBlock = new SaveBlock3SmallRS(SmallBuffer[..0x890]);
        LargeBlock = new SaveBlock3LargeRS(LargeBuffer[..0x3AC0]);
    }

    public SAV3RS(bool japanese = false) : base(japanese)
    {
        SmallBlock = new SaveBlock3SmallRS(SmallBuffer[..0x890]);
        LargeBlock = new SaveBlock3LargeRS(LargeBuffer[..0x3AC0]);
    }

    public override PlayerBag3RS Inventory => new(this);

    #region Small
    public override bool NationalDex
    {
        get => SmallBlock.PokedexNationalMagicRSE == PokedexNationalUnlockRSE;
        set
        {
            SmallBlock.PokedexMode = value ? (byte)1 : (byte)0;
            SmallBlock.PokedexNationalMagicRSE = value ? PokedexNationalUnlockRSE : (byte)0;
            SetEventFlag(0x836, value);
            SetWork(0x46, PokedexNationalUnlockWorkRSE);
        }
    }
    #endregion

    #region Large
    public override uint Money
    {
        get => LargeBlock.Money;
        set => LargeBlock.Money = value;
    }

    public override uint Coin
    {
        get => LargeBlock.Coin;
        set => LargeBlock.Coin = (ushort)value;
    }

    protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(2) + (2 * 0x38) + (4 * slot); // consecutive vals, after both consecutive slots & 2 mail
    ushort IDaycareRandomState<ushort>.Seed
    {
        get => LargeBlock.DaycareSeed;
        set => LargeBlock.DaycareSeed = value;
    }
    #endregion
}
