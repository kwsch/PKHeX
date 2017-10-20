using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK2()
        {
            if (!(pkm is PK2 pk2))
                return;

            LoadMisc1(pk2);
            LoadMisc2(pk2);

            TB_MetLevel.Text = pk2.Met_Level.ToString();
            CB_MetLocation.SelectedValue = pk2.Met_Location;
            CB_MetTimeOfDay.SelectedIndex = pk2.Met_TimeOfDay;

            // Attempt to detect language
            if (pk2.Japanese)
                CB_Language.SelectedValue = 1;
            else if (pk2.Korean)
                CB_Language.SelectedValue = 8;
            else
            {
                int lang = PKX.GetSpeciesNameLanguage(pk2.Species, pk2.Nickname, 2);
                CB_Language.SelectedValue = lang > 0 ? lang : 2;
            }

            LoadPartyStats(pk2);
            UpdateStats();
        }

        private PKM PreparePK2()
        {
            if (!(pkm is PK2 pk2))
                return null;

            SaveMisc1(pk2);
            SaveMisc2(pk2);

            pk2.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk2.Met_Location = WinFormsUtil.GetIndex(CB_MetLocation);
            pk2.Met_TimeOfDay = CB_MetTimeOfDay.SelectedIndex;

            SavePartyStats(pk2);
            pk2.FixMoves();
            return pk2;
        }
    }
}
