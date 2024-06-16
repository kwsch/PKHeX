using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 2 <see cref="PKM"/> format for <see cref="GameVersion.Stadium2"/>. </summary>
public sealed class SK2 : GBPKM, ICaughtData2
{
    public override PersonalInfo2 PersonalInfo => PersonalTable.C[Species];

    public override bool Valid => Species <= Legal.MaxSpeciesID_2;

    public override int SIZE_PARTY => PokeCrypto.SIZE_2STADIUM;
    public override int SIZE_STORED => PokeCrypto.SIZE_2STADIUM;
    private bool IsEncodingJapanese { get; set; }
    public override bool Japanese => IsEncodingJapanese;
    public override bool Korean => false;
    private const int StringLength = 12;

    public override EntityContext Context => EntityContext.Gen2;
    public override int MaxStringLengthTrainer => StringLength;
    public override int MaxStringLengthNickname => StringLength;

    public SK2(bool jp = false) : base(PokeCrypto.SIZE_2STADIUM) => IsEncodingJapanese = jp;
    public SK2(byte[] data) : this(data, IsJapanese(data)) { }
    public SK2(byte[] data, bool jp) : base(data) => IsEncodingJapanese = jp;

    public override SK2 Clone() => new((byte[])Data.Clone(), Japanese)
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
    public override ushort TID16 { get => ReadUInt16BigEndian(Data.AsSpan(6)); set => WriteUInt16BigEndian(Data.AsSpan(6), value); }
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
    public override byte CurrentFriendship { get => Data[0x1C]; set => Data[0x1C] = value; }

    public override byte Stat_Level { get => Data[0x1D]; set => Data[0x1D] = value; }
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
            // Rentals do not have an OT name, so clear it
            OriginalTrainerTrash.Clear();
        }
    }

    // 0x1F

    public byte PokerusState { get => Data[0x20]; set => Data[0x20] = value; }
    // Crystal only Caught Data
    public override int PokerusDays { get => PokerusState & 0xF; set => PokerusState = (byte)((PokerusState & ~0xF) | value); }
    public override int PokerusStrain { get => PokerusState >> 4; set => PokerusState = (byte)((PokerusState & 0xF) | (value << 4)); }

    public ushort CaughtData { get => ReadUInt16BigEndian(Data.AsSpan(0x21)); set => WriteUInt16BigEndian(Data.AsSpan(0x21), value); }

    public int MetTimeOfDay         { get => (CaughtData >> 14) & 0x3; set => CaughtData = (ushort)((CaughtData & 0x3FFF) | ((value & 0x3) << 14)); }
    public override byte MetLevel    { get => (byte)((CaughtData >> 8) & 0x3F); set => CaughtData = (ushort)((CaughtData & 0xC0FF) | ((value & 0x3F) << 8)); }
    public override byte OriginalTrainerGender    { get => (byte)((CaughtData >> 7) & 1);    set => CaughtData = (ushort)((CaughtData & 0xFF7F) | ((value & 1) << 7)); }
    public override ushort MetLocation { get => (ushort)(CaughtData & 0x7F);        set => CaughtData = (ushort)((CaughtData & 0xFF80) | (value & 0x7F)); }

    public override string Nickname
    {
        get => StringConverter2.GetString(NicknameTrash, Language);
        set => StringConverter2.SetString(NicknameTrash, value, StringLength, Language, StringConverterOption.None);
    }

    public override string OriginalTrainerName
    {
        get => StringConverter2.GetString(OriginalTrainerTrash, Language);
        set
        {
            if (IsRental)
            {
                OriginalTrainerTrash.Clear();
                return;
            }
            StringConverter2.SetString(OriginalTrainerTrash, value, StringLength, Language, StringConverterOption.None);
        }
    }

    public override Span<byte> NicknameTrash => Data.AsSpan(0x24, StringLength);
    public override Span<byte> OriginalTrainerTrash => Data.AsSpan(0x30, StringLength);
    public override int TrashCharCountTrainer => StringLength;
    public override int TrashCharCountNickname => StringLength;

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

    public override byte OriginalTrainerFriendship { get => CurrentFriendship; set => CurrentFriendship = value; }
    public override bool HasOriginalMetLocation => CaughtData != 0;
    public override GameVersion Version { get => GameVersion.GSC; set { } }

    protected override void GetNonNickname(int language, Span<byte> data)
    {
        var name = SpeciesName.GetSpeciesNameGeneration(Species, language, 2);
        StringConverter2.SetString(data, name, data.Length, language, StringConverterOption.Clear50);
    }

    public override void SetNotNicknamed(int language) => GetNonNickname(language, NicknameTrash);

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
        TID16 = TID16,
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
        PokerusState = PokerusState,
        CaughtData = CaughtData,

        // Only copies until first 0x50 terminator, but just copy everything
        Nickname = Nickname,
        OriginalTrainerName = IsRental ? Japanese ? "1337" : "PKHeX" : OriginalTrainerName,
    };

    private static bool IsJapanese(ReadOnlySpan<byte> data)
    {
        const byte empty = 0;
        const byte terminator = StringConverter2.TerminatorCode;

        var ot = data.Slice(0x30, StringLength);
        if (ot[6..].ContainsAnyExcept(empty, terminator))
            return false;
        if (!StringConverter2.GetIsJapanese(ot))
            return false;

        var nick = data.Slice(0x24, StringLength);
        if (nick[6..].ContainsAnyExcept(empty, terminator))
            return false;
        if (!StringConverter2.GetIsJapanese(nick))
            return false;

        return true;
    }

    private static bool IsEnglish(ReadOnlySpan<byte> data)
    {
        if (!StringConverter2.GetIsEnglish(data.Slice(0x30, StringLength)))
            return false;
        if (!StringConverter2.GetIsEnglish(data.Slice(0x24, StringLength)))
            return false;
        return true;
    }

    public bool IsPossible(bool japanese) => japanese ? IsJapanese(Data) : IsEnglish(Data);
    public void SwapLanguage() => IsEncodingJapanese ^= true;

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter2.GetString(data, Language);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter2.LoadString(data, destBuffer, Language);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter2.SetString(destBuffer, value, maxLength, Language, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesGB.GetTerminatorIndex(data);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesGB.GetStringLength(data);
    public override int GetBytesPerChar() => 1;
}
