namespace TubkalaStars.Core.Models;

public record ObserverLocation(
    double LatitudeDegrees,
    double LongitudeDegrees,
    double AltitudeMeters = 0,
    string Name = ""
)
{
    // Collado Villalba como ubicación por defecto
    public static readonly ObserverLocation ColladoVillalba = new(
        LatitudeDegrees: 40.634,
        LongitudeDegrees: -4.004,
        AltitudeMeters: 920,
        Name: "Collado Villalba"
    );

    public double LatitudeRadians => LatitudeDegrees * Math.PI / 180.0;
    public double LongitudeRadians => LongitudeDegrees * Math.PI / 180.0;
}
