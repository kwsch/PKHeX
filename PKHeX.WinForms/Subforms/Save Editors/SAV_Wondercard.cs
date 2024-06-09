using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Drawing.Misc;
using PKHeX.Drawing.PokeSprite;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_Wondercard : Form
{
    private readonly SaveFile Origin;
    private readonly SaveFile SAV;
    private readonly SummaryPreviewer Summary = new();
    private readonly IMysteryGiftStorage Cards;
    private readonly IMysteryGiftFlags? Flags;
    private readonly DataMysteryGift[] Album;

    public SAV_Wondercard(SaveFile sav, DataMysteryGift? g = null)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (Origin = sav).Clone();
        Cards = GetMysteryGiftProvider(SAV);

        Album = LoadMysteryGifts();
        Flags = Cards as IMysteryGiftFlags;

        pba = GetGiftPictureBoxes(SAV.Generation);
        foreach (var pb in pba)
        {
            pb.AllowDrop = true;
            pb.DragDrop += BoxSlot_DragDrop;
            pb.DragEnter += BoxSlot_DragEnter;
            pb.MouseDown += BoxSlot_MouseDown;
            pb.ContextMenuStrip = mnuVSD;
            pb.MouseHover += (_, _) => Summary.Show(pb, Album[pba.IndexOf(pb)]);
            pb.Enter += (_, _) =>
            {
                var index = pba.IndexOf(pb);
                if (index < 0)
                    return;

                var enc = Album[index];
                pb.AccessibleDescription = string.Join(Environment.NewLine, enc.GetTextLines());
            };
        }

        SetGiftBoxes();
        GetReceivedFlags();

        if (LB_Received.Items.Count > 0)
            LB_Received.SelectedIndex = 0;

        if (Album[0] is WR7) // giftused is not a valid prop
            B_UnusedAll.Visible = B_UsedAll.Visible = L_QR.Visible = false;

        DragEnter += Main_DragEnter;
        DragDrop += Main_DragDrop;

        if (g == null)
            ClickView(pba[0], EventArgs.Empty);
        else
            ViewGiftData(g);
    }

    private DataMysteryGift[] LoadMysteryGifts()
    {
        var count = Cards.GiftCountMax;
        var size = SAV is SAV4HGSS ? count + 1 : count;
        var result = new DataMysteryGift[size];
        for (int i = 0; i < count; i++)
            result[i] = Cards.GetMysteryGift(i);
        if (SAV is SAV4HGSS s4)
            result[^1] = s4.LockCapsuleSlot;
        return result;
    }

    private static IMysteryGiftStorage GetMysteryGiftProvider(SaveFile sav)
    {
        if (sav is IMysteryGiftStorageProvider provider)
            return provider.MysteryGiftStorage;
        throw new ArgumentException("Save file does not support Mystery Gifts.", nameof(sav));
    }

    private List<PictureBox> GetGiftPictureBoxes(byte generation) => generation switch
    {
        4 => PopulateViewGiftsG4(),
        5 or 6 or 7 => PopulateViewGiftsG567(),
        _ => throw new ArgumentOutOfRangeException(nameof(generation), generation, "Game not supported."),
    };

    private DataMysteryGift? mg;
    private readonly List<PictureBox> pba; // don't mutate this list

    // Re-population Functions
    private void SetBackground(int index, Image bg)
    {
        for (int i = 0; i < Album.Length; i++)
            pba[i].BackgroundImage = index == i ? bg : null;
    }

    private void SetGiftBoxes()
    {
        for (int i = 0; i < Album.Length; i++)
            pba[i].Image = Album[i].Sprite();
    }

    private void ViewGiftData(DataMysteryGift g)
    {
        try
        {
            // only check if the form is visible (not opening)
            if (Visible && g.GiftUsed)
            {
                var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgMsyteryGiftUsedAlert, MsgMysteryGiftUsedFix);
                if (prompt == DialogResult.Yes)
                    g.GiftUsed = false;
            }

            RTB.Lines = g.GetDescription().ToArray();
            PB_Preview.Image = g.Sprite();
            mg = g;
        }
        // Some user input mystery gifts can have out-of-bounds values. Just swallow any exception.
        catch (Exception e)
        {
            RTB.Clear();
            WinFormsUtil.Error(MsgMysteryGiftParseTypeUnknown, e);
        }
    }

    private void GetReceivedFlags()
    {
        LB_Received.Items.Clear();
        if (Flags is not { } f)
            return;
        var count = f.MysteryGiftReceivedFlagMax;
        for (int i = 1; i < count; i++)
        {
            if (f.GetMysteryGiftReceivedFlag(i))
                LB_Received.Items.Add(i.ToString("0000"));
        }

        if (LB_Received.Items.Count > 0)
            LB_Received.SelectedIndex = 0;
    }

    private void SetCardID(int cardID)
    {
        if (Flags is null || (uint)cardID >= Flags.MysteryGiftReceivedFlagMax)
            return;

        string card = cardID.ToString("0000");
        if (!LB_Received.Items.Contains(card))
            LB_Received.Items.Add(card);
        LB_Received.SelectedIndex = LB_Received.Items.IndexOf(card);
    }

    // Mystery Gift IO (.file<->window)
    private void B_Import_Click(object sender, EventArgs e)
    {
        var fileFilter = WinFormsUtil.GetMysterGiftFilter(SAV.Context);
        using var import = new OpenFileDialog();
        import.Filter = fileFilter;
        if (import.ShowDialog() != DialogResult.OK)
            return;

        var path = import.FileName;
        var data = File.ReadAllBytes(path);
        var ext = Path.GetExtension(path.AsSpan());
        var gift = MysteryGift.GetMysteryGift(data, ext);
        if (gift == null)
        {
            WinFormsUtil.Error(MsgMysteryGiftInvalid, path);
            return;
        }
        ViewGiftData(gift);
    }

    private void B_Output_Click(object sender, EventArgs e)
    {
        if (mg == null)
            return;
        WinFormsUtil.ExportMGDialog(mg);
    }

    private static int GetLastUnfilledByType(DataMysteryGift gift, ReadOnlySpan<DataMysteryGift> album)
    {
        for (int i = 0; i < album.Length; i++)
        {
            var exist = album[i];
            if (!exist.Empty)
                continue;
            if (exist.Type != gift.Type)
                continue;
            return i;
        }
        return -1;
    }

    // Mystery Gift RW (window<->sav)
    private void ClickView(object sender, EventArgs e)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        if (pb == null)
            return;
        int index = pba.IndexOf(pb);

        SetBackground(index, Drawing.PokeSprite.Properties.Resources.slotView);
        ViewGiftData(Album[index]);
    }

    private void ClickSet(object sender, EventArgs e)
    {
        if (mg is not { } gift)
            return;

        if (!gift.IsCardCompatible(SAV, out var msg))
        {
            WinFormsUtil.Alert(MsgMysteryGiftSlotFail, msg);
            return;
        }

        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        if (pb == null)
            return;
        int index = pba.IndexOf(pb);

        // Hijack to the latest unfilled slot if index creates interstitial empty slots.
        int lastUnfilled = GetLastUnfilledByType(gift, Album);
        if (lastUnfilled > -1 && lastUnfilled < index)
            index = lastUnfilled;
        if (gift is PCD { IsLockCapsule: true })
            index = 11;

        var gifts = Album;
        var other = gifts[index];
        if (gift is PCD { CanConvertToPGT: true } pcd && other is PGT)
        {
            gift = pcd.Gift;
        }
        else if (gift.Type != other.Type)
        {
            WinFormsUtil.Alert(MsgMysteryGiftSlotFail, $"{gift.Type} != {other.Type}");
            return;
        }
        else if (gift is PCD g && (g is { IsLockCapsule: true } != (index == 11)))
        {
            WinFormsUtil.Alert(MsgMysteryGiftSlotFail, $"{GameInfo.Strings.Item[533]} slot not valid.");
            return;
        }
        gifts[index] = (DataMysteryGift)gift.Clone();
        SetBackground(index, Drawing.PokeSprite.Properties.Resources.slotSet);
        SetGiftBoxes();
        SetCardID(gift.CardID);
    }

    private void ClickDelete(object sender, EventArgs e)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        if (pb == null)
            return;
        int index = pba.IndexOf(pb);

        var arr = Album[index].Data;
        Array.Clear(arr, 0, arr.Length);

        // Shuffle blank card down
        int i = index;
        while (i < Album.Length - 1)
        {
            if (Album[i + 1].Empty)
                break;
            if (Album[i + 1].Type != Album[i].Type)
                break;

            i++;

            var mg1 = Album[i];
            var mg2 = Album[i - 1];

            Album[i - 1] = mg1;
            Album[i] = mg2;
        }
        SetBackground(i, Drawing.PokeSprite.Properties.Resources.slotDel);
        SetGiftBoxes();
    }

    // Close Window
    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveReceivedFlags();
        SaveReceivedCards();

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void SaveReceivedCards()
    {
        if (Cards is MysteryBlock4 s4)
        {
            s4.IsDeliveryManActive = Album.Any(g => !g.Empty);
            MysteryBlock4.UpdateSlotPGT(Album, SAV is SAV4HGSS);
            if (SAV is SAV4HGSS hgss)
                hgss.LockCapsuleSlot = (PCD)Album[^1];
        }
        int count = Cards.GiftCountMax;
        for (int i = 0; i < count; i++)
            Cards.SetMysteryGift(i, Album[i]);
        if (Cards is MysteryBlock5 s5)
            s5.EndAccess(); // need to encrypt the at-rest data with the seed.
    }

    private void SaveReceivedFlags()
    {
        if (Flags is null)
            return; // nothing to save

        // Store the list of set flag indexes back to the bitflag array.
        Flags.ClearReceivedFlags();
        foreach (var o in LB_Received.Items)
        {
            if (o?.ToString() is not { } x || !int.TryParse(x, out var index))
                continue;
            Flags.SetMysteryGiftReceivedFlag(index, true);
        }
    }

    // Delete Received Flag
    private void ClearReceivedFlag(object sender, EventArgs e)
    {
        if (LB_Received.SelectedIndex < 0)
            return;

        if (LB_Received.SelectedIndices.Count > 1)
        {
            for (int i = LB_Received.SelectedIndices.Count - 1; i >= 0; i--)
                LB_Received.Items.RemoveAt(LB_Received.SelectedIndices[i]);
        }
        else if (LB_Received.SelectedIndices.Count == 1)
        {
            int lastIndex = LB_Received.SelectedIndex;
            LB_Received.Items.RemoveAt(lastIndex);
            if (LB_Received.Items.Count == 0)
                return;
            if (lastIndex == LB_Received.Items.Count)
                lastIndex--;
            LB_Received.SelectedIndex = lastIndex;
        }
    }

    // Drag & Drop Wonder Cards
    private static void Main_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e?.Data is null)
            return;
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
    }

    private void Main_DragDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;

        var first = files[0];
        // Check for multiple wondercards
        if (Directory.Exists(first))
            files = Directory.GetFiles(first, "*", SearchOption.AllDirectories);

        if (files.Length == 1 && !Directory.Exists(files[0]))
        {
            string path = files[0]; // open first D&D
            if (!MysteryGift.IsMysteryGift(new FileInfo(path).Length)) // arbitrary
            {
                WinFormsUtil.Alert(MsgMysteryGiftInvalid, path);
                return;
            }
            var gift = MysteryGift.GetMysteryGift(File.ReadAllBytes(path), Path.GetExtension(path));
            if (gift == null)
            {
                WinFormsUtil.Error(MsgMysteryGiftInvalid, path);
                return;
            }
            ViewGiftData(gift);
            return;
        }
        SetGiftBoxes();
    }

    private void ClickQR(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Alt)
        {
            string url = Clipboard.GetText();
            if (!string.IsNullOrWhiteSpace(url))
            {
                ImportQRToView(url);
                return;
            }
        }
        ExportQRFromView();
    }

    private void ExportQRFromView()
    {
        if (mg == null)
            return;
        if (mg.Empty)
        {
            WinFormsUtil.Alert(MsgMysteryGiftSlotNone);
            return;
        }
        if (SAV.Generation == 6 && mg.ItemID == 726 && mg.IsItem)
        {
            WinFormsUtil.Alert(MsgMysteryGiftQREonTicket, MsgMysteryGiftQREonTicketAdvice);
            return;
        }

        Image qr = QREncode.GenerateQRCode(mg);

        string desc = $"({mg.Type}) {string.Join(Environment.NewLine, mg.GetDescription())}";

        using var form = new QR(qr, PB_Preview.Image, desc + Environment.NewLine + "PKHeX Wonder Card @ ProjectPokemon.org");
        form.ShowDialog();
    }

    private void ImportQRToView(string url)
    {
        var msg = QRDecode.GetQRData(url, out var data);
        if (msg != 0)
        {
            WinFormsUtil.Alert(msg.ConvertMsg());
            return;
        }

        if (data.Length == 0)
            return;

        string[] types = Album.Select(g => g.Type).Distinct().ToArray();
        var gift = MysteryGift.GetMysteryGift(data);
        if (gift == null)
            return;

        string giftType = gift.Type;

        if (Album.All(card => card.Data.Length != data.Length))
            WinFormsUtil.Alert(MsgMysteryGiftQRTypeLength, string.Format(MsgQRDecodeSize, $"0x{data.Length:X}"));
        else if (types.All(type => type != giftType))
            WinFormsUtil.Alert(MsgMysteryGiftTypeIncompatible, $"{MsgMysteryGiftQRReceived} {gift.Type}{Environment.NewLine}{MsgMysteryGiftTypeUnexpected} {string.Join(", ", types)}");
        else if (!SAV.CanReceiveGift(gift))
            WinFormsUtil.Alert(MsgMysteryGiftTypeDetails);
        else
            ViewGiftData(gift);
    }

    private async void BoxSlot_MouseDown(object? sender, MouseEventArgs e)
    {
        if (sender == null)
            return;
        switch (ModifierKeys)
        {
            case Keys.Control: ClickView(sender, e); return;
            case Keys.Shift: ClickSet(sender, e); return;
            case Keys.Alt: ClickDelete(sender, e); return;
        }
        var pb = sender as PictureBox;
        if (pb?.Image == null)
            return;

        if (e.Button != MouseButtons.Left || e.Clicks != 1)
            return;

        int index = pba.IndexOf(pb);
        var gift = Album[index];
        if (gift.Empty)
            return;

        // Create Temp File to Drag
        wc_slot = index;
        Cursor.Current = Cursors.Hand;
        string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(gift.FileName));
        try
        {
            await File.WriteAllBytesAsync(newfile, gift.Write()).ConfigureAwait(true);
            DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Copy | DragDropEffects.Move);
        }
        // Sometimes the drag-drop is canceled or ends up at a bad location. Don't bother recovering from an exception; just display a safe error message.
        catch (Exception x)
        { WinFormsUtil.Error("Drag & Drop Error", x); }
        wc_slot = -1;
        await DeleteAsync(newfile, 20_000).ConfigureAwait(false);
    }

    private static async Task DeleteAsync(string path, int delay)
    {
        await Task.Delay(delay).ConfigureAwait(true);
        if (!File.Exists(path))
            return;

        try { File.Delete(path); }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
    }

    private void BoxSlot_DragDrop(object? sender, DragEventArgs? e)
    {
        if (mg == null || sender is not PictureBox pb)
            return;

        int index = pba.IndexOf(pb);

        // Hijack to the latest unfilled slot if index creates interstitial empty slots.
        int lastUnfilled = GetLastUnfilledByType(mg, Album);
        if (lastUnfilled > -1 && lastUnfilled < index && Album[lastUnfilled].Type == Album[index].Type)
            index = lastUnfilled;
        if (mg is PCD { IsLockCapsule: true })
            index = 11;

        if (wc_slot == -1) // dropped
        {
            if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
                return;

            var first = files[0];
            var fi = new FileInfo(first);
            if (!MysteryGift.IsMysteryGift(fi.Length))
            { WinFormsUtil.Alert(MsgFileUnsupported, first); return; }

            byte[] data = File.ReadAllBytes(first);
            var gift = MysteryGift.GetMysteryGift(data, fi.Extension);
            if (gift == null)
            { WinFormsUtil.Alert(MsgFileUnsupported, first); return; }

            ref var dest = ref Album[index];
            if (gift is PCD { CanConvertToPGT: true } pcd && dest is PGT)
            {
                gift = pcd.Gift;
            }
            else if (gift.Type != dest.Type)
            {
                WinFormsUtil.Alert(MsgMysteryGiftSlotFail, $"{gift.Type} != {dest.Type}");
                return;
            }
            SetBackground(index, Drawing.PokeSprite.Properties.Resources.slotSet);
            dest = (DataMysteryGift)gift.Clone();

            SetCardID(dest.CardID);
            ViewGiftData(dest);
        }
        else // Swap Data
        {
            index = SwapSlots(index, wc_slot);
            if (index == -1)
                return;
        }
        SetBackground(index, Drawing.PokeSprite.Properties.Resources.slotView);
        SetGiftBoxes();
    }

    private int SwapSlots(int dest, int src)
    {
        var gifts = Album;
        var s1 = gifts[dest];
        var s2 = gifts[src];

        // Double check compatibility of slots
        if (s1.Type != s2.Type)
        {
            if (s2 is PCD { CanConvertToPGT: true } && s1 is PGT)
            {
                // Get first empty slot
                var firstEmpty = Array.FindIndex(gifts, static z => z.Empty);
                if ((uint)firstEmpty < dest)
                    dest = firstEmpty;

                // set the PGT to the destination PGT slot instead
                ViewGiftData(s2);
                ClickSet(pba[dest], EventArgs.Empty);
                WinFormsUtil.Alert(string.Format(MsgMysteryGiftSlotAlternate, s2.Type, s1.Type));
            }
            else
            {
                WinFormsUtil.Alert(string.Format(MsgMysteryGiftSlotFailSwap, s2.Type, s1.Type));
            }
            return -1;
        }

        if ((s1 is PCD && dest == 11) || (s2 is PCD && src == 11))
        {
            WinFormsUtil.Alert(MsgMysteryGiftSlotFail, $"{GameInfo.Strings.Item[533]} swap not valid.");
            return -1;
        }

        // If data is present in both slots, just swap.
        if (!s1.Empty)
        {
            // Swap
            (gifts[src], gifts[dest]) = (s1, s2);
            return dest;
        }

        // empty slot created, bubble this slot to the end of its list
        for (int i = src; i != dest; i++)
        {
            if (gifts[i + 1].Empty)
                return i; // done bubbling
            (gifts[i + 1], gifts[i]) = (gifts[i], gifts[i + 1]);
        }
        throw new InvalidOperationException(); // shouldn't ever hit here.
    }

    private static void BoxSlot_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
            e.Effect = DragDropEffects.Copy;
        else if (e.Data != null) // within
            e.Effect = DragDropEffects.Move;
        Debug.WriteLine(e.Effect);
    }

    private int wc_slot = -1;

    // UI Generation
    private List<PictureBox> PopulateViewGiftsG4()
    {
        List<PictureBox> pb = [];
        var spriter = SpriteUtil.Spriter;

        // Row 1
        var f1 = GetFlowLayoutPanel();
        f1.Controls.Add(GetLabel($"{nameof(PGT)} 1-6"));
        for (int i = 0; i < 6; i++)
        {
            var p = GetPictureBox(spriter.Width, spriter.Height, $"PGT {i + 1}");
            f1.Controls.Add(p);
            pb.Add(p);
        }
        // Row 2
        var f2 = GetFlowLayoutPanel();
        f2.Controls.Add(GetLabel($"{nameof(PGT)} 7-8"));
        for (int i = 6; i < 8; i++)
        {
            var p = GetPictureBox(spriter.Width, spriter.Height, $"PGT {i + 1}");
            f2.Controls.Add(p);
            pb.Add(p);
        }
        // Row 3
        var f3 = GetFlowLayoutPanel();
        f3.Margin = new Padding(0, 12, 0, 0);
        f3.Controls.Add(GetLabel($"{nameof(PCD)} 1-3"));
        for (int i = 8; i < 11; i++)
        {
            var p = GetPictureBox(spriter.Width, spriter.Height, $"PCD {i - 7}");
            f3.Controls.Add(p);
            pb.Add(p);
        }

        FLP_Gifts.Controls.Add(f1);
        FLP_Gifts.Controls.Add(f2);
        FLP_Gifts.Controls.Add(f3);

        if (Album.Length == 12) // lock capsule
        {
            // Row 4
            var f4 = GetFlowLayoutPanel();
            f4.Controls.Add(GetLabel(GameInfo.Strings.Item[533])); // Lock Capsule
            {
                var p = GetPictureBox(spriter.Width, spriter.Height, "PCD Lock Capsule");
                f4.Controls.Add(p);
                pb.Add(p);
            }
            FLP_Gifts.Controls.Add(f4);
        }
        return pb;
    }

    private List<PictureBox> PopulateViewGiftsG567()
    {
        List<PictureBox> pb = [];

        const int cellsPerRow = 6;
        int rows = (int)Math.Ceiling(Album.Length / (decimal)cellsPerRow);
        int countRemaining = Album.Length;
        var spriter = SpriteUtil.Spriter;

        for (int i = 0; i < rows; i++)
        {
            var row = GetFlowLayoutPanel();
            int count = cellsPerRow >= countRemaining ? countRemaining : cellsPerRow;
            countRemaining -= count;
            int start = (i * cellsPerRow) + 1;
            row.Controls.Add(GetLabel($"{start}-{start + count - 1}"));
            for (int j = 0; j < count; j++)
            {
                var p = GetPictureBox(spriter.Width, spriter.Height, $"Row {i} Slot {start + j}");
                row.Controls.Add(p);
                pb.Add(p);
            }
            FLP_Gifts.Controls.Add(row);
        }
        return pb;
    }

    private static FlowLayoutPanel GetFlowLayoutPanel() => new()
    {
        Width = 490,
        Height = 60,
        Padding = new Padding(0),
        Margin = new Padding(0),
    };

    private static Label GetLabel(string text) => new()
    {
        Size = new Size(50, 60),
        AutoSize = false,
        TextAlign = ContentAlignment.MiddleRight,
        Text = text,
        Padding = new Padding(0),
        Margin = new Padding(0),
    };

    private static SelectablePictureBox GetPictureBox(int width, int height, string name) => new()
    {
        Size = new Size(width + 2, height + 2), // +1 to each side for the FixedSingle border
        SizeMode = PictureBoxSizeMode.CenterImage,
        BorderStyle = BorderStyle.FixedSingle,
        BackColor = SlotUtil.GoodDataColor,
        Padding = new Padding(0),
        Margin = new Padding(1),
        Name = name,
        AccessibleName = name,
        AccessibleRole = AccessibleRole.Graphic,
    };

    private void B_ModifyAll_Click(object sender, EventArgs e)
    {
        foreach (var g in Album)
            g.GiftUsed = sender == B_UsedAll;
        SetGiftBoxes();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void LB_Received_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            if (LB_Received.SelectedIndices.Count > 1)
            {
                for (int i = LB_Received.SelectedIndices.Count - 1; i >= 0; i--)
                    LB_Received.Items.RemoveAt(LB_Received.SelectedIndices[i]);
            }
            else if (LB_Received.SelectedIndices.Count == 1)
            {
                int lastIndex = LB_Received.SelectedIndex;
                LB_Received.Items.RemoveAt(lastIndex);
                if (LB_Received.Items.Count == 0)
                    return;
                if (lastIndex == LB_Received.Items.Count)
                    lastIndex--;
                LB_Received.SelectedIndex = lastIndex;
            }
        }
    }
}
