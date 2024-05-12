using System;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for Pokémon Ruby Sapphire Box saves.
/// </summary>
public sealed class SAV3RSBox : SaveFile, IGCSaveFile, IBoxDetailName, IBoxDetailWallpaper
{
    protected internal override string ShortSummary => $"{Version} #{SaveCount:0000}";
    public override string Extension => this.GCExtension();
    public override PersonalTable3 Personal => PersonalTable.RS;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_RS;
    public SAV3GCMemoryCard? MemoryCard { get; init; }
    private readonly bool Japanese;

    public SAV3RSBox(byte[] data, SAV3GCMemoryCard memCard) : this(data) => MemoryCard = memCard;

    public SAV3RSBox(bool japanese = false) : base(SaveUtil.SIZE_G3BOX)
    {
        Japanese = japanese;
        Box = 0;
        Blocks = [];
        ClearBoxes();
    }

    public SAV3RSBox(byte[] data) : base(data)
    {
        Japanese = data[0] == 0x83; // ポケモンボックス Ｒ＆Ｓ
        Blocks = ReadBlocks(data);
        InitializeData();
    }

    private void InitializeData()
    {
        // Detect active save
        int[] SaveCounts = Array.ConvertAll(Blocks, block => (int)block.SaveCount);
        SaveCount = SaveCounts.Max();
        int ActiveSAV = Array.IndexOf(SaveCounts, SaveCount) / BLOCK_COUNT;
        var ordered = Blocks
            .Skip(ActiveSAV * BLOCK_COUNT)
            .Take(BLOCK_COUNT)
            .OrderBy(b => b.ID);
        Blocks = [..ordered];

        // Set up PC data buffer beyond end of save file.
        Box = Data.Length;
        Array.Resize(ref Data, Data.Length + SIZE_RESERVED); // More than enough empty space.

        // Copy block to the allocated location
        const int copySize = BLOCK_SIZE - 0x10;
        foreach (var b in Blocks)
            Array.Copy(Data, b.Offset + 0xC, Data, (int) (Box + (b.ID * copySize)), copySize);
    }

    private static BlockInfoRSBOX[] ReadBlocks(ReadOnlySpan<byte> data)
    {
        var blocks = new BlockInfoRSBOX[2 * BLOCK_COUNT];
        for (int i = 0; i < blocks.Length; i++)
        {
            int offset = BLOCK_SIZE + (i * BLOCK_SIZE);
            blocks[i] = new BlockInfoRSBOX(data, offset);
        }

        return blocks;
    }

    private BlockInfoRSBOX[] Blocks;
    private int SaveCount;
    private const int BLOCK_COUNT = 23;
    private const int BLOCK_SIZE = 0x2000;
    private const int SIZE_RESERVED = BLOCK_COUNT * BLOCK_SIZE; // unpacked box data

    protected override byte[] GetFinalData()
    {
        var newFile = GetInnerData();

        // Return the gci if Memory Card is not being exported
        if (MemoryCard is null)
            return newFile;

        MemoryCard.WriteSaveGameData(newFile);
        return MemoryCard.Data.ToArray();
    }

    private byte[] GetInnerData()
    {
        // Copy Box data back
        const int copySize = BLOCK_SIZE - 0x10;
        foreach (var b in Blocks)
            Array.Copy(Data, (int) (Box + (b.ID * copySize)), Data, b.Offset + 0xC, copySize);

        SetChecksums();

        return Data[..^SIZE_RESERVED];
    }

    // Configuration
    protected override SAV3RSBox CloneInternal() => new(GetInnerData()) { MemoryCard = MemoryCard };

    protected override int SIZE_STORED => PokeCrypto.SIZE_3STORED + 4;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_3PARTY; // unused
    public override PK3 BlankPKM => new();
    public override Type PKMType => typeof(PK3);

    public override ushort MaxMoveID => Legal.MaxMoveID_3;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_3;
    public override int MaxAbilityID => Legal.MaxAbilityID_3;
    public override int MaxItemID => Legal.MaxItemID_3;
    public override int MaxBallID => Legal.MaxBallID_3;
    public override GameVersion MaxGameID => Legal.MaxGameID_3;

    public override int MaxEV => EffortValues.Max255;
    public override byte Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;
    public override int MaxStringLengthTrainer => 7;
    public override int MaxStringLengthNickname => 10;
    public override int MaxMoney => 999999;

    public override int BoxCount => 50;
    public override bool HasParty => false;
    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGBA(data);

    // Checksums
    protected override void SetChecksums() => Blocks.SetChecksums(Data);
    public override bool ChecksumsValid => Blocks.GetChecksumsValid(Data);
    public override string ChecksumInfo => Blocks.GetChecksumInfo(Data);

    // Trainer Info
    public override GameVersion Version { get => GameVersion.RSBOX; set { } }

    // Storage
    public override int GetPartyOffset(int slot) => -1;
    public override int GetBoxOffset(int box) => Box + 8 + (SIZE_STORED * box * 30);

    public override int CurrentBox
    {
        get => Data[Box + 4] * 2;
        set => Data[Box + 4] = (byte)(value / 2);
    }

    private Span<byte> GetBoxNameSpan(int box)
    {
        int offset = Box + 0x1EC38 + (9 * box);
        return Data.AsSpan(offset, 9);
    }

    private int GetBoxWallpaperOffset(int box)
    {
        // Box Wallpaper is directly after the Box Names
        return Box + 0x1ED19 + (box / 2);
    }

    public int GetBoxWallpaper(int box) => Data[GetBoxWallpaperOffset(box)];
    public void SetBoxWallpaper(int box, int value) => Data[GetBoxWallpaperOffset(box)] = (byte)value;

    public string GetBoxName(int box)
    {
        // Tweaked for the 1-30/31-60 box showing
        int lo = (30 *(box%2)) + 1;
        int hi = 30*((box % 2) + 1);
        string boxName = $"[{lo:00}-{hi:00}] ";
        box /= 2;

        var span = GetBoxNameSpan(box);
        if (span[0] is 0 or 0xFF)
            boxName += BoxDetailNameExtensions.GetDefaultBoxNameCaps(box);
        else
            boxName += GetString(span);

        return boxName;
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        var span = GetBoxNameSpan(box);
        if (value == BoxDetailNameExtensions.GetDefaultBoxNameCaps(box))
        {
            span.Clear();
            return;
        }
        SetString(span, value, 8, StringConverterOption.ClearZero);
    }

    protected override PK3 GetPKM(byte[] data)
    {
        if (data.Length != PokeCrypto.SIZE_3STORED)
            Array.Resize(ref data, PokeCrypto.SIZE_3STORED);
        return new(data);
    }

    protected override byte[] DecryptPKM(byte[] data)
    {
        if (data.Length != PokeCrypto.SIZE_3STORED)
            Array.Resize(ref data, PokeCrypto.SIZE_3STORED);
        return PokeCrypto.DecryptArray3(data);
    }

    protected override void SetDex(PKM pk) { /* No Pokédex for this game, do nothing */ }

    public override void WriteBoxSlot(PKM pk, Span<byte> data)
    {
        base.WriteBoxSlot(pk, data);
        WriteUInt16LittleEndian(data[(PokeCrypto.SIZE_3STORED)..], pk.TID16);
        WriteUInt16LittleEndian(data[(PokeCrypto.SIZE_3STORED + 2)..], pk.SID16);
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter3.GetString(data, Japanese);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter3.LoadString(data, destBuffer, Japanese);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter3.SetString(destBuffer, value, maxLength, Japanese, option);
}
