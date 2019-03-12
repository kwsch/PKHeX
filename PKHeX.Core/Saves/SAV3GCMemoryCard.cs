using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
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
    public sealed class SAV3GCMemoryCard
    {
        private const int BLOCK_SIZE = 0x2000;
        private const int MBIT_TO_BLOCKS = 0x10;
        private const int DENTRY_STRLEN = 0x20;
        private const int DENTRY_SIZE = 0x40;
        private const int NumEntries_Directory = BLOCK_SIZE / DENTRY_SIZE;

        private static readonly HashSet<int> ValidMemoryCardSizes = new HashSet<int>
        {
            0x0080000, // 512KB 59 Blocks Memory Card
            0x0100000, // 1MB
            0x0200000, // 2MB
            0x0400000, // 4MB 251 Blocks Memory Card
            0x0800000, // 8MB
            0x1000000, // 16MB 1019 Blocks Default Dolphin Memory Card
            0x2000000, // 64MB
            0x4000000, // 128MB
        };

        public static bool IsMemoryCardSize(long Size) => ValidMemoryCardSizes.Contains((int)Size);

        public static bool IsMemoryCardSize(byte[] Data)
        {
            if (!IsMemoryCardSize(Data.Length))
                return false; // bad size
            if (BitConverter.ToUInt64(Data, 0) == ulong.MaxValue)
                return false; // uninitialized
            return true;
        }

        private static readonly byte[] RawEmpty_DEntry = { 0xFF, 0xFF, 0xFF, 0xFF };

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

        public SAV3GCMemoryCard(byte[] data) => Data = data;

        // Checksums
        private void GetChecksum(int block, int offset, int length, out ushort csum, out ushort inv_csum)
        {
            csum = inv_csum = 0;
            var ofs = (block * BLOCK_SIZE) + offset;

            for (int i = 0; i < length; i++)
            {
                var val = BigEndian.ToUInt16(Data, ofs + (i * 2));
                csum += val;
                inv_csum += (ushort)~val;
            }
            if (csum == 0xffff)
                csum = 0;
            if (inv_csum == 0xffff)
                inv_csum = 0;
        }

        private uint VerifyChecksums()
        {
            uint results = 0;

            GetChecksum(Header_Block, 0, 0xFE, out ushort csum, out ushort csum_inv);
            if (Header_Checksum != csum || (Header_Checksum_Inv != csum_inv))
                results |= 1;

            GetChecksum(Directory_Block, 0, 0xFFE, out csum, out csum_inv);
            if (Directory_Checksum != csum || (Directory_Checksum_Inv != csum_inv))
                results |= 2;

            GetChecksum(DirectoryBackup_Block, 0, 0xFFE, out csum, out csum_inv);
            if (DirectoryBAK_Checksum != csum || (DirectoryBAK_Checksum_Inv != csum_inv))
                results |= 4;

            GetChecksum(BlockAlloc_Block, 4, 0xFFE, out csum, out csum_inv);
            if (BlockAlloc_Checksum != csum || (BlockAlloc_Checksum_Inv != csum_inv))
                results |= 8;

            GetChecksum(BlockAllocBackup_Block, 4, 0xFFE, out csum, out csum_inv);
            if ((BlockAllocBAK_Checksum != csum) || BlockAllocBAK_Checksum_Inv != csum_inv)
                results |= 16;

            return results;
        }

        // Structure
        private int Header_Size => BigEndian.ToUInt16(Data, Header + 0x0022);
        private ushort Header_Checksum => BigEndian.ToUInt16(Data, Header + 0x01fc);
        private ushort Header_Checksum_Inv => BigEndian.ToUInt16(Data, Header + 0x01fe);

        // Encoding (Windows-1252 or Shift JIS)
        private int Header_Encoding => BigEndian.ToUInt16(Data, Header + 0x0024);
        private bool Header_Japanese => Header_Encoding == 1;
        private Encoding EncodingType => Header_Japanese ? Encoding.GetEncoding(1252) : Encoding.GetEncoding(932);

        private int Directory_UpdateCounter => BigEndian.ToUInt16(Data, Directory + 0x1ffa);
        private int Directory_Checksum => BigEndian.ToUInt16(Data, Directory + 0x1ffc);
        private int Directory_Checksum_Inv => BigEndian.ToUInt16(Data, Directory + 0x1ffe);

        private int DirectoryBAK_UpdateCounter => BigEndian.ToUInt16(Data, DirectoryBAK + 0x1ffa);
        private int DirectoryBAK_Checksum => BigEndian.ToUInt16(Data, DirectoryBAK + 0x1ffc);
        private int DirectoryBAK_Checksum_Inv => BigEndian.ToUInt16(Data, DirectoryBAK + 0x1ffe);

        private int BlockAlloc_Checksum => BigEndian.ToUInt16(Data, BlockAlloc + 0x0000);
        private int BlockAlloc_Checksum_Inv => BigEndian.ToUInt16(Data, BlockAlloc + 0x0002);

        private int BlockAllocBAK_Checksum => BigEndian.ToUInt16(Data, BlockAllocBAK + 0x0000);
        private int BlockAllocBAK_Checksum_Inv => BigEndian.ToUInt16(Data, BlockAllocBAK + 0x0002);

        private int DirectoryBlock_Used;
        private int NumBlocks => (Data.Length/BLOCK_SIZE) - 5;

        private int EntryCOLO = -1;
        private int EntryXD = -1;
        private int EntryRSBOX = -1;
        private int EntrySelected = -1;
        public bool HasCOLO => EntryCOLO > -1;
        public bool HasXD => EntryXD > -1;
        public bool HasRSBOX => EntryRSBOX > -1;
        public int SaveGameCount;

        private bool IsCorruptedMemoryCard()
        {
            uint csums = VerifyChecksums();

            if ((csums & 0x1) == 1) // Header checksum failed
                return true;

            if ((csums & 0x2) == 1)  // directory checksum error!
            {
                if ((csums & 0x4) == 1) // backup is also wrong
                    return true; // Directory checksum and directory backup checksum failed

                RestoreBackup(); // backup is correct, restore
                csums = VerifyChecksums(); // update checksums
            }

            if ((csums & 0x8) != 1)
                return false;
            if ((csums & 0x10) == 1) // backup is also wrong
                return true;

            // backup is correct, restore
            RestoreBackup();
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

            string Empty_DEntry = EncodingType.GetString(RawEmpty_DEntry, 0, 4);
            // Search for pokemon savegames in the directory
            for (int i = 0; i < NumEntries_Directory; i++)
            {
                int offset = (DirectoryBlock_Used * BLOCK_SIZE) + (i * DENTRY_SIZE);
                string GameCode = EncodingType.GetString(Data, offset, 4);
                if (GameCode == Empty_DEntry)
                    continue;

                int FirstBlock = BigEndian.ToUInt16(Data, offset + 0x36);
                int BlockCount = BigEndian.ToUInt16(Data, offset + 0x38);

                // Memory card directory contains info for deleted files with boundaries beyond memory card size, ignore
                if (FirstBlock + BlockCount > NumBlocks)
                    continue;

                if (SaveUtil.HEADER_COLO.Contains(GameCode))
                {
                    if (EntryCOLO > -1) // another entry already exists
                        return GCMemoryCardState.DuplicateCOLO;
                    EntryCOLO = i;
                    SaveGameCount++;
                }
                if (SaveUtil.HEADER_XD.Contains(GameCode))
                {
                    if (EntryXD > -1) // another entry already exists
                        return GCMemoryCardState.DuplicateXD;
                    EntryXD = i;
                    SaveGameCount++;
                }
                if (SaveUtil.HEADER_RSBOX.Contains(GameCode))
                {
                    if (EntryRSBOX > -1) // another entry already exists
                        return GCMemoryCardState.DuplicateRSBOX;
                    EntryRSBOX = i;
                    SaveGameCount++;
                }
            }
            if (SaveGameCount == 0)
                return GCMemoryCardState.NoPkmSaveGame;

            if (SaveGameCount > 1)
                return GCMemoryCardState.MultipleSaveGame;

            if (EntryCOLO > -1)
            {
                EntrySelected = EntryCOLO;
                return GCMemoryCardState.SaveGameCOLO;
            }
            if (EntryXD > -1)
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
                if (EntryCOLO > -1 && EntrySelected == EntryCOLO)
                    return GameVersion.COLO;
                if (EntryXD > -1 && EntrySelected == EntryXD)
                    return GameVersion.XD;
                if (EntryRSBOX > -1 && EntrySelected == EntryRSBOX)
                    return GameVersion.RSBOX;
                return GameVersion.Any; //Default for no game selected
            }
        }

        public void SelectSaveGame(GameVersion Game)
        {
            switch (Game)
            {
                case GameVersion.COLO: if (EntryCOLO > -1) EntrySelected = EntryCOLO; break;
                case GameVersion.XD: if (EntryXD > -1) EntrySelected = EntryXD; break;
                case GameVersion.RSBOX: if (EntryRSBOX > -1) EntrySelected = EntryRSBOX; break;
            }
        }

        public string GCISaveName => GCISaveGameName();
        public byte[] SelectedSaveData { get => ReadSaveGameData(); set => WriteSaveGameData(value); }
        public byte[] Data { get; private set; }

        private string GCISaveGameName()
        {
            int offset = (DirectoryBlock_Used * BLOCK_SIZE) + (EntrySelected * DENTRY_SIZE);
            string GameCode = EncodingType.GetString(Data, offset, 4);
            string Makercode = EncodingType.GetString(Data, offset + 0x04, 2);
            string FileName = EncodingType.GetString(Data, offset + 0x08, DENTRY_STRLEN);

            return $"{Makercode}-{GameCode}-{Util.TrimFromZero(FileName)}.gci";
        }

        private byte[] ReadSaveGameData()
        {
            if (EntrySelected == -1)
                return Array.Empty<byte>(); // No entry selected

            int offset = (DirectoryBlock_Used * BLOCK_SIZE) + (EntrySelected * DENTRY_SIZE);
            int FirstBlock = BigEndian.ToUInt16(Data, offset + 0x36);
            int BlockCount = BigEndian.ToUInt16(Data, offset + 0x38);

            byte[] SaveData = new byte[BlockCount * BLOCK_SIZE];
            Array.Copy(Data, FirstBlock * BLOCK_SIZE, SaveData, 0, BlockCount * BLOCK_SIZE);

            return SaveData;
        }

        private void WriteSaveGameData(byte[] SaveData)
        {
            if (EntrySelected == -1) // Can't write anywhere
                return;

            int offset = (DirectoryBlock_Used * BLOCK_SIZE) + (EntrySelected * DENTRY_SIZE);
            int FirstBlock = BigEndian.ToUInt16(Data, offset + 0x36);
            int BlockCount = BigEndian.ToUInt16(Data, offset + 0x38);

            if (SaveData.Length != BlockCount * BLOCK_SIZE) // Invalid File Size
                return;

            Array.Copy(SaveData, 0, Data, FirstBlock * BLOCK_SIZE, BlockCount * BLOCK_SIZE);
        }
    }
}
