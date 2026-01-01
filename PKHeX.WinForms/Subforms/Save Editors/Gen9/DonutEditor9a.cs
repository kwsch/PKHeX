using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public sealed partial class DonutEditor9a : UserControl
{
    private Donut9a _donut;
    public event EventHandler? ValueChanged;

    private readonly ComboBox[] Berry;
    private readonly PictureBox[] BerryIcons;
    private readonly ComboBox[] Flavor;
    private readonly PictureBox[] FlavorIcons;
    private readonly PictureBox[] Stars;

    private bool Loading;

    private static readonly DateTime Epoch = Donut9a.Epoch;

    public DonutEditor9a()
    {
        InitializeComponent();

        Berry = [CB_Berry0, CB_Berry1, CB_Berry2, CB_Berry3, CB_Berry4, CB_Berry5, CB_Berry6, CB_Berry7, CB_Berry8];
        BerryIcons = [PB_Berry0, PB_Berry1, PB_Berry2, PB_Berry3, PB_Berry4, PB_Berry5, PB_Berry6, PB_Berry7, PB_Berry8];
        Flavor = [CB_Flavor0, CB_Flavor1, CB_Flavor2];
        FlavorIcons = [PB_Flavor0, PB_Flavor1, PB_Flavor2];
        Stars = [PB_Star1, PB_Star2, PB_Star3, PB_Star4, PB_Star5];
    }

    public void InitializeLists(ReadOnlySpan<string> flavors, ReadOnlySpan<string> items, ReadOnlySpan<string> donutNames)
    {
        var berryList = GetBerryList(ItemStorage9ZA.Berry, items, items[0]);
        var flavorList = GetFlavorText(flavors, items[0]);
        var donutList = GetDonutList(donutNames);

        InitializeEvents([NUD_Calories, NUD_LevelBoost, NUD_Stars]);
        InitializeEvents(Berry);
        InitializeEvents(Flavor);

        for (var i = 0; i < Berry.Length; i++)
        {
            var cb = Berry[i];
            var pb = BerryIcons[i];
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

        foreach (var cb in Flavor)
            SetDataSource(cb, flavorList);
        SetDataSource(CB_Donut, donutList);

        CB_Donut.SelectedIndexChanged += OnValueChanged;
        CB_Donut.SelectedIndexChanged += CB_Donut_SelectedIndexChanged;

        // Not really necessary to indicate value changes (name wouldn't be different), but for consistency and GUI updates...
        CAL_Date.ValueChanged += OnValueChanged;
        CAL_Date.ValueChanged += ChangeDateTime;
        TB_Milliseconds.TextChanged += OnValueChanged;
        TB_Milliseconds.TextChanged += ChangeMilliseconds;
        NUD_Stars.ValueChanged += OnValueChanged;
        NUD_Stars.ValueChanged += NUD_Stars_ValueChanged;
        // flavor already set via InitializeEvents
        CB_Flavor0.SelectedIndexChanged += CB_Flavor_SelectedIndexChanged;
        CB_Flavor1.SelectedIndexChanged += CB_Flavor_SelectedIndexChanged;
        CB_Flavor2.SelectedIndexChanged += CB_Flavor_SelectedIndexChanged;
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

        for (int i = 0; i < all.Count; i++)
        {
            var flavor = all[i];
            var text = localized[i];
            var value = flavor.Name;
            result.Add(new ComboText(text, value));
        }

        return result;
    }

    public void LoadDonut(Donut9a donut)
    {
        Loading = true;
        _donut = donut;

        LoadClamp(NUD_Stars, donut.Stars);
        LoadClamp(NUD_Calories, donut.Calories);
        LoadClamp(NUD_LevelBoost, donut.LevelBoost);

        CB_Donut.SelectedValue = (int)donut.Donut;

        LoadDonutStarCount(donut.Stars); // acknowledge existing star count

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

        LoadDonutFlavorImages(donut.GetFlavors());

        DateTime dt;
        if (!donut.HasDateTime())
            dt = Epoch;
        else
            dt = donut.DateTime1900.Timestamp;
        try
        {
            CAL_Date.Value = dt;
        }
        catch
        {
            CAL_Date.Value = Epoch;
        }

        TB_Milliseconds.Text = donut.MillisecondsSince1900.ToString();

        Loading = false;
        return;

        static void LoadClamp(NumericUpDown nud, decimal value) => nud.Value = Math.Clamp(value, nud.Minimum, nud.Maximum);
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
        if (dt is { Year: 1900, Month: <= 1, Day: <= 1 } and { Day: 1, Hour: 0, Minute: 0, Second: 0 })
        {
            donut.ClearDateTime();
        }
        else
        {
            try
            {
                donut.DateTime1900.Timestamp = dt;
            }
            catch
            {
                donut.ClearDateTime();
            }
        }
        donut.MillisecondsSince1900 = ulong.TryParse(TB_Milliseconds.Text, out var unk) ? unk : 0;
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
        if (cb.SelectedIndex <= 0)
            return 0; // No flavor

        // Grab the internal value (not the localized display value)
        var text = cb.SelectedValue?.ToString();
        if (text is null)
            return 0; // No selection? fail-safe

        var hash = DonutInfo.GetFlavorHash(text);
        return hash;
    }

    private void LoadDonutFlavorImages(params ReadOnlySpan<ulong> flavors)
    {
        for (int i = 0; i < FlavorIcons.Length; i++)
        {
            Image? img;
            if (flavors[i] != 0 && DonutInfo.TryGetFlavorName(flavors[i], out var name))
                img = DonutSpriteUtil.GetDonutFlavorImage(name);
            else
                img = null;
            FlavorIcons[i].Image = img;
        }
    }

    private void LoadDonutStarCount(byte count)
    {
        var star = DonutSpriteUtil.StarSprite;
        for (int i = 0; i < Stars.Length; i++)
            Stars[i].Image = i < count ? star : null;
    }

    private void CB_Donut_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _donut.Donut = (ushort)CB_Donut.SelectedIndex;
        PB_Donut.Image = _donut.Sprite();
    }

    private void CB_Flavor_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is not ComboBox cb)
            return;
        var index = Flavor.IndexOf(cb);
        if (index < 0)
            return;
        var text = cb.SelectedIndex > 0 ? cb.SelectedValue?.ToString() : null;
        FlavorIcons[index].Image = text is null ? null : DonutSpriteUtil.GetDonutFlavorImage(text);
    }

    public void Reset()
    {
        _donut.Clear();
        LoadDonut(_donut);
    }

    private void ChangeMilliseconds(object? sender, EventArgs e)
    {
        if (Loading)
            return;

        if (!ulong.TryParse(TB_Milliseconds.Text, out var ms))
            return;

        try
        {
            var ticks = Epoch.AddMilliseconds(ms);

            // If date is same, don't update the ticks.
            var date = CAL_Date.Value;
            if (IsDateEquivalent(date, ticks))
                return;

            Loading = true;
            CAL_Date.Value = ticks;
            Loading = false;
        }
        catch
        {
            // Why are you putting ugly values??
            // Reset.
            Loading = true;
            CAL_Date.Value = DateTime.Now;
            TB_Milliseconds.Text = ((ulong)(CAL_Date.Value - Epoch).TotalMilliseconds).ToString();
            Loading = false;
        }
    }

    private void ChangeDateTime(object? sender, EventArgs e)
    {
        if (Loading)
            return;

        if (!ulong.TryParse(TB_Milliseconds.Text, out var ms))
            return;

        try
        {
            var ticks = Epoch.AddMilliseconds(ms);

            // If date is same, don't update the ticks.
            var date = CAL_Date.Value;
            if (IsDateEquivalent(date, ticks))
                return;

            var delta = ((ulong)(date - Epoch).TotalMilliseconds);
            // retain existing ticks _xxx component, since datetime picker does not configure millis
            var exist = ms % 1000;
            delta -= delta % 1000;
            delta += exist;

            Loading = true;
            TB_Milliseconds.Text = delta.ToString();
            Loading = false;
        }
        catch
        {
            // Why are you putting ugly values??
            // Reset.
            Loading = true;
            CAL_Date.Value = DateTime.Now;
            TB_Milliseconds.Text = ((ulong)(CAL_Date.Value - Epoch).TotalMilliseconds).ToString();
            Loading = false;
        }
    }

    private static bool IsDateEquivalent(DateTime a, DateTime b) =>
        a.Year == b.Year && a.Month == b.Month && a.Day == b.Day &&
        a.Hour == b.Hour && a.Minute == b.Minute && a.Second == b.Second;

    // bubble up to the parent control, if subscribed.
    private void OnValueChanged(object? sender, EventArgs e) => ValueChanged?.Invoke(this, EventArgs.Empty);

    // ReSharper disable NotAccessedPositionalProperty.Local
    private sealed record ComboText(string Text, string Value);
    // ReSharper enable NotAccessedPositionalProperty.Local
    public string GetDonutName() => CB_Donut.Text;

    private void NUD_Stars_ValueChanged(object? sender, EventArgs e) => LoadDonutStarCount((byte)NUD_Stars.Value);
}
