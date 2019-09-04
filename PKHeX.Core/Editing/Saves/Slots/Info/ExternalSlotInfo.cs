using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="ISlotInfo"/> originating from outside of a save file (e.g. a saved file on a hard drive).
    /// </summary>
    public sealed class ExternalSlotInfo : ISlotInfo
    {
        private readonly PKM Data;
        public ExternalSlotInfo(PKM pkm) => Data = pkm;

        public int Slot { get; } = -1;
        public bool Equals(ISlotInfo other) => false;
        public bool CanWriteTo(SaveFile SAV) => false;
        public WriteBlockedMessage CanWriteTo(SaveFile SAV, PKM pkm) => WriteBlockedMessage.InvalidDestination;
        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault) => throw new InvalidOperationException();

        public PKM Read(SaveFile sav) => Data;
    }

    public sealed class SlotViewExternal<T> : ISlotViewer<T>
    {
        private readonly ExternalSlotInfo Slot;
        public SlotViewExternal(ExternalSlotInfo slot) => Slot = slot;

        public int ViewIndex { get; } = -1;
        public void NotifySlotOld(ISlotInfo previous) { }
        public void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pkm) { }
        public ISlotInfo GetSlotData(T view) => Slot;
        public int GetViewIndex(ISlotInfo slot) => 0;
        public IList<T> SlotPictureBoxes => null;
        public SaveFile SAV => null;
    }
}