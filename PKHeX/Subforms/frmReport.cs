using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class frmReport : Form
    {
        public class Preview
        {
            private readonly PKM pkm;
            public string Position => pkm.Identifier;
            public Image Sprite => pkm.Sprite;
            public string Nickname => pkm.Nickname;
            public string Species => Main.GameStrings.specieslist[pkm.Species];
            public string Nature => Main.GameStrings.natures[pkm.Nature];
            public string Gender => Main.gendersymbols[pkm.Gender];
            public string ESV => pkm.PSV.ToString("0000");
            public string HP_Type => Main.GameStrings.types[pkm.HPType+1];
            public string Ability => Main.GameStrings.abilitylist[pkm.Ability];
            public string Move1 => Main.GameStrings.movelist[pkm.Move1];
            public string Move2 => Main.GameStrings.movelist[pkm.Move2];
            public string Move3 => Main.GameStrings.movelist[pkm.Move3];
            public string Move4 => Main.GameStrings.movelist[pkm.Move4];
            public string HeldItem => Main.GameStrings.itemlist[pkm.HeldItem];
            public string MetLoc => PKX.getLocation(pkm, eggmet: false);
            public string EggLoc => PKX.getLocation(pkm, eggmet: true);
            public string Ball => Main.GameStrings.balllist[pkm.Ball];
            public string OT => pkm.OT_Name;
            public string Version => Main.GameStrings.gamelist[pkm.Version];
            public string OTLang => Main.GameStrings.gamelanguages[pkm.Language] ?? $"UNK {pkm.Language}";
            public string CountryID => pkm.Format > 5 ? pkm.Country.ToString() : "N/A";
            public string RegionID => pkm.Format > 5 ? pkm.Region.ToString() : "N/A";
            public string DSRegionID => pkm.Format > 5 ? pkm.ConsoleRegion.ToString() : "N/A";

            #region Extraneous
            public string EC => pkm.EncryptionConstant.ToString("X8");
            public string PID => pkm.PID.ToString("X8");
            public int HP_IV => pkm.IV_HP;
            public int ATK_IV => pkm.IV_ATK;
            public int DEF_IV => pkm.IV_DEF;
            public int SPA_IV => pkm.IV_SPA;
            public int SPD_IV => pkm.IV_SPD;
            public int SPE_IV => pkm.IV_SPE;
            public uint EXP => pkm.EXP;
            public int Level => pkm.CurrentLevel;
            public int HP_EV => pkm.EV_HP;
            public int ATK_EV => pkm.EV_ATK;
            public int DEF_EV => pkm.EV_DEF;
            public int SPA_EV => pkm.EV_SPA;
            public int SPD_EV => pkm.EV_SPD;
            public int SPE_EV => pkm.EV_SPE;
            public int Cool => pkm.CNT_Cool;
            public int Beauty => pkm.CNT_Beauty;
            public int Cute => pkm.CNT_Cute;
            public int Smart => pkm.CNT_Smart;
            public int Tough => pkm.CNT_Tough;
            public int Sheen => pkm.CNT_Sheen;
            public int Markings => pkm.MarkValue;

            public string NotOT => pkm.Format > 5 ? pkm.HT_Name : "N/A";

            public int AbilityNum => pkm.Format > 5 ? pkm.AbilityNumber : -1;
            public int GenderFlag => pkm.Gender;
            public int AltForms => pkm.AltForm;
            public int PKRS_Strain => pkm.PKRS_Strain;
            public int PKRS_Days => pkm.PKRS_Days;
            public int MetLevel => pkm.Met_Level;
            public int OT_Gender => pkm.OT_Gender;

            public bool FatefulFlag => pkm.FatefulEncounter;
            public bool IsEgg => pkm.IsEgg;
            public bool IsNicknamed => pkm.IsNicknamed;
            public bool IsShiny => pkm.IsShiny;

            public int TID => pkm.TID;
            public int SID => pkm.SID;
            public int TSV => pkm.TSV;
            public int Move1_PP => pkm.Move1_PP;
            public int Move2_PP => pkm.Move2_PP;
            public int Move3_PP => pkm.Move3_PP;
            public int Move4_PP => pkm.Move4_PP;
            public int Move1_PPUp => pkm.Move1_PPUps;
            public int Move2_PPUp => pkm.Move2_PPUps;
            public int Move3_PPUp => pkm.Move3_PPUps;
            public int Move4_PPUp => pkm.Move4_PPUps;
            public string Relearn1 => Main.GameStrings.movelist[pkm.RelearnMove1];
            public string Relearn2 => Main.GameStrings.movelist[pkm.RelearnMove2];
            public string Relearn3 => Main.GameStrings.movelist[pkm.RelearnMove3];
            public string Relearn4 => Main.GameStrings.movelist[pkm.RelearnMove4];
            public ushort Checksum => pkm.Checksum;
            public int mFriendship => pkm.OT_Friendship;
            public int OT_Affection => pkm.OT_Affection;
            public int Egg_Year => pkm.EggMetDate.GetValueOrDefault().Year;
            public int Egg_Month => pkm.EggMetDate.GetValueOrDefault().Month;
            public int Egg_Day => pkm.EggMetDate.GetValueOrDefault().Day;
            public int Met_Year => pkm.MetDate.GetValueOrDefault().Year;
            public int Met_Month => pkm.MetDate.GetValueOrDefault().Month;
            public int Met_Day => pkm.MetDate.GetValueOrDefault().Day;
            public int Encounter => pkm.EncounterType;

            #endregion
            public Preview(PKM p) { pkm = p; }
        }
        public frmReport()
        {
            InitializeComponent();
            dgData.DoubleBuffered(true);
            CenterToParent();
        }
        public void PopulateData(PKM[] Data)
        {
            SuspendLayout();
            BoxBar.Step = 1;
            PokemonList PL = new PokemonList();
            foreach (PKM pkm in Data.Where(pkm => pkm.ChecksumValid && pkm.Species != 0))
            {
                pkm.Stat_Level = PKX.getLevel(pkm.Species, pkm.EXP); // recalc Level
                PL.Add(new Preview(pkm));
                BoxBar.PerformStep();
            }

            dgData.DataSource = PL;
            dgData.AutoGenerateColumns = true;
            BoxBar.Maximum = Data.Length + dgData.Columns.Count;
            for (int i = 0; i < dgData.Columns.Count; i++)
            {
                BoxBar.PerformStep();
                if (dgData.Columns[i] is DataGridViewImageColumn) continue; // Don't add sorting for Sprites
                dgData.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }
            BoxBar.Visible = false;
            ResumeLayout();
        }
        private void promptSaveCSV(object sender, FormClosingEventArgs e)
        {
            if (Util.Prompt(MessageBoxButtons.YesNo,"Save all the data to CSV?") == DialogResult.Yes)
            {
                SaveFileDialog savecsv = new SaveFileDialog
                {
                    Filter = "Spreadsheet|*.csv",
                    FileName = "Box Data Dump.csv"
                };
                if (savecsv.ShowDialog() == DialogResult.OK)
                    Export_CSV(savecsv.FileName);
            }
        }
        private void Export_CSV(string path)
        {
            var sb = new StringBuilder();

            var headers = dgData.Columns.Cast<DataGridViewColumn>();
            sb.AppendLine(string.Join(",", headers.Select(column => $"\"{column.HeaderText}\"")));

            foreach (var cells in from DataGridViewRow row in dgData.Rows select row.Cells.Cast<DataGridViewCell>())
                sb.AppendLine(string.Join(",", cells.Select(cell => $"\"{cell.Value}\"")));

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        public class PokemonList : SortableBindingList<Preview> { }
    }
    public static class ExtensionMethods    // Speed up scrolling
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    public class SortableBindingList<T> : BindingList<T>
    {
        private readonly Dictionary<Type, PropertyComparer<T>> comparers;
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor propertyDescriptor;

        protected SortableBindingList() : base(new List<T>())
        {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => isSorted;

        protected override PropertyDescriptor SortPropertyCore => propertyDescriptor;

        protected override ListSortDirection SortDirectionCore => listSortDirection;

        protected override bool SupportsSearchingCore => true;

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> itemsList = (List<T>)Items;

            Type propertyType = property.PropertyType;
            PropertyComparer<T> comparer;
            if (!comparers.TryGetValue(propertyType, out comparer))
            {
                comparer = new PropertyComparer<T>(property, direction);
                comparers.Add(propertyType, comparer);
            }

            comparer.SetPropertyAndDirection(property, direction);
            itemsList.Sort(comparer);

            propertyDescriptor = property;
            listSortDirection = direction;
            isSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            isSorted = false;
            propertyDescriptor = base.SortPropertyCore;
            listSortDirection = base.SortDirectionCore;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key)
        {
            int count = Count;
            for (int i = 0; i < count; ++i)
                if (property.GetValue(this[i]).Equals(key))
                    return i;

            return -1;
        }
    }
    public class PropertyComparer<T> : IComparer<T>
    {
        private readonly IComparer comparer;
        private PropertyDescriptor propertyDescriptor;
        private int reverse;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            propertyDescriptor = property;
            Type comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
            comparer = (IComparer)comparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
            SetListSortDirection(direction);
        }

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return reverse * comparer.Compare(propertyDescriptor.GetValue(x), propertyDescriptor.GetValue(y));
        }

        #endregion

        private void SetPropertyDescriptor(PropertyDescriptor descriptor)
        {
            propertyDescriptor = descriptor;
        }

        private void SetListSortDirection(ListSortDirection direction)
        {
            reverse = direction == ListSortDirection.Ascending ? 1 : -1;
        }

        public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction)
        {
            SetPropertyDescriptor(descriptor);
            SetListSortDirection(direction);
        }
    }
}
