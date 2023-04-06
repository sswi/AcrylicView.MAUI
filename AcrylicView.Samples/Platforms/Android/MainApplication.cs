using Android.App;
using Android.Graphics;
using Android.Runtime;

namespace AcrylicView.Samples
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {

       

        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}