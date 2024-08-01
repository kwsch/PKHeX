using System;
using System.Numerics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 5 <see cref="PKM"/> format. </summary>
public sealed class PK5 : PKM, ISanityChecksum,
    IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetUnique3, IRibbonSetUnique4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetRibbons,
    IContestStats, IGroundTile, IAppliedMarkings4, IHandlerUpdate
{
    public override ReadOnlySpan<ushort> ExtraBytes =>
    [
        0x43, 0x44, 0x45, 0x46, 0x47,
        0x5E, // unused
        0x63, // last 8 bits of a 32bit ribbonset
        0x64, 0x65, 0x66, 0x67, // unused 32bit ribbonset?
        0x86, // unused
        0x87, // PokeStar Fame
    ];

    public override int SIZE_PARTY => PokeCrypto.SIZE_5PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_5STORED;
    public override EntityContext Context => EntityContext.Gen5;
    public override PersonalInfo5B2W2 PersonalInfo => PersonalTable.B2W2.GetFormEntry(Species, Form);

    public PK5() : base(PokeCrypto.SIZE_5PARTY) { }
    public PK5(byte[] data) : base(DecryptParty(data)) { }

    private static byte[] DecryptParty(byte[] data)
    {
        PokeCrypto.DecryptIfEncrypted45(ref data);
        Array.Resize(ref data, PokeCrypto.SIZE_5PARTY);
        return data;
    }

    public override PK5 Clone() => new((byte[])Data.Clone());
    public override void RefreshChecksum() => Checksum = CalculateChecksum();
    public override bool ChecksumValid => CalculateChecksum() == Checksum;
    public override bool Valid { get => Sanity == 0 && ChecksumValid; set { if (!value) return; Sanity = 0; RefreshChecksum(); } }
    private ushort CalculateChecksum() => Checksums.Add16(Data.AsSpan()[8..PokeCrypto.SIZE_5STORED]);

    // Trash Bytes
    public override Span<byte> NicknameTrash => Data.AsSpan(0x48, 22);
    public override Span<byte> OriginalTrainerTrash => Data.AsSpan(0x68, 16);
    public override int TrashCharCountNickname => 11;
    public override int TrashCharCountTrainer => 8;

    // Future Attributes
    public override uint EncryptionConstant { get => PID; set { } }
    public override byte CurrentFriendship { get => OriginalTrainerFriendship; set => OriginalTrainerFriendship = value; }
    public override byte CurrentHandler { get => 0; set { } }
    public override int AbilityNumber { get => HiddenAbility ? 4 : 1 << PIDAbility; set { } }

    // Structure
    public override uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0x00)); set => WriteUInt32LittleEndian(Data.AsSpan(0x00), value); }
    public ushort Sanity { get => ReadUInt16LittleEndian(Data.AsSpan(0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(0x04), value); }
    public ushort Checksum { get => ReadUInt16LittleEndian(Data.AsSpan(0x06)); set => WriteUInt16LittleEndian(Data.AsSpan(0x06), value); }

    #region Block A
    public override ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(0x08)); set => WriteUInt16LittleEndian(Data.AsSpan(0x08), value); }
    public override int HeldItem { get => ReadUInt16LittleEndian(Data.AsSpan(0x0A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value); }
    public override uint ID32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x0C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x0C), value); }
    public override ushort TID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x0C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0C), value); }
    public override ushort SID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), value); }
    public override uint EXP { get => ReadUInt32LittleEndian(Data.AsSpan(0x10)); set => WriteUInt32LittleEndian(Data.AsSpan(0x10), value); }
    public override byte OriginalTrainerFriendship { get => Data[0x14]; set => Data[0x14] = value; }
    public override int Ability { get => Data[0x15]; set => Data[0x15] = (byte)value; }
    public byte MarkingValue { get => Data[0x16]; set => Data[0x16] = value; }
    public override int Language { get => Data[0x17]; set => Data[0x17] = (byte)value; }
    public override int EV_HP { get => Data[0x18]; set => Data[0x18] = (byte)value; }
    public override int EV_ATK { get => Data[0x19]; set => Data[0x19] = (byte)value; }
    public override int EV_DEF { get => Data[0x1A]; set => Data[0x1A] = (byte)value; }
    public override int EV_SPE { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
    public override int EV_SPA { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
    public override int EV_SPD { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
    public byte ContestCool   { get => Data[0x1E]; set => Data[0x1E] = value; }
    public byte ContestBeauty { get => Data[0x1F]; set => Data[0x1F] = value; }
    public byte ContestCute   { get => Data[0x20]; set => Data[0x20] = value; }
    public byte ContestSmart  { get => Data[0x21]; set => Data[0x21] = value; }
    public byte ContestTough  { get => Data[0x22]; set => Data[0x22] = value; }
    public byte ContestSheen  { get => Data[0x23]; set => Data[0x23] = value; }

    private byte RIB0 { get => Data[0x24]; set => Data[0x24] = value; } // Sinnoh 1
    private byte RIB1 { get => Data[0x25]; set => Data[0x25] = value; } // Sinnoh 2
    private byte RIB2 { get => Data[0x26]; set => Data[0x26] = value; } // Sinnoh 3
    private byte RIB3 { get => Data[0x27]; set => Data[0x27] = value; } // Sinnoh 4
    public bool RibbonChampionSinnoh    { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonAbility           { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonAbilityGreat      { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonAbilityDouble     { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonAbilityMulti      { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonAbilityPair       { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonAbilityWorld      { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonAlert             { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonShock             { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonDowncast          { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonCareless          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonRelax             { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonSnooze            { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonSmile             { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonGorgeous          { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonRoyal             { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonGorgeousRoyal     { get => (RIB2 & (1 << 0)) == 1 << 0; set => RIB2 = (byte)((RIB2 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonFootprint         { get => (RIB2 & (1 << 1)) == 1 << 1; set => RIB2 = (byte)((RIB2 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonRecord            { get => (RIB2 & (1 << 2)) == 1 << 2; set => RIB2 = (byte)((RIB2 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonEvent             { get => (RIB2 & (1 << 3)) == 1 << 3; set => RIB2 = (byte)((RIB2 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonLegend            { get => (RIB2 & (1 << 4)) == 1 << 4; set => RIB2 = (byte)((RIB2 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonChampionWorld     { get => (RIB2 & (1 << 5)) == 1 << 5; set => RIB2 = (byte)((RIB2 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonBirthday          { get => (RIB2 & (1 << 6)) == 1 << 6; set => RIB2 = (byte)((RIB2 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonSpecial           { get => (RIB2 & (1 << 7)) == 1 << 7; set => RIB2 = (byte)((RIB2 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonSouvenir          { get => (RIB3 & (1 << 0)) == 1 << 0; set => RIB3 = (byte)((RIB3 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonWishing           { get => (RIB3 & (1 << 1)) == 1 << 1; set => RIB3 = (byte)((RIB3 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonClassic           { get => (RIB3 & (1 << 2)) == 1 << 2; set => RIB3 = (byte)((RIB3 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonPremier           { get => (RIB3 & (1 << 3)) == 1 << 3; set => RIB3 = (byte)((RIB3 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RIB3_4 { get => (RIB3 & (1 << 4)) == 1 << 4; set => RIB3 = (byte)((RIB3 & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
    public bool RIB3_5 { get => (RIB3 & (1 << 5)) == 1 << 5; set => RIB3 = (byte)((RIB3 & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
    public bool RIB3_6 { get => (RIB3 & (1 << 6)) == 1 << 6; set => RIB3 = (byte)((RIB3 & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
    public bool RIB3_7 { get => (RIB3 & (1 << 7)) == 1 << 7; set => RIB3 = (byte)((RIB3 & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused

    public int RibbonCount => BitOperations.PopCount(ReadUInt32LittleEndian(Data.AsSpan(0x30)) & 0b00001111_11111111__11111111_11111111)
                            + BitOperations.PopCount(ReadUInt32LittleEndian(Data.AsSpan(0x3C)))
                            + BitOperations.PopCount(ReadUInt32LittleEndian(Data.AsSpan(0x60)) & 0b00000000_00001111__11111111_11111111);
    #endregion

    #region Block B
    public override ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x28)); set => WriteUInt16LittleEndian(Data.AsSpan(0x28), value); }
    public override ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x2A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2A), value); }
    public override ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x2C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2C), value); }
    public override ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x2E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x2E), value); }
    public override int Move1_PP { get => Data[0x30]; set => Data[0x30] = (byte)value; }
    public override int Move2_PP { get => Data[0x31]; set => Data[0x31] = (byte)value; }
    public override int Move3_PP { get => Data[0x32]; set => Data[0x32] = (byte)value; }
    public override int Move4_PP { get => Data[0x33]; set => Data[0x33] = (byte)value; }
    public override int Move1_PPUps { get => Data[0x34]; set => Data[0x34] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x35]; set => Data[0x35] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x36]; set => Data[0x36] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x37]; set => Data[0x37] = (byte)value; }
    public uint IV32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x38)); set => WriteUInt32LittleEndian(Data.AsSpan(0x38), value); }
    public override int IV_HP  { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
    public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
    public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
    public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
    public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
    public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }
    public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0u); }
    public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFFu) | (value ? 0x80000000u : 0u); }

    private byte RIB4 { get => Data[0x3C]; set => Data[0x3C] = value; } // Hoenn 1a
    private byte RIB5 { get => Data[0x3D]; set => Data[0x3D] = value; } // Hoenn 1b
    private byte RIB6 { get => Data[0x3E]; set => Data[0x3E] = value; } // Hoenn 2a
    private byte RIB7 { get => Data[0x3F]; set => Data[0x3F] = value; } // Hoenn 2b
    public bool RibbonG3Cool            { get => (RIB4 & (1 << 0)) == 1 << 0; set => RIB4 = (byte)((RIB4 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonG3CoolSuper       { get => (RIB4 & (1 << 1)) == 1 << 1; set => RIB4 = (byte)((RIB4 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonG3CoolHyper       { get => (RIB4 & (1 << 2)) == 1 << 2; set => RIB4 = (byte)((RIB4 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonG3CoolMaster      { get => (RIB4 & (1 << 3)) == 1 << 3; set => RIB4 = (byte)((RIB4 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonG3Beauty          { get => (RIB4 & (1 << 4)) == 1 << 4; set => RIB4 = (byte)((RIB4 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonG3BeautySuper     { get => (RIB4 & (1 << 5)) == 1 << 5; set => RIB4 = (byte)((RIB4 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonG3BeautyHyper     { get => (RIB4 & (1 << 6)) == 1 << 6; set => RIB4 = (byte)((RIB4 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonG3BeautyMaster    { get => (RIB4 & (1 << 7)) == 1 << 7; set => RIB4 = (byte)((RIB4 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonG3Cute            { get => (RIB5 & (1 << 0)) == 1 << 0; set => RIB5 = (byte)((RIB5 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonG3CuteSuper       { get => (RIB5 & (1 << 1)) == 1 << 1; set => RIB5 = (byte)((RIB5 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonG3CuteHyper       { get => (RIB5 & (1 << 2)) == 1 << 2; set => RIB5 = (byte)((RIB5 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonG3CuteMaster      { get => (RIB5 & (1 << 3)) == 1 << 3; set => RIB5 = (byte)((RIB5 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonG3Smart           { get => (RIB5 & (1 << 4)) == 1 << 4; set => RIB5 = (byte)((RIB5 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonG3SmartSuper      { get => (RIB5 & (1 << 5)) == 1 << 5; set => RIB5 = (byte)((RIB5 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonG3SmartHyper      { get => (RIB5 & (1 << 6)) == 1 << 6; set => RIB5 = (byte)((RIB5 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonG3SmartMaster     { get => (RIB5 & (1 << 7)) == 1 << 7; set => RIB5 = (byte)((RIB5 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonG3Tough           { get => (RIB6 & (1 << 0)) == 1 << 0; set => RIB6 = (byte)((RIB6 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonG3ToughSuper      { get => (RIB6 & (1 << 1)) == 1 << 1; set => RIB6 = (byte)((RIB6 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonG3ToughHyper      { get => (RIB6 & (1 << 2)) == 1 << 2; set => RIB6 = (byte)((RIB6 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonG3ToughMaster     { get => (RIB6 & (1 << 3)) == 1 << 3; set => RIB6 = (byte)((RIB6 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonChampionG3        { get => (RIB6 & (1 << 4)) == 1 << 4; set => RIB6 = (byte)((RIB6 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonWinning           { get => (RIB6 & (1 << 5)) == 1 << 5; set => RIB6 = (byte)((RIB6 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonVictory           { get => (RIB6 & (1 << 6)) == 1 << 6; set => RIB6 = (byte)((RIB6 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonArtist            { get => (RIB6 & (1 << 7)) == 1 << 7; set => RIB6 = (byte)((RIB6 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonEffort            { get => (RIB7 & (1 << 0)) == 1 << 0; set => RIB7 = (byte)((RIB7 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonChampionBattle    { get => (RIB7 & (1 << 1)) == 1 << 1; set => RIB7 = (byte)((RIB7 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonChampionRegional  { get => (RIB7 & (1 << 2)) == 1 << 2; set => RIB7 = (byte)((RIB7 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonChampionNational  { get => (RIB7 & (1 << 3)) == 1 << 3; set => RIB7 = (byte)((RIB7 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonCountry           { get => (RIB7 & (1 << 4)) == 1 << 4; set => RIB7 = (byte)((RIB7 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonNational          { get => (RIB7 & (1 << 5)) == 1 << 5; set => RIB7 = (byte)((RIB7 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonEarth             { get => (RIB7 & (1 << 6)) == 1 << 6; set => RIB7 = (byte)((RIB7 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonWorld             { get => (RIB7 & (1 << 7)) == 1 << 7; set => RIB7 = (byte)((RIB7 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

    public override bool FatefulEncounter { get => (Data[0x40] & 1) == 1; set => Data[0x40] = (byte)((Data[0x40] & ~0x01) | (value ? 1 : 0)); }
    public override byte Gender { get => (byte)((Data[0x40] >> 1) & 0x3); set => Data[0x40] = (byte)((Data[0x40] & ~0x06) | (value << 1)); }
    public override byte Form { get => (byte)(Data[0x40] >> 3); set => Data[0x40] = (byte)((Data[0x40] & 0x07) | (value << 3)); }
    public override Nature Nature { get => (Nature)Data[0x41]; set => Data[0x41] = (byte)value; }
    public bool HiddenAbility { get => (Data[0x42] & 1) == 1; set => Data[0x42] = (byte)((Data[0x42] & ~0x01) | (value ? 1 : 0)); }
    public bool NSparkle { get => (Data[0x42] & 2) == 2; set => Data[0x42] = (byte)((Data[0x42] & ~0x02) | (value ? 2 : 0)); }
    // 0x43-0x47 Unused
    #endregion

    #region Block C

    public override string Nickname
    {
        get => StringConverter5.GetString(NicknameTrash);
        set
        {
            var language = Language;
            CheckKoreanNidoranDPPt(value, ref language);
            StringConverter5.SetString(NicknameTrash, value, 10, language, StringConverterOption.None);
        }
    }

    // 0x5E unused
    public override GameVersion Version { get => (GameVersion)Data[0x5F]; set => Data[0x5F] = (byte)value; }
    private byte RIB8 { get => Data[0x60]; set => Data[0x60] = value; } // Sinnoh 3
    private byte RIB9 { get => Data[0x61]; set => Data[0x61] = value; } // Sinnoh 4
    private byte RIBA { get => Data[0x62]; set => Data[0x62] = value; } // Sinnoh 5
    private byte RIBB { get => Data[0x63]; set => Data[0x63] = value; } // Sinnoh 6
    public bool RibbonG4Cool            { get => (RIB8 & (1 << 0)) == 1 << 0; set => RIB8 = (byte)((RIB8 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonG4CoolGreat       { get => (RIB8 & (1 << 1)) == 1 << 1; set => RIB8 = (byte)((RIB8 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonG4CoolUltra       { get => (RIB8 & (1 << 2)) == 1 << 2; set => RIB8 = (byte)((RIB8 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonG4CoolMaster      { get => (RIB8 & (1 << 3)) == 1 << 3; set => RIB8 = (byte)((RIB8 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonG4Beauty          { get => (RIB8 & (1 << 4)) == 1 << 4; set => RIB8 = (byte)((RIB8 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonG4BeautyGreat     { get => (RIB8 & (1 << 5)) == 1 << 5; set => RIB8 = (byte)((RIB8 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonG4BeautyUltra     { get => (RIB8 & (1 << 6)) == 1 << 6; set => RIB8 = (byte)((RIB8 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonG4BeautyMaster    { get => (RIB8 & (1 << 7)) == 1 << 7; set => RIB8 = (byte)((RIB8 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonG4Cute            { get => (RIB9 & (1 << 0)) == 1 << 0; set => RIB9 = (byte)((RIB9 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonG4CuteGreat       { get => (RIB9 & (1 << 1)) == 1 << 1; set => RIB9 = (byte)((RIB9 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonG4CuteUltra       { get => (RIB9 & (1 << 2)) == 1 << 2; set => RIB9 = (byte)((RIB9 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonG4CuteMaster      { get => (RIB9 & (1 << 3)) == 1 << 3; set => RIB9 = (byte)((RIB9 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonG4Smart           { get => (RIB9 & (1 << 4)) == 1 << 4; set => RIB9 = (byte)((RIB9 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonG4SmartGreat      { get => (RIB9 & (1 << 5)) == 1 << 5; set => RIB9 = (byte)((RIB9 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonG4SmartUltra      { get => (RIB9 & (1 << 6)) == 1 << 6; set => RIB9 = (byte)((RIB9 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonG4SmartMaster     { get => (RIB9 & (1 << 7)) == 1 << 7; set => RIB9 = (byte)((RIB9 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonG4Tough           { get => (RIBA & (1 << 0)) == 1 << 0; set => RIBA = (byte)((RIBA & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonG4ToughGreat      { get => (RIBA & (1 << 1)) == 1 << 1; set => RIBA = (byte)((RIBA & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonG4ToughUltra      { get => (RIBA & (1 << 2)) == 1 << 2; set => RIBA = (byte)((RIBA & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonG4ToughMaster     { get => (RIBA & (1 << 3)) == 1 << 3; set => RIBA = (byte)((RIBA & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RIBA_4 { get => (RIBA & (1 << 4)) == 1 << 4; set => RIBA = (byte)((RIBA & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
    public bool RIBA_5 { get => (RIBA & (1 << 5)) == 1 << 5; set => RIBA = (byte)((RIBA & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
    public bool RIBA_6 { get => (RIBA & (1 << 6)) == 1 << 6; set => RIBA = (byte)((RIBA & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
    public bool RIBA_7 { get => (RIBA & (1 << 7)) == 1 << 7; set => RIBA = (byte)((RIBA & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
    public bool RIBB_0 { get => (RIBB & (1 << 0)) == 1 << 0; set => RIBB = (byte)((RIBB & ~(1 << 0)) | (value ? 1 << 0 : 0)); } // Unused
    public bool RIBB_1 { get => (RIBB & (1 << 1)) == 1 << 1; set => RIBB = (byte)((RIBB & ~(1 << 1)) | (value ? 1 << 1 : 0)); } // Unused
    public bool RIBB_2 { get => (RIBB & (1 << 2)) == 1 << 2; set => RIBB = (byte)((RIBB & ~(1 << 2)) | (value ? 1 << 2 : 0)); } // Unused
    public bool RIBB_3 { get => (RIBB & (1 << 3)) == 1 << 3; set => RIBB = (byte)((RIBB & ~(1 << 3)) | (value ? 1 << 3 : 0)); } // Unused
    public bool RIBB_4 { get => (RIBB & (1 << 4)) == 1 << 4; set => RIBB = (byte)((RIBB & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
    public bool RIBB_5 { get => (RIBB & (1 << 5)) == 1 << 5; set => RIBB = (byte)((RIBB & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
    public bool RIBB_6 { get => (RIBB & (1 << 6)) == 1 << 6; set => RIBB = (byte)((RIBB & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
    public bool RIBB_7 { get => (RIBB & (1 << 7)) == 1 << 7; set => RIBB = (byte)((RIBB & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
    // 0x64-0x67 Unused
    #endregion

    #region Block D
    public override string OriginalTrainerName { get => StringConverter5.GetString(OriginalTrainerTrash); set => StringConverter5.SetString(OriginalTrainerTrash, value, 7, Language, StringConverterOption.None); }
    public override byte EggYear { get => Data[0x78]; set => Data[0x78] = value; }
    public override byte EggMonth { get => Data[0x79]; set => Data[0x79] = value; }
    public override byte EggDay { get => Data[0x7A]; set => Data[0x7A] = value; }
    public override byte MetYear { get => Data[0x7B]; set => Data[0x7B] = value; }
    public override byte MetMonth { get => Data[0x7C]; set => Data[0x7C] = value; }
    public override byte MetDay { get => Data[0x7D]; set => Data[0x7D] = value; }
    public override ushort EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0x7E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x7E), value); }
    public override ushort MetLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0x80)); set => WriteUInt16LittleEndian(Data.AsSpan(0x80), value); }
    public byte PokerusState { get => Data[0x82]; set => Data[0x82] = value; }
    public override int PokerusDays { get => PokerusState & 0xF; set => PokerusState = (byte)((PokerusState & ~0xF) | value); }
    public override int PokerusStrain { get => PokerusState >> 4; set => PokerusState = (byte)((PokerusState & 0xF) | (value << 4)); }
    public override byte Ball { get => Data[0x83]; set => Data[0x83] = value; }
    public override byte MetLevel { get => (byte)(Data[0x84] & ~0x80); set => Data[0x84] = (byte)((Data[0x84] & 0x80) | value); }
    public override byte OriginalTrainerGender { get => (byte)(Data[0x84] >> 7); set => Data[0x84] = (byte)((Data[0x84] & ~0x80) | (value << 7)); }
    public GroundTileType GroundTile { get => (GroundTileType)Data[0x85]; set => Data[0x85] = (byte)value; }
    // 0x86 Unused
    public byte PokeStarFame { get => Data[0x87]; set => Data[0x87] = value; }
    public bool IsPokeStar { get => PokeStarFame > 250; set => PokeStarFame = value ? (byte)255 : (byte)0; }
    #endregion

    #region Battle Stats
    public override int Status_Condition { get => ReadInt32LittleEndian(Data.AsSpan(0x88)); set => WriteInt32LittleEndian(Data.AsSpan(0x88), value); }
    public override byte Stat_Level { get => Data[0x8C]; set => Data[0x8C] = value; }
    /// <summary> <see cref="PK4.BallCapsuleIndex"/>, now unused. </summary>
    public byte JunkByte { get => Data[0x8D]; set => Data[0x8D] = value; }
    public override int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data.AsSpan(0x8E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x8E), (ushort)value); }
    public override int Stat_HPMax { get => ReadUInt16LittleEndian(Data.AsSpan(0x90)); set => WriteUInt16LittleEndian(Data.AsSpan(0x90), (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16LittleEndian(Data.AsSpan(0x92)); set => WriteUInt16LittleEndian(Data.AsSpan(0x92), (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16LittleEndian(Data.AsSpan(0x94)); set => WriteUInt16LittleEndian(Data.AsSpan(0x94), (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16LittleEndian(Data.AsSpan(0x96)); set => WriteUInt16LittleEndian(Data.AsSpan(0x96), (ushort)value); }
    public override int Stat_SPA { get => ReadUInt16LittleEndian(Data.AsSpan(0x98)); set => WriteUInt16LittleEndian(Data.AsSpan(0x98), (ushort)value); }
    public override int Stat_SPD { get => ReadUInt16LittleEndian(Data.AsSpan(0x9A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x9A), (ushort)value); }

    public Span<byte> HeldMail => Data.AsSpan(0x9C, 0x38);
    public ulong JunkData { get => ReadUInt64LittleEndian(Data.AsSpan(0xD4, 8)); set => WriteUInt64LittleEndian(Data.AsSpan(0xD4, 8), value); }

    #endregion

    // Generated Attributes
    public override uint PSV => ((PID >> 16) ^ (PID & 0xFFFF)) >> 3;
    public override uint TSV => (uint)(TID16 ^ SID16) >> 3;
    public override int Characteristic => EntityCharacteristic.GetCharacteristic(PID, IV32);

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_5;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_5;
    public override int MaxAbilityID => Legal.MaxAbilityID_5;
    public override int MaxItemID => Legal.MaxItemID_5_B2W2;
    public override int MaxBallID => Legal.MaxBallID_5;
    public override GameVersion MaxGameID => Legal.MaxGameID_5; // B2
    public override int MaxIV => 31;
    public override int MaxEV => EffortValues.Max255;
    public override int MaxStringLengthTrainer => 7;
    public override int MaxStringLengthNickname => 10;

    // Methods
    protected override byte[] Encrypt()
    {
        RefreshChecksum();
        return PokeCrypto.EncryptArray45(Data);
    }

    // Synthetic Trading Logic
    public bool BelongsTo(ITrainerInfo tr)
    {
        if (tr.Version != Version)
            return false;
        if (tr.ID32 != ID32)
            return false;
        if (tr.Gender != OriginalTrainerGender)
            return false;

        Span<char> ot = stackalloc char[MaxStringLengthTrainer];
        int len = LoadString(OriginalTrainerTrash, ot);
        return ot[..len].SequenceEqual(tr.OT);
    }

    public void UpdateHandler(ITrainerInfo tr)
    {
        if (IsEgg)
        {
            // Don't bother updating eggs that were already traded.
            const ushort location = Locations.LinkTrade5;
            if (MetLocation != location && !BelongsTo(tr))
            {
                var date = EncounterDate.GetDateNDS();
                SetLinkTradeEgg(date.Day, date.Month, date.Year, location);
            }
        }
    }

    public int MarkingCount => 6;

    public bool GetMarking(int index)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return ((MarkingValue >> index) & 1) != 0;
    }

    public void SetMarking(int index, bool value)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        MarkingValue = (byte)((MarkingValue & ~(1 << index)) | ((value ? 1 : 0) << index));
    }

    public bool MarkingCircle   { get => GetMarking(0); set => SetMarking(0, value); }
    public bool MarkingTriangle { get => GetMarking(1); set => SetMarking(1, value); }
    public bool MarkingSquare   { get => GetMarking(2); set => SetMarking(2, value); }
    public bool MarkingHeart    { get => GetMarking(3); set => SetMarking(3, value); }
    public bool MarkingStar     { get => GetMarking(4); set => SetMarking(4, value); }
    public bool MarkingDiamond  { get => GetMarking(5); set => SetMarking(5, value); }

    public override void RefreshAbility(int n)
    {
        base.RefreshAbility(n);
        HiddenAbility = n == 2;
    }

    public PK6 ConvertToPK6()
    {
        PK6 pk6 = new() // Convert away!
        {
            EncryptionConstant = PID,
            Species = Species,
            TID16 = TID16,
            SID16 = SID16,
            EXP = EXP,
            PID = PID,
            Ability = Ability,
            AbilityNumber = 1 << CalculateAbilityIndex(),
            MarkingValue = MarkingValue,
            Language = Math.Max((int)LanguageID.Japanese, Language), // Hacked or Bad IngameTrade (Japanese B/W)

            ContestCool = ContestCool,
            ContestBeauty = ContestBeauty,
            ContestCute = ContestCute,
            ContestSmart = ContestSmart,
            ContestTough = ContestTough,
            ContestSheen = ContestSheen,

            // Cap EVs -- anything above 252 is dropped down to 252.
            EV_HP  = Math.Min(EV_HP , EffortValues.Max252),
            EV_ATK = Math.Min(EV_ATK, EffortValues.Max252),
            EV_DEF = Math.Min(EV_DEF, EffortValues.Max252),
            EV_SPA = Math.Min(EV_SPA, EffortValues.Max252),
            EV_SPD = Math.Min(EV_SPD, EffortValues.Max252),
            EV_SPE = Math.Min(EV_SPE, EffortValues.Max252),

            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,

            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,

            IV32 = IV32,

            FatefulEncounter = FatefulEncounter,
            Gender = Gender,
            Form = Form,
            Nature = Nature,

            Version = Version,
            OriginalTrainerName = OriginalTrainerName,

            // Dates are kept upon transfer
            MetDate = MetDate,
            EggMetDate = EggMetDate,

            // Locations are kept upon transfer
            MetLocation = MetLocation,
            EggLocation = EggLocation,

            PokerusState = PokerusState,
            Ball = Ball,

            // OT Gender & Encounter Level
            MetLevel = MetLevel,
            OriginalTrainerGender = OriginalTrainerGender,
            GroundTile = GroundTile,

            // Fill the Ribbon Counter Bytes
            RibbonCountMemoryContest = CountContestRibbons(Data),
            RibbonCountMemoryBattle = CountBattleRibbons(Data),

            // Copy Ribbons to their new locations.
            RibbonChampionG3 = RibbonChampionG3,
            RibbonChampionSinnoh = RibbonChampionSinnoh,
            RibbonEffort = RibbonEffort,

            RibbonAlert = RibbonAlert,
            RibbonShock = RibbonShock,
            RibbonDowncast = RibbonDowncast,
            RibbonCareless = RibbonCareless,
            RibbonRelax = RibbonRelax,
            RibbonSnooze = RibbonSnooze,
            RibbonSmile = RibbonSmile,
            RibbonGorgeous = RibbonGorgeous,

            RibbonRoyal = RibbonRoyal,
            RibbonGorgeousRoyal = RibbonGorgeousRoyal,
            RibbonArtist = RibbonArtist,
            RibbonFootprint = RibbonFootprint,
            RibbonRecord = RibbonRecord,
            RibbonLegend = RibbonLegend,
            RibbonCountry = RibbonCountry,
            RibbonNational = RibbonNational,

            RibbonEarth = RibbonEarth,
            RibbonWorld = RibbonWorld,
            RibbonClassic = RibbonClassic,
            RibbonPremier = RibbonPremier,
            RibbonEvent = RibbonEvent,
            RibbonBirthday = RibbonBirthday,
            RibbonSpecial = RibbonSpecial,
            RibbonSouvenir = RibbonSouvenir,

            RibbonWishing = RibbonWishing,
            RibbonChampionBattle = RibbonChampionBattle,
            RibbonChampionRegional = RibbonChampionRegional,
            RibbonChampionNational = RibbonChampionNational,
            RibbonChampionWorld = RibbonChampionWorld,

            // Write the Memories, Friendship, and Origin!
            CurrentHandler = 1,
            HandlingTrainerName = RecentTrainerCache.OriginalTrainerName,
            HandlingTrainerGender = RecentTrainerCache.OriginalTrainerGender,
            HandlingTrainerMemoryIntensity = 1,
            HandlingTrainerMemory = 4,
            HandlingTrainerMemoryFeeling = MemoryContext6.GetRandomFeeling6(4, 10),
        };

        // Write Transfer Location - location is dependent on 3DS system that transfers.
        RecentTrainerCache.SetConsoleRegionData3DS(pk6);
        RecentTrainerCache.SetFirstCountryRegion(pk6);

        // Apply trash bytes for species name of current app language -- default to PKM's language if no match
        int curLang = SpeciesName.GetSpeciesNameLanguage(Species, Nickname, 5);
        if (curLang < 0)
            curLang = Language;
        pk6.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, curLang, 6);
        if (IsNicknamed)
            pk6.Nickname = Nickname;

        // When transferred, friendship gets reset.
        pk6.OriginalTrainerFriendship = pk6.HandlingTrainerFriendship = PersonalInfo.BaseFriendship;

        // Gen6 changed the shiny correlation to have 2x the rate.
        // If the current PID would be shiny with those increased odds, fix it.
        if ((PSV ^ TSV) == 1)
            pk6.PID ^= 0x80000000;

        // HMs are not deleted 5->6, transfer away (but fix if blank spots?)
        pk6.FixMoves();

        // Fix PP
        pk6.HealPP();

        // Fix Name Strings
        StringConverter345.TransferGlyphs56(pk6.NicknameTrash);
        StringConverter345.TransferGlyphs56(pk6.OriginalTrainerTrash);

        // Fix Checksum
        pk6.RefreshChecksum();

        return pk6; // Done!
    }

    private static byte CountBattleRibbons(ReadOnlySpan<byte> data)
    {
        var bits1 = data[0x24] & 0b0111_1110u; // Battle Ribbons
        var bits2 = data[0x3E] & 0b0110_0000u; // Winning & Victory Ribbons
        return (byte)BitOperations.PopCount(bits1 | (bits2 << 2));
    }

    private static byte CountContestRibbons(ReadOnlySpan<byte> data)
    {
        // Gather all 40 bits into a single ulong and PopCount rather than looping each byte.
        var bits1 = ReadUInt32LittleEndian(data[0x60..]) & 0b1111_11111111_11111111;
        var bits2 = ReadUInt32LittleEndian(data[0x3C..]) & 0b1111_11111111_11111111;
        return (byte)BitOperations.PopCount(((ulong)bits1 << 20) | bits2);
    }

    private int CalculateAbilityIndex()
    {
        if (HiddenAbility)
            return 2;
        var pi = PersonalInfo;
        if (pi.Ability1 == Ability)
            return 0;
        if (pi.Ability2 == Ability)
            return 1;
        // reset ability, invalid
        var pid = PID;
        if (Gen5)
            pid >>= 16;
        return (int)(pid & 1);
    }

    internal static ushort GetTransferMetLocation4(PKM pk)
    {
        // Everything except for crown beasts and Celebi get the default transfer location.
        // Crown beasts and Celebi are 100% identifiable by the species ID and fateful encounter, originating from Gen4.
        if (!pk.FatefulEncounter || !pk.Gen4)
            return Locations.Transfer4; // Pokétransfer

        return pk.Species switch
        {
            // Crown Beast
            (int)Core.Species.Raikou or (int)Core.Species.Entei or (int)Core.Species.Suicune => Locations.Transfer4_CrownUnused,
            // Celebi
            (int)Core.Species.Celebi => Locations.Transfer4_CelebiUnused,
            // Default
            _ => Locations.Transfer4,
        };
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter5.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter5.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter5.SetString(destBuffer, value, maxLength, Language, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetTerminatorIndex(data, StringConverter5.Terminator);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetStringLength(data, StringConverter5.Terminator);
    public override int GetBytesPerChar() => 2;

    /// <inheritdoc cref="G4PKM.CheckKoreanNidoranDPPt"/>
    /// <remarks> Gen4->Gen5 chars transfer without resetting the name. Still relevant even as PK5. </remarks>
    private void CheckKoreanNidoranDPPt(ReadOnlySpan<char> value, ref int language)
    {
        if (language != (int)LanguageID.Korean)
            return;
        if (IsNicknamed)
            return;
        if (Version is not (GameVersion.D or GameVersion.P or GameVersion.Pt))
            return;
        // Full-width gender symbols for not-nicknamed Nidoran in D/P/Pt
        // Full/Half is technically legal either way as trainers can reset the nickname in HG/SS, or vice versa for origins.
        // Still try to set whichever it originated with. Default would be half, but if it's a Nidoran species name, set full-width.
        if (Species == (int)Core.Species.NidoranM && value is "니드런♂")
            language = 1; // Use Japanese to force full-width encoding of the gender symbol.
        else if (Species == (int)Core.Species.NidoranF && value is "니드런♀")
            language = 1;
    }
}
