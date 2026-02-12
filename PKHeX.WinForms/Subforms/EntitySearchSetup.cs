using System;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class EntitySearchSetup : Form
{
    private EntityInstructionBuilder? UC_Builder;
    private SaveFile? CurrentSave;
    public Func<PKM, bool>? SearchFilter { get; private set; }

    public EntitySearchSetup()
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
    }

    /// <summary>
    /// Occurs when the Search action is requested.
    /// </summary>
    public event EventHandler? SearchRequested;

    /// <summary>
    /// Occurs when the Reset action is requested.
    /// </summary>
    public event EventHandler? ResetRequested;

    /// <summary>
    /// Occurs when the next item in a sequence is sought.
    /// </summary>
    public event EventHandler? SeekNext;

    /// <summary>
    /// Occurs when the Seek Previous action is requested.
    /// </summary>
    public event EventHandler? SeekPrevious;

    /// <summary>
    /// Initializes the search setup controls using the provided save file.
    /// </summary>
    /// <param name="sav">Save file used to configure search settings.</param>
    /// <param name="edit">Editor to provide the current PKM.</param>
    public void Initialize(SaveFile sav, IPKMView edit)
    {
        ArgumentNullException.ThrowIfNull(sav);

        UC_EntitySearch.PopulateComboBoxes(GameInfo.FilteredSources);
        UC_EntitySearch.SetFormatAnyText(MsgAny);
        UC_EntitySearch.InitializeSelections(sav, showContext: false);
        CurrentSave = sav;
        EnsureBuilder(edit);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        UC_EntitySearch.ResetComboBoxSelections();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (RTB_Instructions.Focused)
                return;

            B_Search_Click(this, EventArgs.Empty);
            e.Handled = true;
        }

        // Quick close with Ctrl+W
        if (e.KeyCode == Keys.W && ModifierKeys == Keys.Control)
            Hide();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            return;
        }
        CurrentSave = null;
        SearchFilter = null;
        base.OnFormClosing(e);
    }

    private void EnsureBuilder(IPKMView edit)
    {
        if (UC_Builder is not null)
            return;

        UC_Builder = new EntityInstructionBuilder(() => edit.PreparePKM())
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Dock = DockStyle.Top,
            ReadOnly = true,
        };
        Tab_Advanced.Controls.Add(UC_Builder);
        UC_Builder.SendToBack();
    }

    private void B_Search_Click(object? sender, EventArgs e)
    {
        SearchFilter = UC_EntitySearch.GetFilter(RTB_Instructions.Text);
        SearchRequested?.Invoke(this, EventArgs.Empty);
        B_Next.Visible = B_Previous.Visible = true;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Reset_Click(object? sender, EventArgs e)
    {
        UC_EntitySearch.ResetFilters();
        RTB_Instructions.Clear();
        SearchFilter = null;
        ResetRequested?.Invoke(this, EventArgs.Empty);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Add_Click(object? sender, EventArgs e)
    {
        if (UC_Builder is null)
            return;

        var s = UC_Builder.Create();
        if (s.Length == 0)
        {
            WinFormsUtil.Alert(MsgBEPropertyInvalid);
            return;
        }

        var tb = RTB_Instructions;
        var batchText = tb.Text;
        if (batchText.Length != 0 && !batchText.EndsWith('\n'))
            tb.AppendText(Environment.NewLine);
        tb.AppendText(s);
    }

    public bool IsSameSaveFile(SaveFile sav) => CurrentSave is not null && CurrentSave == sav;

    public void ForceReset()
    {
        SearchFilter = null;
        UC_EntitySearch.ResetFilters();
        RTB_Instructions.Clear();
        B_Next.Visible = B_Previous.Visible = false;
        ResetRequested?.Invoke(this, EventArgs.Empty);
    }

    private void B_Next_Click(object sender, EventArgs e) => SeekNext?.Invoke(this, EventArgs.Empty);
    private void B_Previous_Click(object sender, EventArgs e) => SeekPrevious?.Invoke(this, EventArgs.Empty);
}
