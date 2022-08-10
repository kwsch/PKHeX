using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.Drawing.Misc;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public partial class KChart : Form
{
    private readonly SaveFile SAV;
    private readonly string[] abilities;

    public KChart(SaveFile sav)
    {
        InitializeComponent();
        Icon = Properties.Resources.Icon;
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        var pt = SAV.Personal;
        var strings = GameInfo.Strings;
        var species = strings.specieslist;
        abilities = strings.abilitylist;

        DGV.Rows.Clear();
        for (int s = 1; s <= pt.MaxSpeciesID; s++)
        {
            var fc = pt[s, 0].FormCount;
            var formNames = fc <= 1
                ? Array.Empty<string>()
                : FormConverter.GetFormList(s, strings.Types, strings.forms, Main.GenderSymbols, SAV.Context);

            for (int f = 0; f < fc; f++)
            {
                var name = f == 0 ? species[s] : $"{species[s]}-{(f < formNames.Length ? formNames[f] : f.ToString())}";
                PopEntry(s, f, name);
            }
        }

        DGV.DoubleBuffered(true);

        DGV.Sort(DGV.Columns[0], ListSortDirection.Ascending);
    }

    private void PopEntry(int species, int form, string name)
    {
        var p = SAV.Personal.GetFormEntry(species, form);
        if (p.HP == 0)
            return;

        var row = new DataGridViewRow();
        row.CreateCells(DGV);
        var cells = row.Cells;
        int c = 0;

        var bst = p.GetBaseStatTotal();
        cells[c++].Value = species.ToString("000") + (form > 0 ? $"-{form:00}" : "");
        cells[c++].Value = SpriteUtil.GetSprite(species, form, 0, 0, 0, false, Shiny.Never, SAV.Generation);
        cells[c++].Value = name;
        cells[c++].Value = GetIsNative(p, species);
        cells[c].Style.BackColor = ColorUtil.ColorBaseStatTotal(bst);
        cells[c++].Value = bst.ToString("000");
        cells[c++].Value = p.CatchRate.ToString("000");
        cells[c++].Value = TypeSpriteUtil.GetTypeSprite(p.Type1, SAV.Generation);
        cells[c++].Value = p.Type1 == p.Type2 ? SpriteUtil.Spriter.Transparent : TypeSpriteUtil.GetTypeSprite(p.Type2, SAV.Generation);
        cells[c].Style.BackColor = ColorUtil.ColorBaseStat(p.HP);
        cells[c++].Value = p.HP.ToString("000");
        cells[c].Style.BackColor = ColorUtil.ColorBaseStat(p.ATK);
        cells[c++].Value = p.ATK.ToString("000");
        cells[c].Style.BackColor = ColorUtil.ColorBaseStat(p.DEF);
        cells[c++].Value = p.DEF.ToString("000");
        cells[c].Style.BackColor = ColorUtil.ColorBaseStat(p.SPA);
        cells[c++].Value = p.SPA.ToString("000");
        cells[c].Style.BackColor = ColorUtil.ColorBaseStat(p.SPD);
        cells[c++].Value = p.SPD.ToString("000");
        cells[c].Style.BackColor = ColorUtil.ColorBaseStat(p.SPE);
        cells[c++].Value = p.SPE.ToString("000");
        var abils = p.Abilities;
        cells[c++].Value = GetAbility(abils, 0);
        cells[c++].Value = GetAbility(abils, 1);
        cells[c].Value = GetAbility(abils, 2);

        row.Height = SpriteUtil.Spriter.Height + 1;
        DGV.Rows.Add(row);
    }

    private string GetAbility(IReadOnlyList<int> abilityIDs, int index)
    {
        if ((uint)index >= abilityIDs.Count)
            return abilities[0];
        return abilities[abilityIDs[index]];
    }

    private static bool GetIsNative(IPersonalInfo personalInfo, int s) => personalInfo switch
    {
        PersonalInfo7 => s > 721 || Legal.PastGenAlolanNatives.Contains(s),
        PersonalInfo8SWSH ss => ss.IsInDex,
        PersonalInfo8BDSP bs => bs.IsInDex,
        _ => true,
    };
}
