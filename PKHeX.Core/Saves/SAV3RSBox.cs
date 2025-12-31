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

    public SAV3RSBox(bool japanese = false) : base(SaveUtil.SIZE_G3BOX)
    {
        Japanese = japanese;
        Box = 0;
        Blocks = [];
        ClearBoxes();
    }

    public SAV3RSBox(Memory<byte> data) : base(data)
    {
        Japanese = Data[0] == 0x83; // ポケモンボックス Ｒ＆Ｓ
        Blocks = ReadBlocks(Data);
        InitializeData();
    }

    private readonly Memory<byte> Boxes = new byte[SIZE_RESERVED];
    protected override Span<byte> BoxBuffer => Boxes.Span;

    private void InitializeData()
    {
        // Detect active save
        int[] SaveCounts = Array.ConvertAll(Blocks, block => (int)block.SaveCount);
        SaveCount = SaveCounts.Max();
        int ActiveSAV = SaveCounts.IndexOf(SaveCount) / BLOCK_COUNT;
        var ordered = Blocks
            .Skip(ActiveSAV * BLOCK_COUNT)
            .Take(BLOCK_COUNT)
            .OrderBy(b => b.ID);
        Blocks = [..ordered];

        // Set up PC data buffer beyond end of save file.
        Box = 0;

        UnpackBoxes(Data);
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

    protected override Memory<byte> GetFinalData()
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
        var result = Data.ToArray();
        PackBoxes(result);

        Blocks.SetChecksums(result);
        return result;
    }

    private void PackBoxes(Span<byte> data)
    {
        // Copy Box data back
        const int copySize = BLOCK_SIZE - 0x10;
        foreach (var b in Blocks)
        {
            var src = BoxBuffer.Slice((int)(b.ID * copySize), copySize);
            var dest = data.Slice(b.Offset + 0xC, copySize);
            src.CopyTo(dest);
        }
    }

    private void UnpackBoxes(Span<byte> data)
    {
        // Copy block to the allocated location
        const int copySize = BLOCK_SIZE - 0x10;
        foreach (var b in Blocks)
        {
            var dest = BoxBuffer.Slice((int)(b.ID * copySize), copySize);
            var src = data.Slice(b.Offset + 0xC, copySize);
            src.CopyTo(dest);
        }
    }

    // Configuration
    protected override SAV3RSBox CloneInternal() => new(GetInnerData()) { MemoryCard = MemoryCard };

    public override void CopyChangesFrom(SaveFile sav)
    {
        if (sav is not SAV3RSBox s)
            return;
        s.BoxBuffer.CopyTo(BoxBuffer);
    }

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
    public override int GetBoxOffset(int box) => 8 + (SIZE_STORED * box * 30);
    public override int GetBoxSlotOffset(int box, int slot)
    {
        // Boxes are a 12x5 grid instead of the usual 6x5
        // Without some swizzling, a box is the first 30 slots of the box data.
        // Convert the box/slot back to a 0,59 number
        int row = slot / 6;
        int col = slot % 6;
        if (box % 2 == 1) // right side
            col += 6;
        int boxSlot = (row * 12) + col;
        return GetBoxOffset(box &~1) + (boxSlot * SIZE_STORED);
    }

    public override int CurrentBox
    {
        get => BoxBuffer[4] * 2;
        set => BoxBuffer[4] = (byte)(value / 2);
    }

    private Span<byte> GetBoxNameSpan(int box)
    {
        int offset = 0x1EC38 + (9 * box);
        return BoxBuffer.Slice(offset, 9);
    }

    // Box Wallpaper is directly after the Box Names
    private static int GetBoxWallpaperOffset(int box) => 0x1ED19 + (box / 2);

    public int GetBoxWallpaper(int box) => BoxBuffer[GetBoxWallpaperOffset(box)];
    public void SetBoxWallpaper(int box, int value) => BoxBuffer[GetBoxWallpaperOffset(box)] = (byte)value;

    public const int BoxNamePrefix = 5;
    private const string BoxName1 = "[◖ ] ";
    private const string BoxName2 = "[ ◗] ";

    public string GetBoxName(int box)
    {
        // Tweaked for the 1-30/31-60 box showing
        var boxName = box % 2 == 0 ? BoxName1 : BoxName2;
        box /= 2;

        var span = GetBoxNameSpan(box);
        if (span[0] is 0 or 0xFF)
            boxName += BoxDetailNameExtensions.GetDefaultBoxNameCaps(box);
        else
            boxName += GetString(span);

        return boxName;
    }

    private static bool IsBoxNamePrefix(ReadOnlySpan<char> value)
    {
        if (value.StartsWith(BoxName1))
            return true;
        if (value.StartsWith(BoxName2))
            return true;
        return false;
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        if (IsBoxNamePrefix(value))
            value = value[BoxNamePrefix..];

        box /= 2;
        var span = GetBoxNameSpan(box);
        if (value.SequenceEqual(BoxDetailNameExtensions.GetDefaultBoxNameCaps(box)))
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
