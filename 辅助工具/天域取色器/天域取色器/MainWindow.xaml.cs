using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace 天域取色器
{
    public partial class MainWindow
    {
        private readonly HookHelper _hk;
        private ColorView _cv;
        private PM _p;
        private bool _isZt;

        public MainWindow()
        {
            InitializeComponent();
            _hk = new HookHelper();
            _hk.KeyDownEvent += (o, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.F8:
                        Qs();
                        break;
                    case Keys.Escape:
                        _p?.Close();
                        _cv?.Close();
                        _isZt = false;
                        break;
                    case Keys.Enter:
                        if (_isZt) SetColor();
                        _p?.Close();
                        _cv?.Close();
                        _isZt = false;
                        break;
                    case Keys.Up:
                        Yx.GetCursorPos(out var x, out var y);
                        Yx.MoveTo(x, y - 1);
                        break;
                    case Keys.Down:
                        Yx.GetCursorPos(out x, out y);
                        Yx.MoveTo(x, y + 1);
                        break;
                    case Keys.Left:
                        Yx.GetCursorPos(out x, out y);
                        Yx.MoveTo(x - 1, y);
                        break;
                    case Keys.Right:
                        Yx.GetCursorPos(out x, out y);
                        Yx.MoveTo(x + 1, y);
                        break;
                    case Keys.F7:
                        Yx.MoveWindow(Yx.FindWindowCursor(), 0, 0);
                        break;
                }
            };
            _hk.SetHook();
        }

        private void Qs()
        {
            if (_isZt)
            {
                SetColor();
            }
            else
            {
                _isZt = true;
                ShowColor();
            }
        }

        private int _btn;
        private void SetColor()
        {
            TxtYs.Text = _cv.LabYs.Content.ToString().Substring(3);
            TxtZb.Text = _cv.LabZb.Content.ToString();
            if (_btn == 12)
            {
                _btn = 0;
            }
            SetBtn($"Btn{_btn++}", "#FF" + TxtYs.Text, TxtZb.Text);
        }

        private void SetBtn(string name, string color, string zb)
        {
            foreach (var c in BtnGrid.Children)
            {
                if (!(c is System.Windows.Controls.Button btn)) continue;
                if (btn.Name != name) continue;
                var tmpColor = ColorConverter.ConvertFromString(color);
                if (tmpColor != null) btn.Background = new SolidColorBrush((Color)tmpColor);
                btn.Tag = zb;
                btn.Click -= Btn_OnClick;
                btn.Click += Btn_OnClick;
                btn.Content = string.Empty;
                LabColor.Background = btn.Background;
            }
        }

        public void ShowColor()
        {
            if (GlobalVar.W == null || GlobalVar.H == null) return;
            Dispatcher?.Invoke(Hide);
            var w = (int)GlobalVar.W;
            var h = (int)GlobalVar.H;
            var bitmap = Yx.CopyScreen(0, 0, w, h);
            _cv = new ColorView(bitmap);
            _p = new PM(bitmap,_cv);
            _cv.Show();
            _p.ShowDialog();
            if (_p.IsClose)
            {
                SetColor();
            }
            Dispatcher?.Invoke(Show);
            _isZt = false;
        }

        private void ButtonMin_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            _hk.SetHook();
            Environment.Exit(0);
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                //移到窗体错误肯定是鼠标右键移动这里不需要处理异常
            }
        }

        private void Btn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is System.Windows.Controls.Button btn)) return;
            TxtYs.Text = btn.Background.ToString().Substring(3);
            TxtZb.Text = btn.Tag.ToString();
            LabColor.Background = btn.Background;
        }

        private void BtnYs_OnClick(object sender, RoutedEventArgs e)
        {
            Yx.SetClipboard(TxtYs.Text);
        }

        private void BtnZb_OnClick(object sender, RoutedEventArgs e)
        {
            Yx.SetClipboard(TxtZb.Text);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Qs();
        }

    }
}
