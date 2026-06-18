using Vehicles.Api.Models;

namespace Vehicles.Api.Interfaces
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();

        // Realistically I would have paginated all GET requests
        Task<PagedResult<Vehicle>> GetAllAsync(int pageNumber, int listSize);

        Task<IEnumerable<Vehicle>> GetMakeAsync(string name);

        Task<IEnumerable<Vehicle>> GetModelAsync(string model);

        Task<Vehicle?> GetByIdAsync(Guid id);

        Task<IEnumerable<Vehicle>> GetSearchAsync(FilterSearch filter);

        Task<Vehicle> CreateAsync(Vehicle vehicle);

        Task<bool> UpdateAsync(Vehicle vehicle);

        Task<bool> DeleteAsync(Guid id);
    }
}
