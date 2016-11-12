using System;

namespace PKHeX
{
    public class CK3 : PKM // 3rd Generation PKM File
    {
        internal static readonly byte[] ExtraBytes =
        {
            0x11, 0x12, 0x13,
            0x61, 0x62, 0x63, 0x64,
            0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xDA, 0xDB,
            0xE4, 0xE5, 0xE6, 0xE7, 0xCE,
            // 0xFC onwards unused?
        };
        public sealed override int SIZE_PARTY => PKX.SIZE_3CSTORED;
        public override int SIZE_STORED => PKX.SIZE_3CSTORED;
        public override int Format => 3;
        public override PersonalInfo PersonalInfo => PersonalTable.RS[Species];

        public CK3(byte[] decryptedData = null, string ident = null)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            PKMConverter.checkEncrypted(ref Data);
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
        }
        public override PKM Clone() { return new CK3(Data); }

        // Future Attributes
        public override uint EncryptionConstant { get { return PID; } set { } }
        public override int Nature { get { return (int)(PID % 25); } set { } }
        public override int AltForm { get { return Species == 201 ? PKX.getUnownForm(PID) : 0; } set { } }

        public override bool IsNicknamed { get { return PKX.getIsNicknamed(Species, Nickname); } set { } }
        public override int Gender { get { return PKX.getGender(Species, PID); } set { } }
        public override int Characteristic => -1;
        public override int CurrentFriendship { get { return OT_Friendship; } set { OT_Friendship = value; } }
        public override int Ability { get { int[] abils = PersonalTable.RS.getAbilities(Species, 0); return abils[abils[1] == 0 ? 0 : AbilityNumber]; } set { } }
        public override int CurrentHandler { get { return 0; } set { } }
        public override int Egg_Location { get { return 0; } set { } }

        // Silly Attributes
        public override ushort Sanity { get { return 0; } set { } } // valid flag set in pkm structure.
        public override ushort Checksum { get { return SaveUtil.ccitt16(Data); } set { } } // totally false, just a way to get a 'random' ident for the pkm.

        public override int Species { get { return PKX.getG4Species(BigEndian.ToUInt16(Data, 0x00)); } set { BigEndian.GetBytes((ushort)PKX.getG3Species(value)).CopyTo(Data, 0x00); } }
        // 02-04 unused
        public override uint PID { get { return BigEndian.ToUInt32(Data, 0x04); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x04); } }
        public override int Version { get { return SaveUtil.getG3VersionID(Data[0x08]); } set { Data[0x08] = (byte)SaveUtil.getCXDVersionID(value); } }
        public int CurrentRegion { get { return Data[0x09]; } set { Data[0x09] = (byte)value; } }
        public int OriginalRegion { get { return Data[0x0A]; } set { Data[0x0A] = (byte)value; } }
        public override int Language { get { return Data[0x0B]; } set { Data[0x0B] = (byte)value; } }
        public override int Met_Location { get { return BigEndian.ToUInt16(Data, 0x0C); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x0C);} }
        public override int Met_Level { get { return Data[0x0E]; } set { Data[0x0E] = (byte)value; } }
        public override int Ball { get { return Data[0x0F]; } set { Data[0x0F] = (byte)value; } }
        public override int OT_Gender { get { return Data[0x10]; } set { Data[0x10] = (byte)value; } }
        public override int SID { get { return BigEndian.ToUInt16(Data, 0x14); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x14); } }
        public override int TID { get { return BigEndian.ToUInt16(Data, 0x16); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x16); } }
        public override string OT_Name { get { return PKX.getColoStr(Data, 0x18, 10); } set { PKX.setColoStr(value, 10).CopyTo(Data, 0x18); } } // +2 terminator
        public override string Nickname { get { return PKX.getColoStr(Data, 0x2E, 10); } set { PKX.setColoStr(value, 10).CopyTo(Data, 0x2E); Nickname2 = value; } } // +2 terminator
        private string Nickname2 { get { return PKX.getColoStr(Data, 0x44, 10); } set { PKX.setColoStr(value, 10).CopyTo(Data, 0x44); } } // +2 terminator
        public override uint EXP { get { return BigEndian.ToUInt32(Data, 0x5C); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x5C);} }
        public override int Stat_Level { get { return Data[0x60]; } set { Data[0x60] = (byte)value; } }

        // 0x64-0x77 are battle/status related
        // Not that the program cares

        // Moves
        public override int Move1 { get { return BigEndian.ToUInt16(Data, 0x78); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x78); } }
        public override int Move1_PP { get { return Data[0x7A]; } set { Data[0x7A] = (byte)value; } }
        public override int Move1_PPUps { get { return Data[0x7B]; } set { Data[0x7B] = (byte)value; } }
        public override int Move2 { get { return BigEndian.ToUInt16(Data, 0x7C); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x7C); } }
        public override int Move2_PP { get { return Data[0x7E]; } set { Data[0x7E] = (byte)value; } }
        public override int Move2_PPUps { get { return Data[0x7F]; } set { Data[0x7F] = (byte)value; } }
        public override int Move3 { get { return BigEndian.ToUInt16(Data, 0x80); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x80); } }
        public override int Move3_PP { get { return Data[0x82]; } set { Data[0x82] = (byte)value; } }
        public override int Move3_PPUps { get { return Data[0x83]; } set { Data[0x83] = (byte)value; } }
        public override int Move4 { get { return BigEndian.ToUInt16(Data, 0x84); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x84); } }
        public override int Move4_PP { get { return Data[0x86]; } set { Data[0x86] = (byte)value; } }
        public override int Move4_PPUps { get { return Data[0x87]; } set { Data[0x87] = (byte)value; } }

        public override int SpriteItem => PKX.getG4Item((ushort)HeldItem);
        public override int HeldItem { get { return BigEndian.ToUInt16(Data, 0x88); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x88); } }

        // More party stats
        public override int Stat_HPCurrent { get { return BigEndian.ToUInt16(Data, 0x8A); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x8A); } }
        public override int Stat_HPMax { get { return BigEndian.ToUInt16(Data, 0x8C); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x8C); } }
        public override int Stat_ATK { get { return BigEndian.ToUInt16(Data, 0x8E); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x8E); } }
        public override int Stat_DEF { get { return BigEndian.ToUInt16(Data, 0x90); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x90); } }
        public override int Stat_SPA { get { return BigEndian.ToUInt16(Data, 0x92); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x92); } }
        public override int Stat_SPD { get { return BigEndian.ToUInt16(Data, 0x94); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x94); } }
        public override int Stat_SPE { get { return BigEndian.ToUInt16(Data, 0x96); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x96); } }
        
        // EVs
        public override int EV_HP { 
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0x98)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0x98); } }
        public override int EV_ATK {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0x9A)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0x9A); } }
        public override int EV_DEF {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0x9C)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0x9C); } }
        public override int EV_SPA {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0x9E)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0x9E); } }
        public override int EV_SPD {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA0)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA0); } }
        public override int EV_SPE {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA2)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA2); } }

        // IVs
        public override int IV_HP {
            get { return Math.Min((ushort)31, BigEndian.ToUInt16(Data, 0xA4)); }
            set { BigEndian.GetBytes((ushort)(value & 0x1F)).CopyTo(Data, 0xA4); } }
        public override int IV_ATK {
            get { return Math.Min((ushort)31, BigEndian.ToUInt16(Data, 0xA6)); }
            set { BigEndian.GetBytes((ushort)(value & 0x1F)).CopyTo(Data, 0xA6); } }
        public override int IV_DEF {
            get { return Math.Min((ushort)31, BigEndian.ToUInt16(Data, 0xA8)); }
            set { BigEndian.GetBytes((ushort)(value & 0x1F)).CopyTo(Data, 0xA8); } }
        public override int IV_SPA {
            get { return Math.Min((ushort)31, BigEndian.ToUInt16(Data, 0xAA)); }
            set { BigEndian.GetBytes((ushort)(value & 0x1F)).CopyTo(Data, 0xAA); } }
        public override int IV_SPD {
            get { return Math.Min((ushort)31, BigEndian.ToUInt16(Data, 0xAC)); }
            set { BigEndian.GetBytes((ushort)(value & 0x1F)).CopyTo(Data, 0xAC); } }
        public override int IV_SPE {
            get { return Math.Min((ushort)31, BigEndian.ToUInt16(Data, 0xAE)); }
            set { BigEndian.GetBytes((ushort)(value & 0x1F)).CopyTo(Data, 0xAE); } }
        
        public override int OT_Friendship { get { return Data[0xB0]; } set { Data[0xB0] = (byte)value; } }

        // Contest
        public override int CNT_Cool { get { return Data[0xB2]; } set { Data[0xB2] = (byte)value; } }
        public override int CNT_Beauty { get { return Data[0xB3]; } set { Data[0xB3] = (byte)value; } }
        public override int CNT_Cute { get { return Data[0xB4]; } set { Data[0xB4] = (byte)value; } }
        public override int CNT_Smart { get { return Data[0xB5]; } set { Data[0xB5] = (byte)value; } }
        public override int CNT_Tough { get { return Data[0xB6]; } set { Data[0xB6] = (byte)value; } }
        public int RibbonCountG3Cool { get { return Data[0xB7]; } set { Data[0xB7] = (byte)value; } }
        public int RibbonCountG3Beauty { get { return Data[0xB8]; } set { Data[0xB8] = (byte)value; } }
        public int RibbonCountG3Cute { get { return Data[0xB9]; } set { Data[0xB9] = (byte)value; } }
        public int RibbonCountG3Smart { get { return Data[0xBA]; } set { Data[0xBA] = (byte)value; } }
        public int RibbonCountG3Tough { get { return Data[0xBB]; } set { Data[0xBB] = (byte)value; } }
        public override int CNT_Sheen { get { return Data[0xBC]; } set { Data[0xBC] = (byte)value; } }
        
        // Ribbons
        public bool RibbonChampionG3Hoenn { get { return Data[0xBD] == 1; } set { Data[0xBD] = (byte)(value ? 1 : 0); } }
        public bool RibbonWinning { get { return Data[0xBE] == 1; } set { Data[0xBE] = (byte)(value ? 1 : 0); } }
        public bool RibbonVictory { get { return Data[0xBF] == 1; } set { Data[0xBF] = (byte)(value ? 1 : 0); } }
        public bool RibbonArtist { get { return Data[0xC0] == 1; } set { Data[0xC0] = (byte)(value ? 1 : 0); } }
        public bool RibbonEffort { get { return Data[0xC1] == 1; } set { Data[0xC1] = (byte)(value ? 1 : 0); } }
        public bool RibbonChampionBattle { get { return Data[0xC2] == 1; } set { Data[0xC2] = (byte)(value ? 1 : 0); } }
        public bool RibbonChampionRegional { get { return Data[0xC3] == 1; } set { Data[0xC3] = (byte)(value ? 1 : 0); } }
        public bool RibbonChampionNational { get { return Data[0xC4] == 1; } set { Data[0xC4] = (byte)(value ? 1 : 0); } }
        public bool RibbonCountry { get { return Data[0xC5] == 1; } set { Data[0xC5] = (byte)(value ? 1 : 0); } }
        public bool RibbonNational { get { return Data[0xC6] == 1; } set { Data[0xC6] = (byte)(value ? 1 : 0); } }
        public bool RibbonEarth { get { return Data[0xC7] == 1; } set { Data[0xC7] = (byte)(value ? 1 : 0); } }
        public bool RibbonWorld { get { return Data[0xC8] == 1; } set { Data[0xC8] = (byte)(value ? 1 : 0); } }
        public bool Unused1 { get { return ((Data[0xC9]>>0) & 1) == 1; } set { Data[0xC9] = (byte)(Data[0xC9] & ~1 | (value ? 1 : 0)); } }
        public bool Unused2 { get { return ((Data[0xC9]>>1) & 1) == 1; } set { Data[0xC9] = (byte)(Data[0xC9] & ~2 | (value ? 2 : 0)); } }
        public bool Unused3 { get { return ((Data[0xC9]>>2) & 1) == 1; } set { Data[0xC9] = (byte)(Data[0xC9] & ~4 | (value ? 4 : 0)); } }
        public bool Unused4 { get { return ((Data[0xC9]>>3) & 1) == 1; } set { Data[0xC9] = (byte)(Data[0xC9] & ~8 | (value ? 8 : 0)); } }

        public override int PKRS_Strain { get { return Data[0xCA] & 0xF; } set { Data[0xCA] = (byte)(value & 0xF); } }
        public override bool IsEgg { get { return Data[0xCB] == 1; } set { Data[0xCB] = (byte)(value ? 1 : 0); } }
        public override int AbilityNumber { get { return Data[0xCC]; } set { Data[0xCC] = (byte)(value & 1); } }
        public override bool Valid { get { return Data[0xCD] == 0; } set { if (value) Data[0xCD] = 0; } }
        // 0xCE unknown
        public override int MarkValue { get { return Data[0xCF]; } protected set { Data[0xCF] = (byte)value; } }
        public override int PKRS_Days { get { return Math.Max((sbyte)Data[0xD0], (sbyte)0); } set { Data[0xD0] = (byte)(value == 0 ? 0xFF : value & 0xF); } }
        public int ShadowID { get { return BigEndian.ToUInt16(Data, 0xD8); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xD8); } }
        public int Purification { get { return BigEndian.ToInt32(Data, 0xDC); } set { BigEndian.GetBytes(value).CopyTo(Data, 0xDC); } }
        public uint EXP_Shadow { get { return BigEndian.ToUInt32(Data, 0xC0); } set { BigEndian.GetBytes(value).CopyTo(Data, 0xC0); } }
        public override bool FatefulEncounter { get { return Data[0x11C] == 1; } set { Data[0x11C] = (byte)(value ? 1 : 0); } }
        public new int EncounterType { get { return Data[0xFB]; } set { Data[0xFB] = (byte)value; } }

        // Generated Attributes
        public override int PSV => (int)((PID >> 16 ^ PID & 0xFFFF) >> 3);
        public override int TSV => (TID ^ SID) >> 3;
        public bool Japanese => Language == 1;

        public override byte[] Encrypt()
        {
            return (byte[])Data.Clone();
        }
        public override bool getGenderIsValid()
        {
            int gv = PersonalTable.RS[Species].Gender;

            if (gv == 255)
                return Gender == 2;
            if (gv == 254)
                return Gender == 0;
            if (gv == 0)
                return Gender == 1;
            if (gv <= (PID & 0xFF))
                return Gender == 0;
            if ((PID & 0xFF) < gv)
                return Gender == 1;

            return false;
        }
    }
}
