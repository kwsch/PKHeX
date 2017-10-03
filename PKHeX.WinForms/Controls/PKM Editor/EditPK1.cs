using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK1()
        {
            if (!(pkm is PK1 pk1))
                return;

            LoadMisc1(pk1);

            // Attempt to detect language
            if (pk1.Japanese)
                CB_Language.SelectedIndex = 0;
            else
            {
                int lang = PKX.GetSpeciesNameLanguage(pk1.Species, pk1.Nickname, 2);
                CB_Language.SelectedValue = lang > 0 ? lang : 2;
            }

            LoadPartyStats(pk1);
            UpdateStats();
        }

        private PKM PreparePK1()
        {
            if (!(pkm is PK1 pk1))
                return null;

            SaveMisc1(pk1);

            SavePartyStats(pk1);
            pk1.FixMoves();
            pk1.RefreshChecksum();
            return pk1;
        }
    }
}
