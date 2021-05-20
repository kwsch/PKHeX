using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_EventReset1 : Form
    {
        private readonly G1OverworldSpawner Overworld;
        private void SAV_EventReset1_FormClosing(object sender, FormClosingEventArgs e) => Overworld.Save();

        public SAV_EventReset1(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            Overworld = new G1OverworldSpawner((SAV1)sav);

            InitializeButtons();
        }

        private void InitializeButtons()
        {
            var pairs = Overworld.GetFlagPairs().OrderBy(z => z.Name);
            foreach (var pair in pairs)
            {
                var split = pair.Name.Split('_');
                var specName = split[0][G1OverworldSpawner.FlagPropertyPrefix.Length..];

                // convert species name to current localization language
                int species = SpeciesName.GetSpeciesID(specName);
                var pkmname = GameInfo.Strings.specieslist[species];

                if (split.Length != 1)
                    pkmname += $" {split[1]}";
                var b = new Button
                {
                    Text = pkmname, Enabled = pair.IsHidden,
                    Size = new Size((Width / 2) - 25, 22),
                };
                b.Click += (s, e) =>
                {
                    pair.Reset();
                    b.Enabled = false;
                    WinFormsUtil.Alert("Reset!");
                };

                FLP_List.Controls.Add(b);
            }
        }
    }
}
