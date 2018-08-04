using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_OPower : Form
    {
        private readonly SAV6 Origin;
        private readonly OPower6 Data;

        public SAV_OPower(SAV6 sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            Origin = sav;
            Data = sav.OPowerData;

            Current = Types[0];
            foreach (var z in Types)
                CB_Type.Items.Add(z.ToString());
            CB_Type.SelectedIndex = 0;
            CHK_Master.Checked = Data.MasterFlag;
            LoadCurrent();

            CB_Type.SelectedIndexChanged += (s, e) => { SaveCurrent(); LoadCurrent(); };
            B_ClearAll.Click += (s, e) => { Data.ClearAll(); LoadCurrent(); };
            B_GiveAll.Click += (s, e) => { Data.UnlockRegular(); LoadCurrent(); };
            B_GiveAllMAX.Click += (s, e) => { Data.UnlockAll(); LoadCurrent(); };
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveData();
            Close();
        }

        private static readonly OPower6Type[] Types = (OPower6Type[])Enum.GetValues(typeof(OPower6Type));
        private static readonly string[] Values = Enum.GetNames(typeof(OPower6Value));

        private OPower6Type Current;

        private void SaveData()
        {
            Data.MasterFlag = CHK_Master.Checked;
            SaveCurrent();
            Origin.OPowerData = Data;
        }

        private void LoadCurrent()
        {
            Current = Types[CB_Type.SelectedIndex];

            CB_Value.Items.Clear();
            int count = Data.GetOPowerCount(Current);
            for (int i = 0; i <= count; i++)
                CB_Value.Items.Add(Values[i]);

            CB_Value.SelectedIndex = Data.GetOPowerLevel(Current);

            CHK_S.Enabled = Data.GetHasOPowerS(Current);
            CHK_S.Checked = Data.GetOPowerS(Current);
            CHK_MAX.Enabled = Data.GetHasOPowerMAX(Current);
            CHK_MAX.Checked = Data.GetOPowerMAX(Current);
        }

        private void SaveCurrent()
        {
            Data.SetOPowerLevel(Current, CB_Value.SelectedIndex);
            Data.SetOPowerS(Current, CHK_S.Checked);
            Data.SetOPowerMAX(Current, CHK_MAX.Checked);
        }
    }
}
