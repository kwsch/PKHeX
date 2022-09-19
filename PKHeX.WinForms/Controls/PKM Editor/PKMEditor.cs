using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.Drawing.PokeSprite;
using PKHeX.Drawing.PokeSprite.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls;

public sealed partial class PKMEditor : UserControl, IMainEditor
{
    public bool IsInitialized { get; private set; }

    public PKMEditor()
    {
        InitializeComponent();

        // Groupbox doesn't show Click event in Designer...
        GB_OT.Click += ClickGT;
        GB_nOT.Click += ClickGT;
        GB_CurrentMoves.Click += ClickMoves;
        GB_RelearnMoves.Click += ClickMoves;

        var font = FontUtil.GetPKXFont();
        TB_Nickname.Font = TB_OT.Font = TB_HT.Font = font;

        // Commonly reused Control arrays
        Moves = new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
        Relearn = new[] { CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 };
        PPUps = new[] { CB_PPu1, CB_PPu2, CB_PPu3, CB_PPu4 };
        MovePP = new[] { TB_PP1, TB_PP2, TB_PP3, TB_PP4 };
        Markings = new[] { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };

        // Legality Indicators
        relearnPB = new[] { PB_WarnRelearn1, PB_WarnRelearn2, PB_WarnRelearn3, PB_WarnRelearn4 };
        movePB = new[] { PB_WarnMove1, PB_WarnMove2, PB_WarnMove3, PB_WarnMove4 };

        // Validation of incompletely entered data fields
        bool Criteria(Control c) => c.BackColor == Draw.InvalidSelection && c is ComboBox { Items.Count: not 0 };
        ValidatedControls = new ValidationRequiredSet[]
        {
            new(Moves, _ => true, Criteria),
            new(new[] {CB_Species}, _ => true, Criteria),
            new(new[] {CB_HeldItem}, pk => pk.Format >= 2, Criteria),
            new(new[] {CB_Ability, CB_Nature, CB_MetLocation, CB_Ball}, pk => pk.Format >= 3, Criteria),
            new(new[] {CB_EggLocation}, pk => pk.Format >= 4, Criteria),
            new(new[] {CB_Country, CB_SubRegion}, pk => pk is PK6 or PK7, Criteria),
            new(Relearn, pk => pk.Format >= 6, Criteria),
            new(new[] {CB_StatNature}, pk => pk.Format >= 8, Criteria),
            new(new[] {CB_AlphaMastered}, pk => pk is PA8, Criteria),
        };

        foreach (var c in WinFormsUtil.GetAllControlsOfType<ComboBox>(this))
            c.KeyDown += WinFormsUtil.RemoveDropCB;

        Stats.MainEditor = this;
        LoadShowdownSet = LoadShowdownSetDefault;
        TID_Trainer.UpdatedID += Update_ID;

        // Controls contained in a TabPage are not created until the tab page is shown
        // Any data bindings in these controls are not activated until the tab page is shown.
        FlickerInterface();
    }

    private sealed class ValidationRequiredSet
    {
        private readonly Control[] Controls;
        private readonly Func<PKM, bool> ShouldCheck;
        private readonly Func<Control, bool> IsInvalidState;

        public Control? IsNotValid(PKM pk)
        {
            if (!ShouldCheck(pk))
                return null;
            return Array.Find(Controls, z => IsInvalidState(z));
        }

        public ValidationRequiredSet(Control[] controls, Func<PKM, bool> shouldCheck, Func<Control, bool> state)
        {
            Controls = controls;
            ShouldCheck = shouldCheck;
            IsInvalidState = state;
        }
    }

    public void InitializeBinding()
    {
        ComboBox[] cbs =
        {
            CB_Nature, CB_StatNature,
            CB_Country, CB_SubRegion, CB_3DSReg, CB_Language, CB_Ball, CB_HeldItem, CB_Species, DEV_Ability,
            CB_GroundTile, CB_GameOrigin, CB_BattleVersion, CB_Ability, CB_MetLocation, CB_EggLocation, CB_Language, CB_HTLanguage,
            CB_AlphaMastered,
        };
        foreach (var cb in cbs.Concat(Moves.Concat(Relearn)))
            cb.InitializeBinding();

        IsInitialized = true;
    }

    private void UpdateStats()
    {
        Stats.UpdateStats();
        if (Entity is IScaledSizeAbsolute)
            SizeCP.TryResetStats();
    }

    private void LoadPartyStats(PKM pk) => Stats.LoadPartyStats(pk);

    private void SavePartyStats(PKM pk) => Stats.SavePartyStats(pk);

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
            ToggleSecrets(_hideSecret, sav.Generation);
        }
    }

    public DrawConfig Draw { private get; set; } = null!;
    public bool Unicode { get; set; } = true;
    private bool _hax;
    public bool HaX { get => _hax; set => _hax = Stats.HaX = value; }
    private byte[] LastData = Array.Empty<byte>();

    public PKM Data { get => Entity; set => Entity = value; }
    public PKM Entity { get; private set; } = null!;
    public bool FieldsLoaded { get; private set; }
    public bool ChangingFields { get; set; }

    /// <summary>
    /// Currently loaded met location group that is populating Met and Egg location comboboxes
    /// </summary>
    private GameVersion origintrack;

    private EntityContext originFormat = EntityContext.None;

    /// <summary>
    /// Action to perform when loading a PKM to the editor GUI.
    /// </summary>
    private Action GetFieldsfromPKM = null!;

    /// <summary>
    /// Function that returns a <see cref="PKM"/> from the loaded fields.
    /// </summary>
    private Func<PKM> GetPKMfromFields = null!;

    /// <summary>
    /// Latest legality check result used to show legality indication.
    /// </summary>
    private LegalityAnalysis Legality = null!;

    /// <summary>
    /// List of legal moves for the latest <see cref="Legality"/>.
    /// </summary>
    private readonly LegalMoveSource<ComboItem> LegalMoveSource = new(new LegalMoveComboSource());

    /// <summary>
    /// Gender Symbols for showing Genders
    /// </summary>
    private IReadOnlyList<string> gendersymbols = GameInfo.GenderSymbolUnicode;

    public event EventHandler? LegalityChanged;
    public event EventHandler? UpdatePreviewSprite;
    public event EventHandler? RequestShowdownImport;
    public event EventHandler? RequestShowdownExport;
    public event ReturnSAVEventHandler SaveFileRequested = null!;
    public delegate SaveFile ReturnSAVEventHandler(object sender, EventArgs e);

    private readonly PictureBox[] movePB, relearnPB;
    public SaveFile RequestSaveFile => SaveFileRequested.Invoke(this, EventArgs.Empty);
    public bool PKMIsUnsaved => FieldsLoaded && LastData.Any(b => b != 0) && !LastData.SequenceEqual(CurrentPKM.Data);

    private readonly ComboBox[] Moves, Relearn, PPUps;
    private readonly ValidationRequiredSet[] ValidatedControls;
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

            // If any controls are partially filled out, find the first one so we can indicate as such.
            Control? cb = null;
            foreach (var type in ValidatedControls)
            {
                cb = type.IsNotValid(Entity);
                if (cb is not null)
                    break;
            }

            if (cb != null)
                tabMain.SelectedTab = WinFormsUtil.FindFirstControlOfType<TabPage>(cb);
            else if (!Stats.Valid)
                tabMain.SelectedTab = Tab_Stats;
            else if (WinFormsUtil.GetIndex(CB_Species) == 0 && !HaX) // can't set an empty slot...
                tabMain.SelectedTab = Tab_Main;
            else
                return true;

            System.Media.SystemSounds.Exclamation.Play();
            return false;
        }
    }

    public void SetPKMFormatMode(PKM pk)
    {
        // Load Extra Byte List
        SetPKMFormatExtraBytes(pk);
        (GetFieldsfromPKM, GetPKMfromFields) = GetLoadSet(pk);
    }

    private (Action Load, Func<PKM> Set) GetLoadSet(PKM pk) => GetLoadSet(pk.Context);

    private (Action Load, Func<PKM> Set) GetLoadSet(EntityContext context) => context switch
    {
        EntityContext.Gen1 => (PopulateFieldsPK1, PreparePK1),
        EntityContext.Gen2 => (PopulateFieldsPK2, PreparePK2),
        EntityContext.Gen3 => (PopulateFieldsPK3, PreparePK3),
        EntityContext.Gen4 => (PopulateFieldsPK4, PreparePK4),
        EntityContext.Gen5 => (PopulateFieldsPK5, PreparePK5),
        EntityContext.Gen6 => (PopulateFieldsPK6, PreparePK6),
        EntityContext.Gen7 => (PopulateFieldsPK7, PreparePK7),
        EntityContext.Gen8 => (PopulateFieldsPK8, PreparePK8),

        EntityContext.Gen7b => (PopulateFieldsPB7, PreparePB7),
        EntityContext.Gen8a => (PopulateFieldsPA8, PreparePA8),
        EntityContext.Gen8b => (PopulateFieldsPB8, PreparePB8),
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

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
        if (focus)
            Tab_Main.Focus();

        if (!skipConversionCheck && !EntityConverter.TryMakePKMCompatible(pk, Entity, out var c, out pk))
        {
            var msg = c.GetDisplayString(pk, Entity.GetType());
            WinFormsUtil.Alert(msg);
            return;
        }

        FieldsLoaded = false;

        Entity = pk.Clone();

#if !DEBUG
            try { GetFieldsfromPKM(); }
            catch { }
#else
        GetFieldsfromPKM();
#endif

        Stats.UpdateIVs(this, EventArgs.Empty);
        UpdatePKRSInfected(this, EventArgs.Empty);
        UpdatePKRSCured(this, EventArgs.Empty);
        UpdateNatureModification(CB_StatNature, Entity.StatNature);

        if (HaX)
        {
            if (pk.PartyStatsPresent) // stats present
                Stats.LoadPartyStats(pk);
        }
        FieldsLoaded = true;

        SetMarkings();
        UpdateLegality();
        UpdateSprite();
        LastData = PreparePKM().Data;
    }

    public void UpdateLegality(LegalityAnalysis? la = null, bool skipMoveRepop = false)
    {
        if (!FieldsLoaded)
            return;

        Legality = la ?? new LegalityAnalysis(Entity, RequestSaveFile.Personal);
        if (!Legality.Parsed || HaX || Entity.Species == 0)
        {
            PB_WarnMove1.Visible = PB_WarnMove2.Visible = PB_WarnMove3.Visible = PB_WarnMove4.Visible = false;
            PB_WarnRelearn1.Visible = PB_WarnRelearn2.Visible = PB_WarnRelearn3.Visible = PB_WarnRelearn4.Visible = false;
            LegalityChanged?.Invoke(Legality.Valid, EventArgs.Empty);
            return;
        }

        // Refresh Move Legality
        var info = Legality.Info;
        var moves = info.Moves;
        for (int i = 0; i < 4; i++)
        {
            var pb = movePB[i];
            pb.Visible = true;
            pb.Image = GetMoveImage(!moves[i].Valid, Entity, i);
        }

        if (Entity.Format >= 6)
        {
            var relearn = info.Relearn;
            for (int i = 0; i < 4; i++)
                relearnPB[i].Visible = !relearn[i].Valid;
        }

        if (skipMoveRepop)
            return;
        // Resort moves
        FieldsLoaded = false;
        LegalMoveSource.ReloadMoves(Legality);
        FieldsLoaded = true;
        LegalityChanged?.Invoke(Legality.Valid, EventArgs.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Bitmap? GetMoveImage(bool isIllegal, PKM pk, int index)
    {
        if (isIllegal)
            return Resources.warn;
        if (pk.Format >= 8 && MoveInfo.GetDummiedMovesHashSet(pk.Context).Contains(pk.GetMove(index)))
            return Resources.hint;
        return null;
    }

    public void UpdateUnicode(IReadOnlyList<string> symbols)
    {
        gendersymbols = symbols;
        if (!Unicode)
        {
            BTN_Shinytize.Text = Draw.ShinyDefault;
            TB_Nickname.Font = TB_OT.Font = TB_HT.Font = GB_OT.Font;
        }
        else
        {
            BTN_Shinytize.Text = Draw.ShinyUnicode;
            TB_Nickname.Font = TB_OT.Font = TB_HT.Font = FontUtil.GetPKXFont();
        }
    }

    internal void UpdateSprite()
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
        UC_OTGender.Gender = tr.Gender & 1;
        TID_Trainer.LoadInfo(tr);

        if (tr.Game >= 0)
            CB_GameOrigin.SelectedValue = tr.Game;

        var lang = tr.Language;
        if (lang <= 0)
            lang = (int)LanguageID.English;
        CB_Language.SelectedValue = lang;
        if (tr is IRegionOrigin o)
        {
            CB_3DSReg.SelectedValue = (int)o.ConsoleRegion;
            CB_Country.SelectedValue = (int)o.Country;
            CB_SubRegion.SelectedValue = (int)o.Region;
        }

        // Copy OT trash bytes for sensitive games (Gen1/2)
        if (tr is SAV1 s1 && Entity is PK1 p1) s1.OT_Trash.CopyTo(p1.OT_Trash);
        else if (tr is SAV2 s2 && Entity is PK2 p2) s2.OT_Trash.CopyTo(p2.OT_Trash);

        UpdateNickname(this, EventArgs.Empty);
    }

    private void SetDetailsHT(ITrainerInfo tr)
    {
        var trainer = tr.OT;
        if (trainer.Length == 0)
            return;

        if (!tr.IsOriginalHandler(Entity, false))
        {
            TB_HT.Text = trainer;
            UC_HTGender.Gender = tr.Gender & 1;
            if (Entity is IHandlerLanguage)
                CB_HTLanguage.SelectedValue = tr.Language;
        }
        else if (TB_HT.Text.Length != 0)
        {
            if (CB_HTLanguage.SelectedIndex == 0 && Entity is IHandlerLanguage)
                CB_HTLanguage.SelectedValue = tr.Language;
        }
    }

    private void SetForms()
    {
        var species = Entity.Species;
        var pi = RequestSaveFile.Personal[species];
        bool hasForms = FormInfo.HasFormSelection(pi, species, Entity.Format);
        CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = hasForms;

        if (HaX && Entity.Format >= 4)
            Label_Form.Visible = true; // show with value entry textbox

        if (!hasForms)
        {
            if (HaX)
                return;
            Entity.Form = 0;
            if (CB_Form.Items.Count > 0)
                CB_Form.SelectedIndex = 0;
            return;
        }

        var str = GameInfo.Strings;
        var forms = FormConverter.GetFormList(species, str.types, str.forms, gendersymbols, Entity.Context);
        if (forms.Length <= 1) // no choices
            CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = false;
        else
            CB_Form.DataSource = forms;
    }

    private void SetAbilityList()
    {
        if (Entity.Format < 3) // no abilities
            return;

        if (Entity.Format > 3 && FieldsLoaded) // has forms
            Entity.Form = (byte)CB_Form.SelectedIndex; // update pk field for form specific abilities

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
        // Set the Controls
        var type = ShinyExtensions.GetType(Entity);
        BTN_Shinytize.Visible = BTN_Shinytize.Enabled = type == Shiny.Never;
        PB_ShinyStar.Visible = type == Shiny.AlwaysStar;
        PB_ShinySquare.Visible = type == Shiny.AlwaysSquare;

        // Refresh Markings (for Shiny Star if applicable)
        SetMarkings();
    }

    private void SetMarkings()
    {
        var pba = Markings;
        var count = Entity.MarkingCount;
        for (int i = 0; i < pba.Length; i++)
            pba[i].Image = GetMarkSprite(pba[i], i < count && Entity.GetMarking(i) != 0);

        PB_MarkShiny.Image = GetMarkSprite(PB_MarkShiny, !BTN_Shinytize.Enabled);
        PB_MarkCured.Image = GetMarkSprite(PB_MarkCured, CHK_Cured.Checked);

        PB_Favorite.Image = GetMarkSprite(PB_Favorite, Entity is IFavorite {Favorite: true});
        PB_Origin.Image = GetOriginSprite(Entity);

        // Colored Markings
        if (Entity.Format < 7)
            return;

        for (int i = 0; i < count; i++)
        {
            if (!Draw.GetMarkingColor(Entity.GetMarking(i), out Color c))
                continue;
            var pb = pba[i];
            pb.Image = ImageUtil.ChangeAllColorTo(pb.Image, c);
        }
    }

    private static Bitmap? GetOriginSprite(PKM pk) => OriginMarkUtil.GetOriginMark(pk) switch
    {
        OriginMark.Gen6Pentagon => Properties.Resources.gen_6,
        OriginMark.Gen7Clover   => Properties.Resources.gen_7,
        OriginMark.Gen8Galar    => Properties.Resources.gen_8,
        OriginMark.Gen8Trio     => Properties.Resources.gen_bs,
        OriginMark.Gen8Arc      => Properties.Resources.gen_la,
        OriginMark.GameBoy      => Properties.Resources.gen_vc,
        OriginMark.GO           => Properties.Resources.gen_go,
        OriginMark.LetsGo       => Properties.Resources.gen_gg,
        _ => null,
    };

    private static void SetCountrySubRegion(ComboBox cb, string type)
    {
        int oldIndex = cb.SelectedIndex;
        cb.DataSource = Util.GetCountryRegionList(type, GameInfo.CurrentLanguage);

        if (oldIndex > 0 && oldIndex < cb.Items.Count)
            cb.SelectedIndex = oldIndex;
    }

    // Prompted Updates of PKM //
    private void ClickFriendship(object sender, EventArgs e)
    {
        var pk = Entity;
        bool worst = (ModifierKeys == Keys.Control) ^ pk.IsEgg;
        var current = int.Parse(TB_Friendship.Text);
        var value = worst
            ? pk.IsEgg ? EggStateLegality.GetMinimumEggHatchCycles(pk) : 0
            : pk.IsEgg ? EggStateLegality.GetMaximumEggHatchCycles(pk) : current == 255 ? pk.PersonalInfo.BaseFriendship : 255;
        TB_Friendship.Text = value.ToString();
    }

    private void ClickLevel(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control && sender is TextBoxBase tb)
            tb.Text = "100";
    }

    private void ClickGender(object sender, EventArgs e)
    {
        if (!Entity.PersonalInfo.IsDualGender)
            return; // can't toggle

        var (canToggle, gender) = UC_Gender.ToggleGender();
        if (!canToggle)
            return;
        if (Entity.Format <= 2)
        {
            Stats.SetATKIVGender(gender);
            UpdateIsShiny();
        }
        else if (Entity.Format <= 4)
        {
            Entity.Version = WinFormsUtil.GetIndex(CB_GameOrigin);
            Entity.Nature = WinFormsUtil.GetIndex(CB_Nature);
            Entity.Form = (byte)CB_Form.SelectedIndex;

            Entity.SetPIDGender(gender);
            TB_PID.Text = Entity.PID.ToString("X8");
        }
        Entity.Gender = gender;

        if (EntityGender.GetFromString(CB_Form.Text) < 2) // Gendered Forms
            CB_Form.SelectedIndex = Math.Min(gender, CB_Form.Items.Count - 1);

        UpdatePreviewSprite?.Invoke(UC_Gender, EventArgs.Empty);
    }

    private void ClickPP(object sender, EventArgs e)
    {
        for (int i = 0; i < MovePP.Length; i++)
            RefreshMovePP(i);
    }

    private void ClickPPUps(object sender, EventArgs e)
    {
        bool min = (ModifierKeys & Keys.Control) != 0 || !Legal.IsPPUpAvailable(Entity);
        static int GetValue(ListControl cb, bool zero) => zero || WinFormsUtil.GetIndex(cb) == 0 ? 0 : 3;
        CB_PPu1.SelectedIndex = GetValue(CB_Move1, min);
        CB_PPu2.SelectedIndex = GetValue(CB_Move2, min);
        CB_PPu3.SelectedIndex = GetValue(CB_Move3, min);
        CB_PPu4.SelectedIndex = GetValue(CB_Move4, min);
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

    private void ClickOT(object sender, EventArgs e) => SetDetailsOT(SaveFileRequested.Invoke(this, e));
    private void ClickCT(object sender, EventArgs e) => SetDetailsHT(SaveFileRequested.Invoke(this, e));

    private void ClickBall(object sender, EventArgs e)
    {
        Entity.Ball = WinFormsUtil.GetIndex(CB_Ball);
        if ((ModifierKeys & Keys.Alt) != 0)
        {
            CB_Ball.SelectedValue = (int)Ball.Poke;
            return;
        }
        if ((ModifierKeys & Keys.Shift) != 0)
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

    private void ClickGT(object? sender, EventArgs e)
    {
        if (!GB_nOT.Visible)
            return;

        if (sender == GB_OT)
            Entity.CurrentHandler = 0;
        else if (TB_HT.Text.Length > 0)
            Entity.CurrentHandler = 1;
        UpadteHandlingTrainerBackground(Entity.CurrentHandler);

        ReloadToFriendshipTextBox(Entity);
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

    private void ClickMoves(object? sender, EventArgs e)
    {
        UpdateLegality(skipMoveRepop: true);
        if (sender == GB_CurrentMoves)
        {
            bool random = ModifierKeys == Keys.Control;
            if (!SetSuggestedMoves(random))
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
        var m = Entity.GetMoveSet(random);
        if (m.Length == 0 || m.All(z => z == 0))
        {
            if (!silent)
                WinFormsUtil.Alert(MsgPKMSuggestionFormat);
            return false;
        }

        Span<ushort> moves = stackalloc ushort[4];
        Entity.GetMoves(moves);
        if (moves.SequenceEqual(m))
            return false;

        if (!silent)
        {
            var mv = GameInfo.Strings.Move;
            var movestrings = m.Select(v => v >= mv.Count ? MsgProgramError : mv[v]);
            var msg = string.Join(Environment.NewLine, movestrings);
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgPKMSuggestionMoves, msg))
                return false;
        }

        Entity.SetMoves(m);
        FieldsLoaded = false;
        LoadMoves(Entity);
        ClickPP(this, EventArgs.Empty);
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
            var movestrings = m.Select(v => v >= mv.Count ? MsgProgramError : mv[v]);
            var msg = string.Join(Environment.NewLine, movestrings);
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgPKMSuggestionRelearn, msg))
                return false;
        }

        CB_RelearnMove4.SelectedValue = (int)m[3];
        CB_RelearnMove3.SelectedValue = (int)m[2];
        CB_RelearnMove2.SelectedValue = (int)m[1];
        CB_RelearnMove1.SelectedValue = (int)m[0];
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
        {
            if (!encounter.HasGroundTile(Entity.Format) || WinFormsUtil.GetIndex(CB_GroundTile) == (int)encounter.GetSuggestedGroundTile())
                return false;
        }
        if (minlvl < level)
            minlvl = level;

        if (!silent)
        {
            var suggestions = EntitySuggestionUtil.GetMetLocationSuggestionMessage(Entity, level, location, minlvl);
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

            if (encounter.HasGroundTile(Entity.Format))
                CB_GroundTile.SelectedValue = (int)encounter.GetSuggestedGroundTile();

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
        UC_Gender.Gender = Entity.Gender;
        if (Entity.Species == (int)Species.Unown && !skipForm)
            CB_Form.SelectedIndex = Entity.Form;

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
            var input = Util.ToUInt32(TB_EXP.Text);
            var exp = input;
            var gr = Entity.PersonalInfo.EXPGrowth;
            int level = Experience.GetLevel(exp, gr);
            if (level == 100)
                exp = Experience.GetEXP(100, gr);

            if (level != Util.ToInt32(TB_Level.Text))
                TB_Level.Text = level.ToString();
            if (input != exp && !HaX)
                TB_EXP.Text = exp.ToString();
        }
        else
        {
            // Change the XP
            int input = Util.ToInt32(TB_Level.Text);
            int level = Math.Max(1, Math.Min(input, 100));

            if (input != level)
                TB_Level.Text = level.ToString();
            TB_EXP.Text = Experience.GetEXP(level, Entity.PersonalInfo.EXPGrowth).ToString();
        }
        ChangingFields = false;
        if (FieldsLoaded) // store values back
            Entity.EXP = Util.ToUInt32(TB_EXP.Text);
        UpdateStats();
        UpdateLegality();
    }

    private void UpdateRandomPID(object sender, EventArgs e)
    {
        if (Entity.Format < 3)
            return;
        if (FieldsLoaded)
            Entity.PID = Util.GetHexValue(TB_PID.Text);

        if (sender == UC_Gender)
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
        if (sender is not MaskedTextBox tb)
            return;
        if (Util.ToInt32(tb.Text) > byte.MaxValue)
            tb.Text = "255";
        if (sender == TB_Friendship && int.TryParse(TB_Friendship.Text, out var value))
        {
            UpdateFromFriendshipTextBox(Entity, value);
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
        if (FieldsLoaded && sender == CB_Form)
        {
            Entity.Form = (byte)CB_Form.SelectedIndex;
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
                Entity.SetPIDUnown3((byte)CB_Form.SelectedIndex);
                TB_PID.Text = Entity.PID.ToString("X8");
            }
            else if (Entity.Format == 2)
            {
                int desiredForm = CB_Form.SelectedIndex;
                while (Entity.Form != desiredForm)
                {
                    FieldsLoaded = false;
                    Stats.UpdateRandomIVs(sender, EventArgs.Empty);
                    FieldsLoaded = true;
                }
            }
        }
        else if (CB_Form.Enabled && EntityGender.GetFromString(CB_Form.Text) < 2)
        {
            if (CB_Form.Items.Count == 2) // actually M/F; Pumpkaboo forms in German are S,M,L,XL
            {
                Entity.Gender = CB_Form.SelectedIndex;
                UC_Gender.Gender = Entity.GetSaneGender();
            }
        }
        else
        {
            UC_Gender.Gender = Entity.GetSaneGender();
        }

        RefreshFormArguments();
        if (ChangingFields)
            return;
        UpdateSprite();
    }

    private void RefreshFormArguments()
    {
        if (Entity is not IFormArgument f)
            return;

        if (FieldsLoaded)
            FA_Form.SaveArgument(f);
        FA_Form.LoadArgument(f, Entity.Species, Entity.Form, Entity.Format);
    }

    private void UpdatePP(object sender, EventArgs e)
    {
        if (sender is not ComboBox cb)
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
        var moveCB = Moves[index];
        var ppText = MovePP[index];
        var ppUps = PPUps[index];
        int move = WinFormsUtil.GetIndex(moveCB);
        if (move <= 0)
        {
            ppUps.SelectedIndex = 0;
            ppText.Text = 0.ToString();
        }
        else
        {
            int ppUpCount = ppUps.SelectedIndex;
            ppText.Text = Entity.GetMovePP((ushort)move, ppUpCount).ToString();
        }
    }

    private void UpdatePKRSstrain(object sender, EventArgs e)
    {
        // Change the PKRS Days to the legal bounds.
        int currentDuration = CB_PKRSDays.SelectedIndex;
        CB_PKRSDays.Items.Clear();

        var strain = CB_PKRSStrain.SelectedIndex;
        int max = Pokerus.GetMaxDuration(strain);
        for (int day = 0; day <= max; day++)
            CB_PKRSDays.Items.Add(day.ToString());

        // Set the days back if they're legal
        CB_PKRSDays.SelectedIndex = strain == 0 ? 0 : Math.Min(max, currentDuration);
    }

    private void UpdatePKRSdays(object sender, EventArgs e)
    {
        var days = CB_PKRSDays.SelectedIndex;
        if (days != 0)
            return;

        // If no days are selected
        var strain = CB_PKRSStrain.SelectedIndex;
        if (Pokerus.IsSusceptible(strain, days))
            CHK_Cured.Checked = CHK_Infected.Checked = false; // No Strain = Never Cured / Infected, triggers Strain update
        else if (Pokerus.IsImmune(strain, days))
            CHK_Cured.Checked = true; // Any Strain = Cured
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
            CB_PKRSDays.SelectedIndex = 1;

        SetMarkings();
    }

    private void UpdatePKRSInfected(object sender, EventArgs e)
    {
        if (CHK_Cured.Checked)
        {
            if (!CHK_Infected.Checked)
                CHK_Cured.Checked = false;
            return;
        }

        Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked;
        if (!CHK_Infected.Checked)
        {
            CB_PKRSStrain.SelectedIndex = 0;
            CB_PKRSDays.SelectedIndex = 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = false;
        }
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
            Entity.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        SpeciesIDTip.SetToolTip(CB_Species, Entity.Species.ToString("000"));
        SetAbilityList();
        SetForms();
        UpdateForm(sender, EventArgs.Empty);

        if (!FieldsLoaded)
            return;

        // Recalculate EXP for Given Level
        uint EXP = Experience.GetEXP(Entity.CurrentLevel, Entity.PersonalInfo.EXPGrowth);
        TB_EXP.Text = EXP.ToString();

        // Check for Gender Changes
        UC_Gender.Gender = Entity.GetSaneGender();

        // If species changes and no nickname, set the new name == speciesName.
        if (!CHK_Nicknamed.Checked)
            UpdateNickname(sender, e);

        UpdateLegality();
    }

    private void UpdateOriginGame(object sender, EventArgs e)
    {
        GameVersion version = (GameVersion)WinFormsUtil.GetIndex(CB_GameOrigin);
        if (version.IsValidSavedVersion())
        {
            CheckMetLocationChange(version, Entity.Context);
            if (FieldsLoaded)
                Entity.Version = (int)version;
        }

        // Visibility logic for Gen 4 ground tile; only show for Gen 4 Pokemon.
        if (Entity is IGroundTile)
        {
            bool g4 = Entity.Gen4;
            CB_GroundTile.Visible = Label_GroundTile.Visible = g4 && Entity.Format < 7;
            if (!g4)
                CB_GroundTile.SelectedValue = (int)GroundTileType.None;
        }

        if (!FieldsLoaded)
            return;

        PB_Origin.Image = GetOriginSprite(Entity);
        TID_Trainer.LoadIDValues(Entity);
        UpdateLegality();
    }

    private void CheckMetLocationChange(GameVersion version, EntityContext context)
    {
        // Does the list of locations need to be changed to another group?
        var group = GameUtil.GetMetLocationVersionGroup(version);
        if (group != origintrack || context != originFormat)
            ReloadMetLocations(version, context);
        origintrack = group;
        originFormat = context;
    }

    private void ReloadMetLocations(GameVersion version, EntityContext context)
    {
        var metList = GameInfo.GetLocationList(version, context, egg: false);
        CB_MetLocation.DataSource = new BindingSource(metList, null);
        CB_MetLocation.DropDownWidth = GetWidth(metList, CB_MetLocation.Font);

        var eggList = GameInfo.GetLocationList(version, context, egg: true);
        CB_EggLocation.DataSource = new BindingSource(eggList, null);
        CB_EggLocation.DropDownWidth = GetWidth(eggList, CB_EggLocation.Font);

        static int GetWidth(IEnumerable<ComboItem> items, Font f) =>
            items.Max(z => TextRenderer.MeasureText(z.Text, f).Width) +
            SystemInformation.VerticalScrollBarWidth;

        if (FieldsLoaded)
        {
            SetMarkings(); // Set/Remove the Nativity marking when gamegroup changes too
            int metLoc = EncounterSuggestion.GetSuggestedTransferLocation(Entity);
            int eggLoc = CHK_AsEgg.Checked
                ? EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(Entity, true)
                : LocationEdits.GetNoneLocation(Entity);

            CB_MetLocation.SelectedValue = Math.Max(0, metLoc);
            CB_EggLocation.SelectedValue = eggLoc;
        }
        else
        {
            ValidateChildren(); // hacky validation forcing
        }
    }

    private void UpdateExtraByteValue(object sender, EventArgs e)
    {
        if (!FieldsLoaded || CB_ExtraBytes.Items.Count == 0 || sender is not MaskedTextBox mtb)
            return;
        // Changed Extra Byte's Value
        var value = Util.ToInt32(mtb.Text);
        if (value > byte.MaxValue)
        {
            mtb.Text = "255";
            return; // above statement triggers the event again.
        }

        int offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
        Entity.Data[offset] = (byte)value;
    }

    private void UpdateExtraByteIndex(object sender, EventArgs e)
    {
        if (CB_ExtraBytes.Items.Count == 0)
            return;
        // Byte changed, need to refresh the Text box for the byte's value.
        var offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
        TB_ExtraByte.Text = Entity.Data[offset].ToString();
    }

    public void ChangeNature(int newNature)
    {
        if (Entity.Format < 3)
            return;

        var cb = Entity.Format >= 8 ? CB_StatNature : CB_Nature;
        cb.SelectedValue = newNature;
    }

    private void UpdateNatureModification(ComboBox cb, int nature)
    {
        string text = Stats.UpdateNatureModification(nature);
        NatureTip.SetToolTip(cb, text);
    }

    private void UpdateIsNicknamed(object sender, EventArgs e)
    {
        if (!FieldsLoaded)
            return;

        Entity.Nickname = TB_Nickname.Text;
        if (CHK_Nicknamed.Checked)
            return;

        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
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

        if (CHK_Nicknamed.Checked)
            return;

        // Fetch Current Species and set it as Nickname Text
        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        if (species is 0 || species > Entity.MaxSpeciesID)
        {
            TB_Nickname.Text = string.Empty;
            return;
        }

        string nick;
        if (CHK_IsEgg.Checked)
        {
            // Get the egg name.
            int language = WinFormsUtil.GetIndex(CB_Language);
            nick = SpeciesName.GetEggName(language, Entity.Format);
        }
        else
        {
            // If name is that of another language, don't replace the nickname
            if (sender != CB_Language && !SpeciesName.IsNicknamedAnyLanguage(species, TB_Nickname.Text, Entity.Format))
                return;
            int lang = WinFormsUtil.GetIndex(CB_Language);
            nick = SpeciesName.GetSpeciesNameGeneration(species, lang, Entity.Format);
        }

        TB_Nickname.Text = nick;
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

        if (tb == TB_Nickname)
        {
            Entity.Nickname = tb.Text;
            var span = Entity.Nickname_Trash;
            var d = new TrashEditor(tb, span, sav);
            d.ShowDialog();
            tb.Text = d.FinalString;
            d.FinalBytes.CopyTo(span);
        }
        else if (tb == TB_OT)
        {
            Entity.OT_Name = tb.Text;
            var span = Entity.OT_Trash;
            var d = new TrashEditor(tb, span, sav);
            d.ShowDialog();
            tb.Text = d.FinalString;
            d.FinalBytes.CopyTo(span);
        }
        else if (tb == TB_HT)
        {
            Entity.HT_Name = tb.Text;
            var span = Entity.HT_Trash;
            var d = new TrashEditor(tb, span, sav);
            d.ShowDialog();
            tb.Text = d.FinalString;
            d.FinalBytes.CopyTo(span);
        }
    }

    private void UpdateNotOT(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TB_HT.Text))
        {
            ClickGT(GB_OT, EventArgs.Empty); // Switch CT over to OT.
            UC_HTGender.Visible = false;
            UC_HTGender.Gender = 0;
            ReloadToFriendshipTextBox(Entity);
        }
        else if (!UC_HTGender.Visible)
        {
            UC_HTGender.Visible = true;
        }
    }

    private void UpdateIsEgg(object sender, EventArgs e)
    {
        // Display hatch counter if it is an egg, Display Friendship if it is not.
        Label_HatchCounter.Visible = CHK_IsEgg.Checked && Entity.Format > 1;
        Label_Friendship.Visible = !CHK_IsEgg.Checked && Entity.Format > 1;

        if (!FieldsLoaded)
            return;

        if (Entity.Format == 3 && CHK_IsEgg.Checked)
            Entity.OT_Name = TB_OT.Text; // going to be remapped

        Entity.IsEgg = CHK_IsEgg.Checked;
        if (CHK_IsEgg.Checked)
        {
            TB_Friendship.Text = EggStateLegality.GetMinimumEggHatchCycles(Entity).ToString();

            // If we are an egg, it won't have a met location.
            CHK_AsEgg.Checked = true;
            GB_EggConditions.Enabled = true;

            CAL_MetDate.Value = new DateTime(2000, 01, 01);

            // if egg wasn't originally obtained by OT => Link Trade, else => None
            if (Entity.Format >= 4)
            {
                var sav = SaveFileRequested.Invoke(this, e);
                bool isTraded = sav.OT != TB_OT.Text || sav.TID != Entity.TID || sav.SID != Entity.SID;
                var loc = isTraded ? Locations.TradedEggLocation(sav.Generation, sav.Version) : LocationEdits.GetNoneLocation(Entity);
                CB_MetLocation.SelectedValue = loc;
            }
            else if (Entity.Format == 3)
            {
                CB_Language.SelectedValue = Entity.Language; // JPN
                TB_OT.Text = Entity.OT_Name;
            }

            CHK_Nicknamed.Checked = EggStateLegality.IsNicknameFlagSet(Entity);
            TB_Nickname.Text = SpeciesName.GetEggName(WinFormsUtil.GetIndex(CB_Language), Entity.Format);

            // Wipe egg memories
            if (Entity.Format >= 6 && ModifyPKM)
                Entity.ClearMemories();
        }
        else // Not Egg
        {
            if (!CHK_Nicknamed.Checked)
                UpdateNickname(this, EventArgs.Empty);

            TB_Friendship.Text = Entity.PersonalInfo.BaseFriendship.ToString();

            if (CB_EggLocation.SelectedIndex == 0)
            {
                CAL_MetDate.Value = DateTime.Now;
                CAL_EggDate.Value = new DateTime(2000, 01, 01);
                CHK_AsEgg.Checked = false;
                GB_EggConditions.Enabled = false;
            }
            else
            {
                CAL_MetDate.Value = CAL_EggDate.Value;
                CB_MetLocation.SelectedValue = EncounterSuggestion.GetSuggestedEggMetLocation(Entity);
            }

            var nick = SpeciesName.GetEggName(WinFormsUtil.GetIndex(CB_Language), Entity.Format);
            if (TB_Nickname.Text == nick)
                CHK_Nicknamed.Checked = false;
        }

        UpdateNickname(this, EventArgs.Empty);
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

            bool isTradedEgg = Entity.IsEgg && Entity.Version != (int)RequestSaveFile.Version;
            CB_EggLocation.SelectedValue = EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(Entity, isTradedEgg);
            return;
        }
        // Remove egg met data
        CHK_IsEgg.Checked = false;
        CAL_EggDate.Value = new DateTime(2000, 01, 01);
        CB_EggLocation.SelectedValue = LocationEdits.GetNoneLocation(Entity);

        UpdateLegality();
    }

    private void UpdateShinyPID(object sender, EventArgs e)
    {
        var changePID = Entity.Format >= 3 && (ModifierKeys & Keys.Alt) == 0;
        UpdateShiny(changePID);
    }

    private void UpdateShiny(bool changePID)
    {
        Entity.PID = Util.GetHexValue(TB_PID.Text);
        Entity.Nature = WinFormsUtil.GetIndex(CB_Nature);
        Entity.Gender = UC_Gender.Gender;
        Entity.Form = (byte)CB_Form.SelectedIndex;
        Entity.Version = WinFormsUtil.GetIndex(CB_GameOrigin);

        if (Entity.Format > 2)
        {
            var type = (ModifierKeys & ~Keys.Alt) switch
            {
                Keys.Shift => Shiny.AlwaysSquare,
                Keys.Control => Shiny.AlwaysStar,
                _ => Shiny.Random,
            };
            if (changePID)
            {
                CommonEdits.SetShiny(Entity, type);
                TB_PID.Text = Entity.PID.ToString("X8");

                int gen = Entity.Generation;
                bool pre3DS = gen is 3 or 4 or 5;
                if (pre3DS && Entity.Format >= 6)
                    TB_EC.Text = TB_PID.Text;
            }
            else
            {
                Entity.SetShinySID(type);
                TID_Trainer.UpdateSID();
            }
        }
        else
        {
            Entity.SetShiny();
            LoadIVs(Entity);
            Stats.UpdateIVs(this, EventArgs.Empty);
        }

        UpdateIsShiny();
        UpdatePreviewSprite?.Invoke(this, EventArgs.Empty);
        UpdateLegality();
    }

    private void UpdateTSV(object sender, EventArgs e)
    {
        if (Entity.Format <= 2)
            return;

        TID_Trainer.UpdateTSV();

        Entity.PID = Util.GetHexValue(TB_PID.Text);
        var tip = $"PSV: {Entity.PSV:d4}";
        if (Entity.IsShiny)
            tip += $" | Xor = {Entity.ShinyXor}";
        Tip3.SetToolTip(TB_PID, tip);
    }

    private void Update_ID(object? sender, EventArgs e)
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
            UC_Gender.Gender = Entity.Gender;
            UpdateNatureModification(CB_Nature, Entity.Nature);
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
        var value = NUD_Purification.Value;
        CHK_Shadow.Checked = Entity is CK3 ? value != CK3.Purified : value > 0;
        FieldsLoaded = true;
    }

    private void UpdateShadowCHK(object sender, EventArgs e)
    {
        if (!FieldsLoaded)
            return;
        FieldsLoaded = false;
        NUD_Purification.Value = CHK_Shadow.Checked ? 1 : Entity is CK3 && NUD_ShadowID.Value != 0 ? CK3.Purified : 0;
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
        if (sender is not ComboBox cb)
            return;

        ValidateComboBox(cb);
        UpdateSprite();
    }

    private void ValidateComboBox2(object sender, EventArgs e)
    {
        if (!FieldsLoaded)
            return;

        ValidateComboBox(sender, new CancelEventArgs());
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
            UpdateNatureModification(CB_Nature, Entity.Nature);
            Stats.UpdateIVs(sender, EventArgs.Empty); // updating Nature will trigger stats to update as well
            UpdateLegality();
        }
        else if (sender == CB_StatNature)
        {
            Entity.StatNature = WinFormsUtil.GetIndex(CB_StatNature);
            UpdateNatureModification(CB_StatNature, Entity.StatNature);
            Stats.UpdateIVs(sender, EventArgs.Empty); // updating Nature will trigger stats to update as well
            UpdateLegality();
        }
        else if (sender == CB_HeldItem)
        {
            UpdateLegality();
        }
    }

    private void ValidateMove(object sender, EventArgs e)
    {
        if (!FieldsLoaded || sender is not ComboBox cb)
            return;

        ValidateComboBox(cb);

        // Store value back, repopulate legality.
        var value = (ushort)WinFormsUtil.GetIndex(cb);
        int index = Array.IndexOf(Moves, cb);
        if (index != -1)
        {
            UpdatePP(sender, e);
            Entity.SetMove(index, value);
        }
        else if ((index = Array.IndexOf(Relearn, cb)) != -1)
        {
            Entity.SetRelearnMove(index, value);
        }
        else if (cb == CB_AlphaMastered && Entity is PA8 pa8)
        {
            pa8.AlphaMove = value;
        }
        else
        {
            // Shouldn't hit here.
            throw new InvalidOperationException();
        }
        UpdateLegality(skipMoveRepop: true);
    }

    private void ValidateMovePaint(object sender, DrawItemEventArgs e)
    {
        if (e.Index < 0)
            return;

        var (text, value) = (ComboItem)((ComboBox)sender).Items[e.Index];
        var valid = LegalMoveSource.Info.CanLearn((ushort)value) && !HaX;

        var current = (e.State & DrawItemState.Selected) != 0;
        var brush = Draw.Brushes.GetBackground(valid, current);
        var textColor = Draw.GetText(current);

        DrawMoveRectangle(e, brush, text, textColor);
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

        // Populating the combobox drop-down list is deferred until the dropdown is entered into at least once.
        // Saves some lag delays when viewing a pk.
        if (LegalMoveSource.Display.GetIsMoveBoxOrdered(index))
            return;
        SetMoveDataSource(s);
        LegalMoveSource.Display.SetIsMoveBoxOrdered(index, true);
    }

    private void SetMoveDataSource(ComboBox c)
    {
        FieldsLoaded = false;
        var index = WinFormsUtil.GetIndex(c);
        c.DataSource = new BindingSource(LegalMoveSource.Display.DataSource, null);
        c.SelectedValue = index;
        FieldsLoaded = true;
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
        Entity.HT_Name = TB_HT.Text;
        Entity.OT_Name = TB_OT.Text;
        Entity.IsEgg = CHK_IsEgg.Checked;
        UpdateFromFriendshipTextBox(Entity, Util.ToInt32(TB_Friendship.Text));
        using var form = new MemoryAmie(Entity);
        form.ShowDialog();
        ReloadToFriendshipTextBox(Entity);
    }

    private void B_Records_Click(object sender, EventArgs e)
    {
        if (Entity is not ITechRecord8 t)
            return;

        if (ModifierKeys == Keys.Shift)
        {
            Span<ushort> moves = stackalloc ushort[4];
            Entity.GetMoves(moves);
            t.SetRecordFlags(moves);
            UpdateLegality();
            return;
        }

        using var form = new TechRecordEditor(t, Entity);
        form.ShowDialog();
        UpdateLegality();
    }

    private void B_MoveShop_Click(object sender, EventArgs e)
    {
        if (Entity is not IMoveShop8Mastery m)
            return;

        if (ModifierKeys == Keys.Shift)
        {
            m.ClearMoveShopFlags();
            if (Legality.EncounterMatch is IMasteryInitialMoveShop8 enc)
                enc.SetInitialMastery(Entity);
            m.SetMoveShopFlags(Entity);
            UpdateLegality();
            return;
        }

        using var form = new MoveShopEditor(m, m, Entity);
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
        PB_BattleVersion.Visible = FLP_BattleVersion.Visible = t is IBattleVersion;
        BTN_History.Visible = gen >= 6 && !pb7;
        BTN_Ribbons.Visible = gen >= 3 && !pb7;
        BTN_Medals.Visible = gen is 6 or 7 && !pb7;
        FLP_Country.Visible = FLP_SubRegion.Visible = FLP_3DSRegion.Visible = t is IRegionOrigin;
        FLP_OriginalNature.Visible = gen >= 8;
        B_Records.Visible = t is ITechRecord8;
        B_MoveShop.Visible = t is IMoveShop8Mastery;
        CB_HTLanguage.Visible = gen >= 8;
        L_AlphaMastered.Visible = CB_AlphaMastered.Visible = t is PA8;
        Contest.ToggleInterface(Entity, Entity.Context);

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

        CHK_AsEgg.Visible = GB_EggConditions.Visible = PB_Mark5.Visible = PB_Mark6.Visible = gen >= 4;
        FLP_ShinyLeaf.Visible = L_ShinyLeaf.Visible = ShinyLeaf.Visible = gen == 4;

        DEV_Ability.Enabled = DEV_Ability.Visible = gen > 3 && HaX;
        CB_Ability.Visible = !DEV_Ability.Enabled && gen >= 3;
        FLP_Nature.Visible = gen >= 3;
        FLP_Ability.Visible = gen >= 3;
        GB_ExtraBytes.Visible = GB_ExtraBytes.Enabled = gen >= 3;
        GB_Markings.Visible = gen >= 3;
        CB_Form.Enabled = gen >= 3;
        FA_Form.Visible = gen >= 6;

        FLP_Friendship.Visible = FLP_Form.Visible = gen >= 2;
        FLP_HeldItem.Visible = gen >= 2;
        CHK_IsEgg.Visible = gen >= 2;
        FLP_PKRS.Visible = FLP_EggPKRSRight.Visible = gen >= 2;
        UC_Gender.Visible = gen >= 2;
        FLP_CatchRate.Visible = gen == 1;

        // HaX override, needs to be after DEV_Ability enabled assignment.
        TB_AbilityNumber.Visible = gen >= 6 && DEV_Ability.Enabled;

        // Met Tab
        L_HomeTracker.Visible = TB_HomeTracker.Visible = gen >= 8;
        FLP_MetDate.Visible = gen >= 4;
        FLP_Fateful.Visible = FLP_Ball.Visible = FLP_OriginGame.Visible = gen >= 3;
        FLP_MetLocation.Visible = FLP_MetLevel.Visible = gen >= 2;
        FLP_GroundTile.Visible = gen is 4 or 5 or 6;
        FLP_TimeOfDay.Visible = gen == 2;

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
        TB_HT.MaxLength = Entity.OTLength;
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

        if (!HaX && sav is SAV8LA)
        {
            FLP_HeldItem.Visible = false;
        }

        // pk2 save files do not have an Origin Game stored. Prompt the met location list to update.
        if (Entity.Format == 2)
            CheckMetLocationChange(GameVersion.C, Entity.Context);
        return TranslationRequired;
    }

    private void CenterSubEditors()
    {
        // Recenter PKM SubEditors
        var firstTabArea = tabMain.TabPages[0]; // first is always initialized
        FLP_PKMEditors.HorizontallyCenter(firstTabArea);
        FLP_MoveFlags.HorizontallyCenter(firstTabArea);
    }

    public void EnableDragDrop(DragEventHandler enter, DragEventHandler drop)
    {
        AllowDrop = true;
        DragDrop += drop;
        foreach (var tab in tabMain.TabPages.OfType<TabPage>())
        {
            tab.AllowDrop = true;
            tab.DragEnter += enter;
            tab.DragDrop += drop;
        }
    }

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public Action<IBattleTemplate> LoadShowdownSet;

    private void LoadShowdownSetDefault(IBattleTemplate Set)
    {
        var pk = PreparePKM();
        pk.ApplySetDetails(Set);
        PopulateFields(pk);
    }

    private void CB_BattleVersion_SelectedValueChanged(object sender, EventArgs e)
    {
        if (Entity is not IBattleVersion b)
            return;
        var value = (byte)WinFormsUtil.GetIndex(CB_BattleVersion);
        b.BattleVersion = value;
        PB_BattleVersion.Image = GetMarkSprite(PB_BattleVersion, value != 0);
    }

    private static Image GetMarkSprite(PictureBox p, bool opaque, double trans = 0.175)
    {
        var sprite = p.InitialImage;
        return opaque ? sprite : ImageUtil.ChangeOpacity(sprite, trans);
    }

    private void ClickVersionMarking(object sender, EventArgs e)
    {
        tabMain.SelectedTab = Tab_Met;
        if (sender == PB_BattleVersion)
            CB_BattleVersion.DroppedDown = true;
        else
            CB_GameOrigin.DroppedDown = true;
    }

    public void ChangeLanguage(ITrainerInfo sav)
    {
        // Force an update to the met locations
        origintrack = GameVersion.Invalid;

        InitializeLanguage(sav);
        CenterSubEditors();
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

        CB_GroundTile.DataSource = new BindingSource(source.G4GroundTiles, null);
        CB_Nature.DataSource = new BindingSource(source.Natures, null);
        CB_StatNature.DataSource = new BindingSource(source.Natures, null);

        // Sub editors
        Stats.InitializeDataSources();

        PopulateFilteredDataSources(sav, true);
    }

    private static void SetIfDifferentCount(IReadOnlyCollection<ComboItem> update, ComboBox exist, bool force = false)
    {
        if (!force && exist.DataSource is BindingSource b && b.Count == update.Count)
            return;
        exist.DataSource = new BindingSource(update, null);
    }

    private void PopulateFilteredDataSources(ITrainerInfo sav, bool force = false)
    {
        var source = GameInfo.FilteredSources;
        SetIfDifferentCount(source.Languages, CB_Language, force);

        if (sav.Generation >= 2)
        {
            var game = (GameVersion) sav.Game;
            if (game <= 0)
                game = Entity.Context.GetSingleGameVersion();
            CheckMetLocationChange(game, sav.Context);
            SetIfDifferentCount(source.Items, CB_HeldItem, force);
        }

        if (sav.Generation >= 3)
        {
            SetIfDifferentCount(source.Balls, CB_Ball, force);
            SetIfDifferentCount(source.Games, CB_GameOrigin, force);
        }

        if (sav.Generation >= 4)
            SetIfDifferentCount(source.Abilities, DEV_Ability, force);

        if (sav.Generation >= 8)
        {
            var lang = source.Languages;
            var langWith0 = new List<ComboItem>(1 + lang.Count) {GameInfo.Sources.Empty};
            langWith0.AddRange(lang);
            SetIfDifferentCount(langWith0, CB_HTLanguage, force);

            var game = source.Games;
            var gamesWith0 = new List<ComboItem>(1 + game.Count) {GameInfo.Sources.Empty};
            gamesWith0.AddRange(game);
            SetIfDifferentCount(gamesWith0, CB_BattleVersion, force);
        }
        SetIfDifferentCount(source.Species, CB_Species, force);

        // Set the Move ComboBoxes too..
        LegalMoveSource.ChangeMoveSource(source.Moves);
        foreach (var cb in Moves.Concat(Relearn))
            SetIfDifferentCount(source.Moves, cb, force);
        if (sav is SAV8LA)
            SetIfDifferentCount(source.Moves, CB_AlphaMastered, force);
    }
}
