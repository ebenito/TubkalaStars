# TubkalaStars 🌟

App de astronomía para sesiones públicas de observación de Tubkala.
Multiplataforma: **Android** y **Windows** desde un único proyecto .NET MAUI.

---

## Requisitos

- Visual Studio 2022/2026 con workload **MAUI** instalado
- .NET 10 SDK
- Android SDK (API 24+) para despliegue en móvil

---

## Cómo abrir el proyecto

1. Abre `TubkalaStars.sln` en Visual Studio
2. Restaura paquetes NuGet (automático al abrir)
3. Selecciona el perfil de ejecución: `Android Emulator` o `Windows Machine`
4. Pulsa **F5** para ejecutar

---

## Estructura del proyecto

```
TubkalaStars.sln
├── TubkalaStars.Core/          ← Lógica pura, sin dependencias MAUI
│   ├── Astronomy/
│   │   └── CoordinateConverter.cs   ← RA/Dec → Alt/Az (algoritmos Meeus)
│   ├── Catalog/
│   │   ├── StarCatalog.cs           ← Catálogo HYG + demo de 30 estrellas
│   │   └── DeepSkyCatalog.cs        ← 12 DSO con mitología y notas educativas
│   ├── Models/                      ← Star, CelestialObject, Overlay, etc.
│   └── Overlays/
│       └── OverlayManager.cs        ← Gestión de gráficos superpuestos
│
├── TubkalaStars/               ← App MAUI (Android + Windows)
│   ├── Views/
│   │   ├── MainPage            ← Mapa estelar SkiaSharp en tiempo real
│   │   ├── ObjectDetailPage    ← Ficha educativa de objetos DSO
│   │   └── OverlayManagerPage  ← Gestión de overlays propios
│   ├── ViewModels/             ← MVVM con CommunityToolkit
│   └── Services/               ← GPS, giroscopio, orientación
│
└── TubkalaStars.Tests/         ← Tests xUnit de algoritmos astronómicos
```

---

## Fase actual: Fase 1 (Fundamentos)

✅ Mapa estelar con 30 estrellas brillantes (catálogo demo)  
✅ Conversión de coordenadas ecuatoriales → horizontales  
✅ Líneas de constelación (Orión)  
✅ 12 objetos DSO con información educativa y mitología  
✅ Sistema de overlays (gráficos anclados al cielo)  
✅ Modo nocturno (pantalla roja)  
✅ Zoom +/−  
✅ Detección de toque sobre objetos DSO  
✅ GPS con fallback a Collado Villalba  

---

## Próximos pasos: Fase 2

- [ ] Cargar catálogo HYG completo (120.000 estrellas)
  - Descarga: https://github.com/astronexus/HYG-Database/raw/main/hyg/v41/hygdata_v41.csv
  - Añadirlo como `MauiAsset` en el proyecto
- [ ] Integrar giroscopio para modo "apunta al cielo"
- [ ] Líneas de todas las constelaciones IAU
- [ ] Corrección de declinación magnética

---

## Cargar el catálogo HYG completo

```csharp
// En StarMapViewModel.InitializeAsync():
var stream = await FileSystem.OpenAppPackageFileAsync("hygdata_v41.csv");
await _starCatalog.LoadAsync(stream, magnitudeLimit: 6.5);
```

Descarga el CSV y añádelo al proyecto MAUI como:
```xml
<MauiAsset Include="Resources/Raw/hygdata_v41.csv" />
```

---

## Tests

```bash
cd TubkalaStars.Tests
dotnet test
```

---

## Créditos

- Algoritmos: *Astronomical Algorithms* - Jean Meeus
- Catálogo estelar: HYG Database (David Nash) - CC0
- Motor de renderizado: SkiaSharp
- Framework: .NET MAUI + CommunityToolkit.Mvvm
