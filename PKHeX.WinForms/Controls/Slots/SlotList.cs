using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

public partial class SlotList : UserControl, ISlotViewer<PictureBox>
{
    private static readonly string[] names = GetEnumNames();

    public static string[] GetEnumNames()
    {
        var list = Enum.GetNames<StorageSlotType>();
        foreach (ref var item in list.AsSpan())
        {
            if (item.StartsWith("Fused"))
                item = "Fused";
        }
        return list;
    }

    public readonly Label[] Labels = new Label[names.Length];
    private readonly List<PictureBox> slots = [];
    private List<SlotInfoMisc> SlotOffsets = [];
    public int SlotCount { get; private set; }
    public SaveFile SAV { get; set; } = null!;
    public bool FlagIllegal { get; set; }

    public SlotList()
    {
        InitializeComponent();
        AddLabels();
    }

    /// <summary>
    /// Initializes the extra slot viewers with a list of offsets and sets up event handling.
    /// </summary>
    /// <param name="list">Extra slots to show</param>
    /// <param name="enableDragDropContext">Events to set up</param>
    /// <remarks>Uses an object pool for viewers (only generates as needed)</remarks>
    public void Initialize(List<SlotInfoMisc> list, Action<Control> enableDragDropContext)
    {
        SlotOffsets = list;
        LoadSlots(list.Count, enableDragDropContext);
    }

    /// <summary>
    /// Hides all slots from the <see cref="SlotList"/>.
    /// </summary>
    public void HideAllSlots() => LoadSlots(0, _ => { });

    public void NotifySlotOld(ISlotInfo previous)
    {
        if (previous is not SlotInfoMisc m)
            return;
        var index = SlotOffsets.FindIndex(m.Equals);
        if (index < 0)
            return;
        var pb = slots[index];
        pb.BackgroundImage = null;
    }

    public void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pk)
    {
        if (slot is not SlotInfoMisc m)
            return;
        var index = GetViewIndex(m);
        if (index < 0)
            return;
        var pb = slots[index];
        SlotUtil.UpdateSlot(pb, slot, pk, SAV, FlagIllegal, type);
    }

    public int GetViewIndex(ISlotInfo info) => SlotOffsets.FindIndex(info.Equals);

    public ISlotInfo GetSlotData(PictureBox view)
    {
        int slot = GetSlot(view);
        return GetSlotData(slot);
    }

    public ISlotInfo GetSlotData(int slot) => SlotOffsets[slot];

    public IList<PictureBox> SlotPictureBoxes => slots;

    public int GetSlot(PictureBox sender)
    {
        var view = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        if (view == null)
            return -1;
        return slots.IndexOf(view);
    }

    public int ViewIndex { get; set; } = -1;

    private void LoadSlots(int count, Action<Control> enableDragDropContext)
    {
        var controls = FLP_Slots.Controls;
        controls.Clear();
        if (count == 0)
        {
            SlotCount = 0;
            return;
        }
        AddSlots(count, enableDragDropContext);
        AddControls(count);
        SlotCount = count;
    }

    private void AddControls(int countTotal)
    {
        var type = string.Empty;
        int added = -1;
        for (int i = 0; i < countTotal; i++)
        {
            var info = SlotOffsets[i];
            var label = Labels[(int)info.Type];
            if (label.Text != type)
            {
                added++;
                type = label.Text;
                FLP_Slots.Controls.Add(label, 0, added++);
            }

            var slot = slots[i];
            FLP_Slots.Controls.Add(slot, 0, added);
        }
    }

    private void AddSlots(int after, Action<Control> enableDragDropContext)
    {
        int before = SlotCount;
        int diff = after - before;
        if (diff <= 0)
            return;
        for (int i = 0; i < diff; i++)
        {
            var name = $"bpkm{before + i}";
            var slot = GetPictureBox(SpriteUtil.Spriter, name);
            enableDragDropContext(slot);
            slots.Add(slot);
        }
    }

    private const int PadPixels = 2;

    private static SelectablePictureBox GetPictureBox(SpriteBuilder s, string name) => new()
    {
        BorderStyle = BorderStyle.FixedSingle,
        Width = s.Width + 2,
        Height = s.Height + 2,
        AllowDrop = true,
        Margin = new Padding(PadPixels),
        Padding = Padding.Empty,
        SizeMode = PictureBoxSizeMode.CenterImage,
        Name = name,
        AccessibleName = name,
        AccessibleRole = AccessibleRole.Graphic,
    };

    private void AddLabels()
    {
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            Labels[i] = new Label
            {
                Name = $"L_{name}",
                Text = name,
                AutoSize = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
            };
        }
    }
}
