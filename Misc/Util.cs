using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace PKHeX
{
    public partial class Util
    {
        // Image Layering/Blending Utility
        public static Image layerImage(Image baseLayer, Image overLayer, int x, int y, double trans)
        {
            Bitmap overlayImage = (Bitmap)overLayer;
            Bitmap newImage = (Bitmap)baseLayer;
            if (baseLayer == null) return overLayer;
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
        public static Color AlphaBlend(Color ForeGround, Color BackGround)
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
    
        // 3DSSE Utility
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
        public static string GetBackupLocation()
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
        public static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        public static string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            if (index < 0)
                return input;

            return input.Substring(0, index);
        }
        public static string[] getStringList(string f, string l)
        {
            object txt = Properties.Resources.ResourceManager.GetObject("text_" + f + "_" + l); // Fetch File, \n to list.
            List<string> rawlist = ((string)txt).Split(new char[] { '\n' }).ToList();

            string[] stringdata = new string[rawlist.Count];
            for (int i = 0; i < rawlist.Count; i++)
                stringdata[i] = rawlist[i];

            return stringdata;
        }


        public static Random rand = new Random();
        public static uint rnd32()
        {
            return (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
        }

        public static int ToInt32(TextBox tb)
        {
            string value = tb.Text;
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return Int32.Parse(value); }
            catch
            { return 0; }
        }
        public static uint ToUInt32(TextBox tb)
        {
            string value = tb.Text;
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return UInt32.Parse(value); }
            catch
            { return 0; }
        }
        public static int ToInt32(MaskedTextBox tb)
        {
            string value = tb.Text;
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return Int32.Parse(value); }
            catch
            { return 0; }
        }
        public static uint ToUInt32(MaskedTextBox tb)
        {
            string value = tb.Text;
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return UInt32.Parse(value); }
            catch
            { return 0; }
        }
        public static int ToInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return Int32.Parse(value); }
            catch
            { return 0; }
        }
        public static uint ToUInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
            { return 0; }
            try
            { return UInt32.Parse(value); }
            catch
            { return 0; }
        }
        public static uint getHEXval(TextBox tb)
        {
            if (tb.Text == null)
                return 0;
            string str = RemoveTroublesomeCharacters(tb);
            return UInt32.Parse(str, NumberStyles.HexNumber);
        }
        public static int getIndex(ComboBox cb)
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
        public static string RemoveTroublesomeCharacters(TextBox tb)
        {
            string inString = tb.Text;
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                // filter for hex
                if ((ch < 0x0047 && ch > 0x002F) || (ch < 0x0067 && ch > 0x0060))
                    newString.Append(ch);
                else
                    System.Media.SystemSounds.Beep.Play();
            }
            if (newString.Length == 0)
                newString.Append("0");
            uint value = UInt32.Parse(newString.ToString(), NumberStyles.HexNumber);
            tb.Text = value.ToString("X8");
            return newString.ToString();
        }
    }
}
