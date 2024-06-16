using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using PKHeX.WinForms.Properties;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_MysteryGiftDB : Form
{
    private readonly PKMEditor PKME_Tabs;
    private readonly SaveFile SAV;
    private readonly SAVEditor BoxView;
    private readonly SummaryPreviewer ShowSet = new();
    private readonly EntityInstructionBuilder UC_Builder;

    private const int GridWidth = 6;
    private const int GridHeight = 11;

    public SAV_MysteryGiftDB(PKMEditor tabs, SAVEditor sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        UC_Builder = new EntityInstructionBuilder(() => tabs.PreparePKM())
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Width = Tab_Advanced.Width,
            Dock = DockStyle.Top,
            ReadOnly = true,
        };
        Tab_Advanced.Controls.Add(UC_Builder);
        UC_Builder.SendToBack();

        SAV = sav.SAV;
        BoxView = sav;
        PKME_Tabs = tabs;

        // Preset Filters to only show PKM available for loaded save
        CB_FormatComparator.SelectedIndex = 3; // <=

        var grid = MysteryPokeGrid;
        var smallWidth = grid.Width;
        var smallHeight = grid.Height;
        grid.InitializeGrid(GridWidth, GridHeight, SpriteUtil.Spriter);
        grid.SetBackground(Resources.box_wp_clean);
        var newWidth = grid.Width;
        var newHeight = grid.Height;
        var wDelta = newWidth - smallWidth;
        if (wDelta != 0)
            Width += wDelta;
        var hDelta = newHeight - smallHeight;
        if (hDelta != 0)
            Height += hDelta;

        PKXBOXES = [.. grid.Entries];

        // Enable Scrolling when hovered over
        foreach (var slot in PKXBOXES)
        {
            // Enable Click
            slot.MouseClick += (_, e) =>
            {
                if (ModifierKeys == Keys.Control)
                    ClickView(slot, e);
            };

            slot.ContextMenuStrip = mnu;
            slot.MouseEnter += (_, _) => ShowHoverTextForSlot(slot);
            slot.Enter += (_, _) =>
            {
                var index = Array.IndexOf(PKXBOXES, slot);
                if (index < 0)
                    return;
                index += (SCR_Box.Value * RES_MIN);
                if (index >= Results.Count)
                    return;

                var enc = Results[index];
                slot.AccessibleDescription = string.Join(Environment.NewLine, enc.GetTextLines());
            };
        }

        Counter = L_Count.Text;
        Viewed = L_Viewed.Text;
        L_Viewed.Text = string.Empty; // invis for now
        L_Viewed.MouseEnter += (_, _) => hover.SetToolTip(L_Viewed, L_Viewed.Text);

        // Load Data
        B_Search.Enabled = false;
        L_Count.Text = "Loading...";
        new Task(LoadDatabase).Start();

        CB_Format.Items[0] = MsgAny;
        CenterToParent();
    }

    private readonly PictureBox[] PKXBOXES;
    private readonly string DatabasePath = Main.MGDatabasePath;
    private List<MysteryGift> Results = [];
    private List<MysteryGift> RawDB = [];
    private int slotSelected = -1; // = null;
    private Image? slotColor;
    private const int RES_MIN = GridWidth * 1;
    private const int RES_MAX = GridWidth * GridHeight;
    private readonly string Counter;
    private readonly string Viewed;
    private const int MAXFORMAT = PKX.Generation;

    private bool GetShiftedIndex(ref int index)
    {
        if (index >= RES_MAX)
            return false;
        index += SCR_Box.Value * RES_MIN;
        return index < Results.Count;
    }

    // Important Events
    private void ClickView(object sender, EventArgs e)
    {
        int index = GetSenderIndex(sender);
        if (index < 0)
            return;
        var temp = Results[index].ConvertToPKM(SAV);
        var pk = EntityConverter.ConvertToType(temp, SAV.PKMType, out var c);
        if (pk == null)
        {
            WinFormsUtil.Error(c.GetDisplayString(temp, SAV.PKMType));
            return;
        }
        SAV.AdaptPKM(pk);
        PKME_Tabs.PopulateFields(pk, false);
        slotSelected = index;
        slotColor = SpriteUtil.Spriter.View;
        UpdateSlotColor(SCR_Box.Value);
        L_Viewed.Text = string.Format(Viewed, Results[index].FileName);
    }

    private void ClickSavePK(object sender, EventArgs e)
    {
        int index = GetSenderIndex(sender);
        if (index < 0)
            return;
        var gift = Results[index];
        var pk = gift.ConvertToPKM(SAV);
        WinFormsUtil.SavePKMDialog(pk);
    }

    private void ClickSaveMG(object sender, EventArgs e)
    {
        int index = GetSenderIndex(sender);
        if (index < 0)
            return;
        var gift = Results[index];
        if (gift is not DataMysteryGift g) // e.g. WC3
        {
            WinFormsUtil.Alert(MsgExportWC3DataFail);
            return;
        }
        WinFormsUtil.ExportMGDialog(g);
    }

    private int GetSenderIndex(object sender)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        int index = Array.IndexOf(PKXBOXES, pb);
        if (index >= RES_MAX)
        {
            System.Media.SystemSounds.Exclamation.Play();
            return -1;
        }
        index += SCR_Box.Value * RES_MIN;
        if (index >= Results.Count)
        {
            System.Media.SystemSounds.Exclamation.Play();
            return -1;
        }
        return index;
    }

    private void PopulateComboBoxes()
    {
        // Set the Text
        CB_HeldItem.InitializeBinding();
        CB_Species.InitializeBinding();

        var comboAny = new ComboItem(MsgAny, -1);

        var species = new List<ComboItem>(GameInfo.SpeciesDataSource);
        species.RemoveAt(0);
        var filteredSpecies = species.Where(z => RawDB.Any(mg => mg.Species == z.Value)).ToList();
        filteredSpecies.Insert(0, comboAny);
        CB_Species.DataSource = filteredSpecies;

        var items = new List<ComboItem>(GameInfo.ItemDataSource);
        items.Insert(0, comboAny); CB_HeldItem.DataSource = items;

        // Set the Move ComboBoxes too.
        var moves = new List<ComboItem>(GameInfo.MoveDataSource);
        moves.RemoveAt(0); moves.Insert(0, comboAny);
        {
            ComboBox[] arr = [CB_Move1, CB_Move2, CB_Move3, CB_Move4];
            foreach (var cb in arr)
            {
                cb.InitializeBinding();
                cb.DataSource = new BindingSource(moves, null);
            }
        }

        // Trigger a Reset
        ResetFilters(this, EventArgs.Empty);
        B_Search.Enabled = true;
    }

    private void ResetFilters(object sender, EventArgs e)
    {
        CHK_Shiny.Checked = CHK_IsEgg.Checked = true;
        CHK_Shiny.CheckState = CHK_IsEgg.CheckState = CheckState.Indeterminate;
        CB_HeldItem.SelectedIndex = 0;
        CB_Species.SelectedIndex = 0;

        CB_Move1.SelectedIndex = CB_Move2.SelectedIndex = CB_Move3.SelectedIndex = CB_Move4.SelectedIndex = 0;
        RTB_Instructions.Clear();

        if (sender != this)
            System.Media.SystemSounds.Asterisk.Play();
    }

    private static Func<MysteryGift, bool> IsPresent<TTable>(TTable pt) where TTable : IPersonalTable => z => pt.IsPresentInGame(z.Species, z.Form);

    private void LoadDatabase()
    {
        var db = EncounterEvent.GetAllEvents();

        if (Main.Settings.MysteryDb.FilterUnavailableSpecies)
        {
            db = SAV switch
            {
                SAV9SV s9 => db.Where(IsPresent(s9.Personal)),
                SAV8SWSH s8 => db.Where(IsPresent(s8.Personal)),
                SAV8BS b8 => db.Where(IsPresent(b8.Personal)),
                SAV8LA a8 => db.Where(IsPresent(a8.Personal)),
                SAV7b => db.Where(z => z is WB7),
                SAV7 => db.Where(z => z.Generation < 7 || z is WC7),
                _ => db.Where(z => z.Generation <= SAV.Generation),
            };
        }

        RawDB = [..db];
        foreach (var mg in RawDB)
            mg.GiftUsed = false;

        try
        {
            while (!IsHandleCreated) { }
            BeginInvoke(new MethodInvoker(delegate
            {
                SetResults(RawDB);
                PopulateComboBoxes();
            }));
        }
        catch { /* Window Closed? */ }
    }

    // IO Usage
    private void OpenDB(object sender, EventArgs e)
    {
        if (Directory.Exists(DatabasePath))
            Process.Start("explorer.exe", DatabasePath);
    }

    private void Menu_Export_Click(object sender, EventArgs e)
    {
        if (Results.Count == 0)
        { WinFormsUtil.Alert(MsgDBCreateReportFail); return; }

        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDBExportResultsPrompt))
            return;

        using var fbd = new FolderBrowserDialog();
        if (DialogResult.OK != fbd.ShowDialog())
            return;

        string folder = fbd.SelectedPath;
        Directory.CreateDirectory(folder);

        foreach (var gift in Results.OfType<DataMysteryGift>()) // WC3 have no data
        {
            var fileName = Util.CleanFileName(gift.FileName);
            var path = Path.Combine(folder, fileName);
            var data = gift.Write();
            File.WriteAllBytes(path, data);
        }
    }

    // View Updates
    private void B_Search_Click(object sender, EventArgs e)
    {
        // Populate Search Query Result
        IEnumerable<MysteryGift> res = RawDB;

        byte format = (byte)(MAXFORMAT + 1 - CB_Format.SelectedIndex);

        switch (CB_FormatComparator.SelectedIndex)
        {
            case 0: /* Do nothing */                            break;
            case 1: res = res.Where(mg => mg.Generation >= format); break;
            case 2: res = res.Where(mg => mg.Generation == format); break;
            case 3: res = res.Where(mg => mg.Generation <= format); break;
        }

        // Primary Searchables
        var species = WinFormsUtil.GetIndex(CB_Species);
        int item = WinFormsUtil.GetIndex(CB_HeldItem);
        if (species != -1) res = res.Where(pk => pk.Species == species);
        if (item != -1) res = res.Where(pk => pk.HeldItem == item);

        // Secondary Searchables
        int move1 = WinFormsUtil.GetIndex(CB_Move1);
        int move2 = WinFormsUtil.GetIndex(CB_Move2);
        int move3 = WinFormsUtil.GetIndex(CB_Move3);
        int move4 = WinFormsUtil.GetIndex(CB_Move4);
        if (move1 != -1) res = res.Where(mg => mg.HasMove((ushort)move1));
        if (move2 != -1) res = res.Where(mg => mg.HasMove((ushort)move2));
        if (move3 != -1) res = res.Where(mg => mg.HasMove((ushort)move3));
        if (move4 != -1) res = res.Where(mg => mg.HasMove((ushort)move4));

        var shiny = CHK_Shiny.CheckState;
        if (shiny == CheckState.Checked) res = res.Where(pk => pk.IsShiny);
        else if (shiny == CheckState.Unchecked) res = res.Where(pk => !pk.IsShiny);

        var egg = CHK_IsEgg.CheckState;
        if (egg == CheckState.Checked) res = res.Where(pk => pk.IsEgg);
        else if (egg == CheckState.Unchecked) res = res.Where(pk => !pk.IsEgg);

        slotSelected = -1; // reset the slot last viewed

        ReadOnlySpan<char> batchText = RTB_Instructions.Text;
        if (batchText.Length != 0 && !StringInstructionSet.HasEmptyLine(batchText))
        {
            var filters = StringInstruction.GetFilters(batchText);
            BatchEditing.ScreenStrings(filters);
            res = res.Where(pk => BatchEditing.IsFilterMatch(filters, pk)); // Compare across all filters
        }

        var results = res.ToArray();
        if (results.Length == 0)
            WinFormsUtil.Alert(MsgDBSearchNone);

        SetResults([..results]); // updates Count Label as well.
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void UpdateScroll(object sender, ScrollEventArgs e)
    {
        if (e.OldValue == e.NewValue)
            return;
        FillPKXBoxes(e.NewValue);
        ShowSet.Clear();
    }

    private void SetResults(List<MysteryGift> res)
    {
        Results = [..res];
        ShowSet.Clear();

        SCR_Box.Maximum = (int)Math.Ceiling((decimal)Results.Count / RES_MIN);
        if (SCR_Box.Maximum > 0)
            SCR_Box.Maximum--;

        SCR_Box.Value = 0;
        FillPKXBoxes(0);

        L_Count.Text = string.Format(Counter, Results.Count);
    }

    private void FillPKXBoxes(int start)
    {
        if (Results.Count == 0)
        {
            for (int i = 0; i < RES_MAX; i++)
            {
                PKXBOXES[i].Image = null;
                PKXBOXES[i].BackgroundImage = null;
            }
            return;
        }
        int begin = start * RES_MIN;
        int end = Math.Min(RES_MAX, Results.Count - (start * RES_MIN));
        for (int i = 0; i < end; i++)
            PKXBOXES[i].Image = Results[i + begin].Sprite();
        for (int i = end; i < RES_MAX; i++)
            PKXBOXES[i].Image = null;
        UpdateSlotColor(start);
    }

    private void UpdateSlotColor(int start)
    {
        for (int i = 0; i < RES_MAX; i++)
            PKXBOXES[i].BackgroundImage = SpriteUtil.Spriter.Transparent;
        if (slotSelected != -1 && slotSelected >= RES_MIN * start && slotSelected < (RES_MIN * start) + RES_MAX)
            PKXBOXES[slotSelected - (start * RES_MIN)].BackgroundImage = slotColor ?? SpriteUtil.Spriter.View;
    }

    private void Menu_Import_Click(object sender, EventArgs e)
    {
        if (!BoxView.GetBulkImportSettings(out var clearAll, out var overwrite, out var noSetb))
            return;

        int box = BoxView.Box.CurrentBox;
        int ctr = SAV.LoadBoxes(Results, out var result, box, clearAll, overwrite, noSetb);
        if (ctr <= 0)
            return;

        BoxView.SetPKMBoxes();
        BoxView.UpdateBoxViewers();
        WinFormsUtil.Alert(result);
    }

    private void Menu_Exit_Click(object sender, EventArgs e) => Close();

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (!MysteryPokeGrid.RectangleToScreen(MysteryPokeGrid.ClientRectangle).Contains(MousePosition))
            return;
        int oldVal = SCR_Box.Value;
        int newVal = oldVal + (e.Delta < 0 ? 1 : -1);
        if (newVal < SCR_Box.Minimum || SCR_Box.Maximum < newVal)
            return;
        FillPKXBoxes(SCR_Box.Value = newVal);
        ShowSet.Clear();
    }

    private void ChangeFormatFilter(object sender, EventArgs e)
    {
        if (CB_FormatComparator.SelectedIndex == 0)
        {
            CB_Format.Visible = false; // !any
            CB_Format.SelectedIndex = 0;
        }
        else
        {
            CB_Format.Visible = true;
            int index = MAXFORMAT - SAV.Generation + 1;
            CB_Format.SelectedIndex = index < CB_Format.Items.Count ? index : 0; // SAV generation (offset by 1 for "Any")
        }
    }

    private void ShowHoverTextForSlot(PictureBox pb)
    {
        int index = Array.IndexOf(PKXBOXES, pb);
        if (!GetShiftedIndex(ref index))
            return;

        ShowSet.Show(pb, Results[index]);
    }

    private void B_Add_Click(object sender, EventArgs e)
    {
        var s = UC_Builder.Create();
        if (s.Length == 0)
        { WinFormsUtil.Alert(MsgBEPropertyInvalid); return; }

        // If we already have text, add a new line (except if the last line is blank).
        var tb = RTB_Instructions;
        var batchText = tb.Text;
        if (batchText.Length != 0 && !batchText.EndsWith('\n'))
            tb.AppendText(Environment.NewLine);
        tb.AppendText(s);
    }
}
