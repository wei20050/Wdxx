using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 天域取色器
{
    public partial class ColorView
    {
        public ColorView(Bitmap bitmap)
        {
            _bitmap = bitmap;
            InitializeComponent();
            Left = 6;
            Top = 6;
            Ini();
            _timer.Tick += (o, e) =>
            {
                Dispatcher?.Invoke(SetBackground);
            };
            _timer.Interval = TimeSpan.FromMilliseconds(18);
        }
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly Bitmap _bitmap;
        private void SetBackground()
        {
            try
            {
                ColorGrid.Children.Clear();
                Yx.GetCursorPos(out var x, out var y);
                if (x < Width && y < 300)
                {
                    if (GlobalVar.W != null) Left = (double)GlobalVar.W - Width - 6;
                }
                if (x > GlobalVar.W - Width && y < 300)
                {
                    Left = 6;
                }
                var cArr = YxColor.GetColor(x, y, _bitmap);
                LabZb.Content = $"{x},{y}";
                LabYs.Content = $"#{cArr[7, 7]}";
                var c = System.Windows.Media.ColorConverter.ConvertFromString(LabYs?.Content?.ToString());
                var color = (System.Windows.Media.Color?)c ?? System.Windows.Media.Color.FromRgb(0, 0, 0);
                LabYsx.Background = new SolidColorBrush(color);
                for (var i = 0; i < 15; i++)
                {
                    for (var j = 0; j < 15; j++)
                    {
                        var w = i * 20 + 2;
                        var h = j * 20 + 2;
                        var converter = new BrushConverter();
                        SolidColorBrush brush;
                        try
                        {
                            brush = (SolidColorBrush)converter.ConvertFromString("#" + cArr[i, j]);
                        }
                        catch
                        {
                            brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
                        }
                        var l = new Label
                        {
                            Width = 18,
                            Height = 18,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(w, h, 0, 0),
                            Background = brush
                        };
                        if (i == 7)
                        {
                            if (j == 6 || j == 8)
                            {
                                l.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));
                            }
                        }
                        if (j == 7)
                        {
                            if (i == 6 || i == 8)
                            {
                                l.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                            }
                        }
                        ColorGrid.Children.Add(l);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private void Ini()
        {
            for (var i = 0; i < 15; i++)
            {
                var index = i * 20 + 1;
                var l = new Line
                {
                    X1 = index,
                    X2 = index,
                    Y1 = 0,
                    Y2 = 300,
                    StrokeThickness = 2,
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0))
                };
                MainGrid.Children.Add(l);
            }
            for (var j = 0; j < 15; j++)
            {
                var index = j * 20 + 1;
                var l = new Line
                {
                    X1 = 0,
                    X2 = 300,
                    Y1 = index,
                    Y2 = index,
                    StrokeThickness = 2,
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0))
                };
                MainGrid.Children.Add(l);
            }
            var l1 = new Line
            {
                X1 = 0,
                X2 = 300,
                Y1 = 299,
                Y2 = 299,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0))
            };
            MainGrid.Children.Add(l1);
            var l2 = new Line
            {
                X1 = 299,
                X2 = 299,
                Y1 = 0,
                Y2 = 300,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0))
            };
            MainGrid.Children.Add(l2);
        }

        private void ColorView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void ColorView_OnClosed(object sender, EventArgs e)
        {
            _timer.Stop();
        }
    }
}
