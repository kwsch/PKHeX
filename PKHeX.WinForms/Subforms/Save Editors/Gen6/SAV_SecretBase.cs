using System;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_SecretBase : Form
{
    private readonly SaveFile Origin;
    private readonly SAV6AO SAV;

    private SecretBase6? CurrentBase;
    private int CurrentPKMIndex;
    private SecretBase6PKM? CurrentPKM;

    private int CurrentPlacementIndex;
    private SecretBase6GoodPlacement? CurrentPlacement;

    private bool loading = true;

    public SAV_SecretBase(SAV6AO sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV6AO)(Origin = sav).Clone();

        NUD_FObject.Maximum = SecretBase6.COUNT_GOODS - 1; // zero indexed!
        NUD_FPKM.Maximum = SecretBase6Other.COUNT_TEAM - 1; // zero indexed!
        PG_Base.Font = FontUtil.GetPKXFont();

        SetupComboBoxes();
        ReloadSecretBaseList();
        LB_Bases.SelectedIndex = 0;

        // Extra data
        NUD_CapturedRecord.Value = SAV.Records.GetRecord(080);
    }

    private void SetupComboBoxes()
    {
        CB_Ball.InitializeBinding();
        CB_HeldItem.InitializeBinding();
        CB_Species.InitializeBinding();
        CB_Nature.InitializeBinding();
        CB_Ability.InitializeBinding();
        CB_Form.InitializeBinding();

        var filtered = GameInfo.FilteredSources;
        CB_Ball.DataSource = new BindingSource(filtered.Balls, null);
        CB_HeldItem.DataSource = new BindingSource(filtered.Items, null);
        CB_Species.DataSource = new BindingSource(filtered.Species, null);
        CB_Nature.DataSource = new BindingSource(filtered.Natures, null);

        CB_Move1.InitializeBinding();
        CB_Move2.InitializeBinding();
        CB_Move3.InitializeBinding();
        CB_Move4.InitializeBinding();

        var moves = filtered.Moves;
        CB_Move1.DataSource = new BindingSource(moves, null);
        CB_Move2.DataSource = new BindingSource(moves, null);
        CB_Move3.DataSource = new BindingSource(moves, null);
        CB_Move4.DataSource = new BindingSource(moves, null);
    }

    private void ReloadSecretBaseList()
    {
        loading = true;
        int index = LB_Bases.SelectedIndex;
        LB_Bases.Items.Clear();

        var block = SAV.SecretBase;
        var self = block.GetSecretBaseSelf();
        LB_Bases.Items.Add($"* {self.TrainerName}");
        for (int i = 0; i < SecretBase6Block.OtherSecretBaseCount; i++)
        {
            var other = block.GetSecretBaseOther(i);
            string name = other.TrainerName;
            if (string.IsNullOrWhiteSpace(name))
                name = "Empty";
            LB_Bases.Items.Add($"{i + 1:00} {name}");
        }

        if (index >= 0)
            LB_Bases.SelectedIndex = index;

        loading = false;
    }

    private void ResetLoadNew()
    {
        // Wipe all cached references so nothing is inadvertently saved
        CurrentPKM = null;
        CurrentPlacement = null;
        CurrentBase = null;

        CurrentPKMIndex = -1;
        CurrentPlacementIndex = -1;
    }

    private SecretBase6 GetSecretBaseReference(int index)
    {
        if (index == 0)
            return SAV.SecretBase.GetSecretBaseSelf();
        return SAV.SecretBase.GetSecretBaseOther(index - 1);
    }

    private void LoadCurrent(SecretBase6 bdata)
    {
        loading = true;
        CurrentBase = bdata;

        PG_Base.SelectedObject = bdata;

        var pIndex = (int)NUD_FObject.Value;
        LoadPlacement(bdata.GetPlacement(pIndex), pIndex);
        if (bdata is SecretBase6Other o)
            LoadOtherData(o);
        else
            SetOtherDataVisible(false);

        loading = false;
    }

    private void SaveCurrent(SecretBase6 bdata)
    {
        SavePlacement((int)NUD_FObject.Value);
        if (bdata is SecretBase6Other o)
            SaveOtherData(o);
    }

    private void LoadPlacement(SecretBase6GoodPlacement p, int index)
    {
        SavePlacement(index);
        CurrentPlacement = p;
        CurrentPlacementIndex = index;

        NUD_FObjType.Value = Clamp(NUD_FObjType, p.Good);
        NUD_FX.Value = Clamp(NUD_FX, p.X);
        NUD_FY.Value = Clamp(NUD_FY, p.Y);
        NUD_FRot.Value = Clamp(NUD_FRot, p.Rotation);
        static decimal Clamp(NumericUpDown nud, decimal value) => Math.Clamp(value, nud.Minimum, nud.Maximum);
    }

    private void SavePlacement(int index)
    {
        var p = CurrentPlacement;
        if (p is null || index < 0)
            return;

        p.Good = (ushort)NUD_FObjType.Value;
        p.X = (ushort)NUD_FX.Value;
        p.Y = (ushort)NUD_FY.Value;
        p.Rotation = (byte)NUD_FRot.Value;
    }

    private void SaveOtherData(SecretBase6Other full)
    {
        var pk = CurrentPKM;
        var index = CurrentPKMIndex;
        if (pk is not null && index >= 0)
        {
            SavePKM(pk);
            full.SetParticipant(index, pk);
        }
    }

    private void LoadOtherData(SecretBase6Other full)
    {
        var pIndex = CurrentPKMIndex = (int)NUD_FPKM.Value;
        var pk = full.GetParticipant(pIndex);
        LoadPKM(pk);
        SetOtherDataVisible(true);
    }

    private void SetOtherDataVisible(bool visible)
    {
        PAN_PKM.Visible = visible;
    }

    private void SavePKM(SecretBase6PKM pk)
    {
        pk.EncryptionConstant = Util.GetHexValue(TB_EC.Text);
        pk.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        pk.HeldItem = WinFormsUtil.GetIndex(CB_HeldItem);
        pk.Ability = WinFormsUtil.GetIndex(CB_Ability);
        pk.AbilityNumber = CB_Ability.SelectedIndex << 1;
        pk.Nature = (Nature)WinFormsUtil.GetIndex(CB_Nature);
        pk.Gender = EntityGender.GetFromString(Label_Gender.Text);
        pk.Form = (byte)CB_Form.SelectedIndex;
        pk.EV_HP  = Math.Clamp(Convert.ToInt32(TB_HPEV.Text) , 0, EffortValues.Max252);
        pk.EV_ATK = Math.Clamp(Convert.ToInt32(TB_ATKEV.Text), 0, EffortValues.Max252);
        pk.EV_DEF = Math.Clamp(Convert.ToInt32(TB_DEFEV.Text), 0, EffortValues.Max252);
        pk.EV_SPA = Math.Clamp(Convert.ToInt32(TB_SPAEV.Text), 0, EffortValues.Max252);
        pk.EV_SPD = Math.Clamp(Convert.ToInt32(TB_SPDEV.Text), 0, EffortValues.Max252);
        pk.EV_SPE = Math.Clamp(Convert.ToInt32(TB_SPEEV.Text), 0, EffortValues.Max252);
        pk.Move1 = (ushort)WinFormsUtil.GetIndex(CB_Move1);
        pk.Move2 = (ushort)WinFormsUtil.GetIndex(CB_Move2);
        pk.Move3 = (ushort)WinFormsUtil.GetIndex(CB_Move3);
        pk.Move4 = (ushort)WinFormsUtil.GetIndex(CB_Move4);
        pk.Move1_PPUps = CB_PPu1.SelectedIndex;
        pk.Move2_PPUps = CB_PPu2.SelectedIndex;
        pk.Move3_PPUps = CB_PPu3.SelectedIndex;
        pk.Move4_PPUps = CB_PPu4.SelectedIndex;
        pk.IV_HP  = Convert.ToByte(TB_HPIV.Text)  & 0x1F;
        pk.IV_ATK = Convert.ToByte(TB_ATKIV.Text) & 0x1F;
        pk.IV_DEF = Convert.ToByte(TB_DEFIV.Text) & 0x1F;
        pk.IV_SPA = Convert.ToByte(TB_SPAIV.Text) & 0x1F;
        pk.IV_SPD = Convert.ToByte(TB_SPDIV.Text) & 0x1F;
        pk.IV_SPE = Convert.ToByte(TB_SPEIV.Text) & 0x1F;
        pk.IsShiny = CHK_Shiny.Checked;
        pk.CurrentFriendship = Convert.ToByte(TB_Friendship.Text);
        pk.Ball = (byte)WinFormsUtil.GetIndex(CB_Ball);
        pk.CurrentLevel = Convert.ToByte(TB_Level.Text);
    }

    private void LoadPKM(SecretBase6PKM pk)
    {
        CurrentPKM = pk;

        // Put data into fields.
        TB_EC.Text = pk.EncryptionConstant.ToString("X8");
        SetGenderLabel(pk.Gender);
        CB_Species.SelectedValue = (int)pk.Species;
        CB_HeldItem.SelectedValue = pk.HeldItem;
        CB_Form.SelectedIndex = pk.Form;

        CB_Nature.SelectedValue = (int)pk.Nature;
        CB_Ball.SelectedValue = (int)pk.Ball;

        TB_HPIV.Text = pk.IV_HP.ToString();
        TB_ATKIV.Text = pk.IV_ATK.ToString();
        TB_DEFIV.Text = pk.IV_DEF.ToString();
        TB_SPAIV.Text = pk.IV_SPA.ToString();
        TB_SPDIV.Text = pk.IV_SPD.ToString();
        TB_SPEIV.Text = pk.IV_SPE.ToString();

        TB_HPEV.Text = pk.EV_HP.ToString();
        TB_ATKEV.Text = pk.EV_ATK.ToString();
        TB_DEFEV.Text = pk.EV_DEF.ToString();
        TB_SPAEV.Text = pk.EV_SPA.ToString();
        TB_SPDEV.Text = pk.EV_SPD.ToString();
        TB_SPEEV.Text = pk.EV_SPE.ToString();

        TB_Friendship.Text = pk.CurrentFriendship.ToString();
        TB_Level.Text = pk.CurrentLevel.ToString();

        CB_Move1.SelectedValue = (int)pk.Move1;
        CB_Move2.SelectedValue = (int)pk.Move2;
        CB_Move3.SelectedValue = (int)pk.Move3;
        CB_Move4.SelectedValue = (int)pk.Move4;
        CB_PPu1.SelectedIndex = pk.Move1_PPUps;
        CB_PPu2.SelectedIndex = pk.Move2_PPUps;
        CB_PPu3.SelectedIndex = pk.Move3_PPUps;
        CB_PPu4.SelectedIndex = pk.Move4_PPUps;

        CHK_Shiny.Checked = pk.IsShiny;

        // Set Ability
        SetAbilityList(pk.Species, pk.Form, pk.AbilityNumber >> 1);
    }

    #region Editing Index Changing
    private void ChangeIndexBase(object sender, EventArgs e)
    {
        int index = LB_Bases.SelectedIndex;
        if (index < 0 || loading)
            return;

        var bdata = CurrentBase;
        if (bdata != null)
            SaveCurrent(bdata);

        ResetLoadNew();
        bdata = CurrentBase = GetSecretBaseReference(index);
        LoadCurrent(bdata);
    }

    private void ChangeIndexPlacement(object sender, EventArgs e)
    {
        var bdata = CurrentBase;
        if (bdata is null)
            return;

        SavePlacement(CurrentPlacementIndex);
        var pIndex = (int)NUD_FObject.Value;
        LoadPlacement(bdata.GetPlacement(pIndex), pIndex);
    }

    private void ChangeIndexPKM(object sender, EventArgs e)
    {
        var bdata = CurrentBase;
        if (bdata is not SecretBase6Other o)
            return;

        var pk = CurrentPKM;
        if (pk is null || CurrentPKMIndex < 0)
            return;

        SavePKM(pk);
        o.SetParticipant(CurrentPKMIndex, pk);

        var index = CurrentPKMIndex = (int)NUD_FPKM.Value;
        pk = o.GetParticipant(index);
        LoadPKM(pk);
    }
    #endregion

    #region PKM
    private void SetAbilityList()
    {
        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        var form = (byte)CB_Form.SelectedIndex;
        var index = CB_Ability.SelectedIndex;
        SetAbilityList(species, form, index);
    }

    private void SetAbilityList(ushort species, byte form, int abilityIndex)
    {
        var abilities = PersonalTable.AO.GetFormEntry(species, form);
        var list = GameInfo.FilteredSources.GetAbilityList(abilities);
        CB_Ability.DataSource = new BindingSource(list, null);
        CB_Ability.SelectedIndex = abilityIndex < 3 ? abilityIndex : 0;
    }

    private void SetForms()
    {
        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        bool hasForms = FormInfo.HasFormSelection(PersonalTable.AO[species], species, 6);
        CB_Form.Enabled = CB_Form.Visible = hasForms;

        var list = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Context);
        CB_Form.DataSource = new BindingSource(list, null);
    }

    private void UpdateSpecies(object sender, EventArgs e)
    {
        // Get Forms for Given Species
        SetForms();

        var species = WinFormsUtil.GetIndex(CB_Species);

        // Set a sane gender
        var pi = SAV.Personal[species];
        var gender = pi.IsDualGender
            ? EntityGender.GetFromString(Label_Gender.Text)
            : pi.FixedGender();
        SetGenderLabel(gender);

        SetAbilityList();
    }

    private void UpdateForm(object sender, EventArgs e)
    {
        SetAbilityList();

        // If form has a single gender, account for it.
        if (EntityGender.GetFromString(CB_Form.Text) < 2)
            Label_Gender.Text = Main.GenderSymbols[CB_Form.SelectedIndex];
    }

    private void Label_Gender_Click(object sender, EventArgs e)
    {
        var species = WinFormsUtil.GetIndex(CB_Species);
        var pi = SAV.Personal[species];
        byte gender;
        if (pi.IsDualGender) // dual gender
            gender = (byte)((EntityGender.GetFromString(Label_Gender.Text) ^ 1) & 1);
        else
            gender = pi.FixedGender();
        Label_Gender.Text = Main.GenderSymbols[gender];
    }

    private void SetGenderLabel(byte gender)
    {
        var symbols = Main.GenderSymbols;
        if ((uint)gender >= symbols.Count)
            gender = 0;
        Label_Gender.Text = symbols[gender];
    }
    #endregion

    #region I/O
    private void B_Import_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var path = ofd.FileName;
        if (new FileInfo(path).Length is not (SecretBase6.SIZE or SecretBase6Other.SIZE))
            return;

        var data = File.ReadAllBytes(path);
        var obj = SecretBase6.Read(data);
        // shouldn't happen, we already size-check above.
        ArgumentNullException.ThrowIfNull(obj);

        var sb = CurrentBase;
        ArgumentNullException.ThrowIfNull(sb);

        ResetLoadNew();
        sb.Load(obj);
        ReloadSecretBaseList();
        LoadCurrent(sb);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Export_Click(object sender, EventArgs e)
    {
        var sb = CurrentBase;
        ArgumentNullException.ThrowIfNull(sb);

        SaveCurrent(sb);
        var tr = sb.TrainerName;
        if (string.IsNullOrWhiteSpace(tr))
            tr = "Trainer";
        using var sfd = new SaveFileDialog();
        sfd.Filter = "Secret Base Data|*.sb6";
        sfd.FileName = $"{sb.BaseLocation:D2} - {Util.CleanFileName(tr)}.sb6";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        var path = sfd.FileName;
        var data = sb.Write();
        File.WriteAllBytes(path, data);
    }
    #endregion

    #region Meta Buttons
    private void B_FDelete_Click(object sender, EventArgs e)
    {
        if (LB_Bases.SelectedIndex < 1)
        {
            WinFormsUtil.Alert(MsgSecretBaseDeleteSelf);
            return;
        }

        int index = LB_Bases.SelectedIndex - 1;
        var bdata = CurrentBase;
        ArgumentNullException.ThrowIfNull(bdata);

        string BaseTrainer = bdata.TrainerName;
        if (string.IsNullOrEmpty(BaseTrainer))
            BaseTrainer = "Empty";

        var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Format(MsgSecretBaseDeleteConfirm, BaseTrainer, index));
        if (dr != DialogResult.Yes)
            return;

        SAV.Blocks.SecretBase.DeleteOther(index);
        ReloadSecretBaseList();
        ResetLoadNew();
        bdata = CurrentBase = GetSecretBaseReference(index + 1);
        LoadCurrent(bdata);
        LB_Bases.SelectedIndex = index + 1;
    }

    private void B_GiveDecor_Click(object sender, EventArgs e) => SAV.Blocks.SecretBase.GiveAllGoods();
    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        uint flags = (uint)NUD_CapturedRecord.Value;
        SAV.Records.SetRecord(080, (int)flags);

        var bdata = CurrentBase;
        if (bdata != null)
            SaveCurrent(bdata);

        Origin.CopyChangesFrom(SAV);
        Close();
    }
    #endregion
}
