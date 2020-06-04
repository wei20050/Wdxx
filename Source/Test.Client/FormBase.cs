using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace Test.Client
{
    /// <summary>
    /// 一个带阴影 可随意拖动的基类窗口
    /// </summary>
    public partial class FormBase : System.Windows.Forms.Form
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool _mAeroEnabled;                    
        private const int CsDropshadow = 0x00020000;
        private const int WmNcpaint = 0x0085;
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        public FormBase()
        {
            _mAeroEnabled = false;
            InitializeComponent();
            MouseDown += FormBase_MouseDown;
        }
        public void FormBase_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks != 1 || e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 0x02, 0);
        }

        public struct Margins
        {
            public int LeftWidth;
            public int RightWidth;
            public int TopHeight;
            public int BottomHeight;
        }

        private const int WmNchittest = 0x84;
        private const int Htclient = 0x1;
        private const int Htcaption = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                _mAeroEnabled = CheckAeroEnabled();

                var cp = base.CreateParams;
                if (!_mAeroEnabled)
                    cp.ClassStyle |= CsDropshadow;

                return cp;
            }
        }

        private static bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major < 6) return false;
            var enabled = 0;
            DwmIsCompositionEnabled(ref enabled);
            return enabled == 1;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WmNcpaint:
                    if (_mAeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(Handle, 2, ref v, 4);
                        var margins = new Margins()
                        {
                            BottomHeight = 1,
                            LeftWidth = 1,
                            RightWidth = 1,
                            TopHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(Handle, ref margins);

                    }
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WmNchittest && (int)m.Result == Htclient)
                m.Result = (IntPtr)Htcaption;
        }
    }
}
