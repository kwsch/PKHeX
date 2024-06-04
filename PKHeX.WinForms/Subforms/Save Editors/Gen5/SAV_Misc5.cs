using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.WinForms;

public partial class SAV_Misc5 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV5 SAV;
    private readonly BattleSubway5 sw;

    private bool editing;

    private ComboBox[] cbr = null!;
    private int ofsFly;
    private int[] FlyDestC = null!;

    public SAV_Misc5(SAV5 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV5)(Origin = sav).Clone();

        sw = SAV.BattleSubway;
        ReadMain();
        LoadForest();
        ReadSubway();
        ReadEntralink();
        ReadMedals();
        ReadMusical();
        ReadRecord();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveMain();
        SaveForest();
        SaveSubway();
        SaveEntralink();
        SaveRecord();

        Forest.EnsureDecrypted(false);
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void ReadRecord()
    {
        var record = SAV.Records;
        NUD_Record16.Maximum = Record5.Record16;
        NUD_Record32.Maximum = Record5.Record32;
        NUD_Record16V.Value = record.GetRecord16(0);
        NUD_Record32V.Value = record.GetRecord32(0);
        NUD_Record16V.ValueChanged += (_, _) => record.SetRecord16((int)NUD_Record16.Value, (ushort)NUD_Record16V.Value);
        NUD_Record32V.ValueChanged += (_, _) => record.SetRecord32((int)NUD_Record32.Value, (uint)NUD_Record32V.Value);
        NUD_Record16.ValueChanged  += (_, _) => NUD_Record16V.Value = record.GetRecord16((int)NUD_Record16.Value);
        NUD_Record32.ValueChanged  += (_, _) => NUD_Record32V.Value = record.GetRecord32((int)NUD_Record32.Value);
    }

    private void SaveRecord() => SAV.Records.EndAccess();

    private void ReadMain()
    {
        string[]? FlyDestA;
        switch (SAV.Version)
        {
            case GameVersion.B or GameVersion.W or GameVersion.BW:
                ofsFly = 0x204B2;
                FlyDestA = [
                    "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
                    "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
                    "Icirrus City", "Opelucid City", "Victory Road", "Pokemon League",
                    "Lacunosa Town", "Undella Town", "Black City/White Forest", "(Unity Tower)",
                ];
                FlyDestC = [
                    0, 1, 2, 3,
                    4, 5, 6, 7,
                    8, 9, 15, 11,
                    10, 13, 12, 14,
                ];
                break;
            case GameVersion.B2 or GameVersion.W2 or GameVersion.B2W2:
                ofsFly = 0x20392;
                FlyDestA = [
                    "Aspertia City", "Floccesy Town", "Virbank City",
                    "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
                    "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
                    "Icirrus City", "Opelucid City",
                    "Lacunosa Town", "Undella Town", "Black City/White Forest",
                    "Lentimas Town", "Humilau City", "Victory Road", "Pokemon League",
                    "Pokestar Studios", "Join Avenue", "PWT", "(Unity Tower)",
                ];
                FlyDestC = [
                    24, 27, 25,
                    8, 9, 10, 11,
                    12, 13, 14, 15,
                    16, 17,
                    18, 21, 20,
                    28, 26, 66, 19,
                    5, 6, 7, 22,
                ];
                break;

            default: throw new ArgumentOutOfRangeException(nameof(SAV.Version));
        }
        uint valFly = ReadUInt32LittleEndian(SAV.Data.AsSpan(ofsFly));
        CLB_FlyDest.Items.Clear();
        CLB_FlyDest.Items.AddRange(FlyDestA);
        for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
        {
            if (FlyDestC[i] < 32)
                CLB_FlyDest.SetItemChecked(i, (valFly & (1u << FlyDestC[i])) != 0);
            else
                CLB_FlyDest.SetItemChecked(i, (SAV.Data[ofsFly + (FlyDestC[i] >> 3)] & (1 << (FlyDestC[i] & 7))) != 0);
        }

        if (SAV is SAV5BW bw)
        {
            TC_Misc.TabPages.Remove(TAB_Medals);
            GB_KeySystem.Visible = false;
            // Roamer
            cbr = [CB_Roamer642, CB_Roamer641];
            // CurrentStat:ComboboxSource
            // Not roamed: Not roamed/Defeated/Captured
            //    Roaming: Roaming/Defeated/Captured
            //   Defeated: Defeated/Captured
            //   Captured: Defeated/Captured
            // Top 2 bit acts as flags of some sorts
            for (int i = 0; i < cbr.Length; i++)
            {
                byte c = bw.Encount.GetRoamerState(i);
                var states = GetStates();
                if (states.All(z => z.Value != c))
                    states.Add(new ComboItem($"Unknown (0x{c:X2})", c));
                cbr[i].Items.Clear();
                cbr[i].InitializeBinding();
                cbr[i].DataSource = new BindingSource(states.Where(v => v.Value >= 2 || v.Value == c).ToList(), null);
                cbr[i].SelectedValue = c;
            }

            // Roamer status
            // If you wish to re-catch thundurus/tornadus,
            // set the status to "Go to route 7" and head
            // to the cabin in where old grandpa and grandma live
            // located at route 7.
            {
                var current = bw.EventWork.GetWorkRoamer();
                var states = GetRoamStatusStates();
                if (states.All(z => z.Value != current))
                    states.Add(new ComboItem($"Unknown (0x{current:X2})", current));
                CB_RoamStatus.Items.Clear();
                CB_RoamStatus.InitializeBinding();
                CB_RoamStatus.DataSource = new BindingSource(states, null);
                CB_RoamStatus.SelectedValue = (int)current;
            }

            // LibertyPass
            CHK_LibertyPass.Checked = bw.Misc.IsLibertyTicketActivated;
        }
        else if (SAV is SAV5B2W2 b2w2)
        {
            TC_Misc.TabPages.Remove(TAB_BWCityForest);
            GB_Roamer.Visible = CHK_LibertyPass.Visible = false;

            var keys = b2w2.Keys;
            // KeySystem
            string[] KeySystemA =
            [
                "Obtain EasyKey", "Obtain ChallengeKey", "Obtain CityKey", "Obtain IronKey", "Obtain IcebergKey",
                "Unlock EasyMode", "Unlock ChallengeMode", "Unlock City", "Unlock IronChamber", "Unlock IcebergChamber",
            ];
            CLB_KeySystem.Items.Clear();
            for (int i = 0; i < 5; i++)
            {
                CLB_KeySystem.Items.Add(KeySystemA[i], keys.GetIsKeyObtained((KeyType5)i));
                CLB_KeySystem.Items.Add(KeySystemA[i + 5], keys.GetIsKeyUnlocked((KeyType5)i));
            }
        }
        else
        {
            TC_Misc.TabPages.Remove(TAB_BWCityForest);
            GB_KeySystem.Visible = GB_Roamer.Visible = CHK_LibertyPass.Visible = false;
        }
    }

    private static List<ComboItem> GetStates() =>
    [
        new ComboItem("Not roamed", 0),
        new ComboItem("Roaming", 1),
        new ComboItem("Defeated", 2),
        new ComboItem("Captured", 3),
    ];

    private static List<ComboItem> GetRoamStatusStates() =>
    [
        new ComboItem("Not happened", 0),
        new ComboItem("Go to route 7", 1),
        new ComboItem("Event finished", 3),
    ];

    private void SaveMain()
    {
        uint valFly = ReadUInt32LittleEndian(SAV.Data.AsSpan(ofsFly));
        for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
        {
            if (FlyDestC[i] < 32)
            {
                if (CLB_FlyDest.GetItemChecked(i))
                    valFly |= 1u << FlyDestC[i];
                else
                    valFly &= ~(1u << FlyDestC[i]);
            }
            else
            {
                var ofs = ofsFly + (FlyDestC[i] >> 3);
                SAV.Data[ofs] = (byte)((SAV.Data[ofs] & ~(1 << (FlyDestC[i] & 7))) | ((CLB_FlyDest.GetItemChecked(i) ? 1 : 0) << (FlyDestC[i] & 7)));
            }
        }
        WriteUInt32LittleEndian(SAV.Data.AsSpan(ofsFly), valFly);

        if (SAV is SAV5BW bw)
        {
            // Roamer
            var encount = bw.Encount;
            for (int i = 0; i < cbr.Length; i++)
            {
                int c = bw.Encount.GetRoamerState(i);
                var d = (byte)WinFormsUtil.GetIndex(cbr[i]);

                if (c == d)
                    continue;
                encount.SetRoamerState(i, d);
                if (c != 1)
                    continue;
                var roamer = i == 0 ? encount.Roamer1 : encount.Roamer2;
                roamer.Clear();
                encount.SetRoamerState2C(i, 0);
            }

            // RoamStatus
            {
                var desired = (ushort)WinFormsUtil.GetIndex(CB_RoamStatus);
                bw.EventWork.SetWorkRoamer(desired);
            }

            // LibertyPass
            if (CHK_LibertyPass.Checked != bw.Misc.IsLibertyTicketActivated)
                bw.Misc.IsLibertyTicketActivated = CHK_LibertyPass.Checked;
        }
        else if (SAV is SAV5B2W2 b2w2)
        {
            // KeySystem
            var keys = b2w2.Keys;
            for (int i = 0; i < 5; i++)
            {
                var index = i * 2;
                var obtain = CLB_KeySystem.GetItemChecked(index);
                if (obtain != keys.GetIsKeyObtained((KeyType5)i))
                    keys.SetIsKeyObtained((KeyType5)i, obtain);

                var unlock = CLB_KeySystem.GetItemChecked(index + 1);
                if (unlock != keys.GetIsKeyUnlocked((KeyType5)i))
                    keys.SetIsKeyUnlocked((KeyType5)i, unlock);
            }
        }
    }

    private void B_AllFlyDest_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            CLB_FlyDest.SetItemChecked(i, true);
    }

    private void B_AllKeys_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < CLB_KeySystem.Items.Count; i++)
            CLB_KeySystem.SetItemChecked(i, true);
    }

    private void ReadEntralink()
    {
        var entree = SAV.Entralink;
        editing = true;

        NUD_EntreeWhiteLV.SetValueClamped(entree.WhiteForestLevel);
        NUD_EntreeBlackLV.SetValueClamped(entree.BlackCityLevel);

        if (SAV is SAV5B2W2 b2w2)
        {
            var pass = (Entralink5B2W2)entree;
            var ppv = Enum.GetValues<PassPower5>();
            var ppn = WinFormsTranslator.GetEnumTranslation<PassPower5>(Main.CurrentLanguage);
            var PassPowerB = new ComboItem[ppv.Length];
            for (int i = 0; i < ppv.Length; i++)
                PassPowerB[i] = new ComboItem(ppn[i], (int)ppv[i]);
            foreach (var cb in (ComboBox[])[CB_PassPower1, CB_PassPower2, CB_PassPower3])
            {
                cb.Items.Clear();
                cb.InitializeBinding();
                cb.DataSource = new BindingSource(PassPowerB, null);
            }

            CB_PassPower1.SelectedValue = (int)pass.PassPower1;
            CB_PassPower2.SelectedValue = (int)pass.PassPower2;
            CB_PassPower3.SelectedValue = (int)pass.PassPower3;

            var block = b2w2.Festa;
            NUD_FMHosted.SetValueClamped(block.Hosted);
            NUD_FMParticipated.SetValueClamped(block.Participated);
            NUD_FMCompleted.SetValueClamped(block.Completed);
            NUD_FMTopScores.SetValueClamped(block.TopScores);
            NUD_FMMostParticipants.SetValueClamped(block.Participants);
            NUD_EntreeWhiteEXP.SetValueClamped(block.WhiteEXP);
            NUD_EntreeBlackEXP.SetValueClamped(block.BlackEXP);

            string[] FMTitles = WinFormsTranslator.GetEnumTranslation<Funfest5Mission>(Main.CurrentLanguage);
            LB_FunfestMissions.Items.Clear();
            LB_FunfestMissions.Items.AddRange(FMTitles);

            CB_FMLevel.Items.Clear();
            CB_FMLevel.Items.AddRange(["Lv.1", "Lv.2 +", "Lv.3 ++", "Lv.3 +++"]);
            SetNudMax();
            SetEntreeExpTooltip();
            LB_FunfestMissions.SelectedIndex = 0;
            LoadFestaMissionRecord();
        }
        else
        {
            GB_PassPowers.Visible = false;
            PAN_MissionMeta.Visible = false;
            GB_FunfestMissions.Visible = false;
            NUD_EntreeWhiteEXP.Visible = NUD_EntreeBlackEXP.Visible = false;
        }
        editing = false;
    }

    private void SaveEntralink()
    {
        var entree = SAV.Entralink;
        entree.WhiteForestLevel = (ushort)NUD_EntreeWhiteLV.Value;
        entree.BlackCityLevel = (ushort)NUD_EntreeBlackLV.Value;

        if (SAV is SAV5B2W2 b2w2)
        {
            var pass = (Entralink5B2W2)entree;
            if (CB_PassPower1.SelectedIndex >= 0)
                pass.PassPower1 = (byte)WinFormsUtil.GetIndex(CB_PassPower1);
            if (CB_PassPower2.SelectedIndex >= 0)
                pass.PassPower2 = (byte)WinFormsUtil.GetIndex(CB_PassPower2);
            if (CB_PassPower3.SelectedIndex >= 0)
                pass.PassPower3 = (byte)WinFormsUtil.GetIndex(CB_PassPower3);

            var block = b2w2.Festa;
            block.Hosted = (ushort)NUD_FMHosted.Value;
            block.Participated = (ushort)NUD_FMParticipated.Value;
            block.Completed = (ushort)NUD_FMCompleted.Value;
            block.TopScores = (ushort)NUD_FMTopScores.Value;
            block.WhiteEXP = (byte)NUD_EntreeWhiteEXP.Value;
            block.BlackEXP = (byte)NUD_EntreeBlackEXP.Value;
            block.Participants = (byte)NUD_FMMostParticipants.Value;
        }
    }

    private void SetEntreeExpTooltip(bool? isBlack = null)
    {
        for (int i = 0; i < 2; i++)
        {
            if (isBlack == true) continue;

            var nud_lvl = i == 0 ? NUD_EntreeWhiteLV : NUD_EntreeBlackLV;
            var nud_exp = i == 0 ? NUD_EntreeWhiteEXP : NUD_EntreeBlackEXP;

            var lv = (int)nud_lvl.Value;
            var totalExp = FestaBlock5.GetTotalEntreeExp(lv);
            totalExp += (int)nud_exp.Value;

            var expToLevelUp = lv == 999 ? -1 : FestaBlock5.GetExpNeededForLevelUp(lv) - (int)nud_exp.Value;
            var tip0 = $"{(i == 0 ? "White" : "Black")} LV {lv}{Environment.NewLine}" +
                       $"Exp.Points: {totalExp}{Environment.NewLine}" +
                       $"To Next Lv: {expToLevelUp}";

            // Reset tooltip
            var tip = i == 0 ? TipExpW : TipExpB;
            tip.RemoveAll();
            tip.SetToolTip(nud_lvl, tip0);
            tip.SetToolTip(nud_exp, tip0);
        }
    }

    private void SetNudMax(bool? isBlack = null)
    {
        if (isBlack == true)
            return;

        for (int i = 0; i < 2; i++)
        {
            var nud_lvl = i == 0 ? NUD_EntreeWhiteLV : NUD_EntreeBlackLV;
            var nud_exp = i == 0 ? NUD_EntreeWhiteEXP : NUD_EntreeBlackEXP;

            var lv = (int)nud_lvl.Value;
            var expmax = FestaBlock5.GetExpNeededForLevelUp(lv) - 1;

            if (nud_exp.Value > expmax)
                nud_exp.Value = expmax;
            nud_exp.Maximum = expmax;
        }
    }

    private void LB_FunfestMissions_SelectedIndexChanged(object sender, EventArgs e)
    {
        editing = true;
        LoadFestaMissionRecord();
        editing = false;
    }

    private void LoadFestaMissionRecord()
    {
        FestaBlock5 block = ((SAV5B2W2)SAV).Festa;
        int mission = LB_FunfestMissions.SelectedIndex;
        if ((uint)mission > FestaBlock5.MaxMissionIndex)
            return;
        bool unlocked = block.IsFunfestMissionUnlocked(mission);
        L_FMUnlocked.Visible = unlocked;
        L_FMLocked.Visible = !unlocked;

        var record = block.GetMissionRecord(mission);
        CHK_FMNew.Checked = record.IsNew;
        CB_FMLevel.SelectedIndex = record.Level;
        NUD_FMBestScore.SetValueClamped(record.Score);
        NUD_FMBestTotal.SetValueClamped(record.Total);
    }

    private void ChangeFestaMissionValue(object sender, EventArgs e)
    {
        if (editing)
            return;

        FestaBlock5 block = ((SAV5B2W2)SAV).Festa;
        int mission = LB_FunfestMissions.SelectedIndex;
        if ((uint)mission > FestaBlock5.MaxMissionIndex)
            return;

        var score = new Funfest5Score((int)NUD_FMBestTotal.Value, (int)NUD_FMBestScore.Value, CB_FMLevel.SelectedIndex & 3, CHK_FMNew.Checked);
        block.SetMissionRecord(mission, score);
    }

    private void B_FunfestMissions_Click(object sender, EventArgs e)
    {
        FestaBlock5 block = ((SAV5B2W2)SAV).Festa;
        block.UnlockAllFunfestMissions();
        L_FMUnlocked.Visible = true;
        L_FMLocked.Visible = false;
    }

    private void NUD_EntreeBlackLV_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        SetNudMax(isBlack: true);
        SetEntreeExpTooltip(isBlack: true);
    }

    private void NUD_EntreeWhiteLV_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        SetNudMax(isBlack: false);
        SetEntreeExpTooltip(isBlack: false);
    }

    private void NUD_EntreeBlackEXP_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        SetEntreeExpTooltip(isBlack: true);
    }

    private void NUD_EntreeWhiteEXP_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        SetEntreeExpTooltip(isBlack: false);
    }

    private EntreeForest Forest = null!;
    private IList<EntreeSlot> AllSlots = null!;

    private void LoadForest()
    {
        Forest = SAV.EntreeForest;
        Forest.EnsureDecrypted();
        AllSlots = Forest.Slots;
        NUD_Unlocked.SetValueClamped(Forest.Unlock38Areas + 2);
        CHK_Area9.Checked = Forest.Unlock9thArea;

        var areas = AllSlots.Select(z => z.Area).Distinct()
            .Select(z => new ComboItem(z.ToString(), (int)z)).ToList();

        CB_Species.InitializeBinding();
        CB_Move.InitializeBinding();
        CB_Areas.InitializeBinding();

        var filtered = GameInfo.FilteredSources;
        CB_Species.DataSource = new BindingSource(filtered.Species, null);
        CB_Move.DataSource = new BindingSource(filtered.Moves, null);
        CB_Areas.DataSource = new BindingSource(areas, null);

        CB_Areas.SelectedIndex = 0;
    }

    private void SaveForest()
    {
        Forest.Unlock38Areas = (int)NUD_Unlocked.Value - 2;
        Forest.Unlock9thArea = CHK_Area9.Checked;
    }

    private IList<EntreeSlot> CurrentSlots = null!;
    private int currentIndex = -1;

    private void ChangeArea(object sender, EventArgs e)
    {
        var area = WinFormsUtil.GetIndex(CB_Areas);
        CurrentSlots = AllSlots.Where(z => (int)z.Area == area).ToArray();
        LB_Slots.Items.Clear();
        foreach (var z in CurrentSlots.Select(z => GetSpeciesName(z.Species)))
            LB_Slots.Items.Add(z);
        LB_Slots.SelectedIndex = currentIndex = 0;
    }

    private void ChangeSlot(object sender, EventArgs e)
    {
        CurrentSlot = null;
        if (LB_Slots.SelectedIndex >= 0)
            currentIndex = LB_Slots.SelectedIndex;
        var current = CurrentSlots[currentIndex];
        CB_Species.SelectedValue = (int)current.Species;
        SetForms(current);
        SetGenders(current);
        CB_Move.SelectedValue = (int)current.Move;
        CB_Gender.SelectedValue = current.Gender;
        CB_Form.SelectedIndex = CB_Form.Items.Count <= current.Form ? 0 : current.Form;
        NUD_Animation.SetValueClamped(current.Animation);
        CurrentSlot = current;
        SetSprite(current);
    }

    private EntreeSlot? CurrentSlot;

    public static string GetSpeciesName(ushort species)
    {
        var arr = GameInfo.Strings.Species;
        if (species >= arr.Count)
            return $"Invalid: {species}";
        return arr[species];
    }

    private void UpdateSlotValue(object sender, EventArgs e)
    {
        if (CurrentSlot == null)
            return;

        if (sender == CB_Species)
        {
            CurrentSlot.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
            LB_Slots.Items[currentIndex] = GetSpeciesName(CurrentSlot.Species);
            SetForms(CurrentSlot);
            SetGenders(CurrentSlot);
        }
        else if (sender == CB_Move)
        {
            CurrentSlot.Move = (ushort)WinFormsUtil.GetIndex(CB_Move);
        }
        else if (sender == CB_Gender)
        {
            CurrentSlot.Gender = (byte)WinFormsUtil.GetIndex(CB_Gender);
        }
        else if (sender == CB_Form)
        {
            CurrentSlot.Form = (byte)CB_Form.SelectedIndex;
        }
        else if (sender == CHK_Invisible)
        {
            CurrentSlot.Invisible = CHK_Invisible.Checked;
        }
        else if (sender == NUD_Animation)
        {
            CurrentSlot.Animation = (int)NUD_Animation.Value;
        }

        SetSprite(CurrentSlot);
    }

    private void SetSprite(EntreeSlot slot)
    {
        PB_SlotPreview.Image = SpriteUtil.GetSprite(slot.Species, slot.Form, slot.Gender, 0, 0, false, Shiny.Never, EntityContext.Gen5);
    }

    private void SetGenders(EntreeSlot slot)
    {
        CB_Gender.InitializeBinding();
        CB_Gender.DataSource = new BindingSource(GetGenderChoices(slot.Species), null);
    }

    private void B_RandForest_Click(object sender, EventArgs e)
    {
        var source = (SAV is SAV5BW ? Encounters5BW.DreamWorld_BW : Encounters5B2W2.DreamWorld_B2W2).Concat(Encounters5DR.DreamWorld_Common).ToList();
        var rnd = Util.Rand;
        Span<ushort> moves = stackalloc ushort[4];
        foreach (var s in AllSlots)
        {
            int index = rnd.Next(source.Count);
            var slot = source[index];
            source.Remove(slot);
            s.Species = slot.Species;
            s.Form = slot.Form;
            s.Gender = !((IFixedGender)slot).IsFixedGender ? PersonalTable.B2W2[slot.Species].RandomGender() : slot.Gender;

            slot.Moves.CopyTo(moves);
            var count = moves.Length - moves.Count<ushort>(0);
            s.Move = count == 0 ? (ushort)0 : moves[rnd.Next(count)];
        }
        ChangeArea(this, EventArgs.Empty); // refresh
        NUD_Unlocked.Value = 8;
        CHK_Area9.Checked = true;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private static List<ComboItem> GetGenderChoices(ushort species)
    {
        var pi = PersonalTable.B2W2[species];
        var list = new List<ComboItem>();
        if (pi.Genderless)
        {
            list.Add(new ComboItem("Genderless", 2));
            return list;
        }

        if (!pi.OnlyFemale)
            list.Add(new ComboItem("Male", 0));
        if (!pi.OnlyMale)
            list.Add(new ComboItem("Female", 1));
        return list;
    }

    private void SetForms(EntreeSlot slot)
    {
        bool hasForms = PersonalTable.B2W2[slot.Species].HasForms || slot.Species == (int)Species.Mothim;
        L_Form.Visible = CB_Form.Enabled = CB_Form.Visible = hasForms;

        CB_Form.InitializeBinding();
        var list = FormConverter.GetFormList(slot.Species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Context);
        CB_Form.DataSource = new BindingSource(list, null);
    }

    private void ReadSubway()
    {
        // Save Normal Checks
        CHK_Subway0.Checked = sw.Flag0;
        CHK_Subway1.Checked = sw.Flag1;
        CHK_Subway2.Checked = sw.Flag2;
        CHK_Subway7.Checked = sw.Flag3;

        // Save Super Checks
        CHK_SuperSingle.Checked = sw.SuperSingle;
        CHK_SuperDouble.Checked = sw.SuperDouble;
        CHK_SuperMulti.Checked = sw.SuperMulti;
        CHK_Subway7.Checked = sw.Flag7;

        // Normal
        // Single
        NUD_SinglePast.SetValueClamped(sw.SinglePast);
        NUD_SingleRecord.SetValueClamped(sw.SingleRecord);

        // Double
        NUD_DoublePast.SetValueClamped(sw.DoublePast);
        NUD_DoubleRecord.SetValueClamped(sw.DoubleRecord);

        // Multi NPC
        NUD_MultiNpcPast.SetValueClamped(sw.MultiNPCPast);
        NUD_MultiNpcRecord.SetValueClamped(sw.MultiNPCRecord);

        // Multi Friends
        NUD_MultiFriendsPast.SetValueClamped(sw.MultiFriendsPast);
        NUD_MultiFriendsRecord.SetValueClamped(sw.MultiFriendsRecord);

        // Super
        // Single
        NUD_SSinglePast.SetValueClamped(sw.SuperSinglePast);
        NUD_SSingleRecord.SetValueClamped(sw.SuperSingleRecord);

        // Double
        NUD_SDoublePast.SetValueClamped(sw.SuperDoublePast);
        NUD_SDoubleRecord.SetValueClamped(sw.SuperDoubleRecord);

        // Multi NPC
        NUD_SMultiNpcPast.SetValueClamped(sw.SuperMultiNPCPast);
        NUD_SMultiNpcRecord.SetValueClamped(sw.SuperMultiNPCRecord);

        // Multi Friends
        NUD_SMultiFriendsPast.SetValueClamped(sw.SuperMultiFriendsPast);
        NUD_SMultiFriendsRecord.SetValueClamped(sw.SuperMultiFriendsRecord);
    }

    private void SaveSubway()
    {
        // Save Normal Checks
        sw.Flag0 = CHK_Subway0.Checked;
        sw.Flag1 = CHK_Subway1.Checked;
        sw.Flag2 = CHK_Subway2.Checked;
        sw.Flag3 = CHK_Subway7.Checked;

        // Save Super Checks
        sw.SuperSingle = CHK_SuperSingle.Checked;
        sw.SuperDouble = CHK_SuperDouble.Checked;
        sw.SuperMulti = CHK_SuperMulti.Checked;
        sw.Flag7 = CHK_Subway7.Checked;

        // Normal
        // Single
        sw.SinglePast = (int)NUD_SinglePast.Value;
        sw.SingleRecord = (int)NUD_SingleRecord.Value;

        // Double
        sw.DoublePast = (int)NUD_DoublePast.Value;
        sw.DoubleRecord = (int)NUD_DoubleRecord.Value;

        // Multi NPC
        sw.MultiNPCPast = (int)NUD_MultiNpcPast.Value;
        sw.MultiNPCRecord = (int)NUD_MultiNpcRecord.Value;

        // Multi Friends
        sw.MultiFriendsPast = (int)NUD_MultiFriendsPast.Value;
        sw.MultiFriendsRecord = (int)NUD_MultiFriendsRecord.Value;

        // Super
        // Single
        sw.SuperSinglePast = (int)NUD_SSinglePast.Value;
        sw.SuperSingleRecord = (int)NUD_SSingleRecord.Value;

        // Double
        sw.SuperDoublePast = (int)NUD_SDoublePast.Value;
        sw.SuperDoubleRecord = (int)NUD_SDoubleRecord.Value;

        // Multi NPC
        sw.SuperMultiNPCPast = (int)NUD_SMultiNpcPast.Value;
        sw.SuperMultiNPCRecord = (int)NUD_SMultiNpcRecord.Value;

        // Multi Friends
        sw.SuperMultiFriendsPast = (int)NUD_SMultiFriendsPast.Value;
        sw.SuperMultiFriendsRecord = (int)NUD_SMultiFriendsRecord.Value;
    }

    private const string ForestCityBinFilter = "Forest City Bin|*.fc5";
    private const string ForestCityBinPath = "{0}.fc5";

    private void B_DumpFC_Click(object sender, EventArgs e)
    {
        if (SAV is not SAV5BW bw)
            return;
        using var sfd = new SaveFileDialog();
        sfd.Filter = ForestCityBinFilter;
        sfd.FileName = string.Format(ForestCityBinPath, SAV.Version);
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        var data = bw.Forest.ForestCity.ToArray();
        File.WriteAllBytes(sfd.FileName, data);
    }

    private void B_ImportFC_Click(object sender, EventArgs e)
    {
        if (SAV is not SAV5BW bw)
            return;

        using var ofd = new OpenFileDialog();
        ofd.Filter = ForestCityBinFilter;
        ofd.FileName = string.Format(ForestCityBinPath, SAV.Version);
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var fi = new FileInfo(ofd.FileName);
        if (fi.Length != WhiteBlack5BW.ForestCitySize)
        {
            WinFormsUtil.Alert(string.Format(MessageStrings.MsgFileSizeIncorrect, fi.Length, WhiteBlack5BW.ForestCitySize));
            return;
        }

        var data = File.ReadAllBytes(ofd.FileName);
        bw.SetData(bw.Forest.ForestCity.Span, data);
    }

    private readonly string[] MedalNames = Util.GetStringList("medals", Main.CurrentLanguage);
    private readonly string[] MedalTypeNames = Util.GetStringList("medal_types", Main.CurrentLanguage);

    private void ReadMedals()
    {
        if (SAV is SAV5B2W2)
        {
            CB_CurrentMedal.Items.AddRange(MedalNames);
            CB_MedalState.Items.AddRange(["Unobtained", "Can Obtain Hint Medal", "Hint Medal Obtained", "Can Obtain Medal", "Medal Obtained"]);
            CB_CurrentMedal.SelectedIndex = 0;
        }
    }

    private void CB_CurrentMedal_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SAV is SAV5B2W2 b2w2)
        {
            var index = CB_CurrentMedal.SelectedIndex;
            var medal = b2w2.Medals[index];
            var type = MedalList5.GetMedalType(index);
            TB_MedalType.Text = MedalTypeNames[(int)type];
            CB_MedalState.SelectedIndex = (int)medal.State;
            if (medal.CanHaveDate)
            {
                CAL_MedalDate.Value = medal.Date.ToDateTime(new TimeOnly());
                CAL_MedalDate.Enabled = true;
            }
            else
            {
                CAL_MedalDate.Enabled = false;
                CAL_MedalDate.ValueChanged -= CAL_MedalDate_ValueChanged;
                CAL_MedalDate.Value = EncounterDate.GetDateNDS().ToDateTime(new TimeOnly());
                CAL_MedalDate.ValueChanged += CAL_MedalDate_ValueChanged;
            }
            CHK_MedalUnread.Checked = medal.IsUnread;
        }
    }

    private void CB_MedalState_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SAV is SAV5B2W2 b2w2)
        {
            var medal = b2w2.Medals[CB_CurrentMedal.SelectedIndex];
            medal.State = (Medal5State)CB_MedalState.SelectedIndex;
            if (medal.CanHaveDate)
            {
                if (!medal.HasDate)
                    medal.Date = EncounterDate.GetDateNDS();
                CAL_MedalDate.Enabled = true;
            }
            else
            {
                CAL_MedalDate.Enabled = false;
            }
        }
    }

    private void CAL_MedalDate_ValueChanged(object? sender, EventArgs e)
    {
        if (SAV is SAV5B2W2 b2w2)
        {
            var medal = b2w2.Medals[CB_CurrentMedal.SelectedIndex];
            medal.Date = DateOnly.FromDateTime(CAL_MedalDate.Value);
        }
    }

    private void CHK_MedalUnread_CheckedChanged(object sender, EventArgs e)
    {
        if (SAV is SAV5B2W2 b2w2)
        {
            var medal = b2w2.Medals[CB_CurrentMedal.SelectedIndex];
            medal.IsUnread = CHK_MedalUnread.Checked;
        }
    }

    private void B_ObtainAllMedals_Click(object sender, EventArgs e)
    {
        if (SAV is SAV5B2W2 b2w2)
        {
            var now = EncounterDate.GetDateNDS();
            b2w2.Medals.ObtainAll(now, unread: true);
            System.Media.SystemSounds.Asterisk.Play();
        }
    }

    private readonly string[] PropNames = Util.GetStringList("props", Main.CurrentLanguage);

    private void ReadMusical()
    {
        CB_Prop.Items.AddRange(PropNames);
        CB_Prop.SelectedIndex = 0;
    }

    private void CB_Prop_SelectedIndexChanged(object sender, EventArgs e)
    {
        CHK_PropObtained.Checked = SAV.Musical.GetHasProp(CB_Prop.SelectedIndex);
    }

    private void CHK_PropObtained_CheckedChanged(object sender, EventArgs e)
    {
        SAV.Musical.SetHasProp(CB_Prop.SelectedIndex, CHK_PropObtained.Checked);
    }

    private void B_UnlockAllProps_Click(object sender, EventArgs e)
    {
        SAV.Musical.UnlockAllMusicalProps();
        B_UnlockAllProps.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }
}
