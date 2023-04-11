using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xe.AcrylicView.Platforms.MacCatalyst
{
    public class BorderView : UIView
    {
        private CAShapeLayer borderLayer;

        private Thickness borderThickness;

        private Thickness cornerRadius;

        private CGColor borderColor;

        public CGColor BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
                SetupBorderLayer();
            }
        }

        public Thickness BorderThickness
        {
            get
            {
                return borderThickness;
            }
            set
            {
                borderThickness = value;
                SetupBorderLayer();
            }
        }

        public Thickness CornerRadius
        {
            get
            {
                return cornerRadius;
            }
            set
            {
                cornerRadius = value;
                SetupBorderLayer();
            }
        }

        internal Func<Rect, Size> CrossPlatformArrange
        {
            get;
            set;
        }

        internal Func<double, double, Size> CrossPlatformMeasure
        {
            get;
            set;
        }

        public override CGRect Frame
        {
            get
            {
                return base.Frame;
            }
            set
            {
                base.Frame = value;
                SetupBorderLayer();
            }
        }

        private NFloat CapRadius(double a, double b, double c)
        {
            if (a <= 0)
            {
                return (NFloat)a;
            }
            return (NFloat)Math.Min(a, Math.Min(b, c));
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            Rect rectangle = CoreGraphicsExtensions.ToRectangle(Bounds);
            Func<double, double, Size> crossPlatformMeasure = CrossPlatformMeasure;
            if (crossPlatformMeasure != null)
            {
                crossPlatformMeasure.Invoke(rectangle.Width, rectangle.Height);
            }
            else
            {
            }
            Func<Rect, Size> crossPlatformArrange = CrossPlatformArrange;
            if (crossPlatformArrange != null)
            {
                crossPlatformArrange.Invoke(rectangle);
            }
            else
            {
            }
            SetupBorderLayer();
        }




        public override bool PointInside(CGPoint point, UIEvent uievent)
        {
            UIView[] subviews = Subviews;
            for (int i = 0; i < (int)subviews.Length; i++)
            {
                UIView uIView = subviews[i];
                if (uIView.HitTest(ConvertPointToView(point, uIView), uievent) != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void SetupBorderLayer()
        {

            if (Frame.IsEmpty)
            {
                return;
            }
            if (borderLayer != null)
            {
                borderLayer.RemoveFromSuperLayer();
                borderLayer = null;
            }
            CALayer layer = Layer;
            layer.BorderWidth = 0;
            layer.CornerRadius = 0;
            CGSize size = layer.Bounds.Size;
            NFloat width = size.Width;
            size = layer.Bounds.Size;
            NFloat height = size.Height;
            NFloat x = layer.Bounds.X;
            NFloat y = layer.Bounds.Y;
            NFloat nFloat = x;
            NFloat nFloat1 = y;
            NFloat nFloat2 = x + width;
            NFloat nFloat3 = y + height;
            Thickness borderThickness = BorderThickness;
            double num = Math.Max(0, borderThickness.Left);
            double num1 = Math.Max(0, borderThickness.Top);
            double num2 = Math.Max(0, borderThickness.Right);
            double num3 = Math.Max(0, borderThickness.Bottom);
            double num4 = num1 + num3;
            double num5 = num + num2;
            double num6 = (num1 > 0 ? num1 * Math.Min(1, height / num4) : num1);
            double num7 = (num2 > 0 ? num2 * Math.Min(1, width / num5) : num2);
            double num8 = (num3 > 0 ? num3 * Math.Min(1, height / num4) : num3);
            double num9 = (num > 0 ? num * Math.Min(1, width / num5) : num);
            Thickness cornerRadius = CornerRadius;
            NFloat left = (NFloat)cornerRadius.Left;
            NFloat top = (NFloat)cornerRadius.Top;
            NFloat right = (NFloat)cornerRadius.Right;
            NFloat bottom = (NFloat)cornerRadius.Bottom;
            NFloat nFloat4 = left + top;
            NFloat nFloat5 = top + right;
            NFloat nFloat6 = right + bottom;
            NFloat nFloat7 = bottom + left;
            NFloat nFloat8 = CapRadius(left, (left / nFloat4) * width, (left / nFloat7) * height);
            NFloat nFloat9 = CapRadius(top, (top / nFloat4) * width, (top / nFloat5) * height);
            NFloat nFloat10 = CapRadius(right, (right / nFloat6) * width, (right / nFloat5) * height);
            NFloat nFloat11 = CapRadius(bottom, (bottom / nFloat6) * width, (bottom / nFloat7) * height);
            CGPath cGPath = new();
            cGPath.MoveToPoint(nFloat + nFloat8, nFloat1);
            cGPath.AddArcToPoint(nFloat2, nFloat1, nFloat2, nFloat1 + nFloat9, nFloat9);
            cGPath.AddArcToPoint(nFloat2, nFloat3, nFloat2 - nFloat10, nFloat3, nFloat10);
            cGPath.AddArcToPoint(nFloat, nFloat3, nFloat, nFloat3 - nFloat11, nFloat11);
            cGPath.AddArcToPoint(nFloat, nFloat1, nFloat + nFloat8, nFloat1, nFloat8);
            cGPath.CloseSubpath();
            layer.Mask = new CAShapeLayer
            {
                Name = "ClipShapeLayer",
                Path = cGPath
            };
            if (num9 > 0 || num6 > 0 || num7 > 0 || num8 > 0)
            {
                CGPath cGPath1 = new();
                cGPath1.AddRect(new CGRect(nFloat, nFloat1, width, height));
                if (num6 > 0 || num9 > 0)
                {
                    cGPath1.MoveToPoint(nFloat + nFloat8, (NFloat)(nFloat1 + num6));
                }
                else
                {
                    cGPath1.MoveToPoint(nFloat, nFloat1);
                }
                if (num6 > 0 || num7 > 0)
                {
                    double num10 = Math.Max(0, nFloat9 - num7);
                    double num11 = Math.Max(0, nFloat9 - num6);
                    double num12 = Math.Max(num10, num11);
                    CGAffineTransform cGAffineTransform = new((num12 > 0 ? (NFloat)(num10 / num12) : (NFloat)num12), 0, 0, (num12 > 0 ? (NFloat)(num11 / num12) : (NFloat)num12), (NFloat)(nFloat2 - num7 - num10), (NFloat)(nFloat1 + num6 + num11));
                    cGPath1.AddArc(cGAffineTransform, 0, 0, (NFloat)num12, (NFloat)4.71238898038469, 0, false);
                }
                else
                {
                    cGPath1.MoveToPoint(nFloat2, nFloat1);
                }
                if (num8 > 0 || num7 > 0)
                {
                    double num13 = Math.Max(0, nFloat10 - num7);
                    double num14 = Math.Max(0, nFloat10 - num8);
                    double num15 = Math.Max(num13, num14);
                    CGAffineTransform cGAffineTransform1 = new((num15 > 0 ? (NFloat)(num13 / num15) : (NFloat)num15), 0, 0, (num15 > 0 ? (NFloat)(num14 / num15) : (NFloat)num15), (NFloat)(nFloat2 - num7 - num13), (NFloat)(nFloat3 - num8 - num14));
                    cGPath1.AddArc(cGAffineTransform1, 0, 0, (NFloat)num15, 0, (NFloat)1.5707963267949, false);
                }
                else
                {
                    cGPath1.AddLineToPoint(nFloat2, nFloat3);
                }
                if (num8 > 0 || num9 > 0)
                {
                    NFloat nFloat12 = (NFloat)Math.Max(0, nFloat11 - num9);
                    NFloat nFloat13 = (NFloat)Math.Max(0, nFloat11 - num8);
                    NFloat nFloat14 = (NFloat)Math.Max(nFloat12, nFloat13);
                    CGAffineTransform cGAffineTransform2 = new((nFloat14 > 0 ? nFloat12 / nFloat14 : nFloat14), 0, 0, (nFloat14 > 0 ? nFloat13 / nFloat14 : nFloat14), (NFloat)(nFloat + num9 + nFloat12), (NFloat)(nFloat3 - num8 - nFloat13));
                    cGPath1.AddArc(cGAffineTransform2, 0, 0, nFloat14, (NFloat)3.14159265358979 / 2, (NFloat)3.14159265358979, false);
                }
                else
                {
                    cGPath1.AddLineToPoint(nFloat, nFloat3);
                }
                if (num6 > 0 || num9 > 0)
                {
                    NFloat nFloat15 = (NFloat)Math.Max(0, nFloat8 - num9);
                    NFloat nFloat16 = (NFloat)Math.Max(0, nFloat8 - num6);
                    NFloat nFloat17 = (NFloat)Math.Max(nFloat15, nFloat16);
                    CGAffineTransform cGAffineTransform3 = new((nFloat17 > 0 ? nFloat15 / nFloat17 : nFloat17), 0, 0, (nFloat17 > 0 ? nFloat16 / nFloat17 : nFloat17), (NFloat)(nFloat + num9 + nFloat15), (NFloat)(nFloat1 + num6 + nFloat16));
                    cGPath1.AddArc(cGAffineTransform3, 0, 0, nFloat17, (NFloat)3.14159265358979, ((NFloat)3.14159265358979 * 3) / 2, false);
                }
                else
                {
                    cGPath1.AddLineToPoint(nFloat, nFloat1);
                }
                cGPath1.CloseSubpath();
                CGColor borderColor = BorderColor;
                borderLayer = new CAShapeLayer
                {
                    FillRule = CAShapeLayer.FillRuleEvenOdd,
                    FillColor = borderColor,
                    Path = cGPath1
                };
                layer.AddSublayer(borderLayer);
            }
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            NFloat width = size.Width;
            NFloat height = size.Height;
            return CoreGraphicsExtensions.ToCGSize(CrossPlatformMeasure.Invoke(width, height));
        }
    }
}
