using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Platform;
using Rect = Microsoft.Maui.Graphics.Rect;
using View = Android.Views.View;

namespace Xe.AcrylicView.Platforms.Android.Drawable
{
    public class BorderViewGroup(Context context) : FrameLayout(context)
    {
        /// <summary>
        /// 跨平台测量
        /// </summary>
        internal Func<double, double, Size> CrossPlatformMeasure { get; set; }

        /// <summary>
        /// 跨平台排列
        /// </summary>
        internal Func<Rect, Size> CrossPlatformArrange { get; set; }

        public BorderDrawable BorderDrawable
        {
            get => borderDrawable;

            set
            {
                if (borderDrawable != value)
                {
                    borderDrawable = value;
                    Background = borderDrawable;
                }
            }
        }

        /// <summary>
        /// 调度绘制
        /// </summary>
        /// <param name="canvas"></param>
        protected override void DispatchDraw(Canvas canvas)
        {
            if (borderDrawable != null)
            {
                canvas.ClipPath(borderDrawable.GetClipPath());
            }
            base.DispatchDraw(canvas);
        }

        /// <summary>
        /// 测量
        /// </summary>
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            Context context = Context;
            if (context == null)
            {
                return;
            }
            if (CrossPlatformMeasure == null)
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
                return;
            }
            double num = MeasureSpecExtensions.ToDouble(widthMeasureSpec, context);
            double num2 = MeasureSpecExtensions.ToDouble(heightMeasureSpec, context);
            View childAt = GetChildAt(0);
            Size size = Size.Zero;
            if (childAt != null && childAt.Visibility != ViewStates.Gone)
            {
                size = CrossPlatformMeasure(num, num2);
            }
            double defaultSize = GetDefaultSize(MeasureSpecExtensions.GetMode(widthMeasureSpec), ContextExtensions.FromPixels(context, SuggestedMinimumWidth), size.Width, num);
            double defaultSize2 = GetDefaultSize(MeasureSpecExtensions.GetMode(heightMeasureSpec), ContextExtensions.FromPixels(context, SuggestedMinimumHeight), size.Height, num2);
            float num3 = ContextExtensions.ToPixels(context, defaultSize);
            float num4 = ContextExtensions.ToPixels(context, defaultSize2);
            //设置测量尺寸
            SetMeasuredDimension((int)num3, (int)num4);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            double num = ContextExtensions.FromPixels(Context, left);
            double num2 = ContextExtensions.FromPixels(Context, top);
            double num3 = ContextExtensions.FromPixels(Context, right);
            double num4 = ContextExtensions.FromPixels(Context, bottom);
            double num5 = num3 - num;
            double num6 = num4 - num2;

            Rect rect = new(0, 0, num5, num6);
            CrossPlatformArrange(rect);
        }

        /// <summary>
        /// 获取默认尺寸
        /// </summary>
        private static double GetDefaultSize(MeasureSpecMode mode, double minSize, double desiredSize, double constraint)
        {
            if (mode == MeasureSpecMode.AtMost)
            {
                return Math.Min(Math.Max(minSize, desiredSize), constraint);
            }
            if (mode == MeasureSpecMode.Unspecified)
            {
                return Math.Max(minSize, desiredSize);
            }
            return constraint;
        }

        /// <summary>
        /// 底色 圆角 画板
        /// </summary>
        private BorderDrawable borderDrawable;
    }
}