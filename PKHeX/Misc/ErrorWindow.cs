using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX.Misc
{
    public partial class ErrorWindow : Form
    {
        public static void ShowErrorDialog(string friendlyMessage, Exception ex, bool allowContinue)
        {
            var dialog = new ErrorWindow(); // Todo: provide language
            dialog.ShowContinue = allowContinue;
            dialog.Message = friendlyMessage;
            dialog.Error = ex;

            var dialogResult = dialog.ShowDialog();
            if (dialogResult == DialogResult.Abort)
            {
                Application.Exit();
            }
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
                T_ExceptionDetails.Text = value.ToString();
            }
        }
        private Exception _error;

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
