using System;
using Microsoft.Extensions.DependencyInjection;
using TubkalaStars.Views;

namespace TubkalaStars;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Resolver MainPage después de InitializeComponent para que los recursos
        // definidos en App.xaml (por ejemplo `SkyBlack`) estén disponibles.
        var mainPage = serviceProvider.GetRequiredService<MainPage>();
        MainPage = new NavigationPage(mainPage)
        {
            BarBackgroundColor = Color.FromArgb("#0D1B2A"),
            BarTextColor = Colors.White
        };
    }
}
