using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK6()
        {
            if (Entity is not PK6 pk6)
                throw new FormatException(nameof(Entity));

            LoadMisc1(pk6);
            LoadMisc2(pk6);
            LoadMisc3(pk6);
            LoadMisc4(pk6);
            LoadMisc6(pk6);

            CB_EncounterType.SelectedValue = pk6.Gen4 ? pk6.EncounterType : 0;
            CB_EncounterType.Visible = Label_EncounterType.Visible = pk6.Gen4;

            LoadPartyStats(pk6);
            UpdateStats();
        }

        private PK6 PreparePK6()
        {
            if (Entity is not PK6 pk6)
                throw new FormatException(nameof(Entity));

            SaveMisc1(pk6);
            SaveMisc2(pk6);
            SaveMisc3(pk6);
            SaveMisc4(pk6);
            SaveMisc6(pk6);
            CheckTransferPIDValid(pk6);

            pk6.EncounterType = WinFormsUtil.GetIndex(CB_EncounterType);

            // Toss in Party Stats
            SavePartyStats(pk6);

            // Unneeded Party Stats (Status, Flags, Unused)
            pk6.Data[0xE8] = pk6.Data[0xE9] = pk6.Data[0xEA] = pk6.Data[0xEB] =
                pk6.Data[0xEF] =
                pk6.Data[0xFE] = pk6.Data[0xFF] = pk6.Data[0x100] =
                pk6.Data[0x101] = pk6.Data[0x102] = pk6.Data[0x103] = 0;

            pk6.FixMoves();
            pk6.FixRelearn();
            if (ModifyPKM)
                pk6.FixMemories();
            pk6.RefreshChecksum();
            return pk6;
        }
    }
}
