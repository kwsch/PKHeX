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
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            RTB.Text = PKHeX.Properties.Resources.changelog;
        }

        private void B_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
