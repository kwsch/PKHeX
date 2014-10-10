using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class frmReport : Form1
    {
        private byte[] SaveData;
        private const int PIDOFFSET = 0x18;
        private const int TIDOFFSET = 0x0C;
        private const int SIDOFFSET = 0x0E;
        static Form1 m_parent;
        public frmReport(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            dgData.DoubleBuffered(true);
        }
        private static uint LCRNG(uint seed)
        {
            uint a = 0x41C64E6D;
            uint c = 0x00006073;

            seed = (seed * a + c) & 0xFFFFFFFF;
            return seed;
        }
        public byte[] shuffleArray(byte[] pkx, uint sv)
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
        public byte[] decryptArray(byte[] ekx)
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
        private UInt16 getTSV(UInt32 PID, UInt16 TID, UInt16 SID)
        {
            UInt16 tsv = Convert.ToUInt16((TID ^ SID) >> 4);
            return Convert.ToUInt16(((PID >> 16) ^ (PID & 0xFFFF)) >> 4);
        }
        private int detectSAVIndex(byte[] data)
        {
            int savindex = 0;
            SHA256 mySHA256 = SHA256Managed.Create();
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
                    savindex = 0;
                }
                else if (hashValue2.SequenceEqual(actualhash))
                {
                    savindex = 1;
                }
                else
                {
                    savindex = 2;
                }
            }
            if ((data[0x168] ^ 1) != savindex && savindex != 2)
            {
                savindex = 2;
            }
            return savindex;
        }
        public void PopulateData(byte[] InputData)
        {
            //IList<PKX> PL = new List<PKX>();
            SaveData = new byte[InputData.Length];
            Array.Copy(InputData, SaveData, InputData.Length);
            PokemonList PL = new PokemonList();
            SaveGames.SaveStruct SaveGame = new SaveGames.SaveStruct("XY");
            int savindex = detectSAVIndex(SaveData);
            if (savindex > 1) savindex = 0;
            for (int BoxNum = 0; BoxNum < 31; BoxNum++)
            {
                int boxoffset = 0x27A00 + 0x7F000 * savindex + BoxNum * (0xE8 * 30);
                for (int SlotNum = 0; SlotNum < 30; SlotNum++)
                {
                    int offset = boxoffset + 0xE8 * SlotNum;
                    byte[] slotdata = new Byte[0xE8];
                    Array.Copy(SaveData, offset, slotdata, 0, 0xE8);
                    byte[] dslotdata = decryptArray(slotdata);
                    PKX pkm = new PKX(dslotdata);
                    if ((pkm.EC == "00000000") && (pkm.Species == "---")) continue;
                    PL.Add(pkm);
                }
            }
            dgData.DataSource = PL;
            dgData.AutoGenerateColumns = true;
        }
        public static string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            if (index < 0)
                return input;

            return input.Substring(0, index);
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
