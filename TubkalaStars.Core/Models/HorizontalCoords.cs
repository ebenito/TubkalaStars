namespace TubkalaStars.Core.Models;

/// <summary>Coordenadas horizontales locales del observador.</summary>
public record HorizontalCoords(
    double AltitudeDegrees,  // 0 = horizonte, 90 = cénit
    double AzimuthDegrees    // 0 = Norte, 90 = Este, 180 = Sur, 270 = Oeste
)
{
    public bool IsAboveHorizon => AltitudeDegrees > 0;
    public double AltitudeRadians => AltitudeDegrees * Math.PI / 180.0;
    public double AzimuthRadians => AzimuthDegrees * Math.PI / 180.0;

    /// <summary>Convierte a coordenadas XY en pantalla (proyección estereográfica).</summary>
    public (float X, float Y) ToScreenXY(float centerX, float centerY, float radius)
    {
        if (!IsAboveHorizon) return (-9999, -9999); // Fuera de pantalla

        double altRad = AltitudeRadians;
        double azRad = AzimuthRadians;

        // Proyección azimutal equidistante
        double r = radius * (1.0 - altRad / (Math.PI / 2.0));
        float x = centerX + (float)(r * Math.Sin(azRad));
        float y = centerY - (float)(r * Math.Cos(azRad));

        return (x, y);
    }
}
