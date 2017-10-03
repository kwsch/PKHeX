using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK3()
        {
            if (!(pkm is PK3 pk3))
                return;

            LoadMisc1(pk3);
            LoadMisc2(pk3);
            LoadMisc3(pk3);

            CB_Ability.SelectedIndex = pk3.AbilityBit && CB_Ability.Items.Count > 1 ? 1 : 0;

            LoadPartyStats(pk3);
            UpdateStats();
        }
        private void PopulateFieldsCK3()
        {
            if (!(pkm is CK3 ck3))
                return;

            LoadMisc1(ck3);
            LoadMisc2(ck3);
            LoadMisc3(ck3);

            int abil = ck3.AbilityNumber >> 1;
            CB_Ability.SelectedIndex = abil > CB_Ability.Items.Count ? 0 : abil;
            LoadShadow3(ck3);

            LoadPartyStats(ck3);
            UpdateStats();
        }
        private void PopulateFieldsXK3()
        {
            if (!(pkm is XK3 xk3))
                return;

            LoadMisc1(xk3);
            LoadMisc2(xk3);
            LoadMisc3(xk3);

            int abil = xk3.AbilityNumber >> 1;
            CB_Ability.SelectedIndex = abil > CB_Ability.Items.Count ? 0 : abil;
            LoadShadow3(xk3);

            LoadPartyStats(xk3);
            UpdateStats();
        }

        private PKM PreparePK3()
        {
            if (!(pkm is PK3 pk3))
                return null;

            SaveMisc1(pk3);
            SaveMisc2(pk3);
            SaveMisc3(pk3);

            pk3.AbilityNumber = 1 << CB_Ability.SelectedIndex;

            SavePartyStats(pk3);
            pk3.FixMoves();
            pk3.RefreshChecksum();
            return pk3;
        }
        private PKM PrepareCK3()
        {
            if (!(pkm is CK3 ck3))
                return null;

            SaveMisc1(ck3);
            SaveMisc2(ck3);
            SaveMisc3(ck3);

            ck3.AbilityNumber = 1 << CB_Ability.SelectedIndex; // to match gen6+
            SaveShadow3(ck3);

            SavePartyStats(ck3);
            ck3.FixMoves();
            ck3.RefreshChecksum();
            return ck3;
        }
        private PKM PrepareXK3()
        {
            if (!(pkm is XK3 xk3))
                return null;

            SaveMisc1(xk3);
            SaveMisc2(xk3);
            SaveMisc3(xk3);

            xk3.AbilityNumber = 1 << CB_Ability.SelectedIndex; // to match gen6+
            SaveShadow3(xk3);

            SavePartyStats(xk3);
            xk3.FixMoves();
            xk3.RefreshChecksum();
            return xk3;
        }
    }
}
