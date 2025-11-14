namespace Xe.AcrylicView.Controls
{
    public interface IAcrylicView : IView, IContentView
    {
        Thickness CornerRadius { get; set; }

        Color TintColor { get; set; }

        Color BorderColor { get; }
        double TintOpacity { get; set; }

        EffectStyle EffectStyle { get; set; }

        Thickness BorderThickness { get; set; }
#if WINDOWS

        double HeightRequest { get;set;  }

        double WidthRequest { get;set; }
#endif
    }

    public enum EffectStyle
    {
        ExtraLight = 0,
        Light = 1,
        Dark = 2,
        ExtraDark = 3,
        Custom = 4
    }
}