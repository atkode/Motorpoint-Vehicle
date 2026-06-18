using Microsoft.AspNetCore.Mvc;
using Vehicles.Api.Interfaces;
using Vehicles.Api.Models;

namespace Vehicles.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly ILogger<VehicleController> _logger;
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleController(ILogger<VehicleController> logger, IVehicleRepository vehicleRepository)
        {
            _logger = logger;
            _vehicleRepository = vehicleRepository;
        }

        [HttpGet]
        [Route("get-all")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetAllAsync()
        {
            var results = await _vehicleRepository.GetAllAsync();

            if (results == null || !results.Any())
            {
                // Logging as ERROR as this should not happen on get-all
                _logger.LogError("No vehicle results found for GET-ALL. DATETIME: {datetime}", DateTime.UtcNow.ToString());
                return NoContent();
            }

            return Ok(results);
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Vehicle>> GetByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);

            if (vehicle == null)
            {
                // Logging as ERROR as this should not happen as Id must've been passed as param from UI so should exist?
                _logger.LogError("No vehicle results found for get-id. Id: {id}. DATETIME: {datetime}", id, DateTime.UtcNow.ToString());
                return NotFound();
            }

            return Ok(vehicle);
        }

        [HttpGet]
        [Route("get-all/{pageNumber}/{listSize}")]
        public async Task<ActionResult<PagedResult<Vehicle>>> GetAllAsync(int pageNumber, int listSize)
        {
            if (pageNumber < 1 || listSize < 1)
            {
                return BadRequest(new ArgumentOutOfRangeException());
            }

            var results = await _vehicleRepository.GetAllAsync(pageNumber, listSize);

            if (results == null || results.TotalCount == 0)
            {
                return NotFound();
            }

            return Ok(results);
        }

        [HttpGet]
        [Route("get-make/{make}")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetMakeAsync(string make)
        {
            if (string.IsNullOrWhiteSpace(make))
            {
                return BadRequest("Make is required");
            }

            // I will assume 'make' is from a select components on the UI
            var results = await _vehicleRepository.GetMakeAsync(make);

            if (results is null || !results.Any())
            {
                // Log as WARNING as maybe our MAKE select in UI is displaying makes that we don't have? Not actual error
                _logger.LogWarning("No vehicle results found MAKE: {make}. DATETIME: {datetime}", make, DateTime.UtcNow.ToString());
                return NotFound();
            }

            return Ok(results);
        }

        [HttpGet]
        [Route("get-model/{model}")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetModelAsync(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                return BadRequest("Model is required");
            }

            // I will assume 'model' is from a select components on the UI
            var results = await _vehicleRepository.GetModelAsync(model);

            if (results is null || !results.Any())
            {
                // Log as WARNING as maybe our MODEL select in UI is displaying models that we don't have? Not actual error
                _logger.LogWarning("No vehicle results found MODEL: {make}. DATETIME: {datetime}", model, DateTime.UtcNow.ToString());
                return NotFound();
            }

            return Ok(results);
        }

        [HttpPost]
        [Route("get-search")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetSearchAsync([FromBody] FilterSearch filter)
        {
            if (filter == null)
            {
                return BadRequest("Filter is required in the request body.");
            }

            // I will assume all param values are from select components on the UI
            var results = await _vehicleRepository.GetSearchAsync(filter);

            if (results is null || !results.Any())
            {
                // Log depending on UI implementation. Possible filters may not produce a match
                _logger.LogWarning("No vehicle results found for filter search. DATETIME: {datetime}", DateTime.UtcNow.ToString());
                return NotFound(new List<Vehicle>());
            }

            return Ok(results);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Vehicle>> CreateAsync([FromBody]Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return BadRequest(new Vehicle());
            }

            var created = await _vehicleRepository.CreateAsync(vehicle);

            if (created == null)
            {
                _logger.LogError("Unable to create new vehicle. DATETIME: {datetime}", DateTime.UtcNow.ToString());
                return NotFound(new Vehicle());
            }

            return Ok(vehicle);
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult<bool>> UpdateAsync([FromBody] Vehicle vehicle)
        {
            var isSuccessful = await _vehicleRepository.UpdateAsync(vehicle);

            if (!isSuccessful)
            {
                _logger.LogError("Failed to update vehicle ID: {id}. DATETIME: {datetime}", vehicle.Id, DateTime.UtcNow.ToString());
                NotFound(isSuccessful);
            }

            return Ok(isSuccessful);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(Guid id)
        {
            var isSuccessful = await _vehicleRepository.DeleteAsync(id);

            if (!isSuccessful)
            {
                _logger.LogError("Unable to delete vehicle. Id: {id}. DATETIME: {datetime}", id, DateTime.UtcNow.ToString());
                return NotFound(isSuccessful);
            }

            return Ok(isSuccessful);
        }
    }
}