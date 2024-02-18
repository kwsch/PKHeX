using System;
using System.Numerics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Core game data storage, format 1.
/// </summary>
public sealed class GameDataCore : IHomeTrack, ISpeciesForm, ITrainerID, INature, IFatefulEncounter, IContestStats, IScaledSize, ITrainerMemories, IHandlerLanguage, IBattleVersion, IHyperTrain, IFormArgument, IFavorite,
    IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetMemory6, IRibbonSetCommon7,
    IRibbonSetCommon8, IRibbonSetMark8,
    IRibbonSetCommon9, IRibbonSetMark9,
    IAppliedMarkings7
{
    // Internal Attributes set on creation
    private readonly Memory<byte> Buffer; // Raw Storage
    public int SerializedSize => Buffer.Length;
    private Span<byte> Data => Buffer.Span;

    public GameDataCore(Memory<byte> buffer)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(buffer.Length, HomeCrypto.SIZE_CORE);
        Buffer = buffer;
    }

    public int WriteTo(Span<byte> result)
    {
        var span = Data;
        span.CopyTo(result);
        return span.Length;
    }

    public ulong Tracker { get => ReadUInt64LittleEndian(Data); set => WriteUInt64LittleEndian(Data, value); }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], value); }
    public bool IsBadEgg { get => Data[0x0C] != 0; set => Data[0x0C] = (byte)(value ? 1 : 0); }
    public ushort Species { get => ReadUInt16LittleEndian(Data[0x0D..]); set => WriteUInt16LittleEndian(Data[0x0D..], value); }
    public uint ID32 { get => ReadUInt32LittleEndian(Data[0x0F..]); set => WriteUInt32LittleEndian(Data[0x0F..], value); }
    public ushort TID16 { get => ReadUInt16LittleEndian(Data[0x0F..]); set => WriteUInt16LittleEndian(Data[0x0F..], value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[0x11..]); set => WriteUInt16LittleEndian(Data[0x11..], value); }
    public uint EXP { get => ReadUInt32LittleEndian(Data[0x13..]); set => WriteUInt32LittleEndian(Data[0x13..], value); }
    public bool IsFavorite { get => Data[0x17] != 0; set => Data[0x17] = (byte)(value ? 1 : 0); }
    public ushort MarkingValue { get => ReadUInt16LittleEndian(Data[0x18..]); set => WriteUInt16LittleEndian(Data[0x18..], value); }
    public uint PID { get => ReadUInt32LittleEndian(Data[0x1A..]); set => WriteUInt32LittleEndian(Data[0x1A..], value); }
    public Nature Nature            { get => (Nature)Data[0x1E];      set => Data[0x1E] = (byte)value; }
    public Nature StatNature        { get => (Nature)Data[0x1F];      set => Data[0x1F] = (byte)value; }
    public bool FatefulEncounter { get => Data[0x20] != 0; set => Data[0x20] = (byte)(value ? 1 : 0); }
    public byte Gender { get => Data[0x21]; set => Data[0x21] = value; }
    public byte Form  { get => Data[0x22]; set => WriteUInt16LittleEndian(Data[0x22..], value); }
    public int EV_HP  { get => Data[0x24]; set => Data[0x24] = (byte)value; }
    public int EV_ATK { get => Data[0x25]; set => Data[0x25] = (byte)value; }
    public int EV_DEF { get => Data[0x26]; set => Data[0x26] = (byte)value; }
    public int EV_SPE { get => Data[0x27]; set => Data[0x27] = (byte)value; }
    public int EV_SPA { get => Data[0x28]; set => Data[0x28] = (byte)value; }
    public int EV_SPD { get => Data[0x29]; set => Data[0x29] = (byte)value; }
    public byte ContestCool   { get => Data[0x2A]; set => Data[0x2A] = value; }
    public byte ContestBeauty { get => Data[0x2B]; set => Data[0x2B] = value; }
    public byte ContestCute   { get => Data[0x2C]; set => Data[0x2C] = value; }
    public byte ContestSmart  { get => Data[0x2D]; set => Data[0x2D] = value; }
    public byte ContestTough  { get => Data[0x2E]; set => Data[0x2E] = value; }
    public byte ContestSheen  { get => Data[0x2F]; set => Data[0x2F] = value; }

    private bool GetFlag(int offset, int bit) => FlagUtil.GetFlag(Data, offset, bit);
    private void SetFlag(int offset, int bit, bool value) => FlagUtil.SetFlag(Data, offset, bit, value);

    public bool RibbonChampionKalos   { get => GetFlag(0x30, 0); set => SetFlag(0x30, 0, value); }
    public bool RibbonChampionG3      { get => GetFlag(0x30, 1); set => SetFlag(0x30, 1, value); }
    public bool RibbonChampionSinnoh  { get => GetFlag(0x30, 2); set => SetFlag(0x30, 2, value); }
    public bool RibbonBestFriends     { get => GetFlag(0x30, 3); set => SetFlag(0x30, 3, value); }
    public bool RibbonTraining        { get => GetFlag(0x30, 4); set => SetFlag(0x30, 4, value); }
    public bool RibbonBattlerSkillful { get => GetFlag(0x30, 5); set => SetFlag(0x30, 5, value); }
    public bool RibbonBattlerExpert   { get => GetFlag(0x30, 6); set => SetFlag(0x30, 6, value); }
    public bool RibbonEffort          { get => GetFlag(0x30, 7); set => SetFlag(0x30, 7, value); }

    public bool RibbonAlert    { get => GetFlag(0x31, 0); set => SetFlag(0x31, 0, value); }
    public bool RibbonShock    { get => GetFlag(0x31, 1); set => SetFlag(0x31, 1, value); }
    public bool RibbonDowncast { get => GetFlag(0x31, 2); set => SetFlag(0x31, 2, value); }
    public bool RibbonCareless { get => GetFlag(0x31, 3); set => SetFlag(0x31, 3, value); }
    public bool RibbonRelax    { get => GetFlag(0x31, 4); set => SetFlag(0x31, 4, value); }
    public bool RibbonSnooze   { get => GetFlag(0x31, 5); set => SetFlag(0x31, 5, value); }
    public bool RibbonSmile    { get => GetFlag(0x31, 6); set => SetFlag(0x31, 6, value); }
    public bool RibbonGorgeous { get => GetFlag(0x31, 7); set => SetFlag(0x31, 7, value); }

    public bool RibbonRoyal         { get => GetFlag(0x32, 0); set => SetFlag(0x32, 0, value); }
    public bool RibbonGorgeousRoyal { get => GetFlag(0x32, 1); set => SetFlag(0x32, 1, value); }
    public bool RibbonArtist        { get => GetFlag(0x32, 2); set => SetFlag(0x32, 2, value); }
    public bool RibbonFootprint     { get => GetFlag(0x32, 3); set => SetFlag(0x32, 3, value); }
    public bool RibbonRecord        { get => GetFlag(0x32, 4); set => SetFlag(0x32, 4, value); }
    public bool RibbonLegend        { get => GetFlag(0x32, 5); set => SetFlag(0x32, 5, value); }
    public bool RibbonCountry       { get => GetFlag(0x32, 6); set => SetFlag(0x32, 6, value); }
    public bool RibbonNational      { get => GetFlag(0x32, 7); set => SetFlag(0x32, 7, value); }

    public bool RibbonEarth    { get => GetFlag(0x33, 0); set => SetFlag(0x33, 0, value); }
    public bool RibbonWorld    { get => GetFlag(0x33, 1); set => SetFlag(0x33, 1, value); }
    public bool RibbonClassic  { get => GetFlag(0x33, 2); set => SetFlag(0x33, 2, value); }
    public bool RibbonPremier  { get => GetFlag(0x33, 3); set => SetFlag(0x33, 3, value); }
    public bool RibbonEvent    { get => GetFlag(0x33, 4); set => SetFlag(0x33, 4, value); }
    public bool RibbonBirthday { get => GetFlag(0x33, 5); set => SetFlag(0x33, 5, value); }
    public bool RibbonSpecial  { get => GetFlag(0x33, 6); set => SetFlag(0x33, 6, value); }
    public bool RibbonSouvenir { get => GetFlag(0x33, 7); set => SetFlag(0x33, 7, value); }

    // ribbon u32
    public bool RibbonWishing          { get => GetFlag(0x34, 0); set => SetFlag(0x34, 0, value); }
    public bool RibbonChampionBattle   { get => GetFlag(0x34, 1); set => SetFlag(0x34, 1, value); }
    public bool RibbonChampionRegional { get => GetFlag(0x34, 2); set => SetFlag(0x34, 2, value); }
    public bool RibbonChampionNational { get => GetFlag(0x34, 3); set => SetFlag(0x34, 3, value); }
    public bool RibbonChampionWorld    { get => GetFlag(0x34, 4); set => SetFlag(0x34, 4, value); }
    public bool HasContestMemoryRibbon { get => GetFlag(0x34, 5); set => SetFlag(0x34, 5, value); }
    public bool HasBattleMemoryRibbon  { get => GetFlag(0x34, 6); set => SetFlag(0x34, 6, value); }
    public bool RibbonChampionG6Hoenn  { get => GetFlag(0x34, 7); set => SetFlag(0x34, 7, value); }

    public bool RibbonContestStar      { get => GetFlag(0x35, 0); set => SetFlag(0x35, 0, value); }
    public bool RibbonMasterCoolness   { get => GetFlag(0x35, 1); set => SetFlag(0x35, 1, value); }
    public bool RibbonMasterBeauty     { get => GetFlag(0x35, 2); set => SetFlag(0x35, 2, value); }
    public bool RibbonMasterCuteness   { get => GetFlag(0x35, 3); set => SetFlag(0x35, 3, value); }
    public bool RibbonMasterCleverness { get => GetFlag(0x35, 4); set => SetFlag(0x35, 4, value); }
    public bool RibbonMasterToughness  { get => GetFlag(0x35, 5); set => SetFlag(0x35, 5, value); }
    public bool RibbonChampionAlola    { get => GetFlag(0x35, 6); set => SetFlag(0x35, 6, value); }
    public bool RibbonBattleRoyale     { get => GetFlag(0x35, 7); set => SetFlag(0x35, 7, value); }

    public bool RibbonBattleTreeGreat  { get => GetFlag(0x36, 0); set => SetFlag(0x36, 0, value); }
    public bool RibbonBattleTreeMaster { get => GetFlag(0x36, 1); set => SetFlag(0x36, 1, value); }
    public bool RibbonChampionGalar    { get => GetFlag(0x36, 2); set => SetFlag(0x36, 2, value); }
    public bool RibbonTowerMaster      { get => GetFlag(0x36, 3); set => SetFlag(0x36, 3, value); }
    public bool RibbonMasterRank       { get => GetFlag(0x36, 4); set => SetFlag(0x36, 4, value); }
    public bool RibbonMarkLunchtime    { get => GetFlag(0x36, 5); set => SetFlag(0x36, 5, value); }
    public bool RibbonMarkSleepyTime   { get => GetFlag(0x36, 6); set => SetFlag(0x36, 6, value); }
    public bool RibbonMarkDusk         { get => GetFlag(0x36, 7); set => SetFlag(0x36, 7, value); }

    public bool RibbonMarkDawn         { get => GetFlag(0x37, 0); set => SetFlag(0x37, 0, value); }
    public bool RibbonMarkCloudy       { get => GetFlag(0x37, 1); set => SetFlag(0x37, 1, value); }
    public bool RibbonMarkRainy        { get => GetFlag(0x37, 2); set => SetFlag(0x37, 2, value); }
    public bool RibbonMarkStormy       { get => GetFlag(0x37, 3); set => SetFlag(0x37, 3, value); }
    public bool RibbonMarkSnowy        { get => GetFlag(0x37, 4); set => SetFlag(0x37, 4, value); }
    public bool RibbonMarkBlizzard     { get => GetFlag(0x37, 5); set => SetFlag(0x37, 5, value); }
    public bool RibbonMarkDry          { get => GetFlag(0x37, 6); set => SetFlag(0x37, 6, value); }
    public bool RibbonMarkSandstorm    { get => GetFlag(0x37, 7); set => SetFlag(0x37, 7, value); }

    public byte RibbonCountMemoryContest { get => Data[0x38]; set => HasContestMemoryRibbon = (Data[0x38] = value) != 0; }
    public byte RibbonCountMemoryBattle  { get => Data[0x39]; set => HasBattleMemoryRibbon  = (Data[0x39] = value) != 0; }
    // !!! no padding, unlike PKM formats!

    // 0x3E Ribbon 3
    public bool RibbonMarkMisty        { get => GetFlag(0x3A, 0); set => SetFlag(0x3A, 0, value); }
    public bool RibbonMarkDestiny      { get => GetFlag(0x3A, 1); set => SetFlag(0x3A, 1, value); }
    public bool RibbonMarkFishing      { get => GetFlag(0x3A, 2); set => SetFlag(0x3A, 2, value); }
    public bool RibbonMarkCurry        { get => GetFlag(0x3A, 3); set => SetFlag(0x3A, 3, value); }
    public bool RibbonMarkUncommon     { get => GetFlag(0x3A, 4); set => SetFlag(0x3A, 4, value); }
    public bool RibbonMarkRare         { get => GetFlag(0x3A, 5); set => SetFlag(0x3A, 5, value); }
    public bool RibbonMarkRowdy        { get => GetFlag(0x3A, 6); set => SetFlag(0x3A, 6, value); }
    public bool RibbonMarkAbsentMinded { get => GetFlag(0x3A, 7); set => SetFlag(0x3A, 7, value); }

    public bool RibbonMarkJittery     { get => GetFlag(0x3B, 0); set => SetFlag(0x3B, 0, value); }
    public bool RibbonMarkExcited     { get => GetFlag(0x3B, 1); set => SetFlag(0x3B, 1, value); }
    public bool RibbonMarkCharismatic { get => GetFlag(0x3B, 2); set => SetFlag(0x3B, 2, value); }
    public bool RibbonMarkCalmness    { get => GetFlag(0x3B, 3); set => SetFlag(0x3B, 3, value); }
    public bool RibbonMarkIntense     { get => GetFlag(0x3B, 4); set => SetFlag(0x3B, 4, value); }
    public bool RibbonMarkZonedOut    { get => GetFlag(0x3B, 5); set => SetFlag(0x3B, 5, value); }
    public bool RibbonMarkJoyful      { get => GetFlag(0x3B, 6); set => SetFlag(0x3B, 6, value); }
    public bool RibbonMarkAngry       { get => GetFlag(0x3B, 7); set => SetFlag(0x3B, 7, value); }

    public bool RibbonMarkSmiley       { get => GetFlag(0x3C, 0); set => SetFlag(0x3C, 0, value); }
    public bool RibbonMarkTeary        { get => GetFlag(0x3C, 1); set => SetFlag(0x3C, 1, value); }
    public bool RibbonMarkUpbeat       { get => GetFlag(0x3C, 2); set => SetFlag(0x3C, 2, value); }
    public bool RibbonMarkPeeved       { get => GetFlag(0x3C, 3); set => SetFlag(0x3C, 3, value); }
    public bool RibbonMarkIntellectual { get => GetFlag(0x3C, 4); set => SetFlag(0x3C, 4, value); }
    public bool RibbonMarkFerocious    { get => GetFlag(0x3C, 5); set => SetFlag(0x3C, 5, value); }
    public bool RibbonMarkCrafty       { get => GetFlag(0x3C, 6); set => SetFlag(0x3C, 6, value); }
    public bool RibbonMarkScowling     { get => GetFlag(0x3C, 7); set => SetFlag(0x3C, 7, value); }

    public bool RibbonMarkKindly       { get => GetFlag(0x3D, 0); set => SetFlag(0x3D, 0, value); }
    public bool RibbonMarkFlustered    { get => GetFlag(0x3D, 1); set => SetFlag(0x3D, 1, value); }
    public bool RibbonMarkPumpedUp     { get => GetFlag(0x3D, 2); set => SetFlag(0x3D, 2, value); }
    public bool RibbonMarkZeroEnergy   { get => GetFlag(0x3D, 3); set => SetFlag(0x3D, 3, value); }
    public bool RibbonMarkPrideful     { get => GetFlag(0x3D, 4); set => SetFlag(0x3D, 4, value); }
    public bool RibbonMarkUnsure       { get => GetFlag(0x3D, 5); set => SetFlag(0x3D, 5, value); }
    public bool RibbonMarkHumble       { get => GetFlag(0x3D, 6); set => SetFlag(0x3D, 6, value); }
    public bool RibbonMarkThorny       { get => GetFlag(0x3D, 7); set => SetFlag(0x3D, 7, value); }

    public bool RibbonMarkVigor        { get => GetFlag(0x3E, 0); set => SetFlag(0x3E, 0, value); }
    public bool RibbonMarkSlump        { get => GetFlag(0x3E, 1); set => SetFlag(0x3E, 1, value); }
    public bool RibbonHisui            { get => GetFlag(0x3E, 2); set => SetFlag(0x3E, 2, value); }
    public bool RibbonTwinklingStar    { get => GetFlag(0x3E, 3); set => SetFlag(0x3E, 3, value); }
    public bool RibbonChampionPaldea   { get => GetFlag(0x3E, 4); set => SetFlag(0x3E, 4, value); }
    public bool RibbonMarkJumbo        { get => GetFlag(0x3E, 5); set => SetFlag(0x3E, 5, value); }
    public bool RibbonMarkMini         { get => GetFlag(0x3E, 6); set => SetFlag(0x3E, 6, value); }
    public bool RibbonMarkItemfinder   { get => GetFlag(0x3E, 7); set => SetFlag(0x3E, 7, value); }

    public bool RibbonMarkPartner      { get => GetFlag(0x3F, 0); set => SetFlag(0x3F, 0, value); }
    public bool RibbonMarkGourmand     { get => GetFlag(0x3F, 1); set => SetFlag(0x3F, 1, value); }
    public bool RibbonOnceInALifetime  { get => GetFlag(0x3F, 2); set => SetFlag(0x3F, 2, value); }
    public bool RibbonMarkAlpha        { get => GetFlag(0x3F, 3); set => SetFlag(0x3F, 3, value); }
    public bool RibbonMarkMightiest    { get => GetFlag(0x3F, 4); set => SetFlag(0x3F, 4, value); }
    public bool RibbonMarkTitan        { get => GetFlag(0x3F, 5); set => SetFlag(0x3F, 5, value); }
    public bool RibbonPartner          { get => GetFlag(0x3F, 6); set => SetFlag(0x3F, 6, value); }
    public bool RIB45_7                { get => GetFlag(0x3F, 7); set => SetFlag(0x3F, 7, value); }

    public bool RIB46_0                { get => GetFlag(0x40, 0); set => SetFlag(0x40, 0, value); }
    public bool RIB46_1                { get => GetFlag(0x40, 1); set => SetFlag(0x40, 1, value); }
    public bool RIB46_2                { get => GetFlag(0x40, 2); set => SetFlag(0x40, 2, value); }
    public bool RIB46_3                { get => GetFlag(0x40, 3); set => SetFlag(0x40, 3, value); }
    public bool RIB46_4                { get => GetFlag(0x40, 4); set => SetFlag(0x40, 4, value); }
    public bool RIB46_5                { get => GetFlag(0x40, 5); set => SetFlag(0x40, 5, value); }
    public bool RIB46_6                { get => GetFlag(0x40, 6); set => SetFlag(0x40, 6, value); }
    public bool RIB46_7                { get => GetFlag(0x40, 7); set => SetFlag(0x40, 7, value); }

    public bool RIB47_0                { get => GetFlag(0x41, 0); set => SetFlag(0x41, 0, value); }
    public bool RIB47_1                { get => GetFlag(0x41, 1); set => SetFlag(0x41, 1, value); }
    public bool RIB47_2                { get => GetFlag(0x41, 2); set => SetFlag(0x41, 2, value); }
    public bool RIB47_3                { get => GetFlag(0x41, 3); set => SetFlag(0x41, 3, value); }
    public bool RIB47_4                { get => GetFlag(0x41, 4); set => SetFlag(0x41, 4, value); }
    public bool RIB47_5                { get => GetFlag(0x41, 5); set => SetFlag(0x41, 5, value); }
    public bool RIB47_6                { get => GetFlag(0x41, 6); set => SetFlag(0x41, 6, value); }
    public bool RIB47_7                { get => GetFlag(0x41, 7); set => SetFlag(0x41, 7, value); }

    public int RibbonCount     => BitOperations.PopCount(ReadUInt64LittleEndian(Data[0x30..]) & 0b00000000_00011111__11111111_11111111__11111111_11111111__11111111_11111111)
                                + BitOperations.PopCount(ReadUInt64LittleEndian(Data[0x3A..]) & 0b00000000_00000000__00000100_00011100__00000000_00000000__00000000_00000000);
    public int MarkCount       => BitOperations.PopCount(ReadUInt64LittleEndian(Data[0x30..]) & 0b11111111_11100000__00000000_00000000__00000000_00000000__00000000_00000000)
                                + BitOperations.PopCount(ReadUInt64LittleEndian(Data[0x3A..]) & 0b00000000_00000000__00111011_11100011__11111111_11111111__11111111_11111111);
    public int RibbonMarkCount => BitOperations.PopCount(ReadUInt64LittleEndian(Data[0x30..]) & 0b11111111_11111111__11111111_11111111__11111111_11111111__11111111_11111111)
                                + BitOperations.PopCount(ReadUInt64LittleEndian(Data[0x3A..]) & 0b00000000_00000000__00111111_11111111__11111111_11111111__11111111_11111111);

    public bool HasMarkEncounter8 => BitOperations.PopCount(ReadUInt64LittleEndian(Data[0x30..]) & 0b11111111_11100000__00000000_00000000__00000000_00000000__00000000_00000000)
                                   + BitOperations.PopCount(ReadUInt64LittleEndian(Data[0x3A..]) & 0b00000000_00000000__00000000_00000011__11111111_11111111__11111111_11111111) != 0;
    public bool HasMarkEncounter9 => (Data[0x3F] & 0b00111000) != 0;

    public byte HeightScalar { get => Data[0x42]; set => Data[0x42] = value; }
    public byte WeightScalar { get => Data[0x43]; set => Data[0x43] = value; }

    public Span<byte> NicknameTrash => Data.Slice(0x44, 26);

    public string Nickname
    {
        get => StringConverter8.GetString(NicknameTrash);
        set => StringConverter8.SetString(NicknameTrash, value, 12, StringConverterOption.None);
    }

    public int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data[0x5E..]); set => WriteUInt16LittleEndian(Data[0x5E..], (ushort)value); }
    public int IV_HP  { get => Data[0x60]; set => Data[0x60] = (byte)value; }
    public int IV_ATK { get => Data[0x61]; set => Data[0x61] = (byte)value; }
    public int IV_DEF { get => Data[0x62]; set => Data[0x62] = (byte)value; }
    public int IV_SPE { get => Data[0x63]; set => Data[0x63] = (byte)value; }
    public int IV_SPA { get => Data[0x64]; set => Data[0x64] = (byte)value; }
    public int IV_SPD { get => Data[0x65]; set => Data[0x65] = (byte)value; }
    public bool IsEgg { get => Data[0x66] != 0; set => Data[0x66] = (byte)(value ? 1 : 0); }
    public bool IsNicknamed { get => Data[0x67] != 0; set => Data[0x67] = (byte)(value ? 1 : 0); }
    public int Status_Condition { get => ReadInt32LittleEndian(Data[0x68..]); set => WriteInt32LittleEndian(Data[0x68..], value); }
    public Span<byte> HandlingTrainerTrash => Data.Slice(0x6C, 26);
    public string HandlingTrainerName
    {
        get => StringConverter8.GetString(HandlingTrainerTrash);
        set => StringConverter8.SetString(HandlingTrainerTrash, value, 12, StringConverterOption.None);
    }
    public byte HandlingTrainerGender      { get => Data[0x86]; set => Data[0x86] = value; }
    public byte HandlingTrainerLanguage   { get => Data[0x87]; set => Data[0x87] = value; }
    public byte CurrentHandler { get => Data[0x88]; set => Data[0x88] = value; }
    public ushort HandlingTrainerID   { get => ReadUInt16LittleEndian(Data[0x89..]); set => WriteUInt16LittleEndian(Data[0x89..], value); } // unused?
    public byte HandlingTrainerFriendship  { get => Data[0x8B]; set => Data[0x8B] = value; }
    public byte HandlingTrainerMemoryIntensity  { get => Data[0x8C]; set => Data[0x8C] = value; }
    public byte HandlingTrainerMemory     { get => Data[0x8D]; set => Data[0x8D] = value; }
    public byte HandlingTrainerMemoryFeeling    { get => Data[0x8E]; set => Data[0x8E] = value; }
    public ushort HandlingTrainerMemoryVariable  { get => ReadUInt16LittleEndian(Data[0x8F..]); set => WriteUInt16LittleEndian(Data[0x8F..], value); }
    public GameVersion Version        { get => (GameVersion)Data[0x91]; set => Data[0x91] = (byte)value; }
    public GameVersion BattleVersion  { get => (GameVersion)Data[0x92]; set => Data[0x92] = (byte)value; }
    public int Language       { get => Data[0x93]; set => Data[0x93] = (byte)value; }
    public uint FormArgument        { get => ReadUInt32LittleEndian(Data[0x94..]); set => WriteUInt32LittleEndian(Data[0x94..], value); }
    public byte FormArgumentRemain  { get => (byte)FormArgument; set => FormArgument = (FormArgument & ~0xFFu) | value; }
    public byte FormArgumentElapsed { get => (byte)(FormArgument >> 8); set => FormArgument = (FormArgument & ~0xFF00u) | (uint)(value << 8); }
    public byte FormArgumentMaximum { get => (byte)(FormArgument >> 16); set => FormArgument = (FormArgument & ~0xFF0000u) | (uint)(value << 16); }
    public sbyte AffixedRibbon      { get => (sbyte)Data[0x98]; set => Data[0x98] = (byte)value; } // selected ribbon
    public Span<byte> OriginalTrainerTrash => Data.Slice(0x99, 26);
    public string OriginalTrainerName
    {
        get => StringConverter8.GetString(OriginalTrainerTrash);
        set => StringConverter8.SetString(OriginalTrainerTrash, value, 12, StringConverterOption.None);
    }
    public byte OriginalTrainerFriendship    { get => Data[0xB3]; set => Data[0xB3] = value; }
    public byte OriginalTrainerMemoryIntensity    { get => Data[0xB4]; set => Data[0xB4] = value; }
    public byte OriginalTrainerMemory       { get => Data[0xB5]; set => Data[0xB5] = value; }
    public ushort OriginalTrainerMemoryVariable    { get => ReadUInt16LittleEndian(Data[0xB6..]); set => WriteUInt16LittleEndian(Data[0xB6..], value); }
    public byte OriginalTrainerMemoryFeeling      { get => Data[0xB8]; set => Data[0xB8] = value; }
    public byte EggYear         { get => Data[0xB9]; set => Data[0xB9] = value; }
    public byte EggMonth        { get => Data[0xBA]; set => Data[0xBA] = value; }
    public byte EggDay          { get => Data[0xBB]; set => Data[0xBB] = value; }
    public byte MetYear         { get => Data[0xBC]; set => Data[0xBC] = value; }
    public byte MetMonth        { get => Data[0xBD]; set => Data[0xBD] = value; }
    public byte MetDay          { get => Data[0xBE]; set => Data[0xBE] = value; }
    public byte MetLevel        { get => Data[0xBF]; set => Data[0xBF] = value; }
    public byte OriginalTrainerGender        { get => Data[0xC0]; set => Data[0xC0] = value; }
    public byte HyperTrainFlags { get => Data[0xC1]; set => Data[0xC1] = value; }
    public bool HT_HP { get => ((HyperTrainFlags >> 0) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 0)) | ((value ? 1 : 0) << 0)); }
    public bool HT_ATK { get => ((HyperTrainFlags >> 1) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 1)) | ((value ? 1 : 0) << 1)); }
    public bool HT_DEF { get => ((HyperTrainFlags >> 2) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 2)) | ((value ? 1 : 0) << 2)); }
    public bool HT_SPA { get => ((HyperTrainFlags >> 3) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 3)) | ((value ? 1 : 0) << 3)); }
    public bool HT_SPD { get => ((HyperTrainFlags >> 4) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 4)) | ((value ? 1 : 0) << 4)); }
    public bool HT_SPE { get => ((HyperTrainFlags >> 5) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 5)) | ((value ? 1 : 0) << 5)); }

    public int HeldItem { get => ReadUInt16LittleEndian(Data[0xC2..]); set => WriteUInt16LittleEndian(Data[0xC2..], (ushort)value); }

    public TrainerIDFormat TrainerIDDisplayFormat => (Version).GetGeneration() >= 7 ? TrainerIDFormat.SixDigit : TrainerIDFormat.SixteenBit;

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

    public void CopyFrom(PKM pk)
    {
        EncryptionConstant = pk.EncryptionConstant;
        PID = pk.PID;
        Species = pk.Species;
        Form = pk.Form;
        Gender = pk.Gender;
        TID16 = pk.TID16;
        SID16 = pk.SID16;
        EXP = pk.EXP;
        MarkingValue = pk is IAppliedMarkings7 m7 ? m7.MarkingValue : (ushort)0;
        Nature = pk.Nature;
        StatNature = pk.StatNature;
        FatefulEncounter = pk.FatefulEncounter;
        // HeldItem = pk.HeldItem;
        IV_HP = pk.IV_HP;
        IV_ATK = pk.IV_ATK;
        IV_DEF = pk.IV_DEF;
        IV_SPE = pk.IV_SPE;
        IV_SPA = pk.IV_SPA;
        IV_SPD = pk.IV_SPD;
        IsEgg = pk.IsEgg;
        IsNicknamed = pk.IsNicknamed;
        EV_HP = pk.EV_HP;
        EV_ATK = pk.EV_ATK;
        EV_DEF = pk.EV_DEF;
        EV_SPE = pk.EV_SPE;
        EV_SPA = pk.EV_SPA;
        EV_SPD = pk.EV_SPD;

        HandlingTrainerGender = pk.HandlingTrainerGender;
        CurrentHandler = pk.CurrentHandler;
        // pk.HandlingTrainerID
        HandlingTrainerFriendship = pk.HandlingTrainerFriendship;

        Version = pk.Version;
        Language = pk.Language;

        OriginalTrainerFriendship = pk.OriginalTrainerFriendship;
        EggYear = pk.EggYear;
        EggMonth = pk.EggMonth;
        EggDay = pk.EggDay;
        MetYear = pk.MetYear;
        MetMonth = pk.MetMonth;
        MetDay = pk.MetDay;
        MetLevel = pk.MetLevel;
        OriginalTrainerGender = pk.OriginalTrainerGender;

        CopyConditionalInterfaceFrom(pk);

        pk.OriginalTrainerTrash.CopyTo(OriginalTrainerTrash);
        pk.NicknameTrash.CopyTo(NicknameTrash);
        pk.HandlingTrainerTrash.CopyTo(HandlingTrainerTrash);

        CopyConditionalRibbonMarkFrom(pk);
    }

    public void CopyTo(PKM pk)
    {
        pk.EncryptionConstant = EncryptionConstant;
        pk.PID = PID;
        pk.Species = Species;
        pk.Form = Form;
        pk.Gender = Gender;
        pk.TID16 = TID16;
        pk.SID16 = SID16;
        pk.EXP = EXP;
        if (pk is IAppliedMarkings7 m7)
            m7.MarkingValue = MarkingValue;
        pk.Nature = Nature;
        pk.StatNature = StatNature;
        pk.FatefulEncounter = FatefulEncounter;
        pk.HeldItem = HeldItem;
        pk.IV_HP  = IV_HP;
        pk.IV_ATK = IV_ATK;
        pk.IV_DEF = IV_DEF;
        pk.IV_SPE = IV_SPE;
        pk.IV_SPA = IV_SPA;
        pk.IV_SPD = IV_SPD;
        pk.IsEgg = IsEgg;
        pk.IsNicknamed = IsNicknamed;
        pk.EV_HP  = EV_HP;
        pk.EV_ATK = EV_ATK;
        pk.EV_DEF = EV_DEF;
        pk.EV_SPE = EV_SPE;
        pk.EV_SPA = EV_SPA;
        pk.EV_SPD = EV_SPD;

        pk.HandlingTrainerGender = HandlingTrainerGender;
        pk.CurrentHandler = CurrentHandler;
        // pk.HandlingTrainerID
        pk.HandlingTrainerFriendship = HandlingTrainerFriendship;

        pk.Version = Version;
        pk.Language = Language;

        pk.OriginalTrainerFriendship = OriginalTrainerFriendship;
        pk.EggYear = EggYear;
        pk.EggMonth = EggMonth;
        pk.EggDay = EggDay;
        pk.MetYear = MetYear;
        pk.MetMonth = MetMonth;
        pk.MetDay = MetDay;
        pk.MetLevel = MetLevel;
        pk.OriginalTrainerGender = OriginalTrainerGender;

        CopyConditionalInterfaceTo(pk);

        OriginalTrainerTrash.CopyTo(pk.OriginalTrainerTrash);
        NicknameTrash.CopyTo(pk.NicknameTrash);
        HandlingTrainerTrash.CopyTo(pk.HandlingTrainerTrash);

        CopyConditionalRibbonMarkTo(pk);
    }

    private void CopyConditionalInterfaceTo(PKM pk)
    {
        if (pk is IScaledSize ss)
        {
            ss.HeightScalar = HeightScalar;
            ss.WeightScalar = WeightScalar;
        }

        if (pk is IMemoryOT ot)
        {
            ot.OriginalTrainerMemoryIntensity = OriginalTrainerMemoryIntensity;
            ot.OriginalTrainerMemory = OriginalTrainerMemory;
            ot.OriginalTrainerMemoryVariable = OriginalTrainerMemoryVariable;
            ot.OriginalTrainerMemoryFeeling = OriginalTrainerMemoryFeeling;
        }
        if (pk is IMemoryHT hm)
        {
            hm.HandlingTrainerMemoryIntensity = HandlingTrainerMemoryIntensity;
            hm.HandlingTrainerMemory = HandlingTrainerMemory;
            hm.HandlingTrainerMemoryFeeling = HandlingTrainerMemoryFeeling;
            hm.HandlingTrainerMemoryVariable = HandlingTrainerMemoryVariable;
        }
        if (pk is IHandlerLanguage hl)
            hl.HandlingTrainerLanguage = HandlingTrainerLanguage;

        if (pk is IContestStats cm)
            this.CopyContestStatsTo(cm);
        if (pk is IRibbonSetAffixed affix)
            affix.AffixedRibbon = AffixedRibbon;
        if (pk is IHyperTrain hy)
            hy.HyperTrainFlags = HyperTrainFlags;
        if (pk is IFormArgument fa)
            fa.FormArgument = FormArgument;
        if (pk is IBattleVersion bv)
            bv.BattleVersion = BattleVersion;
        if (pk is IFavorite fav)
            fav.IsFavorite = IsFavorite;
        if (pk is IHomeTrack home)
            home.Tracker = Tracker;
    }

    private void CopyConditionalInterfaceFrom(PKM pk)
    {
        if (pk is IScaledSize ss)
        {
            HeightScalar = ss.HeightScalar;
            WeightScalar = ss.WeightScalar;
        }

        if (pk is IMemoryOT ot)
        {
            OriginalTrainerMemoryIntensity = ot.OriginalTrainerMemoryIntensity;
            OriginalTrainerMemory = ot.OriginalTrainerMemory;
            OriginalTrainerMemoryVariable = ot.OriginalTrainerMemoryVariable;
            OriginalTrainerMemoryFeeling = ot.OriginalTrainerMemoryFeeling;
        }
        if (pk is IMemoryHT ht)
        {
            HandlingTrainerMemoryIntensity = ht.HandlingTrainerMemoryIntensity;
            HandlingTrainerMemory = ht.HandlingTrainerMemory;
            HandlingTrainerMemoryFeeling = ht.HandlingTrainerMemoryFeeling;
            HandlingTrainerMemoryVariable = ht.HandlingTrainerMemoryVariable;
        }
        if (pk is IHandlerLanguage hl)
            HandlingTrainerLanguage = hl.HandlingTrainerLanguage;

        if (pk is IContestStats cm)
            cm.CopyContestStatsTo(this);
        if (pk is IRibbonSetAffixed affix)
            AffixedRibbon = affix.AffixedRibbon;
        if (pk is IHyperTrain hy)
            HyperTrainFlags = hy.HyperTrainFlags;
        if (pk is IFormArgument fa)
            FormArgument = fa.FormArgument;
        if (pk is IBattleVersion bv)
            BattleVersion = bv.BattleVersion;
        if (pk is IFavorite fav)
            IsFavorite = fav.IsFavorite;
        if (pk is IHomeTrack home)
            Tracker = home.Tracker;
    }

    private void CopyConditionalRibbonMarkTo(PKM pk)
    {
        if (pk is IRibbonSetEvent3  e3) this.CopyRibbonSetEvent3 (e3);
        if (pk is IRibbonSetEvent4  e4) this.CopyRibbonSetEvent4 (e4);
        if (pk is IRibbonSetCommon3 c3) this.CopyRibbonSetCommon3(c3);
        if (pk is IRibbonSetCommon4 c4) this.CopyRibbonSetCommon4(c4);
        if (pk is IRibbonSetCommon6 c6) this.CopyRibbonSetCommon6(c6);
        if (pk is IRibbonSetMemory6 m6) this.CopyRibbonSetMemory6(m6);
        if (pk is IRibbonSetCommon7 c7) this.CopyRibbonSetCommon7(c7);
        if (pk is IRibbonSetCommon8 c8) this.CopyRibbonSetCommon8(c8);
        if (pk is IRibbonSetMark8   m8) this.CopyRibbonSetMark8  (m8);
        if (pk is IRibbonSetCommon9 c9) this.CopyRibbonSetCommon9(c9);
        if (pk is IRibbonSetMark9   m9) this.CopyRibbonSetMark9  (m9);
    }

    private void CopyConditionalRibbonMarkFrom(PKM pk)
    {
        if (pk is IRibbonSetEvent3  e3) e3.CopyRibbonSetEvent3 (this);
        if (pk is IRibbonSetEvent4  e4) e4.CopyRibbonSetEvent4 (this);
        if (pk is IRibbonSetCommon3 c3) c3.CopyRibbonSetCommon3(this);
        if (pk is IRibbonSetCommon4 c4) c4.CopyRibbonSetCommon4(this);
        if (pk is IRibbonSetCommon6 c6) c6.CopyRibbonSetCommon6(this);
        if (pk is IRibbonSetMemory6 m6) m6.CopyRibbonSetMemory6(this);
        if (pk is IRibbonSetCommon7 c7) c7.CopyRibbonSetCommon7(this);
        if (pk is IRibbonSetCommon8 c8) c8.CopyRibbonSetCommon8(this);
        if (pk is IRibbonSetMark8   m8) m8.CopyRibbonSetMark8  (this);
        if (pk is IRibbonSetCommon9 c9) c9.CopyRibbonSetCommon9(this);
        if (pk is IRibbonSetMark9   m9) m9.CopyRibbonSetMark9  (this);
    }
}
