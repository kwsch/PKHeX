using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PKHeX
{
    public partial class CodeImportPKM : Form
    {
        public CodeImportPKM()
        {
            InitializeComponent();
            this.returnArray = null;
        }
        public byte[] returnArray { get; set; } 
        private void B_Paste_Click(object sender, EventArgs e)
        {
            RTB_Code.Text = Clipboard.GetText();
        }

        private void B_Import_Click(object sender, EventArgs e)
        {
            // Gotta read in the textbox.
            if (RTB_Code.Text.Length < 1) return;
            byte[] data = new Byte[0];
            // Get Actual Lines
            for (int i = 0; i < RTB_Code.Lines.Count(); i++)
            {
                if (RTB_Code.Lines[i].Length > 0)
                {
                    if (RTB_Code.Lines[i].Length <= 2 * 8 && RTB_Code.Lines[i].Length > 2 * 8 + 2)
                    {
                        MessageBox.Show("Invalid code pasted (Type)", "Error"); return;
                    }
                    else
                    {
                        try
                        {
                            // Grab Line Data
                            string line = RTB_Code.Lines[i];
                            string[] rip = Regex.Split(line, " ");
                            Array.Resize(ref data, data.Length + 4);
                            Array.Copy(BitConverter.GetBytes(UInt32.Parse(rip[1], NumberStyles.HexNumber)), 0, data, data.Length - 4, 4);
                        }
                        catch
                        {
                            MessageBox.Show("Invalid code pasted (Content)", "Error"); return;
                        }
                    }
                }
            }
            // Go over the data
            if ((data.Length == 232 - 4) || (data.Length == 260 - 4))
            {
                Array.Resize(ref data, data.Length + 4);
                Array.Copy(data, 0, data, 4, data.Length);
                data[0] = data[1] = data[2] = data[3] = 0;
            }
            if ((data.Length == 232) || (data.Length == 260))
            {
                this.returnArray = data;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid code pasted (Length)", "Error"); return;
            }
        }
    }
}
