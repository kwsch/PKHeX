using System;
using static PKHeX.Core.OPower6Type;

namespace PKHeX.Core
{
    public class OPower6
    {
        private static readonly OPowerFlagSet[] Mapping =
        {
            new OPowerFlagSet(5, Hatching),
            new OPowerFlagSet(5, Bargain),
            new OPowerFlagSet(5, Prize_Money),
            new OPowerFlagSet(5, Exp_Point),
            new OPowerFlagSet(5, Capture),

            new OPowerFlagSet(3, Encounter),
            new OPowerFlagSet(3, Stealth),
            new OPowerFlagSet(3, HP_Restoring),
            new OPowerFlagSet(3, PP_Restoring),

            new OPowerFlagSet(1, Full_Recovery),

            new OPowerFlagSet(5, Befriending),

            new OPowerFlagSet(3, Attack),
            new OPowerFlagSet(3, Defense),
            new OPowerFlagSet(3, Sp_Attack),
            new OPowerFlagSet(3, Sp_Defense),
            new OPowerFlagSet(3, Speed),
            new OPowerFlagSet(3, Critical),
            new OPowerFlagSet(3, Accuracy),
        };
        static OPower6()
        {
            int index = 1; // Skip unused byte
            foreach (var m in Mapping)
            {
                m.Offset = index;
                index += m.Count;
            }
        }

        private readonly byte[] Data;
        private readonly int Offset;
        public OPower6(byte[] data, int offset)
        {
            Offset = offset;
            Data = data;
        }

        public int GetOPowerCount(OPower6Type type)
        {
            var m = Array.Find(Mapping, t => t.Identifier == type);
            return m.Count;
        }

        public int GetOPowerLevel(OPower6Type type)
        {
            var m = Array.Find(Mapping, t => t.Identifier == type);
            return m.GetOPowerLevel(Data, Offset);
        }
        public void SetOPowerLevel(OPower6Type type, int lvl)
        {
            var m = Array.Find(Mapping, t => t.Identifier == type);
            m.SetOPowerLevel(Data, Offset, lvl);
        }
        public bool MasterFlag
        {
            get => Data[Offset] == 1;
            set => Data[Offset] = (byte) (value ? OPowerFlagState.Unlocked : OPowerFlagState.Locked);
        }

        public void UnlockAll() => ToggleFlags(allEvents: true);
        public void UnlockRegular() => ToggleFlags();
        public void ClearAll() => ToggleFlags(clearOnly: true);
        private void ToggleFlags(bool allEvents = false, bool clearOnly = false)
        {
            foreach (var m in Mapping)
            {
                // Clear before applying new value
                m.SetOPowerLevel(Data, Offset, 0);

                if (clearOnly)
                    continue;

                int lvl = allEvents ? m.Count : (m.Count != 1 ? 3 : 0); // Full_Recovery is event only @ 1 level
                m.SetOPowerLevel(Data, Offset, lvl);
            }
        }

        public byte[] Write() => Data;
    }
}
