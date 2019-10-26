using System;
using static PKHeX.Core.OPower6Type;

namespace PKHeX.Core
{
    public sealed class OPower6 : SaveBlock
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

        public OPower6(SaveFile sav, int offset) : base(sav) => Offset = offset;

        private static OPowerFlagSet Get(OPower6Type type) => Array.Find(Mapping, t => t.Identifier == type);
        public static int GetOPowerCount(OPower6Type type) => Get(type).BaseCount;
        public int GetOPowerLevel(OPower6Type type) => Get(type).GetOPowerLevel(Data, Offset);

        public static bool GetHasOPowerS(OPower6Type type) => Get(type).HasOPowerS;
        public static bool GetHasOPowerMAX(OPower6Type type) => Get(type).HasOPowerMAX;
        public bool GetOPowerS(OPower6Type type) => Get(type).GetOPowerS(Data, Offset);
        public bool GetOPowerMAX(OPower6Type type) => Get(type).GetOPowerMAX(Data, Offset);

        public void SetOPowerLevel(OPower6Type type, int lvl) => Get(type).SetOPowerLevel(Data, Offset, lvl);
        public void SetOPowerS(OPower6Type type, bool value) => Get(type).SetOPowerS(Data, Offset, value);
        public void SetOPowerMAX(OPower6Type type, bool value) => Get(type).SetOPowerMAX(Data, Offset, value);

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
                m.SetOPowerS(Data, Offset, false);
                m.SetOPowerMAX(Data, Offset, false);

                if (clearOnly)
                    continue;

                int lvl = allEvents ? m.BaseCount : (m.BaseCount != 1 ? 3 : 0); // Full_Recovery is ORAS/event only @ 1 level
                m.SetOPowerLevel(Data, Offset, lvl);
                if (!allEvents)
                    continue;

                m.SetOPowerS(Data, Offset, true);
                m.SetOPowerMAX(Data, Offset, true);
            }
        }

        public byte[] Write() => Data;
    }
}
