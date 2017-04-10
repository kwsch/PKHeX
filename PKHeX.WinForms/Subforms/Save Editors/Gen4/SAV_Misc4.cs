using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Misc4 : Form
    {
        private readonly SAV4 SAV = (SAV4)Main.SAV.Clone();
        public SAV_Misc4()
        {
            InitializeComponent();

            Apps = new[] {
                CHK_App01,CHK_App02,CHK_App03,CHK_App04,CHK_App05,
                CHK_App06,CHK_App07,CHK_App08,CHK_App09,CHK_App10,
                CHK_App11,CHK_App12,CHK_App13,CHK_App14,CHK_App15,
                CHK_App16,CHK_App17,CHK_App18,CHK_App19,CHK_App20,
                CHK_App21,CHK_App22,CHK_App23,CHK_App24,CHK_App25,
            };

            int ret = SAV.PoketchApps;
            for (int i = 0; i < Apps.Length; i++)
                Apps[i].Checked = (ret & 1 << i) != 0;
            DotArtistByte = SAV.PoketchDotArtist;
            ColorTable = new byte[] { 248, 168, 88, 8 };
            setPictureBoxFromFlags(DotArtistByte);
            string tip = "Guide about D&D ImageFile Format";
            tip += Environment.NewLine + " width = 24px";
            tip += Environment.NewLine + " height = 20px";
            tip += Environment.NewLine + " used color count <= 4";
            tip += Environment.NewLine + " file size < 2058byte";
            tip1.SetToolTip(PB_DotArtist, tip);
        }

        private readonly CheckBox[] Apps;
        private readonly byte[] DotArtistByte;
        private readonly byte[] ColorTable;
        private readonly ToolTip tip1 = new ToolTip();

        private void setPictureBoxFromFlags(byte[] inp)
        {
            if (inp.Length != 120) return;
            byte[] dupbyte = new byte[23040];
            for (int iy = 0; iy < 20; iy++)
                for (int ix = 0; ix < 24; ix++)
                {
                    var ib = ix + 24 * iy;
                    var ict = ColorTable[inp[ib >> 2] >> (ib % 4 << 1) & 3];
                    var iz = 12 * ix + 0x480 * iy;
                    for (int izy = 0; izy < 4; izy++)
                        for (int izx = 0; izx < 4; izx++)
                            for (int ic = 0; ic < 3; ic++)
                                dupbyte[ic + 3 * izx + 0x120 * izy + iz] = ict;
                }
            Bitmap dabmp = new Bitmap(96, 80);
            BitmapData dabdata = dabmp.LockBits(new Rectangle(0, 0, dabmp.Width, dabmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.Runtime.InteropServices.Marshal.Copy(dupbyte, 0, dabdata.Scan0, dupbyte.Length);
            dabmp.UnlockBits(dabdata);
            PB_DotArtist.Image = dabmp;
        }
        private void setFlagsFromFileName(string inpFileName)
        {
            if (new FileInfo(inpFileName).Length > 2058) return; // 24*20*4(ARGB)=1920
            Bitmap bmp;
            FileStream fs = new FileStream(inpFileName, FileMode.Open, FileAccess.Read);
            try
            {
                bmp = (Bitmap)Image.FromStream(fs);
            }
            catch
            {
                bmp = null;
            }
            fs.Close();
            if (bmp == null || bmp.Width != 24 || bmp.Height != 20) return;

            byte[] BrightMap = new byte[480];
            byte[] BrightCount = new byte[0x100];
            byte[] iBrightCount = new byte[0x100];
            for (int iy = 0; iy < 20; iy++)
                for (int ix = 0; ix < 24; ix++)
                {
                    var ig = (byte)(0xFF * bmp.GetPixel(ix, iy).GetBrightness());
                    BrightMap[ix + 24 * iy] = ig;
                    BrightCount[ig]++;
                }

            int ColorCount = BrightCount.Count(v => v > 0);
            if (ColorCount > 4 || ColorCount == 0) return;
            int errmin = int.MaxValue;
            byte[] LCT = new byte[4];
            byte[] mLCT = new byte[4];
            for (int i = 0; i < 4; i++)
                LCT[i] = (byte)(ColorCount < i + 1 ? 4 : ColorCount - i - 1);
            int ee = 0;
            while (++ee < 1000)
            {
                BrightCount.CopyTo(iBrightCount, 0);
                for (int i = 0, j = 0; i < 0x100; i++)
                    if (iBrightCount[i] > 0)
                        iBrightCount[i] = LCT[j++];
                var errtot = 0;
                for (int i = 0; i < 480; i++)
                    errtot += Math.Abs(BrightMap[i] - ColorTable[iBrightCount[BrightMap[i]]]);
                if (errmin > errtot)
                {
                    errmin = errtot;
                    LCT.CopyTo(mLCT, 0);
                }
                LCT = getNextLCT(LCT);
                if (LCT[0] >= 4) break;
            }
            for (int i = 0, j = 0; i < 0x100; i++)
                if (BrightCount[i] > 0)
                    BrightCount[i] = mLCT[j++];
            for (int i = 0; i < 480; i++)
                BrightMap[i] = BrightCount[BrightMap[i]];

            byte[] ndab = new byte[120];
            for (int i = 0; i < 480; i++)
                ndab[i >> 2] |= (byte)((BrightMap[i] & 3) << (i % 4 << 1));

            ndab.CopyTo(DotArtistByte, 0);
        }

        private byte[] getNextLCT(byte[] inp)
        {
            while (true)
            {
                if (++inp[0] < 4)
                    continue;

                inp[0] = 0;
                if (++inp[1] < 4)
                    continue;

                inp[1] = 0;
                if (++inp[2] < 4)
                    continue;

                inp[2] = 0;
                if (++inp[3] < 4)
                    continue;

                inp[0] = 4;
                return inp;
            }
        }

        private void setFlagsFromClickPoint(int inpX, int inpY)
        {
            if (inpX < 0) inpX = 0;
            else if (inpX > 95) inpX = 95;
            if (inpY < 0) inpY = 0;
            else if (inpY > 79) inpY = 79;
            int i = (inpX >> 2) + 24 * (inpY >> 2);
            byte[] ndab = new byte[120];
            DotArtistByte.CopyTo(ndab, 0);
            byte c = (byte)(ndab[i >> 2] >> (i % 4 << 1) & 3);
            if (++c >= 4) c = 0;
            ndab[i >> 2] &= (byte)~(3 << (i % 4 << 1));
            ndab[i >> 2] |= (byte)((c & 3) << (i % 4 << 1));
            ndab.CopyTo(DotArtistByte, 0);
        }

        private void saveMisc()
        {
            int ret = 0;
            for (int i = 0; i < Apps.Length; i++)
            {
                if (Apps[i].Checked) ret |= 1 << i;
            }
            SAV.PoketchApps = ret;
            SAV.PoketchDotArtist = DotArtistByte;
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            saveMisc();
            SAV.Data.CopyTo(Main.SAV.Data, 0);
            Main.SAV.Edited = true;
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            foreach (CheckBox c in Apps) c.Checked = true;
        }

        private void TAB_Poketch_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void TAB_Poketch_DragDrop(object sender, DragEventArgs e)
        {
            string[] t = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (t.Length != 1) return;
            setFlagsFromFileName(t[0]);
            setPictureBoxFromFlags(DotArtistByte);
        }

        private void PB_DotArtist_MouseClick(object sender, MouseEventArgs e)
        {
            setFlagsFromClickPoint(e.X, e.Y);
            setPictureBoxFromFlags(DotArtistByte);
        }
    }
}
