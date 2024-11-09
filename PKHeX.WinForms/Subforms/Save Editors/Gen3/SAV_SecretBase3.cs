using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PKHeX.WinForms
{
    public partial class SAV_SecretBase3 : Form
    {

        private readonly SaveFile Origin;
        private readonly SAV3 SAV;

        public SAV_SecretBase3(SAV3 sav)
        {
            InitializeComponent();
            //WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV3)(Origin = sav).Clone();

            SecretBaseManager3 manager = ((IGen3Hoenn)SAV).SecretBases;
            LB_Bases.InitializeBinding();
            LB_Bases.DataSource = manager.Bases;
            LB_Bases.DisplayMember = "OriginalTrainerName";
            LB_Bases.SelectedIndexChanged += (_, _) =>
            {
                TB_Name.Text = ((SecretBase3)LB_Bases.SelectedItem).OriginalTrainerName;
                T_TrainerGender.Gender = ((SecretBase3)LB_Bases.SelectedItem).OriginalTrainerGender;
                T_TrainerGender.Show();
            };
            if (manager.Count > 0)
                LB_Bases.SelectedIndex = 0;
        }
    }
}
