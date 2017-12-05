using System;
using System.Linq;

namespace PKHeX.Core
{
    public class Mail3 : Mail
    {
        private const int SIZE = 0x24;

        public Mail3(SAV3 sav, int index)
        {
            DataOffset = index * SIZE + sav.GetBlockOffset(3) + 0xCE0;
            Data = sav.GetData(DataOffset, SIZE);
        }
        public Mail3()
        {
            Data = new byte[SIZE];
            DataOffset = -1;
            ResetData();
        }
        private void ResetData()
        {
            for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                SetMessage(y, x, 0xFFFF);
            AuthorName = "";
            AuthorTID = 0;
            AuthorTID = 0;
            AppearPKM = 1;
            MailType = 0;
        }
        public override ushort GetMessage(int index1, int index2) => BitConverter.ToUInt16(Data, (index1 * 3 + index2) * 2);
        public override void SetMessage(int index1, int index2, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, (index1 * 3 + index2) * 2);
        public override string AuthorName
        {
            get => StringConverter.GetString3(Data, 0x12, 7, false);
            set
            {
                if (value.Length == 0)
                    Enumerable.Repeat<byte>(0xFF, 8).ToArray().CopyTo(Data, 0x12);
                else
                {
                    Data[0x18] = Data[0x19] = 0xFF;
                    StringConverter.SetString3(value, 7, false, 6).CopyTo(Data, 0x12);
                }
            }
        }
        public override ushort AuthorTID { get => BitConverter.ToUInt16(Data, 0x1A); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1A); }
        public ushort AuthorSID { get => BitConverter.ToUInt16(Data, 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1C); }
        public override int AppearPKM { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes((ushort)(value == 0 ? 1 : value)).CopyTo(Data, 0x1E); }
        public override int MailType { get => BitConverter.ToUInt16(Data, 0x20); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x20); }
        public override bool? IsEmpty
        {
            get
            {
                if (MailType == 0) return true;
                else if (MailType >= 0x79 && MailType <= 0x84) return false;
                else return null;
            }
        }
        public override void SetBlank() => (new Mail3()).Data.CopyTo(Data, 0);
    }
}