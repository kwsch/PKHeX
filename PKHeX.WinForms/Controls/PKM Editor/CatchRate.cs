using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class CatchRate : UserControl
{
    private PK1? Entity;
    public CatchRate() => InitializeComponent();

    public void LoadPK1(PK1 pk) => NUD_CatchRate.Value = (Entity = pk).CatchRate;

    private void ChangeValue(object sender, EventArgs e)
    {
        if (Entity is null)
            return;
        Entity.CatchRate = (byte)NUD_CatchRate.Value;
    }

    private void Clear(object sender, EventArgs e) => NUD_CatchRate.Value = 0;

    private void Reset(object sender, EventArgs e)
    {
        if (Entity is null)
            return;
        var sav = WinFormsUtil.FindFirstControlOfType<IMainEditor>(this)?.RequestSaveFile;
        if (sav == null)
            return;
        NUD_CatchRate.Value = CatchRateApplicator.GetSuggestedCatchRate(Entity, sav);
    }
}
