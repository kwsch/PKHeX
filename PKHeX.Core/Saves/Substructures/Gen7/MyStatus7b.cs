using System;

namespace PKHeX.Core
{
    public sealed class MyStatus7b : SaveBlock
    {
        public MyStatus7b(SAV7b sav, int offset) : base(sav) => Offset = offset;

        // Player Information

        // idb uint8 offset: 0x58

        public int TID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0);
        }

        public int SID
        {
            get => BitConverter.ToUInt16(Data, Offset + 2);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 2);
        }

        public int Game
        {
            get => Data[Offset + 4];
            set => Data[Offset + 4] = (byte)value;
        }

        public int Gender
        {
            get => Data[Offset + 5];
            set => Data[Offset + 5] = OverworldGender = (byte)value;
        }

        public const int GameSyncIDSize = 16; // 8 bytes

        public string GameSyncID
        {
            get => Util.GetHexStringFromBytes(Data, Offset + 0x10, GameSyncIDSize / 2);
            set
            {
                if (value.Length > 16)
                    throw new ArgumentException(nameof(value));

                var data = Util.GetBytesFromHexString(value);
                SAV.SetData(data, Offset + 0x10);
            }
        }

        public int Language
        {
            get => Data[Offset + 0x35];
            set => Data[Offset + 0x35] = (byte)value;
        }

        public string OT
        {
            get => SAV.GetString(Offset + 0x38, 0x1A);
            set => SAV.SetString(value, SAV.OTLength).CopyTo(Data, Offset + 0x38);
        }

        public byte StarterGender
        {
            get => Data[Offset + 0x0B9];
            set => Data[Offset + 0x0B9] = value;
        }

        public byte OverworldGender // Model
        {
            get => Data[Offset + 0x108];
            set => Data[Offset + 0x108] = value;
        }
    }
}