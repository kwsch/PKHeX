using System;
using System.Linq;

namespace PKHeX.Core
{
    public class Mail3 : Mail
    {
        private const int SIZE = 0x24;

        public Mail3(SAV3 sav, int index)
        {
            GetMailBlockOffset(sav.Version, index, out int block, out int offset);
            DataOffset = (index * SIZE) + sav.GetBlockOffset(block) + offset;
            Data = sav.GetData(DataOffset, SIZE);
        }

        private static void GetMailBlockOffset(GameVersion game, int index, out int block, out int offset)
        {
            block = 3;
            if (game == GameVersion.E)
            {
                offset = 0xCE0;
            }
            else if (GameVersion.RS.Contains(game))
            {
                offset = 0xC4C;
            }
            else // FRLG
            {
                if (index >= 12)
                {
                    block = 4;
                    offset = 0;
                }
                else
                {
                    offset = 0xDD0;
                }
            }
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
            {
                for (int x = 0; x < 3; x++)
                    SetMessage(y, x, 0xFFFF);
            }

            AuthorName = string.Empty;
            AuthorTID = 0;
            AuthorTID = 0;
            AppearPKM = 1;
            MailType = 0;
        }

        public override ushort GetMessage(int index1, int index2) => BitConverter.ToUInt16(Data, ((index1 * 3) + index2) * 2);
        public override void SetMessage(int index1, int index2, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, ((index1 * 3) + index2) * 2);

        public override string AuthorName
        {
            get => StringConverter3.GetString3(Data, 0x12, 7, false);
            set
            {
                if (value.Length == 0)
                {
                    Enumerable.Repeat<byte>(0xFF, 8).ToArray().CopyTo(Data, 0x12);
                }
                else
                {
                    Data[0x18] = Data[0x19] = 0xFF;
                    StringConverter3.SetString3(value, 7, false, 6).CopyTo(Data, 0x12);
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