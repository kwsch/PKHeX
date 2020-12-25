using System;

namespace PKHeX.Core
{
    public sealed class PWTBlock5 : SaveBlock
    {
        public PWTBlock5(SAV5B2W2 sav, int offset) : base(sav) => Offset = offset;

        public ushort GetPWTRecord(int id) => GetPWTRecord((PWTRecordID)id);

        public ushort GetPWTRecord(PWTRecordID id)
        {
            if (id is < PWTRecordID.Normal or > PWTRecordID.MixMaster)
                throw new ArgumentException(nameof(id));
            int ofs = Offset + 0x5C + ((int)id * 2);
            return BitConverter.ToUInt16(Data, ofs);
        }

        public void SetPWTRecord(int id, ushort value) => SetPWTRecord((PWTRecordID)id, value);

        public void SetPWTRecord(PWTRecordID id, ushort value)
        {
            if (id is < PWTRecordID.Normal or > PWTRecordID.MixMaster)
                throw new ArgumentException(nameof(id));
            int ofs = Offset + 0x5C + ((int)id * 2);
            SAV.SetData(BitConverter.GetBytes(value), ofs);
        }
    }
}