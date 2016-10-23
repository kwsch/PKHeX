using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_CGearSkin : Form
    {
        public SAV_CGearSkin()
        {
            InitializeComponent();

            SAV = (SAV5)Main.SAV.Clone();

            bool cgearPresent = SAV.Data[SAV.CGearInfoOffset + 0x26] == 1;
            bg = new CGearBackground(cgearPresent ?
                CGearBackground.PSKtoCGB(SAV.Data.Skip(SAV.CGearDataOffset).Take(CGearBackground.SIZE_CGB).ToArray(), SAV.B2W2)
                : new byte[CGearBackground.SIZE_CGB]);

            PB_Background.Image = bg.GetImage();
        }

        private readonly SAV5 SAV;
        private CGearBackground bg;

        private void B_ImportPNG_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "PNG File|*.png",
                FileName = "Background.png",
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            Bitmap img = (Bitmap)Image.FromFile(ofd.FileName);

            try
            {
                bg.SetImage(img);
                PB_Background.Image = bg.GetImage();
            }
            catch (Exception ex)
            {
                Util.Error(ex.Message);
            }
        }
        private void B_ExportPNG_Click(object sender, EventArgs e)
        {
            Image png = PB_Background.Image;
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PNG File|*.png",
                FileName = "Background.png",
            };

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            png.Save(sfd.FileName, ImageFormat.Png);
        }
        private void B_ImportCGB_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = CGearBackground.Filter + "|PokeStock C-Gear Skin|*.psk"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            var len = new FileInfo(ofd.FileName).Length;
            if (len != CGearBackground.SIZE_CGB)
            {
                Util.Error($"Incorrect size, got {len} bytes, expected {CGearBackground.SIZE_CGB} bytes.");
                return;
            }

            byte[] data = File.ReadAllBytes(ofd.FileName);
            if (!CGearBackground.getIsCGB(data))
            {
                bool B2W2 = data[0x2000] != 0x00;
                data = CGearBackground.PSKtoCGB(data, B2W2);
            }

            bg = new CGearBackground(data);
            PB_Background.Image = bg.GetImage();
        }
        private void B_ExportCGB_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = CGearBackground.Filter,
            };

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            byte[] data = bg.Write();
            File.WriteAllBytes(sfd.FileName, data);
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            byte[] bgdata = bg.Write();
            if (bgdata.SequenceEqual(new byte[CGearBackground.SIZE_CGB]))
                return;

            // Data present

            bgdata = CGearBackground.CGBtoPSK(bgdata, SAV.B2W2);

            Array.Copy(bgdata, 0, Main.SAV.Data, SAV.CGearDataOffset, bgdata.Length);
            ushort chk = SaveUtil.ccitt16(bgdata);
            BitConverter.GetBytes(chk).CopyTo(Main.SAV.Data, SAV.CGearDataOffset + bgdata.Length + 2);
            BitConverter.GetBytes(chk).CopyTo(Main.SAV.Data, SAV.CGearDataOffset + bgdata.Length + 0x100);

            byte[] skinchkdata = Main.SAV.Data.Skip(SAV.CGearDataOffset + bgdata.Length + 0x100).Take(4).ToArray();
            ushort skinchkval = SaveUtil.ccitt16(skinchkdata);
            BitConverter.GetBytes(skinchkval).CopyTo(Main.SAV.Data, SAV.CGearDataOffset + bgdata.Length + 0x112);

            // Indicate in the save file that data is present
            BitConverter.GetBytes((ushort)0xC21E).CopyTo(Main.SAV.Data, 0x19438);



            Main.SAV.Data[SAV.CGearInfoOffset + 0x26] = 1; // data present
            BitConverter.GetBytes(chk).CopyTo(Main.SAV.Data, SAV.CGearInfoOffset + 0x24);

            Main.SAV.Edited = true;
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
