using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Trainer : Form
    {
        private readonly SAV6 SAV = new SAV6(Main.SAV.Data);
        public SAV_Trainer()
        {
            InitializeComponent();
            if (Main.unicode)
            try { TB_OTName.Font = FontUtil.getPKXFont(11); }
            catch (Exception e) { WinFormsUtil.Alert("Font loading failed...", e.ToString()); }

            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
            B_MaxCash.Click += (sender, e) => MT_Money.Text = "9,999,999";

            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.gendersymbols.Take(2).ToArray()); // m/f depending on unicode selection

            MaisonRecords = new[]
            {
                TB_MCSN,TB_MCSS,TB_MBSN,TB_MBSS,
                TB_MCDN,TB_MCDS,TB_MBDN,TB_MBDS,
                TB_MCTN,TB_MCTS,TB_MBTN,TB_MBTS,
                TB_MCRN,TB_MCRS,TB_MBRN,TB_MBRS,
                TB_MCMN,TB_MCMS,TB_MBMN,TB_MBMS,
            };
            cba = new[] { CHK_Badge1, CHK_Badge2, CHK_Badge3, CHK_Badge4, CHK_Badge5, CHK_Badge6, CHK_Badge7, CHK_Badge8, };
            pba = new [] { PB_Badge1, PB_Badge2, PB_Badge3, PB_Badge4, PB_Badge5, PB_Badge6, PB_Badge7, PB_Badge8, };

            L_MultiplayerSprite.Enabled = CB_MultiplayerSprite.Enabled = Main.SAV.ORAS;
            L_Style.Visible = TB_Style.Visible = SAV.XY;
            if (!SAV.XY)
                TC_Editor.TabPages.Remove(Tab_Appearance);
            if (SAV.ORASDEMO)
                TC_Editor.TabPages.Remove(Tab_Multiplayer);
            if (SAV.MaisonStats < 0)
                TC_Editor.TabPages.Remove(Tab_Maison);

            editing = true;
            getComboBoxes();
            getTextBoxes();
            getBadges();

            statdata = new[] {
                "0x000",	"0x000", // Steps taken?
                "0x004",	"0x004", // Minutes Played / Pokemon Encountered?
                "0x008",	"0x008",
                "0x00C",	"0x00C",
                "0x010",	"0x010",
                "0x014",	"0x014",
                "0x018",	"0x018",
                "0x01C",	"Pokémon Captured",
                "0x020",	"0x020",
                "0x024",	"Eggs Hatched",
                "0x028",	"Pokémon Evolved",
                "0x02C",	"0x02C",
                "0x030",	"~People Passed", // I think the following ones are Passerby actions...
                "0x034",	"0x034",
                "0x038",	"0x038",
                "0x03C",	"0x03C",
                "0x040",	"Link Trades",
                "0x044",	"Link Battles",
                "0x048",	"Link Battle Wins",
                "0x04C",	"0x04C",
                "0x050",	"0x050",
                "0x054",	"0x054",
                "0x058",	"0x058",
                "0x05C",	"0x05C",
                "0x060",	"0x060",
                "0x064",	"0x064",
                "0x068",	"0x068",
                "0x06C",	"0x06C",
                "0x070",	"0x070",
                "0x074",	"0x074",
                "0x078",	"0x078",
                "0x07C",	"0x07C",
                "0x080",	"0x080",
                "0x084",	"0x084",
                "0x088",	"BP Earned",
                "0x08C",	"0x08C",
                "0x090",	"0x090",
                "0x094",	"0x094",
                "0x098",	"0x098",
                "0x09C",	"0x09C",
                "0x0A0",	"0x0A0",
                "0x0A4",	"0x0A4",
                "0x0A8",	"0x0A8",
                "0x0AC",	"0x0AC",
                "0x0B0",	"0x0B0",
                "0x0B4",	"0x0B4",
                "0x0B8",	"0x0B8",
                "0x0BC",	"0x0BC",
                "0x0C0",	"0x0C0",
                "0x0C4",	"0x0C4",
                "0x0C8",	"0x0C8",
                "0x0CC",	"0x0CC",
                "0x0D0",	"0x0D0",
                "0x0D4",	"0x0D4",
                "0x0D8",	"0x0D8",
                "0x0DC",	"0x0DC",
                "0x0E0",	"0x0E0",
                "0x0E4",	"0x0E4",
                "0x0E8",	"0x0E8",
                "0x0EC",	"Nice! Received",
                "0x0F0",	"Birthday Wishes",
                "0x0F4",	"Total People Met Online",
                "0x0F8",	"0x0F8",
                //"0x0FC",	"Current Pokemiles",
                "0x100",	"Obtained Pokemiles",
                "0x104",	"0x104",
                "0x108",	"0x108",
                "0x10C",	"Super Training Clears",
                "0x110",	"Judge Evaluations",
                "0x114",	"0x114",
                "0x118",	"0x118", // Link Trades?
                "0x11C",	"Link Battle", // Wins", // ?
                "0x120",	"0x120", // Link Battle Losses?
                "0x124",	"0x124",
                "0x128",	"0x128",
                "0x12C",	"0x12C",
                "0x130",	"0x130",
                "0x134",	"0x134",
                "0x138",	"0x138",
                "0x13C",	"0x13C",
                "0x140",	"Flags Captured",
                "0x144",	"0x144",
                "0x148",	"0x148",
                "0x14C",	"0x14C",
                "0x150",	"0x150",
                "0x154",	"0x154",
                "0x158",	"0x158",
                "0x15C",	"0x15C",
                "0x160",	"0x160",
                "0x164",	"0x164",
                "0x168",	"0x168",
                "0x16C",	"0x16C",
                "0x170",	"0x170",
                "0x174",	"0x174",
                "0x178",	"0x178",
                "0x17C",	"0x17C",
                "0x180",	"0x180",
                "0x184",	"0x184",
                "0x188",	"0x188",
                "0x18C",	"0x18C",
                "0x190",	"0x190",
                "0x194",	"0x194",
                "0x198",	"0x198",
                "0x19C",	"0x19C",
                "0x1A0",	"0x1A0",
                "0x1A4",	"0x1A4",
                "0x1A8",	"0x1A8",
                "0x1AC",	"0x1AC",
                "0x1B0",	"0x1B0",
                "0x1B4",	"0x1B4",
                "0x1B8",	"0x1B8",
                "0x1BC",	"Battle Tests",
                "0x1C0",	"0x1C0",
                "0x1C4",	"0x1C4",
                "0x1C8",	"0x1C8",
                "0x1CC",	"0x1CC",
                "0x1D0",	"0x1D0",
                "0x1D4",	"0x1D4",
                "0x1D8",	"0x1D8",
                "0x1DC",	"0x1DC",
                "0x1E0",	"0x1E0",
                "0x1E4",	"0x1E4",
                "0x1E8",	"0x1E8",
                "0x1EC",	"0x1EC",
                "0x1F0",	"0x1F0",
                "0x1F4",	"0x1F4",
                "0x1F8",	"0x1F8",
                "0x1FC",	"0x1FC",
                "0x200",	"0x200",
                "0x204",	"0x204",
                "0x208",	"0x208",
                "0x20C",	"0x20C",
                "0x210",	"0x210",
                "0x214",	"0x214",
                "0x218",	"0x218",
                "0x21C",	"0x21C",
                "0x220",	"0x220",
                "0x224",	"0x224",
                "0x228",	"0x228",
                "0x22C",	"0x22C",
                "0x230",	"0x230",
                "0x234",	"0x234",
                "0x238",	"0x238",
                "0x23C",	"0x23C",
                "0x240",	"0x240",
                "0x244",	"0x244",
                "0x248",	"0x248",
                "0x24C",	"0x24C",
                "0x250",	"0x250",
                "0x254",	"0x254",
                "0x258",	"0x258",
            }; // Offset, Title. Horrible implementation, but works.

            CB_Stats.Items.Clear();
            for (int i = 0; i < statdata.Length / 2; i++)
                CB_Stats.Items.Add(statdata[2 * i + 1]);
            CB_Stats.SelectedIndex = 0;
        }
        private readonly string[] statdata;
        private bool editing;
        private readonly ToolTip Tip1 = new ToolTip(), Tip2 = new ToolTip();
        private readonly MaskedTextBox[] MaisonRecords;
        private readonly CheckBox[] cba;
        private readonly PictureBox[] pba;

        private void getComboBoxes()
        {
            var dsregion_list = new[] {
                    new { Text = "NA/SA", Value = 1 },
                    new { Text = "EUR", Value = 2 },
                    new { Text = "JPN", Value = 0 },
                    new { Text = "CN", Value = 4 },
                    new { Text = "KOR", Value = 5 },
                    new { Text = "TW", Value = 6 }
                };

            var language_list = new[] {
                    new { Text = "ENG", Value = 2 },
                    new { Text = "JPN", Value = 1 },
                    new { Text = "FRE", Value = 3 },
                    new { Text = "ITA", Value = 4 },
                    new { Text = "GER", Value = 5 },
                    new { Text = "SPA", Value = 7 },
                    new { Text = "KOR", Value = 8 }
                };

            CB_3DSReg.DisplayMember = "Text";
            CB_3DSReg.ValueMember = "Value";
            CB_3DSReg.DataSource = dsregion_list;
            CB_Language.DisplayMember = "Text";
            CB_Language.ValueMember = "Value";
            CB_Language.DataSource = language_list;

            CB_Country.DisplayMember = "Text";
            CB_Country.ValueMember = "Value";
            CB_Region.DisplayMember = "Text";
            CB_Region.ValueMember = "Value";
            Main.setCountrySubRegion(CB_Country, "countries");

            var oras_sprite_list = new[] {
              //new { Text = "Calem",                       Value = 00 },
              //new { Text = "Serena",                      Value = 01 },
                new { Text = "Sycamore",                    Value = 02 },
                new { Text = "Diantha",                     Value = 03 },
                new { Text = "Wikstrom",                    Value = 04 },
                new { Text = "Malva",                       Value = 05 },
                new { Text = "Drasna",                      Value = 06 },
                new { Text = "Siebold",                     Value = 07 },
                new { Text = "Viola",                       Value = 08 },
                new { Text = "Grant",                       Value = 09 },
                new { Text = "Korrina",                     Value = 10 },
                new { Text = "Ramos",                       Value = 11 },
                new { Text = "Clemont",                     Value = 12 },
                new { Text = "Valerie",                     Value = 13 },
                new { Text = "Olympia",                     Value = 14 },
                new { Text = "Wulfric",                     Value = 15 },
                new { Text = "Youngster (XY)",              Value = 16 },
              //new { Text = "(None)",                      Value = 17 },
                new { Text = "Lass (XY)",                   Value = 18 },
                new { Text = "Lady (XY)",                   Value = 19 },
                new { Text = "Schoolgirl (XY)",             Value = 20 },
                new { Text = "Battle Girl (XY)",            Value = 21 },
                new { Text = "Schoolboy (XY)",              Value = 22 },
                new { Text = "Rich Boy (XY)",               Value = 23 },
                new { Text = "Female Ace Trainer (XY)",     Value = 24 },
              //new { Text = "(None)",                      Value = 25 },
                new { Text = "Female Ranger (XY)",          Value = 26 },
                new { Text = "Male Ace Trainer (XY)",       Value = 27 },
                new { Text = "Male Ranger (XY)",            Value = 28 },
                new { Text = "Madame",                      Value = 29 },
                new { Text = "Monsieur",                    Value = 30 },
                new { Text = "Black Belt (XY)",             Value = 31 },
                new { Text = "Male Punk (XY)",              Value = 32 },
                new { Text = "Fairy Tale Girl (XY)",        Value = 33 },
                new { Text = "Shauna",                      Value = 34 },
                new { Text = "Tierno",                      Value = 35 },
                new { Text = "Trevor",                      Value = 36 },
                new { Text = "Brendan",                     Value = 37 },
                new { Text = "May",                         Value = 38 },
              //new { Text = "(None)",                      Value = 39 },
                new { Text = "Hiker",                       Value = 40 },
                new { Text = "Aroma Lady",                  Value = 41 },
                new { Text = "Male Schoolkid",              Value = 42 },
                new { Text = "Female Schoolkid",            Value = 43 },
                new { Text = "Black Belt (ORAS)",           Value = 44 },
                new { Text = "Battle Girl (ORAS)",          Value = 45 },
                new { Text = "Pokemaniac (ORAS)",           Value = 46 },
                new { Text = "Fairy Tale Girl (ORAS)",      Value = 47 },
                new { Text = "Victor Winstrate",            Value = 48 },
                new { Text = "Victoria Winstrate",          Value = 49 },
                new { Text = "Male Ranger (ORAS)",          Value = 50 },
                new { Text = "Female Ranger (ORAS)",        Value = 51 },
                new { Text = "Male Swimmer (ORAS)",         Value = 52 },
                new { Text = "Hex Maniac",                  Value = 53 },
                new { Text = "Male Ace Trainer (ORAS)",     Value = 54 },
                new { Text = "Female Ace Trainer (ORAS)",   Value = 55 },
                new { Text = "Street Thug",                 Value = 56 },
                new { Text = "Delinquent",                  Value = 57 },
                new { Text = "Male Expert",                 Value = 58 },
                new { Text = "Female Expert",               Value = 59 },
                new { Text = "Lady (ORAS)",                 Value = 60 },
                new { Text = "Rich Boy (ORAS)",             Value = 61 },
                new { Text = "Ninja Boy",                   Value = 62 },
                new { Text = "Beauty (ORAS)",               Value = 63 },
                new { Text = "Guitarist",                   Value = 64 },
                new { Text = "Lass (ORAS)",                 Value = 65 },
                new { Text = "Male Breeder (ORAS)",         Value = 66 },
                new { Text = "Female Breeder (ORAS)",       Value = 67 },
                new { Text = "Camper",                      Value = 68 },
                new { Text = "Picnicker",                   Value = 69 },
                new { Text = "Wally",                       Value = 70 },
                new { Text = "Steven",                      Value = 71 },
                new { Text = "Maxie",                       Value = 72 },
                new { Text = "Archie",                      Value = 73 },
                new { Text = "Pokécenter",                  Value = 0x80 },
                new { Text = "Gift",                        Value = 0x81 },
            };

            CB_MultiplayerSprite.DisplayMember = "Text";
            CB_MultiplayerSprite.ValueMember = "Value";
            CB_MultiplayerSprite.DataSource = oras_sprite_list;

            L_Vivillon.Text = GameInfo.Strings.specieslist[666] + ":";
            CB_Vivillon.DisplayMember = "Text";
            CB_Vivillon.ValueMember = "Value";
            CB_Vivillon.DataSource = PKX.getFormList(666, GameInfo.Strings.types, GameInfo.Strings.forms, Main.gendersymbols).ToList();
        }
        private void getBadges()
        {
            // Fetch Badges
            Bitmap[] bma = Main.SAV.ORAS ? 
                new[] {
                                   Core.Properties.Resources.badge_01, // ORAS Badges
                                   Core.Properties.Resources.badge_02,  
                                   Core.Properties.Resources.badge_03,   
                                   Core.Properties.Resources.badge_04,
                                   Core.Properties.Resources.badge_05, 
                                   Core.Properties.Resources.badge_06,  
                                   Core.Properties.Resources.badge_07, 
                                   Core.Properties.Resources.badge_08,
                } : 
                new [] {
                                   Core.Properties.Resources.badge_1, // XY Badges
                                   Core.Properties.Resources.badge_2,  
                                   Core.Properties.Resources.badge_3,   
                                   Core.Properties.Resources.badge_4,
                                   Core.Properties.Resources.badge_5, 
                                   Core.Properties.Resources.badge_6,  
                                   Core.Properties.Resources.badge_7, 
                                   Core.Properties.Resources.badge_8,
                };

            for (int i = 0; i < 8; i++)
                pba[i].Image = ImageUtil.ChangeOpacity(bma[i], cba[i].Checked ? 1 : 0.1);
        }
        private void getTextBoxes()
        {
            int badgeval = SAV.Badges;
            for (int i = 0; i < 8; i++)
                cba[i].Checked = (badgeval & 1 << i) != 0;

            // Get Data
            string OT_NAME = SAV.OT;

            CB_Game.SelectedIndex = SAV.Game - 0x18;
            CB_Gender.SelectedIndex = SAV.Gender;
            
            // Display Data
            TB_OTName.Text = OT_NAME;

            MT_TID.Text = SAV.TID.ToString("00000");
            MT_SID.Text = SAV.SID.ToString("00000");
            MT_Money.Text = SAV.Money.ToString();

            TB_Saying1.Text = SAV.Saying1;
            TB_Saying2.Text = SAV.Saying2;
            TB_Saying3.Text = SAV.Saying3;
            TB_Saying4.Text = SAV.Saying4;
            TB_Saying5.Text = SAV.Saying5;

            CB_Country.SelectedValue = SAV.Country;
            CB_Region.SelectedValue = SAV.SubRegion;
            CB_3DSReg.SelectedValue = SAV.ConsoleRegion;
            CB_Language.SelectedValue = SAV.Language;

            // Maison Data
            if (SAV.MaisonStats > -1)
                for (int i = 0; i < MaisonRecords.Length; i++)
                    MaisonRecords[i].Text = SAV.getMaisonStat(i).ToString();

            NUD_M.Value = SAV.M;
            // Sanity Check Map Coordinates
            if (!GB_Map.Enabled || SAV.X%0.5 != 0 || SAV.Z%0.5 != 0 || SAV.Y%0.5 != 0)
                GB_Map.Enabled = false;
            else try
            {
                NUD_X.Value = (decimal)SAV.X;
                NUD_Z.Value = (decimal)SAV.Z;
                NUD_Y.Value = (decimal)SAV.Y;
            }
            catch { GB_Map.Enabled = false; }

            // Load BP and PokeMiles
            TB_BP.Text = SAV.BP.ToString();
            TB_PM.Text = SAV.getPSSStat(0xFC/4).ToString();

            TB_Style.Text = SAV.Style.ToString();

            // Load Play Time
            MT_Hours.Text = SAV.PlayedHours.ToString();
            MT_Minutes.Text = SAV.PlayedMinutes.ToString();
            MT_Seconds.Text = SAV.PlayedSeconds.ToString();

            // Load PSS Sprite
            CB_MultiplayerSprite.SelectedValue = SAV.MultiplayerSpriteID;
            PB_Sprite.Image = SAV.Sprite();
            
            if (SAV.XY)
            {
                // Load Clothing Data
                int hat = SAV.Data[SAV.TrainerCard + 0x31] >> 3;
                int haircolor = SAV.Data[SAV.TrainerCard + 0x31] & 7;
                MT_Hat.Text = hat.ToString();
                MT_HairColor.Text = haircolor.ToString();
                MT_14030.Text = SAV.Data[SAV.TrainerCard + 0x30].ToString();
                MT_14031.Text = SAV.Data[SAV.TrainerCard + 0x31].ToString();
                MT_14032.Text = SAV.Data[SAV.TrainerCard + 0x32].ToString();
                MT_14033.Text = SAV.Data[SAV.TrainerCard + 0x33].ToString();
                MT_14034.Text = SAV.Data[SAV.TrainerCard + 0x34].ToString();
                MT_14035.Text = SAV.Data[SAV.TrainerCard + 0x35].ToString();
                MT_14036.Text = SAV.Data[SAV.TrainerCard + 0x36].ToString();
                MT_14037.Text = SAV.Data[SAV.TrainerCard + 0x37].ToString();
                MT_14038.Text = SAV.Data[SAV.TrainerCard + 0x38].ToString();
                MT_14039.Text = SAV.Data[SAV.TrainerCard + 0x39].ToString();
                MT_1403A.Text = SAV.Data[SAV.TrainerCard + 0x3A].ToString();
                MT_1403B.Text = SAV.Data[SAV.TrainerCard + 0x3B].ToString();
                MT_1403C.Text = SAV.Data[SAV.TrainerCard + 0x3C].ToString();
                MT_1403D.Text = SAV.Data[SAV.TrainerCard + 0x3D].ToString();
                MT_1403E.Text = SAV.Data[SAV.TrainerCard + 0x3E].ToString();
                MT_1403F.Text = SAV.Data[SAV.TrainerCard + 0x3F].ToString();
            }

            CB_Vivillon.SelectedIndex = SAV.Vivillon;
            if (SAV.LastSavedDate.HasValue)
            {
                CAL_LastSavedDate.Value = SAV.LastSavedDate.Value;
                CAL_LastSavedTime.Value = SAV.LastSavedDate.Value;
            }
            else
            {
                L_LastSaved.Visible = CAL_LastSavedDate.Visible = CAL_LastSavedTime.Visible = false;
            }
            CAL_AdventureStartDate.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToStart);
            CAL_AdventureStartTime.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToStart % 86400);
            CAL_HoFDate.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToFame);
            CAL_HoFTime.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToFame % 86400);
        }
        private void save()
        {
            SAV.Game = (byte)(CB_Game.SelectedIndex + 0x18);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;
            
            SAV.TID = (ushort)Util.ToUInt32(MT_TID.Text);
            SAV.SID = (ushort)Util.ToUInt32(MT_SID.Text);
            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.SubRegion = WinFormsUtil.getIndex(CB_Region);
            SAV.Country = WinFormsUtil.getIndex(CB_Country);
            SAV.ConsoleRegion = WinFormsUtil.getIndex(CB_3DSReg);
            SAV.Language = WinFormsUtil.getIndex(CB_Language);

            SAV.OT = TB_OTName.Text;

            SAV.Saying1 = TB_Saying1.Text;
            SAV.Saying2 = TB_Saying2.Text;
            SAV.Saying3 = TB_Saying3.Text;
            SAV.Saying4 = TB_Saying4.Text;
            SAV.Saying5 = TB_Saying5.Text;

            // Copy Maison Data in
            if (SAV.MaisonStats > -1)
                for (int i = 0; i < MaisonRecords.Length; i++)
                    SAV.setMaisonStat(i, ushort.Parse(MaisonRecords[i].Text));

            // Copy Position
            if (GB_Map.Enabled)
            {
                SAV.M = (int)NUD_M.Value;
                SAV.X = (float)NUD_X.Value;
                SAV.Z = (float)NUD_Z.Value;
                SAV.Y = (float)NUD_Y.Value;
            }

            SAV.BP = ushort.Parse(TB_BP.Text);
            // Set Current PokéMiles
            SAV.setPSSStat(0xFC / 4, Util.ToUInt32(TB_PM.Text));
            // Set Max Obtained Pokémiles
            SAV.setPSSStat(0x100 / 4, Util.ToUInt32(TB_PM.Text));
            SAV.Style = byte.Parse(TB_Style.Text);

            // Copy Badges
            int badgeval = 0;
            for (int i = 0; i < 8; i++)
                badgeval |= (cba[i].Checked ? 1 : 0) << i;
            SAV.Badges = badgeval;

            // Save PlayTime
            SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
            SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
            SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;

            // Sprite
            SAV.MultiplayerSpriteID = Convert.ToByte(CB_MultiplayerSprite.SelectedValue);

            // Appearance
            if (SAV.XY)
            {
                SAV.Data[SAV.TrainerCard + 0x30] = byte.Parse(MT_14030.Text);
                SAV.Data[SAV.TrainerCard + 0x31] = (byte)(byte.Parse(MT_HairColor.Text) | (byte.Parse(MT_Hat.Text) << 3));
                SAV.Data[SAV.TrainerCard + 0x32] = byte.Parse(MT_14032.Text);
                SAV.Data[SAV.TrainerCard + 0x33] = byte.Parse(MT_14033.Text);
                SAV.Data[SAV.TrainerCard + 0x34] = byte.Parse(MT_14034.Text);
                SAV.Data[SAV.TrainerCard + 0x35] = byte.Parse(MT_14035.Text);
                SAV.Data[SAV.TrainerCard + 0x36] = byte.Parse(MT_14036.Text);
                SAV.Data[SAV.TrainerCard + 0x37] = byte.Parse(MT_14037.Text);
                SAV.Data[SAV.TrainerCard + 0x38] = byte.Parse(MT_14038.Text);
                SAV.Data[SAV.TrainerCard + 0x39] = byte.Parse(MT_14039.Text);
                SAV.Data[SAV.TrainerCard + 0x3A] = byte.Parse(MT_1403A.Text);
                SAV.Data[SAV.TrainerCard + 0x3B] = byte.Parse(MT_1403B.Text);
                SAV.Data[SAV.TrainerCard + 0x3C] = byte.Parse(MT_1403C.Text);
                SAV.Data[SAV.TrainerCard + 0x3D] = byte.Parse(MT_1403D.Text);
                SAV.Data[SAV.TrainerCard + 0x3E] = byte.Parse(MT_1403E.Text);
                SAV.Data[SAV.TrainerCard + 0x3F] = byte.Parse(MT_1403F.Text);
            }

            // Vivillon
            SAV.Vivillon = CB_Vivillon.SelectedIndex;

            int seconds = (int)(CAL_AdventureStartDate.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            seconds -= seconds%86400;
            seconds += (int)(CAL_AdventureStartTime.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            SAV.SecondsToStart = seconds;

            int fame = (int)(CAL_HoFDate.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            fame -= fame % 86400;
            fame += (int)(CAL_HoFTime.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            SAV.SecondsToFame = fame;

            if (SAV.LastSavedDate.HasValue)
                SAV.LastSavedDate = new DateTime(CAL_LastSavedDate.Value.Year, CAL_LastSavedDate.Value.Month, CAL_LastSavedDate.Value.Day, CAL_LastSavedTime.Value.Hour, CAL_LastSavedTime.Value.Minute, 0);
        }

        private void clickOT(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox ?? TB_OTName;
            // Special Character Form
            if (ModifierKeys != Keys.Control)
                return;

            var z = Application.OpenForms.Cast<Form>().FirstOrDefault(form => form.Name == typeof(f2_Text).Name) as f2_Text;
            if (z != null)
            { z.Location = Location; z.BringToFront(); return; }
            new f2_Text(tb).Show();
        }
        private void showTSV(object sender, EventArgs e)
        {
            uint TID = Util.ToUInt32(MT_TID.Text);
            uint SID = Util.ToUInt32(MT_SID.Text);
            uint tsv = (TID ^ SID) >> 4;
            Tip1.SetToolTip(MT_TID, "TSV: " + tsv.ToString("0000"));
            Tip2.SetToolTip(MT_SID, "TSV: " + tsv.ToString("0000"));
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            save();
            Main.SAV.Data = SAV.Data;
            Main.SAV.Edited = true;
            Close();
        }
        private void changeBadge(object sender, EventArgs e)
        {
            getBadges();
        }
        private void changeSpecial(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            int val = Util.ToInt32(box?.Text);

            if (box == MT_HairColor)
                box.Text = (val > 7 ? 7 : val).ToString();
            if (box == MT_Hat)
                box.Text = (val > 31 ? 31 : val).ToString();
        }
        private void change255(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box?.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 255) box.Text = "255";
        }
        private void changeFFFF(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box?.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 65535) box.Text = "65535";
        }
        private void changeStat(object sender, EventArgs e)
        {
            editing = true;
            int offset = Convert.ToInt32(statdata[CB_Stats.SelectedIndex * 2].Substring(2), 16);
            MT_Stat.Text = SAV.getPSSStat(offset/4).ToString();
            L_Offset.Text = "0x" + offset.ToString("X3");
            editing = false;
        }
        private void changeStatVal(object sender, EventArgs e)
        {
            if (editing) return;
            int offset = Convert.ToInt32(statdata[CB_Stats.SelectedIndex * 2].Substring(2), 16);
            SAV.setPSSStat(offset/4, uint.Parse(MT_Stat.Text));
        }
        private void giveAllAccessories(object sender, EventArgs e)
        {
            new byte[]
            {
                0xFE,0xFF,0xFF,0x7E,0xFF,0xFD,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
                0xFF,0xEF,0xFF,0xFF,0xFF,0xF9,0xFF,0xFB,0xFF,0xF7,0xFF,0xFF,0x0F,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFE,0xFF,
                0xFF,0x7E,0xFF,0xFD,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xEF,
                0xFF,0xFF,0xFF,0xF9,0xFF,0xFB,0xFF,0xF7,0xFF,0xFF,0x0F,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            }.CopyTo(SAV.Data, SAV.Accessories);
        }

        private void updateCountry(object sender, EventArgs e)
        {
            if (WinFormsUtil.getIndex(sender as ComboBox) > 0)
                Main.setCountrySubRegion(CB_Region, "sr_" + WinFormsUtil.getIndex(sender as ComboBox).ToString("000"));
        }
        private void toggleBadge(object sender, EventArgs e)
        {
            cba[Array.IndexOf(pba, sender)].Checked ^= true;
        }

        private void CB_Multi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing)
                return;
            SAV.MultiplayerSpriteID = WinFormsUtil.getIndex(CB_MultiplayerSprite);
            PB_Sprite.Image = SAV.Sprite();
        }
    }
}