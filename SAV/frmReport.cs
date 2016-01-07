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
        private byte[] SaveData;

        public class Preview
        {
            private PK6 pk6;
            public string Position { get { return pk6.Identifier; } }
            public Image Sprite { get { return pk6.Sprite; } }
            public string Nickname { get { return pk6.Nickname; } }
            public string Species { get { return Main.specieslist[pk6.Species]; } }
            public string Nature { get { return Main.natures[pk6.Nature]; } }
            public string Gender { get { return Main.gendersymbols[pk6.Gender]; } }
            public string ESV { get { return pk6.PSV.ToString("0000"); } }
            public string HP_Type { get { return Main.types[pk6.HPType]; } }
            public string Ability { get { return Main.abilitylist[pk6.Ability]; } }
            public string Move1 { get { return Main.movelist[pk6.Move1]; } }
            public string Move2 { get { return Main.movelist[pk6.Move2]; } }
            public string Move3 { get { return Main.movelist[pk6.Move3]; } }
            public string Move4 { get { return Main.movelist[pk6.Move4]; } }
            public string HeldItem { get { return Main.itemlist[pk6.HeldItem]; } }
            public string MetLoc { get { return PKX.getLocation(false, pk6.Version, pk6.Met_Location); } }
            public string EggLoc { get { return PKX.getLocation(true, pk6.Version, pk6.Egg_Location); } }
            public string Ball { get { return Main.balllist[pk6.Ball]; } }
            public string OT { get { return pk6.OT_Name; } }
            public string Version { get { return Main.gamelist[pk6.Version]; } }
            public string OTLang { get { return Main.gamelanguages[pk6.Language] ?? String.Format("UNK {0}", pk6.Language); } }
            public string CountryID { get { return pk6.Country.ToString(); } }
            public string RegionID { get { return pk6.Region.ToString(); } }
            public string DSRegionID { get { return pk6.ConsoleRegion.ToString(); } }

            #region Extraneous
            public string EC { get { return pk6.EncryptionConstant.ToString("X8"); } }
            public string PID { get { return pk6.PID.ToString("X8"); } }
            public int HP_IV { get { return pk6.IV_HP; } }
            public int ATK_IV { get { return pk6.IV_ATK; } }
            public int DEF_IV { get { return pk6.IV_DEF; } }
            public int SPA_IV { get { return pk6.IV_SPA; } }
            public int SPD_IV { get { return pk6.IV_SPD; } }
            public int SPE_IV { get { return pk6.IV_SPE; } }
            public uint EXP { get { return pk6.EXP; } }
            public int Level { get { return pk6.Stat_Level; } }
            public int HP_EV { get { return pk6.EV_HP; } }
            public int ATK_EV { get { return pk6.EV_ATK; } }
            public int DEF_EV { get { return pk6.EV_DEF; } }
            public int SPA_EV { get { return pk6.EV_SPA; } }
            public int SPD_EV { get { return pk6.EV_SPD; } }
            public int SPE_EV { get { return pk6.EV_SPE; } }
            public int Cool { get { return pk6.CNT_Cool; } }
            public int Beauty { get { return pk6.CNT_Beauty; } }
            public int Cute { get { return pk6.CNT_Cute; } }
            public int Smart { get { return pk6.CNT_Smart; } }
            public int Tough { get { return pk6.CNT_Tough; } }
            public int Sheen { get { return pk6.CNT_Sheen; } }
            public int Markings { get { return pk6.Markings; } }

            public string NotOT { get { return pk6.HT_Name; } }

            public int AbilityNum { get { return pk6.AbilityNumber; } }
            public int GenderFlag { get { return pk6.Gender; } }
            public int AltForms { get { return pk6.AltForm; } }
            public int PKRS_Strain { get { return pk6.PKRS_Strain; } }
            public int PKRS_Days { get { return pk6.PKRS_Days; } }
            public int MetLevel { get { return pk6.Met_Level; } }
            public int OT_Gender { get { return pk6.OT_Gender; } }

            public bool FatefulFlag { get { return pk6.FatefulEncounter; } }
            public bool IsEgg { get { return pk6.IsEgg; } }
            public bool IsNicknamed { get { return pk6.IsNicknamed; } }
            public bool IsShiny { get { return pk6.IsShiny; } }

            public int TID { get { return pk6.TID; } }
            public int SID { get { return pk6.SID; } }
            public int TSV { get { return pk6.TSV; } }
            public int Move1_PP { get { return pk6.Move1_PP; } }
            public int Move2_PP { get { return pk6.Move2_PP; } }
            public int Move3_PP { get { return pk6.Move3_PP; } }
            public int Move4_PP { get { return pk6.Move4_PP; } }
            public int Move1_PPUp { get { return pk6.Move1_PPUps; } }
            public int Move2_PPUp { get { return pk6.Move2_PPUps; } }
            public int Move3_PPUp { get { return pk6.Move3_PPUps; } }
            public int Move4_PPUp { get { return pk6.Move4_PPUps; } }
            public string Relearn1 { get { return Main.movelist[pk6.RelearnMove1]; } }
            public string Relearn2 { get { return Main.movelist[pk6.RelearnMove2]; } }
            public string Relearn3 { get { return Main.movelist[pk6.RelearnMove3]; } }
            public string Relearn4 { get { return Main.movelist[pk6.RelearnMove4]; } }
            public ushort Checksum { get { return pk6.Checksum; } }
            public int mFriendship { get { return pk6.OT_Friendship; } }
            public int OT_Affection { get { return pk6.OT_Affection; } }
            public int Egg_Year { get { return pk6.Egg_Year; } }
            public int Egg_Month { get { return pk6.Egg_Month; } }
            public int Egg_Day { get { return pk6.Egg_Day; } }
            public int Met_Year { get { return pk6.Met_Year; } }
            public int Met_Month { get { return pk6.Met_Month; } }
            public int Met_Day { get { return pk6.Met_Day; } }
            public int Encounter { get { return pk6.EncounterType; } }
            #endregion
            public Preview(PK6 p) { pk6 = p; }
        }
        public frmReport()
        {
            InitializeComponent();
            dgData.DoubleBuffered(true);
        }
        public void PopulateData(byte[] InputData, int BoxDataOffset)
        {
            SaveData = (byte[])InputData.Clone();
            PokemonList PL = new PokemonList();
            BoxBar.Maximum = 930 + 100;
            BoxBar.Step = 1;
            for (int BoxNum = 0; BoxNum < 31; BoxNum++)
            {
                int boxoffset = BoxDataOffset + BoxNum * (0xE8 * 30);
                for (int SlotNum = 0; SlotNum < 30; SlotNum++)
                {
                    BoxBar.PerformStep();
                    int offset = boxoffset + 0xE8 * SlotNum;
                    byte[] slotdata = new byte[0xE8];
                    Array.Copy(SaveData, offset, slotdata, 0, 0xE8);
                    byte[] dslotdata = PKX.decryptArray(slotdata);
                    if (BitConverter.ToUInt16(dslotdata, 0x8) == 0) continue;
                    string Identifier = String.Format("B{0}:{1}", (BoxNum + 1).ToString("00"), (SlotNum + 1).ToString("00"));
                    PK6 pkm = new PK6(dslotdata, Identifier);
                    if (pkm.EncryptionConstant == 0 && pkm.Species == 0) continue;
                    if (pkm.Checksum != pkm.CalculateChecksum()) continue;
                    pkm.Stat_Level = PKX.getLevel(pkm.Species, pkm.EXP); // recalc Level
                    PL.Add(new Preview(pkm));
                }
            }
            dgData.DataSource = PL;
            dgData.AutoGenerateColumns = true;
            BoxBar.Maximum = 930 + dgData.Columns.Count;
            for (int i = 0; i < dgData.Columns.Count; i++)
            {
                BoxBar.PerformStep();
                if (dgData.Columns[i] is DataGridViewImageColumn) continue; // Don't add sorting for Sprites
                dgData.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }
            BoxBar.Visible = false;
        }
        public void PopulateData(PK6[] data)
        {
            BoxBar.Step = 1;
            PokemonList PL = new PokemonList();
            foreach (PK6 p in data)
                PL.Add(new Preview(p));

            dgData.DataSource = PL;
            dgData.AutoGenerateColumns = true;
            BoxBar.Maximum = data.Length + dgData.Columns.Count;
            for (int i = 0; i < dgData.Columns.Count; i++)
            {
                BoxBar.PerformStep();
                if (dgData.Columns[i] is DataGridViewImageColumn) continue; // Don't add sorting for Sprites
                dgData.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }
            BoxBar.Visible = false;
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
            sb.AppendLine(string.Join(",", headers.Select(column => "\"" + column.HeaderText + "\"").ToArray()));

            foreach (DataGridViewRow row in dgData.Rows)
            {
                var cells = row.Cells.Cast<DataGridViewCell>();
                sb.AppendLine(string.Join(",", cells.Select(cell => "\"" + cell.Value + "\"").ToArray()));
            }
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

        protected SortableBindingList()
            : base(new List<T>())
        {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        public SortableBindingList(IEnumerable<T> enumeration)
            : base(new List<T>(enumeration))
        {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return isSorted; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return propertyDescriptor; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return listSortDirection; }
        }

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

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
            {
                T element = this[i];
                if (property.GetValue(element).Equals(key))
                {
                    return i;
                }
            }

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