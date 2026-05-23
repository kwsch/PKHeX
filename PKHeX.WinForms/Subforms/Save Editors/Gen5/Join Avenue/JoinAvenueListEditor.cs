using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

internal partial class JoinAvenueListEditor : UserControl
{
    protected JoinAvenueListEditor() => InitializeComponent();
}

internal sealed class JoinAvenueListEditor<T, TSpecific> : JoinAvenueListEditor
    where T : class, IJoinAvenueEntity5
    where TSpecific : UserControl, IJoinAvenueSpecificEditor<T>
{
    private const string ImportFilter = "Join Avenue Entity (*.jav5;*.jaa5;*.jah5)|*.jav5;*.jaa5;*.jah5|All Files|*.*";
    private readonly JoinAvenueEntityGeneralEditor GeneralEditor = new();
    private readonly TSpecific SpecificEditor;
    private readonly Func<int, T> Getter;
    private readonly int Count;
    private int CurrentIndex = -1;
    private bool Loading;

    public JoinAvenueListEditor(int count, Func<int, T> getter, TSpecific specificEditor)
    {
        Count = count;
        Getter = getter;
        SpecificEditor = specificEditor;

        AddDockedControl(Tab_General, GeneralEditor);
        AddDockedControl(Tab_Specific, specificEditor);
        LB_Entries.SelectedIndexChanged += LB_Entries_SelectedIndexChanged;
        B_Import.Click += B_Import_Click;
        B_Export.Click += B_Export_Click;
    }

    private static void AddDockedControl(Control parent, Control child)
    {
        child.Dock = DockStyle.Fill;
        parent.Controls.Add(child);
    }

    public void LoadAll()
    {
        Loading = true;
        LB_Entries.Items.Clear();
        for (int i = 0; i < Count; i++)
            LB_Entries.Items.Add(GetLabel(i, Getter(i)));
        Loading = false;
        if (LB_Entries.Items.Count != 0)
            LB_Entries.SelectedIndex = 0;
    }

    public void SaveAll()
    {
        SaveCurrent();
        RefreshLabels();
    }

    private void LB_Entries_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (Loading)
            return;

        SaveCurrent();
        CurrentIndex = LB_Entries.SelectedIndex;
        if (CurrentIndex < 0)
            return;

        Loading = true;
        var entity = Getter(CurrentIndex);
        GeneralEditor.LoadObject(entity);
        SpecificEditor.LoadObject(entity);
        Loading = false;
    }

    private void SaveCurrent()
    {
        if (Loading || CurrentIndex < 0)
            return;

        var entity = Getter(CurrentIndex);
        GeneralEditor.SaveObject(entity);
        SpecificEditor.SaveObject(entity);

        Loading = true;
        LB_Entries.Items[CurrentIndex] = GetLabel(CurrentIndex, entity);
        Loading = false;
    }

    private void RefreshLabels()
    {
        for (int i = 0; i < Count; i++)
            LB_Entries.Items[i] = GetLabel(i, Getter(i));
    }

    private string GetLabel(int index, T entity)
    {
        var label = entity.Name.Trim();
        if (string.IsNullOrWhiteSpace(label))
            label = GameInfo.Strings.specieslist[0];
        return $"{index + 1:00} - {label}";
    }

    private void B_Export_Click(object? sender, EventArgs e)
    {
        SaveCurrent();
        if (CurrentIndex < 0)
            return;

        var entity = Getter(CurrentIndex);
        using var sfd = new SaveFileDialog();
        sfd.Filter = $"Join Avenue Entity (*.{entity.FileExtension})|*.{entity.FileExtension}|All Files|*.*";
        sfd.DefaultExt = entity.FileExtension;
        sfd.FileName = PathUtil.CleanFileName($"{CurrentIndex + 1:00}_{entity.Name}");
        if (sfd.ShowDialog(this) != DialogResult.OK)
            return;

        File.WriteAllBytes(sfd.FileName, entity.Write());
        WinFormsUtil.Asterisk();
    }

    private void B_Import_Click(object? sender, EventArgs e)
    {
        if (CurrentIndex < 0)
            return;

        using var ofd = new OpenFileDialog();
        ofd.Filter = ImportFilter;
        if (ofd.ShowDialog(this) != DialogResult.OK)
            return;

        byte[] data = File.ReadAllBytes(ofd.FileName);
        var entity = Getter(CurrentIndex);
        if (!TryCreateImportedEntity(data, out var imported))
        {
            WinFormsUtil.Error("Unable to import Join Avenue entity.");
            return;
        }

        entity.CopyFrom(imported);
        Loading = true;
        GeneralEditor.LoadObject(entity);
        SpecificEditor.LoadObject(entity);
        LB_Entries.Items[CurrentIndex] = GetLabel(CurrentIndex, entity);
        Loading = false;
    }

    private static bool TryCreateImportedEntity(Memory<byte> data, [NotNullWhen(true)] out IJoinAvenueEntity5? entity)
    {
        switch (data.Length)
        {
            case JoinAvenueVisitor5.SIZE:
                entity = new JoinAvenueVisitor5(data);
                return true;
            case JoinAvenueFan5.SIZE:
                entity = new JoinAvenueFan5(data);
                return true;
            case JoinAvenueAssistant5.SIZE:
                entity = new JoinAvenueAssistant5(data);
                return true;
            default:
                entity = null;
                return false;
        }
    }
}
