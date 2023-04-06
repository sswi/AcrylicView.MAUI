using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using Microsoft.Maui.Platform;
using Paint = Android.Graphics.Paint;
using Path = Android.Graphics.Path;
using Color = Android.Graphics.Color;
using RectF = Android.Graphics.RectF;
using Xe.AcrylicView.Controls;
using Microsoft.Maui.Controls;

namespace Xe.AcrylicView.Platforms.Android.Drawable
{
    public class BorderDrawable : ColorDrawable
    {

        public BorderDrawable(Context context, IAcrylicView border)
        {
            //将背景色转换成 本机颜色
            var nativeColor = GetNativeColor(PaintExtensions.ToColor(border.Background), Color.Transparent);
            //将边框色转换为本机颜色
            var nativeColor2 = GetNativeColor(border.BorderColor, Color.Transparent);
            //初始化
            Initialize(context, nativeColor2, nativeColor, border.BorderThickness, border.CornerRadius);
        }


        public BorderDrawable(Context context,Thickness cornerRadius,Color color)
        {
            Initialize(context, Colors.Transparent.ToPlatform(), color,new Thickness(0), cornerRadius);
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
                var paint = new Paint(); //油漆笔

                paint.SetStyle(Paint.Style.Fill);  //填充

                paint.Color = backgroundColor;  //填充颜色

                paint.AntiAlias = true; //抗锯齿

                //绘制圆角
                var path = new Path();
                path.AddRoundRect(GetBorderInnerRect(), GetBorderInnerRadii(), Path.Direction.Cw);
                canvas.DrawPath(path, paint);
            }
            if (borderThickness.Left > 0.0 || borderThickness.Top > 0.0 || borderThickness.Right > 0.0 || borderThickness.Bottom > 0.0)
            {
                var paint2 = new Paint() { Color = borderColor };
                paint2.SetStyle(Paint.Style.Fill);
                paint2.AntiAlias = true;
                var path2 = new Path();
                path2.AddRoundRect(GetBorderOuterRect(), GetBorderOuterRadii(), Path.Direction.Cw);
                path2.AddRoundRect(GetBorderInnerRect(), GetBorderInnerRadii(), Path.Direction.Ccw);
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
            return ColorExtensions.ToPlatform(mauiColor);
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
            this.cornerRadius = cornerRadius;


            borderTopLeftRadius = ContextExtensions.ToPixels(context, this.cornerRadius.Left);
            borderTopRightRadius = ContextExtensions.ToPixels(context, this.cornerRadius.Top);
            borderBottomRightRadius = ContextExtensions.ToPixels(context, this.cornerRadius.Right);
            borderBottomLeftRadius = ContextExtensions.ToPixels(context, this.cornerRadius.Bottom);
            borderLeftWidth = ContextExtensions.ToPixels(context, this.borderThickness.Left);
            borderTopWidth = ContextExtensions.ToPixels(context, this.borderThickness.Top);
            borderRightWidth = ContextExtensions.ToPixels(context, this.borderThickness.Right);
            borderBottomWidth = ContextExtensions.ToPixels(context, this.borderThickness.Bottom);
        }

        /// <summary>
        /// 获取边框外矩形
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
            return new float[]
            {
                borderTopLeftRadius,
                borderTopLeftRadius,
                borderTopRightRadius,
                borderTopRightRadius,
                borderBottomRightRadius,
                borderBottomRightRadius,
                borderBottomLeftRadius,
                borderBottomLeftRadius
            };
        }

        /// <summary>
        /// 获取边框线条
        /// </summary>
        private RectF GetBorderInnerRect()
        {
            return new RectF(borderLeftWidth, borderTopWidth, Bounds.Width() - borderRightWidth, Bounds.Height() - borderBottomWidth);
        }

        /// <summary>
        /// 获取边框内半径
        /// </summary>
        private float[] GetBorderInnerRadii()
        {
            return new float[]
            {
                Math.Max(0f, borderTopLeftRadius - borderLeftWidth),
                Math.Max(0f, borderTopLeftRadius - borderTopWidth),
                Math.Max(0f, borderTopRightRadius - borderRightWidth),
                Math.Max(0f,borderTopRightRadius - borderTopWidth),
                Math.Max(0f, borderBottomRightRadius - borderRightWidth),
                Math.Max(0f, borderBottomRightRadius - borderBottomWidth),
                Math.Max(0f, borderBottomLeftRadius -borderLeftWidth),
                Math.Max(0f, borderBottomLeftRadius - borderBottomWidth)
            };
        }


        private Color borderColor;


        private Color backgroundColor;


        private Thickness borderThickness;


        private Thickness cornerRadius;


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
