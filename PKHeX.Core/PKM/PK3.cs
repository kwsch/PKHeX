using System;
using System.Linq;

namespace PKHeX.Core
{
    public class PK3 : PKM, IRibbonSet1
    {
        public static readonly byte[] ExtraBytes =
        {
            0x2A, 0x2B
        };
        public sealed override int SIZE_PARTY => PKX.SIZE_3PARTY;
        public override int SIZE_STORED => PKX.SIZE_3STORED;
        public override int Format => 3;
        public override PersonalInfo PersonalInfo => PersonalTable.RS[Species];

        public PK3(byte[] decryptedData = null, string ident = null)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            PKMConverter.checkEncrypted(ref Data);
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
        }
        public override PKM Clone() { return new PK3(Data); }

        public override string getString(int Offset, int Count) => PKX.getString3(Data, Offset, Count, Japanese);
        public override byte[] setString(string value, int maxLength) => PKX.setString3(value, maxLength, Japanese);

        // Trash Bytes
        public override byte[] Nickname_Trash { get => getData(0x08, 10); set { if (value?.Length == 10) value.CopyTo(Data, 0x08); } }
        public override byte[] OT_Trash { get => getData(0x14, 7); set { if (value?.Length == 7) value.CopyTo(Data, 0x14); } }

        // Future Attributes
        public override uint EncryptionConstant { get => PID; set { } }
        public override int Nature { get => (int)(PID % 25); set { } }
        public override int AltForm { get => Species == 201 ? PKX.getUnownForm(PID) : 0; set { } }

        public override bool IsNicknamed { get => PKX.getIsNicknamedAnyLanguage(Species, Nickname, Format); set { } }
        public override int Gender { get => PKX.getGender(Species, PID); set { } }
        public override int Characteristic => -1;
        public override int CurrentFriendship { get => OT_Friendship; set => OT_Friendship = value; }
        public override int Ability { get { int[] abils = PersonalInfo.Abilities; return abils[AbilityBit && abils[1] != 0 ? 1 : 0]; } set { } }
        public override int CurrentHandler { get => 0; set { } }
        public override int Egg_Location { get => 0; set { } }

        // 0x20 Intro
        public override uint PID { get => BitConverter.ToUInt32(Data, 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, 0x00); }
        public override int TID { get => BitConverter.ToUInt16(Data, 0x04); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x04); }
        public override int SID { get => BitConverter.ToUInt16(Data, 0x06); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x06); }
        public override string Nickname { get => getString(0x08, 10); set => setString(IsEgg ? "タマゴ" : value, 10).CopyTo(Data, 0x08); }
        public override int Language { get => BitConverter.ToUInt16(Data, 0x12) & 0xFF; set => BitConverter.GetBytes((ushort)(IsEgg ? 0x601 : value | 0x200)).CopyTo(Data, 0x12); }
        public override string OT_Name { get => getString(0x14, 7); set => setString(value, 7).CopyTo(Data, 0x14); }
        public override int MarkValue { get => Data[0x1B]; protected set => Data[0x1B] = (byte)value; }
        public override ushort Checksum { get => BitConverter.ToUInt16(Data, 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1C); }
        public override ushort Sanity { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1E); }

        #region Block A
        public override int Species { get => PKX.getG4Species(BitConverter.ToUInt16(Data, 0x20)); set => BitConverter.GetBytes((ushort)PKX.getG3Species(value)).CopyTo(Data, 0x20); }
        public override int SpriteItem => PKX.getG4Item((ushort)HeldItem);
        public override int HeldItem { get => BitConverter.ToUInt16(Data, 0x22); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x22); }

        public override uint EXP { get => BitConverter.ToUInt32(Data, 0x24); set => BitConverter.GetBytes(value).CopyTo(Data, 0x24); }
        private byte PPUps { get => Data[0x28]; set => Data[0x28] = value; }
        public override int Move1_PPUps { get => (PPUps >> 0) & 3; set => PPUps = (byte)((PPUps & ~(3 << 0)) | value << 0); }
        public override int Move2_PPUps { get => (PPUps >> 2) & 3; set => PPUps = (byte)((PPUps & ~(3 << 2)) | value << 2); }
        public override int Move3_PPUps { get => (PPUps >> 4) & 3; set => PPUps = (byte)((PPUps & ~(3 << 4)) | value << 4); }
        public override int Move4_PPUps { get => (PPUps >> 6) & 3; set => PPUps = (byte)((PPUps & ~(3 << 6)) | value << 6); }
        public override int OT_Friendship { get => Data[0x29]; set => Data[0x29] = (byte)value; }
        // Unused 0x2A 0x2B
        #endregion

        #region Block B
        public override int Move1 { get => BitConverter.ToUInt16(Data, 0x2C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2C); }
        public override int Move2 { get => BitConverter.ToUInt16(Data, 0x2E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E); }
        public override int Move3 { get => BitConverter.ToUInt16(Data, 0x30); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x30); }
        public override int Move4 { get => BitConverter.ToUInt16(Data, 0x32); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x32); }
        public override int Move1_PP { get => Data[0x34]; set => Data[0x34] = (byte)value; }
        public override int Move2_PP { get => Data[0x35]; set => Data[0x35] = (byte)value; }
        public override int Move3_PP { get => Data[0x36]; set => Data[0x36] = (byte)value; }
        public override int Move4_PP { get => Data[0x37]; set => Data[0x37] = (byte)value; }
        #endregion

        #region Block C
        public override int EV_HP { get => Data[0x38]; set => Data[0x38] = (byte)value; }
        public override int EV_ATK { get => Data[0x39]; set => Data[0x39] = (byte)value; }
        public override int EV_DEF { get => Data[0x3A]; set => Data[0x3A] = (byte)value; }
        public override int EV_SPE { get => Data[0x3B]; set => Data[0x3B] = (byte)value; }
        public override int EV_SPA { get => Data[0x3C]; set => Data[0x3C] = (byte)value; }
        public override int EV_SPD { get => Data[0x3D]; set => Data[0x3D] = (byte)value; }
        public override int CNT_Cool { get => Data[0x3E]; set => Data[0x3E] = (byte)value; }
        public override int CNT_Beauty { get => Data[0x3F]; set => Data[0x3F] = (byte)value; }
        public override int CNT_Cute { get => Data[0x40]; set => Data[0x40] = (byte)value; }
        public override int CNT_Smart { get => Data[0x41]; set => Data[0x41] = (byte)value; }
        public override int CNT_Tough { get => Data[0x42]; set => Data[0x42] = (byte)value; }
        public override int CNT_Sheen { get => Data[0x43]; set => Data[0x43] = (byte)value; }
        #endregion

        #region Block D
        private byte PKRS { get => Data[0x44]; set => Data[0x44] = value; }
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)(PKRS & ~0xF | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)(PKRS & 0xF | value << 4); }
        public override int Met_Location { get => Data[0x45]; set => Data[0x45] = (byte)value; }
        // Origins
        private ushort Origins { get => BitConverter.ToUInt16(Data, 0x46); set => BitConverter.GetBytes(value).CopyTo(Data, 0x46); }
        public override int Met_Level { get => Origins & 0x7F; set => Origins = (ushort)((Origins & ~0x7F) | value); }
        public override int Version { get => (Origins >> 7) & 0xF; set => Origins = (ushort)((Origins & ~0x780) | ((value & 0xF) << 7)); }
        public override int Ball { get => (Origins >> 11) & 0xF; set => Origins = (ushort)((Origins & ~0x7800) | ((value & 0xF) << 11)); }
        public override int OT_Gender { get => (Origins >> 15) & 1; set => Origins = (ushort)(Origins & ~(1 << 15) | ((value & 1) << 15)); }

        public uint IV32 { get => BitConverter.ToUInt32(Data, 0x48); set => BitConverter.GetBytes(value).CopyTo(Data, 0x48); }
        public override int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); }
        public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); }
        public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); }
        public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); }
        public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); }
        public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); }
        public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (uint)((IV32 & ~0x40000000) | (uint)(value ? 0x40000000 : 0)); }
        public bool AbilityBit { get => IV32 >> 31 == 1; set => IV32 = (IV32 & 0x7FFFFFFF) | (uint)(value ? 1 << 31 : 0); }

        private uint RIB0 { get => BitConverter.ToUInt32(Data, 0x4C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x4C); }
        public int RibbonCountG3Cool        { get => (int)(RIB0 >> 00) & 7; set => RIB0 = (uint)((RIB0 & ~(7 << 00)) | (uint)(value & 7) << 00); }
        public int RibbonCountG3Beauty      { get => (int)(RIB0 >> 03) & 7; set => RIB0 = (uint)((RIB0 & ~(7 << 03)) | (uint)(value & 7) << 03); }
        public int RibbonCountG3Cute        { get => (int)(RIB0 >> 06) & 7; set => RIB0 = (uint)((RIB0 & ~(7 << 06)) | (uint)(value & 7) << 06); }
        public int RibbonCountG3Smart       { get => (int)(RIB0 >> 09) & 7; set => RIB0 = (uint)((RIB0 & ~(7 << 09)) | (uint)(value & 7) << 09); }
        public int RibbonCountG3Tough       { get => (int)(RIB0 >> 12) & 7; set => RIB0 = (uint)((RIB0 & ~(7 << 12)) | (uint)(value & 7) << 12); }
        public bool RibbonChampionG3Hoenn   { get => (RIB0 & (1 << 15)) == 1 << 15; set => RIB0 = (uint)(RIB0 & ~(1 << 15) | (uint)(value ? 1 << 15 : 0)); }
        public bool RibbonWinning           { get => (RIB0 & (1 << 16)) == 1 << 16; set => RIB0 = (uint)(RIB0 & ~(1 << 16) | (uint)(value ? 1 << 16 : 0)); }
        public bool RibbonVictory           { get => (RIB0 & (1 << 17)) == 1 << 17; set => RIB0 = (uint)(RIB0 & ~(1 << 17) | (uint)(value ? 1 << 17 : 0)); }
        public bool RibbonArtist            { get => (RIB0 & (1 << 18)) == 1 << 18; set => RIB0 = (uint)(RIB0 & ~(1 << 18) | (uint)(value ? 1 << 18 : 0)); }
        public bool RibbonEffort            { get => (RIB0 & (1 << 19)) == 1 << 19; set => RIB0 = (uint)(RIB0 & ~(1 << 19) | (uint)(value ? 1 << 19 : 0)); }
        public bool RibbonChampionBattle    { get => (RIB0 & (1 << 20)) == 1 << 20; set => RIB0 = (uint)(RIB0 & ~(1 << 20) | (uint)(value ? 1 << 20 : 0)); }
        public bool RibbonChampionRegional  { get => (RIB0 & (1 << 21)) == 1 << 21; set => RIB0 = (uint)(RIB0 & ~(1 << 21) | (uint)(value ? 1 << 21 : 0)); }
        public bool RibbonChampionNational  { get => (RIB0 & (1 << 22)) == 1 << 22; set => RIB0 = (uint)(RIB0 & ~(1 << 22) | (uint)(value ? 1 << 22 : 0)); }
        public bool RibbonCountry           { get => (RIB0 & (1 << 23)) == 1 << 23; set => RIB0 = (uint)(RIB0 & ~(1 << 23) | (uint)(value ? 1 << 23 : 0)); }
        public bool RibbonNational          { get => (RIB0 & (1 << 24)) == 1 << 24; set => RIB0 = (uint)(RIB0 & ~(1 << 24) | (uint)(value ? 1 << 24 : 0)); }
        public bool RibbonEarth             { get => (RIB0 & (1 << 25)) == 1 << 25; set => RIB0 = (uint)(RIB0 & ~(1 << 25) | (uint)(value ? 1 << 25 : 0)); }
        public bool RibbonWorld             { get => (RIB0 & (1 << 26)) == 1 << 26; set => RIB0 = (uint)(RIB0 & ~(1 << 26) | (uint)(value ? 1 << 26 : 0)); }
        public bool Unused1 { get => (RIB0 & (1 << 27)) == 1 << 27; set => RIB0 = (uint)(RIB0 & ~(1 << 27) | (uint)(value ? 1 << 27 : 0)); }
        public bool Unused2 { get => (RIB0 & (1 << 28)) == 1 << 28; set => RIB0 = (uint)(RIB0 & ~(1 << 28) | (uint)(value ? 1 << 28 : 0)); }
        public bool Unused3 { get => (RIB0 & (1 << 29)) == 1 << 29; set => RIB0 = (uint)(RIB0 & ~(1 << 29) | (uint)(value ? 1 << 29 : 0)); }
        public bool Unused4 { get => (RIB0 & (1 << 30)) == 1 << 30; set => RIB0 = (uint)(RIB0 & ~(1 << 30) | (uint)(value ? 1 << 30 : 0)); }
        public override bool FatefulEncounter { get => RIB0 >> 31 == 1; set => RIB0 = (RIB0 & ~(1 << 31)) | (uint)(value ? 1 << 31 : 0); }
        #endregion

        public override int Stat_Level { get => Data[0x54]; set => Data[0x54] = (byte)value; }
        public override int Stat_HPCurrent { get => BitConverter.ToUInt16(Data, 0x56); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x56); }
        public override int Stat_HPMax { get => BitConverter.ToUInt16(Data, 0x58); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x58); }
        public override int Stat_ATK { get => BitConverter.ToUInt16(Data, 0x5A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5A); }
        public override int Stat_DEF { get => BitConverter.ToUInt16(Data, 0x5C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5C); }
        public override int Stat_SPE { get => BitConverter.ToUInt16(Data, 0x5E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5E); }
        public override int Stat_SPA { get => BitConverter.ToUInt16(Data, 0x60); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x60); }
        public override int Stat_SPD { get => BitConverter.ToUInt16(Data, 0x62); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x62); }

        // Generated Attributes
        public override int AbilityNumber { get => 1 << PIDAbility; set => AbilityBit = value > 1; } // 1/2 -> 0/1
        public override int PSV => (int)((PID >> 16 ^ PID & 0xFFFF) >> 3);
        public override int TSV => (TID ^ SID) >> 3;
        public bool Japanese => IsEgg || Language == 1;
        public override bool WasEgg => Met_Level == 0;
        public override bool WasEvent => Met_Location == 255; // Fateful
        public override bool WasIngameTrade => Met_Location == 254; // Trade
        public override bool WasGiftEgg => IsEgg && Met_Location == 253; // Gift Egg, indistinguible from normal eggs after hatch
        public override bool WasEventEgg => IsEgg && Met_Location == 255; // Event Egg, indistinguible from normal eggs after hatch

        public override byte[] Encrypt()
        {
            RefreshChecksum();
            return PKX.encryptArray3(Data);
        }
        public PK4 convertToPK4()
        {
            DateTime moment = DateTime.Now;
            PK4 pk4 = new PK4 // Convert away!
            {
                PID = PID,
                Species = Species,
                TID = TID,
                SID = SID,
                EXP = IsEgg ? PKX.getEXP(5, Species) : EXP,
                IsEgg = false,
                OT_Friendship = 70,
                Markings = Markings,
                Language = Language,
                EV_HP = EV_HP,
                EV_ATK = EV_ATK,
                EV_DEF = EV_DEF,
                EV_SPA = EV_SPA,
                EV_SPD = EV_SPD,
                EV_SPE = EV_SPE,
                CNT_Cool = CNT_Cool,
                CNT_Beauty = CNT_Beauty,
                CNT_Cute = CNT_Cute,
                CNT_Smart = CNT_Smart,
                CNT_Tough = CNT_Tough,
                CNT_Sheen = CNT_Sheen,
                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                Move1_PPUps = Move1_PPUps,
                Move2_PPUps = Move2_PPUps,
                Move3_PPUps = Move3_PPUps,
                Move4_PPUps = Move4_PPUps,
                IV_HP = IV_HP,
                IV_ATK = IV_ATK,
                IV_DEF = IV_DEF,
                IV_SPA = IV_SPA,
                IV_SPD = IV_SPD,
                IV_SPE = IV_SPE,
                Ability = Ability,
                Version = Version,
                Ball = Ball,
                PKRS_Strain = PKRS_Strain,
                PKRS_Days = PKRS_Days,
                OT_Gender = OT_Gender,
                MetDate = moment,
                Met_Location = 0x37, // Pal Park

                RibbonChampionG3Hoenn = RibbonChampionG3Hoenn,
                RibbonWinning     = RibbonWinning,
                RibbonVictory     = RibbonVictory,
                RibbonArtist        = RibbonArtist,
                RibbonEffort = RibbonEffort,
                RibbonChampionBattle = RibbonChampionBattle,
                RibbonChampionRegional = RibbonChampionRegional,
                RibbonChampionNational = RibbonChampionNational,
                RibbonCountry = RibbonCountry,
                RibbonNational = RibbonNational,
                RibbonEarth = RibbonEarth,
                RibbonWorld = RibbonWorld,
            };

            // Fix PP
            pk4.Move1_PP = pk4.getMovePP(pk4.Move1, pk4.Move1_PPUps);
            pk4.Move2_PP = pk4.getMovePP(pk4.Move2, pk4.Move2_PPUps);
            pk4.Move3_PP = pk4.getMovePP(pk4.Move3, pk4.Move3_PPUps);
            pk4.Move4_PP = pk4.getMovePP(pk4.Move4, pk4.Move4_PPUps);

            pk4.FatefulEncounter = FatefulEncounter; // obedience flag

            // Remaining Ribbons
            pk4.RibbonG3Cool          |= RibbonCountG3Cool > 0;
            pk4.RibbonG3CoolSuper     |= RibbonCountG3Cool > 1;
            pk4.RibbonG3CoolHyper     |= RibbonCountG3Cool > 2;
            pk4.RibbonG3CoolMaster    |= RibbonCountG3Cool > 3;
            pk4.RibbonG3Beauty        |= RibbonCountG3Beauty > 0;
            pk4.RibbonG3BeautySuper   |= RibbonCountG3Beauty > 1;
            pk4.RibbonG3BeautyHyper   |= RibbonCountG3Beauty > 2;
            pk4.RibbonG3BeautyMaster  |= RibbonCountG3Beauty > 3;
            pk4.RibbonG3Cute          |= RibbonCountG3Cute > 0;
            pk4.RibbonG3CuteSuper     |= RibbonCountG3Cute > 1;
            pk4.RibbonG3CuteHyper     |= RibbonCountG3Cute > 2;
            pk4.RibbonG3CuteMaster    |= RibbonCountG3Cute > 3;
            pk4.RibbonG3Smart         |= RibbonCountG3Smart > 0;
            pk4.RibbonG3SmartSuper    |= RibbonCountG3Smart > 1;
            pk4.RibbonG3SmartHyper    |= RibbonCountG3Smart > 2;
            pk4.RibbonG3SmartMaster   |= RibbonCountG3Smart > 3;
            pk4.RibbonG3Tough         |= RibbonCountG3Tough > 0;
            pk4.RibbonG3ToughSuper    |= RibbonCountG3Tough > 1;
            pk4.RibbonG3ToughHyper    |= RibbonCountG3Tough > 2;
            pk4.RibbonG3ToughMaster   |= RibbonCountG3Tough > 3;

            // Yay for reusing string buffers!
            PKX.G4TransferTrashBytes[pk4.Language].CopyTo(pk4.Data, 0x48 + 4);
            pk4.Nickname = IsEgg ? PKX.getSpeciesName(pk4.Species, pk4.Language) : Nickname;
            Array.Copy(pk4.Data, 0x48, pk4.Data, 0x68, 0x10);
            pk4.OT_Name = OT_Name;
            
            // Set Final Data
            pk4.Met_Level = PKX.getLevel(pk4.Species, pk4.EXP);
            pk4.Gender = PKX.getGender(pk4.Species, pk4.PID);
            pk4.IsNicknamed = IsNicknamed;

            // Unown Form
            pk4.AltForm = AltForm;

            if (HeldItem > 0)
            {
                ushort item = PKX.getG4Item((ushort)HeldItem);
                if (PKX.isTransferrable34(item))
                    pk4.HeldItem = item;
            }

            // Remove HM moves
            int[] newMoves = pk4.Moves;
            for (int i = 0; i < 4; i++)
                if (Legal.HM_3.Contains(newMoves[i]))
                    newMoves[i] = 0;
            pk4.Moves = newMoves;
            pk4.FixMoves();

            pk4.RefreshChecksum();
            return pk4;
        }
    }
}
