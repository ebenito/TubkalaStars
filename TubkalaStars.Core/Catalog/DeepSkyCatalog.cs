using TubkalaStars.Core.Models;

namespace TubkalaStars.Core.Catalog;

/// <summary>
/// Catálogo de objetos de cielo profundo (DSO) con notas educativas y mitología.
/// Selección de los más espectaculares y pedagógicos para sesiones de Tubkala.
/// </summary>
public class DeepSkyCatalog
{
    private static readonly List<CelestialObject> _objects =
    [
        // ── MESSIER ───────────────────────────────────────────────────────────
        new("M", 1, "Nebulosa del Cangrejo", CelestialObjectType.Nebula,
            5.5753, 22.0145, 8.4,
            "Remanente de supernova observada por astrónomos chinos en 1054. " +
            "En su centro hay un púlsar que gira 30 veces por segundo.",
            "Los astrónomos chinos de la dinastía Song registraron la explosión el 4 de julio de 1054. " +
            "Era tan brillante que se veía de día durante 23 días."),

        new("M", 13, "Cúmulo de Hércules", CelestialObjectType.GlobularCluster,
            16.6949, 36.4613, 5.8,
            "Uno de los cúmulos globulares más brillantes del cielo boreal. " +
            "Contiene unos 300.000 estrellas a 25.100 años-luz.",
            "En 1974 el radiotelescopio de Arecibo envió el famoso 'Mensaje de Arecibo' " +
            "dirigido hacia M13, viajará 25.000 años hasta llegar."),

        new("M", 31, "Galaxia de Andrómeda", CelestialObjectType.Galaxy,
            0.7122, 41.2692, 3.4,
            "La galaxia más cercana a la Vía Láctea visible a simple vista. " +
            "Contiene un billón de estrellas y está a 2,5 millones de años-luz.",
            "En la mitología árabe, Al-Sufi la describió en el año 964 como una 'pequeña nube'. " +
            "Se llama Andrómeda por la princesa etíope encadenada de la mitología griega."),

        new("M", 42, "Nebulosa de Orión", CelestialObjectType.Nebula,
            5.5855, -5.3900, 4.0,
            "Región de formación estelar a 1.344 años-luz. " +
            "Con un telescopio pequeño se ven los 4 Trapecios (estrellas jóvenes).",
            "Orión era un cazador gigante en la mitología griega, hijo de Poseidón. " +
            "Artemisa le mató accidentalmente y Zeus lo colocó entre las estrellas."),

        new("M", 45, "Pléyades (Las Siete Cabrillas)", CelestialObjectType.OpenCluster,
            3.7872, 24.1167, 1.6,
            "Cúmulo abierto de estrellas jóvenes azules a 444 años-luz. " +
            "A simple vista se ven 6-7 estrellas pero contiene unas 3.000.",
            "Prácticamente TODAS las culturas del mundo tienen mitos sobre las Pléyades. " +
            "En Grecia eran las 7 hijas de Atlas y Pleyone. Los mayas iniciaban su año con su orto helíaco."),

        new("M", 57, "Nebulosa del Anillo", CelestialObjectType.PlanetaryNebula,
            18.8928, 33.0292, 8.8,
            "Nebulosa planetaria clásica en Lyra. El anillo de gas rodea a una enana blanca central. " +
            "Fue la envoltura exterior de una estrella como el Sol.",
            "Las nebulosas planetarias muestran el destino final del Sol dentro de ~5.000 millones de años. " +
            "El nombre 'planetaria' se debe a Herschel (1785) por su aspecto circular similar a Urano."),

        new("M", 8, "Nebulosa de la Laguna", CelestialObjectType.Nebula,
            18.0619, -24.3833, 5.8,
            "Gran nube de gas y polvo en Sagitario, a 4.100 años-luz. " +
            "Visible a simple vista desde lugares oscuros.",
            "En la dirección de Sagitario miramos hacia el centro de la Vía Láctea, " +
            "a unos 26.000 años-luz. Hay un agujero negro supermasivo de 4 millones de masas solares."),

        new("M", 51, "Galaxia del Remolino", CelestialObjectType.Galaxy,
            13.4997, 47.1952, 8.4,
            "Primera galaxia en la que se observaron brazos espirales (Lord Rosse, 1845). " +
            "Interactúa gravitacionalmente con NGC 5195.",
            "Lord Rosse la observó con su telescopio 'Leviathan' de 1,8m en 1845. " +
            "Fue la primera evidencia de que existían objetos espirales fuera de la Vía Láctea."),

        new("M", 44, "Cúmulo del Pesebre (Praesepe)", CelestialObjectType.OpenCluster,
            8.6667, 19.9833, 3.7,
            "Cúmulo abierto de unas 1.000 estrellas en Cáncer, a 577 años-luz. " +
            "Visible a simple vista como una mancha difusa.",
            "Para los griegos y romanos era el 'pesebre' donde comían dos asnos (Asellus Borealis y Australis). " +
            "Si el Pesebre desaparecía de un cielo claro, anunciaba tormenta próxima."),

        new("M", 27, "Nebulosa de la Mancuerna", CelestialObjectType.PlanetaryNebula,
            19.9933, 22.7214, 7.5,
            "La nebulosa planetaria más grande del cielo en extensión aparente. " +
            "A 1.360 años-luz en Vulpecula.",
            "Charles Messier la descubrió en 1764. Su forma recuerda a una pesa de gimnasio, " +
            "de ahí su nombre inglés 'Dumbbell Nebula'. Tiene unos 3 años-luz de diámetro real."),

        // ── NGC / IC ─────────────────────────────────────────────────────────
        new("NGC", 7000, "Nebulosa de Norteamérica", CelestialObjectType.Nebula,
            20.9667, 44.3333, 4.0,
            "Nebulosa de emisión en Cygnus cuya silueta recuerda al continente americano. " +
            "A 1.600 años-luz.",
            "La 'Bahía de México' se debe a una nube de polvo oscura que bloquea la luz. " +
            "Está iluminada por Deneb, la supergigante azul más luminosa del vecindario solar."),

        new("NGC", 869, "h Persei (Doble Cúmulo)", CelestialObjectType.OpenCluster,
            2.3322, 57.1333, 4.3,
            "Dos cúmulos abiertos jóvenes a unos 7.500 años-luz, en Perseo. " +
            "Espectaculares en telescopio o prismáticos.",
            "Hiparco los catalogó ya en el siglo II a.C. Los dos cúmulos están separados " +
            "unos 300 años-luz y se formaron hace apenas 13 millones de años."),
    ];

    public static IReadOnlyList<CelestialObject> All => _objects.AsReadOnly();

    public static IEnumerable<CelestialObject> GetByType(CelestialObjectType type) =>
        _objects.Where(o => o.Type == type);

    public static CelestialObject? FindByName(string name) =>
        _objects.FirstOrDefault(o =>
            o.CommonName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
            $"{o.Catalog}{o.Number}".Equals(name, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<CelestialObject> GetVisibleTonight(
        ObserverLocation location, DateTime utcNow, double minAltitude = 10.0)
    {
        return _objects.Where(o =>
        {
            var coords = TubkalaStars.Core.Astronomy.CoordinateConverter
                .EquatorialToHorizontal(o.RightAscension, o.Declination, utcNow, location);
            return coords.AltitudeDegrees >= minAltitude;
        });
    }
}
