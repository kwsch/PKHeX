using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    internal enum SlotIndex
    {
        Party = 0,
        BattleBox = 6,
        Daycare = 12,
    }

    public static partial class Extensions
    {
        internal static bool IsEditable(this SlotIndex type) => type == SlotIndex.Party;
        internal static bool IsParty(this SlotIndex type, int format) => type < SlotIndex.BattleBox || (format == 5 && type == SlotIndex.BattleBox);

        internal static SlotIndex GetMiscSlotType(int slot)
        {
            if (slot < (int)SlotIndex.BattleBox) return SlotIndex.Party;
            if (slot < (int)SlotIndex.Daycare) return SlotIndex.BattleBox;
            return SlotIndex.Daycare;
        }

        internal static StorageSlotType GetMiscSlotType(this SlotIndex type)
        {
            switch (type)
            {
                case SlotIndex.Party: return StorageSlotType.Party;
                case SlotIndex.Daycare: return StorageSlotType.Daycare;
                case SlotIndex.BattleBox: return StorageSlotType.BattleBox;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
