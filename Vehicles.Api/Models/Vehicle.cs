using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vehicles.Api.Models
{
    public sealed class Vehicle
    {
        [Required]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [Required]
        [JsonPropertyName("make")]
        public string Make { get; set; }

        [Required]
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [Required]
        [JsonPropertyName("trim")]
        public string Trim { get; set; }

        [Required]
        [JsonPropertyName("colour")]
        public string Colour { get; set; }

        [Required]
        [JsonPropertyName("co2_level")]
        public int Co2Level { get; set; }

        [Required]
        [JsonPropertyName("transmission")]
        public string Transmission { get; set; }

        [Required]
        [JsonPropertyName("fuel_type")]
        public string FuelType { get; set; }

        [Required]
        [JsonPropertyName("engine_size")]
        public int EngineSize { get; set; }

        [Required]
        [JsonPropertyName("date_first_reg")]
        public string DateFirstRegistration { get; set; }

        [Required]
        [JsonPropertyName("mileage")]
        public int Mileage { get; set; }

        [JsonPropertyName("is_deleted")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
