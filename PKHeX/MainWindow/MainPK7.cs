using System;
using System.Drawing;

namespace PKHeX
{
    public partial class Main
    {
        private void populateFieldsPK7()
        {
            PK7 pk7 = pkm as PK7;
            if (pk7 == null)
                return;

            // Do first
            pk7.Stat_Level = HaX && pk7.Stat_HPMax != 0 ? pk7.Stat_Level : PKX.getLevel(pk7.Species, pk7.EXP);
            if (pk7.Stat_Level == 100)
                pk7.EXP = PKX.getEXP(pk7.Stat_Level, pk7.Species);

            CB_Species.SelectedValue = pk7.Species;
            TB_Level.Text = pk7.Stat_Level.ToString();
            TB_EXP.Text = pk7.EXP.ToString();

            // Load rest
            TB_EC.Text = pk7.EncryptionConstant.ToString("X8");
            CHK_Fateful.Checked = pk7.FatefulEncounter;
            CHK_IsEgg.Checked = pk7.IsEgg;
            CHK_Nicknamed.Checked = pk7.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk7.OT_Gender];
            Label_OTGender.ForeColor = pk7.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = pk7.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk7.HeldItem;
            TB_AbilityNumber.Text = pk7.AbilityNumber.ToString();
            CB_Ability.SelectedIndex = pk7.AbilityNumber < 6 ? pk7.AbilityNumber >> 1 : 0; // with some simple error handling
            CB_Nature.SelectedValue = pk7.Nature;
            TB_TID.Text = pk7.TID.ToString("00000");
            TB_SID.Text = pk7.SID.ToString("00000");
            TB_Nickname.Text = pk7.Nickname;
            TB_OT.Text = pk7.OT_Name;
            TB_OTt2.Text = pk7.HT_Name;
            TB_Friendship.Text = pk7.CurrentFriendship.ToString();
            if (pk7.CurrentHandler == 1)  // HT
            {
                GB_nOT.BackgroundImage = mixedHighlight;
                GB_OT.BackgroundImage = null;
            }
            else                  // = 0
            {
                GB_OT.BackgroundImage = mixedHighlight;
                GB_nOT.BackgroundImage = null;
            }
            CB_Language.SelectedValue = pk7.Language;
            CB_Country.SelectedValue = pk7.Country;
            CB_SubRegion.SelectedValue = pk7.Region;
            CB_3DSReg.SelectedValue = pk7.ConsoleRegion;
            CB_GameOrigin.SelectedValue = pk7.Version;
            CB_EncounterType.SelectedValue = pk7.Gen4 ? pk7.EncounterType : 0;
            CB_Ball.SelectedValue = pk7.Ball;

            CAL_MetDate.Value = pk7.MetDate ?? new DateTime(2000, 1, 1);

            if (pk7.Egg_Location != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = pk7.Egg_Location;
                CAL_EggDate.Value = pk7.EggMetDate ?? new DateTime(2000, 1, 1);
            }
            else { CAL_EggDate.Value = new DateTime(2000, 01, 01); CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = pk7.Met_Location;

            // Set CT Gender to None if no CT, else set to gender symbol.
            Label_CTGender.Text = pk7.HT_Name == "" ? "" : gendersymbols[pk7.HT_Gender % 2];
            Label_CTGender.ForeColor = pk7.HT_Gender == 1 ? Color.Red : Color.Blue;

            TB_MetLevel.Text = pk7.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk7.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk7.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk7.PKRS_Strain;
            CHK_Cured.Checked = pk7.PKRS_Strain > 0 && pk7.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk7.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = pk7.CNT_Cool.ToString();
            TB_Beauty.Text = pk7.CNT_Beauty.ToString();
            TB_Cute.Text = pk7.CNT_Cute.ToString();
            TB_Smart.Text = pk7.CNT_Smart.ToString();
            TB_Tough.Text = pk7.CNT_Tough.ToString();
            TB_Sheen.Text = pk7.CNT_Sheen.ToString();

            TB_HPIV.Text = pk7.IV_HP.ToString();
            TB_ATKIV.Text = pk7.IV_ATK.ToString();
            TB_DEFIV.Text = pk7.IV_DEF.ToString();
            TB_SPEIV.Text = pk7.IV_SPE.ToString();
            TB_SPAIV.Text = pk7.IV_SPA.ToString();
            TB_SPDIV.Text = pk7.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk7.HPType;

            TB_HPEV.Text = pk7.EV_HP.ToString();
            TB_ATKEV.Text = pk7.EV_ATK.ToString();
            TB_DEFEV.Text = pk7.EV_DEF.ToString();
            TB_SPEEV.Text = pk7.EV_SPE.ToString();
            TB_SPAEV.Text = pk7.EV_SPA.ToString();
            TB_SPDEV.Text = pk7.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk7.Move1;
            CB_Move2.SelectedValue = pk7.Move2;
            CB_Move3.SelectedValue = pk7.Move3;
            CB_Move4.SelectedValue = pk7.Move4;
            CB_RelearnMove1.SelectedValue = pk7.RelearnMove1;
            CB_RelearnMove2.SelectedValue = pk7.RelearnMove2;
            CB_RelearnMove3.SelectedValue = pk7.RelearnMove3;
            CB_RelearnMove4.SelectedValue = pk7.RelearnMove4;
            CB_PPu1.SelectedIndex = pk7.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk7.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk7.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk7.Move4_PPUps;
            TB_PP1.Text = pk7.Move1_PP.ToString();
            TB_PP2.Text = pk7.Move2_PP.ToString();
            TB_PP3.Text = pk7.Move3_PP.ToString();
            TB_PP4.Text = pk7.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk7.AltForm ? pk7.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk7.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();

            TB_EXP.Text = pk7.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk7.Gender];
            Label_Gender.ForeColor = pk7.Gender == 2 ? Label_Species.ForeColor : (pk7.Gender == 1 ? Color.Red : Color.Blue);

            // Highlight the Current Handler
            clickGT(pk7.CurrentHandler == 1 ? GB_nOT : GB_OT, null);

            if (HaX)
                DEV_Ability.SelectedValue = pk7.Ability;
        }
        private PKM preparePK7()
        {
            PK7 pk7 = pkm as PK7;
            if (pk7 == null)
                return null;

            // Repopulate PK6 with Edited Stuff
            if (Util.getIndex(CB_GameOrigin) < 24)
            {
                uint EC = Util.getHEXval(TB_EC.Text);
                uint PID = Util.getHEXval(TB_PID.Text);
                uint SID = Util.ToUInt32(TB_TID.Text);
                uint TID = Util.ToUInt32(TB_TID.Text);
                uint LID = PID & 0xFFFF;
                uint HID = PID >> 16;
                uint XOR = TID ^ LID ^ SID ^ HID;

                // Ensure we don't have a shiny.
                if (XOR >> 3 == 1) // Illegal, fix. (not 16<XOR>=8)
                {
                    // Keep as shiny, so we have to mod the PID
                    PID ^= XOR;
                    TB_PID.Text = PID.ToString("X8");
                    TB_EC.Text = PID.ToString("X8");
                }
                else if ((XOR ^ 0x8000) >> 3 == 1 && PID != EC)
                    TB_EC.Text = (PID ^ 0x80000000).ToString("X8");
                else // Not Illegal, no fix.
                    TB_EC.Text = PID.ToString("X8");
            }

            pk7.EncryptionConstant = Util.getHEXval(TB_EC.Text);
            pk7.Checksum = 0; // 0 CHK for now

            // Block A
            pk7.Species = Util.getIndex(CB_Species);
            pk7.HeldItem = Util.getIndex(CB_HeldItem);
            pk7.TID = Util.ToInt32(TB_TID.Text);
            pk7.SID = Util.ToInt32(TB_SID.Text);
            pk7.EXP = Util.ToUInt32(TB_EXP.Text);
            pk7.Ability = (byte)Array.IndexOf(GameStrings.abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));
            pk7.AbilityNumber = Util.ToInt32(TB_AbilityNumber.Text);   // Number
            // pkx[0x16], pkx[0x17] are handled by the Medals UI (Hits & Training Bag)
            pk7.PID = Util.getHEXval(TB_PID.Text);
            pk7.Nature = (byte)Util.getIndex(CB_Nature);
            pk7.FatefulEncounter = CHK_Fateful.Checked;
            pk7.Gender = PKX.getGender(Label_Gender.Text);
            pk7.AltForm = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
            pk7.EV_HP = Util.ToInt32(TB_HPEV.Text);       // EVs
            pk7.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk7.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk7.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk7.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk7.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk7.CNT_Cool = Util.ToInt32(TB_Cool.Text);       // CNT
            pk7.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            pk7.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            pk7.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            pk7.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            pk7.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            pk7.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk7.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            // Already in buff (then transferred to new pkx)
            // 0x2C, 0x2D, 0x2E, 0x2F
            // 0x30, 0x31, 0x32, 0x33
            // 0x34, 0x35, 0x36, 0x37
            // 0x38, 0x39

            // Unused
            // 0x3A, 0x3B
            // 0x3C, 0x3D, 0x3E, 0x3F

            // Block B
            // Convert Nickname field back to bytes
            pk7.Nickname = TB_Nickname.Text;
            pk7.Move1 = Util.getIndex(CB_Move1);
            pk7.Move2 = Util.getIndex(CB_Move2);
            pk7.Move3 = Util.getIndex(CB_Move3);
            pk7.Move4 = Util.getIndex(CB_Move4);
            pk7.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk7.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk7.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk7.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk7.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk7.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk7.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk7.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;
            pk7.RelearnMove1 = Util.getIndex(CB_RelearnMove1);
            pk7.RelearnMove2 = Util.getIndex(CB_RelearnMove2);
            pk7.RelearnMove3 = Util.getIndex(CB_RelearnMove3);
            pk7.RelearnMove4 = Util.getIndex(CB_RelearnMove4);
            // 0x72 - Ribbon editor sets this flag (Secret Super Training)
            // 0x73
            pk7.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk7.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk7.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk7.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk7.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk7.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk7.IsEgg = CHK_IsEgg.Checked;
            pk7.IsNicknamed = CHK_Nicknamed.Checked;

            // Block C
            pk7.HT_Name = TB_OTt2.Text;

            // 0x90-0xAF
            pk7.HT_Gender = PKX.getGender(Label_CTGender.Text) & 1;
            // Plus more, set by MemoryAmie (already in buff)

            // Block D
            pk7.OT_Name = TB_OT.Text;
            pk7.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            DateTime? egg_date = null;
            int egg_location = 0;
            if (CHK_AsEgg.Checked)      // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_date = CAL_EggDate.Value;
                egg_location = Util.getIndex(CB_EggLocation);
            }
            // Egg Met Data
            pk7.EggMetDate = egg_date;
            pk7.Egg_Location = egg_location;
            // Met Data
            pk7.MetDate = CAL_MetDate.Value;
            pk7.Met_Location = Util.getIndex(CB_MetLocation);

            if (pk7.IsEgg && pk7.Met_Location == 0)    // If still an egg, it has no hatch location/date. Zero it!
                pk7.MetDate = null;

            // 0xD7 Unknown

            pk7.Ball = Util.getIndex(CB_Ball);
            pk7.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk7.OT_Gender = PKX.getGender(Label_OTGender.Text);
            pk7.EncounterType = Util.getIndex(CB_EncounterType);
            pk7.Version = Util.getIndex(CB_GameOrigin);
            pk7.Country = Util.getIndex(CB_Country);
            pk7.Region = Util.getIndex(CB_SubRegion);
            pk7.ConsoleRegion = Util.getIndex(CB_3DSReg);
            pk7.Language = Util.getIndex(CB_Language);
            // 0xE4-0xE7

            // Toss in Party Stats
            Array.Resize(ref pk7.Data, pk7.SIZE_PARTY);
            pk7.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk7.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk7.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk7.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk7.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk7.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk7.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk7.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            // Unneeded Party Stats (Status, Flags, Unused)
            pk7.Data[0xE8] = pk7.Data[0xE9] = pk7.Data[0xEA] = pk7.Data[0xEB] =
                pk7.Data[0xED] = pk7.Data[0xEE] = pk7.Data[0xEF] =
                pk7.Data[0xFE] = pk7.Data[0xFF] = pk7.Data[0x100] =
                pk7.Data[0x101] = pk7.Data[0x102] = pk7.Data[0x103] = 0;

            // Hax Illegality
            if (HaX)
            {
                pk7.Ability = (byte)Util.getIndex(DEV_Ability);
                pk7.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk7.FixMoves();
            pk7.FixRelearn();

            // Fix Handler (Memories & OT) -- no foreign memories for Pokemon without a foreign trainer (none for eggs)
            if (Menu_ModifyPKM.Checked)
                pk7.FixMemories();

            // PKX is now filled
            pk7.RefreshChecksum();
            return pk7;
        }
    }
}
