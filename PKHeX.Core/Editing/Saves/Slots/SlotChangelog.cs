using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Maintains undo and redo history for changes made to a <see cref="SaveFile"/>.
/// </summary>
public sealed record SlotChangelog(SaveFile Parent)
{
    private readonly Stack<ISlotReversion> _undo = new();
    private readonly Stack<ISlotReversion> _redo = new();

    public bool CanUndo => _undo.Count != 0;
    public bool CanRedo => _redo.Count != 0;

    /// <summary>
    /// Begins tracking a change affecting one slot.
    /// </summary>
    public Change Begin(ISlotInfo info) => Begin([info]);

    /// <summary>
    /// Begins tracking a change affecting multiple slots.
    /// The resulting change is represented by a single undo/redo entry.
    /// </summary>
    public Change Begin(params IEnumerable<ISlotInfo> slots)
    {
        var reversion = CreateReversion(slots, Parent);
        return new Change(this, reversion);
    }

    /// <summary>
    /// Undoes the most recent committed change.
    /// </summary>
    /// <returns>The slots affected by the change.</returns>
    public IReadOnlyList<ISlotInfo> Undo()
    {
        if (!CanUndo)
            return [];

        var change = _undo.Pop();

        // Capture the state that exists immediately before the undo.
        // That state becomes the redo operation.
        var redo = change.CreateInverse(Parent);

        change.Revert(Parent);
        _redo.Push(redo);

        return change.Slots;
    }

    /// <summary>
    /// Redoes the most recently undone change.
    /// </summary>
    /// <returns>The slots affected by the change.</returns>
    public IReadOnlyList<ISlotInfo> Redo()
    {
        if (!CanRedo)
            return [];

        var change = _redo.Pop();

        // Capture the state that exists immediately before the redo.
        // That state becomes the next undo operation.
        var undo = change.CreateInverse(Parent);

        change.Revert(Parent);
        _undo.Push(undo);

        return change.Slots;
    }

    private void Commit(ISlotReversion slotReversion)
    {
        _undo.Push(slotReversion);
        _redo.Clear();
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    // ReSharper disable once UnusedParameter.Local
    private void Discard(ISlotReversion slotReversion)
    {
        // A discarded change was never committed, so there is nothing to do.
        // This method exists to keep Change's lifecycle explicit.
    }

    private static ISlotReversion CreateReversion(IEnumerable<ISlotInfo> slots, SaveFile parent)
    {
        var reversions = slots.Select(info => CreateReversion(info, parent)).ToArray();
        return reversions.Length switch
        {
            0 => throw new ArgumentException("At least one slot is required.", nameof(slots)),
            1 => reversions[0],
            _ => new CompositeSlotReversion(reversions),
        };
    }

    private static ISlotReversion CreateReversion(ISlotInfo info, SaveFile parent) => info switch
    {
        SlotInfoParty party => new PartySlotReversion(party, parent),
        _ => new SlotSlotReversion(info, parent),
    };

    /// <summary>
    /// Represents a change being prepared for commit.
    /// Capture the change before modifying the save, then call <see cref="Commit"/>
    /// after the modification succeeds.
    /// </summary>
    public sealed class Change(SlotChangelog owner, ISlotReversion slotReversion) : IDisposable
    {
        private bool _isCompleted;

        /// <summary>
        /// Commits the change to the undo history.
        /// </summary>
        public void Commit()
        {
            if (_isCompleted)
                throw new InvalidOperationException("The change has already been completed.");

            _isCompleted = true;
            owner.Commit(slotReversion);
        }

        /// <summary>
        /// Restores the state captured when the change began and discards it.
        /// </summary>
        public void Rollback()
        {
            if (_isCompleted)
                throw new InvalidOperationException("The change has already been completed.");

            _isCompleted = true;
            slotReversion.Revert(owner.Parent);
            owner.Discard(slotReversion);
        }

        /// <summary>
        /// Discards the change without restoring anything.
        /// Use this when no mutation was performed.
        /// </summary>
        public void Cancel()
        {
            if (_isCompleted)
                throw new InvalidOperationException("The change has already been completed.");

            _isCompleted = true;
            owner.Discard(slotReversion);
        }

        public void Dispose()
        {
            // An uncommitted change is automatically rolled back.
            if (!_isCompleted)
                Rollback();
        }
    }

    public interface ISlotReversion
    {
        IReadOnlyList<ISlotInfo> Slots { get; }
        ISlotReversion CreateInverse(SaveFile parent);
        void Revert(SaveFile parent);
    }

    private sealed class SlotSlotReversion(ISlotInfo info, SaveFile parent) : ISlotReversion
    {
        private readonly PKM _entity = info.Read(parent).Clone();
        public IReadOnlyList<ISlotInfo> Slots => [info];
        public ISlotReversion CreateInverse(SaveFile parent) => new SlotSlotReversion(info, parent);
        public void Revert(SaveFile parent) => info.WriteTo(parent, _entity.Clone(), EntityImportSettings.None);
    }

    private sealed class PartySlotReversion(SlotInfoParty info, SaveFile parent) : ISlotReversion
    {
        private readonly PKM[] _party = CloneParty(parent.PartyData);
        public IReadOnlyList<ISlotInfo> Slots => [info];
        public ISlotReversion CreateInverse(SaveFile parent) => new PartySlotReversion(info, parent);
        public void Revert(SaveFile parent) => parent.PartyData = CloneParty(_party);
        private static PKM[] CloneParty(IEnumerable<PKM> party) => [.. party.Select(pk => pk.Clone())];
    }

    private sealed class CompositeSlotReversion : ISlotReversion
    {
        private readonly IReadOnlyList<ISlotReversion> Reversions;

        public CompositeSlotReversion(params IReadOnlyList<ISlotReversion> reversions)
        {
            if (reversions.Count == 0)
                throw new ArgumentException("At least one reversion is required.", nameof(reversions));
            Reversions = reversions;
        }

        public IReadOnlyList<ISlotInfo> Slots => [.. Reversions.SelectMany(x => x.Slots)];

        public ISlotReversion CreateInverse(SaveFile parent)
        {
            // Capture every inverse before modifying the save.
            var inverse = Reversions
                .Select(x => x.CreateInverse(parent))
                .ToArray();

            return new CompositeSlotReversion(inverse);
        }

        public void Revert(SaveFile parent)
        {
            foreach (var reversion in Reversions)
                reversion.Revert(parent);
        }
    }
}
