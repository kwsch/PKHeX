using System;
using System.Text;
using System.Windows.Forms;

namespace PKHeX.WinForms
{
    public sealed partial class ErrorWindow : Form
    {
        public static DialogResult ShowErrorDialog(string friendlyMessage, Exception ex, bool allowContinue)
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            using var dialog = new ErrorWindow(lang)
            {
                ShowContinue = allowContinue,
                Message = friendlyMessage,
                Error = ex
            };
            var dialogResult = dialog.ShowDialog();
            if (dialogResult == DialogResult.Abort)
                Environment.Exit(1);
            return dialogResult;
        }

        private ErrorWindow()
        {
            InitializeComponent();
        }

        private ErrorWindow(string lang) : this()
        {
            WinFormsUtil.TranslateInterface(this, lang);
        }

        /// <summary>
        /// Gets or sets whether or not the "Continue" button is visible.
        /// </summary>
        /// <remarks>For UI exceptions, continuing could be safe.
        /// For application exceptions, continuing is not possible, so the button should not be shown.</remarks>
        private bool ShowContinue
        {
            set => B_Continue.Visible = value;
        }

        /// <summary>
        /// Friendly, context-specific method shown to the user.
        /// </summary>
        /// <remarks>This property is intended to be a user-friendly context-specific message about what went wrong.
        /// For example: "An error occurred while attempting to automatically load the save file."</remarks>
        private string Message
        {
            get => L_Message.Text;
            set => L_Message.Text = value;
        }

        private Exception? _error;

        public Exception Error
        {
            get => _error ?? throw new ArgumentNullException(nameof(_error));
            set
            {
                _error = value;
                UpdateExceptionDetailsMessage();
            }
        }

        private void UpdateExceptionDetailsMessage()
        {
            var details = new StringBuilder();
            details.AppendLine("Exception Details:");
            details.AppendLine(Error.ToString());
            details.AppendLine();

            details.AppendLine("Loaded Assemblies:");
            details.AppendLine("--------------------");
            try
            {
                foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                {
                    details.AppendLine(item.FullName);
                    details.AppendLine(item.Location);
                    details.AppendLine();
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                details.AppendLine("An error occurred while listing the Loaded Assemblies:");
                details.AppendLine(ex.ToString());
            }
            details.AppendLine("--------------------");

            // Include message in case it contains important information, like a file path.
            details.AppendLine("User Message:");
            details.AppendLine(Message);

            T_ExceptionDetails.Text = details.ToString();
        }

        private void ClickCopyException(object sender, EventArgs e) => WinFormsUtil.SetClipboardText(T_ExceptionDetails.Text);

        private void ClickContinue(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ClickAbort(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
    }
}
