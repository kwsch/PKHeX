using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class SAV_SecretBase : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6AO SAV;

        private SecretBase6? CurrentBase;
        private int CurrentPKMIndex;
        private SecretBase6PKM? CurrentPKM;

        private int CurrentPlacementIndex ;
        private SecretBase6GoodPlacement? CurrentPlacement;

        private bool loading = true;

        public SAV_SecretBase(SaveFile sav)
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

            CB_Ball.DataSource = new BindingSource(GameInfo.BallDataSource.Where(b => b.Value <= SAV.MaxBallID).ToList(), null);
            CB_HeldItem.DataSource = new BindingSource(GameInfo.ItemDataSource.Where(i => i.Value < SAV.MaxItemID).ToList(), null);
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList(), null);
            CB_Nature.DataSource = new BindingSource(GameInfo.NatureDataSource, null);

            CB_Move1.InitializeBinding();
            CB_Move2.InitializeBinding();
            CB_Move3.InitializeBinding();
            CB_Move4.InitializeBinding();

            var moves = GameInfo.MoveDataSource;
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
                LB_Bases.Items.Add($"{i+1:00} {name}");
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
            LoadPlacement(bdata, bdata.GetPlacement(pIndex), pIndex);
            if (bdata is SecretBase6Other o)
                LoadOtherData(o);
            else
                SetOtherDataVisible(false);

            loading = false;
        }

        private void SaveCurrent(SecretBase6 bdata)
        {
            SavePlacement(bdata, (int)NUD_FObject.Value);
            if (bdata is SecretBase6Other o)
                SaveOtherData(o);
        }

        private void LoadPlacement(SecretBase6 bdata, SecretBase6GoodPlacement p, int index)
        {
            SavePlacement(bdata, index);
            CurrentPlacement = p;
            CurrentPlacementIndex = index;

            static decimal Clamp(NumericUpDown nud, decimal value) => Math.Min(nud.Maximum, Math.Max(nud.Minimum, value));
            NUD_FObjType.Value = Clamp(NUD_FObjType, p.Good);
            NUD_FX.Value = Clamp(NUD_FX, p.X);
            NUD_FY.Value = Clamp(NUD_FY, p.Y);
            NUD_FRot.Value = Clamp(NUD_FRot, p.Rotation);
        }

        private void SavePlacement(SecretBase6 bdata, int index)
        {
            var p = CurrentPlacement;
            if (p is null || index < 0)
                return;

            p.Good = (ushort) NUD_FObjType.Value;
            p.X = (ushort) NUD_FX.Value;
            p.Y = (ushort) NUD_FY.Value;
            p.Rotation = (byte) NUD_FRot.Value;

            bdata.SetPlacement(index, p);
        }

        private void SaveOtherData(SecretBase6Other full)
        {
            var pkm = CurrentPKM;
            var index = CurrentPKMIndex;
            if (pkm is not null && index >= 0)
            {
                SavePKM(pkm);
                full.SetParticipant(index, pkm);
            }
        }

        private void LoadOtherData(SecretBase6Other full)
        {
            var pIndex = CurrentPKMIndex = (int)NUD_FPKM.Value;
            var pkm = full.GetParticipant(pIndex);
            LoadPKM(pkm);
            SetOtherDataVisible(true);
        }

        private void SetOtherDataVisible(bool visible)
        {
            PAN_PKM.Visible = visible;
        }

        private void SavePKM(SecretBase6PKM pkm)
        {
            pkm.EncryptionConstant = Util.GetHexValue(TB_EC.Text);
            pkm.Species = WinFormsUtil.GetIndex(CB_Species);
            pkm.HeldItem = WinFormsUtil.GetIndex(CB_HeldItem);
            pkm.Ability = WinFormsUtil.GetIndex(CB_Ability);
            pkm.AbilityNumber = CB_Ability.SelectedIndex << 1;
            pkm.Nature = WinFormsUtil.GetIndex(CB_Nature);
            pkm.Gender = PKX.GetGenderFromString(Label_Gender.Text);
            pkm.Form = CB_Form.SelectedIndex;
            pkm.EV_HP = Math.Min(Convert.ToInt32(TB_HPEV.Text), 252);
            pkm.EV_ATK = Math.Min(Convert.ToInt32(TB_ATKEV.Text), 252);
            pkm.EV_DEF = Math.Min(Convert.ToInt32(TB_DEFEV.Text), 252);
            pkm.EV_SPA = Math.Min(Convert.ToInt32(TB_SPAEV.Text), 252);
            pkm.EV_SPD = Math.Min(Convert.ToInt32(TB_SPDEV.Text), 252);
            pkm.EV_SPE = Math.Min(Convert.ToInt32(TB_SPEEV.Text), 252);
            pkm.Move1 = WinFormsUtil.GetIndex(CB_Move1);
            pkm.Move2 = WinFormsUtil.GetIndex(CB_Move2);
            pkm.Move3 = WinFormsUtil.GetIndex(CB_Move3);
            pkm.Move4 = WinFormsUtil.GetIndex(CB_Move4);
            pkm.Move1_PPUps = CB_PPu1.SelectedIndex;
            pkm.Move2_PPUps = CB_PPu2.SelectedIndex;
            pkm.Move3_PPUps = CB_PPu3.SelectedIndex;
            pkm.Move4_PPUps = CB_PPu4.SelectedIndex;
            pkm.IV_HP = Convert.ToByte(TB_HPIV.Text) & 0x1F;
            pkm.IV_ATK = Convert.ToByte(TB_ATKIV.Text) & 0x1F;
            pkm.IV_DEF = Convert.ToByte(TB_DEFIV.Text) & 0x1F;
            pkm.IV_SPA = Convert.ToByte(TB_SPAIV.Text) & 0x1F;
            pkm.IV_SPD = Convert.ToByte(TB_SPDIV.Text) & 0x1F;
            pkm.IV_SPE = Convert.ToByte(TB_SPEIV.Text) & 0x1F;
            pkm.IsShiny = CHK_Shiny.Checked;
            pkm.CurrentFriendship = Convert.ToByte(TB_Friendship.Text);
            pkm.Ball = WinFormsUtil.GetIndex(CB_Ball);
            pkm.CurrentLevel = Convert.ToByte(TB_Level.Text);
        }

        private void LoadPKM(SecretBase6PKM pkm)
        {
            CurrentPKM = pkm;

            // Put data into fields.
            TB_EC.Text = pkm.EncryptionConstant.ToString("X8");
            SetGenderLabel(pkm.Gender);
            CB_Species.SelectedValue = pkm.Species;
            CB_HeldItem.SelectedValue = pkm.HeldItem;
            CB_Form.SelectedIndex = pkm.Form;

            CB_Nature.SelectedValue = pkm.Nature;
            CB_Ball.SelectedValue = pkm.Ball;

            TB_HPIV.Text = pkm.IV_HP.ToString();
            TB_ATKIV.Text = pkm.IV_ATK.ToString();
            TB_DEFIV.Text = pkm.IV_DEF.ToString();
            TB_SPAIV.Text = pkm.IV_SPA.ToString();
            TB_SPDIV.Text = pkm.IV_SPD.ToString();
            TB_SPEIV.Text = pkm.IV_SPE.ToString();

            TB_HPEV.Text = pkm.EV_HP.ToString();
            TB_ATKEV.Text = pkm.EV_ATK.ToString();
            TB_DEFEV.Text = pkm.EV_DEF.ToString();
            TB_SPAEV.Text = pkm.EV_SPA.ToString();
            TB_SPDEV.Text = pkm.EV_SPD.ToString();
            TB_SPEEV.Text = pkm.EV_SPE.ToString();

            TB_Friendship.Text = pkm.CurrentFriendship.ToString();
            TB_Level.Text = pkm.CurrentLevel.ToString();

            CB_Move1.SelectedValue = pkm.Move1;
            CB_Move2.SelectedValue = pkm.Move2;
            CB_Move3.SelectedValue = pkm.Move3;
            CB_Move4.SelectedValue = pkm.Move4;
            CB_PPu1.SelectedIndex = pkm.Move1_PPUps;
            CB_PPu2.SelectedIndex = pkm.Move2_PPUps;
            CB_PPu3.SelectedIndex = pkm.Move3_PPUps;
            CB_PPu4.SelectedIndex = pkm.Move4_PPUps;

            CHK_Shiny.Checked = pkm.IsShiny;

            // Set Ability
            SetAbilityList(pkm.Species, pkm.Form, pkm.AbilityNumber >> 1);
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

            SavePlacement(bdata, CurrentPlacementIndex);
            var pIndex = (int)NUD_FObject.Value;
            LoadPlacement(bdata, bdata.GetPlacement(pIndex), pIndex);
        }

        private void ChangeIndexPKM(object sender, EventArgs e)
        {
            var bdata = CurrentBase;
            if (bdata is not SecretBase6Other o)
                return;

            var pkm = CurrentPKM;
            if (pkm is null || CurrentPKMIndex < 0)
                return;

            SavePKM(pkm);
            o.SetParticipant(CurrentPKMIndex, pkm);

            var index = CurrentPKMIndex = (int) NUD_FPKM.Value;
            pkm = o.GetParticipant(index);
            LoadPKM(pkm);
        }
        #endregion

        #region PKM
        private void SetAbilityList()
        {
            int species = WinFormsUtil.GetIndex(CB_Species);
            var form = CB_Form.SelectedIndex;
            var index = CB_Ability.SelectedIndex;
            SetAbilityList(species, form, index);
        }

        private void SetAbilityList(int species, int form, int abilityIndex)
        {
            var abilities = PersonalTable.AO.GetFormEntry(species, form).Abilities;
            var list = GameInfo.FilteredSources.GetAbilityList(abilities, 6);
            CB_Ability.DataSource = new BindingSource(list, null);
            CB_Ability.SelectedIndex = abilityIndex < 3 ? abilityIndex : 0;
        }

        private void SetForms()
        {
            int species = WinFormsUtil.GetIndex(CB_Species);
            bool hasForms = FormInfo.HasFormSelection(PersonalTable.AO[species], species, 6);
            CB_Form.Enabled = CB_Form.Visible = hasForms;

            var list = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, 6);
            CB_Form.DataSource = new BindingSource(list, null);
        }

        private void UpdateSpecies(object sender, EventArgs e)
        {
            // Get Forms for Given Species
            SetForms();

            var species = WinFormsUtil.GetIndex(CB_Species);

            // Set a sane gender
            var gender = SAV.Personal[species].FixedGender;
            if (gender == -1)
                gender = PKX.GetGenderFromString(Label_Gender.Text);
            SetGenderLabel(gender);

            SetAbilityList();
        }

        private void UpdateForm(object sender, EventArgs e)
        {
            SetAbilityList();

            // If form has a single gender, account for it.
            if (PKX.GetGenderFromString(CB_Form.Text) < 2)
                Label_Gender.Text = Main.GenderSymbols[CB_Form.SelectedIndex];
        }

        private void Label_Gender_Click(object sender, EventArgs e)
        {
            var species = WinFormsUtil.GetIndex(CB_Species);
            var pi = SAV.Personal[species];
            var fg = pi.FixedGender;
            if (fg == -1) // dual gender
            {
                fg = PKX.GetGenderFromString(Label_Gender.Text);
                fg = (fg ^ 1) & 1;
            }
            Label_Gender.Text = Main.GenderSymbols[fg];
        }

        private void SetGenderLabel(int gender)
        {
            var symbols = Main.GenderSymbols;
            if ((uint) gender >= symbols.Count)
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
            if (obj is null) // shouldn't happen, we already size-check above.
                throw new NullReferenceException();

            var sb = CurrentBase;
            if (sb is null)
                throw new NullReferenceException();

            ResetLoadNew();
            sb.Load(obj);
            ReloadSecretBaseList();
            LoadCurrent(sb);
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void B_Export_Click(object sender, EventArgs e)
        {
            var sb = CurrentBase;
            if (sb is null)
                throw new NullReferenceException();

            SaveCurrent(sb);
            var tr = sb.TrainerName;
            if (string.IsNullOrWhiteSpace(tr))
                tr = "Trainer";
            using var sfd = new SaveFileDialog {Filter = "Secret Base Data|*.sb6", FileName = $"{sb.BaseLocation:D2} - {Util.CleanFileName(tr)}.sb6"};
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
            if (bdata is null)
                throw new NullReferenceException();

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
}
