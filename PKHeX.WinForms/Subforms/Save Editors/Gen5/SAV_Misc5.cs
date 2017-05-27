using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Misc5 : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV5 SAV;
        public SAV_Misc5(SaveFile sav)
        {
            SAV = (SAV5)(Origin = sav).Clone();
            InitializeComponent();
            readMain();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            saveMain();
            Origin.setData(SAV.Data, 0);
            Close();
        }

        private ComboBox[] cbr;
        private int ofsFly;
        private int[] FlyDestC;
        protected int ofsRoamer = 0x21B00;
        protected int ofsLibPass = 0x212BC;
        protected uint keyLibPass = 0x0132B536;
        private uint valLibPass;
        private bool bLibPass;
        protected int ofsKS = 0x25828;
        protected uint[] keyKS = new uint[] {
            // 0x34525, 0x11963,           // Selected City
            // 0x31239, 0x15657, 0x49589,  // Selected Difficulty
            // 0x94525, 0x81963, 0x38569,  // Selected Mystery Door
            0x35691, 0x18256, 0x59389, 0x48292, 0x09892, // Obtained Keys(EasyMode, Challenge, City, Iron, Iceberg)
            0x93389, 0x22843, 0x34771, 0xAB031, 0xB3818, // Unlocked(EasyMode, Challenge, City, Iron, Iceberg)
        };
        private uint[] valKS;
        private bool[] bKS;
        private void readMain()
        {
            string[] FlyDestA = null;
            switch (SAV.Version)
            {
                case GameVersion.BW:
                    ofsFly = 0x204B2;
                    FlyDestA = new[] {
                        "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
                        "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
                        "Icirrus City", "Opelucid City", "Victory Road", "Pokemon League",
                        "Lacunosa Town", "Undella Town", "Black City/White Forest", "(Unity Tower)",
                    };
                    FlyDestC = new[] {
                        0, 1, 2, 3,
                        4, 5, 6, 7,
                        8, 9, 15, 11,
                        10, 13, 12, 14,
                    };
                    break;
                case GameVersion.B2W2:
                    ofsFly = 0x20392;
                    FlyDestA = new[] {
                        "Aspertia City", "Floccesy Town", "Virbank City",
                        "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
                        "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
                        "Icirrus City", "Opelucid City",
                        "Lacunosa Town", "Undella Town", "Black City/White Forest",
                        "Lentimas Town", "Humilau City", "Victory Road", "Pokemon League",
                        "Pokestar Studios", "Join Avenue", "PWT", "(Unity Tower)",
                    };
                    FlyDestC = new[] {
                        24, 27, 25,
                        8, 9, 10, 11,
                        12, 13, 14, 15,
                        16, 17,
                        18, 21, 20,
                        28, 26, 66, 19,
                        5, 6, 7, 22,
                    };
                    break;
            }
            uint valFly = BitConverter.ToUInt32(SAV.Data, ofsFly);
            CLB_FlyDest.Items.Clear();
            CLB_FlyDest.Items.AddRange(FlyDestA);
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            {
                if (FlyDestC[i] < 32)
                    CLB_FlyDest.SetItemChecked(i, (valFly & (uint)1 << FlyDestC[i]) != 0);
                else
                    CLB_FlyDest.SetItemChecked(i, (SAV.Data[ofsFly + (FlyDestC[i] >> 3)] & 1 << (FlyDestC[i] & 7)) != 0);
            }

            if (SAV.BW)
            {
                GB_KeySystem.Visible = false;
                // Roamer
                cbr = new ComboBox[] { CB_Roamer642, CB_Roamer641 };
                ComboItem[] states = new[] {
                    new ComboItem { Text = "Not roamed", Value = 0, },
                    new ComboItem { Text = "Roaming", Value = 1, },
                    new ComboItem { Text = "Defeated", Value = 2, },
                    new ComboItem { Text = "Captured", Value = 3, },
                };
                // CurrentStat:ComboboxSource
                // Not roamed: Not roamed/Defeated/Captured
                //    Roaming: Roaming/Defeated/Captured
                //   Defeated: Defeated/Captured
                //   Captured: Defeated/Captured
                for (int i = 0, c; i < cbr.Length; i++)
                {
                    c = SAV.Data[ofsRoamer + 0x2E + i];
                    cbr[i].Items.Clear();
                    cbr[i].DisplayMember = "Text";
                    cbr[i].ValueMember = "Value";
                    cbr[i].DataSource = new BindingSource(states.Where(v => v.Value >= 2 || v.Value == c).ToList(), null);
                    cbr[i].SelectedValue = c;
                }

                // LibertyPass
                valLibPass = keyLibPass ^ (uint)(SAV.SID << 16 | SAV.TID);
                bLibPass = BitConverter.ToUInt32(SAV.Data, ofsLibPass) == valLibPass;
                CHK_LibertyPass.Checked = bLibPass;
            }
            else if (SAV.B2W2)
            {
                GB_Roamer.Visible = CHK_LibertyPass.Visible = false;
                // KeySystem
                string[] KeySystemA = new[] {
                    "Obtain EasyKey", "Obtain ChallengeKey", "Obtain CityKey", "Obtain IronKey", "Obtain IcebergKey",
                    "Unlock EasyMode", "Unlock ChallengeMode", "Unlock City", "Unlock IronChamber", "Unlock IcebergChamber",
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
            else GB_KeySystem.Visible = GB_Roamer.Visible = CHK_LibertyPass.Visible = false;
        }
        private void saveMain()
        {
            uint valFly = BitConverter.ToUInt32(SAV.Data, ofsFly);
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            {
                if (FlyDestC[i] < 32)
                {
                    if (CLB_FlyDest.GetItemChecked(i))
                        valFly |= (uint)1 << FlyDestC[i];
                    else
                        valFly &= ~((uint)1 << FlyDestC[i]);
                }
                else SAV.Data[ofsFly + (FlyDestC[i] >> 3)] = (byte)(SAV.Data[ofsFly + (FlyDestC[i] >> 3)] & ~(1 << (FlyDestC[i] & 7)) | ((CLB_FlyDest.GetItemChecked(i) ? 1 : 0) << (FlyDestC[i] & 7)));
            }
            BitConverter.GetBytes(valFly).CopyTo(SAV.Data, ofsFly);

            if (SAV.BW)
            {
                // Roamer
                for (int i = 0, c, d; i < cbr.Length; i++)
                {
                    c = SAV.Data[ofsRoamer + 0x2E + i];
                    d = (int)cbr[i].SelectedValue;
                    if (c != d)
                    {
                        SAV.Data[ofsRoamer + 0x2E + i] = (byte)d;
                        if (c == 1)
                        {
                            new byte[14].CopyTo(SAV.Data, ofsRoamer + 4 + i * 0x14);
                            SAV.Data[ofsRoamer + 0x2C + i] = 0;
                        }
                    }
                }

                // LibertyPass
                if (CHK_LibertyPass.Checked ^ bLibPass)
                    BitConverter.GetBytes(bLibPass ? (uint)0 : valLibPass).CopyTo(SAV.Data, ofsLibPass);
            }
            else if (SAV.B2W2)
            {
                // KeySystem
                for (int i = 0; i < CLB_KeySystem.Items.Count; i++)
                    if (CLB_KeySystem.GetItemChecked(i) ^ bKS[i])
                        BitConverter.GetBytes(bKS[i] ? (uint)0 : valKS[i]).CopyTo(SAV.Data, ofsKS + (i << 2));
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
    }
}
