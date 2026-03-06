using TubkalaStars.Core.Astronomy;
using TubkalaStars.Core.Catalog;
using TubkalaStars.Core.Models;
using Xunit;

namespace TubkalaStars.Tests;

/// <summary>
/// Tests de los algoritmos astronómicos.
/// Usamos casos verificables con valores publicados.
/// </summary>
public class CoordinateConverterTests
{
    private static readonly ObserverLocation Madrid = ObserverLocation.ColladoVillalba;

    [Fact]
    public void ToJulianDay_J2000_ReturnsCorrectValue()
    {
        // J2000.0 = 1 enero 2000, 12:00 TT ≈ JD 2451545.0
        var j2000 = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        double jd = CoordinateConverter.ToJulianDay(j2000);
        Assert.Equal(2451545.0, jd, precision: 1);
    }

    [Fact]
    public void GMST_KnownDate_IsWithinTolerance()
    {
        // 10 abril 1987, 0h UT → GMST ≈ 13h 10m 46.3672s (Meeus ejemplo 12.a)
        var date = new DateTime(1987, 4, 10, 0, 0, 0, DateTimeKind.Utc);
        double gmst = CoordinateConverter.GetGMST(date);
        // GMST en horas: 13h 10m 46.37s ≈ 13.179548h
        Assert.InRange(gmst, 13.17, 13.19);
    }

    [Fact]
    public void EquatorialToHorizontal_SiriusAboveHorizon_InSummerNight()
    {
        // Sirio (RA=6.752h, Dec=-16.72°) es visible desde Madrid en enero/febrero
        var date = new DateTime(2025, 1, 15, 21, 0, 0, DateTimeKind.Utc);
        var coords = CoordinateConverter.EquatorialToHorizontal(6.7525, -16.7161, date, Madrid);

        Assert.True(coords.IsAboveHorizon,
            $"Sirio debería estar sobre el horizonte. Alt={coords.AltitudeDegrees:F1}°");
    }

    [Fact]
    public void EquatorialToHorizontal_AzimuthRange_IsValid()
    {
        var date = DateTime.UtcNow;
        var coords = CoordinateConverter.EquatorialToHorizontal(6.7525, -16.7161, date, Madrid);
        Assert.InRange(coords.AzimuthDegrees, 0.0, 360.0);
    }

    [Fact]
    public void HorizontalCoords_ToScreenXY_AboveHorizon_IsOnCanvas()
    {
        var coords = new HorizontalCoords(45.0, 180.0);
        var (x, y) = coords.ToScreenXY(400, 400, 380);
        Assert.InRange(x, 0, 800);
        Assert.InRange(y, 0, 800);
    }

    [Fact]
    public void HorizontalCoords_ToScreenXY_BelowHorizon_IsOffscreen()
    {
        var coords = new HorizontalCoords(-10.0, 180.0);
        var (x, y) = coords.ToScreenXY(400, 400, 380);
        // Debe devolver valor fuera de pantalla
        Assert.True(x < -100 || y < -100, "Objeto bajo el horizonte debe estar fuera de pantalla");
    }

    [Fact]
    public void AtmosphericRefraction_AtHorizon_IsSignificant()
    {
        double refraction = CoordinateConverter.AtmosphericRefraction(0.0);
        // Refracción en el horizonte ≈ 34 arcominutos ≈ 0.566°
        Assert.InRange(refraction, 0.4, 0.7);
    }
}

public class StarColorTests
{
    [Theory]
    [InlineData(-0.5, 155, 176, 255)]  // Azul-blanca
    [InlineData(0.6,  255, 210, 161)]  // Naranja
    [InlineData(1.5,  255, 150, 100)]  // Roja
    public void GetColor_ByColorIndex_ReturnsCorrectCategory(double ci, byte r, byte g, byte b)
    {
        var star = new Star(1, "Test", "", 0, 0, 1.0, ci, 10.0);
        var (R, G, B) = star.GetColor();
        Assert.Equal(r, R);
        Assert.Equal(g, G);
        Assert.Equal(b, B);
    }

    [Fact]
    public void GetRenderRadius_BrightStar_LargerThanFaint()
    {
        var bright = new Star(1, "Bright", "", 0, 0, -1.5, 0, 10);
        var faint  = new Star(2, "Faint",  "", 0, 0, 6.0,  0, 10);
        Assert.True(bright.GetRenderRadius() > faint.GetRenderRadius());
    }
}

public class StarCatalogTests
{
    [Fact]
    public void LoadDemo_LoadsStars()
    {
        var catalog = new StarCatalog();
        catalog.LoadDemo();
        Assert.True(catalog.IsLoaded);
        Assert.NotEmpty(catalog.Stars);
    }

    [Fact]
    public void GetVisible_WithMagnitudeLimit_FiltersCorrectly()
    {
        var catalog = new StarCatalog();
        catalog.LoadDemo();
        var bright = catalog.GetVisible(2.0).ToList();
        Assert.All(bright, s => Assert.True(s.Magnitude <= 2.0));
    }
}

public class DeepSkyCatalogTests
{
    [Fact]
    public void All_ContainsExpectedObjects()
    {
        Assert.NotEmpty(DeepSkyCatalog.All);
        Assert.Contains(DeepSkyCatalog.All, o => o.CommonName.Contains("Orión"));
        Assert.Contains(DeepSkyCatalog.All, o => o.CommonName.Contains("Andrómeda"));
    }

    [Fact]
    public void FindByName_M42_ReturnsOrionNebula()
    {
        var obj = DeepSkyCatalog.FindByName("M42");
        Assert.NotNull(obj);
        Assert.Contains("Orión", obj.CommonName);
    }

    [Fact]
    public void GetByType_Galaxies_OnlyReturnsGalaxies()
    {
        var galaxies = DeepSkyCatalog.GetByType(CelestialObjectType.Galaxy).ToList();
        Assert.All(galaxies, g => Assert.Equal(CelestialObjectType.Galaxy, g.Type));
    }
}
