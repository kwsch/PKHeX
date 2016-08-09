using System;
using System.Drawing;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class f2_Text : Form
    {
        public f2_Text(TextBox TB_NN)
        {
            Hide();
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            TB_Nickname = TB_NN;
            Font pkxFont = PKX.getPKXFont(12F);
            Label[] lbla =
            { 
                L00, L01, L02, L03, L04, L05, L06, L07,
                L08, L09, L0A, L0B, L0C, L0D, L0E, L0F,
                L10, L11, L12, L13, L14, L15, L16, L17,
                L18, L19, L1A, L1B, L1C, L1D, L1E, L1F,
            };
            ushort[] chars =
            {
                0xE081, 0xE082, 0xE083, 0xE084, 0xE085, 0xE086, 0xE087, 0xE08D, 
                0xE08E, 0xE08F, 0xE090, 0xE091, 0xE092, 0xE093, 0xE094, 0xE095, 
                0xE096, 0xE097, 0xE098, 0xE099, 0xE09A, 0xE09B, 0xE09C, 0xE09D, 
                0xE09E, 0xE09F, 0xE0A0, 0xE0A1, 0xE0A2, 0xE0A3, 0xE0A4, 0xE0A5, 
            };
            for (int i = 0; i < 32; i++)
            {
                lbla[i].Font = pkxFont;
                lbla[i].Text = Convert.ToChar(chars[i]).ToString();
                lbla[i].Click += onClick;
            }
            CenterToParent();
            Show();
        }

        readonly TextBox TB_Nickname;
        private void onClick(object sender, EventArgs e)
        {
            string nickname = TB_Nickname.Text;
            if (nickname.Length < TB_Nickname.MaxLength)
                TB_Nickname.Text += ((Label) sender).Text;
        }
    }
}
