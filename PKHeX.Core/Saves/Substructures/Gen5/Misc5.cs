using System;

namespace PKHeX.Core
{
    public abstract class Misc5 : SaveBlock, IGymTeamInfo
    {
        protected Misc5(SAV5 sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => BitConverter.ToUInt32(Data, Offset);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset);
        }

        public int Badges
        {
            get => Data[Offset + 0x4];
            set => Data[Offset + 0x4] = (byte)value;
        }

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

    public sealed class Misc5BW : Misc5
    {
        public Misc5BW(SAV5BW sav, int offset) : base(sav, offset) { }
        protected override int BadgeVictoryOffset => 0x58; // thru 0xB7
    }

    public sealed class Misc5B2W2 : Misc5
    {
        public Misc5B2W2(SAV5B2W2 sav, int offset) : base(sav, offset) { }
        protected override int BadgeVictoryOffset => 0x5C; // thru 0xBB
    }
}