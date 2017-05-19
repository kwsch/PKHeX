using System;

namespace PKHeX.Core
{
    public class QRPK7
    {
        public byte[] Data;

        public uint EncryptionConstant => BitConverter.ToUInt32(Data, 0);
        public int HT_Flags => Data[4];
        public int Unk_5 => Data[5];
        public int Unk_6 => Data[6];
        public int Unk_7 => Data[7];
        public int Move1_PPUps => Data[8];
        public int Move2_PPUps => Data[9];
        public int Move3_PPUps => Data[0xA];
        public int Move4_PPUps => Data[0xB];
        public uint IV32 => BitConverter.ToUInt32(Data, 0xC);
        public uint Unk_10 => BitConverter.ToUInt32(Data, 0x10);
        public ushort Species => BitConverter.ToUInt16(Data, 0x14);
        public ushort HeldItem => BitConverter.ToUInt16(Data, 0x16);
        public ushort Move1 => BitConverter.ToUInt16(Data, 0x18);
        public ushort Move2 => BitConverter.ToUInt16(Data, 0x1A);
        public ushort Move3 => BitConverter.ToUInt16(Data, 0x1C);
        public ushort Move4 => BitConverter.ToUInt16(Data, 0x1E);
        public int Unk_20 => Data[0x20];
        public int AbilityIndex => Data[0x21];
        public int Nature => Data[0x22];
        public int EncounterFlags => Data[0x23]; // Data[0x1D] in PK7
        public int EV_HP => Data[0x24];
        public int EV_ATK => Data[0x25];
        public int EV_DEF => Data[0x26];
        public int EV_SPE => Data[0x27];
        public int EV_SPA => Data[0x28];
        public int EV_SPD => Data[0x29];
        public int Unk_2A => Data[0x2A];
        public int Familiarity => Data[0x2B];
        public int Ball => Data[0x2C];
        public int Level => Data[0x2D];
        public int CassetteVersion => Data[0x2E];
        public int Language => Data[0x2F];

        public QRPK7(byte[] d)
        {
            if (d.Length != 0x30)
            {
                throw new ArgumentException("Invalid QRPK7 Data!");
            }

            Data = (byte[]) d.Clone();
        }

    }
}
