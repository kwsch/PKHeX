using System.ComponentModel;
using System.Drawing;
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
    private readonly string[] moves;
    private readonly string SpeciesNumberFormat;

    public KChart(SaveFile sav)
    {
        InitializeComponent();
        Icon = Properties.Resources.Icon;
        if (sav is SAV9ZA)
            DGV.Columns.Add(new DataGridViewTextBoxColumn { ReadOnly = true, HeaderText = "Alpha Move" });
        DGV.AllowUserToOrderColumns = true;

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        var pt = SAV.Personal;
        var strings = GameInfo.Strings;
        var species = strings.specieslist;
        abilities = strings.abilitylist;
        moves = strings.movelist;
        SpeciesNumberFormat = sav.MaxSpeciesID >= 1000 ? "0000" : "000";

        DGV.Rows.Clear();
        for (ushort s = 1; s <= pt.MaxSpeciesID; s++)
        {
            var fc = pt[s, 0].FormCount;
            var formNames = fc <= 1
                ? []
                : FormConverter.GetFormList(s, strings.Types, strings.forms, Main.GenderSymbols, SAV.Context);

            for (byte f = 0; f < fc; f++)
            {
                var name = f == 0 ? species[s] : $"{species[s]}-{(f < formNames.Length ? formNames[f] : f.ToString())}";
                PopEntry(s, f, name, pt);
            }
        }

        DGV.Sort(DGV.Columns[0], ListSortDirection.Ascending);
    }

    private void PopEntry(ushort species, byte form, string name, IPersonalTable pt)
    {
        if (!pt.IsPresentInGame(species, form))
            return;

        var p = pt.GetFormEntry(species, form);
        var row = new DataGridViewRow();
        row.CreateCells(DGV);
        var cells = row.Cells;
        int c = 0;

        var bst = p.BST;
        cells[c++].Value = species.ToString(SpeciesNumberFormat) + (form > 0 ? $"-{form:00}" : string.Empty);
        cells[c++].Value = SpriteUtil.GetSprite(species, form, 0, 0, 0, false, Shiny.Never, SAV.Context);
        cells[c++].Value = name;
        cells[c++].Value = GetIsNative(p, species);
        cells[c].Style.BackColor = ColorUtil.ColorBaseStatTotal(bst);
        cells[c].Style.ForeColor = Color.Black;
        cells[c++].Value = bst.ToString("000");
        cells[c++].Value = p.CatchRate.ToString("000");
        cells[c++].Value = TypeSpriteUtil.GetTypeSpriteWide(p.Type1, SAV.Generation);
        cells[c++].Value = p.Type1 == p.Type2 ? SpriteUtil.Spriter.Transparent : TypeSpriteUtil.GetTypeSpriteWide(p.Type2, SAV.Generation);
        Stat(cells[c++], p.HP);
        Stat(cells[c++], p.ATK);
        Stat(cells[c++], p.DEF);
        Stat(cells[c++], p.SPA);
        Stat(cells[c++], p.SPD);
        Stat(cells[c++], p.SPE);
        var abils = p.AbilityCount;
        cells[c++].Value = abilities[abils > 0 ? p.GetAbilityAtIndex(0) : 0];
        cells[c++].Value = abilities[abils > 1 ? p.GetAbilityAtIndex(1) : 0];
        cells[c++].Value = abilities[abils > 2 ? p.GetAbilityAtIndex(2) : 0];
        if (p is PersonalInfo9ZA za)
            cells[c].Value = moves[za.AlphaMove];

        row.Height = SpriteUtil.Spriter.Height + 1;
        DGV.Rows.Add(row);

        static void Stat(DataGridViewCell cell, int value)
        {
            cell.Style.ForeColor = Color.Black;
            cell.Style.BackColor = ColorUtil.ColorBaseStat(value);
            cell.Value = value.ToString("000");
        }
    }

    private static bool GetIsNative(IPersonalInfo personalInfo, ushort species) => personalInfo switch
    {
        PersonalInfo7 => PersonalInfo7.IsPastGenNative(species),
        PersonalInfo8SWSH ss => ss.IsInDex,
        PersonalInfo8BDSP bs => bs.IsInDex,
        PersonalInfo8LA bs => bs.IsPresentInGame,
        PersonalInfo9SV sv => sv.IsInDex,
        PersonalInfo9ZA za => za is { IsLumioseNative: true },
        _ => true,
    };
}
