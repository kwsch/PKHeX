using System;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public class PK6 : PKX
    {
        public PK6(byte[] pkx, string ident = null)
        {
            Data = (byte[])pkx.Clone();
            Identifier = ident;
            if (Data.Length != 260)
                Array.Resize(ref Data, 260);
        }

        // Internal Attributes set on creation
        public byte[] Data; // Raw Storage
        public string Identifier; // User or Form Custom Attribute

        // Structure
        #region Block A
        public uint EncryptionConstant
        {
            get { return BitConverter.ToUInt32(Data, 0x00); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x00, 4); }
        }
        public ushort Sanity
        {
            get { return BitConverter.ToUInt16(Data, 0x02); }
            // set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x04, 2); }
        }
        public ushort Checksum
        {
            get { return BitConverter.ToUInt16(Data, 0x06); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x06, 2); }
        }
        public int Species
        {
            get { return BitConverter.ToUInt16(Data, 0x08); }
            set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0x08, 2); }
        }
        public int HeldItem
        {
            get { return BitConverter.ToUInt16(Data, 0x0A); }
            set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0x0A, 2); }
        }
        public int TID
        {
            get { return BitConverter.ToUInt16(Data, 0x0C); }
            set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0x0C, 2); }
        }
        public int SID
        {
            get { return BitConverter.ToUInt16(Data, 0x0E); }
            set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0x0E, 2); }
        }
        public uint EXP
        {
            get { return BitConverter.ToUInt32(Data, 0x10); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x10, 4); }
        }
        public int Ability { get { return Data[0x14]; } set { Data[0x14] = (byte)value; } }
        public int AbilityNumber { get { return Data[0x15]; } set { Data[0x15] = (byte)value; } }
        public int TrainingBagHits { get { return Data[0x16]; } set { Data[0x16] = (byte)value; } }
        public int TrainingBag { get { return Data[0x17]; } set { Data[0x17] = (byte)value; } } 
        public uint PID
        {
            get { return BitConverter.ToUInt32(Data, 0x18); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x18, 4); }
        }
        public int Nature { get { return Data[0x1C]; } set { Data[0x1C] = (byte)value; } }
        public bool FatefulEncounter { get { return (Data[0x1D] & 1) == 1; } set { Data[0x1D] = (byte)(Data[0x1D] & ~0x01 | (value ? 1 : 0)); } }
        public int Gender { get { return (Data[0x1D] >> 1) & 0x3; } set { Data[0x1D] = (byte)(Data[0x1D] & ~0x06 | (value << 1)); } }
        public int AltForm { get { return Data[0x1D] >> 3; } set { Data[0x1D] = (byte)(Data[0x1D] & 0x07 | (value << 3)); } }
        public int EV_HP { get { return Data[0x1E]; } set { Data[0x1E] = (byte)value; } }
        public int EV_ATK { get { return Data[0x1F]; } set { Data[0x1F] = (byte)value; } }
        public int EV_DEF { get { return Data[0x20]; } set { Data[0x20] = (byte)value; } }
        public int EV_SPE { get { return Data[0x21]; } set { Data[0x21] = (byte)value; } }
        public int EV_SPA { get { return Data[0x22]; } set { Data[0x22] = (byte)value; } }
        public int EV_SPD { get { return Data[0x23]; } set { Data[0x23] = (byte)value; } }
        public int CNT_Cool { get { return Data[0x24]; } set { Data[0x24] = (byte)value; } }
        public int CNT_Beauty { get { return Data[0x25]; } set { Data[0x25] = (byte)value; } }
        public int CNT_Cute { get { return Data[0x26]; } set { Data[0x26] = (byte)value; } }
        public int CNT_Smart { get { return Data[0x27]; } set { Data[0x27] = (byte)value; } }
        public int CNT_Tough { get { return Data[0x28]; } set { Data[0x28] = (byte)value; } }
        public int CNT_Sheen { get { return Data[0x29]; } set { Data[0x29] = (byte)value; } }
        public byte Markings { get { return Data[0x2A]; } set { Data[0x2A] = value; } }
        public bool Circle { get { return (Markings & (1 << 0)) == 1 << 0; } set { Markings = (byte)(Markings & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool Triangle { get { return (Markings & (1 << 1)) == 1 << 1; } set { Markings = (byte)(Markings & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool Square { get { return (Markings & (1 << 2)) == 1 << 2; } set { Markings = (byte)(Markings & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool Heart { get { return (Markings & (1 << 3)) == 1 << 3; } set { Markings = (byte)(Markings & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool Star { get { return (Markings & (1 << 4)) == 1 << 4; } set { Markings = (byte)(Markings & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool Diamond { get { return (Markings & (1 << 5)) == 1 << 5; } set { Markings = (byte)(Markings & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        private byte PKRS { get { return Data[0x2B]; } set { Data[0x2B] = value; } }
        public int PKRS_Days { get { return PKRS & 0xF; } set { PKRS = (byte)(PKRS & ~0xF | value); } }
        public int PKRS_Strain { get { return PKRS >> 4; } set { PKRS = (byte)(PKRS & 0xF | (value << 4)); } }
        private byte ST1 { get { return Data[0x2C]; } set { Data[0x2C] = value; } }
        public bool Unused0 { get { return (ST1 & (1 << 0)) == 1 << 0; } set { ST1 = (byte)(ST1 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool Unused1 { get { return (ST1 & (1 << 1)) == 1 << 1; } set { ST1 = (byte)(ST1 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool ST1_SPA { get { return (ST1 & (1 << 2)) == 1 << 2; } set { ST1 = (byte)(ST1 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool ST1_HP  { get { return (ST1 & (1 << 3)) == 1 << 3; } set { ST1 = (byte)(ST1 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool ST1_ATK { get { return (ST1 & (1 << 4)) == 1 << 4; } set { ST1 = (byte)(ST1 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool ST1_SPD { get { return (ST1 & (1 << 5)) == 1 << 5; } set { ST1 = (byte)(ST1 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool ST1_SPE { get { return (ST1 & (1 << 6)) == 1 << 6; } set { ST1 = (byte)(ST1 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool ST1_DEF { get { return (ST1 & (1 << 7)) == 1 << 7; } set { ST1 = (byte)(ST1 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte ST2 { get { return Data[0x2D]; } set { Data[0x2D] = value; } }
        public bool ST2_SPA { get { return (ST2 & (1 << 0)) == 1 << 0; } set { ST2 = (byte)(ST2 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool ST2_HP  { get { return (ST2 & (1 << 1)) == 1 << 1; } set { ST2 = (byte)(ST2 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool ST2_ATK { get { return (ST2 & (1 << 2)) == 1 << 2; } set { ST2 = (byte)(ST2 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool ST2_SPD { get { return (ST2 & (1 << 3)) == 1 << 3; } set { ST2 = (byte)(ST2 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool ST2_SPE { get { return (ST2 & (1 << 4)) == 1 << 4; } set { ST2 = (byte)(ST2 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool ST2_DEF { get { return (ST2 & (1 << 5)) == 1 << 5; } set { ST2 = (byte)(ST2 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool ST3_SPA { get { return (ST2 & (1 << 6)) == 1 << 6; } set { ST2 = (byte)(ST2 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool ST3_HP  { get { return (ST2 & (1 << 7)) == 1 << 7; } set { ST2 = (byte)(ST2 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte ST3 { get { return Data[0x2E]; } set { Data[0x2E] = value; } }
        public bool ST3_ATK { get { return (ST3 & (1 << 0)) == 1 << 0; } set { ST3 = (byte)(ST3 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool ST3_SPD { get { return (ST3 & (1 << 1)) == 1 << 1; } set { ST3 = (byte)(ST3 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool ST3_SPE { get { return (ST3 & (1 << 2)) == 1 << 2; } set { ST3 = (byte)(ST3 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool ST3_DEF { get { return (ST3 & (1 << 3)) == 1 << 3; } set { ST3 = (byte)(ST3 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool ST4_1 { get { return (ST3 & (1 << 4)) == 1 << 4; } set { ST3 = (byte)(ST3 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool ST5_1 { get { return (ST3 & (1 << 5)) == 1 << 5; } set { ST3 = (byte)(ST3 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool ST5_2 { get { return (ST3 & (1 << 6)) == 1 << 6; } set { ST3 = (byte)(ST3 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool ST5_3 { get { return (ST3 & (1 << 7)) == 1 << 7; } set { ST3 = (byte)(ST3 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte ST4 { get { return Data[0x2F]; } set { Data[0x2F] = value; } }
        public bool ST5_4 { get { return (ST4 & (1 << 0)) == 1 << 0; } set { ST4 = (byte)(ST4 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool ST6_1 { get { return (ST4 & (1 << 1)) == 1 << 1; } set { ST4 = (byte)(ST4 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool ST6_2 { get { return (ST4 & (1 << 2)) == 1 << 2; } set { ST4 = (byte)(ST4 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool ST6_3 { get { return (ST4 & (1 << 3)) == 1 << 3; } set { ST4 = (byte)(ST4 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool ST7_1 { get { return (ST4 & (1 << 4)) == 1 << 4; } set { ST4 = (byte)(ST4 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool ST7_2 { get { return (ST4 & (1 << 5)) == 1 << 5; } set { ST4 = (byte)(ST4 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool ST7_3 { get { return (ST4 & (1 << 6)) == 1 << 6; } set { ST4 = (byte)(ST4 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool ST8_1 { get { return (ST4 & (1 << 7)) == 1 << 7; } set { ST4 = (byte)(ST4 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte RIB0 { get { return Data[0x30]; } set { Data[0x30] = value; } }
        public bool RIB0_0 { get { return (RIB0 & (1 << 0)) == 1 << 0; } set { RIB0 = (byte)(RIB0 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool RIB0_1 { get { return (RIB0 & (1 << 1)) == 1 << 1; } set { RIB0 = (byte)(RIB0 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool RIB0_2 { get { return (RIB0 & (1 << 2)) == 1 << 2; } set { RIB0 = (byte)(RIB0 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool RIB0_3 { get { return (RIB0 & (1 << 3)) == 1 << 3; } set { RIB0 = (byte)(RIB0 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool RIB0_4 { get { return (RIB0 & (1 << 4)) == 1 << 4; } set { RIB0 = (byte)(RIB0 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool RIB0_5 { get { return (RIB0 & (1 << 5)) == 1 << 5; } set { RIB0 = (byte)(RIB0 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool RIB0_6 { get { return (RIB0 & (1 << 6)) == 1 << 6; } set { RIB0 = (byte)(RIB0 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool RIB0_7 { get { return (RIB0 & (1 << 7)) == 1 << 7; } set { RIB0 = (byte)(RIB0 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte RIB1 { get { return Data[0x31]; } set { Data[0x31] = value; } }
        public bool RIB1_0 { get { return (RIB1 & (1 << 0)) == 1 << 0; } set { RIB1 = (byte)(RIB1 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool RIB1_1 { get { return (RIB1 & (1 << 1)) == 1 << 1; } set { RIB1 = (byte)(RIB1 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool RIB1_2 { get { return (RIB1 & (1 << 2)) == 1 << 2; } set { RIB1 = (byte)(RIB1 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool RIB1_3 { get { return (RIB1 & (1 << 3)) == 1 << 3; } set { RIB1 = (byte)(RIB1 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool RIB1_4 { get { return (RIB1 & (1 << 4)) == 1 << 4; } set { RIB1 = (byte)(RIB1 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool RIB1_5 { get { return (RIB1 & (1 << 5)) == 1 << 5; } set { RIB1 = (byte)(RIB1 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool RIB1_6 { get { return (RIB1 & (1 << 6)) == 1 << 6; } set { RIB1 = (byte)(RIB1 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool RIB1_7 { get { return (RIB1 & (1 << 7)) == 1 << 7; } set { RIB1 = (byte)(RIB1 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte RIB2 { get { return Data[0x32]; } set { Data[0x32] = value; } }
        public bool RIB2_0 { get { return (RIB2 & (1 << 0)) == 1 << 0; } set { RIB2 = (byte)(RIB2 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool RIB2_1 { get { return (RIB2 & (1 << 1)) == 1 << 1; } set { RIB2 = (byte)(RIB2 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool RIB2_2 { get { return (RIB2 & (1 << 2)) == 1 << 2; } set { RIB2 = (byte)(RIB2 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool RIB2_3 { get { return (RIB2 & (1 << 3)) == 1 << 3; } set { RIB2 = (byte)(RIB2 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool RIB2_4 { get { return (RIB2 & (1 << 4)) == 1 << 4; } set { RIB2 = (byte)(RIB2 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool RIB2_5 { get { return (RIB2 & (1 << 5)) == 1 << 5; } set { RIB2 = (byte)(RIB2 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool RIB2_6 { get { return (RIB2 & (1 << 6)) == 1 << 6; } set { RIB2 = (byte)(RIB2 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool RIB2_7 { get { return (RIB2 & (1 << 7)) == 1 << 7; } set { RIB2 = (byte)(RIB2 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte RIB3 { get { return Data[0x33]; } set { Data[0x33] = value; } }
        public bool RIB3_0 { get { return (RIB3 & (1 << 0)) == 1 << 0; } set { RIB3 = (byte)(RIB3 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool RIB3_1 { get { return (RIB3 & (1 << 1)) == 1 << 1; } set { RIB3 = (byte)(RIB3 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool RIB3_2 { get { return (RIB3 & (1 << 2)) == 1 << 2; } set { RIB3 = (byte)(RIB3 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool RIB3_3 { get { return (RIB3 & (1 << 3)) == 1 << 3; } set { RIB3 = (byte)(RIB3 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool RIB3_4 { get { return (RIB3 & (1 << 4)) == 1 << 4; } set { RIB3 = (byte)(RIB3 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool RIB3_5 { get { return (RIB3 & (1 << 5)) == 1 << 5; } set { RIB3 = (byte)(RIB3 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool RIB3_6 { get { return (RIB3 & (1 << 6)) == 1 << 6; } set { RIB3 = (byte)(RIB3 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool RIB3_7 { get { return (RIB3 & (1 << 7)) == 1 << 7; } set { RIB3 = (byte)(RIB3 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte RIB4 { get { return Data[0x34]; } set { Data[0x34] = value; } }
        public bool RIB4_0 { get { return (RIB4 & (1 << 0)) == 1 << 0; } set { RIB4 = (byte)(RIB4 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool RIB4_1 { get { return (RIB4 & (1 << 1)) == 1 << 1; } set { RIB4 = (byte)(RIB4 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool RIB4_2 { get { return (RIB4 & (1 << 2)) == 1 << 2; } set { RIB4 = (byte)(RIB4 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool RIB4_3 { get { return (RIB4 & (1 << 3)) == 1 << 3; } set { RIB4 = (byte)(RIB4 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool RIB4_4 { get { return (RIB4 & (1 << 4)) == 1 << 4; } set { RIB4 = (byte)(RIB4 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool RIB4_5 { get { return (RIB4 & (1 << 5)) == 1 << 5; } set { RIB4 = (byte)(RIB4 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool RIB4_6 { get { return (RIB4 & (1 << 6)) == 1 << 6; } set { RIB4 = (byte)(RIB4 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool RIB4_7 { get { return (RIB4 & (1 << 7)) == 1 << 7; } set { RIB4 = (byte)(RIB4 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        private byte RIB5 { get { return Data[0x35]; } set { Data[0x35] = value; } }
        public bool RIB5_0 { get { return (RIB5 & (1 << 0)) == 1 << 0; } set { RIB5 = (byte)(RIB5 & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool RIB5_1 { get { return (RIB5 & (1 << 1)) == 1 << 1; } set { RIB5 = (byte)(RIB5 & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool RIB5_2 { get { return (RIB5 & (1 << 2)) == 1 << 2; } set { RIB5 = (byte)(RIB5 & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool RIB5_3 { get { return (RIB5 & (1 << 3)) == 1 << 3; } set { RIB5 = (byte)(RIB5 & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool RIB5_4 { get { return (RIB5 & (1 << 4)) == 1 << 4; } set { RIB5 = (byte)(RIB5 & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool RIB5_5 { get { return (RIB5 & (1 << 5)) == 1 << 5; } set { RIB5 = (byte)(RIB5 & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool RIB5_6 { get { return (RIB5 & (1 << 6)) == 1 << 6; } set { RIB5 = (byte)(RIB5 & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool RIB5_7 { get { return (RIB5 & (1 << 7)) == 1 << 7; } set { RIB5 = (byte)(RIB5 & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        public byte _0x36 { get { return Data[0x36]; } set { Data[0x36] = value; } }
        public byte _0x37 { get { return Data[0x37]; } set { Data[0x37] = value; } }
        public int Memory_ContestCount { get { return Data[0x38]; } set { Data[0x38] = (byte)value; } }
        public int Memory_BattleCount { get { return Data[0x39]; } set { Data[0x39] = (byte)value; } }
        private byte DistByte { get { return Data[0x3A]; } set { Data[0x3A] = value; } }
        public bool Dist1 { get { return (DistByte & (1 << 0)) == 1 << 0; } set { DistByte = (byte)(DistByte & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool Dist2 { get { return (DistByte & (1 << 1)) == 1 << 1; } set { DistByte = (byte)(DistByte & ~(1 << 1) | (value ? 1 << 1 : 0)); } }
        public bool Dist3 { get { return (DistByte & (1 << 2)) == 1 << 2; } set { DistByte = (byte)(DistByte & ~(1 << 2) | (value ? 1 << 2 : 0)); } }
        public bool Dist4 { get { return (DistByte & (1 << 3)) == 1 << 3; } set { DistByte = (byte)(DistByte & ~(1 << 3) | (value ? 1 << 3 : 0)); } }
        public bool Dist5 { get { return (DistByte & (1 << 4)) == 1 << 4; } set { DistByte = (byte)(DistByte & ~(1 << 4) | (value ? 1 << 4 : 0)); } }
        public bool Dist6 { get { return (DistByte & (1 << 5)) == 1 << 5; } set { DistByte = (byte)(DistByte & ~(1 << 5) | (value ? 1 << 5 : 0)); } }
        public bool Dist7 { get { return (DistByte & (1 << 6)) == 1 << 6; } set { DistByte = (byte)(DistByte & ~(1 << 6) | (value ? 1 << 6 : 0)); } }
        public bool Dist8 { get { return (DistByte & (1 << 7)) == 1 << 7; } set { DistByte = (byte)(DistByte & ~(1 << 7) | (value ? 1 << 7 : 0)); } }
        public byte _0x3B { get { return Data[0x3B]; } set { Data[0x3B] = value; } }
        public byte _0x3C { get { return Data[0x3C]; } set { Data[0x3C] = value; } }
        public byte _0x3D { get { return Data[0x3D]; } set { Data[0x3D] = value; } }
        public byte _0x3E { get { return Data[0x3E]; } set { Data[0x3E] = value; } }
        public byte _0x3F { get { return Data[0x3F]; } set { Data[0x3F] = value; } }
        #endregion
        #region Block B
        public string Nickname
        {
            get
            {
                return Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x40, 24))
                    .Replace("\uE08F", "\u2640") // nidoran
                    .Replace("\uE08E", "\u2642") // nidoran
                    .Replace("\u2019", "\u0027"); // farfetch'd
            }
            set
            {
                string TempNick = value // Replace Special Characters and add Terminator
                .Replace("\u2640", "\uE08F") // nidoran
                .Replace("\u2642", "\uE08E") // nidoran
                .Replace("\u0027", "\u2019") // farfetch'd
                .PadRight(value.Length + 1, '\0'); // Null Terminator
                byte[] NickBytes = Encoding.Unicode.GetBytes(TempNick);
                Array.Copy(NickBytes, 0, Data, 0x40, NickBytes.Length);
            }
        }
        public int Move1
        {
            get { return BitConverter.ToUInt16(Data, 0x5A); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x5A, 2); }
        }
        public int Move2
        {
            get { return BitConverter.ToUInt16(Data, 0x5C); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x5C, 2); }
        }
        public int Move3
        {
            get { return BitConverter.ToUInt16(Data, 0x5E); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x5E, 2); }
        }
        public int Move4
        {
            get { return BitConverter.ToUInt16(Data, 0x60); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x60, 2); }
        }
        public int Move1_PP { get { return Data[0x62]; } set { Data[0x62] = (byte)value; } }
        public int Move2_PP { get { return Data[0x63]; } set { Data[0x63] = (byte)value; } }
        public int Move3_PP { get { return Data[0x64]; } set { Data[0x64] = (byte)value; } }
        public int Move4_PP { get { return Data[0x65]; } set { Data[0x65] = (byte)value; } }
        public int Move1_PPUps { get { return Data[0x66]; } set { Data[0x66] = (byte)value; } }
        public int Move2_PPUps { get { return Data[0x67]; } set { Data[0x67] = (byte)value; } }
        public int Move3_PPUps { get { return Data[0x68]; } set { Data[0x68] = (byte)value; } }
        public int Move4_PPUps { get { return Data[0x69]; } set { Data[0x69] = (byte)value; } }
        public int RelearnMove1
        {
            get { return BitConverter.ToUInt16(Data, 0x6A); }
            set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0x6A, 2); }
        }
        public int RelearnMove2
        {
            get { return BitConverter.ToUInt16(Data, 0x6C); }
            set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0x6C, 2); }
        }
        public int RelearnMove3
        {
            get { return BitConverter.ToUInt16(Data, 0x6E); }
            set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0x6E, 2); }
        }
        public int RelearnMove4
        {
            get { return BitConverter.ToUInt16(Data, 0x70); }
            set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0x70, 2); }
        }
        public bool SecretSuperTraining { get { return Data[0x72] == 1; } set { Data[0x72] = (byte)((Data[0x72] & ~1) | (value ? 1 : 0)); } }
        public byte _0x73 { get { return Data[0x73]; } set { Data[0x73] = value; } }
        private uint IV32 { get { return BitConverter.ToUInt32(Data, 0x74); } set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0x74, 4); } }
        public int IV_HP { get { return (int)(IV32 >> 00) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 00)) | (uint)((value > 31 ? 31 : value) << 00)); } }
        public int IV_ATK { get { return (int)(IV32 >> 05) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 05)) | (uint)((value > 31 ? 31 : value) << 05)); } }
        public int IV_DEF { get { return (int)(IV32 >> 10) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 10)) | (uint)((value > 31 ? 31 : value) << 10)); } }
        public int IV_SPE { get { return (int)(IV32 >> 15) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 15)) | (uint)((value > 31 ? 31 : value) << 15)); } }
        public int IV_SPA { get { return (int)(IV32 >> 20) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 20)) | (uint)((value > 31 ? 31 : value) << 20)); } }
        public int IV_SPD { get { return (int)(IV32 >> 25) & 0x1F; } set { IV32 = (uint)((IV32 & ~(0x1F << 25)) | (uint)((value > 31 ? 31 : value) << 25)); } }
        public bool IsEgg { get { return ((IV32 >> 30) & 1) == 1; } set { IV32 = (uint)((IV32 & ~0x40000000) | (uint)(value ? 0x40000000 : 0)); } }
        public bool IsNicknamed { get { return ((IV32 >> 31) & 1) == 1; } set { IV32 = ((IV32 & ~0x80000000) | (value ? 0x80000000 : 0)); } }
        #endregion
        #region Block C
        public string HT_Name
        {
            get
            {
                return Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x78, 24))
                    .Replace("\uE08F", "\u2640") // nidoran
                    .Replace("\uE08E", "\u2642") // nidoran
                    .Replace("\u2019", "\u0027"); // farfetch'd
            }
            set
            {
                string TempNick = value // Replace Special Characters and add Terminator
                .Replace("\u2640", "\uE08F") // nidoran
                .Replace("\u2642", "\uE08E") // nidoran
                .Replace("\u0027", "\u2019") // farfetch'd
                .PadRight(value.Length + 1, '\0'); // Null Terminator
                byte[] nameBytes = Encoding.Unicode.GetBytes(TempNick);
                Array.Copy(nameBytes, 0, Data, 0x78, nameBytes.Length);
            }
        }
        public int HT_Gender { get { return Data[0x92]; } set { Data[0x92] = (byte)value; } }
        public int CurrentHandler { get { return Data[0x93]; } set { Data[0x93] = (byte)value; } }
        public int Geo1_Region { get { return Data[0x94]; } set { Data[0x94] = (byte)value; } }
        public int Geo1_Country { get { return Data[0x95]; } set { Data[0x95] = (byte)value; } }
        public int Geo2_Region { get { return Data[0x96]; } set { Data[0x96] = (byte)value; } }
        public int Geo2_Country { get { return Data[0x97]; } set { Data[0x97] = (byte)value; } }
        public int Geo3_Region { get { return Data[0x98]; } set { Data[0x98] = (byte)value; } }
        public int Geo3_Country { get { return Data[0x99]; } set { Data[0x99] = (byte)value; } }
        public int Geo4_Region { get { return Data[0x9A]; } set { Data[0x9A] = (byte)value; } }
        public int Geo4_Country { get { return Data[0x9B]; } set { Data[0x9B] = (byte)value; } }
        public int Geo5_Region { get { return Data[0x9C]; } set { Data[0x9C] = (byte)value; } }
        public int Geo5_Country { get { return Data[0x9D]; } set { Data[0x9D] = (byte)value; } }
        public byte _0x9E { get { return Data[0x9E]; } set { Data[0x9E] = value; } }
        public byte _0x9F { get { return Data[0x9F]; } set { Data[0x9F] = value; } }
        public byte _0xA0 { get { return Data[0xA0]; } set { Data[0xA0] = value; } }
        public byte _0xA1 { get { return Data[0xA1]; } set { Data[0xA1] = value; } }
        public int HT_Friendship { get { return Data[0xA2]; } set { Data[0xA2] = (byte)value; } }
        public int HT_Affection { get { return Data[0xA3]; } set { Data[0xA3] = (byte)value; } }
        public int HT_Intensity { get { return Data[0xA4]; } set { Data[0xA4] = (byte)value; } }
        public int HT_Memory { get { return Data[0xA5]; } set { Data[0xA5] = (byte)value; } }
        public int HT_Feeling { get { return Data[0xA6]; } set { Data[0xA6] = (byte)value; } }
        public byte _0xA7 { get { return Data[0xA7]; } set { Data[0xA7] = value; } }
        public int HT_TextVar { get { return BitConverter.ToUInt16(Data, 0xA8); } set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0xA8, 2); } }
        public byte _0xAA { get { return Data[0xAA]; } set { Data[0xAA] = value; } }
        public byte _0xAB { get { return Data[0xAB]; } set { Data[0xAB] = value; } }
        public byte _0xAC { get { return Data[0xAC]; } set { Data[0xAC] = value; } }
        public byte _0xAD { get { return Data[0xAD]; } set { Data[0xAD] = value; } }
        public byte Fullness { get { return Data[0xAE]; } set { Data[0xAE] = value; } }
        public byte Enjoyment { get { return Data[0xAF]; } set { Data[0xAF] = value; } }
        #endregion
        #region Block D
        public string OT_Name
        {
            get
            {
                return Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0xB0, 24))
                    .Replace("\uE08F", "\u2640") // nidoran
                    .Replace("\uE08E", "\u2642") // nidoran
                    .Replace("\u2019", "\u0027"); // farfetch'd
            }
            set
            {
                string TempNick = value // Replace Special Characters and add Terminator
                .Replace("\u2640", "\uE08F") // nidoran
                .Replace("\u2642", "\uE08E") // nidoran
                .Replace("\u0027", "\u2019") // farfetch'd
                .PadRight(value.Length + 1, '\0'); // Null Terminator
                byte[] nameBytes = Encoding.Unicode.GetBytes(TempNick);
                Array.Copy(nameBytes, 0, Data, 0xB0, nameBytes.Length);
            }
        }
        public int OT_Friendship { get { return Data[0xCA]; } set { Data[0xCA] = (byte)value; } }
        public int OT_Affection { get { return Data[0xCB]; } set { Data[0xCB] = (byte)value; } }
        public int OT_Intensity { get { return Data[0xCC]; } set { Data[0xCC] = (byte)value; } }
        public int OT_Memory { get { return Data[0xCD]; } set { Data[0xCD] = (byte)value; } }
        public int OT_TextVar { get { return BitConverter.ToUInt16(Data, 0xCE); } set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0xCE, 2); } }
        public int OT_Feeling { get { return Data[0xD0]; } set { Data[0xD0] = (byte)value; } }
        public int Egg_Year { get { return Data[0xD1]; } set { Data[0xD1] = (byte)value; } }
        public int Egg_Month { get { return Data[0xD2]; } set { Data[0xD2] = (byte)value; } }
        public int Egg_Day { get { return Data[0xD3]; } set { Data[0xD3] = (byte)value; } }
        public int Met_Year { get { return Data[0xD4]; } set { Data[0xD4] = (byte)value; } }
        public int Met_Month { get { return Data[0xD5]; } set { Data[0xD5] = (byte)value; } }
        public int Met_Day { get { return Data[0xD6]; } set { Data[0xD6] = (byte)value; } }
        public byte _0xD7 { get { return Data[0xD7]; } set { Data[0xD7] = value; } }
        public int Egg_Location { get { return BitConverter.ToUInt16(Data, 0xD8); } set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0xD8, 2); } }
        public int Met_Location { get { return BitConverter.ToUInt16(Data, 0xDA); } set { Array.Copy(BitConverter.GetBytes((ushort)value), 0, Data, 0xDA, 2); } }
        public int Ball { get { return Data[0xDC]; } set { Data[0xDC] = (byte)value; } }
        public int Met_Level { get { return Data[0xDD] & ~0x80; } set { Data[0xDD] = (byte)((Data[0xDD] & 0x80) | value); } }
        public int OT_Gender { get { return Data[0xDD] >> 7; } set { Data[0xDD] = (byte)((Data[0xDD] & ~0x80) | (value << 7)); } }
        public int EncounterType { get { return Data[0xDE]; } set { Data[0xDE] = (byte)value; } }
        public int Version { get { return Data[0xDF]; } set { Data[0xDF] = (byte)value; } }
        public int Country { get { return Data[0xE0]; } set { Data[0xE0] = (byte)value; } }
        public int Region { get { return Data[0xE1]; } set { Data[0xE1] = (byte)value; } }
        public int ConsoleRegion { get { return Data[0xE2]; } set { Data[0xE2] = (byte)value; } }
        public int Language { get { return Data[0xE3]; } set { Data[0xE3] = (byte)value; } }
        #endregion
        #region Battle Stats
        public int Stat_Level { get { return Data[0xEC]; } set { Data[0xEC] = (byte)value; } }
        public int Stat_HPCurrent { get { return BitConverter.ToUInt16(Data, 0xF0); } set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0xF0, 2); } }
        public int Stat_HPMax { get { return BitConverter.ToUInt16(Data, 0xF2); } set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0xF2, 2); } }
        public int Stat_ATK { get { return BitConverter.ToUInt16(Data, 0xF4); } set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0xF4, 2); } }
        public int Stat_DEF { get { return BitConverter.ToUInt16(Data, 0xF6); } set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0xF6, 2); } }
        public int Stat_SPE { get { return BitConverter.ToUInt16(Data, 0xF8); } set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0xF8, 2); } }
        public int Stat_SPA { get { return BitConverter.ToUInt16(Data, 0xFA); } set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0xFA, 2); } }
        public int Stat_SPD { get { return BitConverter.ToUInt16(Data, 0xFC); } set { Array.Copy(BitConverter.GetBytes(value), 0, Data, 0xFC, 2); } }
        #endregion

        // Simple Generated Attributes
        public int[] IVs { get { return new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD }; } }
        public int[] EVs { get { return new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD }; } }
        public int[] Moves { get { return new[] { Move1, Move2, Move3, Move4 }; } }
        public int PSV { get { return (int)(((PID >> 16) ^ (PID & 0xFFFF)) >> 4); } }
        public int TSV { get { return (TID ^ SID) >> 4; } }
        public bool IsShiny { get { return TSV == PSV; } }
        public bool PKRS_Infected { get { return PKRS_Strain > 0; } }
        public bool PKRS_Cured { get { return PKRS_Days == 0 && PKRS_Strain > 0; } }
        public bool Gen6 { get { return Version >= 24 && Version <= 29; } }
        public bool Gen5 { get { return Version >= 20 && Version <= 23; } }
        public bool Gen4 { get { return (Version >= 10 && Version < 12) || (Version >= 7 && Version <= 8); } }
        public bool Gen3 { get { return ((Version >= 1 && Version <= 5) || Version == 15); } }
        public bool GenU { get { return !(Gen6 || Gen5 || Gen4 || Gen3); } }

        // Complex Generated Attributes
        public Image Sprite { get { return getSprite(this); } }
        public string[] ShowdownText { get { return getShowdownText(this); } }
        public string[] QRText { get { return getQRText(this); } }
        public int HPType
        {
            get { return (15 * ((IV_HP & 1) + 2 * (IV_ATK & 1) + 4 * (IV_DEF & 1) + 8 * (IV_SPE & 1) + 16 * (IV_SPA & 1) + 32 * (IV_SPD & 1))) / 63; }
            set
            {
                IV_HP = (IV_HP & ~1) + hpivs[value, 0];
                IV_ATK = (IV_ATK & ~1) + hpivs[value, 1];
                IV_DEF = (IV_DEF & ~1) + hpivs[value, 2];
                IV_SPE = (IV_SPE & ~1) + hpivs[value, 3];
                IV_SPA = (IV_SPA & ~1) + hpivs[value, 4];
                IV_SPD = (IV_SPD & ~1) + hpivs[value, 5];
            }
        }
        public int Characteristic
        {
            get
            {
                // Characteristic with EC%6
                int pm6 = (int)(EncryptionConstant % 6); // EC MOD 6
                int maxIV = IVs.Max();
                int pm6stat = 0;

                for (int i = 0; i < 6; i++)
                {
                    pm6stat = (pm6 + i) % 6;
                    if (IVs[pm6stat] == maxIV)
                        break; // P%6 is this stat
                }
                return pm6stat*5 + maxIV%5;
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
        public string FileName { get { return getFileName(this); } }

        // Methods
        public void RefreshChecksum()
        {
            Checksum = CalculateChecksum();
        }
        public ushort CalculateChecksum()
        {
            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(Data, i);

            return chk;
        }
        public byte[] Write()
        {
            RefreshChecksum();
            return Data;
        }
        public byte[] Encrypt()
        {
            Checksum = CalculateChecksum();
            return encryptArray(Data);
        }
        public void CalculateStats()
        {
            ushort[] Stats = getStats(this);
            Stat_HPMax = Stat_HPCurrent = Stats[0];
            Stat_ATK = Stats[1];
            Stat_DEF = Stats[2];
            Stat_SPE = Stats[3];
            Stat_SPA = Stats[4];
            Stat_SPD = Stats[5];
        }

        // General User-error Fixes
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
        public void FixRelearn()
        {
            if (RelearnMove4 != 0 && RelearnMove3 == 0)
            {
                RelearnMove3 = RelearnMove4;
                RelearnMove4 = 0;
            }
            if (RelearnMove3 != 0 && RelearnMove2 == 0)
            {
                RelearnMove2 = RelearnMove3;
                RelearnMove3 = 0;
            }
            if (RelearnMove2 != 0 && RelearnMove1 == 0)
            {
                RelearnMove1 = RelearnMove2;
                RelearnMove2 = 0;
            }
        }
        public void FixMemories()
        {
            if (IsEgg) // No memories if is egg.
            {
                Geo1_Country = Geo2_Country = Geo3_Country = Geo4_Country = Geo5_Country =
                Geo1_Region = Geo2_Region = Geo3_Region = Geo4_Region = Geo5_Region =
                HT_Friendship = HT_Affection = HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling =
                /* OT_Friendship */ OT_Affection = OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;

                // Clear Handler
                HT_Name = "".PadRight(11, '\0');
                return;
            }
            
            if (BitConverter.ToUInt16(Data, 0x78) == 0) // first character of handler name is \0
                HT_Friendship = HT_Affection = HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = 0;

            Geo1_Region = Geo1_Country > 0 ? Geo1_Region : 0;
            Geo2_Region = Geo2_Country > 0 ? Geo2_Region : 0;
            Geo3_Region = Geo3_Country > 0 ? Geo3_Region : 0;
            Geo4_Region = Geo4_Country > 0 ? Geo4_Region : 0;
            Geo5_Region = Geo5_Country > 0 ? Geo5_Region : 0;

            if (Geo5_Country != 0 && Geo4_Country == 0)
            {
                Geo4_Country = Geo5_Country;
                Geo4_Region = Geo5_Region;
                Geo5_Country = Geo5_Region = 0;
            }
            if (Geo4_Country != 0 && Geo3_Country == 0)
            {
                Geo3_Country = Geo4_Country;
                Geo3_Region = Geo4_Region;
                Geo4_Country = Geo4_Region = 0;
            }
            if (Geo3_Country != 0 && Geo2_Country == 0)
            {
                Geo2_Country = Geo3_Country;
                Geo2_Region = Geo3_Region;
                Geo3_Country = Geo3_Region = 0;
            }
            if (Geo2_Country != 0 && Geo1_Country == 0)
            {
                Geo1_Country = Geo2_Country;
                Geo1_Region = Geo2_Region;
                Geo2_Country = Geo2_Region = 0;
            }
            if (Geo1_Country == 0 /* && IsEgg (which is always true) */)
            {
                // Non-Eggs need to have a current location.
                Geo1_Country = Country;
                Geo1_Region = Region;
            }
        }

        // Synthetic Trading Logic
        public void Trade(string SAV_Trainer, int SAV_TID, int SAV_SID, int SAV_COUNTRY, int SAV_REGION)
        {
            // Eggs do not have any modifications done if they are traded
            if (IsEgg)
                return;
            // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
            if (!TradeOT(SAV_Trainer, SAV_TID, SAV_SID, SAV_COUNTRY, SAV_REGION))
                TradeHT(SAV_Trainer, SAV_COUNTRY, SAV_REGION);
        }
        private bool TradeOT(string SAV_Trainer, int SAV_TID, int SAV_SID, int SAV_COUNTRY, int SAV_REGION)
        {
            // Check to see if the OT matches the SAV's OT info.
            if (!(SAV_Trainer == OT_Name && SAV_TID == TID && SAV_SID == SID))
                return false;

            CurrentHandler = 0;
            if (SAV_COUNTRY != Geo1_Country || SAV_REGION != Geo1_Region)
                TradeGeoLocation(SAV_COUNTRY, SAV_REGION);

            return true;
        }
        private void TradeHT(string SAV_Trainer, int SAV_COUNTRY, int SAV_REGION)
        {
            if (SAV_Trainer != HT_Name || SAV_COUNTRY != Geo1_Country || SAV_REGION != Geo1_Region)
                TradeGeoLocation(SAV_COUNTRY, SAV_REGION);

            CurrentHandler = 1;

            // Make a memory if no memory already exists. Pretty terrible way of doing this but I'd rather not overwrite existing memories.
            if (HT_Memory == 0)
                TradeMemory();
        }
        private void TradeGeoLocation(int GeoCountry, int GeoRegion)
        {
            // Allow the method to abort if the values are invalid
            if (GeoCountry < 0 || GeoRegion < 0)
                return;

            // Trickle down
            Geo5_Country = Geo4_Country;
            Geo5_Region = Geo4_Region;

            Geo4_Country = Geo3_Country;
            Geo4_Region = Geo3_Region;

            Geo3_Country = Geo2_Country;
            Geo3_Region = Geo2_Region;

            Geo2_Country = Geo1_Country;
            Geo2_Region = Geo1_Region;

            Geo1_Country = Country;
            Geo1_Region = Region;
        }
        private void TradeMemory()
        {
            HT_Memory = 4; // Link trade to [VAR: General Location]
            HT_TextVar = 9; // Pokécenter
            HT_Intensity = 1;
            HT_Feeling = Util.rand.Next(0, 9);
        }
        public void TradeFriendshipAffection(string SAV_TRAINER)
        {
            // Don't alter the data if the info is the same.
            if (SAV_TRAINER == HT_Name) 
                return;

            // Reset
            HT_Friendship = getBaseFriendship(Species);
            HT_Affection = 0;
        }
    }
}
