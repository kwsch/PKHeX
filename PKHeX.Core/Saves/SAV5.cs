using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object.
/// </summary>
public abstract class SAV5 : SaveFile, ISaveBlock5BW, IEventFlagProvider37, IBoxDetailName, IBoxDetailWallpaper, IDaycareRandomState<ulong>, IDaycareStorage, IDaycareExperience, IDaycareEggState, IMysteryGiftStorageProvider
{
    protected override PK5 GetPKM(byte[] data) => new(data);
    protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray45(data);

    protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
    public override string Extension => ".sav";

    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_BW;
    protected override int SIZE_STORED => PokeCrypto.SIZE_5STORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_5PARTY;
    public override PK5 BlankPKM => new();
    public override Type PKMType => typeof(PK5);

    public override int BoxCount => 24;
    public override int MaxEV => EffortValues.Max255;
    public override byte Generation => 5;
    public override EntityContext Context => EntityContext.Gen5;
    public override int MaxStringLengthTrainer => 7;
    public override int MaxStringLengthNickname => 10;

    public override ushort MaxMoveID => Legal.MaxMoveID_5;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_5;
    public override int MaxAbilityID => Legal.MaxAbilityID_5;
    public override int MaxBallID => Legal.MaxBallID_5;
    public override GameVersion MaxGameID => Legal.MaxGameID_5; // B2

    protected SAV5([ConstantExpected] int size) : base(size)
    {
        Initialize();
        ClearBoxes();
    }

    protected SAV5(byte[] data) : base(data)
    {
        Initialize();
    }

    private void Initialize()
    {
        Box = 0x400;
        Party = 0x18E00;
    }

    // Blocks & Offsets
    protected override void SetChecksums() => AllBlocks.SetChecksums(Data);
    public override bool ChecksumsValid => AllBlocks.GetChecksumsValid(Data);
    public override string ChecksumInfo => AllBlocks.GetChecksumInfo(Data);

    private int CGearInfoOffset => AllBlocks[32].Offset; // 0x1C000 - Options / Skin Info
    protected abstract int CGearDataOffset { get; } // extdata
    public sealed override bool HasPokeDex => true;

    // Daycare
    public int DaycareSlotCount => 2;
    public Memory<byte> GetDaycareSlot(int slot) => Daycare.GetDaycareSlot(slot);
    public bool IsDaycareOccupied(int slot) => Daycare.IsDaycareOccupied(slot);
    public uint GetDaycareEXP(int slot) => Daycare.GetDaycareEXP(slot);
    public void SetDaycareEXP(int slot, uint value) => Daycare.SetDaycareEXP(slot, value);
    public void SetDaycareOccupied(int slot, bool occupied) => Daycare.SetDaycareOccupied(slot, occupied);
    public bool IsEggAvailable { get => Daycare.IsEggAvailable; set => Daycare.IsEggAvailable = value; }
    ulong IDaycareRandomState<ulong>.Seed { get => Daycare.Seed; set => Daycare.Seed = value; }

    // Storage
    public override int PartyCount
    {
        get => Data[Party + 4];
        protected set => Data[Party + 4] = (byte)value;
    }

    public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30) + (box * 0x10);
    public override int GetPartyOffset(int slot) => Party + 8 + (SIZE_PARTY * slot);

    public override int BoxesUnlocked { get => BoxLayout.BoxesUnlocked; set => BoxLayout.BoxesUnlocked = (byte)value; }
    public int GetBoxWallpaper(int box) => BoxLayout.GetBoxWallpaper(box);
    public void SetBoxWallpaper(int box, int value) => BoxLayout.SetBoxWallpaper(box, value);
    public string GetBoxName(int box) => BoxLayout[box];
    public void SetBoxName(int box, ReadOnlySpan<char> value) => BoxLayout.SetBoxName(box, value);
    public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        var pk5 = (PK5)pk;
        // Apply to this Save File
        pk5.UpdateHandler(this);
        pk5.RefreshChecksum();
    }

    // Player Data
    public override string OT { get => PlayerData.OT; set => PlayerData.OT = value; }
    public override uint ID32 { get => PlayerData.ID32; set => PlayerData.ID32 = value; }
    public override ushort TID16 { get => PlayerData.TID16; set => PlayerData.TID16 = value; }
    public override ushort SID16 { get => PlayerData.SID16; set => PlayerData.SID16 = value; }
    public int Country { get => PlayerData.Country; set => PlayerData.Country = value; }
    public int Region { get => PlayerData.Region; set => PlayerData.Region = value; }
    public override int Language { get => PlayerData.Language; set => PlayerData.Language = value; }
    public override GameVersion Version { get => (GameVersion)PlayerData.Game; set => PlayerData.Game = (byte)value; }
    public override byte Gender { get => PlayerData.Gender; set => PlayerData.Gender = value; }
    public override int PlayedHours { get => PlayerData.PlayedHours; set => PlayerData.PlayedHours = value; }
    public override int PlayedMinutes { get => PlayerData.PlayedMinutes; set => PlayerData.PlayedMinutes = value; }
    public override int PlayedSeconds { get => PlayerData.PlayedSeconds; set => PlayerData.PlayedSeconds = value; }
    public override uint Money { get => Misc.Money; set => Misc.Money = value; }
    public override uint SecondsToStart { get => AdventureInfo.SecondsToStart; set => AdventureInfo.SecondsToStart = value; }
    public override uint SecondsToFame  { get => AdventureInfo.SecondsToFame ; set => AdventureInfo.SecondsToFame  = value; }
    public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

    protected override void SetDex(PKM pk) => Zukan.SetDex(pk);
    public override bool GetCaught(ushort species) => Zukan.GetCaught(species);
    public override bool GetSeen(ushort species) => Zukan.GetSeen(species);

    public sealed override string GetString(ReadOnlySpan<byte> data)
        => StringConverter5.GetString(data);
    public sealed override int LoadString(ReadOnlySpan<byte> data, Span<char> result)
        => StringConverter5.LoadString(data, result);
    public sealed override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter5.SetString(destBuffer, value, maxLength, Language, option);

    // DLC
    private int CGearSkinInfoOffset => CGearInfoOffset + (this is SAV5B2W2 ? 0x10 : 0) + 0x24;

    private bool CGearSkinPresent
    {
        get => Data[CGearSkinInfoOffset + 2] == 1;
        set => Data[CGearSkinInfoOffset + 2] = PlayerData.Data[this is SAV5B2W2 ? 0x6C : 0x54] = value ? (byte)1 : (byte)0;
    }

    private static ReadOnlySpan<byte> DLCFooter => [ 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x27, 0x00, 0x00, 0x27, 0x35, 0x05, 0x31, 0x00, 0x00 ];

    public byte[] CGearSkinData
    {
        get
        {
            if (CGearSkinPresent)
                return Data.AsSpan(CGearDataOffset, CGearBackground.SIZE_CGB).ToArray();
            return new byte[CGearBackground.SIZE_CGB];
        }
        set
        {
            SetData(value, CGearDataOffset);

            ushort chk = Checksums.CRC16_CCITT(value);
            var footer = Data.AsSpan(CGearDataOffset + value.Length);

            WriteUInt16LittleEndian(footer, 1); // block updated once
            WriteUInt16LittleEndian(footer[2..], chk); // checksum
            WriteUInt16LittleEndian(footer[0x100..], chk);  // second checksum

            DLCFooter.CopyTo(footer[0x102..]);

            ushort skinchkval = Checksums.CRC16_CCITT(footer[0x100..0x104]);
            WriteUInt16LittleEndian(footer[0x112..], skinchkval);

            // Indicate in the save file that data is present
            WriteUInt16LittleEndian(Data.AsSpan(0x19438), 0xC21E);

            WriteUInt16LittleEndian(Data.AsSpan(CGearSkinInfoOffset), chk);
            CGearSkinPresent = true;

            State.Edited = true;
        }
    }

    public abstract IReadOnlyList<BlockInfo> AllBlocks { get; }
    public abstract MyItem Items { get; }
    public abstract Zukan5 Zukan { get; }
    public abstract Misc5 Misc { get; }
    public abstract MysteryBlock5 Mystery { get; }
    public abstract Chatter5 Chatter { get; }
    public abstract Daycare5 Daycare { get; }
    public abstract BoxLayout5 BoxLayout { get; }
    public abstract PlayerData5 PlayerData { get; }
    public abstract PlayerPosition5 PlayerPosition { get; }
    public abstract BattleSubway5 BattleSubway { get; }
    public abstract Entralink5 Entralink { get; }
    public abstract Musical5 Musical { get; }
    public abstract Encount5 Encount { get; }
    public abstract UnityTower5 UnityTower { get; }
    public abstract EventWork5 EventWork { get; }
    public abstract BattleBox5 BattleBox { get; }
    public abstract EntreeForest EntreeForest { get; }
    public abstract GlobalLink5 GlobalLink { get; }
    public abstract WhiteBlack5 Forest { get; }
    public abstract GTS5 GTS { get; }
    public abstract AdventureInfo5 AdventureInfo { get; }
    public abstract Record5 Records { get; }
    IEventFlag37 IEventFlagProvider37.EventWork => EventWork;

    public abstract Memory<byte> BattleVideoNative { get; }
    public abstract Memory<byte> BattleVideoDownload1 { get; }
    public abstract Memory<byte> BattleVideoDownload2 { get; }
    public abstract Memory<byte> BattleVideoDownload3 { get; }

    protected override byte[] GetFinalData()
    {
        EntreeForest.EndAccess();
        Mystery.EndAccess();
        return base.GetFinalData();
    }

    public static int GetMailOffset(int index) => (index * Mail5.SIZE) + 0x1DD00;
    public byte[] GetMailData(int offset) => Data.AsSpan(offset, Mail5.SIZE).ToArray();

    public MailDetail GetMail(int mailIndex)
    {
        int ofs = GetMailOffset(mailIndex);
        var data = GetMailData(ofs);
        return new Mail5(data, ofs);
    }

    IMysteryGiftStorage IMysteryGiftStorageProvider.MysteryGiftStorage => Mystery;
}
