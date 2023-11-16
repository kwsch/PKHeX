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
        PersonalInfo7 => s > 721 || PastGenAlolanNatives.BinarySearch(s) >= 0,
        PersonalInfo8SWSH ss => ss.IsInDex,
        PersonalInfo8BDSP bs => bs.IsInDex,
        PersonalInfo8LA bs => bs.IsPresentInGame,
        PersonalInfo9SV sv => sv.IsInDex,
        _ => true,
    };

    private static ReadOnlySpan<ushort> PastGenAlolanNatives =>
    [
        010, 011, 012, 019, 020, 021, 022, 023, 025, 026, 027, 028, 035, 036, 037, 038, 039, 040, 041, 042, 046, 047,
        050, 051, 052, 053, 054, 055, 056, 057, 058, 059, 060, 061, 062, 063, 064, 065, 066, 067, 068, 072, 073, 074,
        075, 076, 079, 080, 081, 082, 086, 088, 089, 090, 091, 092, 093, 094, 096, 097, 100, 101, 102, 103, 104, 105,
        108, 113, 115, 118, 119, 120, 121, 122, 123, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137,
        138, 140, 142, 143, 147, 148, 149, 163, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 177, 179, 185, 186,
        190, 193, 194, 195, 196, 197, 198, 199, 200, 204, 206, 209, 210, 212, 214, 215, 218, 219, 222, 223, 225, 227,
        228, 233, 235, 238, 239, 240, 241, 242, 246, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 283, 284, 296,
        297, 299, 302, 303, 307, 308, 309, 318, 319, 320, 321, 324, 325, 326, 327, 328, 329, 330, 333, 334, 339, 340,
        341, 343, 345, 347, 349, 350, 351, 352, 353, 357, 359, 361, 362, 366, 369, 370, 371, 372, 373, 374, 375, 376,
        408, 409, 410, 411, 418, 419, 422, 423, 425, 426, 427, 429, 430, 438, 439, 440, 443, 444, 445, 446, 447, 448,
        449, 450, 451, 452, 456, 457, 458, 459, 460, 461, 462, 466, 467, 469, 470, 471, 474, 476, 478, 506, 507, 508,
        524, 525, 526, 531, 546, 547, 548, 549, 550, 551, 552, 553, 557, 558, 559, 561, 564, 565, 566, 567, 568, 569,
        570, 572, 580, 581, 582, 583, 584, 587, 592, 594, 605, 618, 619, 621, 622, 624, 627, 628, 629, 630, 636, 661,
        662, 663, 667, 669, 674, 675, 676, 686, 688, 689, 690, 692, 694, 695, 696, 698, 700, 701, 702, 703, 704, 705,
        706, 707, 708, 709, 714, 718,
    ];
}
