using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_MailBox : Form
    {
        private readonly SaveFile Origin;
        private readonly SaveFile SAV;

        public SAV_MailBox(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (Origin = sav).Clone();
            Gen = SAV.Generation;
            p = SAV.PartyData;
            editing = true;

            Messages = new[]
            {
                new[] { NUD_Message00, NUD_Message01, NUD_Message02, NUD_Message03 },
                new[] { NUD_Message10, NUD_Message11, NUD_Message12, NUD_Message13 },
                new[] { NUD_Message20, NUD_Message21, NUD_Message22, NUD_Message23 }
            };
            PKMLabels = new[] { L_PKM1, L_PKM2, L_PKM3, L_PKM4, L_PKM5, L_PKM6 };
            PKMHeldItems = new[] { L_HeldItem1, L_HeldItem2, L_HeldItem3, L_HeldItem4, L_HeldItem5, L_HeldItem6 };
            PKMNUDs = new[] { NUD_MailID1, NUD_MailID2, NUD_MailID3, NUD_MailID4, NUD_MailID5, NUD_MailID6 };
            AppearPKMs = new[] { CB_AppearPKM1, CB_AppearPKM2, CB_AppearPKM3 };
            Miscs = new[] { NUD_Misc1, NUD_Misc2, NUD_Misc3 };

            NUD_BoxSize.Visible = L_BoxSize.Visible = Gen == 2;
            GB_MessageTB.Visible = Gen == 2;
            GB_MessageNUD.Visible = Gen != 2;
            Messages[0][3].Visible = Messages[1][3].Visible = Messages[2][3].Visible = Gen is 4 or 5;
            NUD_AuthorSID.Visible = Gen != 2;
            Label_OTGender.Visible = CB_AuthorLang.Visible = CB_AuthorVersion.Visible = Gen is 4 or 5;
            L_AppearPKM.Visible = AppearPKMs[0].Visible = Gen != 5;
            AppearPKMs[1].Visible = AppearPKMs[2].Visible = Gen == 4;
            NUD_MessageEnding.Visible = Gen == 5;
            L_MiscValue.Visible = NUD_Misc1.Visible = NUD_Misc2.Visible = NUD_Misc3.Visible = Gen == 5;

            for (int i = p.Count; i < 6; i++)
                PKMNUDs[i].Visible = PKMLabels[i].Visible = PKMHeldItems[i].Visible = false;
            if (Gen != 3)
            {
                for (int i = 0; i < PKMNUDs.Length; i++)
                {
                    PKMNUDs[i].Value = i;
                    PKMNUDs[i].Enabled = false;
                }
            }

            switch (SAV)
            {
                case SAV2 sav2:
                    m = new Mail2[6 + 10];
                    for (int i = 0; i < m.Length; i++)
                        m[i] = new Mail2(sav2, i);

                    NUD_BoxSize.Value = SAV.Data[0x834];
                    MailItemID = new[] { 0x9E, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD };
                    PartyBoxCount = 6;
                    break;
                case SAV3 sav3:
                    m = new Mail3[6 + 10];
                    for (int i = 0; i < m.Length; i++)
                    {
                        var ofs = sav3.GetMailOffset(i);
                        var data = sav.GetData(ofs, Mail3.SIZE);
                        m[i] = new Mail3(data, ofs, sav3.Japanese);
                    }

                    MailItemID = Enumerable.Range(0x79, 12).ToArray();
                    PartyBoxCount = 6;
                    break;
                case SAV4 sav4:
                    m = new Mail4[p.Count + 20];
                    for (int i = 0; i < p.Count; i++)
                        m[i] = new Mail4(((PK4)p[i]).GetHeldMailData());
                    for (int i = p.Count, j = 0; i < m.Length; i++, j++)
                    {
                        int ofs = sav4.GetMailOffset(j);
                        m[i] = new Mail4(sav4.GetMailData(ofs), ofs);
                    }
                    var l4 = (Mail4)m.Last();
                    ResetVer = l4.AuthorVersion;
                    ResetLang = l4.AuthorLanguage;
                    MailItemID = Enumerable.Range(0x89, 12).ToArray();
                    PartyBoxCount = p.Count;
                    break;
                case SAV5 sav5:
                    m = new Mail5[p.Count + 20];
                    for (int i = 0; i < p.Count; i++)
                        m[i] = new Mail5(((PK5)p[i]).GetHeldMailData());
                    for (int i = p.Count, j = 0; i < m.Length; i++, j++)
                    {
                        int ofs = SAV5.GetMailOffset(j);
                        var data = sav5.GetMailData(ofs);
                        m[i] = new Mail5(data, ofs);
                    }
                    var l5 = (Mail5)m.Last();
                    ResetVer = l5.AuthorVersion;
                    ResetLang = l5.AuthorLanguage;
                    MailItemID = Enumerable.Range(0x89, 12).ToArray();
                    PartyBoxCount = p.Count;
                    break;
            }
            MakePartyList();
            MakePCList();

            if (Gen is 2 or 3)
            {
                CB_AppearPKM1.Items.Clear();
                CB_AppearPKM1.InitializeBinding();
                CB_AppearPKM1.DataSource = new BindingSource(GameInfo.FilteredSources.Species.ToList(), null);
            }
            else if (Gen is 4 or 5)
            {
                var species = GameInfo.FilteredSources.Species.ToList();
                foreach (ComboBox a in AppearPKMs)
                {
                    a.Items.Clear();
                    a.InitializeBinding();
                    a.DataSource = new BindingSource(species, null);
                }

                CB_AuthorVersion.Items.Clear();
                CB_AuthorVersion.InitializeBinding();
                CB_AuthorVersion.DataSource = new BindingSource(Gen == 4
                    ? new[] {
                        new ComboItem("Diamond", (int)GameVersion.D),
                        new ComboItem("Pearl", (int)GameVersion.P),
                        new ComboItem("Platinum", (int)GameVersion.Pt),
                        new ComboItem("HeartGold", (int)GameVersion.HG),
                        new ComboItem("SoulSilver", (int)GameVersion.SS),
                    }
                    : new[] {
                        new ComboItem("Black", (int)GameVersion.B),
                        new ComboItem("White", (int)GameVersion.W),
                        new ComboItem("Black2", (int)GameVersion.B2),
                        new ComboItem("White2", (int)GameVersion.W2),
                    }, null);

                CB_AuthorLang.Items.Clear();
                CB_AuthorLang.InitializeBinding();
                CB_AuthorLang.DataSource = new BindingSource(new[] {
                    // not sure
                    new ComboItem("JPN", 1),
                    new ComboItem("ENG", 2),
                    new ComboItem("FRE", 3),
                    new ComboItem("ITA", 4),
                    new ComboItem("GER", 5),
                    new ComboItem("ESP", 7),
                    new ComboItem("KOR", 8),
                }, null);
            }

            var ItemList = GameInfo.Strings.GetItemStrings(Gen, SAV.Version);
            CB_MailType.Items.Clear();
            CB_MailType.Items.Add(ItemList[0]);
            foreach (int item in MailItemID)
                CB_MailType.Items.Add(ItemList[item]);

            LoadPKM(true);
            entry = -1;
            editing = false;
            if (LB_PartyHeld.Items.Count > 0)
                LB_PartyHeld.SelectedIndex = 0;
            else if (LB_PCBOX.Items.Count > 0)
                LB_PCBOX.SelectedIndex = 0;
        }

        private readonly int[] HoennListMixed = {
                277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,
            304,305,309,310,392,393,394,311,312,306,307,364,365,366,301,302,303,370,371,372,335,336,350,320,315,316,
                                    322,355,382,383,384,356,357,337,338,353,354,386,387,363,367,368,330,331,313,314,
                                    339,340,321,351,352,308,332,333,334,344,345,358,359,380,379,348,349,323,324,
                                326,327,318,319,388,389,390,391,328,329,385,317,377,378,361,362,369,411,376,360,
                                346,347,341,342,343,373,374,375,381,325,395,396,397,398,399,400,
                401,402,403,407,408,404,405,406,409,410
        };

        private void LoadList()
        {
            if (entry < PartyBoxCount) MakePartyList();
            else MakePCList();
        }

        private void MakePartyList()
        {
            LB_PartyHeld.BeginUpdate();
            LB_PartyHeld.Items.Clear();
            for (int i = 0; i < PartyBoxCount; i++)
                LB_PartyHeld.Items.Add(GetLBLabel(i));
            LB_PartyHeld.EndUpdate();
        }

        private void MakePCList()
        {
            LB_PCBOX.BeginUpdate();
            LB_PCBOX.Items.Clear();
            if (Gen == 2)
            {
                for (int i = PartyBoxCount, j = 0, boxsize = (int)NUD_BoxSize.Value; i < m.Length; i++, j++)
                {
                    if (j < boxsize)
                        LB_PCBOX.Items.Add(GetLBLabel(i));
                }
            }
            else
            {
                for (int i = PartyBoxCount; i < m.Length; i++)
                    LB_PCBOX.Items.Add(GetLBLabel(i));
            }
            LB_PCBOX.EndUpdate();
        }

        private void LoadPKM(bool isInit)
        {
            editing = true;
            for (int i = 0; i < p.Count; i++)
            {
                if (isInit)
                    PKMLabels[i].Text = GetSpeciesNameFromCB(p[i].Species);
                int j = Array.IndexOf(MailItemID, p[i].HeldItem);
                PKMHeldItems[i].Text = j >= 0 ? CB_MailType.Items[j + 1].ToString() : "(not Mail)";
                if (Gen != 3)
                    continue;
                int k = ((PK3)p[i]).HeldMailID;
                PKMNUDs[i].Value = k >= -1 && k <= 5 ? k : -1;
            }
            editing = false;
        }

        private readonly Mail[] m = null!;
        private bool editing;
        private int entry;
        private readonly NumericUpDown[][] Messages;
        private readonly NumericUpDown[] PKMNUDs, Miscs;
        private readonly Label[] PKMLabels, PKMHeldItems;
        private readonly ComboBox[] AppearPKMs;
        private readonly int Gen;
        private readonly byte ResetVer, ResetLang;
        private readonly int PartyBoxCount;
        private string loadedLBItemLabel = null!;
        private bool LabelValue_GenderF;
        private readonly int[] MailItemID = null!;
        private readonly IList<PKM> p;

        private void Save()
        {
            switch (Gen)
            {
                case 2:
                    foreach (var n in m) n.CopyTo(SAV);
                    // duplicate
                    int ofs = 0x600;
                    int len = 0x2F * 6;
                    Array.Copy(SAV.Data, ofs, SAV.Data, ofs + len, len);
                    ofs += len << 1;
                    SAV.Data[ofs] = (byte)NUD_BoxSize.Value;
                    len = (0x2F * 10) + 1;
                    Array.Copy(SAV.Data, ofs, SAV.Data, ofs + len, len);
                    break;
                case 3:
                    foreach (var n in m) n.CopyTo(SAV);
                    break;
                case 4:
                    for (int i = 0; i < p.Count; i++)
                        m[i].CopyTo((PK4)p[i]);
                    for (int i = p.Count; i < m.Length; i++)
                        m[i].CopyTo(SAV);
                    break;
                case 5:
                    for (int i = 0; i < p.Count; i++)
                        m[i].CopyTo((PK5)p[i]);
                    for (int i = p.Count; i < m.Length; i++)
                        m[i].CopyTo(SAV);
                    break;
            }
            if (p.Count > 0)
                SAV.PartyData = p;
        }

        private void TempSave()
        {
            Mail mail = m[entry];
            mail.AuthorName = TB_AuthorName.Text;
            mail.AuthorTID = (ushort)NUD_AuthorTID.Value;
            mail.MailType = CBIndexToMailType(CB_MailType.SelectedIndex);
            // ReSharper disable once ConstantNullCoalescingCondition
            int v = (int?)CB_AppearPKM1.SelectedValue ?? 0;
            if (Gen == 2)
            {
                mail.AppearPKM = v;
                mail.SetMessage(TB_MessageBody21.Text, TB_MessageBody22.Text);
                return;
            }
            mail.AuthorSID = (ushort)NUD_AuthorSID.Value;
            for (int y = 0, xc = Gen == 3 ? 3 : 4; y < 3; y++)
            {
                for (int x = 0; x < xc; x++)
                    mail.SetMessage(y, x, (ushort)Messages[y][x].Value);
            }
            if (Gen == 3)
            {
                mail.AppearPKM = v < 252 ? v : HoennListMixed[v - 252];
                return;
            }

            // ReSharper disable once ConstantNullCoalescingCondition
            mail.AuthorVersion = (byte)((int?)CB_AuthorVersion.SelectedValue ?? 0);

            // ReSharper disable once ConstantNullCoalescingCondition
            mail.AuthorLanguage = (byte)((int?)CB_AuthorLang.SelectedValue ?? 0);

            mail.AuthorGender = (byte)((mail.AuthorGender & 0xFE) | (LabelValue_GenderF ? 1 : 0));
            switch (mail)
            {
                case Mail4 m4:
                    for (int i = 0; i < AppearPKMs.Length; i++)
                        m4.SetAppearPKM(i, (AppearPKMs[i].SelectedValue as int?) + 7 ?? 0);
                    break;
                case Mail5 m5:
                    for (int i = 0; i < Miscs.Length; i++)
                        m5.SetMisc(i, (ushort)Miscs[i].Value);
                    m5.MessageEnding = (ushort)NUD_MessageEnding.Value;
                    break;
            }
        }

        private List<string> CheckValid()
        {
            var ret = new List<string>();
            // Gen3
            // A: held item is mail, but heldMailID is not 0 to 5. it should be 0 to 5, or held not mail.
            // B: held item is mail, but mail is empty(mail type is 0). it should be not empty, or held not mail and heldMailId -1.
            // C: held item is not mail, but heldMailID is not -1. it should be -1, or held mail and mail not empty.
            // D: other pkm have same heldMailID. it should be different.
            // E: mail is not empty, but no pkm refer to the mail. it should be empty, or someone refer to the mail.
            if (Gen == 3)
            {
                int[] heldMailIDs = new int[p.Count];
                for (int i = 0; i < p.Count; i++)
                {
                    int h = ((PK3)p[i]).HeldMailID;
                    heldMailIDs[i] = h;
                    if (ItemIsMail(p[i].HeldItem))
                    {
                        if (h is < 0 or > 5) //A
                            ret.Add($"Party#{i + 1} MailID mismatch");
                        else if (m[h].IsEmpty == true) //B
                            ret.Add($"Party#{i + 1} MailID mismatch");
                    }
                    else if (h != -1) //C
                    {
                        ret.Add($"Party#{i + 1} MailID mismatch");
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    var index = i;
                    if (heldMailIDs.Count(v => v == index) > 1) //D
                        ret.Add($"MailID{i} duplicated");
                    if (m[i].IsEmpty == false && heldMailIDs.All(v => v != index)) //E
                        ret.Add($"MailID{i} not referred");
                }
            }
            // Gen2, Gen4
            // P: held item is mail, but mail is empty(invalid mail type. g2:not 181 to 189, g4:12 to 255). it should be not empty or held not mail.
            // Q: held item is not mail, but mail is not empty. it should be empty or held mail.
            else if (Gen is 2 or 4)
            {
                for (int i = 0; i < p.Count; i++)
                {
                    if (ItemIsMail(p[i].HeldItem))
                    {
                        if (m[i].IsEmpty == true) //P
                            ret.Add($"MailID{i} MailType mismatch");
                    }
                    else if (m[i].IsEmpty == false) //Q
                    {
                        ret.Add($"MailID{i} MailType mismatch");
                    }
                }
            }
            // Gen5
            // P
            // gen5, move mail to pc will not erase mail data, still remains, duplicates.
            else if (Gen == 5)
            {
                for (int i = 0; i < p.Count; i++)
                {
                    if (ItemIsMail(p[i].HeldItem))
                    {
                        if (m[i].IsEmpty == true) //P
                            ret.Add($"MailID{i} MailType mismatch");
                    }
                }
            }
            // Gen*
            // Z: mail type is illegal
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i].IsEmpty == null) // Z
                    ret.Add($"MailID{i} MailType mismatch");
            }

            return ret;
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            if (entry >= 0) TempSave();
            Save();
            var Err = CheckValid();
            if (Err.Count != 0 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"{Err.Aggregate($"Validation Error. Save?{Environment.NewLine}", (tmp, v) => $"{tmp}{Environment.NewLine}{v}")}"))
                return;
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private string GetLBLabel(int index) => m[index].IsEmpty != true ? $"{index}: From {m[index].AuthorName}" : $"{index}:  (empty)";
        private bool ItemIsMail(int itemID) => Array.IndexOf(MailItemID, itemID) >= 0;
        private int MailTypeToCBIndex(Mail mail) => Gen <= 3 ? 1 + Array.IndexOf(MailItemID, mail.MailType) : (mail.IsEmpty == false ? 1 + mail.MailType : 0);
        private int CBIndexToMailType(int cbindex) => Gen <= 3 ? (cbindex > 0 ? MailItemID[cbindex - 1] : 0) : (cbindex > 0 ? cbindex - 1 : 0xFF);

        private string GetSpeciesNameFromCB(int index)
        {
            foreach (var i in CB_AppearPKM1.Items.OfType<ComboItem>())
            {
                if (index == i.Value)
                    return i.Text;
            }
            return "PKM";
        }

        private DialogResult ModifyHeldItem()
        {
            DialogResult ret = DialogResult.Abort;
            var s = p.Select((pkm, i) => ((sbyte)PKMNUDs[i].Value == entry) && ItemIsMail(pkm.HeldItem) ? pkm : null).ToArray();
            if (s.All(v => v == null))
                return ret;
            System.Media.SystemSounds.Question.Play();
            var msg = $"{s.Select((v, i) => v == null ? string.Empty : $"{Environment.NewLine}  {PKMLabels[i].Text}: {PKMHeldItems[i].Text} -> {CB_MailType.Items[0]}").Aggregate($"Modify PKM's HeldItem?{Environment.NewLine}", (tmp, v) => $"{tmp}{v}")}{Environment.NewLine}{Environment.NewLine}Yes: Delete Mail & Modify PKM{Environment.NewLine}No: Delete Mail";
            ret = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel, msg);
            if (ret != DialogResult.Yes)
                return ret;
            foreach (var pkm in s)
            {
                if (pkm == null)
                    continue;

                pkm.HeldItem = 0;
                if (Gen == 3)
                    ((PK3)pkm).HeldMailID = -1;
            }
            LoadPKM(false);
            return ret;
        }

        private void B_Delete_Click(object sender, EventArgs e)
        {
            if (entry < 0)
                return;
            if (entry < p.Count)
            {
                if (ModifyHeldItem() == DialogResult.Cancel)
                    return;
            }
            switch (m[entry])
            {
                case Mail4 m4: m4.SetBlank(ResetLang, ResetVer); break;
                case Mail5 m5: m5.SetBlank(ResetLang, ResetVer); break;
                default: m[entry].SetBlank(); break;
            }
            LoadList();
            LoadMail();
        }

        private void EntryControl(object sender, EventArgs e)
        {
            if (editing) return;
            editing = true;
            int partyIndex = LB_PartyHeld.SelectedIndex;
            int pcIndex = LB_PCBOX.SelectedIndex;
            if (entry >= 0)
            {
                TempSave();
                if (GetLBLabel(entry) != loadedLBItemLabel)
                    LoadList();
            }
            if (sender == LB_PartyHeld && partyIndex >= 0)
            {
                entry = partyIndex;
                LB_PartyHeld.SelectedIndex = partyIndex;
                LB_PCBOX.SelectedIndex = -1;
            }
            else if (sender == LB_PCBOX && pcIndex >= 0)
            {
                entry = PartyBoxCount + pcIndex;
                LB_PCBOX.SelectedIndex = pcIndex;
                LB_PartyHeld.SelectedIndex = -1;
            }
            else
            {
                entry = -1;
            }
            editing = false;
            if (entry >= 0)
            {
                LoadMail();
                loadedLBItemLabel = GetLBLabel(entry);
            }
        }

        private void LoadMail()
        {
            editing = true;
            Mail mail = m[entry];
            TB_AuthorName.Text = mail.AuthorName;
            NUD_AuthorTID.Value = mail.AuthorTID;
            CB_MailType.SelectedIndex = MailTypeToCBIndex(mail);
            int v = mail.AppearPKM;
            if (Gen == 2)
            {
                AppearPKMs[0].SelectedValue = v;
                TB_MessageBody21.Text = mail.GetMessage(false);
                TB_MessageBody22.Text = mail.GetMessage(true);
                editing = false;
                return;
            }
            NUD_AuthorSID.Value = mail.AuthorSID;
            for (int y = 0, xc = Gen == 3 ? 3 : 4; y < 3; y++)
            {
                for (int x = 0; x < xc; x++)
                    Messages[y][x].Value = mail.GetMessage(y, x);
            }
            if (Gen == 3)
            {
                if (v < 252)
                {
                    AppearPKMs[0].SelectedValue = v;
                }
                else
                {
                    v = Array.IndexOf(HoennListMixed, v);
                    AppearPKMs[0].SelectedValue = v >= 0 ? (252 + v) : 0;
                }
                editing = false;
                return;
            }
            CB_AuthorVersion.SelectedValue = (int)mail.AuthorVersion;
            CB_AuthorLang.SelectedValue = (int)mail.AuthorLanguage;
            LabelValue_GenderF = (mail.AuthorGender & 1) != 0;
            LoadOTlabel();
            switch (mail)
            {
                case Mail4 m4:
                    for (int i = 0; i < AppearPKMs.Length; i++)
                        AppearPKMs[i].SelectedValue = Math.Max(0, m4.GetAppearPKM(i) - 7);
                    break;
                case Mail5 m5:
                    for (int i = 0; i < Miscs.Length; i++)
                        Miscs[i].Value = m5.GetMisc(i);
                    NUD_MessageEnding.Value = m5.MessageEnding;
                    break;
            }
            editing = false;
        }

        private readonly string[] gendersymbols = { "♂", "♀" };

        private void LoadOTlabel()
        {
            Label_OTGender.Text = gendersymbols[LabelValue_GenderF ? 1 : 0];
            Label_OTGender.ForeColor = Main.Draw.GetGenderColor(LabelValue_GenderF ? 1 : 0);
        }

        private void Label_OTGender_Click(object sender, EventArgs e)
        {
            LabelValue_GenderF ^= true;
            LoadOTlabel();
        }

        private void NUD_BoxSize_ValueChanged(object sender, EventArgs e) => MakePCList();

        private void NUD_MailIDn_ValueChanged(object sender, EventArgs e)
        {
            if (editing || Gen != 3) return;
            int index = Array.IndexOf(PKMNUDs, (NumericUpDown)sender);
            if (index < 0 || index >= p.Count) return;
            ((PK3)p[index]).HeldMailID = (sbyte)PKMNUDs[index].Value;
        }
    }
}
