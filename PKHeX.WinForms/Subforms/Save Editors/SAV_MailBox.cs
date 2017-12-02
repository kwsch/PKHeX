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
            SAV = (Origin = sav).Clone();
            Gen = SAV.Generation;
            p = SAV.PartyData;
            editing = true;
            InitializeComponent();

            Messages = new NumericUpDown[][] {
                new[] { NUD_Message00, NUD_Message01, NUD_Message02, NUD_Message03 },
                new[] { NUD_Message10, NUD_Message11, NUD_Message12, NUD_Message13 },
                new[] { NUD_Message20, NUD_Message21, NUD_Message22, NUD_Message23 }
            };
            PKMLabels = new[] { L_PKM1, L_PKM2, L_PKM3, L_PKM4, L_PKM5, L_PKM6 };
            PKMHeldItems = new[] { L_HeldItem1, L_HeldItem2, L_HeldItem3, L_HeldItem4, L_HeldItem5, L_HeldItem6 };
            PKMNUDs = new[] { NUD_MailID1, NUD_MailID2, NUD_MailID3, NUD_MailID4, NUD_MailID5, NUD_MailID6 };
            AppearPKMs = new[] { CB_AppearPKM1, CB_AppearPKM2, CB_AppearPKM3 };

            NUD_BoxSize.Visible = L_BoxSize.Visible = Gen == 2;
            GB_MessageTB.Visible = Gen == 2;
            GB_MessageNUD.Visible = Gen != 2;
            Messages[0][3].Visible = Messages[1][3].Visible = Messages[2][3].Visible = Gen == 4;
            NUD_AuthorSID.Visible = Gen != 2;
            Label_OTGender.Visible = CB_AuthorLang.Visible = CB_AuthorVersion.Visible = AppearPKMs[1].Visible = AppearPKMs[2].Visible = Gen == 4;
            for (int i = p.Count; i < 6; i++)
                PKMNUDs[i].Visible = PKMLabels[i].Visible = PKMHeldItems[i].Visible = false;
            if (Gen == 2 || Gen == 4)
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
                    MakePartyList();
                    MakePCList();
                    MailItemID = Enumerable.Range(0xB5, 9).ToArray();
                    break;
                case SAV3 sav3:
                    m = new Mail3[6 + 10];
                    for (int i = 0; i < m.Length; i++)
                        m[i] = new Mail3(sav3, i);

                    MakePartyList();
                    MakePCList();
                    MailItemID = Enumerable.Range(0x79, 12).ToArray();
                    break;
                case SAV4 sav4:
                    m = new Mail4[p.Count + 20];
                    for (int i = 0; i < p.Count; i++)
                        m[i] = new Mail4((p[i] as PK4).HeldMailData);
                    for (int i = p.Count, j = 0; i < m.Length; i++, j++)
                        m[i] = new Mail4(sav4, j);

                    MakePartyList();
                    MakePCList();
                    MailItemID = Enumerable.Range(0x89, 12).ToArray();
                    break;
            }

            if (Gen == 2 || Gen == 3)
            {
                CB_AppearPKM1.Items.Clear();
                CB_AppearPKM1.DisplayMember = "Text";
                CB_AppearPKM1.ValueMember = "Value";
                CB_AppearPKM1.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(id => id.Value <= sav.MaxSpeciesID).ToList(), null);
            }
            else if (Gen == 4)
            {
                var species = GameInfo.SpeciesDataSource.Where(id => id.Value <= sav.MaxSpeciesID).ToList();
                foreach(ComboBox a in AppearPKMs)
                {
                    a.Items.Clear();
                    a.DisplayMember = "Text";
                    a.ValueMember = "Value";
                    a.DataSource = new BindingSource(species, null);
                }

                CB_AuthorVersion.Items.Clear();
                CB_AuthorVersion.DisplayMember = "Text";
                CB_AuthorVersion.ValueMember = "Value";
                CB_AuthorVersion.DataSource = new BindingSource(new ComboItem[] {
                    new ComboItem { Text = "Diamond", Value = (int)GameVersion.D },
                    new ComboItem { Text = "Pearl", Value = (int)GameVersion.P },
                    new ComboItem { Text = "Platinum", Value = (int)GameVersion.Pt },
                    new ComboItem { Text = "HeartGold", Value = (int)GameVersion.HG },
                    new ComboItem { Text = "SoulSilver", Value = (int)GameVersion.P },
                }.ToList(), null);

                CB_AuthorLang.Items.Clear();
                CB_AuthorLang.DisplayMember = "Text";
                CB_AuthorLang.ValueMember = "Value";
                CB_AuthorLang.DataSource = new BindingSource(new ComboItem[] {
                    // not sure
                    new ComboItem { Text = "JP", Value = 1 },
                    new ComboItem { Text = "US", Value = 2 },
                    new ComboItem { Text = "KOR", Value = 3 },
                }.ToList(), null);
            }

            string[] ItemList = GameInfo.Strings.GetItemStrings(Gen, SAV.Version);
            CB_MailType.Items.Clear();
            CB_MailType.Items.Add(ItemList[0]);
            for (int i = 0; i < MailItemID.Length; i++)
                CB_MailType.Items.Add(ItemList[MailItemID[i]]);

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
            for (int i = 0, j = Gen == 4 ? p.Count : 6; i < j; i++)
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
                    for (int i = 6, j = 0, boxsize = (int)NUD_BoxSize.Value; i < m.Length; i++, j++) {
                        if (j < boxsize) LB_PCBOX.Items.Add($"{i}: {m[i].AuthorName}");
                        else LB_PCBOX.Items.Add("x");
                    }
                    break;
                case 3:
                    for (int i = 6; i < m.Length; i++) {
                        if (m[i].MailType != 0) LB_PCBOX.Items.Add($"{i}: {m[i].AuthorName}");
                        else LB_PCBOX.Items.Add("x");
                    }
                    break;
                case 4:
                    for (int i = p.Count; i < m.Length; i++) {
                        if (m[i].MailType <= 11) LB_PCBOX.Items.Add($"{i}: {m[i].AuthorName}");
                        else LB_PCBOX.Items.Add("x");
                    }
                    break;
            }
            if (s != LB_PCBOX.SelectedIndex && s < LB_PCBOX.Items.Count)
                LB_PCBOX.SelectedIndex = s;
            LB_PCBOX.EndUpdate();
        }
        private void LoadPKM(bool isInit)
        {
            for (int i = 0, j, k; i < p.Count; i++)
            {
                if (isInit)
                    PKMLabels[i].Text = GetSpeciesNameFromCB(p[i].Species);
                j = MailTypeToCBIndex(p[i].HeldItem);
                PKMHeldItems[i].Text = j <= 0 ? "(not Mail)" : CB_MailType.Items[j].ToString();
                PKMHeldItems[i].TextAlign = j <= 0 ? ContentAlignment.TopLeft : ContentAlignment.TopRight;
                if (Gen == 3)
                {
                    k = (p[i] as PK3).HeldMailID;
                    PKMNUDs[i].Value = k >= -1 && k <= 5 ? k : -1;
                }
            }
        }
        private readonly Mail[] m;
        private bool editing;
        private int entry;
        private readonly NumericUpDown[][] Messages;
        private readonly NumericUpDown[] PKMNUDs;
        private readonly Label[] PKMLabels, PKMHeldItems;
        private readonly ComboBox[] AppearPKMs;
        private readonly int Gen;
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
                    len = 0x2F * 10 + 1;
                    Array.Copy(SAV.Data, ofs, SAV.Data, ofs + len, len);
                    break;
                case 3:
                    foreach (var n in m) n.CopyTo(SAV);
                    for (int i = 0; i < p.Count; i++)
                        (p[i] as PK3).HeldMailID = (sbyte)PKMNUDs[i].Value;
                    break;
                case 4:
                    for (int i = 0; i < p.Count; i++)
                        m[i].CopyTo(p[i] as PK4);
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

                    for (int y = 0; y < 2; y++)
                        for (int x = 0; x < 2; x++)
                            m3.SetMessage(y, x, (ushort)Messages[y][x].Value);
                    m3.AuthorSID = (ushort)NUD_AuthorSID.Value;
                    int v = CB_AppearPKM1.SelectedValue as int? ?? 0;
                    m3.AppearPKM = v < 252 ? v : HoennListMixed[v - 252];
                    break;
                case Mail4 m4:
                    m4.AuthorName = TB_AuthorName.Text;
                    m4.AuthorTID = (ushort)NUD_AuthorTID.Value;
                    v = CB_MailType.SelectedIndex;
                    m4.MailType = v > 0 ? v - 1 : 0xFF;

                    for (int y = 0; y < 2; y++)
                        for (int x = 0; x < 3; x++)
                            m4.SetMessage(y, x, (ushort)Messages[y][x].Value);
                    m4.AuthorSID = (ushort)NUD_AuthorSID.Value;
                    for (int i = 0; i < AppearPKMs.Length; i++)
                        m4.SetAppearPKM(i, (AppearPKMs[i].SelectedValue as int?) + 7 ?? 0);
                    m4.AuthorVersion = CB_AuthorVersion.SelectedValue as byte? ?? 0;
                    m4.AuthorLanguage = CB_AuthorLang.SelectedValue as byte? ?? 0;
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
                for (int i = 0, h; i < p.Count; i++)
                {
                    h = (p[i] as PK3).HeldMailID;
                    heldMailIDs[i] = h;
                    if (ItemIsMail(p[i].HeldItem))
                    {
                        if (h < 0 || h > 5) //A
                            ret += $"{Environment.NewLine}Party#{i + 1} MailID mismatch";
                        else if (m[h].MailType == 0) //B
                            ret += $"{Environment.NewLine}Party#{i + 1} MailID mismatch";
                    }
                    else if (h != -1) //C
                        ret += $"{Environment.NewLine}Party#{i + 1} MailID mismatch";
                }
                for(int i = 0; i < 6; i++)
                {
                    if (heldMailIDs.Count(v => v == i) > 1) //D
                        ret += $"{Environment.NewLine}MailID{i} duplicated";
                    if (m[i].MailType != 0 && heldMailIDs.All(v => v != i)) //E
                        ret += $"{Environment.NewLine}MailID{i} not referred";
                }
            }
            // Gen2, Gen4
            // P: held item is mail, but mail is empty(invalid mail type. g2:not 181 to 189, g4:12 to 255). it should be not empty or held not mail.
            // Q: held item is not mail, but mail is not empty. it should be empty or held mail.
            else if (Gen == 2)
            {
                for(int i = 0; i < p.Count; i++)
                {
                    if (ItemIsMail(p[i].HeldItem))
                    {
                        if (!ItemIsMail(m[i].MailType)) //P
                            ret += $"{Environment.NewLine}MailID{i} MailType mismatch";
                    }
                    else if (m[i].MailType != 0) //Q
                        ret += $"{Environment.NewLine}MailID{i} MailType mismatch";
                }
            }
            else if (Gen == 4)
            {
                for(int i = 0; i < p.Count; i++)
                {
                    if (ItemIsMail(p[i].HeldItem))
                    {
                        if (m[i].MailType >= 12) //P
                            ret += $"{Environment.NewLine}MailID{i} MailType mismatch";
                    }
                    else if (m[i].MailType != 0xFF) //Q
                        ret += $"{Environment.NewLine}MailID{i} MailType mismatch";
                }
            }
            return ret;
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            if (entry >= 0) TempSave();
            Save();
            string Err = CheckValid();
            if (Err.Length == 0 || DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Validation Error. Save?{Environment.NewLine}{Err}"))
            {
                Origin.SetData(SAV.Data, 0);
                Close();
            }
        }
        private bool ItemIsMail(int itemID) => Array.IndexOf(MailItemID, itemID) >= 0;
        private int MailTypeToCBIndex(int mailtype) => 1 + Array.IndexOf(MailItemID, mailtype);
        private int CBIndexToMailType(int cbindex) => cbindex <= 0 ? 0 : cbindex <= MailItemID.Length ? MailItemID[cbindex - 1] : MailItemID[0];
        private int[] MailItemID;
        private IList<PKM> p;
        private string GetSpeciesNameFromCB(int index)
        {
            foreach (ComboItem i in CB_AppearPKM1.Items)
                if (index == i.Value) return i.Text;
            return "PKM";
        }
        private DialogResult ModifyHeldItem()
        {
            DialogResult ret = DialogResult.Abort;
            if (entry >= p.Count) return ret;
            PKM s = p[entry];
            if (!ItemIsMail(s.HeldItem)) return ret;
            System.Media.SystemSounds.Question.Play();
            ret = MessageBox.Show($"Modify {PKMLabels[entry].Text}'s HeldItem?{Environment.NewLine}{Environment.NewLine} {PKMHeldItems[entry].Text} -> {CB_MailType.Items[0].ToString()}{Environment.NewLine}{Environment.NewLine}Yes: Delete Mail & Modify PKM{Environment.NewLine}No: Delete Mail", "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button3);
            if (ret != DialogResult.Yes) return ret;
            s.HeldItem = 0;
            if (Gen == 3) (s as PK3).HeldMailID = -1;
            LoadPKM(false);
            return ret;
        }
        private void B_Delete_Click(object sender, EventArgs e)
        {
            if (entry < 0) return;
            if (entry < 6)
            {
                 DialogResult ret= ModifyHeldItem();
                if (ret == DialogResult.Cancel) return;
            }
            editing = true;
            switch (Gen)
            {
                case 2:
                    TB_MessageBody21.Text = TB_MessageBody22.Text = "";
                    break;
                case 3:
                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 3; x++)
                            Messages[y][x].Value = 0xFFFF;
                    NUD_AuthorSID.Value = 0;
                    break;
                case 4:
                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 4; x++)
                            Messages[y][x].Value = x == 1 ? 0 : 0xFFFF;
                    NUD_AuthorSID.Value = 0;
                    CB_AppearPKM2.SelectedValue = CB_AppearPKM3.SelectedValue = 0;
                    // which value to reset? (version value is not detected)
                    // CB_AuthorLang.SelectedIndex = 0;
                    // CB_AuthorVersion.SelectedValue = 0;
                    break;
            }
            TB_AuthorName.Text = "";
            NUD_AuthorTID.Value = 0;
            CB_AppearPKM1.SelectedValue = Gen == 3 ? 1 : 0;
            CB_MailType.SelectedIndex = 0;
            TempSave();
            LoadList();
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
                entry = 6 + pcIndex;
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
                        for (int x = 0; x < 3; x++)
                            Messages[y][x].Value = m3.GetMessage(y, x);
                    NUD_AuthorSID.Value = m3.AuthorSID;
                    int v = Array.IndexOf(HoennListMixed, m3.AppearPKM);
                    AppearPKMs[0].SelectedValue = m3.AppearPKM < 252 ? m3.AppearPKM : v >= 0 ? 252 + v : 0;
                    break;
                case Mail4 m4:
                    TB_AuthorName.Text = m4.AuthorName;
                    NUD_AuthorTID.Value = m4.AuthorTID;
                    v = m4.MailType;
                    CB_MailType.SelectedIndex = v <= 11 ? v + 1 : 0;

                    for (int y = 0; y < 3; y++)
                        for (int x = 0; x < 4; x++)
                            Messages[y][x].Value = m4.GetMessage(y, x);
                    NUD_AuthorSID.Value = m4.AuthorSID;
                    for (int i = 0; i < AppearPKMs.Length; i++)
                        AppearPKMs[i].SelectedValue = Math.Max(0, m4.GetAppearPKM(i) - 7);
                    LoadOTlabel(m4.AuthorGender);
                    CB_AuthorVersion.SelectedValue = m4.AuthorVersion;
                    CB_AuthorLang.SelectedValue = m4.AuthorLanguage;
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
            if (Gen != 4) return;
            var mail = m[entry] as Mail4;
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
    public class Mail2 : Mail
    {
        private bool US;
        public Mail2(SAV2 sav, int index)
        {
            US = !sav.Japanese && !sav.Korean;
            DataOffset = index < 6 ? index * 0x2F + 0x600 : (index - 6) * 0x2F + 0x835;
            Data = sav.GetData(DataOffset, 0x2F);
        }
        public override string GetMessage(bool isLastLine) => US ? StringConverter.GetString1(Data, isLastLine ? 0x11 : 0, 0x10, false) : string.Empty;
        public override void SetMessage(string line1, string line2)
        {
            if (US)
            {
                StringConverter.SetString1(line2, 0x10, false, 0x10, 0x50).CopyTo(Data, 0x11);
                StringConverter.SetString1(line1, 0x10, false, 0x10, (ushort)(Data.Skip(0x11).Take(0x10).All(v => v == 0x50) ? 0x50 : 0x7F)).CopyTo(Data, 0);
                Data[0x10] = 0x4E;
            }
        }
        public override string AuthorName
        {
            get => US ? StringConverter.GetString1(Data, 0x21, 7, false) : string.Empty;
            set
            {
                if (US)
                {
                    StringConverter.SetString1(value, 7, false, 8, 0x50).CopyTo(Data, 0x21);
                    Data[0x29] = Data[0x2A] = 0;
                }
            }
        }
        public override ushort AuthorTID
        {
            get => (ushort)(Data[0x2B] << 8 | Data[0x2C]);
            set
            {
                Data[0x2B] = (byte)(value >> 8);
                Data[0x2C] = (byte)(value & 0xFF);
            }
        }
        public override int AppearPKM { get => Data[0x2D]; set => Data[0x2D] = (byte)value; }
        public override int MailType { get => Data[0x2E]; set => Data[0x2E] = (byte)value; }
    }
    public class Mail3 : Mail
    {
        public Mail3(SAV3 sav, int index)
        {
            DataOffset = index * 0x24 + sav.GetBlockOffset(3) + 0xCE0;
            Data = sav.GetData(DataOffset, 0x24);
        }
        public override ushort GetMessage(int index1, int index2) => BitConverter.ToUInt16(Data, (index1 * 3 + index2) * 2);
        public override void SetMessage(int index1, int index2, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, (index1 * 3 + index2) * 2);
        public override string AuthorName
        {
            get => StringConverter.GetString3(Data, 0x12, 7, false);
            set
            {
                Data[0x18] = Data[0x19] = 0xFF;
                StringConverter.SetString3(value, 7, false, 6, 0).CopyTo(Data, 0x12);
            }
        }
        public override ushort AuthorTID { get => BitConverter.ToUInt16(Data, 0x1A); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1A); }
        public ushort AuthorSID { get => BitConverter.ToUInt16(Data, 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1C); }
        public override int AppearPKM { get => BitConverter.ToUInt16(Data, 0x1E); set => BitConverter.GetBytes((ushort)(value == 0 ? 1 : value)).CopyTo(Data, 0x1E); }
        public override int MailType { get => BitConverter.ToUInt16(Data, 0x20); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x20); }
    }
    public class Mail4 : Mail
    {
        public Mail4(SAV4 sav, int index)
        {
            switch (sav.Version)
            {
                case GameVersion.DP: DataOffset = index * 0x38 + 0x4BEC + sav.GetGBO; break;
                case GameVersion.Pt: DataOffset = index * 0x38 + 0x4E80 + sav.GetGBO; break;
                case GameVersion.HGSS: DataOffset = index * 0x38 + 0x3FA8 + sav.GetGBO; break;
            }
            Data = sav.GetData(DataOffset, 0x38);
        }
        public Mail4(byte[] data) => Data = data;
        public override void CopyTo(PK4 pk4) => pk4.HeldMailData = Data;
        public override ushort AuthorTID { get => BitConverter.ToUInt16(Data, 0); set => BitConverter.GetBytes(value).CopyTo(Data, 0); }
        public ushort AuthorSID { get => BitConverter.ToUInt16(Data, 2); set => BitConverter.GetBytes(value).CopyTo(Data, 2); }
        public byte AuthorGender { get => Data[4]; set => Data[4] = value; }
        public byte AuthorLanguage { get => Data[5]; set => Data[5] = value; }
        public byte AuthorVersion { get => Data[6]; set => Data[6] = value; }
        public override int MailType { get => Data[7]; set => Data[7] = (byte)value; }
        public override string AuthorName { get => StringConverter.GetString4(Data, 8, 0x10); set => StringConverter.SetString4(value, 7, 8, 0xFFFF).CopyTo(Data, 8); }
        public int GetAppearPKM(int index) => BitConverter.ToUInt16(Data, 0x1C - index * 2);
        public void SetAppearPKM(int index, int value) => BitConverter.GetBytes((ushort)(value == 0 ? 0xFFFF : value)).CopyTo(Data, 0x1C - index * 2);
        public override ushort GetMessage(int index1, int index2) => BitConverter.ToUInt16(Data, 0x20 + (index1 * 4 + index2) * 2);
        public override void SetMessage(int index1, int index2, ushort value) => BitConverter.GetBytes(value).CopyTo(Data, 0x20 + (index1 * 4 + index2) * 2);
    }
    public class Mail
    {
        protected byte[] Data;
        protected int DataOffset;
        public void CopyTo(SaveFile sav) => sav.SetData(Data, DataOffset);
        public virtual void CopyTo(PK4 pk4) { }
        public virtual string GetMessage(bool isLastLine) => null;
        public virtual ushort GetMessage(int index1, int index2) => 0;
        public virtual void SetMessage(string line1, string line2) { }
        public virtual void SetMessage(int index1, int index2, ushort value) { }
        public virtual string AuthorName { get; set; }
        public virtual ushort AuthorTID { get; set; }
        public virtual int AppearPKM { get; set; }
        public virtual int MailType { get; set; }
    }
}
