using System;
using System.Linq;

namespace PKHeX
{
    public class XK3 : PKM // 3rd Generation PKM File
    {
        internal static readonly byte[] ExtraBytes =
        {
            0x0A, 0x0B, 0x0C, 0x0D, 0x1E, 0x1F,
            0x2A, 0x2B,
            0x7A, 0x7B,
            0x7E, 0x7F
        };
        public sealed override int SIZE_PARTY => PKX.SIZE_3XSTORED;
        public override int SIZE_STORED => PKX.SIZE_3XSTORED;
        public override int Format => 3;
        public override PersonalInfo PersonalInfo => PersonalTable.RS[Species];
        public XK3(byte[] decryptedData = null, string ident = null)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            PKMConverter.checkEncrypted(ref Data);
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
        }
        public override PKM Clone() { return new XK3(Data) {Purification = Purification}; }

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
        public override int SpriteItem => PKX.getG4Item((ushort)HeldItem);
        public override int HeldItem { get { return BigEndian.ToUInt16(Data, 0x02); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x02); } }
        public override int Stat_HPCurrent { get { return BigEndian.ToUInt16(Data, 0x04); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x04); } }
        public override int OT_Friendship { get { return Data[0x06]; } set { Data[0x06] = (byte)value; } }
        public override int Met_Location { get { return BigEndian.ToUInt16(Data, 0x08); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x08); } }
        // 0x0A-0x0B Unknown
        // 0x0C-0x0D Unknown
        public override int Met_Level { get { return Data[0x0E]; } set { Data[0x0E] = (byte)value; } }
        public override int Ball { get { return Data[0x0F]; } set { Data[0x0F] = (byte)value; } }
        public override int OT_Gender { get { return Data[0x10]; } set { Data[0x10] = (byte)value; } }
        public override int Stat_Level { get { return Data[0x11]; } set { Data[0x11] = (byte)value; } }
        public override int CNT_Sheen { get { return Data[0x12]; } set { Data[0x12] = (byte)value; } }
        public override int PKRS_Strain { get { return Data[0x13] & 0xF; } set { Data[0x13] = (byte)(value & 0xF); } }
        public override int MarkValue { get { return Data[0x14]; } protected set { Data[0x14] = (byte)value; } }
        public override int PKRS_Days { get { return Math.Max((sbyte)Data[0x15], (sbyte)0); } set { Data[0x15] = (byte)(value == 0 ? 0xFF : value & 0xF); } }
        // 0x16-0x1C Battle Related
        private int XDPKMFLAGS { get { return Data[0x1D]; } set { Data[0x1D] = (byte)value; } }
        public bool UnusedFlag0     { get { return (XDPKMFLAGS & (1 << 0)) == 1 << 0; } set { XDPKMFLAGS = XDPKMFLAGS & ~(1 << 0) | (value ? 1 << 0 : 0); } }
        public bool UnusedFlag1     { get { return (XDPKMFLAGS & (1 << 1)) == 1 << 1; } set { XDPKMFLAGS = XDPKMFLAGS & ~(1 << 1) | (value ? 1 << 1 : 0); } }
        public bool CapturedFlag    { get { return (XDPKMFLAGS & (1 << 2)) == 1 << 2; } set { XDPKMFLAGS = XDPKMFLAGS & ~(1 << 2) | (value ? 1 << 2 : 0); } }
        public bool UnusedFlag3     { get { return (XDPKMFLAGS & (1 << 3)) == 1 << 3; } set { XDPKMFLAGS = XDPKMFLAGS & ~(1 << 3) | (value ? 1 << 3 : 0); } }
        public bool BlockTrades     { get { return (XDPKMFLAGS & (1 << 4)) == 1 << 4; } set { XDPKMFLAGS = XDPKMFLAGS & ~(1 << 4) | (value ? 1 << 4 : 0); } }
        public override bool Valid  { get { return (XDPKMFLAGS & (1 << 5)) == 0; }      set { XDPKMFLAGS = XDPKMFLAGS & ~(1 << 5) | (value ? 0 : 1 << 5); } } // invalid flag
        public override int AbilityNumber    { get { return (XDPKMFLAGS >> 6) & 1; }             set { XDPKMFLAGS = XDPKMFLAGS & ~(1 << 6) | (value << 6); } }
        public override bool IsEgg  { get { return (XDPKMFLAGS & (1 << 7)) == 1 << 7; } set { XDPKMFLAGS = XDPKMFLAGS & ~(1 << 7) | (value ? 1 << 7 : 0); } }
        // 0x1E-0x1F Unknown
        public override uint EXP { get { return BigEndian.ToUInt32(Data, 0x20); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x20); } }
        public override int SID { get { return BigEndian.ToUInt16(Data, 0x24); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x24); } }
        public override int TID { get { return BigEndian.ToUInt16(Data, 0x26); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x26); } }
        public override uint PID { get { return BigEndian.ToUInt32(Data, 0x28); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x28); } }
        // 0x2A-0x2B Unknown
        // 0x2C-0x2F Battle Related
        public override bool FatefulEncounter { get { return Data[0x30] == 1; } set { Data[0x30] = (byte)(value ? 1 : 0); } }
        // 0x31-0x32 Unknown
        public new int EncounterType { get { return Data[0x33]; } set { Data[0x33] = (byte)value; } }
        public override int Version { get { return SaveUtil.getG3VersionID(Data[0x34]); } set { Data[0x34] = (byte)SaveUtil.getCXDVersionID(value); } }
        public int CurrentRegion { get { return Data[0x35]; } set { Data[0x35] = (byte)value; } }
        public int OriginalRegion { get { return Data[0x36]; } set { Data[0x36] = (byte)value; } }
        public override int Language { get { return Data[0x37]; } set { Data[0x37] = (byte)value; } }
        public override string OT_Name { get { return PKX.getColoStr(Data, 0x38, 10); } set { PKX.setColoStr(value, 10).CopyTo(Data, 0x38); } } // +2 terminator
        public override string Nickname { get { return PKX.getColoStr(Data, 0x4E, 10); } set { PKX.setColoStr(value, 10).CopyTo(Data, 0x4E); Nickname2 = value; } } // +2 terminator
        private string Nickname2 { get { return PKX.getColoStr(Data, 0x64, 10); } set { PKX.setColoStr(value, 10).CopyTo(Data, 0x64); } } // +2 terminator
        // 0x7A-0x7B Unknown
        private ushort RIB0 { get { return BigEndian.ToUInt16(Data, 0x7C); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x7C); } }
        public bool RibbonChampionG3Hoenn   { get { return (RIB0 & (1 << 15)) == 1 << 15; } set { RIB0 = (ushort)(RIB0 & ~(1 << 15) | (ushort)(value ? 1 << 15 : 0)); } }
        public bool RibbonWinning           { get { return (RIB0 & (1 << 14)) == 1 << 14; } set { RIB0 = (ushort)(RIB0 & ~(1 << 14) | (ushort)(value ? 1 << 14 : 0)); } }
        public bool RibbonVictory           { get { return (RIB0 & (1 << 13)) == 1 << 13; } set { RIB0 = (ushort)(RIB0 & ~(1 << 13) | (ushort)(value ? 1 << 13 : 0)); } }
        public bool RibbonArtist            { get { return (RIB0 & (1 << 12)) == 1 << 12; } set { RIB0 = (ushort)(RIB0 & ~(1 << 12) | (ushort)(value ? 1 << 12 : 0)); } }
        public bool RibbonEffort            { get { return (RIB0 & (1 << 11)) == 1 << 11; } set { RIB0 = (ushort)(RIB0 & ~(1 << 11) | (ushort)(value ? 1 << 11 : 0)); } }
        public bool RibbonChampionBattle    { get { return (RIB0 & (1 << 10)) == 1 << 10; } set { RIB0 = (ushort)(RIB0 & ~(1 << 10) | (ushort)(value ? 1 << 10 : 0)); } }
        public bool RibbonChampionRegional  { get { return (RIB0 & (1 << 09)) == 1 << 09; } set { RIB0 = (ushort)(RIB0 & ~(1 << 09) | (ushort)(value ? 1 << 09 : 0)); } }
        public bool RibbonChampionNational  { get { return (RIB0 & (1 << 08)) == 1 << 08; } set { RIB0 = (ushort)(RIB0 & ~(1 << 08) | (ushort)(value ? 1 << 08 : 0)); } }
        public bool RibbonCountry           { get { return (RIB0 & (1 << 07)) == 1 << 07; } set { RIB0 = (ushort)(RIB0 & ~(1 << 07) | (ushort)(value ? 1 << 07 : 0)); } }
        public bool RibbonNational          { get { return (RIB0 & (1 << 06)) == 1 << 06; } set { RIB0 = (ushort)(RIB0 & ~(1 << 06) | (ushort)(value ? 1 << 06 : 0)); } }
        public bool RibbonEarth             { get { return (RIB0 & (1 << 05)) == 1 << 05; } set { RIB0 = (ushort)(RIB0 & ~(1 << 05) | (ushort)(value ? 1 << 05 : 0)); } }
        public bool RibbonWorld             { get { return (RIB0 & (1 << 04)) == 1 << 04; } set { RIB0 = (ushort)(RIB0 & ~(1 << 04) | (ushort)(value ? 1 << 04 : 0)); } }
        public bool Unused1                 { get { return (RIB0 & (1 << 03)) == 1 << 03; } set { RIB0 = (ushort)(RIB0 & ~(1 << 03) | (ushort)(value ? 1 << 03 : 0)); } }
        public bool Unused2                 { get { return (RIB0 & (1 << 02)) == 1 << 02; } set { RIB0 = (ushort)(RIB0 & ~(1 << 02) | (ushort)(value ? 1 << 02 : 0)); } }
        public bool Unused3                 { get { return (RIB0 & (1 << 01)) == 1 << 01; } set { RIB0 = (ushort)(RIB0 & ~(1 << 01) | (ushort)(value ? 1 << 01 : 0)); } }
        public bool Unused4                 { get { return (RIB0 & (1 << 00)) == 1 << 00; } set { RIB0 = (ushort)(RIB0 & ~(1 << 00) | (ushort)(value ? 1 << 00 : 0)); } }
        // 0x7E-0x7F Unknown

        // Moves
        public override int Move1 { get { return BigEndian.ToUInt16(Data, 0x80); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x80); } }
        public override int Move1_PP { get { return Data[0x82]; } set { Data[0x82] = (byte)value; } }
        public override int Move1_PPUps { get { return Data[0x83]; } set { Data[0x83] = (byte)value; } }
        public override int Move2 { get { return BigEndian.ToUInt16(Data, 0x84); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x84); } }
        public override int Move2_PP { get { return Data[0x86]; } set { Data[0x86] = (byte)value; } }
        public override int Move2_PPUps { get { return Data[0x87]; } set { Data[0x87] = (byte)value; } }
        public override int Move3 { get { return BigEndian.ToUInt16(Data, 0x88); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x88); } }
        public override int Move3_PP { get { return Data[0x8A]; } set { Data[0x8A] = (byte)value; } }
        public override int Move3_PPUps { get { return Data[0x8B]; } set { Data[0x8B] = (byte)value; } }
        public override int Move4 { get { return BigEndian.ToUInt16(Data, 0x8C); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x8C); } }
        public override int Move4_PP { get { return Data[0x8E]; } set { Data[0x8E] = (byte)value; } }
        public override int Move4_PPUps { get { return Data[0x8F]; } set { Data[0x8F] = (byte)value; } }

        // More party stats
        public override int Stat_HPMax { get { return BigEndian.ToUInt16(Data, 0x90); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x90); } }
        public override int Stat_ATK { get { return BigEndian.ToUInt16(Data, 0x92); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x92); } }
        public override int Stat_DEF { get { return BigEndian.ToUInt16(Data, 0x94); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x94); } }
        public override int Stat_SPA { get { return BigEndian.ToUInt16(Data, 0x96); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x96); } }
        public override int Stat_SPD { get { return BigEndian.ToUInt16(Data, 0x98); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x98); } }
        public override int Stat_SPE { get { return BigEndian.ToUInt16(Data, 0x9A); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x9A); } }

        // EVs
        public override int EV_HP
        {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0x9C)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0x9C); }
        }
        public override int EV_ATK
        {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0x9E)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0x9E); }
        }
        public override int EV_DEF
        {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA0)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA0); }
        }
        public override int EV_SPA
        {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA2)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA2); }
        }
        public override int EV_SPD
        {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA4)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA4); }
        }
        public override int EV_SPE
        {
            get { return Math.Min(byte.MaxValue, BigEndian.ToUInt16(Data, 0xA6)); }
            set { BigEndian.GetBytes((ushort)(value & 0xFF)).CopyTo(Data, 0xA6); }
        }

        // IVs
        public override int IV_HP { get { return Data[0xA8]; } set { Data[0xA8] = (byte)(value & 0x1F); } }
        public override int IV_ATK { get { return Data[0xA9]; } set { Data[0xA9] = (byte)(value & 0x1F); } }
        public override int IV_DEF { get { return Data[0xAA]; } set { Data[0xAA] = (byte)(value & 0x1F); } }
        public override int IV_SPA { get { return Data[0xAB]; } set { Data[0xAB] = (byte)(value & 0x1F); } }
        public override int IV_SPD { get { return Data[0xAC]; } set { Data[0xAC] = (byte)(value & 0x1F); } }
        public override int IV_SPE { get { return Data[0xAD]; } set { Data[0xAD] = (byte)(value & 0x1F); } }
        
        // Contest
        public override int CNT_Cool { get { return Data[0xAE]; } set { Data[0xAE] = (byte)value; } }
        public override int CNT_Beauty { get { return Data[0xAF]; } set { Data[0xAF] = (byte)value; } }
        public override int CNT_Cute { get { return Data[0xB0]; } set { Data[0xB0] = (byte)value; } }
        public override int CNT_Smart { get { return Data[0xB1]; } set { Data[0xB1] = (byte)value; } }
        public override int CNT_Tough { get { return Data[0xB2]; } set { Data[0xB2] = (byte)value; } }
        public int RibbonCountG3Cool { get { return Data[0xB3]; } set { Data[0xB3] = (byte)value; } }
        public int RibbonCountG3Beauty { get { return Data[0xB4]; } set { Data[0xB4] = (byte)value; } }
        public int RibbonCountG3Cute { get { return Data[0xB5]; } set { Data[0xB5] = (byte)value; } }
        public int RibbonCountG3Smart { get { return Data[0xB6]; } set { Data[0xB6] = (byte)value; } }
        public int RibbonCountG3Tough { get { return Data[0xB7]; } set { Data[0xB7] = (byte)value; } }

        public int ShadowID { get { return BigEndian.ToUInt16(Data, 0xBA); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0xBA); } }
        
        // Purification information is stored in the save file and accessed based on the Shadow ID.
        public int Purification { get; set; }

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
