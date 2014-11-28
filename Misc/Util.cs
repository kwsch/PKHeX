using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Imaging;

namespace PKHeX
{
    public partial class Util
    {
        // Image Layering/Blending Utility
        internal static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y, double trans)
        {
            Bitmap overlayImage = (Bitmap)overLayer;
            Bitmap newImage = (Bitmap)baseLayer;
            if (baseLayer == null) return overlayImage;
            for (int i = 0; i < (overlayImage.Width * overlayImage.Height); i++)
            {
                Color newColor = overlayImage.GetPixel(i % (overlayImage.Width), i / (overlayImage.Width));
                Color oldColor = newImage.GetPixel(i % (overlayImage.Width) + x, i / (overlayImage.Width) + y);
                newColor = Color.FromArgb((int)((double)(newColor.A) * trans), newColor.R, newColor.G, newColor.B); // Apply transparency change
                // if (newColor.A != 0) // If Pixel isn't transparent, we'll overwrite the color.
                {
                    // if (newColor.A < 100) 
                    newColor = AlphaBlend(newColor, oldColor);
                    newImage.SetPixel(
                        i % (overlayImage.Width) + x,
                        i / (overlayImage.Width) + y,
                        newColor);
                }
            }
            return newImage;
        }
        internal static Bitmap ChangeOpacity(Image img, double trans)
        {
            if (img == null) return null;
            Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = (float)trans;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();   // Releasing all resource used by graphics
            return bmp;
        }
        internal static Color AlphaBlend(Color ForeGround, Color BackGround)
        {
            if (ForeGround.A == 0)
                return BackGround;
            if (BackGround.A == 0)
                return ForeGround;
            if (ForeGround.A == 255)
                return ForeGround;
            int Alpha = Convert.ToInt32(ForeGround.A) + 1;
            int B = Alpha * ForeGround.B + (255 - Alpha) * BackGround.B >> 8;
            int G = Alpha * ForeGround.G + (255 - Alpha) * BackGround.G >> 8;
            int R = Alpha * ForeGround.R + (255 - Alpha) * BackGround.R >> 8;
            int A = ForeGround.A;
            if (BackGround.A == 255)
                A = 255;
            if (A > 255)
                A = 255;
            if (R > 255)
                R = 255;
            if (G > 255)
                G = 255;
            if (B > 255)
                B = 255;
            return Color.FromArgb(Math.Abs(A), Math.Abs(R), Math.Abs(G), Math.Abs(B));
        }
        internal static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            return directory.GetFiles()
                .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }
        internal static string GetTempFolder() // From 3DSSE's decompiled source.
        {
            string tempPath = Path.GetTempPath();
            string str2 = "SE3DS";
            str2 = "3DSSE";
            tempPath = Path.Combine(tempPath, str2);
            // Directory.CreateDirectory(tempPath);
            return (tempPath);
        }
        internal static string GetCacheFolder() // edited
        {
            return Path.Combine(GetBackupLocation(), "cache");
        }
        internal static string GetRegistryValue(string key)
        {
            Microsoft.Win32.RegistryKey currentUser = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey key3 = currentUser.OpenSubKey(GetRegistryBase());
            if (key3 == null)
            {
                return null;
            }
            string str = key3.GetValue(key) as string;
            key3.Close();
            currentUser.Close();
            return str;
        }
        internal static string GetRegistryBase()
        {
            return @"SOFTWARE\CYBER Gadget\3DSSaveEditor";
        }
        internal static string GetBackupLocation()
        {
            string registryValue = GetRegistryValue("Location");
            if (!string.IsNullOrEmpty(registryValue))
            {
                Directory.CreateDirectory(registryValue);
                return registryValue;
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar + "3DSSaveBank";
            Directory.CreateDirectory(path);
            return path;
        }
        internal static string GetSDFLocation()
        {
            // start by checking if the 3DS file path exists or not.
            string path_SDF = null;
            string[] DriveList = Environment.GetLogicalDrives();
            for (int i = 1; i < DriveList.Length; i++)
            {
                path_SDF = DriveList[i] + "filer\\UserSaveData\\";
                if (Directory.Exists(path_SDF))
                    break;
            }
            if (path_SDF == null)
                return null;
            else
            {
                // 3DS data found in SD card reader. Let's get the title folder location!
                string[] folders = Directory.GetDirectories(path_SDF, "*", System.IO.SearchOption.TopDirectoryOnly);

                // Loop through all the folders in the Nintendo 3DS folder to see if any of them contain 'title'.
                for (int i = folders.Length - 1; i > 0; i--)
                {
                    if (File.Exists(Path.Combine(folders[i], "000011c4\\main"))) return Path.Combine(folders[i], "000011c4"); // OR
                    if (File.Exists(Path.Combine(folders[i], "000011c5\\main"))) return Path.Combine(folders[i], "000011c5"); // AS
                    if (File.Exists(Path.Combine(folders[i], "0000055d\\main"))) return Path.Combine(folders[i], "0000055d"); // X
                    if (File.Exists(Path.Combine(folders[i], "0000055e\\main"))) return Path.Combine(folders[i], "0000055e"); // Y

                    // I don't know
                    if (File.Exists(Path.Combine(folders[i], "00055d00\\main"))) return Path.Combine(folders[i], "00055d00"); // X
                    if (File.Exists(Path.Combine(folders[i], "00055e00\\main"))) return Path.Combine(folders[i], "00055e00"); // Y
                }
                return null;
            }
        }
        internal static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        internal static string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            if (index < 0)
                return input;

            return input.Substring(0, index);
        }
        internal static string[] getStringList(string f, string l)
        {
            object txt = Properties.Resources.ResourceManager.GetObject("text_" + f + "_" + l); // Fetch File, \n to list.
            List<string> rawlist = ((string)txt).Split(new char[] { '\n' }).ToList();

            string[] stringdata = new string[rawlist.Count];
            for (int i = 0; i < rawlist.Count; i++)
                stringdata[i] = rawlist[i];

            return stringdata;
        }

        internal static Random rand = new Random();
        internal static uint rnd32()
        {
            return (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
        }

        internal static int ToInt32(TextBox tb)
        {
            string value = tb.Text;
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return Int32.Parse(value); }
            catch
            { return 0; }
        }
        internal static uint ToUInt32(TextBox tb)
        {
            string value = tb.Text;
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return UInt32.Parse(value); }
            catch
            { return 0; }
        }
        internal static int ToInt32(MaskedTextBox tb)
        {
            string value = tb.Text;
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return Int32.Parse(value); }
            catch
            { return 0; }
        }
        internal static uint ToUInt32(MaskedTextBox tb)
        {
            string value = tb.Text;
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return UInt32.Parse(value); }
            catch
            { return 0; }
        }
        internal static int ToInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return Int32.Parse(value); }
            catch
            { return 0; }
        }
        internal static uint ToUInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return UInt32.Parse(value); }
            catch
            { return 0; }
        }
        internal static uint getHEXval(TextBox tb)
        {
            if (tb.Text == null)
                return 0;
            string str = getOnlyHex(tb.Text);
            return UInt32.Parse(str, NumberStyles.HexNumber);
        }
        internal static int getIndex(ComboBox cb)
        {
            int val = 0;
            try { val = Util.ToInt32(cb.SelectedValue.ToString()); }
            catch
            {
                val = cb.SelectedIndex;
                if (val < 0) val = 0;
            };
            return val;
        }
        internal static string getOnlyHex(string str)
        {
            if (str == null) return "0";

            char c;
            string s = "";

            for (int i = 0; i < str.Length; i++)
            {
                c = str[i];
                // filter for hex
                if ((c < 0x0047 && c > 0x002F) || (c < 0x0067 && c > 0x0060))
                    s+= c;
                else
                    System.Media.SystemSounds.Beep.Play();
            }
            if (s.Length == 0)
                s = "0";
            return s;
        }

        internal static MaskedTextBox[] shuffle(MaskedTextBox[] charArray)
        {
            MaskedTextBox[] shuffledArray = new MaskedTextBox[charArray.Length];
            int rndNo;

            for (int i = charArray.Length; i >= 1; i--)
            {
                rndNo = rand.Next(1, i + 1) - 1;
                shuffledArray[i - 1] = charArray[rndNo];
                charArray[rndNo] = charArray[i - 1];
            }
            return shuffledArray;
        }
    }
}
