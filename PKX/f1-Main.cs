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
            #region Initialize Form
            new Thread(() => new SplashScreen().ShowDialog()).Start();
            pkm_from = SAV.BlankPKM.EncryptedPartyData;
            InitializeComponent();
            CB_ExtraBytes.SelectedIndex = 0;
            SaveFile.SetUpdateDex = Menu_ModifyDex.Checked;
            SaveFile.SetUpdatePKM = Menu_ModifyPK6.Checked;

            // Set up form properties and arrays.
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
            relearnPB = new[] { PB_WarnRelearn1, PB_WarnRelearn2, PB_WarnRelearn3, PB_WarnRelearn4 };
            movePB = new[] { PB_WarnMove1, PB_WarnMove2, PB_WarnMove3, PB_WarnMove4 };
            defaultControlWhite = CB_Species.BackColor;
            defaultControlText = Label_Species.ForeColor;

            // Set up Language Selection
            foreach (var cbItem in main_langlist)
                CB_MainLanguage.Items.Add(cbItem);

            // ToolTips for Drag&Drop
            new ToolTip().SetToolTip(dragout, "PKM QuickSave");

            // Box Drag & Drop
            foreach (PictureBox pb in PAN_Box.Controls)
            {
                pb.AllowDrop = true; // The PictureBoxes have their own drag&drop event handlers (pbBoxSlot)
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

            GB_OT.Click += clickGT;
            GB_nOT.Click += clickGT;
            GB_Daycare.Click += switchDaycare;
            GB_RelearnMoves.Click += clickMoves;

            TB_Nickname.Font = PKX.getPKXFont(11);
            TB_OT.Font = (Font)TB_Nickname.Font.Clone();
            TB_OTt2.Font = (Font)TB_Nickname.Font.Clone();

            Menu_Modify.DropDown.Closing += (sender, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
            };

            // Box to Tabs D&D
            dragout.AllowDrop = true;

            // Load WC6 folder to legality
            refreshWC6DB();

            #endregion
            #region Localize & Populate Fields
            string[] args = Environment.GetCommandLineArgs();
            string filename = args.Length > 0 ? Path.GetFileNameWithoutExtension(args[0]).ToLower() : "";
            HaX = filename.IndexOf("hax", StringComparison.Ordinal) >= 0;

            // Try and detect the language
            string lastTwoChars = filename.Length > 2 ? filename.Substring(filename.Length - 2) : "";
            if (lastTwoChars == "jp") lastTwoChars = "ja";
            int lang = Array.IndexOf(lang_val, lastTwoChars);
            CB_MainLanguage.SelectedIndex = lang < 0 ? 1 : lang;

            InitializeFields();
            formInitialized = true;
            #endregion
            #region Load Initial File(s)
            if (args.Length > 1) // Load the arguments
            {
                foreach (string arg in args.Skip(1).Where(a => a.Length > 4))
                    openQuick(arg, force: true);
            }
            else // Detect save
            {
                string path = detectSaveFile();
                if (path != null)
                    openQuick(path, force: true);
            }

            // Splash Screen closes on its own.
            BringToFront();
            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
            if (HaX) Util.Alert("Illegal mode activated.", "Please behave.");
            #endregion
        }

        #region Important Variables
        public static PKM pkm = new PK6(); // Tab Pokemon Data Storage
        public static SaveFile SAV = new SAV6 { Game = (int)GameVersion.AS, OT = "PKHeX", TID = 12345, SID = 54321, Language = 2, Country = 49, SubRegion = 7 }; // Save File
        public static Color defaultControlWhite, defaultControlText;
        public static string eggname = "";
        public const string DatabasePath = "db";
        private const string WC6DatabasePath = "wc6";
        private const string BackupPath = "bak";
        public static string curlanguage = "en";
        public static string[] gendersymbols = { "♂", "♀", "-" };
        public static string[] specieslist, movelist, itemlist, abilitylist, types, natures, forms,
            memories, genloc, trainingbags, trainingstage, characteristics,
            encountertypelist, gamelanguages, balllist, gamelist, pokeblocks = { };
        public static string[] metHGSS_00000, metHGSS_02000, metHGSS_03000 = { };
        public static string[] metBW2_00000, metBW2_30000, metBW2_40000, metBW2_60000 = { };
        public static string[] metXY_00000, metXY_30000, metXY_40000, metXY_60000 = { };
        public static string[] wallpapernames, puffs = { };
        public static bool unicode;
        public static List<Util.cbItem> MoveDataSource, ItemDataSource, SpeciesDataSource, BallDataSource, NatureDataSource, AbilityDataSource, VersionDataSource;

        public static volatile bool formInitialized, fieldsInitialized, fieldsLoaded;
        private static int colorizedbox = -1;
        private static Image colorizedcolor;
        private static int colorizedslot;
        private static bool HaX;
        private LegalityAnalysis Legality = new LegalityAnalysis(new PK3());
        private static readonly Image mixedHighlight = Util.ChangeOpacity(Properties.Resources.slotSet, 0.5);
        private static readonly string[] lang_val = { "ja", "en", "fr", "it", "de", "es", "ko", "zh", "pt" };
        private static readonly string[] main_langlist =
            {
                "日本語", // JPN
                "English", // ENG
                "Français", // FRE
                "Italiano", // ITA
                "Deutsch", // GER
                "Español", // SPA
                "한국어", // KOR
                "中文", // CHN
                "Português", // Portuguese
            };
        private static string origintrack;
        private readonly PictureBox[] SlotPictureBoxes, movePB, relearnPB;
        private readonly ToolTip Tip1 = new ToolTip(), Tip2 = new ToolTip(), Tip3 = new ToolTip(), NatureTip = new ToolTip();
        #endregion

        #region //// MAIN MENU FUNCTIONS ////
        // Main Menu Strip UI Functions
        private void mainMenuOpen(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "PKX File|*.pk6;*.pkx" +
                         "|EKX File|*.ek6;*.ekx" +
                         "|BIN File|*.bin" +
                         "|All Files|*.*",
                RestoreDirectory = true,
                FilterIndex = 4,
                FileName = "main",
            };

            // Reset file dialog path if it no longer exists
            if (!Directory.Exists(ofd.InitialDirectory))
                ofd.InitialDirectory = Environment.CurrentDirectory;

            // Detect main
            string path = detectSaveFile();
            if (path != null)
            { ofd.InitialDirectory = Path.GetDirectoryName(path); }
            else if (File.Exists(Path.Combine(ofd.InitialDirectory, "main")))
            { }
            else if (!Directory.Exists(ofd.InitialDirectory))
            { ofd.RestoreDirectory = false; ofd.FilterIndex = 1; ofd.FileName = ""; }

            if (ofd.ShowDialog() == DialogResult.OK) 
                openQuick(ofd.FileName);
        }
        private void mainMenuSave(object sender, EventArgs e)
        {
            if (!verifiedPKM()) return;
            PKM pk = preparePKM();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PKX File|*.pk6;*.pkx" +
                         "|EKX File|*.ek6;*.ekx" +
                         "|BIN File|*.bin" +
                         "|All Files|*.*",
                DefaultExt = "pk6",
                FileName = Util.CleanFileName(pk.FileName)
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

            if (new[] {".ekx", ".ek6", ".bin"}.Contains(ext))
                File.WriteAllBytes(path, pk.EncryptedPartyData);
            else if (new[] { ".pkx", ".pk6" }.Contains(ext))
                File.WriteAllBytes(path, pk.Data);
            else
            {
                Util.Error($"Foreign File Extension: {ext}", "Exporting as encrypted.");
                File.WriteAllBytes(path, pkm.EncryptedPartyData);
            }
        }
        private void mainMenuExit(object sender, EventArgs e)
        {
            if (ModifierKeys == (Keys.Control | Keys.Q)) // Hotkey Triggered
                if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Quit PKHeX?")) return;
            Close();
        }
        private void mainMenuAbout(object sender, EventArgs e)
        {
            // Open a new form with the About details.
            new About().ShowDialog();
        }
        // Sub Menu Options
        private void mainMenuBoxReport(object sender, EventArgs e)
        {
            var z = Application.OpenForms.Cast<Form>().FirstOrDefault(form => form.GetType() == typeof(frmReport)) as frmReport;
            if (z != null)
            { Util.CenterToForm(z, this); z.BringToFront(); return; }
            
            frmReport ReportForm = new frmReport();
            ReportForm.Show();
            ReportForm.PopulateData(SAV.BoxData);
        }
        private void mainMenuDatabase(object sender, EventArgs e)
        {
            var z = Application.OpenForms.Cast<Form>().FirstOrDefault(form => form.GetType() == typeof(SAV_Database)) as SAV_Database;
            if (z != null)
            { Util.CenterToForm(z, this); z.BringToFront(); return; }

            if (Directory.Exists("db"))
                new SAV_Database(this).Show();
            else
                Util.Alert("PKHeX's database was not found",
                    "Please dump all boxes from a save file, then ensure the 'db' folder exists.");
        }
        private void mainMenuUnicode(object sender, EventArgs e)
        {
            unicode = Menu_Unicode.Checked;
            updateUnicode();
        }
        private void mainMenuModifyDex(object sender, EventArgs e)
        {
            SaveFile.SetUpdateDex = Menu_ModifyDex.Checked;
        }
        private void mainMenuModifyPKM(object sender, EventArgs e)
        {
            SaveFile.SetUpdatePKM = Menu_ModifyPK6.Checked;
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
            if (!formInitialized)
                return;
            if (!Clipboard.ContainsText())
            { Util.Alert("Clipboard does not contain text."); return; }

            // Get Simulator Data
            ShowdownSet Set = new ShowdownSet(Clipboard.GetText());

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
                Util.getStringList("types", "en"),
                Util.getStringList("forms", "en"), gendersymbols);
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
            pkm = preparePKM();
            updateLegality();
        }
        private void clickShowdownExportPK6(object sender, EventArgs e)
        {
            if (!formInitialized)
                return;
            if (!verifiedPKM())
            { Util.Alert("Fix data before exporting."); return; }

            Clipboard.SetText(preparePKM().ShowdownText);
            Util.Alert("Exported Showdown Set to Clipboard:", Clipboard.GetText());
        }
        private void clickShowdownExportParty(object sender, EventArgs e)
        {
            if (SAV.PartyData.Length <= 0) return;
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
            if (SAV.BattleBoxData.Length <= 0) return;
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
            if (Util.get3DSLocation() != null && Directory.Exists(path = Util.GetSDFLocation()))
                Process.Start("explorer.exe", @path);
            else
                Util.Alert("Can't find the SaveDataFiler folder.");
        }
        private void clickOpenSDBFolder(object sender, EventArgs e)
        {
            string path3DS = Util.get3DSLocation();
            string path;
            if (path3DS != null && Directory.Exists(path = Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup")))
                Process.Start("explorer.exe", @path);
            else
                Util.Alert("Can't find the SaveDataBackup folder.");
        }

        // Main Menu Subfunctions
        private void openQuick(string path, bool force = false)
        {
            if (!(CanFocus || force))
            {
                SystemSounds.Asterisk.Play();
                return;
            }
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
                catch (Exception e) { Util.Error("File is in use by another program!", path, e.ToString()); return; }

                try { openFile(input, path, ext); }
                catch (Exception e) { Util.Error("Unable to load file.", e.ToString()); }
            }
        }
        private void openFile(byte[] input, string path, string ext)
        {
            MysteryGift tg; PKM temp; string c;
            byte[] footer = new byte[0];
            #region DeSmuME .dsv detect
            if (input.Length > SaveUtil.SIZE_G4RAW)
            {
                bool dsv = SaveUtil.FOOTER_DSV.SequenceEqual(input.Skip(input.Length - SaveUtil.FOOTER_DSV.Length));
                if (dsv)
                {
                    footer = input.Skip(SaveUtil.SIZE_G4RAW).ToArray();
                    input = input.Take(SaveUtil.SIZE_G4RAW).ToArray();
                }
            }
            #endregion
            #region Powersaves Read-Only Conversion
            if (input.Length == 0x10009C) // Resize to 1MB
            {
                Array.Copy(input, 0x9C, input, 0, 0x100000);
                Array.Resize(ref input, 0x100000);
            }
            // Verify the Data Input Size is Proper
            if (input.Length == 0x100000)
            {
                if (openXOR(input, path)) // Check if we can load the save via xorpad
                    return; // only if a save is loaded we abort
                if (BitConverter.ToUInt64(input, 0x10) != 0) // encrypted save
                { Util.Error("PKHeX only edits decrypted save files." + Environment.NewLine + "This save file is not decrypted.", path); return; }
                
                DialogResult sdr = Util.Prompt(MessageBoxButtons.YesNoCancel, "Press Yes to load the sav at 0x3000", "Press No for the one at 0x82000");
                if (sdr == DialogResult.Cancel)
                    return;
                int savshift = sdr == DialogResult.Yes ? 0 : 0x7F000;
                byte[] psdata = input.Skip(0x5400 + savshift).Take(SaveUtil.SIZE_G6ORAS).ToArray();
                if (BitConverter.ToUInt32(psdata, psdata.Length - 0x1F0) != SaveUtil.BEEF)
                    Array.Resize(ref psdata, SaveUtil.SIZE_G6XY);
                if (BitConverter.ToUInt32(psdata, psdata.Length - 0x1F0) != SaveUtil.BEEF)
                { Util.Error("The data file is not a valid save file", path); return; }

                openSAV(psdata, path);
            }
            #endregion
            #region SAV/PKM
            else if (SaveUtil.getSAVGeneration(input) > -1) // Supports Gen4/5/6
            { openSAV(input, path); SAV.Footer = footer; }
            else if ((temp = PKMConverter.getPKMfromBytes(input)) != null)
            {
                PKM pk = PKMConverter.convertToFormat(temp, SAV.Generation, out c);
                if (pk == null)
                    Util.Alert("Conversion failed.", c);
                else 
                    populateFields(pk);
                Console.WriteLine(c);
            }
            #endregion
            #region PC/Box Data
            else if (BitConverter.ToUInt16(input, 4) == 0 && BitConverter.ToUInt32(input, 8) > 0 && PKX.getIsPKM(input.Length / 30 / SAV.BoxCount) || PKX.getIsPKM(input.Length / 30))
            {
                if (SAV.setPCBin(input))
                    Util.Alert("PC Binary loaded.");
                else if (SAV.setBoxBin(input, CB_BoxSelect.SelectedIndex))
                    Util.Alert("Box Binary loaded.");
                else
                {
                    Util.Alert("Binary is not compatible with save file.", "Current SAV Generation: " + SAV.Generation);
                    return;
                }
                setPKXBoxes();
            }
            #endregion
            #region Battle Video
            else if (input.Length == 0x2E60 && BitConverter.ToUInt64(input, 0xE18) != 0 && BitConverter.ToUInt16(input, 0xE12) == 0)
            {
                if (SAV.Generation < 6)
                { Util.Alert("Cannot load a Gen6 Battle Video to a past generation save file."); return; }

                if (Util.Prompt(MessageBoxButtons.YesNo, "Load Batte Video Pokémon data to " + CB_BoxSelect.Text + "?", "The box will be overwritten.") != DialogResult.Yes)
                    return;

                bool? noSetb = getPKMSetOverride();
                int offset = SAV.getBoxOffset(CB_BoxSelect.SelectedIndex);
                for (int i = 0; i < 24; i++)
                {
                    byte[] data = input.Skip(0xE18 + PKX.SIZE_6PARTY * i + i / 6 * 8).Take(PKX.SIZE_6STORED).ToArray();
                    SAV.setStoredSlot(data, offset + i * SAV.SIZE_STORED, noSetb);
                }
                setPKXBoxes();
            }
            #endregion
            #region Mystery Gift (Templates)
            else if ((tg = MysteryGift.getMysteryGift(input, ext)) != null)
            {
                if (!tg.IsPokémon)
                {
                    Util.Alert("Mystery Gift is not a Pokémon.", path);
                    return;
                }
                temp = tg.convertToPKM(SAV);
                if (temp.Format == SAV.Generation && ModifierKeys == Keys.Control && SAV.HasWondercards)
                {
                    B_OpenWondercards_Click(tg, null);
                    return;
                }
                PKM pk = PKMConverter.convertToFormat(temp, SAV.Generation, out c);
                if (pk == null)
                    Util.Alert("Conversion failed.", c);
                else
                    populateFields(pk);
                Console.WriteLine(c);
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
                byte[] decryptedPS = input.Skip(0x5400).Take(SaveUtil.SIZE_G6ORAS).ToArray();

                // xor through and decrypt
                for (int z = 0; z < decryptedPS.Length; z++)
                    decryptedPS[z] ^= xorpad[0x5400 + z];

                // Weakly check the validity of the decrypted content
                if (BitConverter.ToUInt32(decryptedPS, SaveUtil.SIZE_G6ORAS - 0x1F0) != SaveUtil.BEEF) // Not OR/AS
                    if (BitConverter.ToUInt32(decryptedPS, SaveUtil.SIZE_G6XY - 0x1F0) != SaveUtil.BEEF) // Not X/Y
                        continue;
                    else
                        Array.Resize(ref decryptedPS, SaveUtil.SIZE_G6XY); // set to X/Y size
                else Array.Resize(ref decryptedPS, SaveUtil.SIZE_G6ORAS); // set to ORAS size just in case

                // Save file is now decrypted!

                // Trigger Loading of the decrypted save file.
                openSAV(decryptedPS, path);

                // Abort the opening of a non-cyber file.
                return true;
            }
            // End file check loop, check the input path for xorpads too if it isn't the same as the EXE (quite common).
            if (xorpath != exepath || loop++ > 0) return false; // no xorpad compatible
            xorpath = Path.GetDirectoryName(path); goto check;
        }
        private void openSAV(byte[] input, string path)
        {
            SaveFile sav = SaveUtil.getVariantSAV(input);
            if (sav == null || sav.Version == GameVersion.Invalid)
            { Util.Error("Invalid save file loaded. Aborting.", path); return; }
            SAV = sav;

            SAV.FilePath = Path.GetDirectoryName(path);
            SAV.FileName = Path.GetExtension(path) == ".bak"
                ? Path.GetFileName(path).Split(new[] {" ["}, StringSplitOptions.None)[0]
                : Path.GetFileName(path);
            L_Save.Text = $"SAV{SAV.Generation}: {Path.GetFileNameWithoutExtension(Util.CleanFileName(SAV.BAKName))}"; // more descriptive
            
            Menu_ExportSAV.Enabled = B_VerifyCHK.Enabled = SAV.Exportable;

            setBoxNames();   // Display the Box Names
            if (SAV.HasBox)
            {
                int startBox = SAV.CurrentBox; // FF if BattleBox
                if (startBox > SAV.BoxCount - 1) { tabBoxMulti.SelectedIndex = 1; CB_BoxSelect.SelectedIndex = 0; }
                else { tabBoxMulti.SelectedIndex = 0; CB_BoxSelect.SelectedIndex = startBox; }
            }
            setPKXBoxes();   // Reload all of the PKX Windows

            // Hide content if not present in game.
            GB_SUBE.Visible = SAV.HasSUBE;
            PB_Locked.Visible = SAV.HasBattleBox && SAV.BattleBoxLocked;

            PAN_Box.Visible = CB_BoxSelect.Visible = B_BoxLeft.Visible = B_BoxRight.Visible = SAV.HasBox;
            Menu_LoadBoxes.Enabled = Menu_DumpBoxes.Enabled = Menu_Report.Enabled = Menu_Modify.Enabled = B_SaveBoxBin.Enabled = SAV.HasBox;

            PAN_BattleBox.Visible = L_BattleBox.Visible = L_ReadOnlyPBB.Visible = SAV.HasBattleBox;
            GB_Daycare.Visible = SAV.HasDaycare;
            GB_Fused.Visible = SAV.HasFused;
            GB_GTS.Visible = SAV.HasGTS;
            B_OpenSecretBase.Visible = SAV.HasSecretBase;
            B_OpenPokepuffs.Visible = SAV.HasPuff;
            B_OUTPasserby.Visible = SAV.HasPSS;
            B_OpenBoxLayout.Visible = SAV.HasBoxWallpapers;
            B_OpenWondercards.Visible = SAV.HasWondercards;
            B_OpenSuperTraining.Visible = SAV.HasSuperTrain;
            B_OpenHallofFame.Visible = SAV.HasHoF;
            B_OpenOPowers.Visible = SAV.HasOPower;
            B_OpenPokedex.Visible = SAV.HasPokeDex;
            B_OpenBerryField.Visible = SAV.HasBerryField;
            B_Pokeblocks.Visible = SAV.HasPokeBlock;
            B_JPEG.Visible = SAV.HasJPEG;
            B_OpenEventFlags.Visible = SAV.HasEvents;
            
            // Generational Interface
            byte[] extraBytes = new byte[1];
            Tip1.RemoveAll(); Tip2.RemoveAll(); Tip3.RemoveAll(); // TSV/PSV

            CB_Country.Visible = CB_SubRegion.Visible = CB_3DSReg.Visible =
            Label_Country.Visible = Label_SubRegion.Visible = Label_3DSRegion.Visible = SAV.Generation >= 6;
            CB_Ability.Visible = TB_AbilityNumber.Visible = SAV.Generation >= 6;
            Label_EncryptionConstant.Visible = BTN_RerollEC.Visible = TB_EC.Visible = SAV.Generation >= 6;
            GB_nOT.Visible = GB_RelearnMoves.Visible = BTN_History.Visible = BTN_Ribbons.Visible = SAV.Generation >= 6;
            PB_Legal.Visible = PB_WarnMove1.Visible = PB_WarnMove2.Visible = PB_WarnMove3.Visible = PB_WarnMove4.Visible = SAV.Generation >= 6;

            PB_MarkPentagon.Visible = SAV.Generation == 6;
            PB_Legal.Visible = PB_WarnMove1.Visible = PB_WarnMove2.Visible = PB_WarnMove3.Visible = PB_WarnMove4.Visible = SAV.Generation == 6;
            TB_GameSync.Visible = TB_Secure1.Visible = TB_Secure2.Visible = L_GameSync.Visible = L_Secure1.Visible = L_Secure2.Visible = SAV.Generation == 6;

            CB_Form.Visible = Label_Form.Visible = CHK_AsEgg.Visible = GB_EggConditions.Visible = 
            Label_MetDate.Visible = CAL_MetDate.Visible = PB_Mark5.Visible = PB_Mark6.Visible = SAV.Generation >= 4;

            DEV_Ability.Enabled = DEV_Ability.Visible = SAV.Generation != 3 && HaX;
            TB_AbilityNumber.Visible = SAV.Generation >= 6 && DEV_Ability.Enabled;
            CB_Ability.Visible = !DEV_Ability.Enabled;

            switch (SAV.Generation)
            {
                case 3:
                    extraBytes = PK3.ExtraBytes;
                    break;
                case 4:
                    
                    extraBytes = PK4.ExtraBytes;
                    break;
                case 5:
                    extraBytes = PK5.ExtraBytes;
                    break;
                case 6:
                    extraBytes = PK6.ExtraBytes;
                    TB_GameSync.Enabled = (SAV as SAV6).GameSyncID != 0;
                    TB_GameSync.Text = (SAV as SAV6).GameSyncID.ToString("X16");
                    TB_Secure1.Text = (SAV as SAV6).Secure1.ToString("X16");
                    TB_Secure2.Text = (SAV as SAV6).Secure2.ToString("X16");
                    break;
            }
            PKX.Personal = SAV.Personal;

            PKM pk = preparePKM();
            populateFilteredDataSources();
            populateFields(pkm.Format != SAV.Generation ? SAV.BlankPKM : pk);

            // SAV Specific Limits
            TB_OT.MaxLength = SAV.OTLength;
            TB_OTt2.MaxLength = SAV.OTLength;
            TB_Nickname.MaxLength = SAV.NickLength;

            // Common HaX Interface
            CHK_HackedStats.Enabled = CHK_HackedStats.Visible = MT_Level.Enabled = MT_Level.Visible = MT_Form.Enabled = MT_Form.Visible = HaX;
            PB_Legal.Visible = PB_WarnMove1.Visible = PB_WarnMove2.Visible = PB_WarnMove3.Visible = PB_WarnMove4.Visible &= !HaX;
            TB_Level.Visible = !HaX;

            // Load Extra Byte List
            CB_ExtraBytes.Items.Clear();
            foreach (byte b in extraBytes)
                CB_ExtraBytes.Items.Add("0x" + b.ToString("X2"));
            CB_ExtraBytes.SelectedIndex = 0;

            // Refresh PK#->PK6 conversion info
            PKMConverter.updateConfig(SAV.SubRegion, SAV.Country, SAV.ConsoleRegion, SAV.OT, SAV.Gender);

            // If backup folder exists, save a backup.
            string backupName = Path.Combine(BackupPath, Util.CleanFileName(SAV.BAKName));
            if (SAV.Exportable && Directory.Exists(BackupPath) && !File.Exists(backupName))
                File.WriteAllBytes(backupName, SAV.BAK);

            // Indicate audibly the save is loaded
            SystemSounds.Beep.Play();
        }
        private static void refreshWC6DB()
        {
            List<WC6> wc6db = new List<WC6>();
            byte[] wc6bin = Properties.Resources.wc6;
            for (int i = 0; i < wc6bin.Length; i += WC6.Size)
                wc6db.Add(new WC6(wc6bin.Skip(i).Take(WC6.Size).ToArray()));
            byte[] wc6full = Properties.Resources.wc6full;
            for (int i = 0; i < wc6full.Length; i += WC6.SizeFull)
                wc6db.Add(new WC6(wc6full.Skip(i).Take(WC6.SizeFull).ToArray()));

            if (Directory.Exists(WC6DatabasePath))
                wc6db.AddRange(from file in Directory.GetFiles(WC6DatabasePath, "*", SearchOption.AllDirectories)
                    let fi = new FileInfo(file)
                    where new[] {".wc6full", ".wc6"}.Contains(fi.Extension) && new[] {WC6.Size, WC6.SizeFull}.Contains((int)fi.Length)
                    select new WC6(File.ReadAllBytes(file)));

            Legal.WC6DB = wc6db.Distinct().ToArray();
        }

        // Language Translation
        private void changeMainLanguage(object sender, EventArgs e)
        {
            PKM pk = SAV.getPKM((fieldsInitialized ? preparePKM() : pkm).Data);
            bool alreadyInit = fieldsInitialized;
            fieldsInitialized = false;
            Menu_Options.DropDown.Close();
            InitializeStrings();
            InitializeLanguage();
            Util.TranslateInterface(this, lang_val[CB_MainLanguage.SelectedIndex]); // Translate the UI to language.
            populateFields(pk); // put data back in form
            fieldsInitialized |= alreadyInit;
        }
        private void InitializeStrings()
        {
            if (CB_MainLanguage.SelectedIndex < 8)
                curlanguage = lang_val[CB_MainLanguage.SelectedIndex];

            string l = curlanguage;
            natures = Util.getStringList("natures", l);
            types = Util.getStringList("types", l);
            abilitylist = Util.getStringList("abilities", l);
            movelist = Util.getStringList("moves", l);
            itemlist = Util.getStringList("items", l);
            characteristics = Util.getStringList("character", l);
            specieslist = Util.getStringList("species", l);
            wallpapernames = Util.getStringList("wallpaper", l);
            encountertypelist = Util.getStringList("encountertype", l);
            gamelist = Util.getStringList("games", l);
            gamelanguages = Util.getNulledStringArray(Util.getStringList("languages"));

            balllist = new string[Legal.Items_Ball.Length];
            for (int i = 0; i < balllist.Length; i++)
                balllist[i] = itemlist[Legal.Items_Ball[i]];

            if (l != "pt" || (l == "pt" && !fieldsInitialized)) // load initial binaries
            {
                pokeblocks = Util.getStringList("pokeblock", l);
                forms = Util.getStringList("forms", l);
                memories = Util.getStringList("memories", l);
                genloc = Util.getStringList("genloc", l);
                trainingbags = Util.getStringList("trainingbag", l);
                trainingstage = Util.getStringList("supertraining", l);
                puffs = Util.getStringList("puff", l);
            }

            // Fix Item Names (Duplicate entries)
            itemlist[456] += " (HG/SS)"; // S.S. Ticket
            itemlist[736] += " (OR/AS)"; // S.S. Ticket
            itemlist[463] += " (DPPt)"; // Storage Key
            itemlist[734] += " (OR/AS)"; // Storage Key
            itemlist[478] += " (HG/SS)"; // Basement Key
            itemlist[478] += " (OR/AS)"; // Basement Key
            itemlist[621] += " (M)"; // Xtransceiver
            itemlist[626] += " (F)"; // Xtransceiver
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
            string[] ptransp = { "ポケシフター", "Poké Transfer", "Poké Fret", "Pokétrasporto", "Poképorter", "Pokétransfer", "포케시프터", "ポケシフター" };
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
            metXY_00000[106] += " (X/Y)";              // Pokémon League
            metXY_00000[202] += " (OR/AS)";            // Pokémon League
            metXY_00000[298] += " (OR/AS)";            // Victory Road
            metXY_30000[0] += " (NPC)";                // Anything from an NPC
            metXY_30000[1] += " (" + eggname + ")";    // Egg From Link Trade

            // Set the first entry of a met location to "" (nothing)
            // Fix (None) tags
            abilitylist[0] = itemlist[0] = movelist[0] = metXY_00000[0] = metBW2_00000[0] = metHGSS_00000[0] = "(" + itemlist[0] + ")";

            // Force an update to the met locations
            origintrack = "";

            // Update Legality Analysis strings
            LegalityAnalysis.movelist = movelist;

            if (fieldsInitialized)
                updateIVs(null, null); // Prompt an update for the characteristics
        }
        #endregion

        #region //// PKX WINDOW FUNCTIONS ////
        private void InitializeFields()
        {
            // Now that the ComboBoxes are ready, load the data.
            fieldsInitialized = true;
            pkm.RefreshChecksum();

            // Load Data
            populateFields(pkm);
            {
                CB_Species.SelectedValue = 493;
                CB_Move1.SelectedValue = 1;
                TB_OT.Text = "PKHeX";
                TB_TID.Text = 12345.ToString();
                TB_SID.Text = 54321.ToString();
                CB_GameOrigin.SelectedIndex = 0;
                int curlang = Array.IndexOf(lang_val, curlanguage);
                CB_Language.SelectedIndex = curlang > CB_Language.Items.Count - 1 ? 1 : curlang;
                CB_BoxSelect.SelectedIndex = 0;
                CB_Ball.SelectedIndex = 0;
                CB_Country.SelectedIndex = 0;
                CAL_MetDate.Value = CAL_EggDate.Value = DateTime.Today;
            }
        }
        private void InitializeLanguage()
        {
            ComboBox[] cbs =
            {
                CB_Country, CB_SubRegion, CB_3DSReg, CB_Language, CB_Ball, CB_HeldItem, CB_Species, DEV_Ability,
                CB_Nature, CB_EncounterType, CB_GameOrigin, CB_HPType
            };
            foreach (var cb in cbs) { cb.DisplayMember = "Text"; cb.ValueMember = "Value"; }

            // Set the various ComboBox DataSources up with their allowed entries
            setCountrySubRegion(CB_Country, "countries");
            CB_3DSReg.DataSource = Util.getUnsortedCBList("regions3ds");
            CB_Language.DataSource = Util.getUnsortedCBList("languages");
            int[] ball_nums = { 7, 576, 13, 492, 497, 14, 495, 493, 496, 494, 11, 498, 8, 6, 12, 15, 9, 5, 499, 10, 1, 16 };
            int[] ball_vals = { 7, 25, 13, 17, 22, 14, 20, 18, 21, 19, 11, 23, 8, 6, 12, 15, 9, 5, 24, 10, 1, 16 };
            BallDataSource = Util.getVariedCBList(itemlist, ball_nums, ball_vals);
            SpeciesDataSource = Util.getCBList(specieslist, null);
            NatureDataSource = Util.getCBList(natures, null);
            AbilityDataSource = Util.getCBList(abilitylist, null);
            VersionDataSource = Util.getCBList(gamelist, Legal.Games_6oras, Legal.Games_6xy, Legal.Games_5, Legal.Games_4, Legal.Games_4e, Legal.Games_4r, Legal.Games_3, Legal.Games_3e, Legal.Games_3r, Legal.Games_3s);

            MoveDataSource = Util.getCBList(movelist, null);

            CB_EncounterType.DataSource = Util.getCBList(encountertypelist, new[] { 0 }, Legal.Gen4EncounterTypes);
            CB_HPType.DataSource = Util.getCBList(types.Skip(1).Take(16).ToArray(), null);
            CB_Nature.DataSource = new BindingSource(NatureDataSource, null);

            populateFilteredDataSources();
        }
        private void populateFilteredDataSources()
        {
            ItemDataSource = Util.getCBList(itemlist, (HaX ? Enumerable.Range(0, SAV.MaxItemID) : SAV.HeldItems.Select(i => (int)i)).ToArray());
            CB_HeldItem.DataSource = new BindingSource(ItemDataSource.Where(i => i.Value <= SAV.MaxItemID), null);

            CB_Ball.DataSource = new BindingSource(BallDataSource.Where(b => b.Value <= SAV.MaxBallID), null);
            CB_Species.DataSource = new BindingSource(SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID), null);
            DEV_Ability.DataSource = new BindingSource(AbilityDataSource.Where(a => a.Value <= SAV.MaxAbilityID), null);
            CB_GameOrigin.DataSource = new BindingSource(VersionDataSource.Where(g => g.Value <= SAV.MaxGameID || SAV.Generation >= 3 && g.Value == 15), null);

            // Set the Move ComboBoxes too..
            foreach (ComboBox cb in new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4, CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 })
            {
                cb.DisplayMember = "Text"; cb.ValueMember = "Value";
                cb.DataSource = new BindingSource(MoveDataSource.Where(m => m.Value <= SAV.MaxMoveID), null);
            }
        }
        public void populateFields(PKM pk, bool focus = true)
        {
            if (pk == null) { Util.Error("Attempted to load a null file."); return; }

            if (pk.Format != SAV.Generation)
            { Util.Alert("Can't load future generation files."); return; }

            bool oldInit = fieldsInitialized;
            fieldsInitialized = fieldsLoaded = false;
            if (focus)
                Tab_Main.Focus();

            pkm = pk.Clone();
            if (fieldsInitialized & !pkm.ChecksumValid)
                Util.Alert("PKX File has an invalid checksum.");
            switch (pkm.Format)
            {
                case 3:
                    populateFieldsPK3(pkm as PK3);
                    break;
                case 4:
                    populateFieldsPK4(pkm as PK4);
                    break;
                case 5:
                    populateFieldsPK5(pkm as PK5);
                    break;
                case 6:
                    populateFieldsPK6(pkm as PK6);
                    break;
            }

            CB_EncounterType.Visible = Label_EncounterType.Visible = pkm.Gen4;
            fieldsInitialized = oldInit;
            updateIVs(null, null);
            updatePKRSInfected(null, null);
            updatePKRSCured(null, null);

            if (HaX)
            {
                MT_Level.Text = pkm.Stat_Level.ToString();
                MT_Form.Text = pkm.AltForm.ToString();
                if (pkm.Stat_HPMax != 0) // stats present
                {
                    Stat_HP.Text = pkm.Stat_HPCurrent.ToString();
                    Stat_ATK.Text = pkm.Stat_ATK.ToString();
                    Stat_DEF.Text = pkm.Stat_DEF.ToString();
                    Stat_SPA.Text = pkm.Stat_SPA.ToString();
                    Stat_SPD.Text = pkm.Stat_SPD.ToString();
                    Stat_SPE.Text = pkm.Stat_SPE.ToString();
                }
            }
            fieldsLoaded = true;

            // Set the Preview Box
            dragout.Image = pk.Sprite;
            updateLegality();
        }
        private void populateFieldsPK3(PK3 pk3)
        {
            // Do first
            pk3.Stat_Level = PKX.getLevel(pk3.Species, pk3.EXP);
            if (pk3.Stat_Level == 100)
                pk3.EXP = PKX.getEXP(pk3.Stat_Level, pk3.Species);

            CB_Species.SelectedValue = pk3.Species;
            TB_Level.Text = pk3.Stat_Level.ToString();
            TB_EXP.Text = pk3.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = pk3.FatefulEncounter;
            CHK_IsEgg.Checked = pk3.IsEgg;
            CHK_Nicknamed.Checked = pk3.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk3.OT_Gender];
            Label_OTGender.ForeColor = pk3.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = pk3.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk3.HeldItem;
            setAbilityList();
            DEV_Ability.SelectedValue = pk3.Ability;
            CB_Nature.SelectedValue = pk3.Nature;
            TB_TID.Text = pk3.TID.ToString("00000");
            TB_SID.Text = pk3.SID.ToString("00000");
            TB_Nickname.Text = pk3.Nickname;
            TB_OT.Text = pk3.OT_Name;
            TB_Friendship.Text = pk3.CurrentFriendship.ToString();
            GB_OT.BackgroundImage = null;
            CB_Language.SelectedValue = pk3.Language;
            CB_GameOrigin.SelectedValue = pk3.Version;
            CB_EncounterType.SelectedValue = pk3.Gen4 ? pk3.EncounterType : 0;
            CB_Ball.SelectedValue = pk3.Ball;
            
            CB_MetLocation.SelectedValue = pk3.Met_Location;

            TB_MetLevel.Text = pk3.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk3.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk3.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk3.PKRS_Strain;
            CHK_Cured.Checked = pk3.PKRS_Strain > 0 && pk3.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk3.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = pk3.CNT_Cool.ToString();
            TB_Beauty.Text = pk3.CNT_Beauty.ToString();
            TB_Cute.Text = pk3.CNT_Cute.ToString();
            TB_Smart.Text = pk3.CNT_Smart.ToString();
            TB_Tough.Text = pk3.CNT_Tough.ToString();
            TB_Sheen.Text = pk3.CNT_Sheen.ToString();

            TB_HPIV.Text = pk3.IV_HP.ToString();
            TB_ATKIV.Text = pk3.IV_ATK.ToString();
            TB_DEFIV.Text = pk3.IV_DEF.ToString();
            TB_SPEIV.Text = pk3.IV_SPE.ToString();
            TB_SPAIV.Text = pk3.IV_SPA.ToString();
            TB_SPDIV.Text = pk3.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk3.HPType;

            TB_HPEV.Text = pk3.EV_HP.ToString();
            TB_ATKEV.Text = pk3.EV_ATK.ToString();
            TB_DEFEV.Text = pk3.EV_DEF.ToString();
            TB_SPEEV.Text = pk3.EV_SPE.ToString();
            TB_SPAEV.Text = pk3.EV_SPA.ToString();
            TB_SPDEV.Text = pk3.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk3.Move1;
            CB_Move2.SelectedValue = pk3.Move2;
            CB_Move3.SelectedValue = pk3.Move3;
            CB_Move4.SelectedValue = pk3.Move4;
            CB_PPu1.SelectedIndex = pk3.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk3.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk3.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk3.Move4_PPUps;
            TB_PP1.Text = pk3.Move1_PP.ToString();
            TB_PP2.Text = pk3.Move2_PP.ToString();
            TB_PP3.Text = pk3.Move3_PP.ToString();
            TB_PP4.Text = pk3.Move4_PP.ToString();
            
            // Load Extrabyte Value
            TB_ExtraByte.Text = pk3.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();
            setIsShiny();

            TB_EXP.Text = pk3.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk3.Gender];
            Label_Gender.ForeColor = pk3.Gender == 2 ? Label_Species.ForeColor : (pk3.Gender == 1 ? Color.Red : Color.Blue);
        }
        private void populateFieldsPK4(PK4 pk4)
        {
            // Do first
            pk4.Stat_Level = PKX.getLevel(pk4.Species, pk4.EXP);
            if (pk4.Stat_Level == 100)
                pk4.EXP = PKX.getEXP(pk4.Stat_Level, pk4.Species);

            CB_Species.SelectedValue = pk4.Species;
            TB_Level.Text = pk4.Stat_Level.ToString();
            TB_EXP.Text = pk4.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = pk4.FatefulEncounter;
            CHK_IsEgg.Checked = pk4.IsEgg;
            CHK_Nicknamed.Checked = pk4.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk4.OT_Gender];
            Label_OTGender.ForeColor = pk4.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = pk4.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk4.HeldItem;
            setAbilityList();
            DEV_Ability.SelectedValue = pk4.Ability;
            CB_Nature.SelectedValue = pk4.Nature;
            TB_TID.Text = pk4.TID.ToString("00000");
            TB_SID.Text = pk4.SID.ToString("00000");
            TB_Nickname.Text = pk4.Nickname;
            TB_OT.Text = pk4.OT_Name;
            TB_Friendship.Text = pk4.CurrentFriendship.ToString();
            GB_OT.BackgroundImage = null;
            CB_Language.SelectedValue = pk4.Language;
            CB_GameOrigin.SelectedValue = pk4.Version;
            CB_EncounterType.SelectedValue = pk4.Gen4 ? pk4.EncounterType : 0;
            CB_Ball.SelectedValue = pk4.Ball;

            if (pk4.Met_Month == 0) { pk4.Met_Month = 1; }
            if (pk4.Met_Day == 0) { pk4.Met_Day = 1; }
            try { CAL_MetDate.Value = new DateTime(pk4.Met_Year + 2000, pk4.Met_Month, pk4.Met_Day); }
            catch { CAL_MetDate.Value = new DateTime(2000, 1, 1); }

            if (pk4.Egg_Location != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = pk4.Egg_Location;
                try { CAL_EggDate.Value = new DateTime(pk4.Egg_Year + 2000, pk4.Egg_Month, pk4.Egg_Day); }
                catch { CAL_MetDate.Value = new DateTime(2000, 1, 1); }
            }
            else { CAL_EggDate.Value = new DateTime(2000, 01, 01); CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = pk4.Met_Location;

            TB_MetLevel.Text = pk4.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk4.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk4.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk4.PKRS_Strain;
            CHK_Cured.Checked = pk4.PKRS_Strain > 0 && pk4.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk4.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = pk4.CNT_Cool.ToString();
            TB_Beauty.Text = pk4.CNT_Beauty.ToString();
            TB_Cute.Text = pk4.CNT_Cute.ToString();
            TB_Smart.Text = pk4.CNT_Smart.ToString();
            TB_Tough.Text = pk4.CNT_Tough.ToString();
            TB_Sheen.Text = pk4.CNT_Sheen.ToString();

            TB_HPIV.Text = pk4.IV_HP.ToString();
            TB_ATKIV.Text = pk4.IV_ATK.ToString();
            TB_DEFIV.Text = pk4.IV_DEF.ToString();
            TB_SPEIV.Text = pk4.IV_SPE.ToString();
            TB_SPAIV.Text = pk4.IV_SPA.ToString();
            TB_SPDIV.Text = pk4.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk4.HPType;

            TB_HPEV.Text = pk4.EV_HP.ToString();
            TB_ATKEV.Text = pk4.EV_ATK.ToString();
            TB_DEFEV.Text = pk4.EV_DEF.ToString();
            TB_SPEEV.Text = pk4.EV_SPE.ToString();
            TB_SPAEV.Text = pk4.EV_SPA.ToString();
            TB_SPDEV.Text = pk4.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk4.Move1;
            CB_Move2.SelectedValue = pk4.Move2;
            CB_Move3.SelectedValue = pk4.Move3;
            CB_Move4.SelectedValue = pk4.Move4;
            CB_PPu1.SelectedIndex = pk4.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk4.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk4.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk4.Move4_PPUps;
            TB_PP1.Text = pk4.Move1_PP.ToString();
            TB_PP2.Text = pk4.Move2_PP.ToString();
            TB_PP3.Text = pk4.Move3_PP.ToString();
            TB_PP4.Text = pk4.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk4.AltForm ? pk4.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk4.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();
            setIsShiny();
            
            TB_EXP.Text = pk4.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk4.Gender];
            Label_Gender.ForeColor = pk4.Gender == 2 ? Label_Species.ForeColor : (pk4.Gender == 1 ? Color.Red : Color.Blue);

            if (HaX)
                DEV_Ability.SelectedValue = pk4.Ability;
        }
        private void populateFieldsPK5(PK5 pk5)
        {
            // Do first
            pk5.Stat_Level = PKX.getLevel(pk5.Species, pk5.EXP);
            if (pk5.Stat_Level == 100)
                pk5.EXP = PKX.getEXP(pk5.Stat_Level, pk5.Species);

            CB_Species.SelectedValue = pk5.Species;
            TB_Level.Text = pk5.Stat_Level.ToString();
            TB_EXP.Text = pk5.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = pk5.FatefulEncounter;
            CHK_IsEgg.Checked = pk5.IsEgg;
            CHK_Nicknamed.Checked = pk5.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk5.OT_Gender];
            Label_OTGender.ForeColor = pk5.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = pk5.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk5.HeldItem;
            setAbilityList();
            DEV_Ability.SelectedValue = pk5.Ability;
            CB_Nature.SelectedValue = pk5.Nature;
            TB_TID.Text = pk5.TID.ToString("00000");
            TB_SID.Text = pk5.SID.ToString("00000");
            TB_Nickname.Text = pk5.Nickname;
            TB_OT.Text = pk5.OT_Name;
            TB_Friendship.Text = pk5.CurrentFriendship.ToString();
            if (pk5.CurrentHandler == 1)  // HT
            {
                GB_nOT.BackgroundImage = mixedHighlight;
                GB_OT.BackgroundImage = null;
            }
            else                  // = 0
            {
                GB_OT.BackgroundImage = mixedHighlight;
                GB_nOT.BackgroundImage = null;
            }
            CB_Language.SelectedValue = pk5.Language;
            CB_GameOrigin.SelectedValue = pk5.Version;
            CB_EncounterType.SelectedValue = pk5.Gen4 ? pk5.EncounterType : 0;
            CB_Ball.SelectedValue = pk5.Ball;

            if (pk5.Met_Month == 0) { pk5.Met_Month = 1; }
            if (pk5.Met_Day == 0) { pk5.Met_Day = 1; }
            try { CAL_MetDate.Value = new DateTime(pk5.Met_Year + 2000, pk5.Met_Month, pk5.Met_Day); }
            catch { CAL_MetDate.Value = new DateTime(2000, 1, 1); }

            if (pk5.Egg_Location != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = pk5.Egg_Location;
                try { CAL_EggDate.Value = new DateTime(pk5.Egg_Year + 2000, pk5.Egg_Month, pk5.Egg_Day); }
                catch { CAL_MetDate.Value = new DateTime(2000, 1, 1); }
            }
            else { CAL_EggDate.Value = new DateTime(2000, 01, 01); CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = pk5.Met_Location;
            
            TB_MetLevel.Text = pk5.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk5.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk5.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk5.PKRS_Strain;
            CHK_Cured.Checked = pk5.PKRS_Strain > 0 && pk5.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk5.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = pk5.CNT_Cool.ToString();
            TB_Beauty.Text = pk5.CNT_Beauty.ToString();
            TB_Cute.Text = pk5.CNT_Cute.ToString();
            TB_Smart.Text = pk5.CNT_Smart.ToString();
            TB_Tough.Text = pk5.CNT_Tough.ToString();
            TB_Sheen.Text = pk5.CNT_Sheen.ToString();

            TB_HPIV.Text = pk5.IV_HP.ToString();
            TB_ATKIV.Text = pk5.IV_ATK.ToString();
            TB_DEFIV.Text = pk5.IV_DEF.ToString();
            TB_SPEIV.Text = pk5.IV_SPE.ToString();
            TB_SPAIV.Text = pk5.IV_SPA.ToString();
            TB_SPDIV.Text = pk5.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk5.HPType;

            TB_HPEV.Text = pk5.EV_HP.ToString();
            TB_ATKEV.Text = pk5.EV_ATK.ToString();
            TB_DEFEV.Text = pk5.EV_DEF.ToString();
            TB_SPEEV.Text = pk5.EV_SPE.ToString();
            TB_SPAEV.Text = pk5.EV_SPA.ToString();
            TB_SPDEV.Text = pk5.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk5.Move1;
            CB_Move2.SelectedValue = pk5.Move2;
            CB_Move3.SelectedValue = pk5.Move3;
            CB_Move4.SelectedValue = pk5.Move4;
            CB_PPu1.SelectedIndex = pk5.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk5.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk5.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk5.Move4_PPUps;
            TB_PP1.Text = pk5.Move1_PP.ToString();
            TB_PP2.Text = pk5.Move2_PP.ToString();
            TB_PP3.Text = pk5.Move3_PP.ToString();
            TB_PP4.Text = pk5.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk5.AltForm ? pk5.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk5.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();
            setIsShiny();
            
            TB_EXP.Text = pk5.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk5.Gender];
            Label_Gender.ForeColor = pk5.Gender == 2 ? Label_Species.ForeColor : (pk5.Gender == 1 ? Color.Red : Color.Blue);

            if (HaX)
                DEV_Ability.SelectedValue = pk5.Ability;
        }
        private void populateFieldsPK6(PK6 pk6)
        {
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
            TB_PID.Text = pk6.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk6.HeldItem;
            setAbilityList();
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
            else { CAL_EggDate.Value = new DateTime(2000, 01, 01); CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = pk6.Met_Location;

            // Set CT Gender to None if no CT, else set to gender symbol.
            Label_CTGender.Text = pk6.HT_Name == "" ? "" : gendersymbols[pk6.HT_Gender % 2];
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

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk6.AltForm ? pk6.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk6.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();
            setIsShiny();
            
            TB_EXP.Text = pk6.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk6.Gender];
            Label_Gender.ForeColor = pk6.Gender == 2 ? Label_Species.ForeColor : (pk6.Gender == 1 ? Color.Red : Color.Blue);
            
            // Highlight the Current Handler
            clickGT(pk6.CurrentHandler == 1 ? GB_nOT : GB_OT, null);

            if (HaX)
                DEV_Ability.SelectedValue = pk6.Ability;
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
        private void setForms()
        {
            if (SAV.Generation < 4)
            {
                Label_Form.Visible = CB_Form.Visible = CB_Form.Enabled = false;
                return;
            }

            int species = Util.getIndex(CB_Species);
            bool hasForms = SAV.Personal[species].HasFormes || new[] { 664, 665, 414 }.Contains(species);
            CB_Form.Enabled = CB_Form.Visible = Label_Form.Visible = hasForms;

            if (!hasForms)
                return;

            CB_Form.DataSource = PKX.getFormList(species, types, forms, gendersymbols).ToList();
        }
        private void setAbilityList()
        {
            if (SAV.Generation < 3) // no abilities
                return;

            int formnum = 0;
            int species = Util.getIndex(CB_Species);
            if (SAV.Generation > 3) // has forms
                formnum = CB_Form.SelectedIndex;

            byte[] abils = SAV.Personal[SAV.Personal[species].FormeIndex(species, formnum)].Abilities;

            List<string> ability_list = new List<string>
            {
                abilitylist[abils[0]] + " (1)",
                abilitylist[abils[1]] + " (2)",
            };
            if (SAV.Generation >= 5) // hidden ability
                ability_list.Add(abilitylist[abils[1]] + " (H)");

            int curAbil = CB_Ability.SelectedIndex;
            CB_Ability.DataSource = ability_list;
            CB_Ability.SelectedIndex = curAbil;
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
            for (int i = 0; i < 6; i++)
                pba[i].Image = Util.ChangeOpacity(pba[i].InitialImage, (pkm.Markings[i] ? 1 : 0) * 0.9 + 0.1);

            PB_MarkShiny.Image = Util.ChangeOpacity(PB_MarkShiny.InitialImage, (!BTN_Shinytize.Enabled ? 1 : 0) * 0.9 + 0.1);
            PB_MarkCured.Image = Util.ChangeOpacity(PB_MarkCured.InitialImage, (CHK_Cured.Checked ? 1 : 0) * 0.9 + 0.1);

            int Version = Util.getIndex(CB_GameOrigin); // 24,25 = XY, 26,27 = ORAS, 28,29 = ???
            PB_MarkPentagon.Image = Util.ChangeOpacity(PB_MarkPentagon.InitialImage, (Version >= 24 && Version <= 29 ? 1 : 0) * 0.9 + 0.1);
        }
        // Clicked Label Shortcuts //
        private void clickQR(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
            {
                Util.Alert("QR feature only available for 6th Gen Games.");
                return;
            }
            if (ModifierKeys == Keys.Alt)
            {
                // Fetch data from QR code...
                byte[] ekx = QR.getQRData();

                if (ekx == null) return;

                if (ekx.Length != PKX.SIZE_6STORED) { Util.Alert($"Decoded data not {PKX.SIZE_6STORED} bytes.", $"QR Data Size: {ekx.Length}"); }
                else try
                    {
                        PK6 pk = new PK6(PKX.decryptArray(ekx));
                        if (pk.ChecksumValid) { populateFields(pk); }
                        else Util.Alert("Invalid checksum in QR data.");
                    }
                    catch { Util.Alert("Error loading decrypted data."); }
            }
            else
            {
                if (!verifiedPKM()) return;
                PKM pkx = preparePKM();
                byte[] ekx = pkx.EncryptedBoxData;
                const string server = "http://loadcode.projectpokemon.org/b1s1.html#"; // Rehosted with permission from LC/MS -- massive thanks!
                Image qr = QR.getQRImage(ekx, server);

                if (qr == null) return;

                string[] r = pkx.QRText;
                new QR(qr, dragout.Image, r[0], r[1], r[2], "PKHeX @ ProjectPokemon.org").ShowDialog();
            }
        }
        private void clickFriendship(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // prompt to reset
                TB_Friendship.Text = pkm.CurrentFriendship.ToString();
            else
                TB_Friendship.Text = TB_Friendship.Text == "255" ? PKX.getBaseFriendship(pkm.Species).ToString() : "255";
        }
        private void clickGender(object sender, EventArgs e)
        {
            // Get Gender Threshold
            int gt = PKX.Personal[Util.getIndex(CB_Species)].Gender;

            if (gt == 255 || gt == 0 || gt == 254) // Single gender/genderless
                return;

            if (gt >= 255) return; 
            // If not a single gender(less) species: (should be <254 but whatever, 255 never happens)

            pkm.Gender = PKX.getGender(Label_Gender.Text) ^ 1;
            Label_Gender.Text = gendersymbols[pkm.Gender];
            Label_Gender.ForeColor = pkm.Gender == 2 ? Label_Species.ForeColor : (pkm.Gender == 1 ? Color.Red : Color.Blue);

            if (SAV.Generation < 6)
                updateRandomPID(null, null);

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
            int index = Array.IndexOf(pba, sender);
            bool[] markings = pkm.Markings;
            markings[index] ^= true;
            pkm.Markings = markings;
            setMarkings();
        }
        private void clickStatLabel(object sender, MouseEventArgs e)
        {
            if (!(ModifierKeys == Keys.Control || ModifierKeys == Keys.Alt))
                return;

            int index = Array.IndexOf(new[] { Label_HP, Label_ATK, Label_DEF, Label_SPA, Label_SPD, Label_SPE }, sender);

            if (ModifierKeys == Keys.Alt) // EV
            {
                var arrayEV = new[] {TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPAEV, TB_SPDEV, TB_SPEEV};
                arrayEV[index].Text = (e.Button == MouseButtons.Left
                    ? Math.Min(Math.Max(510 - Util.ToInt32(TB_EVTotal.Text) + Util.ToInt32(arrayEV[index].Text), 0), 252) 
                    : 0).ToString();
            }
            else
            new[] {TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPAIV, TB_SPDIV, TB_SPEIV}[index].Text =
                (e.Button == MouseButtons.Left ? 31 : 0).ToString();
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
            if (!SAV.Exportable)
                return;

            // Get Save Information
            TB_OT.Text = SAV.OT;
            Label_OTGender.Text = gendersymbols[SAV.Gender % 2];
            Label_OTGender.ForeColor = SAV.Gender == 1 ? Color.Red : Color.Blue;
            TB_TID.Text = SAV.TID.ToString();
            TB_SID.Text = SAV.SID.ToString();

            if (SAV.Game >= 0)
                CB_GameOrigin.SelectedValue = SAV.Game;
            if (SAV.Language >= 0)
                CB_Language.SelectedValue = SAV.Language;
            if (SAV.HasGeolocation)
            {
                CB_SubRegion.SelectedValue = SAV.SubRegion;
                CB_Country.SelectedValue = SAV.Country;
                CB_3DSReg.SelectedValue = SAV.ConsoleRegion;
            }
            updateNickname(null, null);
        }
        private void clickCT(object sender, EventArgs e)
        {
            if (TB_OTt2.Text.Length > 0)
                Label_CTGender.Text = gendersymbols[SAV.Gender % 2];
        }
        private void clickGT(object sender, EventArgs e)
        {
            if (!GB_nOT.Visible)
                return;
            if (sender == GB_OT)
            {
                pkm.CurrentHandler = 0;
                GB_OT.BackgroundImage = mixedHighlight;
                GB_nOT.BackgroundImage = null;
            }
            else if (TB_OTt2.Text.Length > 0)
            {
                pkm.CurrentHandler = 1;
                GB_OT.BackgroundImage = null;
                GB_nOT.BackgroundImage = mixedHighlight;
            }
            TB_Friendship.Text = pkm.CurrentFriendship.ToString();
        }
        private void clickTRGender(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (!string.IsNullOrWhiteSpace(lbl?.Text)) // set gender label (toggle M/F)
            {
                int gender = PKX.getGender(lbl.Text) ^ 1;
                lbl.Text = gendersymbols[gender];
                lbl.ForeColor = gender == 1 ? Color.Red : Color.Blue;
            }
        }
        private void clickMoves(object sender, EventArgs e)
        {
            updateLegality();
            int[] m = Legality.getSuggestedRelearn();
            string r = string.Join(Environment.NewLine, m.Select(v => v >= movelist.Length ? "ERROR" : movelist[v]));
            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Apply suggested relearn moves?", r))
                return;

            CB_RelearnMove1.SelectedValue = m[0];
            CB_RelearnMove2.SelectedValue = m[1];
            CB_RelearnMove3.SelectedValue = m[2];
            CB_RelearnMove4.SelectedValue = m[3];
            updateLegality();
        }
        // Prompted Updates of PKX Functions // 
        private bool changingFields;
        private void updateEXPLevel(object sender, EventArgs e)
        {
            if (changingFields) return;

            changingFields = true;
            if (sender == TB_EXP)
            {
                // Change the Level
                uint EXP = Util.ToUInt32(TB_EXP.Text);
                int Species = Util.getIndex(CB_Species);
                int Level = PKX.getLevel(Species, EXP);
                if (Level == 100)
                    EXP = PKX.getEXP(100, Species);

                TB_Level.Text = Level.ToString();
                if (!MT_Level.Visible)
                    TB_EXP.Text = EXP.ToString();
                else
                    MT_Level.Text = Level.ToString();
            }
            else
            {
                // Change the XP
                int Level = Util.ToInt32((MT_Level.Focused ? MT_Level : TB_Level).Text);
                if (Level > 100) TB_Level.Text = "100";
                if (Level > byte.MaxValue) MT_Level.Text = "255";

                TB_EXP.Text = PKX.getEXP(Level, Util.getIndex(CB_Species)).ToString();
            }
            changingFields = false;
            updateStats();
            updateLegality();
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
            if (sender != null && Util.ToInt32((sender as MaskedTextBox).Text) > 31)
                (sender as MaskedTextBox).Text = "31";

            changingFields = true;

            // Update IVs
            pkm.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pkm.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pkm.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pkm.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pkm.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pkm.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
                    
            CB_HPType.SelectedValue = pkm.HPType;
            changingFields = false;

            // Potential Reading
            L_Potential.Text = (!unicode
                ? new[] {"★☆☆☆", "★★☆☆", "★★★☆", "★★★★"}
                : new[] {"+", "++", "+++", "++++"}
                )[pkm.PotentialRating];

            TB_IVTotal.Text = pkm.IVs.Sum().ToString();

            int characteristic = pkm.Characteristic;
            L_Characteristic.Visible = Label_CharacteristicPrefix.Visible = characteristic > -1;
            if (characteristic > -1)
                L_Characteristic.Text = characteristics[pkm.Characteristic];
            updateStats();
        }
        private void updateEVs(object sender, EventArgs e)
        {
            if (sender != null)
                if (Util.ToInt32((sender as MaskedTextBox).Text) > SAV.MaxEV)
                    (sender as MaskedTextBox).Text = SAV.MaxEV.ToString();

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
            else TB_EVTotal.BackColor = TB_IVTotal.BackColor;

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
            updateIVs(null, null);
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
            int origin = Util.getIndex(CB_GameOrigin);
            uint PID = PKX.getRandomPID(Util.getIndex(CB_Species), PKX.getGender(Label_Gender.Text), origin, Util.getIndex(CB_Nature), CB_Form.SelectedIndex);
            TB_PID.Text = PID.ToString("X8");
            getQuickFiller(dragout);
            if (origin >= 24)
                return;

            // Before Gen6, EC and PID are related
            // Ensure we don't have an illegal newshiny PID.
            uint SID = Util.ToUInt32(TB_TID.Text);
            uint TID = Util.ToUInt32(TB_TID.Text);
            uint XOR = TID ^ SID ^ PID >> 16 ^ PID & 0xFFFF;
            if (XOR >> 3 == 1) // Illegal
                updateRandomPID(sender, e); // Get a new PID

            TB_EC.Text = PID.ToString("X8");
        }
        private void updateRandomEC(object sender, EventArgs e)
        {
            int origin = Util.getIndex(CB_GameOrigin);
            if (origin < 24)
            {
                TB_EC.Text = TB_PID.Text;
                Util.Alert("EC should match PID.");
            }
            
            int wIndex = Array.IndexOf(Legal.WurmpleFamily, Util.getIndex(CB_Species));
            if (wIndex < 0)
            {
                TB_EC.Text = Util.rnd32().ToString("X8");
            }
            else
            {
                uint EC;
                do { EC = Util.rnd32(); } while ((EC >> 16)%10/5 != wIndex/2);
                TB_EC.Text = EC.ToString("X8");
            }
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
            if (string.IsNullOrWhiteSpace(text))
                ((TextBox)sender).Text = "0";

            if (Convert.ToUInt32(text) > ushort.MaxValue)
                ((TextBox)sender).Text = "65535";
        }
        private void update255_MTB(object sender, EventArgs e)
        {
            if (Util.ToInt32((sender as MaskedTextBox).Text) > byte.MaxValue)
                    (sender as MaskedTextBox).Text = "255";
        }
        private void updateForm(object sender, EventArgs e)
        {
            updateStats();
            // Repopulate Abilities if Species Form has different abilities
            setAbilityList();

            // Gender Forms
            if (PKX.getGender(CB_Form.Text) < 2 && Util.getIndex(CB_Species) != 201) // don't do this for Unown
                Label_Gender.Text = CB_Form.Text;

            if (changingFields) 
                return;
            changingFields = true;
            MT_Form.Text = CB_Form.SelectedIndex.ToString();
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
            setForms();

            // Recalculate EXP for Given Level
            uint EXP = PKX.getEXP(Level, Species);
            TB_EXP.Text = EXP.ToString();

            // Check for Gender Changes
            // Get Gender Threshold
            int gt = PKX.Personal[Species].Gender;
            int cg = Array.IndexOf(gendersymbols, Label_Gender.Text);
            int Gender;

            if (gt == 255)      // Genderless
                Gender = 2;
            else if (gt == 254) // Female Only
                Gender = 1;
            else if (gt == 0)  // Male Only
                Gender = 0;
            else if (cg == 2 || Util.getIndex(CB_GameOrigin) < 24)
                Gender = (Util.getHEXval(TB_PID.Text) & 0xFF) <= gt ? 1 : 0;
            else
                Gender = cg;
            
            Label_Gender.Text = gendersymbols[Gender];
            Label_Gender.ForeColor = Gender == 2 ? Label_Species.ForeColor : (Gender == 1 ? Color.Red : Color.Blue);
            setAbilityList();
            updateForm(null, null);

            // If species changes and no nickname, set the new name == speciesName.
            if (!CHK_Nicknamed.Checked)
                updateNickname(sender, e);

            updateLegality();
        }
        private void updateOriginGame(object sender, EventArgs e)
        {
            int Version = Util.getIndex(CB_GameOrigin);

            if (Version < 24 && origintrack != "Past" && SAV.Generation != 4)
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
            else if (Version > 23 && origintrack != "XY")
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
            else if (SAV.Generation == 4 && origintrack != "Gen4")
            {
                var met_list = Util.getCBList(metHGSS_00000, new[] { 0 });
                met_list = Util.getOffsetCBList(met_list, metHGSS_02000, 2000, new[] { 2000 });
                met_list = Util.getOffsetCBList(met_list, metHGSS_02000, 2000, new[] { 2002 });
                met_list = Util.getOffsetCBList(met_list, metHGSS_03000, 3000, new[] { 3001 });
                met_list = Util.getOffsetCBList(met_list, metHGSS_00000, 0000, Legal.Met_HGSS_0);
                met_list = Util.getOffsetCBList(met_list, metHGSS_02000, 2000, Legal.Met_HGSS_2);
                met_list = Util.getOffsetCBList(met_list, metHGSS_03000, 3000, Legal.Met_HGSS_3);
                CB_MetLocation.DisplayMember = "Text";
                CB_MetLocation.ValueMember = "Value";
                CB_MetLocation.DataSource = met_list;
                CB_MetLocation.SelectedValue = 0;
                origintrack = "Gen4";
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
            bool g4 = Version >= 7 && Version <= 12 && Version != 9;
            CB_EncounterType.Visible = Label_EncounterType.Visible = g4;
            if (!g4)
                CB_EncounterType.SelectedValue = 0;

            setMarkings(); // Set/Remove KB marking
            if (!fieldsLoaded)
                return;
            pkm.Version = Version;
            updateLegality();
        }
        private void updateExtraByteValue(object sender, EventArgs e)
        {
            if (CB_ExtraBytes.Items.Count == 0)
                return;
            // Changed Extra Byte's Value
            if (Util.ToInt32((sender as MaskedTextBox).Text) > byte.MaxValue)
                (sender as MaskedTextBox).Text = "255";

            int value = Util.ToInt32(TB_ExtraByte.Text);
            int offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
            pkm.Data[offset] = (byte)value;
        }
        private void updateExtraByteIndex(object sender, EventArgs e)
        {
            if (CB_ExtraBytes.Items.Count == 0)
                return;
            // Byte changed, need to refresh the Text box for the byte's value.
            TB_ExtraByte.Text = pkm.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
        }
        private void updateNatureModification(object sender, EventArgs e)
        {
            if (sender != CB_Nature) return;
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
            { clickShowdownImportPK6(sender, e); return; }
            if (fieldsInitialized && ModifierKeys == Keys.Alt && sender != null) // Export Showdown
            { clickShowdownExportPK6(sender, e); return; }
            if (!fieldsInitialized || CHK_Nicknamed.Checked)
                return;

            // Fetch Current Species and set it as Nickname Text
            int species = Util.getIndex(CB_Species);
            if (species < 1 || species > SAV.MaxSpeciesID)
                TB_Nickname.Text = "";
            else
            {
                // get language
                int lang = Util.getIndex(CB_Language);
                if (CHK_IsEgg.Checked) species = 0; // Set species to 0 to get the egg name.
                string nick = PKX.getSpeciesName(CHK_IsEgg.Checked ? 0 : species, lang);

                if (SAV.Generation < 5) // All caps GenIV and previous
                    nick = nick.ToUpper();
                TB_Nickname.Text = nick;
            }
        }
        private void updateNicknameClick(object sender, MouseEventArgs e)
        {
            TextBox tb = !(sender is TextBox) ? TB_Nickname : sender as TextBox;
            // Special Character Form
            if (ModifierKeys != Keys.Control)
                return;

            var z = Application.OpenForms.Cast<Form>().FirstOrDefault(form => form.GetType() == typeof(f2_Text)) as f2_Text;
            if (z != null)
            { Util.CenterToForm(z, this); z.BringToFront(); return; }
            new f2_Text(tb).Show();
        }
        private void updateNotOT(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TB_OTt2.Text))
            {
                clickGT(GB_OT, null); // Switch CT over to OT.
                Label_CTGender.Text = "";
                TB_Friendship.Text = pkm.CurrentFriendship.ToString();
            }
            else if (string.IsNullOrWhiteSpace(Label_CTGender.Text))
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

            updateLegality();
        }
        private void updateShinyPID(object sender, EventArgs e)
        {
            uint TID = Util.ToUInt32(TB_TID.Text);
            uint SID = Util.ToUInt32(TB_SID.Text);
            uint PID = Util.getHEXval(TB_PID.Text);
            if (SAV.Generation == 4)
            {
                int nature = Util.getIndex(CB_Nature);
                int species = Util.getIndex(CB_Species);
                int gender = PKX.getGender(Label_Gender.Text);
                
                uint oldbits = PID & 0x00010001;
                while ((TID ^ SID ^ (PID >> 16) ^ PID & 0xFFFF) > 8 || (PID & 0x00010001) != oldbits || PID%25 != nature || gender != PKX.getGender(species, PID))
                    PID = Util.rnd32();
                TB_PID.Text = PID.ToString("X8");

                getQuickFiller(dragout);
                return;
            }

            uint UID = PID >> 16;
            uint LID = PID & 0xFFFF;
            uint PSV = UID ^ LID;
            uint TSV = TID ^ SID;
            uint XOR = TSV ^ PSV;

            // Preserve Gen5 Origin Ability bit just in case
            XOR &= 0xFFFE; XOR |= UID & 1;

            // New XOR should be 0 or 1.
            TB_PID.Text = (((UID ^ XOR) << 16) + LID).ToString("X8");
            if (Util.getIndex(CB_GameOrigin) < 24) // Pre Gen6
                TB_EC.Text = TB_PID.Text;

            getQuickFiller(dragout);
        }
        private void updateTSV(object sender, EventArgs e)
        {
            if (SAV.Generation < 6)
                return;
            ushort TID = (ushort)Util.ToUInt32(TB_TID.Text);
            ushort SID = (ushort)Util.ToUInt32(TB_SID.Text);
            uint TSV = PKX.getTSV(TID, SID);
            Tip1.SetToolTip(TB_TID, "TSV: " + TSV.ToString("0000"));
            Tip2.SetToolTip(TB_SID, "TSV: " + TSV.ToString("0000"));

            uint PSV = PKX.getPSV(Util.getHEXval(TB_PID.Text));
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
            updateIVs(null, null);   // If the EC is changed, EC%6 (Characteristic) might be changed. 
            TB_PID.Select(60, 0);   // position cursor at end of field
            if (SAV.Generation == 4 && fieldsInitialized)
            {
                pkm.PID = Util.getHEXval(TB_PID.Text);
                CB_Nature.SelectedValue = pkm.Nature;
            }
        }
        private void validateComboBox(object sender)
        {
            if (!formInitialized)
                return;
            ComboBox cb = sender as ComboBox;
            if (cb == null) 
                return;
            
            cb.SelectionLength = 0;
            if (cb.Text == "")
            { cb.SelectedIndex = 0; return; }
            cb.BackColor = cb.SelectedValue == null ? Color.DarkSalmon : defaultControlWhite;
        }
        private void validateComboBox(object sender, EventArgs e)
        {
            if (!(sender is ComboBox))
                return;

            validateComboBox(sender);

            if (fieldsLoaded)
                getQuickFiller(dragout);
        }
        private void validateComboBox2(object sender, EventArgs e)
        {
            validateComboBox(sender, e);
            if (sender == CB_Ability)
                TB_AbilityNumber.Text = (1 << CB_Ability.SelectedIndex).ToString();
            if (fieldsInitialized && sender == CB_Nature && SAV.Generation == 4)
                BTN_RerollPID.PerformClick();
            updateNatureModification(sender, null);
            updateIVs(null, null); // updating Nature will trigger stats to update as well
        }
        private void validateMove(object sender, EventArgs e)
        {
            validateComboBox(sender);
            if (!fieldsLoaded)
                return;

            if (new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 }.Contains(sender)) // Move
                updatePP(sender, e);
            
            // Refresh Relearn if...
            if (new[] { CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 }.Contains(sender))
            {
                if (pkm.Format < 6)
                    return;
                (pkm as PK6).RelearnMoves = new[] { Util.getIndex(CB_RelearnMove1), Util.getIndex(CB_RelearnMove2), Util.getIndex(CB_RelearnMove3), Util.getIndex(CB_RelearnMove4) };
                Legality.updateRelearnLegality();
                for (int i = 0; i < 4; i++)
                    movePB[i].Visible = !Legality.vRelearn[i].Valid;
            }
            // else, Refresh Moves
            {
                pkm.Moves = new[] { Util.getIndex(CB_Move1), Util.getIndex(CB_Move2), Util.getIndex(CB_Move3), Util.getIndex(CB_Move4) };
                Legality.updateMoveLegality();
                for (int i = 0; i < 4; i++)
                    movePB[i].Visible = !Legality.vMoves[i].Valid;
            }
            if (relearnPB.Any(p => p.Visible) || movePB.Any(p => p.Visible))
            {
                Legality.Valid = false;
                PB_Legal.Image = Properties.Resources.warn;
            }
        }
        private void validateLocation(object sender, EventArgs e)
        {
            validateComboBox(sender);
            if (!fieldsLoaded)
                return;

            pkm.Met_Location = Util.getIndex(CB_MetLocation);
            pkm.Egg_Location = Util.getIndex(CB_EggLocation);
            updateLegality();
        }
        private void removedropCB(object sender, KeyEventArgs e)
        {
            ((ComboBox)sender).DroppedDown = false;
        }
        private void showLegality(PK6 pk, bool tabs, bool verbose)
        {
            LegalityAnalysis la = new LegalityAnalysis(pk);
            if (tabs)
                updateLegality(la);
            Util.Alert(verbose ? la.VerboseReport : la.Report);
        }
        private void updateLegality(LegalityAnalysis la = null)
        {
            if (!fieldsLoaded)
                return;
            if (!(pkm is PK6))
                return;
            Legality = la ?? new LegalityAnalysis(pkm as PK6);
            PB_Legal.Image = Legality.Valid ? Properties.Resources.valid : Properties.Resources.warn;
            PB_Legal.Visible = pkm.Gen6 /*&& pkm is PK6*/;

            // Refresh Move Legality
            for (int i = 0; i < 4; i++)
                movePB[i].Visible = !Legality.vMoves[i].Valid;
            for (int i = 0; i < 4; i++)
                relearnPB[i].Visible = !Legality.vRelearn[i].Valid;
        }
        private void updateStats()
        {
            // Gather the needed information.
            int species = Util.getIndex(CB_Species);
            int level = Util.ToInt32(MT_Level.Enabled ? MT_Level.Text : TB_Level.Text);
            if (level == 0) level = 1;
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
        private void updateUnicode()
        {
            if (!unicode)
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
        // Secondary Windows for Ribbons/Amie/Memories
        private void openRibbons(object sender, EventArgs e)
        {
            new RibbMedal().ShowDialog();
        }
        private void openHistory(object sender, EventArgs e)
        {
            if (pkm.Format < 6) return;
            // Write back current values
            PK6 pk6 = pkm as PK6;
            pk6.Version = Util.getIndex(CB_GameOrigin);
            pk6.HT_Name = TB_OTt2.Text;
            pk6.OT_Name = TB_OT.Text;
            pk6.IsEgg = CHK_IsEgg.Checked;
            pk6.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);
            new MemoryAmie().ShowDialog();
            TB_Friendship.Text = pk6.CurrentFriendship.ToString();
        }
        // Open/Save Array Manipulation //
        public bool verifiedPKM()
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
        private static string[] verifyPKMtoSAV(PKM pk)
        {
            // Check if PKM properties are outside of the valid range
            List<string> errata = new List<string>();
            if (pk.HeldItem > itemlist.Length)
                errata.Add($"Item Index beyond range: {pk.HeldItem}");
            else
            {
                if (pk.HeldItem > SAV.MaxItemID)
                    errata.Add($"Game can't obtain item: {itemlist[pk.HeldItem]}");
                if (!SAV.HeldItems.Contains((ushort)pk.HeldItem))
                    errata.Add($"Game can't hold item: {itemlist[pk.HeldItem]}");
            }

            if (pk.Species > specieslist.Length)
                errata.Add($"Species Index beyond range: {pk.HeldItem}");
            else if (SAV.MaxSpeciesID < pk.Species)
                errata.Add($"Game can't obtain species: {specieslist[pk.Species]}");

            if (pk.Moves.Any(m => m > movelist.Length))
                errata.Add($"Item Index beyond range: {string.Join(", ", pk.Moves.Where(m => m > movelist.Length).Select(m => m.ToString()))}");
            else if (pk.Moves.Any(m => m > SAV.MaxMoveID))
                errata.Add($"Game can't have move: {string.Join(", ", pk.Moves.Where(m => m > SAV.MaxMoveID).Select(m => movelist[m]))}");

            if (pk.Ability > abilitylist.Length)
                errata.Add($"Ability Index beyond range: {pk.Ability}");
            else if (pk.Ability > SAV.MaxAbilityID)
                errata.Add($"Game can't have ability: {abilitylist[pk.Ability]}");

            return errata.ToArray();
        }
        public PKM preparePKM(bool click = true)
        {
            if (click)
                tabMain.Select(); // hack to make sure comboboxes are set (users scrolling through and immediately setting causes this)

            PKM pk = null;
            switch (pkm.Format)
            {
                case 3:
                    pk = preparePK3();
                    break;
                case 4:
                    pk = preparePK4();
                    break;
                case 5:
                    pk = preparePK5();
                    break;
                case 6:
                    pk = preparePK6();
                    break;
            }
            return pk?.Clone();
        }
        private PK3 preparePK3()
        {
            PK3 pk3 = pkm as PK3;
            if (pk3 == null)
                return null;

            pk3.Species = Util.getIndex(CB_Species);
            pk3.G3Item = Util.getIndex(CB_HeldItem);
            pk3.TID = Util.ToInt32(TB_TID.Text);
            pk3.SID = Util.ToInt32(TB_SID.Text);
            pk3.EXP = Util.ToUInt32(TB_EXP.Text);
            pk3.PID = Util.getHEXval(TB_PID.Text);
            pk3.Ability = (byte)Util.getIndex(DEV_Ability);
            //pk4.Ability = (byte)Array.IndexOf(abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));
            
            pk3.FatefulEncounter = CHK_Fateful.Checked;
            pk3.Gender = PKX.getGender(Label_Gender.Text);
            pk3.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk3.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk3.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk3.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk3.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk3.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk3.CNT_Cool = Util.ToInt32(TB_Cool.Text);
            pk3.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            pk3.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            pk3.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            pk3.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            pk3.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            pk3.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk3.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            pk3.Nickname = TB_Nickname.Text;
            pk3.Move1 = Util.getIndex(CB_Move1);
            pk3.Move2 = Util.getIndex(CB_Move2);
            pk3.Move3 = Util.getIndex(CB_Move3);
            pk3.Move4 = Util.getIndex(CB_Move4);
            pk3.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk3.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk3.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk3.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk3.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk3.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk3.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk3.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk3.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk3.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk3.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk3.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk3.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk3.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk3.IsEgg = CHK_IsEgg.Checked;
            pk3.IsNicknamed = CHK_Nicknamed.Checked;

            pk3.OT_Name = TB_OT.Text;
            pk3.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            pk3.Ball = Util.getIndex(CB_Ball);
            pk3.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk3.OT_Gender = PKX.getGender(Label_OTGender.Text);
            pk3.Version = Util.getIndex(CB_GameOrigin);
            pk3.Language = Util.getIndex(CB_Language);
            
            pk3.Met_Location = Util.getIndex(CB_MetLocation);
            
            // Toss in Party Stats
            Array.Resize(ref pk3.Data, pk3.SIZE_PARTY);
            pk3.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk3.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk3.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk3.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk3.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk3.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk3.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk3.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk3.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk3.FixMoves();

            pk3.RefreshChecksum();
            return pk3;
        }
        private PK4 preparePK4()
        {
            PK4 pk4 = pkm as PK4;
            if (pk4 == null)
                return null;

            pk4.Species = Util.getIndex(CB_Species);
            pk4.HeldItem = Util.getIndex(CB_HeldItem);
            pk4.TID = Util.ToInt32(TB_TID.Text);
            pk4.SID = Util.ToInt32(TB_SID.Text);
            pk4.EXP = Util.ToUInt32(TB_EXP.Text);
            pk4.PID = Util.getHEXval(TB_PID.Text);
            pk4.Ability = (byte)Util.getIndex(DEV_Ability);
          //pk4.Ability = (byte)Array.IndexOf(abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));
          
            pk4.FatefulEncounter = CHK_Fateful.Checked;
            pk4.Gender = PKX.getGender(Label_Gender.Text);
            pk4.AltForm = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
            pk4.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk4.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk4.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk4.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk4.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk4.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk4.CNT_Cool = Util.ToInt32(TB_Cool.Text);
            pk4.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            pk4.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            pk4.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            pk4.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            pk4.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            pk4.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk4.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            pk4.Nickname = TB_Nickname.Text;
            pk4.Move1 = Util.getIndex(CB_Move1);
            pk4.Move2 = Util.getIndex(CB_Move2);
            pk4.Move3 = Util.getIndex(CB_Move3);
            pk4.Move4 = Util.getIndex(CB_Move4);
            pk4.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk4.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk4.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk4.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk4.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk4.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk4.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk4.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk4.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk4.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk4.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk4.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk4.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk4.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk4.IsEgg = CHK_IsEgg.Checked;
            pk4.IsNicknamed = CHK_Nicknamed.Checked;

            pk4.OT_Name = TB_OT.Text;
            pk4.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            pk4.Ball = Util.getIndex(CB_Ball);
            pk4.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk4.OT_Gender = PKX.getGender(Label_OTGender.Text);
            pk4.EncounterType = Util.getIndex(CB_EncounterType);
            pk4.Version = Util.getIndex(CB_GameOrigin);
            pk4.Language = Util.getIndex(CB_Language);

            // Default Dates
            int egg_year = 2000;                                   
            int egg_month = 0;
            int egg_day = 0;
            int egg_location = 0;
            if (CHK_AsEgg.Checked) // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_year = CAL_EggDate.Value.Year;
                egg_month = CAL_EggDate.Value.Month;
                egg_day = CAL_EggDate.Value.Day;
                egg_location = Util.getIndex(CB_EggLocation);
            }
            // Egg Met Data
            pk4.Egg_Year = egg_year - 2000;
            pk4.Egg_Month = egg_month;
            pk4.Egg_Day = egg_day;
            pk4.Egg_Location = egg_location;
            // Met Data
            pk4.Met_Year = CAL_MetDate.Value.Year - 2000;
            pk4.Met_Month = CAL_MetDate.Value.Month;
            pk4.Met_Day = CAL_MetDate.Value.Day;
            pk4.Met_Location = Util.getIndex(CB_MetLocation);

            if (pk4.IsEgg && pk4.Met_Location == 0)    // If still an egg, it has no hatch location/date. Zero it!
                pk4.Met_Year = pk4.Met_Month = pk4.Met_Day = 0;
            
            // Toss in Party Stats
            Array.Resize(ref pk4.Data, pk4.SIZE_PARTY);
            pk4.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk4.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk4.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk4.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk4.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk4.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk4.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk4.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk4.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk4.FixMoves();
            
            pk4.RefreshChecksum();
            return pk4;
        }
        private PK5 preparePK5()
        {
            PK5 pk5 = pkm as PK5;
            if (pk5 == null)
                return null;

            pk5.Species = Util.getIndex(CB_Species);
            pk5.HeldItem = Util.getIndex(CB_HeldItem);
            pk5.TID = Util.ToInt32(TB_TID.Text);
            pk5.SID = Util.ToInt32(TB_SID.Text);
            pk5.EXP = Util.ToUInt32(TB_EXP.Text);
            pk5.PID = Util.getHEXval(TB_PID.Text);
            pk5.Ability = (byte)Util.getIndex(DEV_Ability);
          //pk5.Ability = (byte)Array.IndexOf(abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));

            pk5.Nature = (byte)Util.getIndex(CB_Nature);
            pk5.FatefulEncounter = CHK_Fateful.Checked;
            pk5.Gender = PKX.getGender(Label_Gender.Text);
            pk5.AltForm = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
            pk5.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk5.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk5.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk5.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk5.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk5.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk5.CNT_Cool = Util.ToInt32(TB_Cool.Text);
            pk5.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            pk5.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            pk5.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            pk5.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            pk5.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            pk5.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk5.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            pk5.Nickname = TB_Nickname.Text;
            pk5.Move1 = Util.getIndex(CB_Move1);
            pk5.Move2 = Util.getIndex(CB_Move2);
            pk5.Move3 = Util.getIndex(CB_Move3);
            pk5.Move4 = Util.getIndex(CB_Move4);
            pk5.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk5.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk5.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk5.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk5.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk5.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk5.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk5.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk5.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk5.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk5.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk5.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk5.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk5.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk5.IsEgg = CHK_IsEgg.Checked;
            pk5.IsNicknamed = CHK_Nicknamed.Checked;

            pk5.OT_Name = TB_OT.Text;
            pk5.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            // Default Dates
            int egg_year = 2000;
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
            pk5.Egg_Year = egg_year - 2000;
            pk5.Egg_Month = egg_month;
            pk5.Egg_Day = egg_day;
            pk5.Egg_Location = egg_location;
            // Met Data
            pk5.Met_Year = CAL_MetDate.Value.Year - 2000;
            pk5.Met_Month = CAL_MetDate.Value.Month;
            pk5.Met_Day = CAL_MetDate.Value.Day;
            pk5.Met_Location = Util.getIndex(CB_MetLocation);

            if (pk5.IsEgg && pk5.Met_Location == 0)    // If still an egg, it has no hatch location/date. Zero it!
                pk5.Met_Year = pk5.Met_Month = pk5.Met_Day = 0;
            
            pk5.Ball = Util.getIndex(CB_Ball);
            pk5.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk5.OT_Gender = PKX.getGender(Label_OTGender.Text);
            pk5.EncounterType = Util.getIndex(CB_EncounterType);
            pk5.Version = Util.getIndex(CB_GameOrigin);
            pk5.Language = Util.getIndex(CB_Language);

            // Toss in Party Stats
            Array.Resize(ref pk5.Data, pk5.SIZE_PARTY);
            pk5.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk5.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk5.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk5.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk5.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk5.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk5.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk5.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk5.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk5.FixMoves();
            
            pk5.RefreshChecksum();
            return pk5;
        }
        private PK6 preparePK6()
        {
            PK6 pk6 = pkm as PK6;
            if (pk6 == null)
                return null;
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
            pk6.AltForm = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
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
            pk6.HT_Name = TB_OTt2.Text;

            // 0x90-0xAF
            pk6.HT_Gender = PKX.getGender(Label_CTGender.Text);
            // Plus more, set by MemoryAmie (already in buff)

            // Block D
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
            pk6.EncounterType = Util.getIndex(CB_EncounterType);
            pk6.Version = Util.getIndex(CB_GameOrigin);
            pk6.Country = Util.getIndex(CB_Country);
            pk6.Region = Util.getIndex(CB_SubRegion);
            pk6.ConsoleRegion = Util.getIndex(CB_3DSReg);
            pk6.Language = Util.getIndex(CB_Language);
            // 0xE4-0xE7

            // Toss in Party Stats
            Array.Resize(ref pk6.Data, pk6.SIZE_PARTY);
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

            // Fix Handler (Memories & OT) -- no foreign memories for Pokemon without a foreign trainer (none for eggs)
            if (Menu_ModifyPK6.Checked)
                pk6.FixMemories();

            // PKX is now filled
            pk6.RefreshChecksum();
            return pk6;
        }
        // Drag & Drop Events
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            openQuick(files[0]);
        }
        // Decrypted Export
        private void dragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift))
                clickQR(sender, e);
            if (e.Button == MouseButtons.Right)
                return;
            if (!verifiedPKM())
                return;

            // Create Temp File to Drag
            PKM pkx = preparePKM();
            bool encrypt = ModifierKeys == Keys.Control;
            string filename = $"{Path.GetFileNameWithoutExtension(pkx.FileName)}{(encrypt ? ".ek" : ".pk") + pkx.Format}";
            byte[] dragdata = encrypt ? pkx.EncryptedBoxData : pkx.DecryptedBoxData;
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

            if (SAV.ChecksumsValid) { Util.Alert("Checksums are valid."); return; }
            if (DialogResult.Yes == Util.Prompt(MessageBoxButtons.YesNo, "Export Checksum Info to Clipboard?"))
                Clipboard.SetText(SAV.ChecksumInfo);
        }
        private void clickExportSAVBAK(object sender, EventArgs e)
        {
            if (!SAV.Exportable)
                return;
            SaveFileDialog sfd = new SaveFileDialog
            { FileName = Util.CleanFileName(SAV.BAKName) };
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            string path = sfd.FileName;
            File.WriteAllBytes(path, SAV.BAK);
            Util.Alert("Saved Backup of current SAV to:", path);

            if (Directory.Exists(BackupPath)) return;
            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo,
                $"PKHeX can perform automatic backups if you create a folder with the name \"{BackupPath}\" in the same folder as PKHeX's executable.",
                "Would you like to create the backup folder now and save backup of current save?")) return;

            try { Directory.CreateDirectory(BackupPath); Util.Alert("Backup folder created!", 
                $"If you wish to no longer automatically back up save files, delete the \"{BackupPath}\" folder."); }
            catch { Util.Error($"Unable to create backup folder @ {BackupPath}"); }
        }
        private void clickExportSAV(object sender, EventArgs e)
        {
            if (!Menu_ExportSAV.Enabled)
                return;

            // Chunk Error Checking
            string err = SAV.MiscSaveChecks();
            if (err.Length > 0 && Util.Prompt(MessageBoxButtons.YesNo, err, "Continue saving?") != DialogResult.Yes)
                return;

            SaveFileDialog main = new SaveFileDialog
            {
                Filter = SAV.Filter, 
                FileName = SAV.FileName,
                RestoreDirectory = true
            };
            if (Directory.Exists(SAV.FilePath))
                main.InitialDirectory = SAV.FilePath;

            // Export
            if (main.ShowDialog() != DialogResult.OK) return;

            if (SAV.HasBox)
                SAV.CurrentBox = CB_BoxSelect.SelectedIndex;

            bool dsv = Path.GetExtension(main.FileName).ToLower() == ".dsv";
            File.WriteAllBytes(main.FileName, SAV.Write(dsv));
            Util.Alert("SAV exported to:", main.FileName);
        }

        // Box/SAV Functions //
        private void clickBoxRight(object sender, EventArgs e)
        {
            CB_BoxSelect.SelectedIndex = (CB_BoxSelect.SelectedIndex + 1) % SAV.BoxCount;
        }
        private void clickBoxLeft(object sender, EventArgs e)
        {
            CB_BoxSelect.SelectedIndex = (CB_BoxSelect.SelectedIndex + SAV.BoxCount-1) % SAV.BoxCount;
        }
        private void clickBoxSort(object sender, EventArgs e)
        {
            if (tabBoxMulti.SelectedIndex != 0)
                return;
            if (ModifierKeys == (Keys.Alt | Keys.Shift))
            {
                if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Clear ALL Boxes?!"))
                    return;

                SAV.resetBoxes();
                CB_BoxSelect.SelectedIndex = 0;
                Util.Alert("Boxes cleared!");
            }
            else if (ModifierKeys == Keys.Alt)
            {
                if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Clear Current Box?"))
                    return;

                SAV.resetBoxes(CB_BoxSelect.SelectedIndex, CB_BoxSelect.SelectedIndex + 1);
                setPKXBoxes();
                Util.Alert("Current Box cleared!");
            }
            else if (ModifierKeys == (Keys.Control | Keys.Shift))
            {
                if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Sort ALL Boxes?!"))
                    return;

                SAV.sortBoxes();
                CB_BoxSelect.SelectedIndex = 0;
                Util.Alert("Boxes sorted!");
            }
            else if (ModifierKeys == Keys.Control)
            {
                if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Sort Current Box?"))
                    return;

                SAV.sortBoxes(CB_BoxSelect.SelectedIndex, CB_BoxSelect.SelectedIndex + 1);
                setPKXBoxes();
                Util.Alert("Current Box sorted!");
            }
        }

        private void clickSlot(object sender, EventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control | Keys.Alt: clickClone(sender, e); break;
                case Keys.Control: clickView(sender, e); break;
                case Keys.Shift: clickSet(sender, e); break;
                case Keys.Alt: clickDelete(sender, e); break;
            }
        }
        private void clickView(object sender, EventArgs e)
        {
            int slot = getSlot(sender);

            if (SlotPictureBoxes[slot].Image == null)
            { SystemSounds.Exclamation.Play(); return; }
            int offset = getPKXOffset(slot);
            if (offset < 0)
            {
                Util.Error($"Slot read error @ slot {slot}.");
                return;
            }

            // Load the PKX file
            PKM pk = 30 <= slot && slot < 36 ? SAV.getPartySlot(offset) : SAV.getStoredSlot(offset);
            if (pk.Sanity == 0 && pk.Species != 0)
            {
                try { populateFields(pk); }
                catch { }
                // Visual to display what slot is currently loaded.
                getSlotColor(slot, Properties.Resources.slotView);
            }
            else
                SystemSounds.Exclamation.Play();
        }
        private void clickSet(object sender, EventArgs e)
        {
            if (!verifiedPKM()) return;
            int slot = getSlot(sender);
            if (slot == 30 && (CB_Species.SelectedIndex == 0 || CHK_IsEgg.Checked))
            { Util.Alert("Can't have empty/egg first slot."); return; }

            int offset = getPKXOffset(slot);
            if (offset < 0)
            {
                Util.Error($"Slot read error @ slot {slot}.");
                return;
            }
            PKM pk = preparePKM();

            string[] errata = verifyPKMtoSAV(pk);
            if (errata.Length > 0 && DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, string.Join(Environment.NewLine, errata), "Continue?"))
                return;

            if (slot >= 30 && slot < 36) // Party
                SAV.setPartySlot(pk, offset);
            else if (slot < 30 || (slot >= 36 && slot < 42 && DEV_Ability.Enabled))
                SAV.setStoredSlot(pk, offset);
            else return;
            
            if (slot >= 30 && slot < 36) 
                setParty();
            else 
                getQuickFiller(SlotPictureBoxes[slot], pk);

            getSlotColor(slot, Properties.Resources.slotSet);
        }
        private void clickDelete(object sender, EventArgs e)
        {
            int slot = getSlot(sender);
            if (slot == 30 && SAV.PartyCount == 1 && !DEV_Ability.Enabled) { Util.Alert("Can't delete first slot."); return; }

            int offset = getPKXOffset(slot);
            if (offset < 0)
            {
                Util.Error($"Slot read error @ slot {slot}.");
                return;
            }
            if (slot >= 30 && slot < 36) // Party
            { SAV.setPartySlot(SAV.BlankPKM, offset); setParty(); return; }
            if (slot < 30 || (slot >= 36 && slot < 42 && DEV_Ability.Enabled))
            { SAV.setStoredSlot(SAV.BlankPKM, getPKXOffset(slot)); }
            else return;

            SlotPictureBoxes[slot].Image = null;
            getSlotColor(slot, Properties.Resources.slotDel);
        }
        private void clickClone(object sender, EventArgs e)
        {
            if (getSlot(sender) > 30) return; // only perform action if cloning to boxes
            if (!verifiedPKM()) return; // don't copy garbage to the box
            
            if (Util.Prompt(MessageBoxButtons.YesNo, $"Clone Pokemon from Editing Tabs to all slots in {CB_BoxSelect.Text}?") != DialogResult.Yes)
                return;

            PKM pk = preparePKM();
            for (int i = 0; i < 30; i++) // set to every slot in box
            {
                SAV.setStoredSlot(pk, getPKXOffset(i));
                getQuickFiller(SlotPictureBoxes[i], pk);
            }
        }
        private void clickLegality(object sender, EventArgs e)
        {
            int slot = getSlot(sender);
            PKM pk;
            if (slot >= 0)
                pk = SAV.getStoredSlot(getPKXOffset(slot));
            else if (verifiedPKM())
                pk = preparePKM();
            else
                return;

            if (pk.Species == 0 || !pk.ChecksumValid)
            { SystemSounds.Asterisk.Play(); return; }
            if (typeof (PK6) != pk.GetType())
            {
                Util.Alert($"Checking legality of {pk.GetType()} files is not supported.");
                return;
            }
            showLegality(pk as PK6, slot < 0, ModifierKeys == Keys.Control);
        }

        private void updateEggRNGSeed(object sender, EventArgs e)
        {
            if (TB_RNGSeed.Text.Length == 0)
            {
                // Reset to 0
                TB_RNGSeed.Text = 0.ToString("X"+SAV.DaycareSeedSize);
                return; // recursively triggers this method, no need to continue
            }

            string filterText = Util.getOnlyHex(TB_RNGSeed.Text);
            if (filterText.Length != TB_RNGSeed.Text.Length)
            {
                Util.Alert("Expected HEX (0-9, A-F).", "Received: " + Environment.NewLine + TB_RNGSeed.Text);
                // Reset to Stored Value
                var seed = SAV.getDaycareRNGSeed(SAV.DaycareIndex);
                if (seed != null)
                    TB_RNGSeed.Text = ((ulong)seed).ToString("X"+SAV.DaycareSeedSize);
                return; // recursively triggers this method, no need to continue
            }

            // Write final value back to the save
            ulong value = Convert.ToUInt64(filterText, 16);
            if (value != SAV.getDaycareRNGSeed(SAV.DaycareIndex))
            {
                SAV.setDaycareRNGSeed(SAV.DaycareIndex, value);
                SAV.Edited = true;
            }            
        }
        private void updateU64(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length == 0)
            {
                // Reset to 0
                tb.Text = 0.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            // Currently saved Value
            ulong oldval = 0;
            if (tb == TB_GameSync)
                oldval = (SAV as SAV6).GameSyncID;
            else if (tb == TB_Secure1)
                oldval = (SAV as SAV6).Secure1;
            else if (tb == TB_Secure2)
                oldval = (SAV as SAV6).Secure2;

            string filterText = Util.getOnlyHex(tb.Text);

            if (filterText.Length != tb.Text.Length)
            {
                Util.Alert("Expected HEX (0-9, A-F).", "Received: " + Environment.NewLine + tb.Text);
                // Reset to Stored Value
                tb.Text = oldval.ToString("X16");
                return; // recursively triggers this method, no need to continue
            }

            // Write final value back to the save
            ulong newval = Convert.ToUInt64(filterText, 16);
            if (newval != oldval)
            {
                if (tb == TB_GameSync)
                    (SAV as SAV6).GameSyncID = newval;
                else if (tb == TB_Secure1)
                    (SAV as SAV6).Secure1 = newval;
                else if (tb == TB_Secure2)
                    (SAV as SAV6).Secure2 = newval;
                SAV.Edited = true;
            }
        }
        // Generic Subfunctions //
        private void setParty()
        {
            PKM[] party = SAV.PartyData;
            PKM[] battle = SAV.BattleBoxData;
            // Refresh slots
            if (SAV.HasParty)
                for (int i = 0; i < 6; i++)
                    getQuickFiller(SlotPictureBoxes[i + 30], party[i]);
            if (SAV.HasBattleBox)
                for (int i = 0; i < 6; i++)
                    getQuickFiller(SlotPictureBoxes[i + 36], battle[i]);
        }
        private int getPKXOffset(int slot)
        {
            if (slot < 30) // Box Slot
                return SAV.getBoxOffset(CB_BoxSelect.SelectedIndex) + slot * SAV.SIZE_STORED;
            slot -= 30;
            if (slot < 6) // Party Slot
                return SAV.getPartyOffset(slot);
            slot -= 6;
            if (slot < 6) // Battle Box Slot
                return SAV.BattleBox + slot * SAV.SIZE_STORED;
            slot -= 6;
            if (slot < 2) // Daycare
                return SAV.getDaycareSlotOffset(SAV.DaycareIndex, slot);
            slot -= 2;
            if (slot == 0) // GTS
                return SAV.GTS;
            slot -= 1;
            if (slot == 0) // Fused
                return SAV.Fused;
            slot -= 1;
            if (slot < 3) // SUBE
                return SAV.SUBE + slot * (SAV.SIZE_STORED + 4);
            return -1;
        }
        private int getSlot(object sender)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            return Array.IndexOf(SlotPictureBoxes, sender);
        }
        public void setPKXBoxes()
        {
            if (SAV.HasBox)
            {
                int boxoffset = SAV.getBoxOffset(CB_BoxSelect.SelectedIndex);
                if (SAV.HasBoxWallpapers)
                {
                    int boxbgval = SAV.getBoxWallpaper(CB_BoxSelect.SelectedIndex);
                    string imagename = "";
                    switch (SAV.Generation)
                    {
                        case 6:
                            imagename = "box_wp" + boxbgval.ToString("00");
                            if (SAV.ORAS && boxbgval > 16)
                                imagename += "o";
                            break;
                    }
                    if (!string.IsNullOrEmpty(imagename))
                        PAN_Box.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject(imagename);
                }

                for (int i = 0; i < 30; i++)
                    getSlotFiller(boxoffset + SAV.SIZE_STORED * i, SlotPictureBoxes[i]);
            }

            // Reload Party
            if (SAV.HasParty)
            {
                for (int i = 0; i < 6; i++)
                    getSlotFiller(SAV.getPartyOffset(i), SlotPictureBoxes[i + 30]);
            }

            // Reload Battle Box
            if (SAV.HasBattleBox)
            {
                for (int i = 0; i < 6; i++)
                    getSlotFiller(SAV.BattleBox + SAV.SIZE_STORED * i, SlotPictureBoxes[i + 36]);
            }

            // Reload Daycare
            if (SAV.HasDaycare)
            {
                Label[] L_SlotOccupied = { L_DC1, L_DC2 };
                TextBox[] TB_SlotEXP = { TB_Daycare1XP, TB_Daycare2XP };
                Label[] L_SlotEXP = { L_XP1, L_XP2 };

                for (int i = 0; i < 2; i++)
                {
                    getSlotFiller(SAV.getDaycareSlotOffset(SAV.DaycareIndex, i), SlotPictureBoxes[i + 42]);
                    uint? exp = SAV.getDaycareEXP(SAV.DaycareIndex, i);
                    TB_SlotEXP[i].Visible = L_SlotEXP[i].Visible = exp != null;
                    TB_SlotEXP[i].Text = exp.ToString();
                    bool? occ = SAV.getDaycareOccupied(SAV.DaycareIndex, i);
                    L_SlotOccupied[i].Visible = occ != null;
                    if (occ == true)   // If Occupied
                        L_SlotOccupied[i].Text = $"{i + 1}: ✓";
                    else
                    {
                        L_SlotOccupied[i].Text = $"{i + 1}: ✘";
                        SlotPictureBoxes[i + 42].Image = Util.ChangeOpacity(SlotPictureBoxes[i + 42].Image, 0.6);
                    }
                }
                bool? egg = SAV.getDaycareHasEgg(SAV.DaycareIndex);
                DayCare_HasEgg.Visible = egg != null;
                DayCare_HasEgg.Checked = egg == true;

                ulong? seed = SAV.getDaycareRNGSeed(SAV.DaycareIndex);
                L_DaycareSeed.Visible = TB_RNGSeed.Visible = seed != null;
                if (seed != null)
                {
                    TB_RNGSeed.MaxLength = SAV.DaycareSeedSize;
                    TB_RNGSeed.Text = ((ulong)seed).ToString("X"+SAV.DaycareSeedSize);
                }
            }

            // GTS
            if (SAV.HasGTS)
            getSlotFiller(SAV.GTS, SlotPictureBoxes[44]);

            // Fused
            if (SAV.HasFused)
            getSlotFiller(SAV.Fused, SlotPictureBoxes[45]);

            // SUBE
            if (SAV.HasSUBE)
            for (int i = 0; i < 3; i++)
            {
                int offset = SAV.SUBE + i * (SAV.SIZE_STORED + 4);
                if (BitConverter.ToUInt64(SAV.Data, offset) != 0)
                    getSlotFiller(offset, SlotPictureBoxes[46 + i]);
                else SlotPictureBoxes[46 + i].Image = null;
            }

            // Recoloring of a storage box slot (to not show for other storage boxes)
            if (colorizedslot <= SAV.BoxCount)
                SlotPictureBoxes[colorizedslot].BackgroundImage = colorizedbox == CB_BoxSelect.SelectedIndex ? colorizedcolor : null;
        }
        private void setBoxNames()
        {
            if (!SAV.HasBox)
                return;
            int selectedbox = CB_BoxSelect.SelectedIndex; // precache selected box
            // Build ComboBox Dropdown Items
            try
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 0; i < SAV.BoxCount; i++)
                    CB_BoxSelect.Items.Add(SAV.getBoxName(i));
            }
            catch
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 0; i < SAV.BoxCount; i++)
                    CB_BoxSelect.Items.Add("BOX " + (i+1));
            }
            if (selectedbox < CB_BoxSelect.Items.Count)
                CB_BoxSelect.SelectedIndex = selectedbox; // restore selected box
        }
        private void getQuickFiller(PictureBox pb, PKM pk = null)
        {
            if (!fieldsInitialized) return;
            pk = pk ?? preparePKM(false); // don't perform control loss click

            if (pb == dragout) mnuLQR.Enabled = pk.Species != 0; // Species
            pb.Image = pk.Sprite;
            if (pb.BackColor == Color.Red)
                pb.BackColor = Color.Transparent;
        }
        private void getSlotFiller(int offset, PictureBox pb)
        {
            if (SAV.getData(offset, SAV.SIZE_STORED).SequenceEqual(new byte[SAV.SIZE_STORED]))
            {
                // 00s present in slot.
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                return;
            }
            PKM p = SAV.getStoredSlot(offset);
            if (p.Sanity != 0 || !p.ChecksumValid) // Invalid
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

            if (slot < 30)
                colorizedbox = CB_BoxSelect.SelectedIndex;

            SlotPictureBoxes[slot].BackgroundImage = color;
            colorizedcolor = color;
            colorizedslot = slot;
        }
        private void getBox(object sender, EventArgs e)
        {
            setPKXBoxes();
        }
        private void switchDaycare(object sender, EventArgs e)
        {
            if (!SAV.ORAS) return;
            if (DialogResult.Yes == Util.Prompt(MessageBoxButtons.YesNo, "Would you like to switch the view to the other Daycare?",
                $"Currently viewing daycare {SAV.DaycareIndex + 1}."))
                // If ORAS, alter the daycare offset via toggle.
                SAV.DaycareIndex ^= 1;

            // Refresh Boxes
            setPKXBoxes();
        }
        private void dumpBoxesToDB(string path, bool individualBoxFolders)
        {
            PKM[] boxdata = SAV.BoxData;
            if (boxdata == null) { Util.Error("Null argument when dumping boxes."); return; } 
            for (int i = 0; i < boxdata.Length; i++)
            {
                PKM pk = boxdata[i];
                if (pk.Species == 0 || pk.Sanity != 0)
                    continue;
                string fileName = Util.CleanFileName(pk.FileName);
                string boxfolder = "";
                if (individualBoxFolders)
                {
                    boxfolder = SAV.getBoxName(i/30);
                    Directory.CreateDirectory(Path.Combine(path, boxfolder));
                }
                if (!File.Exists(Path.Combine(Path.Combine(path, boxfolder), fileName)))
                    File.WriteAllBytes(Path.Combine(Path.Combine(path, boxfolder), fileName), pk.Data.Take(SAV.SIZE_STORED).ToArray());
            }
        }
        private void loadBoxesFromDB(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (!SAV.HasBox) return;
            
            DialogResult dr = Util.Prompt(MessageBoxButtons.YesNoCancel, "Clear subsequent boxes when importing data?", "If you only want to overwrite for new data, press no.");
            if (dr == DialogResult.Cancel)
                return;
            if (dr == DialogResult.Yes)
                SAV.resetBoxes(CB_BoxSelect.SelectedIndex);

            bool? noSetb = getPKMSetOverride();

            int ctr = CB_BoxSelect.SelectedIndex*30;
            int pastctr = 0;
            string[] filepaths = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);

            foreach (byte[] data in from file in filepaths where PKX.getIsPKM(new FileInfo(file).Length) select File.ReadAllBytes(file))
            {
                string c;
                PKM temp = PKMConverter.getPKMfromBytes(data);
                PKM pk = PKMConverter.convertToFormat(temp, SAV.Generation, out c);

                if (pk != null) // Write to save
                {
                    if (verifyPKMtoSAV(pk).Length > 0)
                        continue;
                    SAV.setStoredSlot(pk, SAV.getBoxOffset(ctr/30) + ctr%30 * SAV.SIZE_STORED, noSetb);
                    if (pk.Format != temp.Format) // Transferred
                        pastctr++;
                    if (++ctr == SAV.BoxCount*30) // Boxes full!
                        break; 
                }
                Console.WriteLine(c);
            }
            ctr -= 30*CB_BoxSelect.SelectedIndex; // actual imported count
            if (ctr <= 0)
                return; 

            setPKXBoxes();
            string result = $"Loaded {ctr} files to boxes.";
            if (pastctr > 0)
                Util.Alert(result, $"Conversion successful for {pastctr} past generation files.");
            else
                Util.Alert(result);
        }
        private void B_SaveBoxBin_Click(object sender, EventArgs e)
        {
            if (!SAV.HasBox)
            { Util.Alert("Save file does not have boxes to dump!"); return; }

            DialogResult dr = Util.Prompt(MessageBoxButtons.YesNoCancel, 
                "Yes: Export All Boxes" + Environment.NewLine + 
                $"No: Export {CB_BoxSelect.Text} (Box {CB_BoxSelect.SelectedIndex + 1})" + Environment.NewLine + 
                "Cancel: Abort");

            if (dr == DialogResult.Yes)
            {
                SaveFileDialog sfd = new SaveFileDialog {Filter = "Box Data|*.bin", FileName = "pcdata.bin"};
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                File.WriteAllBytes(sfd.FileName, SAV.getPCBin());
            }
            else if (dr == DialogResult.No)
            {
                SaveFileDialog sfd = new SaveFileDialog {Filter = "Box Data|*.bin", FileName = $"boxdata {CB_BoxSelect.Text}.bin"};
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                File.WriteAllBytes(sfd.FileName, SAV.getBoxBin(CB_BoxSelect.SelectedIndex));
            }
        }
        private bool? getPKMSetOverride()
        {
            var yn = Menu_ModifyPK6.Checked ? "Yes" : "No";
            DialogResult noSet = Util.Prompt(MessageBoxButtons.YesNoCancel, 
                "Loading overrides:",
                    "Yes - Modify .pk* when set to SAV" + Environment.NewLine +
                    "No - Don't modify .pk*" + Environment.NewLine +
                    $"Cancel - Use current settings ({yn})");
            return noSet == DialogResult.Yes ? true : (noSet == DialogResult.No ? (bool?)false : null);
        }

        // Subfunction Save Buttons //
        private void B_OpenWondercards_Click(object sender, EventArgs e)
        {
            new SAV_Wondercard(sender as MysteryGift).ShowDialog();
        }
        private void B_OpenBoxLayout_Click(object sender, EventArgs e)
        {
            new SAV_BoxLayout(CB_BoxSelect.SelectedIndex).ShowDialog();
            setBoxNames(); // fix box names
            setPKXBoxes(); // refresh box background
        }
        private void B_OpenTrainerInfo_Click(object sender, EventArgs e)
        {
            if (SAV.Generation < 6)
                new SAV_SimpleTrainer().ShowDialog();
            else if (SAV.Generation == 6)
                new SAV_Trainer().ShowDialog();
            // Refresh conversion info
            PKMConverter.updateConfig(SAV.SubRegion, SAV.Country, SAV.ConsoleRegion, SAV.OT, SAV.Gender);
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
            new SAV_BerryFieldXY().ShowDialog();
        }
        private void B_OpenPokeblocks_Click(object sender, EventArgs e)
        {
            new SAV_PokeBlockORAS().ShowDialog();
        }
        private void B_OpenEventFlags_Click(object sender, EventArgs e)
        {
            new SAV_EventFlags().ShowDialog();
        }
        private void B_OpenSuperTraining_Click(object sender, EventArgs e)
        {
            new SAV_SuperTrain().ShowDialog();
        }
        private void B_OpenOPowers_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
                return;
            if (SAV.ORAS)
            {
                DialogResult dr = Util.Prompt(MessageBoxButtons.YesNo, "No editing support for ORAS :(", "Max O-Powers with a working code?");
                if (dr != DialogResult.Yes) return;
                new byte[] 
                { 
                    0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00,
                    0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x00, 0x00, 0x00,
                }.CopyTo(SAV.Data, (SAV as SAV6).OPower);
            }
            else if (SAV.XY)
                new SAV_OPower().ShowDialog();
        }
        private void B_OpenPokedex_Click(object sender, EventArgs e)
        {
            if (SAV.ORAS)
                new SAV_PokedexORAS().ShowDialog();
            else if (SAV.XY)
                new SAV_PokedexXY().ShowDialog();
        }
        private void B_OUTPasserby_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
                return;
            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Export Passerby Info to Clipboard?"))
                return;
            string result = "PSS List" + Environment.NewLine;
            string[] headers = { "PSS Data - Friends", "PSS Data - Acquaintances", "PSS Data - Passerby", };
            int offset = (SAV as SAV6).PSS;
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
            new SAV_HallOfFame().ShowDialog();
        }
        private void B_OpenSecretBase_Click(object sender, EventArgs e)
        {
            new SAV_SecretBase().ShowDialog();
        }
        private void B_JPEG_Click(object sender, EventArgs e)
        {
            byte[] jpeg = SAV.JPEGData;
            if (SAV.JPEGData == null)
            { Util.Alert("No PGL picture data found in the save file!"); return; }
            string filename = SAV.JPEGTitle + "'s picture";
            SaveFileDialog sfd = new SaveFileDialog {FileName = filename, Filter = "JPEG|*.jpeg"};
            if (sfd.ShowDialog() != DialogResult.OK) return;
            File.WriteAllBytes(sfd.FileName, jpeg);
        }
        // Save Folder Related
        private void clickSaveFileName(object sender, EventArgs e)
        {
            string path = detectSaveFile();
            if (path == null || !File.Exists(path)) return;
            if (Util.Prompt(MessageBoxButtons.YesNo, "Open save file from the following location?", path) == DialogResult.Yes)
                openQuick(path); // load save
        }
        private static string detectSaveFile()
        {
            string pathSDF = Util.GetSDFLocation();
            string path3DS = Util.get3DSLocation();
            string pathCache = Util.GetCacheFolder();
            
            if (path3DS != null && Directory.Exists(Path.Combine(path3DS, "SaveDataBackup")) && ModifierKeys != Keys.Control)
                return Path.Combine(Path.GetPathRoot(path3DS), "SaveDataBackup", "main");
            if (pathSDF != null && ModifierKeys != Keys.Shift) // if we have a result
                return Path.Combine(pathSDF, "main");
            if (path3DS != null && Directory.Exists(Path.Combine(Path.GetPathRoot(path3DS), "JKSV", "Saves")))
                return Directory.GetFiles(Path.Combine(Path.GetPathRoot(path3DS), "JKSV", "Saves"), "main", SearchOption.AllDirectories)
                        .Where(f => SaveUtil.SizeValidSAV6((int)new FileInfo(f).Length)) // filter
                        .OrderByDescending(f => new FileInfo(f).LastWriteTime).FirstOrDefault();
            if (Directory.Exists(pathCache))
                return Directory.GetFiles(pathCache).Where(f => SaveUtil.SizeValidSAV6((int)new FileInfo(f).Length)) // filter
                    .OrderByDescending(f => new FileInfo(f).LastWriteTime).FirstOrDefault();
            if (File.Exists(Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root", "main")))) // if cgse exists
                return Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root", "main"));

            return null;
        }

        // Drag & Drop within Box
        private void pbBoxSlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.Clicks != 1) return;
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift || ModifierKeys == (Keys.Control | Keys.Alt))
            { clickSlot(sender, e); return; }
            PictureBox pb = (PictureBox)sender;
            if (pb.Image == null)
                return;

            pkm_from_slot = getSlot(sender);
            int offset = getPKXOffset(pkm_from_slot);
            // Create Temp File to Drag
            Cursor.Current = Cursors.Hand;

            // Prepare Data
            pkm_from = SAV.getData(offset, SAV.SIZE_STORED);
            pkm_from_offset = offset;

            // Make a new file name based off the PID
            byte[] dragdata = SAV.decryptPKM(pkm_from);
            Array.Resize(ref dragdata, SAV.SIZE_STORED);
            PKM pkx = SAV.getPKM(dragdata);
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
                if (files.Length <= 0)
                    return;
                string file = files[0];
                if (!PKX.getIsPKM(new FileInfo(file).Length))
                { openQuick(file); return; }

                byte[] data = File.ReadAllBytes(file);
                PKM temp = PKMConverter.getPKMfromBytes(data);
                string c;

                PKM pk = PKMConverter.convertToFormat(temp, SAV.Generation, out c);
                if (pk == null)
                { Util.Error(c); Console.WriteLine(c); return; }

                string[] errata = verifyPKMtoSAV(pk);
                if (errata.Length > 0)
                {
                    string concat = string.Join(Environment.NewLine, errata);
                    if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, concat, "Continue?"))
                    { Console.WriteLine(c); Console.WriteLine(concat); return; }
                }

                SAV.setStoredSlot(pk, offset);
                getQuickFiller(SlotPictureBoxes[slot], pk);
                getSlotColor(slot, Properties.Resources.slotSet);
                Console.WriteLine(c);
            }
            else
            {
                if (ModifierKeys == Keys.Alt && slot > -1) // overwrite delete old slot
                {
                    // Clear from slot 
                    getQuickFiller(SlotPictureBoxes[pkm_from_slot], SAV.BlankPKM); // picturebox
                    SAV.setStoredSlot(SAV.BlankPKM, pkm_from_offset); // savefile
                }
                else if (ModifierKeys != Keys.Control && slot > -1)
                {
                    // Load data from destination
                    PKM pk = SAV.getStoredSlot(offset);

                    // Swap slot picture
                    getQuickFiller(SlotPictureBoxes[pkm_from_slot], pk);

                    // Swap slot data to source
                    SAV.setStoredSlot(pk, pkm_from_offset);
                }
                // Copy from temp slot to new.
                SAV.setStoredSlot(pkm_from, offset);
                PKM pkz = SAV.getPKM(SAV.decryptPKM(pkm_from));
                getQuickFiller(SlotPictureBoxes[slot], pkz);

                pkm_from_offset = 0; // Clear offset value
            }
        }
        private void pbBoxSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }

        private byte[] pkm_from;
        private int pkm_from_offset;
        private int pkm_from_slot = -1;
        #endregion
    }
}
