using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace PKHeX
{
    public partial class SplashScreen : Form
    {
        Form1 m_parent;
        public SplashScreen(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
            myTimer.Interval = 50; // milliseconds per trigger interval
            myTimer.Start();
        }
        public void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                if (m_parent.init)
                {
                    if (InvokeRequired && IsHandleCreated)
                        Invoke((MethodInvoker)delegate(){Close();});
                    else
                        Close();
                }
            }
            catch { };
        }
    }
   
}
