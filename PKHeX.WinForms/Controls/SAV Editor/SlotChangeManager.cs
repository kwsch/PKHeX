using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Orchestrates the movement of slots within the GUI.
/// </summary>
public sealed class SlotChangeManager(SAVEditor se) : IDisposable
{
    public readonly SAVEditor SE = se;
    public readonly SlotTrackerImage LastSlot = new();
    public readonly DragManager Drag = new();
    public SaveDataEditor<PictureBox> Env { get; set; } = null!;

    public readonly List<BoxEditor> Boxes = [];
    public readonly SlotHoverHandler Hover = new();

    public void Reset()
    {
        Drag.Initialize();
        LastSlot.Reset();
    }

    public void MouseEnter(object? sender, EventArgs e)
    {
        if (sender is not PictureBox pb)
            return;
        bool dataPresent = pb.Image is not null;
        if (dataPresent)
            Hover.Start(pb, LastSlot);
        pb.Cursor = dataPresent ? Cursors.Hand : Cursors.Default;
    }

    public void MouseLeave(object? sender, EventArgs e)
    {
        Hover.Stop();
    }

    public void MouseClick(object? sender, MouseEventArgs e)
    {
        if (sender == null)
            return;
        if (!Drag.Info.DragDropInProgress)
            SE.ClickSlot(sender, e);
    }

    public void MouseUp(object? sender, MouseEventArgs e)
    {
        if (sender == null)
            return;
        if (e.Button == MouseButtons.Left)
            Drag.Info.LeftMouseIsDown = false;
        Drag.Info.Source = null;
    }

    public void MouseDown(object? sender, MouseEventArgs e)
    {
        if (sender == null)
            return;
        if (e.Button == MouseButtons.Left)
        {
            Drag.Info.LeftMouseIsDown = true;
            Drag.MouseDownPosition = Cursor.Position;
        }
    }

    public void QueryContinueDrag(object? sender, QueryContinueDragEventArgs e)
    {
        if (sender == null)
            return;
        if (e.Action != DragAction.Cancel && e.Action != DragAction.Drop)
            return;
        Drag.Info.LeftMouseIsDown = false;
        Drag.Info.DragDropInProgress = false;
    }

    public void DragEnter(object? sender, DragEventArgs e)
    {
        if (sender == null)
            return;
        if ((e.AllowedEffect & DragDropEffects.Copy) != 0) // external file
            e.Effect = DragDropEffects.Copy;
        else if (e.Data != null) // within
            e.Effect = DragDropEffects.Move;

        if (Drag.Info.DragDropInProgress)
            Drag.SetCursor(((Control)sender).FindForm(), Drag.Info.Cursor);
    }

    private static SlotViewInfo<T> GetSlotInfo<T>(T pb) where T : Control
    {
        var view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<T>>(pb);
        ArgumentNullException.ThrowIfNull(view);
        var src = view.GetSlotData(pb);
        return new SlotViewInfo<T>(src, view);
    }

    public void MouseMove(object? sender, MouseEventArgs e)
    {
        if (!Drag.CanStartDrag)
        {
            Hover.UpdateMousePosition(e.Location);
            return;
        }
        if (sender is not PictureBox pb)
            return;

        // Abort if there is no Pokémon in the given slot.
        if (pb.Image == null)
            return;
        bool encrypt = Control.ModifierKeys == Keys.Control;
        HandleMovePKM(pb, encrypt);
    }

    public void DragDrop(object? sender, DragEventArgs e)
    {
        if (sender is not PictureBox pb)
            return;
        var info = GetSlotInfo(pb);
        if (!info.CanWriteTo() || Drag.Info.Source?.CanWriteTo() == false)
        {
            SystemSounds.Asterisk.Play();
            e.Effect = DragDropEffects.Copy;
            Drag.Reset();
            return;
        }

        var mod = SlotUtil.GetDropModifier();
        Drag.Info.Destination = info;
        HandleDropPKM(pb, e, mod);
    }

    private void HandleMovePKM(PictureBox pb, bool encrypt)
    {
        // Create a temporary PKM file to perform a drag drop operation.

        // Set flag to prevent re-entering.
        Drag.Info.DragDropInProgress = true;

        // Prepare Data
        Drag.Info.Source = GetSlotInfo(pb);
        Drag.Info.Destination = null;

        // Make a new file name based off the PID
        string newfile = CreateDragDropPKM(pb, encrypt, out bool external);

        // drop finished, clean up
        Drag.Info.Source = null;
        Drag.Reset();
        Drag.ResetCursor(pb.FindForm());

        // Browser apps need time to load data since the file isn't moved to a location on the user's local storage.
        // Tested 10ms -> too quick, 100ms was fine. 500ms should be safe?
        // Keep it to 20 seconds; Discord upload only stores the file path until you click Upload.
        int delay = external ? 20_000 : 0;
        DeleteAsync(newfile, delay);
        if (Drag.Info.DragIsParty)
            SE.SetParty();
    }

    private async void DeleteAsync(string path, int delay)
    {
        await Task.Delay(delay).ConfigureAwait(true);
        if (!File.Exists(path) || Drag.Info.CurrentPath == path)
            return;

        try { File.Delete(path); }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
    }

    private string CreateDragDropPKM(PictureBox pb, bool encrypt, out bool external)
    {
        // Make File
        var pk = Drag.Info.Source!.ReadCurrent();
        string newfile = FileUtil.GetPKMTempFileName(pk, encrypt);
        try
        {
            var data = encrypt ? pk.EncryptedPartyData : pk.DecryptedPartyData;
            external = TryMakeDragDropPKM(pb, data, newfile);
        }
        // Tons of things can happen with drag & drop; don't try to handle things, just indicate failure.
        catch (Exception x)
        {
            WinFormsUtil.Error("Drag && Drop Error", x);
            external = false;
        }

        return newfile;
    }

    private bool TryMakeDragDropPKM(PictureBox pb, byte[] data, string newfile)
    {
        File.WriteAllBytes(newfile, data);
        var img = (Bitmap)pb.Image;
        Drag.SetCursor(pb.FindForm(), new Cursor(img.GetHicon()));
        Hover.Stop();
        pb.Image = null;
        pb.BackgroundImage = SpriteUtil.Spriter.Drag;

        // Thread Blocks on DoDragDrop
        Drag.Info.CurrentPath = newfile;
        var result = pb.DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Copy);
        var external = Drag.Info.Destination == null || result != DragDropEffects.Link;
        if (external || Drag.Info.SameLocation) // not dropped to another box slot, restore img
        {
            pb.Image = img;
            pb.BackgroundImage = LastSlot.OriginalBackground;
            Drag.ResetCursor(pb.FindForm());
            return external;
        }

        if (result == DragDropEffects.Copy) // viewed in tabs or cloned
        {
            if (Drag.Info.Destination == null) // apply 'view' highlight
                Env.Slots.Get(Drag.Info.Source!.Slot);
            return false;
        }
        return true;
    }

    private void HandleDropPKM(PictureBox pb, DragEventArgs? e, DropModifier mod)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] {Length: not 0} files)
        {
            Drag.Reset();
            return;
        }

        if (Directory.Exists(files[0])) // folder
        {
            SE.LoadBoxes(out string _, files[0]);
            Drag.Reset();
            return;
        }

        e.Effect = mod == DropModifier.Clone ? DragDropEffects.Copy : DragDropEffects.Link;

        // file
        if (Drag.Info.SameLocation)
        {
            e.Effect = DragDropEffects.Link;
            return;
        }

        var dest = Drag.Info.Destination;

        if (Drag.Info.Source == null) // external source
        {
            bool badDest = !dest!.CanWriteTo();
            if (!TryLoadFiles(files, e, badDest))
                WinFormsUtil.Alert(MessageStrings.MsgSaveSlotBadData);
        }
        else if (!TrySetPKMDestination(pb, mod))
        {
            WinFormsUtil.Alert(MessageStrings.MsgSaveSlotEmpty);
        }
        Drag.Reset();
    }

    /// <summary>
    /// Tries to load the input <see cref="files"/>
    /// </summary>
    /// <param name="files">Files to load</param>
    /// <param name="e">Args</param>
    /// <param name="badDest">Destination slot disallows eggs/blanks</param>
    /// <returns>True if loaded</returns>
    private bool TryLoadFiles(ReadOnlySpan<string> files, DragEventArgs e, bool badDest)
    {
        if (files.Length == 0)
            return false;

        var sav = Drag.Info.Destination!.View.SAV;
        var path = files[0];
        var temp = FileUtil.GetSingleFromPath(path, sav);
        if (temp == null)
        {
            Drag.RequestDD(this, e); // pass through
            return true; // treat as handled
        }

        var pk = EntityConverter.ConvertToType(temp, sav.PKMType, out var result);
        if (pk == null)
        {
            var c = result.GetDisplayString(temp, sav.PKMType);
            WinFormsUtil.Error(c);
            Debug.WriteLine(c);
            return false;
        }

        if (badDest && (pk.Species == 0 || pk.IsEgg))
            return false;

        if (sav is ILangDeviantSave il && !EntityConverter.IsCompatibleGB(temp, il.Japanese, pk.Japanese))
        {
            var str = EntityConverterResult.IncompatibleLanguageGB.GetIncompatibleGBMessage(pk, il.Japanese);
            WinFormsUtil.Error(str);
            Debug.WriteLine(str);
            return false;
        }

        var errata = sav.EvaluateCompatibility(pk);
        if (errata.Count > 0)
        {
            string concat = string.Join(Environment.NewLine, errata);
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, concat, MessageStrings.MsgContinue))
            {
                Debug.WriteLine(result.GetDisplayString(temp, sav.PKMType));
                Debug.WriteLine(concat);
                return false;
            }
        }

        Env.Slots.Set(Drag.Info.Destination!.Slot, pk);
        Debug.WriteLine(result.GetDisplayString(temp, sav.PKMType));
        return true;
    }

    private bool TrySetPKMDestination(PictureBox pb, DropModifier mod)
    {
        var info = Drag.Info;
        var pk = info.Source!.ReadCurrent();
        var msg = Drag.Info.Destination!.CanWriteTo(pk);
        if (msg != WriteBlockedMessage.None)
            return false;

        if (Drag.Info.Source != null)
            TrySetPKMSource(mod);

        // Copy from temp to destination slot.
        var type = info.DragIsSwap ? SlotTouchType.Swap : SlotTouchType.Set;
        Env.Slots.Set(info.Destination!.Slot, pk, type);
        Drag.ResetCursor(pb.FindForm());
        return true;
    }

    private bool TrySetPKMSource(DropModifier mod)
    {
        var info = Drag.Info;
        var dest = info.Destination;
        if (dest == null || mod == DropModifier.Clone)
            return false;

        if (dest.IsEmpty() || mod == DropModifier.Overwrite)
        {
            Env.Slots.Delete(info.Source!.Slot);
            return true;
        }

        var type = info.DragIsSwap ? SlotTouchType.Swap : SlotTouchType.Set;
        var pk = dest.ReadCurrent();
        Env.Slots.Set(Drag.Info.Source!.Slot, pk, type);
        return true;
    }

    // Utility
    public void SwapBoxes(int index, int other, SaveFile SAV)
    {
        if (index == other)
            return;
        SAV.SwapBox(index, other);
        UpdateBoxViewAtBoxIndexes(index, other);
    }

    public void Dispose()
    {
        Hover.Dispose();
        SE.Dispose();
        LastSlot.OriginalBackground?.Dispose();
        LastSlot.CurrentBackground?.Dispose();
    }

    private void UpdateBoxViewAtBoxIndexes(params int[] boxIndexes)
    {
        foreach (var box in Boxes)
        {
            var current = box.CurrentBox;
            if (!boxIndexes.Contains(current))
                continue;
            box.ResetSlots();
            box.ResetBoxNames(current);
        }
    }
}
