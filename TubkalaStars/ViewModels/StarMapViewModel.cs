using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TubkalaStars.Core.Astronomy;
using TubkalaStars.Core.Catalog;
using TubkalaStars.Core.Models;
using TubkalaStars.Core.Overlays;
using TubkalaStars.Services;

namespace TubkalaStars.ViewModels;

public partial class StarMapViewModel : ObservableObject
{
    private readonly StarCatalog _starCatalog;
    private readonly OverlayManager _overlayManager;
    private readonly ILocationService _locationService;
    private readonly IOrientationService _orientationService;

    // ── Estado del mapa ───────────────────────────────────────────────────────

    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private bool _isNightMode = false;
    [ObservableProperty] private bool _isFollowingOrientation = false;
    [ObservableProperty] private double _fieldOfViewDegrees = 90.0;   // FOV del mapa
    [ObservableProperty] private string _statusText = "Cargando catálogo estelar...";
    [ObservableProperty] private CelestialObject? _selectedObject;
    [ObservableProperty] private ObserverLocation _location = ObserverLocation.ColladoVillalba;

    // Orientación del centro del mapa
    [ObservableProperty] private double _centerAzimuth = 180.0;  // Mirando al Sur
    [ObservableProperty] private double _centerAltitude = 45.0;

    // Hora de observación (puede ser manual para simulaciones)
    [ObservableProperty] private DateTime _observationTime = DateTime.UtcNow;
    [ObservableProperty] private bool _useCurrentTime = true;

    public StarCatalog StarCatalog => _starCatalog;
    public OverlayManager OverlayManager => _overlayManager;

    // Evento para forzar redibujado del canvas SkiaSharp
    public event Action? MapInvalidated;

    public StarMapViewModel(
        StarCatalog starCatalog,
        OverlayManager overlayManager,
        ILocationService locationService,
        IOrientationService orientationService)
    {
        _starCatalog = starCatalog;
        _overlayManager = overlayManager;
        _locationService = locationService;
        _orientationService = orientationService;

        _overlayManager.OverlaysChanged += () => MapInvalidated?.Invoke();
        _orientationService.Changed += OnOrientationChanged;
    }

    // ── Inicialización ────────────────────────────────────────────────────────

    [RelayCommand]
    public async Task InitializeAsync()
    {
        IsLoading = true;
        StatusText = "Cargando catálogo estelar...";

        // Cargar catálogo demo (en Fase 2 se cargará el HYG completo)
        _starCatalog.LoadDemo();

        StatusText = "Obteniendo ubicación...";
        Location = await _locationService.GetCurrentLocationAsync();

        // Cargar overlays de demo
        _overlayManager.LoadDemoOverlays();

        IsLoading = false;
        StatusText = $"{_starCatalog.Stars.Count} estrellas · {Location.Name}";
        MapInvalidated?.Invoke();
    }

    // ── Tiempo ────────────────────────────────────────────────────────────────

    [RelayCommand]
    public void TickTime()
    {
        if (UseCurrentTime)
        {
            ObservationTime = DateTime.UtcNow;
            MapInvalidated?.Invoke();
        }
    }

    // ── Orientación ───────────────────────────────────────────────────────────

    [RelayCommand]
    public void ToggleOrientationTracking()
    {
        IsFollowingOrientation = !IsFollowingOrientation;
        if (IsFollowingOrientation)
            _orientationService.StartTracking();
        else
            _orientationService.StopTracking();
    }

    private void OnOrientationChanged()
    {
        if (!IsFollowingOrientation) return;
        CenterAzimuth = _orientationService.AzimuthDegrees;
        CenterAltitude = Math.Max(0, _orientationService.AltitudeDegrees);
        MapInvalidated?.Invoke();
    }

    // ── Modo nocturno ─────────────────────────────────────────────────────────

    [RelayCommand]
    public void ToggleNightMode() => IsNightMode = !IsNightMode;

    // ── Zoom ─────────────────────────────────────────────────────────────────

    public void ZoomIn()
    {
        FieldOfViewDegrees = Math.Max(10.0, FieldOfViewDegrees - 10.0);
        MapInvalidated?.Invoke();
    }

    public void ZoomOut()
    {
        FieldOfViewDegrees = Math.Min(180.0, FieldOfViewDegrees + 10.0);
        MapInvalidated?.Invoke();
    }

    // ── Selección de objetos ──────────────────────────────────────────────────

    /// <summary>
    /// Busca el objeto más cercano a las coordenadas de toque en pantalla.
    /// </summary>
    public CelestialObject? HitTestDso(float touchX, float touchY, float canvasW, float canvasH)
    {
        float cx = canvasW / 2f, cy = canvasH / 2f;
        float radius = Math.Min(cx, cy) * 0.95f;
        const float hitRadius = 24f;

        foreach (var obj in DeepSkyCatalog.All)
        {
            var altAz = CoordinateConverter.EquatorialToHorizontal(
                obj.RightAscension, obj.Declination, ObservationTime, Location);
            var (sx, sy) = altAz.ToScreenXY(cx, cy, radius);

            float dx = touchX - sx, dy = touchY - sy;
            if (Math.Sqrt(dx * dx + dy * dy) < hitRadius)
                return obj;
        }
        return null;
    }

    public void SelectObject(CelestialObject? obj) => SelectedObject = obj;
}
