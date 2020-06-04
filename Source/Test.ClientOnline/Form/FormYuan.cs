using System.Drawing;
using System.Drawing.Drawing2D;

namespace Test.ClientOnline.Form
{
    public partial class FormYuan : FormBase
    {
        public FormYuan()
        {
            InitializeComponent();
            Opacity = 0.5;
            SetWindowRegion();
        }
        /// <summary>
        /// 设置窗体的Region
        /// </summary>
        public void SetWindowRegion()
        {
            var rect = new Rectangle(0, 0, Width, Height);
            var formPath = GetRoundedRectPath(rect, 150);
            Region = new Region(formPath);

        }
        /// <summary>
        /// 绘制圆角路径
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var diameter = radius;
            var arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            var path = new GraphicsPath();

            // 左上角
            path.AddArc(arcRect, 180, 90);

            // 右上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // 右下角
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // 左下角
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();//闭合曲线
            return path;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
