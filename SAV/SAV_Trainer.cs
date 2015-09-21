using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Trainer : Form
    {
        public SAV_Trainer()
        {
            InitializeComponent();
            if (!Main.unicode)
            try { TB_OTName.Font = PKX.getPKXFont(11); }
            catch (Exception e) { Util.Alert("Font loading failed...", e.ToString()); }

            Util.TranslateInterface(this, Main.curlanguage);
            sav = (byte[])Main.savefile.Clone();
            if (Main.SaveGame.ORAS)
            {
                Width = (int)((float)Width * 428 / 590);
                CB_Multi.Enabled = true;
                L_MultiplayerSprite.Enabled = true; // Multiplayer Sprite Label

                L_Style.Visible = false; // Style Label
                TB_Style.Visible = false;
                GB_Appear.Visible = false;
            }
            editing = true;
            getComboBoxes();
            getTextBoxes();
            getBadges();
            GB_Map.Enabled = !Main.ramsavloaded;

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
                "0x0FC",	"Current Pokemiles",
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
        private string[] statdata = { };
        public byte[] sav = new byte[0x100000];
        public bool editing;
        public byte badgeval;
        public ToolTip Tip1 = new ToolTip();
        public ToolTip Tip2 = new ToolTip();

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
            };

            CB_Multi.DisplayMember = "Text";
            CB_Multi.ValueMember = "Value";
            CB_Multi.DataSource = oras_sprite_list;

            L_Vivillon.Text = Main.specieslist[666] + ":";
            Main.setForms(666, CB_Vivillon);
        }
        private void getBadges()
        {
            // Fetch Badges
            Bitmap[] bma = (Main.SaveGame.ORAS) ? 
                new[] {
                                   Properties.Resources.badge_01, // ORAS Badges
                                   Properties.Resources.badge_02,  
                                   Properties.Resources.badge_03,   
                                   Properties.Resources.badge_04,
                                   Properties.Resources.badge_05, 
                                   Properties.Resources.badge_06,  
                                   Properties.Resources.badge_07, 
                                   Properties.Resources.badge_08,
                } : 
                new [] {
                                   Properties.Resources.badge_1, // XY Badges
                                   Properties.Resources.badge_2,  
                                   Properties.Resources.badge_3,   
                                   Properties.Resources.badge_4,
                                   Properties.Resources.badge_5, 
                                   Properties.Resources.badge_6,  
                                   Properties.Resources.badge_7, 
                                   Properties.Resources.badge_8,
                };
            CheckBox[] cba = { cb1, cb2, cb3, cb4, cb5, cb6, cb7, cb8, };
            PictureBox[] pba = { pb1, pb2, pb3, pb4, pb5, pb6, pb7, pb8, };

            for (int i = 0; i < 8; i++)
                pba[i].Image = Util.ChangeOpacity(bma[i], !cba[i].Checked ? 0.1 : 1);
        }
        private void getTextBoxes()
        {
            badgeval = sav[Main.SaveGame.Trainer2 + 0xC];
            CheckBox[] cba = { cb1, cb2, cb3, cb4, cb5, cb6, cb7, cb8, };
            for (int i = 0; i < 8; i++)
                cba[i].Checked = (badgeval & (1 << i)) != 0;

            // Get Data
            string OT_NAME = Encoding.Unicode.GetString(sav, Main.SaveGame.TrainerCard + 0x48, 0x1A);

            CB_Game.SelectedIndex = sav[Main.SaveGame.TrainerCard + 0x04] - 0x18;
            CB_Gender.SelectedIndex = sav[Main.SaveGame.TrainerCard + 0x05];

            int TID = BitConverter.ToUInt16(sav, Main.SaveGame.TrainerCard + 0x0);
            int SID = BitConverter.ToUInt16(sav, Main.SaveGame.TrainerCard + 0x2);
            uint money = BitConverter.ToUInt32(sav, Main.SaveGame.Trainer2 + 0x8);

            string saying1 = Encoding.Unicode.GetString(sav, Main.SaveGame.TrainerCard + 0x7C, 0x20);
            string saying2 = Encoding.Unicode.GetString(sav, Main.SaveGame.TrainerCard + 0x9E, 0x20);
            string saying3 = Encoding.Unicode.GetString(sav, Main.SaveGame.TrainerCard + 0xC0, 0x20);
            string saying4 = Encoding.Unicode.GetString(sav, Main.SaveGame.TrainerCard + 0xE2, 0x20);
            string saying5 = Encoding.Unicode.GetString(sav, Main.SaveGame.TrainerCard + 0x104, 0x20);

            int _region = sav[Main.SaveGame.TrainerCard + 0x26];
            int _country = sav[Main.SaveGame.TrainerCard + 0x27];
            int _3dsreg = sav[Main.SaveGame.TrainerCard + 0x2C];
            int _language = sav[Main.SaveGame.TrainerCard + 0x2D];

            // Display Data
            TB_OTName.Text = OT_NAME;

            MT_TID.Text = TID.ToString("00000");
            MT_SID.Text = SID.ToString("00000");
            MT_Money.Text = money.ToString();

            TB_Saying1.Text = saying1;
            TB_Saying2.Text = saying2;
            TB_Saying3.Text = saying3;
            TB_Saying4.Text = saying4;
            TB_Saying5.Text = saying5;

            CB_Country.SelectedValue = _country;
            CB_Region.SelectedValue = _region;

            CB_3DSReg.SelectedValue = _3dsreg;
            CB_Language.SelectedValue = _language;

            // Maison Data
            TB_MCSN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 0).ToString();
            TB_MCSS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 1).ToString();

            TB_MBSN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 2).ToString();
            TB_MBSS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 3).ToString();

            TB_MCDN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 4).ToString();
            TB_MCDS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 5).ToString();

            TB_MBDN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 6).ToString();
            TB_MBDS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 7).ToString();

            TB_MCTN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 8).ToString();
            TB_MCTS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 9).ToString();

            TB_MBTN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 10).ToString();
            TB_MBTS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 11).ToString();

            TB_MCRN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 12).ToString();
            TB_MCRS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 13).ToString();

            TB_MBRN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 14).ToString();
            TB_MBRS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 15).ToString();

            TB_MCMN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 16).ToString();
            TB_MCMS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 17).ToString();

            TB_MBMN.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 18).ToString();
            TB_MBMS.Text = BitConverter.ToUInt16(sav, Main.SaveGame.MaisonStats + 2 * 19).ToString();

            TB_CoordM.Text = BitConverter.ToUInt16(sav, Main.SaveGame.Trainer1 + 0x02).ToString();
            TB_CoordX.Text = BitConverter.ToSingle(sav, Main.SaveGame.Trainer1 + 0x10).ToString();
            TB_CoordZ.Text = BitConverter.ToSingle(sav, Main.SaveGame.Trainer1 + 0x14).ToString();
            TB_CoordY.Text = BitConverter.ToSingle(sav, Main.SaveGame.Trainer1 + 0x18).ToString();

            // Load BP and PokeMiles
            TB_BP.Text = BitConverter.ToUInt32(sav, Main.SaveGame.Trainer2 + 0x3C - 0xC * Convert.ToInt16(Main.SaveGame.ORAS)).ToString();
            TB_PM.Text = BitConverter.ToUInt32(sav, Main.SaveGame.PSSStats + 0xFC).ToString();

            // Temp ORAS
            GB_Misc.Visible = true;

            TB_Style.Text = sav[0x694D].ToString();

            // Load Play Time
            MT_Hours.Text = BitConverter.ToUInt16(sav, 0x6C00).ToString();
            MT_Minutes.Text = sav[0x6C02].ToString();
            MT_Seconds.Text = sav[0x6C03].ToString();

            // Load PSS Sprite
            int sprite = sav[Main.SaveGame.TrainerCard + 0x07];
            CB_Multi.SelectedValue = sprite;
            PB_Sprite.Image = (Image)Properties.Resources.ResourceManager.GetObject("tr_" + sprite.ToString("00"));

            // Load Clothing Data
            int hat = sav[Main.SaveGame.TrainerCard + 0x31] >> 3;
            int haircolor = sav[Main.SaveGame.TrainerCard + 0x31] & 7;
            MT_Hat.Text = hat.ToString();
            MT_HairColor.Text = haircolor.ToString();

            MT_14030.Text = sav[Main.SaveGame.TrainerCard + 0x30].ToString();
            MT_14031.Text = sav[Main.SaveGame.TrainerCard + 0x31].ToString();
            MT_14032.Text = sav[Main.SaveGame.TrainerCard + 0x32].ToString();
            MT_14033.Text = sav[Main.SaveGame.TrainerCard + 0x33].ToString();
            MT_14034.Text = sav[Main.SaveGame.TrainerCard + 0x34].ToString();
            MT_14035.Text = sav[Main.SaveGame.TrainerCard + 0x35].ToString();
            MT_14036.Text = sav[Main.SaveGame.TrainerCard + 0x36].ToString();
            MT_14037.Text = sav[Main.SaveGame.TrainerCard + 0x37].ToString();
            MT_14038.Text = sav[Main.SaveGame.TrainerCard + 0x38].ToString();
            MT_14039.Text = sav[Main.SaveGame.TrainerCard + 0x39].ToString();
            MT_1403A.Text = sav[Main.SaveGame.TrainerCard + 0x3A].ToString();
            MT_1403B.Text = sav[Main.SaveGame.TrainerCard + 0x3B].ToString();
            MT_1403C.Text = sav[Main.SaveGame.TrainerCard + 0x3C].ToString();
            MT_1403D.Text = sav[Main.SaveGame.TrainerCard + 0x3D].ToString();
            MT_1403E.Text = sav[Main.SaveGame.TrainerCard + 0x3E].ToString();
            MT_1403F.Text = sav[Main.SaveGame.TrainerCard + 0x3F].ToString();

            // Vivillon
            byte vivillon = sav[Main.SaveGame.Vivillon];
            CB_Vivillon.SelectedIndex = vivillon;
        }
        private void save()
        {
            string OT_Name = TB_OTName.Text;
            //string RIV_Name = TB_Rival.Text;

            sav[Main.SaveGame.TrainerCard + 0x04] = (byte)(CB_Game.SelectedIndex + 0x18);
            sav[Main.SaveGame.TrainerCard + 0x05] = (byte)CB_Gender.SelectedIndex;

            uint TID = Util.ToUInt32(MT_TID.Text);
            uint SID = Util.ToUInt32(MT_SID.Text);
            uint money = Util.ToUInt32(MT_Money.Text);

            string saying1 = TB_Saying1.Text;
            string saying2 = TB_Saying2.Text;
            string saying3 = TB_Saying3.Text;
            string saying4 = TB_Saying4.Text;
            string saying5 = TB_Saying5.Text;

            uint _region = Util.ToUInt32(CB_Region.SelectedValue.ToString());
            uint _country = Util.ToUInt32(CB_Country.SelectedValue.ToString());
            uint _3dsreg = Util.ToUInt32(CB_3DSReg.SelectedValue.ToString());
            uint _language = Util.ToUInt32(CB_Language.SelectedValue.ToString());

            Array.Copy(BitConverter.GetBytes(TID), 0, sav, Main.SaveGame.TrainerCard + 0x0, 2);
            Array.Copy(BitConverter.GetBytes(SID), 0, sav, Main.SaveGame.TrainerCard + 0x2, 2);
            Array.Copy(BitConverter.GetBytes(money), 0, sav, Main.SaveGame.Trainer2 + 0x8, 4);
            Array.Copy(BitConverter.GetBytes(_region), 0, sav, Main.SaveGame.TrainerCard + 0x26, 1);
            Array.Copy(BitConverter.GetBytes(_country), 0, sav, Main.SaveGame.TrainerCard + 0x27, 1);
            Array.Copy(BitConverter.GetBytes(_3dsreg), 0, sav, Main.SaveGame.TrainerCard + 0x2C, 1);
            Array.Copy(BitConverter.GetBytes(_language), 0, sav, Main.SaveGame.TrainerCard + 0x2D, 1);

            Array.Copy(Encoding.Unicode.GetBytes(OT_Name.PadRight(13, '\0')), 0, sav, Main.SaveGame.TrainerCard + 0x48, 0x1A);

            Array.Copy(Encoding.Unicode.GetBytes(saying1.PadRight(17, '\0')), 0, sav, Main.SaveGame.TrainerCard + 0x7C, 0x22);
            Array.Copy(Encoding.Unicode.GetBytes(saying2.PadRight(17, '\0')), 0, sav, Main.SaveGame.TrainerCard + 0x9E, 0x22);
            Array.Copy(Encoding.Unicode.GetBytes(saying3.PadRight(17, '\0')), 0, sav, Main.SaveGame.TrainerCard + 0xC0, 0x22);
            Array.Copy(Encoding.Unicode.GetBytes(saying4.PadRight(17, '\0')), 0, sav, Main.SaveGame.TrainerCard + 0xE2, 0x22);
            Array.Copy(Encoding.Unicode.GetBytes(saying5.PadRight(17, '\0')), 0, sav, Main.SaveGame.TrainerCard + 0x104, 0x22);

            // New stuff.
            // Copy Maison Data in
            MaskedTextBox[] tba =
            {
                TB_MCSN,TB_MCSS,TB_MBSN,TB_MBSS,
                TB_MCDN,TB_MCDS,TB_MBDN,TB_MBDS,
                TB_MCTN,TB_MCTS,TB_MBTN,TB_MBTS,
                TB_MCRN,TB_MCRS,TB_MBRN,TB_MBRS,
                TB_MCMN,TB_MCMS,TB_MBMN,TB_MBMS,
            };
            for (int i = 0; i < tba.Length; i++)
                Array.Copy(BitConverter.GetBytes(UInt16.Parse(tba[i].Text)), 0, sav, Main.SaveGame.MaisonStats + 2 * i, 2);

            // Copy Position
            Array.Copy(BitConverter.GetBytes(UInt16.Parse(TB_CoordM.Text)), 0, sav, Main.SaveGame.Trainer1 + 0x02, 2); // m
            Array.Copy(BitConverter.GetBytes(Single.Parse(TB_CoordX.Text)), 0, sav, Main.SaveGame.Trainer1 + 0x10, 4); // x
            Array.Copy(BitConverter.GetBytes(Single.Parse(TB_CoordZ.Text)), 0, sav, Main.SaveGame.Trainer1 + 0x14, 4); // y
            Array.Copy(BitConverter.GetBytes(Single.Parse(TB_CoordY.Text)), 0, sav, Main.SaveGame.Trainer1 + 0x18, 4); // z

            Array.Copy(BitConverter.GetBytes(UInt16.Parse(TB_BP.Text)), 0, sav, 0x963C - 0xC * Convert.ToInt16(Main.SaveGame.ORAS), 2);
            Array.Copy(BitConverter.GetBytes(Util.ToUInt32(TB_PM.Text)), 0, sav, Main.SaveGame.PSSStats + 0xFC, 4);
            Array.Copy(BitConverter.GetBytes(Util.ToUInt32(TB_PM.Text)), 0, sav, Main.SaveGame.PSSStats + 0x100, 4);
            sav[0x694D] = Byte.Parse(TB_Style.Text);

            // Copy Badges
            badgeval = 0;
            CheckBox[] cba = { cb1, cb2, cb3, cb4, cb5, cb6, cb7, cb8, };
            for (int i = 0; i < 8; i++)
                badgeval |= (byte)(Convert.ToByte(cba[i].Checked) << i);
            sav[0x960C] = badgeval;

            // Save PlayTime
            Array.Copy(BitConverter.GetBytes(UInt16.Parse(MT_Hours.Text)), 0, sav, 0x6C00, 2);
            sav[0x6C02] = (byte)(UInt16.Parse(MT_Minutes.Text) % 60);
            sav[0x6C03] = (byte)(UInt16.Parse(MT_Seconds.Text) % 60);

            // Sprite
            sav[Main.SaveGame.TrainerCard + 0x07] = Convert.ToByte(CB_Multi.SelectedValue);

            // Appearance
            sav[Main.SaveGame.TrainerCard + 0x30] = Byte.Parse(MT_14030.Text);
            sav[Main.SaveGame.TrainerCard + 0x31] = (byte)(Byte.Parse(MT_HairColor.Text) + (Byte.Parse(MT_Hat.Text) << 3));
            sav[Main.SaveGame.TrainerCard + 0x32] = Byte.Parse(MT_14032.Text);
            sav[Main.SaveGame.TrainerCard + 0x33] = Byte.Parse(MT_14033.Text);
            sav[Main.SaveGame.TrainerCard + 0x34] = Byte.Parse(MT_14034.Text);
            sav[Main.SaveGame.TrainerCard + 0x35] = Byte.Parse(MT_14035.Text);
            sav[Main.SaveGame.TrainerCard + 0x36] = Byte.Parse(MT_14036.Text);
            sav[Main.SaveGame.TrainerCard + 0x37] = Byte.Parse(MT_14037.Text);
            sav[Main.SaveGame.TrainerCard + 0x38] = Byte.Parse(MT_14038.Text);
            sav[Main.SaveGame.TrainerCard + 0x39] = Byte.Parse(MT_14039.Text);
            sav[Main.SaveGame.TrainerCard + 0x3A] = Byte.Parse(MT_1403A.Text);
            sav[Main.SaveGame.TrainerCard + 0x3B] = Byte.Parse(MT_1403B.Text);
            sav[Main.SaveGame.TrainerCard + 0x3C] = Byte.Parse(MT_1403C.Text);
            sav[Main.SaveGame.TrainerCard + 0x3D] = Byte.Parse(MT_1403D.Text);
            sav[Main.SaveGame.TrainerCard + 0x3E] = Byte.Parse(MT_1403E.Text);
            sav[Main.SaveGame.TrainerCard + 0x3F] = Byte.Parse(MT_1403F.Text);

            // Vivillon
            sav[Main.SaveGame.Vivillon] = (byte)CB_Vivillon.SelectedIndex;
        }

        private void clickOT(object sender, MouseEventArgs e)
        {
            TextBox tb = (!(sender is TextBox)) ? TB_OTName : (sender as TextBox);
            // Special Character Form
            if (ModifierKeys == Keys.Control && !Main.specialChars)
                (new f2_Text(tb)).Show();
        }
        private void showTSV(object sender, EventArgs e)
        {
            uint TID = Util.ToUInt32(MT_TID.Text);
            uint SID = Util.ToUInt32(MT_SID.Text);
            uint tsv = PKX.getTSV(TID, SID);
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
            Array.Copy(sav, Main.savefile, Main.savefile.Length);
            Main.savedited = true;
            Close();
        }
        private void B_MaxCash_Click(object sender, EventArgs e)
        {
            MT_Money.Text = "9,999,999";
        }
        private void changeBadge(object sender, EventArgs e)
        {
            getBadges();
        }
        private void changeSpecial(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            int val = Util.ToInt32(box.Text);

            if (box == MT_HairColor)
                box.Text = (val > 7 ? 7 : val).ToString();
            if (box == MT_Hat)
                box.Text = (val > 31 ? 31 : val).ToString();
        }
        private void change255(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 255) box.Text = "255";
        }
        private void changeFFFF(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 65535) box.Text = "65535";
        }
        private void changeStat(object sender, EventArgs e)
        {
            editing = true;
            {
                string offsetstr = statdata[CB_Stats.SelectedIndex * 2];
                int offset = (int)new System.ComponentModel.Int32Converter().ConvertFromString(offsetstr);

                MT_Stat.Text = BitConverter.ToUInt32(sav, Main.SaveGame.PSSStats + offset).ToString();
                L_Offset.Text = "0x" + offset.ToString("X3");
            }
            editing = false;
        }
        private void changeStatVal(object sender, EventArgs e)
        {
            if (editing) return;

            string offsetstr = statdata[CB_Stats.SelectedIndex * 2];
            int offset = (int)new System.ComponentModel.Int32Converter().ConvertFromString(offsetstr);

            uint val = UInt32.Parse(MT_Stat.Text);
            byte[] data = BitConverter.GetBytes(val);
            Array.Resize(ref data, 4);
            Array.Copy(data, 0, sav, Main.SaveGame.PSSStats + offset, 4);
        }
        private void giveAllAccessories(object sender, EventArgs e)
        {
            byte[] data =
            {
                0xFE,0xFF,0xFF,0x7E,0xFF,0xFD,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
                0xFF,0xEF,0xFF,0xFF,0xFF,0xF9,0xFF,0xFB,0xFF,0xF7,0xFF,0xFF,0x0F,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFE,0xFF,
                0xFF,0x7E,0xFF,0xFD,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xEF,
                0xFF,0xFF,0xFF,0xF9,0xFF,0xFB,0xFF,0xF7,0xFF,0xFF,0x0F,0x00,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            };
            Array.Copy(data, 0, sav, 0x6E00, 0x6C);
        }

        private void updateCountry(object sender, EventArgs e)
        {
            if (Util.getIndex(sender as ComboBox) > 0)
                Main.setCountrySubRegion(CB_Region, "sr_" + Util.getIndex(sender as ComboBox).ToString("000"));
        }
        private void toggleBadge(object sender, EventArgs e)
        {
            int val = Convert.ToInt16(((PictureBox)sender).Name.Last().ToString()) - 1;
            CheckBox[] chka = { cb1, cb2, cb3, cb4, cb5, cb6, cb7, cb8 };
            chka[val].Checked = !chka[val].Checked;
        }

        private void CB_Multi_SelectedIndexChanged(object sender, EventArgs e)
        {
            PB_Sprite.Image = (Image)Properties.Resources.ResourceManager.GetObject((Main.SaveGame.ORAS) ? "tr_" + Util.getIndex(CB_Multi).ToString("00") : "tr_00");
        }
    }
}