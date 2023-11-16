using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;
/* GameCube memory card format data, checksum and code to extract files based on Dolphin code, adapted from C++ to C#
 * https://github.com/dolphin-emu/dolphin/
 */

/// <summary>
/// Flags for indicating what data is present in the Memory Card
/// </summary>
public enum GCMemoryCardState
{
    Invalid,
    NoPkmSaveGame,
    SaveGameCOLO,
    SaveGameXD,
    SaveGameRSBOX,
    MultipleSaveGame,
    DuplicateCOLO,
    DuplicateXD,
    DuplicateRSBOX,
}

/// <summary>
/// GameCube save container which may or may not contain Generation 3 <see cref="SaveFile"/> objects.
/// </summary>
public sealed class SAV3GCMemoryCard(byte[] Data)
{
    private const int BLOCK_SIZE = 0x2000;
    private const int MBIT_TO_BLOCKS = 0x10;
    private const int DENTRY_STRLEN = 0x20;
    private const int DENTRY_SIZE = 0x40;
    private const int NumEntries_Directory = BLOCK_SIZE / DENTRY_SIZE;

    public static bool IsMemoryCardSize(long size)
    {
        if ((size & 0x7F8_0000) == 0) // 512KB - 64MB
            return false;
        return (size & (size - 1)) == 0; // size is a power of 2
    }

    public static bool IsMemoryCardSize(ReadOnlySpan<byte> Data)
    {
        if (!IsMemoryCardSize(Data.Length))
            return false; // bad size
        if (ReadUInt64BigEndian(Data) == ulong.MaxValue)
            return false; // uninitialized
        return true;
    }

    // Control blocks
    private const int Header_Block = 0;
    private const int Directory_Block = 1;
    private const int DirectoryBackup_Block = 2;
    private const int BlockAlloc_Block = 3;
    private const int BlockAllocBackup_Block = 4;

    private const int Header = BLOCK_SIZE * Header_Block;
    private const int Directory = BLOCK_SIZE * Directory_Block;
    private const int DirectoryBAK = BLOCK_SIZE * DirectoryBackup_Block;
    private const int BlockAlloc = BLOCK_SIZE * BlockAlloc_Block;
    private const int BlockAllocBAK = BLOCK_SIZE * BlockAllocBackup_Block;

    // Checksums
    private (ushort Checksum, ushort Inverse) GetChecksum(int block, int offset, [ConstantExpected(Min = 0)] int length)
    {
        var ofs = (block * BLOCK_SIZE) + offset;
        var span = Data.AsSpan(ofs, length);
        return GetChecksum(span);
    }

    private static (ushort Checksum, ushort Inverse) GetChecksum(ReadOnlySpan<byte> span)
    {
        ushort csum = 0;
        ushort inv_csum = 0;

        for (int i = 0; i < span.Length; i += 2)
        {
            var value = ReadUInt16BigEndian(span[i..]);
            csum += value;
            inv_csum += (ushort)~value;
        }
        if (csum == 0xffff)
            csum = 0;
        if (inv_csum == 0xffff)
            inv_csum = 0;

        return (csum, inv_csum);
    }

    private MemoryCardChecksumStatus VerifyChecksums()
    {
        MemoryCardChecksumStatus results = 0;

        var (chk, inv) = GetChecksum(Header_Block, 0, 0x1FC);
        if (Header_Checksum != chk || Header_Checksum_Inv != inv)
            results |= MemoryCardChecksumStatus.HeaderBad;

        (chk, inv) = GetChecksum(Directory_Block, 0, 0x1FFC);
        if (Directory_Checksum != chk || Directory_Checksum_Inv != inv)
            results |= MemoryCardChecksumStatus.DirectoryBad;

        (chk, inv) = GetChecksum(DirectoryBackup_Block, 0, 0x1FFC);
        if (DirectoryBAK_Checksum != chk || DirectoryBAK_Checksum_Inv != inv)
            results |= MemoryCardChecksumStatus.DirectoryBackupBad;

        (chk, inv) = GetChecksum(BlockAlloc_Block, 4, 0x1FFC);
        if (BlockAlloc_Checksum != chk || BlockAlloc_Checksum_Inv != inv)
            results |= MemoryCardChecksumStatus.BlockAllocBad;

        (chk, inv) = GetChecksum(BlockAllocBackup_Block, 4, 0x1FFC);
        if ((BlockAllocBAK_Checksum != chk) || BlockAllocBAK_Checksum_Inv != inv)
            results |= MemoryCardChecksumStatus.BlockAllocBackupBad;

        return results;
    }

    // Structure
    private int Header_Size => ReadUInt16BigEndian(Data.AsSpan(Header + 0x0022));
    private ushort Header_Checksum => ReadUInt16BigEndian(Data.AsSpan(Header + 0x01fc));
    private ushort Header_Checksum_Inv => ReadUInt16BigEndian(Data.AsSpan(Header + 0x01fe));

    // Encoding (Windows-1252 or Shift JIS)
    private int Header_Encoding => ReadUInt16BigEndian(Data.AsSpan(Header + 0x0024));
    private bool Header_Japanese => Header_Encoding == 1;
    private Encoding EncodingType => Header_Japanese ? Encoding.GetEncoding(1252) : Encoding.GetEncoding(932);

    private int Directory_UpdateCounter => ReadUInt16BigEndian(Data.AsSpan(Directory + 0x1ffa));
    private int Directory_Checksum => ReadUInt16BigEndian(Data.AsSpan(Directory + 0x1ffc));
    private int Directory_Checksum_Inv => ReadUInt16BigEndian(Data.AsSpan(Directory + 0x1ffe));

    private int DirectoryBAK_UpdateCounter => ReadUInt16BigEndian(Data.AsSpan(DirectoryBAK + 0x1ffa));
    private int DirectoryBAK_Checksum => ReadUInt16BigEndian(Data.AsSpan(DirectoryBAK + 0x1ffc));
    private int DirectoryBAK_Checksum_Inv => ReadUInt16BigEndian(Data.AsSpan(DirectoryBAK + 0x1ffe));

    private int BlockAlloc_Checksum => ReadUInt16BigEndian(Data.AsSpan(BlockAlloc + 0x0000));
    private int BlockAlloc_Checksum_Inv => ReadUInt16BigEndian(Data.AsSpan(BlockAlloc + 0x0002));

    private int BlockAllocBAK_Checksum => ReadUInt16BigEndian(Data.AsSpan(BlockAllocBAK + 0x0000));
    private int BlockAllocBAK_Checksum_Inv => ReadUInt16BigEndian(Data.AsSpan(BlockAllocBAK + 0x0002));

    private int DirectoryBlock_Used;

    private int EntryCOLO = -1;
    private int EntryXD = -1;
    private int EntryRSBOX = -1;
    private int EntrySelected = -1;
    public bool HasCOLO => EntryCOLO >= 0;
    public bool HasXD => EntryXD >= 0;
    public bool HasRSBOX => EntryRSBOX >= 0;
    public int SaveGameCount { get; private set; }

    [Flags]
    public enum MemoryCardChecksumStatus
    {
        None = 0,
        HeaderBad = 1 << 0,
        DirectoryBad = 1 << 1,
        DirectoryBackupBad = 1 << 2,
        BlockAllocBad,
        BlockAllocBackupBad,
    }

    private bool IsCorruptedMemoryCard()
    {
        var csums = VerifyChecksums();

        if ((csums & MemoryCardChecksumStatus.HeaderBad) != 0)
            return true;

        if ((csums & MemoryCardChecksumStatus.DirectoryBad) != 0)
        {
            if ((csums & MemoryCardChecksumStatus.DirectoryBackupBad) != 0) // backup is also wrong
                return true; // Directory checksum and directory backup checksum failed

            // backup is correct, restore
            RestoreBackup();
            csums = VerifyChecksums(); // update checksums
        }

        if ((csums & MemoryCardChecksumStatus.BlockAllocBad) != 0)
        {
            if ((csums & MemoryCardChecksumStatus.BlockAllocBackupBad) != 0) // backup is also wrong
                return true;

            // backup is correct, restore
            RestoreBackup();
        }

        return false;
    }

    private void RestoreBackup()
    {
        Array.Copy(Data, DirectoryBackup_Block*BLOCK_SIZE, Data, Directory_Block*BLOCK_SIZE, BLOCK_SIZE);
        Array.Copy(Data, BlockAllocBackup_Block*BLOCK_SIZE, Data, BlockAlloc_Block*BLOCK_SIZE, BLOCK_SIZE);
    }

    public GCMemoryCardState GetMemoryCardState()
    {
        if (!IsMemoryCardSize(Data))
            return GCMemoryCardState.Invalid; // Invalid size

        // Size in megabits, not megabytes
        int m_sizeMb = Data.Length / BLOCK_SIZE / MBIT_TO_BLOCKS;
        if (m_sizeMb != Header_Size) // Memory card file size does not match the header size
            return GCMemoryCardState.Invalid;

        if (IsCorruptedMemoryCard())
            return GCMemoryCardState.Invalid;

        // Use the most recent directory block
        DirectoryBlock_Used = DirectoryBAK_UpdateCounter > Directory_UpdateCounter
            ? DirectoryBackup_Block
            : Directory_Block;

        // Search for Pokémon saves in the directory
        SaveGameCount = 0;
        for (int i = 0; i < NumEntries_Directory; i++)
        {
            int offset = (DirectoryBlock_Used * BLOCK_SIZE) + (i * DENTRY_SIZE);
            if (ReadUInt32BigEndian(Data.AsSpan(offset)) == uint.MaxValue) // empty entry
                continue;

            int FirstBlock = ReadUInt16BigEndian(Data.AsSpan(offset + 0x36));
            if (FirstBlock < 5)
                continue;

            int BlockCount = ReadUInt16BigEndian(Data.AsSpan(offset + 0x38));
            var dataEnd = (FirstBlock + BlockCount) * BLOCK_SIZE;

            // Memory card directory contains info for deleted files with boundaries beyond memory card size, ignore
            if (dataEnd > Data.Length)
                continue;

            var header = Data.AsSpan(offset, 4);
            var ver = SaveHandlerGCI.GetGameCode(header);
            if (ver == GameVersion.COLO)
            {
                if (HasCOLO) // another entry already exists
                    return GCMemoryCardState.DuplicateCOLO;
                EntryCOLO = i;
                SaveGameCount++;
            }
            else if (ver == GameVersion.XD)
            {
                if (HasXD) // another entry already exists
                    return GCMemoryCardState.DuplicateXD;
                EntryXD = i;
                SaveGameCount++;
            }
            else if (ver == GameVersion.RSBOX)
            {
                if (HasRSBOX) // another entry already exists
                    return GCMemoryCardState.DuplicateRSBOX;
                EntryRSBOX = i;
                SaveGameCount++;
            }
        }
        if (SaveGameCount == 0)
            return GCMemoryCardState.NoPkmSaveGame;

        if (SaveGameCount > 1)
            return GCMemoryCardState.MultipleSaveGame;

        if (HasCOLO)
        {
            EntrySelected = EntryCOLO;
            return GCMemoryCardState.SaveGameCOLO;
        }
        if (HasXD)
        {
            EntrySelected = EntryXD;
            return GCMemoryCardState.SaveGameXD;
        }
        EntrySelected = EntryRSBOX;
        return GCMemoryCardState.SaveGameRSBOX;
    }

    public GameVersion SelectedGameVersion
    {
        get
        {
            if (EntrySelected < 0)
                return GameVersion.Any;

            if (EntrySelected == EntryCOLO)
                return GameVersion.COLO;
            if (EntrySelected == EntryXD)
                return GameVersion.XD;
            if (EntrySelected == EntryRSBOX)
                return GameVersion.RSBOX;
            return GameVersion.Any; //Default for no game selected
        }
    }

    public void SelectSaveGame(GameVersion Game)
    {
        switch (Game)
        {
            case GameVersion.COLO: if (HasCOLO) EntrySelected = EntryCOLO; break;
            case GameVersion.XD: if (HasXD) EntrySelected = EntryXD; break;
            case GameVersion.RSBOX: if (HasRSBOX) EntrySelected = EntryRSBOX; break;
        }
    }

    public string GCISaveName => GCISaveGameName();
    public readonly byte[] Data = Data;

    private string GCISaveGameName()
    {
        int offset = (DirectoryBlock_Used * BLOCK_SIZE) + (EntrySelected * DENTRY_SIZE);
        var span = Data.AsSpan(offset, DENTRY_SIZE);
        return GetSaveName(EncodingType, span);
    }

    private static string GetSaveName(Encoding encoding, ReadOnlySpan<byte> data)
    {
        string GameCode = encoding.GetString(data[..4]);
        string Makercode = encoding.GetString(data.Slice(4, 2));

        Span<char> FileName = stackalloc char[DENTRY_STRLEN];
        encoding.GetString(data.Slice(0x08, DENTRY_STRLEN));
        var zero = FileName.IndexOf('\0');
        if (zero >= 0)
            FileName = FileName[..zero];

        return $"{Makercode}-{GameCode}-{FileName}.gci";
    }

    public ReadOnlyMemory<byte> ReadSaveGameData()
    {
        var entry = EntrySelected;
        if (entry < 0)
            return ReadOnlyMemory<byte>.Empty; // No entry selected
        return ReadSaveGameData(entry);
    }

    private ReadOnlyMemory<byte> ReadSaveGameData(int entry)
    {
        int offset = (DirectoryBlock_Used * BLOCK_SIZE) + (entry * DENTRY_SIZE);
        var span = Data.AsSpan(offset);
        int blockFirst = ReadUInt16BigEndian(span[0x36..]);
        int blockCount = ReadUInt16BigEndian(span[0x38..]);

        return Data.AsMemory(blockFirst * BLOCK_SIZE, blockCount * BLOCK_SIZE);
    }

    public void WriteSaveGameData(ReadOnlySpan<byte> data)
    {
        var entry = EntrySelected;
        if (entry < 0) // Can't write anywhere
            return;
        WriteSaveGameData(data, entry);
    }

    private void WriteSaveGameData(ReadOnlySpan<byte> data, int entry)
    {
        int offset = (DirectoryBlock_Used * BLOCK_SIZE) + (entry * DENTRY_SIZE);
        var span = Data.AsSpan(offset);
        int blockFirst = ReadUInt16BigEndian(span[0x36..]);
        int blockCount = ReadUInt16BigEndian(span[0x38..]);

        if (data.Length != blockCount * BLOCK_SIZE) // Invalid File Size
            return;

        var dest = Data.AsSpan(blockFirst * BLOCK_SIZE);
        data[..(blockCount * BLOCK_SIZE)].CopyTo(dest);
    }
}
