

namespace Xe.AcrylicView.Controls
{
    public partial class AcrylicViewHandler
    {
        public static PropertyMapper<IAcrylicView, AcrylicViewHandler> AcrylicViewMapper = new(ViewMapper)
        {
            [nameof(IAcrylicView.CornerRadius)] = MapCornerRadius,
            [nameof(IAcrylicView.TintColor)] = MapTintColor,
            [nameof(IAcrylicView.TintOpacity)] = MapTintOpacity,
            [nameof(IAcrylicView.EffectStyle)] = MapEffectStyle

        };

        public AcrylicViewHandler() : base(AcrylicViewMapper)
        {


        }

        public AcrylicViewHandler(PropertyMapper mapper) : base(mapper)
        {

        }
    }
}
