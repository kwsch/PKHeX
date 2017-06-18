using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PKHeX.WinForms
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            new Task(() =>
            {
                while (!Main.IsInitialized)
                    Thread.Sleep(50);

                if (InvokeRequired)
                    try { Invoke((MethodInvoker)Close); }
                    catch { Close(); }
                else Close();
            }).Start();
        }
    }
}