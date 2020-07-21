using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Party Data <see cref="ISlotInfo"/>
    /// </summary>
    public sealed class SlotInfoParty : ISlotInfo
    {
        public int Slot { get; private set; }
        public bool CanWriteTo(SaveFile sav) => sav.HasParty;

        public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pkm) => pkm.IsEgg && sav.IsPartyAllEggs(Slot)
            ? WriteBlockedMessage.InvalidPartyConfiguration
            : WriteBlockedMessage.None;

        public SlotInfoParty(int slot) => Slot = slot;

        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault)
        {
            if (pkm.Species == 0)
            {
                sav.DeletePartySlot(Slot);
                Slot = sav.PartyCount;
                return true;
            }
            Slot = Math.Min(Slot, sav.PartyCount); // realign if necessary
            sav.SetPartySlotAtIndex(pkm, Slot, setting, setting);
            return true;
        }

        public PKM Read(SaveFile sav) => sav.GetPartySlotAtIndex(Slot);

        private bool Equals(SlotInfoParty other) => Slot == other.Slot;
        public bool Equals(ISlotInfo other) => other is SlotInfoParty p && Equals(p);
        public override bool Equals(object obj) => obj is SlotInfoParty p && Equals(p);
        public override int GetHashCode() => Slot;
    }
}