using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Tracks <see cref="PKM"/> slot changes and provides the ability to revert a change.
/// </summary>
public sealed class SlotChangelog(SaveFile SAV)
{
    private readonly Stack<SlotReversion> UndoStack = new();
    private readonly Stack<SlotReversion> RedoStack = new();

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

    private static SlotReversion GetReversion(ISlotInfo info, SaveFile sav) => info switch
    {
        SlotInfoParty p => new PartyReversion(p, sav),
        _ => new SingleSlotReversion(info, sav),
    };

    private abstract class SlotReversion(ISlotInfo Info)
    {
        internal readonly ISlotInfo Info = Info;
        public abstract void Revert(SaveFile sav);
    }

    private sealed class PartyReversion(ISlotInfo info, IList<PKM> Party) : SlotReversion(info)
    {
        public PartyReversion(ISlotInfo info, SaveFile s) : this(info, s.PartyData) { }

        public override void Revert(SaveFile sav) => sav.PartyData = Party;
    }

    private sealed class SingleSlotReversion(ISlotInfo info, PKM Entity) : SlotReversion(info)
    {
        public SingleSlotReversion(ISlotInfo info, SaveFile sav) : this(info, info.Read(sav)) { }

        public override void Revert(SaveFile sav) => Info.WriteTo(sav, Entity, PKMImportSetting.Skip);
    }
}
