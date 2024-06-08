using System;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms;

public sealed partial class SAV_BoxViewer : Form
{
    private readonly SAVEditor parent;

    public SAV_BoxViewer(SAVEditor p, SlotChangeManager m, int box)
    {
        InitializeComponent();

        parent = p;
        int deltaW = Width - Box.BoxPokeGrid.Width;
        int deltaH = Height - Box.BoxPokeGrid.Height;
        Box.Editor = new BoxEdit(m.SE.SAV);
        Box.Setup(m);
        Box.InitializeGrid();

        Width = Box.BoxPokeGrid.Width + deltaW + 2;
        Height = Box.BoxPokeGrid.Height + deltaH + 2;

        Box.RecenterControls();
        Box.HorizontallyCenter(this);
        Box.Reset();
        CenterToParent();

        AllowDrop = true;
        GiveFeedback += (_, e) => e.UseDefaultCursors = false;
        DragEnter += Main_DragEnter;
        DragDrop += (_, _) =>
        {
            Cursor = DefaultCursor;
            System.Media.SystemSounds.Asterisk.Play();
        };
        Owner = p.ParentForm;

        MouseWheel += (_, e) =>
        {
            if (parent.menu.mnuVSD.Visible)
                return;
            Box.CurrentBox = e.Delta > 1 ? Box.Editor.MoveLeft() : Box.Editor.MoveRight();
        };

        var mnu = parent.SlotPictureBoxes[0].ContextMenuStrip;
        foreach (var pb in Box.SlotPictureBoxes)
            pb.ContextMenuStrip = mnu;

        Box.ResetBoxNames(box); // fix box names
        Box.ResetSlots(); // refresh box background
        p.EditEnv.Slots.Publisher.Subscribers.Add(Box);
    }

    private void PB_BoxSwap_Click(object sender, EventArgs e) => Box.CurrentBox = parent.SwapBoxesViewer(Box.CurrentBox);

    private static void Main_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e is null)
            return;
        if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
            e.Effect = DragDropEffects.Copy;
        else if (e.Data != null) // within
            e.Effect = DragDropEffects.Move;
    }

    private void SAV_BoxViewer_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Remove viewer from manager list
        Box.M?.Boxes.Remove(Box);
        parent.EditEnv.Slots.Publisher.Subscribers.Remove(Box);
    }
}
