using System;
using System.Numerics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 7 <see cref="PKM"/> format. </summary>
public sealed class PK7 : G6PKM, IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetMemory6, IRibbonSetCommon7, IRibbonSetRibbons,
    IContestStats, IHyperTrain, IGeoTrack, ISuperTrainRegimen, IFormArgument, ITrainerMemories, IAffection, IPokerusStatus, IAppliedMarkings7
{
    public override ReadOnlySpan<ushort> ExtraBytes =>
    [
        0x2A, // Old Marking Value (PelagoEventStatus)
        // 0x36, 0x37, // Unused Ribbons
        0x58, 0x59, 0x73, 0x90, 0x91, 0x9E, 0x9F, 0xA0, 0xA1, 0xA7, 0xAA, 0xAB, 0xAC, 0xAD, 0xC8, 0xC9, 0xD7, 0xE4, 0xE5, 0xE6, 0xE7,
    ];

    public override EntityContext Context => EntityContext.Gen7;
    public override PersonalInfo7 PersonalInfo => PersonalTable.USUM.GetFormEntry(Species, Form);

    public PK7() : base(PokeCrypto.SIZE_6PARTY) { }
    public PK7(byte[] data) : base(DecryptParty(data)) { }

    private static byte[] DecryptParty(byte[] data)
    {
        PokeCrypto.DecryptIfEncrypted67(ref data);
        Array.Resize(ref data, PokeCrypto.SIZE_6PARTY);
        return data;
    }

    public override PK7 Clone() => new((byte[])Data.Clone());

    // Structure
    #region Block A
    public override uint EncryptionConstant
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x00));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x00), value);
    }

    public override ushort Sanity
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x04));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x04), value);
    }

    public override ushort Checksum
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x06));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x06), value);
    }

    public override ushort Species
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x08));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x08), value);
    }

    public override int HeldItem
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x0A));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value);
    }

    public override uint ID32
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x0C));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x0C), value);
    }

    public override ushort TID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x0C));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x0C), value);
    }

    public override ushort SID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x0E));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), value);
    }

    public override uint EXP
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x10));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x10), value);
    }

    public override int Ability { get => Data[0x14]; set => Data[0x14] = (byte)value; }
    public override int AbilityNumber { get => Data[0x15] & 7; set => Data[0x15] = (byte)((Data[0x15] & ~7) | (value & 7)); }
    public ushort MarkingValue { get => ReadUInt16LittleEndian(Data.AsSpan(0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(0x16), value); }

    public override uint PID
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x18));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x18), value);
    }

    public override Nature Nature { get => (Nature)Data[0x1C]; set => Data[0x1C] = (byte)value; }
    public override bool FatefulEncounter { get => (Data[0x1D] & 1) == 1; set => Data[0x1D] = (byte)((Data[0x1D] & ~0x01) | (value ? 1 : 0)); }
    public override byte Gender { get => (byte)((Data[0x1D] >> 1) & 0x3); set => Data[0x1D] = (byte)((Data[0x1D] & ~0x06) | (value << 1)); }
    public override byte Form { get => (byte)(Data[0x1D] >> 3); set => Data[0x1D] = (byte)((Data[0x1D] & 0x07) | (value << 3)); }
    public override int EV_HP { get => Data[0x1E]; set => Data[0x1E] = (byte)value; }
    public override int EV_ATK { get => Data[0x1F]; set => Data[0x1F] = (byte)value; }
    public override int EV_DEF { get => Data[0x20]; set => Data[0x20] = (byte)value; }
    public override int EV_SPE { get => Data[0x21]; set => Data[0x21] = (byte)value; }
    public override int EV_SPA { get => Data[0x22]; set => Data[0x22] = (byte)value; }
    public override int EV_SPD { get => Data[0x23]; set => Data[0x23] = (byte)value; }
    public byte ContestCool   { get => Data[0x24]; set => Data[0x24] = value; }
    public byte ContestBeauty { get => Data[0x25]; set => Data[0x25] = value; }
    public byte ContestCute   { get => Data[0x26]; set => Data[0x26] = value; }
    public byte ContestSmart  { get => Data[0x27]; set => Data[0x27] = value; }
    public byte ContestTough  { get => Data[0x28]; set => Data[0x28] = value; }
    public byte ContestSheen  { get => Data[0x29]; set => Data[0x29] = value; }
    public ResortEventState ResortEventStatus { get => (ResortEventState)Data[0x2A]; set => Data[0x2A] = (byte)value; }
    public byte PokerusState { get => Data[0x2B]; set => Data[0x2B] = value; }
    public override int PokerusDays { get => PokerusState & 0xF; set => PokerusState = (byte)((PokerusState & ~0xF) | value); }
    public override int PokerusStrain { get => PokerusState >> 4; set => PokerusState = (byte)((PokerusState & 0xF) | (value << 4)); }
    private byte ST1 { get => Data[0x2C]; set => Data[0x2C] = value; }
    public bool Unused0 { get => (ST1 & (1 << 0)) == 1 << 0; set => ST1 = (byte)((ST1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool Unused1 { get => (ST1 & (1 << 1)) == 1 << 1; set => ST1 = (byte)((ST1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool SuperTrain1_SPA { get => (ST1 & (1 << 2)) == 1 << 2; set => ST1 = (byte)((ST1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool SuperTrain1_HP  { get => (ST1 & (1 << 3)) == 1 << 3; set => ST1 = (byte)((ST1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool SuperTrain1_ATK { get => (ST1 & (1 << 4)) == 1 << 4; set => ST1 = (byte)((ST1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool SuperTrain1_SPD { get => (ST1 & (1 << 5)) == 1 << 5; set => ST1 = (byte)((ST1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool SuperTrain1_SPE { get => (ST1 & (1 << 6)) == 1 << 6; set => ST1 = (byte)((ST1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool SuperTrain1_DEF { get => (ST1 & (1 << 7)) == 1 << 7; set => ST1 = (byte)((ST1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    private byte ST2 { get => Data[0x2D]; set => Data[0x2D] = value; }
    public bool SuperTrain2_SPA { get => (ST2 & (1 << 0)) == 1 << 0; set => ST2 = (byte)((ST2 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool SuperTrain2_HP  { get => (ST2 & (1 << 1)) == 1 << 1; set => ST2 = (byte)((ST2 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool SuperTrain2_ATK { get => (ST2 & (1 << 2)) == 1 << 2; set => ST2 = (byte)((ST2 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool SuperTrain2_SPD { get => (ST2 & (1 << 3)) == 1 << 3; set => ST2 = (byte)((ST2 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool SuperTrain2_SPE { get => (ST2 & (1 << 4)) == 1 << 4; set => ST2 = (byte)((ST2 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool SuperTrain2_DEF { get => (ST2 & (1 << 5)) == 1 << 5; set => ST2 = (byte)((ST2 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool SuperTrain3_SPA { get => (ST2 & (1 << 6)) == 1 << 6; set => ST2 = (byte)((ST2 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool SuperTrain3_HP { get => (ST2 & (1 << 7)) == 1 << 7; set => ST2 = (byte)((ST2 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    private byte ST3 { get => Data[0x2E]; set => Data[0x2E] = value; }
    public bool SuperTrain3_ATK { get => (ST3 & (1 << 0)) == 1 << 0; set => ST3 = (byte)((ST3 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool SuperTrain3_SPD { get => (ST3 & (1 << 1)) == 1 << 1; set => ST3 = (byte)((ST3 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool SuperTrain3_SPE { get => (ST3 & (1 << 2)) == 1 << 2; set => ST3 = (byte)((ST3 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool SuperTrain3_DEF { get => (ST3 & (1 << 3)) == 1 << 3; set => ST3 = (byte)((ST3 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool SuperTrain4_1 { get => (ST3 & (1 << 4)) == 1 << 4; set => ST3 = (byte)((ST3 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool SuperTrain5_1 { get => (ST3 & (1 << 5)) == 1 << 5; set => ST3 = (byte)((ST3 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool SuperTrain5_2 { get => (ST3 & (1 << 6)) == 1 << 6; set => ST3 = (byte)((ST3 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool SuperTrain5_3 { get => (ST3 & (1 << 7)) == 1 << 7; set => ST3 = (byte)((ST3 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    private byte ST4 { get => Data[0x2F]; set => Data[0x2F] = value; }
    public bool SuperTrain5_4 { get => (ST4 & (1 << 0)) == 1 << 0; set => ST4 = (byte)((ST4 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool SuperTrain6_1 { get => (ST4 & (1 << 1)) == 1 << 1; set => ST4 = (byte)((ST4 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool SuperTrain6_2 { get => (ST4 & (1 << 2)) == 1 << 2; set => ST4 = (byte)((ST4 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool SuperTrain6_3 { get => (ST4 & (1 << 3)) == 1 << 3; set => ST4 = (byte)((ST4 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool SuperTrain7_1 { get => (ST4 & (1 << 4)) == 1 << 4; set => ST4 = (byte)((ST4 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool SuperTrain7_2 { get => (ST4 & (1 << 5)) == 1 << 5; set => ST4 = (byte)((ST4 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool SuperTrain7_3 { get => (ST4 & (1 << 6)) == 1 << 6; set => ST4 = (byte)((ST4 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool SuperTrain8_1 { get => (ST4 & (1 << 7)) == 1 << 7; set => ST4 = (byte)((ST4 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public uint SuperTrainBitFlags { get => ReadUInt32LittleEndian(Data.AsSpan(0x2C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x2C), value); }
    private byte RIB0 { get => Data[0x30]; set => Data[0x30] = value; } // Ribbons are read as uints, but let's keep them per byte.
    private byte RIB1 { get => Data[0x31]; set => Data[0x31] = value; }
    private byte RIB2 { get => Data[0x32]; set => Data[0x32] = value; }
    private byte RIB3 { get => Data[0x33]; set => Data[0x33] = value; }
    private byte RIB4 { get => Data[0x34]; set => Data[0x34] = value; }
    private byte RIB5 { get => Data[0x35]; set => Data[0x35] = value; }
    private byte RIB6 { get => Data[0x36]; set => Data[0x36] = value; }
    //private byte RIB7 { get => Data[0x37]; set => Data[0x37] = value; } // Unused
    public bool RibbonChampionKalos         { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonChampionG3            { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonChampionSinnoh        { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonBestFriends           { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonTraining              { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonBattlerSkillful       { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonBattlerExpert         { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonEffort                { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonAlert                 { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonShock                 { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonDowncast              { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonCareless              { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonRelax                 { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonSnooze                { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonSmile                 { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonGorgeous              { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonRoyal                 { get => (RIB2 & (1 << 0)) == 1 << 0; set => RIB2 = (byte)((RIB2 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonGorgeousRoyal         { get => (RIB2 & (1 << 1)) == 1 << 1; set => RIB2 = (byte)((RIB2 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonArtist                { get => (RIB2 & (1 << 2)) == 1 << 2; set => RIB2 = (byte)((RIB2 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonFootprint             { get => (RIB2 & (1 << 3)) == 1 << 3; set => RIB2 = (byte)((RIB2 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonRecord                { get => (RIB2 & (1 << 4)) == 1 << 4; set => RIB2 = (byte)((RIB2 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonLegend                { get => (RIB2 & (1 << 5)) == 1 << 5; set => RIB2 = (byte)((RIB2 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonCountry               { get => (RIB2 & (1 << 6)) == 1 << 6; set => RIB2 = (byte)((RIB2 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonNational              { get => (RIB2 & (1 << 7)) == 1 << 7; set => RIB2 = (byte)((RIB2 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonEarth                 { get => (RIB3 & (1 << 0)) == 1 << 0; set => RIB3 = (byte)((RIB3 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonWorld                 { get => (RIB3 & (1 << 1)) == 1 << 1; set => RIB3 = (byte)((RIB3 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonClassic               { get => (RIB3 & (1 << 2)) == 1 << 2; set => RIB3 = (byte)((RIB3 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonPremier               { get => (RIB3 & (1 << 3)) == 1 << 3; set => RIB3 = (byte)((RIB3 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonEvent                 { get => (RIB3 & (1 << 4)) == 1 << 4; set => RIB3 = (byte)((RIB3 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonBirthday              { get => (RIB3 & (1 << 5)) == 1 << 5; set => RIB3 = (byte)((RIB3 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonSpecial               { get => (RIB3 & (1 << 6)) == 1 << 6; set => RIB3 = (byte)((RIB3 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonSouvenir              { get => (RIB3 & (1 << 7)) == 1 << 7; set => RIB3 = (byte)((RIB3 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonWishing               { get => (RIB4 & (1 << 0)) == 1 << 0; set => RIB4 = (byte)((RIB4 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonChampionBattle        { get => (RIB4 & (1 << 1)) == 1 << 1; set => RIB4 = (byte)((RIB4 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonChampionRegional      { get => (RIB4 & (1 << 2)) == 1 << 2; set => RIB4 = (byte)((RIB4 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonChampionNational      { get => (RIB4 & (1 << 3)) == 1 << 3; set => RIB4 = (byte)((RIB4 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonChampionWorld         { get => (RIB4 & (1 << 4)) == 1 << 4; set => RIB4 = (byte)((RIB4 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool HasContestMemoryRibbon      { get => (RIB4 & (1 << 5)) == 1 << 5; set => RIB4 = (byte)((RIB4 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool HasBattleMemoryRibbon       { get => (RIB4 & (1 << 6)) == 1 << 6; set => RIB4 = (byte)((RIB4 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonChampionG6Hoenn       { get => (RIB4 & (1 << 7)) == 1 << 7; set => RIB4 = (byte)((RIB4 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonContestStar           { get => (RIB5 & (1 << 0)) == 1 << 0; set => RIB5 = (byte)((RIB5 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonMasterCoolness        { get => (RIB5 & (1 << 1)) == 1 << 1; set => RIB5 = (byte)((RIB5 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonMasterBeauty          { get => (RIB5 & (1 << 2)) == 1 << 2; set => RIB5 = (byte)((RIB5 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonMasterCuteness        { get => (RIB5 & (1 << 3)) == 1 << 3; set => RIB5 = (byte)((RIB5 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonMasterCleverness      { get => (RIB5 & (1 << 4)) == 1 << 4; set => RIB5 = (byte)((RIB5 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonMasterToughness       { get => (RIB5 & (1 << 5)) == 1 << 5; set => RIB5 = (byte)((RIB5 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonChampionAlola         { get => (RIB5 & (1 << 6)) == 1 << 6; set => RIB5 = (byte)((RIB5 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonBattleRoyale          { get => (RIB5 & (1 << 7)) == 1 << 7; set => RIB5 = (byte)((RIB5 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonBattleTreeGreat       { get => (RIB6 & (1 << 0)) == 1 << 0; set => RIB6 = (byte)((RIB6 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonBattleTreeMaster      { get => (RIB6 & (1 << 1)) == 1 << 1; set => RIB6 = (byte)((RIB6 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RIB6_2                      { get => (RIB6 & (1 << 2)) == 1 << 2; set => RIB6 = (byte)((RIB6 & ~(1 << 2)) | (value ? 1 << 2 : 0)); } // Unused
    public bool RIB6_3                      { get => (RIB6 & (1 << 3)) == 1 << 3; set => RIB6 = (byte)((RIB6 & ~(1 << 3)) | (value ? 1 << 3 : 0)); } // Unused
    public bool RIB6_4                      { get => (RIB6 & (1 << 4)) == 1 << 4; set => RIB6 = (byte)((RIB6 & ~(1 << 4)) | (value ? 1 << 4 : 0)); } // Unused
    public bool RIB6_5                      { get => (RIB6 & (1 << 5)) == 1 << 5; set => RIB6 = (byte)((RIB6 & ~(1 << 5)) | (value ? 1 << 5 : 0)); } // Unused
    public bool RIB6_6                      { get => (RIB6 & (1 << 6)) == 1 << 6; set => RIB6 = (byte)((RIB6 & ~(1 << 6)) | (value ? 1 << 6 : 0)); } // Unused
    public bool RIB6_7                      { get => (RIB6 & (1 << 7)) == 1 << 7; set => RIB6 = (byte)((RIB6 & ~(1 << 7)) | (value ? 1 << 7 : 0)); } // Unused
    public byte RibbonCountMemoryContest { get => Data[0x38]; set => Data[0x38] = value; }
    public byte RibbonCountMemoryBattle  { get => Data[0x39]; set => Data[0x39] = value; }
    private ushort DistByte { get => ReadUInt16LittleEndian(Data.AsSpan(0x3A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x3A), value); }
    public bool DistSuperTrain1 { get => (DistByte & (1 << 0)) == 1 << 0; set => DistByte = (byte)((DistByte & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool DistSuperTrain2 { get => (DistByte & (1 << 1)) == 1 << 1; set => DistByte = (byte)((DistByte & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool DistSuperTrain3 { get => (DistByte & (1 << 2)) == 1 << 2; set => DistByte = (byte)((DistByte & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool DistSuperTrain4 { get => (DistByte & (1 << 3)) == 1 << 3; set => DistByte = (byte)((DistByte & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool DistSuperTrain5 { get => (DistByte & (1 << 4)) == 1 << 4; set => DistByte = (byte)((DistByte & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool DistSuperTrain6 { get => (DistByte & (1 << 5)) == 1 << 5; set => DistByte = (byte)((DistByte & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool Dist7 { get => (DistByte & (1 << 6)) == 1 << 6; set => DistByte = (byte)((DistByte & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool Dist8 { get => (DistByte & (1 << 7)) == 1 << 7; set => DistByte = (byte)((DistByte & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public uint FormArgument { get => ReadUInt32LittleEndian(Data.AsSpan(0x3C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x3C), value); }
    public byte FormArgumentRemain { get => (byte)FormArgument; set => FormArgument = (FormArgument & ~0xFFu) | value; }
    public byte FormArgumentElapsed { get => (byte)(FormArgument >> 8); set => FormArgument = (FormArgument & ~0xFF00u) | (uint)(value << 8); }
    public byte FormArgumentMaximum { get => (byte)(FormArgument >> 16); set => FormArgument = (FormArgument & ~0xFF0000u) | (uint)(value << 16); }

    public int RibbonCount     => BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x30)) & 0b00000000_00000011__11111111_11111111__11111111_11111111__11111111_11111111);
    #endregion
    #region Block B
    public override string Nickname
    {
        get => StringConverter7.GetString(NicknameTrash);
        set
        {
            // For Pokémon with no nickname, and match their Chinese species name, we need to use the private codepoint range instead of unicode.
            // Can't use the stored language as it might have been traded & evolved -> mismatch; Gen8+ will match the origin language, not Gen7 :(
            if (!IsNicknamed)
            {
                // Detect the language of the species name.
                // If the species name is the same for Traditional and Simplified Chinese, we prefer the saved language.
                int lang = SpeciesName.GetSpeciesNameLanguage(Species, Language, value, 7);
                if (lang is (int)LanguageID.ChineseS or (int)LanguageID.ChineseT)
                {
                    StringConverter7.SetString(NicknameTrash, value, 12, lang, StringConverterOption.None, chinese: true);
                    return;
                }
            }
            StringConverter7.SetString(NicknameTrash, value, 12, 0, StringConverterOption.None);
        }
    }

    public override ushort Move1
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x5A));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x5A), value);
    }

    public override ushort Move2
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x5C));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x5C), value);
    }

    public override ushort Move3
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x5E));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x5E), value);
    }

    public override ushort Move4
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x60));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x60), value);
    }

    public override int Move1_PP { get => Data[0x62]; set => Data[0x62] = (byte)value; }
    public override int Move2_PP { get => Data[0x63]; set => Data[0x63] = (byte)value; }
    public override int Move3_PP { get => Data[0x64]; set => Data[0x64] = (byte)value; }
    public override int Move4_PP { get => Data[0x65]; set => Data[0x65] = (byte)value; }
    public override int Move1_PPUps { get => Data[0x66]; set => Data[0x66] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x67]; set => Data[0x67] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x68]; set => Data[0x68] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x69]; set => Data[0x69] = (byte)value; }

    public override ushort RelearnMove1
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x6A));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x6A), value);
    }

    public override ushort RelearnMove2
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x6C));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x6C), value);
    }

    public override ushort RelearnMove3
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x6E));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x6E), value);
    }

    public override ushort RelearnMove4
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x70));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x70), value);
    }

    public bool SecretSuperTrainingUnlocked { get => (Data[0x72] & 1) == 1; set => Data[0x72] = (byte)((Data[0x72] & ~1) | (value ? 1 : 0)); }
    public bool SecretSuperTrainingComplete { get => (Data[0x72] & 2) == 2; set => Data[0x72] = (byte)((Data[0x72] & ~2) | (value ? 2 : 0)); }
    // 0x73 Unused
    public override uint IV32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x74)); set => WriteUInt32LittleEndian(Data.AsSpan(0x74), value); }
    public override int IV_HP  { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
    public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
    public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
    public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
    public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
    public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }
    public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0u); }
    public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFFu) | (value ? 0x80000000u : 0u); }
    #endregion
    #region Block C
    public override string HandlingTrainerName
    {
        get => StringConverter7.GetString(HandlingTrainerTrash);
        set => StringConverter7.SetString(HandlingTrainerTrash, value, 12, 0, StringConverterOption.None);
    }

    public override byte HandlingTrainerGender { get => Data[0x92]; set => Data[0x92] = value; }
    public override byte CurrentHandler { get => Data[0x93]; set => Data[0x93] = value; }
    public byte Geo1_Region  { get => Data[0x94]; set => Data[0x94] = value; }
    public byte Geo1_Country { get => Data[0x95]; set => Data[0x95] = value; }
    public byte Geo2_Region  { get => Data[0x96]; set => Data[0x96] = value; }
    public byte Geo2_Country { get => Data[0x97]; set => Data[0x97] = value; }
    public byte Geo3_Region  { get => Data[0x98]; set => Data[0x98] = value; }
    public byte Geo3_Country { get => Data[0x99]; set => Data[0x99] = value; }
    public byte Geo4_Region  { get => Data[0x9A]; set => Data[0x9A] = value; }
    public byte Geo4_Country { get => Data[0x9B]; set => Data[0x9B] = value; }
    public byte Geo5_Region  { get => Data[0x9C]; set => Data[0x9C] = value; }
    public byte Geo5_Country { get => Data[0x9D]; set => Data[0x9D] = value; }
    // 0x9E Unused
    // 0x9F Unused
    // 0xA0 Unused
    // 0xA1 Unused
    public override byte HandlingTrainerFriendship { get => Data[0xA2]; set => Data[0xA2] = value; }
    public byte HandlingTrainerAffection { get => Data[0xA3]; set => Data[0xA3] = value; }
    public byte HandlingTrainerMemoryIntensity { get => Data[0xA4]; set => Data[0xA4] = value; }
    public byte HandlingTrainerMemory { get => Data[0xA5]; set => Data[0xA5] = value; }
    public byte HandlingTrainerMemoryFeeling { get => Data[0xA6]; set => Data[0xA6] = value; }
    // 0xA7 Unused
    public ushort HandlingTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data.AsSpan(0xA8)); set => WriteUInt16LittleEndian(Data.AsSpan(0xA8), value); }
    // 0xAA Unused
    // 0xAB Unused
    // 0xAC Unused
    // 0xAD Unused
    public override byte Fullness { get => Data[0xAE]; set => Data[0xAE] = value; }
    public override byte Enjoyment { get => Data[0xAF]; set => Data[0xAF] = value; }
    #endregion
    #region Block D
    public override string OriginalTrainerName
    {
        get => StringConverter7.GetString(OriginalTrainerTrash);
        set => StringConverter7.SetString(OriginalTrainerTrash, value, 12, 0, StringConverterOption.None);
    }

    public override byte OriginalTrainerFriendship { get => Data[0xCA]; set => Data[0xCA] = value; }
    public byte OriginalTrainerAffection { get => Data[0xCB]; set => Data[0xCB] = value; }
    public byte OriginalTrainerMemoryIntensity { get => Data[0xCC]; set => Data[0xCC] = value; }
    public byte OriginalTrainerMemory { get => Data[0xCD]; set => Data[0xCD] = value; }
    public ushort OriginalTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data.AsSpan(0xCE)); set => WriteUInt16LittleEndian(Data.AsSpan(0xCE), value); }
    public byte OriginalTrainerMemoryFeeling { get => Data[0xD0]; set => Data[0xD0] = value; }
    public override byte EggYear { get => Data[0xD1]; set => Data[0xD1] = value; }
    public override byte EggMonth { get => Data[0xD2]; set => Data[0xD2] = value; }
    public override byte EggDay { get => Data[0xD3]; set => Data[0xD3] = value; }
    public override byte MetYear { get => Data[0xD4]; set => Data[0xD4] = value; }
    public override byte MetMonth { get => Data[0xD5]; set => Data[0xD5] = value; }
    public override byte MetDay { get => Data[0xD6]; set => Data[0xD6] = value; }
    // Unused 0xD7
    public override ushort EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0xD8)); set => WriteUInt16LittleEndian(Data.AsSpan(0xD8), value); }
    public override ushort MetLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0xDA)); set => WriteUInt16LittleEndian(Data.AsSpan(0xDA), value); }
    public override byte Ball { get => Data[0xDC]; set => Data[0xDC] = value; }
    public override byte MetLevel { get => (byte)(Data[0xDD] & ~0x80); set => Data[0xDD] = (byte)((Data[0xDD] & 0x80) | value); }
    public override byte OriginalTrainerGender { get => (byte)(Data[0xDD] >> 7); set => Data[0xDD] = (byte)((Data[0xDD] & ~0x80) | (value << 7)); }
    public byte HyperTrainFlags { get => Data[0xDE]; set => Data[0xDE] = value; }
    public bool HT_HP  { get => ((HyperTrainFlags >> 0) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 0)) | ((value ? 1 : 0) << 0)); }
    public bool HT_ATK { get => ((HyperTrainFlags >> 1) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 1)) | ((value ? 1 : 0) << 1)); }
    public bool HT_DEF { get => ((HyperTrainFlags >> 2) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 2)) | ((value ? 1 : 0) << 2)); }
    public bool HT_SPA { get => ((HyperTrainFlags >> 3) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 3)) | ((value ? 1 : 0) << 3)); }
    public bool HT_SPD { get => ((HyperTrainFlags >> 4) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 4)) | ((value ? 1 : 0) << 4)); }
    public bool HT_SPE { get => ((HyperTrainFlags >> 5) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 5)) | ((value ? 1 : 0) << 5)); }
    public override GameVersion Version { get => (GameVersion)Data[0xDF]; set => Data[0xDF] = (byte)value; }
    public byte Country { get => Data[0xE0]; set => Data[0xE0] = value; }
    public byte Region { get => Data[0xE1]; set => Data[0xE1] = value; }
    public byte ConsoleRegion { get => Data[0xE2]; set => Data[0xE2] = value; }
    public override int Language { get => Data[0xE3]; set => Data[0xE3] = (byte)value; }
    #endregion
    #region Battle Stats
    public override int Status_Condition { get => ReadInt32LittleEndian(Data.AsSpan(0xE8)); set => WriteInt32LittleEndian(Data.AsSpan(0xE8), value); }
    public override byte Stat_Level { get => Data[0xEC]; set => Data[0xEC] = value; }
    public byte DirtType { get => Data[0xED]; set => Data[0xED] = value; }
    public byte DirtLocation { get => Data[0xEE]; set => Data[0xEE] = value; }
    // 0xEF unused
    public override int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data.AsSpan(0xF0)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF0), (ushort)value); }
    public override int Stat_HPMax { get => ReadUInt16LittleEndian(Data.AsSpan(0xF2)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF2), (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16LittleEndian(Data.AsSpan(0xF4)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF4), (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16LittleEndian(Data.AsSpan(0xF6)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF6), (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16LittleEndian(Data.AsSpan(0xF8)); set => WriteUInt16LittleEndian(Data.AsSpan(0xF8), (ushort)value); }
    public override int Stat_SPA { get => ReadUInt16LittleEndian(Data.AsSpan(0xFA)); set => WriteUInt16LittleEndian(Data.AsSpan(0xFA), (ushort)value); }
    public override int Stat_SPD { get => ReadUInt16LittleEndian(Data.AsSpan(0xFC)); set => WriteUInt16LittleEndian(Data.AsSpan(0xFC), (ushort)value); }
    #endregion

    private const int MedalCount = 30;
    public int SuperTrainingMedalCount(int lowBitCount = MedalCount) => BitOperations.PopCount((SuperTrainBitFlags >> 2) & (uint.MaxValue >> (MedalCount - lowBitCount)));

    public bool IsUntradedEvent6 => Geo1_Country == 0 && Geo1_Region == 0 && MetLocation / 10000 == 4 && Gen6;

    public int MarkingCount => 6;

    public MarkingColor GetMarking(int index)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (MarkingColor)((MarkingValue >> (index * 2)) & 3);
    }

    public void SetMarking(int index, MarkingColor value)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        var shift = index * 2;
        MarkingValue = (ushort)((MarkingValue & ~(0b11 << shift)) | (((byte)value & 3) << shift));
    }

    public MarkingColor MarkingCircle   { get => GetMarking(0); set => SetMarking(0, value); }
    public MarkingColor MarkingTriangle { get => GetMarking(1); set => SetMarking(1, value); }
    public MarkingColor MarkingSquare   { get => GetMarking(2); set => SetMarking(2, value); }
    public MarkingColor MarkingHeart    { get => GetMarking(3); set => SetMarking(3, value); }
    public MarkingColor MarkingStar     { get => GetMarking(4); set => SetMarking(4, value); }
    public MarkingColor MarkingDiamond  { get => GetMarking(5); set => SetMarking(5, value); }

    public void FixMemories()
    {
        if (IsEgg) // No memories if is egg.
        {
            HandlingTrainerMemoryVariable = HandlingTrainerFriendship = HandlingTrainerMemory = HandlingTrainerMemoryIntensity = HandlingTrainerMemoryFeeling = 0;
            /* OriginalTrainerFriendship */ OriginalTrainerMemoryVariable = OriginalTrainerMemory = OriginalTrainerMemoryIntensity = OriginalTrainerMemoryFeeling = HandlingTrainerAffection = OriginalTrainerAffection = 0;
            this.ClearGeoLocationData();

            // Clear Handler
            HandlingTrainerTrash.Clear();
            return;
        }

        if (IsUntraded)
            HandlingTrainerMemoryVariable = HandlingTrainerFriendship = HandlingTrainerMemory = HandlingTrainerMemoryIntensity = HandlingTrainerMemoryFeeling = HandlingTrainerAffection = 0;
        if (Generation < 6)
            /* OriginalTrainerAffection = */ OriginalTrainerMemoryVariable = OriginalTrainerMemory = OriginalTrainerMemoryIntensity = OriginalTrainerMemoryFeeling = 0;

        this.SanitizeGeoLocationData();

        if (Generation < 7) // must be transferred via bank, and must have memories
        {
            this.SetTradeMemoryHT6(true); // oh no, memories on Gen7 pk
            // geolocations cleared on 6->7, no need to set
        }
    }

    protected override bool TradeOT(ITrainerInfo tr)
    {
        // Check to see if the OT matches the SAV's OT info.
        if (!BelongsTo(tr))
            return false;

        CurrentHandler = 0;
        return true;
    }

    protected override void TradeHT(ITrainerInfo tr)
    {
        // No geolocations are set in-game -- except for bank transfers. Don't emulate bank transfers
        // this.TradeGeoLocation(tr.Country, tr.SubRegion);

        Span<char> ht = stackalloc char[TrashCharCountTrainer];
        var len = LoadString(HandlingTrainerTrash, ht);
        ht = ht[..len];

        var other = tr.OT;
        if (!ht.SequenceEqual(other))
        {
            HandlingTrainerName = other;
            HandlingTrainerFriendship = PersonalInfo.BaseFriendship;
            HandlingTrainerAffection = 0;
        }
        CurrentHandler = 1;
        HandlingTrainerGender = tr.Gender;
    }

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_7_USUM;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_7_USUM;
    public override int MaxAbilityID => Legal.MaxAbilityID_7_USUM;
    public override int MaxItemID => Legal.MaxItemID_7_USUM;
    public override int MaxBallID => Legal.MaxBallID_7;
    public override GameVersion MaxGameID => Legal.MaxGameID_7;

    internal void SetTransferLocale(int lang)
    {
        this.SetTradeMemoryHT6(bank: true); // oh no, memories on Gen7 pk
        RecentTrainerCache.SetConsoleRegionData3DS(this);
        RecentTrainerCache.SetFirstCountryRegion(this);
        if (lang == 1 && Country != 1) // Japan Only
        {
            ConsoleRegion = 1;
            Country = 1;
            Region = 0;
        }
    }

    internal void SetTransferIVs(int flawless, Random rnd)
    {
        Span<int> finalIVs = stackalloc int[6];
        for (var i = 0; i < finalIVs.Length; i++)
            finalIVs[i] = rnd.Next(32);
        for (var i = 0; i < flawless; i++)
            finalIVs[i] = 31;
        rnd.Shuffle(finalIVs);
        SetIVs(finalIVs);
    }

    internal void SetTransferPID(bool isShiny)
    {
        switch (isShiny ? Shiny.Always : Shiny.Never)
        {
            case Shiny.Always when !IsShiny: // Force Square
                var low = PID & 0xFFFF;
                PID = (low ^ TID16 ^ 0u) << 16 | low;
                break;
            case Shiny.Never when IsShiny: // Force Not Shiny
                PID ^= 0x1000_0000;
                break;
        }
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter7.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter7.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter7.SetString(destBuffer, value, maxLength, Language);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetTerminatorIndex(data);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetStringLength(data);
    public override int GetBytesPerChar() => 2;
}

public enum ResortEventState : byte
{
    NONE = 0,
    SEIKAKU = 1,
    CARE = 2,
    LIKE_RESORT = 3,
    LIKE_BATTLE = 4,
    LIKE_ADV = 5,
    GOOD_FRIEND = 6,
    GIM = 7,
    HOTSPA = 8,
    WILD = 9,
    WILD_LOVE = 10,
    WILD_LIVE = 11,
    POKEMAME_GET1 = 12,
    POKEMAME_GET2 = 13,
    POKEMAME_GET3 = 14,
    KINOMI_HELP = 15,
    PLAY_STATE = 16,
    HOTSPA_STATE = 17,
    HOTSPA_DIZZY = 18,
    HOTSPA_EGG_HATCHING = 19,
    MAX = 20,
}
