using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Test.Client
{
    public partial class FormBase : System.Windows.Forms.Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        public FormBase()
        {
            InitializeComponent();
            MouseDown += FormBase_MouseDown;
        }
        public void FormBase_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks != 1 || e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 0x02, 0);
        }
    }
}
