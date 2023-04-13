using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Lights;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Math = System.Math;
using Paint = Android.Graphics.Paint;
using Path = Android.Graphics.Path;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;
using View = Android.Views.View;

namespace Xe.AcrylicView.Platforms.Android
{
    public class RealtimeBlurView : View
    {
        private Path mPath = new Path();

        private float[] mRadii = new float[8];

        private float mDownsampleFactor; // default 4

        private int mOverlayColor; // default #aaffffff

        private float mBlurRadius; // default 10dp (0 < r <= 25)

        //  private float mCornerRadius; // default 0

        private readonly IBlurImpl mBlurImpl;

        //  private readonly string _formsId;

        private bool mDirty;

        private Bitmap mBitmapToBlur, mBlurredBitmap;

        private Canvas mBlurringCanvas;

        private bool mIsRendering;

        private readonly Paint mPaint;

        private readonly Rect mRectSrc = new(), mRectDst = new();

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

        [Obsolete("此类库 在>=Android12 已经不再使用，谷歌已经更新了一套新的模糊操作类库")]
        public RealtimeBlurView(Context context, string formsId = null) : base(context)
        {
            // provide your own by override getBlurImpl()
            mBlurImpl = GetBlurImpl();

            mPaint = new Paint();

            // _formsId = formsId;
            _isContainerShown = true;
            _autoUpdate = true;

            preDrawListener = new PreDrawListener(this);
        }

        public RealtimeBlurView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
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
            {
                throw new ArgumentException("Downsample factor must be greater than 0.");
            }

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
            //判断上层视图是否为空 或 视图树为空
            if (decorView.IsNullOrDisposed() || decorView.ViewTreeObserver.IsNullOrDisposed())
            {
                return;
            }
            //添加预绘制侦听器
            decorView.ViewTreeObserver.AddOnPreDrawListener(preDrawListener);
        }

        private void UnsubscribeToPreDraw(View decorView)
        {
            if (decorView.IsNullOrDisposed() || decorView.ViewTreeObserver.IsNullOrDisposed())
            {
                return;
            }

            decorView.ViewTreeObserver.RemoveOnPreDrawListener(preDrawListener);
        }

        public void Destroy()
        {
            if (_weakDecorView != null && _weakDecorView.TryGetTarget(out var mDecorView))
            {
                UnsubscribeToPreDraw(mDecorView);
            }

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
            if (mBlurRadius != radius)
            {
                mBlurRadius = radius;
                mDirty = true;
                if (invalidate)
                {
                    Invalidate();
                }
            }
        }

        public void SetOverlayColor(int color, bool invalidate = true)
        {
            if (mOverlayColor != color)
            {
                mOverlayColor = color;
                if (invalidate)
                {
                    Invalidate();
                }
            }
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

                //  using var handler = new Handler();
                handler.PostDelayed(() =>
                {
                    SubscribeToPreDraw(mDecorView);
                    mDifferentRoot = mDecorView.RootView != RootView;
                    if (mDifferentRoot)
                    {
                        mDecorView.PostInvalidate();
                    }
                },
                    //模糊处理延迟毫秒
                    //AndroidMaterialFrameRenderer.BlurProcessingDelayMilliseconds
                    10
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
            if (_autoUpdate)
            {
                return;
            }

            _autoUpdate = true;

            using var handler = new Handler(Looper.MainLooper);
            //获取根视图，实时获取 ，间隔100ms      
            handler.PostDelayed(
                () =>
                {
                    var mDecorView = GetRootView();
                    if (mDecorView == null || !_autoUpdate)             
                        return;       

                    SubscribeToPreDraw(mDecorView);
                },
                //模糊自动更新延迟（毫秒）
                100
                //AndroidMaterialFrameRenderer.BlurAutoUpdateDelayMilliseconds
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

            if (mBlurringCanvas == null
                || mBlurredBitmap == null
                || mBlurredBitmap.Width != scaledWidth
                || mBlurredBitmap.Height != scaledHeight)
            {
                dirty = true;
                ReleaseBitmap();

                bool r = false;
                try
                {
                    mBitmapToBlur = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);
                    if (mBitmapToBlur == null)
                    {
                        return false;
                    }

                    mBlurringCanvas = new Canvas(mBitmapToBlur);

                    mBlurredBitmap = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);
                    if (mBlurredBitmap == null)
                    {
                        return false;
                    }

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
                    {
                        Release();
                    }
                }

                if (!r)
                {
                    return false;
                }
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
            }

            public PreDrawListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public bool OnPreDraw()
            {
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
                    decor.GetLocationOnScreen(locations);
                    int x = -locations[0];
                    int y = -locations[1];

                    blurView.GetLocationOnScreen(locations);
                    x += locations[0] > 5 ? locations[0] - 5 : locations[0];  //-5  为了不受BorderColor的影响
                    y += locations[1] > 5 ? locations[1] - 5 : locations[1];

                    // just erase transparent
                    blurView.mBitmapToBlur.EraseColor(blurView.mOverlayColor & 0xffffff);

                    int rc = blurView.mBlurringCanvas.Save();
                    blurView.mIsRendering = true;
                    RENDERING_COUNT++;
                    try
                    {
                        blurView.mBlurringCanvas.Scale(1f * blurView.mBitmapToBlur.Width / blurView.Width, 1f * blurView.mBitmapToBlur.Height / blurView.Height);
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

            if (ctx is Activity activity)
            {
                return activity.Window.DecorView;
            }
            else
            {
                return null;
            }
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
            {
                UnsubscribeToPreDraw(mDecorView);
            }
            Release();
            base.OnDetachedFromWindow();
        }

        public override void Draw(Canvas canvas)
        {
            if (mIsRendering)
            {
                return;
            }

            if (RENDERING_COUNT > 0)
            {
            }
            else
            {
                base.Draw(canvas);
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            DrawRoundedBlurredBitmap(canvas, mBlurredBitmap, mOverlayColor);
        }

        //绘制圆角模糊视图
        private void DrawRoundedBlurredBitmap(Canvas canvas, Bitmap blurredBitmap, int overlayColor)
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
            if (mRadii == radius)
                return;

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
            preDrawListener.OnPreDraw();
        }

    }
}