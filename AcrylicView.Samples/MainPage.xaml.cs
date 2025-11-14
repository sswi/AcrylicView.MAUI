namespace AcrylicView.Samples
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
#if NET10_0_OR_GREATER
            SafeAreaEdges = SafeAreaEdges.All;
#endif
#if NET9_0
            grid.IgnoreSafeArea = true;
#endif
        }



    }
}