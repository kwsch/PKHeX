using System;
using System.Text;
using System.Windows.Forms;

namespace PKHeX.Misc
{
    public partial class ErrorWindow : Form
    {
        public static DialogResult ShowErrorDialog(string friendlyMessage, Exception ex, bool allowContinue)
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            var dialog = new ErrorWindow(lang)
            {
                ShowContinue = allowContinue,
                Message = friendlyMessage,
                Error = ex
            };
            var dialogResult = dialog.ShowDialog();
            if (dialogResult == DialogResult.Abort)
            {
                Environment.Exit(1);
            }
            return dialogResult;
        }

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow(string lang) : this()
        {
            Util.TranslateInterface(this, lang);
        }

        /// <summary>
        /// Gets or sets whether or not the "Continue" button is visible.
        /// </summary>
        /// <remarks>For UI exceptions, continuing could be safe.
        /// For application exceptions, continuing is not possible, so the button should not be shown.</remarks>
        public bool ShowContinue
        {
            get
            {
                return B_Continue.Visible;
            }
            set
            {
                B_Continue.Visible = value;
            }
        }

        /// <summary>
        /// Friendly, context-specific method shown to the user.
        /// </summary>
        /// <remarks>This property is intended to be a user-friendly context-specific message about what went wrong.
        /// For example: "An error occurred while attempting to automatically load the save file."</remarks>
        public string Message
        {
            get
            {
                return L_Message.Text;
            }
            set
            {
                L_Message.Text = value;
            }
        }

        public Exception Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
                UpdateExceptionDetailsMessage();             
            }
        }
        private Exception _error;

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
            catch (Exception ex)
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

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(T_ExceptionDetails.Text);
        }

        private void B_Continue_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void B_Abort_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
        
    }
}
