using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_OPower : Form
{
    private readonly SaveFile Origin;
    private readonly SaveFile SAV;
    private readonly OPower6 Block;

    private readonly NumericUpDown[] NUDField_A;
    private readonly NumericUpDown[] NUDField_B;
    private readonly NumericUpDown[] NUDBattle_A;
    private readonly NumericUpDown[] NUDBattle_B;

    public SAV_OPower(ISaveBlock6Main sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        Origin = (SaveFile)sav;
        SAV = Origin.Clone();
        Block = ((ISaveBlock6Main)SAV).OPower;

        Label[] LabelField = [L_F0, L_F1, L_F2, L_F3, L_F4, L_F5, L_F6, L_F7, L_F8, L_F9];
        Label[] LabelBattle = [L_B0, L_B1, L_B2, L_B3, L_B4, L_B5, L_B6];
        NUDField_A = [NUD_F0A, NUD_F1A, NUD_F2A, NUD_F3A, NUD_F4A, NUD_F5A, NUD_F6A, NUD_F7A, NUD_F8A, NUD_F9A];
        NUDField_B = [NUD_F0B, NUD_F1B, NUD_F2B, NUD_F3B, NUD_F4B, NUD_F5B, NUD_F6B, NUD_F7B, NUD_F8B, NUD_F9B];
        NUDBattle_A = [NUD_B0A, NUD_B1A, NUD_B2A, NUD_B3A, NUD_B4A, NUD_B5A, NUD_B6A];
        NUDBattle_B = [NUD_B0B, NUD_B1B, NUD_B2B, NUD_B3B, NUD_B4B, NUD_B5B, NUD_B6B];

        // get names, without the "Count" enum value at the end.
        var nameIndex = WinFormsTranslator.GetEnumTranslation<OPower6Index>(Main.CurrentLanguage).AsSpan()[..^1];
        var nameField = WinFormsTranslator.GetEnumTranslation<OPower6FieldType>(Main.CurrentLanguage).AsSpan()[..^1];
        var nameBattle = WinFormsTranslator.GetEnumTranslation<OPower6BattleType>(Main.CurrentLanguage).AsSpan()[..^1];
        foreach (string index in nameIndex)
            CLB_Unlock.Items.Add(index);
        for (int i = 0; i < nameField.Length; i++)
            LabelField[i].Text = nameField[i];
        for (int i = 0; i < nameBattle.Length; i++)
            LabelBattle[i].Text = nameBattle[i];

        LoadCurrent();

        B_ClearAll.Click += (_, _) => { Block.ClearAll(); LoadCurrent(); };
        B_GiveAll.Click += (_, _) => { Block.UnlockAll(); LoadCurrent(); };
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveCurrent();
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void LoadCurrent()
    {
        for (int i = 0; i < CLB_Unlock.Items.Count; i++)
            CLB_Unlock.SetItemChecked(i, Block.GetState((OPower6Index)i) == OPowerFlagState.Unlocked);
        for (int i = 0; i < NUDField_A.Length; i++)
        {
            NUDField_A[i].Value = Block.GetLevel1((OPower6FieldType)i);
            NUDField_B[i].Value = Block.GetLevel2((OPower6FieldType)i);
        }
        for (int i = 0; i < NUDBattle_A.Length; i++)
        {
            NUDBattle_A[i].Value = Block.GetLevel1((OPower6BattleType)i);
            NUDBattle_B[i].Value = Block.GetLevel2((OPower6BattleType)i);
        }
        NUD_Points.Value = Block.Points;
    }

    private void SaveCurrent()
    {
        for (int i = 0; i < CLB_Unlock.Items.Count; i++)
            Block.SetState((OPower6Index)i, CLB_Unlock.GetItemChecked(i) ? OPowerFlagState.Unlocked : OPowerFlagState.Locked);
        for (int i = 0; i < NUDField_A.Length; i++)
        {
            Block.SetLevel1((OPower6FieldType)i, (byte)NUDField_A[i].Value);
            Block.SetLevel2((OPower6FieldType)i, (byte)NUDField_B[i].Value);
        }
        for (int i = 0; i < NUDBattle_A.Length; i++)
        {
            Block.SetLevel1((OPower6BattleType)i, (byte)NUDBattle_A[i].Value);
            Block.SetLevel2((OPower6BattleType)i, (byte)NUDBattle_B[i].Value);
        }
        Block.Points = (byte)NUD_Points.Value;
    }
}
