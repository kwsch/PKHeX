using System;

namespace PKHeX.Core
{
    public sealed class MyStatus7 : SaveBlock
    {
        public const int GameSyncIDSize = 16; // 64 bits
        public const int NexUniqueIDSize = 32; // 128 bits

        public MyStatus7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public MyStatus7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

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
            set => Data[Offset + 5] = (byte)value;
        }

        public string GameSyncID
        {
            get => Util.GetHexStringFromBytes(Data, Offset + 0x10, GameSyncIDSize / 2);
            set
            {
                if (value.Length != GameSyncIDSize)
                    throw new ArgumentException(nameof(value));

                var data = Util.GetBytesFromHexString(value);
                SAV.SetData(data, Offset + 0x10);
            }
        }

        public string NexUniqueID
        {
            get => Util.GetHexStringFromBytes(Data, Offset + 0x18, NexUniqueIDSize / 2);
            set
            {
                if (value.Length != NexUniqueIDSize)
                    throw new ArgumentException(nameof(value));

                var data = Util.GetBytesFromHexString(value);
                SAV.SetData(data, Offset + 0x18);
            }
        }

        public uint FestaID
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x28);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x28);
        }

        public int SubRegion
        {
            get => Data[Offset + 0x2E];
            set => Data[Offset + 0x2E] = (byte)value;
        }

        public int Country
        {
            get => Data[Offset + 0x2F];
            set => Data[Offset + 0x2F] = (byte)value;
        }

        public int ConsoleRegion
        {
            get => Data[Offset + 0x34];
            set => Data[Offset + 0x34] = (byte)value;
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

        public int DressUpSkinColor
        {
            get => (Data[Offset + 0x54] >> 2) & 7;
            set => Data[Offset + 0x54] = (byte)((Data[Offset + 0x54] & ~(7 << 2)) | (value << 2));
        }

        public int MultiplayerSpriteID
        {
            get => Data[Offset + 0x58];
            set => Data[Offset + 0x58] = (byte)value;
        }

        public bool MegaUnlocked
        {
            get => (Data[Offset + 0x78] & 0x01) != 0;
            set => Data[Offset + 0x78] = (byte)((Data[Offset + 0x78] & 0xFE) | (value ? 1 : 0)); // in battle
        }

        public bool ZMoveUnlocked
        {
            get => (Data[Offset + 0x78] & 2) != 0;
            set => Data[Offset + 0x78] = (byte)((Data[Offset + 0x78] & ~2) | (value ? 2 : 0)); // in battle
        }

        public int BallThrowType
        {
            get => Data[Offset + 0x7A];
            set => Data[Offset + 0x7A] = (byte)(value > 8 ? 0 : value);
        }
    }
}