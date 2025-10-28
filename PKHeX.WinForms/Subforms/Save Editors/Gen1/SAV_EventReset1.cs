using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_EventReset1 : Form
{
    private readonly G1OverworldSpawner Overworld;
    private void SAV_EventReset1_FormClosing(object sender, FormClosingEventArgs e) => Overworld.Save();

    public SAV_EventReset1(SAV1 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        Overworld = new G1OverworldSpawner(sav);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        var pairs = Overworld.GetFlagPairs().OrderBy(z => z.Name);
        foreach (var pair in pairs)
        {
            var name = pair.Name.AsSpan(G1OverworldSpawner.FlagPropertyPrefix.Length);
            var index = name.IndexOf('_');
            var specName = index == -1 ? name : name[..index];

            // convert species name to current localization language
            SpeciesName.TryGetSpecies(specName, (int)LanguageID.English, out var species);
            var localized = GameInfo.Strings.specieslist[species];
            if (index != -1) // assume there is a suffix after the underscore; throw exception when not
                localized += $" {name[(index + 1)..]}";

            var b = new Button
            {
                Text = localized, Enabled = pair.IsHidden,
                Size = new Size((Width / 2) - 25, 22),
            };
            b.Click += (_, _) =>
            {
                pair.Reset();
                b.Enabled = false;
                WinFormsUtil.Alert("Reset!");
            };

            FLP_List.Controls.Add(b);
        }
    }
}
