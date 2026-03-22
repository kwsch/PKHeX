using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object for <see cref="GameVersion.FRLG"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3FRLG : SAV3, IDaycareRandomState<ushort>
{
    // Configuration
    protected override SAV3FRLG CloneInternal() => new(GetFinalData()) { Language = Language };
    public override SaveBlock3SmallFRLG SmallBlock { get; }
    public override SaveBlock3LargeFRLG LargeBlock { get; }
    public override GameVersion Version
    {
        get;
        set => field = value is GameVersion.FR or GameVersion.LG ? value : GameVersion.FRLG;
    } = GameVersion.FR; // allow mutation
    private PersonalTable3 _personal = PersonalTable.FR;
    public override PersonalTable3 Personal => _personal;
    public override int MaxItemID => Legal.MaxItemID_3_FRLG;

    public SAV3FRLG(bool japanese = false) : base(japanese)
    {
        SmallBlock = new SaveBlock3SmallFRLG(SmallBuffer[..0xF24]);
        LargeBlock = new SaveBlock3LargeFRLG(LargeBuffer[..0x3D68]);
    }

    public SAV3FRLG(Memory<byte> data) : base(data)
    {
        SmallBlock = new SaveBlock3SmallFRLG(SmallBuffer[..0xF24]);
        LargeBlock = new SaveBlock3LargeFRLG(LargeBuffer[..0x3D68]);

        // Fix save files that have an overflow corruption with the Pokédex.
        // Future loads of this save file will cause it to be recognized as FR/LG correctly.
        if (IsCorruptPokedexFF())
            SmallBlock.FixDummyFlags();
    }

    public override PlayerBag3FRLG Inventory => new(this);

    public bool ResetPersonal(GameVersion g)
    {
        if (g is not (GameVersion.FR or GameVersion.LG))
            return false;
        _personal = g == GameVersion.FR ? PersonalTable.FR : PersonalTable.LG;
        Version = g;
        return true;
    }

    #region Small
    public override bool NationalDex
    {
        get => SmallBlock.PokedexNationalMagicFRLG == PokedexNationalUnlockFRLG;
        set
        {
            SmallBlock.PokedexNationalMagicFRLG = value ? PokedexNationalUnlockFRLG : (byte)0;
            SetEventFlag(0x840, value);
            SetWork(0x4E, PokedexNationalUnlockWorkFRLG);
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
    ushort IDaycareRandomState<ushort>.Seed
    {
        get => LargeBlock.DaycareSeed;
        set => LargeBlock.DaycareSeed = value;
    }

    public string RivalName
    {
        get => GetString(LargeBlock.RivalNameTrash);
        set => SetString(LargeBlock.RivalNameTrash, value, 7, StringConverterOption.ClearZero);
    }
    #endregion
}
