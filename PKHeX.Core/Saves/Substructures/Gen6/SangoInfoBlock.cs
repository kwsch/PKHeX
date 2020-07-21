using System;

namespace PKHeX.Core
{
    public sealed class SangoInfoBlock : SaveBlock
    {
        public SangoInfoBlock(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

        private const uint EON_MAGIC = WC6.EonTicketConst;

        public uint EonTicketReceivedMagic // 0x319B8
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x63B8);
            set => SAV.SetData(BitConverter.GetBytes(value), Offset + 0x63B8);
        }

        public string SecretBaseQRText // 0x319BC -- 17*u16
        {
            get => SAV.GetString(Offset + 0x63BC, 0x10);
            set => SAV.SetData(SAV.SetString(value, 0x10), Offset + 0x63BC);
        }

        public uint EonTicketSendMagic // 0x319DE
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x63DE);
            set => SAV.SetData(BitConverter.GetBytes(value), Offset + 0x63DE);
        }

        public void ReceiveEon() => EonTicketReceivedMagic = EON_MAGIC;
        public void EnableSendEon() => EonTicketSendMagic = EON_MAGIC;
    }
}