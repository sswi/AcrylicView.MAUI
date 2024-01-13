namespace Xe.AcrylicView.Controls
{
    public partial class AcrylicViewHandler
    {
        public static PropertyMapper<IAcrylicView, AcrylicViewHandler> AcrylicViewMapper = new(ViewMapper)
        {
            [nameof(IAcrylicView.CornerRadius)] = MapCornerRadius,
            [nameof(IAcrylicView.TintColor)] = MapTintColor,
            [nameof(IAcrylicView.TintOpacity)] = MapTintOpacity,
            [nameof(IAcrylicView.EffectStyle)] = MapEffectStyle,
            [nameof(IAcrylicView.Content)] = MapContent,
            [nameof(IAcrylicView.BorderThickness)] = MapBorderThickness,
            [nameof(IAcrylicView.BorderColor)] = MapBorderColor,
#if WINDOWS
            [nameof(IAcrylicView.Padding)] = MapPadding,
#endif
        };

        public AcrylicViewHandler() : base(AcrylicViewMapper)
        {
        }

        public AcrylicViewHandler(PropertyMapper mapper) : base(mapper)
        {
        }
    }
}