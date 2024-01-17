using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Path = Android.Graphics.Path;
using RectF = Android.Graphics.RectF;

namespace Xe.AcrylicView.Platforms.Android.Drawable
{
    public class CornerFrameLayout : FrameLayout
    {
        public CornerFrameLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.SetClipChildren(true);
        }
        public CornerFrameLayout(Context context) : base(context)
        {
            this.SetClipChildren(true);
        }



        private readonly Path mPath = new Path();
        private readonly float[] mRadii = new float[8];

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (!mPath.IsEmpty)
                canvas.ClipPath(mPath);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            mPath.Reset();
            mPath.AddRoundRect(new RectF(0, 0, w, h), mRadii, Path.Direction.Cw);
        }


        /// <summary>
        ///  set each corner radius.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topRight"></param>
        /// <param name="bottomRight"></param>
        /// <param name="bottomLeft"></param>
        public void SetRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            mRadii[0] = topLeft;
            mRadii[1] = topLeft;

            mRadii[2] = topRight;
            mRadii[3] = topRight;

            mRadii[4] = bottomRight;
            mRadii[5] = bottomRight;

            mRadii[6] = bottomLeft;
            mRadii[7] = bottomLeft;
            Invalidate();
        }




        /// <summary>
        /// 设置四个圆角
        /// Set each corner radius.
        /// </summary>
        public void SetRadius(float topLeftX, float topLeftY, float topRightX, float topRightY, float bottomRightX, float bottomRightY, float bottomLeftX, float bottomLeftY)
        {
            mRadii[0] = topLeftX;
            mRadii[1] = topLeftY;

            mRadii[2] = topRightX;
            mRadii[3] = topRightY;

            mRadii[4] = bottomRightX;
            mRadii[5] = bottomRightY;

            mRadii[6] = bottomLeftX;
            mRadii[7] = bottomLeftY;
            Invalidate();
        }
    }
}