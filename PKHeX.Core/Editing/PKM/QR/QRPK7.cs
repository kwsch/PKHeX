using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 PGL QR Code encoded <see cref="PKM"/> entities.
/// </summary>
public sealed class QRPK7(Memory<byte> Raw) : IEncounterInfo
{
    public const int SIZE = 0x30;
    private Span<byte> Data => Raw.Span;

    public bool IsEgg => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public byte Generation => Version.GetGeneration();
    public EntityContext Context => EntityContext.Gen7;
    public bool IsShiny => false;
    public ushort Location => 0;
    public ushort EggLocation => 0;
    public AbilityPermission Ability => (AbilityPermission)AbilityIndex;
    public Ball FixedBall => (Ball)Ball;
    public Shiny Shiny => Shiny.Never;

    public uint EncryptionConstant => ReadUInt32LittleEndian(Data[..]);
    public byte HyperTrainFlags => Data[4];
    public byte Unk_5 => Data[5];
    public byte Unk_6 => Data[6];
    public byte Unk_7 => Data[7];
    public byte Move1_PPUps => Data[8];
    public byte Move2_PPUps => Data[9];
    public byte Move3_PPUps => Data[0xA];
    public byte Move4_PPUps => Data[0xB];
    public uint IV32 { get => ReadUInt32LittleEndian(Data[0xC..]); set => WriteUInt32LittleEndian(Data[0xC..], value); }
    public int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | (uint)((value > 31 ? 31 : value) << 00); }
    public int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | (uint)((value > 31 ? 31 : value) << 05); }
    public int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | (uint)((value > 31 ? 31 : value) << 10); }
    public int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | (uint)((value > 31 ? 31 : value) << 15); }
    public int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | (uint)((value > 31 ? 31 : value) << 20); }
    public int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | (uint)((value > 31 ? 31 : value) << 25); }
    public uint PID => ReadUInt32LittleEndian(Data[0x10..]);
    public ushort Species => ReadUInt16LittleEndian(Data[0x14..]);
    public ushort HeldItem => ReadUInt16LittleEndian(Data[0x16..]);
    public ushort Move1 => ReadUInt16LittleEndian(Data[0x18..]);
    public ushort Move2 => ReadUInt16LittleEndian(Data[0x1A..]);
    public ushort Move3 => ReadUInt16LittleEndian(Data[0x1C..]);
    public ushort Move4 => ReadUInt16LittleEndian(Data[0x1E..]);
    public byte Unk_20 => Data[0x20];
    public byte AbilityIndex => Data[0x21];
    public Nature Nature => (Nature)Data[0x22];
    public bool FatefulEncounter => (Data[0x23] & 1) == 1;
    public byte Gender => (byte)((Data[0x23] >> 1) & 3);
    public byte Form => (byte)(Data[0x23] >> 3);
    public byte EV_HP => Data[0x24];
    public byte EV_ATK => Data[0x25];
    public byte EV_DEF => Data[0x26];
    public byte EV_SPE => Data[0x27];
    public byte EV_SPA => Data[0x28];
    public byte EV_SPD => Data[0x29];
    public byte Unk_2A => Data[0x2A];
    public byte Friendship => Data[0x2B];
    public byte Ball => Data[0x2C];
    public byte Level => Data[0x2D];
    public GameVersion Version => (GameVersion)Data[0x2E];
    public byte Language => Data[0x2F];

    /// <summary>
    /// Converts the <see cref="Data"/> to a rough PKM.
    /// </summary>
    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    /// <summary>
    /// Converts the <see cref="Data"/> to a rough PKM.
    /// </summary>
    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var pk = new PK7
        {
            EncryptionConstant = EncryptionConstant,
            PID = PID,
            Language = Language,
            Species = Species,
            Gender = Gender,
            Nature = Nature,
            FatefulEncounter = FatefulEncounter,
            Form = Form,
            HyperTrainFlags = HyperTrainFlags,
            IV_HP = IV_HP,
            IV_ATK = IV_ATK,
            IV_DEF = IV_DEF,
            IV_SPA = IV_SPA,
            IV_SPD = IV_SPD,
            IV_SPE = IV_SPE,
            EV_HP = EV_HP,
            EV_ATK = EV_ATK,
            EV_DEF = EV_DEF,
            EV_SPA = EV_SPA,
            EV_SPD = EV_SPD,
            EV_SPE = EV_SPE,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,
            HeldItem = HeldItem,
            HandlingTrainerFriendship = Friendship,
            OriginalTrainerFriendship = Friendship,
            Ball = Ball,
            Version = Version,

            OriginalTrainerName = tr.OT,
            HandlingTrainerName = tr.OT,
            CurrentLevel = Level,
            MetLevel = Level,
            MetDate = EncounterDate.GetDate3DS(),
        };
        RecentTrainerCache.SetConsoleRegionData3DS(pk, tr);

        pk.RefreshAbility(AbilityIndex >> 1);
        pk.ForcePartyData();

        pk.RefreshChecksum();
        return pk;
    }
}
