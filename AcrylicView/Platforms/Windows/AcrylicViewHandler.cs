using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace Xe.AcrylicView.Controls
{
    public partial class AcrylicViewHandler : ViewHandler<IAcrylicView, Rectangle>
    {
        private AcrylicBrush _acrylicBrush;

        protected override Rectangle CreatePlatformView()
        {
            _acrylicBrush = new AcrylicBrush();
            var rectangle = new Rectangle()
            {
                Fill = _acrylicBrush
            };
            return rectangle;
        }

        static void MapTintColor(AcrylicViewHandler handler, IAcrylicView view)
        {
            handler._acrylicBrush.TintColor = view.TintColor.ToWindowsColor();
        }

        static void MapTintOpacity(AcrylicViewHandler handler, IAcrylicView view)
        {
            handler._acrylicBrush.TintOpacity = view.TintOpacity;
        }



        static void MapCornerRadius(AcrylicViewHandler handler, IAcrylicView view)
        {
            var nativView = handler?.PlatformView;
            nativView.RadiusX = view.CornerRadius;
            nativView.RadiusY = view.CornerRadius;
        }



        private static void MapEffectStyle(AcrylicViewHandler handler, IAcrylicView view)
        {

            switch (view.EffectStyle)
            {
                case EffectStyle.Dark:
                    handler._acrylicBrush.TintColor = Colors.Black.ToWindowsColor();
                    handler._acrylicBrush.TintOpacity = 0.3;
                    break;
                case EffectStyle.ExtraDark:
                    handler._acrylicBrush.TintColor = Colors.Black.ToWindowsColor();
                    handler._acrylicBrush.TintOpacity = 0.6;
                    break;
                case EffectStyle.Light:
                    handler._acrylicBrush.TintColor = Colors.Transparent.ToWindowsColor();
                    handler._acrylicBrush.TintOpacity = 0.0;
                    break;
                case EffectStyle.ExtraLight:
                    handler._acrylicBrush.TintColor = Colors.White.ToWindowsColor();
                    handler._acrylicBrush.TintOpacity = 0.3;
                    break;
            }


        }





    }
}
