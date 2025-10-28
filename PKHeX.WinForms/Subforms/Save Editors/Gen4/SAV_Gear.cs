using PKHeX.Core;
using System;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class SAV_Gear : Form
{
    private readonly SaveFile Origin;
    private readonly SAV4BR SAV;

    private readonly string[] CharacterStyles = WinFormsTranslator.GetEnumTranslation<ModelBR>(Main.CurrentLanguage);
    private readonly string[] GearCategories = WinFormsTranslator.GetEnumTranslation<GearCategory>(Main.CurrentLanguage);
    private readonly string[] GearNames = GameLanguage.GetStrings("gear", Main.CurrentLanguage);

    public SAV_Gear(SAV4BR sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4BR)(Origin = sav).Clone();
        InitializeDGV();

        CHK_Groudon.Checked = SAV.GearShinyGroudonOutfit;
        CHK_Lucario.Checked = SAV.GearShinyLucarioOutfit;
        CHK_Electivire.Checked = SAV.GearShinyElectivireOutfit;
        CHK_Kyogre.Checked = SAV.GearShinyKyogreOutfit;
        CHK_Roserade.Checked = SAV.GearShinyRoseradeOutfit;
        CHK_Pachirisu.Checked = SAV.GearShinyPachirisuOutfit;
    }

    private void InitializeDGV()
    {
        DGV_Gear.Rows.Clear();
        for (ModelBR model = ModelBR.YoungBoy; model <= ModelBR.LittleGirl; model++)
        {
            for (GearCategory category = 0; (int)category < GearUnlock.CategoryCount; category++)
            {
                var (offset, count) = GearUnlock.GetOffsetCount(model, category);
                for (int i = 0; i < count; i++)
                {
                    // The unlock flags for badges are shared.
                    // In the Character Style column, display "All" instead of "Young Boy".
                    bool shared = category is GearCategory.Badges && i != 0;
                    AddRowDGV(model, category, offset + i, shared);
                }
            }
        }
    }

    private void AddRowDGV(ModelBR model, GearCategory category, int index, bool shared)
    {
        var row = DGV_Gear.Rows[DGV_Gear.Rows.Add()];
        row.Cells[0].Value = index;
        row.Cells[1].Value = shared ? MessageStrings.MsgGearAllCharacterStyles : CharacterStyles[(int)model];
        row.Cells[2].Value = GearCategories[(int)category];
        row.Cells[3].Value = GearNames[index];
        row.Cells[4].Value = SAV.GearUnlock.Get(index);
    }

    private void RefreshDGV()
    {
        for (int i = 0; i < DGV_Gear.Rows.Count; i++)
        {
            var row = DGV_Gear.Rows[i];
            var index = (int)row.Cells[0].Value!;
            row.Cells[4].Value = SAV.GearUnlock.Get(index);
        }
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < DGV_Gear.Rows.Count; i++)
        {
            var row = DGV_Gear.Rows[i];
            var index = (int)row.Cells[0].Value!;
            var unlocked = (bool)row.Cells[4].Value!;
            SAV.GearUnlock.Set(index, unlocked);
        }

        SAV.GearShinyGroudonOutfit = CHK_Groudon.Checked;
        SAV.GearShinyLucarioOutfit = CHK_Lucario.Checked;
        SAV.GearShinyElectivireOutfit = CHK_Electivire.Checked;
        SAV.GearShinyKyogreOutfit = CHK_Kyogre.Checked;
        SAV.GearShinyRoseradeOutfit = CHK_Roserade.Checked;
        SAV.GearShinyPachirisuOutfit = CHK_Pachirisu.Checked;

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_UnlockAll_Click(object sender, EventArgs e)
    {
        SAV.GearUnlock.UnlockAll();
        RefreshDGV();
    }

    private void B_Clear_Click(object sender, EventArgs e)
    {
        SAV.GearUnlock.Clear();
        RefreshDGV();
    }
}
