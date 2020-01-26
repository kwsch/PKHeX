using System;

namespace PKHeX.Core
{
    public sealed class Puff6 : SaveBlock
    {
        private const int MaxPuffID = 26; // Supreme Winter Poké Puff
        private const int PuffSlots = 100;

        public Puff6(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

        public byte[] Puffs
        {
            get => SAV.GetData(Offset, PuffSlots);
            set => SAV.SetData(value, Offset);
        }

        public int PuffCount
        {
            get => BitConverter.ToInt32(Data, Offset + PuffSlots);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + PuffSlots);
        }

        public void Reset()
        {
            Array.Clear(Data, Offset, PuffSlots);
            // Set the first few default Puffs
            Data[Offset + 0] = 1;
            Data[Offset + 1] = 2;
            Data[Offset + 2] = 3;
            Data[Offset + 3] = 4;
            Data[Offset + 4] = 5;
            PuffCount = 5;
        }

        public void MaxCheat(bool special = false)
        {
            var rnd = Util.Rand;
            if (special)
            {
                for (int i = 0; i < PuffSlots; i++)
                    Data[Offset + i] = (byte)(21 + rnd.Next(2)); // Supreme Wish or Honor
            }
            else
            {
                for (int i = 0; i < PuffSlots; i++)
                    Data[Offset + i] = (byte)((i % MaxPuffID) + 1);
                Util.Shuffle(Data, Offset, Offset + PuffSlots, rnd);
            }
            PuffCount = PuffSlots;
        }

        public void Sort(bool reverse = false)
        {
            Array.Sort(Data, Offset, PuffCount);
            if (reverse)
                Array.Reverse(Data, Offset, PuffCount);
        }
    }

    public sealed class BattleBox6 : SaveBlock
    {
        public BattleBox6(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

        private int LockedFlagOffset => Offset + (6 * PokeCrypto.SIZE_6STORED);

        public bool Locked
        {
            get => LockedWiFiTournament || LockedLiveTournament;
            set => LockedWiFiTournament = LockedLiveTournament = value;
        }

        public bool LockedWiFiTournament
        {
            get => (Data[LockedFlagOffset] & 1) != 0;
            set => Data[LockedFlagOffset] = (byte)((Data[Offset + LockedFlagOffset] & ~1) | (value ? 1 : 0));
        }

        public bool LockedLiveTournament
        {
            get => (Data[LockedFlagOffset] & 2) != 0;
            set => Data[LockedFlagOffset] = (byte)((Data[Offset + LockedFlagOffset] & ~2) | (value ? 2 : 0));
        }
    }
}