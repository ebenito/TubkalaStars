using CommunityToolkit.Mvvm.ComponentModel;
using TubkalaStars.Core.Models;

namespace TubkalaStars.ViewModels;

[QueryProperty(nameof(ObjectKey), "objectKey")]
public partial class ObjectDetailViewModel : ObservableObject
{
    [ObservableProperty] private CelestialObject? _object;
    [ObservableProperty] private string _objectKey = string.Empty;

    partial void OnObjectKeyChanged(string value)
    {
        // Buscar objeto por "M42", "NGC7000", etc.
        Object = Core.Catalog.DeepSkyCatalog.FindByName(value);
    }

    public string TypeDisplay => Object?.Type switch
    {
        CelestialObjectType.Galaxy            => "🌌 Galaxia",
        CelestialObjectType.OpenCluster       => "✨ Cúmulo Abierto",
        CelestialObjectType.GlobularCluster   => "⚪ Cúmulo Globular",
        CelestialObjectType.Nebula            => "🌫️ Nebulosa",
        CelestialObjectType.PlanetaryNebula   => "🔵 Nebulosa Planetaria",
        CelestialObjectType.Planet            => "🪐 Planeta",
        _ => "Objeto celeste"
    };

    public string CatalogId => Object is null ? "" : $"{Object.Catalog} {Object.Number}";
    public string Coordinates => Object is null ? "" :
        $"AR: {Object.RightAscension:F2}h  Dec: {Object.Declination:+0.00;-0.00}°";
}
