using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Inventory : Form
    {
        public SAV_Inventory(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            shiftval = savindex * 0x7F000;

            getListItems();
            getListKeyItems();
            getListTHMH();
            getListMedicine();
            getListBerries();

            B_DisplayItems.ForeColor = Color.Red;

            popItems();

        }
        Form1 m_parent;
        public byte[] sav = new Byte[0x100000];
        public int savindex; int shiftval;
        public bool editing = false;

        public string[] item_val;
        public string[] keyitem_val;
        public string[] tmhm_val;
        public string[] medicine_val;
        public string[] berries_val;

        // Initialize String Tables
        private void getListItems()
        {
            item_val = new string[] {   "(None)",	        "0",
                                        "Abomasite",	    "674",
                                        "Absolite",	        "677",
                                        "Absorb Bulb",	    "545",
                                        "Adamant Orb",	    "135",
                                        "Aerodactylite",	"672",
                                        "Aggronite",	    "667",
                                        "Air Balloon",	    "541",
                                        "Alakazite",	    "679",
                                        "Amaze Mulch",	    "655",
                                        "Ampharosite",	    "658",
                                        "Amulet Coin",	    "223",
                                        "Armor Fossil",	    "104",
                                        "Assault Vest",	    "640",
                                        "Balm Mushroom",	"580",
                                        "Banettite",	    "668",
                                        "Big Mushroom",	    "87",
                                        "Big Nugget",	    "581",
                                        "Big Pearl",	    "89",
                                        "Big Root",	        "296",
                                        "Binding Band",	    "544",
                                        "Black Belt",	    "241",
                                        "Black Flute",	    "68",
                                        "Black Glasses",	"240",
                                        "Black Sludge",	    "281",
                                        "Blastoisinite",	"661",
                                        "Blazikenite",	    "664",
                                        "Blue Flute",	    "65",
                                        "Blue Scarf",	    "261",
                                        "Blue Shard",	    "73",
                                        "Boost Mulch",	    "654",
                                        "Bright Powder",	"213",
                                        "Bug Gem",	        "558",
                                        "Burn Drive",	    "118",
                                        "Cell Battery",	    "546",
                                        "Charcoal",	        "249",
                                        "Charizardite X",	"660",
                                        "Charizardite Y",	"678",
                                        "Cherish Ball",	    "16",
                                        "Chill Drive",	    "119",
                                        "Choice Band",	    "220",
                                        "Choice Scarf",	    "287",
                                        "Choice Specs",	    "297",
                                        "Claw Fossil",	    "100",
                                        "Cleanse Tag",	    "224",
                                        "Comet Shard",	    "583",
                                        "Cover Fossil",	    "572",
                                        "Damp Rock",	    "285",
                                        "Dark Gem",	        "562",
                                        "Dawn Stone",	    "109",
                                        "Deep Sea Scale",	"227",
                                        "Deep Sea Tooth",	"226",
                                        "Destiny Knot",	    "280",
                                        "Dire Hit",	        "56",
                                        "Discount Coupon",	"699",
                                        "Dive Ball",	    "7",
                                        "Dome Fossil",	    "102",
                                        "Douse Drive",	    "116",
                                        "Draco Plate",	    "311",
                                        "Dragon Fang",	    "250",
                                        "Dragon Gem",	    "561",
                                        "Dragon Scale",	    "235",
                                        "Dread Plate",	    "312",
                                        "Dream Ball",	    "576",
                                        "Dubious Disc",	    "324",
                                        "Dusk Ball",	    "13",
                                        "Dusk Stone",	    "108",
                                        "Earth Plate",	    "305",
                                        "Eject Button",	    "547",
                                        "Electirizer",	    "322",
                                        "Electric Gem",	    "550",
                                        "Escape Rope",	    "78",
                                        "Everstone",	    "229",
                                        "Eviolite",	        "538",
                                        "Expert Belt",	    "268",
                                        "Fairy Gem",	    "715",
                                        "Fast Ball",	    "492",
                                        "Fighting Gem",	    "553",
                                        "Fire Gem",	        "548",
                                        "Fire Stone",	    "82",
                                        "Fist Plate",	    "303",
                                        "Flame Orb",	    "273",
                                        "Flame Plate",	    "298",
                                        "Float Stone",	    "539",
                                        "Fluffy Tail",	    "64",
                                        "Flying Gem",	    "556",
                                        "Focus Band",	    "230",
                                        "Focus Sash",	    "275",
                                        "Friend Ball",	    "497",
                                        "Full Incense",	    "316",
                                        "Garchompite",	    "683",
                                        "Gardevoirite",	    "657",
                                        "Gengarite",	    "656",
                                        "Ghost Gem",	    "560",
                                        "Grass Gem",	    "551",
                                        "Great Ball",	    "3",
                                        "Green Scarf",	    "263",
                                        "Green Shard",	    "75",
                                        "Grip Claw",	    "286",
                                        "Griseous Orb",	    "112",
                                        "Ground Gem",	    "555",
                                        "Guard Spec.",	    "55",
                                        "Gyaradosite",	    "676",
                                        "Hard Stone",	    "238",
                                        "Heal Ball",	    "14",
                                        "Heart Scale",	    "93",
                                        "Heat Rock",	    "284",
                                        "Heavy Ball",	    "495",
                                        "Helix Fossil",	    "101",
                                        "Heracronite",	    "680",
                                        "Honey",	        "94",
                                        "Houndoominite",	"666",
                                        "Ice Gem",	        "552",
                                        "Icicle Plate",	    "302",
                                        "Icy Rock",	        "282",
                                        "Insect Plate",	    "308",
                                        "Iron Ball",	    "278",
                                        "Iron Plate",	    "313",
                                        "Jaw Fossil",	    "710",
                                        "Kangaskhanite",	"675",
                                        "King's Rock",	    "221",
                                        "Lagging Tail",	    "279",
                                        "Latiasite",	    "684",
                                        "Latiosite",	    "685",
                                        "Lax Incense",	    "255",
                                        "Leaf Stone",	    "85",
                                        "Leftovers",	    "234",
                                        "Level Ball",	    "493",
                                        "Life Orb",	        "270",
                                        "Light Ball",	    "236",
                                        "Light Clay",	    "269",
                                        "Love Ball",	    "496",
                                        "Lucarionite",	    "673",
                                        "Luck Incense",	    "319",
                                        "Lucky Egg",	    "231",
                                        "Lucky Punch",	    "256",
                                        "Luminous Moss",	"648",
                                        "Lure Ball",	    "494",
                                        "Lustrous Orb",	    "136",
                                        "Luxury Ball",	    "11",
                                        "Macho Brace",	    "215",
                                        "Magmarizer",	    "323",
                                        "Magnet",	        "242",
                                        "Manectite",	    "682",
                                        "Master Ball",	    "1",
                                        "Mawilite",	        "681",
                                        "Max Repel",	    "77",
                                        "Meadow Plate",	    "301",
                                        "Medichamite",	    "665",
                                        "Mental Herb",	    "219",
                                        "Metal Coat",	    "233",
                                        "Metal Powder",	    "257",
                                        "Metronome",	    "277",
                                        "Mewtwonite X",	    "662",
                                        "Mewtwonite Y",	    "663",
                                        "Mind Plate",	    "307",
                                        "Miracle Seed",	    "239",
                                        "Moon Ball",	    "498",
                                        "Moon Stone",	    "81",
                                        "Muscle Band",	    "266",
                                        "Mystic Water",	    "243",
                                        "Nest Ball",	    "8",
                                        "Net Ball",	        "6",
                                        "Never-Melt Ice",	"246",
                                        "Normal Gem",	    "564",
                                        "Nugget",	        "92",
                                        "Odd Incense",	    "314",
                                        "Old Amber",	    "103",
                                        "Oval Stone",	    "110",
                                        "Park Ball",	    "500",
                                        "Pearl String",	    "582",
                                        "Pearl",	        "88",
                                        "Pink Scarf",	    "262",
                                        "Pinsirite",	    "671",
                                        "Pixie Plate",	    "644",
                                        "Plume Fossil",	    "573",
                                        "Poison Barb",	    "245",
                                        "Poison Gem",	    "554",
                                        "Poké Ball",	    "4",
                                        "Poké Doll",	    "63",
                                        "Poké Toy",	        "577",
                                        "Power Anklet",	    "293",
                                        "Power Band",	    "292",
                                        "Power Belt",	    "290",
                                        "Power Bracer",	    "289",
                                        "Power Herb",	    "271",
                                        "Power Lens",	    "291",
                                        "Power Weight",	    "294",
                                        "Premier Ball",	    "12",
                                        "Pretty Wing",	    "571",
                                        "Prism Scale",	    "537",
                                        "Protector",	    "321",
                                        "Psychic Gem",	    "557",
                                        "Pure Incense",	    "320",
                                        "Quick Ball",	    "15",
                                        "Quick Claw",	    "217",
                                        "Quick Powder",	    "274",
                                        "Rare Bone",	    "106",
                                        "Razor Claw",	    "326",
                                        "Razor Fang",	    "327",
                                        "Reaper Cloth",	    "325",
                                        "Red Card",     	"542",
                                        "Red Flute",	    "67",
                                        "Red Scarf",	    "260",
                                        "Red Shard",	    "72",
                                        "Relic Band",	    "588",
                                        "Relic Copper",	    "584",
                                        "Relic Crown",	    "590",
                                        "Relic Gold",	    "586",
                                        "Relic Silver", 	"585",
                                        "Relic Statue", 	"589",
                                        "Relic Vase",	    "587",
                                        "Repeat Ball",	    "9",
                                        "Repel",	        "79",
                                        "Rich Mulch",	    "652",
                                        "Ring Target",  	"543",
                                        "Rock Gem", 	    "559",
                                        "Rock Incense",	    "315",
                                        "Rocky Helmet", 	"540",
                                        "Root Fossil",  	"99",
                                        "Rose Incense", 	"318",
                                        "Sachet",	        "647",
                                        "Safari Ball",	    "5",
                                        "Safety Goggles",	"650",
                                        "Sail Fossil",	    "711",
                                        "Scizorite",	    "670",
                                        "Scope Lens",	    "232",
                                        "Sea Incense",	    "254",
                                        "Sharp Beak",	    "244",
                                        "Shed Shell",	    "295",
                                        "Shell Bell",	    "253",
                                        "Shiny Stone",	    "107",
                                        "Shoal Salt",	    "70",
                                        "Shoal Shell",	    "71",
                                        "Shock Drive",	    "117",
                                        "Silk Scarf",	    "251",
                                        "Silver Powder",	"222",
                                        "Skull Fossil",	    "105",
                                        "Sky Plate",	    "306",
                                        "Smoke Ball",	    "228",
                                        "Smooth Rock",	    "283",
                                        "Snowball",	        "649",
                                        "Soft Sand",	    "237",
                                        "Soothe Bell",	    "218",
                                        "Soul Dew",	        "225",
                                        "Spell Tag",	    "247",
                                        "Splash Plate",	    "299",
                                        "Spooky Plate",	    "310",
                                        "Sport Ball",	    "499",
                                        "Star Piece",	    "91",
                                        "Stardust",	        "90",
                                        "Steel Gem",	    "563",
                                        "Stick",	        "259",
                                        "Sticky Barb",	    "288",
                                        "Stone Plate",	    "309",
                                        "Strange Souvenir",	"704",
                                        "Sun Stone",	    "80",
                                        "Super Repel",	    "76",
                                        "Surprise Mulch",	"653",
                                        "Thick Club",	    "258",
                                        "Thunder Stone",	"83",
                                        "Timer Ball",	    "10",
                                        "Tiny Mushroom",	"86",
                                        "Toxic Orb",	    "272",
                                        "Toxic Plate",	    "304",
                                        "Twisted Spoon",	"248",
                                        "Tyranitarite",	    "669",
                                        "Ultra Ball",	    "2",
                                        "Up-Grade",	        "252",
                                        "Venusaurite",	    "659",
                                        "Water Gem",	    "549",
                                        "Water Stone",	    "84",
                                        "Wave Incense",	    "317",
                                        "Weakness Policy",	"639",
                                        "Whipped Dream",	"646",
                                        "White Flute",	    "69",
                                        "White Herb",	    "214",
                                        "Wide Lens",	    "265",
                                        "Wise Glasses",	    "267",
                                        "X Accuracy",	    "60",
                                        "X Attack",     	"57",
                                        "X Defense",	    "58",
                                        "X Sp. Atk",	    "61",
                                        "X Sp. Def",	    "62",
                                        "X Speed",	        "59",
                                        "Yellow Flute",	    "66",
                                        "Yellow Scarf",	    "264",
                                        "Yellow Shard",	    "74",
                                        "Zap Plate",	    "300",
                                        "Zoom Lens",	    "276",
            };

        }
        private void getListKeyItems()
        {
            keyitem_val = new string[] { "(None)",	        "0",
                                        "Adventure Rules",	"703",
                                        "Bicycle (Green)",  "450",
                                        "Bicycle (Yellow)",	"713",
                                        "Common Stone",     "698",
                                        "DNA Splicers (Fuse)","628",
                                        "DNA Splicers (Split)","629",
                                        "Dowsing MCHN",	    "471",
                                        "Elevator Key",	    "700",
                                        "Exp. Share",	    "216",
                                        "Good Rod",         "446",
                                        "Gracidea",         "466",
                                        "Holo Caster (M)",  "641",
                                        "Holo Caster (F)",  "714",
                                        "Honor of Kalos",	"702",
                                        "Intriguing Stone",	"697",
                                        "Lens Case",	    "705",
                                        "Looker Ticket",	"712",
                                        "Mega Ring",	    "696",
                                        "Old Rod",          "445",
                                        "Oval Charm",	    "631",
                                        "Poké Radar",	    "431",
                                        "Power Plant Pass",	"695",
                                        "Prof's Letter",	"642",
                                        "Reveal Glass",     "638",
                                        "Roller Skates",	"643",
                                        "Shiny Charm",	    "632",
                                        "Sprinklotad",	    "689",
                                        "Super Rod",	    "447",
                                        "TMV Pass",	        "701",
                                        "Town Map",	        "442",
                                        "Travel Trunk (Silver)","706",
                                        "Travel Trunk (Gold)","707",
                                        "Vs. Recorder",	    "465",
                                       };
        }
        private void getListTHMH()
        {
            tmhm_val = new string[] {   "(None)",	        "0",
                                        "TM01",	"328",
                                        "TM02",	"329",
                                        "TM03",	"330",
                                        "TM04",	"331",
                                        "TM05",	"332",
                                        "TM06",	"333",
                                        "TM07",	"334",
                                        "TM08",	"335",
                                        "TM09",	"336",
                                        "TM10",	"337",
                                        "TM11",	"338",
                                        "TM12",	"339",
                                        "TM13",	"340",
                                        "TM14",	"341",
                                        "TM15",	"342",
                                        "TM16",	"343",
                                        "TM17",	"344",
                                        "TM18",	"345",
                                        "TM19",	"346",
                                        "TM20",	"347",
                                        "TM21",	"348",
                                        "TM22",	"349",
                                        "TM23",	"350",
                                        "TM24",	"351",
                                        "TM25",	"352",
                                        "TM26",	"353",
                                        "TM27",	"354",
                                        "TM28",	"355",
                                        "TM29",	"356",
                                        "TM30",	"357",
                                        "TM31",	"358",
                                        "TM32",	"359",
                                        "TM33",	"360",
                                        "TM34",	"361",
                                        "TM35",	"362",
                                        "TM36",	"363",
                                        "TM37",	"364",
                                        "TM38",	"365",
                                        "TM39",	"366",
                                        "TM40",	"367",
                                        "TM41",	"368",
                                        "TM42",	"369",
                                        "TM43",	"370",
                                        "TM44",	"371",
                                        "TM45",	"372",
                                        "TM46",	"373",
                                        "TM47",	"374",
                                        "TM48",	"375",
                                        "TM49",	"376",
                                        "TM50",	"377",
                                        "TM51",	"378",
                                        "TM52",	"379",
                                        "TM53",	"380",
                                        "TM54",	"381",
                                        "TM55",	"382",
                                        "TM56",	"383",
                                        "TM57",	"384",
                                        "TM58",	"385",
                                        "TM59",	"386",
                                        "TM60",	"387",
                                        "TM61",	"388",
                                        "TM62",	"389",
                                        "TM63",	"390",
                                        "TM64",	"391",
                                        "TM65",	"392",
                                        "TM66",	"393",
                                        "TM67",	"394",
                                        "TM68",	"395",
                                        "TM69",	"396",
                                        "TM70",	"397",
                                        "TM71",	"398",
                                        "TM72",	"399",
                                        "TM73",	"400",
                                        "TM74",	"401",
                                        "TM75",	"402",
                                        "TM76",	"403",
                                        "TM77",	"404",
                                        "TM78",	"405",
                                        "TM79",	"406",
                                        "TM80",	"407",
                                        "TM81",	"408",
                                        "TM82",	"409",
                                        "TM83",	"410",
                                        "TM84",	"411",
                                        "TM85",	"412",
                                        "TM86",	"413",
                                        "TM87",	"414",
                                        "TM88",	"415",
                                        "TM89",	"416",
                                        "TM90",	"417",
                                        "TM91",	"418",
                                        "TM92",	"419",
                                        "TM93",	"618",
                                        "TM94",	"619",
                                        "TM95",	"620",
                                        "TM96",	"690",
                                        "TM97",	"691",
                                        "TM98",	"692",
                                        "TM99",	"693",
                                        "TM100","694",
                                        "HM01",	"420",
                                        "HM02",	"421",
                                        "HM03",	"422",
                                        "HM04",	"423",
                                        "HM05",	"424",

                                    };
        }
        private void getListMedicine()
        {
            medicine_val = new string[] { "(None)",	        "0",
                                            "Ability Capsule",	"645",
                                            "Antidote",	        "18",
                                            "Awakening",	    "21",
                                            "Berry Juice",	    "43",
                                            "Burn Heal",	    "19",
                                            "Calcium",	        "49",
                                            "Carbos",	        "48",
                                            "Casteliacone",	    "591",
                                            "Clever Wing",	    "569",
                                            "Elixir",	        "40",
                                            "Energy Powder",	"34",
                                            "Energy Root",	    "35",
                                            "Ether",	        "38",
                                            "Fresh Water",	    "30",
                                            "Full Heal",	    "27",
                                            "Full Restore",	    "23",
                                            "Genius Wing",	    "568",
                                            "Heal Powder",	    "36",
                                            "Health Wing",	    "565",
                                            "HP Up",	        "45",
                                            "Hyper Potion",	    "25",
                                            "Ice Heal",	        "20",
                                            "Iron",	            "47",
                                            "Lava Cookie",	    "42",
                                            "Lemonade",	        "32",
                                            "Lumiose Galette",	"708",
                                            "Max Elixir",	    "41",
                                            "Max Ether",	    "39",
                                            "Max Potion",	    "24",
                                            "Max Revive",	    "29",
                                            "Moomoo Milk",	    "33",
                                            "Muscle Wing",	    "566",
                                            "Old Gateau",	    "54",
                                            "Paralyze Heal",	"22",
                                            "Potion",	        "17",
                                            "PP Max",	        "53",
                                            "PP Up",	        "51",
                                            "Protein",	        "46",
                                            "Pretty Wing",      "571",
                                            "Rage Candy Bar",	"504",
                                            "Rare Candy",	    "50",
                                            "Resist Wing",	    "567",
                                            "Revival Herb",	    "37",
                                            "Revive",	        "28",
                                            "Sacred Ash",	    "44",
                                            "Shalour Sable",	"709",
                                            "Soda Pop",	        "31",
                                            "Super Potion",	    "26",
                                            "Sweet Heart",	    "134",
                                            "Swift Wing",	    "570",
                                            "Zinc",	            "52",
                                        };
        }
        private void getListBerries()
        {
            berries_val = new string[] { "(None)",	        "0",
                                            "Aguav Berry",	"162",
                                            "Apicot Berry",	"205",
                                            "Aspear Berry",	"153",
                                            "Babiri Berry",	"199",
                                            "Belue Berry",	"183",
                                            "Bluk Berry",	"165",
                                            "Charti Berry",	"195",
                                            "Cheri Berry",	"149",
                                            "Chesto Berry",	"150",
                                            "Chilan Berry",	"200",
                                            "Chople Berry",	"189",
                                            "Coba Berry",	"192",
                                            "Colbur Berry",	"198",
                                            "Cornn Berry",	"175",
                                            "Custap Berry",	"210",
                                            "Durin Berry",	"182",
                                            "Enigma Berry",	"208",
                                            "Figy Berry",	"159",
                                            "Ganlon Berry",	"202",
                                            "Grepa Berry",	"173",
                                            "Haban Berry",	"197",
                                            "Hondew Berry",	"172",
                                            "Iapapa Berry",	"163",
                                            "Jaboca Berry",	"211",
                                            "Kasib Berry",	"196",
                                            "Kebia Berry",	"190",
                                            "Kee Berry",	"687",
                                            "Kelpsy Berry",	"170",
                                            "Lansat Berry",	"206",
                                            "Leppa Berry",	"154",
                                            "Liechi Berry",	"201",
                                            "Lum Berry",	"157",
                                            "Mago Berry",	"161",
                                            "Magost Berry",	"176",
                                            "Maranga Berry","688",
                                            "Micle Berry",	"209",
                                            "Nanab Berry",	"166",
                                            "Nomel Berry",	"178",
                                            "Occa Berry",	"184",
                                            "Oran Berry",	"155",
                                            "Pamtre Berry",	"180",
                                            "Passho Berry",	"185",
                                            "Payapa Berry",	"193",
                                            "Pecha Berry",	"151",
                                            "Persim Berry",	"156",
                                            "Petaya Berry",	"204",
                                            "Pinap Berry",	"168",
                                            "Pomeg Berry",	"169",
                                            "Qualot Berry",	"171",
                                            "Rabuta Berry",	"177",
                                            "Rawst Berry",	"152",
                                            "Razz Berry",	"164",
                                            "Rindo Berry",	"187",
                                            "Roseli Berry",	"686",
                                            "Rowap Berry",	"212",
                                            "Salac Berry",	"203",
                                            "Shuca Berry",	"191",
                                            "Sitrus Berry",	"158",
                                            "Spelon Berry",	"179",
                                            "Starf Berry",	"207",
                                            "Tamato Berry",	"174",
                                            "Tanga Berry",	"194",
                                            "Wacan Berry",	"186",
                                            "Watmel Berry",	"181",
                                            "Wepear Berry",	"167",
                                            "Wiki Berry",	"160",
                                            "Yache Berry",	"188",
                                       };
        }

        // Populate DataGrid
        private void popItems()
        {
            int offset = 0x05800 + shiftval;
            populateList(item_val, offset, 200); // max 400
        }
        private void popKeyItems()
        {
            int offset = 0x05E40 + shiftval;
            populateList(keyitem_val, offset, 50); // max 96
        }
        private void popTMHM()
        {
            int offset = 0x05FC0 + shiftval;
            populateList(tmhm_val, offset, 105); // 106 total tho
        }
        private void popMedicine()
        {
            int offset = 0x06168 + shiftval;
            populateList(medicine_val, offset, 60); // 64 total slots 
        }
        private void popBerries()
        {
            int offset = 0x06268 + shiftval;
            populateList(berries_val, offset, 70); // 102 slots
        }

        private void populateList(string[] itemarr, int offset, int itemcount)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "CNT";
                dgvIndex.DisplayIndex = 1;
                dgvIndex.Width = 45;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvItemVal = new DataGridViewComboBoxColumn();
            dgvItemVal.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            {
                for (int i = 0; i < itemarr.Length/2; i++)
                {
                    dgvItemVal.Items.Add(itemarr[i*2]); // add only the Item Names, not Values
                }
                dgvItemVal.DisplayIndex = 0;
                dgvItemVal.Width = 135;
                dgvItemVal.FlatStyle = FlatStyle.Flat;
            }
            dataGridView1.Columns.Add(dgvItemVal);
            dataGridView1.Columns.Add(dgvIndex);

            dataGridView1.Rows.Add(itemcount);

            string itemname = "";
            for (int i = 0; i < itemcount; i++)
            {
                int itemvalue = BitConverter.ToUInt16(sav, offset + i*4);
                int itemposition = Array.IndexOf(itemarr, itemvalue.ToString());
                try
                {
                    itemname = itemarr[itemposition - 1];
                }
                catch
                {
                    MessageBox.Show("Unknown item detected.\r\n\r\nItem ID: " + itemvalue + "\r\n" + "Item is after: " + itemname, "Error");
                    continue;
                }
                int itemarrayval = itemposition - 1;
                dataGridView1.Rows[i].Cells[0].Value = itemarr[itemarrayval];
                dataGridView1.Rows[i].Cells[1].Value = BitConverter.ToUInt16(sav, offset + i * 4 + 2);
            }
        }
        private void dropclick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
                comboBox.DroppedDown = true;
            }
        }
        private void saveBag(object sender)
        {
            string[] itemstrarr = {};
            int offset = 0;
            if (B_DisplayItems.ForeColor == Color.Red)
            {
                itemstrarr = item_val;
                offset = 0x05800 + shiftval;
            }
            else if (B_DisplayKeyItems.ForeColor == Color.Red)
            {
                itemstrarr = keyitem_val;
                offset = 0x05E40 + shiftval;
            }
            else if (B_DisplayTMHM.ForeColor == Color.Red)
            {
                itemstrarr = tmhm_val;
                offset = 0x05FC0 + shiftval;
            }
            else if (B_DisplayMedicine.ForeColor == Color.Red)
            {
                itemstrarr = medicine_val;
                offset = 0x06168 + shiftval;
            }
            else if (B_DisplayBerries.ForeColor == Color.Red)
            {
                itemstrarr = berries_val;
                offset = 0x06268 + shiftval;
            }

            // Fetch Data
            int itemcount = dataGridView1.Rows.Count;
            int emptyslots = 0;
            for (int i = 0; i < itemcount; i++)
            {
                string item = dataGridView1.Rows[i].Cells[0].Value.ToString();
                string itemindexstr = itemstrarr[Array.IndexOf(itemstrarr, item) + 1];
                int itemindex = Convert.ToUInt16(itemindexstr);
                int itemcnt = Convert.ToUInt16(dataGridView1.Rows[i].Cells[1].Value.ToString());

                if (itemindex == 0) // Compression of Empty Slots
                {
                    emptyslots++;
                    continue;
                }

                // Write Data into Save File
                Array.Copy(BitConverter.GetBytes(itemindex), 0, sav, offset + 4 * (i - emptyslots), 2);
                Array.Copy(BitConverter.GetBytes(itemcnt), 0, sav, offset + 4 * (i - emptyslots) + 2, 2);
            }

            // Delete Empty Trash
            for (int i = itemcount - emptyslots; i < itemcount; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, sav, offset + 4 * (i), 2);
                Array.Copy(BitConverter.GetBytes(0), 0, sav, offset + 4 * (i) + 2, 2);
            }

            // Load New Button Color, after finished we'll load the new data.
            Button btn = sender as Button;
            B_DisplayItems.ForeColor = Color.Black;
            B_DisplayKeyItems.ForeColor = Color.Black;
            B_DisplayTMHM.ForeColor = Color.Black;
            B_DisplayMedicine.ForeColor = Color.Black;
            B_DisplayBerries.ForeColor = Color.Black;

            btn.ForeColor = Color.Red;
        }

        private void B_DisplayItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popItems();
        }
        private void B_DisplayKeyItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popKeyItems();
        }
        private void B_DisplayTMHM_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popTMHM();
        }
        private void B_DisplayMedicine_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popMedicine();
        }
        private void B_DisplayBerries_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popBerries();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            saveBag(sender);
            Array.Copy(sav, m_parent.savefile, 0x100000);
            m_parent.savedited = true;
            Close();
        }
    }
}
