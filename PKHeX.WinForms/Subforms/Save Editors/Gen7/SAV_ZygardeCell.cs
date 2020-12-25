using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_ZygardeCell : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV7 SAV;

        public SAV_ZygardeCell(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV7)(Origin = sav).Clone();

            // Constants @ 0x1C00
            // Cell Data @ 0x1D8C
            // Use constants 0x18C/2 = 198 thru +95
            ushort[] constants = SAV.GetEventConsts();
            ushort[] cells = constants.Skip(celloffset).Take(CellCount).ToArray();

            int cellCount = constants[cellstotal];
            int cellCollected = constants[cellscollected];

            NUD_Cells.Value = cellCount;
            NUD_Collected.Value = cellCollected;

            var combo = (DataGridViewComboBoxColumn)dgv.Columns[2];
            foreach (string t in states)
                combo.Items.Add(t); // add only the Names
            dgv.Columns[0].ValueType = typeof(int);

            // Populate Grid
            dgv.Rows.Add(CellCount);
            var locations = SAV is SAV7SM ? locationsSM : locationsUSUM;
            for (int i = 0; i < CellCount; i++)
            {
                if (cells[i] > 2)
                    throw new IndexOutOfRangeException("Unable to find cell index.");

                dgv.Rows[i].Cells[0].Value = (i+1);
                dgv.Rows[i].Cells[1].Value = locations[i];
                dgv.Rows[i].Cells[2].Value = states[cells[i]];
            }
        }

        private const int cellstotal = 161;
        private const int cellscollected = 169;
        private const int celloffset = 0xC6;
        private int CellCount => SAV is SAV7USUM ? 100 : 95;
        private readonly string[] states = {"None", "Available", "Received"};

        private void B_Save_Click(object sender, EventArgs e)
        {
            ushort[] constants = SAV.GetEventConsts();
            for (int i = 0; i < CellCount; i++)
            {
                string str = (string)dgv.Rows[i].Cells[2].Value;
                int val = Array.IndexOf(states, str);
                if (val < 0)
                    throw new IndexOutOfRangeException("Unable to find cell index.");

                constants[celloffset + i] = (ushort)val;
            }

            constants[cellstotal] = (ushort)NUD_Cells.Value;
            constants[cellscollected] = (ushort)NUD_Collected.Value;
            if (SAV is SAV7USUM)
                SAV.SetRecord(72, (int)NUD_Collected.Value);

            SAV.SetEventConsts(constants);
            Origin.CopyChangesFrom(SAV);

            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            int added = 0;
            for (int i = 0; i < dgv.RowCount; i++)
            {
                if (Array.IndexOf(states, (string)dgv.Rows[i].Cells[2].Value) != 2) // Not Collected
                    added++;
                dgv.Rows[i].Cells[2].Value = states[2];
            }

            NUD_Collected.Value += added;
            if (SAV is not SAV7USUM)
                NUD_Cells.Value += added;

            System.Media.SystemSounds.Asterisk.Play();
        }

        #region locations -- lazy

        private readonly string[] locationsSM =
        {
            "Verdant Cave - Trial Site",
            "Ruins of Conflict - Outside",
            "Route 1 (Day)",
            "Route 3",
            "Route 3 (Day)",
            "Kala'e Bay",
            "Hau'oli Cemetery",
            "Route 2",
            "Route 1 - Trainer School (Night)",
            "Hau'oli City - Shopping District",
            "Route 1 - Outskirts",
            "Hau'oli City - Shopping District (Night)",
            "Route 1",
            "Iki Town (Night)",
            "Route 4",
            "Paniola Ranch (Night)",
            "Paniola Ranch (Day)",
            "Wela Volcano Park - Top",
            "Lush Jungle - Cave",
            "Route 7",
            "Akala Outskirts",
            "Royal Avenue (Day)",
            "Royal Avenue (Night)",
            "Konikoni City (Night)",
            "Heahea City (Night)",
            "Route 8",
            "Route 8 (Day)",
            "Route 5",
            "Hano Beach (Day)",
            "Heahea City",
            "Diglett's Tunnel",
            "Hano Beach",
            "Malie Garden",
            "Malie City - Community Center (Night)",
            "Malie City (Day)",
            "Malie City - Outer Cape (Day)",
            "Route 11 (Night)",
            "Route 12 (Day)",
            "Route 12",
            "Secluded Shore (Night)",
            "Blush Mountain",
            "Route 13",
            "Haina Desert",
            "Ruins of Abundance - Outside",
            "Route 14",
            "Route 14 (Night)",
            "Tapu Village",
            "Route 15",
            "Aether House (Day)",
            "Ula'ula Meadow - Boardwalk",
            "Route 16 (Day)",
            "Ula'ula Meadow - Grass",
            "Route 17 - Building",
            "Route 17 - Ledge",
            "Po Town (Night)",
            "Route 10 (Day)",
            "Hokulani Observatory (Night)",
            "Mount Lanakila - Mountainside",
            "Mount Lanakila - High Mountainside",
            "Secluded Shore (Day)",
            "Route 13 (Night)",
            "Po Town (Day)",
            "Seafolk Village - Blue Food Boat",
            "Seafolk Village - Unbuilt House",
            "Poni Wilds (Day)",
            "Poni Wilds (Night)",
            "Poni Wilds",
            "Ancient Poni Path - Near Well (Day)",
            "Ancient Poni Path (Night)",
            "Poni Breaker Coast (Day)",
            "Ruins of Hope",
            "Poni Grove - Mountain Corner",
            "Poni Grove - Near a Bush",
            "Poni Plains (Day)",
            "Poni Plains (Night)",
            "Poni Plains",
            "Poni Meadow",
            "Poni Coast (Night)",
            "Poni Coast",
            "Poni Gauntlet - On Bridge",
            "Poni Gauntlet - Island w/ Trainer",
            "Resolution Cave - 1F (Day)",
            "Resolution Cave - B1F (Night)",
            "Vast Poni Canyon - 3F",
            "Vast Poni Canyon - 2F",
            "Vast Poni Canyon - Top",
            "Vast Poni Canyon - Inside",
            "Ancient Poni Path - Brickwall (Day)",
            "Poni Breaker Coast (Night)",
            "Resolution Cave - B1F",
            "Aether Foundation B2F - Right Hallway",
            "Aether Foundation 1F - Outside - Right Side",
            "Aether Foundation 1F - Outside (Day)",
            "Aether Foundation 1F - Entrance (Night)",
            "Aether Foundation 1F - Main Building",
        };

        private readonly string[] locationsUSUM =
        {
            "Hau'oli City (Shopping) - Salon (Outside)",
            "Hau'oli City (Shopping) - Malasada Shop (Outside)",
            "Hau'oli City (Shopping) - Ilima's House (2F)",
            "Malie City - Library (1F)",
            "Hau'oli City (Marina) - Pier",
            "Route 2 - Southeast House",
            "Hau'oli City (Shopping) - Ilima's House (Outside)",
            "Hau'oli City (Shopping) - City Hall",
            "Heahea City - Hotel (3F)",
            "Route 2 - Berry Fields House",
            "Route 2 - Berry Fields House (Outside)",
            "Royal Avenue - Northeast",
            "Hau'oli City (Shopping) - Pokemon Center (Outside)",
            "Royal Avenue - South",
            "Hokulani Observatory - Room",
            "Hokulani Observatory - Reception",
            "Hau'oli City (Shopping) - City Hall (Outside)",
            "Konikoni City - Olivia's Jewelry Shop (2F)",
            "Heahea City - Surfboard (Outside)",
            "Po Town - Southwest",
            "Hano Resort Lobby - Southwest Water",
            "Hau'oli City (Shopping) - Northwest of Police Station",
            "Hau'oli City (Marina) - Ferry Terminal (Outside)",
            "Route 2 - Southeast House (Outside)",
            "Route 2 - Pokemon Center (Outside)",
            "Heahea City - West",
            "Heahea City - Hotel West (Outside)",
            "Heahea City - Hotel East (Outside)",
            "Heahea City - Research Lab East (Outside)",
            "Heahea City - Research Lab South (Outside)",
            "Heahea City - Game Freak",
            "Hokulani Observatory - Dead End",
            "Heahea City - Game Freak Building (3F)",
            "Heahea City - Research Lab",
            "Heahea City - Hotel (1F)",
            "Battle Royal Dome - 2F",
            "Paniola Town - West",
            "Paniola Town - Kiawe's House (1F)",
            "Paniola Town - Kiawe's House (2F)",
            "Paniola Ranch - Northwest",
            "Paniola Ranch - Southeast",
            "Hano Beach",
            "Hano Resort - South",
            "Hano Resort - North",
            "Konikoni City Lighthouse (Through Diglett's Tunnel)",
            "Battle Royal Dome - 1F",
            "Route 8 - Aether Base (Outside)",
            "Route 8 - Fossil Restoration Center (Outside)",
            "Konikoni City - West",
            "Konikoni City - Restaurant (1F)",
            "Iki Town - Southwest",
            "Hau'oli City (Shopping) - Ilima's House Pool",
            "Wela Volcano Park - Rocks Behind Sign",
            "Route 5 - South of Pokemon Center",
            "Hano Beach - Below Sandygast",
            "Malie City (Outer Cape) - Recycling Plant (Outside)",
            "Malie City - Ferry Terminal (Outside)",
            "Malie City - Apparel Shop (Outside)",
            "Malie City - Salon (Outside)",
            "Route 16 - Aether Base (Outside)",
            "Blush Mountain - Power Plant (Outside)",
            "Malie City - Library (2F)",
            "Malie Garden - Northeast",
            "Malie City - CommunityCenter",
            "Hokulani Observatory - Outside",
            "Mount Hokulani",
            "Blush Mountain - Power Plant",
            "Route 13",
            "Route 14 - Front of Abandoned Megamart",
            "Route 14 - North",
            "Route 15 - Islet Surfboard (Outside)",
            "Route 17 - Police Station (Outside)",
            "Route 17 - Police Station",
            "Po Town - Pokemon Center (Outside)",
            "Exeggutor Island - Under Rock",
            "Po Town - Shady House East (Outside)",
            "Po Town - Pokemon Center",
            "Po Town - Shady House (1F)",
            "Route 13 - Motel (Outside)",
            "Po Town - Shady House 2F (Outside)",
            "Route 17 - South of Po Town",
            "Ula'ula Meadow",
            "Po Town - Shady House West Rocks (Outside) 1",
            "Po Town - Shady House West Rocks (Outside) 2",
            "Po Town - Shady House West Rocks (Outside) 3",
            "Seafolk Village - Southeast Whiscash (Mina's Ship) (Outside)",
            "Seafolk Village - Southwest Huntail",
            "Seafolk Village - Southwest Huntail (Outside)",
            "Seafolk Village - Southeast Whiscash (Mina's Ship)",
            "Seafolk Village - West Wailord (Restaurant)",
            "Seafolk Village - East Steelix",
            "Poni Wilds - Southeast",
            "Ancient Poni Path - Hapu's House (Kitchen)",
            "Seafolk Village - Northeast",
            "Ancient Poni Path - Hapu's House (Bedroom)",
            "Ancient Poni Path - Southwest",
            "Ancient Poni Path - Hapu's House (Courtyard)",
            "Ancient Poni Path - Hapu's House (Outside Behind Well)",
            "Ancient Poni Path - Northeast",
            "Battle Tree - Entrance",
        };

        #endregion
    }
}
