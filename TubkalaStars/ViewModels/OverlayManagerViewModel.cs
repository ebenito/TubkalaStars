using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TubkalaStars.Core.Models;
using TubkalaStars.Core.Overlays;

namespace TubkalaStars.ViewModels;

public partial class OverlayManagerViewModel : ObservableObject
{
    private readonly OverlayManager _manager;

    public IReadOnlyList<StarMapOverlay> Overlays => _manager.Overlays;

    [ObservableProperty] private StarMapOverlay? _selected;

    public OverlayManagerViewModel(OverlayManager manager)
    {
        _manager = manager;
        _manager.OverlaysChanged += () => OnPropertyChanged(nameof(Overlays));
    }

    [RelayCommand]
    public async Task AddOverlayAsync()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Selecciona imagen del overlay",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, ["image/png", "image/jpeg", "image/svg+xml"] },
                { DevicePlatform.WinUI,   [".png", ".jpg", ".svg"] }
            })
        });

        if (result is null) return;

        var overlay = new StarMapOverlay
        {
            Name = Path.GetFileNameWithoutExtension(result.FileName),
            FilePath = result.FullPath,
            // Anclado al centro del cielo por defecto, el usuario puede ajustarlo
            AnchorRA = 12.0,
            AnchorDec = 0.0,
            WidthDegrees = 10.0,
            HeightDegrees = 10.0
        };

        _manager.Add(overlay);
    }

    [RelayCommand]
    public void RemoveOverlay(Guid id) => _manager.Remove(id);

    [RelayCommand]
    public void ToggleVisibility(Guid id) => _manager.ToggleVisibility(id);
}
