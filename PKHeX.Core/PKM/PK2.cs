using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 2 <see cref="PKM"/> format. </summary>
public sealed class PK2 : GBPKML, ICaughtData2
{
    public override PersonalInfo2 PersonalInfo => PersonalTable.C[Species];

    public override bool Valid => Species <= Legal.MaxSpeciesID_2;

    public override int SIZE_PARTY => PokeCrypto.SIZE_2PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_2STORED;
    public override bool Korean => !Japanese && OriginalTrainerTrash[0] <= 0xB;

    public override EntityContext Context => EntityContext.Gen2;

    public PK2(bool jp = false) : base(PokeCrypto.SIZE_2PARTY, jp) { }
    public PK2(byte[] decryptedData, bool jp = false) : base(EnsurePartySize(decryptedData), jp) { }

    public PK2(ReadOnlySpan<byte> data, ReadOnlySpan<byte> ot, ReadOnlySpan<byte> nick)
        : this(ot.Length == StringLengthJapanese)
    {
        data.CopyTo(Data);
        ot.CopyTo(OriginalTrainerTrash);
        nick.CopyTo(NicknameTrash);
    }

    private static byte[] EnsurePartySize(byte[] data)
    {
        if (data.Length != PokeCrypto.SIZE_2PARTY)
            Array.Resize(ref data, PokeCrypto.SIZE_2PARTY);
        return data;
    }

    public override PK2 Clone()
    {
        PK2 clone = new((byte[])Data.Clone(), Japanese) { IsEgg = IsEgg };
        OriginalTrainerTrash.CopyTo(clone.OriginalTrainerTrash);
        NicknameTrash.CopyTo(clone.NicknameTrash);
        return clone;
    }

    protected override byte[] Encrypt() => PokeList2.WrapSingle(this);

    #region Stored Attributes
    public override ushort Species { get => Data[0]; set => Data[0] = (byte)value; }
    public byte SpeciesInternal { get => Data[0]; set => Data[0] = value; } // Alias with a different type.
    public override int SpriteItem => ItemConverter.GetItemFuture2((byte)HeldItem);
    public override int HeldItem { get => Data[0x1]; set => Data[0x1] = (byte)value; }
    public override ushort Move1 { get => Data[2]; set => Data[2] = (byte)value; }
    public override ushort Move2 { get => Data[3]; set => Data[3] = (byte)value; }
    public override ushort Move3 { get => Data[4]; set => Data[4] = (byte)value; }
    public override ushort Move4 { get => Data[5]; set => Data[5] = (byte)value; }
    public override ushort TID16 { get => ReadUInt16BigEndian(Data.AsSpan(6)); set => WriteUInt16BigEndian(Data.AsSpan(6), value); }
    public override uint EXP { get => ReadUInt32BigEndian(Data.AsSpan(0x08)) >> 8; set => WriteUInt32BigEndian(Data.AsSpan(8), (value << 8) | Data[0xB]); }
    public override int EV_HP  { get => ReadUInt16BigEndian(Data.AsSpan(0x0B)); set => WriteUInt16BigEndian(Data.AsSpan(0xB), (ushort)value); }
    public override int EV_ATK { get => ReadUInt16BigEndian(Data.AsSpan(0x0D)); set => WriteUInt16BigEndian(Data.AsSpan(0xD), (ushort)value); }
    public override int EV_DEF { get => ReadUInt16BigEndian(Data.AsSpan(0x0F)); set => WriteUInt16BigEndian(Data.AsSpan(0xF), (ushort)value); }
    public override int EV_SPE { get => ReadUInt16BigEndian(Data.AsSpan(0x11)); set => WriteUInt16BigEndian(Data.AsSpan(0x11), (ushort)value); }
    public override int EV_SPC { get => ReadUInt16BigEndian(Data.AsSpan(0x13)); set => WriteUInt16BigEndian(Data.AsSpan(0x13), (ushort)value); }
    public override ushort DV16 { get => ReadUInt16BigEndian(Data.AsSpan(0x15)); set => WriteUInt16BigEndian(Data.AsSpan(0x15), value); }
    public override int Move1_PP { get => Data[0x17] & 0x3F; set => Data[0x17] = (byte)((Data[0x17] & 0xC0) | Math.Min(63, value)); }
    public override int Move2_PP { get => Data[0x18] & 0x3F; set => Data[0x18] = (byte)((Data[0x18] & 0xC0) | Math.Min(63, value)); }
    public override int Move3_PP { get => Data[0x19] & 0x3F; set => Data[0x19] = (byte)((Data[0x19] & 0xC0) | Math.Min(63, value)); }
    public override int Move4_PP { get => Data[0x1A] & 0x3F; set => Data[0x1A] = (byte)((Data[0x1A] & 0xC0) | Math.Min(63, value)); }
    public override int Move1_PPUps { get => (Data[0x17] & 0xC0) >> 6; set => Data[0x17] = (byte)((Data[0x17] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move2_PPUps { get => (Data[0x18] & 0xC0) >> 6; set => Data[0x18] = (byte)((Data[0x18] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move3_PPUps { get => (Data[0x19] & 0xC0) >> 6; set => Data[0x19] = (byte)((Data[0x19] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move4_PPUps { get => (Data[0x1A] & 0xC0) >> 6; set => Data[0x1A] = (byte)((Data[0x1A] & 0x3F) | ((value & 0x3) << 6)); }
    public override byte CurrentFriendship { get => Data[0x1B]; set => Data[0x1B] = value; }
    public byte PokerusState { get => Data[0x1C]; set => Data[0x1C] = value; }
    public override int PokerusDays { get => PokerusState & 0xF; set => PokerusState = (byte)((PokerusState & ~0xF) | value); }
    public override int PokerusStrain { get => PokerusState >> 4; set => PokerusState = (byte)((PokerusState & 0xF) | (value << 4)); }
    // Crystal only Caught Data
    public ushort CaughtData { get => ReadUInt16BigEndian(Data.AsSpan(0x1D)); set => WriteUInt16BigEndian(Data.AsSpan(0x1D), value); }
    public int MetTimeOfDay         { get => (CaughtData >> 14) & 0x3; set => CaughtData = (ushort)((CaughtData & 0x3FFF) | ((value & 0x3) << 14)); }
    public override byte MetLevel    { get => (byte)((CaughtData >> 8) & 0x3F); set => CaughtData = (ushort)((CaughtData & 0xC0FF) | ((value & 0x3F) << 8)); }
    public override byte OriginalTrainerGender    { get => (byte)((CaughtData >> 7) & 1); set => CaughtData = (ushort)((CaughtData & 0xFF7F) | ((value & 1) << 7)); }
    public override ushort MetLocation { get => (byte)(CaughtData & 0x7F); set => CaughtData = (ushort)((CaughtData & 0xFF80) | (value & 0x7F)); }

    public override byte Stat_Level
    {
        get => Data[0x1F];
        set => Data[0x1F] = value;
    }

    #endregion

    #region Party Attributes
    public override int Status_Condition { get => Data[0x20]; set => Data[0x20] = (byte)value; }

    public override int Stat_HPCurrent { get => ReadUInt16BigEndian(Data.AsSpan(0x22)); set => WriteUInt16BigEndian(Data.AsSpan(0x22), (ushort)value); }
    public override int Stat_HPMax     { get => ReadUInt16BigEndian(Data.AsSpan(0x24)); set => WriteUInt16BigEndian(Data.AsSpan(0x24), (ushort)value); }
    public override int Stat_ATK       { get => ReadUInt16BigEndian(Data.AsSpan(0x26)); set => WriteUInt16BigEndian(Data.AsSpan(0x26), (ushort)value); }
    public override int Stat_DEF       { get => ReadUInt16BigEndian(Data.AsSpan(0x28)); set => WriteUInt16BigEndian(Data.AsSpan(0x28), (ushort)value); }
    public override int Stat_SPE       { get => ReadUInt16BigEndian(Data.AsSpan(0x2A)); set => WriteUInt16BigEndian(Data.AsSpan(0x2A), (ushort)value); }
    public override int Stat_SPA       { get => ReadUInt16BigEndian(Data.AsSpan(0x2C)); set => WriteUInt16BigEndian(Data.AsSpan(0x2C), (ushort)value); }
    public override int Stat_SPD       { get => ReadUInt16BigEndian(Data.AsSpan(0x2E)); set => WriteUInt16BigEndian(Data.AsSpan(0x2E), (ushort)value); }
    #endregion

    public override bool IsEgg { get; set; }
    public override byte OriginalTrainerFriendship { get => CurrentFriendship; set => CurrentFriendship = value; }
    public override bool HasOriginalMetLocation => CaughtData != 0;
    public override GameVersion Version { get => GameVersion.GSC; set { } }

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_2;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_2;
    public override int MaxAbilityID => Legal.MaxAbilityID_2;
    public override int MaxItemID => Legal.MaxItemID_2;

    public PK1 ConvertToPK1()
    {
        PK1 pk1 = new(Japanese);
        Array.Copy(Data, 0x1, pk1.Data, 0x7, 0x1A);
        pk1.Species = Species; // This will take care of Typing :)

        var lvl = Stat_Level;
        if (lvl == 0) // no party stats (originated from box format), need to regenerate
        {
            pk1.Stat_HPCurrent = GetStat(PersonalInfo.HP, IV_ATK, EV_ATK, Stat_Level);
            pk1.Stat_Level = CurrentLevel;
        }
        else
        {
            pk1.Stat_HPCurrent = Stat_HPCurrent;
            pk1.Stat_Level = Stat_Level;
        }
        // Status = 0
        OriginalTrainerTrash.CopyTo(pk1.OriginalTrainerTrash);
        NicknameTrash.CopyTo(pk1.NicknameTrash);

        pk1.ClearInvalidMoves();

        return pk1;
    }

    public PK7 ConvertToPK7()
    {
        var rnd = Util.Rand;
        var lang = TransferLanguage(RecentTrainerCache.Language);
        if ((lang == 1) != Japanese)
            lang = Japanese ? 1 : 2;
        var pi = PersonalTable.SM[Species];
        int ability = TransporterLogic.IsHiddenDisallowedVC2(Species) ? 0 : 2; // Hidden
        var pk7 = new PK7
        {
            EncryptionConstant = rnd.Rand32(),
            Species = Species,
            TID16 = TID16,
            CurrentLevel = CurrentLevel,
            EXP = EXP,
            MetLevel = CurrentLevel,
            Nature = Experience.GetNatureVC(EXP),
            PID = rnd.Rand32(),
            Ball = 4,
            MetDate = EncounterDate.GetDate3DS(),
            Version = HasOriginalMetLocation ? GameVersion.C : EntityConverter.VirtualConsoleSourceGen2,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,
            MetLocation = Locations.Transfer2, // "Johto region", hardcoded.
            Gender = Gender,
            IsNicknamed = false,
            Form = Form,

            CurrentHandler = 1,
            HandlingTrainerName = RecentTrainerCache.OriginalTrainerName,
            HandlingTrainerGender = RecentTrainerCache.OriginalTrainerGender,

            Language = lang,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, 7),
            OriginalTrainerName = GetTransferTrainerName(lang),
            OriginalTrainerGender = OriginalTrainerGender, // Crystal
            OriginalTrainerFriendship = pi.BaseFriendship,
            HandlingTrainerFriendship = pi.BaseFriendship,

            Ability = pi.GetAbilityAtIndex(ability),
            AbilityNumber = 1 << ability,
        };

        var special = Species is 151 or 251;
        int flawless = special ? 5 : 3;
        pk7.SetTransferIVs(flawless, rnd);
        pk7.SetTransferPID(IsShiny);
        pk7.SetTransferLocale(lang);

        if (special)
        {
            pk7.FatefulEncounter = true;
        }
        else if (IsNicknamedBank)
        {
            pk7.IsNicknamed = true;
            pk7.Nickname = Korean ? Nickname : StringConverter12Transporter.GetString(NicknameTrash, Japanese);
        }

        // Dizzy Punch cannot be transferred
        var dizzy = pk7.GetMoveIndex(146); // Dizzy Punch
        if (dizzy != -1)
        {
            pk7.SetMove(dizzy, 0);
            pk7.FixMoves();
        }

        pk7.HealPP();
        pk7.RefreshChecksum();
        return pk7;
    }

    private string GetTransferTrainerName(int lang)
    {
        if (OriginalTrainerTrash[0] == StringConverter1.TradeOTCode) // In-game Trade
            return StringConverter12Transporter.GetTradeNameGen1(lang);
        if (Korean)
            return OriginalTrainerName;
        return StringConverter12Transporter.GetString(OriginalTrainerTrash, Japanese);
    }

    public SK2 ConvertToSK2() => new(Japanese)
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
        IsEgg = IsEgg,
        Stat_Level = Stat_Level,
        PokerusState = PokerusState,
        CaughtData = CaughtData,

        // Only copies until first 0x50 terminator, but just copy everything
        Nickname = Nickname,
        OriginalTrainerName = OriginalTrainerName,
    };

    public override string GetString(ReadOnlySpan<byte> data)
    {
        if (Korean)
            return StringConverter2KOR.GetString(data);
        return StringConverter2.GetString(data, Language);
    }

    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
    {
        if (Korean)
            return StringConverter2KOR.LoadString(data, destBuffer);
        return StringConverter2.LoadString(data, destBuffer, Language);
    }

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        if (Korean)
            return StringConverter2KOR.SetString(destBuffer, value, maxLength, option);
        return StringConverter2.SetString(destBuffer, value, maxLength, Language, option);
    }
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => Korean ? StringConverter2KOR.GetTerminatorIndex(data) : TrashBytesGB.GetTerminatorIndex(data);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => Korean ? StringConverter2KOR.GetStringLength(data) : TrashBytesGB.GetStringLength(data);
    public override int GetBytesPerChar() => 1;

    /// <summary>
    /// Gets a checksum over all the entity's data using a single list to wrap all components.
    /// </summary>
    public ushort GetSingleListChecksum()
    {
        Span<byte> tmp = stackalloc byte[PokeList2.GetListLengthSingle(Japanese)];
        PokeList2.WrapSingle(this, tmp);
        return Checksums.CRC16_CCITT(tmp);
    }
}
