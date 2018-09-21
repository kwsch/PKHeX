using System;
using System.Diagnostics;

namespace PKHeX.Core
{
    internal class OPowerFlagSet
    {
        public readonly OPower6Type Identifier;
        public readonly int Count;
        public int Offset;
        public int BaseCount => Math.Min(3, Count);
        public bool HasOPowerS => Count > 3;
        public bool HasOPowerMAX => Count == 5;

        public OPowerFlagSet(int count, OPower6Type ident)
        {
            Identifier = ident;
            Count = count;
        }

        public int GetOPowerLevel(byte[] data, int offset)
        {
            for (int i = 0; i < BaseCount; i++)
            {
                if (GetFlag(data, offset, i))
                    continue;
                Debug.WriteLine($"Fetched {Identifier}: {i}");
                return i;
            }

            Debug.WriteLine($"Fetched {Identifier}: {BaseCount}");
            return BaseCount;
        }

        public void SetOPowerLevel(byte[] data, int offset, int value)
        {
            Debug.WriteLine($"Setting {Identifier}: {value}");
            for (int i = 0; i < BaseCount; i++)
                SetFlag(data, offset, i, i + 1 <= value);
            Debug.Assert(value == GetOPowerLevel(data, offset));
        }

        public bool GetOPowerS(byte[] data, int offset) => HasOPowerS && GetFlag(data, offset, 3);
        public bool GetOPowerMAX(byte[] data, int offset) => HasOPowerMAX && GetFlag(data, offset, 4);
        public void SetOPowerS(byte[] data, int offset, bool value) => SetFlag(data, offset, 3, value);
        public void SetOPowerMAX(byte[] data, int offset, bool value) => SetFlag(data, offset, 4, value);

        private bool GetFlag(byte[] data, int offset, int index)
        {
            return data[Offset + offset + index] == (byte)OPowerFlagState.Unlocked;
        }

        private void SetFlag(byte[] data, int offset, int index, bool value)
        {
            if (index < Count)
                data[Offset + offset + index] = (byte)(value ? OPowerFlagState.Unlocked : OPowerFlagState.Locked);
        }
    }
}