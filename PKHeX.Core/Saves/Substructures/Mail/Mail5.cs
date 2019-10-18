using System;

namespace PKHeX.Core
{
    public sealed class Mail5 : Mail
    {
        public const int SIZE = 0x38;

        public Mail5(byte[] data, int ofs = -1) : base(data, ofs) { }

        public Mail5(byte? lang, byte? ver) : base(new byte[SIZE])
        {
            if (lang != null) AuthorLanguage = (byte)lang;
            if (ver != null) AuthorVersion = (byte)ver;
            ResetData();
        }

        private void ResetData()
        {
            AuthorTID = 0;
            AuthorSID = 0;
            AuthorGender = 0;
            MailType = 0xFF;
            AuthorName = string.Empty;
            for (int i = 0; i < 3; i++)
                SetMisc(i, 0);
            MessageEnding = 0xFFFF;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 4; x++)
                    SetMessage(y, x, (ushort)(x == 1 ? 0 : 0xFFFF));
            }
        }

        public override void CopyTo(PK5 pk5) => pk5.HeldMailData = Data;
        public override ushort AuthorTID { get => BitConverter.ToUInt16(Data, 0); set => BitConverter.GetBytes(value).CopyTo(Data, 0); }
        public override ushort AuthorSID { get => BitConverter.ToUInt16(Data, 2); set => BitConverter.GetBytes(value).CopyTo(Data, 2); }
        public override byte AuthorGender { get => Data[4]; set => Data[4] = value; }
        public override byte AuthorLanguage { get => Data[5]; set => Data[5] = value; }
        public override byte AuthorVersion { get => Data[6]; set => Data[6] = value; }
        public override int MailType { get => Data[7]; set => Data[7] = (byte)value; }
        public override string AuthorName { get => StringConverter.GetString5(Data, 8, 0x10); set => StringConverter.SetString5(value, 7, 8).CopyTo(Data, 8); }
        public int GetMisc(int index) => BitConverter.ToUInt16(Data, 0x1C - (index * 2));
        public void SetMisc(int index, int value) => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1C - (index * 2));
        public ushort MessageEnding { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1E); }
        public override ushort GetMessage(int index1, int index2) => BitConverter.ToUInt16(Data, 0x20 + (((index1 * 4) + index2) * 2));
        public override void SetMessage(int index1, int index2, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, 0x20 + (((index1 * 4) + index2) * 2));

        public override bool? IsEmpty
        {
            get
            {
                if (MailType == 0xFF) return true;
                if (MailType <= 11) return false;
                return null;
            }
        }

        public override void SetBlank() => SetBlank(null, null);
        public void SetBlank(byte? lang, byte? ver) => new Mail5(lang: lang, ver: ver).Data.CopyTo(Data, 0);
    }
}
