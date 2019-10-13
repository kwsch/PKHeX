using System;

namespace PKHeX.Core
{
    public interface IPokePuff
    {
        Puff6 PuffBlock { get; }
    }

    public interface IOPower
    {
        OPower6 OPowerBlock { get; }
    }

    public interface ILink
    {
        Link6 LinkBlock { get; }
    }

    public sealed class Link6 : SaveBlock
    {
        public Link6(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public Link6(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        public byte[] GetLinkInfoData() => Data.Slice(Offset + 0x1FF, PL6.Size);
        public PL6 GetLinkInfo() => new PL6(GetLinkInfoData());

        public void SetLinkInfoData(byte[] data)
        {
            data.CopyTo(Data, Offset);
            Checksum = GetCalculatedChecksum(); // [app,chk)
        }

        public void SetLinkInfo(PL6 pl6)
        {
            pl6.Data.CopyTo(Data, Offset + 0x1FF);
            Checksum = GetCalculatedChecksum(); // [app,chk)
        }

        private ushort GetCalculatedChecksum() => Checksums.CRC16_CCITT(Data, Offset + 0x200, 0xC48 - 4 - 0x200); // [app,chk)

        private int GetChecksumOffset() => Offset + 0xC48 - 4;

        public ushort Checksum
        {
            get => BitConverter.ToUInt16(Data, GetChecksumOffset());
            set => BitConverter.GetBytes(value).CopyTo(Data, GetChecksumOffset());
        }
    }
}