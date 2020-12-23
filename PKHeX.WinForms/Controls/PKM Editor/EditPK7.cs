using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK7()
        {
            if (Entity is not PK7 pk7)
                throw new FormatException(nameof(Entity));

            LoadMisc1(pk7);
            LoadMisc2(pk7);
            LoadMisc3(pk7);
            LoadMisc4(pk7);
            LoadMisc6(pk7);

            LoadPartyStats(pk7);
            UpdateStats();
        }

        private PK7 PreparePK7()
        {
            if (Entity is not PK7 pk7)
                throw new FormatException(nameof(Entity));

            SaveMisc1(pk7);
            SaveMisc2(pk7);
            SaveMisc3(pk7);
            SaveMisc4(pk7);
            SaveMisc6(pk7);
            CheckTransferPIDValid(pk7);

            // Toss in Party Stats
            SavePartyStats(pk7);

            // Unneeded Party Stats (Status, Flags, Unused)
            pk7.Data[0xE8] = pk7.Data[0xE9] = pk7.Data[0xEA] = pk7.Data[0xEB] =
                pk7.Data[0xED] = pk7.Data[0xEE] = pk7.Data[0xEF] =
                pk7.Data[0xFE] = pk7.Data[0xFF] = pk7.Data[0x100] =
                pk7.Data[0x101] = pk7.Data[0x102] = pk7.Data[0x103] = 0;

            pk7.FixMoves();
            pk7.FixRelearn();
            if (ModifyPKM)
                pk7.FixMemories();
            pk7.RefreshChecksum();
            return pk7;
        }

        private void PopulateFieldsPB7()
        {
            if (Entity is not PB7 pk7)
                throw new FormatException(nameof(Entity));

            LoadMisc1(pk7);
            LoadMisc2(pk7);
            LoadMisc3(pk7);
            LoadMisc4(pk7);
            LoadMisc6(pk7);
            LoadAVs(pk7);
            SizeCP.LoadPKM(pk7);

            LoadPartyStats(pk7);
            UpdateStats();
        }

        private PB7 PreparePB7()
        {
            if (Entity is not PB7 pk7)
                throw new FormatException(nameof(Entity));

            SaveMisc1(pk7);
            SaveMisc2(pk7);
            SaveMisc3(pk7);
            SaveMisc4(pk7);
            SaveMisc6(pk7);

            // Toss in Party Stats
            SavePartyStats(pk7);

            if (pk7.Stat_CP == 0)
                pk7.ResetCP();

            // heal values to original
            pk7.FieldEventFatigue1 = pk7.FieldEventFatigue2 = 100;

            pk7.FixMoves();
            pk7.FixRelearn();
            if (ModifyPKM)
                pk7.FixMemories();
            pk7.RefreshChecksum();
            return pk7;
        }
    }
}
