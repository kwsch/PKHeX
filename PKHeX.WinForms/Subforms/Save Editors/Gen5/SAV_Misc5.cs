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
                    /* case GameVersion.B2W2:
                        ofsFly = 0;
                        FlyDestA = new[];
                        FlyDestC = new[];
                        break; */
            }
            ushort val = BitConverter.ToUInt16(SAV.Data, ofsFly);
            CLB_FlyDest.Items.Clear();
            CLB_FlyDest.Items.AddRange(FlyDestA);
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
                CLB_FlyDest.SetItemChecked(i, (val & 1 << FlyDestC[i]) != 0);

            if (SAV.BW)
            {
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
                CHK_LibertyPass.Checked = BitConverter.ToUInt32(SAV.Data, ofsLibPass) == valLibPass;
            }
            else
                GB_Roamer.Visible = CHK_LibertyPass.Visible = false;
        }
        private void saveMain()
        {
            ushort valFly = BitConverter.ToUInt16(SAV.Data, ofsFly);
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            {
                if (CLB_FlyDest.GetItemChecked(i))
                    valFly |= (ushort)(1 << FlyDestC[i]);
                else
                    valFly &= (ushort)~(1 << FlyDestC[i]);
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
                if (CHK_LibertyPass.Checked ^ BitConverter.ToUInt32(SAV.Data, ofsLibPass) == valLibPass)
                    BitConverter.GetBytes(CHK_LibertyPass.Checked ? valLibPass : (uint)0).CopyTo(SAV.Data, ofsLibPass);
            }
        }
        private void B_AllFlyDest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
                CLB_FlyDest.SetItemChecked(i, true);
        }
    }
}
