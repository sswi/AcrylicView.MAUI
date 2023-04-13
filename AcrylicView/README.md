using Xe.AcrylicView;

public static class MauiProgram { public static MauiApp CreateMauiApp() { var builder = MauiApp.CreateBuilder(); builder .UseMauiApp<App>()

        .UseAcrylicView()  //
        
        
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
    return builder.Build();
}
} }



xmlns:ui="clr-namespace:Xe.AcrylicView;assembly=Xe.AcrylicView" 


            <ui:AcrylicView VerticalOptions="Center"   HeightRequest="100"  EffectStyle="Custom"    Margin="10"  TintColor="OrangeRed" TintOpacity=".15 "  BorderColor="OrangeRed" BorderThickness="2" CornerRadius="50,10,30,20" >
                <Grid>
                    <Label Text="Hello Word" FontSize="25" HorizontalOptions="Center" VerticalOptions="Center" TextColor="OrangeRed"/>
                    <Label/>
                </Grid>
            </ui:AcrylicView>

iOS、Mac平台不支持颜色混合

Android平台的实现方式是从这个仓库获取，从Xamarin.Forms简单移植过来的，请支持原作者： https://github.com/roubachof/Sharpnado.MaterialFrame