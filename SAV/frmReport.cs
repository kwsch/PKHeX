using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Reflection;
using System.Text.RegularExpressions;
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
        public void PopulateData(byte[] InputData, int savindex, int baseoffset)
        {
            SaveData = new byte[InputData.Length];
            Array.Copy(InputData, SaveData, InputData.Length);
            PokemonList PL = new PokemonList();
            SaveGames.SaveStruct SaveGame = new SaveGames.SaveStruct("XY");
            if (savindex > 1) savindex = 0;
            for (int BoxNum = 0; BoxNum < 31; BoxNum++)
            {
                int boxoffset = baseoffset + 0x7F000 * savindex + BoxNum * (0xE8 * 30);
                for (int SlotNum = 0; SlotNum < 30; SlotNum++)
                {
                    int offset = boxoffset + 0xE8 * SlotNum;
                    byte[] slotdata = new Byte[0xE8];
                    Array.Copy(SaveData, offset, slotdata, 0, 0xE8);
                    byte[] dslotdata = PKX.decryptArray(slotdata);
                    PKX pkm = new PKX(dslotdata);
                    if ((pkm.EC == "00000000") && (pkm.Species == "---")) continue;
                    PL.Add(pkm);
                }
            }
            dgData.DataSource = PL;
            dgData.AutoGenerateColumns = true;
        }
        public class PokemonList : System.Collections.ObjectModel.ObservableCollection<PKX> { }
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
}