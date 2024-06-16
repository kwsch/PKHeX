using System;
using System.Numerics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 3 <see cref="PKM"/> format, exclusively for Pokémon XD. </summary>
public sealed class XK3 : G3PKM, IShadowCapture
{
    public override ReadOnlySpan<ushort> ExtraBytes =>
    [
        0x0A, 0x0B, 0x0C, 0x0D, 0x1E, 0x1F,
        0x2A, 0x2B,
        0x7A, 0x7B,
        0x7E, 0x7F,
    ];

    public override int SIZE_PARTY => PokeCrypto.SIZE_3XSTORED;
    public override int SIZE_STORED => PokeCrypto.SIZE_3XSTORED;
    public override EntityContext Context => EntityContext.Gen3;
    public override PersonalInfo3 PersonalInfo => PersonalTable.RS[Species];
    public XK3(byte[] data) : base(data) { }
    public XK3() : base(PokeCrypto.SIZE_3XSTORED) { }
    public override XK3 Clone() => new((byte[])Data.Clone()) { Purification = Purification };
    public override void RefreshChecksum() => Valid = true;

    // Trash Bytes
    public override Span<byte> OriginalTrainerTrash => Data.AsSpan(0x38, 22);
    public Span<byte> NicknameDisplayTrash => Data.AsSpan(0x4E, 22);
    public override Span<byte> NicknameTrash => Data.AsSpan(0x64, 22);
    public override int TrashCharCountTrainer => 11;
    public override int TrashCharCountNickname => 11;

    public override ushort SpeciesInternal { get => ReadUInt16BigEndian(Data.AsSpan(0x00)); set => WriteUInt16BigEndian(Data.AsSpan(0x00), value); } // raw access
    public override ushort Species { get => SpeciesConverter.GetNational3(SpeciesInternal); set => SpeciesInternal = SpeciesConverter.GetInternal3(value); }
    public override int SpriteItem => ItemConverter.GetItemFuture3((ushort)HeldItem);
    public override int HeldItem { get => ReadUInt16BigEndian(Data.AsSpan(0x02)); set => WriteUInt16BigEndian(Data.AsSpan(0x02), (ushort)value); }
    public override int Stat_HPCurrent { get => ReadUInt16BigEndian(Data.AsSpan(0x04)); set => WriteUInt16BigEndian(Data.AsSpan(0x04), (ushort)value); }
    public override byte OriginalTrainerFriendship { get => (byte)ReadUInt16BigEndian(Data.AsSpan(0x06)); set => WriteUInt16BigEndian(Data.AsSpan(0x06), value); }
    public override ushort MetLocation { get => ReadUInt16BigEndian(Data.AsSpan(0x08)); set => WriteUInt16BigEndian(Data.AsSpan(0x08), value); }
    // 0x0A-0x0B Unknown
    // 0x0C-0x0D Unknown
    public override byte MetLevel { get => Data[0x0E]; set => Data[0x0E] = value; }
    public override byte Ball { get => Data[0x0F]; set => Data[0x0F] = value; }
    public override byte OriginalTrainerGender { get => Data[0x10]; set => Data[0x10] = value; }
    public override byte Stat_Level { get => Data[0x11]; set => Data[0x11] = value; }
    public override byte ContestSheen { get => Data[0x12]; set => Data[0x12] = value; }
    public override int PokerusStrain { get => Data[0x13] & 0xF; set => Data[0x13] = (byte)(value & 0xF); }
    public override byte MarkingValue { get => (byte)SwapBits(Data[0x14], 1, 2); set => Data[0x14] = (byte)SwapBits(value, 1, 2); }
    public override int PokerusDays { get => Math.Max((sbyte)Data[0x15], (sbyte)0); set => Data[0x15] = (byte)(value == 0 ? 0xFF : value & 0xF); }
    // 0x16-0x1C Battle Related
    private int XDPKMFLAGS { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
    public bool UnusedFlag0         { get => (XDPKMFLAGS & (1 << 0)) == 1 << 0; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 0)) | (value ? 1 << 0 : 0); }
    public bool UnusedFlag1         { get => (XDPKMFLAGS & (1 << 1)) == 1 << 1; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 1)) | (value ? 1 << 1 : 0); }
    public bool CapturedFlag        { get => (XDPKMFLAGS & (1 << 2)) == 1 << 2; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 2)) | (value ? 1 << 2 : 0); }
    public bool UnusedFlag3         { get => (XDPKMFLAGS & (1 << 3)) == 1 << 3; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 3)) | (value ? 1 << 3 : 0); }
    public bool BlockTrades         { get => (XDPKMFLAGS & (1 << 4)) == 1 << 4; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 4)) | (value ? 1 << 4 : 0); }
    public override bool Valid      { get => (XDPKMFLAGS & (1 << 5)) == 0;      set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 5)) | (value ? 0 : 1 << 5); } // invalid flag
    public override bool AbilityBit { get => (XDPKMFLAGS & (1 << 6)) == 1 << 6; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 6)) | (value ? 1 << 6 : 0); }
    public override bool IsEgg      { get => (XDPKMFLAGS & (1 << 7)) == 1 << 7; set => XDPKMFLAGS = (XDPKMFLAGS & ~(1 << 7)) | (value ? 1 << 7 : 0); }
    // 0x1E-0x1F Unknown
    public override uint EXP { get => ReadUInt32BigEndian(Data.AsSpan(0x20)); set => WriteUInt32BigEndian(Data.AsSpan(0x20), value); }
    public override uint ID32 { get => ReadUInt32BigEndian(Data.AsSpan(0x24)); set => WriteUInt32BigEndian(Data.AsSpan(0x24), value); }
    public override ushort SID16 { get => ReadUInt16BigEndian(Data.AsSpan(0x24)); set => WriteUInt16BigEndian(Data.AsSpan(0x24), value); }
    public override ushort TID16 { get => ReadUInt16BigEndian(Data.AsSpan(0x26)); set => WriteUInt16BigEndian(Data.AsSpan(0x26), value); }
    public override uint PID { get => ReadUInt32BigEndian(Data.AsSpan(0x28)); set => WriteUInt32BigEndian(Data.AsSpan(0x28), value); }
    // 0x2A-0x2B Unknown
    // 0x2C-0x2F Battle Related
    public bool Obedient { get => Data[0x30] == 1; set => Data[0x30] = value ? (byte)1 : (byte)0; }
    // 0x31-0x32 Unknown
    public int EncounterInfo { get => Data[0x33]; set => Data[0x33] = (byte)value; }

    public override bool FatefulEncounter
    {
        get => EncounterInfo != 0 || Obedient;
        set
        {
            if (EncounterInfo != 0)
            {
                if (!value)
                    EncounterInfo = 0;
                return;
            }
            EncounterInfo = (byte) ((EncounterInfo & ~(1 << 0)) | (value ? 1 << 0 : 0));
        }
    }

    public override GameVersion Version { get => GetGBAVersionID((GCVersion)Data[0x34]); set => Data[0x34] = (byte)GetGCVersionID(value); }
    public GCRegion CurrentRegion { get => (GCRegion)Data[0x35]; set => Data[0x35] = (byte)value; }
    public GCRegion OriginalRegion { get => (GCRegion)Data[0x36]; set => Data[0x36] = (byte)value; }
    public override int Language { get => Core.Language.GetMainLangIDfromGC(Data[0x37]); set => Data[0x37] = Core.Language.GetGCLangIDfromMain((byte)value); }
    public override string OriginalTrainerName { get => GetString(OriginalTrainerTrash); set => SetString(OriginalTrainerTrash, value, 10, StringConverterOption.None); }
    public string NicknameDisplay { get => GetString(NicknameDisplayTrash); set => SetString(NicknameDisplayTrash, value, 10, StringConverterOption.None); }
    public override string Nickname { get => GetString(NicknameTrash); set { SetString(NicknameTrash, value, 10, StringConverterOption.None); ResetNicknameDisplay(); } }

    public void ResetNicknameDisplay()
    {
        var current = NicknameDisplayTrash;
        NicknameTrash.CopyTo(current);
        if (CurrentRegion == GCRegion.NTSC_J)
            current[10..].Clear(); // clamp to 5 chars at most
    }

    // 0x7A-0x7B Unknown
    private ushort RIB0 { get => ReadUInt16BigEndian(Data.AsSpan(0x7C)); set => WriteUInt16BigEndian(Data.AsSpan(0x7C), value); }
    public override bool RibbonChampionG3        { get => (RIB0 & (1 << 15)) == 1 << 15; set => RIB0 = (ushort)((RIB0 & ~(1 << 15)) | (value ? 1 << 15 : 0)); }
    public override bool RibbonWinning           { get => (RIB0 & (1 << 14)) == 1 << 14; set => RIB0 = (ushort)((RIB0 & ~(1 << 14)) | (value ? 1 << 14 : 0)); }
    public override bool RibbonVictory           { get => (RIB0 & (1 << 13)) == 1 << 13; set => RIB0 = (ushort)((RIB0 & ~(1 << 13)) | (value ? 1 << 13 : 0)); }
    public override bool RibbonArtist            { get => (RIB0 & (1 << 12)) == 1 << 12; set => RIB0 = (ushort)((RIB0 & ~(1 << 12)) | (value ? 1 << 12 : 0)); }
    public override bool RibbonEffort            { get => (RIB0 & (1 << 11)) == 1 << 11; set => RIB0 = (ushort)((RIB0 & ~(1 << 11)) | (value ? 1 << 11 : 0)); }
    public override bool RibbonChampionBattle    { get => (RIB0 & (1 << 10)) == 1 << 10; set => RIB0 = (ushort)((RIB0 & ~(1 << 10)) | (value ? 1 << 10 : 0)); }
    public override bool RibbonChampionRegional  { get => (RIB0 & (1 << 09)) == 1 << 09; set => RIB0 = (ushort)((RIB0 & ~(1 << 09)) | (value ? 1 << 09 : 0)); }
    public override bool RibbonChampionNational  { get => (RIB0 & (1 << 08)) == 1 << 08; set => RIB0 = (ushort)((RIB0 & ~(1 << 08)) | (value ? 1 << 08 : 0)); }
    public override bool RibbonCountry           { get => (RIB0 & (1 << 07)) == 1 << 07; set => RIB0 = (ushort)((RIB0 & ~(1 << 07)) | (value ? 1 << 07 : 0)); }
    public override bool RibbonNational          { get => (RIB0 & (1 << 06)) == 1 << 06; set => RIB0 = (ushort)((RIB0 & ~(1 << 06)) | (value ? 1 << 06 : 0)); }
    public override bool RibbonEarth             { get => (RIB0 & (1 << 05)) == 1 << 05; set => RIB0 = (ushort)((RIB0 & ~(1 << 05)) | (value ? 1 << 05 : 0)); }
    public override bool RibbonWorld             { get => (RIB0 & (1 << 04)) == 1 << 04; set => RIB0 = (ushort)((RIB0 & ~(1 << 04)) | (value ? 1 << 04 : 0)); }
    public override bool Unused1                 { get => (RIB0 & (1 << 03)) == 1 << 03; set => RIB0 = (ushort)((RIB0 & ~(1 << 03)) | (value ? 1 << 03 : 0)); }
    public override bool Unused2                 { get => (RIB0 & (1 << 02)) == 1 << 02; set => RIB0 = (ushort)((RIB0 & ~(1 << 02)) | (value ? 1 << 02 : 0)); }
    public override bool Unused3                 { get => (RIB0 & (1 << 01)) == 1 << 01; set => RIB0 = (ushort)((RIB0 & ~(1 << 01)) | (value ? 1 << 01 : 0)); }
    public override bool Unused4                 { get => (RIB0 & (1 << 00)) == 1 << 00; set => RIB0 = (ushort)((RIB0 & ~(1 << 00)) | (value ? 1 << 00 : 0)); }
    public override int RibbonCount => BitOperations.PopCount((uint)(RIB0 & 0b1111_1111_1111_0000)) + RibbonCountG3Cool + RibbonCountG3Beauty + RibbonCountG3Cute + RibbonCountG3Smart + RibbonCountG3Tough;
    // 0x7E-0x7F Unknown

    // Moves
    public override ushort Move1 { get => ReadUInt16BigEndian(Data.AsSpan(0x80)); set => WriteUInt16BigEndian(Data.AsSpan(0x80), value); }
    public override int Move1_PP { get => Data[0x82]; set => Data[0x82] = (byte)value; }
    public override int Move1_PPUps { get => Data[0x83]; set => Data[0x83] = (byte)value; }
    public override ushort Move2 { get => ReadUInt16BigEndian(Data.AsSpan(0x84)); set => WriteUInt16BigEndian(Data.AsSpan(0x84), value); }
    public override int Move2_PP { get => Data[0x86]; set => Data[0x86] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x87]; set => Data[0x87] = (byte)value; }
    public override ushort Move3 { get => ReadUInt16BigEndian(Data.AsSpan(0x88)); set => WriteUInt16BigEndian(Data.AsSpan(0x88), value); }
    public override int Move3_PP { get => Data[0x8A]; set => Data[0x8A] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x8B]; set => Data[0x8B] = (byte)value; }
    public override ushort Move4 { get => ReadUInt16BigEndian(Data.AsSpan(0x8C)); set => WriteUInt16BigEndian(Data.AsSpan(0x8C), value); }
    public override int Move4_PP { get => Data[0x8E]; set => Data[0x8E] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x8F]; set => Data[0x8F] = (byte)value; }

    // More party stats
    public override int Stat_HPMax { get => ReadUInt16BigEndian(Data.AsSpan(0x90)); set => WriteUInt16BigEndian(Data.AsSpan(0x90), (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16BigEndian(Data.AsSpan(0x92)); set => WriteUInt16BigEndian(Data.AsSpan(0x92), (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16BigEndian(Data.AsSpan(0x94)); set => WriteUInt16BigEndian(Data.AsSpan(0x94), (ushort)value); }
    public override int Stat_SPA { get => ReadUInt16BigEndian(Data.AsSpan(0x96)); set => WriteUInt16BigEndian(Data.AsSpan(0x96), (ushort)value); }
    public override int Stat_SPD { get => ReadUInt16BigEndian(Data.AsSpan(0x98)); set => WriteUInt16BigEndian(Data.AsSpan(0x98), (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16BigEndian(Data.AsSpan(0x9A)); set => WriteUInt16BigEndian(Data.AsSpan(0x9A), (ushort)value); }

    // EVs
    public override int EV_HP
    {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0x9C)));
        set => WriteUInt16BigEndian(Data.AsSpan(0x9C), (ushort)(value & 0xFF));
    }

    public override int EV_ATK
    {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0x9E)));
        set => WriteUInt16BigEndian(Data.AsSpan(0x9E), (ushort)(value & 0xFF));
    }

    public override int EV_DEF
    {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0xA0)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA0), (ushort)(value & 0xFF));
    }

    public override int EV_SPA
    {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0xA2)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA2), (ushort)(value & 0xFF));
    }

    public override int EV_SPD
    {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0xA4)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA4), (ushort)(value & 0xFF));
    }

    public override int EV_SPE
    {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0xA6)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA6), (ushort)(value & 0xFF));
    }

    // IVs
    public override int IV_HP { get => Data[0xA8]; set => Data[0xA8] = (byte)(value & 0x1F); }
    public override int IV_ATK { get => Data[0xA9]; set => Data[0xA9] = (byte)(value & 0x1F); }
    public override int IV_DEF { get => Data[0xAA]; set => Data[0xAA] = (byte)(value & 0x1F); }
    public override int IV_SPA { get => Data[0xAB]; set => Data[0xAB] = (byte)(value & 0x1F); }
    public override int IV_SPD { get => Data[0xAC]; set => Data[0xAC] = (byte)(value & 0x1F); }
    public override int IV_SPE { get => Data[0xAD]; set => Data[0xAD] = (byte)(value & 0x1F); }

    // Contest
    public override byte ContestCool   { get => Data[0xAE]; set => Data[0xAE] = value; }
    public override byte ContestBeauty { get => Data[0xAF]; set => Data[0xAF] = value; }
    public override byte ContestCute   { get => Data[0xB0]; set => Data[0xB0] = value; }
    public override byte ContestSmart  { get => Data[0xB1]; set => Data[0xB1] = value; }
    public override byte ContestTough  { get => Data[0xB2]; set => Data[0xB2] = value; }
    public override byte RibbonCountG3Cool   { get => Data[0xB3]; set => Data[0xB3] = value; }
    public override byte RibbonCountG3Beauty { get => Data[0xB4]; set => Data[0xB4] = value; }
    public override byte RibbonCountG3Cute   { get => Data[0xB5]; set => Data[0xB5] = value; }
    public override byte RibbonCountG3Smart  { get => Data[0xB6]; set => Data[0xB6] = value; }
    public override byte RibbonCountG3Tough  { get => Data[0xB7]; set => Data[0xB7] = value; }

    public ushort ShadowID { get => ReadUInt16BigEndian(Data.AsSpan(0xBA)); set => WriteUInt16BigEndian(Data.AsSpan(0xBA), value); }

    // Purification information is stored in the save file and accessed based on the Shadow ID.
    public int Purification { get; set; }

    // stored in the data, offset undocumented
    public override int Status_Condition { get; set; }

    public bool IsShadow { get; internal set; } // determined by savedata, not written back to sav

    protected override byte[] Encrypt()
    {
        return (byte[])Data.Clone();
    }

    public PK3 ConvertToPK3()
    {
        var pk = ConvertTo<PK3>();
        StringConverter3GC.RemapGlyphs3GBA(NicknameTrash, CurrentRegion, Language, pk.NicknameTrash);
        StringConverter3GC.RemapGlyphs3GBA(OriginalTrainerTrash, CurrentRegion, Language, pk.OriginalTrainerTrash);
        if (Version == GameVersion.CXD)
        {
            // Transferring XK3 to PK3 when it originates from XD sets the fateful encounter (obedience) flag.
            if (ShadowID != 0)
            {
                pk.RibbonNational = true; // must be purified before trading away; force purify
                pk.FatefulEncounter = true;
            }
            else if (IsGiftXD(MetLocation))
            {
                pk.FatefulEncounter = true;
            }
        }
        pk.FlagHasSpecies = pk.SpeciesInternal != 0; // Update Flag
        pk.RefreshChecksum();
        return pk;
    }

    private static bool IsGiftXD(ushort met) => met switch
    {
        0 or 16 => true, // Starter Eevee / Hordel Gift
        90 or 91 or 92 => true, // Pokespot: Rock / Oasis / Cave
        _ => false,
    };

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter3GC.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter3GC.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter3GC.SetString(destBuffer, value, maxLength, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetTerminatorIndex(data, StringConverter3GC.TerminatorBigEndian);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetStringLength(data, StringConverter3GC.TerminatorBigEndian);
    public override int GetBytesPerChar() => 2;
}
