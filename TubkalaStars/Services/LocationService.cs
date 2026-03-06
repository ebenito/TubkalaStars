using TubkalaStars.Core.Models;

namespace TubkalaStars.Services;

public class LocationService : ILocationService
{
    private ObserverLocation _last = ObserverLocation.ColladoVillalba;

    public ObserverLocation LastKnownLocation => _last;

    public async Task<ObserverLocation> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) return _last;

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location != null)
            {
                _last = new ObserverLocation(
                    location.Latitude,
                    location.Longitude,
                    location.Altitude ?? 0,
                    "Mi ubicación"
                );
            }
        }
        catch (FeatureNotSupportedException)
        {
            // GPS no disponible en este dispositivo
        }
        catch (PermissionException)
        {
            // Sin permisos, usar ubicación por defecto
        }

        return _last;
    }
}
