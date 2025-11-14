using AcrylicView.Samples.Platforms.Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace AcrylicView.Samples
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {      
            base.OnCreate(savedInstanceState);
            ViewCompat.SetOnApplyWindowInsetsListener(Window?.DecorView, new WindowInsetsListener());
               
        }
    }
}