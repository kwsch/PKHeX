using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_MailBox : Form
{
    private readonly SaveFile Origin;
    private readonly SaveFile SAV;

    private readonly MailDetail[] m = null!;
    private bool editing;
    private int entry;
    private readonly NumericUpDown[][] Messages;
    private readonly NumericUpDown[] PKMNUDs, Miscs;
    private readonly Label[] PKMLabels, PKMHeldItems;
    private readonly ComboBox[] AppearPKMs;
    private readonly byte Generation;
    private readonly byte ResetVer, ResetLang;
    private readonly int PartyBoxCount;
    private string loadedLBItemLabel = null!;
    private bool LabelValue_GenderF;
    private readonly int[] MailItemID = null!;
    private readonly IList<PKM> p;

    public SAV_MailBox(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (Origin = sav).Clone();
        Generation = SAV.Generation;
        p = SAV.PartyData;
        editing = true;

        Messages =
        [
            [NUD_Message00, NUD_Message01, NUD_Message02, NUD_Message03],
            [NUD_Message10, NUD_Message11, NUD_Message12, NUD_Message13],
            [NUD_Message20, NUD_Message21, NUD_Message22, NUD_Message23],
        ];
        PKMLabels = [L_PKM1, L_PKM2, L_PKM3, L_PKM4, L_PKM5, L_PKM6];
        PKMHeldItems = [L_HeldItem1, L_HeldItem2, L_HeldItem3, L_HeldItem4, L_HeldItem5, L_HeldItem6];
        PKMNUDs = [NUD_MailID1, NUD_MailID2, NUD_MailID3, NUD_MailID4, NUD_MailID5, NUD_MailID6];
        AppearPKMs = [CB_AppearPKM1, CB_AppearPKM2, CB_AppearPKM3];
        Miscs = [NUD_Misc1, NUD_Misc2, NUD_Misc3];

        NUD_BoxSize.Visible = L_BoxSize.Visible = CHK_UserEntered.Visible = Generation == 2;
        GB_MessageTB.Visible = Generation == 2;
        GB_MessageNUD.Visible = Generation != 2;
        Messages[0][3].Visible = Messages[1][3].Visible = Messages[2][3].Visible = Generation is 4 or 5;
        NUD_AuthorSID.Visible = Generation != 2;
        Label_OTGender.Visible = CB_AuthorVersion.Visible = Generation is 4 or 5;
        CB_AuthorLang.Visible = Generation is 2 or 4 or 5;
        L_AppearPKM.Visible = AppearPKMs[0].Visible = Generation != 5;
        AppearPKMs[1].Visible = AppearPKMs[2].Visible = Generation == 4;
        NUD_MessageEnding.Visible = Generation == 5;
        L_MiscValue.Visible = NUD_Misc1.Visible = NUD_Misc2.Visible = NUD_Misc3.Visible = Generation == 5;
        GB_PKM.Visible = B_PartyUp.Enabled = B_PartyDown.Enabled = SAV is not SAV2Stadium;

        for (int i = p.Count; i < 6; i++)
            PKMNUDs[i].Visible = PKMLabels[i].Visible = PKMHeldItems[i].Visible = false;
        if (Generation != 3)
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

                NUD_BoxSize.Value = SAV.Data[Mail2.GetMailboxOffset(SAV.Language)];
                MailItemID = [0x9E, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD];
                PartyBoxCount = 6;
                NUD_BoxSize.Maximum = 10;
                break;
            case SAV2Stadium sav2Stadium:
                m = new Mail2[SAV2Stadium.MailboxHeldMailCount + SAV2Stadium.MailboxMailCount];
                for (int i = 0; i < m.Length; i++)
                    m[i] = new Mail2(sav2Stadium, i);

                NUD_BoxSize.Value = SAV.Data[Mail2.GetMailboxOffsetStadium2(SAV.Language)];
                MailItemID = [0x9E, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD];
                PartyBoxCount = SAV2Stadium.MailboxHeldMailCount;
                NUD_BoxSize.Maximum = SAV2Stadium.MailboxMailCount;
                break;
            case SAV3 sav3:
                m = new Mail3[6 + 10];
                for (int i = 0; i < m.Length; i++)
                    m[i] = sav3.GetMail(i);

                MailItemID = [121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132];
                PartyBoxCount = 6;
                break;
            case SAV4 sav4:
                m = new Mail4[p.Count + 20];
                for (int i = 0; i < p.Count; i++)
                    m[i] = new Mail4(((PK4)p[i]).HeldMail.ToArray());
                for (int i = p.Count, j = 0; i < m.Length; i++, j++)
                    m[i] = sav4.GetMail(j);
                var l4 = (Mail4)m[^1];
                ResetVer = l4.AuthorVersion;
                ResetLang = l4.AuthorLanguage;
                MailItemID = [137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148];
                PartyBoxCount = p.Count;
                break;
            case SAV5 sav5:
                m = new Mail5[p.Count + 20];
                for (int i = 0; i < p.Count; i++)
                    m[i] = new Mail5(((PK5)p[i]).HeldMail.ToArray());
                for (int i = p.Count, j = 0; i < m.Length; i++, j++)
                    m[i] = sav5.GetMail(j);
                var l5 = (Mail5)m[^1];
                ResetVer = l5.AuthorVersion;
                ResetLang = l5.AuthorLanguage;
                MailItemID = [137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148];
                PartyBoxCount = p.Count;
                break;
        }
        MakePartyList();
        MakePCList();

        if (Generation is 2 or 3)
        {
            CB_AppearPKM1.Items.Clear();
            CB_AppearPKM1.InitializeBinding();
            CB_AppearPKM1.DataSource = new BindingSource(GameInfo.FilteredSources.Species.ToList(), null);
            B_PartyUp.Visible = B_PartyDown.Visible = B_BoxUp.Visible = B_BoxDown.Visible = true;
        }
        else if (Generation is 4 or 5)
        {
            var species = GameInfo.FilteredSources.Species.ToList();
            foreach (ComboBox a in AppearPKMs)
            {
                a.Items.Clear();
                a.InitializeBinding();
                a.DataSource = new BindingSource(species, null);
            }

            var vers = GameInfo.VersionDataSource
                .Where(z => ((GameVersion)z.Value).GetGeneration() == Generation);
            CB_AuthorVersion.Items.Clear();
            CB_AuthorVersion.InitializeBinding();
            CB_AuthorVersion.DataSource = new BindingSource(vers, null);
        }

        if (Generation is 2 or 4 or 5)
        {
            CB_AuthorLang.Items.Clear();
            CB_AuthorLang.InitializeBinding();
            CB_AuthorLang.DataSource = new BindingSource(GameInfo.LanguageDataSource(SAV.Generation), null);
        }

        var ItemList = GameInfo.Strings.GetItemStrings(SAV.Context, SAV.Version);
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
        if (Generation == 2)
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
            PKMHeldItems[i].Text = j >= 0 ? CB_MailType.Items[j + 1]!.ToString() : "(not Mail)";
            if (Generation != 3)
                continue;
            int k = ((PK3)p[i]).HeldMailID;
            PKMNUDs[i].Value = k is >= -1 and <= 5 ? k : -1;
        }
        editing = false;
    }

    private void Save()
    {
        switch (Generation)
        {
            case 2:
                foreach (var n in m) n.CopyTo(SAV);
                if (SAV is SAV2)
                {
                    // duplicate
                    int ofs = 0x600;
                    int len = Mail2.GetMailSize(SAV.Language) * 6;
                    Array.Copy(SAV.Data, ofs, SAV.Data, ofs + len, len);
                    ofs += len << 1;
                    SAV.Data[ofs] = (byte)NUD_BoxSize.Value;
                    len = (Mail2.GetMailSize(SAV.Language) * 10) + 1;
                    Array.Copy(SAV.Data, ofs, SAV.Data, ofs + len, len);
                }
                else if (SAV is SAV2Stadium)
                {
                    int ofs = Mail2.GetMailboxOffsetStadium2(SAV.Language);
                    SAV.Data[ofs] = (byte)NUD_BoxSize.Value;
                }
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
        MailDetail mail = m[entry];
        mail.AuthorName = TB_AuthorName.Text;
        mail.AuthorTID = (ushort)NUD_AuthorTID.Value;
        mail.MailType = CBIndexToMailType(CB_MailType.SelectedIndex);
        // ReSharper disable once ConstantNullCoalescingCondition
        var species = (ushort)WinFormsUtil.GetIndex(CB_AppearPKM1);
        if (Generation == 2)
        {
            mail.AppearPKM = species;
            mail.SetMessage(TB_MessageBody21.Text, TB_MessageBody22.Text, CHK_UserEntered.Checked);
            // ReSharper disable once ConstantNullCoalescingCondition
            mail.AuthorLanguage = (byte)((int?)CB_AuthorLang.SelectedValue ?? (int)LanguageID.English);
            return;
        }
        mail.AuthorSID = (ushort)NUD_AuthorSID.Value;
        for (int y = 0, xc = Generation == 3 ? 3 : 4; y < 3; y++)
        {
            for (int x = 0; x < xc; x++)
                mail.SetMessage(y, x, (ushort)Messages[y][x].Value);
        }
        if (Generation == 3)
        {
            mail.AppearPKM = SpeciesConverter.GetInternal3(species);
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
                {
                    var index = WinFormsUtil.GetIndex(AppearPKMs[i]);
                    if (index == -1)
                        index = 0;
                    else
                        index += 7;
                    m4.SetAppearSpecies(i, (ushort)index);
                }

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
        // D: other pk have same heldMailID. it should be different.
        // E: mail is not empty, but no pk refer to the mail. it should be empty, or someone refer to the mail.
        if (Generation == 3)
        {
            Span<int> heldMailIDs = stackalloc int[p.Count];
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
                var count = heldMailIDs.Count(i);
                if (count > 1) //D
                    ret.Add($"MailID{i} duplicated");
                if (m[i].IsEmpty == false && count == 0) //E
                    ret.Add($"MailID{i} not referred");
            }
        }
        // Gen2, Gen4
        // P: held item is mail, but mail is empty(invalid mail type. g2:not 181 to 189, g4:12 to 255). it should be not empty or held not mail.
        // Q: held item is not mail, but mail is not empty. it should be empty or held mail.
        else if (Generation is 2 or 4)
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
        // Gen5, move mail to pc will not erase mail data, still remains, duplicates.
        else if (Generation == 5)
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
    private int MailTypeToCBIndex(MailDetail mail) => Generation <= 3 ? 1 + Array.IndexOf(MailItemID, mail.MailType) : (mail.IsEmpty == false ? 1 + mail.MailType : 0);
    private int CBIndexToMailType(int cbindex) => Generation <= 3 ? (cbindex > 0 ? MailItemID[cbindex - 1] : 0) : (cbindex > 0 ? cbindex - 1 : 0xFF);

    private string GetSpeciesNameFromCB(int index)
    {
        var result = CB_AppearPKM1.Items.OfType<ComboItem>().FirstOrDefault(z => z.Value == index);
        return result != null ? result.Text : "PKM";
    }

    private DialogResult ModifyHeldItem()
    {
        DialogResult ret = DialogResult.Abort;
        var s = p.Select((pk, i) => ((sbyte)PKMNUDs[i].Value == entry) && ItemIsMail(pk.HeldItem) ? pk : null).ToArray();
        if (s.All(v => v == null))
            return ret;
        System.Media.SystemSounds.Question.Play();
        var msg = $"{s.Select((v, i) => v == null ? string.Empty : $"{Environment.NewLine}  {PKMLabels[i].Text}: {PKMHeldItems[i].Text} -> {CB_MailType.Items[0]}").Aggregate($"Modify PKM's HeldItem?{Environment.NewLine}", (tmp, v) => $"{tmp}{v}")}{Environment.NewLine}{Environment.NewLine}Yes: Delete Mail & Modify PKM{Environment.NewLine}No: Delete Mail";
        ret = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel, msg);
        if (ret != DialogResult.Yes)
            return ret;
        foreach (var pk in s)
        {
            if (pk == null)
                continue;

            pk.HeldItem = 0;
            if (Generation == 3)
                ((PK3)pk).HeldMailID = -1;
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
        if (editing)
            return;
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
        MailDetail mail = m[entry];
        TB_AuthorName.Text = mail.AuthorName;
        NUD_AuthorTID.Value = mail.AuthorTID;
        CB_MailType.SelectedIndex = MailTypeToCBIndex(mail);
        var species = mail.AppearPKM;
        if (Generation == 2)
        {
            AppearPKMs[0].SelectedValue = (int)species;
            TB_MessageBody21.Text = mail.GetMessage(false);
            TB_MessageBody22.Text = mail.GetMessage(true);
            CB_AuthorLang.SelectedValue = (int)mail.AuthorLanguage;
            CB_AuthorLang.Enabled = CB_AuthorLang.SelectedValue is not (int)LanguageID.Japanese and not (int)LanguageID.Korean;
            CHK_UserEntered.Checked = mail.UserEntered;
            editing = false;
            return;
        }
        NUD_AuthorSID.Value = mail.AuthorSID;
        for (int y = 0, xc = Generation == 3 ? 3 : 4; y < 3; y++)
        {
            for (int x = 0; x < xc; x++)
                Messages[y][x].Value = mail.GetMessage(y, x);
        }
        if (Generation == 3)
        {
            AppearPKMs[0].SelectedValue = (int)SpeciesConverter.GetNational3(species);
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
                    AppearPKMs[i].SelectedValue = Math.Max(0, m4.GetAppearSpecies(i) - 7);
                break;
            case Mail5 m5:
                for (int i = 0; i < Miscs.Length; i++)
                    Miscs[i].Value = m5.GetMisc(i);
                NUD_MessageEnding.Value = m5.MessageEnding;
                break;
        }
        editing = false;
    }

    private readonly string[] gendersymbols = ["♂", "♀"];

    private void LoadOTlabel()
    {
        Label_OTGender.Text = gendersymbols[LabelValue_GenderF ? 1 : 0];
        Label_OTGender.ForeColor = Main.Draw.GetGenderColor((byte)(LabelValue_GenderF ? 1 : 0));
    }

    private void Label_OTGender_Click(object sender, EventArgs e)
    {
        LabelValue_GenderF ^= true;
        LoadOTlabel();
    }

    private void NUD_BoxSize_ValueChanged(object sender, EventArgs e) => MakePCList();

    private void NUD_MailIDn_ValueChanged(object sender, EventArgs e)
    {
        if (editing || Generation != 3)
            return;
        int index = Array.IndexOf(PKMNUDs, (NumericUpDown)sender);
        if (index < 0 || index >= p.Count)
            return;
        ((PK3)p[index]).HeldMailID = (sbyte)PKMNUDs[index].Value;
    }

    private void B_PartyUp_Click(object sender, EventArgs e) => SwapSlots(LB_PartyHeld, false);
    private void B_PartyDown_Click(object sender, EventArgs e) => SwapSlots(LB_PartyHeld, true);
    private void B_BoxUp_Click(object sender, EventArgs e) => SwapSlots(LB_PCBOX, false);
    private void B_BoxDown_Click(object sender, EventArgs e) => SwapSlots(LB_PCBOX, true);

    private void SwapSlots(ListBox lb, bool down)
    {
        int index = lb.SelectedIndex;
        var otherIndex = index + (down ? 1 : -1);
        if ((uint)otherIndex >= lb.Items.Count)
        {
            System.Media.SystemSounds.Asterisk.Play();
            return;
        }

        // Shift to actual indexes
        bool isBox = lb == LB_PCBOX;
        if (isBox)
        {
            index += PartyBoxCount;
            otherIndex += PartyBoxCount;
        }

        editing = true;

        // swap mail objects
        (m[otherIndex], m[index]) = (m[index], m[otherIndex]);
        if ((entry >= PartyBoxCount) == isBox)
            entry = otherIndex;
        LoadList(); // reset labels

        // move selection to new spot
        if (isBox)
            otherIndex -= PartyBoxCount;
        lb.SelectedIndex = otherIndex;

        editing = false;
    }
}
