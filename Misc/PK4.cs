using System;
using System.Linq;

namespace PKHeX
{
    public class PK4 // 4th Generation PKM File
    {
        internal const int SIZE_PARTY = 236;
        internal const int SIZE_STORED = 136;
        internal const int SIZE_BLOCK = 32;

        public PK4(byte[] decryptedData = null, string ident = null)
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
        public uint IV32 { get { return BitConverter.ToUInt32(Data, 0x38); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x38); } }
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
        // 0x43-0x47 Unused
        #endregion

        #region Block C
        public string Nickname
        {
            get
            {
                return PKM.array2strG4(Data.Skip(0x48).Take(22).ToArray())
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
                    .Replace("\u0027", "\u2019"); // farfetch'd
                PKM.str2arrayG4(TempNick).CopyTo(Data, 0x48);
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
                return PKM.array2strG4(Data.Skip(0x68).Take(16).ToArray())
                    .Replace("\uE08F", "\u2640") // Nidoran ♂
                    .Replace("\uE08E", "\u2642") // Nidoran ♀
                    .Replace("\u2019", "\u0027"); // Farfetch'd
            }
            set
            {
                if (value.Length > 7)
                    value = value.Substring(0, 7); // Hard cap
                string TempNick = value // Replace Special Characters and add Terminator
                .Replace("\u2640", "\uE08F") // Nidoran ♂
                .Replace("\u2642", "\uE08E") // Nidoran ♀
                .Replace("\u0027", "\u2019"); // Farfetch'd
                PKM.str2arrayG4(TempNick).CopyTo(Data, 0x68);
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
        public int HGSSBall { get { return Data[0x86]; } set { Data[0x86] = (byte)value; } }
        // Unused 0x87
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
        public bool Gen4 => Version >= 10 && Version < 12 || Version >= 7 && Version <= 8;
        public bool Gen3 => Version >= 1 && Version <= 5 || Version == 15;
        public bool GenU => !(Gen4 || Gen3);

        public int[] Moves
        {
            get { return new[] { Move1, Move2, Move3, Move4 }; }
            set
            {
                if (value.Length > 0) Move1 = value[0];
                if (value.Length > 1) Move2 = value[1];
                if (value.Length > 2) Move3 = value[2];
                if (value.Length > 3) Move4 = value[3];
            }
        }

        // Complex Generated Attributes
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
            for (int i = 8; i < SIZE_STORED; i += 2) // Loop through the entire PK6
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

        public PK5 convertToPK5()
        {
            // Double Check Location Data to see if we're already a PK5
            if (Data[0x5F] < 0x10 && BitConverter.ToUInt16(Data, 0x80) > 0x4000)
                return new PK5(Data);

            DateTime moment = DateTime.Now;

            PK5 pk5 = new PK5(Data) // Convert away!
            {
                HeldItem = 0,
                Friendship = 70,
                // Apply new met date
                Met_Year = moment.Year - 2000,
                Met_Month = moment.Month,
                Met_Day = moment.Day
            };

            // Fix PP
            pk5.Move1_PP = PKX.getMovePP(pk5.Move1_PP, pk5.Move1_PPUps);
            pk5.Move2_PP = PKX.getMovePP(pk5.Move2_PP, pk5.Move2_PPUps);
            pk5.Move3_PP = PKX.getMovePP(pk5.Move3_PP, pk5.Move3_PPUps);
            pk5.Move4_PP = PKX.getMovePP(pk5.Move4_PP, pk5.Move4_PPUps);

            // Disassociate Nature and PID
            pk5.Nature = (int)(pk5.PID % 0x19);

            // Delete Platinum/HGSS Met Location Data
            BitConverter.GetBytes((uint)0).CopyTo(pk5.Data, 0x44);

            // Met / Crown Data Detection
            pk5.Met_Location = pk5.FatefulEncounter && Array.IndexOf(new[] {251, 243, 244, 245}, pk5.Species) >= 0
                ? (pk5.Species == 251 ? 30010 : 30012) // Celebi : Beast
                : 30001; // Pokétransfer (not Crown)
            
            // Delete HGSS Data
            BitConverter.GetBytes((ushort)0).CopyTo(pk5.Data, 0x86);
            if (HGSSBall > 0 && HGSSBall != 4)
                pk5.Ball = HGSSBall;

            // Transfer Nickname and OT Name
            pk5.Nickname = Nickname;
            pk5.OT_Name = OT_Name;

            // Fix Level
            pk5.Met_Level = PKX.getLevel(pk5.Species, pk5.EXP);

            // Remove HM moves; Defog should be kept if both are learned.
            int[] banned = Moves.Contains(250) /*Whirlpool*/ && !Moves.Contains(432) /*Defog*/
                ? new[] {15, 19, 57, 70, 432, 249, 127, 431} // No Defog
                : new[] {15, 19, 57, 70, 250, 249, 127, 431};// No Whirlpool

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
