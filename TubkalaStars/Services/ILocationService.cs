using TubkalaStars.Core.Models;

namespace TubkalaStars.Services;

public interface ILocationService
{
    Task<ObserverLocation> GetCurrentLocationAsync();
    ObserverLocation LastKnownLocation { get; }
}
