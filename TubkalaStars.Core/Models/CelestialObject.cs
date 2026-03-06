namespace TubkalaStars.Core.Models;

public enum CelestialObjectType
{
    Galaxy, OpenCluster, GlobularCluster,
    Nebula, PlanetaryNebula, Planet, Comet
}

public record CelestialObject(
    string Catalog,         // "NGC", "IC", "M" (Messier)
    int Number,
    string CommonName,
    CelestialObjectType Type,
    double RightAscension,  // horas
    double Declination,     // grados
    double Magnitude,
    string Description,
    string MythologyNote    // Nota educativa / mitología
);
