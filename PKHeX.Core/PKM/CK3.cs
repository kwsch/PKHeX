using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 3 <see cref="PKM"/> format, exclusively for Pokémon Colosseum. </summary>
public sealed class CK3(byte[] Data) : G3PKM(Data), IShadowCapture
{
    public CK3() : this(new byte[PokeCrypto.SIZE_3CSTORED]) { }

    public override ReadOnlySpan<ushort> ExtraBytes =>
    [
        0x11, 0x12, 0x13,
        0x61, 0x62, 0x63, 0x64,
        0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xDA, 0xDB,
        0xE4, 0xE5, 0xE6, 0xE7, 0xCE,
        0xD7, // index within party
        // 0xFC onwards unused? no, it's some pointers and values used by the game?
    ];

    public override int SIZE_PARTY => PokeCrypto.SIZE_3CSTORED;
    public override int SIZE_STORED => PokeCrypto.SIZE_3CSTORED;
    public override EntityContext Context => EntityContext.Gen3;
    public override PersonalInfo3 PersonalInfo => PersonalTable.RS[Species];
    public override CK3 Clone() => new((byte[])Data.Clone());

    // Trash Bytes
    public override Span<byte> OriginalTrainerTrash => Data.AsSpan(0x18, 22);
    public Span<byte> NicknameDisplayTrash => Data.AsSpan(0x2E, 22);
    public override Span<byte> NicknameTrash => Data.AsSpan(0x44, 22);
    public override int TrashCharCountTrainer => 11;
    public override int TrashCharCountNickname => 11;

    // Future Attributes
    public override ushort SpeciesInternal { get => ReadUInt16BigEndian(Data.AsSpan(0x00)); set => WriteUInt16BigEndian(Data.AsSpan(0x00), value); } // raw access
    public override ushort Species { get => SpeciesConverter.GetNational3(SpeciesInternal); set => SpeciesInternal = SpeciesConverter.GetInternal3(value); }
    // 02-04 unused
    public override uint PID { get => ReadUInt32BigEndian(Data.AsSpan(0x04)); set => WriteUInt32BigEndian(Data.AsSpan(0x04), value); }
    public override GameVersion Version { get => GetGBAVersionID((GCVersion)Data[0x08]); set => Data[0x08] = (byte)GetGCVersionID(value); }
    public GCRegion CurrentRegion { get => (GCRegion)Data[0x09]; set => Data[0x09] = (byte)value; }
    public GCRegion OriginalRegion { get => (GCRegion)Data[0x0A]; set => Data[0x0A] = (byte)value; }
    public override int Language { get => Core.Language.GetMainLangIDfromGC(Data[0x0B]); set => Data[0x0B] = Core.Language.GetGCLangIDfromMain((byte)value); }
    public override ushort MetLocation { get => ReadUInt16BigEndian(Data.AsSpan(0x0C)); set => WriteUInt16BigEndian(Data.AsSpan(0x0C), value); }
    public override byte MetLevel { get => Data[0x0E]; set => Data[0x0E] = value; }
    public override byte Ball { get => Data[0x0F]; set => Data[0x0F] = value; }
    public override byte OriginalTrainerGender { get => Data[0x10]; set => Data[0x10] = value; }
    public override uint ID32 { get => ReadUInt32BigEndian(Data.AsSpan(0x14)); set => WriteUInt32BigEndian(Data.AsSpan(0x14), value); }
    public override ushort SID16 { get => ReadUInt16BigEndian(Data.AsSpan(0x14)); set => WriteUInt16BigEndian(Data.AsSpan(0x14), value); }
    public override ushort TID16 { get => ReadUInt16BigEndian(Data.AsSpan(0x16)); set => WriteUInt16BigEndian(Data.AsSpan(0x16), value); }
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

    public override uint EXP { get => ReadUInt32BigEndian(Data.AsSpan(0x5C)); set => WriteUInt32BigEndian(Data.AsSpan(0x5C), value); }
    public override byte Stat_Level { get => Data[0x60]; set => Data[0x60] = value; }

    // 0x64-0x77 are battle/status related
    public override int Status_Condition { get; set; } // where are we
    // Not that the program cares

    // Moves
    public override ushort Move1 { get => ReadUInt16BigEndian(Data.AsSpan(0x78)); set => WriteUInt16BigEndian(Data.AsSpan(0x78), value); }
    public override int Move1_PP { get => Data[0x7A]; set => Data[0x7A] = (byte)value; }
    public override int Move1_PPUps { get => Data[0x7B]; set => Data[0x7B] = (byte)value; }
    public override ushort Move2 { get => ReadUInt16BigEndian(Data.AsSpan(0x7C)); set => WriteUInt16BigEndian(Data.AsSpan(0x7C), value); }
    public override int Move2_PP { get => Data[0x7E]; set => Data[0x7E] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x7F]; set => Data[0x7F] = (byte)value; }
    public override ushort Move3 { get => ReadUInt16BigEndian(Data.AsSpan(0x80)); set => WriteUInt16BigEndian(Data.AsSpan(0x80), value); }
    public override int Move3_PP { get => Data[0x82]; set => Data[0x82] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x83]; set => Data[0x83] = (byte)value; }
    public override ushort Move4 { get => ReadUInt16BigEndian(Data.AsSpan(0x84)); set => WriteUInt16BigEndian(Data.AsSpan(0x84), value); }
    public override int Move4_PP { get => Data[0x86]; set => Data[0x86] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x87]; set => Data[0x87] = (byte)value; }

    public override int SpriteItem => ItemConverter.GetItemFuture3((ushort)HeldItem);
    public override int HeldItem { get => ReadUInt16BigEndian(Data.AsSpan(0x88)); set => WriteUInt16BigEndian(Data.AsSpan(0x88), (ushort)value); }

    // More party stats
    public override int Stat_HPCurrent { get => ReadUInt16BigEndian(Data.AsSpan(0x8A)); set => WriteUInt16BigEndian(Data.AsSpan(0x8A), (ushort)value); }
    public override int Stat_HPMax { get => ReadUInt16BigEndian(Data.AsSpan(0x8C)); set => WriteUInt16BigEndian(Data.AsSpan(0x8C), (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16BigEndian(Data.AsSpan(0x8E)); set => WriteUInt16BigEndian(Data.AsSpan(0x8E), (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16BigEndian(Data.AsSpan(0x90)); set => WriteUInt16BigEndian(Data.AsSpan(0x90), (ushort)value); }
    public override int Stat_SPA { get => ReadUInt16BigEndian(Data.AsSpan(0x92)); set => WriteUInt16BigEndian(Data.AsSpan(0x92), (ushort)value); }
    public override int Stat_SPD { get => ReadUInt16BigEndian(Data.AsSpan(0x94)); set => WriteUInt16BigEndian(Data.AsSpan(0x94), (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16BigEndian(Data.AsSpan(0x96)); set => WriteUInt16BigEndian(Data.AsSpan(0x96), (ushort)value); }

    // EVs
    public override int EV_HP {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0x98)));
        set => WriteUInt16BigEndian(Data.AsSpan(0x98), (ushort)(value & 0xFF)); }

    public override int EV_ATK {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0x9A)));
        set => WriteUInt16BigEndian(Data.AsSpan(0x9A), (ushort)(value & 0xFF)); }

    public override int EV_DEF {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0x9C)));
        set => WriteUInt16BigEndian(Data.AsSpan(0x9C), (ushort)(value & 0xFF)); }

    public override int EV_SPA {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0x9E)));
        set => WriteUInt16BigEndian(Data.AsSpan(0x9E), (ushort)(value & 0xFF)); }

    public override int EV_SPD {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0xA0)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA0), (ushort)(value & 0xFF)); }

    public override int EV_SPE {
        get => Math.Min(byte.MaxValue, ReadUInt16BigEndian(Data.AsSpan(0xA2)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA2), (ushort)(value & 0xFF)); }

    // IVs
    public override int IV_HP {
        get => Math.Min((ushort)31, ReadUInt16BigEndian(Data.AsSpan(0xA4)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA4), (ushort)(value & 0x1F)); }

    public override int IV_ATK {
        get => Math.Min((ushort)31, ReadUInt16BigEndian(Data.AsSpan(0xA6)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA6), (ushort)(value & 0x1F)); }

    public override int IV_DEF {
        get => Math.Min((ushort)31, ReadUInt16BigEndian(Data.AsSpan(0xA8)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xA8), (ushort)(value & 0x1F)); }

    public override int IV_SPA {
        get => Math.Min((ushort)31, ReadUInt16BigEndian(Data.AsSpan(0xAA)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xAA), (ushort)(value & 0x1F)); }

    public override int IV_SPD {
        get => Math.Min((ushort)31, ReadUInt16BigEndian(Data.AsSpan(0xAC)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xAC), (ushort)(value & 0x1F)); }

    public override int IV_SPE {
        get => Math.Min((ushort)31, ReadUInt16BigEndian(Data.AsSpan(0xAE)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xAE), (ushort)(value & 0x1F)); }

    public override byte OriginalTrainerFriendship {
        get => (byte)Math.Min((ushort)0xFF, ReadUInt16BigEndian(Data.AsSpan(0xB0)));
        set => WriteUInt16BigEndian(Data.AsSpan(0xB0), (ushort)(value & 0xFF));
    }

    // Contest
    public override byte ContestCool   { get => Data[0xB2]; set => Data[0xB2] = value; }
    public override byte ContestBeauty { get => Data[0xB3]; set => Data[0xB3] = value; }
    public override byte ContestCute   { get => Data[0xB4]; set => Data[0xB4] = value; }
    public override byte ContestSmart  { get => Data[0xB5]; set => Data[0xB5] = value; }
    public override byte ContestTough  { get => Data[0xB6]; set => Data[0xB6] = value; }
    public override byte RibbonCountG3Cool   { get => Data[0xB7]; set => Data[0xB7] = value; }
    public override byte RibbonCountG3Beauty { get => Data[0xB8]; set => Data[0xB8] = value; }
    public override byte RibbonCountG3Cute   { get => Data[0xB9]; set => Data[0xB9] = value; }
    public override byte RibbonCountG3Smart  { get => Data[0xBA]; set => Data[0xBA] = value; }
    public override byte RibbonCountG3Tough  { get => Data[0xBB]; set => Data[0xBB] = value; }
    public override byte ContestSheen { get => Data[0xBC]; set => Data[0xBC] = value; }

    // Ribbons
    public override bool RibbonChampionG3       { get => Data[0xBD] == 1; set => Data[0xBD] = value ? (byte)1 : (byte)0; }
    public override bool RibbonWinning          { get => Data[0xBE] == 1; set => Data[0xBE] = value ? (byte)1 : (byte)0; }
    public override bool RibbonVictory          { get => Data[0xBF] == 1; set => Data[0xBF] = value ? (byte)1 : (byte)0; }
    public override bool RibbonArtist           { get => Data[0xC0] == 1; set => Data[0xC0] = value ? (byte)1 : (byte)0; }
    public override bool RibbonEffort           { get => Data[0xC1] == 1; set => Data[0xC1] = value ? (byte)1 : (byte)0; }
    public override bool RibbonChampionBattle   { get => Data[0xC2] == 1; set => Data[0xC2] = value ? (byte)1 : (byte)0; }
    public override bool RibbonChampionRegional { get => Data[0xC3] == 1; set => Data[0xC3] = value ? (byte)1 : (byte)0; }
    public override bool RibbonChampionNational { get => Data[0xC4] == 1; set => Data[0xC4] = value ? (byte)1 : (byte)0; }
    public override bool RibbonCountry          { get => Data[0xC5] == 1; set => Data[0xC5] = value ? (byte)1 : (byte)0; }
    public override bool RibbonNational         { get => Data[0xC6] == 1; set => Data[0xC6] = value ? (byte)1 : (byte)0; }
    public override bool RibbonEarth            { get => Data[0xC7] == 1; set => Data[0xC7] = value ? (byte)1 : (byte)0; }
    public override bool RibbonWorld            { get => Data[0xC8] == 1; set => Data[0xC8] = value ? (byte)1 : (byte)0; }
    public override bool Unused1                { get => ((Data[0xC9] >> 0) & 1) == 1; set => Data[0xC9] = (byte)((Data[0xC9] & ~1) | (value ? 1 : 0)); }
    public override bool Unused2                { get => ((Data[0xC9] >> 1) & 1) == 1; set => Data[0xC9] = (byte)((Data[0xC9] & ~2) | (value ? 2 : 0)); }
    public override bool Unused3                { get => ((Data[0xC9] >> 2) & 1) == 1; set => Data[0xC9] = (byte)((Data[0xC9] & ~4) | (value ? 4 : 0)); }
    public override bool Unused4                { get => ((Data[0xC9] >> 3) & 1) == 1; set => Data[0xC9] = (byte)((Data[0xC9] & ~8) | (value ? 8 : 0)); }
    private bool FatefulEncounterJPN            { get => ((Data[0xC9] >> 4) & 1) == 1; set => Data[0xC9] = (byte)((Data[0xC9] &~16) | (value ?16 : 0)); }
    public override int RibbonCount => Data.AsSpan(0xBD, 12).Count<byte>(1) + RibbonCountG3Cool + RibbonCountG3Beauty + RibbonCountG3Cute + RibbonCountG3Smart + RibbonCountG3Tough;

    public override int PokerusStrain { get => Data[0xCA] & 0xF; set => Data[0xCA] = (byte)(value & 0xF); }
    public override bool IsEgg { get => Data[0xCB] == 1; set => Data[0xCB] = value ? (byte)1 : (byte)0; }
    public override bool AbilityBit { get => Data[0xCC] == 1; set => Data[0xCC] = value ? (byte)1 : (byte)0; }
    public override bool Valid { get => Data[0xCD] == 0; set => Data[0xCD] = !value ? (byte)1 : (byte)0; }

    public override byte MarkingValue { get => (byte)SwapBits(Data[0xCF], 1, 2); set => Data[0xCF] = (byte)SwapBits(value, 1, 2); }
    public override int PokerusDays { get => Math.Max((sbyte)Data[0xD0], (sbyte)0); set => Data[0xD0] = (byte)(value == 0 ? 0xFF : value & 0xF); }

    public int PartySlot { get => Data[0xD7]; set => Data[0xD7] = (byte)value; } // or not; only really used while in party?
    public ushort ShadowID { get => ReadUInt16BigEndian(Data.AsSpan(0xD8)); set => WriteUInt16BigEndian(Data.AsSpan(0xD8), value); }
    public int Purification { get => ReadInt32BigEndian(Data.AsSpan(0xDC)); set => WriteInt32BigEndian(Data.AsSpan(0xDC), value); }
    public uint EXP_Shadow { get => ReadUInt32BigEndian(Data.AsSpan(0xC0)); set => WriteUInt32BigEndian(Data.AsSpan(0xC0), value); }

    private bool FatefulEncounterINT { get => ((Data[0xFB] >> 0) & 1) == 1; set => Data[0xFB] = (byte)((Data[0xFB] & ~1) | (value ? 1 : 0)); }

    public override bool FatefulEncounter
    {
        // Property reads depend on the save file language, and we aren't differentiating via file.
        // set based on language, resolve when setting to actual save
        // treat both as False (more likely to be flagged invalid by legality checks)
        get => FatefulEncounterJPN != FatefulEncounterINT;
        set => ForceCorrectFatefulState(Japanese, value);
    }

    /// <summary>
    /// Sets the Fateful Encounter flag to the correct value for the given language format.
    /// </summary>
    /// <param name="japanese">Is saved to a Japanese language save file</param>
    /// <param name="value">Fateful Encounter state</param>
    public void ForceCorrectFatefulState(bool japanese, bool value)
    {
        FatefulEncounterJPN = japanese && value;
        FatefulEncounterINT = !japanese && value;
    }

    /// <summary>
    /// Checks if the Fateful Encounter flag states are valid for the given language format.
    /// </summary>
    /// <param name="japanese">Is saved to a Japanese language save file</param>
    /// <returns>True if the other-language Fateful Encounter flag is not set</returns>
    public bool IsFatefulValid(bool japanese)
    {
        if (japanese)
            return !FatefulEncounterINT;
        return !FatefulEncounterJPN;
    }

    public const int Purified = -100;
    public bool IsShadow => ShadowID != 0 && Purification != Purified;

    protected override byte[] Encrypt() => (byte[])Data.Clone();

    public PK3 ConvertToPK3()
    {
        var pk = ConvertTo<PK3>();
        StringConverter3GC.RemapGlyphs3GBA(NicknameTrash, CurrentRegion, Language, pk.NicknameTrash);
        StringConverter3GC.RemapGlyphs3GBA(OriginalTrainerTrash, CurrentRegion, Language, pk.OriginalTrainerTrash);
        pk.FlagHasSpecies = pk.SpeciesInternal != 0; // Update Flag
        pk.RefreshChecksum();
        return pk;
    }

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
