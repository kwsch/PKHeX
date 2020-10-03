using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK2()
        {
            if (!(Entity is GBPKM pk2) || !(Entity is ICaughtData2 c2))
                throw new FormatException(nameof(Entity));

            LoadMisc1(pk2);
            LoadMisc2(pk2);

            TID_Trainer.LoadIDValues(pk2);
            TB_MetLevel.Text = c2.Met_Level.ToString();
            CB_MetLocation.SelectedValue = c2.Met_Location;
            CB_MetTimeOfDay.SelectedIndex = c2.Met_TimeOfDay;

            // Attempt to detect language
            CB_Language.SelectedValue = pk2.GuessedLanguage();

            LoadPartyStats(pk2);
            UpdateStats();
        }

        private GBPKM PreparePK2()
        {
            if (!(Entity is GBPKM pk2) || !(Entity is ICaughtData2 c2))
                throw new FormatException(nameof(Entity));

            SaveMisc1(pk2);
            SaveMisc2(pk2);

            c2.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            c2.Met_Location = WinFormsUtil.GetIndex(CB_MetLocation);
            c2.Met_TimeOfDay = CB_MetTimeOfDay.SelectedIndex;

            SavePartyStats(pk2);
            pk2.FixMoves();
            return pk2;
        }
    }
}
