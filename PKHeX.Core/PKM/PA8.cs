using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 8 <see cref="PKM"/> format. </summary>
public sealed class PA8 : PKM, ISanityChecksum,
    IGanbaru, IAlpha, INoble, ITechRecord, ISociability, IMoveShop8Mastery, IContestStats, IHyperTrain, IScaledSizeValue, IScaledSize3, IGigantamax, IFavorite, IDynamaxLevel, IHandlerLanguage, IFormArgument, IHomeTrack, IBattleVersion, ITrainerMemories, IPokerusStatus,
    IRibbonIndex, IRibbonSetAffixed, IRibbonSetRibbons, IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetMemory6, IRibbonSetCommon7, IRibbonSetCommon8, IRibbonSetMarks, IRibbonSetMark8, IRibbonSetCommon9, IRibbonSetMark9,
    IHandlerUpdate, IAppliedMarkings7
{
    public override ReadOnlySpan<ushort> ExtraBytes =>
    [
        0x17, 0x1A, 0x1B, 0x23, 0x33,
        0x4C, 0x4D, 0x4E, 0x4F,
        0x53,
        0xA0, 0xA1, 0xA2, 0xA3,
        0xAA, 0xAB,
        0xB4, 0xB5, 0xB6, 0xB7,
        0xD5,
        0xD6, 0xD7,
        0xDE, 0xDF, 0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xE9,
        0xF0, 0xF1,
        0xF3,
        0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF,
        0x100, 0x101, 0x102, 0x103, 0x104, 0x105, 0x106, 0x107, 0x018, 0x109, 0x10A, 0x10B, 0x10C, 0x10D, 0x10E, 0x10F,
        0x12D, 0x13C,
    ];

    public override PersonalInfo8LA PersonalInfo => PersonalTable.LA.GetFormEntry(Species, Form);
    public IPermitRecord Permit => PersonalInfo;

    public override EntityContext Context => EntityContext.Gen8a;
    public PA8() : base(PokeCrypto.SIZE_8APARTY) => AffixedRibbon = -1; // 00 would make it show Kalos Champion :)
    public PA8(byte[] data) : base(DecryptParty(data)) { }

    public override int SIZE_PARTY => PokeCrypto.SIZE_8APARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_8ASTORED;
    public override bool ChecksumValid => CalculateChecksum() == Checksum;
    public override PA8 Clone() => new((byte[])Data.Clone());

    private static byte[] DecryptParty(byte[] data)
    {
        PokeCrypto.DecryptIfEncrypted8A(ref data);
        Array.Resize(ref data, PokeCrypto.SIZE_8APARTY);
        return data;
    }

    private ushort CalculateChecksum() => Checksums.Add16(Data.AsSpan()[8..PokeCrypto.SIZE_8ASTORED]);

    // Simple Generated Attributes

    public override byte CurrentFriendship
    {
        get => CurrentHandler == 0 ? OriginalTrainerFriendship : HandlingTrainerFriendship;
        set { if (CurrentHandler == 0) OriginalTrainerFriendship = value; else HandlingTrainerFriendship = value; }
    }

    public override void RefreshChecksum() => Checksum = CalculateChecksum();
    public override bool Valid { get => Sanity == 0 && ChecksumValid; set { if (!value) return; Sanity = 0; RefreshChecksum(); } }

    // Trash Bytes
    public override Span<byte> NicknameTrash => Data.AsSpan(0x60, 26);
    public override Span<byte> HandlingTrainerTrash => Data.AsSpan(0xB8, 26);
    public override Span<byte> OriginalTrainerTrash => Data.AsSpan(0x110, 26);
    public override int TrashCharCountTrainer => 13;
    public override int TrashCharCountNickname => 13;

    // Maximums
    public override int MaxIV => 31;
    public override int MaxEV => EffortValues.Max252;
    public override int MaxStringLengthTrainer => 12;
    public override int MaxStringLengthNickname => 12;

    public override uint PSV => ((PID >> 16) ^ (PID & 0xFFFF)) >> 4;
    public override uint TSV => (uint)(TID16 ^ SID16) >> 4;
    public override bool IsUntraded => Data[0xB8] == 0 && Data[0xB8 + 1] == 0 && Format == Generation; // immediately terminated HandlingTrainerName data (\0)

    // Complex Generated Attributes
    public override int Characteristic => EntityCharacteristic.GetCharacteristic(EncryptionConstant, IV32);

    // Methods
    protected override byte[] Encrypt()
    {
        RefreshChecksum();
        return PokeCrypto.EncryptArray8A(Data);
    }

    public void FixRelearn()
    {
        while (true)
        {
            if (RelearnMove4 != 0 && RelearnMove3 == 0)
            {
                RelearnMove3 = RelearnMove4;
                RelearnMove4 = 0;
            }
            if (RelearnMove3 != 0 && RelearnMove2 == 0)
            {
                RelearnMove2 = RelearnMove3;
                RelearnMove3 = 0;
                continue;
            }
            if (RelearnMove2 != 0 && RelearnMove1 == 0)
            {
                RelearnMove1 = RelearnMove2;
                RelearnMove2 = 0;
                continue;
            }
            break;
        }
    }

    public override uint EncryptionConstant { get => ReadUInt32LittleEndian(Data.AsSpan(0x00)); set => WriteUInt32LittleEndian(Data.AsSpan(0x00), value); }
    public ushort Sanity { get => ReadUInt16LittleEndian(Data.AsSpan(0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(0x04), value); }
    public ushort Checksum { get => ReadUInt16LittleEndian(Data.AsSpan(0x06)); set => WriteUInt16LittleEndian(Data.AsSpan(0x06), value); }

    // Structure
    #region Block A
    public override ushort Species { get => ReadUInt16LittleEndian(Data.AsSpan(0x08)); set => WriteUInt16LittleEndian(Data.AsSpan(0x08), value); }
    public override int HeldItem { get => ReadUInt16LittleEndian(Data.AsSpan(0x0A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value); }
    public override uint ID32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x0C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x0C), value); }
    public override ushort TID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x0C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0C), value); }
    public override ushort SID16 { get => ReadUInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), value); }
    public override uint EXP { get => ReadUInt32LittleEndian(Data.AsSpan(0x10)); set => WriteUInt32LittleEndian(Data.AsSpan(0x10), value); }
    public override int Ability { get => ReadUInt16LittleEndian(Data.AsSpan(0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14), (ushort)value); }
    public override int AbilityNumber { get => Data[0x16] & 7; set => Data[0x16] = (byte)((Data[0x16] & ~7) | (value & 7)); }
    public bool IsFavorite { get => (Data[0x16] & 8) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~8) | ((value ? 1 : 0) << 3)); } // unused, was in LGPE but not in SWSH
    public bool CanGigantamax { get => (Data[0x16] & 16) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~16) | (value ? 16 : 0)); }
    public bool IsAlpha { get => (Data[0x16] & 32) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~32) | ((value ? 1 : 0) << 5)); }
    public bool IsNoble { get => (Data[0x16] & 64) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~64) | ((value ? 1 : 0) << 6)); }
    // 0x17 alignment unused
    public ushort MarkingValue { get => ReadUInt16LittleEndian(Data.AsSpan(0x18)); set => WriteUInt16LittleEndian(Data.AsSpan(0x18), value); }
    // 0x1A alignment unused
    // 0x1B alignment unused
    public override uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0x1C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x1C), value); }
    public override Nature Nature { get => (Nature)Data[0x20]; set => Data[0x20] = (byte)value; }
    public override Nature StatNature { get => (Nature)Data[0x21]; set => Data[0x21] = (byte)value; }
    public override bool FatefulEncounter { get => (Data[0x22] & 1) == 1; set => Data[0x22] = (byte)((Data[0x22] & ~0x01) | (value ? 1 : 0)); }
    public bool Flag2 { get => (Data[0x22] & 2) == 2; set => Data[0x22] = (byte)((Data[0x22] & ~0x02) | (value ? 2 : 0)); }
    public override byte Gender { get => (byte)((Data[0x22] >> 2) & 0x3); set => Data[0x22] = (byte)((Data[0x22] & 0xF3) | (value << 2)); }
    // 0x23 alignment unused

    public override byte Form { get => Data[0x24]; set => WriteUInt16LittleEndian(Data.AsSpan(0x24), value); }
    public override int EV_HP { get => Data[0x26]; set => Data[0x26] = (byte)value; }
    public override int EV_ATK { get => Data[0x27]; set => Data[0x27] = (byte)value; }
    public override int EV_DEF { get => Data[0x28]; set => Data[0x28] = (byte)value; }
    public override int EV_SPE { get => Data[0x29]; set => Data[0x29] = (byte)value; }
    public override int EV_SPA { get => Data[0x2A]; set => Data[0x2A] = (byte)value; }
    public override int EV_SPD { get => Data[0x2B]; set => Data[0x2B] = (byte)value; }
    public byte ContestCool { get => Data[0x2C]; set => Data[0x2C] = value; }
    public byte ContestBeauty { get => Data[0x2D]; set => Data[0x2D] = value; }
    public byte ContestCute { get => Data[0x2E]; set => Data[0x2E] = value; }
    public byte ContestSmart { get => Data[0x2F]; set => Data[0x2F] = value; }
    public byte ContestTough { get => Data[0x30]; set => Data[0x30] = value; }
    public byte ContestSheen { get => Data[0x31]; set => Data[0x31] = value; }
    public byte PokerusState { get => Data[0x32]; set => Data[0x32] = value; }
    public override int PokerusDays { get => PokerusState & 0xF; set => PokerusState = (byte)((PokerusState & ~0xF) | value); }
    public override int PokerusStrain { get => PokerusState >> 4; set => PokerusState = (byte)((PokerusState & 0xF) | (value << 4)); }
    // 0x33 unused padding

    // ribbon u32
    public bool RibbonChampionKalos { get => FlagUtil.GetFlag(Data, 0x34, 0); set => FlagUtil.SetFlag(Data, 0x34, 0, value); }
    public bool RibbonChampionG3 { get => FlagUtil.GetFlag(Data, 0x34, 1); set => FlagUtil.SetFlag(Data, 0x34, 1, value); }
    public bool RibbonChampionSinnoh { get => FlagUtil.GetFlag(Data, 0x34, 2); set => FlagUtil.SetFlag(Data, 0x34, 2, value); }
    public bool RibbonBestFriends { get => FlagUtil.GetFlag(Data, 0x34, 3); set => FlagUtil.SetFlag(Data, 0x34, 3, value); }
    public bool RibbonTraining { get => FlagUtil.GetFlag(Data, 0x34, 4); set => FlagUtil.SetFlag(Data, 0x34, 4, value); }
    public bool RibbonBattlerSkillful { get => FlagUtil.GetFlag(Data, 0x34, 5); set => FlagUtil.SetFlag(Data, 0x34, 5, value); }
    public bool RibbonBattlerExpert { get => FlagUtil.GetFlag(Data, 0x34, 6); set => FlagUtil.SetFlag(Data, 0x34, 6, value); }
    public bool RibbonEffort { get => FlagUtil.GetFlag(Data, 0x34, 7); set => FlagUtil.SetFlag(Data, 0x34, 7, value); }

    public bool RibbonAlert { get => FlagUtil.GetFlag(Data, 0x35, 0); set => FlagUtil.SetFlag(Data, 0x35, 0, value); }
    public bool RibbonShock { get => FlagUtil.GetFlag(Data, 0x35, 1); set => FlagUtil.SetFlag(Data, 0x35, 1, value); }
    public bool RibbonDowncast { get => FlagUtil.GetFlag(Data, 0x35, 2); set => FlagUtil.SetFlag(Data, 0x35, 2, value); }
    public bool RibbonCareless { get => FlagUtil.GetFlag(Data, 0x35, 3); set => FlagUtil.SetFlag(Data, 0x35, 3, value); }
    public bool RibbonRelax { get => FlagUtil.GetFlag(Data, 0x35, 4); set => FlagUtil.SetFlag(Data, 0x35, 4, value); }
    public bool RibbonSnooze { get => FlagUtil.GetFlag(Data, 0x35, 5); set => FlagUtil.SetFlag(Data, 0x35, 5, value); }
    public bool RibbonSmile { get => FlagUtil.GetFlag(Data, 0x35, 6); set => FlagUtil.SetFlag(Data, 0x35, 6, value); }
    public bool RibbonGorgeous { get => FlagUtil.GetFlag(Data, 0x35, 7); set => FlagUtil.SetFlag(Data, 0x35, 7, value); }

    public bool RibbonRoyal { get => FlagUtil.GetFlag(Data, 0x36, 0); set => FlagUtil.SetFlag(Data, 0x36, 0, value); }
    public bool RibbonGorgeousRoyal { get => FlagUtil.GetFlag(Data, 0x36, 1); set => FlagUtil.SetFlag(Data, 0x36, 1, value); }
    public bool RibbonArtist { get => FlagUtil.GetFlag(Data, 0x36, 2); set => FlagUtil.SetFlag(Data, 0x36, 2, value); }
    public bool RibbonFootprint { get => FlagUtil.GetFlag(Data, 0x36, 3); set => FlagUtil.SetFlag(Data, 0x36, 3, value); }
    public bool RibbonRecord { get => FlagUtil.GetFlag(Data, 0x36, 4); set => FlagUtil.SetFlag(Data, 0x36, 4, value); }
    public bool RibbonLegend { get => FlagUtil.GetFlag(Data, 0x36, 5); set => FlagUtil.SetFlag(Data, 0x36, 5, value); }
    public bool RibbonCountry { get => FlagUtil.GetFlag(Data, 0x36, 6); set => FlagUtil.SetFlag(Data, 0x36, 6, value); }
    public bool RibbonNational { get => FlagUtil.GetFlag(Data, 0x36, 7); set => FlagUtil.SetFlag(Data, 0x36, 7, value); }

    public bool RibbonEarth { get => FlagUtil.GetFlag(Data, 0x37, 0); set => FlagUtil.SetFlag(Data, 0x37, 0, value); }
    public bool RibbonWorld { get => FlagUtil.GetFlag(Data, 0x37, 1); set => FlagUtil.SetFlag(Data, 0x37, 1, value); }
    public bool RibbonClassic { get => FlagUtil.GetFlag(Data, 0x37, 2); set => FlagUtil.SetFlag(Data, 0x37, 2, value); }
    public bool RibbonPremier { get => FlagUtil.GetFlag(Data, 0x37, 3); set => FlagUtil.SetFlag(Data, 0x37, 3, value); }
    public bool RibbonEvent { get => FlagUtil.GetFlag(Data, 0x37, 4); set => FlagUtil.SetFlag(Data, 0x37, 4, value); }
    public bool RibbonBirthday { get => FlagUtil.GetFlag(Data, 0x37, 5); set => FlagUtil.SetFlag(Data, 0x37, 5, value); }
    public bool RibbonSpecial { get => FlagUtil.GetFlag(Data, 0x37, 6); set => FlagUtil.SetFlag(Data, 0x37, 6, value); }
    public bool RibbonSouvenir { get => FlagUtil.GetFlag(Data, 0x37, 7); set => FlagUtil.SetFlag(Data, 0x37, 7, value); }

    // ribbon u32
    public bool RibbonWishing { get => FlagUtil.GetFlag(Data, 0x38, 0); set => FlagUtil.SetFlag(Data, 0x38, 0, value); }
    public bool RibbonChampionBattle { get => FlagUtil.GetFlag(Data, 0x38, 1); set => FlagUtil.SetFlag(Data, 0x38, 1, value); }
    public bool RibbonChampionRegional { get => FlagUtil.GetFlag(Data, 0x38, 2); set => FlagUtil.SetFlag(Data, 0x38, 2, value); }
    public bool RibbonChampionNational { get => FlagUtil.GetFlag(Data, 0x38, 3); set => FlagUtil.SetFlag(Data, 0x38, 3, value); }
    public bool RibbonChampionWorld { get => FlagUtil.GetFlag(Data, 0x38, 4); set => FlagUtil.SetFlag(Data, 0x38, 4, value); }
    public bool HasContestMemoryRibbon { get => FlagUtil.GetFlag(Data, 0x38, 5); set => FlagUtil.SetFlag(Data, 0x38, 5, value); }
    public bool HasBattleMemoryRibbon { get => FlagUtil.GetFlag(Data, 0x38, 6); set => FlagUtil.SetFlag(Data, 0x38, 6, value); }
    public bool RibbonChampionG6Hoenn { get => FlagUtil.GetFlag(Data, 0x38, 7); set => FlagUtil.SetFlag(Data, 0x38, 7, value); }

    public bool RibbonContestStar { get => FlagUtil.GetFlag(Data, 0x39, 0); set => FlagUtil.SetFlag(Data, 0x39, 0, value); }
    public bool RibbonMasterCoolness { get => FlagUtil.GetFlag(Data, 0x39, 1); set => FlagUtil.SetFlag(Data, 0x39, 1, value); }
    public bool RibbonMasterBeauty { get => FlagUtil.GetFlag(Data, 0x39, 2); set => FlagUtil.SetFlag(Data, 0x39, 2, value); }
    public bool RibbonMasterCuteness { get => FlagUtil.GetFlag(Data, 0x39, 3); set => FlagUtil.SetFlag(Data, 0x39, 3, value); }
    public bool RibbonMasterCleverness { get => FlagUtil.GetFlag(Data, 0x39, 4); set => FlagUtil.SetFlag(Data, 0x39, 4, value); }
    public bool RibbonMasterToughness { get => FlagUtil.GetFlag(Data, 0x39, 5); set => FlagUtil.SetFlag(Data, 0x39, 5, value); }
    public bool RibbonChampionAlola { get => FlagUtil.GetFlag(Data, 0x39, 6); set => FlagUtil.SetFlag(Data, 0x39, 6, value); }
    public bool RibbonBattleRoyale { get => FlagUtil.GetFlag(Data, 0x39, 7); set => FlagUtil.SetFlag(Data, 0x39, 7, value); }

    public bool RibbonBattleTreeGreat { get => FlagUtil.GetFlag(Data, 0x3A, 0); set => FlagUtil.SetFlag(Data, 0x3A, 0, value); }
    public bool RibbonBattleTreeMaster { get => FlagUtil.GetFlag(Data, 0x3A, 1); set => FlagUtil.SetFlag(Data, 0x3A, 1, value); }
    public bool RibbonChampionGalar { get => FlagUtil.GetFlag(Data, 0x3A, 2); set => FlagUtil.SetFlag(Data, 0x3A, 2, value); }
    public bool RibbonTowerMaster { get => FlagUtil.GetFlag(Data, 0x3A, 3); set => FlagUtil.SetFlag(Data, 0x3A, 3, value); }
    public bool RibbonMasterRank { get => FlagUtil.GetFlag(Data, 0x3A, 4); set => FlagUtil.SetFlag(Data, 0x3A, 4, value); }
    public bool RibbonMarkLunchtime { get => FlagUtil.GetFlag(Data, 0x3A, 5); set => FlagUtil.SetFlag(Data, 0x3A, 5, value); }
    public bool RibbonMarkSleepyTime { get => FlagUtil.GetFlag(Data, 0x3A, 6); set => FlagUtil.SetFlag(Data, 0x3A, 6, value); }
    public bool RibbonMarkDusk { get => FlagUtil.GetFlag(Data, 0x3A, 7); set => FlagUtil.SetFlag(Data, 0x3A, 7, value); }

    public bool RibbonMarkDawn { get => FlagUtil.GetFlag(Data, 0x3B, 0); set => FlagUtil.SetFlag(Data, 0x3B, 0, value); }
    public bool RibbonMarkCloudy { get => FlagUtil.GetFlag(Data, 0x3B, 1); set => FlagUtil.SetFlag(Data, 0x3B, 1, value); }
    public bool RibbonMarkRainy { get => FlagUtil.GetFlag(Data, 0x3B, 2); set => FlagUtil.SetFlag(Data, 0x3B, 2, value); }
    public bool RibbonMarkStormy { get => FlagUtil.GetFlag(Data, 0x3B, 3); set => FlagUtil.SetFlag(Data, 0x3B, 3, value); }
    public bool RibbonMarkSnowy { get => FlagUtil.GetFlag(Data, 0x3B, 4); set => FlagUtil.SetFlag(Data, 0x3B, 4, value); }
    public bool RibbonMarkBlizzard { get => FlagUtil.GetFlag(Data, 0x3B, 5); set => FlagUtil.SetFlag(Data, 0x3B, 5, value); }
    public bool RibbonMarkDry { get => FlagUtil.GetFlag(Data, 0x3B, 6); set => FlagUtil.SetFlag(Data, 0x3B, 6, value); }
    public bool RibbonMarkSandstorm { get => FlagUtil.GetFlag(Data, 0x3B, 7); set => FlagUtil.SetFlag(Data, 0x3B, 7, value); }
    public byte RibbonCountMemoryContest { get => Data[0x3C]; set => HasContestMemoryRibbon = (Data[0x3C] = value) != 0; }
    public byte RibbonCountMemoryBattle  { get => Data[0x3D]; set => HasBattleMemoryRibbon  = (Data[0x3D] = value) != 0; }

    public ushort AlphaMove { get => ReadUInt16LittleEndian(Data.AsSpan(0x3E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x3E), value); }

    // 0x40 Ribbon 1
    public bool RibbonMarkMisty { get => FlagUtil.GetFlag(Data, 0x40, 0); set => FlagUtil.SetFlag(Data, 0x40, 0, value); }
    public bool RibbonMarkDestiny { get => FlagUtil.GetFlag(Data, 0x40, 1); set => FlagUtil.SetFlag(Data, 0x40, 1, value); }
    public bool RibbonMarkFishing { get => FlagUtil.GetFlag(Data, 0x40, 2); set => FlagUtil.SetFlag(Data, 0x40, 2, value); }
    public bool RibbonMarkCurry { get => FlagUtil.GetFlag(Data, 0x40, 3); set => FlagUtil.SetFlag(Data, 0x40, 3, value); }
    public bool RibbonMarkUncommon { get => FlagUtil.GetFlag(Data, 0x40, 4); set => FlagUtil.SetFlag(Data, 0x40, 4, value); }
    public bool RibbonMarkRare { get => FlagUtil.GetFlag(Data, 0x40, 5); set => FlagUtil.SetFlag(Data, 0x40, 5, value); }
    public bool RibbonMarkRowdy { get => FlagUtil.GetFlag(Data, 0x40, 6); set => FlagUtil.SetFlag(Data, 0x40, 6, value); }
    public bool RibbonMarkAbsentMinded { get => FlagUtil.GetFlag(Data, 0x40, 7); set => FlagUtil.SetFlag(Data, 0x40, 7, value); }

    public bool RibbonMarkJittery { get => FlagUtil.GetFlag(Data, 0x41, 0); set => FlagUtil.SetFlag(Data, 0x41, 0, value); }
    public bool RibbonMarkExcited { get => FlagUtil.GetFlag(Data, 0x41, 1); set => FlagUtil.SetFlag(Data, 0x41, 1, value); }
    public bool RibbonMarkCharismatic { get => FlagUtil.GetFlag(Data, 0x41, 2); set => FlagUtil.SetFlag(Data, 0x41, 2, value); }
    public bool RibbonMarkCalmness { get => FlagUtil.GetFlag(Data, 0x41, 3); set => FlagUtil.SetFlag(Data, 0x41, 3, value); }
    public bool RibbonMarkIntense { get => FlagUtil.GetFlag(Data, 0x41, 4); set => FlagUtil.SetFlag(Data, 0x41, 4, value); }
    public bool RibbonMarkZonedOut { get => FlagUtil.GetFlag(Data, 0x41, 5); set => FlagUtil.SetFlag(Data, 0x41, 5, value); }
    public bool RibbonMarkJoyful { get => FlagUtil.GetFlag(Data, 0x41, 6); set => FlagUtil.SetFlag(Data, 0x41, 6, value); }
    public bool RibbonMarkAngry { get => FlagUtil.GetFlag(Data, 0x41, 7); set => FlagUtil.SetFlag(Data, 0x41, 7, value); }

    public bool RibbonMarkSmiley { get => FlagUtil.GetFlag(Data, 0x42, 0); set => FlagUtil.SetFlag(Data, 0x42, 0, value); }
    public bool RibbonMarkTeary { get => FlagUtil.GetFlag(Data, 0x42, 1); set => FlagUtil.SetFlag(Data, 0x42, 1, value); }
    public bool RibbonMarkUpbeat { get => FlagUtil.GetFlag(Data, 0x42, 2); set => FlagUtil.SetFlag(Data, 0x42, 2, value); }
    public bool RibbonMarkPeeved { get => FlagUtil.GetFlag(Data, 0x42, 3); set => FlagUtil.SetFlag(Data, 0x42, 3, value); }
    public bool RibbonMarkIntellectual { get => FlagUtil.GetFlag(Data, 0x42, 4); set => FlagUtil.SetFlag(Data, 0x42, 4, value); }
    public bool RibbonMarkFerocious { get => FlagUtil.GetFlag(Data, 0x42, 5); set => FlagUtil.SetFlag(Data, 0x42, 5, value); }
    public bool RibbonMarkCrafty { get => FlagUtil.GetFlag(Data, 0x42, 6); set => FlagUtil.SetFlag(Data, 0x42, 6, value); }
    public bool RibbonMarkScowling { get => FlagUtil.GetFlag(Data, 0x42, 7); set => FlagUtil.SetFlag(Data, 0x42, 7, value); }

    public bool RibbonMarkKindly { get => FlagUtil.GetFlag(Data, 0x43, 0); set => FlagUtil.SetFlag(Data, 0x43, 0, value); }
    public bool RibbonMarkFlustered { get => FlagUtil.GetFlag(Data, 0x43, 1); set => FlagUtil.SetFlag(Data, 0x43, 1, value); }
    public bool RibbonMarkPumpedUp { get => FlagUtil.GetFlag(Data, 0x43, 2); set => FlagUtil.SetFlag(Data, 0x43, 2, value); }
    public bool RibbonMarkZeroEnergy { get => FlagUtil.GetFlag(Data, 0x43, 3); set => FlagUtil.SetFlag(Data, 0x43, 3, value); }
    public bool RibbonMarkPrideful { get => FlagUtil.GetFlag(Data, 0x43, 4); set => FlagUtil.SetFlag(Data, 0x43, 4, value); }
    public bool RibbonMarkUnsure { get => FlagUtil.GetFlag(Data, 0x43, 5); set => FlagUtil.SetFlag(Data, 0x43, 5, value); }
    public bool RibbonMarkHumble { get => FlagUtil.GetFlag(Data, 0x43, 6); set => FlagUtil.SetFlag(Data, 0x43, 6, value); }
    public bool RibbonMarkThorny { get => FlagUtil.GetFlag(Data, 0x43, 7); set => FlagUtil.SetFlag(Data, 0x43, 7, value); }
    // 0x44 Ribbon 2

    public bool RibbonMarkVigor      { get => FlagUtil.GetFlag(Data, 0x44, 0); set => FlagUtil.SetFlag(Data, 0x44, 0, value); }
    public bool RibbonMarkSlump      { get => FlagUtil.GetFlag(Data, 0x44, 1); set => FlagUtil.SetFlag(Data, 0x44, 1, value); }
    public bool RibbonHisui          { get => FlagUtil.GetFlag(Data, 0x44, 2); set => FlagUtil.SetFlag(Data, 0x44, 2, value); }
    public bool RibbonTwinklingStar  { get => FlagUtil.GetFlag(Data, 0x44, 3); set => FlagUtil.SetFlag(Data, 0x44, 3, value); }
    public bool RibbonChampionPaldea { get => FlagUtil.GetFlag(Data, 0x44, 4); set => FlagUtil.SetFlag(Data, 0x44, 4, value); }
    public bool RibbonMarkJumbo      { get => FlagUtil.GetFlag(Data, 0x44, 5); set => FlagUtil.SetFlag(Data, 0x44, 5, value); }
    public bool RibbonMarkMini       { get => FlagUtil.GetFlag(Data, 0x44, 6); set => FlagUtil.SetFlag(Data, 0x44, 6, value); }
    public bool RibbonMarkItemfinder { get => FlagUtil.GetFlag(Data, 0x44, 7); set => FlagUtil.SetFlag(Data, 0x44, 7, value); }

    public bool RibbonMarkPartner     { get => FlagUtil.GetFlag(Data, 0x45, 0); set => FlagUtil.SetFlag(Data, 0x45, 0, value); }
    public bool RibbonMarkGourmand    { get => FlagUtil.GetFlag(Data, 0x45, 1); set => FlagUtil.SetFlag(Data, 0x45, 1, value); }
    public bool RibbonOnceInALifetime { get => FlagUtil.GetFlag(Data, 0x45, 2); set => FlagUtil.SetFlag(Data, 0x45, 2, value); }
    public bool RibbonMarkAlpha       { get => FlagUtil.GetFlag(Data, 0x45, 3); set => FlagUtil.SetFlag(Data, 0x45, 3, value); }
    public bool RibbonMarkMightiest   { get => FlagUtil.GetFlag(Data, 0x45, 4); set => FlagUtil.SetFlag(Data, 0x45, 4, value); }
    public bool RibbonMarkTitan       { get => FlagUtil.GetFlag(Data, 0x45, 5); set => FlagUtil.SetFlag(Data, 0x45, 5, value); }
    public bool RibbonPartner         { get => FlagUtil.GetFlag(Data, 0x45, 6); set => FlagUtil.SetFlag(Data, 0x45, 6, value); }
    public bool RIB45_7 { get => FlagUtil.GetFlag(Data, 0x45, 7); set => FlagUtil.SetFlag(Data, 0x45, 7, value); }

    public bool RIB46_0 { get => FlagUtil.GetFlag(Data, 0x46, 0); set => FlagUtil.SetFlag(Data, 0x46, 0, value); }
    public bool RIB46_1 { get => FlagUtil.GetFlag(Data, 0x46, 1); set => FlagUtil.SetFlag(Data, 0x46, 1, value); }
    public bool RIB46_2 { get => FlagUtil.GetFlag(Data, 0x46, 2); set => FlagUtil.SetFlag(Data, 0x46, 2, value); }
    public bool RIB46_3 { get => FlagUtil.GetFlag(Data, 0x46, 3); set => FlagUtil.SetFlag(Data, 0x46, 3, value); }
    public bool RIB46_4 { get => FlagUtil.GetFlag(Data, 0x46, 4); set => FlagUtil.SetFlag(Data, 0x46, 4, value); }
    public bool RIB46_5 { get => FlagUtil.GetFlag(Data, 0x46, 5); set => FlagUtil.SetFlag(Data, 0x46, 5, value); }
    public bool RIB46_6 { get => FlagUtil.GetFlag(Data, 0x46, 6); set => FlagUtil.SetFlag(Data, 0x46, 6, value); }
    public bool RIB46_7 { get => FlagUtil.GetFlag(Data, 0x46, 7); set => FlagUtil.SetFlag(Data, 0x46, 7, value); }

    public bool RIB47_0 { get => FlagUtil.GetFlag(Data, 0x47, 0); set => FlagUtil.SetFlag(Data, 0x47, 0, value); }
    public bool RIB47_1 { get => FlagUtil.GetFlag(Data, 0x47, 1); set => FlagUtil.SetFlag(Data, 0x47, 1, value); }
    public bool RIB47_2 { get => FlagUtil.GetFlag(Data, 0x47, 2); set => FlagUtil.SetFlag(Data, 0x47, 2, value); }
    public bool RIB47_3 { get => FlagUtil.GetFlag(Data, 0x47, 3); set => FlagUtil.SetFlag(Data, 0x47, 3, value); }
    public bool RIB47_4 { get => FlagUtil.GetFlag(Data, 0x47, 4); set => FlagUtil.SetFlag(Data, 0x47, 4, value); }
    public bool RIB47_5 { get => FlagUtil.GetFlag(Data, 0x47, 5); set => FlagUtil.SetFlag(Data, 0x47, 5, value); }
    public bool RIB47_6 { get => FlagUtil.GetFlag(Data, 0x47, 6); set => FlagUtil.SetFlag(Data, 0x47, 6, value); }
    public bool RIB47_7 { get => FlagUtil.GetFlag(Data, 0x47, 7); set => FlagUtil.SetFlag(Data, 0x47, 7, value); }

    public int RibbonCount     => BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x34)) & 0b00000000_00011111__11111111_11111111__11111111_11111111__11111111_11111111)
                                + BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x40)) & 0b00000000_00000000__00000100_00011100__00000000_00000000__00000000_00000000);
    public int MarkCount       => BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x34)) & 0b11111111_11100000__00000000_00000000__00000000_00000000__00000000_00000000)
                                + BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x40)) & 0b00000000_00000000__00111011_11100011__11111111_11111111__11111111_11111111);
    public int RibbonMarkCount => BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x34)) & 0b11111111_11111111__11111111_11111111__11111111_11111111__11111111_11111111)
                                + BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x40)) & 0b00000000_00000000__00111111_11111111__11111111_11111111__11111111_11111111);

    public bool HasMarkEncounter8 => BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x34)) & 0b11111111_11100000__00000000_00000000__00000000_00000000__00000000_00000000)
                                   + BitOperations.PopCount(ReadUInt64LittleEndian(Data.AsSpan(0x40)) & 0b00000000_00000000__00000000_00000011__11111111_11111111__11111111_11111111) != 0;
    public bool HasMarkEncounter9 => (Data[0x45] & 0b00111000) != 0;

    public uint Sociability { get => ReadUInt32LittleEndian(Data.AsSpan(0x48)); set => WriteUInt32LittleEndian(Data.AsSpan(0x48), value); }

    // 0x4C-0x4F unused

    public byte HeightScalar { get => Data[0x50]; set => Data[0x50] = value; }
    public byte WeightScalar { get => Data[0x51]; set => Data[0x51] = value; }
    public byte Scale { get => Data[0x52]; set => Data[0x52] = value; }

    // 0x53 unused

    public override ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x54)); set => WriteUInt16LittleEndian(Data.AsSpan(0x54), value); }
    public override ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x56)); set => WriteUInt16LittleEndian(Data.AsSpan(0x56), value); }
    public override ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x58)); set => WriteUInt16LittleEndian(Data.AsSpan(0x58), value); }
    public override ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x5A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5A), value); }

    public override int Move1_PP { get => Data[0x5C]; set => Data[0x5C] = (byte)value; }
    public override int Move2_PP { get => Data[0x5D]; set => Data[0x5D] = (byte)value; }
    public override int Move3_PP { get => Data[0x5E]; set => Data[0x5E] = (byte)value; }
    public override int Move4_PP { get => Data[0x5F]; set => Data[0x5F] = (byte)value; }
    #endregion
    #region Block B
    public override string Nickname
    {
        get => StringConverter8.GetString(NicknameTrash);
        set => StringConverter8.SetString(NicknameTrash, value, 12, StringConverterOption.None);
    }

    // 2 bytes for \0, automatically handled above

    public override int Move1_PPUps { get => Data[0x86]; set => Data[0x86] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x87]; set => Data[0x87] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x88]; set => Data[0x88] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x89]; set => Data[0x89] = (byte)value; }

    public override ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x8A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x8A), value); }
    public override ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x8C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x8C), value); }
    public override ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x8E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x8E), value); }
    public override ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x90)); set => WriteUInt16LittleEndian(Data.AsSpan(0x90), value); }

    public override int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data.AsSpan(0x92)); set => WriteUInt16LittleEndian(Data.AsSpan(0x92), (ushort)value); }
    public uint IV32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x94)); set => WriteUInt32LittleEndian(Data.AsSpan(0x94), value); }
    public override int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
    public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
    public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
    public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
    public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
    public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }
    public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0u); }
    public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFFu) | (value ? 0x80000000u : 0u); }

    public byte DynamaxLevel { get => Data[0x98]; set => Data[0x98] = value; }

    public override int Status_Condition { get => ReadInt32LittleEndian(Data.AsSpan(0x9C)); set => WriteInt32LittleEndian(Data.AsSpan(0x9C), value); }
    public int UnkA0 { get => ReadInt32LittleEndian(Data.AsSpan(0xA0)); set => WriteInt32LittleEndian(Data.AsSpan(0xA0), value); }
    public byte GV_HP  { get => Data[0xA4]; set => Data[0xA4] = value; }
    public byte GV_ATK { get => Data[0xA5]; set => Data[0xA5] = value; }
    public byte GV_DEF { get => Data[0xA6]; set => Data[0xA6] = value; }
    public byte GV_SPE { get => Data[0xA7]; set => Data[0xA7] = value; }
    public byte GV_SPA { get => Data[0xA8]; set => Data[0xA8] = value; }
    public byte GV_SPD { get => Data[0xA9]; set => Data[0xA9] = value; }

    // 0xAA-0xAB unused

    public float HeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(0xAC)); set => WriteSingleLittleEndian(Data.AsSpan(0xAC), value); }
    public float WeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(0xB0)); set => WriteSingleLittleEndian(Data.AsSpan(0xB0), value); }

    // 0xB4-0xB7 unused

    #endregion
    #region Block C
    public override string HandlingTrainerName
    {
        get => StringConverter8.GetString(HandlingTrainerTrash);
        set => StringConverter8.SetString(HandlingTrainerTrash, value, 12, StringConverterOption.None);
    }

    public override byte HandlingTrainerGender { get => Data[0xD2]; set => Data[0xD2] = value; }
    public byte HandlingTrainerLanguage { get => Data[0xD3]; set => Data[0xD3] = value; }
    public override byte CurrentHandler { get => Data[0xD4]; set => Data[0xD4] = value; }
    // 0xD5 unused (alignment)
    public ushort HandlingTrainerID { get => ReadUInt16LittleEndian(Data.AsSpan(0xD6)); set => WriteUInt16LittleEndian(Data.AsSpan(0xD6), value); } // unused?
    public override byte HandlingTrainerFriendship { get => Data[0xD8]; set => Data[0xD8] = value; }
    public byte HandlingTrainerMemoryIntensity { get => Data[0xD9]; set => Data[0xD9] = value; }
    public byte HandlingTrainerMemory { get => Data[0xDA]; set => Data[0xDA] = value; }
    public byte HandlingTrainerMemoryFeeling { get => Data[0xDB]; set => Data[0xDB] = value; }
    public ushort HandlingTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data.AsSpan(0xDC)); set => WriteUInt16LittleEndian(Data.AsSpan(0xDC), value); }

    // 0xDE-0xEB unused

    public override byte Fullness { get => Data[0xEC]; set => Data[0xEC] = value; }
    public override byte Enjoyment { get => Data[0xED]; set => Data[0xED] = value; }
    public override GameVersion Version { get => (GameVersion)Data[0xEE]; set => Data[0xEE] = (byte)value; }
    public GameVersion BattleVersion { get => (GameVersion)Data[0xEF]; set => Data[0xEF] = (byte)value; }
    // public override byte Region { get => Data[0xF0]; set => Data[0xF0] = (byte)value; }
    // public override byte ConsoleRegion { get => Data[0xF1]; set => Data[0xF1] = (byte)value; }
    public override int Language { get => Data[0xF2]; set => Data[0xF2] = (byte)value; }
    public int UnkF3 { get => Data[0xF3]; set => Data[0xF3] = (byte)value; }
    public uint FormArgument { get => ReadUInt32LittleEndian(Data.AsSpan(0xF4)); set => WriteUInt32LittleEndian(Data.AsSpan(0xF4), value); }
    public byte FormArgumentRemain { get => (byte)FormArgument; set => FormArgument = (FormArgument & ~0xFFu) | value; }
    public byte FormArgumentElapsed { get => (byte)(FormArgument >> 8); set => FormArgument = (FormArgument & ~0xFF00u) | (uint)(value << 8); }
    public byte FormArgumentMaximum { get => (byte)(FormArgument >> 16); set => FormArgument = (FormArgument & ~0xFF0000u) | (uint)(value << 16); }
    public sbyte AffixedRibbon { get => (sbyte)Data[0xF8]; set => Data[0xF8] = (byte)value; } // selected ribbon
                                                                                              // remainder unused
    #endregion
    #region Block D
    public override string OriginalTrainerName
    {
        get => StringConverter8.GetString(OriginalTrainerTrash);
        set => StringConverter8.SetString(OriginalTrainerTrash, value, 12, StringConverterOption.None);
    }

    public override byte OriginalTrainerFriendship { get => Data[0x12A]; set => Data[0x12A] = value; }
    public byte OriginalTrainerMemoryIntensity { get => Data[0x12B]; set => Data[0x12B] = value; }
    public byte OriginalTrainerMemory { get => Data[0x12C]; set => Data[0x12C] = value; }
    // 0x12D unused align
    public ushort OriginalTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data.AsSpan(0x12E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x12E), value); }
    public byte OriginalTrainerMemoryFeeling { get => Data[0x130]; set => Data[0x130] = value; }
    public override byte EggYear { get => Data[0x131]; set => Data[0x131] = value; }
    public override byte EggMonth { get => Data[0x132]; set => Data[0x132] = value; }
    public override byte EggDay { get => Data[0x133]; set => Data[0x133] = value; }
    public override byte MetYear { get => Data[0x134]; set => Data[0x134] = value; }
    public override byte MetMonth { get => Data[0x135]; set => Data[0x135] = value; }
    public override byte MetDay { get => Data[0x136]; set => Data[0x136] = value; }
    public override byte Ball { get => Data[0x137]; set => Data[0x137] = value; }
    public override ushort EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0x138)); set => WriteUInt16LittleEndian(Data.AsSpan(0x138), value); }
    public override ushort MetLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0x13A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x13A), value); }
    // 0x13C unused align
    public override byte MetLevel { get => (byte)(Data[0x13D] & ~0x80); set => Data[0x13D] = (byte)((Data[0x13D] & 0x80) | value); }
    public override byte OriginalTrainerGender { get => (byte)(Data[0x13D] >> 7); set => Data[0x13D] = (byte)((Data[0x13D] & ~0x80) | (value << 7)); }
    public byte HyperTrainFlags { get => Data[0x13E]; set => Data[0x13E] = value; }
    public bool HT_HP  { get => ((HyperTrainFlags >> 0) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 0)) | ((value ? 1 : 0) << 0)); }
    public bool HT_ATK { get => ((HyperTrainFlags >> 1) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 1)) | ((value ? 1 : 0) << 1)); }
    public bool HT_DEF { get => ((HyperTrainFlags >> 2) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 2)) | ((value ? 1 : 0) << 2)); }
    public bool HT_SPA { get => ((HyperTrainFlags >> 3) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 3)) | ((value ? 1 : 0) << 3)); }
    public bool HT_SPD { get => ((HyperTrainFlags >> 4) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 4)) | ((value ? 1 : 0) << 4)); }
    public bool HT_SPE { get => ((HyperTrainFlags >> 5) & 1) == 1; set => HyperTrainFlags = (byte)((HyperTrainFlags & ~(1 << 5)) | ((value ? 1 : 0) << 5)); }

    public bool GetMoveRecordFlag(int index)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, 0x13F + ofs, index & 7);
    }

    public void SetMoveRecordFlag(int index, bool value = true)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, 0x13F + ofs, index & 7, value);
    }

    public Span<byte> MoveRecordFlags => Data.AsSpan(0x13F, 14);
    public bool GetMoveRecordFlagAny() => MoveRecordFlags.ContainsAnyExcept<byte>(0);
    public void ClearMoveRecordFlags() => MoveRecordFlags.Clear();

    public ulong Tracker
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(0x14D));
        set => WriteUInt64LittleEndian(Data.AsSpan(0x14D), value);
    }

    public Span<byte> PurchasedRecord => Data.AsSpan(0x155, 8);
    public bool GetPurchasedRecordFlag(int index) => FlagUtil.GetFlag(PurchasedRecord, index >> 3, index & 7);
    public void SetPurchasedRecordFlag(int index, bool value) => FlagUtil.SetFlag(PurchasedRecord, index >> 3, index & 7, value);
    public bool GetPurchasedRecordFlagAny() => PurchasedRecord.ContainsAnyExcept<byte>(0);
    public int GetPurchasedCount() => BitOperations.PopCount(ReadUInt64LittleEndian(PurchasedRecord));

    public Span<byte> MasteredRecord => Data.AsSpan(0x15D, 8);
    public bool GetMasteredRecordFlag(int index) => FlagUtil.GetFlag(MasteredRecord, index >> 3, index & 7);
    public void SetMasteredRecordFlag(int index, bool value) => FlagUtil.SetFlag(MasteredRecord, index >> 3, index & 7, value);
    public bool GetMasteredRecordFlagAny() => MasteredRecord.ContainsAnyExcept<byte>(0);

    #endregion
    #region Battle Stats
    public override byte Stat_Level { get => Data[0x168]; set => Data[0x168] = value; }
    // 0x149 unused alignment
    public override int Stat_HPMax { get => ReadUInt16LittleEndian(Data.AsSpan(0x16A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x16A), (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16LittleEndian(Data.AsSpan(0x16C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x16C), (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16LittleEndian(Data.AsSpan(0x16E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x16E), (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16LittleEndian(Data.AsSpan(0x170)); set => WriteUInt16LittleEndian(Data.AsSpan(0x170), (ushort)value); }
    public override int Stat_SPA { get => ReadUInt16LittleEndian(Data.AsSpan(0x172)); set => WriteUInt16LittleEndian(Data.AsSpan(0x172), (ushort)value); }
    public override int Stat_SPD { get => ReadUInt16LittleEndian(Data.AsSpan(0x174)); set => WriteUInt16LittleEndian(Data.AsSpan(0x174), (ushort)value); }
    #endregion

    public override void LoadStats(IBaseStat p, Span<ushort> stats)
    {
        int level = CurrentLevel;
        var nature = StatNature;

        stats[0] = (ushort)(GetGanbaruStat(p.HP, HT_HP ? 31 : IV_HP, GV_HP, level) + GetStatHp(p.HP, level));
        stats[1] = (ushort)(GetGanbaruStat(p.ATK, HT_ATK ? 31 : IV_ATK, GV_ATK, level) + GetStat(p.ATK, level, nature, 0));
        stats[2] = (ushort)(GetGanbaruStat(p.DEF, HT_DEF ? 31 : IV_DEF, GV_DEF, level) + GetStat(p.DEF, level, nature, 1));
        stats[3] = (ushort)(GetGanbaruStat(p.SPE, HT_SPE ? 31 : IV_SPE, GV_SPE, level) + GetStat(p.SPE, level, nature, 4));
        stats[4] = (ushort)(GetGanbaruStat(p.SPA, HT_SPA ? 31 : IV_SPA, GV_SPA, level) + GetStat(p.SPA, level, nature, 2));
        stats[5] = (ushort)(GetGanbaruStat(p.SPD, HT_SPD ? 31 : IV_SPD, GV_SPD, level) + GetStat(p.SPD, level, nature, 3));
    }

    public static int GetGanbaruStat(int baseStat, int iv, byte gv, int level)
    {
        var mul = GanbaruExtensions.GetGanbaruMultiplier(gv, iv);
        double step1 = Math.Abs(Math.Sqrt(baseStat)) * mul; // The game does abs after sqrt; should be before. It's fine because baseStat is never negative.
        var result = ((float)step1 + level) / 2.5f;
        return (int)Math.Round(result, MidpointRounding.AwayFromZero);
    }

    public static int GetStatHp(int baseStat, int level)
    {
        return (int)((((level / 100.0f) + 1.0f) * baseStat) + level);
    }

    public static int GetStat(int baseStat, int level, Nature nature, int statIndex)
    {
        var initial = (int)((((level / 50.0f) + 1.0f) * baseStat) / 1.5f);
        return NatureAmp.AmplifyStat(nature, statIndex, initial);
    }

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

    public bool GetRibbon(int index) => FlagUtil.GetFlag(Data, GetRibbonByte(index), index & 7);
    public void SetRibbon(int index, bool value = true) => FlagUtil.SetFlag(Data, GetRibbonByte(index), index & 7, value);

    public int GetRibbonByte(int index)
    {
        if ((uint)index >= 128)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (index < 64)
            return 0x34 + (index >> 3);
        index -= 64;
        return 0x40 + (index >> 3);
    }

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
        // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
        if (!TradeOT(tr))
            TradeHT(tr);
    }

    public void FixMemories()
    {
        if (LA)
        {
            this.ClearMemoriesOT();
            this.ClearMemoriesHT();
        }

        if (IsUntraded)
        {
            HandlingTrainerGender = HandlingTrainerFriendship = HandlingTrainerLanguage = 0;
            HandlingTrainerTrash.Clear();
        }
        else
        {
            var gen = Generation;
            if (gen < 6)
                this.ClearMemoriesOT();
        }
    }

    private bool TradeOT(ITrainerInfo tr)
    {
        // Check to see if the OT matches the SAV's OT info.
        if (!BelongsTo(tr))
            return false;

        CurrentHandler = 0;
        return true;
    }

    private void TradeHT(ITrainerInfo tr) => PKH.UpdateHandler(this, tr);

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_8a;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_8a;
    public override int MaxAbilityID => Legal.MaxAbilityID_8a;
    public override int MaxItemID => Legal.MaxItemID_8a;
    public override int MaxBallID => Legal.MaxBallID_8a;
    public override GameVersion MaxGameID => Legal.MaxGameID_HOME;

    public float HeightRatio => GetHeightRatio(HeightScalar);
    public float WeightRatio => GetWeightRatio(WeightScalar);

    public float CalcHeightAbsolute => GetHeightAbsolute(PersonalInfo, HeightScalar);
    public float CalcWeightAbsolute => GetWeightAbsolute(PersonalInfo, HeightScalar, WeightScalar);

    public void ResetHeight() => HeightAbsolute = CalcHeightAbsolute;
    public void ResetWeight() => WeightAbsolute = CalcWeightAbsolute;

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    private static float GetHeightRatio(byte heightScalar)
    {
        // +/- 20% (down from +/- 40% in LGP/E)
        float result = heightScalar / 255f; // 0x437F0000
        result *= 0.40000004f; // 0x3ECCCCCE
        result += 0.8f; // 0x3F4CCCCD
        return result;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    private static float GetWeightRatio(byte weightScalar)
    {
        // +/- 20%
        float result = weightScalar / 255f; // 0x437F0000
        result *= 0.40000004f; // 0x3ECCCCCE
        result += 0.8f; // 0x3F4CCCCD
        return result;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static float GetHeightAbsolute(IPersonalMisc p, byte heightScalar)
    {
        float HeightRatio = GetHeightRatio(heightScalar);
        return HeightRatio * p.Height;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static float GetWeightAbsolute(IPersonalMisc p, byte heightScalar, byte weightScalar)
    {
        float HeightRatio = GetHeightRatio(heightScalar);
        float WeightRatio = GetWeightRatio(weightScalar);

        float ratio = (WeightRatio * HeightRatio);
        return ratio * p.Weight;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static byte GetHeightScalar(float height, int avgHeight)
    {
        // height is already *100
        float biasH = avgHeight * -0.8f;
        float biasL = avgHeight * 0.40000004f;
        float numerator = biasH + height;
        float result = numerator / biasL;
        result *= 255f;
        int value = (int)result;
        int unsigned = value & ~(value >> 31);
        return (byte)Math.Min(255, unsigned);
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static byte GetWeightScalar(float height, float weight, int avgHeight, int avgWeight)
    {
        // height is already *100
        // weight is already *10
        float heightRatio = height / avgHeight;
        float weightComponent = heightRatio * weight;
        float top = avgWeight * -0.8f;
        top += weightComponent;
        float bot = avgWeight * 0.40000004f;
        float result = top / bot;
        result *= 255f;
        int value = (int)result;
        int unsigned = value & ~(value >> 31);
        return (byte)Math.Min(255, unsigned);
    }

    public void SetMasteryFlags()
    {
        for (int i = 0; i < 4; i++)
            SetMasteryFlagMove(GetMove(i));
    }

    public void SetMasteryFlagMove(ushort move)
    {
        var permit = Permit;
        var moves = permit.RecordPermitIndexes;
        int flagIndex = moves.IndexOf(move);
        if (flagIndex == -1)
            return;
        if (permit.IsRecordPermitted(flagIndex))
            SetMasteredRecordFlag(flagIndex, true);
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter8.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter8.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter8.SetString(destBuffer, value, maxLength, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetTerminatorIndex(data);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetStringLength(data);
    public override int GetBytesPerChar() => 2;
}
