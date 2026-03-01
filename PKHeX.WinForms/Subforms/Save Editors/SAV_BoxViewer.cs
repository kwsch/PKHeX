using System;
using System.Drawing;
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
        StartPosition = FormStartPosition.Manual;
        int deltaW = Width - Box.BoxPokeGrid.Width;
        int deltaH = Height - Box.BoxPokeGrid.Height;
        Box.Editor = new BoxEdit(m.SE.SAV);
        Box.Setup(m);
        Box.InitializeGrid();

        if (Application.IsDarkModeEnabled)
        {
            WinFormsTranslator.ReformatDark(Box.B_BoxLeft);
            WinFormsTranslator.ReformatDark(Box.B_BoxRight);
            WinFormsTranslator.ReformatDark(B_BoxSwap);
            WinFormsTranslator.ReformatDark(Box.CB_BoxSelect);
        }

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
        Load += (_, _) => PositionRelativeToParent();

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
        p.EditEnv.Slots.Publisher.Subscribe(Box);
    }

    private void PositionRelativeToParent()
    {
        var parentForm = parent.ParentForm;
        if (parentForm is null)
            return;

        var parentBoxLeft = parent.Box.B_BoxLeft;
        var thisBoxLeft = Box.B_BoxLeft;
        if (!parentBoxLeft.IsHandleCreated || !thisBoxLeft.IsHandleCreated)
            return;

        var parentBoxLeftScreen = parentBoxLeft.PointToScreen(Point.Empty);
        var thisBoxLeftScreen = thisBoxLeft.PointToScreen(Point.Empty);
        var newX = parentForm.Location.X + parentForm.Width;
        var newY = Location.Y + (parentBoxLeftScreen.Y - thisBoxLeftScreen.Y);
        Location = new Point(newX, newY);
    }

    private void PB_BoxSwap_Click(object sender, EventArgs e) => Box.CurrentBox = parent.SwapBoxesViewer(Box.CurrentBox);

    private static void Main_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e is null)
            return;
        if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
            e.Effect = DragDropEffects.Copy;
        else if (e.Data is not null) // within
            e.Effect = DragDropEffects.Move;
    }

    private void SAV_BoxViewer_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Remove viewer from manager list
        Box.M?.Boxes.Remove(Box);
        parent.EditEnv.Slots.Publisher.Unsubscribe(Box);
    }
}
