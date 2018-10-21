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
            TID_Trainer.LoadIDValues(pkm);
            CR_PK1.LoadPK1(pk1);

            // Attempt to detect language
            CB_Language.SelectedValue = PKX.GetVCLanguage(pk1);

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
