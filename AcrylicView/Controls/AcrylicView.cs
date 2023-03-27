using Xe.AcrylicView.Controls;

namespace Xe.AcrylicView
{
    public partial class AcrylicView : View, IAcrylicView
    {
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
            nameof(TintColor),
            typeof(Color),
            typeof(AcrylicView), DeviceInfo.Current.Platform == DevicePlatform.Android ? Colors.LightGray : Colors.Transparent);

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(float),
        typeof(AcrylicView), 0.0f);

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly BindableProperty TintOpacityProperty = BindableProperty.Create(
            nameof(TintOpacity),
            typeof(double),
            typeof(AcrylicView), 0.0);

        public double TintOpacity
        {
            get => (double)GetValue(TintOpacityProperty);
            set => SetValue(TintOpacityProperty, value);
        }



        public static readonly BindableProperty EffectStyleProperty = BindableProperty.Create(
            nameof(EffectStyle),
            typeof(EffectStyle),
            typeof(AcrylicView), EffectStyle.Light);

        public EffectStyle EffectStyle
        {
            get => (EffectStyle)GetValue(EffectStyleProperty);
            set => SetValue(EffectStyleProperty, value);
        }


    }
}
