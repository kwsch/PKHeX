using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 2 <see cref="PKM"/> format for <see cref="GameVersion.Stadium2"/>. </summary>
public sealed class SK2 : GBPKM, ICaughtData2
{
    public override PersonalInfo PersonalInfo => PersonalTable.C[Species];

    public override bool Valid => Species <= 252;

    public override int SIZE_PARTY => PokeCrypto.SIZE_2STADIUM;
    public override int SIZE_STORED => PokeCrypto.SIZE_2STADIUM;
    private bool IsEncodingJapanese { get; set; }
    public override bool Japanese => IsEncodingJapanese;
    public override bool Korean => false;
    private const int StringLength = 12;

    public override EntityContext Context => EntityContext.Gen2;
    public override int OTLength => StringLength;
    public override int NickLength => StringLength;

    public SK2(bool jp = false) : base(PokeCrypto.SIZE_2STADIUM) => IsEncodingJapanese = jp;
    public SK2(byte[] data) : this(data, IsJapanese(data)) { }
    public SK2(byte[] data, bool jp) : base(data) => IsEncodingJapanese = jp;

    public override PKM Clone() => new SK2((byte[])Data.Clone(), Japanese)
    {
        IsEgg = IsEgg,
    };

    protected override byte[] Encrypt() => Data;

    #region Stored Attributes
    public override ushort Species { get => Data[0]; set => Data[0] = (byte)value; }
    public override int SpriteItem => ItemConverter.GetItemFuture2((byte)HeldItem);
    public override int HeldItem { get => Data[0x1]; set => Data[0x1] = (byte)value; }
    public override ushort Move1 { get => Data[2]; set => Data[2] = (byte)value; }
    public override ushort Move2 { get => Data[3]; set => Data[3] = (byte)value; }
    public override ushort Move3 { get => Data[4]; set => Data[4] = (byte)value; }
    public override ushort Move4 { get => Data[5]; set => Data[5] = (byte)value; }
    public override int TID { get => ReadUInt16BigEndian(Data.AsSpan(6)); set => WriteUInt16BigEndian(Data.AsSpan(6), (ushort)value); }
    public override uint EXP { get => ReadUInt32BigEndian(Data.AsSpan(8)); set => WriteUInt32BigEndian(Data.AsSpan(8), value); } // not 3 bytes like in PK2
    public override int EV_HP { get => ReadUInt16BigEndian(Data.AsSpan(0x0C)); set => WriteUInt16BigEndian(Data.AsSpan(0x0C), (ushort)value); }
    public override int EV_ATK { get => ReadUInt16BigEndian(Data.AsSpan(0x0E)); set => WriteUInt16BigEndian(Data.AsSpan(0x0E), (ushort)value); }
    public override int EV_DEF { get => ReadUInt16BigEndian(Data.AsSpan(0x10)); set => WriteUInt16BigEndian(Data.AsSpan(0x10), (ushort)value); }
    public override int EV_SPE { get => ReadUInt16BigEndian(Data.AsSpan(0x12)); set => WriteUInt16BigEndian(Data.AsSpan(0x12), (ushort)value); }
    public override int EV_SPC { get => ReadUInt16BigEndian(Data.AsSpan(0x14)); set => WriteUInt16BigEndian(Data.AsSpan(0x14), (ushort)value); }
    public override ushort DV16 { get => ReadUInt16BigEndian(Data.AsSpan(0x16)); set => WriteUInt16BigEndian(Data.AsSpan(0x16), value); }
    public override int Move1_PP { get => Data[0x18] & 0x3F; set => Data[0x18] = (byte)((Data[0x18] & 0xC0) | Math.Min(63, value)); }
    public override int Move2_PP { get => Data[0x19] & 0x3F; set => Data[0x19] = (byte)((Data[0x19] & 0xC0) | Math.Min(63, value)); }
    public override int Move3_PP { get => Data[0x1A] & 0x3F; set => Data[0x1A] = (byte)((Data[0x1A] & 0xC0) | Math.Min(63, value)); }
    public override int Move4_PP { get => Data[0x1B] & 0x3F; set => Data[0x1B] = (byte)((Data[0x1B] & 0xC0) | Math.Min(63, value)); }
    public override int Move1_PPUps { get => (Data[0x18] & 0xC0) >> 6; set => Data[0x18] = (byte)((Data[0x18] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move2_PPUps { get => (Data[0x19] & 0xC0) >> 6; set => Data[0x19] = (byte)((Data[0x19] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move3_PPUps { get => (Data[0x1A] & 0xC0) >> 6; set => Data[0x1A] = (byte)((Data[0x1A] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move4_PPUps { get => (Data[0x1B] & 0xC0) >> 6; set => Data[0x1B] = (byte)((Data[0x1B] & 0x3F) | ((value & 0x3) << 6)); }
    public override int CurrentFriendship { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }

    public override int Stat_Level { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
    public override bool IsEgg { get => (Data[0x1E] & 1) == 1; set => Data[0x1E] = (byte)((Data[0x1E] & ~1) | (value ? 1 : 0)); }

    public bool IsRental
    {
        get => (Data[0x1E] & 4) == 4;
        set
        {
            if (!value)
            {
                Data[0x1E] &= 0xFB;
                return;
            }

            Data[0x1E] |= 4;
            OT_Name = string.Empty;
        }
    }

    // 0x1F

    private byte PKRS { get => Data[0x20]; set => Data[0x20] = value; }
    // Crystal only Caught Data
    public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
    public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | (value << 4)); }

    public ushort CaughtData { get => ReadUInt16BigEndian(Data.AsSpan(0x21)); set => WriteUInt16BigEndian(Data.AsSpan(0x21), value); }

    public int Met_TimeOfDay         { get => (CaughtData >> 14) & 0x3; set => CaughtData = (ushort)((CaughtData & 0x3FFF) | ((value & 0x3) << 14)); }
    public override int Met_Level    { get => (CaughtData >> 8) & 0x3F; set => CaughtData = (ushort)((CaughtData & 0xC0FF) | ((value & 0x3F) << 8)); }
    public override int OT_Gender    { get => (CaughtData >> 7) & 1;    set => CaughtData = (ushort)((CaughtData & 0xFF7F) | ((value & 1) << 7)); }
    public override int Met_Location { get => CaughtData & 0x7F;        set => CaughtData = (ushort)((CaughtData & 0xFF80) | (value & 0x7F)); }

    public override string Nickname
    {
        get => StringConverter12.GetString(Nickname_Trash, Japanese);
        set => StringConverter12.SetString(Nickname_Trash, value.AsSpan(), 12, Japanese, StringConverterOption.None);
    }

    public override string OT_Name
    {
        get => StringConverter12.GetString(OT_Trash, Japanese);
        set
        {
            if (IsRental)
            {
                OT_Trash.Clear();
                return;
            }
            StringConverter12.SetString(OT_Trash, value.AsSpan(), StringLength, Japanese, StringConverterOption.None);
        }
    }

    public override Span<byte> Nickname_Trash => Data.AsSpan(0x24, 12);
    public override Span<byte> OT_Trash => Data.AsSpan(0x30, 12);

    #endregion

    #region Party Attributes
    public override int Status_Condition { get; set; }
    public override int Stat_HPCurrent { get; set; }
    public override int Stat_HPMax { get; set; }
    public override int Stat_ATK { get; set; }
    public override int Stat_DEF { get; set; }
    public override int Stat_SPE { get; set; }
    public override int Stat_SPA { get; set; }
    public override int Stat_SPD { get; set; }
    #endregion

    public override int OT_Friendship { get => CurrentFriendship; set => CurrentFriendship = value; }
    public override bool HasOriginalMetLocation => CaughtData != 0;
    public override int Version { get => (int)GameVersion.GSC; set { } }

    protected override byte[] GetNonNickname(int language)
    {
        var name = SpeciesName.GetSpeciesNameGeneration(Species, language, 2);
        byte[] data = new byte[name.Length];
        StringConverter12.SetString(data, name.AsSpan(), data.Length, Japanese, StringConverterOption.Clear50);
        return data;
    }

    public override void SetNotNicknamed(int language)
    {
        var name = SpeciesName.GetSpeciesNameGeneration(Species, language, 2);
        Nickname_Trash.Clear();
        Nickname = name;
    }

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_2;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_2;
    public override int MaxAbilityID => Legal.MaxAbilityID_2;
    public override int MaxItemID => Legal.MaxItemID_2;

    public PK2 ConvertToPK2() => new(Japanese)
    {
        Species = Species,
        HeldItem = HeldItem,
        Move1 = Move1,
        Move2 = Move2,
        Move3 = Move3,
        Move4 = Move4,
        TID = TID,
        EXP = EXP,
        EV_HP = EV_HP,
        EV_ATK = EV_ATK,
        EV_DEF = EV_DEF,
        EV_SPE = EV_SPE,
        EV_SPC = EV_SPC,
        DV16 = DV16,
        Move1_PP = Move1_PP,
        Move2_PP = Move2_PP,
        Move3_PP = Move3_PP,
        Move4_PP = Move4_PP,
        Move1_PPUps = Move1_PPUps,
        Move2_PPUps = Move2_PPUps,
        Move3_PPUps = Move3_PPUps,
        Move4_PPUps = Move4_PPUps,
        CurrentFriendship = CurrentFriendship,
        Stat_Level = Stat_Level,
        IsEgg = IsEgg,
        PKRS_Days = PKRS_Days,
        PKRS_Strain = PKRS_Strain,
        CaughtData = CaughtData,

        // Only copies until first 0x50 terminator, but just copy everything
        Nickname = Nickname,
        OT_Name = IsRental ? Japanese ? "1337" : "PKHeX" : OT_Name,
    };

    private static bool IsJapanese(ReadOnlySpan<byte> data)
    {
        if (!StringConverter12.GetIsG1Japanese(data.Slice(0x30, StringLength)))
            return false;
        if (!StringConverter12.GetIsG1Japanese(data.Slice(0x24, StringLength)))
            return false;

        for (int i = 6; i < 0xC; i++)
        {
            if (data[0x30 + i] is not (0 or StringConverter12.G1TerminatorCode))
                return false;
            if (data[0x24 + i] is not (0 or StringConverter12.G1TerminatorCode))
                return false;
        }
        return true;
    }

    private static bool IsEnglish(ReadOnlySpan<byte> data)
    {
        if (!StringConverter12.GetIsG1English(data.Slice(0x30, StringLength)))
            return false;
        if (!StringConverter12.GetIsG1English(data.Slice(0x24, StringLength)))
            return false;
        return true;
    }

    public bool IsPossible(bool japanese) => japanese ? IsJapanese(Data) : IsEnglish(Data);
    public void SwapLanguage() => IsEncodingJapanese ^= true;
}
