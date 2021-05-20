using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public sealed partial class SAV_GroupViewer : Form
    {
        private readonly SaveFile SAV;
        private readonly IPKMView View;
        private readonly IReadOnlyList<SlotGroup> Groups;
        private readonly SummaryPreviewer Preview = new();

        public int CurrentGroup { get; set; } = -1;

        public SAV_GroupViewer(SaveFile sav, IPKMView view, IReadOnlyList<SlotGroup> groups)
        {
            SAV = sav;
            View = view;
            InitializeComponent();

            Groups = groups;
            int count = groups[0].Slots.Length;
            Regenerate(count);
            CenterToParent();

            MouseWheel += (s, e) => CurrentGroup = e.Delta > 1 ? MoveLeft() : MoveRight();

            var names = groups.Select(z => $"{z.GroupName}").ToArray();
            CB_BoxSelect.Items.AddRange(names);
            CB_BoxSelect.SelectedIndex = GetFirstTeamWithContent(groups);

            foreach (PictureBox pb in Box.Entries)
            {
                pb.Click += (o, args) => OmniClick(pb, args);
                pb.MouseHover += (o, args) => HoverSlot(pb, args);
                pb.ContextMenuStrip = mnu;
            }
        }

        private void HoverSlot(object sender, EventArgs e)
        {
            var group = Groups[CurrentGroup];
            var pb = (PictureBox) sender;
            var index = Box.Entries.IndexOf(pb);
            var slot = group.Slots[index];
            Preview.Show(pb, slot);
        }

        private void OmniClick(object sender, EventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control: ClickView(sender, e); break;
                default:
                    return;
            }
        }

        private static int GetFirstTeamWithContent(IReadOnlyList<SlotGroup> groups)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].Slots.Any(z => z.Species != 0))
                    return i;
            }
            return 0;
        }

        private void Regenerate(int count)
        {
            int deltaW = Width - Box.Width;
            int deltaH = Height - Box.Height;
            var height = count / 5;
            var width = count / height;
            bool changed = Box.InitializeGrid(width, height, SpriteUtil.Spriter);
            if (!changed)
                return;

            Width = Box.Width + deltaW + 2;
            Height = Box.Height + deltaH + 2;

            RecenterControls();
        }

        private void RecenterControls()
        {
            if (Width < Box.Width)
                Width = Box.Width;
            Box.HorizontallyCenter(this);
            Box.Location = new Point(Box.Location.X - 8, Box.Location.Y); // manual fudge
            int p1 = CB_BoxSelect.Location.X;
            CB_BoxSelect.HorizontallyCenter(this);
            int p2 = CB_BoxSelect.Location.X;
            if (p1 == p2)
                return;

            B_BoxLeft.Location = new Point(B_BoxLeft.Location.X + p2 - p1, B_BoxLeft.Location.Y);
            B_BoxRight.Location = new Point(B_BoxRight.Location.X + p2 - p1, B_BoxRight.Location.Y);
        }

        private void LoadGroup(int index)
        {
            if (index == CurrentGroup)
                return;

            var group = Groups[index];
            Regenerate(group.Slots.Length);

            var sav = SAV;
            for (int i = 0; i < group.Slots.Length; i++)
                Box.Entries[i].Image = group.Slots[i].Sprite(sav, -1, -1, true);

            if (slotSelected != -1 && (uint)slotSelected < Box.Entries.Count)
                Box.Entries[slotSelected].BackgroundImage = groupSelected != index ? null : SpriteUtil.Spriter.View;

            CurrentGroup = index;
        }

        public int MoveLeft(bool max = false)
        {
            int newBox = max ? 0 : (CurrentGroup + Groups.Count - 1) % Groups.Count;
            LoadGroup(newBox);
            return newBox;
        }

        public int MoveRight(bool max = false)
        {
            int newBox = max ? Groups.Count - 1 : (CurrentGroup + 1) % Groups.Count;
            LoadGroup(newBox);
            return newBox;
        }

        private int groupSelected = -1;
        private int slotSelected = -1;

        private void ClickView(object sender, EventArgs e)
        {
            var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
            if (pb == null)
                return;
            int index = Box.Entries.IndexOf(pb);

            var group = Groups[CurrentGroup];
            View.PopulateFields(group.Slots[index], false);

            if (slotSelected != index && (uint) slotSelected < Box.Entries.Count)
                Box.Entries[slotSelected].BackgroundImage = null;

            groupSelected = CurrentGroup;
            slotSelected = index;
            Box.Entries[index].BackgroundImage = SpriteUtil.Spriter.View;
        }

        private void B_BoxRight_Click(object sender, EventArgs e) => CB_BoxSelect.SelectedIndex = MoveRight((ModifierKeys & Keys.Control) != 0);
        private void B_BoxLeft_Click(object sender, EventArgs e) => CB_BoxSelect.SelectedIndex = MoveLeft((ModifierKeys & Keys.Control) != 0);
        private void CB_BoxSelect_SelectedIndexChanged(object sender, EventArgs e) => LoadGroup(CB_BoxSelect.SelectedIndex);
    }
}
