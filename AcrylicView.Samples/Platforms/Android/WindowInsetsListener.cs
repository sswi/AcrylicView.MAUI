using AndroidX.Core.View;


namespace AcrylicView.Samples.Platforms.Android
{
    internal class WindowInsetsListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat? OnApplyWindowInsets(global::Android.Views.View? v, WindowInsetsCompat? insets)
        {
            v?.SetPadding(0, 0, 0, 0);
            return WindowInsetsCompat.Consumed;
        }
    }
}
