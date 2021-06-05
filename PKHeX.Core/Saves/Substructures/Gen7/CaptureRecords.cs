using System;

namespace PKHeX.Core
{
    public sealed class CaptureRecords : SaveBlock
    {
        public CaptureRecords(SAV7b sav, int offset) : base(sav) => Offset = offset;

        private const int ENTRY_COUNT = 153;
        private const int MAX_COUNT_ENTRY_CAPTURE = 9_999;
        private const int MAX_COUNT_ENTRY_TRANSFER = 999_999_999;
        private const int MAX_COUNT_TOTAL = 999_999_999;

        // 0x468A8 to 0x46B0B contains 153 entries (u32) denoting how many of said Species you've caught (each cap out at 9,999).
        private const int CapturedOffset = 0x2A8;
        // 0x46B0C to 0x46D6F contains 153 entries (u32) denoting how many of said Species you've transferred to Professor Oak (each cap out at 999,999,999).
        private const int TransferredOffset = CapturedOffset + (ENTRY_COUNT * sizeof(uint)); // 0x50C

        // 0x770 ??

        // 0x46D94 is a u32 stores how many total Pokémon you've caught (caps out at 999,999,999).
        private const int TotalCapturedOffset = 0x794;

        // 0x46DA8 is a u32 that stores how many Pokémon you've transferred to Professor Oak.
        // This value is equal to the sum of all individual transferred Species, but caps out at 999,999,999 even if the sum of all individual Species exceeds this.
        private const int TotalTransferredOffset = 0x7A8;

        // Calling into these directly, you should be sure that you're less than ENTRY_COUNT.
        private int GetCapturedOffset(int index) => Offset + CapturedOffset + (index * 4);
        private int GetTransferredOffset(int index) => Offset + TransferredOffset + (index * 4);
        public uint GetCapturedCountIndex(int index) => BitConverter.ToUInt32(Data, GetCapturedOffset(index));
        public uint GetTransferredCountIndex(int index) => BitConverter.ToUInt32(Data, GetTransferredOffset(index));
        public void SetCapturedCountIndex(int index, uint value) => BitConverter.GetBytes(Math.Min(MAX_COUNT_ENTRY_CAPTURE, value)).CopyTo(Data, GetCapturedOffset(index));
        public void SetTransferredCountIndex(int index, uint value) => BitConverter.GetBytes(Math.Min(MAX_COUNT_ENTRY_TRANSFER, value)).CopyTo(Data, GetTransferredOffset(index));

        public static int GetSpeciesIndex(int species) => (uint)species switch
        {
            <= (int) Species.Mew => species - 1,
            (int) Species.Meltan or (int) Species.Melmetal => species - 657, // 151, 152
            _ => -1
        };

        public static int GetIndexSpecies(int index)
        {
            if (index < (int) Species.Mew)
                return index + 1;
            return index + 657;
        }

        public uint GetCapturedCount(int species)
        {
            int index = GetSpeciesIndex(species);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(species));
            return GetCapturedCountIndex(index);
        }

        public void SetCapturedCount(int species, uint value)
        {
            int index = GetSpeciesIndex(species);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(species));
            SetCapturedCountIndex(index, value);
        }

        public uint GetTransferredCount(int species)
        {
            int index = GetSpeciesIndex(species);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(species));
            return GetTransferredCountIndex(index);
        }

        public void SetTransferredCount(int species, uint value)
        {
            int index = GetSpeciesIndex(species);
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(species));
            SetTransferredCountIndex(index, value);
        }

        public uint TotalCaptured
        {
            get => BitConverter.ToUInt32(Data, Offset + TotalCapturedOffset);
            set => BitConverter.GetBytes(Math.Min(MAX_COUNT_TOTAL, value)).CopyTo(Data, Offset + TotalCapturedOffset);
        }

        public uint TotalTransferred
        {
            get => BitConverter.ToUInt32(Data, Offset + TotalTransferredOffset);
            set => BitConverter.GetBytes(Math.Min(MAX_COUNT_TOTAL, value)).CopyTo(Data, Offset + TotalTransferredOffset);
        }

        public uint CalculateTotalCaptured()
        {
            uint total = 0;
            for (int i = 0; i < ENTRY_COUNT; i++)
                total += GetCapturedCountIndex(i);
            return Math.Min(total, MAX_COUNT_TOTAL);
        }

        public uint CalculateTotalTransferred()
        {
            ulong total = 0;
            for (int i = 0; i < ENTRY_COUNT; i++)
                total += GetTransferredCountIndex(i);
            return (uint)Math.Min(total, MAX_COUNT_TOTAL);
        }

        public void SetAllCaptured(uint count = MAX_COUNT_ENTRY_CAPTURE, Zukan7b? dex = null)
        {
            uint total = 0;
            count = Math.Min(count, MAX_COUNT_ENTRY_CAPTURE);
            for (int i = 0; i < ENTRY_COUNT; i++)
            {
                int species = GetIndexSpecies(i);
                if (count != 0 && dex?.GetCaught(species) == false)
                {
                    total += GetCapturedCountIndex(i);
                    continue;
                }
                SetCapturedCountIndex(i, count);
                total += count;
            }
            if (total < TotalCaptured)
                TotalCaptured = total;
        }

        public void SetAllTransferred(uint count = MAX_COUNT_ENTRY_TRANSFER, Zukan7b? dex = null)
        {
            uint total = 0;
            count = Math.Min(count, MAX_COUNT_ENTRY_TRANSFER);
            for (int i = 0; i < ENTRY_COUNT; i++)
            {
                int species = GetIndexSpecies(i);
                if (count != 0 && dex?.GetCaught(species) == false)
                {
                    total += GetTransferredCountIndex(i);
                    continue;
                }
                SetTransferredCountIndex(i, count);
                total += count;
            }
            if (total < TotalTransferred)
                TotalTransferred = total;
        }
    }
}
