using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Trainer7GG : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV7b SAV;

        public SAV_Trainer7GG(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV7b)(Origin = sav).Clone();
            if (Main.Unicode)
            {
                try { TB_OTName.Font = TB_RivalName.Font = FontUtil.GetPKXFont(11); }
                catch (Exception e) { WinFormsUtil.Alert("Font loading failed...", e.ToString()); }
            }

            B_MaxCash.Click += (sender, e) => MT_Money.Text = "9,999,999";

            GetComboBoxes();
            LoadTrainerInfo();

        }

        private void GetComboBoxes()
        {
            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection
            CB_Language.InitializeBinding();
            CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
            CB_Game.InitializeBinding();
            CB_Game.DataSource = new BindingSource(GameInfo.VersionDataSource.Where(z => GameVersion.GG.Contains(z.Value)).ToList(), null);
        }

        private void LoadTrainerInfo()
        {
            // Get Data
            TB_OTName.Text = SAV.OT;
            TB_RivalName.Text = SAV.Misc.Rival;
            CB_Language.SelectedValue = SAV.Language;
            MT_Money.Text = SAV.Misc.Money.ToString();

            CB_Game.SelectedValue = SAV.Game;
            CB_Gender.SelectedIndex = SAV.Gender;
            trainerID1.LoadIDValues(SAV);

            // Load Play Time
            MT_Hours.Text = SAV.PlayedHours.ToString();
            MT_Minutes.Text = SAV.PlayedMinutes.ToString();
            MT_Seconds.Text = SAV.PlayedSeconds.ToString();
        }

        private void Save()
        {
            SaveTrainerInfo();
        }

        private void SaveTrainerInfo()
        {
            SAV.Game = WinFormsUtil.GetIndex(CB_Game);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;

            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.Language = WinFormsUtil.GetIndex(CB_Language);

            SAV.OT = TB_OTName.Text;
            SAV.Misc.Rival = TB_RivalName.Text;

            // Save PlayTime
            SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
            SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
            SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;
        }

        private void ClickString(object sender, MouseEventArgs e)
        {
            if (ModifierKeys != Keys.Control)
                return;

            TextBox tb = sender as TextBox ?? TB_OTName;

            // Special Character Form
            var d = new TrashEditor(tb, null, SAV);
            d.ShowDialog();
            tb.Text = d.FinalString;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            Save();
            Origin.SetData(SAV.Data, 0);
            Close();
        }

        private void Change255(object sender, EventArgs e)
        {
            MaskedTextBox box = (MaskedTextBox)sender;
            if (box.Text.Length == 0) box.Text = "0";
            if (Util.ToInt32(box.Text) > 255) box.Text = "255";
        }

        private void B_ExportGoSummary_Click(object sender, EventArgs e)
        {
            var block = new GoParkEntities(SAV);
            var summary = block.DumpAll(GameInfo.Strings.Species).ToArray();
            if (summary.Length == 0)
                return;
            Clipboard.SetText(string.Join(Environment.NewLine, summary));
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
