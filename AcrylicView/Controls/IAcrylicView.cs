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

        /// <summary>
        /// Warning！！！|注意啦！
        /// OnlyAndroid/仅安卓系统可用
        /// Enabling this option will affect system performance/启用此项会完美显示，不会泛光，同时将会影响系统性能，因为会不停地对区域进行监听绘制
        /// </summary>
        bool AndroidPerfect { get;set; }



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