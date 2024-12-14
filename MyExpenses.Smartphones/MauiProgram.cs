using CommunityToolkit.Maui;
using Material.Components.Maui.Extensions;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using UraniumUI;

namespace MyExpenses.Smartphones;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMaterialComponents()
            .UseUraniumUIMaterial();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}