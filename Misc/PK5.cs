using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public class PK5 : PKM // 5th Generation PKM File
    {
        internal const int SIZE_PARTY = 220;
        internal const int SIZE_STORED = 136;
        internal const int SIZE_BLOCK = 32;

        public PK5(byte[] decryptedData = null, string ident = null)
        {
            Data = (byte[])(decryptedData ?? new byte[SIZE_PARTY]).Clone();
            Identifier = ident;
            if (Data.Length != SIZE_PARTY)
                Array.Resize(ref Data, SIZE_PARTY);
        }

        // Internal Attributes set on creation
        public byte[] Data; // Raw Storage
        public string Identifier; // User or Form Custom Attribute

        // Structure
        public uint PID { get { return BitConverter.ToUInt32(Data, 0x00); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x00); } }
        public ushort Sanity { get { return BitConverter.ToUInt16(Data, 0x04); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x04); } }
        public ushort Checksum { get { return BitConverter.ToUInt16(Data, 0x06); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x06); } }

        #region Block A
        public int Species { get { return BitConverter.ToUInt16(Data, 0x08); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x08); } }
        public int HeldItem { get { return BitConverter.ToUInt16(Data, 0x0A); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0A); } }
        public int TID { get { return BitConverter.ToUInt16(Data, 0x0C); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0C); } }
        public int SID { get { return BitConverter.ToUInt16(Data, 0x0E); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0E); } }
        public uint EXP { get { return BitConverter.ToUInt32(Data, 0x10); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x10); } }
        public int Friendship { get { return Data[0x14]; } set { Data[0x14] = (byte)value; } }
        public int Ability { get { return Data[0x15]; } set { Data[0x15] = (byte)value; } }
        public byte Markings { get { return Data[0x16]; } set { Data[0x16] = value; } }
        public bool Circle { get { return (Markings & (1 << 0)) == 1 << 0; } set { Markings = (byte)(Markings & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool Triangle { get { return (Markings & (1 << 1)) == 1 << 1; } set { Markings = (byte)(Markings & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool Square { get { return (Markings & (1 << 2)) == 1 << 2; } set { Markings = (byte)(Markings & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool Heart { get { return (Markings & (1 << 3)) == 1 << 3; } set { Markings = (byte)(Markings & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool Star { get { return (Markings & (1 << 4)) == 1 << 4; } set { Markings = (byte)(Markings & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool Diamond { get { return (Markings & (1 << 5)) == 1 << 5; } set { Markings = (byte)(Markings & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public int Language { get { return Data[0x17]; } set { Data[0x17] = (byte)value; } }
        public int EV_HP { get { return Data[0x18]; } set { Data[0x18] = (byte)value; } }
        public int EV_ATK { get { return Data[0x19]; } set { Data[0x19] = (byte)value; } }
        public int EV_DEF { get { return Data[0x1A]; } set { Data[0x1A] = (byte)value; } }
        public int EV_SPE { get { return Data[0x1B]; } set { Data[0x1B] = (byte)value; } }
        public int EV_SPA { get { return Data[0x1C]; } set { Data[0x1C] = (byte)value; } }
        public int EV_SPD { get { return Data[0x1D]; } set { Data[0x1D] = (byte)value; } }
        public int CNT_Cool { get { return Data[0x1E]; } set { Data[0x1E] = (byte)value; } }
        public int CNT_Beauty { get { return Data[0x1F]; } set { Data[0x1F] = (byte)value; } }
        public int CNT_Cute { get { return Data[0x20]; } set { Data[0x20] = (byte)value; } }
        public int CNT_Smart { get { return Data[0x21]; } set { Data[0x21] = (byte)value; } }
        public int CNT_Tough { get { return Data[0x22]; } set { Data[0x22] = (byte)value; } }
        public int CNT_Sheen { get { return Data[0x23]; } set { Data[0x23] = (byte)value; } }

        private byte RIB0 { get { return Data[0x24]; } set { Data[0x24] = value; } } // Sinnoh 1
        public bool RIB0_0 { get { return (RIB0 & (1 << 0)) == 1 << 0; } set { RIB0 = (byte)(RIB0 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Sinnoh Champ Ribbon
        public bool RIB0_1 { get { return (RIB0 & (1 << 1)) == 1 << 1; } set { RIB0 = (byte)(RIB0 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Ability Ribbon
        public bool RIB0_2 { get { return (RIB0 & (1 << 2)) == 1 << 2; } set { RIB0 = (byte)(RIB0 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Great Ability Ribbon
        public bool RIB0_3 { get { return (RIB0 & (1 << 3)) == 1 << 3; } set { RIB0 = (byte)(RIB0 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Double Ability Ribbon
        public bool RIB0_4 { get { return (RIB0 & (1 << 4)) == 1 << 4; } set { RIB0 = (byte)(RIB0 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Multi Ability Ribbon
        public bool RIB0_5 { get { return (RIB0 & (1 << 5)) == 1 << 5; } set { RIB0 = (byte)(RIB0 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Pair Ability Ribbon
        public bool RIB0_6 { get { return (RIB0 & (1 << 6)) == 1 << 6; } set { RIB0 = (byte)(RIB0 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // World Ability Ribbon
        public bool RIB0_7 { get { return (RIB0 & (1 << 7)) == 1 << 7; } set { RIB0 = (byte)(RIB0 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Alert Ribbon
        private byte RIB1 { get { return Data[0x25]; } set { Data[0x25] = value; } } // Sinnoh 2                                           
        public bool RIB1_0 { get { return (RIB1 & (1 << 0)) == 1 << 0; } set { RIB1 = (byte)(RIB1 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Shock Ribbon
        public bool RIB1_1 { get { return (RIB1 & (1 << 1)) == 1 << 1; } set { RIB1 = (byte)(RIB1 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Downcast Ribbon
        public bool RIB1_2 { get { return (RIB1 & (1 << 2)) == 1 << 2; } set { RIB1 = (byte)(RIB1 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Careless Ribbon
        public bool RIB1_3 { get { return (RIB1 & (1 << 3)) == 1 << 3; } set { RIB1 = (byte)(RIB1 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Relax Ribbon
        public bool RIB1_4 { get { return (RIB1 & (1 << 4)) == 1 << 4; } set { RIB1 = (byte)(RIB1 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Snooze Ribbon
        public bool RIB1_5 { get { return (RIB1 & (1 << 5)) == 1 << 5; } set { RIB1 = (byte)(RIB1 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Smile Ribbon
        public bool RIB1_6 { get { return (RIB1 & (1 << 6)) == 1 << 6; } set { RIB1 = (byte)(RIB1 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Gorgeous Ribbon
        public bool RIB1_7 { get { return (RIB1 & (1 << 7)) == 1 << 7; } set { RIB1 = (byte)(RIB1 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Royal Ribbon
        private byte RIB2 { get { return Data[0x26]; } set { Data[0x26] = value; } } // Unova 1
        public bool RIB2_0 { get { return (RIB2 & (1 << 0)) == 1 << 0; } set { RIB2 = (byte)(RIB2 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Gorgeous Royal Ribbon
        public bool RIB2_1 { get { return (RIB2 & (1 << 1)) == 1 << 1; } set { RIB2 = (byte)(RIB2 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Footprint Ribbon
        public bool RIB2_2 { get { return (RIB2 & (1 << 2)) == 1 << 2; } set { RIB2 = (byte)(RIB2 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Record Ribbon
        public bool RIB2_3 { get { return (RIB2 & (1 << 3)) == 1 << 3; } set { RIB2 = (byte)(RIB2 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Event Ribbon
        public bool RIB2_4 { get { return (RIB2 & (1 << 4)) == 1 << 4; } set { RIB2 = (byte)(RIB2 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Legend Ribbon
        public bool RIB2_5 { get { return (RIB2 & (1 << 5)) == 1 << 5; } set { RIB2 = (byte)(RIB2 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // World Champion Ribbon
        public bool RIB2_6 { get { return (RIB2 & (1 << 6)) == 1 << 6; } set { RIB2 = (byte)(RIB2 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Birthday Ribbon
        public bool RIB2_7 { get { return (RIB2 & (1 << 7)) == 1 << 7; } set { RIB2 = (byte)(RIB2 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Special Ribbon
        private byte RIB3 { get { return Data[0x27]; } set { Data[0x27] = value; } } // Unova 2                                            
        public bool RIB3_0 { get { return (RIB3 & (1 << 0)) == 1 << 0; } set { RIB3 = (byte)(RIB3 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Souvenir Ribbon
        public bool RIB3_1 { get { return (RIB3 & (1 << 1)) == 1 << 1; } set { RIB3 = (byte)(RIB3 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Wishing Ribbon
        public bool RIB3_2 { get { return (RIB3 & (1 << 2)) == 1 << 2; } set { RIB3 = (byte)(RIB3 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Classic Ribbon
        public bool RIB3_3 { get { return (RIB3 & (1 << 3)) == 1 << 3; } set { RIB3 = (byte)(RIB3 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Premier Ribbon
        public bool RIB3_4 { get { return (RIB3 & (1 << 4)) == 1 << 4; } set { RIB3 = (byte)(RIB3 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Unused
        public bool RIB3_5 { get { return (RIB3 & (1 << 5)) == 1 << 5; } set { RIB3 = (byte)(RIB3 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Unused
        public bool RIB3_6 { get { return (RIB3 & (1 << 6)) == 1 << 6; } set { RIB3 = (byte)(RIB3 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Unused
        public bool RIB3_7 { get { return (RIB3 & (1 << 7)) == 1 << 7; } set { RIB3 = (byte)(RIB3 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Unused
        #endregion

        #region Block B
        public int Move1 { get { return BitConverter.ToUInt16(Data, 0x28); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x28); } }
        public int Move2 { get { return BitConverter.ToUInt16(Data, 0x2A); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2A); } }
        public int Move3 { get { return BitConverter.ToUInt16(Data, 0x2C); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2C); } }
        public int Move4 { get { return BitConverter.ToUInt16(Data, 0x2E); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E); } }
        public int Move1_PP { get { return Data[0x30]; } set { Data[0x30] = (byte)value; } }
        public int Move2_PP { get { return Data[0x31]; } set { Data[0x31] = (byte)value; } }
        public int Move3_PP { get { return Data[0x32]; } set { Data[0x32] = (byte)value; } }
        public int Move4_PP { get { return Data[0x33]; } set { Data[0x33] = (byte)value; } }
        public int Move1_PPUps { get { return Data[0x34]; } set { Data[0x34] = (byte)value; } }
        public int Move2_PPUps { get { return Data[0x35]; } set { Data[0x35] = (byte)value; } }
        public int Move3_PPUps { get { return Data[0x36]; } set { Data[0x36] = (byte)value; } }
        public int Move4_PPUps { get { return Data[0x37]; } set { Data[0x37] = (byte)value; } }
        private uint IV32 { get { return BitConverter.ToUInt32(Data, 0x38); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x38); } }
        public int IV_HP { get { return (int)(IV32 >> 00) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); } }
        public int IV_ATK { get { return (int)(IV32 >> 05) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); } }
        public int IV_DEF { get { return (int)(IV32 >> 10) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); } }
        public int IV_SPE { get { return (int)(IV32 >> 15) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); } }
        public int IV_SPA { get { return (int)(IV32 >> 20) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); } }
        public int IV_SPD { get { return (int)(IV32 >> 25) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); } }
        public bool IsEgg { get { return ((IV32 >> 30) & 1) == 1; } set { IV32 = (uint)((IV32 & ~0x40000000) | (uint)(value ? 0x40000000 : 0)); } }
        public bool IsNicknamed { get { return ((IV32 >> 31) & 1) == 1; } set { IV32 = (IV32 & 0x7FFFFFFF) | (value ? 0x80000000 : 0); } }

        private byte RIB4 { get { return Data[0x3C]; } set { Data[0x3C] = value; } } // Hoenn 1a
        public bool RIB4_0 { get { return (RIB4 & (1 << 0)) == 1 << 0; } set { RIB4 = (byte)(RIB4 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } //	Cool Ribbon
        public bool RIB4_1 { get { return (RIB4 & (1 << 1)) == 1 << 1; } set { RIB4 = (byte)(RIB4 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } //	Cool Ribbon Super
        public bool RIB4_2 { get { return (RIB4 & (1 << 2)) == 1 << 2; } set { RIB4 = (byte)(RIB4 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } //	Cool Ribbon Hyper
        public bool RIB4_3 { get { return (RIB4 & (1 << 3)) == 1 << 3; } set { RIB4 = (byte)(RIB4 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } //	Cool Ribbon Master
        public bool RIB4_4 { get { return (RIB4 & (1 << 4)) == 1 << 4; } set { RIB4 = (byte)(RIB4 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } //	Beauty Ribbon
        public bool RIB4_5 { get { return (RIB4 & (1 << 5)) == 1 << 5; } set { RIB4 = (byte)(RIB4 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } //	Beauty Ribbon Super
        public bool RIB4_6 { get { return (RIB4 & (1 << 6)) == 1 << 6; } set { RIB4 = (byte)(RIB4 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } //	Beauty Ribbon Hyper
        public bool RIB4_7 { get { return (RIB4 & (1 << 7)) == 1 << 7; } set { RIB4 = (byte)(RIB4 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } //	Beauty Ribbon Master
        private byte RIB5 { get { return Data[0x3D]; } set { Data[0x3D] = value; } } // Hoenn 1b                                           
        public bool RIB5_0 { get { return (RIB5 & (1 << 0)) == 1 << 0; } set { RIB5 = (byte)(RIB5 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } //	Cute Ribbon
        public bool RIB5_1 { get { return (RIB5 & (1 << 1)) == 1 << 1; } set { RIB5 = (byte)(RIB5 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } //	Cute Ribbon Super
        public bool RIB5_2 { get { return (RIB5 & (1 << 2)) == 1 << 2; } set { RIB5 = (byte)(RIB5 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } //	Cute Ribbon Hyper
        public bool RIB5_3 { get { return (RIB5 & (1 << 3)) == 1 << 3; } set { RIB5 = (byte)(RIB5 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } //	Cute Ribbon Master
        public bool RIB5_4 { get { return (RIB5 & (1 << 4)) == 1 << 4; } set { RIB5 = (byte)(RIB5 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } //	Smart Ribbon
        public bool RIB5_5 { get { return (RIB5 & (1 << 5)) == 1 << 5; } set { RIB5 = (byte)(RIB5 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } //	Smart Ribbon Super
        public bool RIB5_6 { get { return (RIB5 & (1 << 6)) == 1 << 6; } set { RIB5 = (byte)(RIB5 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } //	Smart Ribbon Hyper
        public bool RIB5_7 { get { return (RIB5 & (1 << 7)) == 1 << 7; } set { RIB5 = (byte)(RIB5 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } //	Smart Ribbon Master
        private byte RIB6 { get { return Data[0x3E]; } set { Data[0x3E] = value; } } // Hoenn 2a
        public bool RIB6_0 { get { return (RIB6 & (1 << 0)) == 1 << 0; } set { RIB6 = (byte)(RIB6 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } //	Tough Ribbon
        public bool RIB6_1 { get { return (RIB6 & (1 << 1)) == 1 << 1; } set { RIB6 = (byte)(RIB6 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } //	Tough Ribbon Super
        public bool RIB6_2 { get { return (RIB6 & (1 << 2)) == 1 << 2; } set { RIB6 = (byte)(RIB6 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } //	Tough Ribbon Hyper
        public bool RIB6_3 { get { return (RIB6 & (1 << 3)) == 1 << 3; } set { RIB6 = (byte)(RIB6 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } //	Tough Ribbon Master
        public bool RIB6_4 { get { return (RIB6 & (1 << 4)) == 1 << 4; } set { RIB6 = (byte)(RIB6 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } //	Champion Ribbon
        public bool RIB6_5 { get { return (RIB6 & (1 << 5)) == 1 << 5; } set { RIB6 = (byte)(RIB6 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } //	Winning Ribbon
        public bool RIB6_6 { get { return (RIB6 & (1 << 6)) == 1 << 6; } set { RIB6 = (byte)(RIB6 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } //	Victory Ribbon
        public bool RIB6_7 { get { return (RIB6 & (1 << 7)) == 1 << 7; } set { RIB6 = (byte)(RIB6 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } //	Artist Ribbon
        private byte RIB7 { get { return Data[0x3F]; } set { Data[0x3F] = value; } } // Hoenn 2b                                           
        public bool RIB7_0 { get { return (RIB7 & (1 << 0)) == 1 << 0; } set { RIB7 = (byte)(RIB7 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } //	Effort Ribbon
        public bool RIB7_1 { get { return (RIB7 & (1 << 1)) == 1 << 1; } set { RIB7 = (byte)(RIB7 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } //	Battle Champion Ribbon
        public bool RIB7_2 { get { return (RIB7 & (1 << 2)) == 1 << 2; } set { RIB7 = (byte)(RIB7 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } //	Regional Champion Ribbon
        public bool RIB7_3 { get { return (RIB7 & (1 << 3)) == 1 << 3; } set { RIB7 = (byte)(RIB7 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } //	National Champion Ribbon
        public bool RIB7_4 { get { return (RIB7 & (1 << 4)) == 1 << 4; } set { RIB7 = (byte)(RIB7 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } //	Country Ribbon
        public bool RIB7_5 { get { return (RIB7 & (1 << 5)) == 1 << 5; } set { RIB7 = (byte)(RIB7 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } //	National Ribbon
        public bool RIB7_6 { get { return (RIB7 & (1 << 6)) == 1 << 6; } set { RIB7 = (byte)(RIB7 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } //	Earth Ribbon
        public bool RIB7_7 { get { return (RIB7 & (1 << 7)) == 1 << 7; } set { RIB7 = (byte)(RIB7 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } //	World Ribbon

        public bool FatefulEncounter { get { return (Data[0x40] & 1) == 1; } set { Data[0x40] = (byte)(Data[0x40] & ~0x01 | (value ? 1 : 0)); } }
        public int Gender { get { return (Data[0x40] >> 1) & 0x3; } set { Data[0x40] = (byte)(Data[0x40] & ~0x06 | (value << 1)); } }
        public int AltForm { get { return Data[0x40] >> 3; } set { Data[0x40] = (byte)(Data[0x40] & 0x07 | (value << 3)); } }
        public int Nature { get { return Data[0x41]; } set { Data[0x41] = (byte)value; } }
        public bool HiddenAbility { get { return (Data[0x41] & 1) == 1; } set { Data[0x41] = (byte)(Data[0x41] & ~0x01 | (value ? 1 : 0)); } }
        public bool NPokémon { get { return (Data[0x41] & 2) == 2; } set { Data[0x41] = (byte)(Data[0x41] & ~0x02 | (value ? 2 : 0)); } }
        // 0x43-0x47 Unused
        #endregion

        #region Block C
        public string Nickname
        {
            get
            {
                return TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x48, 22))
                    .Replace("\uE08F", "\u2640") // nidoran
                    .Replace("\uE08E", "\u2642") // nidoran
                    .Replace("\u2019", "\u0027"); // farfetch'd
            }
            set
            {
                if (value.Length > 11)
                    value = value.Substring(0, 11); // Hard cap
                string TempNick = value // Replace Special Characters and add Terminator
                    .Replace("\u2640", "\uE08F") // nidoran
                    .Replace("\u2642", "\uE08E") // nidoran
                    .Replace("\u0027", "\u2019") // farfetch'd
                    .PadRight(value.Length + 1, (char)0xFFFF); // Null Terminator
                Encoding.Unicode.GetBytes(TempNick).CopyTo(Data, 0x48);
            }
        }
        // 0x5E unused
        public int Version { get { return Data[0x5F]; } set { Data[0x5F] = (byte)value; } }
        private byte RIB8 { get { return Data[0x60]; } set { Data[0x60] = value; } } // Sinnoh 3
        public bool RIB8_0 { get { return (RIB8 & (1 << 0)) == 1 << 0; } set { RIB8 = (byte)(RIB8 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Cool Ribbon
        public bool RIB8_1 { get { return (RIB8 & (1 << 1)) == 1 << 1; } set { RIB8 = (byte)(RIB8 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Cool Ribbon Great
        public bool RIB8_2 { get { return (RIB8 & (1 << 2)) == 1 << 2; } set { RIB8 = (byte)(RIB8 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Cool Ribbon Ultra
        public bool RIB8_3 { get { return (RIB8 & (1 << 3)) == 1 << 3; } set { RIB8 = (byte)(RIB8 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Cool Ribbon Master
        public bool RIB8_4 { get { return (RIB8 & (1 << 4)) == 1 << 4; } set { RIB8 = (byte)(RIB8 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Beauty Ribbon
        public bool RIB8_5 { get { return (RIB8 & (1 << 5)) == 1 << 5; } set { RIB8 = (byte)(RIB8 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Beauty Ribbon Great
        public bool RIB8_6 { get { return (RIB8 & (1 << 6)) == 1 << 6; } set { RIB8 = (byte)(RIB8 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Beauty Ribbon Ultra
        public bool RIB8_7 { get { return (RIB8 & (1 << 7)) == 1 << 7; } set { RIB8 = (byte)(RIB8 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Beauty Ribbon Master
        private byte RIB9 { get { return Data[0x61]; } set { Data[0x61] = value; } } // Sinnoh 4                                           
        public bool RIB9_0 { get { return (RIB9 & (1 << 0)) == 1 << 0; } set { RIB9 = (byte)(RIB9 & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Cute Ribbon
        public bool RIB9_1 { get { return (RIB9 & (1 << 1)) == 1 << 1; } set { RIB9 = (byte)(RIB9 & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Cute Ribbon Great
        public bool RIB9_2 { get { return (RIB9 & (1 << 2)) == 1 << 2; } set { RIB9 = (byte)(RIB9 & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Cute Ribbon Ultra
        public bool RIB9_3 { get { return (RIB9 & (1 << 3)) == 1 << 3; } set { RIB9 = (byte)(RIB9 & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Cute Ribbon Master
        public bool RIB9_4 { get { return (RIB9 & (1 << 4)) == 1 << 4; } set { RIB9 = (byte)(RIB9 & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Smart Ribbon
        public bool RIB9_5 { get { return (RIB9 & (1 << 5)) == 1 << 5; } set { RIB9 = (byte)(RIB9 & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Smart Ribbon Great
        public bool RIB9_6 { get { return (RIB9 & (1 << 6)) == 1 << 6; } set { RIB9 = (byte)(RIB9 & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Smart Ribbon Ultra
        public bool RIB9_7 { get { return (RIB9 & (1 << 7)) == 1 << 7; } set { RIB9 = (byte)(RIB9 & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Smart Ribbon Master
        private byte RIBA { get { return Data[0x62]; } set { Data[0x62] = value; } } // Sinnoh 5
        public bool RIBA_0 { get { return (RIBA & (1 << 0)) == 1 << 0; } set { RIBA = (byte)(RIBA & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Tough Ribbon
        public bool RIBA_1 { get { return (RIBA & (1 << 1)) == 1 << 1; } set { RIBA = (byte)(RIBA & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Tough Ribbon Great
        public bool RIBA_2 { get { return (RIBA & (1 << 2)) == 1 << 2; } set { RIBA = (byte)(RIBA & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Tough Ribbon Ultra
        public bool RIBA_3 { get { return (RIBA & (1 << 3)) == 1 << 3; } set { RIBA = (byte)(RIBA & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Tough Ribbon Master
        public bool RIBA_4 { get { return (RIBA & (1 << 4)) == 1 << 4; } set { RIBA = (byte)(RIBA & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Unused
        public bool RIBA_5 { get { return (RIBA & (1 << 5)) == 1 << 5; } set { RIBA = (byte)(RIBA & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Unused
        public bool RIBA_6 { get { return (RIBA & (1 << 6)) == 1 << 6; } set { RIBA = (byte)(RIBA & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Unused
        public bool RIBA_7 { get { return (RIBA & (1 << 7)) == 1 << 7; } set { RIBA = (byte)(RIBA & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Unused
        private byte RIBB { get { return Data[0x63]; } set { Data[0x63] = value; } } // Sinnoh 6
        public bool RIBB_0 { get { return (RIBB & (1 << 0)) == 1 << 0; } set { RIBB = (byte)(RIBB & ~(1 << 0) | (value ? 1 << 0 : 0)); } } // Unused
        public bool RIBB_1 { get { return (RIBB & (1 << 1)) == 1 << 1; } set { RIBB = (byte)(RIBB & ~(1 << 1) | (value ? 1 << 1 : 0)); } } // Unused
        public bool RIBB_2 { get { return (RIBB & (1 << 2)) == 1 << 2; } set { RIBB = (byte)(RIBB & ~(1 << 2) | (value ? 1 << 2 : 0)); } } // Unused
        public bool RIBB_3 { get { return (RIBB & (1 << 3)) == 1 << 3; } set { RIBB = (byte)(RIBB & ~(1 << 3) | (value ? 1 << 3 : 0)); } } // Unused
        public bool RIBB_4 { get { return (RIBB & (1 << 4)) == 1 << 4; } set { RIBB = (byte)(RIBB & ~(1 << 4) | (value ? 1 << 4 : 0)); } } // Unused
        public bool RIBB_5 { get { return (RIBB & (1 << 5)) == 1 << 5; } set { RIBB = (byte)(RIBB & ~(1 << 5) | (value ? 1 << 5 : 0)); } } // Unused
        public bool RIBB_6 { get { return (RIBB & (1 << 6)) == 1 << 6; } set { RIBB = (byte)(RIBB & ~(1 << 6) | (value ? 1 << 6 : 0)); } } // Unused
        public bool RIBB_7 { get { return (RIBB & (1 << 7)) == 1 << 7; } set { RIBB = (byte)(RIBB & ~(1 << 7) | (value ? 1 << 7 : 0)); } } // Unused
        // 0x64-0x67 Unused
        #endregion

        #region Block D
        public string OT_Name
        {
            get
            {
                return TrimFromFFFF(Encoding.Unicode.GetString(Data, 0x68, 16))
                    .Replace("\uE08F", "\u2640") // Nidoran ♂
                    .Replace("\uE08E", "\u2642") // Nidoran ♀
                    .Replace("\u2019", "\u0027"); // farfetch'd
            }
            set
            {
                if (value.Length > 7)
                    value = value.Substring(0, 7); // Hard cap
                string TempNick = value // Replace Special Characters and add Terminator
                .Replace("\u2640", "\uE08F") // Nidoran ♂
                .Replace("\u2642", "\uE08E") // Nidoran ♀
                .Replace("\u0027", "\u2019") // Farfetch'd
                .PadRight(value.Length + 1, (char)0xFFFF); // Null Terminator
                Encoding.Unicode.GetBytes(TempNick).CopyTo(Data, 0x68);
            }
        }
        public int Egg_Year { get { return Data[0x78]; } set { Data[0x78] = (byte)value; } }
        public int Egg_Month { get { return Data[0x79]; } set { Data[0x79] = (byte)value; } }
        public int Egg_Day { get { return Data[0x7A]; } set { Data[0x7A] = (byte)value; } }
        public int Met_Year { get { return Data[0x7B]; } set { Data[0x7B] = (byte)value; } }
        public int Met_Month { get { return Data[0x7C]; } set { Data[0x7C] = (byte)value; } }
        public int Met_Day { get { return Data[0x7D]; } set { Data[0x7D] = (byte)value; } }
        public int Egg_Location { get { return BitConverter.ToUInt16(Data, 0x7E); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x7E); } }
        public int Met_Location { get { return BitConverter.ToUInt16(Data, 0x80); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x80); } }
        private byte PKRS { get { return Data[0x82]; } set { Data[0x82] = value; } }
        public int PKRS_Days { get { return PKRS & 0xF; } set { PKRS = (byte)(PKRS & ~0xF | value); } }
        public int PKRS_Strain { get { return PKRS >> 4; } set { PKRS = (byte)(PKRS & 0xF | (value << 4)); } }
        public int Ball { get { return Data[0x83]; } set { Data[0x83] = (byte)value; } }
        public int Met_Level { get { return Data[0x84] & ~0x80; } set { Data[0x84] = (byte)((Data[0x84] & 0x80) | value); } }
        public int OT_Gender { get { return Data[0x84] >> 7; } set { Data[0x84] = (byte)((Data[0x84] & ~0x80) | value << 7); } }
        public int EncounterType { get { return Data[0x85]; } set { Data[0x85] = (byte)value; } }
        // 0x86-0x87 Unused
        #endregion

        // Simple Generated Attributes
        public int[] IVs
        {
            get { return new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD }; }
            set
            {
                if (value == null || value.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }
        public int[] EVs => new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD };
        public int PSV => (int)((PID >> 16 ^ PID & 0xFFFF) >> 3);
        public int TSV => (TID ^ SID) >> 3;
        public bool IsShiny => TSV == PSV;
        public bool PKRS_Infected => PKRS_Strain > 0;
        public bool PKRS_Cured => PKRS_Days == 0 && PKRS_Strain > 0;
        public bool Gen5 => Version >= 20 && Version <= 23;
        public bool Gen4 => Version >= 10 && Version < 12 || Version >= 7 && Version <= 8;
        public bool Gen3 => Version >= 1 && Version <= 5 || Version == 15;
        public bool GenU => !(Gen5 || Gen4 || Gen3);

        public int[] Moves { 
            get { return new[] {Move1, Move2, Move3, Move4}; }
            set { 
                if (value.Length > 0) Move1 = value[0]; 
                if (value.Length > 1) Move2 = value[1]; 
                if (value.Length > 2) Move3 = value[2]; 
                if (value.Length > 3) Move4 = value[3]; } }

        // Complex Generated Attributes
        public byte[] EncryptedPartyData => Encrypt().Take(SIZE_PARTY).ToArray();
        public byte[] EncryptedBoxData => Encrypt().Take(SIZE_STORED).ToArray();
        public byte[] DecryptedPartyData => Data.Take(SIZE_PARTY).ToArray();
        public byte[] DecryptedBoxData => Data.Take(SIZE_STORED).ToArray();
        public int Characteristic
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
        public int PotentialRating
        {
            get
            {
                int ivTotal = IVs.Sum();
                if (ivTotal <= 90)
                    return 0;
                if (ivTotal <= 120)
                    return 1;
                return ivTotal <= 150 ? 2 : 3;
            }
        }

        // Methods
        public void RefreshChecksum()
        {
            Checksum = CalculateChecksum();
        }
        public ushort CalculateChecksum()
        {
            ushort chk = 0;
            for (int i = 8; i < SIZE_STORED; i += 2) // Loop through the entire PK5
                chk += BitConverter.ToUInt16(Data, i);

            return chk;
        }
        public byte[] Write()
        {
            RefreshChecksum();
            return Data;
        }
        public bool getGenderIsValid()
        {
            int gv = PKX.Personal[Species].Gender;
            if (gv == 255 && Gender == 2)
                return true;
            if (gv == 0 && Gender == 1)
                return true;
            if (gv == 254 && Gender == 0)
                return true;
            if (gv <= (PID & 0xFF) && Gender == 0)
                return true;
            if (gv > (PID & 0xFF) && Gender == 1)
                return true;
            return false;
        }
        public void FixMoves()
        {
            if (Move4 != 0 && Move3 == 0)
            {
                Move3 = Move4;
                Move3_PP = Move4_PP;
                Move3_PPUps = Move4_PPUps;
                Move4 = Move4_PP = Move4_PPUps = 0;
            }
            if (Move3 != 0 && Move2 == 0)
            {
                Move2 = Move3;
                Move2_PP = Move3_PP;
                Move2_PPUps = Move3_PPUps;
                Move3 = Move3_PP = Move3_PPUps = 0;
            }
            if (Move2 != 0 && Move1 == 0)
            {
                Move1 = Move2;
                Move1_PP = Move2_PP;
                Move1_PPUps = Move2_PPUps;
                Move2 = Move2_PP = Move2_PPUps = 0;
            }
        }
        public byte[] Encrypt()
        {
            Checksum = CalculateChecksum();
            return encryptArray(Data);
        }

        public PK6 convertToPK6()
        {
            PK6 pk6 = new PK6 // Convert away!
            {
                EncryptionConstant = PID,
                Species = Species,
                TID = TID,
                SID = SID,
                EXP = EXP,
                PID = PID,
                Ability = Ability
            };

            int abilnum = PKX.getAbilityNumber(Species, Ability, AltForm);
            if (abilnum > 0) pk6.AbilityNumber = abilnum;
            else // Fallback (shouldn't happen)
            {
                if (HiddenAbility) pk6.AbilityNumber = 4; // Hidden, else G5 or G3/4 correlation.
                else pk6.AbilityNumber = Gen5 ? 1 << (int)(PID >> 16 & 1) : 1 << (int)(PID & 1);
            }
            pk6.Circle = Circle;
            pk6.Square = Square;
            pk6.Triangle = Triangle;
            pk6.Heart = Heart;
            pk6.Star = Star;
            pk6.Diamond = Diamond;
            pk6.Language = Language;

            pk6.CNT_Cool = CNT_Cool;
            pk6.CNT_Beauty = CNT_Beauty;
            pk6.CNT_Cute = CNT_Cute;
            pk6.CNT_Smart = CNT_Smart;
            pk6.CNT_Tough = CNT_Tough;

            // Cap EVs
            pk6.EV_HP = EV_HP > 252 ? 252 : EV_HP;
            pk6.EV_ATK = EV_ATK > 252 ? 252 : EV_ATK;
            pk6.EV_DEF = EV_DEF > 252 ? 252 : EV_DEF;
            pk6.EV_SPA = EV_SPA > 252 ? 252 : EV_SPA;
            pk6.EV_SPD = EV_SPD > 252 ? 252 : EV_SPD;
            pk6.EV_SPE = EV_SPE > 252 ? 252 : EV_SPE;

            pk6.Move1 = Move1;
            pk6.Move2 = Move2;
            pk6.Move3 = Move3;
            pk6.Move4 = Move4;

            pk6.Move1_PP = PKX.getMovePP(Move1, Move1_PPUps);
            pk6.Move2_PP = PKX.getMovePP(Move2, Move2_PPUps);
            pk6.Move3_PP = PKX.getMovePP(Move3, Move3_PPUps);
            pk6.Move4_PP = PKX.getMovePP(Move4, Move4_PPUps);

            pk6.Move1_PPUps = Move1_PPUps;
            pk6.Move2_PPUps = Move2_PPUps;
            pk6.Move3_PPUps = Move3_PPUps;
            pk6.Move4_PPUps = Move4_PPUps;

            pk6.IV_HP = IV_HP;
            pk6.IV_ATK = IV_ATK;
            pk6.IV_DEF = IV_DEF;
            pk6.IV_SPA = IV_SPA;
            pk6.IV_SPD = IV_SPD;
            pk6.IV_SPE = IV_SPE;
            pk6.IsEgg = IsEgg;
            pk6.IsNicknamed = IsNicknamed;

            pk6.FatefulEncounter = FatefulEncounter;
            pk6.Gender = Gender;
            pk6.AltForm = AltForm;
            pk6.Nature = Nature;

            pk6.Nickname = Nickname.Length > 1 && !IsNicknamed
                ? Nickname[0] + Nickname.Substring(1).ToLower() // Decapitalize
                : Nickname;

            pk6.Version = Version;

            pk6.OT_Name = OT_Name;

            // Dates are kept upon transfer
            pk6.Met_Year = Met_Year;
            pk6.Met_Month = Met_Month;
            pk6.Met_Day = Met_Day;
            pk6.Egg_Year = Egg_Year;
            pk6.Egg_Month = Egg_Month;
            pk6.Egg_Day = Egg_Day;

            // Locations are kept upon transfer
            pk6.Met_Location = Met_Location;
            pk6.Egg_Location = Egg_Location;

            pk6.PKRS_Strain = PKRS_Strain;
            pk6.PKRS_Days = PKRS_Days;
            pk6.Ball = Ball;

            // OT Gender & Encounter Level
            pk6.Met_Level = Met_Level;
            pk6.OT_Gender = OT_Gender;
            pk6.EncounterType = EncounterType;
            
            // Ribbon Decomposer (Contest & Battle)
            byte contestribbons = 0;
            byte battleribbons = 0;

            // Contest Ribbon Counter
            for (int i = 0; i < 8; i++) // Sinnoh 3, Hoenn 1
            {
                if (((Data[0x60] >> i) & 1) == 1) contestribbons++;
                if (((Data[0x61] >> i) & 1) == 1) contestribbons++;
                if (((Data[0x3C] >> i) & 1) == 1) contestribbons++;
                if (((Data[0x3D] >> i) & 1) == 1) contestribbons++;
            }
            for (int i = 0; i < 4; i++) // Sinnoh 4, Hoenn 2
            {
                if (((Data[0x62] >> i) & 1) == 1) contestribbons++;
                if (((Data[0x3E] >> i) & 1) == 1) contestribbons++;
            }

            // Battle Ribbon Counter
            // Winning Ribbon
            if ((Data[0x3E] & 0x20) >> 5 == 1) battleribbons++;
            // Victory Ribbon
            if ((Data[0x3E] & 0x40) >> 6 == 1) battleribbons++;
            for (int i = 1; i < 7; i++)     // Sinnoh Battle Ribbons
                if (((Data[0x24] >> i) & 1) == 1) battleribbons++;

            // Fill the Ribbon Counter Bytes
            pk6.Memory_ContestCount = contestribbons;
            pk6.Memory_BattleCount = battleribbons;

            // Copy Ribbons to their new locations.
            int bx30 = 0;
            // bx30 |= 0;                             // Kalos Champ - New Kalos Ribbon
            bx30 |= ((Data[0x3E] & 0x10) >> 4) << 1; // Hoenn Champion
            bx30 |= ((Data[0x24] & 0x01) >> 0) << 2; // Sinnoh Champ
            // bx30 |= 0;                             // Best Friend - New Kalos Ribbon
            // bx30 |= 0;                             // Training    - New Kalos Ribbon
            // bx30 |= 0;                             // Skillful    - New Kalos Ribbon
            // bx30 |= 0;                             // Expert      - New Kalos Ribbon
            bx30 |= ((Data[0x3F] & 0x01) >> 0) << 7; // Effort Ribbon
            pk6.Data[0x30] = (byte)bx30;

            int bx31 = 0;
            bx31 |= ((Data[0x24] & 0x80) >> 7) << 0;  // Alert
            bx31 |= ((Data[0x25] & 0x01) >> 0) << 1;  // Shock
            bx31 |= ((Data[0x25] & 0x02) >> 1) << 2;  // Downcast
            bx31 |= ((Data[0x25] & 0x04) >> 2) << 3;  // Careless
            bx31 |= ((Data[0x25] & 0x08) >> 3) << 4;  // Relax
            bx31 |= ((Data[0x25] & 0x10) >> 4) << 5;  // Snooze
            bx31 |= ((Data[0x25] & 0x20) >> 5) << 6;  // Smile
            bx31 |= ((Data[0x25] & 0x40) >> 6) << 7;  // Gorgeous
            pk6.Data[0x31] = (byte)bx31;

            int bx32 = 0;
            bx32 |= ((Data[0x25] & 0x80) >> 7) << 0;  // Royal
            bx32 |= ((Data[0x26] & 0x01) >> 0) << 1;  // Gorgeous Royal
            bx32 |= ((Data[0x3E] & 0x80) >> 7) << 2;  // Artist
            bx32 |= ((Data[0x26] & 0x02) >> 1) << 3;  // Footprint
            bx32 |= ((Data[0x26] & 0x04) >> 2) << 4;  // Record
            bx32 |= ((Data[0x26] & 0x10) >> 4) << 5;  // Legend
            bx32 |= ((Data[0x3F] & 0x10) >> 4) << 6;  // Country
            bx32 |= ((Data[0x3F] & 0x20) >> 5) << 7;  // National
            pk6.Data[0x32] = (byte)bx32;

            int bx33 = 0;
            bx33 |= ((Data[0x3F] & 0x40) >> 6) << 0;  // Earth
            bx33 |= ((Data[0x3F] & 0x80) >> 7) << 1;  // World
            bx33 |= ((Data[0x27] & 0x04) >> 2) << 2;  // Classic
            bx33 |= ((Data[0x27] & 0x08) >> 3) << 3;  // Premier
            bx33 |= ((Data[0x26] & 0x08) >> 3) << 4;  // Event
            bx33 |= ((Data[0x26] & 0x40) >> 6) << 5;  // Birthday
            bx33 |= ((Data[0x26] & 0x80) >> 7) << 6;  // Special
            bx33 |= ((Data[0x27] & 0x01) >> 0) << 7;  // Souvenir
            pk6.Data[0x33] = (byte)bx33;

            int bx34 = 0;
            bx34 |= ((Data[0x27] & 0x02) >> 1) << 0;  // Wishing Ribbon
            bx34 |= ((Data[0x3F] & 0x02) >> 1) << 1;  // Battle Champion
            bx34 |= ((Data[0x3F] & 0x04) >> 2) << 2;  // Regional Champion
            bx34 |= ((Data[0x3F] & 0x08) >> 3) << 3;  // National Champion
            bx34 |= ((Data[0x26] & 0x20) >> 5) << 4;  // World Champion
            pk6.Data[0x34] = (byte)bx34;
            
            // Write Transfer Location - location is dependent on 3DS system that transfers.
            pk6.Country = Converter.Country;
            pk6.Region = Converter.Region;
            pk6.ConsoleRegion = Converter.ConsoleRegion;

            // Write the Memories, Friendship, and Origin!
            pk6.CurrentHandler = 1;
            pk6.HT_Name = Converter.OT_Name;
            pk6.HT_Gender = Converter.OT_Gender;
            pk6.Geo1_Region = Converter.Region;
            pk6.Geo1_Country = Converter.Country;
            pk6.HT_Intensity = 1;
            pk6.HT_Memory = 4;
            pk6.HT_Feeling = (int)(Util.rnd32() % 10);
            // When transferred, friendship gets reset.
            pk6.OT_Friendship = pk6.HT_Friendship = PKX.getBaseFriendship(Species);

            // Antishiny Mechanism
            ushort LID = (ushort)(PID & 0xFFFF);
            ushort HID = (ushort)(PID >> 0x10);

            int XOR = TID ^ SID ^ LID ^ HID;
            if (XOR >= 8 && XOR < 16) // If we get an illegal collision...
                pk6.PID ^= 0x80000000;

            // HMs are not deleted 5->6, transfer away (but fix if blank spots?)
            pk6.FixMoves();

            // Decapitalize
            if (!pk6.IsNicknamed && pk6.Nickname.Length > 1)
                pk6.Nickname = char.ToUpper(pk6.Nickname[0]) + pk6.Nickname.Substring(1).ToLower();

            // Fix Name Strings
            pk6.Nickname = pk6.Nickname
                .Replace('\u2467', '\u00d7') // ×
                .Replace('\u2468', '\u00f7') // ÷
                .Replace('\u246c', '\u2026') // …

                .Replace('\u246d', '\uE08E') // ♂
                .Replace('\u246e', '\uE08F') // ♀
                .Replace('\u246f', '\uE090') // ♠
                .Replace('\u2470', '\uE091') // ♣
                .Replace('\u2471', '\uE092') // ♥
                .Replace('\u2472', '\uE093') // ♦
                .Replace('\u2473', '\uE094') // ★
                .Replace('\u2474', '\uE095') // ◎

                .Replace('\u2475', '\uE096') // ○
                .Replace('\u2476', '\uE097') // □
                .Replace('\u2477', '\uE098') // △
                .Replace('\u2478', '\uE099') // ◇
                .Replace('\u2479', '\uE09A') // ♪
                .Replace('\u247a', '\uE09B') // ☀
                .Replace('\u247b', '\uE09C') // ☁
                .Replace('\u247d', '\uE09D') // ☂
                ;

            pk6.OT_Name = pk6.OT_Name
                .Replace('\u2467', '\u00d7') // ×
                .Replace('\u2468', '\u00f7') // ÷
                .Replace('\u246c', '\u2026') // …

                .Replace('\u246d', '\uE08E') // ♂
                .Replace('\u246e', '\uE08F') // ♀
                .Replace('\u246f', '\uE090') // ♠
                .Replace('\u2470', '\uE091') // ♣
                .Replace('\u2471', '\uE092') // ♥
                .Replace('\u2472', '\uE093') // ♦
                .Replace('\u2473', '\uE094') // ★
                .Replace('\u2474', '\uE095') // ◎

                .Replace('\u2475', '\uE096') // ○
                .Replace('\u2476', '\uE097') // □
                .Replace('\u2477', '\uE098') // △
                .Replace('\u2478', '\uE099') // ◇
                .Replace('\u2479', '\uE09A') // ♪
                .Replace('\u247a', '\uE09B') // ☀
                .Replace('\u247b', '\uE09C') // ☁
                .Replace('\u247d', '\uE09D') // ☂
                ;

            // Fix Checksum
            pk6.RefreshChecksum();

            return pk6; // Done!
        }
    }
}
