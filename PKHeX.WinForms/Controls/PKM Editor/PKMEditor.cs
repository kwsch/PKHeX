using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using System.ComponentModel;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor : UserControl
    {
        public PKMEditor()
        {
            InitializeComponent();
            Legality = new LegalityAnalysis(pkm = new PK7());
            SetPKMFormatMode(pkm.Format);

            GB_OT.Click += ClickGT;
            GB_nOT.Click += ClickGT;
            GB_CurrentMoves.Click += ClickMoves;
            GB_RelearnMoves.Click += ClickMoves;

            TB_Nickname.Font = FontUtil.GetPKXFont(11);
            TB_OT.Font = (Font)TB_Nickname.Font.Clone();
            TB_OTt2.Font = (Font)TB_Nickname.Font.Clone();

            relearnPB = new[] { PB_WarnRelearn1, PB_WarnRelearn2, PB_WarnRelearn3, PB_WarnRelearn4 };
            movePB = new[] { PB_WarnMove1, PB_WarnMove2, PB_WarnMove3, PB_WarnMove4 };
            foreach (var c in WinFormsUtil.GetAllControlsOfType(this, typeof(ComboBox)))
                c.KeyDown += WinFormsUtil.RemoveDropCB;
        }

        public PKM CurrentPKM { get => fieldsInitialized ? PreparePKM() : pkm; set => pkm = value; }
        public bool ModifyPKM { private get; set; } = true;
        public bool Unicode { private get; set; } = true;
        public bool HaX { private get; set; }
        public byte[] LastData { private get; set; }

        private PKM pkm;
        private bool fieldsInitialized;
        private bool fieldsLoaded;
        private bool changingFields;
        private GameVersion origintrack;
        private Action GetFieldsfromPKM;
        private Func<PKM> GetPKMfromFields;
        private LegalityAnalysis Legality;
        private string[] gendersymbols = { "♂", "♀", "-" };
        private readonly Image mixedHighlight = ImageUtil.ChangeOpacity(Resources.slotSet, 0.5);

        public event EventHandler LegalityChanged;
        public event EventHandler UpdatePreviewSprite;
        public event EventHandler RequestShowdownImport;
        public event EventHandler RequestShowdownExport;
        public event ReturnSAVEventHandler SaveFileRequested;
        public delegate SaveFile ReturnSAVEventHandler(object sender, EventArgs args);

        private readonly PictureBox[] movePB, relearnPB;
        private readonly ToolTip Tip1 = new ToolTip(), Tip2 = new ToolTip(), Tip3 = new ToolTip(), NatureTip = new ToolTip(), EVTip = new ToolTip();
        private SaveFile RequestSaveFile => SaveFileRequested?.Invoke(this, EventArgs.Empty);
        public bool PKMIsUnsaved => fieldsInitialized && fieldsLoaded && LastData != null && LastData.Any(b => b != 0) && !LastData.SequenceEqual(PreparePKM().Data);
        public bool IsEmptyOrEgg => CHK_IsEgg.Checked || CB_Species.SelectedIndex == 0;

        public PKM PreparePKM(bool click = true)
        {
            if (click)
                ValidateChildren();
            PKM pk = GetPKMfromFields();
            return pk?.Clone();
        }
        public bool VerifiedPKM()
        {
            if (ModifierKeys == (Keys.Control | Keys.Shift | Keys.Alt))
                return true; // Override
            // Make sure the PKX Fields are filled out properly (color check)
            ComboBox[] cba = {
                CB_Species, CB_Nature, CB_HeldItem, CB_Ability, // Main Tab
                CB_MetLocation, CB_EggLocation, CB_Ball,   // Met Tab
                CB_Move1, CB_Move2, CB_Move3, CB_Move4,    // Moves
                CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 // Moves
            };

            ComboBox cb = cba.FirstOrDefault(c => c.BackColor == Color.DarkSalmon && c.Items.Count != 0);
            if (cb != null)
            {
                Control c = cb.Parent; while (!(c is TabPage)) c = c.Parent;
                tabMain.SelectedTab = c as TabPage;
            }
            else if (pkm.Format >= 3 && Convert.ToUInt32(TB_EVTotal.Text) > 510 && !CHK_HackedStats.Checked)
                tabMain.SelectedTab = Tab_Stats;
            else if (WinFormsUtil.GetIndex(CB_Species) == 0)
                tabMain.SelectedTab = Tab_Main;
            else
                return true;

            System.Media.SystemSounds.Exclamation.Play();
            return false;
        }

        public void InitializeFields()
        {
            // Now that the ComboBoxes are ready, load the data.
            fieldsInitialized = true;
            pkm.RefreshChecksum();

            // Load Data
            PopulateFields(pkm);
        }

        public void SetPKMFormatMode(int Format)
        {
            byte[] extraBytes = new byte[0];
            switch (Format)
            {
                case 1:
                    GetFieldsfromPKM = PopulateFieldsPK1;
                    GetPKMfromFields = PreparePK1;
                    break;
                case 2:
                    GetFieldsfromPKM = PopulateFieldsPK2;
                    GetPKMfromFields = PreparePK2;
                    break;
                case 3:
                    if (pkm is CK3)
                    {
                        GetFieldsfromPKM = PopulateFieldsCK3;
                        GetPKMfromFields = PrepareCK3;
                        extraBytes = CK3.ExtraBytes;
                        break;
                    }
                    if (pkm is XK3)
                    {
                        GetFieldsfromPKM = PopulateFieldsXK3;
                        GetPKMfromFields = PrepareXK3;
                        extraBytes = XK3.ExtraBytes;
                        break;
                    }
                    GetFieldsfromPKM = PopulateFieldsPK3;
                    GetPKMfromFields = PreparePK3;
                    extraBytes = PK3.ExtraBytes;
                    break;
                case 4:
                    GetFieldsfromPKM = PopulateFieldsPK4;
                    GetPKMfromFields = PreparePK4;
                    extraBytes = PK4.ExtraBytes;
                    break;
                case 5:
                    GetFieldsfromPKM = PopulateFieldsPK5;
                    GetPKMfromFields = PreparePK5;
                    extraBytes = PK5.ExtraBytes;
                    break;
                case 6:
                    GetFieldsfromPKM = PopulateFieldsPK6;
                    GetPKMfromFields = PreparePK6;
                    extraBytes = PK6.ExtraBytes;
                    break;
                case 7:
                    GetFieldsfromPKM = PopulateFieldsPK7;
                    GetPKMfromFields = PreparePK7;
                    extraBytes = PK7.ExtraBytes;
                    break;
            }

            // Load Extra Byte List
            GB_ExtraBytes.Visible = GB_ExtraBytes.Enabled = extraBytes.Length != 0;
            CB_ExtraBytes.Items.Clear();
            foreach (byte b in extraBytes)
                CB_ExtraBytes.Items.Add($"0x{b:X2}");
            if (GB_ExtraBytes.Enabled)
                CB_ExtraBytes.SelectedIndex = 0;
        }
        public void PopulateFields(PKM pk, bool focus = true)
        {
            if (pk == null) { WinFormsUtil.Error("Attempted to load a null file."); return; }

            if (!PKMConverter.IsConvertibleToFormat(pk, pkm.Format))
            { WinFormsUtil.Alert($"Can't load Gen{pk.Format} to Gen{pkm.Format} games."); return; }

            bool oldInit = fieldsInitialized;
            fieldsInitialized = fieldsLoaded = false;
            if (focus)
                Tab_Main.Focus();

            if (fieldsInitialized & !pkm.ChecksumValid)
                WinFormsUtil.Alert("PKM File has an invalid checksum.");

            if (pk.Format != pkm.Format) // past gen format
            {
                pkm = PKMConverter.ConvertToType(pk.Clone(), pkm.GetType(), out string _);
                if (pkm == null)
                    pkm = pk.Clone();
                else if (pk.Format != pkm.Format && focus) // converted
                    WinFormsUtil.Alert("Converted File.");
            }
            else
                pkm = pk.Clone();

            try { GetFieldsfromPKM(); }
            catch { fieldsInitialized = oldInit; throw; }

            CB_EncounterType.Visible = Label_EncounterType.Visible = pkm.Gen4 && pkm.Format < 7;
            fieldsInitialized = oldInit;
            UpdateIVs(null, null);
            UpdatePKRSInfected(null, null);
            UpdatePKRSCured(null, null);

            if (HaX) // Load original values from pk not pkm
            {
                MT_Level.Text = (pk.Stat_HPMax != 0 ? pk.Stat_Level : PKX.GetLevel(pk.Species, pk.EXP)).ToString();
                TB_EXP.Text = pk.EXP.ToString();
                MT_Form.Text = pk.AltForm.ToString();
                if (pk.Stat_HPMax != 0) // stats present
                {
                    Stat_HP.Text = pk.Stat_HPCurrent.ToString();
                    Stat_ATK.Text = pk.Stat_ATK.ToString();
                    Stat_DEF.Text = pk.Stat_DEF.ToString();
                    Stat_SPA.Text = pk.Stat_SPA.ToString();
                    Stat_SPD.Text = pk.Stat_SPD.ToString();
                    Stat_SPE.Text = pk.Stat_SPE.ToString();
                }
            }
            fieldsLoaded = true;

            Label_HatchCounter.Visible = CHK_IsEgg.Checked && pkm.Format > 1;
            Label_Friendship.Visible = !CHK_IsEgg.Checked && pkm.Format > 1;

            SetMarkings();
            UpdateLegality();
            UpdateSprite();
            LastData = PreparePKM()?.Data;
        }
        public void UpdateLegality(LegalityAnalysis la = null, bool skipMoveRepop = false)
        {
            if (!fieldsLoaded)
                return;

            Legality = la ?? new LegalityAnalysis(pkm);
            if (!Legality.Parsed || HaX || pkm.Species == 0)
            {
                PB_WarnMove1.Visible = PB_WarnMove2.Visible = PB_WarnMove3.Visible = PB_WarnMove4.Visible =
                    PB_WarnRelearn1.Visible = PB_WarnRelearn2.Visible = PB_WarnRelearn3.Visible = PB_WarnRelearn4.Visible = false;
                return;
            }

            // Refresh Move Legality
            for (int i = 0; i < 4; i++)
                movePB[i].Visible = !Legality.Info?.Moves[i].Valid ?? false;

            if (pkm.Format >= 6)
                for (int i = 0; i < 4; i++)
                    relearnPB[i].Visible = !Legality.Info?.Relearn[i].Valid ?? false;

            if (skipMoveRepop)
                return;
            // Resort moves
            bool tmp = fieldsLoaded;
            fieldsLoaded = false;
            var cb = new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
            var moves = Legality.AllSuggestedMovesAndRelearn;
            var moveList = GameInfo.MoveDataSource.OrderByDescending(m => moves.Contains(m.Value)).ToArray();
            foreach (ComboBox c in cb)
            {
                var index = WinFormsUtil.GetIndex(c);
                c.DataSource = new BindingSource(moveList, null);
                c.SelectedValue = index;
                if (c.Visible)
                    c.SelectionLength = 0; // flicker hack
            }
            fieldsLoaded |= tmp;
            LegalityChanged?.Invoke(Legality.Valid, null);
        }
        public void UpdateUnicode(string[] symbols)
        {
            gendersymbols = symbols;
            if (!Unicode)
            {
                BTN_Shinytize.Text = "*";
                TB_Nickname.Font = TB_OT.Font = TB_OTt2.Font = Label_TID.Font;
            }
            else
            {
                BTN_Shinytize.Text = "☆";
                TB_Nickname.Font = TB_OT.Font = TB_OTt2.Font = FontUtil.GetPKXFont(11);
            }
            // Switch active gender labels to new if they are active.
            if (PKX.GetGender(Label_Gender.Text) < 2)
                Label_Gender.Text = gendersymbols[PKX.GetGender(Label_Gender.Text)];
            if (PKX.GetGender(Label_OTGender.Text) < 2)
                Label_OTGender.Text = gendersymbols[PKX.GetGender(Label_OTGender.Text)];
            if (PKX.GetGender(Label_CTGender.Text) < 2)
                Label_CTGender.Text = gendersymbols[PKX.GetGender(Label_CTGender.Text)];
        }
        private void UpdateSprite()
        {
            if (fieldsLoaded && fieldsInitialized)
                UpdatePreviewSprite?.Invoke(this, null);
        }

        // General Use Functions //
        private Color GetGenderColor(int gender)
        {
            if (gender == 0) // male
                return Color.Blue;
            if (gender == 1) // female
                return Color.Red;
            return CB_Species.ForeColor;
        }
        private void SetDetailsOT(SaveFile SAV)
        {
            if (SAV?.Exportable != true)
                return;

            // Get Save Information
            TB_OT.Text = SAV.OT;
            Label_OTGender.Text = gendersymbols[SAV.Gender & 1];
            Label_OTGender.ForeColor = GetGenderColor(SAV.Gender & 1);
            TB_TID.Text = SAV.TID.ToString("00000");
            TB_SID.Text = SAV.SID.ToString("00000");

            if (SAV.Game >= 0)
                CB_GameOrigin.SelectedValue = SAV.Game;
            if (SAV.Language >= 0)
                CB_Language.SelectedValue = SAV.Language;
            if (SAV.HasGeolocation)
            {
                CB_3DSReg.SelectedValue = SAV.ConsoleRegion;
                CB_Country.SelectedValue = SAV.Country;
                CB_SubRegion.SelectedValue = SAV.SubRegion;
            }
            UpdateNickname(null, null);
        }
        private void SetDetailsHT(SaveFile SAV)
        {
            if (SAV?.Exportable != true)
                return;

            if (TB_OTt2.Text.Length > 0)
                Label_CTGender.Text = gendersymbols[SAV.Gender & 1];
        }
        private void SetForms()
        {
            int species = WinFormsUtil.GetIndex(CB_Species);
            if (pkm.Format < 4 && species != 201)
            {
                Label_Form.Visible = CB_Form.Visible = CB_Form.Enabled = false;
                return;
            }

            int count = (RequestSaveFile?.Personal[species] ?? pkm.PersonalInfo).FormeCount;
            bool hasForms = count > 1 || new[] { 201, 664, 665, 414 }.Contains(species);
            CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = hasForms;

            if (HaX && pkm.Format >= 4)
                Label_Form.Visible = true;

            if (!hasForms)
                return;

            var ds = PKX.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, gendersymbols, pkm.Format).ToList();
            if (ds.Count == 1 && string.IsNullOrEmpty(ds[0])) // empty (Alolan Totems)
                CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = false;
            else CB_Form.DataSource = ds;
        }
        private void SetAbilityList()
        {
            if (pkm.Format < 3) // no abilities
                return;

            if (pkm.Format > 3 && fieldsLoaded) // has forms
                pkm.AltForm = CB_Form.SelectedIndex;

            int[] abils = pkm.PersonalInfo.Abilities;
            if (abils[1] == 0 && pkm.Format != 3)
                abils[1] = abils[0];
            string[] abilIdentifier = { " (1)", " (2)", " (H)" };
            List<string> ability_list = abils.Where(a => a != 0).Select((t, i) => GameInfo.Strings.abilitylist[t] + abilIdentifier[i]).ToList();
            if (!ability_list.Any())
                ability_list.Add(GameInfo.Strings.abilitylist[0] + abilIdentifier[0]);

            bool tmp = fieldsLoaded;
            fieldsLoaded = false;
            int abil = CB_Ability.SelectedIndex;
            CB_Ability.DataSource = ability_list;
            CB_Ability.SelectedIndex = abil < 0 || abil >= CB_Ability.Items.Count ? 0 : abil;
            fieldsLoaded = tmp;
        }
        private void SetIsShiny(object sender)
        {
            if (sender == TB_PID)
                pkm.PID = Util.GetHexValue(TB_PID.Text);
            else if (sender == TB_TID)
                pkm.TID = (int)Util.ToUInt32(TB_TID.Text);
            else if (sender == TB_SID)
                pkm.SID = (int)Util.ToUInt32(TB_SID.Text);

            bool isShiny = pkm.IsShiny;

            // Set the Controls
            BTN_Shinytize.Visible = BTN_Shinytize.Enabled = !isShiny;
            Label_IsShiny.Visible = isShiny;

            // Refresh Markings (for Shiny Star if applicable)
            SetMarkings();
        }
        private void SetMarkings()
        {
            double getOpacity(bool b) => b ? 1 : 0.175;
            PictureBox[] pba = { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            for (int i = 0; i < pba.Length; i++)
                pba[i].Image = ImageUtil.ChangeOpacity(pba[i].InitialImage, getOpacity(pkm.Markings[i] != 0));

            PB_MarkShiny.Image = ImageUtil.ChangeOpacity(PB_MarkShiny.InitialImage, getOpacity(!BTN_Shinytize.Enabled));
            PB_MarkCured.Image = ImageUtil.ChangeOpacity(PB_MarkCured.InitialImage, getOpacity(CHK_Cured.Checked));

            PB_MarkPentagon.Image = ImageUtil.ChangeOpacity(PB_MarkPentagon.InitialImage, getOpacity(pkm.Gen6));

            // Gen7 Markings
            if (pkm.Format != 7)
                return;

            PB_MarkAlola.Image = ImageUtil.ChangeOpacity(PB_MarkAlola.InitialImage, getOpacity(pkm.Gen7));
            PB_MarkVC.Image = ImageUtil.ChangeOpacity(PB_MarkVC.InitialImage, getOpacity(pkm.VC));
            PB_MarkHorohoro.Image = ImageUtil.ChangeOpacity(PB_MarkHorohoro.InitialImage, getOpacity(pkm.Horohoro));

            for (int i = 0; i < pba.Length; i++)
            {
                switch (pkm.Markings[i])
                {
                    case 1:
                        pba[i].Image = ImageUtil.ChangeAllColorTo(pba[i].Image, Color.FromArgb(000, 191, 255));
                        break;
                    case 2:
                        pba[i].Image = ImageUtil.ChangeAllColorTo(pba[i].Image, Color.FromArgb(255, 117, 179));
                        break;
                }
            }
        }
        private void UpdateGender()
        {
            int cg = PKX.GetGender(Label_Gender.Text);
            int gt = pkm.PersonalInfo.Gender;

            int Gender;
            if (gt == 255)      // Genderless
                Gender = 2;
            else if (gt == 254) // Female Only
                Gender = 1;
            else if (gt == 0)  // Male Only
                Gender = 0;
            else if (cg == 2 || WinFormsUtil.GetIndex(CB_GameOrigin) < 24)
                Gender = (Util.GetHexValue(TB_PID.Text) & 0xFF) <= gt ? 1 : 0;
            else
                Gender = cg;

            Label_Gender.Text = gendersymbols[Gender];
            Label_Gender.ForeColor = GetGenderColor(Gender);
        }
        private void UpdateStats()
        {
            // Generate the stats.
            pkm.SetStats(pkm.GetStats(pkm.PersonalInfo));

            Stat_HP.Text = pkm.Stat_HPCurrent.ToString();
            Stat_ATK.Text = pkm.Stat_ATK.ToString();
            Stat_DEF.Text = pkm.Stat_DEF.ToString();
            Stat_SPA.Text = pkm.Stat_SPA.ToString();
            Stat_SPD.Text = pkm.Stat_SPD.ToString();
            Stat_SPE.Text = pkm.Stat_SPE.ToString();

            // Recolor the Stat Labels based on boosted stats.
            {
                int incr = pkm.Nature / 5;
                int decr = pkm.Nature % 5;

                Label[] labarray = { Label_ATK, Label_DEF, Label_SPE, Label_SPA, Label_SPD };
                // Reset Label Colors
                foreach (Label label in labarray)
                    label.ResetForeColor();

                // Set Colored StatLabels only if Nature isn't Neutral
                if (incr == decr) return;
                labarray[incr].ForeColor = Color.Red;
                labarray[decr].ForeColor = Color.Blue;
            }
        }
        private void SetCountrySubRegion(ComboBox CB, string type)
        {
            int index = CB.SelectedIndex;
            // fix for Korean / Chinese being swapped
            string cl = GameInfo.CurrentLanguage + "";
            cl = cl == "zh" ? "ko" : cl == "ko" ? "zh" : cl;

            CB.DataSource = Util.GetCBList(type, cl);

            if (index > 0 && index < CB.Items.Count && fieldsInitialized)
                CB.SelectedIndex = index;
        }

        // Prompted Updates of PKM //
        private void ClickFriendship(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // prompt to reset
                TB_Friendship.Text = pkm.CurrentFriendship.ToString();
            else
                TB_Friendship.Text = TB_Friendship.Text == "255" ? pkm.PersonalInfo.BaseFriendship.ToString() : "255";
        }
        private void ClickLevel(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
                ((MaskedTextBox)sender).Text = "100";
        }
        private void ClickGender(object sender, EventArgs e)
        {
            // Get Gender Threshold
            int gt = pkm.PersonalInfo.Gender;

            if (gt == 255 || gt == 0 || gt == 254) // Single gender/genderless
                return;

            if (gt >= 255) return;
            // If not a single gender(less) species: (should be <254 but whatever, 255 never happens)

            int newGender = PKX.GetGender(Label_Gender.Text) ^ 1;
            if (pkm.Format == 2)
                do { TB_ATKIV.Text = (Util.Rand32() & pkm.MaxIV).ToString(); } while (PKX.GetGender(Label_Gender.Text) != newGender);
            else if (pkm.Format <= 4)
            {
                if (fieldsLoaded)
                    pkm.Species = WinFormsUtil.GetIndex(CB_Species);
                pkm.Version = WinFormsUtil.GetIndex(CB_GameOrigin);
                pkm.Nature = WinFormsUtil.GetIndex(CB_Nature);
                pkm.AltForm = CB_Form.SelectedIndex;

                pkm.SetPIDGender(newGender);
                TB_PID.Text = pkm.PID.ToString("X8");
            }
            pkm.Gender = newGender;
            Label_Gender.Text = gendersymbols[pkm.Gender];
            Label_Gender.ForeColor = GetGenderColor(pkm.Gender);

            if (PKX.GetGender(CB_Form.Text) < 2) // Gendered Forms
                CB_Form.SelectedIndex = PKX.GetGender(Label_Gender.Text);

            UpdatePreviewSprite(Label_Gender, null);
        }
        private void ClickPPUps(object sender, EventArgs e)
        {
            CB_PPu1.SelectedIndex = ModifierKeys != Keys.Control && WinFormsUtil.GetIndex(CB_Move1) > 0 ? 3 : 0;
            CB_PPu2.SelectedIndex = ModifierKeys != Keys.Control && WinFormsUtil.GetIndex(CB_Move2) > 0 ? 3 : 0;
            CB_PPu3.SelectedIndex = ModifierKeys != Keys.Control && WinFormsUtil.GetIndex(CB_Move3) > 0 ? 3 : 0;
            CB_PPu4.SelectedIndex = ModifierKeys != Keys.Control && WinFormsUtil.GetIndex(CB_Move4) > 0 ? 3 : 0;
        }
        private void ClickMarking(object sender, EventArgs e)
        {
            PictureBox[] pba = { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            int index = Array.IndexOf(pba, sender);

            // Handling Gens 3-6
            int[] markings = pkm.Markings;
            switch (pkm.Format)
            {
                case 3:
                case 4:
                case 5:
                case 6: // on/off
                    markings[index] ^= 1; // toggle
                    pkm.Markings = markings;
                    break;
                case 7: // 0 (none) | 1 (blue) | 2 (pink)
                    markings[index] = (markings[index] + 1) % 3; // cycle
                    pkm.Markings = markings;
                    break;
                default:
                    return;
            }
            SetMarkings();
        }
        private void ClickStatLabel(object sender, MouseEventArgs e)
        {
            if (!(ModifierKeys == Keys.Control || ModifierKeys == Keys.Alt))
                return;

            if (sender == Label_SPC)
                sender = Label_SPA;
            int index = Array.IndexOf(new[] { Label_HP, Label_ATK, Label_DEF, Label_SPA, Label_SPD, Label_SPE }, sender);

            if (ModifierKeys == Keys.Alt) // EV
            {
                var mt = new[] { TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPAEV, TB_SPDEV, TB_SPEEV }[index];
                if (e.Button == MouseButtons.Left) // max
                    mt.Text = pkm.Format >= 3
                        ? Math.Min(Math.Max(510 - Util.ToInt32(TB_EVTotal.Text) + Util.ToInt32(mt.Text), 0), 252).ToString()
                        : ushort.MaxValue.ToString();
                else // min
                    mt.Text = 0.ToString();
            }
            else
                new[] { TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPAIV, TB_SPDIV, TB_SPEIV }[index].Text =
                    (e.Button == MouseButtons.Left ? pkm.MaxIV : 0).ToString();
        }
        private void ClickIV(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
                if (pkm.Format < 7)
                    ((MaskedTextBox)sender).Text = pkm.MaxIV.ToString();
                else
                {
                    var index = Array.IndexOf(new[] { TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPAIV, TB_SPDIV, TB_SPEIV }, sender);
                    pkm.HyperTrainInvert(index);
                    UpdateIVs(sender, e);
                }
            else if (ModifierKeys == Keys.Alt)
                ((MaskedTextBox)sender).Text = 0.ToString();
        }
        private void ClickEV(object sender, EventArgs e)
        {
            MaskedTextBox mt = (MaskedTextBox)sender;
            if (ModifierKeys == Keys.Control) // EV
                mt.Text = pkm.Format >= 3
                    ? Math.Min(Math.Max(510 - Util.ToInt32(TB_EVTotal.Text) + Util.ToInt32(mt.Text), 0), 252).ToString()
                    : ushort.MaxValue.ToString();
            else if (ModifierKeys == Keys.Alt)
                mt.Text = 0.ToString();
        }
        private void ClickOT(object sender, EventArgs e) => SetDetailsOT(SaveFileRequested?.Invoke(this, e));
        private void ClickCT(object sender, EventArgs e) => SetDetailsHT(SaveFileRequested?.Invoke(this, e));
        private void ClickTRGender(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (!string.IsNullOrWhiteSpace(lbl?.Text)) // set gender label (toggle M/F)
            {
                int gender = PKX.GetGender(lbl.Text) ^ 1;
                lbl.Text = gendersymbols[gender];
                lbl.ForeColor = GetGenderColor(gender);
            }
        }
        private void ClickBall(object sender, EventArgs e) => CB_Ball.SelectedIndex = 0;
        private void ClickShinyLeaf(object sender, EventArgs e) => ShinyLeaf.CheckAll(ModifierKeys != Keys.Control);
        private void ClickMetLocation(object sender, EventArgs e)
        {
            if (HaX)
                return;

            pkm = PreparePKM();
            UpdateLegality(skipMoveRepop: true);
            if (Legality.Valid)
                return;
            if (!SetSuggestedMetLocation())
                return;

            pkm = PreparePKM();
            UpdateLegality();
        }
        private void ClickGT(object sender, EventArgs e)
        {
            if (!GB_nOT.Visible)
                return;
            if (sender == GB_OT)
            {
                pkm.CurrentHandler = 0;
                GB_OT.BackgroundImage = mixedHighlight;
                GB_nOT.BackgroundImage = null;
            }
            else if (TB_OTt2.Text.Length > 0)
            {
                pkm.CurrentHandler = 1;
                GB_OT.BackgroundImage = null;
                GB_nOT.BackgroundImage = mixedHighlight;
            }
            TB_Friendship.Text = pkm.CurrentFriendship.ToString();
        }
        private void ClickMoves(object sender, EventArgs e)
        {
            UpdateLegality(skipMoveRepop: true);
            if (sender == GB_CurrentMoves)
            {
                if (!SetSuggestedMoves(random: ModifierKeys == Keys.Control))
                    return;
            }
            else if (sender == GB_RelearnMoves)
            {
                if (!SetSuggestedRelearnMoves())
                    return;
            }
            else
            {
                return;
            }

            UpdateLegality();
        }
        private bool SetSuggestedMoves(bool random = false, bool silent = false)
        {
            int[] m = Legality.GetSuggestedMoves(tm: random, tutor: random, reminder: random);
            if (m == null)
            {
                if (!silent)
                    WinFormsUtil.Alert("Suggestions are not enabled for this PKM format.");
                return false;
            }

            if (random)
                Util.Shuffle(m);
            if (m.Length > 4)
                m = m.Skip(m.Length - 4).ToArray();
            Array.Resize(ref m, 4);

            if (pkm.Moves.SequenceEqual(m))
                return false;

            if (!silent)
            {
                var movestrings = m.Select(v => v >= GameInfo.Strings.movelist.Length ? "ERROR" : GameInfo.Strings.movelist[v]);
                string r = string.Join(Environment.NewLine, movestrings);
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Apply suggested current moves?", r))
                    return false;
            }

            CB_Move1.SelectedValue = m[0];
            CB_Move2.SelectedValue = m[1];
            CB_Move3.SelectedValue = m[2];
            CB_Move4.SelectedValue = m[3];
            return true;
        }
        private bool SetSuggestedRelearnMoves(bool silent = false)
        {
            if (pkm.Format < 6)
                return false;

            int[] m = Legality.GetSuggestedRelearn();
            if (m.All(z => z == 0))
                if (!pkm.WasEgg && !pkm.WasEvent && !pkm.WasEventEgg && !pkm.WasLink)
                {
                    var encounter = Legality.GetSuggestedMetInfo();
                    if (encounter != null)
                        m = encounter.Relearn;
                }

            if (pkm.RelearnMoves.SequenceEqual(m))
                return false;

            if (!silent)
            {
                var movestrings = m.Select(v => v >= GameInfo.Strings.movelist.Length ? "ERROR" : GameInfo.Strings.movelist[v]);
                string r = string.Join(Environment.NewLine, movestrings);
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Apply suggested relearn moves?", r))
                    return false;
            }

            CB_RelearnMove1.SelectedValue = m[0];
            CB_RelearnMove2.SelectedValue = m[1];
            CB_RelearnMove3.SelectedValue = m[2];
            CB_RelearnMove4.SelectedValue = m[3];
            return true;
        }
        private bool SetSuggestedMetLocation(bool silent = false)
        {
            var encounter = Legality.GetSuggestedMetInfo();
            if (encounter == null || pkm.Format >= 3 && encounter.Location < 0)
            {
                if (!silent)
                    WinFormsUtil.Alert("Unable to provide a suggestion.");
                return false;
            }

            int level = encounter.Level;
            int location = encounter.Location;
            int minlvl = Legal.GetLowestLevel(pkm, encounter.Species);
            if (minlvl == 0)
                minlvl = level;

            if (pkm.CurrentLevel >= minlvl && pkm.Met_Level == level && pkm.Met_Location == location)
                return false;
            if (minlvl < level)
                minlvl = level;

            if (!silent)
            {
                var suggestion = new List<string> { "Suggested:" };
                if (pkm.Format >= 3)
                {
                    var met_list = GameInfo.GetLocationList((GameVersion)pkm.Version, pkm.Format, egg: false);
                    var locstr = met_list.FirstOrDefault(loc => loc.Value == location).Text;
                    suggestion.Add($"Met Location: {locstr}");
                    suggestion.Add($"Met Level: {level}");
                }
                if (pkm.CurrentLevel < minlvl)
                    suggestion.Add($"Current Level: {minlvl}");

                if (suggestion.Count == 1) // no suggestion
                    return false;

                string suggest = string.Join(Environment.NewLine, suggestion);
                if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, suggest) != DialogResult.Yes)
                    return false;
            }

            if (pkm.Format >= 3)
            {
                TB_MetLevel.Text = level.ToString();
                CB_MetLocation.SelectedValue = location;
            }

            if (pkm.CurrentLevel < minlvl)
                TB_Level.Text = minlvl.ToString();

            return true;
        }

        private void UpdateIVs(object sender, EventArgs e)
        {
            if (changingFields || !fieldsInitialized) return;
            if (sender != null && Util.ToInt32(((MaskedTextBox)sender).Text) > pkm.MaxIV)
                ((MaskedTextBox)sender).Text = pkm.MaxIV.ToString("00");

            changingFields = true;

            // Update IVs
            pkm.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pkm.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pkm.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pkm.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pkm.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pkm.IV_SPD = Util.ToInt32(TB_SPDIV.Text);

            var IV_Boxes = new[] { TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPAIV, TB_SPDIV, TB_SPEIV };
            var HT_Vals = new[] { pkm.HT_HP, pkm.HT_ATK, pkm.HT_DEF, pkm.HT_SPA, pkm.HT_SPD, pkm.HT_SPE };
            for (int i = 0; i < IV_Boxes.Length; i++)
                if (HT_Vals[i])
                    IV_Boxes[i].BackColor = Color.LightGreen;
                else
                    IV_Boxes[i].ResetBackColor();

            if (pkm.Format < 3)
            {
                TB_HPIV.Text = pkm.IV_HP.ToString();
                TB_SPDIV.Text = TB_SPAIV.Text;
                if (pkm.Format == 2)
                {
                    Label_Gender.Text = gendersymbols[pkm.Gender];
                    Label_Gender.ForeColor = GetGenderColor(pkm.Gender);
                    if (pkm.Species == 201 && e != null) // Unown
                        CB_Form.SelectedIndex = pkm.AltForm;
                }
                SetIsShiny(null);
                UpdateSprite();
            }

            CB_HPType.SelectedValue = pkm.HPType;
            changingFields = false;

            // Potential Reading
            L_Potential.Text = (Unicode
                ? new[] { "★☆☆☆", "★★☆☆", "★★★☆", "★★★★" }
                : new[] { "+", "++", "+++", "++++" }
            )[pkm.PotentialRating];

            TB_IVTotal.Text = pkm.IVs.Sum().ToString();

            int characteristic = pkm.Characteristic;
            L_Characteristic.Visible = Label_CharacteristicPrefix.Visible = characteristic > -1;
            if (characteristic > -1)
                L_Characteristic.Text = GameInfo.Strings.characteristics[pkm.Characteristic];
            UpdateStats();
        }
        private void UpdateEVs(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox m)
            {
                if (Util.ToInt32(m.Text) > pkm.MaxEV)
                { m.Text = pkm.MaxEV.ToString(); return; } // recursive on text set
            }

            changingFields = true;
            if (sender == TB_HPEV) pkm.EV_HP = Util.ToInt32(TB_HPEV.Text);
            else if (sender == TB_ATKEV) pkm.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            else if (sender == TB_DEFEV) pkm.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            else if (sender == TB_SPEEV) pkm.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            else if (sender == TB_SPAEV) pkm.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            else if (sender == TB_SPDEV) pkm.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            if (pkm.Format < 3)
                TB_SPDEV.Text = TB_SPAEV.Text;

            int evtotal = pkm.EVs.Sum();

            if (evtotal > 510) // Background turns Red
                TB_EVTotal.BackColor = Color.Red;
            else if (evtotal == 510) // Maximum EVs
                TB_EVTotal.BackColor = Color.Honeydew;
            else if (evtotal == 508) // Fishy EVs
                TB_EVTotal.BackColor = Color.LightYellow;
            else TB_EVTotal.BackColor = TB_IVTotal.BackColor;

            TB_EVTotal.Text = evtotal.ToString();
            EVTip.SetToolTip(TB_EVTotal, $"Remaining: {510 - evtotal}");
            changingFields = false;
            UpdateStats();
        }
        private void UpdateBall(object sender, EventArgs e)
        {
            PB_Ball.Image = PKMUtil.GetBallSprite(WinFormsUtil.GetIndex(CB_Ball));
        }
        private void UpdateEXPLevel(object sender, EventArgs e)
        {
            if (changingFields || !fieldsInitialized) return;

            changingFields = true;
            if (sender == TB_EXP)
            {
                // Change the Level
                uint EXP = Util.ToUInt32(TB_EXP.Text);
                int Species = WinFormsUtil.GetIndex(CB_Species);
                int Level = PKX.GetLevel(Species, EXP);
                if (Level == 100)
                    EXP = PKX.GetEXP(100, Species);

                TB_Level.Text = Level.ToString();
                if (!HaX)
                    TB_EXP.Text = EXP.ToString();
                else if (Level <= 100 && Util.ToInt32(MT_Level.Text) <= 100)
                    MT_Level.Text = Level.ToString();
            }
            else
            {
                // Change the XP
                int Level = Util.ToInt32((HaX ? MT_Level : TB_Level).Text);
                if (Level <= 0)
                    TB_Level.Text = "1";
                else if (Level > 100)
                {
                    TB_Level.Text = "100";
                    if (!HaX)
                        Level = 100;
                }
                if (Level > byte.MaxValue) MT_Level.Text = "255";

                if (Level <= 100)
                    TB_EXP.Text = PKX.GetEXP(Level, WinFormsUtil.GetIndex(CB_Species)).ToString();
            }
            changingFields = false;
            if (fieldsLoaded) // store values back
            {
                pkm.EXP = Util.ToUInt32(TB_EXP.Text);
                pkm.Stat_Level = Util.ToInt32((HaX ? MT_Level : TB_Level).Text);
            }
            UpdateStats();
            UpdateLegality();
        }
        private void UpdateHPType(object sender, EventArgs e)
        {
            if (changingFields || !fieldsInitialized) return;
            changingFields = true;
            int[] ivs =
            {
                Util.ToInt32(TB_HPIV.Text), Util.ToInt32(TB_ATKIV.Text), Util.ToInt32(TB_DEFIV.Text),
                Util.ToInt32(TB_SPAIV.Text), Util.ToInt32(TB_SPDIV.Text), Util.ToInt32(TB_SPEIV.Text)
            };

            // Change IVs to match the new Hidden Power
            int[] newIVs = PKX.SetHPIVs(WinFormsUtil.GetIndex(CB_HPType), ivs);
            TB_HPIV.Text = newIVs[0].ToString();
            TB_ATKIV.Text = newIVs[1].ToString();
            TB_DEFIV.Text = newIVs[2].ToString();
            TB_SPAIV.Text = newIVs[3].ToString();
            TB_SPDIV.Text = newIVs[4].ToString();
            TB_SPEIV.Text = newIVs[5].ToString();

            // Refresh View
            changingFields = false;
            UpdateIVs(null, null);
        }
        private void UpdateRandomIVs(object sender, EventArgs e)
        {
            changingFields = true;
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift) // Max IVs
            {
                TB_HPIV.Text = TB_ATKIV.Text = TB_DEFIV.Text = TB_SPAIV.Text = TB_SPDIV.Text = TB_SPEIV.Text = pkm.MaxIV.ToString();
            }
            else
            {
                var IVs = pkm.SetRandomIVs();
                var IVBoxes = new[] { TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPAIV, TB_SPDIV, TB_SPEIV };
                for (int i = 0; i < 6; i++)
                    IVBoxes[i].Text = IVs[i].ToString();
            }
            changingFields = false;
            UpdateIVs(null, e);
        }
        private void UpdateRandomEVs(object sender, EventArgs e)
        {
            changingFields = true;

            var tb = new[] { TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPAEV, TB_SPDEV, TB_SPEEV };
            bool zero = ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift;
            var evs = zero ? new uint[6] : PKX.GetRandomEVs(pkm.Format);
            for (int i = 0; i < 6; i++)
                tb[i].Text = evs[i].ToString();

            changingFields = false;
            UpdateEVs(null, null);
        }
        private void UpdateRandomPID(object sender, EventArgs e)
        {
            if (pkm.Format < 3)
                return;
            if (fieldsLoaded)
                pkm.PID = Util.GetHexValue(TB_PID.Text);

            if (sender == Label_Gender)
                pkm.SetPIDGender(pkm.Gender);
            else if (sender == CB_Nature && pkm.Nature != WinFormsUtil.GetIndex(CB_Nature))
                pkm.SetPIDNature(WinFormsUtil.GetIndex(CB_Nature));
            else if (sender == BTN_RerollPID)
                pkm.SetPIDGender(pkm.Gender);
            else if (sender == CB_Ability && CB_Ability.SelectedIndex != pkm.PIDAbility && pkm.PIDAbility > -1)
                pkm.PID = PKX.GetRandomPID(pkm.Species, pkm.Gender, pkm.Version, pkm.Nature, pkm.Format, (uint)(CB_Ability.SelectedIndex * 0x10001));

            TB_PID.Text = pkm.PID.ToString("X8");
            SetIsShiny(null);
            UpdateSprite();
            if (pkm.GenNumber < 6 && pkm.Format >= 6)
                TB_EC.Text = TB_PID.Text;
        }
        private void UpdateRandomEC(object sender, EventArgs e)
        {
            if (pkm.Format < 6)
                return;

            int wIndex = Array.IndexOf(Legal.WurmpleEvolutions, WinFormsUtil.GetIndex(CB_Species));
            if (wIndex < 0)
            {
                TB_EC.Text = Util.Rand32().ToString("X8");
            }
            else
            {
                int gen = pkm.GenNumber;
                uint EC;
                bool valid;
                do
                {
                    EC = Util.Rand32();
                    uint evoVal = PKX.GetWurmpleEvoVal(gen, EC);
                    valid = evoVal == wIndex / 2;
                } while (!valid);
                TB_EC.Text = EC.ToString("X8");
            }
            UpdateLegality();
        }
        private void UpdateHackedStats(object sender, EventArgs e)
        {
            Stat_HP.Enabled =
                Stat_ATK.Enabled =
                    Stat_DEF.Enabled =
                        Stat_SPA.Enabled =
                            Stat_SPD.Enabled =
                                Stat_SPE.Enabled = CHK_HackedStats.Checked;
        }
        private void UpdateHackedStatText(object sender, EventArgs e)
        {
            if (!CHK_HackedStats.Checked || !(sender is TextBox tb))
                return;

            string text = tb.Text;
            if (string.IsNullOrWhiteSpace(text))
                tb.Text = "0";

            if (Convert.ToUInt32(text) > ushort.MaxValue)
                tb.Text = "65535";
        }
        private void Update255_MTB(object sender, EventArgs e)
        {
            if (!(sender is MaskedTextBox tb)) return;
            if (Util.ToInt32(tb.Text) > byte.MaxValue)
                tb.Text = "255";
        }
        private void UpdateForm(object sender, EventArgs e)
        {
            if (CB_Form == sender && fieldsLoaded)
                pkm.AltForm = CB_Form.SelectedIndex;

            UpdateGender();
            UpdateStats();
            // Repopulate Abilities if Species Form has different abilities
            SetAbilityList();

            // Gender Forms
            if (WinFormsUtil.GetIndex(CB_Species) == 201 && fieldsLoaded)
            {
                if (pkm.Format == 3)
                {
                    pkm.SetPIDUnown3(CB_Form.SelectedIndex);
                    TB_PID.Text = pkm.PID.ToString("X8");
                }
                else if (pkm.Format == 2)
                {
                    int desiredForm = CB_Form.SelectedIndex;
                    while (pkm.AltForm != desiredForm)
                        UpdateRandomIVs(null, null);
                }
            }
            else if (PKX.GetGender(CB_Form.Text) < 2)
            {
                if (CB_Form.Items.Count == 2) // actually M/F; Pumpkaboo formes in German are S,M,L,XL
                    Label_Gender.Text = gendersymbols[PKX.GetGender(CB_Form.Text)];
            }

            if (changingFields)
                return;
            changingFields = true;
            MT_Form.Text = CB_Form.SelectedIndex.ToString();
            changingFields = false;

            UpdateSprite();
        }
        private void UpdateHaXForm(object sender, EventArgs e)
        {
            if (changingFields)
                return;
            changingFields = true;
            int form = pkm.AltForm = Util.ToInt32(MT_Form.Text);
            CB_Form.SelectedIndex = CB_Form.Items.Count > form ? form : -1;
            changingFields = false;

            UpdateSprite();
        }
        private void UpdatePP(object sender, EventArgs e)
        {
            ComboBox[] cbs = { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
            ComboBox[] pps = { CB_PPu1, CB_PPu2, CB_PPu3, CB_PPu4 };
            MaskedTextBox[] tbs = { TB_PP1, TB_PP2, TB_PP3, TB_PP4 };
            int index = Array.IndexOf(cbs, sender);
            if (index < 0)
                index = Array.IndexOf(pps, sender);
            if (index < 0)
                return;

            int move = WinFormsUtil.GetIndex(cbs[index]);
            int pp = pps[index].SelectedIndex;
            if (move == 0 && pp != 0)
            {
                pps[index].SelectedIndex = 0;
                return; // recursively triggers
            }
            tbs[index].Text = pkm.GetMovePP(move, pp).ToString();
        }
        private void UpdatePKRSstrain(object sender, EventArgs e)
        {
            // Change the PKRS Days to the legal bounds.
            int currentDuration = CB_PKRSDays.SelectedIndex;
            CB_PKRSDays.Items.Clear();
            foreach (int day in Enumerable.Range(0, CB_PKRSStrain.SelectedIndex % 4 + 2)) CB_PKRSDays.Items.Add(day);

            // Set the days back if they're legal, else set it to 1. (0 always passes).
            CB_PKRSDays.SelectedIndex = currentDuration < CB_PKRSDays.Items.Count ? currentDuration : 1;

            if (CB_PKRSStrain.SelectedIndex != 0) return;

            // Never Infected
            CB_PKRSDays.SelectedIndex = 0;
            CHK_Cured.Checked = false;
            CHK_Infected.Checked = false;
        }
        private void UpdatePKRSdays(object sender, EventArgs e)
        {
            if (CB_PKRSDays.SelectedIndex != 0) return;

            // If no days are selected
            if (CB_PKRSStrain.SelectedIndex == 0)
                CHK_Cured.Checked = CHK_Infected.Checked = false; // No Strain = Never Cured / Infected, triggers Strain update
            else CHK_Cured.Checked = true; // Any Strain = Cured
        }
        private void UpdatePKRSCured(object sender, EventArgs e)
        {
            if (!fieldsInitialized) return;
            // Cured PokeRus is toggled
            if (CHK_Cured.Checked)
            {
                // Has Had PokeRus
                Label_PKRSdays.Visible = CB_PKRSDays.Visible = false;
                CB_PKRSDays.SelectedIndex = 0;

                Label_PKRS.Visible = CB_PKRSStrain.Visible = true;
                CHK_Infected.Checked = true;

                // If we're cured we have to have a strain infection.
                if (CB_PKRSStrain.SelectedIndex == 0)
                    CB_PKRSStrain.SelectedIndex = 1;
            }
            else if (!CHK_Infected.Checked)
            {
                // Not Infected, Disable the other
                Label_PKRS.Visible = CB_PKRSStrain.Visible = false;
                CB_PKRSStrain.SelectedIndex = 0;
            }
            else
            {
                // Still Infected for a duration
                Label_PKRSdays.Visible = CB_PKRSDays.Visible = true;
                CB_PKRSDays.SelectedValue = 1;
            }
            // if not cured yet, days > 0
            if (!CHK_Cured.Checked && CHK_Infected.Checked && CB_PKRSDays.SelectedIndex == 0)
                CB_PKRSDays.SelectedIndex++;

            SetMarkings();
        }
        private void UpdatePKRSInfected(object sender, EventArgs e)
        {
            if (!fieldsInitialized) return;
            if (CHK_Cured.Checked && !CHK_Infected.Checked) { CHK_Cured.Checked = false; return; }
            if (CHK_Cured.Checked) return;
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked;
            if (!CHK_Infected.Checked) { CB_PKRSStrain.SelectedIndex = 0; CB_PKRSDays.SelectedIndex = 0; Label_PKRSdays.Visible = CB_PKRSDays.Visible = false; }
            else if (CB_PKRSStrain.SelectedIndex == 0) { CB_PKRSStrain.SelectedIndex = 1; Label_PKRSdays.Visible = CB_PKRSDays.Visible = true; UpdatePKRSCured(sender, e); }

            // if not cured yet, days > 0
            if (CHK_Infected.Checked && CB_PKRSDays.SelectedIndex == 0) CB_PKRSDays.SelectedIndex++;
        }
        private void UpdateCountry(object sender, EventArgs e)
        {
            if (WinFormsUtil.GetIndex(sender as ComboBox) > 0)
                SetCountrySubRegion(CB_SubRegion, "sr_" + WinFormsUtil.GetIndex(sender as ComboBox).ToString("000"));
        }
        private void UpdateSpecies(object sender, EventArgs e)
        {
            // Get Species dependent information
            if (fieldsLoaded)
                pkm.Species = WinFormsUtil.GetIndex(CB_Species);
            SetAbilityList();
            SetForms();
            UpdateForm(null, null);

            if (!fieldsLoaded)
                return;

            // Recalculate EXP for Given Level
            uint EXP = PKX.GetEXP(pkm.CurrentLevel, pkm.Species);
            TB_EXP.Text = EXP.ToString();

            // Check for Gender Changes
            UpdateGender();

            // If species changes and no nickname, set the new name == speciesName.
            if (!CHK_Nicknamed.Checked)
                UpdateNickname(sender, e);

            UpdateLegality();
        }
        private void UpdateOriginGame(object sender, EventArgs e)
        {
            GameVersion Version = (GameVersion)WinFormsUtil.GetIndex(CB_GameOrigin);

            // check if differs
            GameVersion newTrack = GameUtil.GetMetLocationVersionGroup(Version);
            if (newTrack != origintrack)
            {
                var met_list = GameInfo.GetLocationList(Version, pkm.Format, egg: false);
                CB_MetLocation.DisplayMember = "Text";
                CB_MetLocation.ValueMember = "Value";
                CB_MetLocation.DataSource = new BindingSource(met_list, null);

                int metLoc = 0; // transporter or pal park for past gen pkm
                switch (newTrack)
                {
                    case GameVersion.GO: metLoc = 30012; break;
                    case GameVersion.RBY: metLoc = 30013; break;
                }
                if (metLoc != 0)
                    CB_MetLocation.SelectedValue = metLoc;
                else
                    CB_MetLocation.SelectedIndex = metLoc;

                var egg_list = GameInfo.GetLocationList(Version, pkm.Format, egg: true);
                CB_EggLocation.DisplayMember = "Text";
                CB_EggLocation.ValueMember = "Value";
                CB_EggLocation.DataSource = new BindingSource(egg_list, null);
                CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none

                origintrack = newTrack;

                // Stretch C/XD met location dropdowns
                int width = CB_EggLocation.DropDownWidth;
                if (Version == GameVersion.CXD && pkm.Format == 3)
                    width = 2 * width;
                CB_MetLocation.DropDownWidth = width;
            }

            // Visibility logic for Gen 4 encounter type; only show for Gen 4 Pokemon.
            if (pkm.Format >= 4)
            {
                bool g4 = Version >= GameVersion.HG && Version <= GameVersion.Pt;
                if ((int)Version == 9) // invalid
                    g4 = false;
                CB_EncounterType.Visible = Label_EncounterType.Visible = g4 && pkm.Format < 7;
                if (!g4)
                    CB_EncounterType.SelectedValue = 0;
            }

            if (!fieldsLoaded)
                return;
            pkm.Version = (int)Version;
            SetMarkings(); // Set/Remove KB marking
            UpdateLegality();
        }
        private void UpdateExtraByteValue(object sender, EventArgs e)
        {
            if (CB_ExtraBytes.Items.Count == 0)
                return;
            // Changed Extra Byte's Value
            if (Util.ToInt32(((MaskedTextBox)sender).Text) > byte.MaxValue)
                ((MaskedTextBox)sender).Text = "255";

            int value = Util.ToInt32(TB_ExtraByte.Text);
            int offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
            pkm.Data[offset] = (byte)value;
        }
        private void UpdateExtraByteIndex(object sender, EventArgs e)
        {
            if (CB_ExtraBytes.Items.Count == 0)
                return;
            // Byte changed, need to refresh the Text box for the byte's value.
            TB_ExtraByte.Text = pkm.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
        }
        private void UpdateNatureModification(object sender, EventArgs e)
        {
            if (sender != CB_Nature) return;
            int nature = WinFormsUtil.GetIndex(CB_Nature);
            int incr = nature / 5;
            int decr = nature % 5;

            Label[] labarray = { Label_ATK, Label_DEF, Label_SPE, Label_SPA, Label_SPD };
            // Reset Label Colors
            foreach (Label label in labarray)
                label.ResetForeColor();

            // Set Colored StatLabels only if Nature isn't Neutral
            NatureTip.SetToolTip(CB_Nature,
                incr != decr
                    ? $"+{labarray[incr].Text} / -{labarray[decr].Text}".Replace(":", "")
                    : "-/-");
        }
        private void UpdateIsNicknamed(object sender, EventArgs e)
        {
            if (!fieldsLoaded)
                return;

            pkm.Nickname = TB_Nickname.Text;
            if (CHK_Nicknamed.Checked)
                return;

            int species = WinFormsUtil.GetIndex(CB_Species);
            if (species < 1 || species > pkm.MaxSpeciesID)
                return;

            if (CHK_IsEgg.Checked)
                species = 0; // get the egg name.

            if (PKX.IsNicknamedAnyLanguage(species, TB_Nickname.Text, pkm.Format))
                CHK_Nicknamed.Checked = true;
        }
        private void UpdateNickname(object sender, EventArgs e)
        {
            if (sender == Label_Species)
            {
                switch (ModifierKeys)
                {
                    case Keys.Control: RequestShowdownImport?.Invoke(sender, e); return;
                    case Keys.Alt: RequestShowdownExport?.Invoke(sender, e); return;
                    default:
                        if (pkm is PK1 pk1)
                            pk1.Catch_Rate = pk1.PersonalInfo.CatchRate;
                        return;
                }
            }

            int lang = WinFormsUtil.GetIndex(CB_Language);

            if (!fieldsInitialized || CHK_Nicknamed.Checked)
                return;

            // Fetch Current Species and set it as Nickname Text
            int species = WinFormsUtil.GetIndex(CB_Species);
            if (species < 1 || species > pkm.MaxSpeciesID)
            { TB_Nickname.Text = ""; return; }

            if (CHK_IsEgg.Checked)
                species = 0; // get the egg name.

            // If name is that of another language, don't replace the nickname
            if (sender != CB_Language && species != 0 && !PKX.IsNicknamedAnyLanguage(species, TB_Nickname.Text, pkm.Format))
                return;

            TB_Nickname.Text = PKX.GetSpeciesNameGeneration(species, lang, pkm.Format);
            if (pkm.Format == 1)
                ((PK1)pkm).SetNotNicknamed();
            if (pkm.Format == 2)
                ((PK2)pkm).SetNotNicknamed();
        }
        private void UpdateNicknameClick(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox ?? TB_Nickname;
            // Special Character Form
            if (ModifierKeys != Keys.Control)
                return;

            var SAV = RequestSaveFile;
            if (SAV == null) // form did not provide the needed info
                return;

            if (tb == TB_Nickname)
            {
                pkm.Nickname = tb.Text;
                var d = new TrashEditor(tb, pkm.Nickname_Trash, SAV);
                d.ShowDialog();
                tb.Text = d.FinalString;
                pkm.Nickname_Trash = d.FinalBytes;
            }
            else if (tb == TB_OT)
            {
                pkm.OT_Name = tb.Text;
                var d = new TrashEditor(tb, pkm.OT_Trash, SAV);
                d.ShowDialog();
                tb.Text = d.FinalString;
                pkm.OT_Trash = d.FinalBytes;
            }
            else if (tb == TB_OTt2)
            {
                pkm.HT_Name = tb.Text;
                var d = new TrashEditor(tb, pkm.HT_Trash, SAV);
                d.ShowDialog();
                tb.Text = d.FinalString;
                pkm.HT_Trash = d.FinalBytes;
            }
        }
        private void UpdateNotOT(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TB_OTt2.Text))
            {
                ClickGT(GB_OT, null); // Switch CT over to OT.
                Label_CTGender.Text = "";
                TB_Friendship.Text = pkm.CurrentFriendship.ToString();
            }
            else if (string.IsNullOrWhiteSpace(Label_CTGender.Text))
                Label_CTGender.Text = gendersymbols[0];
        }
        private void UpdateIsEgg(object sender, EventArgs e)
        {
            // Display hatch counter if it is an egg, Display Friendship if it is not.
            Label_HatchCounter.Visible = CHK_IsEgg.Checked && pkm.Format > 1;
            Label_Friendship.Visible = !CHK_IsEgg.Checked && pkm.Format > 1;

            if (!fieldsLoaded)
                return;

            pkm.IsEgg = CHK_IsEgg.Checked;
            if (CHK_IsEgg.Checked)
            {
                TB_Friendship.Text = "1";

                // If we are an egg, it won't have a met location.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CAL_MetDate.Value = new DateTime(2000, 01, 01);

                // if egg wasn't originally obtained by OT => Link Trade, else => None
                bool isTraded = false;
                var SAV = SaveFileRequested?.Invoke(this, e);
                if (SAV != null)
                    isTraded = SAV.OT != TB_OT.Text || SAV.TID != Util.ToInt32(TB_TID.Text) || SAV.SID != Util.ToInt32(TB_SID.Text);
                CB_MetLocation.SelectedIndex = isTraded ? 2 : 0;

                if (!CHK_Nicknamed.Checked)
                {
                    TB_Nickname.Text = PKX.GetSpeciesNameGeneration(0, WinFormsUtil.GetIndex(CB_Language), pkm.Format);
                    if (pkm.Format != 4) // eggs in gen4 do not have nickname flag
                        CHK_Nicknamed.Checked = true;
                }
            }
            else // Not Egg
            {
                if (!CHK_Nicknamed.Checked)
                    UpdateNickname(null, null);

                TB_Friendship.Text = pkm.PersonalInfo.BaseFriendship.ToString();

                if (CB_EggLocation.SelectedIndex == 0)
                {
                    CAL_EggDate.Value = new DateTime(2000, 01, 01);
                    CHK_AsEgg.Checked = false;
                    GB_EggConditions.Enabled = false;
                }

                if (TB_Nickname.Text == PKX.GetSpeciesNameGeneration(0, WinFormsUtil.GetIndex(CB_Language), pkm.Format))
                    CHK_Nicknamed.Checked = false;
            }

            UpdateNickname(null, null);
            UpdateSprite();
        }
        private void UpdateMetAsEgg(object sender, EventArgs e)
        {
            GB_EggConditions.Enabled = CHK_AsEgg.Checked;
            if (CHK_AsEgg.Checked)
            {
                if (!fieldsLoaded)
                    return;

                CAL_EggDate.Value = DateTime.Now;
                CB_EggLocation.SelectedIndex = 1;
                return;
            }
            // Remove egg met data
            CHK_IsEgg.Checked = false;
            CAL_EggDate.Value = new DateTime(2000, 01, 01);
            CB_EggLocation.SelectedValue = 0;

            UpdateLegality();
        }
        private void UpdateShinyPID(object sender, EventArgs e)
        {
            var ShinyPID = pkm.Format <= 2 || ModifierKeys != Keys.Control;
            UpdateShiny(ShinyPID);
        }
        private void UpdateShiny(bool PID)
        {
            pkm.TID = Util.ToInt32(TB_TID.Text);
            pkm.SID = Util.ToInt32(TB_SID.Text);
            pkm.PID = Util.GetHexValue(TB_PID.Text);
            pkm.Nature = WinFormsUtil.GetIndex(CB_Nature);
            pkm.Gender = PKX.GetGender(Label_Gender.Text);
            pkm.AltForm = CB_Form.SelectedIndex;
            pkm.Version = WinFormsUtil.GetIndex(CB_GameOrigin);

            if (pkm.Format > 2)
            { 
                if (PID)
                {
                    pkm.SetShinyPID();
                    TB_PID.Text = pkm.PID.ToString("X8");

                    if (pkm.GenNumber < 6 && TB_EC.Visible)
                        TB_EC.Text = TB_PID.Text;
                }
                else
                {
                    pkm.SetShinySID();
                    TB_SID.Text = pkm.SID.ToString();
                }
            }
            else
            {
                // IVs determine shininess
                // All 10IV except for one where (IV & 2 == 2) [gen specific]
                int[] and2 = { 2, 3, 6, 7, 10, 11, 14, 15 };
                int randIV = and2[Util.Rand32() % and2.Length];
                if (pkm.Format == 1)
                {
                    TB_ATKIV.Text = "10"; // an attempt was made
                    TB_DEFIV.Text = randIV.ToString();
                }
                else // pkm.Format == 2
                {
                    TB_ATKIV.Text = randIV.ToString();
                    TB_DEFIV.Text = "10";
                }
                TB_SPEIV.Text = "10";
                TB_SPAIV.Text = "10";
                UpdateIVs(null, null);
            }

            SetIsShiny(null);
            UpdatePreviewSprite?.Invoke(this, null);
            UpdateLegality();
        }
        private void UpdateTSV(object sender, EventArgs e)
        {
            if (pkm.Format < 6)
                return;

            string IDstr = $"TSV: {pkm.TSV:d4}";
            if (pkm.Format > 6)
                IDstr += Environment.NewLine + $"G7TID: {pkm.TrainerID7:d6}";

            Tip1.SetToolTip(TB_TID, IDstr);
            Tip2.SetToolTip(TB_SID, IDstr);

            pkm.PID = Util.GetHexValue(TB_PID.Text);
            Tip3.SetToolTip(TB_PID, $"PSV: {pkm.PSV:d4}");
        }
        private void Update_ID(object sender, EventArgs e)
        {
            // Trim out nonhex characters
            TB_PID.Text = Util.GetHexValue(TB_PID.Text).ToString("X8");
            TB_EC.Text = Util.GetHexValue(TB_EC.Text).ToString("X8");

            // Max TID/SID is 65535
            if (Util.ToUInt32(TB_TID.Text) > ushort.MaxValue) TB_TID.Text = "65535";
            if (Util.ToUInt32(TB_SID.Text) > ushort.MaxValue) TB_SID.Text = "65535";

            SetIsShiny(sender);
            UpdateSprite();
            UpdateIVs(null, null);   // If the EC is changed, EC%6 (Characteristic) might be changed. 
            TB_PID.Select(60, 0);   // position cursor at end of field
            if (pkm.Format <= 4 && fieldsLoaded)
            {
                fieldsLoaded = false;
                pkm.PID = Util.GetHexValue(TB_PID.Text);
                CB_Nature.SelectedValue = pkm.Nature;
                Label_Gender.Text = gendersymbols[pkm.Gender];
                Label_Gender.ForeColor = GetGenderColor(pkm.Gender);
                fieldsLoaded = true;
            }
        }
        private void UpdateShadowID(object sender, EventArgs e)
        {
            if (!fieldsLoaded)
                return;
            FLP_Purification.Visible = NUD_ShadowID.Value > 0;
        }
        private void UpdatePurification(object sender, EventArgs e)
        {
            if (!fieldsLoaded)
                return;
            fieldsLoaded = false;
            CHK_Shadow.Checked = NUD_Purification.Value > 0;
            fieldsLoaded = true;
        }
        private void UpdateShadowCHK(object sender, EventArgs e)
        {
            if (!fieldsLoaded)
                return;
            fieldsLoaded = false;
            NUD_Purification.Value = CHK_Shadow.Checked ? NUD_Purification.Maximum : 0;
            fieldsLoaded = true;
        }
        private void ValidateComboBox(object sender)
        {
            if (!fieldsInitialized)
                return;
            ComboBox cb = sender as ComboBox;
            if (cb == null)
                return;

            if (cb.Text == "" && cb.Items.Count > 0)
            { cb.SelectedIndex = 0; return; }
            if (cb.SelectedValue == null)
                cb.BackColor = Color.DarkSalmon;
            else
                cb.ResetBackColor();
        }
        private void ValidateComboBox(object sender, CancelEventArgs e)
        {
            if (!(sender is ComboBox))
                return;

            ValidateComboBox(sender);
            UpdateSprite();
        }
        private void ValidateComboBox2(object sender, EventArgs e)
        {
            if (!fieldsInitialized)
                return;
            ValidateComboBox(sender, null);
            if (fieldsLoaded)
            {
                if (sender == CB_Ability && pkm.Format >= 6)
                    TB_AbilityNumber.Text = (1 << CB_Ability.SelectedIndex).ToString();
                if (sender == CB_Ability && pkm.Format <= 5 && CB_Ability.SelectedIndex < 2) // not hidden
                    UpdateRandomPID(sender, e);
                if (sender == CB_Nature && pkm.Format <= 4)
                {
                    pkm.Nature = CB_Nature.SelectedIndex;
                    UpdateRandomPID(sender, e);
                }
                if (sender == CB_HeldItem || sender == CB_Ability)
                    UpdateLegality();
            }
            UpdateNatureModification(sender, null);
            UpdateIVs(null, null); // updating Nature will trigger stats to update as well
        }
        private void ValidateMove(object sender, EventArgs e)
        {
            if (!fieldsInitialized)
                return;
            ValidateComboBox(sender);
            if (!fieldsLoaded)
                return;

            if (new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 }.Contains(sender)) // Move
                UpdatePP(sender, e);

            // Legality
            pkm.Moves = new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 }.Select(WinFormsUtil.GetIndex).ToArray();
            pkm.RelearnMoves = new[] { CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 }.Select(WinFormsUtil.GetIndex).ToArray();
            UpdateLegality(skipMoveRepop: true);
        }
        private void ValidateMovePaint(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var i = (ComboItem)(sender as ComboBox).Items[e.Index];
            var moves = Legality.AllSuggestedMovesAndRelearn;
            bool vm = moves != null && moves.Contains(i.Value) && !HaX;

            bool current = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Brush tBrush = current ? SystemBrushes.HighlightText : new SolidBrush(e.ForeColor);
            Brush brush = current ? SystemBrushes.Highlight : vm ? Brushes.PaleGreen : new SolidBrush(e.BackColor);

            e.Graphics.FillRectangle(brush, e.Bounds);
            e.Graphics.DrawString(i.Text, e.Font, tBrush, e.Bounds, StringFormat.GenericDefault);
            if (current) return;
            tBrush.Dispose();
            if (!vm)
                brush.Dispose();
        }
        private void ValidateLocation(object sender, EventArgs e)
        {
            ValidateComboBox(sender);
            if (!fieldsLoaded)
                return;

            pkm.Met_Location = WinFormsUtil.GetIndex(CB_MetLocation);
            pkm.Egg_Location = WinFormsUtil.GetIndex(CB_EggLocation);
            UpdateLegality();
        }

        // Secondary Windows for Ribbons/Amie/Memories
        private void OpenRibbons(object sender, EventArgs e)
        {
            new RibbonEditor(pkm).ShowDialog();
        }
        private void OpenMedals(object sender, EventArgs e)
        {
            new SuperTrainingEditor(pkm).ShowDialog();
        }
        private void OpenHistory(object sender, EventArgs e)
        {
            // Write back current values
            pkm.HT_Name = TB_OTt2.Text;
            pkm.OT_Name = TB_OT.Text;
            pkm.IsEgg = CHK_IsEgg.Checked;
            pkm.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);
            new MemoryAmie(pkm).ShowDialog();
            TB_Friendship.Text = pkm.CurrentFriendship.ToString();
        }

        /// <summary>
        /// Refreshes the interface for the current PKM format.
        /// </summary>
        public bool ToggleInterface(SaveFile sav, PKM pk)
        {
            if (pk.GetType() != sav.PKMType || pkm.Format < 3)
                pk = sav.BlankPKM;
            pkm = pk;

            ToggleInterface(pkm.Format);
            ToggleInterface(pkm.GetType());

            return FinalizeInterface(sav);
        }
        private void ToggleInterface(Type t)
        {
            FLP_Purification.Visible = FLP_ShadowID.Visible = t == typeof(XK3) || t == typeof(CK3);
        }
        private void ToggleInterface(int gen)
        {
            Tip1.RemoveAll(); Tip2.RemoveAll(); Tip3.RemoveAll(); // TSV/PSV

            FLP_Country.Visible = FLP_SubRegion.Visible = FLP_3DSRegion.Visible = gen >= 6;
            Label_EncryptionConstant.Visible = BTN_RerollEC.Visible = TB_EC.Visible = gen >= 6;
            GB_nOT.Visible = GB_RelearnMoves.Visible = BTN_Medals.Visible = BTN_History.Visible = gen >= 6;

            PB_MarkPentagon.Visible = gen >= 6;
            PB_MarkAlola.Visible = PB_MarkVC.Visible = PB_MarkHorohoro.Visible = gen >= 7;

            FLP_NSparkle.Visible = L_NSparkle.Visible = CHK_NSparkle.Visible = gen == 5;

            CB_Form.Visible = Label_Form.Visible = CHK_AsEgg.Visible = GB_EggConditions.Visible = PB_Mark5.Visible = PB_Mark6.Visible = gen >= 4;
            FLP_ShinyLeaf.Visible = L_ShinyLeaf.Visible = ShinyLeaf.Visible = gen == 4;

            DEV_Ability.Enabled = DEV_Ability.Visible = gen > 3 && HaX;
            CB_Ability.Visible = !DEV_Ability.Enabled && gen >= 3;
            FLP_Nature.Visible = gen >= 3;
            FLP_Ability.Visible = gen >= 3;
            FLP_Language.Visible = gen >= 3;
            GB_ExtraBytes.Visible = GB_ExtraBytes.Enabled = gen >= 3;
            GB_Markings.Visible = gen >= 3;
            BTN_Ribbons.Visible = gen >= 3;
            CB_HPType.Enabled = CB_Form.Enabled = gen >= 3;
            BTN_RerollPID.Visible = Label_PID.Visible = TB_PID.Visible = Label_SID.Visible = TB_SID.Visible = gen >= 3;

            FLP_FriendshipForm.Visible = gen >= 2;
            FLP_HeldItem.Visible = gen >= 2;
            CHK_IsEgg.Visible = Label_Gender.Visible = gen >= 2;
            FLP_PKRS.Visible = FLP_EggPKRSRight.Visible = gen >= 2;
            Label_OTGender.Visible = gen >= 2;

            // HaX override, needs to be after DEV_Ability enabled assignment.
            TB_AbilityNumber.Visible = gen >= 6 && DEV_Ability.Enabled;

            // Met Tab
            FLP_MetDate.Visible = gen >= 4;
            FLP_Fateful.Visible = FLP_Ball.Visible = FLP_OriginGame.Visible = gen >= 3;
            FLP_MetLocation.Visible = FLP_MetLevel.Visible = gen >= 2;
            FLP_TimeOfDay.Visible = gen == 2;

            // Stats
            FLP_StatsTotal.Visible = gen >= 3;
            FLP_Characteristic.Visible = gen >= 3;
            FLP_HPType.Visible = gen >= 2;

            Contest.ToggleInterface(gen);

            ToggleStats(gen);
            CenterSubEditors();
        }
        private void ToggleStats(int gen)
        {
            if (pkm.Format == 1)
            {
                FLP_SpD.Visible = false;
                Label_SPA.Visible = false;
                Label_SPC.Visible = true;
                TB_HPIV.Enabled = false;
                MaskedTextBox[] evControls = {TB_SPAEV, TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPEEV, TB_SPDEV};
                foreach (var ctrl in evControls)
                {
                    ctrl.Mask = "00000";
                    ctrl.Size = Stat_HP.Size;
                }
            }
            else if (gen == 2)
            {
                FLP_SpD.Visible = true;
                Label_SPA.Visible = true;
                Label_SPC.Visible = false;
                TB_SPDEV.Enabled = TB_SPDIV.Enabled = false;
                TB_HPIV.Enabled = false;
                MaskedTextBox[] evControls = {TB_SPAEV, TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPEEV, TB_SPDEV};
                foreach (var ctrl in evControls)
                {
                    ctrl.Mask = "00000";
                    ctrl.Size = Stat_HP.Size;
                }
            }
            else
            {
                FLP_SpD.Visible = true;
                Label_SPA.Visible = true;
                Label_SPC.Visible = false;
                TB_SPDEV.Enabled = TB_SPDIV.Enabled = true;
                TB_HPIV.Enabled = true;
                MaskedTextBox[] evControls = {TB_SPAEV, TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPEEV, TB_SPDEV};
                foreach (var ctrl in evControls)
                {
                    ctrl.Mask = "000";
                    ctrl.Size = TB_ExtraByte.Size;
                }
            }
        }
        private bool FinalizeInterface(SaveFile sav)
        {
            bool init = fieldsInitialized;
            fieldsInitialized = fieldsLoaded = false;

            bool TranslationRequired = false;
            PopulateFilteredDataSources(sav);
            PopulateFields(pkm);
            fieldsInitialized |= init;

            // SAV Specific Limits
            TB_OT.MaxLength = pkm.OTLength;
            TB_OTt2.MaxLength = pkm.OTLength;
            TB_Nickname.MaxLength = pkm.NickLength;

            // Hide Unused Tabs
            if (pkm.Format == 1 && tabMain.TabPages.Contains(Tab_Met))
                tabMain.TabPages.Remove(Tab_Met);
            else if (pkm.Format != 1 && !tabMain.TabPages.Contains(Tab_Met))
            {
                tabMain.TabPages.Insert(1, Tab_Met);
                TranslationRequired = true;
            }

            // Common HaX Interface
            CHK_HackedStats.Enabled = CHK_HackedStats.Visible = MT_Level.Enabled = MT_Level.Visible = MT_Form.Enabled = MT_Form.Visible = HaX;
            TB_Level.Visible = !HaX;

            // Setup PKM Preparation/Extra Bytes
            SetPKMFormatMode(pkm.Format);

            // pk2 save files do not have an Origin Game stored. Prompt the met location list to update.
            if (pkm.Format == 2)
                UpdateOriginGame(null, null);
            return TranslationRequired;
        }
        private void CenterSubEditors()
        {
            // Recenter PKM SubEditors
            FLP_PKMEditors.Location = new Point((Tab_OTMisc.Width - FLP_PKMEditors.Width) / 2, FLP_PKMEditors.Location.Y);
        }
        
        // Loading Setup
        public void TemplateFields(PKM template)
        {
            if (template != null)
            {
                PopulateFields(template);
                LastData = null;
                return;
            }
            if (CB_GameOrigin.Items.Count > 0)
                CB_GameOrigin.SelectedIndex = 0;
            CB_Move1.SelectedValue = 1;
            TB_OT.Text = "PKHeX";
            TB_TID.Text = 12345.ToString();
            TB_SID.Text = 54321.ToString();
            int curlang = GameInfo.Language();
            CB_Language.SelectedIndex = curlang > CB_Language.Items.Count - 1 ? 1 : curlang;
            CB_Ball.SelectedIndex = Math.Min(0, CB_Ball.Items.Count - 1);
            CB_Country.SelectedIndex = Math.Min(0, CB_Country.Items.Count - 1);
            CAL_MetDate.Value = CAL_EggDate.Value = DateTime.Today;
            CB_Species.SelectedValue = pkm.MaxSpeciesID;
            CHK_Nicknamed.Checked = false;
            LastData = null;
        }
        public void EnableDragDrop(DragEventHandler enter, DragEventHandler drop)
        {
            AllowDrop = true;
            DragDrop += drop;
            foreach (TabPage tab in tabMain.TabPages)
            {
                tab.AllowDrop = true;
                tab.DragEnter += enter;
                tab.DragDrop += drop;
            }
        }
        public void LoadShowdownSet(ShowdownSet Set)
        {
            CB_Species.SelectedValue = Set.Species;
            CHK_Nicknamed.Checked = Set.Nickname != null;
            if (Set.Nickname != null)
                TB_Nickname.Text = Set.Nickname;
            if (Set.Gender != null)
            {
                int Gender = PKX.GetGender(Set.Gender);
                Label_Gender.Text = gendersymbols[Gender];
                Label_Gender.ForeColor = GetGenderColor(Gender);
            }

            // Set Form
            string[] formStrings = PKX.GetFormList(Set.Species,
                Util.GetTypesList("en"),
                Util.GetFormsList("en"), gendersymbols, pkm.Format);
            int form = 0;
            for (int i = 0; i < formStrings.Length; i++)
                if (formStrings[i].Contains(Set.Form ?? ""))
                { form = i; break; }
            CB_Form.SelectedIndex = Math.Min(CB_Form.Items.Count - 1, form);

            // Set Ability and Moves
            CB_Ability.SelectedIndex = Math.Max(0, Array.IndexOf(pkm.PersonalInfo.Abilities, Set.Ability));
            ComboBox[] m = { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
            for (int i = 0; i < 4; i++) m[i].SelectedValue = Set.Moves[i];

            // Set Item and Nature
            CB_HeldItem.SelectedValue = Set.HeldItem < 0 ? 0 : Set.HeldItem;
            CB_Nature.SelectedValue = Set.Nature < 0 ? 0 : Set.Nature;

            // Set IVs
            TB_HPIV.Text = Set.IVs[0].ToString();
            TB_ATKIV.Text = Set.IVs[1].ToString();
            TB_DEFIV.Text = Set.IVs[2].ToString();
            TB_SPAIV.Text = Set.IVs[4].ToString();
            TB_SPDIV.Text = Set.IVs[5].ToString();
            TB_SPEIV.Text = Set.IVs[3].ToString();

            // Set EVs
            TB_HPEV.Text = Set.EVs[0].ToString();
            TB_ATKEV.Text = Set.EVs[1].ToString();
            TB_DEFEV.Text = Set.EVs[2].ToString();
            TB_SPAEV.Text = Set.EVs[4].ToString();
            TB_SPDEV.Text = Set.EVs[5].ToString();
            TB_SPEEV.Text = Set.EVs[3].ToString();

            // Set Level and Friendship
            TB_Level.Text = Set.Level.ToString();
            TB_Friendship.Text = Set.Friendship.ToString();

            // Reset IV/EVs
            UpdateRandomPID(null, null);
            UpdateRandomEC(null, null);
            ComboBox[] p = { CB_PPu1, CB_PPu2, CB_PPu3, CB_PPu4 };
            for (int i = 0; i < 4; i++)
                p[i].SelectedIndex = m[i].SelectedIndex != 0 ? 3 : 0; // max PP

            if (Set.Shiny) UpdateShiny(true);
            pkm = PreparePKM();
            UpdateLegality();

            if (Legality.Info.Relearn.Any(z => !z.Valid))
                SetSuggestedRelearnMoves(silent: true);
        }
        public void ChangeLanguage(SaveFile sav, PKM pk)
        {
            // Force an update to the met locations
            origintrack = GameVersion.Unknown;

            bool alreadyInit = fieldsInitialized;
            fieldsInitialized = false;
            InitializeLanguage(sav);
            CenterSubEditors();
            PopulateFields(pk); // put data back in form
            fieldsInitialized |= alreadyInit;
        }
        public void FlickerInterface()
        {
            tabMain.SelectedTab = Tab_Met; // parent tab of CB_GameOrigin
            tabMain.SelectedTab = Tab_Main; // first tab
        }

        private void InitializeLanguage(SaveFile SAV)
        {
            ComboBox[] cbs =
            {
                CB_Country, CB_SubRegion, CB_3DSReg, CB_Language, CB_Ball, CB_HeldItem, CB_Species, DEV_Ability,
                CB_Nature, CB_EncounterType, CB_GameOrigin, CB_HPType
            };
            foreach (var cb in cbs) { cb.DisplayMember = "Text"; cb.ValueMember = "Value"; }

            // Set the various ComboBox DataSources up with their allowed entries
            SetCountrySubRegion(CB_Country, "countries");
            CB_3DSReg.DataSource = Util.GetUnsortedCBList("regions3ds");

            GameInfo.InitializeDataSources(GameInfo.Strings);

            CB_EncounterType.DataSource = Util.GetCBList(GameInfo.Strings.encountertypelist, new[] { 0 }, Legal.Gen4EncounterTypes);
            CB_HPType.DataSource = Util.GetCBList(GameInfo.Strings.types.Skip(1).Take(16).ToArray(), null);
            CB_Nature.DataSource = new BindingSource(GameInfo.NatureDataSource, null);

            PopulateFilteredDataSources(SAV);
        }
        private void PopulateFilteredDataSources(SaveFile SAV)
        {
            GameInfo.SetItemDataSource(HaX, pkm.MaxItemID, SAV.HeldItems, pkm.Format, SAV.Version, GameInfo.Strings);
            if (pkm.Format > 1)
                CB_HeldItem.DataSource = new BindingSource(GameInfo.ItemDataSource.Where(i => i.Value <= pkm.MaxItemID).ToList(), null);

            var languages = Util.GetUnsortedCBList("languages");
            if (pkm.Format < 7)
                languages = languages.Where(l => l.Value <= 8).ToList(); // Korean
            CB_Language.DataSource = languages;

            CB_Ball.DataSource = new BindingSource(GameInfo.BallDataSource.Where(b => b.Value <= pkm.MaxBallID).ToList(), null);
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(s => s.Value <= pkm.MaxSpeciesID).ToList(), null);
            DEV_Ability.DataSource = new BindingSource(GameInfo.AbilityDataSource.Where(a => a.Value <= pkm.MaxAbilityID).ToList(), null);
            CB_GameOrigin.DataSource = new BindingSource(GameInfo.VersionDataSource.Where(g => g.Value <= pkm.MaxGameID || pkm.Format >= 3 && g.Value == 15).ToList(), null);

            // Set the Move ComboBoxes too..
            GameInfo.MoveDataSource = (HaX ? GameInfo.HaXMoveDataSource : GameInfo.LegalMoveDataSource).Where(m => m.Value <= pkm.MaxMoveID).ToList(); // Filter Z-Moves if appropriate
            foreach (ComboBox cb in new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4, CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 })
            {
                cb.DisplayMember = "Text"; cb.ValueMember = "Value";
                cb.DataSource = new BindingSource(GameInfo.MoveDataSource, null);
            }
        }
    }
}
