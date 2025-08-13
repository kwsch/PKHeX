using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_BattlePass : Form
{
    private readonly SAV4BR Origin;
    private readonly SAV4BR SAV;
    private readonly IPKMView View;
    private readonly SummaryPreviewer Preview = new();

    private BattlePass CurrentPass;
    private int CurrentPassIndex;

    private const ushort TitleOffset1 = 11057;
    private const ushort TitleOffset2 = 18015;

    private readonly NumericUpDown[] PartyBox;
    private readonly NumericUpDown[] PartySlot;
    private readonly NumericUpDown[] PartyFlags;

    private readonly string[] CharacterStyles = WinFormsTranslator.GetEnumTranslation<ModelBR>(Main.CurrentLanguage);
    private readonly string[] Gear = GameLanguage.GetStrings("gear", Main.CurrentLanguage);
    private readonly string[] Skin = WinFormsTranslator.GetEnumTranslation<SkinColorBR>(Main.CurrentLanguage);
    private readonly string[] PictureTypes = WinFormsTranslator.GetEnumTranslation<PictureTypeBR>(Main.CurrentLanguage);
    private readonly string[] PassDesigns = GameLanguage.GetStrings("pass_design", Main.CurrentLanguage);
    private readonly string[] TrainerTitles1 = GameLanguage.GetStrings($"trainer_title", Main.CurrentLanguage);
    private readonly string[] TrainerTitles2 = GameLanguage.GetStrings($"trainer_title_npc", Main.CurrentLanguage);

    private readonly IReadOnlyList<ComboItem> Languages = GameInfo.LanguageDataSource(3);
    private readonly IReadOnlyList<ComboItem> EmptyCBList = [new ComboItem(string.Empty, 0)];
    private const string NPC = "NPC";
    private string None => CharacterStyles[0];

    private bool loading = true;

    public SAV_BattlePass(SAV4BR sav, IPKMView view, int index = 0)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4BR)(Origin = sav).Clone();
        View = view;

        PartyBox = [NUD_PKM1Box, NUD_PKM2Box, NUD_PKM3Box, NUD_PKM4Box, NUD_PKM5Box, NUD_PKM6Box];
        PartySlot = [NUD_PKM1Slot, NUD_PKM2Slot, NUD_PKM3Slot, NUD_PKM4Slot, NUD_PKM5Slot, NUD_PKM6Slot];
        PartyFlags = [NUD_PKM1Flags, NUD_PKM2Flags, NUD_PKM3Flags, NUD_PKM4Flags, NUD_PKM5Flags, NUD_PKM6Flags];

        SetupComboBoxes();
        SetupPictureBoxes();
        LoadBattlePassList();

        loading = true;

        LB_Passes.SelectedIndex = CurrentPassIndex = index;
        CurrentPass = GetBattlePassReference(index);
        LoadCurrent(CurrentPass);

        loading = false;
    }

    #region Combo Boxes
    private void SetupComboBoxes()
    {
        CB_TrainerTitle.InitializeBinding();
        CB_Model.InitializeBinding();
        CB_SkinColor.InitializeBinding();
        CB_PictureType.InitializeBinding();
        CB_PassDesign.InitializeBinding();
        CB_Country.InitializeBinding();
        CB_Region.InitializeBinding();
        CB_Language.InitializeBinding();
        CB_Head.InitializeBinding();
        CB_Hair.InitializeBinding();
        CB_Face.InitializeBinding();
        CB_Glasses.InitializeBinding();
        CB_Top.InitializeBinding();
        CB_Hands.InitializeBinding();
        CB_Bottom.InitializeBinding();
        CB_Shoes.InitializeBinding();
        CB_Badge.InitializeBinding();
        CB_Bag.InitializeBinding();

        CB_TrainerTitle.DataSource = GetTrainerTitles();
        CB_Model.DataSource = Util.GetCBList(CharacterStyles);
        CB_SkinColor.DataSource = Util.GetCBList(Skin);
        CB_PictureType.DataSource = Util.GetCBList(PictureTypes);
        CB_PassDesign.DataSource = Util.GetCBList(PassDesigns);
        Main.SetCountrySubRegion(CB_Country, "gen4_countries");
        CB_Language.DataSource = Languages;
    }

    private List<ComboItem> GetTrainerTitles()
    {
        string[] titles1 = (string[])TrainerTitles1.Clone();
        string[] titles2 = (string[])TrainerTitles2.Clone();

        // Deduplicate by group
        var counts = new Dictionary<string, int>();

        foreach (var s in titles1)
        {
            counts.TryGetValue(s, out var value);
            counts[s] = value + 1;
        }
        foreach (var s in titles2)
        {
            counts.TryGetValue(s, out var value);
            counts[s] = value + 1;
        }

        for (var i = 0; i < titles1.Length; i++)
        {
            if (counts[titles1[i]] > 1)
            {
                var model = CharacterStyles[i / (titles1.Length / 6) + 1];
                titles1[i] += $" ({model})";
            }
        }
        for (var i = 0; i < titles2.Length; i++)
        {
            if (counts[titles2[i]] > 1)
                titles2[i] += $" ({NPC})";
        }

        // Deduplicate by number
        counts.Clear();

        foreach (var s in titles1)
        {
            counts.TryGetValue(s, out var value);
            counts[s] = value + 1;
        }
        foreach (var s in titles2)
        {
            counts.TryGetValue(s, out var value);
            counts[s] = value + 1;
        }

        var maxCounts = new Dictionary<string, int>(counts);
        for (var i = titles2.Length - 1; i >= 0; i--)
        {
            var s = titles2[i];
            var count = counts[s]--;
            if (maxCounts[s] == 1)
                continue;
            titles2[i] += $" ({count})";
        }
        for (var i = titles1.Length - 1; i >= 0; i--)
        {
            var s = titles1[i];
            var count = counts[s]--;
            if (maxCounts[s] == 1)
                continue;
            titles1[i] += $" ({count})";
        }

        List<ComboItem> cbList = [new ComboItem(None, 0)];
        Util.AddCBWithOffset(cbList, titles1, TitleOffset1);
        Util.AddCBWithOffset(cbList, titles2, TitleOffset2);
        return cbList;
    }

    private void SetupComboBoxesAppearance(ModelBR model)
    {
        SetupGearCategoryComboBoxes(CB_Head, model, GearCategory.Head);
        SetupGearCategoryComboBoxes(CB_Hair, model, GearCategory.Hair);
        SetupGearCategoryComboBoxes(CB_Face, model, GearCategory.Face);
        SetupGearCategoryComboBoxes(CB_Glasses, model, GearCategory.Glasses);
        SetupGearCategoryComboBoxes(CB_Top, model, GearCategory.Top);
        SetupGearCategoryComboBoxes(CB_Hands, model, GearCategory.Hands);
        SetupGearCategoryComboBoxes(CB_Bottom, model, GearCategory.Bottom);
        SetupGearCategoryComboBoxes(CB_Shoes, model, GearCategory.Shoes);
        SetupGearCategoryComboBoxes(CB_Badge, model, GearCategory.Badges);
        SetupGearCategoryComboBoxes(CB_Bag, model, GearCategory.Bags);
    }

    private void SetupGearCategoryComboBoxes(ComboBox cb, ModelBR model, GearCategory category)
    {
        if (model is >= ModelBR.YoungBoy and <= ModelBR.LittleGirl)
        {
            if (category == GearCategory.Badges)
                model = ModelBR.YoungBoy;
            var (offset, count) = GearUnlock.GetOffsetCount(model, category);
            cb.DataSource = Util.GetCBList(Gear.AsSpan(offset, count));
            cb.SelectedValue = GearUnlock.GetDefault(model, category);
            cb.Enabled = true;
        }
        else
        {
            cb.DataSource = EmptyCBList;
            cb.SelectedValue = 0;
            cb.Enabled = false;
        }
    }
    #endregion

    #region Picture Boxes
    private void SetupPictureBoxes()
    {
        Box.InitializeGrid(3, 2, SpriteUtil.Spriter);
        Box.HorizontallyCenter(f_PKM);
        foreach (PictureBox pb in Box.Entries)
        {
            pb.Click += (_, args) => OmniClick(pb, args);
            pb.ContextMenuStrip = mnu;
            pb.MouseMove += (_, args) => Preview.UpdatePreviewPosition(args.Location);
            pb.MouseEnter += (_, _) => HoverSlot(pb);
            pb.MouseLeave += (_, _) => Preview.Clear();
        }
        Closing += (_, _) => Preview.Clear();
    }

    private void HoverSlot(PictureBox pb)
    {
        var index = Box.Entries.IndexOf(pb);
        var slot = CurrentPass.GetPartySlotAtIndex(index);
        Preview.Show(pb, slot);
    }

    private void OmniClick(object sender, EventArgs e)
    {
        switch (ModifierKeys)
        {
            case Keys.Control: ClickView(sender, e); break;
            case Keys.Shift: ClickSet(sender, e); break;
            case Keys.Alt: ClickDelete(sender, e); break;
            default:
                return;
        }
    }

    private int groupSelected = -1;
    private int slotSelected = -1;

    private void ClickView(object sender, EventArgs e)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        if (pb is null)
            return;
        int index = Box.Entries.IndexOf(pb);

        View.PopulateFields(CurrentPass.GetPartySlotAtIndex(index), false);

        if (slotSelected != index && (uint)slotSelected < Box.Entries.Count)
            Box.Entries[slotSelected].BackgroundImage = null;

        groupSelected = CurrentPassIndex;
        slotSelected = index;
        Box.Entries[index].BackgroundImage = SpriteUtil.Spriter.View;
    }

    private void ClickSet(object sender, EventArgs e)
    {
        if (!View.EditsComplete)
            return;
        PKM pk = View.PreparePKM();
        if (pk.Species == 0)
        { System.Media.SystemSounds.Asterisk.Play(); return; }

        var index = GetSenderIndex(sender);
        var sav = SAV;

        var errata = sav.EvaluateCompatibility(pk);
        if (errata.Count != 0)
        {
            var msg = string.Join(Environment.NewLine, errata);
            var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, msg, MsgContinue);
            if (prompt != DialogResult.Yes)
                return;
        }

        SaveCurrent(CurrentPass);
        CurrentPass.SetPartySlotAtIndex(pk, index);
        switch (sav.BattlePasses.GetPassType(CurrentPassIndex))
        {
            case BattlePassType.Custom:
                var (box, slot) = Origin.FindSlot(pk);
                CurrentPass.SetPartySlotBoxSlot(index, box, slot);
                break;
            case BattlePassType.Rental:
                CurrentPass.SetPartySlotBoxSlot(index, 255, 0);
                break;
        }
        LoadCurrent(CurrentPass);
    }

    private void ClickDelete(object sender, EventArgs e)
    {
        var index = GetSenderIndex(sender);
        var pk = CurrentPass.GetPartySlotAtIndex(index);
        if (pk.Species == 0)
        { System.Media.SystemSounds.Asterisk.Play(); return; }

        SaveCurrent(CurrentPass);
        CurrentPass.DeletePartySlot(index);
        LoadCurrent(CurrentPass);
    }

    private int GetSenderIndex(object sender)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        ArgumentNullException.ThrowIfNull(pb);
        var index = Box.Entries.IndexOf(pb);
        return index;
    }
    #endregion

    private void LoadBattlePassList()
    {
        loading = true;
        int selected = LB_Passes.SelectedIndex;
        LB_Passes.Items.Clear();

        for (int i = 0; i < BattlePassAccessor.PASS_COUNT; i++)
        {
            string type = WinFormsTranslator.TranslateEnum(SAV.BattlePasses.GetPassType(i), Main.CurrentLanguage);
            BattlePass pass = GetBattlePassReference(i);
            string name = pass.Name;
            if ((!pass.Rental && !pass.Issued) || string.IsNullOrWhiteSpace(name))
                name = None;
            LB_Passes.Items.Add($"{i + 1:00} {type}/{name}");
        }

        if (selected >= 0)
            LB_Passes.SelectedIndex = selected;

        loading = false;
    }

    private void ReloadBattlePassList()
    {
        loading = true;
        int selected = LB_Passes.SelectedIndex;

        for (int i = 0; i < BattlePassAccessor.PASS_COUNT; i++)
        {
            string type = WinFormsTranslator.TranslateEnum(SAV.BattlePasses.GetPassType(i), Main.CurrentLanguage);
            BattlePass pass = GetBattlePassReference(i);
            string name = pass.Name;
            if ((!pass.Rental && !pass.Issued) || string.IsNullOrWhiteSpace(name))
                name = None;
            LB_Passes.Items[i] = $"{i + 1:00} {type}/{name}";
        }

        if (selected >= 0)
            LB_Passes.SelectedIndex = selected;

        loading = false;
    }

    private BattlePass GetBattlePassReference(int index) => SAV.BattlePasses[index];

    private void LoadCurrent(BattlePass pdata)
    {
        loading = true;

        TB_Name.Text = pdata.Name;
        CB_TrainerTitle.SelectedValue = (int)pdata.TrainerTitle;
        MT_TID.Text = pdata.TID.ToString("00000");
        MT_SID.Text = pdata.SID.ToString("00000");

        CB_Model.SelectedValue = pdata.Model;
        CB_SkinColor.SelectedValue = pdata.Skin;
        CB_Head.SelectedValue = pdata.Head;
        CB_Hair.SelectedValue = pdata.Hair;
        CB_Face.SelectedValue = pdata.Face;
        CB_Glasses.SelectedValue = pdata.Glasses;
        CB_Top.SelectedValue = pdata.Top;
        CB_Hands.SelectedValue = pdata.Hands;
        CB_Bottom.SelectedValue = pdata.Bottom;
        CB_Shoes.SelectedValue = pdata.Shoes;
        CB_Badge.SelectedValue = pdata.Badge;
        CB_Bag.SelectedValue = pdata.Bag;

        CB_PictureType.SelectedValue = pdata.PictureType;
        CB_PassDesign.SelectedValue = pdata.PassDesign;

        CHK_Available.Checked = pdata.Available;
        CHK_Issued.Checked = pdata.Issued;
        CHK_Rental.Checked = pdata.Rental;
        CHK_Friend.Checked = pdata.Friend;

        for (int i = 0; i < BattlePass.Count; i++)
        {
            (PartyBox[i].Value, PartySlot[i].Value) = pdata.GetPartySlotBoxSlot(i);
            PartyFlags[i].Value = pdata.GetPartySlotFlags(i);
        }

        CHK_PresetGreeting.Checked = pdata.PresetGreeting;
        CHK_PresetSentOut.Checked = pdata.PresetSentOut;
        CHK_PresetShift1.Checked = pdata.PresetShift1;
        CHK_PresetShift2.Checked = pdata.PresetShift2;
        CHK_PresetWin.Checked = pdata.PresetWin;
        CHK_PresetLose.Checked = pdata.PresetLose;
        TB_Greeting.Text = pdata.Greeting;
        TB_SentOut.Lines = pdata.SentOut.Split(StringConverter4GC.LineBreak);
        TB_Shift1.Text = pdata.Shift1;
        TB_Shift2.Text = pdata.Shift2;
        TB_Win.Lines = pdata.Win.Split(StringConverter4GC.LineBreak);
        TB_Lose.Lines = pdata.Lose.Split(StringConverter4GC.LineBreak);
        NUD_PresetGreetingIndex.Value = pdata.PresetGreetingIndex;
        NUD_PresetSentOutIndex.Value = pdata.PresetSentOutIndex;
        NUD_PresetShift1Index.Value = pdata.PresetShift1Index;
        NUD_PresetShift2Index.Value = pdata.PresetShift2Index;
        NUD_PresetWinIndex.Value = pdata.PresetWinIndex;
        NUD_PresetLoseIndex.Value = pdata.PresetLoseIndex;

        TB_CreatorName.Text = pdata.CreatorName;
        TB_BirthMonth.Text = pdata.BirthMonth;
        TB_BirthDay.Text = pdata.BirthDay;
        CB_Country.SelectedValue = pdata.Country;
        CB_Region.SelectedValue = pdata.Region;
        CB_Language.SelectedValue = (int)pdata.Language.ToLanguageID();
        TB_SelfIntroduction.Lines = pdata.SelfIntroduction.TrimStart(StringConverter4GC.Proportional).Split(StringConverter4GC.LineBreak);
        TB_RegionCode.Text = pdata.RegionCode;
        MT_PlayerID.Text = pdata.PlayerID.ToString("X16");

        NUD_Battles.Value = pdata.Battles;
        NUD_RecordColosseumBattles.Value = pdata.RecordColosseumBattles;
        NUD_RecordFreeBattles.Value = pdata.RecordFreeBattles;
        NUD_RecordWiFiBattles.Value = pdata.RecordWiFiBattles;
        NUD_RecordGatewayColosseumClears.Value = pdata.RecordGatewayColosseumClears;
        NUD_RecordMainStreetColosseumClears.Value = pdata.RecordMainStreetColosseumClears;
        NUD_RecordWaterfallColosseumClears.Value = pdata.RecordWaterfallColosseumClears;
        NUD_RecordNeonColosseumClears.Value = pdata.RecordNeonColosseumClears;
        NUD_RecordCrystalColosseumClears.Value = pdata.RecordCrystalColosseumClears;
        NUD_RecordSunnyParkColosseumClears.Value = pdata.RecordSunnyParkColosseumClears;
        NUD_RecordMagmaColosseumClears.Value = pdata.RecordMagmaColosseumClears;
        NUD_RecordCourtyardColosseumClears.Value = pdata.RecordCourtyardColosseumClears;
        NUD_RecordSunsetColosseumClears.Value = pdata.RecordSunsetColosseumClears;
        NUD_RecordStargazerColosseumClears.Value = pdata.RecordStargazerColosseumClears;

        for (int i = 0; i < BattlePass.Count; i++)
            Box.Entries[i].Image = pdata.GetPartySlotPresent(i) ? pdata.GetPartySlotAtIndex(i).Sprite(SAV, flagIllegal: true) : SpriteUtil.Spriter.None;

        if (slotSelected != -1 && (uint)slotSelected < Box.Entries.Count)
            Box.Entries[slotSelected].BackgroundImage = groupSelected != CurrentPassIndex ? null : SpriteUtil.Spriter.View;

        loading = false;
    }

    private void SaveCurrent(BattlePass pdata)
    {
        pdata.Name = TB_Name.Text;
        pdata.TrainerTitle = (short)WinFormsUtil.GetIndex(CB_TrainerTitle);
        pdata.TID = (ushort)Util.ToUInt32(MT_TID.Text);
        pdata.SID = (ushort)Util.ToUInt32(MT_SID.Text);

        pdata.Model = WinFormsUtil.GetIndex(CB_Model);
        pdata.Skin = WinFormsUtil.GetIndex(CB_SkinColor);
        pdata.Head = WinFormsUtil.GetIndex(CB_Head);
        pdata.Hair = WinFormsUtil.GetIndex(CB_Hair);
        pdata.Face = WinFormsUtil.GetIndex(CB_Face);
        pdata.Glasses = WinFormsUtil.GetIndex(CB_Glasses);
        pdata.Top = WinFormsUtil.GetIndex(CB_Top);
        pdata.Hands = WinFormsUtil.GetIndex(CB_Hands);
        pdata.Bottom = WinFormsUtil.GetIndex(CB_Bottom);
        pdata.Shoes = WinFormsUtil.GetIndex(CB_Shoes);
        pdata.Badge = WinFormsUtil.GetIndex(CB_Badge);
        pdata.Bag = WinFormsUtil.GetIndex(CB_Bag);

        pdata.PictureType = WinFormsUtil.GetIndex(CB_PictureType);
        pdata.PassDesign = WinFormsUtil.GetIndex(CB_PassDesign);

        pdata.Available = CHK_Available.Checked;
        pdata.Issued = CHK_Issued.Checked;
        pdata.Rental = CHK_Rental.Checked;
        pdata.Friend = CHK_Friend.Checked;

        for (int i = 0; i < BattlePass.Count; i++)
        {
            pdata.SetPartySlotBoxSlot(i, (byte)PartyBox[i].Value, (byte)PartySlot[i].Value);
            pdata.SetPartySlotFlags(i, (ushort)PartyFlags[i].Value);
        }

        pdata.PresetGreeting = CHK_PresetGreeting.Checked;
        pdata.PresetSentOut = CHK_PresetSentOut.Checked;
        pdata.PresetShift1 = CHK_PresetShift1.Checked;
        pdata.PresetShift2 = CHK_PresetShift2.Checked;
        pdata.PresetWin = CHK_PresetWin.Checked;
        pdata.PresetLose = CHK_PresetLose.Checked;
        pdata.Greeting = TB_Greeting.Text;
        pdata.SentOut = string.Join(StringConverter4GC.LineBreak, TB_SentOut.Lines);
        pdata.Shift1 = TB_Shift1.Text;
        pdata.Shift2 = TB_Shift2.Text;
        pdata.Win = string.Join(StringConverter4GC.LineBreak, TB_Win.Lines);
        pdata.Lose = string.Join(StringConverter4GC.LineBreak, TB_Lose.Lines);
        pdata.PresetGreetingIndex = (ushort)NUD_PresetGreetingIndex.Value;
        pdata.PresetSentOutIndex = (ushort)NUD_PresetSentOutIndex.Value;
        pdata.PresetShift1Index = (ushort)NUD_PresetShift1Index.Value;
        pdata.PresetShift2Index = (ushort)NUD_PresetShift2Index.Value;
        pdata.PresetWinIndex = (ushort)NUD_PresetWinIndex.Value;
        pdata.PresetLoseIndex = (ushort)NUD_PresetLoseIndex.Value;

        pdata.CreatorName = TB_CreatorName.Text;
        pdata.BirthMonth = TB_BirthMonth.Text;
        pdata.BirthDay = TB_BirthDay.Text;
        pdata.Country = WinFormsUtil.GetIndex(CB_Country);
        pdata.Region = WinFormsUtil.GetIndex(CB_Region);
        pdata.Language = ((LanguageID)WinFormsUtil.GetIndex(CB_Language)).ToBattlePassLanguage();
        pdata.SelfIntroduction = (pdata.Language == BattlePassLanguage.Japanese ? string.Empty : StringConverter4GC.Proportional.ToString()) + string.Join(StringConverter4GC.LineBreak, TB_SelfIntroduction.Lines);
        pdata.RegionCode = TB_RegionCode.Text;
        pdata.PlayerID = Util.GetHexValue64(MT_PlayerID.Text);

        pdata.Battles = (int)NUD_Battles.Value;
        pdata.RecordColosseumBattles = (int)NUD_RecordColosseumBattles.Value;
        pdata.RecordFreeBattles = (int)NUD_RecordFreeBattles.Value;
        pdata.RecordWiFiBattles = (int)NUD_RecordWiFiBattles.Value;
        pdata.RecordGatewayColosseumClears = (int)NUD_RecordGatewayColosseumClears.Value;
        pdata.RecordMainStreetColosseumClears = (int)NUD_RecordMainStreetColosseumClears.Value;
        pdata.RecordWaterfallColosseumClears = (int)NUD_RecordWaterfallColosseumClears.Value;
        pdata.RecordNeonColosseumClears = (int)NUD_RecordNeonColosseumClears.Value;
        pdata.RecordCrystalColosseumClears = (int)NUD_RecordCrystalColosseumClears.Value;
        pdata.RecordSunnyParkColosseumClears = (int)NUD_RecordSunnyParkColosseumClears.Value;
        pdata.RecordMagmaColosseumClears = (int)NUD_RecordMagmaColosseumClears.Value;
        pdata.RecordCourtyardColosseumClears = (int)NUD_RecordCourtyardColosseumClears.Value;
        pdata.RecordSunsetColosseumClears = (int)NUD_RecordSunsetColosseumClears.Value;
        pdata.RecordStargazerColosseumClears = (int)NUD_RecordStargazerColosseumClears.Value;

        ReloadBattlePassList();
    }

    #region Editing Index Changing
    private void ChangeIndexPass(object sender, EventArgs e)
    {
        int index = LB_Passes.SelectedIndex;
        if (index < 0 || loading)
            return;

        SaveCurrent(CurrentPass);
        B_FDelete.Enabled = SAV.BattlePasses.GetPassType(index) != BattlePassType.Rental;
        LoadCurrent(CurrentPass = GetBattlePassReference(CurrentPassIndex = index));
    }
    #endregion

    #region I/O
    private void B_Import_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var path = ofd.FileName;
        if (new FileInfo(path).Length is not BattlePass.Size)
            return;

        var data = File.ReadAllBytes(path);
        data.CopyTo(CurrentPass.Data);

        LoadCurrent(CurrentPass);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Export_Click(object sender, EventArgs e)
    {
        var bp = CurrentPass;
        ArgumentNullException.ThrowIfNull(bp);

        SaveCurrent(bp);
        var tr = bp.Name;
        if (string.IsNullOrWhiteSpace(tr))
            tr = "Trainer";
        using var sfd = new SaveFileDialog();
        sfd.Filter = "Battle Pass Data|*.bin";
        sfd.FileName = $"{PathUtil.CleanFileName(tr)}.bin";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        var path = sfd.FileName;
        File.WriteAllBytes(path, bp.Data);
    }
    #endregion

    #region Meta Buttons
    private void B_Up_Click(object sender, EventArgs e) => SwapSlots(false);
    private void B_Down_Click(object sender, EventArgs e) => SwapSlots(true);

    private void SwapSlots(bool down)
    {
        if (LB_Passes.SelectedIndex == -1)
            return;
        int index = LB_Passes.SelectedIndex;
        var other = index + (down ? 1 : -1);
        if ((uint)other >= LB_Passes.Items.Count)
        {
            System.Media.SystemSounds.Asterisk.Play();
            return;
        }
        SaveCurrent(CurrentPass);

        SAV.BattlePasses.Swap(index, other);
        ReloadBattlePassList();

        loading = true;

        LB_Passes.SelectedIndex = CurrentPassIndex = other;
        CurrentPass = GetBattlePassReference(other);
        LoadCurrent(CurrentPass);
        B_FDelete.Enabled = SAV.BattlePasses.GetPassType(other) != BattlePassType.Rental;

        loading = false;
    }

    private void B_Delete_Click(object sender, EventArgs e)
    {
        var index = CurrentPassIndex;
        SaveCurrent(CurrentPass);

        SAV.BattlePasses.Delete(index);
        ReloadBattlePassList();

        loading = true;

        CurrentPassIndex = index;
        CurrentPass = GetBattlePassReference(index);
        LoadCurrent(CurrentPass);

        loading = false;
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveCurrent(CurrentPass);

        // Since there may have been other changes in the main window, only overwrite the Battle Passes
        Origin.BattlePasses.CopyChangesFrom(SAV.BattlePasses);
        Close();
    }
    #endregion

    private void B_UnlockCustom_Click(object sender, EventArgs e)
    {
        SaveCurrent(CurrentPass);
        SAV.BattlePasses.UnlockAllCustomPasses();
        LoadCurrent(CurrentPass);
    }

    private void B_UnlockRental_Click(object sender, EventArgs e)
    {
        SaveCurrent(CurrentPass);
        SAV.BattlePasses.UnlockAllRentalPasses();
        LoadCurrent(CurrentPass);
    }

    private void ValidateCatchphrase(object sender, EventArgs e)
    {
        if (sender is not TextBox tb)
            return;

        int i = 0;
        int length = 0;
        foreach (string line in tb.Lines)
        {
            foreach (char c in line)
            {
                length += c switch
                {
                    StringConverter4GC.LineBreak or StringConverter4GC.Proportional or StringConverter4GC.PokemonName => 2,
                    _ => 1,
                };
                if (length > tb.MaxLength)
                {
                    tb.Text = tb.Text[..i];
                    return;
                }
                i++;
            }
            length += 2;
            i += Environment.NewLine.Length;
        }
    }

    private void ValidatePlayerID(object sender, EventArgs e)
    {
        if (sender is MaskedTextBox mt)
            mt.Text = Util.GetHexValue64(mt.Text).ToString("X16");
    }

    private void CB_Model_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is ComboBox c)
        {
            int index = WinFormsUtil.GetIndex(c);
            SetupComboBoxesAppearance((ModelBR)index);
            if (!loading)
                UpdatePresetIndexes(sender, e);
        }
    }

    private void UpdatePresetIndexes(object sender, EventArgs e)
    {
        SaveCurrent(CurrentPass);
        CurrentPass.ResetPresetIndexes();
        LoadCurrent(CurrentPass);
    }

    private void UpdateCountry(object sender, EventArgs e)
    {
        if (sender is ComboBox c)
        {
            int index = WinFormsUtil.GetIndex(c);
            Main.SetCountrySubRegion(CB_Region, $"gen4_sr_{index:000}");
            if (CB_Region.Items.Count == 0)
                Main.SetCountrySubRegion(CB_Region, "gen4_sr_default");
        }
    }

    private void CB_Language_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is ComboBox c)
        {
            int index = WinFormsUtil.GetIndex(c);
            if (index != (int)LanguageID.Japanese)
                TB_SelfIntroduction.MaxLength = 51;
            else
                TB_SelfIntroduction.MaxLength = 53;
            ValidateCatchphrase(TB_SelfIntroduction, e);
        }
    }
}
