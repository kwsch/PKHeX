using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Underground : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV4 SAV;

        public SAV_Underground(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV4)(Origin = sav).Clone();

            GetUGScores();
        }

        private void GetUGScores()
        {
            U_PlayersMet.Value = SAV.UG_PlayersMet;
            U_Gifts.Value = SAV.UG_Gifts;
            U_Spheres.Value = SAV.UG_Spheres;
            U_Fossils.Value = SAV.UG_Fossils;
            U_TrapsA.Value = SAV.UG_TrapsAvoided;
            U_TrapsT.Value = SAV.UG_TrapsTriggered;
            U_Flags.Value = SAV.UG_Flags;
        }

        private void SetUGScores()
        {
            SAV.UG_PlayersMet = (int)U_PlayersMet.Value;
            SAV.UG_Gifts = (int)U_Gifts.Value;
            SAV.UG_Spheres = (int)U_Spheres.Value;
            SAV.UG_Fossils = (int)U_Fossils.Value;
            SAV.UG_TrapsAvoided = (int)U_TrapsA.Value;
            SAV.UG_TrapsTriggered = (int)U_TrapsT.Value;
            SAV.UG_Flags = (int)U_Flags.Value;
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SetUGScores();
            Origin.SetData(SAV.Data, 0);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();
    }
}
