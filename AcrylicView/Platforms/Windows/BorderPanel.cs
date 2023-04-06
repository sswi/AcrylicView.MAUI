using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Rect = Microsoft.Maui.Graphics.Rect;
using Size = Windows.Foundation.Size;

namespace Xe.AcrylicView.Platforms.Windows
{
    internal class BorderPanel : Panel
    {
        internal Func<double, double, Microsoft.Maui.Graphics.Size> CrossPlatformMeasure { get; set; }

        internal Func<Rect, Microsoft.Maui.Graphics.Size> CrossPlatformArrange { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            return SizeExtensions.ToPlatform(CrossPlatformMeasure(availableSize.Width, availableSize.Height));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {       
            Rect arg = new(0.0, 0.0, finalSize.Width, finalSize.Height);
            return SizeExtensions.ToPlatform(CrossPlatformArrange(arg));
        }
    }
}