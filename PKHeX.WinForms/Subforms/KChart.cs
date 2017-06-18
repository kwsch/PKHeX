using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
            SAV = sav;
            alolanOnly = SAV.Generation == 7 && DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Alolan Dex only?");
            InitializeComponent();

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
            row.Cells[r++].Value = PKMUtil.GetSprite(s, f, 0, 0, false, false, SAV.Generation);
            row.Cells[r++].Value = species[index];
            row.Cells[r++].Value = s > 721 || Legal.PastGenAlolanNatives.Contains(s);
            row.Cells[r].Style.BackColor = MapColor((int)((p.BST - 175) / 3f));
            row.Cells[r++].Value = p.BST.ToString("000");
            row.Cells[r++].Value = PKMUtil.GetTypeSprite(p.Types[0]);
            row.Cells[r++].Value = p.Types[0] == p.Types[1] ? Resources.slotTrans : PKMUtil.GetTypeSprite(p.Types[1]);
            row.Cells[r].Style.BackColor = MapColor(p.HP);
            row.Cells[r++].Value = p.HP.ToString("000");
            row.Cells[r].Style.BackColor = MapColor(p.ATK);
            row.Cells[r++].Value = p.ATK.ToString("000");
            row.Cells[r].Style.BackColor = MapColor(p.DEF);
            row.Cells[r++].Value = p.DEF.ToString("000");
            row.Cells[r].Style.BackColor = MapColor(p.SPA);
            row.Cells[r++].Value = p.SPA.ToString("000");
            row.Cells[r].Style.BackColor = MapColor(p.SPD);
            row.Cells[r++].Value = p.SPD.ToString("000");
            row.Cells[r].Style.BackColor = MapColor(p.SPE);
            row.Cells[r++].Value = p.SPE.ToString("000");
            row.Cells[r++].Value = abilities[p.Abilities[0]];
            row.Cells[r++].Value = abilities[p.Abilities[1]];
            row.Cells[r].Value = abilities[p.Abilities[2]];
            DGV.Rows.Add(row);
        }
        private static Color MapColor(int v)
        {
            const float maxval = 180; // shift the green cap down
            float x = 100f * v / maxval;
            if (x > 100)
                x = 100;
            double red = 255f * (x > 50 ? 1 - 2 * (x - 50) / 100.0 : 1.0);
            double green = 255f * (x > 50 ? 1.0 : 2 * x / 100.0);

            return Blend(Color.FromArgb((int)red, (int)green, 0), Color.White, 0.4);
        }
        private static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)(color.R * amount + backColor.R * (1 - amount));
            byte g = (byte)(color.G * amount + backColor.G * (1 - amount));
            byte b = (byte)(color.B * amount + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }
    }
}
