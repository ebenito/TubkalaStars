using TubkalaStars.ViewModels;

namespace TubkalaStars.Views;

public partial class ObjectDetailPage : ContentPage
{
    public ObjectDetailPage(ObjectDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // Permite también asignar el BindingContext directamente desde MainPage
    public new object? BindingContext
    {
        get => base.BindingContext;
        set => base.BindingContext = value;
    }
}
