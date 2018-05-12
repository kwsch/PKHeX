using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK4()
        {
            var pk4 = pkm;
            if (pk4?.Format != 4)
                return;

            LoadMisc1(pk4);
            LoadMisc2(pk4);
            LoadMisc3(pk4);
            LoadMisc4(pk4);

            CB_EncounterType.SelectedValue = pk4.Gen4 ? pk4.EncounterType : 0;
            CB_EncounterType.Visible = Label_EncounterType.Visible = pkm.Gen4;

            if (HaX)
                DEV_Ability.SelectedValue = pk4.Ability;
            else
                LoadAbility4(pk4);

            // Minor properties
            switch (pk4)
            {
                case PK4 p4: ShinyLeaf.Value = p4.ShinyLeaf;
                    break;
                case BK4 b4: ShinyLeaf.Value = b4.ShinyLeaf;
                    break;
            }

            LoadPartyStats(pk4);
            UpdateStats();
        }

        private PKM PreparePK4()
        {
            var pk4 = pkm;
            if (pk4?.Format != 4)
                return null;

            SaveMisc1(pk4);
            SaveMisc2(pk4);
            SaveMisc3(pk4);
            SaveMisc4(pk4);

            pk4.EncounterType = WinFormsUtil.GetIndex(CB_EncounterType);

            // Minor properties
            switch (pk4)
            {
                case PK4 p4:
                    p4.ShinyLeaf = ShinyLeaf.Value;
                    break;
                case BK4 b4:
                    b4.ShinyLeaf = ShinyLeaf.Value;
                    break;
            }

            SavePartyStats(pk4);
            pk4.FixMoves();
            pk4.RefreshChecksum();
            return pk4;
        }
    }
}
