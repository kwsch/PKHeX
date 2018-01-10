using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_CGearSkin : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV5 SAV;
        public SAV_CGearSkin(SaveFile sav)
        {
            SAV = (SAV5)(Origin = sav).Clone();
            InitializeComponent();

            bool cgearPresent = SAV.Data[SAV.CGearInfoOffset + 0x26] == 1;
            byte[] data = new byte[CGearBackground.SIZE_CGB];
            if (cgearPresent)
                Array.Copy(SAV.Data, SAV.CGearDataOffset, data, 0, CGearBackground.SIZE_CGB);
            bg = new CGearBackground(data);

            PB_Background.Image = bg.GetImage();
            WinFormsUtil.Alert("Editor is incomplete.", "No guarantee of functionality.");
        }

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
                WinFormsUtil.Error(ex.Message);
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

            var path = ofd.FileName;
            var len = new FileInfo(path).Length;
            if (len != CGearBackground.SIZE_CGB)
            {
                WinFormsUtil.Error($"Incorrect size, got {len} bytes, expected {CGearBackground.SIZE_CGB} bytes.");
                return;
            }

            byte[] data = File.ReadAllBytes(path);
            LoadBackground(data);
        }

        private void LoadBackground(byte[] data)
        {
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

            byte[] data = bg.GetCGB();
            File.WriteAllBytes(sfd.FileName, data);
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            byte[] bgdata = bg.GetPSK(SAV.B2W2);
            if (bgdata.SequenceEqual(new byte[CGearBackground.SIZE_CGB]))
                return;

            byte[] dlcfooter = { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x27, 0x00, 0x00, 0x27, 0x35, 0x05, 0x31, 0x00, 0x00 };

            bgdata.CopyTo(SAV.Data, SAV.CGearDataOffset);
            ushort chk = SaveUtil.CRC16_CCITT(bgdata);
            var chkbytes = BitConverter.GetBytes(chk);
            int footer = SAV.CGearDataOffset + bgdata.Length;

            BitConverter.GetBytes((ushort)1).CopyTo(SAV.Data, footer); // block updated once
            chkbytes.CopyTo(SAV.Data, footer + 2); // checksum
            chkbytes.CopyTo(SAV.Data, footer + 0x100); // second checksum

            ushort skinchkval = SaveUtil.CRC16_CCITT(SAV.Data, footer + 0x100, 4);
            dlcfooter.CopyTo(SAV.Data, footer + 0x104);
            BitConverter.GetBytes(skinchkval).CopyTo(SAV.Data, footer + 0x112);

            // Indicate in the save file that data is present
            BitConverter.GetBytes((ushort)0xC21E).CopyTo(SAV.Data, 0x19438);

            int info = SAV.CGearInfoOffset + 0x24;
            if (SAV.B2W2)
                info += 0x10;
            chkbytes.CopyTo(SAV.Data, info);
            SAV.Data[info + 2] = 1; // data present
            int flag = SAV.CGearDataOffset + (SAV.B2W2 ? 0x6C : 0x54);
            SAV.Data[flag] = 1; // data present

            Origin.SetData(SAV.Data, 0);
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
