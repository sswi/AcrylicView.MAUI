using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Media;
using Border = Microsoft.UI.Xaml.Controls.Border;
using Grid = Microsoft.UI.Xaml.Controls.Grid;
using Thickness = Microsoft.UI.Xaml.Thickness;

namespace Xe.AcrylicView.Controls
{
    public partial class AcrylicViewHandler : ViewHandler<IAcrylicView, Grid>
    {
        private readonly AcrylicBrush _acrylicBrush = new();
        private Border _border;
        private readonly Grid _contentGrid = new();
        protected override Grid CreatePlatformView()
        {
            _border = new Border()
            {
                Background = _acrylicBrush,
                Child = _contentGrid
            };

            var grid = new Grid();

            grid.Children.Add(_border);
            return grid;

            //2024.1.14
            //return new Border()
            //{
            //    Child = new Grid
            //    {
            //        Background = _acrylicBrush
            //    }
            //};
        }

        private static void MapTintColor(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (view.EffectStyle != EffectStyle.Custom) return;
            handler._acrylicBrush.TintColor = view.TintColor.ToWindowsColor();
        }

        private static void MapTintOpacity(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (view.EffectStyle != EffectStyle.Custom) return;
            handler._acrylicBrush.TintOpacity = view.TintOpacity;
        }

        private static void MapBorderThickness(AcrylicViewHandler handler, IAcrylicView view)
        {
            handler._border.BorderThickness = view.BorderThickness.ToPlatform();

            //2024.1.14
            //var nativView = handler?.PlatformView;
            //if (nativView == null) return;
            //nativView.BorderThickness = view.BorderThickness.ToPlatform();
        }


        //2025.11.15
        private static void MapSize(AcrylicViewHandler handler, IAcrylicView view)
        {

            if (view.HeightRequest >= 0)
                handler._border.Height = view.HeightRequest;
            if (view.WidthRequest >= 0)
                handler._border.Width = view.WidthRequest;
        }


        private static void MapCornerRadius(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (handler == null) return;
            handler._border.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(view.CornerRadius.Left, view.CornerRadius.Top, view.CornerRadius.Right, view.CornerRadius.Bottom);

            //2024.2.16
            //var nativView = handler?.PlatformView;
            //if (nativView == null) return;
            //nativView.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(view.CornerRadius.Left, view.CornerRadius.Top, view.CornerRadius.Right, view.CornerRadius.Bottom);
        }
        private static void MapPadding(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (handler == null) return;
            handler._contentGrid.Padding = new Thickness(view.Padding.Left, view.Padding.Top, view.Padding.Right, view.Padding.Bottom);
        }

        private static void MapContent(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (view.Content is IView content && view.Handler != null)
            {
                handler._contentGrid.Children.Clear();
                handler._contentGrid.Children.Add(ElementExtensions.ToPlatform(content, view.Handler.MauiContext));
            }

            //2024.1.14
            //var nativView = handler?.PlatformView;
            //if (nativView == null) return;

            //Grid grid = nativView.Child as Grid;
            //if (grid == null) return;
            //grid.Children.Clear();
            //grid.Padding = new Thickness(view.Padding.Left, view.Padding.Top, view.Padding.Right, view.Padding.Bottom);
            //grid.Margin = new Thickness(view.Margin.Left, view.Margin.Top, view.Margin.Right, view.Margin.Bottom);
            //if (view.Content is IView content && view.Handler != null)
            //{
            //    FrameworkElement frameworkElement = ElementExtensions.ToPlatform(content, view.Handler.MauiContext);
            //    grid.Children.Add(frameworkElement);
            //}
        }
        private static void MapBorderColor(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (handler == null) return;
            handler._border.BorderBrush = view.BorderColor.ToPlatform();

            //2024.1.14
            //var nativView = handler?.PlatformView;
            //if (nativView == null) return;
            //nativView.BorderBrush = view.BorderColor.ToPlatform();
        }

        private static void MapEffectStyle(AcrylicViewHandler handler, IAcrylicView view)
        {
            if (handler == null) return;
            if (view.EffectStyle == EffectStyle.Custom)
            {
                MapTintColor(handler, view);
                MapTintOpacity(handler, view);
                return;
            }

            switch (view.EffectStyle)
            {
                case EffectStyle.Dark:
                    handler._acrylicBrush.TintColor = Colors.Black.ToWindowsColor();
                    handler._acrylicBrush.TintOpacity = 0.35;
                    break;

                case EffectStyle.ExtraDark:
                    handler._acrylicBrush.TintColor = Colors.Black.ToWindowsColor();
                    handler._acrylicBrush.TintOpacity = 0.6;
                    break;

                case EffectStyle.Light:
                    handler._acrylicBrush.TintColor = Colors.White.ToWindowsColor();
                    handler._acrylicBrush.TintOpacity = 0.05;
                    break;

                case EffectStyle.ExtraLight:
                    handler._acrylicBrush.TintColor = Colors.White.ToWindowsColor();
                    handler._acrylicBrush.TintOpacity = 0.35;
                    break;
            }
        }
    }
}