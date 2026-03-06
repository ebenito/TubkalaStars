namespace TubkalaStars.Core.Models;

/// <summary>
/// Representa una estrella del catálogo HYG.
/// RA en horas (0-24), Dec en grados (-90/+90).
/// </summary>
public record Star(
    int Id,
    string CommonName,
    string BayerName,       // p.ej. "Alp CMa" = Sirio
    double RightAscension,  // horas
    double Declination,     // grados
    double Magnitude,       // magnitud visual aparente
    double ColorIndex,      // índice B-V para color
    double Distance         // parsecs
)
{
    /// <summary>Color RGB aproximado según índice B-V.</summary>
    public (byte R, byte G, byte B) GetColor()
    {
        return ColorIndex switch
        {
            < -0.3 => (155, 176, 255),   // Azul-blanca (O/B)
            < 0.0  => (170, 191, 255),   // Blanca-azul (A)
            < 0.3  => (255, 255, 255),   // Blanca (F)
            < 0.6  => (255, 255, 210),   // Blanca-amarilla (G)
            < 1.0  => (255, 210, 161),   // Naranja (K)
            _      => (255, 150, 100),   // Roja (M)
        };
    }

    /// <summary>Radio de renderizado en pantalla según magnitud.</summary>
    public float GetRenderRadius()
    {
        // Magnitud más baja = estrella más brillante = radio mayor
        double clamped = Math.Clamp(Magnitude, -2, 8);
        return (float)(4.5 - clamped * 0.45);
    }
}
