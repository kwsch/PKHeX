using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public static class DialogUtil
{
    extension(Control c)
    {
        public bool TrySelectIndex(string caption, string text, IReadOnlyList<string> options, out int index, int preSelect = -1)
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
                DefaultButton = TaskDialogButton.OK,
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

        public DualDiffSelection SelectNewOld(string file, string asNew, string asOld)
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

        public DialogResult RequestOverwrite(string exist)
        {
            var taskButtonOverwrite = new TaskDialogCommandLinkButton("Overwrite") { AllowCloseDialog = true };
            var taskButtonSelect = new TaskDialogCommandLinkButton("Save As...") { AllowCloseDialog = true };
            var page = new TaskDialogPage
            {
                Caption = "Overwrite existing file?",
                Text = exist,
                Icon = TaskDialogIcon.Information,
                Buttons = [taskButtonOverwrite, taskButtonSelect],
                SizeToContent = true,
                DefaultButton = taskButtonOverwrite,
                AllowCancel = true,
            };

            var result = TaskDialog.ShowDialog(c, page);
            if (result == taskButtonOverwrite)
                return DialogResult.Yes;
            if (result == taskButtonSelect)
                return DialogResult.No;
            return DialogResult.Cancel;
        }
    }
}

public enum DualDiffSelection
{
    None,
    Old,
    New
}
