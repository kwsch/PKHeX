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

        var bst = p.GetBaseStatTotal();
        cells[c++].Value = species.ToString(pt.MaxSpeciesID > 999 ? "0000" : "000") + (form > 0 ? $"-{form:00}" : "");
        cells[c++].Value = SpriteUtil.GetSprite(species, form, 0, 0, 0, false, Shiny.Never, SAV.Context);
        cells[c++].Value = name;
        cells[c++].Value = GetIsNative(p, species);
        cells[c].Style.BackColor = ColorUtil.ColorBaseStatTotal(bst);
        cells[c++].Value = bst.ToString("000");
        cells[c++].Value = p.CatchRate.ToString("000");
        cells[c++].Value = TypeSpriteUtil.GetTypeSpriteWide(p.Type1, SAV.Generation);
        cells[c++].Value = p.Type1 == p.Type2 ? SpriteUtil.Spriter.Transparent : TypeSpriteUtil.GetTypeSpriteWide(p.Type2, SAV.Generation);
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
        PersonalInfo7 => IsAlolanNative(s),
        PersonalInfo8SWSH ss => ss.IsInDex,
        PersonalInfo8BDSP bs => bs.IsInDex,
        PersonalInfo8LA bs => bs.IsPresentInGame,
        PersonalInfo9SV sv => sv.IsInDex,
        _ => true,
    };

    private static ReadOnlySpan<byte> PastGenAlolanNatives =>
    [
        0x00, 0x1C, 0xF8, 0x1E, 0xF8, 0xC7, 0xFC, 0xFF, 0x1F, 0x9F, 0x47, 0x7F, 0xF3, 0x13, 0xCA, 0xEF,
        0xFF, 0xD7, 0x38, 0x00, 0xE8, 0x7F, 0x0A, 0x46, 0xFE, 0x51, 0xD6, 0xCC, 0x1A, 0xCA, 0x47, 0x00,
        0x00, 0xC0, 0xFF, 0x18, 0x00, 0xCB, 0x38, 0xC0, 0xF3, 0x67, 0xB8, 0xEA, 0xA3, 0x46, 0xFE, 0x01,
        0x00, 0x00, 0x00, 0x0F, 0xCC, 0x6E, 0xC0, 0xF9, 0x1F, 0x7F, 0xEC, 0x54, 0x00, 0x00, 0x00, 0x1C,
        0x00, 0x70, 0x08, 0x00, 0xFC, 0xE3, 0xF2, 0x17, 0xF0, 0x09, 0x05, 0x20, 0x00, 0x6C, 0x79, 0x10,
        0x00, 0x00, 0xE0, 0x28, 0x1C, 0x40, 0xD7, 0xF5, 0x3F, 0x44,
    ];

    private static bool IsAlolanNative(ushort species)
    {
        if (species >= 721)
            return true;
        if (species == 720)
            return false;

        // [0, 719]; always will be a bit in the array.
        var offset = species >> 3;
        var bitSet = PastGenAlolanNatives;
        var bit = species & 7;
        if ((bitSet[offset] & (1 << bit)) != 0)
            return true;
        return false;
    }
}
