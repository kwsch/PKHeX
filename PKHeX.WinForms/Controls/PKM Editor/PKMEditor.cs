using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using System.ComponentModel;
using PKHeX.Drawing.Properties;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public sealed partial class PKMEditor : UserControl, IMainEditor
    {
        public PKMEditor()
        {
            InitializeComponent();

            GB_OT.Click += ClickGT;
            GB_nOT.Click += ClickGT;
            GB_CurrentMoves.Click += ClickMoves;
            GB_RelearnMoves.Click += ClickMoves;

            TB_Nickname.Font = FontUtil.GetPKXFont();
            TB_OT.Font = (Font)TB_Nickname.Font.Clone();
            TB_OTt2.Font = (Font)TB_Nickname.Font.Clone();

            // Commonly reused Control arrays
            Moves = new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
            Relearn = new[] { CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 };
            PPUps = new[] { CB_PPu1, CB_PPu2, CB_PPu3, CB_PPu4 };
            MovePP = new[] { TB_PP1, TB_PP2, TB_PP3, TB_PP4 };
            Markings = new[] { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            ValidationRequired = Moves.Concat(Relearn).Concat(new[]
            {
                CB_Species, CB_Nature, CB_HeldItem, CB_Ability, // Main Tab
                CB_MetLocation, CB_EggLocation, CB_Ball, // Met Tab
                CB_StatNature,
            }).ToArray();
            relearnPB = new[] { PB_WarnRelearn1, PB_WarnRelearn2, PB_WarnRelearn3, PB_WarnRelearn4 };
            movePB = new[] { PB_WarnMove1, PB_WarnMove2, PB_WarnMove3, PB_WarnMove4 };

            foreach (var c in WinFormsUtil.GetAllControlsOfType<ComboBox>(this))
                c.KeyDown += WinFormsUtil.RemoveDropCB;

            Stats.MainEditor = this;
            LoadShowdownSet = LoadShowdownSetDefault;
            TID_Trainer.UpdatedID += Update_ID;
        }

        public void InitializeBinding()
        {
            ComboBox[] cbs =
            {
                CB_Nature, CB_StatNature,
                CB_Country, CB_SubRegion, CB_3DSReg, CB_Language, CB_Ball, CB_HeldItem, CB_Species, DEV_Ability,
                CB_EncounterType, CB_GameOrigin, CB_Ability, CB_MetLocation, CB_EggLocation, CB_Language, CB_HTLanguage,
            };
            foreach (var cb in cbs.Concat(Moves.Concat(Relearn)))
                cb.InitializeBinding();
        }

        private void UpdateStats()
        {
            Stats.UpdateStats();
            if (Entity is PB7)
                SizeCP.TryResetStats();
        }

        private void LoadPartyStats(PKM pk) => Stats.LoadPartyStats(pk);

        private void SavePartyStats(PKM pk)
        {
            Stats.SavePartyStats(pk);
            pk.Stat_Level = Util.ToInt32((HaX ? MT_Level : TB_Level).Text);
    }

        public PKM CurrentPKM { get => PreparePKM(); set => Entity = value; }
        public bool ModifyPKM { private get; set; } = true;
        private bool _hideSecret;

        public bool HideSecretValues
        {
            private get => _hideSecret;
            set
            {
                _hideSecret = value;
                var sav = RequestSaveFile;
                if (sav != null)
                    ToggleSecrets(_hideSecret, sav.Generation);
            }
        }

        public DrawConfig Draw { private get; set; }
        public bool Unicode { get; set; } = true;
        private bool _hax;
        public bool HaX { get => _hax; set => _hax = Stats.HaX = value; }
        public byte[] LastData { private get; set; }

        public PKM Data { get => Entity; set => Entity = value; }
        public PKM Entity { get; private set; }
        public bool FieldsLoaded { get; private set; }
        public bool ChangingFields { get; set; }

        private GameVersion origintrack;
        private Action GetFieldsfromPKM;
        private Func<PKM> GetPKMfromFields;
        private LegalityAnalysis Legality;
        private IReadOnlyList<string> gendersymbols = GameInfo.GenderSymbolUnicode;
        private readonly LegalMoveSource LegalMoveSource = new LegalMoveSource();

        public event EventHandler LegalityChanged;
        public event EventHandler UpdatePreviewSprite;
        public event EventHandler RequestShowdownImport;
        public event EventHandler RequestShowdownExport;
        public event ReturnSAVEventHandler SaveFileRequested;
        public delegate SaveFile ReturnSAVEventHandler(object sender, EventArgs e);

        private readonly PictureBox[] movePB, relearnPB;
        public SaveFile RequestSaveFile => SaveFileRequested?.Invoke(this, EventArgs.Empty);
        public bool PKMIsUnsaved => FieldsLoaded && LastData?.Any(b => b != 0) == true && !LastData.SequenceEqual(CurrentPKM.Data);

        private readonly ComboBox[] Moves, Relearn, ValidationRequired, PPUps;
        private readonly MaskedTextBox[] MovePP;
        private readonly PictureBox[] Markings;

        private bool forceValidation;

        public PKM PreparePKM(bool click = true)
        {
            if (click)
            {
                forceValidation = true;
                ValidateChildren();
                forceValidation = false;
            }

            var pk = GetPKMfromFields();
            LastData = pk.Data;
            return pk.Clone();
        }

        public bool EditsComplete
        {
            get
            {
                if (ModifierKeys == (Keys.Control | Keys.Shift | Keys.Alt))
                    return true; // Override

                var cb = Array.Find(ValidationRequired, c => c.BackColor == Draw.InvalidSelection && c.Items.Count != 0);
                if (cb != null)
                    tabMain.SelectedTab = WinFormsUtil.FindFirstControlOfType<TabPage>(cb);
                else if (!Stats.Valid)
                    tabMain.SelectedTab = Tab_Stats;
                else if (WinFormsUtil.GetIndex(CB_Species) == 0)
                    tabMain.SelectedTab = Tab_Main;
                else
                    return true;

                System.Media.SystemSounds.Exclamation.Play();
                return false;
            }
        }

        public void SetPKMFormatMode(int Format, PKM pk)
        {
            // Load Extra Byte List
            SetPKMFormatExtraBytes(pk);

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
                    GetFieldsfromPKM = PopulateFieldsPK3;
                    GetPKMfromFields = PreparePK3;
                    break;
                case 4:
                    GetFieldsfromPKM = PopulateFieldsPK4;
                    GetPKMfromFields = PreparePK4;
                    break;
                case 5:
                    GetFieldsfromPKM = PopulateFieldsPK5;
                    GetPKMfromFields = PreparePK5;
                    break;
                case 6:
                    GetFieldsfromPKM = PopulateFieldsPK6;
                    GetPKMfromFields = PreparePK6;
                    break;
                case 7:
                    switch (pk)
                    {
                        case PK7 _:
                            GetFieldsfromPKM = PopulateFieldsPK7;
                            GetPKMfromFields = PreparePK7;
                            break;

                        case PB7 _:
                            GetFieldsfromPKM = PopulateFieldsPB7;
                            GetPKMfromFields = PreparePB7;
                            break;
                    }
                    break;
                case 8:
                    GetFieldsfromPKM = PopulateFieldsPK8;
                    GetPKMfromFields = PreparePK8;
                    break;
            }
        }

        private void SetPKMFormatExtraBytes(PKM pk)
        {
            var extraBytes = pk.ExtraBytes;
            GB_ExtraBytes.Visible = GB_ExtraBytes.Enabled = extraBytes.Count != 0;
            CB_ExtraBytes.Items.Clear();
            foreach (var b in extraBytes)
                CB_ExtraBytes.Items.Add($"0x{b:X2}");
            if (GB_ExtraBytes.Enabled)
                CB_ExtraBytes.SelectedIndex = 0;
        }

        public void PopulateFields(PKM pk, bool focus = true, bool skipConversionCheck = false) => LoadFieldsFromPKM(pk, focus, skipConversionCheck);

        private void LoadFieldsFromPKM(PKM pk, bool focus = true, bool skipConversionCheck = true)
        {
            if (pk == null) { WinFormsUtil.Error(MsgPKMLoadNull); return; }
            if (focus)
                Tab_Main.Focus();

            if (!skipConversionCheck && !PKMConverter.TryMakePKMCompatible(pk, Entity, out string c, out pk))
            { WinFormsUtil.Alert(c); return; }

            FieldsLoaded = false;

            Entity = pk.Clone();

#if !DEBUG
            try { GetFieldsfromPKM(); }
            catch { }
#else
            GetFieldsfromPKM();
#endif

            Stats.UpdateIVs(null, EventArgs.Empty);
            UpdatePKRSInfected(null, EventArgs.Empty);
            UpdatePKRSCured(null, EventArgs.Empty);
            UpdateNatureModification(CB_StatNature);

            if (HaX) // Load original values from pk not pkm
            {
                MT_Level.Text = (pk.PartyStatsPresent ? pk.Stat_Level : Experience.GetLevel(pk.EXP, pk.PersonalInfo.EXPGrowth)).ToString();
                TB_EXP.Text = pk.EXP.ToString();
                MT_Form.Text = Math.Max(0, pk.AltForm).ToString();
                if (pk.PartyStatsPresent) // stats present
                    Stats.LoadPartyStats(pk);
            }
            FieldsLoaded = true;

            SetMarkings();
            UpdateLegality();
            UpdateSprite();
            LastData = PreparePKM()?.Data;
        }

        public void UpdateLegality(LegalityAnalysis la = null, bool skipMoveRepop = false)
        {
            if (!FieldsLoaded)
                return;

            Legality = la ?? new LegalityAnalysis(Entity, RequestSaveFile.Personal);
            if (!Legality.Parsed || HaX || Entity.Species == 0)
            {
                PB_WarnMove1.Visible = PB_WarnMove2.Visible = PB_WarnMove3.Visible = PB_WarnMove4.Visible =
                    PB_WarnRelearn1.Visible = PB_WarnRelearn2.Visible = PB_WarnRelearn3.Visible = PB_WarnRelearn4.Visible = false;
                LegalityChanged?.Invoke(Legality.Valid, EventArgs.Empty);
                return;
            }

            // Refresh Move Legality
            var moves = Entity.Moves;
            for (int i = 0; i < 4; i++)
            {
                bool invalid = !Legality.Info?.Moves[i]?.Valid ?? false;

                Bitmap img;
                if (invalid)
                    img = Resources.warn;
                else if (Entity.Format >= 8 && Legal.DummiedMoves_SWSH.Contains(moves[i]))
                    img = Resources.hint;
                else
                    img = null;
                movePB[i].Visible = true;
                movePB[i].Image = img;
            }

            if (Entity.Format >= 6)
            {
                for (int i = 0; i < 4; i++)
                    relearnPB[i].Visible = !Legality.Info?.Relearn[i]?.Valid ?? false;
            }

            if (skipMoveRepop)
                return;
            // Resort moves
            FieldsLoaded = false;
            LegalMoveSource.ReloadMoves(Legality.AllSuggestedMovesAndRelearn());
            FieldsLoaded = true;
            LegalityChanged?.Invoke(Legality.Valid, EventArgs.Empty);
        }

        public void UpdateUnicode(IReadOnlyList<string> symbols)
        {
            gendersymbols = symbols;
            if (!Unicode)
            {
                BTN_Shinytize.Text = Draw.ShinyDefault;
                TB_Nickname.Font = TB_OT.Font = TB_OTt2.Font = GB_OT.Font;
            }
            else
            {
                BTN_Shinytize.Text = Draw.ShinyUnicode;
                TB_Nickname.Font = TB_OT.Font = TB_OTt2.Font = FontUtil.GetPKXFont();
            }

            // Switch active gender labels to new if they are active.
            ReloadGender(Label_Gender, gendersymbols);
            ReloadGender(Label_OTGender, gendersymbols);
            ReloadGender(Label_CTGender, gendersymbols);
        }

        private static string ReloadGender(string text, IReadOnlyList<string> genders)
        {
            var index = PKX.GetGenderFromString(text);
            if (index >= 2)
                return text;
            return genders[index];
        }

        private static void ReloadGender(Label l, IReadOnlyList<string> genders) => l.Text = ReloadGender(l.Text, genders);

        private void UpdateSprite()
        {
            if (FieldsLoaded && !forceValidation)
                UpdatePreviewSprite?.Invoke(this, EventArgs.Empty);
        }

        // General Use Functions //
        private void SetDetailsOT(ITrainerInfo tr)
        {
            if (string.IsNullOrWhiteSpace(tr.OT))
                return;

            // Get Save Information
            TB_OT.Text = tr.OT;
            Label_OTGender.Text = gendersymbols[tr.Gender & 1];
            Label_OTGender.ForeColor = Draw.GetGenderColor(tr.Gender & 1);
            TID_Trainer.LoadInfo(tr);

            if (tr.Game >= 0)
                CB_GameOrigin.SelectedValue = tr.Game;

            var lang = tr.Language;
            if (lang <= 0)
                lang = (int)LanguageID.English;
            CB_Language.SelectedValue = lang;
            if (tr.ConsoleRegion != 0)
            {
                CB_3DSReg.SelectedValue = tr.ConsoleRegion;
                CB_Country.SelectedValue = tr.Country;
                CB_SubRegion.SelectedValue = tr.SubRegion;
            }

            // Copy OT trash bytes for sensitive games (Gen1/2)
                 if (tr is SAV1 s1 && Entity is PK1 p1) p1.OT_Trash = s1.OT_Trash;
            else if (tr is SAV2 s2 && Entity is PK2 p2) p2.OT_Trash = s2.OT_Trash;

            UpdateNickname(null, EventArgs.Empty);
        }

        private void SetDetailsHT(ITrainerInfo tr)
        {
            if (string.IsNullOrWhiteSpace(tr.OT))
                return;

            if (TB_OTt2.Text.Length > 0)
            {
                Label_CTGender.Text = gendersymbols[tr.Gender & 1];
                if (Entity is IHandlerLanguage)
                    CB_HTLanguage.SelectedValue = tr.Language;
            }
        }

        private void SetForms()
        {
            int species = Entity.Species;
            var pi = RequestSaveFile?.Personal[species] ?? Entity.PersonalInfo;
            bool hasForms = FormConverter.HasFormSelection(pi, species, Entity.Format);
            CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = hasForms;

            if (HaX && Entity.Format >= 4)
                Label_Form.Visible = true; // show with value entry textbox

            if (!hasForms)
                return;

            var ds = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, gendersymbols, Entity.Format);
            if (ds.Length == 1 && string.IsNullOrEmpty(ds[0])) // empty (Alolan Totems)
                CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = false;
            else
                CB_Form.DataSource = ds;
        }

        private void SetAbilityList()
        {
            if (Entity.Format < 3) // no abilities
                return;

            if (Entity.Format > 3 && FieldsLoaded) // has forms
                Entity.AltForm = CB_Form.SelectedIndex; // update pkm field for form specific abilities

            int abil = CB_Ability.SelectedIndex;

            bool tmp = FieldsLoaded;
            FieldsLoaded = false;
            CB_Ability.DataSource = GameInfo.FilteredSources.GetAbilityList(Entity);
            CB_Ability.SelectedIndex = GetSafeIndex(CB_Ability, abil); // restore original index if available
            FieldsLoaded = tmp;
        }

        private static int GetSafeIndex(ComboBox cb, int index) => Math.Max(0, Math.Min(cb.Items.Count - 1, index));

        private void UpdateIsShiny()
        {
            // Recalculate shininiess
            bool isShiny = Entity.IsShiny;

            // Set the Controls
            BTN_Shinytize.Visible = BTN_Shinytize.Enabled = !isShiny;
            if (Entity.Format >= 8 && (Entity.ShinyXor == 0 || Entity.FatefulEncounter))
            {
                Label_IsShiny.Visible = false;
                Label_IsShiny2.Visible = isShiny;
            }
            else
            {
                Label_IsShiny.Visible = isShiny;
                Label_IsShiny2.Visible = false;
            }

            // Refresh Markings (for Shiny Star if applicable)
            SetMarkings();
        }

        private void SetMarkings()
        {
            static double getOpacity(bool b) => b ? 1 : 0.175;
            static Image changeOpacity(PictureBox p, double opacity) => opacity == 1 ? p.InitialImage
                : ImageUtil.ChangeOpacity(p.InitialImage, opacity);

            var pba = Markings;
            var markings = Entity.Markings;
            for (int i = 0; i < pba.Length; i++)
                pba[i].Image = changeOpacity(pba[i], getOpacity(markings[i] != 0));

            PB_MarkShiny.Image = changeOpacity(PB_MarkShiny, getOpacity(!BTN_Shinytize.Enabled));
            PB_MarkCured.Image = changeOpacity(PB_MarkCured, getOpacity(CHK_Cured.Checked));

            PB_Favorite.Image = changeOpacity(PB_Favorite, getOpacity(Entity is IFavorite pb7 && pb7.Favorite));
            PB_Origin.Image = GetOriginSprite(Entity);

            // Colored Markings
            if (Entity.Format < 7)
                return;

            for (int i = 0; i < pba.Length; i++)
            {
                if (Draw.GetMarkingColor(markings[i], out Color c))
                    pba[i].Image = ImageUtil.ChangeAllColorTo(pba[i].Image, c);
            }
        }

        private static Image GetOriginSprite(PKM pkm)
        {
            if (pkm.Format < 6)
                return null;

            // Specific Markings
            if (pkm.VC)
                return Properties.Resources.gen_vc;
            if (pkm.GO)
                return Properties.Resources.gen_go;
            if (pkm.GG) // LGP/E -- GO already returned above.
                return Properties.Resources.gen_gg;

            // Lumped Generations
            if (pkm.Gen6)
                return Properties.Resources.gen_6;
            if (pkm.Gen7)
                return Properties.Resources.gen_7;
            if (pkm.Gen8)
                return Properties.Resources.gen_8;

            return null;
        }

        private void UpdateGender()
        {
            int Gender = Entity.GetSaneGender();
            Label_Gender.Text = gendersymbols[Gender];
            Label_Gender.ForeColor = Draw.GetGenderColor(Gender);
        }

        private static void SetCountrySubRegion(ComboBox CB, string type)
        {
            int index = CB.SelectedIndex;
            CB.DataSource = Util.GetCountryRegionList(type, GameInfo.CurrentLanguage);

            if (index > 0 && index < CB.Items.Count)
                CB.SelectedIndex = index;
        }

        // Prompted Updates of PKM //
        private void ClickFriendship(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // clear
                TB_Friendship.Text = "0";
            else
                TB_Friendship.Text = TB_Friendship.Text == "255" ? Entity.PersonalInfo.BaseFriendship.ToString() : "255";
        }

        private void ClickLevel(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
                ((MaskedTextBox)sender).Text = "100";
        }

        private void ClickGender(object sender, EventArgs e)
        {
            if (!Entity.PersonalInfo.IsDualGender)
                return; // can't toggle

            int newGender = (PKX.GetGenderFromString(Label_Gender.Text) & 1) ^ 1;
            if (Entity.Format <= 2)
            {
                Stats.SetATKIVGender(newGender);
                UpdateIsShiny();
            }
            else if (Entity.Format <= 4)
            {
                Entity.Version = WinFormsUtil.GetIndex(CB_GameOrigin);
                Entity.Nature = WinFormsUtil.GetIndex(CB_Nature);
                Entity.AltForm = CB_Form.SelectedIndex;

                Entity.SetPIDGender(newGender);
                TB_PID.Text = Entity.PID.ToString("X8");
            }
            Entity.Gender = newGender;
            Label_Gender.Text = gendersymbols[newGender];
            Label_Gender.ForeColor = Draw.GetGenderColor(newGender);

            if (PKX.GetGenderFromString(CB_Form.Text) < 2) // Gendered Forms
                CB_Form.SelectedIndex = Math.Min(newGender, CB_Form.Items.Count - 1);

            UpdatePreviewSprite(Label_Gender, EventArgs.Empty);
        }

        private void ClickPP(object sender, EventArgs e)
        {
            for (int i = 0; i < MovePP.Length; i++)
                RefreshMovePP(i);
        }

        private void ClickPPUps(object sender, EventArgs e)
        {
            bool min = ModifierKeys.HasFlag(Keys.Control);
            static int getValue(ComboBox cb, bool zero) => zero || WinFormsUtil.GetIndex(cb) == 0 ? 0 : 3;
            CB_PPu1.SelectedIndex = getValue(CB_Move1, min);
            CB_PPu2.SelectedIndex = getValue(CB_Move2, min);
            CB_PPu3.SelectedIndex = getValue(CB_Move3, min);
            CB_PPu4.SelectedIndex = getValue(CB_Move4, min);
        }

        private void ClickMarking(object sender, EventArgs e)
        {
            int index = Array.IndexOf(Markings, (PictureBox)sender);
            Entity.ToggleMarking(index);
            SetMarkings();
        }

        private void ClickFavorite(object sender, EventArgs e)
        {
            if (Entity is IFavorite pb7)
                pb7.Favorite ^= true;
            SetMarkings();
        }

        private void ClickOT(object sender, EventArgs e) => SetDetailsOT(SaveFileRequested?.Invoke(this, e));
        private void ClickCT(object sender, EventArgs e) => SetDetailsHT(SaveFileRequested?.Invoke(this, e));

        private void ClickTRGender(object sender, EventArgs e)
        {
            if (!(sender is Label lbl))
                return;
            if (string.IsNullOrWhiteSpace(lbl.Text))
                return;

            int gender = PKX.GetGenderFromString(lbl.Text) ^ 1;
            lbl.Text = gendersymbols[gender];
            lbl.ForeColor = Draw.GetGenderColor(gender);
        }

        private void ClickBall(object sender, EventArgs e)
        {
            Entity.Ball = WinFormsUtil.GetIndex(CB_Ball);
            if (ModifierKeys.HasFlag(Keys.Alt))
            {
                CB_Ball.SelectedValue = (int)Ball.Poke;
                return;
            }
            if (ModifierKeys.HasFlag(Keys.Shift))
            {
                CB_Ball.SelectedValue = BallApplicator.ApplyBallLegalByColor(Entity);
                return;
            }

            using var frm = new BallBrowser();
            frm.LoadBalls(Entity);
            frm.ShowDialog();
            if (frm.BallChoice >= 0)
                CB_Ball.SelectedValue = frm.BallChoice;
        }

        private void ClickShinyLeaf(object sender, EventArgs e) => ShinyLeaf.CheckAll(ModifierKeys != Keys.Control);

        private void ClickMetLocation(object sender, EventArgs e)
        {
            if (HaX)
                return;

            Entity = PreparePKM();
            UpdateLegality(skipMoveRepop: true);
            if (Legality.Valid)
                return;
            if (!SetSuggestedMetLocation())
                return;

            Entity = PreparePKM();
            UpdateLegality();
        }

        private void ClickGT(object sender, EventArgs e)
        {
            if (!GB_nOT.Visible)
                return;

            if (sender == GB_OT)
                Entity.CurrentHandler = 0;
            else if (TB_OTt2.Text.Length > 0)
                Entity.CurrentHandler = 1;
            UpadteHandlingTrainerBackground(Entity);

            TB_Friendship.Text = Entity.CurrentFriendship.ToString();
        }

        private void ClickNature(object sender, EventArgs e)
        {
            if (Entity.Format < 8)
                return;
            if (sender == Label_Nature)
                CB_Nature.SelectedIndex = CB_StatNature.SelectedIndex;
            else
                CB_StatNature.SelectedIndex = CB_Nature.SelectedIndex;
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
            int[] m = Entity.GetMoveSet(random);
            if (m?.Any(z => z != 0) != true)
            {
                if (!silent)
                    WinFormsUtil.Alert(MsgPKMSuggestionFormat);
                return false;
            }

            if (Entity.Moves.SequenceEqual(m))
                return false;

            if (!silent)
            {
                var mv = GameInfo.Strings.Move;
                var movestrings = m.Select(v => (uint)v >= mv.Count ? MsgProgramError : mv[v]);
                var msg = string.Join(Environment.NewLine, movestrings);
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgPKMSuggestionMoves, msg))
                    return false;
            }

            Entity.SetMoves(m);
            FieldsLoaded = false;
            LoadMoves(Entity);
            ClickPP(null, EventArgs.Empty);
            FieldsLoaded = true;
            return true;
        }

        private bool SetSuggestedRelearnMoves(bool silent = false)
        {
            if (Entity.Format < 6)
                return false;

            var m = Legality.GetSuggestedRelearnMoves();
            if (Entity.RelearnMoves.SequenceEqual(m) || m.Count != 4)
                return false;

            if (!silent)
            {
                var mv = GameInfo.Strings.Move;
                var movestrings = m.Select(v => (uint)v >= mv.Count ? MsgProgramError : mv[v]);
                var msg = string.Join(Environment.NewLine, movestrings);
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgPKMSuggestionRelearn, msg))
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
            var encounter = EncounterSuggestion.GetSuggestedMetInfo(Entity);
            if (encounter == null || (Entity.Format >= 3 && encounter.Location < 0))
            {
                if (!silent)
                    WinFormsUtil.Alert(MsgPKMSuggestionNone);
                return false;
            }

            int level = encounter.LevelMin;
            int location = encounter.Location;
            int minlvl = EncounterSuggestion.GetLowestLevel(Entity, encounter.LevelMin);
            if (minlvl == 0)
                minlvl = level;

            if (Entity.CurrentLevel >= minlvl && Entity.Met_Level == level && Entity.Met_Location == location)
                return false;
            if (minlvl < level)
                minlvl = level;

            if (!silent)
            {
                var suggestions = EditPKMUtil.GetSuggestionMessage(Entity, level, location, minlvl);
                if (suggestions.Count <= 1) // no suggestion
                    return false;

                var msg = string.Join(Environment.NewLine, suggestions);
                if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, msg) != DialogResult.Yes)
                    return false;
            }

            if (Entity.Format >= 3)
            {
                Entity.Met_Location = location;
                TB_MetLevel.Text = encounter.GetSuggestedMetLevel(Entity).ToString();
                CB_MetLocation.SelectedValue = location;

                if (Entity.Gen6 && Entity.WasEgg && ModifyPKM)
                    Entity.SetHatchMemory6();
            }

            if (Entity.CurrentLevel < minlvl)
                TB_Level.Text = minlvl.ToString();

            return true;
        }

        public void UpdateIVsGB(bool skipForm)
        {
            if (!FieldsLoaded)
                return;
            Label_Gender.Text = gendersymbols[Entity.Gender];
            Label_Gender.ForeColor = Draw.GetGenderColor(Entity.Gender);
            if (Entity.Species == (int)Species.Unown && !skipForm)
                CB_Form.SelectedIndex = Entity.AltForm;

            UpdateIsShiny();
            UpdateSprite();
        }

        private void UpdateBall(object sender, EventArgs e)
        {
            PB_Ball.Image = SpriteUtil.GetBallSprite(WinFormsUtil.GetIndex(CB_Ball));
        }

        private void UpdateEXPLevel(object sender, EventArgs e)
        {
            if (ChangingFields)
                return;
            ChangingFields = true;
            if (sender == TB_EXP)
            {
                // Change the Level
                uint EXP = Util.ToUInt32(TB_EXP.Text);
                var gr = Entity.PersonalInfo.EXPGrowth;
                int Level = Experience.GetLevel(EXP, gr);
                if (Level == 100)
                    EXP = Experience.GetEXP(100, gr);

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
                {
                    TB_Level.Text = "1";
                }
                else if (Level > 100)
                {
                    TB_Level.Text = "100";
                    if (!HaX)
                        Level = 100;
                }
                if (Level > byte.MaxValue)
                    MT_Level.Text = "255";
                else if (Level <= 100)
                    TB_EXP.Text = Experience.GetEXP(Level, Entity.PersonalInfo.EXPGrowth).ToString();
            }
            ChangingFields = false;
            if (FieldsLoaded) // store values back
            {
                Entity.EXP = Util.ToUInt32(TB_EXP.Text);
                Entity.Stat_Level = Util.ToInt32((HaX ? MT_Level : TB_Level).Text);
            }
            UpdateStats();
            UpdateLegality();
        }

        private void UpdateRandomPID(object sender, EventArgs e)
        {
            if (Entity.Format < 3)
                return;
            if (FieldsLoaded)
                Entity.PID = Util.GetHexValue(TB_PID.Text);

            if (sender == Label_Gender)
                Entity.SetPIDGender(Entity.Gender);
            else if (sender == CB_Nature && Entity.Nature != WinFormsUtil.GetIndex(CB_Nature))
                Entity.SetPIDNature(WinFormsUtil.GetIndex(CB_Nature));
            else if (sender == BTN_RerollPID)
                Entity.SetPIDGender(Entity.Gender);
            else if (sender == CB_Ability && CB_Ability.SelectedIndex != Entity.PIDAbility && Entity.PIDAbility > -1)
                Entity.SetAbilityIndex(CB_Ability.SelectedIndex);

            TB_PID.Text = Entity.PID.ToString("X8");
            if (Entity.Format >= 6 && (Entity.Gen3 || Entity.Gen4 || Entity.Gen5))
                TB_EC.Text = TB_PID.Text;
            Update_ID(TB_EC, e);
        }

        private void UpdateRandomEC(object sender, EventArgs e)
        {
            if (Entity.Format < 6)
                return;

            Entity.SetRandomEC();
            TB_EC.Text = Entity.EncryptionConstant.ToString("X8");
            Update_ID(TB_EC, e);
            UpdateLegality();
        }

        private void Update255_MTB(object sender, EventArgs e)
        {
            if (!(sender is MaskedTextBox tb))
                return;
            if (Util.ToInt32(tb.Text) > byte.MaxValue)
                tb.Text = "255";
            if (sender == TB_Friendship && int.TryParse(TB_Friendship.Text, out var val))
            {
                Entity.CurrentFriendship = val;
                UpdateStats();
            }
        }

        private void UpdateFormArgument(object sender, EventArgs e)
        {
            if (FieldsLoaded && Entity.Species == (int)Species.Alcremie)
                UpdateSprite();
        }

        private void UpdateForm(object sender, EventArgs e)
        {
            if (CB_Form == sender && FieldsLoaded)
            {
                Entity.AltForm = CB_Form.SelectedIndex;
                uint EXP = Experience.GetEXP(Entity.CurrentLevel, Entity.PersonalInfo.EXPGrowth);
                TB_EXP.Text = EXP.ToString();
            }

            UpdateStats();
            SetAbilityList();

            // Gender Forms
            if (WinFormsUtil.GetIndex(CB_Species) == (int)Species.Unown && FieldsLoaded)
            {
                if (Entity.Format == 3)
                {
                    Entity.SetPIDUnown3(CB_Form.SelectedIndex);
                    TB_PID.Text = Entity.PID.ToString("X8");
                }
                else if (Entity.Format == 2)
                {
                    int desiredForm = CB_Form.SelectedIndex;
                    while (Entity.AltForm != desiredForm)
                    {
                        FieldsLoaded = false;
                        Stats.UpdateRandomIVs(null, EventArgs.Empty);
                        FieldsLoaded = true;
                    }
                }
            }
            else if (CB_Form.Enabled && PKX.GetGenderFromString(CB_Form.Text) < 2)
            {
                if (CB_Form.Items.Count == 2) // actually M/F; Pumpkaboo formes in German are S,M,L,XL
                {
                    Entity.Gender = CB_Form.SelectedIndex;
                    UpdateGender();
                }
            }
            else
            {
                UpdateGender();
            }

            RefreshFormArguments();
            if (ChangingFields)
                return;
            ChangingFields = true;
            MT_Form.Text = Math.Max(0, CB_Form.SelectedIndex).ToString();
            ChangingFields = false;

            UpdateSprite();
        }

        private void RefreshFormArguments()
        {
            int index = CB_FormArgument.SelectedIndex;
            var items = FormConverter.GetFormArgumentStrings(Entity.Species, Entity.AltForm, Entity.Format);
            CB_FormArgument.Items.Clear();
            CB_FormArgument.Items.AddRange(items);
            CB_FormArgument.Visible = !string.IsNullOrWhiteSpace(items[0]);
            if (ChangingFields)
                return;
            CB_FormArgument.SelectedIndex = index < items.Length ? index : 0;
        }

        private void UpdateHaXForm(object sender, EventArgs e)
        {
            if (ChangingFields)
                return;
            ChangingFields = true;
            int form = Entity.AltForm = Util.ToInt32(MT_Form.Text);
            CB_Form.SelectedIndex = CB_Form.Items.Count > form ? form : -1;
            ChangingFields = false;

            UpdateSprite();
        }

        private void UpdatePP(object sender, EventArgs e)
        {
            if (!(sender is ComboBox cb))
                return;
            int index = Array.IndexOf(Moves, cb);
            if (index < 0)
                index = Array.IndexOf(PPUps, cb);
            if (index < 0)
                return;

            RefreshMovePP(index);
        }

        private void RefreshMovePP(int index)
        {
            int move = WinFormsUtil.GetIndex(Moves[index]);
            var ppctrl = PPUps[index];
            int ppups = ppctrl.SelectedIndex;
            if (move <= 0)
            {
                ppctrl.SelectedIndex = 0;
                MovePP[index].Text = 0.ToString();
            }
            else
            {
                MovePP[index].Text = Entity.GetMovePP(move, ppups).ToString();
            }
        }

        private void UpdatePKRSstrain(object sender, EventArgs e)
        {
            // Change the PKRS Days to the legal bounds.
            int currentDuration = CB_PKRSDays.SelectedIndex;
            CB_PKRSDays.Items.Clear();

            int max = (CB_PKRSStrain.SelectedIndex % 4) + 2;
            foreach (int day in Enumerable.Range(0, max))
                CB_PKRSDays.Items.Add(day);

            // Set the days back if they're legal, else set it to 1. (0 always passes).
            CB_PKRSDays.SelectedIndex = currentDuration < CB_PKRSDays.Items.Count ? currentDuration : 1;

            if (CB_PKRSStrain.SelectedIndex != 0)
                return;

            // Never Infected
            CB_PKRSDays.SelectedIndex = 0;
            CHK_Cured.Checked = false;
            CHK_Infected.Checked = false;
        }

        private void UpdatePKRSdays(object sender, EventArgs e)
        {
            if (CB_PKRSDays.SelectedIndex != 0)
                return;

            // If no days are selected
            if (CB_PKRSStrain.SelectedIndex == 0)
                CHK_Cured.Checked = CHK_Infected.Checked = false; // No Strain = Never Cured / Infected, triggers Strain update
            else CHK_Cured.Checked = true; // Any Strain = Cured
        }

        private void UpdatePKRSCured(object sender, EventArgs e)
        {
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
            if (CHK_Cured.Checked && !CHK_Infected.Checked)
            { CHK_Cured.Checked = false; return; }
            if (CHK_Cured.Checked)
                return;
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked;
            if (!CHK_Infected.Checked) { CB_PKRSStrain.SelectedIndex = 0; CB_PKRSDays.SelectedIndex = 0; Label_PKRSdays.Visible = CB_PKRSDays.Visible = false; }
            else if (CB_PKRSStrain.SelectedIndex == 0)
            {
                CB_PKRSStrain.SelectedIndex = CB_PKRSDays.SelectedIndex = 1;
                Label_PKRSdays.Visible = CB_PKRSDays.Visible = true;
                UpdatePKRSCured(sender, e);
            }
        }

        private void UpdateCountry(object sender, EventArgs e)
        {
            int index;
            if (sender is ComboBox c && (index = WinFormsUtil.GetIndex(c)) > 0)
                SetCountrySubRegion(CB_SubRegion, $"sr_{index:000}");
        }

        private void UpdateSpecies(object sender, EventArgs e)
        {
            // Get Species dependent information
            if (FieldsLoaded)
                Entity.Species = WinFormsUtil.GetIndex(CB_Species);
            SpeciesIDTip.SetToolTip(CB_Species, Entity.Species.ToString("000"));
            SetAbilityList();
            SetForms();
            UpdateForm(null, EventArgs.Empty);

            if (!FieldsLoaded)
                return;

            // Recalculate EXP for Given Level
            uint EXP = Experience.GetEXP(Entity.CurrentLevel, Entity.PersonalInfo.EXPGrowth);
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
            if (FieldsLoaded)
                Entity.Version = (int)Version;

            // check if differs
            var group = GameUtil.GetMetLocationVersionGroup(Version);

            if (group != origintrack)
                ReloadMetLocations(Version);
            origintrack = group;

            // Visibility logic for Gen 4 encounter type; only show for Gen 4 Pokemon.
            if (Entity.Format >= 4)
            {
                bool g4 = Entity.Gen4;
                CB_EncounterType.Visible = Label_EncounterType.Visible = g4 && Entity.Format < 7;
                if (!g4)
                    CB_EncounterType.SelectedValue = 0;
            }

            if (!FieldsLoaded)
                return;

            TID_Trainer.LoadIDValues(Entity);
            UpdateLegality();
        }

        private void ReloadMetLocations(GameVersion Version)
        {
            var met_list = GameInfo.GetLocationList(Version, Entity.Format, egg: false);
            CB_MetLocation.DataSource = new BindingSource(met_list, null);

            var egg_list = GameInfo.GetLocationList(Version, Entity.Format, egg: true);
            CB_EggLocation.DataSource = new BindingSource(egg_list, null);

            // Stretch C/XD met location dropdowns
            int width = CB_EggLocation.DropDownWidth;
            if (Version == GameVersion.CXD && Entity.Format == 3)
                width *= 2;
            CB_MetLocation.DropDownWidth = width;

            if (FieldsLoaded)
            {
                SetMarkings(); // Set/Remove the Nativity marking when gamegroup changes too
                int metLoc = EncounterSuggestion.GetSuggestedTransferLocation(Entity);
                CB_MetLocation.SelectedValue = Math.Max(0, metLoc);
                CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
            }
            else
            {
                CB_GameOrigin.Focus(); // hacky validation forcing
            }
        }

        private void UpdateExtraByteValue(object sender, EventArgs e)
        {
            if (CB_ExtraBytes.Items.Count == 0 || !(sender is MaskedTextBox mtb))
                return;
            // Changed Extra Byte's Value
            if (Util.ToInt32(mtb.Text) > byte.MaxValue)
                mtb.Text = "255";

            int value = Util.ToInt32(mtb.Text);
            int offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
            Entity.Data[offset] = (byte)value;
        }

        private void UpdateExtraByteIndex(object sender, EventArgs e)
        {
            if (CB_ExtraBytes.Items.Count == 0)
                return;
            // Byte changed, need to refresh the Text box for the byte's value.
            TB_ExtraByte.Text = Entity.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
        }

        private void UpdateNatureModification(ComboBox cb)
        {
            string text = Stats.UpdateNatureModification(Entity.StatNature);
            NatureTip.SetToolTip(cb, text);
        }

        private void UpdateIsNicknamed(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;

            Entity.Nickname = TB_Nickname.Text;
            if (CHK_Nicknamed.Checked)
                return;

            int species = WinFormsUtil.GetIndex(CB_Species);
            if (species < 1 || species > Entity.MaxSpeciesID)
                return;

            if (CHK_IsEgg.Checked)
                species = 0; // get the egg name.

            if (SpeciesName.IsNicknamedAnyLanguage(species, TB_Nickname.Text, Entity.Format))
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
                        return;
                }
            }

            int lang = WinFormsUtil.GetIndex(CB_Language);

            if (CHK_Nicknamed.Checked)
                return;

            // Fetch Current Species and set it as Nickname Text
            int species = WinFormsUtil.GetIndex(CB_Species);
            if (species < 1 || species > Entity.MaxSpeciesID)
            { TB_Nickname.Text = string.Empty; return; }

            if (CHK_IsEgg.Checked)
                species = 0; // get the egg name.

            // If name is that of another language, don't replace the nickname
            if (sender != CB_Language && species != 0 && !SpeciesName.IsNicknamedAnyLanguage(species, TB_Nickname.Text, Entity.Format))
                return;

            TB_Nickname.Text = SpeciesName.GetSpeciesNameGeneration(species, lang, Entity.Format);
            if (Entity is GBPKM pk)
                pk.SetNotNicknamed();
        }

        private void UpdateNicknameClick(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox ?? TB_Nickname;
            // Special Character Form
            if (ModifierKeys != Keys.Control)
                return;

            var sav = RequestSaveFile;
            if (sav == null) // form did not provide the needed info
                return;

            if (tb == TB_Nickname)
            {
                Entity.Nickname = tb.Text;
                var d = new TrashEditor(tb, Entity.Nickname_Trash, sav);
                d.ShowDialog();
                tb.Text = d.FinalString;
                Entity.Nickname_Trash = d.FinalBytes;
            }
            else if (tb == TB_OT)
            {
                Entity.OT_Name = tb.Text;
                var d = new TrashEditor(tb, Entity.OT_Trash, sav);
                d.ShowDialog();
                tb.Text = d.FinalString;
                Entity.OT_Trash = d.FinalBytes;
            }
            else if (tb == TB_OTt2)
            {
                Entity.HT_Name = tb.Text;
                var d = new TrashEditor(tb, Entity.HT_Trash, sav);
                d.ShowDialog();
                tb.Text = d.FinalString;
                Entity.HT_Trash = d.FinalBytes;
            }
        }

        private void UpdateNotOT(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TB_OTt2.Text))
            {
                ClickGT(GB_OT, null); // Switch CT over to OT.
                Label_CTGender.Text = string.Empty;
                TB_Friendship.Text = Entity.CurrentFriendship.ToString();
            }
            else if (string.IsNullOrWhiteSpace(Label_CTGender.Text))
            {
                Label_CTGender.Text = gendersymbols[0];
            }
        }

        private void UpdateIsEgg(object sender, EventArgs e)
        {
            // Display hatch counter if it is an egg, Display Friendship if it is not.
            Label_HatchCounter.Visible = CHK_IsEgg.Checked && Entity.Format > 1;
            Label_Friendship.Visible = !CHK_IsEgg.Checked && Entity.Format > 1;

            if (!FieldsLoaded)
                return;

            Entity.IsEgg = CHK_IsEgg.Checked;
            if (CHK_IsEgg.Checked)
            {
                TB_Friendship.Text = "1";

                // If we are an egg, it won't have a met location.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CAL_MetDate.Value = new DateTime(2000, 01, 01);

                // if egg wasn't originally obtained by OT => Link Trade, else => None
                bool isTraded = false;
                var sav = SaveFileRequested?.Invoke(this, e);
                if (sav != null)
                    isTraded = sav.OT != TB_OT.Text || sav.TID != Entity.TID || sav.SID != Entity.SID;
                CB_MetLocation.SelectedIndex = isTraded ? 2 : 0;

                if (!CHK_Nicknamed.Checked)
                {
                    TB_Nickname.Text = SpeciesName.GetSpeciesNameGeneration(0, WinFormsUtil.GetIndex(CB_Language), Entity.Format);
                    if (Entity.Format != 4) // eggs in gen4 do not have nickname flag
                        CHK_Nicknamed.Checked = true;
                }

                // Wipe egg memories
                if (Entity.Format >= 6 && ModifyPKM)
                    Entity.ClearMemories();
            }
            else // Not Egg
            {
                if (!CHK_Nicknamed.Checked)
                    UpdateNickname(null, EventArgs.Empty);

                TB_Friendship.Text = Entity.PersonalInfo.BaseFriendship.ToString();

                if (CB_EggLocation.SelectedIndex == 0)
                {
                    CAL_EggDate.Value = new DateTime(2000, 01, 01);
                    CHK_AsEgg.Checked = false;
                    GB_EggConditions.Enabled = false;
                }

                if (TB_Nickname.Text == SpeciesName.GetSpeciesNameGeneration(0, WinFormsUtil.GetIndex(CB_Language), Entity.Format))
                    CHK_Nicknamed.Checked = false;
            }

            UpdateNickname(null, EventArgs.Empty);
            UpdateSprite();
        }

        private void UpdateMetAsEgg(object sender, EventArgs e)
        {
            GB_EggConditions.Enabled = CHK_AsEgg.Checked;
            if (CHK_AsEgg.Checked)
            {
                if (!FieldsLoaded)
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
            var ShinyPID = Entity.Format <= 2 || ModifierKeys != Keys.Control;
            UpdateShiny(ShinyPID);
        }

        private void UpdateShiny(bool PID)
        {
            Entity.PID = Util.GetHexValue(TB_PID.Text);
            Entity.Nature = WinFormsUtil.GetIndex(CB_Nature);
            Entity.Gender = PKX.GetGenderFromString(Label_Gender.Text);
            Entity.AltForm = CB_Form.SelectedIndex;
            Entity.Version = WinFormsUtil.GetIndex(CB_GameOrigin);

            if (Entity.Format > 2)
            {
                if (PID)
                {
                    CommonEdits.SetShiny(Entity, ModifierKeys == Keys.Shift);
                    TB_PID.Text = Entity.PID.ToString("X8");

                    int gen = Entity.GenNumber;
                    bool pre3DS = 1 <= gen && gen < 6;
                    if (pre3DS && TB_EC.Visible)
                        TB_EC.Text = TB_PID.Text;
                }
                else
                {
                    Entity.SetShinySID();
                    TID_Trainer.UpdateSID();
                }
            }
            else
            {
                Entity.SetShiny();
                Stats.LoadIVs(Entity.IVs);
                Stats.UpdateIVs(null, EventArgs.Empty);
            }

            UpdateIsShiny();
            UpdatePreviewSprite?.Invoke(this, EventArgs.Empty);
            UpdateLegality();
        }

        private void UpdateTSV(object sender, EventArgs e)
        {
            if (Entity.Format < 6)
                return;

            TID_Trainer.UpdateTSV();

            Entity.PID = Util.GetHexValue(TB_PID.Text);
            var tip = $"PSV: {Entity.PSV:d4}";
            if (Entity.IsShiny)
                tip += $" | Xor = {Entity.ShinyXor}";
            Tip3.SetToolTip(TB_PID, tip);
        }

        private void Update_ID(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;
            // Trim out nonhex characters
            TB_PID.Text = (Entity.PID = Util.GetHexValue(TB_PID.Text)).ToString("X8");
            TB_EC.Text = (Entity.EncryptionConstant = Util.GetHexValue(TB_EC.Text)).ToString("X8");

            UpdateIsShiny();
            UpdateSprite();
            Stats.UpdateCharacteristic();   // If the EC is changed, EC%6 (Characteristic) might be changed.
            if (Entity.Format <= 4)
            {
                FieldsLoaded = false;
                Entity.PID = Util.GetHexValue(TB_PID.Text);
                CB_Nature.SelectedValue = Entity.Nature;
                Label_Gender.Text = gendersymbols[Entity.Gender];
                Label_Gender.ForeColor = Draw.GetGenderColor(Entity.Gender);
                FieldsLoaded = true;
            }
        }

        private void Update_ID64(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;
            // Trim out nonhex characters
            if (sender == TB_HomeTracker && Entity is IHomeTrack home)
            {
                var value = Util.GetHexValue64(TB_HomeTracker.Text);
                home.Tracker = value;
                TB_HomeTracker.Text = value.ToString("X16");
            }
        }

        private void UpdateShadowID(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;
            FLP_Purification.Visible = NUD_ShadowID.Value > 0;
        }

        private void UpdatePurification(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;
            FieldsLoaded = false;
            CHK_Shadow.Checked = NUD_Purification.Value > 0;
            FieldsLoaded = true;
        }

        private void UpdateShadowCHK(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;
            FieldsLoaded = false;
            NUD_Purification.Value = CHK_Shadow.Checked ? NUD_Purification.Maximum : 0;
            ((IShadowPKM)Entity).Purification = (int)NUD_Purification.Value;
            UpdatePreviewSprite?.Invoke(this, EventArgs.Empty);
            FieldsLoaded = true;
        }

        private void ValidateComboBox(ComboBox cb)
        {
            if (cb.Text.Length == 0 && cb.Items.Count > 0)
                cb.SelectedIndex = 0;
            else if (cb.SelectedValue == null)
                cb.BackColor = Draw.InvalidSelection;
            else
                cb.ResetBackColor();
        }

        private void ValidateComboBox(object sender, CancelEventArgs e)
        {
            if (!(sender is ComboBox cb))
                return;

            ValidateComboBox(cb);
            UpdateSprite();
        }

        private void ValidateComboBox2(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;

            ValidateComboBox(sender, null);
            if (sender == CB_Ability)
            {
                if (Entity.Format >= 6)
                    TB_AbilityNumber.Text = (1 << CB_Ability.SelectedIndex).ToString();
                else if (Entity.Format <= 5 && CB_Ability.SelectedIndex < 2) // Format <= 5, not hidden
                    UpdateRandomPID(sender, e);
                UpdateLegality();
            }
            else if (sender == CB_Nature)
            {
                if (Entity.Format <= 4)
                    UpdateRandomPID(sender, e);
                Entity.Nature = WinFormsUtil.GetIndex(CB_Nature);
                UpdateNatureModification(CB_Nature);
                Stats.UpdateIVs(null, EventArgs.Empty); // updating Nature will trigger stats to update as well
                UpdateLegality();
            }
            else if (sender == CB_StatNature)
            {
                Entity.StatNature = WinFormsUtil.GetIndex(CB_StatNature);
                UpdateNatureModification(CB_StatNature);
                Stats.UpdateIVs(null, EventArgs.Empty); // updating Nature will trigger stats to update as well
                UpdateLegality();
            }
            else if (sender == CB_HeldItem)
            {
                UpdateLegality();
            }
        }

        private void ValidateMove(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;

            ValidateComboBox((ComboBox)sender);
            if (Moves.Contains(sender)) // Move
                UpdatePP(sender, e);

            // Legality
            Entity.Moves = Moves.Select(WinFormsUtil.GetIndex).ToArray();
            Entity.RelearnMoves = Relearn.Select(WinFormsUtil.GetIndex).ToArray();
            UpdateLegality(skipMoveRepop: true);
        }

        private void ValidateMovePaint(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            var item = (ComboItem)((ComboBox)sender).Items[e.Index];
            var valid = LegalMoveSource.CanLearn(item.Value) && !HaX;

            var current = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var brush = Draw.Brushes.GetBackground(valid, current);
            var textColor = Draw.GetText(current);

            DrawMoveRectangle(e, brush, item.Text, textColor);
        }

        private static void DrawMoveRectangle(DrawItemEventArgs e, Brush brush, string text, Color textColor)
        {
            var rec = new Rectangle(e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height + 0); // 1px left
            e.Graphics.FillRectangle(brush, rec);

            const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.ExpandTabs | TextFormatFlags.SingleLine;
            TextRenderer.DrawText(e.Graphics, text, e.Font, rec, textColor, flags);
        }

        private void MeasureDropDownHeight(object sender, MeasureItemEventArgs e) => e.ItemHeight = CB_RelearnMove1.ItemHeight;

        private void ValidateMoveDropDown(object sender, EventArgs e)
        {
            var s = (ComboBox) sender;
            var index = Array.IndexOf(Moves, s);
            if (LegalMoveSource.IsMoveBoxOrdered[index])
                return;
            SetMoveDataSource(s);
            LegalMoveSource.IsMoveBoxOrdered[index] = true;
        }

        private void SetMoveDataSource(ComboBox c)
        {
            var index = WinFormsUtil.GetIndex(c);
            c.DataSource = new BindingSource(LegalMoveSource.DataSource, null);
            c.SelectedValue = index;
        }

        private void ValidateLocation(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;

            ValidateComboBox((ComboBox)sender);
            Entity.Met_Location = WinFormsUtil.GetIndex(CB_MetLocation);
            Entity.Egg_Location = WinFormsUtil.GetIndex(CB_EggLocation);
            UpdateLegality();
        }

        // Secondary Windows for Ribbons/Amie/Memories
        private void OpenRibbons(object sender, EventArgs e)
        {
            using var form = new RibbonEditor(Entity);
            form.ShowDialog();
        }

        private void OpenMedals(object sender, EventArgs e)
        {
            using var form = new SuperTrainingEditor(Entity);
            form.ShowDialog();
        }

        private void OpenHistory(object sender, EventArgs e)
        {
            // Write back current values
            Entity.HT_Name = TB_OTt2.Text;
            Entity.OT_Name = TB_OT.Text;
            Entity.IsEgg = CHK_IsEgg.Checked;
            Entity.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);
            using var form = new MemoryAmie(Entity);
            form.ShowDialog();
            TB_Friendship.Text = Entity.CurrentFriendship.ToString();
        }

        private void B_Records_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                Entity.SetRecordFlags(Entity.Moves);
                UpdateLegality();
                return;
            }

            using var form = new TechRecordEditor(Entity);
            form.ShowDialog();
            UpdateLegality();
        }

        /// <summary>
        /// Refreshes the interface for the current PKM format.
        /// </summary>
        /// <param name="sav">Save File context the editor is editing for</param>
        /// <param name="pk">Pokémon data to edit</param>
        public bool ToggleInterface(SaveFile sav, PKM pk)
        {
            Entity = sav.GetCompatiblePKM(pk);
            ToggleInterface(Entity);
            return FinalizeInterface(sav);
        }

        private void ToggleInterface(PKM t)
        {
            var pb7 = t is PB7;
            int gen = t.Format;
            FLP_Purification.Visible = FLP_ShadowID.Visible = t is IShadowPKM;
            bool sizeCP = gen >= 8 || pb7;
            FLP_SizeCP.Visible = sizeCP;
            if (sizeCP)
                SizeCP.ToggleVisibility(t);
            PB_Favorite.Visible = t is IFavorite;
            BTN_History.Visible = gen >= 6 && !pb7;
            BTN_Ribbons.Visible = gen >= 3 && !pb7;
            BTN_Medals.Visible = gen >= 6 && gen <= 7 && !pb7;
            FLP_Country.Visible = FLP_SubRegion.Visible = FLP_3DSRegion.Visible = gen >= 6 && gen <= 7 && !pb7;
            FLP_OriginalNature.Visible = gen >= 8;
            B_Records.Visible = gen >= 8;
            CB_HTLanguage.Visible = gen >= 8;

            ToggleInterface(Entity.Format);
        }

        private void ToggleSecrets(bool hidden, int gen)
        {
            Label_EncryptionConstant.Visible = BTN_RerollEC.Visible = TB_EC.Visible = gen >= 6 && !hidden;
            BTN_RerollPID.Visible = Label_PID.Visible = TB_PID.Visible = gen >= 3 && !hidden;
            TB_HomeTracker.Visible = L_HomeTracker.Visible = gen >= 8 && !hidden;
        }

        private void ToggleInterface(int gen)
        {
            ToggleSecrets(HideSecretValues, gen);
            GB_nOT.Visible = GB_RelearnMoves.Visible = gen >= 6;

            PB_Origin.Visible = gen >= 6;
            FLP_NSparkle.Visible = L_NSparkle.Visible = CHK_NSparkle.Visible = gen == 5;

            CB_Form.Visible = Label_Form.Visible = CHK_AsEgg.Visible = GB_EggConditions.Visible = PB_Mark5.Visible = PB_Mark6.Visible = gen >= 4;
            FLP_ShinyLeaf.Visible = L_ShinyLeaf.Visible = ShinyLeaf.Visible = gen == 4;

            DEV_Ability.Enabled = DEV_Ability.Visible = gen > 3 && HaX;
            CB_Ability.Visible = !DEV_Ability.Enabled && gen >= 3;
            FLP_Nature.Visible = gen >= 3;
            FLP_Ability.Visible = gen >= 3;
            GB_ExtraBytes.Visible = GB_ExtraBytes.Enabled = gen >= 3;
            GB_Markings.Visible = gen >= 3;
            CB_Form.Enabled = gen >= 3;

            FLP_Friendship.Visible = FLP_Form.Visible = gen >= 2;
            FLP_HeldItem.Visible = gen >= 2;
            CHK_IsEgg.Visible = gen >= 2;
            FLP_PKRS.Visible = FLP_EggPKRSRight.Visible = gen >= 2;
            Label_OTGender.Visible = gen >= 2;
            FLP_CatchRate.Visible = gen == 1;

            // HaX override, needs to be after DEV_Ability enabled assignment.
            TB_AbilityNumber.Visible = gen >= 6 && DEV_Ability.Enabled;

            // Met Tab
            L_HomeTracker.Visible = TB_HomeTracker.Visible = gen >= 8;
            FLP_MetDate.Visible = gen >= 4;
            FLP_Fateful.Visible = FLP_Ball.Visible = FLP_OriginGame.Visible = gen >= 3;
            FLP_MetLocation.Visible = FLP_MetLevel.Visible = gen >= 2;
            FLP_EncounterType.Visible = gen >= 4 && gen <= 6;
            FLP_TimeOfDay.Visible = gen == 2;

            Contest.ToggleInterface(Entity, gen);
            Stats.ToggleInterface(Entity, gen);

            CenterSubEditors();
        }

        private bool FinalizeInterface(SaveFile sav)
        {
            FieldsLoaded = false;

            bool TranslationRequired = false;
            PopulateFilteredDataSources(sav);
            PopulateFields(Entity);

            // Save File Specific Limits
            TB_OT.MaxLength = Entity.OTLength;
            TB_OTt2.MaxLength = Entity.OTLength;
            TB_Nickname.MaxLength = Entity.NickLength;

            // Hide Unused Tabs
            if (Entity.Format == 1 && tabMain.TabPages.Contains(Tab_Met))
            {
                tabMain.TabPages.Remove(Tab_Met);
            }
            else if (Entity.Format != 1 && !tabMain.TabPages.Contains(Tab_Met))
            {
                tabMain.TabPages.Insert(1, Tab_Met);
                TranslationRequired = true;
            }

            if (!HaX && sav is SAV7b)
            {
                FLP_HeldItem.Visible = false;
                FLP_Country.Visible = false;
                FLP_SubRegion.Visible = false;
                FLP_3DSRegion.Visible = false;
            }

            // Common HaX Interface
            MT_Level.Enabled = MT_Level.Visible = MT_Form.Enabled = MT_Form.Visible = HaX;
            TB_Level.Visible = !HaX;

            // pk2 save files do not have an Origin Game stored. Prompt the met location list to update.
            if (Entity.Format == 2)
                UpdateOriginGame(null, EventArgs.Empty);
            return TranslationRequired;
        }

        private void CenterSubEditors()
        {
            // Recenter PKM SubEditors
            FLP_PKMEditors.Location = new Point((tabMain.TabPages[0].Width - FLP_PKMEditors.Width) / 2, FLP_PKMEditors.Location.Y);
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

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Action<ShowdownSet> LoadShowdownSet;

        private void LoadShowdownSetDefault(ShowdownSet Set)
        {
            var pk = PreparePKM();
            pk.ApplySetDetails(Set);
            PopulateFields(pk);
        }

        public void ChangeLanguage(ITrainerInfo sav, PKM pk)
        {
            // Force an update to the met locations
            origintrack = GameVersion.Invalid;

            InitializeLanguage(sav);
            CenterSubEditors();
            PopulateFields(pk); // put data back in form
        }

        public void FlickerInterface()
        {
            tabMain.SelectedTab = Tab_Met; // parent tab of CB_GameOrigin
            tabMain.SelectedTab = Tab_Main; // first tab
        }

        private void InitializeLanguage(ITrainerInfo sav)
        {
            var source = GameInfo.FilteredSources;
            // Set the various ComboBox DataSources up with their allowed entries
            SetCountrySubRegion(CB_Country, "countries");
            CB_3DSReg.DataSource = source.ConsoleRegions;

            CB_EncounterType.DataSource = new BindingSource(source.G4EncounterTypes, null);
            CB_Nature.DataSource = new BindingSource(source.Natures, null);
            CB_StatNature.DataSource = new BindingSource(source.Natures, null);

            // Sub editors
            Stats.InitializeDataSources();

            PopulateFilteredDataSources(sav);
        }

        private void PopulateFilteredDataSources(ITrainerInfo sav)
        {
            var source = GameInfo.FilteredSources;

            if (sav.Generation > 1)
                CB_HeldItem.DataSource = new BindingSource(source.Items, null);

            CB_Language.DataSource = new BindingSource(source.Languages, null);

            var langWith0 = new[] {GameInfo.Sources.Empty}.Concat(source.Languages).ToArray();
            CB_HTLanguage.DataSource = new BindingSource(langWith0, null);

            CB_Ball.DataSource = new BindingSource(source.Balls, null);
            CB_Species.DataSource = new BindingSource(source.Species, null);
            DEV_Ability.DataSource = new BindingSource(source.Abilities, null);
            CB_GameOrigin.DataSource = new BindingSource(source.Games, null);

            // Set the Move ComboBoxes too..
            LegalMoveSource.ReloadMoves(source.Moves);
            foreach (var cb in Moves.Concat(Relearn))
                cb.DataSource = new BindingSource(source.Moves, null);
        }
    }
}
