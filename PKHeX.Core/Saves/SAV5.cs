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

    protected SAV5(Memory<byte> data) : base(data)
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
    public override uint SecondsToFame  { get => AdventureInfo.SecondsToFame; set => AdventureInfo.SecondsToFame  = value; }
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
    public abstract BattleSubwayPlay5 BattleSubwayPlay { get; }
    public abstract BattleSubway5 BattleSubway { get; }
    public abstract Entralink5 Entralink { get; }
    public abstract Musical5 Musical { get; }
    public abstract Encount5 Encount { get; }
    public abstract UnityTower5 UnityTower { get; }
    public abstract SkinInfo5 SkinInfo { get; }
    public abstract EventWork5 EventWork { get; }
    public abstract BattleBox5 BattleBox { get; }
    public abstract EntreeForest EntreeForest { get; }
    public abstract GlobalLink5 GlobalLink { get; }
    public abstract WhiteBlack5 Forest { get; }
    public abstract GTS5 GTS { get; }
    public abstract AdventureInfo5 AdventureInfo { get; }
    public abstract Record5 Records { get; }
    IEventFlag37 IEventFlagProvider37.EventWork => EventWork;

    protected override Memory<byte> GetFinalData()
    {
        EntreeForest.EndAccess();
        Mystery.EndAccess();
        return base.GetFinalData();
    }

    public static int GetMailOffset(int index) => (index * Mail5.SIZE) + 0x1DD00;
    public byte[] GetMailData(int offset) => Data.Slice(offset, Mail5.SIZE).ToArray();

    public MailDetail GetMail(int mailIndex)
    {
        int ofs = GetMailOffset(mailIndex);
        var data = GetMailData(ofs);
        return new Mail5(data, ofs);
    }

    IMysteryGiftStorage IMysteryGiftStorageProvider.MysteryGiftStorage => Mystery;
    protected abstract int ExtBattleVideoNativeOffset { get; }
    protected abstract int ExtBattleVideoDownload1Offset { get; }
    protected abstract int ExtBattleVideoDownload2Offset { get; }
    protected abstract int ExtBattleVideoDownload3Offset { get; }
    protected abstract int ExtCGearOffset { get; }
    protected abstract int ExtBattleTestOffset { get; }
    protected abstract int ExtMusicalDownloadOffset { get; }
    protected abstract int ExtPokeDexSkinOffset { get; }
    protected abstract int ExtHallOfFame1Offset { get; }
    protected abstract int ExtHallOfFame2Offset { get; }
    protected abstract int ExtLink1Offset { get; }
    protected abstract int ExtLink2Offset { get; }

    /// <summary> Variable sized NARC download depending on the game (B/W vs B2/W2). </summary>
    public abstract int MusicalDownloadSize { get; }
    public const int HallOfFameSize = 0x155C;
    private const int Link3DSDataSize = 0x400;

    public Memory<byte> BattleVideoNative => Buffer.Slice(ExtBattleVideoNativeOffset, BattleVideo5.SIZE);
    public Memory<byte> BattleVideoDownload1 => Buffer.Slice(ExtBattleVideoDownload1Offset, BattleVideo5.SIZE);
    public Memory<byte> BattleVideoDownload2 => Buffer.Slice(ExtBattleVideoDownload2Offset, BattleVideo5.SIZE);
    public Memory<byte> BattleVideoDownload3 => Buffer.Slice(ExtBattleVideoDownload3Offset, BattleVideo5.SIZE);
    public Memory<byte> CGearSkinData => Buffer.Slice(ExtCGearOffset, CGearBackground.SIZE);
    public Memory<byte> BattleTest => Buffer.Slice(ExtBattleTestOffset, BattleTest5.SIZE);
    public Memory<byte> MusicalDownloadData => Buffer.Slice(ExtMusicalDownloadOffset, MusicalDownloadSize);
    public Memory<byte> PokedexSkinData => Buffer.Slice(ExtPokeDexSkinOffset, PokeDexSkin5.SIZE);
    public Memory<byte> HallOfFame1 => Buffer.Slice(ExtHallOfFame1Offset, HallOfFameSize);
    public Memory<byte> HallOfFame2 => Buffer.Slice(ExtHallOfFame2Offset, HallOfFameSize);
    public Memory<byte> Link1Data => Buffer.Slice(ExtLink1Offset, Link3DSDataSize);
    public Memory<byte> Link2Data => Buffer.Slice(ExtLink2Offset, Link3DSDataSize);

    private const int ExtFooterLength = 0x14;

    /// <summary>
    /// Writes an extdata section
    /// </summary>
    /// <param name="data">Section of data to write</param>
    /// <param name="offset">Offset within the save file to write to</param>
    /// <param name="size">Expected size of the data</param>
    /// <param name="count">Update count</param>
    /// <returns>Checksum of the <see cref="data"/></returns>
    protected ushort WriteExtSection(ReadOnlySpan<byte> data, int offset, int size, ushort count)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, size);
        SetData(data, offset);

        // Update Tail Section
        ushort chk = Checksums.CRC16_CCITT(data);
        var tail = Data[(offset + size)..];
        WriteUInt16LittleEndian(tail, count); // block updated counter
        WriteUInt16LittleEndian(tail[2..], chk); // checksum

        // Update Footer
        int lengthInner = size + 0x100 - (size % 0x100); // wasting 0x100 bytes, nice!
        var footer = Data.Slice(offset + lengthInner, ExtFooterLength);
        WriteFooterDLC(footer, chk, lengthInner + ExtFooterLength, count);
        return chk;
    }

    private static void WriteFooterDLC(Span<byte> data, ushort chk, int length, ushort count)
    {
        WriteInt32LittleEndian(data, chk);
        chk = Checksums.CRC16_CCITT(data[..4]); // checksum of a checksum
        WriteFooter51(data[4..], chk, length, count);
    }

    private static void WriteFooter51(Span<byte> data, ushort chk, int length, ushort count)
    {
        WriteInt32LittleEndian(data, count);
        WriteInt32LittleEndian(data[0x04..], length);
        WriteInt32LittleEndian(data[0x08..], 0x31053527); // '5.1 magic prime 822424871

        WriteUInt16LittleEndian(data[0x0C..], 0);
        WriteUInt16LittleEndian(data[0x0E..], chk);
    }

    public void SetBattleVideo(int index, ReadOnlySpan<byte> data, ushort count = 1)
    {
        PlayerData.UpdateExtData(ExtDataSectionNote5.BattleVideo0 + index, count);
        var offset = index switch
        {
            0 => ExtBattleVideoNativeOffset,
            1 => ExtBattleVideoDownload1Offset,
            2 => ExtBattleVideoDownload2Offset,
            3 => ExtBattleVideoDownload3Offset,
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };
        WriteExtSection(data, offset, BattleVideo5.SIZE, count);
    }

    public Memory<byte> GetBattleVideo(int index) => index switch
    {
        0 => BattleVideoNative,
        1 => BattleVideoDownload1,
        2 => BattleVideoDownload2,
        3 => BattleVideoDownload3,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public void SetCGearSkin(ReadOnlySpan<byte> data, ushort count = 1)
    {
        var chk = WriteExtSection(data, ExtCGearOffset, CGearBackground.SIZE, count);

        // Indicate in the save file that data is present
        SkinInfo.CGearSkinChecksum = chk;
        SkinInfo.HasCGearSkin = true;
        PlayerData.UpdateExtData(ExtDataSectionNote5.CGearSkin, count);
    }

    public void SetBattleTest(ReadOnlySpan<byte> data, ushort count = 1)
    {
        WriteExtSection(data, ExtBattleTestOffset, BattleTest5.SIZE, count);
        PlayerData.UpdateExtData(ExtDataSectionNote5.BattleTest, count);
    }

    public void SetMusical(ReadOnlySpan<byte> data, ushort count = 1)
    {
        WriteExtSection(data, ExtMusicalDownloadOffset, MusicalDownloadSize, count);
        PlayerData.UpdateExtData(ExtDataSectionNote5.Musical, count);
        Musical.IsAvailableMusicalDLC = true;
    }

    public void SetPokeDexSkin(ReadOnlySpan<byte> data, ushort count = 1)
    {
        WriteExtSection(data, ExtPokeDexSkinOffset, PokeDexSkin5.SIZE, count);
        PlayerData.UpdateExtData(ExtDataSectionNote5.PokedexSkin, count);
        IsAvailablePokedexSkin = true;
    }

    private Span<byte> DexSkinFooter => Data.Slice(ExtPokeDexSkinOffset + PokeDexSkin5.SIZE - 4, 4);

    public bool IsAvailablePokedexSkin
    {
        get => ReadUInt32LittleEndian(DexSkinFooter) == 1;
        set => WriteUInt32LittleEndian(DexSkinFooter, value ? 1u : 0u);
    }

    public void SetHallOfFame(ReadOnlySpan<byte> data, ushort count = 1)
    {
        WriteExtSection(data, ExtHallOfFame1Offset, HallOfFameSize, count);
        PlayerData.UpdateExtData(ExtDataSectionNote5.HallOfFame, count);
    }

    public void SetLink1Data(ReadOnlySpan<byte> data) => SetData(data, ExtLink1Offset);
    public void SetLink2Data(ReadOnlySpan<byte> data) => SetData(data, ExtLink2Offset);
}
