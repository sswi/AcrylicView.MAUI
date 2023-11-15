using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Color = Android.Graphics.Color;
using Math = System.Math;
using Paint = Android.Graphics.Paint;
using Path = Android.Graphics.Path;
using RectF = Android.Graphics.RectF;
using View = Android.Views.View;

namespace Xe.AcrylicView.Platforms.Android
{
    public class RealtimeBlurView : View
    {
        private readonly float[] mRadii = new float[8];

        private float mDownsampleFactor; // default 4

        private int mOverlayColor; // default #aaffffff

        private float mBlurRadius; // default 10dp (0 < r <= 25)

        private readonly IBlurImpl mBlurImpl;

        private bool mDirty;

        private Bitmap mBitmapToBlur, mBlurredBitmap;

        private Canvas mBlurringCanvas;

        private bool mIsRendering;

        private readonly Paint mPaint;

        // mDecorView should be the root view of the activity (even if you are on a different window like a dialog)
        // private View mDecorView;
        private JniWeakReference<View> _weakDecorView;

        // If the view is on different root view (usually means we are on a PopupWindow),
        // we need to manually call invalidate() in onPreDraw(), otherwise we will not be able to see the changes
        private bool mDifferentRoot;

        private bool _isContainerShown;

        private bool _autoUpdate;

        private static int RENDERING_COUNT;

        private static int BLUR_IMPL;
        private Thickness borderThickness = new();

        public delegate void SetContentVisibel(bool visible);

        private readonly SetContentVisibel _contentSetVisibel;

        [Obsolete("此类库 在>=Android12 已经不再使用，谷歌已经更新了一套新的模糊操作类库")]
        public RealtimeBlurView(Context context, SetContentVisibel visibel, string formsId = null) : base(context)
        {
            // provide your own by override getBlurImpl()
            mBlurImpl = GetBlurImpl();

            mPaint = new Paint();

            // _formsId = formsId;
            _isContainerShown = true;
            _autoUpdate = true;

            preDrawListener = new PreDrawListener(this);
            _contentSetVisibel = visibel;
        }

        public RealtimeBlurView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        /// <param name="borderThickness"></param>
        public void SetBorderThickness(Thickness borderThickness)
        {
            this.borderThickness = borderThickness;
            preDrawListener.OnPreDraw(borderThickness, _contentSetVisibel);
        }

        protected IBlurImpl GetBlurImpl()
        {
            try
            {
                AndroidStockBlurImpl impl = new();
                Bitmap bmp = Bitmap.CreateBitmap(4, 4, Bitmap.Config.Argb8888);
                impl.Prepare(Context, bmp, 4);
                impl.Release();
                bmp.Recycle();
                BLUR_IMPL = 3;
            }
            catch
            {
            }

            if (BLUR_IMPL == 0)
            {
                // fallback to empty impl, which doesn't have blur effect
                BLUR_IMPL = -1;
            }

            return BLUR_IMPL switch
            {
                3 => new AndroidStockBlurImpl(),
                _ => new EmptyBlurImpl(),
            };
        }

        public void SetDownsampleFactor(float factor)
        {
            if (factor <= 0)
                throw new ArgumentException("Downsample factor must be greater than 0.");

            if (mDownsampleFactor != factor)
            {
                mDownsampleFactor = factor;
                mDirty = true; // may also change blur radius
                ReleaseBitmap();
                Invalidate();
            }
        }

        private void SubscribeToPreDraw(View decorView)
        {
            if (decorView.IsNullOrDisposed() || decorView.ViewTreeObserver.IsNullOrDisposed()) return;

            decorView.ViewTreeObserver.AddOnPreDrawListener(preDrawListener);
        }

        private void UnsubscribeToPreDraw(View decorView)
        {
            if (decorView.IsNullOrDisposed() || decorView.ViewTreeObserver.IsNullOrDisposed()) return;

            decorView.ViewTreeObserver.RemoveOnPreDrawListener(preDrawListener);
        }

        public void Destroy()
        {
            if (_weakDecorView != null && _weakDecorView.TryGetTarget(out var mDecorView))
                UnsubscribeToPreDraw(mDecorView);

            Release();
            _weakDecorView = null;
        }

        public void Release()
        {
            SetRootView(null);
            ReleaseBitmap();
            mBlurImpl?.Release();
        }

        public void SetBlurRadius(float radius, bool invalidate = true)
        {
            if (mBlurRadius == radius) return;
            mBlurRadius = radius;
            mDirty = true;
            if (invalidate)
                Invalidate();
        }

        public void SetOverlayColor(int color, bool invalidate = true)
        {
            if (mOverlayColor == color) return;
            mOverlayColor = color;
            if (invalidate)
                Invalidate();
        }

        public void SetRootView(View rootView)
        {
            var mDecorView = GetRootView();
            if (mDecorView != rootView)
            {
                UnsubscribeToPreDraw(mDecorView);

                _weakDecorView = new JniWeakReference<View>(rootView);

                if (IsAttachedToWindow)
                {
                    OnAttached(rootView);
                }
            }
        }

        private View GetRootView()
        {
            View mDecorView = null;
            _weakDecorView?.TryGetTarget(out mDecorView);
            return mDecorView;
        }

        private void OnAttached(View mDecorView)
        {
            if (mDecorView != null)
            {
                using var handler = new Handler(Looper.MainLooper);
                handler.PostDelayed(() =>
                {
                    SubscribeToPreDraw(mDecorView);
                    mDifferentRoot = mDecorView.RootView != RootView;
                    if (mDifferentRoot)
                        mDecorView.PostInvalidate();
                },
                    8  //AndroidMaterialFrameRenderer.BlurProcessingDelayMilliseconds 模糊处理延迟毫秒
                 );
            }
            else
            {
                mDifferentRoot = false;
            }
        }

        protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);

            if (changedView.GetType().Name == "PageContainer")
            {
                _isContainerShown = visibility == ViewStates.Visible;
                SetAutoUpdate(_isContainerShown);
            }
        }

        private void SetAutoUpdate(bool autoUpdate)
        {
            if (autoUpdate)
            {
                EnableAutoUpdate();
                return;
            }

            DisableAutoUpdate();
        }

        private void EnableAutoUpdate()
        {
            if (_autoUpdate) return;

            _autoUpdate = true;
            using var handler = new Handler(Looper.MainLooper);
            //获取根视图，实时获取 ，间隔80ms
            handler.PostDelayed(() =>
                 {
                     var mDecorView = GetRootView();
                     if (mDecorView == null || !_autoUpdate) return;
                     SubscribeToPreDraw(mDecorView);
                 },
                80   //AndroidMaterialFrameRenderer.BlurAutoUpdateDelayMilliseconds 模糊自动更新延迟（毫秒）
                );
        }

        private void DisableAutoUpdate()
        {
            if (!_autoUpdate)
                return;

            _autoUpdate = false;
            var mDecorView = GetRootView();

            if (mDecorView == null)
                return;

            UnsubscribeToPreDraw(mDecorView);
        }

        private void ReleaseBitmap()
        {
            if (!mBitmapToBlur.IsNullOrDisposed())
            {
                mBitmapToBlur.Recycle();
                mBitmapToBlur = null;
            }

            if (!mBlurredBitmap.IsNullOrDisposed())
            {
                mBlurredBitmap.Recycle();
                mBlurredBitmap = null;
            }
        }

        protected bool Prepare()
        {
            if (mBlurRadius == 0)
            {
                Release();
                return false;
            }
            float downsampleFactor = mDownsampleFactor;
            float radius = mBlurRadius / downsampleFactor;
            if (radius > 25)
            {
                downsampleFactor = downsampleFactor * radius / 25;
                radius = 25;
            }
            int width = Width;
            int height = Height;
            int scaledWidth = Math.Max(1, (int)(width / downsampleFactor));
            int scaledHeight = Math.Max(1, (int)(height / downsampleFactor));
            bool dirty = mDirty;

            if (mBlurringCanvas == null || mBlurredBitmap == null || mBlurredBitmap.Width != scaledWidth || mBlurredBitmap.Height != scaledHeight)
            {
                dirty = true;
                ReleaseBitmap();
                bool r = false;
                try
                {
                    mBitmapToBlur = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);

                    if (mBitmapToBlur == null) return false;

                    mBlurringCanvas = new Canvas(mBitmapToBlur);
                    mBlurredBitmap = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);

                    if (mBlurredBitmap == null) return false;

                    r = true;
                }
                catch /*(OutOfMemoryError e)*/
                {
                    // Bitmap.createBitmap() may cause OOM error
                    // Simply ignore and fallback
                }
                finally
                {
                    if (!r)
                        Release();
                }

                if (!r) return false;
            }

            if (dirty)
            {
                if (mBlurImpl.Prepare(Context, mBitmapToBlur, radius))
                {
                    mDirty = false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        protected void Blur(Bitmap bitmapToBlur, Bitmap blurredBitmap)
        {
            mBlurImpl.Blur(bitmapToBlur, blurredBitmap);
        }

        private readonly PreDrawListener preDrawListener;

        private class PreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
        {
            private readonly JniWeakReference<RealtimeBlurView> _weakBlurView;

            public PreDrawListener(RealtimeBlurView blurView)
            {
                _weakBlurView = new JniWeakReference<RealtimeBlurView>(blurView);
                _density = DeviceDisplay.Current.MainDisplayInfo.Density;
            }

            public PreDrawListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            /// <summary>
            /// 屏幕缩放率
            /// </summary>
            private readonly double _density;

            private Thickness _borderThickness = new();

            /// <summary>
            /// 控制顶层视图透明度
            /// </summary>
            private SetContentVisibel _setContentVisibel;

            public void OnPreDraw(Thickness thickness, SetContentVisibel setContentvisibel)
            {
                _borderThickness = thickness;
                _setContentVisibel = setContentvisibel;
                OnPreDraw();
            }

            private int i = 0;

            public bool OnPreDraw()
            {
                if (i == 2)
                {
                    i = 0;
                    return true;
                }
                _setContentVisibel(false);
                i++;
                if (!_weakBlurView.TryGetTarget(out var blurView))
                {
                    return false;
                }

                if (!blurView._isContainerShown)
                {
                    return false;
                }
                var mDecorView = blurView.GetRootView();

                int[] locations = new int[2];
                Bitmap oldBmp = blurView.mBlurredBitmap;
                View decor = mDecorView;

                if (!decor.IsNullOrDisposed() && blurView.IsShown && blurView.Prepare())
                {
                    bool redrawBitmap = blurView.mBlurredBitmap != oldBmp;

                    //获取view所在的左上角位置
                    decor.GetLocationOnScreen(locations);
                    blurView.GetLocationOnScreen(locations);

                    //计算边框宽高，避免截图时候把边框也算进去造成边缘有虚化颜色
                    float x = _borderThickness.Left > 0 ? (float)(locations[0] + _borderThickness.Left * _density) : locations[0];
                    float y = _borderThickness.Top > 0 ? (float)(locations[1] + _borderThickness.Top * _density) : locations[1];

                    // just erase transparent
                    blurView.mBitmapToBlur.EraseColor(Color.Transparent);
                    int rc = blurView.mBlurringCanvas.Save();
                    blurView.mIsRendering = true;
                    RENDERING_COUNT++;
                    try
                    {
                        float _borderWidth = (float)(_density * (_borderThickness.Left + _borderThickness.Right));
                        float _borderHeight = (float)(_density * (_borderThickness.Top + _borderThickness.Bottom));
                        blurView.mBlurringCanvas.Scale((blurView.mBitmapToBlur.Width + _borderWidth) / blurView.Width, (blurView.mBitmapToBlur.Height + _borderHeight) / blurView.Height);
                        blurView.mBlurringCanvas.Translate(-x, -y);
                        decor.Background?.Draw(blurView.mBlurringCanvas);
                        decor.Draw(blurView.mBlurringCanvas);
                    }
                    finally
                    {
                        blurView.mIsRendering = false;
                        RENDERING_COUNT--;
                        blurView.mBlurringCanvas.RestoreToCount(rc);
                    }
                    blurView.Blur(blurView.mBitmapToBlur, blurView.mBlurredBitmap);

                    if (redrawBitmap || blurView.mDifferentRoot)
                    {
                        blurView.Invalidate();
                    }
                }

                _setContentVisibel(true);
                return true;
            }
        }

        protected View GetActivityDecorView()
        {
            Context ctx = Context;
            for (int i = 0; i < 4 && ctx != null && ctx is not Activity && ctx is ContextWrapper wrapper; i++)
            {
                ctx = wrapper.BaseContext;
            }

            return (ctx is Activity activity) ? activity.Window.DecorView : null;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            var mDecorView = GetRootView();
            if (mDecorView == null)
            {
                SetRootView(GetActivityDecorView());
            }
            else
            {
                OnAttached(mDecorView);
            }
        }

        protected override void OnDetachedFromWindow()
        {
            var mDecorView = GetRootView();
            if (mDecorView != null)
                UnsubscribeToPreDraw(mDecorView);

            Release();
            base.OnDetachedFromWindow();
        }

        public override void Draw(Canvas canvas)
        {
            if (mIsRendering) return;

            if (RENDERING_COUNT <= 0)
                base.Draw(canvas);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            DrawRoundedBlurredBitmap(canvas, mBlurredBitmap);
        }

        //绘制圆角模糊视图
        private void DrawRoundedBlurredBitmap(Canvas canvas, Bitmap blurredBitmap)
        {
            if (blurredBitmap != null)
            {
                var mRectF = new RectF { Right = Width, Bottom = Height };
                mPaint.Reset();
                mPaint.AntiAlias = true;
                var shader = new BitmapShader(blurredBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
                var matrix = new Matrix();
                matrix.PostScale(mRectF.Width() / blurredBitmap.Width, mRectF.Height() / blurredBitmap.Height);
                shader.SetLocalMatrix(matrix);
                mPaint.SetShader(shader);
                var path2 = new Path();
                path2.AddRoundRect(mRectF, mRadii, Path.Direction.Cw);
                canvas.DrawPath(path2, mPaint);
            }
        }

        public void SetCornerRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            var radius = new float[8] { topLeft, topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft };
            if (mRadii == radius) return;

            mDirty = true;

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

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            if (w > 0 && h > 0)
                preDrawListener.OnPreDraw(borderThickness, _contentSetVisibel);
        }
    }
}