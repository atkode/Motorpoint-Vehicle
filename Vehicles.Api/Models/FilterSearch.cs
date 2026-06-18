using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Vehicles.Api.Models
{
    public sealed class FilterSearch
    {
        [JsonPropertyName("make")]
        [DefaultValue(new[] { "BMW" })]
        public List<string> Make { get; set; }

        [JsonPropertyName("max_price")]
        [AllowNull]
        [DefaultValue(null)]
        public Decimal? MaxPrice { get; set; }

        [JsonPropertyName("transmission")]
        [AllowNull]
        [DefaultValue(null)]
        public string? Transmission { get; set; }
    }
}
