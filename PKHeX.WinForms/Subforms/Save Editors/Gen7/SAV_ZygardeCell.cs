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
            SAV = (SAV7)(Origin = sav).Clone();
            InitializeComponent();

            // Constants @ 0x1C00
            // Cell Data @ 0x1D8C
            // Use constants 0x18C/2 = 198 thru +95
            ushort[] constants = SAV.EventConsts;
            ushort[] cells = constants.Skip(celloffset).Take(cellcount).ToArray();
            
            int cellCount = constants[cellstotal];
            int cellCollected = constants[cellscollected];

            NUD_Cells.Value = cellCount;
            NUD_Collected.Value = cellCollected;

            var combo = dgv.Columns[2] as DataGridViewComboBoxColumn;
            foreach (string t in states)
                combo.Items.Add(t); // add only the Names

            // Populate Grid
            dgv.Rows.Add(cellcount);
            for (int i = 0; i < cellcount; i++)
            {
                if (cells[i] > 2)
                    throw new ArgumentException();

                dgv.Rows[i].Cells[0].Value = (i+1).ToString();
                dgv.Rows[i].Cells[1].Value = locations[i];
                dgv.Rows[i].Cells[2].Value = states[cells[i]];
            }
        }

        private const int cellstotal = 0x142/2;
        private const int cellscollected = 0x152/2;
        private const int celloffset = 198;
        private const int cellcount = 95;
        private readonly string[] states = {"None", "Available", "Received"};

        private void B_Save_Click(object sender, EventArgs e)
        {
            ushort[] constants = SAV.EventConsts;
            for (int i = 0; i < cellcount; i++)
            {
                string str = (string)dgv.Rows[i].Cells[2].Value;
                int val = Array.IndexOf(states, str);
                if (val < 0)
                    throw new ArgumentException();

                constants[celloffset + i] = (ushort)val;
            }

            constants[cellstotal] = (ushort)NUD_Cells.Value;
            constants[cellscollected] = (ushort)NUD_Collected.Value;

            SAV.EventConsts = constants;
            Origin.setData(SAV.Data, 0);

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
            NUD_Cells.Value += added;

            System.Media.SystemSounds.Asterisk.Play();
        }

        #region locations -- lazy

        private readonly string[] locations =
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

        #endregion
    }
}
