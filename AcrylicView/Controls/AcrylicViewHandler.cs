namespace Xe.AcrylicView.Controls
{
    public partial class AcrylicViewHandler
    {
        private static readonly PropertyMapper<IAcrylicView, AcrylicViewHandler> propertyMapper = new()
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
            [nameof(IAcrylicView.HeightRequest)] = MapSize,
            [nameof(IAcrylicView.WidthRequest)] = MapSize
#endif
        };

        public AcrylicViewHandler() : base(propertyMapper)
        {
        }

        public AcrylicViewHandler(PropertyMapper mapper) : base(mapper)
        {
        }
    }
}