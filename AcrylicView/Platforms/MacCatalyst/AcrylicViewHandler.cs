using Microsoft.Maui.Handlers;
using UIKit;


namespace Xe.AcrylicView.Controls
{
    public partial class AcrylicViewHandler : ViewHandler<IAcrylicView, UIVisualEffectView>
    {


        protected override UIVisualEffectView CreatePlatformView()
        {

            var _blurView = new UIVisualEffectView()
            {
                ClipsToBounds = true,
                Effect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light)
            };
            return _blurView;
        }

        static void MapTintColor(AcrylicViewHandler handler, IAcrylicView view)
        {
            //  handler.layer.BackgroundColor = view.TintColor.ToPlatform();

        }

        static void MapTintOpacity(AcrylicViewHandler handler, IAcrylicView view)
        {
            var nativView = handler?.PlatformView;



        }



        static void MapCornerRadius(AcrylicViewHandler handler, IAcrylicView view)
        {
            var nativView = handler?.PlatformView;
            nativView.Layer.CornerRadius = view.CornerRadius;
        }


        private static void MapEffectStyle(AcrylicViewHandler handler, IAcrylicView view)
        {
            var nativView = handler?.PlatformView;

            var ver = UIDevice.CurrentDevice.SystemVersion;

            float.TryParse(ver, out float version);


            var style = view.EffectStyle switch
            {
                EffectStyle.Light => UIBlurEffectStyle.Light,
                EffectStyle.Dark => UIBlurEffectStyle.Dark,
                EffectStyle.ExtraLight => UIBlurEffectStyle.ExtraLight,
                EffectStyle.ExtraDark => version > 11.0 ? UIBlurEffectStyle.ExtraDark : UIBlurEffectStyle.Dark,
                _ => UIBlurEffectStyle.Light
            };


            nativView.Effect = UIBlurEffect.FromStyle(style);



        }
    }
}
