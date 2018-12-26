using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public sealed partial class SAV_BoxList : Form
    {
        private readonly List<BoxEditor> Boxes = new List<BoxEditor>();

        public SAV_BoxList(SAVEditor p, SlotChangeManager m)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            // initialize boxes dynamically
            var sav = p.SAV;

            AddControls(p, m, sav);
            SetWindowDimensions(sav.BoxCount);

            AllowDrop = true;
            AddEvents();
            CenterToParent();
            Owner = p.ParentForm;
            FormClosing += (sender, e) =>
            {
                foreach (var b in Boxes)
                    b.M.Boxes.Remove(b);
            };
        }

        private void AddEvents()
        {
            GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            DragEnter += Main_DragEnter;
            DragDrop += (sender, e) =>
            {
                Cursor = DefaultCursor;
                System.Media.SystemSounds.Asterisk.Play();
            };
        }

        private void AddControls(SAVEditor p, SlotChangeManager m, SaveFile sav)
        {
            for (int i = 0; i < sav.BoxCount; i++)
            {
                var boxEditor = new BoxEditor { Name = $"BE_Box{i:00}", Margin = new Padding(1) };
                foreach (PictureBox pb in boxEditor.SlotPictureBoxes)
                    pb.ContextMenuStrip = p.SlotPictureBoxes[0].ContextMenuStrip;
                boxEditor.Setup(m);
                boxEditor.Reset();
                boxEditor.CurrentBox = i;
                boxEditor.CB_BoxSelect.Enabled = false;
                Boxes.Add(boxEditor);
                FLP_Boxes.Controls.Add(Boxes[i]);
            }

            // Setup swapping
            foreach (var box in Boxes)
            {
                box.ClearEvents();
                box.B_BoxLeft.Click += (s, e) =>
                {
                    int index = Boxes.FindIndex(z => z == ((Button)s).Parent);
                    int other = (index + Boxes.Count - 1) % Boxes.Count;
                    m.SwapBoxes(index, other);
                };
                box.B_BoxRight.Click += (s, e) =>
                {
                    int index = Boxes.FindIndex(z => z == ((Button)s).Parent);
                    int other = (index + 1) % Boxes.Count;
                    m.SwapBoxes(index, other);
                };
            }
        }

        private void SetWindowDimensions(int count)
        {
            // adjust layout to stack up boxes
            var sqrt = Math.Sqrt(count);
            int height = Math.Min(5, (int)Math.Ceiling(sqrt));
            int width = (int)Math.Ceiling((float)count / height);
            Debug.Assert(height * width >= count);
            width = Math.Min(4, width);

            var padWidth = (Boxes[0].Margin.Horizontal * 2) + 1;
            Width = ((Boxes[0].Width + padWidth) * width) - (padWidth / 2) + 0x10;

            var padHeight = (Boxes[0].Margin.Vertical * 2) + 1;
            Height = ((Boxes[0].Height + padHeight) * height) - (padHeight / 2);
        }

        private static void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }
    }
}
