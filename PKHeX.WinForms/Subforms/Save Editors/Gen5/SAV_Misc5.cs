using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms
{
    public partial class SAV_Misc5 : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV5 SAV;

        public SAV_Misc5(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV5)(Origin = sav).Clone();
            ReadMain();
            if (SAV is SAV5B2W2)
                ReadEntralink();
            else TC_Misc.Controls.Remove(TAB_Entralink);
            LoadForest();
            ReadSubway();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveMain();
            if (SAV is SAV5B2W2)
                SaveEntralink();
            SaveForest();
            SaveSubway();
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private ComboBox[] cbr = null!;
        private int ofsFly;
        private int[] FlyDestC = null!;
        private const int ofsRoamer = 0x21B00;
        private const int ofsLibPass = 0x212BC;
        private const uint keyLibPass = 0x0132B536;
        private uint valLibPass;
        private bool bLibPass;
        private const int ofsKS = 0x25828;

        private readonly uint[] keyKS = {
            // 0x34525, 0x11963,           // Selected City
            // 0x31239, 0x15657, 0x49589,  // Selected Difficulty
            // 0x94525, 0x81963, 0x38569,  // Selected Mystery Door
            0x35691, 0x18256, 0x59389, 0x48292, 0x09892, // Obtained Keys(EasyMode, Challenge, City, Iron, Iceberg)
            0x93389, 0x22843, 0x34771, 0xAB031, 0xB3818 // Unlocked(EasyMode, Challenge, City, Iron, Iceberg)
        };

        private uint[] valKS = null!;
        private bool[] bKS = null!;

        private void ReadMain()
        {
            string[]? FlyDestA;
            switch (SAV.Version)
            {
                case GameVersion.B or GameVersion.W or GameVersion.BW:
                    ofsFly = 0x204B2;
                    FlyDestA = new[] {
                        "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
                        "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
                        "Icirrus City", "Opelucid City", "Victory Road", "Pokemon League",
                        "Lacunosa Town", "Undella Town", "Black City/White Forest", "(Unity Tower)"
                    };
                    FlyDestC = new[] {
                        0, 1, 2, 3,
                        4, 5, 6, 7,
                        8, 9, 15, 11,
                        10, 13, 12, 14
                    };
                    break;
                case GameVersion.B2 or GameVersion.W2 or GameVersion.B2W2:
                    ofsFly = 0x20392;
                    FlyDestA = new[] {
                        "Aspertia City", "Floccesy Town", "Virbank City",
                        "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
                        "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
                        "Icirrus City", "Opelucid City",
                        "Lacunosa Town", "Undella Town", "Black City/White Forest",
                        "Lentimas Town", "Humilau City", "Victory Road", "Pokemon League",
                        "Pokestar Studios", "Join Avenue", "PWT", "(Unity Tower)"
                    };
                    FlyDestC = new[] {
                        24, 27, 25,
                        8, 9, 10, 11,
                        12, 13, 14, 15,
                        16, 17,
                        18, 21, 20,
                        28, 26, 66, 19,
                        5, 6, 7, 22
                    };
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(SAV.Version));
            }
            uint valFly = BitConverter.ToUInt32(SAV.Data, ofsFly);
            CLB_FlyDest.Items.Clear();
            CLB_FlyDest.Items.AddRange(FlyDestA);
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            {
                if (FlyDestC[i] < 32)
                    CLB_FlyDest.SetItemChecked(i, (valFly & 1u << FlyDestC[i]) != 0);
                else
                    CLB_FlyDest.SetItemChecked(i, (SAV.Data[ofsFly + (FlyDestC[i] >> 3)] & 1 << (FlyDestC[i] & 7)) != 0);
            }

            if (SAV is SAV5BW)
            {
                GB_KeySystem.Visible = false;
                // Roamer
                cbr = new[] { CB_Roamer642, CB_Roamer641 };
                // CurrentStat:ComboboxSource
                // Not roamed: Not roamed/Defeated/Captured
                //    Roaming: Roaming/Defeated/Captured
                //   Defeated: Defeated/Captured
                //   Captured: Defeated/Captured
                // Top 2 bit acts as flags of some sorts
                for (int i = 0; i < cbr.Length; i++)
                {
                    int c = SAV.Data[ofsRoamer + 0x2E + i];
                    var states = GetStates();
                    if (states.All(z => z.Value != c))
                        states.Add(new ComboItem($"Unknown (0x{c:X2})", c));
                    cbr[i].Items.Clear();
                    cbr[i].InitializeBinding();
                    cbr[i].DataSource = new BindingSource(states.Where(v => v.Value >= 2 || v.Value == c).ToList(), null);
                    cbr[i].SelectedValue = c;
                }

                // LibertyPass
                valLibPass = keyLibPass ^ (uint)(SAV.SID << 16 | SAV.TID);
                bLibPass = BitConverter.ToUInt32(SAV.Data, ofsLibPass) == valLibPass;
                CHK_LibertyPass.Checked = bLibPass;
            }
            else if (SAV is SAV5B2W2)
            {
                GB_Roamer.Visible = CHK_LibertyPass.Visible = false;
                // KeySystem
                string[] KeySystemA =
                {
                    "Obtain EasyKey", "Obtain ChallengeKey", "Obtain CityKey", "Obtain IronKey", "Obtain IcebergKey",
                    "Unlock EasyMode", "Unlock ChallengeMode", "Unlock City", "Unlock IronChamber",
                    "Unlock IcebergChamber"
                };
                uint KSID = BitConverter.ToUInt32(SAV.Data, ofsKS + 0x34);
                valKS = new uint[keyKS.Length];
                bKS = new bool[keyKS.Length];
                CLB_KeySystem.Items.Clear();
                for (int i = 0; i < valKS.Length; i++)
                {
                    valKS[i] = keyKS[i] ^ KSID;
                    bKS[i] = BitConverter.ToUInt32(SAV.Data, ofsKS + (i << 2)) == valKS[i];
                    CLB_KeySystem.Items.Add(KeySystemA[i], bKS[i]);
                }
            }
            else
            {
                GB_KeySystem.Visible = GB_Roamer.Visible = CHK_LibertyPass.Visible = false;
            }
        }

        private static List<ComboItem> GetStates()
        {
            return new()
            {
                new ComboItem("Not roamed", 0),
                new ComboItem("Roaming", 1),
                new ComboItem("Defeated", 2),
                new ComboItem("Captured", 3),
            };
        }

        private void SaveMain()
        {
            uint valFly = BitConverter.ToUInt32(SAV.Data, ofsFly);
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            {
                if (FlyDestC[i] < 32)
                {
                    if (CLB_FlyDest.GetItemChecked(i))
                        valFly |= (uint) 1 << FlyDestC[i];
                    else
                        valFly &= ~((uint) 1 << FlyDestC[i]);
                }
                else
                {
                    var ofs = ofsFly + (FlyDestC[i] >> 3);
                    SAV.Data[ofs] = (byte)((SAV.Data[ofs] & ~(1 << (FlyDestC[i] & 7))) | ((CLB_FlyDest.GetItemChecked(i) ? 1 : 0) << (FlyDestC[i] & 7)));
                }
            }
            BitConverter.GetBytes(valFly).CopyTo(SAV.Data, ofsFly);

            if (SAV is SAV5BW)
            {
                // Roamer
                for (int i = 0; i < cbr.Length; i++)
                {
                    int c = SAV.Data[ofsRoamer + 0x2E + i];
                    var d = (int)cbr[i].SelectedValue;

                    if (c == d)
                        continue;
                    SAV.Data[ofsRoamer + 0x2E + i] = (byte)d;
                    if (c != 1)
                        continue;
                    new byte[14].CopyTo(SAV.Data, ofsRoamer + 4 + (i * 0x14));
                    SAV.Data[ofsRoamer + 0x2C + i] = 0;
                }

                // LibertyPass
                if (CHK_LibertyPass.Checked ^ bLibPass)
                    BitConverter.GetBytes(bLibPass ? 0 : valLibPass).CopyTo(SAV.Data, ofsLibPass);
            }
            else if (SAV is SAV5B2W2)
            {
                // KeySystem
                for (int i = 0; i < CLB_KeySystem.Items.Count; i++)
                {
                    if (CLB_KeySystem.GetItemChecked(i) ^ bKS[i])
                        BitConverter.GetBytes(bKS[i] ? 0 : valKS[i]).CopyTo(SAV.Data, ofsKS + (i << 2));
                }
            }
        }

        private void B_AllFlyDest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
                CLB_FlyDest.SetItemChecked(i, true);
        }

        private void B_AllKeys_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_KeySystem.Items.Count; i++)
                CLB_KeySystem.SetItemChecked(i, true);
        }

        private readonly int[]?[] FMUnlockConditions = {
            null, // 00
            null, // 01
            new[] { 2444 }, // 02
            null, // 03
            new[] { 2445 }, // 04
            null, // 05
            new[] { 2462 }, // 06
            new[] { 2452, 2476 }, // 07
            new[] { 2476, 2548 }, // 08
            new[] { 2447 }, new[] { 2447 }, // 09
            new[] { 2453 }, new[] { 2453 }, // 10
            new[] { 2504 }, // 11
            new[] { 2457, 2507 }, // 12
            new[] { 2458, 2478 }, // 13
            new[] { 2456, 2508 }, // 14
            new[] { 2448 }, new[] { 2448 }, // 15
            new[] { 2549 }, // 16
            new[] { 2449 }, // 17
            new[] { 2479, 2513 }, // 18
            new[] { 2479, 2550 }, // 19
            new[] { 2481 }, // 20
            new[] { 2459 }, // 21
            new[] { 2454 }, // 22
            new[] { 2551 }, // 23
            new[] { 2400 }, // 24
            new[] { 2400 }, // 25
            new[] { 2400 }, new[] { 2400 }, // 26
            new[] { 2400 }, new[] { 2400 }, // 27
            new[] { 2400 }, // 28
            new[] { 2400, 2460 }, // 29
            new[] { 2400 }, // 30
            new[] { 2400, 2461 }, new[] { 2400, 2461 }, // 31
            new[] { 2437 }, // 32
            new[] { 2450 }, // 33
            new[] { 2451 }, // 34
            new[] { 2455 }, // 35
            new[] { 105 }, // 36
            new[] { 2400 }, // 37
            new[] { 2557 } // 38
        };

        private bool editing;
        private const int ofsFM = 0x25900;
        private NumericUpDown[] nudaE = null!, nudaF = null!;
        private ComboBox[] cba = null!;
        private ToolTip[] ta = null!;

        private void ReadEntralink()
        {
            editing = true;
            nudaE = new[] { NUD_EntreeWhiteLV, NUD_EntreeWhiteEXP, NUD_EntreeBlackLV, NUD_EntreeBlackEXP };
            ushort u;
            for (int i = 0; i < 2; i++)
            {
                u = BitConverter.ToUInt16(SAV.Data, 0x2120C + (i << 1));
                nudaE[i << 1].Value = u > 999 ? 999 : u;
                nudaE[(i << 1) + 1].Value = SAV.Data[ofsFM + 0xF8 + i];
            }

            string[] PassPowerA = {
                "(none)",
                "-1 Encounter", "-2 Encounter", "-3 Encounter", "+1 Encounter", "+2 Encounter", "+3 Encounter",
                "+1 Hatching", "+2 Hatching", "+3 Hatching", "S Hatching",
                "+1 Befriending", "+2 Befriending", "+3 Befriending", "S Befriending",
                "+1 Bargain", "+2 Bargain", "+3 Bargain", "S Bargain",
                "+1 HP(20)", "+2 HP(50)", "+3 HP(200)", "+1 PP(5)", "+2 PP(10)", "+3 PP(ALL)",
                "-1 Exp.", "-2 Exp.", "-3 Exp.", "+1 Exp.", "+2 Exp.", "+3 Exp.", "S Exp.",
                "+1 PrizeMoney", "+2 PrizeMoney", "+3 PrizeMoney", "S PrizeMoney",
                "+1 Capture", "+2 Capture", "+3 Capture", "S Capture",
                "+1 Search", "+2 Search", "+3 Search", "S Search",
                "+1 HiddenGrotto", "+2 HiddenGrotto", "+3 HiddenGrotto", "S HiddenGrotto",
                "+1 Charm", "+2 Charm", "+3 Charm", "S Charm",
                "(HP Full Recovery)", "(MAX Hatching)", "(MAX Bargain)", "(MAX Befriending)", "(MAX Exp.)", "(MAX PrizeMoney)", "(MAX Capture)", "(MAX Search)", "(MAX HiddenGrotto)", "(MAX Charm)"
            };
            int[] PassPowerC = {
                48,
                3, 4, 5, 0, 1, 2,
                6, 7, 8, 33,
                9, 10, 11, 35,
                12, 13, 14, 34,
                15, 16, 17, 18, 19, 20,
                24, 25, 26, 21, 22, 23, 36,
                27, 28, 29, 37,
                30, 31, 32, 38,
                49, 50, 51, 58,
                52, 53, 54, 60,
                55, 56, 57, 62,
                39, 40, 41, 42, 43, 44, 45, 59, 61, 63
            };
            ComboItem[] PassPowerB = PassPowerA.Zip(PassPowerC, (f, s) => new ComboItem(f, s)).ToArray();
            cba = new[] { CB_PassPower1, CB_PassPower2, CB_PassPower3 };
            for (int i = 0; i < cba.Length; i++)
            {
                cba[i].Items.Clear();
                cba[i].InitializeBinding();
                cba[i].DataSource = new BindingSource(PassPowerB, null);
                cba[i].SelectedValue = (int)SAV.Data[0x213A0 + i];
            }

            nudaF = new[] { NUD_FMHosted, NUD_FMParticipated, NUD_FMCompleted, NUD_FMTopScores };
            for (int i = 0; i < nudaF.Length; i++)
            {
                u = BitConverter.ToUInt16(SAV.Data, ofsFM + 0xF0 + (i << 1));
                nudaF[i].Value = u > 9999 ? 9999 : u;
            }
            NUD_FMMostParticipants.Value = SAV.Data[ofsFM + 0xFA];

            string[] FMTitles = {
                "00 The First Berry Search!",
                "01 Collect Berries!",
                "02 Find Lost Items!",
                "03 Find Lost Boys!",
                "04 Enjoy Shopping!",
                "05 Find Audino!",
                "06 Search for 3 Pokemon!",
                "07 Train with Martial Artists!",
                "08 Sparring with 10 Trainers!",
                "09B Get Rich Quick!",
                "09W Treasure Hunting!",
                "10B Exciting Trading!",
                "10W Exhilarating Trading!",
                "11 Find Emolga!",
                "12 Wings Falling on the Drawbridge!",
                "13 Find Treasures!",
                "14 Mushrooms Hide-and-Seek!",
                "15B Find Mysterious Ores!",
                "15W Find Shining Ores!",
                "16 The 2 Lost Treasures",
                "17 Big Harvest of Berries!",
                "18 Ring the Bell...",
                "19 The Bell that Rings 3 Times",
                "20 Path to an Ace!",
                "21 Shocking Shopping!",
                "22 Memory Training!",
                "23 Push the Limit of Your Memory...",
                "24 Find Rustling Grass!",
                "25 Find Shards!",
                "26B Forgotten Lost Items",
                "26W Not-Found Lost Items",
                "27B What is the Best Price?",
                "27W What is the Real Price?",
                "28 Give me the Item!",
                "29 Do a Great Trade-Up!",
                "30 Search Hidden Grottes!",
                "31B Noisy Hidden Grottes!",
                "31W Quiet Hidden Grottes!",
                "32 Fishing Competition!",
                "33 Mulch Collector!",
                "34 Where are Fluttering Hearts?",
                "35 Rock-Paper-Scissors Competition!",
                "36 Take a Walk with Eggs!",
                "37 Find Steelix!",
                "38 The Berry-Hunting Adventure!"
            };
            LB_FunfestMissions.Items.Clear();
            LB_FunfestMissions.Items.AddRange(FMTitles);

            CB_FMLevel.Items.Clear();
            CB_FMLevel.Items.AddRange(new[] {"Lv.1", "Lv.2 +", "Lv.3 ++", "Lv.3 +++"});
            ta = new[] { TipExpW, TipExpB };
            SetNudMax();
            SetEntreeExpTooltip();
            editing = false;
        }

        private void SaveEntralink()
        {
            for (int i = 0; i < 2; i++)
            {
                BitConverter.GetBytes((ushort)nudaE[i << 1].Value).CopyTo(SAV.Data, 0x2120C + (i << 1));
                SAV.Data[ofsFM + 0xF8 + i] = (byte)nudaE[(i << 1) + 1].Value;
            }
            for (int i = 0; i < cba.Length; i++)
            {
                if (cba[i].SelectedIndex < 0) continue;
                var j = (int)cba[i].SelectedValue;
                SAV.Data[0x213A0 + i] = (byte)j;
            }
            for (int i = 0; i < nudaF.Length; i++)
                BitConverter.GetBytes((ushort)nudaF[i].Value).CopyTo(SAV.Data, ofsFM + 0xF0 + (i << 1));
            SAV.Data[ofsFM + 0xFA] = (byte)NUD_FMMostParticipants.Value;
        }

        private void SetEntreeExpTooltip(bool? isBlack = null)
        {
            for (int i = 0; i < 2; i++)
            {
                if (isBlack == true) continue;
                var lv = (int)nudaE[i << 1].Value;
                int exp;
                if (lv < 9)
                    exp = lv * (lv + 1) * 5 / 2;
                else
                    exp = ((lv - 9) * 50) + 225;
                exp += (int)nudaE[(i << 1) + 1].Value;
                var lvl = lv == 999 ? -1 : nudaE[(i << 1) + 1].Maximum - nudaE[(i << 1) + 1].Value + 1;
                var tip0 = $"{(i == 0 ? "White" : "Black")} LV {lv}{Environment.NewLine}" +
                           $"Exp.Points: {exp}{Environment.NewLine}" +
                           $"To Next Lv: {lvl}";
                ta[i].RemoveAll();
                ta[i].SetToolTip(nudaE[i << 1], tip0);
                ta[i].SetToolTip(nudaE[(i << 1) + 1], tip0);
            }
        }

        private void SetNudMax(bool? isBlack = null)
        {
            for (int i = 0; i < 2; i++)
            {
                if (isBlack == true)
                    continue;
                var lv = (int)nudaE[i << 1].Value;
                var expmax = lv > 8 ? 49 : (lv * 5) + 4;
                if (nudaE[(i << 1) + 1].Value > expmax)
                    nudaE[(i << 1) + 1].Value = expmax;
                nudaE[(i << 1) + 1].Maximum = expmax;
            }
        }

        private void SetFMVal(int ofsB, int len, uint val)
        {
            int s = LB_FunfestMissions.SelectedIndex;
            if ((uint)s >= FMUnlockConditions.Length)
                return;
            BitConverter.GetBytes((BitConverter.ToUInt32(SAV.Data, ofsFM + (s << 2)) & ~(~(uint)0 >> (32 - len) << ofsB)) | val << ofsB).CopyTo(SAV.Data, ofsFM + (s << 2));
        }

        private void LB_FunfestMissions_SelectedIndexChanged(object sender, EventArgs e)
        {
            int s = LB_FunfestMissions.SelectedIndex;
            if ((uint)s >= FMUnlockConditions.Length)
                return;
            editing = true;
            bool FirstMissionCleared = (SAV.Data[0x2025E + (2438 >> 3)] & 1 << (2438 & 7)) != 0;
            L_FMUnlocked.Visible = s == 0 ? !FirstMissionCleared : FirstMissionCleared && FMUnlockConditions[s]?.All(v => (SAV.Data[0x2025E + (v >> 3)] & 1 << (v & 7)) != 0) != false;
            L_FMLocked.Visible = !L_FMUnlocked.Visible;
            uint u = BitConverter.ToUInt32(SAV.Data, ofsFM + (s << 2));
            CHK_FMNew.Checked = u >> 31 != 0;
            CB_FMLevel.SelectedIndex = (int)(u << 2 >> 30);
            int i = (int)(u << 4 >> 18);
            NUD_FMBestScore.Value = i > 9999 ? 9999 : i;
            i = (int)(u & 0x3FF);
            NUD_FMBestTotal.Value = i > 9999 ? 9999 : i;
            editing = false;
        }

        private void CHK_FMNew_CheckedChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SetFMVal(31, 1, CHK_FMNew.Checked ? 1 : 0);
        }

        private void CB_FMLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SetFMVal(28, 3, (uint)(CB_FMLevel.SelectedIndex & 3));
        }

        private void NUD_FMBestScore_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SetFMVal(14, 14, (uint)NUD_FMBestScore.Value);
        }

        private void NUD_FMBestTotal_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SetFMVal(0, 14, (uint)NUD_FMBestTotal.Value);
        }

        private void B_FunfestMissions_Click(object sender, EventArgs e)
        {
            const int FunfestFlag = 2438;
            SAV.Data[0x2025E + (FunfestFlag >> 3)] |= 1 << (FunfestFlag & 7);
            foreach (var ia in FMUnlockConditions)
            {
                if (ia == null)
                    continue;
                foreach (var index in ia)
                    SAV.Data[0x2025E + (index >> 3)] |= (byte)(1 << (index & 7));
            }

            L_FMUnlocked.Visible = true;
            L_FMLocked.Visible = false;
        }

        private void NUD_EntreeBlackLV_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SetNudMax(isBlack: true);
            SetEntreeExpTooltip(isBlack: true);
        }

        private void NUD_EntreeWhiteLV_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SetNudMax(isBlack: false);
            SetEntreeExpTooltip(isBlack: false);
        }

        private void NUD_EntreeBlackEXP_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SetEntreeExpTooltip(isBlack: true);
        }

        private void NUD_EntreeWhiteEXP_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            SetEntreeExpTooltip(isBlack: false);
        }

        private EntreeForest Forest = null!;
        private IList<EntreeSlot> AllSlots = null!;

        private void LoadForest()
        {
            Forest = SAV.EntreeData;
            AllSlots = Forest.Slots;
            NUD_Unlocked.Value = Forest.Unlock38Areas + 2;
            CHK_Area9.Checked = Forest.Unlock9thArea;

            var areas = AllSlots.Select(z => z.Area).Distinct()
                .Select(z => new ComboItem(z.ToString(), (int) z)).ToList();

            CB_Species.InitializeBinding();
            CB_Move.InitializeBinding();
            CB_Areas.InitializeBinding();

            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList(), null);
            CB_Move.DataSource = new BindingSource(GameInfo.MoveDataSource, null);
            CB_Areas.DataSource = new BindingSource(areas, null);

            CB_Areas.SelectedIndex = 0;
        }

        private void SaveForest()
        {
            Forest.Unlock38Areas = (int) NUD_Unlocked.Value - 2;
            Forest.Unlock9thArea = CHK_Area9.Checked;
            SAV.EntreeData = Forest;
        }

        private IList<EntreeSlot> CurrentSlots = null!;
        private int currentIndex = -1;

        private void ChangeArea(object sender, EventArgs e)
        {
            var area = WinFormsUtil.GetIndex(CB_Areas);
            CurrentSlots = AllSlots.Where(z => (int) z.Area == area).ToArray();
            LB_Slots.Items.Clear();
            foreach (var z in CurrentSlots.Select(z => GameInfo.Strings.Species[z.Species]))
                LB_Slots.Items.Add(z);
            LB_Slots.SelectedIndex = currentIndex = 0;
        }

        private void ChangeSlot(object sender, EventArgs e)
        {
            CurrentSlot = null;
            if (LB_Slots.SelectedIndex >= 0)
                currentIndex = LB_Slots.SelectedIndex;
            var current = CurrentSlots[currentIndex];
            CB_Species.SelectedValue = current.Species;
            SetForms(current);
            SetGenders(current);
            CB_Move.SelectedValue = current.Move;
            CB_Gender.SelectedValue = current.Gender;
            CB_Form.SelectedIndex = current.Form;
            CurrentSlot = current;
            SetSprite(current);
        }

        private EntreeSlot? CurrentSlot;

        private void UpdateSlotValue(object sender, EventArgs e)
        {
            if (CurrentSlot == null)
                return;

            if (sender == CB_Species)
            {
                CurrentSlot.Species = WinFormsUtil.GetIndex(CB_Species);
                LB_Slots.Items[currentIndex] = GameInfo.Strings.Species[CurrentSlot.Species];
                SetForms(CurrentSlot);
                SetGenders(CurrentSlot);
            }
            else if (sender == CB_Move)
            {
                CurrentSlot.Move = WinFormsUtil.GetIndex(CB_Move);
            }
            else if (sender == CB_Gender)
            {
                CurrentSlot.Gender = WinFormsUtil.GetIndex(CB_Gender);
            }
            else if (sender == CB_Form)
            {
                CurrentSlot.Form = CB_Form.SelectedIndex;
            }
            else if (sender == CHK_Invisible)
            {
                CurrentSlot.Invisible = CHK_Invisible.Checked;
            }
            else if (sender == NUD_Animation)
            {
                CurrentSlot.Animation = (int)NUD_Animation.Value;
            }

            SetSprite(CurrentSlot);
        }

        private void SetSprite(EntreeSlot slot)
        {
            PB_SlotPreview.Image = SpriteUtil.GetSprite(slot.Species, slot.Form, slot.Gender, 0, 0, false, false);
        }

        private void SetGenders(EntreeSlot slot)
        {
            CB_Gender.InitializeBinding();
            CB_Gender.DataSource = new BindingSource(GetGenderChoices(slot.Species), null);
        }

        private void B_RandForest_Click(object sender, EventArgs e)
        {
            var source = (SAV is SAV5B2W2 ? Encounters5.DreamWorld_BW : Encounters5.DreamWorld_B2W2).Concat(Encounters5.DreamWorld_Common).ToList();
            var rnd = Util.Rand;
            foreach (var s in AllSlots)
            {
                int index = rnd.Next(source.Count);
                var slot = source[index];
                source.Remove(slot);
                s.Species = slot.Species;
                s.Form = slot.Form;
                s.Move = slot.Moves.Count > 0 ? slot.Moves[rnd.Next(slot.Moves.Count)] : 0;
                s.Gender = slot.Gender == -1 ? PersonalTable.B2W2[slot.Species].RandomGender() : slot.Gender;
            }
            ChangeArea(this, EventArgs.Empty); // refresh
            NUD_Unlocked.Value = 8;
            CHK_Area9.Checked = true;
            System.Media.SystemSounds.Asterisk.Play();
        }

        private static List<ComboItem> GetGenderChoices(int species)
        {
            var pi = PersonalTable.B2W2[species];
            var list = new List<ComboItem>();
            if (pi.Genderless)
            {
                list.Add(new ComboItem("Genderless", 2));
                return list;
            }

            if (!pi.OnlyFemale)
                list.Add(new ComboItem("Male", 0));
            if (!pi.OnlyMale)
                list.Add(new ComboItem("Female", 1));
            return list;
        }

        private void SetForms(EntreeSlot slot)
        {
            bool hasForms = PersonalTable.B2W2[slot.Species].HasForms || slot.Species == (int)Species.Mothim;
            L_Form.Visible = CB_Form.Enabled = CB_Form.Visible = hasForms;

            CB_Form.InitializeBinding();
            var list = FormConverter.GetFormList(slot.Species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Generation);
            CB_Form.DataSource = new BindingSource(list, null);
        }

        // Subway
        private BattleSubway5 sw = null!;

        private void ReadSubway()
        {
            sw = SAV.BattleSubway;

            // Figure out the Super Checks
            var swSuperCheck = sw.SuperCheck;
            if (swSuperCheck == 0x00)
            {
                CHK_SuperSingle.Checked = CHK_SuperDouble.Checked = CHK_SuperMulti.Checked = false;
            }
            else if (swSuperCheck >= 0x70) // 0x70 or anything else means all super enabled
            {
                CHK_SuperSingle.Checked = CHK_SuperDouble.Checked = CHK_SuperMulti.Checked = true;
            }
            else
            {
                if (swSuperCheck is 0x10 or 0x30 or 0x50) CHK_SuperSingle.Checked = true;
                if (swSuperCheck is 0x20 or 0x30 or 0x60) CHK_SuperDouble.Checked = true;
                if (swSuperCheck is 0x40 or 0x50 or 0x60) CHK_SuperMulti.Checked = true;
            }

            // Normal
            // Single
            NUD_SinglePast.Value = sw.SinglePast;
            NUD_SingleRecord.Value = sw.SingleRecord;

            // Double
            NUD_DoublePast.Value = sw.DoublePast;
            NUD_DoubleRecord.Value = sw.DoubleRecord;

            // Multi NPC
            NUD_MultiNpcPast.Value = sw.MultiNPCPast;
            NUD_MultiNpcRecord.Value = sw.MultiNPCRecord;

            // Multi Friends
            NUD_MultiFriendsPast.Value = sw.MultiFriendsPast;
            NUD_MultiFriendsRecord.Value = sw.MultiFriendsRecord;

            // Super
            // Single
            NUD_SSinglePast.Value = sw.SuperSinglePast;
            NUD_SSingleRecord.Value = sw.SuperSingleRecord;

            // Double
            NUD_SDoublePast.Value = sw.SuperDoublePast;
            NUD_SDoubleRecord.Value = sw.SuperDoubleRecord;

            // Multi NPC
            NUD_SMultiNpcPast.Value = sw.SuperMultiNPCPast;
            NUD_SMultiNpcRecord.Value = sw.SuperMultiNPCRecord;

            // Multi Friends
            NUD_SMultiFriendsPast.Value = sw.SuperMultiFriendsPast;
            NUD_SMultiFriendsRecord.Value = sw.SuperMultiFriendsRecord;
        }

        private void SaveSubway()
        {
            // Save Super Checks
            sw.SuperCheck = ((CHK_SuperSingle.Checked ? 0x10 : 0x00) + (CHK_SuperDouble.Checked ? 0x20 : 0x00) + (CHK_SuperMulti.Checked ? 0x40 : 0x00));

            // Normal
            // Single
            sw.SinglePast = (int)NUD_SinglePast.Value;
            sw.SingleRecord = (int)NUD_SingleRecord.Value;

            // Double
            sw.DoublePast = (int)NUD_DoublePast.Value;
            sw.DoubleRecord = (int)NUD_DoubleRecord.Value;

            // Multi NPC
            sw.MultiNPCPast = (int)NUD_MultiNpcPast.Value;
            sw.MultiNPCRecord = (int)NUD_MultiNpcRecord.Value;

            // Multi Friends
            sw.MultiFriendsPast = (int)NUD_MultiFriendsPast.Value;
            sw.MultiFriendsRecord = (int)NUD_MultiFriendsRecord.Value;

            // Super
            // Single
            sw.SuperSinglePast = (int)NUD_SSinglePast.Value;
            sw.SuperSingleRecord = (int)NUD_SSingleRecord.Value;

            // Double
            sw.SuperDoublePast = (int)NUD_SDoublePast.Value;
            sw.SuperDoubleRecord = (int)NUD_SDoubleRecord.Value;

            // Multi NPC
            sw.SuperMultiNPCPast = (int)NUD_SMultiNpcPast.Value;
            sw.SuperMultiNPCRecord = (int)NUD_SMultiNpcRecord.Value;

            // Multi Friends
            sw.SuperMultiFriendsPast = (int)NUD_SMultiFriendsPast.Value;
            sw.SuperMultiFriendsRecord = (int)NUD_SMultiFriendsRecord.Value;
        }
    }
}
