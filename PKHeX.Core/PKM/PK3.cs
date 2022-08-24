using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 3 <see cref="PKM"/> format. </summary>
public sealed class PK3 : G3PKM, ISanityChecksum
{
    private static readonly ushort[] Unused =
    {
        0x2A, 0x2B,
    };

    public override int SIZE_PARTY => PokeCrypto.SIZE_3PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_3STORED;
    public override EntityContext Context => EntityContext.Gen3;
    public override PersonalInfo PersonalInfo => PersonalTable.RS[Species];

    public override IReadOnlyList<ushort> ExtraBytes => Unused;

    public PK3() : base(PokeCrypto.SIZE_3PARTY) { }
    public PK3(byte[] data) : base(DecryptParty(data)) { }

    private static byte[] DecryptParty(byte[] data)
    {
        PokeCrypto.DecryptIfEncrypted3(ref data);
        Array.Resize(ref data, PokeCrypto.SIZE_3PARTY);
        return data;
    }

    public override PKM Clone()
    {
        // Don't use the byte[] constructor, the DecryptIfEncrypted call is based on checksum.
        // An invalid checksum will shuffle the data; we already know it's un-shuffled. Set up manually.
        var pk = new PK3();
        Data.CopyTo(pk.Data, 0);
        return pk;
    }

    private const string EggNameJapanese = "タマゴ";

    // Trash Bytes
    public override Span<byte> Nickname_Trash => Data.AsSpan(0x08, 10); // no inaccessible terminator
    public override Span<byte> OT_Trash => Data.AsSpan(0x14, 7); // no inaccessible terminator

    // At top for System.Reflection execution order hack

    // 0x20 Intro
    public override uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0x00)); set => WriteUInt32LittleEndian(Data.AsSpan(0x00), value); }
    public override int TID { get => ReadUInt16LittleEndian(Data.AsSpan(0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(0x04), (ushort)value); }
    public override int SID { get => ReadUInt16LittleEndian(Data.AsSpan(0x06)); set => WriteUInt16LittleEndian(Data.AsSpan(0x06), (ushort)value); }
    public override string Nickname
    {
        get => StringConverter3.GetString(Nickname_Trash, Japanese);
        set => StringConverter3.SetString(Nickname_Trash, (IsEgg ? EggNameJapanese : value).AsSpan(), 10, Japanese, StringConverterOption.None);
    }
    public override int Language { get => Data[0x12]; set => Data[0x12] = (byte)value; }
    public bool FlagIsBadEgg   { get => (Data[0x13] & 1) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~1) | (value ? 1 : 0)); }
    public bool FlagHasSpecies { get => (Data[0x13] & 2) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~2) | (value ? 2 : 0)); }
    public bool FlagIsEgg      { get => (Data[0x13] & 4) != 0; set => Data[0x13] = (byte)((Data[0x13] & ~4) | (value ? 4 : 0)); }
    public override string OT_Name
    {
        get => StringConverter3.GetString(OT_Trash, Japanese);
        set => StringConverter3.SetString(OT_Trash, value.AsSpan(), 7, Japanese, StringConverterOption.None);
    }
    public override int MarkValue { get => SwapBits(Data[0x1B], 1, 2); set => Data[0x1B] = (byte)SwapBits(value, 1, 2); }
    public ushort Checksum { get => ReadUInt16LittleEndian(Data.AsSpan(0x1C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1C), value); }
    public ushort Sanity { get => ReadUInt16LittleEndian(Data.AsSpan(0x1E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), value); }

    #region Block A
    public override ushort SpeciesID3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x20)); set => WriteUInt16LittleEndian(Data.AsSpan(0x20), value); } // raw access

    public override int Species
    {
        get => SpeciesConverter.GetG4Species(SpeciesID3);
        set
        {
            var s3 = SpeciesConverter.GetG3Species(value);
            FlagHasSpecies = (SpeciesID3 = s3) != 0;
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
    public override int OT_Friendship { get => Data[0x29]; set => Data[0x29] = (byte)value; }
    // Unused 0x2A 0x2B
    #endregion

    #region Block B
    public override int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x2C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2C), (ushort)value); }
    public override int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x2E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2E), (ushort)value); }
    public override int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x30)); set => WriteUInt16LittleEndian(Data.AsSpan(0x30), (ushort)value); }
    public override int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x32)); set => WriteUInt16LittleEndian(Data.AsSpan(0x32), (ushort)value); }
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
    public override byte CNT_Cool   { get => Data[0x3E]; set => Data[0x3E] = value; }
    public override byte CNT_Beauty { get => Data[0x3F]; set => Data[0x3F] = value; }
    public override byte CNT_Cute   { get => Data[0x40]; set => Data[0x40] = value; }
    public override byte CNT_Smart  { get => Data[0x41]; set => Data[0x41] = value; }
    public override byte CNT_Tough  { get => Data[0x42]; set => Data[0x42] = value; }
    public override byte CNT_Sheen  { get => Data[0x43]; set => Data[0x43] = value; }
    #endregion

    #region Block D
    private byte PKRS { get => Data[0x44]; set => Data[0x44] = value; }
    public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
    public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | (value << 4)); }
    public override int Met_Location { get => Data[0x45]; set => Data[0x45] = (byte)value; }
    // Origins
    private ushort Origins { get => ReadUInt16LittleEndian(Data.AsSpan(0x46)); set => WriteUInt16LittleEndian(Data.AsSpan(0x46), value); }
    public override int Met_Level { get => Origins & 0x7F; set => Origins = (ushort)((Origins & ~0x7F) | value); }
    public override int Version { get => (Origins >> 7) & 0xF; set => Origins = (ushort)((Origins & ~0x780) | ((value & 0xF) << 7)); }
    public override int Ball { get => (Origins >> 11) & 0xF; set => Origins = (ushort)((Origins & ~0x7800) | ((value & 0xF) << 11)); }
    public override int OT_Gender { get => (Origins >> 15) & 1; set => Origins = (ushort)((Origins & ~(1 << 15)) | ((value & 1) << 15)); }

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
    #endregion

    #region Battle Stats
    public override int Status_Condition { get => ReadInt32LittleEndian(Data.AsSpan(0x50)); set => WriteInt32LittleEndian(Data.AsSpan(0x50), value); }
    public override int Stat_Level { get => Data[0x54]; set => Data[0x54] = (byte)value; }
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

    public override void RefreshChecksum()
    {
        FlagIsBadEgg = false;
        Checksum = PokeCrypto.GetCHK3(Data);
    }

    public override bool ChecksumValid => CalculateChecksum() == Checksum;
    private ushort CalculateChecksum() => PokeCrypto.GetCHK3(Data);

    public PK4 ConvertToPK4()
    {
        PK4 pk4 = new() // Convert away!
        {
            PID = PID,
            Species = Species,
            TID = TID,
            SID = SID,
            EXP = IsEgg ? Experience.GetEXP(5, PersonalInfo.EXPGrowth) : EXP,
            Gender = EntityGender.GetFromPID(Species, PID),
            Form = Form,
            // IsEgg = false, -- already false
            OT_Friendship = 70,
            MarkValue = MarkValue & 0b1111,
            Language = Language,
            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,
            EV_SPE = EV_SPE,
            CNT_Cool = CNT_Cool,
            CNT_Beauty = CNT_Beauty,
            CNT_Cute = CNT_Cute,
            CNT_Smart = CNT_Smart,
            CNT_Tough = CNT_Tough,
            CNT_Sheen = CNT_Sheen,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,
            IV_HP = IV_HP,
            IV_ATK = IV_ATK,
            IV_DEF = IV_DEF,
            IV_SPA = IV_SPA,
            IV_SPD = IV_SPD,
            IV_SPE = IV_SPE,
            Ability = Ability,
            Version = Version,
            Ball = Ball,
            PKRS_Strain = PKRS_Strain,
            PKRS_Days = PKRS_Days,
            OT_Gender = OT_Gender,
            MetDate = DateTime.Now,
            Met_Level = CurrentLevel,
            Met_Location = Locations.Transfer3, // Pal Park

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
        var trash = StringConverter345.G4TransferTrashBytes;
        if (pk4.Language < trash.Length)
            trash[pk4.Language].CopyTo(pk4.Data, 0x48 + 4);
        pk4.Nickname = IsEgg ? SpeciesName.GetSpeciesNameGeneration(pk4.Species, pk4.Language, 4) : Nickname;
        pk4.IsNicknamed = !IsEgg && IsNicknamed;

        // Trash from the current string (Nickname) is in our string buffer. Slap the OT name over-top.
        Buffer.BlockCopy(pk4.Data, 0x48, pk4.Data, 0x68, 0x10);
        pk4.OT_Name = OT_Name;

        if (HeldItem > 0)
        {
            ushort item = ItemConverter.GetItemFuture3((ushort)HeldItem);
            if (ItemConverter.IsItemTransferable34(item))
                pk4.HeldItem = item;
        }

        // Remove HM moves
        ReadOnlySpan<int> banned = LearnSource3.HM_3;
        if (banned.Contains(Move1)) pk4.Move1 = 0;
        if (banned.Contains(Move2)) pk4.Move2 = 0;
        if (banned.Contains(Move3)) pk4.Move3 = 0;
        if (banned.Contains(Move4)) pk4.Move4 = 0;
        pk4.FixMoves();
        pk4.HealPP();

        pk4.RefreshChecksum();
        return pk4;
    }

    public XK3 ConvertToXK3()
    {
        var pk = ConvertTo<XK3>();
        // Set these even if the settings don't SetPKM
        pk.CurrentRegion = 2; // NTSC-U
        pk.OriginalRegion = 2; // NTSC-U
        pk.ResetPartyStats();
        return pk;
    }

    public CK3 ConvertToCK3()
    {
        var pk = ConvertTo<CK3>();
        // Set these even if the settings don't SetPKM
        pk.CurrentRegion = 2; // NTSC-U
        pk.OriginalRegion = 2; // NTSC-U
        pk.ResetPartyStats();
        return pk;
    }
}
