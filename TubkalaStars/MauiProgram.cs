using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using TubkalaStars.Core.Catalog;
using TubkalaStars.Core.Overlays;
using TubkalaStars.Services;
using TubkalaStars.ViewModels;
using TubkalaStars.Views;

namespace TubkalaStars;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Servicios Core ─────────────────────────────────────────────────
        builder.Services.AddSingleton<StarCatalog>();
        builder.Services.AddSingleton<OverlayManager>();

        // ── Servicios MAUI ─────────────────────────────────────────────────
        builder.Services.AddSingleton<ILocationService, LocationService>();

        // OrientationService implementa ambas interfaces → registrar una vez
        builder.Services.AddSingleton<OrientationService>();
        builder.Services.AddSingleton<IGyroscopeService>(sp =>
            sp.GetRequiredService<OrientationService>());
        builder.Services.AddSingleton<IOrientationService>(sp =>
            sp.GetRequiredService<OrientationService>());

        // ── ViewModels ─────────────────────────────────────────────────────
        builder.Services.AddSingleton<StarMapViewModel>();
        builder.Services.AddTransient<ObjectDetailViewModel>();
        builder.Services.AddTransient<OverlayManagerViewModel>();

        // ── Views ──────────────────────────────────────────────────────────
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<ObjectDetailPage>();
        builder.Services.AddTransient<OverlayManagerPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
