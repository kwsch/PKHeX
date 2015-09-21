using System.Timers;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += DisplayTimeEvent;
            myTimer.Interval = 50; // milliseconds per trigger interval
            myTimer.Start();
        }
        public void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            if (!Main.init)
                if (L_Status.InvokeRequired) L_Status.Invoke((MethodInvoker)delegate { L_Status.Text = Main.Status; });
                else { L_Status.Text = Main.Status; }
            else
                if (InvokeRequired && IsHandleCreated)
                    try { Invoke((MethodInvoker)Close); } catch { Close(); }
                else Close();
        }
    }
}