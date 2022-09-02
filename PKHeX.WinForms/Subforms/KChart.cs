using System;
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
        for (ushort s = 1; s <= pt.MaxSpeciesID; s++)
        {
            var fc = pt[s, 0].FormCount;
            var formNames = fc <= 1
                ? Array.Empty<string>()
                : FormConverter.GetFormList(s, strings.Types, strings.forms, Main.GenderSymbols, SAV.Context);

            for (byte f = 0; f < fc; f++)
            {
                var name = f == 0 ? species[s] : $"{species[s]}-{(f < formNames.Length ? formNames[f] : f.ToString())}";
                PopEntry(s, f, name);
            }
        }

        DGV.DoubleBuffered(true);

        DGV.Sort(DGV.Columns[0], ListSortDirection.Ascending);
    }

    private void PopEntry(ushort species, byte form, string name)
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
        var abils = p.AbilityCount;
        cells[c++].Value = abilities[abils > 0 ? p.GetAbilityAtIndex(0) : 0];
        cells[c++].Value = abilities[abils > 1 ? p.GetAbilityAtIndex(1) : 0];
        cells[c].Value   = abilities[abils > 2 ? p.GetAbilityAtIndex(2) : 0];

        row.Height = SpriteUtil.Spriter.Height + 1;
        DGV.Rows.Add(row);
    }

    private static bool GetIsNative(IPersonalInfo personalInfo, ushort s) => personalInfo switch
    {
        PersonalInfo7 => s > 721 || Legal.PastGenAlolanNatives.Contains(s),
        PersonalInfo8SWSH ss => ss.IsInDex,
        PersonalInfo8BDSP bs => bs.IsInDex,
        PersonalInfo8LA bs => bs.IsPresentInGame,
        _ => true,
    };
}
