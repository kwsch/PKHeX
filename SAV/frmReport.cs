using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
                    string Identifier = String.Format("B{0}:{1}",BoxNum.ToString("00"),SlotNum.ToString("00"));
                    PKX pkm = new PKX(dslotdata, Identifier);
                    if ((pkm.EC == "00000000") && (pkm.Species == "---")) continue;
                    PL.Add(pkm);
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

        public class PokemonList : SortableBindingList<PKX> { }
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