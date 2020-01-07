using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Tracks <see cref="PKM"/> slot changes and provides the ability to revert a change.
    /// </summary>
    public sealed class SlotChangelog
    {
        private readonly SaveFile SAV;
        private readonly Stack<SlotReversion> UndoStack = new Stack<SlotReversion>();
        private readonly Stack<SlotReversion> RedoStack = new Stack<SlotReversion>();

        public SlotChangelog(SaveFile sav) => SAV = sav;

        public bool CanUndo => UndoStack.Count != 0;
        public bool CanRedo => RedoStack.Count != 0;

        public void AddNewChange(ISlotInfo info)
        {
            var revert = GetReversion(info, SAV);
            AddUndo(revert);
        }

        public ISlotInfo Undo()
        {
            var change = UndoStack.Pop();
            var revert = GetReversion(change.Info, SAV);
            AddRedo(revert);
            change.Revert(SAV);
            return change.Info;
        }

        public ISlotInfo Redo()
        {
            var change = RedoStack.Pop();
            var revert = GetReversion(change.Info, SAV);
            AddUndo(revert);
            change.Revert(SAV);
            return change.Info;
        }

        private void AddRedo(SlotReversion change)
        {
            RedoStack.Push(change);
        }

        private void AddUndo(SlotReversion change)
        {
            UndoStack.Push(change);
            RedoStack.Clear();
        }

        private static SlotReversion GetReversion(ISlotInfo info, SaveFile sav)
        {
            return info switch
            {
                SlotInfoParty p => new PartyReversion(p, sav),
                _ => new SingleSlotReversion(info, sav)
            };
        }

        private abstract class SlotReversion
        {
            internal readonly ISlotInfo Info;
            protected SlotReversion(ISlotInfo info) => Info = info;

            public abstract void Revert(SaveFile sav);
        }

        private class PartyReversion : SlotReversion
        {
            private readonly IList<PKM> Party;
            public PartyReversion(ISlotInfo info, SaveFile s) : base(info) => Party = s.PartyData;

            public override void Revert(SaveFile sav) => sav.PartyData = Party;
        }

        private class SingleSlotReversion : SlotReversion
        {
            private readonly PKM pkm;
            public SingleSlotReversion(ISlotInfo info, SaveFile sav) : base(info) => pkm = info.Read(sav);

            public override void Revert(SaveFile sav) => Info.WriteTo(sav, pkm, PKMImportSetting.Skip);
        }
    }
}