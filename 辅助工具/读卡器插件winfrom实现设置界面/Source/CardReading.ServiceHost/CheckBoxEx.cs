using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CardReading.ServiceHost
{
    [ToolboxBitmap(typeof(CheckBox))]
    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    public class CheckBoxEx : CheckBox
    {
        private Color _baseColor = Color.Black;
        private int _defaultCheckButtonWidth = 12;
        private ControlState _controlState;
        private const ContentAlignment RightAlignment = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
        private const ContentAlignment LeftAligbment = ContentAlignment.TopLeft | ContentAlignment.BottomLeft | ContentAlignment.MiddleLeft;

        public CheckBoxEx()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
        }

        public Color BaseColor
        {
            get
            {
                return _baseColor;
            }

            set
            {
                _baseColor = value;
                Invalidate();
            }
        }

        internal ControlState ControlState
        {
            get
            {
                return _controlState;
            }

            set
            {
                if (_controlState == value) return;
                _controlState = value;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            ControlState = ControlState.Hover;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ControlState = ControlState.Normal;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                ControlState = ControlState.Pressed;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != MouseButtons.Left || e.Clicks != 1) return;
            ControlState = ClientRectangle.Contains(e.Location) ? ControlState.Hover : ControlState.Normal;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            OnPaintBackground(e);
            var g = e.Graphics;
            Rectangle checkButtonRect;
            Rectangle textRect;
            CalculateRect(out checkButtonRect, out textRect);
            var gr = g.ClipBounds;
            g.Clip = new Region(new Rectangle { X = (int)gr.X, Y = (int)gr.Y, Width = checkButtonRect.Width + textRect.Width + 30, Height = (int)gr.Height });

            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color borderColor;
            Color innerBorderColor;
            Color checkColor;
            var hover = false;
            if (Enabled)
            {
                switch (ControlState)
                {
                    case ControlState.Hover:
                        borderColor = _baseColor;
                        innerBorderColor = _baseColor;
                        checkColor = GetColor(_baseColor, 0, 35, 24, 9);
                        hover = true;
                        break;
                    case ControlState.Pressed:
                        borderColor = _baseColor;
                        innerBorderColor = GetColor(_baseColor, 0, -13, -8, -3);
                        checkColor = GetColor(_baseColor, 0, -35, -24, -9);
                        hover = true;
                        break;
                    case ControlState.Normal:
                        borderColor = _baseColor;
                        innerBorderColor = Color.Empty;
                        checkColor = _baseColor;
                        break;
                    case ControlState.Focused:
                        borderColor = _baseColor;
                        innerBorderColor = Color.Empty;
                        checkColor = _baseColor;
                        break;
                    default:
                        borderColor = _baseColor;
                        innerBorderColor = Color.Empty;
                        checkColor = _baseColor;
                        break;
                }
            }
            else
            {
                borderColor = SystemColors.ControlDark;
                innerBorderColor = SystemColors.ControlDark;
                checkColor = SystemColors.ControlDark;
            }

            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, checkButtonRect);
            }

            if (hover)
            {
                using (var pen = new Pen(innerBorderColor, 2F))
                {
                    g.DrawRectangle(pen, checkButtonRect);
                }
            }
            switch (CheckState)
            {
                case CheckState.Checked:
                    DrawCheckedFlag(
                        g,
                        checkButtonRect,
                        checkColor);
                    break;
                case CheckState.Indeterminate:
                    checkButtonRect.Inflate(-1, -1);
                    using (var path = new GraphicsPath())
                    {
                        path.AddEllipse(checkButtonRect);
                        using (var brush = new PathGradientBrush(path))
                        {
                            brush.CenterColor = checkColor;
                            brush.SurroundColors = new[] { Color.White };
                            var blend = new Blend
                            {
                                Positions = new[] { 0f, 0.4f, 1f },
                                Factors = new[] { 0f, 0.3f, 1f }
                            };
                            brush.Blend = blend;
                            g.FillEllipse(brush, checkButtonRect);
                        }
                    }
                    checkButtonRect.Inflate(1, 1);
                    break;
                case CheckState.Unchecked:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            using (var pen = new Pen(borderColor))
            {
                g.DrawRectangle(pen, checkButtonRect);
            }

            var textColor = Enabled ? ForeColor : SystemColors.GrayText;
            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textRect,
                textColor,
                GetTextFormatFlags(TextAlign, RightToLeft == RightToLeft.Yes));
        }

        private void CalculateRect(out Rectangle checkButtonRect, out Rectangle textRect)
        {
            _defaultCheckButtonWidth = Height - 4;
            checkButtonRect = new Rectangle(
                0, 0, _defaultCheckButtonWidth, _defaultCheckButtonWidth);
            textRect = Rectangle.Empty;
            var bCheckAlignLeft = (LeftAligbment & CheckAlign) != 0;
            var bCheckAlignRight = (RightAlignment & CheckAlign) != 0;
            var bRightToLeft = RightToLeft == RightToLeft.Yes;

            if (bCheckAlignLeft && !bRightToLeft || bCheckAlignRight && bRightToLeft)
            {
                switch (CheckAlign)
                {
                    case ContentAlignment.TopRight:
                    case ContentAlignment.TopLeft:
                        checkButtonRect.Y = 2;
                        break;
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.MiddleLeft:
                        checkButtonRect.Y = (Height - _defaultCheckButtonWidth) / 2;
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.BottomLeft:
                        checkButtonRect.Y = Height - _defaultCheckButtonWidth - 2;
                        break;
                    case ContentAlignment.TopCenter:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.MiddleCenter:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.BottomCenter:
                        throw new ArgumentOutOfRangeException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                checkButtonRect.X = 1;

                textRect = new Rectangle(
                    checkButtonRect.Right + 2,
                    0,
                    Width - checkButtonRect.Right - 4,
                    Height);
            }
            else if (bCheckAlignRight || bCheckAlignLeft)
            {
                switch (CheckAlign)
                {
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        checkButtonRect.Y = 2;
                        break;
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        checkButtonRect.Y = (Height - _defaultCheckButtonWidth) / 2;
                        break;
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        checkButtonRect.Y = Height - _defaultCheckButtonWidth - 2;
                        break;
                    case ContentAlignment.TopCenter:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.MiddleCenter:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.BottomCenter:
                        throw new ArgumentOutOfRangeException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                checkButtonRect.X = Width - _defaultCheckButtonWidth - 1;
                textRect = new Rectangle(2, 0, Width - _defaultCheckButtonWidth - 6, Height);
            }
            else
            {
                switch (CheckAlign)
                {
                    case ContentAlignment.TopCenter:
                        checkButtonRect.Y = 2;
                        textRect.Y = checkButtonRect.Bottom + 2;
                        textRect.Height = Height - _defaultCheckButtonWidth - 6;
                        break;
                    case ContentAlignment.MiddleCenter:
                        checkButtonRect.Y = (Height - _defaultCheckButtonWidth) / 2;
                        textRect.Y = 0;
                        textRect.Height = Height;
                        break;
                    case ContentAlignment.BottomCenter:
                        checkButtonRect.Y = Height - _defaultCheckButtonWidth - 2;
                        textRect.Y = 0;
                        textRect.Height = Height - _defaultCheckButtonWidth - 6;
                        break;
                    case ContentAlignment.TopLeft:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.TopRight:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.MiddleLeft:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.MiddleRight:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.BottomLeft:
                        throw new ArgumentOutOfRangeException();
                    case ContentAlignment.BottomRight:
                        throw new ArgumentOutOfRangeException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                checkButtonRect.X = (Width - _defaultCheckButtonWidth) / 2;

                textRect.X = 2;
                textRect.Width = Width - 4;
            }
        }

        private static void DrawCheckedFlag(Graphics graphics, Rectangle rect, Color color)
        {
            var points = new PointF[3];
            points[0] = new PointF(
                rect.X + rect.Width / 4.5f,
                rect.Y + rect.Height / 2.5f);
            points[1] = new PointF(
                rect.X + rect.Width / 2.5f,
                rect.Bottom - rect.Height / 3f);
            points[2] = new PointF(
                rect.Right - rect.Width / 4.0f,
                rect.Y + rect.Height / 4.5f);
            using (var pen = new Pen(color, 2F))
            {
                graphics.DrawLines(pen, points);
            }
        }

        private static Color GetColor(Color colorBase, int a, int r, int g, int b)
        {
            int a0 = colorBase.A;
            int r0 = colorBase.R;
            int g0 = colorBase.G;
            int b0 = colorBase.B;
            a = a + a0 > 255 ? 255 : Math.Max(a + a0, 0);
            r = r + r0 > 255 ? 255 : Math.Max(r + r0, 0);
            g = g + g0 > 255 ? 255 : Math.Max(g + g0, 0);
            b = b + b0 > 255 ? 255 : Math.Max(b + b0, 0);
            return Color.FromArgb(a, r, g, b);
        }

        internal static TextFormatFlags GetTextFormatFlags(
            ContentAlignment alignment,
            bool rightToleft)
        {
            var flags = TextFormatFlags.WordBreak |
                TextFormatFlags.SingleLine;
            if (rightToleft)
            {
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }

            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
                case ContentAlignment.MiddleCenter:
                    flags |= TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case ContentAlignment.TopCenter:
                    flags |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopLeft:
                    flags |= TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopRight:
                    flags |= TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("alignment", alignment, null);
            }
            return flags;
        }
    }
}
