using System;
using System.Collections.Generic;
using System.Drawing;
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
            Messages[0][3].Visible = Messages[1][3].Visible = Messages[2][3].Visible = Gen == 4 || Gen == 5;
            NUD_AuthorSID.Visible = Gen != 2;
            Label_OTGender.Visible = CB_AuthorLang.Visible = CB_AuthorVersion.Visible = Gen == 4 || Gen == 5;
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
                    break;
                case SAV3 sav3:
                    m = new Mail3[6 + 10];
                    for (int i = 0; i < m.Length; i++)
                        m[i] = new Mail3(sav3, i);

                    MailItemID = Enumerable.Range(0x79, 12).ToArray();
                    break;
                case SAV4 sav4:
                    m = new Mail4[p.Count + 20];
                    for (int i = 0; i < p.Count; i++)
                        m[i] = new Mail4(((PK4) p[i]).HeldMailData);
                    for (int i = p.Count, j = 0; i < m.Length; i++, j++)
                        m[i] = new Mail4(sav4, j);
                    var l4 = m.Last() as Mail4;
                    ResetVer = l4.AuthorVersion;
                    ResetLang = l4.AuthorLanguage;
                    MailItemID = Enumerable.Range(0x89, 12).ToArray();
                    break;
                case SAV5 sav5:
                    m = new Mail5[p.Count + 20];
                    for (int i = 0; i < p.Count; i++)
                        m[i] = new Mail5(((PK5) p[i]).HeldMailData);
                    for (int i = p.Count, j = 0; i < m.Length; i++, j++)
                        m[i] = new Mail5(sav5, j);
                    var l5 = m.Last() as Mail5;
                    ResetVer = l5.AuthorVersion;
                    ResetLang = l5.AuthorLanguage;
                    MailItemID = Enumerable.Range(0x89, 12).ToArray();
                    break;
            }
            MakePartyList();
            MakePCList();

            if (Gen == 2 || Gen == 3)
            {
                CB_AppearPKM1.Items.Clear();
                CB_AppearPKM1.InitializeBinding();
                CB_AppearPKM1.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(id => id.Value <= sav.MaxSpeciesID).ToList(), null);
            }
            else if (Gen == 4 || Gen == 5)
            {
                var species = GameInfo.SpeciesDataSource.Where(id => id.Value <= sav.MaxSpeciesID).ToList();
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
                        new ComboItem { Text = "Diamond", Value = (int)GameVersion.D },
                        new ComboItem { Text = "Pearl", Value = (int)GameVersion.P },
                        new ComboItem { Text = "Platinum", Value = (int)GameVersion.Pt },
                        new ComboItem { Text = "HeartGold", Value = (int)GameVersion.HG },
                        new ComboItem { Text = "SoulSilver", Value = (int)GameVersion.SS },
                    }.ToList()
                    : new[] {
                        new ComboItem { Text = "Black", Value = (int)GameVersion.B },
                        new ComboItem { Text = "White", Value = (int)GameVersion.W },
                        new ComboItem { Text = "Black2", Value = (int)GameVersion.B2 },
                        new ComboItem { Text = "White2", Value = (int)GameVersion.W2 },
                    }.ToList(), null);

                CB_AuthorLang.Items.Clear();
                CB_AuthorLang.InitializeBinding();
                CB_AuthorLang.DataSource = new BindingSource(new[] {
                    // not sure
                    new ComboItem { Text = "JPN", Value = 1 },
                    new ComboItem { Text = "ENG", Value = 2 },
                    new ComboItem { Text = "FRE", Value = 3 },
                    new ComboItem { Text = "ITA", Value = 4 },
                    new ComboItem { Text = "GER", Value = 5 },
                    new ComboItem { Text = "ESP", Value = 7 },
                    new ComboItem { Text = "KOR", Value = 8 },
                }.ToList(), null);
            }

            var ItemList = GameInfo.Strings.GetItemStrings(Gen, SAV.Version);
            CB_MailType.Items.Clear();
            CB_MailType.Items.Add(ItemList[0]);
            foreach (int item in MailItemID)
                CB_MailType.Items.Add(ItemList[item]);

            LoadPKM(true);
            entry = -1;
            editing = false;
            LB_PartyHeld.SelectedIndex = 0;
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
            if (entry < 6) MakePartyList();
            else MakePCList();
        }

        private void MakePartyList()
        {
            LB_PartyHeld.BeginUpdate();
            int s = LB_PartyHeld.SelectedIndex;
            LB_PartyHeld.Items.Clear();
            for (int i = 0, j = Gen >= 4 ? p.Count : 6; i < j; i++)
                LB_PartyHeld.Items.Add($"{i}: {m[i].AuthorName}");
            if (s != LB_PartyHeld.SelectedIndex && s < LB_PartyHeld.Items.Count)
                LB_PartyHeld.SelectedIndex = s;
            LB_PartyHeld.EndUpdate();
        }

        private void MakePCList()
        {
            LB_PCBOX.BeginUpdate();
            int s = LB_PCBOX.SelectedIndex;
            LB_PCBOX.Items.Clear();
            switch (Gen)
            {
                case 2:
                    for (int i = 6, j = 0, boxsize = (int)NUD_BoxSize.Value; i < m.Length; i++, j++)
                    {
                        LB_PCBOX.Items.Add(j < boxsize ? $"{i}: {m[i].AuthorName}" : "x");
                    }
                    break;
                case 3:
                    for (int i = 6; i < m.Length; i++)
                    {
                        LB_PCBOX.Items.Add(m[i].IsEmpty != true ? $"{i}: {m[i].AuthorName}" : "x");
                    }
                    break;
                case 4:
                case 5:
                    for (int i = p.Count; i < m.Length; i++)
                    {
                        LB_PCBOX.Items.Add(m[i].IsEmpty != true ? $"{i}: {m[i].AuthorName}" : "x");
                    }
                    break;
            }
            if (s != LB_PCBOX.SelectedIndex && s < LB_PCBOX.Items.Count)
                LB_PCBOX.SelectedIndex = s;
            LB_PCBOX.EndUpdate();
        }

        private void LoadPKM(bool isInit)
        {
            for (int i = 0; i < p.Count; i++)
            {
                if (isInit)
                    PKMLabels[i].Text = GetSpeciesNameFromCB(p[i].Species);
                var j = MailTypeToCBIndex(p[i].HeldItem);
                PKMHeldItems[i].Text = j <= 0 ? "(not Mail)" : CB_MailType.Items[j].ToString();
                PKMHeldItems[i].TextAlign = j <= 0 ? ContentAlignment.TopLeft : ContentAlignment.TopRight;
                if (Gen != 3)
                    continue;
                int k = ((PK3)p[i]).HeldMailID;
                PKMNUDs[i].Value = k >= -1 && k <= 5 ? k : -1;
            }
        }

        private readonly Mail[] m;
        private bool editing;
        private int entry;
        private readonly NumericUpDown[][] Messages;
        private readonly NumericUpDown[] PKMNUDs, Miscs;
        private readonly Label[] PKMLabels, PKMHeldItems;
        private readonly ComboBox[] AppearPKMs;
        private readonly int Gen;
        private readonly byte ResetVer, ResetLang;

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
                    for (int i = 0; i < p.Count; i++)
                        ((PK3) p[i]).HeldMailID = (sbyte)PKMNUDs[i].Value;
                    break;
                case 4:
                    for (int i = 0; i < p.Count; i++)
                        m[i].CopyTo((PK4) p[i]);
                    for (int i = p.Count; i < m.Length; i++)
                        m[i].CopyTo(SAV);
                    break;
                case 5:
                    for (int i = 0; i < p.Count; i++)
                        m[i].CopyTo((PK5) p[i]);
                    for (int i = p.Count; i < m.Length; i++)
                        m[i].CopyTo(SAV);
                    break;
            }
            SAV.PartyData = p;
        }

        private void TempSave()
        {
            switch (m[entry])
            {
                case Mail2 m2:
                    m2.AuthorName = TB_AuthorName.Text;
                    m2.AuthorTID = (ushort)NUD_AuthorTID.Value;
                    m2.MailType = CBIndexToMailType(CB_MailType.SelectedIndex);

                    m2.SetMessage(TB_MessageBody21.Text, TB_MessageBody22.Text);
                    m2.AppearPKM = CB_AppearPKM1.SelectedValue as int? ?? 0;
                    break;
                case Mail3 m3:
                    m3.AuthorName = TB_AuthorName.Text;
                    m3.AuthorTID = (ushort)NUD_AuthorTID.Value;
                    m3.MailType = CBIndexToMailType(CB_MailType.SelectedIndex);

                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                            m3.SetMessage(y, x, (ushort)Messages[y][x].Value);
                    }

                    m3.AuthorSID = (ushort)NUD_AuthorSID.Value;
                    int v = CB_AppearPKM1.SelectedValue as int? ?? 0;
                    m3.AppearPKM = v < 252 ? v : HoennListMixed[v - 252];
                    break;
                case Mail4 m4:
                    m4.AuthorName = TB_AuthorName.Text;
                    m4.AuthorTID = (ushort)NUD_AuthorTID.Value;
                    v = CB_MailType.SelectedIndex;
                    m4.MailType = v > 0 ? v - 1 : 0xFF;

                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 4; x++)
                            m4.SetMessage(y, x, (ushort)Messages[y][x].Value);
                    }

                    m4.AuthorSID = (ushort)NUD_AuthorSID.Value;
                    for (int i = 0; i < AppearPKMs.Length; i++)
                        m4.SetAppearPKM(i, (AppearPKMs[i].SelectedValue as int?) + 7 ?? 0);
                    if (CB_AuthorVersion.SelectedValue != null)
                        m4.AuthorVersion = (byte)((int?)CB_AuthorVersion.SelectedValue ?? 0);
                    if (CB_AuthorLang.SelectedValue != null)
                        m4.AuthorLanguage = (byte)((int?)CB_AuthorLang.SelectedValue ?? 0);
                    break;
                case Mail5 m5:
                    m5.AuthorName = TB_AuthorName.Text;
                    m5.AuthorTID = (ushort)NUD_AuthorTID.Value;
                    v = CB_MailType.SelectedIndex;
                    m5.MailType = v > 0 ? v - 1 : 0xFF;

                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 4; x++)
                            m5.SetMessage(y, x, (ushort)Messages[y][x].Value);
                    }

                    m5.AuthorSID = (ushort)NUD_AuthorSID.Value;
                    for (int i = 0; i < Miscs.Length; i++)
                        m5.SetMisc(i, (ushort)Miscs[i].Value);
                    m5.MessageEnding = (ushort)NUD_MessageEnding.Value;
                    m5.AuthorVersion = (byte)((int?)CB_AuthorVersion.SelectedValue ?? 0);
                    m5.AuthorLanguage = (byte)((int?)CB_AuthorLang.SelectedValue ?? 0);
                    break;
            }
        }

        private string CheckValid()
        {
            string ret = "";
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
                    int h = ((PK3) p[i]).HeldMailID;
                    heldMailIDs[i] = h;
                    if (ItemIsMail(p[i].HeldItem))
                    {
                        if (h < 0 || h > 5) //A
                            ret += $"{Environment.NewLine}Party#{i + 1} MailID mismatch";
                        else if (m[h].IsEmpty == true) //B
                            ret += $"{Environment.NewLine}Party#{i + 1} MailID mismatch";
                    }
                    else if (h != -1) //C
                    {
                        ret += $"{Environment.NewLine}Party#{i + 1} MailID mismatch";
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (heldMailIDs.Count(v => v == i) > 1) //D
                        ret += $"{Environment.NewLine}MailID{i} duplicated";
                    if (m[i].IsEmpty == false && heldMailIDs.All(v => v != i)) //E
                        ret += $"{Environment.NewLine}MailID{i} not referred";
                }
            }
            // Gen2, Gen4
            // P: held item is mail, but mail is empty(invalid mail type. g2:not 181 to 189, g4:12 to 255). it should be not empty or held not mail.
            // Q: held item is not mail, but mail is not empty. it should be empty or held mail.
            else if (Gen == 2 || Gen == 4)
            {
                for (int i = 0; i < p.Count; i++)
                {
                    if (ItemIsMail(p[i].HeldItem))
                    {
                        if (m[i].IsEmpty == true) //P
                            ret += $"{Environment.NewLine}MailID{i} MailType mismatch";
                    }
                    else if (m[i].IsEmpty == false) //Q
                    {
                        ret += $"{Environment.NewLine}MailID{i} MailType mismatch";
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
                            ret += $"{Environment.NewLine}MailID{i} MailType mismatch";
                    }
                }
            }
            // Gen*
            // Z: mail type is illegal
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i].IsEmpty == null) // Z
                    ret += $"{Environment.NewLine}MailID{i} MailType mismatch";
            }

            return ret;
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            if (entry >= 0) TempSave();
            Save();
            string Err = CheckValid();
            if (Err.Length != 0 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Validation Error. Save?{Environment.NewLine}{Err}"))
                return;
            Origin.SetData(SAV.Data, 0);
            Close();
        }

        private bool ItemIsMail(int itemID) => Array.IndexOf(MailItemID, itemID) >= 0;
        private int MailTypeToCBIndex(int mailtype) => 1 + Array.IndexOf(MailItemID, mailtype);
        private int CBIndexToMailType(int cbindex) => cbindex <= 0 ? 0 : cbindex <= MailItemID.Length ? MailItemID[cbindex - 1] : MailItemID[0];
        private readonly int[] MailItemID;
        private readonly IList<PKM> p;

        private string GetSpeciesNameFromCB(int index)
        {
            foreach (ComboItem i in CB_AppearPKM1.Items)
                if (index == i.Value) return i.Text;
            return "PKM";
        }

        private DialogResult ModifyHeldItem()
        {
            DialogResult ret = DialogResult.Abort;
            PKM s = p[entry];
            if (!ItemIsMail(s.HeldItem)) return ret;
            System.Media.SystemSounds.Question.Play();
            ret = MessageBox.Show($"Modify {PKMLabels[entry].Text}'s HeldItem?{Environment.NewLine}{Environment.NewLine} {PKMHeldItems[entry].Text} -> {CB_MailType.Items[0]}{Environment.NewLine}{Environment.NewLine}Yes: Delete Mail & Modify PKM{Environment.NewLine}No: Delete Mail", "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button3);
            if (ret != DialogResult.Yes) return ret;
            s.HeldItem = 0;
            if (Gen == 3) ((PK3) s).HeldMailID = -1;
            LoadPKM(false);
            return ret;
        }

        private void B_Delete_Click(object sender, EventArgs e)
        {
            if (entry < 0) return;
            if (entry < p.Count)
            {
                 DialogResult ret= ModifyHeldItem();
                if (ret == DialogResult.Cancel) return;
            }
            switch (m[entry])
            {
                case Mail4 m4: m4.SetBlank(ResetLang, ResetVer); break;
                case Mail5 m5: m5.SetBlank(ResetLang, ResetVer); break;
                default: m[entry].SetBlank(); break;
            }
            editing = true;
            LoadList();
            LoadMail();
            editing = false;
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
                LoadList();
            }
            if (sender == LB_PartyHeld && pcIndex >= 0)
            {
                pcIndex = -1;
                LB_PCBOX.SelectedIndex = -1;
            }
            else if (sender == LB_PCBOX && partyIndex >= 0)
            {
                partyIndex = -1;
                LB_PartyHeld.SelectedIndex = -1;
            }
            if (partyIndex >= 0)
                entry = partyIndex;
            else if (pcIndex >= 0)
                entry = (Gen >= 4 ? p.Count : 6) + pcIndex;
            else entry = -1;
            if (entry >= 0) LoadMail();
            editing = false;
        }

        private void LoadMail()
        {
            switch (m[entry])
            {
                case Mail2 m2:
                    TB_AuthorName.Text = m2.AuthorName;
                    NUD_AuthorTID.Value = m2.AuthorTID;
                    CB_MailType.SelectedIndex = MailTypeToCBIndex(m2.MailType);

                    TB_MessageBody21.Text = m2.GetMessage(false);
                    TB_MessageBody22.Text = m2.GetMessage(true);
                    AppearPKMs[0].SelectedValue = m2.AppearPKM;
                    break;
                case Mail3 m3:
                    TB_AuthorName.Text = m3.AuthorName;
                    NUD_AuthorTID.Value = m3.AuthorTID;
                    CB_MailType.SelectedIndex = MailTypeToCBIndex(m3.MailType);

                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                            Messages[y][x].Value = m3.GetMessage(y, x);
                    }

                    NUD_AuthorSID.Value = m3.AuthorSID;
                    int v = Array.IndexOf(HoennListMixed, m3.AppearPKM);
                    AppearPKMs[0].SelectedValue = m3.AppearPKM < 252 ? m3.AppearPKM : v >= 0 ? 252 + v : 0;
                    break;
                case Mail4 m4:
                    TB_AuthorName.Text = m4.AuthorName;
                    NUD_AuthorTID.Value = m4.AuthorTID;
                    CB_MailType.SelectedIndex = m4.IsEmpty == false ? m4.MailType + 1 : 0;

                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 4; x++)
                            Messages[y][x].Value = m4.GetMessage(y, x);
                    }

                    NUD_AuthorSID.Value = m4.AuthorSID;
                    for (int i = 0; i < AppearPKMs.Length; i++)
                        AppearPKMs[i].SelectedValue = Math.Max(0, m4.GetAppearPKM(i) - 7);
                    LoadOTlabel(m4.AuthorGender);
                    CB_AuthorVersion.SelectedValue = (int)m4.AuthorVersion;
                    CB_AuthorLang.SelectedValue = (int)m4.AuthorLanguage;
                    break;
                case Mail5 m5:
                    TB_AuthorName.Text = m5.AuthorName;
                    NUD_AuthorTID.Value = m5.AuthorTID;
                    CB_MailType.SelectedIndex = m5.IsEmpty == false ? m5.MailType + 1 : 0;

                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 4; x++)
                            Messages[y][x].Value = m5.GetMessage(y, x);
                    }

                    NUD_AuthorSID.Value = m5.AuthorSID;
                    for (int i = 0; i < Miscs.Length; i++)
                        Miscs[i].Value = m5.GetMisc(i);
                    NUD_MessageEnding.Value = m5.MessageEnding;
                    LoadOTlabel(m5.AuthorGender);
                    CB_AuthorVersion.SelectedValue = (int)m5.AuthorVersion;
                    CB_AuthorLang.SelectedValue = (int)m5.AuthorLanguage;
                    break;
            }
        }

        private readonly string[] gendersymbols = { "♂", "♀" };

        private void LoadOTlabel(int b)
        {
            Label_OTGender.Text = gendersymbols[b & 1];
            Label_OTGender.ForeColor = b == 1 ? Color.Red : Color.Blue;
        }

        private void Label_OTGender_Click(object sender, EventArgs e)
        {
            if (entry < 0) return;
            if (Gen < 4) return;
            var mail = m[entry];
            var b = mail.AuthorGender;
            b ^= 1;
            mail.AuthorGender = b;
            LoadOTlabel(b);
        }

        private void NUD_BoxSize_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            editing = true;
            MakePCList();
            editing = false;
        }
    }
}
