
using Xe.AcrylicView.Controls;

namespace Xe.AcrylicView
{
    public static class Initializer
    {
        public static MauiAppBuilder UseAcrylicView(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(handlers =>
                         {
#if ANDROID || WINDOWS || IOS || MACCATALYST
                         
                             handlers.AddHandler(typeof(AcrylicView), typeof(AcrylicViewHandler));
#endif
                         });
            return builder;
        }
    }
}
