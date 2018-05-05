using System.Collections.Generic;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public interface ISlotViewer<T>
    {
        SlotChange GetSlotData(T view);
        SlotChangeManager M { get; }
        IList<T> SlotPictureBoxes { get; }
        int ViewIndex { get; }
    }
}
