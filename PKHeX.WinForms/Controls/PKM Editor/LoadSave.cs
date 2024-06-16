using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void LoadNickname(PKM pk)
    {
        CHK_NicknamedFlag.Checked = pk.IsNicknamed;
        TB_Nickname.Text = pk.Nickname;
    }

    private void SaveNickname(PKM pk)
    {
        pk.IsNicknamed = CHK_NicknamedFlag.Checked;
        pk.Nickname = TB_Nickname.Text;
    }

    private void LoadSpeciesLevelEXP(PKM pk)
    {
        if (!HaX)
        {
            // Sanity check level and EXP
            var current = pk.CurrentLevel;
            if (current == 100) // clamp back to max EXP
                pk.CurrentLevel = 100;
        }

        CB_Species.SelectedValue = (int)pk.Species;
        TB_Level.Text = pk.Stat_Level.ToString();
        TB_EXP.Text = pk.EXP.ToString();
    }

    private void SaveSpeciesLevelEXP(PKM pk)
    {
        pk.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        pk.EXP = Util.ToUInt32(TB_EXP.Text);
        pk.Stat_Level = (byte)Math.Max(1, Util.ToInt32(TB_Level.Text));
    }

    private void LoadOT(PKM pk)
    {
        GB_OT.BackgroundImage = null; // clear the Current Handler indicator just in case we switched formats.
        TB_OT.Text = pk.OriginalTrainerName;
        UC_OTGender.Gender = (byte)(pk.OriginalTrainerGender & 1);
    }

    private void SaveOT(PKM pk)
    {
        pk.OriginalTrainerName = TB_OT.Text;
        pk.OriginalTrainerGender = UC_OTGender.Gender;
    }

    private void LoadPokerus(PKM pk)
    {
        var infected = pk.IsPokerusInfected;
        var cured = pk.IsPokerusCured;
        CHK_Infected.Checked = Label_PKRS.Visible = CB_PKRSStrain.Visible = infected;
        Label_PKRSdays.Visible = CB_PKRSDays.Visible = !cured && infected;
        CHK_Cured.Checked = cured;
        ChangePKRSstrainDropDownLists(CB_PKRSStrain.SelectedIndex, pk.PokerusStrain, 0);
        LoadClamp(CB_PKRSStrain, pk.PokerusStrain);
        LoadClamp(CB_PKRSDays, pk.PokerusDays); // clamp to valid day values for the current strain
    }

    private void SavePokerus(PKM pk)
    {
        pk.PokerusDays = CB_PKRSDays.SelectedIndex;
        pk.PokerusStrain = CB_PKRSStrain.SelectedIndex;
    }

    private void LoadIVs(PKM pk)
    {
        Span<int> span = stackalloc int[6];
        pk.GetIVs(span);
        Stats.LoadIVs(span);
    }

    private void LoadEVs(PKM pk)
    {
        Span<int> span = stackalloc int[6];
        pk.GetEVs(span);
        Stats.LoadEVs(span);
    }

    private void LoadAVs(IAwakened pk) => Stats.LoadAVs(pk);
    private void LoadGVs(IGanbaru pk) => Stats.LoadGVs(pk);

    private static void LoadClamp(ComboBox cb, int value)
    {
        var max = cb.Items.Count - 1;
        if (value > max)
            value = max;
        else if (value < -1)
            value = 0;
        cb.SelectedIndex = value;
    }

    private void LoadMoves(PKM pk)
    {
        MC_Move1.SelectedMove = pk.Move1;
        MC_Move2.SelectedMove = pk.Move2;
        MC_Move3.SelectedMove = pk.Move3;
        MC_Move4.SelectedMove = pk.Move4;
        MC_Move1.PPUps = pk.Move1_PPUps;
        MC_Move2.PPUps = pk.Move2_PPUps;
        MC_Move3.PPUps = pk.Move3_PPUps;
        MC_Move4.PPUps = pk.Move4_PPUps;
        MC_Move1.PP = pk.Move1_PP;
        MC_Move2.PP = pk.Move2_PP;
        MC_Move3.PP = pk.Move3_PP;
        MC_Move4.PP = pk.Move4_PP;
    }

    private void SaveMoves(PKM pk)
    {
        pk.Move1 = MC_Move1.SelectedMove;
        pk.Move2 = MC_Move2.SelectedMove;
        pk.Move3 = MC_Move3.SelectedMove;
        pk.Move4 = MC_Move4.SelectedMove;
        pk.Move1_PP = MC_Move1.PP;
        pk.Move2_PP = MC_Move2.PP;
        pk.Move3_PP = MC_Move3.PP;
        pk.Move4_PP = MC_Move4.PP;
        pk.Move1_PPUps = MC_Move1.PPUps;
        pk.Move2_PPUps = MC_Move2.PPUps;
        pk.Move3_PPUps = MC_Move3.PPUps;
        pk.Move4_PPUps = MC_Move4.PPUps;
    }

    private void LoadShadow3(IShadowCapture pk)
    {
        NUD_ShadowID.Value = pk.ShadowID;
        FLP_Purification.Visible = pk.ShadowID > 0;
        if (pk.ShadowID > 0)
        {
            int value = pk.Purification;
            if (value < NUD_Purification.Minimum)
                value = (int)NUD_Purification.Minimum;

            NUD_Purification.Value = value;
            CHK_Shadow.Checked = pk.IsShadow;

            NUD_ShadowID.Value = Math.Max(pk.ShadowID, (ushort)0);
        }
        else
        {
            NUD_Purification.Value = 0;
            CHK_Shadow.Checked = false;
            NUD_ShadowID.Value = 0;
        }
    }

    private void SaveShadow3(IShadowCapture pk)
    {
        pk.ShadowID = (ushort)NUD_ShadowID.Value;
        if (pk.ShadowID > 0)
            pk.Purification = (int)NUD_Purification.Value;
    }

    private void LoadRelearnMoves(PKM pk)
    {
        CB_RelearnMove1.SelectedValue = (int)pk.RelearnMove1;
        CB_RelearnMove2.SelectedValue = (int)pk.RelearnMove2;
        CB_RelearnMove3.SelectedValue = (int)pk.RelearnMove3;
        CB_RelearnMove4.SelectedValue = (int)pk.RelearnMove4;
    }

    private void SaveRelearnMoves(PKM pk)
    {
        pk.RelearnMove1 = (ushort)WinFormsUtil.GetIndex(CB_RelearnMove1);
        pk.RelearnMove2 = (ushort)WinFormsUtil.GetIndex(CB_RelearnMove2);
        pk.RelearnMove3 = (ushort)WinFormsUtil.GetIndex(CB_RelearnMove3);
        pk.RelearnMove4 = (ushort)WinFormsUtil.GetIndex(CB_RelearnMove4);
    }

    private void LoadMisc1(PKM pk)
    {
        LoadSpeciesLevelEXP(pk);
        LoadNickname(pk);
        LoadOT(pk);
        LoadIVs(pk);
        LoadEVs(pk);
        LoadMoves(pk);
    }

    private void SaveMisc1(PKM pk)
    {
        SaveSpeciesLevelEXP(pk);
        SaveNickname(pk);
        SaveOT(pk);
        SaveMoves(pk);
    }

    private void LoadMisc2(PKM pk)
    {
        LoadPokerus(pk);
        CHK_IsEgg.Checked = pk.IsEgg;
        CB_HeldItem.SelectedValue = pk.HeldItem;
        LoadClamp(CB_Form, pk.Form);
        L_FormArgument.Visible = pk is IFormArgument f && FA_Form.LoadArgument(f, pk.Species, pk.Form, pk.Context);

        ReloadToFriendshipTextBox(pk);

        Label_HatchCounter.Visible = CHK_IsEgg.Checked;
        Label_Friendship.Visible = !CHK_IsEgg.Checked;
    }

    private void ReloadToFriendshipTextBox(PKM pk)
    {
        // Show OT friendship always if it is an egg.
        var fs = (pk.IsEgg ? pk.OriginalTrainerFriendship : pk.CurrentFriendship);
        TB_Friendship.Text = fs.ToString();
    }

    private void SaveMisc2(PKM pk)
    {
        SavePokerus(pk);
        pk.IsEgg = CHK_IsEgg.Checked;
        pk.HeldItem = WinFormsUtil.GetIndex(CB_HeldItem);
        pk.Form = (byte)(CB_Form.Enabled ? CB_Form.SelectedIndex & 0x1F : 0);
        if (Entity is IFormArgument f)
            FA_Form.SaveArgument(f);

        var friendship = (byte)Util.ToInt32(TB_Friendship.Text);
        UpdateFromFriendshipTextBox(pk, friendship);
    }

    private static void UpdateFromFriendshipTextBox(PKM pk, byte friendship)
    {
        if (pk.IsEgg)
            pk.OriginalTrainerFriendship = friendship;
        else
            pk.CurrentFriendship = friendship;
    }

    private void LoadMisc3(PKM pk)
    {
        TB_PID.Text = pk.PID.ToString("X8");
        UC_Gender.Gender = pk.Gender;
        CB_Nature.SelectedValue = (int)pk.Nature;
        CB_Language.SelectedValue = pk.Language;
        CB_GameOrigin.SelectedValue = (int)pk.Version;
        CB_Ball.SelectedValue = (int)pk.Ball;
        CB_MetLocation.SelectedValue = (int)pk.MetLocation;
        TB_MetLevel.Text = pk.MetLevel.ToString();
        CHK_Fateful.Checked = pk.FatefulEncounter;

        if (pk is IContestStatsReadOnly s)
            s.CopyContestStatsTo(Contest);

        TID_Trainer.LoadIDValues(pk, pk.Format);

        // Load Extrabyte Value
        var offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
        var value = pk.Data[offset];
        TB_ExtraByte.Text = value.ToString();
    }

    private void SaveMisc3(PKM pk)
    {
        pk.PID = Util.GetHexValue(TB_PID.Text);
        pk.Nature = (Nature)WinFormsUtil.GetIndex(CB_Nature);
        pk.Gender = UC_Gender.Gender;

        if (pk is IContestStats s)
            Contest.CopyContestStatsTo(s);

        pk.FatefulEncounter = CHK_Fateful.Checked;
        pk.Ball = (byte)WinFormsUtil.GetIndex(CB_Ball);
        pk.Version = (GameVersion)WinFormsUtil.GetIndex(CB_GameOrigin);
        pk.Language = (byte)WinFormsUtil.GetIndex(CB_Language);
        pk.MetLevel = (byte)Util.ToInt32(TB_MetLevel.Text);
        pk.MetLocation = (ushort)WinFormsUtil.GetIndex(CB_MetLocation);
    }

    private void LoadMisc4(PKM pk)
    {
        CAL_MetDate.Value = pk.MetDate?.ToDateTime(new TimeOnly()) ?? new(2000, 1, 1);
        if (!EncounterStateUtil.IsMetAsEgg(pk))
        {
            CHK_AsEgg.Checked = GB_EggConditions.Enabled = false;
            CAL_EggDate.Value = new DateTime(2000, 01, 01);
        }
        else
        {
            // Was obtained initially as an egg.
            CHK_AsEgg.Checked = GB_EggConditions.Enabled = true;
            CAL_EggDate.Value = pk.EggMetDate?.ToDateTime(new TimeOnly()) ?? new(2000, 1, 1);
        }
        CB_EggLocation.SelectedValue = (int)pk.EggLocation;
    }

    private void SaveMisc4(PKM pk)
    {
        if (CHK_AsEgg.Checked) // If encountered as an egg, load the Egg Met data from fields.
        {
            pk.EggMetDate = DateOnly.FromDateTime(CAL_EggDate.Value);
            pk.EggLocation = (ushort)WinFormsUtil.GetIndex(CB_EggLocation);
        }
        else // Default Dates
        {
            pk.EggMetDate = null; // clear
            pk.EggLocation = LocationEdits.GetNoneLocation(pk);
        }

        // Met Data
        if (pk.IsEgg && pk.MetLocation == LocationEdits.GetNoneLocation(pk)) // If still an egg, it has no hatch location/date. Zero it!
            pk.MetDate = null; // clear
        else
            pk.MetDate = DateOnly.FromDateTime(CAL_MetDate.Value);

        pk.Ability = WinFormsUtil.GetIndex(HaX ? DEV_Ability : CB_Ability);
    }

    private void LoadMisc6(PKM pk)
    {
        TB_EC.Text = pk.EncryptionConstant.ToString("X8");
        DEV_Ability.SelectedValue = pk.Ability;

        // with some simple error handling
        var bitNumber = pk.AbilityNumber;
        int abilityIndex = AbilityVerifier.IsValidAbilityBits(bitNumber) ? bitNumber >> 1 : 0;
        LoadClamp(CB_Ability, abilityIndex);
        TB_AbilityNumber.Text = bitNumber.ToString();

        LoadRelearnMoves(pk);
        LoadHandlingTrainer(pk);

        if (pk is IRegionOrigin tr)
            LoadGeolocation(tr);
    }

    private void SaveMisc6(PKM pk)
    {
        pk.EncryptionConstant = Util.GetHexValue(TB_EC.Text);
        if (PIDVerifier.GetTransferEC(pk, out var ec))
            pk.EncryptionConstant = ec;

        pk.AbilityNumber = Util.ToInt32(TB_AbilityNumber.Text);

        SaveRelearnMoves(pk);
        SaveHandlingTrainer(pk);

        if (pk is IRegionOrigin tr)
            SaveGeolocation(tr);
    }

    private void LoadGeolocation(IRegionOrigin pk)
    {
        CB_Country.SelectedValue = (int)pk.Country;
        CB_SubRegion.SelectedValue = (int)pk.Region;
        CB_3DSReg.SelectedValue = (int)pk.ConsoleRegion;
    }

    private void SaveGeolocation(IRegionOrigin pk)
    {
        pk.Country = (byte)WinFormsUtil.GetIndex(CB_Country);
        pk.Region = (byte)WinFormsUtil.GetIndex(CB_SubRegion);
        pk.ConsoleRegion = (byte)WinFormsUtil.GetIndex(CB_3DSReg);
    }

    private void LoadHandlingTrainer(PKM pk)
    {
        var handler = pk.HandlingTrainerName;
        byte gender = (byte)(pk.HandlingTrainerGender & 1);

        TB_HT.Text = handler;
        UC_HTGender.Gender = gender;
        ToggleHandlerVisibility(handler.Length != 0);

        // Indicate who is currently in possession of the PKM
        UpadteHandlingTrainerBackground(pk.CurrentHandler);
    }

    private void ToggleHandlerVisibility(bool hasValue)
    {
        L_CurrentHandler.Visible = CB_Handler.Visible = UC_HTGender.Visible = hasValue;
    }

    private void UpadteHandlingTrainerBackground(int handler)
    {
        if (handler == 0) // OT
        {
            GB_OT.ForeColor = Color.Red;
            GB_nOT.ResetForeColor();
            CB_Handler.SelectedIndex = 0;
        }
        else // Handling Trainer
        {
            GB_nOT.ForeColor = Color.Red;
            GB_OT.ResetForeColor();
            CB_Handler.SelectedIndex = 1;
        }
    }

    private void SaveHandlingTrainer(PKM pk)
    {
        pk.HandlingTrainerName = TB_HT.Text;
        pk.HandlingTrainerGender = UC_HTGender.Gender;
    }

    private void LoadAbility4(PKM pk)
    {
        var index = GetAbilityIndex4(pk);
        LoadClamp(CB_Ability, index);
    }

    private static int GetAbilityIndex4(PKM pk)
    {
        var pi = pk.PersonalInfo;
        int abilityIndex = pi.GetIndexOfAbility(pk.Ability);
        if (abilityIndex < 0)
            return 0;
        if (abilityIndex >= 2)
            return 2;

        var abils = (IPersonalAbility12)pi;
        if (abils.GetIsAbility12Same())
            return pk.PIDAbility;
        return abilityIndex;
    }

    private void LoadMisc8(PK8 pk8)
    {
        CB_StatNature.SelectedValue = (int)pk8.StatNature;
        LoadClamp(Stats.CB_DynamaxLevel, pk8.DynamaxLevel);
        Stats.CHK_Gigantamax.Checked = pk8.CanGigantamax;
        CB_HTLanguage.SelectedValue = (int)pk8.HandlingTrainerLanguage;
        TB_HomeTracker.Text = pk8.Tracker.ToString("X16");
        CB_BattleVersion.SelectedValue = (int)pk8.BattleVersion;
    }

    private void SaveMisc8(PK8 pk8)
    {
        pk8.StatNature = (Nature)WinFormsUtil.GetIndex(CB_StatNature);
        pk8.DynamaxLevel = (byte)Math.Max(0, Stats.CB_DynamaxLevel.SelectedIndex);
        pk8.CanGigantamax = Stats.CHK_Gigantamax.Checked;
        pk8.HandlingTrainerLanguage = (byte)WinFormsUtil.GetIndex(CB_HTLanguage);
        pk8.BattleVersion = (GameVersion)WinFormsUtil.GetIndex(CB_BattleVersion);
    }

    private void LoadMisc8(PB8 pk8)
    {
        CB_StatNature.SelectedValue = (int)pk8.StatNature;
        LoadClamp(Stats.CB_DynamaxLevel, pk8.DynamaxLevel);
        Stats.CHK_Gigantamax.Checked = pk8.CanGigantamax;
        CB_HTLanguage.SelectedValue = (int)pk8.HandlingTrainerLanguage;
        TB_HomeTracker.Text = pk8.Tracker.ToString("X16");
        CB_BattleVersion.SelectedValue = (int)pk8.BattleVersion;
    }

    private void SaveMisc8(PB8 pk8)
    {
        pk8.StatNature = (Nature)WinFormsUtil.GetIndex(CB_StatNature);
        pk8.DynamaxLevel = (byte)Math.Max(0, Stats.CB_DynamaxLevel.SelectedIndex);
        pk8.CanGigantamax = Stats.CHK_Gigantamax.Checked;
        pk8.HandlingTrainerLanguage = (byte)WinFormsUtil.GetIndex(CB_HTLanguage);
        pk8.BattleVersion = (GameVersion)WinFormsUtil.GetIndex(CB_BattleVersion);
    }

    private void LoadMisc8(PA8 pk8)
    {
        CB_StatNature.SelectedValue = (int)pk8.StatNature;
        LoadClamp(Stats.CB_DynamaxLevel, pk8.DynamaxLevel);
        Stats.CHK_Gigantamax.Checked = pk8.CanGigantamax;
        CB_HTLanguage.SelectedValue = (int)pk8.HandlingTrainerLanguage;
        TB_HomeTracker.Text = pk8.Tracker.ToString("X16");
        CB_BattleVersion.SelectedValue = (int)pk8.BattleVersion;
        Stats.CHK_IsAlpha.Checked = pk8.IsAlpha;
        Stats.CHK_IsNoble.Checked = pk8.IsNoble;
        CB_AlphaMastered.SelectedValue = (int)pk8.AlphaMove;
    }

    private void SaveMisc8(PA8 pk8)
    {
        pk8.StatNature = (Nature)WinFormsUtil.GetIndex(CB_StatNature);
        pk8.DynamaxLevel = (byte)Math.Max(0, Stats.CB_DynamaxLevel.SelectedIndex);
        pk8.CanGigantamax = Stats.CHK_Gigantamax.Checked;
        pk8.HandlingTrainerLanguage = (byte)WinFormsUtil.GetIndex(CB_HTLanguage);
        pk8.BattleVersion = (GameVersion)WinFormsUtil.GetIndex(CB_BattleVersion);
        pk8.IsAlpha = Stats.CHK_IsAlpha.Checked;
        pk8.IsNoble = Stats.CHK_IsNoble.Checked;
        pk8.AlphaMove = (ushort)WinFormsUtil.GetIndex(CB_AlphaMastered);
    }

    private void LoadMisc9(PK9 pk9)
    {
        CB_StatNature.SelectedValue = (int)pk9.StatNature;
        CB_HTLanguage.SelectedValue = (int)pk9.HandlingTrainerLanguage;
        TB_HomeTracker.Text = pk9.Tracker.ToString("X16");
        CB_BattleVersion.SelectedValue = (int)pk9.BattleVersion;
        Stats.CB_TeraTypeOriginal.SelectedValue = (int)pk9.TeraTypeOriginal;
        Stats.CB_TeraTypeOverride.SelectedValue = (int)pk9.TeraTypeOverride;
        TB_ObedienceLevel.Text = pk9.ObedienceLevel.ToString();
    }

    private void SaveMisc9(PK9 pk9)
    {
        pk9.StatNature = (Nature)WinFormsUtil.GetIndex(CB_StatNature);
        pk9.HandlingTrainerLanguage = (byte)WinFormsUtil.GetIndex(CB_HTLanguage);
        pk9.BattleVersion = (GameVersion)WinFormsUtil.GetIndex(CB_BattleVersion);
        pk9.TeraTypeOriginal = (MoveType)WinFormsUtil.GetIndex(Stats.CB_TeraTypeOriginal);
        pk9.TeraTypeOverride = (MoveType)WinFormsUtil.GetIndex(Stats.CB_TeraTypeOverride);
        pk9.ObedienceLevel = (byte)Util.ToInt32(TB_ObedienceLevel.Text);
    }
}
