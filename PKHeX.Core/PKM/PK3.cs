using System;
using System.Numerics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 3 <see cref="PKM"/> format. </summary>
public sealed class PK3 : G3PKM, ISanityChecksum
{
    public override ReadOnlySpan<ushort> ExtraBytes => [0x2A, 0x2B];

    public override int SIZE_PARTY => PokeCrypto.SIZE_3PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_3STORED;
    public override EntityContext Context => EntityContext.Gen3;
    public override PersonalInfo3 PersonalInfo => PersonalTable.RS[Species];

    public PK3() : base(PokeCrypto.SIZE_3PARTY) { }
    public PK3(byte[] data) : base(DecryptParty(data)) { }

    private static byte[] DecryptParty(byte[] data)
    {
        PokeCrypto.DecryptIfEncrypted3(ref data);
        Array.Resize(ref data, PokeCrypto.SIZE_3PARTY);
        return data;
    }

    public override PK3 Clone()
    {
        // Don't use the byte[] constructor, the DecryptIfEncrypted call is based on checksum.
        // An invalid checksum will shuffle the data; we already know it's un-shuffled. Set up manually.
        PK3 pk = new();
        Data.CopyTo(pk.Data, 0);
        return pk;
    }

    private const string EggNameJapanese = "タマゴ";

    // Trash Bytes
    public override Span<byte> NicknameTrash => Data.AsSpan(0x08, 10); // no inaccessible terminator
    public override Span<byte> OriginalTrainerTrash => Data.AsSpan(0x14, 7); // no inaccessible terminator
    public override int TrashCharCountTrainer => 7;
    public override int TrashCharCountNickname => 10;

    // At top for System.Reflection execution order hack

    // 0x20 Intro
    public override uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0x00)); set => WriteUInt32LittleEndian(Data.AsSpan(0x00), value); }
    public override uint ID32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x04)); set => WriteUInt32LittleEndian(Data.AsSpan(0x04), value); }
    public override ushort TID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(0x04), value); }
    public override ushort SID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x06)); set => WriteUInt16LittleEndian(Data.AsSpan(0x06), value); }
    public override string Nickname
    {
        get => StringConverter3.GetString(NicknameTrash, Language);
        set => StringConverter3.SetString(NicknameTrash, IsEgg ? EggNameJapanese : value, 10, Language, StringConverterOption.None);
    }
    public override int Language { get => Data[0x12]; set => Data[0x12] = (byte)value; }
    public bool FlagIsBadEgg   { get => (Data[0x13] & 1) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~1) | (value ? 1 : 0)); }
    public bool FlagHasSpecies { get => (Data[0x13] & 2) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~2) | (value ? 2 : 0)); }
    public bool FlagIsEgg      { get => (Data[0x13] & 4) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~4) | (value ? 4 : 0)); }
    public override string OriginalTrainerName
    {
        get => StringConverter3.GetString(OriginalTrainerTrash, Language);
        set => StringConverter3.SetString(OriginalTrainerTrash, value, 7, Language, StringConverterOption.None);
    }
    public override byte MarkingValue { get => (byte)SwapBits(Data[0x1B], 1, 2); set => Data[0x1B] = (byte)SwapBits(value, 1, 2); }
    public ushort Checksum { get => ReadUInt16LittleEndian(Data.AsSpan(0x1C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1C), value); }
    public ushort Sanity { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), value); }

    #region Block A
    public override ushort SpeciesInternal { get => ReadUInt16LittleEndian(Data.AsSpan(0x20)); set => WriteUInt16LittleEndian(Data.AsSpan(0x20), value); } // raw access

    public override ushort Species
    {
        get => SpeciesConverter.GetNational3(SpeciesInternal);
        set
        {
            var s3 = SpeciesConverter.GetInternal3(value);
            FlagHasSpecies = (SpeciesInternal = s3) != 0;
        }
    }

    public override int SpriteItem => ItemConverter.GetItemFuture3((ushort)HeldItem);
    public override int HeldItem { get => ReadUInt16LittleEndian(Data.AsSpan(0x22)); set => WriteUInt16LittleEndian(Data.AsSpan(0x22), (ushort)value); }

    public override uint EXP { get => ReadUInt32LittleEndian(Data.AsSpan(0x24)); set => WriteUInt32LittleEndian(Data.AsSpan(0x24), value); }
    private byte PPUps { get => Data[0x28]; set => Data[0x28] = value; }
    public override int Move1_PPUps { get => (PPUps >> 0) & 3; set => PPUps = (byte)((PPUps & ~(3 << 0)) | (value << 0)); }
    public override int Move2_PPUps { get => (PPUps >> 2) & 3; set => PPUps = (byte)((PPUps & ~(3 << 2)) | (value << 2)); }
    public override int Move3_PPUps { get => (PPUps >> 4) & 3; set => PPUps = (byte)((PPUps & ~(3 << 4)) | (value << 4)); }
    public override int Move4_PPUps { get => (PPUps >> 6) & 3; set => PPUps = (byte)((PPUps & ~(3 << 6)) | (value << 6)); }
    public override byte OriginalTrainerFriendship { get => Data[0x29]; set => Data[0x29] = value; }
    // Unused 0x2A 0x2B
    #endregion

    #region Block B
    public override ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x2C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2C), value); }
    public override ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x2E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2E), value); }
    public override ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x30)); set => WriteUInt16LittleEndian(Data.AsSpan(0x30), value); }
    public override ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x32)); set => WriteUInt16LittleEndian(Data.AsSpan(0x32), value); }
    public override int Move1_PP { get => Data[0x34]; set => Data[0x34] = (byte)value; }
    public override int Move2_PP { get => Data[0x35]; set => Data[0x35] = (byte)value; }
    public override int Move3_PP { get => Data[0x36]; set => Data[0x36] = (byte)value; }
    public override int Move4_PP { get => Data[0x37]; set => Data[0x37] = (byte)value; }
    #endregion

    #region Block C
    public override int EV_HP { get => Data[0x38]; set => Data[0x38] = (byte)value; }
    public override int EV_ATK { get => Data[0x39]; set => Data[0x39] = (byte)value; }
    public override int EV_DEF { get => Data[0x3A]; set => Data[0x3A] = (byte)value; }
    public override int EV_SPE { get => Data[0x3B]; set => Data[0x3B] = (byte)value; }
    public override int EV_SPA { get => Data[0x3C]; set => Data[0x3C] = (byte)value; }
    public override int EV_SPD { get => Data[0x3D]; set => Data[0x3D] = (byte)value; }
    public override byte ContestCool   { get => Data[0x3E]; set => Data[0x3E] = value; }
    public override byte ContestBeauty { get => Data[0x3F]; set => Data[0x3F] = value; }
    public override byte ContestCute   { get => Data[0x40]; set => Data[0x40] = value; }
    public override byte ContestSmart  { get => Data[0x41]; set => Data[0x41] = value; }
    public override byte ContestTough  { get => Data[0x42]; set => Data[0x42] = value; }
    public override byte ContestSheen  { get => Data[0x43]; set => Data[0x43] = value; }
    #endregion

    #region Block D
    public byte PokerusState { get => Data[0x44]; set => Data[0x44] = value; }
    public override int PokerusDays { get => PokerusState & 0xF; set => PokerusState = (byte)((PokerusState & ~0xF) | value); }
    public override int PokerusStrain { get => PokerusState >> 4; set => PokerusState = (byte)((PokerusState & 0xF) | (value << 4)); }
    public override ushort MetLocation { get => Data[0x45]; set => Data[0x45] = (byte)value; }
    // Origins
    private ushort Origins { get => ReadUInt16LittleEndian(Data.AsSpan(0x46)); set => WriteUInt16LittleEndian(Data.AsSpan(0x46), value); }
    public override byte MetLevel { get => (byte)(Origins & 0x7F); set => Origins = (ushort)((Origins & ~0x7F) | value); }
    public override GameVersion Version { get => (GameVersion)((Origins >> 7) & 0xF); set => Origins = (ushort)((Origins & ~0x780) | (((byte)value & 0xF) << 7)); }
    public override byte Ball { get => (byte)((Origins >> 11) & 0xF); set => Origins = (ushort)((Origins & ~0x7800) | ((value & 0xF) << 11)); }
    public override byte OriginalTrainerGender { get => (byte)((Origins >> 15) & 1); set => Origins = (ushort)((Origins & ~(1 << 15)) | ((value & 1) << 15)); }

    public uint IV32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x48)); set => WriteUInt32LittleEndian(Data.AsSpan(0x48), value); }
    public override int IV_HP  { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
    public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
    public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
    public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
    public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
    public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }

    public override bool IsEgg
    {
        get => ((IV32 >> 30) & 1) == 1;
        set
        {
            IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0);
            FlagIsEgg = value;
            if (value)
            {
                Nickname = EggNameJapanese;
                Language = (int) LanguageID.Japanese;
            }
        }
    }

    public override bool AbilityBit { get => IV32 >> 31 == 1; set => IV32 = (IV32 & 0x7FFFFFFF) | (value ? 1u << 31 : 0u); }

    private uint RIB0 { get => ReadUInt32LittleEndian(Data.AsSpan(0x4C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x4C), value); }
    public override byte RibbonCountG3Cool        { get => (byte)((RIB0 >> 00) & 7); set => RIB0 = ((RIB0 & ~(7u << 00)) | ((uint)(value & 7) << 00)); }
    public override byte RibbonCountG3Beauty      { get => (byte)((RIB0 >> 03) & 7); set => RIB0 = ((RIB0 & ~(7u << 03)) | ((uint)(value & 7) << 03)); }
    public override byte RibbonCountG3Cute        { get => (byte)((RIB0 >> 06) & 7); set => RIB0 = ((RIB0 & ~(7u << 06)) | ((uint)(value & 7) << 06)); }
    public override byte RibbonCountG3Smart       { get => (byte)((RIB0 >> 09) & 7); set => RIB0 = ((RIB0 & ~(7u << 09)) | ((uint)(value & 7) << 09)); }
    public override byte RibbonCountG3Tough       { get => (byte)((RIB0 >> 12) & 7); set => RIB0 = ((RIB0 & ~(7u << 12)) | ((uint)(value & 7) << 12)); }
    public override bool RibbonChampionG3        { get => (RIB0 & (1 << 15)) == 1 << 15; set => RIB0 = ((RIB0 & ~(1u << 15)) | (value ? 1u << 15 : 0u)); }
    public override bool RibbonWinning           { get => (RIB0 & (1 << 16)) == 1 << 16; set => RIB0 = ((RIB0 & ~(1u << 16)) | (value ? 1u << 16 : 0u)); }
    public override bool RibbonVictory           { get => (RIB0 & (1 << 17)) == 1 << 17; set => RIB0 = ((RIB0 & ~(1u << 17)) | (value ? 1u << 17 : 0u)); }
    public override bool RibbonArtist            { get => (RIB0 & (1 << 18)) == 1 << 18; set => RIB0 = ((RIB0 & ~(1u << 18)) | (value ? 1u << 18 : 0u)); }
    public override bool RibbonEffort            { get => (RIB0 & (1 << 19)) == 1 << 19; set => RIB0 = ((RIB0 & ~(1u << 19)) | (value ? 1u << 19 : 0u)); }
    public override bool RibbonChampionBattle    { get => (RIB0 & (1 << 20)) == 1 << 20; set => RIB0 = ((RIB0 & ~(1u << 20)) | (value ? 1u << 20 : 0u)); }
    public override bool RibbonChampionRegional  { get => (RIB0 & (1 << 21)) == 1 << 21; set => RIB0 = ((RIB0 & ~(1u << 21)) | (value ? 1u << 21 : 0u)); }
    public override bool RibbonChampionNational  { get => (RIB0 & (1 << 22)) == 1 << 22; set => RIB0 = ((RIB0 & ~(1u << 22)) | (value ? 1u << 22 : 0u)); }
    public override bool RibbonCountry           { get => (RIB0 & (1 << 23)) == 1 << 23; set => RIB0 = ((RIB0 & ~(1u << 23)) | (value ? 1u << 23 : 0u)); }
    public override bool RibbonNational          { get => (RIB0 & (1 << 24)) == 1 << 24; set => RIB0 = ((RIB0 & ~(1u << 24)) | (value ? 1u << 24 : 0u)); }
    public override bool RibbonEarth             { get => (RIB0 & (1 << 25)) == 1 << 25; set => RIB0 = ((RIB0 & ~(1u << 25)) | (value ? 1u << 25 : 0u)); }
    public override bool RibbonWorld             { get => (RIB0 & (1 << 26)) == 1 << 26; set => RIB0 = ((RIB0 & ~(1u << 26)) | (value ? 1u << 26 : 0u)); }
    public override bool Unused1 { get => (RIB0 & (1 << 27)) == 1 << 27; set => RIB0 = ((RIB0 & ~(1u << 27)) | (value ? 1u << 27 : 0u)); }
    public override bool Unused2 { get => (RIB0 & (1 << 28)) == 1 << 28; set => RIB0 = ((RIB0 & ~(1u << 28)) | (value ? 1u << 28 : 0u)); }
    public override bool Unused3 { get => (RIB0 & (1 << 29)) == 1 << 29; set => RIB0 = ((RIB0 & ~(1u << 29)) | (value ? 1u << 29 : 0u)); }
    public override bool Unused4 { get => (RIB0 & (1 << 30)) == 1 << 30; set => RIB0 = ((RIB0 & ~(1u << 30)) | (value ? 1u << 30 : 0u)); }
    public override bool FatefulEncounter { get => RIB0 >> 31 == 1; set => RIB0 = (RIB0 & ~(1 << 31)) | (uint)(value ? 1 << 31 : 0); }
    public override int RibbonCount => BitOperations.PopCount(RIB0 & 0b00000111_11111111_11111111_11111111);

    #endregion

    #region Battle Stats
    public override int Status_Condition { get => ReadInt32LittleEndian(Data.AsSpan(0x50)); set => WriteInt32LittleEndian(Data.AsSpan(0x50), value); }
    public override byte Stat_Level { get => Data[0x54]; set => Data[0x54] = value; }
    public sbyte HeldMailID { get => (sbyte)Data[0x55]; set => Data[0x55] = (byte)value; }
    public override int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data.AsSpan(0x56)); set => WriteUInt16LittleEndian(Data.AsSpan(0x56), (ushort)value); }
    public override int Stat_HPMax { get => ReadUInt16LittleEndian(Data.AsSpan(0x58)); set => WriteUInt16LittleEndian(Data.AsSpan(0x58), (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16LittleEndian(Data.AsSpan(0x5A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5A), (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16LittleEndian(Data.AsSpan(0x5C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5C), (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16LittleEndian(Data.AsSpan(0x5E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5E), (ushort)value); }
    public override int Stat_SPA { get => ReadUInt16LittleEndian(Data.AsSpan(0x60)); set => WriteUInt16LittleEndian(Data.AsSpan(0x60), (ushort)value); }
    public override int Stat_SPD { get => ReadUInt16LittleEndian(Data.AsSpan(0x62)); set => WriteUInt16LittleEndian(Data.AsSpan(0x62), (ushort)value); }
    #endregion

    protected override byte[] Encrypt()
    {
        RefreshChecksum();
        return PokeCrypto.EncryptArray3(Data);
    }

    private ushort CalculateChecksum() => Checksums.Add16(Data.AsSpan()[0x20..PokeCrypto.SIZE_3STORED]);

    public override void RefreshChecksum()
    {
        FlagIsBadEgg = false;
        Checksum = CalculateChecksum();
    }

    public override bool ChecksumValid => CalculateChecksum() == Checksum;

    public PK4 ConvertToPK4()
    {
        PK4 pk4 = new() // Convert away!
        {
            PID = PID,
            Species = Species,
            TID16 = TID16,
            SID16 = SID16,
            EXP = IsEgg ? Experience.GetEXP(5, PersonalInfo.EXPGrowth) : EXP,
            Gender = EntityGender.GetFromPID(Species, PID),
            Form = Form,
            // IsEgg = false, -- already false
            OriginalTrainerFriendship = 70,
            MarkingValue = (byte)(MarkingValue & 0b1111),
            Language = Language,
            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,
            EV_SPE = EV_SPE,
            ContestCool = ContestCool,
            ContestBeauty = ContestBeauty,
            ContestCute = ContestCute,
            ContestSmart = ContestSmart,
            ContestTough = ContestTough,
            ContestSheen = ContestSheen,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,
            IV32 = IV32 & 0x3FFFFFFF, // keep low 30 bits for all IVs to copy. Nickname set later.
            Ability = Ability,
            Version = Version,
            Ball = Ball,
            PokerusState = PokerusState,
            OriginalTrainerGender = OriginalTrainerGender,
            MetDate = EncounterDate.GetDateNDS(),
            MetLevel = CurrentLevel,
            MetLocation = Locations.Transfer3, // Pal Park

            RibbonChampionG3 = RibbonChampionG3,
            RibbonWinning = RibbonWinning,
            RibbonVictory = RibbonVictory,
            RibbonArtist = RibbonArtist,
            RibbonEffort = RibbonEffort,
            RibbonChampionBattle = RibbonChampionBattle,
            RibbonChampionRegional = RibbonChampionRegional,
            RibbonChampionNational = RibbonChampionNational,
            RibbonCountry = RibbonCountry,
            RibbonNational = RibbonNational,
            RibbonEarth = RibbonEarth,
            RibbonWorld = RibbonWorld,

            // byte -> bool contest ribbons
            RibbonG3Cool         = RibbonCountG3Cool > 0,
            RibbonG3CoolSuper    = RibbonCountG3Cool > 1,
            RibbonG3CoolHyper    = RibbonCountG3Cool > 2,
            RibbonG3CoolMaster   = RibbonCountG3Cool > 3,

            RibbonG3Beauty       = RibbonCountG3Beauty > 0,
            RibbonG3BeautySuper  = RibbonCountG3Beauty > 1,
            RibbonG3BeautyHyper  = RibbonCountG3Beauty > 2,
            RibbonG3BeautyMaster = RibbonCountG3Beauty > 3,

            RibbonG3Cute         = RibbonCountG3Cute > 0,
            RibbonG3CuteSuper    = RibbonCountG3Cute > 1,
            RibbonG3CuteHyper    = RibbonCountG3Cute > 2,
            RibbonG3CuteMaster   = RibbonCountG3Cute > 3,

            RibbonG3Smart        = RibbonCountG3Smart > 0,
            RibbonG3SmartSuper   = RibbonCountG3Smart > 1,
            RibbonG3SmartHyper   = RibbonCountG3Smart > 2,
            RibbonG3SmartMaster  = RibbonCountG3Smart > 3,

            RibbonG3Tough        = RibbonCountG3Tough > 0,
            RibbonG3ToughSuper   = RibbonCountG3Tough > 1,
            RibbonG3ToughHyper   = RibbonCountG3Tough > 2,
            RibbonG3ToughMaster  = RibbonCountG3Tough > 3,

            FatefulEncounter = FatefulEncounter,
        };

        // Yay for reusing string buffers! The game allocates a buffer and reuses it when creating strings.
        // Trash from the {unknown source} is currently in buffer. Set it to the Nickname region.
        var trash = StringConverter345.GetTrashBytes(pk4.Language);
        var nickTrash = pk4.NicknameTrash;
        trash.CopyTo(nickTrash[4..]); // min of 1 char and terminator, ignore first 2.

        if (IsEgg)
            pk4.Nickname = SpeciesName.GetSpeciesNameGeneration(pk4.Species, pk4.Language, 4);
        else
            StringConverter345.TransferGlyphs34(NicknameTrash, Language, Japanese ? 5 : 10, nickTrash);
        pk4.IsNicknamed = !IsEgg && IsNicknamed;

        // Trash from the current string (Nickname) is in our string buffer. Slap the OT name over-top.
        var destOT = pk4.OriginalTrainerTrash;
        nickTrash[..destOT.Length].CopyTo(destOT);
        StringConverter345.TransferGlyphs34(OriginalTrainerTrash, Language, Japanese ? 5 : 7, destOT);

        var item = (ushort)HeldItem;
        if (item != 0)
        {
            item = ItemConverter.GetItemFuture3(item);
            if (ItemConverter.IsItemTransferable34(item))
                pk4.HeldItem = item;
        }

        // Remove HM moves
        ReadOnlySpan<ushort> banned = LearnSource3.HM_3;
        if (banned.Contains(Move1)) pk4.Move1 = 0;
        if (banned.Contains(Move2)) pk4.Move2 = 0;
        if (banned.Contains(Move3)) pk4.Move3 = 0;
        if (banned.Contains(Move4)) pk4.Move4 = 0;
        pk4.FixMoves();
        pk4.HealPP();

        pk4.RefreshChecksum();
        return pk4;
    }

    // Use Japanese since the JPN GC/GBA string conversion table is less lossy than INT
    private const GCRegion GCRegionTemp = GCRegion.NTSC_J;

    public XK3 ConvertToXK3()
    {
        var pk = ConvertTo<XK3>();
        // Set these even if the settings don't SetPKM
        pk.CurrentRegion = GCRegionTemp;
        pk.OriginalRegion = GCRegionTemp;

        StringConverter3GC.RemapGlyphs3GC(NicknameTrash, GCRegionTemp, Language, pk.NicknameTrash);
        StringConverter3GC.RemapGlyphs3GC(OriginalTrainerTrash, GCRegionTemp, Language, pk.OriginalTrainerTrash);
        pk.ResetNicknameDisplay();

        pk.ResetPartyStats();
        return pk;
    }

    public CK3 ConvertToCK3()
    {
        var pk = ConvertTo<CK3>();
        // Set these even if the settings don't SetPKM
        pk.CurrentRegion = GCRegionTemp;
        pk.OriginalRegion = GCRegionTemp;

        StringConverter3GC.RemapGlyphs3GC(NicknameTrash, GCRegionTemp, Language, pk.NicknameTrash);
        StringConverter3GC.RemapGlyphs3GC(OriginalTrainerTrash, GCRegionTemp, Language, pk.OriginalTrainerTrash);
        pk.ResetNicknameDisplay();

        pk.ResetPartyStats();
        return pk;
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter3.GetString(data, Language);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter3.LoadString(data, destBuffer, Language);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter3.SetString(destBuffer, value, maxLength, Language, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytes8.GetTerminatorIndex(data);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytes8.GetStringLength(data);
    public override int GetBytesPerChar() => 1;
}
