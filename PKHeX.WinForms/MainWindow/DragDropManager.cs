using PKHeX.Core;

namespace PKHeX.WinForms
{
    public class DragDropManager
    {
        private readonly SaveFile SAV;

        public bool LeftMouseIsDown;
        public bool RightMouseIsDown;
        public bool DragDropInProgress;

        public object Cursor;
        public string CurrentPath;

        public DragLocation Source = new DragLocation();
        public DragLocation Destination = new DragLocation();

        public DragDropManager(SaveFile sav)
        {
            SAV = sav;
            Source.Data = SAV.BlankPKM.EncryptedPartyData;
        }

        public class DragLocation
        {
            public object Parent;
            public byte[] Data;
            public int Offset = -1;
            public int Slot = -1;
            public int Box = -1;

            public bool IsParty => 30 <= Slot && Slot < 36;
            public bool IsValid => Slot > -1 && (Box > -1 || IsParty);
        }

        public bool SameBox => Source.Box > -1 && Source.Box == Destination.Box;
        public bool SameSlot => Source.Slot == Destination.Slot && Source.Box == Destination.Box;

        // PKM Get Set
        public PKM GetPKM(bool src)
        {
            var slot = src ? Source : Destination;
            int o = slot.Offset;
            return slot.IsParty ? SAV.getPartySlot(o) : SAV.getStoredSlot(o);
        }
        public void SetPKM(PKM pk, bool src)
        {
            var slot = src ? Source : Destination;
            int o = slot.Offset;
            if (!slot.IsParty)
            { SAV.setStoredSlot(pk, o); return; }

            if (src)
            {
                if (pk.Species == 0) // Empty Slot
                {
                    SAV.deletePartySlot(Source.Slot - 30);
                    return;
                }
            }
            else
            {
                if (30 + SAV.PartyCount < slot.Slot)
                {
                    o = SAV.getPartyOffset(SAV.PartyCount);
                    slot.Slot = 30 + SAV.PartyCount;
                }
            }

            if (pk.Stat_HPMax == 0) // Without Stats (Box)
            {
                pk.setStats(pk.getStats(SAV.Personal.getFormeEntry(pk.Species, pk.AltForm)));
                pk.Stat_Level = pk.CurrentLevel;
            }
            SAV.setPartySlot(pk, o);
        }

        public bool? WasDragParticipant(object form, int index)
        {
            if (Destination.Box != index && Source.Box != index)
                return null; // form was not watching box
            return Source.Parent == form || Destination.Parent == form; // form already updated?
        }

        public void Reset()
        {
            LeftMouseIsDown = RightMouseIsDown = DragDropInProgress = false;
            Cursor = CurrentPath = null;
            Source = new DragLocation();
            Destination = new DragLocation();
        }
    }
}
