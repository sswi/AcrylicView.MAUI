using Android.Graphics.Drawables;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using Xe.AcrylicView.Platforms.Android;

namespace Xe.AcrylicView.Controls
{
    public partial class AcrylicViewHandler : ViewHandler<IAcrylicView, RelativeLayout>
    {

        private RealtimeBlurView realtimeBlurView;
        private Android.Views.View colorblendLayer;
        private int colorblendLayerAlpha = 64;



        protected override RelativeLayout CreatePlatformView()
        {

            colorblendLayer = new Android.Views.View(Context);

            realtimeBlurView = new RealtimeBlurView(Context);
            realtimeBlurView.SetBlurRadius(160);
            realtimeBlurView.SetOverlayColor(Colors.Transparent.ToAndroid());
            realtimeBlurView.SetDownsampleFactor(1);
            var acrylicView = new RelativeLayout(Context);

            // realtimeBlurView.SetRenderEffect(RenderEffect.CreateBlurEffect(3, 3, Shader.TileMode.Repeat));
            acrylicView.AddView(realtimeBlurView);
            acrylicView.AddView(colorblendLayer);
            return acrylicView;

        }







        private static void MapTintColor(AcrylicViewHandler handler, IAcrylicView view)
        {
            handler.UpdateColorblendLayer(view);
        }


        //单独 设置颜色层的不透明度
        private static void MapTintOpacity(AcrylicViewHandler handler, IAcrylicView view)
        {
            //如果颜色为空 或者 为透明时，就不改变
            if (view.TintColor == null || view.TintColor == Colors.Transparent)
                return;

            handler.colorblendLayerAlpha = (int)((view.TintOpacity >= 0 && view.TintOpacity <= 1) ? (view.TintOpacity * 255) : 64);
            handler.colorblendLayer.Background.SetAlpha(handler.colorblendLayerAlpha);
        }



        private static void MapEffectStyle(AcrylicViewHandler handler, IAcrylicView view)
        {

            switch (view.EffectStyle)
            {
                case EffectStyle.Dark:

                    handler.UpdateEffectStyle(view, Colors.Black, 0.15);

                    break;
                case EffectStyle.ExtraDark:
                    handler.UpdateEffectStyle(view, Colors.Black, 0.3);
                    break;
                case EffectStyle.Light:
                    handler.UpdateEffectStyle(view, Colors.White, 0.1);
                    break;
                case EffectStyle.ExtraLight:
                    handler.UpdateEffectStyle(view, Colors.White, 0.3);
                    break;
            }
        }


        private void UpdateEffectStyle(IAcrylicView view, Microsoft.Maui.Graphics.Color color, double tintOpacity)
        {
            //颜色层
            var d = new GradientDrawable();
            d.SetTint(color.ToAndroid());

            //圆角
            d.SetCornerRadius(view.CornerRadius);
            //颜色层与模糊层叠加
            colorblendLayer.SetBackgroundDrawable(d);

            //设置颜色层不透明度
            colorblendLayer.Background.SetAlpha(colorblendLayerAlpha);


            colorblendLayerAlpha = (int)((tintOpacity >= 0 && tintOpacity <= 1) ? (tintOpacity * 255) : 64);
            colorblendLayer.Background.SetAlpha(colorblendLayerAlpha);
        }





        private static void MapCornerRadius(AcrylicViewHandler handler, IAcrylicView view)
        {

            //绘制模糊层
            handler.realtimeBlurView.SetCornerRadius(view.CornerRadius);

            //更新颜色混合层
            handler.UpdateColorblendLayer(view);
        }


        /// <summary>
        /// 更新颜色混合层
        /// </summary>
        /// <param name="view"></param>
        private void UpdateColorblendLayer(IAcrylicView view)
        {
            if (view.TintColor == null || view.TintColor == Colors.Transparent)
            {
                colorblendLayer.SetBackgroundDrawable(null);
                return;
            }

            //颜色层
            var d = new GradientDrawable();
            d.SetTint(view.TintColor.ToAndroid());

            //圆角
            d.SetCornerRadius(view.CornerRadius);
            //颜色层与模糊层叠加
            colorblendLayer.SetBackgroundDrawable(d);

            //设置颜色层不透明度
            colorblendLayer.Background.SetAlpha(colorblendLayerAlpha);

        }
    }
}
