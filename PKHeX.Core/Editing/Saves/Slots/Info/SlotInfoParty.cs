using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Party Data <see cref="ISlotInfo"/>
    /// </summary>
    public sealed record SlotInfoParty(int Slot) : ISlotInfo
    {
        public int Slot { get; private set; } = Slot;
        public SlotOrigin Origin => SlotOrigin.Party;
        public bool CanWriteTo(SaveFile sav) => sav.HasParty;

        public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pkm) => pkm.IsEgg && sav.IsPartyAllEggs(Slot)
            ? WriteBlockedMessage.InvalidPartyConfiguration
            : WriteBlockedMessage.None;

        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault)
        {
            if (pkm.Species == 0)
            {
                sav.DeletePartySlot(Slot);
                Slot = Math.Max(0, sav.PartyCount - 1);
                return true;
            }
            Slot = Math.Min(Slot, sav.PartyCount); // realign if necessary
            sav.SetPartySlotAtIndex(pkm, Slot, setting, setting);
            return true;
        }

        public PKM Read(SaveFile sav) => sav.GetPartySlotAtIndex(Slot);
    }
}
