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
                    if ((pkm.EC == "00000000") && (pkm.Checksum == 0)) continue;
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
        public class PKX
        {
            #region Define
            private uint mEC, mPID, mIV32,

                mexp,
                mHP_EV, mATK_EV, mDEF_EV, mSPA_EV, mSPD_EV, mSPE_EV,
                mHP_IV, mATK_IV, mDEF_IV, mSPE_IV, mSPA_IV, mSPD_IV,
                mcnt_cool, mcnt_beauty, mcnt_cute, mcnt_smart, mcnt_tough, mcnt_sheen,
                mmarkings, mhptype;

            private string
                mnicknamestr, mgenderstring, mnotOT, mot, mSpeciesName, mNatureName, mHPName, mAbilityName,
                mMove1N, mMove2N, mMove3N, mMove4N;

            private int
                mability, mabilitynum, mnature, mfeflag, mgenderflag, maltforms, mPKRS_Strain, mPKRS_Duration,
                mmetlevel, motgender;

            private bool
                misegg, misnick, misshiny;

            private ushort
                mspecies, mhelditem, mTID, mSID, mTSV, mESV,
                mmove1, mmove2, mmove3, mmove4,
                mmove1_pp, mmove2_pp, mmove3_pp, mmove4_pp,
                mmove1_ppu, mmove2_ppu, mmove3_ppu, mmove4_ppu,
                meggmove1, meggmove2, meggmove3, meggmove4,
                mchk,

                mOTfriendship, mOTaffection,
                megg_year, megg_month, megg_day,
                mmet_year, mmet_month, mmet_day,
                meggloc, mmetloc,
                mball, mencountertype,
                mgamevers, mcountryID, mregionID, mdsregID, motlang;

            #endregion
            public string Nickname { get { return mnicknamestr; } }
            public string Species { get { return mSpeciesName; } }
            public string Nature { get { return mNatureName; } }
            public string Gender { get { return mgenderstring; } }
            public string ESV { get { return mESV.ToString("0000"); } }
            public string HP_Type { get { return mHPName; } }
            public string Ability { get { return mAbilityName; } }
            public string Move1 { get { return mMove1N; } }
            public string Move2 { get { return mMove2N; } }
            public string Move3 { get { return mMove3N; } }
            public string Move4 { get { return mMove4N; } }
            #region Extraneous
            public string EC { get { return mEC.ToString("X8"); } }
            public string PID { get { return mPID.ToString("X8"); } }
            public uint HP_IV { get { return mHP_IV; } }
            public uint ATK_IV { get { return mATK_IV; } }
            public uint DEF_IV { get { return mDEF_IV; } }
            public uint SPA_IV { get { return mSPA_IV; } }
            public uint SPD_IV { get { return mSPD_IV; } }
            public uint SPE_IV { get { return mSPE_IV; } }
            public uint EXP { get { return mexp; } }
            public uint HP_EV { get { return mHP_EV; } }
            public uint ATK_EV { get { return mATK_EV; } }
            public uint DEF_EV { get { return mDEF_EV; } }
            public uint SPA_EV { get { return mSPA_EV; } }
            public uint SPD_EV { get { return mSPD_EV; } }
            public uint SPE_EV { get { return mSPE_EV; } }
            public uint Cool { get { return mcnt_cool; } }
            public uint Beauty { get { return mcnt_beauty; } }
            public uint Cute { get { return mcnt_cute; } }
            public uint Smart { get { return mcnt_smart; } }
            public uint Tough { get { return mcnt_tough; } }
            public uint Sheen { get { return mcnt_sheen; } }
            public uint Markings { get { return mmarkings; } }

            public string NotOT { get { return mnotOT; } }
            public string OT { get { return mot; } }

            public int AbilityNum { get { return mabilitynum; } }
            public int FatefulFlag { get { return mfeflag; } }
            public int GenderFlag { get { return mgenderflag; } }
            public int AltForms { get { return maltforms; } }
            public int PKRS_Strain { get { return mPKRS_Strain; } }
            public int PKRS_Days { get { return mPKRS_Duration; } }
            public int MetLevel { get { return mmetlevel; } }
            public int OT_Gender { get { return motgender; } }

            public bool IsEgg { get { return misegg; } }
            public bool IsNicknamed { get { return misnick; } }
            public bool IsShiny { get { return misshiny; } }

            public ushort HeldItem { get { return mhelditem; } }
            public ushort TID { get { return mTID; } }
            public ushort SID { get { return mSID; } }
            public ushort TSV { get { return mTSV; } }
            public ushort Move1_PP { get { return mmove1_pp; } }
            public ushort Move2_PP { get { return mmove2_pp; } }
            public ushort Move3_PP { get { return mmove3_pp; } }
            public ushort Move4_PP { get { return mmove4_pp; } }
            public ushort Move1_PPUp { get { return mmove1_ppu; } }
            public ushort Move2_PPUp { get { return mmove2_ppu; } }
            public ushort Move3_PPUp { get { return mmove3_ppu; } }
            public ushort Move4_PPUp { get { return mmove4_ppu; } }
            public ushort EggMove1 { get { return meggmove1; } }
            public ushort EggMove2 { get { return meggmove2; } }
            public ushort EggMove3 { get { return meggmove3; } }
            public ushort EggMove4 { get { return meggmove4; } }
            public ushort Checksum { get { return mchk; } }
            public ushort Friendship { get { return mOTfriendship; } }
            public ushort OT_Affection { get { return mOTaffection; } }
            public ushort Egg_Year { get { return megg_year; } }
            public ushort Egg_Day { get { return megg_month; } }
            public ushort Egg_Month { get { return megg_day; } }
            public ushort Met_Year { get { return mmet_year; } }
            public ushort Met_Day { get { return mmet_month; } }
            public ushort Met_Month { get { return mmet_day; } }
            public ushort Egg_Location { get { return meggloc; } }
            public ushort Met_Location { get { return mmetloc; } }
            public ushort Ball { get { return mball; } }
            public ushort Encounter { get { return mencountertype; } }
            public ushort GameVersion { get { return mgamevers; } }
            public ushort CountryID { get { return mcountryID; } }
            public ushort RegionID { get { return mregionID; } }
            public ushort DSRegionID { get { return mdsregID; } }
            public ushort OTLang { get { return motlang; } }

            #endregion
            public PKX(byte[] pkx)
            {
                mnicknamestr = "";
                mnotOT = "";
                mot = "";
                mEC = BitConverter.ToUInt32(pkx, 0);
                mchk = BitConverter.ToUInt16(pkx, 6);
                mspecies = BitConverter.ToUInt16(pkx, 0x08);
                mhelditem = BitConverter.ToUInt16(pkx, 0x0A);
                mTID = BitConverter.ToUInt16(pkx, 0x0C);
                mSID = BitConverter.ToUInt16(pkx, 0x0E);
                mexp = BitConverter.ToUInt32(pkx, 0x10);
                mability = pkx[0x14];
                mabilitynum = pkx[0x15];
                // 0x16, 0x17 - unknown
                mPID = BitConverter.ToUInt32(pkx, 0x18);
                mnature = pkx[0x1C];
                mfeflag = pkx[0x1D] % 2;
                mgenderflag = (pkx[0x1D] >> 1) & 0x3;
                maltforms = (pkx[0x1D] >> 3);
                mHP_EV = pkx[0x1E];
                mATK_EV = pkx[0x1F];
                mDEF_EV = pkx[0x20];
                mSPA_EV = pkx[0x22];
                mSPD_EV = pkx[0x23];
                mSPE_EV = pkx[0x21];
                mcnt_cool = pkx[0x24];
                mcnt_beauty = pkx[0x25];
                mcnt_cute = pkx[0x26];
                mcnt_smart = pkx[0x27];
                mcnt_tough = pkx[0x28];
                mcnt_sheen = pkx[0x29];
                mmarkings = pkx[0x2A];
                mPKRS_Strain = pkx[0x2B] >> 4;
                mPKRS_Duration = pkx[0x2B] % 0x10;

                // Block B
                mnicknamestr = TrimFromZero(Encoding.Unicode.GetString(pkx, 0x40, 24));
                // 0x58, 0x59 - unused
                mmove1 = BitConverter.ToUInt16(pkx, 0x5A);
                mmove2 = BitConverter.ToUInt16(pkx, 0x5C);
                mmove3 = BitConverter.ToUInt16(pkx, 0x5E);
                mmove4 = BitConverter.ToUInt16(pkx, 0x60);
                mmove1_pp = pkx[0x62];
                mmove2_pp = pkx[0x63];
                mmove3_pp = pkx[0x64];
                mmove4_pp = pkx[0x65];
                mmove1_ppu = pkx[0x66];
                mmove2_ppu = pkx[0x67];
                mmove3_ppu = pkx[0x68];
                mmove4_ppu = pkx[0x69];
                meggmove1 = BitConverter.ToUInt16(pkx, 0x6A);
                meggmove2 = BitConverter.ToUInt16(pkx, 0x6C);
                meggmove3 = BitConverter.ToUInt16(pkx, 0x6E);
                meggmove4 = BitConverter.ToUInt16(pkx, 0x70);

                // 0x72 - Super Training Flag - Passed with pkx to new form

                // 0x73 - unused/unknown
                mIV32 = BitConverter.ToUInt32(pkx, 0x74);
                mHP_IV = mIV32 & 0x1F;
                mATK_IV = (mIV32 >> 5) & 0x1F;
                mDEF_IV = (mIV32 >> 10) & 0x1F;
                mSPE_IV = (mIV32 >> 15) & 0x1F;
                mSPA_IV = (mIV32 >> 20) & 0x1F;
                mSPD_IV = (mIV32 >> 25) & 0x1F;
                misegg = Convert.ToBoolean((mIV32 >> 30) & 1);
                misnick = Convert.ToBoolean((mIV32 >> 31));

                // Block C
                mnotOT = TrimFromZero(Encoding.Unicode.GetString(pkx, 0x78, 24));
                bool notOTG = Convert.ToBoolean(pkx[0x92]);
                // Memory Editor edits everything else with pkx in a new form

                // Block D
                mot = TrimFromZero(Encoding.Unicode.GetString(pkx, 0xB0, 24));
                // 0xC8, 0xC9 - unused
                mOTfriendship = pkx[0xCA];
                mOTaffection = pkx[0xCB]; // Handled by Memory Editor
                // 0xCC, 0xCD, 0xCE, 0xCF, 0xD0
                megg_year = pkx[0xD1];
                megg_month = pkx[0xD2];
                megg_day = pkx[0xD3];
                mmet_year = pkx[0xD4];
                mmet_month = pkx[0xD5];
                mmet_day = pkx[0xD6];
                // 0xD7 - unused
                meggloc = BitConverter.ToUInt16(pkx, 0xD8);
                mmetloc = BitConverter.ToUInt16(pkx, 0xDA);
                mball = pkx[0xDC];
                mmetlevel = pkx[0xDD] & 0x7F;
                motgender = (pkx[0xDD]) >> 7;
                mencountertype = pkx[0xDE];
                mgamevers = pkx[0xDF];
                mcountryID = pkx[0xE0];
                mregionID = pkx[0xE1];
                mdsregID = pkx[0xE2];
                motlang = pkx[0xE3];

                if (mgenderflag == 0)
                {
                    mgenderstring = "♂";
                }
                else if (mgenderflag == 1)
                {
                    mgenderstring = "♀";
                }
                else mgenderstring = "-";

                mhptype = (15 * ((mHP_IV & 1) + 2 * (mATK_IV & 1) + 4 * (mDEF_IV & 1) + 8 * (mSPE_IV & 1) + 16 * (mSPA_IV & 1) + 32 * (mSPD_IV & 1))) / 63 + 1;

                mTSV = (ushort)((mTID ^ mSID) >> 4);
                mESV = (ushort)(((mPID >> 16) ^ (mPID & 0xFFFF)) >> 4);

                misshiny = (mTSV == mESV);
                // Nidoran Gender Fixing Text
                if (!Convert.ToBoolean(misnick))
                {
                    if (mnicknamestr.Contains((char)0xE08F))
                    {
                        mnicknamestr = Regex.Replace(mnicknamestr, "\uE08F", "\u2640");
                    }
                    else if (mnicknamestr.Contains((char)0xE08E))
                    {
                        mnicknamestr = Regex.Replace(mnicknamestr, "\uE08E", "\u2642");
                    }
                }
                try
                {
                    mSpeciesName = m_parent.specieslist[mspecies];
                    mNatureName = m_parent.natures[mnature];
                    mHPName = m_parent.types[mhptype];
                    mAbilityName = m_parent.abilitylist[mability];
                    mMove1N = m_parent.movelist[mmove1];
                    mMove2N = m_parent.movelist[mmove2];
                    mMove3N = m_parent.movelist[mmove3];
                    mMove4N = m_parent.movelist[mmove4];
                }
                catch { return; }
            }
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
