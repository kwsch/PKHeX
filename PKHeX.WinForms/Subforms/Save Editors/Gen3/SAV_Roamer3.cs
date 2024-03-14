using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Roamer3 : Form
{
    private readonly Roamer3 Reader;
    private readonly SAV3 SAV;

    public SAV_Roamer3(SAV3 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        Reader = new Roamer3(sav);
        SAV = sav;

        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(GameInfo.FilteredSources.Species, null);

        LoadData();
    }

    private void LoadData()
    {
        TB_PID.Text = Reader.PID.ToString("X8");
        CHK_Shiny.Checked = Roamer3.IsShiny(Reader.PID, SAV);

        CB_Species.SelectedValue = (int)Reader.Species;
        var IVs = Reader.IVs;

        var iv = new[] { TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPEIV, TB_SPAIV, TB_SPDIV };
        for (int i = 0; i < iv.Length; i++)
            iv[i].Text = IVs[i].ToString();

        CHK_Active.Checked = Reader.Active;
        NUD_Level.Value = Math.Min(NUD_Level.Maximum, Reader.CurrentLevel);
    }

    private void SaveData()
    {
        Reader.PID = Util.GetHexValue(TB_PID.Text);
        Reader.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        Reader.SetIVs(
        [
            Util.ToInt32(TB_HPIV.Text),
            Util.ToInt32(TB_ATKIV.Text),
            Util.ToInt32(TB_DEFIV.Text),
            Util.ToInt32(TB_SPEIV.Text),
            Util.ToInt32(TB_SPAIV.Text),
            Util.ToInt32(TB_SPDIV.Text),
        ]);
        Reader.Active = CHK_Active.Checked;
        Reader.CurrentLevel = (byte)NUD_Level.Value;
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveData();
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void TB_PID_TextChanged(object sender, EventArgs e)
    {
        var pid = Util.GetHexValue(TB_PID.Text);
        CHK_Shiny.Checked = Roamer3.IsShiny(pid, SAV);
    }
}
