namespace TubkalaStars.Services;

public interface IGyroscopeService
{
    bool IsAvailable { get; }
    void Start();
    void Stop();
    event Action<double, double, double>? OrientationChanged; // azimuth, pitch, roll
}

public interface IOrientationService
{
    double AzimuthDegrees { get; }
    double AltitudeDegrees { get; }
    bool IsTracking { get; }
    void StartTracking();
    void StopTracking();
    event Action? Changed;
}

/// <summary>
/// Servicio de orientación usando los sensores de MAUI.
/// Combina giroscopio + acelerómetro para azimut y altitud del dispositivo.
/// </summary>
public class OrientationService : IOrientationService, IGyroscopeService
{
    public double AzimuthDegrees { get; private set; }
    public double AltitudeDegrees { get; private set; }
    public bool IsTracking { get; private set; }
    public bool IsAvailable => Accelerometer.Default.IsSupported;

    public event Action? Changed;
    public event Action<double, double, double>? OrientationChanged;

    public void StartTracking()
    {
        if (!IsTracking && Accelerometer.Default.IsSupported)
        {
            Accelerometer.Default.ReadingChanged += OnAccelerometerChanged;
            Accelerometer.Default.Start(SensorSpeed.UI);

            if (Compass.Default.IsSupported)
            {
                Compass.Default.ReadingChanged += OnCompassChanged;
                Compass.Default.Start(SensorSpeed.UI);
            }

            IsTracking = true;
        }
    }

    public void StopTracking()
    {
        if (IsTracking)
        {
            Accelerometer.Default.Stop();
            Accelerometer.Default.ReadingChanged -= OnAccelerometerChanged;

            if (Compass.Default.IsSupported)
            {
                Compass.Default.Stop();
                Compass.Default.ReadingChanged -= OnCompassChanged;
            }

            IsTracking = false;
        }
    }

    public void Start() => StartTracking();
    public void Stop() => StopTracking();

    private void OnAccelerometerChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var v = e.Reading.Acceleration;
        // Calcular ángulo de inclinación (altitud del dispositivo)
        double pitch = Math.Atan2(v.Y, Math.Sqrt(v.X * v.X + v.Z * v.Z)) * 180.0 / Math.PI;
        AltitudeDegrees = pitch;
        Changed?.Invoke();
        OrientationChanged?.Invoke(AzimuthDegrees, AltitudeDegrees, 0);
    }

    private void OnCompassChanged(object? sender, CompassChangedEventArgs e)
    {
        AzimuthDegrees = e.Reading.HeadingMagneticNorth;
        Changed?.Invoke();
    }
}
