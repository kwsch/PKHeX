using System;
using System.Linq;

namespace PKHeX.Core
{
    public class PK4 : PKM, IRibbonSet1, IRibbonSet2
    {
        public static readonly byte[] ExtraBytes =
        {
            0x42, 0x43, 0x5E, 0x63, 0x64, 0x65, 0x66, 0x67, 0x87
        };
        public sealed override int SIZE_PARTY => PKX.SIZE_4PARTY;
        public override int SIZE_STORED => PKX.SIZE_4STORED;
        public override int Format => 4;
        public override PersonalInfo PersonalInfo => PersonalTable.HGSS.getFormeEntry(Species, AltForm);

        public PK4(byte[] decryptedData = null, string ident = null)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            PKMConverter.checkEncrypted(ref Data);
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
        }
        public override PKM Clone() { return new PK4(Data); }

        public override string getString(int Offset, int Count) => PKX.getString4(Data, Offset, Count);
        public override byte[] setString(string value, int maxLength) => PKX.setString4(value, maxLength);

        // Trash Bytes
        public override byte[] Nickname_Trash { get => getData(0x48, 22); set { if (value?.Length == 22) value.CopyTo(Data, 0x48); } }
        public override byte[] OT_Trash { get => getData(0x68, 16); set { if (value?.Length == 16) value.CopyTo(Data, 0x68); } }

        // Future Attributes
        public override uint EncryptionConstant { get => PID; set { } }
        public override int Nature { get => (int)(PID % 25); set { } }
        public override int CurrentFriendship { get => OT_Friendship; set => OT_Friendship = value; }
        public override int CurrentHandler { get => 0; set { } }
        public override int AbilityNumber { get => 1 << PIDAbility; set { } }

        // Structure
        public override uint PID { get => BitConverter.ToUInt32(Data, 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, 0x00); }
        public override ushort Sanity { get => BitConverter.ToUInt16(Data, 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, 0x04); }
        public override ushort Checksum { get => BitConverter.ToUInt16(Data, 0x06); set => BitConverter.GetBytes(value).CopyTo(Data, 0x06); }
        
        #region Block A
        public override int Species { get => BitConverter.ToUInt16(Data, 0x08); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x08); }
        public override int HeldItem { get => BitConverter.ToUInt16(Data, 0x0A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0A); }
        public override int TID { get => BitConverter.ToUInt16(Data, 0x0C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0C); }
        public override int SID { get => BitConverter.ToUInt16(Data, 0x0E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0E); }
        public override uint EXP { get => BitConverter.ToUInt32(Data, 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, 0x10); }
        public override int OT_Friendship { get => Data[0x14]; set => Data[0x14] = (byte)value; }
        public override int Ability { get => Data[0x15]; set => Data[0x15] = (byte)value; }
        public override int MarkValue { get => Data[0x16]; protected set => Data[0x16] = (byte)value; }
        public override int Language { get => Data[0x17]; set => Data[0x17] = (byte)value; }
        public override int EV_HP { get => Data[0x18]; set => Data[0x18] = (byte)value; }
        public override int EV_ATK { get => Data[0x19]; set => Data[0x19] = (byte)value; }
        public override int EV_DEF { get => Data[0x1A]; set => Data[0x1A] = (byte)value; }
        public override int EV_SPE { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
        public override int EV_SPA { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
        public override int EV_SPD { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }
        public override int CNT_Cool { get => Data[0x1E]; set => Data[0x1E] = (byte)value; }
        public override int CNT_Beauty { get => Data[0x1F]; set => Data[0x1F] = (byte)value; }
        public override int CNT_Cute { get => Data[0x20]; set => Data[0x20] = (byte)value; }
        public override int CNT_Smart { get => Data[0x21]; set => Data[0x21] = (byte)value; }
        public override int CNT_Tough { get => Data[0x22]; set => Data[0x22] = (byte)value; }
        public override int CNT_Sheen { get => Data[0x23]; set => Data[0x23] = (byte)value; }

        private byte RIB0 { get => Data[0x24]; set => Data[0x24] = value; } // Sinnoh 1
        private byte RIB1 { get => Data[0x25]; set => Data[0x25] = value; } // Sinnoh 2
        private byte RIB2 { get => Data[0x26]; set => Data[0x26] = value; } // Unova 1
        private byte RIB3 { get => Data[0x27]; set => Data[0x27] = value; } // Unova 2
        public bool RibbonChampionSinnoh    { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)(RIB0 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonAbility           { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)(RIB0 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonAbilityGreat      { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)(RIB0 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonAbilityDouble     { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)(RIB0 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonAbilityMulti      { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)(RIB0 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonAbilityPair       { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)(RIB0 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonAbilityWorld      { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)(RIB0 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonAlert             { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)(RIB0 & ~(1 << 7) | (value ? 1 << 7 : 0)); }
        public bool RibbonShock             { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)(RIB1 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonDowncast          { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)(RIB1 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonCareless          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)(RIB1 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonRelax             { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)(RIB1 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonSnooze            { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)(RIB1 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonSmile             { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)(RIB1 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonGorgeous          { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)(RIB1 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonRoyal             { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)(RIB1 & ~(1 << 7) | (value ? 1 << 7 : 0)); }
        public bool RibbonGorgeousRoyal     { get => (RIB2 & (1 << 0)) == 1 << 0; set => RIB2 = (byte)(RIB2 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonFootprint         { get => (RIB2 & (1 << 1)) == 1 << 1; set => RIB2 = (byte)(RIB2 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonRecord            { get => (RIB2 & (1 << 2)) == 1 << 2; set => RIB2 = (byte)(RIB2 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonEvent             { get => (RIB2 & (1 << 3)) == 1 << 3; set => RIB2 = (byte)(RIB2 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonLegend            { get => (RIB2 & (1 << 4)) == 1 << 4; set => RIB2 = (byte)(RIB2 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonChampionWorld     { get => (RIB2 & (1 << 5)) == 1 << 5; set => RIB2 = (byte)(RIB2 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonBirthday          { get => (RIB2 & (1 << 6)) == 1 << 6; set => RIB2 = (byte)(RIB2 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonSpecial           { get => (RIB2 & (1 << 7)) == 1 << 7; set => RIB2 = (byte)(RIB2 & ~(1 << 7) | (value ? 1 << 7 : 0)); }
        public bool RibbonSouvenir          { get => (RIB3 & (1 << 0)) == 1 << 0; set => RIB3 = (byte)(RIB3 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonWishing           { get => (RIB3 & (1 << 1)) == 1 << 1; set => RIB3 = (byte)(RIB3 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonClassic           { get => (RIB3 & (1 << 2)) == 1 << 2; set => RIB3 = (byte)(RIB3 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonPremier           { get => (RIB3 & (1 << 3)) == 1 << 3; set => RIB3 = (byte)(RIB3 & ~(1 << 3) | (value ? 1 << 3 : 0)); }              
        public bool RIB3_4 { get => (RIB3 & (1 << 4)) == 1 << 4; set => RIB3 = (byte)(RIB3 & ~(1 << 4) | (value ? 1 << 4 : 0)); } // Unused
        public bool RIB3_5 { get => (RIB3 & (1 << 5)) == 1 << 5; set => RIB3 = (byte)(RIB3 & ~(1 << 5) | (value ? 1 << 5 : 0)); } // Unused
        public bool RIB3_6 { get => (RIB3 & (1 << 6)) == 1 << 6; set => RIB3 = (byte)(RIB3 & ~(1 << 6) | (value ? 1 << 6 : 0)); } // Unused
        public bool RIB3_7 { get => (RIB3 & (1 << 7)) == 1 << 7; set => RIB3 = (byte)(RIB3 & ~(1 << 7) | (value ? 1 << 7 : 0)); } // Unused
        #endregion

        #region Block B
        public override int Move1 { get => BitConverter.ToUInt16(Data, 0x28); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x28); }
        public override int Move2 { get => BitConverter.ToUInt16(Data, 0x2A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2A); }
        public override int Move3 { get => BitConverter.ToUInt16(Data, 0x2C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2C); }
        public override int Move4 { get => BitConverter.ToUInt16(Data, 0x2E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E); }
        public override int Move1_PP { get => Data[0x30]; set => Data[0x30] = (byte)value; }
        public override int Move2_PP { get => Data[0x31]; set => Data[0x31] = (byte)value; }
        public override int Move3_PP { get => Data[0x32]; set => Data[0x32] = (byte)value; }
        public override int Move4_PP { get => Data[0x33]; set => Data[0x33] = (byte)value; }
        public override int Move1_PPUps { get => Data[0x34]; set => Data[0x34] = (byte)value; }
        public override int Move2_PPUps { get => Data[0x35]; set => Data[0x35] = (byte)value; }
        public override int Move3_PPUps { get => Data[0x36]; set => Data[0x36] = (byte)value; }
        public override int Move4_PPUps { get => Data[0x37]; set => Data[0x37] = (byte)value; }
        public uint IV32 { get => BitConverter.ToUInt32(Data, 0x38); set => BitConverter.GetBytes(value).CopyTo(Data, 0x38); }
        public override int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); }
        public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); }
        public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); }
        public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); }
        public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); }
        public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); }
        public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (uint)((IV32 & ~0x40000000) | (uint)(value ? 0x40000000 : 0)); }
        public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFF) | (value ? 0x80000000 : 0); }

        private byte RIB4 { get => Data[0x3C]; set => Data[0x3C] = value; } // Hoenn 1a
        private byte RIB5 { get => Data[0x3D]; set => Data[0x3D] = value; } // Hoenn 1b
        private byte RIB6 { get => Data[0x3E]; set => Data[0x3E] = value; } // Hoenn 2a
        private byte RIB7 { get => Data[0x3F]; set => Data[0x3F] = value; } // Hoenn 2b
        public bool RibbonG3Cool            { get => (RIB4 & (1 << 0)) == 1 << 0; set => RIB4 = (byte)(RIB4 & ~(1 << 0) | (value ? 1 << 0 : 0)); }             
        public bool RibbonG3CoolSuper       { get => (RIB4 & (1 << 1)) == 1 << 1; set => RIB4 = (byte)(RIB4 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonG3CoolHyper       { get => (RIB4 & (1 << 2)) == 1 << 2; set => RIB4 = (byte)(RIB4 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonG3CoolMaster      { get => (RIB4 & (1 << 3)) == 1 << 3; set => RIB4 = (byte)(RIB4 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonG3Beauty          { get => (RIB4 & (1 << 4)) == 1 << 4; set => RIB4 = (byte)(RIB4 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonG3BeautySuper     { get => (RIB4 & (1 << 5)) == 1 << 5; set => RIB4 = (byte)(RIB4 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonG3BeautyHyper     { get => (RIB4 & (1 << 6)) == 1 << 6; set => RIB4 = (byte)(RIB4 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonG3BeautyMaster    { get => (RIB4 & (1 << 7)) == 1 << 7; set => RIB4 = (byte)(RIB4 & ~(1 << 7) | (value ? 1 << 7 : 0)); }
        public bool RibbonG3Cute            { get => (RIB5 & (1 << 0)) == 1 << 0; set => RIB5 = (byte)(RIB5 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonG3CuteSuper       { get => (RIB5 & (1 << 1)) == 1 << 1; set => RIB5 = (byte)(RIB5 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonG3CuteHyper       { get => (RIB5 & (1 << 2)) == 1 << 2; set => RIB5 = (byte)(RIB5 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonG3CuteMaster      { get => (RIB5 & (1 << 3)) == 1 << 3; set => RIB5 = (byte)(RIB5 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonG3Smart           { get => (RIB5 & (1 << 4)) == 1 << 4; set => RIB5 = (byte)(RIB5 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonG3SmartSuper      { get => (RIB5 & (1 << 5)) == 1 << 5; set => RIB5 = (byte)(RIB5 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonG3SmartHyper      { get => (RIB5 & (1 << 6)) == 1 << 6; set => RIB5 = (byte)(RIB5 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonG3SmartMaster     { get => (RIB5 & (1 << 7)) == 1 << 7; set => RIB5 = (byte)(RIB5 & ~(1 << 7) | (value ? 1 << 7 : 0)); }
        public bool RibbonG3Tough           { get => (RIB6 & (1 << 0)) == 1 << 0; set => RIB6 = (byte)(RIB6 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonG3ToughSuper      { get => (RIB6 & (1 << 1)) == 1 << 1; set => RIB6 = (byte)(RIB6 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonG3ToughHyper      { get => (RIB6 & (1 << 2)) == 1 << 2; set => RIB6 = (byte)(RIB6 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonG3ToughMaster     { get => (RIB6 & (1 << 3)) == 1 << 3; set => RIB6 = (byte)(RIB6 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonChampionG3Hoenn   { get => (RIB6 & (1 << 4)) == 1 << 4; set => RIB6 = (byte)(RIB6 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonWinning           { get => (RIB6 & (1 << 5)) == 1 << 5; set => RIB6 = (byte)(RIB6 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonVictory           { get => (RIB6 & (1 << 6)) == 1 << 6; set => RIB6 = (byte)(RIB6 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonArtist            { get => (RIB6 & (1 << 7)) == 1 << 7; set => RIB6 = (byte)(RIB6 & ~(1 << 7) | (value ? 1 << 7 : 0)); }
        public bool RibbonEffort            { get => (RIB7 & (1 << 0)) == 1 << 0; set => RIB7 = (byte)(RIB7 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonChampionBattle    { get => (RIB7 & (1 << 1)) == 1 << 1; set => RIB7 = (byte)(RIB7 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonChampionRegional  { get => (RIB7 & (1 << 2)) == 1 << 2; set => RIB7 = (byte)(RIB7 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonChampionNational  { get => (RIB7 & (1 << 3)) == 1 << 3; set => RIB7 = (byte)(RIB7 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonCountry           { get => (RIB7 & (1 << 4)) == 1 << 4; set => RIB7 = (byte)(RIB7 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonNational          { get => (RIB7 & (1 << 5)) == 1 << 5; set => RIB7 = (byte)(RIB7 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonEarth             { get => (RIB7 & (1 << 6)) == 1 << 6; set => RIB7 = (byte)(RIB7 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonWorld             { get => (RIB7 & (1 << 7)) == 1 << 7; set => RIB7 = (byte)(RIB7 & ~(1 << 7) | (value ? 1 << 7 : 0)); }

        public override bool FatefulEncounter { get => (Data[0x40] & 1) == 1; set => Data[0x40] = (byte)(Data[0x40] & ~0x01 | (value ? 1 : 0)); }
        public override int Gender { get => (Data[0x40] >> 1) & 0x3; set => Data[0x40] = (byte)(Data[0x40] & ~0x06 | (value << 1)); }
        public override int AltForm { get => Data[0x40] >> 3; set => Data[0x40] = (byte)(Data[0x40] & 0x07 | (value << 3)); }
        // 0x43-0x47 Unused
        #endregion

        #region Block C
        public override string Nickname { get => getString(0x48, 22); set => setString(value, 11).CopyTo(Data, 0x48); }
        // 0x5E unused
        public override int Version { get => Data[0x5F]; set => Data[0x5F] = (byte)value; }
        private byte RIB8 { get => Data[0x60]; set => Data[0x60] = value; } // Sinnoh 3
        private byte RIB9 { get => Data[0x61]; set => Data[0x61] = value; } // Sinnoh 4
        private byte RIBA { get => Data[0x62]; set => Data[0x62] = value; } // Sinnoh 5
        private byte RIBB { get => Data[0x63]; set => Data[0x63] = value; } // Sinnoh 6
        public bool RibbonG4Cool            { get => (RIB8 & (1 << 0)) == 1 << 0; set => RIB8 = (byte)(RIB8 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonG4CoolGreat       { get => (RIB8 & (1 << 1)) == 1 << 1; set => RIB8 = (byte)(RIB8 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonG4CoolUltra       { get => (RIB8 & (1 << 2)) == 1 << 2; set => RIB8 = (byte)(RIB8 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonG4CoolMaster      { get => (RIB8 & (1 << 3)) == 1 << 3; set => RIB8 = (byte)(RIB8 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonG4Beauty          { get => (RIB8 & (1 << 4)) == 1 << 4; set => RIB8 = (byte)(RIB8 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonG4BeautyGreat     { get => (RIB8 & (1 << 5)) == 1 << 5; set => RIB8 = (byte)(RIB8 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonG4BeautyUltra     { get => (RIB8 & (1 << 6)) == 1 << 6; set => RIB8 = (byte)(RIB8 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonG4BeautyMaster    { get => (RIB8 & (1 << 7)) == 1 << 7; set => RIB8 = (byte)(RIB8 & ~(1 << 7) | (value ? 1 << 7 : 0)); }
        public bool RibbonG4Cute            { get => (RIB9 & (1 << 0)) == 1 << 0; set => RIB9 = (byte)(RIB9 & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonG4CuteGreat       { get => (RIB9 & (1 << 1)) == 1 << 1; set => RIB9 = (byte)(RIB9 & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonG4CuteUltra       { get => (RIB9 & (1 << 2)) == 1 << 2; set => RIB9 = (byte)(RIB9 & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonG4CuteMaster      { get => (RIB9 & (1 << 3)) == 1 << 3; set => RIB9 = (byte)(RIB9 & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RibbonG4Smart           { get => (RIB9 & (1 << 4)) == 1 << 4; set => RIB9 = (byte)(RIB9 & ~(1 << 4) | (value ? 1 << 4 : 0)); }
        public bool RibbonG4SmartGreat      { get => (RIB9 & (1 << 5)) == 1 << 5; set => RIB9 = (byte)(RIB9 & ~(1 << 5) | (value ? 1 << 5 : 0)); }
        public bool RibbonG4SmartUltra      { get => (RIB9 & (1 << 6)) == 1 << 6; set => RIB9 = (byte)(RIB9 & ~(1 << 6) | (value ? 1 << 6 : 0)); }
        public bool RibbonG4SmartMaster     { get => (RIB9 & (1 << 7)) == 1 << 7; set => RIB9 = (byte)(RIB9 & ~(1 << 7) | (value ? 1 << 7 : 0)); }
        public bool RibbonG4Tough           { get => (RIBA & (1 << 0)) == 1 << 0; set => RIBA = (byte)(RIBA & ~(1 << 0) | (value ? 1 << 0 : 0)); }
        public bool RibbonG4ToughGreat      { get => (RIBA & (1 << 1)) == 1 << 1; set => RIBA = (byte)(RIBA & ~(1 << 1) | (value ? 1 << 1 : 0)); }
        public bool RibbonG4ToughUltra      { get => (RIBA & (1 << 2)) == 1 << 2; set => RIBA = (byte)(RIBA & ~(1 << 2) | (value ? 1 << 2 : 0)); }
        public bool RibbonG4ToughMaster     { get => (RIBA & (1 << 3)) == 1 << 3; set => RIBA = (byte)(RIBA & ~(1 << 3) | (value ? 1 << 3 : 0)); }
        public bool RIBA_4 { get => (RIBA & (1 << 4)) == 1 << 4; set => RIBA = (byte)(RIBA & ~(1 << 4) | (value ? 1 << 4 : 0)); } // Unused
        public bool RIBA_5 { get => (RIBA & (1 << 5)) == 1 << 5; set => RIBA = (byte)(RIBA & ~(1 << 5) | (value ? 1 << 5 : 0)); } // Unused
        public bool RIBA_6 { get => (RIBA & (1 << 6)) == 1 << 6; set => RIBA = (byte)(RIBA & ~(1 << 6) | (value ? 1 << 6 : 0)); } // Unused
        public bool RIBA_7 { get => (RIBA & (1 << 7)) == 1 << 7; set => RIBA = (byte)(RIBA & ~(1 << 7) | (value ? 1 << 7 : 0)); } // Unused
        public bool RIBB_0 { get => (RIBB & (1 << 0)) == 1 << 0; set => RIBB = (byte)(RIBB & ~(1 << 0) | (value ? 1 << 0 : 0)); } // Unused
        public bool RIBB_1 { get => (RIBB & (1 << 1)) == 1 << 1; set => RIBB = (byte)(RIBB & ~(1 << 1) | (value ? 1 << 1 : 0)); } // Unused
        public bool RIBB_2 { get => (RIBB & (1 << 2)) == 1 << 2; set => RIBB = (byte)(RIBB & ~(1 << 2) | (value ? 1 << 2 : 0)); } // Unused
        public bool RIBB_3 { get => (RIBB & (1 << 3)) == 1 << 3; set => RIBB = (byte)(RIBB & ~(1 << 3) | (value ? 1 << 3 : 0)); } // Unused
        public bool RIBB_4 { get => (RIBB & (1 << 4)) == 1 << 4; set => RIBB = (byte)(RIBB & ~(1 << 4) | (value ? 1 << 4 : 0)); } // Unused
        public bool RIBB_5 { get => (RIBB & (1 << 5)) == 1 << 5; set => RIBB = (byte)(RIBB & ~(1 << 5) | (value ? 1 << 5 : 0)); } // Unused
        public bool RIBB_6 { get => (RIBB & (1 << 6)) == 1 << 6; set => RIBB = (byte)(RIBB & ~(1 << 6) | (value ? 1 << 6 : 0)); } // Unused
        public bool RIBB_7 { get => (RIBB & (1 << 7)) == 1 << 7; set => RIBB = (byte)(RIBB & ~(1 << 7) | (value ? 1 << 7 : 0)); } // Unused
        // 0x64-0x67 Unused
        #endregion

        #region Block D
        public override string OT_Name { get => getString(0x68, 16); set => setString(value, 7).CopyTo(Data, 0x68); }
        public override int Egg_Year { get => Data[0x78]; set => Data[0x78] = (byte)value; }
        public override int Egg_Month { get => Data[0x79]; set => Data[0x79] = (byte)value; }
        public override int Egg_Day { get => Data[0x7A]; set => Data[0x7A] = (byte)value; }
        public override int Met_Year { get => Data[0x7B]; set => Data[0x7B] = (byte)value; }
        public override int Met_Month { get => Data[0x7C]; set => Data[0x7C] = (byte)value; }
        public override int Met_Day { get => Data[0x7D]; set => Data[0x7D] = (byte)value; }

        public override int Egg_Location
        {
            get
            {
                ushort hgssloc = BitConverter.ToUInt16(Data, 0x44);
                if (hgssloc != 0)
                    return hgssloc;
                return BitConverter.ToUInt16(Data, 0x7E);
            }
            set
            {
                if (value == 0)
                {
                    BitConverter.GetBytes((ushort)0).CopyTo(Data, 0x44);
                    BitConverter.GetBytes((ushort)0).CopyTo(Data, 0x7E);
                }
                else if (value < 2000 && value > 111 || value < 3000 && value > 2010)
                {
                    // Met location not in DP, set to Faraway Place
                    BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x44);
                    BitConverter.GetBytes((ushort)0xBBA).CopyTo(Data, 0x7E);
                }
                else
                {
                    int pthgss = PtHGSS ? value : 0; // only set to PtHGSS loc if encountered in game
                    BitConverter.GetBytes((ushort)pthgss).CopyTo(Data, 0x44);
                    BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7E);
                }
            }
        }
        public override int Met_Location
        {
            get
            {
                ushort hgssloc = BitConverter.ToUInt16(Data, 0x46);
                if (hgssloc != 0)
                    return hgssloc;
                return BitConverter.ToUInt16(Data, 0x80);
            }
            set
            {
                if (value == 0)
                {
                    BitConverter.GetBytes((ushort)0).CopyTo(Data, 0x46);
                    BitConverter.GetBytes((ushort)0).CopyTo(Data, 0x80);
                }
                else if (value < 2000 && value > 111 || value < 3000 && value > 2010)
                {
                    // Met location not in DP, set to Faraway Place
                    BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x46);
                    BitConverter.GetBytes((ushort)0xBBA).CopyTo(Data, 0x80);
                }
                else
                {
                    int pthgss = PtHGSS ? value : 0; // only set to PtHGSS loc if encountered in game
                    BitConverter.GetBytes((ushort)pthgss).CopyTo(Data, 0x46);
                    BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x80);
                }
            }
        }
        private byte PKRS { get => Data[0x82]; set => Data[0x82] = value; }
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)(PKRS & ~0xF | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)(PKRS & 0xF | (value << 4)); }
        public override int Ball
        {
            get =>
                // Pokemon obtained in HGSS have the HGSS ball set (@0x86)
                // However, this info is not set when receiving a wondercard!
                // The PGT contains a preformatted PK4 file, which is slightly modified.
                // No HGSS balls were used, and no HGSS ball info is set.

                // Sneaky way = return the higher of the two values.
                Math.Max(Data[0x86], Data[0x83]);
            set
            {
                // Ball to display in DPPt
                Data[0x83] = (byte)(value <= 0x10 ? value : 4); // Cap at Cherish Ball

                // HGSS Exclusive Balls -- If the user wants to screw things up, let them. Any legality checking could catch hax.
                if (value > 0x10 || (HGSS && !FatefulEncounter))
                    Data[0x86] = (byte)(value <= 0x18 ? value : 4); // Cap at Comp Ball
                else
                    Data[0x86] = 0; // Unused
            }
        }
        public override int Met_Level { get => Data[0x84] & ~0x80; set => Data[0x84] = (byte)((Data[0x84] & 0x80) | value); }
        public override int OT_Gender { get => Data[0x84] >> 7; set => Data[0x84] = (byte)((Data[0x84] & ~0x80) | value << 7); }
        public override int EncounterType { get => Data[0x85]; set => Data[0x85] = (byte)value; }
        // Unused 0x87
        #endregion

        public override int Stat_Level { get => Data[0x8C]; set => Data[0x8C] = (byte)value; }
        public override int Stat_HPCurrent { get => BitConverter.ToUInt16(Data, 0x8E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x8E); }
        public override int Stat_HPMax { get => BitConverter.ToUInt16(Data, 0x90); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x90); }
        public override int Stat_ATK { get => BitConverter.ToUInt16(Data, 0x92); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x92); }
        public override int Stat_DEF { get => BitConverter.ToUInt16(Data, 0x94); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x94); }
        public override int Stat_SPE { get => BitConverter.ToUInt16(Data, 0x96); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x96); }
        public override int Stat_SPA { get => BitConverter.ToUInt16(Data, 0x98); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x98); }
        public override int Stat_SPD { get => BitConverter.ToUInt16(Data, 0x9A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x9A); }

        public override int PSV => (int)((PID >> 16 ^ PID & 0xFFFF) >> 3);
        public override int TSV => (TID ^ SID) >> 3;
        public override int Characteristic
        {
            get
            {
                // Characteristic with PID%6
                int pm6 = (int)(PID % 6); // PID MOD 6
                int maxIV = IVs.Max();
                int pm6stat = 0;

                for (int i = 0; i < 6; i++)
                {
                    pm6stat = (pm6 + i) % 6;
                    if (IVs[pm6stat] == maxIV)
                        break; // P%6 is this stat
                }
                return pm6stat * 5 + maxIV % 5;
            }
        }
        // Legality Extensions
        public override bool WasEgg => GenNumber < 4 ? base.WasEgg : Species == 490 && Egg_Location == 3001 || Legal.EggLocations4.Contains(Egg_Location);
        public override bool WasEvent => Met_Location >= 3000 && Met_Location <= 3076 || FatefulEncounter;
        public override bool WasIngameTrade => Met_Location == 2001; // Trade
        public override bool WasEventEgg => WasEgg && Species == 490; // Manaphy was the only generation 4 released event egg
        // Methods
        public override byte[] Encrypt()
        {
            RefreshChecksum();
            return PKX.encryptArray45(Data);
        }

        public BK4 convertToBK4()
        {
            BK4 bk4 = new BK4();
            TransferPropertiesWithReflection(this, bk4);
            // Fix Non-Reflectable properties
            Array.Copy(Data, 0x78, bk4.Data, 0x78, 6); // Met Info
            // Preserve Trash Bytes
            for (int i = 0; i < 11; i++) // Nickname
            {
                bk4.Data[0x48 + 2*i] = Data[0x48 + 2*i + 1];
                bk4.Data[0x48 + 2*i + 1] = Data[0x48 + 2*i];
            }
            for (int i = 0; i < 8; i++) // OT_Name
            {
                bk4.Data[0x68 + 2*i] = Data[0x68 + 2*i + 1];
                bk4.Data[0x68 + 2*i + 1] = Data[0x68 + 2*i];
            }
            bk4.Sanity = 0x4000;
            bk4.RefreshChecksum();
            return bk4;
        }

        public PK5 convertToPK5()
        {
            // Double Check Location Data to see if we're already a PK5
            if (Data[0x5F] < 0x10 && BitConverter.ToUInt16(Data, 0x80) > 0x4000)
                return new PK5(Data);

            DateTime moment = DateTime.Now;

            PK5 pk5 = new PK5(Data) // Convert away!
            {
                OT_Friendship = 70,
                // Apply new met date
                MetDate = moment
            };

            // Arceus Type Changing -- Plate forcibly removed.
            if (pk5.Species == 493)
            {
                pk5.AltForm = 0;
                pk5.HeldItem = 0;
            }
            else
            {
                pk5.HeldItem = Legal.HeldItems_BW.Contains((ushort) HeldItem) ? HeldItem : 0;
            }

            // Fix PP
            pk5.Move1_PP = pk5.getMovePP(pk5.Move1, pk5.Move1_PPUps);
            pk5.Move2_PP = pk5.getMovePP(pk5.Move2, pk5.Move2_PPUps);
            pk5.Move3_PP = pk5.getMovePP(pk5.Move3, pk5.Move3_PPUps);
            pk5.Move4_PP = pk5.getMovePP(pk5.Move4, pk5.Move4_PPUps);

            // Disassociate Nature and PID, pk4 getter does PID%25
            pk5.Nature = Nature;

            // Delete Platinum/HGSS Met Location Data
            BitConverter.GetBytes((uint)0).CopyTo(pk5.Data, 0x44);

            // Met / Crown Data Detection
            pk5.Met_Location = pk5.Gen4 && pk5.FatefulEncounter && Array.IndexOf(Legal.CrownBeasts, pk5.Species) >= 0
                ? (pk5.Species == 251 ? 30010 : 30012) // Celebi : Beast
                : 30001; // Pokétransfer (not Crown)
            
            // Delete HGSS Data
            BitConverter.GetBytes((ushort)0).CopyTo(pk5.Data, 0x86);
            pk5.Ball = Ball;

            // Transfer Nickname and OT Name
            pk5.Nickname = Nickname;
            pk5.OT_Name = OT_Name;

            // Fix Level
            pk5.Met_Level = PKX.getLevel(pk5.Species, pk5.EXP);

            // Remove HM moves; Defog should be kept if both are learned.
            int[] banned = Moves.Contains(250) && Moves.Contains(432) // Whirlpool & Defog
                ? new[] {15, 19, 57, 70, 250, 249, 127, 431} // No Whirlpool
                : new[] {15, 19, 57, 70,      249, 127, 431};// Transfer via advantageous game

            int[] newMoves = pk5.Moves;
            for (int i = 0; i < 4; i++)
                if (banned.Contains(newMoves[i]))
                    newMoves[i] = 0;
            pk5.Moves = newMoves;
            pk5.FixMoves();

            pk5.RefreshChecksum();
            return pk5;
        }
    }
}
