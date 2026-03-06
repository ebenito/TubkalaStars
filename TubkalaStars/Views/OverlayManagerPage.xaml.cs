using TubkalaStars.ViewModels;

namespace TubkalaStars.Views;

public partial class OverlayManagerPage : ContentPage
{
    public OverlayManagerPage(OverlayManagerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
