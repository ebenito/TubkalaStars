namespace TubkalaStars.Core.Models;

/// <summary>
/// Gráfico personalizado anclado a coordenadas celestes.
/// Se mueve con el mapa estelar manteniendo su posición en el cielo.
/// </summary>
public class StarMapOverlay
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;   // PNG/SVG local

    // Anclaje celeste (coordenadas ecuatoriales del centro del gráfico)
    public double AnchorRA { get; set; }    // horas
    public double AnchorDec { get; set; }  // grados

    // Extensión angular en el cielo
    public double WidthDegrees { get; set; } = 5.0;
    public double HeightDegrees { get; set; } = 5.0;

    // Apariencia
    public float Opacity { get; set; } = 0.85f;
    public float RotationDegrees { get; set; } = 0f;
    public bool IsVisible { get; set; } = true;

    // Metadatos educativos
    public string EducationalNote { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
