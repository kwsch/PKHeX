using System;
using System.ComponentModel;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms
{
    public partial class KChart : Form
    {
        private readonly SaveFile SAV;
        private readonly string[] species = GameInfo.Strings.specieslist;
        private readonly string[] abilities = GameInfo.Strings.abilitylist;
        private readonly int[] baseForm;
        private readonly int[] formVal;

        public KChart(SaveFile sav)
        {
            InitializeComponent();
            Icon = Properties.Resources.Icon;
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = sav;

            Array.Resize(ref species, SAV.Personal.TableLength);

            var AltForms = SAV.Personal.GetFormList(species, SAV.MaxSpeciesID);
            species = SAV.Personal.GetPersonalEntryList(AltForms, species, SAV.MaxSpeciesID, out baseForm, out formVal);

            DGV.Rows.Clear();
            for (int i = 1; i < species.Length; i++)
                PopEntry(i);

            DGV.DoubleBuffered(true);

            DGV.Sort(DGV.Columns[0], ListSortDirection.Ascending);
        }

        private void PopEntry(int index)
        {
            var p = SAV.Personal[index];
            if (p.HP == 0)
                return;

            int s = index > SAV.MaxSpeciesID ? baseForm[index] : index;
            var f = index <= SAV.MaxSpeciesID ? 0 : formVal[index];

            var row = new DataGridViewRow();
            row.CreateCells(DGV);

            int r = 0;
            row.Cells[r++].Value = s.ToString("000") + (f > 0 ? "-"+f.ToString("00") :"");
            row.Cells[r++].Value = SpriteUtil.GetSprite(s, f, 0, 0, 0, false, false, SAV.Generation);
            row.Cells[r++].Value = species[index];
            row.Cells[r++].Value = GetIsNative(p, s);
            row.Cells[r].Style.BackColor = ImageUtil.ColorBaseStatTotal(p.BST);
            row.Cells[r++].Value = p.BST.ToString("000");
            row.Cells[r++].Value = SpriteUtil.GetTypeSprite(p.Type1, SAV.Generation);
            row.Cells[r++].Value = p.Type1 == p.Type2 ? SpriteUtil.Spriter.Transparent : SpriteUtil.GetTypeSprite(p.Type2, SAV.Generation);
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
            row.Height = SpriteUtil.Spriter.Height + 1;
            DGV.Rows.Add(row);
        }

        private static bool GetIsNative(PersonalInfo personalInfo, int s)
        {
            return personalInfo switch
            {
                PersonalInfoSM _ => s > 721 || Legal.PastGenAlolanNatives.Contains(s),
                PersonalInfoSWSH ss => ss.PokeDexIndex > 0,
                _ => true,
            };
        }
    }
}
