using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PKHeX.Core
{
    /* GameCube memory card format data, checksum and code to extract files based on Dolphin code, adapted from C++ to C#
     * https://github.com/dolphin-emu/dolphin/
     */

    public enum GCMemoryCardState
    {
        Invalid,
        NoPkmSaveGame,
        ColloseumSaveGame,
        XDSaveGame,
        RSBoxSaveGame,
        MultipleSaveGame,
        ColloseumSaveGameDuplicated,
        XDSaveGameDuplicated,
        RSBoxSaveGameDuplicated,
    }

    public sealed class SAV3GCMemoryCard
    {
        const int BLOCK_SIZE = 0x2000;
        const int MBIT_TO_BLOCKS = 0x10;
        const int DENTRY_STRLEN = 0x20;
        const int DENTRY_SIZE = 0x40;
        int NumEntries_Directory { get { return BLOCK_SIZE / DENTRY_SIZE; } }

        internal readonly string[] Colloseum_GameCode = new[]
        {
            "GC6J","GC6E","GC6P" // NTSC-J, NTSC-U, PAL
        };
        internal readonly string[] XD_GameCode = new[]
        {
            "GXXJ","GXXE","GXXP" // NTSC-J, NTSC-U, PAL
        };
        internal readonly string[] Box_GameCode = new[]
        {
            "GPXJ","GPXE","GPXP" // NTSC-J, NTSC-U, PAL
        };
        internal static readonly int[] validMCSizes = new[]
        {
            524288, // 512KB 59 Blocks Memory Card
            1048576, // 1MB
            2097152, // 2MB
            4194304, // 4MB 251 Blocks Memory Card
            8388608, // 8MB
            16777216, // 16MB 1019 Blocks Default Dolphin Memory Card
            33554432, // 64MB 
            67108864 // 128 MB
        };
        internal readonly byte[] RawEmpty_DEntry = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

        // Control blocks
        private const int Header_Block = 0;
        private const int Directory_Block = 1;
        private const int DirectoryBackup_Block = 2;
        private const int BlockAlloc_Block = 3;
        private const int BlockAllocBackup_Block = 4;

        // BigEndian treatment
        private ushort SwapEndian(ushort x)
        {
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }
        private ushort BigEndianToUint16(byte[] value, int startIndex)
        {
            ushort x = BitConverter.ToUInt16(value, startIndex);
            if (!BitConverter.IsLittleEndian)
                return x;
            return SwapEndian(x);
        }

        private void calc_checksumsBE(int blockoffset, int offset, int length, ref ushort csum, ref ushort inv_csum)
        {
            csum = inv_csum = 0;
            var ofs = blockoffset * BLOCK_SIZE + offset;

            for (int i = 0; i < length; ++i)
            {
                csum += BigEndianToUint16(RawData, ofs + i * 2);
                inv_csum += SwapEndian((ushort)(BitConverter.ToUInt16(RawData, ofs + i * 2) ^ 0xffff));
            }
            if (csum == 0xffff)
            {
                csum = 0;
            }
            if (inv_csum == 0xffff)
            {
                inv_csum = 0;
            }
        }
        
        private uint TestChecksums()
        {
            ushort csum = 0, csum_inv = 0;

            uint results = 0;

            calc_checksumsBE(Header_Block, 0, 0xFE, ref csum, ref csum_inv);
            if ((Header_Checksum != csum) || (Header_Checksum_Inv != csum_inv))
                results |= 1;

            calc_checksumsBE(Directory_Block, 0, 0xFFE, ref csum, ref csum_inv);
            if ((Directory_Checksum != csum) || (Directory_Checksum_Inv != csum_inv))
                results |= 2;

            calc_checksumsBE(DirectoryBackup_Block, 0, 0xFFE, ref csum, ref csum_inv);
            if ((DirectoryBck_Checksum != csum) || (DirectoryBck_Checksum_Inv != csum_inv))
                results |= 4;

            calc_checksumsBE(BlockAlloc_Block, 4, 0xFFE, ref csum, ref csum_inv);
            if ((BlockAlloc_Checksum != csum) || (BlockAlloc_Checksum_Inv != csum_inv))
                results |= 8;

            calc_checksumsBE(BlockAllocBackup_Block, 4, 0xFFE, ref csum, ref csum_inv);
            if ((BlockAllocBck_Checksum != csum) || (BlockAllocBck_Checksum_Inv != csum_inv))
                results |= 16;

            return results;
        }

        int Header_Size { get { return BigEndianToUint16(RawData, Header_Block * BLOCK_SIZE + 0x0022); } }
        ushort Header_Checksum { get { return BigEndianToUint16(RawData, Header_Block * BLOCK_SIZE + 0x01fc); } }
        ushort Header_Checksum_Inv { get { return BigEndianToUint16(RawData, Header_Block * BLOCK_SIZE + 0x01fe); } }

        //Encoding (Windows-1252 or Shift JIS)
        int Header_Encoding { get { return BigEndianToUint16(RawData, Header_Block * BLOCK_SIZE + 0x0024); } }
        bool Header_Japanese { get { return Header_Encoding == 1; } }
        Encoding Header_EncodingType { get { return Header_Japanese ? Encoding.GetEncoding(1252) : Encoding.GetEncoding(932); } }

        int Directory_UpdateCounter { get { return BigEndianToUint16(RawData, Directory_Block * BLOCK_SIZE + 0x1ffa); } }
        int Directory_Checksum { get { return BigEndianToUint16(RawData, Directory_Block * BLOCK_SIZE + 0x1ffc); } }
        int Directory_Checksum_Inv { get { return BigEndianToUint16(RawData, Directory_Block * BLOCK_SIZE + 0x1ffe); } }

        int DirectoryBck_UpdateCounter { get { return BigEndianToUint16(RawData, DirectoryBackup_Block * BLOCK_SIZE + 0x1ffa); } }
        int DirectoryBck_Checksum { get { return BigEndianToUint16(RawData, DirectoryBackup_Block * BLOCK_SIZE + 0x1ffc); } }
        int DirectoryBck_Checksum_Inv { get { return BigEndianToUint16(RawData, DirectoryBackup_Block * BLOCK_SIZE + 0x1ffe); } }

        int BlockAlloc_Checksum { get { return BigEndianToUint16(RawData, BlockAlloc_Block * BLOCK_SIZE + 0x0000); } }
        int BlockAlloc_Checksum_Inv { get { return BigEndianToUint16(RawData, BlockAlloc_Block * BLOCK_SIZE + 0x0002); } }

        int BlockAllocBck_Checksum { get { return BigEndianToUint16(RawData, BlockAllocBackup_Block * BLOCK_SIZE + 0x0000); } }
        int BlockAllocBck_Checksum_Inv { get { return BigEndianToUint16(RawData, BlockAllocBackup_Block * BLOCK_SIZE + 0x0002); } }

        byte[] RawData;
        int DirectoryBlock_Used;
        int NumBlocks => RawData.Length / BLOCK_SIZE - 5;
        
        int Colloseum_Entry = -1;
        int XD_Entry = -1;
        int RSBox_Entry = -1;
        int Selected_Entry = -1;
        public int SaveGameCount = 0;
        public bool HaveColloseumSaveGame => Colloseum_Entry > -1;
        public bool HaveXDSaveGame => XD_Entry > -1;
        public bool HaveRSBoxSaveGame => RSBox_Entry > -1;

        private bool IsCorruptedMemoryCard()
        {
            uint csums = TestChecksums();

            if ((csums & 0x1) == 1)
            {
                // Header checksum failed
                // invalid files do not always get here
                return true;
            }

            if ((csums & 0x2) == 1)  // directory checksum error!
            {
                if ((csums & 0x4) == 1) // backup is also wrong!
                {
                    // Directory checksum and directory backup checksum failed
                    return true;
                }
                else
                {
                    // backup is correct, restore
                    Array.Copy(RawData, DirectoryBackup_Block * BLOCK_SIZE, RawData, Directory_Block * BLOCK_SIZE, BLOCK_SIZE);
                    Array.Copy(RawData, BlockAlloc_Block * BLOCK_SIZE, RawData, BlockAllocBackup_Block * BLOCK_SIZE, BLOCK_SIZE);

                    // update checksums
                    csums = TestChecksums();
                }
            }

            if ((csums & 0x8) == 1)  // BAT checksum error!
            {
                if ((csums & 0x10) == 1)  // backup is also wrong!
                {
                    // Block Allocation Table checksum failed
                    return true;
                }
                else
                {
                    // backup is correct, restore
                    Array.Copy(RawData, DirectoryBackup_Block * BLOCK_SIZE, RawData, Directory_Block * BLOCK_SIZE, BLOCK_SIZE);
                    Array.Copy(RawData, BlockAlloc_Block * BLOCK_SIZE, RawData, BlockAllocBackup_Block * BLOCK_SIZE, BLOCK_SIZE);
                }
            }
            return false;
        }

        public static bool IsMemoryCardSize(long Size)
        {
            if (Size > int.MaxValue)
                return false;
            return validMCSizes.Contains(((int)Size));
        }

        public static bool IsMemoryCardSize(byte[] Data)
        {
            return validMCSizes.Contains(Data.Length);
        }

        public GCMemoryCardState LoadMemoryCardFile(byte[] Data)
        {
            RawData = Data;
            if (!IsMemoryCardSize(RawData))
                // Invalid size
                return GCMemoryCardState.Invalid;

            // Size in megabits, not megabytes
            int m_sizeMb = ((RawData.Length / BLOCK_SIZE) / MBIT_TO_BLOCKS);
            if (m_sizeMb != Header_Size)
                //Memory card file size does not match the header size
                return GCMemoryCardState.Invalid;

            if (IsCorruptedMemoryCard())
                return GCMemoryCardState.Invalid;

            // Use the most recent directory block
            if (DirectoryBck_UpdateCounter > Directory_UpdateCounter)
                DirectoryBlock_Used = DirectoryBackup_Block;
            else
                DirectoryBlock_Used = Directory_Block;

            string Empty_DEntry = Header_EncodingType.GetString(RawEmpty_DEntry, 0, 4);
            // Search for pokemon savegames in the directory
            for (int i = 0; i < NumEntries_Directory; i++)
            {
                string GameCode = Header_EncodingType.GetString(RawData, DirectoryBlock_Used * BLOCK_SIZE + i * DENTRY_SIZE, 4);
                if (GameCode == Empty_DEntry)
                    continue;
                int FirstBlock = BigEndianToUint16(RawData, DirectoryBlock_Used * BLOCK_SIZE + i * DENTRY_SIZE + 0x36);
                int BlockCount = BigEndianToUint16(RawData, DirectoryBlock_Used * BLOCK_SIZE + i * DENTRY_SIZE + 0x38);
                // Memory card directory contains info for deleted files with boundaries beyond memory card size, ignore
                if (FirstBlock + BlockCount > NumBlocks)
                    continue;
                if (Colloseum_GameCode.Contains(GameCode))
                {
                    if (Colloseum_Entry > -1)
                        // Memory Card contains more than 1 Pokémon Colloseum save data.
                        // It is not possible with a real GC nor with Dolphin to have multiple savegames in the same MC
                        // If two are found assume corrupted memory card, it wont work with the games after all
                        return GCMemoryCardState.ColloseumSaveGameDuplicated;
                    
                    Colloseum_Entry = i;
                    SaveGameCount++;
                }
                if (XD_GameCode.Contains(GameCode))
                {
                    if (XD_Entry > -1)
                        // Memory Card contains more than 1 Pokémon XD save data.
                        return GCMemoryCardState.XDSaveGameDuplicated;
                    XD_Entry = i;
                    SaveGameCount++;
                }
                if (Box_GameCode.Contains(GameCode))
                {
                    if (RSBox_Entry > -1)
                        // Memory Card contains more than 1 Pokémon RS Box save data.
                        return GCMemoryCardState.RSBoxSaveGameDuplicated;
                    RSBox_Entry = i;
                    SaveGameCount++;
                }
            }
            if (SaveGameCount == 0)
                // There is no savedata from a Pokémon GameCube game.
                return GCMemoryCardState.NoPkmSaveGame;
            
            if (SaveGameCount > 1)
                return GCMemoryCardState.MultipleSaveGame;
            
            if (Colloseum_Entry > -1)
            {
                Selected_Entry = Colloseum_Entry;
                return GCMemoryCardState.ColloseumSaveGame;
            }
            if(XD_Entry > -1)
            {
                Selected_Entry = XD_Entry;
                return GCMemoryCardState.XDSaveGame;
            }
            Selected_Entry = RSBox_Entry;
            return GCMemoryCardState.RSBoxSaveGame;
        }

        public GameVersion SelectedGameVersion
        {
            get
            {
                if(Colloseum_Entry > -1 && Selected_Entry == Colloseum_Entry)
                    return GameVersion.COLO;
                if (XD_Entry > -1 && Selected_Entry == XD_Entry)
                    return GameVersion.XD;
                if (RSBox_Entry > -1 && Selected_Entry == RSBox_Entry)
                    return GameVersion.RSBOX;
                return GameVersion.CXD; //Default for no game selected
            } 
        }

        public void SelectSaveGame(GameVersion Game)
        {
            switch(Game)
            {
                case GameVersion.COLO: if (Colloseum_Entry > -1) Selected_Entry = Colloseum_Entry; break;
                case GameVersion.XD: if (XD_Entry > -1) Selected_Entry = XD_Entry; break;
                case GameVersion.RSBOX: if (RSBox_Entry > -1) Selected_Entry = RSBox_Entry; break;
            }
        }

        public string getGCISaveGameName()
        {
            string GameCode = Header_EncodingType.GetString(RawData, DirectoryBlock_Used * BLOCK_SIZE + Selected_Entry * DENTRY_SIZE, 4);
            string Makercode = Header_EncodingType.GetString(RawData, DirectoryBlock_Used * BLOCK_SIZE + Selected_Entry * DENTRY_SIZE + 0x04, 2);
            string FileName = Header_EncodingType.GetString(RawData, DirectoryBlock_Used * BLOCK_SIZE + Selected_Entry * DENTRY_SIZE + 0x08, DENTRY_STRLEN);

            return Makercode + "-" + GameCode + "-" + FileName.Replace("\0", "") + ".gci";
        }

        public byte[] ReadSaveGameData()
        {
            if (Selected_Entry == -1)
                // Not selected any entry
                return null;

            int FirstBlock = BigEndianToUint16(RawData, DirectoryBlock_Used * BLOCK_SIZE + Selected_Entry * DENTRY_SIZE + 0x36);
            int BlockCount = BigEndianToUint16(RawData, DirectoryBlock_Used * BLOCK_SIZE + Selected_Entry * DENTRY_SIZE + 0x38);

            byte[] SaveData = new byte[BlockCount * BLOCK_SIZE];
            Array.Copy(RawData, FirstBlock * BLOCK_SIZE, SaveData, 0, BlockCount * BLOCK_SIZE);

            return SaveData;
        }

        public byte[] WriteSaveGameData(byte[] SaveData)
        {
            if (Selected_Entry == -1)
                // Not selected any entry
                return RawData;

            int FirstBlock = BigEndianToUint16(RawData, DirectoryBlock_Used * BLOCK_SIZE + Selected_Entry * DENTRY_SIZE + 0x36);
            int BlockCount = BigEndianToUint16(RawData, DirectoryBlock_Used * BLOCK_SIZE + Selected_Entry * DENTRY_SIZE + 0x38);

            if (SaveData.Length != BlockCount * BLOCK_SIZE)
                // Invalid File Size
                return null;

            Array.Copy(SaveData, 0, RawData, FirstBlock * BLOCK_SIZE, BlockCount * BLOCK_SIZE);
            return RawData;
        }
    }
}
