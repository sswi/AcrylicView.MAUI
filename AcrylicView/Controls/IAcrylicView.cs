
namespace Xe.AcrylicView.Controls
{
    public interface IAcrylicView : IView
    {
        float CornerRadius { get; set; }

        Color TintColor { get; set; }

        double TintOpacity { get; set; }

        EffectStyle EffectStyle { get; set; }

    }


    public enum EffectStyle
    {
        ExtraLight = 0,
        Light = 1,
        Dark = 2,
        ExtraDark = 3
    }
}
