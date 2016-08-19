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
            slotPkmSource = SAV.BlankPKM.EncryptedPartyData;
            InitializeComponent();
            CB_ExtraBytes.SelectedIndex = 0;
            SaveFile.SetUpdateDex = Menu_ModifyDex.Checked;
            SaveFile.SetUpdatePKM = Menu_ModifyPKM.Checked;
            getFieldsfromPKM = populateFieldsPK6;
            getPKMfromFields = preparePK6;

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

            FLP_SAVtools.Scroll += Util.PanelScroll;

            // Load WC6 folder to legality
            refreshWC6DB();

            #endregion
            #region Localize & Populate Fields
            string[] args = Environment.GetCommandLineArgs();
            string filename = args.Length > 0 ? Path.GetFileNameWithoutExtension(args[0])?.ToLower() : "";
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
            if (!SAV.Exportable) // No SAV loaded from exe args
            {
                string path = SaveUtil.detectSaveFile();
                if (path != null && File.Exists(path))
                    openQuick(path, force: true);
                else
                    loadSAV(SAV, null);
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
        public static SaveFile SAV = new SAV6 { Game = (int)GameVersion.AS, OT = "PKHeX", TID = 12345, SID = 54321, Language = 2, Country = 49, SubRegion = 7, ConsoleRegion = 1 }; // Save File
        public static Color defaultControlWhite, defaultControlText;
        public static string eggname = "";

        public static string curlanguage = "en";
        public static string[] gendersymbols = { "♂", "♀", "-" };
        public static string[] specieslist, movelist, itemlist, abilitylist, types, natures, forms,
            memories, genloc, trainingbags, trainingstage, characteristics,
            encountertypelist, gamelanguages, balllist, gamelist, pokeblocks, g3items = { };
        public static string[] metRSEFRLG_00000 = { };
        public static string[] metHGSS_00000, metHGSS_02000, metHGSS_03000 = { };
        public static string[] metBW2_00000, metBW2_30000, metBW2_40000, metBW2_60000 = { };
        public static string[] metXY_00000, metXY_30000, metXY_40000, metXY_60000 = { };
        public static string[] wallpapernames, puffs = { };
        public static bool unicode;
        public static List<ComboItem> MoveDataSource, ItemDataSource, SpeciesDataSource, BallDataSource, NatureDataSource, AbilityDataSource, VersionDataSource;

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

        #region Path Variables

        public static string WorkingDirectory => Environment.CurrentDirectory;
        public static string DatabasePath => Path.Combine(WorkingDirectory, "db");
        private static string WC6DatabasePath => Path.Combine(WorkingDirectory, "wc6");
        private static string BackupPath => Path.Combine(WorkingDirectory, "bak");

        #endregion

        #region //// MAIN MENU FUNCTIONS ////
        // Main Menu Strip UI Functions
        private void mainMenuOpen(object sender, EventArgs e)
        {
            string pkx = pkm.Extension;
            string ekx = 'e' + pkx.Substring(1, pkx.Length-1);

            string supported = "*.pkm;";
            for (int i = 3; i <= SAV.Generation; i++)
            {
                supported += $"*.pk{i}";
                if (i != pkm.Format)
                    supported += ";";
            }

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = $"Decrypted PKM File|{supported}" +
                         $"|Encrypted PKM File|*.{ekx}" +
                         "|Binary File|*.bin" +
                         "|All Files|*.*",
                RestoreDirectory = true,
                FilterIndex = 4,
                FileName = "main",
            };

            // Reset file dialog path if it no longer exists
            if (!Directory.Exists(ofd.InitialDirectory))
                ofd.InitialDirectory = WorkingDirectory;

            // Detect main
            string path = SaveUtil.detectSaveFile();
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
            string pkx = pk.Extension;
            string ekx = 'e' + pkx.Substring(1, pkx.Length - 1);
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = $"Decrypted PKM File|*.{pkx}" +
                         $"|Encrypted PKM File|*.{ekx}" +
                         "|Binary File|*.bin" +
                         "|All Files|*.*",
                DefaultExt = pkx,
                FileName = Util.CleanFileName(pk.FileName)
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            string path = sfd.FileName;
            string ext = Path.GetExtension(path);

            if (File.Exists(path))
            {
                // File already exists, save a .bak
                byte[] backupfile = File.ReadAllBytes(path);
                File.WriteAllBytes(path + ".bak", backupfile);
            }

            if (new[] {".ekx", "."+ekx, ".bin"}.Contains(ext))
                File.WriteAllBytes(path, pk.EncryptedPartyData);
            else if (new[] { "."+pkx }.Contains(ext))
                File.WriteAllBytes(path, pk.DecryptedBoxData);
            else
            {
                Util.Error($"Foreign File Extension: {ext}", "Exporting as encrypted.");
                File.WriteAllBytes(path, pkm.EncryptedPartyData);
            }
        }
        private void mainMenuExit(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // Hotkey Triggered
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

            if (Directory.Exists(DatabasePath))
                new SAV_Database(this).Show();
            else
                Util.Alert("PKHeX's database was not found.",
                    $"Please dump all boxes from a save file, then ensure the '{DatabasePath}' folder exists.");
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
            SaveFile.SetUpdatePKM = Menu_ModifyPKM.Checked;
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
        private void manMenuBatchEditor(object sender, EventArgs e)
        {
            new BatchEditor().ShowDialog();
            setPKXBoxes(); // refresh
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
            int[] abilities = SAV.Personal.getAbilities(Set.Species, form);
            int ability = Array.IndexOf(abilities, Set.Ability);
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
                Process.Start("explorer.exe", Path.Combine(path, "root"));
            else if (Directory.Exists(path))
                Process.Start("explorer.exe", path);
            else
                Util.Alert("Can't find the temporary file.", "Make sure the Cyber Gadget software is paused.");
        }
        private void clickOpenCacheFolder(object sender, EventArgs e)
        {
            string path = Util.GetCacheFolder();
            if (Directory.Exists(path))
                Process.Start("explorer.exe", path);
            else
                Util.Alert("Can't find the cache folder.");
        }
        private void clickOpenSDFFolder(object sender, EventArgs e)
        {
            string path = Path.GetPathRoot(Util.get3DSLocation());
            if (path != null && Directory.Exists(path = Path.Combine(path, "filer", "UserSaveData")))
                Process.Start("explorer.exe", path);
            else
                Util.Alert("Can't find the SaveDataFiler folder.");
        }
        private void clickOpenSDBFolder(object sender, EventArgs e)
        {
            string path3DS = Path.GetPathRoot(Util.get3DSLocation());
            string path;
            if (path3DS != null && Directory.Exists(path = Path.Combine(path3DS, "SaveDataBackup")))
                Process.Start("explorer.exe", path);
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

                if (Util.Prompt(MessageBoxButtons.YesNo, "Load Battle Video Pokémon data to " + CB_BoxSelect.Text + "?", "The box will be overwritten.") != DialogResult.Yes)
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
            if (sav.Generation <= 3) // Japanese Save files are different. Get isJapanese
            {
                if (sav.Version == GameVersion.Unknown)
                {
                    // Hacky cheats invalidated the Game Code value.
                    var drGame = Util.Prompt(MessageBoxButtons.YesNoCancel,
                        "Unknown Gen3 Game Detected. Select Origins:",
                        "Yes: Ruby / Sapphire" + Environment.NewLine + 
                        "No: Emerald" + Environment.NewLine +
                        "Cancel: FireRed / LeafGreen");

                    if (drGame == DialogResult.Yes)
                        sav = new SAV3(sav.BAK, GameVersion.RS);
                    else if (drGame == DialogResult.No)
                        sav = new SAV3(sav.BAK, GameVersion.E);
                    else
                        sav = new SAV3(sav.BAK, GameVersion.FRLG);
                }
                var drJP = Util.Prompt(MessageBoxButtons.YesNoCancel, $"Generation 3 ({sav.Version}) Save File detected. Select Origins:", "Yes: International" + Environment.NewLine + "No: Japanese");
                if (drJP == DialogResult.Cancel)
                    return;
                sav.Japanese = drJP == DialogResult.No;

                if (sav.Version == GameVersion.FRLG)
                {
                    var drFRLG = Util.Prompt(MessageBoxButtons.YesNoCancel, $"{sav.Version} detected. Select version...", "Yes: FireRed" + Environment.NewLine + "No: LeafGreen");
                    if (drFRLG == DialogResult.Cancel)
                        return;

                    sav.Personal = drFRLG == DialogResult.Yes ? PersonalTable.FR : PersonalTable.LG;
                }
            }
            loadSAV(sav, path);
        }
        private void loadSAV(SaveFile sav, string path)
        {
            PKM pk = preparePKM();
            SAV = sav;

            if (path != null) // Actual save file
            {
                SAV.FilePath = Path.GetDirectoryName(path);
                SAV.FileName = Path.GetExtension(path) == ".bak"
                    ? Path.GetFileName(path).Split(new[] { " [" }, StringSplitOptions.None)[0]
                    : Path.GetFileName(path);
                L_Save.Text = $"SAV{SAV.Generation}: {Path.GetFileNameWithoutExtension(Util.CleanFileName(SAV.BAKName))}"; // more descriptive

                // If backup folder exists, save a backup.
                string backupName = Path.Combine(BackupPath, Util.CleanFileName(SAV.BAKName));
                if (SAV.Exportable && Directory.Exists(BackupPath) && !File.Exists(backupName))
                    File.WriteAllBytes(backupName, SAV.BAK);

                GB_SAVtools.Visible = true;
            }
            else // Blank save file
            {
                SAV.FilePath = null;
                SAV.FileName = "Blank Save File";
                L_Save.Text = $"SAV{SAV.Generation}: {SAV.FileName} [{SAV.OT} ({SAV.Version})]";

                GB_SAVtools.Visible = false;
            }
            Menu_ExportSAV.Enabled = B_VerifyCHK.Enabled = SAV.Exportable;

            setBoxNames();   // Display the Box Names
            if (SAV.HasBox)
            {
                int startBox = path == null ? 0 : SAV.CurrentBox; // FF if BattleBox
                if (startBox > SAV.BoxCount - 1) { tabBoxMulti.SelectedIndex = 1; CB_BoxSelect.SelectedIndex = 0; }
                else { tabBoxMulti.SelectedIndex = 0; CB_BoxSelect.SelectedIndex = startBox; }
            }
            setPKXBoxes();   // Reload all of the PKX Windows

            // Hide content if not present in game.
            GB_SUBE.Visible = SAV.HasSUBE;
            PB_Locked.Visible = SAV.HasBattleBox && SAV.BattleBoxLocked;

            PAN_Box.Visible = CB_BoxSelect.Visible = B_BoxLeft.Visible = B_BoxRight.Visible = SAV.HasBox;
            Menu_LoadBoxes.Enabled = Menu_DumpBoxes.Enabled = Menu_Report.Enabled = Menu_Modify.Enabled = B_SaveBoxBin.Enabled = SAV.HasBox;

            if (GB_SAVtools.Visible)
            {
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
                B_LinkInfo.Visible = SAV.HasLink;
                B_CGearSkin.Visible = SAV.Generation == 5;
            }
            
            // Generational Interface
            byte[] extraBytes = new byte[1];
            Tip1.RemoveAll(); Tip2.RemoveAll(); Tip3.RemoveAll(); // TSV/PSV

            CB_Country.Visible = CB_SubRegion.Visible = CB_3DSReg.Visible =
            Label_Country.Visible = Label_SubRegion.Visible = Label_3DSRegion.Visible = SAV.Generation >= 6;
            Label_EncryptionConstant.Visible = BTN_RerollEC.Visible = TB_EC.Visible = SAV.Generation >= 6;
            GB_nOT.Visible = GB_RelearnMoves.Visible = BTN_Medals.Visible = BTN_History.Visible = SAV.Generation >= 6;
            PB_Legal.Visible = PB_WarnMove1.Visible = PB_WarnMove2.Visible = PB_WarnMove3.Visible = PB_WarnMove4.Visible = SAV.Generation >= 6;

            PB_MarkPentagon.Visible = SAV.Generation == 6;
            TB_GameSync.Visible = TB_Secure1.Visible = TB_Secure2.Visible = L_GameSync.Visible = L_Secure1.Visible = L_Secure2.Visible = SAV.Exportable && SAV.Generation == 6;

            CB_Form.Visible = Label_Form.Visible = CHK_AsEgg.Visible = GB_EggConditions.Visible = 
            Label_MetDate.Visible = CAL_MetDate.Visible = PB_Mark5.Visible = PB_Mark6.Visible = SAV.Generation >= 4;

            BTN_Ribbons.Visible = SAV.Generation >= 3;
            DEV_Ability.Enabled = DEV_Ability.Visible = SAV.Generation > 3 && HaX;
            TB_AbilityNumber.Visible = SAV.Generation >= 6 && DEV_Ability.Enabled;
            CB_Ability.Visible = !DEV_Ability.Enabled && SAV.Generation >= 3;

            // Recenter PKM SubEditors
            FLP_PKMEditors.Location = new Point((tabMain.TabPages[4].Width - FLP_PKMEditors.Width) / 2, FLP_PKMEditors.Location.Y);

            switch (SAV.Generation)
            {
                case 3:
                    getFieldsfromPKM = populateFieldsPK3;
                    getPKMfromFields = preparePK3;
                    extraBytes = PK3.ExtraBytes;
                    break;
                case 4:
                    getFieldsfromPKM = populateFieldsPK4;
                    getPKMfromFields = preparePK4;
                    extraBytes = PK4.ExtraBytes;
                    break;
                case 5:
                    getFieldsfromPKM = populateFieldsPK5;
                    getPKMfromFields = preparePK5;
                    extraBytes = PK5.ExtraBytes;
                    break;
                case 6:
                    getFieldsfromPKM = populateFieldsPK6;
                    getPKMfromFields = preparePK6;
                    extraBytes = PK6.ExtraBytes;
                    SAV6 sav6 = (SAV6)SAV;
                    TB_GameSync.Enabled = sav6.GameSyncID != 0;
                    TB_GameSync.Text = sav6.GameSyncID.ToString("X16");
                    TB_Secure1.Text = sav6.Secure1.ToString("X16");
                    TB_Secure2.Text = sav6.Secure2.ToString("X16");
                    break;
            }
            bool init = fieldsInitialized;
            fieldsInitialized = false;
            populateFilteredDataSources();
            populateFields(pkm.Format != SAV.Generation ? SAV.BlankPKM : pk);
            fieldsInitialized |= init;

            // SAV Specific Limits
            TB_OT.MaxLength = SAV.OTLength;
            TB_OTt2.MaxLength = SAV.OTLength;
            TB_Nickname.MaxLength = SAV.NickLength;

            // Common HaX Interface
            CHK_HackedStats.Enabled = CHK_HackedStats.Visible = MT_Level.Enabled = MT_Level.Visible = MT_Form.Enabled = MT_Form.Visible = HaX;
            TB_Level.Visible = !HaX;

            // Load Extra Byte List
            CB_ExtraBytes.Items.Clear();
            foreach (byte b in extraBytes)
                CB_ExtraBytes.Items.Add("0x" + b.ToString("X2"));
            CB_ExtraBytes.SelectedIndex = 0;

            // Refresh PK#->PK6 conversion info
            PKMConverter.updateConfig(SAV.SubRegion, SAV.Country, SAV.ConsoleRegion, SAV.OT, SAV.Gender);

            // Indicate audibly the save is loaded
            SystemSounds.Beep.Play();
        }
        private static void refreshWC6DB()
        {
            List<WC6> wc6db = new List<WC6>();
            byte[] wc6bin = Properties.Resources.wc6;
            for (int i = 0; i < wc6bin.Length; i += WC6.Size)
            {
                byte[] data = new byte[WC6.Size];
                Array.Copy(wc6bin, i, data, 0, WC6.Size);
                wc6db.Add(new WC6(data));
            }
            byte[] wc6full = Properties.Resources.wc6full;
            for (int i = 0; i < wc6full.Length; i += WC6.SizeFull)
            {
                byte[] data = new byte[WC6.SizeFull];
                Array.Copy(wc6bin, i, data, 0, WC6.SizeFull);
                wc6db.Add(new WC6(data));
            }

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
            // Recenter PKM SubEditors
            FLP_PKMEditors.Location = new Point((tabMain.TabPages[4].Width - FLP_PKMEditors.Width)/2, FLP_PKMEditors.Location.Y);
            populateFields(pk); // put data back in form
            fieldsInitialized |= alreadyInit;
        }
        private void InitializeStrings()
        {
            if (CB_MainLanguage.SelectedIndex < 8)
                curlanguage = lang_val[CB_MainLanguage.SelectedIndex];

            // Past Generation strings
            g3items = Util.getStringList("ItemsG3", "en");
            metRSEFRLG_00000 = Util.getStringList("rsefrlg_00000", "en");

            // Current Generation strings
            string l = curlanguage;
            natures = Util.getStringList("natures", l);
            types = Util.getStringList("types", l);
            abilitylist = Util.getAbilitiesList(l);
            movelist = Util.getMovesList(l);
            itemlist = Util.getStringList("items", l);
            characteristics = Util.getStringList("character", l);
            specieslist = Util.getSpeciesList(l);
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
            string[] items = itemlist;
            if (SAV.Generation == 3)
                items = g3items;

            ItemDataSource = Util.getCBList(items, (HaX ? Enumerable.Range(0, SAV.MaxItemID) : SAV.HeldItems.Select(i => (int)i)).ToArray());
            CB_HeldItem.DataSource = new BindingSource(ItemDataSource.Where(i => i.Value <= SAV.MaxItemID).ToList(), null);

            CB_Ball.DataSource = new BindingSource(BallDataSource.Where(b => b.Value <= SAV.MaxBallID).ToList(), null);
            CB_Species.DataSource = new BindingSource(SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList(), null);
            DEV_Ability.DataSource = new BindingSource(AbilityDataSource.Where(a => a.Value <= SAV.MaxAbilityID).ToList(), null);
            CB_GameOrigin.DataSource = new BindingSource(VersionDataSource.Where(g => g.Value <= SAV.MaxGameID || SAV.Generation >= 3 && g.Value == 15).ToList(), null);

            // Set the Move ComboBoxes too..
            var moves = MoveDataSource.Where(m => m.Value <= SAV.MaxMoveID).ToList();
            foreach (ComboBox cb in new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4, CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 })
            {
                cb.DisplayMember = "Text"; cb.ValueMember = "Value";
                cb.DataSource = new BindingSource(moves, null);
            }
        }
        private Action getFieldsfromPKM;
        private Func<PKM> getPKMfromFields;
        public void populateFields(PKM pk, bool focus = true)
        {
            if (pk == null) { Util.Error("Attempted to load a null file."); return; }

            if (pk.Format > SAV.Generation)
            { Util.Alert("Can't load future generation files."); return; }

            bool oldInit = fieldsInitialized;
            fieldsInitialized = fieldsLoaded = false;
            if (focus)
                Tab_Main.Focus();

            pkm = pk.Clone();

            if (fieldsInitialized & !pkm.ChecksumValid)
                Util.Alert("PKX File has an invalid checksum.");

            if (pkm.Format != SAV.Generation) // past gen format
            {
                string c;
                pkm = PKMConverter.convertToFormat(pkm, SAV.Generation, out c);
                if (pk.Format != pkm.Format && focus) // converted
                    Util.Alert("Converted File.");
            }

            getFieldsfromPKM();

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
            int species = Util.getIndex(CB_Species);
            if (SAV.Generation < 4 && species != 201)
            {
                Label_Form.Visible = CB_Form.Visible = CB_Form.Enabled = false;
                return;
            }

            bool hasForms = SAV.Personal[species].HasFormes || new[] { 201, 664, 665, 414 }.Contains(species);
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

            int[] abils = SAV.Personal.getAbilities(species, formnum);
            string[] abilIdentifier = {" (1)", " (2)", " (H)"};
            List<string> ability_list = abils.Where(a => a != 0).Select((t, i) => abilitylist[t] + abilIdentifier[i]).ToList();
            if (!ability_list.Any())
                ability_list.Add(abilitylist[0] + abilIdentifier[0]);

            int abil = CB_Ability.SelectedIndex;
            CB_Ability.DataSource = ability_list;
            CB_Ability.SelectedIndex = abil < 0 || abil >= CB_Ability.Items.Count ? 0 : abil;
        }
        // PKX Data Calculation Functions //
        private void setIsShiny(object sender)
        {
            if (sender == TB_PID)
                pkm.PID = Util.getHEXval(TB_PID.Text);
            else if (sender == TB_TID)
                pkm.TID = (int)Util.ToUInt32(TB_TID.Text);
            else if (sender == TB_SID)
                pkm.SID = (int)Util.ToUInt32(TB_SID.Text);

            bool isShiny = pkm.IsShiny;

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
                TB_Friendship.Text = TB_Friendship.Text == "255" ? SAV.Personal[pkm.Species].BaseFriendship.ToString() : "255";
        }
        private void clickGender(object sender, EventArgs e)
        {
            // Get Gender Threshold
            int gt = SAV.Personal[Util.getIndex(CB_Species)].Gender;

            if (gt == 255 || gt == 0 || gt == 254) // Single gender/genderless
                return;

            if (gt >= 255) return; 
            // If not a single gender(less) species: (should be <254 but whatever, 255 never happens)

            int newGender = PKX.getGender(Label_Gender.Text) ^ 1;
            if (SAV.Generation <= 4)
            {
                pkm.Species = Util.getIndex(CB_Species);
                pkm.Version = Util.getIndex(CB_GameOrigin);
                pkm.Nature = Util.getIndex(CB_Nature);
                pkm.AltForm = CB_Form.SelectedIndex;

                pkm.setPIDGender(newGender);
                TB_PID.Text = pkm.PID.ToString("X8");
            }
            pkm.Gender = newGender;
            Label_Gender.Text = gendersymbols[pkm.Gender];
            Label_Gender.ForeColor = pkm.Gender == 2 ? Label_Species.ForeColor : (pkm.Gender == 1 ? Color.Red : Color.Blue);


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
                ((MaskedTextBox) sender).Text = 31.ToString();
            else if (ModifierKeys == Keys.Alt)
                ((MaskedTextBox) sender).Text = 0.ToString();
        }
        private void clickEV(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // EV
                ((MaskedTextBox) sender).Text = Math.Min(Math.Max(510 - Util.ToInt32(TB_EVTotal.Text) + Util.ToInt32((sender as MaskedTextBox).Text), 0), 252).ToString();
            else if (ModifierKeys == Keys.Alt)
                ((MaskedTextBox) sender).Text = 0.ToString();
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
            if (fieldsLoaded)
                pkm.EXP = Util.ToUInt32(TB_EXP.Text);
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
            if (sender != null && Util.ToInt32(((MaskedTextBox) sender).Text) > 31)
                ((MaskedTextBox) sender).Text = "31";

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
            if (sender is MaskedTextBox)
            {
                MaskedTextBox m = (MaskedTextBox)sender;
                if (Util.ToInt32(m.Text) > SAV.MaxEV)
                { m.Text = SAV.MaxEV.ToString(); return; } // recursive on text set
            }

            changingFields = true;
            if (sender == TB_HPEV) pkm.EV_HP = Util.ToInt32(TB_HPEV.Text);
            else if (sender == TB_ATKEV) pkm.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            else if (sender == TB_DEFEV) pkm.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            else if (sender == TB_SPEEV) pkm.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            else if (sender == TB_SPAEV) pkm.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            else if (sender == TB_SPDEV) pkm.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            int evtotal = pkm.EVs.Sum();

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
            if (fieldsLoaded)
            {
                pkm.Version = Util.getIndex(CB_GameOrigin);
                pkm.PID = Util.getHEXval(TB_PID.Text);
                pkm.Species = Util.getIndex(CB_Species);
                pkm.Nature = Util.getIndex(CB_Nature);
                pkm.AltForm = CB_Form.SelectedIndex;
            }
            if (sender == Label_Gender)
                pkm.setPIDGender(pkm.Gender);
            else if (sender == CB_Nature && pkm.Nature != Util.getIndex(CB_Nature))
                pkm.setPIDNature(Util.getIndex(CB_Nature));
            else if (sender == BTN_RerollPID)
                pkm.setPIDGender(pkm.Gender);
            TB_PID.Text = pkm.PID.ToString("X8");
            getQuickFiller(dragout);
            if (pkm.GenNumber < 6 && TB_EC.Visible)
                TB_EC.Text = TB_PID.Text;
        }
        private void updateRandomEC(object sender, EventArgs e)
        {
            pkm.Version = Util.getIndex(CB_GameOrigin);
            if (pkm.GenNumber < 6)
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
            if (Util.ToInt32(((MaskedTextBox) sender).Text) > byte.MaxValue)
                    ((MaskedTextBox) sender).Text = "255";
        }
        private void updateForm(object sender, EventArgs e)
        {
            if (CB_Form == sender && fieldsLoaded)
                pkm.AltForm = CB_Form.SelectedIndex;
            updateStats();
            // Repopulate Abilities if Species Form has different abilities
            setAbilityList();

            // Gender Forms
            if (Util.getIndex(CB_Species) == 201)
            {
                if (fieldsLoaded && SAV.Generation == 3)
                    updateRandomPID(sender, e); // Fix AltForm
            }
            else if (PKX.getGender(CB_Form.Text) < 2)
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
            int form = pkm.AltForm = Util.ToInt32(MT_Form.Text);
            CB_Form.SelectedIndex = CB_Form.Items.Count > form ? form : -1;
            changingFields = false;
        }
        private void updatePP(object sender, EventArgs e)
        {
            TB_PP1.Text = pkm.getMovePP(Util.getIndex(CB_Move1), CB_PPu1.SelectedIndex).ToString();
            TB_PP2.Text = pkm.getMovePP(Util.getIndex(CB_Move2), CB_PPu2.SelectedIndex).ToString();
            TB_PP3.Text = pkm.getMovePP(Util.getIndex(CB_Move3), CB_PPu3.SelectedIndex).ToString();
            TB_PP4.Text = pkm.getMovePP(Util.getIndex(CB_Move4), CB_PPu4.SelectedIndex).ToString();
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
            int gt = SAV.Personal[Species].Gender;
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
            
            if (SAV.Generation == 3 && origintrack != "Gen3")
            {
                var met_list = Util.getCBList(metRSEFRLG_00000, Enumerable.Range(0, 213).ToArray());
                met_list = Util.getOffsetCBList(met_list, metRSEFRLG_00000, 00000, new[] {253, 254, 255});
                origintrack = "Gen3";
                CB_MetLocation.DisplayMember = "Text";
                CB_MetLocation.ValueMember = "Value";
                CB_MetLocation.DataSource = met_list;
                CB_MetLocation.SelectedValue = 0;
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
                CB_EggLocation.DisplayMember = "Text";
                CB_EggLocation.ValueMember = "Value";
                CB_EggLocation.DataSource = new BindingSource(met_list, null);
                CB_EggLocation.SelectedValue = 0;
                CB_MetLocation.SelectedValue = 0;
                origintrack = "Gen4";
            }
            else if (Version < 24 && origintrack != "Past" && SAV.Generation >= 5)
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

            if (SAV.Generation >= 4 && Version < 0x10 && origintrack != "Gen4")
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
            if (SAV.Generation >= 4)
            {
                bool g4 = Version >= 7 && Version <= 12 && Version != 9;
                CB_EncounterType.Visible = Label_EncounterType.Visible = g4;
                if (!g4)
                    CB_EncounterType.SelectedValue = 0;
            }

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
            if (Util.ToInt32(((MaskedTextBox) sender).Text) > byte.MaxValue)
                ((MaskedTextBox) sender).Text = "255";

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
            TextBox tb = !(sender is TextBox) ? TB_Nickname : (TextBox) sender;
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

                TB_Friendship.Text = SAV.Personal[Util.getIndex(CB_Species)].BaseFriendship.ToString();

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
            pkm.TID = Util.ToInt32(TB_TID.Text);
            pkm.SID = Util.ToInt32(TB_SID.Text);
            pkm.PID = Util.getHEXval(TB_PID.Text);
            pkm.Nature = Util.getIndex(CB_Nature);
            pkm.Gender = PKX.getGender(Label_Gender.Text);
            pkm.AltForm = CB_Form.SelectedIndex;
            pkm.Version = Util.getIndex(CB_GameOrigin);

            pkm.setShinyPID();
            TB_PID.Text = pkm.PID.ToString("X8");

            if (pkm.GenNumber < 6 && TB_EC.Visible)
                TB_EC.Text = TB_PID.Text;

            getQuickFiller(dragout);
            updateLegality();
        }
        private void updateTSV(object sender, EventArgs e)
        {
            if (SAV.Generation < 6)
                return;

            var TSV = pkm.TSV;
            Tip1.SetToolTip(TB_TID, "TSV: " + TSV.ToString("0000"));
            Tip2.SetToolTip(TB_SID, "TSV: " + TSV.ToString("0000"));

            pkm.PID = Util.getHEXval(TB_PID.Text);
            var PSV = pkm.PSV;
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

            setIsShiny(sender);
            getQuickFiller(dragout);
            updateIVs(null, null);   // If the EC is changed, EC%6 (Characteristic) might be changed. 
            TB_PID.Select(60, 0);   // position cursor at end of field
            if (SAV.Generation <= 4 && fieldsLoaded)
            {
                fieldsLoaded = false;
                pkm.PID = Util.getHEXval(TB_PID.Text);
                CB_Nature.SelectedValue = pkm.Nature;
                Label_Gender.Text = gendersymbols[pkm.Gender];
                Label_Gender.ForeColor = pkm.Gender == 2 ? Label_Species.ForeColor : (pkm.Gender == 1 ? Color.Red : Color.Blue);
                fieldsLoaded = true;
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
            if (!fieldsInitialized)
                return;
            validateComboBox(sender, e);
            if (sender == CB_Ability)
                TB_AbilityNumber.Text = (1 << CB_Ability.SelectedIndex).ToString();
            if (fieldsLoaded && sender == CB_Nature && SAV.Generation <= 4)
                updateRandomPID(sender, e);
            updateNatureModification(sender, null);
            updateIVs(null, null); // updating Nature will trigger stats to update as well
        }
        private void validateMove(object sender, EventArgs e)
        {
            if (!fieldsInitialized)
                return;
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
                ((PK6) pkm).RelearnMoves = new[] { Util.getIndex(CB_RelearnMove1), Util.getIndex(CB_RelearnMove2), Util.getIndex(CB_RelearnMove3), Util.getIndex(CB_RelearnMove4) };
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
            Legality = la ?? new LegalityAnalysis((PK6) pkm);
            PB_Legal.Image = Legality.Valid ? Properties.Resources.valid : Properties.Resources.warn;
            PB_Legal.Visible = pkm.Gen6 /*&& pkm is PK6*/ && !HaX;

            // Refresh Move Legality
            for (int i = 0; i < 4; i++)
                movePB[i].Visible = !Legality.vMoves[i].Valid && !HaX;
            for (int i = 0; i < 4; i++)
                relearnPB[i].Visible = !Legality.vRelearn[i].Valid && !HaX;
        }
        private void updateStats()
        {
            // Generate the stats.
            ushort[] stats = pkm.getStats(SAV.Personal.getFormeEntry(pkm.Species, pkm.AltForm));

            Stat_HP.Text = stats[0].ToString();
            Stat_ATK.Text = stats[1].ToString();
            Stat_DEF.Text = stats[2].ToString();
            Stat_SPA.Text = stats[4].ToString();
            Stat_SPD.Text = stats[5].ToString();
            Stat_SPE.Text = stats[3].ToString();

            // Recolor the Stat Labels based on boosted stats.
            {
                int incr = pkm.Nature / 5;
                int decr = pkm.Nature % 5;

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
            new RibbonEditor().ShowDialog();
        }
        private void openMedals(object sender, EventArgs e)
        {
            new SuperTrainingEditor().ShowDialog();
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
                if (!pk.CanHoldItem(SAV.HeldItems))
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

            PKM pk = getPKMfromFields();
            return pk?.Clone();
        }
        // Drag & Drop Events
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
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

            bool dsv = Path.GetExtension(main.FileName)?.ToLower() == ".dsv";
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
            if (!SAV.HasBox)
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
            {
                // If slot isn't overwriting existing PKM, make it write to the lowest empty PKM slot
                if (SAV.PartyCount < slot + 1 - 30)
                { slot = SAV.PartyCount + 30; offset = getPKXOffset(slot); }
                SAV.setPartySlot(pk, offset);
                setParty();
                getSlotColor(slot, Properties.Resources.slotSet);
            }
            else if (slot < 30 || HaX && slot >= 36 && slot < 42)
            {
                SAV.setStoredSlot(pk, offset);
                getQuickFiller(SlotPictureBoxes[slot], pk);
                getSlotColor(slot, Properties.Resources.slotSet);
            }
        }
        private void clickDelete(object sender, EventArgs e)
        {
            int slot = getSlot(sender);
            if (slot == 30 && SAV.PartyCount == 1 && !HaX) { Util.Alert("Can't delete first slot."); return; }

            int offset = getPKXOffset(slot);
            if (offset < 0)
            {
                Util.Error($"Slot read error @ slot {slot}.");
                return;
            }
            if (slot >= 30 && slot < 36) // Party
            {
                SAV.deletePartySlot(slot-30);
                setParty();
                getSlotColor(slot, Properties.Resources.slotDel);
                return;
            }
            if (slot < 30 || HaX && slot >= 36 && slot < 42)
            { SAV.setStoredSlot(SAV.BlankPKM, getPKXOffset(slot)); }
            else return;

            SlotPictureBoxes[slot].Image = null;
            getSlotColor(slot, Properties.Resources.slotDel);
        }
        private void clickClone(object sender, EventArgs e)
        {
            if (getSlot(sender) > 30) return; // only perform action if cloning to boxes
            if (!verifiedPKM()) return; // don't copy garbage to the box

            PKM pk;
            if (Util.Prompt(MessageBoxButtons.YesNo, $"Clone Pokemon from Editing Tabs to all slots in {CB_BoxSelect.Text}?") == DialogResult.Yes)
                pk = preparePKM();
            else if (Util.Prompt(MessageBoxButtons.YesNo, $"Delete all Pokemon in {CB_BoxSelect.Text}?") == DialogResult.Yes)
                pk = SAV.BlankPKM;
            else
                return;

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
            if (SAV.Generation == 6)
            {
                SAV6 sav6 = (SAV6) SAV;
                if (tb == TB_GameSync)
                    oldval = sav6.GameSyncID;
                else if (tb == TB_Secure1)
                    oldval = sav6.Secure1;
                else if (tb == TB_Secure2)
                    oldval = sav6.Secure2;
            }

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
            if (newval == oldval) return;

            if (SAV.Generation == 6)
            {
                SAV6 sav6 = (SAV6) SAV;
                if (tb == TB_GameSync)
                    sav6.GameSyncID = newval;
                else if (tb == TB_Secure1)
                    sav6.Secure1 = newval;
                else if (tb == TB_Secure2)
                    sav6.Secure2 = newval;
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
            {
                for (int i = 0; i < party.Length; i++)
                    getQuickFiller(SlotPictureBoxes[i + 30], party[i]);
                for (int i = party.Length; i < 6; i++)
                    SlotPictureBoxes[i + 30].Image = null;
            }
            if (SAV.HasBattleBox)
            {
                for (int i = 0; i < battle.Length; i++)
                    getQuickFiller(SlotPictureBoxes[i + 36], battle[i]);
                for (int i = battle.Length; i < 6; i++)
                    SlotPictureBoxes[i + 36].Image = null;
            }
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
                int boxbgval = SAV.getBoxWallpaper(CB_BoxSelect.SelectedIndex);
                PAN_Box.BackgroundImage = BoxWallpaper.getWallpaper(SAV, boxbgval);

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
            var yn = Menu_ModifyPKM.Checked ? "Yes" : "No";
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
                }.CopyTo(SAV.Data, ((SAV6) SAV).OPower);
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
            int offset = ((SAV6) SAV).PSS;
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
        private void B_LinkInfo_Click(object sender, EventArgs e)
        {
            new SAV_Link6().ShowDialog();
        }
        private void B_CGearSkin_Click(object sender, EventArgs e)
        {
            new SAV_CGearSkin().ShowDialog();
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
            string path = SaveUtil.detectSaveFile();
            if (path == null || !File.Exists(path)) return;
            if (Util.Prompt(MessageBoxButtons.YesNo, "Open save file from the following location?", path) == DialogResult.Yes)
                openQuick(path); // load save
        }

        // Drag and drop related functions
        private void pbBoxSlot_MouseClick(object sender, MouseEventArgs e)
        {
            if (slotDragDropInProgress)
                return;
            
            clickSlot(sender, e);
        }
        private void pbBoxSlot_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                slotLeftMouseIsDown = false;
            if (e.Button == MouseButtons.Right)
                slotRightMouseIsDown = false;
        }
        private void pbBoxSlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                slotLeftMouseIsDown = true;
            if (e.Button == MouseButtons.Right)
                slotRightMouseIsDown = true;
        }
        private void pbBoxSlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (slotDragDropInProgress)
                return;

            if (slotLeftMouseIsDown)
            {
                // The goal is to create a temporary PKX file for the underlying Pokemon
                // and use that file to perform a drag drop operation.

                // Abort if there is no Pokemon in the given slot.
                if (((PictureBox)sender).Image == null)
                    return;

                // Set flag to prevent re-entering.
                slotDragDropInProgress = true;

                slotSourceSlotNumber = getSlot(sender);
                int offset = getPKXOffset(slotSourceSlotNumber);

                // Prepare Data
                slotPkmSource = SAV.getData(offset, SAV.SIZE_STORED);
                slotSourceOffset = offset;

                // Make a new file name based off the PID
                byte[] dragdata = SAV.decryptPKM(slotPkmSource);
                Array.Resize(ref dragdata, SAV.SIZE_STORED);
                PKM pkx = SAV.getPKM(dragdata);
                string filename = pkx.FileName;

                // Make File
                string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
                try
                {
                    File.WriteAllBytes(newfile, dragdata);
                    // Thread Blocks on DoDragDrop
                    ((PictureBox)sender).DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
                }
                catch (Exception x)
                {
                    Util.Error("Drag & Drop Error:", x.ToString());
                }
                slotSourceOffset = 0;

                // Browser apps need time to load data since the file isn't moved to a location on the user's local storage.
                // Tested 10ms -> too quick, 100ms was fine. 500ms should be safe?
                new Thread(() =>
                {
                    Thread.Sleep(500);
                    if (File.Exists(newfile))
                        File.Delete(newfile);
                }).Start();
            }
        }
        private void pbBoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            int slotDestinationSlotNumber = getSlot(sender);
            int slotDestinationOffset = getPKXOffset(slotDestinationSlotNumber);

            // Check for In-Dropped files (PKX,SAV,ETC)
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (Directory.Exists(files[0])) { loadBoxesFromDB(files[0]); return; }
            if (slotSourceOffset == 0)
            {
                if (files.Length <= 0)
                    return;
                string file = files[0];
                FileInfo fi = new FileInfo(file);
                if (!PKX.getIsPKM(fi.Length) && !MysteryGift.getIsMysteryGift(fi.Length))
                { openQuick(file); return; }

                byte[] data = File.ReadAllBytes(file);
                MysteryGift mg = MysteryGift.getMysteryGift(data, fi.Extension);
                PKM temp = mg != null ? mg.convertToPKM(SAV) : PKMConverter.getPKMfromBytes(data);
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

                SAV.setStoredSlot(pk, slotDestinationOffset);
                getQuickFiller(SlotPictureBoxes[slotDestinationSlotNumber], pk);
                getSlotColor(slotDestinationSlotNumber, Properties.Resources.slotSet);
                Console.WriteLine(c);
            }
            else
            {
                PKM pkz = SAV.getStoredSlot(slotSourceOffset);
                if (ModifierKeys == Keys.Alt && slotDestinationSlotNumber > -1) // overwrite delete old slot
                {
                    // Clear from slot 
                    getQuickFiller(SlotPictureBoxes[slotSourceSlotNumber], SAV.BlankPKM); // picturebox
                    SAV.setStoredSlot(SAV.BlankPKM, slotSourceOffset); // savefile
                }
                else if (ModifierKeys != Keys.Control && slotDestinationSlotNumber > -1)
                {
                    if (((PictureBox)sender).Image != null)
                    {
                        // Load data from destination
                        PKM pk = SAV.getStoredSlot(slotDestinationOffset);

                        // Set destination pokemon image to source picture box
                        getQuickFiller(SlotPictureBoxes[slotSourceSlotNumber], pk);

                        // Set destination pokemon data to source slot
                        SAV.setStoredSlot(pk, slotSourceOffset);
                    }
                    else
                    {
                        // Set blank to source slot
                        SAV.setStoredSlot(SAV.BlankPKM, slotSourceOffset);
                        SlotPictureBoxes[slotSourceSlotNumber].Image = null;
                    }
                }
                // Copy from temp to destination slot.
                SAV.setStoredSlot(pkz, slotDestinationOffset);
                getQuickFiller(SlotPictureBoxes[slotDestinationSlotNumber], pkz);

                slotSourceOffset = 0; // Clear offset value
            }
        }
        private void pbBoxSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }
        private void pbBoxSlot_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.Action == DragAction.Drop)
            {
                slotLeftMouseIsDown = false;
                slotRightMouseIsDown = false;
                slotDragDropInProgress = false;
            }
        }

        private static bool slotLeftMouseIsDown = false;
        private static bool slotRightMouseIsDown = false;
        private static bool slotDragDropInProgress = false;
        private byte[] slotPkmSource;
        private int slotSourceOffset;
        private int slotSourceSlotNumber = -1;
        #endregion
    }
}
