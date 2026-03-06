using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using TubkalaStars.Core.Astronomy;
using TubkalaStars.Core.Catalog;
using TubkalaStars.Core.Models;
using TubkalaStars.ViewModels;

namespace TubkalaStars.Views;

public partial class MainPage : ContentPage
{
    private readonly StarMapViewModel _vm;
    private System.Timers.Timer? _clockTimer;

    // Cache de imágenes de overlays
    private readonly Dictionary<string, SKBitmap> _overlayBitmaps = new();

    public MainPage(StarMapViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        vm.MapInvalidated += () => MainThread.BeginInvokeOnMainThread(
            () => StarCanvas.InvalidateSurface());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();

        // Timer para actualizar el mapa cada segundo
        _clockTimer = new System.Timers.Timer(1000);
        _clockTimer.Elapsed += (_, _) => _vm.TickTime();
        _clockTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _clockTimer?.Stop();
        _clockTimer?.Dispose();
    }

    // ── Renderizado SkiaSharp ─────────────────────────────────────────────────

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Clear(_vm.IsNightMode ? new SKColor(20, 0, 0) : new SKColor(5, 10, 20));

        if (!_vm.StarCatalog.IsLoaded) return;

        float cx = info.Width / 2f;
        float cy = info.Height / 2f;
        float radius = Math.Min(cx, cy) * 0.95f;

        DrawConstellationLines(canvas, cx, cy, radius);
        DrawStars(canvas, cx, cy, radius);
        DrawDsoObjects(canvas, cx, cy, radius);
        DrawOverlays(canvas, cx, cy, radius);
        DrawCompass(canvas, cx, cy, radius);
        DrawHorizonLabel(canvas, cx, cy, radius);
    }

    // ── Estrellas ─────────────────────────────────────────────────────────────

    private void DrawStars(SKCanvas canvas, float cx, float cy, float radius)
    {
        var utc = _vm.ObservationTime;
        var loc = _vm.Location;

        using var paint = new SKPaint { IsAntialias = true };
        using var glowPaint = new SKPaint { IsAntialias = true, MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3f) };

        foreach (var star in _vm.StarCatalog.GetVisible(6.0))
        {
            var altAz = CoordinateConverter.EquatorialToHorizontal(
                star.RightAscension, star.Declination, utc, loc);

            if (!altAz.IsAboveHorizon) continue;

            var (sx, sy) = altAz.ToScreenXY(cx, cy, radius);
            if (sx < -100 || sy < -100) continue;

            float r = star.GetRenderRadius();
            var (R, G, B) = star.GetColor();

            if (_vm.IsNightMode)
            {
                paint.Color = new SKColor((byte)(R / 3), 0, 0, 220);
            }
            else
            {
                paint.Color = new SKColor(R, G, B, 230);
                // Halo para estrellas brillantes
                if (star.Magnitude < 2.0)
                {
                    glowPaint.Color = new SKColor(R, G, B, 60);
                    canvas.DrawCircle(sx, sy, r * 2.5f, glowPaint);
                }
            }

            canvas.DrawCircle(sx, sy, r, paint);

            // Etiqueta para estrellas muy brillantes
            if (star.Magnitude < 1.5 && !string.IsNullOrEmpty(star.CommonName))
                DrawStarLabel(canvas, sx, sy + r + 10, star.CommonName);
        }
    }

    // ── Objetos DSO ──────────────────────────────────────────────────────────

    private void DrawDsoObjects(SKCanvas canvas, float cx, float cy, float radius)
    {
        var utc = _vm.ObservationTime;
        var loc = _vm.Location;

        using var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 1.5f };
        using var textPaint = new SKPaint
        {
            IsAntialias = true,
            TextSize = 11,
            Color = new SKColor(160, 220, 255, 200)
        };

        foreach (var obj in DeepSkyCatalog.All)
        {
            var altAz = CoordinateConverter.EquatorialToHorizontal(
                obj.RightAscension, obj.Declination, utc, loc);

            if (!altAz.IsAboveHorizon) continue;
            var (sx, sy) = altAz.ToScreenXY(cx, cy, radius);

            // Color según tipo
            paint.Color = obj.Type switch
            {
                CelestialObjectType.Galaxy          => new SKColor(206, 147, 216, 200),
                CelestialObjectType.OpenCluster     => new SKColor(255, 241, 118, 200),
                CelestialObjectType.GlobularCluster => new SKColor(255, 241, 118, 200),
                CelestialObjectType.Nebula          => new SKColor(128, 222, 234, 200),
                CelestialObjectType.PlanetaryNebula => new SKColor(128, 222, 234, 200),
                _ => new SKColor(165, 214, 167, 200)
            };

            DrawDsoSymbol(canvas, paint, obj.Type, sx, sy);

            // Nombre del objeto
            canvas.DrawText($"{obj.Catalog}{obj.Number}", sx + 12, sy + 4, textPaint);
        }
    }

    private static void DrawDsoSymbol(SKCanvas canvas, SKPaint paint, CelestialObjectType type, float x, float y)
    {
        const float s = 8f;
        switch (type)
        {
            case CelestialObjectType.Galaxy:
                // Elipse inclinada
                canvas.Save();
                canvas.RotateDegrees(30, x, y);
                canvas.DrawOval(x, y, s, s * 0.4f, paint);
                canvas.Restore();
                break;
            case CelestialObjectType.OpenCluster:
                // Círculo punteado (aproximado con círculo y X)
                canvas.DrawCircle(x, y, s, paint);
                canvas.DrawLine(x - s * 0.5f, y - s * 0.5f, x + s * 0.5f, y + s * 0.5f, paint);
                canvas.DrawLine(x + s * 0.5f, y - s * 0.5f, x - s * 0.5f, y + s * 0.5f, paint);
                break;
            case CelestialObjectType.GlobularCluster:
                // Círculo con cruz
                canvas.DrawCircle(x, y, s, paint);
                canvas.DrawLine(x - s, y, x + s, y, paint);
                canvas.DrawLine(x, y - s, x, y + s, paint);
                break;
            case CelestialObjectType.Nebula:
            case CelestialObjectType.PlanetaryNebula:
                // Cuadrado
                canvas.DrawRect(x - s, y - s, s * 2, s * 2, paint);
                break;
            default:
                canvas.DrawCircle(x, y, s, paint);
                break;
        }
    }

    // ── Líneas de constelaciones (Orión de ejemplo) ──────────────────────────

    private void DrawConstellationLines(SKCanvas canvas, float cx, float cy, float radius)
    {
        // Líneas básicas de Orión (RA/Dec aproximadas)
        var orionLines = new (double ra1, double dec1, double ra2, double dec2)[]
        {
            (5.919, 7.407, 5.603, -1.202),   // Betelgeuse → Alnilam
            (5.533, -0.299, 5.603, -1.202),   // Mintaka → Alnilam
            (5.603, -1.202, 5.679, -1.943),   // Alnilam → Alnitak
            (5.242, -8.202, 5.679, -1.943),   // Rigel → Alnitak
            (5.418, 6.350, 5.919, 7.407),     // Bellatrix → Betelgeuse
        };

        var utc = _vm.ObservationTime;
        var loc = _vm.Location;

        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = _vm.IsNightMode
                ? new SKColor(60, 0, 0, 100)
                : new SKColor(100, 140, 200, 80),
            StrokeWidth = 1f,
            Style = SKPaintStyle.Stroke
        };

        foreach (var (ra1, dec1, ra2, dec2) in orionLines)
        {
            var a1 = CoordinateConverter.EquatorialToHorizontal(ra1, dec1, utc, loc);
            var a2 = CoordinateConverter.EquatorialToHorizontal(ra2, dec2, utc, loc);

            if (!a1.IsAboveHorizon || !a2.IsAboveHorizon) continue;

            var (x1, y1) = a1.ToScreenXY(cx, cy, radius);
            var (x2, y2) = a2.ToScreenXY(cx, cy, radius);
            canvas.DrawLine(x1, y1, x2, y2, paint);
        }
    }

    // ── Overlays ──────────────────────────────────────────────────────────────

    private void DrawOverlays(SKCanvas canvas, float cx, float cy, float radius)
    {
        var utc = _vm.ObservationTime;
        var loc = _vm.Location;

        foreach (var overlay in _vm.OverlayManager.GetVisible())
        {
            var altAz = CoordinateConverter.EquatorialToHorizontal(
                overlay.AnchorRA, overlay.AnchorDec, utc, loc);

            if (!altAz.IsAboveHorizon) continue;
            var (sx, sy) = altAz.ToScreenXY(cx, cy, radius);

            // Calcular tamaño en píxeles según FOV y tamaño angular
            float pxPerDeg = radius / 90f;
            float w = (float)overlay.WidthDegrees * pxPerDeg;
            float h = (float)overlay.HeightDegrees * pxPerDeg;

            // Si hay bitmap cargado, dibujarlo
            if (_overlayBitmaps.TryGetValue(overlay.FilePath, out var bmp))
            {
                using var paint = new SKPaint { IsAntialias = true };
                paint.Color = paint.Color.WithAlpha((byte)(overlay.Opacity * 255));
                var destRect = new SKRect(sx - w / 2, sy - h / 2, sx + w / 2, sy + h / 2);

                canvas.Save();
                canvas.RotateDegrees(overlay.RotationDegrees, sx, sy);
                canvas.DrawBitmap(bmp, destRect, paint);
                canvas.Restore();
            }
            else
            {
                // Placeholder visual mientras no hay imagen real
                DrawOverlayPlaceholder(canvas, sx, sy, w, h, overlay.Name, overlay.Opacity);
            }
        }
    }

    private static void DrawOverlayPlaceholder(SKCanvas canvas, float x, float y,
        float w, float h, string name, float opacity)
    {
        using var borderPaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(30, 136, 229, (byte)(opacity * 180)),
            StrokeWidth = 1.5f,
            PathEffect = SKPathEffect.CreateDash([6f, 4f], 0)
        };
        using var textPaint = new SKPaint
        {
            IsAntialias = true,
            TextSize = 12,
            Color = new SKColor(30, 136, 229, (byte)(opacity * 200))
        };

        canvas.DrawRect(x - w / 2, y - h / 2, w, h, borderPaint);
        canvas.DrawText($"📐 {name}", x - w / 2 + 4, y + 6, textPaint);
    }

    // ── UI helpers ────────────────────────────────────────────────────────────

    private void DrawStarLabel(SKCanvas canvas, float x, float y, string text)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            TextSize = 12,
            Color = _vm.IsNightMode
                ? new SKColor(120, 0, 0, 180)
                : new SKColor(180, 200, 255, 180)
        };
        canvas.DrawText(text, x, y, paint);
    }

    private void DrawCompass(SKCanvas canvas, float cx, float cy, float radius)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            TextSize = 16,
            Color = new SKColor(100, 130, 160, 160),
            TextAlign = SKTextAlign.Center
        };
        float r = radius + 20;
        canvas.DrawText("N", cx, cy - r, paint);
        canvas.DrawText("S", cx, cy + r + 16, paint);
        canvas.DrawText("E", cx + r, cy + 6, paint);
        canvas.DrawText("O", cx - r, cy + 6, paint);
    }

    private void DrawHorizonLabel(SKCanvas canvas, float cx, float cy, float radius)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(60, 100, 60, 80),
            StrokeWidth = 1f
        };
        canvas.DrawCircle(cx, cy, radius, paint);
    }

    // ── Eventos UI ────────────────────────────────────────────────────────────

    private void OnCanvasTouch(object? sender, SKTouchEventArgs e)
    {
        if (e.ActionType == SKTouchAction.Released)
        {
            var info = StarCanvas.CanvasSize;
            var obj = _vm.HitTestDso(e.Location.X, e.Location.Y, info.Width, info.Height);
            _vm.SelectObject(obj);
            StarCanvas.InvalidateSurface();
        }
        e.Handled = true;
    }

    private void OnZoomIn(object? sender, EventArgs e) => _vm.ZoomIn();
    private void OnZoomOut(object? sender, EventArgs e) => _vm.ZoomOut();

    private async void OnOverlaysClicked(object? sender, EventArgs e) =>
        await Navigation.PushAsync(
            App.Current!.Handler.MauiContext!.Services.GetRequiredService<OverlayManagerPage>());

    private async void OnObjectDetailClicked(object? sender, EventArgs e)
    {
        if (_vm.SelectedObject is null) return;
        var page = App.Current!.Handler.MauiContext!.Services.GetRequiredService<ObjectDetailPage>();
        page.BindingContext = new ObjectDetailViewModel { Object = _vm.SelectedObject };
        await Navigation.PushAsync(page);
    }
}
