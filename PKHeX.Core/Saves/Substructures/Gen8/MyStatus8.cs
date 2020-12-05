using System;
using System.Text;

namespace PKHeX.Core
{
    public sealed class MyStatus8 : SaveBlock
    {
        public const uint MaxWatt = 9999999;

        public MyStatus8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        public string Number
        {
            get => Encoding.ASCII.GetString(Data, 0x01, 3);
            set
            {
                for (int i = 0; i < 3; i++)
                    Data[0x01 + i] = (byte)(value.Length > i ? value[i] : '\0');
                SAV.State.Edited = true;
            }
        }

        public ulong Skin // aka the base model
        {
            get => BitConverter.ToUInt64(Data, 0x08);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x08);
        }

        public ulong Hair
        {
            get => BitConverter.ToUInt64(Data, 0x10);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x10);
        }

        public ulong Brow
        {
            get => BitConverter.ToUInt64(Data, 0x18);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x18);
        }

        public ulong Lashes
        {
            get => BitConverter.ToUInt64(Data, 0x20);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x20);
        }

        public ulong Contacts
        {
            get => BitConverter.ToUInt64(Data, 0x28);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x28);
        }

        public ulong Lips
        {
            get => BitConverter.ToUInt64(Data, 0x30);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x30);
        }

        public ulong Glasses
        {
            get => BitConverter.ToUInt64(Data, 0x38);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x38);
        }

        public ulong Hat
        {
            get => BitConverter.ToUInt64(Data, 0x40);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x40);
        }

        public ulong Jacket
        {
            get => BitConverter.ToUInt64(Data, 0x48);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x48);
        }

        public ulong Top
        {
            get => BitConverter.ToUInt64(Data, 0x50);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x50);
        }

        public ulong Bag
        {
            get => BitConverter.ToUInt64(Data, 0x58);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x58);
        }

        public ulong Gloves
        {
            get => BitConverter.ToUInt64(Data, 0x60);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x60);
        }

        public ulong BottomOrDress
        {
            get => BitConverter.ToUInt64(Data, 0x68);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x68);
        }

        public ulong Sock
        {
            get => BitConverter.ToUInt64(Data, 0x70);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x70);
        }

        public ulong Shoe
        {
            get => BitConverter.ToUInt64(Data, 0x78);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x78);
        }

        // 80 - 87

        public ulong MomSkin // aka the base model
        {
            get => BitConverter.ToUInt64(Data, 0x88);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x88);
        }

        // 8C - 9F

        public int TID
        {
            get => BitConverter.ToUInt16(Data, 0xA0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA0);
        }

        public int SID
        {
            get => BitConverter.ToUInt16(Data, 0xA2);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xA2);
        }

        public int Game
        {
            get => Data[0xA4];
            set => Data[0xA4] = (byte)value;
        }

        public int Gender
        {
            get => Data[0xA5];
            set => Data[0xA5] = (byte)value;
        }

        // A6
        public int Language
        {
            get => Data[Offset + 0xA7];
            set
            {
                if (value == Language)
                    return;
                Data[Offset + 0xA7] = (byte) value;

                // For runtime language, the game shifts all languages above Language 6 (unused) down one.
                if (value >= 6)
                    value--;
                ((SAV8SWSH)SAV).SetValue(SaveBlockAccessor8SWSH.KGameLanguage, (uint)value);
            }
        }

        public string OT
        {
            get => SAV.GetString(Data, 0xB0, 0x1A);
            set => SAV.SetData(Data, SAV.SetString(value, SAV.OTLength), 0xB0);
        }

        // D0
        public uint Watt
        {
            get => BitConverter.ToUInt32(Data, Offset + 0xD0);
            set
            {
                if (value > MaxWatt)
                    value = MaxWatt;
                SAV.SetData(Data, BitConverter.GetBytes(value), Offset + 0xD0);
            }
        }
    }
}
