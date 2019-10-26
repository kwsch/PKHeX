using System;

namespace PKHeX.Core
{
    public class MyStatus6 : SaveBlock
    {
        public MyStatus6(SaveFile sav, int offset) : base(sav) => Offset = offset;

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

        public int MultiplayerSpriteID_1
        {
            get => Data[Offset + 6];
            set => Data[Offset + 6] = (byte)value;
        }

        public int MultiplayerSpriteID_2
        {
            get => Data[Offset + 7];
            set => Data[Offset + 7] = (byte)value;
        }

        public const int GameSyncIDSize = 16; // 64 bits

        public string GameSyncID
        {
            get => Util.GetHexStringFromBytes(Data, Offset + 0x08, GameSyncIDSize / 2);
            set
            {
                if (value.Length != GameSyncIDSize)
                    throw new ArgumentException(nameof(value));

                var data = Util.GetBytesFromHexString(value);
                SAV.SetData(data, Offset + 0x08);
            }
        }

        public int SubRegion
        {
            get => Data[Offset + 0x26];
            set => Data[Offset + 0x26] = (byte)value;
        }

        public int Country
        {
            get => Data[Offset + 0x27];
            set => Data[Offset + 0x27] = (byte)value;
        }

        public decimal Latitude // don't use the setters
        {
            get => (BitConverter.ToInt16(Data, Offset + 0x28) * 180m) / 0x8000;
            set => SAV.SetData(BitConverter.GetBytes((short)(value * 0x8000) / 180), Offset + 0x28);
        }

        public decimal Longitude // don't use the setters
        {
            get => (BitConverter.ToInt16(Data, Offset + 0x2A) * 180m) / 0x8000;
            set => SAV.SetData(BitConverter.GetBytes((short)(value * 0x8000) / 180), Offset + 0x2A);
        }

        public int ConsoleRegion
        {
            get => Data[Offset + 0x2C];
            set => Data[Offset + 0x2C] = (byte)value;
        }

        public int Language
        {
            get => Data[Offset + 0x2D];
            set => Data[Offset + 0x2D] = (byte)value;
        }

        public string OT
        {
            get => SAV.GetString(Offset + 0x48, 0x1A);
            set => SAV.SetData(SAV.SetString(value, SAV.OTLength), Offset + 0x48);
        }

        private int GetSayingOffset(int say) => Offset + 0x7C + (SAV6.LongStringLength * say);
        private string GetSaying(int say) => SAV.GetString(GetSayingOffset(say), SAV6.LongStringLength);
        private void SetSaying(int say, string value) => SAV.SetData(SAV.SetString(value, SAV6.LongStringLength / 2), GetSayingOffset(say));

        public string Saying1 { get => GetSaying(0); set => SetSaying(0, value); }
        public string Saying2 { get => GetSaying(1); set => SetSaying(1, value); }
        public string Saying3 { get => GetSaying(2); set => SetSaying(2, value); }
        public string Saying4 { get => GetSaying(3); set => SetSaying(3, value); }
        public string Saying5 { get => GetSaying(4); set => SetSaying(4, value); }

        public bool IsMegaEvolutionUnlocked
        {
            get => (Data[Offset + 0x14A] & 0x01) != 0;
            set => Data[Offset + 0x14A] = (byte)((Data[Offset + 0x14A] & 0xFE) | (value ? 1 : 0)); // in battle
        }

        public bool IsMegaRayquazaUnlocked
        {
            get => (Data[Offset + 0x14A] & 0x02) != 0;
            set => Data[Offset + 0x14A] = (byte)((Data[Offset + 0x14A] & ~2) | (value ? 2 : 0)); // in battle
        }
    }
}