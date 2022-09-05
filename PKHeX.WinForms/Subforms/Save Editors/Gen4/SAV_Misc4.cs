using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.WinForms;

public partial class SAV_Misc4 : Form
{
    private readonly SAV4 Origin;
    private readonly SAV4 SAV;
    private readonly Hall4? Hall;

    public SAV_Misc4(SAV4 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        int ofsFlag;
        SAV = (SAV4)(Origin = sav).Clone();

        switch (SAV.Version)
        {
            case GameVersion.D or GameVersion.P or GameVersion.DP:
                ofsFlag = 0xFDC;
                ofsBP = 0x65F8;
                ofsUGFlagCount = 0x3A60;
                L_CurrentMap.Visible = CB_UpgradeMap.Visible = false;
                GB_Prints.Visible = GB_Prints.Enabled = GB_Hall.Visible = GB_Hall.Enabled = GB_Castle.Visible = GB_Castle.Enabled = false;
                BFF = new[] { new[] { 0, 1, 0x5FCA, 0x04, 0x6601 } };
                break;
            case GameVersion.Pt:
                ofsFlag = 0xFEC;
                ofsBP = 0x7234;
                ofsUGFlagCount = 0x3CE8;
                L_CurrentMap.Visible = CB_UpgradeMap.Visible = false;
                ofsPrints = 0xE4A;
                BFF = new[] {
                    new[] { 0, 1, 0x68E0, 0x04, 0x723D },
                    new[] { 1, 0, 0x68F4, 0x10, 0x7EF8 },
                    new[] { 0, 0, 0x6924, 0x18, 0x7EFC },
                    new[] { 2, 0, 0x696C, 0x10, 0x7F00 },
                    new[] { 0, 0, 0x699C, 0x04, 0x7F04 },
                };
                Hall = FetchHallBlock(SAV, 0x2820);
                break;
            case GameVersion.HG or GameVersion.SS or GameVersion.HGSS:
                ofsFlag = 0x10C4;
                ofsBP = 0x5BB8;
                L_UGFlags.Visible = NUD_UGFlags.Visible = false;
                GB_Poketch.Visible = false;
                ofsMap = 0xBAE7;
                ofsPrints = 0xE7E;
                BFF = new[] {
                    // { BFV, BFT, addr, 1BFTlen, checkBit
                    new[] { 0, 1, 0x5264, 0x04, 0x5BC1 },
                    new[] { 1, 0, 0x5278, 0x10, 0x687C },
                    new[] { 0, 0, 0x52A8, 0x18, 0x6880 },
                    new[] { 2, 0, 0x52F0, 0x10, 0x6884 },
                    new[] { 0, 0, 0x5320, 0x04, 0x6888 },
                };
                Hall = FetchHallBlock(SAV, 0x230C);
                break;
            default: return;
        }
        ofsFly = ofsFlag + 0x136;
        ReadMain();
        ReadBattleFrontier();
        if (SAV is SAV4Sinnoh s)
        {
            TC_Misc.Controls.Remove(TAB_Walker);
            poffinCase4Editor1.Initialize(s);
            TC_Misc.Controls.Remove(Tab_PokeGear);
        }
        else
        {
            pokeGear4Editor1.Initialize((SAV4HGSS)SAV);
            TC_Misc.Controls.Remove(Tab_Poffins);
        }
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveMain();
        SaveBattleFrontier();
        if (SAV is SAV4HGSS)
            pokeGear4Editor1.Save();
        else if (SAV is SAV4Sinnoh)
            poffinCase4Editor1.Save();

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private readonly int ofsFly;
    private readonly int ofsBP;
    private readonly int ofsMap = -1;
    private readonly int ofsUGFlagCount = -1;
    private int[] FlyDestC = null!;

    private void ReadMain()
    {
        NUD_Coin.Value = SAV.Coin;
        NUD_Coin.Maximum = SAV.MaxCoins;
        int[] FlyDestD;
        IReadOnlyList<ComboItem> metLocationList;
        switch (SAV)
        {
            case SAV4Sinnoh:
                metLocationList = GameInfo.GetLocationList(GameVersion.Pt, EntityContext.Gen4, false);
                FlyDestD = new[] { 001, 002, 006, 008, 003, 009, 010, 004, 012, 011, 005, 007, 014, 013, 054, 015, 081, 082, 083, 055 };
                FlyDestC = new[] { 000, 001, 007, 009, 002, 010, 011, 003, 013, 012, 004, 008, 015, 014, 016, 068, 017, 005, 006, 067 };
                break;
            case SAV4HGSS:
                metLocationList = GameInfo.GetLocationList(GameVersion.HG, EntityContext.Gen4, false);
                FlyDestD = new[] { 126, 127, 128, 129, 131, 133, 132, 130, 134, 135, 136, 227, 229, 137, 221, 147, 138, 139, 140, 141, 143, 142, 144, 148, 145, 146, 225 };
                FlyDestC = new[] { 011, 012, 013, 014, 016, 018, 017, 015, 019, 020, 021, 030, 027, 022, 033, 009, 000, 001, 002, 003, 005, 004, 006, 010, 007, 008, 035 };
                break;
            default: return;
        }
        uint valFly = ReadUInt32LittleEndian(SAV.General.AsSpan(ofsFly));
        CLB_FlyDest.Items.Clear();
        for (int i = 0; i < FlyDestD.Length; i++)
        {
            var dest = FlyDestD[i];
            var name = metLocationList.First(v => v.Value == dest).Text;
            var state = FlyDestC[i] < 32
                ? (valFly & (1u << FlyDestC[i])) != 0
                : (SAV.General[ofsFly + (FlyDestC[i] >> 3)] & (1 << (FlyDestC[i] & 7))) != 0;
            CLB_FlyDest.Items.Add(name, state);
        }
        uint valBP = ReadUInt16LittleEndian(SAV.General.AsSpan(ofsBP));
        NUD_BP.Value = valBP > 9999 ? 9999 : valBP;

        if (SAV is SAV4Sinnoh sinnoh)
            ReadPoketch(sinnoh);
        else if (SAV is SAV4HGSS hgss)
            ReadWalker(hgss);

        if (ofsUGFlagCount > 0)
        {
            uint fc = ReadUInt32LittleEndian(SAV.General.AsSpan(ofsUGFlagCount)) & 0xFFFFF;
            NUD_UGFlags.Value = fc > 999999 ? 999999 : fc;
        }
        if (ofsMap > 0)
        {
            string[] items = { "Map Johto", "Map Johto+", "Map Johto & Kanto" };
            int index = (SAV.General[ofsMap] >> 3) & 3;
            if (index > 2) index = 2;
            CB_UpgradeMap.Items.AddRange(items);
            CB_UpgradeMap.SelectedIndex = index;
        }
    }

    private void SaveMain()
    {
        SAV.Coin = (uint)NUD_Coin.Value;
        uint valFly = ReadUInt32LittleEndian(SAV.General.AsSpan(ofsFly));
        for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
        {
            if (FlyDestC[i] < 32)
            {
                if (CLB_FlyDest.GetItemChecked(i))
                    valFly |= (uint) 1 << FlyDestC[i];
                else
                    valFly &= ~((uint) 1 << FlyDestC[i]);
            }
            else
            {
                var o = ofsFly + (FlyDestC[i] >> 3);
                SAV.General[o] = (byte)((SAV.General[o] & ~(1 << (FlyDestC[i] & 7))) | (CLB_FlyDest.GetItemChecked(i) ? 1 << (FlyDestC[i] & 7) : 0));
            }
        }
        WriteUInt32LittleEndian(SAV.General.AsSpan(ofsFly), valFly);
        WriteUInt16LittleEndian(SAV.General.AsSpan(ofsBP), (ushort)NUD_BP.Value);

        if (SAV is SAV4Sinnoh sinnoh)
            SavePoketch(sinnoh);
        if (SAV is SAV4HGSS hgss)
            SaveWalker(hgss);

        if (ofsUGFlagCount > 0)
        {
            var current = ReadUInt32LittleEndian(SAV.General.AsSpan(ofsUGFlagCount)) & ~0xFFFFFu;
            var update = current | (uint)NUD_UGFlags.Value;
            WriteUInt32LittleEndian(SAV.General.AsSpan(ofsUGFlagCount), update);
        }
        if (ofsMap > 0)
        {
            int valMap = CB_UpgradeMap.SelectedIndex;
            if (valMap >= 0)
                SAV.General[ofsMap] = (byte)((SAV.General[ofsMap] & 0xE7) | (valMap << 3));
        }
    }

    private void B_AllFlyDest_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            CLB_FlyDest.SetItemChecked(i, true);
    }

    #region Poketch
    private byte[] DotArtistByte = null!;
    private byte[] ColorTable = null!;

    private void ReadPoketch(SAV4Sinnoh s)
    {
        string[] PoketchTitle = Enum.GetNames(typeof(PoketchApp));

        CB_CurrentApp.Items.AddRange(PoketchTitle);
        CB_CurrentApp.SelectedIndex = s.CurrentPoketchApp;
        CLB_Poketch.Items.Clear();
        for (int i = 0; i < PoketchTitle.Length; i++)
        {
            var title = $"{i:00} - {PoketchTitle[i]}";
            var value = s.GetPoketchAppUnlocked((PoketchApp)i);
            CLB_Poketch.Items.Add(title, value);
        }

        DotArtistByte = s.GetPoketchDotArtistData();
        ColorTable = new byte[] { 248, 168, 88, 8 };
        SetPictureBoxFromFlags(DotArtistByte);
        string tip = "Guide about D&D ImageFile Format";
        tip += Environment.NewLine + " width = 24px";
        tip += Environment.NewLine + " height = 20px";
        tip += Environment.NewLine + " used color count <= 4";
        tip += Environment.NewLine + " file size < 2058byte";
        tip1.SetToolTip(PB_DotArtist, tip);
        TAB_Main.AllowDrop = true;
    }

    private void SavePoketch(SAV4Sinnoh s)
    {
        s.CurrentPoketchApp = (sbyte)CB_CurrentApp.SelectedIndex;
        for (int i = 0; i < CLB_Poketch.Items.Count; i++)
        {
            var b = CLB_Poketch.GetItemChecked(i);
            s.SetPoketchAppUnlocked((PoketchApp)i, b);
        }
        s.SetPoketchDotArtistData(DotArtistByte);
    }

    private void SetPictureBoxFromFlags(ReadOnlySpan<byte> inp)
    {
        if (inp.Length != 120)
            return;
        PB_DotArtist.Image = GetDotArt(inp);
    }

    private Bitmap GetDotArt(ReadOnlySpan<byte> inp)
    {
        byte[] dupbyte = new byte[23040];
        for (int iy = 0; iy < 20; iy++)
        {
            for (int ix = 0; ix < 24; ix++)
            {
                var ib = ix + (24 * iy);
                var ict = ColorTable[(inp[ib >> 2] >> ((ib % 4) << 1)) & 3];
                var iz = (12 * ix) + (0x480 * iy);
                for (int izy = 0; izy < 4; izy++)
                {
                    for (int izx = 0; izx < 4; izx++)
                    {
                        for (int ic = 0; ic < 3; ic++)
                            dupbyte[ic + (3 * izx) + (0x120 * izy) + iz] = ict;
                    }
                }
            }
        }

        Bitmap dabmp = new(96, 80);
        BitmapData dabdata = dabmp.LockBits(new Rectangle(0, 0, dabmp.Width, dabmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        System.Runtime.InteropServices.Marshal.Copy(dupbyte, 0, dabdata.Scan0, dupbyte.Length);
        dabmp.UnlockBits(dabdata);
        return dabmp;
    }

    private void SetFlagsFromFileName(string inpFileName)
    {
        if (FileUtil.GetFileSize(inpFileName) > 2058)
            return; // 24*20*4(ARGB)=1920

        Bitmap bmp; try { bmp = (Bitmap)Image.FromFile(inpFileName); }
        catch { return; }

        if (bmp.Width != 24 || bmp.Height != 20)
            return;

        Span<byte> brightMap = stackalloc byte[480];
        Span<byte> brightCount = stackalloc byte[0x100];
        Span<byte> iBrightCount = stackalloc byte[0x100];
        for (int iy = 0; iy < 20; iy++)
        {
            for (int ix = 0; ix < 24; ix++)
            {
                var ig = (byte)(0xFF * bmp.GetPixel(ix, iy).GetBrightness());
                brightMap[ix + (24 * iy)] = ig;
                brightCount[ig]++;
            }
        }

        int colorCount = 0;
        foreach (var value in brightCount)
        {
            if (value > 0)
                ++colorCount;
        }

        if (colorCount is 0 or > 4)
            return;
        int errorsMin = int.MaxValue;
        Span<byte> LCT = stackalloc byte[4];
        Span<byte> mLCT = stackalloc byte[4];
        for (int i = 0; i < 4; i++)
            LCT[i] = (byte)(colorCount < i + 1 ? 4 : colorCount - i - 1);
        int ee = 0;
        while (++ee < 1000)
        {
            brightCount.CopyTo(iBrightCount);
            for (int i = 0, j = 0; i < 0x100; i++)
            {
                if (iBrightCount[i] > 0)
                    iBrightCount[i] = LCT[j++];
            }

            var errorsTotal = 0;
            for (int i = 0; i < 480; i++)
                errorsTotal += Math.Abs(brightMap[i] - ColorTable[iBrightCount[brightMap[i]]]);
            if (errorsMin > errorsTotal)
            {
                errorsMin = errorsTotal;
                LCT.CopyTo(mLCT);
            }
            GetNextLCT(LCT);
            if (LCT[0] >= 4)
                break;
        }
        for (int i = 0, j = 0; i < 0x100; i++)
        {
            if (brightCount[i] > 0)
                brightCount[i] = mLCT[j++];
        }

        for (int i = 0; i < 480; i++)
            brightMap[i] = brightCount[brightMap[i]];

        Span<byte> ndab = stackalloc byte[120];
        for (int i = 0; i < 480; i++)
            ndab[i >> 2] |= (byte)((brightMap[i] & 3) << ((i % 4) << 1));

        ndab.CopyTo(DotArtistByte.AsSpan(0));
    }

    private static void GetNextLCT(Span<byte> inp)
    {
        while (true)
        {
            if (++inp[0] < 4)
                continue;

            inp[0] = 0;
            if (++inp[1] < 4)
                continue;

            inp[1] = 0;
            if (++inp[2] < 4)
                continue;

            inp[2] = 0;
            if (++inp[3] < 4)
                continue;

            inp[0] = 4;
            return;
        }
    }

    private void SetFlagsFromClickPoint(int inpX, int inpY)
    {
        static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        inpX = Clamp(inpX, 0, 95);
        inpY = Clamp(inpY, 0, 79);
        int i = (inpX >> 2) + (24 * (inpY >> 2));
        byte[] ndab = new byte[120];
        DotArtistByte.CopyTo(ndab, 0);
        byte c = (byte)((ndab[i >> 2] >> ((i % 4) << 1)) & 3);
        if (++c >= 4) c = 0;
        ndab[i >> 2] &= (byte)~(3 << ((i % 4) << 1));
        ndab[i >> 2] |= (byte)((c & 3) << ((i % 4) << 1));
        ndab.CopyTo(DotArtistByte, 0);
    }

    private void B_GiveAll_Click(object sender, EventArgs e)
    {
        // foreach (CheckBox c in Apps) c.Checked = true;
        for (int i = 0; i < CLB_Poketch.Items.Count; i++)
            CLB_Poketch.SetItemChecked(i, true);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void TAB_Poketch_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e?.Data is null)
            return;
        if (TAB_Main.AllowDrop && e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
    }

    private void TAB_Poketch_DragDrop(object? sender, DragEventArgs? e)
    {
        if (!TAB_Main.AllowDrop)
            return;
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;
        SetFlagsFromFileName(files[0]);
        SetPictureBoxFromFlags(DotArtistByte);
    }

    private void PB_DotArtist_MouseClick(object sender, MouseEventArgs e)
    {
        SetFlagsFromClickPoint(e.X, e.Y);
        SetPictureBoxFromFlags(DotArtistByte);
    }
    #endregion

    #region BattleFrontier
    private int[] Prints = null!;
    private readonly int ofsPrints = -1;
    private Color[] PrintColorA = null!;
    private Button[] PrintButtonA = null!;
    private bool editing;
    private RadioButton[] StatRBA = null!;
    private NumericUpDown[] StatNUDA = null!;
    private Label[] StatLabelA = null!;
    private readonly int[][] BFF = null!;
    private string[][] BFT = null!;
    private int[][] BFV = null!;
    private string[] BFN = null!;
    private NumericUpDown[] HallNUDA = null!;
    private bool HallStatUpdated;

    private void ReadBattleFrontier()
    {
        BFV = new[] {
            new[] { 2, 0 }, // Max, Current
            new[] { 2, 0, 3, 1 }, // Max, Current, Max(Trade), Current(Trade)
            new[] { 2, 0, 1, -1, 3 }, // Max, Current, Current(CP), (UsedCP), Max(CP)
        };
        BFT = new[] {
            new[] { "Singles", "Doubles", "Multi" },
            new[] { "Singles", "Doubles", "Multi (Trainer)", "Multi (Friend)", "Wi-Fi" },
        };
        BFN = new[] { "Tower", "Factory", "Hall", "Castle", "Arcade" };
        if (SAV is SAV4DP) BFN = BFN.Take(1).ToArray();
        StatNUDA = new[] { NUD_Stat0, NUD_Stat1, NUD_Stat2, NUD_Stat3 };
        StatLabelA = new[] { L_Stat0, L_Stat1, L_Stat2, L_Stat3 };
        StatRBA = new[] { RB_Stats3_01, RB_Stats3_02 };

        if (ofsPrints > 0)
        {
            PrintColorA = new[] { Color.Transparent, Color.Silver, Color.Gold };
            PrintButtonA = new[] { BTN_PrintTower, BTN_PrintFactory, BTN_PrintHall, BTN_PrintCastle, BTN_PrintArcade };
            Prints = new int[PrintButtonA.Length];
            for (int i = 0; i < Prints.Length; i++)
                Prints[i] = 1 + Math.Sign((ReadUInt16LittleEndian(SAV.General.AsSpan(ofsPrints + (i << 1))) >> 1) - 1);
            SetPrints();

            HallNUDA = new[] {
                NUD_HallType01, NUD_HallType02, NUD_HallType03, NUD_HallType04, NUD_HallType05, NUD_HallType06,
                NUD_HallType07, NUD_HallType08, NUD_HallType09, NUD_HallType10, NUD_HallType11, NUD_HallType12,
                NUD_HallType13, NUD_HallType14, NUD_HallType15, NUD_HallType16, NUD_HallType17,
            };
            string[] typeNames = GameInfo.Strings.types;
            int[] typenameIndex = { 0, 9, 10, 12, 11, 14, 1, 3, 4, 2, 13, 6, 5, 7, 15, 16, 8 };
            for (int i = 0; i < HallNUDA.Length; i++)
                tip2.SetToolTip(HallNUDA[i], typeNames[typenameIndex[i]]);
        }
        if (Hall is null)
            NUD_HallStreaks.Visible = NUD_HallStreaks.Enabled = false;

        editing = true;
        CB_Stats1.Items.Clear();
        foreach (string t in BFN)
            CB_Stats1.Items.Add(t);
        StatRBA[0].Checked = true;

        // Clear Listbox and ComboBox
        CB_Species.Items.Clear();

        // Fill List
        CB_Species.InitializeBinding();

        var speciesList = GameInfo.FilteredSources.Species.Skip(1).ToList();
        CB_Species.DataSource = new BindingSource(speciesList, null);

        editing = false;
        CB_Stats1.SelectedIndex = 0;
    }

    private static Hall4? FetchHallBlock(SAV4 sav, int magicKeyOffset)
    {
        for (int i = 0; i < 2; i++, magicKeyOffset += 0x14)
        {
            var h = ReadInt32LittleEndian(sav.General.AsSpan(magicKeyOffset));
            if (h == -1)
                continue;

            for (int j = 0; j < 0x20; j++)
            {
                for (int k = 0, a = (j + 0x20) << 12; k < 2; k++, a += 0x40000)
                {
                    var span = sav.Data.AsSpan(a);
                    if (h != ReadInt32LittleEndian(span))
                        continue;
                    if (ReadInt16LittleEndian(span[0xBA8..]) != 0xBA0)
                        continue;
                    return new Hall4(sav.Data, a);
                }
            }
        }

        return null;
    }

    private void SaveBattleFrontier()
    {
        if (ofsPrints > 0)
        {
            for (int i = 0; i < Prints.Length; i++)
            {
                if (Prints[i] == 1 + Math.Sign((ReadUInt16LittleEndian(SAV.General.AsSpan(ofsPrints + (i << 1))) >> 1) - 1))
                    continue;
                var value = Prints[i] << 1;
                WriteInt32LittleEndian(SAV.General.AsSpan(ofsPrints + (i << 1)), value);
            }
        }

        if (HallStatUpdated)
            Hall?.RefreshChecksum();
    }

    private void SetPrints()
    {
        for (int i = 0; i < PrintButtonA.Length; i++)
            PrintButtonA[i].BackColor = PrintColorA[Prints[i]];
    }

    private void BTN_Print_Click(object sender, EventArgs e)
    {
        int index = Array.IndexOf(PrintButtonA, sender);
        if (index < 0)
            return;
        Prints[index] = (Prints[index] + 1) % 3;
        SetPrints();
    }

    private void ChangeStat1(object sender, EventArgs e)
    {
        if (editing)
            return;
        int facility = CB_Stats1.SelectedIndex;
        if (facility < 0)
            return;

        editing = true;
        CB_Stats2.Items.Clear();
        CB_Stats2.Items.AddRange(BFT[BFF[facility][1]]);

        StatRBA[0].Checked = true;
        foreach (RadioButton rb in StatRBA)
            rb.Visible = rb.Enabled = facility == 1;

        for (int i = 0; i < StatLabelA.Length; i++)
            StatLabelA[i].Visible = StatLabelA[i].Enabled = StatNUDA[i].Visible = StatNUDA[i].Enabled = Array.IndexOf(BFV[BFF[facility][0]], i) >= 0;
        if (facility == 0)
        {
            StatLabelA[1].Visible = StatLabelA[1].Enabled = StatNUDA[1].Visible = StatNUDA[1].Enabled = true;
            StatLabelA[1].Text = "Continue";
            StatNUDA[1].Maximum = 65535;
        }
        else
        {
            if (StatNUDA[1].Value > 9999) StatNUDA[1].Value = 9999;
            StatNUDA[1].Maximum = 9999;
        }
        if (facility == 1) StatLabelA[1].Text = StatLabelA[3].Text = "Trade";
        if (facility == 3) StatLabelA[1].Text = StatLabelA[3].Text = "CP";
        GB_Hall.Visible = facility == 2;
        GB_Castle.Visible = facility == 3;

        editing = false;
        CB_Stats2.SelectedIndex = 0;
    }

    private void ChangeStat(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (sender is RadioButton {Checked: false})
            return;
        StatAddrControl(SetValToSav: -2, SetSavToVal: true);
        if (GB_Hall.Visible)
        {
            GB_Hall.Text = $"Battle Hall ({(string) CB_Stats2.SelectedItem})";
            editing = true;
            GetHallStat();
            editing = false;
        }
        else if (GB_Castle.Visible)
        {
            GB_Castle.Text = $"Battle Castle ({(string) CB_Stats2.SelectedItem})";
            editing = true;
            GetCastleStat();
            editing = false;
        }
    }

    private void StatAddrControl(int SetValToSav = -2, bool SetSavToVal = false)
    {
        int Facility = CB_Stats1.SelectedIndex;
        int BattleType = CB_Stats2.SelectedIndex;
        int RBi = StatRBA[1].Checked ? 1 : 0;
        int addrVal = BFF[Facility][2] + (BFF[Facility][3] * BattleType) + (RBi << 3);
        int addrFlag = BFF[Facility][4];
        byte maskFlag = (byte)(1 << (BattleType + (RBi << 2)));
        int TowerContinueCountOfs = SAV is SAV4DP ? 3 : 1;

        if (SetSavToVal)
        {
            editing = true;
            for (int i = 0; i < BFV[BFF[Facility][0]].Length; i++)
            {
                if (BFV[BFF[Facility][0]][i] < 0) continue;
                int vali = ReadUInt16LittleEndian(SAV.General.AsSpan(addrVal + (i << 1)));
                StatNUDA[BFV[BFF[Facility][0]][i]].Value = vali > 9999 ? 9999 : vali;
            }
            CHK_Continue.Checked = (SAV.General[addrFlag] & maskFlag) != 0;

            if (Facility == 0) // tower continue count
                StatNUDA[1].Value = ReadUInt16LittleEndian(SAV.General.AsSpan(addrFlag + TowerContinueCountOfs + (BattleType << 1)));

            editing = false;
            return;
        }
        if (SetValToSav >= 0)
        {
            ushort val = (ushort)StatNUDA[SetValToSav].Value;

            if (Facility == 0 && SetValToSav == 1) // tower continue count
            {
                var offset = addrFlag + TowerContinueCountOfs + (BattleType << 1);
                WriteUInt16LittleEndian(SAV.General.AsSpan(offset), val);
            }

            SetValToSav = Array.IndexOf(BFV[BFF[Facility][0]], SetValToSav);
            if (SetValToSav < 0)
                return;
            var clamp = Math.Min((ushort)9999, val);
            WriteUInt16LittleEndian(SAV.General.AsSpan(addrVal + (SetValToSav << 1)), clamp);
            return;
        }
        if (SetValToSav == -1)
        {
            if (CHK_Continue.Checked)
            {
                SAV.General[addrFlag] |= maskFlag;
                if (Facility == 3) SAV.General[addrFlag + 1] |= 0x01; // not found what this flag means
            }
            else
            {
                SAV.General[addrFlag] &= (byte)~maskFlag;
            }
        }
    }

    private void ChangeStatVal(object sender, EventArgs e)
    {
        if (editing)
            return;
        int n = Array.IndexOf(StatNUDA, sender);
        if (n < 0)
            return;
        StatAddrControl(SetValToSav: n, SetSavToVal: false);

        if (CB_Stats1.SelectedIndex == 0 && Math.Floor(StatNUDA[0].Value / 7) != StatNUDA[1].Value)
        {
            if (n == 0)
            {
                StatNUDA[1].Value = Math.Floor(StatNUDA[0].Value / 7);
            }
            else if (n == 1)
            {
                if (StatNUDA[0].Maximum > StatNUDA[1].Value * 7)
                    StatNUDA[0].Value = StatNUDA[1].Value * 7;
                else if (StatNUDA[0].Value < StatNUDA[0].Maximum)
                    StatNUDA[0].Value = StatNUDA[0].Maximum;
            }
        }
    }

    private void CHK_Continue_CheckedChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        StatAddrControl(SetValToSav: -1, SetSavToVal: false);
    }

    private ushort species = ushort.MaxValue;

    private void ChangeSpecies(object sender, EventArgs e)
    {
        species = (ushort)CB_Species.SelectedValue;
        if (editing)
            return;
        editing = true;
        GetHallStat();
        editing = false;
    }

    private void GetCastleStat()
    {
        int ofs = BFF[3][2] + (BFF[3][3] * CB_Stats2.SelectedIndex) + 0x0A;
        NumericUpDown[] na = { NUD_CastleRankRcv, NUD_CastleRankItem, NUD_CastleRankInfo };
        for (int i = 0; i < na.Length; i++)
        {
            int val = ReadInt16LittleEndian(SAV.General.AsSpan(ofs + (i << 1)));
            na[i].Value = val > na[i].Maximum ? na[i].Maximum : val < na[i].Minimum ? na[i].Minimum : val;
        }
    }

    private void NUD_CastleRank_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        NumericUpDown[] na = { NUD_CastleRankRcv, NUD_CastleRankItem, NUD_CastleRankInfo };
        int i = Array.IndexOf(na, sender);
        if (i < 0)
            return;
        var offset = BFF[3][2] + (BFF[3][3] * CB_Stats2.SelectedIndex) + 0x0A + (i << 1);
        WriteInt32LittleEndian(SAV.General.AsSpan(offset), (int)na[i].Value);
    }

    private void GetHallStat()
    {
        int ofscur = BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex);
        var curspe = ReadUInt16LittleEndian(SAV.General.AsSpan(ofscur + 4));
        bool c = curspe == species;
        CHK_HallCurrent.Checked = c;
        CHK_HallCurrent.Text = curspe > 0 && curspe <= SAV.MaxSpeciesID
            ? $"Current: {SpeciesName.GetSpeciesName(curspe, GameLanguage.GetLanguageIndex(Main.CurrentLanguage))}"
            : "Current: (none)";

        int s = 0;
        for (int i = 0; i < HallNUDA.Length; i++)
        {
            var d = c ? Math.Min(10, (SAV.General[ofscur + 6 + ((i >> 1) << 1)] >> ((i & 1) << 2)) & 0x0F) : 0;
            HallNUDA[i].Value = d;
            HallNUDA[i].Enabled = c;
            s += d;
        }
        L_SumHall.Text = s.ToString();

        if (Hall is not null)
        {
            ushort v = Hall.GetCount(CB_Stats2.SelectedIndex, species);
            NUD_HallStreaks.Value = Math.Min((ushort)9999, v);
        }
    }

    private void CHK_HallCurrent_CheckedChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        var offset = BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex) + 4;
        ushort value = (ushort)(CHK_HallCurrent.Checked ? species : 0);
        WriteUInt16LittleEndian(SAV.General.AsSpan(offset), value);
        editing = true;
        GetHallStat();
        editing = false;
    }

    private void NUD_HallType_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        int i = Array.IndexOf(HallNUDA, sender);
        if (i < 0)
            return;
        int ofs = BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex) + 6 + ((i >> 1) << 1);
        SAV.General[ofs] = (byte)((SAV.General[ofs] & ~(0xF << ((i & 1) << 2))) | ((int)HallNUDA[i].Value << ((i & 1) << 2)));
        L_SumHall.Text = HallNUDA.Sum(x => x.Value).ToString(CultureInfo.InvariantCulture);
    }

    private void NUD_HallStreaks_ValueChanged(object sender, EventArgs e)
    {
        if (editing || Hall is null)
            return;
        Hall.SetCount(CB_Stats2.SelectedIndex, species, (ushort)NUD_HallStreaks.Value);
        HallStatUpdated = true;
    }
    #endregion

    private void ReadWalker(SAV4HGSS s)
    {
        string[] walkercourses = GameInfo.Sources.Strings.walkercourses;
        bool[] isChecked = s.GetPokewalkerCoursesUnlocked();
        CLB_WalkerCourses.Items.Clear();
        for (int i = 0; i < walkercourses.Length; i++)
            CLB_WalkerCourses.Items.Add(walkercourses[i], isChecked[i]);
        NUD_Watts.Value = s.PokewalkerWatts;
        NUD_Steps.Value = s.PokewalkerSteps;
    }

    private void SaveWalker(SAV4HGSS s)
    {
        Span<bool> courses = stackalloc bool[32];
        for (int i = 0; i < CLB_WalkerCourses.Items.Count; i++)
            courses[i] = CLB_WalkerCourses.GetItemChecked(i);
        s.SetPokewalkerCoursesUnlocked(courses);
        s.PokewalkerWatts = (uint)NUD_Watts.Value;
        s.PokewalkerSteps = (uint)NUD_Steps.Value;
    }

    private void B_AllWalkerCourses_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < CLB_WalkerCourses.Items.Count; i++)
            CLB_WalkerCourses.SetItemChecked(i, true);
    }

    private void OnBAllSealsLegalOnClick(object sender, EventArgs e)
    {
        SAV.SetAllSeals(99, sender == B_AllSealsIllegal);
        System.Media.SystemSounds.Asterisk.Play();
    }
}
