using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class Main : Form
    {
        public Main()
        {
            #region Pop open a splash screen while we load up.
            new Thread(() => new SplashScreen().ShowDialog()).Start();
            #endregion
            #region Initialize Form
            pk6.RefreshChecksum();
            InitializeComponent();
            // Initialize SAV-Set Parameters in case compilation settings were changed.
            SAV6.SetUpdateDex = Menu_ModifyDex.Checked;
            SAV6.SetUpdatePK6 = Menu_ModifyPK6.Checked;
            SlotPictureBoxes = new[] {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                                };
            defaultControlWhite = CB_Species.BackColor;
            defaultControlText = Label_Species.ForeColor;
            CB_ExtraBytes.SelectedIndex = 0;

            // Initialize Boxes
            for (int i = 0; i < 30*31; i++)
                SAV.setData(blankEK6, SAV.Box + i*PK6.SIZE_STORED);

            // Set up Language Selection
            foreach (var cbItem in main_langlist)
                CB_MainLanguage.Items.Add(cbItem);

            // ToolTips for Drag&Drop
            new ToolTip().SetToolTip(dragout, "PK6 QuickSave");

            // Box Drag & Drop
            foreach (PictureBox pb in PAN_Box.Controls)
            {
                pb.AllowDrop = true;
                // The PictureBoxes have their own drag&drop event handlers.
            }
            foreach (TabPage tab in tabMain.TabPages)
            {
                tab.AllowDrop = true;
                tab.DragDrop += tabMain_DragDrop;
                tab.DragEnter += tabMain_DragEnter;
            }
            foreach (TabPage tab in tabBoxMulti.TabPages)
            {
                tab.AllowDrop = true;
                tab.DragDrop += tabMain_DragDrop;
                tab.DragEnter += tabMain_DragEnter;
            }
            // Box to Tabs D&D
            dragout.AllowDrop = true;

            string[] args = Environment.GetCommandLineArgs();
            string filename = args.Length > 0 ? Path.GetFileNameWithoutExtension(args[0]) : "";
            HaX = filename.IndexOf("HaX", StringComparison.Ordinal) >= 0;
            // Show Hacked Stuff if HaX
            CHK_HackedStats.Enabled = CHK_HackedStats.Visible = DEV_Ability.Enabled = DEV_Ability.Visible =
            MT_Level.Enabled = MT_Level.Visible = TB_AbilityNumber.Visible = MT_Form.Enabled = MT_Form.Visible = HaX;
            // Hide Regular Stuff if !HaX
            TB_Level.Visible = CB_Ability.Visible = !HaX;
            #endregion
            #region Localize & Populate Fields
            // Try and detect the language
            int[] main_langnum = {1, 2, 3, 4, 5, 7, 8, 9};
            main_langnum = main_langnum.Concat(Enumerable.Range(10, lang_val.Length).Select(i => i).ToArray()).ToArray();
            string lastTwoChars = filename.Length > 2 ? filename.Substring(filename.Length - 2) : "";
            int lang = filename.Length > 2 ? Array.IndexOf(lang_val, lastTwoChars) : - 1;
            CB_MainLanguage.SelectedIndex = lang >= 0 ? main_langnum[lang] - 1 : (lastTwoChars == "jp" ? 1 : 0);

            InitializeFields();
            CB_Language.SelectedIndex = lang >= 0 && lang < 7 ? main_langnum[lang] : 1;
            #endregion
            #region Load Initial File(s)
            // Load the arguments
            pathSDF = Util.GetSDFLocation();
            path3DS = Util.get3DSLocation();
            if (args.Length > 1)
            {
                foreach (string arg in args.Skip(1).Where(a => a.Length > 4))
                    openQuick(arg);
            }
            else if (path3DS != null && File.Exists(Path.Combine(Path.GetPathRoot(path3DS), "ramsav.bin")))
                openQuick(Path.Combine(Path.GetPathRoot(path3DS), "ramsav.bin"));
            else if (path3DS != null && File.Exists(Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup", "main")))
                openQuick(Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup", "main"));
            else if (pathSDF != null)
                openQuick(Path.Combine(pathSDF, "main"));
            else if (File.Exists(Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root", "main"))))
                openQuick(Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root", "main")));

            GB_OT.Click += clickGT;
            GB_nOT.Click += clickGT;
            GB_Daycare.Click += switchDaycare;
            GB_RelearnMoves.Click += clickMoves;

            TB_Nickname.Font = PKX.getPKXFont(11);
            TB_OT.Font = (Font)TB_Nickname.Font.Clone();
            TB_OTt2.Font = (Font)TB_Nickname.Font.Clone();
            formInitialized = true;

            // Splash Screen closes on its own.
            BringToFront();
            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
            if (HaX) Util.Alert("Illegal mode activated.", "Please behave.");
            #endregion
        }

        #region Global Variables: Always Visible!
        public static readonly byte[] blankEK6 = PKX.encryptArray(new byte[PK6.SIZE_PARTY]);
        public static PK6 pk6 = new PK6(); // Tab Pokemon Data Storage
        public static SAV6 SAV = new SAV6();
        public static byte[] originalSAV; // original save for CyberGadget Codes
        public static byte[] ramsav; // original ramsav for ramsav exporting
        public static string pathSDF;
        public static string path3DS;

        public static volatile bool formInitialized, fieldsInitialized;
        public static bool HaX;
        public static bool specialChars; // Open Form Tracking
        public static Color defaultControlWhite;
        public static Color defaultControlText;
        public static int colorizedbox = 32;
        public static Image mixedHighlight = Util.ChangeOpacity(Properties.Resources.slotSet, 0.5);
        public static Image colorizedcolor;
        public static int colorizedslot;
        public static string eggname = "";
        public static string DatabasePath = "db";
        public static string[] lang_val = { "en", "ja", "fr", "it", "de", "es", "ko", "zh", "pt" };
        public static string[] main_langlist = 
            {
                "English", // ENG
                "日本語", // JPN
                "Français", // FRE
                "Italiano", // ITA
                "Deutsch", // GER
                "Español", // SPA
                "한국어", // KOR
                "中文", // CHN
                "Português", // Portuguese
            };
        public static string[] gendersymbols = { "♂", "♀", "-" };
        public static string[] specieslist, movelist, itemlist, abilitylist, types, natures, forms, 
            memories, genloc, trainingbags, trainingstage, characteristics, 
            encountertypelist, gamelanguages, consoleregions, balllist, gamelist = { };
        public static string origintrack;
        public static string[] metHGSS_00000, metHGSS_02000, metHGSS_03000 = { };
        public static string[] metBW2_00000, metBW2_30000, metBW2_40000, metBW2_60000 = { };
        public static string[] metXY_00000, metXY_30000, metXY_40000, metXY_60000 = { };
        public static string[] wallpapernames, puffs, itempouch = { };
        public static string curlanguage = "en";
        public static bool unicode;
        public ToolTip Tip1 = new ToolTip(), Tip2 = new ToolTip(), Tip3 = new ToolTip(), NatureTip = new ToolTip();
        public static List<Util.cbItem> MoveDataSource, ItemDataSource, SpeciesDataSource, BallDataSource, NatureDataSource, AbilityDataSource, VersionDataSource;
        public readonly PictureBox[] SlotPictureBoxes;
        #endregion

        #region //// MAIN MENU FUNCTIONS ////
        // Main Menu Strip UI Functions
        private void mainMenuOpen(object sender, EventArgs e)
        {
            string cyberpath = Util.GetTempFolder();
            pathSDF = Util.GetSDFLocation();
            path3DS = Util.get3DSLocation();
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "PKX File|*.pk6;*.pkx" +
                         "|EKX File|*.ek6;*.ekx" +
                         "|BIN File|*.bin" +
                         "|All Files|*.*",
            };
            if (path3DS != null && File.Exists(Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup", "main")))
            {
                ofd.InitialDirectory = Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup");
                ofd.RestoreDirectory = true;
                ofd.FilterIndex = 4;
            }
            else if (pathSDF != null)
            {
                ofd.InitialDirectory = pathSDF;
                ofd.RestoreDirectory = true;
                ofd.FilterIndex = 4;
            }
            else if (path3DS != null)
            {
                ofd.InitialDirectory = Path.GetPathRoot(path3DS);
                ofd.RestoreDirectory = true;
                ofd.FilterIndex = 4;
            }
            else if (Directory.Exists(Path.Combine(cyberpath, "root")))
            {
                ofd.InitialDirectory = Path.Combine(cyberpath, "root");
                ofd.RestoreDirectory = true;
                ofd.FilterIndex = 4;
            }
            else if (Directory.Exists(cyberpath))
            {
                ofd.InitialDirectory = cyberpath;
                ofd.RestoreDirectory = true;
                ofd.FilterIndex = 4;
            }

            if (ofd.ShowDialog() != DialogResult.OK) 
                return;

            openQuick(ofd.FileName);
        }
        private void mainMenuSave(object sender, EventArgs e)
        {
            if (!verifiedPKX()) { return; }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PKX File|*.pk6;*.pkx" +
                         "|EKX File|*.ek6;*.ekx" +
                         "|BIN File|*.bin" +
                         "|All Files|*.*",
                DefaultExt = "pk6",
                FileName = TB_Nickname.Text + " - " + TB_PID.Text
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            string path = sfd.FileName;
            // Injection Dummy Override
            if (path.Contains("pokemon.ekx")) path = Path.Combine(Path.GetDirectoryName(path), "pokemon.ekx");
            string ext = Path.GetExtension(path);

            if (File.Exists(path) && !path.Contains("pokemon.ekx"))
            {
                // File already exists, save a .bak
                byte[] backupfile = File.ReadAllBytes(path);
                File.WriteAllBytes(path + ".bak", backupfile);
            }
            byte[] pkx = preparepkx();

            if ((ext == ".ekx") || (ext == ".bin") || (ext == ".pkx") || (ext == ".ek6") || (ext == ".pk6"))
            {
                if ((ext == ".ekx") || (ext == ".bin") || (ext == ".ek6")) // User Requested Encrypted File
                    pkx = PKX.encryptArray(pkx);
                File.WriteAllBytes(path, pkx.ToArray());
            }
            else
            {
                Util.Error($"Foreign File Extension: {ext}", "Exporting as encrypted.");
                pkx = PKX.encryptArray(pkx);
                File.WriteAllBytes(path, pkx);
            }
        }
        private void mainMenuExit(object sender, EventArgs e)
        {
            if (ModifierKeys == (Keys.Control | Keys.E)) // Hotkey Triggered
                if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Quit PKHeX?")) return;
            Close();
        }
        private void mainMenuAbout(object sender, EventArgs e)
        {
            // Open a new form with the About details.
            new About().ShowDialog();
        }
        // Sub Menu Options
        private void mainMenuCodeGen(object sender, EventArgs e)
        {
            // Open Code Generator
            byte[] formdata = null;
            if (verifiedPKX()) formdata = preparepkx();
            CodeGenerator CodeGen = new CodeGenerator(formdata);
            CodeGen.ShowDialog();
            byte[] data = CodeGen.returnArray;
            if (data == null) return;
            byte[] decdata = PKX.decryptArray(data);
            Array.Copy(decdata, pk6.Data, PK6.SIZE_STORED);
            try { populateFields(pk6.Data); }
            catch
            {
                Array.Copy(new byte[PK6.SIZE_STORED], pk6.Data, PK6.SIZE_STORED);
                populateFields(pk6.Data);
                Util.Error("Imported code did not decrypt properly", "Please verify that what you imported was correct.");
            }
        }
        private void mainMenuBoxReport(object sender, EventArgs e)
        {
            frmReport ReportForm = new frmReport();
            ReportForm.Show();
            ReportForm.PopulateData(SAV.Data, SAV.Box);
        }
        private void mainMenuUnicode(object sender, EventArgs e)
        {
            unicode = gendersymbols[0] == "♂";
            if (unicode)
            {
                gendersymbols = new[] { "M", "F", "-" };
                BTN_Shinytize.Text = "*";
                TB_Nickname.Font = TB_OT.Font = TB_OTt2.Font = Label_TID.Font;
            }
            else
            {
                gendersymbols = new[] { "♂", "♀", "-" };
                BTN_Shinytize.Text = "☆";
                TB_Nickname.Font = TB_OT.Font = TB_OTt2.Font = PKX.getPKXFont(11);
            }
            // Switch active gender labels to new if they are active.
            if (PKX.getGender(Label_Gender.Text) < 2)
                Label_Gender.Text = gendersymbols[PKX.getGender(Label_Gender.Text)];
            if (PKX.getGender(Label_OTGender.Text) < 2)
                Label_OTGender.Text = gendersymbols[PKX.getGender(Label_OTGender.Text)];
            if (PKX.getGender(Label_CTGender.Text) < 2)
                Label_CTGender.Text = gendersymbols[PKX.getGender(Label_CTGender.Text)];
        }
        private void mainMenuModifyDex(object sender, EventArgs e)
        {
            SAV6.SetUpdateDex = Menu_ModifyDex.Checked;
        }
        private void mainMenuModifyPK6(object sender, EventArgs e)
        {
            SAV6.SetUpdatePK6 = Menu_ModifyPK6.Checked;
        }
        private void mainMenuBoxLoad(object sender, EventArgs e)
        {
            string path = "";
            if (Directory.Exists(DatabasePath))
            {
                DialogResult ld = Util.Prompt(MessageBoxButtons.YesNo, "Load from PKHeX's database?");
                if (ld == DialogResult.Yes)
                    path = DatabasePath;
                else if (ld == DialogResult.No)
                {
                    // open folder dialog
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                        path = fbd.SelectedPath;
                }
                else return;
            }
            else
            {
                // open folder dialog
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                    path = fbd.SelectedPath;
            }
            loadBoxesFromDB(path);
        }
        private void mainMenuBoxDump(object sender, EventArgs e)
        {
            string path;
            bool dumptoboxes = false;
            // Dump all of box content to files.
            DialogResult ld = Util.Prompt(MessageBoxButtons.YesNo, "Save to PKHeX's database?");
            if (ld == DialogResult.Yes)
            {
                path = DatabasePath;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            else if (ld == DialogResult.No)
            {
                dumptoboxes = DialogResult.Yes == Util.Prompt(MessageBoxButtons.YesNo, "Save each box separately?");

                // open folder dialog
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                path = fbd.SelectedPath;
            }
            else return;

            dumpBoxesToDB(path, dumptoboxes);
        }
        // Misc Options
        private void clickShowdownImportPK6(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
            { Util.Alert("Clipboard does not contain text."); return; }

            // Get Simulator Data
            PKX.ShowdownSet Set = new PKX.ShowdownSet(Clipboard.GetText());

            if (Set.Species < 0)
            { Util.Alert("Set data not found in clipboard."); return; }

            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Import this set?", Clipboard.GetText())) 
            { return; }

            // Set Species & Nickname
            CB_Species.SelectedValue = Set.Species;
            CHK_Nicknamed.Checked = Set.Nickname != null;
            if (Set.Nickname != null)
                TB_Nickname.Text = Set.Nickname;
            if (Set.Gender != null && PKX.getGender(Set.Gender) != 2 && PKX.getGender(Set.Gender) != 2)
            {
                int Gender = PKX.getGender(Set.Gender);
                Label_Gender.Text = gendersymbols[Gender];
                Label_Gender.ForeColor = Gender == 2 ? Label_Species.ForeColor : (Gender == 1 ? Color.Red : Color.Blue);
            }

            // Set Form
            string[] formStrings = PKX.getFormList(Set.Species,
                Util.getStringList("Types", "en"),
                Util.getStringList("Forms", "en"), gendersymbols);
            int form = 0;
            for (int i = 0; i < formStrings.Length; i++)
                if (formStrings[i].Contains(Set.Form ?? ""))
                { form = i; break; }
            CB_Form.SelectedIndex = form;

            // Set Ability
            byte[] abilities = PKX.getAbilities(Set.Species, form);
            int ability = Array.IndexOf(abilities, (byte)Set.Ability);
            if (ability < 0) ability = 0;
            CB_Ability.SelectedIndex = ability;
            ComboBox[] m = { CB_Move1, CB_Move2, CB_Move3, CB_Move4, };
            for (int i = 0; i < 4; i++) m[i].SelectedValue = Set.Moves[i];

            // Set Item and Nature
            CB_HeldItem.SelectedValue = Set.Item < 0 ? 0 : Set.Item;
            CB_Nature.SelectedValue = Set.Nature < 0 ? 0 : Set.Nature;

            // Set IVs
            TB_HPIV.Text = Set.IVs[0].ToString();
            TB_ATKIV.Text = Set.IVs[1].ToString();
            TB_DEFIV.Text = Set.IVs[2].ToString();
            TB_SPAIV.Text = Set.IVs[3].ToString();
            TB_SPDIV.Text = Set.IVs[4].ToString();
            TB_SPEIV.Text = Set.IVs[5].ToString();

            // Set EVs
            TB_HPEV.Text = Set.EVs[0].ToString();
            TB_ATKEV.Text = Set.EVs[1].ToString();
            TB_DEFEV.Text = Set.EVs[2].ToString();
            TB_SPAEV.Text = Set.EVs[3].ToString();
            TB_SPDEV.Text = Set.EVs[4].ToString();
            TB_SPEEV.Text = Set.EVs[5].ToString();

            // Set Level and Friendship
            TB_Level.Text = Set.Level.ToString();
            TB_Friendship.Text = Set.Friendship.ToString();

            // Reset IV/EVs
            BTN_RerollPID.PerformClick();
            BTN_RerollEC.PerformClick();
            if (Set.Shiny) BTN_Shinytize.PerformClick();
        }
        private void clickShowdownExportPK6(object sender, EventArgs e)
        {
            if (!verifiedPKX())
            { Util.Alert("Fix data before exporting."); return; }

            Clipboard.SetText(new PK6(preparepkx()).ShowdownText);
            Util.Alert("Exported Showdown Set to Clipboard:", Clipboard.GetText());
        }
        private void clickShowdownExportParty(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(
                    SAV.PartyData.Aggregate("", (current, pk) => current + pk.ShowdownText
                            + Environment.NewLine + Environment.NewLine).Trim());
                Util.Alert("Showdown Team (Party) set to Clipboard.");
            }
            catch { }
        }
        private void clickShowdownExportBattleBox(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(
                    SAV.BattleBoxData.Aggregate("", (current, pk) => current + pk.ShowdownText
                            + Environment.NewLine + Environment.NewLine).Trim());
                Util.Alert("Showdown Team (Battle Box) set to Clipboard.");
            }
            catch { }
        }
        private void clickOpenTempFolder(object sender, EventArgs e)
        {
            string path = Util.GetTempFolder();
            if (Directory.Exists(Path.Combine(path, "root")))
                Process.Start("explorer.exe", @Path.Combine(path, "root"));
            else if (Directory.Exists(path))
                Process.Start("explorer.exe", @path);
            else
                Util.Alert("Can't find the temporary file.", "Make sure the Cyber Gadget software is paused.");
        }
        private void clickOpenCacheFolder(object sender, EventArgs e)
        {
            string path = Util.GetCacheFolder();
            if (Directory.Exists(path))
                Process.Start("explorer.exe", @path);
            else
                Util.Alert("Can't find the cache folder.");
        }
        private void clickOpenSDFFolder(object sender, EventArgs e)
        {
            string path;
            if (path3DS != null && Directory.Exists(path = Util.GetSDFLocation()))
                Process.Start("explorer.exe", @path);
            else
                Util.Alert("Can't find the SaveDataFiler folder.");
        }
        private void clickOpenSDBFolder(object sender, EventArgs e)
        {
            string path;
            if (path3DS != null && Directory.Exists(path = Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup")))
                Process.Start("explorer.exe", @path);
            else
                Util.Alert("Can't find the SaveDataBackup folder.");
        }

        // Main Menu Subfunctions
        private void openQuick(string path)
        {
            // detect if it is a folder (load into boxes or not)
            if (Directory.Exists(path))
            { loadBoxesFromDB(path); return; }

            string ext = Path.GetExtension(path);
            FileInfo fi = new FileInfo(path);
            if (fi.Length > 0x10009C)
                Util.Error("Input file is too large.", path);
            else
            {
                byte[] input; try { input = File.ReadAllBytes(path); }
                catch { Util.Error("File is in use by another program!", path); return; }

                try { openFile(input, path, ext); }
                catch
                {
                    try
                    {
                        byte[] blank = (byte[])blankEK6.Clone();

                        for (int i = 0; i < PK6.SIZE_STORED; i++)
                            blank[i] ^= input[i];

                        openFile(blank, path, ext);
                    }
                    catch { openFile(input, path, ext); }
                }
            }
        }
        private void openFile(byte[] input, string path, string ext)
        {
            #region Powersaves Read-Only Conversion
            if (input.Length == 0x10009C) // Resize to 1MB
            {
                Array.Copy(input, 0x9C, input, 0, 0x100000);
                Array.Resize(ref input, 0x100000);
            }
            #endregion

            #region Saves
            if ((input.Length == SAV6.SIZE_ORAS) && BitConverter.ToUInt32(input, SAV6.SIZE_ORAS - 0x1F0) == SAV6.BEEF) // ORAS
                openMAIN(input, path);
            else if ((input.Length == SAV6.SIZE_XY) && BitConverter.ToUInt32(input, SAV6.SIZE_XY - 0x1F0) == SAV6.BEEF) // XY
                openMAIN(input, path);
            else if ((input.Length == SAV6.SIZE_ORASDEMO) && BitConverter.ToUInt32(input, SAV6.SIZE_ORASDEMO - 0x1F0) == SAV6.BEEF) // ORAS Demo
                openMAIN(input, path);
            // Verify the Data Input Size is Proper
            else if (input.Length == 0x100000)
            {
                if (openXOR(input, path)) // Check if we can load the save via xorpad
                    return; // only if a save is loaded we abort
                if (BitConverter.ToUInt64(input, 0x10) != 0) // encrypted save
                { Util.Error("PKHeX only edits decrypted save files.", "This save file is not decrypted."); return; }
                
                DialogResult sdr = Util.Prompt(MessageBoxButtons.YesNoCancel, "Press Yes to load the sav at 0x3000", "Press No for the one at 0x82000");
                int savshift = 0;
                if (sdr == DialogResult.Yes)
                    savshift += 0x7F000;
                if (sdr == DialogResult.Cancel)
                    return;
                byte[] psdata = input.Skip(0x5400 + savshift).Take(SAV6.SIZE_ORAS).ToArray();
                if (BitConverter.ToUInt32(psdata, psdata.Length - 0x1F0) != SAV6.BEEF)
                    Array.Resize(ref psdata, SAV6.SIZE_XY);
                if (BitConverter.ToUInt32(psdata, psdata.Length - 0x1F0) != SAV6.BEEF)
                    return;

                openMAIN(psdata, path);
            }
            #endregion
            #region PK6/EK6
            else if ((input.Length == PK6.SIZE_PARTY || input.Length == PK6.SIZE_STORED) && ext != ".pgt")
            {
                // Check if Input is PKX
                if ((ext == ".pk6") || (ext == ".ek6") || (ext == ".pkx") || (ext == ".ekx") || (ext == ".bin") || (ext == ""))
                {
                    // Check if Encrypted before Loading
                    populateFields((BitConverter.ToUInt16(input, 0xC8) == 0 && BitConverter.ToUInt16(input, 0x58) == 0) ? input : PKX.decryptArray(input));
                }
                else
                    Util.Error("Unable to recognize file." + Environment.NewLine + "Only valid .pk* .ek* .bin supported.",
                        $"File Loaded:{Environment.NewLine}{path}");
            }
            #endregion
            #region PK3/PK4/PK5
            else if ((input.Length == 136) || (input.Length == 220) || (input.Length == 236) || (input.Length == 100) || (input.Length == 80)) // to convert g5pkm
            {
                if (!PKX.verifychk(input)) Util.Error("Invalid File (Checksum Error)");
                try // to convert g5pkm
                {
                    byte[] data = Converter.ConvertPKMtoPK6(input);
                    Array.Resize(ref data, PK6.SIZE_STORED);
                    populateFields(data);
                }
                catch
                {
                    populateFields(new byte[PK6.SIZE_STORED]);
                    Util.Error("Attempted to load previous generation PKM.", "Conversion failed.");
                }
            }
            #endregion
            #region Trade Packets
            else if (input.Length == 363 && BitConverter.ToUInt16(input, 0x6B) == 0)
            {
                // EAD Packet of 363 length
                Array.Copy(input, 0x67, pk6.Data, 0, PK6.SIZE_STORED);
                Array.Resize(ref pk6.Data, PK6.SIZE_STORED);
                populateFields(pk6.Data);
            }
            else if (input.Length == 407 && BitConverter.ToUInt16(input, 0x98) == 0)
            {
                // EAD Packet of 407 length
                Array.Copy(input, 0x93, pk6.Data, 0, PK6.SIZE_STORED);
                Array.Resize(ref pk6.Data, PK6.SIZE_STORED);
                populateFields(pk6.Data);
            }
            #endregion
            #region Box Data
            else if ((input.Length == PK6.SIZE_STORED * 30 || input.Length == PK6.SIZE_STORED * 30 * 31) && BitConverter.ToUInt16(input, 4) == 0 && BitConverter.ToUInt32(input, 8) > 0)
            {
                int baseOffset = SAV.Box + PK6.SIZE_STORED * 30 * ((input.Length == PK6.SIZE_STORED * 30) ? CB_BoxSelect.SelectedIndex : 0);
                for (int i = 0; i < input.Length / PK6.SIZE_STORED; i++)
                {
                    byte[] data = input.Skip(PK6.SIZE_STORED * i).Take(PK6.SIZE_STORED).ToArray();
                    SAV.setEK6Stored(data, baseOffset + i * PK6.SIZE_STORED);
                }
                setPKXBoxes();
                Util.Alert("Box Binary loaded.");
            }
            #endregion
            #region injectiondebug
            else if (input.Length == 0x10000)
            { 
                int offset = -1; // Seek to find data start
                for (int i = 0; i < 0x800; i++)
                {
                    byte[] data = PKX.decryptArray(input.Skip(i).Take(PK6.SIZE_STORED).ToArray());
                    if (PKX.getCHK(data) != BitConverter.ToUInt16(data, 6)) continue;
                    offset = i; break;
                }
                if (offset < 0) { Util.Alert(path, "Unable to read the input file; not an expected injectiondebug.bin."); return; }
                CB_BoxSelect.SelectedIndex = 0;
                for (int i = 0; i < input.Length / (9*30); i++)
                {
                    byte[] data = input.Skip(offset + PK6.SIZE_STORED * i).Take(PK6.SIZE_STORED).ToArray();
                    SAV.setEK6Stored(data, SAV.Box + i * PK6.SIZE_STORED);
                }
                setPKXBoxes();
                Util.Alert("Injection Binary loaded."); 
            }
            #endregion
            #region RAMSAV
            else if (( /*XY*/ input.Length == 0x70000 || /*ORAS*/ input.Length == 0x80000) && Path.GetFileName(path).Contains("ram"))
            {
                if (input.Length == 0x80000)
                    // Scan for FEEB in XY location, 3DS only overwrites data if file already exists.
                    for (int i = 0x60000; i < 0x64000; i+=4)
                        if (BitConverter.ToUInt32(input, i) == SAV6.BEEF) { Array.Resize(ref input, 0x70000); break; }

                ramsav = (byte[])input.Clone();
                try { openMAIN(ram2sav.getMAIN(input), path, true); }
                catch { ramsav = null; }
            }
            #endregion
            #region Battle Video
            else if (input.Length == 0x2E60 && BitConverter.ToUInt64(input, 0xE18) != 0 && BitConverter.ToUInt16(input, 0xE12) == 0)
            {
                if (Util.Prompt(MessageBoxButtons.YesNo, "Load Batte Video Pokémon data to " + CB_BoxSelect.Text + "?", "The first 24 slots will be overwritten.") != DialogResult.Yes) return;
                for (int i = 0; i < 24; i++)
                {
                    byte[] data = input.Skip(0xE18 + PK6.SIZE_PARTY * i + (i / 6) * 8).Take(PK6.SIZE_STORED).ToArray();
                    SAV.setEK6Stored(data, SAV.Box + i*PK6.SIZE_STORED + CB_BoxSelect.SelectedIndex*30*PK6.SIZE_STORED);
                }
                setPKXBoxes();
            }
            #endregion
            #region Wondercard
            else if (input.Length == WC6.Size && ext == ".wc6")
                if (ModifierKeys == Keys.Control)
                    new SAV_Wondercard(input).Show();
                else
                {
                    PK6 pk = new WC6(input).convertToPK6(SAV);
                    if (pk == null || pk.Species == 0 || pk.Species > 721)
                    {
                        Util.Error("Failed to convert Wondercard.",
                            (pk == null ? "Not a Pokémon Wondercard." : "Invalid species."));
                        return;
                    }
                    populateFields(pk.Data);
                }
            else if (input.Length == PGF.Size && ext == ".pgf")
            {
                PK5 pk = new PGF(input).convertToPK5(SAV);
                if (pk == null || pk.Species == 0 || pk.Species > 721)
                {
                    Util.Error("Failed to convert PGF.",
                        (pk == null ? "Not a Pokémon PGF." : "Invalid species."));
                    return;
                }
                populateFields(Converter.ConvertPKMtoPK6(pk.Data));
            }
            else if ((input.Length == PGT.Size && ext == ".pgt"))
            {
                PGT pgt = new PGT(input);
                PK4 pk = pgt.convertToPK4(SAV);
                if (pk == null || pk.Species == 0 || pk.Species > 721)
                {
                    Util.Error("Failed to convert PGT.",
                        (pk == null ? "Not a Pokémon PGT." : "Invalid species."));
                    return;
                }
                populateFields(Converter.ConvertPKMtoPK6(pk.Data));
            }
            else if (input.Length == PCD.Size && ext == ".pcd")
            {
                PCD pcd = new PCD(input);
                PGT pgt = pcd.Gift;
                PK4 pk = pgt.convertToPK4(SAV);
                if (pk == null || pk.Species == 0 || pk.Species > 721)
                {
                    Util.Error("Failed to convert PCD.",
                        (pk == null ? "Not a Pokémon PCD." : "Invalid species."));
                    return;
                }
                populateFields(Converter.ConvertPKMtoPK6(pk.Data));
            }
            #endregion
            else
                Util.Error("Attempted to load an unsupported file type/size.",
                    $"File Loaded:{Environment.NewLine}{path}",
                    $"File Size:{Environment.NewLine}{input.Length} bytes (0x{input.Length.ToString("X4")})");
        }
        private bool openXOR(byte[] input, string path)
        {
            // Detection of stored Decryption XORpads:
            if (ModifierKeys == Keys.Control) return false; // no xorpad compatible
            byte[] savID = input.Take(0x10).ToArray();
            string exepath = Application.StartupPath;
            string xorpath = exepath.Clone().ToString();
            string[] XORpads = Directory.GetFiles(xorpath);

            int loop = 0;
        check:
            foreach (byte[] data in from file in XORpads let fi = new FileInfo(file) where (fi.Name.ToLower().Contains("xorpad") || fi.Name.ToLower().Contains("key")) && (fi.Length == 0x10009C || fi.Length == 0x100000) select File.ReadAllBytes(file))
            {
                // Fix xorpad alignment
                byte[] xorpad = data;
                if (xorpad.Length == 0x10009C) // Trim off Powersaves' header
                    xorpad = xorpad.Skip(0x9C).ToArray(); // returns 0x100000

                if (!xorpad.Take(0x10).SequenceEqual(savID)) continue;

                // Set up Decrypted File
                byte[] decryptedPS = input.Skip(0x5400).Take(SAV6.SIZE_ORAS).ToArray();

                // xor through and decrypt
                for (int z = 0; z < decryptedPS.Length; z++)
                    decryptedPS[z] ^= xorpad[0x5400 + z];

                // Weakly check the validity of the decrypted content
                if (BitConverter.ToUInt32(decryptedPS, SAV6.SIZE_ORAS - 0x1F0) != SAV6.BEEF) // Not OR/AS
                    if (BitConverter.ToUInt32(decryptedPS, SAV6.SIZE_XY - 0x1F0) != SAV6.BEEF) // Not X/Y
                        continue;
                    else
                        Array.Resize(ref decryptedPS, SAV6.SIZE_XY); // set to X/Y size
                else Array.Resize(ref decryptedPS, SAV6.SIZE_ORAS); // set to ORAS size just in case

                // Save file is now decrypted!

                // Trigger Loading of the decrypted save file.
                openMAIN(decryptedPS, path);

                // Abort the opening of a non-cyber file.
                return true;
            }
            // End file check loop, check the input path for xorpads too if it isn't the same as the EXE (quite common).
            if (xorpath != exepath || loop++ > 0) return false; // no xorpad compatible
            xorpath = Path.GetDirectoryName(path); goto check;
        }
        private void openMAIN(byte[] input, string path, bool ram = false)
        {
            if (!ram)
                ramsav = null;
            L_Save.Text = "SAV: " + Path.GetFileName(path);
            SAV = new SAV6(input);

            // Enable Secondary Tools
            GB_SAVtools.Enabled =
                B_JPEG.Enabled = true;

            SAV.Edited = false;

            Menu_ExportSAV.Enabled = SAV.Exportable;
            Menu_ExportTransfer.Enabled = Menu_ExportRAMSAV.Enabled = ramsav != null;
            B_VerifyCHK.Enabled = ramsav == null;
            DaycareSlot = 0;
            
            setBoxNames();   // Display the Box Names
            setPKXBoxes();   // Reload all of the PKX Windows

            // Version Exclusive Editors
            GB_SUBE.Visible = !SAV.ORAS;
            B_OpenSecretBase.Visible = SAV.ORAS;

            if (SAV.Box > -1)
            {
                int startBox = SAV.CurrentBox; // FF if BattleBox
                if (startBox > 30) { tabBoxMulti.SelectedIndex = 1; CB_BoxSelect.SelectedIndex = 0; }
                else { tabBoxMulti.SelectedIndex = 0; CB_BoxSelect.SelectedIndex = startBox; }
            }

            TB_GameSync.Enabled = SAV.GameSyncID != 0;
            TB_GameSync.Text = SAV.GameSyncID.ToString("X16");
            TB_Secure1.Text = SAV.Secure1.ToString("X16");
            TB_Secure2.Text = SAV.Secure2.ToString("X16");
            PB_Locked.Visible = SAV.BattleBoxLocked;

            // Hide content if not present in game.
            PAN_Box.Visible = CB_BoxSelect.Visible = B_BoxLeft.Visible = B_BoxRight.Visible = SAV.Box > -1;
            Menu_LoadBoxes.Enabled = Menu_Report.Enabled = Menu_Modify.Enabled = B_SaveBoxBin.Enabled = SAV.Box > -1;
            PAN_BattleBox.Visible = L_BattleBox.Visible = L_ReadOnlyPBB.Visible = SAV.BattleBox > -1;
            GB_Daycare.Visible = SAV.Daycare > -1;
            GB_Fused.Visible = SAV.Fused > -1;
            GB_GTS.Visible = SAV.GTS > -1;
            B_OpenSecretBase.Enabled = SAV.SecretBase > -1;
            B_OpenPokepuffs.Enabled = SAV.Puff > -1;
            B_OUTPasserby.Enabled = SAV.PSS > -1;
            B_OpenBoxLayout.Enabled = SAV.BoxWallpapers > -1;
            B_OpenWondercards.Enabled = SAV.WondercardFlags > -1;
            B_OpenSuperTraining.Enabled = SAV.SuperTrain > -1;
            B_OpenHallofFame.Enabled = SAV.HoF > -1;
            B_OpenOPowers.Enabled = SAV.OPower > -1;
            B_OpenPokedex.Enabled = SAV.PokeDex > -1;
            B_OpenBerryField.Enabled = SAV.BerryField > -1;
            B_JPEG.Enabled = SAV.JPEG > -1;

            // Refresh PK#->PK6 conversion info
            Converter.updateConfig(SAV.SubRegion, SAV.Country, SAV.ConsoleRegion, SAV.OT, SAV.Gender);

            // Indicate audibly the save is loaded
            SystemSounds.Beep.Play();
        }

        // Language Translation
        private void changeMainLanguage(object sender, EventArgs e)
        {
            byte[] data = fieldsInitialized ? preparepkx() : pk6.Data;
            bool alreadyInit = fieldsInitialized;
            fieldsInitialized = false;
            Menu_Options.DropDown.Close();
            InitializeStrings();
            InitializeLanguage();
            Util.TranslateInterface(this, lang_val[CB_MainLanguage.SelectedIndex], menuStrip1); // Translate the UI to language.
            populateFields(data); // put data back in form
            fieldsInitialized |= alreadyInit;
        }
        private void InitializeStrings()
        {
            if (CB_MainLanguage.SelectedIndex < 8)
                curlanguage = lang_val[CB_MainLanguage.SelectedIndex];

            string l = curlanguage;
            natures = Util.getStringList("Natures", l);
            types = Util.getStringList("Types", l);
            abilitylist = Util.getStringList("Abilities", l);
            movelist = Util.getStringList("Moves", l);
            itemlist = Util.getStringList("Items", l);
            characteristics = Util.getStringList("Character", l);
            specieslist = Util.getStringList("Species", l);
            wallpapernames = Util.getStringList("Wallpaper", l);
            itempouch = Util.getStringList("ItemPouch", l);
            encountertypelist = Util.getStringList("EncounterType", l);
            gamelist = Util.getStringList("Games", l);
            gamelanguages = Util.getNulledStringArray(Util.getSimpleStringList("languages"));
            consoleregions = Util.getNulledStringArray(Util.getSimpleStringList("regions3ds"));

            balllist = new string[Legal.Items_Ball.Length];
            for (int i = 0; i < balllist.Length; i++)
                balllist[i] = itemlist[Legal.Items_Ball[i]];

            if ((l != "zh") || (l == "zh" && !fieldsInitialized)) // load initial binaries
            {
                forms = Util.getStringList("Forms", l);
                memories = Util.getStringList("Memories", l);
                genloc = Util.getStringList("GenLoc", l);
                trainingbags = Util.getStringList("TrainingBag", l);
                trainingstage = Util.getStringList("SuperTraining", l);
                puffs = Util.getStringList("Puff", l);
            }

            // Fix Item Names (Duplicate entries)
            itemlist[456] += " (OLD)"; // S.S. Ticket
            itemlist[463] += " (OLD)"; // Storage Key
            itemlist[478] += " (OLD)"; // Basement Key
            itemlist[626] += " (2)"; // Xtransceiver
            itemlist[629] += " (2)"; // DNA Splicers
            itemlist[637] += " (2)"; // Dropped Item
            itemlist[707] += " (2)"; // Travel Trunk
            itemlist[713] += " (2)"; // Alt Bike
            itemlist[714] += " (2)"; // Holo Caster
            itemlist[729] += " (1)"; // Meteorite
            itemlist[740] += " (2)"; // Contest Costume
            itemlist[751] += " (2)"; // Meteorite
            itemlist[771] += " (3)"; // Meteorite
            itemlist[772] += " (4)"; // Meteorite

            // Get the Egg Name and then replace it with --- for the comboboxes.
            eggname = specieslist[0];
            specieslist[0] = "---";

            // Get the met locations... for all of the games...
            metHGSS_00000 = Util.getStringList("hgss_00000", l);
            metHGSS_02000 = Util.getStringList("hgss_02000", l);
            metHGSS_03000 = Util.getStringList("hgss_03000", l);

            metBW2_00000 = Util.getStringList("bw2_00000", l);
            metBW2_30000 = Util.getStringList("bw2_30000", l);
            metBW2_40000 = Util.getStringList("bw2_40000", l);
            metBW2_60000 = Util.getStringList("bw2_60000", l);

            metXY_00000 = Util.getStringList("xy_00000", l);
            metXY_30000 = Util.getStringList("xy_30000", l);
            metXY_40000 = Util.getStringList("xy_40000", l);
            metXY_60000 = Util.getStringList("xy_60000", l);

            // Fix up some of the Location strings to make them more descriptive:
            metHGSS_02000[1] += " (NPC)";         // Anything from an NPC
            metHGSS_02000[2] += " (" + eggname + ")"; // Egg From Link Trade
            metBW2_00000[36] = metBW2_00000[84] + "/" + metBW2_00000[36]; // Cold Storage in BW = PWT in BW2

            // BW2 Entries from 76 to 105 are for Entralink in BW
            for (int i = 76; i < 106; i++)
                metBW2_00000[i] = metBW2_00000[i] + "●";

            // Localize the Poketransfer to the language (30001)
            string[] ptransp = { "Poké Transfer", "ポケシフター", "Poké Fret", "Pokétrasporto", "Poképorter", "Pokétransfer", "포케시프터", "ポケシフター" };
            metBW2_30000[1 - 1] = ptransp[Array.IndexOf(lang_val, curlanguage)];
            metBW2_30000[2 - 1] += " (NPC)";              // Anything from an NPC
            metBW2_30000[3 - 1] += " (" + eggname + ")";  // Egg From Link Trade

            // Zorua/Zoroark events
            metBW2_30000[10 - 1] = specieslist[251] + " (" + specieslist[570] + " 1)"; // Celebi's Zorua Event
            metBW2_30000[11 - 1] = specieslist[251] + " (" + specieslist[570] + " 2)"; // Celebi's Zorua Event
            metBW2_30000[12 - 1] = specieslist[571] + " (" + "1)"; // Zoroark
            metBW2_30000[13 - 1] = specieslist[571] + " (" + "2)"; // Zoroark

            metBW2_60000[3 - 1] += " (" + eggname + ")";  // Egg Treasure Hunter/Breeder, whatever...

            metXY_00000[104] += " (X/Y)";              // Victory Road
            metXY_00000[298] += " (OR/AS)";            // Victory Road
            metXY_30000[0] += " (NPC)";                // Anything from an NPC
            metXY_30000[1] += " (" + eggname + ")";    // Egg From Link Trade

            // Set the first entry of a met location to "" (nothing)
            // Fix (None) tags
            abilitylist[0] = itemlist[0] = movelist[0] = metXY_00000[0] = metBW2_00000[0] = metHGSS_00000[0] = "(" + itemlist[0] + ")";

            // Force an update to the met locations
            origintrack = "";

            if (fieldsInitialized)
                updateIVs(null, null); // Prompt an update for the characteristics
        }
        #endregion

        #region //// PKX WINDOW FUNCTIONS ////
        private void InitializeFields()
        {
            // Now that the ComboBoxes are ready, load the data.
            fieldsInitialized = true;
            pk6.Species = 493;
            pk6.Move1 = 1;
            pk6.RefreshChecksum();

            // Load Data
            populateFields(pk6.Data);
            {
                TB_OT.Text = "PKHeX";
                TB_TID.Text = 12345.ToString();
                TB_SID.Text = 54321.ToString();
                CB_GameOrigin.SelectedIndex = 0;
                CB_Language.SelectedIndex = 0;
                CB_BoxSelect.SelectedIndex = 0;
                CB_GameOrigin.SelectedIndex = 0;
                CB_PPu1.SelectedIndex = CB_PPu2.SelectedIndex = CB_PPu3.SelectedIndex = CB_PPu4.SelectedIndex = 0;
                CB_Ball.SelectedIndex = 0;
                CB_Country.SelectedIndex = 0;
                setAbilityList(TB_AbilityNumber, Util.getIndex(CB_Species), CB_Ability, CB_Form);
            }
        }
        private void InitializeLanguage()
        {
            // Set the Display
            CB_Country.DisplayMember = 
                CB_SubRegion.DisplayMember = 
                CB_3DSReg.DisplayMember =
                CB_Language.DisplayMember =
                CB_Ball.DisplayMember =
                CB_HeldItem.DisplayMember =
                CB_Species.DisplayMember =
                DEV_Ability.DisplayMember =
                CB_Nature.DisplayMember =
                CB_EncounterType.DisplayMember =
                CB_GameOrigin.DisplayMember =
                CB_HPType.DisplayMember = "Text";

            // Set the Value
            CB_Country.ValueMember = 
                CB_SubRegion.ValueMember = 
                CB_3DSReg.ValueMember =
                CB_Language.ValueMember =
                CB_Ball.ValueMember =
                CB_HeldItem.ValueMember =
                CB_Species.ValueMember =
                DEV_Ability.ValueMember =
                CB_Nature.ValueMember =
                CB_EncounterType.ValueMember =
                CB_GameOrigin.ValueMember = 
                CB_HPType.ValueMember = "Value";

            // Set the various ComboBox DataSources up with their allowed entries
            setCountrySubRegion(CB_Country, "countries");
            CB_3DSReg.DataSource = Util.getUnsortedCBList("regions3ds");
            CB_Language.DataSource = Util.getUnsortedCBList("languages");
            int[] ball_nums = { 7, 576, 13, 492, 497, 14, 495, 493, 496, 494, 11, 498, 8, 6, 12, 15, 9, 5, 499, 10, 1, 16 };
            int[] ball_vals = { 7, 25, 13, 17, 22, 14, 20, 18, 21, 19, 11, 23, 8, 6, 12, 15, 9, 5, 24, 10, 1, 16 };
            BallDataSource = Util.getVariedCBList(Util.getCBList(itemlist, new[] { 4 }, new[] { 3 }, new[] { 2 }, new[] { 1 }), itemlist, ball_nums, ball_vals);
            ItemDataSource = Util.getCBList(itemlist, DEV_Ability.Enabled ? null : Legal.Items_Held);
            SpeciesDataSource = Util.getCBList(specieslist, null);
            NatureDataSource = Util.getCBList(natures, null);
            AbilityDataSource = Util.getCBList(abilitylist, null);
            VersionDataSource = Util.getCBList(gamelist, Legal.Games_6oras, Legal.Games_6xy, Legal.Games_5, Legal.Games_4, Legal.Games_4e, Legal.Games_4r, Legal.Games_3, Legal.Games_3e, Legal.Games_3r, Legal.Games_3s);

            CB_Ball.DataSource = new BindingSource(BallDataSource, null);
            CB_Species.DataSource = new BindingSource(SpeciesDataSource, null);
            CB_HeldItem.DataSource = new BindingSource(ItemDataSource, null);
            CB_Nature.DataSource = new BindingSource(NatureDataSource, null);

            DEV_Ability.DataSource = new BindingSource(AbilityDataSource, null);
            CB_EncounterType.DataSource = Util.getCBList(encountertypelist, new[] { 0 }, Legal.Gen4EncounterTypes);
            CB_GameOrigin.DataSource = new BindingSource(VersionDataSource, null);
            CB_HPType.DataSource = Util.getCBList(types.Skip(1).Take(16).ToArray(), null);

            // Set the Move ComboBoxes too..
            {
                MoveDataSource = Util.getCBList(movelist, null);
                foreach (ComboBox cb in new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4, CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 })
                {
                    cb.DisplayMember = "Text"; cb.ValueMember = "Value";
                    cb.DataSource = new BindingSource(MoveDataSource, null);
                }
            }
        }
        public void populateFields(byte[] data, bool focus = true)
        {
            pk6 = new PK6(data);
            if (fieldsInitialized & !PKX.verifychk(pk6.Data))
                Util.Alert("PKX File has an invalid checksum.");

            // Reset a little.
            bool oldInit = fieldsInitialized;
            fieldsInitialized = false;
            CAL_EggDate.Value = new DateTime(2000, 01, 01);
            if (focus)
                Tab_Main.Focus();

            // Do first
            pk6.Stat_Level = PKX.getLevel(pk6.Species, pk6.EXP);
            if (pk6.Stat_Level == 100)
                pk6.EXP = PKX.getEXP(pk6.Stat_Level, pk6.Species);

            CB_Species.SelectedValue = pk6.Species;
            TB_Level.Text = pk6.Stat_Level.ToString();
            TB_EXP.Text = pk6.EXP.ToString();

            // Load rest
            TB_EC.Text = pk6.EncryptionConstant.ToString("X8");
            CHK_Fateful.Checked = pk6.FatefulEncounter;
            CHK_IsEgg.Checked = pk6.IsEgg;
            CHK_Nicknamed.Checked = pk6.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk6.OT_Gender];
            Label_OTGender.ForeColor = pk6.OT_Gender == 1 ? Color.Red : Color.Blue;
            CHK_Circle.Checked = pk6.Circle;
            CHK_Triangle.Checked = pk6.Triangle;
            CHK_Square.Checked = pk6.Square;
            CHK_Heart.Checked = pk6.Heart;
            CHK_Star.Checked = pk6.Star;
            CHK_Diamond.Checked = pk6.Diamond;
            TB_PID.Text = pk6.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk6.HeldItem;
            setAbilityList(TB_AbilityNumber, pk6.Species, CB_Ability, CB_Form);
            TB_AbilityNumber.Text = pk6.AbilityNumber.ToString();
            CB_Ability.SelectedIndex = pk6.AbilityNumber < 6 ? pk6.AbilityNumber >> 1 : 0; // with some simple error handling
            CB_Nature.SelectedValue = pk6.Nature;
            TB_TID.Text = pk6.TID.ToString("00000");
            TB_SID.Text = pk6.SID.ToString("00000");
            TB_Nickname.Text = pk6.Nickname;
            TB_OT.Text = pk6.OT_Name;
            TB_OTt2.Text = pk6.HT_Name;
            TB_Friendship.Text = pk6.CurrentFriendship.ToString();
            if (pk6.CurrentHandler == 1)  // HT
            {
                GB_nOT.BackgroundImage = mixedHighlight;
                GB_OT.BackgroundImage = null;
            }
            else                  // = 0
            {
                GB_OT.BackgroundImage = mixedHighlight;
                GB_nOT.BackgroundImage = null;
            }
            CB_Language.SelectedValue = pk6.Language;
            CB_Country.SelectedValue = pk6.Country;
            CB_SubRegion.SelectedValue = pk6.Region;
            CB_3DSReg.SelectedValue = pk6.ConsoleRegion;
            CB_GameOrigin.SelectedValue = pk6.Version;
            CB_EncounterType.SelectedValue = pk6.Gen4 ? pk6.EncounterType : 0;
            CB_Ball.SelectedValue = pk6.Ball;

            if (pk6.Met_Month == 0) { pk6.Met_Month = 1; }
            if (pk6.Met_Day == 0) { pk6.Met_Day = 1; }
            try { CAL_MetDate.Value = new DateTime(pk6.Met_Year + 2000, pk6.Met_Month, pk6.Met_Day); }
            catch { CAL_MetDate.Value = new DateTime(2000, 1, 1); }

            if (pk6.Egg_Location != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = pk6.Egg_Location;
                try { CAL_EggDate.Value = new DateTime(pk6.Egg_Year + 2000, pk6.Egg_Month, pk6.Egg_Day); }
                catch { CAL_MetDate.Value = new DateTime(2000, 1, 1); }
            }
            else { CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = pk6.Met_Location;

            // Set CT Gender to None if no CT, else set to gender symbol.
            Label_CTGender.Text = pk6.HT_Name == "" ? "" : gendersymbols[pk6.HT_Gender];
            Label_CTGender.ForeColor = pk6.HT_Gender == 1 ? Color.Red : Color.Blue;

            TB_MetLevel.Text = pk6.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk6.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk6.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk6.PKRS_Strain;
            CHK_Cured.Checked = pk6.PKRS_Strain > 0 && pk6.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk6.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = pk6.CNT_Cool.ToString();
            TB_Beauty.Text = pk6.CNT_Beauty.ToString();
            TB_Cute.Text = pk6.CNT_Cute.ToString();
            TB_Smart.Text = pk6.CNT_Smart.ToString();
            TB_Tough.Text = pk6.CNT_Tough.ToString();
            TB_Sheen.Text = pk6.CNT_Sheen.ToString();

            TB_HPIV.Text = pk6.IV_HP.ToString();
            TB_ATKIV.Text = pk6.IV_ATK.ToString();
            TB_DEFIV.Text = pk6.IV_DEF.ToString();
            TB_SPEIV.Text = pk6.IV_SPE.ToString();
            TB_SPAIV.Text = pk6.IV_SPA.ToString();
            TB_SPDIV.Text = pk6.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk6.HPType;

            TB_HPEV.Text = pk6.EV_HP.ToString();
            TB_ATKEV.Text = pk6.EV_ATK.ToString();
            TB_DEFEV.Text = pk6.EV_DEF.ToString();
            TB_SPEEV.Text = pk6.EV_SPE.ToString();
            TB_SPAEV.Text = pk6.EV_SPA.ToString();
            TB_SPDEV.Text = pk6.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk6.Move1;
            CB_Move2.SelectedValue = pk6.Move2;
            CB_Move3.SelectedValue = pk6.Move3;
            CB_Move4.SelectedValue = pk6.Move4;
            CB_RelearnMove1.SelectedValue = pk6.RelearnMove1;
            CB_RelearnMove2.SelectedValue = pk6.RelearnMove2;
            CB_RelearnMove3.SelectedValue = pk6.RelearnMove3;
            CB_RelearnMove4.SelectedValue = pk6.RelearnMove4;
            CB_PPu1.SelectedIndex = pk6.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk6.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk6.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk6.Move4_PPUps;
            TB_PP1.Text = pk6.Move1_PP.ToString();
            TB_PP2.Text = pk6.Move2_PP.ToString();
            TB_PP3.Text = pk6.Move3_PP.ToString();
            TB_PP4.Text = pk6.Move4_PP.ToString();
            
            // Set Form if count is enough, else if count is more than 1 set equal to max else zero.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk6.AltForm ? pk6.AltForm : (CB_Form.Items.Count > 1 ? CB_Form.Items.Count - 1 : 0);

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk6.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();
            setIsShiny();

            CB_EncounterType.Visible = Label_EncounterType.Visible = pk6.Gen4;

            fieldsInitialized = oldInit;
            updateIVs(null, null);
            updatePKRSInfected(null, null);
            updatePKRSCured(null, null);

            TB_EXP.Text = pk6.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk6.Gender];
            Label_Gender.ForeColor = pk6.Gender == 2 ? Label_Species.ForeColor : (pk6.Gender == 1 ? Color.Red : Color.Blue);
            if (HaX) // DEV Illegality
            {
                DEV_Ability.SelectedValue = pk6.Ability;
                MT_Level.Text = pk6.Stat_Level.ToString();
                MT_Form.Text = pk6.AltForm.ToString();
            }
            
            // Set the Preview Box
            dragout.Image = pk6.Sprite;

            // Highlight the Current Handler
            clickGT(pk6.CurrentHandler == 1 ? GB_nOT : GB_OT, null);
        }
        // General Use Functions shared by other Forms // 
        internal static void setCountrySubRegion(ComboBox CB, string type)
        {
            int index = CB.SelectedIndex;
            // fix for Korean / Chinese being swapped
            string cl = curlanguage + "";
            cl = cl == "zh" ? "ko" : cl == "ko" ? "zh" : cl;

            CB.DataSource = Util.getCBList(type, cl);

            if (index > 0 && index < CB.Items.Count && fieldsInitialized)
                CB.SelectedIndex = index;
        }
        internal static void setForms(int species, ComboBox cb, Label l = null)
        {
            // Form Tables
            cb.DisplayMember = "Text";
            cb.ValueMember = "Value";
            bool hasForms = PKX.Personal[species].HasFormes || new[] { 664, 665, 414, }.Contains(species);
            cb.Enabled = cb.Visible = hasForms;
            if (l != null) l.Visible = hasForms;
            
            cb.DataSource = PKX.getFormList(species, types, forms, gendersymbols).ToList();
        }
        internal static void setAbilityList(MaskedTextBox tb_abil, int species, ComboBox cb_abil, ComboBox cb_forme)
        {
            if (!fieldsInitialized && tb_abil.Text == "")
                return;
            int newabil = Convert.ToInt16(tb_abil.Text) >> 1;

            int form = cb_forme.SelectedIndex;
            byte[] abils = PKX.getAbilities(species, form);

            // Build Ability List
            List<string> ability_list = new List<string>
            {
                abilitylist[abils[0]] + " (1)",
                abilitylist[abils[1]] + " (2)",
                abilitylist[abils[2]] + " (H)"
            };
            cb_abil.DataSource = ability_list;

            cb_abil.SelectedIndex = newabil < 3 ? newabil : 0;
        }
        // PKX Data Calculation Functions //
        private void setIsShiny()
        {
            bool isShiny = PKX.getIsShiny(Util.getHEXval(TB_PID.Text), Util.ToUInt32(TB_TID.Text), Util.ToUInt32(TB_SID.Text));

            // Set the Controls
            BTN_Shinytize.Visible = BTN_Shinytize.Enabled = !isShiny;
            Label_IsShiny.Visible = isShiny;

            // Refresh Markings (for Shiny Star if applicable)
            setMarkings();
        }
        private void setMarkings()
        {
            PictureBox[] pba = { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            CheckBox[] cba = { CHK_Circle, CHK_Triangle, CHK_Square, CHK_Heart, CHK_Star, CHK_Diamond };
            for (int i = 0; i < 6; i++)
                pba[i].Image = Util.ChangeOpacity(pba[i].InitialImage, (cba[i].Checked ? 1 : 0) * 0.9 + 0.1);

            PB_MarkShiny.Image = Util.ChangeOpacity(PB_MarkShiny.InitialImage, (!BTN_Shinytize.Enabled ? 1 : 0) * 0.9 + 0.1);
            PB_MarkCured.Image = Util.ChangeOpacity(PB_MarkCured.InitialImage, (CHK_Cured.Checked ? 1 : 0) * 0.9 + 0.1);

            int Version = Util.getIndex(CB_GameOrigin); // 24,25 = XY, 26,27 = ORAS, 28,29 = ???
            PB_MarkPentagon.Image = Util.ChangeOpacity(PB_MarkPentagon.InitialImage, ((Version >= 24 && Version <= 29) ? 1 : 0) * 0.9 + 0.1);
        }
        // Clicked Label Shortcuts //
        private void clickQR(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Alt)
            {
                // Fetch data from QR code...
                byte[] ekx = Util.getQRData();

                if (ekx == null) return;

                if (ekx.Length != PK6.SIZE_STORED) { Util.Alert($"Decoded data not {PK6.SIZE_STORED} bytes.", $"QR Data Size: {ekx.Length}"); }
                else try
                    {
                        byte[] pkx = PKX.decryptArray(ekx);
                        if (PKX.verifychk(pkx)) { populateFields(pkx); }
                        else Util.Alert("Invalid checksum in QR data.");
                    }
                    catch { Util.Alert("Error loading decrypted data."); }
            }
            else
            {
                if (!verifiedPKX()) return;
                byte[] pkx = preparepkx();
                byte[] ekx = PKX.encryptArray(pkx);

                Array.Resize(ref ekx, PK6.SIZE_STORED);
                const string server = "http://loadcode.projectpokemon.org/b1s1.html#"; // Rehosted with permission from LC/MS -- massive thanks!
                Image qr = Util.getQRImage(ekx, server);

                if (qr == null) return;

                string[] r = new PK6(pkx, "Tabs").QRText;
                new QR(qr, dragout.Image, r[0], r[1], r[2], "PKHeX @ ProjectPokemon.org").ShowDialog();
            }
        }
        private void clickFriendship(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // prompt to reset
                TB_Friendship.Text = pk6.CurrentHandler == 0 ? pk6.OT_Friendship.ToString() : pk6.HT_Friendship.ToString();
            else
                TB_Friendship.Text = TB_Friendship.Text == "255" ? PKX.getBaseFriendship(pk6.Species).ToString() : "255";
        }
        private void clickGender(object sender, EventArgs e)
        {
            // Get Gender Threshold
            int gt = PKX.Personal[Util.getIndex(CB_Species)].Gender;

            if (gt == 255 || gt == 0 || gt == 254) // Single gender/genderless
                return;

            if (gt >= 255) return; 
            // If not a single gender(less) species: (should be <254 but whatever, 255 never happens)

            pk6.Gender = PKX.getGender(Label_Gender.Text) ^ 1;
            Label_Gender.Text = gendersymbols[pk6.Gender];
            Label_Gender.ForeColor = pk6.Gender == 2 ? Label_Species.ForeColor : ((pk6.Gender == 1) ? Color.Red : Color.Blue);

            if (PKX.getGender(CB_Form.Text) < 2) // Gendered Forms
                CB_Form.SelectedIndex = PKX.getGender(Label_Gender.Text);

            getQuickFiller(dragout);
        }
        private void clickPPUps(object sender, EventArgs e)
        {
            CB_PPu1.SelectedIndex = ModifierKeys != Keys.Control && Util.getIndex(CB_Move1) > 0 ? 3 : 0;
            CB_PPu2.SelectedIndex = ModifierKeys != Keys.Control && Util.getIndex(CB_Move2) > 0 ? 3 : 0;
            CB_PPu3.SelectedIndex = ModifierKeys != Keys.Control && Util.getIndex(CB_Move3) > 0 ? 3 : 0;
            CB_PPu4.SelectedIndex = ModifierKeys != Keys.Control && Util.getIndex(CB_Move4) > 0 ? 3 : 0;
        }
        private void clickMarking(object sender, EventArgs e)
        {
            PictureBox[] pba = { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            CheckBox[] cba = { CHK_Circle, CHK_Triangle, CHK_Square, CHK_Heart, CHK_Star, CHK_Diamond };

            CheckBox cb = cba[Array.IndexOf(pba, sender as PictureBox)];
            cb.Checked = !cb.Checked;
            setMarkings();
        }
        private void clickStatLabel(object sender, MouseEventArgs e)
        {
            if (!(ModifierKeys == Keys.Control || ModifierKeys == Keys.Alt))
                return;

            int index = Array.IndexOf(new[] { Label_HP, Label_ATK, Label_DEF, Label_SPA, Label_SPD, Label_SPE }, sender as Label);

            if (ModifierKeys == Keys.Alt) // EV
            {
                var arrayEV = new[] {TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPAEV, TB_SPDEV, TB_SPEEV};
                arrayEV[index].Text = (e.Button == MouseButtons.Left
                    ? Math.Min(Math.Max(510 - Util.ToInt32(TB_EVTotal.Text) + Util.ToInt32(arrayEV[index].Text), 0), 252) 
                    : 0).ToString();
            }
            else
            new[] {TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPAIV, TB_SPDIV, TB_SPEIV}[index].Text =
                ((e.Button == MouseButtons.Left) ? 31 : 0).ToString();
        }
        private void clickIV(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
                (sender as MaskedTextBox).Text = 31.ToString();
            else if (ModifierKeys == Keys.Alt)
                (sender as MaskedTextBox).Text = 0.ToString();
        }
        private void clickEV(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // EV
                (sender as MaskedTextBox).Text = Math.Min(Math.Max(510 - Util.ToInt32(TB_EVTotal.Text) + Util.ToInt32((sender as MaskedTextBox).Text), 0), 252).ToString();
            else if (ModifierKeys == Keys.Alt)
                (sender as MaskedTextBox).Text = 0.ToString();
        }
        private void clickOT(object sender, EventArgs e)
        {
            string OT = SAV.OT;
            if (OT.Length <= 0) return;

            TB_OT.Text = OT;
            // Set Gender Label
            Label_OTGender.Text = SAV.Gender == 1 ? gendersymbols[1] : gendersymbols[0];

            // Get Save Information
            TB_TID.Text = SAV.TID.ToString();
            TB_SID.Text = SAV.SID.ToString();
            CB_GameOrigin.SelectedValue = SAV.Game;
            CB_SubRegion.SelectedValue = SAV.SubRegion;
            CB_Country.SelectedValue = SAV.Country;
            CB_3DSReg.SelectedValue = SAV.ConsoleRegion;
            CB_Language.SelectedValue = SAV.Language;
            updateNickname(null, null);
        }
        private void clickCT(object sender, EventArgs e)
        {
            if (TB_OTt2.Text.Length > 0)
                Label_CTGender.Text = gendersymbols[SAV.Gender];
        }
        private void clickGT(object sender, EventArgs e)
        {
            if (sender as GroupBox == GB_OT)
            {
                pk6.CurrentHandler = 0;
                TB_Friendship.Text = pk6.OT_Friendship.ToString();
                GB_OT.BackgroundImage = mixedHighlight;
                GB_nOT.BackgroundImage = null;
            }
            else if (TB_OTt2.Text.Length > 0)
            {
                pk6.CurrentHandler = 1;
                TB_Friendship.Text = pk6.HT_Friendship.ToString();
                GB_OT.BackgroundImage = null;
                GB_nOT.BackgroundImage = mixedHighlight;
            }
        }
        private void clickTRGender(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl.Text != "") // set gender label (toggle M/F)
            {
                int gender = PKX.getGender(lbl.Text) ^ 1;
                lbl.Text = gendersymbols[gender];
                lbl.ForeColor = gender == 1 ? Color.Red : Color.Blue;
            }
        }
        private void clickMoves(object sender, EventArgs e)
        {
            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Copy current moves to Relearn Moves?"))
                return;

            CB_RelearnMove1.SelectedIndex = CB_Move1.SelectedIndex > 0 ? CB_Move1.SelectedIndex : 0;
            CB_RelearnMove2.SelectedIndex = CB_Move2.SelectedIndex > 0 ? CB_Move2.SelectedIndex : 0;
            CB_RelearnMove3.SelectedIndex = CB_Move3.SelectedIndex > 0 ? CB_Move3.SelectedIndex : 0;
            CB_RelearnMove4.SelectedIndex = CB_Move4.SelectedIndex > 0 ? CB_Move4.SelectedIndex : 0;
        }
        // Prompted Updates of PKX Functions // 
        private bool changingFields;
        private void updateEXPLevel(object sender, EventArgs e)
        {
            if (changingFields) return;

            if (sender as MaskedTextBox == TB_EXP)
            {
                changingFields = true;
                // Change the Level
                uint EXP = Util.ToUInt32(TB_EXP.Text);
                int Species = Util.getIndex(CB_Species);
                int Level = EXP == 0 ? 1 : PKX.getLevel(Species, EXP);
                if (Level == 100)
                    EXP = PKX.getEXP(Level, Species);

                TB_Level.Text = Level.ToString();
                if (!MT_Level.Visible)
                    TB_EXP.Text = EXP.ToString();
                else
                    MT_Level.Text = Level.ToString();
            }
            else
            {
                changingFields = true;
                // Change the XP
                int Level = Util.ToInt32((MT_Level.Focused ? MT_Level : TB_Level).Text);
                if (Level > 100) TB_Level.Text = "100";
                if (Level > byte.MaxValue) MT_Level.Text = "255";

                TB_EXP.Text = PKX.getEXP(Level, Util.getIndex(CB_Species)).ToString();
            }
            changingFields = false;
            updateStats();
        }
        private void updateHPType(object sender, EventArgs e)
        {
            if (changingFields || !fieldsInitialized) return;
            changingFields = true;
            int[] ivs =
            {
                Util.ToInt32(TB_HPIV.Text), Util.ToInt32(TB_ATKIV.Text), Util.ToInt32(TB_DEFIV.Text), 
                Util.ToInt32(TB_SPAIV.Text), Util.ToInt32(TB_SPDIV.Text), Util.ToInt32(TB_SPEIV.Text) 
            };

            // Change IVs to match the new Hidden Power
            int[] newIVs = PKX.setHPIVs(Util.getIndex(CB_HPType), ivs);
            TB_HPIV.Text = newIVs[0].ToString();
            TB_ATKIV.Text = newIVs[1].ToString();
            TB_DEFIV.Text = newIVs[2].ToString();

            TB_SPAIV.Text = newIVs[3].ToString();
            TB_SPDIV.Text = newIVs[4].ToString();
            TB_SPEIV.Text = newIVs[5].ToString();

            // Refresh View
            changingFields = false;
            updateIVs(null, null);
        }
        private void updateIVs(object sender, EventArgs e)
        {
            if (changingFields || !fieldsInitialized) return;
            if (sender != null)
                if (Util.ToInt32((sender as MaskedTextBox).Text) > 31)
                    (sender as MaskedTextBox).Text = "31";

            changingFields = true;

            // Update IVs
            pk6.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk6.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk6.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk6.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk6.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk6.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
                    
            CB_HPType.SelectedValue = pk6.HPType;
            changingFields = false;

            // Potential Reading
            L_Potential.Text = (!unicode
                ? new[] {"★☆☆☆", "★★☆☆", "★★★☆", "★★★★"}
                : new[] {"+", "++", "+++", "++++"}
                )[pk6.PotentialRating];

            TB_IVTotal.Text = pk6.IVs.Sum().ToString();

            L_Characteristic.Text = characteristics[pk6.Characteristic];
            updateStats();
        }
        private void updateEVs(object sender, EventArgs e)
        {
            if (sender != null)
                if (Util.ToInt32((sender as MaskedTextBox).Text) > 252)
                    (sender as MaskedTextBox).Text = "252";

            changingFields = true;
            int EV_HP = Util.ToInt32(TB_HPEV.Text);
            int EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            int EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            int EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            int EV_SPD = Util.ToInt32(TB_SPDEV.Text);
            int EV_SPE = Util.ToInt32(TB_SPEEV.Text);

            int evtotal = EV_HP + EV_ATK + EV_DEF + EV_SPA + EV_SPD + EV_SPE;

            if (evtotal > 510) // Background turns Red
                TB_EVTotal.BackColor = Color.Red;
            else if (evtotal == 510) // Maximum EVs
                TB_EVTotal.BackColor = Color.Honeydew;
            else if (evtotal == 508) // Fishy EVs
                TB_EVTotal.BackColor = Color.LightYellow;
            else TB_EVTotal.BackColor = Color.WhiteSmoke;

            TB_EVTotal.Text = evtotal.ToString();
            changingFields = false;
            updateStats();
        }
        private void updateRandomIVs(object sender, EventArgs e)
        {
            changingFields = true;
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift)
            {
                // Max IVs
                TB_HPIV.Text = 31.ToString();
                TB_ATKIV.Text = 31.ToString();
                TB_DEFIV.Text = 31.ToString();
                TB_SPAIV.Text = 31.ToString();
                TB_SPDIV.Text = 31.ToString();
                TB_SPEIV.Text = 31.ToString();
            }
            else
            {
                TB_HPIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_ATKIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_DEFIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_SPAIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_SPDIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_SPEIV.Text = (Util.rnd32() & 0x1F).ToString();
            }
            changingFields = false;
            updateStats();
        }
        private void updateRandomEVs(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift)
            {
                // Max IVs
                TB_HPEV.Text = 0.ToString();
                TB_ATKEV.Text = 0.ToString();
                TB_DEFEV.Text = 0.ToString();
                TB_SPAEV.Text = 0.ToString();
                TB_SPDEV.Text = 0.ToString();
                TB_SPEEV.Text = 0.ToString();
            }
            else
            {
                byte[] evs = PKX.getRandomEVs();
                TB_HPEV.Text = evs[0].ToString();
                TB_ATKEV.Text = evs[1].ToString();
                TB_DEFEV.Text = evs[2].ToString();
                TB_SPAEV.Text = evs[3].ToString();
                TB_SPDEV.Text = evs[4].ToString();
                TB_SPEEV.Text = evs[5].ToString();
            }
        }
        private void updateRandomPID(object sender, EventArgs e)
        {
            TB_PID.Text = PKX.getRandomPID(Util.getIndex(CB_Species), PKX.getGender(Label_Gender.Text)).ToString("X8");
            getQuickFiller(dragout);
        }
        private void updateRandomEC(object sender, EventArgs e)
        {
            TB_EC.Text = Util.rnd32().ToString("X8");
        }
        private void updateHackedStats(object sender, EventArgs e)
        {
            Stat_HP.Enabled =
                Stat_ATK.Enabled =
                Stat_DEF.Enabled =
                Stat_SPA.Enabled =
                Stat_SPD.Enabled =
                Stat_SPE.Enabled = CHK_HackedStats.Checked;
        }
        private void updateHackedStatText(object sender, EventArgs e)
        {
            if (!CHK_HackedStats.Checked || !(sender is TextBox))
                return;

            string text = ((TextBox)sender).Text;
            if (string.IsNullOrEmpty(text))
                ((TextBox)sender).Text = "0";

            if (Convert.ToUInt32(text) > ushort.MaxValue)
                ((TextBox)sender).Text = "65535";
        }
        private void update255_MTB(object sender, EventArgs e)
        {
            MaskedTextBox mtb = sender as MaskedTextBox;
            try
            {
                if (Util.ToInt32((sender as MaskedTextBox).Text) > byte.MaxValue)
                    (sender as MaskedTextBox).Text = "255";
            }
            catch { mtb.Text = "0"; }
        }
        private void update255_TB(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                if (Util.ToInt32((sender as TextBox).Text) > byte.MaxValue)
                    (sender as TextBox).Text = "255";
            }
            catch { tb.Text = "0"; }
        }
        private void updateForm(object sender, EventArgs e)
        {
            updateStats();
            // Repopulate Abilities if Species Form has different abilities
            setAbilityList(TB_AbilityNumber, Util.getIndex(CB_Species), CB_Ability, CB_Form);

            // Gender Forms
            if (PKX.getGender(CB_Form.Text) < 2 && Util.getIndex(CB_Species) != 201) // don't do this for Unown
                Label_Gender.Text = CB_Form.Text;

            if (changingFields) 
                return;
            changingFields = true;
            MT_Form.Text = Util.getIndex(CB_Form).ToString();
            changingFields = false;
        }
        private void updateHaXForm(object sender, EventArgs e)
        {
            if (changingFields)
                return;
            changingFields = true;
            int form = Util.ToInt32(MT_Form.Text);
            CB_Form.SelectedIndex = CB_Form.Items.Count > form ? form : -1;
            changingFields = false;
        }
        private void updatePP(object sender, EventArgs e)
        {
            TB_PP1.Text = PKX.getMovePP(Util.getIndex(CB_Move1), CB_PPu1.SelectedIndex).ToString();
            TB_PP2.Text = PKX.getMovePP(Util.getIndex(CB_Move2), CB_PPu2.SelectedIndex).ToString();
            TB_PP3.Text = PKX.getMovePP(Util.getIndex(CB_Move3), CB_PPu3.SelectedIndex).ToString();
            TB_PP4.Text = PKX.getMovePP(Util.getIndex(CB_Move4), CB_PPu4.SelectedIndex).ToString();
        }
        private void updatePKRSstrain(object sender, EventArgs e)
        {
            // Change the PKRS Days to the legal bounds.
            int currentDuration = CB_PKRSDays.SelectedIndex;
            CB_PKRSDays.Items.Clear();
            foreach (int day in Enumerable.Range(0, CB_PKRSStrain.SelectedIndex % 4 + 2)) CB_PKRSDays.Items.Add(day);

            // Set the days back if they're legal, else set it to 1. (0 always passes).
            CB_PKRSDays.SelectedIndex = currentDuration < CB_PKRSDays.Items.Count ? currentDuration : 1;

            if (CB_PKRSStrain.SelectedIndex != 0) return;
            
            // Never Infected
            CB_PKRSDays.SelectedIndex = 0;
            CHK_Cured.Checked = false;
            CHK_Infected.Checked = false;
        }
        private void updatePKRSdays(object sender, EventArgs e)
        {
            if (CB_PKRSDays.SelectedIndex != 0) return;

            // If no days are selected
            if (CB_PKRSStrain.SelectedIndex == 0)
            {
                // Never Infected
                CHK_Cured.Checked = false;
                CHK_Infected.Checked = false;
            }
            else CHK_Cured.Checked = true;
        }
        private void updatePKRSCured(object sender, EventArgs e)
        {
            if (!fieldsInitialized) return;
            // Cured PokeRus is toggled
            if (CHK_Cured.Checked)
            {
                // Has Had PokeRus
                Label_PKRSdays.Visible = CB_PKRSDays.Visible = false;
                CB_PKRSDays.SelectedIndex = 0;

                Label_PKRS.Visible = CB_PKRSStrain.Visible = true;
                CHK_Infected.Checked = true;

                // If we're cured we have to have a strain infection.
                if (CB_PKRSStrain.SelectedIndex == 0)
                    CB_PKRSStrain.SelectedIndex = 1;
            }
            else if (!CHK_Infected.Checked)
            {
                // Not Infected, Disable the other
                Label_PKRS.Visible = CB_PKRSStrain.Visible = false;
                CB_PKRSStrain.SelectedIndex = 0;
            }
            else
            {
                // Still Infected for a duration
                Label_PKRSdays.Visible = CB_PKRSDays.Visible = true;
                CB_PKRSDays.SelectedValue = 1;
            }
            // if not cured yet, days > 0
            if (!CHK_Cured.Checked && CHK_Infected.Checked && CB_PKRSDays.SelectedIndex == 0)
                CB_PKRSDays.SelectedIndex++;

            setMarkings();
        }
        private void updatePKRSInfected(object sender, EventArgs e)
        {
            if (!fieldsInitialized) return;
            if (CHK_Cured.Checked && !CHK_Infected.Checked) { CHK_Cured.Checked = false; return; }
            if (CHK_Cured.Checked) return;
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked;
            if (!CHK_Infected.Checked) { CB_PKRSStrain.SelectedIndex = 0; CB_PKRSDays.SelectedIndex = 0; Label_PKRSdays.Visible = CB_PKRSDays.Visible = false; }
            else if (CB_PKRSStrain.SelectedIndex == 0) { CB_PKRSStrain.SelectedIndex = 1; Label_PKRSdays.Visible = CB_PKRSDays.Visible = true; updatePKRSCured(sender, e);}

            // if not cured yet, days > 0
            if (CHK_Infected.Checked && CB_PKRSDays.SelectedIndex == 0) CB_PKRSDays.SelectedIndex++;
        }
        private void updateCountry(object sender, EventArgs e)
        {
            if (Util.getIndex(sender as ComboBox) > 0)
                setCountrySubRegion(CB_SubRegion, "sr_" + Util.getIndex(sender as ComboBox).ToString("000"));
        }
        private void updateSpecies(object sender, EventArgs e)
        {
            // Change Species Prompted
            int Species = Util.getIndex(CB_Species);
            int Level = Util.ToInt32(TB_Level.Text);
            if (MT_Level.Visible) Level = Util.ToInt32(MT_Level.Text);

            // Get Forms for Given Species
            setForms(Species, CB_Form, Label_Form);

            // Recalculate EXP for Given Level
            uint EXP = PKX.getEXP(Level, Species);
            TB_EXP.Text = EXP.ToString();

            // Check for Gender Changes
            // Get Gender Threshold
            int gt = PKX.Personal[Species].Gender;
            int genderflag;

            if (gt == 255)      // Genderless
                genderflag = 2;
            else if (gt == 254) // Female Only
                genderflag = 1;
            else if (gt == 0) // Male Only
                genderflag = 0;
            else // get gender from old PID correlation
                genderflag = (Util.getHEXval(TB_PID.Text) & 0xFF) <= gt ? 1 : 0;

            int Gender = genderflag;
            Label_Gender.Text = gendersymbols[Gender];
            Label_Gender.ForeColor = Gender == 2 ? Label_Species.ForeColor : (Gender == 1 ? Color.Red : Color.Blue);
            setAbilityList(TB_AbilityNumber, Species, CB_Ability, CB_Form);
            updateForm(null, null);

            // If species changes and no nickname, set the new name == speciesName.
            if (!CHK_Nicknamed.Checked)
                updateNickname(sender, e);
        }
        private void updateOriginGame(object sender, EventArgs e)
        {
            int Version = Util.getIndex(CB_GameOrigin);

            if (Version < 24 && origintrack != "Past")
            {
                // Load Past Gen Locations
                #region B2W2 Met Locations
                {
                    // Build up our met list
                    var met_list = Util.getCBList(metBW2_00000, new[] { 0 });
                    met_list = Util.getOffsetCBList(met_list, metBW2_60000, 60001, new[] { 60002 });
                    met_list = Util.getOffsetCBList(met_list, metBW2_30000, 30001, new[] { 30003 });
                    met_list = Util.getOffsetCBList(met_list, metBW2_00000, 00000, Legal.Met_BW2_0);
                    met_list = Util.getOffsetCBList(met_list, metBW2_30000, 30001, Legal.Met_BW2_3);
                    met_list = Util.getOffsetCBList(met_list, metBW2_40000, 40001, Legal.Met_BW2_4);
                    met_list = Util.getOffsetCBList(met_list, metBW2_60000, 60001, Legal.Met_BW2_6);
                    CB_MetLocation.DisplayMember = "Text";
                    CB_MetLocation.ValueMember = "Value";
                    CB_MetLocation.DataSource = met_list;
                    CB_EggLocation.DisplayMember = "Text";
                    CB_EggLocation.ValueMember = "Value";
                    CB_EggLocation.DataSource = new BindingSource(met_list, null);
                    CB_EggLocation.SelectedValue = 0;
                    CB_MetLocation.SelectedValue = Version < 20 ? 30001 : 60001;
                    origintrack = "Past";
                }
                #endregion
            }
            else if (Version > 23 && (origintrack != "XY"))
            {
                // Load X/Y/OR/AS locations
                #region ORAS Met Locations
                {
                    // Build up our met list
                    var met_list = Util.getCBList(metXY_00000, new[] { 0 });
                    met_list = Util.getOffsetCBList(met_list, metXY_60000, 60001, new[] { 60002 });
                    met_list = Util.getOffsetCBList(met_list, metXY_30000, 30001, new[] { 30002 });
                    met_list = Util.getOffsetCBList(met_list, metXY_00000, 00000, Legal.Met_XY_0);
                    met_list = Util.getOffsetCBList(met_list, metXY_30000, 30001, Legal.Met_XY_3);
                    met_list = Util.getOffsetCBList(met_list, metXY_40000, 40001, Legal.Met_XY_4);
                    met_list = Util.getOffsetCBList(met_list, metXY_60000, 60001, Legal.Met_XY_6);
                    CB_MetLocation.DisplayMember = "Text";
                    CB_MetLocation.ValueMember = "Value";
                    CB_MetLocation.DataSource = met_list;
                    CB_EggLocation.DisplayMember = "Text";
                    CB_EggLocation.ValueMember = "Value";
                    CB_EggLocation.DataSource = new BindingSource(met_list, null);
                    CB_EggLocation.SelectedValue = 0;
                    CB_MetLocation.SelectedValue = 0;
                    origintrack = "XY";
                }
                #endregion
            }
            if (Version < 0x10 && origintrack != "Gen4")
            {
                // Load Gen 4 egg locations if Gen 4 Origin.
                #region HGSS Met Locations
                var met_list = Util.getCBList(metHGSS_00000, new[] { 0 });
                met_list = Util.getOffsetCBList(met_list, metHGSS_02000, 2000, new[] { 2000 });
                met_list = Util.getOffsetCBList(met_list, metHGSS_02000, 2000, new[] { 2002 });
                met_list = Util.getOffsetCBList(met_list, metHGSS_03000, 3000, new[] { 3001 });
                met_list = Util.getOffsetCBList(met_list, metHGSS_00000, 0000, Legal.Met_HGSS_0);
                met_list = Util.getOffsetCBList(met_list, metHGSS_02000, 2000, Legal.Met_HGSS_2);
                met_list = Util.getOffsetCBList(met_list, metHGSS_03000, 3000, Legal.Met_HGSS_3);

                CB_EggLocation.DisplayMember = "Text";
                CB_EggLocation.ValueMember = "Value";
                CB_EggLocation.DataSource = met_list;
                CB_EggLocation.SelectedValue = 0;
                origintrack = "Gen4";
                #endregion
            }

            // Visibility logic for Gen 4 encounter type; only show for Gen 4 Pokemon.
            CB_EncounterType.Visible = Label_EncounterType.Visible = !(Version > 12 || Version < 7);
            // If not Gen 4, set Encounter Type to 0 after it set !visible.
            if (Version > 12 || Version < 7)
                CB_EncounterType.SelectedValue = 0;

            setMarkings(); // Set/Remove KB marking
        }
        private void updateExtraByteValue(object sender, EventArgs e)
        {
            // Changed Extra Byte's Value
            if (Util.ToInt32((sender as MaskedTextBox).Text) > byte.MaxValue)
                (sender as MaskedTextBox).Text = "255";

            int value = Util.ToInt32(TB_ExtraByte.Text);
            int offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
            pk6.Data[offset] = (byte)value;
        }
        private void updateExtraByteIndex(object sender, EventArgs e)
        {
            // Byte changed, need to refresh the Text box for the byte's value.
            TB_ExtraByte.Text = pk6.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
        }
        private void updateNatureModification(object sender, EventArgs e)
        {
            if (sender is ComboBox && (sender as ComboBox).Name != CB_Nature.Name) return;
            int nature = Util.getIndex(CB_Nature);
            int incr = nature / 5;
            int decr = nature % 5;

            Label[] labarray = { Label_ATK, Label_DEF, Label_SPE, Label_SPA, Label_SPD };
            // Reset Label Colors
            foreach (Label label in labarray)
                label.ForeColor = defaultControlText;

            // Set Colored StatLabels only if Nature isn't Neutral
            NatureTip.SetToolTip(CB_Nature,
                incr != decr
                    ? $"+{labarray[incr].Text} / -{labarray[decr].Text}".Replace(":", "")
                    : "-/-");
        }
        private void updateNickname(object sender, EventArgs e)
        {
            if (fieldsInitialized && ModifierKeys == Keys.Control && sender != null) // Import Showdown
            {
                clickShowdownImportPK6(sender, e);
                return;
            }
            if (fieldsInitialized && ModifierKeys == Keys.Alt && sender != null) // Export Showdown
            {
                clickShowdownExportPK6(sender, e);
                return; 
            }
            if (!fieldsInitialized || (CHK_Nicknamed.Checked)) return;

            // Fetch Current Species and set it as Nickname Text
            int species = Util.getIndex(CB_Species);
            if (species == 0 || species > 721)
                TB_Nickname.Text = "";
            else
            {
                // get language
                int lang = Util.getIndex(CB_Language);
                if (CHK_IsEgg.Checked) species = 0; // Set species to 0 to get the egg name.
                TB_Nickname.Text = PKX.getSpeciesName(CHK_IsEgg.Checked ? 0 : species, lang);
            }
        }
        private void updateNicknameClick(object sender, MouseEventArgs e)
        {
            TextBox tb = !(sender is TextBox) ? TB_Nickname : sender as TextBox;
            // Special Character Form
            if (ModifierKeys == Keys.Control && !specialChars)
                new f2_Text(tb).Show();
        }
        private void updateNotOT(object sender, EventArgs e)
        {
            if (TB_OTt2.Text == "")
            {
                clickGT(GB_OT, null); // Switch CT over to OT.
                Label_CTGender.Text = "";
                TB_Friendship.Text = pk6.OT_Friendship.ToString();
            }
            else if (Label_CTGender.Text == "")
                Label_CTGender.Text = gendersymbols[0];
        }
        private void updateIsEgg(object sender, EventArgs e)
        {
            if (CHK_IsEgg.Checked)
            {
                CHK_Nicknamed.Checked = false;
                TB_Friendship.Text = "1";

                // If we are an egg, it won't have a met location.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CAL_MetDate.Value = new DateTime(2000, 01, 01);
                CB_MetLocation.SelectedIndex = 2;
            }
            else // Not Egg
            {
                if (!CHK_Nicknamed.Checked)
                    updateNickname(null, null);

                TB_Friendship.Text = PKX.getBaseFriendship(Util.getIndex(CB_Species)).ToString();

                if (CB_EggLocation.SelectedIndex == 0)
                {
                    CAL_EggDate.Value = new DateTime(2000, 01, 01);
                    CHK_AsEgg.Checked = false;
                    GB_EggConditions.Enabled = false;
                }
            }
            // Display hatch counter if it is an egg, Display Friendship if it is not.
            Label_HatchCounter.Visible = CHK_IsEgg.Checked;
            Label_Friendship.Visible = !CHK_IsEgg.Checked;

            // Update image to (not) show egg.
            if (!fieldsInitialized) return;
            updateNickname(null, null);
            getQuickFiller(dragout);
        }
        private void updateMetAsEgg(object sender, EventArgs e)
        {
            GB_EggConditions.Enabled = CHK_AsEgg.Checked;
            if (CHK_AsEgg.Checked) return;
            // Remove egg met data
            CHK_IsEgg.Checked = false;
            CAL_EggDate.Value = new DateTime(2000, 01, 01);
            CB_EggLocation.SelectedValue = 0;
        }
        private void updateShinyPID(object sender, EventArgs e)
        {
            uint PID = Util.getHEXval(TB_PID.Text);
            uint UID = PID >> 16;
            uint LID = PID & 0xFFFF;
            uint PSV = UID ^ LID;
            uint TID = Util.ToUInt32(TB_TID.Text);
            uint SID = Util.ToUInt32(TB_SID.Text);
            uint TSV = TID ^ SID;
            uint XOR = TSV ^ PSV;

            // Preserve Gen5 Origin Ability bit just in case
            XOR &= 0xFFFE; XOR |= UID & 1;

            // New XOR should be 0 or 1.
            if (XOR > 16)
                TB_PID.Text = (((UID ^ XOR) << 16) + LID).ToString("X8");

            setIsShiny();
            getQuickFiller(dragout);
        }
        private void updateTSV(object sender, EventArgs e)
        {
            ushort TID = (ushort)Util.ToUInt32(TB_TID.Text);
            ushort SID = (ushort)Util.ToUInt32(TB_SID.Text);
            var TSV = PKX.getTSV(TID, SID);
            Tip1.SetToolTip(TB_TID, "TSV: " + TSV.ToString("0000"));
            Tip2.SetToolTip(TB_SID, "TSV: " + TSV.ToString("0000"));

            var PSV = PKX.getPSV(Util.getHEXval(TB_PID.Text));
            Tip3.SetToolTip(TB_PID, "PSV: " + PSV.ToString("0000"));
        }
        private void update_ID(object sender, EventArgs e)
        {
            // Trim out nonhex characters
            TB_PID.Text = Util.getHEXval(TB_PID.Text).ToString("X8");
            TB_EC.Text = Util.getHEXval(TB_EC.Text).ToString("X8");

            // Max TID/SID is 65535
            if (Util.ToUInt32(TB_TID.Text) > ushort.MaxValue) TB_TID.Text = "65535";
            if (Util.ToUInt32(TB_SID.Text) > ushort.MaxValue) TB_SID.Text = "65535";

            setIsShiny();
            getQuickFiller(dragout);
            updateIVs(null, null);   // If the PID is changed, PID%6 (Characteristic) might be changed. 
            TB_PID.Select(60, 0);   // position cursor at end of field
        }
        private void validateComboBox(object sender, EventArgs e)
        {
            if (!(sender is ComboBox)) { return; }

            ComboBox cb = sender as ComboBox;
            cb.SelectionLength = 0;

            cb.BackColor = cb.SelectedValue == null ? Color.DarkSalmon : defaultControlWhite;

            if (fieldsInitialized)
            { getQuickFiller(dragout); }
        }
        private void validateComboBox2(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            validateComboBox(sender, e);
            if (cb == CB_Ability)
                TB_AbilityNumber.Text = (1 << CB_Ability.SelectedIndex).ToString();
            else if (cb == CB_Move1 || cb == CB_Move2 || cb == CB_Move3 || cb == CB_Move4)
                updatePP(sender, e);
            updateNatureModification(sender, null);
            updateIVs(null, null); // updating Nature will trigger stats to update as well
        }
        private void removedropCB(object sender, KeyEventArgs e)
        {
            ((ComboBox)sender).DroppedDown = false;
        }
        private void updateStats()
        {
            // Gather the needed information.
            int species = Util.getIndex(CB_Species);
            int level = Util.ToInt32((MT_Level.Enabled) ? MT_Level.Text : TB_Level.Text);
            if (level == 0) { level = 1; }
            int form = CB_Form.SelectedIndex;
            int HP_IV = Util.ToInt32(TB_HPIV.Text);
            int ATK_IV = Util.ToInt32(TB_ATKIV.Text);
            int DEF_IV = Util.ToInt32(TB_DEFIV.Text);
            int SPA_IV = Util.ToInt32(TB_SPAIV.Text);
            int SPD_IV = Util.ToInt32(TB_SPDIV.Text);
            int SPE_IV = Util.ToInt32(TB_SPEIV.Text);

            int HP_EV = Util.ToInt32(TB_HPEV.Text);
            int ATK_EV = Util.ToInt32(TB_ATKEV.Text);
            int DEF_EV = Util.ToInt32(TB_DEFEV.Text);
            int SPA_EV = Util.ToInt32(TB_SPAEV.Text);
            int SPD_EV = Util.ToInt32(TB_SPDEV.Text);
            int SPE_EV = Util.ToInt32(TB_SPEEV.Text);

            int nature = Util.getIndex(CB_Nature);

            // Generate the stats.
            ushort[] stats = PKX.getStats(species, level, nature, form,
                                        HP_EV, ATK_EV, DEF_EV, SPA_EV, SPD_EV, SPE_EV,
                                        HP_IV, ATK_IV, DEF_IV, SPA_IV, SPD_IV, SPE_IV);

            Stat_HP.Text = stats[0].ToString();
            Stat_ATK.Text = stats[1].ToString();
            Stat_DEF.Text = stats[2].ToString();
            Stat_SPA.Text = stats[4].ToString();
            Stat_SPD.Text = stats[5].ToString();
            Stat_SPE.Text = stats[3].ToString();

            // Recolor the Stat Labels based on boosted stats.
            {
                int incr = nature / 5;
                int decr = nature % 5;

                Label[] labarray = { Label_ATK, Label_DEF, Label_SPE, Label_SPA, Label_SPD };
                // Reset Label Colors
                foreach (Label label in labarray)
                    label.ForeColor = defaultControlText;

                // Set Colored StatLabels only if Nature isn't Neutral
                if (incr == decr) return;
                labarray[incr].ForeColor = Color.Red;
                labarray[decr].ForeColor = Color.Blue;
            }
        }
        // Secondary Windows for Ribbons/Amie/Memories
        private void openRibbons(object sender, EventArgs e)
        {
            new RibbMedal().ShowDialog();
        }
        private void openHistory(object sender, EventArgs e)
        {
            // Write back current values
            pk6.Version = Util.getIndex(CB_GameOrigin);
            pk6.HT_Name = TB_OTt2.Text;
            pk6.OT_Name = TB_OT.Text;
            pk6.IsEgg = CHK_IsEgg.Checked;
            if (pk6.CurrentHandler == 0)
                pk6.OT_Friendship = Util.ToInt32(TB_Friendship.Text);
            else          // 1
                pk6.HT_Friendship = Util.ToInt32(TB_Friendship.Text);
            new MemoryAmie().ShowDialog();
            TB_Friendship.Text = (pk6.CurrentHandler == 0 ? pk6.OT_Friendship : pk6.HT_Friendship).ToString();
        }
        // Open/Save Array Manipulation //
        public bool verifiedPKX()
        {
            if (ModifierKeys == (Keys.Control | Keys.Shift | Keys.Alt))
                return true; // Override
            // Make sure the PKX Fields are filled out properly (color check)
            #region ComboBoxes to verify they are set.
            ComboBox[] cba = {
                                 CB_Species, CB_Nature, CB_HeldItem, CB_Ability, // Main Tab
                                 CB_MetLocation, CB_EggLocation, CB_Ball,   // Met Tab
                                 CB_Move1, CB_Move2, CB_Move3, CB_Move4,    // Moves
                                 CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 // Moves
                             };
            for (int i = 0; i < cba.Length; i++)
            {
                int back = cba[i].BackColor.ToArgb();
                if (back == SystemColors.Control.ToArgb() || back == 0 ||
                    !(back != -1 & back != defaultControlWhite.ToArgb())) continue;
                if (i < 6)      // Main Tab
                    tabMain.SelectedIndex = 0;
                else if (i < 9) // Met Tab
                    tabMain.SelectedIndex = 1;
                else            // Moves
                    tabMain.SelectedIndex = 3;
                goto invalid;
            }
            #endregion
            // Further logic checking
            if (Convert.ToUInt32(TB_EVTotal.Text) > 510 && !CHK_HackedStats.Checked)
            { tabMain.SelectedIndex = 2; goto invalid; }
            // If no errors detected...
            if (Util.getIndex(CB_Species) != 0) return true;
            // Else
            tabMain.SelectedIndex = 0;

            // else...
        invalid:
            { SystemSounds.Exclamation.Play(); return false; }
        }
        public byte[] preparepkx(bool click = true)
        {
            if (click)
                tabMain.Select(); // hack to make sure comboboxes are set (users scrolling through and immediately setting causes this)

            // Repopulate PK6 with Edited Stuff
            if (Util.getIndex(CB_GameOrigin) < 24)
            {
                uint EC = Util.getHEXval(TB_EC.Text);
                uint PID = Util.getHEXval(TB_PID.Text);
                uint SID = Util.ToUInt32(TB_TID.Text);
                uint TID = Util.ToUInt32(TB_TID.Text);
                uint LID = PID & 0xFFFF;
                uint HID = PID >> 16;
                uint XOR = TID ^ LID ^ SID ^ HID;

                // Ensure we don't have a shiny.
                if (XOR >> 3 == 1) // Illegal, fix. (not 16<XOR>=8)
                {
                    // Keep as shiny, so we have to mod the PID
                    PID ^= XOR;
                    TB_PID.Text = PID.ToString("X8");
                    TB_EC.Text = PID.ToString("X8");
                }
                else if ((XOR ^ 0x8000) >> 3 == 1 && PID != EC)
                    TB_EC.Text = (PID ^ 0x80000000).ToString("X8");
                else // Not Illegal, no fix.
                    TB_EC.Text = PID.ToString("X8");
            }

            pk6.EncryptionConstant = Util.getHEXval(TB_EC.Text);
            pk6.Checksum = 0; // 0 CHK for now

            // Block A
            pk6.Species = Util.getIndex(CB_Species);
            pk6.HeldItem = Util.getIndex(CB_HeldItem);
            pk6.TID = Util.ToInt32(TB_TID.Text);
            pk6.SID = Util.ToInt32(TB_SID.Text);
            pk6.EXP = Util.ToUInt32(TB_EXP.Text);
            pk6.Ability = (byte)Array.IndexOf(abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));
            pk6.AbilityNumber = Util.ToInt32(TB_AbilityNumber.Text);   // Number
            // pkx[0x16], pkx[0x17] are handled by the Medals UI (Hits & Training Bag)
            pk6.PID = Util.getHEXval(TB_PID.Text);
            pk6.Nature = (byte)Util.getIndex(CB_Nature);
            pk6.FatefulEncounter = CHK_Fateful.Checked;
            pk6.Gender = PKX.getGender(Label_Gender.Text);
            pk6.AltForm = Math.Min(MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.SelectedIndex, 32); // Form
            pk6.EV_HP = Util.ToInt32(TB_HPEV.Text);       // EVs
            pk6.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk6.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk6.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk6.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk6.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk6.CNT_Cool = Util.ToInt32(TB_Cool.Text);       // CNT
            pk6.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            pk6.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            pk6.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            pk6.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            pk6.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            pk6.Circle = CHK_Circle.Checked;
            pk6.Triangle = CHK_Triangle.Checked;
            pk6.Square = CHK_Square.Checked;
            pk6.Heart = CHK_Heart.Checked;
            pk6.Star = CHK_Star.Checked;
            pk6.Diamond = CHK_Diamond.Checked;

            pk6.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk6.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            // Already in buff (then transferred to new pkx)
            // 0x2C, 0x2D, 0x2E, 0x2F
            // 0x30, 0x31, 0x32, 0x33
            // 0x34, 0x35, 0x36, 0x37
            // 0x38, 0x39

            // Unused
            // 0x3A, 0x3B
            // 0x3C, 0x3D, 0x3E, 0x3F

            // Block B
            // Convert Nickname field back to bytes
            pk6.Nickname = TB_Nickname.Text;
            pk6.Move1 = Util.getIndex(CB_Move1);
            pk6.Move2 = Util.getIndex(CB_Move2);
            pk6.Move3 = Util.getIndex(CB_Move3);
            pk6.Move4 = Util.getIndex(CB_Move4);
            pk6.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk6.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk6.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk6.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk6.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk6.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk6.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk6.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;
            pk6.RelearnMove1 = Util.getIndex(CB_RelearnMove1);
            pk6.RelearnMove2 = Util.getIndex(CB_RelearnMove2);
            pk6.RelearnMove3 = Util.getIndex(CB_RelearnMove3);
            pk6.RelearnMove4 = Util.getIndex(CB_RelearnMove4);
            // 0x72 - Ribbon editor sets this flag (Secret Super Training)
            // 0x73
            pk6.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk6.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk6.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk6.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk6.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk6.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk6.IsEgg = CHK_IsEgg.Checked;
            pk6.IsNicknamed = CHK_Nicknamed.Checked;

            // Block C
            // Convert Latest OT field back to bytes
            pk6.HT_Name = TB_OTt2.Text;

            // 0x90-0xAF
            pk6.HT_Gender = PKX.getGender(Label_CTGender.Text);
            // Plus more, set by MemoryAmie (already in buff)

            // Block D
            // Convert OT field back to bytes
            pk6.OT_Name = TB_OT.Text;

            pk6.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            int egg_year = 2000;                                   // Default Dates
            int egg_month = 0;
            int egg_day = 0;
            int egg_location = 0;
            if (CHK_AsEgg.Checked)      // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_year = CAL_EggDate.Value.Year;
                egg_month = CAL_EggDate.Value.Month;
                egg_day = CAL_EggDate.Value.Day;
                egg_location = Util.getIndex(CB_EggLocation);
            }
            // Egg Met Data
            pk6.Egg_Year = egg_year - 2000;
            pk6.Egg_Month = egg_month;
            pk6.Egg_Day = egg_day;
            pk6.Egg_Location = egg_location;
            // Met Data
            pk6.Met_Year = CAL_MetDate.Value.Year - 2000;
            pk6.Met_Month = CAL_MetDate.Value.Month;
            pk6.Met_Day = CAL_MetDate.Value.Day;
            pk6.Met_Location = Util.getIndex(CB_MetLocation);

            if (pk6.IsEgg && pk6.Met_Location == 0)    // If still an egg, it has no hatch location/date. Zero it!
                pk6.Met_Year = pk6.Met_Month = pk6.Met_Day = 0;

            // 0xD7 Unknown

            pk6.Ball = Util.getIndex(CB_Ball);
            pk6.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk6.OT_Gender = PKX.getGender(Label_OTGender.Text);
            pk6.EncounterType = Util.ToInt32(CB_EncounterType.SelectedValue.ToString());
            pk6.Version = Util.getIndex(CB_GameOrigin);
            pk6.Country = Util.getIndex(CB_Country);
            pk6.Region = Util.getIndex(CB_SubRegion);
            pk6.ConsoleRegion = Util.getIndex(CB_3DSReg);
            pk6.Language = Util.getIndex(CB_Language);
            // 0xE4-0xE7

            // Toss in Party Stats
            Array.Resize(ref pk6.Data, PK6.SIZE_PARTY);
            pk6.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk6.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk6.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk6.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk6.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk6.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk6.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk6.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            // Unneeded Party Stats (Status, Flags, Unused)
            pk6.Data[0xE8] = pk6.Data[0xE9] = pk6.Data[0xEA] = pk6.Data[0xEB] = 
                pk6.Data[0xED] = pk6.Data[0xEE] = pk6.Data[0xEF] = 
                pk6.Data[0xFE] = pk6.Data[0xFF] = pk6.Data[0x100] = 
                pk6.Data[0x101] = pk6.Data[0x102] = pk6.Data[0x103] = 0;

            // Hax Illegality
            if (HaX)
            {
                pk6.Ability = (byte)Util.getIndex(DEV_Ability);
                pk6.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk6.FixMoves();
            pk6.FixRelearn();
            if (fieldsInitialized && pk6.Moves.All(m => m == 0))
                Util.Alert("Pokémon has no moves!");

            // Fix Handler (Memories & OT) -- no foreign memories for Pokemon without a foreign trainer (none for eggs)
            if (Menu_ModifyPK6.Checked)
                pk6.FixMemories();

            // PKX is now filled
            return pk6.Write();
        }
        // Drag & Drop Events
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string path = files[0]; // open first D&D
            openQuick(path);
        }
        // Decrypted Export
        private void dragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (!verifiedPKX()) { return; }
            {
                // Create Temp File to Drag
                Cursor.Current = Cursors.Hand;

                // Make a new file name
                byte[] dragdata = preparepkx();
                var pkx = new PK6(dragdata, "Tabs");
                string filename = Path.GetFileNameWithoutExtension(pkx.FileName) + ((e.Button == MouseButtons.Right) ? ".ek6" : ".pk6");
                dragdata = e.Button == MouseButtons.Right ? PKX.encryptArray(preparepkx()) : preparepkx();
                // Strip out party stats (if they are there)
                Array.Resize(ref dragdata, PK6.SIZE_STORED);
                // Make file
                string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
                try
                {
                    File.WriteAllBytes(newfile, dragdata);
                    DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
                }
                catch (Exception x)
                { Util.Error("Drag & Drop Error", x.ToString()); }
                File.Delete(newfile);
            }
        }
        private void dragout_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        // Dragout Display
        private void dragoutHover(object sender, EventArgs e)
        {
            dragout.BackgroundImage = Util.getIndex(CB_Species) > 0 ? Properties.Resources.slotSet : Properties.Resources.slotDel;
        }
        private void dragoutLeave(object sender, EventArgs e)
        {
            dragout.BackgroundImage = Properties.Resources.slotTrans;
        }
        private void dragoutDrop(object sender, DragEventArgs e)
        {
            openQuick(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
        }
        #endregion

        #region //// SAVE FILE FUNCTIONS ////
        private void clickVerifyCHK(object sender, EventArgs e)
        {
            if (SAV.Edited) { Util.Alert("Save has been edited. Cannot integrity check."); return; }

            if (PKX.verifyG6SAV(SAV.Data))
            { 
                Util.Alert("Checksums are correct.");
                return;
            }
            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNoCancel, "Export Checksum Info to Clipboard?"))
                return;

            Clipboard.SetText(PKX.verifyG6CHK(SAV.Data));
        }
        private void clickExportSAVBAK(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName =
                    Util.CleanFileName(ramsav == null
                        ? $"main ({SAV.OT} - {SAV.TID}).bak"
                        : $"ramsav ({SAV.OT} - {SAV.TID}).bak")
            };
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            string path = sfd.FileName;
            File.WriteAllBytes(sfd.FileName, ramsav ?? SAV.BAK);
            Util.Alert("Saved Backup of current SAV to:", path);
        }
        private void clickExportSAV(object sender, EventArgs e)
        {
            if (!Menu_ExportSAV.Enabled)
                return;

            // Chunk Error Checking
            string err = SAV.checkChunkFF();
            if (err.Length > 0 && Util.Prompt(MessageBoxButtons.YesNo, err, "Continue saving?") != DialogResult.Yes)
                return;

            if (SAV.Box > -1)
                SAV.CurrentBox = CB_BoxSelect.SelectedIndex;
            byte[] sav = SAV.Write();

            SaveFileDialog cySAV = new SaveFileDialog();
            // Try for file path
            string cyberpath = Util.GetTempFolder();
            if (ramsav != null && Directory.Exists(path3DS))
            {
                cySAV.InitialDirectory = Path.GetPathRoot(path3DS);
                cySAV.RestoreDirectory = true;
            }
            else if (path3DS != null && File.Exists(Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup", "main")))
            {
                cySAV.InitialDirectory = Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup");
                cySAV.RestoreDirectory = true;
            }
            else if (pathSDF != null && Directory.Exists(pathSDF))
            {
                cySAV.InitialDirectory = pathSDF;
                cySAV.RestoreDirectory = true;
            }
            else if (Directory.Exists(Path.Combine(cyberpath, "root")))
            {
                cySAV.InitialDirectory = Path.Combine(cyberpath, "root");
                cySAV.RestoreDirectory = true;
            }
            else if (Directory.Exists(cyberpath))
            {
                cySAV.InitialDirectory = cyberpath;
                cySAV.RestoreDirectory = true;
            }

            // Export
            if (sender == Menu_ExportTransfer) // Export RAM SAV to another.
            {
                Util.Alert("Please specify the target cart/console-RAM save.");
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() != DialogResult.OK) return;
                string target = ofd.FileName;
                byte[] targetRAM = File.ReadAllBytes(target);
                byte[] newRAM = ram2sav.getRAM(targetRAM, sav);

                cySAV.Filter = "ramsav|*.bin";
                cySAV.FileName = "ramsav.bin";
                DialogResult sdr = cySAV.ShowDialog();
                if (sdr != DialogResult.OK) return;
                string path = cySAV.FileName;
                File.WriteAllBytes(path, newRAM);
                Util.Alert("Saved RAM SAV to:" + Environment.NewLine + path, "Target RAM:" + Environment.NewLine + target);
            }
            else if (sender == Menu_ExportRAMSAV) // Export RAM SAV if it is the currently loaded one.
            {
                cySAV.Filter = "ramsav|*.bin";
                cySAV.FileName = "ramsav.bin";
                DialogResult sdr = cySAV.ShowDialog();
                if (sdr != DialogResult.OK) return;
                string path = cySAV.FileName;
                File.WriteAllBytes(path, ram2sav.getRAM(ramsav, sav));
                Util.Alert("Saved RAM SAV to:", path);
            }
            else if (sender == Menu_ExportMAIN)
            {
                cySAV.Filter = "Cyber SAV|*.*";
                cySAV.FileName = L_Save.Text.Split(new[] { ": " }, StringSplitOptions.None)[1];
                DialogResult sdr = cySAV.ShowDialog();
                if (sdr != DialogResult.OK) return;
                string path = cySAV.FileName;
                File.WriteAllBytes(path, sav);
                Util.Alert("Saved Cyber SAV to:", path);
            }
        }
        // Box/SAV Functions //
        private void clickBoxRight(object sender, EventArgs e)
        {
            CB_BoxSelect.SelectedIndex = (CB_BoxSelect.SelectedIndex + 1) % 31;
        }
        private void clickBoxLeft(object sender, EventArgs e)
        {
            CB_BoxSelect.SelectedIndex = (CB_BoxSelect.SelectedIndex + 30) % 31;
        }
        private void clickSlot(object sender, EventArgs e)
        {
            switch (ModifierKeys)
            {
                case (Keys.Control | Keys.Alt): clickClone(sender, e); break;
                case Keys.Control: clickView(sender, e); break;
                case Keys.Shift: clickSet(sender, e); break;
                case Keys.Alt: clickDelete(sender, e); break;
            }
        }
        private void clickView(object sender, EventArgs e)
        {
            int slot = getSlot(sender);
            int offset = getPKXOffset(slot);

            if (SlotPictureBoxes[slot].Image == null)
            { SystemSounds.Exclamation.Play(); return; }

            // Load the PKX file
            PK6 pk = SAV.getPK6Stored(offset);
            if (pk.Sanity == 0 && pk.Species != 0)
            {
                try
                { 
                    populateFields(pk.Data);
                }
                catch // If it fails, try XORing encrypted zeroes
                {
                    try
                    {
                        byte[] blank = (byte[])blankEK6.Clone();

                        for (int i = 0; i < PK6.SIZE_STORED; i++)
                            blank[i] = (byte)(pk6.Data[i] ^ blank[i]);

                        populateFields(blank);
                    }
                    catch   // Still fails, just let the original errors occur.
                    { populateFields(pk6.Data); }
                }
                // Visual to display what slot is currently loaded.
                getSlotColor(slot, Properties.Resources.slotView);
            }
            else
                SystemSounds.Exclamation.Play();
        }
        private void clickSet(object sender, EventArgs e)
        {
            if (!verifiedPKX()) { return; }
            int slot = getSlot(sender);
            if (slot == 30 && (CB_Species.SelectedIndex == 0 || CHK_IsEgg.Checked)) { Util.Alert("Can't have empty/egg first slot."); return; }
            int offset = getPKXOffset(slot);

            byte[] pkxdata = preparepkx();
            if (!(SAV.ORAS || SAV.ORASDEMO))
            {
                PK6 pk = new PK6(pkxdata);
                // User Protection
                string err = "";
                if (pk.Moves.Any(m => m > 617))
                    err = "Move does not exist in X/Y.";
                else if (pk.Ability > 188)
                    err = "Ability does not exist in X/Y.";
                else if (pk.HeldItem > 717)
                    err = "Item does not exist in X/Y.";

                if ((err != "") && Util.Prompt(MessageBoxButtons.YesNo, err, "Continue?") != DialogResult.Yes)
                    return;
            }
            if (slot >= 30 && slot < 36) // Party
                SAV.setPK6Party(new PK6(pkxdata), offset);
            else if (slot < 30 || (slot >= 36 && slot < 42 && DEV_Ability.Enabled))
                SAV.setPK6Stored(new PK6(pkxdata), offset);
            else return;
            
            if (slot >= 30 && slot < 36) 
                setParty();
            else 
                getQuickFiller(SlotPictureBoxes[slot], pkxdata);

            getSlotColor(slot, Properties.Resources.slotSet);
        }
        private void clickDelete(object sender, EventArgs e)
        {
            int slot = getSlot(sender);
            if (slot == 30 && SAV.PartyCount == 1 && !DEV_Ability.Enabled) { Util.Alert("Can't delete first slot."); return; }
            
            if (slot >= 30 && slot < 36) // Party
            { SAV.setPK6Party(new PK6(new byte[PK6.SIZE_PARTY]), getPKXOffset(slot)); setParty(); return; }
            if (slot < 30 || (slot >= 36 && slot < 42 && DEV_Ability.Enabled))
            { SAV.setPK6Stored(new PK6(new byte[PK6.SIZE_STORED]), getPKXOffset(slot)); }
            else return;

            SlotPictureBoxes[slot].Image = null;
            getSlotColor(slot, Properties.Resources.slotDel);
        }
        private void clickClone(object sender, EventArgs e)
        {
            if (getSlot(sender) > 30) return; // only perform action if cloning to boxes
            if (!verifiedPKX()) { return; } // don't copy garbage to the box

            byte[] pkxdata;
            int box = CB_BoxSelect.SelectedIndex + 1; // get box we're cloning to

            if (Util.Prompt(MessageBoxButtons.YesNo, $"Clone Pokemon from Editing Tabs to all slots in Box {box}?") == DialogResult.Yes)
                pkxdata = preparepkx();
            else if (Util.Prompt(MessageBoxButtons.YesNo, $"Delete Pokemon from all slots in Box {box}?") == DialogResult.Yes)
                pkxdata = new byte[PK6.SIZE_STORED];
            else
                return; // abort clone/delete

            for (int i = 0; i < 30; i++) // write encrypted array to all box slots
            {
                SAV.setPK6Stored(new PK6(pkxdata), getPKXOffset(i));
                getQuickFiller(SlotPictureBoxes[i], pkxdata);
            }
        }
        private void updateEggRNGSeed(object sender, EventArgs e)
        {
            // Run through a LINQ filter for fun; works fine for GUI purposes, although LINQ may not be the fastest way to do it!
            string filterText = TB_RNGSeed.Text.Select(char.ToUpper).Where("0123456789ABCDEF".Contains).Aggregate("", (str, c) => str + c);
            if (filterText.Length != TB_RNGSeed.Text.Length)
            {
                Util.Alert("Expected HEX (0-9, A-F).", "Received: " + Environment.NewLine + TB_RNGSeed.Text);
                // Reset to Stored Value
                TB_RNGSeed.Text = SAV.DaycareRNGSeed.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            if (TB_RNGSeed.Text.Length == 0)
            {
                // Reset to 0
                TB_RNGSeed.Text = 0.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            // Write final value back to the save
            ulong value = Convert.ToUInt64(TB_RNGSeed.Text, 16);
            if (value != SAV.DaycareRNGSeed)
            {
                SAV.DaycareRNGSeed = value;
                SAV.Edited = true;
            }            
        }
        private void updateGameSync(object sender, EventArgs e)
        {
            // Run through a LINQ filter for fun; works fine for GUI purposes, although LINQ may not be the fastest way to do it!
            string filterText = TB_GameSync.Text.Select(char.ToUpper).Where("0123456789ABCDEF".Contains).Aggregate("", (str, c) => str + c);
            if (filterText.Length != TB_GameSync.Text.Length)
            {
                Util.Alert("Expected HEX (0-9, A-F).", "Received: " + Environment.NewLine + TB_GameSync.Text);
                // Reset to Stored Value
                TB_GameSync.Text = SAV.GameSyncID.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            if (TB_GameSync.Text.Length == 0)
            {
                // Reset to 0
                TB_GameSync.Text = 0.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            // Write final value back to the save
            ulong value = Convert.ToUInt64(TB_GameSync.Text, 16);
            if (value != SAV.GameSyncID)
            {
                SAV.GameSyncID = value;
                SAV.Edited = true;
            }
        }
        private void updateSecure1(object sender, EventArgs e)
        {
            // Run through a LINQ filter for fun; works fine for GUI purposes, although LINQ may not be the fastest way to do it!
            string filterText = TB_Secure1.Text.Select(char.ToUpper).Where("0123456789ABCDEF".Contains).Aggregate("", (str, c) => str + c);
            if (filterText.Length != TB_Secure1.Text.Length)
            {
                Util.Alert("Expected HEX (0-9, A-F).", "Received: " + Environment.NewLine + TB_Secure1.Text);
                // Reset to Stored Value
                TB_Secure1.Text = SAV.Secure1.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            if (TB_Secure1.Text.Length == 0)
            {
                // Reset to 0
                TB_Secure1.Text = 0.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            // Write final value back to the save
            ulong value = Convert.ToUInt64(TB_Secure1.Text, 16);
            if (value != SAV.Secure1)
            {
                SAV.Secure1 = value;
                SAV.Edited = true;
            }
        }
        private void updateSecure2(object sender, EventArgs e)
        {
            // Run through a LINQ filter for fun; works fine for GUI purposes, although LINQ may not be the fastest way to do it!
            string filterText = TB_Secure2.Text.Select(char.ToUpper).Where("0123456789ABCDEF".Contains).Aggregate("", (str, c) => str + c);
            if (filterText.Length != TB_Secure2.Text.Length)
            {
                Util.Alert("Expected HEX (0-9, A-F).", "Received: " + Environment.NewLine + TB_Secure2.Text);
                // Reset to Stored Value
                TB_Secure2.Text = SAV.Secure2.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            if (TB_Secure2.Text.Length == 0)
            {
                // Reset to 0
                TB_Secure2.Text = 0.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            // Write final value back to the save
            ulong value = Convert.ToUInt64(TB_Secure2.Text, 16);
            if (value != SAV.Secure2)
            {
                SAV.Secure2 = value;
                SAV.Edited = true;
            }
        }
        // Generic Subfunctions //
        private void setParty()
        {
            SAV.setParty();
            // Refresh slots
            for (int i = 0; i < 6; i++)
            {
                getQuickFiller(SlotPictureBoxes[i + 30], SAV.getPK6Stored(SAV.Party + PK6.SIZE_PARTY*i).Data);
                getQuickFiller(SlotPictureBoxes[i + 36], SAV.getPK6Stored(SAV.BattleBox + PK6.SIZE_STORED*i).Data);
            }
        }
        private int getPKXOffset(int slot)
        {
            int offset = SAV.Box + (30 * CB_BoxSelect.SelectedIndex + slot) * PK6.SIZE_STORED;

            if (slot > 29)          // Not a party
            {
                if (slot < 36)      // Party Slot
                    offset = SAV.Party + (slot - 30) * PK6.SIZE_PARTY;
                else if (slot < 42) // Battle Box Slot
                    offset = SAV.BattleBox + (slot - 36) * PK6.SIZE_STORED;
                else if (slot < 44) // Daycare
                    offset = SAV.DaycareSlot[DaycareSlot] + 8 + (slot - 42) * (PK6.SIZE_STORED + 8);
                else if (slot < 45) // GTS
                    offset = SAV.GTS;
                else if (slot < 46) // Fused
                    offset = SAV.Fused;
                else                // SUBE
                    offset = SAV.SUBE + (slot - 46) * (PK6.SIZE_STORED + 4);
            }
            return offset;
        }
        private int getSlot(object sender)
        {
            string name = sender is ToolStripItem
                ? ((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl.Name
                : (sender as PictureBox).Name;

            return Array.FindIndex(SlotPictureBoxes, p => p.Name == name);
        }
        public void setPKXBoxes()
        {
            if (SAV.Box > -1)
            {
                int boxoffset = SAV.Box + CB_BoxSelect.SelectedIndex * (PK6.SIZE_STORED * 30);
                int boxbgval = SAV.getBoxWallpaper(CB_BoxSelect.SelectedIndex);
                string imagename = "box_wp" + boxbgval.ToString("00"); if (SAV.ORAS && boxbgval > 16) imagename += "o";
                PAN_Box.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject(imagename);

                for (int i = 0; i < 30; i++)
                    getSlotFiller(boxoffset + PK6.SIZE_STORED * i, SlotPictureBoxes[i]);
            }

            // Reload Party
            if (SAV.Party > -1)
            for (int i = 0; i < 6; i++)
                getSlotFiller(SAV.Party + PK6.SIZE_PARTY * i, SlotPictureBoxes[i + 30]);

            // Reload Battle Box
            if (SAV.BattleBox > -1)
            for (int i = 0; i < 6; i++)
                getSlotFiller(SAV.BattleBox + PK6.SIZE_STORED * i, SlotPictureBoxes[i + 36]);

            // Reload Daycare
            if (SAV.Daycare > -1)
            {
                Label[] dclabela = { L_DC1, L_DC2, };
                TextBox[] dctexta = { TB_Daycare1XP, TB_Daycare2XP };
                uint[] exp = { SAV.DaycareEXP1, SAV.DaycareEXP2 };
                bool[] occ = { SAV.DaycareOccupied1, SAV.DaycareOccupied2 };

                for (int i = 0; i < 2; i++)
                {
                    getSlotFiller(SAV.DaycareSlot[DaycareSlot] + PK6.SIZE_STORED * i + 8 * (i + 1), SlotPictureBoxes[i + 42]);
                    dctexta[i].Text = exp[i].ToString();
                    if (occ[i])   // If Occupied
                        dclabela[i].Text = $"{(i + 1)}: ✓";
                    else
                    {
                        dclabela[i].Text = $"{(i + 1)}: ✘";
                        SlotPictureBoxes[i + 42].Image = Util.ChangeOpacity(SlotPictureBoxes[i + 42].Image, 0.6);
                    }
                }
                DayCare_HasEgg.Checked = SAV.DaycareHasEgg;
                TB_RNGSeed.Text = SAV.DaycareRNGSeed.ToString("X16");
            }

            // GTS
            if (SAV.GTS > -1)
            getSlotFiller(SAV.GTS, SlotPictureBoxes[44]);

            // Fused
            if (SAV.Fused > -1)
            getSlotFiller(SAV.Fused, SlotPictureBoxes[45]);

            // SUBE
            if (SAV.SUBE > -1)
            for (int i = 0; i < 3; i++)
            {
                int offset = SAV.SUBE + i * (PK6.SIZE_STORED + 4);
                if (BitConverter.ToUInt64(SAV.Data, offset) != 0)
                    getSlotFiller(offset, SlotPictureBoxes[46 + i]);
                else SlotPictureBoxes[46 + i].Image = null;
            }

            // Recoloring of a storage box slot (to not show for other storage boxes)
            if (colorizedslot < 32)
                SlotPictureBoxes[colorizedslot].BackgroundImage = colorizedbox == CB_BoxSelect.SelectedIndex ? colorizedcolor : null;
        }
        private void setBoxNames()
        {
            if (SAV.Box < 0)
                return;
            int selectedbox = CB_BoxSelect.SelectedIndex; // precache selected box
            // Build ComboBox Dropdown Items
            try
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 0; i < 31; i++)
                    CB_BoxSelect.Items.Add(SAV.getBoxName(i));
            }
            catch
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 0; i < 31; i++)
                    CB_BoxSelect.Items.Add("BOX " + (i+1));
            }
            CB_BoxSelect.SelectedIndex = selectedbox;    // restore selected box
        }
        private void getQuickFiller(PictureBox pb, byte[] dslotdata = null)
        {
            if (!fieldsInitialized) return;
            dslotdata = dslotdata ?? preparepkx(false); // don't perform control loss click

            if (pb == dragout) L_QR.Visible = BitConverter.ToInt16(dslotdata, 0x08) != 0; // Species
            pb.Image = PKX.getSprite(dslotdata);
        }
        private void getSlotFiller(int offset, PictureBox pb)
        {
            if (SAV.getData(offset, PK6.SIZE_STORED).SequenceEqual(new byte[PK6.SIZE_STORED]))
            {
                // 00s present in slot.
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                return;
            }
            PK6 p = SAV.getPK6Stored(offset);
            if (p.Sanity != 0 || p.Checksum != p.CalculateChecksum()) // Invalid
            {
                // Bad Egg present in slot.
                pb.Image = null;
                pb.BackColor = Color.Red;
                return;
            }
            // Something stored in slot. Only display if species is valid.
            pb.Image = p.Species == 0 ? null : p.Sprite;
            pb.BackColor = Color.Transparent;
        }
        private void getSlotColor(int slot, Image color)
        {
            foreach (PictureBox t in SlotPictureBoxes)
                t.BackgroundImage = null;

            if (slot < 32)
                colorizedbox = CB_BoxSelect.SelectedIndex;

            SlotPictureBoxes[slot].BackgroundImage = color;
            colorizedcolor = color;
            colorizedslot = slot;
        }
        private void getBox(object sender, EventArgs e)
        {
            setPKXBoxes();
        }

        private int DaycareSlot;
        private void switchDaycare(object sender, EventArgs e)
        {
            if (!SAV.ORAS) return;
            if (DialogResult.Yes == Util.Prompt(MessageBoxButtons.YesNo, "Would you like to switch the view to the other Daycare?",
                $"Currently viewing daycare {DaycareSlot + 1}."))
                // If ORAS, alter the daycare offset via toggle.
                DaycareSlot ^= 1;

            // Refresh Boxes
            setPKXBoxes();
        }

        private void dumpBoxesToDB(string path, bool individualBoxFolders)
        {
            PK6[] boxdata = SAV.BoxData;
            for (int i = 0; i < boxdata.Length; i++)
            {
                PK6 pk = boxdata[i];
                if (pk.Species == 0 || pk.Sanity != 0)
                    continue;
                string fileName = Util.CleanFileName(pk.FileName);
                string boxfolder = "";
                if (individualBoxFolders)
                {
                    boxfolder = "BOX" + ((i / 30) + 1);
                    Directory.CreateDirectory(Path.Combine(path, boxfolder));
                }
                if (!File.Exists(Path.Combine(Path.Combine(path, boxfolder), fileName)))
                    File.WriteAllBytes(Path.Combine(Path.Combine(path, boxfolder), fileName), pk.Data.Take(PK6.SIZE_STORED).ToArray());
            }
        }
        private void loadBoxesFromDB(string path)
        {
            if (path == "") return;
            int offset = SAV.Box;
            int ctr = CB_BoxSelect.SelectedIndex * 30;
            int pastctr = 0;

            // Clear out the box data array.
            // Array.Clear(savefile, offset, size * 30 * 31);
            DialogResult dr = Util.Prompt(MessageBoxButtons.YesNoCancel, "Clear subsequent boxes when importing data?", "If you only want to overwrite for new data, press no.");
            if (dr == DialogResult.Cancel) return;
            if (dr == DialogResult.Yes)
            {
                for (int i = ctr; i < 30 * 31; i++)
                    SAV.setData(blankEK6, offset + i * PK6.SIZE_STORED);
            }
            string[] filepaths = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);

            foreach (string t in filepaths)
            {
                long len = new FileInfo(t).Length;
                if (len > PK6.SIZE_PARTY)
                    continue;

                if (
                    len != PK6.SIZE_STORED && len != PK6.SIZE_PARTY // 6th Gen
                    && len != 136 && len != 220 && len != 236 // 5th Gen
                    && len != 100 && len != 80) // 4th Gen
                    continue;
                byte[] data = new byte[PK6.SIZE_STORED];
                switch (Path.GetExtension(t)) // Filter all files by extension
                {
                    case ".pk5":
                    case ".pk4":
                    case ".pk3":
                    case ".3gpkm":
                    case ".pkm":
                    {
                        // Verify PKM (decrypted)
                        byte[] input = File.ReadAllBytes(t);
                        if (!PKX.verifychk(input)) continue;
                        {
                            try // to convert g5pkm
                            { data = PKX.encryptArray(Converter.ConvertPKMtoPK6(input)); pastctr++; }
                            catch { continue; }
                        }
                    }
                        break;
                    case ".pk6":
                    case ".pkx":
                    {
                        byte[] input = File.ReadAllBytes(t);
                        if ((BitConverter.ToUInt16(input, 0xC8) == 0) && (BitConverter.ToUInt16(input, 0x58) == 0))
                        {
                            if (BitConverter.ToUInt16(input, 0x8) == 0) // if species = 0
                                continue;
                            Array.Resize(ref input, PK6.SIZE_STORED);

                            if (PKX.getCHK(input) != BitConverter.ToUInt16(input, 0x6)) continue;
                            data = PKX.encryptArray(input);
                        }
                    }
                        break;
                    case ".ek6":
                    case ".ekx":
                    {
                        byte[] input = File.ReadAllBytes(t);
                        Array.Resize(ref input, PK6.SIZE_STORED);
                        Array.Copy(input, data, PK6.SIZE_STORED);
                        // check if it is good data
                        byte[] decrypteddata = PKX.decryptArray(input);

                        if (BitConverter.ToUInt16(decrypteddata, 0xC8) != 0 && BitConverter.ToUInt16(decrypteddata, 0x58) != 0)
                            continue; 
                        if (BitConverter.ToUInt16(decrypteddata, 0x8) == 0) // if species = 0
                            continue;
                        if (PKX.getCHK(input) != BitConverter.ToUInt16(decrypteddata, 0x6)) continue;
                    }
                        break;
                    default:
                        continue;
                }
                SAV.setEK6Stored(data, offset + ctr++ * PK6.SIZE_STORED);
                if (ctr == 30 * 31) break; // break out if we have written all 31 boxes
            }
            if (ctr <= 0) return; 

            setPKXBoxes();
            string result = $"Loaded {ctr} files to boxes.";
            if (pastctr > 0)
                Util.Alert(result, $"Conversion successful for {pastctr} past generation files.");
            else
                Util.Alert(result);
        }
        private void B_SaveBoxBin_Click(object sender, EventArgs e)
        {
            DialogResult dr = Util.Prompt(MessageBoxButtons.YesNoCancel, "Yes: Export All Boxes" + Environment.NewLine + String.Format("No: Export {1} (Box {0})", CB_BoxSelect.SelectedIndex + 1, CB_BoxSelect.Text) + Environment.NewLine + "Cancel: Abort");

            if (dr == DialogResult.Yes)
            {
                SaveFileDialog sfd = new SaveFileDialog {Filter = "Box Data|*.bin", FileName = "pcdata.bin"};
                if (sfd.ShowDialog() == DialogResult.OK)
                    File.WriteAllBytes(sfd.FileName, SAV.getData(SAV.Box, PK6.SIZE_STORED * 30 * 31));
            }
            else if (dr == DialogResult.No)
            {
                SaveFileDialog sfd = new SaveFileDialog {Filter = "Box Data|*.bin", FileName = "boxdata.bin"};
                if (sfd.ShowDialog() == DialogResult.OK)
                    File.WriteAllBytes(sfd.FileName, SAV.getData(SAV.Box + PK6.SIZE_STORED * 30 * CB_BoxSelect.SelectedIndex, PK6.SIZE_STORED * 30));
            }
        }

        // Subfunction Save Buttons //
        private void B_OpenWondercards_Click(object sender, EventArgs e)
        {
            // Open Wondercard Menu
            new SAV_Wondercard().ShowDialog();
        }
        private void B_OpenBoxLayout_Click(object sender, EventArgs e)
        {
            // Open Box Layout Menu
            new SAV_BoxLayout(CB_BoxSelect.SelectedIndex).ShowDialog();
            setBoxNames(); // fix box names
            setPKXBoxes(); // refresh box background
        }
        private void B_OpenTrainerInfo_Click(object sender, EventArgs e)
        {
            new SAV_Trainer().ShowDialog();
            // Refresh PK#->PK6 conversion info
            Converter.updateConfig(SAV.SubRegion, SAV.Country, SAV.ConsoleRegion, SAV.OT, SAV.Gender);
        }
        private void B_OpenPokepuffs_Click(object sender, EventArgs e)
        {
            new SAV_Pokepuff().ShowDialog();
        }
        private void B_OpenItemPouch_Click(object sender, EventArgs e)
        {
            new SAV_Inventory().ShowDialog();
        }
        private void B_OpenBerryField_Click(object sender, EventArgs e)
        {
            if (SAV.ORAS)
            {
                DialogResult dr = Util.Prompt(MessageBoxButtons.YesNo, "No editing support for ORAS :(", "Repopulate all with random berries?");
                if (dr != DialogResult.Yes) return; // abort
                // Randomize the trees.
                
                byte[] ready = { 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x80, 0x40, 0x01, 0x00, 0x00, 0x00, };
                int[] berrylist =
                {
                    0,149,150,151,152,153,154,155,156,157,158,159,160,161,162,
                    163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,
                    178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,
                    193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,
                    208,209,210,211,212,686,687,688,
                };
                for (int i = 0; i < 90; i++)
                {
                    Array.Copy(ready, 0, SAV.Data, SAV.BerryField + 0x10 * i, 0x10); // prep the berry template tree (which we replace offset 0x6 for the Tree Item)
                    int randberry = (int)(Util.rnd32() % berrylist.Length); // generate a random berry that will go into the tree
                    int index = berrylist[randberry]; // get berry item ID from list
                    Array.Copy(BitConverter.GetBytes(index), 0, SAV.Data, SAV.BerryField + 0x10 * i + 6, 2); // put berry into tree.
                }
            }
            else
                new SAV_BerryField().ShowDialog();
        }
        private void B_OpenEventFlags_Click(object sender, EventArgs e)
        {
            // Open Flag Menu
            if (SAV.ORAS)
                new SAV_EventFlagsORAS().ShowDialog();
            else
                new SAV_EventFlagsXY().ShowDialog();
        }
        private void B_OpenSuperTraining_Click(object sender, EventArgs e)
        {
            // Open ST Menu
            new SAV_SuperTrain(this).ShowDialog();
        }
        private void B_OpenOPowers_Click(object sender, EventArgs e)
        {
            // Open O-Power Menu
            if (SAV.ORAS)
            {
                DialogResult dr = Util.Prompt(MessageBoxButtons.YesNo, "No editing support for ORAS :(", "Max O-Powers with a working code?");
                if (dr != DialogResult.Yes) return;
                byte[] maxoras =
                { 
                    0x00, 0x01, 0x01, 0x01,
                    0x01, 0x00, 0x01, 0x01,
                    0x01, 0x01, 0x00, 0x01,
                    0x01, 0x01, 0x01, 0x00,
                    0x01, 0x01, 0x01, 0x01,
                    0x00, 0x01, 0x01, 0x01,
                    0x01, 0x00, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x00, 0x01,
                    0x01, 0x01, 0x01, 0x00,
                    0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01,
                    0x01, 0x00, 0x00, 0x00, 
                };
                Array.Copy(maxoras, 0, SAV.Data, SAV.OPower, 0x44);
            }
            else
                new SAV_OPower().ShowDialog();
        }
        private void B_OpenPokedex_Click(object sender, EventArgs e)
        {
            // Open Pokedex Menu
            if (SAV.ORAS)
                new SAV_PokedexORAS().ShowDialog();
            else
                new SAV_PokedexXY().ShowDialog();
        }
        private void B_OUTPasserby_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNoCancel, "Export Passerby Info to Clipboard?"))
                return;

            string result = "PSS List" + Environment.NewLine;
            string[] headers = { "PSS Data - Friends", "PSS Data - Acquaintances", "PSS Data - Passerby", };
            int offset = SAV.PSS;
            for (int g = 0; g < 3; g++)
            {
                result += Environment.NewLine
                    + "----" + Environment.NewLine
                    + headers[g] + Environment.NewLine
                    + "----" + Environment.NewLine;
                // uint count = BitConverter.ToUInt32(savefile, offset + 0x4E20);
                int r_offset = offset;

                for (int i = 0; i < 100; i++)
                {
                    ulong unkn = BitConverter.ToUInt64(SAV.Data, r_offset);
                    if (unkn == 0) break; // No data present here
                    if (i > 0) result += Environment.NewLine + Environment.NewLine;

                    string otname = Util.TrimFromZero(Encoding.Unicode.GetString(SAV.Data, r_offset + 8, 0x1A));
                    string message = Util.TrimFromZero(Encoding.Unicode.GetString(SAV.Data, r_offset + 0x22, 0x22));

                    // Trim terminated

                    // uint unk1 = BitConverter.ToUInt32(savefile, r_offset + 0x44);
                    // ulong unk2 = BitConverter.ToUInt64(savefile, r_offset + 0x48);
                    // uint unk3 = BitConverter.ToUInt32(savefile, r_offset + 0x50);
                    // uint unk4 = BitConverter.ToUInt16(savefile, r_offset + 0x54);
                    byte region = SAV.Data[r_offset + 0x56];
                    byte country = SAV.Data[r_offset + 0x57];
                    byte game = SAV.Data[r_offset + 0x5A];
                    // ulong outfit = BitConverter.ToUInt64(savefile, r_offset + 0x5C);
                    int favpkm = BitConverter.ToUInt16(SAV.Data, r_offset + 0x9C) & 0x7FF;
                    string gamename;
                    try { gamename = gamelist[game]; }
                    catch { gamename = "UNKNOWN GAME"; }

                    string[] cr = PKX.getCountryRegionText(country, region, curlanguage);
                    result +=
                        "OT: " + otname + Environment.NewLine +
                        "Message: " + message + Environment.NewLine +
                        "Game: " + gamename + Environment.NewLine +
                        "Country: " + cr[0] + Environment.NewLine +
                        "Region: " + cr[1] + Environment.NewLine +
                        "Favorite: " + specieslist[favpkm];

                    r_offset += 0xC8; // Advance to next entry
                }
                offset += 0x5000; // Advance to next block
            }
            Clipboard.SetText(result);
        }
        private void B_OUTHallofFame_Click(object sender, EventArgs e)
        {
            // Open HoF Menu
            new SAV_HallOfFame().ShowDialog();
        }
        private void B_OpenSecretBase_Click(object sender, EventArgs e)
        {
            // Open Secret Base Menu
            new SAV_SecretBase().ShowDialog();
        }
        private void B_JPEG_Click(object sender, EventArgs e)
        {
            int offset = SAV.JPEG;
            string filename = Encoding.Unicode.GetString(SAV.Data, offset, 0x1A).Trim();
            filename += "'s picture";
            offset += 0x54;
            if (SAV.Data[offset] != 0xFF)
            {
                Util.Alert("No PGL picture data found in the save file!");
                return;
            }
            const int length = 0xE004;

            byte[] jpeg = new byte[length];
            Array.Copy(SAV.Data, offset, jpeg, 0, length);
            SaveFileDialog savejpeg = new SaveFileDialog {FileName = filename, Filter = "JPEG|*.jpeg"};
            if (savejpeg.ShowDialog() != DialogResult.OK) return;
            string path = savejpeg.FileName;
            if (File.Exists(path))
            {
                // File already exists, save a .bak
                byte[] backupfile = File.ReadAllBytes(path);
                File.WriteAllBytes(path + ".bak", backupfile);
            }
            File.WriteAllBytes(path, jpeg);
        }
        private void B_OpenDB_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("db"))
                new SAV_Database(this).Show();
            else
                Util.Alert("PKHeX's database was not found",
                    "Please dump all boxes from a save file, then ensure the 'db' folder exists.");
        }
        // Save Folder Related
        private void clickSaveFileName(object sender, EventArgs e)
        {
            // Get latest SaveDataFiler save location
            pathSDF = Util.GetSDFLocation();
            path3DS = Util.get3DSLocation();
            string path = null;

            if (path3DS != null && File.Exists(Path.Combine(Path.GetPathRoot(path3DS), "ramsav.bin")))
                path = Path.Combine(Path.GetPathRoot(path3DS), "ramsav.bin");
            if (path3DS != null && Directory.Exists(Path.Combine(path3DS, "SaveDataBackup")) && ModifierKeys != Keys.Control)
                path = Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup", "main");
            else if (pathSDF != null && ModifierKeys != Keys.Shift) // if we have a result
                path = Path.Combine(pathSDF, "main");
            else if (File.Exists(Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root", "main")))) // else if cgse exists
                path = Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root", "main"));

            if (path == null || !File.Exists(path)) return;
            if (Util.Prompt(MessageBoxButtons.YesNo, "Open save file from the following location?", path) == DialogResult.Yes)
                openQuick(path); // load save
        }

        // Drag & Drop within Box
        private void pbBoxSlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.Clicks != 1) return;
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift || ModifierKeys == (Keys.Control | Keys.Alt))
            { clickSlot(sender, e); return; }
            PictureBox pb = (PictureBox)(sender);
            if (pb.Image == null)
                return;

            pkm_from_slot = getSlot(sender);
            int offset = getPKXOffset(pkm_from_slot);
            // Create Temp File to Drag
            Cursor.Current = Cursors.Hand;

            // Prepare Data
            pkm_from = SAV.getData(offset, PK6.SIZE_STORED);
            pkm_from_offset = offset;

            // Make a new file name based off the PID
            byte[] dragdata = PKX.decryptArray(pkm_from);
            Array.Resize(ref dragdata, PK6.SIZE_STORED);
            var pkx = new PK6(dragdata, "Boxes");
            string filename = pkx.FileName;

            // Make File
            string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
            try
            {
                File.WriteAllBytes(newfile, dragdata);
                DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
            }
            catch (ArgumentException x)
            { Util.Error("Drag & Drop Error:", x.ToString()); }
            File.Delete(newfile);
            pkm_from_offset = 0;
        }
        private void pbBoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            int slot = getSlot(sender);
            int offset = getPKXOffset(slot);

            // Check for In-Dropped files (PKX,SAV,ETC)
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (Directory.Exists(files[0])) { loadBoxesFromDB(files[0]); return; }
            if (pkm_from_offset == 0)
            {
                if (files.Length > 0)
                {
                    FileInfo fi = new FileInfo(files[0]);

                    // Detect if PKM/PKX
                    if ((fi.Length == 136) || (fi.Length == 220) || (fi.Length == 236) || (fi.Length == 100) || (fi.Length == 80))
                    {
                        byte[] input = File.ReadAllBytes(files[0]);
                        if (!PKX.verifychk(input)) Util.Alert("Invalid File Loaded.", "Checksum is not valid.");
                        try // to convert past gen pkm
                        {
                            byte[] data = Converter.ConvertPKMtoPK6(input);
                            SAV.setPK6Stored(new PK6(data), offset);
                        }
                        catch
                        { Util.Error("Attempted to load previous generation PKM.", "Conversion failed."); }
                    }
                    else if (fi.Length == PK6.SIZE_STORED || fi.Length == PK6.SIZE_PARTY)
                    {
                        byte[] data = File.ReadAllBytes(files[0]);
                        if (fi.Extension == ".pkx" || fi.Extension == ".pk6")
                            data = PKX.encryptArray(data);
                        else if (fi.Extension != ".ekx" && fi.Extension != ".ek6")
                        { openQuick(files[0]); return; } // lazy way of aborting 

                        byte[] decdata = PKX.decryptArray(data);
                        if (!PKX.verifychk(decdata))
                            Util.Alert("Attempted to load Invalid File.", "Checksum is not valid.");
                        else
                        {
                            SAV.setEK6Stored(data, offset);
                            getQuickFiller(SlotPictureBoxes[slot], decdata);
                            getSlotColor(slot, Properties.Resources.slotSet);
                        }
                    }
                    else // not PKX/EKX, so load with the general function
                    { openQuick(files[0]); }
                }
            }
            else
            {
                if (ModifierKeys == Keys.Alt && slot > -1) // overwrite delete old slot
                {
                    byte[] cleardata = new Byte[PK6.SIZE_STORED];

                    // Clear from slot picture
                    getQuickFiller(SlotPictureBoxes[pkm_from_slot], cleardata);

                    // Clear from slot data
                    SAV.setPK6Stored(new PK6(cleardata), pkm_from_offset);
                }
                else if (ModifierKeys != Keys.Control && slot > -1)
                {
                    // Load data from destination
                    PK6 pk = SAV.getPK6Stored(offset);

                    // Swap slot picture
                    getQuickFiller(SlotPictureBoxes[pkm_from_slot], pk.Data);

                    // Swap slot data to source
                    SAV.setPK6Stored(pk, pkm_from_offset);
                }
                // Copy from temp slot to new.
                SAV.setEK6Stored(pkm_from, offset);
                getQuickFiller(SlotPictureBoxes[slot], PKX.decryptArray(pkm_from));

                pkm_from_offset = 0; // Clear offset value
            }

            SAV.Edited = true;
        }
        private void pbBoxSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }
        private byte[] pkm_from = (byte[])blankEK6.Clone();
        private int pkm_from_offset;
        private int pkm_from_slot = -1;
        #endregion
    }
}
