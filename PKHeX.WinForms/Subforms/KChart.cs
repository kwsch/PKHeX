using System;
using System.ComponentModel;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public partial class KChart : Form
    {
        private readonly SaveFile SAV;
        private readonly string[] species = GameInfo.Strings.specieslist;
        private readonly string[] abilities = GameInfo.Strings.abilitylist;
        private readonly bool alolanOnly;
        private readonly int[] baseForm;
        private readonly int[] formVal;

        public KChart(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = sav;
            alolanOnly = SAV is SAV7 && DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Alolan Dex only?");

            Array.Resize(ref species, SAV.Personal.TableLength);

            var AltForms = SAV.Personal.GetFormList(species, SAV.MaxSpeciesID);
            species = SAV.Personal.GetPersonalEntryList(AltForms, species, SAV.MaxSpeciesID, out baseForm, out formVal);

            DGV.Rows.Clear();
            for (int i = 1; i < species.Length; i++)
                PopEntry(i);

            DGV.Sort(DGV.Columns[0], ListSortDirection.Ascending);
        }

        private void PopEntry(int index)
        {
            var p = SAV.Personal[index];

            int s = index > SAV.MaxSpeciesID ? baseForm[index] : index;
            var f = index <= SAV.MaxSpeciesID ? 0 : formVal[index];
            bool alolan = s > 721 || Legal.PastGenAlolanNatives.Contains(s);

            if (alolanOnly && !alolan)
                return;

            var row = new DataGridViewRow();
            row.CreateCells(DGV);

            int r = 0;
            row.Cells[r++].Value = s.ToString("000") + (f > 0 ? "-"+f.ToString("00") :"");
            row.Cells[r++].Value = SpriteUtil.GetSprite(s, f, 0, 0, false, false, SAV.Generation);
            row.Cells[r++].Value = species[index];
            row.Cells[r++].Value = s > 721 || Legal.PastGenAlolanNatives.Contains(s);
            row.Cells[r].Style.BackColor = ImageUtil.ColorBaseStat((int)((Math.Max(p.BST - 175, 0)) / 3f));
            row.Cells[r++].Value = p.BST.ToString("000");
            row.Cells[r++].Value = SpriteUtil.GetTypeSprite(p.Type1, SAV.Generation);
            row.Cells[r++].Value = p.Type1 == p.Type2 ? Resources.slotTrans : SpriteUtil.GetTypeSprite(p.Type2, SAV.Generation);
            row.Cells[r].Style.BackColor = ImageUtil.ColorBaseStat(p.HP);
            row.Cells[r++].Value = p.HP.ToString("000");
            row.Cells[r].Style.BackColor = ImageUtil.ColorBaseStat(p.ATK);
            row.Cells[r++].Value = p.ATK.ToString("000");
            row.Cells[r].Style.BackColor = ImageUtil.ColorBaseStat(p.DEF);
            row.Cells[r++].Value = p.DEF.ToString("000");
            row.Cells[r].Style.BackColor = ImageUtil.ColorBaseStat(p.SPA);
            row.Cells[r++].Value = p.SPA.ToString("000");
            row.Cells[r].Style.BackColor = ImageUtil.ColorBaseStat(p.SPD);
            row.Cells[r++].Value = p.SPD.ToString("000");
            row.Cells[r].Style.BackColor = ImageUtil.ColorBaseStat(p.SPE);
            row.Cells[r++].Value = p.SPE.ToString("000");
            row.Cells[r++].Value = abilities[p.Abilities[0]];
            row.Cells[r++].Value = abilities[p.Abilities[1]];
            row.Cells[r].Value = abilities[p.Abilities.Length <= 2 ? 0 : p.Abilities[2]];
            DGV.Rows.Add(row);
        }
    }
}
