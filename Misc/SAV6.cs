using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public class BlockInfo
    {
        public int Offset;
        public int Length;
        public ushort ID;
        public ushort Checksum;
    }
    public enum GameVersion
    {
        X = 24, Y = 25,
        AS = 26, OR = 27,
        Z = 28,
        Unknown = -1,
    }
    public class SAV6 : PKX
    {
        // Global Settings
        internal static bool SetUpdateDex = true;
        internal static bool SetUpdatePK6 = true;
        // Save Data Attributes
        public byte[] Data;
        public bool Exportable;
        public SAV6(byte[] data)
        {
            Exportable = !data.SequenceEqual(new byte[data.Length]);
            Data = data;

            // Load Info
            getBlockInfo();
            getSAVOffsets();
        }
        
        public void getSAVOffsets()
        {
            if (XY)
            {
                Box = 0x22600;
                TrainerCard = 0x14000;
                Party = 0x14200;
                BattleBox = 0x04A00;
                Daycare = 0x1B200;
                GTS = 0x17800;
                Fused = 0x16000;
                SUBE = 0x1D890;
                Puff = 0x00000;
                Item = 0x00400;
                Items = new Inventory(Item, 0);
                Trainer1 = 0x1400;
                Trainer2 = 0x4200;
                PCLayout = 0x4400;
                PCBackgrounds = PCLayout + 0x41E;
                PCFlags = PCLayout + 0x43D;
                WondercardFlags = 0x1BC00;
                WondercardData = WondercardFlags + 0x100;
                BerryField = 0x1B800;
                OPower = 0x16A00;
                EventConst = 0x14A00;
                EventAsh = -1;
                EventFlag = EventConst + 0x2FC;
                PokeDex = 0x15000;
                PokeDexLanguageFlags = PokeDex + 0x3C8;
                Spinda = PokeDex + 0x648;
                EncounterCount = -1;
                HoF = 0x19400;
                SuperTrain = 0x1F200;
                JPEG = 0x57200;
                MaisonStats = 0x1B1C0;
                PSS = 0x05000;
                PSSStats = 0x1E400;
                Vivillon = 0x4250;
                BoxWallpapers = 0x481E;
                SecretBase = -1;
                EonTicket = -1;
                LastViewedBox = PCLayout + 0x43F;
            }
            else if (ORAS)
            {
                Box = 0x33000; // Confirmed
                TrainerCard = 0x14000; // Confirmed
                Party = 0x14200; // Confirmed
                BattleBox = 0x04A00; // Confirmed
                Daycare = 0x1BC00; // Confirmed (thanks Rei)
                GTS = 0x18200; // Confirmed
                Fused = 0x16A00; // Confirmed
                SUBE = 0x1D890; // ****not in use, not updating?****
                Puff = 0x00000; // Confirmed
                Item = 0x00400; // Confirmed
                Items = new Inventory(Item, 1);
                Trainer1 = 0x01400; // Confirmed
                Trainer2 = 0x04200; // Confirmed
                PCLayout = 0x04400; // Confirmed
                PCBackgrounds = PCLayout + 0x41E;
                PCFlags = PCLayout + 0x43D;
                WondercardFlags = 0x1CC00; // Confirmed
                WondercardData = WondercardFlags + 0x100;
                BerryField = 0x1C400; // ****changed****
                OPower = 0x17400; // ****changed****
                EventConst = 0x14A00;
                EventAsh = EventConst + 0x78;
                EventFlag = EventConst + 0x2FC;
                PokeDex = 0x15000;
                Spinda = PokeDex + 0x680;
                EncounterCount = PokeDex + 0x686;
                PokeDexLanguageFlags = PokeDex + 0x400;
                HoF = 0x19E00; // Confirmed
                SuperTrain = 0x20200;
                JPEG = 0x67C00; // Confirmed
                MaisonStats = 0x1BBC0;
                PSS = 0x05000; // Confirmed (thanks Rei)
                PSSStats = 0x1F400;
                Vivillon = 0x4244;
                BoxWallpapers = 0x481E;
                SecretBase = 0x23A00;
                EonTicket = 0x319B8;
                LastViewedBox = PCLayout + 0x43F;
            }
            DaycareSlot = new[] { Daycare, Daycare + 0x1F0 };
        }
        public class Inventory
        {
            public int HeldItem, KeyItem, Medicine, TMHM, Berry = 0;
            public Inventory(int Offset, int Game)
            {
                switch (Game)
                {
                    case 0:
                        HeldItem = Offset + 0;
                        KeyItem = Offset + 0x640;
                        TMHM = Offset + 0x7C0;
                        Medicine = Offset + 0x968;
                        Berry = Offset + 0xA68;
                        break;
                    case 1: 
                        HeldItem = Offset + 0;
                        KeyItem = Offset + 0x640;
                        TMHM = Offset + 0x7C0;
                        Medicine = Offset + 0x970;
                        Berry = Offset + 0xA70;
                        break;
                }
            }
        }
        public Inventory Items = new Inventory(0, -1);
        public int BattleBox, GTS, Daycare, EonTicket,
            Fused, SUBE, Puff, Item, Trainer1, Trainer2, SuperTrain, PSSStats, MaisonStats, Vivillon, SecretBase, BoxWallpapers, LastViewedBox,
            PCLayout, PCBackgrounds, PCFlags, WondercardFlags, WondercardData, BerryField, OPower, EventConst, EventFlag, EventAsh,
            PokeDex, PokeDexLanguageFlags, Spinda, EncounterCount, HoF, PSS, JPEG = 0;
        public int TrainerCard = 0x14000;
        public int Box = 0x33000, Party = 0x14200;
        public int[] DaycareSlot;
        public int DaycareIndex = 0;

        public GameVersion Version
        {
            get
            {
                switch (Game)
                {
                    case 24: return GameVersion.X;
                    case 25: return GameVersion.Y;
                    case 26: return GameVersion.AS;
                    case 27: return GameVersion.OR;
                }
                return GameVersion.Unknown;
            }
        }
        public bool ORAS { get { return ((Version == GameVersion.OR) || (Version == GameVersion.AS)); } }
        public bool XY { get { return ((Version == GameVersion.X) || (Version == GameVersion.Y)); } }

        // Save Information
        private int BlockInfoOffset;
        private BlockInfo[] Blocks;
        private void getBlockInfo()
        {
            BlockInfoOffset = Data.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(Data, BlockInfoOffset) != 0x42454546) // BEEF, nice!
                BlockInfoOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?
            int count = (Data.Length - BlockInfoOffset - 0x8) / 8;
            BlockInfoOffset += 4;

            Blocks = new BlockInfo[count];
            int CurrentPosition = 0;
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = new BlockInfo
                {
                    Offset = CurrentPosition,
                    Length = BitConverter.ToInt32(Data, BlockInfoOffset + 0 + 8*i),
                    ID = BitConverter.ToUInt16(Data, BlockInfoOffset + 4 + 8*i),
                    Checksum = BitConverter.ToUInt16(Data, BlockInfoOffset + 6 + 8*i)
                };

                // Expand out to nearest 0x200
                CurrentPosition += (Blocks[i].Length % 0x200 == 0) ? Blocks[i].Length : (0x200 - Blocks[i].Length % 0x200 + Blocks[i].Length);

                if ((Blocks[i].ID != 0) || i == 0) continue;
                count = i;
                break;
            }
            // Fix Final Array Lengths
            Array.Resize(ref Blocks, count);
        }
        private void setChecksums()
        {
            // Check for invalid block lengths
            if (Blocks.Length < 3) // arbitrary...
            {
                Console.WriteLine("Not enough blocks ({0}), aborting setChecksums", Blocks.Length);
                return;
            }
            // Apply checksums
            for (int i = 0; i < Blocks.Length; i++)
            {
                byte[] array = Data.Skip(Blocks[i].Offset).Take(Blocks[i].Length).ToArray();
                Array.Copy(BitConverter.GetBytes(ccitt16(array)), 0, Data, BlockInfoOffset + 6 + i * 8, 2);
            }
        }
        public byte[] Write()
        {
            setChecksums();
            return Data;
        }
        public int CurrentBox { get { return Data[LastViewedBox] & 0x1F; } set { Data[LastViewedBox] = (byte)value; } }

        // Player Information
        public ushort TID { get { return BitConverter.ToUInt16(Data, TrainerCard + 0); } }
        public ushort SID { get { return BitConverter.ToUInt16(Data, TrainerCard + 2); } }
        public byte Game { get { return Data[TrainerCard + 4]; } }
        public byte Gender { get { return Data[TrainerCard + 5]; } }
        public byte SubRegion { get { return Data[TrainerCard + 0x26]; } }
        public byte Country { get { return Data[TrainerCard + 0x27]; } }
        public byte ConsoleRegion { get { return Data[TrainerCard + 0x2C]; } }
        public byte Language { get { return Data[TrainerCard + 0x2D]; } }
        public string OT { get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, TrainerCard + 0x48, 0x1A)); } }

        // Misc Properties
        public int PartyCount
        {
            get { return Data[Party + 6 * PK6.SIZE_PARTY]; }
            set { Data[Party + 6 * PK6.SIZE_PARTY] = (byte)value; }
        }
        public bool BattleBoxLocked
        {
            get { return Data[BattleBox + 6 * PK6.SIZE_STORED] != 0; }
            set { Data[BattleBox + 6 * PK6.SIZE_STORED] = (byte)(value ? 1 : 0); }
        }
        public ulong DaycareRNGSeed
        {
            get { return BitConverter.ToUInt64(Data, DaycareSlot[DaycareIndex] + 0x1E8); }
            set { BitConverter.GetBytes(value).CopyTo(Data, DaycareSlot[DaycareIndex] + 0x1E8); }
        }
        public bool DaycareHasEgg
        {
            get { return Data[DaycareSlot[DaycareIndex] + 0x1E0] == 1; }
            set { Data[DaycareSlot[DaycareIndex] + 0x1E0] = (byte)(value ? 1 : 0); }
        }
        public uint DaycareEXP1
        {
            get { return BitConverter.ToUInt32(Data, DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*0 + 4); }
            set { BitConverter.GetBytes(value).CopyTo(Data, DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*0 + 4); }
        }
        public uint DaycareEXP2
        {
            get { return BitConverter.ToUInt32(Data, DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*1 + 4); }
            set { BitConverter.GetBytes(value).CopyTo(Data, DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*1 + 4); }
        }
        public bool DaycareOccupied1
        {
            get { return Data[DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*0] != 0; }
            set { Data[DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8)*0] = (byte) (value ? 1 : 0); }
        }
        public bool DaycareOccupied2
        {
            get { return Data[DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8) * 1] != 0; }
            set { Data[DaycareSlot[DaycareIndex] + (PK6.SIZE_STORED + 8) * 1] = (byte)(value ? 1 : 0); }
        }

        // Data Accessing
        public byte[] getData(int Offset, int Length)
        {
            return Data.Skip(Offset).Take(Length).ToArray();
        }
        public void setData(byte[] input, int Offset)
        {
            input.CopyTo(Data, Offset);
        }

        // Pokémon Requests
        public PK6 getPK6Party(int offset)
        {
            return new PK6(decryptArray(getData(offset , PK6.SIZE_PARTY)));
        }
        public PK6 getPK6Stored(int offset)
        {
            return new PK6(decryptArray(getData(offset, PK6.SIZE_STORED)));
        }
        public void setPK6Party(PK6 pk6, int offset, bool? trade = null, bool? dex = null)
        {
            if (SetUpdatePK6 || (trade ?? false))
                setPK6(pk6);
            if (SetUpdateDex || (dex ?? false))
                setDex(pk6);

            byte[] ek6 = encryptArray(pk6.Data);
            Array.Resize(ref ek6, PK6.SIZE_PARTY);
            setData(ek6, offset);
        }
        public void setPK6Stored(PK6 pk6, int offset, bool? trade = null, bool? dex = null)
        {
            if (SetUpdatePK6 || (trade ?? false))
                setPK6(pk6);
            if (SetUpdateDex || (dex ?? false))
                setDex(pk6);

            byte[] ek6 = encryptArray(pk6.Data);
            Array.Resize(ref ek6, PK6.SIZE_STORED);
            setData(ek6, offset);
        }
        public void setEK6Stored(byte[] ek6, int offset, bool? trade = null, bool? dex = null)
        {
            PK6 pk6 = new PK6(decryptArray(ek6));
            if (SetUpdatePK6 || (trade ?? false))
                setPK6(pk6);
            if (SetUpdateDex || (dex ?? false))
                setDex(pk6);

            Array.Resize(ref ek6, PK6.SIZE_STORED);
            setData(ek6, offset);
        }

        // Meta
        public void setPK6(PK6 pk6)
        {
            // Apply to this Save File
            int CT = pk6.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk6.Trade(OT, TID, SID, Country, SubRegion, Gender, false, Date.Day, Date.Month, Date.Year);
            if (CT != pk6.CurrentHandler) // Logic updated Friendship
            {
                if (pk6.Moves.Contains(216) && pk6.CurrentFriendship != 255) // Return
                    if (pk6.CurrentHandler == 0)
                        pk6.OT_Friendship = 255;
                    else pk6.HT_Friendship = 255;
                else if (pk6.Moves.Contains(218) && pk6.CurrentFriendship != 0)  // Frustration
                    if (pk6.CurrentHandler == 0)
                        pk6.OT_Friendship = 0;
                    else pk6.HT_Friendship = 0;
                else if (pk6.CurrentHandler == 1) // OT->HT, needs new Friendship/Affection
                    pk6.TradeFriendshipAffection(OT);
            }
            pk6.RefreshChecksum();
        }
        public void setDex(PK6 pk6)
        {
            if (pk6.Species == 0)
                return;

            int bit = pk6.Species - 1;
            int lang = pk6.Language - 1; if (lang > 5) lang--; // 0-6 language vals
            int origin = pk6.Version;
            int gender = pk6.Gender % 2; // genderless -> male
            int shiny = pk6.IsShiny ? 1 : 0;
            int shiftoff = (shiny * 0x60 * 2) + (gender * 0x60) + 0x60;

            // Set the [Species/Gender/Shiny] Owned Flag
            Data[PokeDex + shiftoff + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Owned quality flag
            if (origin < 0x18 && bit < 649 && !ORAS) // Species: 1-649 for X/Y, and not for ORAS; Set the Foreign Owned Flag
                Data[PokeDex + 0x64C + bit / 8] |= (byte)(1 << (bit % 8));
            else if (origin >= 0x18 || ORAS) // Set Native Owned Flag (should always happen)
                Data[PokeDex + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Set the Display flag if none are set
            bool[] chk =
            {
                // Flag Regions (base index 1 to reference Wiki and editor)
                (Data[PokeDex + 0x60*(5) + bit/8 + 0x8] & (byte) (1 << (bit%8))) != 0,
                (Data[PokeDex + 0x60*(6) + bit/8 + 0x8] & (byte) (1 << (bit%8))) != 0,
                (Data[PokeDex + 0x60*(7) + bit/8 + 0x8] & (byte) (1 << (bit%8))) != 0,
                (Data[PokeDex + 0x60*(8) + bit/8 + 0x8] & (byte) (1 << (bit%8))) != 0,
            };
            if (!chk.Contains(true)) // offset is already biased by 0x60, reuse shiftoff but for the display flags.
                Data[PokeDex + shiftoff + 0x60 * (4) + bit / 8 + 0x8] |= (byte)(1 << (bit % 8));

            // Set the Language
            if (lang < 0) lang = 1;
            Data[PokeDex + PokeDexLanguageFlags + (bit * 7 + lang) / 8] |= (byte)(1 << (((bit * 7) + lang) % 8));
        }
        public int getBoxWallpaper(int box)
        {
            return 1 + Data[BoxWallpapers + box];
        }
        public void setParty()
        {
            byte partymembers = 0; // start off with a ctr of 0
            for (int i = 0; i < 6; i++)
            {
                // Gather all the species
                byte[] data = new byte[PK6.SIZE_PARTY];
                Array.Copy(Data, Party + i * PK6.SIZE_PARTY, data, 0, PK6.SIZE_PARTY);
                byte[] decdata = decryptArray(data);
                int species = BitConverter.ToInt16(decdata, 8);
                if ((species != 0) && (species < 722))
                    Array.Copy(data, 0, Data, Party + (partymembers++) * PK6.SIZE_PARTY, PK6.SIZE_PARTY);
            }

            // Write in the current party count
            PartyCount = partymembers;
            // Zero out the party slots that are empty.
            for (int i = 0; i < 6; i++)
                if (i >= partymembers)
                    Array.Copy(encryptArray(new byte[PK6.SIZE_PARTY]), 0, Data, Party + (i * PK6.SIZE_PARTY), PK6.SIZE_PARTY);

            // Repeat for Battle Box.
            byte battlemem = 0;
            for (int i = 0; i < 6; i++)
            {
                // Gather all the species
                byte[] data = new byte[PK6.SIZE_PARTY];
                Array.Copy(Data, BattleBox + i * PK6.SIZE_STORED, data, 0, PK6.SIZE_STORED);
                byte[] decdata = decryptArray(data);
                int species = BitConverter.ToInt16(decdata, 8);
                if ((species != 0) && (species < 722))
                    Array.Copy(data, 0, Data, BattleBox + (battlemem++) * PK6.SIZE_STORED, PK6.SIZE_STORED);
            }

            // Zero out the party slots that are empty.
            for (int i = 0; i < 6; i++)
                if (i >= battlemem)
                    Array.Copy(encryptArray(new byte[PK6.SIZE_PARTY]), 0, Data, BattleBox + (i * PK6.SIZE_STORED), PK6.SIZE_STORED);

            if (battlemem == 0)
                BattleBoxLocked = false;
        }
        public void sortBoxes()
        {
            const int len = 31 * 30; // amount of pk6's in boxes

            // Fetch encrypted box data
            byte[][] bdata = new byte[len][];
            for (int i = 0; i < len; i++)
                bdata[i] = getData(Box + i * PK6.SIZE_STORED, PK6.SIZE_STORED);

            // Sorting Method: Data will sort empty slots to the end, then from the filled slots eggs will be last, then species will be sorted.
            var query = from i in bdata
                        let p = new PK6(decryptArray(i))
                        orderby
                            p.Species == 0 ascending,
                            p.IsEgg ascending,
                            p.Species ascending,
                            p.IsNicknamed ascending
                        select i;
            byte[][] sorted = query.ToArray();

            // Write data back
            for (int i = 0; i < len; i++)
                setData(sorted[i], Box + i * PK6.SIZE_STORED);
        }
    }
}
