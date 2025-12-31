using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms;

public partial class SAV_Donut9a : Form
{
    private readonly SAV9ZA Origin;
    private readonly SAV9ZA SAV;
    private readonly DonutPocket9a Donuts;

    private int lastIndex;
    private bool Loading;

    public SAV_Donut9a(SAV9ZA sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9ZA)(Origin = sav).Clone();
        Donuts = SAV.Donuts;

        var strings = GameInfo.Strings;
        donutEditor.InitializeLists(strings.donutFlavor, strings.itemlist, strings.donutName);
        donutEditor.ValueChanged += Editor_ValueChanged;

        Loading = true;
        LoadDonutNames();
        LB_Donut.SelectedIndex = 0;
        Loading = false;

        lastIndex = 0;
        GetEntry(0);

        AddDrop(this, LB_Donut, donutEditor);

        DonutFlavorProfile.BackgroundImage = DonutSpriteUtil.GetFlavorProfileImage();
    }

    private void AddDrop(params ReadOnlySpan<Control> objects)
    {
        foreach (var control in objects)
        {
            control.DragDrop += DragoutDrop;
            control.DragEnter += Dragout_DragOver;
        }
    }

    private void LoadDonutNames()
    {
        // update the list in one shot for less lag
        const int count = DonutPocket9a.MaxCount;
        var names = new object[count];
        for (int i = 0; i < count; i++)
            names[i] = GetDonutName(i);
        LB_Donut.Items.AddRange(names);
    }

    private void ReloadDonutNames()
    {
        for (int i = 0; i < DonutPocket9a.MaxCount; i++)
            LB_Donut.Items[i] = GetDonutName(i);
    }

    private string GetDonutName(int i)
    {
        var donut = Donuts.GetDonut(i);
        return GetDonutName(donut, i);
    }

    private static string GetDonutName(Donut9a donut, int i)
    {
        var flavorCount = donut.FlavorCount;
        var flavorString = new string('*', flavorCount);
        return $"#{i + 1:000} {donut.Stars}⭐ @ {donut.Calories:0000} cal {flavorString}";
    }

    private void Editor_ValueChanged(object? sender, EventArgs e)
    {
        if (Loading)
            return;

        Loading = true;
        var index = lastIndex;
        var donut = Donuts.GetDonut(index);
        // Only refresh the name in the list if it has changed.
        var currentName = GetDonutName(donut, index);
        var existing = LB_Donut.Items[index];
        if (existing.ToString() != currentName)
            LB_Donut.Items[index] = currentName;

        // Update profile if applicable
        donutEditor.SaveDonut();
        DonutFlavorProfile.LoadFromDonut(donut);
        Loading = false;
    }

    private void ChangeIndex(object sender, EventArgs e)
    {
        if (Loading || LB_Donut.SelectedIndex < 0)
            return;

        SetEntry(lastIndex);
        lastIndex = LB_Donut.SelectedIndex;
        GetEntry(lastIndex);
    }

    private void GetEntry(int index)
    {
        if (Loading || index < 0)
            return;

        Loading = true;
        var donut = Donuts.GetDonut(index);
        donutEditor.LoadDonut(donut);
        DonutFlavorProfile.LoadFromDonut(donut);
        Loading = false;
    }

    private void SetEntry(int index)
    {
        if (Loading || index < 0)
            return;

        donutEditor.SaveDonut();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetEntry(lastIndex);
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Modify_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void RandomizeAll(object sender, EventArgs e)
    {
        Donuts.SetAllRandomLv3();
        ReloadDonutNames();
        GetEntry(lastIndex);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void CloneCurrent(object sender, EventArgs e)
    {
        SetEntry(lastIndex);
        Donuts.CloneAllFromIndex(lastIndex);
        ReloadDonutNames();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void ShinyAssortment(object sender, EventArgs e)
    {
        Donuts.SetAllAsShinyTemplate();
        ReloadDonutNames();
        GetEntry(lastIndex);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Reset_Click(object sender, EventArgs e) => donutEditor.Reset();

    private void B_ImportClick(object sender, EventArgs e)
    {
        var current = Donuts.GetDonut(lastIndex);
        var data = current.Data;

        if (!TryLoadDonut(data))
            return;

        donutEditor.LoadDonut(current);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private static bool TryLoadDonut(Span<byte> data)
    {
        if (ModifierKeys == Keys.Control)
            return TryLoadDonutClipboard(data);
        return TryLoadDonutOpenFile(data);
    }

    private static bool TryLoadDonutClipboard(Span<byte> data)
    {
        // Import from clipboard as hex string
        try
        {
            var hex = Clipboard.GetText().Trim();
            Util.GetBytesFromHexString(hex.Replace(" ", ""), data);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error($"Failed to import donut from clipboard:\n{ex.Message}");
            return false;
        }
        return true;
    }

    private static bool TryLoadDonutOpenFile(Span<byte> data)
    {
        using var ofd = new OpenFileDialog();
        ofd.Title = "Import Donut";
        ofd.Filter = "Donut File (*.donut)|*.donut|All Files (*.*)|*.*";
        if (ofd.ShowDialog() != DialogResult.OK)
            return false;

        return ImportDonutFromPath(data, ofd.FileName);
    }

    private void Dragout_DragOver(object? sender, DragEventArgs e) => e.Effect = DragDropEffects.Copy;
    private void DragoutDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;

        var current = Donuts.GetDonut(lastIndex);
        var data = current.Data;
        ImportDonutFromPath(data, files[0]);
        donutEditor.LoadDonut(current);
        System.Media.SystemSounds.Asterisk.Play();
        e.Effect = DragDropEffects.Copy;

        Cursor = DefaultCursor;
    }

    private static bool ImportDonutFromPath(Span<byte> data, string path)
    {
        try
        {
            var fileData = System.IO.File.ReadAllBytes(path);
            if (fileData.Length != data.Length)
                throw new Exception($"Invalid donut size: expected {data.Length} bytes, got {fileData.Length} bytes.");
            fileData.CopyTo(data);
            return true;
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error($"Failed to import donut from file:\n{ex.Message}");
            return false;
        }
    }

    private void B_Export_Click(object sender, EventArgs e)
    {
        SetEntry(lastIndex);
        var current = Donuts.GetDonut(lastIndex);
        var data = current.Data;

        if (ModifierKeys == Keys.Control)
        {
            // Copy to clipboard as hex string
            var sb = new StringBuilder(data.Length * 3);
            foreach (var b in data)
                sb.Append($"{b:X2} ");
            Clipboard.SetText(sb.ToString().TrimEnd());
            System.Media.SystemSounds.Asterisk.Play();
            return;
        }

        using var sfd = new SaveFileDialog();
        sfd.Title = "Export Donut";
        sfd.Filter = "Donut File (*.donut)|*.donut|All Files (*.*)|*.*";
        sfd.FileName = $"{lastIndex + 1:000}_{donutEditor.GetDonutName()}.donut";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;
        System.IO.File.WriteAllBytes(sfd.FileName, data);
    }
}
