using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK1()
        {
            if (!(Entity is PK1 pk1))
                throw new FormatException(nameof(Entity));

            LoadMisc1(pk1);
            TID_Trainer.LoadIDValues(pk1);
            CR_PK1.LoadPK1(pk1);

            // Attempt to detect language
            CB_Language.SelectedValue = pk1.GuessedLanguage();

            LoadPartyStats(pk1);
            UpdateStats();
        }

        private PK1 PreparePK1()
        {
            if (!(Entity is PK1 pk1))
                throw new FormatException(nameof(Entity));

            SaveMisc1(pk1);

            SavePartyStats(pk1);
            pk1.FixMoves();
            pk1.RefreshChecksum();
            return pk1;
        }
    }
}
