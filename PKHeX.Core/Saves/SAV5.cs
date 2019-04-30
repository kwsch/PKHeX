using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 <see cref="SaveFile"/> object.
    /// </summary>
    public sealed class SAV5 : SaveFile
    {
        // Save Data Attributes
        protected override string BAKText => $"{OT} ({(GameVersion)Game}) - {PlayTimeString}";
        public override string Filter => (Footer.Length != 0 ? "DeSmuME DSV|*.dsv|" : string.Empty) + "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        public SAV5(byte[] data = null, GameVersion versionOverride = GameVersion.Any)
        {
            Data = data ?? new byte[SaveUtil.SIZE_G5RAW];
            BAK = (byte[])Data.Clone();
            Exportable = !IsRangeEmpty(0, Data.Length);

            // Get Version
            if (data == null)
                Version = GameVersion.B2W2;
            else if (versionOverride != GameVersion.Any)
                Version = versionOverride;
            else Version = SaveUtil.GetIsG5SAV(Data);
            if (Version == GameVersion.Invalid)
                return;

            // First blocks are always the same position/size
            PCLayout = 0x0;
            Box = 0x400;
            Party = 0x18E00;
            Trainer1 = 0x19400;
            WondercardData = 0x1C800;
            AdventureInfo = 0x1D900;

            // Different Offsets for later blocks
            switch (Version)
            {
                case GameVersion.BW:
                    BattleBox = 0x20A00;
                    Trainer2 = 0x21200;
                    EventConst = 0x20100;
                    EventFlag = EventConst + 0x27C;
                    Daycare = 0x20E00;
                    PokeDex = 0x21600;
                    PokeDexLanguageFlags = PokeDex + 0x320;
                    BattleSubway = 0x21D00;
                    CGearInfoOffset = 0x1C000;
                    CGearDataOffset = 0x52000;
                    EntreeForestOffset = 0x22C00;

                    // Inventory offsets are the same for each game.
                    OFS_PouchHeldItem = 0x18400; // 0x188D7
                    OFS_PouchKeyItem = 0x188D8; // 0x18A23
                    OFS_PouchTMHM = 0x18A24; // 0x18BD7
                    OFS_PouchMedicine = 0x18BD8; // 0x18C97
                    OFS_PouchBerry = 0x18C98; // 0x18DBF
                    LegalItems = Legal.Pouch_Items_BW;
                    LegalKeyItems = Legal.Pouch_Key_BW;
                    LegalTMHMs = Legal.Pouch_TMHM_BW;
                    LegalMedicine = Legal.Pouch_Medicine_BW;
                    LegalBerries = Legal.Pouch_Berries_BW;

                    Personal = PersonalTable.BW;
                    break;
                case GameVersion.B2W2: // B2W2
                    BattleBox = 0x20900;
                    Trainer2 = 0x21100;
                    EventConst = 0x1FF00;
                    EventFlag = EventConst + 0x35E;
                    Daycare = 0x20D00;
                    PokeDex = 0x21400;
                    PokeDexLanguageFlags = PokeDex + 0x328; // forme flags size is + 8 from bw with new formes (therians)
                    BattleSubway = 0x21B00;
                    CGearInfoOffset = 0x1C000;
                    CGearDataOffset = 0x52800;
                    EntreeForestOffset = 0x22A00;

                    // Inventory offsets are the same for each game.
                    OFS_PouchHeldItem = 0x18400; // 0x188D7
                    OFS_PouchKeyItem = 0x188D8; // 0x18A23
                    OFS_PouchTMHM = 0x18A24; // 0x18BD7
                    OFS_PouchMedicine = 0x18BD8; // 0x18C97
                    OFS_PouchBerry = 0x18C98; // 0x18DBF
                    LegalItems = Legal.Pouch_Items_BW;
                    LegalKeyItems = Legal.Pouch_Key_B2W2;
                    LegalTMHMs = Legal.Pouch_TMHM_BW;
                    LegalMedicine = Legal.Pouch_Medicine_BW;
                    LegalBerries = Legal.Pouch_Berries_BW;

                    Personal = PersonalTable.B2W2;
                    break;
            }
            HeldItems = Legal.HeldItems_BW;
            Blocks = Version == GameVersion.BW ? BlockInfoNDS.BlocksBW : BlockInfoNDS.BlocksB2W2;

            if (!Exportable)
                ClearBoxes();
        }

        // Configuration
        public override SaveFile Clone() => new SAV5((byte[])Data.Clone(), Version) {Footer = (byte[])Footer.Clone()};

        public override int SIZE_STORED => PKX.SIZE_5STORED;
        protected override int SIZE_PARTY => PKX.SIZE_5PARTY;
        public override PKM BlankPKM => new PK5();
        public override Type PKMType => typeof(PK5);

        public override int BoxCount => 24;
        public override int MaxEV => 255;
        public override int Generation => 5;
        public override int OTLength => 7;
        public override int NickLength => 10;
        protected override int EventConstMax => (Version == GameVersion.BW ? 0x27C : 0x35E) >> 1;
        protected override int EventFlagMax => (Version == GameVersion.BW ? 0x16C : 0x17F) << 3;
        protected override int GiftCountMax => 12;

        public override int MaxMoveID => Legal.MaxMoveID_5;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_5;
        public override int MaxItemID => Version == GameVersion.BW ? Legal.MaxItemID_5_BW : Legal.MaxItemID_5_B2W2;
        public override int MaxAbilityID => Legal.MaxAbilityID_5;
        public override int MaxBallID => Legal.MaxBallID_5;
        public override int MaxGameID => Legal.MaxGameID_5; // B2

        // Blocks & Offsets
        public readonly IReadOnlyList<BlockInfoNDS> Blocks;
        protected override void SetChecksums() => Blocks.SetChecksums(Data);
        public override bool ChecksumsValid => Blocks.GetChecksumsValid(Data);
        public override string ChecksumInfo => Blocks.GetChecksumInfo(Data);

        private const int wcSeed = 0x1D290;

        public readonly int CGearInfoOffset, CGearDataOffset;
        private readonly int EntreeForestOffset;
        private readonly int Trainer2, AdventureInfo, BattleSubway;
        public readonly int PokeDexLanguageFlags;

        // Daycare
        public override int DaycareSeedSize => 16;

        public override int GetDaycareSlotOffset(int loc, int slot)
        {
            return Daycare + 4 + (0xE4 * slot);
        }

        public override string GetDaycareRNGSeed(int loc)
        {
            if (Version != GameVersion.B2W2)
                return null;
            var data = Data.Skip(Daycare + 0x1CC).Take(DaycareSeedSize/2).Reverse().ToArray();
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        public override uint? GetDaycareEXP(int loc, int slot)
        {
            return BitConverter.ToUInt32(Data, Daycare + 4 + 0xDC + (slot * 0xE4));
        }

        public override bool? IsDaycareOccupied(int loc, int slot)
        {
            return BitConverter.ToUInt32(Data, Daycare + (0xE4 * slot)) == 1;
        }

        public override void SetDaycareEXP(int loc, int slot, uint EXP)
        {
            BitConverter.GetBytes(EXP).CopyTo(Data, Daycare + 4 + 0xDC + (slot * 0xE4));
        }

        public override void SetDaycareOccupied(int loc, int slot, bool occupied)
        {
            BitConverter.GetBytes((uint)(occupied ? 1 : 0)).CopyTo(Data, Daycare + 0x1CC);
        }

        public override void SetDaycareRNGSeed(int loc, string seed)
        {
            if (Version != GameVersion.B2W2)
                return;
            Enumerable.Range(0, seed.Length)
                 .Where(x => x % 2 == 0)
                 .Select(x => Convert.ToByte(seed.Substring(x, 2), 16))
                 .Reverse().ToArray().CopyTo(Data, Daycare + 0x1CC);
        }

        // Inventory
        private readonly ushort[] LegalItems, LegalKeyItems, LegalTMHMs, LegalMedicine, LegalBerries;

        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch4(InventoryType.Items, LegalItems, 999, OFS_PouchHeldItem),
                    new InventoryPouch4(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem),
                    new InventoryPouch4(InventoryType.TMHMs, LegalTMHMs, 1, OFS_PouchTMHM),
                    new InventoryPouch4(InventoryType.Medicine, LegalMedicine, 999, OFS_PouchMedicine),
                    new InventoryPouch4(InventoryType.Berries, LegalBerries, 999, OFS_PouchBerry),
                };
                return pouch.LoadAll(Data);
            }
            set => value.SaveAll(Data);
        }

        // Storage
        public override int PartyCount
        {
            get => Data[Party + 4];
            protected set => Data[Party + 4] = (byte)value;
        }

        public override int GetBoxOffset(int box)
        {
            return Box + (SIZE_STORED * box * 30) + (box * 0x10);
        }

        public override int GetPartyOffset(int slot)
        {
            return Party + 8 + (SIZE_PARTY * slot);
        }

        public override string GetBoxName(int box)
        {
            if (box >= BoxCount)
                return string.Empty;
            return Util.TrimFromFFFF(Encoding.Unicode.GetString(Data, GetBoxNameOffset(box), 0x28));
        }

        public override void SetBoxName(int box, string value)
        {
            if (value.Length > 38)
                return;
            value += '\uFFFF';
            var data = Encoding.Unicode.GetBytes(value.PadRight(0x14, '\0'));
            SetData(data, GetBoxNameOffset(box));
        }

        private int GetBoxNameOffset(int box) => PCLayout + (0x28 * box) + 4;

        protected override int GetBoxWallpaperOffset(int box)
        {
            return PCLayout + 0x3C4 + box;
        }

        public override int CurrentBox
        {
            get => Data[PCLayout];
            set => Data[PCLayout] = (byte)value;
        }

        public override bool BattleBoxLocked
        {
            get => BattleBox >= 0 && Data[BattleBox + 0x358] != 0; // wifi/live
            set { }
        }

        protected override PKM GetPKM(byte[] data)
        {
            return new PK5(data);
        }

        protected override byte[] DecryptPKM(byte[] data)
        {
            return PKX.DecryptArray45(data);
        }

        protected override void SetPKM(PKM pkm)
        {
            var pk5 = (PK5)pkm;
            // Apply to this Save File
            DateTime Date = DateTime.Now;
            if (pk5.Trade(OT, TID, SID, Gender, Date.Day, Date.Month, Date.Year))
                pkm.RefreshChecksum();
        }

        // Mystery Gift
        public override MysteryGiftAlbum GiftAlbum
        {
            get
            {
                uint seed = BitConverter.ToUInt32(Data, wcSeed);
                MysteryGiftAlbum Info = new MysteryGiftAlbum { Seed = seed };
                byte[] wcData = GetData(WondercardData, 0xA90); // Encrypted, Decrypt
                PKX.CryptArray(wcData, seed);

                Info.Flags = new bool[GiftFlagMax];
                Info.Gifts = new MysteryGift[GiftCountMax];
                // 0x100 Bytes for Used Flags
                for (int i = 0; i < GiftFlagMax; i++)
                    Info.Flags[i] = (wcData[i/8] >> i%8 & 0x1) == 1;
                // 12 PGFs
                for (int i = 0; i < Info.Gifts.Length; i++)
                    Info.Gifts[i] = new PGF(wcData.Skip(0x100 + (i *PGF.Size)).Take(PGF.Size).ToArray());

                return Info;
            }
            set
            {
                byte[] wcData = new byte[0xA90];

                // Toss back into byte[]
                for (int i = 0; i < value.Flags.Length; i++)
                {
                    if (value.Flags[i])
                        wcData[i/8] |= (byte)(1 << (i & 7));
                }

                for (int i = 0; i < value.Gifts.Length; i++)
                    value.Gifts[i].Data.CopyTo(wcData, 0x100 + (i *PGF.Size));

                // Decrypted, Encrypt
                PKX.CryptArray(wcData, value.Seed);

                // Write Back
                wcData.CopyTo(Data, WondercardData);
                BitConverter.GetBytes(value.Seed).CopyTo(Data, wcSeed);
            }
        }

        protected override bool[] MysteryGiftReceivedFlags { get => Array.Empty<bool>(); set { } }
        protected override MysteryGift[] MysteryGiftCards { get => Array.Empty<MysteryGift>(); set { } }

        // Trainer Info
        public override string OT
        {
            get => GetString(Trainer1 + 0x4, 16);
            set => SetString(value, OTLength).CopyTo(Data, Trainer1 + 0x4);
        }

        public override int TID
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x14 + 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x14 + 0);
        }

        public override int SID
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x14 + 2);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x14 + 2);
        }

        public override uint Money
        {
            get => BitConverter.ToUInt32(Data, Trainer2);
            set => BitConverter.GetBytes(value).CopyTo(Data, Trainer2);
        }

        public override int Gender
        {
            get => Data[Trainer1 + 0x21];
            set => Data[Trainer1 + 0x21] = (byte)value;
        }

        public override int Language
        {
            get => Data[Trainer1 + 0x1E];
            set => Data[Trainer1 + 0x1E] = (byte)value;
        }

        public override int Game
        {
            get => Data[Trainer1 + 0x1F];
            set => Data[Trainer1 + 0x1F] = (byte)value;
        }

        public int Badges
        {
            get => Data[Trainer2 + 0x4];
            set => Data[Trainer2 + 0x4] = (byte)value;
        }

        public int M
        {
            get => BitConverter.ToInt32(Data, Trainer1 + 0x180);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x180);
        }

        public int X
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x186);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x186);
        }

        public int Z
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x18A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x18A);
        }

        public int Y
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x18E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x18E);
        }

        public override int PlayedHours
        {
            get => BitConverter.ToUInt16(Data, Trainer1 + 0x24);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x24);
        }

        public override int PlayedMinutes
        {
            get => Data[Trainer1 + 0x24 + 2];
            set => Data[Trainer1 + 0x24 + 2] = (byte)value;
        }

        public override int PlayedSeconds
        {
            get => Data[Trainer1 + 0x24 + 3];
            set => Data[Trainer1 + 0x24 + 3] = (byte)value;
        }

        public override uint SecondsToStart { get => BitConverter.ToUInt32(Data, AdventureInfo + 0x34); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x34); }
        public override uint SecondsToFame { get => BitConverter.ToUInt32(Data, AdventureInfo + 0x3C); set => BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x3C); }

        public int BP
        {
            get => BitConverter.ToUInt16(Data, BattleSubway);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, BattleSubway);
        }

        public ushort GetPWTRecord(int id) => GetPWTRecord((PWTRecordID) id);

        public ushort GetPWTRecord(PWTRecordID id)
        {
            if (id < PWTRecordID.Normal || id > PWTRecordID.MixMaster)
                throw new ArgumentException(nameof(id));
            int ofs = 0x2375C + ((int)id * 2);
            return BitConverter.ToUInt16(Data, ofs);
        }

        public void SetPWTRecord(int id, ushort value) => SetPWTRecord((PWTRecordID) id, value);

        public void SetPWTRecord(PWTRecordID id, ushort value)
        {
            if (id < PWTRecordID.Normal || id > PWTRecordID.MixMaster)
                throw new ArgumentException(nameof(id));
            int ofs = 0x2375C + ((int)id * 2);
            SetData(BitConverter.GetBytes(value), ofs);
        }

        protected override void SetDex(PKM pkm)
        {
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Invalid)
                return;
            if (PokeDex < 0)
                return;

            const int brSize = 0x54;
            int bit = pkm.Species - 1;
            int gender = pkm.Gender % 2; // genderless -> male
            int shiny = pkm.IsShiny ? 1 : 0;
            int shift = (shiny * 2) + gender + 1;
            int shiftoff = (shiny * brSize * 2) + (gender * brSize) + brSize;
            int ofs = PokeDex + 0x8 + (bit >> 3);

            // Set the Species Owned Flag
            Data[ofs + (brSize * 0)] |= (byte)(1 << (bit % 8));

            // Set the [Species/Gender/Shiny] Seen Flag
            Data[PokeDex + 0x8 + shiftoff + (bit / 8)] |= (byte)(1 << (bit&7));

            // Set the Display flag if none are set
            bool Displayed = false;
            Displayed |= (Data[ofs + (brSize * 5)] & (byte)(1 << (bit&7))) != 0;
            Displayed |= (Data[ofs + (brSize * 6)] & (byte)(1 << (bit&7))) != 0;
            Displayed |= (Data[ofs + (brSize * 7)] & (byte)(1 << (bit&7))) != 0;
            Displayed |= (Data[ofs + (brSize * 8)] & (byte)(1 << (bit&7))) != 0;
            if (!Displayed) // offset is already biased by brSize, reuse shiftoff but for the display flags.
                Data[ofs + (brSize *(shift + 4))] |= (byte)(1 << (bit&7));

            // Set the Language
            if (bit < 493) // shifted by 1, Gen5 species do not have international language bits
            {
                int lang = pkm.Language - 1; if (lang > 5) lang--; // 0-6 language vals
                if (lang < 0) lang = 1;
                Data[PokeDexLanguageFlags + (((bit * 7) + lang)>>3)] |= (byte)(1 << (((bit * 7) + lang) & 7));
            }

            // Formes
            int fc = Personal[pkm.Species].FormeCount;
            int f = B2W2 ? DexFormUtil.GetDexFormIndexB2W2(pkm.Species, fc) : DexFormUtil.GetDexFormIndexBW(pkm.Species, fc);
            if (f < 0) return;

            int FormLen = B2W2 ? 0xB : 0x9;
            int FormDex = PokeDex + 0x8 + (brSize * 9);
            bit = f + pkm.AltForm;

            // Set Form Seen Flag
            Data[FormDex + (FormLen * shiny) + (bit>>3)] |= (byte)(1 << (bit&7));

            // Set Displayed Flag if necessary, check all flags
            for (int i = 0; i < fc; i++)
            {
                bit = f + i;
                if ((Data[FormDex + (FormLen * 2) + (bit>>3)] & (byte)(1 << (bit&7))) != 0) // Nonshiny
                    return; // already set
                if ((Data[FormDex + (FormLen * 3) + (bit>>3)] & (byte)(1 << (bit&7))) != 0) // Shiny
                    return; // already set
            }
            bit = f + pkm.AltForm;
            Data[FormDex + (FormLen * (2 + shiny)) + (bit>>3)] |= (byte)(1 << (bit&7));
        }

        public override bool GetCaught(int species)
        {
            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int ofs = PokeDex // Raw Offset
                      + 0x08; // Magic + Flags
            return (1 << bm & Data[ofs + bd]) != 0;
        }

        public override bool GetSeen(int species)
        {
            const int brSize = 0x54;

            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int ofs = PokeDex // Raw Offset
                      + 0x08; // Magic + Flags

            for (int i = 1; i <= 4; i++)
            {
                if ((1 << bm & Data[ofs + bd + (i * brSize)]) != 0)
                    return true;
            }

            return false;
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString5(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString5(value, maxLength, PadToSize, PadWith);
        }

        // DLC
        private int CGearSkinInfoOffset => CGearInfoOffset + (B2W2 ? 0x10 : 0) + 0x24;

        private bool CGearSkinPresent
        {
            get => Data[CGearSkinInfoOffset + 2] == 1;
            set => Data[CGearSkinInfoOffset + 2] = Data[Trainer1 + (B2W2 ? 0x6C : 0x54)] = (byte) (value ? 1 : 0);
        }

        public byte[] CGearSkinData
        {
            get
            {
                byte[] data = new byte[0x2600];
                if (CGearSkinPresent)
                    Array.Copy(Data, CGearDataOffset, data, 0, data.Length);
                return data;
            }
            set
            {
                if (value == null)
                    return; // no clearing
                byte[] dlcfooter = { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x27, 0x00, 0x00, 0x27, 0x35, 0x05, 0x31, 0x00, 0x00 };

                byte[] bgdata = value;
                SetData(bgdata, CGearDataOffset);

                ushort chk = Checksums.CRC16_CCITT(bgdata);
                var chkbytes = BitConverter.GetBytes(chk);
                int footer = CGearDataOffset + bgdata.Length;

                BitConverter.GetBytes((ushort)1).CopyTo(Data, footer); // block updated once
                chkbytes.CopyTo(Data, footer + 2); // checksum
                chkbytes.CopyTo(Data, footer + 0x100); // second checksum
                dlcfooter.CopyTo(Data, footer + 0x102);
                ushort skinchkval = Checksums.CRC16_CCITT(Data, footer + 0x100, 4);
                BitConverter.GetBytes(skinchkval).CopyTo(Data, footer + 0x112);

                // Indicate in the save file that data is present
                BitConverter.GetBytes((ushort)0xC21E).CopyTo(Data, 0x19438);

                chkbytes.CopyTo(Data, CGearSkinInfoOffset);
                CGearSkinPresent = true;

                Edited = true;
            }
        }

        public EntreeForest EntreeData
        {
            get => new EntreeForest(GetData(EntreeForestOffset, 0x850));
            set => SetData(value.Write(), EntreeForestOffset);
        }
    }
}
