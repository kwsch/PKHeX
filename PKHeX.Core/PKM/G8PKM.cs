using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 8 <see cref="PKM"/> format. </summary>
public abstract class G8PKM : PKM, ISanityChecksum, IMoveReset,
    IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetMemory6, IRibbonSetCommon7, IRibbonSetCommon8, IRibbonSetMark8, IRibbonSetAffixed, ITechRecord8, ISociability,
    IContestStats, IContestStatsMutable, IHyperTrain, IScaledSize, IGigantamax, IFavorite, IDynamaxLevel, IRibbonIndex, IHandlerLanguage, IFormArgument, IHomeTrack, IBattleVersion, ITrainerMemories
{
    protected G8PKM() : base(PokeCrypto.SIZE_8PARTY) { }
    protected G8PKM(byte[] data) : base(DecryptParty(data)) { }

    public abstract void ResetMoves();

    private static byte[] DecryptParty(byte[] data)
    {
        PokeCrypto.DecryptIfEncrypted8(ref data);
        Array.Resize(ref data, PokeCrypto.SIZE_8PARTY);
        return data;
    }

    private ushort CalculateChecksum()
    {
        ushort chk = 0;
        for (int i = 8; i < PokeCrypto.SIZE_8STORED; i += 2)
            chk += ReadUInt16LittleEndian(Data.AsSpan(i));
        return chk;
    }

    // Simple Generated Attributes
    public ReadOnlySpan<bool> TechRecordPermitFlags => PersonalInfo.TMHM.AsSpan(PersonalInfo8SWSH.CountTM);
    public ReadOnlySpan<ushort> TechRecordPermitIndexes => LearnSource8SWSH.TR_SWSH.AsSpan();
    public override int CurrentFriendship
    {
        get => CurrentHandler == 0 ? OT_Friendship : HT_Friendship;
        set { if (CurrentHandler == 0) OT_Friendship = value; else HT_Friendship = value; }
    }

    public override int SIZE_PARTY => PokeCrypto.SIZE_8PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_8STORED;

    public sealed override bool ChecksumValid => CalculateChecksum() == Checksum;
    public sealed override void RefreshChecksum() => Checksum = CalculateChecksum();
    public sealed override bool Valid { get => Sanity == 0 && ChecksumValid; set { if (!value) return; Sanity = 0; RefreshChecksum(); } }

    // Trash Bytes
    public override Span<byte> Nickname_Trash => Data.AsSpan(0x58, 26);
    public override Span<byte> HT_Trash => Data.AsSpan(0xA8, 26);
    public override Span<byte> OT_Trash => Data.AsSpan(0xF8, 26);

    // Maximums
    public override int MaxIV => 31;
    public override int MaxEV => 252;
    public override int OTLength => 12;
    public override int NickLength => 12;

    public override int PSV => (int)(((PID >> 16) ^ (PID & 0xFFFF)) >> 4);
    public override int TSV => (TID ^ SID) >> 4;
    public override bool IsUntraded => Data[0xA8] == 0 && Data[0xA8 + 1] == 0 && Format == Generation; // immediately terminated HT_Name data (\0)

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
        return PokeCrypto.EncryptArray8(Data);
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
    public override int TID { get => ReadUInt16LittleEndian(Data.AsSpan(0x0C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0C), (ushort)value); }
    public override int SID { get => ReadUInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), (ushort)value); }
    public override uint EXP { get => ReadUInt32LittleEndian(Data.AsSpan(0x10)); set => WriteUInt32LittleEndian(Data.AsSpan(0x10), value); }
    public override int Ability { get => ReadUInt16LittleEndian(Data.AsSpan(0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14), (ushort)value); }
    public override int AbilityNumber { get => Data[0x16] & 7; set => Data[0x16] = (byte)((Data[0x16] & ~7) | (value & 7)); }
    public bool Favorite { get => (Data[0x16] & 8) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~8) | ((value ? 1 : 0) << 3)); } // unused, was in LGPE but not in SWSH
    public bool CanGigantamax { get => (Data[0x16] & 16) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~16) | (value ? 16 : 0)); }
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

    public override byte Form { get => Data[0x24]; set => WriteUInt16LittleEndian(Data.AsSpan(0x24), value); }
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
    public byte RibbonCountMemoryBattle  { get => Data[0x3D]; set => HasBattleMemoryRibbon =  (Data[0x3D] = value) != 0; }
    // 0x3E padding
    // 0x3F padding

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

    // 0x52-0x57 unused

    #endregion
    #region Block B
    public override string Nickname
    {
        get => StringConverter8.GetString(Nickname_Trash);
        set => StringConverter8.SetString(Nickname_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    // 2 bytes for \0, automatically handled above

    public override ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x72)); set => WriteUInt16LittleEndian(Data.AsSpan(0x72), value); }
    public override ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x74)); set => WriteUInt16LittleEndian(Data.AsSpan(0x74), value); }
    public override ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x76)); set => WriteUInt16LittleEndian(Data.AsSpan(0x76), value); }
    public override ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x78)); set => WriteUInt16LittleEndian(Data.AsSpan(0x78), value); }

    public override int Move1_PP { get => Data[0x7A]; set => Data[0x7A] = (byte)value; }
    public override int Move2_PP { get => Data[0x7B]; set => Data[0x7B] = (byte)value; }
    public override int Move3_PP { get => Data[0x7C]; set => Data[0x7C] = (byte)value; }
    public override int Move4_PP { get => Data[0x7D]; set => Data[0x7D] = (byte)value; }
    public override int Move1_PPUps { get => Data[0x7E]; set => Data[0x7E] = (byte)value; }
    public override int Move2_PPUps { get => Data[0x7F]; set => Data[0x7F] = (byte)value; }
    public override int Move3_PPUps { get => Data[0x80]; set => Data[0x80] = (byte)value; }
    public override int Move4_PPUps { get => Data[0x81]; set => Data[0x81] = (byte)value; }

    public override ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x82)); set => WriteUInt16LittleEndian(Data.AsSpan(0x82), value); }
    public override ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x84)); set => WriteUInt16LittleEndian(Data.AsSpan(0x84), value); }
    public override ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x86)); set => WriteUInt16LittleEndian(Data.AsSpan(0x86), value); }
    public override ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x88)); set => WriteUInt16LittleEndian(Data.AsSpan(0x88), value); }

    public override int Stat_HPCurrent { get => ReadUInt16LittleEndian(Data.AsSpan(0x8A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x8A), (ushort)value); }

    private uint IV32 { get => ReadUInt32LittleEndian(Data.AsSpan(0x8C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x8C), value); }
    public override int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
    public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
    public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
    public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
    public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
    public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }
    public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0u); }
    public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFFu) | (value ? 0x80000000u : 0u); }

    public byte DynamaxLevel { get => Data[0x90]; set => Data[0x90] = value; }

    // 0x91-0x93 unused

    public override int Status_Condition { get => ReadInt32LittleEndian(Data.AsSpan(0x94)); set => WriteInt32LittleEndian(Data.AsSpan(0x94), value); }
    public int Palma { get => ReadInt32LittleEndian(Data.AsSpan(0x98)); set => WriteInt32LittleEndian(Data.AsSpan(0x98), value); }

    // 0x9C-0xA7 unused

    #endregion
    #region Block C
    public override string HT_Name
    {
        get => StringConverter8.GetString(HT_Trash);
        set => StringConverter8.SetString(HT_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    public override int HT_Gender { get => Data[0xC2]; set => Data[0xC2] = (byte)value; }
    public byte HT_Language { get => Data[0xC3]; set => Data[0xC3] = value; }
    public override int CurrentHandler { get => Data[0xC4]; set => Data[0xC4] = (byte)value; }
    // 0xC5 unused (alignment)
    public int HT_TrainerID { get => ReadUInt16LittleEndian(Data.AsSpan(0xC6)); set => WriteUInt16LittleEndian(Data.AsSpan(0xC6), (ushort)value); } // unused?
    public override int HT_Friendship { get => Data[0xC8]; set => Data[0xC8] = (byte)value; }
    public byte HT_Intensity { get => Data[0xC9]; set => Data[0xC9] = value; }
    public byte HT_Memory { get => Data[0xCA]; set => Data[0xCA] = value; }
    public byte HT_Feeling { get => Data[0xCB]; set => Data[0xCB] = value; }
    public ushort HT_TextVar { get => ReadUInt16LittleEndian(Data.AsSpan(0xCC)); set => WriteUInt16LittleEndian(Data.AsSpan(0xCC), value); }

    public bool GetPokeJobFlag(int index)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, 0xCE + ofs, index & 7);
    }

    public void SetPokeJobFlag(int index, bool value)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, 0xCE + ofs, index & 7, value);
    }

    public bool GetPokeJobFlagAny() => Array.FindIndex(Data, 0xCE, 14, static z => z != 0) >= 0;

    public void ClearPokeJobFlags() => Data.AsSpan(0xCE, 14).Clear();

    public override byte Fullness { get => Data[0xDC]; set => Data[0xDC] = value; }
    public override byte Enjoyment { get => Data[0xDD]; set => Data[0xDD] = value; }
    public override int Version { get => Data[0xDE]; set => Data[0xDE] = (byte)value; }
    public byte BattleVersion { get => Data[0xDF]; set => Data[0xDF] = value; }
    // public override int Region { get => Data[0xE0]; set => Data[0xE0] = (byte)value; }
    // public override int ConsoleRegion { get => Data[0xE1]; set => Data[0xE1] = (byte)value; }
    public override int Language { get => Data[0xE2]; set => Data[0xE2] = (byte)value; }
    // 0xE3 alignment
    public uint FormArgument { get => ReadUInt32LittleEndian(Data.AsSpan(0xE4)); set => WriteUInt32LittleEndian(Data.AsSpan(0xE4), value); }
    public byte FormArgumentRemain { get => (byte)FormArgument; set => FormArgument = (FormArgument & ~0xFFu) | value; }
    public byte FormArgumentElapsed { get => (byte)(FormArgument >> 8); set => FormArgument = (FormArgument & ~0xFF00u) | (uint)(value << 8); }
    public byte FormArgumentMaximum { get => (byte)(FormArgument >> 16); set => FormArgument = (FormArgument & ~0xFF0000u) | (uint)(value << 16); }
    public sbyte AffixedRibbon { get => (sbyte)Data[0xE8]; set => Data[0xE8] = (byte)value; } // selected ribbon
    // remainder unused

    #endregion
    #region Block D
    public override string OT_Name
    {
        get => StringConverter8.GetString(OT_Trash);
        set => StringConverter8.SetString(OT_Trash, value.AsSpan(), 12, StringConverterOption.None);
    }

    public override int OT_Friendship { get => Data[0x112]; set => Data[0x112] = (byte)value; }
    public byte OT_Intensity { get => Data[0x113]; set => Data[0x113] = value; }
    public byte OT_Memory { get => Data[0x114]; set => Data[0x114] = value; }
    // 0x115 unused align
    public ushort OT_TextVar { get => ReadUInt16LittleEndian(Data.AsSpan(0x116)); set => WriteUInt16LittleEndian(Data.AsSpan(0x116), value); }
    public byte OT_Feeling { get => Data[0x118]; set => Data[0x118] = value; }
    public override int Egg_Year { get => Data[0x119]; set => Data[0x119] = (byte)value; }
    public override int Egg_Month { get => Data[0x11A]; set => Data[0x11A] = (byte)value; }
    public override int Egg_Day { get => Data[0x11B]; set => Data[0x11B] = (byte)value; }
    public override int Met_Year { get => Data[0x11C]; set => Data[0x11C] = (byte)value; }
    public override int Met_Month { get => Data[0x11D]; set => Data[0x11D] = (byte)value; }
    public override int Met_Day { get => Data[0x11E]; set => Data[0x11E] = (byte)value; }
    // 0x11F unused align
    public override int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(0x120)); set => WriteUInt16LittleEndian(Data.AsSpan(0x120), (ushort)value); }
    public override int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(0x122)); set => WriteUInt16LittleEndian(Data.AsSpan(0x122), (ushort)value); }
    public override int Ball { get => Data[0x124]; set => Data[0x124] = (byte)value; }
    public override int Met_Level { get => Data[0x125] & ~0x80; set => Data[0x125] = (byte)((Data[0x125] & 0x80) | value); }
    public override int OT_Gender { get => Data[0x125] >> 7; set => Data[0x125] = (byte)((Data[0x125] & ~0x80) | (value << 7)); }
    public byte HyperTrainFlags { get => Data[0x126]; set => Data[0x126] = value; }
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
        return FlagUtil.GetFlag(Data, 0x127 + ofs, index & 7);
    }

    public void SetMoveRecordFlag(int index, bool value = true)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, 0x127 + ofs, index & 7, value);
    }

    public bool GetMoveRecordFlagAny() => Array.FindIndex(Data, 0x127, 14, static z => z != 0) >= 0;

    public void ClearMoveRecordFlags() => Data.AsSpan(0x127, 14).Clear();

    // Why did you mis-align this field, GameFreak?
    public ulong Tracker
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(0x135));
        set => WriteUInt64LittleEndian(Data.AsSpan(0x135), value);
    }

    #endregion
    #region Battle Stats
    public override int Stat_Level { get => Data[0x148]; set => Data[0x148] = (byte)value; }
    // 0x149 unused alignment
    public override int Stat_HPMax { get => ReadUInt16LittleEndian(Data.AsSpan(0x14A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14A), (ushort)value); }
    public override int Stat_ATK   { get => ReadUInt16LittleEndian(Data.AsSpan(0x14C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14C), (ushort)value); }
    public override int Stat_DEF   { get => ReadUInt16LittleEndian(Data.AsSpan(0x14E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14E), (ushort)value); }
    public override int Stat_SPE   { get => ReadUInt16LittleEndian(Data.AsSpan(0x150)); set => WriteUInt16LittleEndian(Data.AsSpan(0x150), (ushort)value); }
    public override int Stat_SPA   { get => ReadUInt16LittleEndian(Data.AsSpan(0x152)); set => WriteUInt16LittleEndian(Data.AsSpan(0x152), (ushort)value); }
    public override int Stat_SPD   { get => ReadUInt16LittleEndian(Data.AsSpan(0x154)); set => WriteUInt16LittleEndian(Data.AsSpan(0x154), (ushort)value); }
    #endregion

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

    protected T ConvertTo<T>() where T : G8PKM, new()
    {
        var pk = new T();
        Data.AsSpan().CopyTo(pk.Data);
        pk.Move1_PPUps = pk.Move2_PPUps = pk.Move3_PPUps = pk.Move4_PPUps = 0;
        pk.RelearnMove1 = pk.RelearnMove2 = pk.RelearnMove3 = pk.RelearnMove4 = 0;
        pk.ClearMoveRecordFlags();
        pk.ClearPokeJobFlags();

        pk.CanGigantamax = false;
        pk.DynamaxLevel = 0;
        pk.Sociability = 0;
        pk.Fullness = 0;
        pk.Data[0x52] = 0;

        pk.ResetMoves();
        pk.ResetPartyStats();
        pk.RefreshChecksum();

        return pk;
    }

    public virtual PA8 ConvertToPA8()
    {
        var pk = new PA8
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

            BattleVersion = BattleVersion,
            PKRS_Days = PKRS_Days,
            PKRS_Strain = PKRS_Strain,
            HeightScalar = HeightScalar,
            WeightScalar = WeightScalar,

            Favorite = Favorite,
            MarkValue = MarkValue,
        };

        Nickname_Trash.CopyTo(pk.Nickname_Trash);
        OT_Trash.CopyTo(pk.OT_Trash);
        HT_Trash.CopyTo(pk.HT_Trash);

        pk.SanitizeImport();

        pk.ResetMoves();
        pk.ResetPartyStats();
        pk.RefreshChecksum();

        return pk;
    }
}
