using System;

namespace PKHeX.Core
{
    public class Mail4 : Mail
    {
        private const int SIZE = 0x38;

        public Mail4(SAV4 sav, int index)
        {
            switch (sav.Version)
            {
                case GameVersion.DP: DataOffset = (index * SIZE) + 0x4BEC + sav.GBO; break;
                case GameVersion.Pt: DataOffset = (index * SIZE) + 0x4E80 + sav.GBO; break;
                case GameVersion.HGSS: DataOffset = (index * SIZE) + 0x3FA8 + sav.GBO; break;
            }
            Data = sav.GetData(DataOffset, SIZE);
        }

        public Mail4(byte[] data)
        {
            Data = data;
            DataOffset = -1;
        }

        public Mail4(byte? lang = null, byte? ver = null)
        {
            Data = new byte[SIZE];
            DataOffset = -1;
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
                SetAppearPKM(i, 0xFFFF);
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 4; x++)
                    SetMessage(y, x, (ushort)(x == 1 ? 0 : 0xFFFF));
            }
        }

        public override void CopyTo(PK4 pk4) => pk4.HeldMailData = Data;
        public override ushort AuthorTID { get => BitConverter.ToUInt16(Data, 0); set => BitConverter.GetBytes(value).CopyTo(Data, 0); }
        public ushort AuthorSID { get => BitConverter.ToUInt16(Data, 2); set => BitConverter.GetBytes(value).CopyTo(Data, 2); }
        public override byte AuthorGender { get => Data[4]; set => Data[4] = value; }
        public byte AuthorLanguage { get => Data[5]; set => Data[5] = value; }
        public byte AuthorVersion { get => Data[6]; set => Data[6] = value; }
        public override int MailType { get => Data[7]; set => Data[7] = (byte)value; }
        public override string AuthorName { get => StringConverter4.GetString4(Data, 8, 0x10); set => StringConverter4.SetString4(value, 7, 8, 0xFFFF).CopyTo(Data, 8); }
        public int GetAppearPKM(int index) => BitConverter.ToUInt16(Data, 0x1C - (index * 2));
        public void SetAppearPKM(int index, int value) => BitConverter.GetBytes((ushort)(value == 0 ? 0xFFFF : value)).CopyTo(Data, 0x1C - (index * 2));
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
        public void SetBlank(byte? lang, byte? ver) => new Mail4(lang: lang, ver: ver).Data.CopyTo(Data, 0);
    }
}