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
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV5)(Origin = sav).Clone();

            byte[] data = SAV.CGearSkinData;
            bg = new CGearBackground(data);

            PB_Background.Image = CGearImage.GetBitmap(bg);
        }

        private CGearBackground bg;

        private void B_ImportPNG_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "PNG File|*.png",
                FileName = "Background.png",
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            Bitmap img = (Bitmap)Image.FromFile(ofd.FileName);

            try
            {
                bg = CGearImage.GetCGearBackground(img);
                PB_Background.Image = CGearImage.GetBitmap(bg);
            }
            catch (Exception ex)
            {
                WinFormsUtil.Error(ex.Message);
            }
        }

        private void B_ExportPNG_Click(object sender, EventArgs e)
        {
            Image png = PB_Background.Image;
            using var sfd = new SaveFileDialog
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
            using var ofd = new OpenFileDialog
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
            bg = new CGearBackground(data);
            PB_Background.Image = CGearImage.GetBitmap(bg);
        }

        private void B_ExportCGB_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = CGearBackground.Filter,
            };

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            byte[] data = bg.GetSkin(true);
            File.WriteAllBytes(sfd.FileName, data);
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            byte[] bgdata = bg.GetSkin(SAV is SAV5B2W2);
            if (bgdata.Any(z => z != 0))
            {
                SAV.CGearSkinData = bgdata;
                Origin.CopyChangesFrom(SAV);
            }
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
