using System;
using System.Linq;

namespace PKHeX
{
    public class PK3 : PKM // 3rd Generation PKM File
    {
        internal static readonly byte[] ExtraBytes =
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

        // Future Attributes
        public override uint EncryptionConstant { get { return PID; } set { } }
        public override int Nature { get { return (int)(PID % 25); } set { } }
        public override int AltForm { get { return Species == 201 ? PKX.getUnownForm(PID) : 0; } set { } }

        public override bool IsNicknamed { get { return PKX.getIsNicknamed(Species, Nickname); } set { } }
        public override int Gender { get { return PKX.getGender(Species, PID); } set { } }
        public override int Characteristic => -1;
        public override int CurrentFriendship { get { return OT_Friendship; } set { OT_Friendship = value; } }
        public override int Ability { get { int[] abils = PersonalInfo.Abilities; return abils[abils[1] == 0 ? 0 : AbilityNumber]; } set { } }
        public override int CurrentHandler { get { return 0; } set { } }
        public override int Egg_Location { get { return 0; } set { } }

        // 0x20 Intro
        public override uint PID { get { return BitConverter.ToUInt32(Data, 0x00); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x00); } }
        public override int TID { get { return BitConverter.ToUInt16(Data, 0x04); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x04); } }
        public override int SID { get { return BitConverter.ToUInt16(Data, 0x06); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x06); } }
        public override string Nickname { 
            get { return PKX.getG3Str(Data.Skip(0x08).Take(10).ToArray(), Japanese); } 
            set { byte[] strdata = PKX.setG3Str(value, Japanese);
                if (strdata.Length > 10) 
                    Array.Resize(ref strdata, 10);
                strdata.CopyTo(Data, 0x08); } }
        public override int Language { get { return BitConverter.ToUInt16(Data, 0x12) & 0xFF; } set { BitConverter.GetBytes((ushort)(value | 0x200)).CopyTo(Data, 0x12); } }
        public override string OT_Name { 
            get { return PKX.getG3Str(Data.Skip(0x14).Take(7).ToArray(), Japanese); } 
            set { byte[] strdata = PKX.setG3Str(value, Japanese);
                if (strdata.Length > 7) 
                    Array.Resize(ref strdata, 7);
                strdata.CopyTo(Data, 0x14); } }

        public override int MarkValue { get { return Data[0x1B]; } protected set { Data[0x1B] = (byte)value; } }
        public override ushort Checksum { get { return BitConverter.ToUInt16(Data, 0x1C); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1C); } }
        public override ushort Sanity { get { return BitConverter.ToUInt16(Data, 0x1E); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1E); } }

        #region Block A
        public override int Species { get { return PKX.getG4Species(BitConverter.ToUInt16(Data, 0x20)); } set { BitConverter.GetBytes((ushort)PKX.getG3Species(value)).CopyTo(Data, 0x20); } }
        public override int SpriteItem => PKX.getG4Item((ushort)HeldItem);
        public override int HeldItem { get { return BitConverter.ToUInt16(Data, 0x22); } set { BitConverter.GetBytes((ushort) value).CopyTo(Data, 0x22); } }

        public override uint EXP { get { return BitConverter.ToUInt32(Data, 0x24); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x24); } }
        private byte PPUps { get { return Data[0x28]; } set { Data[0x28] = value; } }
        public override int Move1_PPUps { get { return (PPUps >> 0) & 3; } set { PPUps = (byte)((PPUps & ~(3 << 0)) | value << 0); } }
        public override int Move2_PPUps { get { return (PPUps >> 2) & 3; } set { PPUps = (byte)((PPUps & ~(3 << 2)) | value << 2); } }
        public override int Move3_PPUps { get { return (PPUps >> 4) & 3; } set { PPUps = (byte)((PPUps & ~(3 << 4)) | value << 4); } }
        public override int Move4_PPUps { get { return (PPUps >> 6) & 3; } set { PPUps = (byte)((PPUps & ~(3 << 6)) | value << 6); } }
        public override int OT_Friendship { get { return Data[0x29]; } set { Data[0x29] = (byte)value; } }
        // Unused 0x2A 0x2B
        #endregion

        #region Block B
        public override int Move1 { get { return BitConverter.ToUInt16(Data, 0x2C); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2C); } }
        public override int Move2 { get { return BitConverter.ToUInt16(Data, 0x2E); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E); } }
        public override int Move3 { get { return BitConverter.ToUInt16(Data, 0x30); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x30); } }
        public override int Move4 { get { return BitConverter.ToUInt16(Data, 0x32); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x32); } }
        public override int Move1_PP { get { return Data[0x34]; } set { Data[0x34] = (byte)value; } }
        public override int Move2_PP { get { return Data[0x35]; } set { Data[0x35] = (byte)value; } }
        public override int Move3_PP { get { return Data[0x36]; } set { Data[0x36] = (byte)value; } }
        public override int Move4_PP { get { return Data[0x37]; } set { Data[0x37] = (byte)value; } }
        #endregion

        #region Block C
        public override int EV_HP { get { return Data[0x38]; } set { Data[0x38] = (byte)value; } }
        public override int EV_ATK { get { return Data[0x39]; } set { Data[0x39] = (byte)value; } }
        public override int EV_DEF { get { return Data[0x3A]; } set { Data[0x3A] = (byte)value; } }
        public override int EV_SPE { get { return Data[0x3B]; } set { Data[0x3B] = (byte)value; } }
        public override int EV_SPA { get { return Data[0x3C]; } set { Data[0x3C] = (byte)value; } }
        public override int EV_SPD { get { return Data[0x3D]; } set { Data[0x3D] = (byte)value; } }
        public override int CNT_Cool { get { return Data[0x3E]; } set { Data[0x3E] = (byte)value; } }
        public override int CNT_Beauty { get { return Data[0x3F]; } set { Data[0x3F] = (byte)value; } }
        public override int CNT_Cute { get { return Data[0x40]; } set { Data[0x40] = (byte)value; } }
        public override int CNT_Smart { get { return Data[0x41]; } set { Data[0x41] = (byte)value; } }
        public override int CNT_Tough { get { return Data[0x42]; } set { Data[0x42] = (byte)value; } }
        public override int CNT_Sheen { get { return Data[0x43]; } set { Data[0x43] = (byte)value; } }
        #endregion

        #region Block D
        private byte PKRS { get { return Data[0x44]; } set { Data[0x44] = value; } }
        public override int PKRS_Days { get { return PKRS & 0xF; } set { PKRS = (byte)(PKRS & ~0xF | value); } }
        public override int PKRS_Strain { get { return PKRS >> 4; } set { PKRS = (byte)(PKRS & 0xF | value << 4); } }
        public override int Met_Location { get { return Data[0x45]; } set { Data[0x45] = (byte)value; } }
        // Origins
        private ushort Origins { get { return BitConverter.ToUInt16(Data, 0x46); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x46); } }
        public override int Met_Level { get { return Origins & 0x7F; } set { Origins = (ushort)((Origins & ~0x7F) | value); } }
        public override int Version { get { return (Origins >> 7) & 0xF; } set { Origins = (ushort)((Origins & ~0x780) | ((value & 0xF) << 7));} }
        public override int Ball { get { return (Origins >> 11) & 0xF; } set { Origins = (ushort)((Origins & ~0x7800) | ((value & 0xF) << 11)); } }
        public override int OT_Gender { get { return (Origins >> 15) & 1; } set { Origins = (ushort)(Origins & ~(1 << 15) | ((value & 1) << 15)); } }

        public uint IV32 { get { return BitConverter.ToUInt32(Data, 0x48); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x48); } }
        public override int IV_HP { get { return (int)(IV32 >> 00) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); } }
        public override int IV_ATK { get { return (int)(IV32 >> 05) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); } }
        public override int IV_DEF { get { return (int)(IV32 >> 10) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); } }
        public override int IV_SPE { get { return (int)(IV32 >> 15) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); } }
        public override int IV_SPA { get { return (int)(IV32 >> 20) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); } }
        public override int IV_SPD { get { return (int)(IV32 >> 25) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); } }
        public override bool IsEgg { get { return ((IV32 >> 30) & 1) == 1; } set { IV32 = (uint)((IV32 & ~0x40000000) | (uint)(value ? 0x40000000 : 0)); } }
        public override int AbilityNumber { get { return (int)((IV32 >> 31) & 1); } set { IV32 = (IV32 & 0x7FFFFFFF) | (value == 1 ? 0x80000000 : 0); } }

        private uint RIB0 { get { return BitConverter.ToUInt32(Data, 0x4C); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x4C); } }
        public int RibbonCountG3Cool        { get { return (int)(RIB0 >> 00) & 7; } set { RIB0 = (uint)((RIB0 & ~(7 << 00)) | (uint)(value & 7) << 00); } }
        public int RibbonCountG3Beauty      { get { return (int)(RIB0 >> 03) & 7; } set { RIB0 = (uint)((RIB0 & ~(7 << 03)) | (uint)(value & 7) << 03); } }
        public int RibbonCountG3Cute        { get { return (int)(RIB0 >> 06) & 7; } set { RIB0 = (uint)((RIB0 & ~(7 << 06)) | (uint)(value & 7) << 06); } }
        public int RibbonCountG3Smart       { get { return (int)(RIB0 >> 09) & 7; } set { RIB0 = (uint)((RIB0 & ~(7 << 09)) | (uint)(value & 7) << 09); } }
        public int RibbonCountG3Tough       { get { return (int)(RIB0 >> 12) & 7; } set { RIB0 = (uint)((RIB0 & ~(7 << 12)) | (uint)(value & 7) << 12); } }
        public bool RibbonChampionG3Hoenn   { get { return (RIB0 & (1 << 15)) == 1 << 15; } set { RIB0 = (uint)(RIB0 & ~(1 << 15) | (uint)(value ? 1 << 15 : 0)); } }
        public bool RibbonWinning           { get { return (RIB0 & (1 << 16)) == 1 << 16; } set { RIB0 = (uint)(RIB0 & ~(1 << 16) | (uint)(value ? 1 << 16 : 0)); } }
        public bool RibbonVictory           { get { return (RIB0 & (1 << 17)) == 1 << 17; } set { RIB0 = (uint)(RIB0 & ~(1 << 17) | (uint)(value ? 1 << 17 : 0)); } }
        public bool RibbonArtist            { get { return (RIB0 & (1 << 18)) == 1 << 18; } set { RIB0 = (uint)(RIB0 & ~(1 << 18) | (uint)(value ? 1 << 18 : 0)); } }
        public bool RibbonEffort            { get { return (RIB0 & (1 << 19)) == 1 << 19; } set { RIB0 = (uint)(RIB0 & ~(1 << 19) | (uint)(value ? 1 << 19 : 0)); } }
        public bool RibbonChampionBattle    { get { return (RIB0 & (1 << 20)) == 1 << 20; } set { RIB0 = (uint)(RIB0 & ~(1 << 20) | (uint)(value ? 1 << 20 : 0)); } }
        public bool RibbonChampionRegional  { get { return (RIB0 & (1 << 21)) == 1 << 21; } set { RIB0 = (uint)(RIB0 & ~(1 << 21) | (uint)(value ? 1 << 21 : 0)); } }
        public bool RibbonChampionNational  { get { return (RIB0 & (1 << 22)) == 1 << 22; } set { RIB0 = (uint)(RIB0 & ~(1 << 22) | (uint)(value ? 1 << 22 : 0)); } }
        public bool RibbonCountry           { get { return (RIB0 & (1 << 23)) == 1 << 23; } set { RIB0 = (uint)(RIB0 & ~(1 << 23) | (uint)(value ? 1 << 23 : 0)); } }
        public bool RibbonNational          { get { return (RIB0 & (1 << 24)) == 1 << 24; } set { RIB0 = (uint)(RIB0 & ~(1 << 24) | (uint)(value ? 1 << 24 : 0)); } }
        public bool RibbonEarth             { get { return (RIB0 & (1 << 25)) == 1 << 25; } set { RIB0 = (uint)(RIB0 & ~(1 << 25) | (uint)(value ? 1 << 25 : 0)); } }
        public bool RibbonWorld             { get { return (RIB0 & (1 << 26)) == 1 << 26; } set { RIB0 = (uint)(RIB0 & ~(1 << 26) | (uint)(value ? 1 << 26 : 0)); } }
        public bool Unused1 { get { return (RIB0 & (1 << 27)) == 1 << 27; } set { RIB0 = (uint)(RIB0 & ~(1 << 27) | (uint)(value ? 1 << 27 : 0)); } }
        public bool Unused2 { get { return (RIB0 & (1 << 28)) == 1 << 28; } set { RIB0 = (uint)(RIB0 & ~(1 << 28) | (uint)(value ? 1 << 28 : 0)); } }
        public bool Unused3 { get { return (RIB0 & (1 << 29)) == 1 << 29; } set { RIB0 = (uint)(RIB0 & ~(1 << 29) | (uint)(value ? 1 << 29 : 0)); } }
        public bool Unused4 { get { return (RIB0 & (1 << 30)) == 1 << 30; } set { RIB0 = (uint)(RIB0 & ~(1 << 30) | (uint)(value ? 1 << 30 : 0)); } }
        public override bool FatefulEncounter { get { return RIB0 >> 31 == 1; } set { RIB0 = (RIB0 & ~(1 << 31)) | (uint)(value ? 1 << 31 : 0); } }
        #endregion

        public override int Stat_Level { get { return Data[0x54]; } set { Data[0x54] = (byte)value; } }
        public override int Stat_HPCurrent { get { return BitConverter.ToUInt16(Data, 0x56); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x56); } }
        public override int Stat_HPMax { get { return BitConverter.ToUInt16(Data, 0x58); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x58); } }
        public override int Stat_ATK { get { return BitConverter.ToUInt16(Data, 0x5A); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5A); } }
        public override int Stat_DEF { get { return BitConverter.ToUInt16(Data, 0x5C); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5C); } }
        public override int Stat_SPE { get { return BitConverter.ToUInt16(Data, 0x5E); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x5E); } }
        public override int Stat_SPA { get { return BitConverter.ToUInt16(Data, 0x60); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x60); } }
        public override int Stat_SPD { get { return BitConverter.ToUInt16(Data, 0x62); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x62); } }

        // Generated Attributes
        public override int PSV => (int)((PID >> 16 ^ PID & 0xFFFF) >> 3);
        public override int TSV => (TID ^ SID) >> 3;
        public bool Japanese => Language == 1;

        public override byte[] Encrypt()
        {
            return PKX.encryptArray3(Data);
        }
        public override bool getGenderIsValid()
        {
            int gv = PersonalInfo.Gender;

            if (gv == 255)
                return Gender == 2;
            if (gv == 254)
                return Gender == 1;
            if (gv == 0)
                return Gender == 0;
            if ((PID & 0xFF) <= gv)
                return Gender == 1;
            if (gv < (PID & 0xFF))
                return Gender == 0;

            return false;
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
                MarkCircle = MarkCircle,
                MarkSquare = MarkSquare,
                MarkTriangle = MarkTriangle,
                MarkHeart = MarkHeart,
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

            pk4.FatefulEncounter = Met_Location == 0xFF || FatefulEncounter; // obedience flag

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

            int item = HeldItem;
            if (HeldItem > 0)
                pk4.HeldItem = item;

            // Remove HM moves
            int[] banned = { 15, 19, 57, 70, 148, 249, 127, 291 };
            int[] newMoves = pk4.Moves;
            for (int i = 0; i < 4; i++)
                if (banned.Contains(newMoves[i]))
                    newMoves[i] = 0;
            pk4.Moves = newMoves;
            pk4.FixMoves();

            pk4.RefreshChecksum();
            return pk4;
        }
    }
}
