using Microsoft.Maui;
using Microsoft.Maui.Controls;
using TubkalaStars.Views;

namespace TubkalaStars;

public partial class App : Application
{
    public App(MainPage mainPage)
    {
        InitializeComponent();
        MainPage = new NavigationPage(mainPage)
        {
            BarBackgroundColor = Color.FromArgb("#0D1B2A"),
            BarTextColor = Colors.White
        };
    }
}
