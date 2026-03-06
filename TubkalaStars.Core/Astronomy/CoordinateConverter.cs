using TubkalaStars.Core.Models;

namespace TubkalaStars.Core.Astronomy;

/// <summary>
/// Motor de conversión de coordenadas astronómicas.
/// Convierte RA/Dec (ecuatoriales) → Alt/Az (horizontales locales).
/// Algoritmos basados en "Astronomical Algorithms" de Jean Meeus.
/// </summary>
public static class CoordinateConverter
{
    // ── Tiempo sidéreo ──────────────────────────────────────────────────────

    /// <summary>Tiempo Sidéreo Medio de Greenwich (GMST) en horas.</summary>
    public static double GetGMST(DateTime utcTime)
    {
        // Días julianos desde J2000.0
        double jd = ToJulianDay(utcTime);
        double T = (jd - 2451545.0) / 36525.0;

        // Fórmula de Meeus (cap. 12)
        double gmst = 280.46061837
                    + 360.98564736629 * (jd - 2451545.0)
                    + 0.000387933 * T * T
                    - T * T * T / 38710000.0;

        return NormalizeAngle(gmst) / 15.0; // convertir grados a horas
    }

    /// <summary>Tiempo Sidéreo Local (LST) en horas.</summary>
    public static double GetLST(DateTime utcTime, double longitudeDegrees)
    {
        double gmst = GetGMST(utcTime);
        double lst = gmst + longitudeDegrees / 15.0;
        return NormalizeTo24h(lst);
    }

    // ── Conversión principal RA/Dec → Alt/Az ────────────────────────────────

    /// <summary>
    /// Convierte coordenadas ecuatoriales a horizontales.
    /// </summary>
    /// <param name="ra">Ascensión Recta en horas (0-24)</param>
    /// <param name="dec">Declinación en grados (-90 a +90)</param>
    /// <param name="utcTime">Hora UTC de observación</param>
    /// <param name="location">Ubicación del observador</param>
    public static HorizontalCoords EquatorialToHorizontal(
        double ra, double dec, DateTime utcTime, ObserverLocation location)
    {
        double lst = GetLST(utcTime, location.LongitudeDegrees);
        double ha = NormalizeTo24h(lst - ra);  // Ángulo horario en horas

        double haRad  = ha * 15.0 * Math.PI / 180.0;
        double decRad = dec * Math.PI / 180.0;
        double latRad = location.LatitudeRadians;

        // Altitud
        double sinAlt = Math.Sin(decRad) * Math.Sin(latRad)
                      + Math.Cos(decRad) * Math.Cos(latRad) * Math.Cos(haRad);
        double altitude = Math.Asin(sinAlt) * 180.0 / Math.PI;

        // Azimut
        double cosAz = (Math.Sin(decRad) - Math.Sin(altitude * Math.PI / 180.0) * Math.Sin(latRad))
                     / (Math.Cos(altitude * Math.PI / 180.0) * Math.Cos(latRad));
        cosAz = Math.Clamp(cosAz, -1.0, 1.0);
        double azimuth = Math.Acos(cosAz) * 180.0 / Math.PI;

        // Corrección de cuadrante
        if (Math.Sin(haRad) > 0) azimuth = 360.0 - azimuth;

        return new HorizontalCoords(altitude, azimuth);
    }

    // ── Refracción atmosférica ───────────────────────────────────────────────

    /// <summary>
    /// Corrección de refracción atmosférica en grados.
    /// Solo aplicable para altitudes > -1°.
    /// </summary>
    public static double AtmosphericRefraction(double apparentAltitudeDeg)
    {
        if (apparentAltitudeDeg < -1.0) return 0;
        double altRad = apparentAltitudeDeg * Math.PI / 180.0;
        // Fórmula de Saemundsson (simplificada)
        return 1.02 / (60.0 * Math.Tan((apparentAltitudeDeg + 10.3 / (apparentAltitudeDeg + 5.11)) * Math.PI / 180.0));
    }

    // ── Utilidades ───────────────────────────────────────────────────────────

    public static double ToJulianDay(DateTime utc)
    {
        int y = utc.Year, m = utc.Month, d = utc.Day;
        double h = utc.Hour + utc.Minute / 60.0 + utc.Second / 3600.0;

        if (m <= 2) { y--; m += 12; }
        int A = y / 100;
        int B = 2 - A + A / 4;
        return Math.Floor(365.25 * (y + 4716))
             + Math.Floor(30.6001 * (m + 1))
             + d + h / 24.0 + B - 1524.5;
    }

    private static double NormalizeAngle(double deg) =>
        deg - 360.0 * Math.Floor(deg / 360.0);

    private static double NormalizeTo24h(double hours)
    {
        hours %= 24.0;
        return hours < 0 ? hours + 24.0 : hours;
    }
}
