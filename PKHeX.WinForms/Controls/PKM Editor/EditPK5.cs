using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK5()
        {
            if (Entity is not PK5 pk5)
                throw new FormatException(nameof(Entity));

            LoadMisc1(pk5);
            LoadMisc2(pk5);
            LoadMisc3(pk5);
            LoadMisc4(pk5);
            CB_EncounterType.SelectedValue = pk5.Gen4 ? pk5.EncounterType : 0;
            CB_EncounterType.Visible = Label_EncounterType.Visible = pk5.Gen4;
            CHK_NSparkle.Checked = pk5.NPokémon;

            if (HaX)
                DEV_Ability.SelectedValue = pk5.Ability;
            else if (pk5.HiddenAbility)
                CB_Ability.SelectedIndex = CB_Ability.Items.Count - 1;
            else
                LoadAbility4(pk5);

            LoadPartyStats(pk5);
            UpdateStats();
        }

        private PK5 PreparePK5()
        {
            if (Entity is not PK5 pk5)
                throw new FormatException(nameof(Entity));

            SaveMisc1(pk5);
            SaveMisc2(pk5);
            SaveMisc3(pk5);
            SaveMisc4(pk5);

            pk5.EncounterType = WinFormsUtil.GetIndex(CB_EncounterType);
            pk5.NPokémon = CHK_NSparkle.Checked;
            if (!HaX) // specify via extra 0x42 instead
                pk5.HiddenAbility = CB_Ability.SelectedIndex > 1; // not 0 or 1

            SavePartyStats(pk5);
            pk5.FixMoves();
            pk5.RefreshChecksum();
            return pk5;
        }
    }
}
