using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PKHeX
{
    public class Util
    {
        // Image Layering/Blending Utility
        internal static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y, double trans)
        {
            Bitmap img = new Bitmap(baseLayer.Width, baseLayer.Height);
            using (Graphics gr = Graphics.FromImage(img))
            {
                gr.DrawImage(baseLayer, new Point(0, 0));
                Bitmap o = ChangeOpacity(overLayer, trans);
                gr.DrawImage(o, new Rectangle(x, y, overLayer.Width, overLayer.Height));
            }
            return img;
        }
        internal static Bitmap ChangeOpacity(Image img, double trans)
        {
            if (img == null)
                return null;
            if (img.PixelFormat.HasFlag(PixelFormat.Indexed))
                return (Bitmap)img;

            Bitmap bmp = (Bitmap)img.Clone();
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;

            int len = bmp.Width*bmp.Height*4;
            byte[] data = new byte[len];

            Marshal.Copy(ptr, data, 0, len);

            for (int i = 0; i < data.Length; i += 4)
                data[i + 3] = (byte)(data[i + 3] * trans);

            Marshal.Copy(data, 0, ptr, len);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        // Strings and Paths
        internal static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
               .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        internal static string GetTempFolder()
        {
            return Path.Combine(Path.GetTempPath(), "3DSSE");
        }
        internal static string GetCacheFolder()
        {
            return Path.Combine(GetBackupLocation(), "cache");
        }
        internal static string GetRegistryValue(string key)
        {
            Microsoft.Win32.RegistryKey currentUser = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey key3 = currentUser.OpenSubKey(GetRegistryBase());
            if (key3 == null)
                return null;

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
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "3DSSaveBank");
            Directory.CreateDirectory(path);
            return path;
        }
        internal static string get3DSLocation()
        {
            try
            {
                string[] DriveList = Environment.GetLogicalDrives();
                for (int i = 1; i < DriveList.Length; i++) // Skip first drive (some users still have floppy drives and would chew up time!)
                {
                    string potentialPath = Path.Combine(DriveList[i], "Nintendo 3DS");
                    if (Directory.Exists(potentialPath))
                        return potentialPath;
                }
            }
            catch {  }
            return null;
        }
        internal static string GetSDFLocation()
        {
            try
            {
                // Start by checking if the 3DS file path exists or not.
                string path_SDF = null;
                string[] DriveList = Environment.GetLogicalDrives();
                for (int i = 1; i < DriveList.Length; i++) // Skip first drive (some users still have floppy drives and would chew up time!)
                {
                    string potentialPath_SDF = NormalizePath(Path.Combine(DriveList[i], "filer", "UserSaveData"));
                    if (!Directory.Exists(potentialPath_SDF)) continue;

                    path_SDF = potentialPath_SDF; break;
                }
                if (path_SDF == null)
                    return null;
                // 3DS data found in SD card reader. Let's get the title folder location!
                string[] folders = Directory.GetDirectories(path_SDF, "*", SearchOption.TopDirectoryOnly);
                Array.Sort(folders); // Don't need Modified Date, sort by path names just in case.

                // Loop through all the folders in the Nintendo 3DS folder to see if any of them contain 'title'.
                for (int i = folders.Length - 1; i >= 0; i--)
                {
                    if (File.Exists(Path.Combine(folders[i], "000011c4", "main"))) return Path.Combine(folders[i], "000011c4"); // OR
                    if (File.Exists(Path.Combine(folders[i], "000011c5", "main"))) return Path.Combine(folders[i], "000011c5"); // AS
                    if (File.Exists(Path.Combine(folders[i], "0000055d", "main"))) return Path.Combine(folders[i], "0000055d"); // X
                    if (File.Exists(Path.Combine(folders[i], "0000055e", "main"))) return Path.Combine(folders[i], "0000055e"); // Y
                }
                return null; // Fallthrough
            }
            catch { return null; }
        }
        internal static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
        internal static string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            return index < 0 ? input : input.Substring(0, index);
        }
        internal static string[] getStringList(string f)
        {
            object txt = Properties.Resources.ResourceManager.GetObject(f); // Fetch File, \n to list.
            string[] rawlist = ((string)txt).Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].Trim();
            return rawlist;
        }
        internal static string[] getStringList(string f, string l)
        {
            object txt = Properties.Resources.ResourceManager.GetObject("text_" + f + "_" + l); // Fetch File, \n to list.
            string[] rawlist = ((string)txt).Split('\n');
            for (int i = 0; i < rawlist.Length; i++)
                rawlist[i] = rawlist[i].Trim();
            return rawlist;
        }
        internal static string[] getNulledStringArray(string[] SimpleStringList)
        {
            try
            {
                string[] newlist = new string[ToInt32(SimpleStringList[SimpleStringList.Length - 1].Split(',')[0]) + 1];
                for (int i = 1; i < SimpleStringList.Length; i++)
                    newlist[ToInt32(SimpleStringList[i].Split(',')[0])] = SimpleStringList[i].Split(',')[1];
                return newlist;
            }
            catch { return null; }
        }

        // Randomization
        internal static readonly Random rand = new Random();
        internal static uint rnd32()
        {
            return (uint)rand.Next(1 << 30) << 2 | (uint)rand.Next(1 << 2);
        }

        // Data Retrieval
        internal static int ToInt32(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value.Trim(' ', '_'));
        }
        internal static uint ToUInt32(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : uint.Parse(value.Trim(' ', '_'));
        }
        internal static uint getHEXval(string s)
        {
            string str = getOnlyHex(s);
            return string.IsNullOrWhiteSpace(str) ? 0 : Convert.ToUInt32(str, 16);
        }
        internal static int getIndex(ComboBox cb)
        {
            return (int)(cb?.SelectedValue ?? 0);
        }
        internal static string getOnlyHex(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "0" : s.Select(char.ToUpper).Where("0123456789ABCDEF".Contains).Aggregate("", (str, c) => str + c);
        }

        // Data Manipulation
        internal static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + (int)(rand.NextDouble() * (n - i));
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        // Form Translation
        internal static void TranslateInterface(Control form, string lang)
        {
            // Check to see if a the translation file exists in the same folder as the executable
            string externalLangPath = "lang_" + lang + ".txt";
            string[] rawlist;
            if (File.Exists(externalLangPath))
                rawlist = File.ReadAllLines(externalLangPath);
            else
            {
                object txt = Properties.Resources.ResourceManager.GetObject("lang_" + lang);
                if (txt == null) return; // Translation file does not exist as a resource; abort this function and don't translate UI.
                rawlist = ((string)txt).Split(new[] { "\n" }, StringSplitOptions.None);
                rawlist = rawlist.Select(i => i.Trim()).ToArray(); // Remove trailing spaces
            }

            string[] stringdata = new string[rawlist.Length];
            int itemsToRename = 0;
            for (int i = 0; i < rawlist.Length; i++)
            {
                // Find our starting point
                if (!rawlist[i].Contains("! " + form.Name)) continue;

                // Allow renaming of the Window Title
                string[] WindowName = rawlist[i].Split(new[] {" = "}, StringSplitOptions.None);
                if (WindowName.Length > 1) form.Text = WindowName[1];
                // Copy our Control Names and Text to a new array for later processing.
                for (int j = i + 1; j < rawlist.Length; j++)
                {
                    if (rawlist[j].Length == 0) continue; // Skip Over Empty Lines, errhandled
                    if (rawlist[j][0] == '-') continue; // Keep translating if line is a comment line
                    if (rawlist[j][0] == '!') // Stop if we have reached the end of translation
                        goto rename;
                    stringdata[itemsToRename] = rawlist[j]; // Add the entry to process later.
                    itemsToRename++;
                }
            }
            return; // Not Found

            // Now that we have our items to rename in: Control = Text format, let's execute the changes!
        rename:
            for (int i = 0; i < itemsToRename; i++)
            {
                string[] SplitString = stringdata[i].Split(new[] {" = "}, StringSplitOptions.None);
                if (SplitString.Length < 2)
                    continue; // Error in Input, errhandled
                string ctrl = SplitString[0]; // Control to change the text of...
                string text = SplitString[1]; // Text to set Control.Text to...
                Control[] controllist = form.Controls.Find(ctrl, true);
                if (controllist.Length != 0) // If Control is found
                { controllist[0].Text = text; goto next; }
                
                // Check MenuStrips
                foreach (MenuStrip menu in form.Controls.OfType<MenuStrip>())
                {
                    // Menu Items aren't in the Form's Control array. Find within the menu's Control array.
                    ToolStripItem[] TSI = menu.Items.Find(ctrl, true);
                    if (TSI.Length <= 0) continue;
                    
                    TSI[0].Text = text; goto next;
                }
                // Check ContextMenuStrips
                foreach (ContextMenuStrip cs in FindContextMenuStrips(form.Controls.OfType<Control>()).Distinct())
                {
                    ToolStripItem[] TSI = cs.Items.Find(ctrl, true);
                    if (TSI.Length <= 0) continue;

                    TSI[0].Text = text; goto next;
                }

                next:;
            }
        }
        internal static List<ContextMenuStrip> FindContextMenuStrips(IEnumerable<Control> c)
        {
            List<ContextMenuStrip> cs = new List<ContextMenuStrip>();
            foreach (Control control in c)
            {
                if (control.ContextMenuStrip != null)
                    cs.Add(control.ContextMenuStrip);

                else if (control.Controls.Count > 0)
                    cs.AddRange(FindContextMenuStrips(control.Controls.OfType<Control>()));
            }
            return cs;
        }
        internal static void CenterToForm(Control child, Control parent)
        {
            int x = parent.Location.X + (parent.Width - child.Width) / 2;
            int y = parent.Location.Y + (parent.Height - child.Height) / 2;
            child.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
        }

        // Message Displays
        internal static DialogResult Error(params string[] lines)
        {
            System.Media.SystemSounds.Exclamation.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        internal static DialogResult Alert(params string[] lines)
        {
            System.Media.SystemSounds.Asterisk.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        internal static DialogResult Prompt(MessageBoxButtons btn, params string[] lines)
        {
            System.Media.SystemSounds.Question.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Prompt", btn, MessageBoxIcon.Asterisk);
        }

        // DataSource Providing
        public class cbItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
        }
        internal static List<cbItem> getCBList(string textfile, string lang)
        {
            // Set up
            string[] inputCSV = getStringList(textfile);

            // Get Language we're fetching for
            int index = Array.IndexOf(new[] { "ja", "en", "fr", "de", "it", "es", "ko", "zh", }, lang);

            // Set up our Temporary Storage
            string[] unsortedList = new string[inputCSV.Length - 1];
            int[] indexes = new int[inputCSV.Length - 1];

            // Gather our data from the input file
            for (int i = 1; i < inputCSV.Length; i++)
            {
                string[] countryData = inputCSV[i].Split(',');
                indexes[i - 1] = Convert.ToInt32(countryData[0]);
                unsortedList[i - 1] = countryData[index + 1];
            }

            // Sort our input data
            string[] sortedList = new string[inputCSV.Length - 1];
            Array.Copy(unsortedList, sortedList, unsortedList.Length);
            Array.Sort(sortedList);

            // Arrange the input data based on original number
            return sortedList.Select(s => new cbItem
            {
                Text = s, 
                Value = indexes[Array.IndexOf(unsortedList, s)]
            }).ToList();
        }
        internal static List<cbItem> getCBList(string[] inStrings, params int[][] allowed)
        {
            List<cbItem> cbList = new List<cbItem>();
            if (allowed?.First() == null)
                allowed = new[] { Enumerable.Range(0, inStrings.Length).ToArray() };

            foreach (int[] list in allowed)
            {
                // Sort the Rest based on String Name
                string[] unsortedChoices = new string[list.Length];
                for (int i = 0; i < list.Length; i++)
                    unsortedChoices[i] = inStrings[list[i]];

                string[] sortedChoices = new string[unsortedChoices.Length];
                Array.Copy(unsortedChoices, sortedChoices, unsortedChoices.Length);
                Array.Sort(sortedChoices);

                // Add the rest of the items
                cbList.AddRange(sortedChoices.Select(s => new cbItem
                {
                    Text = s, 
                    Value = list[Array.IndexOf(unsortedChoices, s)]
                }));
            }
            return cbList;
        }
        internal static List<cbItem> getOffsetCBList(List<cbItem> cbList, string[] inStrings, int offset, int[] allowed)
        {
            if (allowed == null)
                allowed = Enumerable.Range(0, inStrings.Length).ToArray();

            int[] list = (int[])allowed.Clone();
            for (int i = 0; i < list.Length; i++)
                list[i] -= offset;
            
            // Sort the Rest based on String Name
            string[] unsortedChoices = new string[allowed.Length];
            for (int i = 0; i < allowed.Length; i++)
                unsortedChoices[i] = inStrings[list[i]];

            string[] sortedChoices = new string[unsortedChoices.Length];
            Array.Copy(unsortedChoices, sortedChoices, unsortedChoices.Length);
            Array.Sort(sortedChoices);

            // Add the rest of the items
            cbList.AddRange(sortedChoices.Select(s => new cbItem
            {
                Text = s, Value = allowed[Array.IndexOf(unsortedChoices, s)]
            }));
            return cbList;
        }
        internal static List<cbItem> getVariedCBList(string[] inStrings, int[] stringNum, int[] stringVal)
        {
            // Set up
            List<cbItem> newlist = new List<cbItem>();

            for (int i = 4; i > 1; i--) // add 4,3,2
            {
                // First 3 Balls are always first
                cbItem ncbi = new cbItem
                {
                    Text = inStrings[i], 
                    Value = i
                };
                newlist.Add(ncbi);
            }

            // Sort the Rest based on String Name
            string[] ballnames = new string[stringNum.Length];
            for (int i = 0; i < stringNum.Length; i++)
                ballnames[i] = inStrings[stringNum[i]];

            string[] sortedballs = new string[stringNum.Length];
            Array.Copy(ballnames, sortedballs, ballnames.Length);
            Array.Sort(sortedballs);

            // Add the rest of the balls
            newlist.AddRange(sortedballs.Select(s => new cbItem
            {
                Text = s, 
                Value = stringVal[Array.IndexOf(ballnames, s)]
            }));
            return newlist;
        }
        internal static List<cbItem> getUnsortedCBList(string textfile)
        {
            // Set up
            List<cbItem> cbList = new List<cbItem>();
            string[] inputCSV = getStringList(textfile);

            // Gather our data from the input file
            for (int i = 1; i < inputCSV.Length; i++)
            {
                string[] inputData = inputCSV[i].Split(',');
                cbItem ncbi = new cbItem 
                {
                    Text = inputData[1], 
                    Value = Convert.ToInt32(inputData[0])
                };
                cbList.Add(ncbi);
            }
            return cbList;
        }

        // QR Utility
        internal static byte[] getQRData()
        {
            // Fetch data from QR code...
            string address;
            try { address = Clipboard.GetText(); }
            catch { Alert("No text (url) in clipboard."); return null; }
            try { if (address.Length < 4 || address.Substring(0, 3) != "htt") { Alert("Clipboard text is not a valid URL:", address); return null; } }
            catch { Alert("Clipboard text is not a valid URL:", address); return null; }
            string webURL = "http://api.qrserver.com/v1/read-qr-code/?fileurl=" + System.Web.HttpUtility.UrlEncode(address);
            try
            {
                System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(webURL);
                System.Net.HttpWebResponse httpWebReponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                var reader = new StreamReader(httpWebReponse.GetResponseStream());
                string data = reader.ReadToEnd();
                if (data.Contains("could not find")) { Alert("Reader could not find QR data in the image."); return null; }
                if (data.Contains("filetype not supported")) { Alert("Input URL is not valid. Double check that it is an image (jpg/png).", address); return null; }
                // Quickly convert the json response to a data string
                string pkstr = data.Substring(data.IndexOf("#", StringComparison.Ordinal) + 1); // Trim intro
                pkstr = pkstr.Substring(0, pkstr.IndexOf("\",\"error\":null}]}]", StringComparison.Ordinal)); // Trim outro
                if (pkstr.Contains("nQR-Code:")) pkstr = pkstr.Substring(0, pkstr.IndexOf("nQR-Code:", StringComparison.Ordinal)); //  Remove multiple QR codes in same image
                pkstr = pkstr.Replace("\\", ""); // Rectify response

                try { return Convert.FromBase64String(pkstr); }
                catch { Alert("QR string to Data failed.", pkstr); return null; }
            }
            catch { Alert("Unable to connect to the internet to decode QR code."); return null;}
        }
        internal static Image getQRImage(byte[] data, string server)
        {
            string qrdata = Convert.ToBase64String(data);
            string message = server + qrdata;
            string webURL = "http://chart.apis.google.com/chart?chs=365x365&cht=qr&chl=" + System.Web.HttpUtility.UrlEncode(message);

            try
            {
                System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(webURL);
                System.Net.HttpWebResponse httpWebReponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                Stream stream = httpWebReponse.GetResponseStream();
                if (stream != null) return Image.FromStream(stream);
            }
            catch
            {
                if (DialogResult.Yes != Prompt(MessageBoxButtons.YesNo, "Unable to connect to the internet to receive QR code.", "Copy QR URL to Clipboard?"))
                    return null;
                try { Clipboard.SetText(webURL); }
                catch { Alert("Failed to set text to Clipboard"); }
            }
            return null;
        }
    }
}