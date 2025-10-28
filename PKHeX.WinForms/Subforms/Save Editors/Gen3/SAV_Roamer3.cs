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
        CB_Species.DataSource = new BindingSource(GameInfo.FilteredSources.Species, string.Empty);

        LoadData();
    }

    private void LoadData()
    {
        TB_PID.Text = Reader.PID.ToString("X8");
        CHK_Shiny.Checked = Roamer3.IsShiny(Reader.PID, SAV);

        CB_Species.SelectedValue = (int)Reader.Species;

        TB_HPIV.Text = Reader.IV_HP.ToString();
        TB_ATKIV.Text = Reader.IV_ATK.ToString();
        TB_DEFIV.Text = Reader.IV_DEF.ToString();
        TB_SPEIV.Text = Reader.IV_SPE.ToString();
        TB_SPAIV.Text = Reader.IV_SPA.ToString();
        TB_SPDIV.Text = Reader.IV_SPD.ToString();

        CHK_Active.Checked = Reader.Active;
        NUD_Level.Value = Math.Min(Reader.CurrentLevel, NUD_Level.Maximum);
        NUD_HP.Value = Math.Min(Reader.HP_Current, NUD_HP.Maximum);
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
        Reader.HP_Current = (ushort)NUD_HP.Value;
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
