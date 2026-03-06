using System.Reflection;
using TubkalaStars.Core.Models;

namespace TubkalaStars.Core.Catalog;

/// <summary>
/// Carga el catálogo HYG de estrellas (hygdata_v41.csv).
/// El CSV debe incluirse como EmbeddedResource en el proyecto MAUI.
/// Descárgalo de: https://github.com/astronexus/HYG-Database
/// </summary>
public class StarCatalog
{
    private List<Star> _stars = [];

    public IReadOnlyList<Star> Stars => _stars.AsReadOnly();
    public bool IsLoaded { get; private set; }

    /// <summary>Carga las estrellas hasta la magnitud límite especificada.</summary>
    public async Task LoadAsync(Stream csvStream, double magnitudeLimit = 6.5)
    {
        _stars = [];
        using var reader = new StreamReader(csvStream);

        // Saltar cabecera
        await reader.ReadLineAsync();

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var star = ParseLine(line, magnitudeLimit);
            if (star != null) _stars.Add(star);
        }

        // Ordenar por magnitud (más brillantes primero para renderizado)
        _stars.Sort((a, b) => a.Magnitude.CompareTo(b.Magnitude));
        IsLoaded = true;
    }

    /// <summary>Carga el catálogo de demostración integrado (sin fichero externo).</summary>
    public void LoadDemo()
    {
        _stars = GetDemoStars();
        IsLoaded = true;
    }

    /// <summary>Devuelve estrellas visibles a simple vista (mag < 6.5) sobre el horizonte.</summary>
    public IEnumerable<Star> GetVisible(double magnitudeLimit = 6.5) =>
        _stars.Where(s => s.Magnitude <= magnitudeLimit);

    private static Star? ParseLine(string line, double magLimit)
    {
        try
        {
            var cols = line.Split(',');
            if (cols.Length < 15) return null;

            if (!double.TryParse(cols[13], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double mag)) return null;

            if (mag > magLimit) return null;

            int.TryParse(cols[0], out int id);
            double.TryParse(cols[7], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double ra);
            double.TryParse(cols[8], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double dec);
            double.TryParse(cols[16], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double ci);
            double.TryParse(cols[9], System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double dist);

            string common = cols.Length > 6 ? cols[6].Trim('"') : "";
            string bayer  = cols.Length > 5 ? cols[5].Trim('"') : "";

            return new Star(id, common, bayer, ra, dec, mag, ci, dist);
        }
        catch { return null; }
    }

    // ── Catálogo de demostración (estrellas más brillantes) ──────────────────

    private static List<Star> GetDemoStars() =>
    [
        new(32349,  "Sirio",        "Alp CMa", 6.7525,  -16.7161, -1.46, 0.00,  2.64),
        new(71683,  "Alfa Centauri","Alp Cen",14.6597,  -60.8353, -0.27, 0.71,  1.34),
        new(37279,  "Proción",      "Alp CMi", 7.6551,   5.2250,   0.38, 0.42,  3.50),
        new(69673,  "Arturo",       "Alp Boo", 14.2610,  19.1822,  -0.05, 1.23, 11.26),
        new(24436,  "Rigel",        "Bet Ori",  5.2423,  -8.2016,  0.12, -0.03, 236.97),
        new(27989,  "Betelgeuse",   "Alp Ori",  5.9195,   7.4071,  0.42, 1.50, 197.0),
        new(49669,  "Régulo",       "Alp Leo", 10.1395,  11.9672,  1.36, -0.11, 23.77),
        new(91262,  "Vega",         "Alp Lyr", 18.6156,  38.7836,  0.03, 0.00,   7.68),
        new(97649,  "Altair",       "Alp Aql", 19.8463,   8.8683,  0.77, 0.22,   5.13),
        new(113368, "Fomalhaut",    "Alp PsA", 22.9608, -29.6223,  1.17, 0.14,   7.69),
        new(677,    "Alpheratz",    "Alp And",  0.1397,  29.0904,  2.07, -0.11, 29.78),
        new(3419,   "Mirach",       "Bet And",  1.1622,  35.6201,  2.06, 1.57,  60.0),
        new(9884,   "Mirfak",       "Alp Per",  3.4054,  49.8612,  1.79, 0.48, 182.0),
        new(21421,  "Aldebarán",    "Alp Tau",  4.5987,  16.5093,  0.87, 1.54,  20.0),
        new(36850,  "Cástor",       "Alp Gem",  7.5767,  31.8883,  1.58, 0.04,  15.6),
        new(37826,  "Pólux",        "Bet Gem",  7.7553,  28.0262,  1.16, 1.00,  10.34),
        new(45238,  "Naos",         "Zet Pup",  8.0597, -40.0032,  2.25, -0.26, 430.0),
        new(57632,  "Denébola",     "Bet Leo", 11.8177,  14.5720,  2.14, 0.09,  11.0),
        new(60718,  "Acrux",        "Alp Cru", 12.4433, -63.0991,  0.77, -0.24, 98.3),
        new(65474,  "Espiga",       "Alp Vir", 13.4199, -11.1614,  1.04, -0.23, 80.4),
        new(80763,  "Antares",      "Alp Sco", 16.4901, -26.4320,  1.06, 1.83, 185.0),
        new(102098, "Deneb",        "Alp Cyg", 20.6905,  45.2803,  1.25, 0.09, 802.0),
        new(113963, "Enif",         "Eps Peg", 21.7364,   9.8750,  2.38, 1.52,  211.0),
        new(7588,   "Achernar",     "Alp Eri",  1.6286, -57.2368,  0.46, -0.16, 44.1),
        new(30438,  "Canopo",       "Alp Car",  6.3992, -52.6958,  -0.72, 0.15, 95.9),
        new(68702,  "Hadar",        "Bet Cen", 14.0637, -60.3730,  0.61, -0.23, 161.0),
        new(25336,  "Bellatrix",    "Gam Ori",  5.4188,   6.3497,  1.64, -0.22, 77.0),
        new(26727,  "Mintaka",      "Del Ori",  5.5333,  -0.2991,  2.41, -0.22, 380.0),
        new(26311,  "Alnilam",      "Eps Ori",  5.6036,  -1.2019,  1.69, -0.18, 410.0),
        new(26727,  "Alnitak",      "Zet Ori",  5.6795,  -1.9425,  1.74, -0.21, 387.0),
    ];
}
