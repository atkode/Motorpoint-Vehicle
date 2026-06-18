using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;
using Vehicles.Api.Models;
using Xunit;

namespace Vehicles.Api.Repositories.Tests
{
    public class VehicleRepositoryTests : IDisposable
    {
        private readonly string _tempFile;
        private readonly VehicleRepository _vehicleRepository;
        private readonly Mock<ILogger<VehicleRepository>> _loggerMock;

        public VehicleRepositoryTests()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");
            var options = Options.Create(new PathConfiguration { Vehicles = _tempFile });
            _loggerMock = new Mock<ILogger<VehicleRepository>>();
            _vehicleRepository = new VehicleRepository(options, _loggerMock.Object);
        }

        // Clean up the temporary file after each test runs
        public void Dispose()
        {
            if (File.Exists(_tempFile))
            {  
                File.Delete(_tempFile); 
            }
        }

        private static Vehicle NewVehicle(string make = "MakeA", string model = "ModelA", decimal price = 1000, string transmission = "Manual")
        {
            return new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = make,
                Model = model,
                Trim = "Trim",
                Colour = "Red",
                Co2Level = 120,
                Transmission = transmission,
                FuelType = "Petrol",
                EngineSize = 1600,
                DateFirstRegistration = "2020-01-01",
                Mileage = 10000,
                Price = price,
                IsDeleted = false
            };
        }

        private async Task WriteVehiclesFileAsync(IEnumerable<Vehicle> vehicles)
        {
            using var stream = new FileStream(_tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, vehicles);
            await stream.FlushAsync();
        }

        private async Task<List<Vehicle>> ReadVehiclesFileAsync()
        {
            using var stream = new FileStream(_tempFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await JsonSerializer.DeserializeAsync<List<Vehicle>>(stream) ?? new List<Vehicle>();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllVehiclesFromFile()
        {
            // Arrange
            var vehicles = new List<Vehicle> { NewVehicle("A"), NewVehicle("B") };
            await WriteVehiclesFileAsync(vehicles);

            // Act
            var results = (await _vehicleRepository.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Contains(results, v => v.Make == "A");
            Assert.Contains(results, v => v.Make == "B");
        }

        [Fact]
        public async Task GetMakeAsync_FiltersByMake()
        {
            // Arrange
            var vehicles = new List<Vehicle> { NewVehicle("X"), NewVehicle("X"), NewVehicle("Y") };
            await WriteVehiclesFileAsync(vehicles);

            // Act
            var results = (await _vehicleRepository.GetMakeAsync("X")).ToList();

            // Assert
            Assert.Equal(2, results.Count);
            Assert.All(results, v => Assert.Equal("X", v.Make));
        }

        [Fact]
        public async Task GetSearchAsync_FiltersByMakePriceTransmission()
        {
            // Arrange
            var match = NewVehicle(make: "Match", price: 5000, transmission: "Auto");
            var notMatchPrice = NewVehicle(make: "Match", price: 6000, transmission: "Auto");
            var notMatchTrans = NewVehicle(make: "Match", price: 4000, transmission: "Manual");
            var otherMake = NewVehicle(make: "Other", price: 3000, transmission: "Auto");

            await WriteVehiclesFileAsync(new List<Vehicle> { match, notMatchPrice, notMatchTrans, otherMake });

            var filter = new FilterSearch
            {
                Make = new List<string> { "Match" },
                MaxPrice = 5500,
                Transmission = "Auto"
            };

            // Act
            var results = (await _vehicleRepository.GetSearchAsync(filter)).ToList();

            // Assert
            Assert.Equal(match.Id, results[0].Id);
        }

        [Fact]
        public async Task CreateAsync_AddsVehicleAndPersists()
        {
            // Arrange
            var vehicle = NewVehicle("NewMake");

            // Act
            var result = await _vehicleRepository.CreateAsync(vehicle);

            // Assert
            var persisted = await ReadVehiclesFileAsync();
            Assert.Equal(vehicle.Id, persisted[0].Id);
            Assert.Equal(vehicle.Make, persisted[0].Make);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesExisting_ReturnsTrue_AndPersists()
        {
            // Arrange
            var existing = NewVehicle("OldMake");
            await WriteVehiclesFileAsync(new List<Vehicle> { existing });
            existing.Make = "UpdatedMake";

            // Act
            var result = await _vehicleRepository.UpdateAsync(existing);

            // Assert
            Assert.True(result);

            var persisted = await ReadVehiclesFileAsync();
            Assert.Single(persisted);
            Assert.Equal(existing.Make, persisted[0].Make);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            var v = NewVehicle();
            await WriteVehiclesFileAsync(Array.Empty<Vehicle>());

            // Act
            var result = await _vehicleRepository.UpdateAsync(v);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task VehiclesLoadedAsync_HandlesMissingFile_ReturnsEmpty()
        {
            // Arrange
            if (File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }

            // Act
            var all = (await _vehicleRepository.GetAllAsync()).ToList();

            // Assert
            Assert.Empty(all);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsVehicle_WhenExists()
        {
            // Arrange
            var vehicle1 = NewVehicle("Make1");
            var vehicle2 = NewVehicle("Make2");
            await WriteVehiclesFileAsync(new List<Vehicle> { vehicle1, vehicle2 });

            // Act
            var result = await _vehicleRepository.GetByIdAsync(vehicle2.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicle2.Id, result.Id);
            Assert.Equal("Make2", result.Make);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var existing = NewVehicle("Exists");
            await WriteVehiclesFileAsync(new List<Vehicle> { existing });

            // Act
            var result = await _vehicleRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}