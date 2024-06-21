using System;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

public partial class StatusConditionView : UserControl
{
    private PKM? pk;
    private bool Loading;
    private readonly ToolTip Hover = new()
    {
        AutoPopDelay = 5000,
        InitialDelay = 200,
        ReshowDelay = 500,
        ShowAlways = true,
    };

    public StatusConditionView()
    {
        InitializeComponent();
        PB_Status.MouseHover += (_, _) => PB_Status.Cursor = Cursors.Hand;
    }

    public void LoadPKM(PKM entity)
    {
        pk = entity;
        LoadStoredValues();
    }

    public void LoadStoredValues()
    {
        if (pk is null || Loading)
            return;
        Loading = true;
        if (!pk.PartyStatsPresent)
            ClearStatus();
        else if (pk.Stat_HPCurrent == 0)
            SetFaint();
        else
            SetStatus((StatusCondition)(pk.Status_Condition & 0xFF));
        Loading = false;
    }

    private void SetFaint()
    {
        PB_Status.Image = Drawing.PokeSprite.Properties.Resources.sickfaint;
        Hover.RemoveAll();
    }

    private void ClearStatus()
    {
        PB_Status.Image = null;
        Hover.RemoveAll();
    }

    private void SetStatus(StatusCondition status)
    {
        PB_Status.Image = status.GetStatusSprite();

        var text = WinFormsTranslator.TranslateEnum(status, Main.CurrentLanguage);
        Hover.SetToolTip(PB_Status, $"Status Condition: {text}");
    }

    private void PB_Status_Click(object sender, EventArgs e)
    {
        ArgumentNullException.ThrowIfNull(pk);
        using var form = new StatusBrowser();
        form.LoadList(pk);
        form.ShowDialog();
        if (!form.WasChosen)
            return;
        var current = pk.Status_Condition;
        current &= ~0xFF;
        current |= (int)form.Choice;
        pk.Status_Condition = current;
        LoadStoredValues();
    }
}
