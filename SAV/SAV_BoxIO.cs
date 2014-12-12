using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PKHeX
{
    public partial class SAV_BoxIO : Form
    {
        public SAV_BoxIO(Form1 frm1, int bo, int bno)
        {
            boxnameoffset = bno;
            boxoffset = bo;
            InitializeComponent();
            m_parent = frm1;
            savindex = m_parent.savindex;
            shiftval = savindex * 0x7F000;
            CB_Box.SelectedIndex = 0;
            getBoxNames();
        }
        int boxoffset; int boxnameoffset;
        Form1 m_parent;
        public int savindex; int shiftval;

        private void B_ImportBox_Click(object sender, EventArgs e)
        {
            int index = CB_Box.SelectedIndex;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Box File|*.bk6|All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string path = ofd.FileName;
                byte[] data = File.ReadAllBytes(path);
                int offset = shiftval + boxoffset + (CB_Box.SelectedIndex) * (0xE8 * 30);
                if (data.Length <= (31 - index) * (0xE8 * 30))
                {
                    Array.Copy(data, 0, m_parent.savefile, offset, data.Length);
                    m_parent.setPKXBoxes();
                }
                else
                    Util.Error("Box data loaded is too big!");
            }
        }
        private void B_ExportBox_Click(object sender, EventArgs e)
        {
            int index = CB_Box.SelectedIndex;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Box File|*.bk6|All Files|*.*";
            sfd.FileName = CB_Box.Text + ".bk6";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string path = sfd.FileName;
                if (File.Exists(path))
                {
                    // File already exists, save a .bak
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(path + ".bak", backupfile);
                }
                int offset = shiftval + boxoffset + (CB_Box.SelectedIndex) * (0xE8 * 30);
                byte[] newbox = new byte[0xE8 * 30];
                Array.Copy(m_parent.savefile, offset, newbox, 0, 0xE8 * 30);
                File.WriteAllBytes(path, newbox);
            }
        }
        public void getBoxNames()
        {
            int selectedbox = CB_Box.SelectedIndex;    // precache selected box
            // Build ComboBox Dropdown Items
            try
            {
                CB_Box.Items.Clear();
                for (int i = 0; i < 31; i++)
                {
                    string boxname = Encoding.Unicode.GetString(m_parent.savefile, boxnameoffset + (0x7F000 * savindex) + 0x22 * i, 0x22);
                    boxname = Util.TrimFromZero(boxname);
                    if (boxname.Length == 0)
                        CB_Box.Items.Add("Box " + (i+1));
                    else CB_Box.Items.Add(boxname);
                }
            }
            catch
            {
                for (int i = 1; i < 32; i++)
                    CB_Box.Items.Add("Box " + (i+1));
            }

            CB_Box.SelectedIndex = selectedbox;    // restore selected box
        }
    }
}
