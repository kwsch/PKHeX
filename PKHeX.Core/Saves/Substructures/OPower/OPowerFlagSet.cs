using System.Diagnostics;

namespace PKHeX.Core
{
    internal class OPowerFlagSet
    {
        public readonly OPower6Type Identifier;
        public readonly int Count;
        public int Offset;

        public OPowerFlagSet(int count, OPower6Type ident)
        {
            Identifier = ident;
            Count = count;
        }

        public int GetOPowerLevel(byte[] data, int offset)
        {
            for (int i = 0; i < Count; i++)
            {
                if (data[Offset + offset + i] != 0)
                    continue;
                Debug.WriteLine($"Fetched {Identifier}: {i}");
                return i;
            }

            Debug.WriteLine($"Fetched {Identifier}: {Count}");
            return Count;
        }
        public void SetOPowerLevel(byte[] data, int offset, int value)
        {
            Debug.WriteLine($"Setting {Identifier}: {value}");
            for (int i = 0; i < Count; i++)
                data[Offset + offset + i] = (byte)(i + 1 > value ? OPowerFlagState.Locked : OPowerFlagState.Unlocked);
            Debug.Assert(value == GetOPowerLevel(data, offset));
        }
    }
}