using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public sealed partial class DonutEditor9a : UserControl
{
    private Donut9a _donut;
    public event EventHandler? ValueChanged;

    public DonutEditor9a() => InitializeComponent();

    public void InitializeLists(ReadOnlySpan<string> flavors, ReadOnlySpan<string> items, ReadOnlySpan<string> donutNames)
    {
        var berryList = GetBerryList(ItemStorage9ZA.Berry, items, items[0]);
        var flavorList = GetFlavorText(flavors, items[0]);
        var donutList = GetDonutList(donutNames);

        ComboBox[] berry = [CB_Berry0, CB_Berry1, CB_Berry2, CB_Berry3, CB_Berry4, CB_Berry5, CB_Berry6, CB_Berry7, CB_Berry8];
        PictureBox[] icons = [PB_Berry0, PB_Berry1, PB_Berry2, PB_Berry3, PB_Berry4, PB_Berry5, PB_Berry6, PB_Berry7, PB_Berry8];
        ComboBox[] flavor = [CB_Flavor0, CB_Flavor1, CB_Flavor2];

        InitializeEvents([NUD_Calories, NUD_LevelBoost, NUD_Stars]);
        InitializeEvents(berry);
        InitializeEvents(flavor);

        for (var i = 0; i < berry.Length; i++)
        {
            var cb = berry[i];
            var pb = icons[i];
            SetDataSource(cb, berryList);
            cb.SelectedValueChanged += (_, _) =>
            {
                var itemID = WinFormsUtil.GetIndex(cb);
                if (itemID <= 0)
                {
                    pb.Image = null;
                    return;
                }
                pb.Image = SpriteUtil.GetItemSpriteA(itemID);
            };
        }

        foreach (var cb in flavor)
            SetDataSource(cb, flavorList);
        SetDataSource(CB_Donut, donutList);

        CB_Donut.SelectedIndexChanged += OnValueChanged;

        // Not really necessary to indicate value changes (name wouldn't be different), but for consistency...
        CAL_Date.ValueChanged += OnValueChanged;
        TB_Unknown.TextChanged += OnValueChanged;
    }

    private static void SetDataSource<T>(ComboBox cb, List<T> list)
    {
        cb.InitializeBinding();
        cb.DataSource = new BindingSource(list, string.Empty);
    }

    private void InitializeEvents(ReadOnlySpan<NumericUpDown> controls)
    {
        foreach (var control in controls)
            control.ValueChanged += OnValueChanged;
    }

    private void InitializeEvents(ReadOnlySpan<ComboBox> controls)
    {
        foreach (var control in controls)
            control.SelectedIndexChanged += OnValueChanged;
    }

    private static List<ComboItem> GetDonutList(ReadOnlySpan<string> names)
    {
        List<ComboItem> result = [];
        for (int i = 0; i < names.Length; i++)
        {
            var text = names[i];
            result.Add(new ComboItem(text, i));
        }
        return result;
    }

    private static List<ComboItem> GetBerryList(ReadOnlySpan<ushort> berries, ReadOnlySpan<string> localized, string none)
    {
        List<ComboItem> result = [new(none, 0)];
        foreach (var berryItemID in berries)
        {
            var text = localized[berryItemID];
            result.Add(new ComboItem(text, berryItemID));
        }
        return result;
    }

    private static List<ComboText> GetFlavorText(ReadOnlySpan<string> localized, string none)
    {
        var all = DonutInfo.Flavors;
        List<ComboText> result = [new(none, "")];

        for (int i = 0; i < all.Length; i++)
        {
            var flavor = all[i];
            var text = localized[i];
            var value = flavor.Name;
            result.Add(new ComboText(text, value));
        }

        return result;
    }

    private static readonly DateTime Epoch = new(1900, 1, 1);

    public void LoadDonut(Donut9a donut)
    {
        _donut = donut;

        NUD_Stars.Value = donut.Stars;
        NUD_Calories.Value = donut.Calories;
        NUD_LevelBoost.Value = donut.LevelBoost;

        CB_Donut.SelectedValue = (int)donut.Donut;

        CB_Berry0.SelectedValue = (int)donut.BerryName;
        CB_Berry1.SelectedValue = (int)donut.Berry1;
        CB_Berry2.SelectedValue = (int)donut.Berry2;
        CB_Berry3.SelectedValue = (int)donut.Berry3;
        CB_Berry4.SelectedValue = (int)donut.Berry4;
        CB_Berry5.SelectedValue = (int)donut.Berry5;
        CB_Berry6.SelectedValue = (int)donut.Berry6;
        CB_Berry7.SelectedValue = (int)donut.Berry7;
        CB_Berry8.SelectedValue = (int)donut.Berry8;

        LoadDonutFlavorHash(CB_Flavor0, donut.Flavor0);
        LoadDonutFlavorHash(CB_Flavor1, donut.Flavor1);
        LoadDonutFlavorHash(CB_Flavor2, donut.Flavor2);

        DateTime dt;
        if (!donut.HasDateTime())
            dt = Epoch;
        else
            dt = donut.DateTime1900.Timestamp;
        CAL_Date.Value = dt;

        TB_Unknown.Text = donut.Unknown.ToString();
    }

    public void SaveDonut()
    {
        var donut = _donut;

        donut.Stars = (byte)NUD_Stars.Value;
        donut.Calories = (ushort)NUD_Calories.Value;
        donut.LevelBoost = (byte)NUD_LevelBoost.Value;

        donut.Donut = (ushort)WinFormsUtil.GetIndex(CB_Donut);

        donut.BerryName = (ushort)WinFormsUtil.GetIndex(CB_Berry0);
        donut.Berry1 = (ushort)WinFormsUtil.GetIndex(CB_Berry1);
        donut.Berry2 = (ushort)WinFormsUtil.GetIndex(CB_Berry2);
        donut.Berry3 = (ushort)WinFormsUtil.GetIndex(CB_Berry3);
        donut.Berry4 = (ushort)WinFormsUtil.GetIndex(CB_Berry4);
        donut.Berry5 = (ushort)WinFormsUtil.GetIndex(CB_Berry5);
        donut.Berry6 = (ushort)WinFormsUtil.GetIndex(CB_Berry6);
        donut.Berry7 = (ushort)WinFormsUtil.GetIndex(CB_Berry7);
        donut.Berry8 = (ushort)WinFormsUtil.GetIndex(CB_Berry8);

        donut.Flavor0 = GetDonutFlavorHash(CB_Flavor0);
        donut.Flavor1 = GetDonutFlavorHash(CB_Flavor1);
        donut.Flavor2 = GetDonutFlavorHash(CB_Flavor2);

        var date = CAL_Date.Value;
        var dt = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);

        // if date is sufficiently equal to the Epoch (zero), set to zero. Can't set a date of 1900/00/00 via the controls...
        if (dt is { Year: 1900, Month: 1, Day: 1 } and { Day: 1, Hour: 0, Minute: 0, Second: 0 })
            donut.ClearDateTime();
        else
            donut.DateTime1900.Timestamp = dt;
        donut.Unknown = ulong.TryParse(TB_Unknown.Text, out var unk) ? unk : 0;
    }

    private static void LoadDonutFlavorHash(ComboBox cb, ulong flavorHash)
    {
        // Find the matching flavor by hash
        if (flavorHash == 0 || !DonutInfo.TryGetFlavorName(flavorHash, out var name))
        {
            cb.SelectedIndex = 0; // No flavor
            return;
        }
        cb.SelectedValue = name;
    }

    private static ulong GetDonutFlavorHash(ComboBox cb)
    {
        if (cb.SelectedIndex == 0)
            return 0; // No flavor

        // Grab the internal value (not the localized display value)
        var text = cb.SelectedValue?.ToString();
        if (text is null)
            return 0; // No selection? fail-safe

        var hash = DonutInfo.GetFlavorHash(text);
        return hash;
    }

    public void Reset()
    {
        _donut.Clear();
        LoadDonut(_donut);
    }

    // bubble up to the parent control, if subscribed.
    private void OnValueChanged(object? sender, EventArgs e) => ValueChanged?.Invoke(this, EventArgs.Empty);

    // ReSharper disable NotAccessedPositionalProperty.Local
    private sealed record ComboText(string Text, string Value);
    // ReSharper enable NotAccessedPositionalProperty.Local
}
