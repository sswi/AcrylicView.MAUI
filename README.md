
|支持平台/Supported platforms        |
|----------------------------|
| :heavy_check_mark: Android | 
| :heavy_check_mark: iOS     |
| :heavy_check_mark: macOS   |
| :heavy_check_mark: Windows |


温馨提示：在 windows的安卓子系统 中无法正常显示  
Tips：Does not work in the Windows subsystem for Android   


NugetPackage：AcrylicView.Maui

![231667033-e99ed65b-d74a-4e70-9e89-0958afdc5e45](https://github.com/sswi/AcrylicView.MAUI/assets/39110708/1e05f06c-7d43-403a-8f83-74d9436c44f3)
MauiProgram.cs

```csharp

using Xe.AcrylicView; //***1.引入 |import

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                
                .UseAcrylicView()  //*********2.添加此扩展方法使用 | Usage
                
                
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            return builder.Build();
        }
    }
}
```



 
```xml
 xmlns:ui="clr-namespace:Xe.AcrylicView;assembly=Xe.AcrylicView" 

<ui:AcrylicView VerticalOptions="Center"  
                HeightRequest="100"  
                EffectStyle="Custom"    
                Margin="10"  
                TintColor="OrangeRed" 
                TintOpacity=".15 "  
                BorderColor="OrangeRed" 
                BorderThickness="1,2" 
                CornerRadius="50,10,30,20" >           
   <Grid>
    <Label Text="Hello Word" FontSize="25" HorizontalOptions="Center" VerticalOptions="Center" TextColor="OrangeRed"/>    
   </Grid>
</ui:AcrylicView>
```

| 属性名 / Property Name  | 类型 / Type | 描述 / Detail | 默认值 / Default |
| ------------- | ------------- | ------------- | ------------- |
| 效果 / EffectStyle  | 枚举 / Enumeration  | ExtraLight,Light,Dark,ExtraDark,Custom  | 亮效果 / Light |
| 色调 / TintColor | 颜色 / Color  | 混合颜色 / Colors | 透明色 / Transparent |
| 色调深度 / TintOpacity | 双精度小数 / double  | 色调不透明度 / Opacity | 透明 / 0.0 |
| 边框颜色 / BorderColor | 颜色 / Color  | Colors | 透明色 / Transparent |
| 边框厚度 / BorderThickness | 厚度 / Thickness  | Thickness | 无边框 / Thickness(0) |
| 圆角半径 / CornerRadius | 厚度 / Thickness  | Thickness | 直角 / Thickness(0) |

Android平台的实现方式是从这个仓库获取，从Xamarin.Forms简单移植过来的，请支持原作者：
https://github.com/roubachof/Sharpnado.MaterialFrame

