using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 8 <see cref="PKM"/> format. </summary>
public sealed class PA8 : PKM, ISanityChecksum, IMoveReset,
    IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetMemory6, IRibbonSetCommon7, IRibbonSetCommon8, IRibbonSetMark8, IRibbonSetAffixed, IGanbaru, IAlpha, INoble, ITechRecord8, ISociability, IMoveShop8Mastery,
    IContestStats, IContestStatsMutable, IHyperTrain, IScaledSizeValue, IGigantamax, IFavorite, IDynamaxLevel, IRibbonIndex, IHandlerLanguage, IFormArgument, IHomeTrack, IBattleVersion, ITrainerMemories
{
    private static readonly ushort[] Unused =
    {
        0x17, 0x1A, 0x1B, 0x23, 0x33,
        0x4C, 0x4D, 0x4E, 0x4F,
        0x52, 0x53,
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
    };

    public override IReadOnlyList<ushort> ExtraBytes => Unused;
    public override PersonalInfo PersonalInfo => PersonalTable.LA.GetFormEntry(Species, Form);

    public override EntityContext Context => EntityContext.Gen8a;
    public override bool IsNative => LA;
    public PA8() : base(PokeCrypto.SIZE_8APARTY) => AffixedRibbon = -1; // 00 would make it show Kalos Champion :)
    public PA8(byte[] data) : base(DecryptParty(data)) { }

    public override int SIZE_PARTY => PokeCrypto.SIZE_8APARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_8ASTORED;
    public override bool ChecksumValid => CalculateChecksum() == Checksum;
    public override PKM Clone() => new PA8((byte[])Data.Clone());

    private static byte[] DecryptParty(byte[] data)
    {
        PokeCrypto.DecryptIfEncrypted8A(ref data);
        Array.Resize(ref data, PokeCrypto.SIZE_8APARTY);
        return data;
    }

    private ushort CalculateChecksum()
    {
        ushort chk = 0;
        for (int i = 8; i < PokeCrypto.SIZE_8ASTORED; i += 2)
            chk += ReadUInt16LittleEndian(Data.AsSpan(i));
        return chk;
    }

    // Simple Generated Attributes
    public ReadOnlySpan<bool> TechRecordPermitFlags => Span<bool>.Empty;
    public ReadOnlySpan<int> TechRecordPermitIndexes => LearnSource8SWSH.TR_SWSH.AsSpan();
    public ReadOnlySpan<bool> MoveShopPermitFlags => PersonalInfo.SpecialTutors[0];
    public ReadOnlySpan<ushort> MoveShopPermitIndexes => Legal.MoveShop8_LA;

    public override int CurrentFriendship
    {
        get => CurrentHandler == 0 ? OT_Friendship : HT_Friendship;
        set { if (CurrentHandler == 0) OT_Friendship = value; else HT_Friendship = value; }
    }

    public override void RefreshChecksum() => Checksum = CalculateChecksum();
    public override bool Valid { get => Sanity == 0 && ChecksumValid; set { if (!value) return; Sanity = 0; RefreshChecksum(); } }

    // Trash Bytes
    public override Span<byte> Nickname_Trash => Data.AsSpan(0x60, 26);
    public override Span<byte> HT_Trash => Data.AsSpan(0xB8, 26);
    public override Span<byte> OT_Trash => Data.AsSpan(0x110, 26);

    // Maximums
    public override int MaxIV => 31;
    public override int MaxEV => 252;
    public override int OTLength => 12;
    public override int NickLength => 12;

    public override int PSV => (int)(((PID >> 16) ^ (PID & 0xFFFF)) >> 4);
    public override int TSV => (TID ^ SID) >> 4;
    public override bool IsUntraded => Data[0xB8] == 0 && Data[0xB8 + 1] == 0 && Format == Generation; // immediately terminated HT_Name data (\0)

    // Complex Generated Attributes
    public override int Characteristic
    {
        get
        {
            int pm6 = (int)(EncryptionConstant % 6);
            int maxIV = MaximumIV;
            int pm6stat = 0;
            for (int i = 0; i < 6; i++)
            {
                pm6stat = (pm6 + i) % 6;
                if (GetIV(pm6stat) == maxIV)
                    break;
            }
            return (pm6stat * 5) + (maxIV % 5);
        }
    }

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
    public override int Species { get => ReadUInt16LittleEndian(Data.AsSpan(0x08)); set => WriteUInt16LittleEndian(Data.AsSpan(0x08), (ushort)value); }
    public override int HeldItem { get => ReadUInt16LittleEndian(Data.AsSpan(0x0A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value); }
    public override int TID { get => ReadUInt16LittleEndian(Data.AsSpan(0x0C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0C), (ushort)value); }
    public override int SID { get => ReadUInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), (ushort)value); }
    public override uint EXP { get => ReadUInt32LittleEndian(Data.AsSpan(0x10)); set => WriteUInt32LittleEndian(Data.AsSpan(0x10), value); }
    public override int Ability { get => ReadUInt16LittleEndian(Data.AsSpan(0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14), (ushort)value); }
    public override int AbilityNumber { get => Data[0x16] & 7; set => Data[0x16] = (byte)((Data[0x16] & ~7) | (value & 7)); }
    public bool Favorite { get => (Data[0x16] & 8) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~8) | ((value ? 1 : 0) << 3)); } // unused, was in LGPE but not in SWSH
    public bool CanGigantamax { get => (Data[0x16] & 16) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~16) | (value ? 16 : 0)); }
    public bool IsAlpha { get => (Data[0x16] & 32) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~32) | ((value ? 1 : 0) << 5)); }
    public bool IsNoble { get => (Data[0x16] & 64) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~64) | ((value ? 1 : 0) << 6)); }
    // 0x17 alignment unused
    public override int MarkValue { get => ReadUInt16LittleEndian(Data.AsSpan(0x18)); set => WriteUInt16LittleEndian(Data.AsSpan(0x18), (ushort)value); }
    // 0x1A alignment unused
    // 0x1B alignment unused
    public override uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0x1C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x1C), value); }
    public override int Nature { get => Data[0x20]; set => Data[0x20] = (byte)value; }
    public override int StatNature { get => Data[0x21]; set => Data[0x21] = (byte)value; }
    public override bool FatefulEncounter { get => (Data[0x22] & 1) == 1; set => Data[0x22] = (byte)((Data[0x22] & ~0x01) | (value ? 1 : 0)); }
    public bool Flag2 { get => (Data[0x22] & 2) == 2; set => Data[0x22] = (byte)((Data[0x22] & ~0x02) | (value ? 2 : 0)); }
    public override int Gender { get => (Data[0x22] >> 2) & 0x3; set => Data[0x22] = (byte)((Data[0x22] & 0xF3) | (value << 2)); }
    // 0x23 alignment unused

    public override int Form { get => ReadUInt16LittleEndian(Data.AsSpan(0x24)); set => WriteUInt16LittleEndian(Data.AsSpan(0x24), (ushort)value); }
    public override int EV_HP { get => Data[0x26]; set => Data[0x26] = (byte)value; }
    public override int EV_ATK { get => Data[0x27]; set => Data[0x27] = (byte)value; }
    public override int EV_DEF { get => Data[0x28]; set => Data[0x28] = (byte)value; }
    public override int EV_SPE { get => Data[0x29]; set => Data[0x29] = (byte)value; }
    public override int EV_SPA { get => Data[0x2A]; set => Data[0x2A] = (byte)value; }
    public override int EV_SPD { get => Data[0x2B]; set => Data[0x2B] = (byte)value; }
    public byte CNT_Cool { get => Data[0x2C]; set => Data[0x2C] = value; }
    public byte CNT_Beauty { get => Data[0x2D]; set => Data[0x2D] = value; }
    public byte CNT_Cute { get => Data[0x2E]; set => Data[0x2E] = value; }
    public byte CNT_Smart { get => Data[0x2F]; set => Data[0x2F] = value; }
    public byte CNT_Tough { get => Data[0x30]; set => Data[0x30] = value; }
    public byte CNT_Sheen { get => Data[0x31]; set => Data[0x31] = value; }
    private byte PKRS { get => Data[0x32]; set => Data[0x32] = value; }
    public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
    public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | (value << 4)); }
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

    public bool RibbonMarkVigor { get => FlagUtil.GetFlag(Data, 0x44, 0); set => FlagUtil.SetFlag(Data, 0x44, 0, value); }
    public bool RibbonMarkSlump { get => FlagUtil.GetFlag(Data, 0x44, 1); set => FlagUtil.SetFlag(Data, 0x44, 1, value); }
    public bool RibbonPioneer { get => FlagUtil.GetFlag(Data, 0x44, 2); set => FlagUtil.SetFlag(Data, 0x44, 2, value); }
    public bool RibbonTwinklingStar { get => FlagUtil.GetFlag(Data, 0x44, 3); set => FlagUtil.SetFlag(Data, 0x44, 3, value); }
    public bool RIB44_4 { get => FlagUtil.GetFlag(Data, 0x44, 4); set => FlagUtil.SetFlag(Data, 0x44, 4, value); }
    public bool RIB44_5 { get => FlagUtil.GetFlag(Data, 0x44, 5); set => FlagUtil.SetFlag(Data, 0x44, 5, value); }
    public bool RIB44_6 { get => FlagUtil.GetFlag(Data, 0x44, 6); set => FlagUtil.SetFlag(Data, 0x44, 6, value); }
    public bool RIB44_7 { get => FlagUtil.GetFlag(Data, 0x44, 7); set => FlagUtil.SetFlag(Data, 0x44, 7, value); }

    public bool RIB45_0 { get => FlagUtil.GetFlag(Data, 0x45, 0); set => FlagUtil.SetFlag(Data, 0x45, 0, value); }
    public bool RIB45_1 { get => FlagUtil.GetFlag(Data, 0x45, 1); set => FlagUtil.SetFlag(Data, 0x45, 1, value); }
    public bool RIB45_2 { get => FlagUtil.GetFlag(Data, 0x45, 2); set => FlagUtil.SetFlag(Data, 0x45, 2, value); }
    public bool RIB45_3 { get => FlagUtil.GetFlag(Data, 0x45, 3); set => FlagUtil.SetFlag(Data, 0x45, 3, value); }
    public bool RIB45_4 { get => FlagUtil.GetFlag(Data, 0x45, 4); set => FlagUtil.SetFlag(Data, 0x45, 4, value); }
    public bool RIB45_5 { get => FlagUtil.GetFlag(Data, 0x45, 5); set => FlagUtil.SetFlag(Data, 0x45, 5, value); }
    public bool RIB45_6 { get => FlagUtil.GetFlag(Data, 0x45, 6); set => FlagUtil.SetFlag(Data, 0x45, 6, value); }
    public bool RIB45_7 { get => FlagUtil.GetFlag(Data, 0x45, 7); set => FlagUtil.SetFlag(Data, 0x45, 7, value); }

    public bool RIB46_0 { get => FlagUtil.GetFlag(Data, 0x41, 0); set => FlagUtil.SetFlag(Data, 0x41, 0, value); }
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

    public bool HasMark()
    {
        var d = Data.AsSpan();
        if ((ReadUInt16LittleEndian(d[0x3A..]) & 0xFFE0) != 0)
            return true;
        if (ReadUInt32LittleEndian(d[0x40..]) != 0)
            return true;
        return (d[0x44] & 3) != 0;
    }

    public uint Sociability { get => ReadUInt32LittleEndian(Data.AsSpan(0x48)); set => WriteUInt32LittleEndian(Data.AsSpan(0x48), value); }

    // 0x4C-0x4F unused

    public byte HeightScalar { get => Data[0x50]; set => Data[0x50] = value; }
    public byte WeightScalar { get => Data[0x51]; set => Data[0x51] = value; }
    public byte HeightScalarCopy { get => Data[0x52]; set => Data[0x52] = value; }

    // 0x53 unused

    public override int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x54)); set => WriteUInt16LittleEndian(Data.AsSpan(0x54), (ushort)value); }
    public override int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x56)); set => WriteUInt16LittleEndian(Data.AsSpan(0x56), (ushort)value); }
    public override int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x58)); set => WriteUInt16LittleEndian(Data.AsSpan(0x58), (ushort)value); }
    public override int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x5A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x5A), (ushort)value); }

    public override int Move1_PP { get => Data[0x5C]; set => Data[0x5C] = (byte)value; }
    public override int Move2_PP { get => Data[0x5D]; set => Data[0x5D] = (byte)value; }
    public override int Move3_PP { get => Data[0x5E]; set => Data[0x5E] = (byte)value; }
    public override int Move4_PP { get => Data[0x5F]; set => Data[0x5F] = (byte)value; }
    #endregion
    #region Block B
    public override string Nickname
    {
        get => StringConverter8.GetString(Nickname_Trash);
        set => StringConverter8.SetString(Nickname_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    // 2 bytes for \0, automatically handled above

    public override int Move1_PPUps { get => Data[0x86]; set => Data[0x86] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x87]; set => Data[0x87] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x88]; set => Data[0x88] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x89]; set => Data[0x89] = (byte)value; }

    public override int RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x8A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x8A), (ushort)value); }
    public override int RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x8C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x8C), (ushort)value); }
    public override int RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x8E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x8E), (ushort)value); }
    public override int RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x90)); set => WriteUInt16LittleEndian(Data.AsSpan(0x90), (ushort)value); }

    public override int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data.AsSpan(0x92)); set => WriteUInt16LittleEndian(Data.AsSpan(0x92), (ushort)value); }
    private uint IV32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x94)); set => WriteUInt32LittleEndian(Data.AsSpan(0x94), value); }
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
    public override string HT_Name
    {
        get => StringConverter8.GetString(HT_Trash);
        set => StringConverter8.SetString(HT_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    public override int HT_Gender { get => Data[0xD2]; set => Data[0xD2] = (byte)value; }
    public byte HT_Language { get => Data[0xD3]; set => Data[0xD3] = value; }
    public override int CurrentHandler { get => Data[0xD4]; set => Data[0xD4] = (byte)value; }
    // 0xD5 unused (alignment)
    public int HT_TrainerID { get => ReadUInt16LittleEndian(Data.AsSpan(0xD6)); set => WriteUInt16LittleEndian(Data.AsSpan(0xD6), (ushort)value); } // unused?
    public override int HT_Friendship { get => Data[0xD8]; set => Data[0xD8] = (byte)value; }
    public byte HT_Intensity { get => Data[0xD9]; set => Data[0xD9] = value; }
    public byte HT_Memory { get => Data[0xDA]; set => Data[0xDA] = value; }
    public byte HT_Feeling { get => Data[0xDB]; set => Data[0xDB] = value; }
    public ushort HT_TextVar { get => ReadUInt16LittleEndian(Data.AsSpan(0xDC)); set => WriteUInt16LittleEndian(Data.AsSpan(0xDC), value); }

    // 0xDE-0xEB unused

    public override byte Fullness { get => Data[0xEC]; set => Data[0xEC] = value; }
    public override byte Enjoyment { get => Data[0xED]; set => Data[0xED] = value; }
    public override int Version { get => Data[0xEE]; set => Data[0xEE] = (byte)value; }
    public byte BattleVersion { get => Data[0xEF]; set => Data[0xEF] = value; }
    // public override int Region { get => Data[0xF0]; set => Data[0xF0] = (byte)value; }
    // public override int ConsoleRegion { get => Data[0xF1]; set => Data[0xF1] = (byte)value; }
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
    public override string OT_Name
    {
        get => StringConverter8.GetString(OT_Trash);
        set => StringConverter8.SetString(OT_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    public override int OT_Friendship { get => Data[0x12A]; set => Data[0x12A] = (byte)value; }
    public byte OT_Intensity { get => Data[0x12B]; set => Data[0x12B] = value; }
    public byte OT_Memory { get => Data[0x12C]; set => Data[0x12C] = value; }
    // 0x12D unused align
    public ushort OT_TextVar { get => ReadUInt16LittleEndian(Data.AsSpan(0x12E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x12E), value); }
    public byte OT_Feeling { get => Data[0x130]; set => Data[0x130] = value; }
    public override int Egg_Year { get => Data[0x131]; set => Data[0x131] = (byte)value; }
    public override int Egg_Month { get => Data[0x132]; set => Data[0x132] = (byte)value; }
    public override int Egg_Day { get => Data[0x133]; set => Data[0x133] = (byte)value; }
    public override int Met_Year { get => Data[0x134]; set => Data[0x134] = (byte)value; }
    public override int Met_Month { get => Data[0x135]; set => Data[0x135] = (byte)value; }
    public override int Met_Day { get => Data[0x136]; set => Data[0x136] = (byte)value; }
    public override int Ball { get => Data[0x137]; set => Data[0x137] = (byte)value; }
    public override int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(0x138)); set => WriteUInt16LittleEndian(Data.AsSpan(0x138), (ushort)value); }
    public override int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(0x13A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x13A), (ushort)value); }
    // 0x13C unused align
    public override int Met_Level { get => Data[0x13D] & ~0x80; set => Data[0x13D] = (byte)((Data[0x13D] & 0x80) | value); }
    public override int OT_Gender { get => Data[0x13D] >> 7; set => Data[0x13D] = (byte)((Data[0x13D] & ~0x80) | (value << 7)); }
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

    public bool GetMoveRecordFlagAny() => Array.FindIndex(Data, 0x13F, 14, static z => z != 0) >= 0;

    // Why did you mis-align this field, GameFreak?
    public ulong Tracker
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(0x14D));
        set => WriteUInt64LittleEndian(Data.AsSpan(0x14D), value);
    }

    public bool GetPurchasedRecordFlag(int index)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, 0x155 + ofs, index & 7);
    }

    public void SetPurchasedRecordFlag(int index, bool value)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, 0x155 + ofs, index & 7, value);
    }

    public bool GetPurchasedRecordFlagAny() => Array.FindIndex(Data, 0x155, 8, static z => z != 0) >= 0;

    public int GetPurchasedCount()
    {
        var value = ReadUInt64LittleEndian(Data.AsSpan(0x155));
        ulong result = 0;
        for (int i = 0; i < 64; i++)
            result += ((value >> i) & 1);
        return (int)result;
    }

    public bool GetMasteredRecordFlag(int index)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, 0x15D + ofs, index & 7);
    }

    public void SetMasteredRecordFlag(int index, bool value)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, 0x15D + ofs, index & 7, value);
    }

    public bool GetMasteredRecordFlagAny() => Array.FindIndex(Data, 0x15D, 8, static z => z != 0) >= 0;

    #endregion
    #region Battle Stats
    public override int Stat_Level { get => Data[0x168]; set => Data[0x168] = (byte)value; }
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
        int nature = StatNature;

        stats[0] = (ushort)(GetGanbaruStat(p.HP, HT_HP ? 31 : IV_HP, GV_HP, level) + GetStatHp(p.HP, level));
        stats[1] = (ushort)(GetGanbaruStat(p.ATK, HT_ATK ? 31 : IV_ATK, GV_ATK, level) + GetStat(p.ATK, level, nature, 0));
        stats[2] = (ushort)(GetGanbaruStat(p.DEF, HT_DEF ? 31 : IV_DEF, GV_DEF, level) + GetStat(p.DEF, level, nature, 1));
        stats[3] = (ushort)(GetGanbaruStat(p.SPE, HT_SPE ? 31 : IV_SPE, GV_SPE, level) + GetStat(p.SPE, level, nature, 4));
        stats[4] = (ushort)(GetGanbaruStat(p.SPA, HT_SPA ? 31 : IV_SPA, GV_SPA, level) + GetStat(p.SPA, level, nature, 2));
        stats[5] = (ushort)(GetGanbaruStat(p.SPD, HT_SPD ? 31 : IV_SPD, GV_SPD, level) + GetStat(p.SPD, level, nature, 3));
    }

    public static int GetGanbaruStat(int baseStat, int iv, byte gv, int level)
    {
        int mul = GanbaruExtensions.GetGanbaruMultiplier(gv, iv);
        double step1 = Math.Abs(Math.Sqrt((float)baseStat)) * mul; // The game does abs after sqrt; should be before. It's fine because baseStat is never negative.
        var result = ((float)step1 + level) / 2.5f;
        return (int)Math.Round(result, MidpointRounding.AwayFromZero);
    }

    public static int GetStatHp(int baseStat, int level)
    {
        return (int)((((level / 100.0f) + 1.0f) * baseStat) + level);
    }

    public static int GetStat(int baseStat, int level, int nature, int statIndex)
    {
        var initial = (int)((((level / 50.0f) + 1.0f) * baseStat) / 1.5f);
        return AmplifyStat(nature, statIndex, initial);
    }

    private static int AmplifyStat(int nature, int index, int initial) => GetNatureAmp(nature, index) switch
    {
        1 => 110 * initial / 100, // 110%
        -1 => 90 * initial / 100, // 90%
        _ => initial,
    };

    private static sbyte GetNatureAmp(int nature, int index)
    {
        if ((uint)nature >= 25)
            return -1;
        return NatureAmpTable[(5 * nature) + index];
    }

    private static readonly sbyte[] NatureAmpTable =
    {
        0, 0, 0, 0, 0, // Hardy
        1,-1, 0, 0, 0, // Lonely
        1, 0, 0, 0,-1, // Brave
        1, 0,-1, 0, 0, // Adamant
        1, 0, 0,-1, 0, // Naughty
       -1, 1, 0, 0, 0, // Bold
        0, 0, 0, 0, 0, // Docile
        0, 1, 0, 0,-1, // Relaxed
        0, 1,-1, 0, 0, // Impish
        0, 1, 0,-1, 0, // Lax
       -1, 0, 0, 0, 1, // Timid
        0,-1, 0, 0, 1, // Hasty
        0, 0, 0, 0, 0, // Serious
        0, 0,-1, 0, 1, // Jolly
        0, 0, 0,-1, 1, // Naive
       -1, 0, 1, 0, 0, // Modest
        0,-1, 1, 0, 0, // Mild
        0, 0, 1, 0,-1, // Quiet
        0, 0, 0, 0, 0, // Bashful
        0, 0, 1,-1, 0, // Rash
       -1, 0, 0, 1, 0, // Calm
        0,-1, 0, 1, 0, // Gentle
        0, 0, 0, 1,-1, // Sassy
        0, 0,-1, 1, 0, // Careful
        0, 0, 0, 0, 0, // Quirky
    };

    public override int MarkingCount => 6;

    public override int GetMarking(int index)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (MarkValue >> (index * 2)) & 3;
    }

    public override void SetMarking(int index, int value)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        var shift = index * 2;
        MarkValue = (MarkValue & ~(0b11 << shift)) | ((value & 3) << shift);
    }

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

    public void Trade(ITrainerInfo tr)
    {
        // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
        if (!TradeOT(tr))
            TradeHT(tr);
    }

    public void FixMemories()
    {
        if (LA)
        {
            OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;
            HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = 0; // future inter-format conversion?
        }

        if (IsEgg) // No memories if is egg.
        {
            HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = 0;
            OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;

            // Clear Handler
            if (!IsTradedEgg)
            {
                HT_Friendship = HT_Gender = HT_Language = 0;
                HT_Trash.Clear();
            }
            return;
        }

        if (IsUntraded)
            HT_Gender = HT_Friendship = HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = HT_Language = 0;

        int gen = Generation;
        if (gen < 6)
            OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;
        // if (gen != 8) // must be transferred via HOME, and must have memories
        //     this.SetTradeMemoryHT8(); // not faking HOME tracker.
    }

    private bool TradeOT(ITrainerInfo tr)
    {
        // Check to see if the OT matches the SAV's OT info.
        if (!(tr.TID == TID && tr.SID == SID && tr.Gender == OT_Gender && tr.OT == OT_Name))
            return false;

        CurrentHandler = 0;
        return true;
    }

    private void TradeHT(ITrainerInfo tr)
    {
        if (HT_Name != tr.OT)
        {
            HT_Friendship = PersonalInfo.BaseFriendship;
            HT_Name = tr.OT;
        }
        CurrentHandler = 1;
        HT_Gender = tr.Gender;
        HT_Language = (byte)tr.Language;
    }

    // Maximums
    public override int MaxMoveID => Legal.MaxMoveID_8a;
    public override int MaxSpeciesID => Legal.MaxSpeciesID_8a;
    public override int MaxAbilityID => Legal.MaxAbilityID_8a;
    public override int MaxItemID => Legal.MaxItemID_8a;
    public override int MaxBallID => Legal.MaxBallID_8a;
    public override int MaxGameID => Legal.MaxGameID_8a;

    // Casts are as per the game code; they may seem redundant but every bit of precision matters?
    // This still doesn't precisely match :( -- just use a tolerance check when updating.
    // If anyone can figure out how to get all precision to match, feel free to update :)
    public float HeightRatio => GetHeightRatio(HeightScalar);
    public float WeightRatio => GetWeightRatio(WeightScalar);

    public float CalcHeightAbsolute => GetHeightAbsolute(PersonalInfo, HeightScalar);
    public float CalcWeightAbsolute => GetWeightAbsolute(PersonalInfo, HeightScalar, WeightScalar);

    public void ResetHeight() => HeightAbsolute = CalcHeightAbsolute;
    public void ResetWeight() => WeightAbsolute = CalcWeightAbsolute;

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    private static float GetHeightRatio(int heightScalar)
    {
        // +/- 20% (down from +/- 40% in LGP/E)
        float result = heightScalar / 255f; // 0x437F0000
        result *= 0.40000004f; // 0x3ECCCCCE
        result += 0.8f; // 0x3F4CCCCD
        return result;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    private static float GetWeightRatio(int weightScalar)
    {
        // +/- 20%
        float result = weightScalar / 255f; // 0x437F0000
        result *= 0.40000004f; // 0x3ECCCCCE
        result += 0.8f; // 0x3F4CCCCD
        return result;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static float GetHeightAbsolute(IPersonalMisc p, int heightScalar)
    {
        float HeightRatio = GetHeightRatio(heightScalar);
        return HeightRatio * p.Height;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public static float GetWeightAbsolute(IPersonalMisc p, int heightScalar, int weightScalar)
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

    public void SetMasteryFlagMove(int move)
    {
        var moves = MoveShopPermitIndexes;
        int flagIndex = moves.IndexOf((ushort)move);
        if (flagIndex == -1)
            return;
        if (MoveShopPermitFlags[flagIndex])
            SetMasteredRecordFlag(flagIndex, true);
    }

    public PK8 ConvertToPK8()
    {
        var pk = ConvertTo<PK8>();
        pk.SanitizeImport();
        return pk;
    }

    public PB8 ConvertToPB8()
    {
        var pk = ConvertTo<PB8>();
        if (pk.Egg_Location == 0)
            pk.Egg_Location = Locations.Default8bNone;
        return pk;
    }

    private T ConvertTo<T>() where T : G8PKM, new()
    {
        var pk = new T
        {
            EncryptionConstant = EncryptionConstant,
            PID = PID,
            Species = Species,
            Form = Form,
            FormArgument = FormArgument,
            Gender = Gender,
            Nature = Nature,
            StatNature = StatNature,

            TID = TID,
            SID = SID,
            EXP = EXP,
            Ability = Ability,
            AbilityNumber = AbilityNumber,
            Language = Language,
            Version = Version,

            IV_HP = IV_HP,
            IV_ATK = IV_ATK,
            IV_DEF = IV_DEF,
            IV_SPE = IV_SPE,
            IV_SPA = IV_SPA,
            IV_SPD = IV_SPD,
            IsEgg = IsEgg,
            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPE = EV_SPE,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,

            OT_Gender = OT_Gender,
            OT_Friendship = OT_Friendship,
            OT_Intensity = OT_Intensity,
            OT_Memory = OT_Memory,
            OT_TextVar = OT_TextVar,
            OT_Feeling = OT_Feeling,
            Egg_Year = Egg_Year,
            Egg_Month = Egg_Month,
            Egg_Day = Egg_Day,
            Met_Year = Met_Year,
            Met_Month = Met_Month,
            Met_Day = Met_Day,
            Ball = Ball,
            Egg_Location = Egg_Location,
            Met_Location = Met_Location,
            Met_Level = Met_Level,
            Tracker = Tracker,

            IsNicknamed = IsNicknamed,
            CurrentHandler = CurrentHandler,
            HT_Gender = HT_Gender,
            HT_Language = HT_Language,
            HT_Friendship = HT_Friendship,
            HT_Intensity = HT_Intensity,
            HT_Memory = HT_Memory,
            HT_Feeling = HT_Feeling,
            HT_TextVar = HT_TextVar,

            FatefulEncounter = FatefulEncounter,
            CNT_Cool = CNT_Cool,
            CNT_Beauty = CNT_Beauty,
            CNT_Cute = CNT_Cute,
            CNT_Smart = CNT_Smart,
            CNT_Tough = CNT_Tough,
            CNT_Sheen = CNT_Sheen,

            RibbonChampionKalos = RibbonChampionKalos,
            RibbonChampionG3 = RibbonChampionG3,
            RibbonChampionSinnoh = RibbonChampionSinnoh,
            RibbonBestFriends = RibbonBestFriends,
            RibbonTraining = RibbonTraining,
            RibbonBattlerSkillful = RibbonBattlerSkillful,
            RibbonBattlerExpert = RibbonBattlerExpert,
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
            HasContestMemoryRibbon = HasContestMemoryRibbon,
            HasBattleMemoryRibbon = HasBattleMemoryRibbon,
            RibbonChampionG6Hoenn = RibbonChampionG6Hoenn,
            RibbonContestStar = RibbonContestStar,
            RibbonMasterCoolness = RibbonMasterCoolness,
            RibbonMasterBeauty = RibbonMasterBeauty,
            RibbonMasterCuteness = RibbonMasterCuteness,
            RibbonMasterCleverness = RibbonMasterCleverness,
            RibbonMasterToughness = RibbonMasterToughness,
            RibbonChampionAlola = RibbonChampionAlola,
            RibbonBattleRoyale = RibbonBattleRoyale,
            RibbonBattleTreeGreat = RibbonBattleTreeGreat,
            RibbonBattleTreeMaster = RibbonBattleTreeMaster,
            RibbonChampionGalar = RibbonChampionGalar,
            RibbonTowerMaster = RibbonTowerMaster,
            RibbonMasterRank = RibbonMasterRank,

            RibbonMarkLunchtime = RibbonMarkLunchtime,
            RibbonMarkSleepyTime = RibbonMarkSleepyTime,
            RibbonMarkDusk = RibbonMarkDusk,
            RibbonMarkDawn = RibbonMarkDawn,
            RibbonMarkCloudy = RibbonMarkCloudy,
            RibbonMarkRainy = RibbonMarkRainy,
            RibbonMarkStormy = RibbonMarkStormy,
            RibbonMarkSnowy = RibbonMarkSnowy,
            RibbonMarkBlizzard = RibbonMarkBlizzard,
            RibbonMarkDry = RibbonMarkDry,
            RibbonMarkSandstorm = RibbonMarkSandstorm,
            RibbonCountMemoryContest = RibbonCountMemoryContest,
            RibbonCountMemoryBattle = RibbonCountMemoryBattle,
            RibbonMarkMisty = RibbonMarkMisty,
            RibbonMarkDestiny = RibbonMarkDestiny,
            RibbonMarkFishing = RibbonMarkFishing,
            RibbonMarkCurry = RibbonMarkCurry,
            RibbonMarkUncommon = RibbonMarkUncommon,
            RibbonMarkRare = RibbonMarkRare,
            RibbonMarkRowdy = RibbonMarkRowdy,
            RibbonMarkAbsentMinded = RibbonMarkAbsentMinded,
            RibbonMarkJittery = RibbonMarkJittery,
            RibbonMarkExcited = RibbonMarkExcited,
            RibbonMarkCharismatic = RibbonMarkCharismatic,
            RibbonMarkCalmness = RibbonMarkCalmness,
            RibbonMarkIntense = RibbonMarkIntense,
            RibbonMarkZonedOut = RibbonMarkZonedOut,
            RibbonMarkJoyful = RibbonMarkJoyful,
            RibbonMarkAngry = RibbonMarkAngry,
            RibbonMarkSmiley = RibbonMarkSmiley,
            RibbonMarkTeary = RibbonMarkTeary,
            RibbonMarkUpbeat = RibbonMarkUpbeat,
            RibbonMarkPeeved = RibbonMarkPeeved,
            RibbonMarkIntellectual = RibbonMarkIntellectual,
            RibbonMarkFerocious = RibbonMarkFerocious,
            RibbonMarkCrafty = RibbonMarkCrafty,
            RibbonMarkScowling = RibbonMarkScowling,
            RibbonMarkKindly = RibbonMarkKindly,
            RibbonMarkFlustered = RibbonMarkFlustered,
            RibbonMarkPumpedUp = RibbonMarkPumpedUp,
            RibbonMarkZeroEnergy = RibbonMarkZeroEnergy,
            RibbonMarkPrideful = RibbonMarkPrideful,
            RibbonMarkUnsure = RibbonMarkUnsure,
            RibbonMarkHumble = RibbonMarkHumble,
            RibbonMarkThorny = RibbonMarkThorny,
            RibbonMarkVigor = RibbonMarkVigor,
            RibbonMarkSlump = RibbonMarkSlump,
            RibbonPioneer = RibbonPioneer,
            RibbonTwinklingStar = RibbonTwinklingStar,

            AffixedRibbon = AffixedRibbon,
            HyperTrainFlags = HyperTrainFlags,

            Sociability = Sociability,
            Fullness = Fullness,
            Enjoyment = Enjoyment,
            BattleVersion = BattleVersion,
            PKRS_Days = PKRS_Days,
            PKRS_Strain = PKRS_Strain,
            HeightScalar = HeightScalar,
            WeightScalar = WeightScalar,
            CanGigantamax = CanGigantamax,
            DynamaxLevel = DynamaxLevel,

            Favorite = Favorite,
            MarkValue = MarkValue,
        };

        Nickname_Trash.CopyTo(pk.Nickname_Trash);
        OT_Trash.CopyTo(pk.OT_Trash);
        HT_Trash.CopyTo(pk.HT_Trash);
        pk.ResetMoves();
        pk.ResetPartyStats();
        pk.RefreshChecksum();

        return pk;
    }

    public void SanitizeImport()
    {
        HeightScalarCopy = HeightScalar;
        ResetHeight();
        ResetWeight();
    }

    public void ResetMoves()
    {
        var learnsets = Legal.LevelUpLA;
        var table = PersonalTable.LA;

        var index = table.GetFormIndex(Species, Form);
        var learn = learnsets[index];
        Span<int> moves = stackalloc int[4];
        learn.SetEncounterMoves(CurrentLevel, moves);
        SetMoves(moves);
        this.SetMaximumPPCurrent(moves);
    }
}
