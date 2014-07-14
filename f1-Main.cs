using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace PKHeX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            #region Pop open a splash screen while we load up.
            Thread SplashSCR = new Thread(() => new SplashScreen(this).ShowDialog());
            SplashSCR.Start();
            #endregion
            #region Initialize Form
            InitializeComponent();
            // Get Default White Color (for some users who have different color schemes
            defaultwhite = TB_PID.BackColor;
            CB_ExtraBytes.SelectedIndex = 0;

            // Resize Main Window to PKX Editing Mode
            // this.Size = new Size(260, 390);
            Width = (Width * (26000 / 540)) / 100 + 1;
            #endregion
            #region Language Detection before loading
            // Set up Language Selection
            string[] main_langlist = new string[]
            {
                "English", // ENG
                "日本語", // JPN
                "Français", // FRE
                "Italiano", // ITA
                "Deutsch", // GER
                "Español", // SPA
                "한국어", // KOR
            };
            int[] main_langnum = new int[]
            { 2,1,3,4,5,7,8 };
            string[] lang_val = { "en", "ja", "fr", "it", "de", "es", "ko" };
            
            for (int i = 0; i < main_langlist.Length; i++)
            {
                cbItem item = new cbItem();
                item.Text = main_langlist[i];
                item.Value = main_langnum[i];

                CB_MainLanguage.Items.Add(item);
            }
            // Try and detect the language
            string filename = Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string lastTwoChars = filename.Substring(filename.Length - 2);
            int lang = Array.IndexOf(lang_val, lastTwoChars);
            if (lang >= 0)
            {
                CB_MainLanguage.SelectedIndex = main_langnum[lang];
            }
            else
            {
                CB_MainLanguage.SelectedIndex = 0;
            }

            #endregion
            #region Localize & Populate
            InitializeStrings();
            // Initialize Fields
            InitializeFields();
            #endregion
            # region Add ContextMenus to the PictureBoxes (PKX slots)
            PictureBox[] pba = {
                                   bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                   bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                   bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                   bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                   bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,
                               };
            ContextMenuStrip mnu = new ContextMenuStrip();
            ToolStripMenuItem mnuView = new ToolStripMenuItem("View"); 
            ToolStripMenuItem mnuSet = new ToolStripMenuItem("Set");
            ToolStripMenuItem mnuDelete = new ToolStripMenuItem("Delete");
            //Assign event handlers
            mnuView.Click += new EventHandler(rcmView_Click);
            mnuSet.Click += new EventHandler(rcmSet_Click);
            mnuDelete.Click += new EventHandler(rcmDelete_Click);
            //Add to main context menu
            mnu.Items.AddRange(new ToolStripItem[] { mnuView, mnuSet, mnuDelete });
            //Assign to datagridview
            for (int i = 0; i < pba.Length; i++)
            {
                pba[i].ContextMenuStrip = mnu;
            }
            // Add ContextMenus to the PictureBoxes that are read only
            PictureBox[] pba2 = {
                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                               };
            ContextMenuStrip mnu2 = new ContextMenuStrip();
            ToolStripMenuItem mnu2View = new ToolStripMenuItem("View");
            //Assign event handlers
            mnu2View.Click += new EventHandler(rcmView_Click);
            //Add to main context menu
            mnu2.Items.AddRange(new ToolStripItem[] { mnu2View });
            //Assign to datagridview
            for (int i = 0; i < pba2.Length; i++)
            {
                pba2[i].ContextMenuStrip = mnu2;
            }
            #endregion
            #region Enable Drag and Drop on the form & tab control.
            this.tabMain.AllowDrop = true;
            this.DragEnter += new DragEventHandler(tabMain_DragEnter);
            this.DragDrop += new DragEventHandler(tabMain_DragDrop);

            // Enable Drag and Drop on each tab.
            this.tabMain.DragEnter += new DragEventHandler(tabMain_DragEnter);
            this.tabMain.DragDrop += new DragEventHandler(tabMain_DragDrop);

            TabPage[] tca = { Tab_Main, Tab_Met, Tab_Stats, Tab_Attacks, Tab_OTMisc };
            for (int i = 0; i < tca.Length; i++)
            {
                tca[i].DragEnter += new DragEventHandler(tabMain_DragEnter);
                tca[i].DragDrop += new DragEventHandler(tabMain_DragDrop);
            } 

            // Export D&D
            dragout.MouseDown += new MouseEventHandler(dragout_MouseDown);
            dragout.DragOver += new DragEventHandler(dragout_DragOver);
            eragout.MouseDown += new MouseEventHandler(dragout_MouseDown);
            eragout.DragOver += new DragEventHandler(dragout_DragOver);

            // ToolTips for Drag&Drop
            ToolTip dragoutTip1 = new ToolTip();
            ToolTip dragoutTip2 = new ToolTip();
            dragoutTip1.SetToolTip(dragout, "PK6 QuickSave");
            dragoutTip2.SetToolTip(eragout, "EK6 QuickSave");
            #endregion
            #region Finish Up

            // Default Selected Items
            C_BoxSelect.SelectedIndex = 0;
            CB_PPu1.SelectedIndex = CB_PPu2.SelectedIndex = CB_PPu3.SelectedIndex = CB_PPu4.SelectedIndex = 0;
            CB_Ball.SelectedIndex = 0;

            // Close splash screen.  
            init = true;          
            SplashSCR.Join();
            this.BringToFront();
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            #endregion
        }

        #region Global Variables: Always Visible!
        public byte[] buff = new Byte[260];
        public byte[] savefile = new Byte[0x100000];
        public int gt = 258;
        public int genderflag, species;
        public int savindex;
        public bool savedited;
        public int colorizedbox = 32;
        public Color colorizedcolor = Color.Transparent;
        public Color defaultwhite = Color.White;
        public int colorizedslot = 0;
        public string eggname = "";
        public string[] specieslist = { };
        public string[] movelist = { };
        public string[] itemlist = { };
        public string[] abilitylist = { };
        public string[] types = { };
        public string[] natures = { };
        public string[] characteristics = { };
        public string[] memories = { };
        public string[] genloc = { };
        public string[] forms = { };
        public string[] metHGSS_00000 = { };
        public string[] metHGSS_02000 = { };
        public string[] metHGSS_03000 = { };
        public string[] metBW2_00000 = { };
        public string[] metBW2_30000 = { };
        public string[] metBW2_40000 = { };
        public string[] metBW2_60000 = { };
        public string[] metXY_00000 = { };
        public string[] metXY_30000 = { };
        public string[] metXY_40000 = { };
        public string[] metXY_60000 = { };
        public int[] speciesability = { };
        public int[] saveoffsets = { };
        public string origintrack;
        public string curlanguage;
        public volatile bool init = false;
        public ToolTip Tip1 = new ToolTip();
        public ToolTip Tip2 = new ToolTip();
        public ToolTip Tip3 = new ToolTip();
        public SaveGames.SaveStruct SaveGame = new SaveGames.SaveStruct("XY");
        #endregion

        // String Trim //   
        public string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            if (index < 0)
                return input;

            return input.Substring(0, index);
        }
        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        #region //// PKX WINDOW FUNCTIONS ////
        // Randomization //
        private static uint LCRNG(uint seed)
        {
            uint a = 0x41C64E6D;
            uint c = 0x00006073;

            seed = (seed * a + c) & 0xFFFFFFFF;
            return seed;
        }
        private static Random rand = new Random();
        private static uint rnd32()
        {
            return (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
        }
        // Index Tables //
        static DataTable NatureTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Nature", typeof(int));
            table.Columns.Add("A", typeof(int));
            table.Columns.Add("D", typeof(int));
            table.Columns.Add("Spe", typeof(int));
            table.Columns.Add("SpA", typeof(int));
            table.Columns.Add("SpD", typeof(int));

            table.Rows.Add(0, 10, 10, 10, 10, 10);
            table.Rows.Add(1, 11, 9, 10, 10, 10);
            table.Rows.Add(2, 11, 10, 9, 10, 10);
            table.Rows.Add(3, 11, 10, 10, 9, 10);
            table.Rows.Add(4, 11, 10, 10, 10, 9);
            table.Rows.Add(5, 9, 11, 10, 10, 10);
            table.Rows.Add(6, 10, 10, 10, 10, 10);
            table.Rows.Add(7, 10, 11, 9, 10, 10);
            table.Rows.Add(8, 10, 11, 10, 9, 10);
            table.Rows.Add(9, 10, 11, 10, 10, 9);
            table.Rows.Add(10, 9, 10, 11, 10, 10);
            table.Rows.Add(11, 10, 9, 11, 10, 10);
            table.Rows.Add(12, 10, 10, 10, 10, 10);
            table.Rows.Add(13, 10, 10, 11, 9, 10);
            table.Rows.Add(14, 10, 10, 11, 10, 9);
            table.Rows.Add(15, 9, 10, 10, 11, 10);
            table.Rows.Add(16, 10, 9, 10, 11, 10);
            table.Rows.Add(17, 10, 10, 9, 11, 10);
            table.Rows.Add(18, 10, 10, 10, 10, 10);
            table.Rows.Add(19, 10, 10, 10, 11, 9);
            table.Rows.Add(20, 9, 10, 10, 10, 11);
            table.Rows.Add(21, 10, 9, 10, 10, 11);
            table.Rows.Add(22, 10, 10, 9, 10, 11);
            table.Rows.Add(23, 10, 10, 10, 9, 11);
            table.Rows.Add(24, 10, 10, 10, 10, 10);
            return table;
        }
        static DataTable Friendship()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Species", typeof(int));
            table.Columns.Add("Friendship", typeof(int));

            table.Rows.Add(0, 0);
            table.Rows.Add(1, 70);
            table.Rows.Add(2, 70);
            table.Rows.Add(3, 70);
            table.Rows.Add(4, 70);
            table.Rows.Add(5, 70);
            table.Rows.Add(6, 70);
            table.Rows.Add(7, 70);
            table.Rows.Add(8, 70);
            table.Rows.Add(9, 70);
            table.Rows.Add(10, 70);
            table.Rows.Add(11, 70);
            table.Rows.Add(12, 70);
            table.Rows.Add(13, 70);
            table.Rows.Add(14, 70);
            table.Rows.Add(15, 70);
            table.Rows.Add(16, 70);
            table.Rows.Add(17, 70);
            table.Rows.Add(18, 70);
            table.Rows.Add(19, 70);
            table.Rows.Add(20, 70);
            table.Rows.Add(21, 70);
            table.Rows.Add(22, 70);
            table.Rows.Add(23, 70);
            table.Rows.Add(24, 70);
            table.Rows.Add(25, 70);
            table.Rows.Add(26, 70);
            table.Rows.Add(27, 70);
            table.Rows.Add(28, 70);
            table.Rows.Add(29, 70);
            table.Rows.Add(30, 70);
            table.Rows.Add(31, 70);
            table.Rows.Add(32, 70);
            table.Rows.Add(33, 70);
            table.Rows.Add(34, 70);
            table.Rows.Add(35, 140);
            table.Rows.Add(36, 140);
            table.Rows.Add(37, 70);
            table.Rows.Add(38, 70);
            table.Rows.Add(39, 70);
            table.Rows.Add(40, 70);
            table.Rows.Add(41, 70);
            table.Rows.Add(42, 70);
            table.Rows.Add(43, 70);
            table.Rows.Add(44, 70);
            table.Rows.Add(45, 70);
            table.Rows.Add(46, 70);
            table.Rows.Add(47, 70);
            table.Rows.Add(48, 70);
            table.Rows.Add(49, 70);
            table.Rows.Add(50, 70);
            table.Rows.Add(51, 70);
            table.Rows.Add(52, 70);
            table.Rows.Add(53, 70);
            table.Rows.Add(54, 70);
            table.Rows.Add(55, 70);
            table.Rows.Add(56, 70);
            table.Rows.Add(57, 70);
            table.Rows.Add(58, 70);
            table.Rows.Add(59, 70);
            table.Rows.Add(60, 70);
            table.Rows.Add(61, 70);
            table.Rows.Add(62, 70);
            table.Rows.Add(63, 70);
            table.Rows.Add(64, 70);
            table.Rows.Add(65, 70);
            table.Rows.Add(66, 70);
            table.Rows.Add(67, 70);
            table.Rows.Add(68, 70);
            table.Rows.Add(69, 70);
            table.Rows.Add(70, 70);
            table.Rows.Add(71, 70);
            table.Rows.Add(72, 70);
            table.Rows.Add(73, 70);
            table.Rows.Add(74, 70);
            table.Rows.Add(75, 70);
            table.Rows.Add(76, 70);
            table.Rows.Add(77, 70);
            table.Rows.Add(78, 70);
            table.Rows.Add(79, 70);
            table.Rows.Add(80, 70);
            table.Rows.Add(81, 70);
            table.Rows.Add(82, 70);
            table.Rows.Add(83, 70);
            table.Rows.Add(84, 70);
            table.Rows.Add(85, 70);
            table.Rows.Add(86, 70);
            table.Rows.Add(87, 70);
            table.Rows.Add(88, 70);
            table.Rows.Add(89, 70);
            table.Rows.Add(90, 70);
            table.Rows.Add(91, 70);
            table.Rows.Add(92, 70);
            table.Rows.Add(93, 70);
            table.Rows.Add(94, 70);
            table.Rows.Add(95, 70);
            table.Rows.Add(96, 70);
            table.Rows.Add(97, 70);
            table.Rows.Add(98, 70);
            table.Rows.Add(99, 70);
            table.Rows.Add(100, 70);
            table.Rows.Add(101, 70);
            table.Rows.Add(102, 70);
            table.Rows.Add(103, 70);
            table.Rows.Add(104, 70);
            table.Rows.Add(105, 70);
            table.Rows.Add(106, 70);
            table.Rows.Add(107, 70);
            table.Rows.Add(108, 70);
            table.Rows.Add(109, 70);
            table.Rows.Add(110, 70);
            table.Rows.Add(111, 70);
            table.Rows.Add(112, 70);
            table.Rows.Add(113, 140);
            table.Rows.Add(114, 70);
            table.Rows.Add(115, 70);
            table.Rows.Add(116, 70);
            table.Rows.Add(117, 70);
            table.Rows.Add(118, 70);
            table.Rows.Add(119, 70);
            table.Rows.Add(120, 70);
            table.Rows.Add(121, 70);
            table.Rows.Add(122, 70);
            table.Rows.Add(123, 70);
            table.Rows.Add(124, 70);
            table.Rows.Add(125, 70);
            table.Rows.Add(126, 70);
            table.Rows.Add(127, 70);
            table.Rows.Add(128, 70);
            table.Rows.Add(129, 70);
            table.Rows.Add(130, 70);
            table.Rows.Add(131, 70);
            table.Rows.Add(132, 70);
            table.Rows.Add(133, 70);
            table.Rows.Add(134, 70);
            table.Rows.Add(135, 70);
            table.Rows.Add(136, 70);
            table.Rows.Add(137, 70);
            table.Rows.Add(138, 70);
            table.Rows.Add(139, 70);
            table.Rows.Add(140, 70);
            table.Rows.Add(141, 70);
            table.Rows.Add(142, 70);
            table.Rows.Add(143, 70);
            table.Rows.Add(144, 35);
            table.Rows.Add(145, 35);
            table.Rows.Add(146, 35);
            table.Rows.Add(147, 35);
            table.Rows.Add(148, 35);
            table.Rows.Add(149, 35);
            table.Rows.Add(150, 0);
            table.Rows.Add(151, 100);
            table.Rows.Add(152, 70);
            table.Rows.Add(153, 70);
            table.Rows.Add(154, 70);
            table.Rows.Add(155, 70);
            table.Rows.Add(156, 70);
            table.Rows.Add(157, 70);
            table.Rows.Add(158, 70);
            table.Rows.Add(159, 70);
            table.Rows.Add(160, 70);
            table.Rows.Add(161, 70);
            table.Rows.Add(162, 70);
            table.Rows.Add(163, 70);
            table.Rows.Add(164, 70);
            table.Rows.Add(165, 70);
            table.Rows.Add(166, 70);
            table.Rows.Add(167, 70);
            table.Rows.Add(168, 70);
            table.Rows.Add(169, 70);
            table.Rows.Add(170, 70);
            table.Rows.Add(171, 70);
            table.Rows.Add(172, 70);
            table.Rows.Add(173, 140);
            table.Rows.Add(174, 70);
            table.Rows.Add(175, 70);
            table.Rows.Add(176, 70);
            table.Rows.Add(177, 70);
            table.Rows.Add(178, 70);
            table.Rows.Add(179, 70);
            table.Rows.Add(180, 70);
            table.Rows.Add(181, 70);
            table.Rows.Add(182, 70);
            table.Rows.Add(183, 70);
            table.Rows.Add(184, 70);
            table.Rows.Add(185, 70);
            table.Rows.Add(186, 70);
            table.Rows.Add(187, 70);
            table.Rows.Add(188, 70);
            table.Rows.Add(189, 70);
            table.Rows.Add(190, 70);
            table.Rows.Add(191, 70);
            table.Rows.Add(192, 70);
            table.Rows.Add(193, 70);
            table.Rows.Add(194, 70);
            table.Rows.Add(195, 70);
            table.Rows.Add(196, 70);
            table.Rows.Add(197, 35);
            table.Rows.Add(198, 35);
            table.Rows.Add(199, 70);
            table.Rows.Add(200, 35);
            table.Rows.Add(201, 70);
            table.Rows.Add(202, 70);
            table.Rows.Add(203, 70);
            table.Rows.Add(204, 70);
            table.Rows.Add(205, 70);
            table.Rows.Add(206, 70);
            table.Rows.Add(207, 70);
            table.Rows.Add(208, 70);
            table.Rows.Add(209, 70);
            table.Rows.Add(210, 70);
            table.Rows.Add(211, 70);
            table.Rows.Add(212, 70);
            table.Rows.Add(213, 70);
            table.Rows.Add(214, 70);
            table.Rows.Add(215, 35);
            table.Rows.Add(216, 70);
            table.Rows.Add(217, 70);
            table.Rows.Add(218, 70);
            table.Rows.Add(219, 70);
            table.Rows.Add(220, 70);
            table.Rows.Add(221, 70);
            table.Rows.Add(222, 70);
            table.Rows.Add(223, 70);
            table.Rows.Add(224, 70);
            table.Rows.Add(225, 70);
            table.Rows.Add(226, 70);
            table.Rows.Add(227, 70);
            table.Rows.Add(228, 35);
            table.Rows.Add(229, 35);
            table.Rows.Add(230, 70);
            table.Rows.Add(231, 70);
            table.Rows.Add(232, 70);
            table.Rows.Add(233, 70);
            table.Rows.Add(234, 70);
            table.Rows.Add(235, 70);
            table.Rows.Add(236, 70);
            table.Rows.Add(237, 70);
            table.Rows.Add(238, 70);
            table.Rows.Add(239, 70);
            table.Rows.Add(240, 70);
            table.Rows.Add(241, 70);
            table.Rows.Add(242, 140);
            table.Rows.Add(243, 35);
            table.Rows.Add(244, 35);
            table.Rows.Add(245, 35);
            table.Rows.Add(246, 35);
            table.Rows.Add(247, 35);
            table.Rows.Add(248, 35);
            table.Rows.Add(249, 0);
            table.Rows.Add(250, 0);
            table.Rows.Add(251, 100);
            table.Rows.Add(252, 70);
            table.Rows.Add(253, 70);
            table.Rows.Add(254, 70);
            table.Rows.Add(255, 70);
            table.Rows.Add(256, 70);
            table.Rows.Add(257, 70);
            table.Rows.Add(258, 70);
            table.Rows.Add(259, 70);
            table.Rows.Add(260, 70);
            table.Rows.Add(261, 70);
            table.Rows.Add(262, 70);
            table.Rows.Add(263, 70);
            table.Rows.Add(264, 70);
            table.Rows.Add(265, 70);
            table.Rows.Add(266, 70);
            table.Rows.Add(267, 70);
            table.Rows.Add(268, 70);
            table.Rows.Add(269, 70);
            table.Rows.Add(270, 70);
            table.Rows.Add(271, 70);
            table.Rows.Add(272, 70);
            table.Rows.Add(273, 70);
            table.Rows.Add(274, 70);
            table.Rows.Add(275, 70);
            table.Rows.Add(276, 70);
            table.Rows.Add(277, 70);
            table.Rows.Add(278, 70);
            table.Rows.Add(279, 70);
            table.Rows.Add(280, 35);
            table.Rows.Add(281, 35);
            table.Rows.Add(282, 35);
            table.Rows.Add(283, 70);
            table.Rows.Add(284, 70);
            table.Rows.Add(285, 70);
            table.Rows.Add(286, 70);
            table.Rows.Add(287, 70);
            table.Rows.Add(288, 70);
            table.Rows.Add(289, 70);
            table.Rows.Add(290, 70);
            table.Rows.Add(291, 70);
            table.Rows.Add(292, 70);
            table.Rows.Add(293, 70);
            table.Rows.Add(294, 70);
            table.Rows.Add(295, 70);
            table.Rows.Add(296, 70);
            table.Rows.Add(297, 70);
            table.Rows.Add(298, 70);
            table.Rows.Add(299, 70);
            table.Rows.Add(300, 70);
            table.Rows.Add(301, 70);
            table.Rows.Add(302, 35);
            table.Rows.Add(303, 70);
            table.Rows.Add(304, 35);
            table.Rows.Add(305, 35);
            table.Rows.Add(306, 35);
            table.Rows.Add(307, 70);
            table.Rows.Add(308, 70);
            table.Rows.Add(309, 70);
            table.Rows.Add(310, 70);
            table.Rows.Add(311, 70);
            table.Rows.Add(312, 70);
            table.Rows.Add(313, 70);
            table.Rows.Add(314, 70);
            table.Rows.Add(315, 70);
            table.Rows.Add(316, 70);
            table.Rows.Add(317, 70);
            table.Rows.Add(318, 35);
            table.Rows.Add(319, 35);
            table.Rows.Add(320, 70);
            table.Rows.Add(321, 70);
            table.Rows.Add(322, 70);
            table.Rows.Add(323, 70);
            table.Rows.Add(324, 70);
            table.Rows.Add(325, 70);
            table.Rows.Add(326, 70);
            table.Rows.Add(327, 70);
            table.Rows.Add(328, 70);
            table.Rows.Add(329, 70);
            table.Rows.Add(330, 70);
            table.Rows.Add(331, 35);
            table.Rows.Add(332, 35);
            table.Rows.Add(333, 70);
            table.Rows.Add(334, 70);
            table.Rows.Add(335, 70);
            table.Rows.Add(336, 70);
            table.Rows.Add(337, 70);
            table.Rows.Add(338, 70);
            table.Rows.Add(339, 70);
            table.Rows.Add(340, 70);
            table.Rows.Add(341, 70);
            table.Rows.Add(342, 70);
            table.Rows.Add(343, 70);
            table.Rows.Add(344, 70);
            table.Rows.Add(345, 70);
            table.Rows.Add(346, 70);
            table.Rows.Add(347, 70);
            table.Rows.Add(348, 70);
            table.Rows.Add(349, 70);
            table.Rows.Add(350, 70);
            table.Rows.Add(351, 70);
            table.Rows.Add(352, 70);
            table.Rows.Add(353, 35);
            table.Rows.Add(354, 35);
            table.Rows.Add(355, 35);
            table.Rows.Add(356, 35);
            table.Rows.Add(357, 70);
            table.Rows.Add(358, 70);
            table.Rows.Add(359, 35);
            table.Rows.Add(360, 70);
            table.Rows.Add(361, 70);
            table.Rows.Add(362, 70);
            table.Rows.Add(363, 70);
            table.Rows.Add(364, 70);
            table.Rows.Add(365, 70);
            table.Rows.Add(366, 70);
            table.Rows.Add(367, 70);
            table.Rows.Add(368, 70);
            table.Rows.Add(369, 70);
            table.Rows.Add(370, 70);
            table.Rows.Add(371, 35);
            table.Rows.Add(372, 35);
            table.Rows.Add(373, 35);
            table.Rows.Add(374, 35);
            table.Rows.Add(375, 35);
            table.Rows.Add(376, 35);
            table.Rows.Add(377, 35);
            table.Rows.Add(378, 35);
            table.Rows.Add(379, 35);
            table.Rows.Add(380, 90);
            table.Rows.Add(381, 90);
            table.Rows.Add(382, 0);
            table.Rows.Add(383, 0);
            table.Rows.Add(384, 0);
            table.Rows.Add(385, 100);
            table.Rows.Add(386, 0);
            table.Rows.Add(387, 70);
            table.Rows.Add(388, 70);
            table.Rows.Add(389, 70);
            table.Rows.Add(390, 70);
            table.Rows.Add(391, 70);
            table.Rows.Add(392, 70);
            table.Rows.Add(393, 70);
            table.Rows.Add(394, 70);
            table.Rows.Add(395, 70);
            table.Rows.Add(396, 70);
            table.Rows.Add(397, 70);
            table.Rows.Add(398, 70);
            table.Rows.Add(399, 70);
            table.Rows.Add(400, 70);
            table.Rows.Add(401, 70);
            table.Rows.Add(402, 70);
            table.Rows.Add(403, 70);
            table.Rows.Add(404, 100);
            table.Rows.Add(405, 70);
            table.Rows.Add(406, 70);
            table.Rows.Add(407, 70);
            table.Rows.Add(408, 70);
            table.Rows.Add(409, 70);
            table.Rows.Add(410, 70);
            table.Rows.Add(411, 70);
            table.Rows.Add(412, 70);
            table.Rows.Add(413, 70);
            table.Rows.Add(414, 70);
            table.Rows.Add(415, 70);
            table.Rows.Add(416, 70);
            table.Rows.Add(417, 100);
            table.Rows.Add(418, 70);
            table.Rows.Add(419, 70);
            table.Rows.Add(420, 70);
            table.Rows.Add(421, 70);
            table.Rows.Add(422, 70);
            table.Rows.Add(423, 70);
            table.Rows.Add(424, 100);
            table.Rows.Add(425, 70);
            table.Rows.Add(426, 70);
            table.Rows.Add(427, 0);
            table.Rows.Add(428, 140);
            table.Rows.Add(429, 35);
            table.Rows.Add(430, 35);
            table.Rows.Add(431, 70);
            table.Rows.Add(432, 70);
            table.Rows.Add(433, 70);
            table.Rows.Add(434, 70);
            table.Rows.Add(435, 70);
            table.Rows.Add(436, 70);
            table.Rows.Add(437, 70);
            table.Rows.Add(438, 70);
            table.Rows.Add(439, 70);
            table.Rows.Add(440, 140);
            table.Rows.Add(441, 35);
            table.Rows.Add(442, 70);
            table.Rows.Add(443, 70);
            table.Rows.Add(444, 70);
            table.Rows.Add(445, 70);
            table.Rows.Add(446, 70);
            table.Rows.Add(447, 70);
            table.Rows.Add(448, 70);
            table.Rows.Add(449, 70);
            table.Rows.Add(450, 70);
            table.Rows.Add(451, 70);
            table.Rows.Add(452, 70);
            table.Rows.Add(453, 100);
            table.Rows.Add(454, 70);
            table.Rows.Add(455, 70);
            table.Rows.Add(456, 70);
            table.Rows.Add(457, 70);
            table.Rows.Add(458, 70);
            table.Rows.Add(459, 70);
            table.Rows.Add(460, 70);
            table.Rows.Add(461, 35);
            table.Rows.Add(462, 70);
            table.Rows.Add(463, 70);
            table.Rows.Add(464, 70);
            table.Rows.Add(465, 70);
            table.Rows.Add(466, 70);
            table.Rows.Add(467, 70);
            table.Rows.Add(468, 70);
            table.Rows.Add(469, 70);
            table.Rows.Add(470, 35);
            table.Rows.Add(471, 35);
            table.Rows.Add(472, 70);
            table.Rows.Add(473, 70);
            table.Rows.Add(474, 70);
            table.Rows.Add(475, 35);
            table.Rows.Add(476, 70);
            table.Rows.Add(477, 35);
            table.Rows.Add(478, 70);
            table.Rows.Add(479, 70);
            table.Rows.Add(480, 140);
            table.Rows.Add(481, 140);
            table.Rows.Add(482, 140);
            table.Rows.Add(483, 0);
            table.Rows.Add(484, 0);
            table.Rows.Add(485, 100);
            table.Rows.Add(486, 0);
            table.Rows.Add(487, 0);
            table.Rows.Add(488, 100);
            table.Rows.Add(489, 70);
            table.Rows.Add(490, 70);
            table.Rows.Add(491, 0);
            table.Rows.Add(492, 100);
            table.Rows.Add(493, 0);
            table.Rows.Add(494, 100);
            table.Rows.Add(495, 70);
            table.Rows.Add(496, 70);
            table.Rows.Add(497, 70);
            table.Rows.Add(498, 70);
            table.Rows.Add(499, 70);
            table.Rows.Add(500, 70);
            table.Rows.Add(501, 70);
            table.Rows.Add(502, 70);
            table.Rows.Add(503, 70);
            table.Rows.Add(504, 70);
            table.Rows.Add(505, 70);
            table.Rows.Add(506, 70);
            table.Rows.Add(507, 70);
            table.Rows.Add(508, 70);
            table.Rows.Add(509, 70);
            table.Rows.Add(510, 70);
            table.Rows.Add(511, 70);
            table.Rows.Add(512, 70);
            table.Rows.Add(513, 70);
            table.Rows.Add(514, 70);
            table.Rows.Add(515, 70);
            table.Rows.Add(516, 70);
            table.Rows.Add(517, 70);
            table.Rows.Add(518, 70);
            table.Rows.Add(519, 70);
            table.Rows.Add(520, 70);
            table.Rows.Add(521, 70);
            table.Rows.Add(522, 70);
            table.Rows.Add(523, 70);
            table.Rows.Add(524, 70);
            table.Rows.Add(525, 70);
            table.Rows.Add(526, 70);
            table.Rows.Add(527, 70);
            table.Rows.Add(528, 70);
            table.Rows.Add(529, 70);
            table.Rows.Add(530, 70);
            table.Rows.Add(531, 70);
            table.Rows.Add(532, 70);
            table.Rows.Add(533, 70);
            table.Rows.Add(534, 70);
            table.Rows.Add(535, 70);
            table.Rows.Add(536, 70);
            table.Rows.Add(537, 70);
            table.Rows.Add(538, 70);
            table.Rows.Add(539, 70);
            table.Rows.Add(540, 70);
            table.Rows.Add(541, 70);
            table.Rows.Add(542, 70);
            table.Rows.Add(543, 70);
            table.Rows.Add(544, 70);
            table.Rows.Add(545, 70);
            table.Rows.Add(546, 70);
            table.Rows.Add(547, 70);
            table.Rows.Add(548, 70);
            table.Rows.Add(549, 70);
            table.Rows.Add(550, 70);
            table.Rows.Add(551, 70);
            table.Rows.Add(552, 70);
            table.Rows.Add(553, 70);
            table.Rows.Add(554, 70);
            table.Rows.Add(555, 70);
            table.Rows.Add(556, 70);
            table.Rows.Add(557, 70);
            table.Rows.Add(558, 70);
            table.Rows.Add(559, 35);
            table.Rows.Add(560, 70);
            table.Rows.Add(561, 70);
            table.Rows.Add(562, 70);
            table.Rows.Add(563, 70);
            table.Rows.Add(564, 70);
            table.Rows.Add(565, 70);
            table.Rows.Add(566, 70);
            table.Rows.Add(567, 70);
            table.Rows.Add(568, 70);
            table.Rows.Add(569, 70);
            table.Rows.Add(570, 70);
            table.Rows.Add(571, 70);
            table.Rows.Add(572, 70);
            table.Rows.Add(573, 70);
            table.Rows.Add(574, 70);
            table.Rows.Add(575, 70);
            table.Rows.Add(576, 70);
            table.Rows.Add(577, 70);
            table.Rows.Add(578, 70);
            table.Rows.Add(579, 70);
            table.Rows.Add(580, 70);
            table.Rows.Add(581, 70);
            table.Rows.Add(582, 70);
            table.Rows.Add(583, 70);
            table.Rows.Add(584, 70);
            table.Rows.Add(585, 70);
            table.Rows.Add(586, 70);
            table.Rows.Add(587, 70);
            table.Rows.Add(588, 70);
            table.Rows.Add(589, 70);
            table.Rows.Add(590, 70);
            table.Rows.Add(591, 70);
            table.Rows.Add(592, 70);
            table.Rows.Add(593, 70);
            table.Rows.Add(594, 70);
            table.Rows.Add(595, 70);
            table.Rows.Add(596, 70);
            table.Rows.Add(597, 70);
            table.Rows.Add(598, 70);
            table.Rows.Add(599, 70);
            table.Rows.Add(600, 70);
            table.Rows.Add(601, 70);
            table.Rows.Add(602, 70);
            table.Rows.Add(603, 70);
            table.Rows.Add(604, 70);
            table.Rows.Add(605, 70);
            table.Rows.Add(606, 70);
            table.Rows.Add(607, 70);
            table.Rows.Add(608, 70);
            table.Rows.Add(609, 70);
            table.Rows.Add(610, 35);
            table.Rows.Add(611, 35);
            table.Rows.Add(612, 35);
            table.Rows.Add(613, 70);
            table.Rows.Add(614, 70);
            table.Rows.Add(615, 70);
            table.Rows.Add(616, 70);
            table.Rows.Add(617, 70);
            table.Rows.Add(618, 70);
            table.Rows.Add(619, 70);
            table.Rows.Add(620, 70);
            table.Rows.Add(621, 70);
            table.Rows.Add(622, 70);
            table.Rows.Add(623, 70);
            table.Rows.Add(624, 35);
            table.Rows.Add(625, 35);
            table.Rows.Add(626, 70);
            table.Rows.Add(627, 70);
            table.Rows.Add(628, 70);
            table.Rows.Add(629, 35);
            table.Rows.Add(630, 35);
            table.Rows.Add(631, 70);
            table.Rows.Add(632, 70);
            table.Rows.Add(633, 35);
            table.Rows.Add(634, 35);
            table.Rows.Add(635, 35);
            table.Rows.Add(636, 70);
            table.Rows.Add(637, 70);
            table.Rows.Add(638, 35);
            table.Rows.Add(639, 35);
            table.Rows.Add(640, 35);
            table.Rows.Add(641, 90);
            table.Rows.Add(642, 90);
            table.Rows.Add(643, 0);
            table.Rows.Add(644, 0);
            table.Rows.Add(645, 90);
            table.Rows.Add(646, 0);
            table.Rows.Add(647, 35);
            table.Rows.Add(648, 100);
            table.Rows.Add(649, 0);
            table.Rows.Add(650, 70);
            table.Rows.Add(651, 70);
            table.Rows.Add(652, 70);
            table.Rows.Add(653, 70);
            table.Rows.Add(654, 70);
            table.Rows.Add(655, 70);
            table.Rows.Add(656, 70);
            table.Rows.Add(657, 70);
            table.Rows.Add(658, 70);
            table.Rows.Add(659, 70);
            table.Rows.Add(660, 70);
            table.Rows.Add(661, 70);
            table.Rows.Add(662, 70);
            table.Rows.Add(663, 70);
            table.Rows.Add(664, 70);
            table.Rows.Add(665, 70);
            table.Rows.Add(666, 70);
            table.Rows.Add(667, 70);
            table.Rows.Add(668, 70);
            table.Rows.Add(669, 70);
            table.Rows.Add(670, 70);
            table.Rows.Add(671, 70);
            table.Rows.Add(672, 70);
            table.Rows.Add(673, 70);
            table.Rows.Add(674, 70);
            table.Rows.Add(675, 70);
            table.Rows.Add(676, 70);
            table.Rows.Add(677, 70);
            table.Rows.Add(678, 70);
            table.Rows.Add(679, 70);
            table.Rows.Add(680, 70);
            table.Rows.Add(681, 70);
            table.Rows.Add(682, 70);
            table.Rows.Add(683, 70);
            table.Rows.Add(684, 70);
            table.Rows.Add(685, 70);
            table.Rows.Add(686, 70);
            table.Rows.Add(687, 70);
            table.Rows.Add(688, 70);
            table.Rows.Add(689, 70);
            table.Rows.Add(690, 70);
            table.Rows.Add(691, 70);
            table.Rows.Add(692, 70);
            table.Rows.Add(693, 70);
            table.Rows.Add(694, 70);
            table.Rows.Add(695, 70);
            table.Rows.Add(696, 70);
            table.Rows.Add(697, 70);
            table.Rows.Add(698, 70);
            table.Rows.Add(699, 70);
            table.Rows.Add(700, 70);
            table.Rows.Add(701, 70);
            table.Rows.Add(702, 70);
            table.Rows.Add(703, 70);
            table.Rows.Add(704, 35);
            table.Rows.Add(705, 35);
            table.Rows.Add(706, 35);
            table.Rows.Add(707, 70);
            table.Rows.Add(708, 70);
            table.Rows.Add(709, 70);
            table.Rows.Add(710, 70);
            table.Rows.Add(711, 70);
            table.Rows.Add(712, 70);
            table.Rows.Add(713, 70);
            table.Rows.Add(714, 70);
            table.Rows.Add(715, 70);
            table.Rows.Add(716, 0);
            table.Rows.Add(717, 0);
            table.Rows.Add(718, 0);
            table.Rows.Add(719, 70);
            table.Rows.Add(720, 100);
            table.Rows.Add(721, 100);
            table.Rows.Add(722, 0);

            return table;
        }
        static DataTable SpeciesTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Species", typeof(int));
            table.Columns.Add("EXP Growth", typeof(int));
            table.Columns.Add("BST HP", typeof(int));
            table.Columns.Add("BST ATK", typeof(int));
            table.Columns.Add("BST DEF", typeof(int));
            table.Columns.Add("BST SpA", typeof(int));
            table.Columns.Add("BST SpD", typeof(int));
            table.Columns.Add("BST Spe", typeof(int));
            table.Columns.Add("GT ID", typeof(int));

            table.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 256);
            table.Rows.Add(1, 3, 45, 49, 49, 65, 65, 45, 32);
            table.Rows.Add(2, 3, 60, 62, 63, 80, 80, 60, 32);
            table.Rows.Add(3, 3, 80, 82, 83, 100, 100, 80, 32);
            table.Rows.Add(4, 3, 39, 52, 43, 60, 50, 65, 32);
            table.Rows.Add(5, 3, 58, 64, 58, 80, 65, 80, 32);
            table.Rows.Add(6, 3, 78, 84, 78, 109, 85, 100, 32);
            table.Rows.Add(7, 3, 44, 48, 65, 50, 64, 43, 32);
            table.Rows.Add(8, 3, 59, 63, 80, 65, 80, 58, 32);
            table.Rows.Add(9, 3, 79, 83, 100, 85, 105, 78, 32);
            table.Rows.Add(10, 2, 45, 30, 35, 20, 20, 45, 128);
            table.Rows.Add(11, 2, 50, 20, 55, 25, 25, 30, 128);
            table.Rows.Add(12, 2, 60, 45, 50, 90, 80, 70, 128);
            table.Rows.Add(13, 2, 40, 35, 30, 20, 20, 50, 128);
            table.Rows.Add(14, 2, 45, 25, 50, 25, 25, 35, 128);
            table.Rows.Add(15, 2, 65, 90, 40, 45, 80, 75, 128);
            table.Rows.Add(16, 3, 40, 45, 40, 35, 35, 56, 128);
            table.Rows.Add(17, 3, 63, 60, 55, 50, 50, 71, 128);
            table.Rows.Add(18, 3, 83, 80, 75, 70, 70, 101, 128);
            table.Rows.Add(19, 2, 30, 56, 35, 25, 35, 72, 128);
            table.Rows.Add(20, 2, 55, 81, 60, 50, 70, 97, 128);
            table.Rows.Add(21, 2, 40, 60, 30, 31, 31, 70, 128);
            table.Rows.Add(22, 2, 65, 90, 65, 61, 61, 100, 128);
            table.Rows.Add(23, 2, 35, 60, 44, 40, 54, 55, 128);
            table.Rows.Add(24, 2, 60, 85, 69, 65, 79, 80, 128);
            table.Rows.Add(25, 2, 35, 55, 40, 50, 50, 90, 128);
            table.Rows.Add(26, 2, 60, 90, 55, 90, 80, 110, 128);
            table.Rows.Add(27, 2, 50, 75, 85, 20, 30, 40, 128);
            table.Rows.Add(28, 2, 75, 100, 110, 45, 55, 65, 128);
            table.Rows.Add(29, 3, 55, 47, 52, 40, 40, 41, 257);
            table.Rows.Add(30, 3, 70, 62, 67, 55, 55, 56, 257);
            table.Rows.Add(31, 3, 90, 92, 87, 75, 85, 76, 257);
            table.Rows.Add(32, 3, 46, 57, 40, 40, 40, 50, 256);
            table.Rows.Add(33, 3, 61, 72, 57, 55, 55, 65, 256);
            table.Rows.Add(34, 3, 81, 102, 77, 85, 75, 85, 256);
            table.Rows.Add(35, 1, 70, 45, 48, 60, 65, 35, 192);
            table.Rows.Add(36, 1, 95, 70, 73, 95, 90, 60, 192);
            table.Rows.Add(37, 2, 38, 41, 40, 50, 65, 65, 192);
            table.Rows.Add(38, 2, 73, 76, 75, 81, 100, 100, 192);
            table.Rows.Add(39, 1, 115, 45, 20, 45, 25, 20, 192);
            table.Rows.Add(40, 1, 140, 70, 45, 85, 50, 45, 192);
            table.Rows.Add(41, 2, 40, 45, 35, 30, 40, 55, 128);
            table.Rows.Add(42, 2, 75, 80, 70, 65, 75, 90, 128);
            table.Rows.Add(43, 3, 45, 50, 55, 75, 65, 30, 128);
            table.Rows.Add(44, 3, 60, 65, 70, 85, 75, 40, 128);
            table.Rows.Add(45, 3, 75, 80, 85, 110, 90, 50, 128);
            table.Rows.Add(46, 2, 35, 70, 55, 45, 55, 25, 128);
            table.Rows.Add(47, 2, 60, 95, 80, 60, 80, 30, 128);
            table.Rows.Add(48, 2, 60, 55, 50, 40, 55, 45, 128);
            table.Rows.Add(49, 2, 70, 65, 60, 90, 75, 90, 128);
            table.Rows.Add(50, 2, 10, 55, 25, 35, 45, 95, 128);
            table.Rows.Add(51, 2, 35, 80, 50, 50, 70, 120, 128);
            table.Rows.Add(52, 2, 40, 45, 35, 40, 40, 90, 128);
            table.Rows.Add(53, 2, 65, 70, 60, 65, 65, 115, 128);
            table.Rows.Add(54, 2, 50, 52, 48, 65, 50, 55, 128);
            table.Rows.Add(55, 2, 80, 82, 78, 95, 80, 85, 128);
            table.Rows.Add(56, 2, 40, 80, 35, 35, 45, 70, 128);
            table.Rows.Add(57, 2, 65, 105, 60, 60, 70, 95, 128);
            table.Rows.Add(58, 4, 55, 70, 45, 70, 50, 60, 64);
            table.Rows.Add(59, 4, 90, 110, 80, 100, 80, 95, 64);
            table.Rows.Add(60, 3, 40, 50, 40, 40, 40, 90, 128);
            table.Rows.Add(61, 3, 65, 65, 65, 50, 50, 90, 128);
            table.Rows.Add(62, 3, 90, 95, 95, 70, 90, 70, 128);
            table.Rows.Add(63, 3, 25, 20, 15, 105, 55, 90, 64);
            table.Rows.Add(64, 3, 40, 35, 30, 120, 70, 105, 64);
            table.Rows.Add(65, 3, 55, 50, 45, 135, 95, 120, 64);
            table.Rows.Add(66, 3, 70, 80, 50, 35, 35, 35, 64);
            table.Rows.Add(67, 3, 80, 100, 70, 50, 60, 45, 64);
            table.Rows.Add(68, 3, 90, 130, 80, 65, 85, 55, 64);
            table.Rows.Add(69, 3, 50, 75, 35, 70, 30, 40, 128);
            table.Rows.Add(70, 3, 65, 90, 50, 85, 45, 55, 128);
            table.Rows.Add(71, 3, 80, 105, 65, 100, 70, 70, 128);
            table.Rows.Add(72, 4, 40, 40, 35, 50, 100, 70, 128);
            table.Rows.Add(73, 4, 80, 70, 65, 80, 120, 100, 128);
            table.Rows.Add(74, 3, 40, 80, 100, 30, 30, 20, 128);
            table.Rows.Add(75, 3, 55, 95, 115, 45, 45, 35, 128);
            table.Rows.Add(76, 3, 80, 120, 130, 55, 65, 45, 128);
            table.Rows.Add(77, 2, 50, 85, 55, 65, 65, 90, 128);
            table.Rows.Add(78, 2, 65, 100, 70, 80, 80, 105, 128);
            table.Rows.Add(79, 2, 90, 65, 65, 40, 40, 15, 128);
            table.Rows.Add(80, 2, 95, 75, 110, 100, 80, 30, 128);
            table.Rows.Add(81, 2, 25, 35, 70, 95, 55, 45, 258);
            table.Rows.Add(82, 2, 50, 60, 95, 120, 70, 70, 258);
            table.Rows.Add(83, 2, 52, 65, 55, 58, 62, 60, 128);
            table.Rows.Add(84, 2, 35, 85, 45, 35, 35, 75, 128);
            table.Rows.Add(85, 2, 60, 110, 70, 60, 60, 100, 128);
            table.Rows.Add(86, 2, 65, 45, 55, 45, 70, 45, 128);
            table.Rows.Add(87, 2, 90, 70, 80, 70, 95, 70, 128);
            table.Rows.Add(88, 2, 80, 80, 50, 40, 50, 25, 128);
            table.Rows.Add(89, 2, 105, 105, 75, 65, 100, 50, 128);
            table.Rows.Add(90, 4, 30, 65, 100, 45, 25, 40, 128);
            table.Rows.Add(91, 4, 50, 95, 180, 85, 45, 70, 128);
            table.Rows.Add(92, 3, 30, 35, 30, 100, 35, 80, 128);
            table.Rows.Add(93, 3, 45, 50, 45, 115, 55, 95, 128);
            table.Rows.Add(94, 3, 60, 65, 60, 130, 75, 110, 128);
            table.Rows.Add(95, 2, 35, 45, 160, 30, 45, 70, 128);
            table.Rows.Add(96, 2, 60, 48, 45, 43, 90, 42, 128);
            table.Rows.Add(97, 2, 85, 73, 70, 73, 115, 67, 128);
            table.Rows.Add(98, 2, 30, 105, 90, 25, 25, 50, 128);
            table.Rows.Add(99, 2, 55, 130, 115, 50, 50, 75, 128);
            table.Rows.Add(100, 2, 40, 30, 50, 55, 55, 100, 258);
            table.Rows.Add(101, 2, 60, 50, 70, 80, 80, 140, 258);
            table.Rows.Add(102, 4, 60, 40, 80, 60, 45, 40, 128);
            table.Rows.Add(103, 4, 95, 95, 85, 125, 65, 55, 128);
            table.Rows.Add(104, 2, 50, 50, 95, 40, 50, 35, 128);
            table.Rows.Add(105, 2, 60, 80, 110, 50, 80, 45, 128);
            table.Rows.Add(106, 2, 50, 120, 53, 35, 110, 87, 256);
            table.Rows.Add(107, 2, 50, 105, 79, 35, 110, 76, 256);
            table.Rows.Add(108, 2, 90, 55, 75, 60, 75, 30, 128);
            table.Rows.Add(109, 2, 40, 65, 95, 60, 45, 35, 128);
            table.Rows.Add(110, 2, 65, 90, 120, 85, 70, 60, 128);
            table.Rows.Add(111, 4, 80, 85, 95, 30, 30, 25, 128);
            table.Rows.Add(112, 4, 105, 130, 120, 45, 45, 40, 128);
            table.Rows.Add(113, 1, 250, 5, 5, 35, 105, 50, 257);
            table.Rows.Add(114, 2, 65, 55, 115, 100, 40, 60, 128);
            table.Rows.Add(115, 2, 105, 95, 80, 40, 80, 90, 257);
            table.Rows.Add(116, 2, 30, 40, 70, 70, 25, 60, 128);
            table.Rows.Add(117, 2, 55, 65, 95, 95, 45, 85, 128);
            table.Rows.Add(118, 2, 45, 67, 60, 35, 50, 63, 128);
            table.Rows.Add(119, 2, 80, 92, 65, 65, 80, 68, 128);
            table.Rows.Add(120, 4, 30, 45, 55, 70, 55, 85, 258);
            table.Rows.Add(121, 4, 60, 75, 85, 100, 85, 115, 258);
            table.Rows.Add(122, 2, 40, 45, 65, 100, 120, 90, 128);
            table.Rows.Add(123, 2, 70, 110, 80, 55, 80, 105, 128);
            table.Rows.Add(124, 2, 65, 50, 35, 115, 95, 95, 257);
            table.Rows.Add(125, 2, 65, 83, 57, 95, 85, 105, 64);
            table.Rows.Add(126, 2, 65, 95, 57, 100, 85, 93, 64);
            table.Rows.Add(127, 4, 65, 125, 100, 55, 70, 85, 128);
            table.Rows.Add(128, 4, 75, 100, 95, 40, 70, 110, 256);
            table.Rows.Add(129, 4, 20, 10, 55, 15, 20, 80, 128);
            table.Rows.Add(130, 4, 95, 125, 79, 60, 100, 81, 128);
            table.Rows.Add(131, 4, 130, 85, 80, 85, 95, 60, 128);
            table.Rows.Add(132, 2, 48, 48, 48, 48, 48, 48, 258);
            table.Rows.Add(133, 2, 55, 55, 50, 45, 65, 55, 32);
            table.Rows.Add(134, 2, 130, 65, 60, 110, 95, 65, 32);
            table.Rows.Add(135, 2, 65, 65, 60, 110, 95, 130, 32);
            table.Rows.Add(136, 2, 65, 130, 60, 95, 110, 65, 32);
            table.Rows.Add(137, 2, 65, 60, 70, 85, 75, 40, 258);
            table.Rows.Add(138, 2, 35, 40, 100, 90, 55, 35, 32);
            table.Rows.Add(139, 2, 70, 60, 125, 115, 70, 55, 32);
            table.Rows.Add(140, 2, 30, 80, 90, 55, 45, 55, 32);
            table.Rows.Add(141, 2, 60, 115, 105, 65, 70, 80, 32);
            table.Rows.Add(142, 4, 80, 105, 65, 60, 75, 130, 32);
            table.Rows.Add(143, 4, 160, 110, 65, 65, 110, 30, 32);
            table.Rows.Add(144, 4, 90, 85, 100, 95, 125, 85, 258);
            table.Rows.Add(145, 4, 90, 90, 85, 125, 90, 100, 258);
            table.Rows.Add(146, 4, 90, 100, 90, 125, 85, 90, 258);
            table.Rows.Add(147, 4, 41, 64, 45, 50, 50, 50, 128);
            table.Rows.Add(148, 4, 61, 84, 65, 70, 70, 70, 128);
            table.Rows.Add(149, 4, 91, 134, 95, 100, 100, 80, 128);
            table.Rows.Add(150, 4, 106, 110, 90, 154, 90, 130, 258);
            table.Rows.Add(151, 3, 100, 100, 100, 100, 100, 100, 258);
            table.Rows.Add(152, 3, 45, 49, 65, 49, 65, 45, 32);
            table.Rows.Add(153, 3, 60, 62, 80, 63, 80, 60, 32);
            table.Rows.Add(154, 3, 80, 82, 100, 83, 100, 80, 32);
            table.Rows.Add(155, 3, 39, 52, 43, 60, 50, 65, 32);
            table.Rows.Add(156, 3, 58, 64, 58, 80, 65, 80, 32);
            table.Rows.Add(157, 3, 78, 84, 78, 109, 85, 100, 32);
            table.Rows.Add(158, 3, 50, 65, 64, 44, 48, 43, 32);
            table.Rows.Add(159, 3, 65, 80, 80, 59, 63, 58, 32);
            table.Rows.Add(160, 3, 85, 105, 100, 79, 83, 78, 32);
            table.Rows.Add(161, 2, 35, 46, 34, 35, 45, 20, 128);
            table.Rows.Add(162, 2, 85, 76, 64, 45, 55, 90, 128);
            table.Rows.Add(163, 2, 60, 30, 30, 36, 56, 50, 128);
            table.Rows.Add(164, 2, 100, 50, 50, 76, 96, 70, 128);
            table.Rows.Add(165, 1, 40, 20, 30, 40, 80, 55, 128);
            table.Rows.Add(166, 1, 55, 35, 50, 55, 110, 85, 128);
            table.Rows.Add(167, 1, 40, 60, 40, 40, 40, 30, 128);
            table.Rows.Add(168, 1, 70, 90, 70, 60, 60, 40, 128);
            table.Rows.Add(169, 2, 85, 90, 80, 70, 80, 130, 128);
            table.Rows.Add(170, 4, 75, 38, 38, 56, 56, 67, 128);
            table.Rows.Add(171, 4, 125, 58, 58, 76, 76, 67, 128);
            table.Rows.Add(172, 2, 20, 40, 15, 35, 35, 60, 128);
            table.Rows.Add(173, 1, 50, 25, 28, 45, 55, 15, 192);
            table.Rows.Add(174, 1, 90, 30, 15, 40, 20, 15, 192);
            table.Rows.Add(175, 1, 35, 20, 65, 40, 65, 20, 32);
            table.Rows.Add(176, 1, 55, 40, 85, 80, 105, 40, 32);
            table.Rows.Add(177, 2, 40, 50, 45, 70, 45, 70, 128);
            table.Rows.Add(178, 2, 65, 75, 70, 95, 70, 95, 128);
            table.Rows.Add(179, 3, 55, 40, 40, 65, 45, 35, 128);
            table.Rows.Add(180, 3, 70, 55, 55, 80, 60, 45, 128);
            table.Rows.Add(181, 3, 90, 75, 85, 115, 90, 55, 128);
            table.Rows.Add(182, 3, 75, 80, 95, 90, 100, 50, 128);
            table.Rows.Add(183, 1, 70, 20, 50, 20, 50, 40, 128);
            table.Rows.Add(184, 1, 100, 50, 80, 60, 80, 50, 128);
            table.Rows.Add(185, 2, 70, 100, 115, 30, 65, 30, 128);
            table.Rows.Add(186, 3, 90, 75, 75, 90, 100, 70, 128);
            table.Rows.Add(187, 3, 35, 35, 40, 35, 55, 50, 128);
            table.Rows.Add(188, 3, 55, 45, 50, 45, 65, 80, 128);
            table.Rows.Add(189, 3, 75, 55, 70, 55, 95, 110, 128);
            table.Rows.Add(190, 1, 55, 70, 55, 40, 55, 85, 128);
            table.Rows.Add(191, 3, 30, 30, 30, 30, 30, 30, 128);
            table.Rows.Add(192, 3, 75, 75, 55, 105, 85, 30, 128);
            table.Rows.Add(193, 2, 65, 65, 45, 75, 45, 95, 128);
            table.Rows.Add(194, 2, 55, 45, 45, 25, 25, 15, 128);
            table.Rows.Add(195, 2, 95, 85, 85, 65, 65, 35, 128);
            table.Rows.Add(196, 2, 65, 65, 60, 130, 95, 110, 32);
            table.Rows.Add(197, 2, 95, 65, 110, 60, 130, 65, 32);
            table.Rows.Add(198, 3, 60, 85, 42, 85, 42, 91, 128);
            table.Rows.Add(199, 2, 95, 75, 80, 100, 110, 30, 128);
            table.Rows.Add(200, 1, 60, 60, 60, 85, 85, 85, 128);
            table.Rows.Add(201, 2, 48, 72, 48, 72, 48, 48, 258);
            table.Rows.Add(202, 2, 190, 33, 58, 33, 58, 33, 128);
            table.Rows.Add(203, 2, 70, 80, 65, 90, 65, 85, 128);
            table.Rows.Add(204, 2, 50, 65, 90, 35, 35, 15, 128);
            table.Rows.Add(205, 2, 75, 90, 140, 60, 60, 40, 128);
            table.Rows.Add(206, 2, 100, 70, 70, 65, 65, 45, 128);
            table.Rows.Add(207, 3, 65, 75, 105, 35, 65, 85, 128);
            table.Rows.Add(208, 2, 75, 85, 200, 55, 65, 30, 128);
            table.Rows.Add(209, 1, 60, 80, 50, 40, 40, 30, 192);
            table.Rows.Add(210, 1, 90, 120, 75, 60, 60, 45, 192);
            table.Rows.Add(211, 2, 65, 95, 75, 55, 55, 85, 128);
            table.Rows.Add(212, 2, 70, 130, 100, 55, 80, 65, 128);
            table.Rows.Add(213, 3, 20, 10, 230, 10, 230, 5, 128);
            table.Rows.Add(214, 4, 80, 125, 75, 40, 95, 85, 128);
            table.Rows.Add(215, 3, 55, 95, 55, 35, 75, 115, 128);
            table.Rows.Add(216, 2, 60, 80, 50, 50, 50, 40, 128);
            table.Rows.Add(217, 2, 90, 130, 75, 75, 75, 55, 128);
            table.Rows.Add(218, 2, 40, 40, 40, 70, 40, 20, 128);
            table.Rows.Add(219, 2, 50, 50, 120, 80, 80, 30, 128);
            table.Rows.Add(220, 4, 50, 50, 40, 30, 30, 50, 128);
            table.Rows.Add(221, 4, 100, 100, 80, 60, 60, 50, 128);
            table.Rows.Add(222, 1, 55, 55, 85, 65, 85, 35, 192);
            table.Rows.Add(223, 2, 35, 65, 35, 65, 35, 65, 128);
            table.Rows.Add(224, 2, 75, 105, 75, 105, 75, 45, 128);
            table.Rows.Add(225, 1, 45, 55, 45, 65, 45, 75, 128);
            table.Rows.Add(226, 4, 65, 40, 70, 80, 140, 70, 128);
            table.Rows.Add(227, 4, 65, 80, 140, 40, 70, 70, 128);
            table.Rows.Add(228, 4, 45, 60, 30, 80, 50, 65, 128);
            table.Rows.Add(229, 4, 75, 90, 50, 110, 80, 95, 128);
            table.Rows.Add(230, 2, 75, 95, 95, 95, 95, 85, 128);
            table.Rows.Add(231, 2, 90, 60, 60, 40, 40, 40, 128);
            table.Rows.Add(232, 2, 90, 120, 120, 60, 60, 50, 128);
            table.Rows.Add(233, 2, 85, 80, 90, 105, 95, 60, 258);
            table.Rows.Add(234, 4, 73, 95, 62, 85, 65, 85, 128);
            table.Rows.Add(235, 1, 55, 20, 35, 20, 45, 75, 128);
            table.Rows.Add(236, 2, 35, 35, 35, 35, 35, 35, 256);
            table.Rows.Add(237, 2, 50, 95, 95, 35, 110, 70, 256);
            table.Rows.Add(238, 2, 45, 30, 15, 85, 65, 65, 257);
            table.Rows.Add(239, 2, 45, 63, 37, 65, 55, 95, 64);
            table.Rows.Add(240, 2, 45, 75, 37, 70, 55, 83, 64);
            table.Rows.Add(241, 4, 95, 80, 105, 40, 70, 100, 257);
            table.Rows.Add(242, 1, 255, 10, 10, 75, 135, 55, 257);
            table.Rows.Add(243, 4, 90, 85, 75, 115, 100, 115, 258);
            table.Rows.Add(244, 4, 115, 115, 85, 90, 75, 100, 258);
            table.Rows.Add(245, 4, 100, 75, 115, 90, 115, 85, 258);
            table.Rows.Add(246, 4, 50, 64, 50, 45, 50, 41, 128);
            table.Rows.Add(247, 4, 70, 84, 70, 65, 70, 51, 128);
            table.Rows.Add(248, 4, 100, 134, 110, 95, 100, 61, 128);
            table.Rows.Add(249, 4, 106, 90, 130, 90, 154, 110, 258);
            table.Rows.Add(250, 4, 106, 130, 90, 110, 154, 90, 258);
            table.Rows.Add(251, 3, 100, 100, 100, 100, 100, 100, 258);
            table.Rows.Add(252, 3, 40, 45, 35, 65, 55, 70, 32);
            table.Rows.Add(253, 3, 50, 65, 45, 85, 65, 95, 32);
            table.Rows.Add(254, 3, 70, 85, 65, 105, 85, 120, 32);
            table.Rows.Add(255, 3, 45, 60, 40, 70, 50, 45, 32);
            table.Rows.Add(256, 3, 60, 85, 60, 85, 60, 55, 32);
            table.Rows.Add(257, 3, 80, 120, 70, 110, 70, 80, 32);
            table.Rows.Add(258, 3, 50, 70, 50, 50, 50, 40, 32);
            table.Rows.Add(259, 3, 70, 85, 70, 60, 70, 50, 32);
            table.Rows.Add(260, 3, 100, 110, 90, 85, 90, 60, 32);
            table.Rows.Add(261, 2, 35, 55, 35, 30, 30, 35, 128);
            table.Rows.Add(262, 2, 70, 90, 70, 60, 60, 70, 128);
            table.Rows.Add(263, 2, 38, 30, 41, 30, 41, 60, 128);
            table.Rows.Add(264, 2, 78, 70, 61, 50, 61, 100, 128);
            table.Rows.Add(265, 2, 45, 45, 35, 20, 30, 20, 128);
            table.Rows.Add(266, 2, 50, 35, 55, 25, 25, 15, 128);
            table.Rows.Add(267, 2, 60, 70, 50, 100, 50, 65, 128);
            table.Rows.Add(268, 2, 50, 35, 55, 25, 25, 15, 128);
            table.Rows.Add(269, 2, 60, 50, 70, 50, 90, 65, 128);
            table.Rows.Add(270, 3, 40, 30, 30, 40, 50, 30, 128);
            table.Rows.Add(271, 3, 60, 50, 50, 60, 70, 50, 128);
            table.Rows.Add(272, 3, 80, 70, 70, 90, 100, 70, 128);
            table.Rows.Add(273, 3, 40, 40, 50, 30, 30, 30, 128);
            table.Rows.Add(274, 3, 70, 70, 40, 60, 40, 60, 128);
            table.Rows.Add(275, 3, 90, 100, 60, 90, 60, 80, 128);
            table.Rows.Add(276, 3, 40, 55, 30, 30, 30, 85, 128);
            table.Rows.Add(277, 3, 60, 85, 60, 50, 50, 125, 128);
            table.Rows.Add(278, 2, 40, 30, 30, 55, 30, 85, 128);
            table.Rows.Add(279, 2, 60, 50, 100, 85, 70, 65, 128);
            table.Rows.Add(280, 4, 28, 25, 25, 45, 35, 40, 128);
            table.Rows.Add(281, 4, 38, 35, 35, 65, 55, 50, 128);
            table.Rows.Add(282, 4, 68, 65, 65, 125, 115, 80, 128);
            table.Rows.Add(283, 2, 40, 30, 32, 50, 52, 65, 128);
            table.Rows.Add(284, 2, 70, 60, 62, 80, 82, 60, 128);
            table.Rows.Add(285, 5, 60, 40, 60, 40, 60, 35, 128);
            table.Rows.Add(286, 5, 60, 130, 80, 60, 60, 70, 128);
            table.Rows.Add(287, 4, 60, 60, 60, 35, 35, 30, 128);
            table.Rows.Add(288, 4, 80, 80, 80, 55, 55, 90, 128);
            table.Rows.Add(289, 4, 150, 160, 100, 95, 65, 100, 128);
            table.Rows.Add(290, 0, 31, 45, 90, 30, 30, 40, 128);
            table.Rows.Add(291, 0, 61, 90, 45, 50, 50, 160, 128);
            table.Rows.Add(292, 0, 1, 90, 45, 30, 30, 40, 258);
            table.Rows.Add(293, 3, 64, 51, 23, 51, 23, 28, 128);
            table.Rows.Add(294, 3, 84, 71, 43, 71, 43, 48, 128);
            table.Rows.Add(295, 3, 104, 91, 63, 91, 73, 68, 128);
            table.Rows.Add(296, 5, 72, 60, 30, 20, 30, 25, 64);
            table.Rows.Add(297, 5, 144, 120, 60, 40, 60, 50, 64);
            table.Rows.Add(298, 1, 50, 20, 40, 20, 40, 20, 192);
            table.Rows.Add(299, 2, 30, 45, 135, 45, 90, 30, 128);
            table.Rows.Add(300, 1, 50, 45, 45, 35, 35, 50, 192);
            table.Rows.Add(301, 1, 70, 65, 65, 55, 55, 70, 192);
            table.Rows.Add(302, 3, 50, 75, 75, 65, 65, 50, 128);
            table.Rows.Add(303, 1, 50, 85, 85, 55, 55, 50, 128);
            table.Rows.Add(304, 4, 50, 70, 100, 40, 40, 30, 128);
            table.Rows.Add(305, 4, 60, 90, 140, 50, 50, 40, 128);
            table.Rows.Add(306, 4, 70, 110, 180, 60, 60, 50, 128);
            table.Rows.Add(307, 2, 30, 40, 55, 40, 55, 60, 128);
            table.Rows.Add(308, 2, 60, 60, 75, 60, 75, 80, 128);
            table.Rows.Add(309, 4, 40, 45, 40, 65, 40, 65, 128);
            table.Rows.Add(310, 4, 70, 75, 60, 105, 60, 105, 128);
            table.Rows.Add(311, 2, 60, 50, 40, 85, 75, 95, 128);
            table.Rows.Add(312, 2, 60, 40, 50, 75, 85, 95, 128);
            table.Rows.Add(313, 0, 65, 73, 55, 47, 75, 85, 256);
            table.Rows.Add(314, 5, 65, 47, 55, 73, 75, 85, 257);
            table.Rows.Add(315, 3, 50, 60, 45, 100, 80, 65, 128);
            table.Rows.Add(316, 5, 70, 43, 53, 43, 53, 40, 128);
            table.Rows.Add(317, 5, 100, 73, 83, 73, 83, 55, 128);
            table.Rows.Add(318, 4, 45, 90, 20, 65, 20, 65, 128);
            table.Rows.Add(319, 4, 70, 120, 40, 95, 40, 95, 128);
            table.Rows.Add(320, 5, 130, 70, 35, 70, 35, 60, 128);
            table.Rows.Add(321, 5, 170, 90, 45, 90, 45, 60, 128);
            table.Rows.Add(322, 2, 60, 60, 40, 65, 45, 35, 128);
            table.Rows.Add(323, 2, 70, 100, 70, 105, 75, 40, 128);
            table.Rows.Add(324, 2, 70, 85, 140, 85, 70, 20, 128);
            table.Rows.Add(325, 1, 60, 25, 35, 70, 80, 60, 128);
            table.Rows.Add(326, 1, 80, 45, 65, 90, 110, 80, 128);
            table.Rows.Add(327, 1, 60, 60, 60, 60, 60, 60, 128);
            table.Rows.Add(328, 3, 45, 100, 45, 45, 45, 10, 128);
            table.Rows.Add(329, 3, 50, 70, 50, 50, 50, 70, 128);
            table.Rows.Add(330, 3, 80, 100, 80, 80, 80, 100, 128);
            table.Rows.Add(331, 3, 50, 85, 40, 85, 40, 35, 128);
            table.Rows.Add(332, 3, 70, 115, 60, 115, 60, 55, 128);
            table.Rows.Add(333, 0, 45, 40, 60, 40, 75, 50, 128);
            table.Rows.Add(334, 0, 75, 70, 90, 70, 105, 80, 128);
            table.Rows.Add(335, 0, 73, 115, 60, 60, 60, 90, 128);
            table.Rows.Add(336, 5, 73, 100, 60, 100, 60, 65, 128);
            table.Rows.Add(337, 1, 70, 55, 65, 95, 85, 70, 258);
            table.Rows.Add(338, 1, 70, 95, 85, 55, 65, 70, 258);
            table.Rows.Add(339, 2, 50, 48, 43, 46, 41, 60, 128);
            table.Rows.Add(340, 2, 110, 78, 73, 76, 71, 60, 128);
            table.Rows.Add(341, 5, 43, 80, 65, 50, 35, 35, 128);
            table.Rows.Add(342, 5, 63, 120, 85, 90, 55, 55, 128);
            table.Rows.Add(343, 2, 40, 40, 55, 40, 70, 55, 258);
            table.Rows.Add(344, 2, 60, 70, 105, 70, 120, 75, 258);
            table.Rows.Add(345, 0, 66, 41, 77, 61, 87, 23, 32);
            table.Rows.Add(346, 0, 86, 81, 97, 81, 107, 43, 32);
            table.Rows.Add(347, 0, 45, 95, 50, 40, 50, 75, 32);
            table.Rows.Add(348, 0, 75, 125, 100, 70, 80, 45, 32);
            table.Rows.Add(349, 0, 20, 15, 20, 10, 55, 80, 128);
            table.Rows.Add(350, 0, 95, 60, 79, 100, 125, 81, 128);
            table.Rows.Add(351, 2, 70, 70, 70, 70, 70, 70, 128);
            table.Rows.Add(352, 3, 60, 90, 70, 60, 120, 40, 128);
            table.Rows.Add(353, 1, 44, 75, 35, 63, 33, 45, 128);
            table.Rows.Add(354, 1, 64, 115, 65, 83, 63, 65, 128);
            table.Rows.Add(355, 1, 20, 40, 90, 30, 90, 25, 128);
            table.Rows.Add(356, 1, 40, 70, 130, 60, 130, 25, 128);
            table.Rows.Add(357, 4, 99, 68, 83, 72, 87, 51, 128);
            table.Rows.Add(358, 1, 65, 50, 70, 95, 80, 65, 128);
            table.Rows.Add(359, 3, 65, 130, 60, 75, 60, 75, 128);
            table.Rows.Add(360, 2, 95, 23, 48, 23, 48, 23, 128);
            table.Rows.Add(361, 2, 50, 50, 50, 50, 50, 50, 128);
            table.Rows.Add(362, 2, 80, 80, 80, 80, 80, 80, 128);
            table.Rows.Add(363, 3, 70, 40, 50, 55, 50, 25, 128);
            table.Rows.Add(364, 3, 90, 60, 70, 75, 70, 45, 128);
            table.Rows.Add(365, 3, 110, 80, 90, 95, 90, 65, 128);
            table.Rows.Add(366, 0, 35, 64, 85, 74, 55, 32, 128);
            table.Rows.Add(367, 0, 55, 104, 105, 94, 75, 52, 128);
            table.Rows.Add(368, 0, 55, 84, 105, 114, 75, 52, 128);
            table.Rows.Add(369, 4, 100, 90, 130, 45, 65, 55, 32);
            table.Rows.Add(370, 1, 43, 30, 55, 40, 65, 97, 192);
            table.Rows.Add(371, 4, 45, 75, 60, 40, 30, 50, 128);
            table.Rows.Add(372, 4, 65, 95, 100, 60, 50, 50, 128);
            table.Rows.Add(373, 4, 95, 135, 80, 110, 80, 100, 128);
            table.Rows.Add(374, 4, 40, 55, 80, 35, 60, 30, 258);
            table.Rows.Add(375, 4, 60, 75, 100, 55, 80, 50, 258);
            table.Rows.Add(376, 4, 80, 135, 130, 95, 90, 70, 258);
            table.Rows.Add(377, 4, 80, 100, 200, 50, 100, 50, 258);
            table.Rows.Add(378, 4, 80, 50, 100, 100, 200, 50, 258);
            table.Rows.Add(379, 4, 80, 75, 150, 75, 150, 50, 258);
            table.Rows.Add(380, 4, 80, 80, 90, 110, 130, 110, 257);
            table.Rows.Add(381, 4, 80, 90, 80, 130, 110, 110, 256);
            table.Rows.Add(382, 4, 100, 100, 90, 150, 140, 90, 258);
            table.Rows.Add(383, 4, 100, 150, 140, 100, 90, 90, 258);
            table.Rows.Add(384, 4, 105, 150, 90, 150, 90, 95, 258);
            table.Rows.Add(385, 4, 100, 100, 100, 100, 100, 100, 258);
            table.Rows.Add(386, 4, 50, 180, 20, 180, 20, 150, 258);
            table.Rows.Add(387, 3, 55, 68, 64, 45, 55, 31, 32);
            table.Rows.Add(388, 3, 75, 89, 85, 55, 65, 36, 32);
            table.Rows.Add(389, 3, 95, 109, 105, 75, 85, 56, 32);
            table.Rows.Add(390, 3, 44, 58, 44, 58, 44, 61, 32);
            table.Rows.Add(391, 3, 64, 78, 52, 78, 52, 81, 32);
            table.Rows.Add(392, 3, 76, 104, 71, 104, 71, 108, 32);
            table.Rows.Add(393, 3, 53, 51, 53, 61, 56, 40, 32);
            table.Rows.Add(394, 3, 64, 66, 68, 81, 76, 50, 32);
            table.Rows.Add(395, 3, 84, 86, 88, 111, 101, 60, 32);
            table.Rows.Add(396, 3, 40, 55, 30, 30, 30, 60, 128);
            table.Rows.Add(397, 3, 55, 75, 50, 40, 40, 80, 128);
            table.Rows.Add(398, 3, 85, 120, 70, 50, 60, 100, 128);
            table.Rows.Add(399, 2, 59, 45, 40, 35, 40, 31, 128);
            table.Rows.Add(400, 2, 79, 85, 60, 55, 60, 71, 128);
            table.Rows.Add(401, 3, 37, 25, 41, 25, 41, 25, 128);
            table.Rows.Add(402, 3, 77, 85, 51, 55, 51, 65, 128);
            table.Rows.Add(403, 3, 45, 65, 34, 40, 34, 45, 128);
            table.Rows.Add(404, 3, 60, 85, 49, 60, 49, 60, 128);
            table.Rows.Add(405, 3, 80, 120, 79, 95, 79, 70, 128);
            table.Rows.Add(406, 3, 40, 30, 35, 50, 70, 55, 128);
            table.Rows.Add(407, 3, 60, 70, 65, 125, 105, 90, 128);
            table.Rows.Add(408, 0, 67, 125, 40, 30, 30, 58, 32);
            table.Rows.Add(409, 0, 97, 165, 60, 65, 50, 58, 32);
            table.Rows.Add(410, 0, 30, 42, 118, 42, 88, 30, 32);
            table.Rows.Add(411, 0, 60, 52, 168, 47, 138, 30, 32);
            table.Rows.Add(412, 2, 40, 29, 45, 29, 45, 36, 128);
            table.Rows.Add(413, 2, 60, 59, 85, 79, 105, 36, 257);
            table.Rows.Add(414, 2, 70, 94, 50, 94, 50, 66, 256);
            table.Rows.Add(415, 3, 30, 30, 42, 30, 42, 70, 32);
            table.Rows.Add(416, 3, 70, 80, 102, 80, 102, 40, 257);
            table.Rows.Add(417, 2, 60, 45, 70, 45, 90, 95, 128);
            table.Rows.Add(418, 2, 55, 65, 35, 60, 30, 85, 128);
            table.Rows.Add(419, 2, 85, 105, 55, 85, 50, 115, 128);
            table.Rows.Add(420, 2, 45, 35, 45, 62, 53, 35, 128);
            table.Rows.Add(421, 2, 70, 60, 70, 87, 78, 85, 128);
            table.Rows.Add(422, 2, 76, 48, 48, 57, 62, 34, 128);
            table.Rows.Add(423, 2, 111, 83, 68, 92, 82, 39, 128);
            table.Rows.Add(424, 1, 75, 100, 66, 60, 66, 115, 128);
            table.Rows.Add(425, 5, 90, 50, 34, 60, 44, 70, 128);
            table.Rows.Add(426, 5, 150, 80, 44, 90, 54, 80, 128);
            table.Rows.Add(427, 2, 55, 66, 44, 44, 56, 85, 128);
            table.Rows.Add(428, 2, 65, 76, 84, 54, 96, 105, 128);
            table.Rows.Add(429, 1, 60, 60, 60, 105, 105, 105, 128);
            table.Rows.Add(430, 3, 100, 125, 52, 105, 52, 71, 128);
            table.Rows.Add(431, 1, 49, 55, 42, 42, 37, 85, 192);
            table.Rows.Add(432, 1, 71, 82, 64, 64, 59, 112, 192);
            table.Rows.Add(433, 1, 45, 30, 50, 65, 50, 45, 128);
            table.Rows.Add(434, 2, 63, 63, 47, 41, 41, 74, 128);
            table.Rows.Add(435, 2, 103, 93, 67, 71, 61, 84, 128);
            table.Rows.Add(436, 2, 57, 24, 86, 24, 86, 23, 258);
            table.Rows.Add(437, 2, 67, 89, 116, 79, 116, 33, 258);
            table.Rows.Add(438, 2, 50, 80, 95, 10, 45, 10, 128);
            table.Rows.Add(439, 2, 20, 25, 45, 70, 90, 60, 128);
            table.Rows.Add(440, 1, 100, 5, 5, 15, 65, 30, 257);
            table.Rows.Add(441, 3, 76, 65, 45, 92, 42, 91, 128);
            table.Rows.Add(442, 2, 50, 92, 108, 92, 108, 35, 128);
            table.Rows.Add(443, 4, 58, 70, 45, 40, 45, 42, 128);
            table.Rows.Add(444, 4, 68, 90, 65, 50, 55, 82, 128);
            table.Rows.Add(445, 4, 108, 130, 95, 80, 85, 102, 128);
            table.Rows.Add(446, 4, 135, 85, 40, 40, 85, 5, 32);
            table.Rows.Add(447, 3, 40, 70, 40, 35, 40, 60, 32);
            table.Rows.Add(448, 3, 70, 110, 70, 115, 70, 90, 32);
            table.Rows.Add(449, 4, 68, 72, 78, 38, 42, 32, 128);
            table.Rows.Add(450, 4, 108, 112, 118, 68, 72, 47, 128);
            table.Rows.Add(451, 4, 40, 50, 90, 30, 55, 65, 128);
            table.Rows.Add(452, 4, 70, 90, 110, 60, 75, 95, 128);
            table.Rows.Add(453, 2, 48, 61, 40, 61, 40, 50, 128);
            table.Rows.Add(454, 2, 83, 106, 65, 86, 65, 85, 128);
            table.Rows.Add(455, 4, 74, 100, 72, 90, 72, 46, 128);
            table.Rows.Add(456, 0, 49, 49, 56, 49, 61, 66, 128);
            table.Rows.Add(457, 0, 69, 69, 76, 69, 86, 91, 128);
            table.Rows.Add(458, 4, 45, 20, 50, 60, 120, 50, 128);
            table.Rows.Add(459, 4, 60, 62, 50, 62, 60, 40, 128);
            table.Rows.Add(460, 4, 90, 92, 75, 92, 85, 60, 128);
            table.Rows.Add(461, 3, 70, 120, 65, 45, 85, 125, 128);
            table.Rows.Add(462, 2, 70, 70, 115, 130, 90, 60, 258);
            table.Rows.Add(463, 2, 110, 85, 95, 80, 95, 50, 128);
            table.Rows.Add(464, 4, 115, 140, 130, 55, 55, 40, 128);
            table.Rows.Add(465, 2, 100, 100, 125, 110, 50, 50, 128);
            table.Rows.Add(466, 2, 75, 123, 67, 95, 85, 95, 64);
            table.Rows.Add(467, 2, 75, 95, 67, 125, 95, 83, 64);
            table.Rows.Add(468, 1, 85, 50, 95, 120, 115, 80, 32);
            table.Rows.Add(469, 2, 86, 76, 86, 116, 56, 95, 128);
            table.Rows.Add(470, 2, 65, 110, 130, 60, 65, 95, 32);
            table.Rows.Add(471, 2, 65, 60, 110, 130, 95, 65, 32);
            table.Rows.Add(472, 3, 75, 95, 125, 45, 75, 95, 128);
            table.Rows.Add(473, 4, 110, 130, 80, 70, 60, 80, 128);
            table.Rows.Add(474, 2, 85, 80, 70, 135, 75, 90, 258);
            table.Rows.Add(475, 4, 68, 125, 65, 65, 115, 80, 256);
            table.Rows.Add(476, 2, 60, 55, 145, 75, 150, 40, 128);
            table.Rows.Add(477, 1, 45, 100, 135, 65, 135, 45, 128);
            table.Rows.Add(478, 2, 70, 80, 70, 80, 70, 110, 257);
            table.Rows.Add(479, 2, 50, 65, 107, 105, 107, 86, 258);
            table.Rows.Add(480, 4, 75, 75, 130, 75, 130, 95, 258);
            table.Rows.Add(481, 4, 80, 105, 105, 105, 105, 80, 258);
            table.Rows.Add(482, 4, 75, 125, 70, 125, 70, 115, 258);
            table.Rows.Add(483, 4, 100, 120, 120, 150, 100, 90, 258);
            table.Rows.Add(484, 4, 90, 120, 100, 150, 120, 100, 258);
            table.Rows.Add(485, 4, 91, 90, 106, 130, 106, 77, 128);
            table.Rows.Add(486, 4, 110, 160, 110, 80, 110, 100, 258);
            table.Rows.Add(487, 4, 150, 100, 120, 100, 120, 90, 258);
            table.Rows.Add(488, 4, 120, 70, 120, 75, 130, 85, 257);
            table.Rows.Add(489, 4, 80, 80, 80, 80, 80, 80, 258);
            table.Rows.Add(490, 4, 100, 100, 100, 100, 100, 100, 258);
            table.Rows.Add(491, 4, 70, 90, 90, 135, 90, 125, 258);
            table.Rows.Add(492, 3, 100, 100, 100, 100, 100, 100, 258);
            table.Rows.Add(493, 4, 120, 120, 120, 120, 120, 120, 258);
            table.Rows.Add(494, 4, 100, 100, 100, 100, 100, 100, 258);
            table.Rows.Add(495, 3, 45, 45, 55, 45, 55, 63, 32);
            table.Rows.Add(496, 3, 60, 60, 75, 60, 75, 83, 32);
            table.Rows.Add(497, 3, 75, 75, 95, 75, 95, 113, 32);
            table.Rows.Add(498, 3, 65, 63, 45, 45, 45, 45, 32);
            table.Rows.Add(499, 3, 90, 93, 55, 70, 55, 55, 32);
            table.Rows.Add(500, 3, 110, 123, 65, 100, 65, 65, 32);
            table.Rows.Add(501, 3, 55, 55, 45, 63, 45, 45, 32);
            table.Rows.Add(502, 3, 75, 75, 60, 83, 60, 60, 32);
            table.Rows.Add(503, 3, 95, 100, 85, 108, 70, 70, 32);
            table.Rows.Add(504, 2, 45, 55, 39, 35, 39, 42, 128);
            table.Rows.Add(505, 2, 60, 85, 69, 60, 69, 77, 128);
            table.Rows.Add(506, 3, 45, 60, 45, 25, 45, 55, 128);
            table.Rows.Add(507, 3, 65, 80, 65, 35, 65, 60, 128);
            table.Rows.Add(508, 3, 85, 110, 90, 45, 90, 80, 128);
            table.Rows.Add(509, 2, 41, 50, 37, 50, 37, 66, 128);
            table.Rows.Add(510, 2, 64, 88, 50, 88, 50, 106, 128);
            table.Rows.Add(511, 2, 50, 53, 48, 53, 48, 64, 32);
            table.Rows.Add(512, 2, 75, 98, 63, 98, 63, 101, 32);
            table.Rows.Add(513, 2, 50, 53, 48, 53, 48, 64, 32);
            table.Rows.Add(514, 2, 75, 98, 63, 98, 63, 101, 32);
            table.Rows.Add(515, 2, 50, 53, 48, 53, 48, 64, 32);
            table.Rows.Add(516, 2, 75, 98, 63, 98, 63, 101, 32);
            table.Rows.Add(517, 1, 76, 25, 45, 67, 55, 24, 128);
            table.Rows.Add(518, 1, 116, 55, 85, 107, 95, 29, 128);
            table.Rows.Add(519, 3, 50, 55, 50, 36, 30, 43, 128);
            table.Rows.Add(520, 3, 62, 77, 62, 50, 42, 65, 128);
            table.Rows.Add(521, 3, 80, 115, 80, 65, 55, 93, 128);
            table.Rows.Add(522, 2, 45, 60, 32, 50, 32, 76, 128);
            table.Rows.Add(523, 2, 75, 100, 63, 80, 63, 116, 128);
            table.Rows.Add(524, 3, 55, 75, 85, 25, 25, 15, 128);
            table.Rows.Add(525, 3, 70, 105, 105, 50, 40, 20, 128);
            table.Rows.Add(526, 3, 85, 135, 130, 60, 80, 25, 128);
            table.Rows.Add(527, 2, 55, 45, 43, 55, 43, 72, 128);
            table.Rows.Add(528, 2, 67, 57, 55, 77, 55, 114, 128);
            table.Rows.Add(529, 2, 60, 85, 40, 30, 45, 68, 128);
            table.Rows.Add(530, 2, 110, 135, 60, 50, 65, 88, 128);
            table.Rows.Add(531, 1, 103, 60, 86, 60, 86, 50, 128);
            table.Rows.Add(532, 3, 75, 80, 55, 25, 35, 35, 64);
            table.Rows.Add(533, 3, 85, 105, 85, 40, 50, 40, 64);
            table.Rows.Add(534, 3, 105, 140, 95, 55, 65, 45, 64);
            table.Rows.Add(535, 3, 50, 50, 40, 50, 40, 64, 128);
            table.Rows.Add(536, 3, 75, 65, 55, 65, 55, 69, 128);
            table.Rows.Add(537, 3, 105, 95, 75, 85, 75, 74, 128);
            table.Rows.Add(538, 2, 120, 100, 85, 30, 85, 45, 256);
            table.Rows.Add(539, 2, 75, 125, 75, 30, 75, 85, 256);
            table.Rows.Add(540, 3, 45, 53, 70, 40, 60, 42, 128);
            table.Rows.Add(541, 3, 55, 63, 90, 50, 80, 42, 128);
            table.Rows.Add(542, 3, 75, 103, 80, 70, 80, 92, 128);
            table.Rows.Add(543, 3, 30, 45, 59, 30, 39, 57, 128);
            table.Rows.Add(544, 3, 40, 55, 99, 40, 79, 47, 128);
            table.Rows.Add(545, 3, 60, 100, 89, 55, 69, 112, 128);
            table.Rows.Add(546, 2, 40, 27, 60, 37, 50, 66, 128);
            table.Rows.Add(547, 2, 60, 67, 85, 77, 75, 116, 128);
            table.Rows.Add(548, 2, 45, 35, 50, 70, 50, 30, 257);
            table.Rows.Add(549, 2, 70, 60, 75, 110, 75, 90, 257);
            table.Rows.Add(550, 2, 70, 92, 65, 80, 55, 98, 128);
            table.Rows.Add(551, 3, 50, 72, 35, 35, 35, 65, 128);
            table.Rows.Add(552, 3, 60, 82, 45, 45, 45, 74, 128);
            table.Rows.Add(553, 3, 95, 117, 80, 65, 70, 92, 128);
            table.Rows.Add(554, 3, 70, 90, 45, 15, 45, 50, 128);
            table.Rows.Add(555, 3, 105, 140, 55, 30, 55, 95, 128);
            table.Rows.Add(556, 2, 75, 86, 67, 106, 67, 60, 128);
            table.Rows.Add(557, 2, 50, 65, 85, 35, 35, 55, 128);
            table.Rows.Add(558, 2, 70, 95, 125, 65, 75, 45, 128);
            table.Rows.Add(559, 2, 50, 75, 70, 35, 70, 48, 128);
            table.Rows.Add(560, 2, 65, 90, 115, 45, 115, 58, 128);
            table.Rows.Add(561, 2, 72, 58, 80, 103, 80, 97, 128);
            table.Rows.Add(562, 2, 38, 30, 85, 55, 65, 30, 128);
            table.Rows.Add(563, 2, 58, 50, 145, 95, 105, 30, 128);
            table.Rows.Add(564, 2, 54, 78, 103, 53, 45, 22, 32);
            table.Rows.Add(565, 2, 74, 108, 133, 83, 65, 32, 32);
            table.Rows.Add(566, 2, 55, 112, 45, 74, 45, 70, 32);
            table.Rows.Add(567, 2, 75, 140, 65, 112, 65, 110, 32);
            table.Rows.Add(568, 2, 50, 50, 62, 40, 62, 65, 128);
            table.Rows.Add(569, 2, 80, 95, 82, 60, 82, 75, 128);
            table.Rows.Add(570, 3, 40, 65, 40, 80, 40, 65, 32);
            table.Rows.Add(571, 3, 60, 105, 60, 120, 60, 105, 32);
            table.Rows.Add(572, 1, 55, 50, 40, 40, 40, 75, 192);
            table.Rows.Add(573, 1, 75, 95, 60, 65, 60, 115, 192);
            table.Rows.Add(574, 3, 45, 30, 50, 55, 65, 45, 192);
            table.Rows.Add(575, 3, 60, 45, 70, 75, 85, 55, 192);
            table.Rows.Add(576, 3, 70, 55, 95, 95, 110, 65, 192);
            table.Rows.Add(577, 3, 45, 30, 40, 105, 50, 20, 128);
            table.Rows.Add(578, 3, 65, 40, 50, 125, 60, 30, 128);
            table.Rows.Add(579, 3, 110, 65, 75, 125, 85, 30, 128);
            table.Rows.Add(580, 2, 62, 44, 50, 44, 50, 55, 128);
            table.Rows.Add(581, 2, 75, 87, 63, 87, 63, 98, 128);
            table.Rows.Add(582, 4, 36, 50, 50, 65, 60, 44, 128);
            table.Rows.Add(583, 4, 51, 65, 65, 80, 75, 59, 128);
            table.Rows.Add(584, 4, 71, 95, 85, 110, 95, 79, 128);
            table.Rows.Add(585, 2, 60, 60, 50, 40, 50, 75, 128);
            table.Rows.Add(586, 2, 80, 100, 70, 60, 70, 95, 128);
            table.Rows.Add(587, 2, 55, 75, 60, 75, 60, 103, 128);
            table.Rows.Add(588, 2, 50, 75, 45, 40, 45, 60, 128);
            table.Rows.Add(589, 2, 70, 135, 105, 60, 105, 20, 128);
            table.Rows.Add(590, 2, 69, 55, 45, 55, 55, 15, 128);
            table.Rows.Add(591, 2, 114, 85, 70, 85, 80, 30, 128);
            table.Rows.Add(592, 2, 55, 40, 50, 65, 85, 40, 128);
            table.Rows.Add(593, 2, 100, 60, 70, 85, 105, 60, 128);
            table.Rows.Add(594, 1, 165, 75, 80, 40, 45, 65, 128);
            table.Rows.Add(595, 2, 50, 47, 50, 57, 50, 65, 128);
            table.Rows.Add(596, 2, 70, 77, 60, 97, 60, 108, 128);
            table.Rows.Add(597, 2, 44, 50, 91, 24, 86, 10, 128);
            table.Rows.Add(598, 2, 74, 94, 131, 54, 116, 20, 128);
            table.Rows.Add(599, 3, 40, 55, 70, 45, 60, 30, 258);
            table.Rows.Add(600, 3, 60, 80, 95, 70, 85, 50, 258);
            table.Rows.Add(601, 3, 60, 100, 115, 70, 85, 90, 258);
            table.Rows.Add(602, 4, 35, 55, 40, 45, 40, 60, 128);
            table.Rows.Add(603, 4, 65, 85, 70, 75, 70, 40, 128);
            table.Rows.Add(604, 4, 85, 115, 80, 105, 80, 50, 128);
            table.Rows.Add(605, 2, 55, 55, 55, 85, 55, 30, 128);
            table.Rows.Add(606, 2, 75, 75, 75, 125, 95, 40, 128);
            table.Rows.Add(607, 3, 50, 30, 55, 65, 55, 20, 128);
            table.Rows.Add(608, 3, 60, 40, 60, 95, 60, 55, 128);
            table.Rows.Add(609, 3, 60, 55, 90, 145, 90, 80, 128);
            table.Rows.Add(610, 4, 46, 87, 60, 30, 40, 57, 128);
            table.Rows.Add(611, 4, 66, 117, 70, 40, 50, 67, 128);
            table.Rows.Add(612, 4, 76, 147, 90, 60, 70, 97, 128);
            table.Rows.Add(613, 2, 55, 70, 40, 60, 40, 40, 128);
            table.Rows.Add(614, 2, 95, 110, 80, 70, 80, 50, 128);
            table.Rows.Add(615, 2, 70, 50, 30, 95, 135, 105, 258);
            table.Rows.Add(616, 2, 50, 40, 85, 40, 65, 25, 128);
            table.Rows.Add(617, 2, 80, 70, 40, 100, 60, 145, 128);
            table.Rows.Add(618, 2, 109, 66, 84, 81, 99, 32, 128);
            table.Rows.Add(619, 3, 45, 85, 50, 55, 50, 65, 128);
            table.Rows.Add(620, 3, 65, 125, 60, 95, 60, 105, 128);
            table.Rows.Add(621, 2, 77, 120, 90, 60, 90, 48, 128);
            table.Rows.Add(622, 2, 59, 74, 50, 35, 50, 35, 258);
            table.Rows.Add(623, 2, 89, 124, 80, 55, 80, 55, 258);
            table.Rows.Add(624, 2, 45, 85, 70, 40, 40, 60, 128);
            table.Rows.Add(625, 2, 65, 125, 100, 60, 70, 70, 128);
            table.Rows.Add(626, 2, 95, 110, 95, 40, 95, 55, 128);
            table.Rows.Add(627, 4, 70, 83, 50, 37, 50, 60, 256);
            table.Rows.Add(628, 4, 100, 123, 75, 57, 75, 80, 256);
            table.Rows.Add(629, 4, 70, 55, 75, 45, 65, 60, 257);
            table.Rows.Add(630, 4, 110, 65, 105, 55, 95, 80, 257);
            table.Rows.Add(631, 2, 85, 97, 66, 105, 66, 65, 128);
            table.Rows.Add(632, 2, 58, 109, 112, 48, 48, 109, 128);
            table.Rows.Add(633, 4, 52, 65, 50, 45, 50, 38, 128);
            table.Rows.Add(634, 4, 72, 85, 70, 65, 70, 58, 128);
            table.Rows.Add(635, 4, 92, 105, 90, 125, 90, 98, 128);
            table.Rows.Add(636, 4, 55, 85, 55, 50, 55, 60, 128);
            table.Rows.Add(637, 4, 85, 60, 65, 135, 105, 100, 128);
            table.Rows.Add(638, 4, 91, 90, 129, 90, 72, 108, 258);
            table.Rows.Add(639, 4, 91, 129, 90, 72, 90, 108, 258);
            table.Rows.Add(640, 4, 91, 90, 72, 90, 129, 108, 258);
            table.Rows.Add(641, 4, 79, 115, 70, 125, 80, 111, 256);
            table.Rows.Add(642, 4, 79, 115, 70, 125, 80, 111, 256);
            table.Rows.Add(643, 4, 100, 120, 100, 150, 120, 90, 258);
            table.Rows.Add(644, 4, 100, 150, 120, 120, 100, 90, 258);
            table.Rows.Add(645, 4, 89, 125, 90, 115, 80, 101, 256);
            table.Rows.Add(646, 4, 125, 170, 100, 120, 90, 95, 258);
            table.Rows.Add(647, 4, 91, 72, 90, 129, 90, 108, 258);
            table.Rows.Add(648, 4, 100, 77, 77, 128, 128, 90, 258);
            table.Rows.Add(649, 4, 71, 120, 95, 120, 95, 99, 258);
            table.Rows.Add(650, 3, 56, 61, 65, 48, 45, 33, 32);
            table.Rows.Add(651, 3, 61, 78, 95, 56, 58, 57, 32);
            table.Rows.Add(652, 3, 88, 107, 122, 74, 75, 64, 32);
            table.Rows.Add(653, 3, 40, 45, 40, 62, 60, 60, 32);
            table.Rows.Add(654, 3, 59, 59, 58, 90, 70, 73, 32);
            table.Rows.Add(655, 3, 75, 69, 72, 114, 100, 104, 32);
            table.Rows.Add(656, 3, 41, 56, 40, 62, 44, 71, 32);
            table.Rows.Add(657, 3, 54, 63, 52, 83, 56, 97, 32);
            table.Rows.Add(658, 3, 72, 95, 67, 103, 71, 122, 32);
            table.Rows.Add(659, 2, 38, 36, 38, 32, 36, 57, 128);
            table.Rows.Add(660, 2, 85, 56, 77, 50, 77, 78, 128);
            table.Rows.Add(661, 3, 45, 50, 43, 40, 38, 62, 128);
            table.Rows.Add(662, 3, 62, 73, 55, 56, 52, 84, 128);
            table.Rows.Add(663, 3, 78, 81, 71, 74, 69, 126, 128);
            table.Rows.Add(664, 2, 38, 35, 40, 27, 25, 35, 128);
            table.Rows.Add(665, 2, 45, 22, 60, 27, 30, 29, 128);
            table.Rows.Add(666, 2, 80, 52, 50, 90, 50, 89, 128);
            table.Rows.Add(667, 3, 62, 50, 58, 73, 54, 72, 192);
            table.Rows.Add(668, 3, 86, 68, 72, 109, 66, 106, 192);
            table.Rows.Add(669, 2, 44, 38, 39, 61, 79, 42, 257);
            table.Rows.Add(670, 2, 54, 45, 47, 75, 98, 52, 257);
            table.Rows.Add(671, 2, 78, 65, 68, 112, 154, 75, 257);
            table.Rows.Add(672, 2, 66, 65, 48, 62, 57, 52, 128);
            table.Rows.Add(673, 2, 123, 100, 62, 97, 81, 68, 128);
            table.Rows.Add(674, 2, 67, 82, 62, 46, 48, 43, 128);
            table.Rows.Add(675, 2, 95, 124, 78, 69, 71, 58, 128);
            table.Rows.Add(676, 2, 75, 80, 60, 65, 90, 102, 128);
            table.Rows.Add(677, 2, 62, 48, 54, 63, 60, 68, 128);
            table.Rows.Add(678, 2, 74, 48, 76, 83, 81, 104, 128);
            table.Rows.Add(679, 2, 45, 80, 100, 35, 37, 28, 128);
            table.Rows.Add(680, 2, 59, 110, 150, 45, 49, 35, 128);
            table.Rows.Add(681, 2, 60, 150, 50, 150, 50, 60, 128);
            table.Rows.Add(682, 2, 78, 52, 60, 63, 65, 23, 128);
            table.Rows.Add(683, 2, 101, 72, 72, 99, 89, 29, 128);
            table.Rows.Add(684, 2, 62, 48, 66, 59, 57, 49, 128);
            table.Rows.Add(685, 2, 82, 80, 86, 85, 75, 72, 128);
            table.Rows.Add(686, 2, 53, 54, 37, 46, 45, 45, 128);
            table.Rows.Add(687, 2, 86, 92, 88, 68, 75, 73, 128);
            table.Rows.Add(688, 2, 42, 52, 67, 39, 56, 50, 128);
            table.Rows.Add(689, 2, 72, 105, 115, 54, 86, 68, 128);
            table.Rows.Add(690, 2, 50, 60, 60, 60, 60, 30, 128);
            table.Rows.Add(691, 2, 65, 75, 90, 97, 123, 44, 128);
            table.Rows.Add(692, 4, 50, 53, 62, 58, 63, 44, 128);
            table.Rows.Add(693, 4, 71, 73, 88, 120, 89, 59, 128);
            table.Rows.Add(694, 2, 44, 38, 33, 61, 43, 70, 128);
            table.Rows.Add(695, 2, 62, 55, 52, 109, 94, 109, 128);
            table.Rows.Add(696, 2, 58, 89, 77, 45, 45, 48, 32);
            table.Rows.Add(697, 2, 82, 121, 119, 69, 59, 71, 32);
            table.Rows.Add(698, 2, 77, 59, 50, 67, 63, 46, 32);
            table.Rows.Add(699, 2, 123, 77, 72, 99, 92, 58, 32);
            table.Rows.Add(700, 2, 95, 65, 65, 110, 130, 60, 32);
            table.Rows.Add(701, 2, 78, 92, 75, 74, 63, 118, 128);
            table.Rows.Add(702, 2, 67, 58, 57, 81, 67, 101, 128);
            table.Rows.Add(703, 4, 50, 50, 150, 50, 150, 50, 258);
            table.Rows.Add(704, 4, 45, 50, 35, 55, 75, 40, 128);
            table.Rows.Add(705, 4, 68, 75, 53, 83, 113, 60, 128);
            table.Rows.Add(706, 4, 90, 100, 70, 110, 150, 80, 128);
            table.Rows.Add(707, 1, 57, 80, 91, 80, 87, 75, 128);
            table.Rows.Add(708, 2, 43, 70, 48, 50, 60, 38, 128);
            table.Rows.Add(709, 2, 85, 110, 76, 65, 82, 56, 128);
            table.Rows.Add(710, 2, 49, 66, 70, 44, 55, 51, 128);
            table.Rows.Add(711, 2, 65, 90, 122, 58, 75, 84, 128);
            table.Rows.Add(712, 2, 55, 69, 85, 32, 35, 28, 128);
            table.Rows.Add(713, 2, 95, 117, 184, 44, 46, 28, 128);
            table.Rows.Add(714, 2, 40, 30, 35, 45, 40, 55, 128);
            table.Rows.Add(715, 2, 85, 70, 80, 97, 80, 123, 128);
            table.Rows.Add(716, 4, 126, 131, 95, 131, 98, 99, 258);
            table.Rows.Add(717, 4, 126, 131, 95, 131, 98, 99, 258);
            table.Rows.Add(718, 4, 108, 100, 121, 81, 95, 95, 258);
            table.Rows.Add(719, 4, 50, 100, 150, 100, 150, 50, 258);    // Diancie
            table.Rows.Add(720, 4, 80, 110, 60, 150, 130, 70, 258);  // Hoopa
            table.Rows.Add(721, 4, 80, 110, 120, 130, 90, 70, 258);  // Volcanion
            table.Rows.Add(722, 4, 100, 100, 100, 100, 100, 100, 258);

            return table;
        }
        static DataTable ExpTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Level", typeof(int));

            table.Columns.Add("0 - Erratic", typeof(int));
            table.Columns.Add("1 - Fast", typeof(int));
            table.Columns.Add("2 - MF", typeof(int));
            table.Columns.Add("3 - MS", typeof(int));
            table.Columns.Add("4 - Slow", typeof(int));
            table.Columns.Add("5 - Fluctuating", typeof(int));
            table.Rows.Add(0, 0, 0, 0, 0, 0, 0);
            table.Rows.Add(1, 0, 0, 0, 0, 0, 0);
            table.Rows.Add(2, 15, 6, 8, 9, 10, 4);
            table.Rows.Add(3, 52, 21, 27, 57, 33, 13);
            table.Rows.Add(4, 122, 51, 64, 96, 80, 32);
            table.Rows.Add(5, 237, 100, 125, 135, 156, 65);
            table.Rows.Add(6, 406, 172, 216, 179, 270, 112);
            table.Rows.Add(7, 637, 274, 343, 236, 428, 178);
            table.Rows.Add(8, 942, 409, 512, 314, 640, 276);
            table.Rows.Add(9, 1326, 583, 729, 419, 911, 393);
            table.Rows.Add(10, 1800, 800, 1000, 560, 1250, 540);
            table.Rows.Add(11, 2369, 1064, 1331, 742, 1663, 745);
            table.Rows.Add(12, 3041, 1382, 1728, 973, 2160, 967);
            table.Rows.Add(13, 3822, 1757, 2197, 1261, 2746, 1230);
            table.Rows.Add(14, 4719, 2195, 2744, 1612, 3430, 1591);
            table.Rows.Add(15, 5737, 2700, 3375, 2035, 4218, 1957);
            table.Rows.Add(16, 6881, 3276, 4096, 2535, 5120, 2457);
            table.Rows.Add(17, 8155, 3930, 4913, 3120, 6141, 3046);
            table.Rows.Add(18, 9564, 4665, 5832, 3798, 7290, 3732);
            table.Rows.Add(19, 11111, 5487, 6859, 4575, 8573, 4526);
            table.Rows.Add(20, 12800, 6400, 8000, 5460, 10000, 5440);
            table.Rows.Add(21, 14632, 7408, 9261, 6458, 11576, 6482);
            table.Rows.Add(22, 16610, 8518, 10648, 7577, 13310, 7666);
            table.Rows.Add(23, 18737, 9733, 12167, 8825, 15208, 9003);
            table.Rows.Add(24, 21012, 11059, 13824, 10208, 17280, 10506);
            table.Rows.Add(25, 23437, 12500, 15625, 11735, 19531, 12187);
            table.Rows.Add(26, 26012, 14060, 17576, 13411, 21970, 14060);
            table.Rows.Add(27, 28737, 15746, 19683, 15244, 24603, 16140);
            table.Rows.Add(28, 31610, 17561, 21952, 17242, 27440, 18439);
            table.Rows.Add(29, 34632, 19511, 24389, 19411, 30486, 20974);
            table.Rows.Add(30, 37800, 21600, 27000, 21760, 33750, 23760);
            table.Rows.Add(31, 41111, 23832, 29791, 24294, 37238, 26811);
            table.Rows.Add(32, 44564, 26214, 32768, 27021, 40960, 30146);
            table.Rows.Add(33, 48155, 28749, 35937, 29949, 44921, 33780);
            table.Rows.Add(34, 51881, 31443, 39304, 33084, 49130, 37731);
            table.Rows.Add(35, 55737, 34300, 42875, 36435, 53593, 42017);
            table.Rows.Add(36, 59719, 37324, 46656, 40007, 58320, 46656);
            table.Rows.Add(37, 63822, 40522, 50653, 43808, 63316, 50653);
            table.Rows.Add(38, 68041, 43897, 54872, 47846, 68590, 55969);
            table.Rows.Add(39, 72369, 47455, 59319, 52127, 74148, 60505);
            table.Rows.Add(40, 76800, 51200, 64000, 56660, 80000, 66560);
            table.Rows.Add(41, 81326, 55136, 68921, 61450, 86151, 71677);
            table.Rows.Add(42, 85942, 59270, 74088, 66505, 92610, 78533);
            table.Rows.Add(43, 90637, 63605, 79507, 71833, 99383, 84277);
            table.Rows.Add(44, 95406, 68147, 85184, 77440, 106480, 91998);
            table.Rows.Add(45, 100237, 72900, 91125, 83335, 113906, 98415);
            table.Rows.Add(46, 105122, 77868, 97336, 89523, 121670, 107069);
            table.Rows.Add(47, 110052, 83058, 103823, 96012, 129778, 114205);
            table.Rows.Add(48, 115015, 88473, 110592, 102810, 138240, 123863);
            table.Rows.Add(49, 120001, 94119, 117649, 109923, 147061, 131766);
            table.Rows.Add(50, 125000, 100000, 125000, 117360, 156250, 142500);
            table.Rows.Add(51, 131324, 106120, 132651, 125126, 165813, 151222);
            table.Rows.Add(52, 137795, 112486, 140608, 133229, 175760, 163105);
            table.Rows.Add(53, 144410, 119101, 148877, 141677, 186096, 172697);
            table.Rows.Add(54, 151165, 125971, 157464, 150476, 196830, 185807);
            table.Rows.Add(55, 158056, 133100, 166375, 159635, 207968, 196322);
            table.Rows.Add(56, 165079, 140492, 175616, 169159, 219520, 210739);
            table.Rows.Add(57, 172229, 148154, 185193, 179056, 231491, 222231);
            table.Rows.Add(58, 179503, 156089, 195112, 189334, 243890, 238036);
            table.Rows.Add(59, 186894, 164303, 205379, 199999, 256723, 250562);
            table.Rows.Add(60, 194400, 172800, 216000, 211060, 270000, 267840);
            table.Rows.Add(61, 202013, 181584, 226981, 222522, 283726, 281456);
            table.Rows.Add(62, 209728, 190662, 238328, 234393, 297910, 300293);
            table.Rows.Add(63, 217540, 200037, 250047, 246681, 312558, 315059);
            table.Rows.Add(64, 225443, 209715, 262144, 259392, 327680, 335544);
            table.Rows.Add(65, 233431, 219700, 274625, 272535, 343281, 351520);
            table.Rows.Add(66, 241496, 229996, 287496, 286115, 359370, 373744);
            table.Rows.Add(67, 249633, 240610, 300763, 300140, 375953, 390991);
            table.Rows.Add(68, 257834, 251545, 314432, 314618, 393040, 415050);
            table.Rows.Add(69, 267406, 262807, 328509, 329555, 410636, 433631);
            table.Rows.Add(70, 276458, 274400, 343000, 344960, 428750, 459620);
            table.Rows.Add(71, 286328, 286328, 357911, 360838, 447388, 479600);
            table.Rows.Add(72, 296358, 298598, 373248, 377197, 466560, 507617);
            table.Rows.Add(73, 305767, 311213, 389017, 394045, 486271, 529063);
            table.Rows.Add(74, 316074, 324179, 405224, 411388, 506530, 559209);
            table.Rows.Add(75, 326531, 337500, 421875, 429235, 527343, 582187);
            table.Rows.Add(76, 336255, 351180, 438976, 447591, 548720, 614566);
            table.Rows.Add(77, 346965, 365226, 456533, 466464, 570666, 639146);
            table.Rows.Add(78, 357812, 379641, 474552, 485862, 593190, 673863);
            table.Rows.Add(79, 367807, 394431, 493039, 505791, 616298, 700115);
            table.Rows.Add(80, 378880, 409600, 512000, 526260, 640000, 737280);
            table.Rows.Add(81, 390077, 425152, 531441, 547274, 664301, 765275);
            table.Rows.Add(82, 400293, 441094, 551368, 568841, 689210, 804997);
            table.Rows.Add(83, 411686, 457429, 571787, 590969, 714733, 834809);
            table.Rows.Add(84, 423190, 474163, 592704, 613664, 740880, 877201);
            table.Rows.Add(85, 433572, 491300, 614125, 636935, 767656, 908905);
            table.Rows.Add(86, 445239, 508844, 636056, 660787, 795070, 954084);
            table.Rows.Add(87, 457001, 526802, 658503, 685228, 823128, 987754);
            table.Rows.Add(88, 467489, 545177, 681472, 710266, 851840, 1035837);
            table.Rows.Add(89, 479378, 563975, 704969, 735907, 881211, 1071552);
            table.Rows.Add(90, 491346, 583200, 729000, 762160, 911250, 1122660);
            table.Rows.Add(91, 501878, 602856, 753571, 789030, 941963, 1160499);
            table.Rows.Add(92, 513934, 622950, 778688, 816525, 973360, 1214753);
            table.Rows.Add(93, 526049, 643485, 804357, 844653, 1005446, 1254796);
            table.Rows.Add(94, 536557, 664467, 830584, 873420, 1038230, 1312322);
            table.Rows.Add(95, 548720, 685900, 857375, 902835, 1071718, 1354652);
            table.Rows.Add(96, 560922, 707788, 884736, 932903, 1105920, 1415577);
            table.Rows.Add(97, 571333, 730138, 912673, 963632, 1140841, 1460276);
            table.Rows.Add(98, 583539, 752953, 941192, 995030, 1176490, 1524731);
            table.Rows.Add(99, 591882, 776239, 970299, 1027103, 1212873, 1571884);
            table.Rows.Add(100, 600000, 800000, 1000000, 1059860, 1250000, 1640000);
            return table;
        }
        static DataTable MovePPTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Move", typeof(int));
            table.Columns.Add("PP", typeof(int));
            table.Rows.Add(0, 0);
            table.Rows.Add(1, 35);
            table.Rows.Add(2, 25);
            table.Rows.Add(3, 10);
            table.Rows.Add(4, 15);
            table.Rows.Add(5, 20);
            table.Rows.Add(6, 20);
            table.Rows.Add(7, 15);
            table.Rows.Add(8, 15);
            table.Rows.Add(9, 15);
            table.Rows.Add(10, 35);
            table.Rows.Add(11, 30);
            table.Rows.Add(12, 5);
            table.Rows.Add(13, 10);
            table.Rows.Add(14, 20);
            table.Rows.Add(15, 30);
            table.Rows.Add(16, 35);
            table.Rows.Add(17, 35);
            table.Rows.Add(18, 20);
            table.Rows.Add(19, 15);
            table.Rows.Add(20, 20);
            table.Rows.Add(21, 20);
            table.Rows.Add(22, 25);
            table.Rows.Add(23, 20);
            table.Rows.Add(24, 30);
            table.Rows.Add(25, 5);
            table.Rows.Add(26, 10);
            table.Rows.Add(27, 15);
            table.Rows.Add(28, 15);
            table.Rows.Add(29, 15);
            table.Rows.Add(30, 25);
            table.Rows.Add(31, 20);
            table.Rows.Add(32, 5);
            table.Rows.Add(33, 35);
            table.Rows.Add(34, 15);
            table.Rows.Add(35, 20);
            table.Rows.Add(36, 20);
            table.Rows.Add(37, 10);
            table.Rows.Add(38, 15);
            table.Rows.Add(39, 30);
            table.Rows.Add(40, 35);
            table.Rows.Add(41, 20);
            table.Rows.Add(42, 20);
            table.Rows.Add(43, 30);
            table.Rows.Add(44, 25);
            table.Rows.Add(45, 40);
            table.Rows.Add(46, 20);
            table.Rows.Add(47, 15);
            table.Rows.Add(48, 20);
            table.Rows.Add(49, 20);
            table.Rows.Add(50, 20);
            table.Rows.Add(51, 30);
            table.Rows.Add(52, 25);
            table.Rows.Add(53, 15);
            table.Rows.Add(54, 30);
            table.Rows.Add(55, 25);
            table.Rows.Add(56, 5);
            table.Rows.Add(57, 15);
            table.Rows.Add(58, 10);
            table.Rows.Add(59, 5);
            table.Rows.Add(60, 20);
            table.Rows.Add(61, 20);
            table.Rows.Add(62, 20);
            table.Rows.Add(63, 5);
            table.Rows.Add(64, 35);
            table.Rows.Add(65, 20);
            table.Rows.Add(66, 25);
            table.Rows.Add(67, 20);
            table.Rows.Add(68, 20);
            table.Rows.Add(69, 20);
            table.Rows.Add(70, 15);
            table.Rows.Add(71, 25);
            table.Rows.Add(72, 15);
            table.Rows.Add(73, 10);
            table.Rows.Add(74, 20);
            table.Rows.Add(75, 25);
            table.Rows.Add(76, 10);
            table.Rows.Add(77, 35);
            table.Rows.Add(78, 30);
            table.Rows.Add(79, 15);
            table.Rows.Add(80, 10);
            table.Rows.Add(81, 40);
            table.Rows.Add(82, 10);
            table.Rows.Add(83, 15);
            table.Rows.Add(84, 30);
            table.Rows.Add(85, 15);
            table.Rows.Add(86, 20);
            table.Rows.Add(87, 10);
            table.Rows.Add(88, 15);
            table.Rows.Add(89, 10);
            table.Rows.Add(90, 5);
            table.Rows.Add(91, 10);
            table.Rows.Add(92, 10);
            table.Rows.Add(93, 25);
            table.Rows.Add(94, 10);
            table.Rows.Add(95, 20);
            table.Rows.Add(96, 40);
            table.Rows.Add(97, 30);
            table.Rows.Add(98, 30);
            table.Rows.Add(99, 20);
            table.Rows.Add(100, 20);
            table.Rows.Add(101, 15);
            table.Rows.Add(102, 10);
            table.Rows.Add(103, 40);
            table.Rows.Add(104, 15);
            table.Rows.Add(105, 10);
            table.Rows.Add(106, 30);
            table.Rows.Add(107, 10);
            table.Rows.Add(108, 20);
            table.Rows.Add(109, 10);
            table.Rows.Add(110, 40);
            table.Rows.Add(111, 40);
            table.Rows.Add(112, 20);
            table.Rows.Add(113, 30);
            table.Rows.Add(114, 30);
            table.Rows.Add(115, 20);
            table.Rows.Add(116, 30);
            table.Rows.Add(117, 10);
            table.Rows.Add(118, 10);
            table.Rows.Add(119, 20);
            table.Rows.Add(120, 5);
            table.Rows.Add(121, 10);
            table.Rows.Add(122, 30);
            table.Rows.Add(123, 20);
            table.Rows.Add(124, 20);
            table.Rows.Add(125, 20);
            table.Rows.Add(126, 5);
            table.Rows.Add(127, 15);
            table.Rows.Add(128, 10);
            table.Rows.Add(129, 20);
            table.Rows.Add(130, 10);
            table.Rows.Add(131, 15);
            table.Rows.Add(132, 35);
            table.Rows.Add(133, 20);
            table.Rows.Add(134, 15);
            table.Rows.Add(135, 10);
            table.Rows.Add(136, 10);
            table.Rows.Add(137, 30);
            table.Rows.Add(138, 15);
            table.Rows.Add(139, 40);
            table.Rows.Add(140, 20);
            table.Rows.Add(141, 15);
            table.Rows.Add(142, 10);
            table.Rows.Add(143, 5);
            table.Rows.Add(144, 10);
            table.Rows.Add(145, 30);
            table.Rows.Add(146, 10);
            table.Rows.Add(147, 15);
            table.Rows.Add(148, 20);
            table.Rows.Add(149, 15);
            table.Rows.Add(150, 40);
            table.Rows.Add(151, 20);
            table.Rows.Add(152, 10);
            table.Rows.Add(153, 5);
            table.Rows.Add(154, 15);
            table.Rows.Add(155, 10);
            table.Rows.Add(156, 10);
            table.Rows.Add(157, 10);
            table.Rows.Add(158, 15);
            table.Rows.Add(159, 30);
            table.Rows.Add(160, 30);
            table.Rows.Add(161, 10);
            table.Rows.Add(162, 10);
            table.Rows.Add(163, 20);
            table.Rows.Add(164, 10);
            table.Rows.Add(165, 1);
            table.Rows.Add(166, 1);
            table.Rows.Add(167, 10);
            table.Rows.Add(168, 25);
            table.Rows.Add(169, 10);
            table.Rows.Add(170, 5);
            table.Rows.Add(171, 15);
            table.Rows.Add(172, 25);
            table.Rows.Add(173, 15);
            table.Rows.Add(174, 10);
            table.Rows.Add(175, 15);
            table.Rows.Add(176, 30);
            table.Rows.Add(177, 5);
            table.Rows.Add(178, 40);
            table.Rows.Add(179, 15);
            table.Rows.Add(180, 10);
            table.Rows.Add(181, 25);
            table.Rows.Add(182, 10);
            table.Rows.Add(183, 30);
            table.Rows.Add(184, 10);
            table.Rows.Add(185, 20);
            table.Rows.Add(186, 10);
            table.Rows.Add(187, 10);
            table.Rows.Add(188, 10);
            table.Rows.Add(189, 10);
            table.Rows.Add(190, 10);
            table.Rows.Add(191, 20);
            table.Rows.Add(192, 5);
            table.Rows.Add(193, 40);
            table.Rows.Add(194, 5);
            table.Rows.Add(195, 5);
            table.Rows.Add(196, 15);
            table.Rows.Add(197, 5);
            table.Rows.Add(198, 10);
            table.Rows.Add(199, 5);
            table.Rows.Add(200, 15);
            table.Rows.Add(201, 10);
            table.Rows.Add(202, 10);
            table.Rows.Add(203, 10);
            table.Rows.Add(204, 20);
            table.Rows.Add(205, 20);
            table.Rows.Add(206, 40);
            table.Rows.Add(207, 15);
            table.Rows.Add(208, 10);
            table.Rows.Add(209, 20);
            table.Rows.Add(210, 20);
            table.Rows.Add(211, 25);
            table.Rows.Add(212, 5);
            table.Rows.Add(213, 15);
            table.Rows.Add(214, 10);
            table.Rows.Add(215, 5);
            table.Rows.Add(216, 20);
            table.Rows.Add(217, 15);
            table.Rows.Add(218, 20);
            table.Rows.Add(219, 25);
            table.Rows.Add(220, 20);
            table.Rows.Add(221, 5);
            table.Rows.Add(222, 30);
            table.Rows.Add(223, 5);
            table.Rows.Add(224, 10);
            table.Rows.Add(225, 20);
            table.Rows.Add(226, 40);
            table.Rows.Add(227, 5);
            table.Rows.Add(228, 20);
            table.Rows.Add(229, 40);
            table.Rows.Add(230, 20);
            table.Rows.Add(231, 15);
            table.Rows.Add(232, 35);
            table.Rows.Add(233, 10);
            table.Rows.Add(234, 5);
            table.Rows.Add(235, 5);
            table.Rows.Add(236, 5);
            table.Rows.Add(237, 15);
            table.Rows.Add(238, 5);
            table.Rows.Add(239, 20);
            table.Rows.Add(240, 5);
            table.Rows.Add(241, 5);
            table.Rows.Add(242, 15);
            table.Rows.Add(243, 20);
            table.Rows.Add(244, 10);
            table.Rows.Add(245, 5);
            table.Rows.Add(246, 5);
            table.Rows.Add(247, 15);
            table.Rows.Add(248, 10);
            table.Rows.Add(249, 15);
            table.Rows.Add(250, 15);
            table.Rows.Add(251, 10);
            table.Rows.Add(252, 10);
            table.Rows.Add(253, 10);
            table.Rows.Add(254, 20);
            table.Rows.Add(255, 10);
            table.Rows.Add(256, 10);
            table.Rows.Add(257, 10);
            table.Rows.Add(258, 10);
            table.Rows.Add(259, 15);
            table.Rows.Add(260, 15);
            table.Rows.Add(261, 15);
            table.Rows.Add(262, 10);
            table.Rows.Add(263, 20);
            table.Rows.Add(264, 20);
            table.Rows.Add(265, 10);
            table.Rows.Add(266, 20);
            table.Rows.Add(267, 20);
            table.Rows.Add(268, 20);
            table.Rows.Add(269, 20);
            table.Rows.Add(270, 20);
            table.Rows.Add(271, 10);
            table.Rows.Add(272, 10);
            table.Rows.Add(273, 10);
            table.Rows.Add(274, 20);
            table.Rows.Add(275, 20);
            table.Rows.Add(276, 5);
            table.Rows.Add(277, 15);
            table.Rows.Add(278, 10);
            table.Rows.Add(279, 10);
            table.Rows.Add(280, 15);
            table.Rows.Add(281, 10);
            table.Rows.Add(282, 20);
            table.Rows.Add(283, 5);
            table.Rows.Add(284, 5);
            table.Rows.Add(285, 10);
            table.Rows.Add(286, 10);
            table.Rows.Add(287, 20);
            table.Rows.Add(288, 5);
            table.Rows.Add(289, 10);
            table.Rows.Add(290, 20);
            table.Rows.Add(291, 10);
            table.Rows.Add(292, 20);
            table.Rows.Add(293, 20);
            table.Rows.Add(294, 20);
            table.Rows.Add(295, 5);
            table.Rows.Add(296, 5);
            table.Rows.Add(297, 15);
            table.Rows.Add(298, 20);
            table.Rows.Add(299, 10);
            table.Rows.Add(300, 15);
            table.Rows.Add(301, 20);
            table.Rows.Add(302, 15);
            table.Rows.Add(303, 10);
            table.Rows.Add(304, 10);
            table.Rows.Add(305, 15);
            table.Rows.Add(306, 10);
            table.Rows.Add(307, 5);
            table.Rows.Add(308, 5);
            table.Rows.Add(309, 10);
            table.Rows.Add(310, 15);
            table.Rows.Add(311, 10);
            table.Rows.Add(312, 5);
            table.Rows.Add(313, 20);
            table.Rows.Add(314, 25);
            table.Rows.Add(315, 5);
            table.Rows.Add(316, 40);
            table.Rows.Add(317, 15);
            table.Rows.Add(318, 5);
            table.Rows.Add(319, 40);
            table.Rows.Add(320, 15);
            table.Rows.Add(321, 20);
            table.Rows.Add(322, 20);
            table.Rows.Add(323, 5);
            table.Rows.Add(324, 15);
            table.Rows.Add(325, 20);
            table.Rows.Add(326, 20);
            table.Rows.Add(327, 15);
            table.Rows.Add(328, 15);
            table.Rows.Add(329, 5);
            table.Rows.Add(330, 10);
            table.Rows.Add(331, 30);
            table.Rows.Add(332, 20);
            table.Rows.Add(333, 30);
            table.Rows.Add(334, 15);
            table.Rows.Add(335, 5);
            table.Rows.Add(336, 40);
            table.Rows.Add(337, 15);
            table.Rows.Add(338, 5);
            table.Rows.Add(339, 20);
            table.Rows.Add(340, 5);
            table.Rows.Add(341, 15);
            table.Rows.Add(342, 25);
            table.Rows.Add(343, 25);
            table.Rows.Add(344, 15);
            table.Rows.Add(345, 20);
            table.Rows.Add(346, 15);
            table.Rows.Add(347, 20);
            table.Rows.Add(348, 15);
            table.Rows.Add(349, 20);
            table.Rows.Add(350, 10);
            table.Rows.Add(351, 20);
            table.Rows.Add(352, 20);
            table.Rows.Add(353, 5);
            table.Rows.Add(354, 5);
            table.Rows.Add(355, 10);
            table.Rows.Add(356, 5);
            table.Rows.Add(357, 40);
            table.Rows.Add(358, 10);
            table.Rows.Add(359, 10);
            table.Rows.Add(360, 5);
            table.Rows.Add(361, 10);
            table.Rows.Add(362, 10);
            table.Rows.Add(363, 15);
            table.Rows.Add(364, 10);
            table.Rows.Add(365, 20);
            table.Rows.Add(366, 15);
            table.Rows.Add(367, 30);
            table.Rows.Add(368, 10);
            table.Rows.Add(369, 20);
            table.Rows.Add(370, 5);
            table.Rows.Add(371, 10);
            table.Rows.Add(372, 10);
            table.Rows.Add(373, 15);
            table.Rows.Add(374, 10);
            table.Rows.Add(375, 10);
            table.Rows.Add(376, 5);
            table.Rows.Add(377, 15);
            table.Rows.Add(378, 5);
            table.Rows.Add(379, 10);
            table.Rows.Add(380, 10);
            table.Rows.Add(381, 30);
            table.Rows.Add(382, 20);
            table.Rows.Add(383, 20);
            table.Rows.Add(384, 10);
            table.Rows.Add(385, 10);
            table.Rows.Add(386, 5);
            table.Rows.Add(387, 5);
            table.Rows.Add(388, 10);
            table.Rows.Add(389, 5);
            table.Rows.Add(390, 20);
            table.Rows.Add(391, 10);
            table.Rows.Add(392, 20);
            table.Rows.Add(393, 10);
            table.Rows.Add(394, 15);
            table.Rows.Add(395, 10);
            table.Rows.Add(396, 20);
            table.Rows.Add(397, 20);
            table.Rows.Add(398, 20);
            table.Rows.Add(399, 15);
            table.Rows.Add(400, 15);
            table.Rows.Add(401, 10);
            table.Rows.Add(402, 15);
            table.Rows.Add(403, 15);
            table.Rows.Add(404, 15);
            table.Rows.Add(405, 10);
            table.Rows.Add(406, 10);
            table.Rows.Add(407, 10);
            table.Rows.Add(408, 20);
            table.Rows.Add(409, 10);
            table.Rows.Add(410, 30);
            table.Rows.Add(411, 5);
            table.Rows.Add(412, 10);
            table.Rows.Add(413, 15);
            table.Rows.Add(414, 10);
            table.Rows.Add(415, 10);
            table.Rows.Add(416, 5);
            table.Rows.Add(417, 20);
            table.Rows.Add(418, 30);
            table.Rows.Add(419, 10);
            table.Rows.Add(420, 30);
            table.Rows.Add(421, 15);
            table.Rows.Add(422, 15);
            table.Rows.Add(423, 15);
            table.Rows.Add(424, 15);
            table.Rows.Add(425, 30);
            table.Rows.Add(426, 10);
            table.Rows.Add(427, 20);
            table.Rows.Add(428, 15);
            table.Rows.Add(429, 10);
            table.Rows.Add(430, 10);
            table.Rows.Add(431, 20);
            table.Rows.Add(432, 15);
            table.Rows.Add(433, 5);
            table.Rows.Add(434, 5);
            table.Rows.Add(435, 15);
            table.Rows.Add(436, 15);
            table.Rows.Add(437, 5);
            table.Rows.Add(438, 10);
            table.Rows.Add(439, 5);
            table.Rows.Add(440, 20);
            table.Rows.Add(441, 5);
            table.Rows.Add(442, 15);
            table.Rows.Add(443, 20);
            table.Rows.Add(444, 5);
            table.Rows.Add(445, 20);
            table.Rows.Add(446, 20);
            table.Rows.Add(447, 20);
            table.Rows.Add(448, 20);
            table.Rows.Add(449, 10);
            table.Rows.Add(450, 20);
            table.Rows.Add(451, 10);
            table.Rows.Add(452, 15);
            table.Rows.Add(453, 20);
            table.Rows.Add(454, 15);
            table.Rows.Add(455, 10);
            table.Rows.Add(456, 10);
            table.Rows.Add(457, 5);
            table.Rows.Add(458, 10);
            table.Rows.Add(459, 5);
            table.Rows.Add(460, 5);
            table.Rows.Add(461, 10);
            table.Rows.Add(462, 5);
            table.Rows.Add(463, 5);
            table.Rows.Add(464, 10);
            table.Rows.Add(465, 5);
            table.Rows.Add(466, 5);
            table.Rows.Add(467, 5);
            table.Rows.Add(468, 15);
            table.Rows.Add(469, 10);
            table.Rows.Add(470, 10);
            table.Rows.Add(471, 10);
            table.Rows.Add(472, 10);
            table.Rows.Add(473, 10);
            table.Rows.Add(474, 10);
            table.Rows.Add(475, 15);
            table.Rows.Add(476, 20);
            table.Rows.Add(477, 15);
            table.Rows.Add(478, 10);
            table.Rows.Add(479, 15);
            table.Rows.Add(480, 10);
            table.Rows.Add(481, 15);
            table.Rows.Add(482, 10);
            table.Rows.Add(483, 20);
            table.Rows.Add(484, 10);
            table.Rows.Add(485, 15);
            table.Rows.Add(486, 10);
            table.Rows.Add(487, 20);
            table.Rows.Add(488, 20);
            table.Rows.Add(489, 20);
            table.Rows.Add(490, 20);
            table.Rows.Add(491, 20);
            table.Rows.Add(492, 15);
            table.Rows.Add(493, 15);
            table.Rows.Add(494, 15);
            table.Rows.Add(495, 15);
            table.Rows.Add(496, 15);
            table.Rows.Add(497, 15);
            table.Rows.Add(498, 20);
            table.Rows.Add(499, 15);
            table.Rows.Add(500, 10);
            table.Rows.Add(501, 15);
            table.Rows.Add(502, 15);
            table.Rows.Add(503, 15);
            table.Rows.Add(504, 15);
            table.Rows.Add(505, 10);
            table.Rows.Add(506, 10);
            table.Rows.Add(507, 10);
            table.Rows.Add(508, 10);
            table.Rows.Add(509, 10);
            table.Rows.Add(510, 15);
            table.Rows.Add(511, 15);
            table.Rows.Add(512, 15);
            table.Rows.Add(513, 15);
            table.Rows.Add(514, 5);
            table.Rows.Add(515, 5);
            table.Rows.Add(516, 15);
            table.Rows.Add(517, 5);
            table.Rows.Add(518, 10);
            table.Rows.Add(519, 10);
            table.Rows.Add(520, 10);
            table.Rows.Add(521, 20);
            table.Rows.Add(522, 20);
            table.Rows.Add(523, 20);
            table.Rows.Add(524, 10);
            table.Rows.Add(525, 10);
            table.Rows.Add(526, 30);
            table.Rows.Add(527, 15);
            table.Rows.Add(528, 15);
            table.Rows.Add(529, 10);
            table.Rows.Add(530, 15);
            table.Rows.Add(531, 25);
            table.Rows.Add(532, 10);
            table.Rows.Add(533, 15);
            table.Rows.Add(534, 10);
            table.Rows.Add(535, 10);
            table.Rows.Add(536, 10);
            table.Rows.Add(537, 20);
            table.Rows.Add(538, 10);
            table.Rows.Add(539, 10);
            table.Rows.Add(540, 10);
            table.Rows.Add(541, 10);
            table.Rows.Add(542, 10);
            table.Rows.Add(543, 15);
            table.Rows.Add(544, 15);
            table.Rows.Add(545, 5);
            table.Rows.Add(546, 5);
            table.Rows.Add(547, 10);
            table.Rows.Add(548, 10);
            table.Rows.Add(549, 10);
            table.Rows.Add(550, 5);
            table.Rows.Add(551, 5);
            table.Rows.Add(552, 10);
            table.Rows.Add(553, 5);
            table.Rows.Add(554, 5);
            table.Rows.Add(555, 15);
            table.Rows.Add(556, 10);
            table.Rows.Add(557, 5);
            table.Rows.Add(558, 5);
            table.Rows.Add(559, 5);
            table.Rows.Add(560, 10);
            table.Rows.Add(561, 10);
            table.Rows.Add(562, 10);
            table.Rows.Add(563, 10);
            table.Rows.Add(564, 20);
            table.Rows.Add(565, 25);
            table.Rows.Add(566, 10);
            table.Rows.Add(567, 20);
            table.Rows.Add(568, 30);
            table.Rows.Add(569, 25);
            table.Rows.Add(570, 20);
            table.Rows.Add(571, 20);
            table.Rows.Add(572, 15);
            table.Rows.Add(573, 20);
            table.Rows.Add(574, 15);
            table.Rows.Add(575, 20);
            table.Rows.Add(576, 20);
            table.Rows.Add(577, 10);
            table.Rows.Add(578, 10);
            table.Rows.Add(579, 10);
            table.Rows.Add(580, 10);
            table.Rows.Add(581, 10);
            table.Rows.Add(582, 20);
            table.Rows.Add(583, 10);
            table.Rows.Add(584, 30);
            table.Rows.Add(585, 15);
            table.Rows.Add(586, 10);
            table.Rows.Add(587, 10);
            table.Rows.Add(588, 10);
            table.Rows.Add(589, 20);
            table.Rows.Add(590, 20);
            table.Rows.Add(591, 5);
            table.Rows.Add(592, 5);
            table.Rows.Add(593, 5);
            table.Rows.Add(594, 20);
            table.Rows.Add(595, 10);
            table.Rows.Add(596, 10);
            table.Rows.Add(597, 20);
            table.Rows.Add(598, 15);
            table.Rows.Add(599, 20);
            table.Rows.Add(600, 20);
            table.Rows.Add(601, 10);
            table.Rows.Add(602, 20);
            table.Rows.Add(603, 30);
            table.Rows.Add(604, 10);
            table.Rows.Add(605, 10);
            table.Rows.Add(606, 40);
            table.Rows.Add(607, 40);
            table.Rows.Add(608, 30);
            table.Rows.Add(609, 20);
            table.Rows.Add(610, 40);
            table.Rows.Add(611, 20);
            table.Rows.Add(612, 20);
            table.Rows.Add(613, 10);
            table.Rows.Add(614, 10);
            table.Rows.Add(615, 10);
            table.Rows.Add(616, 10);
            table.Rows.Add(617, 5);

            return table;
        }
        // Functions that Set Stuff up for PKX viewing // 
        private void InitializeFields()
        {
            // Initialize Fields

            {
                #region ability table
                speciesability = new int[] {
                                000,	000,	000,	000,
                                001,	065,	065,	034,
                                002,	065,	065,	034,
                                003,	065,	065,	034,
                                004,	066,	066,	094,
                                005,	066,	066,	094,
                                006,	066,	066,	094,
                                007,	067,	067,	044,
                                008,	067,	067,	044,
                                009,	067,	067,	044,
                                010,	019,	019,	050,
                                011,	061,	061,	061,
                                012,	014,	014,	110,
                                013,	019,	019,	050,
                                014,	061,	061,	061,
                                015,	068,	068,	097,
                                016,	051,	077,	145,
                                017,	051,	077,	145,
                                018,	051,	077,	145,
                                019,	050,	062,	055,
                                020,	050,	062,	055,
                                021,	051,	051,	097,
                                022,	051,	051,	097,
                                023,	022,	061,	127,
                                024,	022,	061,	127,
                                025,	009,	009,	031,
                                026,	009,	009,	031,
                                027,	008,	008,	146,
                                028,	008,	008,	146,
                                029,	038,	079,	055,
                                030,	038,	079,	055,
                                031,	038,	079,	125,
                                032,	038,	079,	055,
                                033,	038,	079,	055,
                                034,	038,	079,	125,
                                035,	056,	098,	132,
                                036,	056,	098,	109,
                                037,	018,	018,	070,
                                038,	018,	018,	070,
                                039,	056,	172,	132,
                                040,	056,	172,	119,
                                041,	039,	039,	151,
                                042,	039,	039,	151,
                                043,	034,	034,	050,
                                044,	034,	034,	001,
                                045,	034,	034,	027,
                                046,	027,	087,	006,
                                047,	027,	087,	006,
                                048,	014,	110,	050,
                                049,	019,	110,	147,
                                050,	008,	071,	159,
                                051,	008,	071,	159,
                                052,	053,	101,	127,
                                053,	007,	101,	127,
                                054,	006,	013,	033,
                                055,	006,	013,	033,
                                056,	072,	083,	128,
                                057,	072,	083,	128,
                                058,	022,	018,	154,
                                059,	022,	018,	154,
                                060,	011,	006,	033,
                                061,	011,	006,	033,
                                062,	011,	006,	033,
                                063,	028,	039,	098,
                                064,	028,	039,	098,
                                065,	028,	039,	098,
                                066,	062,	099,	080,
                                067,	062,	099,	080,
                                068,	062,	099,	080,
                                069,	034,	034,	082,
                                070,	034,	034,	082,
                                071,	034,	034,	082,
                                072,	029,	064,	044,
                                073,	029,	064,	044,
                                074,	069,	005,	008,
                                075,	069,	005,	008,
                                076,	069,	005,	008,
                                077,	050,	018,	049,
                                078,	050,	018,	049,
                                079,	012,	020,	144,
                                080,	012,	020,	144,
                                081,	042,	005,	148,
                                082,	042,	005,	148,
                                083,	051,	039,	128,
                                084,	050,	048,	077,
                                085,	050,	048,	077,
                                086,	047,	093,	115,
                                087,	047,	093,	115,
                                088,	001,	060,	143,
                                089,	001,	060,	143,
                                090,	075,	092,	142,
                                091,	075,	092,	142,
                                092,	026,	026,	026,
                                093,	026,	026,	026,
                                094,	026,	026,	026,
                                095,	069,	005,	133,
                                096,	015,	108,	039,
                                097,	015,	108,	039,
                                098,	052,	075,	125,
                                099,	052,	075,	125,
                                100,	043,	009,	106,
                                101,	043,	009,	106,
                                102,	034,	034,	139,
                                103,	034,	034,	139,
                                104,	069,	031,	004,
                                105,	069,	031,	004,
                                106,	007,	120,	084,
                                107,	051,	089,	039,
                                108,	020,	012,	013,
                                109,	026,	026,	026,
                                110,	026,	026,	026,
                                111,	031,	069,	120,
                                112,	031,	069,	120,
                                113,	030,	032,	131,
                                114,	034,	102,	144,
                                115,	048,	113,	039,
                                116,	033,	097,	006,
                                117,	038,	097,	006,
                                118,	033,	041,	031,
                                119,	033,	041,	031,
                                120,	035,	030,	148,
                                121,	035,	030,	148,
                                122,	043,	111,	101,
                                123,	068,	101,	080,
                                124,	012,	108,	087,
                                125,	009,	009,	072,
                                126,	049,	049,	072,
                                127,	052,	104,	153,
                                128,	022,	083,	125,
                                129,	033,	033,	155,
                                130,	022,	022,	153,
                                131,	011,	075,	093,
                                132,	007,	007,	150,
                                133,	050,	091,	107,
                                134,	011,	011,	093,
                                135,	010,	010,	095,
                                136,	018,	018,	062,
                                137,	036,	088,	148,
                                138,	033,	075,	133,
                                139,	033,	075,	133,
                                140,	033,	004,	133,
                                141,	033,	004,	133,
                                142,	069,	046,	127,
                                143,	017,	047,	082,
                                144,	046,	046,	081,
                                145,	046,	046,	009,
                                146,	046,	046,	049,
                                147,	061,	061,	063,
                                148,	061,	061,	063,
                                149,	039,	039,	136,
                                150,	046,	046,	127,
                                151,	028,	028,	028,
                                152,	065,	065,	102,
                                153,	065,	065,	102,
                                154,	065,	065,	102,
                                155,	066,	066,	018,
                                156,	066,	066,	018,
                                157,	066,	066,	018,
                                158,	067,	067,	125,
                                159,	067,	067,	125,
                                160,	067,	067,	125,
                                161,	050,	051,	119,
                                162,	050,	051,	119,
                                163,	015,	051,	110,
                                164,	015,	051,	110,
                                165,	068,	048,	155,
                                166,	068,	048,	089,
                                167,	068,	015,	097,
                                168,	068,	015,	097,
                                169,	039,	039,	151,
                                170,	010,	035,	011,
                                171,	010,	035,	011,
                                172,	009,	009,	031,
                                173,	056,	098,	132,
                                174,	056,	172,	132,
                                175,	055,	032,	105,
                                176,	055,	032,	105,
                                177,	028,	048,	156,
                                178,	028,	048,	156,
                                179,	009,	009,	057,
                                180,	009,	009,	057,
                                181,	009,	009,	057,
                                182,	034,	034,	131,
                                183,	047,	037,	157,
                                184,	047,	037,	157,
                                185,	005,	069,	155,
                                186,	011,	006,	002,
                                187,	034,	102,	151,
                                188,	034,	102,	151,
                                189,	034,	102,	151,
                                190,	050,	053,	092,
                                191,	034,	094,	048,
                                192,	034,	094,	048,
                                193,	003,	014,	119,
                                194,	006,	011,	109,
                                195,	006,	011,	109,
                                196,	028,	028,	156,
                                197,	028,	028,	039,
                                198,	015,	105,	158,
                                199,	012,	020,	144,
                                200,	026,	026,	026,
                                201,	026,	026,	026,
                                202,	023,	023,	140,
                                203,	039,	048,	157,
                                204,	005,	005,	142,
                                205,	005,	005,	142,
                                206,	032,	050,	155,
                                207,	052,	008,	017,
                                208,	069,	005,	125,
                                209,	022,	050,	155,
                                210,	022,	095,	155,
                                211,	038,	033,	022,
                                212,	068,	101,	135,
                                213,	005,	082,	126,
                                214,	068,	062,	153,
                                215,	039,	051,	124,
                                216,	053,	095,	118,
                                217,	062,	095,	127,
                                218,	040,	049,	133,
                                219,	040,	049,	133,
                                220,	012,	081,	047,
                                221,	012,	081,	047,
                                222,	055,	030,	144,
                                223,	055,	097,	141,
                                224,	021,	097,	141,
                                225,	072,	055,	015,
                                226,	033,	011,	041,
                                227,	051,	005,	133,
                                228,	048,	018,	127,
                                229,	048,	018,	127,
                                230,	033,	097,	006,
                                231,	053,	053,	008,
                                232,	005,	005,	008,
                                233,	036,	088,	148,
                                234,	022,	119,	157,
                                235,	020,	101,	141,
                                236,	062,	080,	072,
                                237,	022,	101,	080,
                                238,	012,	108,	093,
                                239,	009,	009,	072,
                                240,	049,	049,	072,
                                241,	047,	113,	157,
                                242,	030,	032,	131,
                                243,	046,	046,	010,
                                244,	046,	046,	018,
                                245,	046,	046,	011,
                                246,	062,	062,	008,
                                247,	061,	061,	061,
                                248,	045,	045,	127,
                                249,	046,	046,	136,
                                250,	046,	046,	144,
                                251,	030,	030,	030,
                                252,	065,	065,	084,
                                253,	065,	065,	084,
                                254,	065,	065,	084,
                                255,	066,	066,	003,
                                256,	066,	066,	003,
                                257,	066,	066,	003,
                                258,	067,	067,	006,
                                259,	067,	067,	006,
                                260,	067,	067,	006,
                                261,	050,	095,	155,
                                262,	022,	095,	153,
                                263,	053,	082,	095,
                                264,	053,	082,	095,
                                265,	019,	019,	050,
                                266,	061,	061,	061,
                                267,	068,	068,	079,
                                268,	061,	061,	061,
                                269,	019,	019,	014,
                                270,	033,	044,	020,
                                271,	033,	044,	020,
                                272,	033,	044,	020,
                                273,	034,	048,	124,
                                274,	034,	048,	124,
                                275,	034,	048,	124,
                                276,	062,	062,	113,
                                277,	062,	062,	113,
                                278,	051,	051,	044,
                                279,	051,	051,	044,
                                280,	028,	036,	140,
                                281,	028,	036,	140,
                                282,	028,	036,	140,
                                283,	033,	033,	044,
                                284,	022,	022,	127,
                                285,	027,	090,	095,
                                286,	027,	090,	101,
                                287,	054,	054,	054,
                                288,	072,	072,	072,
                                289,	054,	054,	054,
                                290,	014,	014,	050,
                                291,	003,	003,	151,
                                292,	025,	025,	025,
                                293,	043,	043,	155,
                                294,	043,	043,	113,
                                295,	043,	043,	113,
                                296,	047,	062,	125,
                                297,	047,	062,	125,
                                298,	047,	037,	157,
                                299,	005,	042,	159,
                                300,	056,	096,	147,
                                301,	056,	096,	147,
                                302,	051,	100,	158,
                                303,	052,	022,	125,
                                304,	005,	069,	134,
                                305,	005,	069,	134,
                                306,	005,	069,	134,
                                307,	074,	074,	140,
                                308,	074,	074,	140,
                                309,	009,	031,	058,
                                310,	009,	031,	058,
                                311,	057,	057,	031,
                                312,	058,	058,	010,
                                313,	035,	068,	158,
                                314,	012,	110,	158,
                                315,	030,	038,	102,
                                316,	064,	060,	082,
                                317,	064,	060,	082,
                                318,	024,	024,	003,
                                319,	024,	024,	003,
                                320,	041,	012,	046,
                                321,	041,	012,	046,
                                322,	012,	086,	020,
                                323,	040,	116,	083,
                                324,	073,	073,	075,
                                325,	047,	020,	082,
                                326,	047,	020,	082,
                                327,	020,	077,	126,
                                328,	052,	071,	125,
                                329,	026,	026,	026,
                                330,	026,	026,	026,
                                331,	008,	008,	011,
                                332,	008,	008,	011,
                                333,	030,	030,	013,
                                334,	030,	030,	013,
                                335,	017,	017,	137,
                                336,	061,	061,	151,
                                337,	026,	026,	026,
                                338,	026,	026,	026,
                                339,	012,	107,	093,
                                340,	012,	107,	093,
                                341,	052,	075,	091,
                                342,	052,	075,	091,
                                343,	026,	026,	026,
                                344,	026,	026,	026,
                                345,	021,	021,	114,
                                346,	021,	021,	114,
                                347,	004,	004,	033,
                                348,	004,	004,	033,
                                349,	033,	012,	091,
                                350,	063,	172,	056,
                                351,	059,	059,	059,
                                352,	016,	016,	168,
                                353,	015,	119,	130,
                                354,	015,	119,	130,
                                355,	026,	026,	119,
                                356,	046,	046,	119,
                                357,	034,	094,	139,
                                358,	026,	026,	026,
                                359,	046,	105,	154,
                                360,	023,	023,	140,
                                361,	039,	115,	141,
                                362,	039,	115,	141,
                                363,	047,	115,	012,
                                364,	047,	115,	012,
                                365,	047,	115,	012,
                                366,	075,	075,	155,
                                367,	033,	033,	041,
                                368,	033,	033,	093,
                                369,	033,	069,	005,
                                370,	033,	033,	093,
                                371,	069,	069,	125,
                                372,	069,	069,	142,
                                373,	022,	022,	153,
                                374,	029,	029,	135,
                                375,	029,	029,	135,
                                376,	029,	029,	135,
                                377,	029,	029,	005,
                                378,	029,	029,	115,
                                379,	029,	029,	135,
                                380,	026,	026,	026,
                                381,	026,	026,	026,
                                382,	002,	002,	002,
                                383,	070,	070,	070,
                                384,	076,	076,	076,
                                385,	032,	032,	032,
                                386,	046,	046,	046,
                                387,	065,	065,	075,
                                388,	065,	065,	075,
                                389,	065,	065,	075,
                                390,	066,	066,	089,
                                391,	066,	066,	089,
                                392,	066,	066,	089,
                                393,	067,	067,	128,
                                394,	067,	067,	128,
                                395,	067,	067,	128,
                                396,	051,	051,	120,
                                397,	022,	022,	120,
                                398,	022,	022,	120,
                                399,	086,	109,	141,
                                400,	086,	109,	141,
                                401,	061,	061,	050,
                                402,	068,	068,	101,
                                403,	079,	022,	062,
                                404,	079,	022,	062,
                                405,	079,	022,	062,
                                406,	030,	038,	102,
                                407,	030,	038,	101,
                                408,	104,	104,	125,
                                409,	104,	104,	125,
                                410,	005,	005,	043,
                                411,	005,	005,	043,
                                412,	061,	061,	142,
                                413,	107,	107,	142,
                                414,	068,	068,	110,
                                415,	118,	118,	055,
                                416,	046,	046,	127,
                                417,	050,	053,	010,
                                418,	033,	033,	041,
                                419,	033,	033,	041,
                                420,	034,	034,	034,
                                421,	122,	122,	122,
                                422,	060,	114,	159,
                                423,	060,	114,	159,
                                424,	101,	053,	092,
                                425,	106,	084,	138,
                                426,	106,	084,	138,
                                427,	050,	103,	007,
                                428,	056,	103,	007,
                                429,	026,	026,	026,
                                430,	015,	105,	153,
                                431,	007,	020,	051,
                                432,	047,	020,	128,
                                433,	026,	026,	026,
                                434,	001,	106,	051,
                                435,	001,	106,	051,
                                436,	026,	085,	134,
                                437,	026,	085,	134,
                                438,	005,	069,	155,
                                439,	043,	111,	101,
                                440,	030,	032,	132,
                                441,	051,	077,	145,
                                442,	046,	046,	151,
                                443,	008,	008,	024,
                                444,	008,	008,	024,
                                445,	008,	008,	024,
                                446,	053,	047,	082,
                                447,	080,	039,	158,
                                448,	080,	039,	154,
                                449,	045,	045,	159,
                                450,	045,	045,	159,
                                451,	004,	097,	051,
                                452,	004,	097,	051,
                                453,	107,	087,	143,
                                454,	107,	087,	143,
                                455,	026,	026,	026,
                                456,	033,	114,	041,
                                457,	033,	114,	041,
                                458,	033,	011,	041,
                                459,	117,	117,	043,
                                460,	117,	117,	043,
                                461,	046,	046,	124,
                                462,	042,	005,	148,
                                463,	020,	012,	013,
                                464,	031,	116,	120,
                                465,	034,	102,	144,
                                466,	078,	078,	072,
                                467,	049,	049,	072,
                                468,	055,	032,	105,
                                469,	003,	110,	119,
                                470,	102,	102,	034,
                                471,	081,	081,	115,
                                472,	052,	008,	090,
                                473,	012,	081,	047,
                                474,	091,	088,	148,
                                475,	080,	080,	154,
                                476,	005,	042,	159,
                                477,	046,	046,	119,
                                478,	081,	081,	130,
                                479,	026,	026,	026,
                                480,	026,	026,	026,
                                481,	026,	026,	026,
                                482,	026,	026,	026,
                                483,	046,	046,	140,
                                484,	046,	046,	140,
                                485,	018,	018,	049,
                                486,	112,	112,	112,
                                487,	046,	046,	140,
                                488,	026,	026,	026,
                                489,	093,	093,	093,
                                490,	093,	093,	093,
                                491,	123,	123,	123,
                                492,	030,	030,	030,
                                493,	121,	121,	121,
                                494,	162,	162,	162,
                                495,	065,	065,	126,
                                496,	065,	065,	126,
                                497,	065,	065,	126,
                                498,	066,	066,	047,
                                499,	066,	066,	047,
                                500,	066,	066,	120,
                                501,	067,	067,	075,
                                502,	067,	067,	075,
                                503,	067,	067,	075,
                                504,	050,	051,	148,
                                505,	035,	051,	148,
                                506,	072,	053,	050,
                                507,	022,	146,	113,
                                508,	022,	146,	113,
                                509,	007,	084,	158,
                                510,	007,	084,	158,
                                511,	082,	082,	065,
                                512,	082,	082,	065,
                                513,	082,	082,	066,
                                514,	082,	082,	066,
                                515,	082,	082,	067,
                                516,	082,	082,	067,
                                517,	108,	028,	140,
                                518,	108,	028,	140,
                                519,	145,	105,	079,
                                520,	145,	105,	079,
                                521,	145,	105,	079,
                                522,	031,	078,	157,
                                523,	031,	078,	157,
                                524,	005,	005,	159,
                                525,	005,	005,	159,
                                526,	005,	005,	159,
                                527,	109,	103,	086,
                                528,	109,	103,	086,
                                529,	146,	159,	104,
                                530,	146,	159,	104,
                                531,	131,	144,	103,
                                532,	062,	125,	089,
                                533,	062,	125,	089,
                                534,	062,	125,	089,
                                535,	033,	093,	011,
                                536,	033,	093,	011,
                                537,	033,	143,	011,
                                538,	062,	039,	104,
                                539,	005,	039,	104,
                                540,	068,	034,	142,
                                541,	102,	034,	142,
                                542,	068,	034,	142,
                                543,	038,	068,	003,
                                544,	038,	068,	003,
                                545,	038,	068,	003,
                                546,	158,	151,	034,
                                547,	158,	151,	034,
                                548,	034,	020,	102,
                                549,	034,	020,	102,
                                550,	120,	091,	104,
                                551,	022,	153,	083,
                                552,	022,	153,	083,
                                553,	022,	153,	083,
                                554,	055,	055,	039,
                                555,	125,	125,	161,
                                556,	011,	034,	114,
                                557,	005,	075,	133,
                                558,	005,	075,	133,
                                559,	061,	153,	022,
                                560,	061,	153,	022,
                                561,	147,	098,	110,
                                562,	152,	152,	152,
                                563,	152,	152,	152,
                                564,	116,	005,	033,
                                565,	116,	005,	033,
                                566,	129,	129,	129,
                                567,	129,	129,	129,
                                568,	001,	060,	106,
                                569,	001,	133,	106,
                                570,	149,	149,	149,
                                571,	149,	149,	149,
                                572,	056,	101,	092,
                                573,	056,	101,	092,
                                574,	119,	172,	023,
                                575,	119,	172,	023,
                                576,	119,	172,	023,
                                577,	142,	098,	144,
                                578,	142,	098,	144,
                                579,	142,	098,	144,
                                580,	051,	145,	093,
                                581,	051,	145,	093,
                                582,	115,	115,	133,
                                583,	115,	115,	133,
                                584,	115,	115,	133,
                                585,	034,	157,	032,
                                586,	034,	157,	032,
                                587,	009,	009,	078,
                                588,	068,	061,	099,
                                589,	068,	075,	142,
                                590,	027,	027,	144,
                                591,	027,	027,	144,
                                592,	011,	130,	006,
                                593,	011,	130,	006,
                                594,	131,	093,	144,
                                595,	014,	127,	068,
                                596,	014,	127,	068,
                                597,	160,	160,	160,
                                598,	160,	160,	107,
                                599,	057,	058,	029,
                                600,	057,	058,	029,
                                601,	057,	058,	029,
                                602,	026,	026,	026,
                                603,	026,	026,	026,
                                604,	026,	026,	026,
                                605,	140,	028,	148,
                                606,	140,	028,	148,
                                607,	018,	049,	151,
                                608,	018,	049,	151,
                                609,	018,	049,	151,
                                610,	079,	104,	127,
                                611,	079,	104,	127,
                                612,	079,	104,	127,
                                613,	081,	081,	155,
                                614,	081,	081,	033,
                                615,	026,	026,	026,
                                616,	093,	075,	142,
                                617,	093,	060,	084,
                                618,	009,	007,	008,
                                619,	039,	144,	120,
                                620,	039,	144,	120,
                                621,	024,	125,	104,
                                622,	089,	103,	099,
                                623,	089,	103,	099,
                                624,	128,	039,	046,
                                625,	128,	039,	046,
                                626,	120,	157,	043,
                                627,	051,	125,	055,
                                628,	051,	125,	128,
                                629,	145,	142,	133,
                                630,	145,	142,	133,
                                631,	082,	018,	073,
                                632,	068,	055,	054,
                                633,	055,	055,	055,
                                634,	055,	055,	055,
                                635,	026,	026,	026,
                                636,	049,	049,	068,
                                637,	049,	049,	068,
                                638,	154,	154,	154,
                                639,	154,	154,	154,
                                640,	154,	154,	154,
                                641,	158,	158,	128,
                                642,	158,	158,	128,
                                643,	163,	163,	163,
                                644,	164,	164,	164,
                                645,	159,	159,	125,
                                646,	046,	046,	046,
                                647,	154,	154,	154,
                                648,	032,	032,	032,
                                649,	088,	088,	088,
                                650,	065,	065,	171,
                                651,	065,	065,	171,
                                652,	065,	065,	171,
                                653,	066,	066,	170,
                                654,	066,	066,	170,
                                655,	066,	066,	170,
                                656,	067,	067,	168,
                                657,	067,	067,	168,
                                658,	067,	067,	168,
                                659,	053,	167,	037,
                                660,	053,	167,	037,
                                661,	145,	145,	177,
                                662,	049,	049,	177,
                                663,	049,	049,	177,
                                664,	019,	014,	132,
                                665,	061,	061,	132,
                                666,	019,	014,	132,
                                667,	079,	127,	153,
                                668,	079,	127,	153,
                                669,	166,	166,	180,
                                670,	166,	166,	180,
                                671,	166,	166,	180,
                                672,	157,	157,	179,
                                673,	157,	157,	179,
                                674,	089,	104,	113,
                                675,	089,	104,	113,
                                676,	169,	169,	169,
                                677,	051,	151,	020,
                                678,	051,	151,	158,
                                679,	099,	099,	099,
                                680,	099,	099,	099,
                                681,	176,	176,	176,
                                682,	131,	131,	165,
                                683,	131,	131,	165,
                                684,	175,	175,	084,
                                685,	175,	175,	084,
                                686,	126,	021,	151,
                                687,	126,	021,	151,
                                688,	181,	097,	124,
                                689,	181,	097,	124,
                                690,	038,	143,	091,
                                691,	038,	143,	091,
                                692,	178,	178,	178,
                                693,	178,	178,	178,
                                694,	087,	008,	094,
                                695,	087,	008,	094,
                                696,	173,	173,	005,
                                697,	173,	173,	069,
                                698,	174,	174,	117,
                                699,	174,	174,	117,
                                700,	056,	056,	182,
                                701,	007,	084,	104,
                                702,	167,	053,	057,
                                703,	029,	029,	005,
                                704,	157,	093,	183,
                                705,	157,	093,	183,
                                706,	157,	093,	183,
                                707,	158,	158,	170,
                                708,	030,	119,	139,
                                709,	030,	119,	139,
                                710,	053,	119,	015,
                                711,	053,	119,	015,
                                712,	020,	115,	005,
                                713,	020,	115,	005,
                                714,	119,	151,	140,
                                715,	119,	151,	140,
                                716,	187,	187,	187,
                                717,	186,	186,	186,
                                718,	188,	188,	188,
                                719,	029,	029,	029,
                                720,	170,	170,	170,
                                721,	011,	011,	011,
                                722,	046,	046,	046,
                                723,	046,	046,	046,
                                724,	046,	046,	046,
                                725,	107,	107,	142,
                                726,	107,	107,	142,
                                727,	032,	032,	032,
                                728,	026,	026,	026,
                                729,	026,	026,	026,
                                730,	026,	026,	026,
                                731,	026,	026,	026,
                                732,	026,	026,	026,
                                733,	026,	026,	026,
                                734,	059,	059,	059,
                                735,	059,	059,	059,
                                736,	059,	059,	059,
                                737,	122,	122,	122,
                                738,	069,	091,	104,
                                739,	125,	125,	161,
                                740,	032,	032,	032,
                                741,	163,	163,	163,
                                742,	164,	164,	164,
                                743,	154,	154,	154,
                                744,	144,	144,	144,
                                745,	010,	010,	010,
                                746,	022,	022,	022,
                                747,	023,	023,	023,
                                748,	051,	151,	172,
                                749,	169,	169,	169,
                                750,	169,	169,	169,
                                751,	169,	169,	169,
                                752,	169,	169,	169,
                                753,	169,	169,	169,
                                754,	169,	169,	169,
                                755,	169,	169,	169,
                                756,	169,	169,	169,
                                757,	169,	169,	169,
                                758,	182,	182,	182,
                                759,	104,	104,	104,
                                760,	047,	047,	047,
                                761,	181,	181,	181,
                                762,	070,	070,	070,
                                763,	080,	080,	080,
                                764,	015,	015,	015,
                                765,	003,	003,	003,
                                766,	074,	074,	074,
                                767,	094,	094,	094,
                                768,	111,	111,	111,
                                769,	158,	158,	158,
                                770,	045,	045,	045,
                                771,	101,	101,	101,
                                772,	184,	184,	184,
                                773,	181,	181,	181,
                                774,	091,	091,	091,
                                775,	117,	117,	117,
                                776,	176,	176,	176,
                                777,	178,	178,	178,
                                778,	185,	185,	185,
                                779,	104,	104,	104,
                                780,	156,	156,	156,
                                781,	036,	036,	036,
                                782,	092,	092,	092,
                                783,	037,	037,	037,
                                784,	022,	022,	022,
                                785,	159,	159,	159,
                                786,	026,	026,	026,
                                787,	026,	026,	026,
                                788,	053,	119,	015,
                                789,	053,	119,	015,
                                790,	053,	119,	015,
                                791,	053,	119,	015,
                                792,	053,	119,	015,
                                793,	053,	119,	015,
                                794,	166,	166,	180,
                                795,	166,	166,	180,
                                796,	166,	166,	180,
                                797,	166,	166,	180,
                };
                #endregion
            }
            {
                #region var tables
                
                var language_list = new[] {
                    new { Text = "ENG (English)", Value = 2 },
                    new { Text = "JPN (日本語)", Value = 1 },
                    new { Text = "FRE (Français)", Value = 3 },
                    new { Text = "ITA (Italiano)", Value = 4 },
                    new { Text = "GER (Deutsch)", Value = 5 },
                    new { Text = "ESP (Español)", Value = 7 },
                    new { Text = "KOR (한국어)", Value = 8 }
                };
                
                var dsregion_list = new[] {
                    new { Text = "Americas (NA/SA)", Value = 1 },
                    new { Text = "Europe (EU)", Value = 2 },
                    new { Text = "Japan (日本)", Value = 0 },
                    new { Text = "China (中国)", Value = 4 },
                    new { Text = "Korea (한국)", Value = 5 },
                    new { Text = "Taiwan (臺灣)", Value = 6 }
                };
                var subreg_list = new[] {
                    new { Text = "sr_0", Value = 0 },
                    new { Text = "sr_1", Value = 1 },
                    new { Text = "sr_2", Value = 2 },
                    new { Text = "sr_3", Value = 3 },
                    new { Text = "sr_4", Value = 4 },
                    new { Text = "sr_5", Value = 5 },
                    new { Text = "sr_6", Value = 6 },
                    new { Text = "sr_7", Value = 7 },
                    new { Text = "sr_8", Value = 8 },
                    new { Text = "sr_9", Value = 9 },
                    new { Text = "sr_10", Value = 10 },
                    new { Text = "sr_11", Value = 11 },
                    new { Text = "sr_12", Value = 12 },
                    new { Text = "sr_13", Value = 13 },
                    new { Text = "sr_14", Value = 14 },
                    new { Text = "sr_15", Value = 15 },
                    new { Text = "sr_16", Value = 16 },
                    new { Text = "sr_17", Value = 17 },
                    new { Text = "sr_18", Value = 18 },
                    new { Text = "sr_19", Value = 19 },
                    new { Text = "sr_20", Value = 20 },
                    new { Text = "sr_21", Value = 21 },
                    new { Text = "sr_22", Value = 22 },
                    new { Text = "sr_23", Value = 23 },
                    new { Text = "sr_24", Value = 24 },
                    new { Text = "sr_25", Value = 25 },
                    new { Text = "sr_26", Value = 26 },
                    new { Text = "sr_27", Value = 27 },
                    new { Text = "sr_28", Value = 28 },
                    new { Text = "sr_29", Value = 29 },
                    new { Text = "sr_30", Value = 30 },
                    new { Text = "sr_31", Value = 31 },
                    new { Text = "sr_32", Value = 32 },
                    new { Text = "sr_33", Value = 33 },
                    new { Text = "sr_34", Value = 34 },
                    new { Text = "sr_35", Value = 35 },
                    new { Text = "sr_36", Value = 36 },
                    new { Text = "sr_37", Value = 37 },
                    new { Text = "sr_38", Value = 38 },
                    new { Text = "sr_39", Value = 39 },
                    new { Text = "sr_40", Value = 40 },
                    new { Text = "sr_41", Value = 41 },
                    new { Text = "sr_42", Value = 42 },
                    new { Text = "sr_43", Value = 43 },
                    new { Text = "sr_44", Value = 44 },
                    new { Text = "sr_45", Value = 45 },
                    new { Text = "sr_46", Value = 46 },
                    new { Text = "sr_47", Value = 47 },
                    new { Text = "sr_48", Value = 48 },
                    new { Text = "sr_49", Value = 49 },
                    new { Text = "sr_50", Value = 50 },
                    new { Text = "sr_51", Value = 51 },
                    new { Text = "sr_52", Value = 52 },
                    new { Text = "sr_53", Value = 53 },
                    new { Text = "sr_54", Value = 54 },
                    new { Text = "sr_55", Value = 55 },
                    new { Text = "sr_56", Value = 56 },
                    new { Text = "sr_57", Value = 57 },
                    new { Text = "sr_58", Value = 58 },
                    new { Text = "sr_59", Value = 59 },
                    new { Text = "sr_60", Value = 60 },
                    new { Text = "sr_61", Value = 61 },
                    new { Text = "sr_62", Value = 62 },
                    new { Text = "sr_63", Value = 63 },
                    new { Text = "sr_64", Value = 64 },
                    new { Text = "sr_65", Value = 65 },
                    new { Text = "sr_66", Value = 66 },
                    new { Text = "sr_67", Value = 67 },
                    new { Text = "sr_68", Value = 68 },
                    new { Text = "sr_69", Value = 69 },
                    new { Text = "sr_70", Value = 70 },
                    new { Text = "sr_71", Value = 71 },
                    new { Text = "sr_72", Value = 72 },
                    new { Text = "sr_73", Value = 73 },
                    new { Text = "sr_74", Value = 74 },
                    new { Text = "sr_75", Value = 75 },
                    new { Text = "sr_76", Value = 76 },
                    new { Text = "sr_77", Value = 77 },
                    new { Text = "sr_78", Value = 78 },
                    new { Text = "sr_79", Value = 79 },
                    new { Text = "sr_80", Value = 80 },
                };
                #endregion

                // Set ComboBox Fields
                
                CB_3DSReg.DataSource = dsregion_list;
                CB_3DSReg.DisplayMember = "Text";
                CB_3DSReg.ValueMember = "Value";

                CB_Language.DataSource = language_list;
                CB_Language.DisplayMember = "Text";
                CB_Language.ValueMember = "Value";

                setcountry(CB_Country);

                CB_SubRegion.DataSource = subreg_list;
                CB_SubRegion.DisplayMember = "Text";
                CB_SubRegion.ValueMember = "Value";
                
                InitializeLanguage();
            }

            // Finish setting up the ComboBoxes
            CB_Ball.SelectedIndex = 0;
            CB_GameOrigin.SelectedIndex = 0;

            // Now that the ComboBoxes are ready, load the data.
            populatefields(buff);
            {
                TB_OT.Text = "PKHeX";
                TB_TID.Text = 12345.ToString();
                TB_SID.Text = 54321.ToString();
            }
            updateAbilityList();
        }
        private void InitializeLanguage()
        {
            #region Balls
            {
                // Allowed Balls
                int[] ball_nums = { 7, 576, 13, 492, 497, 14, 495, 493, 496, 494, 11, 498, 8, 6, 12, 15, 9, 5, 499, 10, 1, 16 };
                int[] ball_vals = { 7, 25, 13, 17, 22, 14, 20, 18, 21, 19, 11, 23, 8, 6, 12, 15, 9, 5, 24, 10, 1, 16 };

                // Set up
                List<cbItem> ball_list = new List<cbItem>();

                for (int i = 4; i > 1; i--) // add 4,3,2
                {
                    // First 3 Balls are always first
                    cbItem ncbi = new cbItem();
                    ncbi.Text = itemlist[i];
                    ncbi.Value = i;
                    ball_list.Add(ncbi);
                }

                // Sort the Rest based on String Name
                string[] ballnames = new string[ball_nums.Length];
                for (int i = 0; i < ball_nums.Length; i++)
                {
                    ballnames[i] = itemlist[ball_nums[i]];
                }
                string[] sortedballs = new string[ball_nums.Length];
                Array.Copy(ballnames, sortedballs, ballnames.Length);
                Array.Sort(sortedballs);

                // Add the rest of the balls
                for (int i = 0; i < sortedballs.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedballs[i];
                    ncbi.Value = ball_vals[Array.IndexOf(ballnames, sortedballs[i])];
                    ball_list.Add(ncbi);
                }
                CB_Ball.DisplayMember = "Text";
                CB_Ball.ValueMember = "Value";
                CB_Ball.DataSource = ball_list;
            }
            #endregion
            #region Held Items
            {
                // List of valid items to hold
                int[] item_nums = { 
                                            000,001,002,003,004,005,006,007,008,009,010,011,012,013,014,015,017,018,019,020,021,022,023,024,025,026,027,028,029,030,031,032,033,034,035,
                                            036,037,038,039,040,041,042,043,044,045,046,047,048,049,050,051,052,053,054,055,056,057,058,059,060,061,062,063,064,065,066,067,068,069,070,
                                            071,072,073,074,075,076,077,078,079,080,081,082,083,084,085,086,087,088,089,090,091,092,093,094,099,100,101,102,103,104,105,106,107,108,109,
                                            110,112,116,117,118,119,134,135,136,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,
                                            175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,
                                            210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,
                                            245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,
                                            280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,
                                            315,316,317,318,319,320,321,322,323,324,325,326,327,504,537,538,539,540,541,542,543,544,545,546,547,548,549,550,551,552,553,554,555,556,557,
                                            558,559,560,561,562,563,564,565,566,567,568,569,570,571,572,573,577,580,581,582,583,584,585,586,587,588,589,590,591,639,640,644,645,646,647,
                                            648,649,650,652,653,654,655,656,657,658,659,660,661,662,663,664,665,666,667,668,669,670,671,672,673,674,675,676,677,678,679,680,681,682,683,
                                            684,685,686,687,688,699,704,708,709,710,711,715,
                                      };

                List<cbItem> item_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] itemnames = new string[item_nums.Length];
                for (int i = 0; i < item_nums.Length; i++)
                {
                    itemnames[i] = itemlist[item_nums[i]];
                }
                string[] sorteditems = new string[item_nums.Length];
                Array.Copy(itemnames, sorteditems, itemnames.Length);
                Array.Sort(sorteditems);

                // Add the rest of the items
                for (int i = 0; i < sorteditems.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sorteditems[i];
                    ncbi.Value = item_nums[Array.IndexOf(itemnames, sorteditems[i])];
                    item_list.Add(ncbi);
                }
                CB_HeldItem.DisplayMember = "Text";
                CB_HeldItem.ValueMember = "Value";
                CB_HeldItem.DataSource = item_list;
            }
            #endregion
            #region Species
            {
                List<cbItem> species_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedspecies = new string[specieslist.Length];
                Array.Copy(specieslist, sortedspecies, specieslist.Length);
                Array.Sort(sortedspecies);

                // Add the rest of the items
                for (int i = 0; i < sortedspecies.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedspecies[i];
                    ncbi.Value = Array.IndexOf(specieslist, sortedspecies[i]);
                    species_list.Add(ncbi);
                }
                CB_Species.DisplayMember = "Text";
                CB_Species.ValueMember = "Value";
                CB_Species.DataSource = species_list;
            }
            #endregion
            #region Natures
            {
                List<cbItem> natures_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortednatures = new string[natures.Length];
                Array.Copy(natures, sortednatures, natures.Length);
                Array.Sort(sortednatures);

                // Add the rest of the items
                for (int i = 0; i < sortednatures.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortednatures[i];
                    ncbi.Value = Array.IndexOf(natures, sortednatures[i]);
                    natures_list.Add(ncbi);
                }
                CB_Nature.DisplayMember = "Text";
                CB_Nature.ValueMember = "Value";
                CB_Nature.DataSource = natures_list;
            }
            #endregion
            #region Moves
            {
                List<cbItem> move_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedmoves = new string[movelist.Length];
                Array.Copy(movelist, sortedmoves, movelist.Length);
                Array.Sort(sortedmoves);

                // Add the rest of the items
                for (int i = 0; i < sortedmoves.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedmoves[i];
                    ncbi.Value = Array.IndexOf(movelist, sortedmoves[i]);
                    move_list.Add(ncbi);
                }

                CB_Move1.DisplayMember = CB_Move2.DisplayMember = CB_Move3.DisplayMember = CB_Move4.DisplayMember = "Text";
                CB_RelearnMove1.DisplayMember = CB_RelearnMove2.DisplayMember = CB_RelearnMove3.DisplayMember = CB_RelearnMove4.DisplayMember = "Text";
                CB_Move1.ValueMember = CB_Move2.ValueMember = CB_Move3.ValueMember = CB_Move4.ValueMember = "Value";
                CB_RelearnMove1.ValueMember = CB_RelearnMove2.ValueMember = CB_RelearnMove3.ValueMember = CB_RelearnMove4.ValueMember = "Value";

                var move1_list = new BindingSource(move_list, null);
                CB_Move1.DataSource = move1_list;

                var move2_list = new BindingSource(move_list, null);
                CB_Move2.DataSource = move2_list;

                var move3_list = new BindingSource(move_list, null);
                CB_Move3.DataSource = move3_list;

                var move4_list = new BindingSource(move_list, null);
                CB_Move4.DataSource = move4_list;

                var eggmove1_list = new BindingSource(move_list, null);
                CB_RelearnMove1.DataSource = eggmove1_list;

                var eggmove2_list = new BindingSource(move_list, null);
                CB_RelearnMove2.DataSource = eggmove2_list;

                var eggmove3_list = new BindingSource(move_list, null);
                CB_RelearnMove3.DataSource = eggmove3_list;

                var eggmove4_list = new BindingSource(move_list, null);
                CB_RelearnMove4.DataSource = eggmove4_list;
            }
            #endregion
            #region Encounter Types

            var EncounterType = new[] {
                    new { Text = "None", Value = 0 },
                    new { Text = "Tall Grass", Value = 2 },
                    new { Text = "Dialga/Palkia", Value = 4 },
                    new { Text = "Cave/Hall of Origin", Value = 5 },
                    new { Text = "Surfing/Fishing", Value = 7 },
                    new { Text = "Building", Value = 9 },
                    new { Text = "Marsh/Safari", Value = 10 },
                    new { Text = "Starter/Fossil/Gift", Value = 24 }
                };
            CB_EncounterType.DataSource = EncounterType;
            CB_EncounterType.DisplayMember = "Text";
            CB_EncounterType.ValueMember = "Value";
            #endregion
            #region Games
            List<cbItem> origin_list = new List<cbItem>();
            // lazy text table... 8 columns
            string[] langlistorigin = new string[] {
                    "24",	"X",	    "X",	            "X",	    "X",	        "X",	    "X",	            "X",
                    "25",	"Y",	    "Y",	            "Y",	    "Y",	        "Y",	    "Y",	            "Y",
                    "20",	"White",	"ホワイト",	        "Blanche",	"Bianca",	    "Weiße",	"Blanca",	        "화이트",
                    "21",	"Black",	"ブラック",	        "Noire",	"Nera",	        "Schwarze",	"Negra",	        "블랙",
                    "22",	"White2",	"ホワイト2",	        "Blanche2",	"Bianca2",	    "Weiße2",	"Blanca2",	        "화이트2",
                    "23",	"Black2",	"ブラック2",	        "Noire2",	"Nera2",	    "Schwarze2","Negra2",	        "블랙2",
                    "10",	"Diamond",	"ダイヤモンド",	    "Diamant",	"Diamante", 	"Diamant",	"Diamante",	        "디아루가",
                    "11",	"Pearl",	"パール",	        "Perle",	"Perla",	    "Perl",	    "Perla",	        "펄기아",
                    "12",	"Platinum",	"プラチナ",	        "Platine",	"Platino",	    "Platin",	"Platino",	        "Pt 기라티나",
                    "7",	"HeartGold","ハートゴールド",	"HeartGold","HeartGold",	"HeartGold","HeartGold",	    "하트골드",
                    "8",	"SoulSilver","ソウルシルバー ",	"SoulSilver","SoulSilver",	"SoulSilver","SoulSilver",	    "소울실버",
                    "2",	"Ruby",	    "ルビー",	        "Rubis",    "Rubino",	    "Rubin",	"Rubí",	            "루비",
                    "1",	"Sapphire",	"サファイア",	    "Saphir",	"Zaffiro",	    "Saphir",	"Zafiro",	        "사파이어",
                    "3",	"Emerald",	"エメラルド",	    "Émeraude",	"Smeraldo",	    "Smaragd",	"Esmeralda",	    "에메랄드",
                    "4",	"FireRed",	"ファイアレッド",	"Rouge-Feu","Rosso Fuoco",	"Feuerrote ","Rojo Fuego",	    "파이어레드",
                    "5",	"LeafGreen",	"リーフグリーン","Vert-Feuille","Verde Foglia","Blattgrüne ","Verde Hoja",	"리프그린",
                    "15",	"Colosseum/XD",	"コロシアム/XD",	"Colisée/XD","Colosseo/XD",	"Kolosseum/XD",	"Colosseum/XD",	"세움/XD",
                };
            // populate the list
            for (int i = 0; i < langlistorigin.Length / 8; i++)
            {
                cbItem item = new cbItem();
                item.Text = langlistorigin[i * 8 + CB_MainLanguage.SelectedIndex + 1];
                item.Value = Convert.ToInt32(langlistorigin[i * 8]);
                origin_list.Add(item);
            }

            CB_GameOrigin.DataSource = origin_list;
            CB_GameOrigin.DisplayMember = "Text";
            CB_GameOrigin.ValueMember = "Value";

            #endregion
        }
        public void TranslateInterface(string FORM_NAME)
        {
            // Fetch a File
            // Check to see if a the translation file exists in the same folder as the executable
            string externalLangPath = System.Windows.Forms.Application.StartupPath + "\\lang_" + curlanguage + ".txt";
            string[] rawlist;
            if (File.Exists(externalLangPath))
            {
                rawlist = File.ReadAllLines(externalLangPath);
            }
            else
            {
                object txt;
                txt = Properties.Resources.ResourceManager.GetObject("lang_" + curlanguage); // Fetch File, \n to list.
                if (txt == null) return; // Translation file does not exist as a resource; abort this function and don't translate UI.
                string[] stringSeparators = new string[] { "\r\n" };
                rawlist = ((string)txt).Split(stringSeparators, StringSplitOptions.None);
            }

            string[] stringdata = new string[rawlist.Length];
            int itemsToRename = 0;
            for (int i = 0; i < rawlist.Length; i++)
            {
                // Find our starting point
                if (rawlist[i] == "! "+FORM_NAME) // Start our data
                {
                    // Copy our Control Names and Text to a new array for later processing.
                    for (int j = i + 1; j < rawlist.Length; j++)
                    {
                        if (rawlist[j].Length == 0)
                            continue; // Skip Over Empty Lines, errhandled
                        if (rawlist[j][0].ToString() != "-") // If line is not a comment line...
                        {
                            if (rawlist[j][0].ToString() == "!") // Stop if we have reached the end of translation
                                break; // exit inner loop
                            stringdata[itemsToRename] = rawlist[j]; // Add the entry to process later.
                            itemsToRename++;
                        }
                    }
                    break; // exit outer loop
                }
            }

            // Now that we have our items to rename in: Control = Text format, let's execute the changes!
            
            for (int i = 0; i < itemsToRename; i++)
            {
                string[] SplitString = Regex.Split(stringdata[i]," = ");
                if (SplitString.Length < 2)
                    continue; // Error in Input, errhandled
                string ctrl = SplitString[0]; // Control to change the text of...
                string text = SplitString[1]; // Text to set Control.Text to...
                Control[] controllist = Controls.Find(ctrl, true);
                if (controllist.Length == 0) // If Control isn't found...
                {
                    // Menu Items can't be found with Controls.Find as they aren't Controls
                    ToolStripDropDownItem TSI = (ToolStripDropDownItem)menuStrip1.Items[ctrl];
                    if (TSI != null)
                    {
                        // We'll rename the main and child in a row.
                        string[] ToolItems = Regex.Split(SplitString[1], " ; ");
                        TSI.Text = ToolItems[0]; // Set parent's text first
                        if (TSI.DropDownItems.Count != ToolItems.Length - 1)
                            continue; // Error in Input, errhandled
                        for (int ti = 1; ti <= TSI.DropDownItems.Count; ti++)
                        {
                            TSI.DropDownItems[ti-1].Text = ToolItems[ti]; // Set child text
                        }
                    }
                    // If not found, it is not something to rename and is thus skipped.
                }
                else // Set the input control's text.
                {
                    controllist[0].Text = text;
                }
            }
        }
        private void populatefields(byte[] buff)
        {
            CAL_EggDate.Value = new DateTime(2000, 01, 01);
            Tab_Main.Focus();
            // Encryption Constant
            TB_EC.Text = BitConverter.ToUInt32(buff, 0).ToString("X8");

            // Block A
            int species = BitConverter.ToInt16(buff, 0x08);
            int helditem = BitConverter.ToUInt16(buff, 0x0A);
            uint TID = BitConverter.ToUInt16(buff, 0x0C);
            uint SID = BitConverter.ToUInt16(buff, 0x0E);
            uint exp = BitConverter.ToUInt32(buff, 0x10);
            int ability = buff[0x14];
            int abilitynum = buff[0x15];
            // 0x16, 0x17 - unknown
            uint PID = BitConverter.ToUInt32(buff, 0x18);
            int nature = buff[0x1C];
            int feflag = buff[0x1D] % 2;
            this.genderflag = (buff[0x1D] >> 1) & 0x3;
            int altforms = (buff[0x1D] >> 3);
            int HP_EV = buff[0x1E];
            int ATK_EV = buff[0x1F];
            int DEF_EV = buff[0x20];
            int SPA_EV = buff[0x22];
            int SPD_EV = buff[0x23];
            int SPE_EV = buff[0x21];
            int cnt_cool = buff[0x24];
            int cnt_beauty = buff[0x25];
            int cnt_cute = buff[0x26];
            int cnt_smart = buff[0x27];
            int cnt_tough = buff[0x28];
            int cnt_sheen = buff[0x29];
            int markings = buff[0x2A];
            int PKRS_Strain = buff[0x2B] >> 4;
            int PKRS_Duration = buff[0x2B] % 0x10;

            // Medals and Ribbons, passed with buff to new form
            // 0x2C, 0x2D, 0x2E, 0x2F
            // 0x33, 0x34, 0x35, 0x36
            // 0x34, 0x35, 0x36, 0x37
            // 0x38, 0x39

            // 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F - unused/unknown

            // Block B
            string nicknamestr = TrimFromZero(Encoding.Unicode.GetString(buff, 0x40, 24));
            // 0x58, 0x59 - unused
            int move1 = BitConverter.ToInt16(buff, 0x5A);
            int move2 = BitConverter.ToInt16(buff, 0x5C);
            int move3 = BitConverter.ToInt16(buff, 0x5E);
            int move4 = BitConverter.ToInt16(buff, 0x60);
            int move1_pp = buff[0x62];
            int move2_pp = buff[0x63];
            int move3_pp = buff[0x64];
            int move4_pp = buff[0x65];
            int move1_ppu = buff[0x66];
            int move2_ppu = buff[0x67];
            int move3_ppu = buff[0x68];
            int move4_ppu = buff[0x69];
            int eggmove1 = BitConverter.ToInt16(buff, 0x6A);
            int eggmove2 = BitConverter.ToInt16(buff, 0x6C);
            int eggmove3 = BitConverter.ToInt16(buff, 0x6E);
            int eggmove4 = BitConverter.ToInt16(buff, 0x70);

            // 0x72 - Super Training Flag - Passed with buff to new form

            // 0x73 - unused/unknown
            uint IV32 = BitConverter.ToUInt32(buff, 0x74);
            uint HP_IV = IV32 & 0x1F;
            uint ATK_IV = (IV32 >> 5) & 0x1F;
            uint DEF_IV = (IV32 >> 10) & 0x1F;
            uint SPE_IV = (IV32 >> 15) & 0x1F;
            uint SPA_IV = (IV32 >> 20) & 0x1F;
            uint SPD_IV = (IV32 >> 25) & 0x1F;
            uint isegg = (IV32 >> 30) & 1;
            uint isnick = (IV32 >> 31);

            // Block C
            string notOT = TrimFromZero(Encoding.Unicode.GetString(buff, 0x78, 24));
            bool notOTG = Convert.ToBoolean(buff[0x92]);
            // Memory Editor edits everything else with buff in a new form

            // Block D
            string ot = TrimFromZero(Encoding.Unicode.GetString(buff, 0xB0, 24));
            // 0xC8, 0xC9 - unused
            int OTfriendship = buff[0xCA];
            int OTaffection = buff[0xCB]; // Handled by Memory Editor
            // 0xCC, 0xCD, 0xCE, 0xCF, 0xD0
            int egg_year = buff[0xD1];
            int egg_month = buff[0xD2];
            int egg_day = buff[0xD3];
            int met_year = buff[0xD4];
            int met_month = buff[0xD5];
            int met_day = buff[0xD6];
            // 0xD7 - unused
            int eggloc = BitConverter.ToUInt16(buff, 0xD8);
            int metloc = BitConverter.ToUInt16(buff, 0xDA);
            int ball = buff[0xDC];
            int metlevel = buff[0xDD] & 0x7F;
            int otgender = (buff[0xDD]) >> 7;
            int encountertype = buff[0xDE];
            int gamevers = buff[0xDF];
            int countryID = buff[0xE0];
            int regionID = buff[0xE1];
            int dsregID = buff[0xE2];
            int otlang = buff[0xE3];
            // 0xE4, 0xE5, 0xE6, 0xE7 - unused

            //
            // Populate Fields
            //

            CHK_Fateful.Checked = Convert.ToBoolean(feflag);
            CHK_IsEgg.Checked = Convert.ToBoolean(isegg);
            CHK_Nicknamed.Checked = Convert.ToBoolean(isnick);
            if (Convert.ToBoolean(otgender))
            {
                Label_OTGender.Text = "♀";
            }
            else Label_OTGender.Text = "♂";
            
            // Nidoran Gender Fixing Text
            if (!Convert.ToBoolean(isnick))
            {
                if (nicknamestr.Contains((char)0xE08F))
                {
                    nicknamestr = Regex.Replace(nicknamestr, "\uE08F", "\u2640");
                }
                else if (nicknamestr.Contains((char)0xE08E))
                {
                    nicknamestr = Regex.Replace(nicknamestr, "\uE08E", "\u2642");
                }
            }
            populatemarkings(markings);
            TB_PID.Text = PID.ToString("X8");
            CB_Species.SelectedValue = species;
            CB_HeldItem.SelectedValue = helditem;
            updateAbilityList();
            TB_AbilityNumber.Text = abilitynum.ToString();
            if (abilitynum>>1 < 3) CB_Ability.SelectedIndex = abilitynum>>1; // error handling
            else CB_Ability.SelectedIndex = 0;
            CB_Nature.SelectedValue = nature;

            TB_EXP.Text = exp.ToString();
            TB_TID.Text = TID.ToString();
            TB_SID.Text = SID.ToString();

            TB_OT.Text = ot;
            TB_Nickname.Text = nicknamestr;
            TB_OTt2.Text = notOT;

            if (buff[0x93] == 1)  // = 1
            {
                TB_Friendship.Text = buff[0xA2].ToString();
                GB_nOT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                GB_OT.BackColor = Color.Transparent;

            }
            else                // = 0
            {
                TB_Friendship.Text = OTfriendship.ToString();
                GB_OT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                GB_nOT.BackColor = Color.Transparent;
            }


            CB_Language.SelectedValue = otlang;
            CB_Country.SelectedValue = countryID;
            CB_SubRegion.SelectedValue = regionID;
            CB_3DSReg.SelectedValue = dsregID;
            CB_GameOrigin.SelectedValue = gamevers;
            CB_EncounterType.SelectedValue = encountertype;
            CB_Ball.SelectedValue = ball;


            if (met_month == 0)
            { met_month = 1; }
            if (met_day == 0)
            { met_day = 1; }

            CAL_MetDate.Value = new DateTime(met_year + 2000, met_month, met_day);

            if (eggloc != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = eggloc;
                CAL_EggDate.Value = new DateTime(egg_year + 2000, egg_month, egg_day);
            }
            else { CHK_AsEgg.Checked = false; CB_EggLocation.SelectedValue = 0; GB_EggConditions.Enabled = false; }

            CB_MetLocation.SelectedValue = metloc;

            if (notOTG) Label_CTGender.Text = "♀";
            else Label_CTGender.Text = "♂";
            if (TB_OTt2.Text == "") Label_CTGender.Text = "";

            if (CHK_IsEgg.Checked)
            {
                CB_MetLocation.Enabled = false;
                CAL_MetDate.Enabled = false;
            }
            else
            {
                CB_MetLocation.Enabled = true;
                CAL_MetDate.Enabled = true;
            }

            // reset
            CHK_Cured.Checked = false;
            CHK_Infected.Checked = false;

            TB_MetLevel.Text = metlevel.ToString();



            CB_PKRSStrain.SelectedIndex = PKRS_Strain;
            CB_PKRSDays.SelectedIndex = (PKRS_Duration & 0x7) % 5; // to strip out bad hacked 'rus
            if (PKRS_Strain > 0)
            {
                CHK_Infected.Checked = true;
                if (PKRS_Duration == 0)
                {
                    CHK_Cured.Checked = true;
                }
            }

            CB_PKRSStrain.SelectedIndex = PKRS_Strain;
            CB_PKRSDays.SelectedIndex = (PKRS_Duration & 0x7) % 5; // to strip out bad hacked 'rus

            TB_Cool.Text = cnt_cool.ToString();
            TB_Beauty.Text = cnt_beauty.ToString();
            TB_Cute.Text = cnt_cute.ToString();
            TB_Smart.Text = cnt_smart.ToString();
            TB_Tough.Text = cnt_tough.ToString();
            TB_Sheen.Text = cnt_sheen.ToString();

            TB_HPIV.Text = HP_IV.ToString();
            TB_ATKIV.Text = ATK_IV.ToString();
            TB_DEFIV.Text = DEF_IV.ToString();
            TB_SPAIV.Text = SPA_IV.ToString();
            TB_SPDIV.Text = SPD_IV.ToString();
            TB_SPEIV.Text = SPE_IV.ToString();

            TB_HPEV.Text = HP_EV.ToString();
            TB_ATKEV.Text = ATK_EV.ToString();
            TB_DEFEV.Text = DEF_EV.ToString();
            TB_SPAEV.Text = SPA_EV.ToString();
            TB_SPDEV.Text = SPD_EV.ToString();
            TB_SPEEV.Text = SPE_EV.ToString();

            CB_Move1.SelectedValue = move1;
            CB_Move2.SelectedValue = move2;
            CB_Move3.SelectedValue = move3;
            CB_Move4.SelectedValue = move4;
            CB_RelearnMove1.SelectedValue = eggmove1;
            CB_RelearnMove2.SelectedValue = eggmove2;
            CB_RelearnMove3.SelectedValue = eggmove3;
            CB_RelearnMove4.SelectedValue = eggmove4;
            CB_PPu1.SelectedIndex = move1_ppu;
            CB_PPu2.SelectedIndex = move2_ppu;
            CB_PPu3.SelectedIndex = move3_ppu;
            CB_PPu4.SelectedIndex = move4_ppu;

            int level;
            if (ToInt32(TB_EXP.Text) == 0) { level = 1; }
            else level = getLevel();
            TB_Level.Text = level.ToString();

            // Setup Forms
            getForms(species);
            CB_Form.SelectedIndex = altforms;

            // Load Extrabyte Value
            TB_ExtraByte.Text = buff[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
            if ((TB_OTt2.Text == "") || (notOT == ""))
            {
                Label_CTGender.Text = "";
            }
            // Reload Gender Flag
            this.genderflag = ((buff[0x1D] >> 1) & 0x3);
            getGenderLabel();
            updateStats();
            getIsShiny();
            if (init)
            {
                uint chk = 0;
                for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                {
                    chk += (uint)(buff[i] + buff[i + 1] * 0x100);
                }
                chk &= 0xFFFF;

                ushort actualsum = BitConverter.ToUInt16(buff, 0x6);
                if (chk != actualsum)
                {
                    MessageBox.Show("PKX File has an invalid checksum.", "Alert");
                }
            }
            init = true;
        }
        private void populatemarkings(int markings)
        {
            int m1 = ((markings) & 1);
            int m2 = ((markings >> 1) & 1);
            int m3 = ((markings >> 2) & 1);
            int m4 = ((markings >> 3) & 1);
            int m5 = ((markings >> 4) & 1);
            int m6 = ((markings >> 5) & 1);

            CHK_Circle.Checked = Convert.ToBoolean(m1);
            CHK_Triangle.Checked = Convert.ToBoolean(m2);
            CHK_Square.Checked = Convert.ToBoolean(m3);
            CHK_Heart.Checked = Convert.ToBoolean(m4);
            CHK_Star.Checked = Convert.ToBoolean(m5);
            CHK_Diamond.Checked = Convert.ToBoolean(m6);
        }
        // PKX Data Calculation Functions // 
        private static int ToInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
                return 0;
            return Int32.Parse(value);
        }
        private static uint ToUInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
                return 0;
            return UInt32.Parse(value);
        }
        private uint getHEXval(TextBox tb)
        {
            if (tb.Text == null)
                return 0;
            string str = RemoveTroublesomeCharacters(tb);
            return UInt32.Parse(str, NumberStyles.HexNumber);
        }
        public int getIndex(ComboBox cb)
        {
            int val = 0;
            try { val = ToInt32(cb.SelectedValue.ToString()); }
            catch { };
            return val;
        }
        private void getIsShiny()
        {
            uint PID = getHEXval(TB_PID);
            uint UID = (PID >> 16);
            uint LID = (PID & 0xFFFF);
            uint PSV = UID ^ LID;
            uint TSV = ToUInt32(TB_TID.Text) ^ ToUInt32(TB_SID.Text);
            uint XOR = TSV ^ PSV;
            int game = getIndex(CB_GameOrigin);
            if (((XOR < 8) && (game < 24)) || ((XOR < 16) && (game >= 24)))
            {   // Is Shiny
                BTN_Shinytize.Visible =
                BTN_Shinytize.Enabled = false;
                Label_IsShiny.Visible = true;
            }
            else
            {   // Is Not Shiny
                BTN_Shinytize.Visible =
                BTN_Shinytize.Enabled = true;
                Label_IsShiny.Visible = false;
            }

            getMarkings();
        }
        private void getForms(int species)
        {
            // Form Tables
            // 
            var form_unown = new[] {
                    new { Text = "A", Value = 0 },
                    new { Text = "B", Value = 1 },
                    new { Text = "C", Value = 2 },
                    new { Text = "D", Value = 3 },
                    new { Text = "E", Value = 4 },
                    new { Text = "F", Value = 5 },
                    new { Text = "G", Value = 6 },
                    new { Text = "H", Value = 7 },
                    new { Text = "I", Value = 8 },
                    new { Text = "J", Value = 9 },
                    new { Text = "K", Value = 10 },
                    new { Text = "L", Value = 11 },
                    new { Text = "M", Value = 12 },
                    new { Text = "N", Value = 13 },
                    new { Text = "O", Value = 14 },
                    new { Text = "P", Value = 15 },
                    new { Text = "Q", Value = 16 },
                    new { Text = "R", Value = 17 },
                    new { Text = "S", Value = 18 },
                    new { Text = "T", Value = 19 },
                    new { Text = "U", Value = 20 },
                    new { Text = "V", Value = 21 },
                    new { Text = "W", Value = 22 },
                    new { Text = "X", Value = 23 },
                    new { Text = "Y", Value = 24 },
                    new { Text = "Z", Value = 25 },
                    new { Text = "!", Value = 26 },
                    new { Text = "?", Value = 27 },
                };
            var form_castform = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = forms[773], Value = 1 }, // Sunny
                    new { Text = forms[774], Value = 2 }, // Rainy
                    new { Text = forms[775], Value = 3 }, // Snowy
                };
            var form_shellos = new[] {
                    new { Text = forms[422], Value = 0 }, // West
                    new { Text = forms[788], Value = 1 }, // East
                };
            var form_deoxys = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = forms[780], Value = 1 }, // Attack
                    new { Text = forms[781], Value = 2 }, // Defense
                    new { Text = forms[782], Value = 3 }, // Speed
                };
            var form_burmy = new[] {
                    new { Text = forms[412], Value = 0 }, // Plant
                    new { Text = forms[783], Value = 1 }, // Sandy
                    new { Text = forms[784], Value = 2 }, // Trash
                };
            var form_cherrim = new[] {
                    new { Text = forms[421], Value = 0 }, // Overcast
                    new { Text = forms[787], Value = 1 }, // Sunshine
                };
            var form_rotom = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = forms[793], Value = 1 }, // Heat
                    new { Text = forms[794], Value = 2 }, // Wash
                    new { Text = forms[795], Value = 3 }, // Frost
                    new { Text = forms[796], Value = 4 }, // Fan
                    new { Text = forms[797], Value = 5 }, // Mow
                };
            var form_giratina = new[] {
                    new { Text = forms[487], Value = 0 }, // Altered
                    new { Text = forms[798], Value = 1 }, // Origin
                };
            var form_shaymin = new[] {
                    new { Text = forms[492], Value = 0 }, // Land
                    new { Text = forms[799], Value = 1 }, // Sky
                };
            var form_arceus = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = types[1], Value = 1 }, // Fighting
                    new { Text = types[2], Value = 2 }, // Flying
                    new { Text = types[3], Value = 3 }, // Poison
                    new { Text = types[4], Value = 4 }, // etc
                    new { Text = types[5], Value = 5 },
                    new { Text = types[6], Value = 6 },
                    new { Text = types[7], Value = 7 },
                    new { Text = types[8], Value = 8 },
                    new { Text = types[9], Value = 9 },
                    new { Text = types[10], Value = 10 },
                    new { Text = types[11], Value = 11 },
                    new { Text = types[12], Value = 12 },
                    new { Text = types[13], Value = 13 },
                    new { Text = types[14], Value = 14 },
                    new { Text = types[15], Value = 15 },
                    new { Text = types[16], Value = 16 },
                    new { Text = types[17], Value = 17 },
                };
            var form_basculin = new[] {
                    new { Text = forms[550], Value = 0 }, // Red
                    new { Text = forms[817], Value = 1 }, // Blue
                };
            var form_darmanitan = new[] {
                    new { Text = forms[555], Value = 0 }, // Standard
                    new { Text = forms[818], Value = 1 }, // Zen
                };
            var form_deerling = new[] {
                    new { Text = forms[585], Value = 0 }, // Spring
                    new { Text = forms[819], Value = 1 }, // Summer
                    new { Text = forms[820], Value = 2 }, // Autumn
                    new { Text = forms[821], Value = 3 }, // Winter
                };
            var form_gender = new[] {
                    new { Text = "♂", Value = 0 }, // Male
                    new { Text = "♀", Value = 1 }, // Female
                };
            var form_therian = new[] {
                    new { Text = forms[641], Value = 0 }, // Incarnate
                    new { Text = forms[825], Value = 1 }, // Therian
                };
            var form_kyurem = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = forms[828], Value = 1 }, // White
                    new { Text = forms[829], Value = 2 }, // Black
                };
            var form_keldeo = new[] {
                    new { Text = forms[647], Value = 0 }, // Ordinary
                    new { Text = forms[830], Value = 1 }, // Resolute
                };
            var form_meloetta = new[] {
                    new { Text = forms[648], Value = 0 }, // Aria
                    new { Text = forms[831], Value = 1 }, // Pirouette
                };
            var form_genesect = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = itemlist[116], Value = 1 }, // Douse
                    new { Text = itemlist[117], Value = 2 }, // Shock
                    new { Text = itemlist[118], Value = 3 }, // Burn
                    new { Text = itemlist[119], Value = 4 }, // Chill
                };
            var form_flabebe = new[] {
                    new { Text = forms[669], Value = 0 }, // Red
                    new { Text = forms[855], Value = 1 }, // Yellow
                    new { Text = forms[856], Value = 2 }, // Orange
                    new { Text = forms[867], Value = 3 }, // Blue
                    new { Text = forms[868], Value = 4 }, // White
                };
            var form_floette = new[] {
                    new { Text = forms[669], Value = 0 }, // Red
                    new { Text = forms[855], Value = 1 }, // Yellow
                    new { Text = forms[856], Value = 2 }, // Orange
                    new { Text = forms[867], Value = 3 }, // Blue
                    new { Text = forms[868], Value = 4 }, // White
                    new { Text = forms[863], Value = 5 }, // Eternal
                };
            var form_furfrou = new[] {
                    new { Text = forms[676], Value = 0 }, // Natural
                    new { Text = forms[868], Value = 1 }, // Heart
                    new { Text = forms[869], Value = 2 }, // Star
                    new { Text = forms[870], Value = 3 }, // Diamond
                    new { Text = forms[871], Value = 4 }, // Deputante
                    new { Text = forms[872], Value = 5 }, // Matron
                    new { Text = forms[873], Value = 6 }, // Dandy
                    new { Text = forms[874], Value = 7 }, // La Reine
                    new { Text = forms[875], Value = 8 }, // Kabuki 
                    new { Text = forms[876], Value = 9 }, // Pharaoh
                }; 
            var form_aegislash = new[] {
                    new { Text = forms[681], Value = 0 }, // Shield
                    new { Text = forms[878], Value = 1 }, // Blade
                };
            var form_butterfly = new[] {
                    new { Text = forms[666], Value = 0 }, // Icy Snow
                    new { Text = forms[836], Value = 1 }, // Polar
                    new { Text = forms[837], Value = 2 }, // Tundra
                    new { Text = forms[838], Value = 3 }, // Continental 
                    new { Text = forms[839], Value = 4 }, // Garden
                    new { Text = forms[840], Value = 5 }, // Elegant
                    new { Text = forms[841], Value = 6 }, // Meadow
                    new { Text = forms[842], Value = 7 }, // Modern 
                    new { Text = forms[843], Value = 8 }, // Marine
                    new { Text = forms[844], Value = 9 }, // Archipelago
                    new { Text = forms[845], Value = 10 }, // High-Plains
                    new { Text = forms[846], Value = 11 }, // Sandstorm
                    new { Text = forms[847], Value = 12 }, // River
                    new { Text = forms[848], Value = 13 }, // Monsoon
                    new { Text = forms[849], Value = 14 }, // Savannah 
                    new { Text = forms[850], Value = 15 }, // Sun
                    new { Text = forms[851], Value = 16 }, // Ocean
                    new { Text = forms[852], Value = 17 }, // Jungle
                    new { Text = forms[853], Value = 18 }, // Fancy
                    new { Text = forms[854], Value = 19 }, // Poké Ball
                };
            var form_list = new[] {
                    new { Text = "", Value = 0}, // None
                };
            var form_pump = new[] {
                    new { Text = forms[879], Value = 0 }, // Small
                    new { Text = forms[710], Value = 1 }, // Average
                    new { Text = forms[880], Value = 2 }, // Large
                    new { Text = forms[881], Value = 3 }, // Super
                };
            var form_mega = new[] {
                    new { Text = types[0], Value = 0}, // Normal
                    new { Text = forms[723], Value = 1}, // Mega
                };
            var form_megaxy = new[] {
                    new { Text = types[0], Value = 0}, // Normal
                    new { Text = forms[723] + " X", Value = 1}, // Mega X
                    new { Text = forms[723] + " Y", Value = 2}, // Mega Y
                };

            CB_Form.DataSource = form_list;
            CB_Form.DisplayMember = "Text";
            CB_Form.ValueMember = "Value";

            // Mega List
            int[] mspec = { 3, 9, 65, 94, 115, 127, 130, 142, 154, 181, 212, 214, 229, 248, 257, 282, 303, 306, 308, 310, 354, 359, 380, 381, 445, 448, 460, 469 };
            for (int i = 0; i < mspec.Length; i++)
            {
                if (mspec[i] == species)
                {
                    CB_Form.DataSource = form_mega;
                    CB_Form.Enabled = true; // Mega Form Selection
                    return;
                }
            }

            // MegaXY List
            if ((species == 6) || (species == 150))
            {
                CB_Form.DataSource = form_megaxy;
                CB_Form.Enabled = true; // Mega Form Selection
                return;
            }

            // Regular Form List
            if (species == 201) { form_list = form_unown; }
            else if (species == 351) { form_list = form_castform; }
            else if (species == 386) { form_list = form_deoxys; }
            else if (species == 421) { form_list = form_cherrim; }
            else if (species == 479) { form_list = form_rotom; }
            else if (species == 487) { form_list = form_giratina; }
            else if (species == 492) { form_list = form_shaymin; }
            else if (species == 493) { form_list = form_arceus; }
            else if (species == 550) { form_list = form_basculin; }
            else if (species == 555) { form_list = form_darmanitan; }
            else if (species == 646) { form_list = form_kyurem; }
            else if (species == 647) { form_list = form_keldeo; }
            else if (species == 648) { form_list = form_meloetta; }
            else if (species == 649) { form_list = form_genesect; }
            else if (species == 676) { form_list = form_furfrou; }
            else if (species == 681) { form_list = form_aegislash; }
            else if (species == 670) { form_list = form_floette; }

            else if ((species == 669) || (species == 671)) { form_list = form_flabebe; }
            else if ((species == 412) || (species == 413)) { form_list = form_burmy; }
            else if ((species == 422) || (species == 423)) { form_list = form_shellos; }
            else if ((species == 585) || (species == 586)) { form_list = form_deerling; }
            else if ((species == 710) || (species == 711)) { form_list = form_pump; }

            else if ((species == 666) || (species == 665) || (species == 664)) { form_list = form_butterfly; }
            else if ((species == 592) || (species == 593) || (species == 678)) { form_list = form_gender; }
            else if ((species == 641) || (species == 642) || (species == 645)) { form_list = form_therian; }
            else
            {
                CB_Form.Enabled = false;
                return;
            };

            CB_Form.DataSource = form_list;
            CB_Form.Enabled = true;
        }
        private void getGenderLabel()
        {
            if (genderflag == 0)
            {
                // Gender = Male
                Label_Gender.Text = "♂";
            }
            else if (genderflag == 1)
            {
                // Gender = Female
                Label_Gender.Text = "♀";
            }
            else { Label_Gender.Text = "-"; }
        }
        private void getMarkings()
        {
            PictureBox[] pba = { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            CheckBox[] cba = { CHK_Circle, CHK_Triangle, CHK_Square, CHK_Heart, CHK_Star, CHK_Diamond };
            for (int i = 0; i < 6; i++)
            {
                pba[i].Image = ImageTransparency.ChangeOpacity(pba[i].InitialImage, (float)(Convert.ToUInt16(cba[i].Checked)) * 0.9 + 0.1);
            }

            PB_MarkShiny.Image = ImageTransparency.ChangeOpacity(PB_MarkShiny.InitialImage, (float)(Convert.ToUInt16(!BTN_Shinytize.Enabled)) * 0.9 + 0.1);
            PB_MarkCured.Image = ImageTransparency.ChangeOpacity(PB_MarkCured.InitialImage, (float)(Convert.ToUInt16(CHK_Cured.Checked)) * 0.9 + 0.1);
            PB_MarkPentagon.Image = ImageTransparency.ChangeOpacity(PB_MarkPentagon.InitialImage, (float)(Convert.ToUInt16(getIndex(CB_GameOrigin) == 24 || getIndex(CB_GameOrigin) == 25)) * 0.9 + 0.1);
        }
        private int getMovePP(int move)
        {
            int pp = 0;
            DataTable movepptable = MovePPTable();
            if (move == -1) { move = 0; }
            pp = (int)movepptable.Rows[move][1];
            return pp;
        }
        private int getSpecies()
        {
            int species = 0;
            try { species = ToInt32(CB_Species.SelectedValue.ToString()); }
            catch { };
            return species;
        }
        private int getNature()
        {
            int nature = 0;
            try { nature = ToInt32(CB_Nature.SelectedValue.ToString()); }
            catch { };
            return nature;
        }
        private int getAbility()
        {
            string abiltext = "";

            if (CB_Ability.Text.Length > 4) // Remove the ability number from the string
                abiltext = (CB_Ability.Text).Remove((CB_Ability.Text).Length - 4);
            return Array.IndexOf(abilitylist, abiltext);
        }
        private int getLevel()
        {
            DataTable spectable = SpeciesTable();
            int exp = ToInt32(TB_EXP.Text);
            int species = getSpecies();
            int growth = (int)spectable.Rows[species][1];
            int tl = 1; // Initial Level
            if (exp == 0) { return tl; }
            DataTable table = ExpTable();
            if ((int)table.Rows[tl][growth + 1] < exp)
            {
                while ((int)table.Rows[tl][growth + 1] < exp)
                {
                    // While EXP for guessed level is below our current exp
                    tl += 1;
                    if (tl == 100)
                    {
                        getEXP(100, species);
                        return tl;
                    }
                    // when calcexp exceeds our exp, we exit loop
                }
                if ((int)table.Rows[tl][growth + 1] == exp)
                {
                    // Matches level threshold
                    return tl;
                }
                else return (tl - 1);
            }
            else return tl;
        }
        private int getLevel(int species, int exp)
        {
            DataTable spectable = SpeciesTable();
            int growth = (int)spectable.Rows[species][1];
            int tl = 1; // Initial Level
            if (exp == 0) { return tl; }
            DataTable table = ExpTable();
            if ((int)table.Rows[tl][growth + 1] < exp)
            {
                while ((int)table.Rows[tl][growth + 1] < exp)
                {
                    // While EXP for guessed level is below our current exp
                    tl += 1;
                    if (tl == 100)
                    {
                        getEXP(100, species);
                        return tl;
                    }
                    // when calcexp exceeds our exp, we exit loop
                }
                if ((int)table.Rows[tl][growth + 1] == exp)
                {
                    // Matches level threshold
                    return tl;
                }
                else return (tl - 1);
            }
            else return tl;
        }
        private int getBaseFriendship(int species)
        {
            int fshp = (int)Friendship().Rows[species][1];
            return fshp;
        }
        private int getEXP(int level, int species)
        {
            // Fetch Growth
            DataTable spectable = SpeciesTable();
            int growth = (int)spectable.Rows[species][1];
            int exp;
            if ((level == 0) || (level == 1))
            {
                exp = 0;
                TB_EXP.Text = exp.ToString();
                return exp;
            }
            switch (growth)
            {
                case 0: // Erratic
                    if (level <= 50)
                    {
                        exp = (level * level * level) * (100 - level) / 50;
                    }
                    else if (level < 69)
                    {
                        exp = (level * level * level) * (150 - level) / 100;
                    }
                    else if (level < 99)
                    {
                        exp = (level * level * level) * ((1911 - 10 * level) / 3) / 500;
                    }
                    else
                    {
                        exp = (level * level * level) * (160 - level) / 100;
                    }
                    TB_EXP.Text = exp.ToString();
                    return exp;
                case 1: // Fast
                    exp = 4 * (level * level * level) / 5;
                    TB_EXP.Text = exp.ToString();
                    return exp;
                case 2: // Medium Fast
                    exp = (level * level * level);
                    TB_EXP.Text = exp.ToString();
                    return exp;
                case 3: // Medium Slow
                    exp = 6 * (level * level * level) / 5 - 15 * (level * level) + 100 * level - 140;
                    TB_EXP.Text = exp.ToString();
                    return exp;
                case 4:
                    exp = 5 * (level * level * level) / 4;
                    TB_EXP.Text = exp.ToString();
                    return exp;
                case 5:
                    if (level <= 15)
                    {
                        exp = (level * level * level) * ((((level + 1) / 3) + 24) / 50);
                    }
                    else if (level <= 36)
                    {
                        exp = (level * level * level) * ((level + 14) / 50);
                    }
                    else
                    {
                        exp = (level * level * level) * (((level / 2) + 32) / 50);
                    }
                    TB_EXP.Text = exp.ToString();
                    return exp;
            }
            return 0;
        }
        // Label Shortcut Tweaks
        private void Label_Friendship_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // prompt to reset
            {
                if (buff[0x93] == 0)
                    TB_Friendship.Text = buff[0xCA].ToString();
                else TB_Friendship.Text = buff[0xA2].ToString();

                return;
            }
            else if (TB_Friendship.Text == "255") // if it's maxed, set it to base
                TB_Friendship.Text = ((int)Friendship().Rows[getIndex(CB_Species)][1]).ToString();
            else // not reset, not maxed, so max
                TB_Friendship.Text = "255";
        }
        private void Label_Gender_Click(object sender, EventArgs e)
        {
            // Get Gender Threshold
            species = getSpecies();
            DataTable spectable = SpeciesTable();
            gt = (int)spectable.Rows[species][8];

            if (gt > 255) // Single gender/genderless
                return;

            if (gt < 256) // If not a single gender(less) species:
            {
                if (Label_Gender.Text == "♂")
                    Label_Gender.Text = "♀";
                else
                    Label_Gender.Text = "♂";
            }
        }
        private void Label_PPups_Click(object sender, EventArgs e)
        {
            int index = 3;
            if (ModifierKeys == Keys.Control)
            {
                index = 0;
            }
            CB_PPu1.SelectedIndex = CB_PPu2.SelectedIndex = CB_PPu3.SelectedIndex = CB_PPu4.SelectedIndex = index;

        }
        private void Label_OTGender_Click(object sender, EventArgs e)
        {
            if (Label_OTGender.Text == "♂")
                Label_OTGender.Text = "♀";
            else Label_OTGender.Text = "♂";
        }
        private void Label_CTGender_Click(object sender, EventArgs e)
        {
            if (Label_CTGender.Text == "") return;
            else if (Label_CTGender.Text == "♂")
                Label_CTGender.Text = "♀";
            else Label_CTGender.Text = "♂";
        }
        private void Marking_Click(object sender, EventArgs e)
        {
            PictureBox[] pba = { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            CheckBox[] cba = { CHK_Circle, CHK_Triangle, CHK_Square, CHK_Heart, CHK_Star, CHK_Diamond };

            CheckBox cb = cba[Array.IndexOf(pba, sender as PictureBox)];
            cb.Checked = !cb.Checked;
            getMarkings();
        }
        // Prompted Updates of PKX Functions // 
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
        public void setcountry(object sender)
        {
            #region country table
            var country_list = new[] {
                            new { Text = "---", Value = 0 },
                            new { Text = "Albania", Value = 64 },
                            new { Text = "Andorra", Value = 122 },
                            new { Text = "Anguilla", Value = 8 },
                            new { Text = "Antigua and Barbuda", Value = 9 },
                            new { Text = "Argentina", Value = 10 },
                            new { Text = "Aruba", Value = 11 },
                            new { Text = "Australia", Value = 65 },
                            new { Text = "Austria", Value = 66 },
                            new { Text = "Azerbaijan", Value = 113 },
                            new { Text = "Bahamas", Value = 12 },
                            new { Text = "Barbados", Value = 13 },
                            new { Text = "Belgium", Value = 67 },
                            new { Text = "Belize", Value = 14 },
                            new { Text = "Bermuda", Value = 186 },
                            new { Text = "Bolivia", Value = 15 },
                            new { Text = "Bosnia and Herzegovina", Value = 68 },
                            new { Text = "Botswana", Value = 69 },
                            new { Text = "Brazil", Value = 16 },
                            new { Text = "British Virgin Islands", Value = 17 },
                            new { Text = "Bulgaria", Value = 70 },
                            new { Text = "Canada", Value = 18 },
                            new { Text = "Cayman Islands", Value = 19 },
                            new { Text = "Chad", Value = 117 },
                            new { Text = "Chile", Value = 20 },
                            new { Text = "China", Value = 160 },
                            new { Text = "Colombia", Value = 21 },
                            new { Text = "Costa Rica", Value = 22 },
                            new { Text = "Croatia", Value = 71 },
                            new { Text = "Cyprus", Value = 72 },
                            new { Text = "Czech Republic", Value = 73 },
                            new { Text = "Denmark (Kingdom of)", Value = 74 },
                            new { Text = "Djibouti", Value = 120 },
                            new { Text = "Dominica", Value = 23 },
                            new { Text = "Dominican Republic", Value = 24 },
                            new { Text = "Ecuador", Value = 25 },
                            new { Text = "El Salvador", Value = 26 },
                            new { Text = "Eritrea", Value = 119 },
                            new { Text = "Estonia", Value = 75 },
                            new { Text = "Finland", Value = 76 },
                            new { Text = "France", Value = 77 },
                            new { Text = "French Guiana", Value = 27 },
                            new { Text = "Germany", Value = 78 },
                            new { Text = "Gibraltar", Value = 123 },
                            new { Text = "Greece", Value = 79 },
                            new { Text = "Grenada", Value = 28 },
                            new { Text = "Guadeloupe", Value = 29 },
                            new { Text = "Guatemala", Value = 30 },
                            new { Text = "Guernsey", Value = 124 },
                            new { Text = "Guyana", Value = 31 },
                            new { Text = "Haiti", Value = 32 },
                            new { Text = "Honduras", Value = 33 },
                            new { Text = "Hong Kong", Value = 144 },
                            new { Text = "Hungary", Value = 80 },
                            new { Text = "Iceland", Value = 81 },
                            new { Text = "India", Value = 169 },
                            new { Text = "Ireland", Value = 82 },
                            new { Text = "Isle of Man", Value = 125 },
                            new { Text = "Italy", Value = 83 },
                            new { Text = "Jamaica", Value = 34 },
                            new { Text = "Japan", Value = 1 },
                            new { Text = "Jersey", Value = 126 },
                            new { Text = "Latvia", Value = 84 },
                            new { Text = "Lesotho", Value = 85 },
                            new { Text = "Liechtenstein", Value = 86 },
                            new { Text = "Lithuania", Value = 87 },
                            new { Text = "Luxembourg", Value = 88 },
                            new { Text = "Macedonia (Republic of)", Value = 89 },
                            new { Text = "Malaysia", Value = 156 },
                            new { Text = "Mali", Value = 115 },
                            new { Text = "Malta", Value = 90 },
                            new { Text = "Martinique", Value = 35 },
                            new { Text = "Mauritania", Value = 114 },
                            new { Text = "Mexico", Value = 36 },
                            new { Text = "Monaco", Value = 127 },
                            new { Text = "Montenegro", Value = 91 },
                            new { Text = "Montserrat", Value = 37 },
                            new { Text = "Mozambique", Value = 92 },
                            new { Text = "Namibia", Value = 93 },
                            new { Text = "Netherlands", Value = 94 },
                            new { Text = "Netherlands Antilles", Value = 38 },
                            new { Text = "New Zealand", Value = 95 },
                            new { Text = "Nicaragua", Value = 39 },
                            new { Text = "Niger", Value = 116 },
                            new { Text = "Norway", Value = 96 },
                            new { Text = "Panama", Value = 40 },
                            new { Text = "Paraguay", Value = 41 },
                            new { Text = "Peru", Value = 42 },
                            new { Text = "Poland", Value = 97 },
                            new { Text = "Portugal", Value = 98 },
                            new { Text = "Romania", Value = 99 },
                            new { Text = "Russia", Value = 100 },
                            new { Text = "San Marino", Value = 184 },
                            new { Text = "Saudi Arabia", Value = 174 },
                            new { Text = "Serbia and Kosovo", Value = 101 },
                            new { Text = "Singapore", Value = 153 },
                            new { Text = "Slovakia", Value = 102 },
                            new { Text = "Slovenia", Value = 103 },
                            new { Text = "Somalia", Value = 121 },
                            new { Text = "South Africa", Value = 104 },
                            new { Text = "South Korea", Value = 136 },
                            new { Text = "Spain", Value = 105 },
                            new { Text = "St. Kitts and Nevis", Value = 43 },
                            new { Text = "St. Lucia", Value = 44 },
                            new { Text = "St. Vincent and the Grenadines", Value = 45 },
                            new { Text = "Sudan", Value = 118 },
                            new { Text = "Suriname", Value = 46 },
                            new { Text = "Swaziland", Value = 106 },
                            new { Text = "Sweden", Value = 107 },
                            new { Text = "Switzerland", Value = 108 },
                            new { Text = "Taiwan", Value = 128 },
                            new { Text = "Trinidad and Tobago", Value = 47 },
                            new { Text = "Turkey", Value = 109 },
                            new { Text = "Turks and Caicos Islands", Value = 48 },
                            new { Text = "U.A.E.", Value = 168 },
                            new { Text = "United Kingdom", Value = 110 },
                            new { Text = "United States", Value = 49 },
                            new { Text = "Uruguay", Value = 50 },
                            new { Text = "US Virgin Islands", Value = 51 },
                            new { Text = "Vatican City", Value = 185 },
                            new { Text = "Venezuela", Value = 52 },
                            new { Text = "Zambia", Value = 111 },
                            new { Text = "Zimbabwe", Value = 112 },
            };
            #endregion
            ComboBox CB = sender as ComboBox;
            CB.DataSource = country_list;
            CB.DisplayMember = "Text";
            CB.ValueMember = "Value";
        }
        private void updateEXPLevel(object sender, EventArgs e)
        {
            if ((TB_EXP.Focused == true) && (TB_EXP.Enabled == true))
            {
                // Change the Level
                TB_Level.Enabled = false;
                int level;
                if (ToInt32(TB_EXP.Text) == 0) { level = 1; }
                else level = getLevel();
                TB_Level.Text = level.ToString();

                TB_Level.Enabled = true;
            }
            else if ((TB_Level.Focused == true) && (TB_Level.Enabled == true))// TB_Level is focused
            {
                // Change the XP
                TB_EXP.Enabled = false;
                int level = ToInt32(TB_Level.Text);
                if ((level <= 100)) // Check for if the user actually changed the level
                {
                    // Valid Level, recalculate EXP
                    getEXP(level, getSpecies());
                    TB_Level.BackColor = Color.White;
                }
                else TB_Level.BackColor = Color.Red;
                TB_EXP.Enabled = true;
            }
            updateStats();
        }
        private void updateContest(object sender, EventArgs e)
        {
            int cnt_cool, cnt_beauty, cnt_cute, cnt_smart, cnt_tough, cnt_sheen;
            cnt_cool = ToInt32(TB_Cool.Text);
            cnt_beauty = ToInt32(TB_Beauty.Text);
            cnt_cute = ToInt32(TB_Cute.Text);
            cnt_smart = ToInt32(TB_Smart.Text);
            cnt_tough = ToInt32(TB_Tough.Text);
            cnt_sheen = ToInt32(TB_Sheen.Text);

            if (cnt_cool > 255)
            {
                // Background turns Red
                TB_Cool.BackColor = Color.Red;
                return;
            }
            else TB_Cool.BackColor = Color.White;

            if (cnt_beauty > 255)
            {
                // Background turns Red
                TB_Beauty.BackColor = Color.Red;
                return;
            }
            else TB_Beauty.BackColor = Color.White;

            if (cnt_cute > 255)
            {
                // Background turns Red
                TB_Cute.BackColor = Color.Red;
                return;
            }
            else TB_Cute.BackColor = Color.White;

            if (cnt_smart > 255)
            {
                // Background turns Red
                TB_Smart.BackColor = Color.Red;
                return;
            }
            else TB_Smart.BackColor = Color.White;

            if (cnt_tough > 255)
            {
                // Background turns Red
                TB_Tough.BackColor = Color.Red;
                return;
            }
            else TB_Tough.BackColor = Color.White;

            if (cnt_sheen > 255)
            {
                // Background turns Red
                TB_Sheen.BackColor = Color.Red;
                return;
            }
            else TB_Sheen.BackColor = Color.White;
        }
        private void UpdateIVs(object sender, EventArgs e)
        {
            int ivtotal, HP_IV, ATK_IV, DEF_IV, SPA_IV, SPD_IV, SPE_IV;
            HP_IV = ToInt32(TB_HPIV.Text);
            ATK_IV = ToInt32(TB_ATKIV.Text);
            DEF_IV = ToInt32(TB_DEFIV.Text);
            SPA_IV = ToInt32(TB_SPAIV.Text);
            SPD_IV = ToInt32(TB_SPDIV.Text);
            SPE_IV = ToInt32(TB_SPEIV.Text);

            int[] iva = new int[] { HP_IV, ATK_IV, DEF_IV, SPE_IV, SPA_IV, SPD_IV };
            MaskedTextBox[] ivt = { TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPEIV, TB_SPAIV, TB_SPDIV };

            bool exitfunction = false;
            for (int i = 0; i < 6; i++)
            {
                if (iva[i] > 31)
                {
                    ivt[i].BackColor = Color.Red;
                    exitfunction = true;
                }
                //else if (iva[i] == 31)
                //    ivt[i].BackColor = Color.Honeydew;
                //else if (iva[i] == 0)
                //    ivt[i].BackColor = Color.LavenderBlush;
                else ivt[i].BackColor = Color.White;
            }
            if (exitfunction) return;

            int HPTYPE = (15 * ((HP_IV & 1) + 2 * (ATK_IV & 1) + 4 * (DEF_IV & 1) + 8 * (SPE_IV & 1) + 16 * (SPA_IV & 1) + 32 * (SPD_IV & 1))) / 63;
            Label_HPTYPE.Text = types[HPTYPE+1]; // type array has normal at index 0, so we offset by 1

            ivtotal = HP_IV + ATK_IV + DEF_IV + SPA_IV + SPD_IV + SPE_IV;
            TB_IVTotal.Text = ivtotal.ToString();

            // Potential Reading
            if (ivtotal <= 90)
                L_Potential.Text = "★☆☆☆";
            else if (ivtotal <= 120)
                L_Potential.Text = "★★☆☆";
            else if (ivtotal <= 150)
                L_Potential.Text = "★★★☆";
            else
                L_Potential.Text = "★★★★";


            // Characteristic with PID%6
            int pm6 = (int)(getHEXval(TB_PID) % 6); // PID MOD 6
            int maxIV = iva.Max();
            int pm6stat = 0;

            for (int i = 0; i < 6; i++)
            {
                pm6stat = (pm6 + i) % 6;
                if (iva[pm6stat] == maxIV)
                    break;  // P%6 is this stat
            }

            L_Characteristic.Text = characteristics[pm6stat * 5 + maxIV % 5];
            updateStats();
        }
        private void UpdateEVs(object sender, EventArgs e)
        {
            int evtotal, HP_EV, ATK_EV, DEF_EV, SPA_EV, SPD_EV, SPE_EV;
            HP_EV = ToInt32(TB_HPEV.Text);
            ATK_EV = ToInt32(TB_ATKEV.Text);
            DEF_EV = ToInt32(TB_DEFEV.Text);
            SPA_EV = ToInt32(TB_SPAEV.Text);
            SPD_EV = ToInt32(TB_SPDEV.Text);
            SPE_EV = ToInt32(TB_SPEEV.Text);

            int[] iva = new int[] { HP_EV, ATK_EV, DEF_EV, SPA_EV, SPD_EV, SPE_EV };
            MaskedTextBox[] ivt = { TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPAEV, TB_SPDEV, TB_SPEEV };

            for (int i = 0; i < 6; i++) // if illegal, mark the box as illegal
            {
                if (iva[i] > 252)
                {
                    ivt[i].BackColor = Color.Red;
                }
                else ivt[i].BackColor = Color.White;
            }
            for (int i = 0; i < 6; i++) // if totally illegal, don't continue.
            {
                if (iva[i] > 255)
                {
                    return;
                }
            }

            evtotal = HP_EV + ATK_EV + DEF_EV + SPA_EV + SPD_EV + SPE_EV;

            if (evtotal > 510) // Background turns Red
                 TB_EVTotal.BackColor = Color.Red;
            else if (evtotal == 510) // Maximum EVs
                 TB_EVTotal.BackColor = Color.Honeydew;
            else TB_EVTotal.BackColor = Color.WhiteSmoke;

            TB_EVTotal.Text = evtotal.ToString();
            updateStats();
        }
        private void updateRandomIVs(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                // Max IVs
                TB_HPIV.Text = 31.ToString();
                TB_ATKIV.Text = 31.ToString();
                TB_DEFIV.Text = 31.ToString();
                TB_SPAIV.Text = 31.ToString();
                TB_SPDIV.Text = 31.ToString();
                TB_SPEIV.Text = 31.ToString();
                return;
            }
            TB_HPIV.Text = (rnd32() & 0x1F).ToString();
            TB_ATKIV.Text = (rnd32() & 0x1F).ToString();
            TB_DEFIV.Text = (rnd32() & 0x1F).ToString();
            TB_SPAIV.Text = (rnd32() & 0x1F).ToString();
            TB_SPDIV.Text = (rnd32() & 0x1F).ToString();
            TB_SPEIV.Text = (rnd32() & 0x1F).ToString();
        }
        private void updateRandomEVs(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                // Max IVs
                TB_HPEV.Text = 0.ToString();
                TB_ATKEV.Text = 0.ToString();
                TB_DEFEV.Text = 0.ToString();
                TB_SPAEV.Text = 0.ToString();
                TB_SPDEV.Text = 0.ToString();
                TB_SPEEV.Text = 0.ToString();
                return;
            }

            MaskedTextBox[] tbEV = { TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPAEV, TB_SPDEV, TB_SPEEV };
            MaskedTextBox[] slEV = shuffle((MaskedTextBox[])tbEV.Clone());  // Shuffle List

            uint e1 = Math.Min(rnd32() % 300, 252); // bias two to get maybe 252
            uint e2 = Math.Min(rnd32() % 300, 252);

            uint e3 = Math.Min(((rnd32()) % (510 - e1 - e2)), 252);
            uint e4 = Math.Min(((rnd32()) % (510 - e1 - e2 - e3)), 252);
            uint e5 = Math.Min(((rnd32()) % (510 - e1 - e2 - e3 - e4)), 252);
            uint e6 = Math.Min((510 - e1 - e2 - e3 - e4 - e5), 252);

            if (e1 + e2 + e3 + e4 + e5 + e6 < 510)
            {
                updateRandomEVs(sender, e);
                return;
            }

            slEV[0].Text = e1.ToString();
            slEV[1].Text = e2.ToString();
            slEV[2].Text = e3.ToString();
            slEV[3].Text = e4.ToString();
            slEV[4].Text = e5.ToString();
            slEV[5].Text = e6.ToString();
        }
        private void updateRandomPID(object sender, EventArgs e)
        {
            species = getSpecies();
            DataTable spectable = SpeciesTable();
            gt = (int)spectable.Rows[species][8];
            TB_PID.Text = getRandomPID(gt, getChosenGender()).ToString("X8");
        }
        private void updateRandomEC(object sender, EventArgs e)
        {
            TB_EC.Text = rnd32().ToString("X8");
        }
        private void updateForm(object sender, EventArgs e)
        {
            updateStats();
            // Repopulate Abilities if Species Form has different abilities
            updateAbilityList();

            // If form has a single gender, account for it.
            if (CB_Form.Text == "♂")
                Label_Gender.Text = "♂";
            else if (CB_Form.Text == "♀")
                Label_Gender.Text = "♀";
        }
        private void updatePP(object sender, EventArgs e)
        {
            // When adding a PP Up, PP = Base*((5+N)/5) as int.
            TB_PP1.Text = (getMovePP(getIndex(CB_Move1)) * (5 + CB_PPu1.SelectedIndex) / 5).ToString();
            TB_PP2.Text = (getMovePP(getIndex(CB_Move2)) * (5 + CB_PPu2.SelectedIndex) / 5).ToString();
            TB_PP3.Text = (getMovePP(getIndex(CB_Move3)) * (5 + CB_PPu3.SelectedIndex) / 5).ToString();
            TB_PP4.Text = (getMovePP(getIndex(CB_Move4)) * (5 + CB_PPu4.SelectedIndex) / 5).ToString();
        }
        private void updatePKRSstrain(object sender, EventArgs e)
        {
            if (CB_PKRSStrain.SelectedIndex == 0)
            {
                // Never Infected
                CB_PKRSDays.SelectedValue = 0;
                CHK_Cured.Checked = false;
                CHK_Infected.Checked = false;
            }
        }
        private void updatePKRSdays(object sender, EventArgs e)
        {
            if (CB_PKRSDays.SelectedIndex == 0)
            {
                // If no days are selected
                if (CB_PKRSStrain.SelectedIndex == 0)
                {
                    // Never Infected
                    CHK_Cured.Checked = false;
                    CHK_Infected.Checked = false;
                }
                else CHK_Cured.Checked = true;
            }
        }
        private void updateSpecies(object sender, EventArgs e)
        {
            // Change Species Prompted
            int species = getSpecies();
            int level = ToInt32(TB_Level.Text);

            // Get Forms for Given Species
            getForms(species);

            // Recalculate EXP for Given Level
            getEXP(level, species);

            // Check for Gender Changes
            // Get Gender Threshold
            species = getSpecies();
            DataTable spectable = SpeciesTable();
            gt = (int)spectable.Rows[species][8];

            if (gt == 258)  // Genderless
                genderflag = 2;
            else if (gt == 257) // Female Only
                genderflag = 1;
            else if (gt == 256) // Male Only
                genderflag = 0;
            else
            {   // get gender from old PID correlation
                if ((getHEXval(TB_PID) & 0xFF) < gt)
                     genderflag = 1;
                else genderflag = 0;
            }

            getGenderLabel();
            updateAbilityList();
            updateForm(null, null);

            // If species changes and no nickname, set the new name == speciesName.
            if (!CHK_Nicknamed.Checked)
                updateNickname(sender, e);
        }
        private void updateOriginGame(object sender, EventArgs e)
        {
            int gameorigin = 0;

            // Error handling for unset field
            try { gameorigin = ToInt32(CB_GameOrigin.SelectedValue.ToString()); }
            catch { gameorigin = 0; }

            if ((gameorigin <= 12) && (gameorigin >= 7))
            {
                // Game Originates In Gen 4; Enable Encounter Type
                CB_EncounterType.Enabled = true;
                Label_EncounterType.Enabled = true;
            }
            else
            {
                CB_EncounterType.Enabled = false;
                Label_EncounterType.Enabled = false;
                CB_EncounterType.SelectedIndex = 0;
            }
            updateLocations(gameorigin);
            getMarkings();
        }
        private void updateLocations(int gameorigin)
        {
            if (gameorigin < 24 && origintrack != "Past") // Load Past Gen Locations
            {
                #region BW2 Met Locations
                {
                    // Allowed Met Locations
                    int[] metlocs = { 0, 60002, 30003 };

                    // Set up
                    List<cbItem> met_list = new List<cbItem>();

                    for (int i = 0; i < metlocs.Length; i++) // add entries to the top
                    {
                        cbItem ncbi = new cbItem();
                        int locval = metlocs[i];
                        string loctext = "";

                        if (locval < 30000)
                            loctext = metBW2_00000[locval];
                        else if (locval < 40000)
                            loctext = metBW2_30000[locval % 10000 - 1];
                        else if (locval < 60000)
                            loctext = metBW2_40000[locval % 10000 - 1];
                        else
                            loctext = metBW2_60000[locval % 10000 - 1];

                        ncbi.Text = loctext;
                        ncbi.Value = locval;
                        met_list.Add(ncbi);
                    }

                    metlocs = new int[] 
                    { 
                        1,2,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,
                        30001,30002,30004,30005,30006,30007,30008,30010,30011,30012,30013,30014,30015,
                        40001,40002,40003,40004,40005,40006,40007,40008,40009,40010,40011,40012,40013,40014,40015,40016,40017,40018,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40032,40033,40034,40035,40036,40037,40038,40039,40040,40041,40042,40043,40044,40045,40046,40047,40048,40049,40050,40051,40052,40053,40054,40055,40056,40057,40058,40059,40060,40061,40062,40063,40064,40065,40066,40067,40068,40069,40070,40071,40072,40073,40074,40075,40076,40077,40078,40079,40080,40081,40082,40083,40084,40085,40086,40087,40088,40089,40090,40091,40092,40093,40094,40095,40096,40097,40098,40099,40100,40101,40102,40103,40104,40105,40106,40107,40108,40109,
                        60001,60003
                    };

                    // Sort the Rest based on String Name
                    string[] lt00000 = new string[metBW2_00000.Length];
                    string[] lt30000 = new string[metBW2_30000.Length];
                    string[] lt40000 = new string[metBW2_40000.Length];
                    string[] lt60000 = new string[metBW2_60000.Length];
                    Array.Copy(metBW2_00000, lt00000, metBW2_00000.Length);
                    Array.Copy(metBW2_30000, lt30000, metBW2_30000.Length);
                    Array.Copy(metBW2_40000, lt40000, metBW2_40000.Length);
                    Array.Copy(metBW2_60000, lt60000, metBW2_60000.Length);
                    Array.Sort(lt00000);
                    Array.Sort(lt30000);
                    Array.Sort(lt40000);
                    Array.Sort(lt60000);

                    // Add the rest of the 00000
                    for (int i = 0; i < lt00000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metBW2_00000, lt00000[i]) + 00000);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt00000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 30000
                    for (int i = 0; i < lt30000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metBW2_30000, lt30000[i]) + 30001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt30000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 40000
                    for (int i = 0; i < lt40000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metBW2_40000, lt40000[i]) + 40001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt40000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 60000
                    for (int i = 0; i < lt60000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metBW2_60000, lt60000[i]) + 60001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt60000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }

                    CB_MetLocation.DataSource = met_list;
                    CB_MetLocation.DisplayMember = "Text";
                    CB_MetLocation.ValueMember = "Value";
                    CB_EggLocation.DataSource = new BindingSource(met_list, null);
                    origintrack = "Past";
                    CB_EggLocation.DisplayMember = "Text";
                    CB_EggLocation.ValueMember = "Value";
                    CB_EggLocation.SelectedValue = 0;
                    if (gameorigin < 20)
                    {
                        CB_MetLocation.SelectedValue = 30001; // Transporter
                    }
                    else CB_MetLocation.SelectedValue = 60001; // Stranger
                }
                #endregion
            }
            else if (gameorigin > 23 && (origintrack != "XY"))
            {
                #region XY Met Locations
                {
                    // Allowed Met Locations
                    int[] metlocs = { 0, 60002, 30002 };

                    // Set up
                    List<cbItem> met_list = new List<cbItem>();

                    for (int i = 0; i < metlocs.Length; i++) // add entries to the top
                    {
                        cbItem ncbi = new cbItem();
                        int locval = metlocs[i];
                        string loctext = "";

                        if (locval < 30000)
                            loctext = metXY_00000[locval];
                        else if (locval < 40000)
                            loctext = metXY_30000[locval % 10000 - 1];
                        else if (locval < 60000)
                            loctext = metXY_40000[locval % 10000 - 1];
                        else
                            loctext = metXY_60000[locval % 10000 - 1];

                        ncbi.Text = loctext;
                        ncbi.Value = locval;
                        met_list.Add(ncbi);
                    }

                    metlocs = new int[] 
                { 
                    2,6,8,10,12,14,16,17,18,20,22,24,26,28,30,32,34,36,38,40,42,44,46,48,50,52,54,56,58,60,62,64,66,68,70,72,74,76,78,82,84,86,88,90,92,94,96,98,100,102,104,106,108,110,112,114,116,118,120,122,124,126,128,130,132,134,136,138,140,142,144,146,148,150,152,154,156,158,160,162,164,166,168,
                    30001,30003,30004,30005,30006,30007,30008,30009,30010,30011,
                    40001,40002,40003,40004,40005,40006,40007,40008,40009,40010,40011,40012,40013,40014,40015,40016,40017,40018,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40032,40033,40034,40035,40036,40037,40038,40039,40040,40041,40042,40043,40044,40045,40046,40047,40048,40049,40050,40051,40052,40053,40054,40055,40056,40057,40058,40059,40060,40061,40062,40063,40064,40065,40066,40067,40068,40069,40070,40071,40072,40073,40074,40075,40076,40077,40078,40079,
                    60001,60003
                };

                    // Sort the Rest based on String Name
                    string[] lt00000 = new string[metXY_00000.Length];
                    string[] lt30000 = new string[metXY_30000.Length];
                    string[] lt40000 = new string[metXY_40000.Length];
                    string[] lt60000 = new string[metXY_60000.Length];
                    Array.Copy(metXY_00000, lt00000, metXY_00000.Length);
                    Array.Copy(metXY_30000, lt30000, metXY_30000.Length);
                    Array.Copy(metXY_40000, lt40000, metXY_40000.Length);
                    Array.Copy(metXY_60000, lt60000, metXY_60000.Length);
                    Array.Sort(lt00000);
                    Array.Sort(lt30000);
                    Array.Sort(lt40000);
                    Array.Sort(lt60000);

                    // Add the rest of the 00000
                    for (int i = 0; i < lt00000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metXY_00000, lt00000[i]) + 00000);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt00000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 30000
                    for (int i = 0; i < lt30000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metXY_30000, lt30000[i]) + 30001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt30000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 40000
                    for (int i = 0; i < lt40000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metXY_40000, lt40000[i]) + 40001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt40000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 60000
                    for (int i = 0; i < lt60000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metXY_60000, lt60000[i]) + 60001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt60000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }

                    CB_MetLocation.DataSource = met_list;
                    CB_MetLocation.DisplayMember = "Text";
                    CB_MetLocation.ValueMember = "Value";
                    CB_EggLocation.DataSource = new BindingSource(met_list, null);
                    CB_EggLocation.DisplayMember = "Text";
                    CB_EggLocation.ValueMember = "Value";
                    origintrack = "XY";
                    CB_EggLocation.SelectedValue = 0;
                    CB_MetLocation.SelectedValue = 0;
                }
                #endregion
            }
            if (gameorigin < 13 && gameorigin > 6 && origintrack != "Gen4")
            {   // Egg Met Locations for Gen 4 are unaltered when transferred to Gen 5. Need a new table if Gen 4 Origin.
                #region HGSS Met Locations
                // Allowed Met Locations
                int[] metlocs = { 0, 2000, 2002, 3001 };

                // Set up
                List<cbItem> met_list = new List<cbItem>();

                for (int i = 0; i < metlocs.Length; i++) // add entries to the top
                {
                    cbItem ncbi = new cbItem();
                    int locval = metlocs[i];
                    string loctext = "";

                    if (locval < 2000)
                        loctext = metHGSS_00000[locval];
                    else if (locval < 3000)
                        loctext = metHGSS_02000[locval % 2000];
                    else
                        loctext = metHGSS_03000[locval % 3000];

                    ncbi.Text = loctext;
                    ncbi.Value = locval;
                    met_list.Add(ncbi);
                }

                metlocs = new int[] 
                { 
                       0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,
                       2000,2001,2002,2003,2004,2005,2006,2008,2009,2010,2011,2012,2013,2014,
                       3000,3001,3002,3003,3004,3005,3006,3007,3008,3009,3010,3011,3012,3013,3014,3015,3016,3017,3018,3019,3020,3021,3022,3023,3024,3025,3026,3027,3028,3029,3030,3031,3032,3033,3034,3035,3036,3037,3038,3039,3040,3041,3042,3043,3044,3045,3046,3047,3048,3049,3050,3051,3052,3053,3054,3055,3056,3057,3058,3059,3060,3061,3062,3063,3064,3065,3066,3067,3068,3069,3070,3071,3072,3073,3074,3075,3076
                };

                // Sort the Rest based on String Name
                string[] lt00000 = new string[metHGSS_00000.Length];
                string[] lt02000 = new string[metHGSS_02000.Length];
                string[] lt03000 = new string[metHGSS_03000.Length];
                Array.Copy(metHGSS_00000, lt00000, metHGSS_00000.Length);
                Array.Copy(metHGSS_02000, lt02000, metHGSS_02000.Length);
                Array.Copy(metHGSS_03000, lt03000, metHGSS_03000.Length);
                Array.Sort(lt00000);
                Array.Sort(lt02000);
                Array.Sort(lt03000);

                // Add the rest of the 0000
                for (int i = 0; i < lt00000.Length; i++)
                {
                    int locnum = Array.IndexOf(metlocs, Array.IndexOf(metHGSS_00000, lt00000[i]) + 0000);
                    if (locnum > 0)	// If the location is allowed (if found, >0)
                    {
                        cbItem ncbi = new cbItem();
                        ncbi.Text = lt00000[i];
                        ncbi.Value = metlocs[locnum];
                        met_list.Add(ncbi);
                    }
                }
                // Add the rest of the 2000
                for (int i = 0; i < lt02000.Length; i++)
                {
                    int locnum = Array.IndexOf(metlocs, Array.IndexOf(metHGSS_02000, lt02000[i]) + 2000);
                    if (locnum > 0)	// If the location is allowed (if found, >0)
                    {
                        cbItem ncbi = new cbItem();
                        ncbi.Text = lt02000[i];
                        ncbi.Value = metlocs[locnum];
                        met_list.Add(ncbi);
                    }
                }
                // Add the rest of the 3000
                for (int i = 0; i < lt03000.Length; i++)
                {
                    int locnum = Array.IndexOf(metlocs, Array.IndexOf(metHGSS_03000, lt03000[i]) + 3000);
                    if (locnum > 0)	// If the location is allowed (if found, >0)
                    {
                        cbItem ncbi = new cbItem();
                        ncbi.Text = lt03000[i];
                        ncbi.Value = metlocs[locnum];
                        met_list.Add(ncbi);
                    }
                }

                CB_EggLocation.DataSource = met_list;
                origintrack = "Gen4";
                CB_EggLocation.DisplayMember = "Text";
                CB_EggLocation.ValueMember = "Value";
                CB_EggLocation.SelectedValue = 0;
                #endregion
            }
        }
        private void updateExtraByteValue(object sender, EventArgs e)
        {
            // Changed Extra Byte's Value
            int value = ToInt32(TB_ExtraByte.Text);
            if (value < 0x100) // If Value Valid, write.
            {
                int offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
                buff[offset] = (byte)value;
                TB_ExtraByte.BackColor = Color.White;
            }
            else { TB_ExtraByte.BackColor = Color.Red; }
        }
        private void updateExtraByteIndex(object sender, EventArgs e)
        {
            int offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
            // Byte changed, need to refresh the Text box for the byte's value.
            TB_ExtraByte.Text = buff[offset].ToString();
        }
        private void updateNickname(object sender, EventArgs e)
        {

            if (!CHK_Nicknamed.Checked)
            {
                // Fetch Current Species and set it as Nickname Text
                int species = getIndex(CB_Species);
                if (species == 0 || species > 721)
                {
                    TB_Nickname.Text = "";
                }
                else
                {
                    // get language
                    int lang = getIndex(CB_Language);
                    string[] lang_val = { "en", "ja", "fr", "it", "de", "es", "ko" };

                    string l = "";
                    switch (lang)
                    {
                        case 1: l = "ja"; break;
                        case 2: l = "en"; break;
                        case 3: l = "fr"; break;
                        case 4: l = "it"; break;
                        case 5: l = "de"; break;
                        case 7: l = "es"; break;
                        case 8: l = "ko"; break;
                        default: l = curlanguage; break;
                    }
                    TB_Nickname.Text = getStringList("Species", l)[species];
                }
            }
        }
        private void updateNotOT(object sender, EventArgs e)
        {
            if (TB_OTt2.Text == "")
            {
                Label_CTGender.Text = "";
                TB_Friendship.Text = buff[0xCA].ToString();
                GB_OT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                GB_nOT.BackColor = Color.Transparent;
                buff[0x93] = 0;
            }
            else if (Label_CTGender.Text == "")
                Label_CTGender.Text = "♂";
        }
        private void updatePKRSCured(object sender, EventArgs e)
        {
            // Cured PokeRus is toggled
            if (CHK_Cured.Checked)
            {
                // Has Had PokeRus
                CB_PKRSDays.Enabled = false;
                CB_PKRSDays.SelectedIndex = 0;

                CB_PKRSStrain.Enabled = true;
                CHK_Infected.Checked = true;

                if (CB_PKRSStrain.SelectedIndex == 0)
                {   // Give it Strain 1 by Default
                    CB_PKRSStrain.SelectedIndex = 1;
                }
            }
            else if (!CHK_Infected.Checked)
            {
                // Not Infected, Disable the other
                CB_PKRSStrain.Enabled = false;
                CB_PKRSStrain.SelectedIndex = 0;
            }
            else
            { // Still Infected for a duration
                CB_PKRSDays.Enabled = true;
                CB_PKRSDays.SelectedValue = 1;
            }
            // if not cured yet, days > 0
            if (!CHK_Cured.Checked && CHK_Infected.Checked && CB_PKRSDays.SelectedIndex == 0) CB_PKRSDays.SelectedIndex++;
        

            getMarkings();
        }
        private void updatePKRSInfected(object sender, EventArgs e)
        {
            //// Infected PokeRus is toggled
            //if (CHK_Infected.Checked)
            //{
            //    // Has PokeRus ~ Give it Strain 1 by Default
            //    CB_PKRSDays.Enabled = true;
            //    CB_PKRSStrain.Enabled = true;
            //    CB_PKRSDays.SelectedValue = 1;
            //    CB_PKRSStrain.SelectedValue = 1;
            //}
            //else // No Pokerus
            //{
            //    // No Pokerus ~ Give it Strain 0
            //    CB_PKRSDays.Enabled = false;
            //    CB_PKRSStrain.Enabled = false;
            //    CB_PKRSDays.SelectedValue = 0;
            //    CB_PKRSStrain.SelectedValue = 0;
            //}

            // cleaner way of doing this:
            CB_PKRSDays.Enabled = CB_PKRSStrain.Enabled = CHK_Infected.Checked;
            if (!CHK_Infected.Checked) { CB_PKRSStrain.SelectedIndex = 0; CB_PKRSDays.SelectedIndex = 0; }
            else if (CB_PKRSStrain.SelectedIndex == 0) CB_PKRSStrain.SelectedIndex++;
            CB_PKRSDays.SelectedValue = CB_PKRSStrain.SelectedValue = Convert.ToInt32(CHK_Infected.Checked);
            // if not cured yet, days > 0
            if (!CHK_Cured.Checked && CHK_Infected.Checked && CB_PKRSDays.SelectedIndex == 0) CB_PKRSDays.SelectedIndex++;
        }
        private void updateIsEgg(object sender, EventArgs e)
        {
            if (CHK_IsEgg.Checked)
            {
                CHK_Nicknamed.Checked = false;
                TB_Nickname.Text = eggname;
                TB_Friendship.Text = "1";

                // If we are an egg, it won't have a met location.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CAL_MetDate.Value = new DateTime(2000, 01, 01);
                CB_MetLocation.SelectedValue = 0;


            }
            else // Not Egg
            {
                if (!CHK_Nicknamed.Checked)
                {
                    updateNickname(null, null);
                }
                TB_Friendship.Text = ((int)Friendship().Rows[getIndex(CB_Species)][1]).ToString();
                
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
            // Disable 
            CB_MetLocation.Enabled = !CHK_IsEgg.Checked;
            CAL_MetDate.Enabled = !CHK_IsEgg.Checked;
        }
        private void updateMetAsEgg(object sender, EventArgs e)
        {
            GB_EggConditions.Enabled = CHK_AsEgg.Checked;
            if (!CHK_AsEgg.Checked)
            {
                // Remove egg met data
                CHK_IsEgg.Checked = false;
                CAL_EggDate.Value = new DateTime(2000, 01, 01);
                CB_EggLocation.SelectedValue = 0;
            }
        }
        private void updateShinyPID(object sender, EventArgs e)
        {
            uint PID = getHEXval(TB_PID);
            uint UID = (PID >> 16);
            uint LID = (PID & 0xFFFF);
            uint PSV = UID ^ LID;
            uint TID = ToUInt32(TB_TID.Text);
            uint SID = ToUInt32(TB_SID.Text);
            uint TSV = TID ^ SID;
            uint XOR = TSV ^ PSV;

            // Check to see if we actually did it right...
            if (((XOR > 8) && (CB_GameOrigin.SelectedIndex < 24)) || ((XOR > 16) && (CB_GameOrigin.SelectedIndex > 24)))
            {
                TB_PID.Text = ((UID ^ XOR) * 0x10000 + LID).ToString("X8");
            }
            getIsShiny();
        }
        private void updateTIDSID(object sender, EventArgs e)
        {
            // Trim out nonhex characters
            RemoveTroublesomeCharacters(TB_PID);
            RemoveTroublesomeCharacters(TB_EC);
            
            getIsShiny();
            UpdateIVs(sender, e);   // If the PID is changed, PID%6 (Characteristic) might be changed. 
            TB_PID.Select(60, 0);   // position cursor at end of field
        }
        private MaskedTextBox[] shuffle(MaskedTextBox[] charArray)
        {
            MaskedTextBox[] shuffledArray = new MaskedTextBox[charArray.Length];
            int rndNo;

            Random rnd = new Random();
            for (int i = charArray.Length; i >= 1; i--)
            {
                rndNo = rnd.Next(1, i + 1) - 1;
                shuffledArray[i - 1] = charArray[rndNo];
                charArray[rndNo] = charArray[i - 1];
            }
            return shuffledArray;
        }
        private void validateComboBox(object sender, CancelEventArgs e)
        {
            if (!(sender is ComboBox)) { return; }

            ComboBox cb = sender as ComboBox;
            cb.SelectionLength = 0;
            if ((cb.SelectedValue == null))
            {
                cb.BackColor = Color.DarkSalmon;
            }
            else
                cb.BackColor = defaultwhite;
        }
        private void validateComboBox2(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            validateComboBox(sender, e as CancelEventArgs);
            if (cb == CB_Ability)
                updateAbilityNumber();
            else if ((cb == CB_Move1) || (cb == CB_Move2) || (cb == CB_Move3) || (cb == CB_Move4))
                updatePP(sender, e);
        }
        private void removedropCB(object sender, KeyEventArgs e)
        {
            ((ComboBox)sender).DroppedDown = false;
        }
        private void updateStats()
        {
            // First verify that our input stats are valid.
            MaskedTextBox[] mbIV = new MaskedTextBox[] {
                TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPAIV, TB_SPDIV, TB_SPEIV,
            };
            MaskedTextBox[] mbEV = new MaskedTextBox[] {
                TB_HPEV, TB_ATKEV, TB_DEFEV, TB_SPAEV, TB_SPDEV, TB_SPEEV,
            };
            for (int i = 0; i < 6; i++)
            {
                if (ToInt32(mbIV[i].Text) > 31 || ToInt32(mbEV[i].Text) > 252)
                {
                    return; // if anything is illegal, don't calc stats.
                }
            }

            // EVs and IVs are valid, continue to generate the stats.
            species = getSpecies();
            int level = ToInt32(TB_Level.Text);
            if (level == 0) { level = 1; }
            DataTable spectable = SpeciesTable();
            int HP_IV = ToInt32(TB_HPIV.Text);
            int ATK_IV = ToInt32(TB_ATKIV.Text);
            int DEF_IV = ToInt32(TB_DEFIV.Text);
            int SPA_IV = ToInt32(TB_SPAIV.Text);
            int SPD_IV = ToInt32(TB_SPDIV.Text);
            int SPE_IV = ToInt32(TB_SPEIV.Text);

            int HP_EV = ToInt32(TB_HPEV.Text);
            int ATK_EV = ToInt32(TB_ATKEV.Text);
            int DEF_EV = ToInt32(TB_DEFEV.Text);
            int SPA_EV = ToInt32(TB_SPAEV.Text);
            int SPD_EV = ToInt32(TB_SPDEV.Text);
            int SPE_EV = ToInt32(TB_SPEEV.Text);

            int HP_B = (int)spectable.Rows[species][2];
            int ATK_B = (int)spectable.Rows[species][3];
            int DEF_B = (int)spectable.Rows[species][4];
            int SPA_B = (int)spectable.Rows[species][5];
            int SPD_B = (int)spectable.Rows[species][6];
            int SPE_B = (int)spectable.Rows[species][7];

            int form = CB_Form.SelectedIndex;
            // Form Stat Recalculation
            if (form != 0)
            {
                if ((form == 1) && (species == 3)) { HP_B = 80; ATK_B = 100; DEF_B = 123; SPA_B = 122; SPD_B = 120; SPE_B = 80; }
                else if ((form == 1) && (species == 6)) { HP_B = 78; ATK_B = 130; DEF_B = 111; SPA_B = 130; SPD_B = 85; SPE_B = 100; }
                else if ((form == 2) && (species == 6)) { HP_B = 78; ATK_B = 104; DEF_B = 78; SPA_B = 159; SPD_B = 115; SPE_B = 100; }
                else if ((form == 1) && (species == 9)) { HP_B = 79; ATK_B = 103; DEF_B = 120; SPA_B = 135; SPD_B = 115; SPE_B = 78; }
                else if ((form == 1) && (species == 65)) { HP_B = 55; ATK_B = 50; DEF_B = 65; SPA_B = 175; SPD_B = 95; SPE_B = 150; }
                else if ((form == 1) && (species == 94)) { HP_B = 60; ATK_B = 65; DEF_B = 80; SPA_B = 170; SPD_B = 95; SPE_B = 130; }
                else if ((form == 1) && (species == 115)) { HP_B = 105; ATK_B = 125; DEF_B = 100; SPA_B = 60; SPD_B = 100; SPE_B = 100; }
                else if ((form == 1) && (species == 127)) { HP_B = 65; ATK_B = 155; DEF_B = 120; SPA_B = 65; SPD_B = 90; SPE_B = 105; }
                else if ((form == 1) && (species == 130)) { HP_B = 95; ATK_B = 155; DEF_B = 109; SPA_B = 70; SPD_B = 130; SPE_B = 81; }
                else if ((form == 1) && (species == 142)) { HP_B = 80; ATK_B = 135; DEF_B = 85; SPA_B = 70; SPD_B = 95; SPE_B = 150; }
                else if ((form == 1) && (species == 150)) { HP_B = 106; ATK_B = 190; DEF_B = 100; SPA_B = 154; SPD_B = 100; SPE_B = 130; }
                else if ((form == 2) && (species == 150)) { HP_B = 106; ATK_B = 150; DEF_B = 70; SPA_B = 194; SPD_B = 120; SPE_B = 140; }
                else if ((form == 1) && (species == 181)) { HP_B = 90; ATK_B = 95; DEF_B = 105; SPA_B = 165; SPD_B = 110; SPE_B = 45; }
                else if ((form == 1) && (species == 212)) { HP_B = 70; ATK_B = 150; DEF_B = 140; SPA_B = 65; SPD_B = 100; SPE_B = 75; }
                else if ((form == 1) && (species == 214)) { HP_B = 80; ATK_B = 185; DEF_B = 115; SPA_B = 40; SPD_B = 105; SPE_B = 75; }
                else if ((form == 1) && (species == 229)) { HP_B = 75; ATK_B = 90; DEF_B = 90; SPA_B = 140; SPD_B = 90; SPE_B = 115; }
                else if ((form == 1) && (species == 248)) { HP_B = 100; ATK_B = 164; DEF_B = 150; SPA_B = 95; SPD_B = 120; SPE_B = 71; }
                else if ((form == 1) && (species == 257)) { HP_B = 80; ATK_B = 160; DEF_B = 80; SPA_B = 130; SPD_B = 80; SPE_B = 100; }
                else if ((form == 1) && (species == 282)) { HP_B = 68; ATK_B = 85; DEF_B = 65; SPA_B = 165; SPD_B = 135; SPE_B = 100; }
                else if ((form == 1) && (species == 303)) { HP_B = 50; ATK_B = 105; DEF_B = 125; SPA_B = 55; SPD_B = 95; SPE_B = 50; }
                else if ((form == 1) && (species == 306)) { HP_B = 70; ATK_B = 140; DEF_B = 230; SPA_B = 60; SPD_B = 80; SPE_B = 50; }
                else if ((form == 1) && (species == 308)) { HP_B = 60; ATK_B = 100; DEF_B = 85; SPA_B = 80; SPD_B = 85; SPE_B = 100; }
                else if ((form == 1) && (species == 310)) { HP_B = 70; ATK_B = 75; DEF_B = 80; SPA_B = 135; SPD_B = 80; SPE_B = 135; }
                else if ((form == 1) && (species == 354)) { HP_B = 64; ATK_B = 165; DEF_B = 75; SPA_B = 93; SPD_B = 83; SPE_B = 75; }
                else if ((form == 1) && (species == 359)) { HP_B = 65; ATK_B = 150; DEF_B = 60; SPA_B = 115; SPD_B = 60; SPE_B = 115; }
                else if ((form == 1) && (species == 380)) { HP_B = 80; ATK_B = 100; DEF_B = 120; SPA_B = 140; SPD_B = 150; SPE_B = 110; }
                else if ((form == 1) && (species == 381)) { HP_B = 80; ATK_B = 130; DEF_B = 100; SPA_B = 160; SPD_B = 120; SPE_B = 110; }
                else if ((form == 1) && (species == 386)) { HP_B = 50; ATK_B = 180; DEF_B = 20; SPA_B = 180; SPD_B = 20; SPE_B = 150; }
                else if ((form == 2) && (species == 386)) { HP_B = 50; ATK_B = 70; DEF_B = 160; SPA_B = 70; SPD_B = 160; SPE_B = 90; }
                else if ((form == 3) && (species == 386)) { HP_B = 50; ATK_B = 95; DEF_B = 90; SPA_B = 95; SPD_B = 90; SPE_B = 180; }
                else if ((form == 1) && (species == 413)) { HP_B = 60; ATK_B = 79; DEF_B = 105; SPA_B = 59; SPD_B = 85; SPE_B = 36; }
                else if ((form == 2) && (species == 413)) { HP_B = 60; ATK_B = 69; DEF_B = 95; SPA_B = 69; SPD_B = 95; SPE_B = 36; }
                else if ((form == 1) && (species == 445)) { HP_B = 108; ATK_B = 170; DEF_B = 115; SPA_B = 120; SPD_B = 95; SPE_B = 92; }
                else if ((form == 1) && (species == 448)) { HP_B = 70; ATK_B = 145; DEF_B = 88; SPA_B = 140; SPD_B = 70; SPE_B = 112; }
                else if ((form == 1) && (species == 460)) { HP_B = 90; ATK_B = 132; DEF_B = 105; SPA_B = 132; SPD_B = 105; SPE_B = 30; }
                else if ((form == 1) && (species == 487)) { HP_B = 150; ATK_B = 120; DEF_B = 100; SPA_B = 120; SPD_B = 100; SPE_B = 90; }
                else if ((form == 1) && (species == 492)) { HP_B = 100; ATK_B = 103; DEF_B = 75; SPA_B = 120; SPD_B = 75; SPE_B = 127; }
                else if ((form == 1) && (species == 555)) { HP_B = 105; ATK_B = 30; DEF_B = 105; SPA_B = 140; SPD_B = 105; SPE_B = 55; }
                else if ((form == 1) && (species == 641)) { HP_B = 79; ATK_B = 100; DEF_B = 80; SPA_B = 110; SPD_B = 90; SPE_B = 121; }
                else if ((form == 1) && (species == 642)) { HP_B = 79; ATK_B = 105; DEF_B = 70; SPA_B = 145; SPD_B = 80; SPE_B = 101; }
                else if ((form == 1) && (species == 645)) { HP_B = 89; ATK_B = 145; DEF_B = 90; SPA_B = 105; SPD_B = 80; SPE_B = 91; }
                else if ((form == 1) && (species == 646)) { HP_B = 125; ATK_B = 170; DEF_B = 100; SPA_B = 120; SPD_B = 90; SPE_B = 95; }
                else if ((form == 2) && (species == 646)) { HP_B = 125; ATK_B = 120; DEF_B = 90; SPA_B = 170; SPD_B = 100; SPE_B = 95; }
                else if ((form == 1) && (species == 648)) { HP_B = 100; ATK_B = 128; DEF_B = 90; SPA_B = 77; SPD_B = 77; SPE_B = 128; }
                else if ((form == 5) && (species == 670)) { HP_B = 74; ATK_B = 65; DEF_B = 67; SPA_B = 125; SPD_B = 128; SPE_B = 92; }
                else if ((form == 1) && (species == 681)) { HP_B = 60; ATK_B = 150; DEF_B = 50; SPA_B = 150; SPD_B = 50; SPE_B = 60; }
                else if ((form == 1) && (species == 710)) { HP_B = 49; ATK_B = 66; DEF_B = 70; SPA_B = 44; SPD_B = 55; SPE_B = 51; }
                else if ((form == 2) && (species == 710)) { HP_B = 54; ATK_B = 66; DEF_B = 70; SPA_B = 44; SPD_B = 55; SPE_B = 46; }
                else if ((form == 3) && (species == 710)) { HP_B = 59; ATK_B = 66; DEF_B = 70; SPA_B = 44; SPD_B = 55; SPE_B = 41; }
                else if ((form == 1) && (species == 711)) { HP_B = 65; ATK_B = 90; DEF_B = 122; SPA_B = 58; SPD_B = 75; SPE_B = 84; }
                else if ((form == 2) && (species == 711)) { HP_B = 75; ATK_B = 95; DEF_B = 122; SPA_B = 58; SPD_B = 75; SPE_B = 69; }
                else if ((form == 3) && (species == 711)) { HP_B = 85; ATK_B = 100; DEF_B = 122; SPA_B = 58; SPD_B = 75; SPE_B = 54; }
            }
            // End Form Stat Recalc

            DataTable naturetable = NatureTable();
            int nature = getNature();
            int n1 = (int)naturetable.Rows[nature][1];
            int n2 = (int)naturetable.Rows[nature][2];
            int n3 = (int)naturetable.Rows[nature][4];
            int n4 = (int)naturetable.Rows[nature][5];
            int n5 = (int)naturetable.Rows[nature][3];

            Stat_HP.Text = ((((HP_IV + (2 * HP_B) + (HP_EV / 4) + 100) * level) / 100) + 10).ToString();
            Stat_ATK.Text = ((((((ATK_IV + (2 * ATK_B) + (ATK_EV / 4)) * level) / 100) + 5) * n1) / 10).ToString();
            Stat_DEF.Text = ((((((DEF_IV + (2 * DEF_B) + (DEF_EV / 4)) * level) / 100) + 5) * n2) / 10).ToString();
            Stat_SPA.Text = ((((((SPA_IV + (2 * SPA_B) + (SPA_EV / 4)) * level) / 100) + 5) * n3) / 10).ToString();
            Stat_SPD.Text = ((((((SPD_IV + (2 * SPD_B) + (SPD_EV / 4)) * level) / 100) + 5) * n4) / 10).ToString();
            Stat_SPE.Text = ((((((SPE_IV + (2 * SPE_B) + (SPE_EV / 4)) * level) / 100) + 5) * n5) / 10).ToString();
        }
        private void updateAbilityList()
        {
            if (!init)
                return;
            int newabil = Convert.ToInt16(TB_AbilityNumber.Text) >> 1;
            int species = getIndex(CB_Species);
            int[] abils = { 0, 0, 0 };
            //
            // Alternate Forms have different abilities. We must account for them!
            //

            if (CB_Form.SelectedIndex > 0)
            {
                int formnum = CB_Form.SelectedIndex;
                if (species == 492 && formnum == 1) { species = 727; }      // Shaymin
                else if (species == 487 && formnum == 1) { species = 728; } // Giratina-O
                else if (species == 550 && formnum == 1) { species = 738; } // Basculin Blue
                else if (species == 646 && formnum == 1) { species = 741; } // Kyurem White
                else if (species == 646 && formnum == 1) { species = 742; } // Kyurem Black
                else if (species == 641 && formnum == 1) { species = 744; } // Tornadus-T
                else if (species == 642 && formnum == 1) { species = 745; } // Thundurus-T
                else if (species == 645 && formnum == 1) { species = 746; } // Landorus-T
                else if (species == 678 && formnum == 1) { species = 748; } // MeowsticF

                else if (species == 094 && formnum == 1) { species = 747; } // Mega Gengar
                else if (species == 282 && formnum == 1) { species = 758; } // Mega Gardevoir
                else if (species == 181 && formnum == 1) { species = 759; } // Mega Ampharos
                else if (species == 003 && formnum == 1) { species = 760; } // Mega Venusaur
                else if (species == 006 && formnum == 1) { species = 761; } // Mega CharizardX
                else if (species == 006 && formnum == 2) { species = 762; } // Mega CharizardY
                else if (species == 150 && formnum == 1) { species = 763; } // Mega MewtwoX
                else if (species == 150 && formnum == 2) { species = 764; } // Mega MewtwoY
                else if (species == 257 && formnum == 1) { species = 765; } // Mega Blaziken
                else if (species == 308 && formnum == 1) { species = 766; } // Mega Medicham
                else if (species == 229 && formnum == 1) { species = 767; } // Mega Houndoom
                else if (species == 306 && formnum == 1) { species = 768; } // Mega Aggron
                else if (species == 354 && formnum == 1) { species = 769; } // Mega Banette
                else if (species == 248 && formnum == 1) { species = 770; } // Mega Tyranitar
                else if (species == 212 && formnum == 1) { species = 771; } // Mega Scizor
                else if (species == 127 && formnum == 1) { species = 772; } // Mega Pinsir
                else if (species == 142 && formnum == 1) { species = 773; } // Mega Aerodactyl
                else if (species == 448 && formnum == 1) { species = 774; } // Mega Lucario
                else if (species == 460 && formnum == 1) { species = 775; } // Mega Abomasnow
                else if (species == 009 && formnum == 1) { species = 777; } // Mega Blastoise
                else if (species == 115 && formnum == 1) { species = 778; } // Mega Kangaskhan
                else if (species == 130 && formnum == 1) { species = 779; } // Mega Gyarados
                else if (species == 359 && formnum == 1) { species = 780; } // Mega Absol
                else if (species == 065 && formnum == 1) { species = 781; } // Mega Alakazam
                else if (species == 214 && formnum == 1) { species = 782; } // Mega Heracross
                else if (species == 303 && formnum == 1) { species = 783; } // Mega Mawile
                else if (species == 310 && formnum == 1) { species = 784; } // Mega Manectric
                else if (species == 445 && formnum == 1) { species = 785; } // Mega Garchomp
                else if (species == 381 && formnum == 1) { species = 786; } // Mega Latios
                else if (species == 380 && formnum == 1) { species = 787; } // Mega Latias
            }
            Array.Copy(speciesability, species * 4 + 1, abils, 0, 3);

            // Build Ability List
            List<string> ability_list = new List<string>();
            ability_list.Add(abilitylist[abils[0]] + " (1)");
            ability_list.Add(abilitylist[abils[1]] + " (2)");
            ability_list.Add(abilitylist[abils[2]] + " (H)");
            CB_Ability.DataSource = ability_list;

            if (newabil < 3) CB_Ability.SelectedIndex = newabil;
            else CB_Ability.SelectedIndex = 0;
        }
        private void updateAbilityNumber()
        {
            TB_AbilityNumber.Text = (1 << CB_Ability.SelectedIndex).ToString();
        }
        // Main Menu Strip UI Functions // 
        private void mainmenuOpen(object sender, EventArgs e)
        {
            DialogResult result = OpenPKX.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = OpenPKX.FileName;
                string ext = Path.GetExtension(path);
                byte[] input = File.ReadAllBytes(path);
                try
                {
                    openfile(input, path, ext);
                }
                catch
                {
                    try
                    {
                        byte[] blank = encryptArray(new Byte[260]);
                        for (int i = 0; i < 232; i++)
                        {
                            blank[i] = (byte)(blank[i] ^ input[i]);
                        }
                        openfile(blank, path, ext);
                    }
                    catch
                    {
                        openfile(input, path, ext);
                    }
                }
            }
        }
        private void mainmenuSave(object sender, EventArgs e)
        {
            if (!verifiedpkx()) { return; }
            SavePKX.FileName = TB_Nickname.Text + " - " + TB_PID.Text;
            DialogResult result = SavePKX.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string path = SavePKX.FileName;
                string ext = Path.GetExtension(path);

                if (File.Exists(path))
                {
                    // File already exists, save a .bak
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(path + ".bak", backupfile);
                }
                byte[] pkx = preparepkx(buff);

                if ((ext == ".ekx") || (ext == ".bin") || (ext == ".pkx") || (ext == ".ek6") || (ext == ".pk6"))
                {
                    if ((ext == ".ekx") || (ext == ".bin") || (ext == ".ek6"))
                    {
                        // User Requested Encrypted File
                        pkx = encryptArray(pkx);
                    }
                    File.WriteAllBytes(path, pkx.ToArray());
                }
                else
                {
                    string message = "Foreign File Extension. Exporting as encrypted.";
                    string caption = "Error Detected in Input";
                    MessageBox.Show(message, caption);
                    pkx = encryptArray(pkx);
                    File.WriteAllBytes(path, pkx.ToArray());
                }
            }
        }
        private void mainmenuExit(object sender, EventArgs e)
        {
            Close();
        }
        private void mainmenuAbout(object sender, EventArgs e)
        {
            string caption = "About";
            string message = "PKHeX - By Kaphotics.\n\nUI Inspiried by Codr's PokeGen.\n\nThanks to all the researchers!";
            MessageBox.Show(message, caption);
        }
        private void mainmenug5pkm(object sender, EventArgs e)
        {
            openg5pkm();
        }
        private void mainmenuWiden(object sender, EventArgs e)
        {
            int newwidth;
            if (Width < Height)
            {
                newwidth = (this.Width * (54000 / 260)) / 100 + 2;
                tabBoxMulti.Enabled = true;
                tabBoxMulti.SelectedIndex = 0;
            }
            else
            {
                newwidth = (this.Width * (26000 / 540)) / 100 + 1;
            }
            this.Width = newwidth;
        }
        // Main Menu Subfunctions // 
        private void openfile(byte[] input, string path, string ext)
        {

            if ((input.Length == 363) && (input[0x6B] == 0) && (input[0x6C] == 00))
            {
                // EAD Packet of 363 length
                byte[] c = new Byte[260];
                Array.Copy(input, 0x67, c, 0, 260);
                input = c;
            }
            else if ((input.Length == 407) && (input[0x98] == 0) && (input[0x99] == 00))
            {
                // EAD Packet of 407 length
                byte[] c = new Byte[260];
                Array.Copy(input, 0x93, c, 0, 260);
                input = c;
            }

            // Verify the Data Input Size is Proper
            #region SAVE FILE LOADING
            if (input.Length == 0x100000)
            {
                B_ExportSAV.Enabled = false;
                B_SwitchSAV.Enabled = false;
                B_OUTPasserby.Enabled = B_OUTHallofFame.Enabled = B_JPEG.Enabled = false;
                if ((BitConverter.ToUInt32(input, 0x100) != 0x41534944) && (BitConverter.ToUInt32(input, 0x5234) != 0x6E69616D))
                {
                    DialogResult dialogResult = MessageBox.Show("Save file is not decrypted.\r\n\r\nPress Yes to ignore this warning and continue loading the save file.", "Error", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DialogResult sdr = MessageBox.Show("Press Yes to load the sav at 0x3000, No for the one at 0x82000", "Prompt", MessageBoxButtons.YesNoCancel);
                        if (sdr == DialogResult.Cancel)
                        {
                            return;
                        }
                        else if (sdr == DialogResult.Yes)
                        {
                            savindex = 0;
                            B_SwitchSAV.Enabled = true;
                        }
                        else savindex = 1;
                        B_SwitchSAV.Enabled = true;
                        opensave(input, path, ext);
                    }
                }
                else if (detectSAVIndex(input) == 2)
                {
                    DialogResult dialogResult = MessageBox.Show("Hash verification failed.\r\n\r\nPress Yes to ignore this warning and continue loading the save file.", "Error", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DialogResult sdr = MessageBox.Show("Press Yes to load the sav at 0x3000, No for the one at 0x82000", "Prompt", MessageBoxButtons.YesNoCancel);
                        if (sdr == DialogResult.Cancel)
                        {
                            savindex = 0;
                            return;
                        }
                        else if (sdr == DialogResult.Yes)
                        {
                            savindex = 0;
                            B_SwitchSAV.Enabled = true;
                        }
                        else savindex = 1;
                        B_SwitchSAV.Enabled = true;
                        opensave(input, path, ext);
                    }
                }
                else
                {
                    B_ExportSAV.Enabled = true;
                    B_SwitchSAV.Enabled = true;
                    savindex = detectSAVIndex(input);
                    opensave(input, path, ext);
                }
                B_OUTPasserby.Enabled = B_OUTHallofFame.Enabled = B_JPEG.Enabled = true;
            }
            #endregion
            else if ((input.Length == 260) || (input.Length == 232))
            {
                // Check if Input is PKX
                if ((ext == ".pk6") || (ext == ".ek6") || (ext == ".pkx") || (ext == ".ekx") || (ext == ".bin") || (ext == ""))
                {
                    // Check if either is encrypted from another program.
                    if ((input[0xC8] == 0) && (input[0xC9] == 0) && (input[0x58] == 0) && (input[0x59] == 0))
                    {   // File isn't encrypted.
                        buff = input;
                        populatefields(buff);
                    }
                    else
                    {
                        // File is encrypted.
                        buff = decryptArray(input);
                        populatefields(buff);
                    }
                }
                else // to convert g5pkm
                {
                    try 
                    {
                        buff = convertPK5toPK6(input);
                        populatefields(buff);
                        string message = "Loaded previous generation PKM. Conversion attempted.";
                        string caption = "Alert";
                        MessageBox.Show(message, caption);
                    }
                    catch
                    {
                        string message = "Foreign File Extension.\nOnly .pk* .ek* .bin supported.";
                        string caption = "Error Detected in Input";
                        MessageBox.Show(message, caption);
                    }
                }
                // End Work
            }
            else
            {
                if ((input.Length == 136) || (input.Length == 220)) // to convert g5pkm
                {
                    try // to convert g5pkm
                    {
                        buff = convertPK5toPK6(input);
                        populatefields(buff);
                        string message = "Loaded previous generation PKM. Conversion attempted.";
                        string caption = "Alert";
                        MessageBox.Show(message, caption);
                    }
                    catch
                    {
                        string message = "Loaded previous generation PKM. Conversion failed.";
                        string caption = "Alert";
                        MessageBox.Show(message, caption);
                    }
                }
                else
                {
                    string message = "Attempted to load an unsupported file type/size.";
                    string caption = "Error";
                    MessageBox.Show(message, caption);
                }
            }
        }
        private void opensave(byte[] input, string path, string ext)
        {
            if (Width < Height) // SAV Interface Not Open
            {
                int newwidth = (this.Width * (54000 / 260)) / 100 + 2;
                this.Width = newwidth;
            }
            Menu_OpenBoxUI.Visible = false;
            savefile = input;
            savedited = false;
            L_Save.Text = "SAV: " + Path.GetFileName(path);
            
            // Enable Secondary Tools
            tabBoxMulti.Enabled = true;
            GB_SAVtools.Enabled = true;

            // Get Next Active Save File
            C_BoxSelect.SelectedIndex = 0;
            tabBoxMulti.SelectedIndex = 0;

            getBoxNames();      // Display the Box Names
            getPKXBoxes();   // Reload all of the PKX Windows
            getSAVLabel();   // Reload the label indicating current save

            // Logic to allow unlocking of Switch SAV
            // Setup SHA
            SHA256 mySHA256 = SHA256Managed.Create();

            // Check both IVFC Hashes
            byte[] zeroarray = new Byte[0x200];
            Array.Copy(savefile, 0x2000 + 0 * 0x7F000, zeroarray, 0, 0x20);
            byte[] hashValue1 = mySHA256.ComputeHash(zeroarray);
            Array.Copy(savefile, 0x2000 + 1 * 0x7F000, zeroarray, 0, 0x20);
            byte[] hashValue2 = mySHA256.ComputeHash(zeroarray);

            byte[] realHash1 = new Byte[0x20];
            byte[] realHash2 = new Byte[0x20];

            Array.Copy(savefile, 0x43C - 0 * 0x130, realHash1, 0, 0x20);
            Array.Copy(savefile, 0x43C - 1 * 0x130, realHash2, 0, 0x20);

            B_SwitchSAV.Enabled = (hashValue1.SequenceEqual(realHash1) && hashValue2.SequenceEqual(realHash2));
            getSAVOffsets();
        }
        private void openg5pkm()
        {
            OpenFileDialog openg5pkm = new OpenFileDialog();
            openg5pkm.Filter = "PK5 File|*.pkm;*.pk5";
            if (openg5pkm.ShowDialog() == DialogResult.OK)
            {
                string path = openg5pkm.FileName;
                string ext = Path.GetExtension(path);
                byte[] pkm = File.ReadAllBytes(path);
                if (((pkm.Length == 136) || (pkm.Length == 220)) && ((ext == ".pkm") || (ext == ".pk5")))
                {
                    buff = convertPK5toPK6(pkm);
                    populatefields(buff);
                }
                else
                {
                    string message = "Did not select a valid PKX";
                    string caption = "Input Error";
                    MessageBox.Show(message, caption);
                }
            }
        }
        // PID Rerolling Functions //  
        private uint getRandomPID(int gt, int cg)
        {
            var r = new Random();
            uint pid = (uint)rnd32();
            if (gt > 255)
                return pid;
            while (true) // Loop until we find a suitable PID
            {
                uint gv = (pid & 0xFF);
                if (cg == 2) // Genderless
                    break;  // PID Passes
                else if ((cg == 1) && (gv < gt)) // Female
                    break;  // PID Passes
                else if ((cg == 0) && (gv >= gt))
                    break;  // PID Passes
                pid = (uint)rnd32();
            }
            return pid;
        }
        private int getChosenGender()
        {
            int cg;
            if (Label_Gender.Text == "♂")
                cg = 0;
            else if (Label_Gender.Text == "♀")
                cg = 1;
            else cg = 2;
            return cg;
        }
        // Secondary Windows for Ribbons/Amie/Memories
        private void openribbons(object sender, EventArgs e)
        {
            PKHeX.RibbMedal f3 = new PKHeX.RibbMedal(this);
            f3.ShowDialog();
        }
        private void openhistory(object sender, EventArgs e)
        {
            PKHeX.MemoryAmie f4 = new PKHeX.MemoryAmie(this);
            f4.ShowDialog();
        }
        // Open/Save Array Manipulation // 
        private byte[] shuffleArray(byte[] pkx, uint sv)
        {
            byte[] ekx = new Byte[260];
            Array.Copy(pkx, ekx, 8);

            // Now to shuffle the blocks

            // Define Shuffle Order Structure
            var aloc = new byte[] { 0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3 };
            var bloc = new byte[] { 1, 1, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2 };
            var cloc = new byte[] { 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 2, 3, 2, 1, 1 };
            var dloc = new byte[] { 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 0, 0, 0, 0, 0, 0 };

            // Get Shuffle Order
            var shlog = new byte[] { aloc[sv], bloc[sv], cloc[sv], dloc[sv] };

            // UnShuffle Away!
            for (int b = 0; b < 4; b++)
            {
                Array.Copy(pkx, 8 + 56 * shlog[b], ekx, 8 + 56 * b, 56);
            }

            // Fill the Battle Stats back
            if (pkx.Length > 232)
            {
                Array.Copy(pkx, 232, ekx, 232, 28);
            }
            return ekx;
        }
        private byte[] decryptArray(byte[] ekx)
        {
            byte[] pkx = ekx;
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (((pv & 0x3E000) >> 0xD) % 24);

            uint seed = pv;

            // Decrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
            {
                int pre = pkx[i] + ((pkx[i + 1]) << 8);
                seed = LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                pkx[i] = (byte)((post) & 0xFF);
                pkx[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Deshuffle
            pkx = shuffleArray(pkx, sv);

            // Decrypt the Party Stats
            seed = pv;
            for (int i = 232; i < 260; i += 2)
            {
                int pre = pkx[i] + ((pkx[i + 1]) << 8);
                seed = LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                pkx[i] = (byte)((post) & 0xFF);
                pkx[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            return pkx;
        }
        private byte[] encryptArray(byte[] pkx)
        {
            // Shuffle
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (((pv & 0x3E000) >> 0xD) % 24);

            byte[] ekxdata = new Byte[pkx.Length];
            Array.Copy(pkx, ekxdata, pkx.Length);

            // If I unshuffle 11 times, the 12th (decryption) will always decrypt to ABCD.
            // 2 x 3 x 4 = 12 (possible unshuffle loops -> total iterations)
            for (int i = 0; i < 11; i++)
            {
                ekxdata = shuffleArray(ekxdata, sv);
            }

            uint seed = pv;
            // Encrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
            {
                int pre = ekxdata[i] + ((ekxdata[i + 1]) << 8);
                seed = LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                ekxdata[i] = (byte)((post) & 0xFF);
                ekxdata[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Encrypt the Party Stats
            seed = pv;
            for (int i = 232; i < 260; i += 2)
            {
                int pre = ekxdata[i] + ((ekxdata[i + 1]) << 8);
                seed = LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                ekxdata[i] = (byte)((post) & 0xFF);
                ekxdata[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Done
            return ekxdata;
        }
        private byte[] convertPK5toPK6(byte[] pkm)
        {
            // To transfer, we will go down the pkm offset list and fill it into the PKX list.
            byte[] pkx = new Byte[232]; // Setup new array to store the new PKX

            // Upon transfer, the PID is also set as the Encryption Key.
            // Copy intro data, it's the same (Encrypt Key -> EXP)
            for (int i = 0; i < 0x14; i++) { pkx[i] = pkm[i]; }
            pkx[0xA] = 0; pkx[0xB] = 0;     // Get rid of the item, those aren't transferred.
            // Set the PID in its new location as well...
            for (int i = 0; i < 0x4; i++) { pkx[0x18 + i] = pkm[i]; }
            pkx[0xCA] = pkm[0x14]; // Friendship
            pkx[0x14] = pkm[0x15]; // Ability
            // Get Ability Number from the PID (the actual valid one)
            if ((pkm[0x42] & 1) == 1)   // Hidden Ability Flag
                pkx[0x15] = 4;
            else if (pkm[0x5F] < 0x10)  // Gen 3-4 Origin Method
                pkx[0x15] = (byte)(pkx[0x0] & 1); // Old Ability Correlation
            else
                pkx[0x15] = (byte)(pkx[0x2] & 1); // Gen5 Correlation
            pkx[0x2A] = pkm[0x16];  // Markings
            pkx[0xE3] = pkm[0x17];  // OT Language

            // Copy EVs and Contest Stats
            for (int i = 0; i < 12; i++)
                pkx[0x1E + i] = pkm[0x18 + i];
            // Fix EVs (<=252)
            for (int i = 0; i < 6; i++)
                if (pkx[0x1E + i] > 252)
                    pkx[0x1E + i] = 252;

            // Copy Moves
            for (int i = 0; i < 16; i++)
                pkx[0x5A + i] = pkm[0x28 + i];
            // Fix PP; some moves have different PP in Gen 6.
            pkx[0x62] = (byte)(getMovePP(BitConverter.ToInt16(pkx, 0x5A)) * (5 + pkx[0x66]) / 5);
            pkx[0x63] = (byte)(getMovePP(BitConverter.ToInt16(pkx, 0x5C)) * (5 + pkx[0x67]) / 5);
            pkx[0x64] = (byte)(getMovePP(BitConverter.ToInt16(pkx, 0x5E)) * (5 + pkx[0x68]) / 5);
            pkx[0x65] = (byte)(getMovePP(BitConverter.ToInt16(pkx, 0x60)) * (5 + pkx[0x69]) / 5);

            // Copy 32bit IV value.
            for (int i = 0; i < 4; i++)
                pkx[0x74 + i] = pkm[0x38 + i];

            pkx[0x1D] = pkm[0x40];  // Copy FE & Gender Flags
            pkx[0x1C] = pkm[0x41];  // Copy Nature

            // Copy Nickname
            string nicknamestr = "";
            for (int i = 0; i < 24; i += 2)
            {
                if ((pkm[0x48 + i] == 0xFF) && pkm[0x48 + i + 1] == 0xFF)
                {   // If given character is a terminator, stop copying. There are no trash bytes or terminators in Gen 6!
                    break;
                }
                nicknamestr += (char)(pkm[0x48 + i] + pkm[0x49 + i] * 0x100);
            }
            // Decapitalize Logic
            if ((nicknamestr.Length > 0) && (pkx[0x77] >> 7 == 0))
                nicknamestr = char.ToUpper(nicknamestr[0]) + nicknamestr.Substring(1).ToLower();
            byte[] nkb = Encoding.Unicode.GetBytes(nicknamestr);
            Array.Resize(ref nkb, 24);
            Array.Copy(nkb, 0, pkx, 0x40, nkb.Length);

            pkx[0xDF] = pkm[0x5F];  // Copy Origin Game

            // Copy OT
            for (int i = 0; i < 24; i += 2)
            {
                if ((pkm[0x68 + i] == 0xFF) && pkm[0x68 + i + 1] == 0xFF)
                {   // If terminated, stop
                    break;
                }
                pkx[0xB0 + i] = pkm[0x68 + i];
                pkx[0xB0 + i + 1] = pkm[0x68 + i + 1];
            }
            // Copy Met Info
            for (int i = 0; i < 0x6; i++)
            {   // Dates are kept upon transfer
                pkx[0xD1 + i] = pkm[0x78 + i];
            }
            // pkx[0xD7] has a gap.
            for (int i = 0; i < 0x4; i++)
            {   // Locations are kept upon transfer
                pkx[0xD8 + i] = pkm[0x7E + i];
            }
            pkx[0x2B] = pkm[0x82];  // Pokerus
            pkx[0xDC] = pkm[0x83];  // Ball

            // Get the current level of the specimen to be transferred
            int species = BitConverter.ToInt16(pkx, 0x08);
            int exp = BitConverter.ToInt32(pkx, 0x10);
            int currentlevel = getLevel((species), (exp));

            pkx[0xDD] = (byte)(((pkm[0x84]) & 0x80) + currentlevel);  // OT Gender & Encounter Level
            pkx[0xDE] = pkm[0x85];  // Encounter Type

            // Ribbon Decomposer (Contest & Battle)
            byte contestribbons = 0;
            byte battleribbons = 0;

            // Contest Ribbon Counter
            for (int i = 0; i < 8; i++) // Sinnoh 3, Hoenn 1
            {
                if (((pkm[0x60] >> i) & 1) == 1)
                    contestribbons++;
                if (((pkm[0x61] >> i) & 1) == 1)
                    contestribbons++;
                if (((pkm[0x3C] >> i) & 1) == 1)
                    contestribbons++;
                if (((pkm[0x3D] >> i) & 1) == 1)
                    contestribbons++;
            }
            for (int i = 0; i < 4; i++) // Sinnoh 4, Hoenn 2
            {
                if (((pkm[0x62] >> i) & 1) == 1)
                    contestribbons++;
                if (((pkm[0x3E] >> i) & 1) == 1)
                    contestribbons++;
            }

            // Battle Ribbon Counter
            if ((pkm[0x3E] & 0x20) >> 5 == 1)    // Winning Ribbon
                battleribbons++;
            if ((pkm[0x3E] & 0x40) >> 6 == 1)    // Victory Ribbon
                battleribbons++;
            for (int i = 1; i < 7; i++)     // Sinnoh Battle Ribbons
            {
                if (((pkm[0x24] >> i) & 1) == 1)
                    battleribbons++;
            }
            // Fill the Ribbon Counter Bytes
            pkx[0x38] = contestribbons;
            pkx[0x39] = battleribbons;

            // Copy Ribbons to their new locations.
            int bx30 = 0;
            // bx30 += 0;                           // //Kalos Champ    - New Ribbon
            bx30 += (((pkm[0x3E] & 0x10) >> 4) << 1); // Hoenn Champion
            bx30 += (((pkm[0x24] & 0x01) >> 0) << 2); // Sinnoh Champ
            // bx30 += 0;                           // //Best Friend    - New Ribbon
            // bx30 += 0;                           // //Training       - New Ribbon
            // bx30 += 0;                           // //Skillful       - New Ribbon
            // bx30 += 0;                           // //Expert         - New Ribbon
            bx30 += (((pkm[0x3F] & 0x01) >> 0) << 7); // Effort Ribbon
            pkx[0x30] = (byte)bx30;

            int bx31 = 0;
            bx31 += (((pkm[0x24] & 0x80) >> 7) << 0);  // Alert
            bx31 += (((pkm[0x25] & 0x01) >> 0) << 1);  // Shock
            bx31 += (((pkm[0x25] & 0x02) >> 1) << 2);  // Downcast
            bx31 += (((pkm[0x25] & 0x04) >> 2) << 3);  // Careless
            bx31 += (((pkm[0x25] & 0x08) >> 3) << 4);  // Relax
            bx31 += (((pkm[0x25] & 0x10) >> 4) << 5);  // Snooze
            bx31 += (((pkm[0x25] & 0x20) >> 5) << 6);  // Smile
            bx31 += (((pkm[0x25] & 0x40) >> 6) << 7);  // Gorgeous
            pkx[0x31] = (byte)bx31;

            int bx32 = 0;
            bx32 += (((pkm[0x25] & 0x80) >> 7) << 0);  // Royal
            bx32 += (((pkm[0x26] & 0x01) >> 0) << 1);  // Gorgeous Royal
            bx32 += (((pkm[0x3E] & 0x80) >> 7) << 2);  // Artist
            bx32 += (((pkm[0x26] & 0x02) >> 1) << 3);  // Footprint
            bx32 += (((pkm[0x26] & 0x04) >> 2) << 4);  // Record
            bx32 += (((pkm[0x26] & 0x10) >> 4) << 5);  // Legend
            bx32 += (((pkm[0x3F] & 0x10) >> 4) << 6);  // Country
            bx32 += (((pkm[0x3F] & 0x20) >> 5) << 7);  // National
            pkx[0x32] = (byte)bx32;

            int bx33 = 0;
            bx33 += (((pkm[0x3F] & 0x40) >> 6) << 0);  // Earth
            bx33 += (((pkm[0x3F] & 0x80) >> 7) << 1);  // World
            bx33 += (((pkm[0x27] & 0x04) >> 2) << 2);  // Classic
            bx33 += (((pkm[0x27] & 0x08) >> 3) << 3);  // Premier
            bx33 += (((pkm[0x26] & 0x08) >> 3) << 4);  // Event
            bx33 += (((pkm[0x26] & 0x40) >> 6) << 5);  // Birthday
            bx33 += (((pkm[0x26] & 0x80) >> 7) << 6);  // Special
            bx33 += (((pkm[0x27] & 0x01) >> 0) << 7);  // Souvenir
            pkx[0x33] = (byte)bx33;

            int bx34 = 0;
            bx34 += (((pkm[0x27] & 0x02) >> 1) << 0);  // Wishing Ribbon
            bx34 += (((pkm[0x3F] & 0x02) >> 1) << 1);  // Battle Champion
            bx34 += (((pkm[0x3F] & 0x04) >> 2) << 2);  // Regional Champion
            bx34 += (((pkm[0x3F] & 0x08) >> 3) << 3);  // National Champion
            bx34 += (((pkm[0x26] & 0x20) >> 5) << 4);  // World Champion
            pkx[0x34] = (byte)bx34;

            // 
            // Extra Modifications:
            // Write the Memories, Friendship, and Origin!
            //

            // Write latest notOT handler as PKHeX
            byte[] newOT = Encoding.Unicode.GetBytes("PKHeX");
            Array.Resize(ref newOT, 24);
            Array.Copy(newOT, 0, pkx, 0x78, newOT.Length);

            // Write Memories as if it was Transferred: USA|California
            // 01 - Not handled by OT
            // 07 - CA
            // 31 - USA
            byte[] x90x = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x07, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, (byte)getBaseFriendship(species), 0x00, 0x01, 0x04, (byte)(rnd32() % 10), 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Array.Copy(x90x, 0, pkx, 0x90, x90x.Length);

            // When transferred, friendship gets reset.
            pkx[0xCA] = (byte)getBaseFriendship(species);

            // Write Origin (USA California) - location is dependent on 3DS system that transfers.
            pkx[0xE0] = 0x31;   // USA
            pkx[0xE1] = 0x07;   // CA
            pkx[0xE2] = 0x01;   // ENG

            // Fix Checksum
            uint chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
            {
                chk += (uint)(pkx[i] + pkx[i + 1] * 0x100);
            }

            // Apply New Checksum
            Array.Copy(BitConverter.GetBytes(chk), 0, pkx, 06, 2);

            return pkx; // Done!
        }
        private bool verifiedpkx()
        {
            // Make sure the PKX Fields are filled out properly (color check)

            ComboBox[] cba = {
                                 CB_Species, CB_Nature, CB_HeldItem, CB_Ability, CB_Country, CB_SubRegion, // Main Tab
                                 CB_MetLocation, CB_EggLocation, CB_Ball,   // Met Tab
                                 CB_Move1, CB_Move2, CB_Move3, CB_Move4,    // Moves
                                 CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4
                             };

            for (int i = 0; i < cba.Length; i++)
            {
                if (cba[i].BackColor != defaultwhite)
                {
                    //MessageBox.Show("The PKX Data has errors. Please fix them.", "Error");
                    System.Media.SystemSounds.Exclamation.Play();

                    if (i < 6) // Main Tab
                    {
                        tabMain.SelectedIndex = 0;
                    }
                    else if (i < 9) // Met Tab
                    {
                        tabMain.SelectedIndex = 1;
                    }
                    else // Moves
                    {
                        tabMain.SelectedIndex = 3;
                    }
                    return false;
                }
            }
            // Further logic checking
            if (Convert.ToUInt32(TB_EVTotal.Text) > 510)
            {
                tabMain.SelectedIndex = 2;
            }
            return true;
        }
        private byte[] preparepkx(byte[] buff)
        {
            // Stuff the Buff 
            // Create a new storage so we don't muck up things with the original
            byte[] pkx = buff;

            // Repopulate PKX with Edited Stuff
            Array.Copy(BitConverter.GetBytes(getHEXval(TB_EC)), 0, pkx, 0, 4);  // EK

            Array.Copy(BitConverter.GetBytes(0), 0, pkx, 0x4, 4);  // 0 CHK for now

            //
            // Block A
            //
            Array.Copy(BitConverter.GetBytes(getSpecies()), 0, pkx, 0x08, 2);           // Species
            Array.Copy(BitConverter.GetBytes(getIndex(CB_HeldItem)), 0, pkx, 0x0A, 2);  // Held Item
            Array.Copy(BitConverter.GetBytes(ToUInt32(TB_TID.Text)), 0, pkx, 0x0C, 2);   // TID
            Array.Copy(BitConverter.GetBytes(ToUInt32(TB_SID.Text)), 0, pkx, 0x0E, 2);   // SID
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt32(TB_EXP.Text)), 0, pkx, 0x10, 4);  // EXP
            pkx[0x14] = (byte)getAbility();            // Ability
            pkx[0x15] = (byte)(ToInt32((TB_AbilityNumber.Text)));   // Number
            // pkx[0x16], pkx[0x17]
            Array.Copy(BitConverter.GetBytes(getHEXval(TB_PID)), 0, pkx, 0x18, 4);  // PID
            pkx[0x1C] = (byte)((getIndex(CB_Nature)));              // Nature
            int fegform = (int)(Convert.ToInt32(CHK_Fateful.Checked));
            fegform += (Convert.ToInt32(Label_Gender.Text == "♀") * 2);
            fegform += (Convert.ToInt32(Label_Gender.Text == "-") * 4);
            fegform += ((getIndex(CB_Form)) * 8);
            pkx[0x1D] = (byte)fegform;
            pkx[0x1E] = (byte)(ToInt32(TB_HPEV.Text) & 0xFF);       // EVs
            pkx[0x1F] = (byte)(ToInt32(TB_ATKEV.Text) & 0xFF);
            pkx[0x20] = (byte)(ToInt32(TB_DEFEV.Text) & 0xFF);
            pkx[0x21] = (byte)(ToInt32(TB_SPEEV.Text) & 0xFF);
            pkx[0x22] = (byte)(ToInt32(TB_SPAEV.Text) & 0xFF);
            pkx[0x23] = (byte)(ToInt32(TB_SPDEV.Text) & 0xFF);

            pkx[0x24] = (byte)(ToInt32(TB_Cool.Text) & 0xFF);       // CNT
            pkx[0x25] = (byte)(ToInt32(TB_Beauty.Text) & 0xFF);
            pkx[0x26] = (byte)(ToInt32(TB_Cute.Text) & 0xFF);
            pkx[0x27] = (byte)(ToInt32(TB_Smart.Text) & 0xFF);
            pkx[0x28] = (byte)(ToInt32(TB_Tough.Text) & 0xFF);
            pkx[0x29] = (byte)(ToInt32(TB_Sheen.Text) & 0xFF);
            // stupid & 0xFF to prevent bad values being stuffed
            int markings = Convert.ToInt32(CHK_Circle.Checked);
            markings += Convert.ToInt32(CHK_Triangle.Checked) * 2;
            markings += Convert.ToInt32(CHK_Square.Checked) * 4;
            markings += Convert.ToInt32(CHK_Heart.Checked) * 8;
            markings += Convert.ToInt32(CHK_Star.Checked) * 16;
            markings += Convert.ToInt32(CHK_Diamond.Checked) * 32;
            pkx[0x2A] = (byte)markings;
            pkx[0x2B] = (byte)(getIndex(CB_PKRSDays) + getIndex(CB_PKRSStrain) * 0x10);

            // Already in buff (then transferred to new pkx)
            // 0x2C, 0x2D, 0x2E, 0x2F
            // 0x30, 0x31, 0x32, 0x33
            // 0x34, 0x35, 0x36, 0x37
            // 0x38, 0x39

            // Unused
            // 0x3A, 0x3B
            // 0x3C, 0x3D, 0x3E, 0x3F

            // 
            // Block B
            //

            // Convert Nickname field back to bytes
            string nicknamestr = TB_Nickname.Text;

            if (!CHK_Nicknamed.Checked) // get the correct private use gender sign
            {
                if (nicknamestr.Contains((char)0x2640))
                {
                    nicknamestr = Regex.Replace(nicknamestr, "\u2640", "\uE08F");
                }
                else if (nicknamestr.Contains((char)0x2642))
                {
                    nicknamestr = Regex.Replace(nicknamestr, "\u2642", "\uE08E");
                }
            }
            byte[] nicknamebytes = Encoding.Unicode.GetBytes(nicknamestr);
            Array.Resize(ref nicknamebytes, 24);
            Array.Copy(nicknamebytes, 0, pkx, 0x40, nicknamebytes.Length);

            // 0x58, 0x59
            Array.Copy(BitConverter.GetBytes(getIndex(CB_Move1)), 0, pkx, 0x5A, 2);  // Move 1
            Array.Copy(BitConverter.GetBytes(getIndex(CB_Move2)), 0, pkx, 0x5C, 2);  // Move 2
            Array.Copy(BitConverter.GetBytes(getIndex(CB_Move3)), 0, pkx, 0x5E, 2);  // Move 3
            Array.Copy(BitConverter.GetBytes(getIndex(CB_Move4)), 0, pkx, 0x60, 2);  // Move 4

            pkx[0x62] = (byte)(ToInt32(TB_PP1.Text) & 0xFF);    // Max PP
            pkx[0x63] = (byte)(ToInt32(TB_PP2.Text) & 0xFF);
            pkx[0x64] = (byte)(ToInt32(TB_PP3.Text) & 0xFF);
            pkx[0x65] = (byte)(ToInt32(TB_PP4.Text) & 0xFF);

            pkx[0x66] = (byte)(CB_PPu1.SelectedIndex);              // PP Ups
            pkx[0x67] = (byte)(CB_PPu2.SelectedIndex);
            pkx[0x68] = (byte)(CB_PPu3.SelectedIndex);
            pkx[0x69] = (byte)(CB_PPu4.SelectedIndex);

            Array.Copy(BitConverter.GetBytes(getIndex(CB_RelearnMove1)), 0, pkx, 0x6A, 2);  // EggMove 1
            Array.Copy(BitConverter.GetBytes(getIndex(CB_RelearnMove2)), 0, pkx, 0x6C, 2);  // EggMove 2
            Array.Copy(BitConverter.GetBytes(getIndex(CB_RelearnMove3)), 0, pkx, 0x6E, 2);  // EggMove 3
            Array.Copy(BitConverter.GetBytes(getIndex(CB_RelearnMove4)), 0, pkx, 0x70, 2);  // EggMove 4

            // 0x72 - Ribbon editor sets this flag (Secret Super Training)
            // 0x73

            uint IV32 = ToUInt32(TB_HPIV.Text) & 0x1F;
            IV32 += ((ToUInt32(TB_ATKIV.Text) & 0x1F) << 5);
            IV32 += ((ToUInt32(TB_DEFIV.Text) & 0x1F) << 10);
            IV32 += ((ToUInt32(TB_SPEIV.Text) & 0x1F) << 15);
            IV32 += ((ToUInt32(TB_SPAIV.Text) & 0x1F) << 20);
            IV32 += ((ToUInt32(TB_SPDIV.Text) & 0x1F) << 25);
            IV32 += (Convert.ToUInt32(CHK_IsEgg.Checked) << 30);
            IV32 += (Convert.ToUInt32(CHK_Nicknamed.Checked) << 31);

            pkx[0x74] = (byte)((IV32 >> 00) & 0xFF); // IVs
            pkx[0x75] = (byte)((IV32 >> 08) & 0xFF);
            pkx[0x76] = (byte)((IV32 >> 16) & 0xFF);
            pkx[0x77] = (byte)((IV32 >> 24) & 0xFF);

            // 
            // Block C
            // 

            // Convert OTT2 field back to bytes
            byte[] OT2 = Encoding.Unicode.GetBytes(TB_OTt2.Text);
            Array.Resize(ref OT2, 24);
            Array.Copy(OT2, 0, pkx, 0x78, OT2.Length);

            //0x90-0xAF
            pkx[0x92] = Convert.ToByte(Label_CTGender.Text == "♀");
            //Plus more, set by MemoryAmie (already in buff)

            //
            // Block D
            //

            // Convert OT field back to bytes
            byte[] OT = Encoding.Unicode.GetBytes(TB_OT.Text);
            Array.Resize(ref OT, 24);
            Array.Copy(OT, 0, pkx, 0xB0, OT.Length);

            if (pkx[0x93] == 0)
            {
                pkx[0xCA] = (byte)(ToInt32(TB_Friendship.Text) & 0xFF);
            }
            else //1
            {
                pkx[0xA2] = (byte)(ToInt32(TB_Friendship.Text) & 0xFF);
            }

            int egg_year = 2000;                                   // Dates
            int egg_month = 0;
            int egg_day = 0;
            int egg_location = 0;
            if (CHK_AsEgg.Checked)      // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_year = CAL_EggDate.Value.Year;
                egg_month = CAL_EggDate.Value.Month;
                egg_day = CAL_EggDate.Value.Day;
                egg_location = getIndex(CB_EggLocation);
            }
            // Egg Met Data
            pkx[0xD1] = (byte)(egg_year - 2000);
            pkx[0xD2] = (byte)egg_month;
            pkx[0xD3] = (byte)egg_day;
            // Met Data
            pkx[0xD4] = (byte)(CAL_MetDate.Value.Year - 2000);
            pkx[0xD5] = (byte)CAL_MetDate.Value.Month;
            pkx[0xD6] = (byte)CAL_MetDate.Value.Day;

            if (CHK_IsEgg.Checked)    // If still an egg, it has no hatch location/date. Zero it!
            {
                pkx[0xD4] = 0;
                pkx[0xD5] = 0;
                pkx[0xD6] = 0;
            }

            // 0xD7 Unknown
            int met_location = getIndex(CB_MetLocation);    // Locations
            Array.Copy(BitConverter.GetBytes(egg_location), 0, pkx, 0xD8, 2);   // Egg Location
            Array.Copy(BitConverter.GetBytes(met_location), 0, pkx, 0xDA, 2);   // Met Location

            pkx[0xDC] = (byte)getIndex(CB_Ball);
            pkx[0xDD] = (byte)(((ToInt32(TB_MetLevel.Text) & 0x7F) + (Convert.ToInt32(Label_OTGender.Text == "♀") << 7)));
            pkx[0xDE] = (byte)(ToInt32(CB_EncounterType.SelectedValue.ToString()));
            pkx[0xDF] = (byte)getIndex(CB_GameOrigin);
            pkx[0xE0] = (byte)getIndex(CB_Country);
            pkx[0xE1] = (byte)getIndex(CB_SubRegion);
            pkx[0xE2] = (byte)getIndex(CB_3DSReg);
            pkx[0xE3] = (byte)getIndex(CB_Language);
            // 0xE4-0xE7

            Array.Resize(ref pkx, 260);
            // Party Stats
            pkx[0xE8] = 0; pkx[0xE9] = 0;
            pkx[0xEA] = 0; pkx[0xEB] = 0;
            pkx[0xEC] = (byte)ToInt32(TB_Level.Text);          // Level
            pkx[0xED] = 0; pkx[0xEE] = 0; pkx[0xEF] = 0;
            Array.Copy(BitConverter.GetBytes(ToInt32(Stat_HP.Text)), 0, pkx, 0xF0, 2);   // Current HP
            Array.Copy(BitConverter.GetBytes(ToInt32(Stat_HP.Text)), 0, pkx, 0xF2, 2);   // Max HP
            Array.Copy(BitConverter.GetBytes(ToInt32(Stat_ATK.Text)), 0, pkx, 0xF4, 2);  // ATK
            Array.Copy(BitConverter.GetBytes(ToInt32(Stat_DEF.Text)), 0, pkx, 0xF6, 2);  // DEF
            Array.Copy(BitConverter.GetBytes(ToInt32(Stat_SPE.Text)), 0, pkx, 0xF8, 2);  // SPE
            Array.Copy(BitConverter.GetBytes(ToInt32(Stat_SPA.Text)), 0, pkx, 0xFA, 2);  // SPA
            Array.Copy(BitConverter.GetBytes(ToInt32(Stat_SPD.Text)), 0, pkx, 0xFC, 2);  // SPD
            pkx[0xFE] = 0; pkx[0xFF] = 0;
            pkx[0x100] = 0; pkx[0x101] = 0; pkx[0x102] = 0; pkx[0x103] = 0;

            // Now we fix the checksum!
            uint chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
            {
                chk += (uint)(buff[i] + buff[i + 1] * 0x100);
            }

            // Apply New Checksum
            Array.Copy(BitConverter.GetBytes(chk), 0, pkx, 06, 2);

            // PKX is now filled
            return pkx;
        }
        // Drag & Drop Events // 
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string path = files[0]; // open first D&D

            // detect if it is a folder (load into boxes or not)
            FileAttributes attr = File.GetAttributes(path);
            bool isFolder = (attr & FileAttributes.Directory) == FileAttributes.Directory;
            if (isFolder)
            {
                loadBoxesFromDB(path);
                return;
            }

            string ext = Path.GetExtension(path);
            byte[] input = File.ReadAllBytes(path);
            try
            {
                openfile(input, path, ext);
            }
            catch
            {
                try
                {
                    byte[] blank = new Byte[260];
                    for (int i = 0; i < 232; i++)
                    {
                        blank[i] = 0;
                    }

                    blank = encryptArray(blank);
                    for (int i = 0; i < 232; i++)
                    {
                        blank[i] = (byte)(blank[i] ^ input[i]);
                    }
                    openfile(blank, path, ext);
                }
                catch
                {
                    openfile(input, path, ext);
                }
            }
        }
        // Decrypted Export
        private void dragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (!verifiedpkx()) { return; }
            {
                // Create Temp File to Drag
                string basepath = System.Windows.Forms.Application.StartupPath;
                Cursor.Current = Cursors.Hand;
                // Make a new file name based off the PID
                string filename = TB_Nickname.Text + " - " + TB_PID.Text + ".pk6";
                byte[] dragdata = preparepkx(buff);
                // Strip out party stats (if they are there)
                Array.Resize(ref dragdata, 232);
                // Make File
                string newfile = basepath + "\\" + CleanFileName(filename);
                try
                {
                    File.WriteAllBytes(newfile, dragdata);

                    string[] filesToDrag = { newfile };
                    dragout.DoDragDrop(new DataObject(DataFormats.FileDrop, filesToDrag), DragDropEffects.Move);
                    File.Delete(newfile);
                }
                catch (ArgumentException x)
                {
                    MessageBox.Show("Drag&Drop Error\r\n" + x, "Error");
                }
                File.Delete(newfile);
            }
        }
        private void dragout_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        // Encrypted Export
        private void eragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (!verifiedpkx()) { return; }
            {
                // Create Temp File to Drag
                string basepath = System.Windows.Forms.Application.StartupPath;
                Cursor.Current = Cursors.Hand;

                // Make a new file name based off the PID
                string filename = TB_Nickname.Text + " - " + TB_PID.Text + ".ek6"; 
                byte[] dragdata = encryptArray(preparepkx(buff));
                // Strip out party stats (if they are there)
                Array.Resize(ref dragdata, 232);
                // Make file
                string newfile = basepath + "\\" + CleanFileName(filename);
                try
                {
                    File.WriteAllBytes(newfile, dragdata);

                    string[] filesToDrag = { newfile };
                    dragout.DoDragDrop(new DataObject(DataFormats.FileDrop, filesToDrag), DragDropEffects.Move);
                    File.Delete(newfile);
                }
                catch (ArgumentException x)
                {
                    MessageBox.Show("Drag&Drop Error\r\n" + x, "Error");
                }
                File.Delete(newfile);
            }
        }
        private void eragout_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        // Dragout Hover Display
        private void dragoutHover(object sender, EventArgs e)
        {
            eragout.BackColor = dragout.BackColor = Color.LightGray;
        }
        private void dragoutLeave(object sender, EventArgs e)
        {
            eragout.BackColor = Color.Transparent;
            dragout.BackColor = Color.Transparent;
        }
        #endregion

        #region //// SAVE FILE FUNCTIONS ////
        private int detectSAVIndex(byte[] data)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            RTB_S.Text = "";
            {
                byte[] difihash1 = new Byte[0x12C];
                byte[] difihash2 = new Byte[0x12C];
                Array.Copy(data, 0x330, difihash1, 0, 0x12C);
                Array.Copy(data, 0x200, difihash2, 0, 0x12C);
                byte[] hashValue1 = mySHA256.ComputeHash(difihash1);
                byte[] hashValue2 = mySHA256.ComputeHash(difihash2);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(data, 0x16C, actualhash, 0, 0x20);
                if (hashValue1.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Active DIFI 2 - Save 1.";
                    savindex = 0;
                }
                else if (hashValue2.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Active DIFI 1 - Save 2.";
                    savindex = 1;
                }
                else
                {
                    RTB_S.Text += "ERROR: NO ACTIVE DIFI HASH MATCH";
                    savindex = 2;
                }
            }
            if ((data[0x168]^1) != savindex && savindex != 2)
            {
                RTB_S.Text += "\r\nERROR: ACTIVE BLOCK MISMATCH";
                savindex = 2;
            }
            return savindex;
        }
        private UInt16 ccitt16(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            return crc;
        }
        // Integrity Checks // 
        private void B_VerifyCHK_Click(object sender, EventArgs e)
        {
            if (savedited)
            {
                RTB_S.Text = "Save has been edited.\r\nCannot integrity check.";
                return;
            }
            // Verify Checksums
            RTB_S.Text = "";
            int invalid1 = 0;
            uint[] start =  {
                               0x05400,	0x05800,	0x06400,	0x06600,	0x06800,	0x06A00,	0x06C00,	0x06E00,	0x07000,	0x07200,	0x07400,	0x09600,	0x09800,	0x09E00,	0x0A400,	0x0F400,	0x14400,	0x19400,	0x19600,	0x19E00,	0x1A400,	0x1AC00,	0x1B400,	0x1B600,	0x1B800,	0x1BE00,	0x1C000,	0x1C400,	0x1CC00,	0x1CE00,	0x1D000,	0x1D200,	0x1D400,	0x1D600,	0x1DE00,	0x1E400,	0x1E800,	0x20400,	0x20600,	0x20800,	0x20C00,	0x21000,	0x22C00,	0x23000,	0x23800,	0x23C00,	0x24600,	0x24A00,	0x25200,	0x26000,	0x26200,	0x26400,	0x27200,	0x27A00,	0x5C600,
                            };
            uint[] length = {
                                0x000002C8,	0x00000B88,	0x0000002C,	0x00000038,	0x00000150,	0x00000004,	0x00000008,	0x000001C0,	0x000000BE,	0x00000024,	0x00002100,	0x00000140,	0x00000440,	0x00000574,	0x00004E28,	0x00004E28,	0x00004E28,	0x00000170,	0x0000061C,	0x00000504,	0x000006A0,	0x00000644,	0x00000104,	0x00000004,	0x00000420,	0x00000064,	0x000003F0,	0x0000070C,	0x00000180,	0x00000004,	0x0000000C,	0x00000048,	0x00000054,	0x00000644,	0x000005C8,	0x000002F8,	0x00001B40,	0x000001F4,	0x000001F0,	0x00000216,	0x00000390,	0x00001A90,	0x00000308,	0x00000618,	0x0000025C,	0x00000834,	0x00000318,	0x000007D0,	0x00000C48,	0x00000078,	0x00000200,	0x00000C84,	0x00000628,	0x00034AD0,	0x0000E058,
                            };

            int csoff = 0x6A81A;

            for (int i = 0; i < length.Length; i++)
            {

                byte[] data = new Byte[length[i]];
                Array.Copy(savefile, start[i], data, 0, length[i]);
                ushort checksum = ccitt16(data);
                ushort actualsum = (ushort)(savefile[csoff + i * 0x8] + savefile[csoff + i * 0x8 + 1] * 0x100);
                if (checksum != actualsum)
                {
                    invalid1++;
                    RTB_S.Text += "Invalid: " + i.ToString("X2") + " @ region " + start[i].ToString("X5") + "\r\n";
                }
            }
            RTB_S.Text += "1st SAV: " + (0x37 - invalid1).ToString() + "/" + 0x37.ToString() + "\r\n";

            // Do it again, but for the second SAV.
            csoff += 0x7F000;
            for (int i = 0; i < start.Length; i++)
            {
                start[i] += 0x7F000;
            }
            int invalid2 = 0;
            for (int i = 0; i < length.Length; i++)
            {

                byte[] data = new Byte[length[i]];
                Array.Copy(savefile, start[i], data, 0, length[i]);
                ushort checksum = ccitt16(data);
                ushort actualsum = (ushort)(savefile[csoff + i * 0x8] + savefile[csoff + i * 0x8 + 1] * 0x100);
                if (checksum != actualsum)
                {
                    invalid2++;
                    RTB_S.Text += "Invalid: " + i.ToString("X2") + " @ region " + start[i].ToString("X5") + "\r\n";
                }
            }
            RTB_S.Text += "2nd SAV: " + (0x37 - invalid2).ToString() + "/" + 0x37.ToString() + "\r\n";
            if (invalid1 + invalid2 == (0x37 * 2))
            {
                RTB_S.Text = "No checksums are valid.";
            }
        }
        private void B_VerifySHA_Click(object sender, EventArgs e)
        {
            if (savedited)
            {
                RTB_S.Text = "Save has been edited.\r\nCannot integrity check.";
                return;
            }
            // Verify Hashes
            RTB_S.Text = "";
            int invalid1 = 0;

            uint[] hashtabledata = {
                                    0x2020,	    0x203F,	    0x2000,	0x200,
                                    0x2040,	    0x2FFF,	    0x2020,	0x1000,
                                    0x3000,	    0x3FFF,	    0x2040,	0x1000,
                                    0x4000,	    0x4FFF,	    0x2060,	0x1000,
                                    0x5000,	    0x5FFF,	    0x2080,	0x1000,
                                    0x6000,	    0x6FFF,	    0x20A0,	0x1000,
                                    0x7000,	    0x7FFF,	    0x20C0,	0x1000,
                                    0x8000,	    0x8FFF,	    0x20E0,	0x1000,
                                    0x9000,	    0x9FFF,	    0x2100,	0x1000,
                                    0xA000,	    0xAFFF,	    0x2120,	0x1000,
                                    0xB000,	    0xBFFF,	    0x2140,	0x1000,
                                    0xC000,	    0xCFFF,	    0x2160,	0x1000,
                                    0xD000,	    0xDFFF,	    0x2180,	0x1000,
                                    0xE000,	    0xEFFF,	    0x21A0,	0x1000,
                                    0xF000,	    0xFFFF,	    0x21C0,	0x1000,
                                    0x10000,	0x10FFF,	0x21E0,	0x1000,
                                    0x11000,	0x11FFF,	0x2200,	0x1000,
                                    0x12000,	0x12FFF,	0x2220,	0x1000,
                                    0x13000,	0x13FFF,	0x2240,	0x1000,
                                    0x14000,	0x14FFF,	0x2260,	0x1000,
                                    0x15000,	0x15FFF,	0x2280,	0x1000,
                                    0x16000,	0x16FFF,	0x22A0,	0x1000,
                                    0x17000,	0x17FFF,	0x22C0,	0x1000,
                                    0x18000,	0x18FFF,	0x22E0,	0x1000,
                                    0x19000,	0x19FFF,	0x2300,	0x1000,
                                    0x1A000,	0x1AFFF,	0x2320,	0x1000,
                                    0x1B000,	0x1BFFF,	0x2340,	0x1000,
                                    0x1C000,	0x1CFFF,	0x2360,	0x1000,
                                    0x1D000,	0x1DFFF,	0x2380,	0x1000,
                                    0x1E000,	0x1EFFF,	0x23A0,	0x1000,
                                    0x1F000,	0x1FFFF,	0x23C0,	0x1000,
                                    0x20000,	0x20FFF,	0x23E0,	0x1000,
                                    0x21000,	0x21FFF,	0x2400,	0x1000,
                                    0x22000,	0x22FFF,	0x2420,	0x1000,
                                    0x23000,	0x23FFF,	0x2440,	0x1000,
                                    0x24000,	0x24FFF,	0x2460,	0x1000,
                                    0x25000,	0x25FFF,	0x2480,	0x1000,
                                    0x26000,	0x26FFF,	0x24A0,	0x1000,
                                    0x27000,	0x27FFF,	0x24C0,	0x1000,
                                    0x28000,	0x28FFF,	0x24E0,	0x1000,
                                    0x29000,	0x29FFF,	0x2500,	0x1000,
                                    0x2A000,	0x2AFFF,	0x2520,	0x1000,
                                    0x2B000,	0x2BFFF,	0x2540,	0x1000,
                                    0x2C000,	0x2CFFF,	0x2560,	0x1000,
                                    0x2D000,	0x2DFFF,	0x2580,	0x1000,
                                    0x2E000,	0x2EFFF,	0x25A0,	0x1000,
                                    0x2F000,	0x2FFFF,	0x25C0,	0x1000,
                                    0x30000,	0x30FFF,	0x25E0,	0x1000,
                                    0x31000,	0x31FFF,	0x2600,	0x1000,
                                    0x32000,	0x32FFF,	0x2620,	0x1000,
                                    0x33000,	0x33FFF,	0x2640,	0x1000,
                                    0x34000,	0x34FFF,	0x2660,	0x1000,
                                    0x35000,	0x35FFF,	0x2680,	0x1000,
                                    0x36000,	0x36FFF,	0x26A0,	0x1000,
                                    0x37000,	0x37FFF,	0x26C0,	0x1000,
                                    0x38000,	0x38FFF,	0x26E0,	0x1000,
                                    0x39000,	0x39FFF,	0x2700,	0x1000,
                                    0x3A000,	0x3AFFF,	0x2720,	0x1000,
                                    0x3B000,	0x3BFFF,	0x2740,	0x1000,
                                    0x3C000,	0x3CFFF,	0x2760,	0x1000,
                                    0x3D000,	0x3DFFF,	0x2780,	0x1000,
                                    0x3E000,	0x3EFFF,	0x27A0,	0x1000,
                                    0x3F000,	0x3FFFF,	0x27C0,	0x1000,
                                    0x40000,	0x40FFF,	0x27E0,	0x1000,
                                    0x41000,	0x41FFF,	0x2800,	0x1000,
                                    0x42000,	0x42FFF,	0x2820,	0x1000,
                                    0x43000,	0x43FFF,	0x2840,	0x1000,
                                    0x44000,	0x44FFF,	0x2860,	0x1000,
                                    0x45000,	0x45FFF,	0x2880,	0x1000,
                                    0x46000,	0x46FFF,	0x28A0,	0x1000,
                                    0x47000,	0x47FFF,	0x28C0,	0x1000,
                                    0x48000,	0x48FFF,	0x28E0,	0x1000,
                                    0x49000,	0x49FFF,	0x2900,	0x1000,
                                    0x4A000,	0x4AFFF,	0x2920,	0x1000,
                                    0x4B000,	0x4BFFF,	0x2940,	0x1000,
                                    0x4C000,	0x4CFFF,	0x2960,	0x1000,
                                    0x4D000,	0x4DFFF,	0x2980,	0x1000,
                                    0x4E000,	0x4EFFF,	0x29A0,	0x1000,
                                    0x4F000,	0x4FFFF,	0x29C0,	0x1000,
                                    0x50000,	0x50FFF,	0x29E0,	0x1000,
                                    0x51000,	0x51FFF,	0x2A00,	0x1000,
                                    0x52000,	0x52FFF,	0x2A20,	0x1000,
                                    0x53000,	0x53FFF,	0x2A40,	0x1000,
                                    0x54000,	0x54FFF,	0x2A60,	0x1000,
                                    0x55000,	0x55FFF,	0x2A80,	0x1000,
                                    0x56000,	0x56FFF,	0x2AA0,	0x1000,
                                    0x57000,	0x57FFF,	0x2AC0,	0x1000,
                                    0x58000,	0x58FFF,	0x2AE0,	0x1000,
                                    0x59000,	0x59FFF,	0x2B00,	0x1000,
                                    0x5A000,	0x5AFFF,	0x2B20,	0x1000,
                                    0x5B000,	0x5BFFF,	0x2B40,	0x1000,
                                    0x5C000,	0x5CFFF,	0x2B60,	0x1000,
                                    0x5D000,	0x5DFFF,	0x2B80,	0x1000,
                                    0x5E000,	0x5EFFF,	0x2BA0,	0x1000,
                                    0x5F000,	0x5FFFF,	0x2BC0,	0x1000,
                                    0x60000,	0x60FFF,	0x2BE0,	0x1000,
                                    0x61000,	0x61FFF,	0x2C00,	0x1000,
                                    0x62000,	0x62FFF,	0x2C20,	0x1000,
                                    0x63000,	0x63FFF,	0x2C40,	0x1000,
                                    0x64000,	0x64FFF,	0x2C60,	0x1000,
                                    0x65000,	0x65FFF,	0x2C80,	0x1000,
                                    0x66000,	0x66FFF,	0x2CA0,	0x1000,
                                    0x67000,	0x67FFF,	0x2CC0,	0x1000,
                                    0x68000,	0x68FFF,	0x2CE0,	0x1000,
                                    0x69000,	0x69FFF,	0x2D00,	0x1000,
                                    0x6A000,	0x6AFFF,	0x2D20,	0x1000,
                                 };

            SHA256 mySHA256 = SHA256Managed.Create();

            for (int i = 0; i < hashtabledata.Length / 4; i++)
            {
                uint start = hashtabledata[0 + 4 * i];
                uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                uint offset = hashtabledata[2 + 4 * i];
                uint blocksize = hashtabledata[3 + 4 * i];

                byte[] zeroarray = new Byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, offset, actualhash, 0, 0x20);

                if (!hashValue.SequenceEqual(actualhash))
                {
                    invalid1++;
                    RTB_S.Text += "Invalid: " + hashtabledata[2 + 4 * i].ToString("X5") + " @ " + hashtabledata[0 + 4 * i].ToString("X5") + "-" + hashtabledata[1 + 4 * i].ToString("X5") + "\r\n";
                }
            }
            RTB_S.Text += "1st SAV: " + (106 - invalid1).ToString() + "/" + 106.ToString() + "\r\n";

            // Check The Second Half of Hashes
            for (int i = 0; i < hashtabledata.Length; i += 4)
            {
                hashtabledata[i + 0] += 0x7F000;
                hashtabledata[i + 1] += 0x7F000;
                hashtabledata[i + 2] += 0x7F000;
            }
            // Problem with save2 saves is that 0x3000-0x4FFF doesn't use save2 data. Probably different when hashed, but different when stored.
            for (int i = 2; i < 4; i++)
            {
                hashtabledata[i * 4 + 0] -= 0x7F000;
                hashtabledata[i * 4 + 1] -= 0x7F000;
            }
            int invalid2 = 0;

            for (int i = 0; i < hashtabledata.Length / 4; i++)
            {
                uint start = hashtabledata[0 + 4 * i];
                uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                uint offset = hashtabledata[2 + 4 * i];
                uint blocksize = hashtabledata[3 + 4 * i];

                byte[] zeroarray = new Byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, offset, actualhash, 0, 0x20);

                if (!hashValue.SequenceEqual(actualhash))
                {
                    invalid2++;
                    RTB_S.Text += "Invalid: " + hashtabledata[2 + 4 * i].ToString("X5") + " @ " + hashtabledata[0 + 4 * i].ToString("X5") + "-" + hashtabledata[1 + 4 * i].ToString("X5") + "\r\n";
                }
            }
            RTB_S.Text += "2nd SAV: " + (106 - invalid2).ToString() + "/" + 106.ToString() + "\r\n";

            if (invalid1 + invalid2 == (2 * 106))
            {
                RTB_S.Text = "None of the IVFC hashes are valid." + "\r\n";
            }

            // Check the Upper Level IVFC Hashes
            {
                byte[] zeroarray = new Byte[0x200];
                Array.Copy(savefile, 0x2000, zeroarray, 0, 0x20);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, 0x43C, actualhash, 0, 0x20);
                if (!hashValue.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Invalid: " + 0x2000.ToString("X5") + " @ " + 0x43C.ToString("X3") + "\r\n";
                }
            }
            {
                byte[] zeroarray = new Byte[0x200];
                Array.Copy(savefile, 0x81000, zeroarray, 0, 0x20);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, 0x30C, actualhash, 0, 0x20);
                if (!hashValue.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Invalid: " + 0x81000.ToString("X5") + " @ " + 0x30C.ToString("X3") + "\r\n";
                }
            }
            {
                byte[] difihash1 = new Byte[0x12C];
                byte[] difihash2 = new Byte[0x12C];
                Array.Copy(savefile, 0x330, difihash1, 0, 0x12C);
                Array.Copy(savefile, 0x200, difihash2, 0, 0x12C);
                byte[] hashValue1 = mySHA256.ComputeHash(difihash1);
                byte[] hashValue2 = mySHA256.ComputeHash(difihash2);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, 0x16C, actualhash, 0, 0x20);
                if (hashValue1.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Active DIFI partition is Save 1.";
                }
                else if (hashValue2.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Active DIFI partition is Save 2.";
                }
                else
                {
                    RTB_S.Text += "ERROR: NO ACTIVE DIFI HASH MATCH";
                }
            }
        }
        private void exportSAV(object sender, EventArgs e)
        {
            // Create another version of the save file.
            byte[] editedsav = new Byte[0x100000];
            Array.Copy(savefile, editedsav, savefile.Length);
            // Since we only edited one of the save files, we only have to fix half of the chk/hashes!

            // Fix Checksums
            {
                uint[] start =  {
                               0x05400,	0x05800,	0x06400,	0x06600,	0x06800,	0x06A00,	0x06C00,	0x06E00,	0x07000,	0x07200,	0x07400,	0x09600,	0x09800,	0x09E00,	0x0A400,	0x0F400,	0x14400,	0x19400,	0x19600,	0x19E00,	0x1A400,	0x1AC00,	0x1B400,	0x1B600,	0x1B800,	0x1BE00,	0x1C000,	0x1C400,	0x1CC00,	0x1CE00,	0x1D000,	0x1D200,	0x1D400,	0x1D600,	0x1DE00,	0x1E400,	0x1E800,	0x20400,	0x20600,	0x20800,	0x20C00,	0x21000,	0x22C00,	0x23000,	0x23800,	0x23C00,	0x24600,	0x24A00,	0x25200,	0x26000,	0x26200,	0x26400,	0x27200,	0x27A00,	0x5C600,
                            };
                uint[] length = {
                                0x000002C8,	0x00000B88,	0x0000002C,	0x00000038,	0x00000150,	0x00000004,	0x00000008,	0x000001C0,	0x000000BE,	0x00000024,	0x00002100,	0x00000140,	0x00000440,	0x00000574,	0x00004E28,	0x00004E28,	0x00004E28,	0x00000170,	0x0000061C,	0x00000504,	0x000006A0,	0x00000644,	0x00000104,	0x00000004,	0x00000420,	0x00000064,	0x000003F0,	0x0000070C,	0x00000180,	0x00000004,	0x0000000C,	0x00000048,	0x00000054,	0x00000644,	0x000005C8,	0x000002F8,	0x00001B40,	0x000001F4,	0x000001F0,	0x00000216,	0x00000390,	0x00001A90,	0x00000308,	0x00000618,	0x0000025C,	0x00000834,	0x00000318,	0x000007D0,	0x00000C48,	0x00000078,	0x00000200,	0x00000C84,	0x00000628,	0x00034AD0,	0x0000E058,
                            };

                int csoff = 0x6A81A;

                if (savindex == 1)
                {
                    csoff += 0x7F000;
                    for (int i = 0; i < start.Length; i++)
                    {
                        start[i] += 0x7F000;
                    }
                }

                for (int i = 0; i < length.Length; i++)
                {
                    byte[] data = new Byte[length[i]];
                    Array.Copy(editedsav, start[i], data, 0, length[i]);
                    ushort checksum = ccitt16(data);
                    Array.Copy(BitConverter.GetBytes(checksum), 0, editedsav, csoff + i * 8, 2);
                }
            }

            // Fix Hashes
            {
                uint[] hashtabledata = {
                                    0x2020,	    0x203F,	    0x2000,	0x200,
                                    0x2040,	    0x2FFF,	    0x2020,	0x1000,
                                    0x3000,	    0x3FFF,	    0x2040,	0x1000,
                                    0x4000,	    0x4FFF,	    0x2060,	0x1000,
                                    0x5000,	    0x5FFF,	    0x2080,	0x1000,
                                    0x6000,	    0x6FFF,	    0x20A0,	0x1000,
                                    0x7000,	    0x7FFF,	    0x20C0,	0x1000,
                                    0x8000,	    0x8FFF,	    0x20E0,	0x1000,
                                    0x9000,	    0x9FFF,	    0x2100,	0x1000,
                                    0xA000,	    0xAFFF,	    0x2120,	0x1000,
                                    0xB000,	    0xBFFF,	    0x2140,	0x1000,
                                    0xC000,	    0xCFFF,	    0x2160,	0x1000,
                                    0xD000,	    0xDFFF,	    0x2180,	0x1000,
                                    0xE000,	    0xEFFF,	    0x21A0,	0x1000,
                                    0xF000,	    0xFFFF,	    0x21C0,	0x1000,
                                    0x10000,	0x10FFF,	0x21E0,	0x1000,
                                    0x11000,	0x11FFF,	0x2200,	0x1000,
                                    0x12000,	0x12FFF,	0x2220,	0x1000,
                                    0x13000,	0x13FFF,	0x2240,	0x1000,
                                    0x14000,	0x14FFF,	0x2260,	0x1000,
                                    0x15000,	0x15FFF,	0x2280,	0x1000,
                                    0x16000,	0x16FFF,	0x22A0,	0x1000,
                                    0x17000,	0x17FFF,	0x22C0,	0x1000,
                                    0x18000,	0x18FFF,	0x22E0,	0x1000,
                                    0x19000,	0x19FFF,	0x2300,	0x1000,
                                    0x1A000,	0x1AFFF,	0x2320,	0x1000,
                                    0x1B000,	0x1BFFF,	0x2340,	0x1000,
                                    0x1C000,	0x1CFFF,	0x2360,	0x1000,
                                    0x1D000,	0x1DFFF,	0x2380,	0x1000,
                                    0x1E000,	0x1EFFF,	0x23A0,	0x1000,
                                    0x1F000,	0x1FFFF,	0x23C0,	0x1000,
                                    0x20000,	0x20FFF,	0x23E0,	0x1000,
                                    0x21000,	0x21FFF,	0x2400,	0x1000,
                                    0x22000,	0x22FFF,	0x2420,	0x1000,
                                    0x23000,	0x23FFF,	0x2440,	0x1000,
                                    0x24000,	0x24FFF,	0x2460,	0x1000,
                                    0x25000,	0x25FFF,	0x2480,	0x1000,
                                    0x26000,	0x26FFF,	0x24A0,	0x1000,
                                    0x27000,	0x27FFF,	0x24C0,	0x1000,
                                    0x28000,	0x28FFF,	0x24E0,	0x1000,
                                    0x29000,	0x29FFF,	0x2500,	0x1000,
                                    0x2A000,	0x2AFFF,	0x2520,	0x1000,
                                    0x2B000,	0x2BFFF,	0x2540,	0x1000,
                                    0x2C000,	0x2CFFF,	0x2560,	0x1000,
                                    0x2D000,	0x2DFFF,	0x2580,	0x1000,
                                    0x2E000,	0x2EFFF,	0x25A0,	0x1000,
                                    0x2F000,	0x2FFFF,	0x25C0,	0x1000,
                                    0x30000,	0x30FFF,	0x25E0,	0x1000,
                                    0x31000,	0x31FFF,	0x2600,	0x1000,
                                    0x32000,	0x32FFF,	0x2620,	0x1000,
                                    0x33000,	0x33FFF,	0x2640,	0x1000,
                                    0x34000,	0x34FFF,	0x2660,	0x1000,
                                    0x35000,	0x35FFF,	0x2680,	0x1000,
                                    0x36000,	0x36FFF,	0x26A0,	0x1000,
                                    0x37000,	0x37FFF,	0x26C0,	0x1000,
                                    0x38000,	0x38FFF,	0x26E0,	0x1000,
                                    0x39000,	0x39FFF,	0x2700,	0x1000,
                                    0x3A000,	0x3AFFF,	0x2720,	0x1000,
                                    0x3B000,	0x3BFFF,	0x2740,	0x1000,
                                    0x3C000,	0x3CFFF,	0x2760,	0x1000,
                                    0x3D000,	0x3DFFF,	0x2780,	0x1000,
                                    0x3E000,	0x3EFFF,	0x27A0,	0x1000,
                                    0x3F000,	0x3FFFF,	0x27C0,	0x1000,
                                    0x40000,	0x40FFF,	0x27E0,	0x1000,
                                    0x41000,	0x41FFF,	0x2800,	0x1000,
                                    0x42000,	0x42FFF,	0x2820,	0x1000,
                                    0x43000,	0x43FFF,	0x2840,	0x1000,
                                    0x44000,	0x44FFF,	0x2860,	0x1000,
                                    0x45000,	0x45FFF,	0x2880,	0x1000,
                                    0x46000,	0x46FFF,	0x28A0,	0x1000,
                                    0x47000,	0x47FFF,	0x28C0,	0x1000,
                                    0x48000,	0x48FFF,	0x28E0,	0x1000,
                                    0x49000,	0x49FFF,	0x2900,	0x1000,
                                    0x4A000,	0x4AFFF,	0x2920,	0x1000,
                                    0x4B000,	0x4BFFF,	0x2940,	0x1000,
                                    0x4C000,	0x4CFFF,	0x2960,	0x1000,
                                    0x4D000,	0x4DFFF,	0x2980,	0x1000,
                                    0x4E000,	0x4EFFF,	0x29A0,	0x1000,
                                    0x4F000,	0x4FFFF,	0x29C0,	0x1000,
                                    0x50000,	0x50FFF,	0x29E0,	0x1000,
                                    0x51000,	0x51FFF,	0x2A00,	0x1000,
                                    0x52000,	0x52FFF,	0x2A20,	0x1000,
                                    0x53000,	0x53FFF,	0x2A40,	0x1000,
                                    0x54000,	0x54FFF,	0x2A60,	0x1000,
                                    0x55000,	0x55FFF,	0x2A80,	0x1000,
                                    0x56000,	0x56FFF,	0x2AA0,	0x1000,
                                    0x57000,	0x57FFF,	0x2AC0,	0x1000,
                                    0x58000,	0x58FFF,	0x2AE0,	0x1000,
                                    0x59000,	0x59FFF,	0x2B00,	0x1000,
                                    0x5A000,	0x5AFFF,	0x2B20,	0x1000,
                                    0x5B000,	0x5BFFF,	0x2B40,	0x1000,
                                    0x5C000,	0x5CFFF,	0x2B60,	0x1000,
                                    0x5D000,	0x5DFFF,	0x2B80,	0x1000,
                                    0x5E000,	0x5EFFF,	0x2BA0,	0x1000,
                                    0x5F000,	0x5FFFF,	0x2BC0,	0x1000,
                                    0x60000,	0x60FFF,	0x2BE0,	0x1000,
                                    0x61000,	0x61FFF,	0x2C00,	0x1000,
                                    0x62000,	0x62FFF,	0x2C20,	0x1000,
                                    0x63000,	0x63FFF,	0x2C40,	0x1000,
                                    0x64000,	0x64FFF,	0x2C60,	0x1000,
                                    0x65000,	0x65FFF,	0x2C80,	0x1000,
                                    0x66000,	0x66FFF,	0x2CA0,	0x1000,
                                    0x67000,	0x67FFF,	0x2CC0,	0x1000,
                                    0x68000,	0x68FFF,	0x2CE0,	0x1000,
                                    0x69000,	0x69FFF,	0x2D00,	0x1000,
                                    0x6A000,	0x6AFFF,	0x2D20,	0x1000,
                                 };

                if (savindex == 1)
                {
                    for (int i = 0; i < hashtabledata.Length/4; i++)
                    {
                        hashtabledata[i * 4 + 0] += 0x7F000;
                        hashtabledata[i * 4 + 1] += 0x7F000;
                        hashtabledata[i * 4 + 2] += 0x7F000;
                    }

                    // Problem with save2 saves is that 0x3000-0x4FFF doesn't use save2 data. Probably different when hashed, but different when stored.
                    for (int i = 2; i < 4; i++)
                    {
                        hashtabledata[i * 4 + 0] -= 0x7F000;
                        hashtabledata[i * 4 + 1] -= 0x7F000;
                    }
                }

                SHA256 mySHA256 = SHA256Managed.Create();

                // Hash for 0x3000 onwards
                for (int i = 2; i < hashtabledata.Length / 4; i++)
                {
                    uint start = hashtabledata[0 + 4 * i];
                    uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                    uint offset = hashtabledata[2 + 4 * i];
                    uint blocksize = hashtabledata[3 + 4 * i];

                    byte[] zeroarray = new Byte[blocksize];
                    Array.Copy(editedsav, start, zeroarray, 0, length + 1);
                    byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                    Array.Copy(hashValue, 0, editedsav, offset, 0x20);
                }
                // Fix 2nd Hash
                {
                    uint start = hashtabledata[0 + 4 * 1];
                    uint length = hashtabledata[1 + 4 * 1] - hashtabledata[0 + 4 * 1];
                    uint offset = hashtabledata[2 + 4 * 1];
                    uint blocksize = hashtabledata[3 + 4 * 1];

                    byte[] zeroarray = new Byte[blocksize];
                    Array.Copy(editedsav, start, zeroarray, 0, length + 1);
                    byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                    Array.Copy(hashValue, 0, editedsav, offset, 0x20);
                }
                // Fix 1st Hash
                {
                    uint start = hashtabledata[0 + 4 * 0];
                    uint length = hashtabledata[1 + 4 * 0] - hashtabledata[0 + 4 * 0];
                    uint offset = hashtabledata[2 + 4 * 0];
                    uint blocksize = hashtabledata[3 + 4 * 0];

                    byte[] zeroarray = new Byte[blocksize];
                    Array.Copy(editedsav, start, zeroarray, 0, length + 1);
                    byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                    Array.Copy(hashValue, 0, editedsav, offset, 0x20);
                }
                // Fix IVFC Hash
                {
                    byte[] zeroarray = new Byte[0x200];
                    Array.Copy(editedsav, 0x2000 + savindex * 0x7F000, zeroarray, 0, 0x20);
                    byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                    Array.Copy(hashValue, 0, editedsav, 0x43C - savindex * 0x130, 0x20);
                }
                // Fix DISA Hash
                {
                    byte[] difihash = new Byte[0x12C];
                    Array.Copy(editedsav, 0x330 - savindex * 0x130, difihash, 0, 0x12C);
                    byte[] hashValue = mySHA256.ComputeHash(difihash);

                    Array.Copy(hashValue, 0, editedsav, 0x16C, 0x20);
                }
            }

            // File Integrity has been restored as well as it can. Export!
            // Write the active save index
            editedsav[0x168] = (byte)(savindex^1);
            // Save Save File
            SaveFileDialog savesav = new SaveFileDialog();
            savesav.Filter = "SAV|*.bin;*.sav";
            savesav.FileName = Regex.Split(L_Save.Text, ": ")[1];
            DialogResult result = savesav.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = savesav.FileName;
                string ext = Path.GetExtension(path);

                if (File.Exists(path))
                {
                    // File already exists, save a .bak
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(path + ".bak", backupfile);
                }

                File.WriteAllBytes(path, editedsav);
                MessageBox.Show("Saved SAV.", "Alert");
            }
        }
        // Box/SAV Functions // 
        private void B_BoxRight_Click(object sender, EventArgs e)
        {
            if (C_BoxSelect.SelectedIndex < 30)
            {
                C_BoxSelect.SelectedIndex++;
            }
        }
        private void B_BoxLeft_Click(object sender, EventArgs e)
        {
            if (C_BoxSelect.SelectedIndex > 0)
            {
                C_BoxSelect.SelectedIndex--;
            }
        }
        private void rcmView_Click(object sender, EventArgs e)
        {
            int slot = getSlot(sender);
            int offset = getPKXOffset(slot);

            PictureBox[] pba = {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                                };

            PictureBox picturebox = pba[slot];
            if (picturebox.Image == null)
            {
                return;
            }

            // Load the PKX file
            if (BitConverter.ToUInt64(savefile, offset + 8) != 0)
            {
                byte[] ekxdata = new Byte[0xE8];
                Array.Copy(savefile, offset, ekxdata, 0, 0xE8);
                byte[] pkxdata = decryptArray(ekxdata);
                int species = BitConverter.ToInt16(pkxdata, 0x08); // Get Species
                if (species == 0)
                {
                    return;
                }
                try 
                {
                    Array.Copy(pkxdata, buff, 0xE8);
                    populatefields(buff);
                }
                catch // If it fails, try XORing encrypted zeroes
                {
                    try
                    {
                        byte[] blank = encryptArray(new Byte[0xE8]);
                        for (int i = 0; i < 0xE8; i++)
                        {
                            blank[i] = (byte)(buff[i] ^ blank[i]);
                        }
                        populatefields(blank);
                    }
                    catch   // Still fails, just let the original errors occur.
                    {
                        populatefields(buff);
                    }
                }
                // Visual to display what slot is currently loaded.
                getSlotColor(slot, Color.PowderBlue);
            }
        }
        private void rcmSet_Click(object sender, EventArgs e)
        {
            if (!verifiedpkx()) { return; }
            int slot = getSlot(sender);
            int offset = SaveGame.Box + 0x7F000 * savindex + C_BoxSelect.SelectedIndex * (0xE8 * 30) + slot * 0xE8;
            byte[] pkxdata = preparepkx(buff);
            byte[] ekxdata = encryptArray(pkxdata);
            Array.Copy(ekxdata, 0, savefile, offset, 0xE8);
            getPKXBoxes();
            savedited = true;

            getSlotColor(slot, Color.Honeydew);
        }
        private void rcmDelete_Click(object sender, EventArgs e)
        {
            int slot = getSlot(sender);
            int offset = SaveGame.Box + 0x7F000 * savindex + C_BoxSelect.SelectedIndex * (0xE8 * 30) + slot * 0xE8;
            byte[] pkxdata = new Byte[0xE8];
            byte[] ekxdata = encryptArray(pkxdata);
            Array.Copy(ekxdata, 0, savefile, offset, 0xE8);
            getPKXBoxes();
            savedited = true;

            getSlotColor(slot, Color.LavenderBlush);
        }
        private void slotModifier_Click(object sender, EventArgs e)
        {
            PictureBox[] pba = {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                                };
            PictureBox pb = sender as PictureBox;
            int slot = Array.IndexOf(pba, pb);

            if (ModifierKeys == Keys.Control)
            {
                if (slot >= 0)
                {
                    int offset = getPKXOffset(slot);
                    if (pb.Image == null)
                    {
                        return;
                    }

                    if (BitConverter.ToUInt64(savefile, offset + 8) != 0)
                    {
                        byte[] ekxdata = new Byte[0xE8];
                        Array.Copy(savefile, offset, ekxdata, 0, 0xE8);
                        byte[] pkxdata = decryptArray(ekxdata);
                        int species = BitConverter.ToInt16(pkxdata, 0x08); // Get Species
                        if (species == 0)
                        {
                            return;
                        }
                        try
                        {
                            Array.Copy(pkxdata, buff, 0xE8);
                            populatefields(buff);
                        }
                        catch
                        {
                            try
                            {
                                byte[] blank = encryptArray(new Byte[0xE8]);
                                for (int i = 0; i < 0xE8; i++)
                                {
                                    blank[i] = (byte)(buff[i] ^ blank[i]);
                                }
                                populatefields(blank);
                            }
                            catch
                            {
                                populatefields(buff);
                            }
                        }

                        getSlotColor(slot, Color.PowderBlue);
                    }
                }
                return;
            }
            if (ModifierKeys == Keys.Shift)
            {
                if (!verifiedpkx()) { return; }
                if (slot >= 0 && slot < 30)
                {
                    int offset = SaveGame.Box + 0x7F000 * savindex + C_BoxSelect.SelectedIndex * (0xE8 * 30) + slot * 0xE8;
                    byte[] pkxdata = preparepkx(buff);
                    byte[] ekxdata = encryptArray(pkxdata);
                    Array.Copy(ekxdata, 0, savefile, offset, 0xE8);
                    getPKXBoxes();
                    savedited = true;
                    getSlotColor(slot, Color.Honeydew);
                }
            }
        }
        // Subfunctions // 
        private int getPKXOffset(int slot)
        {
            int offset = SaveGame.Box + C_BoxSelect.SelectedIndex * (0xE8 * 30) + slot * 0xE8;

            if (slot > 29)          // Not a party
            {
                if (slot < 36)      // Party Slot
                {
                    offset = SaveGame.Party + (slot - 30) * 0x104;
                }
                else if (slot < 42) // Battle Box Slot
                {
                    offset = SaveGame.BattleBox + (slot - 36) * 0xE8;
                }
                else if (slot < 44) // Daycare
                {
                    offset = SaveGame.Daycare + 8 + (slot - 42) * 0xF0;
                }
                else if (slot < 45) // GTS
                {
                    offset = SaveGame.GTS;
                }
                else if (slot < 46) // Fused
                {
                    offset = SaveGame.Fused;
                }
                else                // SUBE
                {
                    offset = SaveGame.SUBE + (slot - 46) * 0xEC;
                }
            }
            offset += 0x7F000 * savindex;
            return offset;
        }
        private int getSlot(object sender)
        {
            // Try to cast the sender to a ToolStripItem
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    Control sourceControl = owner.SourceControl;

                    string[] pba = {
                                    "bpkx1", "bpkx2", "bpkx3", "bpkx4", "bpkx5", "bpkx6",
                                    "bpkx7", "bpkx8", "bpkx9", "bpkx10","bpkx11","bpkx12",
                                    "bpkx13","bpkx14","bpkx15","bpkx16","bpkx17","bpkx18",
                                    "bpkx19","bpkx20","bpkx21","bpkx22","bpkx23","bpkx24",
                                    "bpkx25","bpkx26","bpkx27","bpkx28","bpkx29","bpkx30",

                                    "ppkx1", "ppkx2", "ppkx3", "ppkx4", "ppkx5", "ppkx6",
                                    "bbpkx1","bbpkx2","bbpkx3","bbpkx4","bbpkx5","bbpkx6",

                                    "dcpkx1", "dcpkx2", "gtspkx", "fusedpkx","subepkx1","subepkx2","subepkx3",
                                };
                    int slot = Array.IndexOf(pba, sourceControl.Name);
                    return slot;
                }
            }
            MessageBox.Show("Invalid slot!", "Error");
            return 0;
        }
        public void getPKXBoxes()
        {
            int boxoffset = 0x27A00 + 0x7F000 * savindex + C_BoxSelect.SelectedIndex * (0xE8 * 30);

            PictureBox[] pba = {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                                };
            for (int i = 0; i < 30; i++)
            {
                int offset = boxoffset + 0xE8 * i;
                getSlotFiller(offset, pba[i]);
            }

            // Reload Party
            for (int i = 0; i < 6; i++)
            {
                int offset = 0x19600 + (0x7F000 * savindex) + 0x104 * i;
                getSlotFiller(offset, pba[i + 30]);
            }
            // Reload Battle Box
            for (int i = 0; i < 6; i++)
            {
                int offset = 0x09E00 + (0x7F000 * savindex) + 0xE8 * i;
                getSlotFiller(offset, pba[i + 36]);
            }
            // Reload Daycare
            Label[] dclabela = { 
                                   L_DC1,
                                   L_DC2,
                               };
            TextBox[] dctexta = {
                                    TB_Daycare1XP,
                                    TB_Daycare2XP,
                                };
            for (int i = 0; i < 2; i++)
            {
                int offset = 0x20600 + (0x7F000 * savindex) + 0xE8 * i + 8 * (i + 1);
                getSlotFiller(offset, pba[i + 42]);
                dctexta[i].Text = BitConverter.ToUInt32(savefile, 0x20600 + (0x7F000 * savindex) + 0xF0 * i + 4).ToString();
                if (Convert.ToBoolean(savefile[0x20600 + (0x7F000 * savindex) + 0xF0 * i]))   // If Occupied
                {
                    pba[i + 42].Image = ImageTransparency.ChangeOpacity(pba[i + 42].Image, 1);
                    dclabela[i].Text = (i + 1) + ": Occupied";
                }
                else
                {
                    pba[i + 42].Image = ImageTransparency.ChangeOpacity(pba[i + 42].Image, 0.6);
                    dclabela[i].Text = (i + 1) + ": Not Occupied";
                }
            }
            DayCare_HasEgg.Checked = Convert.ToBoolean(savefile[0x20600 + (0x7F000 * savindex) + 0x1E0]);
            TB_RNGSeed.Text = BitConverter.ToUInt64(savefile, 0x20600 + (0x7F000 * savindex) + 0x1E8).ToString("X16");

            // GTS
            getSlotFiller(0x1CC00 + (0x7F000 * savindex), pba[44]);

            // Fused
            getSlotFiller(0x1B400 + (0x7F000 * savindex), pba[45]);

            // SUBE
            for (int i = 0; i < 3; i++)
            {
                int offset = 0x22C90 + i * 0xEC + (0x7F000 * savindex);
                if (BitConverter.ToUInt64(savefile, offset) != 0)
                {
                    getSlotFiller(offset, pba[46 + i]);
                }
                else pba[46 + i].Image = null;
            }

            // Recoloring of a storage box slot (to not show for other storage boxes)
            if (colorizedslot < 32)
            {
                if (colorizedbox == C_BoxSelect.SelectedIndex)
                {
                    pba[colorizedslot].BackColor = colorizedcolor;
                }
                else
                {
                    pba[colorizedslot].BackColor = Color.Transparent;
                }
            }
        }
        public void getBoxNames()
        {
            int selectedbox = C_BoxSelect.SelectedIndex;    // precache selected box
            // Build ComboBox Dropdown Items
            try
            {
                C_BoxSelect.Items.Clear();
                for (int i = 0; i < 31; i++)
                {
                    string boxname = Encoding.Unicode.GetString(savefile, SaveGame.PCLayout + (0x7F000 * savindex) + 0x22 * i, 0x22);
                    C_BoxSelect.Items.Add(boxname);
                }
            }
            catch
            {
                for (int i = 1; i < 32; i++)
                {
                    C_BoxSelect.Items.Add("Box " + i);
                }
            }

            C_BoxSelect.SelectedIndex = selectedbox;    // restore selected box
        }
        private void getSAVLabel()
        {
            L_SAVINDEX.Text = (savindex + 1).ToString();
            RTB_S.Text += "\r\nLoaded Save File " + (savindex + 1).ToString();
        }
        private void getSAVOffsets()
        {
            // Get the save file offsets for the input game
            bool enableInterface = false;
            if (BitConverter.ToUInt32(savefile, 0x6A810 + 0x7F000 * savindex) == 0x42454546)
            {
                enableInterface = true;
                SaveGame = new SaveGames.SaveStruct("XY");
            }
            else
            {
                MessageBox.Show("Unrecognized Save File loaded.", "Error");
                SaveGame = new SaveGames.SaveStruct("Error");
            }

            // Enable Buttons
            GB_SAVtools.Enabled = B_JPEG.Enabled = B_BoxIO.Enabled = B_OUTPasserby.Enabled = B_VerifyCHK.Enabled = B_VerifySHA.Enabled = B_SwitchSAV.Enabled
                = enableInterface;
        }
        private void getSlotFiller(int offset, PictureBox pb)
        {
            byte[] slotdata = new Byte[0xE8];
            Array.Copy(savefile, offset, slotdata, 0, 0xE8);    // Fill Our EKX Slot
            byte[] dslotdata = decryptArray(slotdata);

            int species = BitConverter.ToInt16(dslotdata, 0x08); // Get Species
            uint isegg = (BitConverter.ToUInt32(dslotdata, 0x74) >> 30) & 1;

            int altforms = (dslotdata[0x1D] >> 3);
            int gender = (dslotdata[0x1D] >> 1) & 0x3;

            string file;
            if (isegg == 1)
            { file = "egg"; }
            else
            {
                file = "_" + species.ToString();
                if (altforms > 0) // Alt Form Handling
                {
                    file = file + "_" + altforms.ToString();
                }
                else if ((species == 521) && (gender == 1))   // Unfezant
                {
                    file = file = "_" + species.ToString() + "f";
                }
            }
            if (species == 0)
                file = "_0";

            pb.Image = (Image)Properties.Resources.ResourceManager.GetObject(file);
        }
        private void getSlotColor(int slot, Color color)
        {
            PictureBox[] pba = {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                                };
            for (int i = 0; i < pba.Length; i++)
            {
                pba[i].BackColor = Color.Transparent;
            }
            if (slot < 32)
            {
                colorizedbox = C_BoxSelect.SelectedIndex;
            }
            pba[slot].BackColor = color;
            colorizedcolor = color;
            colorizedslot = slot;
        }
        private void getBox(object sender, EventArgs e)
        {
            getPKXBoxes();
        }
        private void getTSV(object sender, EventArgs e)
        {
            uint TID = ToUInt32(TB_TID.Text);
            uint SID = ToUInt32(TB_SID.Text);
            uint tsv = (TID ^ SID) >> 4;
            Tip1.SetToolTip(this.TB_TID, "TSV: " + tsv.ToString("0000"));
            Tip2.SetToolTip(this.TB_SID, "TSV: " + tsv.ToString("0000"));

            uint PID = getHEXval(TB_PID);
            uint psv = ((PID >> 16) ^ (PID & 0xFFFF)) >> 4;
            Tip3.SetToolTip(this.TB_PID, "PSV: " + psv.ToString("0000"));
        }
        private void Menu_DumpLoadBoxes_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Press Yes to Import All\r\nPress No to Dump All\r\nPress Cancel to abort.", "Alert", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Cancel)
            {
                string exepath = System.Windows.Forms.Application.StartupPath;
                string path = "";
                {
                    int offset = SaveGame.Box;
                    int size = 232;
                    if (dr == DialogResult.Yes) // Import
                    {
                        if (Directory.Exists(exepath + "\\db"))
                        {
                            DialogResult ld = MessageBox.Show("Load from PKHeX's database?", "Alert", MessageBoxButtons.YesNo);
                            if (ld == DialogResult.Yes)
                            {
                                path = exepath + "\\db";
                            }
                            else if (ld == DialogResult.No)
                            {
                                // open folder dialog
                                FolderBrowserDialog fbd = new FolderBrowserDialog();
                                if (fbd.ShowDialog() == DialogResult.OK)
                                {
                                    path = fbd.SelectedPath;
                                }
                            }
                            else return;
                        }
                        else
                        {
                            // open folder dialog
                            FolderBrowserDialog fbd = new FolderBrowserDialog();
                            if (fbd.ShowDialog() == DialogResult.OK)
                            {
                                path = fbd.SelectedPath;
                            }
                        }

                        loadBoxesFromDB(path);
                    }
                    else if (dr == DialogResult.No)
                    {
                        // Dump all of box content to files.
                        {
                            DialogResult ld = MessageBox.Show("Save to PKHeX's database?", "Alert", MessageBoxButtons.YesNo);
                            if (ld == DialogResult.Yes)
                            {
                                path = exepath + "\\db";
                                if (!Directory.Exists(path))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(path);
                                }
                            }
                            else if (ld == DialogResult.No)
                            {
                                // open folder dialog
                                FolderBrowserDialog fbd = new FolderBrowserDialog();
                                if (fbd.ShowDialog() == DialogResult.OK)
                                {
                                    path = fbd.SelectedPath;
                                }
                            }
                            else return;
                        }
                        for (int i = 0; i < 31 * 30 * size; i += size)
                        {
                            byte[] ekxdata = new Byte[size];
                            Array.Copy(savefile, offset + i, ekxdata, 0, size);
                            byte[] pkxdata = decryptArray(ekxdata);


                            int species = BitConverter.ToInt16(pkxdata, 0x08);
                            if (species == 0) continue;
                            uint chk = BitConverter.ToUInt16(pkxdata, 0x06);
                            uint EC = BitConverter.ToUInt32(pkxdata, 0);
                            uint IV32 = BitConverter.ToUInt32(pkxdata, 0x74);

                            string nick = "";
                            if (Convert.ToBoolean((IV32 >> 31) & 1))
                            {
                                nick = TrimFromZero(Encoding.Unicode.GetString(pkxdata, 0x40, 24)) + " (" + specieslist[species] + ")";
                            }
                            else
                            {
                                nick = specieslist[species];
                            }
                            if (Convert.ToBoolean((IV32 >> 30) & 1))
                            {
                                nick += " (" + eggname + ")";
                            }

                            string isshiny = "";
                            int gamevers = pkxdata[0xDF];

                            uint PID = BitConverter.ToUInt32(pkxdata, 0x18);
                            uint UID = (PID >> 16);
                            uint LID = (PID & 0xFFFF);
                            uint PSV = UID ^ LID;
                            uint TSV = ToUInt32(TB_TID.Text) ^ ToUInt32(TB_SID.Text);
                            uint XOR = TSV ^ PSV;
                            if (((XOR < 8) && (gamevers < 24)) || ((XOR < 16) && (gamevers > 24)))
                            {   // Is Shiny
                                isshiny = " ★";
                            }

                            string savedname =
                                species.ToString("000") + isshiny + " - "
                                + nick + " - "
                                + chk.ToString("X4") + EC.ToString("X8")
                                + ".pk6";
                            Array.Resize(ref pkxdata, 232);
                            if (!File.Exists(path + "\\" + savedname))
                            {
                                File.WriteAllBytes(path + "\\" + CleanFileName(savedname), pkxdata);
                            }
                        }
                    }
                }
            }
        }
        private void loadBoxesFromDB(string path)
        {
            if (path == "") return;
            int offset = SaveGame.Box;
            int size = 232;
            Array.Clear(savefile, offset, size * 30 * 31); // Clear out the box data array.
            string[] filepaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            int ctr = 0;
            for (int i = 0; i < filepaths.Length; i++)
            {
                long len = new FileInfo(filepaths[i]).Length;
                if (len != 232 && len != 260)
                    continue;

                byte[] data = new Byte[232];
                string ext = Path.GetExtension(filepaths[i]);
                if (ext == ".pkx" || ext == ".pk6")
                {
                    byte[] input = File.ReadAllBytes(filepaths[i]);
                    if ((input[0xC8] == 0) && (input[0xC9] == 0) && (input[0x58] == 0) && (input[0x59] == 0))
                    {
                        Array.Resize(ref input, 232);
                        uint chk = 0;
                        for (int z = 8; z < 232; z += 2) // Loop through the entire PKX
                        {
                            chk += (uint)(input[z] + input[z + 1] * 0x100);
                        }
                        chk &= 0xFFFF;

                        ushort actualsum = BitConverter.ToUInt16(input, 0x6);
                        if (chk != actualsum) continue;

                        data = encryptArray(input);
                    }
                }
                else if (ext == ".ekx" || ext == ".ek6")
                {
                    byte[] input = File.ReadAllBytes(filepaths[i]);
                    Array.Resize(ref input, 232);
                    Array.Copy(input, data, 232);
                    // check if it is good data
                    byte[] decrypteddata = decryptArray(input);

                    if (!(BitConverter.ToUInt16(decrypteddata, 0xC8) == 0) && !(BitConverter.ToUInt16(decrypteddata, 0x58) == 0))
                        continue; // don't allow improperly encrypted files. they must be encrypted properly.
                    uint chk = 0;
                    for (int z = 8; z < 232; z += 2) // Loop through the entire PKX
                    {
                        chk += (uint)(decrypteddata[z] + decrypteddata[z + 1] * 0x100);
                    }
                    chk &= 0xFFFF;

                    ushort actualsum = BitConverter.ToUInt16(decrypteddata, 0x6);
                    if (chk != actualsum) continue;

                }
                Array.Copy(data, 0, savefile, offset + ctr * size, 232);
                ctr++;
                if (ctr == 30 * 31) break; // break out if we have written all 31 boxes
            }
            if (ctr > 0) // if we've written at least one pk6 in, go ahead and make sure the window is stretched.
            {
                if (Width < Height) // expand if boxes aren't visible
                {
                    this.Width = (this.Width * (54000 / 260)) / 100 + 2;
                    tabBoxMulti.Enabled = true;
                    tabBoxMulti.SelectedIndex = 0;
                }
                getPKXBoxes();
            }
        }
        // Subfunction Save Buttons // 
        private void B_OpenWondercards_Click(object sender, EventArgs e)
        {
            // Open Wondercard Menu
            PKHeX.SAV_Wondercard sb32 = new PKHeX.SAV_Wondercard(this);
            sb32.ShowDialog();
        }
        private void B_OpenBoxLayout_Click(object sender, EventArgs e)
        {
            // Open Box Layout Menu
            PKHeX.SAV_BoxLayout sb21 = new PKHeX.SAV_BoxLayout(this);
            sb21.ShowDialog();
        }
        private void B_OpenTrainerInfo_Click(object sender, EventArgs e)
        {
            PKHeX.SAV_Trainer sb13 = new PKHeX.SAV_Trainer(this,SaveGame.Name);
            sb13.ShowDialog();
        }
        private void B_OpenPokepuffs_Click(object sender, EventArgs e)
        {
            PKHeX.SAV_Pokepuff sb11 = new PKHeX.SAV_Pokepuff(this);
            sb11.ShowDialog();
        }
        private void B_OpenItemPouch_Click(object sender, EventArgs e)
        {
            PKHeX.SAV_Inventory sb12 = new PKHeX.SAV_Inventory(this);
            sb12.ShowDialog();
        }
        private void B_OpenBerryField_Click(object sender, EventArgs e)
        {
            PKHeX.SAV_BerryField sb23 = new PKHeX.SAV_BerryField(this,SaveGame.BerryField);
            sb23.ShowDialog();
        }
        private void B_OpenEventFlags_Click(object sender, EventArgs e)
        {
            // Open Flag Menu
            SAV_EventFlags eventflags = new PKHeX.SAV_EventFlags(this);
            eventflags.ShowDialog();
        }
        private void B_OUTPasserby_Click(object sender, EventArgs e)
        {
            RTB_T.Text = "PSS List\r\n\r\n";
            string[] headers = {
                                   "PSS Data - Friends",
                                   "PSS Data - Acquaintances",
                                   "PSS Data - Passerby",
                               };
            int offset = savindex * 0x7F000 + SaveGame.PSS;
            for (int g = 0; g < 3; g++)
            {
                RTB_T.Text += "----\r\n" + headers[g] + "\r\n" + "----\r\n" + "\r\n";
                uint count = BitConverter.ToUInt32(savefile,offset + 0x4E20);
                int r_offset = offset;

                for (int i = 0; i < 100; i++)
                {
                    ulong unkn = BitConverter.ToUInt64(savefile, r_offset);
                    if (unkn == 0)
                    {
                        break;
                    }

                    string otname = TrimFromZero(Encoding.Unicode.GetString(savefile, r_offset + 8, 0x1A));
                    string message = TrimFromZero(Encoding.Unicode.GetString(savefile, r_offset + 0x22, 0x22));

                    // Trim terminated

                    uint unk1 = BitConverter.ToUInt32(savefile, r_offset + 0x44);
                    ulong unk2 = BitConverter.ToUInt64(savefile, r_offset + 0x48);
                    uint unk3 = BitConverter.ToUInt32(savefile, r_offset + 0x50);
                    uint unk4 = BitConverter.ToUInt16(savefile, r_offset + 0x54);
                    byte region = savefile[r_offset + 0x56];
                    byte country = savefile[r_offset + 0x57];
                    byte _3dsreg = savefile[r_offset + 0x58];
                    byte _lang = savefile[r_offset + 0x59];
                    byte game = savefile[r_offset + 0x5A];
                    ulong outfit = BitConverter.ToUInt64(savefile, r_offset + 0x5C);
                    int favpkm = BitConverter.ToUInt16(savefile, r_offset + 0x9C) & 0x7FF;
                    string gamename;
                    if (game == 0x18)
                    {
                        gamename = "X";
                    }
                    else gamename = "Y";

                    RTB_T.Text +=
                        "OT: " + otname + "\r\n" +
                        "Message: " + message + "\r\n" +
                        "Game: " + gamename + "\r\n" +
                        "Country ID: " + country + "\r\n" + 
                        "Region ID: " + region + "\r\n" +
                        "Favorite: " + specieslist[favpkm] + "\r\n";

                    RTB_T.Text += "\r\n";
                    r_offset += 0xC8;
                }
                offset += 0x5000;
            }
            RTB_T.Text = RTB_T.Text;
            RTB_T.Font = new Font("Courier New", 8);
        }
        private void B_OUTHallofFame_Click(object sender, EventArgs e)
        {
            // Open HoF Menu
            SAV_HallOfFame halloffame = new PKHeX.SAV_HallOfFame(this);
            halloffame.ShowDialog();
        }
        private void B_SwitchSAV_Click(object sender, EventArgs e)
        {
            DialogResult switchsav = MessageBox.Show("Current Savefile is Save" + ((savindex + 1)).ToString() + ". Would you like to switch to Save" + ((savindex + 1) % 2 + 1).ToString() + "?", "Prompt", MessageBoxButtons.YesNo);
            if (switchsav == DialogResult.Yes)
            {
                savindex = (savindex + 1) % 2;
                getBoxNames();
                getPKXBoxes();
                getSAVLabel();
            }
        }
        private void B_JPEG_Click(object sender, EventArgs e)
        {
            int offset = 0x7F000 * savindex + SaveGame.JPEG;

            string filename = Encoding.Unicode.GetString(savefile, offset + 0, 0x1A).Replace("\0", string.Empty);
            filename += "'s picture";
            offset += 0x54;
            if (savefile[offset] != 0xFF)
            {
                MessageBox.Show("No PGL picture data found!", "Error");
                return;
            }
            
            int length = 0xE004;

            byte[] jpeg = new Byte[length];
            Array.Copy(savefile, offset, jpeg, 0, length);
            SaveFileDialog savejpeg = new SaveFileDialog();
            savejpeg.FileName = filename;
            savejpeg.Filter = "JPEG|*.jpeg";
            if (savejpeg.ShowDialog() == DialogResult.OK)
            {
                string path = savejpeg.FileName;
                if (File.Exists(path))
                {
                    // File already exists, save a .bak
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(path + ".bak", backupfile);
                }
                File.WriteAllBytes(path, jpeg);
            }
        }
        private void B_BoxIO_Click(object sender, EventArgs e)
        {
            // Allow Import/Export of Boxes
            PKHeX.SAV_BoxIO boxio = new PKHeX.SAV_BoxIO(this, SaveGame.Box, SaveGame.PCLayout);
            boxio.ShowDialog();
        }
        #endregion

        // Language Translation
        private void changeMainLanguage(object sender, EventArgs e)
        {
            if (init)
            {
                buff = preparepkx(buff); // get data currently in form
            }
            Menu_Options.DropDown.Close();
            InitializeStrings();
            InitializeLanguage();
            TranslateInterface("Form1");
            populatefields(buff); // put data back in form
        }
        private string[] getStringList(string f, string l)
        {
            object txt = Properties.Resources.ResourceManager.GetObject("text_" + f + "_" + l); // Fetch File, \n to list.
            List<string> rawlist = ((string)txt).Split(new char[] { '\n' }).ToList();

            string[] stringdata = new string[rawlist.Count];
            for (int i = 0; i < rawlist.Count; i++)
            {
                stringdata[i] = rawlist[i];
            }
            return stringdata;
        }
        private void InitializeStrings()
        {
            string[] lang_val = { "en", "ja", "fr", "it", "de", "es", "ko" };
            curlanguage = lang_val[CB_MainLanguage.SelectedIndex];
            
            string l = curlanguage;
            natures = getStringList("Natures", l);
            types = getStringList("Types", l);
            abilitylist = getStringList("Abilities", l);
            movelist = getStringList("Moves", l);
            itemlist = getStringList("Items", l);
            characteristics = getStringList("Character", l);
            specieslist = getStringList("Species", l);
            forms = getStringList("Forms", l);
            memories = getStringList("Memories", l);
            genloc = getStringList("GenLoc", l);

            // Get the Egg Name and then replace it with --- for the comboboxes.
            eggname = specieslist[0];
            specieslist[0] = "---";

            // Get the met locations... for all of the games...
            metHGSS_00000 = getStringList("hgss_00000", l);
            metHGSS_02000 = getStringList("hgss_02000", l);
            metHGSS_03000 = getStringList("hgss_03000", l);

            metBW2_00000 = getStringList("bw2_00000", l);
            metBW2_30000 = getStringList("bw2_30000", l);
            metBW2_40000 = getStringList("bw2_40000", l);
            metBW2_60000 = getStringList("bw2_60000", l);

            metXY_00000 = getStringList("xy_00000", l);
            metXY_30000 = getStringList("xy_30000", l);
            metXY_40000 = getStringList("xy_40000", l);
            metXY_60000 = getStringList("xy_60000", l);

            // Fix up some of the strings to make them more descriptive:
            metHGSS_02000[1] += " (NPC)";         // Anything from an NPC
            metHGSS_02000[2] += " ("+eggname+")"; // Egg From Link Trade
            metBW2_00000[36] = metBW2_00000[84] + "/" + metBW2_00000[36]; // Cold Storage in BW = PWT in BW2
            // BW2 Entries from 76 to 105 are for Entralink in BW
            for (int i = 76; i < 106; i++)
            {
                metBW2_00000[i] = metBW2_00000[i] + "●";
            }
            // Localize the Poketransfer to the language (30001)
            string[] ptransp = {"Poké Transfer", "ポケシフター","Poké Fret","Pokétrasporto","Poképorter","Pokétransfer","포케시프터",};
            metBW2_30000[1-1] = ptransp[CB_MainLanguage.SelectedIndex];
            metBW2_30000[2-1] += " (NPC)";              // Anything from an NPC
            metBW2_30000[3-1] += " (" + eggname + ")";  // Egg From Link Trade

            // Zorua/Zoroark events
            metBW2_30000[10-1] = specieslist[251] + " (" + specieslist[570] + " 1)"; // Celebi's Zorua Event
            metBW2_30000[11-1] = specieslist[251] + " (" + specieslist[570] + " 2)"; // Celebi's Zorua Event
            metBW2_30000[12-1] = specieslist[571] + " (" + "1)"; // Zoroark
            metBW2_30000[13-1] = specieslist[571] + " (" + "2)"; // Zoroark

            metBW2_60000[3-1] += " (" + eggname + ")";  // Egg Treasure Hunter/Breeder, whatever...

            metXY_30000[0] += " (NPC)";                // Anything from an NPC
            metXY_30000[1] += " (" + eggname + ")";    // Egg From Link Trade

            // Set the first entry of a met location to "" (nothing)
            metXY_00000[0] = "";
            metBW2_00000[0] = "";
            metHGSS_00000[0] = "";

            // Force an update to the met locations
            origintrack = "";

            UpdateIVs(null, null); // Prompt an update for the characteristics
        }
    }
    #region Structs & Classes
    public class cbItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
    public class SaveGames
    {
        public struct SaveStruct
        {
            public int Box, Party, BattleBox, GTS, Daycare, Fused, SUBE, Puff, Item, Trainer1, Trainer2, PCLayout, Wondercard, BerryField, OPower, EventFlag, PokeDex, HoF, PSS, JPEG;
            public string Name;
            public SaveStruct(string GameID)
            {
                if (GameID == "XY")
                {
                    Name = "XY";
                    Box = 0x27A00;
                    Party = 0x19600;
                    BattleBox = 0x09E00;
                    Daycare = 0x20600;
                    GTS = 0x1CC00;
                    Fused = 0x1B400;
                    SUBE = 0x22C90;

                    Puff = 0x5400;
                    Item = 0x5800;
                    Trainer1 = 0x6800;
                    Trainer2 = 0x9600;
                    PCLayout = 0x9800;
                    Wondercard = 0x21000;
                    BerryField = 0x20C00;
                    OPower = 0x1BE00;
                    EventFlag = 0x19E00;
                    PokeDex = 0x1A400;

                    HoF = 0x1E800;
                    JPEG = 0x5C600;
                    PSS = 0x0A400;

                }
                else if (GameID == "ORAS")
                {
                    // Temp
                    Name = "ORAS";
                    Box = 0x27A00;
                    Party = 0x19600;
                    BattleBox = 0x09E00;
                    Daycare = 0x20600;
                    GTS = 0x1CC00;
                    Fused = 0x1B400;
                    SUBE = 0x22C90;

                    Puff = 0x5400;
                    Item = 0x5800;
                    Trainer1 = 0x6800;
                    Trainer2 = 0x9600;
                    PCLayout = 0x9800;
                    Wondercard = 0x21000;
                    BerryField = 0x20C00;
                    OPower = 0x1BE00;
                    EventFlag = 0x19E00;
                    PokeDex = 0x1A400;

                    HoF = 0x1E800;
                    JPEG = 0x5C600;
                    PSS = 0x0A400;

                }
                else
                {
                    // Copied...
                    Name = "Unknown";
                    Box = 0x27A00;
                    Party = 0x19600;
                    BattleBox = 0x09E00;
                    Daycare = 0x20600;
                    GTS = 0x1CC00;
                    Fused = 0x1B400;
                    SUBE = 0x22C90;

                    Puff = 0x5400;
                    Item = 0x5800;
                    Trainer1 = 0x6800;
                    Trainer2 = 0x9600;
                    PCLayout = 0x9800;
                    Wondercard = 0x21000;
                    BerryField = 0x20C00;
                    OPower = 0x1BE00;
                    EventFlag = 0x19E00;
                    PokeDex = 0x1A400;

                    HoF = 0x1E800;
                    JPEG = 0x5C600;
                    PSS = 0x0A400;
                }
            }
        }
    }
    public class ImageTransparency
    {
        public static Bitmap ChangeOpacity(Image img, double opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = (float)opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();   // Releasing all resource used by graphics
            return bmp;
        }
    }
    #endregion
}

