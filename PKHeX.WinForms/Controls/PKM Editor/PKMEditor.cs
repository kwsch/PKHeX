using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using System.ComponentModel;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public sealed partial class PKMEditor : UserControl, IMainEditor
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

            DrawForeColor = ForeColor;
            if (TextBrush == null) TextBrush = new SolidBrush(CB_Move1.ForeColor);
            if (BackBrush == null) BackBrush = new SolidBrush(CB_Move1.BackColor);

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
            }).ToArray();
            relearnPB = new[] { PB_WarnRelearn1, PB_WarnRelearn2, PB_WarnRelearn3, PB_WarnRelearn4 };
            movePB = new[] { PB_WarnMove1, PB_WarnMove2, PB_WarnMove3, PB_WarnMove4 };
            foreach (var c in WinFormsUtil.GetAllControlsOfType(this, typeof(ComboBox)))
                c.KeyDown += WinFormsUtil.RemoveDropCB;

            Stats.SetMainEditor(this);
            LoadShowdownSet = LoadShowdownSetDefault;
            TID_Trainer.UpdatedID += Update_ID;
        }

        private void UpdateStats() => Stats.UpdateStats();
        private void LoadPartyStats(PKM pk) => Stats.LoadPartyStats(pk);

        private void SavePartyStats(PKM pk)
        {
            Stats.SavePartyStats(pk);
            pk.Stat_Level = CurrentLevel;
        }

        public PKM CurrentPKM { get => FieldsInitialized ? PreparePKM() : pkm; set => pkm = value; }
        public bool ModifyPKM { private get; set; } = true;
        private bool _hideSecret;
        public bool HideSecretValues { private get => _hideSecret; set => ToggleSecrets(_hideSecret = value, RequestSaveFile.Generation); }
        public bool Unicode { get; set; } = true;
        public bool HaX { get; set; }
        public byte[] LastData { private get; set; }

        public PKM Data { get => pkm; set => pkm = value; }
        public PKM pkm { get; private set; }
        public bool FieldsInitialized { get; private set; }
        public bool FieldsLoaded { get; private set; }
        public bool ChangingFields { get; set; }
        public int CurrentLevel => Util.ToInt32((HaX ? MT_Level : TB_Level).Text);

        private GameVersion origintrack;
        private Action GetFieldsfromPKM;
        private Func<PKM> GetPKMfromFields;
        private LegalityAnalysis Legality;
        private string[] gendersymbols = { "♂", "♀", "-" };
        private readonly Image mixedHighlight = ImageUtil.ChangeOpacity(Resources.slotSet, 0.5);
        private HashSet<int> AllowedMoves = new HashSet<int>();

        public event EventHandler LegalityChanged;
        public event EventHandler UpdatePreviewSprite;
        public event EventHandler RequestShowdownImport;
        public event EventHandler RequestShowdownExport;
        public event ReturnSAVEventHandler SaveFileRequested;
        public delegate SaveFile ReturnSAVEventHandler(object sender, EventArgs e);

        private readonly PictureBox[] movePB, relearnPB;
        private readonly ToolTip Tip3 = new ToolTip(), NatureTip = new ToolTip(), SpeciesIDTip = new ToolTip();
        public SaveFile RequestSaveFile => SaveFileRequested?.Invoke(this, EventArgs.Empty);
        public bool PKMIsUnsaved => FieldsInitialized && FieldsLoaded && LastData?.Any(b => b != 0) == true && !LastData.SequenceEqual(CurrentPKM.Data);
        public bool IsEmptyOrEgg => CHK_IsEgg.Checked || CB_Species.SelectedIndex == 0;

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
            PKM pk = GetPKMfromFields();
            return pk?.Clone();
        }

        public bool VerifiedPKM()
        {
            if (ModifierKeys == (Keys.Control | Keys.Shift | Keys.Alt))
                return true; // Override

            var cb = Array.Find(ValidationRequired, c => c.BackColor == InvalidSelection && c.Items.Count != 0);
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

        public void InitializeFields()
        {
            // Now that the ComboBoxes are ready, load the data.
            Stats.SetMainEditor(this);
            FieldsInitialized = true;
            PopulateFields(pkm);
        }

        public void SetPKMFormatMode(int Format)
        {
            byte[] extraBytes = Array.Empty<byte>();
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
                    switch (pkm)
                    {
                        case CK3 _: extraBytes = CK3.ExtraBytes; break;
                        case XK3 _: extraBytes = XK3.ExtraBytes; break;
                        default: extraBytes = PK3.ExtraBytes; break;
                    }
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

        public void PopulateFields(PKM pk, bool focus = true, bool skipConversionCheck = false) => LoadFieldsFromPKM(pk, focus, skipConversionCheck);

        private void LoadFieldsFromPKM(PKM pk, bool focus = true, bool skipConversionCheck = true)
        {
            if (pk == null) { WinFormsUtil.Error(MsgPKMLoadNull); return; }
            if (focus)
                Tab_Main.Focus();

            if (!skipConversionCheck && !PKMConverter.TryMakePKMCompatible(pk, CurrentPKM, out string c, out pk))
            { WinFormsUtil.Alert(c); return; }

            bool oldInit = FieldsInitialized;
            FieldsInitialized = FieldsLoaded = false;

            pkm = pk.Clone();

            try { GetFieldsfromPKM(); }
            finally { FieldsInitialized = oldInit; }

            Stats.UpdateIVs(null, null);
            UpdatePKRSInfected(null, null);
            UpdatePKRSCured(null, null);
            UpdateNatureModification(null, null);

            if (HaX) // Load original values from pk not pkm
            {
                MT_Level.Text = (pk.Stat_HPMax != 0 ? pk.Stat_Level : PKX.GetLevel(pk.Species, pk.EXP)).ToString();
                TB_EXP.Text = pk.EXP.ToString();
                MT_Form.Text = pk.AltForm.ToString();
                if (pk.Stat_HPMax != 0) // stats present
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

            Legality = la ?? new LegalityAnalysis(pkm, RequestSaveFile.Personal);
            if (!Legality.Parsed || HaX || pkm.Species == 0)
            {
                PB_WarnMove1.Visible = PB_WarnMove2.Visible = PB_WarnMove3.Visible = PB_WarnMove4.Visible =
                    PB_WarnRelearn1.Visible = PB_WarnRelearn2.Visible = PB_WarnRelearn3.Visible = PB_WarnRelearn4.Visible = false;
                LegalityChanged?.Invoke(Legality.Valid, null);
                return;
            }

            // Refresh Move Legality
            for (int i = 0; i < 4; i++)
                movePB[i].Visible = !Legality.Info?.Moves[i].Valid ?? false;

            if (pkm.Format >= 6)
            {
                for (int i = 0; i < 4; i++)
                    relearnPB[i].Visible = !Legality.Info?.Relearn[i].Valid ?? false;
            }

            if (skipMoveRepop)
                return;
            // Resort moves
            bool tmp = FieldsLoaded;
            FieldsLoaded = false;
            ReloadMoves(Legality.AllSuggestedMovesAndRelearn);
            FieldsLoaded |= tmp;
            LegalityChanged?.Invoke(Legality.Valid, null);
        }

        private IReadOnlyList<ComboItem> MoveDataAllowed = new List<ComboItem>();

        private void ReloadMoves(IReadOnlyCollection<int> moves)
        {
            // check prior movepool to not needlessly refresh the dataset
            if (AllowedMoves.Count <= moves.Count && moves.All(AllowedMoves.Contains))
                return;

            AllowedMoves = new HashSet<int>(moves);
            MoveDataAllowed = GameInfo.Strings.MoveDataSource.OrderByDescending(m => AllowedMoves.Contains(m.Value)).ToList();

            // defer repop until dropdown is opened; handled by dropdown event
            for (int i = 0; i < IsMoveBoxOrdered.Count; i++)
                IsMoveBoxOrdered[i] = false;
        }

        private void SetMoveDataSource(ComboBox c)
        {
            var index = WinFormsUtil.GetIndex(c);
            c.DataSource = new BindingSource(MoveDataAllowed, null);
            c.SelectedValue = index;
        }

        public void UpdateUnicode(string[] symbols)
        {
            gendersymbols = symbols;
            if (!Unicode)
            {
                BTN_Shinytize.Text = ShinytizeDefault;
                TB_Nickname.Font = TB_OT.Font = TB_OTt2.Font = GB_OT.Font;
            }
            else
            {
                BTN_Shinytize.Text = ShinytizeUnicode;
                TB_Nickname.Font = TB_OT.Font = TB_OTt2.Font = FontUtil.GetPKXFont(11);
            }
            // Switch active gender labels to new if they are active.
            if (PKX.GetGenderFromString(Label_Gender.Text) < 2)
                Label_Gender.Text = gendersymbols[PKX.GetGenderFromString(Label_Gender.Text)];
            if (PKX.GetGenderFromString(Label_OTGender.Text) < 2)
                Label_OTGender.Text = gendersymbols[PKX.GetGenderFromString(Label_OTGender.Text)];
            if (PKX.GetGenderFromString(Label_CTGender.Text) < 2)
                Label_CTGender.Text = gendersymbols[PKX.GetGenderFromString(Label_CTGender.Text)];
        }

        private void UpdateSprite()
        {
            if (FieldsLoaded && FieldsInitialized && !forceValidation)
                UpdatePreviewSprite?.Invoke(this, null);
        }

        // General Use Functions //
        private Color InvalidSelection { get; set; } = Color.DarkSalmon;
        private Color MarkBlue { get; set; } = Color.FromArgb(000, 191, 255);
        private Color MarkPink { get; set; } = Color.FromArgb(255, 117, 179);
        private Color MarkDefault { get; set; } = Color.Black;
        private Brush LegalMove { get; set; } = Brushes.PaleGreen;
        private Brush TextBrush { get; set; }
        private Brush BackBrush { get; set; }
        private string ShinytizeDefault { get; set; } = "*";
        private string ShinytizeUnicode { get; set; } = "☆";
        private Color DrawForeColor { get; set; }

        private Color GetGenderColor(int gender)
        {
            switch (gender)
            {
                case 0: return Color.Blue;
                case 1: return Color.Red;
                default: return DrawForeColor;
            }
        }

        private bool GetMarkingColor(int markval, out Color c)
        {
            switch (markval)
            {
                case 1: c = MarkBlue; return true;
                case 2: c = MarkPink; return true;
                default: c = MarkDefault; return false; // recolor not required
            }
        }

        private void SetDetailsOT(ITrainerInfo SAV)
        {
            if (!(SAV.OT?.Length > 0))
                return;

            // Get Save Information
            TB_OT.Text = SAV.OT;
            Label_OTGender.Text = gendersymbols[SAV.Gender & 1];
            Label_OTGender.ForeColor = GetGenderColor(SAV.Gender & 1);
            TID_Trainer.LoadInfo(SAV);

            if (SAV.Game >= 0)
                CB_GameOrigin.SelectedValue = SAV.Game;
            if (SAV.Language >= 0)
                CB_Language.SelectedValue = SAV.Language;
            if (SAV.ConsoleRegion != 0)
            {
                CB_3DSReg.SelectedValue = SAV.ConsoleRegion;
                CB_Country.SelectedValue = SAV.Country;
                CB_SubRegion.SelectedValue = SAV.SubRegion;
            }

            // Copy OT trash bytes for sensitive games (Gen1/2)
                 if (SAV is SAV1 s1 && pkm is PK1 p1) p1.OT_Trash = s1.OT_Trash;
            else if (SAV is SAV2 s2 && pkm is PK2 p2) p2.OT_Trash = s2.OT_Trash;

            UpdateNickname(null, null);
        }

        private void SetDetailsHT(ITrainerInfo SAV)
        {
            if (!(SAV.OT?.Length > 0))
                return;

            if (TB_OTt2.Text.Length > 0)
                Label_CTGender.Text = gendersymbols[SAV.Gender & 1];
        }

        private void SetForms()
        {
            int species = pkm.Species;
            var pi = RequestSaveFile?.Personal[species] ?? pkm.PersonalInfo;
            bool hasForms = FormConverter.HasFormSelection(pi, species, pkm.Format);
            CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = hasForms;

            if (HaX && pkm.Format >= 4)
                Label_Form.Visible = true; // show with value entry textbox

            if (!hasForms)
                return;

            var ds = PKX.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, gendersymbols, pkm.Format);
            if (ds.Length == 1 && string.IsNullOrEmpty(ds[0])) // empty (Alolan Totems)
                CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = false;
            else
                CB_Form.DataSource = ds;
        }

        private void SetAbilityList()
        {
            if (pkm.Format < 3) // no abilities
                return;

            if (pkm.Format > 3 && FieldsLoaded) // has forms
                pkm.AltForm = CB_Form.SelectedIndex; // update pkm field for form specific abilities

            int abil = CB_Ability.SelectedIndex;

            bool tmp = FieldsLoaded;
            FieldsLoaded = false;
            CB_Ability.InitializeBinding();
            CB_Ability.DataSource = GetAbilityList(pkm);
            CB_Ability.SelectedIndex = GetSafeIndex(CB_Ability, abil); // restore original index if available
            FieldsLoaded = tmp;
        }

        private static int GetSafeIndex(ComboBox cb, int index) => Math.Max(0, Math.Min(cb.Items.Count - 1, index));

        private void UpdateIsShiny()
        {
            // Recalculate shininiess
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
            Image changeOpacity(PictureBox p, double opacity) => opacity == 1 ? p.InitialImage
                : ImageUtil.ChangeOpacity(p.InitialImage, opacity);

            var pba = Markings;
            var markings = pkm.Markings;
            for (int i = 0; i < pba.Length; i++)
                pba[i].Image = changeOpacity(pba[i], getOpacity(markings[i] != 0));

            PB_MarkShiny.Image = changeOpacity(PB_MarkShiny, getOpacity(!BTN_Shinytize.Enabled));
            PB_MarkCured.Image = changeOpacity(PB_MarkCured, getOpacity(CHK_Cured.Checked));

            PB_MarkPentagon.Image = changeOpacity(PB_MarkPentagon, getOpacity(pkm.Gen6));

            // Gen7 Markings
            if (pkm.Format != 7)
                return;

            PB_MarkAlola.Image = changeOpacity(PB_MarkAlola, getOpacity(pkm.Gen7));
            PB_MarkVC.Image = changeOpacity(PB_MarkVC, getOpacity(pkm.VC));
            PB_MarkHorohoro.Image = changeOpacity(PB_MarkHorohoro, getOpacity(pkm.Horohoro));

            for (int i = 0; i < pba.Length; i++)
            {
                if (GetMarkingColor(markings[i], out Color c))
                    pba[i].Image = ImageUtil.ChangeAllColorTo(pba[i].Image, c);
            }
        }

        private void UpdateGender()
        {
            int Gender = pkm.GetSaneGender();
            Label_Gender.Text = gendersymbols[Gender];
            Label_Gender.ForeColor = GetGenderColor(Gender);
        }

        private void SetCountrySubRegion(ComboBox CB, string type)
        {
            int index = CB.SelectedIndex;
            // fix for Korean / Chinese being swapped
            string cl = GameInfo.CurrentLanguage;
            cl = cl == "zh" ? "ko" : cl == "ko" ? "zh" : cl;

            CB.DataSource = Util.GetCBList(type, cl);

            if (index > 0 && index < CB.Items.Count && FieldsInitialized)
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

            int newGender = (PKX.GetGenderFromString(Label_Gender.Text) & 1) ^ 1;
            if (pkm.Format <= 2)
            {
                Stats.SetATKIVGender(newGender);
                UpdateIsShiny();
            }
            else if (pkm.Format <= 4)
            {
                pkm.Version = WinFormsUtil.GetIndex(CB_GameOrigin);
                pkm.Nature = WinFormsUtil.GetIndex(CB_Nature);
                pkm.AltForm = CB_Form.SelectedIndex;

                pkm.SetPIDGender(newGender);
                TB_PID.Text = pkm.PID.ToString("X8");
            }
            pkm.Gender = newGender;
            Label_Gender.Text = gendersymbols[pkm.Gender];
            Label_Gender.ForeColor = GetGenderColor(pkm.Gender);

            if (PKX.GetGenderFromString(CB_Form.Text) < 2) // Gendered Forms
                CB_Form.SelectedIndex = PKX.GetGenderFromString(Label_Gender.Text);

            UpdatePreviewSprite(Label_Gender, null);
        }

        private void ClickPPUps(object sender, EventArgs e)
        {
            bool min = ModifierKeys.HasFlag(Keys.Control);
            CB_PPu1.SelectedIndex = !min && WinFormsUtil.GetIndex(CB_Move1) > 0 ? 3 : 0;
            CB_PPu2.SelectedIndex = !min && WinFormsUtil.GetIndex(CB_Move2) > 0 ? 3 : 0;
            CB_PPu3.SelectedIndex = !min && WinFormsUtil.GetIndex(CB_Move3) > 0 ? 3 : 0;
            CB_PPu4.SelectedIndex = !min && WinFormsUtil.GetIndex(CB_Move4) > 0 ? 3 : 0;
        }

        private void ClickMarking(object sender, EventArgs e)
        {
            int index = Array.IndexOf(Markings, sender);
            pkm.ToggleMarking(index);
            SetMarkings();
        }

        private void ClickOT(object sender, EventArgs e) => SetDetailsOT(SaveFileRequested?.Invoke(this, e));
        private void ClickCT(object sender, EventArgs e) => SetDetailsHT(SaveFileRequested?.Invoke(this, e));

        private void ClickTRGender(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (!string.IsNullOrWhiteSpace(lbl?.Text)) // set gender label (toggle M/F)
            {
                int gender = PKX.GetGenderFromString(lbl.Text) ^ 1;
                lbl.Text = gendersymbols[gender];
                lbl.ForeColor = GetGenderColor(gender);
            }
        }

        private void ClickBall(object sender, EventArgs e)
        {
            pkm.Ball = WinFormsUtil.GetIndex(CB_Ball);
            if (ModifierKeys.HasFlag(Keys.Alt))
            {
                CB_Ball.SelectedValue = (int)Ball.Poke;
                return;
            }
            if (ModifierKeys.HasFlag(Keys.Shift))
            {
                CB_Ball.SelectedValue = BallRandomizer.ApplyBallLegalByColor(pkm);
                return;
            }

            var legal = BallRandomizer.GetLegalBalls(pkm).ToArray();
            var poss = ((Ball[]) Enum.GetValues(typeof(Ball))).Skip(1)
                .TakeWhile(z => (int) z <= pkm.MaxBallID).ToArray();
            var names = GameInfo.BallDataSource;

            var frm = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                StartPosition = FormStartPosition.CenterParent,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MaximizeBox = false,
                MinimizeBox = false,
            };
            var flp = new FlowLayoutPanel {AutoSize = true, Dock = DockStyle.Fill};

            for (int i = 0; i < poss.Length; i++)
            {
                var b = poss[i];
                var img = SpriteUtil.GetBallSprite((int) b);
                bool valid = legal.Contains(b);
                var pb = new PictureBox
                {
                    Size = img.Size,
                    Image = img,
                    BackgroundImage = valid ? Resources.slotSet : Resources.slotDel,
                    BackgroundImageLayout = ImageLayout.Tile
                };
                pb.MouseEnter += (_, __) => frm.Text = names.First(z => z.Value == (int)b).Text;
                pb.Click += (_, __) =>
                {
                    CB_Ball.SelectedValue = (int)b;
                    frm.Close();
                };
                flp.Controls.Add(pb);
                const int width = 5;
                if (i % width == width - 1)
                    flp.SetFlowBreak(pb, true);
            }
            frm.Controls.Add(flp);
            frm.ShowDialog();
        }

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
            int[] m = pkm.GetMoveSet(random);
            if (m?.Any(z => z != 0) != true)
            {
                if (!silent)
                    WinFormsUtil.Alert(MsgPKMSuggestionFormat);
                return false;
            }

            if (pkm.Moves.SequenceEqual(m))
                return false;

            if (!silent)
            {
                var movestrings = m.Select(v => v >= GameInfo.Strings.Move.Count ? MsgProgramError : GameInfo.Strings.Move[v]);
                string r = string.Join(Environment.NewLine, movestrings);
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgPKMSuggestionMoves, r))
                    return false;
            }

            pkm.SetMoves(m);
            FieldsLoaded = false;
            LoadMoves(pkm);
            FieldsLoaded = true;
            return true;
        }

        private bool SetSuggestedRelearnMoves(bool silent = false)
        {
            if (pkm.Format < 6)
                return false;
            int[] m = pkm.GetSuggestedRelearnMoves(Legality);

            if (pkm.RelearnMoves.SequenceEqual(m))
                return false;

            if (!silent)
            {
                var movestrings = m.Select(v => v >= GameInfo.Strings.Move.Count ? MsgProgramError : GameInfo.Strings.Move[v]);
                string r = string.Join(Environment.NewLine, movestrings);
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgPKMSuggestionRelearn, r))
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
            if (encounter == null || (pkm.Format >= 3 && encounter.Location < 0))
            {
                if (!silent)
                    WinFormsUtil.Alert(MsgPKMSuggestionNone);
                return false;
            }

            int level = encounter.Level;
            int location = encounter.Location;
            int minlvl = Legal.GetLowestLevel(pkm, encounter.LevelMin);
            if (minlvl == 0)
                minlvl = level;

            if (pkm.CurrentLevel >= minlvl && pkm.Met_Level == level && pkm.Met_Location == location)
                return false;
            if (minlvl < level)
                minlvl = level;

            if (!silent)
            {
                List<string> suggestion = GetSuggestionMessage(pkm, level, location, minlvl);
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

                if (pkm.GenNumber == 6 && pkm.WasEgg && ModifyPKM)
                    pkm.SetHatchMemory6();
            }

            if (pkm.CurrentLevel < minlvl)
                TB_Level.Text = minlvl.ToString();

            return true;
        }

        public void UpdateIVsGB(bool skipForm)
        {
            if (!FieldsLoaded)
                return;
            Label_Gender.Text = gendersymbols[pkm.Gender];
            Label_Gender.ForeColor = GetGenderColor(pkm.Gender);
            if (pkm.Species == 201 && !skipForm) // Unown
                CB_Form.SelectedIndex = pkm.AltForm;

            UpdateIsShiny();
            UpdateSprite();
        }

        private void UpdateBall(object sender, EventArgs e)
        {
            PB_Ball.Image = SpriteUtil.GetBallSprite(WinFormsUtil.GetIndex(CB_Ball));
        }

        private void UpdateEXPLevel(object sender, EventArgs e)
        {
            if (ChangingFields || !FieldsInitialized)
                return;
            ChangingFields = true;
            if (sender == TB_EXP)
            {
                // Change the Level
                uint EXP = Util.ToUInt32(TB_EXP.Text);
                int Species = pkm.Species;
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
                    TB_EXP.Text = PKX.GetEXP(Level, pkm.Species).ToString();
            }
            ChangingFields = false;
            if (FieldsLoaded) // store values back
            {
                pkm.EXP = Util.ToUInt32(TB_EXP.Text);
                pkm.Stat_Level = Util.ToInt32((HaX ? MT_Level : TB_Level).Text);
            }
            Stats.UpdateStats();
            UpdateLegality();
        }

        private void UpdateRandomPID(object sender, EventArgs e)
        {
            if (pkm.Format < 3)
                return;
            if (FieldsLoaded)
                pkm.PID = Util.GetHexValue(TB_PID.Text);

            if (sender == Label_Gender)
                pkm.SetPIDGender(pkm.Gender);
            else if (sender == CB_Nature && pkm.Nature != WinFormsUtil.GetIndex(CB_Nature))
                pkm.SetPIDNature(WinFormsUtil.GetIndex(CB_Nature));
            else if (sender == BTN_RerollPID)
                pkm.SetPIDGender(pkm.Gender);
            else if (sender == CB_Ability && CB_Ability.SelectedIndex != pkm.PIDAbility && pkm.PIDAbility > -1)
                pkm.SetAbilityIndex(CB_Ability.SelectedIndex);

            TB_PID.Text = pkm.PID.ToString("X8");
            if (pkm.Format >= 6 && 3 <= pkm.GenNumber && pkm.GenNumber <= 5)
                TB_EC.Text = TB_PID.Text;
            Update_ID(TB_EC, e);
        }

        private void UpdateRandomEC(object sender, EventArgs e)
        {
            if (pkm.Format < 6)
                return;

            pkm.SetRandomEC();
            TB_EC.Text = pkm.EncryptionConstant.ToString("X8");
            Update_ID(TB_EC, e);
            UpdateLegality();
        }

        private void Update255_MTB(object sender, EventArgs e)
        {
            if (!(sender is MaskedTextBox tb))
                return;
            if (Util.ToInt32(tb.Text) > byte.MaxValue)
                tb.Text = "255";
        }

        private void UpdateForm(object sender, EventArgs e)
        {
            if (CB_Form == sender && FieldsLoaded)
                pkm.AltForm = CB_Form.SelectedIndex;

            Stats.UpdateStats();
            SetAbilityList();

            // Gender Forms
            if (WinFormsUtil.GetIndex(CB_Species) == 201 && FieldsLoaded)
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
                    {
                        FieldsLoaded = false;
                        Stats.UpdateRandomIVs(null, null);
                        FieldsLoaded = true;
                    }
                }
            }
            else if (CB_Form.Enabled && PKX.GetGenderFromString(CB_Form.Text) < 2)
            {
                if (CB_Form.Items.Count == 2) // actually M/F; Pumpkaboo formes in German are S,M,L,XL
                {
                    pkm.Gender = CB_Form.SelectedIndex;
                    UpdateGender();
                }
            }
            else
            {
                UpdateGender();
            }

            if (ChangingFields)
                return;
            ChangingFields = true;
            MT_Form.Text = CB_Form.SelectedIndex.ToString();
            ChangingFields = false;

            UpdateSprite();
        }

        private void UpdateHaXForm(object sender, EventArgs e)
        {
            if (ChangingFields)
                return;
            ChangingFields = true;
            int form = pkm.AltForm = Util.ToInt32(MT_Form.Text);
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
                MovePP[index].Text = pkm.GetMovePP(move, ppups).ToString();
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
            if (!FieldsInitialized)
                return;
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
            if (!FieldsInitialized)
                return;
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
                pkm.Species = WinFormsUtil.GetIndex(CB_Species);
            SpeciesIDTip.SetToolTip(CB_Species, pkm.Species.ToString("000"));
            SetAbilityList();
            SetForms();
            UpdateForm(null, null);

            if (!FieldsLoaded)
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
            if (FieldsLoaded)
                pkm.Version = (int)Version;

            // check if differs
            var group = GameUtil.GetMetLocationVersionGroup(Version);
            if (group == GameVersion.GSC && pkm.Format >= 7)
                group = GameVersion.USUM;
            else if (pkm.Format < 3)
                group = GameVersion.GSC;

            if (group != origintrack)
                ReloadMetLocations(Version);
            origintrack = group;

            // Visibility logic for Gen 4 encounter type; only show for Gen 4 Pokemon.
            if (pkm.Format >= 4)
            {
                bool g4 = pkm.Gen4;
                CB_EncounterType.Visible = Label_EncounterType.Visible = g4 && pkm.Format < 7;
                if (!g4)
                    CB_EncounterType.SelectedValue = 0;
            }

            if (!FieldsLoaded)
                return;

            TID_Trainer.LoadIDValues(pkm);
            UpdateLegality();
        }

        private void ReloadMetLocations(GameVersion Version)
        {
            CB_MetLocation.InitializeBinding();
            CB_EggLocation.InitializeBinding();

            var met_list = GameInfo.GetLocationList(Version, pkm.Format, egg: false);
            CB_MetLocation.DataSource = new BindingSource(met_list, null);

            var egg_list = GameInfo.GetLocationList(Version, pkm.Format, egg: true);
            CB_EggLocation.DataSource = new BindingSource(egg_list, null);

            // Stretch C/XD met location dropdowns
            int width = CB_EggLocation.DropDownWidth;
            if (Version == GameVersion.CXD && pkm.Format == 3)
                width *= 2;
            CB_MetLocation.DropDownWidth = width;

            if (FieldsLoaded)
            {
                SetMarkings(); // Set/Remove the Nativity marking when gamegroup changes too
                int metLoc = EncounterSuggestion.GetSuggestedTransferLocation(pkm);
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
            string text = Stats.UpdateNatureModification(pkm.Nature);
            NatureTip.SetToolTip(CB_Nature, text);
        }

        private void UpdateIsNicknamed(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
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

            if (!FieldsInitialized || CHK_Nicknamed.Checked)
                return;

            // Fetch Current Species and set it as Nickname Text
            int species = WinFormsUtil.GetIndex(CB_Species);
            if (species < 1 || species > pkm.MaxSpeciesID)
            { TB_Nickname.Text = string.Empty; return; }

            if (CHK_IsEgg.Checked)
                species = 0; // get the egg name.

            // If name is that of another language, don't replace the nickname
            if (sender != CB_Language && species != 0 && !PKX.IsNicknamedAnyLanguage(species, TB_Nickname.Text, pkm.Format))
                return;

            TB_Nickname.Text = PKX.GetSpeciesNameGeneration(species, lang, pkm.Format);
            if (pkm is _K12 pk)
                pk.SetNotNicknamed();
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
                Label_CTGender.Text = string.Empty;
                TB_Friendship.Text = pkm.CurrentFriendship.ToString();
            }
            else if (string.IsNullOrWhiteSpace(Label_CTGender.Text))
            {
                Label_CTGender.Text = gendersymbols[0];
            }
        }

        private void UpdateIsEgg(object sender, EventArgs e)
        {
            // Display hatch counter if it is an egg, Display Friendship if it is not.
            Label_HatchCounter.Visible = CHK_IsEgg.Checked && pkm.Format > 1;
            Label_Friendship.Visible = !CHK_IsEgg.Checked && pkm.Format > 1;

            if (!FieldsLoaded)
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
                    isTraded = SAV.OT != TB_OT.Text || SAV.TID != pkm.TID || SAV.SID != pkm.SID;
                CB_MetLocation.SelectedIndex = isTraded ? 2 : 0;

                if (!CHK_Nicknamed.Checked)
                {
                    TB_Nickname.Text = PKX.GetSpeciesNameGeneration(0, WinFormsUtil.GetIndex(CB_Language), pkm.Format);
                    if (pkm.Format != 4) // eggs in gen4 do not have nickname flag
                        CHK_Nicknamed.Checked = true;
                }

                // Wipe egg memories
                if (pkm.Format >= 6 && ModifyPKM)
                    pkm.ClearMemories();
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
            var ShinyPID = pkm.Format <= 2 || ModifierKeys != Keys.Control;
            UpdateShiny(ShinyPID);
        }

        private void UpdateShiny(bool PID)
        {
            pkm.PID = Util.GetHexValue(TB_PID.Text);
            pkm.Nature = WinFormsUtil.GetIndex(CB_Nature);
            pkm.Gender = PKX.GetGenderFromString(Label_Gender.Text);
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
                    TID_Trainer.UpdateSID();
                }
            }
            else
            {
                pkm.SetShinyIVs();
                Stats.LoadIVs(pkm.IVs);
                Stats.UpdateIVs(null, null);
            }

            UpdateIsShiny();
            UpdatePreviewSprite?.Invoke(this, null);
            UpdateLegality();
        }

        private void UpdateTSV(object sender, EventArgs e)
        {
            if (pkm.Format < 6)
                return;

            TID_Trainer.UpdateTSV();

            pkm.PID = Util.GetHexValue(TB_PID.Text);
            Tip3.SetToolTip(TB_PID, $"PSV: {pkm.PSV:d4}");
        }

        private void Update_ID(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;
            // Trim out nonhex characters
            TB_PID.Text = (pkm.PID = Util.GetHexValue(TB_PID.Text)).ToString("X8");
            TB_EC.Text = (pkm.EncryptionConstant = Util.GetHexValue(TB_EC.Text)).ToString("X8");

            UpdateIsShiny();
            UpdateSprite();
            Stats.UpdateCharacteristic();   // If the EC is changed, EC%6 (Characteristic) might be changed.
            if (pkm.Format <= 4)
            {
                FieldsLoaded = false;
                pkm.PID = Util.GetHexValue(TB_PID.Text);
                CB_Nature.SelectedValue = pkm.Nature;
                Label_Gender.Text = gendersymbols[pkm.Gender];
                Label_Gender.ForeColor = GetGenderColor(pkm.Gender);
                FieldsLoaded = true;
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
            FieldsLoaded = true;
        }

        private void ValidateComboBox(object sender)
        {
            if (!FieldsInitialized)
                return;
            if (!(sender is ComboBox cb))
                return;

            if (cb.Text.Length == 0 && cb.Items.Count > 0)
                cb.SelectedIndex = 0;
            else if (cb.SelectedValue == null)
                cb.BackColor = InvalidSelection;
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
            if (!FieldsInitialized || !FieldsLoaded)
                return;

            ValidateComboBox(sender, null);
            if (sender == CB_Ability)
            {
                if (pkm.Format >= 6)
                    TB_AbilityNumber.Text = (1 << CB_Ability.SelectedIndex).ToString();
                else if (pkm.Format <= 5 && CB_Ability.SelectedIndex < 2) // Format <= 5, not hidden
                    UpdateRandomPID(sender, e);
                UpdateLegality();
            }
            else if (sender == CB_Nature)
            {
                if (pkm.Format <= 4)
                    UpdateRandomPID(sender, e);
                pkm.Nature = WinFormsUtil.GetIndex(CB_Nature);
                UpdateNatureModification(sender, EventArgs.Empty);
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
            if (!FieldsInitialized || !FieldsLoaded)
                return;

            ValidateComboBox(sender);
            if (Moves.Contains(sender)) // Move
                UpdatePP(sender, e);

            // Legality
            pkm.Moves = Moves.Select(WinFormsUtil.GetIndex).ToArray();
            pkm.RelearnMoves = Relearn.Select(WinFormsUtil.GetIndex).ToArray();
            UpdateLegality(skipMoveRepop: true);
        }

        private void ValidateMovePaint(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            var i = (ComboItem)((ComboBox)sender).Items[e.Index];
            bool valid = AllowedMoves.Contains(i.Value) && !HaX;

            bool current = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Brush tBrush = current ? SystemBrushes.HighlightText : TextBrush;
            Brush brush = current ? SystemBrushes.Highlight : valid ? LegalMove : BackBrush;

            e.Graphics.FillRectangle(brush, e.Bounds);
            e.Graphics.DrawString(i.Text, e.Font, tBrush, e.Bounds, StringFormat.GenericDefault);
        }

        private readonly IList<bool> IsMoveBoxOrdered = new bool[4];

        private void ValidateMoveDropDown(object sender, EventArgs e)
        {
            var s = (ComboBox) sender;
            var index = Array.IndexOf(Moves, s);
            if (IsMoveBoxOrdered[index])
                return;
            SetMoveDataSource(s);
            IsMoveBoxOrdered[index] = true;
        }

        private void ValidateLocation(object sender, EventArgs e)
        {
            ValidateComboBox(sender);
            if (!FieldsLoaded)
                return;

            pkm.Met_Location = WinFormsUtil.GetIndex(CB_MetLocation);
            pkm.Egg_Location = WinFormsUtil.GetIndex(CB_EggLocation);
            UpdateLegality();
        }

        // Secondary Windows for Ribbons/Amie/Memories
        private void OpenRibbons(object sender, EventArgs e) => new RibbonEditor(pkm).ShowDialog();
        private void OpenMedals(object sender, EventArgs e) => new SuperTrainingEditor(pkm).ShowDialog();

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
        /// <param name="sav">Save File context the editor is editing for</param>
        /// <param name="pk">Pokémon data to edit</param>
        public bool ToggleInterface(SaveFile sav, PKM pk)
        {
            pkm = GetCompatiblePKM(sav, pk);
            ToggleInterface(pkm);
            return FinalizeInterface(sav);
        }

        private void ToggleInterface(PKM t)
        {
            FLP_Purification.Visible = FLP_ShadowID.Visible = t is IShadowPKM;
            ToggleInterface(pkm.Format);
        }

        private void ToggleSecrets(bool hidden, int gen)
        {
            Label_EncryptionConstant.Visible = BTN_RerollEC.Visible = TB_EC.Visible = gen >= 6 && !hidden;
            BTN_RerollPID.Visible = Label_PID.Visible = TB_PID.Visible = gen >= 3 && !hidden;
        }

        private void ToggleInterface(int gen)
        {
            ToggleSecrets(HideSecretValues, gen);
            FLP_Country.Visible = FLP_SubRegion.Visible = FLP_3DSRegion.Visible = gen >= 6;
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
            GB_ExtraBytes.Visible = GB_ExtraBytes.Enabled = gen >= 3;
            GB_Markings.Visible = gen >= 3;
            BTN_Ribbons.Visible = gen >= 3;
            CB_Form.Enabled = gen >= 3;

            FLP_FriendshipForm.Visible = gen >= 2;
            FLP_HeldItem.Visible = gen >= 2;
            CHK_IsEgg.Visible = gen >= 2;
            FLP_PKRS.Visible = FLP_EggPKRSRight.Visible = gen >= 2;
            Label_OTGender.Visible = gen >= 2;
            Label_Gender.Visible = gen >= 1;

            // HaX override, needs to be after DEV_Ability enabled assignment.
            TB_AbilityNumber.Visible = gen >= 6 && DEV_Ability.Enabled;

            // Met Tab
            FLP_MetDate.Visible = gen >= 4;
            FLP_Fateful.Visible = FLP_Ball.Visible = FLP_OriginGame.Visible = gen >= 3;
            FLP_MetLocation.Visible = FLP_MetLevel.Visible = gen >= 2;
            FLP_EncounterType.Visible = gen >= 4 && gen <= 6;
            FLP_TimeOfDay.Visible = gen == 2;

            Contest.ToggleInterface(pkm, gen);
            Stats.ToggleInterface(gen);

            CenterSubEditors();
        }

        private bool FinalizeInterface(SaveFile sav)
        {
            bool init = FieldsInitialized;
            FieldsInitialized = FieldsLoaded = false;

            bool TranslationRequired = false;
            PopulateFilteredDataSources(sav);
            PopulateFields(pkm);
            FieldsInitialized |= init;

            // SAV Specific Limits
            TB_OT.MaxLength = pkm.OTLength;
            TB_OTt2.MaxLength = pkm.OTLength;
            TB_Nickname.MaxLength = pkm.NickLength;

            // Hide Unused Tabs
            if (pkm.Format == 1 && tabMain.TabPages.Contains(Tab_Met))
            {
                tabMain.TabPages.Remove(Tab_Met);
            }
            else if (pkm.Format != 1 && !tabMain.TabPages.Contains(Tab_Met))
            {
                tabMain.TabPages.Insert(1, Tab_Met);
                TranslationRequired = true;
            }

            // Common HaX Interface
            MT_Level.Enabled = MT_Level.Visible = MT_Form.Enabled = MT_Form.Visible = HaX;
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
            FLP_PKMEditors.Location = new Point((tabMain.TabPages[0].Width - FLP_PKMEditors.Width) / 2, FLP_PKMEditors.Location.Y);
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
            var info = new SimpleTrainerInfo{Language = Math.Max(0, WinFormsUtil.GetIndex(CB_Language))};
            if (info.Language == 0)
                info.Language = (int)LanguageID.English;
            SetDetailsOT(info);

            CB_Ball.SelectedIndex = Math.Min(0, CB_Ball.Items.Count - 1);
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

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Action<ShowdownSet> LoadShowdownSet;

        private void LoadShowdownSetDefault(ShowdownSet Set)
        {
            var pk = PreparePKM();
            pk.ApplySetDetails(Set);
            PopulateFields(pk);
        }

        public void ChangeLanguage(SaveFile sav, PKM pk)
        {
            // Force an update to the met locations
            origintrack = GameVersion.Invalid;

            bool alreadyInit = FieldsInitialized;
            FieldsInitialized = false;
            InitializeLanguage(sav);
            CenterSubEditors();
            PopulateFields(pk); // put data back in form
            FieldsInitialized |= alreadyInit;
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
                CB_Nature, CB_EncounterType, CB_GameOrigin,
            };
            foreach (var cb in cbs)
                cb.InitializeBinding();

            // Set the various ComboBox DataSources up with their allowed entries
            SetCountrySubRegion(CB_Country, "countries");
            CB_3DSReg.DataSource = Util.GetUnsortedCBList("regions3ds");

            CB_EncounterType.DataSource = new BindingSource(GameInfo.EncounterTypeDataSource, null);
            CB_Nature.DataSource = new BindingSource(GameInfo.NatureDataSource, null);

            // Sub editors
            Stats.InitializeDataSources();

            PopulateFilteredDataSources(SAV);
        }

        private void PopulateFilteredDataSources(SaveFile SAV)
        {
            GameInfo.Strings.SetItemDataSource(SAV.Version, SAV.Generation, SAV.MaxItemID, SAV.HeldItems, HaX);
            if (SAV.Generation > 1)
                CB_HeldItem.DataSource = new BindingSource(GameInfo.ItemDataSource.Where(i => i.Value <= SAV.MaxItemID).ToList(), null);

            CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);

            CB_Ball.DataSource = new BindingSource(GameInfo.BallDataSource.Where(b => b.Value <= SAV.MaxBallID).ToList(), null);
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList(), null);
            DEV_Ability.DataSource = new BindingSource(GameInfo.AbilityDataSource.Where(a => a.Value <= SAV.MaxAbilityID).ToList(), null);
            var gamelist = GameUtil.GetVersionsWithinRange(SAV, SAV.Generation).ToList();
            CB_GameOrigin.DataSource = new BindingSource(GameInfo.VersionDataSource.Where(g => gamelist.Contains((GameVersion)g.Value)).ToList(), null);

            // Set the Move ComboBoxes too..
            MoveDataAllowed = GameInfo.Strings.MoveDataSource = (HaX ? GameInfo.HaXMoveDataSource : GameInfo.LegalMoveDataSource).Where(m => m.Value <= SAV.MaxMoveID).ToList(); // Filter Z-Moves if appropriate
            foreach (var cb in Moves.Concat(Relearn))
            {
                cb.InitializeBinding();
                cb.DataSource = new BindingSource(GameInfo.MoveDataSource, null);
            }
        }

        private static List<string> GetSuggestionMessage(PKM pkm, int level, int location, int minlvl)
        {
            var suggestion = new List<string> { MsgPKMSuggestionStart };
            if (pkm.Format >= 3)
            {
                var met_list = GameInfo.GetLocationList((GameVersion)pkm.Version, pkm.Format, egg: false);
                var locstr = met_list.First(loc => loc.Value == location).Text;
                suggestion.Add($"{MsgPKMSuggestionMetLocation} {locstr}");
                suggestion.Add($"{MsgPKMSuggestionMetLevel} {level}");
            }
            if (pkm.CurrentLevel < minlvl)
                suggestion.Add($"{MsgPKMSuggestionLevel} {minlvl}");
            return suggestion;
        }

        private static IReadOnlyList<ComboItem> GetAbilityList(PKM pkm)
        {
            var abils = pkm.PersonalInfo.Abilities;
            return GameInfo.GetAbilityList(abils, pkm.Format);
        }

        private static PKM GetCompatiblePKM(SaveFile sav, PKM current)
        {
            if (current.Format < 3 || current.GetType() != sav.PKMType)
                return sav.BlankPKM;
            return current;
        }
    }
}
