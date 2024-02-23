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
        internal Func<double, double, Size> CrossPlatformMeasure { get; set; }
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
            double w = widthMeasureSpec.ToDouble(context);
            double h = heightMeasureSpec.ToDouble(context);
            View childAt = GetChildAt(0);
            Size size = Size.Zero;
            if (childAt != null && childAt.Visibility != ViewStates.Gone)
            {
                size = CrossPlatformMeasure(w, h);
            }
            double width = GetDefaultSize(widthMeasureSpec.GetMode(), context.FromPixels(SuggestedMinimumWidth), size.Width, w);
            double height = GetDefaultSize(heightMeasureSpec.GetMode(), context.FromPixels(SuggestedMinimumHeight), size.Height, h);

            int measuredWidth = (int)context.ToPixels(width);
            int measuredHeight = (int)context.ToPixels(height);
            //设置测量尺寸
            SetMeasuredDimension(measuredWidth, measuredHeight);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            var width = Context.FromPixels(right) - Context.FromPixels(left);
            var height = Context.FromPixels(bottom) - Context.FromPixels(top);
            Rect rect = new(0, 0, width, height);
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