using Microsoft.Extensions.Options;
using System.Text.Json;
using Vehicles.Api.Interfaces;
using Vehicles.Api.Models;

namespace Vehicles.Api.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly string _filePath;
        private List<Vehicle> _vehicles;
        private readonly ILogger<VehicleRepository> _logger;

        public VehicleRepository(IOptions<PathConfiguration> options, ILogger<VehicleRepository> logger)
        {
            _filePath = options.Value.Vehicles;
            _logger = logger;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            await VehiclesLoadedAsync();

            return _vehicles;
        }

        public async Task<PagedResult<Vehicle>> GetAllAsync(int pageNumber, int listSize)
        {
            await VehiclesLoadedAsync();

            var totalCount = _vehicles.Count;

            var items = _vehicles
                .Skip((pageNumber - 1) * listSize)
                .Take(listSize)
                .ToList();

            return new PagedResult<Vehicle>
            {
                Items = items,
                PageNumber = pageNumber,
                ListSize = listSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<Vehicle>> GetMakeAsync(string make)
        {
            await VehiclesLoadedAsync();

            var filteredVehicles = _vehicles.Where(vehicle => string.Equals(vehicle.Make, make, StringComparison.OrdinalIgnoreCase));

            return filteredVehicles;
        }

        public async Task<IEnumerable<Vehicle>> GetModelAsync(string model)
        {
            await VehiclesLoadedAsync();

            var filteredVehicles = _vehicles.Where(vehicle => string.Equals(vehicle.Model, model, StringComparison.OrdinalIgnoreCase));

            return filteredVehicles;
        }

        public async Task<IEnumerable<Vehicle>> GetSearchAsync(FilterSearch filter)
        {
            await VehiclesLoadedAsync();

            // Filters are not required, so can filter on any one, or all
            var filteredVehicles = _vehicles.Where(v => 
                (!filter.Make.Any() || filter.Make.Contains(v.Make)) &&
                (filter.MaxPrice == null || v.Price <= filter.MaxPrice) &&
                (string.IsNullOrWhiteSpace(filter.Transmission) || v.Transmission.Equals(filter.Transmission, StringComparison.OrdinalIgnoreCase)))
            .ToList();

            return filteredVehicles;
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
        {
            await VehiclesLoadedAsync();

            return _vehicles.FirstOrDefault(v => v.Id == id);
        }

        public async Task<Vehicle> CreateAsync(Vehicle vehicle)
        {
            await VehiclesLoadedAsync();

            vehicle.Id = Guid.NewGuid();
            _vehicles.Add(vehicle);

            await WriteVehiclesToFile();
            return vehicle;
        }

        public async Task<bool> UpdateAsync(Vehicle vehicle)
        {
            await VehiclesLoadedAsync();

            var existingVehicle = _vehicles.FirstOrDefault(v => v.Id == vehicle.Id);

            if (existingVehicle == null)
            {     
                return false;
            }

            // Get index and update
            int indexToUpdate = _vehicles.IndexOf(existingVehicle);
            _vehicles[indexToUpdate] = vehicle;

            await WriteVehiclesToFile();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await VehiclesLoadedAsync();

            var existingVehicle = _vehicles.FirstOrDefault(v => v.Id == id);

            if (existingVehicle == null)
            {
                return false;
            }

            // Soft delete
            existingVehicle.IsDeleted = true;

            await WriteVehiclesToFile();
            return true;
        }

        private async Task VehiclesLoadedAsync()
        {
            if (_vehicles != null)
            {
                return;
            }

            if (!File.Exists(_filePath))
            {
                _vehicles = new List<Vehicle>();
                return;
            }

            using (var json = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                _vehicles = await JsonSerializer.DeserializeAsync<List<Vehicle>>(json) ?? new List<Vehicle>();
            }

            _vehicles = _vehicles.Where(vehicle => !vehicle.IsDeleted).ToList();
        }

        private async Task WriteVehiclesToFile()
        {
            try
            {
                using (var json = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await JsonSerializer.SerializeAsync(json, _vehicles);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when writing vehicles file");
            }
        }
    }
}
