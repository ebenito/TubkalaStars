using TubkalaStars.Core.Models;

namespace TubkalaStars.Core.Overlays;

/// <summary>
/// Gestiona los gráficos educativos superpuestos al mapa estelar.
/// Los overlays se anclan a coordenadas RA/Dec y se mueven con el cielo.
/// </summary>
public class OverlayManager
{
    private readonly List<StarMapOverlay> _overlays = [];

    public IReadOnlyList<StarMapOverlay> Overlays => _overlays.AsReadOnly();

    public event Action? OverlaysChanged;

    // ── CRUD ─────────────────────────────────────────────────────────────────

    public void Add(StarMapOverlay overlay)
    {
        _overlays.Add(overlay);
        OverlaysChanged?.Invoke();
    }

    public void Remove(Guid id)
    {
        _overlays.RemoveAll(o => o.Id == id);
        OverlaysChanged?.Invoke();
    }

    public void Update(StarMapOverlay updated)
    {
        var idx = _overlays.FindIndex(o => o.Id == updated.Id);
        if (idx >= 0) { _overlays[idx] = updated; OverlaysChanged?.Invoke(); }
    }

    public StarMapOverlay? GetById(Guid id) => _overlays.FirstOrDefault(o => o.Id == id);

    // ── Visibilidad ───────────────────────────────────────────────────────────

    public IEnumerable<StarMapOverlay> GetVisible() => _overlays.Where(o => o.IsVisible);

    public void ToggleVisibility(Guid id)
    {
        var o = GetById(id);
        if (o != null) { o.IsVisible = !o.IsVisible; OverlaysChanged?.Invoke(); }
    }

    // ── Overlays de demostración para Tubkala ─────────────────────────────────

    public void LoadDemoOverlays()
    {
        _overlays.Clear();

        // Diagrama H-R anclado cerca de Orión (zona de formación estelar)
        Add(new StarMapOverlay
        {
            Name = "Diagrama H-R",
            FilePath = "demo_hr_diagram",   // Será dibujado por código en la Fase 4
            AnchorRA = 5.5,
            AnchorDec = 0.0,
            WidthDegrees = 8.0,
            HeightDegrees = 8.0,
            Opacity = 0.8f,
            EducationalNote = "El diagrama de Hertzsprung-Russell clasifica las estrellas " +
                              "por luminosidad y temperatura. Orión tiene ejemplos de todas las categorías."
        });

        // Escala de tamaños estelares anclada en Arturo
        Add(new StarMapOverlay
        {
            Name = "Comparativa de tamaños",
            FilePath = "demo_sizes",
            AnchorRA = 14.26,
            AnchorDec = 19.18,
            WidthDegrees = 6.0,
            HeightDegrees = 6.0,
            Opacity = 0.75f,
            EducationalNote = "Arturo es una gigante naranja 25 veces mayor que el Sol. " +
                              "Betelgeuse (cerca) sería tan grande que engulliría a Júpiter."
        });
    }
}
