using System;
using System.Text;

namespace PKHeX
{
    public class WC6
    {
        internal static int Size;

        public byte[] Data;
        public WC6(byte[] data = null)
        {
            Data = data ?? new byte[Size];
        }

        // General Card Properties
        public int CardID {
            get { return BitConverter.ToUInt16(Data, 0); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0); } }
        public string CardTitle {
            get { return Encoding.Unicode.GetString(Data, 2, 64).Trim(); }
            set { Encoding.Unicode.GetBytes(value.PadRight(32, '\0')).CopyTo(Data, 2); } }
        private uint Date { 
            get { return BitConverter.ToUInt32(Data, 0x4C); } 
            set { BitConverter.GetBytes(value).CopyTo(Data, 0x4C); } }
        public uint Year {
            get { return Date/10000; }
            set { Date = value*10000 + Date%10000; } }
        public uint Month {
            get { return Date%10000/100; }
            set { Date = Year*10000 + value*100 + Date%100; } }
        public uint Day {
            get { return Date%100; }
            set { Date = Year*10000 + Month*100 + value; } }
        public int CardLocation { get { return Data[0x50]; } set { Data[0x50] = (byte)value; } }

        public int CardType { get { return Data[0x51]; } set { Data[0x51] = (byte)value; } }
        public bool GiftUsed { get { return Data[0x52] >> 1 > 0; } set { Data[0x52] = (byte)(Data[0x52] & ~2 | (value ? 2 : 0)); } }
        public bool MultiObtain { get { return Data[0x53] == 1; } set { Data[0x53] = (byte)(value ? 1 : 0); } }

        // Item Properties
        public bool IsItem { get { return CardType == 1; } set { if (value) CardType = 1; } }
        public int Item {
            get { return BitConverter.ToUInt16(Data, 0x68); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x68); } }
        public int Quantity {
            get { return BitConverter.ToUInt16(Data, 0x70); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x70); } }
        
        // Pokémon Properties
        public bool IsPokémon { get { return CardType == 0; } set { if (value) CardType = 0; } }
        public int TID { 
            get { return BitConverter.ToUInt16(Data, 0x68); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x68); } }
        public int SID { 
            get { return BitConverter.ToUInt16(Data, 0x6A); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x6A); } }
        public int OriginGame {
            get { return Data[0x6C]; } 
            set { Data[0x6C] = (byte)value; } }
        public uint EncryptionConstant {
            get { return BitConverter.ToUInt32(Data, 0x70); }
            set { BitConverter.GetBytes(value).CopyTo(Data, 0x70); } }
        public int Pokéball {
            get { return Data[0x76]; } 
            set { Data[0x76] = (byte)value; } }
        public int HeldItem {
            get { return BitConverter.ToUInt16(Data, 0x78); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x78); } }
        public int Move1 {
            get { return BitConverter.ToUInt16(Data, 0x7A); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7A); } }
        public int Move2 {
            get { return BitConverter.ToUInt16(Data, 0x7C); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7C); } }
        public int Move3 {
            get { return BitConverter.ToUInt16(Data, 0x7E); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7E); } }
        public int Move4 {
            get { return BitConverter.ToUInt16(Data, 0x80); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x80); } }
        public int Species {
            get { return BitConverter.ToUInt16(Data, 0x82); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x82); } }
        public int Form {
            get { return Data[0x84]; } 
            set { Data[0x84] = (byte)value; } }
        public int Language {
            get { return Data[0x85]; } 
            set { Data[0x85] = (byte)value; } }
        public string Nickname {
            get { return Encoding.Unicode.GetString(Data, 0x86, 0x1A).Trim(); }
            set { Encoding.Unicode.GetBytes(value.PadRight(12 + 1, '\0')).CopyTo(Data, 0x86); } }
        public int Nature {
            get { return Data[0xA0]; } 
            set { Data[0xA0] = (byte)value; } }
        public int Gender {
            get { return Data[0xA1]; } 
            set { Data[0xA1] = (byte)value; } }
        public int AbilityType {
            get { return Data[0xA2]; } 
            set { Data[0xA2] = (byte)value; } }
        public int PIDType {
            get { return Data[0xA3]; } 
            set { Data[0xA3] = (byte)value; } }
        public int EggLocation {
            get { return BitConverter.ToUInt16(Data, 0xA4); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA4); } }
        public int MetLocation  {
            get { return BitConverter.ToUInt16(Data, 0xA6); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA6); } }

        public int CNT_Cool { get { return Data[0xA9]; } set { Data[0xA9] = (byte)value; } }
        public int CNT_Beauty { get { return Data[0xAA]; } set { Data[0xAA] = (byte)value; } }
        public int CNT_Cute { get { return Data[0xAB]; } set { Data[0xAB] = (byte)value; } }
        public int CNT_Smart { get { return Data[0xAC]; } set { Data[0xAC] = (byte)value; } }
        public int CNT_Tough { get { return Data[0xAD]; } set { Data[0xAD] = (byte)value; } }
        public int CNT_Sheen { get { return Data[0xAE]; } set { Data[0xAE] = (byte)value; } }

        public int IV_HP { get { return Data[0xAF]; } set { Data[0xAF] = (byte)value; } }
        public int IV_ATK { get { return Data[0xB0]; } set { Data[0xB0] = (byte)value; } }
        public int IV_DEF { get { return Data[0xB1]; } set { Data[0xB1] = (byte)value; } }
        public int IV_SPE { get { return Data[0xB2]; } set { Data[0xB2] = (byte)value; } }
        public int IV_SPA { get { return Data[0xB3]; } set { Data[0xB3] = (byte)value; } }
        public int IV_SPD { get { return Data[0xB4]; } set { Data[0xB4] = (byte)value; } }
        
        public string OT {
            get { return Encoding.Unicode.GetString(Data, 0xB6, 0x1A).Trim(); }
            set { Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, 0xB6); } }
        public int Level { get { return Data[0xD0]; } set { Data[0xD0] = (byte)value; } }
        public bool IsEgg { get { return Data[0xD1] == 1; } set { Data[0xD1] = (byte)(value ? 1 : 0); } }
        public uint PID {
            get { return BitConverter.ToUInt32(Data, 0xD4); }
            set { BitConverter.GetBytes(value).CopyTo(Data, 0xD4); } }
        public int RelearnMove1 {
            get { return BitConverter.ToUInt16(Data, 0xD8); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xD8); } }
        public int RelearnMove2 {
            get { return BitConverter.ToUInt16(Data, 0xDA); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDA); } }
        public int RelearnMove3 {
            get { return BitConverter.ToUInt16(Data, 0xDC); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDC); } }
        public int RelearnMove4 {
            get { return BitConverter.ToUInt16(Data, 0xDE); } 
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xDE); } }

        private byte RIB0 { get { return Data[0x74]; } set { Data[0x74] = value; } }
        public bool RIB0_0 { get { return (RIB0 & (1 << 0)) == 1 << 0; } set { RIB0 = (byte)(RIB0 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Battle Champ Ribbon
        public bool RIB0_1 { get { return (RIB0 & (1 << 1)) == 1 << 1; } set { RIB0 = (byte)(RIB0 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Regional Champ Ribbon
        public bool RIB0_2 { get { return (RIB0 & (1 << 2)) == 1 << 2; } set { RIB0 = (byte)(RIB0 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // National Champ Ribbon
        public bool RIB0_3 { get { return (RIB0 & (1 << 3)) == 1 << 3; } set { RIB0 = (byte)(RIB0 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Country Ribbon
        public bool RIB0_4 { get { return (RIB0 & (1 << 4)) == 1 << 4; } set { RIB0 = (byte)(RIB0 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // National Ribbon
        public bool RIB0_5 { get { return (RIB0 & (1 << 5)) == 1 << 5; } set { RIB0 = (byte)(RIB0 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Earth Ribbon
        public bool RIB0_6 { get { return (RIB0 & (1 << 6)) == 1 << 6; } set { RIB0 = (byte)(RIB0 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // World Ribbon
        public bool RIB0_7 { get { return (RIB0 & (1 << 7)) == 1 << 7; } set { RIB0 = (byte)(RIB0 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Event Ribbon
        private byte RIB1 { get { return Data[0x75]; } set { Data[0x75] = value; } }
        public bool RIB1_0 { get { return (RIB1 & (1 << 0)) == 1 << 0; } set { RIB1 = (byte)(RIB1 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // World Champ Ribbon
        public bool RIB1_1 { get { return (RIB1 & (1 << 1)) == 1 << 1; } set { RIB1 = (byte)(RIB1 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Birthday Ribbon
        public bool RIB1_2 { get { return (RIB1 & (1 << 2)) == 1 << 2; } set { RIB1 = (byte)(RIB1 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Special Ribbon
        public bool RIB1_3 { get { return (RIB1 & (1 << 3)) == 1 << 3; } set { RIB1 = (byte)(RIB1 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Souvenir Ribbon
        public bool RIB1_4 { get { return (RIB1 & (1 << 4)) == 1 << 4; } set { RIB1 = (byte)(RIB1 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Wishing Ribbon
        public bool RIB1_5 { get { return (RIB1 & (1 << 5)) == 1 << 5; } set { RIB1 = (byte)(RIB1 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Classic Ribbon
        public bool RIB1_6 { get { return (RIB1 & (1 << 6)) == 1 << 6; } set { RIB1 = (byte)(RIB1 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Premier Ribbon
        public bool RIB1_7 { get { return (RIB1 & (1 << 7)) == 1 << 7; } set { RIB1 = (byte)(RIB1 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Empty

        // Meta Accessible Properties
        public int[] IVs { get { return new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD }; } }
        public bool IsNicknamed { get { return Nickname.Length > 0; } }
        public int[] Moves
        {
            get { return new[] {Move1, Move2, Move3, Move4}; }
            set
            {
                if (value.Length > 0) Move1 = value[0];
                if (value.Length > 1) Move2 = value[1];
                if (value.Length > 2) Move3 = value[2];
                if (value.Length > 3) Move4 = value[3];
            }
        }
        public int[] RelearnMoves
        {
            get { return new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 }; }
            set
            {
                if (value.Length > 0) RelearnMove1 = value[0];
                if (value.Length > 1) RelearnMove2 = value[1];
                if (value.Length > 2) RelearnMove3 = value[2];
                if (value.Length > 3) RelearnMove4 = value[3];
            }
        }
    }
}
