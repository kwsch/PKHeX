using System.Threading;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            new Thread(() =>
            {
                while (!Main.init)
                    Thread.Sleep(50);

                if (InvokeRequired)
                    try { Invoke((MethodInvoker)Close); }
                    catch { Close(); }
                else Close();
            }).Start();
        }
    }
}