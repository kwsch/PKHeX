using System;

namespace PKHeX.Core
{
    /// <summary>
    /// SUBE block that stores in-game event results.
    /// </summary>
    public abstract class SubEventLog6 : SaveBlock, IGymTeamInfo
    {
        protected SubEventLog6(SAV6 sav, int offset) : base(sav) => Offset = offset;

        protected abstract int BadgeVictoryOffset { get; }

        private int GetBadgeVictorySpeciesOffset(uint badge, uint slot)
        {
            if (badge >= 8)
                throw new ArgumentException(nameof(badge));
            if (slot >= 6)
                throw new ArgumentException(nameof(slot));

            return Offset + BadgeVictoryOffset + (int)(((6 * badge) + slot) * sizeof(ushort));
        }

        public ushort GetBadgeVictorySpecies(uint badge, uint slot)
        {
            var ofs = GetBadgeVictorySpeciesOffset(badge, slot);
            return BitConverter.ToUInt16(Data, ofs);
        }

        public void SetBadgeVictorySpecies(uint badge, uint slot, ushort species)
        {
            var ofs = GetBadgeVictorySpeciesOffset(badge, slot);
            SAV.SetData(BitConverter.GetBytes(species), ofs);
        }
    }

    public sealed class SubEventLog6XY : SubEventLog6
    {
        public SubEventLog6XY(SAV6XY sav, int offset) : base(sav, offset) { }
        protected override int BadgeVictoryOffset => 0x2C; // thru 0x8B
    }

    public sealed class SubEventLog6AO : SubEventLog6
    {
        public SubEventLog6AO(SAV6AO sav, int offset) : base(sav, offset) { }
        protected override int BadgeVictoryOffset => 0x60; // thru 0xBF
    }
}