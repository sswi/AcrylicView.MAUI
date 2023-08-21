using Xe.AcrylicView.Controls;

namespace Xe.AcrylicView
{
    public partial class AcrylicView : ContentView, IAcrylicView
    {
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(AcrylicView), Colors.Transparent);

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(nameof(BorderThickness), typeof(Thickness), typeof(AcrylicView), new Thickness(1.0));

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(Thickness),
        typeof(AcrylicView), new Thickness(5.0));

        public static readonly BindableProperty EffectStyleProperty = BindableProperty.Create(
            nameof(EffectStyle),
            typeof(EffectStyle),
            typeof(AcrylicView), EffectStyle.Custom);

        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
                                            nameof(TintColor),
            typeof(Color),
            typeof(AcrylicView), DeviceInfo.Current.Platform == DevicePlatform.Android ? Colors.LightGray : Colors.Transparent);

        public static readonly BindableProperty TintOpacityProperty = BindableProperty.Create(
            nameof(TintOpacity),
            typeof(double),
            typeof(AcrylicView), 0.0);


        public static readonly BindableProperty AndroidPerfectProperty = BindableProperty.Create(
            nameof(AndroidPerfect),
            typeof(bool),
            typeof(AcrylicView), false);

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public Thickness CornerRadius
        {
            get => (Thickness)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public EffectStyle EffectStyle
        {
            get => (EffectStyle)GetValue(EffectStyleProperty);
            set => SetValue(EffectStyleProperty, value);
        }

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }
        public double TintOpacity
        {
            get => (double)GetValue(TintOpacityProperty);
            set => SetValue(TintOpacityProperty, value);
        }

        /// <summary>
        /// Warning！！！|注意啦！
        /// OnlyAndroid/仅安卓系统可用
        /// Enabling this option will affect system performance/启用此项会完美显示，不会泛光，同时将会影响系统性能，因为会不停地对区域进行监听绘制  
        /// </summary>
        public bool AndroidPerfect
        {
            get => (bool)GetValue(AndroidPerfectProperty);
            set => SetValue(AndroidPerfectProperty, value);
        }

    }
}