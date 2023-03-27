





using Xe.AcrylicView;

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                
                .UseAcrylicView()  //*********  Use
                
                
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            return builder.Build();
        }
    }
}




iOS、Mac平台不支持颜色混合

Android平台的实现方式是从这个仓库获取的，请支持原作者：
https://github.com/roubachof/Sharpnado.MaterialFrame



# AcrylicView![QQ截图20230327183718](https://user-images.githubusercontent.com/39110708/227924264-41c35cc1-97f0-4bf6-9130-2cd9ffd8f78f.png)
![Screenshot_20230327-184345](https://user-images.githubusercontent.com/39110708/227924499-aa2bac68-9b4d-4ae7-9a50-5672e9ef695c.png)
![a40734195e4e95b7f069b4946f59fdc](https://user-images.githubusercontent.com/39110708/227924704-bfb7511b-c835-4e67-b19e-5e40a6cd8717.png)
