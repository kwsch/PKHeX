using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// GameCube save container which may or may not contain Generation 3 <see cref="SaveFile"/> objects.
/// </summary>
/// <remarks>
/// GameCube memory card format data, checksum and code to extract files based on Dolphin code, adapted from C++ to C#
/// https://github.com/dolphin-emu/dolphin/
/// </remarks>
public sealed class SAV3GCMemoryCard(Memory<byte> Raw)
{
    public const int BLOCK_SIZE = 0x2000;
    private const int MBIT_TO_BLOCKS = 0x10;
    private const int DENTRY_SIZE = 0x40;
    private const int NumEntries_Directory = (BLOCK_SIZE / DENTRY_SIZE) - 1; // 127

    public Span<byte> Data => Raw.Span;

    public static bool IsMemoryCardSize(long size)
    {
        if ((size & 0x7F8_0000) == 0) // 512KB - 64MB
            return false;
        return (size & (size - 1)) == 0; // size is a power of 2
    }

    public static bool IsMemoryCardSize(ReadOnlySpan<byte> data)
    {
        if (!IsMemoryCardSize(data.Length))
            return false; // bad size
        if (ReadUInt64BigEndian(data) == ulong.MaxValue)
            return false; // uninitialized

        // Size in megabits, not megabytes
        int m_sizeMb = data.Length / BLOCK_SIZE / MBIT_TO_BLOCKS;
        if (m_sizeMb != ReadUInt16BigEndian(data[0x22..])) // Memory card file size does not match the header size
            return false;
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

    private Span<byte> SpanHeader => Data.Slice(Header, BLOCK_SIZE);
    private Span<byte> SpanDirectory => Data.Slice(Directory, BLOCK_SIZE);
    private Span<byte> SpanDirectoryBAK => Data.Slice(DirectoryBAK, BLOCK_SIZE);
    private Span<byte> SpanDirectoryActive => Data.Slice(DirectoryBlock_Used * BLOCK_SIZE, BLOCK_SIZE);
    private Span<byte> SpanBlockAlloc => Data.Slice(BlockAlloc, BLOCK_SIZE);
    private Span<byte> SpanBlockAllocBAK => Data.Slice(BlockAllocBAK, BLOCK_SIZE);

    // Checksums
    private (ushort Checksum, ushort Inverse) GetChecksum(int block, int offset, [ConstantExpected(Min = 0)] int length)
    {
        var ofs = (block * BLOCK_SIZE) + offset;
        var span = Data.Slice(ofs, length);
        return GetChecksum(span);
    }

    private static (ushort Checksum, ushort Inverse) GetChecksum(ReadOnlySpan<byte> span)
    {
        ushort csum = 0;
        ushort inv_csum = 0;

        var cast = MemoryMarshal.Cast<byte, ushort>(span);
        foreach (var u16 in cast)
        {
            var value = BitConverter.IsLittleEndian ? ReverseEndianness(u16) : u16;
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
    private int Header_Size => ReadUInt16BigEndian(SpanHeader[0x0022..]);
    private int Header_Encoding => ReadUInt16BigEndian(SpanHeader[0x0024..]);
    private ushort Header_Checksum => ReadUInt16BigEndian(SpanHeader[0x01fc..]);
    private ushort Header_Checksum_Inv => ReadUInt16BigEndian(SpanHeader[0x01fe..]);

    // Encoding (Windows-1252 or Shift JIS)
    private bool Header_Japanese => Header_Encoding == 1;
    public Encoding EncodingType => Header_Japanese ? Encoding.GetEncoding(1252) : Encoding.GetEncoding(932);

    private int Directory_UpdateCounter => ReadUInt16BigEndian(SpanDirectory[0x1ffa..]);
    private int Directory_Checksum => ReadUInt16BigEndian(SpanDirectory[0x1ffc..]);
    private int Directory_Checksum_Inv => ReadUInt16BigEndian(SpanDirectory[0x1ffe..]);

    private int DirectoryBAK_UpdateCounter => ReadUInt16BigEndian(SpanDirectoryBAK[0x1ffa..]);
    private int DirectoryBAK_Checksum => ReadUInt16BigEndian(SpanDirectoryBAK[0x1ffc..]);
    private int DirectoryBAK_Checksum_Inv => ReadUInt16BigEndian(SpanDirectoryBAK[0x1ffe..]);

    private int BlockAlloc_Checksum => ReadUInt16BigEndian(SpanBlockAlloc);
    private int BlockAlloc_Checksum_Inv => ReadUInt16BigEndian(SpanBlockAlloc[0x0002..]);

    private int BlockAllocBAK_Checksum => ReadUInt16BigEndian(SpanBlockAllocBAK);
    private int BlockAllocBAK_Checksum_Inv => ReadUInt16BigEndian(SpanBlockAllocBAK[0x0002..]);

    private int DirectoryBlock_Used;

    private const int NotPresent = -1;
    private int EntryCOLO = NotPresent;
    private int EntryXD = NotPresent;
    private int EntryRSBOX = NotPresent;
    private int EntrySelected = NotPresent;
    public bool HasCOLO => EntryCOLO >= 0;
    public bool HasXD => EntryXD >= 0;
    public bool HasRSBOX => EntryRSBOX >= 0;
    public int SaveGameCount { get; private set; }

    private bool IsCorruptedMemoryCard()
    {
        var csums = VerifyChecksums();
        if (csums == MemoryCardChecksumStatus.None)
            return false; // eager return, true for all correct Memory Cards.

        if (csums.HasFlag(MemoryCardChecksumStatus.HeaderBad))
            return true;

        if (csums.HasFlag(MemoryCardChecksumStatus.DirectoryBad))
        {
            if (csums.HasFlag(MemoryCardChecksumStatus.DirectoryBackupBad)) // backup is also wrong
                return true; // Directory checksum and directory backup checksum failed
            RestoreBackup(); // backup is correct, restore
            csums = VerifyChecksums(); // update checksums
        }

        if (!csums.HasFlag(MemoryCardChecksumStatus.BlockAllocBad))
            return false;
        if (csums.HasFlag(MemoryCardChecksumStatus.BlockAllocBackupBad)) // backup is also wrong
            return true;
        RestoreBackup(); // backup is correct, restore
        return false;
    }

    private void RestoreBackup()
    {
        SpanDirectoryBAK.CopyTo(SpanDirectory);
        SpanBlockAllocBAK.CopyTo(SpanBlockAlloc);
    }

    public MemoryCardSaveStatus GetMemoryCardState()
    {
        if (!IsMemoryCardSize(Data))
            return MemoryCardSaveStatus.Invalid; // Invalid size

        if (IsCorruptedMemoryCard())
            return MemoryCardSaveStatus.Invalid;

        // Use the most recent directory block
        DirectoryBlock_Used = DirectoryBAK_UpdateCounter > Directory_UpdateCounter
            ? DirectoryBackup_Block
            : Directory_Block;

        // Search for Pokémon saves in the directory
        SaveGameCount = 0;
        for (int i = 0; i < NumEntries_Directory; i++)
        {
            var dEntry = GetDEntry(i);
            if (dEntry.IsEmpty) // empty entry
                continue;
            if (dEntry.IsStartInvalid)
                continue;
            if (dEntry.SaveDataOffset + dEntry.SaveDataLength > Data.Length)
                continue; // Memory card directory contains info for deleted files with boundaries beyond memory card size, ignore

            var version = SaveHandlerGCI.GetGameCode(dEntry.GameCode);
            if (version == GameVersion.COLO)
            {
                if (HasCOLO) // another entry already exists
                    return MemoryCardSaveStatus.DuplicateCOLO;
                EntryCOLO = i;
                SaveGameCount++;
            }
            else if (version == GameVersion.XD)
            {
                if (HasXD) // another entry already exists
                    return MemoryCardSaveStatus.DuplicateXD;
                EntryXD = i;
                SaveGameCount++;
            }
            else if (version == GameVersion.RSBOX)
            {
                if (HasRSBOX) // another entry already exists
                    return MemoryCardSaveStatus.DuplicateRSBOX;
                EntryRSBOX = i;
                SaveGameCount++;
            }
        }
        return AutoInferState();
    }

    private MemoryCardSaveStatus AutoInferState()
    {
        if (SaveGameCount == 0)
            return MemoryCardSaveStatus.NoPkmSaveGame;
        if (SaveGameCount > 1)
            return MemoryCardSaveStatus.MultipleSaveGame;

        if (HasCOLO)
        {
            EntrySelected = EntryCOLO;
            return MemoryCardSaveStatus.SaveGameCOLO;
        }
        if (HasXD)
        {
            EntrySelected = EntryXD;
            return MemoryCardSaveStatus.SaveGameXD;
        }
        EntrySelected = EntryRSBOX;
        return MemoryCardSaveStatus.SaveGameRSBOX;
    }

    public bool IsNoGameSelected => SelectedGameVersion == GameVersion.Any;

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

    public ReadOnlyMemory<byte> ReadSaveGameData()
    {
        var entry = EntrySelected;
        if (entry < 0)
            return default; // No entry selected
        return ReadSaveGameData(entry);
    }

    private Memory<byte> GetSaveGame(DEntry entry) => Raw.Slice(entry.SaveDataOffset, entry.SaveDataLength);

    public DEntry GetDEntry(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, NumEntries_Directory, nameof(index));
        return new DEntry(SpanDirectoryActive.Slice(index * DENTRY_SIZE, DENTRY_SIZE));
    }

    public ReadOnlyMemory<byte> ReadSaveGameData(int entry)
    {
        var dEntry = GetDEntry(entry);
        return GetSaveGame(dEntry);
    }

    public void WriteSaveGameData(Span<byte> data)
    {
        var entry = EntrySelected;
        if (entry < 0) // Can't write anywhere
            return;
        WriteSaveGameData(data, entry);
    }

    public void WriteSaveGameData(Span<byte> data, int entry)
    {
        var dEntry = GetDEntry(entry);
        if (dEntry.SaveDataLength != data.Length) // File size should never change.
            throw new ArgumentException("Save data size changed.", nameof(entry));

        var dest = GetSaveGame(dEntry).Span;
        data.CopyTo(dest);

        var timestamp = EncounterDate.GetDateTimeGC();
        dEntry.SetModificationTime(timestamp);

        // Revise the DEntry of the injected save data to match that of the main DEntry
        var savDEntry = new DEntry(dest[..DENTRY_SIZE]);
        dEntry.CopyTo(savDEntry);
    }
}
