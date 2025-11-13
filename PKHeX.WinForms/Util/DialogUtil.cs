using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public static class DialogUtil
{
    public static bool TrySelectIndex(this Control c, string caption, string text, IReadOnlyList<string> options, out int index, int preSelect = -1)
    {
        List<TaskDialogRadioButton> choices = [];
        for (int i = 0; i < options.Count; i++)
            choices.Add(new(options[i]) { Tag = i, Checked = (i == preSelect) });

        var page = new TaskDialogPage
        {
            Caption = caption,
            Text = text,
            Icon = TaskDialogIcon.Information,
            RadioButtons = [..choices],
            Buttons = [TaskDialogButton.OK],
            AllowCancel = true,
        };

        index = -1;
        var result = TaskDialog.ShowDialog(c, page);
        if (result != TaskDialogButton.OK)
            return false;

        foreach (var rb in choices)
        {
            if (!rb.Checked)
                continue;
            index = (int)rb.Tag!;
            return true;
        }
        return false;
    }

    public static DualDiffSelection SelectNewOld(this Control c, string file, string asNew, string asOld)
    {
        var taskButtonNew = new TaskDialogCommandLinkButton(asNew) { AllowCloseDialog = true };
        var taskButtonOld = new TaskDialogCommandLinkButton(asOld) { AllowCloseDialog = true };

        var page = new TaskDialogPage
        {
            Caption = c.Name,
            Text = Path.GetFileName(file),
            Icon = TaskDialogIcon.Information,
            Buttons = { taskButtonOld, taskButtonNew },
            AllowCancel = true,
        };

        var result = TaskDialog.ShowDialog(c, page);
        if (result == taskButtonNew)
            return DualDiffSelection.New;
        if (result == taskButtonOld)
            return DualDiffSelection.Old;
        return DualDiffSelection.None;
    }
}

public enum DualDiffSelection
{
    None,
    Old,
    New
}
