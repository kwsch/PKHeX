using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class CodeGenerator : Form
    {
        byte[] newdata = new byte[232];

        readonly byte[] tabdata;
        public CodeGenerator(byte[] formdata)
        {
            tabdata = formdata;
            InitializeComponent();
            CenterToParent();
            RTB_Code.Clear();
            TB_Write.Clear();
            CB_Box.Items.Clear();
            for (int i = 1; i <= 31; i++)
                CB_Box.Items.Add(i.ToString());

            CB_Source.SelectedIndex = 0;
            CB_Slot.SelectedIndex = 0;
            CB_Box.SelectedIndex = 0;
        }
        private bool loaddata()
        {
            if (CB_Source.SelectedIndex == 0)
            {
                if (tabdata != null)
                {
                    byte[] pkx = tabdata;
                    newdata = new byte[232];
                    Array.Copy(PKX.encryptArray(pkx), newdata, 232);
                }
                else
                    return false;

            }
            else if (CB_Source.SelectedIndex == 1)
            {
                newdata = new byte[0xE8];
                Array.Copy(Main.SAV.Data,
                    Main.SAV.Box                   // Box Offset
                        + CB_Box.SelectedIndex*232*30 // Box Shift
                        + CB_Slot.SelectedIndex * 232,      // Slot Shift
                    newdata, 0, 0xE8);

                if (!newdata.SequenceEqual(new byte[0xE8])) return true;

                System.Media.SystemSounds.Exclamation.Play();
                return false;
            }
            else if (CB_Source.SelectedIndex == 2)
            {
                // Wondercard
                newdata = new byte[0x108];
                // Wondercard #
                int index = CB_Slot.SelectedIndex;
                // copy from save, the chosen wondercard offset, to new data
                Array.Copy(Main.SAV.Data, Main.SAV.WondercardData + index * 0x108, newdata, 0, 0x108);
                byte[] zerodata = new byte[0x108];
                if (!newdata.SequenceEqual(zerodata)) return true;

                System.Media.SystemSounds.Exclamation.Play();
                return false;
            }
            return true;
        }
        
        private void B_Add_Click(object sender, EventArgs e)
        {
            // Add the new code to the textbox.
            if (!loaddata()) return;
            uint writeoffset = Util.getHEXval(TB_Write.Text);

            for (int i = 0; i < newdata.Length / 4; i++)
            {
                RTB_Code.AppendText((writeoffset + i * 4 + 0x20000000).ToString("X8") + " ");
                RTB_Code.AppendText(BitConverter.ToUInt32(newdata,i*4).ToString("X8") + Environment.NewLine);
            }

            // Mat's Code - Unfinished
            //for (int i = 0; i < newdata.Length / (4); i++)
            //{
            //    // Add Operator

            //    RTB_Code.AppendText("00000001 ");   // 01 00 00 00
            //    RTB_Code.AppendText((writeoffset + i * 4).ToString("X8") + " ");
            //    RTB_Code.AppendText(BitConverter.ToUInt32(newdata,i*4).ToString("X8") + Environment.NewLine);
            //}
        }
        private void B_Clear_Click(object sender, EventArgs e)
        {
            RTB_Code.Clear();
        }
        private void B_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "Code File|*.bin"};
            if (ofd.ShowDialog() != DialogResult.OK) return;

            string path = ofd.FileName;
            byte[] ncf = File.ReadAllBytes(path);
            uint length = BitConverter.ToUInt32(ncf, 0);

            if (ncf.Length != length + 4)
            {
                Util.Error("Not a valid code file.");
                return;
            }
            if (RTB_Code.Text.Length > 0)
            {
                DialogResult ld = Util.Prompt(MessageBoxButtons.YesNo, "Replace current code?");
                if (ld == DialogResult.Yes)
                    RTB_Code.Clear();
                else if (ld != DialogResult.No)
                    return;
            }
            for (int i = 4; i <= ncf.Length-12; i+=12)
            {
                RTB_Code.AppendText(BitConverter.ToUInt32(ncf, i + 0 * 4).ToString("X8") + " ");
                RTB_Code.AppendText(BitConverter.ToUInt32(ncf, i + 1 * 4).ToString("X8") + " ");
                RTB_Code.AppendText(BitConverter.ToUInt32(ncf, i + 2 * 4).ToString("X8") + Environment.NewLine);
            }
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            // Gotta read in the textbox.
            if (RTB_Code.Text.Length < 1) return;

            byte[] ncf = new byte[4 + (RTB_Code.Lines.Length-1)*3*4];
            Array.Copy(BitConverter.GetBytes(ncf.Length - 4), ncf, 4);

            for (int i = 0; i < RTB_Code.Lines.Length-1; i++)
            {
                string line = RTB_Code.Lines[i];
                string[] rip = line.Split(new[] {" "}, StringSplitOptions.None);

                // Write the 3 u32's to an array.
                Array.Copy(BitConverter.GetBytes(uint.Parse(rip[0], NumberStyles.HexNumber)),0,ncf,4+i*12+0,4);
                Array.Copy(BitConverter.GetBytes(uint.Parse(rip[1], NumberStyles.HexNumber)),0,ncf,4+i*12+4,4);
                Array.Copy(BitConverter.GetBytes(uint.Parse(rip[2], NumberStyles.HexNumber)),0,ncf,4+i*12+8,4);
            }

            SaveFileDialog sfd = new SaveFileDialog {FileName = "code.bin", Filter = "Code File|*.bin"};
            if (sfd.ShowDialog() != DialogResult.OK) return;

            string path = sfd.FileName;
            if (File.Exists(path))
            {
                // File already exists, save a .bak
                byte[] backupfile = File.ReadAllBytes(path);
                File.WriteAllBytes(path + ".bak", backupfile);
            }
            File.WriteAllBytes(path, ncf);
        }
        private void B_Copy_Click(object sender, EventArgs e)
        {
            if (RTB_Code.Text.Length > 0)
                Clipboard.SetText(RTB_Code.Text);
            else
            {
                B_Diff.PerformClick();
                try
                {
                    Clipboard.SetText(RTB_Code.Text);
                    Util.Alert(
		        "Code generated and copied to clipboard!",
		        "Next time click [Create Diff] first.");
                }
                catch
                {
                    Util.Alert(
		        "No code created!",
		        "Click [Create Diff], then make sure that data appears in the Text Box below. If no code appears, then you didn't save your changes.",
		        "Be sure to Set the Pokemon you edited back into a Box/Party slot!");
		}
            }
        }
        private void B_Diff_Click(object sender, EventArgs e)
        {
            string result = ""; 
            RTB_Code.Clear();
            byte[] cybersav = Main.SAV.BAK;
            byte[] newcyber = Main.SAV.Data;

            int boxoffset = Main.SAV.Box;
            
            int lines = 0;  // 65400
            for (int i = 0; i < newcyber.Length - 0x200; i += 4)
            {
                // Skip Party and Boxes
                if (i == 0x14200) i += 260 * 6 + 4; // +4 to skip over party count
                if (i == boxoffset) i += 232 * 30 * 31;
                if (BitConverter.ToUInt32(cybersav, i) == BitConverter.ToUInt32(newcyber, i)) continue;

                result += (0x20000000 + i).ToString("X8") + " ";
                result += BitConverter.ToUInt32(newcyber, i).ToString("X8") + Environment.NewLine;

                lines++;
                if ((lines % 128 == 0) && CHK_Break.Checked)
                {
                    result +=
                        Environment.NewLine +
                        "--- Segment " + (lines / 128 + 1) + " ---" +
                        Environment.NewLine + Environment.NewLine;
                }
                if (lines > 10000) goto toomany;
            }

            // Loop Through Party
            for (int i = 0x14200; i < 0x14200 + 260 * 6; i+= 260)
            {
                byte[] bytes = new byte[260]; Array.Copy(newcyber, i, bytes, 0, 260);
                byte[] olddata = new byte[260]; Array.Copy(cybersav, i, olddata, 0, 260);
                if (bytes.SequenceEqual(olddata)) continue;

                for (int z = 0; z < bytes.Length; z += 4)
                {
                    result += (0x20000000 + i + z).ToString("X8") + " ";
                    result += BitConverter.ToUInt32(bytes, z).ToString("X8") + Environment.NewLine;

                    lines++;
                    if (lines % 128 == 0 && CHK_Break.Checked)
                    {
                        result +=
                            Environment.NewLine +
                            "--- Segment " + (lines / 128 + 1) + " ---" +
                            Environment.NewLine + Environment.NewLine;
                    }
                    if (lines > 10000) goto toomany;
                }
            }

            // Fix Party Count if Necessary
            if (cybersav[0x14818] != newcyber[0x14818])
            {
                result += (0x00000000 + 0x14818).ToString("X8") + " ";
                result += newcyber[0x14818].ToString("X8") + Environment.NewLine;

                lines++;
                if ((lines % 128 == 0) && CHK_Break.Checked)
                {
                    result +=
                        Environment.NewLine +
                        "--- Segment " + (lines / 128 + 1) + " ---" +
                        Environment.NewLine + Environment.NewLine;
                }
                if (lines > 10000) goto toomany;
            }

            // Loop Through Boxes
            for (int i = boxoffset; i < boxoffset + 232 * 30 * 31; i += 232)
            {
                byte[] bytes = new byte[232]; Array.Copy(newcyber, i, bytes, 0, 232);
                byte[] olddata = new byte[232]; Array.Copy(cybersav, i, olddata, 0, 232);
                if (bytes.SequenceEqual(olddata)) continue;

                for (int z = 0; z < bytes.Length; z += 4)
                {
                    result += (0x20000000 + i + z).ToString("X8") + " ";
                    result += BitConverter.ToUInt32(bytes, z).ToString("X8") + Environment.NewLine;

                    lines++;
                    if ((lines % 128 == 0) && CHK_Break.Checked)
                    {
                        result +=
                            Environment.NewLine +
                            "--- Segment " + (lines / 128 + 1) + " ---" +
                            Environment.NewLine + Environment.NewLine;
                    }
                    if (lines > 10000) goto toomany;
                }
            }

            if ((lines / 128 > 0) && CHK_Break.Checked)
            {
                Util.Alert($"{1 + lines/128} Code Segments.",
                    $"{lines} Lines.");
            }
            RTB_Code.Text = result;
            return;

        toomany:
            {
                Util.Alert("Too many differences detected.",
                           "Export your save instead.");
            }
        }

        // Import 
        public byte[] returnArray { get; set; } 
        private void B_Paste_Click(object sender, EventArgs e)
        {
            RTB_Code.Text = Clipboard.GetText();
        }
        private void B_Import_Click(object sender, EventArgs e)
        {
            // Gotta read in the textbox.
            if (RTB_Code.Text.Length < 1) return;
            byte[] data = new byte[0];
            // Get Actual Lines
            foreach (string line in RTB_Code.Lines)
            {
                if (line.Length <= 0) continue;

                if (line.Length <= 2 * 8 && line.Length > 2 * 8 + 2)
                { Util.Error("Invalid code pasted (Type)"); return; }

                try
                {
                    // Grab Line Data
                    string[] rip = line.Split(new[] { " " }, StringSplitOptions.None);
                    Array.Resize(ref data, data.Length + 4);
                    BitConverter.GetBytes(uint.Parse(rip[1], NumberStyles.HexNumber)).CopyTo(data, data.Length - 4);
                }
                catch (Exception x)
                { Util.Error("Invalid code pasted (Content):", x.ToString()); return; }
            }
            // Go over the data
            if ((data.Length == 232 - 4) || (data.Length == 260 - 4))
            {
                Array.Resize(ref data, data.Length + 4);
                Array.Copy(data, 0, data, 4, data.Length);
                data[0] = data[1] = data[2] = data[3] = 0;
            }
            if ((data.Length == 232) || (data.Length == 260))
            { returnArray = data; Close(); }
            else { Util.Error("Invalid code pasted (Length)"); }
        }
    }
}
