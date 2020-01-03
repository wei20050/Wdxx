using System;
using System.Drawing;
using System.Windows.Forms;

namespace 天域取色器
{
    public sealed partial class PM : Form
    {
        private readonly ColorView _cv;
        public bool IsClose;
        public PM(Image bitmap, ColorView cv)
        {
            _cv = cv;
            InitializeComponent();
            BackgroundImage = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
        }

        private void PM_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsClose = true;
                _cv.Close();
                Close();
            }
            else
            {
                _cv.Close();
                Close();
            }
        }
    }
}
