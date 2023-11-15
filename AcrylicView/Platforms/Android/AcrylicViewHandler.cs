using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Xe.AcrylicView.Platforms.Android;
using Xe.AcrylicView.Platforms.Android.Drawable;
using BorderDrawable = Xe.AcrylicView.Platforms.Android.Drawable.BorderDrawable;
using Color = Microsoft.Maui.Graphics.Color;
using View = Android.Views.View;

namespace Xe.AcrylicView.Controls
{
    public partial class AcrylicViewHandler : ViewHandler<IAcrylicView, FrameLayout>
    {
        /// <summary>
        /// acrylicBackground layer
        /// </summary>
        private RealtimeBlurView realtimeBlurView;

        private BorderDrawable colorGradientDrawable;

        //颜色层
        private View colorBlendLayer;

        private float colorBlendLayerAlpha = 0f;

        private BorderViewGroup borderViewGroup;

        protected override FrameLayout CreatePlatformView()
        {
            colorBlendLayer = new View(Context);

            realtimeBlurView = new RealtimeBlurView(Context, SetContentVisibel);
            realtimeBlurView.SetBlurRadius(120);
            realtimeBlurView.SetOverlayColor(Colors.Transparent.ToAndroid());
            realtimeBlurView.SetDownsampleFactor(4);

            borderViewGroup = new BorderViewGroup(Context)
            {
                CrossPlatformMeasure = new Func<double, double, Size>(VirtualView.CrossPlatformMeasure),
                CrossPlatformArrange = new Func<Rect, Size>(VirtualView.CrossPlatformArrange)
            };

            var frame = new FrameLayout(Context);
            frame.AddView(realtimeBlurView);
            frame.AddView(colorBlendLayer);
            frame.AddView(borderViewGroup);

            return frame;
        }

        /// <summary>
        /// 控制获取视图层时顶层视图的透明图
        /// </summary>
        /// <param name="isVisibel"></param>
        private void SetContentVisibel(bool isVisibel)
        {
            if (borderViewGroup == null) return;
            borderViewGroup.Alpha = isVisibel ? 1f : 0f;
        }

        private static void MapTintColor(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (view.EffectStyle != EffectStyle.Custom) return;

            var nativView = handler?.PlatformView;

            if (nativView == null) return;

            handler.UpdateColorblendLayer(view);
        }

        private static void MapTintOpacity(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (view.EffectStyle != EffectStyle.Custom) return;

            if (view.TintColor == null || view.TintColor == Colors.Transparent) return;

            handler.colorBlendLayerAlpha = (float)view.TintOpacity;
            handler.colorBlendLayer.Alpha = handler.colorBlendLayerAlpha;
        }

        private static void MapEffectStyle(AcrylicViewHandler handler, IAcrylicView view)
        {
            switch (view.EffectStyle)
            {
                case EffectStyle.Dark:
                    handler.UpdateEffectStyle(view, Colors.Black, 0.15f);
                    break;

                case EffectStyle.ExtraDark:
                    handler.UpdateEffectStyle(view, Colors.Black, 0.3f);
                    break;

                case EffectStyle.Light:
                    handler.UpdateEffectStyle(view, Colors.White, 0.05f);
                    break;

                case EffectStyle.ExtraLight:
                    handler.UpdateEffectStyle(view, Colors.White, 0.3f);
                    break;

                case EffectStyle.Custom:
                    handler.UpdateColorblendLayer(view);
                    break;
            }
        }

        private void UpdateEffectStyle(IAcrylicView view, Color color, float tintOpacity)
        {
            colorGradientDrawable = new BorderDrawable(Context, view.CornerRadius, color.ToPlatform());
            colorBlendLayer.SetBackgroundDrawable(colorGradientDrawable);

            colorBlendLayerAlpha = tintOpacity;
            colorBlendLayer.Alpha = colorBlendLayerAlpha;
        }

        private static void MapContent(AcrylicViewHandler handler, IAcrylicView view)
        {
            var nativView = handler?.PlatformView;
            if (nativView == null) return;

            handler.borderViewGroup.RemoveAllViews();
            if (view.Content is IView content && view.Handler != null)
            {
                var view3 = ElementExtensions.ToPlatform(content, view.Handler.MauiContext);
                handler.borderViewGroup.AddView(view3);
            }
        }

        private static void MapBorderThickness(AcrylicViewHandler handler, IAcrylicView view)
        {
            var nativView = handler?.PlatformView;
            if (nativView == null) return;
            handler.realtimeBlurView.SetBorderThickness(view.BorderThickness);
            PropertyChanged(handler, view);
        }

        private static void MapCornerRadius(AcrylicViewHandler handler, IAcrylicView view)
        {
            var nativView = handler?.PlatformView;
            if (nativView == null) return;

            handler.UpdateColorblendLayer(view);

            var thickness = nativView.Context.ToPixels(view.CornerRadius);
            //亚克力层圆角
            handler.realtimeBlurView.SetCornerRadius((float)thickness.Left, (float)thickness.Top, (float)thickness.Right, (float)thickness.Bottom);

            //边框层
            PropertyChanged(handler, view);
        }

        private static void MapBorderColor(AcrylicViewHandler handler, IAcrylicView view)
        {
            PropertyChanged(handler, view);
        }

        private static void PropertyChanged(AcrylicViewHandler handler, IAcrylicView view)
        {
            var nativView = handler?.PlatformView;
            if (nativView == null) return;
            handler.borderViewGroup.BorderDrawable = new BorderDrawable(nativView.Context, view);
        }

        private void UpdateColorblendLayer(IAcrylicView view)
        {
            if ((view.TintColor == null || view.TintColor == Colors.Transparent) && view.EffectStyle == EffectStyle.Custom)
            {
                colorBlendLayer.SetBackgroundDrawable(null);
                return;
            }
            else
            {
                //混合色层圆角
                colorGradientDrawable = new BorderDrawable(Context, view.CornerRadius, view.TintColor.ToPlatform());
                colorBlendLayer.SetBackgroundDrawable(colorGradientDrawable);

                //设置颜色层不透明度
                colorBlendLayerAlpha = (float)view.TintOpacity;
                colorBlendLayer.Alpha = colorBlendLayerAlpha;
            }
        }
    }
}