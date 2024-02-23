using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Microsoft.Maui.Platform;
using Xe.AcrylicView.Controls;
using Color = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;
using Path = Android.Graphics.Path;
using RectF = Android.Graphics.RectF;

namespace Xe.AcrylicView.Platforms.Android.Drawable
{
    public class BorderDrawable : ColorDrawable
    {
        public BorderDrawable(Context context, IAcrylicView border)
        {
            //将背景色转换成 本机颜色
            var nativeColor = GetNativeColor(border.Background.ToColor(), Color.Transparent);
            //初始化
            Initialize(context, border.BorderColor.ToPlatform(), nativeColor, border.BorderThickness, border.CornerRadius);
        }

        public BorderDrawable(Context context, Thickness cornerRadius, Color color)
        {
            Initialize(context, Color.Transparent, color, new Thickness(0), cornerRadius);
        }

        /// <summary>
        /// 绘制背景色和圆角
        /// </summary>
        /// <param name="canvas"></param>
        public override void Draw(Canvas canvas)
        {
            if (Bounds.IsEmpty)
            {
                return;
            }
            //绘制背景色
            if (backgroundColor != 0)
            {
                //画笔
                var paint = new Paint
                {
                    Color = backgroundColor,  //填充颜色
                    AntiAlias = true //抗锯齿
                };

                paint.SetStyle(Paint.Style.Fill);  //填充

                //绘制圆角
                var path = new Path();
                path.AddRoundRect(GetBorderInnerRect(), GetBorderInnerRadii(), Path.Direction.Cw);
                canvas.DrawPath(path, paint);
            }
            if (borderThickness.Left > 0.0 || borderThickness.Top > 0.0 || borderThickness.Right > 0.0 || borderThickness.Bottom > 0.0)
            {
                var paint2 = new Paint()
                {
                    Color = borderColor,
                    AntiAlias = true
                };
                paint2.SetStyle(Paint.Style.Fill);

                var path2 = new Path();
                //外矩形
                path2.AddRoundRect(GetBorderOuterRect(), GetBorderOuterRadii(), Path.Direction.Cw);
                //内矩形
                path2.AddRoundRect(GetBorderInnerRect(), GetBorderInnerRadii(), Path.Direction.Ccw);
                //绘制先切部分，则为边框
                canvas.DrawPath(path2, paint2);
            }
        }

        /// <summary>
        /// 将Maui颜色转换为 本机颜色
        /// </summary>
        /// <param name="mauiColor"></param>
        /// <param name="nativeDefaultColor"></param>
        /// <returns></returns>
        internal static Color GetNativeColor(Microsoft.Maui.Graphics.Color mauiColor, Color nativeDefaultColor)
        {
            if (mauiColor == null)
            {
                return nativeDefaultColor;
            }
            return mauiColor.ToPlatform();
        }

        /// <summary>
        /// 获取剪辑路径
        /// </summary>
        /// <returns></returns>
        internal Path GetClipPath()
        {
            var path = new Path();
            path.AddRoundRect(GetBorderInnerRect(), GetBorderInnerRadii(), Path.Direction.Cw);
            return path;
        }

        /// <summary>
        /// 初始化各项值
        /// </summary>
        private void Initialize(Context context, Color borderColor, Color backgroundColor, Thickness borderThickness, Thickness cornerRadius)
        {
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.borderThickness = borderThickness;
            borderTopLeftRadius = context.ToPixels(cornerRadius.Left);
            borderTopRightRadius = context.ToPixels(cornerRadius.Top);
            borderBottomRightRadius = context.ToPixels(cornerRadius.Right);
            borderBottomLeftRadius = context.ToPixels(cornerRadius.Bottom);
            borderLeftWidth = context.ToPixels(borderThickness.Left);
            borderTopWidth = context.ToPixels(borderThickness.Top);
            borderRightWidth = context.ToPixels(borderThickness.Right);
            borderBottomWidth = context.ToPixels(borderThickness.Bottom);
        }

        /// <summary>
        /// 获取边框外矩形区域
        /// </summary>
        private RectF GetBorderOuterRect()
        {
            return new RectF(0f, 0f, Bounds.Width(), Bounds.Height());
        }

        /// <summary>
        /// 获取边框外半径
        /// </summary>
        private float[] GetBorderOuterRadii()
        {
            return
            [
                borderTopLeftRadius,
                borderTopLeftRadius,
                borderTopRightRadius,
                borderTopRightRadius,
                borderBottomRightRadius,
                borderBottomRightRadius,
                borderBottomLeftRadius,
                borderBottomLeftRadius
            ];
        }

        /// <summary>
        /// 获取边框内矩形区域
        /// </summary>
        private RectF GetBorderInnerRect()
        {
            return new RectF(borderLeftWidth, borderTopWidth, Bounds.Width() - borderRightWidth, Bounds.Height() - borderBottomWidth);
        }

        /// <summary>
        /// 获取边框四园角内半径
        /// </summary>
        private float[] GetBorderInnerRadii()
        {
            return
            [
                Math.Max(0f, borderTopLeftRadius - borderLeftWidth),
                Math.Max(0f, borderTopLeftRadius - borderTopWidth),
                Math.Max(0f, borderTopRightRadius - borderRightWidth),
                Math.Max(0f, borderTopRightRadius - borderTopWidth),
                Math.Max(0f, borderBottomRightRadius - borderRightWidth),
                Math.Max(0f, borderBottomRightRadius - borderBottomWidth),
                Math.Max(0f, borderBottomLeftRadius -borderLeftWidth),
                Math.Max(0f, borderBottomLeftRadius - borderBottomWidth)
            ];
        }

        private Color borderColor;

        private Color backgroundColor;

        private Thickness borderThickness;

        private float borderTopLeftRadius;

        private float borderTopRightRadius;

        private float borderBottomRightRadius;

        private float borderBottomLeftRadius;

        private float borderLeftWidth;

        private float borderTopWidth;

        private float borderRightWidth;

        private float borderBottomWidth;
    }
}