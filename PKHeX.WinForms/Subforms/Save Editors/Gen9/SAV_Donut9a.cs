using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;

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

        // Not implemented.
        mnuRandomizeMax.Visible = false;
        mnuShinyAssortment.Visible = false;
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
        // Only refresh the name in the list if it has changed.
        var index = lastIndex;
        var currentName = GetDonutName(index);
        var existing = LB_Donut.Items[index];
        if (existing.ToString() != currentName)
            LB_Donut.Items[index] = currentName;
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

        var donut = Donuts.GetDonut(index);
        donutEditor.LoadDonut(donut);
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
        for (int i = 0; i < DonutPocket9a.MaxCount; i++)
        {
            // todo
        }
    }

    private void CloneCurrent(object sender, EventArgs e)
    {
        SetEntry(lastIndex);
        var current = Donuts.GetDonut(lastIndex);
        for (int i = 0; i < DonutPocket9a.MaxCount; i++)
        {
            if (i == lastIndex)
                continue;
            var target = Donuts.GetDonut(i);
            current.CopyTo(target);
            LB_Donut.Items[i] = GetDonutName(target, i); // todo: test this to see if it is any bit slow
        }
    }

    private void ShinyAssortment(object sender, EventArgs e)
    {
        for (int i = 0; i < DonutPocket9a.MaxCount; i++)
        {
            var donut = Donuts.GetDonut(i);
            // todo: generate a shiny donut
            LB_Donut.Items[i] = GetDonutName(donut, i); // todo: test this to see if it is any bit slow
        }
    }

    private void B_Reset_Click(object sender, EventArgs e) => donutEditor.Reset();
}
