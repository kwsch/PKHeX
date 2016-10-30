using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public sealed class SAV5 : SaveFile
    {
        // Save Data Attributes
        public override string BAKName => $"{FileName} [{OT} ({(GameVersion)Game}) - {PlayTimeString}].bak";
        public override string Filter => (Footer.Length > 0 ? "DeSmuME DSV|*.dsv|" : "") + "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";
        public SAV5(byte[] data = null, GameVersion versionOverride = GameVersion.Any)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G5RAW] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            // Get Version
            if (data == null)
                Version = GameVersion.B2W2;
            else if (versionOverride != GameVersion.Any)
                Version = versionOverride;
            else Version = SaveUtil.getIsG5SAV(Data);
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
                    Daycare = 0x20E00;
                    PokeDex = 0x21600;
                    PokeDexLanguageFlags = PokeDex + 0x320;
                    CGearInfoOffset = 0x1C000;
                    CGearDataOffset = 0x52000;

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
                    CGearInfoOffset = 0x1C000;
                    CGearDataOffset = 0x52800;

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
            getBlockInfo();

            if (!Exportable)
                resetBoxes();
        }

        // Configuration
        public override SaveFile Clone() { return new SAV5(Data, Version); }

        public override int SIZE_STORED => PKX.SIZE_5STORED;
        public override int SIZE_PARTY => PKX.SIZE_5PARTY;
        public override PKM BlankPKM => new PK5();
        public override Type PKMType => typeof(PK5);

        public override int BoxCount => 24;
        public override int MaxEV => 255;
        public override int Generation => 5;
        public override int OTLength => 8;
        public override int NickLength => 10;
        protected override int EventConstMax => 0x35E/2;
        protected override int GiftCountMax => 12;

        public override int MaxMoveID => 559;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_5;
        public override int MaxItemID => Version == GameVersion.BW ? 632 : 638;
        public override int MaxAbilityID => 164;
        public override int MaxBallID => 0x19;
        public override int MaxGameID => 23; // B2

        // Blocks & Offsets
        private BlockInfo[] Blocks;
        private void getBlockInfo()
        {
            // Can be slick with just a list of block lengths, but oh well, precomputed.
            if (Version == GameVersion.BW)
            {
                Blocks = new[]
                {
                    new BlockInfo(0x00000, 0x03E0, 0x003E2, 0x23F00), // Box Names
                    new BlockInfo(0x00400, 0x0FF0, 0x013F2, 0x23F02), // Box 1
                    new BlockInfo(0x01400, 0x0FF0, 0x023F2, 0x23F04), // Box 2
                    new BlockInfo(0x02400, 0x0FF0, 0x033F2, 0x23F06), // Box 3
                    new BlockInfo(0x03400, 0x0FF0, 0x043F2, 0x23F08), // Box 4
                    new BlockInfo(0x04400, 0x0FF0, 0x053F2, 0x23F0A), // Box 5
                    new BlockInfo(0x05400, 0x0FF0, 0x063F2, 0x23F0C), // Box 6
                    new BlockInfo(0x06400, 0x0FF0, 0x073F2, 0x23F0E), // Box 7
                    new BlockInfo(0x07400, 0x0FF0, 0x083F2, 0x23F10), // Box 8
                    new BlockInfo(0x08400, 0x0FF0, 0x093F2, 0x23F12), // Box 9
                    new BlockInfo(0x09400, 0x0FF0, 0x0A3F2, 0x23F14), // Box 10
                    new BlockInfo(0x0A400, 0x0FF0, 0x0B3F2, 0x23F16), // Box 11
                    new BlockInfo(0x0B400, 0x0FF0, 0x0C3F2, 0x23F18), // Box 12
                    new BlockInfo(0x0C400, 0x0FF0, 0x0D3F2, 0x23F1A), // Box 13
                    new BlockInfo(0x0D400, 0x0FF0, 0x0E3F2, 0x23F1C), // Box 14
                    new BlockInfo(0x0E400, 0x0FF0, 0x0F3F2, 0x23F1E), // Box 15
                    new BlockInfo(0x0F400, 0x0FF0, 0x103F2, 0x23F20), // Box 16
                    new BlockInfo(0x10400, 0x0FF0, 0x113F2, 0x23F22), // Box 17
                    new BlockInfo(0x11400, 0x0FF0, 0x123F2, 0x23F24), // Box 18
                    new BlockInfo(0x12400, 0x0FF0, 0x133F2, 0x23F26), // Box 19
                    new BlockInfo(0x13400, 0x0FF0, 0x143F2, 0x23F28), // Box 20
                    new BlockInfo(0x14400, 0x0FF0, 0x153F2, 0x23F2A), // Box 21
                    new BlockInfo(0x15400, 0x0FF0, 0x163F2, 0x23F2C), // Box 22
                    new BlockInfo(0x16400, 0x0FF0, 0x173F2, 0x23F2E), // Box 23
                    new BlockInfo(0x17400, 0x0FF0, 0x183F2, 0x23F30), // Box 24
                    new BlockInfo(0x18400, 0x09C0, 0x18DC2, 0x23F32), // Inventory
                    new BlockInfo(0x18E00, 0x0534, 0x19336, 0x23F34), // Party Pokemon
                    new BlockInfo(0x19400, 0x0068, 0x1946A, 0x23F36), // Trainer Data
                    new BlockInfo(0x19500, 0x009C, 0x1959E, 0x23F38), // ???
                    new BlockInfo(0x19600, 0x1338, 0x1A93A, 0x23F3A), // ???
                    new BlockInfo(0x1AA00, 0x07C4, 0x1B1C6, 0x23F3C), // ???
                    new BlockInfo(0x1B200, 0x0D54, 0x1BF56, 0x23F3E), // ???
                    new BlockInfo(0x1C000, 0x002C, 0x1C02E, 0x23F40), // Skin Info
                    new BlockInfo(0x1C100, 0x0658, 0x1C75A, 0x23F42), // ??? Gym badge data
                    new BlockInfo(0x1C800, 0x0A94, 0x1D296, 0x23F44), // ???
                    new BlockInfo(0x1D300, 0x01AC, 0x1D4AE, 0x23F46), // ???
                    new BlockInfo(0x1D500, 0x03EC, 0x1D8EE, 0x23F48), // ???
                    new BlockInfo(0x1D900, 0x005C, 0x1D95E, 0x23F4A), // Adventure Info
                    new BlockInfo(0x1DA00, 0x01E0, 0x1DBE2, 0x23F4C), // ???
                    new BlockInfo(0x1DC00, 0x00A8, 0x1DCAA, 0x23F4E), // ???
                    new BlockInfo(0x1DD00, 0x0460, 0x1E162, 0x23F50), // ???
                    new BlockInfo(0x1E200, 0x1400, 0x1F602, 0x23F52), // ???
                    new BlockInfo(0x1F700, 0x02A4, 0x1F9A6, 0x23F54), // ???
                    new BlockInfo(0x1FA00, 0x02DC, 0x1FCDE, 0x23F56), // ???
                    new BlockInfo(0x1FD00, 0x034C, 0x2004E, 0x23F58), // ???
                    new BlockInfo(0x20100, 0x03EC, 0x204EE, 0x23F5A), // ???
                    new BlockInfo(0x20500, 0x00F8, 0x205FA, 0x23F5C), // ???
                    new BlockInfo(0x20600, 0x02FC, 0x208FE, 0x23F5E), // ???
                    new BlockInfo(0x20900, 0x0094, 0x20996, 0x23F60), // ???
                    new BlockInfo(0x20A00, 0x035C, 0x20D5E, 0x23F62), // ???
                    new BlockInfo(0x20E00, 0x01CC, 0x20FCE, 0x23F64), // ???
                    new BlockInfo(0x21000, 0x0168, 0x2116A, 0x23F66), // ???
                    new BlockInfo(0x21200, 0x00EC, 0x212EE, 0x23F68), // ???
                    new BlockInfo(0x21300, 0x01B0, 0x214B2, 0x23F6A), // ???
                    new BlockInfo(0x21500, 0x001C, 0x2151E, 0x23F6C), // ???
                    new BlockInfo(0x21600, 0x04D4, 0x21AD6, 0x23F6E), // ???
                    new BlockInfo(0x21B00, 0x0034, 0x21B36, 0x23F70), // ???
                    new BlockInfo(0x21C00, 0x003C, 0x21C3E, 0x23F72), // ???
                    new BlockInfo(0x21D00, 0x01AC, 0x21EAE, 0x23F74), // ???
                    new BlockInfo(0x21F00, 0x0B90, 0x22A92, 0x23F76), // ???
                    new BlockInfo(0x22B00, 0x009C, 0x22B9E, 0x23F78), // ???
                    new BlockInfo(0x22C00, 0x0850, 0x23452, 0x23F7A), // Entralink Forest pokémon data
                    new BlockInfo(0x23500, 0x0028, 0x2352A, 0x23F7C), // ???
                    new BlockInfo(0x23600, 0x0284, 0x23886, 0x23F7E), // ???
                    new BlockInfo(0x23900, 0x0010, 0x23912, 0x23F80), // ???
                    new BlockInfo(0x23A00, 0x005C, 0x23A5E, 0x23F82), // ???
                    new BlockInfo(0x23B00, 0x016C, 0x23C6E, 0x23F84), // ???
                    new BlockInfo(0x23D00, 0x0040, 0x23D42, 0x23F86), // ???
                    new BlockInfo(0x23E00, 0x00FC, 0x23EFE, 0x23F88), // ???
                    new BlockInfo(0x23F00, 0x008C, 0x23F9A, 0x23F9A), // Checksums */
                };
            }
            else if (Version == GameVersion.B2W2)
            {
                Blocks = new[]
                {
                    // Offset, Length, CHKOfst, ChkMirror
                    new BlockInfo(0x00000, 0x03e0, 0x003E2, 0x25F00), // Box Names
                    new BlockInfo(0x00400, 0x0ff0, 0x013F2, 0x25F02), // Box 1
                    new BlockInfo(0x01400, 0x0ff0, 0x023F2, 0x25F04), // Box 2
                    new BlockInfo(0x02400, 0x0ff0, 0x033F2, 0x25F06), // Box 3
                    new BlockInfo(0x03400, 0x0ff0, 0x043F2, 0x25F08), // Box 4
                    new BlockInfo(0x04400, 0x0ff0, 0x053F2, 0x25F0A), // Box 5
                    new BlockInfo(0x05400, 0x0ff0, 0x063F2, 0x25F0C), // Box 6
                    new BlockInfo(0x06400, 0x0ff0, 0x073F2, 0x25F0E), // Box 7
                    new BlockInfo(0x07400, 0x0ff0, 0x083F2, 0x25F10), // Box 8
                    new BlockInfo(0x08400, 0x0ff0, 0x093F2, 0x25F12), // Box 9
                    new BlockInfo(0x09400, 0x0ff0, 0x0A3F2, 0x25F14), // Box 10
                    new BlockInfo(0x0A400, 0x0ff0, 0x0B3F2, 0x25F16), // Box 11
                    new BlockInfo(0x0B400, 0x0ff0, 0x0C3F2, 0x25F18), // Box 12
                    new BlockInfo(0x0C400, 0x0ff0, 0x0D3F2, 0x25F1A), // Box 13
                    new BlockInfo(0x0D400, 0x0ff0, 0x0E3F2, 0x25F1C), // Box 14
                    new BlockInfo(0x0E400, 0x0ff0, 0x0F3F2, 0x25F1E), // Box 15
                    new BlockInfo(0x0F400, 0x0ff0, 0x103F2, 0x25F20), // Box 16
                    new BlockInfo(0x10400, 0x0ff0, 0x113F2, 0x25F22), // Box 17
                    new BlockInfo(0x11400, 0x0ff0, 0x123F2, 0x25F24), // Box 18
                    new BlockInfo(0x12400, 0x0ff0, 0x133F2, 0x25F26), // Box 19
                    new BlockInfo(0x13400, 0x0ff0, 0x143F2, 0x25F28), // Box 20
                    new BlockInfo(0x14400, 0x0ff0, 0x153F2, 0x25F2A), // Box 21
                    new BlockInfo(0x15400, 0x0ff0, 0x163F2, 0x25F2C), // Box 22
                    new BlockInfo(0x16400, 0x0ff0, 0x173F2, 0x25F2E), // Box 23
                    new BlockInfo(0x17400, 0x0ff0, 0x183F2, 0x25F30), // Box 24
                    new BlockInfo(0x18400, 0x09ec, 0x18DEE, 0x25F32), // Inventory
                    new BlockInfo(0x18E00, 0x0534, 0x19336, 0x25F34), // Party Pokemon
                    new BlockInfo(0x19400, 0x00b0, 0x194B2, 0x25F36), // Trainer Data
                    new BlockInfo(0x19500, 0x00a8, 0x195AA, 0x25F38), // Trainer Position
                    new BlockInfo(0x19600, 0x1338, 0x1A93A, 0x25F3A), // Unity Tower and survey stuff
                    new BlockInfo(0x1AA00, 0x07c4, 0x1B1C6, 0x25F3C), // Pal Pad Player Data (30d)
                    new BlockInfo(0x1B200, 0x0d54, 0x1BF56, 0x25F3E), // Pal Pad Friend Data
                    new BlockInfo(0x1C000, 0x0094, 0x1C096, 0x25F40), // Skin Info
                    new BlockInfo(0x1C100, 0x0658, 0x1C75A, 0x25F42), // Card Signature Block & ????
                    new BlockInfo(0x1C800, 0x0a94, 0x1D296, 0x25F44), // Mystery Gift
                    new BlockInfo(0x1D300, 0x01ac, 0x1D4AE, 0x25F46), // Dream World Stuff (Catalog)
                    new BlockInfo(0x1D500, 0x03ec, 0x1D8EE, 0x25F48), // Chatter
                    new BlockInfo(0x1D900, 0x005c, 0x1D95E, 0x25F4A), // Adventure data
                    new BlockInfo(0x1DA00, 0x01e0, 0x1DBE2, 0x25F4C), // Trainer Card Records
                    new BlockInfo(0x1DC00, 0x00a8, 0x1DCAA, 0x25F4E), // ???
                    new BlockInfo(0x1DD00, 0x0460, 0x1E162, 0x25F50), // (40d)
                    new BlockInfo(0x1E200, 0x1400, 0x1F602, 0x25F52), // ???
                    new BlockInfo(0x1F700, 0x02a4, 0x1F9A6, 0x25F54), // Contains flags and references for downloaded data (Musical)
                    new BlockInfo(0x1FA00, 0x00e0, 0x1FAE2, 0x25F56), // Fused Reshiram/Zekrom Storage
                    new BlockInfo(0x1FB00, 0x034c, 0x1FE4E, 0x25F58), // ???
                    new BlockInfo(0x1FF00, 0x04e0, 0x203E2, 0x25F5A), // Const Data Block and Event Flag Block (0x35E is the split)
                    new BlockInfo(0x20400, 0x00f8, 0x204FA, 0x25F5C), // ???
                    new BlockInfo(0x20500, 0x02fc, 0x207FE, 0x25F5E), // Tournament Block
                    new BlockInfo(0x20800, 0x0094, 0x20896, 0x25F60), // ???
                    new BlockInfo(0x20900, 0x035c, 0x20C5E, 0x25F62), // Battle Box Block
                    new BlockInfo(0x20D00, 0x01d4, 0x20ED6, 0x25F64), // Daycare Block (50d)
                    new BlockInfo(0x20F00, 0x01e0, 0x201E2, 0x25F66), // Strength Boulder Status Block
                    new BlockInfo(0x21100, 0x00f0, 0x211F2, 0x25F68), // Badge Flags, Money, Trainer Sayings
                    new BlockInfo(0x21200, 0x01b4, 0x213B6, 0x25F6A), // Entralink (Level & Powers etc)
                    new BlockInfo(0x21400, 0x04dc, 0x218DE, 0x25F6C), // Pokedex
                    new BlockInfo(0x21900, 0x0034, 0x21936, 0x25F6E), // Swarm and other overworld info - 2C - swarm, 2D - repel steps, 2E repel type
                    new BlockInfo(0x21A00, 0x003c, 0x21A3E, 0x25F70), // ???
                    new BlockInfo(0x21B00, 0x01ac, 0x21CAE, 0x25F72), // ???
                    new BlockInfo(0x21D00, 0x0b90, 0x22892, 0x25F74), // ???
                    new BlockInfo(0x22900, 0x00ac, 0x229AE, 0x25F76), // Online Records
                    new BlockInfo(0x22A00, 0x0850, 0x23252, 0x25F78), // Area NPC data - encrypted (60d)
                    new BlockInfo(0x23300, 0x0284, 0x23586, 0x25F7A), // ???
                    new BlockInfo(0x23600, 0x0010, 0x23612, 0x25F7C), // ???
                    new BlockInfo(0x23700, 0x00a8, 0x237AA, 0x25F7E), // ???
                    new BlockInfo(0x23800, 0x016c, 0x2396E, 0x25F80), // ???
                    new BlockInfo(0x23A00, 0x0080, 0x23A82, 0x25F82), // ???
                    new BlockInfo(0x23B00, 0x00fc, 0x23BFE, 0x25F84), // Hollow/Rival Block
                    new BlockInfo(0x23C00, 0x16a8, 0x252AA, 0x25F86), // Join Avenue Block
                    new BlockInfo(0x25300, 0x0498, 0x2579A, 0x25F88), // Medal data
                    new BlockInfo(0x25800, 0x0060, 0x25862, 0x25F8A), // Key-related data
                    new BlockInfo(0x25900, 0x00fc, 0x259FE, 0x25F8C), // (70d)
                    new BlockInfo(0x25A00, 0x03e4, 0x25DE6, 0x25F8E), // ???
                    new BlockInfo(0x25E00, 0x00f0, 0x25EF2, 0x25F90), // ???
                    new BlockInfo(0x25F00, 0x0094, 0x25FA2, 0x25FA2), // Checksum Block (73d)
                };
            }
            else
            {
                Blocks = new BlockInfo[] { };
            }
        }
        protected override void setChecksums()
        {
            // Check for invalid block lengths
            if (Blocks.Length < 3) // arbitrary...
            {
                Console.WriteLine("Not enough blocks ({0}), aborting setChecksums", Blocks.Length);
                return;
            }
            // Apply checksums
            foreach (BlockInfo b in Blocks)
            {
                byte[] array = Data.Skip(b.Offset).Take(b.Length).ToArray();
                ushort chk = SaveUtil.ccitt16(array);
                BitConverter.GetBytes(chk).CopyTo(Data, b.ChecksumOffset);
                BitConverter.GetBytes(chk).CopyTo(Data, b.ChecksumMirror);
            }
        }
        public override bool ChecksumsValid
        {
            get
            {
                // Check for invalid block lengths
                if (Blocks.Length < 3) // arbitrary...
                {
                    Console.WriteLine("Not enough blocks ({0}), aborting setChecksums", Blocks.Length);
                    return false;
                }

                foreach (BlockInfo b in Blocks)
                {
                    byte[] array = Data.Skip(b.Offset).Take(b.Length).ToArray();
                    ushort chk = SaveUtil.ccitt16(array);
                    if (chk != BitConverter.ToUInt16(Data, b.ChecksumOffset))
                        return false;
                    if (chk != BitConverter.ToUInt16(Data, b.ChecksumMirror))
                        return false;
                }
                return true;
            }
        }
        public override string ChecksumInfo
        {
            get
            {
                // Check for invalid block lengths
                if (Blocks.Length < 3) // arbitrary...
                {
                    Console.WriteLine("Not enough blocks ({0}), aborting setChecksums", Blocks.Length);
                    return "Not a valid save to check.";
                }
                string r = "";
                int invalid = 0;
                for (int i = 0; i < Blocks.Length; i++)
                {
                    BlockInfo b = Blocks[i];
                    byte[] array = Data.Skip(b.Offset).Take(b.Length).ToArray();
                    ushort chk = SaveUtil.ccitt16(array);
                    if (chk != BitConverter.ToUInt16(Data, b.ChecksumOffset))
                    {
                        r += $"Block {i} has been modified." + Environment.NewLine;
                        invalid++;
                    }
                    else if (chk != BitConverter.ToUInt16(Data, b.ChecksumMirror))
                    {
                        r += $"Block {i} mirror does not match." + Environment.NewLine;
                        invalid++;
                    }
                }
                r += $"SAV: {Blocks.Length - invalid}/{Blocks.Length + Environment.NewLine}";
                return r;
            }
        }
        
        private const int wcSeed = 0x1D290;

        public readonly int CGearInfoOffset, CGearDataOffset;
        private readonly int Trainer2, AdventureInfo, PokeDexLanguageFlags;
        public override bool HasBoxWallpapers => false;
        public override bool HasPokeDex => false;

        // Daycare
        public override int DaycareSeedSize => 16;
        public override int getDaycareSlotOffset(int loc, int slot)
        {
            return Daycare + 4 + 0xE4 * slot;
        }
        public override string getDaycareRNGSeed(int loc)
        {
            if (Version != GameVersion.B2W2)
                return null;
            var data = Data.Skip(Daycare + 0x1CC).Take(DaycareSeedSize/2).Reverse().ToArray();
            return BitConverter.ToString(data).Replace("-", "");
        }
        public override uint? getDaycareEXP(int loc, int slot)
        {
            return BitConverter.ToUInt32(Data, Daycare + 4 + 0xDC + slot * 0xE4);
        }
        public override bool? getDaycareOccupied(int loc, int slot)
        {
            return BitConverter.ToUInt32(Data, Daycare + 0xE4*slot) == 1;
        }
        public override void setDaycareEXP(int loc, int slot, uint EXP)
        {
            BitConverter.GetBytes(EXP).CopyTo(Data, Daycare + 4 + 0xDC + slot * 0xE4);
        }
        public override void setDaycareOccupied(int loc, int slot, bool occupied)
        {
            BitConverter.GetBytes((uint)(occupied ? 1 : 0)).CopyTo(Data, Daycare + 0x1CC);
        }
        public override void setDaycareRNGSeed(int loc, string seed)
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
                    new InventoryPouch(InventoryType.Items, LegalItems, 995, OFS_PouchHeldItem),
                    new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 1, OFS_PouchKeyItem),
                    new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, 1, OFS_PouchTMHM),
                    new InventoryPouch(InventoryType.Medicine, LegalMedicine, 995, OFS_PouchMedicine),
                    new InventoryPouch(InventoryType.Berries, LegalBerries, 995, OFS_PouchBerry),
                };
                foreach (var p in pouch)
                    p.getPouch(ref Data);
                return pouch;
            }
            set
            {
                foreach (var p in value)
                    p.setPouch(ref Data);
            }
        }

        // Storage
        public override int PartyCount
        {
            get { return Data[Party + 4]; }
            protected set { Data[Party + 4] = (byte)value; }
        }
        public override int getBoxOffset(int box)
        {
            return Box + SIZE_STORED * box * 30 + box * 0x10;
        }
        public override int getPartyOffset(int slot)
        {
            return Party + 8 + SIZE_PARTY*slot;
        }
        public override string getBoxName(int box)
        {
            if (box >= BoxCount)
                return "";
            return PKX.TrimFromFFFF(Encoding.Unicode.GetString(Data, PCLayout + 0x28 * box + 4, 0x28));
        }
        public override void setBoxName(int box, string val)
        {
            if (val.Length > 38)
                return;
            val += '\uFFFF';
            Encoding.Unicode.GetBytes(val.PadRight(0x14, '\0')).CopyTo(Data, PCLayout + 0x28 * box + 4);
            Edited = true;
        }
        protected override int getBoxWallpaperOffset(int box)
        {
            return PCLayout + 0x3C4 + box;
        }
        public override int CurrentBox
        {
            get { return Data[PCLayout]; }
            set { Data[PCLayout] = (byte)value; }
        }
        public override bool BattleBoxLocked // TODO
        {
            get { return false; }
            set { }
        }
        public override PKM getPKM(byte[] data)
        {
            return new PK5(data);
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return PKX.decryptArray45(data);
        }
        
        // Mystery Gift
        public override MysteryGiftAlbum GiftAlbum
        {
            get
            {
                uint seed = BitConverter.ToUInt32(Data, wcSeed);
                MysteryGiftAlbum Info = new MysteryGiftAlbum { Seed = seed };
                byte[] wcData = Data.Skip(WondercardData).Take(0xA90).ToArray(); // Encrypted, Decrypt
                for (int i = 0; i < wcData.Length; i += 2)
                    BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(wcData, i) ^ PKX.LCRNG(ref seed) >> 16)).CopyTo(wcData, i);

                Info.Flags = new bool[GiftFlagMax];
                Info.Gifts = new MysteryGift[GiftCountMax];
                // 0x100 Bytes for Used Flags
                for (int i = 0; i < GiftFlagMax; i++)
                    Info.Flags[i] = (wcData[i/8] >> i%8 & 0x1) == 1;
                // 12 PGFs
                for (int i = 0; i < Info.Gifts.Length; i++)
                    Info.Gifts[i] = new PGF(wcData.Skip(0x100 + i*PGF.Size).Take(PGF.Size).ToArray());

                return Info;
            }
            set
            {
                byte[] wcData = new byte[0xA90];

                // Toss back into byte[]
                for (int i = 0; i < value.Flags.Length; i++)
                    if (value.Flags[i])
                        wcData[i/8] |= (byte)(1 << (i & 7));
                for (int i = 0; i < value.Gifts.Length; i++)
                    value.Gifts[i].Data.CopyTo(wcData, 0x100 + i*PGF.Size);

                // Decrypted, Encrypt
                uint seed = value.Seed;
                for (int i = 0; i < wcData.Length; i += 2)
                    BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(wcData, i) ^ PKX.LCRNG(ref seed) >> 16)).CopyTo(wcData, i);

                // Write Back
                wcData.CopyTo(Data, WondercardData);
                BitConverter.GetBytes(value.Seed).CopyTo(Data, wcSeed);
            }
        }
        protected override bool[] MysteryGiftReceivedFlags { get { return null; } set { } }
        protected override MysteryGift[] MysteryGiftCards { get { return new MysteryGift[0]; } set { } }
        
        // Trainer Info
        public override string OT
        {
            get { return PKX.TrimFromFFFF(Encoding.Unicode.GetString(Data, Trainer1 + 0x4, 16)); }
            set
            {
                if (value.Length > 7)
                    value = value.Substring(0, 7); // Hard cap
                Encoding.Unicode.GetBytes((value + '\uFFFF').PadRight(8, '\0')).CopyTo(Data, Trainer1 + 0x4); }
            }
        public override ushort TID
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x14 + 0); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x14 + 0); }
        }
        public override ushort SID
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x14 + 2); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer1 + 0x14 + 2); }
        }
        public override uint Money
        {
            get { return BitConverter.ToUInt32(Data, Trainer2); }
            set { BitConverter.GetBytes(value).CopyTo(Data, Trainer2); }
        }
        public override int Gender
        {
            get { return Data[Trainer1 + 0x21]; }
            set { Data[Trainer1 + 0x21] = (byte)value; }
        }
        public override int Language
        {
            get { return Data[Trainer1 + 0x1E]; }
            set { Data[Trainer1 + 0x1E] = (byte)value; }
        }
        public override int Game
        {
            get { return Data[Trainer1 + 0x1F]; }
            set { Data[Trainer1 + 0x1F] = (byte)value; }
        }
        public int Badges
        {
            get { return Data[Trainer2 + 0x4]; }
            set { Data[Trainer2 + 0x4] = (byte)value; }
        }
        public int M
        {
            get { return BitConverter.ToInt32(Data, Trainer1 + 0x180); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x180); }
        }
        public int X
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x186); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x186); }
        }
        public int Z
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x18A); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x18A); }
        }
        public int Y
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x18E); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x18E); }
        }

        public override int PlayedHours
        {
            get { return BitConverter.ToUInt16(Data, Trainer1 + 0x24); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Trainer1 + 0x24); }
        }
        public override int PlayedMinutes
        {
            get { return Data[Trainer1 + 0x24 + 2]; }
            set { Data[Trainer1 + 0x24 + 2] = (byte)value; }
        }
        public override int PlayedSeconds
        {
            get { return Data[Trainer1 + 0x24 + 3]; }
            set { Data[Trainer1 + 0x24 + 3] = (byte)value; }
        }
        public override int SecondsToStart { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x34); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x34); } }
        public override int SecondsToFame { get { return BitConverter.ToInt32(Data, AdventureInfo + 0x3C); } set { BitConverter.GetBytes(value).CopyTo(Data, AdventureInfo + 0x3C); } }

        protected override void setDex(PKM pkm)
        {
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Unknown)
                return;
            if (PokeDex < 0)
                return;

            const int brSize = 0x54;
            int bit = pkm.Species - 1;
            int lang = pkm.Language - 1; if (lang > 5) lang--; // 0-6 language vals
            int gender = pkm.Gender % 2; // genderless -> male
            int shiny = pkm.IsShiny ? 1 : 0;
            int shiftoff = shiny * brSize * 2 + gender * brSize + brSize;

            // Set the Species Owned Flag
            Data[PokeDex + 0x8 + bit / 8] |= (byte)(1 << (bit % 8));

            // Set the [Species/Gender/Shiny] Seen Flag
            Data[PokeDex + 0x8 + shiftoff + bit / 8] |= (byte)(1 << (bit % 8));

            // Set the Display flag if none are set
            bool Displayed = false;
            Displayed |= (Data[PokeDex + 0x8 + brSize*5 + bit/8] & (byte)(1 << (bit%8))) != 0;
            Displayed |= (Data[PokeDex + 0x8 + brSize*6 + bit/8] & (byte)(1 << (bit%8))) != 0;
            Displayed |= (Data[PokeDex + 0x8 + brSize*7 + bit/8] & (byte)(1 << (bit%8))) != 0;
            Displayed |= (Data[PokeDex + 0x8 + brSize*8 + bit/8] & (byte)(1 << (bit%8))) != 0;
            if (!Displayed) // offset is already biased by brSize, reuse shiftoff but for the display flags.
                Data[PokeDex + 0x8 + shiftoff + brSize*4 + bit/8] |= (byte)(1 << (bit%8));

            // Set the Language
            if (lang < 0) lang = 1;
            Data[PokeDexLanguageFlags + (bit*7 + lang) / 8] |= (byte)(1 << ((bit*7 + lang) % 8));

            // Formes
            int fc = Personal[pkm.Species].FormeCount;
            int f = B2W2 ? SaveUtil.getDexFormIndexB2W2(pkm.Species, fc) : SaveUtil.getDexFormIndexBW(pkm.Species, fc);
            if (f < 0) return;

            int FormLen = B2W2 ? 0xB : 0x9;
            int FormDex = PokeDex + 0x8 + brSize*9;
            bit = f + pkm.AltForm;

            // Set Form Seen Flag
            Data[FormDex + FormLen*shiny + bit/8] |= (byte)(1 << (bit%8));

            // Set Displayed Flag if necessary, check all flags
            for (int i = 0; i < fc; i++)
            {
                bit = f + i;
                if ((Data[FormDex + FormLen*2 + bit/8] & (byte)(1 << (bit%8))) != 0) // Nonshiny
                    return; // already set
                if ((Data[FormDex + FormLen*3 + bit/8] & (byte)(1 << (bit%8))) != 0) // Shiny
                    return; // already set
            }
            bit = f + pkm.AltForm;
            Data[FormDex + FormLen * (2 + shiny) + bit / 8] |= (byte)(1 << (bit % 8));
        }
    }
}
